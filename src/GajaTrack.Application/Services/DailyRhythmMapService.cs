using GajaTrack.Application.DTOs.DailyRhythmMap;
using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;
using GajaTrack.Domain.Services;

namespace GajaTrack.Application.Services;

public class DailyRhythmMapService(ITrackingRepository repository, DailyRhythmMapDomainService domainService) : IDailyRhythmMapService
{
    public async Task<List<DailyRhythmMapDay>> GetDailyRhythmMapAsync(DateOnly startDate, DateOnly endDate, bool mostRecentFirst = false, TimeZoneInfo? timeZone = null, CancellationToken cancellationToken = default)
    {
        timeZone ??= TimeZoneInfo.Local;

        // 1. Determine Fetch Range (Buffer to catch overlapping events)
        var fetchStart = startDate.AddDays(-1).ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);
        var fetchEnd = endDate.AddDays(2).ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);

        var utcStart = UtcDateTime.FromDateTime(fetchStart);
        var utcEnd = UtcDateTime.FromDateTime(fetchEnd);

        // 2. Fetch Data
        var sleepTask = repository.GetSleepSessionsInRangeAsync(utcStart, utcEnd, cancellationToken);
        var nursingTask = repository.GetNursingFeedsInRangeAsync(utcStart, utcEnd, cancellationToken);
        var bottleTask = repository.GetBottleFeedsInRangeAsync(utcStart, utcEnd, cancellationToken);
        var cryingTask = repository.GetCryingSessionsInRangeAsync(utcStart, utcEnd, cancellationToken);
        var diaperTask = repository.GetDiaperChangesInRangeAsync(utcStart, utcEnd, cancellationToken);

        await Task.WhenAll(sleepTask, nursingTask, bottleTask, cryingTask, diaperTask);

        var result = new List<DailyRhythmMapDay>();
        
        // 3. Iterate Days
        for (var d = startDate; d <= endDate; d = d.AddDays(1))
        {
            var window = domainService.GetDailyRhythmMapWindow(d, timeZone);
            var dayEvents = new List<DailyRhythmMapEvent>();

            // Process Sleep
            foreach (var s in sleepTask.Result)
            {
                var intersection = domainService.GetIntersection(window, s.StartTime, s.EndTime);
                if (intersection.HasValue)
                {
                    var displayTime = TimeZoneInfo.ConvertTimeFromUtc(s.StartTime, timeZone);
                    dayEvents.Add(DailyRhythmMapEvent.Create(s.Id, DailyRhythmMapEventType.Sleep, displayTime, window.Start, intersection.Value.Start, intersection.Value.End));
                }
            }
            
            var totalSleep = dayEvents
                .Where(e => e.Type == DailyRhythmMapEventType.Sleep)
                .Sum(e => e.DurationMinutes);

            var sleepEvents = dayEvents
                .Where(e => e.Type == DailyRhythmMapEventType.Sleep)
                .OrderBy(e => e.StartMinute)
                .ToList();

            int nightWakingCount = 0;
            for (int i = 0; i < sleepEvents.Count; i++)
            {
                var ev = sleepEvents[i];
                var endMin = ev.StartMinute + ev.DurationMinutes;

                // Night window is 18:00 (720m) to 06:00 (1440m)
                // We count interruptions: wake-ups that happen during the night, excluding the last wake-up of the day.
                if (endMin >= 720 && endMin < 1440 && i < sleepEvents.Count - 1)
                {
                    nightWakingCount++;
                }
            }

            // Process Crying
            foreach (var c in cryingTask.Result)
            {
                var intersection = domainService.GetIntersection(window, c.StartTime, c.EndTime);
                if (intersection.HasValue)
                {
                    var displayTime = TimeZoneInfo.ConvertTimeFromUtc(c.StartTime, timeZone);
                    dayEvents.Add(DailyRhythmMapEvent.Create(c.Id, DailyRhythmMapEventType.Crying, displayTime, window.Start, intersection.Value.Start, intersection.Value.End));
                }
            }

            // Process Nursing (Point)
            foreach (var n in nursingTask.Result)
            {
                if (domainService.IsInWindow(window, n.StartTime))
                {
                     var displayTime = TimeZoneInfo.ConvertTimeFromUtc(n.StartTime, timeZone);
                     dayEvents.Add(DailyRhythmMapEvent.Create(n.Id, DailyRhythmMapEventType.Nursing, displayTime, window.Start, n.StartTime, n.StartTime));
                }
            }
            
            // Process Bottle (Point)
            foreach (var b in bottleTask.Result)
            {
                if (domainService.IsInWindow(window, b.Time))
                {
                     var displayTime = TimeZoneInfo.ConvertTimeFromUtc(b.Time, timeZone);
                     var type = b.Content == BottleContent.Formula ? DailyRhythmMapEventType.BottleFormula : DailyRhythmMapEventType.BottleMilk;
                     dayEvents.Add(DailyRhythmMapEvent.Create(b.Id, type, displayTime, window.Start, b.Time, b.Time, $"{b.AmountMl}ml"));
                }
            }

            // Process Diaper (Point)
            foreach (var di in diaperTask.Result)
            {
                if (domainService.IsInWindow(window, di.Time))
                {
                    var displayTime = TimeZoneInfo.ConvertTimeFromUtc(di.Time, timeZone);
                    dayEvents.Add(DailyRhythmMapEvent.Create(di.Id, DailyRhythmMapEventType.Diaper, displayTime, window.Start, di.Time, di.Time, di.Type.ToString()));
                }
            }
            result.Add(new DailyRhythmMapDay(d, window.Start, window.End, dayEvents.OrderBy(x => x.StartMinute).ToList(), new DailyRhythmMapSummary(totalSleep, nightWakingCount)));
        }

        if (mostRecentFirst)
        {
            result.Reverse();
        }

        return result;
    }
}

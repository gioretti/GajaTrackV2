using GajaTrack.Application.DTOs.DailyRhythmMap;
using GajaTrack.Application.Interfaces;
using GajaTrack.Application.Queries;
using GajaTrack.Domain;
using GajaTrack.Domain.Entities;
using GajaTrack.Domain.Services;

namespace GajaTrack.Application.Services;

public class DailyRhythmMapService(
    IGetBabyDayQuery getBabyDayQuery,
    ICalculateSleep calculateSleep,
    ICountWakings countWakings,
    DailyRhythmMapDomainService domainService) : IDailyRhythmMapService
{
    public async Task<List<DailyRhythmMapDay>> GetDailyRhythmMapAsync(DateOnly startDate, DateOnly endDate, bool mostRecentFirst = false, TimeZoneInfo? timeZone = null, CancellationToken cancellationToken = default)
    {
        timeZone ??= TimeZoneInfo.Local;

        // 1. Fetch BabyDay Domain Objects
        var babyDays = await getBabyDayQuery.ExecuteAsync(startDate, endDate, timeZone, cancellationToken);

        var result = new List<DailyRhythmMapDay>();

        // 2. Map Domain Objects to DTOs
        foreach (var day in babyDays)
        {
            var dayEvents = new List<DailyRhythmMapEvent>();

            // Process Sleep (Interval)
            foreach (var s in day.SleepSessions)
            {
                var sessionRange = new TimeRange(s.StartTime, s.EndTime ?? UtcDateTime.Now());
                var intersection = day.Window.GetIntersection(sessionRange);
                if (intersection.HasValue)
                {
                    var displayTime = TimeZoneInfo.ConvertTimeFromUtc(s.StartTime, timeZone);
                    dayEvents.Add(DailyRhythmMapEvent.Create(s.Id, DailyRhythmMapEventType.Sleep, displayTime, day.Window.Start, intersection.Value.Start, intersection.Value.End));
                }
            }

            // Process Crying (Interval)
            foreach (var c in day.CryingSessions)
            {
                var sessionRange = new TimeRange(c.StartTime, c.EndTime ?? UtcDateTime.Now());
                var intersection = day.Window.GetIntersection(sessionRange);
                if (intersection.HasValue)
                {
                    var displayTime = TimeZoneInfo.ConvertTimeFromUtc(c.StartTime, timeZone);
                    dayEvents.Add(DailyRhythmMapEvent.Create(c.Id, DailyRhythmMapEventType.Crying, displayTime, day.Window.Start, intersection.Value.Start, intersection.Value.End));
                }
            }

            // Process Nursing (Point)
            foreach (var n in day.NursingFeeds)
            {
                var displayTime = TimeZoneInfo.ConvertTimeFromUtc(n.StartTime, timeZone);
                dayEvents.Add(DailyRhythmMapEvent.Create(n.Id, DailyRhythmMapEventType.Nursing, displayTime, day.Window.Start, n.StartTime, n.StartTime));
            }

            // Process Bottle (Point)
            foreach (var b in day.BottleFeeds)
            {
                var displayTime = TimeZoneInfo.ConvertTimeFromUtc(b.Time, timeZone);
                var type = b.Content == BottleContent.Formula ? DailyRhythmMapEventType.BottleFormula : DailyRhythmMapEventType.BottleMilk;
                dayEvents.Add(DailyRhythmMapEvent.Create(b.Id, type, displayTime, day.Window.Start, b.Time, b.Time, $"{b.AmountMl}ml"));
            }

            // Process Diaper (Point)
            foreach (var di in day.DiaperChanges)
            {
                var displayTime = TimeZoneInfo.ConvertTimeFromUtc(di.Time, timeZone);
                dayEvents.Add(DailyRhythmMapEvent.Create(di.Id, DailyRhythmMapEventType.Diaper, displayTime, day.Window.Start, di.Time, di.Time, di.Type.ToString()));
            }

            // 3. Calculate Summaries using Domain Services
            var totalSleep = calculateSleep.For(day);
            var nightWakings = countWakings.For(day, new TimeOnly(18, 0), new TimeOnly(6, 0));

            result.Add(new DailyRhythmMapDay(
                day.Date,
                day.Window.Start,
                day.Window.End,
                dayEvents.OrderBy(x => x.StartMinute).ToList(),
                new DailyRhythmMapSummary(totalSleep, nightWakings)
            ));
        }

        if (mostRecentFirst)
        {
            result.Reverse();
        }

        return result;
    }
}

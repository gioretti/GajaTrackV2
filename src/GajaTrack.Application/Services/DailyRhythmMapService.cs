using GajaTrack.Application.DTOs.DailyRhythmMap;
using GajaTrack.Application.Interfaces;
using GajaTrack.Application.Queries;
using GajaTrack.Domain;
using GajaTrack.Domain.Entities;

namespace GajaTrack.Application.Services;

public class DailyRhythmMapService(
    GetBabyDay.Execution getBabyDayExecution,
    CalculateSleep calculateSleep,
    CountWakings countWakings) : IDailyRhythmMapService
{
    public async Task<List<DailyRhythmMapDay>> GetDailyRhythmMapAsync(DateOnly startDate, DateOnly endDate, bool mostRecentFirst = false, TimeZoneInfo? timeZone = null, CancellationToken cancellationToken = default)
    {
        timeZone ??= TimeZoneInfo.Local;

        // 1. Fetch BabyDay Domain Objects
        var query = new GetBabyDay.Query(startDate, endDate, timeZone);
        var babyDays = await getBabyDayExecution.RunAsync(query, cancellationToken);

        var result = new List<DailyRhythmMapDay>();

        // 2. Map Domain Objects to DTOs
        foreach (var day in babyDays)
        {
            var dayEvents = new List<DailyRhythmMapEvent>();

            // Process Sleep (Interval)
            foreach (var sleep in day.SleepSessions)
            {
                var sessionRange = new TimeRange(sleep.StartTime, sleep.EndTime ?? UtcDateTime.Now());
                var intersection = day.TimeBounds.GetIntersection(sessionRange);
                if (intersection.HasValue)
                {
                    dayEvents.Add(DailyRhythmMapEvent.Create(
                        sleep.Id, 
                        DailyRhythmMapEventType.Sleep, 
                        sleep.StartTime.Value, 
                        day.TimeBounds.Start, 
                        intersection.Value.Start, 
                        intersection.Value.End)); 
                }
            }

            // Process Crying (Interval)
            foreach (var crying in day.CryingSessions)
            {
                var sessionRange = new TimeRange(crying.StartTime, crying.EndTime ?? UtcDateTime.Now());
                var intersection = day.TimeBounds.GetIntersection(sessionRange);
                if (intersection.HasValue)
                {
                    dayEvents.Add(DailyRhythmMapEvent.Create(
                        crying.Id, 
                        DailyRhythmMapEventType.Crying, 
                        crying.StartTime.Value, 
                        day.TimeBounds.Start, 
                        intersection.Value.Start, 
                        intersection.Value.End));
                }
            }

            // Process Nursing (Point)
            foreach (var nursing in day.NursingFeeds)
            {
                dayEvents.Add(DailyRhythmMapEvent.Create(
                    nursing.Id, 
                    DailyRhythmMapEventType.Nursing, 
                    nursing.StartTime.Value, 
                    day.TimeBounds.Start, 
                    nursing.StartTime, 
                    nursing.StartTime));
            }

            // Process Bottle (Point)
            foreach (var bottle in day.BottleFeeds)
            {
                var type = bottle.Content == BottleContent.Formula ? DailyRhythmMapEventType.BottleFormula : DailyRhythmMapEventType.BottleMilk;
                dayEvents.Add(DailyRhythmMapEvent.Create(
                    bottle.Id, 
                    type, 
                    bottle.Time.Value, 
                    day.TimeBounds.Start, 
                    bottle.Time, 
                    bottle.Time, 
                    $"{bottle.AmountMl}ml"));
            }

            // Process Diaper (Point)
            foreach (var diaper in day.DiaperChanges)
            {
                dayEvents.Add(DailyRhythmMapEvent.Create(
                    diaper.Id, 
                    DailyRhythmMapEventType.Diaper, 
                    diaper.Time.Value, 
                    day.TimeBounds.Start, 
                    diaper.Time, 
                    diaper.Time, 
                    diaper.Type.ToString()));
            }

            // 3. Calculate Summaries using Domain Services
            var totalSleep = calculateSleep.For(day);
            var nightWakings = countWakings.For(day, new TimeOnly(18, 0), new TimeOnly(6, 0), timeZone);

            // Calculate Date from TimeBounds.Start (Port responsibility)
            var localStart = TimeZoneInfo.ConvertTimeFromUtc(day.TimeBounds.Start, timeZone);
            var date = DateOnly.FromDateTime(localStart);

            result.Add(new DailyRhythmMapDay(
                date,
                day.TimeBounds.Start,
                day.TimeBounds.End,
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

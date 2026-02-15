using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain;

public record BabyDay(
    TimeRange TimeBounds,
    IEnumerable<SleepSession> SleepSessions,
    IEnumerable<CryingSession> CryingSessions,
    IEnumerable<NursingFeed> NursingFeeds,
    IEnumerable<BottleFeed> BottleFeeds,
    IEnumerable<DiaperChange> DiaperChanges)
{
    public const int DayStartHour = 6;

    public static BabyDay Create(
        DateOnly date, 
        TimeZoneInfo timeZone,
        IEnumerable<SleepSession> sleepCandidates,
        IEnumerable<CryingSession> cryingCandidates,
        IEnumerable<NursingFeed> nursingCandidates,
        IEnumerable<BottleFeed> bottleCandidates,
        IEnumerable<DiaperChange> diaperCandidates)
    {
        var timeBounds = CalculateTimeBounds(date, timeZone);

        return new BabyDay(
            timeBounds,
            sleepCandidates.Where(sleep => IsIntersecting(timeBounds, sleep.StartTime, sleep.EndTime)).ToList(),
            cryingCandidates.Where(crying => IsIntersecting(timeBounds, crying.StartTime, crying.EndTime)).ToList(),
            nursingCandidates.Where(nursing => IsInBounds(timeBounds, nursing.StartTime)).ToList(),
            bottleCandidates.Where(bottle => IsInBounds(timeBounds, bottle.Time)).ToList(),
            diaperCandidates.Where(diaper => IsInBounds(timeBounds, diaper.Time)).ToList()
        );
    }

    public static TimeRange CalculateTimeBounds(DateOnly date, TimeZoneInfo timeZone)
    {
        var localStart = new DateTime(date.Year, date.Month, date.Day, DayStartHour, 0, 0, DateTimeKind.Unspecified);
        var windowStartUtc = UtcDateTime.FromDateTime(TimeZoneInfo.ConvertTimeToUtc(localStart, timeZone));
        var windowEndUtc = UtcDateTime.FromDateTime(windowStartUtc.Value.AddDays(1));
        
        return new TimeRange(windowStartUtc, windowEndUtc);
    }

    private static bool IsIntersecting(TimeRange timeBounds, UtcDateTime start, UtcDateTime? end)
    {
        var range = new TimeRange(start, end ?? UtcDateTime.Now());
        return timeBounds.Overlaps(range);
    }

    private static bool IsInBounds(TimeRange timeBounds, UtcDateTime time)
    {
        return time.Value >= timeBounds.Start.Value && time.Value < timeBounds.End.Value;
    }
}

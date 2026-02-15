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
        IEnumerable<SleepSession> allSleep,
        IEnumerable<CryingSession> allCrying,
        IEnumerable<NursingFeed> allNursing,
        IEnumerable<BottleFeed> allBottle,
        IEnumerable<DiaperChange> allDiapers)
    {
        var timeBounds = CalculateTimeBounds(date, timeZone);

        return new BabyDay(
            timeBounds,
            allSleep.Where(sleep => IsIntersecting(timeBounds, sleep.StartTime, sleep.EndTime)).ToList(),
            allCrying.Where(crying => IsIntersecting(timeBounds, crying.StartTime, crying.EndTime)).ToList(),
            allNursing.Where(nursing => IsInBounds(timeBounds, nursing.StartTime)).ToList(),
            allBottle.Where(bottle => IsInBounds(timeBounds, bottle.Time)).ToList(),
            allDiapers.Where(diaper => IsInBounds(timeBounds, diaper.Time)).ToList()
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

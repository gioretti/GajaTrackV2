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
    public const int DayTimeStart = 6;
    public const int NightTimeStart = 18;

    public static TimeRange CalculateTimeBounds(DateOnly date, TimeZoneInfo timeZone)
    {
        var localStart = new DateTime(date.Year, date.Month, date.Day, DayTimeStart, 0, 0, DateTimeKind.Unspecified);
        var windowStartUtc = UtcDateTime.FromDateTime(TimeZoneInfo.ConvertTimeToUtc(localStart, timeZone));
        var windowEndUtc = UtcDateTime.FromDateTime(windowStartUtc.Value.AddDays(1));
        
        return new TimeRange(windowStartUtc, windowEndUtc);
    }

    public static bool IsIntersecting(TimeRange timeBounds, UtcDateTime start, UtcDateTime? end)
    {
        var range = new TimeRange(start, end ?? UtcDateTime.Now());
        return timeBounds.Overlaps(range);
    }

    public static bool IsInTimeBounds(TimeRange timeBounds, UtcDateTime time)
    {
        return time.Value >= timeBounds.Start.Value && time.Value < timeBounds.End.Value;
    }
}

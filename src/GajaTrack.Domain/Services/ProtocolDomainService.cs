using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain.Services;

public class ProtocolDomainService
{
    private const int ProtocolDayStartHour = 6;

    public TimeRange GetProtocolWindow(DateOnly date, TimeZoneInfo timeZone)
    {
        var localStart = new DateTime(date.Year, date.Month, date.Day, ProtocolDayStartHour, 0, 0, DateTimeKind.Unspecified);
        var windowStartUtc = UtcDateTime.FromDateTime(TimeZoneInfo.ConvertTimeToUtc(localStart, timeZone));
        var windowEndUtc = UtcDateTime.FromDateTime(windowStartUtc.Value.AddDays(1));
        
        return new TimeRange(windowStartUtc, windowEndUtc);
    }

    public TimeRange? GetIntersection(TimeRange window, UtcDateTime eventStart, UtcDateTime? eventEnd)
    {
        var eventRange = new TimeRange(eventStart, eventEnd ?? UtcDateTime.Now());
        return window.GetIntersection(eventRange);
    }

    public bool IsInWindow(TimeRange window, UtcDateTime eventTime)
    {
        return eventTime.Value >= window.Start.Value && eventTime.Value < window.End.Value;
    }
}

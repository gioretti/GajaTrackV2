using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain;

public class CountWakings
{
    public int For(BabyDay day, TimeOnly from, TimeOnly to, TimeZoneInfo timeZone)
    {
        var sleepSessions = day.SleepSessions
            .OrderBy(session => session.StartTime.Value)
            .ToList();

        if (sleepSessions.Count <= 1)
        {
            return 0;
        }

        // We count interruptions (sessions that END within the night range), excluding the final wake-up of the day.
        return sleepSessions
            .Take(sleepSessions.Count - 1)
            .Where(session => session.EndTime.HasValue)
            .Select(session => TimeOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(session.EndTime!.Value, timeZone)))
            .Count(localEndTime => IsTimeInWindow(localEndTime, from, to));
    }

    private static bool IsTimeInWindow(TimeOnly time, TimeOnly from, TimeOnly to)
    {
        if (from <= to)
        {
            return time >= from && time <= to;
        }
        
        // Overnight range (e.g., 18:00 to 06:00)
        return time >= from || time <= to;
    }
}

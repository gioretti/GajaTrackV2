using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain;

public class CalculateSleep
{
    public (double NapsMinutes, double NightSleepMinutes) For(BabyDay day, TimeZoneInfo timeZone)
    {
        double napsMinutes = 0;
        double nightSleepMinutes = 0;

        foreach (var session in day.SleepSessions)
        {
            var sessionRange = new TimeRange(session.StartTime, session.EndTime ?? UtcDateTime.Now());
            
            // Start-Time Attribution Rule:
            // 1. Must start within the logical day bounds [Start, End)
            if (session.StartTime.Value < day.TimeBounds.Start.Value || session.StartTime.Value >= day.TimeBounds.End.Value)
            {
                continue;
            }

            // 2. Determine category based on local start time
            var localStartTime = TimeOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(session.StartTime.Value, timeZone));
            
            // Naps: 06:00 to 18:00
            // Night: 18:00 to 06:00 (Next day)
            if (IsInNapWindow(localStartTime))
            {
                napsMinutes += sessionRange.TotalMinutes;
            }
            else
            {
                nightSleepMinutes += sessionRange.TotalMinutes;
            }
        }

        return (napsMinutes, nightSleepMinutes);
    }

    private static bool IsInNapWindow(TimeOnly time)
    {
        return time >= new TimeOnly(6, 0) && time < new TimeOnly(18, 0);
    }
}

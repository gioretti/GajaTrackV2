using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain;

public class CalculateSleep
{
    public record Result(double NapsMinutes, double NightSleepMinutes);

    public Result For(BabyDay day, TimeZoneInfo timeZone)
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
            if (IsDayTime(localStartTime))
            {
                napsMinutes += sessionRange.TotalMinutes;
            }
            else
            {
                nightSleepMinutes += sessionRange.TotalMinutes;
            }
        }

        return new Result(napsMinutes, nightSleepMinutes);
    }

    private static bool IsDayTime(TimeOnly time)
    {
        return time >= new TimeOnly(BabyDay.DayTimeStart, 0) && time < new TimeOnly(BabyDay.NightTimeStart, 0);
    }
}

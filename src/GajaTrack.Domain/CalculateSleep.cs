using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain;

public class CalculateSleep
{
    public double For(BabyDay day)
    {
        double totalMinutes = 0;

        foreach (var session in day.SleepSessions)
        {
            var sessionRange = new TimeRange(session.StartTime, session.EndTime ?? UtcDateTime.Now());
            var intersection = day.TimeBounds.GetIntersection(sessionRange);

            if (intersection.HasValue)
            {
                totalMinutes += intersection.Value.TotalMinutes;
            }
        }

        return totalMinutes;
    }
}

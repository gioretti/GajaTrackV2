using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain;

public interface ICalculateSleep
{
    double For(BabyDay day);
}

public class CalculateSleep : ICalculateSleep
{
    public double For(BabyDay day)
    {
        double totalMinutes = 0;

        foreach (var session in day.SleepSessions)
        {
            var sessionRange = new TimeRange(session.StartTime, session.EndTime ?? UtcDateTime.Now());
            var intersection = day.Window.GetIntersection(sessionRange);

            if (intersection.HasValue)
            {
                totalMinutes += intersection.Value.TotalMinutes;
            }
        }

        return totalMinutes;
    }
}

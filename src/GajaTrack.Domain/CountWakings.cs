using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain;

public interface ICountWakings
{
    int For(BabyDay day, TimeOnly from, TimeOnly to);
}

public class CountWakings : ICountWakings
{
    public int For(BabyDay day, TimeOnly from, TimeOnly to)
    {
        var sleepSessions = day.SleepSessions
            .OrderBy(s => s.StartTime.Value)
            .ToList();

        if (sleepSessions.Count <= 1) return 0;

        int count = 0;
        // Night window logic (e.g., 18:00 to 06:00)
        // We count sessions that END within this window, excluding the last one.
        for (int i = 0; i < sleepSessions.Count - 1; i++)
        {
            var session = sleepSessions[i];
            if (!session.EndTime.HasValue) continue;

            // Convert UTC EndTime to Local Time for checking the TimeOnly window
            var localEnd = TimeOnly.FromDateTime(session.EndTime.Value.Value.ToLocalTime());

            if (IsTimeInWindow(localEnd, from, to))
            {
                count++;
            }
        }

        return count;
    }

    private bool IsTimeInWindow(TimeOnly time, TimeOnly from, TimeOnly to)
    {
        if (from <= to)
        {
            // Simple range (e.g., 08:00 to 12:00)
            return time >= from && time <= to;
        }
        else
        {
            // Overnight range (e.g., 18:00 to 06:00)
            return time >= from || time <= to;
        }
    }
}

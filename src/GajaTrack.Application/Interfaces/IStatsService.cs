namespace GajaTrack.Application.Interfaces;

public record TrackingStats(
    int NursingCount,
    int BottleCount,
    int SleepCount,
    int DiaperCount,
    int CryingCount
);

public interface IStatsService
{
    Task<TrackingStats> GetStatsAsync(CancellationToken cancellationToken = default);
}

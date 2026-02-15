using GajaTrack.Application.DTOs.Stats;

namespace GajaTrack.Application.Interfaces;

public interface IStatsService
{
    Task<TrackingStats> GetStatsAsync(CancellationToken cancellationToken = default);
}

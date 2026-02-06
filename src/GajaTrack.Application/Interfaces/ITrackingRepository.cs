using GajaTrack.Domain.Entities;

namespace GajaTrack.Application.Interfaces;

public interface ITrackingRepository
{
    Task<List<SleepSession>> GetSleepSessionsInRangeAsync(UtcDateTime start, UtcDateTime end, CancellationToken ct = default);
    Task<List<NursingFeed>> GetNursingFeedsInRangeAsync(UtcDateTime start, UtcDateTime end, CancellationToken ct = default);
    Task<List<BottleFeed>> GetBottleFeedsInRangeAsync(UtcDateTime start, UtcDateTime end, CancellationToken ct = default);
    Task<List<CryingSession>> GetCryingSessionsInRangeAsync(UtcDateTime start, UtcDateTime end, CancellationToken ct = default);
    Task<List<DiaperChange>> GetDiaperChangesInRangeAsync(UtcDateTime start, UtcDateTime end, CancellationToken ct = default);
}

using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;
using GajaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Infrastructure.Services;

public class TrackingRepository(GajaDbContext dbContext) : ITrackingRepository
{
    public Task<List<SleepSession>> GetSleepSessionsInRangeAsync(UtcDateTime start, UtcDateTime end, CancellationToken ct = default)
    {
        return dbContext.SleepSessions
            .Where(x => (DateTime)x.StartTime < (DateTime)end && (x.EndTime == null || (DateTime)x.EndTime > (DateTime)start))
            .ToListAsync(ct);
    }

    public Task<List<NursingFeed>> GetNursingFeedsInRangeAsync(UtcDateTime start, UtcDateTime end, CancellationToken ct = default)
    {
        return dbContext.NursingFeeds
            .Where(x => (DateTime)x.StartTime >= (DateTime)start && (DateTime)x.StartTime < (DateTime)end)
            .ToListAsync(ct);
    }

    public Task<List<BottleFeed>> GetBottleFeedsInRangeAsync(UtcDateTime start, UtcDateTime end, CancellationToken ct = default)
    {
        return dbContext.BottleFeeds
            .Where(x => (DateTime)x.Time >= (DateTime)start && (DateTime)x.Time < (DateTime)end)
            .ToListAsync(ct);
    }

    public Task<List<CryingSession>> GetCryingSessionsInRangeAsync(UtcDateTime start, UtcDateTime end, CancellationToken ct = default)
    {
        return dbContext.CryingSessions
            .Where(x => (DateTime)x.StartTime < (DateTime)end && (x.EndTime == null || (DateTime)x.EndTime > (DateTime)start))
            .ToListAsync(ct);
    }

    public Task<List<DiaperChange>> GetDiaperChangesInRangeAsync(UtcDateTime start, UtcDateTime end, CancellationToken ct = default)
    {
        return dbContext.DiaperChanges
            .Where(x => (DateTime)x.Time >= (DateTime)start && (DateTime)x.Time < (DateTime)end)
            .ToListAsync(ct);
    }
}

using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Infrastructure.Services;

public class StatsService(GajaDbContext dbContext) : IStatsService
{
    public async Task<TrackingStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        return new TrackingStats(
            await dbContext.NursingFeeds.CountAsync(cancellationToken),
            await dbContext.BottleFeeds.CountAsync(cancellationToken),
            await dbContext.SleepSessions.CountAsync(cancellationToken),
            await dbContext.DiaperChanges.CountAsync(cancellationToken),
            await dbContext.CryingSessions.CountAsync(cancellationToken)
        );
    }
}

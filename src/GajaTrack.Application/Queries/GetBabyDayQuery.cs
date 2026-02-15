using GajaTrack.Application.Interfaces;
using GajaTrack.Domain;
using GajaTrack.Domain.Entities;

namespace GajaTrack.Application.Queries;

public static class GetBabyDay
{
    public record Query(
        DateOnly StartDate,
        DateOnly EndDate,
        TimeZoneInfo TimeZone);

    public class Execution(ITrackingRepository repository)
    {
        public async Task<List<BabyDay>> RunAsync(Query query, CancellationToken cancellationToken)
        {
        // 1. Determine Search Window (Buffer to catch overlapping events)
        var searchWindowStart = query.StartDate.AddDays(-1).ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);
        var searchWindowEnd = query.EndDate.AddDays(2).ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);

        var searchWindowStartUtc = UtcDateTime.FromDateTime(searchWindowStart);
        var searchWindowEndUtc = UtcDateTime.FromDateTime(searchWindowEnd);

        // 2. Fetch Data
        var sleepTask = repository.GetSleepSessionsInRangeAsync(searchWindowStartUtc, searchWindowEndUtc, cancellationToken);
        var nursingTask = repository.GetNursingFeedsInRangeAsync(searchWindowStartUtc, searchWindowEndUtc, cancellationToken);
        var bottleTask = repository.GetBottleFeedsInRangeAsync(searchWindowStartUtc, searchWindowEndUtc, cancellationToken);
        var cryingTask = repository.GetCryingSessionsInRangeAsync(searchWindowStartUtc, searchWindowEndUtc, cancellationToken);
        var diaperTask = repository.GetDiaperChangesInRangeAsync(searchWindowStartUtc, searchWindowEndUtc, cancellationToken);

            await Task.WhenAll(sleepTask, nursingTask, bottleTask, cryingTask, diaperTask);

            var result = new List<BabyDay>();

            // 3. Iterate Days and delegate filtering/construction to Domain
            for (var currentDate = query.StartDate; currentDate <= query.EndDate; currentDate = currentDate.AddDays(1))
            {
                result.Add(BabyDay.Create(
                    currentDate,
                    query.TimeZone,
                    sleepTask.Result,
                    cryingTask.Result,
                    nursingTask.Result,
                    bottleTask.Result,
                    diaperTask.Result
                ));
            }

            return result;
        }
    }
}

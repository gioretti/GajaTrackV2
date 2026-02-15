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
            // 1. Determine Search Range (Buffer to catch overlapping events)
            var rangeBufferStart = query.StartDate.AddDays(-1).ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);
            var rangeBufferEnd = query.EndDate.AddDays(2).ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);

            var repositoryFetchStartUtc = UtcDateTime.FromDateTime(rangeBufferStart);
            var repositoryFetchEndUtc = UtcDateTime.FromDateTime(rangeBufferEnd);

            // 2. Fetch all potential candidates from Repository
            var sleepTask = repository.GetSleepSessionsInRangeAsync(repositoryFetchStartUtc, repositoryFetchEndUtc, cancellationToken);
            var nursingTask = repository.GetNursingFeedsInRangeAsync(repositoryFetchStartUtc, repositoryFetchEndUtc, cancellationToken);
            var bottleTask = repository.GetBottleFeedsInRangeAsync(repositoryFetchStartUtc, repositoryFetchEndUtc, cancellationToken);
            var cryingTask = repository.GetCryingSessionsInRangeAsync(repositoryFetchStartUtc, repositoryFetchEndUtc, cancellationToken);
            var diaperTask = repository.GetDiaperChangesInRangeAsync(repositoryFetchStartUtc, repositoryFetchEndUtc, cancellationToken);

            await Task.WhenAll(sleepTask, nursingTask, bottleTask, cryingTask, diaperTask);

            var result = new List<BabyDay>();

            // 3. Project each day into a BabyDay object with ONLY its specific entities
            for (var currentDate = query.StartDate; currentDate <= query.EndDate; currentDate = currentDate.AddDays(1))
            {
                var timeBounds = BabyDay.CalculateTimeBounds(currentDate, query.TimeZone);

                // Explicitly filter candidates for this specific logical day
                var daySleep = sleepTask.Result.Where(s => BabyDay.IsIntersecting(timeBounds, s.StartTime, s.EndTime)).ToList();
                var dayCrying = cryingTask.Result.Where(c => BabyDay.IsIntersecting(timeBounds, c.StartTime, c.EndTime)).ToList();
                var dayNursing = nursingTask.Result.Where(n => BabyDay.IsInTimeBounds(timeBounds, n.StartTime)).ToList();
                var dayBottle = bottleTask.Result.Where(b => BabyDay.IsInTimeBounds(timeBounds, b.Time)).ToList();
                var dayDiaper = diaperTask.Result.Where(di => BabyDay.IsInTimeBounds(timeBounds, di.Time)).ToList();

                result.Add(new BabyDay(
                    timeBounds,
                    daySleep,
                    dayCrying,
                    dayNursing,
                    dayBottle,
                    dayDiaper
                ));
            }

            return result;
        }
    }
}

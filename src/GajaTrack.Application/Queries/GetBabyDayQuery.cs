using GajaTrack.Application.Interfaces;
using GajaTrack.Domain;
using GajaTrack.Domain.Entities;
using GajaTrack.Domain.Services;

namespace GajaTrack.Application.Queries;

public interface IGetBabyDayQuery
{
    Task<List<BabyDay>> ExecuteAsync(DateOnly startDate, DateOnly endDate, TimeZoneInfo? timeZone = null, CancellationToken cancellationToken = default);
}

public class GetBabyDayQuery(ITrackingRepository repository, DailyRhythmMapDomainService domainService) : IGetBabyDayQuery
{
    public async Task<List<BabyDay>> ExecuteAsync(DateOnly startDate, DateOnly endDate, TimeZoneInfo? timeZone = null, CancellationToken cancellationToken = default)
    {
        timeZone ??= TimeZoneInfo.Local;

        // 1. Determine Fetch Range (Buffer to catch overlapping events)
        var fetchStart = startDate.AddDays(-1).ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);
        var fetchEnd = endDate.AddDays(2).ToDateTime(new TimeOnly(0, 0), DateTimeKind.Utc);

        var utcStart = UtcDateTime.FromDateTime(fetchStart);
        var utcEnd = UtcDateTime.FromDateTime(fetchEnd);

        // 2. Fetch Data
        var sleepTask = repository.GetSleepSessionsInRangeAsync(utcStart, utcEnd, cancellationToken);
        var nursingTask = repository.GetNursingFeedsInRangeAsync(utcStart, utcEnd, cancellationToken);
        var bottleTask = repository.GetBottleFeedsInRangeAsync(utcStart, utcEnd, cancellationToken);
        var cryingTask = repository.GetCryingSessionsInRangeAsync(utcStart, utcEnd, cancellationToken);
        var diaperTask = repository.GetDiaperChangesInRangeAsync(utcStart, utcEnd, cancellationToken);

        await Task.WhenAll(sleepTask, nursingTask, bottleTask, cryingTask, diaperTask);

        var result = new List<BabyDay>();

        // 3. Iterate Days
        for (var d = startDate; d <= endDate; d = d.AddDays(1))
        {
            var window = domainService.GetDailyRhythmMapWindow(d, timeZone);

            // Filter events for this window
            var daySleep = sleepTask.Result.Where(s => IsIntersecting(window, s.StartTime, s.EndTime)).ToList();
            var dayCrying = cryingTask.Result.Where(c => IsIntersecting(window, c.StartTime, c.EndTime)).ToList();
            var dayNursing = nursingTask.Result.Where(n => domainService.IsInWindow(window, n.StartTime)).ToList();
            var dayBottle = bottleTask.Result.Where(b => domainService.IsInWindow(window, b.Time)).ToList();
            var dayDiaper = diaperTask.Result.Where(di => domainService.IsInWindow(window, di.Time)).ToList();

            result.Add(new BabyDay(
                d,
                window,
                daySleep,
                dayCrying,
                dayNursing,
                dayBottle,
                dayDiaper
            ));
        }

        return result;
    }

    private bool IsIntersecting(TimeRange window, UtcDateTime start, UtcDateTime? end)
    {
        var range = new TimeRange(start, end ?? UtcDateTime.Now());
        return window.Overlaps(range);
    }
}

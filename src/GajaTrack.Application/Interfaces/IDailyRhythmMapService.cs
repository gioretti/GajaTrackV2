using GajaTrack.Application.DTOs.DailyRhythmMap;

namespace GajaTrack.Application.Interfaces;

public interface IDailyRhythmMapService
{
    Task<List<DailyRhythmMapDay>> GetDailyRhythmMapAsync(DateOnly startDate, DateOnly endDate, bool mostRecentFirst = false, TimeZoneInfo? timeZone = null, CancellationToken cancellationToken = default);
}

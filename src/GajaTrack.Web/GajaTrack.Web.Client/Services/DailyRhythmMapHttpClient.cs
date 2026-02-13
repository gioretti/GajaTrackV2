using System.Net.Http.Json;
using GajaTrack.Application.DTOs.DailyRhythmMap;
using GajaTrack.Application.Interfaces;

namespace GajaTrack.Web.Client.Services;

public class DailyRhythmMapHttpClient(HttpClient http) : IDailyRhythmMapService
{
    public async Task<List<DailyRhythmMapDay>> GetDailyRhythmMapAsync(DateOnly startDate, DateOnly endDate, bool mostRecentFirst = false, TimeZoneInfo? timeZone = null, CancellationToken cancellationToken = default)
    {
        // Note: The Server API handles the calculation and timezones.
        // We pass the dates and let the server do the heavy lifting.
        var url = $"/api/daily-rhythm-map?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        
        var result = await http.GetFromJsonAsync<List<DailyRhythmMapDay>>(url, cancellationToken);
        
        if (result == null) return new List<DailyRhythmMapDay>();

        if (mostRecentFirst)
        {
            result.Reverse();
        }

        return result;
    }
}

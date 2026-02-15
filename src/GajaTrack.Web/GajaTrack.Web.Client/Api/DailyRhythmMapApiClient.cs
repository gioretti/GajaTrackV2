using System.Net.Http.Json;
using GajaTrack.Application.DTOs.DailyRhythmMap;

namespace GajaTrack.Web.Client.Api;

public class DailyRhythmMapApiClient(HttpClient http)
{
    public async Task<List<DailyRhythmMapDay>> GetAsync(DateOnly startDate, DateOnly endDate, bool mostRecentFirst = false)
    {
        var timeZoneId = TimeZoneInfo.Local.Id;
        var url = $"/api/daily-rhythm-map?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&mostRecentFirst={mostRecentFirst}&timeZoneId={Uri.EscapeDataString(timeZoneId)}";
        
        var result = await http.GetFromJsonAsync<List<DailyRhythmMapDay>>(url);
        return result ?? new List<DailyRhythmMapDay>();
    }
}

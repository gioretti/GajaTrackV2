using System.Net.Http.Json;
using GajaTrack.Application.DTOs.Stats;

namespace GajaTrack.Web.Client.Api;

public class StatsApiClient(HttpClient http)
{
    public async Task<TrackingStats?> GetAsync()
    {
        return await http.GetFromJsonAsync<TrackingStats>("/api/stats");
    }
}

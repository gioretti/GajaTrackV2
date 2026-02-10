using System.Net.Http.Json;
using GajaTrack.Application.Interfaces;

namespace GajaTrack.Web.Client.Services;

public class StatsHttpClient(HttpClient http) : IStatsService
{
    public async Task<TrackingStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = await http.GetFromJsonAsync<TrackingStats>("/api/stats", cancellationToken);
        return stats ?? new TrackingStats(0, 0, 0, 0, 0);
    }
}

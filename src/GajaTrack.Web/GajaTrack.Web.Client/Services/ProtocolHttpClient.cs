using System.Net.Http.Json;
using GajaTrack.Application.DTOs.Protocol;
using GajaTrack.Application.Interfaces;

namespace GajaTrack.Web.Client.Services;

public class ProtocolHttpClient(HttpClient http) : IProtocolService
{
    public async Task<List<ProtocolDay>> GetProtocolAsync(DateOnly startDate, DateOnly endDate, bool mostRecentFirst = false, TimeZoneInfo? timeZone = null, CancellationToken cancellationToken = default)
    {
        // Note: The Server API handles the calculation and timezones.
        // We pass the dates and let the server do the heavy lifting.
        var url = $"/api/protocol?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        
        var result = await http.GetFromJsonAsync<List<ProtocolDay>>(url, cancellationToken);
        
        if (result == null) return new List<ProtocolDay>();

        if (mostRecentFirst)
        {
            result.Reverse();
        }

        return result;
    }
}

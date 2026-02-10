using GajaTrack.Application.Interfaces;

namespace GajaTrack.Web.Client.Services;

public class ExportHttpClient(HttpClient http) : IExportService
{
    public async Task<byte[]> ExportDataAsync(CancellationToken cancellationToken = default)
    {
        var response = await http.GetAsync("/api/export", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }
        return Array.Empty<byte>();
    }
}

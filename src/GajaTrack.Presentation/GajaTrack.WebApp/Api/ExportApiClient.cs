namespace GajaTrack.WebApp.Api;

public class ExportApiClient(HttpClient http)
{
    public async Task<byte[]> GetExportBytesAsync()
    {
        var response = await http.GetAsync("/api/export");
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }
        return Array.Empty<byte>();
    }
}


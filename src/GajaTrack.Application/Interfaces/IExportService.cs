namespace GajaTrack.Application.Interfaces;

public interface IExportService
{
    Task<byte[]> ExportDataAsync(CancellationToken cancellationToken = default);
}

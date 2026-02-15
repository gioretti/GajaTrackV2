using GajaTrack.Application.DTOs.Import;

namespace GajaTrack.Application.Interfaces;

public interface IBabyPlusImportService
{
    Task<ImportSummary> ImportFromStreamAsync(Stream stream, IProgress<string>? progress = null, CancellationToken cancellationToken = default);
}

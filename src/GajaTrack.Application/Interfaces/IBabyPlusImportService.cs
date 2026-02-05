namespace GajaTrack.Application.Interfaces;

public record ImportSummary(
    int NursingFeedsImported,
    int BottleFeedsImported,
    int SleepSessionsImported,
    int DiaperChangesImported,
    int CryingSessionsImported
);

public interface IBabyPlusImportService
{
    Task<ImportSummary> ImportFromStreamAsync(Stream stream, IProgress<string>? progress = null, CancellationToken cancellationToken = default);
}

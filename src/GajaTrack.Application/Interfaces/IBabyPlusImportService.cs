namespace GajaTrack.Application.Interfaces;

public record ImportSummary(
    int NursingFeedsImported,
    int BottleFeedsImported,
    int SleepSessionsImported,
    int DiaperChangesImported
);

public interface IBabyPlusImportService
{
    Task<ImportSummary> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken = default);
}

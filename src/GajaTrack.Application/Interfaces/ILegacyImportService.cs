namespace GajaTrack.Application.Interfaces;

public record ImportSummary(
    int NursingFeedsImported,
    int BottleFeedsImported,
    int SleepSessionsImported,
    int DiaperChangesImported
);

public interface ILegacyImportService
{
    Task<ImportSummary> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken = default);
}

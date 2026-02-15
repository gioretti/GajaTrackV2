namespace GajaTrack.Application.DTOs.Import;

public record ImportSummary(
    int NursingFeedsImported,
    int BottleFeedsImported,
    int SleepSessionsImported,
    int DiaperChangesImported,
    int CryingSessionsImported);

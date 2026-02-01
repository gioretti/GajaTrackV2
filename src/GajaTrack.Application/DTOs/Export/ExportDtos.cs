namespace GajaTrack.Application.DTOs.Export;

public record GajaTrackRoot(GajaTrackExport GajaTracking);

public record GajaTrackExport(
    List<ExportNursingFeed> NursingFeeds,
    List<ExportBottleFeed> BottleFeeds,
    List<ExportSleepSession> SleepSessions,
    List<ExportDiaperChange> DiaperChanges
);

public record ExportNursingFeed(Guid Id, DateTime StartTime, DateTime? EndTime);
public record ExportBottleFeed(Guid Id, DateTime Time, int AmountMl, string Content);
public record ExportSleepSession(Guid Id, DateTime StartTime, DateTime? EndTime);
public record ExportDiaperChange(Guid Id, DateTime Time, string Type);

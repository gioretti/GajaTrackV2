namespace GajaTrack.Application.DTOs.DailyRhythmMap;

public enum DailyRhythmMapEventType
{
    Sleep,
    Crying,
    Nursing,
    BottleFormula,
    BottleMilk,
    Diaper
}

public record DailyRhythmMapEvent(
    Guid Id,
    DailyRhythmMapEventType Type,
    DateTime OriginalStartTime,
    double StartMinute, // Minutes from window start (0 to 1440)
    double DurationMinutes,
    string? Description = null
)
{
    public static DailyRhythmMapEvent Create(
        Guid id,
        DailyRhythmMapEventType type,
        DateTime originalStartTime,
        DateTime windowStart,
        DateTime effectiveStart,
        DateTime effectiveEnd,
        string? description = null)
    {
        var startMin = (effectiveStart - windowStart).TotalMinutes;
        var durMin = (effectiveEnd - effectiveStart).TotalMinutes;

        // Ensure within [0, 1440] bounds
        startMin = Math.Max(0, Math.Min(1440, startMin));
        durMin = Math.Max(0, Math.Min(1440 - startMin, durMin));

        return new DailyRhythmMapEvent(id, type, originalStartTime, startMin, durMin, description);
    }
}

public record DailyRhythmMapSummary(
    double NapsMinutes,
    double NightSleepMinutes,
    int NightWakingCount
);

public record DailyRhythmMapDay(
    DateOnly Date,
    DateTime WindowStart, // Date + 06:00
    DateTime WindowEnd,   // Date + 1 day + 06:00
    List<DailyRhythmMapEvent> Events,
    DailyRhythmMapSummary Summary
);

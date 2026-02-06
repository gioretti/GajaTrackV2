namespace GajaTrack.Application.DTOs.Protocol;

public enum ProtocolEventType
{
    Sleep,
    Crying,
    Nursing,
    BottleFormula,
    BottleMilk,
    Diaper
}

public record ProtocolEvent(
    Guid Id,
    ProtocolEventType Type,
    DateTime OriginalStartTime,
    double StartMinute, // Minutes from window start (0 to 1440)
    double DurationMinutes,
    string? Description = null
)
{
    public static ProtocolEvent Create(
        Guid id,
        ProtocolEventType type,
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

        return new ProtocolEvent(id, type, originalStartTime, startMin, durMin, description);
    }
}

public record ProtocolDay(
    DateOnly Date,
    DateTime WindowStart, // Date + 06:00
    DateTime WindowEnd,   // Date + 1 day + 06:00
    List<ProtocolEvent> Events
);

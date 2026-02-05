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
    double StartMinute, // Minutes from 06:00 (0 to 1440)
    double DurationMinutes,
    string? Description = null
);

public record ProtocolDay(
    DateOnly Date,
    DateTime WindowStart, // Date + 06:00
    DateTime WindowEnd,   // Date + 1 day + 06:00
    List<ProtocolEvent> Events
);

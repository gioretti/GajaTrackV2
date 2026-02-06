namespace GajaTrack.Domain.Entities;

public readonly record struct TimeRange
{
    public UtcDateTime Start { get; init; }
    public UtcDateTime End { get; init; }

    public TimeRange(UtcDateTime start, UtcDateTime end)
    {
        if (end.Value < start.Value)
        {
            throw new ArgumentException("End time cannot be before start time.", nameof(end));
        }

        Start = start;
        End = end;
    }

    public bool Overlaps(TimeRange other)
    {
        return Start.Value < other.End.Value && End.Value > other.Start.Value;
    }

    public TimeRange? GetIntersection(TimeRange other)
    {
        if (!Overlaps(other)) return null;

        var start = Start.Value > other.Start.Value ? Start : other.Start;
        var end = End.Value < other.End.Value ? End : other.End;

        return new TimeRange(start, end);
    }

    public double TotalMinutes => (End.Value - Start.Value).TotalMinutes;
}

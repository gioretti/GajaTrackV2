namespace GajaTrack.Domain.Entities;

public readonly record struct UtcDateTime
{
    public DateTime Value { get; init; }

    private UtcDateTime(DateTime value)
    {
        if (value.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("UtcDateTime only accepts DateTimeKind.Utc.", nameof(value));
        }

        Value = value;
    }

    public static UtcDateTime FromDateTime(DateTime value) => new(value);

    public static UtcDateTime Now() => new(DateTime.UtcNow);

    public static implicit operator DateTime(UtcDateTime utcDateTime) => utcDateTime.Value;
    
    public override string ToString() => Value.ToString("O");
}

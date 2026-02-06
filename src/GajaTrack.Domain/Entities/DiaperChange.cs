namespace GajaTrack.Domain.Entities;

public enum DiaperType
{
    Wet = 0,
    Soiled = 1,
    Mixed = 2
}

public class DiaperChange
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid BabyId { get; init; }
    public string ExternalId { get; init; } = null!;
    public UtcDateTime Time { get; private set; }
    public DiaperType Type { get; private set; }

    // For EF Core
    private DiaperChange() { }

    public static DiaperChange Create(Guid babyId, string externalId, UtcDateTime time, DiaperType type)
    {
        return new DiaperChange
        {
            BabyId = babyId,
            ExternalId = externalId,
            Time = time,
            Type = type
        };
    }

    public void Update(UtcDateTime time, DiaperType type)
    {
        Time = time;
        Type = type;
    }
}
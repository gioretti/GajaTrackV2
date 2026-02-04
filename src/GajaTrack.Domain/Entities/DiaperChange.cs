using GajaTrack.Domain.Enums;

namespace GajaTrack.Domain.Entities;

public class DiaperChange
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid BabyId { get; init; }
    public string ExternalId { get; init; } = null!;
    public DateTime Time { get; private set; }
    public DiaperType Type { get; private set; }

    // For EF Core
    private DiaperChange() { }

    public static DiaperChange Create(Guid babyId, string externalId, DateTime time, DiaperType type)
    {
        return new DiaperChange
        {
            BabyId = babyId,
            ExternalId = externalId,
            Time = time,
            Type = type
        };
    }

    public void Update(DateTime time, DiaperType type)
    {
        Time = time;
        Type = type;
    }
}
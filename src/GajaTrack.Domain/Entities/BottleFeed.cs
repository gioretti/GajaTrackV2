namespace GajaTrack.Domain.Entities;

public enum BottleContent
{
    BreastMilk = 0,
    Formula = 1
}

public class BottleFeed
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid BabyId { get; init; }
    public string ExternalId { get; init; } = null!;
    public UtcDateTime Time { get; private set; }
    public int AmountMl { get; private set; }
    public BottleContent Content { get; private set; }

    // For EF Core
    private BottleFeed() { }

    public static BottleFeed Create(Guid babyId, string externalId, UtcDateTime time, int amountMl, BottleContent content)
    {
        if (amountMl <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.", nameof(amountMl));
        }

        return new BottleFeed
        {
            BabyId = babyId,
            ExternalId = externalId,
            Time = time,
            AmountMl = amountMl,
            Content = content
        };
    }

    public void Update(UtcDateTime time, int amountMl, BottleContent content)
    {
        if (amountMl <= 0)
        {
            throw new ArgumentException("Amount must be greater than zero.", nameof(amountMl));
        }

        Time = time;
        AmountMl = amountMl;
        Content = content;
    }
}
using GajaTrack.Domain.Enums;

namespace GajaTrack.Domain.Entities;

public class BottleFeed
{
  public Guid Id { get; init; } = Guid.CreateVersion7();
  public required Guid BabyId { get; init; }
  public required string ExternalId { get; init; }
  public required DateTime Time { get; init; }
  public int AmountMl { get; set; }
  public required BottleContent Content { get; set; }

  // Private constructor for EF Core and internal creation
  private BottleFeed() { }

  public static BottleFeed Create(Guid babyId, string externalId, DateTime time, int amountMl, BottleContent content)
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
}
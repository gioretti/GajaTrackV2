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
}

using GajaTrack.Domain.Enums;

namespace GajaTrack.Domain.Entities;

public class DiaperChange
{
  public Guid Id { get; init; } = Guid.CreateVersion7();
  public required Guid BabyId { get; init; }
  public required string ExternalId { get; init; }
  public required DateTime Time { get; init; }
  public required DiaperType Type { get; set; }
}

namespace GajaTrack.Domain.Entities;

public class NursingFeed
{
  public Guid Id { get; init; } = Guid.CreateVersion7();
  public required Guid BabyId { get; init; }
  public required string ExternalId { get; init; }
  public required DateTime StartTime { get; init; }
  public DateTime? EndTime { get; set; }
}

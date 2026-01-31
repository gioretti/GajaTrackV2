namespace GajaTrack.Domain.Entities;

public class NursingFeed
{
  public Guid Id { get; init; } = Guid.CreateVersion7();
  public required Guid BabyId { get; init; }
  public required string ExternalId { get; init; }
  public required DateTime StartTime { get; init; }
  public DateTime? EndTime { get; set; }

  private NursingFeed() { }

  public static NursingFeed Create(Guid babyId, string externalId, DateTime startTime, DateTime? endTime)
  {
    if (endTime.HasValue && endTime < startTime)
    {
      throw new ArgumentException("EndTime cannot be before StartTime.", nameof(endTime));
    }

    return new NursingFeed
    {
      BabyId = babyId,
      ExternalId = externalId,
      StartTime = startTime,
      EndTime = endTime
    };
  }
}
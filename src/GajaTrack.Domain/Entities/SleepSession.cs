namespace GajaTrack.Domain.Entities;

public class SleepSession
{
  public Guid Id { get; init; } = Guid.CreateVersion7();
  public required Guid BabyId { get; init; }
  public required string ExternalId { get; init; }
  public required DateTime StartTime { get; set; }
  public DateTime? EndTime { get; set; }

  private SleepSession() { }

  public static SleepSession Create(Guid babyId, string externalId, DateTime startTime, DateTime? endTime)
  {
    if (endTime.HasValue && endTime < startTime)
    {
      throw new ArgumentException("EndTime cannot be before StartTime.", nameof(endTime));
    }

    return new SleepSession
    {
      BabyId = babyId,
      ExternalId = externalId,
      StartTime = startTime,
      EndTime = endTime
    };
  }

  public void Update(DateTime startTime, DateTime? endTime)
  {
    if (endTime.HasValue && endTime < startTime)
    {
      throw new ArgumentException("EndTime cannot be before StartTime.", nameof(endTime));
    }

    StartTime = startTime;
    EndTime = endTime;
  }
}
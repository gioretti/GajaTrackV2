namespace GajaTrack.Domain.Entities;

public class NursingFeed
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid BabyId { get; init; }
    public string ExternalId { get; init; } = null!;
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }

    // For EF Core
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
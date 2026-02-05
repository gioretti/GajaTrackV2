namespace GajaTrack.Domain.Entities;

public class CryingSession
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid BabyId { get; init; }
    public string ExternalId { get; init; } = null!;
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value - StartTime : null;

    // For EF Core
    private CryingSession() { }

    public static CryingSession Create(Guid babyId, string externalId, DateTime startTime, DateTime? endTime)
    {
        if (endTime.HasValue && endTime < startTime)
        {
            throw new ArgumentException("EndTime cannot be before StartTime.", nameof(endTime));
        }

        return new CryingSession
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

namespace GajaTrack.Domain.Entities;

public class CryingSession
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    public Guid BabyId { get; init; }
    public string ExternalId { get; init; } = null!;
    public UtcDateTime StartTime { get; private set; }
    public UtcDateTime? EndTime { get; private set; }
    
    public TimeSpan? Duration => EndTime.HasValue ? EndTime.Value.Value - StartTime.Value : null;

    // For EF Core
    private CryingSession() { }

    public static CryingSession Create(Guid babyId, string externalId, UtcDateTime startTime, UtcDateTime? endTime)
    {
        if (endTime.HasValue && endTime.Value.Value < startTime.Value)
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

    public void Update(UtcDateTime startTime, UtcDateTime? endTime)
    {
        if (endTime.HasValue && endTime.Value.Value < startTime.Value)
        {
            throw new ArgumentException("EndTime cannot be before StartTime.", nameof(endTime));
        }

        StartTime = startTime;
        EndTime = endTime;
    }
}

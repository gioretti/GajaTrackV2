using GajaTrack.Domain.Entities;
using Xunit;

namespace GajaTrack.Tests.Unit.Domain;

public class CryingSessionTests
{
    [Fact]
    public void Create_ShouldInitializeCorrectly()
    {
        // Arrange
        var babyId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddMinutes(15);
        var externalId = "ext-123";

        // Act
        var session = CryingSession.Create(babyId, externalId, startTime, endTime);

        // Assert
        Assert.NotNull(session);
        Assert.Equal(babyId, session.BabyId);
        Assert.Equal(externalId, session.ExternalId);
        Assert.Equal(startTime, session.StartTime);
        Assert.Equal(endTime, session.EndTime);
        Assert.Equal(TimeSpan.FromMinutes(15), session.Duration);
        Assert.NotEqual(Guid.Empty, session.Id);
    }

    [Fact]
    public void Create_ShouldAllowNullEndTime()
    {
        // Arrange
        var babyId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        var externalId = "ext-123";

        // Act
        var session = CryingSession.Create(babyId, externalId, startTime, null);

        // Assert
        Assert.NotNull(session);
        Assert.Equal(startTime, session.StartTime);
        Assert.Null(session.EndTime);
        Assert.Null(session.Duration);
    }

    [Fact]
    public void Create_ShouldThrow_WhenEndTimeBeforeStartTime()
    {
        // Arrange
        var babyId = Guid.NewGuid();
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddMinutes(-1);
        var externalId = "ext-123";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            CryingSession.Create(babyId, externalId, startTime, endTime));
            
        Assert.Contains("EndTime cannot be before StartTime", exception.Message);
    }

    [Fact]
    public void Update_ShouldUpdateValues()
    {
        // Arrange
        var session = CryingSession.Create(Guid.NewGuid(), "ext-123", DateTime.UtcNow, null);
        var newStart = DateTime.UtcNow.AddMinutes(10);
        var newEnd = newStart.AddMinutes(5);

        // Act
        session.Update(newStart, newEnd);

        // Assert
        Assert.Equal(newStart, session.StartTime);
        Assert.Equal(newEnd, session.EndTime);
        Assert.Equal(TimeSpan.FromMinutes(5), session.Duration);
    }
}
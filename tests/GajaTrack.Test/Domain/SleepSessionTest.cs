using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain.Entities;

public class SleepSessionTest
{
    [Fact]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var babyId = Guid.NewGuid();
        var externalId = "pk3";
        var start = UtcDateTime.FromDateTime(DateTime.UtcNow);
        var end = UtcDateTime.FromDateTime(start.Value.AddHours(2));

        // Act
        var result = SleepSession.Create(babyId, externalId, start, end);

        // Assert
        Assert.Equal(babyId, result.BabyId);
        Assert.Equal(externalId, result.ExternalId);
        Assert.Equal(start, result.StartTime);
        Assert.Equal(end, result.EndTime);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public void Create_ShouldThrowArgumentException_WhenEndDateIsBeforeStartTime()
    {
        // Arrange
        var start = UtcDateTime.FromDateTime(DateTime.UtcNow);
        var end = UtcDateTime.FromDateTime(start.Value.AddMinutes(-30));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            SleepSession.Create(Guid.NewGuid(), "pk", start, end));
    }
}

using GajaTrack.Domain.Entities;

namespace GajaTrack.Test.Domain;

public class NursingFeedTests
{
    [Fact]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var babyId = Guid.NewGuid();
        var externalId = "pk1";
        var start = UtcDateTime.FromDateTime(DateTime.UtcNow);
        var end = UtcDateTime.FromDateTime(start.Value.AddMinutes(15));

        // Act
        var result = NursingFeed.Create(babyId, externalId, start, end);

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
        var end = UtcDateTime.FromDateTime(start.Value.AddMinutes(-15));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            NursingFeed.Create(Guid.NewGuid(), "pk", start, end));
    }

    [Fact]
    public void Create_ShouldAllowNullEndDate()
    {
        // Act
        var result = NursingFeed.Create(Guid.NewGuid(), "pk", UtcDateTime.Now(), null);

        // Assert
        Assert.Null(result.EndTime);
    }
}

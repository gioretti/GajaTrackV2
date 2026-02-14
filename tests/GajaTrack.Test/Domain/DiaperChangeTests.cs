using GajaTrack.Domain.Entities;

namespace GajaTrack.Test.Domain;

public class DiaperChangeTests
{
    [Fact]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var babyId = Guid.NewGuid();
        var externalId = "pk4";
        var time = UtcDateTime.FromDateTime(DateTime.UtcNow);
        var type = DiaperType.Soiled;

        // Act
        var result = DiaperChange.Create(babyId, externalId, time, type);

        // Assert
        Assert.Equal(babyId, result.BabyId);
        Assert.Equal(externalId, result.ExternalId);
        Assert.Equal(time, result.Time);
        Assert.Equal(type, result.Type);
        Assert.NotEqual(Guid.Empty, result.Id);
    }
}

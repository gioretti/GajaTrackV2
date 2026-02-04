using GajaTrack.Domain.Entities;

namespace GajaTrack.Tests.Unit.Domain;

public class BottleFeedTests
{
    [Fact]
    public void Create_ShouldInitializePropertiesCorrectly()
    {
        // Arrange
        var babyId = Guid.NewGuid();
        var externalId = "pk1";
        var time = DateTime.UtcNow;
        var amount = 150;
        var content = BottleContent.BreastMilk;

        // Act
        var result = BottleFeed.Create(babyId, externalId, time, amount, content);

        // Assert
        Assert.Equal(babyId, result.BabyId);
        Assert.Equal(externalId, result.ExternalId);
        Assert.Equal(time, result.Time);
        Assert.Equal(amount, result.AmountMl);
        Assert.Equal(content, result.Content);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Create_ShouldThrowArgumentException_WhenAmountIsZeroOrNegative(int invalidAmount)
    {
        // Arrange
        var babyId = Guid.NewGuid();
        var externalId = "pk1";
        var time = DateTime.UtcNow;
        var content = BottleContent.BreastMilk;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            BottleFeed.Create(babyId, externalId, time, invalidAmount, content));
    }
}

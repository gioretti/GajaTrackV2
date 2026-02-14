using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain.Entities;

public class UtcDateTimeTest
{
    [Fact]
    public void FromDateTime_WithUtcKind_ShouldSucceed()
    {
        // Arrange
        var utcNow = DateTime.UtcNow;

        // Act
        var utcDateTime = UtcDateTime.FromDateTime(utcNow);

        // Assert
        Assert.Equal(utcNow, utcDateTime.Value);
    }

    [Fact]
    public void FromDateTime_WithLocalKind_ShouldThrowArgumentException()
    {
        // Arrange
        var localNow = DateTime.Now;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => UtcDateTime.FromDateTime(localNow));
        Assert.Contains("UtcDateTime only accepts DateTimeKind.Utc", exception.Message);
    }

    [Fact]
    public void FromDateTime_WithUnspecifiedKind_ShouldThrowArgumentException()
    {
        // Arrange
        var unspecified = new DateTime(2026, 2, 6, 12, 0, 0, DateTimeKind.Unspecified);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => UtcDateTime.FromDateTime(unspecified));
        Assert.Contains("UtcDateTime only accepts DateTimeKind.Utc", exception.Message);
    }

    [Fact]
    public void Now_ShouldReturnUtcTime()
    {
        // Act
        var now = UtcDateTime.Now();

        // Assert
        Assert.Equal(DateTimeKind.Utc, now.Value.Kind);
    }

    [Fact]
    public void ImplicitConversion_ToDateTime_ShouldWork()
    {
        // Arrange
        var utcNow = DateTime.UtcNow;
        var utcDateTime = UtcDateTime.FromDateTime(utcNow);

        // Act
        DateTime result = utcDateTime;

        // Assert
        Assert.Equal(utcNow, result);
    }
}

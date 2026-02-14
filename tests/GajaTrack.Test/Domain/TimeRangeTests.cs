using GajaTrack.Domain.Entities;

namespace GajaTrack.Test.Domain;

public class TimeRangeTests
{
    [Fact]
    public void Create_WithValidRange_ShouldSucceed()
    {
        // Arrange
        var start = UtcDateTime.Now();
        var end = UtcDateTime.FromDateTime(start.Value.AddHours(1));

        // Act
        var range = new TimeRange(start, end);

        // Assert
        Assert.Equal(start, range.Start);
        Assert.Equal(end, range.End);
    }

    [Fact]
    public void Create_WithEndBeforeStart_ShouldThrow()
    {
        // Arrange
        var start = UtcDateTime.Now();
        var end = UtcDateTime.FromDateTime(start.Value.AddHours(-1));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TimeRange(start, end));
    }

    [Fact]
    public void Overlaps_WithOverlappingRanges_ShouldReturnTrue()
    {
        // Arrange
        var baseStart = UtcDateTime.FromDateTime(new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc));
        var baseEnd = UtcDateTime.FromDateTime(new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        var range = new TimeRange(baseStart, baseEnd);

        // Overlapping cases
        var partialOverlap = new TimeRange(UtcDateTime.FromDateTime(baseStart.Value.AddHours(-1)), UtcDateTime.FromDateTime(baseStart.Value.AddHours(1)));
        var contained = new TimeRange(UtcDateTime.FromDateTime(baseStart.Value.AddMinutes(10)), UtcDateTime.FromDateTime(baseEnd.Value.AddMinutes(-10)));

        // Assert
        Assert.True(range.Overlaps(partialOverlap));
        Assert.True(range.Overlaps(contained));
    }

    [Fact]
    public void GetIntersection_ShouldReturnCorrectRange()
    {
        // Arrange
        var windowStart = UtcDateTime.FromDateTime(new DateTime(2026, 1, 1, 6, 0, 0, DateTimeKind.Utc));
        var windowEnd = UtcDateTime.FromDateTime(new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc));
        var window = new TimeRange(windowStart, windowEnd);

        // Event: 05:00 to 07:00
        var eventStart = UtcDateTime.FromDateTime(new DateTime(2026, 1, 1, 5, 0, 0, DateTimeKind.Utc));
        var eventEnd = UtcDateTime.FromDateTime(new DateTime(2026, 1, 1, 7, 0, 0, DateTimeKind.Utc));
        var @event = new TimeRange(eventStart, eventEnd);

        // Act
        var intersection = window.GetIntersection(@event);

        // Assert
        Assert.NotNull(intersection);
        Assert.Equal(windowStart, intersection.Value.Start);
        Assert.Equal(eventEnd, intersection.Value.End);
        Assert.Equal(60, intersection.Value.TotalMinutes);
    }
}

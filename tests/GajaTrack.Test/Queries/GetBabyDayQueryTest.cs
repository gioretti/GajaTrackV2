using GajaTrack.Application.Interfaces;
using GajaTrack.Application.Queries;
using GajaTrack.Domain;
using GajaTrack.Domain.Entities;
using Moq;
using Xunit;

namespace GajaTrack.Test.Queries;

public class GetBabyDayTest
{
    private readonly Mock<ITrackingRepository> _repositoryMock = new();
    private readonly GetBabyDay.Execution _execution;

    public GetBabyDayTest()
    {
        _execution = new GetBabyDay.Execution(_repositoryMock.Object);
        
        // Default empty returns for all repository methods
        _repositoryMock.Setup(r => r.GetSleepSessionsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SleepSession>());
        _repositoryMock.Setup(r => r.GetNursingFeedsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NursingFeed>());
        _repositoryMock.Setup(r => r.GetBottleFeedsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BottleFeed>());
        _repositoryMock.Setup(r => r.GetCryingSessionsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CryingSession>());
        _repositoryMock.Setup(r => r.GetDiaperChangesInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiaperChange>());
    }

    [Fact]
    public async Task RunAsync_ShouldReturnCorrectNumberOfDays()
    {
        var start = new DateOnly(2026, 2, 15);
        var end = new DateOnly(2026, 2, 16);
        var query = new GetBabyDay.Query(start, end, TimeZoneInfo.Utc);

        var result = await _execution.RunAsync(query, CancellationToken.None);

        Assert.Equal(2, result.Count);
        // Feb 15 window starts at 06:00 UTC
        Assert.Equal(new DateTime(2026, 2, 15, 6, 0, 0, DateTimeKind.Utc), result[0].TimeBounds.Start.Value);
        // Feb 16 window starts at 06:00 UTC
        Assert.Equal(new DateTime(2026, 2, 16, 6, 0, 0, DateTimeKind.Utc), result[1].TimeBounds.Start.Value);
    }

    [Fact]
    public async Task RunAsync_ShouldIncludeSleepSessions_OnlyWhenIntersectingBounds()
    {
        var day = new DateOnly(2026, 2, 15);
        var query = new GetBabyDay.Query(day, day, TimeZoneInfo.Utc);
        
        var inBounds = SleepSession.Create(Guid.NewGuid(), "in", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 10, 0, 0, DateTimeKind.Utc)),
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 11, 0, 0, DateTimeKind.Utc)));
            
        var outOfBounds = SleepSession.Create(Guid.NewGuid(), "out", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc)),
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 1, 0, 0, DateTimeKind.Utc)));

        _repositoryMock.Setup(r => r.GetSleepSessionsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SleepSession> { inBounds, outOfBounds });

        var result = await _execution.RunAsync(query, CancellationToken.None);

        Assert.Single(result[0].SleepSessions);
        Assert.Equal(inBounds.Id, result[0].SleepSessions.First().Id);
    }

    [Fact]
    public async Task RunAsync_ShouldAssignOverlappingSleepSession_ToBothDays()
    {
        var start = new DateOnly(2026, 2, 15);
        var end = new DateOnly(2026, 2, 16);
        var overlappingSession = SleepSession.Create(Guid.NewGuid(), "overlap",
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 16, 5, 0, 0, DateTimeKind.Utc)),
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 16, 7, 0, 0, DateTimeKind.Utc)));

        _repositoryMock.Setup(r => r.GetSleepSessionsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SleepSession> { overlappingSession });

        var result = await _execution.RunAsync(new GetBabyDay.Query(start, end, TimeZoneInfo.Utc), CancellationToken.None);

        Assert.Contains(overlappingSession, result[0].SleepSessions);
        Assert.Contains(overlappingSession, result[1].SleepSessions);
    }

    [Fact]
    public async Task RunAsync_ShouldIncludeNursingFeeds_OnlyWhenInBounds()
    {
        var day = new DateOnly(2026, 2, 15);
        var inBounds = NursingFeed.Create(Guid.NewGuid(), "in", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 08, 0, 0, DateTimeKind.Utc)), null);
        var outOfBounds = NursingFeed.Create(Guid.NewGuid(), "out", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 05, 0, 0, DateTimeKind.Utc)), null);

        _repositoryMock.Setup(r => r.GetNursingFeedsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NursingFeed> { inBounds, outOfBounds });

        var result = await _execution.RunAsync(new GetBabyDay.Query(day, day, TimeZoneInfo.Utc), CancellationToken.None);

        Assert.Single(result[0].NursingFeeds);
        Assert.Equal(inBounds.Id, result[0].NursingFeeds.First().Id);
    }

    [Fact]
    public async Task RunAsync_ShouldIncludeBottleFeeds_OnlyWhenInBounds()
    {
        var day = new DateOnly(2026, 2, 15);
        var inBounds = BottleFeed.Create(Guid.NewGuid(), "in", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 12, 0, 0, DateTimeKind.Utc)), 150, BottleContent.Formula);
        var outOfBounds = BottleFeed.Create(Guid.NewGuid(), "out", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 16, 07, 0, 0, DateTimeKind.Utc)), 150, BottleContent.Formula);

        _repositoryMock.Setup(r => r.GetBottleFeedsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BottleFeed> { inBounds, outOfBounds });

        var result = await _execution.RunAsync(new GetBabyDay.Query(day, day, TimeZoneInfo.Utc), CancellationToken.None);

        Assert.Single(result[0].BottleFeeds);
        Assert.Equal(inBounds.Id, result[0].BottleFeeds.First().Id);
    }

    [Fact]
    public async Task RunAsync_ShouldIncludeDiaperChanges_OnlyWhenInBounds()
    {
        var day = new DateOnly(2026, 2, 15);
        var inBounds = DiaperChange.Create(Guid.NewGuid(), "in", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 14, 0, 0, DateTimeKind.Utc)), DiaperType.Wet);
        var outOfBounds = DiaperChange.Create(Guid.NewGuid(), "out", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 05, 59, 59, DateTimeKind.Utc)), DiaperType.Wet);

        _repositoryMock.Setup(r => r.GetDiaperChangesInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiaperChange> { inBounds, outOfBounds });

        var result = await _execution.RunAsync(new GetBabyDay.Query(day, day, TimeZoneInfo.Utc), CancellationToken.None);

        Assert.Single(result[0].DiaperChanges);
        Assert.Equal(inBounds.Id, result[0].DiaperChanges.First().Id);
    }

    [Fact]
    public async Task RunAsync_ShouldIncludeCryingSessions_OnlyWhenIntersectingBounds()
    {
        var day = new DateOnly(2026, 2, 15);
        var inBounds = CryingSession.Create(Guid.NewGuid(), "in", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 16, 0, 0, DateTimeKind.Utc)), 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 16, 15, 0, DateTimeKind.Utc)));
        var outOfBounds = CryingSession.Create(Guid.NewGuid(), "out", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 16, 06, 0, 0, DateTimeKind.Utc)), 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 16, 06, 15, 0, DateTimeKind.Utc)));

        _repositoryMock.Setup(r => r.GetCryingSessionsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CryingSession> { inBounds, outOfBounds });

        var result = await _execution.RunAsync(new GetBabyDay.Query(day, day, TimeZoneInfo.Utc), CancellationToken.None);

        Assert.Single(result[0].CryingSessions);
        Assert.Equal(inBounds.Id, result[0].CryingSessions.First().Id);
    }
}

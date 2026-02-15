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
    public async Task RunAsync_ShouldIncludeSleepSessions_WhenInBounds()
    {
        var day = new DateOnly(2026, 2, 15);
        var query = new GetBabyDay.Query(day, day, TimeZoneInfo.Utc);
        var session = SleepSession.Create(Guid.NewGuid(), "s1", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 10, 0, 0, DateTimeKind.Utc)),
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 11, 0, 0, DateTimeKind.Utc)));

        _repositoryMock.Setup(r => r.GetSleepSessionsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SleepSession> { session });

        var result = await _execution.RunAsync(query, CancellationToken.None);

        Assert.Single(result[0].SleepSessions);
        Assert.Equal(session.Id, result[0].SleepSessions.First().Id);
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
    public async Task RunAsync_ShouldIncludeNursingFeeds_WhenInBounds()
    {
        var day = new DateOnly(2026, 2, 15);
        var feed = NursingFeed.Create(Guid.NewGuid(), "n1", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 08, 0, 0, DateTimeKind.Utc)), null);

        _repositoryMock.Setup(r => r.GetNursingFeedsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NursingFeed> { feed });

        var result = await _execution.RunAsync(new GetBabyDay.Query(day, day, TimeZoneInfo.Utc), CancellationToken.None);

        Assert.Single(result[0].NursingFeeds);
    }

    [Fact]
    public async Task RunAsync_ShouldIncludeBottleFeeds_WhenInBounds()
    {
        var day = new DateOnly(2026, 2, 15);
        var feed = BottleFeed.Create(Guid.NewGuid(), "b1", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 12, 0, 0, DateTimeKind.Utc)), 150, BottleContent.Formula);

        _repositoryMock.Setup(r => r.GetBottleFeedsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BottleFeed> { feed });

        var result = await _execution.RunAsync(new GetBabyDay.Query(day, day, TimeZoneInfo.Utc), CancellationToken.None);

        Assert.Single(result[0].BottleFeeds);
    }

    [Fact]
    public async Task RunAsync_ShouldIncludeDiaperChanges_WhenInBounds()
    {
        var day = new DateOnly(2026, 2, 15);
        var diaper = DiaperChange.Create(Guid.NewGuid(), "d1", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 14, 0, 0, DateTimeKind.Utc)), DiaperType.Wet);

        _repositoryMock.Setup(r => r.GetDiaperChangesInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiaperChange> { diaper });

        var result = await _execution.RunAsync(new GetBabyDay.Query(day, day, TimeZoneInfo.Utc), CancellationToken.None);

        Assert.Single(result[0].DiaperChanges);
    }

    [Fact]
    public async Task RunAsync_ShouldIncludeCryingSessions_WhenInBounds()
    {
        var day = new DateOnly(2026, 2, 15);
        var crying = CryingSession.Create(Guid.NewGuid(), "c1", 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 16, 0, 0, DateTimeKind.Utc)), 
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 15, 16, 15, 0, DateTimeKind.Utc)));

        _repositoryMock.Setup(r => r.GetCryingSessionsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CryingSession> { crying });

        var result = await _execution.RunAsync(new GetBabyDay.Query(day, day, TimeZoneInfo.Utc), CancellationToken.None);

        Assert.Single(result[0].CryingSessions);
    }
}

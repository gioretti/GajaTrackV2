using GajaTrack.Application.Interfaces;
using GajaTrack.Application.Queries;
using GajaTrack.Domain.Entities;
using GajaTrack.Domain.Services;
using Moq;
using Xunit;

namespace GajaTrack.Test.Queries;

public class GetBabyDayQueryTest
{
    private readonly Mock<ITrackingRepository> _repositoryMock = new();
    private readonly DailyRhythmMapDomainService _domainService = new();
    private readonly GetBabyDayQuery _query;

    public GetBabyDayQueryTest()
    {
        _query = new GetBabyDayQuery(_repositoryMock.Object, _domainService);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldAssignOverlappingSession_ToBothDays()
    {
        // Request: Feb 15 to Feb 16
        var start = new DateOnly(2026, 2, 15);
        var end = new DateOnly(2026, 2, 16);
        var tz = TimeZoneInfo.Utc;

        // Session: Feb 16, 05:00 - 07:00 (Crosses the 06:00 boundary between Day 1 and Day 2)
        var overlappingSession = SleepSession.Create(
            Guid.NewGuid(), 
            "ext-1",
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 16, 5, 0, 0, DateTimeKind.Utc)),
            UtcDateTime.FromDateTime(new DateTime(2026, 2, 16, 7, 0, 0, DateTimeKind.Utc))
        );

        _repositoryMock.Setup(r => r.GetSleepSessionsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SleepSession> { overlappingSession });
        _repositoryMock.Setup(r => r.GetNursingFeedsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<NursingFeed>());
        _repositoryMock.Setup(r => r.GetBottleFeedsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BottleFeed>());
        _repositoryMock.Setup(r => r.GetCryingSessionsInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CryingSession>());
        _repositoryMock.Setup(r => r.GetDiaperChangesInRangeAsync(It.IsAny<UtcDateTime>(), It.IsAny<UtcDateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DiaperChange>());

        // Act
        var result = await _query.ExecuteAsync(start, end, tz);

        // Assert
        Assert.Equal(2, result.Count);
        
        // Day 1 (Feb 15, 06:00 - Feb 16, 06:00) should contain the session
        Assert.Contains(overlappingSession, result[0].SleepSessions);
        
        // Day 2 (Feb 16, 06:00 - Feb 17, 06:00) should also contain the session
        Assert.Contains(overlappingSession, result[1].SleepSessions);
    }
}

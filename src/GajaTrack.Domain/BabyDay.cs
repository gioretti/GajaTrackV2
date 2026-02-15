using GajaTrack.Domain.Entities;

namespace GajaTrack.Domain;

public record BabyDay(
    DateOnly Date,
    TimeRange Window,
    IEnumerable<SleepSession> SleepSessions,
    IEnumerable<CryingSession> CryingSessions,
    IEnumerable<NursingFeed> NursingFeeds,
    IEnumerable<BottleFeed> BottleFeeds,
    IEnumerable<DiaperChange> DiaperChanges
);

using System.Text.Json.Serialization;

namespace GajaTrack.Infrastructure.Services;

internal record BabyPlusExport(
    [property: JsonPropertyName("babyprofile")] List<JsonBabyProfile>? BabyProfiles,
    [property: JsonPropertyName("baby_nursingfeed")] List<JsonNursingFeed>? NursingFeeds,
    [property: JsonPropertyName("baby_bottlefeed")] List<JsonBottleFeed>? BottleFeeds,
    [property: JsonPropertyName("baby_sleep")] List<JsonSleep>? SleepSessions,
    [property: JsonPropertyName("baby_nappy")] List<JsonDiaper>? DiaperChanges,
    [property: JsonPropertyName("baby_crying")] List<JsonCryingSession>? CryingSessions
);

internal record JsonBabyProfile(
    string Name,
    DateTime BirthDate
);

internal record JsonCryingSession(
    string Pk,
    DateTime StartDate,
    DateTime? EndDate
);

internal record JsonNursingFeed(
    string Pk,
    DateTime StartDate,
    DateTime? EndDate
);

internal record JsonBottleFeed(
    string Pk,
    DateTime Date,
    double AmountMl,
    bool IsFormula
);

internal record JsonSleep(
    string Pk,
    DateTime StartDate,
    DateTime? EndDate
);

internal record JsonDiaper(
    string Pk,
    DateTime Date,
    string? Type
);
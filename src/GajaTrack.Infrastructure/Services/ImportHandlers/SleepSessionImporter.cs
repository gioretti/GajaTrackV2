using GajaTrack.Domain.Entities;

namespace GajaTrack.Infrastructure.Services.ImportHandlers;

internal static class SleepSessionImporter
{
    public static List<SleepSession> Map(List<JsonSleep>? source)
    {
        if (source == null) return [];

        var result = new List<SleepSession>();
        foreach (var item in source)
        {
            result.Add(new SleepSession
            {
                BabyId = Guid.Empty,
                ExternalId = item.Pk,
                StartTime = item.StartDate,
                EndTime = item.EndDate.Year == 1 ? null : item.EndDate
            });
        }
        return result;
    }
}

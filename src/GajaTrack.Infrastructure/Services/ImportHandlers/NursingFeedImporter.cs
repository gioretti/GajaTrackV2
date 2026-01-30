using GajaTrack.Domain.Entities;

namespace GajaTrack.Infrastructure.Services.ImportHandlers;

internal static class NursingFeedImporter
{
    public static List<NursingFeed> Map(List<JsonNursingFeed>? source)
    {
        if (source == null) return [];

        var result = new List<NursingFeed>();
        foreach (var item in source)
        {
            result.Add(new NursingFeed
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

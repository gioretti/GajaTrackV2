using GajaTrack.Application.Interfaces;
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
            if (item.StartDate > DateTime.UtcNow)
            {
                throw new ImportValidationException(nameof(NursingFeed), item.Pk, 
                    $"StartTime {item.StartDate} is in the future.");
            }

            if (item.EndDate.HasValue && item.EndDate < item.StartDate)
            {
                throw new ImportValidationException(nameof(NursingFeed), item.Pk, 
                    $"EndTime {item.EndDate} is before StartTime {item.StartDate}");
            }

            result.Add(new NursingFeed
            {
                BabyId = Guid.Empty,
                ExternalId = item.Pk,
                StartTime = item.StartDate,
                EndTime = item.EndDate
            });
        }
        return result;
    }
}
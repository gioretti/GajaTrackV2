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

            try
            {
                result.Add(NursingFeed.Create(
                    Guid.Empty,
                    item.Pk,
                    item.StartDate,
                    item.EndDate
                ));
            }
            catch (ArgumentException ex)
            {
                throw new ImportValidationException(nameof(NursingFeed), item.Pk, ex.Message);
            }
        }
        return result;
    }
}

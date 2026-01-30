using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;
using GajaTrack.Domain.Enums;

namespace GajaTrack.Infrastructure.Services.ImportHandlers;

internal static class DiaperChangeImporter
{
    public static List<DiaperChange> Map(List<JsonDiaper>? source)
    {
        if (source == null) return [];

        var result = new List<DiaperChange>();
        foreach (var item in source)
        {
            if (item.Date > DateTime.UtcNow)
            {
                throw new ImportValidationException(nameof(DiaperChange), item.Pk, 
                    $"Date {item.Date} is in the future.");
            }

            DiaperType type = item.Type switch
            {
                "Wet" => DiaperType.Wet,
                "Soiled" => DiaperType.Soiled,
                "Mixed" => DiaperType.Mixed,
                _ => throw new ArgumentException($"Unknown Diaper Type: {item.Type}")
            };

            result.Add(new DiaperChange
            {
                BabyId = Guid.Empty,
                ExternalId = item.Pk,
                Time = item.Date,
                Type = type
            });
        }
        return result;
    }
}

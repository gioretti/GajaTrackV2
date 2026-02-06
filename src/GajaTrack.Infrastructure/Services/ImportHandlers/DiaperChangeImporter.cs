using GajaTrack.Application.Interfaces;
using GajaTrack.Domain.Entities;

namespace GajaTrack.Infrastructure.Services.ImportHandlers;

internal static class DiaperChangeImporter
{
    public static void Map(List<JsonDiaper>? source, Dictionary<string, DiaperChange> existingEntries, List<DiaperChange> newEntries)
    {
        if (source == null) return;

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
                _ => throw new ImportValidationException(nameof(DiaperChange), item.Pk, $"Unknown Diaper Type: {item.Type}")
            };

            if (existingEntries.TryGetValue(item.Pk, out var existing))
            {
                existing.Update(UtcDateTime.FromDateTime(item.Date), type);
            }
            else
            {
                newEntries.Add(DiaperChange.Create(
                    Guid.Empty,
                    item.Pk,
                    UtcDateTime.FromDateTime(item.Date),
                    type
                ));
            }
        }
    }
}

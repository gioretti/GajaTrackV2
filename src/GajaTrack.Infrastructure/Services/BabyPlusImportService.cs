using System.Text.Json;
using System.Text.Json.Serialization;
using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Infrastructure.Services.ImportHandlers;
using GajaTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GajaTrack.Infrastructure.Services;

public class BabyPlusImportService(GajaDbContext dbContext, ILogger<BabyPlusImportService> logger) : IBabyPlusImportService
{
    public async Task<ImportSummary> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new PolymorphicDateTimeConverter());
        options.Converters.Add(new PolymorphicNullableDateTimeConverter());
        options.Converters.Add(new PolymorphicBoolConverter());
        options.Converters.Add(new PolymorphicStringConverter());
        
        var data = await JsonSerializer.DeserializeAsync<BabyPlusExport>(stream, options, cancellationToken);
        if (data is null) return new ImportSummary(0, 0, 0, 0);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            // 1. Fetch existing entities into dictionaries for O(1) upsert lookup
            var existingNursing = await dbContext.NursingFeeds.ToDictionaryAsync(x => x.ExternalId, cancellationToken);
            var existingBottle = await dbContext.BottleFeeds.ToDictionaryAsync(x => x.ExternalId, cancellationToken);
            var existingSleep = await dbContext.SleepSessions.ToDictionaryAsync(x => x.ExternalId, cancellationToken);
            var existingDiaper = await dbContext.DiaperChanges.ToDictionaryAsync(x => x.ExternalId, cancellationToken);

            var newNursing = new List<NursingFeed>();
            var newBottle = new List<BottleFeed>();
            var newSleep = new List<SleepSession>();
            var newDiaper = new List<DiaperChange>();

            // 2. Process data (DistinctBy PK to handle intra-file duplicates)
            NursingFeedImporter.Map(data.NursingFeeds?.DistinctBy(x => x.Pk).ToList(), existingNursing, newNursing);
            BottleFeedImporter.Map(data.BottleFeeds?.DistinctBy(x => x.Pk).ToList(), existingBottle, newBottle);
            SleepSessionImporter.Map(data.SleepSessions?.DistinctBy(x => x.Pk).ToList(), existingSleep, newSleep);
            DiaperChangeImporter.Map(data.DiaperChanges?.DistinctBy(x => x.Pk).ToList(), existingDiaper, newDiaper);

            // 3. Add only truly new records
            dbContext.NursingFeeds.AddRange(newNursing);
            dbContext.BottleFeeds.AddRange(newBottle);
            dbContext.SleepSessions.AddRange(newSleep);
            dbContext.DiaperChanges.AddRange(newDiaper);

            // 4. Save (EF Core automatically generates UPDATEs for entries in dictionaries if they were modified)
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ImportSummary(
                newNursing.Count,
                newBottle.Count,
                newSleep.Count,
                newDiaper.Count
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Import failed.");
            await transaction.RollbackAsync(cancellationToken);
            dbContext.ChangeTracker.Clear();
            throw;
        }
    }

    private class PolymorphicStringConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String) return reader.GetString();
            if (reader.TokenType == JsonTokenType.Number) return reader.GetInt64().ToString();
            return null;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }

    private class PolymorphicDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return DateTimeOffset.FromUnixTimeSeconds((long)reader.GetDouble()).UtcDateTime;
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParse(reader.GetString(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal, out var date))
                {
                    return date;
                }
            }
            return default;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("o"));
        }
    }

    private class PolymorphicNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                double unixTime = reader.GetDouble();
                if (unixTime == 0) return null;
                return DateTimeOffset.FromUnixTimeSeconds((long)unixTime).UtcDateTime;
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                if (DateTime.TryParse(reader.GetString(), System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AdjustToUniversal, out var date))
                {
                    return date;
                }
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue) writer.WriteStringValue(value.Value.ToString("o"));
            else writer.WriteNullValue();
        }
    }

    private class PolymorphicBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True) return true;
            if (reader.TokenType == JsonTokenType.False) return false;
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32() == 1;
            }
            return false;
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }
}

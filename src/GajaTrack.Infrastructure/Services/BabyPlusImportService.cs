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
    private const int BatchSize = 500; // Increased to 500

    public async Task<ImportSummary> ImportFromStreamAsync(Stream stream, IProgress<string>? progress = null, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new PolymorphicDateTimeConverter());
        options.Converters.Add(new PolymorphicNullableDateTimeConverter());
        options.Converters.Add(new PolymorphicBoolConverter());
        options.Converters.Add(new PolymorphicStringConverter());
        
        progress?.Report("Deserializing JSON data...");
        var data = await JsonSerializer.DeserializeAsync<BabyPlusExport>(stream, options, cancellationToken);
        
        if (data is null) return new ImportSummary(0, 0, 0, 0);

        progress?.Report($"Data loaded. Found: {data.NursingFeeds?.Count ?? 0} Nursing, {data.BottleFeeds?.Count ?? 0} Bottle, {data.SleepSessions?.Count ?? 0} Sleep, {data.DiaperChanges?.Count ?? 0} Diapers.");

        logger.LogInformation("JSON Deserialized. Fetching existing records...");

        // Removed explicit transaction to prevent SQLite locking issues with UI readers.
        // Idempotency logic ensures we can re-run if it fails halfway.
        try
        {
            dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            progress?.Report("Checking for existing records...");
            var existingNursing = await dbContext.NursingFeeds.ToDictionaryAsync(x => x.ExternalId, cancellationToken);
            var existingBottle = await dbContext.BottleFeeds.ToDictionaryAsync(x => x.ExternalId, cancellationToken);
            var existingSleep = await dbContext.SleepSessions.ToDictionaryAsync(x => x.ExternalId, cancellationToken);
            var existingDiaper = await dbContext.DiaperChanges.ToDictionaryAsync(x => x.ExternalId, cancellationToken);

            var newNursing = new List<NursingFeed>();
            var newBottle = new List<BottleFeed>();
            var newSleep = new List<SleepSession>();
            var newDiaper = new List<DiaperChange>();

            progress?.Report("Mapping and validating...");
            NursingFeedImporter.Map(data.NursingFeeds?.DistinctBy(x => x.Pk).ToList(), existingNursing, newNursing);
            BottleFeedImporter.Map(data.BottleFeeds?.DistinctBy(x => x.Pk).ToList(), existingBottle, newBottle);
            SleepSessionImporter.Map(data.SleepSessions?.DistinctBy(x => x.Pk).ToList(), existingSleep, newSleep);
            DiaperChangeImporter.Map(data.DiaperChanges?.DistinctBy(x => x.Pk).ToList(), existingDiaper, newDiaper);

            progress?.Report("Saving new Nursing feeds...");
            await SaveNewEntries(newNursing, dbContext.NursingFeeds, progress, cancellationToken);
            
            progress?.Report("Saving new Bottle feeds...");
            await SaveNewEntries(newBottle, dbContext.BottleFeeds, progress, cancellationToken);
            
            progress?.Report("Saving new Sleep sessions...");
            await SaveNewEntries(newSleep, dbContext.SleepSessions, progress, cancellationToken);
            
            progress?.Report("Saving new Diaper changes...");
            await SaveNewEntries(newDiaper, dbContext.DiaperChanges, progress, cancellationToken);

            progress?.Report("Applying updates...");
            dbContext.ChangeTracker.DetectChanges();
            await dbContext.SaveChangesAsync(cancellationToken);

            progress?.Report("Import successful!");

            return new ImportSummary(newNursing.Count, newBottle.Count, newSleep.Count, newDiaper.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Import failed.");
            dbContext.ChangeTracker.Clear();
            throw;
        }
        finally
        {
            dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }

    private async Task SaveNewEntries<T>(List<T> items, DbSet<T> dbSet, IProgress<string>? progress, CancellationToken ct) where T : class
    {
        int totalSaved = 0;
        for (int i = 0; i < items.Count; i += BatchSize)
        {
            var batch = items.Skip(i).Take(BatchSize).ToList();
            dbSet.AddRange(batch);
            await dbContext.SaveChangesAsync(ct);
            foreach (var item in batch)
            {
                dbContext.Entry(item).State = EntityState.Detached;
            }
            totalSaved += batch.Count;
            progress?.Report($"Saving {typeof(T).Name}s... {totalSaved}/{items.Count}");
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

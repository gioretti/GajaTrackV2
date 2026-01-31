using System.Text.Json;
using System.Text.Json.Serialization;
using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Infrastructure.Services.ImportHandlers;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Infrastructure.Services;

public class BabyPlusImportService(GajaDbContext dbContext) : IBabyPlusImportService
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
            var nursingFeeds = NursingFeedImporter.Map(data.NursingFeeds);
            var bottleFeeds = BottleFeedImporter.Map(data.BottleFeeds);
            var sleepSessions = SleepSessionImporter.Map(data.SleepSessions);
            var diaperChanges = DiaperChangeImporter.Map(data.DiaperChanges);

            var existingNursingIds = await dbContext.NursingFeeds.Select(x => x.ExternalId).ToListAsync(cancellationToken);
            var existingBottleIds = await dbContext.BottleFeeds.Select(x => x.ExternalId).ToListAsync(cancellationToken);
            var existingSleepIds = await dbContext.SleepSessions.Select(x => x.ExternalId).ToListAsync(cancellationToken);
            var existingDiaperIds = await dbContext.DiaperChanges.Select(x => x.ExternalId).ToListAsync(cancellationToken);

            var newNursing = nursingFeeds.DistinctBy(x => x.ExternalId).Where(x => !existingNursingIds.Contains(x.ExternalId)).ToList();
            var newBottle = bottleFeeds.DistinctBy(x => x.ExternalId).Where(x => !existingBottleIds.Contains(x.ExternalId)).ToList();
            var newSleep = sleepSessions.DistinctBy(x => x.ExternalId).Where(x => !existingSleepIds.Contains(x.ExternalId)).ToList();
            var newDiaper = diaperChanges.DistinctBy(x => x.ExternalId).Where(x => !existingDiaperIds.Contains(x.ExternalId)).ToList();

            dbContext.NursingFeeds.AddRange(newNursing);
            dbContext.BottleFeeds.AddRange(newBottle);
            dbContext.SleepSessions.AddRange(newSleep);
            dbContext.DiaperChanges.AddRange(newDiaper);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ImportSummary(
                newNursing.Count,
                newBottle.Count,
                newSleep.Count,
                newDiaper.Count
            );
        }
        catch
        {
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

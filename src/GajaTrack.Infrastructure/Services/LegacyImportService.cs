using System.Text.Json;
using System.Text.Json.Serialization;
using GajaTrack.Application.Interfaces;
using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Infrastructure.Services.ImportHandlers;
using Microsoft.EntityFrameworkCore;

namespace GajaTrack.Infrastructure.Services;

public class LegacyImportService(GajaDbContext dbContext) : ILegacyImportService
{
    public async Task<ImportSummary> ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new PolymorphicDateTimeConverter());
        options.Converters.Add(new PolymorphicNullableDateTimeConverter());
        options.Converters.Add(new PolymorphicBoolConverter());
        
        var data = await JsonSerializer.DeserializeAsync<BabyPlusExport>(stream, options, cancellationToken);
        
        if (data is null) return new ImportSummary(0, 0, 0, 0);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var nursingFeeds = NursingFeedImporter.Map(data.NursingFeeds);
            dbContext.NursingFeeds.AddRange(nursingFeeds);

            var bottleFeeds = BottleFeedImporter.Map(data.BottleFeeds);
            dbContext.BottleFeeds.AddRange(bottleFeeds);

            var sleepSessions = SleepSessionImporter.Map(data.SleepSessions);
            dbContext.SleepSessions.AddRange(sleepSessions);

            var diaperChanges = DiaperChangeImporter.Map(data.DiaperChanges);
            dbContext.DiaperChanges.AddRange(diaperChanges);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ImportSummary(
                nursingFeeds.Count,
                bottleFeeds.Count,
                sleepSessions.Count,
                diaperChanges.Count
            );
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
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
using System.Net.Http.Json;
using GajaTrack.Infrastructure.Persistence;
using GajaTrack.Application.DTOs.Import;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;

namespace GajaTrack.Web.Api;

public class ImportApiTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ImportApiTest(WebApplicationFactory<Program> factory)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing registrations
                var descriptors = services.Where(d => 
                    d.ServiceType == typeof(DbContextOptions<GajaDbContext>) || 
                    d.ServiceType == typeof(IDbContextFactory<GajaDbContext>)).ToList();
                
                foreach (var d in descriptors) services.Remove(d);

                services.AddDbContextFactory<GajaDbContext>(options => options.UseSqlite(connection));
                services.AddScoped(p => p.GetRequiredService<IDbContextFactory<GajaDbContext>>().CreateDbContext());
            });
        });
    }

    [Fact]
    public async Task PostImport_BabyPlus_ReturnsCount()
    {
        // Arrange
        var client = _factory.CreateClient();
        var json = await File.ReadAllTextAsync("Fixtures/import_ios_sample.json");
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync("/api/import/babyplus", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ImportSummary>();
        Assert.NotNull(result);
        
        var totalImported = result.NursingFeedsImported + result.BottleFeedsImported + result.SleepSessionsImported + result.DiaperChangesImported + result.CryingSessionsImported;
        Assert.True(totalImported > 0);

        // Verify data integrity
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<GajaDbContext>();
            Assert.Equal(result.NursingFeedsImported, await context.NursingFeeds.CountAsync());
            Assert.Equal(result.BottleFeedsImported, await context.BottleFeeds.CountAsync());
            Assert.Equal(result.SleepSessionsImported, await context.SleepSessions.CountAsync());
            Assert.Equal(result.DiaperChangesImported, await context.DiaperChanges.CountAsync());
            Assert.Equal(result.CryingSessionsImported, await context.CryingSessions.CountAsync());
        }
    }
}

using System.Net;
using System.Net.Http.Json;
using GajaTrack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using GajaTrack.Domain.Entities;
using GajaTrack.Application.Interfaces;

namespace GajaTrack.IntegrationTest.Api;

public class StatsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public StatsApiTests(WebApplicationFactory<Program> factory)
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
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
    public async Task GetStats_ReturnsCorrectCounts()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GajaDbContext>();
            db.Database.EnsureCreated();
            
            db.NursingFeeds.Add(NursingFeed.Create(Guid.NewGuid(), "n1", UtcDateTime.Now(), null));
            db.BottleFeeds.Add(BottleFeed.Create(Guid.NewGuid(), "b1", UtcDateTime.Now(), 100, BottleContent.Formula));
            await db.SaveChangesAsync();
        }

        var client = _factory.CreateClient();

        // Act
        var stats = await client.GetFromJsonAsync<TrackingStats>("/api/stats");

        // Assert
        Assert.NotNull(stats);
        Assert.Equal(1, stats.NursingCount);
        Assert.Equal(1, stats.BottleCount);
        Assert.Equal(0, stats.SleepCount);
    }
}

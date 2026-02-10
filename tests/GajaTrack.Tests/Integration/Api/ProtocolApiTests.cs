using System.Net.Http.Json;
using GajaTrack.Application.DTOs.Protocol;
using GajaTrack.Domain.Entities;
using GajaTrack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;

namespace GajaTrack.Tests.Integration.Api;

public class ProtocolApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProtocolApiTests(WebApplicationFactory<Program> factory)
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
    public async Task GetProtocol_ReturnsData()
    {
        // Arrange
        var client = _factory.CreateClient();
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<GajaDbContext>();
            
            var day = new DateOnly(2026, 2, 5);
            var sleepStart = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 10, 0, 0, DateTimeKind.Utc));
            var sleepEnd = UtcDateTime.FromDateTime(new DateTime(2026, 2, 5, 12, 0, 0, DateTimeKind.Utc));
            context.SleepSessions.Add(SleepSession.Create(Guid.NewGuid(), "test", sleepStart, sleepEnd));
            await context.SaveChangesAsync();
        }

        var testDay = new DateOnly(2026, 2, 5);

        // Act
        var response = await client.GetAsync($"/api/protocol?startDate={testDay:yyyy-MM-dd}&endDate={testDay:yyyy-MM-dd}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<ProtocolDay>>();
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(testDay, result[0].Date);
        Assert.Single(result[0].Events);
        Assert.Equal(120, result[0].Events[0].DurationMinutes);
    }

    [Fact]
    public async Task GetProtocol_RangeReturnsMultipleDays()
    {
        // Arrange
        var client = _factory.CreateClient();
        var startDay = new DateOnly(2026, 2, 5);
        var endDay = new DateOnly(2026, 2, 6);

        // Act
        var response = await client.GetAsync($"/api/protocol?startDate={startDay:yyyy-MM-dd}&endDate={endDay:yyyy-MM-dd}");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<List<ProtocolDay>>();
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(startDay, result[0].Date);
        Assert.Equal(endDay, result[1].Date);
    }
}

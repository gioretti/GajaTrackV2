using System.Net;
using GajaTrack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using GajaTrack.Domain.Entities;

namespace GajaTrack.RestApi.Api;

public class ExportApiTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ExportApiTest(WebApplicationFactory<Program> factory)
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
    public async Task GetExport_ReturnsJsonFile()
    {
        // Arrange
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<GajaDbContext>();
            db.Database.EnsureCreated();
            
            // Add some data
            db.NursingFeeds.Add(NursingFeed.Create(
                Guid.NewGuid(), 
                "ext1", 
                UtcDateTime.Now(), 
                null
            ));
            await db.SaveChangesAsync();
        }

        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/export");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("nursingFeeds", content);
        Assert.Contains("bottleFeeds", content);
    }
}


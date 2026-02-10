using GajaTrack.Infrastructure;
using GajaTrack.Application;
using GajaTrack.Web.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextFactory<GajaTrack.Infrastructure.Persistence.GajaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Also register the scoped DbContext for normal injection (it resolves from the factory)
builder.Services.AddScoped(p => p.GetRequiredService<IDbContextFactory<GajaTrack.Infrastructure.Persistence.GajaDbContext>>().CreateDbContext());

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options => 
    {
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(10);
    })
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddServerSideBlazor()
    .AddHubOptions(options => 
    {
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
    });

var app = builder.Build();

// Auto-migrate database on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GajaTrack.Infrastructure.Persistence.GajaDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

var api = app.MapGroup("/api");

api.MapGet("/protocol", async (DateOnly startDate, DateOnly endDate, GajaTrack.Application.Interfaces.IProtocolService protocolService) =>
{
    var result = await protocolService.GetProtocolAsync(startDate, endDate);
    return Results.Ok(result);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GajaTrack.Web.Client._Imports).Assembly);

app.Run();

public partial class Program { }

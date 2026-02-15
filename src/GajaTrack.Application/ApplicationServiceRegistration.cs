using GajaTrack.Application.Interfaces;
using GajaTrack.Application.Queries;
using GajaTrack.Application.Services;
using GajaTrack.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace GajaTrack.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IDailyRhythmMapService, DailyRhythmMapService>();
        
        // BabyDay Services (KISS: No interfaces needed for internal domain logic)
        services.AddScoped<CalculateSleep>();
        services.AddScoped<CountWakings>();
        services.AddScoped<GetBabyDay.Execution>();
        
        return services;
    }
}

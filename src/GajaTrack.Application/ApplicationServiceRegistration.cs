using GajaTrack.Application.Interfaces;
using GajaTrack.Application.Services;
using GajaTrack.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GajaTrack.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<DailyRhythmMapDomainService>();
        services.AddScoped<IDailyRhythmMapService, DailyRhythmMapService>();
        return services;
    }
}

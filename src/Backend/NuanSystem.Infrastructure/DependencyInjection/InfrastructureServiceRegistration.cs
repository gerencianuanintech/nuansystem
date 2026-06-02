using Microsoft.Extensions.DependencyInjection;
using NuanSystem.Application.Abstractions.Authentication;
using NuanSystem.Application.Abstractions.Geography;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Infrastructure.Authentication;
using NuanSystem.Infrastructure.Geography;
using NuanSystem.Infrastructure.Security;

namespace NuanSystem.Infrastructure.DependencyInjection;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<ISecretProtector, AesSecretProtector>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddHttpClient<GoogleMapsGeographyService>();
        services.AddScoped<IReverseGeocodingService>(provider =>
            provider.GetRequiredService<GoogleMapsGeographyService>());
        services.AddScoped<IStaticMapService>(provider =>
            provider.GetRequiredService<GoogleMapsGeographyService>());

        return services;
    }
}

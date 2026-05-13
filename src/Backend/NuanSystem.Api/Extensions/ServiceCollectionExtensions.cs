using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NuanSystem.Infrastructure.Authentication;
using NuanSystem.Application.DependencyInjection;
using NuanSystem.Infrastructure.DependencyInjection;
using NuanSystem.Persistence.DependencyInjection;
using NuanSystem.SapIntegration.DependencyInjection;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNuanSystemServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHttpContextAccessor();

        var jwtOptions = ReadJwtOptions(configuration);
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddAuthorization(options =>
        {
            foreach (var permission in PermissionCodes.All)
            {
                options.AddPolicy(permission, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("permission", permission);
                });
            }
        });
        services.AddHealthChecks();

        services
            .AddApplicationServices()
            .AddInfrastructureServices()
            .AddPersistenceServices(configuration)
            .AddSapIntegrationServices();

        return services;
    }

    private static JwtOptions ReadJwtOptions(IConfiguration configuration)
    {
        var options = new JwtOptions
        {
            Issuer = configuration[$"{JwtOptions.SectionName}:Issuer"] ?? "NuanSystem",
            Audience = configuration[$"{JwtOptions.SectionName}:Audience"] ?? "NuanSystem.Client",
            SigningKey = configuration[$"{JwtOptions.SectionName}:SigningKey"] ?? string.Empty
        };

        if (int.TryParse(configuration[$"{JwtOptions.SectionName}:ExpirationMinutes"], out var minutes))
        {
            options.ExpirationMinutes = minutes;
        }

        if (string.IsNullOrWhiteSpace(options.SigningKey) || options.SigningKey.Length < 32)
        {
            throw new InvalidOperationException("Jwt:SigningKey debe tener al menos 32 caracteres.");
        }

        return options;
    }
}

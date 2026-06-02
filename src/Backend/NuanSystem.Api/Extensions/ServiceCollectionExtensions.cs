using System.Text;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using NuanSystem.Infrastructure.Authentication;
using NuanSystem.Application.Abstractions.Authentication;
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
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var userIdValue = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                        var tokenStamp = context.Principal?.FindFirstValue(AuthClaimNames.SecurityStamp);
                        if (!int.TryParse(userIdValue, out var userId) || string.IsNullOrWhiteSpace(tokenStamp))
                        {
                            context.Fail("El token no contiene el estado de seguridad requerido.");
                            return;
                        }

                        var securityState = context.HttpContext.RequestServices.GetRequiredService<IUserSecurityStateService>();
                        var currentStamp = await securityState.GetSecurityStampAsync(userId, context.HttpContext.RequestAborted);
                        if (!string.Equals(tokenStamp, currentStamp, StringComparison.Ordinal))
                        {
                            context.Fail("El estado de seguridad del usuario cambio.");
                        }
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            foreach (var permission in PermissionCodes.All)
            {
                options.AddPolicy(permission, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim(AuthClaimNames.Permission, permission);
                });
            }
        });
        services.AddHealthChecks();
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("auth-login", httpContext =>
            {
                var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(
                    $"auth-login:{ipAddress}",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
        });

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

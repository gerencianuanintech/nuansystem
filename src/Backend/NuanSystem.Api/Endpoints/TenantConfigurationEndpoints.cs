using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Tenancy;

namespace NuanSystem.Api.Endpoints;

public static class TenantConfigurationEndpoints
{
    public static IEndpointRouteBuilder MapTenantConfigurationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tenant-configuration/features", async (
            ITenantFeatureService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetActiveCompanyFeaturesAsync(cancellationToken);
            return result.ToHttpResult();
        })
        .RequireAuthorization();

        app.MapGet("/api/tenant-configuration/integrations", async (
            ITenantIntegrationService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetActiveCompanyIntegrationsAsync(cancellationToken);
            return result.ToHttpResult();
        })
        .RequireAuthorization();

        app.MapGet("/api/tenant-configuration/ownership", async (
            IEntityOwnershipService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetActiveCompanyOwnershipAsync(cancellationToken);
            return result.ToHttpResult();
        })
        .RequireAuthorization();

        app.MapGet("/api/tenant-configuration/ownership/{entityName}", async (
            string entityName,
            IEntityOwnershipService service,
            CancellationToken cancellationToken) =>
        {
            var result = await service.GetActiveCompanyOwnershipAsync(entityName, cancellationToken);
            return result.ToHttpResult();
        })
        .RequireAuthorization();

        return app;
    }
}


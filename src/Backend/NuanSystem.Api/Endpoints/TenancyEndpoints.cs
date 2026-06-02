using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class TenancyEndpoints
{
    public static IEndpointRouteBuilder MapTenancyEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tenancy/current", (NuanSystem.Application.Abstractions.Tenancy.ICompanyContext companyContext) =>
        {
            if (!companyContext.HasActiveCompany)
            {
                return Results.BadRequest(ApiResponse<object>.Fail("No hay empresa activa."));
            }

            return Results.Ok(ApiResponse<object>.Ok(new
            {
                companyContext.CurrentCompany!.CompanyId,
                companyContext.CurrentCompany.CompanyCode,
                companyContext.CurrentCompany.CommercialName,
                DatabaseEngine = companyContext.CurrentCompany.DatabaseEngine.ToString(),
                SapIntegrationMode = companyContext.CurrentCompany.SapIntegrationMode.ToString()
            }));
        })
        .RequirePermission(PermissionCodes.BusinessPartnersRead);

        app.MapPost("/api/tenancy/initialize-database", async (
            ITenantDatabaseInitializer initializer,
            CancellationToken cancellationToken) =>
        {
            await initializer.InitializeCurrentTenantAsync(cancellationToken);

            return Results.Ok(ApiResponse<object>.Ok(new
            {
                Initialized = true
            }, "Base de datos tenant validada correctamente."));
        })
        .RequirePermission(PermissionCodes.BusinessPartnersRead);

        return app;
    }
}

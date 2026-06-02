using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.TaxCatalogs.Catalogs.Commands;
using NuanSystem.Application.Features.TaxCatalogs.Catalogs.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class TaxCatalogEndpoints
{
    public static IEndpointRouteBuilder MapTaxCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        MapCatalog(app, "tax-regimes");
        MapCatalog(app, "taxpayer-types");
        MapCatalog(app, "retention-types");
        MapCatalog(app, "tax-supports");
        MapRetentionConcepts(app);

        return app;
    }

    private static void MapCatalog(IEndpointRouteBuilder app, string catalogKey)
    {
        var route = $"/api/tax-catalogs/{catalogKey}";
        var permissions = GetPermissions(catalogKey);

        app.MapGet(route, async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetTaxCatalogsQuery(catalogKey), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Read);

        app.MapGet($"{route}/lookup", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetTaxCatalogLookupQuery(catalogKey), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Read);

        app.MapGet($"{route}/{{id:int}}", async (int id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetTaxCatalogByIdQuery(catalogKey, id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Read);

        app.MapPost(route, async (
            SaveTaxCatalogRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new CreateTaxCatalogCommand(catalogKey, request.Code, request.Name, request.Description, request.IsActive, auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Manage);

        app.MapPut($"{route}/{{id:int}}", async (
            int id,
            SaveTaxCatalogRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new UpdateTaxCatalogCommand(catalogKey, id, request.Code, request.Name, request.Description, request.IsActive, auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Manage);

        app.MapDelete($"{route}/{{id:int}}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteTaxCatalogCommand(catalogKey, id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Manage);
    }

    private static void MapRetentionConcepts(IEndpointRouteBuilder app)
    {
        const string route = "/api/tax-catalogs/retention-concepts";
        var permissions = GetPermissions("retention-concepts");

        app.MapGet(route, async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetRetentionConceptsQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Read);

        app.MapGet($"{route}/lookup", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetRetentionConceptLookupQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Read);

        app.MapGet($"{route}/{{id:int}}", async (int id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetRetentionConceptByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Read);

        app.MapPost(route, async (
            SaveRetentionConceptRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new CreateRetentionConceptCommand(
                    request.Code,
                    request.Name,
                    request.Description,
                    request.RetentionTypeId,
                    request.SriCode,
                    request.Percent,
                    request.AppliesIva,
                    request.AppliesIncome,
                    request.IsActive,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Manage);

        app.MapPut($"{route}/{{id:int}}", async (
            int id,
            SaveRetentionConceptRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new UpdateRetentionConceptCommand(
                    id,
                    request.Code,
                    request.Name,
                    request.Description,
                    request.RetentionTypeId,
                    request.SriCode,
                    request.Percent,
                    request.AppliesIva,
                    request.AppliesIncome,
                    request.IsActive,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Manage);

        app.MapDelete($"{route}/{{id:int}}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteTaxCatalogCommand("retention-concepts", id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(permissions.Manage);
    }

    private static CatalogPermissions GetPermissions(string catalogKey)
    {
        return catalogKey switch
        {
            "tax-regimes" => new(PermissionCodes.TaxRegimesRead, PermissionCodes.TaxRegimesManage),
            "taxpayer-types" => new(PermissionCodes.TaxpayerTypesRead, PermissionCodes.TaxpayerTypesManage),
            "retention-types" => new(PermissionCodes.RetentionTypesRead, PermissionCodes.RetentionTypesManage),
            "retention-concepts" => new(PermissionCodes.RetentionConceptsRead, PermissionCodes.RetentionConceptsManage),
            "tax-supports" => new(PermissionCodes.TaxSupportsRead, PermissionCodes.TaxSupportsManage),
            _ => throw new InvalidOperationException($"Tax catalog '{catalogKey}' is not configured.")
        };
    }

    private sealed record SaveTaxCatalogRequest(string Code, string Name, string? Description, bool IsActive);

    private sealed record SaveRetentionConceptRequest(
        string Code,
        string Name,
        string? Description,
        int? RetentionTypeId,
        string? SriCode,
        decimal Percent,
        bool AppliesIva,
        bool AppliesIncome,
        bool IsActive);

    private sealed record CatalogPermissions(string Read, string Manage);
}

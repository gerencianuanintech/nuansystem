using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Commands;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class GeneralSupplierEndpoints
{
    public static IEndpointRouteBuilder MapGeneralSupplierEndpoints(this IEndpointRouteBuilder app)
    {
        MapCatalog(
            app,
            "supplier-groups",
            PermissionCodes.GeneralSupplierSupplierGroupsRead,
            PermissionCodes.GeneralSupplierSupplierGroupsManage);
        MapCatalog(
            app,
            "supplier-classes",
            PermissionCodes.GeneralSupplierSupplierClassesRead,
            PermissionCodes.GeneralSupplierSupplierClassesManage);
        MapCatalog(
            app,
            "economic-activities",
            PermissionCodes.GeneralSupplierEconomicActivitiesRead,
            PermissionCodes.GeneralSupplierEconomicActivitiesManage);
        MapCatalog(
            app,
            "zones",
            PermissionCodes.GeneralSupplierZonesRead,
            PermissionCodes.GeneralSupplierZonesManage);
        MapCatalog(
            app,
            "supply-methods",
            PermissionCodes.GeneralSupplierSupplyMethodsRead,
            PermissionCodes.GeneralSupplierSupplyMethodsManage);
        MapCatalog(
            app,
            "contact-types",
            PermissionCodes.GeneralSupplierContactTypesRead,
            PermissionCodes.GeneralSupplierContactTypesManage);
        MapCatalog(
            app,
            "contact-channels",
            PermissionCodes.GeneralSupplierContactChannelsRead,
            PermissionCodes.GeneralSupplierContactChannelsManage);

        return app;
    }

    private static void MapCatalog(
        IEndpointRouteBuilder app,
        string catalogKey,
        string readPermission,
        string managePermission)
    {
        var route = $"/api/general-supplier/{catalogKey}";

        app.MapGet(route, async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetGeneralSupplierCatalogsQuery(catalogKey), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(readPermission);

        app.MapGet($"{route}/lookup", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetGeneralSupplierCatalogLookupQuery(catalogKey), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(readPermission);

        app.MapGet($"{route}/{{id:int}}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetGeneralSupplierCatalogByIdQuery(catalogKey, id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(readPermission);

        app.MapPost(route, async (
            SaveGeneralSupplierCatalogRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new CreateGeneralSupplierCatalogCommand(
                    catalogKey,
                    request.Code,
                    request.Name,
                    request.Description,
                    request.IsActive,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(managePermission);

        app.MapPut($"{route}/{{id:int}}", async (
            int id,
            SaveGeneralSupplierCatalogRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new UpdateGeneralSupplierCatalogCommand(
                    catalogKey,
                    id,
                    request.Code,
                    request.Name,
                    request.Description,
                    request.IsActive,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(managePermission);

        app.MapDelete($"{route}/{{id:int}}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new DeleteGeneralSupplierCatalogCommand(catalogKey, id, auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(managePermission);
    }

    private sealed record SaveGeneralSupplierCatalogRequest(
        string Code,
        string Name,
        string? Description,
        bool IsActive);
}


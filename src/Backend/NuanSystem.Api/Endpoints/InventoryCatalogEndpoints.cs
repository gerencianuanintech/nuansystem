using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ItemTypes;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ItemGroups;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ItemFamilies;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ItemBrands;
using NuanSystem.Api.Endpoints.Definitions.Inventory.UnitMeasures;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ProductTypes;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ItemLines;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ItemSubgroups;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ItemOrigins;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ItemCommercialSegments;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ItemAlertTypes;
using NuanSystem.Api.Endpoints.Definitions.Inventory.SalesChannels;
using NuanSystem.Api.Endpoints.Definitions.Inventory.ReplenishmentMethods;
using NuanSystem.Api.Endpoints.Definitions.Inventory.StorageConditions;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Commands;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Queries;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Queries;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Commands;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Queries;
using NuanSystem.Application.Features.Items.Commands;
using NuanSystem.Application.Features.Items.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class InventoryCatalogEndpoints
{
    public static IEndpointRouteBuilder MapInventoryCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapItemTypeEndpoints();
        app.MapItemGroupEndpoints();
        app.MapItemFamilyEndpoints();
        app.MapItemBrandEndpoints();
        app.MapUnitMeasureEndpoints();
        app.MapProductTypeEndpoints();
        app.MapItemLineEndpoints();
        app.MapItemSubgroupEndpoints();
        app.MapItemOriginEndpoints();
        app.MapItemCommercialSegmentEndpoints();
        app.MapItemAlertTypeEndpoints();
        app.MapSalesChannelEndpoints();
        app.MapReplenishmentMethodEndpoints();
        app.MapStorageConditionEndpoints();
        MapGeneralInventoryCatalog(
            app,
            "warehouses",
            PermissionCodes.GeneralInventoryWarehousesRead,
            PermissionCodes.GeneralInventoryWarehousesManage);
        MapGeneralInventoryCatalog(
            app,
            "warehouse-locations",
            PermissionCodes.GeneralInventoryWarehouseLocationsRead,
            PermissionCodes.GeneralInventoryWarehouseLocationsManage);
        MapGeneralInventoryCatalog(
            app,
            "storage-zones",
            PermissionCodes.GeneralInventoryStorageZonesRead,
            PermissionCodes.GeneralInventoryStorageZonesManage);
        MapGeneralInventoryCatalog(
            app,
            "variant-attributes",
            PermissionCodes.GeneralInventoryVariantAttributesRead,
            PermissionCodes.GeneralInventoryVariantAttributesManage);
        MapGeneralInventoryCatalog(
            app,
            "attachment-document-types",
            PermissionCodes.GeneralInventoryAttachmentDocumentTypesRead,
            PermissionCodes.GeneralInventoryAttachmentDocumentTypesManage);
        MapGeneralInventoryCatalog(
            app,
            "attachment-categories",
            PermissionCodes.GeneralInventoryAttachmentCategoriesRead,
            PermissionCodes.GeneralInventoryAttachmentCategoriesManage);

        app.MapGet("/api/warehouses", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetWarehousesQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.GeneralInventoryWarehousesRead);

        app.MapGet("/api/warehouses/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetWarehouseByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.GeneralInventoryWarehousesRead);

        app.MapPost("/api/warehouses", async (
            CreateWarehouseCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.GeneralInventoryWarehousesManage);

        app.MapPut("/api/warehouses/{id:int}", async (
            int id,
            UpdateWarehouseCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.GeneralInventoryWarehousesManage);

        app.MapPatch("/api/warehouses/{id:int}/active", async (
            int id,
            SetWarehouseActiveStatusRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new SetWarehouseActiveStatusCommand(id, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.GeneralInventoryWarehousesManage);

        app.MapDelete("/api/warehouses/{id:int}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteWarehouseCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.GeneralInventoryWarehousesManage);

        app.MapGet("/api/items", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetItemsQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsRead);

        app.MapGet("/api/items/lookups", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetItemLookupsQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsRead);

        app.MapGet("/api/items/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetItemByIdQuery(id), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsRead);

        app.MapPost("/api/items", async (
            CreateItemCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsManage);

        app.MapPut("/api/items/{id:int}", async (
            int id,
            UpdateItemCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsManage);

        app.MapDelete("/api/items/{id:int}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteItemCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsManage);

        return app;
    }

    private static void MapGeneralInventoryCatalog(
        IEndpointRouteBuilder app,
        string catalogKey,
        string readPermission,
        string managePermission)
    {
        var route = $"/api/general-inventory/{catalogKey}";

        app.MapGet(route, async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetGeneralInventoryCatalogsQuery(catalogKey), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(readPermission);

        app.MapGet($"{route}/lookup", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetGeneralInventoryCatalogLookupQuery(catalogKey), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(readPermission);

        app.MapGet($"{route}/{{id:int}}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetGeneralInventoryCatalogByIdQuery(catalogKey, id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(readPermission);

        app.MapPost(route, async (
            SaveGeneralInventoryCatalogRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new CreateGeneralInventoryCatalogCommand(
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
            SaveGeneralInventoryCatalogRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new UpdateGeneralInventoryCatalogCommand(
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
                new DeleteGeneralInventoryCatalogCommand(catalogKey, id, auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(managePermission);
    }

    private sealed record SaveGeneralInventoryCatalogRequest(
        string Code,
        string Name,
        string? Description,
        bool IsActive);

    private sealed record SetWarehouseActiveStatusRequest(bool IsActive);
}

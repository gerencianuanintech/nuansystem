using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.SapSync.Commands;
using NuanSystem.Application.Features.SapSync.Queries;
using NuanSystem.Application.Features.SapSync.Countries.Commands;
using NuanSystem.Application.Features.SapSync.Countries.Queries;
using NuanSystem.Application.Features.SapSync.Provinces.Commands;
using NuanSystem.Application.Features.SapSync.Provinces.Queries;
using NuanSystem.Application.Features.SapSync.Cities.Commands;
using NuanSystem.Application.Features.SapSync.Cities.Configuration;
using NuanSystem.Application.Features.SapSync.Cities.Queries;
using NuanSystem.Application.Features.Sync.Commands;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SapEndpoints
{
    public static IEndpointRouteBuilder MapSapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sap/sync-logs", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSapSyncLogsQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapRead);

        app.MapGet("/api/sap/settings/service-layer", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSapServiceLayerSettingsQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapPost("/api/sap/purchase-orders/{id:int}/route", async (int id,ISender sender,ClaimsPrincipal user,CancellationToken cancellationToken) =>
        {
            var audit=EndpointContextHelper.GetAuditUser(user);
            return (await sender.Send(new RoutePurchaseOrderCommand(id,audit.UserId,audit.UserName),cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.SapManage);

        app.MapPost("/api/sap/purchase-orders/{id:int}/approve-route", async (int id,ApprovePurchaseOrderRouteRequest request,ISender sender,ClaimsPrincipal user,CancellationToken cancellationToken) =>
        {
            var audit=EndpointContextHelper.GetAuditUser(user);
            return (await sender.Send(new ApprovePurchaseOrderRouteCommand(id,request.BranchCompanyId,request.Reason,audit.UserId,audit.UserName),cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.SapManage);

        app.MapPut("/api/sap/settings/service-layer", async (
            UpdateSapServiceLayerSettingsCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapGet("/api/sap/catalog-mappings", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSapCatalogMappingsQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapRead);

        app.MapPut("/api/sap/catalog-mappings", async (
            ReplaceSapCatalogMappingsCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapGet("/api/sap/suppliers/preview", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new PreviewSuppliersFromSapQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapRead);

        app.MapPost("/api/sap/suppliers/import", async (
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new ImportSuppliersFromSapCommand(auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapGet("/api/sap/warehouses/preview", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new PreviewWarehousesFromSapQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapRead);

        app.MapPost("/api/sap/warehouses/import", async (
            ImportWarehousesFromSapCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapGet("/api/sap/countries/preview", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new PreviewCountriesFromSapQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapRead);

        app.MapPost("/api/sap/countries/import", async (
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new ImportCountriesFromSapCommand(auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapGet("/api/sap/provinces/preview", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new PreviewProvincesFromSapQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapRead);

        app.MapPost("/api/sap/provinces/import", async (
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new ImportProvincesFromSapCommand(auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapGet("/api/sap/settings/cities-query", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSapCityQuerySettingsQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapPut("/api/sap/settings/cities-query", async (
            UpdateSapCityQuerySettingsCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapGet("/api/sap/cities/preview", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new PreviewCitiesFromSapQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapRead);

        app.MapPost("/api/sap/cities/import", async (
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new ImportCitiesFromSapCommand(auditUser.UserId, auditUser.UserName),
                cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapGet("/api/sap/items/preview", async (
            int? take,
            string? search,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new PreviewItemsFromSapQuery(take ?? 200, search), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapRead);

        app.MapPost("/api/sap/items/import", async (
            ImportItemsFromSapCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        app.MapPost("/api/sap/purchase-orders/import", async (
            ImportPurchaseOrdersFromSapCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with { AuditUserId=auditUser.UserId, AuditUserName=auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        return app;
    }
}

public sealed record ApprovePurchaseOrderRouteRequest(int BranchCompanyId,string Reason);

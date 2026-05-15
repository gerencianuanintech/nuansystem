using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Queries;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Queries;
using NuanSystem.Application.Features.Items.Commands;
using NuanSystem.Application.Features.Items.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class InventoryCatalogEndpoints
{
    public static IEndpointRouteBuilder MapInventoryCatalogEndpoints(this IEndpointRouteBuilder app)
    {
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

        app.MapGet("/api/item-groups", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetItemGroupsQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsRead);

        app.MapGet("/api/item-groups/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetItemGroupByIdQuery(id), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsRead);

        app.MapPost("/api/item-groups", async (
            CreateItemGroupCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsManage);

        app.MapPut("/api/item-groups/{id:int}", async (
            int id,
            UpdateItemGroupCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsManage);

        app.MapDelete("/api/item-groups/{id:int}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteItemGroupCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsManage);

        app.MapGet("/api/item-families", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetItemFamiliesQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsRead);

        app.MapGet("/api/item-families/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetItemFamilyByIdQuery(id), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsRead);

        app.MapGet("/api/item-families/by-group/{itemGroupId:int}", async (
            int itemGroupId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetItemFamiliesByGroupQuery(itemGroupId), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsRead);

        app.MapPost("/api/item-families", async (
            CreateItemFamilyCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsManage);

        app.MapPut("/api/item-families/{id:int}", async (
            int id,
            UpdateItemFamilyCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsManage);

        app.MapDelete("/api/item-families/{id:int}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteItemFamilyCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.ItemsManage);

        return app;
    }
}

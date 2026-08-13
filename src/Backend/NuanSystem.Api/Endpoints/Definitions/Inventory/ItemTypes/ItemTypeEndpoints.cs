using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ItemTypes;

public static class ItemTypeEndpoints
{
    private const string FormKey = "inventory-item-types";

    public static IEndpointRouteBuilder MapItemTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/general-inventory/item-types");

        group.MapGet("", async (ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetItemTypesQuery(), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemTypesRead)
            .RequireFormOperation(FormKey, "refresh");

        group.MapGet("/lookup", async (ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetItemTypeLookupQuery(), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemTypesRead);

        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetItemTypeByIdQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemTypesRead)
            .RequireFormOperation(FormKey, "consult");

        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetItemTypeHistoryQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemTypesRead)
            .RequireFormOperation(FormKey, "history");

        group.MapPost("", async (
            SaveItemTypeRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var command = new CreateItemTypeCommand(
                request.Code,
                request.Name,
                request.Description,
                request.BehaviorCode,
                request.DefaultIsPurchaseItem,
                request.DefaultIsSalesItem,
                request.DefaultIsInventoryItem,
                request.SortOrder,
                request.IsActive,
                auditUser.UserId,
                auditUser.UserName);
            return (await sender.Send(command, cancellationToken)).ToHttpResult();
        })
        .RequirePermission(PermissionCodes.GeneralInventoryItemTypesManage)
        .RequireFormOperation(FormKey, "create");

        group.MapPut("/{id:int}", async (
            int id,
            SaveItemTypeRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var command = new UpdateItemTypeCommand(
                id,
                request.Code,
                request.Name,
                request.Description,
                request.BehaviorCode,
                request.DefaultIsPurchaseItem,
                request.DefaultIsSalesItem,
                request.DefaultIsInventoryItem,
                request.SortOrder,
                request.IsActive,
                auditUser.UserId,
                auditUser.UserName);
            return (await sender.Send(command, cancellationToken)).ToHttpResult();
        })
        .RequirePermission(PermissionCodes.GeneralInventoryItemTypesManage)
        .RequireFormOperation(FormKey, "update");

        group.MapDelete("/{id:int}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(
                new DeleteItemTypeCommand(id, auditUser.UserId, auditUser.UserName),
                cancellationToken)).ToHttpResult();
        })
        .RequirePermission(PermissionCodes.GeneralInventoryItemTypesManage)
        .RequireFormOperation(FormKey, "delete");

        return app;
    }

    private sealed record SaveItemTypeRequest(
        string Code,
        string Name,
        string? Description,
        string BehaviorCode,
        bool DefaultIsPurchaseItem,
        bool DefaultIsSalesItem,
        bool DefaultIsInventoryItem,
        int SortOrder,
        bool IsActive);
}

using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ItemFamilies;

public static class ItemFamilyEndpoints
{
    private const string FormKey = "item-families";

    public static IEndpointRouteBuilder MapItemFamilyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/item-families");

        group.MapGet("", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemFamiliesQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemFamiliesRead)
            .RequireFormOperation(FormKey, "refresh");

        group.MapGet("/lookup", async (int? itemGroupId, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemFamilyLookupQuery(itemGroupId), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.ItemsRead);

        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemFamilyByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemFamiliesRead)
            .RequireFormOperation(FormKey, "consult");

        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemFamilyHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemFamiliesRead)
            .RequireFormOperation(FormKey, "history");

        group.MapPost("", async (SaveItemFamilyRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateItemFamilyCommand(
                request.ItemGroupId, request.Code, request.Name, request.Description,
                request.SortOrder, request.IsActive, request.ExternalSystem, request.ExternalCode,
                request.SapFamilyCode, request.SapCode, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemFamiliesManage)
          .RequireFormOperation(FormKey, "create");

        group.MapPut("/{id:int}", async (int id, SaveItemFamilyRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateItemFamilyCommand(
                id, request.ItemGroupId, request.Code, request.Name, request.Description,
                request.SortOrder, request.IsActive, request.ExternalSystem, request.ExternalCode,
                request.SapFamilyCode, request.SapCode, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemFamiliesManage)
          .RequireFormOperation(FormKey, "update");

        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteItemFamilyCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemFamiliesManage)
          .RequireFormOperation(FormKey, "delete");

        return app;
    }

    private sealed record SaveItemFamilyRequest(
        int ItemGroupId,
        string Code,
        string Name,
        string? Description,
        int SortOrder,
        bool IsActive,
        string? ExternalSystem,
        string? ExternalCode,
        string? SapFamilyCode,
        string? SapCode);
}

using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ItemSubgroups;

public static class ItemSubgroupEndpoints
{
    private const string FormKey = "item-subgroups";
    public static IEndpointRouteBuilder MapItemSubgroupEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/item-subgroups");
        group.MapGet("", async (ISender sender, CancellationToken ct) => (await sender.Send(new GetItemSubgroupsQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemSubgroupsRead).RequireFormOperation(FormKey, "refresh");
        group.MapGet("/lookup", async (int? itemFamilyId, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemSubgroupLookupQuery(itemFamilyId), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.ItemsRead);
        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemSubgroupByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemSubgroupsRead).RequireFormOperation(FormKey, "consult");
        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemSubgroupHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemSubgroupsRead).RequireFormOperation(FormKey, "history");
        group.MapPost("", async (SaveItemSubgroupRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateItemSubgroupCommand(request.ItemFamilyId, request.Code, request.Name, request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemSubgroupsManage).RequireFormOperation(FormKey, "create");
        group.MapPut("/{id:int}", async (int id, SaveItemSubgroupRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateItemSubgroupCommand(id, request.ItemFamilyId, request.Code, request.Name, request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemSubgroupsManage).RequireFormOperation(FormKey, "update");
        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteItemSubgroupCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemSubgroupsManage).RequireFormOperation(FormKey, "delete");
        return app;
    }
    private sealed record SaveItemSubgroupRequest(int ItemFamilyId, string Code, string Name, string? Description, int SortOrder, bool IsActive);
}

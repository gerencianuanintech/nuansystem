using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ItemGroups;

public static class ItemGroupEndpoints
{
    private const string BaseRoute = "/api/definitions/inventory/item-groups";
    private const string FormKey = "item-groups";

    public static IEndpointRouteBuilder MapItemGroupEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(BaseRoute);
        group.MapGet("", async (ISender sender, CancellationToken ct) => (await sender.Send(new GetItemGroupsQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemGroupsRead).RequireFormOperation(FormKey, "refresh");
        group.MapGet("/lookup", async (ISender sender, CancellationToken ct) => (await sender.Send(new GetItemGroupLookupQuery(), ct)).ToHttpResult())
            .RequireAuthorization(policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.ItemsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryItemGroupsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryItemGroupsManage)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryItemFamiliesRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryItemFamiliesManage));
            });
        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemGroupByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemGroupsRead).RequireFormOperation(FormKey, "consult");
        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemGroupHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemGroupsRead).RequireFormOperation(FormKey, "history");
        group.MapPost("", async (CreateItemGroupCommand command, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(command with { AuditUserId = audit.UserId, AuditUserName = audit.UserName }, ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemGroupsManage).RequireFormOperation(FormKey, "create");
        group.MapPut("/{id:int}", async (int id, UpdateItemGroupCommand command, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(command with { Id = id, AuditUserId = audit.UserId, AuditUserName = audit.UserName }, ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemGroupsManage).RequireFormOperation(FormKey, "update");
        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteItemGroupCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemGroupsManage).RequireFormOperation(FormKey, "delete");
        return app;
    }
}

using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ItemOrigins;

public static class ItemOriginEndpoints
{
    private const string FormKey = "item-origins";
    public static IEndpointRouteBuilder MapItemOriginEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/item-origins");
        group.MapGet("", async (ISender sender, CancellationToken ct) => (await sender.Send(new GetItemOriginsQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemOriginsRead).RequireFormOperation(FormKey, "refresh");
        group.MapGet("/lookup", async (string? includeCode, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemOriginLookupQuery(includeCode), ct)).ToHttpResult())
            .RequireAuthorization(policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.ItemsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryItemOriginsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryItemOriginsManage));
            });
        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemOriginByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemOriginsRead).RequireFormOperation(FormKey, "consult");
        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemOriginHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemOriginsRead).RequireFormOperation(FormKey, "history");
        group.MapPost("", async (SaveItemOriginRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateItemOriginCommand(request.Code, request.Name, request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemOriginsManage).RequireFormOperation(FormKey, "create");
        group.MapPut("/{id:int}", async (int id, SaveItemOriginRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateItemOriginCommand(id, request.Code, request.Name, request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemOriginsManage).RequireFormOperation(FormKey, "update");
        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteItemOriginCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemOriginsManage).RequireFormOperation(FormKey, "delete");
        return app;
    }
    private sealed record SaveItemOriginRequest(string Code, string Name, string? Description, int SortOrder, bool IsActive);
}

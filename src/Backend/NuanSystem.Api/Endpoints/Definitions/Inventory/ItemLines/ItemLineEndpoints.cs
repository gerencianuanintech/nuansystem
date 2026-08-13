using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ItemLines;

public static class ItemLineEndpoints
{
    private const string FormKey = "item-lines";

    public static IEndpointRouteBuilder MapItemLineEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/item-lines");

        group.MapGet("", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemLinesQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemLinesRead)
            .RequireFormOperation(FormKey, "refresh");

        group.MapGet("/lookup", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemLineLookupQuery(), ct)).ToHttpResult())
            .RequireAuthorization(policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.ItemsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryItemLinesRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryItemLinesManage));
            });

        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemLineByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemLinesRead)
            .RequireFormOperation(FormKey, "consult");

        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemLineHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemLinesRead)
            .RequireFormOperation(FormKey, "history");

        group.MapPost("", async (SaveItemLineRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateItemLineCommand(request.Code, request.Name,
                request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemLinesManage)
          .RequireFormOperation(FormKey, "create");

        group.MapPut("/{id:int}", async (int id, SaveItemLineRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateItemLineCommand(id, request.Code, request.Name,
                request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemLinesManage)
          .RequireFormOperation(FormKey, "update");

        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteItemLineCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemLinesManage)
          .RequireFormOperation(FormKey, "delete");

        return app;
    }

    private sealed record SaveItemLineRequest(
        string Code, string Name, string? Description, int SortOrder, bool IsActive);
}

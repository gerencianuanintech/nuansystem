using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ItemAlertTypes;

public static class ItemAlertTypeEndpoints
{
    private const string FormKey = "item-alert-types";
    public static IEndpointRouteBuilder MapItemAlertTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/item-alert-types");
        group.MapGet("", async (ISender sender, CancellationToken ct) => (await sender.Send(new GetItemAlertTypesQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemAlertTypesRead).RequireFormOperation(FormKey, "refresh");
        group.MapGet("/lookup", async (ISender sender, CancellationToken ct) => (await sender.Send(new GetItemAlertTypeLookupQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.ItemsRead);
        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemAlertTypeByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemAlertTypesRead).RequireFormOperation(FormKey, "consult");
        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemAlertTypeHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemAlertTypesRead).RequireFormOperation(FormKey, "history");
        group.MapPost("", async (SaveItemAlertTypeRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateItemAlertTypeCommand(request.Code, request.Name, request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemAlertTypesManage).RequireFormOperation(FormKey, "create");
        group.MapPut("/{id:int}", async (int id, SaveItemAlertTypeRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateItemAlertTypeCommand(id, request.Code, request.Name, request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemAlertTypesManage).RequireFormOperation(FormKey, "update");
        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteItemAlertTypeCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemAlertTypesManage).RequireFormOperation(FormKey, "delete");
        return app;
    }
    private sealed record SaveItemAlertTypeRequest(string Code, string Name, string? Description, int SortOrder, bool IsActive);
}


using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemCommercialSegments.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ItemCommercialSegments;

public static class ItemCommercialSegmentEndpoints
{
    private const string FormKey = "item-commercial-segments";
    public static IEndpointRouteBuilder MapItemCommercialSegmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/item-commercial-segments");
        group.MapGet("", async (ISender sender, CancellationToken ct) => (await sender.Send(new GetItemCommercialSegmentsQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemCommercialSegmentsRead).RequireFormOperation(FormKey, "refresh");
        group.MapGet("/lookup", async (ISender sender, CancellationToken ct) => (await sender.Send(new GetItemCommercialSegmentLookupQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.ItemsRead);
        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemCommercialSegmentByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemCommercialSegmentsRead).RequireFormOperation(FormKey, "consult");
        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetItemCommercialSegmentHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemCommercialSegmentsRead).RequireFormOperation(FormKey, "history");
        group.MapPost("", async (SaveItemCommercialSegmentRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateItemCommercialSegmentCommand(request.Code, request.Name, request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemCommercialSegmentsManage).RequireFormOperation(FormKey, "create");
        group.MapPut("/{id:int}", async (int id, SaveItemCommercialSegmentRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateItemCommercialSegmentCommand(id, request.Code, request.Name, request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemCommercialSegmentsManage).RequireFormOperation(FormKey, "update");
        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteItemCommercialSegmentCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemCommercialSegmentsManage).RequireFormOperation(FormKey, "delete");
        return app;
    }
    private sealed record SaveItemCommercialSegmentRequest(string Code, string Name, string? Description, int SortOrder, bool IsActive);
}

using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.SalesChannels.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.SalesChannels;

public static class SalesChannelEndpoints
{
    private const string FormKey = "sales-channels";
    public static IEndpointRouteBuilder MapSalesChannelEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/sales-channels");
        group.MapGet("", async (ISender sender, CancellationToken ct) => (await sender.Send(new GetSalesChannelsQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventorySalesChannelsRead).RequireFormOperation(FormKey, "refresh");
        group.MapGet("/lookup", async (ISender sender, CancellationToken ct) => (await sender.Send(new GetSalesChannelLookupQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.ItemsRead);
        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetSalesChannelByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventorySalesChannelsRead).RequireFormOperation(FormKey, "consult");
        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) => (await sender.Send(new GetSalesChannelHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventorySalesChannelsRead).RequireFormOperation(FormKey, "history");
        group.MapPost("", async (SaveSalesChannelRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateSalesChannelCommand(request.Code, request.Name, request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventorySalesChannelsManage).RequireFormOperation(FormKey, "create");
        group.MapPut("/{id:int}", async (int id, SaveSalesChannelRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateSalesChannelCommand(id, request.Code, request.Name, request.Description, request.SortOrder, request.IsActive, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventorySalesChannelsManage).RequireFormOperation(FormKey, "update");
        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteSalesChannelCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventorySalesChannelsManage).RequireFormOperation(FormKey, "delete");
        return app;
    }
    private sealed record SaveSalesChannelRequest(string Code, string Name, string? Description, int SortOrder, bool IsActive);
}



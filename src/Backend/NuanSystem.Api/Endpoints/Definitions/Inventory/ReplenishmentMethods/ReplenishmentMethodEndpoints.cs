using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ReplenishmentMethods;

public static class ReplenishmentMethodEndpoints
{
    private const string FormKey = "replenishment-methods";

    public static IEndpointRouteBuilder MapReplenishmentMethodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/replenishment-methods");
        group.MapGet("", async (ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new GetReplenishmentMethodsQuery(), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryReplenishmentMethodsRead)
            .RequireFormOperation(FormKey, "refresh");
        group.MapGet("/lookup", async (string? includeCode, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new GetReplenishmentMethodLookupQuery(includeCode), cancellationToken)).ToHttpResult())
            .RequireAuthorization(policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.ItemsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryReplenishmentMethodsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryReplenishmentMethodsManage));
            });
        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new GetReplenishmentMethodByIdQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryReplenishmentMethodsRead)
            .RequireFormOperation(FormKey, "consult");
        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken cancellationToken) =>
                (await sender.Send(new GetReplenishmentMethodHistoryQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryReplenishmentMethodsRead)
            .RequireFormOperation(FormKey, "history");
        group.MapPost("", async (SaveReplenishmentMethodRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateReplenishmentMethodCommand(request.Code, request.Name, request.Description,
                request.SortOrder, request.IsActive, audit.UserId, audit.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryReplenishmentMethodsManage).RequireFormOperation(FormKey, "create");
        group.MapPut("/{id:int}", async (int id, SaveReplenishmentMethodRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateReplenishmentMethodCommand(id, request.Code, request.Name, request.Description,
                request.SortOrder, request.IsActive, audit.UserId, audit.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryReplenishmentMethodsManage).RequireFormOperation(FormKey, "update");
        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteReplenishmentMethodCommand(id, audit.UserId, audit.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryReplenishmentMethodsManage).RequireFormOperation(FormKey, "delete");
        return app;
    }

    private sealed record SaveReplenishmentMethodRequest(string Code, string Name, string? Description, int SortOrder, bool IsActive);
}

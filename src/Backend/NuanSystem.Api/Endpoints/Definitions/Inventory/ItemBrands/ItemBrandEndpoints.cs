using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ItemBrands;

public static class ItemBrandEndpoints
{
    private const string FormKey = "item-brands";

    public static IEndpointRouteBuilder MapItemBrandEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/item-brands");

        group.MapGet("", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemBrandsQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemBrandsRead)
            .RequireFormOperation(FormKey, "refresh");

        group.MapGet("/lookup", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemBrandLookupQuery(), ct)).ToHttpResult())
            .RequireAuthorization(policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.ItemsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryItemBrandsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryItemBrandsManage));
            });

        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemBrandByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemBrandsRead)
            .RequireFormOperation(FormKey, "consult");

        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetItemBrandHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryItemBrandsRead)
            .RequireFormOperation(FormKey, "history");

        group.MapPost("", async (SaveItemBrandRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateItemBrandCommand(
                request.Code, request.Name, request.Description, request.SortOrder, request.IsActive,
                request.ExternalSystem, request.ExternalCode, request.SapManufacturerCode, request.SapCode,
                audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemBrandsManage)
          .RequireFormOperation(FormKey, "create");

        group.MapPut("/{id:int}", async (int id, SaveItemBrandRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateItemBrandCommand(
                id, request.Code, request.Name, request.Description, request.SortOrder, request.IsActive,
                request.ExternalSystem, request.ExternalCode, request.SapManufacturerCode, request.SapCode,
                audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemBrandsManage)
          .RequireFormOperation(FormKey, "update");

        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteItemBrandCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryItemBrandsManage)
          .RequireFormOperation(FormKey, "delete");

        return app;
    }

    private sealed record SaveItemBrandRequest(
        string Code, string Name, string? Description, int SortOrder, bool IsActive,
        string? ExternalSystem, string? ExternalCode, string? SapManufacturerCode, string? SapCode);
}

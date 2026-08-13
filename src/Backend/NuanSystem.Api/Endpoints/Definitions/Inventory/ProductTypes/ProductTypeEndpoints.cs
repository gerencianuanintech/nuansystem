using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.ProductTypes;

public static class ProductTypeEndpoints
{
    private const string FormKey = "product-types";

    public static IEndpointRouteBuilder MapProductTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/product-types");

        group.MapGet("", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetProductTypesQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryProductTypesRead)
            .RequireFormOperation(FormKey, "refresh");

        group.MapGet("/lookup", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetProductTypeLookupQuery(), ct)).ToHttpResult())
            .RequireAuthorization(policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.ItemsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryProductTypesRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryProductTypesManage));
            });

        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetProductTypeByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryProductTypesRead)
            .RequireFormOperation(FormKey, "consult");

        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetProductTypeHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryProductTypesRead)
            .RequireFormOperation(FormKey, "history");

        group.MapPost("", async (SaveProductTypeRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateProductTypeCommand(request.Code, request.Name,
                request.Description, request.NatureCode, request.SortOrder, request.IsActive,
                audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryProductTypesManage)
          .RequireFormOperation(FormKey, "create");

        group.MapPut("/{id:int}", async (int id, SaveProductTypeRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateProductTypeCommand(id, request.Code, request.Name,
                request.Description, request.NatureCode, request.SortOrder, request.IsActive,
                audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryProductTypesManage)
          .RequireFormOperation(FormKey, "update");

        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteProductTypeCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryProductTypesManage)
          .RequireFormOperation(FormKey, "delete");

        return app;
    }

    private sealed record SaveProductTypeRequest(string Code, string Name, string? Description,
        string NatureCode, int SortOrder, bool IsActive);
}

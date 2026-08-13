using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Commands;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.Inventory.UnitMeasures;

public static class UnitMeasureEndpoints
{
    private const string FormKey = "unit-measures";

    public static IEndpointRouteBuilder MapUnitMeasureEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/definitions/inventory/unit-measures");

        group.MapGet("", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetUnitMeasuresQuery(), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryUnitMeasuresRead)
            .RequireFormOperation(FormKey, "refresh");

        group.MapGet("/lookup", async (ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetUnitMeasureLookupQuery(), ct)).ToHttpResult())
            .RequireAuthorization(policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                    context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.ItemsRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryUnitMeasuresRead)
                    || context.User.HasClaim(AuthClaimNames.Permission, PermissionCodes.GeneralInventoryUnitMeasuresManage));
            });

        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetUnitMeasureByIdQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryUnitMeasuresRead)
            .RequireFormOperation(FormKey, "consult");

        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken ct) =>
            (await sender.Send(new GetUnitMeasureHistoryQuery(id), ct)).ToHttpResult())
            .RequirePermission(PermissionCodes.GeneralInventoryUnitMeasuresRead)
            .RequireFormOperation(FormKey, "history");

        group.MapPost("", async (SaveUnitMeasureRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new CreateUnitMeasureCommand(request.Code, request.Name,
                request.Description, request.Symbol, request.MagnitudeCode, request.SortOrder,
                request.IsActive, request.ExternalSystem, request.ExternalCode,
                audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryUnitMeasuresManage)
          .RequireFormOperation(FormKey, "create");

        group.MapPut("/{id:int}", async (int id, SaveUnitMeasureRequest request, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new UpdateUnitMeasureCommand(id, request.Code, request.Name,
                request.Description, request.Symbol, request.MagnitudeCode, request.SortOrder,
                request.IsActive, request.ExternalSystem, request.ExternalCode,
                audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryUnitMeasuresManage)
          .RequireFormOperation(FormKey, "update");

        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var audit = user.GetAuditUser();
            return (await sender.Send(new DeleteUnitMeasureCommand(id, audit.UserId, audit.UserName), ct)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeneralInventoryUnitMeasuresManage)
          .RequireFormOperation(FormKey, "delete");

        return app;
    }

    private sealed record SaveUnitMeasureRequest(string Code, string Name, string? Description,
        string? Symbol, string MagnitudeCode, int SortOrder, bool IsActive,
        string? ExternalSystem, string? ExternalCode);
}

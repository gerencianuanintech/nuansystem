using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Carriers.Commands;
using NuanSystem.Application.Features.Carriers.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class CarrierEndpoints
{
    private const string FormKey = "carriers";

    public static IEndpointRouteBuilder MapCarrierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/carriers");

        group.MapGet("/", async (ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetCarriersQuery(), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.CarriersRead)
            .RequireFormOperation(FormKey, "refresh");

        group.MapGet("/lookup", async (ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetCarrierLookupQuery(), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.CarriersRead);

        group.MapGet("/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetCarrierByIdQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.CarriersRead)
            .RequireFormOperation(FormKey, "consult");

        group.MapGet("/{id:int}/history", async (int id, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetCarrierHistoryQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.CarriersRead)
            .RequireFormOperation(FormKey, "history");

        group.MapPost("/", async (SaveCarrierRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var command = new CreateCarrierCommand(request.Code, request.Name, request.IdentificationTypeCode, request.IdentificationNumber, request.Description, request.IsActive, auditUser.UserId, auditUser.UserName);
            return (await sender.Send(command, cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.CarriersManage).RequireFormOperation(FormKey, "create");

        group.MapPut("/{id:int}", async (int id, SaveCarrierRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var command = new UpdateCarrierCommand(id, request.Code, request.Name, request.IdentificationTypeCode, request.IdentificationNumber, request.Description, request.IsActive, auditUser.UserId, auditUser.UserName);
            return (await sender.Send(command, cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.CarriersManage).RequireFormOperation(FormKey, "update");

        group.MapDelete("/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new DeleteCarrierCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.CarriersManage).RequireFormOperation(FormKey, "delete");

        return app;
    }

    private sealed record SaveCarrierRequest(string Code, string Name, string IdentificationTypeCode, string IdentificationNumber, string? Description, bool IsActive);
}

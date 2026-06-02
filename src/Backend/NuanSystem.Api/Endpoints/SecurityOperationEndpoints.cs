using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.SecurityOperations.Commands;
using NuanSystem.Application.Features.SecurityOperations.Queries;

namespace NuanSystem.Api.Endpoints;

public static class SecurityOperationEndpoints
{
    public static IEndpointRouteBuilder MapSecurityOperationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/security/operations", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityOperationsQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("security-operations", "refresh");

        app.MapGet("/api/security/operations/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityOperationByIdQuery(id), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("security-operations", "consult");

        app.MapPost("/api/security/operations", async (
            CreateSecurityOperationCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("security-operations", "create");

        app.MapPut("/api/security/operations/{id:int}", async (
            int id,
            UpdateSecurityOperationCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                Id = id,
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("security-operations", "update");

        app.MapDelete("/api/security/operations/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new DeleteSecurityOperationCommand(id, auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("security-operations", "delete");

        return app;
    }
}

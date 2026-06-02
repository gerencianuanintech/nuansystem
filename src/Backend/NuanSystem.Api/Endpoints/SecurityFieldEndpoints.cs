using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.SecurityFields.Commands;
using NuanSystem.Application.Features.SecurityFields.Queries;

namespace NuanSystem.Api.Endpoints;

public static class SecurityFieldEndpoints
{
    public static IEndpointRouteBuilder MapSecurityFieldEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/security/fields", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityFieldsQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("security-fields", "refresh");

        app.MapGet("/api/security/fields/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityFieldByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("security-fields", "consult");

        app.MapPost("/api/security/fields", async (
            CreateSecurityFieldCommand command,
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
        .RequireFormOperation("security-fields", "create");

        app.MapPut("/api/security/fields/{id:int}", async (
            int id,
            UpdateSecurityFieldCommand command,
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
        .RequireFormOperation("security-fields", "update");

        app.MapDelete("/api/security/fields/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new DeleteSecurityFieldCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("security-fields", "delete");

        return app;
    }
}

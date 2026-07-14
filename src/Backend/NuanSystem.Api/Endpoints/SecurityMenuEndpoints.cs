using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Features.SecurityMenus.Commands;
using NuanSystem.Application.Features.SecurityMenus.Queries;

namespace NuanSystem.Api.Endpoints;

public static class SecurityMenuEndpoints
{
    public static IEndpointRouteBuilder MapSecurityMenuEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/security/menus", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityMenusQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityMenus)
        .RequireFormOperation("security-menus", "refresh");

        app.MapGet("/api/security/menus/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityMenuByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityMenus)
        .RequireFormOperation("security-menus", "consult");

        app.MapPost("/api/security/menus", async (
            CreateSecurityMenuCommand command,
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
        .WithTags(SwaggerTags.SecurityMenus)
        .RequireFormOperation("security-menus", "create");

        app.MapPut("/api/security/menus/{id:int}", async (
            int id,
            UpdateSecurityMenuCommand command,
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
        .WithTags(SwaggerTags.SecurityMenus)
        .RequireFormOperation("security-menus", "update");

        app.MapDelete("/api/security/menus/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new DeleteSecurityMenuCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityMenus)
        .RequireFormOperation("security-menus", "delete");

        return app;
    }
}

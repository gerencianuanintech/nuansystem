using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Features.SecurityAccess.Commands;
using NuanSystem.Application.Features.SecurityAccess.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SecurityAccessEndpoints
{
    public static IEndpointRouteBuilder MapSecurityAccessEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/security/navigation/me", async (
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new GetNavigationQuery(userId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequireAuthorization();

        app.MapGet("/api/security/forms/{formKey}/operations/me", async (
            string formKey,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new GetCurrentFormOperationsQuery(userId, formKey), cancellationToken);
            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequireAuthorization();

        app.MapGet("/api/security/roles/{roleId:int}/access", async (
            int roleId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetRoleAccessQuery(roleId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.RolesManage);

        app.MapPut("/api/security/roles/{roleId:int}/access", async (
            int roleId,
            SaveRoleAccessCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                RoleId = roleId,
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.RolesManage);

        return app;
    }
}

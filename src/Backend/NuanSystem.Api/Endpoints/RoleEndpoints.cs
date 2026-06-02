using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Roles.Commands;
using NuanSystem.Application.Features.Roles.Queries;
using NuanSystem.Application.Features.SecurityRoles.Commands;
using NuanSystem.Application.Features.SecurityRoles.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/roles", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetRolesAdminQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.RolesManage);

        app.MapPost("/api/roles", async (
            CreateRoleCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.RolesManage);

        app.MapGet("/api/roles/permissions", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetPermissionsQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.RolesManage);

        app.MapPost("/api/roles/assign-permission", async (
            AssignRolePermissionCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.RolesManage);

        app.MapGet("/api/security/roles", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityRolesQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("security-roles", "refresh");

        app.MapGet("/api/security/roles/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityRoleByIdQuery(id), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("security-roles", "consult");

        app.MapPost("/api/security/roles", async (
            CreateSecurityRoleCommand command,
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
        .RequireFormOperation("security-roles", "create");

        app.MapPut("/api/security/roles/{id:int}", async (
            int id,
            UpdateSecurityRoleCommand command,
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
        .RequireFormOperation("security-roles", "update");

        app.MapDelete("/api/security/roles/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new DeleteSecurityRoleCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("security-roles", "delete");

        return app;
    }
}

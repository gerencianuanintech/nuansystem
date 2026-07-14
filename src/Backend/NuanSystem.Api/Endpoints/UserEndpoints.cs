using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Features.SecurityUsers.Commands;
using NuanSystem.Application.Features.SecurityUsers.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetUsersQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityUsers)
        .RequirePermission(PermissionCodes.UsersManage);

        app.MapGet("/api/security/users", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetUsersQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityUsers)
        .RequireFormOperation("users", "refresh");

        app.MapGet("/api/security/users/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(id), cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityUsers)
        .RequireFormOperation("users", "consult");

        app.MapPost("/api/users", async (
            CreateUserCommand command,
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
        .WithTags(SwaggerTags.SecurityUsers)
        .RequirePermission(PermissionCodes.UsersManage);

        app.MapPost("/api/security/users", async (
            CreateUserCommand command,
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
        .WithTags(SwaggerTags.SecurityUsers)
        .RequireFormOperation("users", "create");

        app.MapPut("/api/security/users/{id:int}", async (
            int id,
            UpdateUserCommand command,
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
        .WithTags(SwaggerTags.SecurityUsers)
        .RequireFormOperation("users", "update");

        app.MapDelete("/api/security/users/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new DeleteUserCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityUsers)
        .RequireFormOperation("users", "delete");

        app.MapGet("/api/users/roles", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetRolesQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityUsers)
        .RequirePermission(PermissionCodes.UsersManage);

        app.MapGet("/api/security/users/roles", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetRolesQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityUsers)
        .RequireFormOperation("users", "refresh");

        return app;
    }
}

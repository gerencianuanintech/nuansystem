using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Features.SecurityAccess.Commands;
using NuanSystem.Application.Features.SecurityAccess.Dtos;
using NuanSystem.Application.Features.SecurityAccess.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SecurityRoleFormAccessEndpoints
{
    public static IEndpointRouteBuilder MapSecurityRoleFormAccessEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/security/form-access/forms", async (
            int? formType,
            bool? onlyActive,
            string? search,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetSecurityFormAccessFormsQuery(formType, onlyActive ?? true, search),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.SecurityFormAccessMaintenanceManage);

        app.MapGet("/api/security/form-access/roles/{roleId:int}/forms/{formId:int}/operations", async (
            int roleId,
            int formId,
            bool? onlyActive,
            string? search,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetSecurityFormAccessOperationsQuery(roleId, formId, onlyActive ?? true, search),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.SecurityFormAccessMaintenanceManage);

        app.MapPut("/api/security/form-access/roles/{roleId:int}/forms/{formId:int}/operations", async (
            int roleId,
            int formId,
            SaveSecurityFormAccessRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new SaveSecurityFormAccessCommand(
                    roleId,
                    formId,
                    request.Operations,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.SecurityFormAccessMaintenanceManage);

        app.MapGet("/api/security/form-access/me", async (
            string formKey,
            string actionKey,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(
                new ValidateCurrentSecurityFormAccessQuery(userId, formKey, actionKey),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequireAuthorization();

        app.MapGet("/api/security/transactional-form-access/forms", async (
            bool? onlyActive,
            string? search,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetSecurityFormAccessFormsQuery(2, onlyActive ?? true, search),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.SecurityFormAccessTransactionalManage);

        app.MapGet("/api/security/transactional-form-access/roles/{roleId:int}/forms/{formId:int}/operations", async (
            int roleId,
            int formId,
            bool? onlyActive,
            string? search,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetSecurityFormAccessOperationsQuery(roleId, formId, onlyActive ?? true, search),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.SecurityFormAccessTransactionalManage);

        app.MapPut("/api/security/transactional-form-access/roles/{roleId:int}/forms/{formId:int}/operations", async (
            int roleId,
            int formId,
            SaveSecurityFormAccessRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new SaveSecurityFormAccessCommand(
                    roleId,
                    formId,
                    request.Operations,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.SecurityFormAccessTransactionalManage);

        return app;
    }
}

using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SecurityAccess.Commands;
using NuanSystem.Application.Features.SecurityAccess.Dtos;
using NuanSystem.Application.Features.SecurityAccess.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SecurityRoleFormFieldAccessEndpoints
{
    public static IEndpointRouteBuilder MapSecurityRoleFormFieldAccessEndpoints(this IEndpointRouteBuilder app)
    {
        MapMaintenanceFieldAccessRoutes(app);
        MapTransactionalFieldAccessRoutes(app);

        return app;
    }

    private static void MapMaintenanceFieldAccessRoutes(IEndpointRouteBuilder app)
    {
        const string basePath = "/api/security/maintenance-field-access";

        app.MapGet($"{basePath}/roles/{{roleId:int}}/forms/{{formId:int}}/fields", async (
            int roleId,
            int formId,
            bool? onlyActive,
            string? search,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetSecurityFormFieldAccessQuery(roleId, formId, onlyActive ?? true, search),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.SecurityFieldAccessMaintenanceManage);

        app.MapPut($"{basePath}/roles/{{roleId:int}}/forms/{{formId:int}}/fields", async (
            int roleId,
            int formId,
            SaveSecurityFormFieldAccessRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new SaveSecurityFormFieldAccessCommand(
                    roleId,
                    formId,
                    request.Fields,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.SecurityFieldAccessMaintenanceManage);
    }

    private static void MapTransactionalFieldAccessRoutes(IEndpointRouteBuilder app)
    {
        const string basePath = "/api/security/transactional-field-access";

        app.MapGet($"{basePath}/roles/{{roleId:int}}/forms/{{formId:int}}/series/{{seriesId:int}}/fields", async (
            int roleId,
            int formId,
            int seriesId,
            string documentType,
            bool? onlyActive,
            string? search,
            ICompanyContext companyContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (companyContext.CurrentCompany is null)
            {
                return Results.BadRequest("Debe seleccionar una empresa.");
            }

            var result = await sender.Send(
                new GetSecurityDocumentSeriesFieldAccessQuery(
                    roleId,
                    companyContext.CurrentCompany.CompanyCode,
                    formId,
                    documentType,
                    seriesId,
                    onlyActive ?? true,
                    search),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.SecurityFieldAccessTransactionalManage);

        app.MapPut($"{basePath}/roles/{{roleId:int}}/forms/{{formId:int}}/series/{{seriesId:int}}/fields", async (
            int roleId,
            int formId,
            int seriesId,
            string documentType,
            SaveSecurityFormFieldAccessRequest request,
            ICompanyContext companyContext,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (companyContext.CurrentCompany is null)
            {
                return Results.BadRequest("Debe seleccionar una empresa.");
            }

            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new SaveSecurityDocumentSeriesFieldAccessCommand(
                    roleId,
                    companyContext.CurrentCompany.CompanyCode,
                    formId,
                    documentType,
                    seriesId,
                    request.Fields,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SecurityAccess)
        .RequirePermission(PermissionCodes.SecurityFieldAccessTransactionalManage);
    }
}

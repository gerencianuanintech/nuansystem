using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SecurityAccess.Commands;
using NuanSystem.Application.Features.SecurityAccess.Dtos;
using NuanSystem.Application.Features.SecurityAccess.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SecurityDocumentSeriesAccessEndpoints
{
    public static IEndpointRouteBuilder MapSecurityDocumentSeriesAccessEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/security/document-series-access/roles/{roleId:int}/series", async (
            int roleId,
            string formKey,
            string? search,
            string? documentType,
            bool? isActive,
            ICompanyContext companyContext,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (companyContext.CurrentCompany is null)
            {
                return Results.BadRequest("Debe seleccionar una empresa.");
            }

            var result = await sender.Send(
                new GetSecurityDocumentSeriesAccessQuery(
                    roleId,
                    companyContext.CurrentCompany.CompanyCode,
                    formKey,
                    search,
                    documentType,
                    isActive),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SecurityFormAccessTransactionalManage);

        app.MapGet("/api/security/document-series-access/roles/{roleId:int}/series/{seriesId:int}/operations", async (
            int roleId,
            int seriesId,
            string formKey,
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
                new GetSecurityDocumentSeriesOperationsAccessQuery(
                    roleId,
                    companyContext.CurrentCompany.CompanyCode,
                    formKey,
                    documentType,
                    seriesId,
                    onlyActive ?? true,
                    search),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SecurityFormAccessTransactionalManage);

        app.MapPut("/api/security/document-series-access/roles/{roleId:int}/series/{seriesId:int}/operations", async (
            int roleId,
            int seriesId,
            string formKey,
            string documentType,
            SaveSecurityDocumentSeriesAccessRequest request,
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
                new SaveSecurityDocumentSeriesAccessCommand(
                    roleId,
                    companyContext.CurrentCompany.CompanyCode,
                    formKey,
                    documentType,
                    seriesId,
                    request.IsSelected,
                    request.Operations,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SecurityFormAccessTransactionalManage);

        return app;
    }
}

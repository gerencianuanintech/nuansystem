using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.OperationalCatalogs.Commands;
using NuanSystem.Application.Features.OperationalCatalogs.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class OperationalCatalogEndpoints
{
    public static IEndpointRouteBuilder MapOperationalCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/operational-catalogs/{catalogKey}", async (
            string catalogKey,
            string? search,
            string? parentCatalogKey,
            string? parentCode,
            bool? isActive,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetOperationalCatalogsQuery(catalogKey, search, parentCatalogKey, parentCode, isActive), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.OperationalCatalogsRead);

        app.MapGet("/api/operational-catalogs/{catalogKey}/lookup", async (
            string catalogKey,
            string? parentCatalogKey,
            string? parentCode,
            bool? activeOnly,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetOperationalCatalogLookupQuery(catalogKey, parentCatalogKey, parentCode, activeOnly ?? true), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.OperationalCatalogsRead);

        app.MapGet("/api/operational-catalogs/{catalogKey}/{id:int}", async (
            string catalogKey,
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetOperationalCatalogByIdQuery(catalogKey, id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.OperationalCatalogsRead);

        app.MapPost("/api/operational-catalogs/{catalogKey}", async (
            string catalogKey,
            SaveOperationalCatalogRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new CreateOperationalCatalogCommand(
                catalogKey,
                request.Code,
                request.Name,
                request.Description,
                request.ParentCatalogKey,
                request.ParentCode,
                request.DisplayOrder,
                request.IsDefault,
                request.IsActive,
                auditUser.UserId,
                auditUser.UserName), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.OperationalCatalogsManage);

        app.MapPut("/api/operational-catalogs/{catalogKey}/{id:int}", async (
            string catalogKey,
            int id,
            SaveOperationalCatalogRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new UpdateOperationalCatalogCommand(
                id,
                catalogKey,
                request.Code,
                request.Name,
                request.Description,
                request.ParentCatalogKey,
                request.ParentCode,
                request.DisplayOrder,
                request.IsDefault,
                request.IsActive,
                auditUser.UserId,
                auditUser.UserName), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.OperationalCatalogsManage);

        app.MapDelete("/api/operational-catalogs/{catalogKey}/{id:int}", async (
            string catalogKey,
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteOperationalCatalogCommand(catalogKey, id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.OperationalCatalogsManage);

        return app;
    }

    private sealed record SaveOperationalCatalogRequest(
        string Code,
        string Name,
        string? Description,
        string? ParentCatalogKey,
        string? ParentCode,
        int DisplayOrder,
        bool IsDefault,
        bool IsActive);
}

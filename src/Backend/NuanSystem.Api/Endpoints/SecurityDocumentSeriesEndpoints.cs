using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Commands;
using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SecurityDocumentSeriesEndpoints
{
    public static IEndpointRouteBuilder MapSecurityDocumentSeriesEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/security-document-series", async (
            string? search,
            string? documentType,
            bool? isActive,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetSecurityDocumentSeriesQuery(search, documentType, isActive),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.DocumentsSeriesRead);

        app.MapGet("/api/security-document-series/lookups", async (
            string? documentType,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetSecurityDocumentSeriesLookupQuery(documentType),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.DocumentsSeriesRead);

        app.MapGet("/api/security-document-series/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityDocumentSeriesByIdQuery(id), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.DocumentsSeriesRead);

        app.MapPost("/api/security-document-series", async (
            CreateSecurityDocumentSeriesCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName },
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.DocumentsSeriesCreate);

        app.MapPut("/api/security-document-series/{id:int}", async (
            int id,
            UpdateSecurityDocumentSeriesCommand command,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName },
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.DocumentsSeriesUpdate);

        app.MapDelete("/api/security-document-series/{id:int}", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new DeleteSecurityDocumentSeriesCommand(id, auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.DocumentsSeriesDelete);

        app.MapPost("/api/security-document-series/{id:int}/reserve-number", async (
            int id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new ReserveSecurityDocumentNumberCommand(id, auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.DocumentsSeriesManage);

        return app;
    }
}

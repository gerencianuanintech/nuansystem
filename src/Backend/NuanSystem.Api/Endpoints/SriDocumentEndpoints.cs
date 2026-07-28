using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.SriDocuments.Commands;
using NuanSystem.Application.Features.SriDocuments.Dtos;
using NuanSystem.Application.Features.SriDocuments.Queries;
using NuanSystem.Application.Features.Operations;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class SriDocumentEndpoints
{
    public static IEndpointRouteBuilder MapSriDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sri/documents");

        group.MapGet("", async (string? environment, string? status, string? sourceType, string? accessKey, DateTime? createdFrom, DateTime? createdTo, int? page, int? pageSize, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetSriDocumentQueueQuery(new SriDocumentQueueFilter(environment, status, sourceType, accessKey, createdFrom, createdTo, page ?? 1, pageSize ?? 100)), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriDocumentsView);

        group.MapGet("/{id:long}", async (long id, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetSriDocumentQueueByIdQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriDocumentsView);

        group.MapGet("/{id:long}/attempts", async (long id, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetSriDocumentAttemptsQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriDocumentsView);

        group.MapGet("/monitor/summary", async (long? importId, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetSriDocumentMonitorSummaryQuery(importId), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriDocumentsView);

        group.MapGet("/monitor/worker-health", async (ISender sender, IConfiguration configuration, CancellationToken cancellationToken) =>
            (await sender.Send(new GetSriWorkerHealthQuery(configuration.GetSection("SriWorkerHealth").Get<WorkerHealthThresholds>() ?? new WorkerHealthThresholds()), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriWorkerHealthView);

        group.MapGet("/monitor", async (long? importId, string? environment, string? status, string? documentTypeCode, string? sourceType, DateTime? createdFrom, DateTime? createdTo, string? search, int? page, int? pageSize, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new SearchSriDocumentMonitorQuery(new SriDocumentMonitorFilter(environment, status, documentTypeCode, sourceType, createdFrom, createdTo, search, page ?? 1, pageSize ?? 50, importId)), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriDocumentsView);

        group.MapGet("/monitor/{id:long}", async (long id, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetSriDocumentMonitorDetailQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriDocumentsViewPayload);

        group.MapGet("/monitor/{id:long}/audit", async (long id, ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetSriDocumentAuditQuery(id), cancellationToken)).ToHttpResult())
            .RequirePermission(PermissionCodes.SriDocumentsViewPayload);

        group.MapGet("/monitor/{id:long}/xml", async (long id, ISender sender, ClaimsPrincipal user, HttpContext httpContext, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DownloadAuthorizedSriXmlCommand(id, auditUser.UserId, auditUser.UserName, Guid.NewGuid()), cancellationToken);
            if (!result.IsSuccess)
            {
                var response = ApiResponse<SriAuthorizedXmlDownloadDto>.Fail(result.Message, result.Errors);
                return result.Errors.Any(error => error.Code == "SRI_DOCUMENT_NOT_FOUND") ? Results.NotFound(response) : Results.BadRequest(response);
            }

            httpContext.Response.Headers.CacheControl = "no-store";
            return Results.File(result.Value!.Content, "application/xml", result.Value.FileName);
        }).RequirePermission(PermissionCodes.SriDocumentsDownloadXml);

        group.MapPost("", async (EnqueueSriDocumentRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new EnqueueSriDocumentCommand(request.Environment, request.AccessKey, request.SourceType, request.SourceReference, request.BranchCode, request.Priority, request.TraceId, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.SriDocumentsEnqueue);

        group.MapPost("/{id:long}/cancel", async (long id, SriDocumentActionRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new CancelSriDocumentCommand(id, request.RowVersion, request.Reason, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.SriDocumentsCancel);

        group.MapPost("/{id:long}/reprocess", async (long id, SriDocumentActionRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new ReprocessSriDocumentCommand(id, request.RowVersion, request.Reason ?? string.Empty, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.SriDocumentsReprocess);

        return app;
    }

    private sealed record EnqueueSriDocumentRequest(string Environment, string AccessKey, string SourceType, string SourceReference, string? BranchCode, int Priority = 5, Guid? TraceId = null);
    private sealed record SriDocumentActionRequest(byte[] RowVersion, string? Reason);
}

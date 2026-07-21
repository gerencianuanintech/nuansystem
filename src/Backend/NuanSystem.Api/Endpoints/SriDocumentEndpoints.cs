using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.SriDocuments.Commands;
using NuanSystem.Application.Features.SriDocuments.Dtos;
using NuanSystem.Application.Features.SriDocuments.Queries;
using NuanSystem.Shared.Constants;

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

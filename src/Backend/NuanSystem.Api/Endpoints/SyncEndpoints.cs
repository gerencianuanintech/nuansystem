using System.Security.Claims;
using MediatR;
using NuanSystem.Application.Features.Sync.Commands;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Queries;
using NuanSystem.Api.Extensions;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Api.Endpoints;

public static class SyncEndpoints
{
    public static IEndpointRouteBuilder MapSyncEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sync/dashboard", async (
            int? take,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSyncDashboardQuery(take ?? 10), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncOutboxView);

        app.MapGet("/api/sync/summary", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSyncSummaryQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncOutboxView);

        app.MapGet("/api/sync/outbox", async (
            SyncEventStatus? status,
            string? entityName,
            Guid? entityGlobalId,
            Guid? eventId,
            int? branchCompanyId,
            DateTime? createdFrom,
            DateTime? createdTo,
            bool? hasErrors,
            bool? deadLetterOnly,
            int? page,
            int? pageSize,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var filter = new SyncOutboxQueryFilter(
                status,
                entityName,
                entityGlobalId,
                eventId,
                branchCompanyId,
                createdFrom,
                createdTo,
                hasErrors,
                deadLetterOnly,
                page ?? 1,
                pageSize ?? 100);
            var result = await sender.Send(new GetSyncOutboxQuery(filter), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncOutboxView);

        app.MapGet("/api/sync/outbox/{id:long}", async (
            long id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSyncOutboxDetailQuery(id), cancellationToken);
            return result.IsSuccess
                ? result.ToHttpResult()
                : Results.NotFound(ApiResponse<SyncOutboxDetailDto>.Fail(result.Message, result.Errors));
        })
        .RequirePermission(PermissionCodes.SyncOutboxView);

        app.MapGet("/api/sync/outbox/{id:long}/targets", async (
            long id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSyncOutboxTargetsQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncOutboxView);

        app.MapGet("/api/sync/audit", async (
            SyncEventStatus? status,
            string? entityName,
            Guid? entityGlobalId,
            Guid? eventId,
            int? branchCompanyId,
            DateTime? createdFrom,
            DateTime? createdTo,
            bool? hasErrors,
            bool? deadLetterOnly,
            int? page,
            int? pageSize,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var filter = new SyncAuditQueryFilter(
                status,
                entityName,
                entityGlobalId,
                eventId,
                branchCompanyId,
                createdFrom,
                createdTo,
                hasErrors,
                deadLetterOnly,
                page ?? 1,
                pageSize ?? 100);
            var result = await sender.Send(new GetSyncAuditQuery(filter), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncAuditView);

        app.MapPost("/api/sync/outbox/{id:long}/retry", async (
            long id,
            RetrySyncOutboxRequest? request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new RetrySyncOutboxCommand(id, request?.Reason, auditUser.UserName),
                cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncOutboxRetry);

        app.MapPost("/api/sync/outbox/{id:long}/retry-deadletter", async (
            long id,
            RetryDeadLetterSyncOutboxRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new RetryDeadLetterSyncOutboxCommand(id, request.Reason, request.ResetAttemptCount, auditUser.UserName),
                cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncOutboxRetryDeadLetter);

        app.MapPost("/api/sync/outbox/retry-batch", async (RetrySyncOutboxBatchRequest request,ISender sender,ClaimsPrincipal user,CancellationToken cancellationToken) =>
        {
            var auditUser=user.GetAuditUser();
            return (await sender.Send(new RetrySyncOutboxBatchCommand(request.Ids,request.Reason,request.ResetDeadLetterAttempts,auditUser.UserName),cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.SyncOutboxRetryDeadLetter);

        app.MapPost("/api/sync/outbox/{id:long}/release-expired-lock", async (
            long id,
            ReleaseExpiredLockRequest? request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new ReleaseExpiredSyncLockCommand(id, request?.Reason, auditUser.UserName),
                cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncOutboxReleaseLock);

        return app;
    }
}

public sealed record RetrySyncOutboxBatchRequest(IReadOnlyCollection<long> Ids,string Reason,bool ResetDeadLetterAttempts=true);

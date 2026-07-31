using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Executions;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class SapSyncExecutionEndpoints
{
    private const string BaseRoute = "/api/sap/sync-executions";

    public static IEndpointRouteBuilder MapSapSyncExecutionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(BaseRoute).WithTags(SwaggerTags.SapBusinessOneSyncExecutions);
        group.MapGet("/", async (long? profileId,string? entityCode,string? direction,string? status,string? triggerType,
            DateTime? dateFromUtc,DateTime? dateToUtc,int? pageNumber,int? pageSize,ISender sender,CancellationToken token) =>
            ToHttp(await sender.Send(new GetSapSyncExecutionsQuery(new(profileId,entityCode,direction,status,triggerType,dateFromUtc,dateToUtc,pageNumber??1,pageSize??50)),token)))
            .RequirePermission(PermissionCodes.SapSyncExecutionsView);

        group.MapGet("/{executionUid:guid}", async (Guid executionUid,ISender sender,CancellationToken token) =>
            ToHttp(await sender.Send(new GetSapSyncExecutionQuery(executionUid),token)))
            .RequirePermission(PermissionCodes.SapSyncExecutionsView);

        group.MapGet("/{executionUid:guid}/details", async (Guid executionUid,string? status,string? sourceRecordKey,int? pageNumber,int? pageSize,ISender sender,CancellationToken token) =>
            ToHttp(await sender.Send(new GetSapSyncExecutionDetailsQuery(new(executionUid,status,sourceRecordKey,pageNumber??1,pageSize??100)),token)))
            .RequirePermission(PermissionCodes.SapSyncExecutionsViewDetail);

        group.MapPost("/{executionUid:guid}/retry", async (Guid executionUid,SapSyncRetryHttpRequest body,ClaimsPrincipal user,ISender sender,CancellationToken token) =>
        {
            var audit=EndpointContextHelper.GetAuditUser(user);
            return ToHttp(await sender.Send(new RetrySapSyncExecutionCommand(new(executionUid,body.ClientRequestId,body.Reason,audit.UserId,audit.UserName,body.RowVersion)),token));
        }).RequirePermission(PermissionCodes.SapSyncExecutionsRetry);

        group.MapPost("/{executionUid:guid}/cancel", async (Guid executionUid,SapSyncVersionHttpRequest body,ClaimsPrincipal user,ISender sender,CancellationToken token) =>
        {
            var audit=EndpointContextHelper.GetAuditUser(user);
            return ToHttp(await sender.Send(new CancelSapSyncExecutionCommand(executionUid,audit.UserId,audit.UserName,body.RowVersion),token));
        }).RequirePermission(PermissionCodes.SapSyncExecutionsCancel);

        group.MapPost("/details/{detailId:long}/release-expired-lock", async (long detailId,SapSyncReleaseLockHttpRequest body,ClaimsPrincipal user,ISender sender,CancellationToken token) =>
        {
            var audit=EndpointContextHelper.GetAuditUser(user);
            return ToHttp(await sender.Send(new ReleaseExpiredSapSyncDetailLockCommand(detailId,body.Reason,audit.UserId,audit.UserName,body.RowVersion),token));
        }).RequirePermission(PermissionCodes.SapSyncExecutionsReleaseExpiredLock);
        return app;
    }

    private static IResult ToHttp<T>(Result<T> result)
    {
        if (result.IsSuccess) return result.ToHttpResult();
        var response=ApiResponse<T>.Fail(result.Message,result.Errors);
        if (result.Errors.Any(x=>x.Code==SapSyncExecutionErrorCodes.NotFound)) return Results.NotFound(response);
        if (result.Errors.Any(x=>x.Code is SapSyncExecutionErrorCodes.ConcurrencyConflict or SapSyncExecutionErrorCodes.RetryNotAllowed or SapSyncExecutionErrorCodes.LockNotExpired)) return Results.Conflict(response);
        return Results.BadRequest(response);
    }
}

public sealed record SapSyncRetryHttpRequest(Guid ClientRequestId,string Reason,byte[] RowVersion);
public sealed record SapSyncVersionHttpRequest(byte[] RowVersion);
public sealed record SapSyncReleaseLockHttpRequest(string Reason,byte[] RowVersion);

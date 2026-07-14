using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Sync.Configuration.Commands;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Configuration.Queries;
using NuanSystem.Application.Features.Sync.Execution.Commands;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Queries;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class SyncConfigurationEndpoints
{
    public static IEndpointRouteBuilder MapSyncConfigurationEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sync/configuration/profiles", async (
            string? search,
            int? companyId,
            bool? isActive,
            string? executionMode,
            int? pageNumber,
            int? pageSize,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(
                new GetSyncProfilesQuery(search, companyId, isActive, executionMode, pageNumber ?? 1, pageSize ?? 50, userId),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncConfigurationView);

        app.MapGet("/api/sync/configuration/profiles/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new GetSyncProfileByIdQuery(id, userId), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationView);

        app.MapGet("/api/sync/configuration/catalog", async (
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new GetSyncConfigurationCatalogQuery(userId), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncConfigurationView);

        app.MapPost("/api/sync/configuration/profiles", async (
            SaveSyncProfileRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new CreateSyncProfileCommand(request, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.IsSuccess
                ? Results.Created($"/api/sync/configuration/profiles/{result.Value}", ApiResponse<int>.Ok(result.Value, result.Message))
                : ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationCreate);

        app.MapPut("/api/sync/configuration/profiles/{id:int}", async (
            int id,
            SaveSyncProfileRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new UpdateSyncProfileCommand(id, request, auditUser.UserId, auditUser.UserName), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationEdit);

        app.MapDelete("/api/sync/configuration/profiles/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new DeleteSyncProfileCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationDelete);

        app.MapPost("/api/sync/configuration/profiles/validate", async (
            SaveSyncProfileRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new ValidateSyncProfileCommand(request, null, userId), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncConfigurationValidate);

        app.MapPost("/api/sync/configuration/profiles/{id:int}/validate", async (
            int id,
            SaveSyncProfileRequest? request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = request is null
                ? await sender.Send(new ValidatePersistedSyncProfileCommand(id, userId), cancellationToken)
                : await sender.Send(new ValidateSyncProfileCommand(request, id, userId), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncConfigurationValidate);

        app.MapPost("/api/sync/configuration/profiles/{id:int}/activate", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new ActivateSyncProfileCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationActivate);

        app.MapPost("/api/sync/configuration/profiles/{id:int}/deactivate", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new DeactivateSyncProfileCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationActivate);

        app.MapPost("/api/sync/configuration/profiles/{id:int}/execute", async (
            int id,
            ExecuteSyncProfileRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new ExecuteSyncProfileCommand(id, request, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.IsSuccess && result.Value is not null
                ? Results.Accepted($"/api/sync/configuration/executions/{result.Value.ExecutionId}", ApiResponse<CreateSyncProfileExecutionResultDto>.Ok(result.Value, result.Message))
                : ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationExecute);

        app.MapGet("/api/sync/configuration/executions", async (
            int? profileId,
            string? status,
            string? executionType,
            DateTimeOffset? dateFrom,
            DateTimeOffset? dateTo,
            int? pageNumber,
            int? pageSize,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetSyncProfileExecutionsQuery(profileId, status, executionType, dateFrom, dateTo, pageNumber ?? 1, pageSize ?? 50),
                cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncConfigurationViewExecutions);

        app.MapGet("/api/sync/configuration/executions/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSyncProfileExecutionByIdQuery(id), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationViewExecutions);

        app.MapPost("/api/sync/configuration/executions/{id:int}/cancel", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new CancelSyncProfileExecutionCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationCancel);

        app.MapPost("/api/sync/configuration/executions/{id:int}/retry", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new RetrySyncProfileExecutionCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationRetry);

        return app;
    }

    private static IResult ToSyncConfigurationHttpResult<T>(NuanSystem.Application.Common.Models.Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.ToHttpResult();
        }

        if (result.Errors.Any(error => error.Code.Contains("NotFound", StringComparison.OrdinalIgnoreCase)))
        {
            return Results.NotFound(ApiResponse<T>.Fail(result.Message, result.Errors));
        }

        if (result.Errors.Any(error => error.Code.Contains("History", StringComparison.OrdinalIgnoreCase)
            || error.Code.Contains("Blocked", StringComparison.OrdinalIgnoreCase)
            || error.Code.Contains("Duplicated", StringComparison.OrdinalIgnoreCase)))
        {
            return Results.Conflict(ApiResponse<T>.Fail(result.Message, result.Errors));
        }

        return result.ToHttpResult();
    }
}

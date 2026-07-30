using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Features.Sync.Configuration.Commands;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Configuration.Queries;
using NuanSystem.Application.Features.Sync.Execution.Commands;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Queries;
using NuanSystem.Application.Features.Sync.Distribution;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class SyncConfigurationEndpoints
{
    public static IEndpointRouteBuilder MapSyncConfigurationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(string.Empty)
            .WithTags(SwaggerTags.MatrixBranchSynchronization);

        group.MapGet("/api/sync/configuration/profiles", async (
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

        group.MapGet("/api/sync/configuration/profiles/{id:int}", async (
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

        group.MapGet("/api/sync/configuration/catalog", async (
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

        group.MapPost("/api/sync/configuration/profiles", async (
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

        group.MapPut("/api/sync/configuration/profiles/{id:int}", async (
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

        group.MapDelete("/api/sync/configuration/profiles/{id:int}", async (
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

        group.MapPost("/api/sync/configuration/profiles/validate", async (
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

        group.MapPost("/api/sync/configuration/profiles/{id:int}/validate", async (
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

        group.MapPost("/api/sync/configuration/profiles/{id:int}/activate", async (
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

        group.MapPost("/api/sync/configuration/profiles/{id:int}/deactivate", async (
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

        group.MapPost("/api/sync/configuration/profiles/{id:int}/execute", async (
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

        group.MapGet("/api/sync/configuration/executions", async (
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

        group.MapGet("/api/sync/configuration/executions/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSyncProfileExecutionByIdQuery(id), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationViewExecutions);

        group.MapPost("/api/sync/configuration/executions/{id:int}/cancel", async (
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

        group.MapPost("/api/sync/configuration/executions/{id:int}/retry", async (
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

        group.MapGet("/api/sync/configuration/distribution-policies/{matrixId:int}", async (
            int matrixId,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new GetSyncDistributionPolicyQuery(matrixId, userId), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationView);

        group.MapGet("/api/sync/configuration/distribution-policies/catalog/{entityCode}", async (
            string entityCode,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSyncDistributionPolicyCatalogQuery(entityCode), cancellationToken);
            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SyncConfigurationView);

        group.MapGet("/api/sync/configuration/distribution-policies/{matrixId:int}/candidates", async (
            int matrixId,
            string? search,
            int? take,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(
                new GetSyncDistributionCandidatesQuery(matrixId, search, take ?? 100, userId),
                cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationView);

        group.MapPut("/api/sync/configuration/distribution-policies/{matrixId:int}", async (
            int matrixId,
            SaveSyncDistributionPolicyRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new UpdateSyncDistributionPolicyCommand(
                matrixId, request, auditUser.UserId, auditUser.UserName), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationEdit);

        group.MapPost("/api/sync/configuration/distribution-policies/{matrixId:int}/preview", async (
            int matrixId,
            PreviewSyncDistributionPolicyRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(new PreviewSyncDistributionPolicyQuery(matrixId, request, userId), cancellationToken);
            return ToSyncConfigurationHttpResult(result);
        })
        .RequirePermission(PermissionCodes.SyncConfigurationValidate);

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

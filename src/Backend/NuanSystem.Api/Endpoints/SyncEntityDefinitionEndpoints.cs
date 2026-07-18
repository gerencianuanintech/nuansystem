using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Commands;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Dtos;
using NuanSystem.Application.Features.Sync.EntityDefinitions.Queries;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class SyncEntityDefinitionEndpoints
{
    private const string BaseRoute = "/api/sync/configuration/entities";

    public static IEndpointRouteBuilder MapSyncEntityDefinitionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(BaseRoute, async (
            string? search,
            bool? isActive,
            int? pageNumber,
            int? pageSize,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(
                new GetSyncEntityDefinitionsQuery(search, isActive, pageNumber ?? 1, pageSize ?? 50),
                cancellationToken);

            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SyncEntityDefinitions)
        .RequirePermission(PermissionCodes.SyncEntitiesView);

        app.MapGet($"{BaseRoute}/lookup", async (
            int? includeId,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSyncEntityDefinitionLookupQuery(includeId), cancellationToken);
            return result.ToHttpResult();
        })
        .WithTags(SwaggerTags.SyncEntityDefinitions)
        .RequirePermission(PermissionCodes.SyncEntitiesView);

        app.MapGet($"{BaseRoute}/{{id:int}}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSyncEntityDefinitionByIdQuery(id), cancellationToken);
            return ToEntityDefinitionHttpResult(result);
        })
        .WithTags(SwaggerTags.SyncEntityDefinitions)
        .RequirePermission(PermissionCodes.SyncEntitiesView);

        app.MapGet($"{BaseRoute}/{{id:int}}/history", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSyncEntityDefinitionHistoryQuery(id), cancellationToken);
            return ToEntityDefinitionHttpResult(result);
        })
        .WithTags(SwaggerTags.SyncEntityDefinitions)
        .RequirePermission(PermissionCodes.SyncEntitiesView);

        app.MapPost(BaseRoute, async (
            CreateSyncEntityDefinitionRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new CreateSyncEntityDefinitionCommand(
                    request.Code,
                    request.Name,
                    request.Description,
                    request.DefaultExecutionOrder,
                    request.SupportsIncremental,
                    request.SupportsInsert,
                    request.SupportsUpdate,
                    request.SupportsDeactivate,
                    request.DefaultKeyField,
                    request.DefaultModifiedAtField,
                    request.IsActive,
                    request.DependencyDefinitionIds,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return result.IsSuccess && result.Value is not null
                ? Results.Created($"{BaseRoute}/{result.Value.Id}", ApiResponse<SyncEntityDefinitionDetailDto>.Ok(result.Value, result.Message))
                : ToEntityDefinitionHttpResult(result);
        })
        .WithTags(SwaggerTags.SyncEntityDefinitions)
        .RequirePermission(PermissionCodes.SyncEntitiesCreate);

        app.MapPut($"{BaseRoute}/{{id:int}}", async (
            int id,
            UpdateSyncEntityDefinitionRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new UpdateSyncEntityDefinitionCommand(
                    id,
                    request.Name,
                    request.Description,
                    request.DefaultExecutionOrder,
                    request.SupportsIncremental,
                    request.SupportsInsert,
                    request.SupportsUpdate,
                    request.SupportsDeactivate,
                    request.DefaultKeyField,
                    request.DefaultModifiedAtField,
                    request.IsActive,
                    request.DependencyDefinitionIds,
                    auditUser.UserId,
                    auditUser.UserName),
                cancellationToken);

            return ToEntityDefinitionHttpResult(result);
        })
        .WithTags(SwaggerTags.SyncEntityDefinitions)
        .RequirePermission(PermissionCodes.SyncEntitiesEdit);

        app.MapDelete($"{BaseRoute}/{{id:int}}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new DeleteSyncEntityDefinitionCommand(id, auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return ToEntityDefinitionHttpResult(result);
        })
        .WithTags(SwaggerTags.SyncEntityDefinitions)
        .RequirePermission(PermissionCodes.SyncEntitiesDelete);

        return app;
    }

    private static IResult ToEntityDefinitionHttpResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.ToHttpResult();
        }

        if (result.Errors.Any(error => error.Code.Contains("NotFound", StringComparison.OrdinalIgnoreCase)))
        {
            return Results.NotFound(ApiResponse<T>.Fail(result.Message, result.Errors));
        }

        if (result.Errors.Any(error =>
                error.Code.Contains("AlreadyExists", StringComparison.OrdinalIgnoreCase)
                || error.Code.Contains("DependencyCycle", StringComparison.OrdinalIgnoreCase)
                || error.Code.Contains("InUse", StringComparison.OrdinalIgnoreCase)
                || error.Code.Contains("Required", StringComparison.OrdinalIgnoreCase)
                || error.Code.Contains("SystemProtected", StringComparison.OrdinalIgnoreCase)))
        {
            return Results.Conflict(ApiResponse<T>.Fail(result.Message, result.Errors));
        }

        return result.ToHttpResult();
    }
}

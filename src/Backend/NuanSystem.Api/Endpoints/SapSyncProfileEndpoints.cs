using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Profiles;
using NuanSystem.Application.Features.SapSync.Profiles.Commands;
using NuanSystem.Application.Features.SapSync.Profiles.Queries;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class SapSyncProfileEndpoints
{
    private const string BaseRoute = "/api/sap/sync-profiles";

    public static IEndpointRouteBuilder MapSapSyncProfileEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(BaseRoute, async (
            int? companyId,
            string? search,
            bool? isActive,
            string? entityCode,
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
                new GetSapSyncProfilesQuery(
                    new SapSyncProfileListRequest(
                        companyId,
                        search,
                        isActive,
                        entityCode,
                        pageNumber ?? 1,
                        pageSize ?? 50),
                    userId),
                cancellationToken);
            return ToSapSyncProfileHttpResult(result);
        })
        .WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
        .RequirePermission(PermissionCodes.SapSyncProfilesView);

        app.MapGet($"{BaseRoute}/catalog", async (
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(
                new GetSapSyncProfileCatalogQuery(userId),
                cancellationToken);
            return ToSapSyncProfileHttpResult(result);
        })
        .WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
        .RequirePermission(PermissionCodes.SapSyncProfilesView);

        app.MapGet($"{BaseRoute}/{{id:long}}", async (
            long id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(
                new GetSapSyncProfileByIdQuery(id, userId),
                cancellationToken);
            return ToSapSyncProfileHttpResult(result);
        })
        .WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
        .RequirePermission(PermissionCodes.SapSyncProfilesView);

        app.MapPost(BaseRoute, async (
            SaveSapSyncProfileRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var audit = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new CreateSapSyncProfileCommand(
                    request,
                    userId,
                    audit.UserId,
                    audit.UserName),
                cancellationToken);
            return result.IsSuccess && result.Value is not null
                ? Results.Created(
                    $"{BaseRoute}/{result.Value.Id}",
                    ApiResponse<SapSyncProfileWriteDto>.Ok(result.Value, result.Message))
                : ToSapSyncProfileHttpResult(result);
        })
        .WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
        .RequirePermission(PermissionCodes.SapSyncProfilesCreate);

        app.MapPut($"{BaseRoute}/{{id:long}}", async (
            long id,
            UpdateSapSyncProfileRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var audit = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new UpdateSapSyncProfileCommand(
                    id,
                    request,
                    userId,
                    audit.UserId,
                    audit.UserName),
                cancellationToken);
            return ToSapSyncProfileHttpResult(result);
        })
        .WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
        .RequirePermission(PermissionCodes.SapSyncProfilesEdit);

        app.MapDelete($"{BaseRoute}/{{id:long}}", async (
            long id,
            [FromBody] SapSyncProfileVersionRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var audit = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new DeleteSapSyncProfileCommand(
                    id,
                    request.RowVersion,
                    userId,
                    audit.UserId,
                    audit.UserName),
                cancellationToken);
            return ToSapSyncProfileHttpResult(result);
        })
        .WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
        .RequirePermission(PermissionCodes.SapSyncProfilesDelete);

        app.MapPost($"{BaseRoute}/{{id:long}}/validate", async (
            long id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await sender.Send(
                new ValidateSapSyncProfileCommand(id, userId),
                cancellationToken);
            return ToSapSyncProfileHttpResult(result);
        })
        .WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
        .RequirePermission(PermissionCodes.SapSyncProfilesValidate);

        app.MapPost($"{BaseRoute}/{{id:long}}/activate", async (
            long id,
            SapSyncProfileVersionRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var audit = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new ActivateSapSyncProfileCommand(
                    id,
                    request.RowVersion,
                    userId,
                    audit.UserId,
                    audit.UserName),
                cancellationToken);
            return ToSapSyncProfileHttpResult(result);
        })
        .WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
        .RequirePermission(PermissionCodes.SapSyncProfilesActivate);

        app.MapPost($"{BaseRoute}/{{id:long}}/deactivate", async (
            long id,
            SapSyncProfileVersionRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var audit = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new DeactivateSapSyncProfileCommand(
                    id,
                    request.RowVersion,
                    userId,
                    audit.UserId,
                    audit.UserName),
                cancellationToken);
            return ToSapSyncProfileHttpResult(result);
        })
        .WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
        .RequirePermission(PermissionCodes.SapSyncProfilesActivate);

        return app;
    }

    internal static IResult ToSapSyncProfileHttpResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.ToHttpResult();
        }

        var response = ApiResponse<T>.Fail(result.Message, result.Errors);
        if (HasError(result, SapSyncProfileErrorCodes.CompanyAccessDenied))
        {
            return Results.Json(response, statusCode: StatusCodes.Status403Forbidden);
        }

        if (HasError(result, SapSyncProfileErrorCodes.NotFound))
        {
            return Results.NotFound(response);
        }

        if (HasError(
                result,
                SapSyncProfileErrorCodes.DuplicateCode,
                SapSyncProfileErrorCodes.ConcurrencyConflict,
                SapSyncProfileErrorCodes.CompanyImmutable))
        {
            return Results.Conflict(response);
        }

        return Results.BadRequest(response);
    }

    private static bool HasError<T>(Result<T> result, params string[] codes) =>
        result.Errors.Any(error => codes.Contains(error.Code, StringComparer.Ordinal));
}

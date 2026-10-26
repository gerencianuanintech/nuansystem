using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Api.OpenApi;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Enums;
using NuanSystem.Application.Features.SapSync.Mapping;
using NuanSystem.Application.Features.SapSync.Profiles.Commands;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class SapSyncMappingEndpoints
{
    private const string MappingRoute = "/api/sap/mapping";
    private const string ProfileRoute = "/api/sap/profiles";

    public static IEndpointRouteBuilder MapSapSyncMappingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(MappingRoute).WithTags(SwaggerTags.SapBusinessOneSyncProfiles);

        group.MapGet("/query-definitions", async (
            string? entityCode,
            string? transport,
            bool? activeOnly,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var result = await sender.Send(
                new GetSapQueryDefinitionsQuery(entityCode, transport, activeOnly ?? true, userId),
                cancellationToken);
            return ToHttp(result);
        }).RequirePermission(PermissionCodes.SapSyncProfilesView);

        group.MapPost("/query-definitions", async (
            SapQueryDefinitionCreateRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var audit = EndpointContextHelper.GetAuditUser(user);
            var model = new SapQueryDefinitionWriteModel(request.EntityCode, request.Transport, request.QueryName, request.QueryText, request.ParameterSchemaJson, request.WatermarkField, request.Version, request.IsApproved, request.IsActive, audit.UserId, audit.UserName);
            var result = await sender.Send(new CreateSapQueryDefinitionCommand(model, userId), cancellationToken);
            return ToHttp(result);
        }).RequirePermission(PermissionCodes.SapSyncProfilesEdit);

        group.MapPatch("/query-definitions/{queryDefinitionId:long}/deactivate", async (
            long queryDefinitionId,
            SapMappingVersionRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var audit = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new DeactivateSapQueryDefinitionCommand(queryDefinitionId, request.RowVersion, userId, audit.UserId, audit.UserName),
                cancellationToken);
            return ToHttp(result);
        }).RequirePermission(PermissionCodes.SapSyncProfilesEdit);

        app.MapGet($"{ProfileRoute}/{{profileId:long}}/entities/{{profileEntityId:long}}/field-mappings", async (
            long profileId,
            long profileEntityId,
            SapSyncDirection? direction,
            bool? activeOnly,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var result = await sender.Send(
                new GetSapFieldMappingsQuery(profileId, profileEntityId, ToMappingDirection(direction), activeOnly ?? true, userId),
                cancellationToken);
            return ToHttp(result);
        }).WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
          .RequirePermission(PermissionCodes.SapSyncProfilesView);

        app.MapPut($"{ProfileRoute}/{{profileId:long}}/entities/{{profileEntityId:long}}/field-mappings", async (
            long profileId,
            long profileEntityId,
            ReplaceSapFieldMappingsRequest request,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var audit = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new ReplaceSapFieldMappingsCommand(profileId, profileEntityId, request.Mappings, userId, audit.UserId, audit.UserName),
                cancellationToken);
            return ToHttp(result);
        }).WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
          .RequirePermission(PermissionCodes.SapSyncProfilesEdit);

        app.MapPost($"{ProfileRoute}/{{profileId:long}}/validate", async (
            long profileId,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var result = await sender.Send(new ValidateSapSyncProfileCommand(profileId, userId), cancellationToken);
            return ToHttp(result);
        }).WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
          .RequirePermission(PermissionCodes.SapSyncProfilesValidate);

        app.MapPost($"{ProfileRoute}/{{profileId:long}}/entities/{{profileEntityId:long}}/preview", async (
            long profileId,
            long profileEntityId,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId)) return Results.Unauthorized();
            var result = await sender.Send(new PreviewSapEntityMappingQuery(profileId, profileEntityId, userId), cancellationToken);
            return ToHttp(result);
        }).WithTags(SwaggerTags.SapBusinessOneSyncProfiles)
          .RequirePermission(PermissionCodes.SapSyncProfilesValidate);

        return app;
    }

    private static IResult ToHttp<T>(Result<T> result)
    {
        if (result.IsSuccess) return result.ToHttpResult();
        var response = ApiResponse<T>.Fail(result.Message, result.Errors);
        if (result.Errors.Any(error => error.Code is "SAP_PROFILE_NOT_FOUND" or "SAP_PROFILE_ENTITY_NOT_FOUND"))
            return Results.NotFound(response);
        if (result.Errors.Any(error => error.Code == "SAP_PROFILE_ACCESS_DENIED"))
            return Results.Forbid();
        return Results.BadRequest(response);
    }

    private static SapMappingDirection? ToMappingDirection(SapSyncDirection? direction)
        => direction switch
        {
            SapSyncDirection.SapToErp => SapMappingDirection.SapToErp,
            SapSyncDirection.ErpToSap => SapMappingDirection.ErpToSap,
            SapSyncDirection.Both => SapMappingDirection.Both,
            _ => null
        };
}

public sealed record SapMappingVersionRequest(byte[] RowVersion);

public sealed record ReplaceSapFieldMappingsRequest(
    IReadOnlyCollection<SapFieldMappingWriteModel> Mappings);

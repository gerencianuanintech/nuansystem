using System.Security.Claims;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.GridColumnSettings.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class GridColumnSettingsEndpoints
{
    public static IEndpointRouteBuilder MapGridColumnSettingsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/security/grid-columns/{formKey}/{gridName}/me", async (
            string formKey,
            string gridName,
            ClaimsPrincipal user,
            IGridColumnSettingsRepository repository,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var settings = await repository.GetUserSettingsAsync(userId, formKey, gridName, cancellationToken);
            return Results.Ok(ApiResponse<IReadOnlyCollection<GridColumnSettingDto>>.Ok(settings));
        })
        .RequireAuthorization();

        app.MapPut("/api/security/grid-columns/{formKey}/{gridName}/me", async (
            string formKey,
            string gridName,
            IReadOnlyCollection<SaveGridColumnSettingData> columns,
            ClaimsPrincipal user,
            IGridColumnSettingsRepository repository,
            CancellationToken cancellationToken) =>
        {
            if (!EndpointContextHelper.TryGetUserId(user, out var userId))
            {
                return Results.Unauthorized();
            }

            var auditUser = EndpointContextHelper.GetAuditUser(user);
            await repository.SaveUserSettingsAsync(userId, formKey, gridName, columns, auditUser.UserId, auditUser.UserName, cancellationToken);
            return Results.Ok(ApiResponse<bool>.Ok(true, "Configuracion de columnas guardada correctamente."));
        })
        .RequireFormOperation("{formKey}", "customizecolumns");

        return app;
    }
}

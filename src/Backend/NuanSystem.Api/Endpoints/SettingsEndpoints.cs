using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Settings.Commands;
using NuanSystem.Application.Features.Settings.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SettingsEndpoints
{
    public static IEndpointRouteBuilder MapSettingsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/settings/parameters", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCompanyParametersQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SettingsManage);

        app.MapPut("/api/settings/parameters/{key}", async (
            string key,
            UpsertCompanyParameterCommand command,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(command with { Key = key }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SettingsManage);

        return app;
    }
}

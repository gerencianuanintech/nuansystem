using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.ConfigurationSettings.Commands;
using NuanSystem.Application.Features.ConfigurationSettings.Queries;

namespace NuanSystem.Api.Endpoints;

public static class ConfigurationSettingEndpoints
{
    public static IEndpointRouteBuilder MapConfigurationSettingEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/configuration/settings", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetConfigurationSettingsQuery(), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("configuration-settings", "refresh");

        app.MapGet("/api/configuration/settings/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetConfigurationSettingByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("configuration-settings", "consult");

        app.MapPost("/api/configuration/settings", async (
            CreateConfigurationSettingCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with { AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("configuration-settings", "create");

        app.MapPut("/api/configuration/settings/{id:int}", async (
            int id,
            UpdateConfigurationSettingCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with { Id = id, AuditUserId = auditUser.UserId, AuditUserName = auditUser.UserName }, cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("configuration-settings", "update");

        app.MapDelete("/api/configuration/settings/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new DeleteConfigurationSettingCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        })
        .RequireFormOperation("configuration-settings", "delete");

        return app;
    }
}

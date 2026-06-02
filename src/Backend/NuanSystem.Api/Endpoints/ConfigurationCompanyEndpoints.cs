using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.ConfigurationCompanies.Commands;
using NuanSystem.Application.Features.ConfigurationCompanies.Queries;

namespace NuanSystem.Api.Endpoints;

public static class ConfigurationCompanyEndpoints
{
    public static IEndpointRouteBuilder MapConfigurationCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/configuration/companies", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetConfigurationCompaniesQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("configuration-companies", "refresh");

        app.MapGet("/api/configuration/companies/{id:int}", async (
            int id,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetConfigurationCompanyByIdQuery(id), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("configuration-companies", "consult");

        app.MapPost("/api/configuration/companies", async (
            CreateConfigurationCompanyCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("configuration-companies", "create");

        app.MapPut("/api/configuration/companies/{id:int}", async (
            int id,
            UpdateConfigurationCompanyCommand command,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(command with
            {
                Id = id,
                AuditUserId = auditUser.UserId,
                AuditUserName = auditUser.UserName
            }, cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("configuration-companies", "update");

        app.MapDelete("/api/configuration/companies/{id:int}", async (
            int id,
            ClaimsPrincipal user,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(new DeleteConfigurationCompanyCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireFormOperation("configuration-companies", "delete");

        return app;
    }
}

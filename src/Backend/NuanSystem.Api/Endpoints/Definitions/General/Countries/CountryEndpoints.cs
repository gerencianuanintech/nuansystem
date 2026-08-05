using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.General.Countries.Commands;
using NuanSystem.Application.Features.Definitions.General.Countries.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.General.Countries;

internal static class CountryEndpoints
{
    public static RouteGroupBuilder MapCountryEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/countries", async (ISender sender, CancellationToken cancellationToken) => (await sender.Send(new GetCountriesQuery(), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyCountriesRead);
        group.MapGet("/countries/lookup", async (ISender sender, CancellationToken cancellationToken) => (await sender.Send(new GetCountryLookupQuery(), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyCountriesRead);
        group.MapGet("/countries/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) => (await sender.Send(new GetCountryByIdQuery(id), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyCountriesRead);
        group.MapPost("/countries", async (SaveCountryRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new CreateCountryCommand(request.Code, request.Name, request.Iso2, request.Iso3, request.PhonePrefix, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCountriesManage);
        group.MapPut("/countries/{id:int}", async (int id, SaveCountryRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new UpdateCountryCommand(id, request.Code, request.Name, request.Iso2, request.Iso3, request.PhonePrefix, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCountriesManage);
        group.MapDelete("/countries/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new DeleteCountryCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCountriesManage);
        return group;
    }

    private sealed record SaveCountryRequest(string Code, string Name, string? Iso2, string? Iso3, string? PhonePrefix, bool IsActive);
}

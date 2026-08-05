using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.General.Cities.Commands;
using NuanSystem.Application.Features.Definitions.General.Cities.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.General.Cities;

internal static class CityEndpoints
{
    public static RouteGroupBuilder MapCityEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/cities", async (ISender sender, CancellationToken cancellationToken) => (await sender.Send(new GetCitiesQuery(), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyCitiesRead);
        group.MapGet("/cities/lookup", async (string? countryCode, string? provinceCode, ISender sender, CancellationToken cancellationToken) => (await sender.Send(new GetCityLookupQuery(countryCode, provinceCode), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyCitiesRead);
        group.MapGet("/cities/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) => (await sender.Send(new GetCityByIdQuery(id), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyCitiesRead);
        group.MapPost("/cities", async (SaveCityRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new CreateCityCommand(request.CountryId, request.ProvinceId, request.Code, request.Name, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesManage);
        group.MapPut("/cities/{id:int}", async (int id, SaveCityRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new UpdateCityCommand(id, request.CountryId, request.ProvinceId, request.Code, request.Name, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesManage);
        group.MapDelete("/cities/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new DeleteCityCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesManage);
        return group;
    }

    private sealed record SaveCityRequest(int CountryId, int ProvinceId, string Code, string Name, bool IsActive);
}

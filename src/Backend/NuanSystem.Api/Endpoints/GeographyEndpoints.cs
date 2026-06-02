using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Geography.Commands;
using NuanSystem.Application.Features.Geography.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class GeographyEndpoints
{
    public static IEndpointRouteBuilder MapGeographyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/geography");

        group.MapGet("/countries", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCountriesQuery(), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCountriesRead);

        group.MapGet("/countries/lookup", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCountryLookupQuery(), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCountriesRead);

        group.MapGet("/countries/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCountryByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCountriesRead);

        group.MapPost("/countries", async (SaveCountryRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new CreateCountryCommand(request.Code, request.Name, request.Iso2, request.Iso3, request.PhonePrefix, request.IsActive, auditUser.UserId, auditUser.UserName),
                cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCountriesManage);

        group.MapPut("/countries/{id:int}", async (int id, SaveCountryRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(
                new UpdateCountryCommand(id, request.Code, request.Name, request.Iso2, request.Iso3, request.PhonePrefix, request.IsActive, auditUser.UserId, auditUser.UserName),
                cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCountriesManage);

        group.MapDelete("/countries/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteCountryCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCountriesManage);

        group.MapGet("/provinces", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetProvincesQuery(), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyProvincesRead);

        group.MapGet("/provinces/lookup", async (string? countryCode, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetProvinceLookupQuery(countryCode), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyProvincesRead);

        group.MapGet("/provinces/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetProvinceByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyProvincesRead);

        group.MapPost("/provinces", async (SaveProvinceRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new CreateProvinceCommand(request.CountryId, request.Code, request.Name, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyProvincesManage);

        group.MapPut("/provinces/{id:int}", async (int id, SaveProvinceRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new UpdateProvinceCommand(id, request.CountryId, request.Code, request.Name, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyProvincesManage);

        group.MapDelete("/provinces/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteProvinceCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyProvincesManage);

        group.MapGet("/cities", async (ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCitiesQuery(), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesRead);

        group.MapGet("/cities/lookup", async (string? countryCode, string? provinceCode, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCityLookupQuery(countryCode, provinceCode), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesRead);

        group.MapGet("/reverse-geocode", async (decimal latitude, decimal longitude, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new ReverseGeocodeQuery(latitude, longitude), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesRead);

        group.MapGet("/static-map", async (decimal latitude, decimal longitude, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetStaticMapQuery(latitude, longitude), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesRead);

        group.MapGet("/cities/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetCityByIdQuery(id), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesRead);

        group.MapPost("/cities", async (SaveCityRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new CreateCityCommand(request.CountryId, request.ProvinceId, request.Code, request.Name, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesManage);

        group.MapPut("/cities/{id:int}", async (int id, SaveCityRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new UpdateCityCommand(id, request.CountryId, request.ProvinceId, request.Code, request.Name, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesManage);

        group.MapDelete("/cities/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            var result = await sender.Send(new DeleteCityCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken);
            return result.ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyCitiesManage);

        return app;
    }

    private sealed record SaveCountryRequest(string Code, string Name, string? Iso2, string? Iso3, string? PhonePrefix, bool IsActive);

    private sealed record SaveProvinceRequest(int CountryId, string Code, string Name, bool IsActive);

    private sealed record SaveCityRequest(int CountryId, int ProvinceId, string Code, string Name, bool IsActive);
}

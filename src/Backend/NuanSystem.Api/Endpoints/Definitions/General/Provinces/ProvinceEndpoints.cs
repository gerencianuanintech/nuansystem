using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.Definitions.General.Provinces.Commands;
using NuanSystem.Application.Features.Definitions.General.Provinces.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints.Definitions.General.Provinces;

internal static class ProvinceEndpoints
{
    private const string FormKey = "provinces";

    public static RouteGroupBuilder MapProvinceEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/provinces", async (ISender sender, CancellationToken cancellationToken) => (await sender.Send(new GetProvincesQuery(), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyProvincesRead).RequireFormOperation(FormKey, "refresh");
        group.MapGet("/provinces/lookup", async (string? countryCode, ISender sender, CancellationToken cancellationToken) => (await sender.Send(new GetProvinceLookupQuery(countryCode), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyProvincesRead);
        group.MapGet("/provinces/{id:int}", async (int id, ISender sender, CancellationToken cancellationToken) => (await sender.Send(new GetProvinceByIdQuery(id), cancellationToken)).ToHttpResult()).RequirePermission(PermissionCodes.GeographyProvincesRead).RequireFormOperation(FormKey, "consult");
        group.MapPost("/provinces", async (SaveProvinceRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new CreateProvinceCommand(request.CountryId, request.Code, request.Name, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyProvincesManage).RequireFormOperation(FormKey, "create");
        group.MapPut("/provinces/{id:int}", async (int id, SaveProvinceRequest request, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new UpdateProvinceCommand(id, request.CountryId, request.Code, request.Name, request.IsActive, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyProvincesManage).RequireFormOperation(FormKey, "update");
        group.MapDelete("/provinces/{id:int}", async (int id, ISender sender, ClaimsPrincipal user, CancellationToken cancellationToken) =>
        {
            var auditUser = user.GetAuditUser();
            return (await sender.Send(new DeleteProvinceCommand(id, auditUser.UserId, auditUser.UserName), cancellationToken)).ToHttpResult();
        }).RequirePermission(PermissionCodes.GeographyProvincesManage).RequireFormOperation(FormKey, "delete");
        return group;
    }

    private sealed record SaveProvinceRequest(int CountryId, string Code, string Name, bool IsActive);
}

using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Features.SapSync.Commands;
using NuanSystem.Application.Features.SapSync.Queries;
using NuanSystem.Shared.Constants;

namespace NuanSystem.Api.Endpoints;

public static class SapEndpoints
{
    public static IEndpointRouteBuilder MapSapEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/sap/sync-logs", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSapSyncLogsQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapRead);

        app.MapGet("/api/sap/suppliers/preview", async (
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new PreviewSuppliersFromSapQuery(), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapRead);

        app.MapPost("/api/sap/suppliers/import", async (
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var result = await sender.Send(
                new ImportSuppliersFromSapCommand(auditUser.UserId, auditUser.UserName),
                cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.SapManage);

        return app;
    }
}

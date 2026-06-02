using System.Security.Claims;
using MediatR;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Audit.Dtos;
using NuanSystem.Application.Features.Audit.Queries;
using NuanSystem.Shared.Constants;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Endpoints;

public static class AuditEndpoints
{
    public static IEndpointRouteBuilder MapAuditEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/audit/logs", async (
            int? take,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetAuditLogsQuery(take ?? 200), cancellationToken);

            return result.ToHttpResult();
        })
        .RequirePermission(PermissionCodes.AuditRead);

        app.MapGet("/api/audit/security-changes", async (
            string entityName,
            string recordId,
            int? take,
            ISender sender,
            CancellationToken cancellationToken) =>
        {
            var result = await sender.Send(new GetSecurityChangesQuery(entityName, recordId, take ?? 200), cancellationToken);

            return result.ToHttpResult();
        })
        .RequireSecurityHistoryOperation();

        app.MapGet("/api/audit/inventory-changes", async (
            string entityName,
            string recordId,
            int? take,
            IInventoryAuditRepository repository,
            CancellationToken cancellationToken) =>
        {
            var changes = await repository.GetChangesAsync(
                entityName.Trim(),
                recordId.Trim(),
                Math.Clamp(take ?? 200, 1, 500),
                cancellationToken);

            return Results.Ok(ApiResponse<IReadOnlyCollection<SecurityChangeDto>>.Ok(changes));
        })
        .RequirePermission(PermissionCodes.ItemsRead);

        app.MapGet("/api/audit/error-logs", async (
            int? take,
            IAuditLogRepository repository,
            CancellationToken cancellationToken) =>
        {
            var logs = await repository.GetRecentErrorsAsync(Math.Clamp(take ?? 200, 1, 500), cancellationToken);
            return Results.Ok(ApiResponse<IReadOnlyCollection<AuditErrorLogDto>>.Ok(logs));
        })
        .RequirePermission(PermissionCodes.AuditRead);

        app.MapPost("/api/audit/error-logs", async (
            CreateAuditErrorLogData request,
            ClaimsPrincipal user,
            HttpContext httpContext,
            IAuditLogRepository repository,
            CancellationToken cancellationToken) =>
        {
            var auditUser = EndpointContextHelper.GetAuditUser(user);
            var errorLog = new CreateAuditErrorLogData(
                EndpointContextHelper.Trim(request.Source, 30) ?? "WinForms",
                auditUser.UserId,
                auditUser.UserName,
                EndpointContextHelper.Trim(httpContext.Request.Headers["X-Company-Code"].FirstOrDefault(), 50),
                EndpointContextHelper.Trim(request.ModuleKey, 120),
                EndpointContextHelper.Trim(request.FormName, 180),
                EndpointContextHelper.Trim(request.ActionName, 120),
                EndpointContextHelper.Trim(request.HttpMethod, 12),
                EndpointContextHelper.Trim(request.Path, 500),
                EndpointContextHelper.Trim(request.QueryString, 1000),
                request.StatusCode,
                EndpointContextHelper.Trim(request.ErrorMessage, 2000) ?? "Error no controlado en cliente.",
                EndpointContextHelper.Trim(request.ExceptionType, 300),
                request.StackTrace,
                EndpointContextHelper.Trim(request.TraceId, 120),
                EndpointContextHelper.Trim(httpContext.Connection.RemoteIpAddress?.ToString(), 64),
                EndpointContextHelper.Trim(request.MachineName, 120),
                EndpointContextHelper.Trim(httpContext.Request.Headers.UserAgent.FirstOrDefault(), 500));

            await repository.AddErrorAsync(errorLog, cancellationToken);
            return Results.Ok(ApiResponse<bool>.Ok(true, "Error registrado correctamente."));
        })
        .RequireAuthorization();

        return app;
    }
}

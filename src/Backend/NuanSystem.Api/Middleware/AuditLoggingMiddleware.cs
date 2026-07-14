using System.Security.Claims;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Api.Middleware;

public sealed class AuditLoggingMiddleware
{
    private static readonly HashSet<string> AuditedMethods = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Post,
        HttpMethods.Put,
        HttpMethods.Delete
    };

    private readonly RequestDelegate next;
    private readonly ILogger<AuditLoggingMiddleware> logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IAuditLogRepository repository)
    {
        await next(context);

        if (!AuditedMethods.Contains(context.Request.Method))
        {
            return;
        }

        try
        {
            var userId = context.User.TryGetUserId(out var parsedUserId) ? parsedUserId : (int?)null;

            var auditLog = new CreateAuditLogData(
                userId,
                Trim(context.User.FindFirstValue(ClaimTypes.Name) ?? context.User.Identity?.Name, 120),
                Trim(context.Request.Headers["X-Company-Code"].FirstOrDefault(), 50),
                Trim(context.Request.Method, 12) ?? context.Request.Method,
                Trim(context.Request.Path.Value, 500) ?? string.Empty,
                Trim(context.Request.QueryString.Value, 1000),
                context.Response.StatusCode,
                Trim(context.Connection.RemoteIpAddress?.ToString(), 64),
                Trim(context.Request.Headers.UserAgent.FirstOrDefault(), 500));

            await repository.AddAsync(auditLog, context.RequestAborted);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "No fue posible registrar la auditoria de la solicitud {Method} {Path}.", context.Request.Method, context.Request.Path);
        }
    }

    private static string? Trim(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length <= maxLength ? value : value[..maxLength];
    }
}

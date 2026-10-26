using System.Security.Claims;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Diagnostics;
using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Api.Middleware;

public interface IExceptionAuditWriter
{
    Task WriteAsync(HttpContext context, Exception exception, int statusCode);
}

internal sealed class ExceptionAuditWriter(
    IAuditLogRepository repository,
    IErrorSanitizer sanitizer,
    ILogger<ExceptionAuditWriter> logger) : IExceptionAuditWriter
{
    public async Task WriteAsync(HttpContext context, Exception exception, int statusCode)
    {
        logger.LogError(
            "Request failed with {ExceptionType} while processing {Method} {Path}. TraceId: {TraceId}",
            exception.GetType().FullName,
            context.Request.Method,
            sanitizer.Sanitize(context.Request.Path.Value),
            context.TraceIdentifier);

        try
        {
            var userId = context.User.TryGetUserId(out var parsedUserId) ? parsedUserId : (int?)null;
            var rootCause = exception.GetBaseException();
            var errorLog = new CreateAuditErrorLogData(
                "Backend",
                userId,
                Trim(context.User.FindFirstValue(ClaimTypes.Name) ?? context.User.Identity?.Name, 120),
                Trim(context.Request.Headers["X-Company-Code"].FirstOrDefault(), 50),
                null,
                null,
                null,
                Trim(context.Request.Method, 12),
                Trim(sanitizer.Sanitize(context.Request.Path.Value), 500),
                Trim(sanitizer.Sanitize(context.Request.QueryString.Value), 1000),
                statusCode,
                Trim(sanitizer.Sanitize(rootCause.Message), 2000) ?? "Error no controlado.",
                Trim(rootCause.GetType().FullName, 300),
                sanitizer.Sanitize(exception.ToString()),
                Trim(context.TraceIdentifier, 120),
                Trim(context.Connection.RemoteIpAddress?.ToString(), 64),
                Trim(Environment.MachineName, 120),
                Trim(sanitizer.Sanitize(context.Request.Headers.UserAgent.FirstOrDefault()), 500));

            await repository.AddErrorAsync(errorLog, context.RequestAborted);
        }
        catch (Exception logException)
        {
            logger.LogWarning(
                "Error audit failed with {ExceptionType} for {Method} {Path}. TraceId: {TraceId}",
                logException.GetType().FullName,
                context.Request.Method,
                sanitizer.Sanitize(context.Request.Path.Value),
                context.TraceIdentifier);
        }
    }

    private static string? Trim(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return value.Length <= maxLength ? value : value[..maxLength];
    }
}

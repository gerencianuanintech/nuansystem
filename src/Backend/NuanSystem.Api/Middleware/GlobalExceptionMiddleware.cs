using System.Net;
using System.Security.Claims;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Common.Exceptions;
using NuanSystem.Application.Features.Audit.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(HttpContext context, IAuditLogRepository auditLogRepository)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            if (exception is ApplicationValidationException validationException)
            {
                await WriteValidationErrorAsync(context, validationException);
                return;
            }

            logger.LogError(exception, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await TryWriteErrorLogAsync(context, auditLogRepository, exception);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errors = environment.IsDevelopment()
                ? new[] { new ApiError("UnhandledException", exception.Message) }
                : Array.Empty<ApiError>();

            var response = ApiResponse<object>.Fail("Ocurrio un error interno procesando la solicitud.", errors);
            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private async Task TryWriteErrorLogAsync(
        HttpContext context,
        IAuditLogRepository repository,
        Exception exception)
    {
        try
        {
            var userIdValue = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = int.TryParse(userIdValue, out var parsedUserId) ? parsedUserId : (int?)null;

            var errorLog = new CreateAuditErrorLogData(
                "Backend",
                userId,
                Trim(context.User.FindFirstValue(ClaimTypes.Name) ?? context.User.Identity?.Name, 120),
                Trim(context.Request.Headers["X-Company-Code"].FirstOrDefault(), 50),
                null,
                null,
                null,
                Trim(context.Request.Method, 12),
                Trim(context.Request.Path.Value, 500),
                Trim(context.Request.QueryString.Value, 1000),
                (int)HttpStatusCode.InternalServerError,
                Trim(exception.Message, 2000) ?? "Error no controlado.",
                Trim(exception.GetType().FullName, 300),
                exception.ToString(),
                Trim(context.TraceIdentifier, 120),
                Trim(context.Connection.RemoteIpAddress?.ToString(), 64),
                Trim(Environment.MachineName, 120),
                Trim(context.Request.Headers.UserAgent.FirstOrDefault(), 500));

            await repository.AddErrorAsync(errorLog, context.RequestAborted);
        }
        catch (Exception logException)
        {
            logger.LogWarning(logException, "No fue posible registrar la auditoria del error {Method} {Path}.", context.Request.Method, context.Request.Path);
        }
    }

    private static async Task WriteValidationErrorAsync(
        HttpContext context,
        ApplicationValidationException exception)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(exception.Message, exception.Errors);
        await context.Response.WriteAsJsonAsync(response);
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

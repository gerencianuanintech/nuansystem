using System.Net;
using System.Security.Claims;
using NuanSystem.Api.Extensions;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Diagnostics;
using NuanSystem.Application.Common.Exceptions;
using NuanSystem.Application.Features.Audit.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Middleware;

public sealed class GlobalExceptionMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionMiddleware> logger,
    IHostEnvironment environment)
{
    public async Task InvokeAsync(
        HttpContext context,
        IAuditLogRepository auditLogRepository,
        IDependencyFailureClassifier failureClassifier,
        IErrorSanitizer sanitizer)
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

            var failure = failureClassifier.Classify(exception);
            var error = ClassifyException(failure, context.TraceIdentifier);

            logger.LogError(
                "Request failed with {ExceptionType} while processing {Method} {Path}. TraceId: {TraceId}",
                exception.GetType().FullName,
                context.Request.Method,
                sanitizer.Sanitize(context.Request.Path.Value),
                context.TraceIdentifier);

            await TryWriteErrorLogAsync(
                context,
                auditLogRepository,
                exception,
                error.StatusCode,
                sanitizer);

            context.Response.StatusCode = error.StatusCode;
            context.Response.ContentType = "application/json";

            var errors = environment.IsDevelopment()
                ? AddTechnicalDetail(error.Errors, failure, exception, sanitizer)
                : error.Errors;

            var response = ApiResponse<object>.Fail(error.Message, errors);
            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private async Task TryWriteErrorLogAsync(
        HttpContext context,
        IAuditLogRepository repository,
        Exception exception,
        int statusCode,
        IErrorSanitizer sanitizer)
    {
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

    private static async Task WriteValidationErrorAsync(
        HttpContext context,
        ApplicationValidationException validationFailure)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        var response = ApiResponse<object>.Fail(validationFailure.Message, validationFailure.Errors);
        await context.Response.WriteAsJsonAsync(response);
    }

    private static ClassifiedError ClassifyException(
        DependencyFailureException? failure,
        string traceId)
    {
        if (failure is null)
        {
            return new(
                StatusCodes.Status500InternalServerError,
                "Ocurrio un error interno procesando la solicitud.",
                [new ApiError("UNHANDLED_EXCEPTION", $"Error no controlado. Codigo de seguimiento: {traceId}")]);
        }

        var statusCode = failure.Dependency switch
        {
            DependencyKind.Sap => StatusCodes.Status502BadGateway,
            DependencyKind.Database or DependencyKind.Network or DependencyKind.Timeout when failure.IsRetryable =>
                StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };

        return new(
            statusCode,
            failure.Message,
            [new ApiError(failure.SafeErrorCode, $"Codigo de seguimiento: {traceId}")]);
    }

    private static IReadOnlyCollection<ApiError> AddTechnicalDetail(
        IReadOnlyCollection<ApiError> errors,
        DependencyFailureException? failure,
        Exception exception,
        IErrorSanitizer sanitizer)
    {
        var detail = sanitizer.Sanitize(failure?.DiagnosticContext)
            ?? exception.GetType().FullName
            ?? exception.GetType().Name;

        return errors.Concat([new ApiError("TECHNICAL_DETAIL", detail)]).ToArray();
    }

    private static string? Trim(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length <= maxLength ? value : value[..maxLength];
    }

    private sealed record ClassifiedError(
        int StatusCode,
        string Message,
        IReadOnlyCollection<ApiError> Errors);
}

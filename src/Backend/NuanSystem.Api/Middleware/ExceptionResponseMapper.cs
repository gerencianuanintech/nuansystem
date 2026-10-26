using NuanSystem.Application.Abstractions.Diagnostics;
using NuanSystem.Application.Common.Exceptions;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Middleware;

public interface IExceptionResponseMapper
{
    ExceptionResponse Map(Exception exception, string traceId);
}

internal sealed class ExceptionResponseMapper(
    IDependencyFailureClassifier failureClassifier,
    IErrorSanitizer sanitizer,
    IHostEnvironment environment) : IExceptionResponseMapper
{
    public ExceptionResponse Map(Exception exception, string traceId)
    {
        if (exception is ApplicationValidationException validationFailure)
        {
            return new(
                StatusCodes.Status400BadRequest,
                ApiResponse<object>.Fail(validationFailure.Message, validationFailure.Errors),
                ShouldAudit: false);
        }

        var failure = failureClassifier.Classify(exception);
        var statusCode = ResolveStatusCode(failure);
        var message = failure?.Message ?? "Ocurrio un error interno procesando la solicitud.";
        IReadOnlyCollection<ApiError> errors =
        [
            new(
                failure?.SafeErrorCode ?? "UNHANDLED_EXCEPTION",
                failure is null
                    ? $"Error no controlado. Codigo de seguimiento: {traceId}"
                    : $"Codigo de seguimiento: {traceId}")
        ];

        if (environment.IsDevelopment())
        {
            var detail = sanitizer.Sanitize(failure?.DiagnosticContext)
                ?? exception.GetType().FullName
                ?? exception.GetType().Name;
            errors = errors.Concat([new ApiError("TECHNICAL_DETAIL", detail)]).ToArray();
        }

        return new(statusCode, ApiResponse<object>.Fail(message, errors), ShouldAudit: true);
    }

    private static int ResolveStatusCode(DependencyFailureException? failure)
    {
        if (failure is null) return StatusCodes.Status500InternalServerError;

        return failure.Dependency switch
        {
            DependencyKind.Sap => StatusCodes.Status502BadGateway,
            DependencyKind.Database or DependencyKind.Network or DependencyKind.Timeout when failure.IsRetryable =>
                StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}

public sealed record ExceptionResponse(
    int StatusCode,
    ApiResponse<object> Body,
    bool ShouldAudit);

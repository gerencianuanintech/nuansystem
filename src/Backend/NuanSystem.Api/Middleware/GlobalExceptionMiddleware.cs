using System.Net;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using NuanSystem.Api.Extensions;
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

            var error = ClassifyException(exception, context);

            context.Response.StatusCode = error.StatusCode;
            context.Response.ContentType = "application/json";

            var errors = environment.IsDevelopment()
                ? error.Errors.Concat([new ApiError("TechnicalDetail", exception.Message)]).ToArray()
                : error.Errors;

            var response = ApiResponse<object>.Fail(error.Message, errors);
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
            var userId = context.User.TryGetUserId(out var parsedUserId) ? parsedUserId : (int?)null;

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

    private static ClassifiedError ClassifyException(Exception exception, HttpContext context)
    {
        var traceId = context.TraceIdentifier;

        if (TryFindException<SqlException>(exception, out var sqlException))
        {
            return ClassifySqlException(sqlException, traceId);
        }

        if (TryFindException<SapServiceLayerException>(exception, out var sapException))
        {
            var statusDetail = sapException.StatusCode is null
                ? "SAP Service Layer no estuvo disponible."
                : $"SAP Service Layer respondio HTTP {sapException.StatusCode}.";
            var codeDetail = string.IsNullOrWhiteSpace(sapException.SapErrorCode)
                ? string.Empty
                : $" Codigo SAP: {sapException.SapErrorCode}.";

            return new ClassifiedError(
                StatusCodes.Status502BadGateway,
                "SAP Business One no pudo completar la operacion solicitada.",
                [new ApiError(
                    "SapServiceLayerRejected",
                    $"{statusDetail}{codeDetail} Codigo de seguimiento: {traceId}")]);
        }

        if (exception is TimeoutException)
        {
            return new ClassifiedError(
                StatusCodes.Status503ServiceUnavailable,
                "La base de datos no respondio a tiempo. Verifica que el servidor este encendido y que la red este disponible.",
                [new ApiError("DatabaseTimeout", $"La operacion supero el tiempo de espera. Codigo de seguimiento: {traceId}")]);
        }

        if (exception is InvalidOperationException invalidOperation
            && invalidOperation.Message.Contains("ConnectionStrings:", StringComparison.OrdinalIgnoreCase))
        {
            return new ClassifiedError(
                StatusCodes.Status500InternalServerError,
                "La conexion a la base de datos no esta configurada. Revisa la configuracion del servidor antes de continuar.",
                [new ApiError("DatabaseConfigurationMissing", $"Falta una cadena de conexion requerida. Codigo de seguimiento: {traceId}")]);
        }

        return new ClassifiedError(
            StatusCodes.Status500InternalServerError,
            "Ocurrio un error interno procesando la solicitud.",
            [new ApiError("UnhandledException", $"Error no controlado. Codigo de seguimiento: {traceId}")]);
    }

    private static ClassifiedError ClassifySqlException(SqlException exception, string traceId)
    {
        var numbers = exception.Errors.Cast<SqlError>().Select(error => error.Number).ToHashSet();

        if (numbers.Overlaps([-2, 2, 53, 64, 233, 10053, 10054, 10060, 10061, 11001]))
        {
            return new ClassifiedError(
                StatusCodes.Status503ServiceUnavailable,
                "No se pudo conectar con la base de datos. Verifica que el servidor SQL este encendido y accesible desde esta maquina.",
                [new ApiError("DatabaseUnavailable", $"Servidor SQL no disponible o red inaccesible. Codigo de seguimiento: {traceId}")]);
        }

        if (numbers.Contains(18456))
        {
            return new ClassifiedError(
                StatusCodes.Status503ServiceUnavailable,
                "No se pudo iniciar sesion en la base de datos. Verifica el usuario y clave configurados para SQL Server.",
                [new ApiError("DatabaseLoginFailed", $"Credenciales de base de datos rechazadas. Codigo de seguimiento: {traceId}")]);
        }

        if (numbers.Contains(4060))
        {
            return new ClassifiedError(
                StatusCodes.Status503ServiceUnavailable,
                "La base de datos configurada no esta disponible. Verifica que exista y que el usuario tenga permisos para abrirla.",
                [new ApiError("DatabaseNotAvailable", $"La base de datos no pudo abrirse. Codigo de seguimiento: {traceId}")]);
        }

        return new ClassifiedError(
            StatusCodes.Status500InternalServerError,
            "Ocurrio un error de base de datos procesando la solicitud.",
            [new ApiError("DatabaseError", $"SQL Server devolvio un error. Codigo de seguimiento: {traceId}")]);
    }

    private static bool TryFindException<TException>(Exception exception, out TException typedException)
        where TException : Exception
    {
        var current = exception;
        while (current is not null)
        {
            if (current is TException match)
            {
                typedException = match;
                return true;
            }

            current = current.InnerException;
        }

        typedException = null!;
        return false;
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

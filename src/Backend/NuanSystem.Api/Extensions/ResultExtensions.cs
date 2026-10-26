using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(ApiResponse<T>.Ok(result.Value!, result.Message));
        }

        var statusCode = result.FailureKind switch
        {
            null or FailureKind.Validation => StatusCodes.Status400BadRequest,
            FailureKind.NotFound => StatusCodes.Status404NotFound,
            FailureKind.Conflict => StatusCodes.Status409Conflict,
            FailureKind.Forbidden => StatusCodes.Status403Forbidden,
            FailureKind.Unavailable => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            const string safeMessage = "Ocurrio un error inesperado. Consulte el estado de la operacion o contacte al administrador.";
            return Results.Json(ApiResponse<T>.Fail(safeMessage, [new ApiError("UNEXPECTED_ERROR", safeMessage)]),
                statusCode: statusCode);
        }

        return Results.Json(ApiResponse<T>.Fail(result.Message, result.Errors), statusCode: statusCode);
    }
}

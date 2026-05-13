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

        return Results.BadRequest(ApiResponse<T>.Fail(result.Message, result.Errors));
    }
}

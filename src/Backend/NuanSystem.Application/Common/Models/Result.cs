using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Common.Models;

public sealed record Result<T>(
    bool IsSuccess,
    string Message,
    T? Value,
    IReadOnlyCollection<ApiError> Errors)
{
    public FailureKind? FailureKind { get; init; }

    public static Result<T> Success(T value, string message = "Operacion completada correctamente")
    {
        return new Result<T>(true, message, value, Array.Empty<ApiError>());
    }

    public static Result<T> Failure(string message, IReadOnlyCollection<ApiError>? errors = null)
    {
        return Failure(message, global::NuanSystem.Application.Common.Models.FailureKind.Validation, errors);
    }

    public static Result<T> Failure(string message, FailureKind kind, IReadOnlyCollection<ApiError>? errors = null)
    {
        return new Result<T>(false, message, default, errors ?? Array.Empty<ApiError>()) { FailureKind = kind };
    }
}

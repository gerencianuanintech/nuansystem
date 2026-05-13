namespace NuanSystem.Shared.Responses;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    IReadOnlyCollection<ApiError> Errors)
{
    public static ApiResponse<T> Ok(T data, string message = "Operacion completada correctamente")
    {
        return new ApiResponse<T>(true, message, data, Array.Empty<ApiError>());
    }

    public static ApiResponse<T> Fail(string message, IReadOnlyCollection<ApiError>? errors = null)
    {
        return new ApiResponse<T>(false, message, default, errors ?? Array.Empty<ApiError>());
    }
}

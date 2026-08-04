using NuanSystem.Shared.Responses;

namespace NuanSystem.WinForms.Services.Http;

public sealed class ApiClientException : Exception
{
    public ApiClientException(
        string message,
        int? statusCode = null,
        IReadOnlyCollection<ApiError>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors ?? Array.Empty<ApiError>();
    }

    public int? StatusCode { get; }
    public IReadOnlyCollection<ApiError> Errors { get; }
}

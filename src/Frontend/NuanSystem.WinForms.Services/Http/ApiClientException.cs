namespace NuanSystem.WinForms.Services.Http;

public sealed class ApiClientException : Exception
{
    public ApiClientException(string message, int? statusCode = null)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public int? StatusCode { get; }
}

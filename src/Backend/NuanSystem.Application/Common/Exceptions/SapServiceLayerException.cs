namespace NuanSystem.Application.Common.Exceptions;

public sealed class SapServiceLayerException : Exception
{
    public SapServiceLayerException(
        string operation,
        int? statusCode = null,
        string? sapErrorCode = null,
        string? sapErrorMessage = null,
        Exception? innerException = null)
        : base(BuildMessage(operation, statusCode, sapErrorCode, sapErrorMessage), innerException)
    {
        Operation = operation;
        StatusCode = statusCode;
        SapErrorCode = sapErrorCode;
        SapErrorMessage = sapErrorMessage;
    }

    public string Operation { get; }
    public int? StatusCode { get; }
    public string? SapErrorCode { get; }
    public string? SapErrorMessage { get; }

    private static string BuildMessage(
        string operation,
        int? statusCode,
        string? sapErrorCode,
        string? sapErrorMessage)
    {
        var status = statusCode is null ? string.Empty : $" HTTP {statusCode}.";
        var code = string.IsNullOrWhiteSpace(sapErrorCode) ? string.Empty : $" Codigo SAP: {sapErrorCode}.";
        var message = string.IsNullOrWhiteSpace(sapErrorMessage) ? string.Empty : $" Mensaje SAP: {sapErrorMessage}";

        return $"SAP Service Layer no pudo {operation}.{status}{code}{message}";
    }
}

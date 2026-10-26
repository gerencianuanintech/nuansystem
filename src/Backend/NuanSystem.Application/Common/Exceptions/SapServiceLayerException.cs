namespace NuanSystem.Application.Common.Exceptions;

public sealed class SapServiceLayerException : DependencyFailureException
{
    public SapServiceLayerException(
        string operation,
        int? statusCode = null,
        string? sapErrorCode = null,
        string? sapErrorMessage = null,
        Exception? innerException = null)
        : base(
            DependencyKind.Sap,
            "SAP_SERVICE_LAYER_REJECTED",
            "SAP Business One no pudo completar la operacion solicitada.",
            DetermineRetryability(statusCode),
            BuildDiagnosticContext(operation, statusCode, sapErrorCode),
            innerException)
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

    private static string BuildDiagnosticContext(
        string operation,
        int? statusCode,
        string? sapErrorCode)
    {
        var status = statusCode is null ? string.Empty : $" HTTP {statusCode}.";
        var code = string.IsNullOrWhiteSpace(sapErrorCode) ? string.Empty : $" Codigo SAP: {sapErrorCode}.";
        return $"Operacion SAP: {operation}.{status}{code}";
    }

    private static bool DetermineRetryability(int? statusCode) =>
        statusCode is null or 408 or 429 || statusCode >= 500;
}

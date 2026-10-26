namespace NuanSystem.Application.Common.Exceptions;

public enum DependencyKind
{
    Database,
    Sap,
    Network,
    Timeout
}

public class DependencyFailureException : Exception
{
    public DependencyFailureException(
        DependencyKind dependency,
        string safeErrorCode,
        string safeMessage,
        bool isRetryable,
        string? diagnosticContext = null,
        Exception? innerException = null)
        : base(safeMessage, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(safeErrorCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(safeMessage);
        Dependency = dependency;
        SafeErrorCode = safeErrorCode;
        IsRetryable = isRetryable;
        DiagnosticContext = diagnosticContext;
    }

    public DependencyKind Dependency { get; }
    public string SafeErrorCode { get; }
    public bool IsRetryable { get; }
    public string? DiagnosticContext { get; }
}

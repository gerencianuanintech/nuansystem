namespace NuanSystem.Application.Abstractions.Sri;

public interface ISriAuthorizationProvider
{
    Task<SriAuthorizationResult> QueryAsync(string environment, string accessKey, CancellationToken cancellationToken = default);
}

public sealed record SriAuthorizationResult(SriAuthorizationOutcome Outcome, string? AuthorizationNumber = null,
    DateTimeOffset? AuthorizationAt = null, string? ProviderEnvironment = null, string? IssuerRuc = null,
    string? DocumentTypeCode = null, byte[]? XmlContent = null, byte[]? Sha256 = null,
    string? ErrorCategory = null, string? ErrorCode = null, string? ErrorMessage = null,
    string? RemoteCorrelationId = null);

public enum SriAuthorizationOutcome { Authorized, NotFound, TransientFailure, PermanentFailure }

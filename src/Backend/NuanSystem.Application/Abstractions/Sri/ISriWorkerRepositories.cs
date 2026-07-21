namespace NuanSystem.Application.Abstractions.Sri;

public interface ISriWorkerCompanyRepository
{
    Task<IReadOnlyCollection<SriWorkerCompanyDto>> GetEnabledCompaniesAsync(CancellationToken cancellationToken = default);
}

public interface ISriWorkerQueueRepository
{
    Task<int> ReleaseExpiredLeasesAsync(int companyId, string workerInstance, int maxAttempts, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SriClaimedDocumentDto>> ClaimAsync(int companyId, string environment, string workerInstance, int batchSize, int leaseSeconds, int maxAttempts, CancellationToken cancellationToken = default);
    Task<SriWorkerCompletionCode> CompleteAuthorizedAsync(int companyId, SriAuthorizedDocumentData document, CancellationToken cancellationToken = default);
    Task<SriWorkerCompletionCode> CompleteAttemptAsync(int companyId, SriAttemptCompletionData completion, CancellationToken cancellationToken = default);
}

public sealed record SriWorkerCompanyDto(int CompanyId, string CompanyCode, string Environment);

public sealed record SriClaimedDocumentDto(long Id, string Environment, string AccessKey, string DocumentTypeCode,
    string SourceType, string SourceReference, string? BranchCode, int AttemptCount, int MaxAttempts,
    Guid TraceId, DateTime CreatedAt, string LockedBy, DateTime LockExpiresAt);

public sealed record SriAuthorizedDocumentData(long QueueId, string WorkerInstance, int AttemptNumber,
    string AuthorizationNumber, DateTimeOffset AuthorizationAt, string ProviderEnvironment, string IssuerRuc,
    string DocumentTypeCode, byte[] XmlContent, byte[] Sha256, string ContentType, string? RemoteCorrelationId);

public sealed record SriAttemptCompletionData(long QueueId, string WorkerInstance, int AttemptNumber, string Outcome,
    string? ErrorCategory, string? ErrorCode, string? ErrorMessage, string? RemoteCorrelationId, DateTime? NextAttemptAt);

public enum SriWorkerCompletionCode { Updated = 1, NotFound = 0, LeaseLost = -2, InvalidState = -3, IntegrityConflict = -4 }

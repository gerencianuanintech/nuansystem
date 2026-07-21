namespace NuanSystem.WinForms.Services.SriDocuments.Models;

public sealed record SriDocumentMonitorSummary(long Total,long Pending,long Querying,long Authorized,long Errors);
public sealed record SriDocumentMonitorItem(long QueueId,string Environment,string DocumentTypeCode,string SourceType,string SourceReference,string? BranchCode,string Status,int AttemptCount,DateTime CreatedAt,DateTimeOffset? AuthorizationAt,bool HasXml,long TotalCount);
public sealed record SriDocumentMonitorDetail(long QueueId,string Environment,string DocumentTypeCode,string SourceType,string SourceReference,string? BranchCode,string Status,int AttemptCount,DateTime CreatedAt,DateTimeOffset? AuthorizationAt,bool HasXml,long TotalCount,int? MaxAttempts,DateTime? UpdatedAt,DateTime? CompletedAt,string? LastErrorCode,long? DocumentId,string? AuthorizationNumber,string? ProviderEnvironment,string? IssuerRuc,string? ContentType,int? SizeBytes,string? Sha256Hex,DateTime? StoredAt);
public sealed record SriDocumentAttempt(long Id,long QueueId,int AttemptNumber,string Action,string ResultStatus,string? ErrorCategory,string? ErrorCode,string? ErrorMessage,string? RemoteCorrelationId,DateTime StartedAt,DateTime? CompletedAt,int? DurationMs,DateTime CreatedAt);
public sealed record SriDocumentAudit(long Id,long QueueId,string Action,string? PreviousStatus,string NewStatus,string? Reason,int? UserId,string? UserName,Guid TraceId,DateTime CreatedAt);
public sealed record SriWorkerHealthReport(string OverallHealth,DateTime EvaluatedAtUtc,IReadOnlyCollection<SriWorkerHealthInstance> Instances);
public sealed record SriWorkerHealthInstance(string WorkerType,string HostName,string WorkerInstance,string LifecycleState,string Health,
    IReadOnlyCollection<string> ReasonCodes,DateTime LastBeatAtUtc,DateTime? LastSuccessfulCycleAtUtc,int EnabledCompanyCount,
    long PendingCount,long RetryScheduledCount,long DeadLetterCount,long ActiveLeaseCount,long ExpiredLeaseCount,
    string? LastSafeErrorCode,string? LastSafeErrorMessage);
public sealed class SriDocumentMonitorFilter { public string? Environment { get; set; } public string? Status { get; set; } public string? DocumentTypeCode { get; set; } public string? SourceType { get; set; } public DateTime? CreatedFrom { get; set; } public DateTime? CreatedTo { get; set; } public string? Search { get; set; } public int Page { get; set; }=1; public int PageSize { get; set; }=50; }

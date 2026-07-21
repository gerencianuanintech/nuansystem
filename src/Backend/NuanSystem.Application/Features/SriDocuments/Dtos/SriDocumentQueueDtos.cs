namespace NuanSystem.Application.Features.SriDocuments.Dtos;

public class SriDocumentQueueListItemDto
{
    public long Id { get; set; }
    public string Environment { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string DocumentTypeCode { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string SourceReference { get; set; } = string.Empty;
    public string? BranchCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int AttemptCount { get; set; }
    public int? MaxAttempts { get; set; }
    public DateTime? NextAttemptAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? LastErrorCode { get; set; }
}

public class SriDocumentQueueDetailDto : SriDocumentQueueListItemDto
{
    public Guid TraceId { get; set; }
    public string? LastErrorMessage { get; set; }
    public string? LockedBy { get; set; }
    public DateTime? LockedAt { get; set; }
    public DateTime? LockExpiresAt { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class SriDocumentAttemptDto
{
    public long Id { get; set; }
    public long QueueId { get; set; }
    public int AttemptNumber { get; set; }
    public string Action { get; set; } = string.Empty;
    public string ResultStatus { get; set; } = string.Empty;
    public string? ErrorCategory { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? RemoteCorrelationId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? DurationMs { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed record SriDocumentQueueFilter(string? Environment = null, string? Status = null, string? SourceType = null, string? AccessKey = null, DateTime? CreatedFrom = null, DateTime? CreatedTo = null, int Page = 1, int PageSize = 100);
public sealed record EnqueueSriDocumentData(string Environment, string AccessKey, string DocumentTypeCode, string SourceType, string SourceReference, string? BranchCode, int Priority, Guid TraceId, int? AuditUserId, string? AuditUserName);
public sealed record SriDocumentQueuePersistenceResult(SriDocumentQueueDetailDto Queue, bool IsCreated);
public enum SriDocumentQueueActionCode { Updated = 1, NotFound = 0, ConcurrencyConflict = -2, InvalidState = -3 }
public sealed record SriDocumentQueueActionData(long Id, byte[] RowVersion, string? Reason, int? AuditUserId, string? AuditUserName);

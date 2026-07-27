using System.Text.Json.Serialization;

namespace NuanSystem.Application.Features.SriDocuments.Dtos;

public class SriDocumentQueueListItemDto
{
    public long Id { get; set; }
    public string Environment { get; set; } = string.Empty;
    [JsonIgnore]
    public string AccessKey { get; set; } = string.Empty;
    public string MaskedAccessKey =>
        string.IsNullOrEmpty(AccessKey)
            ? string.Empty
            : $"********{AccessKey[^Math.Min(8, AccessKey.Length)..]}";
    public string DocumentTypeCode { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string SourceReference { get; set; } = string.Empty;
    public string? BranchCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDisplayName => SriDocumentQueueStatusCodes.GetDisplayName(Status);
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

public sealed record SriDocumentMonitorSummaryDto(long Total, long Pending, long Querying, long Authorized, long Errors);
public class SriDocumentMonitorListItemDto
{
    public long QueueId { get; set; }
    public string Environment { get; set; } = string.Empty;
    public string DocumentTypeCode { get; set; } = string.Empty;
    public string SourceType { get; set; } = string.Empty;
    public string SourceReference { get; set; } = string.Empty;
    public string? BranchCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusDisplayName => SriDocumentQueueStatusCodes.GetDisplayName(Status);
    public int AttemptCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTimeOffset? AuthorizationAt { get; set; }
    public bool HasXml { get; set; }
    public long TotalCount { get; set; }
}
public sealed class SriDocumentMonitorDetailDto : SriDocumentMonitorListItemDto
{
    public int? MaxAttempts { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? LastErrorCode { get; set; }
    public long? DocumentId { get; set; }
    public string? AuthorizationNumber { get; set; }
    public string? ProviderEnvironment { get; set; }
    public string? IssuerRuc { get; set; }
    public string? ContentType { get; set; }
    public int? SizeBytes { get; set; }
    public string? Sha256Hex { get; set; }
    public DateTime? StoredAt { get; set; }
}
public sealed class SriDocumentAuditDto
{
    public long Id { get; set; }
    public long QueueId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? PreviousStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public Guid TraceId { get; set; }
    public DateTime CreatedAt { get; set; }
}
public sealed record SriDocumentMonitorFilter(string? Environment=null,string? Status=null,string? DocumentTypeCode=null,string? SourceType=null,DateTime? CreatedFrom=null,DateTime? CreatedTo=null,string? Search=null,int Page=1,int PageSize=50);
public sealed record SriAuthorizedXmlDownloadData(long QueueId,int? AuditUserId,string? AuditUserName,Guid TraceId);
public enum SriAuthorizedXmlDownloadCode { Success=1, NotFound=0, NotAuthorized=-3, MissingContent=-4 }
public sealed record SriAuthorizedXmlPersistenceResult(SriAuthorizedXmlDownloadCode Code,long? DocumentId,long QueueId,byte[] XmlContent,string? ContentType,int SizeBytes);
public sealed record SriAuthorizedXmlDownloadDto(long DocumentId,long QueueId,byte[] Content,string ContentType,string FileName,int SizeBytes);

using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record CreateSyncAuditData(
    int CompanyId,
    int? BranchCompanyId,
    Guid? EventId,
    string EntityName,
    Guid? EntityGlobalId,
    SyncAuditAction Action,
    SyncEventStatus? PreviousStatus,
    SyncEventStatus? NewStatus,
    string? Message,
    string? ErrorCode,
    string? ErrorDetail,
    string? CreatedBy);

public sealed record SyncAuditDto(
    long Id,
    int CompanyId,
    int? BranchCompanyId,
    Guid? EventId,
    string EntityName,
    Guid? EntityGlobalId,
    SyncAuditAction Action,
    SyncEventStatus? PreviousStatus,
    SyncEventStatus? NewStatus,
    string? Message,
    string? ErrorCode,
    string? ErrorDetail,
    DateTime CreatedAt,
    string? CreatedBy);

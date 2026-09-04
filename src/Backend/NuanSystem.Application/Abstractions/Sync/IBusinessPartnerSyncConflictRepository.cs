using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IBusinessPartnerSyncConflictRepository
{
    Task<IReadOnlyCollection<BusinessPartnerSyncConflictRecord>> ListAsync(
        int companyId,
        string? status,
        CancellationToken cancellationToken = default);

    Task<BusinessPartnerSyncConflictRecord?> GetByIdAsync(
        int companyId,
        long conflictId,
        CancellationToken cancellationToken = default);

    Task<BusinessPartnerSyncConflictResolutionResult> ResolveAsync(
        BusinessPartnerSyncConflictResolutionData resolution,
        CancellationToken cancellationToken = default);
}

public sealed record BusinessPartnerSyncConflictRecord(
    long Id,
    Guid ProposalEventId,
    int? BusinessPartnerId,
    Guid BusinessPartnerGlobalId,
    int OriginCompanyId,
    long BaseCanonicalVersion,
    long CurrentCanonicalVersion,
    BusinessPartnerCanonicalSnapshot? Base,
    BusinessPartnerCanonicalSnapshot Proposed,
    BusinessPartnerCanonicalSnapshot Canonical,
    IReadOnlyCollection<string> ConflictFields,
    string Status,
    string? Resolution,
    string? ResolutionReason,
    int? CreatedByUserId,
    string? CreatedByUserName,
    DateTime CreatedAt,
    int? ResolvedByUserId,
    string? ResolvedByUserName,
    DateTime? ResolvedAt,
    byte[] RowVersion,
    string? Code,
    string? Name);

public sealed record BusinessPartnerSyncConflictOutboundEvent(
    Guid EventId,
    int? TargetCompanyId,
    Guid CausationEventId,
    SyncPublishRequest PublishRequest);

public sealed record BusinessPartnerSyncConflictLiveCanonicalState(
    int BusinessPartnerId,
    long CanonicalVersion,
    byte[] RowVersion,
    BusinessPartnerCanonicalSnapshot Snapshot);

public sealed record BusinessPartnerSyncConflictResolutionPlan(
    int ExpectedBusinessPartnerId,
    long ExpectedCanonicalVersion,
    byte[] ExpectedBusinessPartnerRowVersion,
    BusinessPartnerCanonicalSnapshot? ResolvedSnapshot,
    BusinessPartnerSyncConflictOutboundEvent OutboundEvent);

public interface IBusinessPartnerSyncConflictResolutionPlanner
{
    BusinessPartnerSyncConflictResolutionPlan? CreatePlan(
        int companyId,
        BusinessPartnerSyncConflictRecord conflict,
        BusinessPartnerSyncConflictLiveCanonicalState live,
        string resolution,
        string reason);
}

public sealed record BusinessPartnerSyncConflictResolutionData(
    int CompanyId,
    long ConflictId,
    string Resolution,
    string Reason,
    byte[] ExpectedRowVersion,
    int? AuditUserId,
    string? AuditUserName);

public enum BusinessPartnerSyncConflictResolutionOutcome
{
    Resolved,
    AlreadyResolved,
    ConcurrencyConflict,
    OutboundEventCollision,
    ReferenceNotFound,
    InvalidConflictPath,
    NotFound
}

public sealed record BusinessPartnerSyncConflictResolutionResult(
    BusinessPartnerSyncConflictResolutionOutcome Outcome,
    BusinessPartnerSyncConflictRecord? Conflict);

using NuanSystem.Application.Features.BusinessPartners.Sync;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IBusinessPartnerSyncApplyRepository
{
    Task<BusinessPartnerSyncApplyResult> ApplyCanonicalAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerCanonicalPayloadV2 payload,
        CancellationToken cancellationToken = default);

    Task<BusinessPartnerSyncApplyResult> ApplyProposalResultAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerProposalResultPayloadV1 payload,
        CancellationToken cancellationToken = default);
}

public sealed record BusinessPartnerSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    int? BusinessPartnerId,
    string Message,
    string? ErrorCode = null,
    bool Retryable = false,
    bool Terminal = false,
    bool Ignored = false);

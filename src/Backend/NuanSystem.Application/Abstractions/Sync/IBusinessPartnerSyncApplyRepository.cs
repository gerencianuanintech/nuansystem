using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IBusinessPartnerSyncApplyRepository
{
    Task<bool> ExistsByGlobalIdAsync(
        int branchCompanyId,
        Guid globalId,
        CancellationToken cancellationToken = default);

    Task<BusinessPartnerSyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);

    Task<BusinessPartnerSyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerSyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default);
}

public sealed record BusinessPartnerSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    int? BusinessPartnerId,
    string Message);

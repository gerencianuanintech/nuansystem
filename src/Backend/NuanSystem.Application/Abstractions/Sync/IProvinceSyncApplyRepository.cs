using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IProvinceSyncApplyRepository
{
    Task<bool> ExistsByGlobalIdAsync(
        int branchCompanyId,
        Guid globalId,
        CancellationToken cancellationToken = default);

    Task<ProvinceSyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ProvinceSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);

    Task<ProvinceSyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ProvinceSyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default);
}

public sealed record ProvinceSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    int? ProvinceId,
    string Message);

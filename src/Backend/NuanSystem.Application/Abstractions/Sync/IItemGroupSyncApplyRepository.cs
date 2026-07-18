using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemGroupSyncApplyRepository
{
    Task<bool> ExistsByGlobalIdAsync(
        int branchCompanyId,
        Guid globalId,
        CancellationToken cancellationToken = default);

    Task<ItemGroupSyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemGroupSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);

    Task<ItemGroupSyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemGroupSyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default);
}

public sealed record ItemGroupSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    int? ItemGroupId,
    string Message);

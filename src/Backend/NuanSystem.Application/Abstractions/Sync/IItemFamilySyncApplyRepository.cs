using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemFamilySyncApplyRepository
{
    Task<bool> ItemGroupExistsAsync(
        int branchCompanyId,
        Guid itemGroupGlobalId,
        CancellationToken cancellationToken = default);

    Task<ItemFamilySyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemFamilySyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);

    Task<ItemFamilySyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemFamilySyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default);
}

public sealed record ItemFamilySyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    bool TerminalConflict,
    int? ItemFamilyId,
    string Message,
    string? ErrorCode = null);

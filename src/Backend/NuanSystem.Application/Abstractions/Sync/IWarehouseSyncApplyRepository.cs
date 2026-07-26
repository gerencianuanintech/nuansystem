using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IWarehouseSyncApplyRepository
{
    Task<bool> ExistsByGlobalIdAsync(
        int branchCompanyId,
        Guid globalId,
        CancellationToken cancellationToken = default);

    Task<WarehouseSyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        WarehouseSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);

    Task<WarehouseSyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        WarehouseSyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default);
}

public sealed record WarehouseSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    bool TerminalConflict,
    int? WarehouseId,
    string Message,
    string? ErrorCode = null);

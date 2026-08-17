using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IStorageConditionSyncApplyRepository
{
    Task<StorageConditionSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        StorageConditionSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);
}

public sealed record StorageConditionSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    bool TerminalConflict,
    int? StorageConditionId,
    string Message,
    string? ErrorCode = null);

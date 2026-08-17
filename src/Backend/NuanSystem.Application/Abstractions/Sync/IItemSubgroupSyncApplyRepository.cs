using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemSubgroupSyncApplyRepository
{
    Task<bool> ItemFamilyExistsAsync(int branchCompanyId, Guid itemFamilyGlobalId,
        CancellationToken cancellationToken = default);

    Task<ItemSubgroupSyncApplyResult> ApplyAsync(int branchCompanyId, SyncEventApplyContext context,
        ItemSubgroupSyncPayload payload, SyncOperation operation,
        CancellationToken cancellationToken = default);
}

public sealed record ItemSubgroupSyncApplyResult(
    bool Applied, bool AlreadyApplied, bool TerminalConflict, int? ItemSubgroupId,
    string Message, string? ErrorCode = null);

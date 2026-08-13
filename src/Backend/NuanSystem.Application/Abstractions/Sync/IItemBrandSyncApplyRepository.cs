using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemBrandSyncApplyRepository
{
    Task<ItemBrandSyncApplyResult> ApplyAsync(int branchCompanyId, SyncEventApplyContext context,
        ItemBrandSyncPayload payload, SyncOperation operation,
        CancellationToken cancellationToken = default);
}

public sealed record ItemBrandSyncApplyResult(
    bool Applied, bool AlreadyApplied, bool TerminalConflict, int? ItemBrandId,
    string Message, string? ErrorCode = null);

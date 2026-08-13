using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemLineSyncApplyRepository
{
    Task<ItemLineSyncApplyResult> ApplyAsync(int branchCompanyId, SyncEventApplyContext context,
        ItemLineSyncPayload payload, SyncOperation operation, CancellationToken cancellationToken = default);
}

public sealed record ItemLineSyncApplyResult(
    bool Applied, bool AlreadyApplied, bool TerminalConflict, int? ItemLineId,
    string Message, string? ErrorCode = null);

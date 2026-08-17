using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemOriginSyncApplyRepository
{
    Task<ItemOriginSyncApplyResult> ApplyAsync(int branchCompanyId, SyncEventApplyContext context,
        ItemOriginSyncPayload payload, SyncOperation operation,
        CancellationToken cancellationToken = default);
}

public sealed record ItemOriginSyncApplyResult(
    bool Applied, bool AlreadyApplied, bool TerminalConflict, int? ItemOriginId,
    string Message, string? ErrorCode = null);

using NuanSystem.Application.Features.Definitions.Inventory.ReplenishmentMethods.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IReplenishmentMethodSyncApplyRepository
{
    Task<ReplenishmentMethodSyncApplyResult> ApplyAsync(int branchCompanyId,
        SyncEventApplyContext context, ReplenishmentMethodSyncPayload payload,
        SyncOperation operation, CancellationToken cancellationToken = default);
}

public sealed record ReplenishmentMethodSyncApplyResult(
    bool Applied, bool AlreadyApplied, bool TerminalConflict, int? ReplenishmentMethodId,
    string Message, string? ErrorCode = null);

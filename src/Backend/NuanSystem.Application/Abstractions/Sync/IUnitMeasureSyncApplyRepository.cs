using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IUnitMeasureSyncApplyRepository
{
    Task<UnitMeasureSyncApplyResult> ApplyAsync(int branchCompanyId, SyncEventApplyContext context,
        UnitMeasureSyncPayload payload, SyncOperation operation, CancellationToken cancellationToken = default);
}

public sealed record UnitMeasureSyncApplyResult(
    bool Applied, bool AlreadyApplied, bool TerminalConflict, int? UnitMeasureId,
    string Message, string? ErrorCode = null);

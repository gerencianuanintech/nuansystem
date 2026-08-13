using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IProductTypeSyncApplyRepository
{
    Task<ProductTypeSyncApplyResult> ApplyAsync(int branchCompanyId, SyncEventApplyContext context,
        ProductTypeSyncPayload payload, SyncOperation operation, CancellationToken cancellationToken = default);
}

public sealed record ProductTypeSyncApplyResult(
    bool Applied, bool AlreadyApplied, bool TerminalConflict, int? ProductTypeId,
    string Message, string? ErrorCode = null);

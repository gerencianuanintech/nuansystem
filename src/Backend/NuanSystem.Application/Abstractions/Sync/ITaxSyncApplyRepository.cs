using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ITaxSyncApplyRepository
{
    Task<TaxSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        TaxSyncPayloadV1 payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);
}

public sealed record TaxSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    bool TerminalConflict,
    int? TaxId,
    string Message,
    string? ErrorCode = null);

using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IPriceListSyncApplyRepository
{
    Task<PriceListSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        PriceListSyncPayloadV2 payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);
}

public sealed record PriceListSyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    bool TerminalConflict,
    int? PriceListId,
    string Message,
    string? ErrorCode = null);

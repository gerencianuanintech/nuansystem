using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ICurrencySyncApplyRepository
{
    Task<bool> ExistsByGlobalIdAsync(
        int branchCompanyId,
        Guid globalId,
        CancellationToken cancellationToken = default);

    Task<CurrencySyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CurrencySyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default);

    Task<CurrencySyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CurrencySyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default);
}

public sealed record CurrencySyncApplyResult(
    bool Applied,
    bool AlreadyApplied,
    int? CurrencyId,
    string Message);

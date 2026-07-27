using System.Data;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IPriceListLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        PriceListDto priceList,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

using System.Data;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ITaxLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        TaxDto tax,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

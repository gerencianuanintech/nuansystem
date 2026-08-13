using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IProductTypeLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(ProductTypeDto productType, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}

using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemBrandLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(ItemBrandDto itemBrand, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

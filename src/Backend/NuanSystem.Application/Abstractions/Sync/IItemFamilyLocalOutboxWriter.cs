using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemFamilies.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemFamilyLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        ItemFamilyDto itemFamily,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

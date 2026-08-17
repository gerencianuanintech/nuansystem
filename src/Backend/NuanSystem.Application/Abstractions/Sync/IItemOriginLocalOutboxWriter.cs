using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemOriginLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(ItemOriginDto itemOrigin, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

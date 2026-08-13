using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemLineLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(ItemLineDto itemLine, SyncOperation operation,
        IDbConnection connection, IDbTransaction transaction, CancellationToken cancellationToken = default);
}

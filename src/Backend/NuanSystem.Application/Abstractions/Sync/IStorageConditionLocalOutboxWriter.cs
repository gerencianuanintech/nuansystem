using System.Data;
using NuanSystem.Application.Features.Definitions.Inventory.StorageConditions.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IStorageConditionLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        StorageConditionDto condition,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

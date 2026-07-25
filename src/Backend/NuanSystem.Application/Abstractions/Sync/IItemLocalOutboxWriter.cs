using System.Data;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IItemLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        ItemDto item,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

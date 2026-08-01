using System.Data;
using NuanSystem.Application.Features.Carriers.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ICarrierLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        CarrierDetailDto carrier,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

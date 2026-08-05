using System.Data;
using NuanSystem.Application.Features.Definitions.General.Countries.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ICountryLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        CountryDto country,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

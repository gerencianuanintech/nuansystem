using System.Data;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IBusinessPartnerLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        BusinessPartnerDto partner,
        SyncOperation operation,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

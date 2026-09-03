using System.Data;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface IBusinessPartnerLocalOutboxWriter
{
    Task<Guid?> EnqueueAsync(
        BusinessPartnerOutboxWriteRequest request,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);
}

public sealed record BusinessPartnerOutboxWriteRequest(
    BusinessPartnerDto Current,
    BusinessPartnerDto? Base,
    SyncOperation Operation,
    int? OriginUserId,
    string? OriginUserName,
    Guid? CausationEventId);

using System.Data;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ILocalSyncOutboxRepository
{
    Task<long> CreateAsync(
        CreateLocalSyncOutboxData data,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LocalSyncOutboxCompanyDto>> GetRelayCompaniesAsync(
        CancellationToken cancellationToken = default);

    Task<int> ReleaseExpiredLeasesAsync(
        int companyId,
        string workerInstance,
        IReadOnlyCollection<string> enabledEntityNames,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LocalSyncOutboxDto>> ClaimAsync(
        int companyId,
        string workerInstance,
        int batchSize,
        TimeSpan leaseDuration,
        IReadOnlyCollection<string> enabledEntityNames,
        CancellationToken cancellationToken = default);

    Task MarkPromotedAsync(
        int companyId,
        long id,
        string workerInstance,
        CancellationToken cancellationToken = default);

    Task MarkRetryAsync(
        int companyId,
        long id,
        string workerInstance,
        string errorMessage,
        TimeSpan retryDelay,
        CancellationToken cancellationToken = default);

    Task MarkConflictAsync(
        int companyId,
        long id,
        string workerInstance,
        string errorMessage,
        CancellationToken cancellationToken = default);
}

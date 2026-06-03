using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncInboxRepository
{
    Task<long> UpsertSupplierAsync(int companyId, string sapCardCode, string payloadJson, SapSyncStatus status, string workerInstance, string correlationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SapSyncInboxItemDto>> ClaimPendingAsync(int companyId, string entityCode, int batchSize, string workerInstance, TimeSpan lockTimeout, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SapSyncInboxItemDto>> ClaimRetryScheduledAsync(int companyId, string entityCode, int batchSize, string workerInstance, TimeSpan lockTimeout, CancellationToken cancellationToken = default);
    Task MarkProcessingAsync(long id, string workerInstance, string correlationId, CancellationToken cancellationToken = default);
    Task MarkImportedAsync(long id, string? localEntityId, CancellationToken cancellationToken = default);
    Task MarkConflictAsync(long id, string? message, CancellationToken cancellationToken = default);
    Task MarkFailedAsync(long id, string? errorCode, string? errorMessage, DateTime? nextAttemptAtUtc, CancellationToken cancellationToken = default);
    Task MarkDeadLetterAsync(long id, string? errorCode, string? errorMessage, CancellationToken cancellationToken = default);
    Task ReleaseExpiredLocksAsync(int companyId, DateTime olderThanUtc, CancellationToken cancellationToken = default);
}

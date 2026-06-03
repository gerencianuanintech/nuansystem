using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.SapSync;

public interface ISapSyncOutboxRepository
{
    Task<IReadOnlyCollection<SapSyncOutboxItemDto>> ClaimPendingAsync(int companyId, int batchSize, string workerInstance, TimeSpan lockTimeout, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SapSyncOutboxItemDto>> ClaimRetryScheduledAsync(int companyId, int batchSize, string workerInstance, TimeSpan lockTimeout, CancellationToken cancellationToken = default);
    Task MarkProcessingAsync(long id, string workerInstance, string correlationId, CancellationToken cancellationToken = default);
    Task MarkSucceededAsync(long id, int? sapDocEntry, int? sapDocNum, string? responseJson, CancellationToken cancellationToken = default);
    Task MarkFailedAsync(long id, string? errorCode, string? errorMessage, DateTime? nextAttemptAtUtc, CancellationToken cancellationToken = default);
    Task MarkDeadLetterAsync(long id, string? errorCode, string? errorMessage, CancellationToken cancellationToken = default);
    Task ReleaseExpiredLocksAsync(int companyId, DateTime olderThanUtc, CancellationToken cancellationToken = default);
}

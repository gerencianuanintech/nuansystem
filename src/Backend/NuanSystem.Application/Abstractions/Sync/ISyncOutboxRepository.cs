using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Abstractions.Sync;

public interface ISyncOutboxRepository
{
    Task<long> CreateAsync(CreateSyncOutboxEventData data, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SyncOutboxDto>> GetPendingAsync(int companyId, int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SyncOutboxDto>> ClaimPendingAsync(string lockedBy, int take, TimeSpan lockDuration, CancellationToken cancellationToken = default);
    Task<int> ReleaseExpiredLocksAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SyncOutboxDto>> GetRecentAsync(int companyId, int take, CancellationToken cancellationToken = default);
    Task<SyncDashboardDto> GetDashboardAsync(int companyId, int take, CancellationToken cancellationToken = default);
    Task<SyncSummaryDto> GetSummaryAsync(int companyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SyncOutboxListItemDto>> SearchOutboxAsync(int companyId, SyncOutboxQueryFilter filter, CancellationToken cancellationToken = default);
    Task<SyncOutboxDetailDto?> GetOutboxDetailAsync(int companyId, long id, CancellationToken cancellationToken = default);
    Task<SyncOutboxDto?> GetByIdAsync(int companyId, long id, CancellationToken cancellationToken = default);
    Task<SyncOutboxActionResultDto?> RetryErrorAsync(int companyId, long id, string? reason, string? createdBy, CancellationToken cancellationToken = default);
    Task<SyncOutboxActionResultDto?> RetryDeadLetterAsync(int companyId, long id, string reason, bool resetAttemptCount, string? createdBy, CancellationToken cancellationToken = default);
    Task<SyncOutboxActionResultDto?> ReleaseExpiredLockAsync(int companyId, long id, string? reason, string? createdBy, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SyncOutboxTargetDto>> GetTargetsAsync(int companyId, long outboxId, CancellationToken cancellationToken = default);
    Task<long> CreateTargetAsync(CreateSyncOutboxTargetData data, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(long id, SyncEventStatus status, string? lastErrorMessage = null, CancellationToken cancellationToken = default);
    Task MarkAppliedAsync(long id, CancellationToken cancellationToken = default);
    Task MarkIgnoredAsync(long id, string? reason, CancellationToken cancellationToken = default);
    Task MarkErrorAsync(long id, string errorMessage, TimeSpan retryDelay, CancellationToken cancellationToken = default);
    Task MarkDeadLetterAsync(long id, string errorMessage, CancellationToken cancellationToken = default);
    Task<bool> TryMarkTargetInProcessAsync(long targetId, CancellationToken cancellationToken = default);
    Task MarkTargetAppliedAsync(long targetId, CancellationToken cancellationToken = default);
    Task MarkTargetIgnoredAsync(long targetId, string? reason, CancellationToken cancellationToken = default);
    Task MarkTargetErrorAsync(long targetId, string errorMessage, TimeSpan retryDelay, CancellationToken cancellationToken = default);
    Task MarkTargetDeadLetterAsync(long targetId, string errorMessage, CancellationToken cancellationToken = default);
}

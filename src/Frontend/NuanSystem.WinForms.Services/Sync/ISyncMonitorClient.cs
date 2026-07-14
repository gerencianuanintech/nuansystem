using NuanSystem.WinForms.Services.Sync.Models;

namespace NuanSystem.WinForms.Services.Sync;

public interface ISyncMonitorClient
{
    Task<SyncDashboard> GetDashboardAsync(int take = 10, CancellationToken cancellationToken = default);

    Task<SyncSummary> GetSummaryAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SyncOutboxListItem>> SearchOutboxAsync(SyncOutboxFilter filter, CancellationToken cancellationToken = default);

    Task<SyncOutboxDetail> GetOutboxDetailAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SyncOutboxTarget>> GetOutboxTargetsAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SyncAuditItem>> SearchAuditAsync(SyncAuditFilter filter, CancellationToken cancellationToken = default);

    Task<SyncManualActionResult> RetryAsync(long id, CancellationToken cancellationToken = default);

    Task<SyncManualActionResult> RetryDeadLetterAsync(long id, RetryDeadLetterRequest request, CancellationToken cancellationToken = default);

    Task<SyncManualActionResult> ReleaseExpiredLockAsync(long id, ReleaseExpiredLockRequest request, CancellationToken cancellationToken = default);
}

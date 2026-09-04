using System.Globalization;
using System.Text;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Sync.Models;

namespace NuanSystem.WinForms.Services.Sync;

public sealed class SyncMonitorClient(INuanApiClient apiClient) : ISyncMonitorClient
{
    public Task<SyncDashboard> GetDashboardAsync(int take = 10, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SyncDashboard>($"/api/sync/dashboard?take={Math.Max(1, take)}", cancellationToken);
    }

    public Task<SyncSummary> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SyncSummary>("/api/sync/summary", cancellationToken);
    }

    public Task<IReadOnlyCollection<SyncOutboxListItem>> SearchOutboxAsync(SyncOutboxFilter filter, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<SyncOutboxListItem>>($"/api/sync/outbox{BuildQuery(filter)}", cancellationToken);
    }

    public Task<SyncOutboxDetail> GetOutboxDetailAsync(long id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<SyncOutboxDetail>($"/api/sync/outbox/{id}", cancellationToken);
    }

    public Task<IReadOnlyCollection<SyncOutboxTarget>> GetOutboxTargetsAsync(long id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<SyncOutboxTarget>>($"/api/sync/outbox/{id}/targets", cancellationToken);
    }

    public Task<IReadOnlyCollection<SyncAuditItem>> SearchAuditAsync(SyncAuditFilter filter, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<SyncAuditItem>>($"/api/sync/audit{BuildQuery(filter)}", cancellationToken);
    }

    public Task<SyncManualActionResult> RetryAsync(long id, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<RetrySyncOutboxRequest, SyncManualActionResult>(
            $"/api/sync/outbox/{id}/retry",
            new RetrySyncOutboxRequest(),
            cancellationToken);
    }

    public Task<SyncManualActionResult> RetryDeadLetterAsync(long id, RetryDeadLetterRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<RetryDeadLetterRequest, SyncManualActionResult>(
            $"/api/sync/outbox/{id}/retry-deadletter",
            request,
            cancellationToken);
    }

    public Task<SyncManualActionResult> ReleaseExpiredLockAsync(long id, ReleaseExpiredLockRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<ReleaseExpiredLockRequest, SyncManualActionResult>(
            $"/api/sync/outbox/{id}/release-expired-lock",
            request,
            cancellationToken);
    }

    public Task<IReadOnlyCollection<BusinessPartnerSyncConflict>> GetBusinessPartnerConflictsAsync(
        string status = "Open",
        CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<IReadOnlyCollection<BusinessPartnerSyncConflict>>(
            $"/api/sync/business-partner-conflicts?status={Uri.EscapeDataString(status)}",
            cancellationToken);
    }

    public Task<BusinessPartnerSyncConflict> ResolveBusinessPartnerConflictAsync(
        long id,
        ResolveBusinessPartnerSyncConflictRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<ResolveBusinessPartnerSyncConflictRequest, BusinessPartnerSyncConflict>(
            $"/api/sync/business-partner-conflicts/{id}/resolve",
            request,
            cancellationToken);
    }

    public Task<RetrySyncOutboxBatchResult> RetryBatchAsync(RetrySyncOutboxBatchRequest request,CancellationToken cancellationToken=default)
        => apiClient.PostAsync<RetrySyncOutboxBatchRequest,RetrySyncOutboxBatchResult>("/api/sync/outbox/retry-batch",request,cancellationToken);

    private static string BuildQuery(SyncOutboxFilter filter)
    {
        var builder = new QueryBuilder()
            .Add("status", filter.Status?.ToString())
            .Add("entityName", filter.EntityName)
            .Add("entityGlobalId", filter.EntityGlobalId?.ToString())
            .Add("eventId", filter.EventId?.ToString())
            .Add("branchCompanyId", filter.BranchCompanyId)
            .Add("createdFrom", filter.CreatedFrom)
            .Add("createdTo", filter.CreatedTo)
            .Add("hasErrors", filter.HasErrors)
            .Add("deadLetterOnly", filter.DeadLetterOnly)
            .Add("page", filter.Page)
            .Add("pageSize", filter.PageSize);

        return builder.ToString();
    }

    private static string BuildQuery(SyncAuditFilter filter)
    {
        var builder = new QueryBuilder()
            .Add("status", filter.Status?.ToString())
            .Add("entityName", filter.EntityName)
            .Add("entityGlobalId", filter.EntityGlobalId?.ToString())
            .Add("eventId", filter.EventId?.ToString())
            .Add("branchCompanyId", filter.BranchCompanyId)
            .Add("createdFrom", filter.CreatedFrom)
            .Add("createdTo", filter.CreatedTo)
            .Add("hasErrors", filter.HasErrors)
            .Add("deadLetterOnly", filter.DeadLetterOnly)
            .Add("page", filter.Page)
            .Add("pageSize", filter.PageSize);

        return builder.ToString();
    }

    private sealed class QueryBuilder
    {
        private readonly StringBuilder builder = new();
        private bool hasValues;

        public QueryBuilder Add(string name, object? value)
        {
            if (value is null)
            {
                return this;
            }

            var text = value switch
            {
                string stringValue => stringValue,
                DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
                bool boolean => boolean ? "true" : "false",
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => Convert.ToString(value, CultureInfo.InvariantCulture)
            };

            if (string.IsNullOrWhiteSpace(text))
            {
                return this;
            }

            builder.Append(hasValues ? '&' : '?');
            builder.Append(Uri.EscapeDataString(name));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(text));
            hasValues = true;
            return this;
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}

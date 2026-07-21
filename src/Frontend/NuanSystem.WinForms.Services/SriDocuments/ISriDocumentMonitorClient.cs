using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SriDocuments.Models;
namespace NuanSystem.WinForms.Services.SriDocuments;
public interface ISriDocumentMonitorClient
{
    Task<SriDocumentMonitorSummary> GetSummaryAsync(CancellationToken cancellationToken=default);
    Task<SriWorkerHealthReport> GetWorkerHealthAsync(CancellationToken cancellationToken=default) =>
        Task.FromResult(new SriWorkerHealthReport("Unknown",DateTime.UtcNow,[]));
    Task<IReadOnlyCollection<SriDocumentMonitorItem>> SearchAsync(SriDocumentMonitorFilter filter,CancellationToken cancellationToken=default);
    Task<SriDocumentMonitorDetail> GetDetailAsync(long queueId,CancellationToken cancellationToken=default);
    Task<IReadOnlyCollection<SriDocumentAttempt>> GetAttemptsAsync(long queueId,CancellationToken cancellationToken=default);
    Task<IReadOnlyCollection<SriDocumentAudit>> GetAuditAsync(long queueId,CancellationToken cancellationToken=default);
    Task<ApiFileResponse> DownloadXmlAsync(long queueId,CancellationToken cancellationToken=default);
}

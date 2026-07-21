using System.Globalization;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SriDocuments.Models;
namespace NuanSystem.WinForms.Services.SriDocuments;
public sealed class SriDocumentMonitorClient(INuanApiClient apiClient) : ISriDocumentMonitorClient
{
    public Task<SriDocumentMonitorSummary> GetSummaryAsync(CancellationToken cancellationToken=default)=>apiClient.GetAsync<SriDocumentMonitorSummary>("/api/sri/documents/monitor/summary",cancellationToken);
    public Task<SriWorkerHealthReport> GetWorkerHealthAsync(CancellationToken cancellationToken=default)=>apiClient.GetAsync<SriWorkerHealthReport>("/api/sri/documents/monitor/worker-health",cancellationToken);
    public Task<IReadOnlyCollection<SriDocumentMonitorItem>> SearchAsync(SriDocumentMonitorFilter filter,CancellationToken cancellationToken=default)=>apiClient.GetAsync<IReadOnlyCollection<SriDocumentMonitorItem>>("/api/sri/documents/monitor"+BuildQuery(filter),cancellationToken);
    public Task<SriDocumentMonitorDetail> GetDetailAsync(long queueId,CancellationToken cancellationToken=default)=>apiClient.GetAsync<SriDocumentMonitorDetail>($"/api/sri/documents/monitor/{queueId}",cancellationToken);
    public Task<IReadOnlyCollection<SriDocumentAttempt>> GetAttemptsAsync(long queueId,CancellationToken cancellationToken=default)=>apiClient.GetAsync<IReadOnlyCollection<SriDocumentAttempt>>($"/api/sri/documents/{queueId}/attempts",cancellationToken);
    public Task<IReadOnlyCollection<SriDocumentAudit>> GetAuditAsync(long queueId,CancellationToken cancellationToken=default)=>apiClient.GetAsync<IReadOnlyCollection<SriDocumentAudit>>($"/api/sri/documents/monitor/{queueId}/audit",cancellationToken);
    public Task<ApiFileResponse> DownloadXmlAsync(long queueId,CancellationToken cancellationToken=default)=>apiClient.GetFileAsync($"/api/sri/documents/monitor/{queueId}/xml",cancellationToken);
    private static string BuildQuery(SriDocumentMonitorFilter f)
    {
        var values=new Dictionary<string,string?> { ["environment"]=f.Environment,["status"]=f.Status,["documentTypeCode"]=f.DocumentTypeCode,["sourceType"]=f.SourceType,["createdFrom"]=f.CreatedFrom?.ToString("O",CultureInfo.InvariantCulture),["createdTo"]=f.CreatedTo?.ToString("O",CultureInfo.InvariantCulture),["search"]=f.Search,["page"]=f.Page.ToString(CultureInfo.InvariantCulture),["pageSize"]=f.PageSize.ToString(CultureInfo.InvariantCulture) };
        var query=string.Join("&",values.Where(x=>!string.IsNullOrWhiteSpace(x.Value)).Select(x=>$"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value!)}"));
        return query.Length==0 ? string.Empty : "?"+query;
    }
}

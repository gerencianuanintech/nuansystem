using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SriDocuments;
using NuanSystem.WinForms.Services.SriDocuments.Models;
namespace NuanSystem.WinForms.ViewModels.SriDocuments;
public sealed class SriDocumentMonitorViewModel(ISriDocumentMonitorClient client,bool canViewDetail,bool canDownload,bool canViewWorkerHealth=false)
{
    public SriDocumentMonitorFilter Filter { get; }=new();
    public SriDocumentMonitorSummary? Summary { get; private set; }
    public SriWorkerHealthReport? WorkerHealth { get; private set; }
    public string WorkerHealthText => SriWorkerHealthTextFormatter.Format(WorkerHealth);
    public IReadOnlyCollection<SriDocumentMonitorItem> Items { get; private set; }=[];
    public SriDocumentMonitorDetail? Detail { get; private set; }
    public SriDocumentMonitorItem? Selected { get; private set; }
    public IReadOnlyCollection<SriDocumentAttempt> Attempts { get; private set; }=[];
    public IReadOnlyCollection<SriDocumentAudit> Audit { get; private set; }=[];
    public bool CanDownload => canDownload && Selected is { Status:"Authorized", HasXml:true };
    public async Task LoadAsync(CancellationToken cancellationToken=default) { Summary=await client.GetSummaryAsync(cancellationToken); Items=await client.SearchAsync(Filter,cancellationToken); WorkerHealth=canViewWorkerHealth ? await client.GetWorkerHealthAsync(cancellationToken) : null; }
    public async Task LoadDetailAsync(long queueId,CancellationToken cancellationToken=default)
    {
        Selected=Items.FirstOrDefault(item=>item.QueueId==queueId);
        Detail=canViewDetail ? await client.GetDetailAsync(queueId,cancellationToken) : null;
        Attempts=await client.GetAttemptsAsync(queueId,cancellationToken);
        Audit=canViewDetail ? await client.GetAuditAsync(queueId,cancellationToken) : [];
    }
    public async Task LoadDirectAsync(long queueId,CancellationToken cancellationToken=default)
    {
        Detail=canViewDetail ? await client.GetDetailAsync(queueId,cancellationToken) : null;
        if(Detail is not null)
        {
            Selected=new SriDocumentMonitorItem(
                Detail.QueueId,
                Detail.Environment,
                Detail.DocumentTypeCode,
                Detail.SourceType,
                Detail.SourceReference,
                Detail.BranchCode,
                Detail.Status,
                Detail.AttemptCount,
                Detail.CreatedAt,
                Detail.AuthorizationAt,
                Detail.HasXml,
                Detail.TotalCount);
        }
        Attempts=await client.GetAttemptsAsync(queueId,cancellationToken);
        Audit=canViewDetail ? await client.GetAuditAsync(queueId,cancellationToken) : [];
    }
    public Task<ApiFileResponse> DownloadAsync(CancellationToken cancellationToken=default)
    {
        if(!CanDownload || Selected is null) throw new InvalidOperationException("El documento seleccionado no esta disponible para descarga.");
        return client.DownloadXmlAsync(Selected.QueueId,cancellationToken);
    }
}

internal static class SriWorkerHealthTextFormatter
{
    public static string Format(SriWorkerHealthReport? report)
    {
        if(report is null) return "Salud del worker restringida por permisos.";
        if(report.Instances.Count==0) return $"Estado general: {report.OverallHealth}\r\nNo existe heartbeat SRI registrado.";

        return $"Estado general: {report.OverallHealth} | Evaluado UTC: {report.EvaluatedAtUtc:u}\r\n\r\n"+
            string.Join("\r\n\r\n",report.Instances.Select(x=>
                $"{x.HostName} / {x.WorkerInstance}\r\nVersión: {FormatVersion(x.WorkerVersion)}\r\n{x.LifecycleState} - {x.Health} | Ultimo heartbeat: {x.LastBeatAtUtc:u}\r\nEmpresas: {x.EnabledCompanyCount} | Pending: {x.PendingCount} | Retry: {x.RetryScheduledCount} | DeadLetter: {x.DeadLetterCount} | Leases: {x.ActiveLeaseCount} activos, {x.ExpiredLeaseCount} vencidos\r\nAlertas: {(x.ReasonCodes.Count==0?"ninguna":string.Join(", ",x.ReasonCodes))}"));
    }

    private static string FormatVersion(string? workerVersion) =>
        string.IsNullOrWhiteSpace(workerVersion) ? "no informada" : workerVersion;
}

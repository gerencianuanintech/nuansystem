using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.SriDocuments;
using NuanSystem.WinForms.Services.SriDocuments.Models;
namespace NuanSystem.WinForms.ViewModels.SriDocuments;
public sealed class SriDocumentMonitorViewModel(ISriDocumentMonitorClient client,bool canViewDetail,bool canDownload)
{
    public SriDocumentMonitorFilter Filter { get; }=new();
    public SriDocumentMonitorSummary? Summary { get; private set; }
    public IReadOnlyCollection<SriDocumentMonitorItem> Items { get; private set; }=[];
    public SriDocumentMonitorDetail? Detail { get; private set; }
    public SriDocumentMonitorItem? Selected { get; private set; }
    public IReadOnlyCollection<SriDocumentAttempt> Attempts { get; private set; }=[];
    public IReadOnlyCollection<SriDocumentAudit> Audit { get; private set; }=[];
    public bool CanDownload => canDownload && Selected is { Status:"Authorized", HasXml:true };
    public async Task LoadAsync(CancellationToken cancellationToken=default) { Summary=await client.GetSummaryAsync(cancellationToken); Items=await client.SearchAsync(Filter,cancellationToken); }
    public async Task LoadDetailAsync(long queueId,CancellationToken cancellationToken=default)
    {
        Selected=Items.FirstOrDefault(item=>item.QueueId==queueId);
        Detail=canViewDetail ? await client.GetDetailAsync(queueId,cancellationToken) : null;
        Attempts=await client.GetAttemptsAsync(queueId,cancellationToken);
        Audit=canViewDetail ? await client.GetAuditAsync(queueId,cancellationToken) : [];
    }
    public Task<ApiFileResponse> DownloadAsync(CancellationToken cancellationToken=default)
    {
        if(!CanDownload || Selected is null) throw new InvalidOperationException("El documento seleccionado no esta disponible para descarga.");
        return client.DownloadXmlAsync(Selected.QueueId,cancellationToken);
    }
}

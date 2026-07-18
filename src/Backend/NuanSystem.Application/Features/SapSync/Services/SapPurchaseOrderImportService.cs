using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Services;

public sealed class SapPurchaseOrderImportService(
    ISapPurchaseOrderReader reader,
    ISapPurchaseOrderImportRepository repository,
    ISapSyncLogRepository logRepository) : ISapPurchaseOrderImportService
{
    public async Task<SapPurchaseOrderImportResultDto> ImportAsync(int companyId, DateTime? modifiedSince, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default)
    {
        var documents=await reader.GetPurchaseOrdersAsync(companyId,modifiedSince,cancellationToken);
        var results=new List<SapPurchaseOrderImportItemResultDto>();
        foreach(var document in documents.OrderBy(x=>x.DocEntry))
        {
            try
            {
                if(document.DocEntry<=0||document.Lines.Count==0) { results.Add(new(document.DocEntry,document.DocNum,"Skipped","Documento sin identidad o lineas.",null));continue; }
                var version=Math.Max(1,document.UpdatedAt.ToUniversalTime().Ticks);
                var applied=await repository.UpsertAsync(new(StableGlobalId(companyId,document.DocEntry),document,version,auditUserId,auditUserName),cancellationToken);
                results.Add(new(document.DocEntry,document.DocNum,applied.Status,applied.Message,applied.PurchaseOrderId));
            }
            catch(Exception e) when(e is not OperationCanceledException)
            { results.Add(new(document.DocEntry,document.DocNum,"Failed",$"No fue posible importar la orden: {e.GetType().Name}.",null)); }
        }
        var summary=new SapPurchaseOrderImportResultDto(documents.Count,results.Count(x=>x.Status=="Created"),results.Count(x=>x.Status=="Updated"),
            results.Count(x=>x.Status=="Unchanged"),results.Count(x=>x.Status=="Skipped"),results.Count(x=>x.Status=="Failed"),results);
        await logRepository.CreateAsync(new(companyId,"PurchaseOrder","PurchaseOrders","22",null,JsonSerializer.Serialize(summary),summary.Failed>0?"Failed":"Succeeded",
            summary.Failed>0?$"Ordenes fallidas: {summary.Failed}.":null,null,null,DateTime.UtcNow),cancellationToken);
        return summary;
    }
    private static Guid StableGlobalId(int companyId,int docEntry)
    { var hash=SHA256.HashData(Encoding.UTF8.GetBytes($"SAP_B1|{companyId}|PurchaseOrder|{docEntry}"));return new Guid(hash[..16]); }
}

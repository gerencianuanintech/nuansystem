using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Commands;

public sealed record RetrySyncOutboxBatchCommand(IReadOnlyCollection<long> Ids,string Reason,bool ResetDeadLetterAttempts,string? AuditUserName)
    : ICommand<RetrySyncOutboxBatchResult>;
public sealed record RetrySyncOutboxBatchResult(int Requested,int Retried,int Skipped,IReadOnlyCollection<RetrySyncOutboxBatchItem> Items);
public sealed record RetrySyncOutboxBatchItem(long Id,string Status,string Message);

public sealed class RetrySyncOutboxBatchCommandHandler(ICompanyContext companyContext,ISyncOutboxRepository repository)
    : ICommandHandler<RetrySyncOutboxBatchCommand,RetrySyncOutboxBatchResult>
{
 public async Task<Result<RetrySyncOutboxBatchResult>> Handle(RetrySyncOutboxBatchCommand request,CancellationToken ct)
 {
  var ids=request.Ids.Distinct().Take(101).ToArray();
  if(ids.Length==0)return Result<RetrySyncOutboxBatchResult>.Failure("Seleccione al menos un evento.");
  if(ids.Length>100)return Result<RetrySyncOutboxBatchResult>.Failure("El reintento por lote admite hasta 100 eventos.");
  if(string.IsNullOrWhiteSpace(request.Reason))return Result<RetrySyncOutboxBatchResult>.Failure("El motivo del reintento por lote es obligatorio.");
  var company=companyContext.CurrentCompany??throw new InvalidOperationException("No hay empresa activa.");
  var items=new List<RetrySyncOutboxBatchItem>();
  foreach(var id in ids)
  {
   var current=await repository.GetOutboxDetailAsync(company.CompanyId,id,ct);
   if(current is null){items.Add(new(id,"Skipped","Evento no encontrado."));continue;}
   var applied=current.Status switch
   {
    SyncEventStatus.Error=>await repository.RetryErrorAsync(company.CompanyId,id,request.Reason.Trim(),request.AuditUserName,ct),
    SyncEventStatus.DeadLetter=>await repository.RetryDeadLetterAsync(company.CompanyId,id,request.Reason.Trim(),request.ResetDeadLetterAttempts,request.AuditUserName,ct),
    _=>null
   };
   items.Add(applied is null?new(id,"Skipped",$"Estado {current.Status} no reintentable."):new(id,"Retried","Evento devuelto a Pending."));
  }
  var value=new RetrySyncOutboxBatchResult(ids.Length,items.Count(x=>x.Status=="Retried"),items.Count(x=>x.Status=="Skipped"),items);
  return Result<RetrySyncOutboxBatchResult>.Success(value,"Reintento por lote procesado.");
 }
}

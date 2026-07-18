using System.Text.Json;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.MasterBranchSyncWorker.Services;

public sealed class PurchaseOrderSyncEventApplier(IPurchaseOrderSyncApplyRepository repository):ISyncEntityEventApplier
{
 private static readonly JsonSerializerOptions JsonOptions=new(JsonSerializerDefaults.Web);
 public bool CanApply(string name)=>string.Equals(name,SyncMasterBranchEntityCodes.PurchaseOrder,StringComparison.OrdinalIgnoreCase);
 public async Task<SyncEventApplyResult> ApplyAsync(SyncEventApplyContext context,CancellationToken ct=default)
 {
  if(context.TargetCompanyId is null)return new(false,"PurchaseOrder requiere sucursal destino.","SYNC_TARGET_REQUIRED");
  using var json=JsonDocument.Parse(context.PayloadJson);if(!json.RootElement.TryGetProperty("payload",out var node))throw new InvalidOperationException("Payload sin nodo payload.");
  var payload=node.Deserialize<PurchaseOrderSyncPayload>(JsonOptions)??throw new InvalidOperationException("Payload PurchaseOrder invalido.");
  if(payload.GlobalId!=context.EntityGlobalId)return new(false,"GlobalId de orden no coincide.","SYNC_PAYLOAD_GLOBAL_ID_MISMATCH");
  try{var result=await repository.ApplyAsync(context.TargetCompanyId.Value,context,payload,Enum.Parse<SyncOperation>(context.Operation,true),ct);return new(result.Applied,result.Message);}
  catch(InvalidOperationException e) when(e.Message.StartsWith("Falta dependencia",StringComparison.OrdinalIgnoreCase)){return new(false,e.Message,"SYNC_DEPENDENCY_MISSING",true);}
 }
}

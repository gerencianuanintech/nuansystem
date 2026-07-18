using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Application.Features.Sync.Services;

public interface IPurchaseOrderRoutingService
{
    Task<PurchaseOrderRoutingResult> RouteAsync(int purchaseOrderId,int? userId,string? userName,CancellationToken cancellationToken=default);
    Task<PurchaseOrderRoutingResult> ApproveAsync(PurchaseOrderRouteApprovalData approval,CancellationToken cancellationToken=default);
}

public sealed class PurchaseOrderRoutingService(IPurchaseOrderRoutingRepository repository,ISyncEventPublisher publisher,ICompanyContext companyContext)
    : IPurchaseOrderRoutingService
{
    public async Task<PurchaseOrderRoutingResult> RouteAsync(int id,int? userId,string? userName,CancellationToken ct=default)
    {
        var source=companyContext.CurrentCompany??throw new InvalidOperationException("Debe seleccionar la empresa Master.");
        var candidate=await repository.GetCandidateAsync(id,ct)??throw new InvalidOperationException("La orden de compra no existe.");
        var warehouses=candidate.Document.Lines.Select(x=>x.WarehouseCode).Where(x=>!string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var routes=await repository.ResolveTargetsAsync(source.CompanyId,warehouses,ct);
        var branchIds=routes.Select(x=>x.BranchCompanyId).Distinct().ToArray();
        if(branchIds.Length!=1||routes.Select(x=>x.WarehouseCode).Distinct(StringComparer.OrdinalIgnoreCase).Count()!=warehouses.Length)
        {
            var reason=branchIds.Length>1?"La orden contiene bodegas asignadas a distintas sucursales.":"Una o mas bodegas no tienen ruta activa.";
            await repository.MarkDecisionAsync(id,"NeedsApproval",null,reason,userId,userName,ct);
            return new(id,"NeedsApproval",null,reason);
        }
        return await PublishAsync(candidate,routes.First(),"Enrutamiento automatico por bodega.",userId,userName,ct);
    }

    public async Task<PurchaseOrderRoutingResult> ApproveAsync(PurchaseOrderRouteApprovalData approval,CancellationToken ct=default)
    {
        if(string.IsNullOrWhiteSpace(approval.Reason))throw new InvalidOperationException("El motivo de aprobacion es obligatorio.");
        var candidate=await repository.GetCandidateAsync(approval.PurchaseOrderId,ct)??throw new InvalidOperationException("La orden no existe.");
        if(!string.Equals(candidate.RoutingStatus,"NeedsApproval",StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Solo se aprueban ordenes NeedsApproval.");
        var routes=await repository.ResolveTargetsAsync(companyContext.CurrentCompany!.CompanyId,candidate.Document.Lines.Select(x=>x.WarehouseCode).Distinct().ToArray(),ct);
        var target=routes.FirstOrDefault(x=>x.BranchCompanyId==approval.BranchCompanyId)??throw new InvalidOperationException("La sucursal aprobada no participa en las rutas de la orden.");
        return await PublishAsync(candidate,target,"Aprobacion manual: "+approval.Reason.Trim(),approval.UserId,approval.UserName,ct);
    }

    private async Task<PurchaseOrderRoutingResult> PublishAsync(PurchaseOrderRoutingCandidate candidate,PurchaseOrderRouteTarget target,string reason,int? userId,string? userName,CancellationToken ct)
    {
        var request=new SyncPublishRequest(companyContext.CurrentCompany!.CompanyId,SyncMasterBranchEntityCodes.PurchaseOrder,candidate.GlobalId,
            candidate.Document.DocEntry.ToString(),SyncOperation.Updated,new PurchaseOrderSyncPayload(candidate.GlobalId,candidate.Document,candidate.SapVersion),
            SourceSystem:"SAP_B1",SourceReference:candidate.Document.DocEntry.ToString(),SyncProfileId:target.SyncProfileId,
            TargetBranchCode:target.BranchCompanyCode,RequireTargetBranchMatch:true);
        var published=await publisher.PublishAsync(request,ct);
        if(!published.IsSuccess||published.Value is null)throw new InvalidOperationException(published.Message);
        await repository.MarkDecisionAsync(candidate.PurchaseOrderId,"Routed",target.BranchCompanyId,reason,userId,userName,ct);
        return new(candidate.PurchaseOrderId,"Routed",target.BranchCompanyId,reason,published.Value.OutboxId);
    }
}

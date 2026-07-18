using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Application.Features.Sync.Services;

namespace NuanSystem.Application.Features.Sync.Commands;

public sealed record RoutePurchaseOrderCommand(int PurchaseOrderId,int? UserId=null,string? UserName=null):ICommand<PurchaseOrderRoutingResult>;
public sealed record ApprovePurchaseOrderRouteCommand(int PurchaseOrderId,int BranchCompanyId,string Reason,int? UserId=null,string? UserName=null):ICommand<PurchaseOrderRoutingResult>;
public sealed class RoutePurchaseOrderCommandHandler(IPurchaseOrderRoutingService service):ICommandHandler<RoutePurchaseOrderCommand,PurchaseOrderRoutingResult>
{
 public async Task<Result<PurchaseOrderRoutingResult>> Handle(RoutePurchaseOrderCommand r,CancellationToken ct)=>Result<PurchaseOrderRoutingResult>.Success(await service.RouteAsync(r.PurchaseOrderId,r.UserId,r.UserName,ct),"Enrutamiento evaluado.");
}
public sealed class ApprovePurchaseOrderRouteCommandHandler(IPurchaseOrderRoutingService service):ICommandHandler<ApprovePurchaseOrderRouteCommand,PurchaseOrderRoutingResult>
{
 public async Task<Result<PurchaseOrderRoutingResult>> Handle(ApprovePurchaseOrderRouteCommand r,CancellationToken ct)=>Result<PurchaseOrderRoutingResult>.Success(await service.ApproveAsync(new(r.PurchaseOrderId,r.BranchCompanyId,r.Reason,r.UserId,r.UserName),ct),"Ruta aprobada.");
}

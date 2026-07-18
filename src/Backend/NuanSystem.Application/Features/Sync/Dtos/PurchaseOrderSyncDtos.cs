using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.Sync.Dtos;

public sealed record PurchaseOrderSyncPayload(Guid GlobalId,SapPurchaseOrderRecord Document,long SapVersion);
public sealed record PurchaseOrderRoutingCandidate(int PurchaseOrderId,Guid GlobalId,SapPurchaseOrderRecord Document,long SapVersion,string RoutingStatus);
public sealed record PurchaseOrderRouteTarget(int BranchCompanyId,string BranchCompanyCode,string WarehouseCode,int SyncProfileId);
public sealed record PurchaseOrderRoutingResult(int PurchaseOrderId,string Status,int? BranchCompanyId,string Message,long? OutboxId=null);
public sealed record PurchaseOrderRouteApprovalData(int PurchaseOrderId,int BranchCompanyId,string Reason,int? UserId,string? UserName);
public sealed record PurchaseOrderSyncApplyResult(bool Applied,bool AlreadyApplied,int? LocalId,string Message);

using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class PurchaseOrderRoutingRepository(ITenantConnectionFactory tenantFactory,IMasterConnectionFactory masterFactory)
    : IPurchaseOrderRoutingRepository
{
    public async Task<PurchaseOrderRoutingCandidate?> GetCandidateAsync(int id,CancellationToken ct=default)
    {
        using var connection=tenantFactory.CreateConnection();
        const string headerSql="""
        SELECT Id PurchaseOrderId,GlobalId,SapDocEntry DocEntry,SapDocNum DocNum,DocumentDate,DeliveryDate,SupplierCode,SupplierName,CurrencyCode,
        ExchangeRate,TotalAmount DocumentTotal,TaxAmount TaxTotal,DiscountPercent,Status,SapStatus,CAST(CASE WHEN Status=N'Cancelled' THEN 1 ELSE 0 END AS bit) Cancelled,
        COALESCE(SapUpdatedAt,UpdatedAt,CreatedAt) UpdatedAt,Comments,SapVersion,RoutingStatus
        FROM dbo.PurchaseOrderHeaders WHERE Id=@Id AND IsDeleted=0;
        """;
        var h=await connection.QuerySingleOrDefaultAsync<Header>(new CommandDefinition(headerSql,new{Id=id},cancellationToken:ct));
        if(h is null)return null;
        const string lineSql="""
        SELECT LineNumber,ItemCode,ItemName,Quantity,OpenQuantity,UnitPrice,DiscountPercent,TaxCode,TaxRate,UnitCode,WarehouseCode,DeliveryDate,Status
        FROM dbo.PurchaseOrderLines WHERE PurchaseOrderId=@Id ORDER BY LineNumber;
        """;
        var lines=(await connection.QueryAsync<SapPurchaseOrderLineRecord>(new CommandDefinition(lineSql,new{Id=id},cancellationToken:ct))).AsList();
        var doc=new SapPurchaseOrderRecord(h.DocEntry,h.DocNum,h.DocumentDate,h.DeliveryDate,h.SupplierCode,h.SupplierName,h.CurrencyCode,h.ExchangeRate,
            h.DocumentTotal,h.TaxTotal,h.DiscountPercent,h.Status,h.Cancelled,h.UpdatedAt,h.Comments,lines);
        return new(h.PurchaseOrderId,h.GlobalId,doc,h.SapVersion,h.RoutingStatus);
    }
    public async Task<IReadOnlyCollection<PurchaseOrderRouteTarget>> ResolveTargetsAsync(int sourceCompanyId,IReadOnlyCollection<string> warehouseCodes,CancellationToken ct=default)
    {
        if(warehouseCodes.Count==0)return [];
        using var connection=masterFactory.CreateConnection();
        var rows=await connection.QueryAsync<PurchaseOrderRouteTarget>(new CommandDefinition("""
            SELECT route.BranchCompanyId,company.BranchCode BranchCompanyCode,route.WarehouseCode,profile.Id SyncProfileId
            FROM dbo.PurchaseOrderWarehouseRoutes route INNER JOIN dbo.Companies company ON company.Id=route.BranchCompanyId AND company.IsDeleted=0
            CROSS APPLY
            (
             SELECT TOP(1) p.Id
             FROM dbo.SyncProfiles p
             INNER JOIN dbo.SyncProfileEntities e ON e.SyncProfileId=p.Id AND e.EntityCode=N'PurchaseOrder' AND e.IsActive=1 AND e.IsDeleted=0
             INNER JOIN dbo.SyncProfileBranches b ON b.SyncProfileId=p.Id AND b.BranchCompanyId=route.BranchCompanyId AND b.IsActive=1 AND b.IsDeleted=0
             INNER JOIN dbo.SyncProfileEntityBranches m ON m.SyncProfileId=p.Id AND m.SyncProfileEntityId=e.Id AND m.SyncProfileBranchId=b.Id AND m.IsEnabled=1 AND m.IsDeleted=0
             WHERE p.CompanyId=route.SourceCompanyId AND p.IsActive=1 AND p.IsDeleted=0 AND p.Direction=N'MasterToBranch'
             ORDER BY p.Id
            ) profile
            WHERE route.SourceCompanyId=@SourceCompanyId AND route.IsActive=1 AND route.WarehouseCode IN @WarehouseCodes;
            """,new{SourceCompanyId=sourceCompanyId,WarehouseCodes=warehouseCodes},cancellationToken:ct));
        return rows.AsList();
    }
    public async Task MarkDecisionAsync(int id,string status,int? branchId,string reason,int? userId,string? userName,CancellationToken ct=default)
    {
        using var connection=tenantFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition("""
            UPDATE dbo.PurchaseOrderHeaders SET RoutingStatus=@Status,RoutedBranchCompanyId=@BranchId,RoutingDecisionAt=SYSUTCDATETIME(),
            RoutingDecisionBy=@UserName,RoutingReason=@Reason,UpdatedByUserId=@UserId,UpdatedByUserName=@UserName,UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;
            INSERT dbo.PurchaseOrderRoutingAudit(PurchaseOrderId,PreviousStatus,NewStatus,BranchCompanyId,Reason,CreatedByUserId,CreatedByUserName)
            SELECT @Id,COALESCE((SELECT TOP(1) NewStatus FROM dbo.PurchaseOrderRoutingAudit WHERE PurchaseOrderId=@Id ORDER BY Id DESC),N'Pending'),
            @Status,@BranchId,@Reason,@UserId,@UserName;
            """,new{Id=id,Status=status,BranchId=branchId,Reason=reason,UserId=userId,UserName=userName},cancellationToken:ct));
    }
    private sealed record Header(int PurchaseOrderId,Guid GlobalId,int DocEntry,int DocNum,DateTime DocumentDate,DateTime DeliveryDate,string SupplierCode,
        string SupplierName,string CurrencyCode,decimal ExchangeRate,decimal DocumentTotal,decimal TaxTotal,decimal DiscountPercent,string Status,string SapStatus,
        bool Cancelled,DateTime UpdatedAt,string? Comments,long SapVersion,string RoutingStatus);
}

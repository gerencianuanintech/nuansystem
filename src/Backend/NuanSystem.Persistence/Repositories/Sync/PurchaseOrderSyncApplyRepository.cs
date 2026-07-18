using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class PurchaseOrderSyncApplyRepository(ICompanyResolver resolver) : IPurchaseOrderSyncApplyRepository
{
 public async Task<PurchaseOrderSyncApplyResult> ApplyAsync(int branchId,SyncEventApplyContext context,PurchaseOrderSyncPayload payload,SyncOperation operation,CancellationToken ct=default)
 {
  var company=await resolver.ResolveByIdAsync(branchId,ct)??throw new InvalidOperationException($"Sucursal {branchId} no existe.");
  if(company.DatabaseEngine!=DatabaseEngine.SqlServer)throw new NotSupportedException("PurchaseOrder Sync solo soporta SQL Server actualmente.");
  await using var cn=new SqlConnection(company.ConnectionString);await cn.OpenAsync(ct);await using var tx=await cn.BeginTransactionAsync(ct);
  try
  {
   var inbox=await cn.QuerySingleOrDefaultAsync<Inbox>(new CommandDefinition("SELECT TOP(1) Id,Status FROM dbo.SyncInbox WITH(UPDLOCK,HOLDLOCK) WHERE EventId=@EventId",new{context.EventId},tx,cancellationToken:ct));
   if(inbox?.Status=="Applied"){await tx.CommitAsync(ct);return new(true,true,null,"Evento de orden ya aplicado.");}
   var inboxId=inbox?.Id??await cn.ExecuteScalarAsync<long>(new CommandDefinition("""
    INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
    VALUES(@EventId,@SourceCompanyId,@EntityName,@EntityGlobalId,@Operation,@PayloadJson,N'Pending');SELECT CONVERT(bigint,SCOPE_IDENTITY());
    """,context,tx,cancellationToken:ct));
   var d=payload.Document;
   var supplier=await Lookup(cn,tx,"BusinessPartners","ExternalCode",d.SupplierCode,ct)??await Lookup(cn,tx,"BusinessPartners","Code",d.SupplierCode,ct)
    ??throw new MissingDependencyException("BusinessPartner",d.SupplierCode);
   var rows=new List<Line>();
   foreach(var line in d.Lines)
   {
    var item=await LookupAny(cn,tx,"Items",line.ItemCode,"SapCode","ExternalCode","Code",ct)??throw new MissingDependencyException("Item",line.ItemCode);
    var wh=await LookupAny(cn,tx,"Warehouses",line.WarehouseCode,"SapCode","ExternalCode","Code",ct)??throw new MissingDependencyException("Warehouse",line.WarehouseCode);
    var unit=string.IsNullOrWhiteSpace(line.UnitCode)?null:await LookupAny(cn,tx,"UnitOfMeasures",line.UnitCode!,"ExternalCode","Code",ct);
    var tax=string.IsNullOrWhiteSpace(line.TaxCode)?null:await LookupAny(cn,tx,"Taxes",line.TaxCode,"ExternalCode","Code",ct);
    rows.Add(new(line,item,wh,unit,tax));
   }
   var current=await cn.QuerySingleOrDefaultAsync<Current>(new CommandDefinition("SELECT Id,SapVersion FROM dbo.PurchaseOrderHeaders WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId OR (SapDocEntry=@DocEntry AND IsDeleted=0)",new{payload.GlobalId,d.DocEntry},tx,cancellationToken:ct));
   if(current is not null&&current.SapVersion>payload.SapVersion){await MarkInbox(cn,tx,inboxId,ct);await tx.CommitAsync(ct);return new(true,false,current.Id,"Version anterior ignorada.");}
   var subtotal=rows.Sum(x=>x.Source.Quantity*x.Source.UnitPrice*(1-x.Source.DiscountPercent/100m));
   var taxTotal=rows.Sum(x=>x.Source.Quantity*x.Source.UnitPrice*(1-x.Source.DiscountPercent/100m)*x.Source.TaxRate/100m);
   var p=new DynamicParameters();p.Add("GlobalId",payload.GlobalId);p.Add("DocEntry",d.DocEntry);p.Add("DocNum",d.DocNum);p.Add("SupplierId",supplier.Id);p.Add("SupplierCode",supplier.Code);p.Add("SupplierName",supplier.Name);
   p.Add("DocumentDate",d.DocumentDate);p.Add("DeliveryDate",d.DeliveryDate);p.Add("CurrencyCode",d.CurrencyCode);p.Add("ExchangeRate",d.ExchangeRate);p.Add("WarehouseId",rows.First().Warehouse.Id);p.Add("Comments",d.Comments);
   p.Add("Status",d.Cancelled?"Cancelled":d.Status);p.Add("Subtotal",subtotal);p.Add("TaxAmount",taxTotal);p.Add("TotalAmount",d.DocumentTotal);p.Add("TotalItems",rows.Count);p.Add("TotalQuantity",rows.Sum(x=>x.Source.Quantity));p.Add("SapVersion",payload.SapVersion);p.Add("SapUpdatedAt",d.UpdatedAt);
   int id;
   if(current is null)
   {
    id=await cn.ExecuteScalarAsync<int>(new CommandDefinition("""
     INSERT dbo.PurchaseOrderHeaders(GlobalId,SeriesCode,DocumentNumber,SupplierId,SupplierCode,SupplierName,DocumentDate,DeliveryDate,CurrencyCode,ExchangeRate,
     MainWarehouseId,Comments,Status,Subtotal,DiscountPercent,DiscountAmount,TaxAmount,TotalAmount,TotalItems,TotalQuantity,TotalWeight,SapObjectType,SapStatus,
     SapDocEntry,SapDocNum,SapSyncDate,SapUpdatedAt,SapVersion,RoutingStatus,RoutedBranchCompanyId,IsDeleted,CreatedByUserName,CreatedAt)
     VALUES(@GlobalId,N'SAP',CONVERT(nvarchar(50),@DocNum),@SupplierId,@SupplierCode,@SupplierName,@DocumentDate,@DeliveryDate,@CurrencyCode,@ExchangeRate,
     @WarehouseId,@Comments,@Status,@Subtotal,0,0,@TaxAmount,@TotalAmount,@TotalItems,@TotalQuantity,0,N'22',@Status,@DocEntry,@DocNum,SYSUTCDATETIME(),
     @SapUpdatedAt,@SapVersion,N'Applied',NULL,0,N'MasterBranchSyncWorker',SYSUTCDATETIME());SELECT CONVERT(int,SCOPE_IDENTITY());
     """,p,tx,cancellationToken:ct));
   }
   else
   {
    id=current.Id;p.Add("Id",id);
    await cn.ExecuteAsync(new CommandDefinition("""
     UPDATE dbo.PurchaseOrderHeaders SET GlobalId=@GlobalId,SupplierId=@SupplierId,SupplierCode=@SupplierCode,SupplierName=@SupplierName,
     DocumentDate=@DocumentDate,DeliveryDate=@DeliveryDate,CurrencyCode=@CurrencyCode,ExchangeRate=@ExchangeRate,MainWarehouseId=@WarehouseId,Comments=@Comments,
     Status=@Status,Subtotal=@Subtotal,TaxAmount=@TaxAmount,TotalAmount=@TotalAmount,TotalItems=@TotalItems,TotalQuantity=@TotalQuantity,SapStatus=@Status,
     SapDocNum=@DocNum,SapSyncDate=SYSUTCDATETIME(),SapUpdatedAt=@SapUpdatedAt,SapVersion=@SapVersion,RoutingStatus=N'Applied',UpdatedAt=SYSUTCDATETIME(),
     UpdatedByUserName=N'MasterBranchSyncWorker' WHERE Id=@Id;DELETE dbo.PurchaseOrderLines WHERE PurchaseOrderId=@Id;
     """,p,tx,cancellationToken:ct));
   }
   foreach(var row in rows)
   {
    var s=row.Source;var baseValue=s.Quantity*s.UnitPrice;var discount=baseValue*s.DiscountPercent/100m;var net=baseValue-discount;var lineTax=net*s.TaxRate/100m;
    await cn.ExecuteAsync(new CommandDefinition("""
     INSERT dbo.PurchaseOrderLines(PurchaseOrderId,LineNumber,ItemId,ItemCode,ItemName,UnitId,UnitCode,Quantity,OpenQuantity,UnitPrice,DiscountPercent,
     DiscountAmount,TaxId,TaxCode,TaxRate,TaxAmount,WarehouseId,WarehouseCode,DeliveryDate,LineSubtotal,LineTotal,Status,CreatedAt)
     VALUES(@Id,@LineNumber,@ItemId,@ItemCode,@ItemName,@UnitId,@UnitCode,@Quantity,@OpenQuantity,@UnitPrice,@DiscountPercent,@DiscountAmount,@TaxId,
     @TaxCode,@TaxRate,@TaxAmount,@WarehouseId,@WarehouseCode,@DeliveryDate,@LineSubtotal,@LineTotal,@Status,SYSUTCDATETIME());
     """,new{Id=id,s.LineNumber,ItemId=row.Item.Id,ItemCode=row.Item.Code,ItemName=row.Item.Name,UnitId=row.Unit?.Id,s.UnitCode,s.Quantity,s.OpenQuantity,s.UnitPrice,s.DiscountPercent,DiscountAmount=discount,TaxId=row.Tax?.Id,s.TaxCode,s.TaxRate,TaxAmount=lineTax,WarehouseId=row.Warehouse.Id,WarehouseCode=row.Warehouse.Code,s.DeliveryDate,LineSubtotal=net,LineTotal=net+lineTax,s.Status},tx,cancellationToken:ct));
   }
   await MarkInbox(cn,tx,inboxId,ct);await tx.CommitAsync(ct);return new(true,false,id,"Orden de compra aplicada transaccionalmente.");
  }
  catch{await tx.RollbackAsync(CancellationToken.None);throw;}
 }
 private static async Task<LookupRow?> Lookup(SqlConnection c,System.Data.IDbTransaction t,string table,string field,string value,CancellationToken ct)=>await c.QuerySingleOrDefaultAsync<LookupRow>(new CommandDefinition($"SELECT TOP(1) Id,Code,Name FROM dbo.{table} WHERE IsDeleted=0 AND {field}=@Value",new{Value=value},t,cancellationToken:ct));
 private static async Task<LookupRow?> LookupAny(SqlConnection c,System.Data.IDbTransaction t,string table,string value,params object[] args)
 { var ct=(CancellationToken)args[^1];var fields=args[..^1].Cast<string>().ToArray();var where=string.Join(" OR ",fields.Select(x=>$"{x}=@Value"));return await c.QuerySingleOrDefaultAsync<LookupRow>(new CommandDefinition($"SELECT TOP(1) Id,Code,Name FROM dbo.{table} WHERE IsDeleted=0 AND ({where})",new{Value=value},t,cancellationToken:ct)); }
 private static Task MarkInbox(SqlConnection c,System.Data.IDbTransaction t,long id,CancellationToken ct)=>c.ExecuteAsync(new CommandDefinition("UPDATE dbo.SyncInbox SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL WHERE Id=@Id",new{Id=id},t,cancellationToken:ct));
 private sealed record Inbox(long Id,string Status);private sealed record Current(int Id,long SapVersion);private sealed record LookupRow(int Id,string Code,string Name);private sealed record Line(NuanSystem.Application.Features.SapSync.Dtos.SapPurchaseOrderLineRecord Source,LookupRow Item,LookupRow Warehouse,LookupRow? Unit,LookupRow? Tax);
 private sealed class MissingDependencyException(string entity,string code):InvalidOperationException($"Falta dependencia {entity} con codigo {code}.");
}

using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SapPurchaseOrderImportRepository(ITenantConnectionFactory connectionFactory)
    : ISapPurchaseOrderImportRepository
{
    public async Task<SapPurchaseOrderImportApplyResult> UpsertAsync(SapPurchaseOrderImportData data, CancellationToken cancellationToken = default)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            var current = await connection.QuerySingleOrDefaultAsync<CurrentOrder>(new CommandDefinition(
                "SELECT Id,GlobalId,SapVersion FROM dbo.PurchaseOrderHeaders WITH(UPDLOCK,HOLDLOCK) WHERE SapDocEntry=@DocEntry AND IsDeleted=0;",
                new { data.Document.DocEntry }, transaction, cancellationToken: cancellationToken));
            if (current is not null && current.SapVersion > data.SapVersion)
            {
                await transaction.CommitAsync(cancellationToken);
                return new("Skipped", current.Id, "Version SAP anterior ignorada.");
            }
            if (current is not null && current.SapVersion == data.SapVersion)
            {
                await transaction.CommitAsync(cancellationToken);
                return new("Unchanged", current.Id, "La version SAP ya fue aplicada.");
            }

            var supplier = await connection.QuerySingleOrDefaultAsync<Lookup>(new CommandDefinition(
                "SELECT TOP(1) Id,Code,Name FROM dbo.BusinessPartners WHERE IsDeleted=0 AND PartnerType IN(N'Supplier',N'Both') AND (ExternalCode=@Code OR Code=@Code) ORDER BY CASE WHEN ExternalCode=@Code THEN 0 ELSE 1 END;",
                new { Code = data.Document.SupplierCode }, transaction, cancellationToken: cancellationToken));
            if (supplier is null) throw new InvalidOperationException($"Proveedor SAP {data.Document.SupplierCode} no existe en Master.");

            var lines = new List<ResolvedLine>();
            foreach (var line in data.Document.Lines.OrderBy(x => x.LineNumber))
            {
                var item = await connection.QuerySingleOrDefaultAsync<Lookup>(new CommandDefinition(
                    "SELECT TOP(1) Id,Code,Name FROM dbo.Items WHERE IsDeleted=0 AND (SapCode=@Code OR ExternalCode=@Code OR Code=@Code) ORDER BY CASE WHEN SapCode=@Code THEN 0 WHEN ExternalCode=@Code THEN 1 ELSE 2 END;",
                    new { Code = line.ItemCode }, transaction, cancellationToken: cancellationToken));
                var warehouse = await connection.QuerySingleOrDefaultAsync<Lookup>(new CommandDefinition(
                    "SELECT TOP(1) Id,Code,Name FROM dbo.Warehouses WHERE IsDeleted=0 AND (SapCode=@Code OR ExternalCode=@Code OR Code=@Code) ORDER BY CASE WHEN SapCode=@Code THEN 0 WHEN ExternalCode=@Code THEN 1 ELSE 2 END;",
                    new { Code = line.WarehouseCode }, transaction, cancellationToken: cancellationToken));
                if (item is null) throw new InvalidOperationException($"Articulo SAP {line.ItemCode} no existe en Master.");
                if (warehouse is null) throw new InvalidOperationException($"Bodega SAP {line.WarehouseCode} no existe en Master.");
                var unitId = string.IsNullOrWhiteSpace(line.UnitCode) ? null : await connection.ExecuteScalarAsync<int?>(new CommandDefinition(
                    "SELECT TOP(1) Id FROM dbo.UnitOfMeasures WHERE IsDeleted=0 AND (ExternalCode=@Code OR Code=@Code);", new { Code=line.UnitCode }, transaction, cancellationToken:cancellationToken));
                var taxId = string.IsNullOrWhiteSpace(line.TaxCode) ? null : await connection.ExecuteScalarAsync<int?>(new CommandDefinition(
                    "SELECT TOP(1) Id FROM dbo.Taxes WHERE IsDeleted=0 AND (ExternalCode=@Code OR Code=@Code);", new { Code=line.TaxCode }, transaction, cancellationToken:cancellationToken));
                lines.Add(new(line,item,warehouse,unitId,taxId));
            }

            var subtotal = lines.Sum(x => x.Line.Quantity * x.Line.UnitPrice * (1m - x.Line.DiscountPercent / 100m));
            var tax = lines.Sum(x => x.Line.Quantity * x.Line.UnitPrice * (1m - x.Line.DiscountPercent / 100m) * x.Line.TaxRate / 100m);
            var status = data.Document.Cancelled ? "Cancelled" : data.Document.Status;
            var sapStatus = data.Document.Cancelled ? "Cancelled" : data.Document.Status;
            var id = current?.Id ?? 0;
            if (current is null)
            {
                id = await connection.ExecuteScalarAsync<int>(new CommandDefinition("""
                    INSERT dbo.PurchaseOrderHeaders(GlobalId,SeriesCode,DocumentNumber,SupplierId,SupplierCode,SupplierName,DocumentDate,DeliveryDate,
                    CurrencyCode,ExchangeRate,MainWarehouseId,Comments,Status,Subtotal,DiscountPercent,DiscountAmount,TaxAmount,TotalAmount,
                    TotalItems,TotalQuantity,TotalWeight,SapObjectType,SapStatus,SapDocEntry,SapDocNum,SapSyncDate,SapUpdatedAt,SapVersion,RoutingStatus,
                    IsDeleted,CreatedByUserId,CreatedByUserName,CreatedAt)
                    VALUES(@GlobalId,N'SAP',@DocumentNumber,@SupplierId,@SupplierCode,@SupplierName,@DocumentDate,@DeliveryDate,@CurrencyCode,@ExchangeRate,
                    @WarehouseId,@Comments,@Status,@Subtotal,@DiscountPercent,@DiscountAmount,@TaxAmount,@TotalAmount,@TotalItems,@TotalQuantity,0,N'22',
                    @SapStatus,@SapDocEntry,@SapDocNum,SYSUTCDATETIME(),@SapUpdatedAt,@SapVersion,N'Pending',0,@AuditUserId,@AuditUserName,SYSUTCDATETIME());
                    SELECT CONVERT(int,SCOPE_IDENTITY());
                    """, HeaderParams(data, supplier, lines, subtotal, tax, status, sapStatus), transaction, cancellationToken:cancellationToken));
            }
            if (current is not null)
            {
                var p = HeaderParams(data, supplier, lines, subtotal, tax, status, sapStatus);
                await connection.ExecuteAsync(new CommandDefinition("""
                    UPDATE dbo.PurchaseOrderHeaders SET SupplierId=@SupplierId,SupplierCode=@SupplierCode,SupplierName=@SupplierName,DocumentDate=@DocumentDate,
                    DeliveryDate=@DeliveryDate,CurrencyCode=@CurrencyCode,ExchangeRate=@ExchangeRate,MainWarehouseId=@WarehouseId,Comments=@Comments,Status=@Status,
                    Subtotal=@Subtotal,DiscountPercent=@DiscountPercent,DiscountAmount=@DiscountAmount,TaxAmount=@TaxAmount,TotalAmount=@TotalAmount,
                    TotalItems=@TotalItems,TotalQuantity=@TotalQuantity,SapStatus=@SapStatus,SapDocNum=@SapDocNum,SapSyncDate=SYSUTCDATETIME(),SapUpdatedAt=@SapUpdatedAt,
                    SapVersion=@SapVersion,RoutingStatus=N'Pending',RoutedBranchCompanyId=NULL,RoutingDecisionAt=NULL,RoutingDecisionBy=NULL,RoutingReason=NULL,
                    UpdatedByUserId=@AuditUserId,UpdatedByUserName=@AuditUserName,UpdatedAt=SYSUTCDATETIME() WHERE Id=@Id;
                    DELETE dbo.PurchaseOrderLines WHERE PurchaseOrderId=@Id;
                    """, MergeId(p,id), transaction, cancellationToken:cancellationToken));
            }

            foreach (var row in lines)
            {
                var baseAmount = row.Line.Quantity * row.Line.UnitPrice;
                var discount = baseAmount * row.Line.DiscountPercent / 100m;
                var lineSubtotal = baseAmount - discount;
                var lineTax = lineSubtotal * row.Line.TaxRate / 100m;
                await connection.ExecuteAsync(new CommandDefinition("""
                    INSERT dbo.PurchaseOrderLines(PurchaseOrderId,LineNumber,ItemId,ItemCode,ItemName,UnitId,UnitCode,Quantity,OpenQuantity,UnitPrice,
                    DiscountPercent,DiscountAmount,TaxId,TaxCode,TaxRate,TaxAmount,WarehouseId,WarehouseCode,DeliveryDate,LineSubtotal,LineTotal,Status,CreatedAt)
                    VALUES(@PurchaseOrderId,@LineNumber,@ItemId,@ItemCode,@ItemName,@UnitId,@UnitCode,@Quantity,@OpenQuantity,@UnitPrice,@DiscountPercent,
                    @DiscountAmount,@TaxId,@TaxCode,@TaxRate,@TaxAmount,@WarehouseId,@WarehouseCode,@DeliveryDate,@LineSubtotal,@LineTotal,@Status,SYSUTCDATETIME());
                    """, new { PurchaseOrderId=id,row.Line.LineNumber,ItemId=row.Item.Id,ItemCode=row.Item.Code,ItemName=row.Item.Name,row.UnitId,row.Line.UnitCode,
                        row.Line.Quantity,row.Line.OpenQuantity,row.Line.UnitPrice,row.Line.DiscountPercent,DiscountAmount=discount,row.TaxId,row.Line.TaxCode,
                        row.Line.TaxRate,TaxAmount=lineTax,WarehouseId=row.Warehouse.Id,WarehouseCode=row.Warehouse.Code,row.Line.DeliveryDate,
                        LineSubtotal=lineSubtotal,LineTotal=lineSubtotal+lineTax,row.Line.Status }, transaction, cancellationToken:cancellationToken));
            }
            await transaction.CommitAsync(cancellationToken);
            return new(current is null ? "Created" : "Updated", id, current is null ? "Orden SAP creada." : "Orden SAP actualizada.");
        }
        catch { await transaction.RollbackAsync(CancellationToken.None); throw; }
    }

    private static object HeaderParams(SapPurchaseOrderImportData data, Lookup supplier, List<ResolvedLine> lines, decimal subtotal, decimal tax, string status, string sapStatus) => new
    {
        data.GlobalId, DocumentNumber=data.Document.DocNum.ToString(), SupplierId=supplier.Id,SupplierCode=supplier.Code,SupplierName=supplier.Name,
        data.Document.DocumentDate,data.Document.DeliveryDate,data.Document.CurrencyCode,data.Document.ExchangeRate,WarehouseId=lines.First().Warehouse.Id,
        data.Document.Comments,Status=status,Subtotal=subtotal,data.Document.DiscountPercent,DiscountAmount=Math.Max(0,subtotal+tax-data.Document.DocumentTotal),
        TaxAmount=tax,TotalAmount=data.Document.DocumentTotal,TotalItems=lines.Count,TotalQuantity=lines.Sum(x=>x.Line.Quantity),SapStatus=sapStatus,
        SapDocEntry=data.Document.DocEntry,SapDocNum=data.Document.DocNum,SapUpdatedAt=data.Document.UpdatedAt,data.SapVersion,data.AuditUserId,data.AuditUserName
    };
    private static DynamicParameters MergeId(object values, int id) { var p=new DynamicParameters(values);p.Add("Id",id);return p; }
    private sealed record CurrentOrder(int Id, Guid GlobalId, long SapVersion);
    private sealed record Lookup(int Id,string Code,string Name);
    private sealed record ResolvedLine(SapPurchaseOrderLineRecord Line,Lookup Item,Lookup Warehouse,int? UnitId,int? TaxId);
}

namespace NuanSystem.WinForms.Services.Purchasing.PurchaseOrders.Models;

public class PurchaseOrderItem
{
    public int Id { get; set; }
    public string SeriesCode { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string SupplierCode { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public decimal TotalAmount { get; set; }
    public string SapStatus { get; set; } = "Pending";
}

public sealed class PurchaseOrderDetail : PurchaseOrderItem
{
    public int? BranchId { get; set; }
    public int? DocumentSeriesId { get; set; }
    public int SupplierId { get; set; }
    public string? SupplierTaxId { get; set; }
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal ExchangeRate { get; set; }
    public int? PaymentTermId { get; set; }
    public int? PriceListId { get; set; }
    public int? BuyerId { get; set; }
    public int? MainWarehouseId { get; set; }
    public int? ProjectId { get; set; }
    public int? CostCenterId { get; set; }
    public int? PurchaseTypeId { get; set; }
    public string? Comments { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalWeight { get; set; }
    public string SapObjectType { get; set; } = "22";
    public int? SapDocEntry { get; set; }
    public int? SapDocNum { get; set; }
    public DateTime? SapSyncDate { get; set; }
    public string? SapMessage { get; set; }
    public IReadOnlyCollection<PurchaseOrderLineItem> Lines { get; set; } = [];
    public IReadOnlyCollection<PurchaseOrderAddressItem> Addresses { get; set; } = [];
    public IReadOnlyCollection<PurchaseOrderApprovalItem> Approvals { get; set; } = [];
    public IReadOnlyCollection<PurchaseOrderRelatedDocumentItem> RelatedDocuments { get; set; } = [];
    public IReadOnlyCollection<PurchaseOrderAttachmentItem> Attachments { get; set; } = [];
    public IReadOnlyCollection<PurchaseOrderSapSyncLogItem> SapLogs { get; set; } = [];
}

public sealed class PurchaseOrderLineItem
{
    public int? Id { get; set; }
    public int LineNumber { get; set; }
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int? UnitId { get; set; }
    public string? UnitCode { get; set; }
    public decimal Quantity { get; set; }
    public decimal OpenQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public int? TaxId { get; set; }
    public string TaxCode { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public int WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; } = DateTime.Today;
    public int? CostCenterId { get; set; }
    public int? ProjectId { get; set; }
    public decimal LineSubtotal { get; set; }
    public decimal LineTotal { get; set; }
    public string Status { get; set; } = "Open";
}

public sealed class PurchaseOrderAddressItem
{
    public int? Id { get; set; }
    public string AddressType { get; set; } = string.Empty;
    public int? SourceAddressId { get; set; }
    public string? AddressName { get; set; }
    public string? Street { get; set; }
    public string? Reference { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsModified { get; set; }
}

public sealed class PurchaseOrderApprovalItem
{
    public int Id { get; set; }
    public int ApprovalLevel { get; set; }
    public string? RoleName { get; set; }
    public string? UserName { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Observation { get; set; }
}

public sealed class PurchaseOrderRelatedDocumentItem
{
    public int? Id { get; set; }
    public string RelatedDocumentType { get; set; } = string.Empty;
    public int? RelatedDocumentId { get; set; }
    public string? Series { get; set; }
    public string? Number { get; set; }
    public DateTime? Date { get; set; }
    public string? Status { get; set; }
    public string? Reference { get; set; }
    public string? Comment { get; set; }
    public decimal Total { get; set; }
}

public sealed class PurchaseOrderAttachmentItem
{
    public int? Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string? FileExtension { get; set; }
    public string? MimeType { get; set; }
    public long FileSize { get; set; }
    public string? StoragePath { get; set; }
    public string Status { get; set; } = "Active";
    public string? Comment { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class PurchaseOrderSapSyncLogItem
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Process { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? UserName { get; set; }
    public int AttemptNumber { get; set; }
}

public sealed record SavePurchaseOrderRequest(
    int? BranchId,
    int? DocumentSeriesId,
    string SeriesCode,
    string DocumentNumber,
    int SupplierId,
    string SupplierCode,
    string SupplierName,
    string? SupplierTaxId,
    string? ContactName,
    string? Phone,
    string? Email,
    DateTime DocumentDate,
    DateTime DeliveryDate,
    string CurrencyCode,
    decimal ExchangeRate,
    int? PaymentTermId,
    int? PriceListId,
    int? BuyerId,
    int? MainWarehouseId,
    int? ProjectId,
    int? CostCenterId,
    int? PurchaseTypeId,
    string? Comments,
    decimal DiscountPercent,
    IReadOnlyCollection<PurchaseOrderLineItem> Lines,
    IReadOnlyCollection<PurchaseOrderAddressItem> Addresses,
    IReadOnlyCollection<PurchaseOrderRelatedDocumentItem>? RelatedDocuments,
    IReadOnlyCollection<PurchaseOrderAttachmentItem>? Attachments);

public sealed record PurchaseOrderWorkflowRequest(string? Observation);

public sealed record PurchaseOrderLookups(
    IReadOnlyCollection<PurchaseOrderLookupOption> Suppliers,
    IReadOnlyCollection<PurchaseOrderLookupOption> Items,
    IReadOnlyCollection<PurchaseOrderLookupOption> Units,
    IReadOnlyCollection<PurchaseOrderWarehouseLookup> Warehouses,
    IReadOnlyCollection<PurchaseOrderTaxLookup> Taxes,
    IReadOnlyCollection<PurchaseOrderLookupOption> Currencies,
    IReadOnlyCollection<PurchaseOrderLookupOption> PaymentTerms,
    IReadOnlyCollection<PurchaseOrderLookupOption> PriceLists,
    IReadOnlyCollection<PurchaseOrderLookupOption> Buyers,
    IReadOnlyCollection<PurchaseOrderLookupOption> CostCenters,
    IReadOnlyCollection<PurchaseOrderLookupOption> Projects,
    IReadOnlyCollection<PurchaseOrderLookupOption> PurchaseTypes,
    IReadOnlyCollection<PurchaseOrderLookupOption> DocumentSeries);

public sealed record PurchaseOrderLookupOption(int Id, string Code, string Name, bool IsActive = true)
{
    public string DisplayText => string.IsNullOrWhiteSpace(Code) ? Name : $"{Code} - {Name}";
}

public sealed record PurchaseOrderWarehouseLookup(int Id, string Code, string Name, bool IsActive = true)
{
    public string DisplayText => string.IsNullOrWhiteSpace(Code) ? Name : $"{Code} - {Name}";
}

public sealed record PurchaseOrderTaxLookup(int Id, string Code, string Name, decimal Rate, bool IsActive = true)
{
    public string DisplayText => string.IsNullOrWhiteSpace(Code) ? Name : $"{Code} - {Name}";
}

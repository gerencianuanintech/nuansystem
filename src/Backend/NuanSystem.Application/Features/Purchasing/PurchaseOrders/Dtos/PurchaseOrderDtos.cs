namespace NuanSystem.Application.Features.Purchasing.PurchaseOrders.Dtos;

public sealed class PurchaseOrderDto
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public int? DocumentSeriesId { get; set; }
    public string SeriesCode { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierCode { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierTaxId { get; set; }
    public string? ContactName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime DocumentDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public int? PaymentTermId { get; set; }
    public int? PriceListId { get; set; }
    public int? BuyerId { get; set; }
    public int? MainWarehouseId { get; set; }
    public int? ProjectId { get; set; }
    public int? CostCenterId { get; set; }
    public int? PurchaseTypeId { get; set; }
    public string? Comments { get; set; }
    public string Status { get; set; } = PurchaseOrderStatuses.Draft;
    public decimal Subtotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalWeight { get; set; }
    public string SapObjectType { get; set; } = "22";
    public string SapStatus { get; set; } = PurchaseOrderSapStatuses.Pending;
    public int? SapDocEntry { get; set; }
    public int? SapDocNum { get; set; }
    public DateTime? SapSyncDate { get; set; }
    public string? SapMessage { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyCollection<PurchaseOrderLineDto> Lines { get; set; } = [];
    public IReadOnlyCollection<PurchaseOrderAddressDto> Addresses { get; set; } = [];
    public IReadOnlyCollection<PurchaseOrderApprovalDto> Approvals { get; set; } = [];
    public IReadOnlyCollection<PurchaseOrderRelatedDocumentDto> RelatedDocuments { get; set; } = [];
    public IReadOnlyCollection<PurchaseOrderAttachmentDto> Attachments { get; set; } = [];
    public IReadOnlyCollection<PurchaseOrderSapSyncLogDto> SapLogs { get; set; } = [];
}

public sealed class PurchaseOrderSummaryDto
{
    public int Id { get; set; }
    public string SeriesCode { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public string SupplierCode { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public DateTime DocumentDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string Status { get; set; } = PurchaseOrderStatuses.Draft;
    public decimal TotalAmount { get; set; }
    public string SapStatus { get; set; } = PurchaseOrderSapStatuses.Pending;
}

public sealed class PurchaseOrderLineDto
{
    public int Id { get; set; }
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
    public DateTime DeliveryDate { get; set; }
    public int? CostCenterId { get; set; }
    public int? ProjectId { get; set; }
    public decimal LineSubtotal { get; set; }
    public decimal LineTotal { get; set; }
    public string Status { get; set; } = PurchaseOrderLineStatuses.Open;
}

public sealed class PurchaseOrderAddressDto
{
    public int Id { get; set; }
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

public sealed class PurchaseOrderApprovalDto
{
    public int Id { get; set; }
    public int ApprovalLevel { get; set; }
    public string? RoleName { get; set; }
    public string? UserName { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string Status { get; set; } = PurchaseOrderApprovalStatuses.Pending;
    public string? Observation { get; set; }
}

public sealed class PurchaseOrderRelatedDocumentDto
{
    public int Id { get; set; }
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

public sealed class PurchaseOrderAttachmentDto
{
    public int Id { get; set; }
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

public sealed class PurchaseOrderSapSyncLogDto
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Process { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? UserName { get; set; }
    public int AttemptNumber { get; set; }
}

public sealed record PurchaseOrderSaveRequest(
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
    IReadOnlyCollection<PurchaseOrderLineSaveRequest> Lines,
    IReadOnlyCollection<PurchaseOrderAddressSaveRequest> Addresses,
    IReadOnlyCollection<PurchaseOrderRelatedDocumentSaveRequest>? RelatedDocuments,
    IReadOnlyCollection<PurchaseOrderAttachmentSaveRequest>? Attachments);

public sealed record PurchaseOrderLineSaveRequest(
    int? Id,
    int LineNumber,
    int ItemId,
    string ItemCode,
    string ItemName,
    int? UnitId,
    string? UnitCode,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountPercent,
    int? TaxId,
    string TaxCode,
    decimal TaxRate,
    int WarehouseId,
    string WarehouseCode,
    DateTime DeliveryDate,
    int? CostCenterId,
    int? ProjectId);

public sealed record PurchaseOrderAddressSaveRequest(
    int? Id,
    string AddressType,
    int? SourceAddressId,
    string? AddressName,
    string? Street,
    string? Reference,
    string? City,
    string? State,
    string? ZipCode,
    string? Country,
    string? Phone,
    string? Email,
    bool IsModified);

public sealed record PurchaseOrderRelatedDocumentSaveRequest(
    int? Id,
    string RelatedDocumentType,
    int? RelatedDocumentId,
    string? Series,
    string? Number,
    DateTime? Date,
    string? Status,
    string? Reference,
    string? Comment,
    decimal Total);

public sealed record PurchaseOrderAttachmentSaveRequest(
    int? Id,
    string FileName,
    string OriginalFileName,
    string? FileExtension,
    string? MimeType,
    long FileSize,
    string? StoragePath,
    string Status,
    string? Comment);

public sealed record PurchaseOrderLookupsDto(
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> Suppliers,
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> Items,
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> Units,
    IReadOnlyCollection<PurchaseOrderWarehouseLookupDto> Warehouses,
    IReadOnlyCollection<PurchaseOrderTaxLookupDto> Taxes,
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> Currencies,
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> PaymentTerms,
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> PriceLists,
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> Buyers,
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> CostCenters,
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> Projects,
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> PurchaseTypes,
    IReadOnlyCollection<PurchaseOrderLookupOptionDto> DocumentSeries);

public sealed record PurchaseOrderFieldAccessDto(
    string FieldKey,
    string ControlType,
    bool IsVisible,
    bool IsEditable,
    bool IsRequired,
    bool IsReadOnly);

public sealed record PurchaseOrderLookupOptionDto(
    int Id,
    string Code,
    string Name,
    bool IsActive = true);

public sealed record PurchaseOrderWarehouseLookupDto(
    int Id,
    string Code,
    string Name,
    bool IsActive = true);

public sealed record PurchaseOrderTaxLookupDto(
    int Id,
    string Code,
    string Name,
    decimal Rate,
    bool IsActive = true);

public static class PurchaseOrderStatuses
{
    public const string Draft = "Draft";
    public const string PendingApproval = "PendingApproval";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string SapPending = "SapPending";
    public const string SapSynced = "SapSynced";
    public const string SapError = "SapError";
    public const string Closed = "Closed";
    public const string Cancelled = "Cancelled";
}

public static class PurchaseOrderLineStatuses
{
    public const string Open = "Open";
    public const string Closed = "Closed";
    public const string Cancelled = "Cancelled";
}

public static class PurchaseOrderApprovalStatuses
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string Cancelled = "Cancelled";
}

public static class PurchaseOrderSapStatuses
{
    public const string Pending = "Pending";
    public const string Synced = "Synced";
    public const string Error = "Error";
    public const string Cancelled = "Cancelled";
}

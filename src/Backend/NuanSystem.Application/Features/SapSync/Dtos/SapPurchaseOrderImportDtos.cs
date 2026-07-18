namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapPurchaseOrderRecord(
    int DocEntry, int DocNum, DateTime DocumentDate, DateTime DeliveryDate,
    string SupplierCode, string SupplierName, string CurrencyCode, decimal ExchangeRate,
    decimal DocumentTotal, decimal TaxTotal, decimal DiscountPercent,
    string Status, bool Cancelled, DateTime UpdatedAt,
    string? Comments, IReadOnlyCollection<SapPurchaseOrderLineRecord> Lines);

public sealed record SapPurchaseOrderLineRecord(
    int LineNumber, string ItemCode, string ItemName, decimal Quantity, decimal OpenQuantity,
    decimal UnitPrice, decimal DiscountPercent, string TaxCode, decimal TaxRate,
    string? UnitCode, string WarehouseCode, DateTime DeliveryDate, string Status);

public sealed record SapPurchaseOrderImportItemResultDto(int DocEntry, int DocNum, string Status, string Message, int? LocalId);

public sealed record SapPurchaseOrderImportResultDto(
    int TotalRead, int Created, int Updated, int Unchanged, int Skipped, int Failed,
    IReadOnlyCollection<SapPurchaseOrderImportItemResultDto> Items);

public sealed record SapPurchaseOrderImportData(
    Guid GlobalId, SapPurchaseOrderRecord Document, long SapVersion, int? AuditUserId, string? AuditUserName);

public sealed record SapPurchaseOrderImportApplyResult(string Status, int? PurchaseOrderId, string Message);

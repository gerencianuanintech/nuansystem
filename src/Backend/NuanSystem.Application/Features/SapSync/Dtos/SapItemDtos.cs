namespace NuanSystem.Application.Features.SapSync.Dtos;

public sealed record SapItemRecord(
    string ItemCode,
    string ItemName,
    int? ItemGroupCode,
    string? InventoryUnitCode,
    string? PurchaseUnitCode,
    string? SalesUnitCode,
    string? Barcode,
    string? PurchaseTaxCode,
    string? SalesTaxCode,
    bool IsPurchaseItem,
    bool IsSalesItem,
    bool IsInventoryItem,
    bool ManageSerialNumbers,
    bool ManageBatchNumbers,
    string ItemType,
    bool IsActive);

public sealed record SapItemReadOptions(
    int? MaxRecords = null,
    string? Search = null,
    IReadOnlyCollection<string>? ItemCodes = null);

public sealed record SapItemPreviewItemDto(
    string SapItemCode,
    string SapItemName,
    int? SapItemGroupCode,
    string? InventoryUnitCode,
    bool IsActiveInSap,
    string Status,
    string StatusName,
    int? LocalItemId,
    string? LocalCode,
    string? LocalName,
    string? DifferenceSummary);

public sealed record SapItemImportResultDto(
    int TotalRead,
    int Selected,
    int Created,
    int Updated,
    int Unchanged,
    int Skipped,
    int Failed,
    bool DetailsTruncated,
    IReadOnlyCollection<SapItemImportItemResultDto> Items);

public sealed record SapItemImportItemResultDto(
    string SapItemCode,
    string SapItemName,
    string Status,
    string Message,
    int? LocalItemId);

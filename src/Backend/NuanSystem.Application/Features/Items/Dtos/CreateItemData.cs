namespace NuanSystem.Application.Features.Items.Dtos;

public sealed record SaveItemBarcodeData(
    string Barcode,
    int? UnitOfMeasureId,
    string BarcodeType,
    decimal ConversionFactor,
    bool IsMain,
    bool IsActive);

public sealed record SaveItemWarehouseData(
    int WarehouseId,
    decimal MinimumStock,
    decimal MaximumStock,
    decimal RequiredStock,
    decimal ReorderPoint,
    string? DefaultLocationCode,
    decimal WarehouseCost,
    bool IsDefaultWarehouse,
    bool IsLocked,
    bool IsActive);

public sealed record CreateItemData(
    string Code,
    string Name,
    string? Description,
    int? ItemGroupId,
    int? ItemFamilyId,
    string ItemType,
    int? InventoryUnitOfMeasureId,
    int? PurchaseUnitOfMeasureId,
    int? SalesUnitOfMeasureId,
    bool IsPurchaseItem,
    bool IsSalesItem,
    bool IsInventoryItem,
    int? PurchaseTaxId,
    int? SalesTaxId,
    string ValuationMethod,
    string ManagedBy,
    string BatchSerialManagementMethod,
    string? PreferredVendorCode,
    string? VendorCatalogCode,
    decimal BaseSalesPrice,
    decimal ReferenceCost,
    decimal PurchaseFactor,
    decimal SalesFactor,
    bool AllowDiscount,
    bool AllowSaleWithoutStock,
    string? Remarks,
    bool IsActive,
    IReadOnlyCollection<SaveItemBarcodeData> Barcodes,
    IReadOnlyCollection<SaveItemWarehouseData> Warehouses,
    int? CreatedByUserId,
    string? CreatedByUserName);

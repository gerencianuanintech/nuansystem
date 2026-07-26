namespace NuanSystem.Application.Features.Items.Dtos;

public sealed class ItemDto
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public string? SapCode { get; set; }
    public string? Description { get; set; }
    public int? ItemGroupId { get; set; }
    public Guid? ItemGroupGlobalId { get; set; }
    public string? ItemGroupCode { get; set; }
    public string? ItemGroupName { get; set; }
    public int? ItemFamilyId { get; set; }
    public Guid? ItemFamilyGlobalId { get; set; }
    public string? ItemFamilyCode { get; set; }
    public string? ItemFamilyName { get; set; }
    public string ItemType { get; set; } = "Product";
    public int? InventoryUnitOfMeasureId { get; set; }
    public Guid? InventoryUnitOfMeasureGlobalId { get; set; }
    public string? InventoryUnitOfMeasureCode { get; set; }
    public string? InventoryUnitOfMeasureName { get; set; }
    public int? PurchaseUnitOfMeasureId { get; set; }
    public Guid? PurchaseUnitOfMeasureGlobalId { get; set; }
    public string? PurchaseUnitOfMeasureCode { get; set; }
    public string? PurchaseUnitOfMeasureName { get; set; }
    public int? SalesUnitOfMeasureId { get; set; }
    public Guid? SalesUnitOfMeasureGlobalId { get; set; }
    public string? SalesUnitOfMeasureCode { get; set; }
    public string? SalesUnitOfMeasureName { get; set; }
    public bool IsPurchaseItem { get; set; }
    public bool IsSalesItem { get; set; }
    public bool IsInventoryItem { get; set; }
    public int? PurchaseTaxId { get; set; }
    public string? PurchaseTaxCode { get; set; }
    public string? PurchaseTaxName { get; set; }
    public int? SalesTaxId { get; set; }
    public string? SalesTaxCode { get; set; }
    public string? SalesTaxName { get; set; }
    public string ValuationMethod { get; set; } = "MovingAverage";
    public string ManagedBy { get; set; } = "None";
    public string BatchSerialManagementMethod { get; set; } = "EveryTransaction";
    public string? PreferredVendorCode { get; set; }
    public string? VendorCatalogCode { get; set; }
    public decimal BaseSalesPrice { get; set; }
    public decimal ReferenceCost { get; set; }
    public decimal PurchaseFactor { get; set; } = 1;
    public decimal SalesFactor { get; set; } = 1;
    public bool AllowDiscount { get; set; }
    public bool AllowSaleWithoutStock { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public string? DeletedByUserName { get; set; }
    public DateTime? DeletedAt { get; set; }
    public IReadOnlyCollection<ItemBarcodeDto> Barcodes { get; set; } = [];
    public IReadOnlyCollection<ItemWarehouseDto> Warehouses { get; set; } = [];
    public ItemMasterData? MasterData { get; set; }
}

public sealed record ItemSyncPayload(
    Guid GlobalId,
    string Code,
    string Name,
    string? Description,
    string ItemType,
    Guid? ItemGroupGlobalId,
    string? ItemGroupCode,
    Guid? ItemFamilyGlobalId,
    string? ItemFamilyCode,
    Guid? InventoryUnitOfMeasureGlobalId,
    string? InventoryUnitOfMeasureCode,
    Guid? PurchaseUnitOfMeasureGlobalId,
    string? PurchaseUnitOfMeasureCode,
    Guid? SalesUnitOfMeasureGlobalId,
    string? SalesUnitOfMeasureCode,
    string? Barcode,
    bool IsInventoryItem,
    bool IsSalesItem,
    bool IsPurchaseItem,
    bool IsActive,
    string? ExternalSystem,
    string? ExternalCode,
    string? SapCode);

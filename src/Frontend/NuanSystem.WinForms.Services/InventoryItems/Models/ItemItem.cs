namespace NuanSystem.WinForms.Services.InventoryItems.Models;

public sealed class ItemItem
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ItemGroupId { get; set; }
    public string? ItemGroupCode { get; set; }
    public string? ItemGroupName { get; set; }
    public int? ItemFamilyId { get; set; }
    public string? ItemFamilyCode { get; set; }
    public string? ItemFamilyName { get; set; }
    public string ItemType { get; set; } = "Product";
    public int? InventoryUnitOfMeasureId { get; set; }
    public string? InventoryUnitOfMeasureCode { get; set; }
    public int? PurchaseUnitOfMeasureId { get; set; }
    public string? PurchaseUnitOfMeasureCode { get; set; }
    public int? SalesUnitOfMeasureId { get; set; }
    public string? SalesUnitOfMeasureCode { get; set; }
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
    public ItemMasterData? MasterData { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public string? DeletedByUserName { get; set; }
    public DateTime? DeletedAt { get; set; }
    public IReadOnlyCollection<ItemBarcodeItem> Barcodes { get; set; } = [];
    public IReadOnlyCollection<ItemWarehouseItem> Warehouses { get; set; } = [];
}

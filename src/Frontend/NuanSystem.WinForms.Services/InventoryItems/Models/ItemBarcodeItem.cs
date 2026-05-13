namespace NuanSystem.WinForms.Services.InventoryItems.Models;

public sealed class ItemBarcodeItem
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public int? UnitOfMeasureId { get; set; }
    public string BarcodeType { get; set; } = "Internal";
    public decimal ConversionFactor { get; set; } = 1;
    public bool IsMain { get; set; }
    public bool IsActive { get; set; } = true;
}

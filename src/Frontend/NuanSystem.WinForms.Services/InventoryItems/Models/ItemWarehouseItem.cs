namespace NuanSystem.WinForms.Services.InventoryItems.Models;

public sealed class ItemWarehouseItem
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int WarehouseId { get; set; }
    public string? WarehouseCode { get; set; }
    public string? WarehouseName { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal MaximumStock { get; set; }
    public decimal RequiredStock { get; set; }
    public decimal ReorderPoint { get; set; }
    public string? DefaultLocationCode { get; set; }
    public decimal WarehouseCost { get; set; }
    public bool IsDefaultWarehouse { get; set; }
    public bool IsLocked { get; set; }
    public bool IsActive { get; set; } = true;
}

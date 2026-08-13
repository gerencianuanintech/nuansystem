namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;

public sealed class ItemGroupItem
{
    public int Id { get; set; }
    public Guid? GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? InventoryAccountCode { get; set; }
    public string? IncomeAccountCode { get; set; }
    public string? CostOfSalesAccountCode { get; set; }
    public string? SalesReturnAccountCode { get; set; }
    public string? PurchaseReturnAccountCode { get; set; }
    public string? CostVarianceAccountCode { get; set; }
    public string? InventoryAdjustmentAccountCode { get; set; }
    public string? PurchaseExpenseAccountCode { get; set; }
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public string? ExternalSystem { get; set; }
    public string? ExternalCode { get; set; }
    public string? SapGroupCode { get; set; }
    public string? SapCode { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

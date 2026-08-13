namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes.Models;

public sealed class ItemTypeItem
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string BehaviorCode { get; set; } = string.Empty;
    public bool DefaultIsPurchaseItem { get; set; }
    public bool DefaultIsSalesItem { get; set; }
    public bool DefaultIsInventoryItem { get; set; }
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

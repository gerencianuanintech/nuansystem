namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes.Models;

public sealed class ItemTypeLookupItem
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string BehaviorCode { get; set; } = string.Empty;
    public bool DefaultIsPurchaseItem { get; set; }
    public bool DefaultIsSalesItem { get; set; }
    public bool DefaultIsInventoryItem { get; set; }
    public int SortOrder { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public string DisplayText => string.IsNullOrWhiteSpace(Code) ? Name : $"{Code} - {Name}";
}

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;

public sealed class ItemFamilyLookupItem
{
    public int Id { get; set; }
    public int ItemGroupId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayText => $"{Code} - {Name}";
}

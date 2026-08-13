namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands.Models;

public sealed class ItemBrandLookupItem
{
    public int Id { get; set; }
    public Guid? GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string DisplayText => $"{Code} - {Name}";
}

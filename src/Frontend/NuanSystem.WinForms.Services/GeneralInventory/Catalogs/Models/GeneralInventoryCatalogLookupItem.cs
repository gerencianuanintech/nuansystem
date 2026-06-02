namespace NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

public sealed class GeneralInventoryCatalogLookupItem
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string DisplayText => string.IsNullOrWhiteSpace(Code) ? Name : $"{Code} - {Name}";
}

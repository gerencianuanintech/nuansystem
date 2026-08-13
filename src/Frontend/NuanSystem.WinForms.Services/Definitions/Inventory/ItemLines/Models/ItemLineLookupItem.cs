namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines.Models;

public sealed class ItemLineLookupItem
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public string DisplayText => string.IsNullOrWhiteSpace(Code) ? Name : $"{Code} - {Name}";
}

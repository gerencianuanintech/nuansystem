namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;

public sealed record ItemFamilyGroupLookupItem(int Id, string Code, string Name, bool IsActive)
{
    public string DisplayText => $"{Code} - {Name}";
}

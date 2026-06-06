namespace NuanSystem.WinForms.Services.OperationalCatalogs.Models;

public sealed class OperationalCatalogLookupItem
{
    public int Id { get; set; }
    public string CatalogKey { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ParentCatalogKey { get; set; }
    public string? ParentCode { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public string DisplayText => $"{Code} - {Name}";
}

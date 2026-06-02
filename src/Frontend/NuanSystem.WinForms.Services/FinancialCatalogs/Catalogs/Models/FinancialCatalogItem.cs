namespace NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs.Models;

public sealed class FinancialCatalogItem
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}

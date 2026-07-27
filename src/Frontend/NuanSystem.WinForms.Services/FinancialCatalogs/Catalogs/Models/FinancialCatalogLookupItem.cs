namespace NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs.Models;

public sealed class FinancialCatalogLookupItem
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string DisplayName => $"{Code} - {Name}";
}

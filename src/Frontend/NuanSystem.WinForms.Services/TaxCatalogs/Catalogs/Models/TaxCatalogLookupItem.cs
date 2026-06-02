namespace NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;

public sealed record TaxCatalogLookupItem(int Id, string Code, string Name, bool IsActive = true);

public sealed record RetentionConceptLookupItem(
    int Id,
    string Code,
    string Name,
    bool IsActive,
    string? SriCode,
    decimal Percent,
    bool AppliesIva,
    bool AppliesIncome);

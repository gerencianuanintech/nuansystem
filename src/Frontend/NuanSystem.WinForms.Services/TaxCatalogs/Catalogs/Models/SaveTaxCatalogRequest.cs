namespace NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;

public sealed record SaveTaxCatalogRequest(
    string Code,
    string Name,
    string? Description,
    bool IsActive);

public sealed record SaveRetentionConceptRequest(
    string Code,
    string Name,
    string? Description,
    int? RetentionTypeId,
    string? SriCode,
    decimal Percent,
    bool AppliesIva,
    bool AppliesIncome,
    bool IsActive);

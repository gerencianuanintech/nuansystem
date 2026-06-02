namespace NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs.Models;

public sealed record SaveFinancialCatalogRequest(
    string Code,
    string Name,
    string? Description,
    bool IsActive);

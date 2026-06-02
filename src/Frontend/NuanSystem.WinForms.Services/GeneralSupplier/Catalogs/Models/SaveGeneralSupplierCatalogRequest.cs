namespace NuanSystem.WinForms.Services.GeneralSupplier.Catalogs.Models;

public sealed record SaveGeneralSupplierCatalogRequest(
    string Code,
    string Name,
    string? Description,
    bool IsActive);


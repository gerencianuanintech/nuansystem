using NuanSystem.WinForms.Services.GeneralSupplier.Catalogs.Models;

namespace NuanSystem.WinForms.Services.GeneralSupplier.Catalogs;

public interface IGeneralSupplierCatalogClient
{
    Task<IReadOnlyCollection<GeneralSupplierCatalogItem>> GetAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GeneralSupplierCatalogLookupItem>> GetLookupAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default);

    Task<GeneralSupplierCatalogItem> GetByIdAsync(
        string catalogRoute,
        int id,
        CancellationToken cancellationToken = default);

    Task<GeneralSupplierCatalogItem> CreateAsync(
        string catalogRoute,
        SaveGeneralSupplierCatalogRequest request,
        CancellationToken cancellationToken = default);

    Task<GeneralSupplierCatalogItem> UpdateAsync(
        string catalogRoute,
        int id,
        SaveGeneralSupplierCatalogRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string catalogRoute,
        int id,
        CancellationToken cancellationToken = default);
}


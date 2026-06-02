using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;

namespace NuanSystem.WinForms.Services.GeneralInventory.Catalogs;

public interface IGeneralInventoryCatalogClient
{
    Task<IReadOnlyCollection<GeneralInventoryCatalogItem>> GetAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GeneralInventoryCatalogLookupItem>> GetLookupAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default);

    Task<GeneralInventoryCatalogItem> GetByIdAsync(
        string catalogRoute,
        int id,
        CancellationToken cancellationToken = default);

    Task<GeneralInventoryCatalogItem> CreateAsync(
        string catalogRoute,
        SaveGeneralInventoryCatalogRequest request,
        CancellationToken cancellationToken = default);

    Task<GeneralInventoryCatalogItem> UpdateAsync(
        string catalogRoute,
        int id,
        SaveGeneralInventoryCatalogRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string catalogRoute,
        int id,
        CancellationToken cancellationToken = default);
}

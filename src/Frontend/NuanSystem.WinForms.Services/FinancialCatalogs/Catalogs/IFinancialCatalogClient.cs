using NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs.Models;

namespace NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs;

public interface IFinancialCatalogClient
{
    Task<IReadOnlyCollection<FinancialCatalogItem>> GetAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FinancialCatalogLookupItem>> GetLookupAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default);

    Task<FinancialCatalogItem> GetByIdAsync(
        string catalogRoute,
        int id,
        CancellationToken cancellationToken = default);

    Task<FinancialCatalogItem> CreateAsync(
        string catalogRoute,
        SaveFinancialCatalogRequest request,
        CancellationToken cancellationToken = default);

    Task<FinancialCatalogItem> UpdateAsync(
        string catalogRoute,
        int id,
        SaveFinancialCatalogRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        string catalogRoute,
        int id,
        CancellationToken cancellationToken = default);
}

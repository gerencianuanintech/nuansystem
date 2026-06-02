using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;

namespace NuanSystem.WinForms.Services.TaxCatalogs.Catalogs;

public interface ITaxCatalogClient
{
    Task<IReadOnlyCollection<TaxCatalogItem>> GetAsync(string catalogRoute, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<TaxCatalogLookupItem>> GetLookupAsync(string catalogRoute, CancellationToken cancellationToken = default);

    Task<TaxCatalogItem> GetByIdAsync(string catalogRoute, int id, CancellationToken cancellationToken = default);

    Task<TaxCatalogItem> CreateAsync(string catalogRoute, SaveTaxCatalogRequest request, CancellationToken cancellationToken = default);

    Task<TaxCatalogItem> UpdateAsync(string catalogRoute, int id, SaveTaxCatalogRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(string catalogRoute, int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RetentionConceptItem>> GetRetentionConceptsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<RetentionConceptLookupItem>> GetRetentionConceptLookupAsync(CancellationToken cancellationToken = default);

    Task<RetentionConceptItem> GetRetentionConceptByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<RetentionConceptItem> CreateRetentionConceptAsync(SaveRetentionConceptRequest request, CancellationToken cancellationToken = default);

    Task<RetentionConceptItem> UpdateRetentionConceptAsync(int id, SaveRetentionConceptRequest request, CancellationToken cancellationToken = default);
}

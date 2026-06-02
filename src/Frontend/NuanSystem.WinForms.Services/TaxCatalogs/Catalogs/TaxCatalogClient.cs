using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;

namespace NuanSystem.WinForms.Services.TaxCatalogs.Catalogs;

public sealed class TaxCatalogClient : ITaxCatalogClient
{
    private readonly INuanApiClient apiClient;

    public TaxCatalogClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<TaxCatalogItem>> GetAsync(string catalogRoute, CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<TaxCatalogItem>>(BuildRoute(catalogRoute), cancellationToken);
    }

    public async Task<IReadOnlyCollection<TaxCatalogLookupItem>> GetLookupAsync(string catalogRoute, CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<TaxCatalogLookupItem>>($"{BuildRoute(catalogRoute)}/lookup", cancellationToken);
    }

    public Task<TaxCatalogItem> GetByIdAsync(string catalogRoute, int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<TaxCatalogItem>($"{BuildRoute(catalogRoute)}/{id}", cancellationToken);
    }

    public Task<TaxCatalogItem> CreateAsync(string catalogRoute, SaveTaxCatalogRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveTaxCatalogRequest, TaxCatalogItem>(BuildRoute(catalogRoute), request, cancellationToken);
    }

    public Task<TaxCatalogItem> UpdateAsync(string catalogRoute, int id, SaveTaxCatalogRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveTaxCatalogRequest, TaxCatalogItem>($"{BuildRoute(catalogRoute)}/{id}", request, cancellationToken);
    }

    public async Task DeleteAsync(string catalogRoute, int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"{BuildRoute(catalogRoute)}/{id}", cancellationToken);
    }

    public async Task<IReadOnlyCollection<RetentionConceptItem>> GetRetentionConceptsAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<RetentionConceptItem>>(BuildRoute(TaxCatalogRoutes.RetentionConcepts), cancellationToken);
    }

    public async Task<IReadOnlyCollection<RetentionConceptLookupItem>> GetRetentionConceptLookupAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<RetentionConceptLookupItem>>($"{BuildRoute(TaxCatalogRoutes.RetentionConcepts)}/lookup", cancellationToken);
    }

    public Task<RetentionConceptItem> GetRetentionConceptByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<RetentionConceptItem>($"{BuildRoute(TaxCatalogRoutes.RetentionConcepts)}/{id}", cancellationToken);
    }

    public Task<RetentionConceptItem> CreateRetentionConceptAsync(SaveRetentionConceptRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveRetentionConceptRequest, RetentionConceptItem>(BuildRoute(TaxCatalogRoutes.RetentionConcepts), request, cancellationToken);
    }

    public Task<RetentionConceptItem> UpdateRetentionConceptAsync(int id, SaveRetentionConceptRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveRetentionConceptRequest, RetentionConceptItem>($"{BuildRoute(TaxCatalogRoutes.RetentionConcepts)}/{id}", request, cancellationToken);
    }

    private static string BuildRoute(string catalogRoute)
    {
        return $"/api/tax-catalogs/{catalogRoute}";
    }
}

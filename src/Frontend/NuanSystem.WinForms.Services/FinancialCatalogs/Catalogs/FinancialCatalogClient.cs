using NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs;

public sealed class FinancialCatalogClient : IFinancialCatalogClient
{
    private readonly INuanApiClient apiClient;

    public FinancialCatalogClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<FinancialCatalogItem>> GetAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<FinancialCatalogItem>>(
            BuildRoute(catalogRoute),
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<FinancialCatalogLookupItem>> GetLookupAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<FinancialCatalogLookupItem>>(
            $"{BuildRoute(catalogRoute)}/lookup",
            cancellationToken);
    }

    public Task<FinancialCatalogItem> GetByIdAsync(
        string catalogRoute,
        int id,
        CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<FinancialCatalogItem>(
            $"{BuildRoute(catalogRoute)}/{id}",
            cancellationToken);
    }

    public Task<FinancialCatalogItem> CreateAsync(
        string catalogRoute,
        SaveFinancialCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveFinancialCatalogRequest, FinancialCatalogItem>(
            BuildRoute(catalogRoute),
            request,
            cancellationToken);
    }

    public Task<FinancialCatalogItem> UpdateAsync(
        string catalogRoute,
        int id,
        SaveFinancialCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveFinancialCatalogRequest, FinancialCatalogItem>(
            $"{BuildRoute(catalogRoute)}/{id}",
            request,
            cancellationToken);
    }

    public async Task DeleteAsync(
        string catalogRoute,
        int id,
        CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"{BuildRoute(catalogRoute)}/{id}", cancellationToken);
    }

    private static string BuildRoute(string catalogRoute)
    {
        return $"/api/financial-catalogs/{catalogRoute}";
    }
}

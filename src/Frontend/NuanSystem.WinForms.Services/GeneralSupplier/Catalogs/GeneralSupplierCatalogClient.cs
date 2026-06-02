using NuanSystem.WinForms.Services.GeneralSupplier.Catalogs.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.GeneralSupplier.Catalogs;

public sealed class GeneralSupplierCatalogClient : IGeneralSupplierCatalogClient
{
    private readonly INuanApiClient apiClient;

    public GeneralSupplierCatalogClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<GeneralSupplierCatalogItem>> GetAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<GeneralSupplierCatalogItem>>(
            BuildRoute(catalogRoute),
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<GeneralSupplierCatalogLookupItem>> GetLookupAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<GeneralSupplierCatalogLookupItem>>(
            $"{BuildRoute(catalogRoute)}/lookup",
            cancellationToken);
    }

    public Task<GeneralSupplierCatalogItem> GetByIdAsync(
        string catalogRoute,
        int id,
        CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<GeneralSupplierCatalogItem>(
            $"{BuildRoute(catalogRoute)}/{id}",
            cancellationToken);
    }

    public Task<GeneralSupplierCatalogItem> CreateAsync(
        string catalogRoute,
        SaveGeneralSupplierCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveGeneralSupplierCatalogRequest, GeneralSupplierCatalogItem>(
            BuildRoute(catalogRoute),
            request,
            cancellationToken);
    }

    public Task<GeneralSupplierCatalogItem> UpdateAsync(
        string catalogRoute,
        int id,
        SaveGeneralSupplierCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveGeneralSupplierCatalogRequest, GeneralSupplierCatalogItem>(
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
        return $"/api/general-supplier/{catalogRoute}";
    }
}


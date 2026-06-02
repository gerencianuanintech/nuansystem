using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.GeneralInventory.Catalogs;

public sealed class GeneralInventoryCatalogClient : IGeneralInventoryCatalogClient
{
    private readonly INuanApiClient apiClient;

    public GeneralInventoryCatalogClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<GeneralInventoryCatalogItem>> GetAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<GeneralInventoryCatalogItem>>(
            BuildRoute(catalogRoute),
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<GeneralInventoryCatalogLookupItem>> GetLookupAsync(
        string catalogRoute,
        CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<GeneralInventoryCatalogLookupItem>>(
            $"{BuildRoute(catalogRoute)}/lookup",
            cancellationToken);
    }

    public Task<GeneralInventoryCatalogItem> GetByIdAsync(
        string catalogRoute,
        int id,
        CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<GeneralInventoryCatalogItem>(
            $"{BuildRoute(catalogRoute)}/{id}",
            cancellationToken);
    }

    public Task<GeneralInventoryCatalogItem> CreateAsync(
        string catalogRoute,
        SaveGeneralInventoryCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveGeneralInventoryCatalogRequest, GeneralInventoryCatalogItem>(
            BuildRoute(catalogRoute),
            request,
            cancellationToken);
    }

    public Task<GeneralInventoryCatalogItem> UpdateAsync(
        string catalogRoute,
        int id,
        SaveGeneralInventoryCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveGeneralInventoryCatalogRequest, GeneralInventoryCatalogItem>(
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
        return $"/api/general-inventory/{catalogRoute}";
    }
}

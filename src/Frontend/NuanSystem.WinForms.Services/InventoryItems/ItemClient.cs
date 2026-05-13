using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.InventoryItems.Models;

namespace NuanSystem.WinForms.Services.InventoryItems;

public sealed class ItemClient : IItemClient
{
    private readonly INuanApiClient apiClient;

    public ItemClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<ItemItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<ItemItem>>("/api/items", cancellationToken);
    }

    public Task<ItemItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<ItemItem>($"/api/items/{id}", cancellationToken);
    }

    public Task<ItemLookups> GetLookupsAsync(CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<ItemLookups>("/api/items/lookups", cancellationToken);
    }

    public Task<ItemItem> CreateAsync(SaveItemRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveItemRequest, ItemItem>("/api/items", request, cancellationToken);
    }

    public Task<ItemItem> UpdateAsync(int id, SaveItemRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveItemRequest, ItemItem>($"/api/items/{id}", request, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/items/{id}", cancellationToken);
    }
}

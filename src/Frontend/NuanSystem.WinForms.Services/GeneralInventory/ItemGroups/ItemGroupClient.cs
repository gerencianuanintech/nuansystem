using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.GeneralInventory.ItemGroups;

public sealed class ItemGroupClient : IItemGroupClient
{
    private readonly INuanApiClient apiClient;

    public ItemGroupClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<ItemGroupItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<ItemGroupItem>>("/api/item-groups", cancellationToken);
    }

    public Task<ItemGroupItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<ItemGroupItem>($"/api/item-groups/{id}", cancellationToken);
    }

    public Task<ItemGroupItem> CreateAsync(SaveItemGroupRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveItemGroupRequest, ItemGroupItem>("/api/item-groups", request, cancellationToken);
    }

    public Task<ItemGroupItem> UpdateAsync(int id, SaveItemGroupRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveItemGroupRequest, ItemGroupItem>($"/api/item-groups/{id}", request, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/item-groups/{id}", cancellationToken);
    }
}

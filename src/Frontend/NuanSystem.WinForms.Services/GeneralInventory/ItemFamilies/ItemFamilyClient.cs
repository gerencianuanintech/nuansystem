using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies;

public sealed class ItemFamilyClient(INuanApiClient apiClient) : IItemFamilyClient
{
    public async Task<IReadOnlyCollection<ItemFamilyItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<ItemFamilyItem>>("/api/item-families", cancellationToken);
    }

    public async Task<IReadOnlyCollection<ItemFamilyItem>> GetByGroupAsync(int itemGroupId, CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<ItemFamilyItem>>($"/api/item-families/by-group/{itemGroupId}", cancellationToken);
    }

    public Task<ItemFamilyItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<ItemFamilyItem>($"/api/item-families/{id}", cancellationToken);
    }

    public Task<ItemFamilyItem> CreateAsync(SaveItemFamilyRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveItemFamilyRequest, ItemFamilyItem>("/api/item-families", request, cancellationToken);
    }

    public Task<ItemFamilyItem> UpdateAsync(int id, SaveItemFamilyRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveItemFamilyRequest, ItemFamilyItem>($"/api/item-families/{id}", request, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.DeleteAsync<object>($"/api/item-families/{id}", cancellationToken);
    }
}

using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies;

public sealed class ItemFamilyClient(INuanApiClient apiClient) : IItemFamilyClient
{
    private const string BaseRoute = "/api/definitions/inventory/item-families";

    public async Task<IReadOnlyCollection<ItemFamilyItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<ItemFamilyItem>>($"{BaseRoute}/lookup", cancellationToken);
    }

    public async Task<IReadOnlyCollection<ItemFamilyItem>> GetByGroupAsync(int itemGroupId, CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<ItemFamilyItem>>($"{BaseRoute}/lookup?itemGroupId={itemGroupId}", cancellationToken);
    }

    public Task<ItemFamilyItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<ItemFamilyItem>($"{BaseRoute}/{id}", cancellationToken);
    }

    public Task<ItemFamilyItem> CreateAsync(SaveItemFamilyRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveItemFamilyRequest, ItemFamilyItem>(BaseRoute, request, cancellationToken);
    }

    public Task<ItemFamilyItem> UpdateAsync(int id, SaveItemFamilyRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveItemFamilyRequest, ItemFamilyItem>($"{BaseRoute}/{id}", request, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.DeleteAsync<object>($"{BaseRoute}/{id}", cancellationToken);
    }
}

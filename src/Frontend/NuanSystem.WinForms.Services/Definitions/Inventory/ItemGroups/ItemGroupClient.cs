using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups;

public sealed class ItemGroupClient(INuanApiClient apiClient) : IItemGroupClient
{
    private const string BaseRoute = "/api/definitions/inventory/item-groups";

    public async Task<IReadOnlyCollection<ItemGroupItem>> GetAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemGroupItem>>(BaseRoute, cancellationToken);

    public async Task<IReadOnlyCollection<ItemGroupLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemGroupLookupItem>>($"{BaseRoute}/lookup", cancellationToken);

    public Task<ItemGroupItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<ItemGroupItem>($"{BaseRoute}/{id}", cancellationToken);

    public Task<ItemGroupItem> CreateAsync(SaveItemGroupRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SaveItemGroupRequest, ItemGroupItem>(BaseRoute, request, cancellationToken);

    public Task<ItemGroupItem> UpdateAsync(int id, SaveItemGroupRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync<SaveItemGroupRequest, ItemGroupItem>($"{BaseRoute}/{id}", request, cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.DeleteAsync<object>($"{BaseRoute}/{id}", cancellationToken);
}

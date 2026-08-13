using NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands;

public sealed class ItemBrandClient(INuanApiClient apiClient) : IItemBrandClient
{
    private const string Route = "/api/definitions/inventory/item-brands";

    public async Task<IReadOnlyCollection<ItemBrandItem>> GetAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemBrandItem>>(Route, cancellationToken);

    public async Task<IReadOnlyCollection<ItemBrandLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemBrandLookupItem>>($"{Route}/lookup", cancellationToken);

    public Task<ItemBrandItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<ItemBrandItem>($"{Route}/{id}", cancellationToken);

    public async Task<IReadOnlyCollection<ItemBrandAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemBrandAuditChange>>($"{Route}/{id}/history", cancellationToken);

    public Task<ItemBrandItem> CreateAsync(SaveItemBrandRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SaveItemBrandRequest, ItemBrandItem>(Route, request, cancellationToken);

    public Task<ItemBrandItem> UpdateAsync(int id, SaveItemBrandRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync<SaveItemBrandRequest, ItemBrandItem>($"{Route}/{id}", request, cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.DeleteAsync<object>($"{Route}/{id}", cancellationToken);
}

using NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies;

public sealed class ItemFamilyClient(INuanApiClient apiClient) : IItemFamilyClient
{
    private const string Route = "/api/definitions/inventory/item-families";

    public async Task<IReadOnlyCollection<ItemFamilyItem>> GetAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemFamilyItem>>(Route, cancellationToken);

    public async Task<IReadOnlyCollection<ItemFamilyLookupItem>> GetLookupAsync(int? itemGroupId = null, CancellationToken cancellationToken = default)
    {
        var route = itemGroupId.HasValue ? $"{Route}/lookup?itemGroupId={itemGroupId.Value}" : $"{Route}/lookup";
        return await apiClient.GetAsync<List<ItemFamilyLookupItem>>(route, cancellationToken);
    }

    public Task<ItemFamilyItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<ItemFamilyItem>($"{Route}/{id}", cancellationToken);

    public async Task<IReadOnlyCollection<ItemFamilyAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemFamilyAuditChange>>($"{Route}/{id}/history", cancellationToken);

    public Task<ItemFamilyItem> CreateAsync(SaveItemFamilyRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SaveItemFamilyRequest, ItemFamilyItem>(Route, request, cancellationToken);

    public Task<ItemFamilyItem> UpdateAsync(int id, SaveItemFamilyRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync<SaveItemFamilyRequest, ItemFamilyItem>($"{Route}/{id}", request, cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.DeleteAsync<object>($"{Route}/{id}", cancellationToken);
}

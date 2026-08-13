using NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes;

public sealed class ItemTypeClient : IItemTypeClient
{
    private const string Route = "/api/general-inventory/item-types";
    private readonly INuanApiClient apiClient;

    public ItemTypeClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<ItemTypeItem>> GetAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemTypeItem>>(Route, cancellationToken);

    public async Task<IReadOnlyCollection<ItemTypeLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemTypeLookupItem>>($"{Route}/lookup", cancellationToken);

    public Task<ItemTypeItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<ItemTypeItem>($"{Route}/{id}", cancellationToken);

    public async Task<IReadOnlyCollection<ItemTypeAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemTypeAuditChange>>($"{Route}/{id}/history", cancellationToken);

    public Task<ItemTypeItem> CreateAsync(SaveItemTypeRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SaveItemTypeRequest, ItemTypeItem>(Route, request, cancellationToken);

    public Task<ItemTypeItem> UpdateAsync(int id, SaveItemTypeRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync<SaveItemTypeRequest, ItemTypeItem>($"{Route}/{id}", request, cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.DeleteAsync<object>($"{Route}/{id}", cancellationToken);
}

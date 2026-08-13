using NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines;

public sealed class ItemLineClient(INuanApiClient apiClient) : IItemLineClient
{
    private const string Route = "/api/definitions/inventory/item-lines";

    public async Task<IReadOnlyCollection<ItemLineItem>> GetAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemLineItem>>(Route, cancellationToken);

    public async Task<IReadOnlyCollection<ItemLineLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemLineLookupItem>>($"{Route}/lookup", cancellationToken);

    public Task<ItemLineItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<ItemLineItem>($"{Route}/{id}", cancellationToken);

    public async Task<IReadOnlyCollection<ItemLineAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<ItemLineAuditChange>>($"{Route}/{id}/history", cancellationToken);

    public Task<ItemLineItem> CreateAsync(SaveItemLineRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SaveItemLineRequest, ItemLineItem>(Route, request, cancellationToken);

    public Task<ItemLineItem> UpdateAsync(int id, SaveItemLineRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync<SaveItemLineRequest, ItemLineItem>($"{Route}/{id}", request, cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.DeleteAsync<object>($"{Route}/{id}", cancellationToken);
}

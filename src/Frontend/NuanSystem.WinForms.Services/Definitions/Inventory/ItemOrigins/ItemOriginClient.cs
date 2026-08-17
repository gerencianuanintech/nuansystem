using NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins;

public interface IItemOriginClient
{
    Task<IReadOnlyCollection<ItemOriginItem>> GetAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<ItemOriginLookupItem>> GetLookupAsync(CancellationToken ct = default);
    Task<ItemOriginItem> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyCollection<ItemOriginAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default);
    Task<ItemOriginItem> CreateAsync(SaveItemOriginRequest request, CancellationToken ct = default);
    Task<ItemOriginItem> UpdateAsync(int id, SaveItemOriginRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
public sealed class ItemOriginClient(INuanApiClient apiClient) : IItemOriginClient
{
    private const string Route = "/api/definitions/inventory/item-origins";
    public async Task<IReadOnlyCollection<ItemOriginItem>> GetAsync(CancellationToken ct = default) => await apiClient.GetAsync<List<ItemOriginItem>>(Route, ct);
    public async Task<IReadOnlyCollection<ItemOriginLookupItem>> GetLookupAsync(CancellationToken ct = default) =>
        await apiClient.GetAsync<List<ItemOriginLookupItem>>($"{Route}/lookup", ct);
    public Task<ItemOriginItem> GetByIdAsync(int id, CancellationToken ct = default) => apiClient.GetAsync<ItemOriginItem>($"{Route}/{id}", ct);
    public async Task<IReadOnlyCollection<ItemOriginAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) =>
        await apiClient.GetAsync<List<ItemOriginAuditChange>>($"{Route}/{id}/history", ct);
    public Task<ItemOriginItem> CreateAsync(SaveItemOriginRequest request, CancellationToken ct = default) => apiClient.PostAsync<SaveItemOriginRequest, ItemOriginItem>(Route, request, ct);
    public Task<ItemOriginItem> UpdateAsync(int id, SaveItemOriginRequest request, CancellationToken ct = default) => apiClient.PutAsync<SaveItemOriginRequest, ItemOriginItem>($"{Route}/{id}", request, ct);
    public async Task DeleteAsync(int id, CancellationToken ct = default) => await apiClient.DeleteAsync<object>($"{Route}/{id}", ct);
}

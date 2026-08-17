using NuanSystem.WinForms.Services.Definitions.Inventory.ItemAlertTypes.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemAlertTypes;

public interface IItemAlertTypeClient
{
    Task<IReadOnlyCollection<ItemAlertTypeItem>> GetAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<ItemAlertTypeItem>> GetLookupAsync(CancellationToken ct = default);
    Task<ItemAlertTypeItem> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyCollection<ItemAlertTypeAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default);
    Task<ItemAlertTypeItem> CreateAsync(SaveItemAlertTypeRequest request, CancellationToken ct = default);
    Task<ItemAlertTypeItem> UpdateAsync(int id, SaveItemAlertTypeRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
public sealed class ItemAlertTypeClient(INuanApiClient apiClient) : IItemAlertTypeClient
{
    private const string Route = "/api/definitions/inventory/item-alert-types";
    public async Task<IReadOnlyCollection<ItemAlertTypeItem>> GetAsync(CancellationToken ct = default) => await apiClient.GetAsync<List<ItemAlertTypeItem>>(Route, ct);
    public async Task<IReadOnlyCollection<ItemAlertTypeItem>> GetLookupAsync(CancellationToken ct = default) => await apiClient.GetAsync<List<ItemAlertTypeItem>>($"{Route}/lookup", ct);
    public Task<ItemAlertTypeItem> GetByIdAsync(int id, CancellationToken ct = default) => apiClient.GetAsync<ItemAlertTypeItem>($"{Route}/{id}", ct);
    public async Task<IReadOnlyCollection<ItemAlertTypeAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => await apiClient.GetAsync<List<ItemAlertTypeAuditChange>>($"{Route}/{id}/history", ct);
    public Task<ItemAlertTypeItem> CreateAsync(SaveItemAlertTypeRequest request, CancellationToken ct = default) => apiClient.PostAsync<SaveItemAlertTypeRequest, ItemAlertTypeItem>(Route, request, ct);
    public Task<ItemAlertTypeItem> UpdateAsync(int id, SaveItemAlertTypeRequest request, CancellationToken ct = default) => apiClient.PutAsync<SaveItemAlertTypeRequest, ItemAlertTypeItem>($"{Route}/{id}", request, ct);
    public async Task DeleteAsync(int id, CancellationToken ct = default) => await apiClient.DeleteAsync<object>($"{Route}/{id}", ct);
}


using NuanSystem.WinForms.Services.Definitions.Inventory.ItemSubgroups.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemSubgroups;

public interface IItemSubgroupClient
{
    Task<IReadOnlyCollection<ItemSubgroupItem>> GetAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<ItemSubgroupLookupItem>> GetLookupAsync(int? itemFamilyId = null, CancellationToken ct = default);
    Task<ItemSubgroupItem> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyCollection<ItemSubgroupAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default);
    Task<ItemSubgroupItem> CreateAsync(SaveItemSubgroupRequest request, CancellationToken ct = default);
    Task<ItemSubgroupItem> UpdateAsync(int id, SaveItemSubgroupRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
public sealed class ItemSubgroupClient(INuanApiClient apiClient) : IItemSubgroupClient
{
    private const string Route = "/api/definitions/inventory/item-subgroups";
    public async Task<IReadOnlyCollection<ItemSubgroupItem>> GetAsync(CancellationToken ct = default) => await apiClient.GetAsync<List<ItemSubgroupItem>>(Route, ct);
    public async Task<IReadOnlyCollection<ItemSubgroupLookupItem>> GetLookupAsync(int? itemFamilyId = null, CancellationToken ct = default)
    {
        var route = itemFamilyId.HasValue ? $"{Route}/lookup?itemFamilyId={itemFamilyId.Value}" : $"{Route}/lookup";
        return await apiClient.GetAsync<List<ItemSubgroupLookupItem>>(route, ct);
    }
    public Task<ItemSubgroupItem> GetByIdAsync(int id, CancellationToken ct = default) => apiClient.GetAsync<ItemSubgroupItem>($"{Route}/{id}", ct);
    public async Task<IReadOnlyCollection<ItemSubgroupAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) =>
        await apiClient.GetAsync<List<ItemSubgroupAuditChange>>($"{Route}/{id}/history", ct);
    public Task<ItemSubgroupItem> CreateAsync(SaveItemSubgroupRequest request, CancellationToken ct = default) => apiClient.PostAsync<SaveItemSubgroupRequest, ItemSubgroupItem>(Route, request, ct);
    public Task<ItemSubgroupItem> UpdateAsync(int id, SaveItemSubgroupRequest request, CancellationToken ct = default) => apiClient.PutAsync<SaveItemSubgroupRequest, ItemSubgroupItem>($"{Route}/{id}", request, ct);
    public async Task DeleteAsync(int id, CancellationToken ct = default) => await apiClient.DeleteAsync<object>($"{Route}/{id}", ct);
}

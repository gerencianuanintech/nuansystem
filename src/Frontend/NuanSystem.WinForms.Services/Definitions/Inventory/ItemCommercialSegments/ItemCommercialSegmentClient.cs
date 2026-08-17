using NuanSystem.WinForms.Services.Definitions.Inventory.ItemCommercialSegments.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ItemCommercialSegments;

public interface IItemCommercialSegmentClient
{
    Task<IReadOnlyCollection<ItemCommercialSegmentItem>> GetAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<ItemCommercialSegmentItem>> GetLookupAsync(CancellationToken ct = default);
    Task<ItemCommercialSegmentItem> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyCollection<ItemCommercialSegmentAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default);
    Task<ItemCommercialSegmentItem> CreateAsync(SaveItemCommercialSegmentRequest request, CancellationToken ct = default);
    Task<ItemCommercialSegmentItem> UpdateAsync(int id, SaveItemCommercialSegmentRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
public sealed class ItemCommercialSegmentClient(INuanApiClient apiClient) : IItemCommercialSegmentClient
{
    private const string Route = "/api/definitions/inventory/item-commercial-segments";
    public async Task<IReadOnlyCollection<ItemCommercialSegmentItem>> GetAsync(CancellationToken ct = default) => await apiClient.GetAsync<List<ItemCommercialSegmentItem>>(Route, ct);
    public async Task<IReadOnlyCollection<ItemCommercialSegmentItem>> GetLookupAsync(CancellationToken ct = default) => await apiClient.GetAsync<List<ItemCommercialSegmentItem>>($"{Route}/lookup", ct);
    public Task<ItemCommercialSegmentItem> GetByIdAsync(int id, CancellationToken ct = default) => apiClient.GetAsync<ItemCommercialSegmentItem>($"{Route}/{id}", ct);
    public async Task<IReadOnlyCollection<ItemCommercialSegmentAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => await apiClient.GetAsync<List<ItemCommercialSegmentAuditChange>>($"{Route}/{id}/history", ct);
    public Task<ItemCommercialSegmentItem> CreateAsync(SaveItemCommercialSegmentRequest request, CancellationToken ct = default) => apiClient.PostAsync<SaveItemCommercialSegmentRequest, ItemCommercialSegmentItem>(Route, request, ct);
    public Task<ItemCommercialSegmentItem> UpdateAsync(int id, SaveItemCommercialSegmentRequest request, CancellationToken ct = default) => apiClient.PutAsync<SaveItemCommercialSegmentRequest, ItemCommercialSegmentItem>($"{Route}/{id}", request, ct);
    public async Task DeleteAsync(int id, CancellationToken ct = default) => await apiClient.DeleteAsync<object>($"{Route}/{id}", ct);
}

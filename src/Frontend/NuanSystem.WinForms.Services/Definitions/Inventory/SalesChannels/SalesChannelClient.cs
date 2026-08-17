using NuanSystem.WinForms.Services.Definitions.Inventory.SalesChannels.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.SalesChannels;

public interface ISalesChannelClient
{
    Task<IReadOnlyCollection<SalesChannelItem>> GetAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<SalesChannelItem>> GetLookupAsync(CancellationToken ct = default);
    Task<SalesChannelItem> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyCollection<SalesChannelAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default);
    Task<SalesChannelItem> CreateAsync(SaveSalesChannelRequest request, CancellationToken ct = default);
    Task<SalesChannelItem> UpdateAsync(int id, SaveSalesChannelRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
public sealed class SalesChannelClient(INuanApiClient apiClient) : ISalesChannelClient
{
    private const string Route = "/api/definitions/inventory/sales-channels";
    public async Task<IReadOnlyCollection<SalesChannelItem>> GetAsync(CancellationToken ct = default) => await apiClient.GetAsync<List<SalesChannelItem>>(Route, ct);
    public async Task<IReadOnlyCollection<SalesChannelItem>> GetLookupAsync(CancellationToken ct = default) => await apiClient.GetAsync<List<SalesChannelItem>>($"{Route}/lookup", ct);
    public Task<SalesChannelItem> GetByIdAsync(int id, CancellationToken ct = default) => apiClient.GetAsync<SalesChannelItem>($"{Route}/{id}", ct);
    public async Task<IReadOnlyCollection<SalesChannelAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => await apiClient.GetAsync<List<SalesChannelAuditChange>>($"{Route}/{id}/history", ct);
    public Task<SalesChannelItem> CreateAsync(SaveSalesChannelRequest request, CancellationToken ct = default) => apiClient.PostAsync<SaveSalesChannelRequest, SalesChannelItem>(Route, request, ct);
    public Task<SalesChannelItem> UpdateAsync(int id, SaveSalesChannelRequest request, CancellationToken ct = default) => apiClient.PutAsync<SaveSalesChannelRequest, SalesChannelItem>($"{Route}/{id}", request, ct);
    public async Task DeleteAsync(int id, CancellationToken ct = default) => await apiClient.DeleteAsync<object>($"{Route}/{id}", ct);
}



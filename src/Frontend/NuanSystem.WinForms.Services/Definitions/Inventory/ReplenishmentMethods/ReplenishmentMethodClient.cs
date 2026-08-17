using NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods;

public interface IReplenishmentMethodClient
{
    Task<IReadOnlyCollection<ReplenishmentMethodItem>> GetAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<ReplenishmentMethodLookupItem>> GetLookupAsync(CancellationToken ct = default);
    Task<ReplenishmentMethodItem> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyCollection<ReplenishmentMethodAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default);
    Task<ReplenishmentMethodItem> CreateAsync(SaveReplenishmentMethodRequest request, CancellationToken ct = default);
    Task<ReplenishmentMethodItem> UpdateAsync(int id, SaveReplenishmentMethodRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public sealed class ReplenishmentMethodClient(INuanApiClient apiClient) : IReplenishmentMethodClient
{
    private const string Route = "/api/definitions/inventory/replenishment-methods";

    public async Task<IReadOnlyCollection<ReplenishmentMethodItem>> GetAsync(CancellationToken ct = default) =>
        await apiClient.GetAsync<List<ReplenishmentMethodItem>>(Route, ct);

    public async Task<IReadOnlyCollection<ReplenishmentMethodLookupItem>> GetLookupAsync(CancellationToken ct = default) =>
        await apiClient.GetAsync<List<ReplenishmentMethodLookupItem>>($"{Route}/lookup", ct);

    public Task<ReplenishmentMethodItem> GetByIdAsync(int id, CancellationToken ct = default) =>
        apiClient.GetAsync<ReplenishmentMethodItem>($"{Route}/{id}", ct);

    public async Task<IReadOnlyCollection<ReplenishmentMethodAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) =>
        await apiClient.GetAsync<List<ReplenishmentMethodAuditChange>>($"{Route}/{id}/history", ct);

    public Task<ReplenishmentMethodItem> CreateAsync(SaveReplenishmentMethodRequest request, CancellationToken ct = default) =>
        apiClient.PostAsync<SaveReplenishmentMethodRequest, ReplenishmentMethodItem>(Route, request, ct);

    public Task<ReplenishmentMethodItem> UpdateAsync(int id, SaveReplenishmentMethodRequest request, CancellationToken ct = default) =>
        apiClient.PutAsync<SaveReplenishmentMethodRequest, ReplenishmentMethodItem>($"{Route}/{id}", request, ct);

    public async Task DeleteAsync(int id, CancellationToken ct = default) =>
        await apiClient.DeleteAsync<object>($"{Route}/{id}", ct);
}

using NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions;

public interface IStorageConditionClient
{
    Task<IReadOnlyCollection<StorageConditionItem>> GetAsync(CancellationToken ct = default);
    Task<IReadOnlyCollection<StorageConditionLookupItem>> GetLookupAsync(CancellationToken ct = default);
    Task<StorageConditionItem> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyCollection<StorageConditionAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default);
    Task<StorageConditionItem> CreateAsync(SaveStorageConditionRequest request, CancellationToken ct = default);
    Task<StorageConditionItem> UpdateAsync(int id, SaveStorageConditionRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

public sealed class StorageConditionClient(INuanApiClient apiClient) : IStorageConditionClient
{
    private const string Route = "/api/definitions/inventory/storage-conditions";
    public async Task<IReadOnlyCollection<StorageConditionItem>> GetAsync(CancellationToken ct = default) => await apiClient.GetAsync<List<StorageConditionItem>>(Route, ct);
    public async Task<IReadOnlyCollection<StorageConditionLookupItem>> GetLookupAsync(CancellationToken ct = default) => await apiClient.GetAsync<List<StorageConditionLookupItem>>($"{Route}/lookup", ct);
    public Task<StorageConditionItem> GetByIdAsync(int id, CancellationToken ct = default) => apiClient.GetAsync<StorageConditionItem>($"{Route}/{id}", ct);
    public async Task<IReadOnlyCollection<StorageConditionAuditChange>> GetHistoryAsync(int id, CancellationToken ct = default) => await apiClient.GetAsync<List<StorageConditionAuditChange>>($"{Route}/{id}/history", ct);
    public Task<StorageConditionItem> CreateAsync(SaveStorageConditionRequest request, CancellationToken ct = default) => apiClient.PostAsync<SaveStorageConditionRequest, StorageConditionItem>(Route, request, ct);
    public Task<StorageConditionItem> UpdateAsync(int id, SaveStorageConditionRequest request, CancellationToken ct = default) => apiClient.PutAsync<SaveStorageConditionRequest, StorageConditionItem>($"{Route}/{id}", request, ct);
    public async Task DeleteAsync(int id, CancellationToken ct = default) => await apiClient.DeleteAsync<object>($"{Route}/{id}", ct);
}

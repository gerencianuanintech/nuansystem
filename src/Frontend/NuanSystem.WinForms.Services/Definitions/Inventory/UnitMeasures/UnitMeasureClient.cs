using NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures;

public sealed class UnitMeasureClient(INuanApiClient apiClient) : IUnitMeasureClient
{
    private const string Route = "/api/definitions/inventory/unit-measures";

    public async Task<IReadOnlyCollection<UnitMeasureItem>> GetAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<UnitMeasureItem>>(Route, cancellationToken);

    public async Task<IReadOnlyCollection<UnitMeasureLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<UnitMeasureLookupItem>>($"{Route}/lookup", cancellationToken);

    public Task<UnitMeasureItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<UnitMeasureItem>($"{Route}/{id}", cancellationToken);

    public async Task<IReadOnlyCollection<UnitMeasureAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<UnitMeasureAuditChange>>($"{Route}/{id}/history", cancellationToken);

    public Task<UnitMeasureItem> CreateAsync(SaveUnitMeasureRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SaveUnitMeasureRequest, UnitMeasureItem>(Route, request, cancellationToken);

    public Task<UnitMeasureItem> UpdateAsync(int id, SaveUnitMeasureRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync<SaveUnitMeasureRequest, UnitMeasureItem>($"{Route}/{id}", request, cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.DeleteAsync<object>($"{Route}/{id}", cancellationToken);
}

using NuanSystem.WinForms.Services.Carriers.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Carriers;

public sealed class CarrierClient(INuanApiClient apiClient) : ICarrierClient
{
    private const string Route = "/api/carriers";

    public Task<IReadOnlyCollection<CarrierItem>> GetAllAsync(CancellationToken cancellationToken = default) => apiClient.GetAsync<IReadOnlyCollection<CarrierItem>>(Route, cancellationToken);
    public Task<IReadOnlyCollection<CarrierLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default) => apiClient.GetAsync<IReadOnlyCollection<CarrierLookupItem>>($"{Route}/lookup", cancellationToken);
    public Task<CarrierDetail> GetByIdAsync(int id, CancellationToken cancellationToken = default) => apiClient.GetAsync<CarrierDetail>($"{Route}/{id}", cancellationToken);
    public Task<IReadOnlyCollection<CarrierAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) => apiClient.GetAsync<IReadOnlyCollection<CarrierAuditChange>>($"{Route}/{id}/history", cancellationToken);
    public Task<CarrierDetail> CreateAsync(SaveCarrierRequest request, CancellationToken cancellationToken = default) => apiClient.PostAsync<SaveCarrierRequest, CarrierDetail>(Route, request, cancellationToken);
    public Task<CarrierDetail> UpdateAsync(int id, SaveCarrierRequest request, CancellationToken cancellationToken = default) => apiClient.PutAsync<SaveCarrierRequest, CarrierDetail>($"{Route}/{id}", request, cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"{Route}/{id}", cancellationToken);
    }
}

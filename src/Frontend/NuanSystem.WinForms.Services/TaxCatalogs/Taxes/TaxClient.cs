using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.TaxCatalogs.Taxes;

public sealed class TaxClient(INuanApiClient apiClient) : ITaxClient
{
    private const string Route = "/api/tax-catalogs/taxes";
    public async Task<IReadOnlyCollection<TaxItem>> GetAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<TaxItem>>(Route, cancellationToken);
    public Task<TaxItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<TaxItem>($"{Route}/{id}", cancellationToken);
    public Task<IReadOnlyCollection<TaxAuditChange>> GetHistoryAsync(int id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<IReadOnlyCollection<TaxAuditChange>>($"{Route}/{id}/history", cancellationToken);
    public Task<TaxItem> CreateAsync(SaveTaxRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SaveTaxRequest, TaxItem>(Route, request, cancellationToken);
    public Task<TaxItem> UpdateAsync(int id, SaveTaxRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync<SaveTaxRequest, TaxItem>($"{Route}/{id}", request, cancellationToken);
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.DeleteAsync<object>($"{Route}/{id}", cancellationToken);
}

using NuanSystem.WinForms.Services.Accounting.ChartOfAccounts.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Accounting.ChartOfAccounts;

public sealed class ChartOfAccountClient : IChartOfAccountClient
{
    private readonly INuanApiClient apiClient;

    public ChartOfAccountClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<ChartOfAccountItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<ChartOfAccountItem>>("/api/accounting/chart-of-accounts", cancellationToken);
    }

    public async Task<IReadOnlyCollection<ChartOfAccountLookupItem>> GetLookupAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<ChartOfAccountLookupItem>>("/api/accounting/chart-of-accounts/lookups", cancellationToken);
    }

    public Task<ChartOfAccountItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return apiClient.GetAsync<ChartOfAccountItem>($"/api/accounting/chart-of-accounts/{id}", cancellationToken);
    }

    public Task<ChartOfAccountItem> CreateAsync(SaveChartOfAccountRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<SaveChartOfAccountRequest, ChartOfAccountItem>("/api/accounting/chart-of-accounts", request, cancellationToken);
    }

    public Task<ChartOfAccountItem> UpdateAsync(int id, SaveChartOfAccountRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PutAsync<SaveChartOfAccountRequest, ChartOfAccountItem>($"/api/accounting/chart-of-accounts/{id}", request, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/accounting/chart-of-accounts/{id}", cancellationToken);
    }
}

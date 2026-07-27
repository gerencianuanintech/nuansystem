using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.FinancialCatalogs.PriceLists;

public sealed class PriceListClient(INuanApiClient apiClient) : IPriceListClient
{
    private const string Route = "/api/financial-catalogs/price-lists";

    public async Task<IReadOnlyCollection<PriceListItem>> GetAsync(CancellationToken cancellationToken = default) =>
        await apiClient.GetAsync<List<PriceListItem>>(Route, cancellationToken);

    public Task<PriceListItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        apiClient.GetAsync<PriceListItem>($"{Route}/{id}", cancellationToken);

    public Task<PriceListItem> CreateAsync(SavePriceListRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync<SavePriceListRequest, PriceListItem>(Route, request, cancellationToken);

    public Task<PriceListItem> UpdateAsync(int id, SavePriceListRequest request, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync<SavePriceListRequest, PriceListItem>($"{Route}/{id}", request, cancellationToken);

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        await apiClient.DeleteAsync<object>($"{Route}/{id}", cancellationToken);
}

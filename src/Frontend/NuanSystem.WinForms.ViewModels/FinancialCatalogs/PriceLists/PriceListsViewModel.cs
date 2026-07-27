using NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs;
using NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs.Models;
using NuanSystem.WinForms.Services.FinancialCatalogs.PriceLists;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.FinancialCatalogs.PriceLists;

public sealed class PriceListsViewModel(
    IPriceListClient priceListClient,
    IFinancialCatalogClient financialCatalogClient)
    : CrudViewModel<PriceListItem, SavePriceListRequest>
{
    public IReadOnlyCollection<FinancialCatalogLookupItem> Currencies { get; private set; } =
        Array.Empty<FinancialCatalogLookupItem>();

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        var currenciesTask = financialCatalogClient.GetLookupAsync("currencies", cancellationToken);
        await LoadItemsAsync(priceListClient.GetAsync, cancellationToken);
        Currencies = await currenciesTask;
    }

    public Task<PriceListItem> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        priceListClient.GetByIdAsync(id, cancellationToken);

    public override Task CreateAsync(SavePriceListRequest request, CancellationToken cancellationToken = default) =>
        priceListClient.CreateAsync(request, cancellationToken);

    public override Task UpdateAsync(int id, SavePriceListRequest request, CancellationToken cancellationToken = default) =>
        priceListClient.UpdateAsync(id, request, cancellationToken);

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        priceListClient.DeleteAsync(id, cancellationToken);
}

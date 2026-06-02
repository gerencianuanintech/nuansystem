using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs;
using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.TaxCatalogs.Catalogs;

public sealed class TaxCatalogsViewModel
    : CrudViewModel<TaxCatalogItem, SaveTaxCatalogRequest>
{
    private readonly ITaxCatalogClient catalogClient;
    private readonly string catalogRoute;

    public TaxCatalogsViewModel(ITaxCatalogClient catalogClient, string catalogRoute)
    {
        this.catalogClient = catalogClient;
        this.catalogRoute = catalogRoute;
    }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(token => catalogClient.GetAsync(catalogRoute, token), cancellationToken);
    }

    public Task<TaxCatalogItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return catalogClient.GetByIdAsync(catalogRoute, id, cancellationToken);
    }

    public override Task CreateAsync(SaveTaxCatalogRequest request, CancellationToken cancellationToken = default)
    {
        return catalogClient.CreateAsync(catalogRoute, request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveTaxCatalogRequest request, CancellationToken cancellationToken = default)
    {
        return catalogClient.UpdateAsync(catalogRoute, id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return catalogClient.DeleteAsync(catalogRoute, id, cancellationToken);
    }
}

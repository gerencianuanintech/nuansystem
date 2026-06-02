using NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs;
using NuanSystem.WinForms.Services.FinancialCatalogs.Catalogs.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.FinancialCatalogs.Catalogs;

public sealed class FinancialCatalogsViewModel
    : CrudViewModel<FinancialCatalogItem, SaveFinancialCatalogRequest>
{
    private readonly IFinancialCatalogClient catalogClient;
    private readonly string catalogRoute;

    public FinancialCatalogsViewModel(
        IFinancialCatalogClient catalogClient,
        string catalogRoute)
    {
        this.catalogClient = catalogClient;
        this.catalogRoute = catalogRoute;
    }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(token => catalogClient.GetAsync(catalogRoute, token), cancellationToken);
    }

    public Task<FinancialCatalogItem> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return catalogClient.GetByIdAsync(catalogRoute, id, cancellationToken);
    }

    public override Task CreateAsync(
        SaveFinancialCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return catalogClient.CreateAsync(catalogRoute, request, cancellationToken);
    }

    public override Task UpdateAsync(
        int id,
        SaveFinancialCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return catalogClient.UpdateAsync(catalogRoute, id, request, cancellationToken);
    }

    public override Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return catalogClient.DeleteAsync(catalogRoute, id, cancellationToken);
    }
}

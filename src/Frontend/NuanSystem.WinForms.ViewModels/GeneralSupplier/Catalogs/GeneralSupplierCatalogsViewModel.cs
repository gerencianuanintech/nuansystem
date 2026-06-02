using NuanSystem.WinForms.Services.GeneralSupplier.Catalogs;
using NuanSystem.WinForms.Services.GeneralSupplier.Catalogs.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.GeneralSupplier.Catalogs;

public sealed class GeneralSupplierCatalogsViewModel
    : CrudViewModel<GeneralSupplierCatalogItem, SaveGeneralSupplierCatalogRequest>
{
    private readonly IGeneralSupplierCatalogClient catalogClient;
    private readonly string catalogRoute;

    public GeneralSupplierCatalogsViewModel(
        IGeneralSupplierCatalogClient catalogClient,
        string catalogRoute)
    {
        this.catalogClient = catalogClient;
        this.catalogRoute = catalogRoute;
    }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(token => catalogClient.GetAsync(catalogRoute, token), cancellationToken);
    }

    public Task<GeneralSupplierCatalogItem> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return catalogClient.GetByIdAsync(catalogRoute, id, cancellationToken);
    }

    public override Task CreateAsync(
        SaveGeneralSupplierCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return catalogClient.CreateAsync(catalogRoute, request, cancellationToken);
    }

    public override Task UpdateAsync(
        int id,
        SaveGeneralSupplierCatalogRequest request,
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


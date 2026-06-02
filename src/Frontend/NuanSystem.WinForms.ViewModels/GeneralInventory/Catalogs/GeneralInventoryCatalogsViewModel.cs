using NuanSystem.WinForms.Services.GeneralInventory.Catalogs;
using NuanSystem.WinForms.Services.GeneralInventory.Catalogs.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.GeneralInventory.Catalogs;

public sealed class GeneralInventoryCatalogsViewModel
    : CrudViewModel<GeneralInventoryCatalogItem, SaveGeneralInventoryCatalogRequest>
{
    private readonly IGeneralInventoryCatalogClient catalogClient;
    private readonly string catalogRoute;

    public GeneralInventoryCatalogsViewModel(
        IGeneralInventoryCatalogClient catalogClient,
        string catalogRoute)
    {
        this.catalogClient = catalogClient;
        this.catalogRoute = catalogRoute;
    }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(token => catalogClient.GetAsync(catalogRoute, token), cancellationToken);
    }

    public Task<GeneralInventoryCatalogItem> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return catalogClient.GetByIdAsync(catalogRoute, id, cancellationToken);
    }

    public override Task CreateAsync(
        SaveGeneralInventoryCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return catalogClient.CreateAsync(catalogRoute, request, cancellationToken);
    }

    public override Task UpdateAsync(
        int id,
        SaveGeneralInventoryCatalogRequest request,
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

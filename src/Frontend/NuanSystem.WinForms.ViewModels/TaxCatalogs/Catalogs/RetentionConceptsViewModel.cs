using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs;
using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.TaxCatalogs.Catalogs;

public sealed class RetentionConceptsViewModel
    : CrudViewModel<RetentionConceptItem, SaveRetentionConceptRequest>
{
    private readonly ITaxCatalogClient catalogClient;

    public RetentionConceptsViewModel(ITaxCatalogClient catalogClient)
    {
        this.catalogClient = catalogClient;
    }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(catalogClient.GetRetentionConceptsAsync, cancellationToken);
    }

    public Task<IReadOnlyCollection<TaxCatalogLookupItem>> GetRetentionTypesLookupAsync(CancellationToken cancellationToken = default)
    {
        return catalogClient.GetLookupAsync(TaxCatalogRoutes.RetentionTypes, cancellationToken);
    }

    public Task<RetentionConceptItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return catalogClient.GetRetentionConceptByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveRetentionConceptRequest request, CancellationToken cancellationToken = default)
    {
        return catalogClient.CreateRetentionConceptAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveRetentionConceptRequest request, CancellationToken cancellationToken = default)
    {
        return catalogClient.UpdateRetentionConceptAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return catalogClient.DeleteAsync(TaxCatalogRoutes.RetentionConcepts, id, cancellationToken);
    }
}

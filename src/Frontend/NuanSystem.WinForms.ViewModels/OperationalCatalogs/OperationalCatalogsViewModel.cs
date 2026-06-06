using NuanSystem.WinForms.Services.OperationalCatalogs;
using NuanSystem.WinForms.Services.OperationalCatalogs.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.OperationalCatalogs;

public sealed class OperationalCatalogsViewModel
    : CrudViewModel<OperationalCatalogItem, SaveOperationalCatalogRequest>
{
    private readonly IOperationalCatalogClient catalogClient;

    public OperationalCatalogsViewModel(IOperationalCatalogClient catalogClient)
    {
        this.catalogClient = catalogClient;
    }

    public string CatalogKey { get; private set; } = OperationalCatalogDescriptors.DocumentEstablishment;

    public IReadOnlyCollection<OperationalCatalogLookupItem> ParentValues { get; private set; } =
        Array.Empty<OperationalCatalogLookupItem>();

    public void SetCatalogKey(string catalogKey)
    {
        CatalogKey = string.IsNullOrWhiteSpace(catalogKey)
            ? OperationalCatalogDescriptors.DocumentEstablishment
            : catalogKey.Trim().ToUpperInvariant();
    }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(token => catalogClient.GetAsync(CatalogKey, cancellationToken: token), cancellationToken);
    }

    public Task<OperationalCatalogItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return catalogClient.GetByIdAsync(CatalogKey, id, cancellationToken);
    }

    public async Task LoadParentValuesAsync(CancellationToken cancellationToken = default)
    {
        var parentCatalogKey = OperationalCatalogDescriptors.All
            .FirstOrDefault(item => string.Equals(item.CatalogKey, CatalogKey, StringComparison.OrdinalIgnoreCase))
            ?.ParentCatalogKey;

        ParentValues = string.IsNullOrWhiteSpace(parentCatalogKey)
            ? Array.Empty<OperationalCatalogLookupItem>()
            : await catalogClient.GetLookupAsync(parentCatalogKey, activeOnly: true, cancellationToken: cancellationToken);
    }

    public override Task CreateAsync(
        SaveOperationalCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return catalogClient.CreateAsync(CatalogKey, request, cancellationToken);
    }

    public override Task UpdateAsync(
        int id,
        SaveOperationalCatalogRequest request,
        CancellationToken cancellationToken = default)
    {
        return catalogClient.UpdateAsync(CatalogKey, id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return catalogClient.DeleteAsync(CatalogKey, id, cancellationToken);
    }
}

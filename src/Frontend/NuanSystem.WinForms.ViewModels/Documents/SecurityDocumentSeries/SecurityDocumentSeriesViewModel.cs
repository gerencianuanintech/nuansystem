using NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries;
using NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;
using NuanSystem.WinForms.Services.OperationalCatalogs;
using NuanSystem.WinForms.Services.OperationalCatalogs.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Documents.SecurityDocumentSeries;

public sealed class SecurityDocumentSeriesViewModel
    : CrudViewModel<SecurityDocumentSeriesItem, SaveSecurityDocumentSeriesRequest>
{
    private readonly ISecurityDocumentSeriesClient seriesClient;
    private readonly IOperationalCatalogClient? operationalCatalogClient;

    public SecurityDocumentSeriesViewModel(
        ISecurityDocumentSeriesClient seriesClient,
        IOperationalCatalogClient? operationalCatalogClient = null)
    {
        this.seriesClient = seriesClient;
        this.operationalCatalogClient = operationalCatalogClient;
    }

    public SecurityDocumentSeriesLookupSet Lookups { get; private set; } = SecurityDocumentSeriesCatalogs.Defaults();

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(token => seriesClient.GetAsync(cancellationToken: token), cancellationToken);
    }

    public Task<SecurityDocumentSeriesItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return seriesClient.GetByIdAsync(id, cancellationToken);
    }

    public async Task LoadLookupsAsync(CancellationToken cancellationToken = default)
    {
        if (operationalCatalogClient is null)
        {
            Lookups = SecurityDocumentSeriesCatalogs.Defaults();
            return;
        }

        try
        {
            var documentTypes = await operationalCatalogClient.GetLookupAsync(
                OperationalCatalogDescriptors.DocumentType,
                cancellationToken: cancellationToken);
            var establishments = await operationalCatalogClient.GetLookupAsync(
                OperationalCatalogDescriptors.DocumentEstablishment,
                cancellationToken: cancellationToken);
            var emissionPoints = await operationalCatalogClient.GetLookupAsync(
                OperationalCatalogDescriptors.DocumentEmissionPoint,
                cancellationToken: cancellationToken);
            var sapObjectTypes = await operationalCatalogClient.GetLookupAsync(
                OperationalCatalogDescriptors.SapObjectType,
                cancellationToken: cancellationToken);

            Lookups = new SecurityDocumentSeriesLookupSet(
                ToLookupOptions(documentTypes),
                ToLookupOptions(establishments),
                ToLookupOptions(emissionPoints),
                ToLookupOptions(sapObjectTypes));
        }
        catch
        {
            Lookups = SecurityDocumentSeriesCatalogs.Defaults();
        }
    }

    public override Task CreateAsync(
        SaveSecurityDocumentSeriesRequest request,
        CancellationToken cancellationToken = default)
    {
        return seriesClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(
        int id,
        SaveSecurityDocumentSeriesRequest request,
        CancellationToken cancellationToken = default)
    {
        return seriesClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return seriesClient.DeleteAsync(id, cancellationToken);
    }

    private static IReadOnlyCollection<LookupOption> ToLookupOptions(IReadOnlyCollection<OperationalCatalogLookupItem> source)
    {
        return source
            .OrderBy(item => item.DisplayOrder)
            .ThenBy(item => item.Name)
            .Select(item => new LookupOption(
                item.Code,
                $"{item.Code} - {item.Name}",
                item.ParentCatalogKey,
                item.ParentCode))
            .ToArray();
    }
}

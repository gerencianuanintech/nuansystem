using NuanSystem.WinForms.Services.Geography;
using NuanSystem.WinForms.Services.Geography.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Geography;

public sealed class ProvincesViewModel(IGeographyClient geographyClient)
    : CrudViewModel<ProvinceItem, SaveProvinceRequest>
{
    public IReadOnlyCollection<GeographyLookupItem> Countries { get; private set; } = Array.Empty<GeographyLookupItem>();

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Countries = await geographyClient.GetCountryLookupAsync(cancellationToken);
        await LoadItemsAsync(geographyClient.GetProvincesAsync, cancellationToken);
    }

    public Task<ProvinceItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.GetProvinceByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveProvinceRequest request, CancellationToken cancellationToken = default)
    {
        return geographyClient.CreateProvinceAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveProvinceRequest request, CancellationToken cancellationToken = default)
    {
        return geographyClient.UpdateProvinceAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.DeleteProvinceAsync(id, cancellationToken);
    }
}

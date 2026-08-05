using NuanSystem.WinForms.Services.Definitions.General.Cities;
using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.General.Cities;

public sealed class CitiesViewModel(IGeographyClient geographyClient)
    : CrudViewModel<CityItem, SaveCityRequest>
{
    public IReadOnlyCollection<GeographyLookupItem> Countries { get; private set; } = Array.Empty<GeographyLookupItem>();

    public IReadOnlyCollection<GeographyLookupItem> Provinces { get; private set; } = Array.Empty<GeographyLookupItem>();

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Countries = await geographyClient.GetCountryLookupAsync(cancellationToken);
        Provinces = await geographyClient.GetProvinceLookupAsync(cancellationToken: cancellationToken);
        await LoadItemsAsync(geographyClient.GetCitiesAsync, cancellationToken);
    }

    public Task<CityItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.GetCityByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveCityRequest request, CancellationToken cancellationToken = default)
    {
        return geographyClient.CreateCityAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveCityRequest request, CancellationToken cancellationToken = default)
    {
        return geographyClient.UpdateCityAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.DeleteCityAsync(id, cancellationToken);
    }
}

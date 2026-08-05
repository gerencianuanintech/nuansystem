using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.General.Countries;

public sealed class CountriesViewModel(IGeographyClient geographyClient)
    : CrudViewModel<CountryItem, SaveCountryRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(geographyClient.GetCountriesAsync, cancellationToken);
    }

    public Task<CountryItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.GetCountryByIdAsync(id, cancellationToken);
    }

    public override Task CreateAsync(SaveCountryRequest request, CancellationToken cancellationToken = default)
    {
        return geographyClient.CreateCountryAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveCountryRequest request, CancellationToken cancellationToken = default)
    {
        return geographyClient.UpdateCountryAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.DeleteCountryAsync(id, cancellationToken);
    }
}

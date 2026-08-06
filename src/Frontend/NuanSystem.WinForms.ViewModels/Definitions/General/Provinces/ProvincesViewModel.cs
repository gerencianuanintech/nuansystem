using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.ViewModels.Common;
using NuanSystem.WinForms.ViewModels.Definitions.General.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.General.Provinces;

public sealed class ProvincesViewModel(
    IGeographyClient geographyClient,
    ISecurityAccessClient securityAccessClient)
    : CrudViewModel<ProvinceItem, SaveProvinceRequest>
{
    public IReadOnlyCollection<GeographyLookupItem> Countries { get; private set; } = Array.Empty<GeographyLookupItem>();

    public bool CanCreateCountries { get; private set; }

    public bool CanUpdateCountries { get; private set; }

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Countries = await geographyClient.GetCountryLookupAsync(cancellationToken);
        var countryAccess = await GeographyRelatedFormAccess.LoadAsync(
            securityAccessClient,
            "countries",
            cancellationToken);
        CanCreateCountries = countryAccess.CanCreate;
        CanUpdateCountries = countryAccess.CanUpdate;
        await LoadItemsAsync(geographyClient.GetProvincesAsync, cancellationToken);
    }

    public Task<ProvinceItem> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.GetProvinceByIdAsync(id, cancellationToken);
    }

    public Task<CountryItem> GetCountryByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return geographyClient.GetCountryByIdAsync(id, cancellationToken);
    }

    public async Task<CountryItem> CreateCountryAsync(SaveCountryRequest request, CancellationToken cancellationToken = default)
    {
        var saved = await geographyClient.CreateCountryAsync(request, cancellationToken);
        UpdateCountryLookup(saved);
        return saved;
    }

    public async Task<CountryItem> UpdateCountryAsync(int id, SaveCountryRequest request, CancellationToken cancellationToken = default)
    {
        var saved = await geographyClient.UpdateCountryAsync(id, request, cancellationToken);
        UpdateCountryLookup(saved);
        return saved;
    }

    private void UpdateCountryLookup(CountryItem country)
    {
        Countries = Countries
            .Where(item => item.Id != country.Id)
            .Concat(country.IsActive
                ? [new GeographyLookupItem
                {
                    Id = country.Id,
                    Code = country.Code,
                    Name = country.Name,
                    IsActive = true
                }]
                : [])
            .OrderBy(item => item.Name)
            .ToArray();
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

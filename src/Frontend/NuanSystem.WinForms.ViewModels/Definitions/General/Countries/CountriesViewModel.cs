using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Definitions.General.Countries;

public sealed class CountriesViewModel(IGeographyClient geographyClient)
    : CrudViewModel<CountryItem, SaveCountryRequest>
{
    public string? Search { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 50;

    public int TotalCount { get; private set; }

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        IsBusy = true;
        try
        {
            var page = await geographyClient.SearchCountriesAsync(
                Search,
                PageNumber,
                PageSize,
                cancellationToken);
            Items = page.Items;
            TotalCount = page.TotalCount;
            PageNumber = page.PageNumber;
            PageSize = page.PageSize;
        }
        finally
        {
            IsBusy = false;
        }
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

using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.Definitions.General.Countries;
using NuanSystem.WinForms.Forms.Definitions.General.Provinces;
using NuanSystem.WinForms.Services.Definitions.General.Cities;
using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.General.Cities;

namespace NuanSystem.WinForms.Forms.Definitions.General.Cities;

public sealed partial class CitiesForm : BaseGridCrudListForm
{
    private const string FormKey = "cities";
    private static readonly CrudOperationPermissions Permissions = new(
        PermissionCodes.GeographyCitiesRead,
        PermissionCodes.GeographyCitiesManage,
        PermissionCodes.GeographyCitiesManage,
        PermissionCodes.GeographyCitiesManage);

    private readonly CitiesViewModel viewModel;
    private readonly ApiSession session;

    public CitiesForm()
    {
        viewModel = null!;
        session = null!;
        InitializeComponent();
        WirePermissions();
    }

    public CitiesForm(CitiesViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, FormKey);
        WirePermissions();
    }

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await viewModel.LoadAsync();
            SetGridData(viewModel.Items);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override async Task CreateAsync()
    {
        using var form = CreateCityEditor(Array.Empty<GeographyLookupItem>());
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Ciudad creada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        var provinces = await viewModel.LoadProvincesAsync(fullItem.CountryCode);
        using var form = CreateCityEditor(provinces, fullItem);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Ciudad actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        var provinces = await viewModel.LoadProvincesAsync(fullItem.CountryCode);
        using var form = CreateCityEditor(provinces, fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Ciudad copiada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item || !Confirm($"Eliminar ciudad {item.Code}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        ShowWarning("Historial preparado para integrarse con auditoría de geografía.");
        return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(CityItem.CountryName), "País", 1, 160);
        ConfigureColumn(nameof(CityItem.ProvinceName), "Provincia", 2, 180);
        ConfigureColumn(nameof(CityItem.Code), "Código", 3, 90);
        ConfigureColumn(nameof(CityItem.Name), "Nombre", 4, 220);
        ConfigureColumn(nameof(CityItem.IsActive), "Activo", 5, 80);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private CityItem? SelectedItem() => SelectedGridItem<CityItem>();

    private CityEditForm CreateCityEditor(
        IReadOnlyCollection<GeographyLookupItem> provinces,
        CityItem? item = null,
        bool copyMode = false)
    {
        var form = item is null
            ? new CityEditForm(
                viewModel.Countries,
                provinces,
                viewModel.CanCreateCountries,
                viewModel.CanUpdateCountries,
                viewModel.CanCreateProvinces,
                viewModel.CanUpdateProvinces)
            : new CityEditForm(
                viewModel.Countries,
                provinces,
                item,
                copyMode,
                viewModel.CanCreateCountries,
                viewModel.CanUpdateCountries,
                viewModel.CanCreateProvinces,
                viewModel.CanUpdateProvinces);
        form.LoadProvincesRequested += (_, countryCode) => viewModel.LoadProvincesAsync(countryCode);
        form.CreateCountryRequested += owner => CreateCountryAsync(owner);
        form.EditCountryRequested += (owner, countryId) => EditCountryAsync(owner, countryId);
        form.CreateProvinceRequested += (owner, countryId) => CreateProvinceAsync(owner, countryId);
        form.EditProvinceRequested += (owner, provinceId) => EditProvinceAsync(owner, provinceId);
        return form;
    }

    private async Task<CountryItem?> CreateCountryAsync(IWin32Window owner)
    {
        using var form = new CountryEditForm();
        if (form.ShowDialog(owner) != DialogResult.OK)
        {
            return null;
        }

        return await viewModel.CreateCountryAsync(form.Request);
    }

    private async Task<CountryItem?> EditCountryAsync(IWin32Window owner, int countryId)
    {
        var country = await viewModel.GetCountryByIdAsync(countryId);
        using var form = new CountryEditForm(country);
        if (form.ShowDialog(owner) != DialogResult.OK)
        {
            return null;
        }

        return await viewModel.UpdateCountryAsync(countryId, form.Request);
    }

    private async Task<ProvinceItem?> CreateProvinceAsync(IWin32Window owner, int countryId)
    {
        using var form = CreateProvinceEditor(countryId);
        if (form.ShowDialog(owner) != DialogResult.OK)
        {
            return null;
        }

        return await viewModel.CreateProvinceAsync(form.Request);
    }

    private async Task<ProvinceItem?> EditProvinceAsync(IWin32Window owner, int provinceId)
    {
        var province = await viewModel.GetProvinceByIdAsync(provinceId);
        using var form = CreateProvinceEditor(province.CountryId, province);
        if (form.ShowDialog(owner) != DialogResult.OK)
        {
            return null;
        }

        return await viewModel.UpdateProvinceAsync(provinceId, form.Request);
    }

    private ProvinceEditForm CreateProvinceEditor(int countryId, ProvinceItem? province = null)
    {
        var form = province is null
            ? new ProvinceEditForm(
                viewModel.Countries,
                viewModel.CanCreateCountries,
                viewModel.CanUpdateCountries,
                countryId)
            : new ProvinceEditForm(
                viewModel.Countries,
                province,
                canCreateCountries: viewModel.CanCreateCountries,
                canUpdateCountries: viewModel.CanUpdateCountries);
        form.CreateCountryRequested += owner => CreateCountryAsync(owner);
        form.EditCountryRequested += (owner, selectedCountryId) => EditCountryAsync(owner, selectedCountryId);
        return form;
    }

    private void ConfigureColumn(string fieldName, string caption, int visibleIndex, int width)
    {
        if (GridView.Columns[fieldName] is not { } column)
        {
            return;
        }

        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
    }

    private void WirePermissions()
    {
        Text = "Ciudades";
        if (session is not null)
        {
            ConfigureCrudPermissions(session, Permissions);
        }
    }
}

using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.Definitions.General.Cities;
using NuanSystem.WinForms.Forms.Definitions.General.Countries;
using NuanSystem.WinForms.Forms.Definitions.General.Provinces;
using NuanSystem.WinForms.Services.Definitions.General.Cities;
using NuanSystem.WinForms.Services.Definitions.General.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;
using NuanSystem.WinForms.Services.GeneralInventory.Warehouses.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.Warehouses;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Warehouses;

public sealed partial class WarehousesForm : BaseGridCrudListForm
{
    private readonly WarehousesViewModel viewModel;
    private readonly ApiSession session;

    public WarehousesForm()
    {
        viewModel = null!;
        session = null!;
        InitializeComponent();
        WirePermissions();
    }

    public WarehousesForm(WarehousesViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, "inventory-warehouses");
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
        using var form = CreateEditForm([], []);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Bodega creada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        var provinces = string.IsNullOrWhiteSpace(fullItem.CountryCode)
            ? []
            : await viewModel.LoadProvincesAsync(fullItem.CountryCode);
        var cities = string.IsNullOrWhiteSpace(fullItem.CountryCode) || string.IsNullOrWhiteSpace(fullItem.ProvinceCode)
            ? []
            : await viewModel.LoadCitiesAsync(fullItem.CountryCode, fullItem.ProvinceCode);
        using var form = CreateEditForm(provinces, cities, fullItem);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Bodega actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        var provinces = string.IsNullOrWhiteSpace(fullItem.CountryCode)
            ? []
            : await viewModel.LoadProvincesAsync(fullItem.CountryCode);
        var cities = string.IsNullOrWhiteSpace(fullItem.CountryCode) || string.IsNullOrWhiteSpace(fullItem.ProvinceCode)
            ? []
            : await viewModel.LoadCitiesAsync(fullItem.CountryCode, fullItem.ProvinceCode);
        using var form = CreateEditForm(provinces, cities, fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Bodega copiada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        if (!Confirm($"Eliminar la bodega {item.Code}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();

        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(WarehouseItem.Code), "Codigo", 1, 110);
        ConfigureColumn(nameof(WarehouseItem.Name), "Bodega", 2, 200);
        ConfigureColumn(nameof(WarehouseItem.BranchCode), "Sucursal", 3, 110);
        ConfigureColumn(nameof(WarehouseItem.City), "Ciudad", 4, 120);
        ConfigureColumn(nameof(WarehouseItem.IsDefault), "Predeterminada", 5, 110);
        ConfigureColumn(nameof(WarehouseItem.AllowsSales), "Ventas", 6, 80);
        ConfigureColumn(nameof(WarehouseItem.AllowsPurchases), "Compras", 7, 80);
        ConfigureColumn(nameof(WarehouseItem.AllowsTransfers), "Transferencias", 8, 110);
        ConfigureColumn(nameof(WarehouseItem.IsActive), "Activo", 9, 80);
        ConfigureColumn(nameof(WarehouseItem.UpdatedAt), "Actualizado", 10, 140);

        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private WarehouseItem? SelectedItem()
    {
        return SelectedGridItem<WarehouseItem>();
    }

    private WarehouseEditForm CreateEditForm(
        IReadOnlyCollection<GeographyLookupItem> provinces,
        IReadOnlyCollection<GeographyLookupItem> cities,
        WarehouseItem? item = null,
        bool copyMode = false)
    {
        var form = item is null
            ? new WarehouseEditForm(
                viewModel.Countries,
                provinces,
                cities,
                viewModel.CanCreateCountries,
                viewModel.CanUpdateCountries,
                viewModel.CanCreateProvinces,
                viewModel.CanUpdateProvinces,
                viewModel.CanCreateCities,
                viewModel.CanUpdateCities)
            : new WarehouseEditForm(
                viewModel.Countries,
                provinces,
                cities,
                item,
                copyMode,
                viewModel.CanCreateCountries,
                viewModel.CanUpdateCountries,
                viewModel.CanCreateProvinces,
                viewModel.CanUpdateProvinces,
                viewModel.CanCreateCities,
                viewModel.CanUpdateCities);
        form.LoadProvincesRequested += (_, countryCode) => viewModel.LoadProvincesAsync(countryCode);
        form.LoadCitiesRequested += (_, countryCode, provinceCode) => viewModel.LoadCitiesAsync(countryCode, provinceCode);
        form.CreateCountryRequested += owner => CreateCountryAsync(owner);
        form.EditCountryRequested += (owner, countryId) => EditCountryAsync(owner, countryId);
        form.CreateProvinceRequested += (owner, countryId) => CreateProvinceAsync(owner, countryId);
        form.EditProvinceRequested += (owner, provinceId) => EditProvinceAsync(owner, provinceId);
        form.CreateCityRequested += (owner, countryId, provinceId) => CreateCityAsync(owner, countryId, provinceId);
        form.EditCityRequested += (owner, cityId) => EditCityAsync(owner, cityId);
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

    private async Task<CityItem?> CreateCityAsync(IWin32Window owner, int countryId, int provinceId)
    {
        var country = await viewModel.GetCountryByIdAsync(countryId);
        var provinces = await viewModel.LoadProvincesAsync(country.Code);
        using var form = CreateCityEditor(provinces, countryId, provinceId);
        if (form.ShowDialog(owner) != DialogResult.OK)
        {
            return null;
        }

        return await viewModel.CreateCityAsync(form.Request);
    }

    private async Task<CityItem?> EditCityAsync(IWin32Window owner, int cityId)
    {
        var city = await viewModel.GetCityByIdAsync(cityId);
        var provinces = await viewModel.LoadProvincesAsync(city.CountryCode);
        using var form = CreateCityEditor(provinces, city.CountryId, city.ProvinceId, city);
        if (form.ShowDialog(owner) != DialogResult.OK)
        {
            return null;
        }

        return await viewModel.UpdateCityAsync(cityId, form.Request);
    }

    private CityEditForm CreateCityEditor(
        IReadOnlyCollection<GeographyLookupItem> provinces,
        int countryId,
        int provinceId,
        CityItem? city = null)
    {
        var form = city is null
            ? new CityEditForm(
                viewModel.Countries,
                provinces,
                viewModel.CanCreateCountries,
                viewModel.CanUpdateCountries,
                viewModel.CanCreateProvinces,
                viewModel.CanUpdateProvinces,
                countryId,
                provinceId)
            : new CityEditForm(
                viewModel.Countries,
                provinces,
                city,
                canCreateCountries: viewModel.CanCreateCountries,
                canUpdateCountries: viewModel.CanUpdateCountries,
                canCreateProvinces: viewModel.CanCreateProvinces,
                canUpdateProvinces: viewModel.CanUpdateProvinces);
        form.LoadProvincesRequested += (_, countryCode) => viewModel.LoadProvincesAsync(countryCode);
        form.CreateCountryRequested += owner => CreateCountryAsync(owner);
        form.EditCountryRequested += (owner, selectedCountryId) => EditCountryAsync(owner, selectedCountryId);
        form.CreateProvinceRequested += (owner, selectedCountryId) => CreateProvinceAsync(owner, selectedCountryId);
        form.EditProvinceRequested += (owner, selectedProvinceId) => EditProvinceAsync(owner, selectedProvinceId);
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
        if (session is not null)
        {
            ConfigureCrudPermissions(session, CrudOperationPermissions.InventoryWarehouses);
        }
    }
}

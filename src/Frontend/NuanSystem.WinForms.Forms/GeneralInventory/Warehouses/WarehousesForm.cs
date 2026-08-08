using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.General.Common;
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
            ? new WarehouseEditForm(viewModel.Countries, provinces, cities)
            : new WarehouseEditForm(viewModel.Countries, provinces, cities, item, copyMode);
        form.LoadProvincesRequested += (_, countryCode) => viewModel.LoadProvincesAsync(countryCode);
        form.LoadCitiesRequested += (_, countryCode, provinceCode) => viewModel.LoadCitiesAsync(countryCode, provinceCode);
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

using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.Definitions.General.Provinces;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.General.Provinces;

namespace NuanSystem.WinForms.Forms.Definitions.General.Provinces;

public sealed partial class ProvincesForm : BaseGridCrudListForm
{
    private const string FormKey = "provinces";
    private static readonly CrudOperationPermissions Permissions = new(
        PermissionCodes.GeographyProvincesRead,
        PermissionCodes.GeographyProvincesManage,
        PermissionCodes.GeographyProvincesManage,
        PermissionCodes.GeographyProvincesManage);

    private readonly ProvincesViewModel viewModel;
    private readonly ApiSession session;

    public ProvincesForm()
    {
        viewModel = null!;
        session = null!;
        InitializeComponent();
        WirePermissions();
    }

    public ProvincesForm(ProvincesViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
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
        using var form = CreateProvinceEditor();
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Provincia creada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = CreateProvinceEditor(fullItem);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Provincia actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = CreateProvinceEditor(fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Provincia copiada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item || !Confirm($"Eliminar provincia {item.Code}?"))
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

        ConfigureColumn(nameof(ProvinceItem.CountryName), "País", 1, 180);
        ConfigureColumn(nameof(ProvinceItem.Code), "Código", 2, 90);
        ConfigureColumn(nameof(ProvinceItem.Name), "Nombre", 3, 240);
        ConfigureColumn(nameof(ProvinceItem.IsActive), "Activo", 4, 80);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ProvinceItem? SelectedItem() => SelectedGridItem<ProvinceItem>();

    private ProvinceEditForm CreateProvinceEditor(ProvinceItem? item = null, bool copyMode = false)
    {
        var canManageCountries = session?.HasPermission(PermissionCodes.GeographyCountriesManage) == true;
        var form = item is null
            ? new ProvinceEditForm(viewModel.Countries, canManageCountries)
            : new ProvinceEditForm(viewModel.Countries, item, copyMode, canManageCountries);
        form.CreateCountryRequested += owner => CreateCountryAsync(owner);
        form.EditCountryRequested += (owner, countryId) => EditCountryAsync(owner, countryId);
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
        Text = "Provincias";
        if (session is not null)
        {
            ConfigureCrudPermissions(session, Permissions);
        }
    }
}

using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Geography.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Geography;

namespace NuanSystem.WinForms.Forms.Geography.Cities;

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
        using var form = new CityEditForm(viewModel.Countries, viewModel.Provinces);
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
        using var form = new CityEditForm(viewModel.Countries, viewModel.Provinces, fullItem);
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
        using var form = new CityEditForm(viewModel.Countries, viewModel.Provinces, fullItem, copyMode: true);
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

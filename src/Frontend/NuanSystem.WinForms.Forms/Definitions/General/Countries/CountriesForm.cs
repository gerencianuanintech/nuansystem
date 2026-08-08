using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Controls.Grids;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.General.Countries;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.General.Countries;

namespace NuanSystem.WinForms.Forms.Definitions.General.Countries;

public sealed partial class CountriesForm : BaseGridCrudListForm
{
    private const string FormKey = "countries";
    private static readonly CrudOperationPermissions Permissions = new(
        PermissionCodes.GeographyCountriesRead,
        PermissionCodes.GeographyCountriesManage,
        PermissionCodes.GeographyCountriesManage,
        PermissionCodes.GeographyCountriesManage);

    private readonly CountriesViewModel viewModel;
    private readonly ApiSession session;

    public CountriesForm()
    {
        viewModel = null!;
        session = null!;
        InitializeComponent();
        WirePermissions();
    }

    public CountriesForm(CountriesViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, FormKey);
        EnableServerPaging(50);
        EnableServerFind(ApplyServerFindAsync);
        NuanGrid.PageRequested += OnPageRequested;
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
            SetPagedGridData(
                viewModel.Items,
                viewModel.PageNumber,
                viewModel.PageSize,
                viewModel.TotalCount);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override async Task CreateAsync()
    {
        using var form = new CountryEditForm();
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("País creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = new CountryEditForm(fullItem);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("País actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = new CountryEditForm(fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("País copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item || !Confirm($"Eliminar país {item.Code}?"))
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

        ConfigureColumn(nameof(CountryItem.Code), "Código", 1, 90);
        ConfigureColumn(nameof(CountryItem.Name), "Nombre", 2, 220);
        ConfigureColumn(nameof(CountryItem.Iso2), "ISO2", 3, 70);
        ConfigureColumn(nameof(CountryItem.Iso3), "ISO3", 4, 70);
        ConfigureColumn(nameof(CountryItem.PhonePrefix), "Prefijo", 5, 90);
        ConfigureColumn(nameof(CountryItem.IsActive), "Activo", 6, 80);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private CountryItem? SelectedItem() => SelectedGridItem<CountryItem>();

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
        Text = "Países";
        if (session is not null)
        {
            ConfigureCrudPermissions(session, Permissions);
        }
    }

    private async void OnPageRequested(object? sender, NuanGridPageRequestEventArgs args)
    {
        viewModel.PageNumber = args.Page;
        viewModel.PageSize = args.PageSize;
        await LoadDataAsync();
    }

    private async Task ApplyServerFindAsync(string? searchText)
    {
        viewModel.Search = searchText;
        viewModel.PageNumber = 1;
        await LoadDataAsync();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        if (viewModel is not null)
        {
            NuanGrid.PageRequested -= OnPageRequested;
        }

        base.OnFormClosed(e);
    }
}

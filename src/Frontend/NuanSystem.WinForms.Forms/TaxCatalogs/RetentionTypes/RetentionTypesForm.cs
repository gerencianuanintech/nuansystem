using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.TaxCatalogs.Catalogs;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.TaxCatalogs.Catalogs.Models;
using NuanSystem.WinForms.ViewModels.TaxCatalogs.Catalogs;

namespace NuanSystem.WinForms.Forms.TaxCatalogs.RetentionTypes;

public sealed partial class RetentionTypesForm : BaseGridCrudListForm
{
    private static readonly TaxCatalogDescriptor Descriptor = TaxCatalogDescriptors.RetentionTypes;
    private readonly TaxCatalogsViewModel viewModel;
    private readonly ApiSession session;

    public RetentionTypesForm()
    {
        viewModel = null!;
        session = null!;
        InitializeComponent();
        WirePermissions();
    }

    public RetentionTypesForm(TaxCatalogsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, Descriptor.FormKey);
        WirePermissions();
    }

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null) return;
        await RunWithBusyStateAsync(async () =>
        {
            await viewModel.LoadAsync();
            SetGridData(viewModel.Items);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override async Task CreateAsync()
    {
        using var form = new RetentionTypeEditForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Registro creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item) return;
        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = new RetentionTypeEditForm(fullItem);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Registro actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item) return;
        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = new RetentionTypeEditForm(fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Registro copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item) return;
        if (!Confirm($"Eliminar {Descriptor.SingularTitle} {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        ShowWarning("Historial preparado para integrarse con auditoria de catalogos tributarios.");
        return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        ConfigureColumn(nameof(TaxCatalogItem.Code), "Codigo", 1, 120);
        ConfigureColumn(nameof(TaxCatalogItem.Name), "Nombre", 2, 240);
        ConfigureColumn(nameof(TaxCatalogItem.Description), "Descripcion", 3, 360);
        ConfigureColumn(nameof(TaxCatalogItem.IsActive), "Activo", 4, 80);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private TaxCatalogItem? SelectedItem() => SelectedGridItem<TaxCatalogItem>();

    private void ConfigureColumn(string fieldName, string caption, int visibleIndex, int width)
    {
        if (GridView.Columns[fieldName] is not { } column) return;
        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
    }

    private void WirePermissions()
    {
        Text = Descriptor.Title;
        if (session is not null) ConfigureCrudPermissions(session, Descriptor.Permissions);
    }
}

using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.Services.TaxCatalogs.Taxes;
using NuanSystem.WinForms.ViewModels.TaxCatalogs.Taxes;

namespace NuanSystem.WinForms.Forms.TaxCatalogs.Taxes;

public sealed partial class TaxesForm : BaseGridCrudListForm
{
    public const string FormKey = "taxes";
    private readonly TaxesViewModel viewModel;
    private readonly ApiSession session;

    public TaxesForm()
    {
        viewModel = null!;
        session = null!;
        InitializeComponent();
    }

    public TaxesForm(TaxesViewModel viewModel, ApiSession session, IGridColumnSettingsClient columns)
    {
        this.viewModel = viewModel;
        this.session = session;
        InitializeComponent();
        ConfigureColumnPersonalization(columns, FormKey);
        ConfigureCrudPermissions(session, new CrudOperationPermissions(
            PermissionCodes.TaxRatesRead, PermissionCodes.TaxRatesManage,
            PermissionCodes.TaxRatesManage, PermissionCodes.TaxRatesManage));
    }

    protected override async Task LoadDataAsync() => await RunWithBusyStateAsync(async () =>
    {
        await viewModel.LoadAsync();
        SetGridData(viewModel.Items);
        await ApplyColumnSettingsAsync();
    });

    protected override async Task CreateAsync()
    {
        using var form = new TaxEditForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Impuesto creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedGridItem<TaxItem>() is not { } item) return;
        using var form = new TaxEditForm(await viewModel.GetByIdAsync(item.Id));
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Impuesto actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedGridItem<TaxItem>() is not { } item) return;
        using var form = new TaxEditForm(await viewModel.GetByIdAsync(item.Id), true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedGridItem<TaxItem>() is not { } item || !Confirm($"Eliminar impuesto {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedGridItem<TaxItem>() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm(
            "Historial de impuesto",
            $"{item.Code} - {item.Name}",
            cancellationToken => viewModel.GetHistoryAsync(item.Id, cancellationToken));
        form.ShowDialog(this);
        return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        Configure(nameof(TaxItem.Code), "Código", 1, 100);
        Configure(nameof(TaxItem.Name), "Nombre", 2, 240);
        Configure(nameof(TaxItem.Rate), "Porcentaje", 3, 100, "p2");
        Configure(nameof(TaxItem.IsActive), "Activo", 4, 70);
    }

    private void Configure(string field, string caption, int index, int width, string? format = null)
    {
        if (GridView.Columns[field] is not { } column) return;
        column.Caption = caption; column.Visible = true; column.VisibleIndex = index; column.Width = width;
        if (format is not null) { column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric; column.DisplayFormat.FormatString = format; }
    }
}

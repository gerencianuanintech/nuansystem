using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.UnitMeasures.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.UnitMeasures;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.UnitMeasures;

public sealed class UnitMeasuresForm : BaseGridCrudListForm
{
    public const string FormKey = "unit-measures";
    private readonly UnitMeasuresViewModel viewModel;
    private readonly ApiSession session;

    public UnitMeasuresForm()
    {
        viewModel = null!;
        session = null!;
        ConfigureWindow();
        WirePermissions();
    }

    public UnitMeasuresForm(
        UnitMeasuresViewModel viewModel,
        ApiSession session,
        IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        ConfigureWindow();
        ConfigureColumnPersonalization(columnSettingsClient, FormKey);
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
        using var form = new UnitMeasureEditForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Unidad de medida creada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        using var form = new UnitMeasureEditForm(item);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Unidad de medida actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        using var form = new UnitMeasureEditForm(item, true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Unidad de medida copiada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item) return;
        if (!Confirm($"¿Eliminar la unidad de medida {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm(
            "Historial de unidad de medida",
            $"{item.Code} - {item.Name}",
            async cancellationToken =>
            {
                var changes = await viewModel.GetHistoryAsync(item.Id, cancellationToken);
                return changes.Select(change => new SecurityChangeItem(
                    0,
                    "UnitMeasure",
                    change.RecordId,
                    change.Action,
                    change.FieldName,
                    change.OldValue,
                    change.NewValue,
                    change.UserId,
                    change.UserName,
                    "API",
                    change.CreatedAt)).ToList();
            });
        form.ShowDialog(this);
        return Task.CompletedTask;
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        Column(nameof(UnitMeasureItem.Code), "Código", 1, 100);
        Column(nameof(UnitMeasureItem.Name), "Nombre", 2, 210);
        Column(nameof(UnitMeasureItem.Symbol), "Símbolo", 3, 85);
        Column(nameof(UnitMeasureItem.MagnitudeName), "Tipo de magnitud", 4, 125);
        Column(nameof(UnitMeasureItem.Description), "Descripción", 5, 270);
        Column(nameof(UnitMeasureItem.SortOrder), "Orden", 6, 70, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(UnitMeasureItem.ExternalSystem), "Sistema externo", 7, 115);
        Column(nameof(UnitMeasureItem.ExternalCode), "Código externo", 8, 120);
        Column(nameof(UnitMeasureItem.IsActive), "Activo", 9, 70);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private UnitMeasureItem? SelectedItem() => SelectedGridItem<UnitMeasureItem>();

    private void Column(
        string field,
        string caption,
        int index,
        int width,
        DevExpress.Utils.HorzAlignment? alignment = null)
    {
        if (GridView.Columns[field] is not { } column) return;
        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = index;
        column.Width = width;
        column.OptionsColumn.AllowEdit = false;
        if (alignment.HasValue) column.AppearanceCell.TextOptions.HAlignment = alignment.Value;
    }

    private void ConfigureWindow()
    {
        ClientSize = new Size(1100, 640);
        MinimumSize = new Size(900, 520);
        Name = nameof(UnitMeasuresForm);
        Text = "Unidades de medida";
    }

    private void WirePermissions()
    {
        if (session is not null) ConfigureCrudPermissions(session, CrudOperationPermissions.UnitMeasures);
    }
}

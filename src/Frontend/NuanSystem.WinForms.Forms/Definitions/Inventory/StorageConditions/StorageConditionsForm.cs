using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.StorageConditions;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.StorageConditions;

public sealed class StorageConditionsForm : BaseGridCrudListForm
{
    public const string FormKey = "storage-conditions";
    private readonly StorageConditionsViewModel viewModel;
    private readonly ApiSession session;
    public StorageConditionsForm() { viewModel = null!; session = null!; ConfigureWindow(); WirePermissions(); }
    public StorageConditionsForm(StorageConditionsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    { this.viewModel = viewModel; this.session = session; ConfigureWindow(); ConfigureColumnPersonalization(columnSettingsClient, FormKey); WirePermissions(); }
    protected override async Task LoadDataAsync() { if (viewModel is null) return; await RunWithBusyStateAsync(async () => { await viewModel.LoadAsync(); SetGridData(viewModel.Items); await ApplyColumnSettingsAsync(); }); }
    protected override async Task CreateAsync() { using var form = new StorageConditionEditForm(); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.CreateAsync(form.Request); ShowSuccess("Condición de almacenamiento creada correctamente."); await LoadDataAsync(); }
    protected override async Task EditAsync() { if (SelectedItem() is not { } selected) return; var item = await viewModel.GetByIdAsync(selected.Id); using var form = new StorageConditionEditForm(item); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.UpdateAsync(item.Id, form.Request); ShowSuccess("Condición de almacenamiento actualizada correctamente."); await LoadDataAsync(); }
    protected override async Task CopyAsync() { if (SelectedItem() is not { } selected) return; var item = await viewModel.GetByIdAsync(selected.Id); using var form = new StorageConditionEditForm(item, true); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.CreateAsync(form.Request); ShowSuccess("Condición de almacenamiento copiada correctamente."); await LoadDataAsync(); }
    protected override async Task DeleteAsync() { if (SelectedItem() is not { } item || !Confirm($"¿Eliminar la condición de almacenamiento {item.Code}?")) return; await viewModel.DeleteAsync(item.Id); await LoadDataAsync(); }
    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm("Historial de condición de almacenamiento", $"{item.Code} - {item.Name}", async ct =>
        {
            var changes = await viewModel.GetHistoryAsync(item.Id, ct);
            return changes.Select(x => new SecurityChangeItem(0, "StorageCondition", item.Id.ToString(), x.Action, x.FieldName, x.OldValue, x.NewValue, x.UserId, x.UserName, "API", x.CreatedAt)).ToList();
        });
        form.ShowDialog(this); return Task.CompletedTask;
    }
    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns(); foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        Column(nameof(StorageConditionItem.Code), "Código", 1, 130); Column(nameof(StorageConditionItem.Name), "Nombre", 2, 280);
        Column(nameof(StorageConditionItem.Description), "Descripción", 3, 420); Column(nameof(StorageConditionItem.SortOrder), "Orden", 4, 80, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(StorageConditionItem.IsActive), "Activo", 5, 80, DevExpress.Utils.HorzAlignment.Center); GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }
    private void Column(string field, string caption, int index, int width, DevExpress.Utils.HorzAlignment? alignment = null)
    { if (GridView.Columns[field] is not { } column) return; column.Caption = caption; column.Visible = true; column.VisibleIndex = index; column.Width = width; column.OptionsColumn.AllowEdit = false; if (alignment.HasValue) column.AppearanceCell.TextOptions.HAlignment = alignment.Value; }
    private StorageConditionItem? SelectedItem() => SelectedGridItem<StorageConditionItem>();
    private void ConfigureWindow() { ClientSize = new Size(1100, 620); MinimumSize = new Size(900, 520); Name = nameof(StorageConditionsForm); Text = "Condiciones de almacenamiento"; }
    private void WirePermissions() { if (session is not null) ConfigureCrudPermissions(session, CrudOperationPermissions.StorageConditions); }
}

using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemAlertTypes.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemAlertTypes;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemAlertTypes;

public sealed class ItemAlertTypesForm : BaseGridCrudListForm
{
    public const string FormKey = "item-alert-types";
    private readonly ItemAlertTypesViewModel viewModel;
    private readonly ApiSession session;
    public ItemAlertTypesForm() { viewModel = null!; session = null!; ConfigureWindow(); WirePermissions(); }
    public ItemAlertTypesForm(ItemAlertTypesViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    { this.viewModel = viewModel; this.session = session; ConfigureWindow(); ConfigureColumnPersonalization(columnSettingsClient, FormKey); WirePermissions(); }
    protected override async Task LoadDataAsync()
    { if (viewModel is null) return; await RunWithBusyStateAsync(async () => { await viewModel.LoadAsync(); SetGridData(viewModel.Items); await ApplyColumnSettingsAsync(); }); }
    protected override async Task CreateAsync()
    { using var form = new ItemAlertTypeEditForm(); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.CreateAsync(form.Request); await LoadDataAsync(); }
    protected override async Task EditAsync()
    { if (SelectedGridItem<ItemAlertTypeItem>() is not { } selected) return; var item = await viewModel.GetByIdAsync(selected.Id); using var form = new ItemAlertTypeEditForm(item); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.UpdateAsync(item.Id, form.Request); await LoadDataAsync(); }
    protected override async Task CopyAsync()
    { if (SelectedGridItem<ItemAlertTypeItem>() is not { } selected) return; var item = await viewModel.GetByIdAsync(selected.Id); using var form = new ItemAlertTypeEditForm(item, true); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.CreateAsync(form.Request); await LoadDataAsync(); }
    protected override async Task DeleteAsync()
    { if (SelectedGridItem<ItemAlertTypeItem>() is not { } item || !Confirm($"¿Eliminar {item.Code}?") ) return; await viewModel.DeleteAsync(item.Id); await LoadDataAsync(); }
    protected override Task HistoryAsync()
    {
        if (SelectedGridItem<ItemAlertTypeItem>() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm("Historial de Tipos de alerta de artículos", $"{item.Code} - {item.Name}", async ct =>
        {
            var changes = await viewModel.GetHistoryAsync(item.Id, ct);
            return changes.Select(change => new SecurityChangeItem(0, "ItemAlertType", item.Id.ToString(), change.Action, change.FieldName, change.OldValue, change.NewValue, change.UserId, change.UserName, "API", change.CreatedAt)).ToList();
        });
        form.ShowDialog(this);
        return Task.CompletedTask;
    }
    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        Column(nameof(ItemAlertTypeItem.Code), "Código", 1, 130);
        Column(nameof(ItemAlertTypeItem.Name), "Nombre", 2, 260);
        Column(nameof(ItemAlertTypeItem.Description), "Descripción", 3, 420);
        Column(nameof(ItemAlertTypeItem.SortOrder), "Orden", 4, 80, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(ItemAlertTypeItem.IsActive), "Activo", 5, 80, DevExpress.Utils.HorzAlignment.Center);

        HiddenColumn(nameof(ItemAlertTypeItem.Id), "Id", 70);
        HiddenColumn(nameof(ItemAlertTypeItem.GlobalId), "Id global", 240);
        HiddenColumn(nameof(ItemAlertTypeItem.CreatedByUserId), "Id usuario creación", 120);
        HiddenColumn(nameof(ItemAlertTypeItem.CreatedByUserName), "Usuario creación", 160);
        HiddenColumn(nameof(ItemAlertTypeItem.CreatedAt), "Fecha creación", 145, "dd/MM/yyyy HH:mm");
        HiddenColumn(nameof(ItemAlertTypeItem.UpdatedByUserId), "Id usuario modificación", 135);
        HiddenColumn(nameof(ItemAlertTypeItem.UpdatedByUserName), "Usuario modificación", 170);
        HiddenColumn(nameof(ItemAlertTypeItem.UpdatedAt), "Fecha modificación", 145, "dd/MM/yyyy HH:mm");
        HiddenColumn(nameof(ItemAlertTypeItem.IsDeleted), "Eliminado", 85);
        HiddenColumn(nameof(ItemAlertTypeItem.DeletedByUserId), "Id usuario eliminación", 130);
        HiddenColumn(nameof(ItemAlertTypeItem.DeletedByUserName), "Usuario eliminación", 165);
        HiddenColumn(nameof(ItemAlertTypeItem.DeletedAt), "Fecha eliminación", 145, "dd/MM/yyyy HH:mm");
    }
    private void Column(string field, string caption, int index, int width, DevExpress.Utils.HorzAlignment? alignment = null)
    { var column = GridView.Columns[field] ?? GridView.Columns.AddField(field); column.Caption = caption; column.Visible = true; column.VisibleIndex = index; column.Width = width; column.OptionsColumn.AllowEdit = false; if (alignment.HasValue) column.AppearanceCell.TextOptions.HAlignment = alignment.Value; }
    private void HiddenColumn(string field, string caption, int width, string? displayFormat = null)
    { var column = GridView.Columns[field] ?? GridView.Columns.AddField(field); column.Caption = caption; column.Visible = false; column.Width = width; column.OptionsColumn.AllowEdit = false; if (!string.IsNullOrWhiteSpace(displayFormat)) { column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime; column.DisplayFormat.FormatString = displayFormat; } }
    private void ConfigureWindow() { ClientSize = new Size(1100, 620); MinimumSize = new Size(900, 520); Name = nameof(ItemAlertTypesForm); Text = "Tipos de alerta de artículos"; }
    private void WirePermissions()
    {
        if (session is not null)
        {
            ConfigureCrudPermissions(session, new CrudOperationPermissions(
                PermissionCodes.GeneralInventoryItemAlertTypesRead,
                PermissionCodes.GeneralInventoryItemAlertTypesManage,
                PermissionCodes.GeneralInventoryItemAlertTypesManage,
                PermissionCodes.GeneralInventoryItemAlertTypesManage));
        }
    }
}

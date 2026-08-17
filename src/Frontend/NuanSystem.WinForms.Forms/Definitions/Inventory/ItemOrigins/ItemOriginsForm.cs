using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemOrigins;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemOrigins;

public sealed class ItemOriginsForm : BaseGridCrudListForm
{
    public const string FormKey = "item-origins";
    private readonly ItemOriginsViewModel viewModel;
    private readonly ApiSession session;
    public ItemOriginsForm() { viewModel = null!; session = null!; ConfigureWindow(); WirePermissions(); }
    public ItemOriginsForm(ItemOriginsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    { this.viewModel = viewModel; this.session = session; ConfigureWindow(); ConfigureColumnPersonalization(columnSettingsClient, FormKey); WirePermissions(); }
    protected override async Task LoadDataAsync()
    { if (viewModel is null) return; await RunWithBusyStateAsync(async () => { await viewModel.LoadAsync(); SetGridData(viewModel.Items); await ApplyColumnSettingsAsync(); }); }
    protected override async Task CreateAsync()
    { using var form = new ItemOriginEditForm(); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.CreateAsync(form.Request); ShowSuccess("Origen de artículos creado correctamente."); await LoadDataAsync(); }
    protected override async Task EditAsync()
    { if (SelectedItem() is not { } selected) return; var item = await viewModel.GetByIdAsync(selected.Id); using var form = new ItemOriginEditForm(item); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.UpdateAsync(item.Id, form.Request); ShowSuccess("Origen de artículos actualizado correctamente."); await LoadDataAsync(); }
    protected override async Task CopyAsync()
    { if (SelectedItem() is not { } selected) return; var item = await viewModel.GetByIdAsync(selected.Id); using var form = new ItemOriginEditForm(item, copyMode: true); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.CreateAsync(form.Request); ShowSuccess("Origen de artículos copiado correctamente."); await LoadDataAsync(); }
    protected override async Task DeleteAsync()
    { if (SelectedItem() is not { } item || !Confirm($"¿Eliminar el origen de artículos {item.Code}?") ) return; await viewModel.DeleteAsync(item.Id); await LoadDataAsync(); }
    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm(
            "Historial de origen de artículos", $"{item.Code} - {item.Name}",
            async cancellationToken =>
            {
                var changes = await viewModel.GetHistoryAsync(item.Id, cancellationToken);
                return changes.Select(change => new SecurityChangeItem(
                    0, "ItemOrigin", change.RecordId, change.Action, change.FieldName,
                    change.OldValue, change.NewValue, change.UserId, change.UserName,
                    "API", change.CreatedAt)).ToList();
            });
        form.ShowDialog(this);
        return Task.CompletedTask;
    }
    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        Column(nameof(ItemOriginItem.Code), "Código", 1, 130);
        Column(nameof(ItemOriginItem.Name), "Nombre", 2, 280);
        Column(nameof(ItemOriginItem.Description), "Descripción", 3, 420);
        Column(nameof(ItemOriginItem.SortOrder), "Orden", 4, 80, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(ItemOriginItem.IsActive), "Activo", 5, 80, DevExpress.Utils.HorzAlignment.Center);
        HiddenColumn(nameof(ItemOriginItem.Id), "Id", 70);
        HiddenColumn(nameof(ItemOriginItem.GlobalId), "Id global", 240);
        HiddenColumn(nameof(ItemOriginItem.CreatedByUserId), "Id usuario creación", 120);
        HiddenColumn(nameof(ItemOriginItem.CreatedByUserName), "Usuario creación", 160);
        HiddenColumn(nameof(ItemOriginItem.CreatedAt), "Fecha creación", 145, "dd/MM/yyyy HH:mm");
        HiddenColumn(nameof(ItemOriginItem.UpdatedByUserId), "Id usuario modificación", 135);
        HiddenColumn(nameof(ItemOriginItem.UpdatedByUserName), "Usuario modificación", 170);
        HiddenColumn(nameof(ItemOriginItem.UpdatedAt), "Fecha modificación", 145, "dd/MM/yyyy HH:mm");
        HiddenColumn(nameof(ItemOriginItem.IsDeleted), "Eliminado", 85);
        HiddenColumn(nameof(ItemOriginItem.DeletedByUserId), "Id usuario eliminación", 130);
        HiddenColumn(nameof(ItemOriginItem.DeletedByUserName), "Usuario eliminación", 165);
        HiddenColumn(nameof(ItemOriginItem.DeletedAt), "Fecha eliminación", 145, "dd/MM/yyyy HH:mm");
    }
    private void Column(string field, string caption, int index, int width, DevExpress.Utils.HorzAlignment? alignment = null)
    { var column = GridView.Columns[field] ?? GridView.Columns.AddField(field); column.Caption = caption; column.Visible = true; column.VisibleIndex = index; column.Width = width; column.OptionsColumn.AllowEdit = false; if (alignment.HasValue) column.AppearanceCell.TextOptions.HAlignment = alignment.Value; }
    private void HiddenColumn(string field, string caption, int width, string? displayFormat = null)
    { var column = GridView.Columns[field] ?? GridView.Columns.AddField(field); column.Caption = caption; column.Visible = false; column.Width = width; column.OptionsColumn.AllowEdit = false; if (!string.IsNullOrWhiteSpace(displayFormat)) { column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime; column.DisplayFormat.FormatString = displayFormat; } }
    private void ConfigureWindow() { ClientSize = new Size(1100, 620); MinimumSize = new Size(900, 520); Name = nameof(ItemOriginsForm); Text = "Orígenes de artículos"; }
    private ItemOriginItem? SelectedItem() => SelectedGridItem<ItemOriginItem>();
    private void WirePermissions()
    {
        if (session is not null) ConfigureCrudPermissions(session, CrudOperationPermissions.ItemOrigins);
    }
}

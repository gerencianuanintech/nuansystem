using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.SalesChannels.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.SalesChannels;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.SalesChannels;

public sealed class SalesChannelsForm : BaseGridCrudListForm
{
    public const string FormKey = "sales-channels";
    private readonly SalesChannelsViewModel viewModel;
    private readonly ApiSession session;
    public SalesChannelsForm() { viewModel = null!; session = null!; ConfigureWindow(); WirePermissions(); }
    public SalesChannelsForm(SalesChannelsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    { this.viewModel = viewModel; this.session = session; ConfigureWindow(); ConfigureColumnPersonalization(columnSettingsClient, FormKey); WirePermissions(); }
    protected override async Task LoadDataAsync()
    { if (viewModel is null) return; await RunWithBusyStateAsync(async () => { await viewModel.LoadAsync(); SetGridData(viewModel.Items); await ApplyColumnSettingsAsync(); }); }
    protected override async Task CreateAsync()
    { using var form = new SalesChannelEditForm(); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.CreateAsync(form.Request); await LoadDataAsync(); }
    protected override async Task EditAsync()
    { if (SelectedGridItem<SalesChannelItem>() is not { } selected) return; var item = await viewModel.GetByIdAsync(selected.Id); using var form = new SalesChannelEditForm(item); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.UpdateAsync(item.Id, form.Request); await LoadDataAsync(); }
    protected override async Task CopyAsync()
    { if (SelectedGridItem<SalesChannelItem>() is not { } selected) return; var item = await viewModel.GetByIdAsync(selected.Id); using var form = new SalesChannelEditForm(item, true); if (form.ShowDialog(this) != DialogResult.OK) return; await viewModel.CreateAsync(form.Request); await LoadDataAsync(); }
    protected override async Task DeleteAsync()
    { if (SelectedGridItem<SalesChannelItem>() is not { } item || !Confirm($"¿Eliminar {item.Code}?") ) return; await viewModel.DeleteAsync(item.Id); await LoadDataAsync(); }
    protected override Task HistoryAsync()
    {
        if (SelectedGridItem<SalesChannelItem>() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm("Historial de Canales de venta", $"{item.Code} - {item.Name}", async ct =>
        {
            var changes = await viewModel.GetHistoryAsync(item.Id, ct);
            return changes.Select(change => new SecurityChangeItem(0, "SalesChannel", item.Id.ToString(), change.Action, change.FieldName, change.OldValue, change.NewValue, change.UserId, change.UserName, "API", change.CreatedAt)).ToList();
        });
        form.ShowDialog(this);
        return Task.CompletedTask;
    }
    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns) column.Visible = false;
        Column(nameof(SalesChannelItem.Code), "Código", 1, 130);
        Column(nameof(SalesChannelItem.Name), "Nombre", 2, 260);
        Column(nameof(SalesChannelItem.Description), "Descripción", 3, 420);
        Column(nameof(SalesChannelItem.SortOrder), "Orden", 4, 80, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(SalesChannelItem.IsActive), "Activo", 5, 80);

        HiddenColumn(nameof(SalesChannelItem.Id), "Id", 70);
        HiddenColumn(nameof(SalesChannelItem.GlobalId), "Id global", 240);
        HiddenColumn(nameof(SalesChannelItem.CreatedByUserId), "Id usuario creación", 120);
        HiddenColumn(nameof(SalesChannelItem.CreatedByUserName), "Usuario creación", 160);
        HiddenColumn(nameof(SalesChannelItem.CreatedAt), "Fecha creación", 145, "dd/MM/yyyy HH:mm");
        HiddenColumn(nameof(SalesChannelItem.UpdatedByUserId), "Id usuario modificación", 135);
        HiddenColumn(nameof(SalesChannelItem.UpdatedByUserName), "Usuario modificación", 170);
        HiddenColumn(nameof(SalesChannelItem.UpdatedAt), "Fecha modificación", 145, "dd/MM/yyyy HH:mm");
        HiddenColumn(nameof(SalesChannelItem.IsDeleted), "Eliminado", 85);
        HiddenColumn(nameof(SalesChannelItem.DeletedByUserId), "Id usuario eliminación", 130);
        HiddenColumn(nameof(SalesChannelItem.DeletedByUserName), "Usuario eliminación", 165);
        HiddenColumn(nameof(SalesChannelItem.DeletedAt), "Fecha eliminación", 145, "dd/MM/yyyy HH:mm");
    }
    private void Column(string field, string caption, int index, int width, DevExpress.Utils.HorzAlignment? alignment = null)
    { var column = GridView.Columns[field] ?? GridView.Columns.AddField(field); column.Caption = caption; column.Visible = true; column.VisibleIndex = index; column.Width = width; column.OptionsColumn.AllowEdit = false; if (alignment.HasValue) column.AppearanceCell.TextOptions.HAlignment = alignment.Value; }
    private void HiddenColumn(string field, string caption, int width, string? displayFormat = null)
    { var column = GridView.Columns[field] ?? GridView.Columns.AddField(field); column.Caption = caption; column.Visible = false; column.Width = width; column.OptionsColumn.AllowEdit = false; if (!string.IsNullOrWhiteSpace(displayFormat)) { column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime; column.DisplayFormat.FormatString = displayFormat; } }
    private void ConfigureWindow() { ClientSize = new Size(1100, 620); MinimumSize = new Size(900, 520); Name = nameof(SalesChannelsForm); Text = "Canales de venta"; }
    private void WirePermissions()
    {
        if (session is null) return;
        ConfigureCrudPermissions(session, new CrudOperationPermissions(
            PermissionCodes.GeneralInventorySalesChannelsRead,
            PermissionCodes.GeneralInventorySalesChannelsManage,
            PermissionCodes.GeneralInventorySalesChannelsManage,
            PermissionCodes.GeneralInventorySalesChannelsManage));
    }
}



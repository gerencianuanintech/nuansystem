using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemTypes.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemTypes;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemTypes;

public sealed class ItemTypesForm : BaseGridCrudListForm
{
    public const string FormKey = "inventory-item-types";
    private static readonly CrudOperationPermissions Permissions = new(
        PermissionCodes.GeneralInventoryItemTypesRead,
        PermissionCodes.GeneralInventoryItemTypesManage,
        PermissionCodes.GeneralInventoryItemTypesManage,
        PermissionCodes.GeneralInventoryItemTypesManage);

    private readonly ItemTypesViewModel viewModel;
    private readonly ApiSession session;

    public ItemTypesForm()
    {
        viewModel = null!;
        session = null!;
        ConfigureWindow();
        WirePermissions();
    }

    public ItemTypesForm(
        ItemTypesViewModel viewModel,
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
        using var form = new ItemTypeEditForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Tipo de ítem creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item) return;
        var detail = await viewModel.GetByIdAsync(item.Id);
        using var form = new ItemTypeEditForm(detail);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Tipo de ítem actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item) return;
        var detail = await viewModel.GetByIdAsync(item.Id);
        using var form = new ItemTypeEditForm(detail, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Tipo de ítem copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item) return;
        if (item.IsSystem)
        {
            ShowWarning("Los tipos de ítem del sistema no se pueden eliminar.");
            return;
        }

        if (!Confirm($"¿Eliminar el tipo de ítem {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm(
            "Historial de tipo de ítem",
            $"{item.Code} - {item.Name}",
            async cancellationToken =>
            {
                var changes = await viewModel.GetHistoryAsync(item.Id, cancellationToken);
                return changes.Select(change => new SecurityChangeItem(
                    0, "ItemType", change.RecordId, change.Action, change.FieldName,
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
        ConfigureColumn(nameof(ItemTypeItem.Code), "Código", 1, 100);
        ConfigureColumn(nameof(ItemTypeItem.Name), "Nombre", 2, 220);
        ConfigureColumn(nameof(ItemTypeItem.BehaviorCode), "Comportamiento", 3, 130);
        ConfigureColumn(nameof(ItemTypeItem.DefaultIsPurchaseItem), "Compra", 4, 80);
        ConfigureColumn(nameof(ItemTypeItem.DefaultIsSalesItem), "Venta", 5, 80);
        ConfigureColumn(nameof(ItemTypeItem.DefaultIsInventoryItem), "Inventario", 6, 90);
        ConfigureColumn(nameof(ItemTypeItem.SortOrder), "Orden", 7, 75);
        ConfigureColumn(nameof(ItemTypeItem.IsSystem), "Sistema", 8, 80);
        ConfigureColumn(nameof(ItemTypeItem.IsActive), "Activo", 9, 75);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ItemTypeItem? SelectedItem() => SelectedGridItem<ItemTypeItem>();

    private void ConfigureColumn(string fieldName, string caption, int visibleIndex, int width)
    {
        if (GridView.Columns[fieldName] is not { } column) return;
        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
        column.OptionsColumn.AllowEdit = false;
    }

    private void ConfigureWindow()
    {
        ClientSize = new Size(1100, 620);
        MinimumSize = new Size(900, 520);
        Name = nameof(ItemTypesForm);
        Text = "Tipos de ítem";
    }

    private void WirePermissions()
    {
        if (session is not null) ConfigureCrudPermissions(session, Permissions);
    }
}

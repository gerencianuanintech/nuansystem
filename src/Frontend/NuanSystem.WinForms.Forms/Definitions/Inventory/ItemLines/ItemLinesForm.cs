using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemLines;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemLines;

public sealed class ItemLinesForm : BaseGridCrudListForm
{
    public const string FormKey = "item-lines";

    private static readonly CrudOperationPermissions Permissions = new(
        PermissionCodes.GeneralInventoryItemLinesRead,
        PermissionCodes.GeneralInventoryItemLinesManage,
        PermissionCodes.GeneralInventoryItemLinesManage,
        PermissionCodes.GeneralInventoryItemLinesManage);

    private readonly ItemLinesViewModel viewModel;
    private readonly ApiSession session;

    public ItemLinesForm()
    {
        viewModel = null!;
        session = null!;
        ConfigureWindow();
        WirePermissions();
    }

    public ItemLinesForm(
        ItemLinesViewModel viewModel,
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
        using var form = new ItemLineEditForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Línea de artículos creada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } selected) return;

        var item = await viewModel.GetByIdAsync(selected.Id);
        using var form = new ItemLineEditForm(item);
        if (form.ShowDialog(this) != DialogResult.OK) return;

        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Línea de artículos actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } selected) return;

        var item = await viewModel.GetByIdAsync(selected.Id);
        using var form = new ItemLineEditForm(item, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK) return;

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Línea de artículos copiada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item) return;
        if (!Confirm($"¿Eliminar la línea de artículos {item.Code}?")) return;

        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;

        using var form = new RecordHistoryForm(
            "Historial de línea de artículos",
            $"{item.Code} - {item.Name}",
            async cancellationToken =>
            {
                var changes = await viewModel.GetHistoryAsync(item.Id, cancellationToken);
                return changes.Select(change => new SecurityChangeItem(
                    0,
                    "ItemLine",
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
        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        Column(nameof(ItemLineItem.Code), "Código", 1, 130);
        Column(nameof(ItemLineItem.Name), "Nombre", 2, 280);
        Column(nameof(ItemLineItem.Description), "Descripción", 3, 420);
        Column(nameof(ItemLineItem.SortOrder), "Orden", 4, 80, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(ItemLineItem.IsActive), "Activo", 5, 80);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ItemLineItem? SelectedItem() => SelectedGridItem<ItemLineItem>();

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
        if (alignment.HasValue)
        {
            column.AppearanceCell.TextOptions.HAlignment = alignment.Value;
        }
    }

    private void ConfigureWindow()
    {
        ClientSize = new Size(1100, 620);
        MinimumSize = new Size(900, 520);
        Name = nameof(ItemLinesForm);
        Text = "Líneas de artículos";
    }

    private void WirePermissions()
    {
        if (session is not null)
        {
            ConfigureCrudPermissions(session, Permissions);
        }
    }
}

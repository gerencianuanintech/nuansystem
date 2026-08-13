using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.Definitions.Inventory.ItemGroups;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemFamilies;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemFamilies;

public sealed class ItemFamiliesForm : BaseGridCrudListForm
{
    public const string FormKey = "item-families";
    private readonly ItemFamiliesViewModel viewModel;
    private readonly ApiSession session;

    public ItemFamiliesForm()
    {
        viewModel = null!;
        session = null!;
        ConfigureWindow();
        WirePermissions();
    }

    public ItemFamiliesForm(
        ItemFamiliesViewModel viewModel,
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
        await viewModel.LoadEditorContextAsync();
        using var form = CreateEditor();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Familia de artículos creada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        await viewModel.LoadEditorContextAsync(item.ItemGroupId, item.ItemGroupCode, item.ItemGroupName);
        using var form = CreateEditor(item);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Familia de artículos actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        await viewModel.LoadEditorContextAsync(item.ItemGroupId, item.ItemGroupCode, item.ItemGroupName);
        using var form = CreateEditor(item, true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Familia de artículos copiada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item) return;
        if (!Confirm($"¿Eliminar la familia de artículos {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm(
            "Historial de familia de artículos",
            $"{item.ItemGroupCode} / {item.Code} - {item.Name}",
            async cancellationToken =>
            {
                var changes = await viewModel.GetHistoryAsync(item.Id, cancellationToken);
                return changes.Select(change => new SecurityChangeItem(
                    0, "ItemFamily", change.RecordId, change.Action, change.FieldName,
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
        Column(nameof(ItemFamilyItem.ItemGroupCode), "Grupo", 1, 105);
        Column(nameof(ItemFamilyItem.ItemGroupName), "Nombre del grupo", 2, 180);
        Column(nameof(ItemFamilyItem.Code), "Código", 3, 105);
        Column(nameof(ItemFamilyItem.Name), "Nombre", 4, 220);
        Column(nameof(ItemFamilyItem.Description), "Descripción", 5, 260);
        Column(nameof(ItemFamilyItem.SortOrder), "Orden", 6, 70, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(ItemFamilyItem.ExternalSystem), "Sistema externo", 7, 115);
        Column(nameof(ItemFamilyItem.ExternalCode), "Código externo", 8, 120);
        Column(nameof(ItemFamilyItem.IsActive), "Activo", 9, 70);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ItemFamilyEditForm CreateEditor(ItemFamilyItem? item = null, bool copyMode = false)
    {
        var form = new ItemFamilyEditForm(
            viewModel.ItemGroupLookups,
            viewModel.CanCreateItemGroups,
            viewModel.CanEditItemGroups,
            item,
            copyMode);
        form.CreateItemGroupRequested += CreateItemGroupAsync;
        form.EditItemGroupRequested += EditItemGroupAsync;
        return form;
    }

    private async Task<ItemGroupItem?> CreateItemGroupAsync(ItemFamilyEditForm owner)
    {
        if (!viewModel.CanCreateItemGroups) return null;
        using var form = new ItemGroupEditForm(viewModel.AccountLookups, false, false);
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.CreateItemGroupAsync(form.Request);
        await viewModel.LoadEditorContextAsync(saved.Id, saved.Code, saved.Name);
        owner.RefreshItemGroups(viewModel.ItemGroupLookups, saved.Id);
        ShowSuccess("Grupo de artículos creado correctamente.");
        return saved;
    }

    private async Task<ItemGroupItem?> EditItemGroupAsync(ItemFamilyEditForm owner, int id)
    {
        if (!viewModel.CanEditItemGroups) return null;
        var item = await viewModel.GetItemGroupByIdAsync(id);
        using var form = new ItemGroupEditForm(viewModel.AccountLookups, false, false, item);
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.UpdateItemGroupAsync(item.Id, form.Request);
        await viewModel.LoadEditorContextAsync(saved.Id, saved.Code, saved.Name);
        owner.RefreshItemGroups(viewModel.ItemGroupLookups, saved.Id);
        ShowSuccess("Grupo de artículos actualizado correctamente.");
        return saved;
    }

    private ItemFamilyItem? SelectedItem() => SelectedGridItem<ItemFamilyItem>();

    private void Column(string field, string caption, int index, int width, DevExpress.Utils.HorzAlignment? alignment = null)
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
        Name = nameof(ItemFamiliesForm);
        Text = "Familias de artículos";
    }

    private void WirePermissions()
    {
        if (session is not null) ConfigureCrudPermissions(session, CrudOperationPermissions.ItemFamilies);
    }
}

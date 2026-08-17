using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.Definitions.Inventory.ItemFamilies;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemSubgroups.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemSubgroups;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemSubgroups;

public sealed class ItemSubgroupsForm : BaseGridCrudListForm
{
    public const string FormKey = "item-subgroups";
    private readonly ItemSubgroupsViewModel viewModel;
    private readonly ApiSession session;

    public ItemSubgroupsForm()
    {
        viewModel = null!;
        session = null!;
        ConfigureWindow();
        WirePermissions();
    }

    public ItemSubgroupsForm(ItemSubgroupsViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
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
        ShowSuccess("Subgrupo de artículos creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        await viewModel.LoadEditorContextAsync(item.ItemFamilyId, item.ItemFamilyCode, item.ItemFamilyName);
        using var form = CreateEditor(item);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Subgrupo de artículos actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        await viewModel.LoadEditorContextAsync(item.ItemFamilyId, item.ItemFamilyCode, item.ItemFamilyName);
        using var form = CreateEditor(item, true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Subgrupo de artículos copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item || !Confirm($"¿Eliminar el subgrupo {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm(
            "Historial de subgrupo de artículos", $"{item.ItemFamilyCode} / {item.Code} - {item.Name}",
            async cancellationToken =>
            {
                var changes = await viewModel.GetHistoryAsync(item.Id, cancellationToken);
                return changes.Select(change => new SecurityChangeItem(
                    0, "ItemSubgroup", change.RecordId, change.Action, change.FieldName,
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
        Column(nameof(ItemSubgroupItem.ItemFamilyCode), "Familia", 1, 110);
        Column(nameof(ItemSubgroupItem.ItemFamilyName), "Nombre de familia", 2, 190);
        Column(nameof(ItemSubgroupItem.Code), "Código", 3, 110);
        Column(nameof(ItemSubgroupItem.Name), "Nombre", 4, 220);
        Column(nameof(ItemSubgroupItem.Description), "Descripción", 5, 280);
        Column(nameof(ItemSubgroupItem.SortOrder), "Orden", 6, 70, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(ItemSubgroupItem.IsActive), "Activo", 7, 70);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ItemSubgroupEditForm CreateEditor(ItemSubgroupItem? item = null, bool copyMode = false)
    {
        var form = new ItemSubgroupEditForm(viewModel.ItemFamilyLookups,
            viewModel.CanCreateItemFamilies, viewModel.CanEditItemFamilies, item, copyMode);
        form.CreateItemFamilyRequested += CreateItemFamilyAsync;
        form.EditItemFamilyRequested += EditItemFamilyAsync;
        return form;
    }

    private async Task<ItemFamilyItem?> CreateItemFamilyAsync(ItemSubgroupEditForm owner)
    {
        if (!viewModel.CanCreateItemFamilies) return null;
        using var form = new ItemFamilyEditForm(viewModel.ItemGroupLookups, false, false);
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.CreateItemFamilyAsync(form.Request);
        await viewModel.LoadEditorContextAsync(saved.Id, saved.Code, saved.Name);
        owner.RefreshItemFamilies(viewModel.ItemFamilyLookups, saved.Id);
        ShowSuccess("Familia de artículos creada correctamente.");
        return saved;
    }

    private async Task<ItemFamilyItem?> EditItemFamilyAsync(ItemSubgroupEditForm owner, int id)
    {
        if (!viewModel.CanEditItemFamilies) return null;
        var item = await viewModel.GetItemFamilyByIdAsync(id);
        using var form = new ItemFamilyEditForm(viewModel.ItemGroupLookups, false, false, item);
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.UpdateItemFamilyAsync(item.Id, form.Request);
        await viewModel.LoadEditorContextAsync(saved.Id, saved.Code, saved.Name);
        owner.RefreshItemFamilies(viewModel.ItemFamilyLookups, saved.Id);
        ShowSuccess("Familia de artículos actualizada correctamente.");
        return saved;
    }

    private ItemSubgroupItem? SelectedItem() => SelectedGridItem<ItemSubgroupItem>();

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
        Name = nameof(ItemSubgroupsForm);
        Text = "Subgrupos de artículos";
    }

    private void WirePermissions()
    {
        if (session is not null) ConfigureCrudPermissions(session, CrudOperationPermissions.ItemSubgroups);
    }
}

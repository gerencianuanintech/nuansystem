using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.ItemBrands;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemBrands;

public sealed class ItemBrandsForm : BaseGridCrudListForm
{
    public const string FormKey = "item-brands";
    private readonly ItemBrandsViewModel viewModel;
    private readonly ApiSession session;

    public ItemBrandsForm()
    {
        viewModel = null!;
        session = null!;
        ConfigureWindow();
        WirePermissions();
    }

    public ItemBrandsForm(
        ItemBrandsViewModel viewModel,
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
        using var form = new ItemBrandEditForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Marca de artículos creada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        using var form = new ItemBrandEditForm(item);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Marca de artículos actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        using var form = new ItemBrandEditForm(item, true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Marca de artículos copiada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item) return;
        if (!Confirm($"¿Eliminar la marca de artículos {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm(
            "Historial de marca de artículos",
            $"{item.Code} - {item.Name}",
            async cancellationToken =>
            {
                var changes = await viewModel.GetHistoryAsync(item.Id, cancellationToken);
                return changes.Select(change => new SecurityChangeItem(
                    0,
                    "ItemBrand",
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
        Column(nameof(ItemBrandItem.Code), "Código", 1, 110);
        Column(nameof(ItemBrandItem.Name), "Nombre", 2, 220);
        Column(nameof(ItemBrandItem.Description), "Descripción", 3, 280);
        Column(nameof(ItemBrandItem.SortOrder), "Orden", 4, 70, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(ItemBrandItem.ExternalSystem), "Sistema externo", 5, 115);
        Column(nameof(ItemBrandItem.ExternalCode), "Código externo", 6, 120);
        Column(nameof(ItemBrandItem.SapManufacturerCode), "Fabricante SAP", 7, 120);
        Column(nameof(ItemBrandItem.SapCode), "Código SAP", 8, 105);
        Column(nameof(ItemBrandItem.IsActive), "Activo", 9, 70);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ItemBrandItem? SelectedItem() => SelectedGridItem<ItemBrandItem>();

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
        Name = nameof(ItemBrandsForm);
        Text = "Marcas de artículos";
    }

    private void WirePermissions()
    {
        if (session is not null) ConfigureCrudPermissions(session, CrudOperationPermissions.ItemBrands);
    }
}

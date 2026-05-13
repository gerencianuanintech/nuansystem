using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.InventoryItems.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.InventoryItems;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemsForm : BaseGridCrudListForm
{
    private readonly ItemsViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public ItemsForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public ItemsForm(ItemsViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, "items");
        WireEvents();
    }

    protected override async Task LoadDataAsync()
    {
        if (viewModel is null)
        {
            return;
        }

        await RunWithBusyStateAsync(async () =>
        {
            await viewModel.LoadLookupsAsync();
            await viewModel.LoadAsync();
            SetGridData(viewModel.Items);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override async Task CreateAsync()
    {
        using var form = new ItemEditForm(viewModel.Lookups);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Registro creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = new ItemEditForm(viewModel.Lookups, fullItem);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Registro actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        var fullItem = await viewModel.GetByIdAsync(item.Id);
        using var form = new ItemEditForm(viewModel.Lookups, fullItem, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Registro copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        if (!Confirm($"Eliminar el item {item.Code}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return Task.CompletedTask;
        }

        using var form = new RecordHistoryForm(
            "Historial de item",
            $"{item.Code} - {item.Name}",
            cancellationToken => auditClient.GetInventoryChangesAsync("Items", item.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(ItemItem.Code), "Codigo", 1, 120);
        ConfigureColumn(nameof(ItemItem.Name), "Nombre", 2, 220);
        ConfigureColumn(nameof(ItemItem.ItemGroupName), "Grupo", 3, 150);
        ConfigureColumn(nameof(ItemItem.ItemType), "Tipo", 4, 100);
        ConfigureColumn(nameof(ItemItem.IsPurchaseItem), "Compra", 5, 70);
        ConfigureColumn(nameof(ItemItem.IsSalesItem), "Venta", 6, 70);
        ConfigureColumn(nameof(ItemItem.IsInventoryItem), "Inventario", 7, 85);
        ConfigureColumn(nameof(ItemItem.ManagedBy), "Maneja por", 8, 100);
        ConfigureColumn(nameof(ItemItem.PurchaseTaxName), "Impuesto compra", 9, 150);
        ConfigureColumn(nameof(ItemItem.SalesTaxName), "Impuesto venta", 10, 150);
        ConfigureColumn(nameof(ItemItem.BaseSalesPrice), "Precio venta", 11, 100);
        ConfigureColumn(nameof(ItemItem.ReferenceCost), "Costo", 12, 100);
        ConfigureColumn(nameof(ItemItem.IsActive), "Activo", 13, 70);

        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ItemItem? SelectedItem()
    {
        return SelectedGridItem<ItemItem>();
    }

    private void ConfigureColumn(string fieldName, string caption, int visibleIndex, int width)
    {
        if (GridView.Columns[fieldName] is not { } column)
        {
            return;
        }

        column.Caption = caption;
        column.Visible = true;
        column.VisibleIndex = visibleIndex;
        column.Width = width;
    }

    private void WireEvents()
    {
        if (session is not null)
        {
            ConfigureCrudPermissions(session, CrudOperationPermissions.Items);
        }
    }
}

using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Forms.Definitions.Inventory.ItemOrigins;
using NuanSystem.WinForms.Forms.Definitions.Inventory.ReplenishmentMethods;
using NuanSystem.WinForms.Forms.Definitions.Inventory.StorageConditions;
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
    private readonly Func<string, Form?>? relatedCatalogFormFactory;

    public ItemsForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public ItemsForm(
        ItemsViewModel viewModel,
        ApiSession session,
        IAuditClient auditClient,
        IGridColumnSettingsClient columnSettingsClient,
        Func<string, Form?>? relatedCatalogFormFactory = null)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        this.relatedCatalogFormFactory = relatedCatalogFormFactory;
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
            await viewModel.LoadItemGroupCreateAccessAsync();
            await viewModel.LoadLookupsAsync();
            await viewModel.LoadAsync();
            SetGridData(viewModel.Items);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override async Task CreateAsync()
    {
        using var form = CreateEditForm();
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
        using var form = CreateEditForm(fullItem);
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
        using var form = CreateEditForm(fullItem, copyMode: true);
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
        ConfigureColumn(nameof(ItemItem.ItemFamilyName), "Linea/Familia", 4, 150);
        ConfigureColumn(nameof(ItemItem.ItemType), "Tipo", 5, 100);
        ConfigureColumn(nameof(ItemItem.IsPurchaseItem), "Compra", 6, 70);
        ConfigureColumn(nameof(ItemItem.IsSalesItem), "Venta", 7, 70);
        ConfigureColumn(nameof(ItemItem.IsInventoryItem), "Inventario", 8, 85);
        ConfigureColumn(nameof(ItemItem.ManagedBy), "Maneja por", 9, 100);
        ConfigureColumn(nameof(ItemItem.PurchaseTaxName), "Impuesto compra", 10, 150);
        ConfigureColumn(nameof(ItemItem.SalesTaxName), "Impuesto venta", 11, 150);
        ConfigureColumn(nameof(ItemItem.BaseSalesPrice), "Precio venta", 12, 100);
        ConfigureColumn(nameof(ItemItem.ReferenceCost), "Costo", 13, 100);
        ConfigureColumn(nameof(ItemItem.IsActive), "Activo", 14, 70);

        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ItemItem? SelectedItem()
    {
        return SelectedGridItem<ItemItem>();
    }

    private ItemEditForm CreateEditForm(ItemItem? item = null, bool copyMode = false)
    {
        return new ItemEditForm(
            viewModel.Lookups,
            item,
            copyMode,
            viewModel.CanCreateItemGroups,
            request => viewModel.CreateItemGroupAsync(request),
            viewModel.CanCreateItemFamilies,
            request => viewModel.CreateItemFamilyAsync(request),
            viewModel.CanCreateRelatedCatalog,
            relatedCatalogFormFactory,
            cancellationToken => viewModel.ReloadLookupsForEditAsync(cancellationToken),
            viewModel.CanEditRelatedCatalog(ItemOriginsForm.FormKey),
            CreateItemOriginAsync,
            EditItemOriginAsync,
            viewModel.CanEditRelatedCatalog(ReplenishmentMethodsForm.FormKey),
            CreateReplenishmentMethodAsync,
            EditReplenishmentMethodAsync,
            viewModel.CanEditRelatedCatalog(StorageConditionsForm.FormKey),
            CreateStorageConditionAsync,
            EditStorageConditionAsync);
    }

    private async Task<NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models.ItemOriginItem?> CreateItemOriginAsync(IWin32Window owner)
    {
        using var form = new ItemOriginEditForm();
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.CreateItemOriginAsync(form.Request);
        ShowSuccess("Origen de artículos creado correctamente.");
        return saved;
    }

    private async Task<NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models.ItemOriginItem?> EditItemOriginAsync(IWin32Window owner, int id)
    {
        var item = await viewModel.GetItemOriginByIdAsync(id);
        using var form = new ItemOriginEditForm(item);
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.UpdateItemOriginAsync(id, form.Request);
        ShowSuccess("Origen de artículos actualizado correctamente.");
        return saved;
    }

    private async Task<NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods.Models.ReplenishmentMethodItem?> CreateReplenishmentMethodAsync(IWin32Window owner)
    {
        using var form = new ReplenishmentMethodEditForm();
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.CreateReplenishmentMethodAsync(form.Request);
        ShowSuccess("Método de reposición creado correctamente.");
        return saved;
    }

    private async Task<NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods.Models.ReplenishmentMethodItem?> EditReplenishmentMethodAsync(IWin32Window owner, int id)
    {
        var item = await viewModel.GetReplenishmentMethodByIdAsync(id);
        using var form = new ReplenishmentMethodEditForm(item);
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.UpdateReplenishmentMethodAsync(id, form.Request);
        ShowSuccess("Método de reposición actualizado correctamente.");
        return saved;
    }

    private async Task<NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions.Models.StorageConditionItem?> CreateStorageConditionAsync(IWin32Window owner)
    {
        using var form = new StorageConditionEditForm();
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.CreateStorageConditionAsync(form.Request);
        ShowSuccess("Condición de almacenamiento creada correctamente.");
        return saved;
    }

    private async Task<NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions.Models.StorageConditionItem?> EditStorageConditionAsync(IWin32Window owner, int id)
    {
        var item = await viewModel.GetStorageConditionByIdAsync(id);
        using var form = new StorageConditionEditForm(item);
        if (form.ShowDialog(owner) != DialogResult.OK) return null;
        var saved = await viewModel.UpdateStorageConditionAsync(id, form.Request);
        ShowSuccess("Condición de almacenamiento actualizada correctamente.");
        return saved;
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

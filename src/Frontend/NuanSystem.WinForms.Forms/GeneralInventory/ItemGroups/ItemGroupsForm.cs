using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.GeneralInventory.ItemGroups.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.ItemGroups;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemGroups;

public sealed partial class ItemGroupsForm : BaseGridCrudListForm
{
    private readonly ItemGroupsViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public ItemGroupsForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public ItemGroupsForm(ItemGroupsViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, "item-groups");
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
            await viewModel.LoadAsync();
            SetGridData(viewModel.Items);
            await ApplyColumnSettingsAsync();
        });
    }

    protected override async Task CreateAsync()
    {
        await viewModel.LoadAccountLookupsAsync();
        using var form = new ItemGroupEditForm(viewModel.AccountLookups);
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
        await viewModel.LoadAccountLookupsAsync();
        using var form = new ItemGroupEditForm(fullItem, viewModel.AccountLookups);
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
        await viewModel.LoadAccountLookupsAsync();
        using var form = new ItemGroupEditForm(fullItem, viewModel.AccountLookups, copyMode: true);
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

        if (!Confirm($"Eliminar el grupo de artículos {item.Code}?"))
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
            "Historial de grupo de artículos",
            $"{item.Code} - {item.Name}",
            cancellationToken => auditClient.GetInventoryChangesAsync("ItemGroups", item.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(ItemGroupItem.Code), "Código", 1, 110);
        ConfigureColumn(nameof(ItemGroupItem.Name), "Nombre", 2, 210);
        ConfigureColumn(nameof(ItemGroupItem.Description), "Descripción", 3, 260);
        ConfigureColumn(nameof(ItemGroupItem.InventoryAccountCode), "Cuenta inventario", 4, 150);
        ConfigureColumn(nameof(ItemGroupItem.CostOfSalesAccountCode), "Cuenta costo ventas", 5, 160);
        ConfigureColumn(nameof(ItemGroupItem.SalesAccountCode), "Cuenta ventas", 6, 140);
        ConfigureColumn(nameof(ItemGroupItem.PurchaseAccountCode), "Cuenta compras", 7, 140);
        ConfigureColumn(nameof(ItemGroupItem.SapGroupCode), "Grupo SAP", 8, 120);
        ConfigureColumn(nameof(ItemGroupItem.SapCode), "Código SAP", 9, 110);
        ConfigureColumn(nameof(ItemGroupItem.IsActive), "Activo", 10, 70);

        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ItemGroupItem? SelectedItem()
    {
        return SelectedGridItem<ItemGroupItem>();
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
            ConfigureCrudPermissions(session, CrudOperationPermissions.ItemGroups);
        }
    }
}

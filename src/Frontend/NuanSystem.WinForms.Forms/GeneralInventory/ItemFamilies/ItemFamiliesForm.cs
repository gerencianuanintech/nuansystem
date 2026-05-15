using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit;
using NuanSystem.WinForms.Services.GeneralInventory.ItemFamilies.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.GeneralInventory.ItemFamilies;

namespace NuanSystem.WinForms.Forms.GeneralInventory.ItemFamilies;

public sealed partial class ItemFamiliesForm : BaseGridCrudListForm
{
    private readonly ItemFamiliesViewModel viewModel;
    private readonly ApiSession session;
    private readonly IAuditClient auditClient;

    public ItemFamiliesForm()
    {
        viewModel = null!;
        session = null!;
        auditClient = null!;
        InitializeComponent();
        WireEvents();
    }

    public ItemFamiliesForm(ItemFamiliesViewModel viewModel, ApiSession session, IAuditClient auditClient, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        this.auditClient = auditClient;
        InitializeComponent();
        ConfigureColumnPersonalization(columnSettingsClient, "item-families");
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
        using var form = new ItemFamilyEditForm(viewModel.Lookups.ItemGroups);
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
        using var form = new ItemFamilyEditForm(viewModel.Lookups.ItemGroups, fullItem);
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
        using var form = new ItemFamilyEditForm(viewModel.Lookups.ItemGroups, fullItem, copyMode: true);
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

        if (!Confirm($"Eliminar la linea/familia {item.Code}?"))
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
            "Historial de linea/familia",
            $"{item.Code} - {item.Name}",
            cancellationToken => auditClient.GetInventoryChangesAsync("ItemFamilies", item.Id.ToString(), 200, cancellationToken));

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

        ConfigureColumn(nameof(ItemFamilyItem.ItemGroupName), "Grupo", 1, 180);
        ConfigureColumn(nameof(ItemFamilyItem.Code), "Codigo", 2, 110);
        ConfigureColumn(nameof(ItemFamilyItem.Name), "Nombre", 3, 210);
        ConfigureColumn(nameof(ItemFamilyItem.Description), "Descripcion", 4, 260);
        ConfigureColumn(nameof(ItemFamilyItem.SapFamilyCode), "Grupo SAP", 5, 120);
        ConfigureColumn(nameof(ItemFamilyItem.SapCode), "Codigo SAP", 6, 110);
        ConfigureColumn(nameof(ItemFamilyItem.IsActive), "Activo", 7, 70);

        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ItemFamilyItem? SelectedItem()
    {
        return SelectedGridItem<ItemFamilyItem>();
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

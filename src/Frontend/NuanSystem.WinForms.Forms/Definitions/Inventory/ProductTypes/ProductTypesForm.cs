using NuanSystem.Shared.Constants;
using NuanSystem.WinForms.Forms.Audit;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Definitions.Inventory.ProductTypes.Models;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Definitions.Inventory.ProductTypes;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ProductTypes;

public sealed class ProductTypesForm : BaseGridCrudListForm
{
    public const string FormKey = "product-types";
    private static readonly CrudOperationPermissions Permissions = new(
        PermissionCodes.GeneralInventoryProductTypesRead,
        PermissionCodes.GeneralInventoryProductTypesManage,
        PermissionCodes.GeneralInventoryProductTypesManage,
        PermissionCodes.GeneralInventoryProductTypesManage);

    private readonly ProductTypesViewModel viewModel;
    private readonly ApiSession session;

    public ProductTypesForm()
    {
        viewModel = null!;
        session = null!;
        ConfigureWindow();
        WirePermissions();
    }

    public ProductTypesForm(
        ProductTypesViewModel viewModel,
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
        using var form = new ProductTypeEditForm();
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Tipo de producto creado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        using var form = new ProductTypeEditForm(item);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Tipo de producto actualizado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task CopyAsync()
    {
        if (SelectedItem() is not { } selected) return;
        var item = await viewModel.GetByIdAsync(selected.Id);
        using var form = new ProductTypeEditForm(item, copyMode: true);
        if (form.ShowDialog(this) != DialogResult.OK) return;
        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Tipo de producto copiado correctamente.");
        await LoadDataAsync();
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item) return;
        if (item.IsSystem)
        {
            ShowWarning("Los tipos de producto del sistema no se pueden eliminar.");
            return;
        }

        if (!Confirm($"¿Eliminar el tipo de producto {item.Code}?")) return;
        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override Task HistoryAsync()
    {
        if (SelectedItem() is not { } item) return Task.CompletedTask;
        using var form = new RecordHistoryForm(
            "Historial de tipo de producto",
            $"{item.Code} - {item.Name}",
            async cancellationToken =>
            {
                var changes = await viewModel.GetHistoryAsync(item.Id, cancellationToken);
                return changes.Select(change => new SecurityChangeItem(
                    0, "ProductType", change.RecordId, change.Action, change.FieldName,
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
        Column(nameof(ProductTypeItem.Code), "Código", 1, 115);
        Column(nameof(ProductTypeItem.Name), "Nombre", 2, 240);
        Column(nameof(ProductTypeItem.Description), "Descripción", 3, 300);
        Column(nameof(ProductTypeItem.NatureName), "Naturaleza", 4, 150);
        Column(nameof(ProductTypeItem.SortOrder), "Orden", 5, 75, DevExpress.Utils.HorzAlignment.Far);
        Column(nameof(ProductTypeItem.IsSystem), "Sistema", 6, 80);
        Column(nameof(ProductTypeItem.IsActive), "Activo", 7, 75);
        GridView.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
    }

    private ProductTypeItem? SelectedItem() => SelectedGridItem<ProductTypeItem>();

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
        ClientSize = new Size(1100, 620);
        MinimumSize = new Size(900, 520);
        Name = nameof(ProductTypesForm);
        Text = "Tipos de producto";
    }

    private void WirePermissions()
    {
        if (session is not null) ConfigureCrudPermissions(session, Permissions);
    }
}

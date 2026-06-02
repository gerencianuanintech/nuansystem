using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GridColumnSettings;
using NuanSystem.WinForms.Services.Purchasing.PurchaseOrders.Models;
using NuanSystem.WinForms.Services.Session;
using NuanSystem.WinForms.ViewModels.Purchasing.PurchaseOrders;

namespace NuanSystem.WinForms.Forms.Purchasing.PurchaseOrders;

public sealed class PurchaseOrdersForm : BaseGridCrudListForm
{
    private readonly PurchaseOrdersViewModel viewModel;
    private readonly ApiSession session;

    public PurchaseOrdersForm()
    {
        viewModel = null!;
        session = null!;
        Text = "Ordenes de compra";
    }

    public PurchaseOrdersForm(PurchaseOrdersViewModel viewModel, ApiSession session, IGridColumnSettingsClient columnSettingsClient)
    {
        this.viewModel = viewModel;
        this.session = session;
        Text = "Ordenes de compra";
        ConfigureColumnPersonalization(columnSettingsClient, "purchase-orders");
        ConfigureCrudPermissions(session, CrudOperationPermissions.PurchaseOrders);
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
        await viewModel.LoadLookupsAsync();
        using var form = new FrmPurchaseOrderEdit(null, viewModel.Lookups);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.CreateAsync(form.Request);
        ShowSuccess("Orden de compra guardada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task EditAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        await viewModel.LoadLookupsAsync();
        var detail = await viewModel.GetByIdAsync(item.Id);
        using var form = new FrmPurchaseOrderEdit(detail, viewModel.Lookups);
        if (form.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        await viewModel.UpdateAsync(item.Id, form.Request);
        ShowSuccess("Orden de compra actualizada correctamente.");
        await LoadDataAsync();
    }

    protected override async Task ConsultAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        await viewModel.LoadLookupsAsync();
        var detail = await viewModel.GetByIdAsync(item.Id);
        using (BaseEditForm.BeginReadOnlyMode())
        using (var form = new FrmPurchaseOrderEdit(detail, viewModel.Lookups))
        {
            form.ShowDialog(this);
        }
    }

    protected override async Task DeleteAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        if (!Confirm($"Eliminar la orden {item.SeriesCode}-{item.DocumentNumber}?"))
        {
            return;
        }

        await viewModel.DeleteAsync(item.Id);
        await LoadDataAsync();
    }

    protected override async Task HistoryAsync()
    {
        if (SelectedItem() is not { } item)
        {
            return;
        }

        if (item.Status != "Approved" && item.Status != "SapPending")
        {
            XtraMessageBox.Show(this, "Solo se pueden sincronizar ordenes aprobadas.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        await viewModel.SyncSapAsync(item.Id);
        ShowSuccess("Orden marcada como pendiente de sincronizacion SAP.");
        await LoadDataAsync();
    }

    protected override void ConfigureGridColumns()
    {
        base.ConfigureGridColumns();

        foreach (DevExpress.XtraGrid.Columns.GridColumn column in GridView.Columns)
        {
            column.Visible = false;
        }

        ConfigureColumn(nameof(PurchaseOrderItem.SeriesCode), "Serie", 1, 90);
        ConfigureColumn(nameof(PurchaseOrderItem.DocumentNumber), "Numero", 2, 110);
        ConfigureColumn(nameof(PurchaseOrderItem.SupplierCode), "Proveedor", 3, 100);
        ConfigureColumn(nameof(PurchaseOrderItem.SupplierName), "Nombre proveedor", 4, 240);
        ConfigureColumn(nameof(PurchaseOrderItem.DocumentDate), "Fecha", 5, 100);
        ConfigureColumn(nameof(PurchaseOrderItem.DeliveryDate), "Entrega", 6, 100);
        ConfigureColumn(nameof(PurchaseOrderItem.CurrencyCode), "Moneda", 7, 80);
        ConfigureColumn(nameof(PurchaseOrderItem.TotalAmount), "Total", 8, 110);
        ConfigureColumn(nameof(PurchaseOrderItem.Status), "Estado", 9, 130);
        ConfigureColumn(nameof(PurchaseOrderItem.SapStatus), "SAP", 10, 120);
    }

    private PurchaseOrderItem? SelectedItem()
    {
        return SelectedGridItem<PurchaseOrderItem>();
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
}

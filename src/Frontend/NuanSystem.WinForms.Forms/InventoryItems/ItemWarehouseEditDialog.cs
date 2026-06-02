using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Services.InventoryItems.Models;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemWarehouseEditDialog : XtraForm
{
    private readonly IReadOnlyCollection<WarehouseLookupItem> warehouses;

    public ItemWarehouseEditDialog(IReadOnlyCollection<WarehouseLookupItem> warehouses)
        : this(warehouses, null)
    {
    }

    public ItemWarehouseEditDialog(IReadOnlyCollection<WarehouseLookupItem> warehouses, ItemWarehouseRow? row)
    {
        this.warehouses = warehouses;
        InitializeComponent();
        ConfigureForm();

        if (row is not null)
        {
            LoadRow(row);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemWarehouseRow Row { get; private set; } = ItemWarehouseRow.Empty;

    private void ConfigureForm()
    {
        lueWarehouse.Properties.DataSource = warehouses.ToList();
        lueWarehouse.Properties.DisplayMember = nameof(WarehouseLookupItem.DisplayText);
        lueWarehouse.Properties.ValueMember = nameof(WarehouseLookupItem.Id);
        lueWarehouse.Properties.NullText = string.Empty;
        lueWarehouse.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueWarehouse.Properties.Columns.Clear();
        lueWarehouse.Properties.Columns.Add(new LookUpColumnInfo(nameof(WarehouseLookupItem.Code), "Codigo", 80));
        lueWarehouse.Properties.Columns.Add(new LookUpColumnInfo(nameof(WarehouseLookupItem.Name), "Nombre", 180));

        btnSave.Click += SaveButtonClick;
    }

    private void LoadRow(ItemWarehouseRow row)
    {
        lueWarehouse.EditValue = row.WarehouseId;
        spnMinimumStock.Value = row.MinimumStock;
        spnMaximumStock.Value = row.MaximumStock;
        spnReorderPoint.Value = row.ReorderPoint;
        spnRequiredStock.Value = row.RequiredStock;
        txtDefaultLocationCode.Text = row.DefaultLocationCode;
        spnWarehouseCost.Value = row.WarehouseCost;
        chkDefaultWarehouse.Checked = row.IsDefaultWarehouse;
        chkLocked.Checked = row.IsLocked;
        chkActive.Checked = row.IsActive;
    }

    private void SaveButtonClick(object? sender, EventArgs e)
    {
        if (!ValidateForm())
        {
            return;
        }

        var warehouse = warehouses.First(x => x.Id == (int)lueWarehouse.EditValue);
        Row = new ItemWarehouseRow(
            warehouse.Id,
            warehouse.Code,
            warehouse.Name,
            spnMinimumStock.Value,
            spnMaximumStock.Value,
            spnRequiredStock.Value,
            spnReorderPoint.Value,
            txtDefaultLocationCode.Text.Trim(),
            spnWarehouseCost.Value,
            chkDefaultWarehouse.Checked,
            chkLocked.Checked,
            chkActive.Checked);

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateForm()
    {
        if (lueWarehouse.EditValue is not int)
        {
            ShowValidation("Seleccione la bodega.");
            lueWarehouse.Focus();
            return false;
        }

        if (spnMinimumStock.Value < 0 || spnMaximumStock.Value < 0 || spnRequiredStock.Value < 0 || spnReorderPoint.Value < 0)
        {
            ShowValidation("Las cantidades no pueden ser negativas.");
            return false;
        }

        if (spnMaximumStock.Value > 0 && spnMaximumStock.Value < spnMinimumStock.Value)
        {
            ShowValidation("El stock maximo no puede ser menor al minimo.");
            spnMaximumStock.Focus();
            return false;
        }

        if (spnWarehouseCost.Value < 0)
        {
            ShowValidation("El costo por bodega no puede ser negativo.");
            spnWarehouseCost.Focus();
            return false;
        }

        return true;
    }

    private void ShowValidation(string message)
    {
        XtraMessageBox.Show(this, message, "Bodega del item", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

public sealed record ItemWarehouseRow(
    int WarehouseId,
    string WarehouseCode,
    string WarehouseName,
    decimal MinimumStock,
    decimal MaximumStock,
    decimal RequiredStock,
    decimal ReorderPoint,
    string? DefaultLocationCode,
    decimal WarehouseCost,
    bool IsDefaultWarehouse,
    bool IsLocked,
    bool IsActive)
{
    public static ItemWarehouseRow Empty { get; } = new(
        0,
        string.Empty,
        string.Empty,
        0,
        0,
        0,
        0,
        null,
        0,
        false,
        false,
        true);
}

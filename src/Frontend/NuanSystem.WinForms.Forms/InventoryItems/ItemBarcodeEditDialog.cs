using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Services.InventoryItems.Models;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemBarcodeEditDialog : XtraForm
{
    private readonly IReadOnlyCollection<UnitOfMeasureLookupItem> units;

    public ItemBarcodeEditDialog(IReadOnlyCollection<UnitOfMeasureLookupItem> units)
        : this(units, null)
    {
    }

    public ItemBarcodeEditDialog(IReadOnlyCollection<UnitOfMeasureLookupItem> units, ItemBarcodeRow? row)
    {
        this.units = units;
        InitializeComponent();
        ConfigureForm();

        if (row is not null)
        {
            LoadRow(row);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemBarcodeRow Row { get; private set; } = ItemBarcodeRow.Empty;

    private void ConfigureForm()
    {
        lueScope.Properties.DataSource = new List<BarcodeScopeOption>
        {
            new("General", "General"),
            new("Purchase", "Compra"),
            new("Sales", "Venta"),
            new("Inventory", "Inventario")
        };
        lueScope.Properties.DisplayMember = nameof(BarcodeScopeOption.Name);
        lueScope.Properties.ValueMember = nameof(BarcodeScopeOption.Code);
        lueScope.Properties.NullText = string.Empty;
        lueScope.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueScope.Properties.Columns.Clear();
        lueScope.Properties.Columns.Add(new LookUpColumnInfo(nameof(BarcodeScopeOption.Name), "Alcance", 160));
        lueScope.EditValue = "General";

        lueUnit.Properties.DataSource = units.ToList();
        lueUnit.Properties.DisplayMember = nameof(UnitOfMeasureLookupItem.DisplayText);
        lueUnit.Properties.ValueMember = nameof(UnitOfMeasureLookupItem.Id);
        lueUnit.Properties.NullText = string.Empty;
        lueUnit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueUnit.Properties.Columns.Clear();
        lueUnit.Properties.Columns.Add(new LookUpColumnInfo(nameof(UnitOfMeasureLookupItem.Code), "Codigo", 80));
        lueUnit.Properties.Columns.Add(new LookUpColumnInfo(nameof(UnitOfMeasureLookupItem.Name), "Nombre", 180));

        btnSave.Click += SaveButtonClick;
    }

    private void LoadRow(ItemBarcodeRow row)
    {
        txtBarcode.Text = row.Barcode;
        lueScope.EditValue = row.Scope;
        txtPresentation.Text = row.Presentation;
        lueUnit.EditValue = row.UnitId;
        spnFactor.Value = row.Factor;
        chkPrincipal.Checked = row.IsMain;
        chkActive.Checked = row.IsActive;
    }

    private void SaveButtonClick(object? sender, EventArgs e)
    {
        if (!ValidateForm())
        {
            return;
        }

        var unit = units.First(x => x.Id == (int)lueUnit.EditValue);
        Row = new ItemBarcodeRow(
            txtBarcode.Text.Trim(),
            Convert.ToString(lueScope.EditValue) ?? "General",
            txtPresentation.Text.Trim(),
            unit.Id,
            unit.Code,
            unit.DisplayText,
            spnFactor.Value,
            chkPrincipal.Checked,
            chkActive.Checked);

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(txtBarcode.Text))
        {
            ShowValidation("Ingrese el codigo de barras.");
            txtBarcode.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtPresentation.Text))
        {
            ShowValidation("Ingrese la presentacion.");
            txtPresentation.Focus();
            return false;
        }

        if (lueUnit.EditValue is not int)
        {
            ShowValidation("Seleccione la unidad.");
            lueUnit.Focus();
            return false;
        }

        if (spnFactor.Value <= 0)
        {
            ShowValidation("El factor debe ser mayor a cero.");
            spnFactor.Focus();
            return false;
        }

        return true;
    }

    private void ShowValidation(string message)
    {
        XtraMessageBox.Show(this, message, "Codigo de barras", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private sealed record BarcodeScopeOption(string Code, string Name);
}

public sealed record ItemBarcodeRow(
    string Barcode,
    string Scope,
    string Presentation,
    int UnitId,
    string UnitCode,
    string UnitDisplay,
    decimal Factor,
    bool IsMain,
    bool IsActive)
{
    public static ItemBarcodeRow Empty { get; } = new(
        string.Empty,
        "General",
        string.Empty,
        0,
        string.Empty,
        string.Empty,
        1,
        false,
        true);
}

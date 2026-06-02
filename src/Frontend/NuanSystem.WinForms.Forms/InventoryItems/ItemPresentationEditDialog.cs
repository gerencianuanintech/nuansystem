using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Services.InventoryItems.Models;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemPresentationEditDialog : XtraForm
{
    private readonly IReadOnlyCollection<UnitOfMeasureLookupItem> units;

    public ItemPresentationEditDialog(IReadOnlyCollection<UnitOfMeasureLookupItem> units)
        : this(units, null)
    {
    }

    public ItemPresentationEditDialog(IReadOnlyCollection<UnitOfMeasureLookupItem> units, ItemPresentationRow? row)
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
    public ItemPresentationRow Row { get; private set; } = ItemPresentationRow.Empty;

    private void ConfigureForm()
    {
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

    private void LoadRow(ItemPresentationRow row)
    {
        txtPresentation.Text = row.Presentation;
        lueUnit.EditValue = row.UnitId;
        spnFactor.Value = row.Factor;
        txtBarcode.Text = row.Barcode;
        tglAppliesPurchase.IsOn = row.AppliesPurchase;
        tglAppliesSale.IsOn = row.AppliesSale;
        tglAppliesInventory.IsOn = row.AppliesInventory;
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
        Row = new ItemPresentationRow(
            txtPresentation.Text.Trim(),
            unit.Id,
            unit.Code,
            unit.DisplayText,
            spnFactor.Value,
            txtBarcode.Text.Trim(),
            tglAppliesPurchase.IsOn,
            tglAppliesSale.IsOn,
            tglAppliesInventory.IsOn,
            chkPrincipal.Checked,
            chkActive.Checked);

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateForm()
    {
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
        XtraMessageBox.Show(this, message, "Presentacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

public sealed record ItemPresentationRow(
    string Presentation,
    int UnitId,
    string UnitCode,
    string UnitDisplay,
    decimal Factor,
    string? Barcode,
    bool AppliesPurchase,
    bool AppliesSale,
    bool AppliesInventory,
    bool IsMain,
    bool IsActive)
{
    public static ItemPresentationRow Empty { get; } = new(
        string.Empty,
        0,
        string.Empty,
        string.Empty,
        1,
        null,
        true,
        true,
        true,
        false,
        true);
}

using System.ComponentModel;
using DevExpress.XtraEditors;
using NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

public sealed partial class SupplierWithholdingEditDialog : XtraForm
{
    public SupplierWithholdingEditDialog()
        : this(null)
    {
    }

    internal SupplierWithholdingEditDialog(SupplierWithholdingViewModel? withholding)
    {
        InitializeComponent();
        BindLookups();

        Withholding = withholding?.Clone() ?? new SupplierWithholdingViewModel
        {
            Document = "RUC 20123456789",
            Type = "Renta",
            IncomeTaxWithholdingPercent = 1.75m,
            VatWithholdingPercent = 30m,
            TaxSupport = "Compra de bienes",
            FiscalRegime = "Régimen General",
            IsRequiredAccounting = true,
            ValidityFrom = new DateTime(2024, 1, 1),
            ValidityTo = new DateTime(2024, 12, 31),
            IsDefault = true,
            IsActive = true,
            Notes = "Configuración de retención aplicable para compras nacionales."
        };

        Text = withholding is null ? "Nueva Configuración de Retención" : "Editar Configuración de Retención";
        LoadWithholding();
        WireEvents();
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal SupplierWithholdingViewModel Withholding { get; private set; }

    private void WireEvents()
    {
        btnSaveWithholding.Click += (_, _) => SaveWithholding();
        btnCancelWithholding.Click += (_, _) => Close();
    }

    private void BindLookups()
    {
        BindLookup(lueWithholdingType, "Renta", "IVA", "Especial", "Renta e IVA");
        BindLookup(lueTaxSupport, "Compra de bienes", "Servicio", "Activo fijo");
        BindLookup(lueFiscalRegime, "Régimen General", "Régimen Especial", "Régimen MYPE Tributario");
    }

    private static void BindLookup(LookUpEdit lookup, params string[] values)
    {
        lookup.Properties.DataSource = values.Select(value => new SupplierTextOptionViewModel(value, value)).ToList();
        lookup.Properties.DisplayMember = nameof(SupplierTextOptionViewModel.Name);
        lookup.Properties.ValueMember = nameof(SupplierTextOptionViewModel.Code);
        lookup.Properties.Columns.Clear();
        lookup.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo(nameof(SupplierTextOptionViewModel.Name), "Nombre", 220));
    }

    private void LoadWithholding()
    {
        txtWithholdingDocument.Text = Withholding.Document;
        lueWithholdingType.EditValue = Withholding.Type;
        spnIncomeTaxWithholdingPercent.Value = Withholding.IncomeTaxWithholdingPercent;
        spnVatWithholdingPercent.Value = Withholding.VatWithholdingPercent;
        lueTaxSupport.EditValue = Withholding.TaxSupport;
        lueFiscalRegime.EditValue = Withholding.FiscalRegime;
        tglSpecialTaxpayer.IsOn = Withholding.IsSpecialTaxpayer;
        tglRequiredAccounting.IsOn = Withholding.IsRequiredAccounting;
        dteValidityFrom.EditValue = Withholding.ValidityFrom;
        dteValidityTo.EditValue = Withholding.ValidityTo;
        tglWithholdingDefault.IsOn = Withholding.IsDefault;
        tglWithholdingActive.IsOn = Withholding.IsActive;
        memWithholdingNotes.Text = Withholding.Notes;
    }

    private void SaveWithholding()
    {
        if (!ValidateWithholding())
        {
            return;
        }

        Withholding.Document = txtWithholdingDocument.Text.Trim();
        Withholding.Type = Convert.ToString(lueWithholdingType.EditValue) ?? string.Empty;
        Withholding.IncomeTaxWithholdingPercent = spnIncomeTaxWithholdingPercent.Value;
        Withholding.VatWithholdingPercent = spnVatWithholdingPercent.Value;
        Withholding.TaxSupport = Convert.ToString(lueTaxSupport.EditValue) ?? string.Empty;
        Withholding.FiscalRegime = Convert.ToString(lueFiscalRegime.EditValue) ?? string.Empty;
        Withholding.IsSpecialTaxpayer = tglSpecialTaxpayer.IsOn;
        Withholding.IsRequiredAccounting = tglRequiredAccounting.IsOn;
        Withholding.ValidityFrom = dteValidityFrom.EditValue is DateTime from ? from : null;
        Withholding.ValidityTo = dteValidityTo.EditValue is DateTime to ? to : null;
        Withholding.IsDefault = tglWithholdingDefault.IsOn;
        Withholding.IsActive = tglWithholdingActive.IsOn;
        Withholding.Notes = memWithholdingNotes.Text.Trim();

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateWithholding()
    {
        if (string.IsNullOrWhiteSpace(txtWithholdingDocument.Text))
        {
            return ShowValidation("Documento es requerido.", txtWithholdingDocument);
        }

        if (lueWithholdingType.EditValue is null)
        {
            return ShowValidation("Tipo es requerido.", lueWithholdingType);
        }

        if (dteValidityFrom.EditValue is not DateTime from)
        {
            return ShowValidation("Vigencia Desde es requerida.", dteValidityFrom);
        }

        if (spnIncomeTaxWithholdingPercent.Value < 0)
        {
            return ShowValidation("Porcentaje Retención Renta debe ser mayor o igual a cero.", spnIncomeTaxWithholdingPercent);
        }

        if (spnVatWithholdingPercent.Value < 0)
        {
            return ShowValidation("Porcentaje Retención IVA debe ser mayor o igual a cero.", spnVatWithholdingPercent);
        }

        if (spnIncomeTaxWithholdingPercent.Value == 0 && spnVatWithholdingPercent.Value == 0)
        {
            return ShowValidation("Al menos un porcentaje de retención debe ser mayor que cero.", spnIncomeTaxWithholdingPercent);
        }

        if (dteValidityTo.EditValue is DateTime to && to < from)
        {
            return ShowValidation("Vigencia Hasta no puede ser menor que Vigencia Desde.", dteValidityTo);
        }

        return true;
    }

    private bool ShowValidation(string message, Control control)
    {
        XtraMessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        control.Focus();
        return false;
    }
}

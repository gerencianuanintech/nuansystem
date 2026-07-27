using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.TaxCatalogs.Taxes;

namespace NuanSystem.WinForms.Forms.TaxCatalogs.Taxes;

public sealed partial class TaxEditForm : BaseEditForm
{
    public TaxEditForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public TaxEditForm(TaxItem item, bool copyMode = false) : this()
    {
        Text = copyMode ? "Copiar impuesto" : "Editar impuesto";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        spnRate.Value = item.Rate * 100m;
        chkIsActive.Checked = item.IsActive;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveTaxRequest Request { get; private set; } = new(string.Empty, string.Empty, null, 0m, true);

    protected override bool ValidateForm()
    {
        var valid = Validator.RequireText(txtCode, "Ingrese el código.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        if (spnRate.Value is < 0m or > 100m)
        {
            Validator.SetError(spnRate, "Ingrese un porcentaje entre 0 y 100.");
            valid = false;
        }
        return valid;
    }

    protected override void BuildRequest() =>
        Request = new SaveTaxRequest(
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            string.IsNullOrWhiteSpace(memDescription.Text) ? null : memDescription.Text.Trim(),
            spnRate.Value / 100m,
            chkIsActive.Checked);
}

using System.ComponentModel;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Carriers.Models;

namespace NuanSystem.WinForms.Forms.Carriers;

public sealed partial class CarrierEditForm : BaseEditForm
{
    public CarrierEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public CarrierEditForm(CarrierDetail item, bool copyMode = false) : this() => LoadCarrier(item, copyMode);

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveCarrierRequest Request { get; private set; } = new(string.Empty, string.Empty, string.Empty, string.Empty, null, true);

    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtCode, "Ingrese el codigo.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        valid &= Validator.RequireText(lueIdentificationType, "Seleccione el tipo de identificacion.");
        valid &= Validator.RequireText(txtIdentificationNumber, "Ingrese la identificacion.");
        return valid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveCarrierRequest(
            txtCode.Text.Trim(), txtName.Text.Trim(), Convert.ToString(lueIdentificationType.EditValue) ?? string.Empty,
            txtIdentificationNumber.Text.Trim(), NormalizeOptional(memDescription.Text), chkIsActive.Checked);
    }

    private void ConfigureForm()
    {
        Text = "Nuevo transportista";
        chkIsActive.Checked = true;
        lueIdentificationType.Properties.DataSource = CarrierIdentificationTypes.All;
        lueIdentificationType.Properties.DisplayMember = nameof(CarrierIdentificationTypeItem.DisplayText);
        lueIdentificationType.Properties.ValueMember = nameof(CarrierIdentificationTypeItem.Code);
        lueIdentificationType.Properties.Columns.Clear();
        lueIdentificationType.Properties.Columns.Add(new LookUpColumnInfo(nameof(CarrierIdentificationTypeItem.DisplayText), "Tipo de identificacion"));
        lueIdentificationType.EditValue = "05";
    }

    private void LoadCarrier(CarrierDetail item, bool copyMode)
    {
        Text = copyMode ? "Copiar transportista" : "Editar transportista";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        lueIdentificationType.EditValue = item.IdentificationTypeCode;
        txtIdentificationNumber.Text = item.IdentificationNumber;
        memDescription.Text = item.Description;
        chkIsActive.Checked = item.IsActive;
    }

    private static string? NormalizeOptional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

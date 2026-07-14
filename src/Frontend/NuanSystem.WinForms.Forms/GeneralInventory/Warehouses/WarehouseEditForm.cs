using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.GeneralInventory.Warehouses.Models;

namespace NuanSystem.WinForms.Forms.GeneralInventory.Warehouses;

public sealed partial class WarehouseEditForm : BaseEditForm
{
    public WarehouseEditForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
        chkAllowsSales.Checked = true;
        chkAllowsPurchases.Checked = true;
        chkAllowsTransfers.Checked = true;
    }

    public WarehouseEditForm(WarehouseItem item, bool copyMode = false)
        : this()
    {
        LoadWarehouse(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveWarehouseRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var isValid = true;
        isValid &= Validator.RequireText(txtCode, "Ingrese el codigo.");
        isValid &= Validator.RequireText(txtName, "Ingrese el nombre.");
        return isValid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveWarehouseRequest(
            GetGlobalId(),
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            NormalizeText(memDescription.Text),
            NormalizeText(txtBranchCode.Text),
            NormalizeText(txtAddress.Text),
            NormalizeText(txtCity.Text),
            NormalizeText(txtProvince.Text),
            NormalizeText(txtCountry.Text),
            NormalizeText(txtPhone.Text),
            NormalizeText(txtEmail.Text),
            NormalizeText(txtManagerName.Text),
            chkAllowsSales.Checked,
            chkAllowsPurchases.Checked,
            chkAllowsTransfers.Checked,
            chkAllowsProduction.Checked,
            chkIsDefault.Checked,
            NormalizeText(txtExternalSystem.Text),
            NormalizeText(txtExternalCode.Text),
            NormalizeText(txtSapCode.Text),
            chkIsActive.Checked);
    }

    private void LoadWarehouse(WarehouseItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar bodega" : "Editar bodega";
        txtGlobalId.Text = copyMode ? string.Empty : item.GlobalId.ToString();
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        txtBranchCode.Text = item.BranchCode;
        txtAddress.Text = item.Address;
        txtCity.Text = item.City;
        txtProvince.Text = item.Province;
        txtCountry.Text = item.Country;
        txtPhone.Text = item.Phone;
        txtEmail.Text = item.Email;
        txtManagerName.Text = item.ManagerName;
        chkAllowsSales.Checked = item.AllowsSales;
        chkAllowsPurchases.Checked = item.AllowsPurchases;
        chkAllowsTransfers.Checked = item.AllowsTransfers;
        chkAllowsProduction.Checked = item.AllowsProduction;
        chkIsDefault.Checked = item.IsDefault;
        txtExternalSystem.Text = item.ExternalSystem;
        txtExternalCode.Text = item.ExternalCode;
        txtSapCode.Text = item.SapCode;
        chkIsActive.Checked = item.IsActive;
    }

    private Guid? GetGlobalId()
    {
        return Guid.TryParse(txtGlobalId.Text, out var globalId) && globalId != Guid.Empty
            ? globalId
            : null;
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static SaveWarehouseRequest EmptyRequest()
    {
        return new SaveWarehouseRequest(null, string.Empty, string.Empty, null, null, null, null, null, null, null, null, null, true, true, true, false, false, null, null, null, true);
    }
}

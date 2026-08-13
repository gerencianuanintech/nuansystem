using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemBrands.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemBrands;

public sealed partial class ItemBrandEditForm : BaseEditForm
{
    public ItemBrandEditForm()
    {
        InitializeComponent();
        ConfigureForm();
    }

    public ItemBrandEditForm(ItemBrandItem item, bool copyMode = false)
        : this()
    {
        LoadItem(item, copyMode);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemBrandRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtCode, "Ingrese el código de la marca.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre de la marca.");
        return valid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveItemBrandRequest(
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            Optional(memDescription.Text),
            Convert.ToInt32(spnSortOrder.Value),
            chkIsActive.Checked,
            Optional(cmbExternalSystem.Text),
            Optional(txtExternalCode.Text),
            Optional(txtSapManufacturerCode.Text),
            Optional(txtSapCode.Text));
    }

    private void ConfigureForm()
    {
        chkIsActive.Checked = true;
        cmbExternalSystem.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
    }

    private void LoadItem(ItemBrandItem item, bool copyMode)
    {
        Text = copyMode ? "Copiar marca de artículos" : "Marca de artículos";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        spnSortOrder.Value = item.SortOrder;
        chkIsActive.Checked = item.IsActive;
        cmbExternalSystem.Text = item.ExternalSystem;
        txtExternalCode.Text = item.ExternalCode;
        txtSapManufacturerCode.Text = item.SapManufacturerCode;
        txtSapCode.Text = item.SapCode;
    }

    private static string? Optional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SaveItemBrandRequest EmptyRequest() =>
        new(string.Empty, string.Empty, null, 0, true, null, null, null, null);
}

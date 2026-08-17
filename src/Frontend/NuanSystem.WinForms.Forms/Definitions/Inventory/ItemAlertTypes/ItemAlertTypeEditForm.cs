using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemAlertTypes.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemAlertTypes;

public sealed partial class ItemAlertTypeEditForm : BaseEditForm
{
    public ItemAlertTypeEditForm()
    {
        InitializeComponent();
    }
    public ItemAlertTypeEditForm(ItemAlertTypeItem item) : this()
    {
        txtCode.EditValue = item.Code;
        txtName.EditValue = item.Name;
        memDescription.Text = item.Description;
        spnSortOrder.Value = item.SortOrder;
        tglIsActive.Checked = item.IsActive;
    }
    public ItemAlertTypeEditForm(ItemAlertTypeItem item, bool copyMode) : this(item)
    {
        if (copyMode) txtCode.EditValue = null;
    }
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemAlertTypeRequest Request { get; private set; } = EmptyRequest();
    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtCode, "Ingrese código.");
        valid &= Validator.RequireText(txtName, "Ingrese nombre.");
        return valid;
    }
    protected override void BuildRequest()
    {
        Request = new SaveItemAlertTypeRequest(
            Convert.ToString(txtCode.EditValue)?.Trim() ?? string.Empty,
            Convert.ToString(txtName.EditValue)?.Trim() ?? string.Empty,
            Optional(memDescription.Text),
            Convert.ToInt32(spnSortOrder.Value),
            tglIsActive.Checked);
    }
    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SaveItemAlertTypeRequest EmptyRequest() => new(string.Empty, string.Empty, null, 0, true);
}

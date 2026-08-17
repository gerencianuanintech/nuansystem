using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.StorageConditions.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.StorageConditions;

public sealed partial class StorageConditionEditForm : BaseEditForm
{
    public StorageConditionEditForm() { InitializeComponent(); tglIsActive.Checked = true; }
    public StorageConditionEditForm(StorageConditionItem item, bool copyMode = false) : this()
    {
        Text = copyMode ? "Copiar condición de almacenamiento" : "Condición de almacenamiento";
        txtCode.EditValue = copyMode ? string.Empty : item.Code;
        txtName.EditValue = item.Name;
        memDescription.Text = item.Description;
        spnSortOrder.Value = item.SortOrder;
        tglIsActive.Checked = item.IsActive;
    }
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveStorageConditionRequest Request { get; private set; } = EmptyRequest();
    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtCode, "Ingrese el código de la condición de almacenamiento.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre de la condición de almacenamiento.");
        return valid;
    }
    protected override void BuildRequest() => Request = new(
        Convert.ToString(txtCode.EditValue)?.Trim() ?? string.Empty,
        Convert.ToString(txtName.EditValue)?.Trim() ?? string.Empty,
        string.IsNullOrWhiteSpace(memDescription.Text) ? null : memDescription.Text.Trim(),
        Convert.ToInt32(spnSortOrder.Value), tglIsActive.Checked);
    private static SaveStorageConditionRequest EmptyRequest() => new(string.Empty, string.Empty, null, 0, true);
}

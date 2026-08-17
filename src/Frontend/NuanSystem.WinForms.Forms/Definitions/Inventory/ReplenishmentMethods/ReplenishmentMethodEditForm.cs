using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.ReplenishmentMethods.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ReplenishmentMethods;

public sealed partial class ReplenishmentMethodEditForm : BaseEditForm
{
    public ReplenishmentMethodEditForm()
    {
        InitializeComponent();
        tglIsActive.Checked = true;
    }

    public ReplenishmentMethodEditForm(ReplenishmentMethodItem item, bool copyMode = false) : this()
    {
        Text = copyMode ? "Copiar método de reposición" : "Método de reposición";
        txtCode.EditValue = copyMode ? string.Empty : item.Code;
        txtName.EditValue = item.Name;
        memDescription.Text = item.Description;
        spnSortOrder.Value = item.SortOrder;
        tglIsActive.Checked = item.IsActive;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveReplenishmentMethodRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtCode, "Ingrese el código del método de reposición.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre del método de reposición.");
        return valid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveReplenishmentMethodRequest(
            Convert.ToString(txtCode.EditValue)?.Trim() ?? string.Empty,
            Convert.ToString(txtName.EditValue)?.Trim() ?? string.Empty,
            Optional(memDescription.Text),
            Convert.ToInt32(spnSortOrder.Value),
            tglIsActive.Checked);
    }

    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SaveReplenishmentMethodRequest EmptyRequest() => new(string.Empty, string.Empty, null, 0, true);
}

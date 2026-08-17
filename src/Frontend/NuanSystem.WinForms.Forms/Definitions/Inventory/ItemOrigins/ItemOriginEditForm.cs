using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemOrigins.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemOrigins;

public sealed partial class ItemOriginEditForm : BaseEditForm
{
    public ItemOriginEditForm()
    {
        InitializeComponent();
        tglIsActive.Checked = true;
    }

    public ItemOriginEditForm(ItemOriginItem item, bool copyMode = false) : this()
    {
        Text = copyMode ? "Copiar origen de artículos" : "Origen de artículos";
        txtCode.EditValue = copyMode ? string.Empty : item.Code;
        txtName.EditValue = item.Name;
        memDescription.Text = item.Description;
        spnSortOrder.Value = item.SortOrder;
        tglIsActive.Checked = item.IsActive;
    }
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemOriginRequest Request { get; private set; } = EmptyRequest();
    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtCode, "Ingrese el código del origen de artículos.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre del origen de artículos.");
        return valid;
    }
    protected override void BuildRequest()
    {
        Request = new SaveItemOriginRequest(
            Convert.ToString(txtCode.EditValue)?.Trim() ?? string.Empty,
            Convert.ToString(txtName.EditValue)?.Trim() ?? string.Empty,
            Optional(memDescription.Text),
            Convert.ToInt32(spnSortOrder.Value),
            tglIsActive.Checked);
    }
    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SaveItemOriginRequest EmptyRequest() => new(string.Empty, string.Empty, null, 0, true);
}

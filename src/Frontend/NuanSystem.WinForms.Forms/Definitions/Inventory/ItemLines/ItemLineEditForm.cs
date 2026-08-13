using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemLines.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemLines;

public sealed partial class ItemLineEditForm : BaseEditForm
{
    public ItemLineEditForm()
    {
        InitializeComponent();
        chkIsActive.Checked = true;
    }

    public ItemLineEditForm(ItemLineItem item, bool copyMode = false)
        : this()
    {
        Text = copyMode ? "Copiar línea de artículos" : "Línea de artículos";
        txtCode.Text = copyMode ? string.Empty : item.Code;
        txtName.Text = item.Name;
        memDescription.Text = item.Description;
        spnSortOrder.Value = item.SortOrder;
        chkIsActive.Checked = item.IsActive;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemLineRequest Request { get; private set; } = EmptyRequest();

    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtCode, "Ingrese el código de la línea de artículos.");
        valid &= Validator.RequireText(txtName, "Ingrese el nombre de la línea de artículos.");
        return valid;
    }

    protected override void BuildRequest()
    {
        Request = new SaveItemLineRequest(
            txtCode.Text.Trim(),
            txtName.Text.Trim(),
            Optional(memDescription.Text),
            Convert.ToInt32(spnSortOrder.Value),
            chkIsActive.Checked);
    }

    private static string? Optional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static SaveItemLineRequest EmptyRequest() =>
        new(string.Empty, string.Empty, null, 0, true);
}

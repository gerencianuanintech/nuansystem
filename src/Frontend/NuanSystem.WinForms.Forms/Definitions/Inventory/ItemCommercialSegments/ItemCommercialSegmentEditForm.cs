using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Definitions.Inventory.ItemCommercialSegments.Models;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemCommercialSegments;

public sealed partial class ItemCommercialSegmentEditForm : BaseEditForm
{
    public ItemCommercialSegmentEditForm()
    {
        InitializeComponent();
    }
    public ItemCommercialSegmentEditForm(ItemCommercialSegmentItem item) : this()
    {
        txtCode.EditValue = item.Code;
        txtName.EditValue = item.Name;
        memDescription.Text = item.Description;
        spnSortOrder.Value = item.SortOrder;
        tglIsActive.Checked = item.IsActive;
    }
    public ItemCommercialSegmentEditForm(ItemCommercialSegmentItem item, bool copyMode) : this(item)
    {
        if (copyMode) txtCode.EditValue = null;
    }
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SaveItemCommercialSegmentRequest Request { get; private set; } = EmptyRequest();
    protected override bool ValidateForm()
    {
        var valid = true;
        valid &= Validator.RequireText(txtCode, "Ingrese código.");
        valid &= Validator.RequireText(txtName, "Ingrese nombre.");
        return valid;
    }
    protected override void BuildRequest()
    {
        Request = new SaveItemCommercialSegmentRequest(
            Convert.ToString(txtCode.EditValue)?.Trim() ?? string.Empty,
            Convert.ToString(txtName.EditValue)?.Trim() ?? string.Empty,
            Optional(memDescription.Text),
            Convert.ToInt32(spnSortOrder.Value),
            tglIsActive.Checked);
    }
    private static string? Optional(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static SaveItemCommercialSegmentRequest EmptyRequest() => new(string.Empty, string.Empty, null, 0, true);
    private sealed record ClassificationOption(string Code, string Name);
}


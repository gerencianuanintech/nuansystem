using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.SriTxtImports.Models;

namespace NuanSystem.WinForms.Forms.SriTxtImports;

public sealed partial class SriTxtImportFilterDialog : DevExpress.XtraEditors.XtraForm
{
    private static readonly string?[] StatusCodes =
    [
        null,
        "Validated",
        "ValidatedWithErrors",
        "Completed",
        "CompletedWithErrors"
    ];

    private static readonly string?[] EnvironmentCodes = [null, "Test", "Production"];
    private static readonly string[] ValidityCodes = ["All", "Valid", "Invalid"];

    public SriTxtImportFilterDialog()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SriTxtImportFilterDialog(SriTxtImportFilter filter, string rowValidity)
        : this()
    {
        dateFrom.EditValue = filter.CreatedFrom?.Date;
        dateTo.EditValue = filter.CreatedTo?.Date;
        txtFileName.Text = filter.FileName ?? string.Empty;
        cmbStatus.SelectedIndex = FindIndex(StatusCodes, filter.Status);
        cmbEnvironment.SelectedIndex = FindIndex(EnvironmentCodes, filter.Environment);
        cmbValidity.SelectedIndex = FindIndex(ValidityCodes, rowValidity);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DateTime? CreatedFrom { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DateTime? CreatedTo { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Status { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? EnvironmentCode { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? FileNameFilter { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string RowValidity { get; private set; } = "All";

    private void ApplyButton_Click(object? sender, EventArgs e)
    {
        var from = dateFrom.EditValue as DateTime?;
        var to = dateTo.EditValue as DateTime?;
        if (from.HasValue && to.HasValue && from.Value.Date > to.Value.Date)
        {
            ShowWarning("La fecha desde no puede ser posterior a la fecha hasta.");
            return;
        }

        CreatedFrom = from?.Date;
        CreatedTo = to?.Date;
        Status = StatusCodes[Math.Max(0, cmbStatus.SelectedIndex)];
        EnvironmentCode = EnvironmentCodes[Math.Max(0, cmbEnvironment.SelectedIndex)];
        FileNameFilter = string.IsNullOrWhiteSpace(txtFileName.Text)
            ? null
            : txtFileName.Text.Trim();
        RowValidity = ValidityCodes[Math.Max(0, cmbValidity.SelectedIndex)];
        DialogResult = DialogResult.OK;
        Close();
    }

    private void ClearButton_Click(object? sender, EventArgs e)
    {
        CreatedFrom = null;
        CreatedTo = null;
        Status = null;
        EnvironmentCode = null;
        FileNameFilter = null;
        RowValidity = "All";
        DialogResult = DialogResult.OK;
        Close();
    }

    private void ShowWarning(string message)
    {
        DevExpress.XtraEditors.XtraMessageBox.Show(
            this,
            message,
            Text,
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning);
    }

    private static int FindIndex(IReadOnlyList<string?> values, string? selected)
    {
        for (var index = 0; index < values.Count; index++)
        {
            if (string.Equals(values[index], selected, StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        return 0;
    }
}

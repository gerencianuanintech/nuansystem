using DevExpress.XtraEditors;
using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sync;

namespace NuanSystem.WinForms.Forms.Sync.Configuration;

public sealed partial class SyncProfileFilterDialog : XtraForm
{
    public SyncProfileFilterDialog()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SyncProfileFilterDialog(SyncProfileListFilter filter)
        : this()
    {
        LoadFilter(filter);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Search { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool? SelectedIsActive { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? ExecutionMode { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool ClearRequested { get; private set; }

    private void LoadFilter(SyncProfileListFilter filter)
    {
        txtSearch.Text = filter.Search ?? string.Empty;
        cboStatus.SelectedIndex = filter.IsActive switch
        {
            true => 1,
            false => 2,
            _ => 0
        };
        cboExecutionMode.SelectedIndex = string.IsNullOrWhiteSpace(filter.ExecutionMode)
            ? 0
            : FindExecutionModeIndex(filter.ExecutionMode);
    }

    private void ApplyButton_Click(object? sender, EventArgs e)
    {
        Search = string.IsNullOrWhiteSpace(txtSearch.Text) ? null : txtSearch.Text.Trim();
        SelectedIsActive = cboStatus.SelectedIndex switch
        {
            1 => true,
            2 => false,
            _ => null
        };
        ExecutionMode = cboExecutionMode.SelectedIndex <= 0 ? null : cboExecutionMode.Text.Trim();
        ClearRequested = false;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void ClearButton_Click(object? sender, EventArgs e)
    {
        Search = null;
        SelectedIsActive = null;
        ExecutionMode = null;
        ClearRequested = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private static int FindExecutionModeIndex(string executionMode)
    {
        return executionMode.Trim().ToUpperInvariant() switch
        {
            "INCREMENTAL" => 1,
            "FULL" => 2,
            "MANUAL" => 3,
            _ => 0
        };
    }
}

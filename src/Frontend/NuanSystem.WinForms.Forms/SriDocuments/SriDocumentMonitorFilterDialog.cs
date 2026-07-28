using System.ComponentModel;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.SriDocuments.Models;

namespace NuanSystem.WinForms.Forms.SriDocuments;

public sealed partial class SriDocumentMonitorFilterDialog : DevExpress.XtraEditors.XtraForm
{
    private static readonly string?[] EnvironmentCodes=[null,"Test","Production"];
    private static readonly string?[] StatusCodes=
    [
        null,
        "Staged",
        "Pending",
        "Querying",
        "RetryScheduled",
        "Authorized",
        "NotFound",
        "Failed",
        "DeadLetter",
        "Cancelled"
    ];
    private static readonly string?[] DocumentTypeCodes=[null,"01","04","07"];
    private static readonly string?[] SourceTypeCodes=[null,"NuanSystem","Txt","SapAddOn","Manual","ExternalApi"];

    public SriDocumentMonitorFilterDialog()
    {
        InitializeComponent();
        FormStyler.ApplyBase(this);
    }

    public SriDocumentMonitorFilterDialog(SriDocumentMonitorFilter filter)
        : this()
    {
        dateFrom.EditValue=filter.CreatedFrom?.Date;
        dateTo.EditValue=filter.CreatedTo?.Date;
        cmbEnvironment.SelectedIndex=FindIndex(EnvironmentCodes,filter.Environment);
        cmbStatus.SelectedIndex=FindIndex(StatusCodes,filter.Status);
        cmbDocumentType.SelectedIndex=FindIndex(DocumentTypeCodes,filter.DocumentTypeCode);
        cmbSourceType.SelectedIndex=FindIndex(SourceTypeCodes,filter.SourceType);
        txtSearch.Text=filter.Search ?? string.Empty;
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DateTime? CreatedFrom { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public DateTime? CreatedTo { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? EnvironmentCode { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Status { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? DocumentTypeCode { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? SourceType { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Search { get; private set; }

    private void ApplyButton_Click(object? sender,EventArgs e)
    {
        var from=dateFrom.EditValue as DateTime?;
        var to=dateTo.EditValue as DateTime?;
        if (from.HasValue && to.HasValue && from.Value.Date>to.Value.Date)
        {
            ShowWarning("La fecha desde no puede ser posterior a la fecha hasta.");
            return;
        }

        CreatedFrom=from?.Date;
        CreatedTo=to?.Date;
        EnvironmentCode=EnvironmentCodes[Math.Max(0,cmbEnvironment.SelectedIndex)];
        Status=StatusCodes[Math.Max(0,cmbStatus.SelectedIndex)];
        DocumentTypeCode=DocumentTypeCodes[Math.Max(0,cmbDocumentType.SelectedIndex)];
        SourceType=SourceTypeCodes[Math.Max(0,cmbSourceType.SelectedIndex)];
        Search=string.IsNullOrWhiteSpace(txtSearch.Text)?null:txtSearch.Text.Trim();
        DialogResult=DialogResult.OK;
        Close();
    }

    private void ClearButton_Click(object? sender,EventArgs e)
    {
        CreatedFrom=null;
        CreatedTo=null;
        EnvironmentCode=null;
        Status=null;
        DocumentTypeCode=null;
        SourceType=null;
        Search=null;
        DialogResult=DialogResult.OK;
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

    private static int FindIndex(IReadOnlyList<string?> values,string? selected)
    {
        for (var index=0;index<values.Count;index++)
        {
            if (string.Equals(values[index],selected,StringComparison.OrdinalIgnoreCase))
                return index;
        }

        return 0;
    }
}

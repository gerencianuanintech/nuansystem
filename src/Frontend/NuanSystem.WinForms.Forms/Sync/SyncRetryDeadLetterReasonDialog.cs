using DevExpress.XtraEditors;
using NuanSystem.WinForms.Services.Sync.Models;

namespace NuanSystem.WinForms.Forms.Sync;

public sealed partial class SyncRetryDeadLetterReasonDialog : XtraForm
{
    public SyncRetryDeadLetterReasonDialog()
    {
        InitializeComponent();
        btnAccept.Click += BtnAccept_Click;
        memoReason.EditValueChanged += (_, _) => UpdateCharacterCount();
        UpdateCharacterCount();
    }

    public SyncRetryDeadLetterReasonDialog(SyncOutboxDetail? detail)
        : this()
    {
        if (detail is null)
        {
            return;
        }

        lblEventIdValue.Text = detail.Id.ToString("N0");
        lblEntityValue.Text = detail.EntityName;
        lblCodeValue.Text = detail.EntityCode ?? "-";
        lblStatusValue.Text = detail.Status.ToString();
        lblOperationValue.Text = detail.Operation.ToString();
    }

    public string Reason => memoReason.Text.Trim();

    private void BtnAccept_Click(object? sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(memoReason.Text))
        {
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        XtraMessageBox.Show(
            this,
            "Debe ingresar un motivo para reintentar DeadLetter.",
            Text,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        memoReason.Focus();
    }

    private void UpdateCharacterCount()
    {
        lblCharacterCount.Text = $"{memoReason.Text.Length:N0} / 500";
    }
}

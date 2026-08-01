using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapSyncExecutionFilterDialog : XtraForm
{
    public string? EntityCode => Value(entityEdit.Text); public string? Direction => Value(directionEdit.Text); public string? Status => Value(statusEdit.Text); public string? TriggerType => Value(triggerEdit.Text);
    public DateTime? DateFromUtc => fromEdit.EditValue is DateTime value ? value.Date.ToUniversalTime() : null; public DateTime? DateToUtc => toEdit.EditValue is DateTime value ? value.Date.AddDays(1).AddTicks(-1).ToUniversalTime() : null;
    public SapSyncExecutionFilterDialog() { InitializeComponent(); FormStyler.ApplyBase(this); acceptButton.Click += (_, _) => DialogResult = DialogResult.OK; }
    public SapSyncExecutionFilterDialog(SapSyncExecutionFilter filter, bool fixedProfile) : this() { entityEdit.Text = filter.EntityCode; directionEdit.Text = filter.Direction; statusEdit.Text = filter.Status; triggerEdit.Text = filter.TriggerType; fromEdit.EditValue = filter.DateFromUtc?.ToLocalTime(); toEdit.EditValue = filter.DateToUtc?.ToLocalTime(); Text = fixedProfile ? "Filtrar ejecuciones del perfil SAP" : "Filtrar ejecuciones SAP"; }
    private void ClearButton_Click(object? sender, EventArgs e) { entityEdit.Text = string.Empty; directionEdit.SelectedIndex = 0; statusEdit.SelectedIndex = 0; triggerEdit.SelectedIndex = 0; fromEdit.EditValue = null; toEdit.EditValue = null; DialogResult = DialogResult.OK; Close(); }
    private static string? Value(string? text) => string.IsNullOrWhiteSpace(text) || string.Equals(text, "Todos", StringComparison.OrdinalIgnoreCase) ? null : text.Trim();
}

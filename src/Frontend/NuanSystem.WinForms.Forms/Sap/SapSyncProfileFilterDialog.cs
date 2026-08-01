using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;
using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.Forms.Sap;

public sealed partial class SapSyncProfileFilterDialog : XtraForm
{
    public string? Search => string.IsNullOrWhiteSpace(searchEdit.Text) ? null : searchEdit.Text.Trim();
    public string? EntityCode => string.IsNullOrWhiteSpace(entityEdit.Text) ? null : entityEdit.Text.Trim();
    public bool? SelectedIsActive => statusEdit.SelectedIndex switch { 1 => true, 2 => false, _ => null };
    public SapSyncProfileFilterDialog() { InitializeComponent(); FormStyler.ApplyBase(this); acceptButton.Click += (_, _) => DialogResult = DialogResult.OK; }
    public SapSyncProfileFilterDialog(SapSyncProfileListFilter filter) : this() { searchEdit.Text = filter.Search; entityEdit.Text = filter.EntityCode; statusEdit.SelectedIndex = filter.IsActive switch { true => 1, false => 2, _ => 0 }; }
    private void ClearButton_Click(object? sender, EventArgs e) { searchEdit.Text = string.Empty; entityEdit.Text = string.Empty; statusEdit.SelectedIndex = 0; DialogResult = DialogResult.OK; Close(); }
}

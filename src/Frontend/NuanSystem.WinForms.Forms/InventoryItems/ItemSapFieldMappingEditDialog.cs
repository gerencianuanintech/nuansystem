using System.ComponentModel;
using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemSapFieldMappingEditDialog : XtraForm
{
    public ItemSapFieldMappingEditDialog()
        : this(null)
    {
    }

    public ItemSapFieldMappingEditDialog(ItemSapFieldMappingRow? row)
    {
        InitializeComponent();
        ConfigureForm();

        if (row is not null)
        {
            LoadRow(row);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemSapFieldMappingRow Row { get; private set; } = ItemSapFieldMappingRow.Empty;

    private void ConfigureForm()
    {
        chkRequired.Checked = true;
        chkEnabled.Checked = true;
        btnSave.Click += SaveButtonClick;
    }

    private void LoadRow(ItemSapFieldMappingRow row)
    {
        txtSystemField.Text = row.SystemField;
        txtSapField.Text = row.SapField;
        txtDescription.Text = row.Description;
        chkRequired.Checked = row.Required;
        chkEnabled.Checked = row.Enabled;
    }

    private void SaveButtonClick(object? sender, EventArgs e)
    {
        if (!ValidateForm())
        {
            return;
        }

        Row = new ItemSapFieldMappingRow(
            txtSystemField.Text.Trim(),
            txtSapField.Text.Trim(),
            txtDescription.Text.Trim(),
            chkRequired.Checked,
            chkEnabled.Checked);

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(txtSystemField.Text))
        {
            ShowValidation("Ingrese el campo del sistema.");
            txtSystemField.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtSapField.Text))
        {
            ShowValidation("Ingrese el campo SAP.");
            txtSapField.Focus();
            return false;
        }

        return true;
    }

    private void ShowValidation(string message)
    {
        XtraMessageBox.Show(this, message, "Campo SAP", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

public sealed record ItemSapFieldMappingRow(
    string SystemField,
    string SapField,
    string Description,
    bool Required,
    bool Enabled)
{
    public static ItemSapFieldMappingRow Empty { get; } = new(
        string.Empty,
        string.Empty,
        string.Empty,
        true,
        true);
}

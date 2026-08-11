using System.ComponentModel;
using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemOperationalAlertEditDialog : XtraForm
{
    public ItemOperationalAlertEditDialog()
        : this(null)
    {
    }

    public ItemOperationalAlertEditDialog(ItemOperationalAlertRow? row)
    {
        InitializeComponent();
        ConfigureForm();

        if (row is not null)
        {
            LoadRow(row);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemOperationalAlertRow Row { get; private set; } = ItemOperationalAlertRow.Empty;

    private void ConfigureForm()
    {
        cboAlertType.Properties.Items.AddRange(new object[] { "Advertencia", "Informativa", "Bloqueante" });
        cboProcess.Properties.Items.AddRange(new object[] { "Compras", "Ventas", "Inventario", "Logistica / Calidad", "SAP" });
        cboPriority.Properties.Items.AddRange(new object[] { "Baja", "Media", "Alta" });
        cboAlertType.SelectedIndex = 0;
        cboProcess.SelectedIndex = 0;
        cboPriority.SelectedItem = "Media";
        dtValidFrom.DateTime = DateTime.Today;
        chkActive.Checked = true;

        btnSave.Click += SaveButtonClick;
    }

    private void LoadRow(ItemOperationalAlertRow row)
    {
        cboAlertType.Text = row.AlertType;
        cboProcess.Text = row.Process;
        memMessage.Text = row.Message;
        dtValidFrom.DateTime = row.ValidFrom;
        dtValidTo.EditValue = row.ValidTo;
        chkBlocking.Checked = row.IsBlocking;
        chkActive.Checked = row.IsActive;
        cboPriority.Text = row.Priority;
        chkRequiresConfirmation.Checked = row.RequiresConfirmation;
    }

    private void SaveButtonClick(object? sender, EventArgs e)
    {
        if (!ValidateForm())
        {
            return;
        }

        Row = new ItemOperationalAlertRow(
            cboAlertType.Text.Trim(),
            cboProcess.Text.Trim(),
            memMessage.Text.Trim(),
            dtValidFrom.DateTime.Date,
            dtValidTo.EditValue is DateTime validTo ? validTo.Date : null,
            chkBlocking.Checked,
            chkActive.Checked,
            cboPriority.Text.Trim(),
            chkRequiresConfirmation.Checked);

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(cboAlertType.Text))
        {
            ShowValidation("Seleccione el tipo de alerta.");
            cboAlertType.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(cboProcess.Text))
        {
            ShowValidation("Seleccione el proceso.");
            cboProcess.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(memMessage.Text))
        {
            ShowValidation("Ingrese el mensaje de la alerta.");
            memMessage.Focus();
            return false;
        }

        if (dtValidTo.EditValue is DateTime validTo && validTo.Date < dtValidFrom.DateTime.Date)
        {
            ShowValidation("La fecha hasta no puede ser menor que la fecha desde.");
            dtValidTo.Focus();
            return false;
        }

        return true;
    }

    private void ShowValidation(string message)
    {
        XtraMessageBox.Show(this, message, "Alerta operativa", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

public sealed record ItemOperationalAlertRow(
    string AlertType,
    string Process,
    string Message,
    DateTime ValidFrom,
    DateTime? ValidTo,
    bool IsBlocking,
    bool IsActive,
    string Priority = "Media",
    bool RequiresConfirmation = false)
{
    public static ItemOperationalAlertRow Empty { get; } = new(
        "Advertencia",
        "Compras",
        string.Empty,
        DateTime.Today,
        null,
        false,
        true);
}

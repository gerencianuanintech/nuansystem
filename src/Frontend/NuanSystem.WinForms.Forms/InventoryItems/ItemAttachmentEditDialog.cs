using System.ComponentModel;
using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.InventoryItems;

public sealed partial class ItemAttachmentEditDialog : XtraForm
{
    public ItemAttachmentEditDialog()
        : this(null)
    {
    }

    public ItemAttachmentEditDialog(ItemAttachmentRow? row)
    {
        InitializeComponent();
        ConfigureForm();

        if (row is not null)
        {
            LoadRow(row);
        }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ItemAttachmentRow Row { get; private set; } = ItemAttachmentRow.Empty;

    private void ConfigureForm()
    {
        cboDocumentType.Properties.Items.AddRange(new object[]
        {
            "Imagen producto",
            "Ficha tecnica",
            "Registro sanitario",
            "Hoja logistica",
            "Documento comercial"
        });

        cboCategory.Properties.Items.AddRange(new object[] { "Comercial", "Compras", "Calidad", "Logistica", "Portal" });
        cboStatus.Properties.Items.AddRange(new object[] { "Activo", "Inactivo" });
        cboDocumentType.SelectedIndex = 0;
        cboCategory.SelectedIndex = 0;
        cboStatus.SelectedIndex = 0;
        dtUploadDate.DateTime = DateTime.Today;
        txtUser.Text = Environment.UserName;
        chkVisibleSales.Checked = true;

        btnSave.Click += SaveButtonClick;
    }

    private void LoadRow(ItemAttachmentRow row)
    {
        cboDocumentType.Text = row.DocumentType;
        txtFileName.Text = row.FileName;
        memDescription.Text = row.Description;
        cboCategory.Text = row.Category;
        txtExtension.Text = row.Extension;
        txtSize.Text = row.Size;
        dtUploadDate.DateTime = row.Date;
        txtUser.Text = row.User;
        chkPrincipal.Checked = row.IsMain;
        chkVisibleSales.Checked = row.VisibleInSales;
        chkVisiblePurchases.Checked = row.VisibleInPurchases;
        chkVisiblePortal.Checked = row.VisibleInPortal;
        cboStatus.Text = row.Status;
    }

    private void SaveButtonClick(object? sender, EventArgs e)
    {
        if (!ValidateForm())
        {
            return;
        }

        Row = new ItemAttachmentRow(
            cboDocumentType.Text.Trim(),
            txtFileName.Text.Trim(),
            memDescription.Text.Trim(),
            cboCategory.Text.Trim(),
            txtExtension.Text.Trim(),
            txtSize.Text.Trim(),
            dtUploadDate.DateTime.Date,
            txtUser.Text.Trim(),
            chkPrincipal.Checked,
            chkVisibleSales.Checked,
            chkVisiblePurchases.Checked,
            chkVisiblePortal.Checked,
            cboStatus.Text.Trim());

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(cboDocumentType.Text))
        {
            ShowValidation("Seleccione el tipo de documento.");
            cboDocumentType.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtFileName.Text))
        {
            ShowValidation("Ingrese el nombre del archivo.");
            txtFileName.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtExtension.Text))
        {
            ShowValidation("Ingrese la extension del archivo.");
            txtExtension.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(cboStatus.Text))
        {
            ShowValidation("Seleccione el estado.");
            cboStatus.Focus();
            return false;
        }

        return true;
    }

    private void ShowValidation(string message)
    {
        XtraMessageBox.Show(this, message, "Anexo del item", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}

public sealed record ItemAttachmentRow(
    string DocumentType,
    string FileName,
    string Description,
    string Category,
    string Extension,
    string Size,
    DateTime Date,
    string User,
    bool IsMain,
    bool VisibleInSales,
    bool VisibleInPurchases,
    bool VisibleInPortal,
    string Status)
{
    public static ItemAttachmentRow Empty { get; } = new(
        "Imagen producto",
        string.Empty,
        string.Empty,
        "Comercial",
        "PNG",
        string.Empty,
        DateTime.Today,
        string.Empty,
        false,
        true,
        false,
        false,
        "Activo");
}

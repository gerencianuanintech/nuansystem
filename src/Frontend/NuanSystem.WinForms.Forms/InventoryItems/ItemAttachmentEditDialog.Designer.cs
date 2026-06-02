using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace NuanSystem.WinForms.Forms.InventoryItems;

partial class ItemAttachmentEditDialog
{
    private System.ComponentModel.IContainer components = null;
    private SimpleButton btnCancel;
    private SimpleButton btnSave;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        btnCancel = new SimpleButton();
        btnSave = new SimpleButton();
        lblDocumentType = new LabelControl();
        cboDocumentType = new ComboBoxEdit();
        lblFileName = new LabelControl();
        txtFileName = new TextEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblCategory = new LabelControl();
        cboCategory = new ComboBoxEdit();
        lblExtension = new LabelControl();
        txtExtension = new TextEdit();
        lblSize = new LabelControl();
        txtSize = new TextEdit();
        lblUploadDate = new LabelControl();
        dtUploadDate = new DateEdit();
        lblUser = new LabelControl();
        txtUser = new TextEdit();
        chkPrincipal = new CheckEdit();
        chkVisibleSales = new CheckEdit();
        chkVisiblePurchases = new CheckEdit();
        chkVisiblePortal = new CheckEdit();
        lblStatus = new LabelControl();
        cboStatus = new ComboBoxEdit();
        ((System.ComponentModel.ISupportInitialize)cboDocumentType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtFileName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboCategory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtExtension.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSize.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtUploadDate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtUploadDate.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtUser.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkPrincipal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleSales.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkVisiblePurchases.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkVisiblePortal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cboStatus.Properties).BeginInit();
        SuspendLayout();
        // 
        // btnCancel
        // 
        btnCancel.Appearance.BackColor = Color.White;
        btnCancel.Appearance.BorderColor = Color.FromArgb(210, 214, 219);
        btnCancel.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancel.Appearance.ForeColor = Color.FromArgb(45, 55, 72);
        btnCancel.Appearance.Options.UseBackColor = true;
        btnCancel.Appearance.Options.UseBorderColor = true;
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.Appearance.Options.UseForeColor = true;
        btnCancel.ButtonStyle = BorderStyles.Simple;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(355, 250);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "Cancelar";
        // 
        // btnSave
        // 
        btnSave.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseBorderColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.ButtonStyle = BorderStyles.Simple;
        btnSave.Location = new Point(461, 250);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 2;
        btnSave.Text = "Guardar";
        // 
        // lblDocumentType
        // 
        lblDocumentType.Appearance.Font = new Font("Segoe UI", 9F);
        lblDocumentType.Appearance.Options.UseFont = true;
        lblDocumentType.Location = new Point(17, 15);
        lblDocumentType.Name = "lblDocumentType";
        lblDocumentType.Size = new Size(92, 15);
        lblDocumentType.TabIndex = 22;
        lblDocumentType.Text = "Tipo documento:";
        // 
        // cboDocumentType
        // 
        cboDocumentType.Location = new Point(132, 12);
        cboDocumentType.Name = "cboDocumentType";
        cboDocumentType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboDocumentType.Properties.Appearance.Options.UseFont = true;
        cboDocumentType.Size = new Size(190, 22);
        cboDocumentType.TabIndex = 23;
        // 
        // lblFileName
        // 
        lblFileName.Appearance.Font = new Font("Segoe UI", 9F);
        lblFileName.Appearance.Options.UseFont = true;
        lblFileName.Location = new Point(17, 43);
        lblFileName.Name = "lblFileName";
        lblFileName.Size = new Size(89, 15);
        lblFileName.TabIndex = 24;
        lblFileName.Text = "Nombre archivo:";
        // 
        // txtFileName
        // 
        txtFileName.Location = new Point(132, 40);
        txtFileName.Name = "txtFileName";
        txtFileName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtFileName.Properties.Appearance.Options.UseFont = true;
        txtFileName.Size = new Size(429, 22);
        txtFileName.TabIndex = 25;
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Location = new Point(17, 70);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(65, 15);
        lblDescription.TabIndex = 26;
        lblDescription.Text = "Descripcion:";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(132, 68);
        memDescription.Name = "memDescription";
        memDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescription.Properties.Appearance.Options.UseFont = true;
        memDescription.Size = new Size(429, 50);
        memDescription.TabIndex = 27;
        // 
        // lblCategory
        // 
        lblCategory.Appearance.Font = new Font("Segoe UI", 9F);
        lblCategory.Appearance.Options.UseFont = true;
        lblCategory.Location = new Point(17, 127);
        lblCategory.Name = "lblCategory";
        lblCategory.Size = new Size(54, 15);
        lblCategory.TabIndex = 28;
        lblCategory.Text = "Categoria:";
        // 
        // cboCategory
        // 
        cboCategory.Location = new Point(132, 124);
        cboCategory.Name = "cboCategory";
        cboCategory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboCategory.Properties.Appearance.Options.UseFont = true;
        cboCategory.Size = new Size(190, 22);
        cboCategory.TabIndex = 29;
        // 
        // lblExtension
        // 
        lblExtension.Appearance.Font = new Font("Segoe UI", 9F);
        lblExtension.Appearance.Options.UseFont = true;
        lblExtension.Location = new Point(349, 127);
        lblExtension.Name = "lblExtension";
        lblExtension.Size = new Size(53, 15);
        lblExtension.TabIndex = 30;
        lblExtension.Text = "Extension:";
        // 
        // txtExtension
        // 
        txtExtension.Location = new Point(429, 124);
        txtExtension.Name = "txtExtension";
        txtExtension.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtExtension.Properties.Appearance.Options.UseFont = true;
        txtExtension.Size = new Size(132, 22);
        txtExtension.TabIndex = 31;
        // 
        // lblSize
        // 
        lblSize.Appearance.Font = new Font("Segoe UI", 9F);
        lblSize.Appearance.Options.UseFont = true;
        lblSize.Location = new Point(17, 155);
        lblSize.Name = "lblSize";
        lblSize.Size = new Size(47, 15);
        lblSize.TabIndex = 32;
        lblSize.Text = "Tamano:";
        // 
        // txtSize
        // 
        txtSize.Location = new Point(132, 152);
        txtSize.Name = "txtSize";
        txtSize.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSize.Properties.Appearance.Options.UseFont = true;
        txtSize.Size = new Size(190, 22);
        txtSize.TabIndex = 33;
        // 
        // lblUploadDate
        // 
        lblUploadDate.Appearance.Font = new Font("Segoe UI", 9F);
        lblUploadDate.Appearance.Options.UseFont = true;
        lblUploadDate.Location = new Point(349, 155);
        lblUploadDate.Name = "lblUploadDate";
        lblUploadDate.Size = new Size(66, 15);
        lblUploadDate.TabIndex = 34;
        lblUploadDate.Text = "Fecha carga:";
        // 
        // dtUploadDate
        // 
        dtUploadDate.EditValue = null;
        dtUploadDate.Location = new Point(429, 152);
        dtUploadDate.Name = "dtUploadDate";
        dtUploadDate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dtUploadDate.Properties.Appearance.Options.UseFont = true;
        dtUploadDate.Size = new Size(132, 22);
        dtUploadDate.TabIndex = 35;
        // 
        // lblUser
        // 
        lblUser.Appearance.Font = new Font("Segoe UI", 9F);
        lblUser.Appearance.Options.UseFont = true;
        lblUser.Location = new Point(17, 183);
        lblUser.Name = "lblUser";
        lblUser.Size = new Size(43, 15);
        lblUser.TabIndex = 36;
        lblUser.Text = "Usuario:";
        // 
        // txtUser
        // 
        txtUser.Location = new Point(132, 180);
        txtUser.Name = "txtUser";
        txtUser.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtUser.Properties.Appearance.Options.UseFont = true;
        txtUser.Size = new Size(190, 22);
        txtUser.TabIndex = 37;
        // 
        // chkPrincipal
        // 
        chkPrincipal.Location = new Point(132, 211);
        chkPrincipal.Name = "chkPrincipal";
        chkPrincipal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkPrincipal.Properties.Appearance.Options.UseFont = true;
        chkPrincipal.Properties.Caption = "Principal";
        chkPrincipal.Size = new Size(75, 20);
        chkPrincipal.TabIndex = 38;
        // 
        // chkVisibleSales
        // 
        chkVisibleSales.Location = new Point(213, 211);
        chkVisibleSales.Name = "chkVisibleSales";
        chkVisibleSales.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkVisibleSales.Properties.Appearance.Options.UseFont = true;
        chkVisibleSales.Properties.Caption = "Visible en ventas";
        chkVisibleSales.Size = new Size(109, 20);
        chkVisibleSales.TabIndex = 39;
        // 
        // chkVisiblePurchases
        // 
        chkVisiblePurchases.Location = new Point(327, 211);
        chkVisiblePurchases.Name = "chkVisiblePurchases";
        chkVisiblePurchases.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkVisiblePurchases.Properties.Appearance.Options.UseFont = true;
        chkVisiblePurchases.Properties.Caption = "Visible en compras";
        chkVisiblePurchases.Size = new Size(123, 20);
        chkVisiblePurchases.TabIndex = 40;
        // 
        // chkVisiblePortal
        // 
        chkVisiblePortal.Location = new Point(456, 211);
        chkVisiblePortal.Name = "chkVisiblePortal";
        chkVisiblePortal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkVisiblePortal.Properties.Appearance.Options.UseFont = true;
        chkVisiblePortal.Properties.Caption = "Visible en portal";
        chkVisiblePortal.Size = new Size(105, 20);
        chkVisiblePortal.TabIndex = 41;
        // 
        // lblStatus
        // 
        lblStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblStatus.Appearance.Options.UseFont = true;
        lblStatus.Location = new Point(349, 183);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(38, 15);
        lblStatus.TabIndex = 42;
        lblStatus.Text = "Estado:";
        // 
        // cboStatus
        // 
        cboStatus.Location = new Point(429, 180);
        cboStatus.Name = "cboStatus";
        cboStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cboStatus.Properties.Appearance.Options.UseFont = true;
        cboStatus.Size = new Size(132, 22);
        cboStatus.TabIndex = 43;
        // 
        // ItemAttachmentEditDialog
        // 
        AcceptButton = btnSave;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(596, 300);
        Controls.Add(lblDocumentType);
        Controls.Add(cboDocumentType);
        Controls.Add(lblFileName);
        Controls.Add(txtFileName);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(lblCategory);
        Controls.Add(cboCategory);
        Controls.Add(lblExtension);
        Controls.Add(txtExtension);
        Controls.Add(lblSize);
        Controls.Add(txtSize);
        Controls.Add(lblUploadDate);
        Controls.Add(dtUploadDate);
        Controls.Add(lblUser);
        Controls.Add(txtUser);
        Controls.Add(chkPrincipal);
        Controls.Add(chkVisibleSales);
        Controls.Add(chkVisiblePurchases);
        Controls.Add(chkVisiblePortal);
        Controls.Add(lblStatus);
        Controls.Add(cboStatus);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ItemAttachmentEditDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Anexo del item";
        ((System.ComponentModel.ISupportInitialize)cboDocumentType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtFileName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboCategory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtExtension.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSize.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtUploadDate.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtUploadDate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtUser.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkPrincipal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleSales.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkVisiblePurchases.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkVisiblePortal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cboStatus.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
    private LabelControl lblDocumentType;
    private ComboBoxEdit cboDocumentType;
    private LabelControl lblFileName;
    private TextEdit txtFileName;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblCategory;
    private ComboBoxEdit cboCategory;
    private LabelControl lblExtension;
    private TextEdit txtExtension;
    private LabelControl lblSize;
    private TextEdit txtSize;
    private LabelControl lblUploadDate;
    private DateEdit dtUploadDate;
    private LabelControl lblUser;
    private TextEdit txtUser;
    private CheckEdit chkPrincipal;
    private CheckEdit chkVisibleSales;
    private CheckEdit chkVisiblePurchases;
    private CheckEdit chkVisiblePortal;
    private LabelControl lblStatus;
    private ComboBoxEdit cboStatus;
}

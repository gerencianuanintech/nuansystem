using System.Drawing;
using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.InventoryItems;

partial class ItemSapFieldMappingEditDialog
{
    private System.ComponentModel.IContainer components = null;
    private PanelControl pnlContent;
    private LabelControl lblSystemField;
    private TextEdit txtSystemField;
    private LabelControl lblSapField;
    private TextEdit txtSapField;
    private LabelControl lblDescription;
    private TextEdit txtDescription;
    private CheckEdit chkRequired;
    private CheckEdit chkEnabled;
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
        components = new System.ComponentModel.Container();
        pnlContent = new PanelControl();
        lblSystemField = new LabelControl();
        txtSystemField = new TextEdit();
        lblSapField = new LabelControl();
        txtSapField = new TextEdit();
        lblDescription = new LabelControl();
        txtDescription = new TextEdit();
        chkRequired = new CheckEdit();
        chkEnabled = new CheckEdit();
        btnCancel = new SimpleButton();
        btnSave = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)pnlContent).BeginInit();
        pnlContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)txtSystemField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkRequired.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkEnabled.Properties).BeginInit();
        SuspendLayout();
        // 
        // pnlContent
        // 
        pnlContent.Controls.Add(lblSystemField);
        pnlContent.Controls.Add(txtSystemField);
        pnlContent.Controls.Add(lblSapField);
        pnlContent.Controls.Add(txtSapField);
        pnlContent.Controls.Add(lblDescription);
        pnlContent.Controls.Add(txtDescription);
        pnlContent.Controls.Add(chkRequired);
        pnlContent.Controls.Add(chkEnabled);
        pnlContent.Location = new Point(12, 12);
        pnlContent.Name = "pnlContent";
        pnlContent.Size = new Size(452, 160);
        pnlContent.TabIndex = 0;
        // 
        // lblSystemField
        // 
        lblSystemField.Appearance.Font = new Font("Segoe UI", 9F);
        lblSystemField.Appearance.Options.UseFont = true;
        lblSystemField.Location = new Point(18, 24);
        lblSystemField.Name = "lblSystemField";
        lblSystemField.Size = new Size(91, 15);
        lblSystemField.TabIndex = 0;
        lblSystemField.Text = "Campo sistema:";
        // 
        // txtSystemField
        // 
        txtSystemField.Location = new Point(132, 21);
        txtSystemField.Name = "txtSystemField";
        txtSystemField.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSystemField.Properties.Appearance.Options.UseFont = true;
        txtSystemField.Size = new Size(296, 22);
        txtSystemField.TabIndex = 1;
        // 
        // lblSapField
        // 
        lblSapField.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapField.Appearance.Options.UseFont = true;
        lblSapField.Location = new Point(18, 58);
        lblSapField.Name = "lblSapField";
        lblSapField.Size = new Size(61, 15);
        lblSapField.TabIndex = 2;
        lblSapField.Text = "Campo SAP:";
        // 
        // txtSapField
        // 
        txtSapField.Location = new Point(132, 55);
        txtSapField.Name = "txtSapField";
        txtSapField.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapField.Properties.Appearance.Options.UseFont = true;
        txtSapField.Size = new Size(296, 22);
        txtSapField.TabIndex = 3;
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Location = new Point(18, 92);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(63, 15);
        lblDescription.TabIndex = 4;
        lblDescription.Text = "Descripcion:";
        // 
        // txtDescription
        // 
        txtDescription.Location = new Point(132, 89);
        txtDescription.Name = "txtDescription";
        txtDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtDescription.Properties.Appearance.Options.UseFont = true;
        txtDescription.Size = new Size(296, 22);
        txtDescription.TabIndex = 5;
        // 
        // chkRequired
        // 
        chkRequired.Location = new Point(132, 124);
        chkRequired.Name = "chkRequired";
        chkRequired.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkRequired.Properties.Appearance.Options.UseFont = true;
        chkRequired.Properties.Caption = "Obligatorio";
        chkRequired.Size = new Size(94, 22);
        chkRequired.TabIndex = 6;
        // 
        // chkEnabled
        // 
        chkEnabled.Location = new Point(244, 124);
        chkEnabled.Name = "chkEnabled";
        chkEnabled.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkEnabled.Properties.Appearance.Options.UseFont = true;
        chkEnabled.Properties.Caption = "Activo";
        chkEnabled.Size = new Size(70, 22);
        chkEnabled.TabIndex = 7;
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
        btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(258, 188);
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
        btnSave.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
        btnSave.Location = new Point(364, 188);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 2;
        btnSave.Text = "Guardar";
        // 
        // ItemSapFieldMappingEditDialog
        // 
        AcceptButton = btnSave;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(476, 236);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(pnlContent);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ItemSapFieldMappingEditDialog";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Campo sincronizado SAP";
        ((System.ComponentModel.ISupportInitialize)pnlContent).EndInit();
        pnlContent.ResumeLayout(false);
        pnlContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)txtSystemField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkRequired.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkEnabled.Properties).EndInit();
        ResumeLayout(false);
    }
}

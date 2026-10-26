using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Sap;

partial class SapIdentityPolicyForm
{
    private System.ComponentModel.IContainer components = null;
    private LabelControl lblPrefixesTitle;
    private LabelControl linePrefixesTitle;
    private LabelControl lblPassportTitle;
    private LabelControl linePassportTitle;
    private LabelControl lblCustomerNational;
    private LabelControl lblCustomerForeign;
    private LabelControl lblSupplierNational;
    private LabelControl lblSupplierForeign;
    private LabelControl lblPassportTypes;
    private LabelControl lblPassportIdentification;
    private TextEdit txtCustomerNational;
    private TextEdit txtCustomerForeign;
    private TextEdit txtSupplierNational;
    private TextEdit txtSupplierForeign;
    private TextEdit txtPassportTypes;
    private TextEdit txtPassportIdentification;

    private void InitializeComponent()
    {
        lblPrefixesTitle = new LabelControl();
        linePrefixesTitle = new LabelControl();
        lblPassportTitle = new LabelControl();
        linePassportTitle = new LabelControl();
        lblCustomerNational = new LabelControl();
        txtCustomerNational = new TextEdit();
        lblCustomerForeign = new LabelControl();
        txtCustomerForeign = new TextEdit();
        lblSupplierNational = new LabelControl();
        txtSupplierNational = new TextEdit();
        lblSupplierForeign = new LabelControl();
        txtSupplierForeign = new TextEdit();
        lblPassportTypes = new LabelControl();
        txtPassportTypes = new TextEdit();
        lblPassportIdentification = new LabelControl();
        txtPassportIdentification = new TextEdit();
        ((System.ComponentModel.ISupportInitialize)txtCustomerNational.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCustomerForeign.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierNational.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierForeign.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPassportTypes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPassportIdentification.Properties).BeginInit();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Location = new Point(628, 248);
        // 
        // btnGuardar
        // 
        btnGuardar.Location = new Point(734, 248);
        // 
        // lblPrefixesTitle
        // 
        lblPrefixesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPrefixesTitle.Appearance.ForeColor = BrandResources.Primary;
        lblPrefixesTitle.Appearance.Options.UseFont = true;
        lblPrefixesTitle.Appearance.Options.UseForeColor = true;
        lblPrefixesTitle.Location = new Point(32, 22);
        lblPrefixesTitle.Name = "lblPrefixesTitle";
        lblPrefixesTitle.Size = new Size(145, 20);
        lblPrefixesTitle.TabIndex = 20;
        lblPrefixesTitle.Text = "1. Prefijos de socios";
        // 
        // linePrefixesTitle
        // 
        linePrefixesTitle.Appearance.BackColor = BrandResources.Border;
        linePrefixesTitle.Appearance.Options.UseBackColor = true;
        linePrefixesTitle.AutoSizeMode = LabelAutoSizeMode.None;
        linePrefixesTitle.Location = new Point(196, 34);
        linePrefixesTitle.Name = "linePrefixesTitle";
        linePrefixesTitle.Size = new Size(642, 1);
        linePrefixesTitle.TabIndex = 21;
        // 
        // lblPassportTitle
        // 
        lblPassportTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPassportTitle.Appearance.ForeColor = BrandResources.Primary;
        lblPassportTitle.Appearance.Options.UseFont = true;
        lblPassportTitle.Appearance.Options.UseForeColor = true;
        lblPassportTitle.Location = new Point(32, 133);
        lblPassportTitle.Name = "lblPassportTitle";
        lblPassportTitle.Size = new Size(156, 20);
        lblPassportTitle.TabIndex = 22;
        lblPassportTitle.Text = "2. Identificación";
        // 
        // linePassportTitle
        // 
        linePassportTitle.Appearance.BackColor = BrandResources.Border;
        linePassportTitle.Appearance.Options.UseBackColor = true;
        linePassportTitle.AutoSizeMode = LabelAutoSizeMode.None;
        linePassportTitle.Location = new Point(206, 145);
        linePassportTitle.Name = "linePassportTitle";
        linePassportTitle.Size = new Size(632, 1);
        linePassportTitle.TabIndex = 23;
        // 
        // lblCustomerNational
        // 
        lblCustomerNational.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerNational.Appearance.ForeColor = BrandResources.Text;
        lblCustomerNational.Appearance.Options.UseFont = true;
        lblCustomerNational.Appearance.Options.UseForeColor = true;
        lblCustomerNational.Location = new Point(32, 63);
        lblCustomerNational.Name = "lblCustomerNational";
        lblCustomerNational.Size = new Size(112, 15);
        lblCustomerNational.TabIndex = 24;
        lblCustomerNational.Text = "Cliente nacional:";
        // 
        // txtCustomerNational
        // 
        txtCustomerNational.Location = new Point(154, 60);
        txtCustomerNational.Name = "txtCustomerNational";
        txtCustomerNational.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCustomerNational.Properties.Appearance.Options.UseFont = true;
        txtCustomerNational.Properties.AutoHeight = false;
        txtCustomerNational.Properties.BorderStyle = BorderStyles.Simple;
        txtCustomerNational.Properties.MaxLength = 50;
        txtCustomerNational.Size = new Size(180, 22);
        txtCustomerNational.TabIndex = 0;
        // 
        // lblCustomerForeign
        // 
        lblCustomerForeign.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerForeign.Appearance.ForeColor = BrandResources.Text;
        lblCustomerForeign.Appearance.Options.UseFont = true;
        lblCustomerForeign.Appearance.Options.UseForeColor = true;
        lblCustomerForeign.Location = new Point(32, 91);
        lblCustomerForeign.Name = "lblCustomerForeign";
        lblCustomerForeign.Size = new Size(112, 15);
        lblCustomerForeign.TabIndex = 25;
        lblCustomerForeign.Text = "Cliente extranjero:";
        // 
        // txtCustomerForeign
        // 
        txtCustomerForeign.Location = new Point(154, 88);
        txtCustomerForeign.Name = "txtCustomerForeign";
        txtCustomerForeign.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCustomerForeign.Properties.Appearance.Options.UseFont = true;
        txtCustomerForeign.Properties.AutoHeight = false;
        txtCustomerForeign.Properties.BorderStyle = BorderStyles.Simple;
        txtCustomerForeign.Properties.MaxLength = 50;
        txtCustomerForeign.Size = new Size(180, 22);
        txtCustomerForeign.TabIndex = 1;
        // 
        // lblSupplierNational
        // 
        lblSupplierNational.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierNational.Appearance.ForeColor = BrandResources.Text;
        lblSupplierNational.Appearance.Options.UseFont = true;
        lblSupplierNational.Appearance.Options.UseForeColor = true;
        lblSupplierNational.Location = new Point(450, 63);
        lblSupplierNational.Name = "lblSupplierNational";
        lblSupplierNational.Size = new Size(118, 15);
        lblSupplierNational.TabIndex = 26;
        lblSupplierNational.Text = "Proveedor nacional:";
        // 
        // txtSupplierNational
        // 
        txtSupplierNational.Location = new Point(572, 60);
        txtSupplierNational.Name = "txtSupplierNational";
        txtSupplierNational.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierNational.Properties.Appearance.Options.UseFont = true;
        txtSupplierNational.Properties.AutoHeight = false;
        txtSupplierNational.Properties.BorderStyle = BorderStyles.Simple;
        txtSupplierNational.Properties.MaxLength = 50;
        txtSupplierNational.Size = new Size(180, 22);
        txtSupplierNational.TabIndex = 2;
        // 
        // lblSupplierForeign
        // 
        lblSupplierForeign.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierForeign.Appearance.ForeColor = BrandResources.Text;
        lblSupplierForeign.Appearance.Options.UseFont = true;
        lblSupplierForeign.Appearance.Options.UseForeColor = true;
        lblSupplierForeign.Location = new Point(450, 91);
        lblSupplierForeign.Name = "lblSupplierForeign";
        lblSupplierForeign.Size = new Size(118, 15);
        lblSupplierForeign.TabIndex = 27;
        lblSupplierForeign.Text = "Proveedor extranjero:";
        // 
        // txtSupplierForeign
        // 
        txtSupplierForeign.Location = new Point(572, 88);
        txtSupplierForeign.Name = "txtSupplierForeign";
        txtSupplierForeign.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierForeign.Properties.Appearance.Options.UseFont = true;
        txtSupplierForeign.Properties.AutoHeight = false;
        txtSupplierForeign.Properties.BorderStyle = BorderStyles.Simple;
        txtSupplierForeign.Properties.MaxLength = 50;
        txtSupplierForeign.Size = new Size(180, 22);
        txtSupplierForeign.TabIndex = 3;
        // 
        // lblPassportTypes
        // 
        lblPassportTypes.Appearance.Font = new Font("Segoe UI", 9F);
        lblPassportTypes.Appearance.ForeColor = BrandResources.Text;
        lblPassportTypes.Appearance.Options.UseFont = true;
        lblPassportTypes.Appearance.Options.UseForeColor = true;
        lblPassportTypes.Location = new Point(32, 174);
        lblPassportTypes.Name = "lblPassportTypes";
        lblPassportTypes.Size = new Size(112, 15);
        lblPassportTypes.TabIndex = 28;
        lblPassportTypes.Text = "Tipos tributarios:";
        // 
        // txtPassportTypes
        // 
        txtPassportTypes.Location = new Point(154, 171);
        txtPassportTypes.Name = "txtPassportTypes";
        txtPassportTypes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPassportTypes.Properties.Appearance.Options.UseFont = true;
        txtPassportTypes.Properties.AutoHeight = false;
        txtPassportTypes.Properties.BorderStyle = BorderStyles.Simple;
        txtPassportTypes.Properties.MaxLength = 250;
        txtPassportTypes.Size = new Size(240, 22);
        txtPassportTypes.TabIndex = 4;
        // 
        // lblPassportIdentification
        // 
        lblPassportIdentification.Appearance.Font = new Font("Segoe UI", 9F);
        lblPassportIdentification.Appearance.ForeColor = BrandResources.Text;
        lblPassportIdentification.Appearance.Options.UseFont = true;
        lblPassportIdentification.Appearance.Options.UseForeColor = true;
        lblPassportIdentification.Location = new Point(450, 174);
        lblPassportIdentification.Name = "lblPassportIdentification";
        lblPassportIdentification.Size = new Size(118, 15);
        lblPassportIdentification.TabIndex = 29;
        lblPassportIdentification.Text = "Tipo identificación:";
        // 
        // txtPassportIdentification
        // 
        txtPassportIdentification.Location = new Point(572, 171);
        txtPassportIdentification.Name = "txtPassportIdentification";
        txtPassportIdentification.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPassportIdentification.Properties.Appearance.Options.UseFont = true;
        txtPassportIdentification.Properties.AutoHeight = false;
        txtPassportIdentification.Properties.BorderStyle = BorderStyles.Simple;
        txtPassportIdentification.Properties.MaxLength = 50;
        txtPassportIdentification.Size = new Size(180, 22);
        txtPassportIdentification.TabIndex = 5;
        // 
        // SapIdentityPolicyForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButtonLocation = new Point(628, 248);
        ClientSize = new Size(870, 294);
        Controls.Add(lblPrefixesTitle);
        Controls.Add(linePrefixesTitle);
        Controls.Add(lblPassportTitle);
        Controls.Add(linePassportTitle);
        Controls.Add(lblCustomerNational);
        Controls.Add(txtCustomerNational);
        Controls.Add(lblCustomerForeign);
        Controls.Add(txtCustomerForeign);
        Controls.Add(lblSupplierNational);
        Controls.Add(txtSupplierNational);
        Controls.Add(lblSupplierForeign);
        Controls.Add(txtSupplierForeign);
        Controls.Add(lblPassportTypes);
        Controls.Add(txtPassportTypes);
        Controls.Add(lblPassportIdentification);
        Controls.Add(txtPassportIdentification);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(886, 333);
        Name = "SapIdentityPolicyForm";
        SaveButtonLocation = new Point(734, 248);
        StartPosition = FormStartPosition.CenterParent;
        Text = "Política de identidad SAP";
        Controls.SetChildIndex(btnCancelar, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        ((System.ComponentModel.ISupportInitialize)txtCustomerNational.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCustomerForeign.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierNational.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierForeign.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPassportTypes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPassportIdentification.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }
}

using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.Definitions.General.Countries;

partial class CountryEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblIso2 = new LabelControl();
        txtIso2 = new TextEdit();
        lblIso3 = new LabelControl();
        txtIso3 = new TextEdit();
        lblPhonePrefix = new LabelControl();
        txtPhonePrefix = new TextEdit();
        chkIsActive = new CheckEdit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtIso2.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtIso3.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPhonePrefix.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        SuspendLayout();
        btnCancelar.Location = new Point(294, 180);
        btnCancelar.TabIndex = 6;
        btnGuardar.Location = new Point(400, 180);
        btnGuardar.TabIndex = 7;
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Location = new Point(28, 29);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(40, 15);
        lblCode.Text = "Código";
        txtCode.Location = new Point(170, 26);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 10;
        txtCode.Size = new Size(330, 22);
        txtCode.TabIndex = 0;
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.Options.UseFont = true;
        lblName.Location = new Point(28, 57);
        lblName.Name = "lblName";
        lblName.Size = new Size(44, 15);
        lblName.Text = "Nombre";
        txtName.Location = new Point(170, 54);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 120;
        txtName.Size = new Size(330, 22);
        txtName.TabIndex = 1;
        lblIso2.Appearance.Font = new Font("Segoe UI", 9F);
        lblIso2.Appearance.Options.UseFont = true;
        lblIso2.Location = new Point(28, 85);
        lblIso2.Name = "lblIso2";
        lblIso2.Size = new Size(26, 15);
        lblIso2.Text = "ISO2";
        txtIso2.Location = new Point(170, 82);
        txtIso2.Name = "txtIso2";
        txtIso2.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtIso2.Properties.Appearance.Options.UseFont = true;
        txtIso2.Properties.AutoHeight = false;
        txtIso2.Properties.MaxLength = 2;
        txtIso2.Size = new Size(120, 22);
        txtIso2.TabIndex = 2;
        lblIso3.Appearance.Font = new Font("Segoe UI", 9F);
        lblIso3.Appearance.Options.UseFont = true;
        lblIso3.Location = new Point(315, 85);
        lblIso3.Name = "lblIso3";
        lblIso3.Size = new Size(26, 15);
        lblIso3.Text = "ISO3";
        txtIso3.Location = new Point(380, 82);
        txtIso3.Name = "txtIso3";
        txtIso3.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtIso3.Properties.Appearance.Options.UseFont = true;
        txtIso3.Properties.AutoHeight = false;
        txtIso3.Properties.MaxLength = 3;
        txtIso3.Size = new Size(120, 22);
        txtIso3.TabIndex = 3;
        lblPhonePrefix.Appearance.Font = new Font("Segoe UI", 9F);
        lblPhonePrefix.Appearance.Options.UseFont = true;
        lblPhonePrefix.Location = new Point(28, 113);
        lblPhonePrefix.Name = "lblPhonePrefix";
        lblPhonePrefix.Size = new Size(83, 15);
        lblPhonePrefix.Text = "Prefijo teléfono";
        txtPhonePrefix.Location = new Point(170, 110);
        txtPhonePrefix.Name = "txtPhonePrefix";
        txtPhonePrefix.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPhonePrefix.Properties.Appearance.Options.UseFont = true;
        txtPhonePrefix.Properties.AutoHeight = false;
        txtPhonePrefix.Properties.MaxLength = 10;
        txtPhonePrefix.Size = new Size(330, 22);
        txtPhonePrefix.TabIndex = 4;
        chkIsActive.EditValue = true;
        chkIsActive.Location = new Point(166, 140);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsActive.Properties.Appearance.Options.UseFont = true;
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(75, 20);
        chkIsActive.TabIndex = 5;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(528, 238);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblIso2);
        Controls.Add(txtIso2);
        Controls.Add(lblIso3);
        Controls.Add(txtIso3);
        Controls.Add(lblPhonePrefix);
        Controls.Add(txtPhonePrefix);
        Controls.Add(chkIsActive);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "CountryEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "País";
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtIso2.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtIso3.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPhonePrefix.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
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

    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblIso2;
    private TextEdit txtIso2;
    private LabelControl lblIso3;
    private TextEdit txtIso3;
    private LabelControl lblPhonePrefix;
    private TextEdit txtPhonePrefix;
    private CheckEdit chkIsActive;
}

using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace NuanSystem.WinForms.Forms.Carriers;

partial class CarrierEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblIdentificationType = new LabelControl();
        lueIdentificationType = new LookUpEdit();
        lblIdentificationNumber = new LabelControl();
        txtIdentificationNumber = new TextEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        chkIsActive = new CheckEdit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueIdentificationType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtIdentificationNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.BorderColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancelar.Appearance.ForeColor = Color.White;
        btnCancelar.Appearance.Options.UseBackColor = true;
        btnCancelar.Appearance.Options.UseBorderColor = true;
        btnCancelar.Appearance.Options.UseFont = true;
        btnCancelar.Appearance.Options.UseForeColor = true;
        btnCancelar.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.BorderColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.ForeColor = Color.White;
        btnCancelar.AppearanceHovered.Options.UseBackColor = true;
        btnCancelar.AppearanceHovered.Options.UseBorderColor = true;
        btnCancelar.AppearanceHovered.Options.UseForeColor = true;
        btnCancelar.AppearancePressed.BackColor = Color.FromArgb(60, 67, 70);
        btnCancelar.AppearancePressed.BorderColor = Color.FromArgb(60, 67, 70);
        btnCancelar.AppearancePressed.ForeColor = Color.White;
        btnCancelar.AppearancePressed.Options.UseBackColor = true;
        btnCancelar.AppearancePressed.Options.UseBorderColor = true;
        btnCancelar.AppearancePressed.Options.UseForeColor = true;
        btnCancelar.ImageOptions.ImageToTextIndent = 0;
        btnCancelar.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnCancelar.ImageOptions.SvgImageSize = new Size(24, 24);
        btnCancelar.Location = new Point(312, 249);
        btnCancelar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancelar.LookAndFeel.UseDefaultLookAndFeel = false;
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.BorderColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseBorderColor = true;
        btnGuardar.Appearance.Options.UseFont = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.BorderColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.ForeColor = Color.White;
        btnGuardar.AppearanceHovered.Options.UseBackColor = true;
        btnGuardar.AppearanceHovered.Options.UseBorderColor = true;
        btnGuardar.AppearanceHovered.Options.UseForeColor = true;
        btnGuardar.AppearancePressed.BackColor = Color.FromArgb(0, 137, 111);
        btnGuardar.AppearancePressed.BorderColor = Color.FromArgb(0, 137, 111);
        btnGuardar.AppearancePressed.ForeColor = Color.White;
        btnGuardar.AppearancePressed.Options.UseBackColor = true;
        btnGuardar.AppearancePressed.Options.UseBorderColor = true;
        btnGuardar.AppearancePressed.Options.UseForeColor = true;
        btnGuardar.ImageOptions.ImageToTextIndent = 0;
        btnGuardar.ImageOptions.Location = ImageLocation.MiddleLeft;
        btnGuardar.ImageOptions.SvgImageSize = new Size(24, 24);
        btnGuardar.Location = new Point(418, 249);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        // 
        // lblCode
        // 
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Location = new Point(28, 29);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(39, 15);
        lblCode.TabIndex = 2;
        lblCode.Text = "Código";
        // 
        // txtCode
        // 
        txtCode.Location = new Point(190, 26);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 50;
        txtCode.Size = new Size(330, 22);
        txtCode.TabIndex = 0;
        // 
        // lblName
        // 
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.Options.UseFont = true;
        lblName.Location = new Point(28, 57);
        lblName.Name = "lblName";
        lblName.Size = new Size(44, 15);
        lblName.TabIndex = 3;
        lblName.Text = "Nombre";
        // 
        // txtName
        // 
        txtName.Location = new Point(190, 54);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(330, 22);
        txtName.TabIndex = 1;
        // 
        // lblIdentificationType
        // 
        lblIdentificationType.Appearance.Font = new Font("Segoe UI", 9F);
        lblIdentificationType.Appearance.Options.UseFont = true;
        lblIdentificationType.Location = new Point(28, 85);
        lblIdentificationType.Name = "lblIdentificationType";
        lblIdentificationType.Size = new Size(115, 15);
        lblIdentificationType.TabIndex = 4;
        lblIdentificationType.Text = "Tipo de identificacion";
        // 
        // lueIdentificationType
        // 
        lueIdentificationType.Location = new Point(190, 82);
        lueIdentificationType.Name = "lueIdentificationType";
        lueIdentificationType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueIdentificationType.Properties.Appearance.Options.UseFont = true;
        lueIdentificationType.Properties.AutoHeight = false;
        lueIdentificationType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueIdentificationType.Properties.NullText = "";
        lueIdentificationType.Properties.ShowFooter = false;
        lueIdentificationType.Properties.ShowHeader = false;
        lueIdentificationType.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueIdentificationType.Size = new Size(330, 22);
        lueIdentificationType.TabIndex = 2;
        // 
        // lblIdentificationNumber
        // 
        lblIdentificationNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblIdentificationNumber.Appearance.Options.UseFont = true;
        lblIdentificationNumber.Location = new Point(28, 113);
        lblIdentificationNumber.Name = "lblIdentificationNumber";
        lblIdentificationNumber.Size = new Size(72, 15);
        lblIdentificationNumber.TabIndex = 5;
        lblIdentificationNumber.Text = "Identificación";
        // 
        // txtIdentificationNumber
        // 
        txtIdentificationNumber.Location = new Point(190, 110);
        txtIdentificationNumber.Name = "txtIdentificationNumber";
        txtIdentificationNumber.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtIdentificationNumber.Properties.Appearance.Options.UseFont = true;
        txtIdentificationNumber.Properties.AutoHeight = false;
        txtIdentificationNumber.Properties.MaxLength = 30;
        txtIdentificationNumber.Size = new Size(330, 22);
        txtIdentificationNumber.TabIndex = 3;
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Location = new Point(28, 141);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(62, 15);
        lblDescription.TabIndex = 6;
        lblDescription.Text = "Descripcion";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(190, 138);
        memDescription.Name = "memDescription";
        memDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescription.Properties.Appearance.Options.UseFont = true;
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(330, 70);
        memDescription.TabIndex = 4;
        // 
        // chkIsActive
        // 
        chkIsActive.EditValue = true;
        chkIsActive.Location = new Point(186, 217);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsActive.Properties.Appearance.Options.UseFont = true;
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(75, 20);
        chkIsActive.TabIndex = 5;
        // 
        // CarrierEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(558, 330);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblIdentificationType);
        Controls.Add(lueIdentificationType);
        Controls.Add(lblIdentificationNumber);
        Controls.Add(txtIdentificationNumber);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(chkIsActive);
        MinimumSize = new Size(560, 362);
        Name = "CarrierEditForm";
        Text = "Transportista";
        Controls.SetChildIndex(chkIsActive, 0);
        Controls.SetChildIndex(memDescription, 0);
        Controls.SetChildIndex(lblDescription, 0);
        Controls.SetChildIndex(txtIdentificationNumber, 0);
        Controls.SetChildIndex(lblIdentificationNumber, 0);
        Controls.SetChildIndex(lueIdentificationType, 0);
        Controls.SetChildIndex(lblIdentificationType, 0);
        Controls.SetChildIndex(txtName, 0);
        Controls.SetChildIndex(lblName, 0);
        Controls.SetChildIndex(txtCode, 0);
        Controls.SetChildIndex(lblCode, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueIdentificationType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtIdentificationNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblIdentificationType;
    private LookUpEdit lueIdentificationType;
    private LabelControl lblIdentificationNumber;
    private TextEdit txtIdentificationNumber;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private CheckEdit chkIsActive;
}

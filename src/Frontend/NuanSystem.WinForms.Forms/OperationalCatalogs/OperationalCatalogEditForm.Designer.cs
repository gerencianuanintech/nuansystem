using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.OperationalCatalogs;

partial class OperationalCatalogEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCatalogKey = new LabelControl();
        txtCatalogKey = new TextEdit();
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblParentCode = new LabelControl();
        lueParentCode = new LookUpEdit();
        lblDisplayOrder = new LabelControl();
        sedDisplayOrder = new SpinEdit();
        chkIsDefault = new CheckEdit();
        chkIsActive = new CheckEdit();
        ((System.ComponentModel.ISupportInitialize)txtCatalogKey.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueParentCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sedDisplayOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsDefault.Properties).BeginInit();
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
        btnCancelar.Location = new Point(324, 305);
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
        btnGuardar.Location = new Point(430, 305);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        // 
        // lblCatalogKey
        // 
        lblCatalogKey.Appearance.Font = new Font("Segoe UI", 9F);
        lblCatalogKey.Appearance.ForeColor = Color.Black;
        lblCatalogKey.Appearance.Options.UseFont = true;
        lblCatalogKey.Appearance.Options.UseForeColor = true;
        lblCatalogKey.Location = new Point(28, 29);
        lblCatalogKey.Name = "lblCatalogKey";
        lblCatalogKey.Size = new Size(48, 15);
        lblCatalogKey.TabIndex = 0;
        lblCatalogKey.Text = "Catalogo";
        // 
        // txtCatalogKey
        // 
        txtCatalogKey.Location = new Point(170, 26);
        txtCatalogKey.Name = "txtCatalogKey";
        txtCatalogKey.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCatalogKey.Properties.Appearance.Options.UseFont = true;
        txtCatalogKey.Properties.AutoHeight = false;
        txtCatalogKey.Properties.ReadOnly = true;
        txtCatalogKey.Size = new Size(360, 22);
        txtCatalogKey.TabIndex = 1;
        // 
        // lblCode
        // 
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.ForeColor = Color.Black;
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Appearance.Options.UseForeColor = true;
        lblCode.Location = new Point(28, 55);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(39, 15);
        lblCode.TabIndex = 2;
        lblCode.Text = "Codigo";
        // 
        // txtCode
        // 
        txtCode.Location = new Point(170, 52);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 40;
        txtCode.Size = new Size(180, 22);
        txtCode.TabIndex = 3;
        // 
        // lblName
        // 
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.ForeColor = Color.Black;
        lblName.Appearance.Options.UseFont = true;
        lblName.Appearance.Options.UseForeColor = true;
        lblName.Location = new Point(28, 81);
        lblName.Name = "lblName";
        lblName.Size = new Size(44, 15);
        lblName.TabIndex = 4;
        lblName.Text = "Nombre";
        // 
        // txtName
        // 
        txtName.Location = new Point(170, 78);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(360, 22);
        txtName.TabIndex = 5;
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.ForeColor = Color.Black;
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Appearance.Options.UseForeColor = true;
        lblDescription.Location = new Point(28, 107);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(62, 15);
        lblDescription.TabIndex = 6;
        lblDescription.Text = "Descripcion";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(170, 104);
        memDescription.Name = "memDescription";
        memDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescription.Properties.Appearance.Options.UseFont = true;
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(360, 58);
        memDescription.TabIndex = 7;
        // 
        // lblParentCode
        // 
        lblParentCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblParentCode.Appearance.ForeColor = Color.Black;
        lblParentCode.Appearance.Options.UseFont = true;
        lblParentCode.Appearance.Options.UseForeColor = true;
        lblParentCode.Location = new Point(28, 171);
        lblParentCode.Name = "lblParentCode";
        lblParentCode.Size = new Size(60, 15);
        lblParentCode.TabIndex = 8;
        lblParentCode.Text = "Valor padre";
        // 
        // lueParentCode
        // 
        lueParentCode.Location = new Point(170, 168);
        lueParentCode.Name = "lueParentCode";
        lueParentCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueParentCode.Properties.Appearance.Options.UseFont = true;
        lueParentCode.Properties.AutoHeight = false;
        lueParentCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        lueParentCode.Size = new Size(360, 22);
        lueParentCode.TabIndex = 9;
        // 
        // lblDisplayOrder
        // 
        lblDisplayOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblDisplayOrder.Appearance.ForeColor = Color.Black;
        lblDisplayOrder.Appearance.Options.UseFont = true;
        lblDisplayOrder.Appearance.Options.UseForeColor = true;
        lblDisplayOrder.Location = new Point(28, 197);
        lblDisplayOrder.Name = "lblDisplayOrder";
        lblDisplayOrder.Size = new Size(33, 15);
        lblDisplayOrder.TabIndex = 10;
        lblDisplayOrder.Text = "Orden";
        // 
        // sedDisplayOrder
        // 
        sedDisplayOrder.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        sedDisplayOrder.Location = new Point(170, 194);
        sedDisplayOrder.Name = "sedDisplayOrder";
        sedDisplayOrder.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sedDisplayOrder.Properties.Appearance.Options.UseFont = true;
        sedDisplayOrder.Properties.Appearance.Options.UseTextOptions = true;
        sedDisplayOrder.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        sedDisplayOrder.Properties.AutoHeight = false;
        sedDisplayOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        sedDisplayOrder.Properties.IsFloatValue = false;
        sedDisplayOrder.Properties.MaskSettings.Set("mask", "N0");
        sedDisplayOrder.Properties.MaxValue = new decimal(new int[] { 999999, 0, 0, 0 });
        sedDisplayOrder.Size = new Size(120, 22);
        sedDisplayOrder.TabIndex = 11;
        // 
        // chkIsDefault
        // 
        chkIsDefault.Location = new Point(166, 222);
        chkIsDefault.Name = "chkIsDefault";
        chkIsDefault.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsDefault.Properties.Appearance.Options.UseFont = true;
        chkIsDefault.Properties.Caption = "Valor por defecto";
        chkIsDefault.Size = new Size(140, 20);
        chkIsDefault.TabIndex = 12;
        // 
        // chkIsActive
        // 
        chkIsActive.EditValue = true;
        chkIsActive.Location = new Point(166, 248);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsActive.Properties.Appearance.Options.UseFont = true;
        chkIsActive.Properties.Caption = "Activo";
        chkIsActive.Size = new Size(75, 20);
        chkIsActive.TabIndex = 13;
        // 
        // OperationalCatalogEditForm
        // 
        AcceptButton = null;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = null;
        ClientSize = new Size(558, 353);
        Controls.Add(lblCatalogKey);
        Controls.Add(txtCatalogKey);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(lblParentCode);
        Controls.Add(lueParentCode);
        Controls.Add(lblDisplayOrder);
        Controls.Add(sedDisplayOrder);
        Controls.Add(chkIsDefault);
        Controls.Add(chkIsActive);
        Name = "OperationalCatalogEditForm";
        Text = "Valor de catalogo";
        Controls.SetChildIndex(chkIsActive, 0);
        Controls.SetChildIndex(chkIsDefault, 0);
        Controls.SetChildIndex(sedDisplayOrder, 0);
        Controls.SetChildIndex(lblDisplayOrder, 0);
        Controls.SetChildIndex(lueParentCode, 0);
        Controls.SetChildIndex(lblParentCode, 0);
        Controls.SetChildIndex(memDescription, 0);
        Controls.SetChildIndex(lblDescription, 0);
        Controls.SetChildIndex(txtName, 0);
        Controls.SetChildIndex(lblName, 0);
        Controls.SetChildIndex(txtCode, 0);
        Controls.SetChildIndex(lblCode, 0);
        Controls.SetChildIndex(txtCatalogKey, 0);
        Controls.SetChildIndex(lblCatalogKey, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)txtCatalogKey.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueParentCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sedDisplayOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsDefault.Properties).EndInit();
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

    private LabelControl lblCatalogKey;
    private TextEdit txtCatalogKey;
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblParentCode;
    private LookUpEdit lueParentCode;
    private LabelControl lblDisplayOrder;
    private SpinEdit sedDisplayOrder;
    private CheckEdit chkIsDefault;
    private CheckEdit chkIsActive;
}

using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.Security.Menus;

partial class MenuEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblPadre = new LabelControl();
        parentLookUpEdit = new LookUpEdit();
        lblCodigo = new LabelControl();
        codeTextEdit = new TextEdit();
        lblNombre = new LabelControl();
        nameTextEdit = new TextEdit();
        lblDescripcion = new LabelControl();
        descriptionMemoEdit = new MemoEdit();
        lblTipo = new LabelControl();
        menuTypeComboBoxEdit = new ComboBoxEdit();
        lblFormKey = new LabelControl();
        formKeyLookUpEdit = new LookUpEdit();
        lblIconLarge = new LabelControl();
        iconLargeTextEdit = new TextEdit();
        lblIconSmall = new LabelControl();
        iconSmallTextEdit = new TextEdit();
        lblDisplayOrder = new LabelControl();
        displayOrderSpinEdit = new SpinEdit();
        visibleCheckEdit = new CheckEdit();
        activeCheckEdit = new CheckEdit();
        ((System.ComponentModel.ISupportInitialize)parentLookUpEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)codeTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nameTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)descriptionMemoEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)menuTypeComboBoxEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)formKeyLookUpEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)iconLargeTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)iconSmallTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)displayOrderSpinEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)visibleCheckEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)activeCheckEdit.Properties).BeginInit();
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
        btnCancelar.Location = new Point(294, 279);
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
        btnGuardar.Location = new Point(400, 279);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        // 
        // lblPadre
        // 
        lblPadre.Appearance.Font = new Font("Segoe UI", 9F);
        lblPadre.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblPadre.Appearance.Options.UseFont = true;
        lblPadre.Appearance.Options.UseForeColor = true;
        lblPadre.Location = new Point(29, 26);
        lblPadre.Name = "lblPadre";
        lblPadre.Size = new Size(30, 15);
        lblPadre.TabIndex = 2;
        lblPadre.Text = "Padre";
        // 
        // parentLookUpEdit
        // 
        parentLookUpEdit.Location = new Point(140, 24);
        parentLookUpEdit.Name = "parentLookUpEdit";
        parentLookUpEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
        parentLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        parentLookUpEdit.Size = new Size(360, 20);
        parentLookUpEdit.TabIndex = 3;
        // 
        // lblCodigo
        // 
        lblCodigo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCodigo.Appearance.Options.UseFont = true;
        lblCodigo.Appearance.Options.UseForeColor = true;
        lblCodigo.Location = new Point(29, 52);
        lblCodigo.Name = "lblCodigo";
        lblCodigo.Size = new Size(39, 15);
        lblCodigo.TabIndex = 4;
        lblCodigo.Text = "Codigo";
        // 
        // codeTextEdit
        // 
        codeTextEdit.Location = new Point(140, 50);
        codeTextEdit.Name = "codeTextEdit";
        codeTextEdit.Size = new Size(360, 20);
        codeTextEdit.TabIndex = 5;
        // 
        // lblNombre
        // 
        lblNombre.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombre.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblNombre.Appearance.Options.UseFont = true;
        lblNombre.Appearance.Options.UseForeColor = true;
        lblNombre.Location = new Point(29, 78);
        lblNombre.Name = "lblNombre";
        lblNombre.Size = new Size(44, 15);
        lblNombre.TabIndex = 6;
        lblNombre.Text = "Nombre";
        // 
        // nameTextEdit
        // 
        nameTextEdit.Location = new Point(140, 76);
        nameTextEdit.Name = "nameTextEdit";
        nameTextEdit.Size = new Size(360, 20);
        nameTextEdit.TabIndex = 7;
        // 
        // lblDescripcion
        // 
        lblDescripcion.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescripcion.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDescripcion.Appearance.Options.UseFont = true;
        lblDescripcion.Appearance.Options.UseForeColor = true;
        lblDescripcion.Location = new Point(29, 103);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(62, 15);
        lblDescripcion.TabIndex = 8;
        lblDescripcion.Text = "Descripcion";
        // 
        // descriptionMemoEdit
        // 
        descriptionMemoEdit.Location = new Point(140, 102);
        descriptionMemoEdit.Name = "descriptionMemoEdit";
        descriptionMemoEdit.Size = new Size(360, 54);
        descriptionMemoEdit.TabIndex = 9;
        // 
        // lblTipo
        // 
        lblTipo.Appearance.Font = new Font("Segoe UI", 9F);
        lblTipo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTipo.Appearance.Options.UseFont = true;
        lblTipo.Appearance.Options.UseForeColor = true;
        lblTipo.Location = new Point(29, 164);
        lblTipo.Name = "lblTipo";
        lblTipo.Size = new Size(24, 15);
        lblTipo.TabIndex = 10;
        lblTipo.Text = "Tipo";
        // 
        // menuTypeComboBoxEdit
        // 
        menuTypeComboBoxEdit.Location = new Point(140, 162);
        menuTypeComboBoxEdit.Name = "menuTypeComboBoxEdit";
        menuTypeComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        menuTypeComboBoxEdit.Properties.Items.AddRange(new object[] { "Grupo", "Submenu", "Formulario" });
        menuTypeComboBoxEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        menuTypeComboBoxEdit.Size = new Size(160, 20);
        menuTypeComboBoxEdit.TabIndex = 11;
        // 
        // lblFormKey
        // 
        lblFormKey.Appearance.Font = new Font("Segoe UI", 9F);
        lblFormKey.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblFormKey.Appearance.Options.UseFont = true;
        lblFormKey.Appearance.Options.UseForeColor = true;
        lblFormKey.Location = new Point(311, 164);
        lblFormKey.Name = "lblFormKey";
        lblFormKey.Size = new Size(47, 15);
        lblFormKey.TabIndex = 12;
        lblFormKey.Text = "FormKey";
        // 
        // formKeyLookUpEdit
        // 
        formKeyLookUpEdit.Location = new Point(370, 162);
        formKeyLookUpEdit.Name = "formKeyLookUpEdit";
        formKeyLookUpEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
        formKeyLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        formKeyLookUpEdit.Size = new Size(130, 20);
        formKeyLookUpEdit.TabIndex = 13;
        // 
        // lblIconLarge
        // 
        lblIconLarge.Appearance.Font = new Font("Segoe UI", 9F);
        lblIconLarge.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblIconLarge.Appearance.Options.UseFont = true;
        lblIconLarge.Appearance.Options.UseForeColor = true;
        lblIconLarge.Location = new Point(29, 190);
        lblIconLarge.Name = "lblIconLarge";
        lblIconLarge.Size = new Size(80, 15);
        lblIconLarge.TabIndex = 14;
        lblIconLarge.Text = "Imagen grande";
        // 
        // iconLargeTextEdit
        // 
        iconLargeTextEdit.Location = new Point(140, 188);
        iconLargeTextEdit.Name = "iconLargeTextEdit";
        iconLargeTextEdit.Size = new Size(360, 20);
        iconLargeTextEdit.TabIndex = 15;
        // 
        // lblIconSmall
        // 
        lblIconSmall.Appearance.Font = new Font("Segoe UI", 9F);
        lblIconSmall.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblIconSmall.Appearance.Options.UseFont = true;
        lblIconSmall.Appearance.Options.UseForeColor = true;
        lblIconSmall.Location = new Point(29, 216);
        lblIconSmall.Name = "lblIconSmall";
        lblIconSmall.Size = new Size(89, 15);
        lblIconSmall.TabIndex = 16;
        lblIconSmall.Text = "Imagen pequena";
        // 
        // iconSmallTextEdit
        // 
        iconSmallTextEdit.Location = new Point(140, 214);
        iconSmallTextEdit.Name = "iconSmallTextEdit";
        iconSmallTextEdit.Size = new Size(360, 20);
        iconSmallTextEdit.TabIndex = 17;
        // 
        // lblDisplayOrder
        // 
        lblDisplayOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblDisplayOrder.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDisplayOrder.Appearance.Options.UseFont = true;
        lblDisplayOrder.Appearance.Options.UseForeColor = true;
        lblDisplayOrder.Location = new Point(29, 242);
        lblDisplayOrder.Name = "lblDisplayOrder";
        lblDisplayOrder.Size = new Size(33, 15);
        lblDisplayOrder.TabIndex = 18;
        lblDisplayOrder.Text = "Orden";
        // 
        // displayOrderSpinEdit
        // 
        displayOrderSpinEdit.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        displayOrderSpinEdit.Location = new Point(140, 240);
        displayOrderSpinEdit.Name = "displayOrderSpinEdit";
        displayOrderSpinEdit.Properties.IsFloatValue = false;
        displayOrderSpinEdit.Properties.MaskSettings.Set("mask", "N00");
        displayOrderSpinEdit.Properties.MaxValue = new decimal(new int[] { 100000, 0, 0, 0 });
        displayOrderSpinEdit.Size = new Size(100, 20);
        displayOrderSpinEdit.TabIndex = 19;
        // 
        // visibleCheckEdit
        // 
        visibleCheckEdit.Location = new Point(246, 240);
        visibleCheckEdit.Name = "visibleCheckEdit";
        visibleCheckEdit.Properties.Caption = "Visible";
        visibleCheckEdit.Size = new Size(75, 20);
        visibleCheckEdit.TabIndex = 20;
        // 
        // activeCheckEdit
        // 
        activeCheckEdit.Location = new Point(326, 240);
        activeCheckEdit.Name = "activeCheckEdit";
        activeCheckEdit.Properties.Caption = "Activo";
        activeCheckEdit.Size = new Size(75, 20);
        activeCheckEdit.TabIndex = 21;
        // 
        // MenuEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(540, 327);
        Controls.Add(lblPadre);
        Controls.Add(parentLookUpEdit);
        Controls.Add(lblCodigo);
        Controls.Add(codeTextEdit);
        Controls.Add(lblNombre);
        Controls.Add(nameTextEdit);
        Controls.Add(lblDescripcion);
        Controls.Add(descriptionMemoEdit);
        Controls.Add(lblTipo);
        Controls.Add(menuTypeComboBoxEdit);
        Controls.Add(lblFormKey);
        Controls.Add(formKeyLookUpEdit);
        Controls.Add(lblIconLarge);
        Controls.Add(iconLargeTextEdit);
        Controls.Add(lblIconSmall);
        Controls.Add(iconSmallTextEdit);
        Controls.Add(lblDisplayOrder);
        Controls.Add(displayOrderSpinEdit);
        Controls.Add(visibleCheckEdit);
        Controls.Add(activeCheckEdit);
        Name = "MenuEditForm";
        Text = "Nuevo menu";
        Controls.SetChildIndex(btnCancelar, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(activeCheckEdit, 0);
        Controls.SetChildIndex(visibleCheckEdit, 0);
        Controls.SetChildIndex(displayOrderSpinEdit, 0);
        Controls.SetChildIndex(lblDisplayOrder, 0);
        Controls.SetChildIndex(iconSmallTextEdit, 0);
        Controls.SetChildIndex(lblIconSmall, 0);
        Controls.SetChildIndex(iconLargeTextEdit, 0);
        Controls.SetChildIndex(lblIconLarge, 0);
        Controls.SetChildIndex(formKeyLookUpEdit, 0);
        Controls.SetChildIndex(lblFormKey, 0);
        Controls.SetChildIndex(menuTypeComboBoxEdit, 0);
        Controls.SetChildIndex(lblTipo, 0);
        Controls.SetChildIndex(descriptionMemoEdit, 0);
        Controls.SetChildIndex(lblDescripcion, 0);
        Controls.SetChildIndex(nameTextEdit, 0);
        Controls.SetChildIndex(lblNombre, 0);
        Controls.SetChildIndex(codeTextEdit, 0);
        Controls.SetChildIndex(lblCodigo, 0);
        Controls.SetChildIndex(parentLookUpEdit, 0);
        Controls.SetChildIndex(lblPadre, 0);
        ((System.ComponentModel.ISupportInitialize)parentLookUpEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)codeTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)nameTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)descriptionMemoEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)menuTypeComboBoxEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)formKeyLookUpEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)iconLargeTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)iconSmallTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)displayOrderSpinEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)visibleCheckEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)activeCheckEdit.Properties).EndInit();
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

    private LabelControl lblPadre;
    private LookUpEdit parentLookUpEdit;
    private LabelControl lblCodigo;
    private TextEdit codeTextEdit;
    private LabelControl lblNombre;
    private TextEdit nameTextEdit;
    private LabelControl lblDescripcion;
    private MemoEdit descriptionMemoEdit;
    private LabelControl lblTipo;
    private ComboBoxEdit menuTypeComboBoxEdit;
    private LabelControl lblFormKey;
    private LookUpEdit formKeyLookUpEdit;
    private LabelControl lblIconLarge;
    private TextEdit iconLargeTextEdit;
    private LabelControl lblIconSmall;
    private TextEdit iconSmallTextEdit;
    private LabelControl lblDisplayOrder;
    private SpinEdit displayOrderSpinEdit;
    private CheckEdit visibleCheckEdit;
    private CheckEdit activeCheckEdit;
}


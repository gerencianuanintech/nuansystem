using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.SecurityMenus;

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
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
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
        // labels and editors
        // 
        lblPadre.Location = new Point(29, 26);
        lblPadre.Text = "Padre";
        parentLookUpEdit.Location = new Point(140, 24);
        parentLookUpEdit.Size = new Size(360, 20);
        parentLookUpEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
        parentLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });

        lblCodigo.Location = new Point(29, 52);
        lblCodigo.Text = "Codigo";
        codeTextEdit.Location = new Point(140, 50);
        codeTextEdit.Size = new Size(360, 20);

        lblNombre.Location = new Point(29, 78);
        lblNombre.Text = "Nombre";
        nameTextEdit.Location = new Point(140, 76);
        nameTextEdit.Size = new Size(360, 20);

        lblDescripcion.Location = new Point(29, 103);
        lblDescripcion.Text = "Descripcion";
        descriptionMemoEdit.Location = new Point(140, 102);
        descriptionMemoEdit.Size = new Size(360, 54);

        lblTipo.Location = new Point(29, 164);
        lblTipo.Text = "Tipo";
        menuTypeComboBoxEdit.Location = new Point(140, 162);
        menuTypeComboBoxEdit.Size = new Size(160, 20);
        menuTypeComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        menuTypeComboBoxEdit.Properties.Items.AddRange(new object[] { "Grupo", "Submenu", "Formulario" });
        menuTypeComboBoxEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

        lblFormKey.Location = new Point(311, 164);
        lblFormKey.Text = "FormKey";
        formKeyLookUpEdit.Location = new Point(370, 162);
        formKeyLookUpEdit.Size = new Size(130, 20);
        formKeyLookUpEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
        formKeyLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });

        lblIconLarge.Location = new Point(29, 190);
        lblIconLarge.Text = "Imagen grande";
        iconLargeTextEdit.Location = new Point(140, 188);
        iconLargeTextEdit.Size = new Size(360, 20);

        lblIconSmall.Location = new Point(29, 216);
        lblIconSmall.Text = "Imagen pequena";
        iconSmallTextEdit.Location = new Point(140, 214);
        iconSmallTextEdit.Size = new Size(360, 20);

        lblDisplayOrder.Location = new Point(29, 242);
        lblDisplayOrder.Text = "Orden";
        displayOrderSpinEdit.Location = new Point(140, 240);
        displayOrderSpinEdit.Properties.IsFloatValue = false;
        displayOrderSpinEdit.Properties.MaskSettings.Set("mask", "N00");
        displayOrderSpinEdit.Properties.MaxValue = new decimal(new int[] { 100000, 0, 0, 0 });
        displayOrderSpinEdit.Size = new Size(100, 20);

        visibleCheckEdit.Location = new Point(246, 240);
        visibleCheckEdit.Properties.Caption = "Visible";
        visibleCheckEdit.Size = new Size(75, 20);
        activeCheckEdit.Location = new Point(326, 240);
        activeCheckEdit.Properties.Caption = "Activo";
        activeCheckEdit.Size = new Size(75, 20);

        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.ForeColor = Color.White;
        btnGuardar.AppearanceHovered.Options.UseBackColor = true;
        btnGuardar.AppearanceHovered.Options.UseForeColor = true;
        btnGuardar.ImageOptions.SvgImageSize = new Size(32, 32);        btnGuardar.Location = new Point(294, 276);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.Text = "Guardar";
        btnCancelar.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancelar.Appearance.ForeColor = Color.White;
        btnCancelar.AppearanceHovered.BackColor = Color.FromArgb(78, 87, 90);
        btnCancelar.AppearanceHovered.ForeColor = Color.White;
        btnCancelar.Appearance.Options.UseBackColor = true;
        btnCancelar.Appearance.Options.UseForeColor = true;
        btnCancelar.AppearanceHovered.Options.UseBackColor = true;
        btnCancelar.AppearanceHovered.Options.UseForeColor = true;
        btnCancelar.ImageOptions.SvgImageSize = new Size(32, 32);
        btnCancelar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancelar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(400, 276);
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.Text = "Cancelar";

        lblPadre.Appearance.Font = new Font("Segoe UI", 9F);
        lblPadre.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblPadre.Appearance.Options.UseFont = true;
        lblPadre.Appearance.Options.UseForeColor = true;
        lblCodigo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCodigo.Appearance.Options.UseFont = true;
        lblCodigo.Appearance.Options.UseForeColor = true;
        lblNombre.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombre.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblNombre.Appearance.Options.UseFont = true;
        lblNombre.Appearance.Options.UseForeColor = true;
        lblDescripcion.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescripcion.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDescripcion.Appearance.Options.UseFont = true;
        lblDescripcion.Appearance.Options.UseForeColor = true;
        lblTipo.Appearance.Font = new Font("Segoe UI", 9F);
        lblTipo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTipo.Appearance.Options.UseFont = true;
        lblTipo.Appearance.Options.UseForeColor = true;
        lblFormKey.Appearance.Font = new Font("Segoe UI", 9F);
        lblFormKey.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblFormKey.Appearance.Options.UseFont = true;
        lblFormKey.Appearance.Options.UseForeColor = true;
        lblIconLarge.Appearance.Font = new Font("Segoe UI", 9F);
        lblIconLarge.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblIconLarge.Appearance.Options.UseFont = true;
        lblIconLarge.Appearance.Options.UseForeColor = true;
        lblIconSmall.Appearance.Font = new Font("Segoe UI", 9F);
        lblIconSmall.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblIconSmall.Appearance.Options.UseFont = true;
        lblIconSmall.Appearance.Options.UseForeColor = true;
        lblDisplayOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblDisplayOrder.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDisplayOrder.Appearance.Options.UseFont = true;
        lblDisplayOrder.Appearance.Options.UseForeColor = true;

        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(540, 327);
        Controls.AddRange(new Control[] { lblPadre, parentLookUpEdit, lblCodigo, codeTextEdit, lblNombre, nameTextEdit, lblDescripcion, descriptionMemoEdit, lblTipo, menuTypeComboBoxEdit, lblFormKey, formKeyLookUpEdit, lblIconLarge, iconLargeTextEdit, lblIconSmall, iconSmallTextEdit, lblDisplayOrder, displayOrderSpinEdit, visibleCheckEdit, activeCheckEdit, btnGuardar, btnCancelar });
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "MenuEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nuevo menu";
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
    private SimpleButton btnGuardar;
    private SimpleButton btnCancelar;
}


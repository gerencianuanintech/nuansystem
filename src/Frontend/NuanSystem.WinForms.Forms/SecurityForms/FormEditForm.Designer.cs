using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.SecurityForms;

partial class FormEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblCodigo = new LabelControl();
        codeTextEdit = new TextEdit();
        lblNombre = new LabelControl();
        nameTextEdit = new TextEdit();
        lblDescripcion = new LabelControl();
        descriptionMemoEdit = new MemoEdit();
        lblFormKey = new LabelControl();
        formKeyTextEdit = new TextEdit();
        lblTipo = new LabelControl();
        formTypeComboBoxEdit = new ComboBoxEdit();
        hasListViewCheckEdit = new CheckEdit();
        hasEditViewCheckEdit = new CheckEdit();
        visibleCheckEdit = new CheckEdit();
        activeCheckEdit = new CheckEdit();
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)codeTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nameTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)descriptionMemoEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)formKeyTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)formTypeComboBoxEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)hasListViewCheckEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)hasEditViewCheckEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)visibleCheckEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)activeCheckEdit.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblCodigo
        // 
        lblCodigo.Location = new Point(29, 26);
        lblCodigo.Text = "Codigo";
        // 
        // codeTextEdit
        // 
        codeTextEdit.Location = new Point(140, 24);
        codeTextEdit.Size = new Size(360, 20);
        // 
        // lblNombre
        // 
        lblNombre.Location = new Point(29, 52);
        lblNombre.Text = "Nombre";
        // 
        // nameTextEdit
        // 
        nameTextEdit.Location = new Point(140, 50);
        nameTextEdit.Size = new Size(360, 20);
        // 
        // lblDescripcion
        // 
        lblDescripcion.Location = new Point(29, 77);
        lblDescripcion.Text = "Descripcion";
        // 
        // descriptionMemoEdit
        // 
        descriptionMemoEdit.Location = new Point(140, 76);
        descriptionMemoEdit.Size = new Size(360, 54);
        // 
        // lblFormKey
        // 
        lblFormKey.Location = new Point(29, 138);
        lblFormKey.Text = "Clave";
        // 
        // formKeyTextEdit
        // 
        formKeyTextEdit.Location = new Point(140, 136);
        formKeyTextEdit.Size = new Size(360, 20);
        // 
        // lblTipo
        // 
        lblTipo.Location = new Point(29, 164);
        lblTipo.Text = "Tipo";
        // 
        // formTypeComboBoxEdit
        // 
        formTypeComboBoxEdit.Location = new Point(140, 162);
        formTypeComboBoxEdit.Size = new Size(160, 20);
        formTypeComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        formTypeComboBoxEdit.Properties.Items.AddRange(new object[] { "Mantenimiento", "Transaccional", "Reporte", "Dialogo", "Proceso" });
        formTypeComboBoxEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        // 
        // hasListViewCheckEdit
        // 
        hasListViewCheckEdit.Location = new Point(306, 162);
        hasListViewCheckEdit.Properties.Caption = "Listado";
        hasListViewCheckEdit.Size = new Size(75, 20);
        // 
        // hasEditViewCheckEdit
        // 
        hasEditViewCheckEdit.Location = new Point(386, 162);
        hasEditViewCheckEdit.Properties.Caption = "Edicion";
        hasEditViewCheckEdit.Size = new Size(75, 20);
        // 
        // visibleCheckEdit
        // 
        visibleCheckEdit.Location = new Point(140, 188);
        visibleCheckEdit.Properties.Caption = "Visible";
        visibleCheckEdit.Size = new Size(75, 20);
        // 
        // activeCheckEdit
        // 
        activeCheckEdit.Location = new Point(220, 188);
        activeCheckEdit.Properties.Caption = "Activo";
        activeCheckEdit.Size = new Size(75, 20);
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.AppearanceHovered.BackColor = Color.FromArgb(0, 160, 128);
        btnGuardar.AppearanceHovered.ForeColor = Color.White;
        btnGuardar.AppearanceHovered.Options.UseBackColor = true;
        btnGuardar.AppearanceHovered.Options.UseForeColor = true;
        btnGuardar.ImageOptions.SvgImageSize = new Size(32, 32);
        btnGuardar.Location = new Point(294, 218);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.Text = "Guardar";
        // 
        // btnCancelar
        // 
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
        btnCancelar.Location = new Point(400, 218);
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.Text = "Cancelar";
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
        lblFormKey.Appearance.Font = new Font("Segoe UI", 9F);
        lblFormKey.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblFormKey.Appearance.Options.UseFont = true;
        lblFormKey.Appearance.Options.UseForeColor = true;
        lblTipo.Appearance.Font = new Font("Segoe UI", 9F);
        lblTipo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTipo.Appearance.Options.UseFont = true;
        lblTipo.Appearance.Options.UseForeColor = true;

        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(540, 274);
        Controls.AddRange(new Control[] { lblCodigo, codeTextEdit, lblNombre, nameTextEdit, lblDescripcion, descriptionMemoEdit, lblFormKey, formKeyTextEdit, lblTipo, formTypeComboBoxEdit, hasListViewCheckEdit, hasEditViewCheckEdit, visibleCheckEdit, activeCheckEdit, btnGuardar, btnCancelar });
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "FormEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nuevo formulario";
        ((System.ComponentModel.ISupportInitialize)codeTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)nameTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)descriptionMemoEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)formKeyTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)formTypeComboBoxEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)hasListViewCheckEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)hasEditViewCheckEdit.Properties).EndInit();
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

    private LabelControl lblCodigo;
    private TextEdit codeTextEdit;
    private LabelControl lblNombre;
    private TextEdit nameTextEdit;
    private LabelControl lblDescripcion;
    private MemoEdit descriptionMemoEdit;
    private LabelControl lblFormKey;
    private TextEdit formKeyTextEdit;
    private LabelControl lblTipo;
    private ComboBoxEdit formTypeComboBoxEdit;
    private CheckEdit hasListViewCheckEdit;
    private CheckEdit hasEditViewCheckEdit;
    private CheckEdit visibleCheckEdit;
    private CheckEdit activeCheckEdit;
    private SimpleButton btnGuardar;
    private SimpleButton btnCancelar;
}


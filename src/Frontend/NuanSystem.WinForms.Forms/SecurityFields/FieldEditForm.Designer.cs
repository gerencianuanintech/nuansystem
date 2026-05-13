using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.SecurityFields;

partial class FieldEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblFormulario = new LabelControl();
        formLookUpEdit = new LookUpEdit();
        lblCodigo = new LabelControl();
        codeTextEdit = new TextEdit();
        lblNombre = new LabelControl();
        nameTextEdit = new TextEdit();
        lblCampo = new LabelControl();
        fieldKeyTextEdit = new TextEdit();
        lblDescripcion = new LabelControl();
        descriptionMemoEdit = new MemoEdit();
        lblTipoControl = new LabelControl();
        controlTypeComboBoxEdit = new ComboBoxEdit();
        lblTipoDato = new LabelControl();
        dataTypeComboBoxEdit = new ComboBoxEdit();
        lblMensaje = new LabelControl();
        validationMessageTextEdit = new TextEdit();
        lblOrden = new LabelControl();
        displayOrderSpinEdit = new SpinEdit();
        requiredCheckEdit = new CheckEdit();
        readOnlyCheckEdit = new CheckEdit();
        visibleCheckEdit = new CheckEdit();
        customCheckEdit = new CheckEdit();
        activeCheckEdit = new CheckEdit();
        btnCancelar = new SimpleButton();
        btnGuardar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)formLookUpEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)codeTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nameTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)fieldKeyTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)descriptionMemoEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)controlTypeComboBoxEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dataTypeComboBoxEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)validationMessageTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)displayOrderSpinEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)requiredCheckEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)readOnlyCheckEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)visibleCheckEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)customCheckEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)activeCheckEdit.Properties).BeginInit();
        SuspendLayout();
        // 
        // lblFormulario
        // 
        lblFormulario.Location = new Point(28, 26);
        lblFormulario.Text = "Formulario";
        // 
        // formLookUpEdit
        // 
        formLookUpEdit.Location = new Point(144, 24);
        formLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        formLookUpEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        formLookUpEdit.Size = new Size(420, 20);
        // 
        // lblCodigo
        // 
        lblCodigo.Location = new Point(28, 52);
        lblCodigo.Text = "Codigo";
        // 
        // codeTextEdit
        // 
        codeTextEdit.Location = new Point(144, 50);
        codeTextEdit.Size = new Size(420, 20);
        // 
        // lblNombre
        // 
        lblNombre.Location = new Point(28, 78);
        lblNombre.Text = "Nombre";
        // 
        // nameTextEdit
        // 
        nameTextEdit.Location = new Point(144, 76);
        nameTextEdit.Size = new Size(420, 20);
        // 
        // lblCampo
        // 
        lblCampo.Location = new Point(28, 104);
        lblCampo.Text = "Campo";
        // 
        // fieldKeyTextEdit
        // 
        fieldKeyTextEdit.Location = new Point(144, 102);
        fieldKeyTextEdit.Size = new Size(420, 20);
        // 
        // lblDescripcion
        // 
        lblDescripcion.Location = new Point(28, 130);
        lblDescripcion.Text = "Descripcion";
        // 
        // descriptionMemoEdit
        // 
        descriptionMemoEdit.Location = new Point(144, 128);
        descriptionMemoEdit.Size = new Size(420, 54);
        // 
        // lblTipoControl
        // 
        lblTipoControl.Location = new Point(28, 190);
        lblTipoControl.Text = "Tipo control";
        // 
        // controlTypeComboBoxEdit
        // 
        controlTypeComboBoxEdit.Location = new Point(144, 188);
        controlTypeComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        controlTypeComboBoxEdit.Properties.Items.AddRange(new object[] { "TextEdit", "ButtonEdit", "DateEdit", "LookUpEdit", "GridLookUpEdit", "CheckEdit", "MemoEdit", "SpinEdit", "PictureEdit", "GridColumn", "Personalizado", "Other" });
        controlTypeComboBoxEdit.Size = new Size(180, 20);
        // 
        // lblTipoDato
        // 
        lblTipoDato.Location = new Point(342, 190);
        lblTipoDato.Text = "Tipo dato";
        // 
        // dataTypeComboBoxEdit
        // 
        dataTypeComboBoxEdit.Location = new Point(408, 188);
        dataTypeComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        dataTypeComboBoxEdit.Properties.Items.AddRange(new object[] { "string", "int", "decimal", "date", "datetime", "bool", "image", "guid" });
        dataTypeComboBoxEdit.Size = new Size(156, 20);
        // 
        // lblMensaje
        // 
        lblMensaje.Location = new Point(28, 216);
        lblMensaje.Text = "Mensaje";
        // 
        // validationMessageTextEdit
        // 
        validationMessageTextEdit.Location = new Point(144, 214);
        validationMessageTextEdit.Size = new Size(420, 20);
        // 
        // lblOrden
        // 
        lblOrden.Location = new Point(28, 242);
        lblOrden.Text = "Orden";
        // 
        // displayOrderSpinEdit
        // 
        displayOrderSpinEdit.Location = new Point(144, 240);
        displayOrderSpinEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        displayOrderSpinEdit.Properties.IsFloatValue = false;
        displayOrderSpinEdit.Properties.MaskSettings.Set("mask", "N00");
        displayOrderSpinEdit.Properties.MaxValue = 99999;
        displayOrderSpinEdit.Size = new Size(100, 20);
        // 
        // requiredCheckEdit
        // 
        requiredCheckEdit.Location = new Point(144, 272);
        requiredCheckEdit.Properties.Caption = "Requerido";
        requiredCheckEdit.Size = new Size(90, 20);
        // 
        // readOnlyCheckEdit
        // 
        readOnlyCheckEdit.Location = new Point(240, 272);
        readOnlyCheckEdit.Properties.Caption = "Solo lectura";
        readOnlyCheckEdit.Size = new Size(96, 20);
        // 
        // visibleCheckEdit
        // 
        visibleCheckEdit.Location = new Point(342, 272);
        visibleCheckEdit.Properties.Caption = "Visible";
        visibleCheckEdit.Size = new Size(75, 20);
        // 
        // customCheckEdit
        // 
        customCheckEdit.Location = new Point(423, 272);
        customCheckEdit.Properties.Caption = "Personalizado";
        customCheckEdit.Size = new Size(105, 20);
        // 
        // activeCheckEdit
        // 
        activeCheckEdit.Location = new Point(144, 298);
        activeCheckEdit.Properties.Caption = "Activo";
        activeCheckEdit.Size = new Size(75, 20);
        // 
        // btnCancelar
        // 
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(358, 340);
        btnCancelar.Size = new Size(100, 36);
        btnCancelar.Text = "Cancelar";
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.Location = new Point(464, 340);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Size = new Size(100, 36);
        btnGuardar.Text = "Guardar";
        lblFormulario.Appearance.Font = new Font("Segoe UI", 9F);
        lblFormulario.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblFormulario.Appearance.Options.UseFont = true;
        lblFormulario.Appearance.Options.UseForeColor = true;
        lblCodigo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCodigo.Appearance.Options.UseFont = true;
        lblCodigo.Appearance.Options.UseForeColor = true;
        lblNombre.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombre.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblNombre.Appearance.Options.UseFont = true;
        lblNombre.Appearance.Options.UseForeColor = true;
        lblCampo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCampo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCampo.Appearance.Options.UseFont = true;
        lblCampo.Appearance.Options.UseForeColor = true;
        lblDescripcion.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescripcion.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDescripcion.Appearance.Options.UseFont = true;
        lblDescripcion.Appearance.Options.UseForeColor = true;
        lblTipoControl.Appearance.Font = new Font("Segoe UI", 9F);
        lblTipoControl.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTipoControl.Appearance.Options.UseFont = true;
        lblTipoControl.Appearance.Options.UseForeColor = true;
        lblTipoDato.Appearance.Font = new Font("Segoe UI", 9F);
        lblTipoDato.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTipoDato.Appearance.Options.UseFont = true;
        lblTipoDato.Appearance.Options.UseForeColor = true;
        lblMensaje.Appearance.Font = new Font("Segoe UI", 9F);
        lblMensaje.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblMensaje.Appearance.Options.UseFont = true;
        lblMensaje.Appearance.Options.UseForeColor = true;
        lblOrden.Appearance.Font = new Font("Segoe UI", 9F);
        lblOrden.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblOrden.Appearance.Options.UseFont = true;
        lblOrden.Appearance.Options.UseForeColor = true;

        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(600, 400);
        Controls.AddRange(new Control[] { lblFormulario, formLookUpEdit, lblCodigo, codeTextEdit, lblNombre, nameTextEdit, lblCampo, fieldKeyTextEdit, lblDescripcion, descriptionMemoEdit, lblTipoControl, controlTypeComboBoxEdit, lblTipoDato, dataTypeComboBoxEdit, lblMensaje, validationMessageTextEdit, lblOrden, displayOrderSpinEdit, requiredCheckEdit, readOnlyCheckEdit, visibleCheckEdit, customCheckEdit, activeCheckEdit, btnCancelar, btnGuardar });
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "FieldEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nuevo campo";
        ((System.ComponentModel.ISupportInitialize)formLookUpEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)codeTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)nameTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)fieldKeyTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)descriptionMemoEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)controlTypeComboBoxEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dataTypeComboBoxEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)validationMessageTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)displayOrderSpinEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)requiredCheckEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)readOnlyCheckEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)visibleCheckEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)customCheckEdit.Properties).EndInit();
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

    private LabelControl lblFormulario;
    private LookUpEdit formLookUpEdit;
    private LabelControl lblCodigo;
    private TextEdit codeTextEdit;
    private LabelControl lblNombre;
    private TextEdit nameTextEdit;
    private LabelControl lblCampo;
    private TextEdit fieldKeyTextEdit;
    private LabelControl lblDescripcion;
    private MemoEdit descriptionMemoEdit;
    private LabelControl lblTipoControl;
    private ComboBoxEdit controlTypeComboBoxEdit;
    private LabelControl lblTipoDato;
    private ComboBoxEdit dataTypeComboBoxEdit;
    private LabelControl lblMensaje;
    private TextEdit validationMessageTextEdit;
    private LabelControl lblOrden;
    private SpinEdit displayOrderSpinEdit;
    private CheckEdit requiredCheckEdit;
    private CheckEdit readOnlyCheckEdit;
    private CheckEdit visibleCheckEdit;
    private CheckEdit customCheckEdit;
    private CheckEdit activeCheckEdit;
    private SimpleButton btnCancelar;
    private SimpleButton btnGuardar;
}

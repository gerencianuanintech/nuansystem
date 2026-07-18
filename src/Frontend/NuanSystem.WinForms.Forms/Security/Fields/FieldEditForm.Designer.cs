using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.Security.Fields;

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
        btnCancelar.Location = new Point(358, 332);
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
        btnGuardar.Location = new Point(464, 332);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        //
        // lblFormulario
        // 
        lblFormulario.Appearance.Font = new Font("Segoe UI", 9F);
        lblFormulario.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblFormulario.Appearance.Options.UseFont = true;
        lblFormulario.Appearance.Options.UseForeColor = true;
        lblFormulario.Location = new Point(28, 26);
        lblFormulario.Name = "lblFormulario";
        lblFormulario.Size = new Size(58, 15);
        lblFormulario.TabIndex = 2;
        lblFormulario.Text = "Formulario";
        // 
        // formLookUpEdit
        // 
        formLookUpEdit.Location = new Point(144, 24);
        formLookUpEdit.Name = "formLookUpEdit";
        formLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        formLookUpEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        formLookUpEdit.Size = new Size(420, 20);
        formLookUpEdit.TabIndex = 3;
        // 
        // lblCodigo
        // 
        lblCodigo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCodigo.Appearance.Options.UseFont = true;
        lblCodigo.Appearance.Options.UseForeColor = true;
        lblCodigo.Location = new Point(28, 52);
        lblCodigo.Name = "lblCodigo";
        lblCodigo.Size = new Size(39, 15);
        lblCodigo.TabIndex = 4;
        lblCodigo.Text = "Codigo";
        // 
        // codeTextEdit
        // 
        codeTextEdit.Location = new Point(144, 50);
        codeTextEdit.Name = "codeTextEdit";
        codeTextEdit.Size = new Size(420, 20);
        codeTextEdit.TabIndex = 5;
        // 
        // lblNombre
        // 
        lblNombre.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombre.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblNombre.Appearance.Options.UseFont = true;
        lblNombre.Appearance.Options.UseForeColor = true;
        lblNombre.Location = new Point(28, 78);
        lblNombre.Name = "lblNombre";
        lblNombre.Size = new Size(44, 15);
        lblNombre.TabIndex = 6;
        lblNombre.Text = "Nombre";
        // 
        // nameTextEdit
        // 
        nameTextEdit.Location = new Point(144, 76);
        nameTextEdit.Name = "nameTextEdit";
        nameTextEdit.Size = new Size(420, 20);
        nameTextEdit.TabIndex = 7;
        // 
        // lblCampo
        // 
        lblCampo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCampo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCampo.Appearance.Options.UseFont = true;
        lblCampo.Appearance.Options.UseForeColor = true;
        lblCampo.Location = new Point(28, 104);
        lblCampo.Name = "lblCampo";
        lblCampo.Size = new Size(39, 15);
        lblCampo.TabIndex = 8;
        lblCampo.Text = "Campo";
        // 
        // fieldKeyTextEdit
        // 
        fieldKeyTextEdit.Location = new Point(144, 102);
        fieldKeyTextEdit.Name = "fieldKeyTextEdit";
        fieldKeyTextEdit.Size = new Size(420, 20);
        fieldKeyTextEdit.TabIndex = 9;
        // 
        // lblDescripcion
        // 
        lblDescripcion.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescripcion.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDescripcion.Appearance.Options.UseFont = true;
        lblDescripcion.Appearance.Options.UseForeColor = true;
        lblDescripcion.Location = new Point(28, 130);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(62, 15);
        lblDescripcion.TabIndex = 10;
        lblDescripcion.Text = "Descripcion";
        // 
        // descriptionMemoEdit
        // 
        descriptionMemoEdit.Location = new Point(144, 128);
        descriptionMemoEdit.Name = "descriptionMemoEdit";
        descriptionMemoEdit.Size = new Size(420, 54);
        descriptionMemoEdit.TabIndex = 11;
        // 
        // lblTipoControl
        // 
        lblTipoControl.Appearance.Font = new Font("Segoe UI", 9F);
        lblTipoControl.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTipoControl.Appearance.Options.UseFont = true;
        lblTipoControl.Appearance.Options.UseForeColor = true;
        lblTipoControl.Location = new Point(28, 190);
        lblTipoControl.Name = "lblTipoControl";
        lblTipoControl.Size = new Size(65, 15);
        lblTipoControl.TabIndex = 12;
        lblTipoControl.Text = "Tipo control";
        // 
        // controlTypeComboBoxEdit
        // 
        controlTypeComboBoxEdit.Location = new Point(144, 188);
        controlTypeComboBoxEdit.Name = "controlTypeComboBoxEdit";
        controlTypeComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        controlTypeComboBoxEdit.Properties.Items.AddRange(new object[] { "TextEdit", "ButtonEdit", "DateEdit", "LookUpEdit", "GridLookUpEdit", "CheckEdit", "MemoEdit", "SpinEdit", "PictureEdit", "GridColumn", "Personalizado", "Other" });
        controlTypeComboBoxEdit.Size = new Size(180, 20);
        controlTypeComboBoxEdit.TabIndex = 13;
        // 
        // lblTipoDato
        // 
        lblTipoDato.Appearance.Font = new Font("Segoe UI", 9F);
        lblTipoDato.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTipoDato.Appearance.Options.UseFont = true;
        lblTipoDato.Appearance.Options.UseForeColor = true;
        lblTipoDato.Location = new Point(342, 190);
        lblTipoDato.Name = "lblTipoDato";
        lblTipoDato.Size = new Size(51, 15);
        lblTipoDato.TabIndex = 14;
        lblTipoDato.Text = "Tipo dato";
        // 
        // dataTypeComboBoxEdit
        // 
        dataTypeComboBoxEdit.Location = new Point(408, 188);
        dataTypeComboBoxEdit.Name = "dataTypeComboBoxEdit";
        dataTypeComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        dataTypeComboBoxEdit.Properties.Items.AddRange(new object[] { "string", "int", "decimal", "date", "datetime", "bool", "image", "guid" });
        dataTypeComboBoxEdit.Size = new Size(156, 20);
        dataTypeComboBoxEdit.TabIndex = 15;
        // 
        // lblMensaje
        // 
        lblMensaje.Appearance.Font = new Font("Segoe UI", 9F);
        lblMensaje.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblMensaje.Appearance.Options.UseFont = true;
        lblMensaje.Appearance.Options.UseForeColor = true;
        lblMensaje.Location = new Point(28, 216);
        lblMensaje.Name = "lblMensaje";
        lblMensaje.Size = new Size(44, 15);
        lblMensaje.TabIndex = 16;
        lblMensaje.Text = "Mensaje";
        // 
        // validationMessageTextEdit
        // 
        validationMessageTextEdit.Location = new Point(144, 214);
        validationMessageTextEdit.Name = "validationMessageTextEdit";
        validationMessageTextEdit.Size = new Size(420, 20);
        validationMessageTextEdit.TabIndex = 17;
        // 
        // lblOrden
        // 
        lblOrden.Appearance.Font = new Font("Segoe UI", 9F);
        lblOrden.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblOrden.Appearance.Options.UseFont = true;
        lblOrden.Appearance.Options.UseForeColor = true;
        lblOrden.Location = new Point(28, 242);
        lblOrden.Name = "lblOrden";
        lblOrden.Size = new Size(33, 15);
        lblOrden.TabIndex = 18;
        lblOrden.Text = "Orden";
        // 
        // displayOrderSpinEdit
        // 
        displayOrderSpinEdit.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        displayOrderSpinEdit.Location = new Point(144, 240);
        displayOrderSpinEdit.Name = "displayOrderSpinEdit";
        displayOrderSpinEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        displayOrderSpinEdit.Properties.IsFloatValue = false;
        displayOrderSpinEdit.Properties.MaskSettings.Set("mask", "N00");
        displayOrderSpinEdit.Properties.MaxValue = new decimal(new int[] { 99999, 0, 0, 0 });
        displayOrderSpinEdit.Size = new Size(100, 20);
        displayOrderSpinEdit.TabIndex = 19;
        // 
        // requiredCheckEdit
        // 
        requiredCheckEdit.Location = new Point(144, 272);
        requiredCheckEdit.Name = "requiredCheckEdit";
        requiredCheckEdit.Properties.Caption = "Requerido";
        requiredCheckEdit.Size = new Size(90, 20);
        requiredCheckEdit.TabIndex = 20;
        // 
        // readOnlyCheckEdit
        // 
        readOnlyCheckEdit.Location = new Point(240, 272);
        readOnlyCheckEdit.Name = "readOnlyCheckEdit";
        readOnlyCheckEdit.Properties.Caption = "Solo lectura";
        readOnlyCheckEdit.Size = new Size(96, 20);
        readOnlyCheckEdit.TabIndex = 21;
        // 
        // visibleCheckEdit
        // 
        visibleCheckEdit.Location = new Point(342, 272);
        visibleCheckEdit.Name = "visibleCheckEdit";
        visibleCheckEdit.Properties.Caption = "Visible";
        visibleCheckEdit.Size = new Size(75, 20);
        visibleCheckEdit.TabIndex = 22;
        // 
        // customCheckEdit
        // 
        customCheckEdit.Location = new Point(423, 272);
        customCheckEdit.Name = "customCheckEdit";
        customCheckEdit.Properties.Caption = "Personalizado";
        customCheckEdit.Size = new Size(105, 20);
        customCheckEdit.TabIndex = 23;
        // 
        // activeCheckEdit
        // 
        activeCheckEdit.Location = new Point(144, 298);
        activeCheckEdit.Name = "activeCheckEdit";
        activeCheckEdit.Properties.Caption = "Activo";
        activeCheckEdit.Size = new Size(75, 20);
        activeCheckEdit.TabIndex = 24;
        // 
        // FieldEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(600, 400);
        Controls.Add(lblFormulario);
        Controls.Add(formLookUpEdit);
        Controls.Add(lblCodigo);
        Controls.Add(codeTextEdit);
        Controls.Add(lblNombre);
        Controls.Add(nameTextEdit);
        Controls.Add(lblCampo);
        Controls.Add(fieldKeyTextEdit);
        Controls.Add(lblDescripcion);
        Controls.Add(descriptionMemoEdit);
        Controls.Add(lblTipoControl);
        Controls.Add(controlTypeComboBoxEdit);
        Controls.Add(lblTipoDato);
        Controls.Add(dataTypeComboBoxEdit);
        Controls.Add(lblMensaje);
        Controls.Add(validationMessageTextEdit);
        Controls.Add(lblOrden);
        Controls.Add(displayOrderSpinEdit);
        Controls.Add(requiredCheckEdit);
        Controls.Add(readOnlyCheckEdit);
        Controls.Add(visibleCheckEdit);
        Controls.Add(customCheckEdit);
        Controls.Add(activeCheckEdit);
        Name = "FieldEditForm";
        Text = "Nuevo campo";
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        Controls.SetChildIndex(activeCheckEdit, 0);
        Controls.SetChildIndex(customCheckEdit, 0);
        Controls.SetChildIndex(visibleCheckEdit, 0);
        Controls.SetChildIndex(readOnlyCheckEdit, 0);
        Controls.SetChildIndex(requiredCheckEdit, 0);
        Controls.SetChildIndex(displayOrderSpinEdit, 0);
        Controls.SetChildIndex(lblOrden, 0);
        Controls.SetChildIndex(validationMessageTextEdit, 0);
        Controls.SetChildIndex(lblMensaje, 0);
        Controls.SetChildIndex(dataTypeComboBoxEdit, 0);
        Controls.SetChildIndex(lblTipoDato, 0);
        Controls.SetChildIndex(controlTypeComboBoxEdit, 0);
        Controls.SetChildIndex(lblTipoControl, 0);
        Controls.SetChildIndex(descriptionMemoEdit, 0);
        Controls.SetChildIndex(lblDescripcion, 0);
        Controls.SetChildIndex(fieldKeyTextEdit, 0);
        Controls.SetChildIndex(lblCampo, 0);
        Controls.SetChildIndex(nameTextEdit, 0);
        Controls.SetChildIndex(lblNombre, 0);
        Controls.SetChildIndex(codeTextEdit, 0);
        Controls.SetChildIndex(lblCodigo, 0);
        Controls.SetChildIndex(formLookUpEdit, 0);
        Controls.SetChildIndex(lblFormulario, 0);
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
}


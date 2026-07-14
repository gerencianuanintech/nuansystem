using DevExpress.XtraEditors;

namespace NuanSystem.WinForms.Forms.Security.Forms;

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
        btnCancelar.Location = new Point(294, 226);
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
        btnGuardar.Location = new Point(400, 226);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        // 
        // lblCodigo
        // 
        lblCodigo.Appearance.Font = new Font("Segoe UI", 9F);
        lblCodigo.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCodigo.Appearance.Options.UseFont = true;
        lblCodigo.Appearance.Options.UseForeColor = true;
        lblCodigo.Location = new Point(29, 26);
        lblCodigo.Name = "lblCodigo";
        lblCodigo.Size = new Size(39, 15);
        lblCodigo.TabIndex = 2;
        lblCodigo.Text = "Codigo";
        // 
        // codeTextEdit
        // 
        codeTextEdit.Location = new Point(140, 24);
        codeTextEdit.Name = "codeTextEdit";
        codeTextEdit.Size = new Size(360, 20);
        codeTextEdit.TabIndex = 3;
        // 
        // lblNombre
        // 
        lblNombre.Appearance.Font = new Font("Segoe UI", 9F);
        lblNombre.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblNombre.Appearance.Options.UseFont = true;
        lblNombre.Appearance.Options.UseForeColor = true;
        lblNombre.Location = new Point(29, 52);
        lblNombre.Name = "lblNombre";
        lblNombre.Size = new Size(44, 15);
        lblNombre.TabIndex = 4;
        lblNombre.Text = "Nombre";
        // 
        // nameTextEdit
        // 
        nameTextEdit.Location = new Point(140, 50);
        nameTextEdit.Name = "nameTextEdit";
        nameTextEdit.Size = new Size(360, 20);
        nameTextEdit.TabIndex = 5;
        // 
        // lblDescripcion
        // 
        lblDescripcion.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescripcion.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblDescripcion.Appearance.Options.UseFont = true;
        lblDescripcion.Appearance.Options.UseForeColor = true;
        lblDescripcion.Location = new Point(29, 77);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(62, 15);
        lblDescripcion.TabIndex = 6;
        lblDescripcion.Text = "Descripcion";
        // 
        // descriptionMemoEdit
        // 
        descriptionMemoEdit.Location = new Point(140, 76);
        descriptionMemoEdit.Name = "descriptionMemoEdit";
        descriptionMemoEdit.Size = new Size(360, 54);
        descriptionMemoEdit.TabIndex = 7;
        // 
        // lblFormKey
        // 
        lblFormKey.Appearance.Font = new Font("Segoe UI", 9F);
        lblFormKey.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblFormKey.Appearance.Options.UseFont = true;
        lblFormKey.Appearance.Options.UseForeColor = true;
        lblFormKey.Location = new Point(29, 138);
        lblFormKey.Name = "lblFormKey";
        lblFormKey.Size = new Size(29, 15);
        lblFormKey.TabIndex = 8;
        lblFormKey.Text = "Clave";
        // 
        // formKeyTextEdit
        // 
        formKeyTextEdit.Location = new Point(140, 136);
        formKeyTextEdit.Name = "formKeyTextEdit";
        formKeyTextEdit.Size = new Size(360, 20);
        formKeyTextEdit.TabIndex = 9;
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
        // formTypeComboBoxEdit
        // 
        formTypeComboBoxEdit.Location = new Point(140, 162);
        formTypeComboBoxEdit.Name = "formTypeComboBoxEdit";
        formTypeComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] { new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo) });
        formTypeComboBoxEdit.Properties.Items.AddRange(new object[] { "Mantenimiento", "Transaccional", "Reporte", "Dialogo", "Proceso" });
        formTypeComboBoxEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
        formTypeComboBoxEdit.Size = new Size(160, 20);
        formTypeComboBoxEdit.TabIndex = 11;
        // 
        // hasListViewCheckEdit
        // 
        hasListViewCheckEdit.Location = new Point(306, 162);
        hasListViewCheckEdit.Name = "hasListViewCheckEdit";
        hasListViewCheckEdit.Properties.Caption = "Listado";
        hasListViewCheckEdit.Size = new Size(75, 20);
        hasListViewCheckEdit.TabIndex = 12;
        // 
        // hasEditViewCheckEdit
        // 
        hasEditViewCheckEdit.Location = new Point(386, 162);
        hasEditViewCheckEdit.Name = "hasEditViewCheckEdit";
        hasEditViewCheckEdit.Properties.Caption = "Edicion";
        hasEditViewCheckEdit.Size = new Size(75, 20);
        hasEditViewCheckEdit.TabIndex = 13;
        // 
        // visibleCheckEdit
        // 
        visibleCheckEdit.Location = new Point(140, 188);
        visibleCheckEdit.Name = "visibleCheckEdit";
        visibleCheckEdit.Properties.Caption = "Visible";
        visibleCheckEdit.Size = new Size(75, 20);
        visibleCheckEdit.TabIndex = 14;
        // 
        // activeCheckEdit
        // 
        activeCheckEdit.Location = new Point(220, 188);
        activeCheckEdit.Name = "activeCheckEdit";
        activeCheckEdit.Properties.Caption = "Activo";
        activeCheckEdit.Size = new Size(75, 20);
        activeCheckEdit.TabIndex = 15;
        // 
        // FormEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(540, 274);
        Controls.Add(lblCodigo);
        Controls.Add(codeTextEdit);
        Controls.Add(lblNombre);
        Controls.Add(nameTextEdit);
        Controls.Add(lblDescripcion);
        Controls.Add(descriptionMemoEdit);
        Controls.Add(lblFormKey);
        Controls.Add(formKeyTextEdit);
        Controls.Add(lblTipo);
        Controls.Add(formTypeComboBoxEdit);
        Controls.Add(hasListViewCheckEdit);
        Controls.Add(hasEditViewCheckEdit);
        Controls.Add(visibleCheckEdit);
        Controls.Add(activeCheckEdit);
        Name = "FormEditForm";
        Text = "Nuevo formulario";
        Controls.SetChildIndex(btnCancelar, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(activeCheckEdit, 0);
        Controls.SetChildIndex(visibleCheckEdit, 0);
        Controls.SetChildIndex(hasEditViewCheckEdit, 0);
        Controls.SetChildIndex(hasListViewCheckEdit, 0);
        Controls.SetChildIndex(formTypeComboBoxEdit, 0);
        Controls.SetChildIndex(lblTipo, 0);
        Controls.SetChildIndex(formKeyTextEdit, 0);
        Controls.SetChildIndex(lblFormKey, 0);
        Controls.SetChildIndex(descriptionMemoEdit, 0);
        Controls.SetChildIndex(lblDescripcion, 0);
        Controls.SetChildIndex(nameTextEdit, 0);
        Controls.SetChildIndex(lblNombre, 0);
        Controls.SetChildIndex(codeTextEdit, 0);
        Controls.SetChildIndex(lblCodigo, 0);
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
}


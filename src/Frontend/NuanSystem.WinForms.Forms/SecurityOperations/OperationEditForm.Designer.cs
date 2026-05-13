using DevExpress.XtraEditors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.SecurityOperations;

partial class OperationEditForm
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
        lblRibbonPage = new LabelControl();
        ribbonPageTextEdit = new TextEdit();
        lblRibbonGroup = new LabelControl();
        ribbonGroupTextEdit = new TextEdit();
        lblActionKey = new LabelControl();
        actionKeyTextEdit = new TextEdit();
        lblIconLarge = new LabelControl();
        iconLargeTextEdit = new TextEdit();
        lblIconSmall = new LabelControl();
        iconSmallTextEdit = new TextEdit();
        lblDisplayOrder = new LabelControl();
        displayOrderSpinEdit = new SpinEdit();
        activeCheckEdit = new CheckEdit();
        btnGuardar = new SimpleButton();
        btnCancelar = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)codeTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)nameTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)descriptionMemoEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)ribbonPageTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)ribbonGroupTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)actionKeyTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)iconLargeTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)iconSmallTextEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)displayOrderSpinEdit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)activeCheckEdit.Properties).BeginInit();
        SuspendLayout();
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
        lblCodigo.TabIndex = 0;
        lblCodigo.Text = "Codigo";
        // 
        // codeTextEdit
        // 
        codeTextEdit.Location = new Point(140, 24);
        codeTextEdit.Name = "codeTextEdit";
        codeTextEdit.Size = new Size(360, 20);
        codeTextEdit.TabIndex = 1;
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
        lblNombre.TabIndex = 2;
        lblNombre.Text = "Nombre";
        // 
        // nameTextEdit
        // 
        nameTextEdit.Location = new Point(140, 50);
        nameTextEdit.Name = "nameTextEdit";
        nameTextEdit.Size = new Size(360, 20);
        nameTextEdit.TabIndex = 3;
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
        lblDescripcion.TabIndex = 4;
        lblDescripcion.Text = "Descripcion";
        // 
        // descriptionMemoEdit
        // 
        descriptionMemoEdit.Location = new Point(140, 76);
        descriptionMemoEdit.Name = "descriptionMemoEdit";
        descriptionMemoEdit.Size = new Size(360, 54);
        descriptionMemoEdit.TabIndex = 5;
        // 
        // lblRibbonPage
        // 
        lblRibbonPage.Appearance.Font = new Font("Segoe UI", 9F);
        lblRibbonPage.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblRibbonPage.Appearance.Options.UseFont = true;
        lblRibbonPage.Appearance.Options.UseForeColor = true;
        lblRibbonPage.Location = new Point(29, 138);
        lblRibbonPage.Name = "lblRibbonPage";
        lblRibbonPage.Size = new Size(31, 15);
        lblRibbonPage.TabIndex = 6;
        lblRibbonPage.Text = "Menu";
        // 
        // ribbonPageTextEdit
        // 
        ribbonPageTextEdit.Location = new Point(140, 136);
        ribbonPageTextEdit.Name = "ribbonPageTextEdit";
        ribbonPageTextEdit.Size = new Size(160, 20);
        ribbonPageTextEdit.TabIndex = 7;
        // 
        // lblRibbonGroup
        // 
        lblRibbonGroup.Appearance.Font = new Font("Segoe UI", 9F);
        lblRibbonGroup.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblRibbonGroup.Appearance.Options.UseFont = true;
        lblRibbonGroup.Appearance.Options.UseForeColor = true;
        lblRibbonGroup.Location = new Point(311, 138);
        lblRibbonGroup.Name = "lblRibbonGroup";
        lblRibbonGroup.Size = new Size(53, 15);
        lblRibbonGroup.TabIndex = 8;
        lblRibbonGroup.Text = "Agrupado";
        // 
        // ribbonGroupTextEdit
        // 
        ribbonGroupTextEdit.Location = new Point(370, 136);
        ribbonGroupTextEdit.Name = "ribbonGroupTextEdit";
        ribbonGroupTextEdit.Size = new Size(130, 20);
        ribbonGroupTextEdit.TabIndex = 9;
        // 
        // lblActionKey
        // 
        lblActionKey.Appearance.Font = new Font("Segoe UI", 9F);
        lblActionKey.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblActionKey.Appearance.Options.UseFont = true;
        lblActionKey.Appearance.Options.UseForeColor = true;
        lblActionKey.Location = new Point(29, 164);
        lblActionKey.Name = "lblActionKey";
        lblActionKey.Size = new Size(37, 15);
        lblActionKey.TabIndex = 10;
        lblActionKey.Text = "Accion";
        // 
        // actionKeyTextEdit
        // 
        actionKeyTextEdit.Location = new Point(140, 162);
        actionKeyTextEdit.Name = "actionKeyTextEdit";
        actionKeyTextEdit.Size = new Size(360, 20);
        actionKeyTextEdit.TabIndex = 11;
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
        lblIconLarge.TabIndex = 12;
        lblIconLarge.Text = "Imagen grande";
        // 
        // iconLargeTextEdit
        // 
        iconLargeTextEdit.Location = new Point(140, 188);
        iconLargeTextEdit.Name = "iconLargeTextEdit";
        iconLargeTextEdit.Size = new Size(360, 20);
        iconLargeTextEdit.TabIndex = 13;
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
        lblIconSmall.TabIndex = 14;
        lblIconSmall.Text = "Imagen pequena";
        // 
        // iconSmallTextEdit
        // 
        iconSmallTextEdit.Location = new Point(140, 214);
        iconSmallTextEdit.Name = "iconSmallTextEdit";
        iconSmallTextEdit.Size = new Size(360, 20);
        iconSmallTextEdit.TabIndex = 15;
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
        lblDisplayOrder.TabIndex = 16;
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
        displayOrderSpinEdit.TabIndex = 17;
        // 
        // activeCheckEdit
        // 
        activeCheckEdit.Location = new Point(246, 240);
        activeCheckEdit.Name = "activeCheckEdit";
        activeCheckEdit.Properties.Caption = "Activo";
        activeCheckEdit.Size = new Size(75, 20);
        activeCheckEdit.TabIndex = 18;
        // 
        // btnGuardar
        // 
        btnGuardar.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnGuardar.Appearance.ForeColor = Color.White;
        btnGuardar.Appearance.Options.UseBackColor = true;
        btnGuardar.Appearance.Options.UseForeColor = true;
        btnGuardar.Location = new Point(294, 276);
        btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(100, 32);
        btnGuardar.TabIndex = 19;
        btnGuardar.Text = "Guardar";
        // 
        // btnCancelar
        // 
        btnCancelar.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        btnCancelar.Appearance.Options.UseForeColor = true;
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.Location = new Point(400, 276);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(100, 32);
        btnCancelar.TabIndex = 20;
        btnCancelar.Text = "Cancelar";
        // 
        // OperationEditForm
        // 
        AcceptButton = btnGuardar;
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancelar;
        ClientSize = new Size(540, 327);
        Controls.Add(lblCodigo);
        Controls.Add(codeTextEdit);
        Controls.Add(lblNombre);
        Controls.Add(nameTextEdit);
        Controls.Add(lblDescripcion);
        Controls.Add(descriptionMemoEdit);
        Controls.Add(lblRibbonPage);
        Controls.Add(ribbonPageTextEdit);
        Controls.Add(lblRibbonGroup);
        Controls.Add(ribbonGroupTextEdit);
        Controls.Add(lblActionKey);
        Controls.Add(actionKeyTextEdit);
        Controls.Add(lblIconLarge);
        Controls.Add(iconLargeTextEdit);
        Controls.Add(lblIconSmall);
        Controls.Add(iconSmallTextEdit);
        Controls.Add(lblDisplayOrder);
        Controls.Add(displayOrderSpinEdit);
        Controls.Add(activeCheckEdit);
        Controls.Add(btnGuardar);
        Controls.Add(btnCancelar);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Name = "OperationEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Nueva operacion";
        ((System.ComponentModel.ISupportInitialize)codeTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)nameTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)descriptionMemoEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)ribbonPageTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)ribbonGroupTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)actionKeyTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)iconLargeTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)iconSmallTextEdit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)displayOrderSpinEdit.Properties).EndInit();
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
    private LabelControl lblRibbonPage;
    private TextEdit ribbonPageTextEdit;
    private LabelControl lblRibbonGroup;
    private TextEdit ribbonGroupTextEdit;
    private LabelControl lblActionKey;
    private TextEdit actionKeyTextEdit;
    private LabelControl lblIconLarge;
    private TextEdit iconLargeTextEdit;
    private LabelControl lblIconSmall;
    private TextEdit iconSmallTextEdit;
    private LabelControl lblDisplayOrder;
    private SpinEdit displayOrderSpinEdit;
    private CheckEdit activeCheckEdit;
    private SimpleButton btnGuardar;
    private SimpleButton btnCancelar;
}

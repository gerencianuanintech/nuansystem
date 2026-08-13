using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Editors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ProductTypes;

partial class ProductTypeEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblGeneralTitle = new LabelControl();
        lineGeneralTitle = new LabelControl();
        lblCode = new LabelControl();
        txtCode = new TextEdit();
        lblName = new LabelControl();
        txtName = new TextEdit();
        lblSortOrder = new LabelControl();
        spnSortOrder = new SpinEdit();
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblIsActive = new LabelControl();
        chkIsActive = new NuanToggleSwitch();
        lblIsSystem = new LabelControl();
        chkIsSystem = new NuanToggleSwitch();
        lblGeneralInfoIcon = new LabelControl();
        lblGeneralNote = new LabelControl();
        lblClassificationTitle = new LabelControl();
        lineClassificationTitle = new LabelControl();
        lblNature = new LabelControl();
        lueNature = new LookUpEdit();
        lblNatureHelp = new LabelControl();
        lblClassificationInfoIcon = new LabelControl();
        lblClassificationNote = new LabelControl();
        lblNatureValues = new LabelControl();
        lineFooter = new LabelControl();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsSystem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueNature.Properties).BeginInit();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Location = new Point(974, 416);
        // 
        // btnGuardar
        // 
        btnGuardar.Location = new Point(1080, 416);
        // 
        // lblGeneralTitle
        // 
        lblGeneralTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneralTitle.Appearance.ForeColor = BrandResources.Primary;
        lblGeneralTitle.Appearance.Options.UseFont = true;
        lblGeneralTitle.Appearance.Options.UseForeColor = true;
        lblGeneralTitle.Location = new Point(32, 22);
        lblGeneralTitle.Name = "lblGeneralTitle";
        lblGeneralTitle.Size = new Size(164, 20);
        lblGeneralTitle.TabIndex = 20;
        lblGeneralTitle.Text = "1. Información general";
        // 
        // lineGeneralTitle
        // 
        lineGeneralTitle.Appearance.BackColor = BrandResources.Border;
        lineGeneralTitle.Appearance.Options.UseBackColor = true;
        lineGeneralTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lineGeneralTitle.Location = new Point(218, 34);
        lineGeneralTitle.Name = "lineGeneralTitle";
        lineGeneralTitle.Size = new Size(950, 1);
        lineGeneralTitle.TabIndex = 21;
        // 
        // lblCode
        // 
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.ForeColor = BrandResources.Text;
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Appearance.Options.UseForeColor = true;
        lblCode.Location = new Point(32, 63);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(42, 15);
        lblCode.TabIndex = 22;
        lblCode.Text = "Código:";
        // 
        // txtCode
        // 
        txtCode.Location = new Point(150, 60);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 50;
        txtCode.Size = new Size(180, 22);
        txtCode.TabIndex = 0;
        // 
        // lblName
        // 
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.ForeColor = BrandResources.Text;
        lblName.Appearance.Options.UseFont = true;
        lblName.Appearance.Options.UseForeColor = true;
        lblName.Location = new Point(360, 63);
        lblName.Name = "lblName";
        lblName.Size = new Size(47, 15);
        lblName.TabIndex = 23;
        lblName.Text = "Nombre:";
        // 
        // txtName
        // 
        txtName.Location = new Point(455, 60);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(455, 22);
        txtName.TabIndex = 1;
        // 
        // lblSortOrder
        // 
        lblSortOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblSortOrder.Appearance.ForeColor = BrandResources.Text;
        lblSortOrder.Appearance.Options.UseFont = true;
        lblSortOrder.Appearance.Options.UseForeColor = true;
        lblSortOrder.Location = new Point(940, 63);
        lblSortOrder.Name = "lblSortOrder";
        lblSortOrder.Size = new Size(40, 15);
        lblSortOrder.TabIndex = 24;
        lblSortOrder.Text = "Orden:";
        // 
        // spnSortOrder
        // 
        spnSortOrder.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnSortOrder.Location = new Point(1018, 60);
        spnSortOrder.Name = "spnSortOrder";
        spnSortOrder.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSortOrder.Properties.Appearance.Options.UseFont = true;
        spnSortOrder.Properties.Appearance.Options.UseTextOptions = true;
        spnSortOrder.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnSortOrder.Properties.AutoHeight = false;
        spnSortOrder.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSortOrder.Properties.IsFloatValue = false;
        spnSortOrder.Properties.MaskSettings.Set("mask", "d");
        spnSortOrder.Properties.MaxValue = new decimal(new int[] { 9999, 0, 0, 0 });
        spnSortOrder.Size = new Size(150, 22);
        spnSortOrder.TabIndex = 2;
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.ForeColor = BrandResources.Text;
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Appearance.Options.UseForeColor = true;
        lblDescription.Location = new Point(32, 91);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(65, 15);
        lblDescription.TabIndex = 25;
        lblDescription.Text = "Descripción:";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(150, 88);
        memDescription.Name = "memDescription";
        memDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescription.Properties.Appearance.Options.UseFont = true;
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(760, 80);
        memDescription.TabIndex = 3;
        // 
        // lblIsActive
        // 
        lblIsActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblIsActive.Appearance.ForeColor = BrandResources.Text;
        lblIsActive.Appearance.Options.UseFont = true;
        lblIsActive.Appearance.Options.UseForeColor = true;
        lblIsActive.Location = new Point(940, 91);
        lblIsActive.Name = "lblIsActive";
        lblIsActive.Size = new Size(40, 15);
        lblIsActive.TabIndex = 26;
        lblIsActive.Text = "Activo:";
        // 
        // chkIsActive
        // 
        chkIsActive.ActiveColor = BrandResources.Primary;
        chkIsActive.InactiveColor = BrandResources.Border;
        chkIsActive.Location = new Point(1055, 89);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsActive.Properties.Appearance.Options.UseFont = true;
        chkIsActive.Properties.OffText = "No";
        chkIsActive.Properties.OnText = "Sí";
        chkIsActive.Size = new Size(70, 20);
        chkIsActive.StateTextColor = BrandResources.Text;
        chkIsActive.TabIndex = 4;
        // 
        // lblIsSystem
        // 
        lblIsSystem.Appearance.Font = new Font("Segoe UI", 9F);
        lblIsSystem.Appearance.ForeColor = BrandResources.Text;
        lblIsSystem.Appearance.Options.UseFont = true;
        lblIsSystem.Appearance.Options.UseForeColor = true;
        lblIsSystem.Location = new Point(940, 119);
        lblIsSystem.Name = "lblIsSystem";
        lblIsSystem.Size = new Size(94, 15);
        lblIsSystem.TabIndex = 27;
        lblIsSystem.Text = "Tipo del sistema:";
        // 
        // chkIsSystem
        // 
        chkIsSystem.ActiveColor = BrandResources.Primary;
        chkIsSystem.Enabled = false;
        chkIsSystem.InactiveColor = BrandResources.Border;
        chkIsSystem.Location = new Point(1055, 117);
        chkIsSystem.Name = "chkIsSystem";
        chkIsSystem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsSystem.Properties.Appearance.Options.UseFont = true;
        chkIsSystem.Properties.OffText = "No";
        chkIsSystem.Properties.OnText = "Sí";
        chkIsSystem.Size = new Size(70, 20);
        chkIsSystem.StateTextColor = BrandResources.Text;
        chkIsSystem.TabIndex = 5;
        // 
        // lblGeneralInfoIcon
        // 
        lblGeneralInfoIcon.Appearance.Font = new Font("Segoe UI", 12F);
        lblGeneralInfoIcon.Appearance.ForeColor = BrandResources.Primary;
        lblGeneralInfoIcon.Appearance.Options.UseFont = true;
        lblGeneralInfoIcon.Appearance.Options.UseForeColor = true;
        lblGeneralInfoIcon.Location = new Point(32, 184);
        lblGeneralInfoIcon.Name = "lblGeneralInfoIcon";
        lblGeneralInfoIcon.Size = new Size(16, 21);
        lblGeneralInfoIcon.TabIndex = 28;
        lblGeneralInfoIcon.Text = "ⓘ";
        // 
        // lblGeneralNote
        // 
        lblGeneralNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblGeneralNote.Appearance.ForeColor = BrandResources.MutedText;
        lblGeneralNote.Appearance.Options.UseFont = true;
        lblGeneralNote.Appearance.Options.UseForeColor = true;
        lblGeneralNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblGeneralNote.Location = new Point(56, 188);
        lblGeneralNote.Name = "lblGeneralNote";
        lblGeneralNote.Size = new Size(1112, 20);
        lblGeneralNote.TabIndex = 29;
        lblGeneralNote.Text = "El código del tipo de producto es único dentro de la empresa.";
        // 
        // lblClassificationTitle
        // 
        lblClassificationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblClassificationTitle.Appearance.ForeColor = BrandResources.Primary;
        lblClassificationTitle.Appearance.Options.UseFont = true;
        lblClassificationTitle.Appearance.Options.UseForeColor = true;
        lblClassificationTitle.Location = new Point(32, 226);
        lblClassificationTitle.Name = "lblClassificationTitle";
        lblClassificationTitle.Size = new Size(178, 20);
        lblClassificationTitle.TabIndex = 30;
        lblClassificationTitle.Text = "2. Clasificación funcional";
        // 
        // lineClassificationTitle
        // 
        lineClassificationTitle.Appearance.BackColor = BrandResources.Border;
        lineClassificationTitle.Appearance.Options.UseBackColor = true;
        lineClassificationTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lineClassificationTitle.Location = new Point(232, 238);
        lineClassificationTitle.Name = "lineClassificationTitle";
        lineClassificationTitle.Size = new Size(936, 1);
        lineClassificationTitle.TabIndex = 31;
        // 
        // lblNature
        // 
        lblNature.Appearance.Font = new Font("Segoe UI", 9F);
        lblNature.Appearance.ForeColor = BrandResources.Text;
        lblNature.Appearance.Options.UseFont = true;
        lblNature.Appearance.Options.UseForeColor = true;
        lblNature.Location = new Point(32, 267);
        lblNature.Name = "lblNature";
        lblNature.Size = new Size(66, 15);
        lblNature.TabIndex = 32;
        lblNature.Text = "Naturaleza:";
        // 
        // lueNature
        // 
        lueNature.Location = new Point(150, 264);
        lueNature.Name = "lueNature";
        lueNature.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueNature.Properties.Appearance.Options.UseFont = true;
        lueNature.Properties.AutoHeight = false;
        lueNature.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueNature.Properties.NullText = "";
        lueNature.Properties.ShowFooter = false;
        lueNature.Properties.ShowHeader = false;
        lueNature.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        lueNature.Size = new Size(300, 22);
        lueNature.TabIndex = 6;
        // 
        // lblNatureHelp
        // 
        lblNatureHelp.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblNatureHelp.Appearance.ForeColor = BrandResources.MutedText;
        lblNatureHelp.Appearance.Options.UseFont = true;
        lblNatureHelp.Appearance.Options.UseForeColor = true;
        lblNatureHelp.AutoSizeMode = LabelAutoSizeMode.None;
        lblNatureHelp.Location = new Point(480, 267);
        lblNatureHelp.Name = "lblNatureHelp";
        lblNatureHelp.Size = new Size(688, 20);
        lblNatureHelp.TabIndex = 33;
        lblNatureHelp.Text = "Clasifica el papel comercial o productivo que cumple el artículo dentro del ERP.";
        // 
        // lblClassificationInfoIcon
        // 
        lblClassificationInfoIcon.Appearance.Font = new Font("Segoe UI", 12F);
        lblClassificationInfoIcon.Appearance.ForeColor = BrandResources.Primary;
        lblClassificationInfoIcon.Appearance.Options.UseFont = true;
        lblClassificationInfoIcon.Appearance.Options.UseForeColor = true;
        lblClassificationInfoIcon.Location = new Point(32, 304);
        lblClassificationInfoIcon.Name = "lblClassificationInfoIcon";
        lblClassificationInfoIcon.Size = new Size(16, 21);
        lblClassificationInfoIcon.TabIndex = 34;
        lblClassificationInfoIcon.Text = "ⓘ";
        // 
        // lblClassificationNote
        // 
        lblClassificationNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblClassificationNote.Appearance.ForeColor = BrandResources.MutedText;
        lblClassificationNote.Appearance.Options.UseFont = true;
        lblClassificationNote.Appearance.Options.UseForeColor = true;
        lblClassificationNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblClassificationNote.Location = new Point(56, 306);
        lblClassificationNote.Name = "lblClassificationNote";
        lblClassificationNote.Size = new Size(1112, 20);
        lblClassificationNote.TabIndex = 35;
        lblClassificationNote.Text = "La naturaleza no reemplaza al tipo de ítem: el tipo define su comportamiento y la naturaleza su función comercial o productiva.";
        // 
        // lblNatureValues
        // 
        lblNatureValues.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblNatureValues.Appearance.ForeColor = BrandResources.MutedText;
        lblNatureValues.Appearance.Options.UseFont = true;
        lblNatureValues.Appearance.Options.UseForeColor = true;
        lblNatureValues.AutoSizeMode = LabelAutoSizeMode.None;
        lblNatureValues.Location = new Point(56, 330);
        lblNatureValues.Name = "lblNatureValues";
        lblNatureValues.Size = new Size(1112, 20);
        lblNatureValues.TabIndex = 36;
        lblNatureValues.Text = "Valores disponibles: Mercadería, Producto terminado, Materia prima, Semielaborado, Insumo, Empaque, Subproducto y Otro.";
        // 
        // lineFooter
        // 
        lineFooter.Appearance.BackColor = BrandResources.Border;
        lineFooter.Appearance.Options.UseBackColor = true;
        lineFooter.AutoSizeMode = LabelAutoSizeMode.None;
        lineFooter.Location = new Point(0, 398);
        lineFooter.Name = "lineFooter";
        lineFooter.Size = new Size(1200, 1);
        lineFooter.TabIndex = 37;
        // 
        // ProductTypeEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 490);
        Controls.Add(lblGeneralTitle);
        Controls.Add(lineGeneralTitle);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblSortOrder);
        Controls.Add(spnSortOrder);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(lblIsActive);
        Controls.Add(chkIsActive);
        Controls.Add(lblIsSystem);
        Controls.Add(chkIsSystem);
        Controls.Add(lblGeneralInfoIcon);
        Controls.Add(lblGeneralNote);
        Controls.Add(lblClassificationTitle);
        Controls.Add(lineClassificationTitle);
        Controls.Add(lblNature);
        Controls.Add(lueNature);
        Controls.Add(lblNatureHelp);
        Controls.Add(lblClassificationInfoIcon);
        Controls.Add(lblClassificationNote);
        Controls.Add(lblNatureValues);
        Controls.Add(lineFooter);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(1216, 529);
        Name = "ProductTypeEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Tipo de producto";
        Controls.SetChildIndex(btnCancelar, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsSystem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueNature.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private LabelControl lblGeneralTitle;
    private LabelControl lineGeneralTitle;
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblSortOrder;
    private SpinEdit spnSortOrder;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblIsActive;
    private NuanToggleSwitch chkIsActive;
    private LabelControl lblIsSystem;
    private NuanToggleSwitch chkIsSystem;
    private LabelControl lblGeneralInfoIcon;
    private LabelControl lblGeneralNote;
    private LabelControl lblClassificationTitle;
    private LabelControl lineClassificationTitle;
    private LabelControl lblNature;
    private LookUpEdit lueNature;
    private LabelControl lblNatureHelp;
    private LabelControl lblClassificationInfoIcon;
    private LabelControl lblClassificationNote;
    private LabelControl lblNatureValues;
    private LabelControl lineFooter;
}

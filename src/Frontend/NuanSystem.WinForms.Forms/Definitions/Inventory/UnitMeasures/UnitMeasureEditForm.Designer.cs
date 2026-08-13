using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Editors;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.UnitMeasures;

partial class UnitMeasureEditForm
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
        lblGeneralInfoIcon = new LabelControl();
        lblGeneralNote = new LabelControl();
        lblClassificationTitle = new LabelControl();
        lineClassificationTitle = new LabelControl();
        lblMagnitude = new LabelControl();
        cmbMagnitude = new ComboBoxEdit();
        lblSymbol = new LabelControl();
        txtSymbol = new TextEdit();
        lblClassificationInfoIcon = new LabelControl();
        lblClassificationNote = new LabelControl();
        lblIntegrationTitle = new LabelControl();
        lineIntegrationTitle = new LabelControl();
        lblExternalSystem = new LabelControl();
        cmbExternalSystem = new ComboBoxEdit();
        lblExternalCode = new LabelControl();
        txtExternalCode = new TextEdit();
        lblSapMappingNote = new LabelControl();
        lblIntegrationInfoIcon = new LabelControl();
        lblIntegrationNote = new LabelControl();
        lineFooter = new LabelControl();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbMagnitude.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSymbol.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbExternalSystem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalCode.Properties).BeginInit();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Location = new Point(974, 566);
        // 
        // btnGuardar
        // 
        btnGuardar.Location = new Point(1080, 566);
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
        lblCode.Size = new Size(39, 15);
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
        lblName.Size = new Size(44, 15);
        lblName.TabIndex = 23;
        lblName.Text = "Nombre:";
        // 
        // txtName
        // 
        txtName.Location = new Point(430, 60);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(450, 22);
        txtName.TabIndex = 1;
        // 
        // lblSortOrder
        // 
        lblSortOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblSortOrder.Appearance.ForeColor = BrandResources.Text;
        lblSortOrder.Appearance.Options.UseFont = true;
        lblSortOrder.Appearance.Options.UseForeColor = true;
        lblSortOrder.Location = new Point(910, 63);
        lblSortOrder.Name = "lblSortOrder";
        lblSortOrder.Size = new Size(37, 15);
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
        lblDescription.Size = new Size(62, 15);
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
        memDescription.Size = new Size(730, 80);
        memDescription.TabIndex = 3;
        // 
        // lblIsActive
        // 
        lblIsActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblIsActive.Appearance.ForeColor = BrandResources.Text;
        lblIsActive.Appearance.Options.UseFont = true;
        lblIsActive.Appearance.Options.UseForeColor = true;
        lblIsActive.Location = new Point(910, 91);
        lblIsActive.Name = "lblIsActive";
        lblIsActive.Size = new Size(37, 15);
        lblIsActive.TabIndex = 26;
        lblIsActive.Text = "Activo:";
        // 
        // chkIsActive
        // 
        chkIsActive.ActiveColor = BrandResources.Primary;
        chkIsActive.InactiveColor = BrandResources.Border;
        chkIsActive.Location = new Point(1018, 89);
        chkIsActive.Name = "chkIsActive";
        chkIsActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsActive.Properties.Appearance.Options.UseFont = true;
        chkIsActive.Properties.OffText = "No";
        chkIsActive.Properties.OnText = "Sí";
        chkIsActive.Size = new Size(70, 20);
        chkIsActive.StateTextColor = BrandResources.Text;
        chkIsActive.TabIndex = 4;
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
        lblGeneralInfoIcon.TabIndex = 27;
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
        lblGeneralNote.TabIndex = 28;
        lblGeneralNote.Text = "El código identifica de forma única la unidad dentro de la empresa.";
        // 
        // lblClassificationTitle
        // 
        lblClassificationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblClassificationTitle.Appearance.ForeColor = BrandResources.Primary;
        lblClassificationTitle.Appearance.Options.UseFont = true;
        lblClassificationTitle.Appearance.Options.UseForeColor = true;
        lblClassificationTitle.Location = new Point(32, 232);
        lblClassificationTitle.Name = "lblClassificationTitle";
        lblClassificationTitle.Size = new Size(193, 20);
        lblClassificationTitle.TabIndex = 29;
        lblClassificationTitle.Text = "2. Clasificación de la medida";
        // 
        // lineClassificationTitle
        // 
        lineClassificationTitle.Appearance.BackColor = BrandResources.Border;
        lineClassificationTitle.Appearance.Options.UseBackColor = true;
        lineClassificationTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lineClassificationTitle.Location = new Point(246, 244);
        lineClassificationTitle.Name = "lineClassificationTitle";
        lineClassificationTitle.Size = new Size(922, 1);
        lineClassificationTitle.TabIndex = 30;
        // 
        // lblMagnitude
        // 
        lblMagnitude.Appearance.Font = new Font("Segoe UI", 9F);
        lblMagnitude.Appearance.ForeColor = BrandResources.Text;
        lblMagnitude.Appearance.Options.UseFont = true;
        lblMagnitude.Appearance.Options.UseForeColor = true;
        lblMagnitude.Location = new Point(32, 281);
        lblMagnitude.Name = "lblMagnitude";
        lblMagnitude.Size = new Size(98, 15);
        lblMagnitude.TabIndex = 31;
        lblMagnitude.Text = "Tipo de magnitud:";
        // 
        // cmbMagnitude
        // 
        cmbMagnitude.Location = new Point(190, 278);
        cmbMagnitude.Name = "cmbMagnitude";
        cmbMagnitude.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cmbMagnitude.Properties.Appearance.Options.UseFont = true;
        cmbMagnitude.Properties.AutoHeight = false;
        cmbMagnitude.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cmbMagnitude.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        cmbMagnitude.Size = new Size(275, 22);
        cmbMagnitude.TabIndex = 5;
        // 
        // lblSymbol
        // 
        lblSymbol.Appearance.Font = new Font("Segoe UI", 9F);
        lblSymbol.Appearance.ForeColor = BrandResources.Text;
        lblSymbol.Appearance.Options.UseFont = true;
        lblSymbol.Appearance.Options.UseForeColor = true;
        lblSymbol.Location = new Point(520, 281);
        lblSymbol.Name = "lblSymbol";
        lblSymbol.Size = new Size(47, 15);
        lblSymbol.TabIndex = 32;
        lblSymbol.Text = "Símbolo:";
        // 
        // txtSymbol
        // 
        txtSymbol.Location = new Point(620, 278);
        txtSymbol.Name = "txtSymbol";
        txtSymbol.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSymbol.Properties.Appearance.Options.UseFont = true;
        txtSymbol.Properties.AutoHeight = false;
        txtSymbol.Properties.MaxLength = 20;
        txtSymbol.Size = new Size(275, 22);
        txtSymbol.TabIndex = 6;
        // 
        // lblClassificationInfoIcon
        // 
        lblClassificationInfoIcon.Appearance.Font = new Font("Segoe UI", 12F);
        lblClassificationInfoIcon.Appearance.ForeColor = BrandResources.Primary;
        lblClassificationInfoIcon.Appearance.Options.UseFont = true;
        lblClassificationInfoIcon.Appearance.Options.UseForeColor = true;
        lblClassificationInfoIcon.Location = new Point(32, 318);
        lblClassificationInfoIcon.Name = "lblClassificationInfoIcon";
        lblClassificationInfoIcon.Size = new Size(16, 21);
        lblClassificationInfoIcon.TabIndex = 33;
        lblClassificationInfoIcon.Text = "ⓘ";
        // 
        // lblClassificationNote
        // 
        lblClassificationNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblClassificationNote.Appearance.ForeColor = BrandResources.MutedText;
        lblClassificationNote.Appearance.Options.UseFont = true;
        lblClassificationNote.Appearance.Options.UseForeColor = true;
        lblClassificationNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblClassificationNote.Location = new Point(56, 322);
        lblClassificationNote.Name = "lblClassificationNote";
        lblClassificationNote.Size = new Size(1112, 20);
        lblClassificationNote.TabIndex = 34;
        lblClassificationNote.Text = "Los factores y las presentaciones se definen por artículo; aquí solo se clasifica la unidad.";
        // 
        // lblIntegrationTitle
        // 
        lblIntegrationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblIntegrationTitle.Appearance.ForeColor = BrandResources.Primary;
        lblIntegrationTitle.Appearance.Options.UseFont = true;
        lblIntegrationTitle.Appearance.Options.UseForeColor = true;
        lblIntegrationTitle.Location = new Point(32, 366);
        lblIntegrationTitle.Name = "lblIntegrationTitle";
        lblIntegrationTitle.Size = new Size(209, 20);
        lblIntegrationTitle.TabIndex = 35;
        lblIntegrationTitle.Text = "3. Referencia externa (opcional)";
        // 
        // lineIntegrationTitle
        // 
        lineIntegrationTitle.Appearance.BackColor = BrandResources.Border;
        lineIntegrationTitle.Appearance.Options.UseBackColor = true;
        lineIntegrationTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lineIntegrationTitle.Location = new Point(268, 378);
        lineIntegrationTitle.Name = "lineIntegrationTitle";
        lineIntegrationTitle.Size = new Size(900, 1);
        lineIntegrationTitle.TabIndex = 36;
        // 
        // lblExternalSystem
        // 
        lblExternalSystem.Appearance.Font = new Font("Segoe UI", 9F);
        lblExternalSystem.Appearance.ForeColor = BrandResources.Text;
        lblExternalSystem.Appearance.Options.UseFont = true;
        lblExternalSystem.Appearance.Options.UseForeColor = true;
        lblExternalSystem.Location = new Point(32, 415);
        lblExternalSystem.Name = "lblExternalSystem";
        lblExternalSystem.Size = new Size(87, 15);
        lblExternalSystem.TabIndex = 37;
        lblExternalSystem.Text = "Sistema externo:";
        // 
        // cmbExternalSystem
        // 
        cmbExternalSystem.Location = new Point(190, 412);
        cmbExternalSystem.Name = "cmbExternalSystem";
        cmbExternalSystem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cmbExternalSystem.Properties.Appearance.Options.UseFont = true;
        cmbExternalSystem.Properties.AutoHeight = false;
        cmbExternalSystem.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cmbExternalSystem.Properties.Items.AddRange(new object[] { "SAP_B1" });
        cmbExternalSystem.Properties.MaxLength = 50;
        cmbExternalSystem.Size = new Size(275, 22);
        cmbExternalSystem.TabIndex = 7;
        // 
        // lblExternalCode
        // 
        lblExternalCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblExternalCode.Appearance.ForeColor = BrandResources.Text;
        lblExternalCode.Appearance.Options.UseFont = true;
        lblExternalCode.Appearance.Options.UseForeColor = true;
        lblExternalCode.Location = new Point(500, 415);
        lblExternalCode.Name = "lblExternalCode";
        lblExternalCode.Size = new Size(82, 15);
        lblExternalCode.TabIndex = 38;
        lblExternalCode.Text = "Código externo:";
        // 
        // txtExternalCode
        // 
        txtExternalCode.Location = new Point(620, 412);
        txtExternalCode.Name = "txtExternalCode";
        txtExternalCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtExternalCode.Properties.Appearance.Options.UseFont = true;
        txtExternalCode.Properties.AutoHeight = false;
        txtExternalCode.Properties.MaxLength = 100;
        txtExternalCode.Size = new Size(230, 22);
        txtExternalCode.TabIndex = 8;
        // 
        // lblSapMappingNote
        // 
        lblSapMappingNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblSapMappingNote.Appearance.ForeColor = BrandResources.MutedText;
        lblSapMappingNote.Appearance.Options.UseFont = true;
        lblSapMappingNote.Appearance.Options.UseForeColor = true;
        lblSapMappingNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblSapMappingNote.Location = new Point(880, 415);
        lblSapMappingNote.Name = "lblSapMappingNote";
        lblSapMappingNote.Size = new Size(288, 20);
        lblSapMappingNote.TabIndex = 39;
        lblSapMappingNote.Text = "La equivalencia SAP se administra por empresa.";
        // 
        // lblIntegrationInfoIcon
        // 
        lblIntegrationInfoIcon.Appearance.Font = new Font("Segoe UI", 12F);
        lblIntegrationInfoIcon.Appearance.ForeColor = BrandResources.Primary;
        lblIntegrationInfoIcon.Appearance.Options.UseFont = true;
        lblIntegrationInfoIcon.Appearance.Options.UseForeColor = true;
        lblIntegrationInfoIcon.Location = new Point(32, 452);
        lblIntegrationInfoIcon.Name = "lblIntegrationInfoIcon";
        lblIntegrationInfoIcon.Size = new Size(16, 21);
        lblIntegrationInfoIcon.TabIndex = 40;
        lblIntegrationInfoIcon.Text = "ⓘ";
        // 
        // lblIntegrationNote
        // 
        lblIntegrationNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblIntegrationNote.Appearance.ForeColor = BrandResources.MutedText;
        lblIntegrationNote.Appearance.Options.UseFont = true;
        lblIntegrationNote.Appearance.Options.UseForeColor = true;
        lblIntegrationNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblIntegrationNote.Location = new Point(56, 456);
        lblIntegrationNote.Name = "lblIntegrationNote";
        lblIntegrationNote.Size = new Size(1112, 20);
        lblIntegrationNote.TabIndex = 41;
        lblIntegrationNote.Text = "La referencia externa es opcional y no condiciona la operación local del ERP.";
        // 
        // lineFooter
        // 
        lineFooter.Appearance.BackColor = BrandResources.Border;
        lineFooter.Appearance.Options.UseBackColor = true;
        lineFooter.AutoSizeMode = LabelAutoSizeMode.None;
        lineFooter.Location = new Point(0, 548);
        lineFooter.Name = "lineFooter";
        lineFooter.Size = new Size(1200, 1);
        lineFooter.TabIndex = 42;
        // 
        // UnitMeasureEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 622);
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
        Controls.Add(lblGeneralInfoIcon);
        Controls.Add(lblGeneralNote);
        Controls.Add(lblClassificationTitle);
        Controls.Add(lineClassificationTitle);
        Controls.Add(lblMagnitude);
        Controls.Add(cmbMagnitude);
        Controls.Add(lblSymbol);
        Controls.Add(txtSymbol);
        Controls.Add(lblClassificationInfoIcon);
        Controls.Add(lblClassificationNote);
        Controls.Add(lblIntegrationTitle);
        Controls.Add(lineIntegrationTitle);
        Controls.Add(lblExternalSystem);
        Controls.Add(cmbExternalSystem);
        Controls.Add(lblExternalCode);
        Controls.Add(txtExternalCode);
        Controls.Add(lblSapMappingNote);
        Controls.Add(lblIntegrationInfoIcon);
        Controls.Add(lblIntegrationNote);
        Controls.Add(lineFooter);
        Font = new Font("Segoe UI", 9F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        MinimumSize = new Size(1216, 661);
        Name = "UnitMeasureEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Unidad de medida";
        Controls.SetChildIndex(btnCancelar, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbMagnitude.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSymbol.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbExternalSystem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalCode.Properties).EndInit();
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
    private LabelControl lblGeneralInfoIcon;
    private LabelControl lblGeneralNote;
    private LabelControl lblClassificationTitle;
    private LabelControl lineClassificationTitle;
    private LabelControl lblMagnitude;
    private ComboBoxEdit cmbMagnitude;
    private LabelControl lblSymbol;
    private TextEdit txtSymbol;
    private LabelControl lblClassificationInfoIcon;
    private LabelControl lblClassificationNote;
    private LabelControl lblIntegrationTitle;
    private LabelControl lineIntegrationTitle;
    private LabelControl lblExternalSystem;
    private ComboBoxEdit cmbExternalSystem;
    private LabelControl lblExternalCode;
    private TextEdit txtExternalCode;
    private LabelControl lblSapMappingNote;
    private LabelControl lblIntegrationInfoIcon;
    private LabelControl lblIntegrationNote;
    private LabelControl lineFooter;
}

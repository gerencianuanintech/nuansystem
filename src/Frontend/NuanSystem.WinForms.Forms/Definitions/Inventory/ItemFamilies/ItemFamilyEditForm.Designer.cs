using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Editors;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemFamilies;

partial class ItemFamilyEditForm
{
    private System.ComponentModel.IContainer components = null;

    private void InitializeComponent()
    {
        lblGeneralTitle = new LabelControl();
        lineGeneralTitle = new LabelControl();
        lblItemGroup = new LabelControl();
        lueItemGroup = new NuanLookupEdit();
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
        lblIntegrationTitle = new LabelControl();
        lineIntegrationTitle = new LabelControl();
        lblExternalSystem = new LabelControl();
        cmbExternalSystem = new ComboBoxEdit();
        lblExternalCode = new LabelControl();
        txtExternalCode = new TextEdit();
        lblSapFamilyCode = new LabelControl();
        txtSapFamilyCode = new TextEdit();
        lblSapCode = new LabelControl();
        txtSapCode = new TextEdit();
        lblIntegrationInfoIcon = new LabelControl();
        lblIntegrationNote = new LabelControl();
        lineFooter = new LabelControl();
        ((System.ComponentModel.ISupportInitialize)lueItemGroup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)cmbExternalSystem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapFamilyCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCode.Properties).BeginInit();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Location = new Point(974, 444);
        // 
        // btnGuardar
        // 
        btnGuardar.Location = new Point(1080, 444);
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
        lblGeneralTitle.TabIndex = 30;
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
        lineGeneralTitle.TabIndex = 31;
        // 
        // lblItemGroup
        // 
        lblItemGroup.Appearance.Font = new Font("Segoe UI", 9F);
        lblItemGroup.Appearance.ForeColor = BrandResources.Text;
        lblItemGroup.Appearance.Options.UseFont = true;
        lblItemGroup.Appearance.Options.UseForeColor = true;
        lblItemGroup.Location = new Point(32, 63);
        lblItemGroup.Name = "lblItemGroup";
        lblItemGroup.Size = new Size(103, 15);
        lblItemGroup.TabIndex = 32;
        lblItemGroup.Text = "Grupo de artículos:";
        // 
        // lueItemGroup
        // 
        lueItemGroup.ClearButtonEnabled = false;
        lueItemGroup.Location = new Point(210, 60);
        lueItemGroup.Name = "lueItemGroup";
        lueItemGroup.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueItemGroup.Properties.Appearance.Options.UseFont = true;
        lueItemGroup.Properties.AutoHeight = false;
        lueItemGroup.Properties.Buttons.Clear();
        lueItemGroup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        lueItemGroup.Properties.NullText = "";
        lueItemGroup.Size = new Size(650, 22);
        lueItemGroup.TabIndex = 0;
        // 
        // lblCode
        // 
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.ForeColor = BrandResources.Text;
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Appearance.Options.UseForeColor = true;
        lblCode.Location = new Point(32, 91);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(39, 15);
        lblCode.TabIndex = 33;
        lblCode.Text = "Código:";
        // 
        // txtCode
        // 
        txtCode.Location = new Point(210, 88);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 50;
        txtCode.Size = new Size(180, 22);
        txtCode.TabIndex = 2;
        // 
        // lblName
        // 
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.ForeColor = BrandResources.Text;
        lblName.Appearance.Options.UseFont = true;
        lblName.Appearance.Options.UseForeColor = true;
        lblName.Location = new Point(430, 91);
        lblName.Name = "lblName";
        lblName.Size = new Size(44, 15);
        lblName.TabIndex = 34;
        lblName.Text = "Nombre:";
        // 
        // txtName
        // 
        txtName.Location = new Point(500, 88);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(450, 22);
        txtName.TabIndex = 3;
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
        lblSortOrder.TabIndex = 35;
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
        spnSortOrder.TabIndex = 1;
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.ForeColor = BrandResources.Text;
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Appearance.Options.UseForeColor = true;
        lblDescription.Location = new Point(32, 119);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(62, 15);
        lblDescription.TabIndex = 36;
        lblDescription.Text = "Descripción:";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(210, 116);
        memDescription.Name = "memDescription";
        memDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescription.Properties.Appearance.Options.UseFont = true;
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(958, 80);
        memDescription.TabIndex = 5;
        // 
        // lblIsActive
        // 
        lblIsActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblIsActive.Appearance.ForeColor = BrandResources.Text;
        lblIsActive.Appearance.Options.UseFont = true;
        lblIsActive.Appearance.Options.UseForeColor = true;
        lblIsActive.Location = new Point(985, 91);
        lblIsActive.Name = "lblIsActive";
        lblIsActive.Size = new Size(37, 15);
        lblIsActive.TabIndex = 37;
        lblIsActive.Text = "Activo:";
        // 
        // chkIsActive
        // 
        chkIsActive.ActiveColor = BrandResources.Primary;
        chkIsActive.InactiveColor = BrandResources.Border;
        chkIsActive.Location = new Point(1040, 89);
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
        lblGeneralInfoIcon.Location = new Point(32, 212);
        lblGeneralInfoIcon.Name = "lblGeneralInfoIcon";
        lblGeneralInfoIcon.Size = new Size(16, 21);
        lblGeneralInfoIcon.TabIndex = 38;
        lblGeneralInfoIcon.Text = "ⓘ";
        // 
        // lblGeneralNote
        // 
        lblGeneralNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblGeneralNote.Appearance.ForeColor = BrandResources.MutedText;
        lblGeneralNote.Appearance.Options.UseFont = true;
        lblGeneralNote.Appearance.Options.UseForeColor = true;
        lblGeneralNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblGeneralNote.Location = new Point(56, 216);
        lblGeneralNote.Name = "lblGeneralNote";
        lblGeneralNote.Size = new Size(1112, 20);
        lblGeneralNote.TabIndex = 39;
        lblGeneralNote.Text = "El código de la familia es único dentro del grupo de artículos.";
        // 
        // lblIntegrationTitle
        // 
        lblIntegrationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblIntegrationTitle.Appearance.ForeColor = BrandResources.Primary;
        lblIntegrationTitle.Appearance.Options.UseFont = true;
        lblIntegrationTitle.Appearance.Options.UseForeColor = true;
        lblIntegrationTitle.Location = new Point(32, 260);
        lblIntegrationTitle.Name = "lblIntegrationTitle";
        lblIntegrationTitle.Size = new Size(214, 20);
        lblIntegrationTitle.TabIndex = 40;
        lblIntegrationTitle.Text = "2. Integración externa (opcional)";
        // 
        // lineIntegrationTitle
        // 
        lineIntegrationTitle.Appearance.BackColor = BrandResources.Border;
        lineIntegrationTitle.Appearance.Options.UseBackColor = true;
        lineIntegrationTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lineIntegrationTitle.Location = new Point(292, 272);
        lineIntegrationTitle.Name = "lineIntegrationTitle";
        lineIntegrationTitle.Size = new Size(876, 1);
        lineIntegrationTitle.TabIndex = 41;
        // 
        // lblExternalSystem
        // 
        lblExternalSystem.Appearance.Font = new Font("Segoe UI", 9F);
        lblExternalSystem.Appearance.ForeColor = BrandResources.Text;
        lblExternalSystem.Appearance.Options.UseFont = true;
        lblExternalSystem.Appearance.Options.UseForeColor = true;
        lblExternalSystem.Location = new Point(32, 309);
        lblExternalSystem.Name = "lblExternalSystem";
        lblExternalSystem.Size = new Size(87, 15);
        lblExternalSystem.TabIndex = 42;
        lblExternalSystem.Text = "Sistema externo:";
        // 
        // cmbExternalSystem
        // 
        cmbExternalSystem.Location = new Point(150, 306);
        cmbExternalSystem.Name = "cmbExternalSystem";
        cmbExternalSystem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        cmbExternalSystem.Properties.Appearance.Options.UseFont = true;
        cmbExternalSystem.Properties.AutoHeight = false;
        cmbExternalSystem.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        cmbExternalSystem.Properties.Items.AddRange(new object[] { "SAP_B1" });
        cmbExternalSystem.Properties.MaxLength = 50;
        cmbExternalSystem.Size = new Size(180, 22);
        cmbExternalSystem.TabIndex = 6;
        // 
        // lblExternalCode
        // 
        lblExternalCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblExternalCode.Appearance.ForeColor = BrandResources.Text;
        lblExternalCode.Appearance.Options.UseFont = true;
        lblExternalCode.Appearance.Options.UseForeColor = true;
        lblExternalCode.Location = new Point(360, 309);
        lblExternalCode.Name = "lblExternalCode";
        lblExternalCode.Size = new Size(82, 15);
        lblExternalCode.TabIndex = 43;
        lblExternalCode.Text = "Código externo:";
        // 
        // txtExternalCode
        // 
        txtExternalCode.Location = new Point(455, 306);
        txtExternalCode.Name = "txtExternalCode";
        txtExternalCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtExternalCode.Properties.Appearance.Options.UseFont = true;
        txtExternalCode.Properties.AutoHeight = false;
        txtExternalCode.Properties.MaxLength = 100;
        txtExternalCode.Size = new Size(155, 22);
        txtExternalCode.TabIndex = 7;
        // 
        // lblSapFamilyCode
        // 
        lblSapFamilyCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapFamilyCode.Appearance.ForeColor = BrandResources.Text;
        lblSapFamilyCode.Appearance.Options.UseFont = true;
        lblSapFamilyCode.Appearance.Options.UseForeColor = true;
        lblSapFamilyCode.Location = new Point(650, 309);
        lblSapFamilyCode.Name = "lblSapFamilyCode";
        lblSapFamilyCode.Size = new Size(82, 15);
        lblSapFamilyCode.TabIndex = 44;
        lblSapFamilyCode.Text = "Familia SAP:";
        // 
        // txtSapFamilyCode
        // 
        txtSapFamilyCode.Location = new Point(740, 306);
        txtSapFamilyCode.Name = "txtSapFamilyCode";
        txtSapFamilyCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapFamilyCode.Properties.Appearance.Options.UseFont = true;
        txtSapFamilyCode.Properties.AutoHeight = false;
        txtSapFamilyCode.Properties.MaxLength = 100;
        txtSapFamilyCode.Size = new Size(170, 22);
        txtSapFamilyCode.TabIndex = 8;
        // 
        // lblSapCode
        // 
        lblSapCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapCode.Appearance.ForeColor = BrandResources.Text;
        lblSapCode.Appearance.Options.UseFont = true;
        lblSapCode.Appearance.Options.UseForeColor = true;
        lblSapCode.Location = new Point(940, 309);
        lblSapCode.Name = "lblSapCode";
        lblSapCode.Size = new Size(62, 15);
        lblSapCode.TabIndex = 45;
        lblSapCode.Text = "Código SAP:";
        // 
        // txtSapCode
        // 
        txtSapCode.Location = new Point(1030, 306);
        txtSapCode.Name = "txtSapCode";
        txtSapCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapCode.Properties.Appearance.Options.UseFont = true;
        txtSapCode.Properties.AutoHeight = false;
        txtSapCode.Properties.MaxLength = 50;
        txtSapCode.Size = new Size(138, 22);
        txtSapCode.TabIndex = 9;
        // 
        // lblIntegrationInfoIcon
        // 
        lblIntegrationInfoIcon.Appearance.Font = new Font("Segoe UI", 12F);
        lblIntegrationInfoIcon.Appearance.ForeColor = BrandResources.Primary;
        lblIntegrationInfoIcon.Appearance.Options.UseFont = true;
        lblIntegrationInfoIcon.Appearance.Options.UseForeColor = true;
        lblIntegrationInfoIcon.Location = new Point(32, 346);
        lblIntegrationInfoIcon.Name = "lblIntegrationInfoIcon";
        lblIntegrationInfoIcon.Size = new Size(16, 21);
        lblIntegrationInfoIcon.TabIndex = 46;
        lblIntegrationInfoIcon.Text = "ⓘ";
        // 
        // lblIntegrationNote
        // 
        lblIntegrationNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblIntegrationNote.Appearance.ForeColor = BrandResources.MutedText;
        lblIntegrationNote.Appearance.Options.UseFont = true;
        lblIntegrationNote.Appearance.Options.UseForeColor = true;
        lblIntegrationNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblIntegrationNote.Location = new Point(56, 350);
        lblIntegrationNote.Name = "lblIntegrationNote";
        lblIntegrationNote.Size = new Size(1112, 20);
        lblIntegrationNote.TabIndex = 47;
        lblIntegrationNote.Text = "La integración es opcional y no condiciona la operación local del ERP.";
        // 
        // lineFooter
        // 
        lineFooter.Appearance.BackColor = BrandResources.Border;
        lineFooter.Appearance.Options.UseBackColor = true;
        lineFooter.AutoSizeMode = LabelAutoSizeMode.None;
        lineFooter.Location = new Point(0, 426);
        lineFooter.Name = "lineFooter";
        lineFooter.Size = new Size(1200, 1);
        lineFooter.TabIndex = 48;
        // 
        // ItemFamilyEditForm
        // 
        Appearance.BackColor = Color.White;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 500);
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        Controls.Add(lblGeneralTitle);
        Controls.Add(lineGeneralTitle);
        Controls.Add(lblItemGroup);
        Controls.Add(lueItemGroup);
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
        Controls.Add(lblIntegrationTitle);
        Controls.Add(lineIntegrationTitle);
        Controls.Add(lblExternalSystem);
        Controls.Add(cmbExternalSystem);
        Controls.Add(lblExternalCode);
        Controls.Add(txtExternalCode);
        Controls.Add(lblSapFamilyCode);
        Controls.Add(txtSapFamilyCode);
        Controls.Add(lblSapCode);
        Controls.Add(txtSapCode);
        Controls.Add(lblIntegrationInfoIcon);
        Controls.Add(lblIntegrationNote);
        Controls.Add(lineFooter);
        MinimumSize = new Size(1216, 539);
        Name = "ItemFamilyEditForm";
        Text = "Familia de artículos";
        Controls.SetChildIndex(lineFooter, 0);
        Controls.SetChildIndex(lblIntegrationNote, 0);
        Controls.SetChildIndex(lblIntegrationInfoIcon, 0);
        Controls.SetChildIndex(txtSapCode, 0);
        Controls.SetChildIndex(lblSapCode, 0);
        Controls.SetChildIndex(txtSapFamilyCode, 0);
        Controls.SetChildIndex(lblSapFamilyCode, 0);
        Controls.SetChildIndex(txtExternalCode, 0);
        Controls.SetChildIndex(lblExternalCode, 0);
        Controls.SetChildIndex(cmbExternalSystem, 0);
        Controls.SetChildIndex(lblExternalSystem, 0);
        Controls.SetChildIndex(lineIntegrationTitle, 0);
        Controls.SetChildIndex(lblIntegrationTitle, 0);
        Controls.SetChildIndex(lblGeneralNote, 0);
        Controls.SetChildIndex(lblGeneralInfoIcon, 0);
        Controls.SetChildIndex(chkIsActive, 0);
        Controls.SetChildIndex(lblIsActive, 0);
        Controls.SetChildIndex(memDescription, 0);
        Controls.SetChildIndex(lblDescription, 0);
        Controls.SetChildIndex(spnSortOrder, 0);
        Controls.SetChildIndex(lblSortOrder, 0);
        Controls.SetChildIndex(txtName, 0);
        Controls.SetChildIndex(lblName, 0);
        Controls.SetChildIndex(txtCode, 0);
        Controls.SetChildIndex(lblCode, 0);
        Controls.SetChildIndex(lueItemGroup, 0);
        Controls.SetChildIndex(lblItemGroup, 0);
        Controls.SetChildIndex(lineGeneralTitle, 0);
        Controls.SetChildIndex(lblGeneralTitle, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)lueItemGroup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)cmbExternalSystem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapFamilyCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCode.Properties).EndInit();
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
    private LabelControl lblItemGroup;
    private NuanLookupEdit lueItemGroup;
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
    private LabelControl lblIntegrationTitle;
    private LabelControl lineIntegrationTitle;
    private LabelControl lblExternalSystem;
    private ComboBoxEdit cmbExternalSystem;
    private LabelControl lblExternalCode;
    private TextEdit txtExternalCode;
    private LabelControl lblSapFamilyCode;
    private TextEdit txtSapFamilyCode;
    private LabelControl lblSapCode;
    private TextEdit txtSapCode;
    private LabelControl lblIntegrationInfoIcon;
    private LabelControl lblIntegrationNote;
    private LabelControl lineFooter;
}

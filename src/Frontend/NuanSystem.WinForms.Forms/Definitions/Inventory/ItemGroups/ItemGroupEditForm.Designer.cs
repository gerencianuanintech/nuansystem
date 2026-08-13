using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using NuanSystem.WinForms.Controls.Editors;
using NuanSystem.WinForms.Controls.Lookups;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Definitions.Inventory.ItemGroups;

partial class ItemGroupEditForm
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
        lblDescription = new LabelControl();
        memDescription = new MemoEdit();
        lblSortOrder = new LabelControl();
        spnSortOrder = new SpinEdit();
        lblIsActive = new LabelControl();
        chkIsActive = new NuanToggleSwitch();
        lblIsSystem = new LabelControl();
        chkIsSystem = new NuanToggleSwitch();
        lblAccountingTitle = new LabelControl();
        lineAccountingTitle = new LabelControl();
        lineAccountingColumns = new LabelControl();
        lblInventory = new LabelControl();
        lueInventory = new NuanLookupEdit();
        lblIncome = new LabelControl();
        lueIncome = new NuanLookupEdit();
        lblCostOfSales = new LabelControl();
        lueCostOfSales = new NuanLookupEdit();
        lblSalesReturn = new LabelControl();
        lueSalesReturn = new NuanLookupEdit();
        lblPurchaseExpense = new LabelControl();
        luePurchaseExpense = new NuanLookupEdit();
        lblPurchaseReturn = new LabelControl();
        luePurchaseReturn = new NuanLookupEdit();
        lblCostVariance = new LabelControl();
        lueCostVariance = new NuanLookupEdit();
        lblInventoryAdjustment = new LabelControl();
        lueInventoryAdjustment = new NuanLookupEdit();
        lblAccountingInfoIcon = new LabelControl();
        lblAccountingNote = new LabelControl();
        lblIntegrationTitle = new LabelControl();
        lineIntegrationTitle = new LabelControl();
        lblExternalSystem = new LabelControl();
        txtExternalSystem = new ComboBoxEdit();
        lblExternalCode = new LabelControl();
        txtExternalCode = new TextEdit();
        lblSapGroupCode = new LabelControl();
        txtSapGroupCode = new TextEdit();
        lblSapCode = new LabelControl();
        txtSapCode = new TextEdit();
        lblIntegrationInfoIcon = new LabelControl();
        lblIntegrationNote = new LabelControl();
        lineFooter = new LabelControl();
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsSystem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueInventory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueIncome.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCostOfSales.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesReturn.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseExpense.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseReturn.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCostVariance.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueInventoryAdjustment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalSystem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapGroupCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCode.Properties).BeginInit();
        SuspendLayout();
        // 
        // btnCancelar
        // 
        btnCancelar.Location = new Point(974, 558);
        // 
        // btnGuardar
        // 
        btnGuardar.Location = new Point(1080, 558);
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
        lblGeneralTitle.TabIndex = 40;
        lblGeneralTitle.Text = "1. Información general";
        // 
        // lineGeneralTitle
        // 
        lineGeneralTitle.Appearance.BackColor = BrandResources.Border;
        lineGeneralTitle.Appearance.Options.UseBackColor = true;
        lineGeneralTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lineGeneralTitle.Location = new Point(240, 34);
        lineGeneralTitle.Name = "lineGeneralTitle";
        lineGeneralTitle.Size = new Size(928, 1);
        lineGeneralTitle.TabIndex = 41;
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
        lblCode.TabIndex = 42;
        lblCode.Text = "Código:";
        // 
        // txtCode
        // 
        txtCode.Location = new Point(130, 60);
        txtCode.Name = "txtCode";
        txtCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCode.Properties.Appearance.Options.UseFont = true;
        txtCode.Properties.AutoHeight = false;
        txtCode.Properties.MaxLength = 50;
        txtCode.Size = new Size(150, 22);
        txtCode.TabIndex = 0;
        // 
        // lblName
        // 
        lblName.Appearance.Font = new Font("Segoe UI", 9F);
        lblName.Appearance.ForeColor = BrandResources.Text;
        lblName.Appearance.Options.UseFont = true;
        lblName.Appearance.Options.UseForeColor = true;
        lblName.Location = new Point(350, 63);
        lblName.Name = "lblName";
        lblName.Size = new Size(44, 15);
        lblName.TabIndex = 43;
        lblName.Text = "Nombre:";
        // 
        // txtName
        // 
        txtName.Location = new Point(450, 60);
        txtName.Name = "txtName";
        txtName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtName.Properties.Appearance.Options.UseFont = true;
        txtName.Properties.AutoHeight = false;
        txtName.Properties.MaxLength = 150;
        txtName.Size = new Size(440, 22);
        txtName.TabIndex = 1;
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
        lblDescription.TabIndex = 44;
        lblDescription.Text = "Descripción:";
        // 
        // memDescription
        // 
        memDescription.Location = new Point(130, 88);
        memDescription.Name = "memDescription";
        memDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memDescription.Properties.Appearance.Options.UseFont = true;
        memDescription.Properties.MaxLength = 500;
        memDescription.Size = new Size(760, 80);
        memDescription.TabIndex = 2;
        // 
        // lblSortOrder
        // 
        lblSortOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblSortOrder.Appearance.ForeColor = BrandResources.Text;
        lblSortOrder.Appearance.Options.UseFont = true;
        lblSortOrder.Appearance.Options.UseForeColor = true;
        lblSortOrder.Location = new Point(930, 63);
        lblSortOrder.Name = "lblSortOrder";
        lblSortOrder.Size = new Size(108, 15);
        lblSortOrder.TabIndex = 45;
        lblSortOrder.Text = "Orden:";
        // 
        // spnSortOrder
        // 
        spnSortOrder.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnSortOrder.Location = new Point(1025, 60);
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
        spnSortOrder.Size = new Size(143, 22);
        spnSortOrder.TabIndex = 3;
        // 
        // lblIsActive
        // 
        lblIsActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblIsActive.Appearance.ForeColor = BrandResources.Text;
        lblIsActive.Appearance.Options.UseFont = true;
        lblIsActive.Appearance.Options.UseForeColor = true;
        lblIsActive.Location = new Point(930, 91);
        lblIsActive.Name = "lblIsActive";
        lblIsActive.Size = new Size(37, 15);
        lblIsActive.TabIndex = 69;
        lblIsActive.Text = "Activo:";
        // 
        // chkIsActive
        // 
        chkIsActive.ActiveColor = BrandResources.Primary;
        chkIsActive.InactiveColor = BrandResources.Border;
        chkIsActive.Location = new Point(1060, 88);
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
        lblIsSystem.Location = new Point(930, 119);
        lblIsSystem.Name = "lblIsSystem";
        lblIsSystem.Size = new Size(102, 15);
        lblIsSystem.TabIndex = 70;
        lblIsSystem.Text = "Grupo del sistema:";
        // 
        // chkIsSystem
        // 
        chkIsSystem.ActiveColor = BrandResources.Primary;
        chkIsSystem.Enabled = false;
        chkIsSystem.InactiveColor = BrandResources.Border;
        chkIsSystem.Location = new Point(1060, 116);
        chkIsSystem.Name = "chkIsSystem";
        chkIsSystem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkIsSystem.Properties.Appearance.Options.UseFont = true;
        chkIsSystem.Properties.OffText = "No";
        chkIsSystem.Properties.OnText = "Sí";
        chkIsSystem.Size = new Size(70, 20);
        chkIsSystem.StateTextColor = BrandResources.Text;
        chkIsSystem.TabIndex = 5;
        chkIsSystem.TabStop = false;
        // 
        // lblAccountingTitle
        // 
        lblAccountingTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAccountingTitle.Appearance.ForeColor = BrandResources.Primary;
        lblAccountingTitle.Appearance.Options.UseFont = true;
        lblAccountingTitle.Appearance.Options.UseForeColor = true;
        lblAccountingTitle.Location = new Point(32, 212);
        lblAccountingTitle.Name = "lblAccountingTitle";
        lblAccountingTitle.Size = new Size(269, 20);
        lblAccountingTitle.TabIndex = 46;
        lblAccountingTitle.Text = "2. Configuración contable predeterminada";
        // 
        // lineAccountingTitle
        // 
        lineAccountingTitle.Appearance.BackColor = BrandResources.Border;
        lineAccountingTitle.Appearance.Options.UseBackColor = true;
        lineAccountingTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lineAccountingTitle.Location = new Point(385, 224);
        lineAccountingTitle.Name = "lineAccountingTitle";
        lineAccountingTitle.Size = new Size(783, 1);
        lineAccountingTitle.TabIndex = 47;
        // 
        // lineAccountingColumns
        // 
        lineAccountingColumns.Appearance.BackColor = BrandResources.Border;
        lineAccountingColumns.Appearance.Options.UseBackColor = true;
        lineAccountingColumns.AutoSizeMode = LabelAutoSizeMode.None;
        lineAccountingColumns.Location = new Point(590, 250);
        lineAccountingColumns.Name = "lineAccountingColumns";
        lineAccountingColumns.Size = new Size(1, 106);
        lineAccountingColumns.TabIndex = 48;
        // 
        // lblInventory
        // 
        lblInventory.Appearance.Font = new Font("Segoe UI", 9F);
        lblInventory.Appearance.ForeColor = BrandResources.Text;
        lblInventory.Appearance.Options.UseFont = true;
        lblInventory.Appearance.Options.UseForeColor = true;
        lblInventory.Location = new Point(32, 253);
        lblInventory.Name = "lblInventory";
        lblInventory.Size = new Size(115, 15);
        lblInventory.TabIndex = 49;
        lblInventory.Text = "Cuenta de inventario:";
        // 
        // lueInventory
        // 
        lueInventory.Location = new Point(215, 250);
        lueInventory.Name = "lueInventory";
        lueInventory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueInventory.Properties.Appearance.Options.UseFont = true;
        lueInventory.Properties.AutoHeight = false;
        lueInventory.Properties.Buttons.Clear();
        lueInventory.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        lueInventory.Properties.NullText = "";
        lueInventory.Size = new Size(340, 22);
        lueInventory.TabIndex = 6;
        // 
        // lblIncome
        // 
        lblIncome.Appearance.Font = new Font("Segoe UI", 9F);
        lblIncome.Appearance.ForeColor = BrandResources.Text;
        lblIncome.Appearance.Options.UseFont = true;
        lblIncome.Appearance.Options.UseForeColor = true;
        lblIncome.Location = new Point(32, 281);
        lblIncome.Name = "lblIncome";
        lblIncome.Size = new Size(107, 15);
        lblIncome.TabIndex = 50;
        lblIncome.Text = "Cuenta de ingresos:";
        // 
        // lueIncome
        // 
        lueIncome.Location = new Point(215, 278);
        lueIncome.Name = "lueIncome";
        lueIncome.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueIncome.Properties.Appearance.Options.UseFont = true;
        lueIncome.Properties.AutoHeight = false;
        lueIncome.Properties.Buttons.Clear();
        lueIncome.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        lueIncome.Properties.NullText = "";
        lueIncome.Size = new Size(340, 22);
        lueIncome.TabIndex = 7;
        // 
        // lblCostOfSales
        // 
        lblCostOfSales.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostOfSales.Appearance.ForeColor = BrandResources.Text;
        lblCostOfSales.Appearance.Options.UseFont = true;
        lblCostOfSales.Appearance.Options.UseForeColor = true;
        lblCostOfSales.Location = new Point(32, 309);
        lblCostOfSales.Name = "lblCostOfSales";
        lblCostOfSales.Size = new Size(141, 15);
        lblCostOfSales.TabIndex = 51;
        lblCostOfSales.Text = "Cuenta de costo de ventas:";
        // 
        // lueCostOfSales
        // 
        lueCostOfSales.Location = new Point(215, 306);
        lueCostOfSales.Name = "lueCostOfSales";
        lueCostOfSales.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCostOfSales.Properties.Appearance.Options.UseFont = true;
        lueCostOfSales.Properties.AutoHeight = false;
        lueCostOfSales.Properties.Buttons.Clear();
        lueCostOfSales.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        lueCostOfSales.Properties.NullText = "";
        lueCostOfSales.Size = new Size(340, 22);
        lueCostOfSales.TabIndex = 8;
        // 
        // lblSalesReturn
        // 
        lblSalesReturn.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesReturn.Appearance.ForeColor = BrandResources.Text;
        lblSalesReturn.Appearance.Options.UseFont = true;
        lblSalesReturn.Appearance.Options.UseForeColor = true;
        lblSalesReturn.Location = new Point(32, 337);
        lblSalesReturn.Name = "lblSalesReturn";
        lblSalesReturn.Size = new Size(178, 15);
        lblSalesReturn.TabIndex = 52;
        lblSalesReturn.Text = "Cuenta de devoluciones en ventas:";
        // 
        // lueSalesReturn
        // 
        lueSalesReturn.Location = new Point(215, 334);
        lueSalesReturn.Name = "lueSalesReturn";
        lueSalesReturn.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSalesReturn.Properties.Appearance.Options.UseFont = true;
        lueSalesReturn.Properties.AutoHeight = false;
        lueSalesReturn.Properties.Buttons.Clear();
        lueSalesReturn.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        lueSalesReturn.Properties.NullText = "";
        lueSalesReturn.Size = new Size(340, 22);
        lueSalesReturn.TabIndex = 9;
        // 
        // lblPurchaseExpense
        // 
        lblPurchaseExpense.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseExpense.Appearance.ForeColor = BrandResources.Text;
        lblPurchaseExpense.Appearance.Options.UseFont = true;
        lblPurchaseExpense.Appearance.Options.UseForeColor = true;
        lblPurchaseExpense.Location = new Point(620, 253);
        lblPurchaseExpense.Name = "lblPurchaseExpense";
        lblPurchaseExpense.Size = new Size(151, 15);
        lblPurchaseExpense.TabIndex = 53;
        lblPurchaseExpense.Text = "Cuenta de gastos de compra:";
        // 
        // luePurchaseExpense
        // 
        luePurchaseExpense.Location = new Point(825, 250);
        luePurchaseExpense.Name = "luePurchaseExpense";
        luePurchaseExpense.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseExpense.Properties.Appearance.Options.UseFont = true;
        luePurchaseExpense.Properties.AutoHeight = false;
        luePurchaseExpense.Properties.Buttons.Clear();
        luePurchaseExpense.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        luePurchaseExpense.Properties.NullText = "";
        luePurchaseExpense.Size = new Size(343, 22);
        luePurchaseExpense.TabIndex = 10;
        // 
        // lblPurchaseReturn
        // 
        lblPurchaseReturn.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseReturn.Appearance.ForeColor = BrandResources.Text;
        lblPurchaseReturn.Appearance.Options.UseFont = true;
        lblPurchaseReturn.Appearance.Options.UseForeColor = true;
        lblPurchaseReturn.Location = new Point(620, 281);
        lblPurchaseReturn.Name = "lblPurchaseReturn";
        lblPurchaseReturn.Size = new Size(190, 15);
        lblPurchaseReturn.TabIndex = 54;
        lblPurchaseReturn.Text = "Cuenta de devoluciones en compras:";
        // 
        // luePurchaseReturn
        // 
        luePurchaseReturn.Location = new Point(825, 278);
        luePurchaseReturn.Name = "luePurchaseReturn";
        luePurchaseReturn.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseReturn.Properties.Appearance.Options.UseFont = true;
        luePurchaseReturn.Properties.AutoHeight = false;
        luePurchaseReturn.Properties.Buttons.Clear();
        luePurchaseReturn.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        luePurchaseReturn.Properties.NullText = "";
        luePurchaseReturn.Size = new Size(343, 22);
        luePurchaseReturn.TabIndex = 11;
        // 
        // lblCostVariance
        // 
        lblCostVariance.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostVariance.Appearance.ForeColor = BrandResources.Text;
        lblCostVariance.Appearance.Options.UseFont = true;
        lblCostVariance.Appearance.Options.UseForeColor = true;
        lblCostVariance.Location = new Point(620, 309);
        lblCostVariance.Name = "lblCostVariance";
        lblCostVariance.Size = new Size(159, 15);
        lblCostVariance.TabIndex = 55;
        lblCostVariance.Text = "Cuenta de variación de costos:";
        // 
        // lueCostVariance
        // 
        lueCostVariance.Location = new Point(825, 306);
        lueCostVariance.Name = "lueCostVariance";
        lueCostVariance.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCostVariance.Properties.Appearance.Options.UseFont = true;
        lueCostVariance.Properties.AutoHeight = false;
        lueCostVariance.Properties.Buttons.Clear();
        lueCostVariance.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        lueCostVariance.Properties.NullText = "";
        lueCostVariance.Size = new Size(343, 22);
        lueCostVariance.TabIndex = 12;
        // 
        // lblInventoryAdjustment
        // 
        lblInventoryAdjustment.Appearance.Font = new Font("Segoe UI", 9F);
        lblInventoryAdjustment.Appearance.ForeColor = BrandResources.Text;
        lblInventoryAdjustment.Appearance.Options.UseFont = true;
        lblInventoryAdjustment.Appearance.Options.UseForeColor = true;
        lblInventoryAdjustment.Location = new Point(620, 337);
        lblInventoryAdjustment.Name = "lblInventoryAdjustment";
        lblInventoryAdjustment.Size = new Size(167, 15);
        lblInventoryAdjustment.TabIndex = 56;
        lblInventoryAdjustment.Text = "Cuenta de ajuste de inventario:";
        // 
        // lueInventoryAdjustment
        // 
        lueInventoryAdjustment.Location = new Point(825, 334);
        lueInventoryAdjustment.Name = "lueInventoryAdjustment";
        lueInventoryAdjustment.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueInventoryAdjustment.Properties.Appearance.Options.UseFont = true;
        lueInventoryAdjustment.Properties.AutoHeight = false;
        lueInventoryAdjustment.Properties.Buttons.Clear();
        lueInventoryAdjustment.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Delete), new EditorButton(ButtonPredefines.Plus), new EditorButton(ButtonPredefines.Ellipsis) });
        lueInventoryAdjustment.Properties.NullText = "";
        lueInventoryAdjustment.Size = new Size(343, 22);
        lueInventoryAdjustment.TabIndex = 13;
        // 
        // lblAccountingInfoIcon
        // 
        lblAccountingInfoIcon.Appearance.Font = new Font("Segoe UI", 12F);
        lblAccountingInfoIcon.Appearance.ForeColor = BrandResources.Primary;
        lblAccountingInfoIcon.Appearance.Options.UseFont = true;
        lblAccountingInfoIcon.Appearance.Options.UseForeColor = true;
        lblAccountingInfoIcon.Location = new Point(32, 370);
        lblAccountingInfoIcon.Name = "lblAccountingInfoIcon";
        lblAccountingInfoIcon.Size = new Size(16, 21);
        lblAccountingInfoIcon.TabIndex = 57;
        lblAccountingInfoIcon.Text = "ⓘ";
        // 
        // lblAccountingNote
        // 
        lblAccountingNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblAccountingNote.Appearance.ForeColor = BrandResources.MutedText;
        lblAccountingNote.Appearance.Options.UseFont = true;
        lblAccountingNote.Appearance.Options.UseForeColor = true;
        lblAccountingNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblAccountingNote.Location = new Point(56, 374);
        lblAccountingNote.Name = "lblAccountingNote";
        lblAccountingNote.Size = new Size(1112, 20);
        lblAccountingNote.TabIndex = 58;
        lblAccountingNote.Text = "Estas cuentas se aplican como valores predeterminados a los artículos del grupo.";
        // 
        // lblIntegrationTitle
        // 
        lblIntegrationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblIntegrationTitle.Appearance.ForeColor = BrandResources.Primary;
        lblIntegrationTitle.Appearance.Options.UseFont = true;
        lblIntegrationTitle.Appearance.Options.UseForeColor = true;
        lblIntegrationTitle.Location = new Point(32, 414);
        lblIntegrationTitle.Name = "lblIntegrationTitle";
        lblIntegrationTitle.Size = new Size(213, 20);
        lblIntegrationTitle.TabIndex = 59;
        lblIntegrationTitle.Text = "3. Integración externa (opcional)";
        // 
        // lineIntegrationTitle
        // 
        lineIntegrationTitle.Appearance.BackColor = BrandResources.Border;
        lineIntegrationTitle.Appearance.Options.UseBackColor = true;
        lineIntegrationTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lineIntegrationTitle.Location = new Point(330, 426);
        lineIntegrationTitle.Name = "lineIntegrationTitle";
        lineIntegrationTitle.Size = new Size(838, 1);
        lineIntegrationTitle.TabIndex = 60;
        // 
        // lblExternalSystem
        // 
        lblExternalSystem.Appearance.Font = new Font("Segoe UI", 9F);
        lblExternalSystem.Appearance.ForeColor = BrandResources.Text;
        lblExternalSystem.Appearance.Options.UseFont = true;
        lblExternalSystem.Appearance.Options.UseForeColor = true;
        lblExternalSystem.Location = new Point(32, 461);
        lblExternalSystem.Name = "lblExternalSystem";
        lblExternalSystem.Size = new Size(87, 15);
        lblExternalSystem.TabIndex = 61;
        lblExternalSystem.Text = "Sistema externo:";
        // 
        // txtExternalSystem
        // 
        txtExternalSystem.Location = new Point(150, 458);
        txtExternalSystem.Name = "txtExternalSystem";
        txtExternalSystem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtExternalSystem.Properties.Appearance.Options.UseFont = true;
        txtExternalSystem.Properties.AutoHeight = false;
        txtExternalSystem.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        txtExternalSystem.Properties.Items.AddRange(new object[] { "SAP_B1" });
        txtExternalSystem.Properties.MaxLength = 50;
        txtExternalSystem.Size = new Size(155, 22);
        txtExternalSystem.TabIndex = 14;
        // 
        // lblExternalCode
        // 
        lblExternalCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblExternalCode.Appearance.ForeColor = BrandResources.Text;
        lblExternalCode.Appearance.Options.UseFont = true;
        lblExternalCode.Appearance.Options.UseForeColor = true;
        lblExternalCode.Location = new Point(345, 461);
        lblExternalCode.Name = "lblExternalCode";
        lblExternalCode.Size = new Size(82, 15);
        lblExternalCode.TabIndex = 62;
        lblExternalCode.Text = "Código externo:";
        // 
        // txtExternalCode
        // 
        txtExternalCode.Location = new Point(455, 458);
        txtExternalCode.Name = "txtExternalCode";
        txtExternalCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtExternalCode.Properties.Appearance.Options.UseFont = true;
        txtExternalCode.Properties.AutoHeight = false;
        txtExternalCode.Properties.MaxLength = 100;
        txtExternalCode.Size = new Size(150, 22);
        txtExternalCode.TabIndex = 15;
        // 
        // lblSapGroupCode
        // 
        lblSapGroupCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapGroupCode.Appearance.ForeColor = BrandResources.Text;
        lblSapGroupCode.Appearance.Options.UseFont = true;
        lblSapGroupCode.Appearance.Options.UseForeColor = true;
        lblSapGroupCode.Location = new Point(650, 461);
        lblSapGroupCode.Name = "lblSapGroupCode";
        lblSapGroupCode.Size = new Size(128, 15);
        lblSapGroupCode.TabIndex = 63;
        lblSapGroupCode.Text = "Grupo SAP:";
        // 
        // txtSapGroupCode
        // 
        txtSapGroupCode.Location = new Point(730, 458);
        txtSapGroupCode.Name = "txtSapGroupCode";
        txtSapGroupCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapGroupCode.Properties.Appearance.Options.UseFont = true;
        txtSapGroupCode.Properties.AutoHeight = false;
        txtSapGroupCode.Properties.MaxLength = 100;
        txtSapGroupCode.Size = new Size(170, 22);
        txtSapGroupCode.TabIndex = 16;
        // 
        // lblSapCode
        // 
        lblSapCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapCode.Appearance.ForeColor = BrandResources.Text;
        lblSapCode.Appearance.Options.UseFont = true;
        lblSapCode.Appearance.Options.UseForeColor = true;
        lblSapCode.Location = new Point(920, 461);
        lblSapCode.Name = "lblSapCode";
        lblSapCode.Size = new Size(62, 15);
        lblSapCode.TabIndex = 64;
        lblSapCode.Text = "Código SAP:";
        // 
        // txtSapCode
        // 
        txtSapCode.Location = new Point(1010, 458);
        txtSapCode.Name = "txtSapCode";
        txtSapCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapCode.Properties.Appearance.Options.UseFont = true;
        txtSapCode.Properties.AutoHeight = false;
        txtSapCode.Properties.MaxLength = 50;
        txtSapCode.Size = new Size(158, 22);
        txtSapCode.TabIndex = 17;
        // 
        // lblIntegrationInfoIcon
        // 
        lblIntegrationInfoIcon.Appearance.Font = new Font("Segoe UI", 12F);
        lblIntegrationInfoIcon.Appearance.ForeColor = BrandResources.Primary;
        lblIntegrationInfoIcon.Appearance.Options.UseFont = true;
        lblIntegrationInfoIcon.Appearance.Options.UseForeColor = true;
        lblIntegrationInfoIcon.Location = new Point(32, 498);
        lblIntegrationInfoIcon.Name = "lblIntegrationInfoIcon";
        lblIntegrationInfoIcon.Size = new Size(16, 21);
        lblIntegrationInfoIcon.TabIndex = 66;
        lblIntegrationInfoIcon.Text = "ⓘ";
        // 
        // lblIntegrationNote
        // 
        lblIntegrationNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblIntegrationNote.Appearance.ForeColor = BrandResources.MutedText;
        lblIntegrationNote.Appearance.Options.UseFont = true;
        lblIntegrationNote.Appearance.Options.UseForeColor = true;
        lblIntegrationNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblIntegrationNote.Location = new Point(56, 502);
        lblIntegrationNote.Name = "lblIntegrationNote";
        lblIntegrationNote.Size = new Size(1112, 20);
        lblIntegrationNote.TabIndex = 67;
        lblIntegrationNote.Text = "La integración es opcional y no condiciona la operación local del ERP.";
        // 
        // lineFooter
        // 
        lineFooter.Appearance.BackColor = BrandResources.Border;
        lineFooter.Appearance.Options.UseBackColor = true;
        lineFooter.AutoSizeMode = LabelAutoSizeMode.None;
        lineFooter.Location = new Point(0, 540);
        lineFooter.Name = "lineFooter";
        lineFooter.Size = new Size(1200, 1);
        lineFooter.TabIndex = 68;
        // 
        // ItemGroupEditForm
        // 
        Appearance.BackColor = BrandResources.Background;
        Appearance.Options.UseBackColor = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 614);
        Controls.Add(lblGeneralTitle);
        Controls.Add(lineGeneralTitle);
        Controls.Add(lblCode);
        Controls.Add(txtCode);
        Controls.Add(lblName);
        Controls.Add(txtName);
        Controls.Add(lblDescription);
        Controls.Add(memDescription);
        Controls.Add(lblSortOrder);
        Controls.Add(spnSortOrder);
        Controls.Add(lblIsActive);
        Controls.Add(chkIsActive);
        Controls.Add(lblIsSystem);
        Controls.Add(chkIsSystem);
        Controls.Add(lblAccountingTitle);
        Controls.Add(lineAccountingTitle);
        Controls.Add(lineAccountingColumns);
        Controls.Add(lblInventory);
        Controls.Add(lueInventory);
        Controls.Add(lblIncome);
        Controls.Add(lueIncome);
        Controls.Add(lblCostOfSales);
        Controls.Add(lueCostOfSales);
        Controls.Add(lblSalesReturn);
        Controls.Add(lueSalesReturn);
        Controls.Add(lblPurchaseExpense);
        Controls.Add(luePurchaseExpense);
        Controls.Add(lblPurchaseReturn);
        Controls.Add(luePurchaseReturn);
        Controls.Add(lblCostVariance);
        Controls.Add(lueCostVariance);
        Controls.Add(lblInventoryAdjustment);
        Controls.Add(lueInventoryAdjustment);
        Controls.Add(lblAccountingInfoIcon);
        Controls.Add(lblAccountingNote);
        Controls.Add(lblIntegrationTitle);
        Controls.Add(lineIntegrationTitle);
        Controls.Add(lblExternalSystem);
        Controls.Add(txtExternalSystem);
        Controls.Add(lblExternalCode);
        Controls.Add(txtExternalCode);
        Controls.Add(lblSapGroupCode);
        Controls.Add(txtSapGroupCode);
        Controls.Add(lblSapCode);
        Controls.Add(txtSapCode);
        Controls.Add(lblIntegrationInfoIcon);
        Controls.Add(lblIntegrationNote);
        Controls.Add(lineFooter);
        MinimumSize = new Size(1216, 653);
        Name = "ItemGroupEditForm";
        Text = "Grupo de artículos";
        Controls.SetChildIndex(lineFooter, 0);
        Controls.SetChildIndex(lblIntegrationNote, 0);
        Controls.SetChildIndex(lblIntegrationInfoIcon, 0);
        Controls.SetChildIndex(txtSapCode, 0);
        Controls.SetChildIndex(lblSapCode, 0);
        Controls.SetChildIndex(txtSapGroupCode, 0);
        Controls.SetChildIndex(lblSapGroupCode, 0);
        Controls.SetChildIndex(txtExternalCode, 0);
        Controls.SetChildIndex(lblExternalCode, 0);
        Controls.SetChildIndex(txtExternalSystem, 0);
        Controls.SetChildIndex(lblExternalSystem, 0);
        Controls.SetChildIndex(lineIntegrationTitle, 0);
        Controls.SetChildIndex(lblIntegrationTitle, 0);
        Controls.SetChildIndex(lblAccountingNote, 0);
        Controls.SetChildIndex(lblAccountingInfoIcon, 0);
        Controls.SetChildIndex(lueInventoryAdjustment, 0);
        Controls.SetChildIndex(lblInventoryAdjustment, 0);
        Controls.SetChildIndex(lueCostVariance, 0);
        Controls.SetChildIndex(lblCostVariance, 0);
        Controls.SetChildIndex(luePurchaseReturn, 0);
        Controls.SetChildIndex(lblPurchaseReturn, 0);
        Controls.SetChildIndex(luePurchaseExpense, 0);
        Controls.SetChildIndex(lblPurchaseExpense, 0);
        Controls.SetChildIndex(lueSalesReturn, 0);
        Controls.SetChildIndex(lblSalesReturn, 0);
        Controls.SetChildIndex(lueCostOfSales, 0);
        Controls.SetChildIndex(lblCostOfSales, 0);
        Controls.SetChildIndex(lueIncome, 0);
        Controls.SetChildIndex(lblIncome, 0);
        Controls.SetChildIndex(lueInventory, 0);
        Controls.SetChildIndex(lblInventory, 0);
        Controls.SetChildIndex(lineAccountingColumns, 0);
        Controls.SetChildIndex(lineAccountingTitle, 0);
        Controls.SetChildIndex(lblAccountingTitle, 0);
        Controls.SetChildIndex(chkIsSystem, 0);
        Controls.SetChildIndex(lblIsSystem, 0);
        Controls.SetChildIndex(chkIsActive, 0);
        Controls.SetChildIndex(lblIsActive, 0);
        Controls.SetChildIndex(spnSortOrder, 0);
        Controls.SetChildIndex(lblSortOrder, 0);
        Controls.SetChildIndex(memDescription, 0);
        Controls.SetChildIndex(lblDescription, 0);
        Controls.SetChildIndex(txtName, 0);
        Controls.SetChildIndex(lblName, 0);
        Controls.SetChildIndex(txtCode, 0);
        Controls.SetChildIndex(lblCode, 0);
        Controls.SetChildIndex(lineGeneralTitle, 0);
        Controls.SetChildIndex(lblGeneralTitle, 0);
        Controls.SetChildIndex(btnGuardar, 0);
        Controls.SetChildIndex(btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)txtCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSortOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsSystem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkIsActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueInventory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueIncome.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCostOfSales.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesReturn.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseExpense.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseReturn.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCostVariance.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueInventoryAdjustment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalSystem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapGroupCode.Properties).EndInit();
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
    private LabelControl lblCode;
    private TextEdit txtCode;
    private LabelControl lblName;
    private TextEdit txtName;
    private LabelControl lblDescription;
    private MemoEdit memDescription;
    private LabelControl lblSortOrder;
    private SpinEdit spnSortOrder;
    private LabelControl lblIsActive;
    private NuanToggleSwitch chkIsActive;
    private LabelControl lblIsSystem;
    private NuanToggleSwitch chkIsSystem;
    private LabelControl lblAccountingTitle;
    private LabelControl lineAccountingTitle;
    private LabelControl lineAccountingColumns;
    private LabelControl lblInventory;
    private NuanLookupEdit lueInventory;
    private LabelControl lblIncome;
    private NuanLookupEdit lueIncome;
    private LabelControl lblCostOfSales;
    private NuanLookupEdit lueCostOfSales;
    private LabelControl lblSalesReturn;
    private NuanLookupEdit lueSalesReturn;
    private LabelControl lblPurchaseExpense;
    private NuanLookupEdit luePurchaseExpense;
    private LabelControl lblPurchaseReturn;
    private NuanLookupEdit luePurchaseReturn;
    private LabelControl lblCostVariance;
    private NuanLookupEdit lueCostVariance;
    private LabelControl lblInventoryAdjustment;
    private NuanLookupEdit lueInventoryAdjustment;
    private LabelControl lblAccountingInfoIcon;
    private LabelControl lblAccountingNote;
    private LabelControl lblIntegrationTitle;
    private LabelControl lineIntegrationTitle;
    private LabelControl lblExternalSystem;
    private ComboBoxEdit txtExternalSystem;
    private LabelControl lblExternalCode;
    private TextEdit txtExternalCode;
    private LabelControl lblSapGroupCode;
    private TextEdit txtSapGroupCode;
    private LabelControl lblSapCode;
    private TextEdit txtSapCode;
    private LabelControl lblIntegrationInfoIcon;
    private LabelControl lblIntegrationNote;
    private LabelControl lineFooter;
}

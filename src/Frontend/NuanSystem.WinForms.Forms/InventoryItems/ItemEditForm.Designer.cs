using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.InventoryItems;

partial class ItemEditForm
{
    private System.ComponentModel.IContainer components = null;

    private GroupControl grpHeader;
    private XtraTabControl tabMain;
    private XtraTabPage tabGeneral;
    private PanelControl pnlFooter;

    private LabelControl lblItemCode;
    private LabelControl lblBarCode;
    private LabelControl lblDescription;
    private LabelControl lblCommercialName;
    private LabelControl lblItemGroup;
    private LabelControl lblBrand;
    private LabelControl lblLine;
    private LabelControl lblUom;
    private LabelControl lblStatus;
    private LabelControl lblItemType;

    private TextEdit codeTextEdit;
    private TextEdit barcodeTextEdit;
    private TextEdit nameTextEdit;
    private TextEdit txtCommercialName;

    private SearchLookUpEdit itemGroupSearchLookUpEdit;
    private GridView itemGroupSearchLookUpView;
    private SearchLookUpEdit brandSearchLookUpEdit;
    private GridView brandSearchLookUpView;
    private SearchLookUpEdit lineSearchLookUpEdit;
    private GridView lineSearchLookUpView;
    private SearchLookUpEdit headerUomSearchLookUpEdit;
    private GridView headerUomSearchLookUpView;

    private LookUpEdit statusLookUpEdit;
    private ComboBoxEdit itemTypeComboBoxEdit;
    private CheckEdit inventoryCheckEdit;
    private CheckEdit purchaseCheckEdit;
    private CheckEdit salesCheckEdit;
    private CheckEdit serviceCheckEdit;
    private PictureEdit picItem;
    private SimpleButton btnChangeImage;
    private SimpleButton btnRemoveImage;

    private GroupControl grpGeneralData;
    private GroupControl grpGeneralUnits;
    private GroupControl grpGeneralNotes;
    private GroupControl grpGeneralAttributes;

    private LabelControl lblLongDescription;
    private LabelControl lblCategory;
    private LabelControl lblSubCategory;
    private LabelControl lblManufacturer;
    private LabelControl lblModel;
    private LabelControl lblCountry;
    private LabelControl lblAlternateCode;
    private LabelControl lblWeight;
    private LabelControl lblVolume;
    private LabelControl lblPurchaseUom;
    private LabelControl lblSalesUom;
    private LabelControl lblInventoryUom;
    private LabelControl lblGeneralNotes;

    private MemoEdit descriptionMemoEdit;
    private SearchLookUpEdit categorySearchLookUpEdit;
    private GridView categorySearchLookUpView;
    private SearchLookUpEdit subCategorySearchLookUpEdit;
    private GridView subCategorySearchLookUpView;
    private SearchLookUpEdit manufacturerSearchLookUpEdit;
    private GridView manufacturerSearchLookUpView;
    private TextEdit modelTextEdit;
    private LookUpEdit countryLookUpEdit;
    private TextEdit alternateCodeTextEdit;
    private SpinEdit weightSpinEdit;
    private SpinEdit volumeSpinEdit;
    private SearchLookUpEdit purchaseUomSearchLookUpEdit;
    private GridView purchaseUomSearchLookUpView;
    private SearchLookUpEdit salesUomSearchLookUpEdit;
    private GridView salesUomSearchLookUpView;
    private SearchLookUpEdit inventoryUomSearchLookUpEdit;
    private GridView inventoryUomSearchLookUpView;
    private MemoEdit remarksMemoEdit;
    private GridControl gridAttributes;
    private GridView viewAttributes;

    private LabelControl lblFooterMode;
    private LabelControl lblFooterRecord;
    private LabelControl lblFooterDatabase;
    private SimpleButton saveButton;
    private SimpleButton cancelButton;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.grpHeader = new GroupControl();
        this.lblItemCode = new LabelControl();
        this.lblBarCode = new LabelControl();
        this.lblDescription = new LabelControl();
        this.lblCommercialName = new LabelControl();
        this.lblItemGroup = new LabelControl();
        this.lblBrand = new LabelControl();
        this.lblLine = new LabelControl();
        this.lblUom = new LabelControl();
        this.lblStatus = new LabelControl();
        this.lblItemType = new LabelControl();
        this.codeTextEdit = new TextEdit();
        this.barcodeTextEdit = new TextEdit();
        this.nameTextEdit = new TextEdit();
        this.txtCommercialName = new TextEdit();
        this.itemGroupSearchLookUpEdit = new SearchLookUpEdit();
        this.itemGroupSearchLookUpView = new GridView();
        this.brandSearchLookUpEdit = new SearchLookUpEdit();
        this.brandSearchLookUpView = new GridView();
        this.lineSearchLookUpEdit = new SearchLookUpEdit();
        this.lineSearchLookUpView = new GridView();
        this.headerUomSearchLookUpEdit = new SearchLookUpEdit();
        this.headerUomSearchLookUpView = new GridView();
        this.statusLookUpEdit = new LookUpEdit();
        this.itemTypeComboBoxEdit = new ComboBoxEdit();
        this.inventoryCheckEdit = new CheckEdit();
        this.purchaseCheckEdit = new CheckEdit();
        this.salesCheckEdit = new CheckEdit();
        this.serviceCheckEdit = new CheckEdit();
        this.picItem = new PictureEdit();
        this.btnChangeImage = new SimpleButton();
        this.btnRemoveImage = new SimpleButton();
        this.tabMain = new XtraTabControl();
        this.tabGeneral = new XtraTabPage();
        this.grpGeneralData = new GroupControl();
        this.lblLongDescription = new LabelControl();
        this.descriptionMemoEdit = new MemoEdit();
        this.lblCategory = new LabelControl();
        this.categorySearchLookUpEdit = new SearchLookUpEdit();
        this.categorySearchLookUpView = new GridView();
        this.lblSubCategory = new LabelControl();
        this.subCategorySearchLookUpEdit = new SearchLookUpEdit();
        this.subCategorySearchLookUpView = new GridView();
        this.lblManufacturer = new LabelControl();
        this.manufacturerSearchLookUpEdit = new SearchLookUpEdit();
        this.manufacturerSearchLookUpView = new GridView();
        this.lblModel = new LabelControl();
        this.modelTextEdit = new TextEdit();
        this.lblCountry = new LabelControl();
        this.countryLookUpEdit = new LookUpEdit();
        this.lblAlternateCode = new LabelControl();
        this.alternateCodeTextEdit = new TextEdit();
        this.grpGeneralUnits = new GroupControl();
        this.lblWeight = new LabelControl();
        this.weightSpinEdit = new SpinEdit();
        this.lblVolume = new LabelControl();
        this.volumeSpinEdit = new SpinEdit();
        this.lblPurchaseUom = new LabelControl();
        this.purchaseUomSearchLookUpEdit = new SearchLookUpEdit();
        this.purchaseUomSearchLookUpView = new GridView();
        this.lblSalesUom = new LabelControl();
        this.salesUomSearchLookUpEdit = new SearchLookUpEdit();
        this.salesUomSearchLookUpView = new GridView();
        this.lblInventoryUom = new LabelControl();
        this.inventoryUomSearchLookUpEdit = new SearchLookUpEdit();
        this.inventoryUomSearchLookUpView = new GridView();
        this.grpGeneralNotes = new GroupControl();
        this.lblGeneralNotes = new LabelControl();
        this.remarksMemoEdit = new MemoEdit();
        this.grpGeneralAttributes = new GroupControl();
        this.gridAttributes = new GridControl();
        this.viewAttributes = new GridView();
        this.pnlFooter = new PanelControl();
        this.lblFooterMode = new LabelControl();
        this.lblFooterRecord = new LabelControl();
        this.lblFooterDatabase = new LabelControl();
        this.saveButton = new SimpleButton();
        this.cancelButton = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)(this.grpHeader)).BeginInit();
        this.grpHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.codeTextEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.barcodeTextEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.nameTextEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.txtCommercialName.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.itemGroupSearchLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.itemGroupSearchLookUpView)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.brandSearchLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.brandSearchLookUpView)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lineSearchLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.lineSearchLookUpView)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.headerUomSearchLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.headerUomSearchLookUpView)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.statusLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.itemTypeComboBoxEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.inventoryCheckEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.purchaseCheckEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.salesCheckEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.serviceCheckEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.picItem.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.tabMain)).BeginInit();
        this.tabMain.SuspendLayout();
        this.tabGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.grpGeneralData)).BeginInit();
        this.grpGeneralData.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.descriptionMemoEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.categorySearchLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.categorySearchLookUpView)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.subCategorySearchLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.subCategorySearchLookUpView)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.manufacturerSearchLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.manufacturerSearchLookUpView)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.modelTextEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.countryLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.alternateCodeTextEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.grpGeneralUnits)).BeginInit();
        this.grpGeneralUnits.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.weightSpinEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.volumeSpinEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.purchaseUomSearchLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.purchaseUomSearchLookUpView)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.salesUomSearchLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.salesUomSearchLookUpView)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.inventoryUomSearchLookUpEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.inventoryUomSearchLookUpView)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.grpGeneralNotes)).BeginInit();
        this.grpGeneralNotes.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.remarksMemoEdit.Properties)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.grpGeneralAttributes)).BeginInit();
        this.grpGeneralAttributes.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.gridAttributes)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.viewAttributes)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.pnlFooter)).BeginInit();
        this.pnlFooter.SuspendLayout();
        this.SuspendLayout();
        // 
        // grpHeader
        // 
        this.grpHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.grpHeader.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        this.grpHeader.AppearanceCaption.ForeColor = BrandResources.Primary;
        this.grpHeader.AppearanceCaption.Options.UseFont = true;
        this.grpHeader.AppearanceCaption.Options.UseForeColor = true;
        this.grpHeader.Controls.Add(this.lblItemCode);
        this.grpHeader.Controls.Add(this.codeTextEdit);
        this.grpHeader.Controls.Add(this.lblBarCode);
        this.grpHeader.Controls.Add(this.barcodeTextEdit);
        this.grpHeader.Controls.Add(this.lblStatus);
        this.grpHeader.Controls.Add(this.statusLookUpEdit);
        this.grpHeader.Controls.Add(this.lblItemType);
        this.grpHeader.Controls.Add(this.itemTypeComboBoxEdit);
        this.grpHeader.Controls.Add(this.lblDescription);
        this.grpHeader.Controls.Add(this.nameTextEdit);
        this.grpHeader.Controls.Add(this.lblCommercialName);
        this.grpHeader.Controls.Add(this.txtCommercialName);
        this.grpHeader.Controls.Add(this.lblItemGroup);
        this.grpHeader.Controls.Add(this.itemGroupSearchLookUpEdit);
        this.grpHeader.Controls.Add(this.lblBrand);
        this.grpHeader.Controls.Add(this.brandSearchLookUpEdit);
        this.grpHeader.Controls.Add(this.lblLine);
        this.grpHeader.Controls.Add(this.lineSearchLookUpEdit);
        this.grpHeader.Controls.Add(this.lblUom);
        this.grpHeader.Controls.Add(this.headerUomSearchLookUpEdit);
        this.grpHeader.Controls.Add(this.inventoryCheckEdit);
        this.grpHeader.Controls.Add(this.purchaseCheckEdit);
        this.grpHeader.Controls.Add(this.salesCheckEdit);
        this.grpHeader.Controls.Add(this.serviceCheckEdit);
        this.grpHeader.Controls.Add(this.picItem);
        this.grpHeader.Controls.Add(this.btnChangeImage);
        this.grpHeader.Controls.Add(this.btnRemoveImage);
        this.grpHeader.Location = new Point(0, 0);
        this.grpHeader.Name = "grpHeader";
        this.grpHeader.Size = new Size(1300, 225);
        this.grpHeader.TabIndex = 0;
        this.grpHeader.Text = "Datos principales del item";
        // 
        // header labels and editors
        // 
        ConfigureLabel(this.lblItemCode, "Codigo item:", 20, 39, 110, 20);
        ConfigureTextEdit(this.codeTextEdit, "codeTextEdit", "A000001", 135, 35, 150, 24);
        ConfigureLabel(this.lblBarCode, "Codigo barras:", 315, 39, 110, 20);
        ConfigureTextEdit(this.barcodeTextEdit, "barcodeTextEdit", "7861234567890", 430, 35, 220, 24);
        ConfigureLabel(this.lblStatus, "Estado:", 680, 39, 60, 20);
        ConfigureLookUp(this.statusLookUpEdit, "statusLookUpEdit", "Activo", 740, 35, 130, 24, "Activo", "Inactivo", "Bloqueado");
        ConfigureLabel(this.lblItemType, "Tipo:", 900, 39, 50, 20);
        ConfigureCombo(this.itemTypeComboBoxEdit, "itemTypeComboBoxEdit", "Product", 950, 35, 160, 24, "Product", "Service", "FixedAsset");
        ConfigureLabel(this.lblDescription, "Descripcion:", 20, 72, 110, 20);
        ConfigureTextEdit(this.nameTextEdit, "nameTextEdit", "ARROZ FLOR 2KG", 135, 68, 515, 24);
        ConfigureLabel(this.lblCommercialName, "Nombre comercial:", 680, 72, 130, 20);
        ConfigureTextEdit(this.txtCommercialName, "txtCommercialName", "ARROZ FLOR PREMIUM 2 KILOS", 815, 68, 295, 24);
        ConfigureLabel(this.lblItemGroup, "Grupo:", 20, 105, 110, 20);
        ConfigureSearchLookUp(this.itemGroupSearchLookUpEdit, this.itemGroupSearchLookUpView, "itemGroupSearchLookUpEdit", "01 - PRODUCTOS PRIMERA NECESIDAD", 135, 101, 360, 24);
        ConfigureLabel(this.lblBrand, "Marca:", 520, 105, 70, 20);
        ConfigureSearchLookUp(this.brandSearchLookUpEdit, this.brandSearchLookUpView, "brandSearchLookUpEdit", "FLOR", 590, 101, 240, 24);
        ConfigureLabel(this.lblLine, "Linea / Familia:", 20, 138, 110, 20);
        ConfigureSearchLookUp(this.lineSearchLookUpEdit, this.lineSearchLookUpView, "lineSearchLookUpEdit", "ALIMENTOS", 135, 134, 360, 24);
        ConfigureLabel(this.lblUom, "Unidad medida:", 520, 138, 100, 20);
        ConfigureSearchLookUp(this.headerUomSearchLookUpEdit, this.headerUomSearchLookUpView, "headerUomSearchLookUpEdit", "UNIDAD", 625, 134, 205, 24);
        ConfigureCheck(this.inventoryCheckEdit, "inventoryCheckEdit", "Inventariable", true, 135, 167, 120, 24);
        ConfigureCheck(this.purchaseCheckEdit, "purchaseCheckEdit", "Comprable", true, 265, 167, 110, 24);
        ConfigureCheck(this.salesCheckEdit, "salesCheckEdit", "Vendible", true, 385, 167, 100, 24);
        ConfigureCheck(this.serviceCheckEdit, "serviceCheckEdit", "Servicio", false, 495, 167, 100, 24);
        this.picItem.Location = new Point(1140, 35);
        this.picItem.Name = "picItem";
        this.picItem.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
        this.picItem.Properties.SizeMode = PictureSizeMode.Zoom;
        this.picItem.Size = new Size(130, 125);
        this.picItem.TabIndex = 24;
        this.btnChangeImage.Location = new Point(1140, 166);
        this.btnChangeImage.Name = "btnChangeImage";
        this.btnChangeImage.Size = new Size(90, 28);
        this.btnChangeImage.TabIndex = 25;
        this.btnChangeImage.Text = "Cambiar imagen";
        this.btnRemoveImage.Location = new Point(1235, 166);
        this.btnRemoveImage.Name = "btnRemoveImage";
        this.btnRemoveImage.Size = new Size(35, 28);
        this.btnRemoveImage.TabIndex = 26;
        this.btnRemoveImage.Text = "Quitar";
        // 
        // tabMain
        // 
        this.tabMain.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.tabMain.Location = new Point(0, 225);
        this.tabMain.Name = "tabMain";
        this.tabMain.SelectedTabPage = this.tabGeneral;
        this.tabMain.Size = new Size(1300, 493);
        this.tabMain.TabIndex = 1;
        this.tabMain.TabPages.AddRange(new XtraTabPage[] { this.tabGeneral });
        // 
        // tabGeneral
        // 
        this.tabGeneral.Controls.Add(this.grpGeneralData);
        this.tabGeneral.Controls.Add(this.grpGeneralUnits);
        this.tabGeneral.Controls.Add(this.grpGeneralNotes);
        this.tabGeneral.Controls.Add(this.grpGeneralAttributes);
        this.tabGeneral.Name = "tabGeneral";
        this.tabGeneral.Size = new Size(1298, 458);
        this.tabGeneral.Text = "General";
        // 
        // grpGeneralData
        // 
        ConfigureGroup(this.grpGeneralData, "Datos generales", 10, 10, 570, 245);
        this.grpGeneralData.Controls.Add(this.lblLongDescription);
        this.grpGeneralData.Controls.Add(this.descriptionMemoEdit);
        this.grpGeneralData.Controls.Add(this.lblCategory);
        this.grpGeneralData.Controls.Add(this.categorySearchLookUpEdit);
        this.grpGeneralData.Controls.Add(this.lblSubCategory);
        this.grpGeneralData.Controls.Add(this.subCategorySearchLookUpEdit);
        this.grpGeneralData.Controls.Add(this.lblManufacturer);
        this.grpGeneralData.Controls.Add(this.manufacturerSearchLookUpEdit);
        this.grpGeneralData.Controls.Add(this.lblModel);
        this.grpGeneralData.Controls.Add(this.modelTextEdit);
        this.grpGeneralData.Controls.Add(this.lblCountry);
        this.grpGeneralData.Controls.Add(this.countryLookUpEdit);
        this.grpGeneralData.Controls.Add(this.lblAlternateCode);
        this.grpGeneralData.Controls.Add(this.alternateCodeTextEdit);
        ConfigureLabel(this.lblLongDescription, "Descripcion larga:", 15, 38, 130, 20);
        this.descriptionMemoEdit.Location = new Point(150, 35);
        this.descriptionMemoEdit.Name = "descriptionMemoEdit";
        this.descriptionMemoEdit.Size = new Size(390, 55);
        this.descriptionMemoEdit.TabIndex = 0;
        this.descriptionMemoEdit.Text = "Arroz blanco de grano largo, seleccionado y empacado bajo estandares de calidad premium.";
        ConfigureLabel(this.lblCategory, "Categoria:", 15, 103, 130, 20);
        ConfigureSearchLookUp(this.categorySearchLookUpEdit, this.categorySearchLookUpView, "categorySearchLookUpEdit", "ALIMENTOS", 150, 100, 250, 24);
        ConfigureLabel(this.lblSubCategory, "Subcategoria:", 15, 133, 130, 20);
        ConfigureSearchLookUp(this.subCategorySearchLookUpEdit, this.subCategorySearchLookUpView, "subCategorySearchLookUpEdit", "GRANOS", 150, 130, 250, 24);
        ConfigureLabel(this.lblManufacturer, "Fabricante:", 15, 163, 130, 20);
        ConfigureSearchLookUp(this.manufacturerSearchLookUpEdit, this.manufacturerSearchLookUpView, "manufacturerSearchLookUpEdit", "INDUSTRIAL XYZ S.A.", 150, 160, 250, 24);
        ConfigureLabel(this.lblModel, "Modelo:", 15, 193, 130, 20);
        ConfigureTextEdit(this.modelTextEdit, "modelTextEdit", "ESTANDAR", 150, 190, 250, 24);
        ConfigureLabel(this.lblCountry, "Pais origen:", 410, 103, 90, 20);
        ConfigureLookUp(this.countryLookUpEdit, "countryLookUpEdit", "ECUADOR", 500, 100, 50, 24, "ECUADOR", "COLOMBIA", "PERU");
        ConfigureLabel(this.lblAlternateCode, "Codigo alterno:", 410, 133, 90, 20);
        ConfigureTextEdit(this.alternateCodeTextEdit, "alternateCodeTextEdit", "ARZ-FLOR-2KG", 500, 130, 50, 24);
        // 
        // grpGeneralUnits
        // 
        ConfigureGroup(this.grpGeneralUnits, "Unidades y medidas", 590, 10, 340, 245);
        this.grpGeneralUnits.Controls.Add(this.lblWeight);
        this.grpGeneralUnits.Controls.Add(this.weightSpinEdit);
        this.grpGeneralUnits.Controls.Add(this.lblVolume);
        this.grpGeneralUnits.Controls.Add(this.volumeSpinEdit);
        this.grpGeneralUnits.Controls.Add(this.lblPurchaseUom);
        this.grpGeneralUnits.Controls.Add(this.purchaseUomSearchLookUpEdit);
        this.grpGeneralUnits.Controls.Add(this.lblSalesUom);
        this.grpGeneralUnits.Controls.Add(this.salesUomSearchLookUpEdit);
        this.grpGeneralUnits.Controls.Add(this.lblInventoryUom);
        this.grpGeneralUnits.Controls.Add(this.inventoryUomSearchLookUpEdit);
        ConfigureLabel(this.lblWeight, "Peso:", 15, 45, 120, 20);
        ConfigureSpin(this.weightSpinEdit, "weightSpinEdit", 2.00m, 145, 42, 160, 24);
        ConfigureLabel(this.lblVolume, "Volumen:", 15, 78, 120, 20);
        ConfigureSpin(this.volumeSpinEdit, "volumeSpinEdit", 0.004m, 145, 75, 160, 24);
        ConfigureLabel(this.lblPurchaseUom, "Unidad compra:", 15, 111, 120, 20);
        ConfigureSearchLookUp(this.purchaseUomSearchLookUpEdit, this.purchaseUomSearchLookUpView, "purchaseUomSearchLookUpEdit", "UNIDAD", 145, 108, 160, 24);
        ConfigureLabel(this.lblSalesUom, "Unidad venta:", 15, 144, 120, 20);
        ConfigureSearchLookUp(this.salesUomSearchLookUpEdit, this.salesUomSearchLookUpView, "salesUomSearchLookUpEdit", "UNIDAD", 145, 141, 160, 24);
        ConfigureLabel(this.lblInventoryUom, "Unidad inventario:", 15, 177, 120, 20);
        ConfigureSearchLookUp(this.inventoryUomSearchLookUpEdit, this.inventoryUomSearchLookUpView, "inventoryUomSearchLookUpEdit", "UNIDAD", 145, 174, 160, 24);
        // 
        // grpGeneralNotes
        // 
        ConfigureGroup(this.grpGeneralNotes, "Observaciones", 940, 10, 330, 245);
        this.grpGeneralNotes.Controls.Add(this.lblGeneralNotes);
        this.grpGeneralNotes.Controls.Add(this.remarksMemoEdit);
        ConfigureLabel(this.lblGeneralNotes, "Observaciones:", 15, 35, 120, 20);
        this.remarksMemoEdit.Location = new Point(15, 60);
        this.remarksMemoEdit.Name = "remarksMemoEdit";
        this.remarksMemoEdit.Size = new Size(300, 160);
        this.remarksMemoEdit.TabIndex = 1;
        this.remarksMemoEdit.Text = "Conservar en lugar fresco y seco. Producto controlado para abastecimiento, venta e inventario.";
        // 
        // grpGeneralAttributes
        // 
        ConfigureGroup(this.grpGeneralAttributes, "Atributos adicionales", 10, 265, 1260, 180);
        this.grpGeneralAttributes.Controls.Add(this.gridAttributes);
        this.gridAttributes.Dock = DockStyle.Fill;
        this.gridAttributes.MainView = this.viewAttributes;
        this.gridAttributes.Name = "gridAttributes";
        this.gridAttributes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { this.viewAttributes });
        this.viewAttributes.GridControl = this.gridAttributes;
        this.viewAttributes.OptionsBehavior.Editable = false;
        this.viewAttributes.OptionsView.ColumnAutoWidth = false;
        this.viewAttributes.OptionsView.ShowAutoFilterRow = true;
        this.viewAttributes.OptionsView.ShowGroupPanel = false;
        this.viewAttributes.Columns.AddVisible("Atributo", "Atributo").Width = 140;
        this.viewAttributes.Columns.AddVisible("Valor", "Valor").Width = 140;
        this.viewAttributes.Columns.AddVisible("Requerido", "Requerido").Width = 140;
        this.viewAttributes.Columns.AddVisible("Activo", "Activo").Width = 140;
        this.tabGeneral.Controls.Add(this.lblLongDescription);
        this.tabGeneral.Controls.Add(this.descriptionMemoEdit);
        this.tabGeneral.Controls.Add(this.lblCategory);
        this.tabGeneral.Controls.Add(this.categorySearchLookUpEdit);
        this.tabGeneral.Controls.Add(this.lblSubCategory);
        this.tabGeneral.Controls.Add(this.subCategorySearchLookUpEdit);
        this.tabGeneral.Controls.Add(this.lblManufacturer);
        this.tabGeneral.Controls.Add(this.manufacturerSearchLookUpEdit);
        this.tabGeneral.Controls.Add(this.lblModel);
        this.tabGeneral.Controls.Add(this.modelTextEdit);
        this.tabGeneral.Controls.Add(this.lblCountry);
        this.tabGeneral.Controls.Add(this.countryLookUpEdit);
        this.tabGeneral.Controls.Add(this.lblAlternateCode);
        this.tabGeneral.Controls.Add(this.alternateCodeTextEdit);
        this.tabGeneral.Controls.Add(this.lblWeight);
        this.tabGeneral.Controls.Add(this.weightSpinEdit);
        this.tabGeneral.Controls.Add(this.lblVolume);
        this.tabGeneral.Controls.Add(this.volumeSpinEdit);
        this.tabGeneral.Controls.Add(this.lblPurchaseUom);
        this.tabGeneral.Controls.Add(this.purchaseUomSearchLookUpEdit);
        this.tabGeneral.Controls.Add(this.lblSalesUom);
        this.tabGeneral.Controls.Add(this.salesUomSearchLookUpEdit);
        this.tabGeneral.Controls.Add(this.lblInventoryUom);
        this.tabGeneral.Controls.Add(this.inventoryUomSearchLookUpEdit);
        this.tabGeneral.Controls.Add(this.lblGeneralNotes);
        this.tabGeneral.Controls.Add(this.remarksMemoEdit);
        this.lblLongDescription.Location = new Point(25, 48);
        this.descriptionMemoEdit.Location = new Point(160, 45);
        this.lblCategory.Location = new Point(25, 113);
        this.categorySearchLookUpEdit.Location = new Point(160, 110);
        this.lblSubCategory.Location = new Point(25, 143);
        this.subCategorySearchLookUpEdit.Location = new Point(160, 140);
        this.lblManufacturer.Location = new Point(25, 173);
        this.manufacturerSearchLookUpEdit.Location = new Point(160, 170);
        this.lblModel.Location = new Point(25, 203);
        this.modelTextEdit.Location = new Point(160, 200);
        this.lblCountry.Location = new Point(420, 113);
        this.countryLookUpEdit.Location = new Point(510, 110);
        this.lblAlternateCode.Location = new Point(420, 143);
        this.alternateCodeTextEdit.Location = new Point(510, 140);
        this.lblWeight.Location = new Point(605, 55);
        this.weightSpinEdit.Location = new Point(735, 52);
        this.lblVolume.Location = new Point(605, 88);
        this.volumeSpinEdit.Location = new Point(735, 85);
        this.lblPurchaseUom.Location = new Point(605, 121);
        this.purchaseUomSearchLookUpEdit.Location = new Point(735, 118);
        this.lblSalesUom.Location = new Point(605, 154);
        this.salesUomSearchLookUpEdit.Location = new Point(735, 151);
        this.lblInventoryUom.Location = new Point(605, 187);
        this.inventoryUomSearchLookUpEdit.Location = new Point(735, 184);
        this.lblGeneralNotes.Location = new Point(955, 45);
        this.remarksMemoEdit.Location = new Point(955, 70);
        this.grpGeneralData.SendToBack();
        this.grpGeneralUnits.SendToBack();
        this.grpGeneralNotes.SendToBack();
        this.grpGeneralAttributes.SendToBack();
        this.lblLongDescription.BringToFront();
        this.descriptionMemoEdit.BringToFront();
        this.lblCategory.BringToFront();
        this.categorySearchLookUpEdit.BringToFront();
        this.lblSubCategory.BringToFront();
        this.subCategorySearchLookUpEdit.BringToFront();
        this.lblManufacturer.BringToFront();
        this.manufacturerSearchLookUpEdit.BringToFront();
        this.lblModel.BringToFront();
        this.modelTextEdit.BringToFront();
        this.lblCountry.BringToFront();
        this.countryLookUpEdit.BringToFront();
        this.lblAlternateCode.BringToFront();
        this.alternateCodeTextEdit.BringToFront();
        this.lblWeight.BringToFront();
        this.weightSpinEdit.BringToFront();
        this.lblVolume.BringToFront();
        this.volumeSpinEdit.BringToFront();
        this.lblPurchaseUom.BringToFront();
        this.purchaseUomSearchLookUpEdit.BringToFront();
        this.lblSalesUom.BringToFront();
        this.salesUomSearchLookUpEdit.BringToFront();
        this.lblInventoryUom.BringToFront();
        this.inventoryUomSearchLookUpEdit.BringToFront();
        this.lblGeneralNotes.BringToFront();
        this.remarksMemoEdit.BringToFront();
        // 
        // pnlFooter
        // 
        this.pnlFooter.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.pnlFooter.Controls.Add(this.lblFooterMode);
        this.pnlFooter.Controls.Add(this.lblFooterRecord);
        this.pnlFooter.Controls.Add(this.lblFooterDatabase);
        this.pnlFooter.Controls.Add(this.saveButton);
        this.pnlFooter.Controls.Add(this.cancelButton);
        this.pnlFooter.Location = new Point(0, 718);
        this.pnlFooter.Name = "pnlFooter";
        this.pnlFooter.Size = new Size(1300, 42);
        this.pnlFooter.TabIndex = 2;
        ConfigureLabel(this.lblFooterMode, "Modo: Edicion", 20, 12, 130, 20);
        ConfigureLabel(this.lblFooterRecord, "Registro: Nuevo", 180, 12, 150, 20);
        ConfigureLabel(this.lblFooterDatabase, "Base de Datos: NuanSystemDB", 740, 12, 220, 20);
        this.saveButton.Appearance.BackColor = BrandResources.Primary;
        this.saveButton.Appearance.ForeColor = Color.White;
        this.saveButton.Appearance.Options.UseBackColor = true;
        this.saveButton.Appearance.Options.UseForeColor = true;
        this.saveButton.Location = new Point(1030, 7);
        this.saveButton.Name = "saveButton";
        this.saveButton.Size = new Size(100, 28);
        this.saveButton.TabIndex = 3;
        this.saveButton.Text = "Guardar";
        this.saveButton.Click += this.SaveButtonClick;
        this.cancelButton.DialogResult = DialogResult.Cancel;
        this.cancelButton.Location = new Point(1140, 7);
        this.cancelButton.Name = "cancelButton";
        this.cancelButton.Size = new Size(100, 28);
        this.cancelButton.TabIndex = 4;
        this.cancelButton.Text = "Cancelar";
        // 
        // ItemEditForm
        // 
        this.Appearance.BackColor = BrandResources.Background;
        this.Appearance.Options.UseBackColor = true;
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1300, 760);
        this.Controls.Add(this.grpHeader);
        this.Controls.Add(this.tabMain);
        this.Controls.Add(this.pnlFooter);
        this.Controls.Add(this.lblItemCode);
        this.Controls.Add(this.codeTextEdit);
        this.Controls.Add(this.lblBarCode);
        this.Controls.Add(this.barcodeTextEdit);
        this.Controls.Add(this.lblStatus);
        this.Controls.Add(this.statusLookUpEdit);
        this.Controls.Add(this.lblItemType);
        this.Controls.Add(this.itemTypeComboBoxEdit);
        this.Controls.Add(this.lblDescription);
        this.Controls.Add(this.nameTextEdit);
        this.Controls.Add(this.lblCommercialName);
        this.Controls.Add(this.txtCommercialName);
        this.Controls.Add(this.lblItemGroup);
        this.Controls.Add(this.itemGroupSearchLookUpEdit);
        this.Controls.Add(this.lblBrand);
        this.Controls.Add(this.brandSearchLookUpEdit);
        this.Controls.Add(this.lblLine);
        this.Controls.Add(this.lineSearchLookUpEdit);
        this.Controls.Add(this.lblUom);
        this.Controls.Add(this.headerUomSearchLookUpEdit);
        this.Controls.Add(this.inventoryCheckEdit);
        this.Controls.Add(this.purchaseCheckEdit);
        this.Controls.Add(this.salesCheckEdit);
        this.Controls.Add(this.serviceCheckEdit);
        this.Controls.Add(this.picItem);
        this.Controls.Add(this.btnChangeImage);
        this.Controls.Add(this.btnRemoveImage);
        this.grpHeader.SendToBack();
        this.tabMain.SendToBack();
        this.pnlFooter.BringToFront();
        this.lblItemCode.BringToFront();
        this.codeTextEdit.BringToFront();
        this.lblBarCode.BringToFront();
        this.barcodeTextEdit.BringToFront();
        this.lblStatus.BringToFront();
        this.statusLookUpEdit.BringToFront();
        this.lblItemType.BringToFront();
        this.itemTypeComboBoxEdit.BringToFront();
        this.lblDescription.BringToFront();
        this.nameTextEdit.BringToFront();
        this.lblCommercialName.BringToFront();
        this.txtCommercialName.BringToFront();
        this.lblItemGroup.BringToFront();
        this.itemGroupSearchLookUpEdit.BringToFront();
        this.lblBrand.BringToFront();
        this.brandSearchLookUpEdit.BringToFront();
        this.lblLine.BringToFront();
        this.lineSearchLookUpEdit.BringToFront();
        this.lblUom.BringToFront();
        this.headerUomSearchLookUpEdit.BringToFront();
        this.inventoryCheckEdit.BringToFront();
        this.purchaseCheckEdit.BringToFront();
        this.salesCheckEdit.BringToFront();
        this.serviceCheckEdit.BringToFront();
        this.picItem.BringToFront();
        this.btnChangeImage.BringToFront();
        this.btnRemoveImage.BringToFront();
        this.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        this.MinimumSize = new Size(1180, 720);
        this.Name = "ItemEditForm";
        this.StartPosition = FormStartPosition.CenterParent;
        this.Text = "Maestro de items";
        this.AcceptButton = this.saveButton;
        this.CancelButton = this.cancelButton;
        ((System.ComponentModel.ISupportInitialize)(this.grpHeader)).EndInit();
        this.grpHeader.ResumeLayout(false);
        this.grpHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.codeTextEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.barcodeTextEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.nameTextEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.txtCommercialName.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.itemGroupSearchLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.itemGroupSearchLookUpView)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.brandSearchLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.brandSearchLookUpView)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lineSearchLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.lineSearchLookUpView)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.headerUomSearchLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.headerUomSearchLookUpView)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.statusLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.itemTypeComboBoxEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.inventoryCheckEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.purchaseCheckEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.salesCheckEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.serviceCheckEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.picItem.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.tabMain)).EndInit();
        this.tabMain.ResumeLayout(false);
        this.tabGeneral.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.grpGeneralData)).EndInit();
        this.grpGeneralData.ResumeLayout(false);
        this.grpGeneralData.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.descriptionMemoEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.categorySearchLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.categorySearchLookUpView)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.subCategorySearchLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.subCategorySearchLookUpView)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.manufacturerSearchLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.manufacturerSearchLookUpView)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.modelTextEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.countryLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.alternateCodeTextEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.grpGeneralUnits)).EndInit();
        this.grpGeneralUnits.ResumeLayout(false);
        this.grpGeneralUnits.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.weightSpinEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.volumeSpinEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.purchaseUomSearchLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.purchaseUomSearchLookUpView)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.salesUomSearchLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.salesUomSearchLookUpView)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.inventoryUomSearchLookUpEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.inventoryUomSearchLookUpView)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.grpGeneralNotes)).EndInit();
        this.grpGeneralNotes.ResumeLayout(false);
        this.grpGeneralNotes.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.remarksMemoEdit.Properties)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.grpGeneralAttributes)).EndInit();
        this.grpGeneralAttributes.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.gridAttributes)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.viewAttributes)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.pnlFooter)).EndInit();
        this.pnlFooter.ResumeLayout(false);
        this.pnlFooter.PerformLayout();
        this.ResumeLayout(false);
    }

    private static void ConfigureLabel(LabelControl label, string text, int x, int y, int width, int height)
    {
        label.Appearance.Font = FormStyler.LabelFont;
        label.Appearance.ForeColor = BrandResources.Text;
        label.Appearance.Options.UseFont = true;
        label.Appearance.Options.UseForeColor = true;
        label.Location = new Point(x, y);
        label.Name = label.Name.Length == 0 ? text.Replace(":", string.Empty).Replace(" ", string.Empty) : label.Name;
        label.Size = new Size(width, height);
        label.Text = text;
    }

    private static void ConfigureTextEdit(TextEdit edit, string name, string text, int x, int y, int width, int height)
    {
        edit.EditValue = text;
        edit.EnterMoveNextControl = true;
        edit.Location = new Point(x, y);
        edit.Name = name;
        edit.Properties.Appearance.Font = FormStyler.LabelFont;
        edit.Properties.Appearance.Options.UseFont = true;
        edit.Properties.AutoHeight = false;
        edit.Properties.BorderStyle = BorderStyles.Flat;
        edit.Size = new Size(width, height);
    }

    private static void ConfigureSearchLookUp(SearchLookUpEdit edit, GridView view, string name, string text, int x, int y, int width, int height)
    {
        edit.EditValue = text;
        edit.Location = new Point(x, y);
        edit.Name = name;
        edit.Properties.Appearance.Font = FormStyler.LabelFont;
        edit.Properties.Appearance.Options.UseFont = true;
        edit.Properties.AutoHeight = false;
        edit.Properties.BorderStyle = BorderStyles.Flat;
        edit.Properties.NullText = string.Empty;
        edit.Properties.PopupView = view;
        edit.Properties.DisplayMember = "DisplayText";
        edit.Properties.ValueMember = "Id";
        edit.Size = new Size(width, height);
        view.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        view.OptionsSelection.EnableAppearanceFocusedCell = false;
        view.OptionsView.ShowGroupPanel = false;
    }

    private static void ConfigureLookUp(LookUpEdit edit, string name, string value, int x, int y, int width, int height, params string[] items)
    {
        edit.EditValue = value;
        edit.Location = new Point(x, y);
        edit.Name = name;
        edit.Properties.Appearance.Font = FormStyler.LabelFont;
        edit.Properties.Appearance.Options.UseFont = true;
        edit.Properties.AutoHeight = false;
        edit.Properties.BorderStyle = BorderStyles.Flat;
        edit.Properties.DataSource = items;
        edit.Properties.NullText = string.Empty;
        edit.Properties.TextEditStyle = TextEditStyles.Standard;
        edit.Size = new Size(width, height);
    }

    private static void ConfigureCombo(ComboBoxEdit edit, string name, string value, int x, int y, int width, int height, params string[] items)
    {
        edit.EditValue = value;
        edit.Location = new Point(x, y);
        edit.Name = name;
        edit.Properties.Appearance.Font = FormStyler.LabelFont;
        edit.Properties.Appearance.Options.UseFont = true;
        edit.Properties.AutoHeight = false;
        edit.Properties.BorderStyle = BorderStyles.Flat;
        edit.Properties.Items.AddRange(items);
        edit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
        edit.Size = new Size(width, height);
    }

    private static void ConfigureCheck(CheckEdit edit, string name, string text, bool isChecked, int x, int y, int width, int height)
    {
        edit.EditValue = isChecked;
        edit.Location = new Point(x, y);
        edit.Name = name;
        edit.Properties.Caption = text;
        edit.Size = new Size(width, height);
    }

    private static void ConfigureSpin(SpinEdit edit, string name, decimal value, int x, int y, int width, int height)
    {
        edit.EditValue = value;
        edit.Location = new Point(x, y);
        edit.Name = name;
        edit.Properties.Appearance.Font = FormStyler.LabelFont;
        edit.Properties.Appearance.Options.UseFont = true;
        edit.Properties.AutoHeight = false;
        edit.Properties.BorderStyle = BorderStyles.Flat;
        edit.Properties.IsFloatValue = true;
        edit.Properties.MaskSettings.Set("mask", "n2");
        edit.Properties.MaxValue = 999999999;
        edit.Properties.MinValue = 0;
        edit.Size = new Size(width, height);
    }

    private static void ConfigureGroup(GroupControl group, string text, int x, int y, int width, int height)
    {
        group.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        group.AppearanceCaption.ForeColor = BrandResources.Primary;
        group.AppearanceCaption.Options.UseFont = true;
        group.AppearanceCaption.Options.UseForeColor = true;
        group.Location = new Point(x, y);
        group.Name = text.Replace(" ", string.Empty);
        group.Size = new Size(width, height);
        group.Text = text;
    }

    private void SaveButtonClick(object sender, EventArgs e)
    {
        Save();
    }
}

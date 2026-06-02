using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;

namespace NuanSystem.WinForms.Forms.InventoryItems;

partial class ItemEditForm
{
    private System.ComponentModel.IContainer components = null;
    private PanelControl pnlHeader;
    private PanelControl pnlFooter;
    private PictureEdit picItem;
    private LabelControl lblItemCode;
    private LabelControl lblDescription;
    private LabelControl lblCommercialName;
    private LabelControl lblItemType;
    private LabelControl lblItemGroup;
    private LabelControl lblItemFamily;
    private LabelControl lblBrand;
    private LabelControl lblBaseUnit;
    private LabelControl lblStatusCaption;
    private TextEdit txtItemCode;
    private TextEdit txtDescription;
    private TextEdit txtCommercialName;
    private LookUpEdit lueItemType;
    private LookUpEdit lueItemGroup;
    private LookUpEdit lueItemFamily;
    private LookUpEdit lueBrand;
    private LookUpEdit lueBaseUnit;
    private LabelControl lblStatus;
    private SimpleButton btnSave;
    private SimpleButton btnCancel;

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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemEditForm));
        pnlHeader = new PanelControl();
        lblStockTotalCaption = new LabelControl();
        lblStockTotal = new LabelControl();
        lblAverageCostCaption = new LabelControl();
        lblAverageCost = new LabelControl();
        lblSalesPriceCaption = new LabelControl();
        lblSalesPrice = new LabelControl();
        lblLastPurchaseCaption = new LabelControl();
        lblLastPurchase = new LabelControl();
        lblSapSyncedCaption = new LabelControl();
        lblSapSynced = new LabelControl();
        picItem = new PictureEdit();
        lblItemCode = new LabelControl();
        txtItemCode = new TextEdit();
        lblDescription = new LabelControl();
        txtDescription = new TextEdit();
        lblCommercialName = new LabelControl();
        txtCommercialName = new TextEdit();
        lblItemType = new LabelControl();
        lueItemType = new LookUpEdit();
        lblItemGroup = new LabelControl();
        lueItemGroup = new LookUpEdit();
        lblItemFamily = new LabelControl();
        lueItemFamily = new LookUpEdit();
        lblBrand = new LabelControl();
        lueBrand = new LookUpEdit();
        lblBaseUnit = new LabelControl();
        lueBaseUnit = new LookUpEdit();
        lblStatusCaption = new LabelControl();
        lblStatus = new LabelControl();
        pnlFooter = new PanelControl();
        btnSave = new SimpleButton();
        btnCancel = new SimpleButton();
        purchasePresentationsTable = new System.Data.DataTable();
        warehouseStockTable = new System.Data.DataTable();
        purchaseHistoryTable = new System.Data.DataTable();
        salesPriceListsTable = new System.Data.DataTable();
        costComponentsTable = new System.Data.DataTable();
        costPriceHistoryTable = new System.Data.DataTable();
        taxMatrixTable = new System.Data.DataTable();
        recentLotsTable = new System.Data.DataTable();
        variantAttributesTable = new System.Data.DataTable();
        registeredVariantsTable = new System.Data.DataTable();
        sapCompanySyncTable = new System.Data.DataTable();
        sapFieldsTable = new System.Data.DataTable();
        sapSyncHistoryTable = new System.Data.DataTable();
        attachmentsTable = new System.Data.DataTable();
        operationalAlertsTable = new System.Data.DataTable();
        allowedLocationsTable = new System.Data.DataTable();
        purchasesPresentationsTable = new System.Data.DataTable();
        itemSuppliersTable = new System.Data.DataTable();
        pnlPresentationBarcodes = new PanelControl();
        lblPresentationBarcodesTitle = new LabelControl();
        grdPresentationBarcodes = new GridControl();
        presentationBarcodesTable = new System.Data.DataTable();
        gvPresentationBarcodes = new GridView();
        colBarcodeValue = new GridColumn();
        colBarcodeScope = new GridColumn();
        colBarcodePresentation = new GridColumn();
        colBarcodeUnit = new GridColumn();
        colBarcodeFactor = new GridColumn();
        colBarcodePrincipal = new GridColumn();
        repoBarcodePrincipal = new RepositoryItemCheckEdit();
        colBarcodeActive = new GridColumn();
        repoBarcodeActive = new RepositoryItemCheckEdit();
        btnAddBarcode = new SimpleButton();
        btnUpdateBarcode = new SimpleButton();
        btnRemoveBarcode = new SimpleButton();
        btnSetMainBarcode = new SimpleButton();
        tabRemarks = new XtraTabPage();
        pnlNotesAlerts = new PanelControl();
        btnClearOperationalAlert = new SimpleButton();
        btnRemoveOperationalAlert = new SimpleButton();
        btnUpdateOperationalAlert = new SimpleButton();
        btnAddOperationalAlert = new SimpleButton();
        grdOperationalAlerts = new GridControl();
        gridView1 = new GridView();
        repoOperationalAlertCheck = new RepositoryItemCheckEdit();
        gvOperationalAlerts = new GridView();
        colOperationalAlertType = new GridColumn();
        colOperationalAlertProcess = new GridColumn();
        colOperationalAlertMessage = new GridColumn();
        colOperationalAlertFrom = new GridColumn();
        colOperationalAlertTo = new GridColumn();
        colOperationalAlertBlocking = new GridColumn();
        colOperationalAlertActive = new GridColumn();
        lblNotesAlertsTitle = new LabelControl();
        pnlNotesProcess = new PanelControl();
        memLogisticsQualityNotes = new MemoEdit();
        lblLogisticsQualityNotes = new LabelControl();
        memInventoryNotes = new MemoEdit();
        lblInventoryNotes = new LabelControl();
        memSalesNotes = new MemoEdit();
        lblSalesNotes = new LabelControl();
        memPurchaseNotes = new MemoEdit();
        lblPurchaseNotes = new LabelControl();
        lblNotesProcessTitle = new LabelControl();
        pnlNotesGeneral = new PanelControl();
        chkGeneralNoteActive = new CheckEdit();
        lueNotePriority = new LookUpEdit();
        lblNotePriority = new LabelControl();
        memGeneralOperationalAlert = new MemoEdit();
        lblGeneralOperationalAlert = new LabelControl();
        memGeneralNotes = new MemoEdit();
        lblGeneralNotes = new LabelControl();
        lblNotesGeneralTitle = new LabelControl();
        tabAttachments = new XtraTabPage();
        gridView2 = new GridView();
        tabSap = new XtraTabPage();
        gridView3 = new GridView();
        gridView4 = new GridView();
        gridView5 = new GridView();
        gridView6 = new GridView();
        gridView7 = new GridView();
        tabLots = new XtraTabPage();
        pnlLotTraceabilityNote = new PanelControl();
        lblLotTraceabilityNote = new LabelControl();
        lblLotTraceabilityNoteIcon = new LabelControl();
        lueNumberingMethod = new LookUpEdit();
        lblNumberingMethod = new LabelControl();
        txtBatchFormat = new TextEdit();
        lblBatchFormat = new LabelControl();
        spnQuarantineDays = new SpinEdit();
        lblQuarantineDays = new LabelControl();
        spnExpirationAlertDays = new SpinEdit();
        lblExpirationAlertDays = new LabelControl();
        spnShelfLifeDays = new SpinEdit();
        lblShelfLifeDays = new LabelControl();
        spnSerialLength = new SpinEdit();
        lblSerialLength = new LabelControl();
        txtBatchPrefix = new TextEdit();
        lblBatchPrefix = new LabelControl();
        tglAutoGenerateBatch = new ToggleSwitch();
        lblAutoGenerateBatch = new LabelControl();
        tglExpirationMandatory = new ToggleSwitch();
        lblExpirationMandatory = new LabelControl();
        tglRequiresExpiration = new ToggleSwitch();
        lblRequiresExpiration = new LabelControl();
        lblLotTraceabilityTitle = new LabelControl();
        pnlLotOperationalNote = new PanelControl();
        lblLotOperationalNote = new LabelControl();
        lblLotOperationalNoteIcon = new LabelControl();
        memLotOperationalNotes = new MemoEdit();
        lblLotOperationalNotes = new LabelControl();
        tglBlockExpiredBatch = new ToggleSwitch();
        lblBlockExpiredBatch = new LabelControl();
        tglBlockQuarantineBatch = new ToggleSwitch();
        lblBlockQuarantineBatch = new LabelControl();
        tglAllowExpiredBatchSale = new ToggleSwitch();
        lblAllowExpiredBatchSale = new LabelControl();
        tglAllowMultipleBatches = new ToggleSwitch();
        lblAllowMultipleBatches = new LabelControl();
        lueIssueMethod = new LookUpEdit();
        lblIssueMethod = new LabelControl();
        lblLotOperationalRulesTitle = new LabelControl();
        tabTaxes = new XtraTabPage();
        pnlTaxConfigurationNote = new PanelControl();
        lblTaxConfigurationNote = new LabelControl();
        lblTaxConfigurationNoteIcon = new LabelControl();
        tglTaxExemptGoods = new ToggleSwitch();
        lblTaxExemptGoods = new LabelControl();
        tglTaxableService = new ToggleSwitch();
        lblTaxableService = new LabelControl();
        tglTaxableGoods = new ToggleSwitch();
        lblTaxableGoods = new LabelControl();
        lueFiscalCountry = new LookUpEdit();
        lblFiscalCountry = new LabelControl();
        txtFiscalCode = new TextEdit();
        lblFiscalCode = new LabelControl();
        lueTaxSupport = new LookUpEdit();
        lblTaxSupport = new LabelControl();
        lueTaxesSuggestedWithholding = new LookUpEdit();
        lblTaxesSuggestedWithholding = new LabelControl();
        lueExciseTax = new LookUpEdit();
        lblExciseTax = new LabelControl();
        lueTaxesSalesVat = new LookUpEdit();
        lblTaxesSalesVat = new LabelControl();
        luePurchaseVat = new LookUpEdit();
        lblPurchaseVat = new LabelControl();
        lueFiscalItemType = new LookUpEdit();
        lblFiscalItemType = new LabelControl();
        lblTaxConfigurationTitle = new LabelControl();
        tabAccounting = new XtraTabPage();
        pnlAccountingRules = new PanelControl();
        pnlAccountingRulesNote = new PanelControl();
        lblAccountingRulesNote = new LabelControl();
        lblAccountingRulesNoteIcon = new LabelControl();
        memAccountingNotes = new MemoEdit();
        lblAccountingNotes = new LabelControl();
        lueAccountingIntegrationMethod = new LookUpEdit();
        lblAccountingIntegrationMethod = new LabelControl();
        spnReconciliationDays = new SpinEdit();
        lblReconciliationDays = new LabelControl();
        tglAccountingBlocked = new ToggleSwitch();
        lblAccountingBlocked = new LabelControl();
        tglAllowCompensation = new ToggleSwitch();
        lblAllowCompensation = new LabelControl();
        tglUseGroupAccount = new ToggleSwitch();
        lblUseGroupAccount = new LabelControl();
        tglUseWarehouseAccount = new ToggleSwitch();
        lblUseWarehouseAccount = new LabelControl();
        tglGenerateInventoryJournal = new ToggleSwitch();
        lblGenerateInventoryJournal = new LabelControl();
        lblAccountingRulesTitle = new LabelControl();
        pnlAccountingAccountsNote = new PanelControl();
        lblAccountingAccountsNote = new LabelControl();
        lblAccountingAccountsNoteIcon = new LabelControl();
        sluePurchaseExpenseAccount = new SearchLookUpEdit();
        gvPurchaseExpenseAccount = new GridView();
        lblPurchaseExpenseAccount = new LabelControl();
        slueInventoryAdjustmentAccount = new SearchLookUpEdit();
        gvInventoryAdjustmentAccount = new GridView();
        lblInventoryAdjustmentAccount = new LabelControl();
        slueCostVarianceAccount = new SearchLookUpEdit();
        gvCostVarianceAccount = new GridView();
        lblCostVarianceAccount = new LabelControl();
        sluePurchaseReturnAccount = new SearchLookUpEdit();
        gvPurchaseReturnAccount = new GridView();
        lblPurchaseReturnAccount = new LabelControl();
        slueSalesReturnAccount = new SearchLookUpEdit();
        gvSalesReturnAccount = new GridView();
        lblSalesReturnAccount = new LabelControl();
        slueCostOfGoodsSoldAccount = new SearchLookUpEdit();
        gvCostOfGoodsSoldAccount = new GridView();
        lblCostOfGoodsSoldAccount = new LabelControl();
        slueRevenueAccount = new SearchLookUpEdit();
        gvRevenueAccount = new GridView();
        lblAccountingRevenueAccount = new LabelControl();
        slueInventoryAccount = new SearchLookUpEdit();
        gvInventoryAccount = new GridView();
        lblAccountingInventoryAccount = new LabelControl();
        lblAccountingAccountsTitle = new LabelControl();
        tabCosts = new XtraTabPage();
        lblSimulatorEquals = new LabelControl();
        lblSimulatorPrice = new LabelControl();
        spnSimulatorMargin = new SpinEdit();
        spnSimulatorPrice = new SpinEdit();
        lblSimulatorMargin = new LabelControl();
        lblSimulatorPlus = new LabelControl();
        spnSimulatorCost = new SpinEdit();
        tglManualCostUpdate = new ToggleSwitch();
        lblSimulatorCost = new LabelControl();
        lblManualCostUpdate = new LabelControl();
        lblSimulatorTitle = new LabelControl();
        dtCostUpdatedAt = new DateEdit();
        dtPriceUpdatedAt = new DateEdit();
        lblCostUpdatedAt = new LabelControl();
        lblPriceUpdatedAt = new LabelControl();
        spnAverageCost = new SpinEdit();
        pnlProfitability12m = new PanelControl();
        lblProfitability12mValue = new LabelControl();
        lblProfitability12mCaption = new LabelControl();
        lblCostsAverageCost = new LabelControl();
        pnlGrossMarginPercent = new PanelControl();
        lblGrossMarginPercentValue = new LabelControl();
        lblGrossMarginPercentCaption = new LabelControl();
        spnLastCost = new SpinEdit();
        pnlGrossMargin = new PanelControl();
        lblGrossMarginUnit = new LabelControl();
        lblGrossMarginValue = new LabelControl();
        lblGrossMarginCaption = new LabelControl();
        lblLastCost = new LabelControl();
        spnTargetMarginPercent = new SpinEdit();
        spnReplacementCost = new SpinEdit();
        lblTargetMarginPercent = new LabelControl();
        lblReplacementCost = new LabelControl();
        spnMinimumMarginPercent = new SpinEdit();
        spnStandardCost = new SpinEdit();
        lblMinimumMarginPercent = new LabelControl();
        lblStandardCost = new LabelControl();
        spnSuggestedPrice = new SpinEdit();
        lueCostCurrency = new LookUpEdit();
        lblSuggestedPrice = new LabelControl();
        lblCostCurrency = new LabelControl();
        spnAnalysisBasePrice = new SpinEdit();
        lblCostsBaseTitle = new LabelControl();
        lblAnalysisBasePrice = new LabelControl();
        lblPricesMarginsTitle = new LabelControl();
        grdCostPriceHistory = new GridControl();
        gridView8 = new GridView();
        gvCostPriceHistory = new GridView();
        colCostHistoryDate = new GridColumn();
        colCostHistoryMovement = new GridColumn();
        colCostHistoryDocument = new GridColumn();
        colCostHistoryPreviousCost = new GridColumn();
        colCostHistoryNewCost = new GridColumn();
        colCostHistoryPreviousPrice = new GridColumn();
        colCostHistoryNewPrice = new GridColumn();
        colCostHistoryVariation = new GridColumn();
        colCostHistoryUser = new GridColumn();
        colCostHistoryObservation = new GridColumn();
        lblCostPriceHistoryTitle = new LabelControl();
        tabSales = new XtraTabPage();
        spnSalesCommission = new SpinEdit();
        lblSalesCommission = new LabelControl();
        lblSalesMultipleUnit = new LabelControl();
        spnSalesMultiple = new SpinEdit();
        lblSalesMultiple = new LabelControl();
        lblMinimumSaleUnit = new LabelControl();
        spnMinimumSale = new SpinEdit();
        lblMinimumSale = new LabelControl();
        spnMinimumMargin = new SpinEdit();
        lblMinimumMargin = new LabelControl();
        spnMaxDiscount = new SpinEdit();
        lblMaxDiscount = new LabelControl();
        tglAllowSalesDiscount = new ToggleSwitch();
        lblAllowSalesDiscount = new LabelControl();
        lueMainPriceList = new LookUpEdit();
        lblMainPriceList = new LabelControl();
        lueSalesCurrency = new LookUpEdit();
        spnBaseSalesPrice = new SpinEdit();
        lblBaseSalesPrice = new LabelControl();
        lueSalesUnit = new LookUpEdit();
        pnlSalesKpiCustomers = new PanelControl();
        lblSalesKpiCustomersValue = new LabelControl();
        lblSalesKpiCustomersCaption = new LabelControl();
        lblSalesUnit = new LabelControl();
        pnlSalesKpiLastPrice = new PanelControl();
        lblSalesKpiLastPriceValue = new LabelControl();
        lblSalesKpiLastPriceCaption = new LabelControl();
        tglAffectsPromotions = new ToggleSwitch();
        pnlSalesKpi12m = new PanelControl();
        lblSalesKpi12mValue = new LabelControl();
        lblSalesKpi12mCaption = new LabelControl();
        lblAffectsPromotions = new LabelControl();
        pnlSalesKpi30d = new PanelControl();
        lblSalesKpi30dValue = new LabelControl();
        lblSalesKpi30dCaption = new LabelControl();
        lblSalesConfigurationTitle = new LabelControl();
        grdSalesPriceLists = new GridControl();
        gridView9 = new GridView();
        repoSalesPriceListActive = new RepositoryItemCheckEdit();
        gvSalesPriceLists = new GridView();
        colSalesPriceListName = new GridColumn();
        colSalesPriceListCurrency = new GridColumn();
        colSalesPriceListPrice = new GridColumn();
        colSalesPriceListMargin = new GridColumn();
        colSalesPriceListValidFrom = new GridColumn();
        colSalesPriceListActive = new GridColumn();
        lblSalesPricePerformanceTitle = new LabelControl();
        tabPurchases = new XtraTabPage();
        lblPurchasePolicy = new LabelControl();
        memPurchasePolicy = new MemoEdit();
        tglPurchaseOnDemand = new ToggleSwitch();
        lblReceivingNote = new LabelControl();
        lblPurchaseOnDemand = new LabelControl();
        memReceivingNote = new MemoEdit();
        tglSupplierBackorderAllowed = new ToggleSwitch();
        lblSupplierBackorderAllowed = new LabelControl();
        pnlPurchaseKpiLeadTime = new PanelControl();
        lblPurchaseKpiLeadTimeValue = new LabelControl();
        lblPurchaseKpiLeadTimeCaption = new LabelControl();
        tglPurchaseApprovalRequired = new ToggleSwitch();
        pnlPurchaseKpiAverage = new PanelControl();
        lblPurchaseKpiAverageValue = new LabelControl();
        lblPurchaseKpiAverageCaption = new LabelControl();
        lblPurchaseApprovalRequired = new LabelControl();
        pnlPurchaseKpiLast = new PanelControl();
        lblPurchaseKpiLastValue = new LabelControl();
        lblPurchaseKpiLastCaption = new LabelControl();
        lblPurchasesConfigurationTitle = new LabelControl();
        grdPurchaseHistory = new GridControl();
        gridView10 = new GridView();
        gvPurchaseHistory = new GridView();
        colPurchaseHistoryDate = new GridColumn();
        colPurchaseHistoryDocument = new GridColumn();
        colPurchaseHistorySupplier = new GridColumn();
        colPurchaseHistoryPresentation = new GridColumn();
        colPurchaseHistoryQuantity = new GridColumn();
        colPurchaseHistoryUnit = new GridColumn();
        colPurchaseHistoryInventoryQty = new GridColumn();
        colPurchaseHistoryUnitCost = new GridColumn();
        colPurchaseHistoryCurrency = new GridColumn();
        colPurchaseHistoryStatus = new GridColumn();
        lblPurchasesHistoryTitle = new LabelControl();
        pnlPurchaseKpiCompliance = new PanelControl();
        lblPurchaseKpiComplianceValue = new LabelControl();
        lblPurchaseKpiComplianceCaption = new LabelControl();
        lookUpEdit1 = new LookUpEdit();
        labelControl1 = new LabelControl();
        tabInventory = new XtraTabPage();
        memInventoryBlockReason = new MemoEdit();
        lblInventoryBlockReason = new LabelControl();
        lueInventoryControlType = new LookUpEdit();
        lblInventoryControlType = new LabelControl();
        lueAbcClassification = new LookUpEdit();
        lblAbcClassification = new LabelControl();
        tglRequiresCycleCount = new ToggleSwitch();
        lblRequiresCycleCount = new LabelControl();
        tglManageLocations = new ToggleSwitch();
        lblManageLocations = new LabelControl();
        tglAutoReplenishment = new ToggleSwitch();
        lblAutoReplenishment = new LabelControl();
        tglReplenishmentApproval = new ToggleSwitch();
        lueNegativeStockPolicy = new LookUpEdit();
        lblReplenishmentApproval = new LabelControl();
        lblNegativeStockPolicy = new LabelControl();
        spnSuggestedPurchaseQty = new SpinEdit();
        lueValuationMethod = new LookUpEdit();
        lblSuggestedPurchaseQty = new LabelControl();
        slueMainWarehouse = new SearchLookUpEdit();
        gvMainWarehouse = new GridView();
        spnGlobalReorderPoint = new SpinEdit();
        memInventoryOperationNote = new MemoEdit();
        lblValuationMethod = new LabelControl();
        lblInventoryOperationNote = new LabelControl();
        lblGlobalReorderPoint = new LabelControl();
        tglBlockedForMovements = new ToggleSwitch();
        lueSupplyMethod = new LookUpEdit();
        lblBlockedForMovements = new LabelControl();
        spnGlobalMaxStock = new SpinEdit();
        lblMainWarehouse = new LabelControl();
        lblGlobalMaxStock = new LabelControl();
        lueReplenishmentMethod = new LookUpEdit();
        spnGlobalMinStock = new SpinEdit();
        lblSupplyMethod = new LabelControl();
        lblGlobalMinStock = new LabelControl();
        lblReplenishmentMethod = new LabelControl();
        spnLeadTimeDays = new SpinEdit();
        lblInventoryParametersTitle = new LabelControl();
        lblLeadTimeDays = new LabelControl();
        lblReplenishmentOperationTitle = new LabelControl();
        spnCoverageDays = new SpinEdit();
        lblCoverageDays = new LabelControl();
        grdWarehouseStock = new GridControl();
        gridView11 = new GridView();
        gvWarehouseStock = new GridView();
        colWarehouseCode = new GridColumn();
        colWarehouseName = new GridColumn();
        colWarehouseStockActual = new GridColumn();
        colWarehouseCommitted = new GridColumn();
        colWarehouseOrdered = new GridColumn();
        colWarehouseAvailable = new GridColumn();
        colWarehouseMinimum = new GridColumn();
        colWarehouseMaximum = new GridColumn();
        colWarehouseReorder = new GridColumn();
        colWarehouseStatus = new GridColumn();
        slueDefaultBinLocation = new SearchLookUpEdit();
        gvDefaultBinLocation = new GridView();
        lblStockByWarehouseTitle = new LabelControl();
        lblDefaultBinLocation = new LabelControl();
        lblInventoryLocationsRestrictionsTitle = new LabelControl();
        btnSetMainWarehouseStock = new SimpleButton();
        btnRemoveWarehouseStock = new SimpleButton();
        btnUpdateWarehouseStock = new SimpleButton();
        btnAddWarehouseStock = new SimpleButton();
        tabUnits = new XtraTabPage();
        lueVolumeUnit = new LookUpEdit();
        lblVolumeUnit = new LabelControl();
        lueWeightUnit = new LookUpEdit();
        lblWeightUnit = new LabelControl();
        lblVolumeUnitCaption = new LabelControl();
        spnVolume = new SpinEdit();
        lblVolume = new LabelControl();
        lblGrossWeightUnit = new LabelControl();
        spnGrossWeight = new SpinEdit();
        lblGrossWeight = new LabelControl();
        lblNetWeightUnit = new LabelControl();
        lueCodeOrigin = new LookUpEdit();
        spnNetWeight = new SpinEdit();
        lblCodeOrigin = new LabelControl();
        lblNetWeight = new LabelControl();
        txtTariffCode = new TextEdit();
        lblTariffCode = new LabelControl();
        btnSetMainPurchasePresentation = new SimpleButton();
        txtUnspscCode = new TextEdit();
        lblUnspscCode = new LabelControl();
        btnRemovePurchasePresentation = new SimpleButton();
        txtManufacturerReference = new TextEdit();
        lueInventoryUnit = new LookUpEdit();
        lblManufacturerReference = new LabelControl();
        btnUpdatePurchasePresentation = new SimpleButton();
        txtPreviousInternalCode = new TextEdit();
        lblInventoryUnit = new LabelControl();
        lblPreviousInternalCode = new LabelControl();
        btnAddPurchasePresentation = new SimpleButton();
        txtPlu = new TextEdit();
        lblInventoryUnitTitle = new LabelControl();
        lblPlu = new LabelControl();
        grdPurchasePresentations = new GridControl();
        gridView12 = new GridView();
        repoPurchasePrincipal = new RepositoryItemCheckEdit();
        repoPurchaseActive = new RepositoryItemCheckEdit();
        gvPurchasePresentations = new GridView();
        colPurchasePresentation = new GridColumn();
        colPurchaseUnit = new GridColumn();
        colPurchaseFactor = new GridColumn();
        colPurchaseBarcode = new GridColumn();
        colPurchaseEnabled = new GridColumn();
        colSalesEnabled = new GridColumn();
        colPurchasePrincipal = new GridColumn();
        colSalesPrincipal = new GridColumn();
        colPurchaseActive = new GridColumn();
        txtQrCode = new TextEdit();
        lblPurchasePresentationsTitle = new LabelControl();
        lblQrCode = new LabelControl();
        lblCodesIdentifiersTitle = new LabelControl();
        tabGeneral = new XtraTabPage();
        tglPurchaseActive = new ToggleSwitch();
        lblPurchaseActive = new LabelControl();
        tglSalesActive = new ToggleSwitch();
        lblSalesActive = new LabelControl();
        txtReference = new TextEdit();
        lblReference = new LabelControl();
        txtModel = new TextEdit();
        lblModel = new LabelControl();
        lueSubGroup = new LookUpEdit();
        lblSubGroup = new LabelControl();
        lueLine = new LookUpEdit();
        lblLine = new LabelControl();
        lueOrigin = new LookUpEdit();
        lblOrigin = new LabelControl();
        lueProductType = new LookUpEdit();
        lblProductType = new LabelControl();
        memLongDescription = new MemoEdit();
        lblLongDescription = new LabelControl();
        slueSupplierSku = new SearchLookUpEdit();
        gvSupplierSku = new GridView();
        lblSupplierSku = new LabelControl();
        txtAlternateCode = new TextEdit();
        lblAlternateCode = new LabelControl();
        lblGeneralIdentificationTitle = new LabelControl();
        tglAffectsInventory = new ToggleSwitch();
        lblAffectsInventory = new LabelControl();
        tglGeneralExpirationManaged = new ToggleSwitch();
        lblAllowDiscount = new LabelControl();
        lblRequiresScale = new LabelControl();
        lblExpirationManaged = new LabelControl();
        lblPerishable = new LabelControl();
        lblSerialManaged = new LabelControl();
        lblBatchManaged = new LabelControl();
        lblGeneralOperationTitle = new LabelControl();
        tglGeneralBatchManaged = new ToggleSwitch();
        tglGeneralSerialManaged = new ToggleSwitch();
        tglGeneralPerishable = new ToggleSwitch();
        tglGeneralAllowDiscount = new ToggleSwitch();
        tglGeneralRequiresScale = new ToggleSwitch();
        lblGeneralMobileItem = new LabelControl();
        tglGeneralMobileItem = new ToggleSwitch();
        pnlKpiVariants = new PanelControl();
        lblKpiVariantsValue = new LabelControl();
        lblKpiVariantsCaption = new LabelControl();
        pnlKpiSap = new PanelControl();
        lblKpiSapValue = new LabelControl();
        lblKpiSapCaption = new LabelControl();
        pnlKpiSales = new PanelControl();
        lblKpiSalesUnit = new LabelControl();
        lblKpiSalesValue = new LabelControl();
        lblKpiSalesCaption = new LabelControl();
        pnlKpiPurchases = new PanelControl();
        lblKpiPurchasesUnit = new LabelControl();
        lblKpiPurchasesValue = new LabelControl();
        lblKpiPurchasesCaption = new LabelControl();
        pnlKpiOrders = new PanelControl();
        lblKpiOrdersUnit = new LabelControl();
        lblKpiOrdersValue = new LabelControl();
        lblKpiOrdersCaption = new LabelControl();
        pnlKpiStock = new PanelControl();
        lblKpiStockUnit = new LabelControl();
        lblKpiStockValue = new LabelControl();
        lblKpiStockCaption = new LabelControl();
        lblGeneralSummaryTitle = new LabelControl();
        tglBlockedEcommerce = new ToggleSwitch();
        lblBlockedEcommerce = new LabelControl();
        tabMain = new XtraTabControl();
        btnClearSapFields = new SimpleButton();
        btnRemoveSapField = new SimpleButton();
        btnUpdateSapField = new SimpleButton();
        btnAddSapField = new SimpleButton();
        grdSapFieldMapping = new GridControl();
        grvSapFieldMapping = new GridView();
        colSapMapSystemField = new GridColumn();
        colSapMapSapField = new GridColumn();
        colSapMapDescription = new GridColumn();
        colSapMapRequired = new GridColumn();
        colSapMapEnabled = new GridColumn();
        lblSapFieldMappingTitle = new LabelControl();
        lblSapMapEnabled = new LabelControl();
        lblSapMapRequired = new LabelControl();
        lueSapMapEnabled = new LookUpEdit();
        lblSapMapDescription = new LabelControl();
        lueSapMapRequired = new LookUpEdit();
        lblSapMapSapField = new LabelControl();
        txtSapMapDescription = new TextEdit();
        lblSapMapSystemField = new LabelControl();
        txtSapMapSapField = new TextEdit();
        lblSapHistoryTitle = new LabelControl();
        txtSapMapSystemField = new TextEdit();
        grdSapSyncHistory = new GridControl();
        grvSapSyncHistory = new GridView();
        colSapHistoryDate = new GridColumn();
        colSapHistoryOperation = new GridColumn();
        colSapHistoryStatus = new GridColumn();
        colSapHistoryDocEntry = new GridColumn();
        colSapHistoryDocNum = new GridColumn();
        colSapHistoryRetryCount = new GridColumn();
        colSapHistoryMessage = new GridColumn();
        lblSapSyncAsSupplier = new LabelControl();
        lblSapMode = new LabelControl();
        lueSapSyncAsSupplier = new LookUpEdit();
        lblSapConfigTitle = new LabelControl();
        lblSapManualRetry = new LabelControl();
        lueSapMode = new LookUpEdit();
        lueSapManualRetry = new LookUpEdit();
        lblSapCompany = new LabelControl();
        lblSapRequiresApproval = new LabelControl();
        lblSapStatusTitle = new LabelControl();
        lueSapRequiresApproval = new LookUpEdit();
        lueSapCompany = new SearchLookUpEdit();
        grvSapCompanyLookup = new GridView();
        lblSapSyncStatus = new LabelControl();
        lueSapSyncStatus = new LookUpEdit();
        lblSapLastSync = new LabelControl();
        txtSapLastSync = new TextEdit();
        lblSapLastError = new LabelControl();
        txtSapLastError = new TextEdit();
        lblSapRetryCount = new LabelControl();
        txtSapRetryCount = new TextEdit();
        lblSapEnabled = new LabelControl();
        lueSapEnabled = new LookUpEdit();
        lblAttachmentPreviewTitle = new LabelControl();
        picMainAttachmentPreview = new PictureEdit();
        btnLoadImage = new SimpleButton();
        btnRemoveImage = new SimpleButton();
        btnPreviewImage = new SimpleButton();
        btnSetMainImage = new SimpleButton();
        pnlAttachmentPreviewNote = new PanelControl();
        lblAttachmentPreviewNoteIcon = new LabelControl();
        lblAttachmentPreviewNote = new LabelControl();
        lblAttachmentMetadataTitle = new LabelControl();
        lblAttachmentType = new LabelControl();
        lueAttachmentType = new LookUpEdit();
        lblAttachmentFileName = new LabelControl();
        txtAttachmentFileName = new TextEdit();
        lblAttachmentDescription = new LabelControl();
        memAttachmentDescription = new MemoEdit();
        lblAttachmentCategory = new LabelControl();
        lueAttachmentCategory = new LookUpEdit();
        chkVisibleInSales = new CheckEdit();
        chkVisibleInPurchases = new CheckEdit();
        chkVisibleInPortal = new CheckEdit();
        lblAttachmentStatus = new LabelControl();
        lueAttachmentStatus = new LookUpEdit();
        lblAttachmentExtension = new LabelControl();
        txtAttachmentExtension = new TextEdit();
        lblAttachmentSize = new LabelControl();
        txtAttachmentSize = new TextEdit();
        lblAttachmentUploadedAt = new LabelControl();
        dteAttachmentUploadedAt = new DateEdit();
        lblAttachmentUser = new LabelControl();
        txtAttachmentUser = new TextEdit();
        lblAttachmentGridTitle = new LabelControl();
        grdAttachments = new GridControl();
        gvAttachments = new GridView();
        colAttachmentDocumentType = new GridColumn();
        colAttachmentFileName = new GridColumn();
        colAttachmentDescription = new GridColumn();
        colAttachmentExtension = new GridColumn();
        colAttachmentSize = new GridColumn();
        colAttachmentDate = new GridColumn();
        colAttachmentUser = new GridColumn();
        colAttachmentPrincipal = new GridColumn();
        repoAttachmentCheck = new RepositoryItemCheckEdit();
        colAttachmentVisibleSales = new GridColumn();
        colAttachmentVisiblePurchases = new GridColumn();
        colAttachmentStatus = new GridColumn();
        btnAddAttachment = new SimpleButton();
        btnUpdateAttachment = new SimpleButton();
        btnRemoveAttachment = new SimpleButton();
        btnDownloadAttachment = new SimpleButton();
        btnOpenAttachment = new SimpleButton();
        btnSetMainAttachment = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)pnlHeader).BeginInit();
        pnlHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picItem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtItemCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCommercialName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueItemType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueItemGroup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueItemFamily.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBrand.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBaseUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlFooter).BeginInit();
        pnlFooter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)purchasePresentationsTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)warehouseStockTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)purchaseHistoryTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)salesPriceListsTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)costComponentsTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)costPriceHistoryTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)taxMatrixTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)recentLotsTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)variantAttributesTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)registeredVariantsTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sapCompanySyncTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sapFieldsTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sapSyncHistoryTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)attachmentsTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)operationalAlertsTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)allowedLocationsTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)purchasesPresentationsTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)itemSuppliersTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlPresentationBarcodes).BeginInit();
        pnlPresentationBarcodes.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdPresentationBarcodes).BeginInit();
        ((System.ComponentModel.ISupportInitialize)presentationBarcodesTable).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvPresentationBarcodes).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoBarcodePrincipal).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoBarcodeActive).BeginInit();
        tabRemarks.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlNotesAlerts).BeginInit();
        pnlNotesAlerts.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdOperationalAlerts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoOperationalAlertCheck).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvOperationalAlerts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlNotesProcess).BeginInit();
        pnlNotesProcess.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memLogisticsQualityNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memInventoryNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memSalesNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memPurchaseNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlNotesGeneral).BeginInit();
        pnlNotesGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)chkGeneralNoteActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueNotePriority.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memGeneralOperationalAlert.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memGeneralNotes.Properties).BeginInit();
        tabAttachments.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridView2).BeginInit();
        tabSap.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridView3).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView4).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView5).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView6).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView7).BeginInit();
        tabLots.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlLotTraceabilityNote).BeginInit();
        pnlLotTraceabilityNote.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueNumberingMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBatchFormat.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnQuarantineDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnExpirationAlertDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnShelfLifeDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSerialLength.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBatchPrefix.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAutoGenerateBatch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglExpirationMandatory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresExpiration.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlLotOperationalNote).BeginInit();
        pnlLotOperationalNote.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memLotOperationalNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockExpiredBatch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockQuarantineBatch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowExpiredBatchSale.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowMultipleBatches.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueIssueMethod.Properties).BeginInit();
        tabTaxes.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlTaxConfigurationNote).BeginInit();
        pnlTaxConfigurationNote.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglTaxExemptGoods.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglTaxableService.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglTaxableGoods.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtFiscalCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxSupport.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxesSuggestedWithholding.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueExciseTax.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxesSalesVat.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseVat.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalItemType.Properties).BeginInit();
        tabAccounting.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingRules).BeginInit();
        pnlAccountingRules.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingRulesNote).BeginInit();
        pnlAccountingRulesNote.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memAccountingNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingIntegrationMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnReconciliationDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAccountingBlocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowCompensation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglUseGroupAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglUseWarehouseAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGenerateInventoryJournal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingAccountsNote).BeginInit();
        pnlAccountingAccountsNote.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sluePurchaseExpenseAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseExpenseAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueInventoryAdjustmentAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvInventoryAdjustmentAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueCostVarianceAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvCostVarianceAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluePurchaseReturnAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseReturnAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueSalesReturnAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvSalesReturnAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueCostOfGoodsSoldAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvCostOfGoodsSoldAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueRevenueAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvRevenueAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueInventoryAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvInventoryAccount).BeginInit();
        tabCosts.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)spnSimulatorMargin.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSimulatorPrice.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSimulatorCost.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglManualCostUpdate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtCostUpdatedAt.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtCostUpdatedAt.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtPriceUpdatedAt.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtPriceUpdatedAt.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnAverageCost.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlProfitability12m).BeginInit();
        pnlProfitability12m.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlGrossMarginPercent).BeginInit();
        pnlGrossMarginPercent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)spnLastCost.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlGrossMargin).BeginInit();
        pnlGrossMargin.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)spnTargetMarginPercent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnReplacementCost.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumMarginPercent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnStandardCost.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSuggestedPrice.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCostCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnAnalysisBasePrice.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdCostPriceHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView8).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvCostPriceHistory).BeginInit();
        tabSales.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)spnSalesCommission.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSalesMultiple.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumSale.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumMargin.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMaxDiscount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowSalesDiscount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueMainPriceList.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnBaseSalesPrice.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlSalesKpiCustomers).BeginInit();
        pnlSalesKpiCustomers.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSalesKpiLastPrice).BeginInit();
        pnlSalesKpiLastPrice.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglAffectsPromotions.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlSalesKpi12m).BeginInit();
        pnlSalesKpi12m.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSalesKpi30d).BeginInit();
        pnlSalesKpi30d.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdSalesPriceLists).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView9).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoSalesPriceListActive).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvSalesPriceLists).BeginInit();
        tabPurchases.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memPurchasePolicy.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseOnDemand.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memReceivingNote.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSupplierBackorderAllowed.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlPurchaseKpiLeadTime).BeginInit();
        pnlPurchaseKpiLeadTime.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseApprovalRequired.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlPurchaseKpiAverage).BeginInit();
        pnlPurchaseKpiAverage.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlPurchaseKpiLast).BeginInit();
        pnlPurchaseKpiLast.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdPurchaseHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView10).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlPurchaseKpiCompliance).BeginInit();
        pnlPurchaseKpiCompliance.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lookUpEdit1.Properties).BeginInit();
        tabInventory.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memInventoryBlockReason.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueInventoryControlType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAbcClassification.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresCycleCount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglManageLocations.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAutoReplenishment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglReplenishmentApproval.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueNegativeStockPolicy.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSuggestedPurchaseQty.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueValuationMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueMainWarehouse.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvMainWarehouse).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalReorderPoint.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memInventoryOperationNote.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockedForMovements.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplyMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalMaxStock.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueReplenishmentMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalMinStock.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnLeadTimeDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCoverageDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdWarehouseStock).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView11).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvWarehouseStock).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueDefaultBinLocation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvDefaultBinLocation).BeginInit();
        tabUnits.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueVolumeUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueWeightUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnVolume.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnGrossWeight.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCodeOrigin.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnNetWeight.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtTariffCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtUnspscCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtManufacturerReference.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueInventoryUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPreviousInternalCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPlu.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdPurchasePresentations).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView12).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoPurchasePrincipal).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoPurchaseActive).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchasePresentations).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtQrCode.Properties).BeginInit();
        tabGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSalesActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtReference.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtModel.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSubGroup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueLine.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueOrigin.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueProductType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memLongDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueSupplierSku.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvSupplierSku).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAlternateCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAffectsInventory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralExpirationManaged.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralBatchManaged.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralSerialManaged.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralPerishable.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralAllowDiscount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralRequiresScale.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralMobileItem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlKpiVariants).BeginInit();
        pnlKpiVariants.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlKpiSap).BeginInit();
        pnlKpiSap.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlKpiSales).BeginInit();
        pnlKpiSales.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlKpiPurchases).BeginInit();
        pnlKpiPurchases.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlKpiOrders).BeginInit();
        pnlKpiOrders.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlKpiStock).BeginInit();
        pnlKpiStock.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglBlockedEcommerce.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tabMain).BeginInit();
        tabMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdSapFieldMapping).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSapFieldMapping).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapMapEnabled.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapMapRequired.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapSapField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapSystemField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdSapSyncHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSapSyncHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapSyncAsSupplier.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapMode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapManualRetry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapRequiresApproval.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapCompany.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSapCompanyLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapSyncStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastSync.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastError.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapRetryCount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapEnabled.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)picMainAttachmentPreview.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentPreviewNote).BeginInit();
        pnlAttachmentPreviewNote.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentFileName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memAttachmentDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentCategory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInSales.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInPurchases.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInPortal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentExtension.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentSize.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentUploadedAt.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentUploadedAt.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentUser.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoAttachmentCheck).BeginInit();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.Appearance.Font = new Font("Segoe UI", 9F);
        pnlHeader.Appearance.Options.UseFont = true;
        pnlHeader.BorderStyle = BorderStyles.NoBorder;
        pnlHeader.Controls.Add(lblStockTotalCaption);
        pnlHeader.Controls.Add(lblStockTotal);
        pnlHeader.Controls.Add(lblAverageCostCaption);
        pnlHeader.Controls.Add(lblAverageCost);
        pnlHeader.Controls.Add(lblSalesPriceCaption);
        pnlHeader.Controls.Add(lblSalesPrice);
        pnlHeader.Controls.Add(lblLastPurchaseCaption);
        pnlHeader.Controls.Add(lblLastPurchase);
        pnlHeader.Controls.Add(lblSapSyncedCaption);
        pnlHeader.Controls.Add(lblSapSynced);
        pnlHeader.Controls.Add(picItem);
        pnlHeader.Controls.Add(lblItemCode);
        pnlHeader.Controls.Add(txtItemCode);
        pnlHeader.Controls.Add(lblDescription);
        pnlHeader.Controls.Add(txtDescription);
        pnlHeader.Controls.Add(lblCommercialName);
        pnlHeader.Controls.Add(txtCommercialName);
        pnlHeader.Controls.Add(lblItemType);
        pnlHeader.Controls.Add(lueItemType);
        pnlHeader.Controls.Add(lblItemGroup);
        pnlHeader.Controls.Add(lueItemGroup);
        pnlHeader.Controls.Add(lblItemFamily);
        pnlHeader.Controls.Add(lueItemFamily);
        pnlHeader.Controls.Add(lblBrand);
        pnlHeader.Controls.Add(lueBrand);
        pnlHeader.Controls.Add(lblBaseUnit);
        pnlHeader.Controls.Add(lueBaseUnit);
        pnlHeader.Controls.Add(lblStatusCaption);
        pnlHeader.Controls.Add(lblStatus);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1420, 204);
        pnlHeader.TabIndex = 3;
        // 
        // lblStockTotalCaption
        // 
        lblStockTotalCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblStockTotalCaption.Appearance.Options.UseFont = true;
        lblStockTotalCaption.Location = new Point(990, 29);
        lblStockTotalCaption.Name = "lblStockTotalCaption";
        lblStockTotalCaption.Size = new Size(59, 15);
        lblStockTotalCaption.TabIndex = 19;
        lblStockTotalCaption.Text = "Stock total:";
        // 
        // lblStockTotal
        // 
        lblStockTotal.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblStockTotal.Appearance.Options.UseFont = true;
        lblStockTotal.Appearance.Options.UseTextOptions = true;
        lblStockTotal.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        lblStockTotal.AutoSizeMode = LabelAutoSizeMode.None;
        lblStockTotal.Location = new Point(1126, 27);
        lblStockTotal.Name = "lblStockTotal";
        lblStockTotal.Size = new Size(92, 18);
        lblStockTotal.TabIndex = 20;
        lblStockTotal.Text = "1,250.00";
        // 
        // lblAverageCostCaption
        // 
        lblAverageCostCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblAverageCostCaption.Appearance.Options.UseFont = true;
        lblAverageCostCaption.Location = new Point(990, 56);
        lblAverageCostCaption.Name = "lblAverageCostCaption";
        lblAverageCostCaption.Size = new Size(89, 15);
        lblAverageCostCaption.TabIndex = 21;
        lblAverageCostCaption.Text = "Costo promedio:";
        // 
        // lblAverageCost
        // 
        lblAverageCost.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblAverageCost.Appearance.Options.UseFont = true;
        lblAverageCost.Appearance.Options.UseTextOptions = true;
        lblAverageCost.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        lblAverageCost.AutoSizeMode = LabelAutoSizeMode.None;
        lblAverageCost.Location = new Point(1126, 54);
        lblAverageCost.Name = "lblAverageCost";
        lblAverageCost.Size = new Size(92, 18);
        lblAverageCost.TabIndex = 22;
        lblAverageCost.Text = "18.65";
        // 
        // lblSalesPriceCaption
        // 
        lblSalesPriceCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesPriceCaption.Appearance.Options.UseFont = true;
        lblSalesPriceCaption.Location = new Point(990, 84);
        lblSalesPriceCaption.Name = "lblSalesPriceCaption";
        lblSalesPriceCaption.Size = new Size(68, 15);
        lblSalesPriceCaption.TabIndex = 23;
        lblSalesPriceCaption.Text = "Precio venta:";
        // 
        // lblSalesPrice
        // 
        lblSalesPrice.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblSalesPrice.Appearance.Options.UseFont = true;
        lblSalesPrice.Appearance.Options.UseTextOptions = true;
        lblSalesPrice.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        lblSalesPrice.AutoSizeMode = LabelAutoSizeMode.None;
        lblSalesPrice.Location = new Point(1126, 82);
        lblSalesPrice.Name = "lblSalesPrice";
        lblSalesPrice.Size = new Size(92, 18);
        lblSalesPrice.TabIndex = 24;
        lblSalesPrice.Text = "28.50";
        // 
        // lblLastPurchaseCaption
        // 
        lblLastPurchaseCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblLastPurchaseCaption.Appearance.Options.UseFont = true;
        lblLastPurchaseCaption.Location = new Point(990, 112);
        lblLastPurchaseCaption.Name = "lblLastPurchaseCaption";
        lblLastPurchaseCaption.Size = new Size(82, 15);
        lblLastPurchaseCaption.TabIndex = 25;
        lblLastPurchaseCaption.Text = "Última compra:";
        // 
        // lblLastPurchase
        // 
        lblLastPurchase.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblLastPurchase.Appearance.Options.UseFont = true;
        lblLastPurchase.Appearance.Options.UseTextOptions = true;
        lblLastPurchase.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        lblLastPurchase.AutoSizeMode = LabelAutoSizeMode.None;
        lblLastPurchase.Location = new Point(1126, 110);
        lblLastPurchase.Name = "lblLastPurchase";
        lblLastPurchase.Size = new Size(92, 18);
        lblLastPurchase.TabIndex = 26;
        lblLastPurchase.Text = "15/05/2026";
        // 
        // lblSapSyncedCaption
        // 
        lblSapSyncedCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSyncedCaption.Appearance.Options.UseFont = true;
        lblSapSyncedCaption.Location = new Point(990, 140);
        lblSapSyncedCaption.Name = "lblSapSyncedCaption";
        lblSapSyncedCaption.Size = new Size(95, 15);
        lblSapSyncedCaption.TabIndex = 27;
        lblSapSyncedCaption.Text = "Sincronizado SAP:";
        // 
        // lblSapSynced
        // 
        lblSapSynced.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblSapSynced.Appearance.ForeColor = Color.FromArgb(0, 168, 120);
        lblSapSynced.Appearance.Options.UseFont = true;
        lblSapSynced.Appearance.Options.UseForeColor = true;
        lblSapSynced.Appearance.Options.UseTextOptions = true;
        lblSapSynced.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        lblSapSynced.AutoSizeMode = LabelAutoSizeMode.None;
        lblSapSynced.Location = new Point(1126, 137);
        lblSapSynced.Name = "lblSapSynced";
        lblSapSynced.Size = new Size(92, 18);
        lblSapSynced.TabIndex = 28;
        lblSapSynced.Text = "Confirmado";
        // 
        // picItem
        // 
        picItem.Location = new Point(18, 26);
        picItem.Name = "picItem";
        picItem.Properties.Appearance.BackColor = Color.White;
        picItem.Properties.Appearance.Options.UseBackColor = true;
        picItem.Properties.BorderStyle = BorderStyles.Simple;
        picItem.Properties.NullText = "Imagen";
        picItem.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
        picItem.Properties.SizeMode = PictureSizeMode.Zoom;
        picItem.Size = new Size(142, 156);
        picItem.TabIndex = 0;
        // 
        // lblItemCode
        // 
        lblItemCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblItemCode.Appearance.Options.UseFont = true;
        lblItemCode.Location = new Point(195, 28);
        lblItemCode.Name = "lblItemCode";
        lblItemCode.Size = new Size(42, 15);
        lblItemCode.TabIndex = 1;
        lblItemCode.Text = "Código:";
        // 
        // txtItemCode
        // 
        txtItemCode.EditValue = "ARZ-001";
        txtItemCode.Location = new Point(325, 24);
        txtItemCode.Name = "txtItemCode";
        txtItemCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtItemCode.Properties.Appearance.Options.UseFont = true;
        txtItemCode.Size = new Size(185, 22);
        txtItemCode.TabIndex = 2;
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Location = new Point(195, 55);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(65, 15);
        lblDescription.TabIndex = 3;
        lblDescription.Text = "Descripción:";
        // 
        // txtDescription
        // 
        txtDescription.EditValue = "Arroz blanco premium 1kg";
        txtDescription.Location = new Point(325, 52);
        txtDescription.Name = "txtDescription";
        txtDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtDescription.Properties.Appearance.Options.UseFont = true;
        txtDescription.Size = new Size(550, 22);
        txtDescription.TabIndex = 4;
        // 
        // lblCommercialName
        // 
        lblCommercialName.Appearance.Font = new Font("Segoe UI", 9F);
        lblCommercialName.Appearance.Options.UseFont = true;
        lblCommercialName.Location = new Point(195, 83);
        lblCommercialName.Name = "lblCommercialName";
        lblCommercialName.Size = new Size(102, 15);
        lblCommercialName.TabIndex = 5;
        lblCommercialName.Text = "Nombre comercial:";
        // 
        // txtCommercialName
        // 
        txtCommercialName.EditValue = "Arroz NuanFood Premium 1kg";
        txtCommercialName.Location = new Point(325, 80);
        txtCommercialName.Name = "txtCommercialName";
        txtCommercialName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCommercialName.Properties.Appearance.Options.UseFont = true;
        txtCommercialName.Size = new Size(550, 22);
        txtCommercialName.TabIndex = 6;
        // 
        // lblItemType
        // 
        lblItemType.Appearance.Font = new Font("Segoe UI", 9F);
        lblItemType.Appearance.Options.UseFont = true;
        lblItemType.Location = new Point(195, 111);
        lblItemType.Name = "lblItemType";
        lblItemType.Size = new Size(54, 15);
        lblItemType.TabIndex = 7;
        lblItemType.Text = "Tipo ítem:";
        // 
        // lueItemType
        // 
        lueItemType.Location = new Point(325, 108);
        lueItemType.Name = "lueItemType";
        lueItemType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueItemType.Properties.Appearance.Options.UseFont = true;
        lueItemType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueItemType.Properties.NullText = "";
        lueItemType.Size = new Size(235, 22);
        lueItemType.TabIndex = 8;
        // 
        // lblItemGroup
        // 
        lblItemGroup.Appearance.Font = new Font("Segoe UI", 9F);
        lblItemGroup.Appearance.Options.UseFont = true;
        lblItemGroup.Location = new Point(581, 111);
        lblItemGroup.Name = "lblItemGroup";
        lblItemGroup.Size = new Size(36, 15);
        lblItemGroup.TabIndex = 9;
        lblItemGroup.Text = "Grupo:";
        // 
        // lueItemGroup
        // 
        lueItemGroup.Location = new Point(640, 108);
        lueItemGroup.Name = "lueItemGroup";
        lueItemGroup.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueItemGroup.Properties.Appearance.Options.UseFont = true;
        lueItemGroup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueItemGroup.Properties.NullText = "";
        lueItemGroup.Size = new Size(235, 22);
        lueItemGroup.TabIndex = 10;
        // 
        // lblItemFamily
        // 
        lblItemFamily.Appearance.Font = new Font("Segoe UI", 9F);
        lblItemFamily.Appearance.Options.UseFont = true;
        lblItemFamily.Location = new Point(195, 139);
        lblItemFamily.Name = "lblItemFamily";
        lblItemFamily.Size = new Size(41, 15);
        lblItemFamily.TabIndex = 11;
        lblItemFamily.Text = "Familia:";
        // 
        // lueItemFamily
        // 
        lueItemFamily.Location = new Point(325, 136);
        lueItemFamily.Name = "lueItemFamily";
        lueItemFamily.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueItemFamily.Properties.Appearance.Options.UseFont = true;
        lueItemFamily.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueItemFamily.Properties.NullText = "";
        lueItemFamily.Size = new Size(235, 22);
        lueItemFamily.TabIndex = 12;
        // 
        // lblBrand
        // 
        lblBrand.Appearance.Font = new Font("Segoe UI", 9F);
        lblBrand.Appearance.Options.UseFont = true;
        lblBrand.Location = new Point(581, 139);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(36, 15);
        lblBrand.TabIndex = 13;
        lblBrand.Text = "Marca:";
        // 
        // lueBrand
        // 
        lueBrand.Location = new Point(640, 136);
        lueBrand.Name = "lueBrand";
        lueBrand.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBrand.Properties.Appearance.Options.UseFont = true;
        lueBrand.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueBrand.Properties.NullText = "";
        lueBrand.Size = new Size(235, 22);
        lueBrand.TabIndex = 14;
        // 
        // lblBaseUnit
        // 
        lblBaseUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblBaseUnit.Appearance.Options.UseFont = true;
        lblBaseUnit.Location = new Point(195, 167);
        lblBaseUnit.Name = "lblBaseUnit";
        lblBaseUnit.Size = new Size(68, 15);
        lblBaseUnit.TabIndex = 15;
        lblBaseUnit.Text = "Unidad base:";
        // 
        // lueBaseUnit
        // 
        lueBaseUnit.Location = new Point(325, 164);
        lueBaseUnit.Name = "lueBaseUnit";
        lueBaseUnit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBaseUnit.Properties.Appearance.Options.UseFont = true;
        lueBaseUnit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueBaseUnit.Properties.NullText = "";
        lueBaseUnit.Size = new Size(235, 22);
        lueBaseUnit.TabIndex = 16;
        // 
        // lblStatusCaption
        // 
        lblStatusCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblStatusCaption.Appearance.Options.UseFont = true;
        lblStatusCaption.Location = new Point(581, 167);
        lblStatusCaption.Name = "lblStatusCaption";
        lblStatusCaption.Size = new Size(38, 15);
        lblStatusCaption.TabIndex = 17;
        lblStatusCaption.Text = "Estado:";
        // 
        // lblStatus
        // 
        lblStatus.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        lblStatus.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblStatus.Appearance.ForeColor = Color.White;
        lblStatus.Appearance.Options.UseBackColor = true;
        lblStatus.Appearance.Options.UseFont = true;
        lblStatus.Appearance.Options.UseForeColor = true;
        lblStatus.Appearance.Options.UseTextOptions = true;
        lblStatus.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        lblStatus.AutoSizeMode = LabelAutoSizeMode.None;
        lblStatus.BorderStyle = BorderStyles.NoBorder;
        lblStatus.Location = new Point(640, 161);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(82, 26);
        lblStatus.TabIndex = 18;
        lblStatus.Text = "Activo";
        // 
        // pnlFooter
        // 
        pnlFooter.BorderStyle = BorderStyles.NoBorder;
        pnlFooter.Controls.Add(btnSave);
        pnlFooter.Controls.Add(btnCancel);
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Location = new Point(0, 768);
        pnlFooter.Name = "pnlFooter";
        pnlFooter.Size = new Size(1420, 60);
        pnlFooter.TabIndex = 2;
        // 
        // btnSave
        // 
        btnSave.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.DialogResult = DialogResult.OK;
        btnSave.Location = new Point(1190, 13);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 0;
        btnSave.Text = "Guardar";
        // 
        // btnCancel
        // 
        btnCancel.Appearance.BackColor = Color.White;
        btnCancel.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancel.Appearance.ForeColor = Color.FromArgb(31, 42, 68);
        btnCancel.Appearance.Options.UseBackColor = true;
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.Appearance.Options.UseForeColor = true;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(1305, 13);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "Cancelar";
        // 
        // pnlPresentationBarcodes
        // 
        pnlPresentationBarcodes.BorderStyle = BorderStyles.Simple;
        pnlPresentationBarcodes.Controls.Add(lblPresentationBarcodesTitle);
        pnlPresentationBarcodes.Controls.Add(grdPresentationBarcodes);
        pnlPresentationBarcodes.Controls.Add(btnAddBarcode);
        pnlPresentationBarcodes.Controls.Add(btnUpdateBarcode);
        pnlPresentationBarcodes.Controls.Add(btnRemoveBarcode);
        pnlPresentationBarcodes.Controls.Add(btnSetMainBarcode);
        pnlPresentationBarcodes.Location = new Point(894, 18);
        pnlPresentationBarcodes.Name = "pnlPresentationBarcodes";
        pnlPresentationBarcodes.Size = new Size(506, 492);
        pnlPresentationBarcodes.TabIndex = 4;
        // 
        // lblPresentationBarcodesTitle
        // 
        lblPresentationBarcodesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPresentationBarcodesTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblPresentationBarcodesTitle.Appearance.Options.UseFont = true;
        lblPresentationBarcodesTitle.Appearance.Options.UseForeColor = true;
        lblPresentationBarcodesTitle.Location = new Point(18, 14);
        lblPresentationBarcodesTitle.Name = "lblPresentationBarcodesTitle";
        lblPresentationBarcodesTitle.Size = new Size(253, 20);
        lblPresentationBarcodesTitle.TabIndex = 0;
        lblPresentationBarcodesTitle.Text = "5. Códigos de barra por presentación";
        // 
        // grdPresentationBarcodes
        // 
        grdPresentationBarcodes.DataSource = presentationBarcodesTable;
        grdPresentationBarcodes.Location = new Point(18, 46);
        grdPresentationBarcodes.MainView = gvPresentationBarcodes;
        grdPresentationBarcodes.Name = "grdPresentationBarcodes";
        grdPresentationBarcodes.RepositoryItems.AddRange(new RepositoryItem[] { repoBarcodePrincipal, repoBarcodeActive });
        grdPresentationBarcodes.Size = new Size(470, 386);
        grdPresentationBarcodes.TabIndex = 1;
        grdPresentationBarcodes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvPresentationBarcodes });
        // 
        // gvPresentationBarcodes
        // 
        gvPresentationBarcodes.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvPresentationBarcodes.Appearance.HeaderPanel.Options.UseFont = true;
        gvPresentationBarcodes.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvPresentationBarcodes.Appearance.Row.Options.UseFont = true;
        gvPresentationBarcodes.Columns.AddRange(new GridColumn[] { colBarcodeValue, colBarcodeScope, colBarcodePresentation, colBarcodeUnit, colBarcodeFactor, colBarcodePrincipal, colBarcodeActive });
        gvPresentationBarcodes.GridControl = grdPresentationBarcodes;
        gvPresentationBarcodes.Name = "gvPresentationBarcodes";
        gvPresentationBarcodes.OptionsView.ShowGroupPanel = false;
        // 
        // colBarcodeValue
        // 
        colBarcodeValue.Caption = "Código de barras";
        colBarcodeValue.FieldName = "CodigoBarras";
        colBarcodeValue.Name = "colBarcodeValue";
        colBarcodeValue.Visible = true;
        colBarcodeValue.VisibleIndex = 0;
        colBarcodeValue.Width = 118;
        // 
        // colBarcodeScope
        // 
        colBarcodeScope.Caption = "Alcance";
        colBarcodeScope.FieldName = "Alcance";
        colBarcodeScope.Name = "colBarcodeScope";
        colBarcodeScope.Visible = true;
        colBarcodeScope.VisibleIndex = 1;
        colBarcodeScope.Width = 60;
        // 
        // colBarcodePresentation
        // 
        colBarcodePresentation.Caption = "Presentación";
        colBarcodePresentation.FieldName = "Presentacion";
        colBarcodePresentation.Name = "colBarcodePresentation";
        colBarcodePresentation.Visible = true;
        colBarcodePresentation.VisibleIndex = 2;
        colBarcodePresentation.Width = 130;
        // 
        // colBarcodeUnit
        // 
        colBarcodeUnit.Caption = "Unidad";
        colBarcodeUnit.FieldName = "Unidad";
        colBarcodeUnit.Name = "colBarcodeUnit";
        colBarcodeUnit.Visible = true;
        colBarcodeUnit.VisibleIndex = 3;
        colBarcodeUnit.Width = 55;
        // 
        // colBarcodeFactor
        // 
        colBarcodeFactor.Caption = "Factor";
        colBarcodeFactor.FieldName = "FactorInventario";
        colBarcodeFactor.Name = "colBarcodeFactor";
        colBarcodeFactor.Visible = true;
        colBarcodeFactor.VisibleIndex = 4;
        colBarcodeFactor.Width = 58;
        // 
        // colBarcodePrincipal
        // 
        colBarcodePrincipal.Caption = "Ppal.";
        colBarcodePrincipal.ColumnEdit = repoBarcodePrincipal;
        colBarcodePrincipal.FieldName = "Principal";
        colBarcodePrincipal.Name = "colBarcodePrincipal";
        colBarcodePrincipal.Visible = true;
        colBarcodePrincipal.VisibleIndex = 5;
        colBarcodePrincipal.Width = 45;
        // 
        // repoBarcodePrincipal
        // 
        repoBarcodePrincipal.AutoHeight = false;
        repoBarcodePrincipal.Name = "repoBarcodePrincipal";
        // 
        // colBarcodeActive
        // 
        colBarcodeActive.Caption = "Activa";
        colBarcodeActive.ColumnEdit = repoBarcodeActive;
        colBarcodeActive.FieldName = "Activa";
        colBarcodeActive.Name = "colBarcodeActive";
        colBarcodeActive.Visible = true;
        colBarcodeActive.VisibleIndex = 6;
        colBarcodeActive.Width = 45;
        // 
        // repoBarcodeActive
        // 
        repoBarcodeActive.AutoHeight = false;
        repoBarcodeActive.Name = "repoBarcodeActive";
        // 
        // btnAddBarcode
        // 
        btnAddBarcode.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddBarcode.Appearance.Options.UseFont = true;
        btnAddBarcode.Location = new Point(18, 448);
        btnAddBarcode.Name = "btnAddBarcode";
        btnAddBarcode.Size = new Size(78, 26);
        btnAddBarcode.TabIndex = 2;
        btnAddBarcode.Text = "Agregar";
        // 
        // btnUpdateBarcode
        // 
        btnUpdateBarcode.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnUpdateBarcode.Appearance.Options.UseFont = true;
        btnUpdateBarcode.Location = new Point(104, 448);
        btnUpdateBarcode.Name = "btnUpdateBarcode";
        btnUpdateBarcode.Size = new Size(86, 26);
        btnUpdateBarcode.TabIndex = 3;
        btnUpdateBarcode.Text = "Actualizar";
        // 
        // btnRemoveBarcode
        // 
        btnRemoveBarcode.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRemoveBarcode.Appearance.Options.UseFont = true;
        btnRemoveBarcode.Location = new Point(198, 448);
        btnRemoveBarcode.Name = "btnRemoveBarcode";
        btnRemoveBarcode.Size = new Size(72, 26);
        btnRemoveBarcode.TabIndex = 4;
        btnRemoveBarcode.Text = "Quitar";
        // 
        // btnSetMainBarcode
        // 
        btnSetMainBarcode.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetMainBarcode.Appearance.Options.UseFont = true;
        btnSetMainBarcode.Location = new Point(278, 448);
        btnSetMainBarcode.Name = "btnSetMainBarcode";
        btnSetMainBarcode.Size = new Size(130, 26);
        btnSetMainBarcode.TabIndex = 5;
        btnSetMainBarcode.Text = "Marcar principal";
        // 
        // tabRemarks
        // 
        tabRemarks.Controls.Add(pnlNotesGeneral);
        tabRemarks.Controls.Add(pnlNotesProcess);
        tabRemarks.Controls.Add(pnlNotesAlerts);
        tabRemarks.Name = "tabRemarks";
        tabRemarks.Size = new Size(1418, 537);
        tabRemarks.Text = "Observaciones";
        // 
        // pnlNotesAlerts
        // 
        pnlNotesAlerts.Controls.Add(lblNotesAlertsTitle);
        pnlNotesAlerts.Controls.Add(grdOperationalAlerts);
        pnlNotesAlerts.Controls.Add(btnAddOperationalAlert);
        pnlNotesAlerts.Controls.Add(btnUpdateOperationalAlert);
        pnlNotesAlerts.Controls.Add(btnRemoveOperationalAlert);
        pnlNotesAlerts.Controls.Add(btnClearOperationalAlert);
        pnlNotesAlerts.Location = new Point(18, 278);
        pnlNotesAlerts.Name = "pnlNotesAlerts";
        pnlNotesAlerts.Size = new Size(1382, 230);
        pnlNotesAlerts.TabIndex = 2;
        // 
        // btnClearOperationalAlert
        // 
        btnClearOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        btnClearOperationalAlert.Appearance.Options.UseFont = true;
        btnClearOperationalAlert.Location = new Point(358, 188);
        btnClearOperationalAlert.Name = "btnClearOperationalAlert";
        btnClearOperationalAlert.Size = new Size(92, 28);
        btnClearOperationalAlert.TabIndex = 5;
        btnClearOperationalAlert.Text = "Limpiar";
        // 
        // btnRemoveOperationalAlert
        // 
        btnRemoveOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        btnRemoveOperationalAlert.Appearance.Options.UseFont = true;
        btnRemoveOperationalAlert.Location = new Point(250, 188);
        btnRemoveOperationalAlert.Name = "btnRemoveOperationalAlert";
        btnRemoveOperationalAlert.Size = new Size(92, 28);
        btnRemoveOperationalAlert.TabIndex = 4;
        btnRemoveOperationalAlert.Text = "Quitar";
        // 
        // btnUpdateOperationalAlert
        // 
        btnUpdateOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        btnUpdateOperationalAlert.Appearance.Options.UseFont = true;
        btnUpdateOperationalAlert.Location = new Point(130, 188);
        btnUpdateOperationalAlert.Name = "btnUpdateOperationalAlert";
        btnUpdateOperationalAlert.Size = new Size(104, 28);
        btnUpdateOperationalAlert.TabIndex = 3;
        btnUpdateOperationalAlert.Text = "Actualizar";
        // 
        // btnAddOperationalAlert
        // 
        btnAddOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        btnAddOperationalAlert.Appearance.Options.UseFont = true;
        btnAddOperationalAlert.Location = new Point(18, 188);
        btnAddOperationalAlert.Name = "btnAddOperationalAlert";
        btnAddOperationalAlert.Size = new Size(96, 28);
        btnAddOperationalAlert.TabIndex = 2;
        btnAddOperationalAlert.Text = "Agregar";
        // 
        // grdOperationalAlerts
        // 
        grdOperationalAlerts.DataSource = operationalAlertsTable;
        grdOperationalAlerts.Location = new Point(18, 44);
        grdOperationalAlerts.MainView = gvOperationalAlerts;
        grdOperationalAlerts.Name = "grdOperationalAlerts";
        grdOperationalAlerts.RepositoryItems.AddRange(new RepositoryItem[] { repoOperationalAlertCheck });
        grdOperationalAlerts.Size = new Size(1346, 128);
        grdOperationalAlerts.TabIndex = 1;
        grdOperationalAlerts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvOperationalAlerts });
        // 
        // gridView1
        // 
        gridView1.GridControl = grdOperationalAlerts;
        gridView1.Name = "gridView1";
        // 
        // repoOperationalAlertCheck
        // 
        repoOperationalAlertCheck.AutoHeight = false;
        repoOperationalAlertCheck.Name = "repoOperationalAlertCheck";
        // 
        // gvOperationalAlerts
        // 
        gvOperationalAlerts.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvOperationalAlerts.Appearance.HeaderPanel.Options.UseFont = true;
        gvOperationalAlerts.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvOperationalAlerts.Appearance.Row.Options.UseFont = true;
        gvOperationalAlerts.Columns.AddRange(new GridColumn[] { colOperationalAlertType, colOperationalAlertProcess, colOperationalAlertMessage, colOperationalAlertFrom, colOperationalAlertTo, colOperationalAlertBlocking, colOperationalAlertActive });
        gvOperationalAlerts.GridControl = grdOperationalAlerts;
        gvOperationalAlerts.Name = "gvOperationalAlerts";
        gvOperationalAlerts.OptionsBehavior.Editable = false;
        gvOperationalAlerts.OptionsView.ShowGroupPanel = false;
        gvOperationalAlerts.OptionsView.ShowIndicator = false;
        // 
        // colOperationalAlertType
        // 
        colOperationalAlertType.Caption = "Tipo alerta";
        colOperationalAlertType.FieldName = "TipoAlerta";
        colOperationalAlertType.Name = "colOperationalAlertType";
        colOperationalAlertType.Visible = true;
        colOperationalAlertType.VisibleIndex = 0;
        colOperationalAlertType.Width = 150;
        // 
        // colOperationalAlertProcess
        // 
        colOperationalAlertProcess.Caption = "Proceso";
        colOperationalAlertProcess.FieldName = "Proceso";
        colOperationalAlertProcess.Name = "colOperationalAlertProcess";
        colOperationalAlertProcess.Visible = true;
        colOperationalAlertProcess.VisibleIndex = 1;
        colOperationalAlertProcess.Width = 120;
        // 
        // colOperationalAlertMessage
        // 
        colOperationalAlertMessage.Caption = "Mensaje";
        colOperationalAlertMessage.FieldName = "Mensaje";
        colOperationalAlertMessage.Name = "colOperationalAlertMessage";
        colOperationalAlertMessage.Visible = true;
        colOperationalAlertMessage.VisibleIndex = 2;
        colOperationalAlertMessage.Width = 430;
        // 
        // colOperationalAlertFrom
        // 
        colOperationalAlertFrom.Caption = "Desde";
        colOperationalAlertFrom.FieldName = "Desde";
        colOperationalAlertFrom.Name = "colOperationalAlertFrom";
        colOperationalAlertFrom.Visible = true;
        colOperationalAlertFrom.VisibleIndex = 3;
        colOperationalAlertFrom.Width = 110;
        // 
        // colOperationalAlertTo
        // 
        colOperationalAlertTo.Caption = "Hasta";
        colOperationalAlertTo.FieldName = "Hasta";
        colOperationalAlertTo.Name = "colOperationalAlertTo";
        colOperationalAlertTo.Visible = true;
        colOperationalAlertTo.VisibleIndex = 4;
        colOperationalAlertTo.Width = 110;
        // 
        // colOperationalAlertBlocking
        // 
        colOperationalAlertBlocking.Caption = "Bloqueante";
        colOperationalAlertBlocking.ColumnEdit = repoOperationalAlertCheck;
        colOperationalAlertBlocking.FieldName = "Bloqueante";
        colOperationalAlertBlocking.Name = "colOperationalAlertBlocking";
        colOperationalAlertBlocking.Visible = true;
        colOperationalAlertBlocking.VisibleIndex = 5;
        colOperationalAlertBlocking.Width = 90;
        // 
        // colOperationalAlertActive
        // 
        colOperationalAlertActive.Caption = "Activa";
        colOperationalAlertActive.ColumnEdit = repoOperationalAlertCheck;
        colOperationalAlertActive.FieldName = "Activa";
        colOperationalAlertActive.Name = "colOperationalAlertActive";
        colOperationalAlertActive.Visible = true;
        colOperationalAlertActive.VisibleIndex = 6;
        colOperationalAlertActive.Width = 70;
        // 
        // lblNotesAlertsTitle
        // 
        lblNotesAlertsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblNotesAlertsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblNotesAlertsTitle.Appearance.Options.UseFont = true;
        lblNotesAlertsTitle.Appearance.Options.UseForeColor = true;
        lblNotesAlertsTitle.Location = new Point(18, 12);
        lblNotesAlertsTitle.Name = "lblNotesAlertsTitle";
        lblNotesAlertsTitle.Size = new Size(138, 20);
        lblNotesAlertsTitle.TabIndex = 0;
        lblNotesAlertsTitle.Text = "3. Alertas operativas";
        // 
        // pnlNotesProcess
        // 
        pnlNotesProcess.Controls.Add(lblNotesProcessTitle);
        pnlNotesProcess.Controls.Add(lblPurchaseNotes);
        pnlNotesProcess.Controls.Add(memPurchaseNotes);
        pnlNotesProcess.Controls.Add(lblSalesNotes);
        pnlNotesProcess.Controls.Add(memSalesNotes);
        pnlNotesProcess.Controls.Add(lblInventoryNotes);
        pnlNotesProcess.Controls.Add(memInventoryNotes);
        pnlNotesProcess.Controls.Add(lblLogisticsQualityNotes);
        pnlNotesProcess.Controls.Add(memLogisticsQualityNotes);
        pnlNotesProcess.Location = new Point(445, 18);
        pnlNotesProcess.Name = "pnlNotesProcess";
        pnlNotesProcess.Size = new Size(955, 245);
        pnlNotesProcess.TabIndex = 1;
        // 
        // memLogisticsQualityNotes
        // 
        memLogisticsQualityNotes.EditValue = "No apilar más de 8 bultos por estiba. Revisar integridad del empaque en cada recepción.";
        memLogisticsQualityNotes.Location = new Point(486, 158);
        memLogisticsQualityNotes.Name = "memLogisticsQualityNotes";
        memLogisticsQualityNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memLogisticsQualityNotes.Properties.Appearance.Options.UseFont = true;
        memLogisticsQualityNotes.Size = new Size(420, 60);
        memLogisticsQualityNotes.TabIndex = 8;
        // 
        // lblLogisticsQualityNotes
        // 
        lblLogisticsQualityNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblLogisticsQualityNotes.Appearance.Options.UseFont = true;
        lblLogisticsQualityNotes.Location = new Point(486, 138);
        lblLogisticsQualityNotes.Name = "lblLogisticsQualityNotes";
        lblLogisticsQualityNotes.Size = new Size(101, 15);
        lblLogisticsQualityNotes.TabIndex = 7;
        lblLogisticsQualityNotes.Text = "Logística / Calidad:";
        // 
        // memInventoryNotes
        // 
        memInventoryNotes.EditValue = "Almacenar en lugar fresco y seco. Controlar humedad relativa menor al 65%.";
        memInventoryNotes.Location = new Point(20, 158);
        memInventoryNotes.Name = "memInventoryNotes";
        memInventoryNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memInventoryNotes.Properties.Appearance.Options.UseFont = true;
        memInventoryNotes.Size = new Size(420, 60);
        memInventoryNotes.TabIndex = 6;
        // 
        // lblInventoryNotes
        // 
        lblInventoryNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblInventoryNotes.Appearance.Options.UseFont = true;
        lblInventoryNotes.Location = new Point(20, 138);
        lblInventoryNotes.Name = "lblInventoryNotes";
        lblInventoryNotes.Size = new Size(56, 15);
        lblInventoryNotes.TabIndex = 5;
        lblInventoryNotes.Text = "Inventario:";
        // 
        // memSalesNotes
        // 
        memSalesNotes.EditValue = "Usar en promociones. No vender productos con vencimiento menor a 30 días.";
        memSalesNotes.Location = new Point(486, 66);
        memSalesNotes.Name = "memSalesNotes";
        memSalesNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memSalesNotes.Properties.Appearance.Options.UseFont = true;
        memSalesNotes.Size = new Size(420, 60);
        memSalesNotes.TabIndex = 4;
        // 
        // lblSalesNotes
        // 
        lblSalesNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesNotes.Appearance.Options.UseFont = true;
        lblSalesNotes.Location = new Point(486, 46);
        lblSalesNotes.Name = "lblSalesNotes";
        lblSalesNotes.Size = new Size(38, 15);
        lblSalesNotes.TabIndex = 3;
        lblSalesNotes.Text = "Ventas:";
        // 
        // memPurchaseNotes
        // 
        memPurchaseNotes.EditValue = "Comprar solo a proveedor certificado y exigir vida útil mínima de 6 meses al momento de la entrega.";
        memPurchaseNotes.Location = new Point(20, 66);
        memPurchaseNotes.Name = "memPurchaseNotes";
        memPurchaseNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memPurchaseNotes.Properties.Appearance.Options.UseFont = true;
        memPurchaseNotes.Size = new Size(420, 60);
        memPurchaseNotes.TabIndex = 2;
        // 
        // lblPurchaseNotes
        // 
        lblPurchaseNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseNotes.Appearance.Options.UseFont = true;
        lblPurchaseNotes.Location = new Point(20, 46);
        lblPurchaseNotes.Name = "lblPurchaseNotes";
        lblPurchaseNotes.Size = new Size(51, 15);
        lblPurchaseNotes.TabIndex = 1;
        lblPurchaseNotes.Text = "Compras:";
        // 
        // lblNotesProcessTitle
        // 
        lblNotesProcessTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblNotesProcessTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblNotesProcessTitle.Appearance.Options.UseFont = true;
        lblNotesProcessTitle.Appearance.Options.UseForeColor = true;
        lblNotesProcessTitle.Location = new Point(18, 14);
        lblNotesProcessTitle.Name = "lblNotesProcessTitle";
        lblNotesProcessTitle.Size = new Size(201, 20);
        lblNotesProcessTitle.TabIndex = 0;
        lblNotesProcessTitle.Text = "2. Observaciones por proceso";
        // 
        // pnlNotesGeneral
        // 
        pnlNotesGeneral.Controls.Add(lblNotesGeneralTitle);
        pnlNotesGeneral.Controls.Add(lblGeneralNotes);
        pnlNotesGeneral.Controls.Add(memGeneralNotes);
        pnlNotesGeneral.Controls.Add(lblGeneralOperationalAlert);
        pnlNotesGeneral.Controls.Add(memGeneralOperationalAlert);
        pnlNotesGeneral.Controls.Add(lblNotePriority);
        pnlNotesGeneral.Controls.Add(lueNotePriority);
        pnlNotesGeneral.Controls.Add(chkGeneralNoteActive);
        pnlNotesGeneral.Location = new Point(18, 18);
        pnlNotesGeneral.Name = "pnlNotesGeneral";
        pnlNotesGeneral.Size = new Size(410, 245);
        pnlNotesGeneral.TabIndex = 0;
        // 
        // chkGeneralNoteActive
        // 
        chkGeneralNoteActive.EditValue = true;
        chkGeneralNoteActive.Location = new Point(246, 218);
        chkGeneralNoteActive.Name = "chkGeneralNoteActive";
        chkGeneralNoteActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkGeneralNoteActive.Properties.Appearance.Options.UseFont = true;
        chkGeneralNoteActive.Properties.Caption = "Vigente";
        chkGeneralNoteActive.Size = new Size(86, 20);
        chkGeneralNoteActive.TabIndex = 7;
        // 
        // lueNotePriority
        // 
        lueNotePriority.EditValue = "Media";
        lueNotePriority.Location = new Point(84, 218);
        lueNotePriority.Name = "lueNotePriority";
        lueNotePriority.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueNotePriority.Properties.Appearance.Options.UseFont = true;
        lueNotePriority.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueNotePriority.Properties.NullText = "";
        lueNotePriority.Size = new Size(124, 22);
        lueNotePriority.TabIndex = 6;
        // 
        // lblNotePriority
        // 
        lblNotePriority.Appearance.Font = new Font("Segoe UI", 9F);
        lblNotePriority.Appearance.Options.UseFont = true;
        lblNotePriority.Location = new Point(16, 222);
        lblNotePriority.Name = "lblNotePriority";
        lblNotePriority.Size = new Size(51, 15);
        lblNotePriority.TabIndex = 5;
        lblNotePriority.Text = "Prioridad:";
        // 
        // memGeneralOperationalAlert
        // 
        memGeneralOperationalAlert.EditValue = "Validar lote y fecha de vencimiento antes de despachar.";
        memGeneralOperationalAlert.Location = new Point(16, 170);
        memGeneralOperationalAlert.Name = "memGeneralOperationalAlert";
        memGeneralOperationalAlert.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memGeneralOperationalAlert.Properties.Appearance.Options.UseFont = true;
        memGeneralOperationalAlert.Size = new Size(376, 44);
        memGeneralOperationalAlert.TabIndex = 4;
        // 
        // lblGeneralOperationalAlert
        // 
        lblGeneralOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        lblGeneralOperationalAlert.Appearance.Options.UseFont = true;
        lblGeneralOperationalAlert.Location = new Point(16, 150);
        lblGeneralOperationalAlert.Name = "lblGeneralOperationalAlert";
        lblGeneralOperationalAlert.Size = new Size(128, 15);
        lblGeneralOperationalAlert.TabIndex = 3;
        lblGeneralOperationalAlert.Text = "Alerta operativa general:";
        // 
        // memGeneralNotes
        // 
        memGeneralNotes.EditValue = "Producto de alta rotación. Mantener disponibilidad mínima en bodegas principales. Revisar calidad visual del empaque en recepción.";
        memGeneralNotes.Location = new Point(16, 66);
        memGeneralNotes.Name = "memGeneralNotes";
        memGeneralNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memGeneralNotes.Properties.Appearance.Options.UseFont = true;
        memGeneralNotes.Size = new Size(376, 76);
        memGeneralNotes.TabIndex = 2;
        // 
        // lblGeneralNotes
        // 
        lblGeneralNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblGeneralNotes.Appearance.Options.UseFont = true;
        lblGeneralNotes.Location = new Point(16, 46);
        lblGeneralNotes.Name = "lblGeneralNotes";
        lblGeneralNotes.Size = new Size(133, 15);
        lblGeneralNotes.TabIndex = 1;
        lblGeneralNotes.Text = "Observaciones generales:";
        // 
        // lblNotesGeneralTitle
        // 
        lblNotesGeneralTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblNotesGeneralTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblNotesGeneralTitle.Appearance.Options.UseFont = true;
        lblNotesGeneralTitle.Appearance.Options.UseForeColor = true;
        lblNotesGeneralTitle.Location = new Point(16, 14);
        lblNotesGeneralTitle.Name = "lblNotesGeneralTitle";
        lblNotesGeneralTitle.Size = new Size(183, 20);
        lblNotesGeneralTitle.TabIndex = 0;
        lblNotesGeneralTitle.Text = "1. Observaciones generales";
        // 
        // tabAttachments
        // 
        tabAttachments.Controls.Add(lblAttachmentGridTitle);
        tabAttachments.Controls.Add(grdAttachments);
        tabAttachments.Controls.Add(btnAddAttachment);
        tabAttachments.Controls.Add(btnUpdateAttachment);
        tabAttachments.Controls.Add(btnRemoveAttachment);
        tabAttachments.Controls.Add(btnDownloadAttachment);
        tabAttachments.Controls.Add(btnOpenAttachment);
        tabAttachments.Controls.Add(btnSetMainAttachment);
        tabAttachments.Controls.Add(lblAttachmentMetadataTitle);
        tabAttachments.Controls.Add(lblAttachmentType);
        tabAttachments.Controls.Add(lueAttachmentType);
        tabAttachments.Controls.Add(lblAttachmentFileName);
        tabAttachments.Controls.Add(txtAttachmentFileName);
        tabAttachments.Controls.Add(lblAttachmentDescription);
        tabAttachments.Controls.Add(memAttachmentDescription);
        tabAttachments.Controls.Add(lblAttachmentCategory);
        tabAttachments.Controls.Add(lueAttachmentCategory);
        tabAttachments.Controls.Add(chkVisibleInSales);
        tabAttachments.Controls.Add(chkVisibleInPurchases);
        tabAttachments.Controls.Add(chkVisibleInPortal);
        tabAttachments.Controls.Add(lblAttachmentStatus);
        tabAttachments.Controls.Add(lueAttachmentStatus);
        tabAttachments.Controls.Add(lblAttachmentExtension);
        tabAttachments.Controls.Add(txtAttachmentExtension);
        tabAttachments.Controls.Add(lblAttachmentSize);
        tabAttachments.Controls.Add(txtAttachmentSize);
        tabAttachments.Controls.Add(lblAttachmentUploadedAt);
        tabAttachments.Controls.Add(dteAttachmentUploadedAt);
        tabAttachments.Controls.Add(lblAttachmentUser);
        tabAttachments.Controls.Add(txtAttachmentUser);
        tabAttachments.Controls.Add(lblAttachmentPreviewTitle);
        tabAttachments.Controls.Add(picMainAttachmentPreview);
        tabAttachments.Controls.Add(btnLoadImage);
        tabAttachments.Controls.Add(btnRemoveImage);
        tabAttachments.Controls.Add(btnPreviewImage);
        tabAttachments.Controls.Add(btnSetMainImage);
        tabAttachments.Controls.Add(pnlAttachmentPreviewNote);
        tabAttachments.Name = "tabAttachments";
        tabAttachments.Size = new Size(1418, 537);
        tabAttachments.Text = "Imágenes / Anexos";
        // 
        // gridView2
        // 
        gridView2.Name = "gridView2";
        // 
        // tabSap
        // 
        tabSap.Controls.Add(btnClearSapFields);
        tabSap.Controls.Add(btnRemoveSapField);
        tabSap.Controls.Add(btnUpdateSapField);
        tabSap.Controls.Add(btnAddSapField);
        tabSap.Controls.Add(grdSapFieldMapping);
        tabSap.Controls.Add(lblSapFieldMappingTitle);
        tabSap.Controls.Add(lblSapMapEnabled);
        tabSap.Controls.Add(lblSapMapRequired);
        tabSap.Controls.Add(lueSapMapEnabled);
        tabSap.Controls.Add(lblSapMapDescription);
        tabSap.Controls.Add(lueSapMapRequired);
        tabSap.Controls.Add(lblSapMapSapField);
        tabSap.Controls.Add(txtSapMapDescription);
        tabSap.Controls.Add(lblSapMapSystemField);
        tabSap.Controls.Add(txtSapMapSapField);
        tabSap.Controls.Add(lblSapHistoryTitle);
        tabSap.Controls.Add(txtSapMapSystemField);
        tabSap.Controls.Add(grdSapSyncHistory);
        tabSap.Controls.Add(lblSapSyncAsSupplier);
        tabSap.Controls.Add(lblSapMode);
        tabSap.Controls.Add(lueSapSyncAsSupplier);
        tabSap.Controls.Add(lblSapConfigTitle);
        tabSap.Controls.Add(lblSapManualRetry);
        tabSap.Controls.Add(lueSapMode);
        tabSap.Controls.Add(lueSapManualRetry);
        tabSap.Controls.Add(lblSapCompany);
        tabSap.Controls.Add(lblSapRequiresApproval);
        tabSap.Controls.Add(lblSapStatusTitle);
        tabSap.Controls.Add(lueSapRequiresApproval);
        tabSap.Controls.Add(lueSapCompany);
        tabSap.Controls.Add(lblSapSyncStatus);
        tabSap.Controls.Add(lueSapSyncStatus);
        tabSap.Controls.Add(lblSapLastSync);
        tabSap.Controls.Add(txtSapLastSync);
        tabSap.Controls.Add(lblSapLastError);
        tabSap.Controls.Add(txtSapLastError);
        tabSap.Controls.Add(lblSapRetryCount);
        tabSap.Controls.Add(txtSapRetryCount);
        tabSap.Controls.Add(lblSapEnabled);
        tabSap.Controls.Add(lueSapEnabled);
        tabSap.Name = "tabSap";
        tabSap.Size = new Size(1418, 537);
        tabSap.Text = "SAP";
        // 
        // gridView3
        // 
        gridView3.Name = "gridView3";
        // 
        // gridView4
        // 
        gridView4.Name = "gridView4";
        // 
        // gridView5
        // 
        gridView5.Name = "gridView5";
        // 
        // gridView6
        // 
        gridView6.Name = "gridView6";
        // 
        // gridView7
        // 
        gridView7.Name = "gridView7";
        // 
        // tabLots
        // 
        tabLots.Controls.Add(lblLotOperationalRulesTitle);
        tabLots.Controls.Add(lblIssueMethod);
        tabLots.Controls.Add(lueIssueMethod);
        tabLots.Controls.Add(lblAllowMultipleBatches);
        tabLots.Controls.Add(tglAllowMultipleBatches);
        tabLots.Controls.Add(lblAllowExpiredBatchSale);
        tabLots.Controls.Add(tglAllowExpiredBatchSale);
        tabLots.Controls.Add(lblBlockQuarantineBatch);
        tabLots.Controls.Add(tglBlockQuarantineBatch);
        tabLots.Controls.Add(lblBlockExpiredBatch);
        tabLots.Controls.Add(tglBlockExpiredBatch);
        tabLots.Controls.Add(lblLotOperationalNotes);
        tabLots.Controls.Add(memLotOperationalNotes);
        tabLots.Controls.Add(pnlLotOperationalNote);
        tabLots.Controls.Add(lblLotTraceabilityTitle);
        tabLots.Controls.Add(lblRequiresExpiration);
        tabLots.Controls.Add(tglRequiresExpiration);
        tabLots.Controls.Add(lblExpirationMandatory);
        tabLots.Controls.Add(tglExpirationMandatory);
        tabLots.Controls.Add(lblAutoGenerateBatch);
        tabLots.Controls.Add(tglAutoGenerateBatch);
        tabLots.Controls.Add(lblBatchPrefix);
        tabLots.Controls.Add(txtBatchPrefix);
        tabLots.Controls.Add(lblSerialLength);
        tabLots.Controls.Add(spnSerialLength);
        tabLots.Controls.Add(lblShelfLifeDays);
        tabLots.Controls.Add(spnShelfLifeDays);
        tabLots.Controls.Add(lblExpirationAlertDays);
        tabLots.Controls.Add(spnExpirationAlertDays);
        tabLots.Controls.Add(lblQuarantineDays);
        tabLots.Controls.Add(spnQuarantineDays);
        tabLots.Controls.Add(lblBatchFormat);
        tabLots.Controls.Add(txtBatchFormat);
        tabLots.Controls.Add(lblNumberingMethod);
        tabLots.Controls.Add(lueNumberingMethod);
        tabLots.Controls.Add(pnlLotTraceabilityNote);
        tabLots.Name = "tabLots";
        tabLots.Size = new Size(1418, 537);
        tabLots.Text = "Lotes / Series";
        // 
        // pnlLotTraceabilityNote
        // 
        pnlLotTraceabilityNote.Appearance.BackColor = Color.FromArgb(238, 248, 255);
        pnlLotTraceabilityNote.Appearance.Options.UseBackColor = true;
        pnlLotTraceabilityNote.Controls.Add(lblLotTraceabilityNoteIcon);
        pnlLotTraceabilityNote.Controls.Add(lblLotTraceabilityNote);
        pnlLotTraceabilityNote.Location = new Point(18, 330);
        pnlLotTraceabilityNote.Name = "pnlLotTraceabilityNote";
        pnlLotTraceabilityNote.Size = new Size(304, 34);
        pnlLotTraceabilityNote.TabIndex = 51;
        // 
        // lblLotTraceabilityNote
        // 
        lblLotTraceabilityNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblLotTraceabilityNote.Appearance.ForeColor = Color.FromArgb(31, 42, 68);
        lblLotTraceabilityNote.Appearance.Options.UseFont = true;
        lblLotTraceabilityNote.Appearance.Options.UseForeColor = true;
        lblLotTraceabilityNote.AutoSizeMode = LabelAutoSizeMode.Vertical;
        lblLotTraceabilityNote.Location = new Point(38, 5);
        lblLotTraceabilityNote.Name = "lblLotTraceabilityNote";
        lblLotTraceabilityNote.Size = new Size(252, 26);
        lblLotTraceabilityNote.TabIndex = 1;
        lblLotTraceabilityNote.Text = "Permite identificar origen, vencimiento, lote o serie de cada movimiento.";
        // 
        // lblLotTraceabilityNoteIcon
        // 
        lblLotTraceabilityNoteIcon.Appearance.BackColor = Color.FromArgb(0, 122, 204);
        lblLotTraceabilityNoteIcon.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblLotTraceabilityNoteIcon.Appearance.ForeColor = Color.White;
        lblLotTraceabilityNoteIcon.Appearance.Options.UseBackColor = true;
        lblLotTraceabilityNoteIcon.Appearance.Options.UseFont = true;
        lblLotTraceabilityNoteIcon.Appearance.Options.UseForeColor = true;
        lblLotTraceabilityNoteIcon.Appearance.Options.UseTextOptions = true;
        lblLotTraceabilityNoteIcon.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        lblLotTraceabilityNoteIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblLotTraceabilityNoteIcon.Location = new Point(10, 8);
        lblLotTraceabilityNoteIcon.Name = "lblLotTraceabilityNoteIcon";
        lblLotTraceabilityNoteIcon.Size = new Size(18, 18);
        lblLotTraceabilityNoteIcon.TabIndex = 0;
        lblLotTraceabilityNoteIcon.Text = "i";
        // 
        // lueNumberingMethod
        // 
        lueNumberingMethod.EditValue = "Automático por recepción";
        lueNumberingMethod.Location = new Point(165, 301);
        lueNumberingMethod.Name = "lueNumberingMethod";
        lueNumberingMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueNumberingMethod.Properties.Appearance.Options.UseFont = true;
        lueNumberingMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueNumberingMethod.Properties.NullText = "";
        lueNumberingMethod.Size = new Size(148, 22);
        lueNumberingMethod.TabIndex = 50;
        // 
        // lblNumberingMethod
        // 
        lblNumberingMethod.Appearance.Font = new Font("Segoe UI", 9F);
        lblNumberingMethod.Appearance.Options.UseFont = true;
        lblNumberingMethod.Location = new Point(18, 304);
        lblNumberingMethod.Name = "lblNumberingMethod";
        lblNumberingMethod.Size = new Size(112, 15);
        lblNumberingMethod.TabIndex = 49;
        lblNumberingMethod.Text = "Método numeración:";
        // 
        // txtBatchFormat
        // 
        txtBatchFormat.EditValue = "ARZ-{YYYYMM}-{SEQ}";
        txtBatchFormat.Location = new Point(165, 273);
        txtBatchFormat.Name = "txtBatchFormat";
        txtBatchFormat.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBatchFormat.Properties.Appearance.Options.UseFont = true;
        txtBatchFormat.Size = new Size(148, 22);
        txtBatchFormat.TabIndex = 48;
        // 
        // lblBatchFormat
        // 
        lblBatchFormat.Appearance.Font = new Font("Segoe UI", 9F);
        lblBatchFormat.Appearance.Options.UseFont = true;
        lblBatchFormat.Location = new Point(18, 276);
        lblBatchFormat.Name = "lblBatchFormat";
        lblBatchFormat.Size = new Size(71, 15);
        lblBatchFormat.TabIndex = 47;
        lblBatchFormat.Text = "Formato lote:";
        // 
        // spnQuarantineDays
        // 
        spnQuarantineDays.EditValue = new decimal(new int[] { 2, 0, 0, 0 });
        spnQuarantineDays.Location = new Point(165, 245);
        spnQuarantineDays.Name = "spnQuarantineDays";
        spnQuarantineDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnQuarantineDays.Properties.Appearance.Options.UseFont = true;
        spnQuarantineDays.Properties.Appearance.Options.UseTextOptions = true;
        spnQuarantineDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnQuarantineDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnQuarantineDays.Properties.IsFloatValue = false;
        spnQuarantineDays.Properties.MaskSettings.Set("mask", "N00");
        spnQuarantineDays.Size = new Size(148, 22);
        spnQuarantineDays.TabIndex = 46;
        // 
        // lblQuarantineDays
        // 
        lblQuarantineDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblQuarantineDays.Appearance.Options.UseFont = true;
        lblQuarantineDays.Location = new Point(17, 248);
        lblQuarantineDays.Name = "lblQuarantineDays";
        lblQuarantineDays.Size = new Size(87, 15);
        lblQuarantineDays.TabIndex = 45;
        lblQuarantineDays.Text = "Días cuarentena:";
        // 
        // spnExpirationAlertDays
        // 
        spnExpirationAlertDays.EditValue = new decimal(new int[] { 30, 0, 0, 0 });
        spnExpirationAlertDays.Location = new Point(165, 217);
        spnExpirationAlertDays.Name = "spnExpirationAlertDays";
        spnExpirationAlertDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnExpirationAlertDays.Properties.Appearance.Options.UseFont = true;
        spnExpirationAlertDays.Properties.Appearance.Options.UseTextOptions = true;
        spnExpirationAlertDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnExpirationAlertDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnExpirationAlertDays.Properties.IsFloatValue = false;
        spnExpirationAlertDays.Properties.MaskSettings.Set("mask", "N00");
        spnExpirationAlertDays.Size = new Size(148, 22);
        spnExpirationAlertDays.TabIndex = 44;
        // 
        // lblExpirationAlertDays
        // 
        lblExpirationAlertDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblExpirationAlertDays.Appearance.Options.UseFont = true;
        lblExpirationAlertDays.Location = new Point(17, 220);
        lblExpirationAlertDays.Name = "lblExpirationAlertDays";
        lblExpirationAlertDays.Size = new Size(142, 15);
        lblExpirationAlertDays.TabIndex = 43;
        lblExpirationAlertDays.Text = "Días de alerta vencimiento:";
        // 
        // spnShelfLifeDays
        // 
        spnShelfLifeDays.EditValue = new decimal(new int[] { 180, 0, 0, 0 });
        spnShelfLifeDays.Location = new Point(165, 189);
        spnShelfLifeDays.Name = "spnShelfLifeDays";
        spnShelfLifeDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnShelfLifeDays.Properties.Appearance.Options.UseFont = true;
        spnShelfLifeDays.Properties.Appearance.Options.UseTextOptions = true;
        spnShelfLifeDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnShelfLifeDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnShelfLifeDays.Properties.IsFloatValue = false;
        spnShelfLifeDays.Properties.MaskSettings.Set("mask", "N00");
        spnShelfLifeDays.Size = new Size(148, 22);
        spnShelfLifeDays.TabIndex = 42;
        // 
        // lblShelfLifeDays
        // 
        lblShelfLifeDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblShelfLifeDays.Appearance.Options.UseFont = true;
        lblShelfLifeDays.Location = new Point(17, 192);
        lblShelfLifeDays.Name = "lblShelfLifeDays";
        lblShelfLifeDays.Size = new Size(101, 15);
        lblShelfLifeDays.TabIndex = 41;
        lblShelfLifeDays.Text = "Vida útil lote (días):";
        // 
        // spnSerialLength
        // 
        spnSerialLength.EditValue = new decimal(new int[] { 12, 0, 0, 0 });
        spnSerialLength.Location = new Point(165, 161);
        spnSerialLength.Name = "spnSerialLength";
        spnSerialLength.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSerialLength.Properties.Appearance.Options.UseFont = true;
        spnSerialLength.Properties.Appearance.Options.UseTextOptions = true;
        spnSerialLength.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSerialLength.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSerialLength.Properties.IsFloatValue = false;
        spnSerialLength.Properties.MaskSettings.Set("mask", "N00");
        spnSerialLength.Size = new Size(148, 22);
        spnSerialLength.TabIndex = 40;
        // 
        // lblSerialLength
        // 
        lblSerialLength.Appearance.Font = new Font("Segoe UI", 9F);
        lblSerialLength.Appearance.Options.UseFont = true;
        lblSerialLength.Location = new Point(17, 164);
        lblSerialLength.Name = "lblSerialLength";
        lblSerialLength.Size = new Size(78, 15);
        lblSerialLength.TabIndex = 39;
        lblSerialLength.Text = "Longitud serie:";
        // 
        // txtBatchPrefix
        // 
        txtBatchPrefix.EditValue = "ARZ";
        txtBatchPrefix.Location = new Point(165, 133);
        txtBatchPrefix.Name = "txtBatchPrefix";
        txtBatchPrefix.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBatchPrefix.Properties.Appearance.Options.UseFont = true;
        txtBatchPrefix.Size = new Size(148, 22);
        txtBatchPrefix.TabIndex = 38;
        // 
        // lblBatchPrefix
        // 
        lblBatchPrefix.Appearance.Font = new Font("Segoe UI", 9F);
        lblBatchPrefix.Appearance.Options.UseFont = true;
        lblBatchPrefix.Location = new Point(18, 138);
        lblBatchPrefix.Name = "lblBatchPrefix";
        lblBatchPrefix.Size = new Size(60, 15);
        lblBatchPrefix.TabIndex = 37;
        lblBatchPrefix.Text = "Prefijo lote:";
        // 
        // tglAutoGenerateBatch
        // 
        tglAutoGenerateBatch.EditValue = true;
        tglAutoGenerateBatch.Location = new Point(165, 106);
        tglAutoGenerateBatch.Name = "tglAutoGenerateBatch";
        tglAutoGenerateBatch.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAutoGenerateBatch.Properties.Appearance.Options.UseFont = true;
        tglAutoGenerateBatch.Properties.OffText = "No";
        tglAutoGenerateBatch.Properties.OnText = "Si";
        tglAutoGenerateBatch.Size = new Size(86, 20);
        tglAutoGenerateBatch.TabIndex = 36;
        // 
        // lblAutoGenerateBatch
        // 
        lblAutoGenerateBatch.Appearance.Font = new Font("Segoe UI", 9F);
        lblAutoGenerateBatch.Appearance.Options.UseFont = true;
        lblAutoGenerateBatch.Location = new Point(17, 108);
        lblAutoGenerateBatch.Name = "lblAutoGenerateBatch";
        lblAutoGenerateBatch.Size = new Size(127, 15);
        lblAutoGenerateBatch.TabIndex = 35;
        lblAutoGenerateBatch.Text = "Genera lote automático:";
        // 
        // tglExpirationMandatory
        // 
        tglExpirationMandatory.EditValue = true;
        tglExpirationMandatory.Location = new Point(165, 76);
        tglExpirationMandatory.Name = "tglExpirationMandatory";
        tglExpirationMandatory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglExpirationMandatory.Properties.Appearance.Options.UseFont = true;
        tglExpirationMandatory.Properties.OffText = "No";
        tglExpirationMandatory.Properties.OnText = "Si";
        tglExpirationMandatory.Size = new Size(86, 20);
        tglExpirationMandatory.TabIndex = 34;
        // 
        // lblExpirationMandatory
        // 
        lblExpirationMandatory.Appearance.Font = new Font("Segoe UI", 9F);
        lblExpirationMandatory.Appearance.Options.UseFont = true;
        lblExpirationMandatory.Location = new Point(18, 78);
        lblExpirationMandatory.Name = "lblExpirationMandatory";
        lblExpirationMandatory.Size = new Size(131, 15);
        lblExpirationMandatory.TabIndex = 33;
        lblExpirationMandatory.Text = "Vencimiento obligatorio:";
        // 
        // tglRequiresExpiration
        // 
        tglRequiresExpiration.EditValue = true;
        tglRequiresExpiration.Location = new Point(165, 48);
        tglRequiresExpiration.Name = "tglRequiresExpiration";
        tglRequiresExpiration.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglRequiresExpiration.Properties.Appearance.Options.UseFont = true;
        tglRequiresExpiration.Properties.OffText = "No";
        tglRequiresExpiration.Properties.OnText = "Si";
        tglRequiresExpiration.Size = new Size(86, 20);
        tglRequiresExpiration.TabIndex = 32;
        // 
        // lblRequiresExpiration
        // 
        lblRequiresExpiration.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiresExpiration.Appearance.Options.UseFont = true;
        lblRequiresExpiration.Location = new Point(18, 50);
        lblRequiresExpiration.Name = "lblRequiresExpiration";
        lblRequiresExpiration.Size = new Size(118, 15);
        lblRequiresExpiration.TabIndex = 31;
        lblRequiresExpiration.Text = "Requiere vencimiento:";
        // 
        // lblLotTraceabilityTitle
        // 
        lblLotTraceabilityTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblLotTraceabilityTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblLotTraceabilityTitle.Appearance.Options.UseFont = true;
        lblLotTraceabilityTitle.Appearance.Options.UseForeColor = true;
        lblLotTraceabilityTitle.Location = new Point(12, 13);
        lblLotTraceabilityTitle.Name = "lblLotTraceabilityTitle";
        lblLotTraceabilityTitle.Size = new Size(217, 20);
        lblLotTraceabilityTitle.TabIndex = 26;
        lblLotTraceabilityTitle.Text = "1. Configuración de trazabilidad";
        // 
        // pnlLotOperationalNote
        // 
        pnlLotOperationalNote.Appearance.BackColor = Color.FromArgb(238, 248, 255);
        pnlLotOperationalNote.Appearance.Options.UseBackColor = true;
        pnlLotOperationalNote.Controls.Add(lblLotOperationalNoteIcon);
        pnlLotOperationalNote.Controls.Add(lblLotOperationalNote);
        pnlLotOperationalNote.Location = new Point(374, 268);
        pnlLotOperationalNote.Name = "pnlLotOperationalNote";
        pnlLotOperationalNote.Size = new Size(346, 26);
        pnlLotOperationalNote.TabIndex = 75;
        // 
        // lblLotOperationalNote
        // 
        lblLotOperationalNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblLotOperationalNote.Appearance.ForeColor = Color.FromArgb(31, 42, 68);
        lblLotOperationalNote.Appearance.Options.UseFont = true;
        lblLotOperationalNote.Appearance.Options.UseForeColor = true;
        lblLotOperationalNote.Location = new Point(38, 6);
        lblLotOperationalNote.Name = "lblLotOperationalNote";
        lblLotOperationalNote.Size = new Size(321, 13);
        lblLotOperationalNote.TabIndex = 1;
        lblLotOperationalNote.Text = "Reglas usadas en documentos y movimientos con trazabilidad.";
        // 
        // lblLotOperationalNoteIcon
        // 
        lblLotOperationalNoteIcon.Appearance.BackColor = Color.FromArgb(0, 122, 204);
        lblLotOperationalNoteIcon.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblLotOperationalNoteIcon.Appearance.ForeColor = Color.White;
        lblLotOperationalNoteIcon.Appearance.Options.UseBackColor = true;
        lblLotOperationalNoteIcon.Appearance.Options.UseFont = true;
        lblLotOperationalNoteIcon.Appearance.Options.UseForeColor = true;
        lblLotOperationalNoteIcon.Appearance.Options.UseTextOptions = true;
        lblLotOperationalNoteIcon.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        lblLotOperationalNoteIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblLotOperationalNoteIcon.Location = new Point(10, 5);
        lblLotOperationalNoteIcon.Name = "lblLotOperationalNoteIcon";
        lblLotOperationalNoteIcon.Size = new Size(18, 18);
        lblLotOperationalNoteIcon.TabIndex = 0;
        lblLotOperationalNoteIcon.Text = "i";
        // 
        // memLotOperationalNotes
        // 
        memLotOperationalNotes.EditValue = "Usar método FEFO para despacho.\r\nNo se permite vender lotes vencidos.\r\nCumplir con días de cuarentena en recepción.";
        memLotOperationalNotes.Location = new Point(374, 212);
        memLotOperationalNotes.Name = "memLotOperationalNotes";
        memLotOperationalNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memLotOperationalNotes.Properties.Appearance.Options.UseFont = true;
        memLotOperationalNotes.Size = new Size(346, 48);
        memLotOperationalNotes.TabIndex = 74;
        // 
        // lblLotOperationalNotes
        // 
        lblLotOperationalNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblLotOperationalNotes.Appearance.Options.UseFont = true;
        lblLotOperationalNotes.Location = new Point(374, 192);
        lblLotOperationalNotes.Name = "lblLotOperationalNotes";
        lblLotOperationalNotes.Size = new Size(121, 15);
        lblLotOperationalNotes.TabIndex = 73;
        lblLotOperationalNotes.Text = "Observación operativa:";
        // 
        // tglBlockExpiredBatch
        // 
        tglBlockExpiredBatch.EditValue = true;
        tglBlockExpiredBatch.Location = new Point(543, 162);
        tglBlockExpiredBatch.Name = "tglBlockExpiredBatch";
        tglBlockExpiredBatch.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglBlockExpiredBatch.Properties.Appearance.Options.UseFont = true;
        tglBlockExpiredBatch.Properties.OffText = "No";
        tglBlockExpiredBatch.Properties.OnText = "Si";
        tglBlockExpiredBatch.Size = new Size(86, 20);
        tglBlockExpiredBatch.TabIndex = 72;
        // 
        // lblBlockExpiredBatch
        // 
        lblBlockExpiredBatch.Appearance.Font = new Font("Segoe UI", 9F);
        lblBlockExpiredBatch.Appearance.Options.UseFont = true;
        lblBlockExpiredBatch.Location = new Point(374, 164);
        lblBlockExpiredBatch.Name = "lblBlockExpiredBatch";
        lblBlockExpiredBatch.Size = new Size(114, 15);
        lblBlockExpiredBatch.TabIndex = 71;
        lblBlockExpiredBatch.Text = "Bloquea lote vencido:";
        // 
        // tglBlockQuarantineBatch
        // 
        tglBlockQuarantineBatch.EditValue = true;
        tglBlockQuarantineBatch.Location = new Point(543, 134);
        tglBlockQuarantineBatch.Name = "tglBlockQuarantineBatch";
        tglBlockQuarantineBatch.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglBlockQuarantineBatch.Properties.Appearance.Options.UseFont = true;
        tglBlockQuarantineBatch.Properties.OffText = "No";
        tglBlockQuarantineBatch.Properties.OnText = "Si";
        tglBlockQuarantineBatch.Size = new Size(86, 20);
        tglBlockQuarantineBatch.TabIndex = 70;
        // 
        // lblBlockQuarantineBatch
        // 
        lblBlockQuarantineBatch.Appearance.Font = new Font("Segoe UI", 9F);
        lblBlockQuarantineBatch.Appearance.Options.UseFont = true;
        lblBlockQuarantineBatch.Location = new Point(374, 136);
        lblBlockQuarantineBatch.Name = "lblBlockQuarantineBatch";
        lblBlockQuarantineBatch.Size = new Size(147, 15);
        lblBlockQuarantineBatch.TabIndex = 69;
        lblBlockQuarantineBatch.Text = "Bloquea lote en cuarentena:";
        // 
        // tglAllowExpiredBatchSale
        // 
        tglAllowExpiredBatchSale.Location = new Point(543, 106);
        tglAllowExpiredBatchSale.Name = "tglAllowExpiredBatchSale";
        tglAllowExpiredBatchSale.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAllowExpiredBatchSale.Properties.Appearance.Options.UseFont = true;
        tglAllowExpiredBatchSale.Properties.OffText = "No";
        tglAllowExpiredBatchSale.Properties.OnText = "Si";
        tglAllowExpiredBatchSale.Size = new Size(86, 20);
        tglAllowExpiredBatchSale.TabIndex = 60;
        // 
        // lblAllowExpiredBatchSale
        // 
        lblAllowExpiredBatchSale.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowExpiredBatchSale.Appearance.Options.UseFont = true;
        lblAllowExpiredBatchSale.Location = new Point(374, 108);
        lblAllowExpiredBatchSale.Name = "lblAllowExpiredBatchSale";
        lblAllowExpiredBatchSale.Size = new Size(144, 15);
        lblAllowExpiredBatchSale.TabIndex = 59;
        lblAllowExpiredBatchSale.Text = "Permite venta lote vencido:";
        // 
        // tglAllowMultipleBatches
        // 
        tglAllowMultipleBatches.EditValue = true;
        tglAllowMultipleBatches.Location = new Point(543, 73);
        tglAllowMultipleBatches.Name = "tglAllowMultipleBatches";
        tglAllowMultipleBatches.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAllowMultipleBatches.Properties.Appearance.Options.UseFont = true;
        tglAllowMultipleBatches.Properties.OffText = "No";
        tglAllowMultipleBatches.Properties.OnText = "Si";
        tglAllowMultipleBatches.Size = new Size(86, 20);
        tglAllowMultipleBatches.TabIndex = 56;
        // 
        // lblAllowMultipleBatches
        // 
        lblAllowMultipleBatches.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowMultipleBatches.Appearance.Options.UseFont = true;
        lblAllowMultipleBatches.Location = new Point(374, 72);
        lblAllowMultipleBatches.Name = "lblAllowMultipleBatches";
        lblAllowMultipleBatches.Size = new Size(119, 30);
        lblAllowMultipleBatches.TabIndex = 55;
        lblAllowMultipleBatches.Text = "Admite múltiples lotes \r\npor documento:";
        // 
        // lueIssueMethod
        // 
        lueIssueMethod.EditValue = "FEFO - Primero en vencer";
        lueIssueMethod.Location = new Point(543, 40);
        lueIssueMethod.Name = "lueIssueMethod";
        lueIssueMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueIssueMethod.Properties.Appearance.Options.UseFont = true;
        lueIssueMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueIssueMethod.Properties.NullText = "";
        lueIssueMethod.Size = new Size(200, 22);
        lueIssueMethod.TabIndex = 54;
        // 
        // lblIssueMethod
        // 
        lblIssueMethod.Appearance.Font = new Font("Segoe UI", 9F);
        lblIssueMethod.Appearance.Options.UseFont = true;
        lblIssueMethod.Location = new Point(374, 47);
        lblIssueMethod.Name = "lblIssueMethod";
        lblIssueMethod.Size = new Size(104, 15);
        lblIssueMethod.TabIndex = 53;
        lblIssueMethod.Text = "Método FEFO/FIFO:";
        // 
        // lblLotOperationalRulesTitle
        // 
        lblLotOperationalRulesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblLotOperationalRulesTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblLotOperationalRulesTitle.Appearance.Options.UseFont = true;
        lblLotOperationalRulesTitle.Appearance.Options.UseForeColor = true;
        lblLotOperationalRulesTitle.Location = new Point(374, 13);
        lblLotOperationalRulesTitle.Name = "lblLotOperationalRulesTitle";
        lblLotOperationalRulesTitle.Size = new Size(135, 20);
        lblLotOperationalRulesTitle.TabIndex = 52;
        lblLotOperationalRulesTitle.Text = "2. Reglas operativas";
        // 
        // tabTaxes
        // 
        tabTaxes.Controls.Add(lblTaxConfigurationTitle);
        tabTaxes.Controls.Add(lblFiscalItemType);
        tabTaxes.Controls.Add(lueFiscalItemType);
        tabTaxes.Controls.Add(lblPurchaseVat);
        tabTaxes.Controls.Add(luePurchaseVat);
        tabTaxes.Controls.Add(lblTaxesSalesVat);
        tabTaxes.Controls.Add(lueTaxesSalesVat);
        tabTaxes.Controls.Add(lblExciseTax);
        tabTaxes.Controls.Add(lueExciseTax);
        tabTaxes.Controls.Add(lblTaxesSuggestedWithholding);
        tabTaxes.Controls.Add(lueTaxesSuggestedWithholding);
        tabTaxes.Controls.Add(lblTaxSupport);
        tabTaxes.Controls.Add(lueTaxSupport);
        tabTaxes.Controls.Add(lblFiscalCode);
        tabTaxes.Controls.Add(txtFiscalCode);
        tabTaxes.Controls.Add(lblFiscalCountry);
        tabTaxes.Controls.Add(lueFiscalCountry);
        tabTaxes.Controls.Add(lblTaxableGoods);
        tabTaxes.Controls.Add(tglTaxableGoods);
        tabTaxes.Controls.Add(lblTaxableService);
        tabTaxes.Controls.Add(tglTaxableService);
        tabTaxes.Controls.Add(lblTaxExemptGoods);
        tabTaxes.Controls.Add(tglTaxExemptGoods);
        tabTaxes.Controls.Add(pnlTaxConfigurationNote);
        tabTaxes.Name = "tabTaxes";
        tabTaxes.Size = new Size(1418, 537);
        tabTaxes.Text = "Impuestos";
        // 
        // pnlTaxConfigurationNote
        // 
        pnlTaxConfigurationNote.Appearance.BackColor = Color.FromArgb(238, 248, 255);
        pnlTaxConfigurationNote.Appearance.Options.UseBackColor = true;
        pnlTaxConfigurationNote.BorderStyle = BorderStyles.Simple;
        pnlTaxConfigurationNote.Controls.Add(lblTaxConfigurationNoteIcon);
        pnlTaxConfigurationNote.Controls.Add(lblTaxConfigurationNote);
        pnlTaxConfigurationNote.Location = new Point(11, 331);
        pnlTaxConfigurationNote.Name = "pnlTaxConfigurationNote";
        pnlTaxConfigurationNote.Size = new Size(374, 62);
        pnlTaxConfigurationNote.TabIndex = 47;
        // 
        // lblTaxConfigurationNote
        // 
        lblTaxConfigurationNote.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxConfigurationNote.Appearance.ForeColor = Color.FromArgb(38, 63, 99);
        lblTaxConfigurationNote.Appearance.Options.UseFont = true;
        lblTaxConfigurationNote.Appearance.Options.UseForeColor = true;
        lblTaxConfigurationNote.Appearance.Options.UseTextOptions = true;
        lblTaxConfigurationNote.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        lblTaxConfigurationNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblTaxConfigurationNote.Location = new Point(44, 10);
        lblTaxConfigurationNote.Name = "lblTaxConfigurationNote";
        lblTaxConfigurationNote.Size = new Size(310, 42);
        lblTaxConfigurationNote.TabIndex = 1;
        lblTaxConfigurationNote.Text = "La configuracion tributaria define como se comporta el articulo en compras, ventas, documentos electronicos y reportes fiscales.";
        // 
        // lblTaxConfigurationNoteIcon
        // 
        lblTaxConfigurationNoteIcon.Appearance.BackColor = Color.FromArgb(0, 122, 204);
        lblTaxConfigurationNoteIcon.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblTaxConfigurationNoteIcon.Appearance.ForeColor = Color.White;
        lblTaxConfigurationNoteIcon.Appearance.Options.UseBackColor = true;
        lblTaxConfigurationNoteIcon.Appearance.Options.UseFont = true;
        lblTaxConfigurationNoteIcon.Appearance.Options.UseForeColor = true;
        lblTaxConfigurationNoteIcon.Appearance.Options.UseTextOptions = true;
        lblTaxConfigurationNoteIcon.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        lblTaxConfigurationNoteIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblTaxConfigurationNoteIcon.Location = new Point(12, 18);
        lblTaxConfigurationNoteIcon.Name = "lblTaxConfigurationNoteIcon";
        lblTaxConfigurationNoteIcon.Size = new Size(18, 18);
        lblTaxConfigurationNoteIcon.TabIndex = 0;
        lblTaxConfigurationNoteIcon.Text = "i";
        // 
        // tglTaxExemptGoods
        // 
        tglTaxExemptGoods.Location = new Point(306, 279);
        tglTaxExemptGoods.Name = "tglTaxExemptGoods";
        tglTaxExemptGoods.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglTaxExemptGoods.Properties.Appearance.Options.UseFont = true;
        tglTaxExemptGoods.Properties.OffText = "No";
        tglTaxExemptGoods.Properties.OnText = "Si";
        tglTaxExemptGoods.Size = new Size(80, 20);
        tglTaxExemptGoods.TabIndex = 46;
        // 
        // lblTaxExemptGoods
        // 
        lblTaxExemptGoods.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxExemptGoods.Appearance.Options.UseFont = true;
        lblTaxExemptGoods.Location = new Point(235, 280);
        lblTaxExemptGoods.Name = "lblTaxExemptGoods";
        lblTaxExemptGoods.Size = new Size(64, 15);
        lblTaxExemptGoods.TabIndex = 45;
        lblTaxExemptGoods.Text = "Bien exento:";
        // 
        // tglTaxableService
        // 
        tglTaxableService.Location = new Point(137, 305);
        tglTaxableService.Name = "tglTaxableService";
        tglTaxableService.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglTaxableService.Properties.Appearance.Options.UseFont = true;
        tglTaxableService.Properties.OffText = "No";
        tglTaxableService.Properties.OnText = "Si";
        tglTaxableService.Size = new Size(86, 20);
        tglTaxableService.TabIndex = 44;
        // 
        // lblTaxableService
        // 
        lblTaxableService.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxableService.Appearance.Options.UseFont = true;
        lblTaxableService.Location = new Point(12, 308);
        lblTaxableService.Name = "lblTaxableService";
        lblTaxableService.Size = new Size(90, 15);
        lblTaxableService.TabIndex = 43;
        lblTaxableService.Text = "Servicio gravado:";
        // 
        // tglTaxableGoods
        // 
        tglTaxableGoods.EditValue = true;
        tglTaxableGoods.Location = new Point(137, 279);
        tglTaxableGoods.Name = "tglTaxableGoods";
        tglTaxableGoods.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglTaxableGoods.Properties.Appearance.Options.UseFont = true;
        tglTaxableGoods.Properties.OffText = "No";
        tglTaxableGoods.Properties.OnText = "Si";
        tglTaxableGoods.Size = new Size(86, 20);
        tglTaxableGoods.TabIndex = 42;
        // 
        // lblTaxableGoods
        // 
        lblTaxableGoods.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxableGoods.Appearance.Options.UseFont = true;
        lblTaxableGoods.Location = new Point(12, 280);
        lblTaxableGoods.Name = "lblTaxableGoods";
        lblTaxableGoods.Size = new Size(72, 15);
        lblTaxableGoods.TabIndex = 41;
        lblTaxableGoods.Text = "Bien gravado:";
        // 
        // lueFiscalCountry
        // 
        lueFiscalCountry.EditValue = "Ecuador";
        lueFiscalCountry.Location = new Point(137, 251);
        lueFiscalCountry.Name = "lueFiscalCountry";
        lueFiscalCountry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueFiscalCountry.Properties.Appearance.Options.UseFont = true;
        lueFiscalCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFiscalCountry.Properties.NullText = "";
        lueFiscalCountry.Size = new Size(249, 22);
        lueFiscalCountry.TabIndex = 40;
        // 
        // lblFiscalCountry
        // 
        lblFiscalCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblFiscalCountry.Appearance.Options.UseFont = true;
        lblFiscalCountry.Location = new Point(12, 253);
        lblFiscalCountry.Name = "lblFiscalCountry";
        lblFiscalCountry.Size = new Size(54, 15);
        lblFiscalCountry.TabIndex = 39;
        lblFiscalCountry.Text = "Pais fiscal:";
        // 
        // txtFiscalCode
        // 
        txtFiscalCode.EditValue = "ALIM-GRA-001";
        txtFiscalCode.Location = new Point(137, 223);
        txtFiscalCode.Name = "txtFiscalCode";
        txtFiscalCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtFiscalCode.Properties.Appearance.Options.UseFont = true;
        txtFiscalCode.Size = new Size(249, 22);
        txtFiscalCode.TabIndex = 38;
        // 
        // lblFiscalCode
        // 
        lblFiscalCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblFiscalCode.Appearance.Options.UseFont = true;
        lblFiscalCode.Location = new Point(11, 225);
        lblFiscalCode.Name = "lblFiscalCode";
        lblFiscalCode.Size = new Size(72, 15);
        lblFiscalCode.TabIndex = 37;
        lblFiscalCode.Text = "Codigo fiscal:";
        // 
        // lueTaxSupport
        // 
        lueTaxSupport.EditValue = "01 - Credito tributario para declaracion";
        lueTaxSupport.Location = new Point(137, 195);
        lueTaxSupport.Name = "lueTaxSupport";
        lueTaxSupport.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueTaxSupport.Properties.Appearance.Options.UseFont = true;
        lueTaxSupport.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueTaxSupport.Properties.NullText = "";
        lueTaxSupport.Size = new Size(249, 22);
        lueTaxSupport.TabIndex = 36;
        // 
        // lblTaxSupport
        // 
        lblTaxSupport.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxSupport.Appearance.Options.UseFont = true;
        lblTaxSupport.Location = new Point(12, 197);
        lblTaxSupport.Name = "lblTaxSupport";
        lblTaxSupport.Size = new Size(101, 15);
        lblTaxSupport.TabIndex = 35;
        lblTaxSupport.Text = "Sustento tributario:";
        // 
        // lueTaxesSuggestedWithholding
        // 
        lueTaxesSuggestedWithholding.EditValue = "1% - Bienes";
        lueTaxesSuggestedWithholding.Location = new Point(137, 167);
        lueTaxesSuggestedWithholding.Name = "lueTaxesSuggestedWithholding";
        lueTaxesSuggestedWithholding.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueTaxesSuggestedWithholding.Properties.Appearance.Options.UseFont = true;
        lueTaxesSuggestedWithholding.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueTaxesSuggestedWithholding.Properties.NullText = "";
        lueTaxesSuggestedWithholding.Size = new Size(249, 22);
        lueTaxesSuggestedWithholding.TabIndex = 34;
        // 
        // lblTaxesSuggestedWithholding
        // 
        lblTaxesSuggestedWithholding.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxesSuggestedWithholding.Appearance.Options.UseFont = true;
        lblTaxesSuggestedWithholding.Location = new Point(12, 170);
        lblTaxesSuggestedWithholding.Name = "lblTaxesSuggestedWithholding";
        lblTaxesSuggestedWithholding.Size = new Size(104, 15);
        lblTaxesSuggestedWithholding.TabIndex = 33;
        lblTaxesSuggestedWithholding.Text = "Retencion sugerida:";
        // 
        // lueExciseTax
        // 
        lueExciseTax.EditValue = "No aplica";
        lueExciseTax.Location = new Point(137, 139);
        lueExciseTax.Name = "lueExciseTax";
        lueExciseTax.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueExciseTax.Properties.Appearance.Options.UseFont = true;
        lueExciseTax.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueExciseTax.Properties.NullText = "";
        lueExciseTax.Size = new Size(249, 22);
        lueExciseTax.TabIndex = 32;
        // 
        // lblExciseTax
        // 
        lblExciseTax.Appearance.Font = new Font("Segoe UI", 9F);
        lblExciseTax.Appearance.Options.UseFont = true;
        lblExciseTax.Location = new Point(11, 142);
        lblExciseTax.Name = "lblExciseTax";
        lblExciseTax.Size = new Size(20, 15);
        lblExciseTax.TabIndex = 31;
        lblExciseTax.Text = "ICE:";
        // 
        // lueTaxesSalesVat
        // 
        lueTaxesSalesVat.EditValue = "IVA 15% - Tarifa general";
        lueTaxesSalesVat.Location = new Point(137, 111);
        lueTaxesSalesVat.Name = "lueTaxesSalesVat";
        lueTaxesSalesVat.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueTaxesSalesVat.Properties.Appearance.Options.UseFont = true;
        lueTaxesSalesVat.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueTaxesSalesVat.Properties.NullText = "";
        lueTaxesSalesVat.Size = new Size(249, 22);
        lueTaxesSalesVat.TabIndex = 30;
        // 
        // lblTaxesSalesVat
        // 
        lblTaxesSalesVat.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxesSalesVat.Appearance.Options.UseFont = true;
        lblTaxesSalesVat.Location = new Point(12, 114);
        lblTaxesSalesVat.Name = "lblTaxesSalesVat";
        lblTaxesSalesVat.Size = new Size(53, 15);
        lblTaxesSalesVat.TabIndex = 29;
        lblTaxesSalesVat.Text = "IVA venta:";
        // 
        // luePurchaseVat
        // 
        luePurchaseVat.EditValue = "IVA 15% - Credito tributario";
        luePurchaseVat.Location = new Point(137, 83);
        luePurchaseVat.Name = "luePurchaseVat";
        luePurchaseVat.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseVat.Properties.Appearance.Options.UseFont = true;
        luePurchaseVat.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseVat.Properties.NullText = "";
        luePurchaseVat.Size = new Size(249, 22);
        luePurchaseVat.TabIndex = 28;
        // 
        // lblPurchaseVat
        // 
        lblPurchaseVat.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseVat.Appearance.Options.UseFont = true;
        lblPurchaseVat.Location = new Point(12, 86);
        lblPurchaseVat.Name = "lblPurchaseVat";
        lblPurchaseVat.Size = new Size(65, 15);
        lblPurchaseVat.TabIndex = 27;
        lblPurchaseVat.Text = "IVA compra:";
        // 
        // lueFiscalItemType
        // 
        lueFiscalItemType.EditValue = "Gravado";
        lueFiscalItemType.Location = new Point(137, 55);
        lueFiscalItemType.Name = "lueFiscalItemType";
        lueFiscalItemType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueFiscalItemType.Properties.Appearance.Options.UseFont = true;
        lueFiscalItemType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFiscalItemType.Properties.NullText = "";
        lueFiscalItemType.Size = new Size(249, 22);
        lueFiscalItemType.TabIndex = 26;
        // 
        // lblFiscalItemType
        // 
        lblFiscalItemType.Appearance.Font = new Font("Segoe UI", 9F);
        lblFiscalItemType.Appearance.Options.UseFont = true;
        lblFiscalItemType.Location = new Point(12, 58);
        lblFiscalItemType.Name = "lblFiscalItemType";
        lblFiscalItemType.Size = new Size(119, 15);
        lblFiscalItemType.TabIndex = 25;
        lblFiscalItemType.Text = "Tipo fiscal del articulo:";
        // 
        // lblTaxConfigurationTitle
        // 
        lblTaxConfigurationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblTaxConfigurationTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblTaxConfigurationTitle.Appearance.Options.UseFont = true;
        lblTaxConfigurationTitle.Appearance.Options.UseForeColor = true;
        lblTaxConfigurationTitle.Location = new Point(12, 12);
        lblTaxConfigurationTitle.Name = "lblTaxConfigurationTitle";
        lblTaxConfigurationTitle.Size = new Size(179, 20);
        lblTaxConfigurationTitle.TabIndex = 24;
        lblTaxConfigurationTitle.Text = "1. Configuracion tributaria";
        // 
        // tabAccounting
        // 
        tabAccounting.Controls.Add(lblAccountingAccountsTitle);
        tabAccounting.Controls.Add(lblAccountingInventoryAccount);
        tabAccounting.Controls.Add(slueInventoryAccount);
        tabAccounting.Controls.Add(lblAccountingRevenueAccount);
        tabAccounting.Controls.Add(slueRevenueAccount);
        tabAccounting.Controls.Add(lblCostOfGoodsSoldAccount);
        tabAccounting.Controls.Add(slueCostOfGoodsSoldAccount);
        tabAccounting.Controls.Add(lblSalesReturnAccount);
        tabAccounting.Controls.Add(slueSalesReturnAccount);
        tabAccounting.Controls.Add(lblPurchaseReturnAccount);
        tabAccounting.Controls.Add(sluePurchaseReturnAccount);
        tabAccounting.Controls.Add(lblCostVarianceAccount);
        tabAccounting.Controls.Add(slueCostVarianceAccount);
        tabAccounting.Controls.Add(lblInventoryAdjustmentAccount);
        tabAccounting.Controls.Add(slueInventoryAdjustmentAccount);
        tabAccounting.Controls.Add(lblPurchaseExpenseAccount);
        tabAccounting.Controls.Add(sluePurchaseExpenseAccount);
        tabAccounting.Controls.Add(pnlAccountingAccountsNote);
        tabAccounting.Controls.Add(pnlAccountingRules);
        tabAccounting.Name = "tabAccounting";
        tabAccounting.Size = new Size(1418, 537);
        tabAccounting.Text = "Contabilidad";
        // 
        // pnlAccountingRules
        // 
        pnlAccountingRules.BorderStyle = BorderStyles.Simple;
        pnlAccountingRules.Controls.Add(lblAccountingRulesTitle);
        pnlAccountingRules.Controls.Add(lblGenerateInventoryJournal);
        pnlAccountingRules.Controls.Add(tglGenerateInventoryJournal);
        pnlAccountingRules.Controls.Add(lblUseWarehouseAccount);
        pnlAccountingRules.Controls.Add(tglUseWarehouseAccount);
        pnlAccountingRules.Controls.Add(lblUseGroupAccount);
        pnlAccountingRules.Controls.Add(tglUseGroupAccount);
        pnlAccountingRules.Controls.Add(lblAllowCompensation);
        pnlAccountingRules.Controls.Add(tglAllowCompensation);
        pnlAccountingRules.Controls.Add(lblAccountingBlocked);
        pnlAccountingRules.Controls.Add(tglAccountingBlocked);
        pnlAccountingRules.Controls.Add(lblReconciliationDays);
        pnlAccountingRules.Controls.Add(spnReconciliationDays);
        pnlAccountingRules.Controls.Add(lblAccountingIntegrationMethod);
        pnlAccountingRules.Controls.Add(lueAccountingIntegrationMethod);
        pnlAccountingRules.Controls.Add(lblAccountingNotes);
        pnlAccountingRules.Controls.Add(memAccountingNotes);
        pnlAccountingRules.Controls.Add(pnlAccountingRulesNote);
        pnlAccountingRules.Location = new Point(958, 18);
        pnlAccountingRules.Name = "pnlAccountingRules";
        pnlAccountingRules.Size = new Size(442, 490);
        pnlAccountingRules.TabIndex = 2;
        // 
        // pnlAccountingRulesNote
        // 
        pnlAccountingRulesNote.Appearance.BackColor = Color.FromArgb(238, 248, 255);
        pnlAccountingRulesNote.Appearance.Options.UseBackColor = true;
        pnlAccountingRulesNote.BorderStyle = BorderStyles.Simple;
        pnlAccountingRulesNote.Controls.Add(lblAccountingRulesNoteIcon);
        pnlAccountingRulesNote.Controls.Add(lblAccountingRulesNote);
        pnlAccountingRulesNote.Location = new Point(16, 392);
        pnlAccountingRulesNote.Name = "pnlAccountingRulesNote";
        pnlAccountingRulesNote.Size = new Size(395, 62);
        pnlAccountingRulesNote.TabIndex = 17;
        // 
        // lblAccountingRulesNote
        // 
        lblAccountingRulesNote.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingRulesNote.Appearance.ForeColor = Color.FromArgb(38, 63, 99);
        lblAccountingRulesNote.Appearance.Options.UseFont = true;
        lblAccountingRulesNote.Appearance.Options.UseForeColor = true;
        lblAccountingRulesNote.Appearance.Options.UseTextOptions = true;
        lblAccountingRulesNote.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        lblAccountingRulesNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblAccountingRulesNote.Location = new Point(44, 12);
        lblAccountingRulesNote.Name = "lblAccountingRulesNote";
        lblAccountingRulesNote.Size = new Size(330, 40);
        lblAccountingRulesNote.TabIndex = 1;
        lblAccountingRulesNote.Text = "Estas reglas determinan el comportamiento contable del item en los diferentes procesos.";
        // 
        // lblAccountingRulesNoteIcon
        // 
        lblAccountingRulesNoteIcon.Appearance.BackColor = Color.FromArgb(0, 122, 204);
        lblAccountingRulesNoteIcon.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblAccountingRulesNoteIcon.Appearance.ForeColor = Color.White;
        lblAccountingRulesNoteIcon.Appearance.Options.UseBackColor = true;
        lblAccountingRulesNoteIcon.Appearance.Options.UseFont = true;
        lblAccountingRulesNoteIcon.Appearance.Options.UseForeColor = true;
        lblAccountingRulesNoteIcon.Appearance.Options.UseTextOptions = true;
        lblAccountingRulesNoteIcon.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        lblAccountingRulesNoteIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblAccountingRulesNoteIcon.Location = new Point(12, 18);
        lblAccountingRulesNoteIcon.Name = "lblAccountingRulesNoteIcon";
        lblAccountingRulesNoteIcon.Size = new Size(18, 18);
        lblAccountingRulesNoteIcon.TabIndex = 0;
        lblAccountingRulesNoteIcon.Text = "i";
        // 
        // memAccountingNotes
        // 
        memAccountingNotes.EditValue = "Item de alta rotacion.\r\nSe utiliza metodo promedio ponderado para valoracion de inventario.";
        memAccountingNotes.Location = new Point(216, 322);
        memAccountingNotes.Name = "memAccountingNotes";
        memAccountingNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memAccountingNotes.Properties.Appearance.Options.UseFont = true;
        memAccountingNotes.Size = new Size(195, 58);
        memAccountingNotes.TabIndex = 16;
        // 
        // lblAccountingNotes
        // 
        lblAccountingNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingNotes.Appearance.Options.UseFont = true;
        lblAccountingNotes.Location = new Point(16, 326);
        lblAccountingNotes.Name = "lblAccountingNotes";
        lblAccountingNotes.Size = new Size(134, 15);
        lblAccountingNotes.TabIndex = 15;
        lblAccountingNotes.Text = "Observaciones contables:";
        // 
        // lueAccountingIntegrationMethod
        // 
        lueAccountingIntegrationMethod.EditValue = "En tiempo real";
        lueAccountingIntegrationMethod.Location = new Point(216, 282);
        lueAccountingIntegrationMethod.Name = "lueAccountingIntegrationMethod";
        lueAccountingIntegrationMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingIntegrationMethod.Properties.Appearance.Options.UseFont = true;
        lueAccountingIntegrationMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingIntegrationMethod.Properties.NullText = "";
        lueAccountingIntegrationMethod.Size = new Size(195, 22);
        lueAccountingIntegrationMethod.TabIndex = 14;
        // 
        // lblAccountingIntegrationMethod
        // 
        lblAccountingIntegrationMethod.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingIntegrationMethod.Appearance.Options.UseFont = true;
        lblAccountingIntegrationMethod.Location = new Point(16, 286);
        lblAccountingIntegrationMethod.Name = "lblAccountingIntegrationMethod";
        lblAccountingIntegrationMethod.Size = new Size(173, 15);
        lblAccountingIntegrationMethod.TabIndex = 13;
        lblAccountingIntegrationMethod.Text = "Metodo de integracion contable:";
        // 
        // spnReconciliationDays
        // 
        spnReconciliationDays.EditValue = new decimal(new int[] { 30, 0, 0, 0 });
        spnReconciliationDays.Location = new Point(262, 242);
        spnReconciliationDays.Name = "spnReconciliationDays";
        spnReconciliationDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnReconciliationDays.Properties.Appearance.Options.UseFont = true;
        spnReconciliationDays.Properties.Appearance.Options.UseTextOptions = true;
        spnReconciliationDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnReconciliationDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnReconciliationDays.Properties.IsFloatValue = false;
        spnReconciliationDays.Properties.MaskSettings.Set("mask", "N00");
        spnReconciliationDays.Size = new Size(86, 22);
        spnReconciliationDays.TabIndex = 12;
        // 
        // lblReconciliationDays
        // 
        lblReconciliationDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblReconciliationDays.Appearance.Options.UseFont = true;
        lblReconciliationDays.Location = new Point(16, 246);
        lblReconciliationDays.Name = "lblReconciliationDays";
        lblReconciliationDays.Size = new Size(92, 15);
        lblReconciliationDays.TabIndex = 11;
        lblReconciliationDays.Text = "Dias conciliacion:";
        // 
        // tglAccountingBlocked
        // 
        tglAccountingBlocked.Location = new Point(262, 202);
        tglAccountingBlocked.Name = "tglAccountingBlocked";
        tglAccountingBlocked.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAccountingBlocked.Properties.Appearance.Options.UseFont = true;
        tglAccountingBlocked.Properties.OffText = "No";
        tglAccountingBlocked.Properties.OnText = "Si";
        tglAccountingBlocked.Size = new Size(86, 20);
        tglAccountingBlocked.TabIndex = 10;
        // 
        // lblAccountingBlocked
        // 
        lblAccountingBlocked.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingBlocked.Appearance.Options.UseFont = true;
        lblAccountingBlocked.Location = new Point(16, 206);
        lblAccountingBlocked.Name = "lblAccountingBlocked";
        lblAccountingBlocked.Size = new Size(109, 15);
        lblAccountingBlocked.TabIndex = 9;
        lblAccountingBlocked.Text = "Bloqueado contable:";
        // 
        // tglAllowCompensation
        // 
        tglAllowCompensation.EditValue = true;
        tglAllowCompensation.Location = new Point(262, 166);
        tglAllowCompensation.Name = "tglAllowCompensation";
        tglAllowCompensation.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAllowCompensation.Properties.Appearance.Options.UseFont = true;
        tglAllowCompensation.Properties.OffText = "No";
        tglAllowCompensation.Properties.OnText = "Si";
        tglAllowCompensation.Size = new Size(86, 20);
        tglAllowCompensation.TabIndex = 8;
        // 
        // lblAllowCompensation
        // 
        lblAllowCompensation.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowCompensation.Appearance.Options.UseFont = true;
        lblAllowCompensation.Location = new Point(16, 170);
        lblAllowCompensation.Name = "lblAllowCompensation";
        lblAllowCompensation.Size = new Size(125, 15);
        lblAllowCompensation.TabIndex = 7;
        lblAllowCompensation.Text = "Permite compensacion:";
        // 
        // tglUseGroupAccount
        // 
        tglUseGroupAccount.Location = new Point(262, 130);
        tglUseGroupAccount.Name = "tglUseGroupAccount";
        tglUseGroupAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglUseGroupAccount.Properties.Appearance.Options.UseFont = true;
        tglUseGroupAccount.Properties.OffText = "No";
        tglUseGroupAccount.Properties.OnText = "Si";
        tglUseGroupAccount.Size = new Size(86, 20);
        tglUseGroupAccount.TabIndex = 6;
        // 
        // lblUseGroupAccount
        // 
        lblUseGroupAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblUseGroupAccount.Appearance.Options.UseFont = true;
        lblUseGroupAccount.Location = new Point(16, 134);
        lblUseGroupAccount.Name = "lblUseGroupAccount";
        lblUseGroupAccount.Size = new Size(117, 15);
        lblUseGroupAccount.TabIndex = 5;
        lblUseGroupAccount.Text = "Usa cuenta por grupo:";
        // 
        // tglUseWarehouseAccount
        // 
        tglUseWarehouseAccount.EditValue = true;
        tglUseWarehouseAccount.Location = new Point(262, 94);
        tglUseWarehouseAccount.Name = "tglUseWarehouseAccount";
        tglUseWarehouseAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglUseWarehouseAccount.Properties.Appearance.Options.UseFont = true;
        tglUseWarehouseAccount.Properties.OffText = "No";
        tglUseWarehouseAccount.Properties.OnText = "Si";
        tglUseWarehouseAccount.Size = new Size(86, 20);
        tglUseWarehouseAccount.TabIndex = 4;
        // 
        // lblUseWarehouseAccount
        // 
        lblUseWarehouseAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblUseWarehouseAccount.Appearance.Options.UseFont = true;
        lblUseWarehouseAccount.Location = new Point(16, 98);
        lblUseWarehouseAccount.Name = "lblUseWarehouseAccount";
        lblUseWarehouseAccount.Size = new Size(125, 15);
        lblUseWarehouseAccount.TabIndex = 3;
        lblUseWarehouseAccount.Text = "Usa cuenta por bodega:";
        // 
        // tglGenerateInventoryJournal
        // 
        tglGenerateInventoryJournal.EditValue = true;
        tglGenerateInventoryJournal.Location = new Point(262, 58);
        tglGenerateInventoryJournal.Name = "tglGenerateInventoryJournal";
        tglGenerateInventoryJournal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGenerateInventoryJournal.Properties.Appearance.Options.UseFont = true;
        tglGenerateInventoryJournal.Properties.OffText = "No";
        tglGenerateInventoryJournal.Properties.OnText = "Si";
        tglGenerateInventoryJournal.Size = new Size(86, 20);
        tglGenerateInventoryJournal.TabIndex = 2;
        // 
        // lblGenerateInventoryJournal
        // 
        lblGenerateInventoryJournal.Appearance.Font = new Font("Segoe UI", 9F);
        lblGenerateInventoryJournal.Appearance.Options.UseFont = true;
        lblGenerateInventoryJournal.Location = new Point(16, 62);
        lblGenerateInventoryJournal.Name = "lblGenerateInventoryJournal";
        lblGenerateInventoryJournal.Size = new Size(137, 15);
        lblGenerateInventoryJournal.TabIndex = 1;
        lblGenerateInventoryJournal.Text = "Genera asiento inventario:";
        // 
        // lblAccountingRulesTitle
        // 
        lblAccountingRulesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAccountingRulesTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAccountingRulesTitle.Appearance.Options.UseFont = true;
        lblAccountingRulesTitle.Appearance.Options.UseForeColor = true;
        lblAccountingRulesTitle.Location = new Point(16, 16);
        lblAccountingRulesTitle.Name = "lblAccountingRulesTitle";
        lblAccountingRulesTitle.Size = new Size(129, 20);
        lblAccountingRulesTitle.TabIndex = 0;
        lblAccountingRulesTitle.Text = "3. Reglas contables";
        // 
        // pnlAccountingAccountsNote
        // 
        pnlAccountingAccountsNote.Appearance.BackColor = Color.FromArgb(238, 248, 255);
        pnlAccountingAccountsNote.Appearance.Options.UseBackColor = true;
        pnlAccountingAccountsNote.BorderStyle = BorderStyles.Simple;
        pnlAccountingAccountsNote.Controls.Add(lblAccountingAccountsNoteIcon);
        pnlAccountingAccountsNote.Controls.Add(lblAccountingAccountsNote);
        pnlAccountingAccountsNote.Location = new Point(12, 332);
        pnlAccountingAccountsNote.Name = "pnlAccountingAccountsNote";
        pnlAccountingAccountsNote.Size = new Size(464, 62);
        pnlAccountingAccountsNote.TabIndex = 35;
        // 
        // lblAccountingAccountsNote
        // 
        lblAccountingAccountsNote.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingAccountsNote.Appearance.ForeColor = Color.FromArgb(38, 63, 99);
        lblAccountingAccountsNote.Appearance.Options.UseFont = true;
        lblAccountingAccountsNote.Appearance.Options.UseForeColor = true;
        lblAccountingAccountsNote.Appearance.Options.UseTextOptions = true;
        lblAccountingAccountsNote.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
        lblAccountingAccountsNote.AutoSizeMode = LabelAutoSizeMode.None;
        lblAccountingAccountsNote.Location = new Point(44, 12);
        lblAccountingAccountsNote.Name = "lblAccountingAccountsNote";
        lblAccountingAccountsNote.Size = new Size(400, 40);
        lblAccountingAccountsNote.TabIndex = 1;
        lblAccountingAccountsNote.Text = "Las cuentas seleccionadas se usaran para la generacion automatica de asientos contables relacionados con este item.";
        // 
        // lblAccountingAccountsNoteIcon
        // 
        lblAccountingAccountsNoteIcon.Appearance.BackColor = Color.FromArgb(0, 122, 204);
        lblAccountingAccountsNoteIcon.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblAccountingAccountsNoteIcon.Appearance.ForeColor = Color.White;
        lblAccountingAccountsNoteIcon.Appearance.Options.UseBackColor = true;
        lblAccountingAccountsNoteIcon.Appearance.Options.UseFont = true;
        lblAccountingAccountsNoteIcon.Appearance.Options.UseForeColor = true;
        lblAccountingAccountsNoteIcon.Appearance.Options.UseTextOptions = true;
        lblAccountingAccountsNoteIcon.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        lblAccountingAccountsNoteIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblAccountingAccountsNoteIcon.Location = new Point(12, 18);
        lblAccountingAccountsNoteIcon.Name = "lblAccountingAccountsNoteIcon";
        lblAccountingAccountsNoteIcon.Size = new Size(18, 18);
        lblAccountingAccountsNoteIcon.TabIndex = 0;
        lblAccountingAccountsNoteIcon.Text = "i";
        // 
        // sluePurchaseExpenseAccount
        // 
        sluePurchaseExpenseAccount.Location = new Point(171, 250);
        sluePurchaseExpenseAccount.Name = "sluePurchaseExpenseAccount";
        sluePurchaseExpenseAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sluePurchaseExpenseAccount.Properties.Appearance.Options.UseFont = true;
        sluePurchaseExpenseAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        sluePurchaseExpenseAccount.Properties.NullText = "5105-04-01 Gastos de compra";
        sluePurchaseExpenseAccount.Properties.PopupView = gvPurchaseExpenseAccount;
        sluePurchaseExpenseAccount.Size = new Size(305, 22);
        sluePurchaseExpenseAccount.TabIndex = 34;
        // 
        // gvPurchaseExpenseAccount
        // 
        gvPurchaseExpenseAccount.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvPurchaseExpenseAccount.Appearance.HeaderPanel.Options.UseFont = true;
        gvPurchaseExpenseAccount.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvPurchaseExpenseAccount.Appearance.Row.Options.UseFont = true;
        gvPurchaseExpenseAccount.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvPurchaseExpenseAccount.Name = "gvPurchaseExpenseAccount";
        gvPurchaseExpenseAccount.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvPurchaseExpenseAccount.OptionsView.ShowGroupPanel = false;
        // 
        // lblPurchaseExpenseAccount
        // 
        lblPurchaseExpenseAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseExpenseAccount.Appearance.Options.UseFont = true;
        lblPurchaseExpenseAccount.Location = new Point(12, 254);
        lblPurchaseExpenseAccount.Name = "lblPurchaseExpenseAccount";
        lblPurchaseExpenseAccount.Size = new Size(117, 15);
        lblPurchaseExpenseAccount.TabIndex = 33;
        lblPurchaseExpenseAccount.Text = "Cuenta gasto compra:";
        // 
        // slueInventoryAdjustmentAccount
        // 
        slueInventoryAdjustmentAccount.Location = new Point(171, 222);
        slueInventoryAdjustmentAccount.Name = "slueInventoryAdjustmentAccount";
        slueInventoryAdjustmentAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueInventoryAdjustmentAccount.Properties.Appearance.Options.UseFont = true;
        slueInventoryAdjustmentAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueInventoryAdjustmentAccount.Properties.NullText = "1205-04-01 Ajuste de inventario";
        slueInventoryAdjustmentAccount.Properties.PopupView = gvInventoryAdjustmentAccount;
        slueInventoryAdjustmentAccount.Size = new Size(305, 22);
        slueInventoryAdjustmentAccount.TabIndex = 32;
        // 
        // gvInventoryAdjustmentAccount
        // 
        gvInventoryAdjustmentAccount.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvInventoryAdjustmentAccount.Appearance.HeaderPanel.Options.UseFont = true;
        gvInventoryAdjustmentAccount.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvInventoryAdjustmentAccount.Appearance.Row.Options.UseFont = true;
        gvInventoryAdjustmentAccount.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvInventoryAdjustmentAccount.Name = "gvInventoryAdjustmentAccount";
        gvInventoryAdjustmentAccount.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvInventoryAdjustmentAccount.OptionsView.ShowGroupPanel = false;
        // 
        // lblInventoryAdjustmentAccount
        // 
        lblInventoryAdjustmentAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblInventoryAdjustmentAccount.Appearance.Options.UseFont = true;
        lblInventoryAdjustmentAccount.Location = new Point(12, 226);
        lblInventoryAdjustmentAccount.Name = "lblInventoryAdjustmentAccount";
        lblInventoryAdjustmentAccount.Size = new Size(131, 15);
        lblInventoryAdjustmentAccount.TabIndex = 31;
        lblInventoryAdjustmentAccount.Text = "Cuenta ajuste inventario:";
        // 
        // slueCostVarianceAccount
        // 
        slueCostVarianceAccount.Location = new Point(171, 194);
        slueCostVarianceAccount.Name = "slueCostVarianceAccount";
        slueCostVarianceAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueCostVarianceAccount.Properties.Appearance.Options.UseFont = true;
        slueCostVarianceAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueCostVarianceAccount.Properties.NullText = "5105-03-01 Variacion de inventario";
        slueCostVarianceAccount.Properties.PopupView = gvCostVarianceAccount;
        slueCostVarianceAccount.Size = new Size(305, 22);
        slueCostVarianceAccount.TabIndex = 30;
        // 
        // gvCostVarianceAccount
        // 
        gvCostVarianceAccount.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvCostVarianceAccount.Appearance.HeaderPanel.Options.UseFont = true;
        gvCostVarianceAccount.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvCostVarianceAccount.Appearance.Row.Options.UseFont = true;
        gvCostVarianceAccount.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvCostVarianceAccount.Name = "gvCostVarianceAccount";
        gvCostVarianceAccount.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvCostVarianceAccount.OptionsView.ShowGroupPanel = false;
        // 
        // lblCostVarianceAccount
        // 
        lblCostVarianceAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostVarianceAccount.Appearance.Options.UseFont = true;
        lblCostVarianceAccount.Location = new Point(12, 198);
        lblCostVarianceAccount.Name = "lblCostVarianceAccount";
        lblCostVarianceAccount.Size = new Size(124, 15);
        lblCostVarianceAccount.TabIndex = 29;
        lblCostVarianceAccount.Text = "Cuenta variacion costo:";
        // 
        // sluePurchaseReturnAccount
        // 
        sluePurchaseReturnAccount.Location = new Point(171, 166);
        sluePurchaseReturnAccount.Name = "sluePurchaseReturnAccount";
        sluePurchaseReturnAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sluePurchaseReturnAccount.Properties.Appearance.Options.UseFont = true;
        sluePurchaseReturnAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        sluePurchaseReturnAccount.Properties.NullText = "2105-02-01 Devoluciones en compras";
        sluePurchaseReturnAccount.Properties.PopupView = gvPurchaseReturnAccount;
        sluePurchaseReturnAccount.Size = new Size(305, 22);
        sluePurchaseReturnAccount.TabIndex = 28;
        // 
        // gvPurchaseReturnAccount
        // 
        gvPurchaseReturnAccount.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvPurchaseReturnAccount.Appearance.HeaderPanel.Options.UseFont = true;
        gvPurchaseReturnAccount.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvPurchaseReturnAccount.Appearance.Row.Options.UseFont = true;
        gvPurchaseReturnAccount.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvPurchaseReturnAccount.Name = "gvPurchaseReturnAccount";
        gvPurchaseReturnAccount.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvPurchaseReturnAccount.OptionsView.ShowGroupPanel = false;
        // 
        // lblPurchaseReturnAccount
        // 
        lblPurchaseReturnAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseReturnAccount.Appearance.Options.UseFont = true;
        lblPurchaseReturnAccount.Location = new Point(12, 170);
        lblPurchaseReturnAccount.Name = "lblPurchaseReturnAccount";
        lblPurchaseReturnAccount.Size = new Size(147, 15);
        lblPurchaseReturnAccount.TabIndex = 27;
        lblPurchaseReturnAccount.Text = "Cuenta devolucion compra:";
        // 
        // slueSalesReturnAccount
        // 
        slueSalesReturnAccount.Location = new Point(171, 138);
        slueSalesReturnAccount.Name = "slueSalesReturnAccount";
        slueSalesReturnAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueSalesReturnAccount.Properties.Appearance.Options.UseFont = true;
        slueSalesReturnAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueSalesReturnAccount.Properties.NullText = "4105-02-01 Devoluciones en ventas";
        slueSalesReturnAccount.Properties.PopupView = gvSalesReturnAccount;
        slueSalesReturnAccount.Size = new Size(305, 22);
        slueSalesReturnAccount.TabIndex = 26;
        // 
        // gvSalesReturnAccount
        // 
        gvSalesReturnAccount.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvSalesReturnAccount.Appearance.HeaderPanel.Options.UseFont = true;
        gvSalesReturnAccount.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvSalesReturnAccount.Appearance.Row.Options.UseFont = true;
        gvSalesReturnAccount.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvSalesReturnAccount.Name = "gvSalesReturnAccount";
        gvSalesReturnAccount.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvSalesReturnAccount.OptionsView.ShowGroupPanel = false;
        // 
        // lblSalesReturnAccount
        // 
        lblSalesReturnAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesReturnAccount.Appearance.Options.UseFont = true;
        lblSalesReturnAccount.Location = new Point(12, 142);
        lblSalesReturnAccount.Name = "lblSalesReturnAccount";
        lblSalesReturnAccount.Size = new Size(135, 15);
        lblSalesReturnAccount.TabIndex = 25;
        lblSalesReturnAccount.Text = "Cuenta devolucion venta:";
        // 
        // slueCostOfGoodsSoldAccount
        // 
        slueCostOfGoodsSoldAccount.Location = new Point(171, 110);
        slueCostOfGoodsSoldAccount.Name = "slueCostOfGoodsSoldAccount";
        slueCostOfGoodsSoldAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueCostOfGoodsSoldAccount.Properties.Appearance.Options.UseFont = true;
        slueCostOfGoodsSoldAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueCostOfGoodsSoldAccount.Properties.NullText = "5105-01-01 Costo de ventas";
        slueCostOfGoodsSoldAccount.Properties.PopupView = gvCostOfGoodsSoldAccount;
        slueCostOfGoodsSoldAccount.Size = new Size(305, 22);
        slueCostOfGoodsSoldAccount.TabIndex = 24;
        // 
        // gvCostOfGoodsSoldAccount
        // 
        gvCostOfGoodsSoldAccount.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvCostOfGoodsSoldAccount.Appearance.HeaderPanel.Options.UseFont = true;
        gvCostOfGoodsSoldAccount.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvCostOfGoodsSoldAccount.Appearance.Row.Options.UseFont = true;
        gvCostOfGoodsSoldAccount.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvCostOfGoodsSoldAccount.Name = "gvCostOfGoodsSoldAccount";
        gvCostOfGoodsSoldAccount.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvCostOfGoodsSoldAccount.OptionsView.ShowGroupPanel = false;
        // 
        // lblCostOfGoodsSoldAccount
        // 
        lblCostOfGoodsSoldAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostOfGoodsSoldAccount.Appearance.Options.UseFont = true;
        lblCostOfGoodsSoldAccount.Location = new Point(12, 114);
        lblCostOfGoodsSoldAccount.Name = "lblCostOfGoodsSoldAccount";
        lblCostOfGoodsSoldAccount.Size = new Size(121, 15);
        lblCostOfGoodsSoldAccount.TabIndex = 23;
        lblCostOfGoodsSoldAccount.Text = "Cuenta costo de venta:";
        // 
        // slueRevenueAccount
        // 
        slueRevenueAccount.Location = new Point(171, 82);
        slueRevenueAccount.Name = "slueRevenueAccount";
        slueRevenueAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueRevenueAccount.Properties.Appearance.Options.UseFont = true;
        slueRevenueAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueRevenueAccount.Properties.NullText = "4105-01-01 Ventas de mercaderias";
        slueRevenueAccount.Properties.PopupView = gvRevenueAccount;
        slueRevenueAccount.Size = new Size(305, 22);
        slueRevenueAccount.TabIndex = 22;
        // 
        // gvRevenueAccount
        // 
        gvRevenueAccount.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvRevenueAccount.Appearance.HeaderPanel.Options.UseFont = true;
        gvRevenueAccount.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvRevenueAccount.Appearance.Row.Options.UseFont = true;
        gvRevenueAccount.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvRevenueAccount.Name = "gvRevenueAccount";
        gvRevenueAccount.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvRevenueAccount.OptionsView.ShowGroupPanel = false;
        // 
        // lblAccountingRevenueAccount
        // 
        lblAccountingRevenueAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingRevenueAccount.Appearance.Options.UseFont = true;
        lblAccountingRevenueAccount.Location = new Point(12, 86);
        lblAccountingRevenueAccount.Name = "lblAccountingRevenueAccount";
        lblAccountingRevenueAccount.Size = new Size(88, 15);
        lblAccountingRevenueAccount.TabIndex = 21;
        lblAccountingRevenueAccount.Text = "Cuenta ingresos:";
        // 
        // slueInventoryAccount
        // 
        slueInventoryAccount.Location = new Point(171, 54);
        slueInventoryAccount.Name = "slueInventoryAccount";
        slueInventoryAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueInventoryAccount.Properties.Appearance.Options.UseFont = true;
        slueInventoryAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueInventoryAccount.Properties.NullText = "1205-01-01 Inventario de mercaderias";
        slueInventoryAccount.Properties.PopupView = gvInventoryAccount;
        slueInventoryAccount.Size = new Size(305, 22);
        slueInventoryAccount.TabIndex = 20;
        // 
        // gvInventoryAccount
        // 
        gvInventoryAccount.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvInventoryAccount.Appearance.HeaderPanel.Options.UseFont = true;
        gvInventoryAccount.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvInventoryAccount.Appearance.Row.Options.UseFont = true;
        gvInventoryAccount.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvInventoryAccount.Name = "gvInventoryAccount";
        gvInventoryAccount.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvInventoryAccount.OptionsView.ShowGroupPanel = false;
        // 
        // lblAccountingInventoryAccount
        // 
        lblAccountingInventoryAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingInventoryAccount.Appearance.Options.UseFont = true;
        lblAccountingInventoryAccount.Location = new Point(12, 58);
        lblAccountingInventoryAccount.Name = "lblAccountingInventoryAccount";
        lblAccountingInventoryAccount.Size = new Size(97, 15);
        lblAccountingInventoryAccount.TabIndex = 19;
        lblAccountingInventoryAccount.Text = "Cuenta inventario:";
        // 
        // lblAccountingAccountsTitle
        // 
        lblAccountingAccountsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAccountingAccountsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAccountingAccountsTitle.Appearance.Options.UseFont = true;
        lblAccountingAccountsTitle.Appearance.Options.UseForeColor = true;
        lblAccountingAccountsTitle.Location = new Point(12, 12);
        lblAccountingAccountsTitle.Name = "lblAccountingAccountsTitle";
        lblAccountingAccountsTitle.Size = new Size(137, 20);
        lblAccountingAccountsTitle.TabIndex = 18;
        lblAccountingAccountsTitle.Text = "1. Cuentas contables";
        // 
        // tabCosts
        // 
        tabCosts.Controls.Add(lblCostPriceHistoryTitle);
        tabCosts.Controls.Add(grdCostPriceHistory);
        tabCosts.Controls.Add(lblPricesMarginsTitle);
        tabCosts.Controls.Add(lblAnalysisBasePrice);
        tabCosts.Controls.Add(lblCostsBaseTitle);
        tabCosts.Controls.Add(spnAnalysisBasePrice);
        tabCosts.Controls.Add(lblCostCurrency);
        tabCosts.Controls.Add(lblSuggestedPrice);
        tabCosts.Controls.Add(lueCostCurrency);
        tabCosts.Controls.Add(spnSuggestedPrice);
        tabCosts.Controls.Add(lblStandardCost);
        tabCosts.Controls.Add(lblMinimumMarginPercent);
        tabCosts.Controls.Add(spnStandardCost);
        tabCosts.Controls.Add(spnMinimumMarginPercent);
        tabCosts.Controls.Add(lblReplacementCost);
        tabCosts.Controls.Add(lblTargetMarginPercent);
        tabCosts.Controls.Add(spnReplacementCost);
        tabCosts.Controls.Add(spnTargetMarginPercent);
        tabCosts.Controls.Add(lblLastCost);
        tabCosts.Controls.Add(pnlGrossMargin);
        tabCosts.Controls.Add(spnLastCost);
        tabCosts.Controls.Add(pnlGrossMarginPercent);
        tabCosts.Controls.Add(lblCostsAverageCost);
        tabCosts.Controls.Add(pnlProfitability12m);
        tabCosts.Controls.Add(spnAverageCost);
        tabCosts.Controls.Add(lblPriceUpdatedAt);
        tabCosts.Controls.Add(lblCostUpdatedAt);
        tabCosts.Controls.Add(dtPriceUpdatedAt);
        tabCosts.Controls.Add(dtCostUpdatedAt);
        tabCosts.Controls.Add(lblSimulatorTitle);
        tabCosts.Controls.Add(lblManualCostUpdate);
        tabCosts.Controls.Add(lblSimulatorCost);
        tabCosts.Controls.Add(tglManualCostUpdate);
        tabCosts.Controls.Add(spnSimulatorCost);
        tabCosts.Controls.Add(lblSimulatorPlus);
        tabCosts.Controls.Add(lblSimulatorMargin);
        tabCosts.Controls.Add(spnSimulatorPrice);
        tabCosts.Controls.Add(spnSimulatorMargin);
        tabCosts.Controls.Add(lblSimulatorPrice);
        tabCosts.Controls.Add(lblSimulatorEquals);
        tabCosts.Name = "tabCosts";
        tabCosts.Size = new Size(1418, 537);
        tabCosts.Text = "Costos y precios";
        // 
        // lblSimulatorEquals
        // 
        lblSimulatorEquals.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblSimulatorEquals.Appearance.Options.UseFont = true;
        lblSimulatorEquals.Location = new Point(334, 380);
        lblSimulatorEquals.Name = "lblSimulatorEquals";
        lblSimulatorEquals.Size = new Size(11, 21);
        lblSimulatorEquals.TabIndex = 20;
        lblSimulatorEquals.Text = "=";
        // 
        // lblSimulatorPrice
        // 
        lblSimulatorPrice.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblSimulatorPrice.Appearance.Options.UseFont = true;
        lblSimulatorPrice.Location = new Point(351, 386);
        lblSimulatorPrice.Name = "lblSimulatorPrice";
        lblSimulatorPrice.Size = new Size(31, 13);
        lblSimulatorPrice.TabIndex = 21;
        lblSimulatorPrice.Text = "Precio";
        // 
        // spnSimulatorMargin
        // 
        spnSimulatorMargin.EditValue = new decimal(new int[] { 3000, 0, 0, 131072 });
        spnSimulatorMargin.Location = new Point(257, 382);
        spnSimulatorMargin.Name = "spnSimulatorMargin";
        spnSimulatorMargin.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSimulatorMargin.Properties.Appearance.Options.UseFont = true;
        spnSimulatorMargin.Properties.Appearance.Options.UseTextOptions = true;
        spnSimulatorMargin.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSimulatorMargin.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSimulatorMargin.Properties.MaskSettings.Set("mask", "n2");
        spnSimulatorMargin.Size = new Size(64, 22);
        spnSimulatorMargin.TabIndex = 19;
        // 
        // spnSimulatorPrice
        // 
        spnSimulatorPrice.EditValue = new decimal(new int[] { 2425, 0, 0, 131072 });
        spnSimulatorPrice.Location = new Point(399, 382);
        spnSimulatorPrice.Name = "spnSimulatorPrice";
        spnSimulatorPrice.Properties.Appearance.BackColor = Color.FromArgb(245, 247, 250);
        spnSimulatorPrice.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSimulatorPrice.Properties.Appearance.Options.UseBackColor = true;
        spnSimulatorPrice.Properties.Appearance.Options.UseFont = true;
        spnSimulatorPrice.Properties.Appearance.Options.UseTextOptions = true;
        spnSimulatorPrice.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSimulatorPrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSimulatorPrice.Properties.MaskSettings.Set("mask", "n2");
        spnSimulatorPrice.Properties.ReadOnly = true;
        spnSimulatorPrice.Size = new Size(64, 22);
        spnSimulatorPrice.TabIndex = 22;
        // 
        // lblSimulatorMargin
        // 
        lblSimulatorMargin.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblSimulatorMargin.Appearance.Options.UseFont = true;
        lblSimulatorMargin.Location = new Point(194, 386);
        lblSimulatorMargin.Name = "lblSimulatorMargin";
        lblSimulatorMargin.Size = new Size(52, 13);
        lblSimulatorMargin.TabIndex = 18;
        lblSimulatorMargin.Text = "Margen %";
        // 
        // lblSimulatorPlus
        // 
        lblSimulatorPlus.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblSimulatorPlus.Appearance.Options.UseFont = true;
        lblSimulatorPlus.Location = new Point(172, 380);
        lblSimulatorPlus.Name = "lblSimulatorPlus";
        lblSimulatorPlus.Size = new Size(11, 21);
        lblSimulatorPlus.TabIndex = 17;
        lblSimulatorPlus.Text = "+";
        // 
        // spnSimulatorCost
        // 
        spnSimulatorCost.EditValue = new decimal(new int[] { 1865, 0, 0, 131072 });
        spnSimulatorCost.Location = new Point(85, 382);
        spnSimulatorCost.Name = "spnSimulatorCost";
        spnSimulatorCost.Properties.Appearance.BackColor = Color.FromArgb(245, 247, 250);
        spnSimulatorCost.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSimulatorCost.Properties.Appearance.Options.UseBackColor = true;
        spnSimulatorCost.Properties.Appearance.Options.UseFont = true;
        spnSimulatorCost.Properties.Appearance.Options.UseTextOptions = true;
        spnSimulatorCost.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSimulatorCost.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSimulatorCost.Properties.MaskSettings.Set("mask", "n2");
        spnSimulatorCost.Properties.ReadOnly = true;
        spnSimulatorCost.Size = new Size(74, 22);
        spnSimulatorCost.TabIndex = 16;
        // 
        // tglManualCostUpdate
        // 
        tglManualCostUpdate.EditValue = true;
        tglManualCostUpdate.Location = new Point(286, 156);
        tglManualCostUpdate.Name = "tglManualCostUpdate";
        tglManualCostUpdate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglManualCostUpdate.Properties.Appearance.Options.UseFont = true;
        tglManualCostUpdate.Properties.OffText = "No";
        tglManualCostUpdate.Properties.OnText = "Sí";
        tglManualCostUpdate.Size = new Size(70, 20);
        tglManualCostUpdate.TabIndex = 37;
        // 
        // lblSimulatorCost
        // 
        lblSimulatorCost.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblSimulatorCost.Appearance.Options.UseFont = true;
        lblSimulatorCost.Location = new Point(18, 386);
        lblSimulatorCost.Name = "lblSimulatorCost";
        lblSimulatorCost.Size = new Size(57, 13);
        lblSimulatorCost.TabIndex = 15;
        lblSimulatorCost.Text = "Costo base";
        // 
        // lblManualCostUpdate
        // 
        lblManualCostUpdate.Appearance.Font = new Font("Segoe UI", 9F);
        lblManualCostUpdate.Appearance.Options.UseFont = true;
        lblManualCostUpdate.Location = new Point(286, 132);
        lblManualCostUpdate.Name = "lblManualCostUpdate";
        lblManualCostUpdate.Size = new Size(43, 15);
        lblManualCostUpdate.TabIndex = 36;
        lblManualCostUpdate.Text = "Manual:";
        // 
        // lblSimulatorTitle
        // 
        lblSimulatorTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblSimulatorTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSimulatorTitle.Appearance.Options.UseFont = true;
        lblSimulatorTitle.Appearance.Options.UseForeColor = true;
        lblSimulatorTitle.Location = new Point(18, 360);
        lblSimulatorTitle.Name = "lblSimulatorTitle";
        lblSimulatorTitle.Size = new Size(93, 15);
        lblSimulatorTitle.TabIndex = 14;
        lblSimulatorTitle.Text = "Simulador simple";
        // 
        // dtCostUpdatedAt
        // 
        dtCostUpdatedAt.EditValue = new DateTime(2026, 5, 15, 8, 30, 0, 0);
        dtCostUpdatedAt.Location = new Point(286, 100);
        dtCostUpdatedAt.Name = "dtCostUpdatedAt";
        dtCostUpdatedAt.Properties.Appearance.BackColor = Color.FromArgb(245, 247, 250);
        dtCostUpdatedAt.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dtCostUpdatedAt.Properties.Appearance.Options.UseBackColor = true;
        dtCostUpdatedAt.Properties.Appearance.Options.UseFont = true;
        dtCostUpdatedAt.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dtCostUpdatedAt.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dtCostUpdatedAt.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
        dtCostUpdatedAt.Properties.DisplayFormat.FormatType = FormatType.DateTime;
        dtCostUpdatedAt.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
        dtCostUpdatedAt.Properties.EditFormat.FormatType = FormatType.DateTime;
        dtCostUpdatedAt.Properties.ReadOnly = true;
        dtCostUpdatedAt.Size = new Size(111, 22);
        dtCostUpdatedAt.TabIndex = 35;
        // 
        // dtPriceUpdatedAt
        // 
        dtPriceUpdatedAt.EditValue = new DateTime(2026, 5, 15, 9, 10, 0, 0);
        dtPriceUpdatedAt.Location = new Point(228, 333);
        dtPriceUpdatedAt.Name = "dtPriceUpdatedAt";
        dtPriceUpdatedAt.Properties.Appearance.BackColor = Color.FromArgb(245, 247, 250);
        dtPriceUpdatedAt.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dtPriceUpdatedAt.Properties.Appearance.Options.UseBackColor = true;
        dtPriceUpdatedAt.Properties.Appearance.Options.UseFont = true;
        dtPriceUpdatedAt.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dtPriceUpdatedAt.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dtPriceUpdatedAt.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
        dtPriceUpdatedAt.Properties.DisplayFormat.FormatType = FormatType.DateTime;
        dtPriceUpdatedAt.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
        dtPriceUpdatedAt.Properties.EditFormat.FormatType = FormatType.DateTime;
        dtPriceUpdatedAt.Properties.ReadOnly = true;
        dtPriceUpdatedAt.Size = new Size(112, 22);
        dtPriceUpdatedAt.TabIndex = 13;
        // 
        // lblCostUpdatedAt
        // 
        lblCostUpdatedAt.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostUpdatedAt.Appearance.Options.UseFont = true;
        lblCostUpdatedAt.Location = new Point(286, 76);
        lblCostUpdatedAt.Name = "lblCostUpdatedAt";
        lblCostUpdatedAt.Size = new Size(74, 15);
        lblCostUpdatedAt.TabIndex = 34;
        lblCostUpdatedAt.Text = "Actualización:";
        // 
        // lblPriceUpdatedAt
        // 
        lblPriceUpdatedAt.Appearance.Font = new Font("Segoe UI", 9F);
        lblPriceUpdatedAt.Appearance.Options.UseFont = true;
        lblPriceUpdatedAt.Location = new Point(228, 309);
        lblPriceUpdatedAt.Name = "lblPriceUpdatedAt";
        lblPriceUpdatedAt.Size = new Size(74, 15);
        lblPriceUpdatedAt.TabIndex = 12;
        lblPriceUpdatedAt.Text = "Actualización:";
        // 
        // spnAverageCost
        // 
        spnAverageCost.EditValue = new decimal(new int[] { 1865, 0, 0, 131072 });
        spnAverageCost.Location = new Point(158, 156);
        spnAverageCost.Name = "spnAverageCost";
        spnAverageCost.Properties.Appearance.BackColor = Color.FromArgb(245, 247, 250);
        spnAverageCost.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnAverageCost.Properties.Appearance.Options.UseBackColor = true;
        spnAverageCost.Properties.Appearance.Options.UseFont = true;
        spnAverageCost.Properties.Appearance.Options.UseTextOptions = true;
        spnAverageCost.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnAverageCost.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnAverageCost.Properties.MaskSettings.Set("mask", "n2");
        spnAverageCost.Properties.ReadOnly = true;
        spnAverageCost.Size = new Size(120, 22);
        spnAverageCost.TabIndex = 29;
        // 
        // pnlProfitability12m
        // 
        pnlProfitability12m.BorderStyle = BorderStyles.Simple;
        pnlProfitability12m.Controls.Add(lblProfitability12mCaption);
        pnlProfitability12m.Controls.Add(lblProfitability12mValue);
        pnlProfitability12m.Location = new Point(354, 305);
        pnlProfitability12m.Name = "pnlProfitability12m";
        pnlProfitability12m.Size = new Size(112, 58);
        pnlProfitability12m.TabIndex = 11;
        // 
        // lblProfitability12mValue
        // 
        lblProfitability12mValue.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblProfitability12mValue.Appearance.Options.UseFont = true;
        lblProfitability12mValue.Location = new Point(12, 29);
        lblProfitability12mValue.Name = "lblProfitability12mValue";
        lblProfitability12mValue.Size = new Size(54, 21);
        lblProfitability12mValue.TabIndex = 1;
        lblProfitability12mValue.Text = "18.40 %";
        // 
        // lblProfitability12mCaption
        // 
        lblProfitability12mCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblProfitability12mCaption.Appearance.Options.UseFont = true;
        lblProfitability12mCaption.Location = new Point(12, 8);
        lblProfitability12mCaption.Name = "lblProfitability12mCaption";
        lblProfitability12mCaption.Size = new Size(90, 13);
        lblProfitability12mCaption.TabIndex = 0;
        lblProfitability12mCaption.Text = "Rentabilidad 12m";
        // 
        // lblCostsAverageCost
        // 
        lblCostsAverageCost.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostsAverageCost.Appearance.Options.UseFont = true;
        lblCostsAverageCost.Location = new Point(18, 160);
        lblCostsAverageCost.Name = "lblCostsAverageCost";
        lblCostsAverageCost.Size = new Size(89, 15);
        lblCostsAverageCost.TabIndex = 28;
        lblCostsAverageCost.Text = "Costo promedio:";
        // 
        // pnlGrossMarginPercent
        // 
        pnlGrossMarginPercent.BorderStyle = BorderStyles.Simple;
        pnlGrossMarginPercent.Controls.Add(lblGrossMarginPercentCaption);
        pnlGrossMarginPercent.Controls.Add(lblGrossMarginPercentValue);
        pnlGrossMarginPercent.Location = new Point(354, 229);
        pnlGrossMarginPercent.Name = "pnlGrossMarginPercent";
        pnlGrossMarginPercent.Size = new Size(112, 64);
        pnlGrossMarginPercent.TabIndex = 10;
        // 
        // lblGrossMarginPercentValue
        // 
        lblGrossMarginPercentValue.Appearance.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
        lblGrossMarginPercentValue.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblGrossMarginPercentValue.Appearance.Options.UseFont = true;
        lblGrossMarginPercentValue.Appearance.Options.UseForeColor = true;
        lblGrossMarginPercentValue.Location = new Point(12, 31);
        lblGrossMarginPercentValue.Name = "lblGrossMarginPercentValue";
        lblGrossMarginPercentValue.Size = new Size(60, 23);
        lblGrossMarginPercentValue.TabIndex = 1;
        lblGrossMarginPercentValue.Text = "34.56 %";
        // 
        // lblGrossMarginPercentCaption
        // 
        lblGrossMarginPercentCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblGrossMarginPercentCaption.Appearance.Options.UseFont = true;
        lblGrossMarginPercentCaption.Location = new Point(12, 8);
        lblGrossMarginPercentCaption.Name = "lblGrossMarginPercentCaption";
        lblGrossMarginPercentCaption.Size = new Size(84, 13);
        lblGrossMarginPercentCaption.TabIndex = 0;
        lblGrossMarginPercentCaption.Text = "Margen bruto %";
        // 
        // spnLastCost
        // 
        spnLastCost.EditValue = new decimal(new int[] { 1840, 0, 0, 131072 });
        spnLastCost.Location = new Point(158, 128);
        spnLastCost.Name = "spnLastCost";
        spnLastCost.Properties.Appearance.BackColor = Color.FromArgb(245, 247, 250);
        spnLastCost.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnLastCost.Properties.Appearance.Options.UseBackColor = true;
        spnLastCost.Properties.Appearance.Options.UseFont = true;
        spnLastCost.Properties.Appearance.Options.UseTextOptions = true;
        spnLastCost.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnLastCost.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnLastCost.Properties.MaskSettings.Set("mask", "n2");
        spnLastCost.Properties.ReadOnly = true;
        spnLastCost.Size = new Size(120, 22);
        spnLastCost.TabIndex = 27;
        // 
        // pnlGrossMargin
        // 
        pnlGrossMargin.BorderStyle = BorderStyles.Simple;
        pnlGrossMargin.Controls.Add(lblGrossMarginCaption);
        pnlGrossMargin.Controls.Add(lblGrossMarginValue);
        pnlGrossMargin.Controls.Add(lblGrossMarginUnit);
        pnlGrossMargin.Location = new Point(228, 229);
        pnlGrossMargin.Name = "pnlGrossMargin";
        pnlGrossMargin.Size = new Size(112, 64);
        pnlGrossMargin.TabIndex = 9;
        // 
        // lblGrossMarginUnit
        // 
        lblGrossMarginUnit.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblGrossMarginUnit.Appearance.Options.UseFont = true;
        lblGrossMarginUnit.Location = new Point(12, 47);
        lblGrossMarginUnit.Name = "lblGrossMarginUnit";
        lblGrossMarginUnit.Size = new Size(22, 13);
        lblGrossMarginUnit.TabIndex = 2;
        lblGrossMarginUnit.Text = "USD";
        // 
        // lblGrossMarginValue
        // 
        lblGrossMarginValue.Appearance.Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
        lblGrossMarginValue.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblGrossMarginValue.Appearance.Options.UseFont = true;
        lblGrossMarginValue.Appearance.Options.UseForeColor = true;
        lblGrossMarginValue.Location = new Point(12, 27);
        lblGrossMarginValue.Name = "lblGrossMarginValue";
        lblGrossMarginValue.Size = new Size(31, 23);
        lblGrossMarginValue.TabIndex = 1;
        lblGrossMarginValue.Text = "9.85";
        // 
        // lblGrossMarginCaption
        // 
        lblGrossMarginCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblGrossMarginCaption.Appearance.Options.UseFont = true;
        lblGrossMarginCaption.Location = new Point(12, 8);
        lblGrossMarginCaption.Name = "lblGrossMarginCaption";
        lblGrossMarginCaption.Size = new Size(72, 13);
        lblGrossMarginCaption.TabIndex = 0;
        lblGrossMarginCaption.Text = "Margen bruto";
        // 
        // lblLastCost
        // 
        lblLastCost.Appearance.Font = new Font("Segoe UI", 9F);
        lblLastCost.Appearance.Options.UseFont = true;
        lblLastCost.Location = new Point(18, 132);
        lblLastCost.Name = "lblLastCost";
        lblLastCost.Size = new Size(72, 15);
        lblLastCost.TabIndex = 26;
        lblLastCost.Text = "Costo último:";
        // 
        // spnTargetMarginPercent
        // 
        spnTargetMarginPercent.EditValue = new decimal(new int[] { 3000, 0, 0, 131072 });
        spnTargetMarginPercent.Location = new Point(126, 313);
        spnTargetMarginPercent.Name = "spnTargetMarginPercent";
        spnTargetMarginPercent.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnTargetMarginPercent.Properties.Appearance.Options.UseFont = true;
        spnTargetMarginPercent.Properties.Appearance.Options.UseTextOptions = true;
        spnTargetMarginPercent.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnTargetMarginPercent.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnTargetMarginPercent.Properties.MaskSettings.Set("mask", "n2");
        spnTargetMarginPercent.Size = new Size(82, 22);
        spnTargetMarginPercent.TabIndex = 8;
        // 
        // spnReplacementCost
        // 
        spnReplacementCost.EditValue = new decimal(new int[] { 1920, 0, 0, 131072 });
        spnReplacementCost.Location = new Point(158, 100);
        spnReplacementCost.Name = "spnReplacementCost";
        spnReplacementCost.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnReplacementCost.Properties.Appearance.Options.UseFont = true;
        spnReplacementCost.Properties.Appearance.Options.UseTextOptions = true;
        spnReplacementCost.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnReplacementCost.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnReplacementCost.Properties.MaskSettings.Set("mask", "n2");
        spnReplacementCost.Size = new Size(120, 22);
        spnReplacementCost.TabIndex = 25;
        // 
        // lblTargetMarginPercent
        // 
        lblTargetMarginPercent.Appearance.Font = new Font("Segoe UI", 9F);
        lblTargetMarginPercent.Appearance.Options.UseFont = true;
        lblTargetMarginPercent.Location = new Point(18, 316);
        lblTargetMarginPercent.Name = "lblTargetMarginPercent";
        lblTargetMarginPercent.Size = new Size(103, 15);
        lblTargetMarginPercent.TabIndex = 7;
        lblTargetMarginPercent.Text = "Margen objetivo %:";
        // 
        // lblReplacementCost
        // 
        lblReplacementCost.Appearance.Font = new Font("Segoe UI", 9F);
        lblReplacementCost.Appearance.Options.UseFont = true;
        lblReplacementCost.Location = new Point(18, 104);
        lblReplacementCost.Name = "lblReplacementCost";
        lblReplacementCost.Size = new Size(92, 15);
        lblReplacementCost.TabIndex = 24;
        lblReplacementCost.Text = "Costo reposición:";
        // 
        // spnMinimumMarginPercent
        // 
        spnMinimumMarginPercent.EditValue = new decimal(new int[] { 2500, 0, 0, 131072 });
        spnMinimumMarginPercent.Location = new Point(126, 285);
        spnMinimumMarginPercent.Name = "spnMinimumMarginPercent";
        spnMinimumMarginPercent.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMinimumMarginPercent.Properties.Appearance.Options.UseFont = true;
        spnMinimumMarginPercent.Properties.Appearance.Options.UseTextOptions = true;
        spnMinimumMarginPercent.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnMinimumMarginPercent.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMinimumMarginPercent.Properties.MaskSettings.Set("mask", "n2");
        spnMinimumMarginPercent.Size = new Size(82, 22);
        spnMinimumMarginPercent.TabIndex = 6;
        // 
        // spnStandardCost
        // 
        spnStandardCost.EditValue = new decimal(new int[] { 1825, 0, 0, 131072 });
        spnStandardCost.Location = new Point(158, 72);
        spnStandardCost.Name = "spnStandardCost";
        spnStandardCost.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnStandardCost.Properties.Appearance.Options.UseFont = true;
        spnStandardCost.Properties.Appearance.Options.UseTextOptions = true;
        spnStandardCost.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnStandardCost.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnStandardCost.Properties.MaskSettings.Set("mask", "n2");
        spnStandardCost.Size = new Size(120, 22);
        spnStandardCost.TabIndex = 23;
        // 
        // lblMinimumMarginPercent
        // 
        lblMinimumMarginPercent.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumMarginPercent.Appearance.Options.UseFont = true;
        lblMinimumMarginPercent.Location = new Point(18, 288);
        lblMinimumMarginPercent.Name = "lblMinimumMarginPercent";
        lblMinimumMarginPercent.Size = new Size(102, 15);
        lblMinimumMarginPercent.TabIndex = 5;
        lblMinimumMarginPercent.Text = "Margen mínimo %:";
        // 
        // lblStandardCost
        // 
        lblStandardCost.Appearance.Font = new Font("Segoe UI", 9F);
        lblStandardCost.Appearance.Options.UseFont = true;
        lblStandardCost.Location = new Point(18, 76);
        lblStandardCost.Name = "lblStandardCost";
        lblStandardCost.Size = new Size(82, 15);
        lblStandardCost.TabIndex = 22;
        lblStandardCost.Text = "Costo estándar:";
        // 
        // spnSuggestedPrice
        // 
        spnSuggestedPrice.EditValue = new decimal(new int[] { 3290, 0, 0, 131072 });
        spnSuggestedPrice.Location = new Point(126, 257);
        spnSuggestedPrice.Name = "spnSuggestedPrice";
        spnSuggestedPrice.Properties.Appearance.BackColor = Color.FromArgb(245, 247, 250);
        spnSuggestedPrice.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSuggestedPrice.Properties.Appearance.Options.UseBackColor = true;
        spnSuggestedPrice.Properties.Appearance.Options.UseFont = true;
        spnSuggestedPrice.Properties.Appearance.Options.UseTextOptions = true;
        spnSuggestedPrice.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSuggestedPrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSuggestedPrice.Properties.MaskSettings.Set("mask", "n2");
        spnSuggestedPrice.Properties.ReadOnly = true;
        spnSuggestedPrice.Size = new Size(82, 22);
        spnSuggestedPrice.TabIndex = 4;
        // 
        // lueCostCurrency
        // 
        lueCostCurrency.Location = new Point(158, 44);
        lueCostCurrency.Name = "lueCostCurrency";
        lueCostCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCostCurrency.Properties.Appearance.Options.UseFont = true;
        lueCostCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCostCurrency.Properties.NullText = "USD - Dólar estadounidense";
        lueCostCurrency.Size = new Size(239, 22);
        lueCostCurrency.TabIndex = 21;
        // 
        // lblSuggestedPrice
        // 
        lblSuggestedPrice.Appearance.Font = new Font("Segoe UI", 9F);
        lblSuggestedPrice.Appearance.Options.UseFont = true;
        lblSuggestedPrice.Location = new Point(18, 260);
        lblSuggestedPrice.Name = "lblSuggestedPrice";
        lblSuggestedPrice.Size = new Size(85, 15);
        lblSuggestedPrice.TabIndex = 3;
        lblSuggestedPrice.Text = "Precio sugerido:";
        // 
        // lblCostCurrency
        // 
        lblCostCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostCurrency.Appearance.Options.UseFont = true;
        lblCostCurrency.Location = new Point(18, 48);
        lblCostCurrency.Name = "lblCostCurrency";
        lblCostCurrency.Size = new Size(79, 15);
        lblCostCurrency.TabIndex = 20;
        lblCostCurrency.Text = "Moneda costo:";
        // 
        // spnAnalysisBasePrice
        // 
        spnAnalysisBasePrice.EditValue = new decimal(new int[] { 2850, 0, 0, 131072 });
        spnAnalysisBasePrice.Location = new Point(126, 229);
        spnAnalysisBasePrice.Name = "spnAnalysisBasePrice";
        spnAnalysisBasePrice.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnAnalysisBasePrice.Properties.Appearance.Options.UseFont = true;
        spnAnalysisBasePrice.Properties.Appearance.Options.UseTextOptions = true;
        spnAnalysisBasePrice.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnAnalysisBasePrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnAnalysisBasePrice.Properties.MaskSettings.Set("mask", "n2");
        spnAnalysisBasePrice.Size = new Size(82, 22);
        spnAnalysisBasePrice.TabIndex = 2;
        // 
        // lblCostsBaseTitle
        // 
        lblCostsBaseTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblCostsBaseTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblCostsBaseTitle.Appearance.Options.UseFont = true;
        lblCostsBaseTitle.Appearance.Options.UseForeColor = true;
        lblCostsBaseTitle.Location = new Point(12, 12);
        lblCostsBaseTitle.Name = "lblCostsBaseTitle";
        lblCostsBaseTitle.Size = new Size(174, 20);
        lblCostsBaseTitle.TabIndex = 19;
        lblCostsBaseTitle.Text = "1. Costos base del artículo";
        // 
        // lblAnalysisBasePrice
        // 
        lblAnalysisBasePrice.Appearance.Font = new Font("Segoe UI", 9F);
        lblAnalysisBasePrice.Appearance.Options.UseFont = true;
        lblAnalysisBasePrice.Location = new Point(18, 232);
        lblAnalysisBasePrice.Name = "lblAnalysisBasePrice";
        lblAnalysisBasePrice.Size = new Size(63, 15);
        lblAnalysisBasePrice.TabIndex = 1;
        lblAnalysisBasePrice.Text = "Precio base:";
        // 
        // lblPricesMarginsTitle
        // 
        lblPricesMarginsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPricesMarginsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblPricesMarginsTitle.Appearance.Options.UseFont = true;
        lblPricesMarginsTitle.Appearance.Options.UseForeColor = true;
        lblPricesMarginsTitle.Location = new Point(12, 196);
        lblPricesMarginsTitle.Name = "lblPricesMarginsTitle";
        lblPricesMarginsTitle.Size = new Size(148, 20);
        lblPricesMarginsTitle.TabIndex = 0;
        lblPricesMarginsTitle.Text = "3. Precios y márgenes";
        // 
        // grdCostPriceHistory
        // 
        grdCostPriceHistory.DataSource = costPriceHistoryTable;
        grdCostPriceHistory.Location = new Point(506, 42);
        grdCostPriceHistory.MainView = gvCostPriceHistory;
        grdCostPriceHistory.Name = "grdCostPriceHistory";
        grdCostPriceHistory.Size = new Size(898, 362);
        grdCostPriceHistory.TabIndex = 1;
        grdCostPriceHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvCostPriceHistory });
        // 
        // gridView8
        // 
        gridView8.GridControl = grdCostPriceHistory;
        gridView8.Name = "gridView8";
        // 
        // gvCostPriceHistory
        // 
        gvCostPriceHistory.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvCostPriceHistory.Appearance.HeaderPanel.Options.UseFont = true;
        gvCostPriceHistory.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvCostPriceHistory.Appearance.Row.Options.UseFont = true;
        gvCostPriceHistory.Columns.AddRange(new GridColumn[] { colCostHistoryDate, colCostHistoryMovement, colCostHistoryDocument, colCostHistoryPreviousCost, colCostHistoryNewCost, colCostHistoryPreviousPrice, colCostHistoryNewPrice, colCostHistoryVariation, colCostHistoryUser, colCostHistoryObservation });
        gvCostPriceHistory.GridControl = grdCostPriceHistory;
        gvCostPriceHistory.Name = "gvCostPriceHistory";
        gvCostPriceHistory.OptionsBehavior.Editable = false;
        gvCostPriceHistory.OptionsView.ShowGroupPanel = false;
        gvCostPriceHistory.OptionsView.ShowIndicator = false;
        // 
        // colCostHistoryDate
        // 
        colCostHistoryDate.Caption = "Fecha";
        colCostHistoryDate.FieldName = "Fecha";
        colCostHistoryDate.Name = "colCostHistoryDate";
        colCostHistoryDate.Visible = true;
        colCostHistoryDate.VisibleIndex = 0;
        colCostHistoryDate.Width = 86;
        // 
        // colCostHistoryMovement
        // 
        colCostHistoryMovement.Caption = "Tipo movimiento";
        colCostHistoryMovement.FieldName = "TipoMovimiento";
        colCostHistoryMovement.Name = "colCostHistoryMovement";
        colCostHistoryMovement.Visible = true;
        colCostHistoryMovement.VisibleIndex = 1;
        colCostHistoryMovement.Width = 120;
        // 
        // colCostHistoryDocument
        // 
        colCostHistoryDocument.Caption = "Documento";
        colCostHistoryDocument.FieldName = "Documento";
        colCostHistoryDocument.Name = "colCostHistoryDocument";
        colCostHistoryDocument.Visible = true;
        colCostHistoryDocument.VisibleIndex = 2;
        colCostHistoryDocument.Width = 100;
        // 
        // colCostHistoryPreviousCost
        // 
        colCostHistoryPreviousCost.Caption = "Costo anterior";
        colCostHistoryPreviousCost.DisplayFormat.FormatString = "n2";
        colCostHistoryPreviousCost.DisplayFormat.FormatType = FormatType.Numeric;
        colCostHistoryPreviousCost.FieldName = "CostoAnterior";
        colCostHistoryPreviousCost.Name = "colCostHistoryPreviousCost";
        colCostHistoryPreviousCost.Visible = true;
        colCostHistoryPreviousCost.VisibleIndex = 3;
        colCostHistoryPreviousCost.Width = 92;
        // 
        // colCostHistoryNewCost
        // 
        colCostHistoryNewCost.Caption = "Costo nuevo";
        colCostHistoryNewCost.DisplayFormat.FormatString = "n2";
        colCostHistoryNewCost.DisplayFormat.FormatType = FormatType.Numeric;
        colCostHistoryNewCost.FieldName = "CostoNuevo";
        colCostHistoryNewCost.Name = "colCostHistoryNewCost";
        colCostHistoryNewCost.Visible = true;
        colCostHistoryNewCost.VisibleIndex = 4;
        colCostHistoryNewCost.Width = 86;
        // 
        // colCostHistoryPreviousPrice
        // 
        colCostHistoryPreviousPrice.Caption = "Precio anterior";
        colCostHistoryPreviousPrice.DisplayFormat.FormatString = "n2";
        colCostHistoryPreviousPrice.DisplayFormat.FormatType = FormatType.Numeric;
        colCostHistoryPreviousPrice.FieldName = "PrecioAnterior";
        colCostHistoryPreviousPrice.Name = "colCostHistoryPreviousPrice";
        colCostHistoryPreviousPrice.Visible = true;
        colCostHistoryPreviousPrice.VisibleIndex = 5;
        colCostHistoryPreviousPrice.Width = 96;
        // 
        // colCostHistoryNewPrice
        // 
        colCostHistoryNewPrice.Caption = "Precio nuevo";
        colCostHistoryNewPrice.DisplayFormat.FormatString = "n2";
        colCostHistoryNewPrice.DisplayFormat.FormatType = FormatType.Numeric;
        colCostHistoryNewPrice.FieldName = "PrecioNuevo";
        colCostHistoryNewPrice.Name = "colCostHistoryNewPrice";
        colCostHistoryNewPrice.Visible = true;
        colCostHistoryNewPrice.VisibleIndex = 6;
        colCostHistoryNewPrice.Width = 90;
        // 
        // colCostHistoryVariation
        // 
        colCostHistoryVariation.Caption = "Variación %";
        colCostHistoryVariation.DisplayFormat.FormatString = "n2";
        colCostHistoryVariation.DisplayFormat.FormatType = FormatType.Numeric;
        colCostHistoryVariation.FieldName = "Variacion";
        colCostHistoryVariation.Name = "colCostHistoryVariation";
        colCostHistoryVariation.Visible = true;
        colCostHistoryVariation.VisibleIndex = 7;
        colCostHistoryVariation.Width = 82;
        // 
        // colCostHistoryUser
        // 
        colCostHistoryUser.Caption = "Usuario / Proceso";
        colCostHistoryUser.FieldName = "UsuarioProceso";
        colCostHistoryUser.Name = "colCostHistoryUser";
        colCostHistoryUser.Visible = true;
        colCostHistoryUser.VisibleIndex = 8;
        colCostHistoryUser.Width = 118;
        // 
        // colCostHistoryObservation
        // 
        colCostHistoryObservation.Caption = "Observación";
        colCostHistoryObservation.FieldName = "Observacion";
        colCostHistoryObservation.Name = "colCostHistoryObservation";
        colCostHistoryObservation.Visible = true;
        colCostHistoryObservation.VisibleIndex = 9;
        colCostHistoryObservation.Width = 450;
        // 
        // lblCostPriceHistoryTitle
        // 
        lblCostPriceHistoryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblCostPriceHistoryTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblCostPriceHistoryTitle.Appearance.Options.UseFont = true;
        lblCostPriceHistoryTitle.Appearance.Options.UseForeColor = true;
        lblCostPriceHistoryTitle.Location = new Point(506, 12);
        lblCostPriceHistoryTitle.Name = "lblCostPriceHistoryTitle";
        lblCostPriceHistoryTitle.Size = new Size(206, 20);
        lblCostPriceHistoryTitle.TabIndex = 0;
        lblCostPriceHistoryTitle.Text = "4. Historial de costos y precios";
        // 
        // tabSales
        // 
        tabSales.Controls.Add(lblSalesPricePerformanceTitle);
        tabSales.Controls.Add(grdSalesPriceLists);
        tabSales.Controls.Add(lblSalesConfigurationTitle);
        tabSales.Controls.Add(pnlSalesKpi30d);
        tabSales.Controls.Add(lblAffectsPromotions);
        tabSales.Controls.Add(pnlSalesKpi12m);
        tabSales.Controls.Add(tglAffectsPromotions);
        tabSales.Controls.Add(pnlSalesKpiLastPrice);
        tabSales.Controls.Add(lblSalesUnit);
        tabSales.Controls.Add(pnlSalesKpiCustomers);
        tabSales.Controls.Add(lueSalesUnit);
        tabSales.Controls.Add(lblBaseSalesPrice);
        tabSales.Controls.Add(spnBaseSalesPrice);
        tabSales.Controls.Add(lueSalesCurrency);
        tabSales.Controls.Add(lblMainPriceList);
        tabSales.Controls.Add(lueMainPriceList);
        tabSales.Controls.Add(lblAllowSalesDiscount);
        tabSales.Controls.Add(tglAllowSalesDiscount);
        tabSales.Controls.Add(lblMaxDiscount);
        tabSales.Controls.Add(spnMaxDiscount);
        tabSales.Controls.Add(lblMinimumMargin);
        tabSales.Controls.Add(spnMinimumMargin);
        tabSales.Controls.Add(lblMinimumSale);
        tabSales.Controls.Add(spnMinimumSale);
        tabSales.Controls.Add(lblMinimumSaleUnit);
        tabSales.Controls.Add(lblSalesMultiple);
        tabSales.Controls.Add(spnSalesMultiple);
        tabSales.Controls.Add(lblSalesMultipleUnit);
        tabSales.Controls.Add(lblSalesCommission);
        tabSales.Controls.Add(spnSalesCommission);
        tabSales.Name = "tabSales";
        tabSales.Size = new Size(1418, 537);
        tabSales.Text = "Ventas";
        // 
        // spnSalesCommission
        // 
        spnSalesCommission.EditValue = new decimal(new int[] { 300, 0, 0, 131072 });
        spnSalesCommission.Location = new Point(165, 310);
        spnSalesCommission.Name = "spnSalesCommission";
        spnSalesCommission.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSalesCommission.Properties.Appearance.Options.UseFont = true;
        spnSalesCommission.Properties.Appearance.Options.UseTextOptions = true;
        spnSalesCommission.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSalesCommission.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSalesCommission.Properties.MaskSettings.Set("mask", "n2");
        spnSalesCommission.Size = new Size(120, 22);
        spnSalesCommission.TabIndex = 45;
        // 
        // lblSalesCommission
        // 
        lblSalesCommission.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesCommission.Appearance.Options.UseFont = true;
        lblSalesCommission.Location = new Point(17, 313);
        lblSalesCommission.Name = "lblSalesCommission";
        lblSalesCommission.Size = new Size(75, 15);
        lblSalesCommission.TabIndex = 44;
        lblSalesCommission.Text = "Comisión (%):";
        // 
        // lblSalesMultipleUnit
        // 
        lblSalesMultipleUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesMultipleUnit.Appearance.Options.UseFont = true;
        lblSalesMultipleUnit.Location = new Point(295, 282);
        lblSalesMultipleUnit.Name = "lblSalesMultipleUnit";
        lblSalesMultipleUnit.Size = new Size(25, 15);
        lblSalesMultipleUnit.TabIndex = 43;
        lblSalesMultipleUnit.Text = "UND";
        // 
        // spnSalesMultiple
        // 
        spnSalesMultiple.EditValue = new decimal(new int[] { 100, 0, 0, 131072 });
        spnSalesMultiple.Location = new Point(165, 278);
        spnSalesMultiple.Name = "spnSalesMultiple";
        spnSalesMultiple.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSalesMultiple.Properties.Appearance.Options.UseFont = true;
        spnSalesMultiple.Properties.Appearance.Options.UseTextOptions = true;
        spnSalesMultiple.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSalesMultiple.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSalesMultiple.Properties.MaskSettings.Set("mask", "n2");
        spnSalesMultiple.Size = new Size(120, 22);
        spnSalesMultiple.TabIndex = 42;
        // 
        // lblSalesMultiple
        // 
        lblSalesMultiple.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesMultiple.Appearance.Options.UseFont = true;
        lblSalesMultiple.Location = new Point(17, 281);
        lblSalesMultiple.Name = "lblSalesMultiple";
        lblSalesMultiple.Size = new Size(96, 15);
        lblSalesMultiple.TabIndex = 41;
        lblSalesMultiple.Text = "Múltiplo de venta:";
        // 
        // lblMinimumSaleUnit
        // 
        lblMinimumSaleUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumSaleUnit.Appearance.Options.UseFont = true;
        lblMinimumSaleUnit.Location = new Point(295, 250);
        lblMinimumSaleUnit.Name = "lblMinimumSaleUnit";
        lblMinimumSaleUnit.Size = new Size(25, 15);
        lblMinimumSaleUnit.TabIndex = 40;
        lblMinimumSaleUnit.Text = "UND";
        // 
        // spnMinimumSale
        // 
        spnMinimumSale.EditValue = new decimal(new int[] { 100, 0, 0, 131072 });
        spnMinimumSale.Location = new Point(165, 246);
        spnMinimumSale.Name = "spnMinimumSale";
        spnMinimumSale.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMinimumSale.Properties.Appearance.Options.UseFont = true;
        spnMinimumSale.Properties.Appearance.Options.UseTextOptions = true;
        spnMinimumSale.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnMinimumSale.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMinimumSale.Properties.MaskSettings.Set("mask", "n2");
        spnMinimumSale.Size = new Size(120, 22);
        spnMinimumSale.TabIndex = 39;
        // 
        // lblMinimumSale
        // 
        lblMinimumSale.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumSale.Appearance.Options.UseFont = true;
        lblMinimumSale.Location = new Point(17, 249);
        lblMinimumSale.Name = "lblMinimumSale";
        lblMinimumSale.Size = new Size(77, 15);
        lblMinimumSale.TabIndex = 38;
        lblMinimumSale.Text = "Venta mínima:";
        // 
        // spnMinimumMargin
        // 
        spnMinimumMargin.EditValue = new decimal(new int[] { 1000, 0, 0, 131072 });
        spnMinimumMargin.Location = new Point(165, 214);
        spnMinimumMargin.Name = "spnMinimumMargin";
        spnMinimumMargin.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMinimumMargin.Properties.Appearance.Options.UseFont = true;
        spnMinimumMargin.Properties.Appearance.Options.UseTextOptions = true;
        spnMinimumMargin.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnMinimumMargin.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMinimumMargin.Properties.MaskSettings.Set("mask", "n2");
        spnMinimumMargin.Size = new Size(120, 22);
        spnMinimumMargin.TabIndex = 37;
        // 
        // lblMinimumMargin
        // 
        lblMinimumMargin.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumMargin.Appearance.Options.UseFont = true;
        lblMinimumMargin.Location = new Point(17, 217);
        lblMinimumMargin.Name = "lblMinimumMargin";
        lblMinimumMargin.Size = new Size(110, 15);
        lblMinimumMargin.TabIndex = 36;
        lblMinimumMargin.Text = "Margen mínimo (%):";
        // 
        // spnMaxDiscount
        // 
        spnMaxDiscount.EditValue = new decimal(new int[] { 1500, 0, 0, 131072 });
        spnMaxDiscount.Location = new Point(165, 186);
        spnMaxDiscount.Name = "spnMaxDiscount";
        spnMaxDiscount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMaxDiscount.Properties.Appearance.Options.UseFont = true;
        spnMaxDiscount.Properties.Appearance.Options.UseTextOptions = true;
        spnMaxDiscount.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnMaxDiscount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMaxDiscount.Properties.MaskSettings.Set("mask", "n2");
        spnMaxDiscount.Size = new Size(120, 22);
        spnMaxDiscount.TabIndex = 35;
        // 
        // lblMaxDiscount
        // 
        lblMaxDiscount.Appearance.Font = new Font("Segoe UI", 9F);
        lblMaxDiscount.Appearance.Options.UseFont = true;
        lblMaxDiscount.Location = new Point(17, 189);
        lblMaxDiscount.Name = "lblMaxDiscount";
        lblMaxDiscount.Size = new Size(126, 15);
        lblMaxDiscount.TabIndex = 34;
        lblMaxDiscount.Text = "Descuento máximo (%):";
        // 
        // tglAllowSalesDiscount
        // 
        tglAllowSalesDiscount.EditValue = true;
        tglAllowSalesDiscount.Location = new Point(165, 158);
        tglAllowSalesDiscount.Name = "tglAllowSalesDiscount";
        tglAllowSalesDiscount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAllowSalesDiscount.Properties.Appearance.Options.UseFont = true;
        tglAllowSalesDiscount.Properties.OffText = "No";
        tglAllowSalesDiscount.Properties.OnText = "Sí";
        tglAllowSalesDiscount.Size = new Size(86, 20);
        tglAllowSalesDiscount.TabIndex = 33;
        // 
        // lblAllowSalesDiscount
        // 
        lblAllowSalesDiscount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowSalesDiscount.Appearance.Options.UseFont = true;
        lblAllowSalesDiscount.Location = new Point(17, 160);
        lblAllowSalesDiscount.Name = "lblAllowSalesDiscount";
        lblAllowSalesDiscount.Size = new Size(102, 15);
        lblAllowSalesDiscount.TabIndex = 32;
        lblAllowSalesDiscount.Text = "Permite descuento:";
        // 
        // lueMainPriceList
        // 
        lueMainPriceList.Location = new Point(165, 103);
        lueMainPriceList.Name = "lueMainPriceList";
        lueMainPriceList.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueMainPriceList.Properties.Appearance.Options.UseFont = true;
        lueMainPriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueMainPriceList.Properties.NullText = "Minorista";
        lueMainPriceList.Size = new Size(190, 22);
        lueMainPriceList.TabIndex = 31;
        // 
        // lblMainPriceList
        // 
        lblMainPriceList.Appearance.Font = new Font("Segoe UI", 9F);
        lblMainPriceList.Appearance.Options.UseFont = true;
        lblMainPriceList.Location = new Point(17, 106);
        lblMainPriceList.Name = "lblMainPriceList";
        lblMainPriceList.Size = new Size(133, 15);
        lblMainPriceList.TabIndex = 30;
        lblMainPriceList.Text = "Lista de precios principal:";
        // 
        // lueSalesCurrency
        // 
        lueSalesCurrency.Location = new Point(293, 75);
        lueSalesCurrency.Name = "lueSalesCurrency";
        lueSalesCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSalesCurrency.Properties.Appearance.Options.UseFont = true;
        lueSalesCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSalesCurrency.Properties.NullText = "USD";
        lueSalesCurrency.Size = new Size(62, 22);
        lueSalesCurrency.TabIndex = 29;
        // 
        // spnBaseSalesPrice
        // 
        spnBaseSalesPrice.EditValue = new decimal(new int[] { 2850, 0, 0, 131072 });
        spnBaseSalesPrice.Location = new Point(165, 75);
        spnBaseSalesPrice.Name = "spnBaseSalesPrice";
        spnBaseSalesPrice.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnBaseSalesPrice.Properties.Appearance.Options.UseFont = true;
        spnBaseSalesPrice.Properties.Appearance.Options.UseTextOptions = true;
        spnBaseSalesPrice.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnBaseSalesPrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnBaseSalesPrice.Properties.MaskSettings.Set("mask", "n2");
        spnBaseSalesPrice.Size = new Size(120, 22);
        spnBaseSalesPrice.TabIndex = 28;
        // 
        // lblBaseSalesPrice
        // 
        lblBaseSalesPrice.Appearance.Font = new Font("Segoe UI", 9F);
        lblBaseSalesPrice.Appearance.Options.UseFont = true;
        lblBaseSalesPrice.Location = new Point(17, 78);
        lblBaseSalesPrice.Name = "lblBaseSalesPrice";
        lblBaseSalesPrice.Size = new Size(63, 15);
        lblBaseSalesPrice.TabIndex = 27;
        lblBaseSalesPrice.Text = "Precio base:";
        // 
        // lueSalesUnit
        // 
        lueSalesUnit.Location = new Point(165, 47);
        lueSalesUnit.Name = "lueSalesUnit";
        lueSalesUnit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSalesUnit.Properties.Appearance.Options.UseFont = true;
        lueSalesUnit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSalesUnit.Properties.NullText = "UND - Unidad";
        lueSalesUnit.Size = new Size(190, 22);
        lueSalesUnit.TabIndex = 26;
        // 
        // pnlSalesKpiCustomers
        // 
        pnlSalesKpiCustomers.BorderStyle = BorderStyles.Simple;
        pnlSalesKpiCustomers.Controls.Add(lblSalesKpiCustomersCaption);
        pnlSalesKpiCustomers.Controls.Add(lblSalesKpiCustomersValue);
        pnlSalesKpiCustomers.Location = new Point(910, 252);
        pnlSalesKpiCustomers.Name = "pnlSalesKpiCustomers";
        pnlSalesKpiCustomers.Size = new Size(156, 57);
        pnlSalesKpiCustomers.TabIndex = 5;
        // 
        // lblSalesKpiCustomersValue
        // 
        lblSalesKpiCustomersValue.Appearance.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
        lblSalesKpiCustomersValue.Appearance.Options.UseFont = true;
        lblSalesKpiCustomersValue.Location = new Point(36, 25);
        lblSalesKpiCustomersValue.Name = "lblSalesKpiCustomersValue";
        lblSalesKpiCustomersValue.Size = new Size(22, 28);
        lblSalesKpiCustomersValue.TabIndex = 1;
        lblSalesKpiCustomersValue.Text = "86";
        // 
        // lblSalesKpiCustomersCaption
        // 
        lblSalesKpiCustomersCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesKpiCustomersCaption.Appearance.ForeColor = Color.FromArgb(38, 63, 99);
        lblSalesKpiCustomersCaption.Appearance.Options.UseFont = true;
        lblSalesKpiCustomersCaption.Appearance.Options.UseForeColor = true;
        lblSalesKpiCustomersCaption.Location = new Point(13, 5);
        lblSalesKpiCustomersCaption.Name = "lblSalesKpiCustomersCaption";
        lblSalesKpiCustomersCaption.Size = new Size(82, 15);
        lblSalesKpiCustomersCaption.TabIndex = 0;
        lblSalesKpiCustomersCaption.Text = "Clientes activos";
        // 
        // lblSalesUnit
        // 
        lblSalesUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesUnit.Appearance.Options.UseFont = true;
        lblSalesUnit.Location = new Point(17, 50);
        lblSalesUnit.Name = "lblSalesUnit";
        lblSalesUnit.Size = new Size(89, 15);
        lblSalesUnit.TabIndex = 25;
        lblSalesUnit.Text = "Unidad de venta:";
        // 
        // pnlSalesKpiLastPrice
        // 
        pnlSalesKpiLastPrice.BorderStyle = BorderStyles.Simple;
        pnlSalesKpiLastPrice.Controls.Add(lblSalesKpiLastPriceCaption);
        pnlSalesKpiLastPrice.Controls.Add(lblSalesKpiLastPriceValue);
        pnlSalesKpiLastPrice.Location = new Point(748, 252);
        pnlSalesKpiLastPrice.Name = "pnlSalesKpiLastPrice";
        pnlSalesKpiLastPrice.Size = new Size(156, 57);
        pnlSalesKpiLastPrice.TabIndex = 4;
        // 
        // lblSalesKpiLastPriceValue
        // 
        lblSalesKpiLastPriceValue.Appearance.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
        lblSalesKpiLastPriceValue.Appearance.Options.UseFont = true;
        lblSalesKpiLastPriceValue.Location = new Point(37, 25);
        lblSalesKpiLastPriceValue.Name = "lblSalesKpiLastPriceValue";
        lblSalesKpiLastPriceValue.Size = new Size(94, 28);
        lblSalesKpiLastPriceValue.TabIndex = 1;
        lblSalesKpiLastPriceValue.Text = "28.50 USD";
        // 
        // lblSalesKpiLastPriceCaption
        // 
        lblSalesKpiLastPriceCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesKpiLastPriceCaption.Appearance.ForeColor = Color.FromArgb(38, 63, 99);
        lblSalesKpiLastPriceCaption.Appearance.Options.UseFont = true;
        lblSalesKpiLastPriceCaption.Appearance.Options.UseForeColor = true;
        lblSalesKpiLastPriceCaption.Location = new Point(14, 5);
        lblSalesKpiLastPriceCaption.Name = "lblSalesKpiLastPriceCaption";
        lblSalesKpiLastPriceCaption.Size = new Size(72, 15);
        lblSalesKpiLastPriceCaption.TabIndex = 0;
        lblSalesKpiLastPriceCaption.Text = "Último precio";
        // 
        // tglAffectsPromotions
        // 
        tglAffectsPromotions.EditValue = true;
        tglAffectsPromotions.Location = new Point(165, 131);
        tglAffectsPromotions.Name = "tglAffectsPromotions";
        tglAffectsPromotions.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAffectsPromotions.Properties.Appearance.Options.UseFont = true;
        tglAffectsPromotions.Properties.OffText = "No";
        tglAffectsPromotions.Properties.OnText = "Sí";
        tglAffectsPromotions.Size = new Size(86, 20);
        tglAffectsPromotions.TabIndex = 10;
        // 
        // pnlSalesKpi12m
        // 
        pnlSalesKpi12m.BorderStyle = BorderStyles.Simple;
        pnlSalesKpi12m.Controls.Add(lblSalesKpi12mCaption);
        pnlSalesKpi12m.Controls.Add(lblSalesKpi12mValue);
        pnlSalesKpi12m.Location = new Point(586, 252);
        pnlSalesKpi12m.Name = "pnlSalesKpi12m";
        pnlSalesKpi12m.Size = new Size(156, 57);
        pnlSalesKpi12m.TabIndex = 3;
        // 
        // lblSalesKpi12mValue
        // 
        lblSalesKpi12mValue.Appearance.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
        lblSalesKpi12mValue.Appearance.Options.UseFont = true;
        lblSalesKpi12mValue.Location = new Point(10, 25);
        lblSalesKpi12mValue.Name = "lblSalesKpi12mValue";
        lblSalesKpi12mValue.Size = new Size(134, 28);
        lblSalesKpi12mValue.TabIndex = 1;
        lblSalesKpi12mValue.Text = "12,420.00 UND";
        // 
        // lblSalesKpi12mCaption
        // 
        lblSalesKpi12mCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesKpi12mCaption.Appearance.ForeColor = Color.FromArgb(38, 63, 99);
        lblSalesKpi12mCaption.Appearance.Options.UseFont = true;
        lblSalesKpi12mCaption.Appearance.Options.UseForeColor = true;
        lblSalesKpi12mCaption.Location = new Point(10, 5);
        lblSalesKpi12mCaption.Name = "lblSalesKpi12mCaption";
        lblSalesKpi12mCaption.Size = new Size(61, 15);
        lblSalesKpi12mCaption.TabIndex = 0;
        lblSalesKpi12mCaption.Text = "Ventas 12m";
        // 
        // lblAffectsPromotions
        // 
        lblAffectsPromotions.Appearance.Font = new Font("Segoe UI", 9F);
        lblAffectsPromotions.Appearance.Options.UseFont = true;
        lblAffectsPromotions.Location = new Point(17, 133);
        lblAffectsPromotions.Name = "lblAffectsPromotions";
        lblAffectsPromotions.Size = new Size(110, 15);
        lblAffectsPromotions.TabIndex = 9;
        lblAffectsPromotions.Text = "Afecta promociones:";
        // 
        // pnlSalesKpi30d
        // 
        pnlSalesKpi30d.BorderStyle = BorderStyles.Simple;
        pnlSalesKpi30d.Controls.Add(lblSalesKpi30dCaption);
        pnlSalesKpi30d.Controls.Add(lblSalesKpi30dValue);
        pnlSalesKpi30d.Location = new Point(424, 252);
        pnlSalesKpi30d.Name = "pnlSalesKpi30d";
        pnlSalesKpi30d.Size = new Size(156, 57);
        pnlSalesKpi30d.TabIndex = 2;
        // 
        // lblSalesKpi30dValue
        // 
        lblSalesKpi30dValue.Appearance.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
        lblSalesKpi30dValue.Appearance.Options.UseFont = true;
        lblSalesKpi30dValue.Location = new Point(15, 25);
        lblSalesKpi30dValue.Name = "lblSalesKpi30dValue";
        lblSalesKpi30dValue.Size = new Size(122, 28);
        lblSalesKpi30dValue.TabIndex = 1;
        lblSalesKpi30dValue.Text = "1,050.00 UND";
        // 
        // lblSalesKpi30dCaption
        // 
        lblSalesKpi30dCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesKpi30dCaption.Appearance.ForeColor = Color.FromArgb(38, 63, 99);
        lblSalesKpi30dCaption.Appearance.Options.UseFont = true;
        lblSalesKpi30dCaption.Appearance.Options.UseForeColor = true;
        lblSalesKpi30dCaption.Location = new Point(15, 5);
        lblSalesKpi30dCaption.Name = "lblSalesKpi30dCaption";
        lblSalesKpi30dCaption.Size = new Size(57, 15);
        lblSalesKpi30dCaption.TabIndex = 0;
        lblSalesKpi30dCaption.Text = "Ventas 30d";
        // 
        // lblSalesConfigurationTitle
        // 
        lblSalesConfigurationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSalesConfigurationTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSalesConfigurationTitle.Appearance.Options.UseFont = true;
        lblSalesConfigurationTitle.Appearance.Options.UseForeColor = true;
        lblSalesConfigurationTitle.Location = new Point(12, 12);
        lblSalesConfigurationTitle.Name = "lblSalesConfigurationTitle";
        lblSalesConfigurationTitle.Size = new Size(181, 20);
        lblSalesConfigurationTitle.TabIndex = 24;
        lblSalesConfigurationTitle.Text = "1. Configuración comercial";
        // 
        // grdSalesPriceLists
        // 
        grdSalesPriceLists.DataSource = salesPriceListsTable;
        grdSalesPriceLists.Location = new Point(424, 44);
        grdSalesPriceLists.MainView = gvSalesPriceLists;
        grdSalesPriceLists.Name = "grdSalesPriceLists";
        grdSalesPriceLists.RepositoryItems.AddRange(new RepositoryItem[] { repoSalesPriceListActive });
        grdSalesPriceLists.Size = new Size(642, 180);
        grdSalesPriceLists.TabIndex = 1;
        grdSalesPriceLists.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvSalesPriceLists });
        // 
        // gridView9
        // 
        gridView9.GridControl = grdSalesPriceLists;
        gridView9.Name = "gridView9";
        // 
        // repoSalesPriceListActive
        // 
        repoSalesPriceListActive.AutoHeight = false;
        repoSalesPriceListActive.Name = "repoSalesPriceListActive";
        // 
        // gvSalesPriceLists
        // 
        gvSalesPriceLists.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvSalesPriceLists.Appearance.HeaderPanel.Options.UseFont = true;
        gvSalesPriceLists.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvSalesPriceLists.Appearance.Row.Options.UseFont = true;
        gvSalesPriceLists.Columns.AddRange(new GridColumn[] { colSalesPriceListName, colSalesPriceListCurrency, colSalesPriceListPrice, colSalesPriceListMargin, colSalesPriceListValidFrom, colSalesPriceListActive });
        gvSalesPriceLists.GridControl = grdSalesPriceLists;
        gvSalesPriceLists.Name = "gvSalesPriceLists";
        gvSalesPriceLists.OptionsBehavior.Editable = false;
        gvSalesPriceLists.OptionsView.ShowGroupPanel = false;
        gvSalesPriceLists.OptionsView.ShowIndicator = false;
        // 
        // colSalesPriceListName
        // 
        colSalesPriceListName.Caption = "Lista de precio";
        colSalesPriceListName.FieldName = "ListaPrecio";
        colSalesPriceListName.Name = "colSalesPriceListName";
        colSalesPriceListName.Visible = true;
        colSalesPriceListName.VisibleIndex = 0;
        colSalesPriceListName.Width = 118;
        // 
        // colSalesPriceListCurrency
        // 
        colSalesPriceListCurrency.Caption = "Moneda";
        colSalesPriceListCurrency.FieldName = "Moneda";
        colSalesPriceListCurrency.Name = "colSalesPriceListCurrency";
        colSalesPriceListCurrency.Visible = true;
        colSalesPriceListCurrency.VisibleIndex = 1;
        colSalesPriceListCurrency.Width = 74;
        // 
        // colSalesPriceListPrice
        // 
        colSalesPriceListPrice.Caption = "Precio";
        colSalesPriceListPrice.FieldName = "Precio";
        colSalesPriceListPrice.Name = "colSalesPriceListPrice";
        colSalesPriceListPrice.Visible = true;
        colSalesPriceListPrice.VisibleIndex = 2;
        colSalesPriceListPrice.Width = 84;
        // 
        // colSalesPriceListMargin
        // 
        colSalesPriceListMargin.Caption = "Margen %";
        colSalesPriceListMargin.FieldName = "Margen";
        colSalesPriceListMargin.Name = "colSalesPriceListMargin";
        colSalesPriceListMargin.Visible = true;
        colSalesPriceListMargin.VisibleIndex = 3;
        colSalesPriceListMargin.Width = 84;
        // 
        // colSalesPriceListValidFrom
        // 
        colSalesPriceListValidFrom.Caption = "Vigencia desde";
        colSalesPriceListValidFrom.FieldName = "Vigencia";
        colSalesPriceListValidFrom.Name = "colSalesPriceListValidFrom";
        colSalesPriceListValidFrom.Visible = true;
        colSalesPriceListValidFrom.VisibleIndex = 4;
        colSalesPriceListValidFrom.Width = 118;
        // 
        // colSalesPriceListActive
        // 
        colSalesPriceListActive.Caption = "Activa";
        colSalesPriceListActive.ColumnEdit = repoSalesPriceListActive;
        colSalesPriceListActive.FieldName = "Activa";
        colSalesPriceListActive.Name = "colSalesPriceListActive";
        colSalesPriceListActive.Visible = true;
        colSalesPriceListActive.VisibleIndex = 5;
        colSalesPriceListActive.Width = 58;
        // 
        // lblSalesPricePerformanceTitle
        // 
        lblSalesPricePerformanceTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSalesPricePerformanceTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSalesPricePerformanceTitle.Appearance.Options.UseFont = true;
        lblSalesPricePerformanceTitle.Appearance.Options.UseForeColor = true;
        lblSalesPricePerformanceTitle.Location = new Point(424, 12);
        lblSalesPricePerformanceTitle.Name = "lblSalesPricePerformanceTitle";
        lblSalesPricePerformanceTitle.Size = new Size(215, 20);
        lblSalesPricePerformanceTitle.TabIndex = 0;
        lblSalesPricePerformanceTitle.Text = "3. Listas de precio y desempeño";
        // 
        // tabPurchases
        // 
        tabPurchases.Controls.Add(labelControl1);
        tabPurchases.Controls.Add(lookUpEdit1);
        tabPurchases.Controls.Add(pnlPurchaseKpiCompliance);
        tabPurchases.Controls.Add(lblPurchasesHistoryTitle);
        tabPurchases.Controls.Add(grdPurchaseHistory);
        tabPurchases.Controls.Add(lblPurchasesConfigurationTitle);
        tabPurchases.Controls.Add(pnlPurchaseKpiLast);
        tabPurchases.Controls.Add(lblPurchaseApprovalRequired);
        tabPurchases.Controls.Add(pnlPurchaseKpiAverage);
        tabPurchases.Controls.Add(tglPurchaseApprovalRequired);
        tabPurchases.Controls.Add(pnlPurchaseKpiLeadTime);
        tabPurchases.Controls.Add(lblSupplierBackorderAllowed);
        tabPurchases.Controls.Add(tglSupplierBackorderAllowed);
        tabPurchases.Controls.Add(memReceivingNote);
        tabPurchases.Controls.Add(lblPurchaseOnDemand);
        tabPurchases.Controls.Add(lblReceivingNote);
        tabPurchases.Controls.Add(tglPurchaseOnDemand);
        tabPurchases.Controls.Add(memPurchasePolicy);
        tabPurchases.Controls.Add(lblPurchasePolicy);
        tabPurchases.Name = "tabPurchases";
        tabPurchases.Size = new Size(1418, 537);
        tabPurchases.Text = "Compras";
        // 
        // lblPurchasePolicy
        // 
        lblPurchasePolicy.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchasePolicy.Appearance.Options.UseFont = true;
        lblPurchasePolicy.Location = new Point(18, 161);
        lblPurchasePolicy.Name = "lblPurchasePolicy";
        lblPurchasePolicy.Size = new Size(42, 15);
        lblPurchasePolicy.TabIndex = 15;
        lblPurchasePolicy.Text = "Política:";
        // 
        // memPurchasePolicy
        // 
        memPurchasePolicy.EditValue = "Comprar a proveedores activos y certificados.";
        memPurchasePolicy.Location = new Point(138, 159);
        memPurchasePolicy.Name = "memPurchasePolicy";
        memPurchasePolicy.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memPurchasePolicy.Properties.Appearance.Options.UseFont = true;
        memPurchasePolicy.Size = new Size(203, 54);
        memPurchasePolicy.TabIndex = 16;
        // 
        // tglPurchaseOnDemand
        // 
        tglPurchaseOnDemand.Location = new Point(138, 131);
        tglPurchaseOnDemand.Name = "tglPurchaseOnDemand";
        tglPurchaseOnDemand.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglPurchaseOnDemand.Properties.Appearance.Options.UseFont = true;
        tglPurchaseOnDemand.Properties.OffText = "No";
        tglPurchaseOnDemand.Properties.OnText = "Sí";
        tglPurchaseOnDemand.Size = new Size(86, 20);
        tglPurchaseOnDemand.TabIndex = 8;
        // 
        // lblReceivingNote
        // 
        lblReceivingNote.Appearance.Font = new Font("Segoe UI", 9F);
        lblReceivingNote.Appearance.Options.UseFont = true;
        lblReceivingNote.Location = new Point(18, 221);
        lblReceivingNote.Name = "lblReceivingNote";
        lblReceivingNote.Size = new Size(58, 15);
        lblReceivingNote.TabIndex = 17;
        lblReceivingNote.Text = "Recepción:";
        // 
        // lblPurchaseOnDemand
        // 
        lblPurchaseOnDemand.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseOnDemand.Appearance.Options.UseFont = true;
        lblPurchaseOnDemand.Location = new Point(18, 133);
        lblPurchaseOnDemand.Name = "lblPurchaseOnDemand";
        lblPurchaseOnDemand.Size = new Size(112, 15);
        lblPurchaseOnDemand.TabIndex = 7;
        lblPurchaseOnDemand.Text = "Compra bajo pedido:";
        // 
        // memReceivingNote
        // 
        memReceivingNote.EditValue = "Verificar empaque, lote y vencimiento.";
        memReceivingNote.Location = new Point(137, 219);
        memReceivingNote.Name = "memReceivingNote";
        memReceivingNote.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memReceivingNote.Properties.Appearance.Options.UseFont = true;
        memReceivingNote.Size = new Size(203, 54);
        memReceivingNote.TabIndex = 18;
        // 
        // tglSupplierBackorderAllowed
        // 
        tglSupplierBackorderAllowed.EditValue = true;
        tglSupplierBackorderAllowed.Location = new Point(137, 103);
        tglSupplierBackorderAllowed.Name = "tglSupplierBackorderAllowed";
        tglSupplierBackorderAllowed.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglSupplierBackorderAllowed.Properties.Appearance.Options.UseFont = true;
        tglSupplierBackorderAllowed.Properties.OffText = "No";
        tglSupplierBackorderAllowed.Properties.OnText = "Sí";
        tglSupplierBackorderAllowed.Size = new Size(86, 20);
        tglSupplierBackorderAllowed.TabIndex = 6;
        // 
        // lblSupplierBackorderAllowed
        // 
        lblSupplierBackorderAllowed.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierBackorderAllowed.Appearance.Options.UseFont = true;
        lblSupplierBackorderAllowed.Location = new Point(18, 105);
        lblSupplierBackorderAllowed.Name = "lblSupplierBackorderAllowed";
        lblSupplierBackorderAllowed.Size = new Size(113, 15);
        lblSupplierBackorderAllowed.TabIndex = 5;
        lblSupplierBackorderAllowed.Text = "Backorder proveedor:";
        // 
        // pnlPurchaseKpiLeadTime
        // 
        pnlPurchaseKpiLeadTime.BorderStyle = BorderStyles.Simple;
        pnlPurchaseKpiLeadTime.Controls.Add(lblPurchaseKpiLeadTimeCaption);
        pnlPurchaseKpiLeadTime.Controls.Add(lblPurchaseKpiLeadTimeValue);
        pnlPurchaseKpiLeadTime.Location = new Point(745, 309);
        pnlPurchaseKpiLeadTime.Name = "pnlPurchaseKpiLeadTime";
        pnlPurchaseKpiLeadTime.Size = new Size(165, 58);
        pnlPurchaseKpiLeadTime.TabIndex = 4;
        // 
        // lblPurchaseKpiLeadTimeValue
        // 
        lblPurchaseKpiLeadTimeValue.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblPurchaseKpiLeadTimeValue.Appearance.Options.UseFont = true;
        lblPurchaseKpiLeadTimeValue.Location = new Point(12, 27);
        lblPurchaseKpiLeadTimeValue.Name = "lblPurchaseKpiLeadTimeValue";
        lblPurchaseKpiLeadTimeValue.Size = new Size(55, 21);
        lblPurchaseKpiLeadTimeValue.TabIndex = 1;
        lblPurchaseKpiLeadTimeValue.Text = "5.2 días";
        // 
        // lblPurchaseKpiLeadTimeCaption
        // 
        lblPurchaseKpiLeadTimeCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchaseKpiLeadTimeCaption.Appearance.Options.UseFont = true;
        lblPurchaseKpiLeadTimeCaption.Location = new Point(12, 8);
        lblPurchaseKpiLeadTimeCaption.Name = "lblPurchaseKpiLeadTimeCaption";
        lblPurchaseKpiLeadTimeCaption.Size = new Size(82, 13);
        lblPurchaseKpiLeadTimeCaption.TabIndex = 0;
        lblPurchaseKpiLeadTimeCaption.Text = "Lead time prom.";
        // 
        // tglPurchaseApprovalRequired
        // 
        tglPurchaseApprovalRequired.Location = new Point(137, 75);
        tglPurchaseApprovalRequired.Name = "tglPurchaseApprovalRequired";
        tglPurchaseApprovalRequired.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglPurchaseApprovalRequired.Properties.Appearance.Options.UseFont = true;
        tglPurchaseApprovalRequired.Properties.OffText = "No";
        tglPurchaseApprovalRequired.Properties.OnText = "Sí";
        tglPurchaseApprovalRequired.Size = new Size(86, 20);
        tglPurchaseApprovalRequired.TabIndex = 4;
        // 
        // pnlPurchaseKpiAverage
        // 
        pnlPurchaseKpiAverage.BorderStyle = BorderStyles.Simple;
        pnlPurchaseKpiAverage.Controls.Add(lblPurchaseKpiAverageCaption);
        pnlPurchaseKpiAverage.Controls.Add(lblPurchaseKpiAverageValue);
        pnlPurchaseKpiAverage.Location = new Point(567, 309);
        pnlPurchaseKpiAverage.Name = "pnlPurchaseKpiAverage";
        pnlPurchaseKpiAverage.Size = new Size(165, 58);
        pnlPurchaseKpiAverage.TabIndex = 3;
        // 
        // lblPurchaseKpiAverageValue
        // 
        lblPurchaseKpiAverageValue.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblPurchaseKpiAverageValue.Appearance.Options.UseFont = true;
        lblPurchaseKpiAverageValue.Location = new Point(12, 27);
        lblPurchaseKpiAverageValue.Name = "lblPurchaseKpiAverageValue";
        lblPurchaseKpiAverageValue.Size = new Size(72, 21);
        lblPurchaseKpiAverageValue.TabIndex = 1;
        lblPurchaseKpiAverageValue.Text = "18.60 USD";
        // 
        // lblPurchaseKpiAverageCaption
        // 
        lblPurchaseKpiAverageCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchaseKpiAverageCaption.Appearance.Options.UseFont = true;
        lblPurchaseKpiAverageCaption.Location = new Point(12, 8);
        lblPurchaseKpiAverageCaption.Name = "lblPurchaseKpiAverageCaption";
        lblPurchaseKpiAverageCaption.Size = new Size(73, 13);
        lblPurchaseKpiAverageCaption.TabIndex = 0;
        lblPurchaseKpiAverageCaption.Text = "Promedio 12m";
        // 
        // lblPurchaseApprovalRequired
        // 
        lblPurchaseApprovalRequired.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseApprovalRequired.Appearance.Options.UseFont = true;
        lblPurchaseApprovalRequired.Location = new Point(18, 77);
        lblPurchaseApprovalRequired.Name = "lblPurchaseApprovalRequired";
        lblPurchaseApprovalRequired.Size = new Size(112, 15);
        lblPurchaseApprovalRequired.TabIndex = 3;
        lblPurchaseApprovalRequired.Text = "Requiere aprobación:";
        // 
        // pnlPurchaseKpiLast
        // 
        pnlPurchaseKpiLast.BorderStyle = BorderStyles.Simple;
        pnlPurchaseKpiLast.Controls.Add(lblPurchaseKpiLastCaption);
        pnlPurchaseKpiLast.Controls.Add(lblPurchaseKpiLastValue);
        pnlPurchaseKpiLast.Location = new Point(389, 309);
        pnlPurchaseKpiLast.Name = "pnlPurchaseKpiLast";
        pnlPurchaseKpiLast.Size = new Size(165, 58);
        pnlPurchaseKpiLast.TabIndex = 2;
        // 
        // lblPurchaseKpiLastValue
        // 
        lblPurchaseKpiLastValue.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblPurchaseKpiLastValue.Appearance.Options.UseFont = true;
        lblPurchaseKpiLastValue.Location = new Point(12, 27);
        lblPurchaseKpiLastValue.Name = "lblPurchaseKpiLastValue";
        lblPurchaseKpiLastValue.Size = new Size(72, 21);
        lblPurchaseKpiLastValue.TabIndex = 1;
        lblPurchaseKpiLastValue.Text = "18.20 USD";
        // 
        // lblPurchaseKpiLastCaption
        // 
        lblPurchaseKpiLastCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchaseKpiLastCaption.Appearance.Options.UseFont = true;
        lblPurchaseKpiLastCaption.Location = new Point(12, 8);
        lblPurchaseKpiLastCaption.Name = "lblPurchaseKpiLastCaption";
        lblPurchaseKpiLastCaption.Size = new Size(74, 13);
        lblPurchaseKpiLastCaption.TabIndex = 0;
        lblPurchaseKpiLastCaption.Text = "Última compra";
        // 
        // lblPurchasesConfigurationTitle
        // 
        lblPurchasesConfigurationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchasesConfigurationTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblPurchasesConfigurationTitle.Appearance.Options.UseFont = true;
        lblPurchasesConfigurationTitle.Appearance.Options.UseForeColor = true;
        lblPurchasesConfigurationTitle.Location = new Point(12, 12);
        lblPurchasesConfigurationTitle.Name = "lblPurchasesConfigurationTitle";
        lblPurchasesConfigurationTitle.Size = new Size(194, 20);
        lblPurchasesConfigurationTitle.TabIndex = 0;
        lblPurchasesConfigurationTitle.Text = "1. Configuración de compras";
        // 
        // grdPurchaseHistory
        // 
        grdPurchaseHistory.DataSource = purchaseHistoryTable;
        grdPurchaseHistory.Location = new Point(389, 42);
        grdPurchaseHistory.MainView = gvPurchaseHistory;
        grdPurchaseHistory.Name = "grdPurchaseHistory";
        grdPurchaseHistory.Size = new Size(882, 261);
        grdPurchaseHistory.TabIndex = 1;
        grdPurchaseHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvPurchaseHistory });
        // 
        // gridView10
        // 
        gridView10.GridControl = grdPurchaseHistory;
        gridView10.Name = "gridView10";
        // 
        // gvPurchaseHistory
        // 
        gvPurchaseHistory.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvPurchaseHistory.Appearance.HeaderPanel.Options.UseFont = true;
        gvPurchaseHistory.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvPurchaseHistory.Appearance.Row.Options.UseFont = true;
        gvPurchaseHistory.Columns.AddRange(new GridColumn[] { colPurchaseHistoryDate, colPurchaseHistoryDocument, colPurchaseHistorySupplier, colPurchaseHistoryPresentation, colPurchaseHistoryQuantity, colPurchaseHistoryUnit, colPurchaseHistoryInventoryQty, colPurchaseHistoryUnitCost, colPurchaseHistoryCurrency, colPurchaseHistoryStatus });
        gvPurchaseHistory.GridControl = grdPurchaseHistory;
        gvPurchaseHistory.Name = "gvPurchaseHistory";
        gvPurchaseHistory.OptionsBehavior.Editable = false;
        gvPurchaseHistory.OptionsView.ShowGroupPanel = false;
        // 
        // colPurchaseHistoryDate
        // 
        colPurchaseHistoryDate.Caption = "Fecha";
        colPurchaseHistoryDate.FieldName = "Fecha";
        colPurchaseHistoryDate.Name = "colPurchaseHistoryDate";
        colPurchaseHistoryDate.Visible = true;
        colPurchaseHistoryDate.VisibleIndex = 0;
        // 
        // colPurchaseHistoryDocument
        // 
        colPurchaseHistoryDocument.Caption = "Documento";
        colPurchaseHistoryDocument.FieldName = "Documento";
        colPurchaseHistoryDocument.Name = "colPurchaseHistoryDocument";
        colPurchaseHistoryDocument.Visible = true;
        colPurchaseHistoryDocument.VisibleIndex = 1;
        // 
        // colPurchaseHistorySupplier
        // 
        colPurchaseHistorySupplier.Caption = "Proveedor";
        colPurchaseHistorySupplier.FieldName = "Proveedor";
        colPurchaseHistorySupplier.Name = "colPurchaseHistorySupplier";
        colPurchaseHistorySupplier.Visible = true;
        colPurchaseHistorySupplier.VisibleIndex = 2;
        // 
        // colPurchaseHistoryPresentation
        // 
        colPurchaseHistoryPresentation.Caption = "Presentación";
        colPurchaseHistoryPresentation.FieldName = "Presentacion";
        colPurchaseHistoryPresentation.Name = "colPurchaseHistoryPresentation";
        colPurchaseHistoryPresentation.Visible = true;
        colPurchaseHistoryPresentation.VisibleIndex = 3;
        // 
        // colPurchaseHistoryQuantity
        // 
        colPurchaseHistoryQuantity.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colPurchaseHistoryQuantity.Caption = "Cant.";
        colPurchaseHistoryQuantity.FieldName = "Cantidad";
        colPurchaseHistoryQuantity.Name = "colPurchaseHistoryQuantity";
        colPurchaseHistoryQuantity.Visible = true;
        colPurchaseHistoryQuantity.VisibleIndex = 4;
        // 
        // colPurchaseHistoryUnit
        // 
        colPurchaseHistoryUnit.Caption = "Und";
        colPurchaseHistoryUnit.FieldName = "Unidad";
        colPurchaseHistoryUnit.Name = "colPurchaseHistoryUnit";
        colPurchaseHistoryUnit.Visible = true;
        colPurchaseHistoryUnit.VisibleIndex = 5;
        // 
        // colPurchaseHistoryInventoryQty
        // 
        colPurchaseHistoryInventoryQty.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colPurchaseHistoryInventoryQty.Caption = "Cant. inv.";
        colPurchaseHistoryInventoryQty.FieldName = "CantidadInventario";
        colPurchaseHistoryInventoryQty.Name = "colPurchaseHistoryInventoryQty";
        colPurchaseHistoryInventoryQty.Visible = true;
        colPurchaseHistoryInventoryQty.VisibleIndex = 6;
        // 
        // colPurchaseHistoryUnitCost
        // 
        colPurchaseHistoryUnitCost.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colPurchaseHistoryUnitCost.Caption = "Costo";
        colPurchaseHistoryUnitCost.FieldName = "CostoUnitario";
        colPurchaseHistoryUnitCost.Name = "colPurchaseHistoryUnitCost";
        colPurchaseHistoryUnitCost.Visible = true;
        colPurchaseHistoryUnitCost.VisibleIndex = 7;
        // 
        // colPurchaseHistoryCurrency
        // 
        colPurchaseHistoryCurrency.Caption = "Mon.";
        colPurchaseHistoryCurrency.FieldName = "Moneda";
        colPurchaseHistoryCurrency.Name = "colPurchaseHistoryCurrency";
        colPurchaseHistoryCurrency.Visible = true;
        colPurchaseHistoryCurrency.VisibleIndex = 8;
        // 
        // colPurchaseHistoryStatus
        // 
        colPurchaseHistoryStatus.Caption = "Estado";
        colPurchaseHistoryStatus.FieldName = "Estado";
        colPurchaseHistoryStatus.Name = "colPurchaseHistoryStatus";
        colPurchaseHistoryStatus.Visible = true;
        colPurchaseHistoryStatus.VisibleIndex = 9;
        // 
        // lblPurchasesHistoryTitle
        // 
        lblPurchasesHistoryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchasesHistoryTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblPurchasesHistoryTitle.Appearance.Options.UseFont = true;
        lblPurchasesHistoryTitle.Appearance.Options.UseForeColor = true;
        lblPurchasesHistoryTitle.Location = new Point(389, 12);
        lblPurchasesHistoryTitle.Name = "lblPurchasesHistoryTitle";
        lblPurchasesHistoryTitle.Size = new Size(252, 20);
        lblPurchasesHistoryTitle.TabIndex = 0;
        lblPurchasesHistoryTitle.Text = "4. Historial y desempeño de compras";
        // 
        // pnlPurchaseKpiCompliance
        // 
        pnlPurchaseKpiCompliance.BorderStyle = BorderStyles.Simple;
        pnlPurchaseKpiCompliance.Controls.Add(lblPurchaseKpiComplianceCaption);
        pnlPurchaseKpiCompliance.Controls.Add(lblPurchaseKpiComplianceValue);
        pnlPurchaseKpiCompliance.Location = new Point(916, 309);
        pnlPurchaseKpiCompliance.Name = "pnlPurchaseKpiCompliance";
        pnlPurchaseKpiCompliance.Size = new Size(184, 58);
        pnlPurchaseKpiCompliance.TabIndex = 5;
        // 
        // lblPurchaseKpiComplianceValue
        // 
        lblPurchaseKpiComplianceValue.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblPurchaseKpiComplianceValue.Appearance.Options.UseFont = true;
        lblPurchaseKpiComplianceValue.Location = new Point(12, 27);
        lblPurchaseKpiComplianceValue.Name = "lblPurchaseKpiComplianceValue";
        lblPurchaseKpiComplianceValue.Size = new Size(44, 21);
        lblPurchaseKpiComplianceValue.TabIndex = 1;
        lblPurchaseKpiComplianceValue.Text = "96.5%";
        // 
        // lblPurchaseKpiComplianceCaption
        // 
        lblPurchaseKpiComplianceCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchaseKpiComplianceCaption.Appearance.Options.UseFont = true;
        lblPurchaseKpiComplianceCaption.Location = new Point(12, 8);
        lblPurchaseKpiComplianceCaption.Name = "lblPurchaseKpiComplianceCaption";
        lblPurchaseKpiComplianceCaption.Size = new Size(128, 13);
        lblPurchaseKpiComplianceCaption.TabIndex = 0;
        lblPurchaseKpiComplianceCaption.Text = "Cumplimiento proveedor";
        // 
        // lookUpEdit1
        // 
        lookUpEdit1.Location = new Point(137, 47);
        lookUpEdit1.Name = "lookUpEdit1";
        lookUpEdit1.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lookUpEdit1.Properties.Appearance.Options.UseFont = true;
        lookUpEdit1.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lookUpEdit1.Properties.NullText = "UND - Unidad";
        lookUpEdit1.Size = new Size(204, 22);
        lookUpEdit1.TabIndex = 20;
        // 
        // labelControl1
        // 
        labelControl1.Appearance.Font = new Font("Segoe UI", 9F);
        labelControl1.Appearance.Options.UseFont = true;
        labelControl1.Location = new Point(18, 50);
        labelControl1.Name = "labelControl1";
        labelControl1.Size = new Size(101, 15);
        labelControl1.TabIndex = 19;
        labelControl1.Text = "Unidad de compra:";
        // 
        // tabInventory
        // 
        tabInventory.Controls.Add(btnAddWarehouseStock);
        tabInventory.Controls.Add(btnUpdateWarehouseStock);
        tabInventory.Controls.Add(btnRemoveWarehouseStock);
        tabInventory.Controls.Add(btnSetMainWarehouseStock);
        tabInventory.Controls.Add(lblInventoryLocationsRestrictionsTitle);
        tabInventory.Controls.Add(lblDefaultBinLocation);
        tabInventory.Controls.Add(lblStockByWarehouseTitle);
        tabInventory.Controls.Add(slueDefaultBinLocation);
        tabInventory.Controls.Add(grdWarehouseStock);
        tabInventory.Controls.Add(lblCoverageDays);
        tabInventory.Controls.Add(spnCoverageDays);
        tabInventory.Controls.Add(lblReplenishmentOperationTitle);
        tabInventory.Controls.Add(lblLeadTimeDays);
        tabInventory.Controls.Add(lblInventoryParametersTitle);
        tabInventory.Controls.Add(spnLeadTimeDays);
        tabInventory.Controls.Add(lblReplenishmentMethod);
        tabInventory.Controls.Add(lblGlobalMinStock);
        tabInventory.Controls.Add(lblSupplyMethod);
        tabInventory.Controls.Add(spnGlobalMinStock);
        tabInventory.Controls.Add(lueReplenishmentMethod);
        tabInventory.Controls.Add(lblGlobalMaxStock);
        tabInventory.Controls.Add(lblMainWarehouse);
        tabInventory.Controls.Add(spnGlobalMaxStock);
        tabInventory.Controls.Add(lblBlockedForMovements);
        tabInventory.Controls.Add(lueSupplyMethod);
        tabInventory.Controls.Add(tglBlockedForMovements);
        tabInventory.Controls.Add(lblGlobalReorderPoint);
        tabInventory.Controls.Add(lblInventoryOperationNote);
        tabInventory.Controls.Add(lblValuationMethod);
        tabInventory.Controls.Add(memInventoryOperationNote);
        tabInventory.Controls.Add(spnGlobalReorderPoint);
        tabInventory.Controls.Add(slueMainWarehouse);
        tabInventory.Controls.Add(lblSuggestedPurchaseQty);
        tabInventory.Controls.Add(lueValuationMethod);
        tabInventory.Controls.Add(spnSuggestedPurchaseQty);
        tabInventory.Controls.Add(lblNegativeStockPolicy);
        tabInventory.Controls.Add(lblReplenishmentApproval);
        tabInventory.Controls.Add(lueNegativeStockPolicy);
        tabInventory.Controls.Add(tglReplenishmentApproval);
        tabInventory.Controls.Add(lblAutoReplenishment);
        tabInventory.Controls.Add(tglAutoReplenishment);
        tabInventory.Controls.Add(lblManageLocations);
        tabInventory.Controls.Add(tglManageLocations);
        tabInventory.Controls.Add(lblRequiresCycleCount);
        tabInventory.Controls.Add(tglRequiresCycleCount);
        tabInventory.Controls.Add(lblAbcClassification);
        tabInventory.Controls.Add(lueAbcClassification);
        tabInventory.Controls.Add(lblInventoryControlType);
        tabInventory.Controls.Add(lueInventoryControlType);
        tabInventory.Controls.Add(lblInventoryBlockReason);
        tabInventory.Controls.Add(memInventoryBlockReason);
        tabInventory.Name = "tabInventory";
        tabInventory.Size = new Size(1418, 537);
        tabInventory.Text = "Inventario";
        // 
        // memInventoryBlockReason
        // 
        memInventoryBlockReason.Location = new Point(1063, 187);
        memInventoryBlockReason.Name = "memInventoryBlockReason";
        memInventoryBlockReason.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memInventoryBlockReason.Properties.Appearance.Options.UseFont = true;
        memInventoryBlockReason.Size = new Size(210, 49);
        memInventoryBlockReason.TabIndex = 39;
        // 
        // lblInventoryBlockReason
        // 
        lblInventoryBlockReason.Location = new Point(927, 193);
        lblInventoryBlockReason.Name = "lblInventoryBlockReason";
        lblInventoryBlockReason.Size = new Size(77, 13);
        lblInventoryBlockReason.TabIndex = 38;
        lblInventoryBlockReason.Text = "Motivo bloqueo:";
        // 
        // lueInventoryControlType
        // 
        lueInventoryControlType.Location = new Point(144, 214);
        lueInventoryControlType.Name = "lueInventoryControlType";
        lueInventoryControlType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueInventoryControlType.Properties.Appearance.Options.UseFont = true;
        lueInventoryControlType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueInventoryControlType.Properties.NullText = "";
        lueInventoryControlType.Size = new Size(180, 22);
        lueInventoryControlType.TabIndex = 35;
        // 
        // lblInventoryControlType
        // 
        lblInventoryControlType.Location = new Point(15, 218);
        lblInventoryControlType.Name = "lblInventoryControlType";
        lblInventoryControlType.Size = new Size(60, 13);
        lblInventoryControlType.TabIndex = 34;
        lblInventoryControlType.Text = "Tipo control:";
        // 
        // lueAbcClassification
        // 
        lueAbcClassification.Location = new Point(144, 186);
        lueAbcClassification.Name = "lueAbcClassification";
        lueAbcClassification.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAbcClassification.Properties.Appearance.Options.UseFont = true;
        lueAbcClassification.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAbcClassification.Properties.NullText = "";
        lueAbcClassification.Size = new Size(180, 22);
        lueAbcClassification.TabIndex = 33;
        // 
        // lblAbcClassification
        // 
        lblAbcClassification.Location = new Point(15, 190);
        lblAbcClassification.Name = "lblAbcClassification";
        lblAbcClassification.Size = new Size(85, 13);
        lblAbcClassification.TabIndex = 32;
        lblAbcClassification.Text = "Clasificación ABC:";
        // 
        // tglRequiresCycleCount
        // 
        tglRequiresCycleCount.Location = new Point(144, 158);
        tglRequiresCycleCount.Name = "tglRequiresCycleCount";
        tglRequiresCycleCount.Properties.OffText = "No";
        tglRequiresCycleCount.Properties.OnText = "Sí";
        tglRequiresCycleCount.Size = new Size(86, 18);
        tglRequiresCycleCount.TabIndex = 31;
        // 
        // lblRequiresCycleCount
        // 
        lblRequiresCycleCount.Location = new Point(15, 160);
        lblRequiresCycleCount.Name = "lblRequiresCycleCount";
        lblRequiresCycleCount.Size = new Size(69, 13);
        lblRequiresCycleCount.TabIndex = 30;
        lblRequiresCycleCount.Text = "Conteo cíclico:";
        // 
        // tglManageLocations
        // 
        tglManageLocations.EditValue = true;
        tglManageLocations.Location = new Point(144, 130);
        tglManageLocations.Name = "tglManageLocations";
        tglManageLocations.Properties.OffText = "No";
        tglManageLocations.Properties.OnText = "Sí";
        tglManageLocations.Size = new Size(86, 18);
        tglManageLocations.TabIndex = 29;
        // 
        // lblManageLocations
        // 
        lblManageLocations.Location = new Point(15, 132);
        lblManageLocations.Name = "lblManageLocations";
        lblManageLocations.Size = new Size(97, 13);
        lblManageLocations.TabIndex = 28;
        lblManageLocations.Text = "Maneja ubicaciones:";
        // 
        // tglAutoReplenishment
        // 
        tglAutoReplenishment.EditValue = true;
        tglAutoReplenishment.Location = new Point(144, 102);
        tglAutoReplenishment.Name = "tglAutoReplenishment";
        tglAutoReplenishment.Properties.OffText = "No";
        tglAutoReplenishment.Properties.OnText = "Sí";
        tglAutoReplenishment.Size = new Size(86, 18);
        tglAutoReplenishment.TabIndex = 27;
        // 
        // lblAutoReplenishment
        // 
        lblAutoReplenishment.Location = new Point(15, 104);
        lblAutoReplenishment.Name = "lblAutoReplenishment";
        lblAutoReplenishment.Size = new Size(111, 13);
        lblAutoReplenishment.TabIndex = 26;
        lblAutoReplenishment.Text = "Reposición automática:";
        // 
        // tglReplenishmentApproval
        // 
        tglReplenishmentApproval.Location = new Point(545, 215);
        tglReplenishmentApproval.Name = "tglReplenishmentApproval";
        tglReplenishmentApproval.Properties.OffText = "No";
        tglReplenishmentApproval.Properties.OnText = "Sí";
        tglReplenishmentApproval.Size = new Size(86, 18);
        tglReplenishmentApproval.TabIndex = 20;
        // 
        // lueNegativeStockPolicy
        // 
        lueNegativeStockPolicy.Location = new Point(144, 74);
        lueNegativeStockPolicy.Name = "lueNegativeStockPolicy";
        lueNegativeStockPolicy.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueNegativeStockPolicy.Properties.Appearance.Options.UseFont = true;
        lueNegativeStockPolicy.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueNegativeStockPolicy.Properties.NullText = "";
        lueNegativeStockPolicy.Size = new Size(180, 22);
        lueNegativeStockPolicy.TabIndex = 25;
        // 
        // lblReplenishmentApproval
        // 
        lblReplenishmentApproval.Location = new Point(422, 219);
        lblReplenishmentApproval.Name = "lblReplenishmentApproval";
        lblReplenishmentApproval.Size = new Size(109, 13);
        lblReplenishmentApproval.TabIndex = 19;
        lblReplenishmentApproval.Text = "Aprobación reposición:";
        // 
        // lblNegativeStockPolicy
        // 
        lblNegativeStockPolicy.Location = new Point(15, 78);
        lblNegativeStockPolicy.Name = "lblNegativeStockPolicy";
        lblNegativeStockPolicy.Size = new Size(75, 13);
        lblNegativeStockPolicy.TabIndex = 24;
        lblNegativeStockPolicy.Text = "Stock negativo:";
        // 
        // spnSuggestedPurchaseQty
        // 
        spnSuggestedPurchaseQty.EditValue = new decimal(new int[] { 500, 0, 0, 0 });
        spnSuggestedPurchaseQty.Location = new Point(782, 187);
        spnSuggestedPurchaseQty.Name = "spnSuggestedPurchaseQty";
        spnSuggestedPurchaseQty.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSuggestedPurchaseQty.Properties.Appearance.Options.UseFont = true;
        spnSuggestedPurchaseQty.Properties.Appearance.Options.UseTextOptions = true;
        spnSuggestedPurchaseQty.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSuggestedPurchaseQty.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSuggestedPurchaseQty.Size = new Size(76, 22);
        spnSuggestedPurchaseQty.TabIndex = 18;
        // 
        // lueValuationMethod
        // 
        lueValuationMethod.Location = new Point(144, 46);
        lueValuationMethod.Name = "lueValuationMethod";
        lueValuationMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueValuationMethod.Properties.Appearance.Options.UseFont = true;
        lueValuationMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueValuationMethod.Properties.NullText = "";
        lueValuationMethod.Size = new Size(180, 22);
        lueValuationMethod.TabIndex = 23;
        // 
        // lblSuggestedPurchaseQty
        // 
        lblSuggestedPurchaseQty.Location = new Point(676, 191);
        lblSuggestedPurchaseQty.Name = "lblSuggestedPurchaseQty";
        lblSuggestedPurchaseQty.Size = new Size(65, 13);
        lblSuggestedPurchaseQty.TabIndex = 17;
        lblSuggestedPurchaseQty.Text = "Compra sug.:";
        // 
        // slueMainWarehouse
        // 
        slueMainWarehouse.EditValue = "BOD01 - Matriz";
        slueMainWarehouse.Location = new Point(545, 47);
        slueMainWarehouse.Name = "slueMainWarehouse";
        slueMainWarehouse.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueMainWarehouse.Properties.Appearance.Options.UseFont = true;
        slueMainWarehouse.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueMainWarehouse.Properties.NullText = "";
        slueMainWarehouse.Properties.PopupView = gvMainWarehouse;
        slueMainWarehouse.Size = new Size(313, 22);
        slueMainWarehouse.TabIndex = 2;
        // 
        // gvMainWarehouse
        // 
        gvMainWarehouse.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvMainWarehouse.Appearance.HeaderPanel.Options.UseFont = true;
        gvMainWarehouse.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvMainWarehouse.Appearance.Row.Options.UseFont = true;
        gvMainWarehouse.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvMainWarehouse.Name = "gvMainWarehouse";
        gvMainWarehouse.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvMainWarehouse.OptionsView.ShowGroupPanel = false;
        // 
        // spnGlobalReorderPoint
        // 
        spnGlobalReorderPoint.EditValue = new decimal(new int[] { 600, 0, 0, 0 });
        spnGlobalReorderPoint.Location = new Point(545, 187);
        spnGlobalReorderPoint.Name = "spnGlobalReorderPoint";
        spnGlobalReorderPoint.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnGlobalReorderPoint.Properties.Appearance.Options.UseFont = true;
        spnGlobalReorderPoint.Properties.Appearance.Options.UseTextOptions = true;
        spnGlobalReorderPoint.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnGlobalReorderPoint.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnGlobalReorderPoint.Size = new Size(82, 22);
        spnGlobalReorderPoint.TabIndex = 16;
        // 
        // memInventoryOperationNote
        // 
        memInventoryOperationNote.EditValue = "Reposición automática según punto de reorden. Revisar quincenalmente el stock mínimo. Almacenar en zona seca.";
        memInventoryOperationNote.Location = new Point(1063, 104);
        memInventoryOperationNote.Name = "memInventoryOperationNote";
        memInventoryOperationNote.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memInventoryOperationNote.Properties.Appearance.Options.UseFont = true;
        memInventoryOperationNote.Size = new Size(210, 77);
        memInventoryOperationNote.TabIndex = 20;
        // 
        // lblValuationMethod
        // 
        lblValuationMethod.Location = new Point(15, 50);
        lblValuationMethod.Name = "lblValuationMethod";
        lblValuationMethod.Size = new Size(88, 13);
        lblValuationMethod.TabIndex = 22;
        lblValuationMethod.Text = "Método valuación:";
        // 
        // lblInventoryOperationNote
        // 
        lblInventoryOperationNote.Location = new Point(927, 107);
        lblInventoryOperationNote.Name = "lblInventoryOperationNote";
        lblInventoryOperationNote.Size = new Size(113, 13);
        lblInventoryOperationNote.TabIndex = 19;
        lblInventoryOperationNote.Text = "Observación operativa:";
        // 
        // lblGlobalReorderPoint
        // 
        lblGlobalReorderPoint.Location = new Point(422, 193);
        lblGlobalReorderPoint.Name = "lblGlobalReorderPoint";
        lblGlobalReorderPoint.Size = new Size(73, 13);
        lblGlobalReorderPoint.TabIndex = 15;
        lblGlobalReorderPoint.Text = "Punto reorden:";
        // 
        // tglBlockedForMovements
        // 
        tglBlockedForMovements.Location = new Point(1063, 76);
        tglBlockedForMovements.Name = "tglBlockedForMovements";
        tglBlockedForMovements.Properties.OffText = "No";
        tglBlockedForMovements.Properties.OnText = "Sí";
        tglBlockedForMovements.Size = new Size(86, 18);
        tglBlockedForMovements.TabIndex = 18;
        // 
        // lueSupplyMethod
        // 
        lueSupplyMethod.Location = new Point(545, 75);
        lueSupplyMethod.Name = "lueSupplyMethod";
        lueSupplyMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplyMethod.Properties.Appearance.Options.UseFont = true;
        lueSupplyMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplyMethod.Properties.NullText = "";
        lueSupplyMethod.Size = new Size(313, 22);
        lueSupplyMethod.TabIndex = 4;
        // 
        // lblBlockedForMovements
        // 
        lblBlockedForMovements.Location = new Point(927, 78);
        lblBlockedForMovements.Name = "lblBlockedForMovements";
        lblBlockedForMovements.Size = new Size(104, 13);
        lblBlockedForMovements.TabIndex = 17;
        lblBlockedForMovements.Text = "Bloquea movimientos:";
        // 
        // spnGlobalMaxStock
        // 
        spnGlobalMaxStock.EditValue = new decimal(new int[] { 2200, 0, 0, 0 });
        spnGlobalMaxStock.Location = new Point(782, 159);
        spnGlobalMaxStock.Name = "spnGlobalMaxStock";
        spnGlobalMaxStock.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnGlobalMaxStock.Properties.Appearance.Options.UseFont = true;
        spnGlobalMaxStock.Properties.Appearance.Options.UseTextOptions = true;
        spnGlobalMaxStock.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnGlobalMaxStock.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnGlobalMaxStock.Size = new Size(76, 22);
        spnGlobalMaxStock.TabIndex = 14;
        // 
        // lblMainWarehouse
        // 
        lblMainWarehouse.Location = new Point(422, 50);
        lblMainWarehouse.Name = "lblMainWarehouse";
        lblMainWarehouse.Size = new Size(82, 13);
        lblMainWarehouse.TabIndex = 1;
        lblMainWarehouse.Text = "Bodega principal:";
        // 
        // lblGlobalMaxStock
        // 
        lblGlobalMaxStock.Location = new Point(676, 163);
        lblGlobalMaxStock.Name = "lblGlobalMaxStock";
        lblGlobalMaxStock.Size = new Size(57, 13);
        lblGlobalMaxStock.TabIndex = 13;
        lblGlobalMaxStock.Text = "Stock máx.:";
        // 
        // lueReplenishmentMethod
        // 
        lueReplenishmentMethod.Location = new Point(545, 103);
        lueReplenishmentMethod.Name = "lueReplenishmentMethod";
        lueReplenishmentMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueReplenishmentMethod.Properties.Appearance.Options.UseFont = true;
        lueReplenishmentMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueReplenishmentMethod.Properties.NullText = "";
        lueReplenishmentMethod.Size = new Size(313, 22);
        lueReplenishmentMethod.TabIndex = 6;
        // 
        // spnGlobalMinStock
        // 
        spnGlobalMinStock.EditValue = new decimal(new int[] { 330, 0, 0, 0 });
        spnGlobalMinStock.Location = new Point(545, 159);
        spnGlobalMinStock.Name = "spnGlobalMinStock";
        spnGlobalMinStock.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnGlobalMinStock.Properties.Appearance.Options.UseFont = true;
        spnGlobalMinStock.Properties.Appearance.Options.UseTextOptions = true;
        spnGlobalMinStock.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnGlobalMinStock.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnGlobalMinStock.Size = new Size(82, 22);
        spnGlobalMinStock.TabIndex = 12;
        // 
        // lblSupplyMethod
        // 
        lblSupplyMethod.Location = new Point(422, 78);
        lblSupplyMethod.Name = "lblSupplyMethod";
        lblSupplyMethod.Size = new Size(77, 13);
        lblSupplyMethod.TabIndex = 3;
        lblSupplyMethod.Text = "Abastecimiento:";
        // 
        // lblGlobalMinStock
        // 
        lblGlobalMinStock.Location = new Point(422, 163);
        lblGlobalMinStock.Name = "lblGlobalMinStock";
        lblGlobalMinStock.Size = new Size(84, 13);
        lblGlobalMinStock.TabIndex = 11;
        lblGlobalMinStock.Text = "Stock mín. global:";
        // 
        // lblReplenishmentMethod
        // 
        lblReplenishmentMethod.Location = new Point(422, 107);
        lblReplenishmentMethod.Name = "lblReplenishmentMethod";
        lblReplenishmentMethod.Size = new Size(91, 13);
        lblReplenishmentMethod.TabIndex = 5;
        lblReplenishmentMethod.Text = "Método reposición:";
        // 
        // spnLeadTimeDays
        // 
        spnLeadTimeDays.EditValue = new decimal(new int[] { 5, 0, 0, 0 });
        spnLeadTimeDays.Location = new Point(782, 131);
        spnLeadTimeDays.Name = "spnLeadTimeDays";
        spnLeadTimeDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnLeadTimeDays.Properties.Appearance.Options.UseFont = true;
        spnLeadTimeDays.Properties.Appearance.Options.UseTextOptions = true;
        spnLeadTimeDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnLeadTimeDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnLeadTimeDays.Size = new Size(76, 22);
        spnLeadTimeDays.TabIndex = 10;
        // 
        // lblInventoryParametersTitle
        // 
        lblInventoryParametersTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblInventoryParametersTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblInventoryParametersTitle.Appearance.Options.UseFont = true;
        lblInventoryParametersTitle.Appearance.Options.UseForeColor = true;
        lblInventoryParametersTitle.Location = new Point(12, 12);
        lblInventoryParametersTitle.Name = "lblInventoryParametersTitle";
        lblInventoryParametersTitle.Size = new Size(187, 20);
        lblInventoryParametersTitle.TabIndex = 21;
        lblInventoryParametersTitle.Text = "1. Parámetros de inventario";
        // 
        // lblLeadTimeDays
        // 
        lblLeadTimeDays.Location = new Point(676, 135);
        lblLeadTimeDays.Name = "lblLeadTimeDays";
        lblLeadTimeDays.Size = new Size(77, 13);
        lblLeadTimeDays.TabIndex = 9;
        lblLeadTimeDays.Text = "Reposición días:";
        // 
        // lblReplenishmentOperationTitle
        // 
        lblReplenishmentOperationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblReplenishmentOperationTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblReplenishmentOperationTitle.Appearance.Options.UseFont = true;
        lblReplenishmentOperationTitle.Appearance.Options.UseForeColor = true;
        lblReplenishmentOperationTitle.Location = new Point(420, 12);
        lblReplenishmentOperationTitle.Name = "lblReplenishmentOperationTitle";
        lblReplenishmentOperationTitle.Size = new Size(175, 20);
        lblReplenishmentOperationTitle.TabIndex = 0;
        lblReplenishmentOperationTitle.Text = "2. Reposición y operación";
        // 
        // spnCoverageDays
        // 
        spnCoverageDays.EditValue = new decimal(new int[] { 30, 0, 0, 0 });
        spnCoverageDays.Location = new Point(545, 131);
        spnCoverageDays.Name = "spnCoverageDays";
        spnCoverageDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnCoverageDays.Properties.Appearance.Options.UseFont = true;
        spnCoverageDays.Properties.Appearance.Options.UseTextOptions = true;
        spnCoverageDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnCoverageDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnCoverageDays.Size = new Size(82, 22);
        spnCoverageDays.TabIndex = 8;
        // 
        // lblCoverageDays
        // 
        lblCoverageDays.Location = new Point(422, 135);
        lblCoverageDays.Name = "lblCoverageDays";
        lblCoverageDays.Size = new Size(74, 13);
        lblCoverageDays.TabIndex = 7;
        lblCoverageDays.Text = "Días cobertura:";
        // 
        // grdWarehouseStock
        // 
        grdWarehouseStock.DataSource = warehouseStockTable;
        grdWarehouseStock.Location = new Point(12, 284);
        grdWarehouseStock.MainView = gvWarehouseStock;
        grdWarehouseStock.Name = "grdWarehouseStock";
        grdWarehouseStock.Size = new Size(1261, 142);
        grdWarehouseStock.TabIndex = 1;
        grdWarehouseStock.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvWarehouseStock });
        // 
        // gridView11
        // 
        gridView11.GridControl = grdWarehouseStock;
        gridView11.Name = "gridView11";
        // 
        // gvWarehouseStock
        // 
        gvWarehouseStock.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvWarehouseStock.Appearance.HeaderPanel.Options.UseFont = true;
        gvWarehouseStock.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvWarehouseStock.Appearance.Row.Options.UseFont = true;
        gvWarehouseStock.Columns.AddRange(new GridColumn[] { colWarehouseCode, colWarehouseName, colWarehouseStockActual, colWarehouseCommitted, colWarehouseOrdered, colWarehouseAvailable, colWarehouseMinimum, colWarehouseMaximum, colWarehouseReorder, colWarehouseStatus });
        gvWarehouseStock.GridControl = grdWarehouseStock;
        gvWarehouseStock.Name = "gvWarehouseStock";
        gvWarehouseStock.OptionsBehavior.Editable = false;
        gvWarehouseStock.OptionsView.ShowGroupPanel = false;
        // 
        // colWarehouseCode
        // 
        colWarehouseCode.Caption = "Bodega";
        colWarehouseCode.FieldName = "Bodega";
        colWarehouseCode.Name = "colWarehouseCode";
        colWarehouseCode.Visible = true;
        colWarehouseCode.VisibleIndex = 0;
        colWarehouseCode.Width = 70;
        // 
        // colWarehouseName
        // 
        colWarehouseName.Caption = "Nombre bodega";
        colWarehouseName.FieldName = "NombreBodega";
        colWarehouseName.Name = "colWarehouseName";
        colWarehouseName.Visible = true;
        colWarehouseName.VisibleIndex = 1;
        colWarehouseName.Width = 140;
        // 
        // colWarehouseStockActual
        // 
        colWarehouseStockActual.Caption = "Stock actual";
        colWarehouseStockActual.FieldName = "StockActual";
        colWarehouseStockActual.Name = "colWarehouseStockActual";
        colWarehouseStockActual.Visible = true;
        colWarehouseStockActual.VisibleIndex = 2;
        // 
        // colWarehouseCommitted
        // 
        colWarehouseCommitted.Caption = "Comprometido";
        colWarehouseCommitted.FieldName = "Comprometido";
        colWarehouseCommitted.Name = "colWarehouseCommitted";
        colWarehouseCommitted.Visible = true;
        colWarehouseCommitted.VisibleIndex = 3;
        // 
        // colWarehouseOrdered
        // 
        colWarehouseOrdered.Caption = "Pedido";
        colWarehouseOrdered.FieldName = "Pedido";
        colWarehouseOrdered.Name = "colWarehouseOrdered";
        colWarehouseOrdered.Visible = true;
        colWarehouseOrdered.VisibleIndex = 4;
        // 
        // colWarehouseAvailable
        // 
        colWarehouseAvailable.Caption = "Disponible";
        colWarehouseAvailable.FieldName = "Disponible";
        colWarehouseAvailable.Name = "colWarehouseAvailable";
        colWarehouseAvailable.Visible = true;
        colWarehouseAvailable.VisibleIndex = 5;
        // 
        // colWarehouseMinimum
        // 
        colWarehouseMinimum.Caption = "Mínimo";
        colWarehouseMinimum.FieldName = "Minimo";
        colWarehouseMinimum.Name = "colWarehouseMinimum";
        colWarehouseMinimum.Visible = true;
        colWarehouseMinimum.VisibleIndex = 6;
        // 
        // colWarehouseMaximum
        // 
        colWarehouseMaximum.Caption = "Máximo";
        colWarehouseMaximum.FieldName = "Maximo";
        colWarehouseMaximum.Name = "colWarehouseMaximum";
        colWarehouseMaximum.Visible = true;
        colWarehouseMaximum.VisibleIndex = 7;
        // 
        // colWarehouseReorder
        // 
        colWarehouseReorder.Caption = "Reorden";
        colWarehouseReorder.FieldName = "Reorden";
        colWarehouseReorder.Name = "colWarehouseReorder";
        colWarehouseReorder.Visible = true;
        colWarehouseReorder.VisibleIndex = 8;
        // 
        // colWarehouseStatus
        // 
        colWarehouseStatus.Caption = "Estado";
        colWarehouseStatus.FieldName = "Estado";
        colWarehouseStatus.Name = "colWarehouseStatus";
        colWarehouseStatus.Visible = true;
        colWarehouseStatus.VisibleIndex = 9;
        // 
        // slueDefaultBinLocation
        // 
        slueDefaultBinLocation.EditValue = "A1-01-01";
        slueDefaultBinLocation.Location = new Point(1063, 46);
        slueDefaultBinLocation.Name = "slueDefaultBinLocation";
        slueDefaultBinLocation.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueDefaultBinLocation.Properties.Appearance.Options.UseFont = true;
        slueDefaultBinLocation.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueDefaultBinLocation.Properties.NullText = "";
        slueDefaultBinLocation.Properties.PopupView = gvDefaultBinLocation;
        slueDefaultBinLocation.Size = new Size(210, 22);
        slueDefaultBinLocation.TabIndex = 2;
        // 
        // gvDefaultBinLocation
        // 
        gvDefaultBinLocation.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvDefaultBinLocation.Appearance.HeaderPanel.Options.UseFont = true;
        gvDefaultBinLocation.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvDefaultBinLocation.Appearance.Row.Options.UseFont = true;
        gvDefaultBinLocation.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvDefaultBinLocation.Name = "gvDefaultBinLocation";
        gvDefaultBinLocation.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvDefaultBinLocation.OptionsView.ShowGroupPanel = false;
        // 
        // lblStockByWarehouseTitle
        // 
        lblStockByWarehouseTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblStockByWarehouseTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblStockByWarehouseTitle.Appearance.Options.UseFont = true;
        lblStockByWarehouseTitle.Appearance.Options.UseForeColor = true;
        lblStockByWarehouseTitle.Location = new Point(12, 252);
        lblStockByWarehouseTitle.Name = "lblStockByWarehouseTitle";
        lblStockByWarehouseTitle.Size = new Size(138, 20);
        lblStockByWarehouseTitle.TabIndex = 0;
        lblStockByWarehouseTitle.Text = "4. Stock por bodega";
        // 
        // lblDefaultBinLocation
        // 
        lblDefaultBinLocation.Location = new Point(927, 50);
        lblDefaultBinLocation.Name = "lblDefaultBinLocation";
        lblDefaultBinLocation.Size = new Size(89, 13);
        lblDefaultBinLocation.TabIndex = 1;
        lblDefaultBinLocation.Text = "Ubicación defecto:";
        // 
        // lblInventoryLocationsRestrictionsTitle
        // 
        lblInventoryLocationsRestrictionsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblInventoryLocationsRestrictionsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblInventoryLocationsRestrictionsTitle.Appearance.Options.UseFont = true;
        lblInventoryLocationsRestrictionsTitle.Appearance.Options.UseForeColor = true;
        lblInventoryLocationsRestrictionsTitle.Location = new Point(921, 12);
        lblInventoryLocationsRestrictionsTitle.Name = "lblInventoryLocationsRestrictionsTitle";
        lblInventoryLocationsRestrictionsTitle.Size = new Size(197, 20);
        lblInventoryLocationsRestrictionsTitle.TabIndex = 0;
        lblInventoryLocationsRestrictionsTitle.Text = "3. Ubicaciones / restricciones";
        // 
        // btnSetMainWarehouseStock
        // 
        btnSetMainWarehouseStock.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetMainWarehouseStock.Appearance.Options.UseFont = true;
        btnSetMainWarehouseStock.Location = new Point(266, 432);
        btnSetMainWarehouseStock.Name = "btnSetMainWarehouseStock";
        btnSetMainWarehouseStock.Size = new Size(130, 26);
        btnSetMainWarehouseStock.TabIndex = 43;
        btnSetMainWarehouseStock.Text = "Marcar principal";
        // 
        // btnRemoveWarehouseStock
        // 
        btnRemoveWarehouseStock.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRemoveWarehouseStock.Appearance.Options.UseFont = true;
        btnRemoveWarehouseStock.Location = new Point(188, 432);
        btnRemoveWarehouseStock.Name = "btnRemoveWarehouseStock";
        btnRemoveWarehouseStock.Size = new Size(72, 26);
        btnRemoveWarehouseStock.TabIndex = 42;
        btnRemoveWarehouseStock.Text = "Quitar";
        // 
        // btnUpdateWarehouseStock
        // 
        btnUpdateWarehouseStock.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnUpdateWarehouseStock.Appearance.Options.UseFont = true;
        btnUpdateWarehouseStock.Location = new Point(96, 432);
        btnUpdateWarehouseStock.Name = "btnUpdateWarehouseStock";
        btnUpdateWarehouseStock.Size = new Size(86, 26);
        btnUpdateWarehouseStock.TabIndex = 41;
        btnUpdateWarehouseStock.Text = "Actualizar";
        // 
        // btnAddWarehouseStock
        // 
        btnAddWarehouseStock.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddWarehouseStock.Appearance.Options.UseFont = true;
        btnAddWarehouseStock.Location = new Point(12, 432);
        btnAddWarehouseStock.Name = "btnAddWarehouseStock";
        btnAddWarehouseStock.Size = new Size(78, 26);
        btnAddWarehouseStock.TabIndex = 40;
        btnAddWarehouseStock.Text = "Agregar";
        // 
        // tabUnits
        // 
        tabUnits.Controls.Add(lblCodesIdentifiersTitle);
        tabUnits.Controls.Add(lblQrCode);
        tabUnits.Controls.Add(lblPurchasePresentationsTitle);
        tabUnits.Controls.Add(txtQrCode);
        tabUnits.Controls.Add(grdPurchasePresentations);
        tabUnits.Controls.Add(lblPlu);
        tabUnits.Controls.Add(lblInventoryUnitTitle);
        tabUnits.Controls.Add(txtPlu);
        tabUnits.Controls.Add(btnAddPurchasePresentation);
        tabUnits.Controls.Add(lblPreviousInternalCode);
        tabUnits.Controls.Add(lblInventoryUnit);
        tabUnits.Controls.Add(txtPreviousInternalCode);
        tabUnits.Controls.Add(btnUpdatePurchasePresentation);
        tabUnits.Controls.Add(lblManufacturerReference);
        tabUnits.Controls.Add(lueInventoryUnit);
        tabUnits.Controls.Add(txtManufacturerReference);
        tabUnits.Controls.Add(btnRemovePurchasePresentation);
        tabUnits.Controls.Add(lblUnspscCode);
        tabUnits.Controls.Add(txtUnspscCode);
        tabUnits.Controls.Add(btnSetMainPurchasePresentation);
        tabUnits.Controls.Add(lblTariffCode);
        tabUnits.Controls.Add(txtTariffCode);
        tabUnits.Controls.Add(lblNetWeight);
        tabUnits.Controls.Add(lblCodeOrigin);
        tabUnits.Controls.Add(spnNetWeight);
        tabUnits.Controls.Add(lueCodeOrigin);
        tabUnits.Controls.Add(lblNetWeightUnit);
        tabUnits.Controls.Add(lblGrossWeight);
        tabUnits.Controls.Add(spnGrossWeight);
        tabUnits.Controls.Add(lblGrossWeightUnit);
        tabUnits.Controls.Add(lblVolume);
        tabUnits.Controls.Add(spnVolume);
        tabUnits.Controls.Add(lblVolumeUnitCaption);
        tabUnits.Controls.Add(lblWeightUnit);
        tabUnits.Controls.Add(lueWeightUnit);
        tabUnits.Controls.Add(lblVolumeUnit);
        tabUnits.Controls.Add(lueVolumeUnit);
        tabUnits.Name = "tabUnits";
        tabUnits.Size = new Size(1418, 537);
        tabUnits.Text = "Unidades y códigos";
        // 
        // lueVolumeUnit
        // 
        lueVolumeUnit.Location = new Point(127, 102);
        lueVolumeUnit.Name = "lueVolumeUnit";
        lueVolumeUnit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueVolumeUnit.Properties.Appearance.Options.UseFont = true;
        lueVolumeUnit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueVolumeUnit.Properties.NullText = "";
        lueVolumeUnit.Size = new Size(308, 22);
        lueVolumeUnit.TabIndex = 39;
        // 
        // lblVolumeUnit
        // 
        lblVolumeUnit.Location = new Point(18, 106);
        lblVolumeUnit.Name = "lblVolumeUnit";
        lblVolumeUnit.Size = new Size(95, 13);
        lblVolumeUnit.TabIndex = 38;
        lblVolumeUnit.Text = "Unidad de volumen:";
        // 
        // lueWeightUnit
        // 
        lueWeightUnit.Location = new Point(127, 74);
        lueWeightUnit.Name = "lueWeightUnit";
        lueWeightUnit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueWeightUnit.Properties.Appearance.Options.UseFont = true;
        lueWeightUnit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueWeightUnit.Properties.NullText = "";
        lueWeightUnit.Size = new Size(308, 22);
        lueWeightUnit.TabIndex = 37;
        // 
        // lblWeightUnit
        // 
        lblWeightUnit.Location = new Point(18, 78);
        lblWeightUnit.Name = "lblWeightUnit";
        lblWeightUnit.Size = new Size(78, 13);
        lblWeightUnit.TabIndex = 36;
        lblWeightUnit.Text = "Unidad de peso:";
        // 
        // lblVolumeUnitCaption
        // 
        lblVolumeUnitCaption.Location = new Point(231, 162);
        lblVolumeUnitCaption.Name = "lblVolumeUnitCaption";
        lblVolumeUnitCaption.Size = new Size(13, 13);
        lblVolumeUnitCaption.TabIndex = 35;
        lblVolumeUnitCaption.Text = "m³";
        // 
        // spnVolume
        // 
        spnVolume.EditValue = new decimal(new int[] { 20, 0, 0, 262144 });
        spnVolume.Location = new Point(127, 158);
        spnVolume.Name = "spnVolume";
        spnVolume.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnVolume.Properties.Appearance.Options.UseFont = true;
        spnVolume.Properties.Appearance.Options.UseTextOptions = true;
        spnVolume.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnVolume.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnVolume.Size = new Size(95, 22);
        spnVolume.TabIndex = 34;
        // 
        // lblVolume
        // 
        lblVolume.Location = new Point(17, 162);
        lblVolume.Name = "lblVolume";
        lblVolume.Size = new Size(44, 13);
        lblVolume.TabIndex = 33;
        lblVolume.Text = "Volumen:";
        // 
        // lblGrossWeightUnit
        // 
        lblGrossWeightUnit.Location = new Point(424, 134);
        lblGrossWeightUnit.Name = "lblGrossWeightUnit";
        lblGrossWeightUnit.Size = new Size(11, 13);
        lblGrossWeightUnit.TabIndex = 32;
        lblGrossWeightUnit.Text = "kg";
        // 
        // spnGrossWeight
        // 
        spnGrossWeight.EditValue = new decimal(new int[] { 1020, 0, 0, 196608 });
        spnGrossWeight.Location = new Point(320, 130);
        spnGrossWeight.Name = "spnGrossWeight";
        spnGrossWeight.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnGrossWeight.Properties.Appearance.Options.UseFont = true;
        spnGrossWeight.Properties.Appearance.Options.UseTextOptions = true;
        spnGrossWeight.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnGrossWeight.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnGrossWeight.Size = new Size(95, 22);
        spnGrossWeight.TabIndex = 31;
        // 
        // lblGrossWeight
        // 
        lblGrossWeight.Location = new Point(258, 134);
        lblGrossWeight.Name = "lblGrossWeight";
        lblGrossWeight.Size = new Size(56, 13);
        lblGrossWeight.TabIndex = 30;
        lblGrossWeight.Text = "Peso bruto:";
        // 
        // lblNetWeightUnit
        // 
        lblNetWeightUnit.Location = new Point(231, 134);
        lblNetWeightUnit.Name = "lblNetWeightUnit";
        lblNetWeightUnit.Size = new Size(11, 13);
        lblNetWeightUnit.TabIndex = 29;
        lblNetWeightUnit.Text = "kg";
        // 
        // lueCodeOrigin
        // 
        lueCodeOrigin.Location = new Point(320, 371);
        lueCodeOrigin.Name = "lueCodeOrigin";
        lueCodeOrigin.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCodeOrigin.Properties.Appearance.Options.UseFont = true;
        lueCodeOrigin.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueCodeOrigin.Properties.NullText = "";
        lueCodeOrigin.Size = new Size(115, 22);
        lueCodeOrigin.TabIndex = 14;
        // 
        // spnNetWeight
        // 
        spnNetWeight.EditValue = new decimal(new int[] { 1000, 0, 0, 196608 });
        spnNetWeight.Location = new Point(127, 130);
        spnNetWeight.Name = "spnNetWeight";
        spnNetWeight.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnNetWeight.Properties.Appearance.Options.UseFont = true;
        spnNetWeight.Properties.Appearance.Options.UseTextOptions = true;
        spnNetWeight.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnNetWeight.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnNetWeight.Size = new Size(95, 22);
        spnNetWeight.TabIndex = 28;
        // 
        // lblCodeOrigin
        // 
        lblCodeOrigin.Location = new Point(258, 375);
        lblCodeOrigin.Name = "lblCodeOrigin";
        lblCodeOrigin.Size = new Size(36, 13);
        lblCodeOrigin.TabIndex = 13;
        lblCodeOrigin.Text = "Origen:";
        // 
        // lblNetWeight
        // 
        lblNetWeight.Location = new Point(18, 134);
        lblNetWeight.Name = "lblNetWeight";
        lblNetWeight.Size = new Size(52, 13);
        lblNetWeight.TabIndex = 27;
        lblNetWeight.Text = "Peso neto:";
        // 
        // txtTariffCode
        // 
        txtTariffCode.EditValue = "1006.30";
        txtTariffCode.Location = new Point(127, 371);
        txtTariffCode.Name = "txtTariffCode";
        txtTariffCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtTariffCode.Properties.Appearance.Options.UseFont = true;
        txtTariffCode.Size = new Size(96, 22);
        txtTariffCode.TabIndex = 12;
        // 
        // lblTariffCode
        // 
        lblTariffCode.Location = new Point(17, 378);
        lblTariffCode.Name = "lblTariffCode";
        lblTariffCode.Size = new Size(58, 13);
        lblTariffCode.TabIndex = 11;
        lblTariffCode.Text = "Arancelario:";
        // 
        // btnSetMainPurchasePresentation
        // 
        btnSetMainPurchasePresentation.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetMainPurchasePresentation.Appearance.Options.UseFont = true;
        btnSetMainPurchasePresentation.Location = new Point(718, 367);
        btnSetMainPurchasePresentation.Name = "btnSetMainPurchasePresentation";
        btnSetMainPurchasePresentation.Size = new Size(130, 26);
        btnSetMainPurchasePresentation.TabIndex = 5;
        btnSetMainPurchasePresentation.Text = "Marcar principal";
        // 
        // txtUnspscCode
        // 
        txtUnspscCode.EditValue = "50221101";
        txtUnspscCode.Location = new Point(127, 343);
        txtUnspscCode.Name = "txtUnspscCode";
        txtUnspscCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtUnspscCode.Properties.Appearance.Options.UseFont = true;
        txtUnspscCode.Size = new Size(308, 22);
        txtUnspscCode.TabIndex = 10;
        // 
        // lblUnspscCode
        // 
        lblUnspscCode.Location = new Point(17, 347);
        lblUnspscCode.Name = "lblUnspscCode";
        lblUnspscCode.Size = new Size(66, 13);
        lblUnspscCode.TabIndex = 9;
        lblUnspscCode.Text = "SAT/UNSPSC:";
        // 
        // btnRemovePurchasePresentation
        // 
        btnRemovePurchasePresentation.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRemovePurchasePresentation.Appearance.Options.UseFont = true;
        btnRemovePurchasePresentation.Location = new Point(640, 367);
        btnRemovePurchasePresentation.Name = "btnRemovePurchasePresentation";
        btnRemovePurchasePresentation.Size = new Size(72, 26);
        btnRemovePurchasePresentation.TabIndex = 4;
        btnRemovePurchasePresentation.Text = "Quitar";
        // 
        // txtManufacturerReference
        // 
        txtManufacturerReference.EditValue = "NF-ARZ-1KG";
        txtManufacturerReference.Location = new Point(127, 315);
        txtManufacturerReference.Name = "txtManufacturerReference";
        txtManufacturerReference.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtManufacturerReference.Properties.Appearance.Options.UseFont = true;
        txtManufacturerReference.Size = new Size(308, 22);
        txtManufacturerReference.TabIndex = 8;
        // 
        // lueInventoryUnit
        // 
        lueInventoryUnit.Location = new Point(127, 46);
        lueInventoryUnit.Name = "lueInventoryUnit";
        lueInventoryUnit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueInventoryUnit.Properties.Appearance.Options.UseFont = true;
        lueInventoryUnit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueInventoryUnit.Properties.NullText = "";
        lueInventoryUnit.Size = new Size(308, 22);
        lueInventoryUnit.TabIndex = 22;
        // 
        // lblManufacturerReference
        // 
        lblManufacturerReference.Location = new Point(17, 319);
        lblManufacturerReference.Name = "lblManufacturerReference";
        lblManufacturerReference.Size = new Size(77, 13);
        lblManufacturerReference.TabIndex = 7;
        lblManufacturerReference.Text = "Ref. fabricante:";
        // 
        // btnUpdatePurchasePresentation
        // 
        btnUpdatePurchasePresentation.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnUpdatePurchasePresentation.Appearance.Options.UseFont = true;
        btnUpdatePurchasePresentation.Location = new Point(548, 367);
        btnUpdatePurchasePresentation.Name = "btnUpdatePurchasePresentation";
        btnUpdatePurchasePresentation.Size = new Size(86, 26);
        btnUpdatePurchasePresentation.TabIndex = 3;
        btnUpdatePurchasePresentation.Text = "Actualizar";
        // 
        // txtPreviousInternalCode
        // 
        txtPreviousInternalCode.EditValue = "ARZ-BL-001";
        txtPreviousInternalCode.Location = new Point(127, 287);
        txtPreviousInternalCode.Name = "txtPreviousInternalCode";
        txtPreviousInternalCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPreviousInternalCode.Properties.Appearance.Options.UseFont = true;
        txtPreviousInternalCode.Size = new Size(308, 22);
        txtPreviousInternalCode.TabIndex = 6;
        // 
        // lblInventoryUnit
        // 
        lblInventoryUnit.Location = new Point(18, 50);
        lblInventoryUnit.Name = "lblInventoryUnit";
        lblInventoryUnit.Size = new Size(103, 13);
        lblInventoryUnit.TabIndex = 21;
        lblInventoryUnit.Text = "Unidad de inventario:";
        // 
        // lblPreviousInternalCode
        // 
        lblPreviousInternalCode.Location = new Point(17, 291);
        lblPreviousInternalCode.Name = "lblPreviousInternalCode";
        lblPreviousInternalCode.Size = new Size(78, 13);
        lblPreviousInternalCode.TabIndex = 5;
        lblPreviousInternalCode.Text = "Código anterior:";
        // 
        // btnAddPurchasePresentation
        // 
        btnAddPurchasePresentation.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddPurchasePresentation.Appearance.Options.UseFont = true;
        btnAddPurchasePresentation.Location = new Point(464, 367);
        btnAddPurchasePresentation.Name = "btnAddPurchasePresentation";
        btnAddPurchasePresentation.Size = new Size(78, 26);
        btnAddPurchasePresentation.TabIndex = 2;
        btnAddPurchasePresentation.Text = "Agregar";
        // 
        // txtPlu
        // 
        txtPlu.EditValue = "1001";
        txtPlu.Location = new Point(127, 259);
        txtPlu.Name = "txtPlu";
        txtPlu.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPlu.Properties.Appearance.Options.UseFont = true;
        txtPlu.Size = new Size(308, 22);
        txtPlu.TabIndex = 4;
        // 
        // lblInventoryUnitTitle
        // 
        lblInventoryUnitTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblInventoryUnitTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblInventoryUnitTitle.Appearance.Options.UseFont = true;
        lblInventoryUnitTitle.Appearance.Options.UseForeColor = true;
        lblInventoryUnitTitle.Location = new Point(12, 12);
        lblInventoryUnitTitle.Name = "lblInventoryUnitTitle";
        lblInventoryUnitTitle.Size = new Size(159, 20);
        lblInventoryUnitTitle.TabIndex = 20;
        lblInventoryUnitTitle.Text = "1. Unidad de inventario";
        // 
        // lblPlu
        // 
        lblPlu.Location = new Point(17, 263);
        lblPlu.Name = "lblPlu";
        lblPlu.Size = new Size(22, 13);
        lblPlu.TabIndex = 3;
        lblPlu.Text = "PLU:";
        // 
        // grdPurchasePresentations
        // 
        grdPurchasePresentations.DataSource = purchasePresentationsTable;
        grdPurchasePresentations.Location = new Point(464, 46);
        grdPurchasePresentations.MainView = gvPurchasePresentations;
        grdPurchasePresentations.Name = "grdPurchasePresentations";
        grdPurchasePresentations.RepositoryItems.AddRange(new RepositoryItem[] { repoPurchasePrincipal, repoPurchaseActive });
        grdPurchasePresentations.Size = new Size(922, 312);
        grdPurchasePresentations.TabIndex = 1;
        grdPurchasePresentations.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvPurchasePresentations });
        // 
        // gridView12
        // 
        gridView12.GridControl = grdPurchasePresentations;
        gridView12.Name = "gridView12";
        // 
        // repoPurchasePrincipal
        // 
        repoPurchasePrincipal.AutoHeight = false;
        repoPurchasePrincipal.Name = "repoPurchasePrincipal";
        // 
        // repoPurchaseActive
        // 
        repoPurchaseActive.AutoHeight = false;
        repoPurchaseActive.Name = "repoPurchaseActive";
        // 
        // gvPurchasePresentations
        // 
        gvPurchasePresentations.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvPurchasePresentations.Appearance.HeaderPanel.Options.UseFont = true;
        gvPurchasePresentations.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvPurchasePresentations.Appearance.Row.Options.UseFont = true;
        gvPurchasePresentations.Columns.AddRange(new GridColumn[] { colPurchasePresentation, colPurchaseUnit, colPurchaseFactor, colPurchaseBarcode, colPurchaseEnabled, colSalesEnabled, colPurchasePrincipal, colSalesPrincipal, colPurchaseActive });
        gvPurchasePresentations.GridControl = grdPurchasePresentations;
        gvPurchasePresentations.Name = "gvPurchasePresentations";
        gvPurchasePresentations.OptionsView.ShowGroupPanel = false;
        // 
        // colPurchasePresentation
        // 
        colPurchasePresentation.Caption = "Presentación";
        colPurchasePresentation.FieldName = "Presentacion";
        colPurchasePresentation.Name = "colPurchasePresentation";
        colPurchasePresentation.Visible = true;
        colPurchasePresentation.VisibleIndex = 0;
        colPurchasePresentation.Width = 190;
        // 
        // colPurchaseUnit
        // 
        colPurchaseUnit.Caption = "Unidad";
        colPurchaseUnit.FieldName = "Unidad";
        colPurchaseUnit.Name = "colPurchaseUnit";
        colPurchaseUnit.Visible = true;
        colPurchaseUnit.VisibleIndex = 1;
        colPurchaseUnit.Width = 60;
        // 
        // colPurchaseFactor
        // 
        colPurchaseFactor.Caption = "Factor";
        colPurchaseFactor.FieldName = "FactorInventario";
        colPurchaseFactor.Name = "colPurchaseFactor";
        colPurchaseFactor.Visible = true;
        colPurchaseFactor.VisibleIndex = 2;
        colPurchaseFactor.Width = 65;
        // 
        // colPurchaseBarcode
        // 
        colPurchaseBarcode.Caption = "Código de barras";
        colPurchaseBarcode.FieldName = "CodigoBarras";
        colPurchaseBarcode.Name = "colPurchaseBarcode";
        colPurchaseBarcode.Visible = true;
        colPurchaseBarcode.VisibleIndex = 3;
        colPurchaseBarcode.Width = 170;
        // 
        // colPurchaseEnabled
        // 
        colPurchaseEnabled.Caption = "Compra";
        colPurchaseEnabled.ColumnEdit = repoPurchaseActive;
        colPurchaseEnabled.FieldName = "AplicaCompra";
        colPurchaseEnabled.Name = "colPurchaseEnabled";
        colPurchaseEnabled.Visible = true;
        colPurchaseEnabled.VisibleIndex = 4;
        colPurchaseEnabled.Width = 62;
        // 
        // colSalesEnabled
        // 
        colSalesEnabled.Caption = "Venta";
        colSalesEnabled.ColumnEdit = repoPurchaseActive;
        colSalesEnabled.FieldName = "AplicaVenta";
        colSalesEnabled.Name = "colSalesEnabled";
        colSalesEnabled.Visible = true;
        colSalesEnabled.VisibleIndex = 5;
        colSalesEnabled.Width = 58;
        // 
        // colPurchasePrincipal
        // 
        colPurchasePrincipal.Caption = "Principal";
        colPurchasePrincipal.ColumnEdit = repoPurchasePrincipal;
        colPurchasePrincipal.FieldName = "Principal";
        colPurchasePrincipal.Name = "colPurchasePrincipal";
        colPurchasePrincipal.Visible = true;
        colPurchasePrincipal.VisibleIndex = 7;
        colPurchasePrincipal.Width = 74;
        // 
        // colSalesPrincipal
        // 
        colSalesPrincipal.Caption = "Inventario";
        colSalesPrincipal.ColumnEdit = repoPurchaseActive;
        colSalesPrincipal.FieldName = "AplicaInventario";
        colSalesPrincipal.Name = "colSalesPrincipal";
        colSalesPrincipal.Visible = true;
        colSalesPrincipal.VisibleIndex = 6;
        colSalesPrincipal.Width = 74;
        // 
        // colPurchaseActive
        // 
        colPurchaseActive.Caption = "Activa";
        colPurchaseActive.ColumnEdit = repoPurchaseActive;
        colPurchaseActive.FieldName = "Activa";
        colPurchaseActive.Name = "colPurchaseActive";
        colPurchaseActive.Visible = true;
        colPurchaseActive.VisibleIndex = 8;
        colPurchaseActive.Width = 58;
        // 
        // txtQrCode
        // 
        txtQrCode.EditValue = "QR7501234567890";
        txtQrCode.Location = new Point(127, 231);
        txtQrCode.Name = "txtQrCode";
        txtQrCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtQrCode.Properties.Appearance.Options.UseFont = true;
        txtQrCode.Size = new Size(308, 22);
        txtQrCode.TabIndex = 2;
        // 
        // lblPurchasePresentationsTitle
        // 
        lblPurchasePresentationsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchasePresentationsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblPurchasePresentationsTitle.Appearance.Options.UseFont = true;
        lblPurchasePresentationsTitle.Appearance.Options.UseForeColor = true;
        lblPurchasePresentationsTitle.Location = new Point(464, 12);
        lblPurchasePresentationsTitle.Name = "lblPurchasePresentationsTitle";
        lblPurchasePresentationsTitle.Size = new Size(257, 20);
        lblPurchasePresentationsTitle.TabIndex = 0;
        lblPurchasePresentationsTitle.Text = "3. Presentaciones, unidades y códigos";
        // 
        // lblQrCode
        // 
        lblQrCode.Location = new Point(17, 235);
        lblQrCode.Name = "lblQrCode";
        lblQrCode.Size = new Size(55, 13);
        lblQrCode.TabIndex = 1;
        lblQrCode.Text = "Código QR:";
        // 
        // lblCodesIdentifiersTitle
        // 
        lblCodesIdentifiersTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblCodesIdentifiersTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblCodesIdentifiersTitle.Appearance.Options.UseFont = true;
        lblCodesIdentifiersTitle.Appearance.Options.UseForeColor = true;
        lblCodesIdentifiersTitle.Location = new Point(12, 197);
        lblCodesIdentifiersTitle.Name = "lblCodesIdentifiersTitle";
        lblCodesIdentifiersTitle.Size = new Size(187, 20);
        lblCodesIdentifiersTitle.TabIndex = 0;
        lblCodesIdentifiersTitle.Text = "2. Identificadores generales";
        // 
        // tabGeneral
        // 
        tabGeneral.Controls.Add(lblBlockedEcommerce);
        tabGeneral.Controls.Add(tglBlockedEcommerce);
        tabGeneral.Controls.Add(lblGeneralSummaryTitle);
        tabGeneral.Controls.Add(pnlKpiStock);
        tabGeneral.Controls.Add(pnlKpiOrders);
        tabGeneral.Controls.Add(pnlKpiPurchases);
        tabGeneral.Controls.Add(pnlKpiSales);
        tabGeneral.Controls.Add(pnlKpiSap);
        tabGeneral.Controls.Add(pnlKpiVariants);
        tabGeneral.Controls.Add(tglGeneralMobileItem);
        tabGeneral.Controls.Add(lblGeneralMobileItem);
        tabGeneral.Controls.Add(tglGeneralRequiresScale);
        tabGeneral.Controls.Add(tglGeneralAllowDiscount);
        tabGeneral.Controls.Add(tglGeneralPerishable);
        tabGeneral.Controls.Add(tglGeneralSerialManaged);
        tabGeneral.Controls.Add(tglGeneralBatchManaged);
        tabGeneral.Controls.Add(lblGeneralOperationTitle);
        tabGeneral.Controls.Add(lblBatchManaged);
        tabGeneral.Controls.Add(lblSerialManaged);
        tabGeneral.Controls.Add(lblPerishable);
        tabGeneral.Controls.Add(lblExpirationManaged);
        tabGeneral.Controls.Add(lblRequiresScale);
        tabGeneral.Controls.Add(lblAllowDiscount);
        tabGeneral.Controls.Add(tglGeneralExpirationManaged);
        tabGeneral.Controls.Add(lblAffectsInventory);
        tabGeneral.Controls.Add(tglAffectsInventory);
        tabGeneral.Controls.Add(lblGeneralIdentificationTitle);
        tabGeneral.Controls.Add(lblAlternateCode);
        tabGeneral.Controls.Add(txtAlternateCode);
        tabGeneral.Controls.Add(lblSupplierSku);
        tabGeneral.Controls.Add(slueSupplierSku);
        tabGeneral.Controls.Add(lblLongDescription);
        tabGeneral.Controls.Add(memLongDescription);
        tabGeneral.Controls.Add(lblProductType);
        tabGeneral.Controls.Add(lueProductType);
        tabGeneral.Controls.Add(lblOrigin);
        tabGeneral.Controls.Add(lueOrigin);
        tabGeneral.Controls.Add(lblLine);
        tabGeneral.Controls.Add(lueLine);
        tabGeneral.Controls.Add(lblSubGroup);
        tabGeneral.Controls.Add(lueSubGroup);
        tabGeneral.Controls.Add(lblModel);
        tabGeneral.Controls.Add(txtModel);
        tabGeneral.Controls.Add(lblReference);
        tabGeneral.Controls.Add(txtReference);
        tabGeneral.Controls.Add(lblSalesActive);
        tabGeneral.Controls.Add(tglSalesActive);
        tabGeneral.Controls.Add(lblPurchaseActive);
        tabGeneral.Controls.Add(tglPurchaseActive);
        tabGeneral.Name = "tabGeneral";
        tabGeneral.Size = new Size(1418, 537);
        tabGeneral.Text = "General";
        // 
        // tglPurchaseActive
        // 
        tglPurchaseActive.EditValue = true;
        tglPurchaseActive.Location = new Point(638, 78);
        tglPurchaseActive.Name = "tglPurchaseActive";
        tglPurchaseActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglPurchaseActive.Properties.Appearance.Options.UseFont = true;
        tglPurchaseActive.Properties.OffText = "No";
        tglPurchaseActive.Properties.OnText = "Sí";
        tglPurchaseActive.Size = new Size(86, 20);
        tglPurchaseActive.TabIndex = 45;
        // 
        // lblPurchaseActive
        // 
        lblPurchaseActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseActive.Appearance.Options.UseFont = true;
        lblPurchaseActive.Location = new Point(517, 80);
        lblPurchaseActive.Name = "lblPurchaseActive";
        lblPurchaseActive.Size = new Size(87, 15);
        lblPurchaseActive.TabIndex = 44;
        lblPurchaseActive.Text = "Item de compra:";
        // 
        // tglSalesActive
        // 
        tglSalesActive.EditValue = true;
        tglSalesActive.Location = new Point(638, 50);
        tglSalesActive.Name = "tglSalesActive";
        tglSalesActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglSalesActive.Properties.Appearance.Options.UseFont = true;
        tglSalesActive.Properties.OffText = "No";
        tglSalesActive.Properties.OnText = "Sí";
        tglSalesActive.Size = new Size(86, 20);
        tglSalesActive.TabIndex = 43;
        // 
        // lblSalesActive
        // 
        lblSalesActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesActive.Appearance.Options.UseFont = true;
        lblSalesActive.Location = new Point(517, 52);
        lblSalesActive.Name = "lblSalesActive";
        lblSalesActive.Size = new Size(75, 15);
        lblSalesActive.TabIndex = 42;
        lblSalesActive.Text = "Item de venta:";
        // 
        // txtReference
        // 
        txtReference.EditValue = "N/A";
        txtReference.Location = new Point(164, 329);
        txtReference.Name = "txtReference";
        txtReference.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtReference.Properties.Appearance.Options.UseFont = true;
        txtReference.Size = new Size(270, 22);
        txtReference.TabIndex = 41;
        // 
        // lblReference
        // 
        lblReference.Appearance.Font = new Font("Segoe UI", 9F);
        lblReference.Appearance.Options.UseFont = true;
        lblReference.Location = new Point(17, 332);
        lblReference.Name = "lblReference";
        lblReference.Size = new Size(58, 15);
        lblReference.TabIndex = 40;
        lblReference.Text = "Referencia:";
        // 
        // txtModel
        // 
        txtModel.EditValue = "N/A";
        txtModel.Location = new Point(164, 301);
        txtModel.Name = "txtModel";
        txtModel.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtModel.Properties.Appearance.Options.UseFont = true;
        txtModel.Size = new Size(270, 22);
        txtModel.TabIndex = 39;
        // 
        // lblModel
        // 
        lblModel.Appearance.Font = new Font("Segoe UI", 9F);
        lblModel.Appearance.Options.UseFont = true;
        lblModel.Location = new Point(17, 304);
        lblModel.Name = "lblModel";
        lblModel.Size = new Size(44, 15);
        lblModel.TabIndex = 38;
        lblModel.Text = "Modelo:";
        // 
        // lueSubGroup
        // 
        lueSubGroup.Location = new Point(164, 273);
        lueSubGroup.Name = "lueSubGroup";
        lueSubGroup.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSubGroup.Properties.Appearance.Options.UseFont = true;
        lueSubGroup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueSubGroup.Properties.NullText = "";
        lueSubGroup.Size = new Size(270, 22);
        lueSubGroup.TabIndex = 37;
        // 
        // lblSubGroup
        // 
        lblSubGroup.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubGroup.Appearance.Options.UseFont = true;
        lblSubGroup.Location = new Point(17, 276);
        lblSubGroup.Name = "lblSubGroup";
        lblSubGroup.Size = new Size(55, 15);
        lblSubGroup.TabIndex = 36;
        lblSubGroup.Text = "Subgrupo:";
        // 
        // lueLine
        // 
        lueLine.Location = new Point(164, 245);
        lueLine.Name = "lueLine";
        lueLine.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueLine.Properties.Appearance.Options.UseFont = true;
        lueLine.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueLine.Properties.NullText = "";
        lueLine.Size = new Size(270, 22);
        lueLine.TabIndex = 35;
        // 
        // lblLine
        // 
        lblLine.Appearance.Font = new Font("Segoe UI", 9F);
        lblLine.Appearance.Options.UseFont = true;
        lblLine.Location = new Point(17, 248);
        lblLine.Name = "lblLine";
        lblLine.Size = new Size(31, 15);
        lblLine.TabIndex = 34;
        lblLine.Text = "Línea:";
        // 
        // lueOrigin
        // 
        lueOrigin.Location = new Point(164, 217);
        lueOrigin.Name = "lueOrigin";
        lueOrigin.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueOrigin.Properties.Appearance.Options.UseFont = true;
        lueOrigin.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueOrigin.Properties.NullText = "";
        lueOrigin.Size = new Size(270, 22);
        lueOrigin.TabIndex = 33;
        // 
        // lblOrigin
        // 
        lblOrigin.Appearance.Font = new Font("Segoe UI", 9F);
        lblOrigin.Appearance.Options.UseFont = true;
        lblOrigin.Location = new Point(17, 220);
        lblOrigin.Name = "lblOrigin";
        lblOrigin.Size = new Size(39, 15);
        lblOrigin.TabIndex = 32;
        lblOrigin.Text = "Origen:";
        // 
        // lueProductType
        // 
        lueProductType.Location = new Point(164, 189);
        lueProductType.Name = "lueProductType";
        lueProductType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueProductType.Properties.Appearance.Options.UseFont = true;
        lueProductType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueProductType.Properties.NullText = "";
        lueProductType.Size = new Size(270, 22);
        lueProductType.TabIndex = 31;
        // 
        // lblProductType
        // 
        lblProductType.Appearance.Font = new Font("Segoe UI", 9F);
        lblProductType.Appearance.Options.UseFont = true;
        lblProductType.Location = new Point(17, 192);
        lblProductType.Name = "lblProductType";
        lblProductType.Size = new Size(95, 15);
        lblProductType.TabIndex = 30;
        lblProductType.Text = "Tipo de producto:";
        // 
        // memLongDescription
        // 
        memLongDescription.EditValue = "Arroz blanco de grano largo, seleccionado especialmente por su calidad y consistencia.\r\nIdeal para consumo diario.\r\nPresentación de 1 kilogramo.";
        memLongDescription.Location = new Point(164, 105);
        memLongDescription.Name = "memLongDescription";
        memLongDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memLongDescription.Properties.Appearance.Options.UseFont = true;
        memLongDescription.Size = new Size(270, 78);
        memLongDescription.TabIndex = 29;
        // 
        // lblLongDescription
        // 
        lblLongDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblLongDescription.Appearance.Options.UseFont = true;
        lblLongDescription.Location = new Point(17, 108);
        lblLongDescription.Name = "lblLongDescription";
        lblLongDescription.Size = new Size(94, 15);
        lblLongDescription.TabIndex = 28;
        lblLongDescription.Text = "Descripción larga:";
        // 
        // slueSupplierSku
        // 
        slueSupplierSku.EditValue = "PRV-ARZ-001";
        slueSupplierSku.Location = new Point(164, 77);
        slueSupplierSku.Name = "slueSupplierSku";
        slueSupplierSku.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueSupplierSku.Properties.Appearance.Options.UseFont = true;
        slueSupplierSku.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        slueSupplierSku.Properties.NullText = "";
        slueSupplierSku.Properties.PopupView = gvSupplierSku;
        slueSupplierSku.Size = new Size(270, 22);
        slueSupplierSku.TabIndex = 27;
        // 
        // gvSupplierSku
        // 
        gvSupplierSku.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvSupplierSku.Appearance.HeaderPanel.Options.UseFont = true;
        gvSupplierSku.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvSupplierSku.Appearance.Row.Options.UseFont = true;
        gvSupplierSku.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvSupplierSku.Name = "gvSupplierSku";
        gvSupplierSku.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvSupplierSku.OptionsView.ShowGroupPanel = false;
        // 
        // lblSupplierSku
        // 
        lblSupplierSku.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierSku.Appearance.Options.UseFont = true;
        lblSupplierSku.Location = new Point(17, 80);
        lblSupplierSku.Name = "lblSupplierSku";
        lblSupplierSku.Size = new Size(81, 15);
        lblSupplierSku.TabIndex = 26;
        lblSupplierSku.Text = "SKU proveedor:";
        // 
        // txtAlternateCode
        // 
        txtAlternateCode.EditValue = "ARZ001-PREM";
        txtAlternateCode.Location = new Point(164, 49);
        txtAlternateCode.Name = "txtAlternateCode";
        txtAlternateCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAlternateCode.Properties.Appearance.Options.UseFont = true;
        txtAlternateCode.Size = new Size(270, 22);
        txtAlternateCode.TabIndex = 25;
        // 
        // lblAlternateCode
        // 
        lblAlternateCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblAlternateCode.Appearance.Options.UseFont = true;
        lblAlternateCode.Location = new Point(17, 52);
        lblAlternateCode.Name = "lblAlternateCode";
        lblAlternateCode.Size = new Size(82, 15);
        lblAlternateCode.TabIndex = 24;
        lblAlternateCode.Text = "Código alterno:";
        // 
        // lblGeneralIdentificationTitle
        // 
        lblGeneralIdentificationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneralIdentificationTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblGeneralIdentificationTitle.Appearance.Options.UseFont = true;
        lblGeneralIdentificationTitle.Appearance.Options.UseForeColor = true;
        lblGeneralIdentificationTitle.Location = new Point(12, 12);
        lblGeneralIdentificationTitle.Name = "lblGeneralIdentificationTitle";
        lblGeneralIdentificationTitle.Size = new Size(187, 20);
        lblGeneralIdentificationTitle.TabIndex = 23;
        lblGeneralIdentificationTitle.Text = "1. Identificación del artículo";
        // 
        // tglAffectsInventory
        // 
        tglAffectsInventory.EditValue = true;
        tglAffectsInventory.Location = new Point(638, 106);
        tglAffectsInventory.Name = "tglAffectsInventory";
        tglAffectsInventory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAffectsInventory.Properties.Appearance.Options.UseFont = true;
        tglAffectsInventory.Properties.OffText = "No";
        tglAffectsInventory.Properties.OnText = "Sí";
        tglAffectsInventory.Size = new Size(86, 20);
        tglAffectsInventory.TabIndex = 66;
        // 
        // lblAffectsInventory
        // 
        lblAffectsInventory.Appearance.Font = new Font("Segoe UI", 9F);
        lblAffectsInventory.Appearance.Options.UseFont = true;
        lblAffectsInventory.Location = new Point(517, 108);
        lblAffectsInventory.Name = "lblAffectsInventory";
        lblAffectsInventory.Size = new Size(99, 15);
        lblAffectsInventory.TabIndex = 65;
        lblAffectsInventory.Text = "Item de inventario:";
        // 
        // tglGeneralExpirationManaged
        // 
        tglGeneralExpirationManaged.EditValue = true;
        tglGeneralExpirationManaged.Location = new Point(638, 218);
        tglGeneralExpirationManaged.Name = "tglGeneralExpirationManaged";
        tglGeneralExpirationManaged.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralExpirationManaged.Properties.Appearance.Options.UseFont = true;
        tglGeneralExpirationManaged.Properties.OffText = "No";
        tglGeneralExpirationManaged.Properties.OnText = "Sí";
        tglGeneralExpirationManaged.Size = new Size(86, 20);
        tglGeneralExpirationManaged.TabIndex = 64;
        // 
        // lblAllowDiscount
        // 
        lblAllowDiscount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowDiscount.Appearance.Options.UseFont = true;
        lblAllowDiscount.Location = new Point(517, 276);
        lblAllowDiscount.Name = "lblAllowDiscount";
        lblAllowDiscount.Size = new Size(102, 15);
        lblAllowDiscount.TabIndex = 63;
        lblAllowDiscount.Text = "Permite descuento:";
        // 
        // lblRequiresScale
        // 
        lblRequiresScale.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiresScale.Appearance.Options.UseFont = true;
        lblRequiresScale.Location = new Point(517, 248);
        lblRequiresScale.Name = "lblRequiresScale";
        lblRequiresScale.Size = new Size(92, 15);
        lblRequiresScale.TabIndex = 61;
        lblRequiresScale.Text = "Requiere balanza:";
        // 
        // lblExpirationManaged
        // 
        lblExpirationManaged.Appearance.Font = new Font("Segoe UI", 9F);
        lblExpirationManaged.Appearance.Options.UseFont = true;
        lblExpirationManaged.Location = new Point(517, 220);
        lblExpirationManaged.Name = "lblExpirationManaged";
        lblExpirationManaged.Size = new Size(111, 15);
        lblExpirationManaged.TabIndex = 59;
        lblExpirationManaged.Text = "Maneja vencimiento:";
        // 
        // lblPerishable
        // 
        lblPerishable.Appearance.Font = new Font("Segoe UI", 9F);
        lblPerishable.Appearance.Options.UseFont = true;
        lblPerishable.Location = new Point(517, 192);
        lblPerishable.Name = "lblPerishable";
        lblPerishable.Size = new Size(51, 15);
        lblPerishable.TabIndex = 57;
        lblPerishable.Text = "Perecible:";
        // 
        // lblSerialManaged
        // 
        lblSerialManaged.Appearance.Font = new Font("Segoe UI", 9F);
        lblSerialManaged.Appearance.Options.UseFont = true;
        lblSerialManaged.Location = new Point(517, 164);
        lblSerialManaged.Name = "lblSerialManaged";
        lblSerialManaged.Size = new Size(76, 15);
        lblSerialManaged.TabIndex = 55;
        lblSerialManaged.Text = "Controla serie:";
        // 
        // lblBatchManaged
        // 
        lblBatchManaged.Appearance.Font = new Font("Segoe UI", 9F);
        lblBatchManaged.Appearance.Options.UseFont = true;
        lblBatchManaged.Location = new Point(517, 136);
        lblBatchManaged.Name = "lblBatchManaged";
        lblBatchManaged.Size = new Size(72, 15);
        lblBatchManaged.TabIndex = 53;
        lblBatchManaged.Text = "Controla lote:";
        // 
        // lblGeneralOperationTitle
        // 
        lblGeneralOperationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneralOperationTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblGeneralOperationTitle.Appearance.Options.UseFont = true;
        lblGeneralOperationTitle.Appearance.Options.UseForeColor = true;
        lblGeneralOperationTitle.Location = new Point(513, 12);
        lblGeneralOperationTitle.Name = "lblGeneralOperationTitle";
        lblGeneralOperationTitle.Size = new Size(185, 20);
        lblGeneralOperationTitle.TabIndex = 46;
        lblGeneralOperationTitle.Text = "2. Clasificación y operación";
        // 
        // tglGeneralBatchManaged
        // 
        tglGeneralBatchManaged.EditValue = true;
        tglGeneralBatchManaged.Location = new Point(638, 134);
        tglGeneralBatchManaged.Name = "tglGeneralBatchManaged";
        tglGeneralBatchManaged.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralBatchManaged.Properties.Appearance.Options.UseFont = true;
        tglGeneralBatchManaged.Properties.OffText = "No";
        tglGeneralBatchManaged.Properties.OnText = "Sí";
        tglGeneralBatchManaged.Size = new Size(86, 20);
        tglGeneralBatchManaged.TabIndex = 67;
        // 
        // tglGeneralSerialManaged
        // 
        tglGeneralSerialManaged.EditValue = true;
        tglGeneralSerialManaged.Location = new Point(638, 162);
        tglGeneralSerialManaged.Name = "tglGeneralSerialManaged";
        tglGeneralSerialManaged.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralSerialManaged.Properties.Appearance.Options.UseFont = true;
        tglGeneralSerialManaged.Properties.OffText = "No";
        tglGeneralSerialManaged.Properties.OnText = "Sí";
        tglGeneralSerialManaged.Size = new Size(86, 20);
        tglGeneralSerialManaged.TabIndex = 68;
        // 
        // tglGeneralPerishable
        // 
        tglGeneralPerishable.EditValue = true;
        tglGeneralPerishable.Location = new Point(638, 190);
        tglGeneralPerishable.Name = "tglGeneralPerishable";
        tglGeneralPerishable.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralPerishable.Properties.Appearance.Options.UseFont = true;
        tglGeneralPerishable.Properties.OffText = "No";
        tglGeneralPerishable.Properties.OnText = "Sí";
        tglGeneralPerishable.Size = new Size(86, 20);
        tglGeneralPerishable.TabIndex = 69;
        // 
        // tglGeneralAllowDiscount
        // 
        tglGeneralAllowDiscount.EditValue = true;
        tglGeneralAllowDiscount.Location = new Point(638, 274);
        tglGeneralAllowDiscount.Name = "tglGeneralAllowDiscount";
        tglGeneralAllowDiscount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralAllowDiscount.Properties.Appearance.Options.UseFont = true;
        tglGeneralAllowDiscount.Properties.OffText = "No";
        tglGeneralAllowDiscount.Properties.OnText = "Sí";
        tglGeneralAllowDiscount.Size = new Size(86, 20);
        tglGeneralAllowDiscount.TabIndex = 70;
        // 
        // tglGeneralRequiresScale
        // 
        tglGeneralRequiresScale.EditValue = true;
        tglGeneralRequiresScale.Location = new Point(638, 246);
        tglGeneralRequiresScale.Name = "tglGeneralRequiresScale";
        tglGeneralRequiresScale.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralRequiresScale.Properties.Appearance.Options.UseFont = true;
        tglGeneralRequiresScale.Properties.OffText = "No";
        tglGeneralRequiresScale.Properties.OnText = "Sí";
        tglGeneralRequiresScale.Size = new Size(86, 20);
        tglGeneralRequiresScale.TabIndex = 71;
        // 
        // lblGeneralMobileItem
        // 
        lblGeneralMobileItem.Appearance.Font = new Font("Segoe UI", 9F);
        lblGeneralMobileItem.Appearance.Options.UseFont = true;
        lblGeneralMobileItem.Location = new Point(517, 304);
        lblGeneralMobileItem.Name = "lblGeneralMobileItem";
        lblGeneralMobileItem.Size = new Size(86, 15);
        lblGeneralMobileItem.TabIndex = 72;
        lblGeneralMobileItem.Text = "Item para móvil:";
        // 
        // tglGeneralMobileItem
        // 
        tglGeneralMobileItem.EditValue = true;
        tglGeneralMobileItem.Location = new Point(638, 302);
        tglGeneralMobileItem.Name = "tglGeneralMobileItem";
        tglGeneralMobileItem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralMobileItem.Properties.Appearance.Options.UseFont = true;
        tglGeneralMobileItem.Properties.OffText = "No";
        tglGeneralMobileItem.Properties.OnText = "Sí";
        tglGeneralMobileItem.Size = new Size(86, 20);
        tglGeneralMobileItem.TabIndex = 73;
        // 
        // pnlKpiVariants
        // 
        pnlKpiVariants.Controls.Add(lblKpiVariantsCaption);
        pnlKpiVariants.Controls.Add(lblKpiVariantsValue);
        pnlKpiVariants.Location = new Point(966, 212);
        pnlKpiVariants.Name = "pnlKpiVariants";
        pnlKpiVariants.Size = new Size(151, 72);
        pnlKpiVariants.TabIndex = 80;
        // 
        // lblKpiVariantsValue
        // 
        lblKpiVariantsValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblKpiVariantsValue.Appearance.Options.UseFont = true;
        lblKpiVariantsValue.Location = new Point(18, 26);
        lblKpiVariantsValue.Name = "lblKpiVariantsValue";
        lblKpiVariantsValue.Size = new Size(11, 25);
        lblKpiVariantsValue.TabIndex = 1;
        lblKpiVariantsValue.Text = "4";
        // 
        // lblKpiVariantsCaption
        // 
        lblKpiVariantsCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblKpiVariantsCaption.Appearance.Options.UseFont = true;
        lblKpiVariantsCaption.Location = new Point(18, 5);
        lblKpiVariantsCaption.Name = "lblKpiVariantsCaption";
        lblKpiVariantsCaption.Size = new Size(87, 15);
        lblKpiVariantsCaption.TabIndex = 0;
        lblKpiVariantsCaption.Text = "Variantes activas";
        // 
        // pnlKpiSap
        // 
        pnlKpiSap.Controls.Add(lblKpiSapCaption);
        pnlKpiSap.Controls.Add(lblKpiSapValue);
        pnlKpiSap.Location = new Point(806, 212);
        pnlKpiSap.Name = "pnlKpiSap";
        pnlKpiSap.Size = new Size(151, 72);
        pnlKpiSap.TabIndex = 79;
        // 
        // lblKpiSapValue
        // 
        lblKpiSapValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblKpiSapValue.Appearance.ForeColor = Color.FromArgb(0, 168, 120);
        lblKpiSapValue.Appearance.Options.UseFont = true;
        lblKpiSapValue.Appearance.Options.UseForeColor = true;
        lblKpiSapValue.Location = new Point(18, 26);
        lblKpiSapValue.Name = "lblKpiSapValue";
        lblKpiSapValue.Size = new Size(110, 25);
        lblKpiSapValue.TabIndex = 1;
        lblKpiSapValue.Text = "Sincronizado";
        // 
        // lblKpiSapCaption
        // 
        lblKpiSapCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblKpiSapCaption.Appearance.Options.UseFont = true;
        lblKpiSapCaption.Location = new Point(18, 5);
        lblKpiSapCaption.Name = "lblKpiSapCaption";
        lblKpiSapCaption.Size = new Size(59, 15);
        lblKpiSapCaption.TabIndex = 0;
        lblKpiSapCaption.Text = "Estado SAP";
        // 
        // pnlKpiSales
        // 
        pnlKpiSales.Controls.Add(lblKpiSalesCaption);
        pnlKpiSales.Controls.Add(lblKpiSalesValue);
        pnlKpiSales.Controls.Add(lblKpiSalesUnit);
        pnlKpiSales.Location = new Point(966, 134);
        pnlKpiSales.Name = "pnlKpiSales";
        pnlKpiSales.Size = new Size(151, 72);
        pnlKpiSales.TabIndex = 78;
        // 
        // lblKpiSalesUnit
        // 
        lblKpiSalesUnit.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblKpiSalesUnit.Appearance.Options.UseFont = true;
        lblKpiSalesUnit.Location = new Point(18, 52);
        lblKpiSalesUnit.Name = "lblKpiSalesUnit";
        lblKpiSalesUnit.Size = new Size(24, 13);
        lblKpiSalesUnit.TabIndex = 2;
        lblKpiSalesUnit.Text = "UND";
        // 
        // lblKpiSalesValue
        // 
        lblKpiSalesValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblKpiSalesValue.Appearance.Options.UseFont = true;
        lblKpiSalesValue.Location = new Point(18, 23);
        lblKpiSalesValue.Name = "lblKpiSalesValue";
        lblKpiSalesValue.Size = new Size(76, 25);
        lblKpiSalesValue.TabIndex = 1;
        lblKpiSalesValue.Text = "4,250.00";
        // 
        // lblKpiSalesCaption
        // 
        lblKpiSalesCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblKpiSalesCaption.Appearance.Options.UseFont = true;
        lblKpiSalesCaption.Location = new Point(18, 5);
        lblKpiSalesCaption.Name = "lblKpiSalesCaption";
        lblKpiSalesCaption.Size = new Size(61, 15);
        lblKpiSalesCaption.TabIndex = 0;
        lblKpiSalesCaption.Text = "Ventas 12m";
        // 
        // pnlKpiPurchases
        // 
        pnlKpiPurchases.Controls.Add(lblKpiPurchasesCaption);
        pnlKpiPurchases.Controls.Add(lblKpiPurchasesValue);
        pnlKpiPurchases.Controls.Add(lblKpiPurchasesUnit);
        pnlKpiPurchases.Location = new Point(808, 134);
        pnlKpiPurchases.Name = "pnlKpiPurchases";
        pnlKpiPurchases.Size = new Size(151, 72);
        pnlKpiPurchases.TabIndex = 77;
        // 
        // lblKpiPurchasesUnit
        // 
        lblKpiPurchasesUnit.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblKpiPurchasesUnit.Appearance.Options.UseFont = true;
        lblKpiPurchasesUnit.Location = new Point(18, 52);
        lblKpiPurchasesUnit.Name = "lblKpiPurchasesUnit";
        lblKpiPurchasesUnit.Size = new Size(24, 13);
        lblKpiPurchasesUnit.TabIndex = 2;
        lblKpiPurchasesUnit.Text = "UND";
        // 
        // lblKpiPurchasesValue
        // 
        lblKpiPurchasesValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblKpiPurchasesValue.Appearance.Options.UseFont = true;
        lblKpiPurchasesValue.Location = new Point(18, 26);
        lblKpiPurchasesValue.Name = "lblKpiPurchasesValue";
        lblKpiPurchasesValue.Size = new Size(76, 25);
        lblKpiPurchasesValue.TabIndex = 1;
        lblKpiPurchasesValue.Text = "3,450.00";
        // 
        // lblKpiPurchasesCaption
        // 
        lblKpiPurchasesCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblKpiPurchasesCaption.Appearance.Options.UseFont = true;
        lblKpiPurchasesCaption.Location = new Point(18, 5);
        lblKpiPurchasesCaption.Name = "lblKpiPurchasesCaption";
        lblKpiPurchasesCaption.Size = new Size(48, 15);
        lblKpiPurchasesCaption.TabIndex = 0;
        lblKpiPurchasesCaption.Text = "Compras";
        // 
        // pnlKpiOrders
        // 
        pnlKpiOrders.Controls.Add(lblKpiOrdersCaption);
        pnlKpiOrders.Controls.Add(lblKpiOrdersValue);
        pnlKpiOrders.Controls.Add(lblKpiOrdersUnit);
        pnlKpiOrders.Location = new Point(966, 56);
        pnlKpiOrders.Name = "pnlKpiOrders";
        pnlKpiOrders.Size = new Size(151, 72);
        pnlKpiOrders.TabIndex = 76;
        // 
        // lblKpiOrdersUnit
        // 
        lblKpiOrdersUnit.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblKpiOrdersUnit.Appearance.Options.UseFont = true;
        lblKpiOrdersUnit.Location = new Point(18, 52);
        lblKpiOrdersUnit.Name = "lblKpiOrdersUnit";
        lblKpiOrdersUnit.Size = new Size(24, 13);
        lblKpiOrdersUnit.TabIndex = 2;
        lblKpiOrdersUnit.Text = "UND";
        // 
        // lblKpiOrdersValue
        // 
        lblKpiOrdersValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblKpiOrdersValue.Appearance.Options.UseFont = true;
        lblKpiOrdersValue.Location = new Point(18, 26);
        lblKpiOrdersValue.Name = "lblKpiOrdersValue";
        lblKpiOrdersValue.Size = new Size(57, 25);
        lblKpiOrdersValue.TabIndex = 1;
        lblKpiOrdersValue.Text = "120.00";
        // 
        // lblKpiOrdersCaption
        // 
        lblKpiOrdersCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblKpiOrdersCaption.Appearance.Options.UseFont = true;
        lblKpiOrdersCaption.Location = new Point(18, 5);
        lblKpiOrdersCaption.Name = "lblKpiOrdersCaption";
        lblKpiOrdersCaption.Size = new Size(87, 15);
        lblKpiOrdersCaption.TabIndex = 0;
        lblKpiOrdersCaption.Text = "Pedidos abiertos";
        // 
        // pnlKpiStock
        // 
        pnlKpiStock.Controls.Add(lblKpiStockCaption);
        pnlKpiStock.Controls.Add(lblKpiStockValue);
        pnlKpiStock.Controls.Add(lblKpiStockUnit);
        pnlKpiStock.Location = new Point(808, 56);
        pnlKpiStock.Name = "pnlKpiStock";
        pnlKpiStock.Size = new Size(151, 72);
        pnlKpiStock.TabIndex = 75;
        // 
        // lblKpiStockUnit
        // 
        lblKpiStockUnit.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblKpiStockUnit.Appearance.Options.UseFont = true;
        lblKpiStockUnit.Location = new Point(18, 52);
        lblKpiStockUnit.Name = "lblKpiStockUnit";
        lblKpiStockUnit.Size = new Size(24, 13);
        lblKpiStockUnit.TabIndex = 2;
        lblKpiStockUnit.Text = "UND";
        // 
        // lblKpiStockValue
        // 
        lblKpiStockValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblKpiStockValue.Appearance.Options.UseFont = true;
        lblKpiStockValue.Location = new Point(18, 26);
        lblKpiStockValue.Name = "lblKpiStockValue";
        lblKpiStockValue.Size = new Size(73, 25);
        lblKpiStockValue.TabIndex = 1;
        lblKpiStockValue.Text = "1,050.00";
        // 
        // lblKpiStockCaption
        // 
        lblKpiStockCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblKpiStockCaption.Appearance.Options.UseFont = true;
        lblKpiStockCaption.Location = new Point(18, 5);
        lblKpiStockCaption.Name = "lblKpiStockCaption";
        lblKpiStockCaption.Size = new Size(87, 15);
        lblKpiStockCaption.TabIndex = 0;
        lblKpiStockCaption.Text = "Stock disponible";
        // 
        // lblGeneralSummaryTitle
        // 
        lblGeneralSummaryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneralSummaryTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblGeneralSummaryTitle.Appearance.Options.UseFont = true;
        lblGeneralSummaryTitle.Appearance.Options.UseForeColor = true;
        lblGeneralSummaryTitle.Location = new Point(798, 12);
        lblGeneralSummaryTitle.Name = "lblGeneralSummaryTitle";
        lblGeneralSummaryTitle.Size = new Size(159, 20);
        lblGeneralSummaryTitle.TabIndex = 74;
        lblGeneralSummaryTitle.Text = "3. Resumen del artículo";
        // 
        // tglBlockedEcommerce
        // 
        tglBlockedEcommerce.Location = new Point(639, 330);
        tglBlockedEcommerce.Name = "tglBlockedEcommerce";
        tglBlockedEcommerce.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglBlockedEcommerce.Properties.Appearance.Options.UseFont = true;
        tglBlockedEcommerce.Properties.OffText = "No";
        tglBlockedEcommerce.Properties.OnText = "Sí";
        tglBlockedEcommerce.Size = new Size(86, 20);
        tglBlockedEcommerce.TabIndex = 82;
        // 
        // lblBlockedEcommerce
        // 
        lblBlockedEcommerce.Appearance.Font = new Font("Segoe UI", 9F);
        lblBlockedEcommerce.Appearance.Options.UseFont = true;
        lblBlockedEcommerce.Location = new Point(517, 332);
        lblBlockedEcommerce.Name = "lblBlockedEcommerce";
        lblBlockedEcommerce.Size = new Size(98, 15);
        lblBlockedEcommerce.TabIndex = 81;
        lblBlockedEcommerce.Text = "Item e-commerce:";
        // 
        // tabMain
        // 
        tabMain.Appearance.Font = new Font("Segoe UI", 9F);
        tabMain.Appearance.Options.UseFont = true;
        tabMain.AppearancePage.Header.Font = new Font("Segoe UI", 9F);
        tabMain.AppearancePage.Header.Options.UseFont = true;
        tabMain.AppearancePage.HeaderActive.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        tabMain.AppearancePage.HeaderActive.Options.UseFont = true;
        tabMain.Dock = DockStyle.Fill;
        tabMain.Location = new Point(0, 204);
        tabMain.Name = "tabMain";
        tabMain.SelectedTabPage = tabGeneral;
        tabMain.Size = new Size(1420, 564);
        tabMain.TabIndex = 1;
        tabMain.TabPages.AddRange(new XtraTabPage[] { tabGeneral, tabUnits, tabInventory, tabPurchases, tabSales, tabCosts, tabAccounting, tabTaxes, tabLots, tabSap, tabAttachments, tabRemarks });
        // 
        // btnClearSapFields
        // 
        btnClearSapFields.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnClearSapFields.Appearance.Options.UseFont = true;
        btnClearSapFields.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnClearSapFields.ImageOptions.SvgImage");
        btnClearSapFields.Location = new Point(384, 364);
        btnClearSapFields.Name = "btnClearSapFields";
        btnClearSapFields.Size = new Size(118, 28);
        btnClearSapFields.TabIndex = 95;
        btnClearSapFields.Text = "Limpiar";
        // 
        // btnRemoveSapField
        // 
        btnRemoveSapField.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRemoveSapField.Appearance.Options.UseFont = true;
        btnRemoveSapField.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnRemoveSapField.ImageOptions.SvgImage");
        btnRemoveSapField.Location = new Point(260, 364);
        btnRemoveSapField.Name = "btnRemoveSapField";
        btnRemoveSapField.Size = new Size(118, 28);
        btnRemoveSapField.TabIndex = 94;
        btnRemoveSapField.Text = "Quitar";
        // 
        // btnUpdateSapField
        // 
        btnUpdateSapField.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnUpdateSapField.Appearance.Options.UseFont = true;
        btnUpdateSapField.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnUpdateSapField.ImageOptions.SvgImage");
        btnUpdateSapField.Location = new Point(136, 364);
        btnUpdateSapField.Name = "btnUpdateSapField";
        btnUpdateSapField.Size = new Size(118, 28);
        btnUpdateSapField.TabIndex = 93;
        btnUpdateSapField.Text = "Actualizar";
        // 
        // btnAddSapField
        // 
        btnAddSapField.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddSapField.Appearance.Options.UseFont = true;
        btnAddSapField.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddSapField.ImageOptions.SvgImage");
        btnAddSapField.Location = new Point(12, 364);
        btnAddSapField.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAddSapField.Name = "btnAddSapField";
        btnAddSapField.Size = new Size(118, 28);
        btnAddSapField.TabIndex = 92;
        btnAddSapField.Text = "Agregar";
        // 
        // grdSapFieldMapping
        // 
        grdSapFieldMapping.Location = new Point(508, 225);
        grdSapFieldMapping.MainView = grvSapFieldMapping;
        grdSapFieldMapping.Name = "grdSapFieldMapping";
        grdSapFieldMapping.Size = new Size(565, 167);
        grdSapFieldMapping.TabIndex = 61;
        grdSapFieldMapping.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvSapFieldMapping });
        // 
        // grvSapFieldMapping
        // 
        grvSapFieldMapping.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSapFieldMapping.Appearance.HeaderPanel.Options.UseFont = true;
        grvSapFieldMapping.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSapFieldMapping.Appearance.Row.Options.UseFont = true;
        grvSapFieldMapping.Columns.AddRange(new GridColumn[] { colSapMapSystemField, colSapMapSapField, colSapMapDescription, colSapMapRequired, colSapMapEnabled });
        grvSapFieldMapping.GridControl = grdSapFieldMapping;
        grvSapFieldMapping.Name = "grvSapFieldMapping";
        grvSapFieldMapping.OptionsBehavior.Editable = false;
        grvSapFieldMapping.OptionsView.ShowGroupPanel = false;
        // 
        // colSapMapSystemField
        // 
        colSapMapSystemField.Caption = "Campo sistema";
        colSapMapSystemField.FieldName = "SystemField";
        colSapMapSystemField.Name = "colSapMapSystemField";
        colSapMapSystemField.Visible = true;
        colSapMapSystemField.VisibleIndex = 0;
        colSapMapSystemField.Width = 147;
        // 
        // colSapMapSapField
        // 
        colSapMapSapField.Caption = "Campo SAP";
        colSapMapSapField.FieldName = "SapField";
        colSapMapSapField.Name = "colSapMapSapField";
        colSapMapSapField.Visible = true;
        colSapMapSapField.VisibleIndex = 1;
        colSapMapSapField.Width = 117;
        // 
        // colSapMapDescription
        // 
        colSapMapDescription.Caption = "Descripcion";
        colSapMapDescription.FieldName = "Description";
        colSapMapDescription.Name = "colSapMapDescription";
        colSapMapDescription.Visible = true;
        colSapMapDescription.VisibleIndex = 2;
        colSapMapDescription.Width = 150;
        // 
        // colSapMapRequired
        // 
        colSapMapRequired.Caption = "Oblig.";
        colSapMapRequired.FieldName = "Required";
        colSapMapRequired.Name = "colSapMapRequired";
        colSapMapRequired.Visible = true;
        colSapMapRequired.VisibleIndex = 3;
        colSapMapRequired.Width = 31;
        // 
        // colSapMapEnabled
        // 
        colSapMapEnabled.Caption = "Activo";
        colSapMapEnabled.FieldName = "Enabled";
        colSapMapEnabled.Name = "colSapMapEnabled";
        colSapMapEnabled.Visible = true;
        colSapMapEnabled.VisibleIndex = 4;
        colSapMapEnabled.Width = 34;
        // 
        // lblSapFieldMappingTitle
        // 
        lblSapFieldMappingTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapFieldMappingTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSapFieldMappingTitle.Appearance.Options.UseFont = true;
        lblSapFieldMappingTitle.Appearance.Options.UseForeColor = true;
        lblSapFieldMappingTitle.Location = new Point(12, 194);
        lblSapFieldMappingTitle.Name = "lblSapFieldMappingTitle";
        lblSapFieldMappingTitle.Size = new Size(168, 20);
        lblSapFieldMappingTitle.TabIndex = 58;
        lblSapFieldMappingTitle.Text = "4. Campos sincronizados";
        // 
        // lblSapMapEnabled
        // 
        lblSapMapEnabled.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapEnabled.Appearance.Options.UseFont = true;
        lblSapMapEnabled.Location = new Point(12, 339);
        lblSapMapEnabled.Name = "lblSapMapEnabled";
        lblSapMapEnabled.Size = new Size(37, 15);
        lblSapMapEnabled.TabIndex = 76;
        lblSapMapEnabled.Text = "Activo:";
        // 
        // lblSapMapRequired
        // 
        lblSapMapRequired.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapRequired.Appearance.Options.UseFont = true;
        lblSapMapRequired.Location = new Point(12, 311);
        lblSapMapRequired.Name = "lblSapMapRequired";
        lblSapMapRequired.Size = new Size(63, 15);
        lblSapMapRequired.TabIndex = 72;
        lblSapMapRequired.Text = "Obligatorio:";
        // 
        // lueSapMapEnabled
        // 
        lueSapMapEnabled.Location = new Point(142, 336);
        lueSapMapEnabled.Name = "lueSapMapEnabled";
        lueSapMapEnabled.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapMapEnabled.Properties.Appearance.Options.UseFont = true;
        lueSapMapEnabled.Properties.AutoHeight = false;
        lueSapMapEnabled.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapMapEnabled.Properties.NullText = "";
        lueSapMapEnabled.Size = new Size(150, 22);
        lueSapMapEnabled.TabIndex = 78;
        // 
        // lblSapMapDescription
        // 
        lblSapMapDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapDescription.Appearance.Options.UseFont = true;
        lblSapMapDescription.Location = new Point(12, 283);
        lblSapMapDescription.Name = "lblSapMapDescription";
        lblSapMapDescription.Size = new Size(65, 15);
        lblSapMapDescription.TabIndex = 69;
        lblSapMapDescription.Text = "Descripcion:";
        // 
        // lueSapMapRequired
        // 
        lueSapMapRequired.Location = new Point(142, 308);
        lueSapMapRequired.Name = "lueSapMapRequired";
        lueSapMapRequired.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapMapRequired.Properties.Appearance.Options.UseFont = true;
        lueSapMapRequired.Properties.AutoHeight = false;
        lueSapMapRequired.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapMapRequired.Properties.NullText = "";
        lueSapMapRequired.Size = new Size(150, 22);
        lueSapMapRequired.TabIndex = 74;
        // 
        // lblSapMapSapField
        // 
        lblSapMapSapField.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapSapField.Appearance.Options.UseFont = true;
        lblSapMapSapField.Location = new Point(12, 250);
        lblSapMapSapField.Name = "lblSapMapSapField";
        lblSapMapSapField.Size = new Size(24, 15);
        lblSapMapSapField.TabIndex = 66;
        lblSapMapSapField.Text = "SAP:";
        // 
        // txtSapMapDescription
        // 
        txtSapMapDescription.Location = new Point(142, 280);
        txtSapMapDescription.Name = "txtSapMapDescription";
        txtSapMapDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapMapDescription.Properties.Appearance.Options.UseFont = true;
        txtSapMapDescription.Properties.AutoHeight = false;
        txtSapMapDescription.Size = new Size(360, 22);
        txtSapMapDescription.TabIndex = 70;
        // 
        // lblSapMapSystemField
        // 
        lblSapMapSystemField.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapSystemField.Appearance.Options.UseFont = true;
        lblSapMapSystemField.Location = new Point(12, 227);
        lblSapMapSystemField.Name = "lblSapMapSystemField";
        lblSapMapSystemField.Size = new Size(44, 15);
        lblSapMapSystemField.TabIndex = 63;
        lblSapMapSystemField.Text = "Sistema:";
        // 
        // txtSapMapSapField
        // 
        txtSapMapSapField.Location = new Point(142, 252);
        txtSapMapSapField.Name = "txtSapMapSapField";
        txtSapMapSapField.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapMapSapField.Properties.Appearance.Options.UseFont = true;
        txtSapMapSapField.Properties.AutoHeight = false;
        txtSapMapSapField.Size = new Size(360, 22);
        txtSapMapSapField.TabIndex = 68;
        // 
        // lblSapHistoryTitle
        // 
        lblSapHistoryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapHistoryTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSapHistoryTitle.Appearance.Options.UseFont = true;
        lblSapHistoryTitle.Appearance.Options.UseForeColor = true;
        lblSapHistoryTitle.Location = new Point(620, 12);
        lblSapHistoryTitle.Name = "lblSapHistoryTitle";
        lblSapHistoryTitle.Size = new Size(196, 20);
        lblSapHistoryTitle.TabIndex = 57;
        lblSapHistoryTitle.Text = "3. Historial de sincronizacion";
        // 
        // txtSapMapSystemField
        // 
        txtSapMapSystemField.Location = new Point(142, 224);
        txtSapMapSystemField.Name = "txtSapMapSystemField";
        txtSapMapSystemField.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapMapSystemField.Properties.Appearance.Options.UseFont = true;
        txtSapMapSystemField.Properties.AutoHeight = false;
        txtSapMapSystemField.Size = new Size(360, 22);
        txtSapMapSystemField.TabIndex = 65;
        // 
        // grdSapSyncHistory
        // 
        grdSapSyncHistory.Location = new Point(620, 38);
        grdSapSyncHistory.MainView = grvSapSyncHistory;
        grdSapSyncHistory.Name = "grdSapSyncHistory";
        grdSapSyncHistory.Size = new Size(453, 142);
        grdSapSyncHistory.TabIndex = 60;
        grdSapSyncHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvSapSyncHistory });
        // 
        // grvSapSyncHistory
        // 
        grvSapSyncHistory.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSapSyncHistory.Appearance.HeaderPanel.Options.UseFont = true;
        grvSapSyncHistory.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSapSyncHistory.Appearance.Row.Options.UseFont = true;
        grvSapSyncHistory.Columns.AddRange(new GridColumn[] { colSapHistoryDate, colSapHistoryOperation, colSapHistoryStatus, colSapHistoryDocEntry, colSapHistoryDocNum, colSapHistoryRetryCount, colSapHistoryMessage });
        grvSapSyncHistory.GridControl = grdSapSyncHistory;
        grvSapSyncHistory.Name = "grvSapSyncHistory";
        grvSapSyncHistory.OptionsBehavior.Editable = false;
        grvSapSyncHistory.OptionsView.ShowGroupPanel = false;
        // 
        // colSapHistoryDate
        // 
        colSapHistoryDate.Caption = "Fecha";
        colSapHistoryDate.FieldName = "Date";
        colSapHistoryDate.Name = "colSapHistoryDate";
        colSapHistoryDate.Visible = true;
        colSapHistoryDate.VisibleIndex = 0;
        colSapHistoryDate.Width = 115;
        // 
        // colSapHistoryOperation
        // 
        colSapHistoryOperation.Caption = "Operacion";
        colSapHistoryOperation.FieldName = "Operation";
        colSapHistoryOperation.Name = "colSapHistoryOperation";
        colSapHistoryOperation.Visible = true;
        colSapHistoryOperation.VisibleIndex = 1;
        colSapHistoryOperation.Width = 120;
        // 
        // colSapHistoryStatus
        // 
        colSapHistoryStatus.Caption = "Estado";
        colSapHistoryStatus.FieldName = "Status";
        colSapHistoryStatus.Name = "colSapHistoryStatus";
        colSapHistoryStatus.Visible = true;
        colSapHistoryStatus.VisibleIndex = 2;
        colSapHistoryStatus.Width = 90;
        // 
        // colSapHistoryDocEntry
        // 
        colSapHistoryDocEntry.Caption = "SapDocEntry";
        colSapHistoryDocEntry.FieldName = "SapDocEntry";
        colSapHistoryDocEntry.Name = "colSapHistoryDocEntry";
        colSapHistoryDocEntry.Visible = true;
        colSapHistoryDocEntry.VisibleIndex = 3;
        colSapHistoryDocEntry.Width = 90;
        // 
        // colSapHistoryDocNum
        // 
        colSapHistoryDocNum.Caption = "SapDocNum";
        colSapHistoryDocNum.FieldName = "SapDocNum";
        colSapHistoryDocNum.Name = "colSapHistoryDocNum";
        colSapHistoryDocNum.Visible = true;
        colSapHistoryDocNum.VisibleIndex = 4;
        colSapHistoryDocNum.Width = 90;
        // 
        // colSapHistoryRetryCount
        // 
        colSapHistoryRetryCount.Caption = "Reintentos";
        colSapHistoryRetryCount.FieldName = "RetryCount";
        colSapHistoryRetryCount.Name = "colSapHistoryRetryCount";
        colSapHistoryRetryCount.Visible = true;
        colSapHistoryRetryCount.VisibleIndex = 5;
        colSapHistoryRetryCount.Width = 80;
        // 
        // colSapHistoryMessage
        // 
        colSapHistoryMessage.Caption = "Mensaje";
        colSapHistoryMessage.FieldName = "Message";
        colSapHistoryMessage.Name = "colSapHistoryMessage";
        colSapHistoryMessage.Visible = true;
        colSapHistoryMessage.VisibleIndex = 6;
        colSapHistoryMessage.Width = 120;
        // 
        // lblSapSyncAsSupplier
        // 
        lblSapSyncAsSupplier.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSyncAsSupplier.Appearance.Options.UseFont = true;
        lblSapSyncAsSupplier.Location = new Point(319, 101);
        lblSapSyncAsSupplier.Name = "lblSapSyncAsSupplier";
        lblSapSyncAsSupplier.Size = new Size(118, 15);
        lblSapSyncAsSupplier.TabIndex = 71;
        lblSapSyncAsSupplier.Text = "Sincronizar proveedor:";
        // 
        // lblSapMode
        // 
        lblSapMode.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMode.Appearance.Options.UseFont = true;
        lblSapMode.Location = new Point(319, 45);
        lblSapMode.Name = "lblSapMode";
        lblSapMode.Size = new Size(59, 15);
        lblSapMode.TabIndex = 59;
        lblSapMode.Text = "Modo SAP:";
        // 
        // lueSapSyncAsSupplier
        // 
        lueSapSyncAsSupplier.Location = new Point(443, 98);
        lueSapSyncAsSupplier.Name = "lueSapSyncAsSupplier";
        lueSapSyncAsSupplier.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapSyncAsSupplier.Properties.Appearance.Options.UseFont = true;
        lueSapSyncAsSupplier.Properties.AutoHeight = false;
        lueSapSyncAsSupplier.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapSyncAsSupplier.Properties.NullText = "";
        lueSapSyncAsSupplier.Size = new Size(149, 22);
        lueSapSyncAsSupplier.TabIndex = 73;
        // 
        // lblSapConfigTitle
        // 
        lblSapConfigTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapConfigTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSapConfigTitle.Appearance.Options.UseFont = true;
        lblSapConfigTitle.Appearance.Options.UseForeColor = true;
        lblSapConfigTitle.Location = new Point(319, 12);
        lblSapConfigTitle.Name = "lblSapConfigTitle";
        lblSapConfigTitle.Size = new Size(216, 20);
        lblSapConfigTitle.TabIndex = 56;
        lblSapConfigTitle.Text = "2. Configuracion de integracion";
        // 
        // lblSapManualRetry
        // 
        lblSapManualRetry.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapManualRetry.Appearance.Options.UseFont = true;
        lblSapManualRetry.Location = new Point(319, 129);
        lblSapManualRetry.Name = "lblSapManualRetry";
        lblSapManualRetry.Size = new Size(97, 15);
        lblSapManualRetry.TabIndex = 75;
        lblSapManualRetry.Text = "Reintento manual:";
        // 
        // lueSapMode
        // 
        lueSapMode.Location = new Point(443, 42);
        lueSapMode.Name = "lueSapMode";
        lueSapMode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapMode.Properties.Appearance.Options.UseFont = true;
        lueSapMode.Properties.AutoHeight = false;
        lueSapMode.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapMode.Properties.NullText = "";
        lueSapMode.Size = new Size(150, 22);
        lueSapMode.TabIndex = 62;
        // 
        // lueSapManualRetry
        // 
        lueSapManualRetry.Location = new Point(443, 126);
        lueSapManualRetry.Name = "lueSapManualRetry";
        lueSapManualRetry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapManualRetry.Properties.Appearance.Options.UseFont = true;
        lueSapManualRetry.Properties.AutoHeight = false;
        lueSapManualRetry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapManualRetry.Properties.NullText = "";
        lueSapManualRetry.Size = new Size(149, 22);
        lueSapManualRetry.TabIndex = 77;
        // 
        // lblSapCompany
        // 
        lblSapCompany.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapCompany.Appearance.Options.UseFont = true;
        lblSapCompany.Location = new Point(319, 73);
        lblSapCompany.Name = "lblSapCompany";
        lblSapCompany.Size = new Size(72, 15);
        lblSapCompany.TabIndex = 64;
        lblSapCompany.Text = "Empresa SAP:";
        // 
        // lblSapRequiresApproval
        // 
        lblSapRequiresApproval.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapRequiresApproval.Appearance.Options.UseFont = true;
        lblSapRequiresApproval.Location = new Point(319, 157);
        lblSapRequiresApproval.Name = "lblSapRequiresApproval";
        lblSapRequiresApproval.Size = new Size(112, 15);
        lblSapRequiresApproval.TabIndex = 79;
        lblSapRequiresApproval.Text = "Requiere aprobacion:";
        // 
        // lblSapStatusTitle
        // 
        lblSapStatusTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapStatusTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSapStatusTitle.Appearance.Options.UseFont = true;
        lblSapStatusTitle.Appearance.Options.UseForeColor = true;
        lblSapStatusTitle.Location = new Point(12, 12);
        lblSapStatusTitle.Name = "lblSapStatusTitle";
        lblSapStatusTitle.Size = new Size(90, 20);
        lblSapStatusTitle.TabIndex = 81;
        lblSapStatusTitle.Text = "1. Estado SAP";
        // 
        // lueSapRequiresApproval
        // 
        lueSapRequiresApproval.Location = new Point(443, 154);
        lueSapRequiresApproval.Name = "lueSapRequiresApproval";
        lueSapRequiresApproval.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapRequiresApproval.Properties.Appearance.Options.UseFont = true;
        lueSapRequiresApproval.Properties.AutoHeight = false;
        lueSapRequiresApproval.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapRequiresApproval.Properties.NullText = "";
        lueSapRequiresApproval.Size = new Size(149, 22);
        lueSapRequiresApproval.TabIndex = 80;
        // 
        // lueSapCompany
        // 
        lueSapCompany.Location = new Point(443, 70);
        lueSapCompany.Name = "lueSapCompany";
        lueSapCompany.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapCompany.Properties.Appearance.Options.UseFont = true;
        lueSapCompany.Properties.AutoHeight = false;
        lueSapCompany.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapCompany.Properties.NullText = "";
        lueSapCompany.Properties.PopupView = grvSapCompanyLookup;
        lueSapCompany.Size = new Size(150, 22);
        lueSapCompany.TabIndex = 67;
        // 
        // grvSapCompanyLookup
        // 
        grvSapCompanyLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSapCompanyLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvSapCompanyLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSapCompanyLookup.Appearance.Row.Options.UseFont = true;
        grvSapCompanyLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvSapCompanyLookup.Name = "grvSapCompanyLookup";
        grvSapCompanyLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvSapCompanyLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblSapSyncStatus
        // 
        lblSapSyncStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSyncStatus.Appearance.Options.UseFont = true;
        lblSapSyncStatus.Location = new Point(12, 45);
        lblSapSyncStatus.Name = "lblSapSyncStatus";
        lblSapSyncStatus.Size = new Size(117, 15);
        lblSapSyncStatus.TabIndex = 82;
        lblSapSyncStatus.Text = "Estado sincronizacion:";
        // 
        // lueSapSyncStatus
        // 
        lueSapSyncStatus.Location = new Point(142, 42);
        lueSapSyncStatus.Name = "lueSapSyncStatus";
        lueSapSyncStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapSyncStatus.Properties.Appearance.Options.UseFont = true;
        lueSapSyncStatus.Properties.AutoHeight = false;
        lueSapSyncStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapSyncStatus.Properties.NullText = "";
        lueSapSyncStatus.Size = new Size(150, 22);
        lueSapSyncStatus.TabIndex = 83;
        // 
        // lblSapLastSync
        // 
        lblSapLastSync.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastSync.Appearance.Options.UseFont = true;
        lblSapLastSync.Location = new Point(12, 101);
        lblSapLastSync.Name = "lblSapLastSync";
        lblSapLastSync.Size = new Size(117, 15);
        lblSapLastSync.TabIndex = 84;
        lblSapLastSync.Text = "Ultima sincronizacion:";
        // 
        // txtSapLastSync
        // 
        txtSapLastSync.Location = new Point(142, 98);
        txtSapLastSync.Name = "txtSapLastSync";
        txtSapLastSync.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapLastSync.Properties.Appearance.Options.UseFont = true;
        txtSapLastSync.Properties.AutoHeight = false;
        txtSapLastSync.Properties.ReadOnly = true;
        txtSapLastSync.Size = new Size(150, 22);
        txtSapLastSync.TabIndex = 85;
        // 
        // lblSapLastError
        // 
        lblSapLastError.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastError.Appearance.Options.UseFont = true;
        lblSapLastError.Location = new Point(12, 129);
        lblSapLastError.Name = "lblSapLastError";
        lblSapLastError.Size = new Size(67, 15);
        lblSapLastError.TabIndex = 86;
        lblSapLastError.Text = "Ultimo error:";
        // 
        // txtSapLastError
        // 
        txtSapLastError.Location = new Point(142, 126);
        txtSapLastError.Name = "txtSapLastError";
        txtSapLastError.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapLastError.Properties.Appearance.Options.UseFont = true;
        txtSapLastError.Properties.AutoHeight = false;
        txtSapLastError.Properties.ReadOnly = true;
        txtSapLastError.Size = new Size(150, 22);
        txtSapLastError.TabIndex = 87;
        // 
        // lblSapRetryCount
        // 
        lblSapRetryCount.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapRetryCount.Appearance.Options.UseFont = true;
        lblSapRetryCount.Location = new Point(12, 73);
        lblSapRetryCount.Name = "lblSapRetryCount";
        lblSapRetryCount.Size = new Size(59, 15);
        lblSapRetryCount.TabIndex = 88;
        lblSapRetryCount.Text = "Reintentos:";
        // 
        // txtSapRetryCount
        // 
        txtSapRetryCount.Location = new Point(142, 70);
        txtSapRetryCount.Name = "txtSapRetryCount";
        txtSapRetryCount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapRetryCount.Properties.Appearance.Options.UseFont = true;
        txtSapRetryCount.Properties.Appearance.Options.UseTextOptions = true;
        txtSapRetryCount.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        txtSapRetryCount.Properties.AutoHeight = false;
        txtSapRetryCount.Properties.ReadOnly = true;
        txtSapRetryCount.Size = new Size(150, 22);
        txtSapRetryCount.TabIndex = 89;
        // 
        // lblSapEnabled
        // 
        lblSapEnabled.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapEnabled.Appearance.Options.UseFont = true;
        lblSapEnabled.Location = new Point(12, 157);
        lblSapEnabled.Name = "lblSapEnabled";
        lblSapEnabled.Size = new Size(118, 15);
        lblSapEnabled.TabIndex = 90;
        lblSapEnabled.Text = "Integracion habilitada:";
        // 
        // lueSapEnabled
        // 
        lueSapEnabled.Location = new Point(142, 154);
        lueSapEnabled.Name = "lueSapEnabled";
        lueSapEnabled.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapEnabled.Properties.Appearance.Options.UseFont = true;
        lueSapEnabled.Properties.AutoHeight = false;
        lueSapEnabled.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapEnabled.Properties.NullText = "";
        lueSapEnabled.Size = new Size(150, 22);
        lueSapEnabled.TabIndex = 91;
        // 
        // lblAttachmentPreviewTitle
        // 
        lblAttachmentPreviewTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAttachmentPreviewTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAttachmentPreviewTitle.Appearance.Options.UseFont = true;
        lblAttachmentPreviewTitle.Appearance.Options.UseForeColor = true;
        lblAttachmentPreviewTitle.Location = new Point(12, 12);
        lblAttachmentPreviewTitle.Name = "lblAttachmentPreviewTitle";
        lblAttachmentPreviewTitle.Size = new Size(129, 20);
        lblAttachmentPreviewTitle.TabIndex = 7;
        lblAttachmentPreviewTitle.Text = "1. Imagen principal";
        // 
        // picMainAttachmentPreview
        // 
        picMainAttachmentPreview.Location = new Point(14, 48);
        picMainAttachmentPreview.Name = "picMainAttachmentPreview";
        picMainAttachmentPreview.Properties.Appearance.BackColor = Color.White;
        picMainAttachmentPreview.Properties.Appearance.Options.UseBackColor = true;
        picMainAttachmentPreview.Properties.BorderStyle = BorderStyles.Simple;
        picMainAttachmentPreview.Properties.NullText = "Imagen del producto";
        picMainAttachmentPreview.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
        picMainAttachmentPreview.Properties.SizeMode = PictureSizeMode.Zoom;
        picMainAttachmentPreview.Size = new Size(434, 290);
        picMainAttachmentPreview.TabIndex = 8;
        // 
        // btnLoadImage
        // 
        btnLoadImage.Appearance.Font = new Font("Segoe UI", 9F);
        btnLoadImage.Appearance.Options.UseFont = true;
        btnLoadImage.Location = new Point(14, 352);
        btnLoadImage.Name = "btnLoadImage";
        btnLoadImage.Size = new Size(104, 28);
        btnLoadImage.TabIndex = 9;
        btnLoadImage.Text = "Cargar imagen";
        // 
        // btnRemoveImage
        // 
        btnRemoveImage.Appearance.Font = new Font("Segoe UI", 9F);
        btnRemoveImage.Appearance.Options.UseFont = true;
        btnRemoveImage.Location = new Point(128, 352);
        btnRemoveImage.Name = "btnRemoveImage";
        btnRemoveImage.Size = new Size(104, 28);
        btnRemoveImage.TabIndex = 10;
        btnRemoveImage.Text = "Quitar imagen";
        // 
        // btnPreviewImage
        // 
        btnPreviewImage.Appearance.Font = new Font("Segoe UI", 9F);
        btnPreviewImage.Appearance.Options.UseFont = true;
        btnPreviewImage.Location = new Point(242, 352);
        btnPreviewImage.Name = "btnPreviewImage";
        btnPreviewImage.Size = new Size(90, 28);
        btnPreviewImage.TabIndex = 11;
        btnPreviewImage.Text = "Vista previa";
        // 
        // btnSetMainImage
        // 
        btnSetMainImage.Appearance.Font = new Font("Segoe UI", 9F);
        btnSetMainImage.Appearance.Options.UseFont = true;
        btnSetMainImage.Location = new Point(342, 352);
        btnSetMainImage.Name = "btnSetMainImage";
        btnSetMainImage.Size = new Size(106, 28);
        btnSetMainImage.TabIndex = 12;
        btnSetMainImage.Text = "Marcar principal";
        // 
        // pnlAttachmentPreviewNote
        // 
        pnlAttachmentPreviewNote.Appearance.BackColor = Color.FromArgb(238, 248, 255);
        pnlAttachmentPreviewNote.Appearance.Options.UseBackColor = true;
        pnlAttachmentPreviewNote.Controls.Add(lblAttachmentPreviewNoteIcon);
        pnlAttachmentPreviewNote.Controls.Add(lblAttachmentPreviewNote);
        pnlAttachmentPreviewNote.Location = new Point(14, 400);
        pnlAttachmentPreviewNote.Name = "pnlAttachmentPreviewNote";
        pnlAttachmentPreviewNote.Size = new Size(434, 58);
        pnlAttachmentPreviewNote.TabIndex = 13;
        // 
        // lblAttachmentPreviewNoteIcon
        // 
        lblAttachmentPreviewNoteIcon.Appearance.BackColor = Color.FromArgb(0, 122, 204);
        lblAttachmentPreviewNoteIcon.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblAttachmentPreviewNoteIcon.Appearance.ForeColor = Color.White;
        lblAttachmentPreviewNoteIcon.Appearance.Options.UseBackColor = true;
        lblAttachmentPreviewNoteIcon.Appearance.Options.UseFont = true;
        lblAttachmentPreviewNoteIcon.Appearance.Options.UseForeColor = true;
        lblAttachmentPreviewNoteIcon.Appearance.Options.UseTextOptions = true;
        lblAttachmentPreviewNoteIcon.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        lblAttachmentPreviewNoteIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblAttachmentPreviewNoteIcon.Location = new Point(12, 18);
        lblAttachmentPreviewNoteIcon.Name = "lblAttachmentPreviewNoteIcon";
        lblAttachmentPreviewNoteIcon.Size = new Size(18, 18);
        lblAttachmentPreviewNoteIcon.TabIndex = 0;
        lblAttachmentPreviewNoteIcon.Text = "i";
        // 
        // lblAttachmentPreviewNote
        // 
        lblAttachmentPreviewNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblAttachmentPreviewNote.Appearance.ForeColor = Color.FromArgb(31, 42, 68);
        lblAttachmentPreviewNote.Appearance.Options.UseFont = true;
        lblAttachmentPreviewNote.Appearance.Options.UseForeColor = true;
        lblAttachmentPreviewNote.AutoSizeMode = LabelAutoSizeMode.Vertical;
        lblAttachmentPreviewNote.Location = new Point(42, 10);
        lblAttachmentPreviewNote.Name = "lblAttachmentPreviewNote";
        lblAttachmentPreviewNote.Size = new Size(376, 26);
        lblAttachmentPreviewNote.TabIndex = 1;
        lblAttachmentPreviewNote.Text = "La imagen principal se utilizará como referencia visual del artículo en catálogos, documentos y consultas.";
        // 
        // lblAttachmentMetadataTitle
        // 
        lblAttachmentMetadataTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAttachmentMetadataTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAttachmentMetadataTitle.Appearance.Options.UseFont = true;
        lblAttachmentMetadataTitle.Appearance.Options.UseForeColor = true;
        lblAttachmentMetadataTitle.Location = new Point(467, 12);
        lblAttachmentMetadataTitle.Name = "lblAttachmentMetadataTitle";
        lblAttachmentMetadataTitle.Size = new Size(135, 20);
        lblAttachmentMetadataTitle.TabIndex = 22;
        lblAttachmentMetadataTitle.Text = "2. Datos del archivo";
        // 
        // lblAttachmentType
        // 
        lblAttachmentType.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentType.Appearance.Options.UseFont = true;
        lblAttachmentType.Location = new Point(469, 50);
        lblAttachmentType.Name = "lblAttachmentType";
        lblAttachmentType.Size = new Size(92, 15);
        lblAttachmentType.TabIndex = 23;
        lblAttachmentType.Text = "Tipo documento:";
        // 
        // lueAttachmentType
        // 
        lueAttachmentType.EditValue = "Imagen producto";
        lueAttachmentType.Location = new Point(597, 46);
        lueAttachmentType.Name = "lueAttachmentType";
        lueAttachmentType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAttachmentType.Properties.Appearance.Options.UseFont = true;
        lueAttachmentType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAttachmentType.Properties.NullText = "";
        lueAttachmentType.Size = new Size(280, 22);
        lueAttachmentType.TabIndex = 24;
        // 
        // lblAttachmentFileName
        // 
        lblAttachmentFileName.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentFileName.Appearance.Options.UseFont = true;
        lblAttachmentFileName.Location = new Point(469, 82);
        lblAttachmentFileName.Name = "lblAttachmentFileName";
        lblAttachmentFileName.Size = new Size(89, 15);
        lblAttachmentFileName.TabIndex = 25;
        lblAttachmentFileName.Text = "Nombre archivo:";
        // 
        // txtAttachmentFileName
        // 
        txtAttachmentFileName.EditValue = "arroz_1kg_frontal.png";
        txtAttachmentFileName.Location = new Point(597, 78);
        txtAttachmentFileName.Name = "txtAttachmentFileName";
        txtAttachmentFileName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentFileName.Properties.Appearance.Options.UseFont = true;
        txtAttachmentFileName.Size = new Size(280, 22);
        txtAttachmentFileName.TabIndex = 26;
        // 
        // lblAttachmentDescription
        // 
        lblAttachmentDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentDescription.Appearance.Options.UseFont = true;
        lblAttachmentDescription.Location = new Point(469, 114);
        lblAttachmentDescription.Name = "lblAttachmentDescription";
        lblAttachmentDescription.Size = new Size(65, 15);
        lblAttachmentDescription.TabIndex = 27;
        lblAttachmentDescription.Text = "Descripción:";
        // 
        // memAttachmentDescription
        // 
        memAttachmentDescription.EditValue = "Imagen principal frontal del producto";
        memAttachmentDescription.Location = new Point(597, 110);
        memAttachmentDescription.Name = "memAttachmentDescription";
        memAttachmentDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memAttachmentDescription.Properties.Appearance.Options.UseFont = true;
        memAttachmentDescription.Size = new Size(280, 46);
        memAttachmentDescription.TabIndex = 28;
        // 
        // lblAttachmentCategory
        // 
        lblAttachmentCategory.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentCategory.Appearance.Options.UseFont = true;
        lblAttachmentCategory.Location = new Point(469, 172);
        lblAttachmentCategory.Name = "lblAttachmentCategory";
        lblAttachmentCategory.Size = new Size(54, 15);
        lblAttachmentCategory.TabIndex = 29;
        lblAttachmentCategory.Text = "Categoría:";
        // 
        // lueAttachmentCategory
        // 
        lueAttachmentCategory.EditValue = "Comercial";
        lueAttachmentCategory.Location = new Point(597, 168);
        lueAttachmentCategory.Name = "lueAttachmentCategory";
        lueAttachmentCategory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAttachmentCategory.Properties.Appearance.Options.UseFont = true;
        lueAttachmentCategory.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAttachmentCategory.Properties.NullText = "";
        lueAttachmentCategory.Size = new Size(280, 22);
        lueAttachmentCategory.TabIndex = 30;
        // 
        // chkVisibleInSales
        // 
        chkVisibleInSales.EditValue = true;
        chkVisibleInSales.Location = new Point(907, 46);
        chkVisibleInSales.Name = "chkVisibleInSales";
        chkVisibleInSales.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkVisibleInSales.Properties.Appearance.Options.UseFont = true;
        chkVisibleInSales.Properties.Caption = "Visible en ventas";
        chkVisibleInSales.Size = new Size(140, 20);
        chkVisibleInSales.TabIndex = 31;
        // 
        // chkVisibleInPurchases
        // 
        chkVisibleInPurchases.Location = new Point(907, 76);
        chkVisibleInPurchases.Name = "chkVisibleInPurchases";
        chkVisibleInPurchases.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkVisibleInPurchases.Properties.Appearance.Options.UseFont = true;
        chkVisibleInPurchases.Properties.Caption = "Visible en compras";
        chkVisibleInPurchases.Size = new Size(140, 20);
        chkVisibleInPurchases.TabIndex = 32;
        // 
        // chkVisibleInPortal
        // 
        chkVisibleInPortal.EditValue = true;
        chkVisibleInPortal.Location = new Point(907, 106);
        chkVisibleInPortal.Name = "chkVisibleInPortal";
        chkVisibleInPortal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkVisibleInPortal.Properties.Appearance.Options.UseFont = true;
        chkVisibleInPortal.Properties.Caption = "Visible en portal";
        chkVisibleInPortal.Size = new Size(140, 20);
        chkVisibleInPortal.TabIndex = 33;
        // 
        // lblAttachmentStatus
        // 
        lblAttachmentStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentStatus.Appearance.Options.UseFont = true;
        lblAttachmentStatus.Location = new Point(1087, 50);
        lblAttachmentStatus.Name = "lblAttachmentStatus";
        lblAttachmentStatus.Size = new Size(38, 15);
        lblAttachmentStatus.TabIndex = 34;
        lblAttachmentStatus.Text = "Estado:";
        // 
        // lueAttachmentStatus
        // 
        lueAttachmentStatus.EditValue = "Activo";
        lueAttachmentStatus.Location = new Point(1173, 46);
        lueAttachmentStatus.Name = "lueAttachmentStatus";
        lueAttachmentStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAttachmentStatus.Properties.Appearance.Options.UseFont = true;
        lueAttachmentStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAttachmentStatus.Properties.NullText = "";
        lueAttachmentStatus.Size = new Size(140, 22);
        lueAttachmentStatus.TabIndex = 35;
        // 
        // lblAttachmentExtension
        // 
        lblAttachmentExtension.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentExtension.Appearance.Options.UseFont = true;
        lblAttachmentExtension.Location = new Point(1087, 82);
        lblAttachmentExtension.Name = "lblAttachmentExtension";
        lblAttachmentExtension.Size = new Size(53, 15);
        lblAttachmentExtension.TabIndex = 36;
        lblAttachmentExtension.Text = "Extensión:";
        // 
        // txtAttachmentExtension
        // 
        txtAttachmentExtension.EditValue = "PNG";
        txtAttachmentExtension.Location = new Point(1173, 78);
        txtAttachmentExtension.Name = "txtAttachmentExtension";
        txtAttachmentExtension.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentExtension.Properties.Appearance.Options.UseFont = true;
        txtAttachmentExtension.Properties.ReadOnly = true;
        txtAttachmentExtension.Size = new Size(140, 22);
        txtAttachmentExtension.TabIndex = 37;
        // 
        // lblAttachmentSize
        // 
        lblAttachmentSize.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentSize.Appearance.Options.UseFont = true;
        lblAttachmentSize.Location = new Point(1087, 114);
        lblAttachmentSize.Name = "lblAttachmentSize";
        lblAttachmentSize.Size = new Size(47, 15);
        lblAttachmentSize.TabIndex = 38;
        lblAttachmentSize.Text = "Tamaño:";
        // 
        // txtAttachmentSize
        // 
        txtAttachmentSize.EditValue = "2.4 MB";
        txtAttachmentSize.Location = new Point(1173, 110);
        txtAttachmentSize.Name = "txtAttachmentSize";
        txtAttachmentSize.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentSize.Properties.Appearance.Options.UseFont = true;
        txtAttachmentSize.Properties.ReadOnly = true;
        txtAttachmentSize.Size = new Size(140, 22);
        txtAttachmentSize.TabIndex = 39;
        // 
        // lblAttachmentUploadedAt
        // 
        lblAttachmentUploadedAt.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentUploadedAt.Appearance.Options.UseFont = true;
        lblAttachmentUploadedAt.Location = new Point(1087, 146);
        lblAttachmentUploadedAt.Name = "lblAttachmentUploadedAt";
        lblAttachmentUploadedAt.Size = new Size(66, 15);
        lblAttachmentUploadedAt.TabIndex = 40;
        lblAttachmentUploadedAt.Text = "Fecha carga:";
        // 
        // dteAttachmentUploadedAt
        // 
        dteAttachmentUploadedAt.EditValue = new DateTime(2026, 5, 15, 10, 12, 0, 0);
        dteAttachmentUploadedAt.Location = new Point(1173, 142);
        dteAttachmentUploadedAt.Name = "dteAttachmentUploadedAt";
        dteAttachmentUploadedAt.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dteAttachmentUploadedAt.Properties.Appearance.Options.UseFont = true;
        dteAttachmentUploadedAt.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteAttachmentUploadedAt.Properties.CalendarTimeEditing = DefaultBoolean.True;
        dteAttachmentUploadedAt.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteAttachmentUploadedAt.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
        dteAttachmentUploadedAt.Properties.DisplayFormat.FormatType = FormatType.DateTime;
        dteAttachmentUploadedAt.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm";
        dteAttachmentUploadedAt.Properties.EditFormat.FormatType = FormatType.DateTime;
        dteAttachmentUploadedAt.Properties.MaskSettings.Set("mask", "dd/MM/yyyy HH:mm");
        dteAttachmentUploadedAt.Properties.ReadOnly = true;
        dteAttachmentUploadedAt.Size = new Size(140, 22);
        dteAttachmentUploadedAt.TabIndex = 41;
        // 
        // lblAttachmentUser
        // 
        lblAttachmentUser.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentUser.Appearance.Options.UseFont = true;
        lblAttachmentUser.Location = new Point(1087, 178);
        lblAttachmentUser.Name = "lblAttachmentUser";
        lblAttachmentUser.Size = new Size(43, 15);
        lblAttachmentUser.TabIndex = 42;
        lblAttachmentUser.Text = "Usuario:";
        // 
        // txtAttachmentUser
        // 
        txtAttachmentUser.EditValue = "admin";
        txtAttachmentUser.Location = new Point(1173, 174);
        txtAttachmentUser.Name = "txtAttachmentUser";
        txtAttachmentUser.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentUser.Properties.Appearance.Options.UseFont = true;
        txtAttachmentUser.Properties.ReadOnly = true;
        txtAttachmentUser.Size = new Size(140, 22);
        txtAttachmentUser.TabIndex = 43;
        // 
        // lblAttachmentGridTitle
        // 
        lblAttachmentGridTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAttachmentGridTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAttachmentGridTitle.Appearance.Options.UseFont = true;
        lblAttachmentGridTitle.Appearance.Options.UseForeColor = true;
        lblAttachmentGridTitle.Location = new Point(467, 219);
        lblAttachmentGridTitle.Name = "lblAttachmentGridTitle";
        lblAttachmentGridTitle.Size = new Size(155, 20);
        lblAttachmentGridTitle.TabIndex = 44;
        lblAttachmentGridTitle.Text = "3. Archivos registrados";
        // 
        // grdAttachments
        // 
        grdAttachments.DataSource = attachmentsTable;
        grdAttachments.Location = new Point(467, 251);
        grdAttachments.MainView = gvAttachments;
        grdAttachments.Name = "grdAttachments";
        grdAttachments.RepositoryItems.AddRange(new RepositoryItem[] { repoAttachmentCheck });
        grdAttachments.Size = new Size(858, 150);
        grdAttachments.TabIndex = 45;
        grdAttachments.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvAttachments });
        // 
        // gvAttachments
        // 
        gvAttachments.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvAttachments.Appearance.HeaderPanel.Options.UseFont = true;
        gvAttachments.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvAttachments.Appearance.Row.Options.UseFont = true;
        gvAttachments.Columns.AddRange(new GridColumn[] { colAttachmentDocumentType, colAttachmentFileName, colAttachmentDescription, colAttachmentExtension, colAttachmentSize, colAttachmentDate, colAttachmentUser, colAttachmentPrincipal, colAttachmentVisibleSales, colAttachmentVisiblePurchases, colAttachmentStatus });
        gvAttachments.GridControl = grdAttachments;
        gvAttachments.Name = "gvAttachments";
        gvAttachments.OptionsBehavior.Editable = false;
        gvAttachments.OptionsView.ShowGroupPanel = false;
        gvAttachments.OptionsView.ShowIndicator = false;
        // 
        // colAttachmentDocumentType
        // 
        colAttachmentDocumentType.Caption = "Tipo documento";
        colAttachmentDocumentType.FieldName = "TipoDocumento";
        colAttachmentDocumentType.Name = "colAttachmentDocumentType";
        colAttachmentDocumentType.Visible = true;
        colAttachmentDocumentType.VisibleIndex = 0;
        colAttachmentDocumentType.Width = 110;
        // 
        // colAttachmentFileName
        // 
        colAttachmentFileName.Caption = "Nombre archivo";
        colAttachmentFileName.FieldName = "NombreArchivo";
        colAttachmentFileName.Name = "colAttachmentFileName";
        colAttachmentFileName.Visible = true;
        colAttachmentFileName.VisibleIndex = 1;
        colAttachmentFileName.Width = 150;
        // 
        // colAttachmentDescription
        // 
        colAttachmentDescription.Caption = "Descripción";
        colAttachmentDescription.FieldName = "Descripcion";
        colAttachmentDescription.Name = "colAttachmentDescription";
        colAttachmentDescription.Visible = true;
        colAttachmentDescription.VisibleIndex = 2;
        colAttachmentDescription.Width = 130;
        // 
        // colAttachmentExtension
        // 
        colAttachmentExtension.Caption = "Ext.";
        colAttachmentExtension.FieldName = "Extension";
        colAttachmentExtension.Name = "colAttachmentExtension";
        colAttachmentExtension.Visible = true;
        colAttachmentExtension.VisibleIndex = 3;
        colAttachmentExtension.Width = 45;
        // 
        // colAttachmentSize
        // 
        colAttachmentSize.AppearanceCell.Options.UseTextOptions = true;
        colAttachmentSize.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colAttachmentSize.Caption = "Tamaño";
        colAttachmentSize.FieldName = "Tamano";
        colAttachmentSize.Name = "colAttachmentSize";
        colAttachmentSize.Visible = true;
        colAttachmentSize.VisibleIndex = 4;
        colAttachmentSize.Width = 55;
        // 
        // colAttachmentDate
        // 
        colAttachmentDate.Caption = "Fecha";
        colAttachmentDate.FieldName = "Fecha";
        colAttachmentDate.Name = "colAttachmentDate";
        colAttachmentDate.Visible = true;
        colAttachmentDate.VisibleIndex = 5;
        colAttachmentDate.Width = 80;
        // 
        // colAttachmentUser
        // 
        colAttachmentUser.Caption = "Usuario";
        colAttachmentUser.FieldName = "Usuario";
        colAttachmentUser.Name = "colAttachmentUser";
        colAttachmentUser.Visible = true;
        colAttachmentUser.VisibleIndex = 6;
        colAttachmentUser.Width = 70;
        // 
        // colAttachmentPrincipal
        // 
        colAttachmentPrincipal.Caption = "Principal";
        colAttachmentPrincipal.ColumnEdit = repoAttachmentCheck;
        colAttachmentPrincipal.FieldName = "Principal";
        colAttachmentPrincipal.Name = "colAttachmentPrincipal";
        colAttachmentPrincipal.Visible = true;
        colAttachmentPrincipal.VisibleIndex = 7;
        colAttachmentPrincipal.Width = 58;
        // 
        // repoAttachmentCheck
        // 
        repoAttachmentCheck.AutoHeight = false;
        repoAttachmentCheck.Name = "repoAttachmentCheck";
        // 
        // colAttachmentVisibleSales
        // 
        colAttachmentVisibleSales.Caption = "Ventas";
        colAttachmentVisibleSales.ColumnEdit = repoAttachmentCheck;
        colAttachmentVisibleSales.FieldName = "VisibleVentas";
        colAttachmentVisibleSales.Name = "colAttachmentVisibleSales";
        colAttachmentVisibleSales.Visible = true;
        colAttachmentVisibleSales.VisibleIndex = 8;
        colAttachmentVisibleSales.Width = 50;
        // 
        // colAttachmentVisiblePurchases
        // 
        colAttachmentVisiblePurchases.Caption = "Compras";
        colAttachmentVisiblePurchases.ColumnEdit = repoAttachmentCheck;
        colAttachmentVisiblePurchases.FieldName = "VisibleCompras";
        colAttachmentVisiblePurchases.Name = "colAttachmentVisiblePurchases";
        colAttachmentVisiblePurchases.Visible = true;
        colAttachmentVisiblePurchases.VisibleIndex = 9;
        colAttachmentVisiblePurchases.Width = 58;
        // 
        // colAttachmentStatus
        // 
        colAttachmentStatus.Caption = "Estado";
        colAttachmentStatus.FieldName = "Estado";
        colAttachmentStatus.Name = "colAttachmentStatus";
        colAttachmentStatus.Visible = true;
        colAttachmentStatus.VisibleIndex = 10;
        colAttachmentStatus.Width = 70;
        // 
        // btnAddAttachment
        // 
        btnAddAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnAddAttachment.Appearance.Options.UseFont = true;
        btnAddAttachment.Location = new Point(467, 417);
        btnAddAttachment.Name = "btnAddAttachment";
        btnAddAttachment.Size = new Size(78, 28);
        btnAddAttachment.TabIndex = 46;
        btnAddAttachment.Text = "Agregar";
        // 
        // btnUpdateAttachment
        // 
        btnUpdateAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnUpdateAttachment.Appearance.Options.UseFont = true;
        btnUpdateAttachment.Location = new Point(555, 417);
        btnUpdateAttachment.Name = "btnUpdateAttachment";
        btnUpdateAttachment.Size = new Size(86, 28);
        btnUpdateAttachment.TabIndex = 47;
        btnUpdateAttachment.Text = "Actualizar";
        // 
        // btnRemoveAttachment
        // 
        btnRemoveAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnRemoveAttachment.Appearance.Options.UseFont = true;
        btnRemoveAttachment.Location = new Point(651, 417);
        btnRemoveAttachment.Name = "btnRemoveAttachment";
        btnRemoveAttachment.Size = new Size(72, 28);
        btnRemoveAttachment.TabIndex = 48;
        btnRemoveAttachment.Text = "Quitar";
        // 
        // btnDownloadAttachment
        // 
        btnDownloadAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnDownloadAttachment.Appearance.Options.UseFont = true;
        btnDownloadAttachment.Location = new Point(733, 417);
        btnDownloadAttachment.Name = "btnDownloadAttachment";
        btnDownloadAttachment.Size = new Size(86, 28);
        btnDownloadAttachment.TabIndex = 49;
        btnDownloadAttachment.Text = "Descargar";
        // 
        // btnOpenAttachment
        // 
        btnOpenAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnOpenAttachment.Appearance.Options.UseFont = true;
        btnOpenAttachment.Location = new Point(829, 417);
        btnOpenAttachment.Name = "btnOpenAttachment";
        btnOpenAttachment.Size = new Size(70, 28);
        btnOpenAttachment.TabIndex = 50;
        btnOpenAttachment.Text = "Abrir";
        // 
        // btnSetMainAttachment
        // 
        btnSetMainAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnSetMainAttachment.Appearance.Options.UseFont = true;
        btnSetMainAttachment.Location = new Point(909, 417);
        btnSetMainAttachment.Name = "btnSetMainAttachment";
        btnSetMainAttachment.Size = new Size(112, 28);
        btnSetMainAttachment.TabIndex = 51;
        btnSetMainAttachment.Text = "Marcar principal";
        // 
        // ItemEditForm
        // 
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(1420, 828);
        Controls.Add(tabMain);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);
        Font = new Font("Segoe UI", 9F);
        MinimumSize = new Size(1280, 760);
        Name = "ItemEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Maestro de ítems / Artículos";
        ((System.ComponentModel.ISupportInitialize)pnlHeader).EndInit();
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picItem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtItemCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCommercialName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueItemType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueItemGroup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueItemFamily.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBrand.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBaseUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlFooter).EndInit();
        pnlFooter.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)purchasePresentationsTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)warehouseStockTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)purchaseHistoryTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)salesPriceListsTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)costComponentsTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)costPriceHistoryTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)taxMatrixTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)recentLotsTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)variantAttributesTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)registeredVariantsTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)sapCompanySyncTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)sapFieldsTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)sapSyncHistoryTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)attachmentsTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)operationalAlertsTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)allowedLocationsTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)purchasesPresentationsTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)itemSuppliersTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlPresentationBarcodes).EndInit();
        pnlPresentationBarcodes.ResumeLayout(false);
        pnlPresentationBarcodes.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdPresentationBarcodes).EndInit();
        ((System.ComponentModel.ISupportInitialize)presentationBarcodesTable).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvPresentationBarcodes).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoBarcodePrincipal).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoBarcodeActive).EndInit();
        tabRemarks.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlNotesAlerts).EndInit();
        pnlNotesAlerts.ResumeLayout(false);
        pnlNotesAlerts.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdOperationalAlerts).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoOperationalAlertCheck).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvOperationalAlerts).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlNotesProcess).EndInit();
        pnlNotesProcess.ResumeLayout(false);
        pnlNotesProcess.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memLogisticsQualityNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memInventoryNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memSalesNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memPurchaseNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlNotesGeneral).EndInit();
        pnlNotesGeneral.ResumeLayout(false);
        pnlNotesGeneral.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)chkGeneralNoteActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueNotePriority.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memGeneralOperationalAlert.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memGeneralNotes.Properties).EndInit();
        tabAttachments.ResumeLayout(false);
        tabAttachments.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)gridView2).EndInit();
        tabSap.ResumeLayout(false);
        tabSap.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)gridView3).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView4).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView5).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView6).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView7).EndInit();
        tabLots.ResumeLayout(false);
        tabLots.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlLotTraceabilityNote).EndInit();
        pnlLotTraceabilityNote.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)lueNumberingMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBatchFormat.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnQuarantineDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnExpirationAlertDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnShelfLifeDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSerialLength.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBatchPrefix.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAutoGenerateBatch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglExpirationMandatory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresExpiration.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlLotOperationalNote).EndInit();
        pnlLotOperationalNote.ResumeLayout(false);
        pnlLotOperationalNote.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memLotOperationalNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockExpiredBatch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockQuarantineBatch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowExpiredBatchSale.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowMultipleBatches.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueIssueMethod.Properties).EndInit();
        tabTaxes.ResumeLayout(false);
        tabTaxes.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlTaxConfigurationNote).EndInit();
        pnlTaxConfigurationNote.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)tglTaxExemptGoods.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglTaxableService.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglTaxableGoods.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtFiscalCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxSupport.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxesSuggestedWithholding.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueExciseTax.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxesSalesVat.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseVat.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalItemType.Properties).EndInit();
        tabAccounting.ResumeLayout(false);
        tabAccounting.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingRules).EndInit();
        pnlAccountingRules.ResumeLayout(false);
        pnlAccountingRules.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingRulesNote).EndInit();
        pnlAccountingRulesNote.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)memAccountingNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingIntegrationMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnReconciliationDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAccountingBlocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowCompensation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglUseGroupAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglUseWarehouseAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGenerateInventoryJournal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingAccountsNote).EndInit();
        pnlAccountingAccountsNote.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)sluePurchaseExpenseAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseExpenseAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueInventoryAdjustmentAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvInventoryAdjustmentAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueCostVarianceAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvCostVarianceAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluePurchaseReturnAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseReturnAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueSalesReturnAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvSalesReturnAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueCostOfGoodsSoldAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvCostOfGoodsSoldAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueRevenueAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvRevenueAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueInventoryAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvInventoryAccount).EndInit();
        tabCosts.ResumeLayout(false);
        tabCosts.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)spnSimulatorMargin.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSimulatorPrice.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSimulatorCost.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglManualCostUpdate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtCostUpdatedAt.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtCostUpdatedAt.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtPriceUpdatedAt.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtPriceUpdatedAt.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnAverageCost.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlProfitability12m).EndInit();
        pnlProfitability12m.ResumeLayout(false);
        pnlProfitability12m.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlGrossMarginPercent).EndInit();
        pnlGrossMarginPercent.ResumeLayout(false);
        pnlGrossMarginPercent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)spnLastCost.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlGrossMargin).EndInit();
        pnlGrossMargin.ResumeLayout(false);
        pnlGrossMargin.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)spnTargetMarginPercent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnReplacementCost.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumMarginPercent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnStandardCost.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSuggestedPrice.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCostCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnAnalysisBasePrice.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdCostPriceHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView8).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvCostPriceHistory).EndInit();
        tabSales.ResumeLayout(false);
        tabSales.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)spnSalesCommission.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSalesMultiple.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumSale.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumMargin.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMaxDiscount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowSalesDiscount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueMainPriceList.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnBaseSalesPrice.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlSalesKpiCustomers).EndInit();
        pnlSalesKpiCustomers.ResumeLayout(false);
        pnlSalesKpiCustomers.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSalesKpiLastPrice).EndInit();
        pnlSalesKpiLastPrice.ResumeLayout(false);
        pnlSalesKpiLastPrice.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglAffectsPromotions.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlSalesKpi12m).EndInit();
        pnlSalesKpi12m.ResumeLayout(false);
        pnlSalesKpi12m.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSalesKpi30d).EndInit();
        pnlSalesKpi30d.ResumeLayout(false);
        pnlSalesKpi30d.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdSalesPriceLists).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView9).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoSalesPriceListActive).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvSalesPriceLists).EndInit();
        tabPurchases.ResumeLayout(false);
        tabPurchases.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memPurchasePolicy.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseOnDemand.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memReceivingNote.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSupplierBackorderAllowed.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlPurchaseKpiLeadTime).EndInit();
        pnlPurchaseKpiLeadTime.ResumeLayout(false);
        pnlPurchaseKpiLeadTime.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseApprovalRequired.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlPurchaseKpiAverage).EndInit();
        pnlPurchaseKpiAverage.ResumeLayout(false);
        pnlPurchaseKpiAverage.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlPurchaseKpiLast).EndInit();
        pnlPurchaseKpiLast.ResumeLayout(false);
        pnlPurchaseKpiLast.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdPurchaseHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView10).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlPurchaseKpiCompliance).EndInit();
        pnlPurchaseKpiCompliance.ResumeLayout(false);
        pnlPurchaseKpiCompliance.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lookUpEdit1.Properties).EndInit();
        tabInventory.ResumeLayout(false);
        tabInventory.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memInventoryBlockReason.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueInventoryControlType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAbcClassification.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresCycleCount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglManageLocations.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAutoReplenishment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglReplenishmentApproval.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueNegativeStockPolicy.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSuggestedPurchaseQty.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueValuationMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueMainWarehouse.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvMainWarehouse).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalReorderPoint.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memInventoryOperationNote.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockedForMovements.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplyMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalMaxStock.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueReplenishmentMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalMinStock.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnLeadTimeDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCoverageDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdWarehouseStock).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView11).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvWarehouseStock).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueDefaultBinLocation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvDefaultBinLocation).EndInit();
        tabUnits.ResumeLayout(false);
        tabUnits.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueVolumeUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueWeightUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnVolume.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnGrossWeight.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCodeOrigin.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnNetWeight.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtTariffCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtUnspscCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtManufacturerReference.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueInventoryUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPreviousInternalCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPlu.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdPurchasePresentations).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView12).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoPurchasePrincipal).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoPurchaseActive).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchasePresentations).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtQrCode.Properties).EndInit();
        tabGeneral.ResumeLayout(false);
        tabGeneral.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSalesActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtReference.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtModel.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSubGroup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueLine.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueOrigin.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueProductType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memLongDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueSupplierSku.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvSupplierSku).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAlternateCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAffectsInventory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralExpirationManaged.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralBatchManaged.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralSerialManaged.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralPerishable.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralAllowDiscount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralRequiresScale.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralMobileItem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlKpiVariants).EndInit();
        pnlKpiVariants.ResumeLayout(false);
        pnlKpiVariants.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlKpiSap).EndInit();
        pnlKpiSap.ResumeLayout(false);
        pnlKpiSap.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlKpiSales).EndInit();
        pnlKpiSales.ResumeLayout(false);
        pnlKpiSales.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlKpiPurchases).EndInit();
        pnlKpiPurchases.ResumeLayout(false);
        pnlKpiPurchases.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlKpiOrders).EndInit();
        pnlKpiOrders.ResumeLayout(false);
        pnlKpiOrders.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlKpiStock).EndInit();
        pnlKpiStock.ResumeLayout(false);
        pnlKpiStock.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglBlockedEcommerce.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tabMain).EndInit();
        tabMain.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grdSapFieldMapping).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSapFieldMapping).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapMapEnabled.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapMapRequired.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapSapField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapSystemField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdSapSyncHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSapSyncHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapSyncAsSupplier.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapMode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapManualRetry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapRequiresApproval.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapCompany.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSapCompanyLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapSyncStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastSync.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastError.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapRetryCount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapEnabled.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)picMainAttachmentPreview.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentPreviewNote).EndInit();
        pnlAttachmentPreviewNote.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)lueAttachmentType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentFileName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memAttachmentDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentCategory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInSales.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInPurchases.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInPortal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentExtension.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentSize.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentUploadedAt.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentUploadedAt.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentUser.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoAttachmentCheck).EndInit();
        ResumeLayout(false);
    }
    private LabelControl lblStockTotalCaption;
    private LabelControl lblStockTotal;
    private LabelControl lblAverageCostCaption;
    private LabelControl lblAverageCost;
    private LabelControl lblSalesPriceCaption;
    private LabelControl lblSalesPrice;
    private LabelControl lblLastPurchaseCaption;
    private LabelControl lblLastPurchase;
    private LabelControl lblSapSyncedCaption;
    private LabelControl lblSapSynced;
    private PanelControl pnlPresentationBarcodes;
    private LabelControl lblPresentationBarcodesTitle;
    private GridControl grdPresentationBarcodes;
    private GridView gvPresentationBarcodes;
    private GridColumn colBarcodeValue;
    private GridColumn colBarcodeScope;
    private GridColumn colBarcodePresentation;
    private GridColumn colBarcodeUnit;
    private GridColumn colBarcodeFactor;
    private GridColumn colBarcodePrincipal;
    private GridColumn colBarcodeActive;
    private RepositoryItemCheckEdit repoBarcodePrincipal;
    private RepositoryItemCheckEdit repoBarcodeActive;
    private SimpleButton btnAddBarcode;
    private SimpleButton btnUpdateBarcode;
    private SimpleButton btnRemoveBarcode;
    private SimpleButton btnSetMainBarcode;
    private System.Data.DataTable purchasePresentationsTable;
    private System.Data.DataTable warehouseStockTable;
    private System.Data.DataTable allowedLocationsTable;
    private System.Data.DataTable purchasesPresentationsTable;
    private System.Data.DataTable itemSuppliersTable;
    private System.Data.DataTable purchaseHistoryTable;
    private System.Data.DataTable salesPriceListsTable;
    private System.Data.DataTable costComponentsTable;
    private System.Data.DataTable costPriceHistoryTable;
    private System.Data.DataTable taxMatrixTable;
    private System.Data.DataTable recentLotsTable;
    private System.Data.DataTable variantAttributesTable;
    private System.Data.DataTable registeredVariantsTable;
    private System.Data.DataTable sapCompanySyncTable;
    private System.Data.DataTable sapFieldsTable;
    private System.Data.DataTable sapSyncHistoryTable;
    private System.Data.DataTable attachmentsTable;
    private System.Data.DataTable operationalAlertsTable;
    private System.Data.DataTable presentationBarcodesTable;
    private XtraTabPage tabRemarks;
    private PanelControl pnlNotesGeneral;
    private LabelControl lblNotesGeneralTitle;
    private LabelControl lblGeneralNotes;
    private MemoEdit memGeneralNotes;
    private LabelControl lblGeneralOperationalAlert;
    private MemoEdit memGeneralOperationalAlert;
    private LabelControl lblNotePriority;
    private LookUpEdit lueNotePriority;
    private CheckEdit chkGeneralNoteActive;
    private PanelControl pnlNotesProcess;
    private LabelControl lblNotesProcessTitle;
    private LabelControl lblPurchaseNotes;
    private MemoEdit memPurchaseNotes;
    private LabelControl lblSalesNotes;
    private MemoEdit memSalesNotes;
    private LabelControl lblInventoryNotes;
    private MemoEdit memInventoryNotes;
    private LabelControl lblLogisticsQualityNotes;
    private MemoEdit memLogisticsQualityNotes;
    private PanelControl pnlNotesAlerts;
    private LabelControl lblNotesAlertsTitle;
    private GridControl grdOperationalAlerts;
    private GridView gvOperationalAlerts;
    private GridColumn colOperationalAlertType;
    private GridColumn colOperationalAlertProcess;
    private GridColumn colOperationalAlertMessage;
    private GridColumn colOperationalAlertFrom;
    private GridColumn colOperationalAlertTo;
    private GridColumn colOperationalAlertBlocking;
    private RepositoryItemCheckEdit repoOperationalAlertCheck;
    private GridColumn colOperationalAlertActive;
    private SimpleButton btnAddOperationalAlert;
    private SimpleButton btnUpdateOperationalAlert;
    private SimpleButton btnRemoveOperationalAlert;
    private SimpleButton btnClearOperationalAlert;
    private GridView gridView1;
    private XtraTabPage tabAttachments;
    private GridView gridView2;
    private XtraTabPage tabSap;
    private GridView gridView3;
    private GridView gridView4;
    private GridView gridView5;
    private GridView gridView6;
    private GridView gridView7;
    private XtraTabPage tabLots;
    private LabelControl lblLotOperationalRulesTitle;
    private LabelControl lblIssueMethod;
    private LookUpEdit lueIssueMethod;
    private LabelControl lblAllowMultipleBatches;
    private ToggleSwitch tglAllowMultipleBatches;
    private LabelControl lblAllowExpiredBatchSale;
    private ToggleSwitch tglAllowExpiredBatchSale;
    private LabelControl lblBlockQuarantineBatch;
    private ToggleSwitch tglBlockQuarantineBatch;
    private LabelControl lblBlockExpiredBatch;
    private ToggleSwitch tglBlockExpiredBatch;
    private LabelControl lblLotOperationalNotes;
    private MemoEdit memLotOperationalNotes;
    private PanelControl pnlLotOperationalNote;
    private LabelControl lblLotOperationalNoteIcon;
    private LabelControl lblLotOperationalNote;
    private LabelControl lblLotTraceabilityTitle;
    private LabelControl lblRequiresExpiration;
    private ToggleSwitch tglRequiresExpiration;
    private LabelControl lblExpirationMandatory;
    private ToggleSwitch tglExpirationMandatory;
    private LabelControl lblAutoGenerateBatch;
    private ToggleSwitch tglAutoGenerateBatch;
    private LabelControl lblBatchPrefix;
    private TextEdit txtBatchPrefix;
    private LabelControl lblSerialLength;
    private SpinEdit spnSerialLength;
    private LabelControl lblShelfLifeDays;
    private SpinEdit spnShelfLifeDays;
    private LabelControl lblExpirationAlertDays;
    private SpinEdit spnExpirationAlertDays;
    private LabelControl lblQuarantineDays;
    private SpinEdit spnQuarantineDays;
    private LabelControl lblBatchFormat;
    private TextEdit txtBatchFormat;
    private LabelControl lblNumberingMethod;
    private LookUpEdit lueNumberingMethod;
    private PanelControl pnlLotTraceabilityNote;
    private LabelControl lblLotTraceabilityNoteIcon;
    private LabelControl lblLotTraceabilityNote;
    private XtraTabPage tabTaxes;
    private LabelControl lblTaxConfigurationTitle;
    private LabelControl lblFiscalItemType;
    private LookUpEdit lueFiscalItemType;
    private LabelControl lblPurchaseVat;
    private LookUpEdit luePurchaseVat;
    private LabelControl lblTaxesSalesVat;
    private LookUpEdit lueTaxesSalesVat;
    private LabelControl lblExciseTax;
    private LookUpEdit lueExciseTax;
    private LabelControl lblTaxesSuggestedWithholding;
    private LookUpEdit lueTaxesSuggestedWithholding;
    private LabelControl lblTaxSupport;
    private LookUpEdit lueTaxSupport;
    private LabelControl lblFiscalCode;
    private TextEdit txtFiscalCode;
    private LabelControl lblFiscalCountry;
    private LookUpEdit lueFiscalCountry;
    private LabelControl lblTaxableGoods;
    private ToggleSwitch tglTaxableGoods;
    private LabelControl lblTaxableService;
    private ToggleSwitch tglTaxableService;
    private LabelControl lblTaxExemptGoods;
    private ToggleSwitch tglTaxExemptGoods;
    private PanelControl pnlTaxConfigurationNote;
    private LabelControl lblTaxConfigurationNoteIcon;
    private LabelControl lblTaxConfigurationNote;
    private XtraTabPage tabAccounting;
    private LabelControl lblAccountingAccountsTitle;
    private LabelControl lblAccountingInventoryAccount;
    private SearchLookUpEdit slueInventoryAccount;
    private GridView gvInventoryAccount;
    private LabelControl lblAccountingRevenueAccount;
    private SearchLookUpEdit slueRevenueAccount;
    private GridView gvRevenueAccount;
    private LabelControl lblCostOfGoodsSoldAccount;
    private SearchLookUpEdit slueCostOfGoodsSoldAccount;
    private GridView gvCostOfGoodsSoldAccount;
    private LabelControl lblSalesReturnAccount;
    private SearchLookUpEdit slueSalesReturnAccount;
    private GridView gvSalesReturnAccount;
    private LabelControl lblPurchaseReturnAccount;
    private SearchLookUpEdit sluePurchaseReturnAccount;
    private GridView gvPurchaseReturnAccount;
    private LabelControl lblCostVarianceAccount;
    private SearchLookUpEdit slueCostVarianceAccount;
    private GridView gvCostVarianceAccount;
    private LabelControl lblInventoryAdjustmentAccount;
    private SearchLookUpEdit slueInventoryAdjustmentAccount;
    private GridView gvInventoryAdjustmentAccount;
    private LabelControl lblPurchaseExpenseAccount;
    private SearchLookUpEdit sluePurchaseExpenseAccount;
    private GridView gvPurchaseExpenseAccount;
    private PanelControl pnlAccountingAccountsNote;
    private LabelControl lblAccountingAccountsNoteIcon;
    private LabelControl lblAccountingAccountsNote;
    private PanelControl pnlAccountingRules;
    private LabelControl lblAccountingRulesTitle;
    private LabelControl lblGenerateInventoryJournal;
    private ToggleSwitch tglGenerateInventoryJournal;
    private LabelControl lblUseWarehouseAccount;
    private ToggleSwitch tglUseWarehouseAccount;
    private LabelControl lblUseGroupAccount;
    private ToggleSwitch tglUseGroupAccount;
    private LabelControl lblAllowCompensation;
    private ToggleSwitch tglAllowCompensation;
    private LabelControl lblAccountingBlocked;
    private ToggleSwitch tglAccountingBlocked;
    private LabelControl lblReconciliationDays;
    private SpinEdit spnReconciliationDays;
    private LabelControl lblAccountingIntegrationMethod;
    private LookUpEdit lueAccountingIntegrationMethod;
    private LabelControl lblAccountingNotes;
    private MemoEdit memAccountingNotes;
    private PanelControl pnlAccountingRulesNote;
    private LabelControl lblAccountingRulesNoteIcon;
    private LabelControl lblAccountingRulesNote;
    private XtraTabPage tabCosts;
    private LabelControl lblCostPriceHistoryTitle;
    private GridControl grdCostPriceHistory;
    private GridView gvCostPriceHistory;
    private GridColumn colCostHistoryDate;
    private GridColumn colCostHistoryMovement;
    private GridColumn colCostHistoryDocument;
    private GridColumn colCostHistoryPreviousCost;
    private GridColumn colCostHistoryNewCost;
    private GridColumn colCostHistoryPreviousPrice;
    private GridColumn colCostHistoryNewPrice;
    private GridColumn colCostHistoryVariation;
    private GridColumn colCostHistoryUser;
    private GridColumn colCostHistoryObservation;
    private LabelControl lblPricesMarginsTitle;
    private LabelControl lblAnalysisBasePrice;
    private LabelControl lblCostsBaseTitle;
    private SpinEdit spnAnalysisBasePrice;
    private LabelControl lblCostCurrency;
    private LabelControl lblSuggestedPrice;
    private LookUpEdit lueCostCurrency;
    private SpinEdit spnSuggestedPrice;
    private LabelControl lblStandardCost;
    private LabelControl lblMinimumMarginPercent;
    private SpinEdit spnStandardCost;
    private SpinEdit spnMinimumMarginPercent;
    private LabelControl lblReplacementCost;
    private LabelControl lblTargetMarginPercent;
    private SpinEdit spnReplacementCost;
    private SpinEdit spnTargetMarginPercent;
    private LabelControl lblLastCost;
    private PanelControl pnlGrossMargin;
    private LabelControl lblGrossMarginCaption;
    private LabelControl lblGrossMarginValue;
    private LabelControl lblGrossMarginUnit;
    private SpinEdit spnLastCost;
    private PanelControl pnlGrossMarginPercent;
    private LabelControl lblGrossMarginPercentCaption;
    private LabelControl lblGrossMarginPercentValue;
    private LabelControl lblCostsAverageCost;
    private PanelControl pnlProfitability12m;
    private LabelControl lblProfitability12mCaption;
    private LabelControl lblProfitability12mValue;
    private SpinEdit spnAverageCost;
    private LabelControl lblPriceUpdatedAt;
    private LabelControl lblCostUpdatedAt;
    private DateEdit dtPriceUpdatedAt;
    private DateEdit dtCostUpdatedAt;
    private LabelControl lblSimulatorTitle;
    private LabelControl lblManualCostUpdate;
    private LabelControl lblSimulatorCost;
    private ToggleSwitch tglManualCostUpdate;
    private SpinEdit spnSimulatorCost;
    private LabelControl lblSimulatorPlus;
    private LabelControl lblSimulatorMargin;
    private SpinEdit spnSimulatorPrice;
    private SpinEdit spnSimulatorMargin;
    private LabelControl lblSimulatorPrice;
    private LabelControl lblSimulatorEquals;
    private GridView gridView8;
    private XtraTabPage tabSales;
    private LabelControl lblSalesPricePerformanceTitle;
    private GridControl grdSalesPriceLists;
    private GridView gvSalesPriceLists;
    private GridColumn colSalesPriceListName;
    private GridColumn colSalesPriceListCurrency;
    private GridColumn colSalesPriceListPrice;
    private GridColumn colSalesPriceListMargin;
    private GridColumn colSalesPriceListValidFrom;
    private GridColumn colSalesPriceListActive;
    private RepositoryItemCheckEdit repoSalesPriceListActive;
    private LabelControl lblSalesConfigurationTitle;
    private PanelControl pnlSalesKpi30d;
    private LabelControl lblSalesKpi30dCaption;
    private LabelControl lblSalesKpi30dValue;
    private LabelControl lblAffectsPromotions;
    private PanelControl pnlSalesKpi12m;
    private LabelControl lblSalesKpi12mCaption;
    private LabelControl lblSalesKpi12mValue;
    private ToggleSwitch tglAffectsPromotions;
    private PanelControl pnlSalesKpiLastPrice;
    private LabelControl lblSalesKpiLastPriceCaption;
    private LabelControl lblSalesKpiLastPriceValue;
    private LabelControl lblSalesUnit;
    private PanelControl pnlSalesKpiCustomers;
    private LabelControl lblSalesKpiCustomersCaption;
    private LabelControl lblSalesKpiCustomersValue;
    private LookUpEdit lueSalesUnit;
    private LabelControl lblBaseSalesPrice;
    private SpinEdit spnBaseSalesPrice;
    private LookUpEdit lueSalesCurrency;
    private LabelControl lblMainPriceList;
    private LookUpEdit lueMainPriceList;
    private LabelControl lblAllowSalesDiscount;
    private ToggleSwitch tglAllowSalesDiscount;
    private LabelControl lblMaxDiscount;
    private SpinEdit spnMaxDiscount;
    private LabelControl lblMinimumMargin;
    private SpinEdit spnMinimumMargin;
    private LabelControl lblMinimumSale;
    private SpinEdit spnMinimumSale;
    private LabelControl lblMinimumSaleUnit;
    private LabelControl lblSalesMultiple;
    private SpinEdit spnSalesMultiple;
    private LabelControl lblSalesMultipleUnit;
    private LabelControl lblSalesCommission;
    private SpinEdit spnSalesCommission;
    private GridView gridView9;
    private XtraTabPage tabPurchases;
    private LabelControl labelControl1;
    private LookUpEdit lookUpEdit1;
    private PanelControl pnlPurchaseKpiCompliance;
    private LabelControl lblPurchaseKpiComplianceCaption;
    private LabelControl lblPurchaseKpiComplianceValue;
    private LabelControl lblPurchasesHistoryTitle;
    private GridControl grdPurchaseHistory;
    private GridView gvPurchaseHistory;
    private GridColumn colPurchaseHistoryDate;
    private GridColumn colPurchaseHistoryDocument;
    private GridColumn colPurchaseHistorySupplier;
    private GridColumn colPurchaseHistoryPresentation;
    private GridColumn colPurchaseHistoryQuantity;
    private GridColumn colPurchaseHistoryUnit;
    private GridColumn colPurchaseHistoryInventoryQty;
    private GridColumn colPurchaseHistoryUnitCost;
    private GridColumn colPurchaseHistoryCurrency;
    private GridColumn colPurchaseHistoryStatus;
    private LabelControl lblPurchasesConfigurationTitle;
    private PanelControl pnlPurchaseKpiLast;
    private LabelControl lblPurchaseKpiLastCaption;
    private LabelControl lblPurchaseKpiLastValue;
    private LabelControl lblPurchaseApprovalRequired;
    private PanelControl pnlPurchaseKpiAverage;
    private LabelControl lblPurchaseKpiAverageCaption;
    private LabelControl lblPurchaseKpiAverageValue;
    private ToggleSwitch tglPurchaseApprovalRequired;
    private PanelControl pnlPurchaseKpiLeadTime;
    private LabelControl lblPurchaseKpiLeadTimeCaption;
    private LabelControl lblPurchaseKpiLeadTimeValue;
    private LabelControl lblSupplierBackorderAllowed;
    private ToggleSwitch tglSupplierBackorderAllowed;
    private MemoEdit memReceivingNote;
    private LabelControl lblPurchaseOnDemand;
    private LabelControl lblReceivingNote;
    private ToggleSwitch tglPurchaseOnDemand;
    private MemoEdit memPurchasePolicy;
    private LabelControl lblPurchasePolicy;
    private GridView gridView10;
    private XtraTabPage tabInventory;
    private SimpleButton btnAddWarehouseStock;
    private SimpleButton btnUpdateWarehouseStock;
    private SimpleButton btnRemoveWarehouseStock;
    private SimpleButton btnSetMainWarehouseStock;
    private LabelControl lblInventoryLocationsRestrictionsTitle;
    private LabelControl lblDefaultBinLocation;
    private LabelControl lblStockByWarehouseTitle;
    private SearchLookUpEdit slueDefaultBinLocation;
    private GridView gvDefaultBinLocation;
    private GridControl grdWarehouseStock;
    private GridView gvWarehouseStock;
    private GridColumn colWarehouseCode;
    private GridColumn colWarehouseName;
    private GridColumn colWarehouseStockActual;
    private GridColumn colWarehouseCommitted;
    private GridColumn colWarehouseOrdered;
    private GridColumn colWarehouseAvailable;
    private GridColumn colWarehouseMinimum;
    private GridColumn colWarehouseMaximum;
    private GridColumn colWarehouseReorder;
    private GridColumn colWarehouseStatus;
    private LabelControl lblCoverageDays;
    private SpinEdit spnCoverageDays;
    private LabelControl lblReplenishmentOperationTitle;
    private LabelControl lblLeadTimeDays;
    private LabelControl lblInventoryParametersTitle;
    private SpinEdit spnLeadTimeDays;
    private LabelControl lblReplenishmentMethod;
    private LabelControl lblGlobalMinStock;
    private LabelControl lblSupplyMethod;
    private SpinEdit spnGlobalMinStock;
    private LookUpEdit lueReplenishmentMethod;
    private LabelControl lblGlobalMaxStock;
    private LabelControl lblMainWarehouse;
    private SpinEdit spnGlobalMaxStock;
    private LabelControl lblBlockedForMovements;
    private LookUpEdit lueSupplyMethod;
    private ToggleSwitch tglBlockedForMovements;
    private LabelControl lblGlobalReorderPoint;
    private LabelControl lblInventoryOperationNote;
    private LabelControl lblValuationMethod;
    private MemoEdit memInventoryOperationNote;
    private SpinEdit spnGlobalReorderPoint;
    private SearchLookUpEdit slueMainWarehouse;
    private GridView gvMainWarehouse;
    private LabelControl lblSuggestedPurchaseQty;
    private LookUpEdit lueValuationMethod;
    private SpinEdit spnSuggestedPurchaseQty;
    private LabelControl lblNegativeStockPolicy;
    private LabelControl lblReplenishmentApproval;
    private LookUpEdit lueNegativeStockPolicy;
    private ToggleSwitch tglReplenishmentApproval;
    private LabelControl lblAutoReplenishment;
    private ToggleSwitch tglAutoReplenishment;
    private LabelControl lblManageLocations;
    private ToggleSwitch tglManageLocations;
    private LabelControl lblRequiresCycleCount;
    private ToggleSwitch tglRequiresCycleCount;
    private LabelControl lblAbcClassification;
    private LookUpEdit lueAbcClassification;
    private LabelControl lblInventoryControlType;
    private LookUpEdit lueInventoryControlType;
    private LabelControl lblInventoryBlockReason;
    private MemoEdit memInventoryBlockReason;
    private GridView gridView11;
    private XtraTabPage tabUnits;
    private LabelControl lblCodesIdentifiersTitle;
    private LabelControl lblQrCode;
    private LabelControl lblPurchasePresentationsTitle;
    private TextEdit txtQrCode;
    private GridControl grdPurchasePresentations;
    private GridView gvPurchasePresentations;
    private GridColumn colPurchasePresentation;
    private GridColumn colPurchaseUnit;
    private GridColumn colPurchaseFactor;
    private GridColumn colPurchaseBarcode;
    private GridColumn colPurchaseEnabled;
    private RepositoryItemCheckEdit repoPurchaseActive;
    private GridColumn colSalesEnabled;
    private GridColumn colPurchasePrincipal;
    private RepositoryItemCheckEdit repoPurchasePrincipal;
    private GridColumn colSalesPrincipal;
    private GridColumn colPurchaseActive;
    private LabelControl lblPlu;
    private LabelControl lblInventoryUnitTitle;
    private TextEdit txtPlu;
    private SimpleButton btnAddPurchasePresentation;
    private LabelControl lblPreviousInternalCode;
    private LabelControl lblInventoryUnit;
    private TextEdit txtPreviousInternalCode;
    private SimpleButton btnUpdatePurchasePresentation;
    private LabelControl lblManufacturerReference;
    private LookUpEdit lueInventoryUnit;
    private TextEdit txtManufacturerReference;
    private SimpleButton btnRemovePurchasePresentation;
    private LabelControl lblUnspscCode;
    private TextEdit txtUnspscCode;
    private SimpleButton btnSetMainPurchasePresentation;
    private LabelControl lblTariffCode;
    private TextEdit txtTariffCode;
    private LabelControl lblNetWeight;
    private LabelControl lblCodeOrigin;
    private SpinEdit spnNetWeight;
    private LookUpEdit lueCodeOrigin;
    private LabelControl lblNetWeightUnit;
    private LabelControl lblGrossWeight;
    private SpinEdit spnGrossWeight;
    private LabelControl lblGrossWeightUnit;
    private LabelControl lblVolume;
    private SpinEdit spnVolume;
    private LabelControl lblVolumeUnitCaption;
    private LabelControl lblWeightUnit;
    private LookUpEdit lueWeightUnit;
    private LabelControl lblVolumeUnit;
    private LookUpEdit lueVolumeUnit;
    private GridView gridView12;
    private XtraTabPage tabGeneral;
    private LabelControl lblBlockedEcommerce;
    private ToggleSwitch tglBlockedEcommerce;
    private LabelControl lblGeneralSummaryTitle;
    private PanelControl pnlKpiStock;
    private LabelControl lblKpiStockCaption;
    private LabelControl lblKpiStockValue;
    private LabelControl lblKpiStockUnit;
    private PanelControl pnlKpiOrders;
    private LabelControl lblKpiOrdersCaption;
    private LabelControl lblKpiOrdersValue;
    private LabelControl lblKpiOrdersUnit;
    private PanelControl pnlKpiPurchases;
    private LabelControl lblKpiPurchasesCaption;
    private LabelControl lblKpiPurchasesValue;
    private LabelControl lblKpiPurchasesUnit;
    private PanelControl pnlKpiSales;
    private LabelControl lblKpiSalesCaption;
    private LabelControl lblKpiSalesValue;
    private LabelControl lblKpiSalesUnit;
    private PanelControl pnlKpiSap;
    private LabelControl lblKpiSapCaption;
    private LabelControl lblKpiSapValue;
    private PanelControl pnlKpiVariants;
    private LabelControl lblKpiVariantsCaption;
    private LabelControl lblKpiVariantsValue;
    private ToggleSwitch tglGeneralMobileItem;
    private LabelControl lblGeneralMobileItem;
    private ToggleSwitch tglGeneralRequiresScale;
    private ToggleSwitch tglGeneralAllowDiscount;
    private ToggleSwitch tglGeneralPerishable;
    private ToggleSwitch tglGeneralSerialManaged;
    private ToggleSwitch tglGeneralBatchManaged;
    private LabelControl lblGeneralOperationTitle;
    private LabelControl lblBatchManaged;
    private LabelControl lblSerialManaged;
    private LabelControl lblPerishable;
    private LabelControl lblExpirationManaged;
    private LabelControl lblRequiresScale;
    private LabelControl lblAllowDiscount;
    private ToggleSwitch tglGeneralExpirationManaged;
    private LabelControl lblAffectsInventory;
    private ToggleSwitch tglAffectsInventory;
    private LabelControl lblGeneralIdentificationTitle;
    private LabelControl lblAlternateCode;
    private TextEdit txtAlternateCode;
    private LabelControl lblSupplierSku;
    private SearchLookUpEdit slueSupplierSku;
    private GridView gvSupplierSku;
    private LabelControl lblLongDescription;
    private MemoEdit memLongDescription;
    private LabelControl lblProductType;
    private LookUpEdit lueProductType;
    private LabelControl lblOrigin;
    private LookUpEdit lueOrigin;
    private LabelControl lblLine;
    private LookUpEdit lueLine;
    private LabelControl lblSubGroup;
    private LookUpEdit lueSubGroup;
    private LabelControl lblModel;
    private TextEdit txtModel;
    private LabelControl lblReference;
    private TextEdit txtReference;
    private LabelControl lblSalesActive;
    private ToggleSwitch tglSalesActive;
    private LabelControl lblPurchaseActive;
    private ToggleSwitch tglPurchaseActive;
    private XtraTabControl tabMain;
    private SimpleButton btnClearSapFields;
    private SimpleButton btnRemoveSapField;
    private SimpleButton btnUpdateSapField;
    private SimpleButton btnAddSapField;
    private GridControl grdSapFieldMapping;
    private GridView grvSapFieldMapping;
    private GridColumn colSapMapSystemField;
    private GridColumn colSapMapSapField;
    private GridColumn colSapMapDescription;
    private GridColumn colSapMapRequired;
    private GridColumn colSapMapEnabled;
    private LabelControl lblSapFieldMappingTitle;
    private LabelControl lblSapMapEnabled;
    private LabelControl lblSapMapRequired;
    private LookUpEdit lueSapMapEnabled;
    private LabelControl lblSapMapDescription;
    private LookUpEdit lueSapMapRequired;
    private LabelControl lblSapMapSapField;
    private TextEdit txtSapMapDescription;
    private LabelControl lblSapMapSystemField;
    private TextEdit txtSapMapSapField;
    private LabelControl lblSapHistoryTitle;
    private TextEdit txtSapMapSystemField;
    private GridControl grdSapSyncHistory;
    private GridView grvSapSyncHistory;
    private GridColumn colSapHistoryDate;
    private GridColumn colSapHistoryOperation;
    private GridColumn colSapHistoryStatus;
    private GridColumn colSapHistoryDocEntry;
    private GridColumn colSapHistoryDocNum;
    private GridColumn colSapHistoryRetryCount;
    private GridColumn colSapHistoryMessage;
    private LabelControl lblSapSyncAsSupplier;
    private LabelControl lblSapMode;
    private LookUpEdit lueSapSyncAsSupplier;
    private LabelControl lblSapConfigTitle;
    private LabelControl lblSapManualRetry;
    private LookUpEdit lueSapMode;
    private LookUpEdit lueSapManualRetry;
    private LabelControl lblSapCompany;
    private LabelControl lblSapRequiresApproval;
    private LabelControl lblSapStatusTitle;
    private LookUpEdit lueSapRequiresApproval;
    private SearchLookUpEdit lueSapCompany;
    private GridView grvSapCompanyLookup;
    private LabelControl lblSapSyncStatus;
    private LookUpEdit lueSapSyncStatus;
    private LabelControl lblSapLastSync;
    private TextEdit txtSapLastSync;
    private LabelControl lblSapLastError;
    private TextEdit txtSapLastError;
    private LabelControl lblSapRetryCount;
    private TextEdit txtSapRetryCount;
    private LabelControl lblSapEnabled;
    private LookUpEdit lueSapEnabled;
    private LabelControl lblAttachmentGridTitle;
    private GridControl grdAttachments;
    private GridView gvAttachments;
    private GridColumn colAttachmentDocumentType;
    private GridColumn colAttachmentFileName;
    private GridColumn colAttachmentDescription;
    private GridColumn colAttachmentExtension;
    private GridColumn colAttachmentSize;
    private GridColumn colAttachmentDate;
    private GridColumn colAttachmentUser;
    private GridColumn colAttachmentPrincipal;
    private RepositoryItemCheckEdit repoAttachmentCheck;
    private GridColumn colAttachmentVisibleSales;
    private GridColumn colAttachmentVisiblePurchases;
    private GridColumn colAttachmentStatus;
    private SimpleButton btnAddAttachment;
    private SimpleButton btnUpdateAttachment;
    private SimpleButton btnRemoveAttachment;
    private SimpleButton btnDownloadAttachment;
    private SimpleButton btnOpenAttachment;
    private SimpleButton btnSetMainAttachment;
    private LabelControl lblAttachmentMetadataTitle;
    private LabelControl lblAttachmentType;
    private LookUpEdit lueAttachmentType;
    private LabelControl lblAttachmentFileName;
    private TextEdit txtAttachmentFileName;
    private LabelControl lblAttachmentDescription;
    private MemoEdit memAttachmentDescription;
    private LabelControl lblAttachmentCategory;
    private LookUpEdit lueAttachmentCategory;
    private CheckEdit chkVisibleInSales;
    private CheckEdit chkVisibleInPurchases;
    private CheckEdit chkVisibleInPortal;
    private LabelControl lblAttachmentStatus;
    private LookUpEdit lueAttachmentStatus;
    private LabelControl lblAttachmentExtension;
    private TextEdit txtAttachmentExtension;
    private LabelControl lblAttachmentSize;
    private TextEdit txtAttachmentSize;
    private LabelControl lblAttachmentUploadedAt;
    private DateEdit dteAttachmentUploadedAt;
    private LabelControl lblAttachmentUser;
    private TextEdit txtAttachmentUser;
    private LabelControl lblAttachmentPreviewTitle;
    private PictureEdit picMainAttachmentPreview;
    private SimpleButton btnLoadImage;
    private SimpleButton btnRemoveImage;
    private SimpleButton btnPreviewImage;
    private SimpleButton btnSetMainImage;
    private PanelControl pnlAttachmentPreviewNote;
    private LabelControl lblAttachmentPreviewNoteIcon;
    private LabelControl lblAttachmentPreviewNote;
}

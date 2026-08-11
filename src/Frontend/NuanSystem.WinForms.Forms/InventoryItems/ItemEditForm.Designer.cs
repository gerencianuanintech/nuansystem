using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Controls.Kpi;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.InventoryItems;

partial class ItemEditForm
{
    private System.ComponentModel.IContainer components = null;
    private LabelControl lblHeaderDataTitle;
    private LabelControl lblHeaderClassificationTitle;
    private LabelControl lblHeaderCommercialSummaryTitle;
    private LabelControl sepHeaderClassification;
    private LabelControl sepHeaderCommercialSummary;
    private LabelControl lblHeaderStockCaption;
    private LabelControl lblHeaderStockValue;
    private LabelControl lblHeaderAverageCostCaption;
    private LabelControl lblHeaderAverageCostValue;
    private LabelControl lblHeaderSalesPriceCaption;
    private LabelControl lblHeaderSalesPriceValue;
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
    private LabelControl lblUnsavedIndicator;
    private LabelControl lblValidationIndicator;
    private TextEdit txtItemCode;
    private TextEdit txtDescription;
    private TextEdit txtCommercialName;
    private LookUpEdit lueItemType;
    private LookUpEdit lueItemGroup;
    private LookUpEdit lueItemFamily;
    private LookUpEdit lueBrand;
    private LookUpEdit lueBaseUnit;
    private LabelControl lblStatus;

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
        lblHeaderDataTitle = new LabelControl();
        lblHeaderClassificationTitle = new LabelControl();
        lblHeaderCommercialSummaryTitle = new LabelControl();
        sepHeaderClassification = new LabelControl();
        sepHeaderCommercialSummary = new LabelControl();
        lblHeaderStockCaption = new LabelControl();
        lblHeaderStockValue = new LabelControl();
        lblHeaderAverageCostCaption = new LabelControl();
        lblHeaderAverageCostValue = new LabelControl();
        lblHeaderSalesPriceCaption = new LabelControl();
        lblHeaderSalesPriceValue = new LabelControl();
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
        lblUnsavedIndicator = new LabelControl();
        lblValidationIndicator = new LabelControl();
        itemPresentationsTable = new System.Data.DataTable();
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
        lblNotesGeneralTitle = new LabelControl();
        lblGeneralNotes = new LabelControl();
        memGeneralNotes = new MemoEdit();
        lblGeneralOperationalAlert = new LabelControl();
        memGeneralOperationalAlert = new MemoEdit();
        lblNotePriority = new LabelControl();
        lueNotePriority = new LookUpEdit();
        lblNoteVisibility = new LabelControl();
        lueNoteVisibility = new LookUpEdit();
        lblGeneralNoteActive = new LabelControl();
        chkGeneralNoteActive = new ToggleSwitch();
        sepNotesGeneralTitle = new LabelControl();
        lblNotesProcessTitle = new LabelControl();
        lblPurchaseNotes = new LabelControl();
        memPurchaseNotes = new MemoEdit();
        lblSalesNotes = new LabelControl();
        memSalesNotes = new MemoEdit();
        lblInventoryNotes = new LabelControl();
        memInventoryNotes = new MemoEdit();
        lblLogisticsQualityNotes = new LabelControl();
        memLogisticsQualityNotes = new MemoEdit();
        sepNotesProcessTitle = new LabelControl();
        lblNotesAlertsTitle = new LabelControl();
        grdOperationalAlerts = new GridControl();
        gvOperationalAlerts = new GridView();
        colOperationalAlertType = new GridColumn();
        colOperationalAlertProcess = new GridColumn();
        colOperationalAlertMessage = new GridColumn();
        colOperationalAlertFrom = new GridColumn();
        colOperationalAlertTo = new GridColumn();
        colOperationalAlertPriority = new GridColumn();
        colOperationalAlertBlocking = new GridColumn();
        repoOperationalAlertCheck = new RepositoryItemCheckEdit();
        colOperationalAlertConfirmation = new GridColumn();
        colOperationalAlertActive = new GridColumn();
        gvOperationalAlertsAux = new GridView();
        btnAddOperationalAlert = new SimpleButton();
        btnUpdateOperationalAlert = new SimpleButton();
        btnRemoveOperationalAlert = new SimpleButton();
        btnClearOperationalAlert = new SimpleButton();
        sepNotesAlertsTitle = new LabelControl();
        sepRemarksColumn = new LabelControl();
        tabAttachments = new XtraTabPage();
        lblAttachmentGridTitle = new LabelControl();
        grdAttachments = new GridControl();
        gvAttachments = new GridView();
        colAttachmentDocumentType = new GridColumn();
        colAttachmentFileName = new GridColumn();
        colAttachmentDescription = new GridColumn();
        colAttachmentCategory = new GridColumn();
        colAttachmentExtension = new GridColumn();
        colAttachmentSize = new GridColumn();
        colAttachmentDate = new GridColumn();
        colAttachmentUser = new GridColumn();
        colAttachmentPrincipal = new GridColumn();
        repoAttachmentCheck = new RepositoryItemCheckEdit();
        colAttachmentVisibleSales = new GridColumn();
        colAttachmentVisiblePurchases = new GridColumn();
        colAttachmentVisiblePortal = new GridColumn();
        colAttachmentStatus = new GridColumn();
        btnAddAttachment = new SimpleButton();
        btnUpdateAttachment = new SimpleButton();
        btnRemoveAttachment = new SimpleButton();
        btnDownloadAttachment = new SimpleButton();
        btnOpenAttachment = new SimpleButton();
        btnSetMainAttachment = new SimpleButton();
        lblAttachmentMetadataTitle = new LabelControl();
        lblAttachmentPublicationTitle = new LabelControl();
        lblAttachmentType = new LabelControl();
        lueAttachmentType = new LookUpEdit();
        lblAttachmentFileName = new LabelControl();
        txtAttachmentFileName = new TextEdit();
        lblAttachmentDescription = new LabelControl();
        memAttachmentDescription = new MemoEdit();
        lblAttachmentCategory = new LabelControl();
        lueAttachmentCategory = new LookUpEdit();
        chkVisibleInSales = new ToggleSwitch();
        chkVisibleInPurchases = new ToggleSwitch();
        chkVisibleInPortal = new ToggleSwitch();
        lblAttachmentStatus = new LabelControl();
        lueAttachmentStatus = new LookUpEdit();
        lblAttachmentReference = new LabelControl();
        lblAttachmentPrincipal = new LabelControl();
        lblAttachmentVisibleSales = new LabelControl();
        lblAttachmentVisiblePurchases = new LabelControl();
        lblAttachmentVisiblePortal = new LabelControl();
        lblAttachmentConfidential = new LabelControl();
        txtAttachmentReference = new TextEdit();
        chkAttachmentPrincipal = new ToggleSwitch();
        chkAttachmentConfidential = new ToggleSwitch();
        lblAttachmentOrder = new LabelControl();
        spnAttachmentOrder = new SpinEdit();
        lblAttachmentValidFrom = new LabelControl();
        dteAttachmentValidFrom = new DateEdit();
        lblAttachmentValidTo = new LabelControl();
        dteAttachmentValidTo = new DateEdit();
        lblAttachmentAlternativeText = new LabelControl();
        memAttachmentAlternativeText = new MemoEdit();
        sepDocumentsColumnOne = new LabelControl();
        sepDocumentsColumnTwo = new LabelControl();
        sepAttachmentPreviewTitle = new LabelControl();
        sepAttachmentMetadataTitle = new LabelControl();
        sepAttachmentPublicationTitle = new LabelControl();
        sepAttachmentGridTitle = new LabelControl();
        lblAttachmentPreviewTitle = new LabelControl();
        picMainAttachmentPreview = new PictureEdit();
        btnLoadImage = new SimpleButton();
        btnRemoveImage = new SimpleButton();
        btnPreviewImage = new SimpleButton();
        btnSetMainImage = new SimpleButton();
        lblAttachmentPreviewNoteIcon = new LabelControl();
        lblAttachmentPreviewNote = new LabelControl();
        lblAttachmentExtension = new LabelControl();
        txtAttachmentExtension = new TextEdit();
        lblAttachmentSize = new LabelControl();
        txtAttachmentSize = new TextEdit();
        lblAttachmentUploadedAt = new LabelControl();
        dteAttachmentUploadedAt = new DateEdit();
        lblAttachmentUser = new LabelControl();
        txtAttachmentUser = new TextEdit();
        gridView2 = new GridView();
        tabSap = new XtraTabPage();
        tabSapSections = new XtraTabControl();
        tabSapStatusPage = new XtraTabPage();
        lblSapIntegrationNote = new LabelControl();
        lnkSapSynchronizeNow = new HyperlinkLabelControl();
        lnkSapRefreshStatus = new HyperlinkLabelControl();
        lnkSapViewProfile = new HyperlinkLabelControl();
        sepSapCorrespondenceTitleLine = new LabelControl();
        sepSapConfigTitleLine = new LabelControl();
        sepSapStatusTitleLine = new LabelControl();
        sepSapColumnTwo = new LabelControl();
        sepSapColumnOne = new LabelControl();
        lblSapSerialValue = new LabelControl();
        lblSapSerialCaption = new LabelControl();
        lblSapBatchValue = new LabelControl();
        lblSapBatchCaption = new LabelControl();
        lblSapAuthorityValue = new LabelControl();
        lblSapAuthorityCaption = new LabelControl();
        lblSapExternalCodeValue = new LabelControl();
        lblSapExternalCodeCaption = new LabelControl();
        lblSapExternalSystemValue = new LabelControl();
        lblSapExternalSystemCaption = new LabelControl();
        tglSapSynchronize = new ToggleSwitch();
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
        txtSapMapSystemField = new TextEdit();
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
        tabSapHistoryPage = new XtraTabPage();
        lblSapHistoryNote = new LabelControl();
        lnkSapRetry = new HyperlinkLabelControl();
        lnkSapCopyTracking = new HyperlinkLabelControl();
        lnkSapViewDetail = new HyperlinkLabelControl();
        sepSapActionsTitle = new LabelControl();
        lblSapActionsTitle = new LabelControl();
        lblSapExecutionProfileValue = new LabelControl();
        lblSapExecutionProfileCaption = new LabelControl();
        lblSapExecutionUserValue = new LabelControl();
        lblSapExecutionUserCaption = new LabelControl();
        lblSapExecutionTrackingValue = new LabelControl();
        lblSapExecutionTrackingCaption = new LabelControl();
        lblSapExecutionMessageValue = new LabelControl();
        lblSapExecutionMessageCaption = new LabelControl();
        lblSapExecutionResultValue = new LabelControl();
        lblSapExecutionResultCaption = new LabelControl();
        sepSapExecutionDetailTitle = new LabelControl();
        lblSapExecutionDetailTitle = new LabelControl();
        lnkSapRefreshHistory = new HyperlinkLabelControl();
        lblSapPendingRetriesSummary = new LabelControl();
        lblSapLastSyncSummary = new LabelControl();
        lblSapCurrentStatusSummary = new LabelControl();
        lblSapHistoryTitle = new LabelControl();
        grdSapSyncHistory = new GridControl();
        grvSapSyncHistory = new GridView();
        colSapHistoryDate = new GridColumn();
        colSapHistoryDocEntry = new GridColumn();
        colSapHistoryOperation = new GridColumn();
        colSapHistoryStatus = new GridColumn();
        colSapHistoryDocNum = new GridColumn();
        colSapHistoryRetryCount = new GridColumn();
        colSapHistoryDuration = new GridColumn();
        colSapHistoryTracking = new GridColumn();
        colSapHistoryMessage = new GridColumn();
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
        lueSapEnabled = new LookUpEdit();
        gridView3 = new GridView();
        gridView4 = new GridView();
        gridView5 = new GridView();
        gridView6 = new GridView();
        gridView7 = new GridView();
        tabLots = new XtraTabPage();
        sepTraceabilityHeader = new LabelControl();
        sepTraceabilityColumnOne = new LabelControl();
        sepTraceabilityColumnTwo = new LabelControl();
        sepTraceabilityGeneration = new LabelControl();
        sepTraceabilityExpiration = new LabelControl();
        sepTraceabilityOperations = new LabelControl();
        lblInheritedTraceabilityTitle = new LabelControl();
        lblInheritedBatchStatus = new LabelControl();
        lblInheritedSerialStatus = new LabelControl();
        lblInheritedPerishableStatus = new LabelControl();
        lblInheritedExpirationStatus = new LabelControl();
        lblLotOperationalRulesTitle = new LabelControl();
        lblIssueMethod = new LabelControl();
        lueIssueMethod = new LookUpEdit();
        lblAllowMultipleBatches = new LabelControl();
        tglAllowMultipleBatches = new ToggleSwitch();
        lblAllowReceiptWithoutLot = new LabelControl();
        tglAllowReceiptWithoutLot = new ToggleSwitch();
        lblAllowExpiredBatchSale = new LabelControl();
        tglAllowExpiredBatchSale = new ToggleSwitch();
        lblBlockQuarantineBatch = new LabelControl();
        tglBlockQuarantineBatch = new ToggleSwitch();
        lblBlockExpiredBatch = new LabelControl();
        tglBlockExpiredBatch = new ToggleSwitch();
        lblLotOperationalNotes = new LabelControl();
        memLotOperationalNotes = new MemoEdit();
        lblLotTraceabilityTitle = new LabelControl();
        lblLotExpirationTitle = new LabelControl();
        lblRequiresExpiration = new LabelControl();
        tglRequiresExpiration = new ToggleSwitch();
        lblExpirationMandatory = new LabelControl();
        tglExpirationMandatory = new ToggleSwitch();
        lblManufacturingDateRequired = new LabelControl();
        tglManufacturingDateRequired = new ToggleSwitch();
        lblAutoGenerateBatch = new LabelControl();
        tglAutoGenerateBatch = new ToggleSwitch();
        lblBatchPrefix = new LabelControl();
        txtBatchPrefix = new TextEdit();
        lblSerialLength = new LabelControl();
        spnSerialLength = new SpinEdit();
        lblShelfLifeDays = new LabelControl();
        spnShelfLifeDays = new SpinEdit();
        lblExpirationAlertDays = new LabelControl();
        spnExpirationAlertDays = new SpinEdit();
        lblQuarantineDays = new LabelControl();
        spnQuarantineDays = new SpinEdit();
        lblBatchFormat = new LabelControl();
        txtBatchFormat = new TextEdit();
        lblNumberingMethod = new LabelControl();
        lueNumberingMethod = new LookUpEdit();
        lblLotTransferRuleIcon = new LabelControl();
        lblLotTransferRule = new LabelControl();
        lblSerialDispatchRuleIcon = new LabelControl();
        lblSerialDispatchRule = new LabelControl();
        lblTraceabilityFooterIcon = new LabelControl();
        lblTraceabilityFooter = new LabelControl();
        pnlLotOperationalNote = new PanelControl();
        lblLotOperationalNoteIcon = new LabelControl();
        lblLotOperationalNote = new LabelControl();
        pnlLotTraceabilityNote = new PanelControl();
        lblLotTraceabilityNoteIcon = new LabelControl();
        lblLotTraceabilityNote = new LabelControl();
        tabTaxes = new XtraTabPage();
        sepTaxesColumnOne = new LabelControl();
        sepTaxesColumnTwo = new LabelControl();
        sepTaxConfiguration = new LabelControl();
        sepTaxRates = new LabelControl();
        sepTaxApplicability = new LabelControl();
        lblTaxConfigurationTitle = new LabelControl();
        lblFiscalItemType = new LabelControl();
        lueFiscalItemType = new LookUpEdit();
        lblPurchaseVat = new LabelControl();
        luePurchaseVat = new LookUpEdit();
        lblTaxesSalesVat = new LabelControl();
        lueTaxesSalesVat = new LookUpEdit();
        lblExciseTax = new LabelControl();
        lueExciseTax = new LookUpEdit();
        lblTaxesSuggestedWithholding = new LabelControl();
        lueTaxesSuggestedWithholding = new LookUpEdit();
        lblTaxSupport = new LabelControl();
        lueTaxSupport = new LookUpEdit();
        lblFiscalCode = new LabelControl();
        txtFiscalCode = new TextEdit();
        lblFiscalCountry = new LabelControl();
        lueFiscalCountry = new LookUpEdit();
        lblTaxableGoods = new LabelControl();
        tglTaxableGoods = new ToggleSwitch();
        lblTaxableService = new LabelControl();
        tglTaxableService = new ToggleSwitch();
        lblTaxExemptGoods = new LabelControl();
        tglTaxExemptGoods = new ToggleSwitch();
        lblTaxRatesTitle = new LabelControl();
        lblTaxApplicabilityTitle = new LabelControl();
        tabAccounting = new XtraTabPage();
        sepAccountingColumnOne = new LabelControl();
        sepAccountingColumnTwo = new LabelControl();
        sepAccountingAccounts = new LabelControl();
        sepAccountingComplementary = new LabelControl();
        sepAccountingRules = new LabelControl();
        lblAccountingRulesTitle = new LabelControl();
        lblGenerateInventoryJournal = new LabelControl();
        lblAccountingAccountsTitle = new LabelControl();
        tglGenerateInventoryJournal = new ToggleSwitch();
        lblAccountingComplementaryTitle = new LabelControl();
        lblUseWarehouseAccount = new LabelControl();
        lblAccountingInventoryAccount = new LabelControl();
        tglUseWarehouseAccount = new ToggleSwitch();
        slueInventoryAccount = new SearchLookUpEdit();
        gvInventoryAccount = new GridView();
        lblUseGroupAccount = new LabelControl();
        lblAccountingRevenueAccount = new LabelControl();
        tglUseGroupAccount = new ToggleSwitch();
        slueRevenueAccount = new SearchLookUpEdit();
        gvRevenueAccount = new GridView();
        lblAllowCompensation = new LabelControl();
        lblCostOfGoodsSoldAccount = new LabelControl();
        tglAllowCompensation = new ToggleSwitch();
        slueCostOfGoodsSoldAccount = new SearchLookUpEdit();
        gvCostOfGoodsSoldAccount = new GridView();
        lblAccountingBlocked = new LabelControl();
        lblSalesReturnAccount = new LabelControl();
        tglAccountingBlocked = new ToggleSwitch();
        slueSalesReturnAccount = new SearchLookUpEdit();
        gvSalesReturnAccount = new GridView();
        lblReconciliationDays = new LabelControl();
        lblPurchaseReturnAccount = new LabelControl();
        spnReconciliationDays = new SpinEdit();
        sluePurchaseReturnAccount = new SearchLookUpEdit();
        gvPurchaseReturnAccount = new GridView();
        lblAccountingIntegrationMethod = new LabelControl();
        lblCostVarianceAccount = new LabelControl();
        lueAccountingIntegrationMethod = new LookUpEdit();
        slueCostVarianceAccount = new SearchLookUpEdit();
        gvCostVarianceAccount = new GridView();
        lblAccountingNotes = new LabelControl();
        lblInventoryAdjustmentAccount = new LabelControl();
        memAccountingNotes = new MemoEdit();
        slueInventoryAdjustmentAccount = new SearchLookUpEdit();
        gvInventoryAdjustmentAccount = new GridView();
        lblPurchaseExpenseAccount = new LabelControl();
        sluePurchaseExpenseAccount = new SearchLookUpEdit();
        gvPurchaseExpenseAccount = new GridView();
        tabCosts = new XtraTabPage();
        sepCostsColumnOne = new LabelControl();
        sepCostsColumnTwo = new LabelControl();
        sepCostsBase = new LabelControl();
        sepCostsPrices = new LabelControl();
        sepCostsIndicators = new LabelControl();
        sepCostsHistory = new LabelControl();
        lblCostPriceHistoryTitle = new LabelControl();
        grdCostPriceHistory = new GridControl();
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
        gvCostPriceHistoryAux = new GridView();
        lblPricesMarginsTitle = new LabelControl();
        lblAnalysisBasePrice = new LabelControl();
        lblCostsBaseTitle = new LabelControl();
        spnAnalysisBasePrice = new SpinEdit();
        lblCostCurrency = new LabelControl();
        lblSuggestedPrice = new LabelControl();
        lueCostCurrency = new LookUpEdit();
        spnSuggestedPrice = new SpinEdit();
        lblStandardCost = new LabelControl();
        lblMinimumMarginPercent = new LabelControl();
        spnStandardCost = new SpinEdit();
        spnMinimumMarginPercent = new SpinEdit();
        lblReplacementCost = new LabelControl();
        lblTargetMarginPercent = new LabelControl();
        spnReplacementCost = new SpinEdit();
        spnTargetMarginPercent = new SpinEdit();
        lblLastCost = new LabelControl();
        spnLastCost = new SpinEdit();
        lblCostsAverageCost = new LabelControl();
        spnAverageCost = new SpinEdit();
        lblPriceUpdatedAt = new LabelControl();
        lblCostUpdatedAt = new LabelControl();
        dtPriceUpdatedAt = new DateEdit();
        dtCostUpdatedAt = new DateEdit();
        lblManualCostUpdate = new LabelControl();
        tglManualCostUpdate = new ToggleSwitch();
        lblFinanceCostIndicatorsTitle = new LabelControl();
        kpiFinanceGrossMargin = new NuanOperationalKpiCardControl();
        kpiFinanceGrossMarginPercent = new NuanOperationalKpiCardControl();
        kpiFinanceProfitability = new NuanOperationalKpiCardControl();
        kpiFinanceSuggestedPrice = new NuanOperationalKpiCardControl();
        tabSales = new XtraTabPage();
        labelControl8 = new LabelControl();
        labelControl7 = new LabelControl();
        labelControl6 = new LabelControl();
        labelControl5 = new LabelControl();
        sepSalesColumnOne = new LabelControl();
        sepSalesColumnTwo = new LabelControl();
        sepSalesConfiguration = new LabelControl();
        sepSalesConditions = new LabelControl();
        sepSalesIndicators = new LabelControl();
        sepSalesPriceLists = new LabelControl();
        lblSalesConditionsTitle = new LabelControl();
        lblSalesIndicatorsTitle = new LabelControl();
        lblSalesPricePerformanceTitle = new LabelControl();
        grdSalesPriceLists = new GridControl();
        gvSalesPriceLists = new GridView();
        colSalesPriceListName = new GridColumn();
        colSalesPriceListCurrency = new GridColumn();
        colSalesPriceListPrice = new GridColumn();
        colSalesPriceListMargin = new GridColumn();
        colSalesPriceListValidFrom = new GridColumn();
        colSalesPriceListActive = new GridColumn();
        repoSalesPriceListActive = new RepositoryItemCheckEdit();
        gvSalesPriceListsAux = new GridView();
        lblSalesConfigurationTitle = new LabelControl();
        kpiSales30d = new NuanOperationalKpiCardControl();
        lblAffectsPromotions = new LabelControl();
        kpiSales12m = new NuanOperationalKpiCardControl();
        tglAffectsPromotions = new ToggleSwitch();
        kpiSalesLastPrice = new NuanOperationalKpiCardControl();
        lblSalesUnit = new LabelControl();
        kpiSalesCustomers = new NuanOperationalKpiCardControl();
        lueSalesUnit = new LookUpEdit();
        lblBaseSalesPrice = new LabelControl();
        spnBaseSalesPrice = new SpinEdit();
        lueSalesCurrency = new LookUpEdit();
        lblMainPriceList = new LabelControl();
        lueMainPriceList = new LookUpEdit();
        lblAllowSalesDiscount = new LabelControl();
        tglAllowSalesDiscount = new ToggleSwitch();
        lblMaxDiscount = new LabelControl();
        spnMaxDiscount = new SpinEdit();
        lblMinimumMargin = new LabelControl();
        spnMinimumMargin = new SpinEdit();
        lblMinimumSale = new LabelControl();
        spnMinimumSale = new SpinEdit();
        lblMinimumSaleUnit = new LabelControl();
        lblSalesMultiple = new LabelControl();
        spnSalesMultiple = new SpinEdit();
        lblSalesMultipleUnit = new LabelControl();
        lblSalesCommission = new LabelControl();
        spnSalesCommission = new SpinEdit();
        lblSalesChannel = new LabelControl();
        lueSalesChannel = new LookUpEdit();
        lblSalesSegment = new LabelControl();
        lueSalesSegment = new LookUpEdit();
        lblSalesMinimumPriceList = new LabelControl();
        lueSalesMinimumPriceList = new LookUpEdit();
        lblSalesMinimumPrice = new LabelControl();
        spnSalesMinimumPrice = new SpinEdit();
        lblSalesMinimumCurrency = new LabelControl();
        lblSalesValidFrom = new LabelControl();
        dtSalesValidFrom = new DateEdit();
        lblSalesEcommerce = new LabelControl();
        tglSalesEcommerce = new ToggleSwitch();
        lblSalesCommercialObservation = new LabelControl();
        memSalesCommercialObservation = new MemoEdit();
        btnViewSalesHistory = new SimpleButton();
        btnRefreshSales = new SimpleButton();
        tabInventory = new XtraTabPage();
        sepInventoryColumnOne = new LabelControl();
        sepInventoryColumnTwo = new LabelControl();
        sepInventoryParameters = new LabelControl();
        sepInventoryReplenishment = new LabelControl();
        sepInventoryLocations = new LabelControl();
        sepInventoryWarehouse = new LabelControl();
        lblWarehouseSummary = new LabelControl();
        btnAddWarehouseStock = new SimpleButton();
        btnUpdateWarehouseStock = new SimpleButton();
        btnRemoveWarehouseStock = new SimpleButton();
        btnSetMainWarehouseStock = new SimpleButton();
        lblInventoryLocationsRestrictionsTitle = new LabelControl();
        lblDefaultBinLocation = new LabelControl();
        lblStockByWarehouseTitle = new LabelControl();
        slueDefaultBinLocation = new SearchLookUpEdit();
        gvDefaultBinLocation = new GridView();
        grdWarehouseStock = new GridControl();
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
        gvWarehouseStockAux = new GridView();
        lblCoverageDays = new LabelControl();
        spnCoverageDays = new SpinEdit();
        lblReplenishmentOperationTitle = new LabelControl();
        lblLeadTimeDays = new LabelControl();
        lblInventoryParametersTitle = new LabelControl();
        spnLeadTimeDays = new SpinEdit();
        lblReplenishmentMethod = new LabelControl();
        lblGlobalMinStock = new LabelControl();
        lblSupplyMethod = new LabelControl();
        spnGlobalMinStock = new SpinEdit();
        lueReplenishmentMethod = new LookUpEdit();
        lblGlobalMaxStock = new LabelControl();
        lblMainWarehouse = new LabelControl();
        spnGlobalMaxStock = new SpinEdit();
        lblBlockedForMovements = new LabelControl();
        lueSupplyMethod = new LookUpEdit();
        tglBlockedForMovements = new ToggleSwitch();
        lblGlobalReorderPoint = new LabelControl();
        lblInventoryOperationNote = new LabelControl();
        lblValuationMethod = new LabelControl();
        memInventoryOperationNote = new MemoEdit();
        spnGlobalReorderPoint = new SpinEdit();
        slueMainWarehouse = new SearchLookUpEdit();
        gvMainWarehouse = new GridView();
        lblSuggestedPurchaseQty = new LabelControl();
        lueValuationMethod = new LookUpEdit();
        spnSuggestedPurchaseQty = new SpinEdit();
        lblNegativeStockPolicy = new LabelControl();
        lblReplenishmentApproval = new LabelControl();
        lueNegativeStockPolicy = new LookUpEdit();
        tglReplenishmentApproval = new ToggleSwitch();
        lblAutoReplenishment = new LabelControl();
        tglAutoReplenishment = new ToggleSwitch();
        lblManageLocations = new LabelControl();
        tglManageLocations = new ToggleSwitch();
        lblRequiresCycleCount = new LabelControl();
        tglRequiresCycleCount = new ToggleSwitch();
        lblAbcClassification = new LabelControl();
        lueAbcClassification = new LookUpEdit();
        lblInventoryControlType = new LabelControl();
        lueInventoryControlType = new LookUpEdit();
        lblInventoryBlockReason = new LabelControl();
        memInventoryBlockReason = new MemoEdit();
        tabUnits = new XtraTabPage();
        sepUnitsColumn = new LabelControl();
        sepUnitsMeasures = new LabelControl();
        sepUnitsIdentifiers = new LabelControl();
        sepUnitsPresentations = new LabelControl();
        lblPresentationSummary = new LabelControl();
        lblCodesIdentifiersTitle = new LabelControl();
        lblQrCode = new LabelControl();
        lblPurchasePresentationsTitle = new LabelControl();
        txtQrCode = new TextEdit();
        grdItemPresentations = new GridControl();
        gvItemPresentations = new GridView();
        colPurchasePresentation = new GridColumn();
        colPurchaseUnit = new GridColumn();
        colPurchaseFactor = new GridColumn();
        colPurchaseBarcode = new GridColumn();
        colPurchaseEnabled = new GridColumn();
        repoPurchaseActive = new RepositoryItemCheckEdit();
        colSalesEnabled = new GridColumn();
        colPurchasePrincipal = new GridColumn();
        repoPurchasePrincipal = new RepositoryItemCheckEdit();
        colSalesPrincipal = new GridColumn();
        colPurchaseActive = new GridColumn();
        gvItemPresentationsAux = new GridView();
        lblPlu = new LabelControl();
        lblInventoryUnitTitle = new LabelControl();
        txtPlu = new TextEdit();
        btnAddItemPresentation = new SimpleButton();
        lblPreviousInternalCode = new LabelControl();
        lblInventoryUnit = new LabelControl();
        txtPreviousInternalCode = new TextEdit();
        btnUpdateItemPresentation = new SimpleButton();
        lblManufacturerReference = new LabelControl();
        lueInventoryUnit = new LookUpEdit();
        txtManufacturerReference = new TextEdit();
        btnRemoveItemPresentation = new SimpleButton();
        lblUnspscCode = new LabelControl();
        txtUnspscCode = new TextEdit();
        btnSetMainItemPresentation = new SimpleButton();
        lblTariffCode = new LabelControl();
        txtTariffCode = new TextEdit();
        lblNetWeight = new LabelControl();
        lblCodeOrigin = new LabelControl();
        spnNetWeight = new SpinEdit();
        lueCodeOrigin = new LookUpEdit();
        lblNetWeightUnit = new LabelControl();
        lblGrossWeight = new LabelControl();
        spnGrossWeight = new SpinEdit();
        lblGrossWeightUnit = new LabelControl();
        lblVolume = new LabelControl();
        spnVolume = new SpinEdit();
        lblVolumeUnitCaption = new LabelControl();
        lblWeightUnit = new LabelControl();
        lueWeightUnit = new LookUpEdit();
        lblVolumeUnit = new LabelControl();
        lueVolumeUnit = new LookUpEdit();
        tabGeneral = new XtraTabPage();
        sepGeneralColumnTwo = new LabelControl();
        sepGeneralColumnOne = new LabelControl();
        sepGeneralSummary = new LabelControl();
        sepGeneralOperation = new LabelControl();
        sepGeneralIdentification = new LabelControl();
        kpiStockAvailable = new NuanOperationalKpiCardControl();
        kpiOnOrder = new NuanOperationalKpiCardControl();
        kpiCommitted = new NuanOperationalKpiCardControl();
        kpiPurchases = new NuanOperationalKpiCardControl();
        kpiSales = new NuanOperationalKpiCardControl();
        kpiSapStatus = new NuanOperationalKpiCardControl();
        kpiAverageCost = new NuanOperationalKpiCardControl();
        kpiMargin = new NuanOperationalKpiCardControl();
        kpiPurchaseCost = new NuanOperationalKpiCardControl();
        kpiSalesPrice = new NuanOperationalKpiCardControl();
        lblBlockedEcommerce = new LabelControl();
        tglBlockedEcommerce = new ToggleSwitch();
        lblGeneralSummaryTitle = new LabelControl();
        tglGeneralMobileItem = new ToggleSwitch();
        lblGeneralMobileItem = new LabelControl();
        tglGeneralRequiresScale = new ToggleSwitch();
        tglGeneralAllowDiscount = new ToggleSwitch();
        tglGeneralPerishable = new ToggleSwitch();
        btnTraceabilityNone = new SimpleButton();
        btnTraceabilityBatch = new SimpleButton();
        btnTraceabilitySerial = new SimpleButton();
        lblGeneralOperationTitle = new LabelControl();
        lblTraceabilityManagement = new LabelControl();
        lblTraceabilityHintIcon = new LabelControl();
        lblTraceabilityHint = new LabelControl();
        lblPerishable = new LabelControl();
        lblExpirationManaged = new LabelControl();
        lblRequiresScale = new LabelControl();
        lblAllowDiscount = new LabelControl();
        tglGeneralExpirationManaged = new ToggleSwitch();
        lblGeneralIdentificationTitle = new LabelControl();
        lblAlternateCode = new LabelControl();
        txtAlternateCode = new TextEdit();
        lblSupplierSku = new LabelControl();
        slueSupplierSku = new SearchLookUpEdit();
        gvSupplierSku = new GridView();
        lblLongDescription = new LabelControl();
        memLongDescription = new MemoEdit();
        lblProductType = new LabelControl();
        lueProductType = new LookUpEdit();
        lblOrigin = new LabelControl();
        lueOrigin = new LookUpEdit();
        lblLine = new LabelControl();
        lueLine = new LookUpEdit();
        lblSubGroup = new LabelControl();
        lueSubGroup = new LookUpEdit();
        lblModel = new LabelControl();
        txtModel = new TextEdit();
        lblReference = new LabelControl();
        txtReference = new TextEdit();
        lblAffectsInventory = new LabelControl();
        tglAffectsInventory = new ToggleSwitch();
        lblSalesActive = new LabelControl();
        tglSalesActive = new ToggleSwitch();
        lblPurchaseActive = new LabelControl();
        tglPurchaseActive = new ToggleSwitch();
        tabCommercial = new XtraTabPage();
        tabCommercialSections = new XtraTabControl();
        tabPurchases = new XtraTabPage();
        labelControl4 = new LabelControl();
        labelControl3 = new LabelControl();
        labelControl2 = new LabelControl();
        labelControl1 = new LabelControl();
        sepPurchasesColumnOne = new LabelControl();
        sepPurchasesColumnTwo = new LabelControl();
        sepPurchasesConfiguration = new LabelControl();
        sepPurchasesConditions = new LabelControl();
        sepPurchasesIndicators = new LabelControl();
        sepPurchasesHistory = new LabelControl();
        lblPurchasesConditionsTitle = new LabelControl();
        lblPurchasesIndicatorsTitle = new LabelControl();
        lblPurchaseUnit = new LabelControl();
        luePurchaseUnit = new LookUpEdit();
        kpiPurchaseCompliance = new NuanOperationalKpiCardControl();
        lblPurchasesHistoryTitle = new LabelControl();
        grdPurchaseHistory = new GridControl();
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
        gvPurchaseHistoryAux = new GridView();
        gridView1 = new GridView();
        lblPurchasesConfigurationTitle = new LabelControl();
        kpiPurchaseLast = new NuanOperationalKpiCardControl();
        lblPurchaseApprovalRequired = new LabelControl();
        kpiPurchaseAverage = new NuanOperationalKpiCardControl();
        tglPurchaseApprovalRequired = new ToggleSwitch();
        kpiPurchaseLeadTime = new NuanOperationalKpiCardControl();
        lblSupplierBackorderAllowed = new LabelControl();
        tglSupplierBackorderAllowed = new ToggleSwitch();
        memReceivingNote = new MemoEdit();
        lblPurchaseOnDemand = new LabelControl();
        lblReceivingNote = new LabelControl();
        tglPurchaseOnDemand = new ToggleSwitch();
        memPurchasePolicy = new MemoEdit();
        lblPurchasePolicy = new LabelControl();
        lblMainPurchaseSupplier = new LabelControl();
        slueMainPurchaseSupplier = new SearchLookUpEdit();
        gvMainPurchaseSupplier = new GridView();
        lblPreferredPurchasePresentation = new LabelControl();
        luePreferredPurchasePresentation = new LookUpEdit();
        lblPreferredPurchaseCurrency = new LabelControl();
        luePreferredPurchaseCurrency = new LookUpEdit();
        lblPurchaseMinimumQuantity = new LabelControl();
        spnPurchaseMinimumQuantity = new SpinEdit();
        lblPurchaseMultiple = new LabelControl();
        spnPurchaseMultiple = new SpinEdit();
        lblPurchaseDeliveryDays = new LabelControl();
        spnPurchaseDeliveryDays = new SpinEdit();
        btnViewPurchaseDocument = new SimpleButton();
        btnRefreshPurchases = new SimpleButton();
        tabFinance = new XtraTabPage();
        tabFinanceSections = new XtraTabControl();
        tabDocuments = new XtraTabPage();
        tabDocumentSections = new XtraTabControl();
        tabMain = new XtraTabControl();
        sepHeaderData = new LabelControl();
        ((System.ComponentModel.ISupportInitialize)picItem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtItemCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCommercialName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueItemType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueItemGroup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueItemFamily.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBrand.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBaseUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)itemPresentationsTable).BeginInit();
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
        ((System.ComponentModel.ISupportInitialize)memGeneralNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memGeneralOperationalAlert.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueNotePriority.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueNoteVisibility.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkGeneralNoteActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memPurchaseNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memSalesNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memInventoryNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memLogisticsQualityNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdOperationalAlerts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvOperationalAlerts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoOperationalAlertCheck).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvOperationalAlertsAux).BeginInit();
        tabAttachments.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoAttachmentCheck).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentFileName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memAttachmentDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentCategory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInSales.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInPurchases.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInPortal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentReference.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAttachmentPrincipal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAttachmentConfidential.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnAttachmentOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentValidFrom.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentValidFrom.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentValidTo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentValidTo.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memAttachmentAlternativeText.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)picMainAttachmentPreview.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentExtension.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentSize.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentUploadedAt.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentUploadedAt.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentUser.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView2).BeginInit();
        tabSap.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tabSapSections).BeginInit();
        tabSapSections.SuspendLayout();
        tabSapStatusPage.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglSapSynchronize.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapMapEnabled.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapMapRequired.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapSapField.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapSystemField.Properties).BeginInit();
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
        tabSapHistoryPage.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdSapSyncHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSapSyncHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdSapFieldMapping).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSapFieldMapping).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapEnabled.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView3).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView4).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView5).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView6).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView7).BeginInit();
        tabLots.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueIssueMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowMultipleBatches.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowReceiptWithoutLot.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowExpiredBatchSale.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockQuarantineBatch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockExpiredBatch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memLotOperationalNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresExpiration.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglExpirationMandatory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglManufacturingDateRequired.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAutoGenerateBatch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBatchPrefix.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSerialLength.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnShelfLifeDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnExpirationAlertDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnQuarantineDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBatchFormat.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueNumberingMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlLotOperationalNote).BeginInit();
        pnlLotOperationalNote.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlLotTraceabilityNote).BeginInit();
        pnlLotTraceabilityNote.SuspendLayout();
        tabTaxes.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueFiscalItemType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseVat.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxesSalesVat.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueExciseTax.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxesSuggestedWithholding.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxSupport.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtFiscalCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglTaxableGoods.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglTaxableService.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglTaxExemptGoods.Properties).BeginInit();
        tabAccounting.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglGenerateInventoryJournal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglUseWarehouseAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueInventoryAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvInventoryAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglUseGroupAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueRevenueAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvRevenueAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowCompensation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueCostOfGoodsSoldAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvCostOfGoodsSoldAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAccountingBlocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueSalesReturnAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvSalesReturnAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnReconciliationDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluePurchaseReturnAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseReturnAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingIntegrationMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueCostVarianceAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvCostVarianceAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memAccountingNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueInventoryAdjustmentAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvInventoryAdjustmentAccount).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluePurchaseExpenseAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseExpenseAccount).BeginInit();
        tabCosts.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdCostPriceHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvCostPriceHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvCostPriceHistoryAux).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnAnalysisBasePrice.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCostCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSuggestedPrice.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnStandardCost.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumMarginPercent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnReplacementCost.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnTargetMarginPercent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnLastCost.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnAverageCost.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtPriceUpdatedAt.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtPriceUpdatedAt.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtCostUpdatedAt.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtCostUpdatedAt.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglManualCostUpdate.Properties).BeginInit();
        tabSales.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdSalesPriceLists).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvSalesPriceLists).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoSalesPriceListActive).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvSalesPriceListsAux).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAffectsPromotions.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnBaseSalesPrice.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueMainPriceList.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowSalesDiscount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMaxDiscount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumMargin.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumSale.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSalesMultiple.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSalesCommission.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesChannel.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesSegment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesMinimumPriceList.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSalesMinimumPrice.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtSalesValidFrom.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtSalesValidFrom.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSalesEcommerce.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memSalesCommercialObservation.Properties).BeginInit();
        tabInventory.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)slueDefaultBinLocation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvDefaultBinLocation).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdWarehouseStock).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvWarehouseStock).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvWarehouseStockAux).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCoverageDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnLeadTimeDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalMinStock.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueReplenishmentMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalMaxStock.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplyMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockedForMovements.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memInventoryOperationNote.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalReorderPoint.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueMainWarehouse.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvMainWarehouse).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueValuationMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSuggestedPurchaseQty.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueNegativeStockPolicy.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglReplenishmentApproval.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAutoReplenishment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglManageLocations.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresCycleCount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAbcClassification.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueInventoryControlType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memInventoryBlockReason.Properties).BeginInit();
        tabUnits.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)txtQrCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdItemPresentations).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvItemPresentations).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoPurchaseActive).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoPurchasePrincipal).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvItemPresentationsAux).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPlu.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPreviousInternalCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueInventoryUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtManufacturerReference.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtUnspscCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtTariffCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnNetWeight.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCodeOrigin.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnGrossWeight.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnVolume.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueWeightUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueVolumeUnit.Properties).BeginInit();
        tabGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglBlockedEcommerce.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralMobileItem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralRequiresScale.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralAllowDiscount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralPerishable.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralExpirationManaged.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAlternateCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueSupplierSku.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvSupplierSku).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memLongDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueProductType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueOrigin.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueLine.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSubGroup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtModel.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtReference.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAffectsInventory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSalesActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseActive.Properties).BeginInit();
        tabCommercial.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tabCommercialSections).BeginInit();
        tabCommercialSections.SuspendLayout();
        tabPurchases.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)luePurchaseUnit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdPurchaseHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseHistoryAux).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridView1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseApprovalRequired.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSupplierBackorderAllowed.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memReceivingNote.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseOnDemand.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memPurchasePolicy.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)slueMainPurchaseSupplier.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvMainPurchaseSupplier).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePreferredPurchasePresentation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePreferredPurchaseCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnPurchaseMinimumQuantity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnPurchaseMultiple.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnPurchaseDeliveryDays.Properties).BeginInit();
        tabFinance.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tabFinanceSections).BeginInit();
        tabFinanceSections.SuspendLayout();
        tabDocuments.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tabDocumentSections).BeginInit();
        tabDocumentSections.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tabMain).BeginInit();
        tabMain.SuspendLayout();
        this.SuspendLayout();
        // 
        // btnCancelar
        // 
        this.btnCancelar.Appearance.BackColor = Color.FromArgb((int)(byte)99, (int)(byte)110, (int)(byte)114);
        this.btnCancelar.Appearance.BorderColor = Color.FromArgb((int)(byte)99, (int)(byte)110, (int)(byte)114);
        this.btnCancelar.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        this.btnCancelar.Appearance.ForeColor = Color.White;
        this.btnCancelar.Appearance.Options.UseBackColor = true;
        this.btnCancelar.Appearance.Options.UseBorderColor = true;
        this.btnCancelar.Appearance.Options.UseFont = true;
        this.btnCancelar.Appearance.Options.UseForeColor = true;
        this.btnCancelar.AppearanceHovered.BackColor = Color.FromArgb((int)(byte)78, (int)(byte)87, (int)(byte)90);
        this.btnCancelar.AppearanceHovered.BorderColor = Color.FromArgb((int)(byte)78, (int)(byte)87, (int)(byte)90);
        this.btnCancelar.AppearanceHovered.ForeColor = Color.White;
        this.btnCancelar.AppearanceHovered.Options.UseBackColor = true;
        this.btnCancelar.AppearanceHovered.Options.UseBorderColor = true;
        this.btnCancelar.AppearanceHovered.Options.UseForeColor = true;
        this.btnCancelar.AppearancePressed.BackColor = Color.FromArgb((int)(byte)60, (int)(byte)67, (int)(byte)70);
        this.btnCancelar.AppearancePressed.BorderColor = Color.FromArgb((int)(byte)60, (int)(byte)67, (int)(byte)70);
        this.btnCancelar.AppearancePressed.ForeColor = Color.White;
        this.btnCancelar.AppearancePressed.Options.UseBackColor = true;
        this.btnCancelar.AppearancePressed.Options.UseBorderColor = true;
        this.btnCancelar.AppearancePressed.Options.UseForeColor = true;
        this.btnCancelar.ImageOptions.ImageToTextIndent = 0;
        this.btnCancelar.ImageOptions.Location = ImageLocation.MiddleLeft;
        this.btnCancelar.ImageOptions.SvgImageSize = new Size(24, 24);
        this.btnCancelar.Location = new Point(1286, 606);
        this.btnCancelar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        this.btnCancelar.LookAndFeel.UseDefaultLookAndFeel = false;
        this.btnCancelar.Size = new Size(142, 40);
        // 
        // btnGuardar
        // 
        this.btnGuardar.Appearance.BackColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        this.btnGuardar.Appearance.BorderColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        this.btnGuardar.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        this.btnGuardar.Appearance.ForeColor = Color.White;
        this.btnGuardar.Appearance.Options.UseBackColor = true;
        this.btnGuardar.Appearance.Options.UseBorderColor = true;
        this.btnGuardar.Appearance.Options.UseFont = true;
        this.btnGuardar.Appearance.Options.UseForeColor = true;
        this.btnGuardar.AppearanceHovered.BackColor = Color.FromArgb((int)(byte)0, (int)(byte)160, (int)(byte)128);
        this.btnGuardar.AppearanceHovered.BorderColor = Color.FromArgb((int)(byte)0, (int)(byte)160, (int)(byte)128);
        this.btnGuardar.AppearanceHovered.ForeColor = Color.White;
        this.btnGuardar.AppearanceHovered.Options.UseBackColor = true;
        this.btnGuardar.AppearanceHovered.Options.UseBorderColor = true;
        this.btnGuardar.AppearanceHovered.Options.UseForeColor = true;
        this.btnGuardar.AppearancePressed.BackColor = Color.FromArgb((int)(byte)0, (int)(byte)137, (int)(byte)111);
        this.btnGuardar.AppearancePressed.BorderColor = Color.FromArgb((int)(byte)0, (int)(byte)137, (int)(byte)111);
        this.btnGuardar.AppearancePressed.ForeColor = Color.White;
        this.btnGuardar.AppearancePressed.Options.UseBackColor = true;
        this.btnGuardar.AppearancePressed.Options.UseBorderColor = true;
        this.btnGuardar.AppearancePressed.Options.UseForeColor = true;
        this.btnGuardar.ImageOptions.ImageToTextIndent = 0;
        this.btnGuardar.ImageOptions.Location = ImageLocation.MiddleLeft;
        this.btnGuardar.ImageOptions.SvgImageSize = new Size(24, 24);
        this.btnGuardar.Location = new Point(1434, 606);
        this.btnGuardar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        this.btnGuardar.LookAndFeel.UseDefaultLookAndFeel = false;
        this.btnGuardar.Size = new Size(148, 40);
        // 
        // lblHeaderDataTitle
        // 
        lblHeaderDataTitle.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblHeaderDataTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblHeaderDataTitle.Appearance.Options.UseFont = true;
        lblHeaderDataTitle.Appearance.Options.UseForeColor = true;
        lblHeaderDataTitle.Location = new Point(192, 12);
        lblHeaderDataTitle.Name = "lblHeaderDataTitle";
        lblHeaderDataTitle.Size = new Size(103, 17);
        lblHeaderDataTitle.TabIndex = 32;
        lblHeaderDataTitle.Text = "Datos principales";
        // 
        // lblHeaderClassificationTitle
        // 
        lblHeaderClassificationTitle.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblHeaderClassificationTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblHeaderClassificationTitle.Appearance.Options.UseFont = true;
        lblHeaderClassificationTitle.Appearance.Options.UseForeColor = true;
        lblHeaderClassificationTitle.Location = new Point(1065, 12);
        lblHeaderClassificationTitle.Name = "lblHeaderClassificationTitle";
        lblHeaderClassificationTitle.Size = new Size(125, 17);
        lblHeaderClassificationTitle.TabIndex = 33;
        lblHeaderClassificationTitle.Text = "Clasificación del ítem";
        // 
        // lblHeaderCommercialSummaryTitle
        // 
        lblHeaderCommercialSummaryTitle.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblHeaderCommercialSummaryTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblHeaderCommercialSummaryTitle.Appearance.Options.UseFont = true;
        lblHeaderCommercialSummaryTitle.Appearance.Options.UseForeColor = true;
        lblHeaderCommercialSummaryTitle.Location = new Point(1319, 12);
        lblHeaderCommercialSummaryTitle.Name = "lblHeaderCommercialSummaryTitle";
        lblHeaderCommercialSummaryTitle.Size = new Size(117, 17);
        lblHeaderCommercialSummaryTitle.TabIndex = 34;
        lblHeaderCommercialSummaryTitle.Text = "Resumen comercial";
        // 
        // sepHeaderClassification
        // 
        sepHeaderClassification.Appearance.BackColor = Color.FromArgb((int)(byte)223, (int)(byte)228, (int)(byte)234);
        sepHeaderClassification.Appearance.Options.UseBackColor = true;
        sepHeaderClassification.AutoSizeMode = LabelAutoSizeMode.None;
        sepHeaderClassification.Location = new Point(1223, 21);
        sepHeaderClassification.Name = "sepHeaderClassification";
        sepHeaderClassification.Size = new Size(64, 1);
        sepHeaderClassification.TabIndex = 36;
        // 
        // sepHeaderCommercialSummary
        // 
        sepHeaderCommercialSummary.Appearance.BackColor = Color.FromArgb((int)(byte)223, (int)(byte)228, (int)(byte)234);
        sepHeaderCommercialSummary.Appearance.Options.UseBackColor = true;
        sepHeaderCommercialSummary.AutoSizeMode = LabelAutoSizeMode.None;
        sepHeaderCommercialSummary.Location = new Point(1466, 21);
        sepHeaderCommercialSummary.Name = "sepHeaderCommercialSummary";
        sepHeaderCommercialSummary.Size = new Size(105, 1);
        sepHeaderCommercialSummary.TabIndex = 37;
        // 
        // lblHeaderStockCaption
        // 
        lblHeaderStockCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblHeaderStockCaption.Appearance.ForeColor = Color.FromArgb((int)(byte)72, (int)(byte)84, (int)(byte)96);
        lblHeaderStockCaption.Appearance.Options.UseFont = true;
        lblHeaderStockCaption.Appearance.Options.UseForeColor = true;
        lblHeaderStockCaption.Location = new Point(1319, 41);
        lblHeaderStockCaption.Name = "lblHeaderStockCaption";
        lblHeaderStockCaption.Size = new Size(59, 15);
        lblHeaderStockCaption.TabIndex = 38;
        lblHeaderStockCaption.Text = "Stock total:";
        // 
        // lblHeaderStockValue
        // 
        lblHeaderStockValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblHeaderStockValue.Appearance.Options.UseFont = true;
        lblHeaderStockValue.Appearance.Options.UseTextOptions = true;
        lblHeaderStockValue.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        lblHeaderStockValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblHeaderStockValue.Location = new Point(1469, 41);
        lblHeaderStockValue.Name = "lblHeaderStockValue";
        lblHeaderStockValue.Size = new Size(100, 15);
        lblHeaderStockValue.TabIndex = 39;
        lblHeaderStockValue.Text = "0.00";
        // 
        // lblHeaderAverageCostCaption
        // 
        lblHeaderAverageCostCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblHeaderAverageCostCaption.Appearance.ForeColor = Color.FromArgb((int)(byte)72, (int)(byte)84, (int)(byte)96);
        lblHeaderAverageCostCaption.Appearance.Options.UseFont = true;
        lblHeaderAverageCostCaption.Appearance.Options.UseForeColor = true;
        lblHeaderAverageCostCaption.Location = new Point(1319, 69);
        lblHeaderAverageCostCaption.Name = "lblHeaderAverageCostCaption";
        lblHeaderAverageCostCaption.Size = new Size(89, 15);
        lblHeaderAverageCostCaption.TabIndex = 40;
        lblHeaderAverageCostCaption.Text = "Costo promedio:";
        // 
        // lblHeaderAverageCostValue
        // 
        lblHeaderAverageCostValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblHeaderAverageCostValue.Appearance.Options.UseFont = true;
        lblHeaderAverageCostValue.Appearance.Options.UseTextOptions = true;
        lblHeaderAverageCostValue.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        lblHeaderAverageCostValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblHeaderAverageCostValue.Location = new Point(1469, 69);
        lblHeaderAverageCostValue.Name = "lblHeaderAverageCostValue";
        lblHeaderAverageCostValue.Size = new Size(100, 15);
        lblHeaderAverageCostValue.TabIndex = 41;
        lblHeaderAverageCostValue.Text = "0.00";
        // 
        // lblHeaderSalesPriceCaption
        // 
        lblHeaderSalesPriceCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblHeaderSalesPriceCaption.Appearance.ForeColor = Color.FromArgb((int)(byte)72, (int)(byte)84, (int)(byte)96);
        lblHeaderSalesPriceCaption.Appearance.Options.UseFont = true;
        lblHeaderSalesPriceCaption.Appearance.Options.UseForeColor = true;
        lblHeaderSalesPriceCaption.Location = new Point(1319, 97);
        lblHeaderSalesPriceCaption.Name = "lblHeaderSalesPriceCaption";
        lblHeaderSalesPriceCaption.Size = new Size(68, 15);
        lblHeaderSalesPriceCaption.TabIndex = 42;
        lblHeaderSalesPriceCaption.Text = "Precio venta:";
        // 
        // lblHeaderSalesPriceValue
        // 
        lblHeaderSalesPriceValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblHeaderSalesPriceValue.Appearance.Options.UseFont = true;
        lblHeaderSalesPriceValue.Appearance.Options.UseTextOptions = true;
        lblHeaderSalesPriceValue.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        lblHeaderSalesPriceValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblHeaderSalesPriceValue.Location = new Point(1469, 97);
        lblHeaderSalesPriceValue.Name = "lblHeaderSalesPriceValue";
        lblHeaderSalesPriceValue.Size = new Size(100, 15);
        lblHeaderSalesPriceValue.TabIndex = 43;
        lblHeaderSalesPriceValue.Text = "0.00";
        // 
        // picItem
        // 
        picItem.Location = new Point(12, 12);
        picItem.Name = "picItem";
        picItem.Properties.Appearance.BackColor = Color.White;
        picItem.Properties.Appearance.Options.UseBackColor = true;
        picItem.Properties.BorderStyle = BorderStyles.Simple;
        picItem.Properties.NullText = "Imagen";
        picItem.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
        picItem.Properties.SizeMode = PictureSizeMode.Zoom;
        picItem.Size = new Size(137, 146);
        picItem.TabIndex = 0;
        // 
        // lblItemCode
        // 
        lblItemCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblItemCode.Appearance.Options.UseFont = true;
        lblItemCode.Location = new Point(192, 41);
        lblItemCode.Name = "lblItemCode";
        lblItemCode.Size = new Size(42, 15);
        lblItemCode.TabIndex = 1;
        lblItemCode.Text = "Código:";
        // 
        // txtItemCode
        // 
        txtItemCode.Location = new Point(332, 38);
        txtItemCode.Name = "txtItemCode";
        txtItemCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtItemCode.Properties.Appearance.Options.UseFont = true;
        txtItemCode.Size = new Size(144, 22);
        txtItemCode.TabIndex = 2;
        // 
        // lblDescription
        // 
        lblDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblDescription.Appearance.Options.UseFont = true;
        lblDescription.Location = new Point(192, 69);
        lblDescription.Name = "lblDescription";
        lblDescription.Size = new Size(65, 15);
        lblDescription.TabIndex = 3;
        lblDescription.Text = "Descripción:";
        // 
        // txtDescription
        // 
        txtDescription.Location = new Point(332, 66);
        txtDescription.Name = "txtDescription";
        txtDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtDescription.Properties.Appearance.Options.UseFont = true;
        txtDescription.Size = new Size(407, 22);
        txtDescription.TabIndex = 4;
        // 
        // lblCommercialName
        // 
        lblCommercialName.Appearance.Font = new Font("Segoe UI", 9F);
        lblCommercialName.Appearance.Options.UseFont = true;
        lblCommercialName.Location = new Point(192, 97);
        lblCommercialName.Name = "lblCommercialName";
        lblCommercialName.Size = new Size(102, 15);
        lblCommercialName.TabIndex = 5;
        lblCommercialName.Text = "Nombre comercial:";
        // 
        // txtCommercialName
        // 
        txtCommercialName.Location = new Point(332, 94);
        txtCommercialName.Name = "txtCommercialName";
        txtCommercialName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCommercialName.Properties.Appearance.Options.UseFont = true;
        txtCommercialName.Size = new Size(407, 22);
        txtCommercialName.TabIndex = 6;
        // 
        // lblItemType
        // 
        lblItemType.Appearance.Font = new Font("Segoe UI", 9F);
        lblItemType.Appearance.Options.UseFont = true;
        lblItemType.Location = new Point(761, 69);
        lblItemType.Name = "lblItemType";
        lblItemType.Size = new Size(54, 15);
        lblItemType.TabIndex = 7;
        lblItemType.Text = "Tipo ítem:";
        // 
        // lueItemType
        // 
        lueItemType.Location = new Point(838, 66);
        lueItemType.Name = "lueItemType";
        lueItemType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueItemType.Properties.Appearance.Options.UseFont = true;
        lueItemType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueItemType.Properties.NullText = "";
        lueItemType.Size = new Size(200, 22);
        lueItemType.TabIndex = 8;
        // 
        // lblItemGroup
        // 
        lblItemGroup.Appearance.Font = new Font("Segoe UI", 9F);
        lblItemGroup.Appearance.Options.UseFont = true;
        lblItemGroup.Location = new Point(192, 138);
        lblItemGroup.Name = "lblItemGroup";
        lblItemGroup.Size = new Size(36, 15);
        lblItemGroup.TabIndex = 9;
        lblItemGroup.Text = "Grupo:";
        // 
        // lueItemGroup
        // 
        lueItemGroup.Location = new Point(234, 135);
        lueItemGroup.Name = "lueItemGroup";
        lueItemGroup.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueItemGroup.Properties.Appearance.Options.UseFont = true;
        lueItemGroup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueItemGroup.Properties.NullText = "";
        lueItemGroup.Size = new Size(220, 22);
        lueItemGroup.TabIndex = 10;
        // 
        // lblItemFamily
        // 
        lblItemFamily.Appearance.Font = new Font("Segoe UI", 9F);
        lblItemFamily.Appearance.Options.UseFont = true;
        lblItemFamily.Location = new Point(499, 138);
        lblItemFamily.Name = "lblItemFamily";
        lblItemFamily.Size = new Size(41, 15);
        lblItemFamily.TabIndex = 11;
        lblItemFamily.Text = "Familia:";
        // 
        // lueItemFamily
        // 
        lueItemFamily.Location = new Point(546, 135);
        lueItemFamily.Name = "lueItemFamily";
        lueItemFamily.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueItemFamily.Properties.Appearance.Options.UseFont = true;
        lueItemFamily.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueItemFamily.Properties.NullText = "";
        lueItemFamily.Size = new Size(215, 22);
        lueItemFamily.TabIndex = 12;
        // 
        // lblBrand
        // 
        lblBrand.Appearance.Font = new Font("Segoe UI", 9F);
        lblBrand.Appearance.Options.UseFont = true;
        lblBrand.Location = new Point(818, 138);
        lblBrand.Name = "lblBrand";
        lblBrand.Size = new Size(36, 15);
        lblBrand.TabIndex = 13;
        lblBrand.Text = "Marca:";
        // 
        // lueBrand
        // 
        lueBrand.Location = new Point(860, 135);
        lueBrand.Name = "lueBrand";
        lueBrand.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBrand.Properties.Appearance.Options.UseFont = true;
        lueBrand.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueBrand.Properties.NullText = "";
        lueBrand.Size = new Size(178, 22);
        lueBrand.TabIndex = 14;
        // 
        // lblBaseUnit
        // 
        lblBaseUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblBaseUnit.Appearance.Options.UseFont = true;
        lblBaseUnit.Location = new Point(761, 97);
        lblBaseUnit.Name = "lblBaseUnit";
        lblBaseUnit.Size = new Size(68, 15);
        lblBaseUnit.TabIndex = 15;
        lblBaseUnit.Text = "Unidad base:";
        // 
        // lueBaseUnit
        // 
        lueBaseUnit.Location = new Point(838, 94);
        lueBaseUnit.Name = "lueBaseUnit";
        lueBaseUnit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBaseUnit.Properties.Appearance.Options.UseFont = true;
        lueBaseUnit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueBaseUnit.Properties.NullText = "";
        lueBaseUnit.Size = new Size(200, 22);
        lueBaseUnit.TabIndex = 16;
        // 
        // lblStatusCaption
        // 
        lblStatusCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblStatusCaption.Appearance.Options.UseFont = true;
        lblStatusCaption.Location = new Point(1065, 138);
        lblStatusCaption.Name = "lblStatusCaption";
        lblStatusCaption.Size = new Size(38, 15);
        lblStatusCaption.TabIndex = 17;
        lblStatusCaption.Text = "Estado:";
        // 
        // lblStatus
        // 
        lblStatus.Appearance.BackColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblStatus.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblStatus.Appearance.ForeColor = Color.White;
        lblStatus.Appearance.Options.UseBackColor = true;
        lblStatus.Appearance.Options.UseFont = true;
        lblStatus.Appearance.Options.UseForeColor = true;
        lblStatus.Appearance.Options.UseTextOptions = true;
        lblStatus.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        lblStatus.AutoSizeMode = LabelAutoSizeMode.None;
        lblStatus.BorderStyle = BorderStyles.NoBorder;
        lblStatus.Location = new Point(1121, 132);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(141, 26);
        lblStatus.TabIndex = 18;
        lblStatus.Text = "Activo";
        // 
        // lblUnsavedIndicator
        // 
        lblUnsavedIndicator.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblUnsavedIndicator.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)86, (int)(byte)210);
        lblUnsavedIndicator.Appearance.Options.UseFont = true;
        lblUnsavedIndicator.Appearance.Options.UseForeColor = true;
        lblUnsavedIndicator.Location = new Point(1319, 126);
        lblUnsavedIndicator.Name = "lblUnsavedIndicator";
        lblUnsavedIndicator.Size = new Size(118, 15);
        lblUnsavedIndicator.TabIndex = 29;
        lblUnsavedIndicator.Text = "● Cambios sin guardar";
        lblUnsavedIndicator.Visible = false;
        // 
        // lblValidationIndicator
        // 
        lblValidationIndicator.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblValidationIndicator.Appearance.ForeColor = Color.FromArgb((int)(byte)220, (int)(byte)38, (int)(byte)38);
        lblValidationIndicator.Appearance.Options.UseFont = true;
        lblValidationIndicator.Appearance.Options.UseForeColor = true;
        lblValidationIndicator.Location = new Point(1319, 142);
        lblValidationIndicator.Name = "lblValidationIndicator";
        lblValidationIndicator.Size = new Size(100, 15);
        lblValidationIndicator.TabIndex = 30;
        lblValidationIndicator.Text = "● Revise los errores";
        lblValidationIndicator.Visible = false;
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
        lblPresentationBarcodesTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
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
        tabRemarks.Appearance.PageClient.BackColor = Color.White;
        tabRemarks.Appearance.PageClient.BackColor2 = Color.White;
        tabRemarks.Appearance.PageClient.Options.UseBackColor = true;
        tabRemarks.BackColor = Color.White;
        tabRemarks.Controls.Add(lblNotesGeneralTitle);
        tabRemarks.Controls.Add(lblGeneralNotes);
        tabRemarks.Controls.Add(memGeneralNotes);
        tabRemarks.Controls.Add(lblGeneralOperationalAlert);
        tabRemarks.Controls.Add(memGeneralOperationalAlert);
        tabRemarks.Controls.Add(lblNotePriority);
        tabRemarks.Controls.Add(lueNotePriority);
        tabRemarks.Controls.Add(lblNoteVisibility);
        tabRemarks.Controls.Add(lueNoteVisibility);
        tabRemarks.Controls.Add(lblGeneralNoteActive);
        tabRemarks.Controls.Add(chkGeneralNoteActive);
        tabRemarks.Controls.Add(sepNotesGeneralTitle);
        tabRemarks.Controls.Add(lblNotesProcessTitle);
        tabRemarks.Controls.Add(lblPurchaseNotes);
        tabRemarks.Controls.Add(memPurchaseNotes);
        tabRemarks.Controls.Add(lblSalesNotes);
        tabRemarks.Controls.Add(memSalesNotes);
        tabRemarks.Controls.Add(lblInventoryNotes);
        tabRemarks.Controls.Add(memInventoryNotes);
        tabRemarks.Controls.Add(lblLogisticsQualityNotes);
        tabRemarks.Controls.Add(memLogisticsQualityNotes);
        tabRemarks.Controls.Add(sepNotesProcessTitle);
        tabRemarks.Controls.Add(lblNotesAlertsTitle);
        tabRemarks.Controls.Add(grdOperationalAlerts);
        tabRemarks.Controls.Add(btnAddOperationalAlert);
        tabRemarks.Controls.Add(btnUpdateOperationalAlert);
        tabRemarks.Controls.Add(btnRemoveOperationalAlert);
        tabRemarks.Controls.Add(btnClearOperationalAlert);
        tabRemarks.Controls.Add(sepNotesAlertsTitle);
        tabRemarks.Controls.Add(sepRemarksColumn);
        tabRemarks.Name = "tabRemarks";
        tabRemarks.Size = new Size(1404, 399);
        tabRemarks.Text = "Observaciones";
        tabRemarks.Paint += DocumentsTabPagePaint;
        // 
        // lblNotesGeneralTitle
        // 
        lblNotesGeneralTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblNotesGeneralTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblNotesGeneralTitle.Appearance.Options.UseFont = true;
        lblNotesGeneralTitle.Appearance.Options.UseForeColor = true;
        lblNotesGeneralTitle.Location = new Point(18, 10);
        lblNotesGeneralTitle.Name = "lblNotesGeneralTitle";
        lblNotesGeneralTitle.Size = new Size(155, 20);
        lblNotesGeneralTitle.TabIndex = 0;
        lblNotesGeneralTitle.Text = "1. Observación general";
        // 
        // lblGeneralNotes
        // 
        lblGeneralNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblGeneralNotes.Appearance.Options.UseFont = true;
        lblGeneralNotes.Location = new Point(18, 42);
        lblGeneralNotes.Name = "lblGeneralNotes";
        lblGeneralNotes.Size = new Size(133, 15);
        lblGeneralNotes.TabIndex = 1;
        lblGeneralNotes.Text = "Observaciones generales:";
        // 
        // memGeneralNotes
        // 
        memGeneralNotes.EditValue = "Producto de alta rotación. Mantener disponibilidad mínima en bodegas principales. Revisar calidad visual del empaque en recepción.";
        memGeneralNotes.Location = new Point(18, 62);
        memGeneralNotes.Name = "memGeneralNotes";
        memGeneralNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memGeneralNotes.Properties.Appearance.Options.UseFont = true;
        memGeneralNotes.Size = new Size(390, 58);
        memGeneralNotes.TabIndex = 2;
        // 
        // lblGeneralOperationalAlert
        // 
        lblGeneralOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        lblGeneralOperationalAlert.Appearance.Options.UseFont = true;
        lblGeneralOperationalAlert.Location = new Point(18, 128);
        lblGeneralOperationalAlert.Name = "lblGeneralOperationalAlert";
        lblGeneralOperationalAlert.Size = new Size(128, 15);
        lblGeneralOperationalAlert.TabIndex = 3;
        lblGeneralOperationalAlert.Text = "Alerta operativa general:";
        // 
        // memGeneralOperationalAlert
        // 
        memGeneralOperationalAlert.EditValue = "Validar lote y fecha de vencimiento antes de despachar.";
        memGeneralOperationalAlert.Location = new Point(18, 148);
        memGeneralOperationalAlert.Name = "memGeneralOperationalAlert";
        memGeneralOperationalAlert.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memGeneralOperationalAlert.Properties.Appearance.Options.UseFont = true;
        memGeneralOperationalAlert.Size = new Size(390, 48);
        memGeneralOperationalAlert.TabIndex = 4;
        // 
        // lblNotePriority
        // 
        lblNotePriority.Appearance.Font = new Font("Segoe UI", 9F);
        lblNotePriority.Appearance.Options.UseFont = true;
        lblNotePriority.Location = new Point(18, 218);
        lblNotePriority.Name = "lblNotePriority";
        lblNotePriority.Size = new Size(51, 15);
        lblNotePriority.TabIndex = 5;
        lblNotePriority.Text = "Prioridad:";
        // 
        // lueNotePriority
        // 
        lueNotePriority.EditValue = "Media";
        lueNotePriority.Location = new Point(80, 214);
        lueNotePriority.Name = "lueNotePriority";
        lueNotePriority.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueNotePriority.Properties.Appearance.Options.UseFont = true;
        lueNotePriority.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueNotePriority.Properties.NullText = "";
        lueNotePriority.Size = new Size(94, 22);
        lueNotePriority.TabIndex = 6;
        // 
        // lblNoteVisibility
        // 
        lblNoteVisibility.Appearance.Font = new Font("Segoe UI", 9F);
        lblNoteVisibility.Appearance.Options.UseFont = true;
        lblNoteVisibility.Location = new Point(188, 218);
        lblNoteVisibility.Name = "lblNoteVisibility";
        lblNoteVisibility.Size = new Size(57, 15);
        lblNoteVisibility.TabIndex = 7;
        lblNoteVisibility.Text = "Visibilidad:";
        // 
        // lueNoteVisibility
        // 
        lueNoteVisibility.EditValue = "Internal";
        lueNoteVisibility.Location = new Point(252, 214);
        lueNoteVisibility.Name = "lueNoteVisibility";
        lueNoteVisibility.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueNoteVisibility.Properties.Appearance.Options.UseFont = true;
        lueNoteVisibility.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueNoteVisibility.Properties.NullText = "";
        lueNoteVisibility.Size = new Size(108, 22);
        lueNoteVisibility.TabIndex = 8;
        // 
        // lblGeneralNoteActive
        // 
        lblGeneralNoteActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblGeneralNoteActive.Appearance.Options.UseFont = true;
        lblGeneralNoteActive.Location = new Point(318, 218);
        lblGeneralNoteActive.Name = "lblGeneralNoteActive";
        lblGeneralNoteActive.Size = new Size(43, 15);
        lblGeneralNoteActive.TabIndex = 9;
        lblGeneralNoteActive.Text = "Vigente:";
        // 
        // chkGeneralNoteActive
        // 
        chkGeneralNoteActive.EditValue = true;
        chkGeneralNoteActive.Location = new Point(365, 214);
        chkGeneralNoteActive.Name = "chkGeneralNoteActive";
        chkGeneralNoteActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkGeneralNoteActive.Properties.Appearance.Options.UseFont = true;
        chkGeneralNoteActive.Properties.OffText = "No";
        chkGeneralNoteActive.Properties.OnText = "Sí";
        chkGeneralNoteActive.Size = new Size(60, 20);
        chkGeneralNoteActive.TabIndex = 10;
        // 
        // sepNotesGeneralTitle
        // 
        sepNotesGeneralTitle.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)230);
        sepNotesGeneralTitle.Appearance.Options.UseBackColor = true;
        sepNotesGeneralTitle.AutoSizeMode = LabelAutoSizeMode.None;
        sepNotesGeneralTitle.Location = new Point(215, 21);
        sepNotesGeneralTitle.Name = "sepNotesGeneralTitle";
        sepNotesGeneralTitle.Size = new Size(195, 1);
        sepNotesGeneralTitle.TabIndex = 11;
        // 
        // lblNotesProcessTitle
        // 
        lblNotesProcessTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblNotesProcessTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblNotesProcessTitle.Appearance.Options.UseFont = true;
        lblNotesProcessTitle.Appearance.Options.UseForeColor = true;
        lblNotesProcessTitle.Location = new Point(463, 10);
        lblNotesProcessTitle.Name = "lblNotesProcessTitle";
        lblNotesProcessTitle.Size = new Size(201, 20);
        lblNotesProcessTitle.TabIndex = 0;
        lblNotesProcessTitle.Text = "2. Observaciones por proceso";
        // 
        // lblPurchaseNotes
        // 
        lblPurchaseNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseNotes.Appearance.Options.UseFont = true;
        lblPurchaseNotes.Location = new Point(465, 46);
        lblPurchaseNotes.Name = "lblPurchaseNotes";
        lblPurchaseNotes.Size = new Size(51, 15);
        lblPurchaseNotes.TabIndex = 1;
        lblPurchaseNotes.Text = "Compras:";
        // 
        // memPurchaseNotes
        // 
        memPurchaseNotes.EditValue = "Comprar solo a proveedor certificado y exigir vida útil mínima de 6 meses al momento de la entrega.";
        memPurchaseNotes.Location = new Point(465, 66);
        memPurchaseNotes.Name = "memPurchaseNotes";
        memPurchaseNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memPurchaseNotes.Properties.Appearance.Options.UseFont = true;
        memPurchaseNotes.Size = new Size(420, 60);
        memPurchaseNotes.TabIndex = 2;
        // 
        // lblSalesNotes
        // 
        lblSalesNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesNotes.Appearance.Options.UseFont = true;
        lblSalesNotes.Location = new Point(931, 46);
        lblSalesNotes.Name = "lblSalesNotes";
        lblSalesNotes.Size = new Size(38, 15);
        lblSalesNotes.TabIndex = 3;
        lblSalesNotes.Text = "Ventas:";
        // 
        // memSalesNotes
        // 
        memSalesNotes.EditValue = "Usar en promociones. No vender productos con vencimiento menor a 30 días.";
        memSalesNotes.Location = new Point(931, 66);
        memSalesNotes.Name = "memSalesNotes";
        memSalesNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memSalesNotes.Properties.Appearance.Options.UseFont = true;
        memSalesNotes.Size = new Size(420, 60);
        memSalesNotes.TabIndex = 4;
        // 
        // lblInventoryNotes
        // 
        lblInventoryNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblInventoryNotes.Appearance.Options.UseFont = true;
        lblInventoryNotes.Location = new Point(465, 138);
        lblInventoryNotes.Name = "lblInventoryNotes";
        lblInventoryNotes.Size = new Size(56, 15);
        lblInventoryNotes.TabIndex = 5;
        lblInventoryNotes.Text = "Inventario:";
        // 
        // memInventoryNotes
        // 
        memInventoryNotes.EditValue = "Almacenar en lugar fresco y seco. Controlar humedad relativa menor al 65%.";
        memInventoryNotes.Location = new Point(465, 158);
        memInventoryNotes.Name = "memInventoryNotes";
        memInventoryNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memInventoryNotes.Properties.Appearance.Options.UseFont = true;
        memInventoryNotes.Size = new Size(420, 60);
        memInventoryNotes.TabIndex = 6;
        // 
        // lblLogisticsQualityNotes
        // 
        lblLogisticsQualityNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblLogisticsQualityNotes.Appearance.Options.UseFont = true;
        lblLogisticsQualityNotes.Location = new Point(931, 138);
        lblLogisticsQualityNotes.Name = "lblLogisticsQualityNotes";
        lblLogisticsQualityNotes.Size = new Size(101, 15);
        lblLogisticsQualityNotes.TabIndex = 7;
        lblLogisticsQualityNotes.Text = "Logística / Calidad:";
        // 
        // memLogisticsQualityNotes
        // 
        memLogisticsQualityNotes.EditValue = "No apilar más de 8 bultos por estiba. Revisar integridad del empaque en cada recepción.";
        memLogisticsQualityNotes.Location = new Point(931, 158);
        memLogisticsQualityNotes.Name = "memLogisticsQualityNotes";
        memLogisticsQualityNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memLogisticsQualityNotes.Properties.Appearance.Options.UseFont = true;
        memLogisticsQualityNotes.Size = new Size(420, 60);
        memLogisticsQualityNotes.TabIndex = 8;
        // 
        // sepNotesProcessTitle
        // 
        sepNotesProcessTitle.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)230);
        sepNotesProcessTitle.Appearance.Options.UseBackColor = true;
        sepNotesProcessTitle.AutoSizeMode = LabelAutoSizeMode.None;
        sepNotesProcessTitle.Location = new Point(680, 21);
        sepNotesProcessTitle.Name = "sepNotesProcessTitle";
        sepNotesProcessTitle.Size = new Size(700, 1);
        sepNotesProcessTitle.TabIndex = 11;
        // 
        // lblNotesAlertsTitle
        // 
        lblNotesAlertsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblNotesAlertsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblNotesAlertsTitle.Appearance.Options.UseFont = true;
        lblNotesAlertsTitle.Appearance.Options.UseForeColor = true;
        lblNotesAlertsTitle.Location = new Point(18, 256);
        lblNotesAlertsTitle.Name = "lblNotesAlertsTitle";
        lblNotesAlertsTitle.Size = new Size(138, 20);
        lblNotesAlertsTitle.TabIndex = 0;
        lblNotesAlertsTitle.Text = "3. Alertas operativas";
        // 
        // grdOperationalAlerts
        // 
        grdOperationalAlerts.DataSource = operationalAlertsTable;
        grdOperationalAlerts.Location = new Point(18, 284);
        grdOperationalAlerts.MainView = gvOperationalAlerts;
        grdOperationalAlerts.Name = "grdOperationalAlerts";
        grdOperationalAlerts.RepositoryItems.AddRange(new RepositoryItem[] { repoOperationalAlertCheck });
        grdOperationalAlerts.Size = new Size(1368, 106);
        grdOperationalAlerts.TabIndex = 1;
        grdOperationalAlerts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvOperationalAlerts, gvOperationalAlertsAux });
        // 
        // gvOperationalAlerts
        // 
        gvOperationalAlerts.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvOperationalAlerts.Appearance.HeaderPanel.Options.UseFont = true;
        gvOperationalAlerts.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvOperationalAlerts.Appearance.Row.Options.UseFont = true;
        gvOperationalAlerts.Columns.AddRange(new GridColumn[] { colOperationalAlertType, colOperationalAlertProcess, colOperationalAlertMessage, colOperationalAlertFrom, colOperationalAlertTo, colOperationalAlertPriority, colOperationalAlertBlocking, colOperationalAlertConfirmation, colOperationalAlertActive });
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
        // colOperationalAlertPriority
        // 
        colOperationalAlertPriority.Caption = "Prioridad";
        colOperationalAlertPriority.FieldName = "Prioridad";
        colOperationalAlertPriority.Name = "colOperationalAlertPriority";
        colOperationalAlertPriority.Visible = true;
        colOperationalAlertPriority.VisibleIndex = 5;
        colOperationalAlertPriority.Width = 80;
        // 
        // colOperationalAlertBlocking
        // 
        colOperationalAlertBlocking.Caption = "Bloqueante";
        colOperationalAlertBlocking.ColumnEdit = repoOperationalAlertCheck;
        colOperationalAlertBlocking.FieldName = "Bloqueante";
        colOperationalAlertBlocking.Name = "colOperationalAlertBlocking";
        colOperationalAlertBlocking.Visible = true;
        colOperationalAlertBlocking.VisibleIndex = 6;
        colOperationalAlertBlocking.Width = 90;
        // 
        // repoOperationalAlertCheck
        // 
        repoOperationalAlertCheck.AutoHeight = false;
        repoOperationalAlertCheck.Name = "repoOperationalAlertCheck";
        // 
        // colOperationalAlertConfirmation
        // 
        colOperationalAlertConfirmation.Caption = "Confirmación";
        colOperationalAlertConfirmation.ColumnEdit = repoOperationalAlertCheck;
        colOperationalAlertConfirmation.FieldName = "Confirmacion";
        colOperationalAlertConfirmation.Name = "colOperationalAlertConfirmation";
        colOperationalAlertConfirmation.Visible = true;
        colOperationalAlertConfirmation.VisibleIndex = 8;
        colOperationalAlertConfirmation.Width = 90;
        // 
        // colOperationalAlertActive
        // 
        colOperationalAlertActive.Caption = "Activa";
        colOperationalAlertActive.ColumnEdit = repoOperationalAlertCheck;
        colOperationalAlertActive.FieldName = "Activa";
        colOperationalAlertActive.Name = "colOperationalAlertActive";
        colOperationalAlertActive.Visible = true;
        colOperationalAlertActive.VisibleIndex = 7;
        colOperationalAlertActive.Width = 70;
        // 
        // gvOperationalAlertsAux
        // 
        gvOperationalAlertsAux.GridControl = grdOperationalAlerts;
        gvOperationalAlertsAux.Name = "gvOperationalAlertsAux";
        // 
        // btnAddOperationalAlert
        // 
        btnAddOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        btnAddOperationalAlert.Appearance.Options.UseFont = true;
        btnAddOperationalAlert.Location = new Point(998, 253);
        btnAddOperationalAlert.Name = "btnAddOperationalAlert";
        btnAddOperationalAlert.Size = new Size(96, 28);
        btnAddOperationalAlert.TabIndex = 2;
        btnAddOperationalAlert.Text = "Agregar";
        // 
        // btnUpdateOperationalAlert
        // 
        btnUpdateOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        btnUpdateOperationalAlert.Appearance.Options.UseFont = true;
        btnUpdateOperationalAlert.Location = new Point(1102, 253);
        btnUpdateOperationalAlert.Name = "btnUpdateOperationalAlert";
        btnUpdateOperationalAlert.Size = new Size(84, 28);
        btnUpdateOperationalAlert.TabIndex = 3;
        btnUpdateOperationalAlert.Text = "Editar";
        // 
        // btnRemoveOperationalAlert
        // 
        btnRemoveOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        btnRemoveOperationalAlert.Appearance.Options.UseFont = true;
        btnRemoveOperationalAlert.Location = new Point(1194, 253);
        btnRemoveOperationalAlert.Name = "btnRemoveOperationalAlert";
        btnRemoveOperationalAlert.Size = new Size(92, 28);
        btnRemoveOperationalAlert.TabIndex = 4;
        btnRemoveOperationalAlert.Text = "Quitar";
        // 
        // btnClearOperationalAlert
        // 
        btnClearOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        btnClearOperationalAlert.Appearance.Options.UseFont = true;
        btnClearOperationalAlert.Location = new Point(1292, 253);
        btnClearOperationalAlert.Name = "btnClearOperationalAlert";
        btnClearOperationalAlert.Size = new Size(92, 28);
        btnClearOperationalAlert.TabIndex = 5;
        btnClearOperationalAlert.Text = "Limpiar";
        // 
        // sepNotesAlertsTitle
        // 
        sepNotesAlertsTitle.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)230);
        sepNotesAlertsTitle.Appearance.Options.UseBackColor = true;
        sepNotesAlertsTitle.AutoSizeMode = LabelAutoSizeMode.None;
        sepNotesAlertsTitle.Location = new Point(170, 267);
        sepNotesAlertsTitle.Name = "sepNotesAlertsTitle";
        sepNotesAlertsTitle.Size = new Size(790, 1);
        sepNotesAlertsTitle.TabIndex = 6;
        // 
        // sepRemarksColumn
        // 
        sepRemarksColumn.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)230);
        sepRemarksColumn.Appearance.Options.UseBackColor = true;
        sepRemarksColumn.AutoSizeMode = LabelAutoSizeMode.None;
        sepRemarksColumn.Location = new Point(430, 8);
        sepRemarksColumn.Name = "sepRemarksColumn";
        sepRemarksColumn.Size = new Size(1, 232);
        sepRemarksColumn.TabIndex = 10;
        // 
        // tabAttachments
        // 
        tabAttachments.Appearance.PageClient.BackColor = Color.White;
        tabAttachments.Appearance.PageClient.BackColor2 = Color.White;
        tabAttachments.Appearance.PageClient.Options.UseBackColor = true;
        tabAttachments.BackColor = Color.White;
        tabAttachments.Controls.Add(lblAttachmentGridTitle);
        tabAttachments.Controls.Add(grdAttachments);
        tabAttachments.Controls.Add(btnAddAttachment);
        tabAttachments.Controls.Add(btnUpdateAttachment);
        tabAttachments.Controls.Add(btnRemoveAttachment);
        tabAttachments.Controls.Add(btnDownloadAttachment);
        tabAttachments.Controls.Add(btnOpenAttachment);
        tabAttachments.Controls.Add(btnSetMainAttachment);
        tabAttachments.Controls.Add(lblAttachmentMetadataTitle);
        tabAttachments.Controls.Add(lblAttachmentPublicationTitle);
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
        tabAttachments.Controls.Add(lblAttachmentReference);
        tabAttachments.Controls.Add(lblAttachmentPrincipal);
        tabAttachments.Controls.Add(lblAttachmentVisibleSales);
        tabAttachments.Controls.Add(lblAttachmentVisiblePurchases);
        tabAttachments.Controls.Add(lblAttachmentVisiblePortal);
        tabAttachments.Controls.Add(lblAttachmentConfidential);
        tabAttachments.Controls.Add(txtAttachmentReference);
        tabAttachments.Controls.Add(chkAttachmentPrincipal);
        tabAttachments.Controls.Add(chkAttachmentConfidential);
        tabAttachments.Controls.Add(lblAttachmentOrder);
        tabAttachments.Controls.Add(spnAttachmentOrder);
        tabAttachments.Controls.Add(lblAttachmentValidFrom);
        tabAttachments.Controls.Add(dteAttachmentValidFrom);
        tabAttachments.Controls.Add(lblAttachmentValidTo);
        tabAttachments.Controls.Add(dteAttachmentValidTo);
        tabAttachments.Controls.Add(lblAttachmentAlternativeText);
        tabAttachments.Controls.Add(memAttachmentAlternativeText);
        tabAttachments.Controls.Add(sepDocumentsColumnOne);
        tabAttachments.Controls.Add(sepDocumentsColumnTwo);
        tabAttachments.Controls.Add(sepAttachmentPreviewTitle);
        tabAttachments.Controls.Add(sepAttachmentMetadataTitle);
        tabAttachments.Controls.Add(sepAttachmentPublicationTitle);
        tabAttachments.Controls.Add(sepAttachmentGridTitle);
        tabAttachments.Controls.Add(lblAttachmentPreviewTitle);
        tabAttachments.Controls.Add(picMainAttachmentPreview);
        tabAttachments.Controls.Add(btnLoadImage);
        tabAttachments.Controls.Add(btnRemoveImage);
        tabAttachments.Controls.Add(btnPreviewImage);
        tabAttachments.Controls.Add(btnSetMainImage);
        tabAttachments.Controls.Add(lblAttachmentPreviewNoteIcon);
        tabAttachments.Controls.Add(lblAttachmentPreviewNote);
        tabAttachments.Name = "tabAttachments";
        tabAttachments.Size = new Size(1404, 399);
        tabAttachments.Text = "Imágenes / Anexos";
        tabAttachments.Paint += DocumentsTabPagePaint;
        // 
        // lblAttachmentGridTitle
        // 
        lblAttachmentGridTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAttachmentGridTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblAttachmentGridTitle.Appearance.Options.UseFont = true;
        lblAttachmentGridTitle.Appearance.Options.UseForeColor = true;
        lblAttachmentGridTitle.Location = new Point(18, 286);
        lblAttachmentGridTitle.Name = "lblAttachmentGridTitle";
        lblAttachmentGridTitle.Size = new Size(156, 20);
        lblAttachmentGridTitle.TabIndex = 44;
        lblAttachmentGridTitle.Text = "4. Archivos registrados";
        // 
        // grdAttachments
        // 
        grdAttachments.DataSource = attachmentsTable;
        grdAttachments.Location = new Point(18, 314);
        grdAttachments.MainView = gvAttachments;
        grdAttachments.Name = "grdAttachments";
        grdAttachments.RepositoryItems.AddRange(new RepositoryItem[] { repoAttachmentCheck });
        grdAttachments.Size = new Size(1368, 82);
        grdAttachments.TabIndex = 45;
        grdAttachments.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvAttachments });
        // 
        // gvAttachments
        // 
        gvAttachments.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvAttachments.Appearance.HeaderPanel.Options.UseFont = true;
        gvAttachments.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvAttachments.Appearance.Row.Options.UseFont = true;
        gvAttachments.Columns.AddRange(new GridColumn[] { colAttachmentDocumentType, colAttachmentFileName, colAttachmentDescription, colAttachmentCategory, colAttachmentExtension, colAttachmentSize, colAttachmentDate, colAttachmentUser, colAttachmentPrincipal, colAttachmentVisibleSales, colAttachmentVisiblePurchases, colAttachmentVisiblePortal, colAttachmentStatus });
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
        // colAttachmentCategory
        // 
        colAttachmentCategory.Caption = "Categoría";
        colAttachmentCategory.FieldName = "Categoria";
        colAttachmentCategory.Name = "colAttachmentCategory";
        colAttachmentCategory.Visible = true;
        colAttachmentCategory.VisibleIndex = 3;
        // 
        // colAttachmentExtension
        // 
        colAttachmentExtension.Caption = "Ext.";
        colAttachmentExtension.FieldName = "Extension";
        colAttachmentExtension.Name = "colAttachmentExtension";
        colAttachmentExtension.Visible = true;
        colAttachmentExtension.VisibleIndex = 4;
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
        colAttachmentSize.VisibleIndex = 5;
        colAttachmentSize.Width = 55;
        // 
        // colAttachmentDate
        // 
        colAttachmentDate.Caption = "Fecha";
        colAttachmentDate.FieldName = "Fecha";
        colAttachmentDate.Name = "colAttachmentDate";
        colAttachmentDate.Visible = true;
        colAttachmentDate.VisibleIndex = 6;
        colAttachmentDate.Width = 80;
        // 
        // colAttachmentUser
        // 
        colAttachmentUser.Caption = "Usuario";
        colAttachmentUser.FieldName = "Usuario";
        colAttachmentUser.Name = "colAttachmentUser";
        colAttachmentUser.Visible = true;
        colAttachmentUser.VisibleIndex = 7;
        colAttachmentUser.Width = 70;
        // 
        // colAttachmentPrincipal
        // 
        colAttachmentPrincipal.Caption = "Principal";
        colAttachmentPrincipal.ColumnEdit = repoAttachmentCheck;
        colAttachmentPrincipal.FieldName = "Principal";
        colAttachmentPrincipal.Name = "colAttachmentPrincipal";
        colAttachmentPrincipal.Visible = true;
        colAttachmentPrincipal.VisibleIndex = 8;
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
        colAttachmentVisibleSales.VisibleIndex = 9;
        colAttachmentVisibleSales.Width = 50;
        // 
        // colAttachmentVisiblePurchases
        // 
        colAttachmentVisiblePurchases.Caption = "Compras";
        colAttachmentVisiblePurchases.ColumnEdit = repoAttachmentCheck;
        colAttachmentVisiblePurchases.FieldName = "VisibleCompras";
        colAttachmentVisiblePurchases.Name = "colAttachmentVisiblePurchases";
        colAttachmentVisiblePurchases.Visible = true;
        colAttachmentVisiblePurchases.VisibleIndex = 10;
        colAttachmentVisiblePurchases.Width = 58;
        // 
        // colAttachmentVisiblePortal
        // 
        colAttachmentVisiblePortal.Caption = "Portal";
        colAttachmentVisiblePortal.ColumnEdit = repoAttachmentCheck;
        colAttachmentVisiblePortal.FieldName = "VisiblePortal";
        colAttachmentVisiblePortal.Name = "colAttachmentVisiblePortal";
        colAttachmentVisiblePortal.Visible = true;
        colAttachmentVisiblePortal.VisibleIndex = 12;
        colAttachmentVisiblePortal.Width = 50;
        // 
        // colAttachmentStatus
        // 
        colAttachmentStatus.Caption = "Estado";
        colAttachmentStatus.FieldName = "Estado";
        colAttachmentStatus.Name = "colAttachmentStatus";
        colAttachmentStatus.Visible = true;
        colAttachmentStatus.VisibleIndex = 11;
        colAttachmentStatus.Width = 70;
        // 
        // btnAddAttachment
        // 
        btnAddAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnAddAttachment.Appearance.Options.UseFont = true;
        btnAddAttachment.Location = new Point(854, 283);
        btnAddAttachment.Name = "btnAddAttachment";
        btnAddAttachment.Size = new Size(78, 28);
        btnAddAttachment.TabIndex = 46;
        btnAddAttachment.Text = "Agregar";
        // 
        // btnUpdateAttachment
        // 
        btnUpdateAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnUpdateAttachment.Appearance.Options.UseFont = true;
        btnUpdateAttachment.Location = new Point(940, 283);
        btnUpdateAttachment.Name = "btnUpdateAttachment";
        btnUpdateAttachment.Size = new Size(72, 28);
        btnUpdateAttachment.TabIndex = 47;
        btnUpdateAttachment.Text = "Editar";
        // 
        // btnRemoveAttachment
        // 
        btnRemoveAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnRemoveAttachment.Appearance.Options.UseFont = true;
        btnRemoveAttachment.Location = new Point(1018, 283);
        btnRemoveAttachment.Name = "btnRemoveAttachment";
        btnRemoveAttachment.Size = new Size(72, 28);
        btnRemoveAttachment.TabIndex = 48;
        btnRemoveAttachment.Text = "Quitar";
        // 
        // btnDownloadAttachment
        // 
        btnDownloadAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnDownloadAttachment.Appearance.Options.UseFont = true;
        btnDownloadAttachment.Location = new Point(1222, 283);
        btnDownloadAttachment.Name = "btnDownloadAttachment";
        btnDownloadAttachment.Size = new Size(86, 28);
        btnDownloadAttachment.TabIndex = 49;
        btnDownloadAttachment.Text = "Descargar";
        // 
        // btnOpenAttachment
        // 
        btnOpenAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnOpenAttachment.Appearance.Options.UseFont = true;
        btnOpenAttachment.Location = new Point(1316, 283);
        btnOpenAttachment.Name = "btnOpenAttachment";
        btnOpenAttachment.Size = new Size(70, 28);
        btnOpenAttachment.TabIndex = 50;
        btnOpenAttachment.Text = "Abrir";
        // 
        // btnSetMainAttachment
        // 
        btnSetMainAttachment.Appearance.Font = new Font("Segoe UI", 9F);
        btnSetMainAttachment.Appearance.Options.UseFont = true;
        btnSetMainAttachment.Location = new Point(1096, 283);
        btnSetMainAttachment.Name = "btnSetMainAttachment";
        btnSetMainAttachment.Size = new Size(112, 28);
        btnSetMainAttachment.TabIndex = 51;
        btnSetMainAttachment.Text = "Marcar principal";
        // 
        // lblAttachmentMetadataTitle
        // 
        lblAttachmentMetadataTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAttachmentMetadataTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblAttachmentMetadataTitle.Appearance.Options.UseFont = true;
        lblAttachmentMetadataTitle.Appearance.Options.UseForeColor = true;
        lblAttachmentMetadataTitle.Location = new Point(491, 12);
        lblAttachmentMetadataTitle.Name = "lblAttachmentMetadataTitle";
        lblAttachmentMetadataTitle.Size = new Size(135, 20);
        lblAttachmentMetadataTitle.TabIndex = 22;
        lblAttachmentMetadataTitle.Text = "2. Datos del archivo";
        // 
        // lblAttachmentPublicationTitle
        // 
        lblAttachmentPublicationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAttachmentPublicationTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblAttachmentPublicationTitle.Appearance.Options.UseFont = true;
        lblAttachmentPublicationTitle.Appearance.Options.UseForeColor = true;
        lblAttachmentPublicationTitle.Location = new Point(926, 12);
        lblAttachmentPublicationTitle.Name = "lblAttachmentPublicationTitle";
        lblAttachmentPublicationTitle.Size = new Size(160, 20);
        lblAttachmentPublicationTitle.TabIndex = 58;
        lblAttachmentPublicationTitle.Text = "3. Publicación y control";
        // 
        // lblAttachmentType
        // 
        lblAttachmentType.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentType.Appearance.Options.UseFont = true;
        lblAttachmentType.Location = new Point(490, 48);
        lblAttachmentType.Name = "lblAttachmentType";
        lblAttachmentType.Size = new Size(92, 15);
        lblAttachmentType.TabIndex = 23;
        lblAttachmentType.Text = "Tipo documento:";
        // 
        // lueAttachmentType
        // 
        lueAttachmentType.EditValue = "Imagen producto";
        lueAttachmentType.Location = new Point(621, 44);
        lueAttachmentType.Name = "lueAttachmentType";
        lueAttachmentType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAttachmentType.Properties.Appearance.Options.UseFont = true;
        lueAttachmentType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAttachmentType.Properties.NullText = "";
        lueAttachmentType.Size = new Size(273, 22);
        lueAttachmentType.TabIndex = 24;
        // 
        // lblAttachmentFileName
        // 
        lblAttachmentFileName.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentFileName.Appearance.Options.UseFont = true;
        lblAttachmentFileName.Location = new Point(490, 104);
        lblAttachmentFileName.Name = "lblAttachmentFileName";
        lblAttachmentFileName.Size = new Size(89, 15);
        lblAttachmentFileName.TabIndex = 25;
        lblAttachmentFileName.Text = "Nombre archivo:";
        // 
        // txtAttachmentFileName
        // 
        txtAttachmentFileName.EditValue = "arroz_1kg_frontal.png";
        txtAttachmentFileName.Location = new Point(621, 100);
        txtAttachmentFileName.Name = "txtAttachmentFileName";
        txtAttachmentFileName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentFileName.Properties.Appearance.Options.UseFont = true;
        txtAttachmentFileName.Size = new Size(273, 22);
        txtAttachmentFileName.TabIndex = 26;
        // 
        // lblAttachmentDescription
        // 
        lblAttachmentDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentDescription.Appearance.Options.UseFont = true;
        lblAttachmentDescription.Location = new Point(490, 132);
        lblAttachmentDescription.Name = "lblAttachmentDescription";
        lblAttachmentDescription.Size = new Size(65, 15);
        lblAttachmentDescription.TabIndex = 27;
        lblAttachmentDescription.Text = "Descripción:";
        // 
        // memAttachmentDescription
        // 
        memAttachmentDescription.EditValue = "Imagen principal frontal del producto";
        memAttachmentDescription.Location = new Point(621, 128);
        memAttachmentDescription.Name = "memAttachmentDescription";
        memAttachmentDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memAttachmentDescription.Properties.Appearance.Options.UseFont = true;
        memAttachmentDescription.Size = new Size(271, 48);
        memAttachmentDescription.TabIndex = 28;
        // 
        // lblAttachmentCategory
        // 
        lblAttachmentCategory.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentCategory.Appearance.Options.UseFont = true;
        lblAttachmentCategory.Location = new Point(490, 76);
        lblAttachmentCategory.Name = "lblAttachmentCategory";
        lblAttachmentCategory.Size = new Size(54, 15);
        lblAttachmentCategory.TabIndex = 29;
        lblAttachmentCategory.Text = "Categoría:";
        // 
        // lueAttachmentCategory
        // 
        lueAttachmentCategory.EditValue = "Comercial";
        lueAttachmentCategory.Location = new Point(621, 72);
        lueAttachmentCategory.Name = "lueAttachmentCategory";
        lueAttachmentCategory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAttachmentCategory.Properties.Appearance.Options.UseFont = true;
        lueAttachmentCategory.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAttachmentCategory.Properties.NullText = "";
        lueAttachmentCategory.Size = new Size(273, 22);
        lueAttachmentCategory.TabIndex = 30;
        // 
        // chkVisibleInSales
        // 
        chkVisibleInSales.EditValue = true;
        chkVisibleInSales.Location = new Point(1050, 70);
        chkVisibleInSales.Name = "chkVisibleInSales";
        chkVisibleInSales.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkVisibleInSales.Properties.Appearance.Options.UseFont = true;
        chkVisibleInSales.Properties.OffText = "No";
        chkVisibleInSales.Properties.OnText = "Sí";
        chkVisibleInSales.Size = new Size(86, 20);
        chkVisibleInSales.TabIndex = 31;
        // 
        // chkVisibleInPurchases
        // 
        chkVisibleInPurchases.Location = new Point(1050, 98);
        chkVisibleInPurchases.Name = "chkVisibleInPurchases";
        chkVisibleInPurchases.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkVisibleInPurchases.Properties.Appearance.Options.UseFont = true;
        chkVisibleInPurchases.Properties.OffText = "No";
        chkVisibleInPurchases.Properties.OnText = "Sí";
        chkVisibleInPurchases.Size = new Size(86, 20);
        chkVisibleInPurchases.TabIndex = 32;
        // 
        // chkVisibleInPortal
        // 
        chkVisibleInPortal.EditValue = true;
        chkVisibleInPortal.Location = new Point(1292, 70);
        chkVisibleInPortal.Name = "chkVisibleInPortal";
        chkVisibleInPortal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkVisibleInPortal.Properties.Appearance.Options.UseFont = true;
        chkVisibleInPortal.Properties.OffText = "No";
        chkVisibleInPortal.Properties.OnText = "Sí";
        chkVisibleInPortal.Size = new Size(86, 20);
        chkVisibleInPortal.TabIndex = 33;
        // 
        // lblAttachmentStatus
        // 
        lblAttachmentStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentStatus.Appearance.Options.UseFont = true;
        lblAttachmentStatus.Location = new Point(490, 215);
        lblAttachmentStatus.Name = "lblAttachmentStatus";
        lblAttachmentStatus.Size = new Size(38, 15);
        lblAttachmentStatus.TabIndex = 34;
        lblAttachmentStatus.Text = "Estado:";
        // 
        // lueAttachmentStatus
        // 
        lueAttachmentStatus.EditValue = "Activo";
        lueAttachmentStatus.Location = new Point(621, 212);
        lueAttachmentStatus.Name = "lueAttachmentStatus";
        lueAttachmentStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAttachmentStatus.Properties.Appearance.Options.UseFont = true;
        lueAttachmentStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAttachmentStatus.Properties.NullText = "";
        lueAttachmentStatus.Size = new Size(271, 22);
        lueAttachmentStatus.TabIndex = 35;
        // 
        // lblAttachmentReference
        // 
        lblAttachmentReference.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentReference.Appearance.Options.UseFont = true;
        lblAttachmentReference.Location = new Point(490, 188);
        lblAttachmentReference.Name = "lblAttachmentReference";
        lblAttachmentReference.Size = new Size(125, 15);
        lblAttachmentReference.TabIndex = 44;
        lblAttachmentReference.Text = "Referencia documental:";
        // 
        // lblAttachmentPrincipal
        // 
        lblAttachmentPrincipal.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentPrincipal.Appearance.Options.UseFont = true;
        lblAttachmentPrincipal.Location = new Point(926, 46);
        lblAttachmentPrincipal.Name = "lblAttachmentPrincipal";
        lblAttachmentPrincipal.Size = new Size(49, 15);
        lblAttachmentPrincipal.TabIndex = 59;
        lblAttachmentPrincipal.Text = "Principal:";
        // 
        // lblAttachmentVisibleSales
        // 
        lblAttachmentVisibleSales.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentVisibleSales.Appearance.Options.UseFont = true;
        lblAttachmentVisibleSales.Location = new Point(926, 74);
        lblAttachmentVisibleSales.Name = "lblAttachmentVisibleSales";
        lblAttachmentVisibleSales.Size = new Size(90, 15);
        lblAttachmentVisibleSales.TabIndex = 60;
        lblAttachmentVisibleSales.Text = "Visible en ventas:";
        // 
        // lblAttachmentVisiblePurchases
        // 
        lblAttachmentVisiblePurchases.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentVisiblePurchases.Appearance.Options.UseFont = true;
        lblAttachmentVisiblePurchases.Location = new Point(926, 102);
        lblAttachmentVisiblePurchases.Name = "lblAttachmentVisiblePurchases";
        lblAttachmentVisiblePurchases.Size = new Size(102, 15);
        lblAttachmentVisiblePurchases.TabIndex = 61;
        lblAttachmentVisiblePurchases.Text = "Visible en compras:";
        // 
        // lblAttachmentVisiblePortal
        // 
        lblAttachmentVisiblePortal.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentVisiblePortal.Appearance.Options.UseFont = true;
        lblAttachmentVisiblePortal.Location = new Point(1190, 74);
        lblAttachmentVisiblePortal.Name = "lblAttachmentVisiblePortal";
        lblAttachmentVisiblePortal.Size = new Size(87, 15);
        lblAttachmentVisiblePortal.TabIndex = 62;
        lblAttachmentVisiblePortal.Text = "Visible en portal:";
        // 
        // lblAttachmentConfidential
        // 
        lblAttachmentConfidential.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentConfidential.Appearance.Options.UseFont = true;
        lblAttachmentConfidential.Location = new Point(1190, 46);
        lblAttachmentConfidential.Name = "lblAttachmentConfidential";
        lblAttachmentConfidential.Size = new Size(70, 15);
        lblAttachmentConfidential.TabIndex = 63;
        lblAttachmentConfidential.Text = "Confidencial:";
        // 
        // txtAttachmentReference
        // 
        txtAttachmentReference.Location = new Point(621, 184);
        txtAttachmentReference.Name = "txtAttachmentReference";
        txtAttachmentReference.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentReference.Properties.Appearance.Options.UseFont = true;
        txtAttachmentReference.Size = new Size(271, 22);
        txtAttachmentReference.TabIndex = 45;
        // 
        // chkAttachmentPrincipal
        // 
        chkAttachmentPrincipal.Location = new Point(1050, 42);
        chkAttachmentPrincipal.Name = "chkAttachmentPrincipal";
        chkAttachmentPrincipal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkAttachmentPrincipal.Properties.Appearance.Options.UseFont = true;
        chkAttachmentPrincipal.Properties.OffText = "No";
        chkAttachmentPrincipal.Properties.OnText = "Sí";
        chkAttachmentPrincipal.Size = new Size(86, 20);
        chkAttachmentPrincipal.TabIndex = 46;
        // 
        // chkAttachmentConfidential
        // 
        chkAttachmentConfidential.Location = new Point(1292, 42);
        chkAttachmentConfidential.Name = "chkAttachmentConfidential";
        chkAttachmentConfidential.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkAttachmentConfidential.Properties.Appearance.Options.UseFont = true;
        chkAttachmentConfidential.Properties.OffText = "No";
        chkAttachmentConfidential.Properties.OnText = "Sí";
        chkAttachmentConfidential.Size = new Size(86, 20);
        chkAttachmentConfidential.TabIndex = 47;
        // 
        // lblAttachmentOrder
        // 
        lblAttachmentOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentOrder.Appearance.Options.UseFont = true;
        lblAttachmentOrder.Location = new Point(926, 134);
        lblAttachmentOrder.Name = "lblAttachmentOrder";
        lblAttachmentOrder.Size = new Size(69, 15);
        lblAttachmentOrder.TabIndex = 48;
        lblAttachmentOrder.Text = "Orden visual:";
        // 
        // spnAttachmentOrder
        // 
        spnAttachmentOrder.EditValue = new decimal(new int[] { 1, 0, 0, 0 });
        spnAttachmentOrder.Location = new Point(1050, 130);
        spnAttachmentOrder.Name = "spnAttachmentOrder";
        spnAttachmentOrder.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnAttachmentOrder.Properties.IsFloatValue = false;
        spnAttachmentOrder.Properties.MaskSettings.Set("mask", "N00");
        spnAttachmentOrder.Properties.MaxValue = new decimal(new int[] { 9999, 0, 0, 0 });
        spnAttachmentOrder.Size = new Size(167, 20);
        spnAttachmentOrder.TabIndex = 49;
        // 
        // lblAttachmentValidFrom
        // 
        lblAttachmentValidFrom.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentValidFrom.Appearance.Options.UseFont = true;
        lblAttachmentValidFrom.Location = new Point(926, 160);
        lblAttachmentValidFrom.Name = "lblAttachmentValidFrom";
        lblAttachmentValidFrom.Size = new Size(82, 15);
        lblAttachmentValidFrom.TabIndex = 50;
        lblAttachmentValidFrom.Text = "Vigencia desde:";
        // 
        // dteAttachmentValidFrom
        // 
        dteAttachmentValidFrom.EditValue = null;
        dteAttachmentValidFrom.Location = new Point(1050, 156);
        dteAttachmentValidFrom.Name = "dteAttachmentValidFrom";
        dteAttachmentValidFrom.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteAttachmentValidFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteAttachmentValidFrom.Size = new Size(167, 20);
        dteAttachmentValidFrom.TabIndex = 51;
        // 
        // lblAttachmentValidTo
        // 
        lblAttachmentValidTo.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentValidTo.Appearance.Options.UseFont = true;
        lblAttachmentValidTo.Location = new Point(926, 186);
        lblAttachmentValidTo.Name = "lblAttachmentValidTo";
        lblAttachmentValidTo.Size = new Size(79, 15);
        lblAttachmentValidTo.TabIndex = 52;
        lblAttachmentValidTo.Text = "Vigencia hasta:";
        // 
        // dteAttachmentValidTo
        // 
        dteAttachmentValidTo.EditValue = null;
        dteAttachmentValidTo.Location = new Point(1050, 182);
        dteAttachmentValidTo.Name = "dteAttachmentValidTo";
        dteAttachmentValidTo.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteAttachmentValidTo.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteAttachmentValidTo.Size = new Size(167, 20);
        dteAttachmentValidTo.TabIndex = 53;
        // 
        // lblAttachmentAlternativeText
        // 
        lblAttachmentAlternativeText.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentAlternativeText.Appearance.Options.UseFont = true;
        lblAttachmentAlternativeText.Location = new Point(926, 213);
        lblAttachmentAlternativeText.Name = "lblAttachmentAlternativeText";
        lblAttachmentAlternativeText.Size = new Size(91, 15);
        lblAttachmentAlternativeText.TabIndex = 54;
        lblAttachmentAlternativeText.Text = "Texto alternativo:";
        // 
        // memAttachmentAlternativeText
        // 
        memAttachmentAlternativeText.Location = new Point(1050, 208);
        memAttachmentAlternativeText.Name = "memAttachmentAlternativeText";
        memAttachmentAlternativeText.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memAttachmentAlternativeText.Properties.Appearance.Options.UseFont = true;
        memAttachmentAlternativeText.Size = new Size(326, 46);
        memAttachmentAlternativeText.TabIndex = 55;
        // 
        // sepDocumentsColumnOne
        // 
        sepDocumentsColumnOne.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)230);
        sepDocumentsColumnOne.Appearance.Options.UseBackColor = true;
        sepDocumentsColumnOne.AutoSizeMode = LabelAutoSizeMode.None;
        sepDocumentsColumnOne.Location = new Point(472, 10);
        sepDocumentsColumnOne.Name = "sepDocumentsColumnOne";
        sepDocumentsColumnOne.Size = new Size(1, 252);
        sepDocumentsColumnOne.TabIndex = 56;
        // 
        // sepDocumentsColumnTwo
        // 
        sepDocumentsColumnTwo.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)230);
        sepDocumentsColumnTwo.Appearance.Options.UseBackColor = true;
        sepDocumentsColumnTwo.AutoSizeMode = LabelAutoSizeMode.None;
        sepDocumentsColumnTwo.Location = new Point(909, 10);
        sepDocumentsColumnTwo.Name = "sepDocumentsColumnTwo";
        sepDocumentsColumnTwo.Size = new Size(1, 252);
        sepDocumentsColumnTwo.TabIndex = 57;
        // 
        // sepAttachmentPreviewTitle
        // 
        sepAttachmentPreviewTitle.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)230);
        sepAttachmentPreviewTitle.Appearance.Options.UseBackColor = true;
        sepAttachmentPreviewTitle.AutoSizeMode = LabelAutoSizeMode.None;
        sepAttachmentPreviewTitle.Location = new Point(160, 23);
        sepAttachmentPreviewTitle.Name = "sepAttachmentPreviewTitle";
        sepAttachmentPreviewTitle.Size = new Size(250, 1);
        sepAttachmentPreviewTitle.TabIndex = 59;
        // 
        // sepAttachmentMetadataTitle
        // 
        sepAttachmentMetadataTitle.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)230);
        sepAttachmentMetadataTitle.Appearance.Options.UseBackColor = true;
        sepAttachmentMetadataTitle.AutoSizeMode = LabelAutoSizeMode.None;
        sepAttachmentMetadataTitle.Location = new Point(639, 23);
        sepAttachmentMetadataTitle.Name = "sepAttachmentMetadataTitle";
        sepAttachmentMetadataTitle.Size = new Size(259, 1);
        sepAttachmentMetadataTitle.TabIndex = 60;
        // 
        // sepAttachmentPublicationTitle
        // 
        sepAttachmentPublicationTitle.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)230);
        sepAttachmentPublicationTitle.Appearance.Options.UseBackColor = true;
        sepAttachmentPublicationTitle.AutoSizeMode = LabelAutoSizeMode.None;
        sepAttachmentPublicationTitle.Location = new Point(1105, 23);
        sepAttachmentPublicationTitle.Name = "sepAttachmentPublicationTitle";
        sepAttachmentPublicationTitle.Size = new Size(271, 1);
        sepAttachmentPublicationTitle.TabIndex = 61;
        // 
        // sepAttachmentGridTitle
        // 
        sepAttachmentGridTitle.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)230);
        sepAttachmentGridTitle.Appearance.Options.UseBackColor = true;
        sepAttachmentGridTitle.AutoSizeMode = LabelAutoSizeMode.None;
        sepAttachmentGridTitle.Location = new Point(186, 297);
        sepAttachmentGridTitle.Name = "sepAttachmentGridTitle";
        sepAttachmentGridTitle.Size = new Size(600, 1);
        sepAttachmentGridTitle.TabIndex = 62;
        // 
        // lblAttachmentPreviewTitle
        // 
        lblAttachmentPreviewTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAttachmentPreviewTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblAttachmentPreviewTitle.Appearance.Options.UseFont = true;
        lblAttachmentPreviewTitle.Appearance.Options.UseForeColor = true;
        lblAttachmentPreviewTitle.Location = new Point(18, 12);
        lblAttachmentPreviewTitle.Name = "lblAttachmentPreviewTitle";
        lblAttachmentPreviewTitle.Size = new Size(129, 20);
        lblAttachmentPreviewTitle.TabIndex = 7;
        lblAttachmentPreviewTitle.Text = "1. Imagen principal";
        // 
        // picMainAttachmentPreview
        // 
        picMainAttachmentPreview.Location = new Point(28, 42);
        picMainAttachmentPreview.Name = "picMainAttachmentPreview";
        picMainAttachmentPreview.Properties.Appearance.BackColor = Color.White;
        picMainAttachmentPreview.Properties.Appearance.Options.UseBackColor = true;
        picMainAttachmentPreview.Properties.BorderStyle = BorderStyles.Simple;
        picMainAttachmentPreview.Properties.NullText = "Imagen del producto";
        picMainAttachmentPreview.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
        picMainAttachmentPreview.Properties.SizeMode = PictureSizeMode.Zoom;
        picMainAttachmentPreview.Size = new Size(422, 186);
        picMainAttachmentPreview.TabIndex = 8;
        // 
        // btnLoadImage
        // 
        btnLoadImage.Appearance.Font = new Font("Segoe UI", 9F);
        btnLoadImage.Appearance.Options.UseFont = true;
        btnLoadImage.Location = new Point(27, 237);
        btnLoadImage.Name = "btnLoadImage";
        btnLoadImage.Size = new Size(104, 28);
        btnLoadImage.TabIndex = 9;
        btnLoadImage.Text = "Cargar imagen";
        // 
        // btnRemoveImage
        // 
        btnRemoveImage.Appearance.Font = new Font("Segoe UI", 9F);
        btnRemoveImage.Appearance.Options.UseFont = true;
        btnRemoveImage.Location = new Point(137, 237);
        btnRemoveImage.Name = "btnRemoveImage";
        btnRemoveImage.Size = new Size(104, 28);
        btnRemoveImage.TabIndex = 10;
        btnRemoveImage.Text = "Quitar imagen";
        // 
        // btnPreviewImage
        // 
        btnPreviewImage.Appearance.Font = new Font("Segoe UI", 9F);
        btnPreviewImage.Appearance.Options.UseFont = true;
        btnPreviewImage.Location = new Point(247, 237);
        btnPreviewImage.Name = "btnPreviewImage";
        btnPreviewImage.Size = new Size(90, 28);
        btnPreviewImage.TabIndex = 11;
        btnPreviewImage.Text = "Vista previa";
        // 
        // btnSetMainImage
        // 
        btnSetMainImage.Appearance.Font = new Font("Segoe UI", 9F);
        btnSetMainImage.Appearance.Options.UseFont = true;
        btnSetMainImage.Location = new Point(343, 237);
        btnSetMainImage.Name = "btnSetMainImage";
        btnSetMainImage.Size = new Size(106, 28);
        btnSetMainImage.TabIndex = 12;
        btnSetMainImage.Text = "Marcar principal";
        // 
        // lblAttachmentPreviewNoteIcon
        // 
        lblAttachmentPreviewNoteIcon.Appearance.BackColor = Color.FromArgb((int)(byte)0, (int)(byte)122, (int)(byte)204);
        lblAttachmentPreviewNoteIcon.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblAttachmentPreviewNoteIcon.Appearance.ForeColor = Color.White;
        lblAttachmentPreviewNoteIcon.Appearance.Options.UseBackColor = true;
        lblAttachmentPreviewNoteIcon.Appearance.Options.UseFont = true;
        lblAttachmentPreviewNoteIcon.Appearance.Options.UseForeColor = true;
        lblAttachmentPreviewNoteIcon.Appearance.Options.UseTextOptions = true;
        lblAttachmentPreviewNoteIcon.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        lblAttachmentPreviewNoteIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblAttachmentPreviewNoteIcon.Location = new Point(498, 245);
        lblAttachmentPreviewNoteIcon.Name = "lblAttachmentPreviewNoteIcon";
        lblAttachmentPreviewNoteIcon.Size = new Size(18, 18);
        lblAttachmentPreviewNoteIcon.TabIndex = 0;
        lblAttachmentPreviewNoteIcon.Text = "i";
        // 
        // lblAttachmentPreviewNote
        // 
        lblAttachmentPreviewNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblAttachmentPreviewNote.Appearance.ForeColor = Color.FromArgb((int)(byte)31, (int)(byte)42, (int)(byte)68);
        lblAttachmentPreviewNote.Appearance.Options.UseFont = true;
        lblAttachmentPreviewNote.Appearance.Options.UseForeColor = true;
        lblAttachmentPreviewNote.AutoSizeMode = LabelAutoSizeMode.Vertical;
        lblAttachmentPreviewNote.Location = new Point(520, 245);
        lblAttachmentPreviewNote.Name = "lblAttachmentPreviewNote";
        lblAttachmentPreviewNote.Size = new Size(376, 13);
        lblAttachmentPreviewNote.TabIndex = 1;
        lblAttachmentPreviewNote.Text = "PNG  ·  1.8 MB  ·  11/08/2026 10:30  ·  admin";
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
        // gridView2
        // 
        gridView2.Name = "gridView2";
        // 
        // tabSap
        // 
        tabSap.Controls.Add(tabSapSections);
        tabSap.ImageOptions.SvgImageSize = new Size(22, 22);
        tabSap.Name = "tabSap";
        tabSap.Size = new Size(1406, 426);
        tabSap.Text = "Integración";
        // 
        // tabSapSections
        // 
        tabSapSections.Appearance.Font = new Font("Segoe UI", 9F);
        tabSapSections.Appearance.Options.UseFont = true;
        tabSapSections.AppearancePage.Header.Font = new Font("Segoe UI", 9F);
        tabSapSections.AppearancePage.Header.Options.UseFont = true;
        tabSapSections.AppearancePage.HeaderActive.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        tabSapSections.AppearancePage.HeaderActive.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        tabSapSections.AppearancePage.HeaderActive.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        tabSapSections.AppearancePage.HeaderActive.Options.UseBackColor = true;
        tabSapSections.AppearancePage.HeaderActive.Options.UseFont = true;
        tabSapSections.AppearancePage.HeaderActive.Options.UseForeColor = true;
        tabSapSections.AppearancePage.PageClient.BackColor = Color.White;
        tabSapSections.AppearancePage.PageClient.Options.UseBackColor = true;
        tabSapSections.Dock = DockStyle.Fill;
        tabSapSections.HeaderAutoFill = DefaultBoolean.False;
        tabSapSections.Location = new Point(0, 0);
        tabSapSections.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        tabSapSections.LookAndFeel.UseDefaultLookAndFeel = false;
        tabSapSections.Name = "tabSapSections";
        tabSapSections.SelectedTabPage = tabSapStatusPage;
        tabSapSections.Size = new Size(1406, 426);
        tabSapSections.TabIndex = 0;
        tabSapSections.TabPages.AddRange(new XtraTabPage[] { tabSapStatusPage, tabSapHistoryPage });
        tabSapSections.TabPageWidth = 220;
        tabSapSections.SelectedPageChanged += (this.tabSapSections_SelectedPageChanged);
        tabSapSections.CustomDrawTabHeader += (this.tabSapSections_CustomDrawTabHeader);
        tabSapSections.HandleCreated += (this.tabSapSections_HandleCreated);
        // 
        // tabSapStatusPage
        // 
        tabSapStatusPage.Appearance.PageClient.BackColor = Color.White;
        tabSapStatusPage.Appearance.PageClient.Options.UseBackColor = true;
        tabSapStatusPage.Controls.Add(lblSapIntegrationNote);
        tabSapStatusPage.Controls.Add(lnkSapSynchronizeNow);
        tabSapStatusPage.Controls.Add(lnkSapRefreshStatus);
        tabSapStatusPage.Controls.Add(lnkSapViewProfile);
        tabSapStatusPage.Controls.Add(sepSapCorrespondenceTitleLine);
        tabSapStatusPage.Controls.Add(sepSapConfigTitleLine);
        tabSapStatusPage.Controls.Add(sepSapStatusTitleLine);
        tabSapStatusPage.Controls.Add(sepSapColumnTwo);
        tabSapStatusPage.Controls.Add(sepSapColumnOne);
        tabSapStatusPage.Controls.Add(lblSapSerialValue);
        tabSapStatusPage.Controls.Add(lblSapSerialCaption);
        tabSapStatusPage.Controls.Add(lblSapBatchValue);
        tabSapStatusPage.Controls.Add(lblSapBatchCaption);
        tabSapStatusPage.Controls.Add(lblSapAuthorityValue);
        tabSapStatusPage.Controls.Add(lblSapAuthorityCaption);
        tabSapStatusPage.Controls.Add(lblSapExternalCodeValue);
        tabSapStatusPage.Controls.Add(lblSapExternalCodeCaption);
        tabSapStatusPage.Controls.Add(lblSapExternalSystemValue);
        tabSapStatusPage.Controls.Add(lblSapExternalSystemCaption);
        tabSapStatusPage.Controls.Add(tglSapSynchronize);
        tabSapStatusPage.Controls.Add(lblSapFieldMappingTitle);
        tabSapStatusPage.Controls.Add(lblSapMapEnabled);
        tabSapStatusPage.Controls.Add(lblSapMapRequired);
        tabSapStatusPage.Controls.Add(lueSapMapEnabled);
        tabSapStatusPage.Controls.Add(lblSapMapDescription);
        tabSapStatusPage.Controls.Add(lueSapMapRequired);
        tabSapStatusPage.Controls.Add(lblSapMapSapField);
        tabSapStatusPage.Controls.Add(txtSapMapDescription);
        tabSapStatusPage.Controls.Add(lblSapMapSystemField);
        tabSapStatusPage.Controls.Add(txtSapMapSapField);
        tabSapStatusPage.Controls.Add(txtSapMapSystemField);
        tabSapStatusPage.Controls.Add(lblSapSyncAsSupplier);
        tabSapStatusPage.Controls.Add(lblSapMode);
        tabSapStatusPage.Controls.Add(lueSapSyncAsSupplier);
        tabSapStatusPage.Controls.Add(lblSapConfigTitle);
        tabSapStatusPage.Controls.Add(lblSapManualRetry);
        tabSapStatusPage.Controls.Add(lueSapMode);
        tabSapStatusPage.Controls.Add(lueSapManualRetry);
        tabSapStatusPage.Controls.Add(lblSapCompany);
        tabSapStatusPage.Controls.Add(lblSapRequiresApproval);
        tabSapStatusPage.Controls.Add(lblSapStatusTitle);
        tabSapStatusPage.Controls.Add(lueSapRequiresApproval);
        tabSapStatusPage.Controls.Add(lueSapCompany);
        tabSapStatusPage.Controls.Add(lblSapSyncStatus);
        tabSapStatusPage.Controls.Add(lueSapSyncStatus);
        tabSapStatusPage.Controls.Add(lblSapLastSync);
        tabSapStatusPage.Controls.Add(txtSapLastSync);
        tabSapStatusPage.Controls.Add(lblSapLastError);
        tabSapStatusPage.Controls.Add(txtSapLastError);
        tabSapStatusPage.Controls.Add(lblSapRetryCount);
        tabSapStatusPage.Controls.Add(txtSapRetryCount);
        tabSapStatusPage.Controls.Add(lblSapEnabled);
        tabSapStatusPage.Name = "tabSapStatusPage";
        tabSapStatusPage.Size = new Size(1402, 398);
        tabSapStatusPage.Text = "Estado y correspondencia SAP";
        // 
        // lblSapIntegrationNote
        // 
        lblSapIntegrationNote.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapIntegrationNote.Appearance.ForeColor = Color.FromArgb((int)(byte)35, (int)(byte)66, (int)(byte)111);
        lblSapIntegrationNote.Appearance.Options.UseFont = true;
        lblSapIntegrationNote.Appearance.Options.UseForeColor = true;
        lblSapIntegrationNote.Location = new Point(28, 350);
        lblSapIntegrationNote.Name = "lblSapIntegrationNote";
        lblSapIntegrationNote.Size = new Size(558, 15);
        lblSapIntegrationNote.TabIndex = 0;
        lblSapIntegrationNote.Text = "ⓘ   La empresa y el perfil gobiernan la integración; el artículo solo define participación y correspondencia.";
        // 
        // lnkSapSynchronizeNow
        // 
        lnkSapSynchronizeNow.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lnkSapSynchronizeNow.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)91, (int)(byte)211);
        lnkSapSynchronizeNow.Appearance.Options.UseFont = true;
        lnkSapSynchronizeNow.Appearance.Options.UseForeColor = true;
        lnkSapSynchronizeNow.Location = new Point(1170, 304);
        lnkSapSynchronizeNow.Name = "lnkSapSynchronizeNow";
        lnkSapSynchronizeNow.Size = new Size(93, 15);
        lnkSapSynchronizeNow.TabIndex = 1;
        lnkSapSynchronizeNow.Text = "Sincronizar ahora";
        // 
        // lnkSapRefreshStatus
        // 
        lnkSapRefreshStatus.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lnkSapRefreshStatus.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lnkSapRefreshStatus.Appearance.Options.UseFont = true;
        lnkSapRefreshStatus.Appearance.Options.UseForeColor = true;
        lnkSapRefreshStatus.Location = new Point(1040, 304);
        lnkSapRefreshStatus.Name = "lnkSapRefreshStatus";
        lnkSapRefreshStatus.Size = new Size(91, 15);
        lnkSapRefreshStatus.TabIndex = 2;
        lnkSapRefreshStatus.Text = "Actualizar estado";
        // 
        // lnkSapViewProfile
        // 
        lnkSapViewProfile.Appearance.Font = new Font("Segoe UI", 9F);
        lnkSapViewProfile.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lnkSapViewProfile.Appearance.Options.UseFont = true;
        lnkSapViewProfile.Appearance.Options.UseForeColor = true;
        lnkSapViewProfile.Location = new Point(680, 238);
        lnkSapViewProfile.Name = "lnkSapViewProfile";
        lnkSapViewProfile.Size = new Size(142, 15);
        lnkSapViewProfile.TabIndex = 3;
        lnkSapViewProfile.Text = "Ver perfil de sincronización";
        // 
        // sepSapCorrespondenceTitleLine
        // 
        sepSapCorrespondenceTitleLine.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)231);
        sepSapCorrespondenceTitleLine.Appearance.Options.UseBackColor = true;
        sepSapCorrespondenceTitleLine.AutoSizeMode = LabelAutoSizeMode.None;
        sepSapCorrespondenceTitleLine.Location = new Point(1195, 27);
        sepSapCorrespondenceTitleLine.Name = "sepSapCorrespondenceTitleLine";
        sepSapCorrespondenceTitleLine.Size = new Size(165, 1);
        sepSapCorrespondenceTitleLine.TabIndex = 4;
        // 
        // sepSapConfigTitleLine
        // 
        sepSapConfigTitleLine.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)231);
        sepSapConfigTitleLine.Appearance.Options.UseBackColor = true;
        sepSapConfigTitleLine.AutoSizeMode = LabelAutoSizeMode.None;
        sepSapConfigTitleLine.Location = new Point(680, 27);
        sepSapConfigTitleLine.Name = "sepSapConfigTitleLine";
        sepSapConfigTitleLine.Size = new Size(200, 1);
        sepSapConfigTitleLine.TabIndex = 5;
        // 
        // sepSapStatusTitleLine
        // 
        sepSapStatusTitleLine.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)231);
        sepSapStatusTitleLine.Appearance.Options.UseBackColor = true;
        sepSapStatusTitleLine.AutoSizeMode = LabelAutoSizeMode.None;
        sepSapStatusTitleLine.Location = new Point(215, 27);
        sepSapStatusTitleLine.Name = "sepSapStatusTitleLine";
        sepSapStatusTitleLine.Size = new Size(195, 1);
        sepSapStatusTitleLine.TabIndex = 6;
        // 
        // sepSapColumnTwo
        // 
        sepSapColumnTwo.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)231);
        sepSapColumnTwo.Appearance.Options.UseBackColor = true;
        sepSapColumnTwo.AutoSizeMode = LabelAutoSizeMode.None;
        sepSapColumnTwo.Location = new Point(920, 18);
        sepSapColumnTwo.Name = "sepSapColumnTwo";
        sepSapColumnTwo.Size = new Size(1, 250);
        sepSapColumnTwo.TabIndex = 7;
        // 
        // sepSapColumnOne
        // 
        sepSapColumnOne.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)231);
        sepSapColumnOne.Appearance.Options.UseBackColor = true;
        sepSapColumnOne.AutoSizeMode = LabelAutoSizeMode.None;
        sepSapColumnOne.Location = new Point(450, 18);
        sepSapColumnOne.Name = "sepSapColumnOne";
        sepSapColumnOne.Size = new Size(1, 250);
        sepSapColumnOne.TabIndex = 8;
        // 
        // lblSapSerialValue
        // 
        lblSapSerialValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblSapSerialValue.Appearance.ForeColor = Color.FromArgb((int)(byte)31, (int)(byte)42, (int)(byte)68);
        lblSapSerialValue.Appearance.Options.UseFont = true;
        lblSapSerialValue.Appearance.Options.UseForeColor = true;
        lblSapSerialValue.Location = new Point(1160, 231);
        lblSapSerialValue.Name = "lblSapSerialValue";
        lblSapSerialValue.Size = new Size(16, 15);
        lblSapSerialValue.TabIndex = 9;
        lblSapSerialValue.Text = "No";
        // 
        // lblSapSerialCaption
        // 
        lblSapSerialCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSerialCaption.Appearance.Options.UseFont = true;
        lblSapSerialCaption.Location = new Point(960, 231);
        lblSapSerialCaption.Name = "lblSapSerialCaption";
        lblSapSerialCaption.Size = new Size(114, 15);
        lblSapSerialCaption.TabIndex = 10;
        lblSapSerialCaption.Text = "Maneja series en SAP:";
        // 
        // lblSapBatchValue
        // 
        lblSapBatchValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblSapBatchValue.Appearance.ForeColor = Color.FromArgb((int)(byte)31, (int)(byte)42, (int)(byte)68);
        lblSapBatchValue.Appearance.Options.UseFont = true;
        lblSapBatchValue.Appearance.Options.UseForeColor = true;
        lblSapBatchValue.Location = new Point(1160, 203);
        lblSapBatchValue.Name = "lblSapBatchValue";
        lblSapBatchValue.Size = new Size(11, 15);
        lblSapBatchValue.TabIndex = 11;
        lblSapBatchValue.Text = "Sí";
        // 
        // lblSapBatchCaption
        // 
        lblSapBatchCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapBatchCaption.Appearance.Options.UseFont = true;
        lblSapBatchCaption.Location = new Point(960, 203);
        lblSapBatchCaption.Name = "lblSapBatchCaption";
        lblSapBatchCaption.Size = new Size(110, 15);
        lblSapBatchCaption.TabIndex = 12;
        lblSapBatchCaption.Text = "Maneja lotes en SAP:";
        // 
        // lblSapAuthorityValue
        // 
        lblSapAuthorityValue.Appearance.BackColor = Color.FromArgb((int)(byte)248, (int)(byte)250, (int)(byte)252);
        lblSapAuthorityValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapAuthorityValue.Appearance.Options.UseBackColor = true;
        lblSapAuthorityValue.Appearance.Options.UseFont = true;
        lblSapAuthorityValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblSapAuthorityValue.BorderStyle = BorderStyles.Simple;
        lblSapAuthorityValue.Location = new Point(680, 168);
        lblSapAuthorityValue.Name = "lblSapAuthorityValue";
        lblSapAuthorityValue.Padding = new Padding(8, 4, 0, 0);
        lblSapAuthorityValue.Size = new Size(200, 24);
        lblSapAuthorityValue.TabIndex = 13;
        lblSapAuthorityValue.Text = "Según perfil";
        // 
        // lblSapAuthorityCaption
        // 
        lblSapAuthorityCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapAuthorityCaption.Appearance.Options.UseFont = true;
        lblSapAuthorityCaption.Location = new Point(490, 175);
        lblSapAuthorityCaption.Name = "lblSapAuthorityCaption";
        lblSapAuthorityCaption.Size = new Size(102, 15);
        lblSapAuthorityCaption.TabIndex = 14;
        lblSapAuthorityCaption.Text = "Autoridad del dato:";
        // 
        // lblSapExternalCodeValue
        // 
        lblSapExternalCodeValue.Appearance.BackColor = Color.FromArgb((int)(byte)248, (int)(byte)250, (int)(byte)252);
        lblSapExternalCodeValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExternalCodeValue.Appearance.Options.UseBackColor = true;
        lblSapExternalCodeValue.Appearance.Options.UseFont = true;
        lblSapExternalCodeValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblSapExternalCodeValue.BorderStyle = BorderStyles.Simple;
        lblSapExternalCodeValue.Location = new Point(210, 168);
        lblSapExternalCodeValue.Name = "lblSapExternalCodeValue";
        lblSapExternalCodeValue.Padding = new Padding(8, 4, 0, 0);
        lblSapExternalCodeValue.Size = new Size(200, 24);
        lblSapExternalCodeValue.TabIndex = 15;
        lblSapExternalCodeValue.Text = "-";
        // 
        // lblSapExternalCodeCaption
        // 
        lblSapExternalCodeCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExternalCodeCaption.Appearance.Options.UseFont = true;
        lblSapExternalCodeCaption.Location = new Point(28, 175);
        lblSapExternalCodeCaption.Name = "lblSapExternalCodeCaption";
        lblSapExternalCodeCaption.Size = new Size(84, 15);
        lblSapExternalCodeCaption.TabIndex = 16;
        lblSapExternalCodeCaption.Text = "Código externo:";
        // 
        // lblSapExternalSystemValue
        // 
        lblSapExternalSystemValue.Appearance.BackColor = Color.FromArgb((int)(byte)248, (int)(byte)250, (int)(byte)252);
        lblSapExternalSystemValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExternalSystemValue.Appearance.Options.UseBackColor = true;
        lblSapExternalSystemValue.Appearance.Options.UseFont = true;
        lblSapExternalSystemValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblSapExternalSystemValue.BorderStyle = BorderStyles.Simple;
        lblSapExternalSystemValue.Location = new Point(210, 140);
        lblSapExternalSystemValue.Name = "lblSapExternalSystemValue";
        lblSapExternalSystemValue.Padding = new Padding(8, 4, 0, 0);
        lblSapExternalSystemValue.Size = new Size(200, 24);
        lblSapExternalSystemValue.TabIndex = 17;
        lblSapExternalSystemValue.Text = "SAP_B1";
        // 
        // lblSapExternalSystemCaption
        // 
        lblSapExternalSystemCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExternalSystemCaption.Appearance.Options.UseFont = true;
        lblSapExternalSystemCaption.Location = new Point(28, 147);
        lblSapExternalSystemCaption.Name = "lblSapExternalSystemCaption";
        lblSapExternalSystemCaption.Size = new Size(86, 15);
        lblSapExternalSystemCaption.TabIndex = 18;
        lblSapExternalSystemCaption.Text = "Sistema externo:";
        // 
        // tglSapSynchronize
        // 
        tglSapSynchronize.Location = new Point(210, 54);
        tglSapSynchronize.Name = "tglSapSynchronize";
        tglSapSynchronize.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglSapSynchronize.Properties.Appearance.Options.UseFont = true;
        tglSapSynchronize.Properties.OffText = "No";
        tglSapSynchronize.Properties.OnText = "Sí";
        tglSapSynchronize.Size = new Size(86, 20);
        tglSapSynchronize.TabIndex = 1;
        // 
        // lblSapFieldMappingTitle
        // 
        lblSapFieldMappingTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapFieldMappingTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSapFieldMappingTitle.Appearance.Options.UseFont = true;
        lblSapFieldMappingTitle.Appearance.Options.UseForeColor = true;
        lblSapFieldMappingTitle.Location = new Point(960, 18);
        lblSapFieldMappingTitle.Name = "lblSapFieldMappingTitle";
        lblSapFieldMappingTitle.Size = new Size(189, 20);
        lblSapFieldMappingTitle.TabIndex = 58;
        lblSapFieldMappingTitle.Text = "3. Correspondencia efectiva";
        // 
        // lblSapMapEnabled
        // 
        lblSapMapEnabled.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapEnabled.Appearance.Options.UseFont = true;
        lblSapMapEnabled.Location = new Point(960, 175);
        lblSapMapEnabled.Name = "lblSapMapEnabled";
        lblSapMapEnabled.Size = new Size(83, 15);
        lblSapMapEnabled.TabIndex = 76;
        lblSapMapEnabled.Text = "Valoración SAP:";
        // 
        // lblSapMapRequired
        // 
        lblSapMapRequired.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapRequired.Appearance.Options.UseFont = true;
        lblSapMapRequired.Location = new Point(960, 147);
        lblSapMapRequired.Name = "lblSapMapRequired";
        lblSapMapRequired.Size = new Size(110, 15);
        lblSapMapRequired.TabIndex = 72;
        lblSapMapRequired.Text = "Abastecimiento SAP:";
        // 
        // lueSapMapEnabled
        // 
        lueSapMapEnabled.Location = new Point(1160, 168);
        lueSapMapEnabled.Name = "lueSapMapEnabled";
        lueSapMapEnabled.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapMapEnabled.Properties.Appearance.Options.UseFont = true;
        lueSapMapEnabled.Properties.AutoHeight = false;
        lueSapMapEnabled.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapMapEnabled.Properties.NullText = "";
        lueSapMapEnabled.Size = new Size(200, 22);
        lueSapMapEnabled.TabIndex = 78;
        // 
        // lblSapMapDescription
        // 
        lblSapMapDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapDescription.Appearance.Options.UseFont = true;
        lblSapMapDescription.Location = new Point(960, 119);
        lblSapMapDescription.Name = "lblSapMapDescription";
        lblSapMapDescription.Size = new Size(95, 15);
        lblSapMapDescription.TabIndex = 69;
        lblSapMapDescription.Text = "Planificación SAP:";
        // 
        // lueSapMapRequired
        // 
        lueSapMapRequired.Location = new Point(1160, 140);
        lueSapMapRequired.Name = "lueSapMapRequired";
        lueSapMapRequired.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapMapRequired.Properties.Appearance.Options.UseFont = true;
        lueSapMapRequired.Properties.AutoHeight = false;
        lueSapMapRequired.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapMapRequired.Properties.NullText = "";
        lueSapMapRequired.Size = new Size(200, 22);
        lueSapMapRequired.TabIndex = 74;
        // 
        // lblSapMapSapField
        // 
        lblSapMapSapField.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapSapField.Appearance.Options.UseFont = true;
        lblSapMapSapField.Location = new Point(960, 91);
        lblSapMapSapField.Name = "lblSapMapSapField";
        lblSapMapSapField.Size = new Size(127, 15);
        lblSapMapSapField.TabIndex = 66;
        lblSapMapSapField.Text = "Grupo de unidades SAP:";
        // 
        // txtSapMapDescription
        // 
        txtSapMapDescription.Location = new Point(1160, 112);
        txtSapMapDescription.Name = "txtSapMapDescription";
        txtSapMapDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapMapDescription.Properties.Appearance.Options.UseFont = true;
        txtSapMapDescription.Properties.AutoHeight = false;
        txtSapMapDescription.Size = new Size(200, 22);
        txtSapMapDescription.TabIndex = 70;
        // 
        // lblSapMapSystemField
        // 
        lblSapMapSystemField.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapSystemField.Appearance.Options.UseFont = true;
        lblSapMapSystemField.Location = new Point(960, 63);
        lblSapMapSystemField.Name = "lblSapMapSystemField";
        lblSapMapSystemField.Size = new Size(60, 15);
        lblSapMapSystemField.TabIndex = 63;
        lblSapMapSystemField.Text = "Grupo SAP:";
        // 
        // txtSapMapSapField
        // 
        txtSapMapSapField.Location = new Point(1160, 84);
        txtSapMapSapField.Name = "txtSapMapSapField";
        txtSapMapSapField.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapMapSapField.Properties.Appearance.Options.UseFont = true;
        txtSapMapSapField.Properties.AutoHeight = false;
        txtSapMapSapField.Size = new Size(200, 22);
        txtSapMapSapField.TabIndex = 68;
        // 
        // txtSapMapSystemField
        // 
        txtSapMapSystemField.Location = new Point(1160, 56);
        txtSapMapSystemField.Name = "txtSapMapSystemField";
        txtSapMapSystemField.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapMapSystemField.Properties.Appearance.Options.UseFont = true;
        txtSapMapSystemField.Properties.AutoHeight = false;
        txtSapMapSystemField.Size = new Size(200, 22);
        txtSapMapSystemField.TabIndex = 65;
        // 
        // lblSapSyncAsSupplier
        // 
        lblSapSyncAsSupplier.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSyncAsSupplier.Appearance.Options.UseFont = true;
        lblSapSyncAsSupplier.Location = new Point(490, 119);
        lblSapSyncAsSupplier.Name = "lblSapSyncAsSupplier";
        lblSapSyncAsSupplier.Size = new Size(75, 15);
        lblSapSyncAsSupplier.TabIndex = 71;
        lblSapSyncAsSupplier.Text = "Perfil efectivo:";
        // 
        // lblSapMode
        // 
        lblSapMode.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMode.Appearance.Options.UseFont = true;
        lblSapMode.Location = new Point(490, 91);
        lblSapMode.Name = "lblSapMode";
        lblSapMode.Size = new Size(69, 15);
        lblSapMode.TabIndex = 59;
        lblSapMode.Text = "Base destino:";
        // 
        // lueSapSyncAsSupplier
        // 
        lueSapSyncAsSupplier.Location = new Point(680, 112);
        lueSapSyncAsSupplier.Name = "lueSapSyncAsSupplier";
        lueSapSyncAsSupplier.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapSyncAsSupplier.Properties.Appearance.Options.UseFont = true;
        lueSapSyncAsSupplier.Properties.AutoHeight = false;
        lueSapSyncAsSupplier.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapSyncAsSupplier.Properties.NullText = "";
        lueSapSyncAsSupplier.Size = new Size(200, 22);
        lueSapSyncAsSupplier.TabIndex = 73;
        // 
        // lblSapConfigTitle
        // 
        lblSapConfigTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapConfigTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSapConfigTitle.Appearance.Options.UseFont = true;
        lblSapConfigTitle.Appearance.Options.UseForeColor = true;
        lblSapConfigTitle.Location = new Point(490, 18);
        lblSapConfigTitle.Name = "lblSapConfigTitle";
        lblSapConfigTitle.Size = new Size(170, 20);
        lblSapConfigTitle.TabIndex = 56;
        lblSapConfigTitle.Text = "2. Configuración efectiva";
        // 
        // lblSapManualRetry
        // 
        lblSapManualRetry.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapManualRetry.Appearance.Options.UseFont = true;
        lblSapManualRetry.Location = new Point(490, 147);
        lblSapManualRetry.Name = "lblSapManualRetry";
        lblSapManualRetry.Size = new Size(99, 15);
        lblSapManualRetry.TabIndex = 75;
        lblSapManualRetry.Text = "Dirección del flujo:";
        // 
        // lueSapMode
        // 
        lueSapMode.Location = new Point(680, 84);
        lueSapMode.Name = "lueSapMode";
        lueSapMode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapMode.Properties.Appearance.Options.UseFont = true;
        lueSapMode.Properties.AutoHeight = false;
        lueSapMode.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapMode.Properties.NullText = "";
        lueSapMode.Size = new Size(200, 22);
        lueSapMode.TabIndex = 62;
        // 
        // lueSapManualRetry
        // 
        lueSapManualRetry.Location = new Point(680, 140);
        lueSapManualRetry.Name = "lueSapManualRetry";
        lueSapManualRetry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapManualRetry.Properties.Appearance.Options.UseFont = true;
        lueSapManualRetry.Properties.AutoHeight = false;
        lueSapManualRetry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapManualRetry.Properties.NullText = "";
        lueSapManualRetry.Size = new Size(200, 22);
        lueSapManualRetry.TabIndex = 77;
        // 
        // lblSapCompany
        // 
        lblSapCompany.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapCompany.Appearance.Options.UseFont = true;
        lblSapCompany.Location = new Point(490, 63);
        lblSapCompany.Name = "lblSapCompany";
        lblSapCompany.Size = new Size(72, 15);
        lblSapCompany.TabIndex = 64;
        lblSapCompany.Text = "Empresa SAP:";
        // 
        // lblSapRequiresApproval
        // 
        lblSapRequiresApproval.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapRequiresApproval.Appearance.Options.UseFont = true;
        lblSapRequiresApproval.Location = new Point(490, 203);
        lblSapRequiresApproval.Name = "lblSapRequiresApproval";
        lblSapRequiresApproval.Size = new Size(112, 15);
        lblSapRequiresApproval.TabIndex = 79;
        lblSapRequiresApproval.Text = "Requiere aprobación:";
        // 
        // lblSapStatusTitle
        // 
        lblSapStatusTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapStatusTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSapStatusTitle.Appearance.Options.UseFont = true;
        lblSapStatusTitle.Appearance.Options.UseForeColor = true;
        lblSapStatusTitle.Location = new Point(28, 18);
        lblSapStatusTitle.Name = "lblSapStatusTitle";
        lblSapStatusTitle.Size = new Size(184, 20);
        lblSapStatusTitle.TabIndex = 81;
        lblSapStatusTitle.Text = "1. Participación e identidad";
        // 
        // lueSapRequiresApproval
        // 
        lueSapRequiresApproval.Location = new Point(680, 196);
        lueSapRequiresApproval.Name = "lueSapRequiresApproval";
        lueSapRequiresApproval.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapRequiresApproval.Properties.Appearance.Options.UseFont = true;
        lueSapRequiresApproval.Properties.AutoHeight = false;
        lueSapRequiresApproval.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapRequiresApproval.Properties.NullText = "";
        lueSapRequiresApproval.Size = new Size(200, 22);
        lueSapRequiresApproval.TabIndex = 80;
        // 
        // lueSapCompany
        // 
        lueSapCompany.Location = new Point(680, 56);
        lueSapCompany.Name = "lueSapCompany";
        lueSapCompany.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapCompany.Properties.Appearance.Options.UseFont = true;
        lueSapCompany.Properties.AutoHeight = false;
        lueSapCompany.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapCompany.Properties.NullText = "";
        lueSapCompany.Properties.PopupView = grvSapCompanyLookup;
        lueSapCompany.Size = new Size(200, 22);
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
        lblSapSyncStatus.Location = new Point(28, 91);
        lblSapSyncStatus.Name = "lblSapSyncStatus";
        lblSapSyncStatus.Size = new Size(133, 15);
        lblSapSyncStatus.TabIndex = 82;
        lblSapSyncStatus.Text = "Estado de sincronización:";
        // 
        // lueSapSyncStatus
        // 
        lueSapSyncStatus.Location = new Point(210, 84);
        lueSapSyncStatus.Name = "lueSapSyncStatus";
        lueSapSyncStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapSyncStatus.Properties.Appearance.Options.UseFont = true;
        lueSapSyncStatus.Properties.AutoHeight = false;
        lueSapSyncStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapSyncStatus.Properties.NullText = "";
        lueSapSyncStatus.Size = new Size(200, 22);
        lueSapSyncStatus.TabIndex = 83;
        // 
        // lblSapLastSync
        // 
        lblSapLastSync.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastSync.Appearance.Options.UseFont = true;
        lblSapLastSync.Location = new Point(28, 203);
        lblSapLastSync.Name = "lblSapLastSync";
        lblSapLastSync.Size = new Size(117, 15);
        lblSapLastSync.TabIndex = 84;
        lblSapLastSync.Text = "Última sincronización:";
        // 
        // txtSapLastSync
        // 
        txtSapLastSync.Location = new Point(210, 196);
        txtSapLastSync.Name = "txtSapLastSync";
        txtSapLastSync.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapLastSync.Properties.Appearance.Options.UseFont = true;
        txtSapLastSync.Properties.AutoHeight = false;
        txtSapLastSync.Properties.ReadOnly = true;
        txtSapLastSync.Size = new Size(200, 22);
        txtSapLastSync.TabIndex = 85;
        // 
        // lblSapLastError
        // 
        lblSapLastError.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastError.Appearance.Options.UseFont = true;
        lblSapLastError.Location = new Point(28, 231);
        lblSapLastError.Name = "lblSapLastError";
        lblSapLastError.Size = new Size(67, 15);
        lblSapLastError.TabIndex = 86;
        lblSapLastError.Text = "Último error:";
        // 
        // txtSapLastError
        // 
        txtSapLastError.Location = new Point(210, 224);
        txtSapLastError.Name = "txtSapLastError";
        txtSapLastError.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapLastError.Properties.Appearance.Options.UseFont = true;
        txtSapLastError.Properties.AutoHeight = false;
        txtSapLastError.Properties.ReadOnly = true;
        txtSapLastError.Size = new Size(200, 22);
        txtSapLastError.TabIndex = 87;
        // 
        // lblSapRetryCount
        // 
        lblSapRetryCount.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapRetryCount.Appearance.Options.UseFont = true;
        lblSapRetryCount.Location = new Point(28, 119);
        lblSapRetryCount.Name = "lblSapRetryCount";
        lblSapRetryCount.Size = new Size(66, 15);
        lblSapRetryCount.TabIndex = 88;
        lblSapRetryCount.Text = "Código SAP:";
        // 
        // txtSapRetryCount
        // 
        txtSapRetryCount.Location = new Point(210, 112);
        txtSapRetryCount.Name = "txtSapRetryCount";
        txtSapRetryCount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapRetryCount.Properties.Appearance.Options.UseFont = true;
        txtSapRetryCount.Properties.Appearance.Options.UseTextOptions = true;
        txtSapRetryCount.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        txtSapRetryCount.Properties.AutoHeight = false;
        txtSapRetryCount.Properties.ReadOnly = true;
        txtSapRetryCount.Size = new Size(200, 22);
        txtSapRetryCount.TabIndex = 89;
        // 
        // lblSapEnabled
        // 
        lblSapEnabled.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapEnabled.Appearance.Options.UseFont = true;
        lblSapEnabled.Location = new Point(28, 63);
        lblSapEnabled.Name = "lblSapEnabled";
        lblSapEnabled.Size = new Size(108, 15);
        lblSapEnabled.TabIndex = 90;
        lblSapEnabled.Text = "Sincronizar con SAP:";
        // 
        // tabSapHistoryPage
        // 
        tabSapHistoryPage.Appearance.PageClient.BackColor = Color.White;
        tabSapHistoryPage.Appearance.PageClient.Options.UseBackColor = true;
        tabSapHistoryPage.Controls.Add(lblSapHistoryNote);
        tabSapHistoryPage.Controls.Add(lnkSapRetry);
        tabSapHistoryPage.Controls.Add(lnkSapCopyTracking);
        tabSapHistoryPage.Controls.Add(lnkSapViewDetail);
        tabSapHistoryPage.Controls.Add(sepSapActionsTitle);
        tabSapHistoryPage.Controls.Add(lblSapActionsTitle);
        tabSapHistoryPage.Controls.Add(lblSapExecutionProfileValue);
        tabSapHistoryPage.Controls.Add(lblSapExecutionProfileCaption);
        tabSapHistoryPage.Controls.Add(lblSapExecutionUserValue);
        tabSapHistoryPage.Controls.Add(lblSapExecutionUserCaption);
        tabSapHistoryPage.Controls.Add(lblSapExecutionTrackingValue);
        tabSapHistoryPage.Controls.Add(lblSapExecutionTrackingCaption);
        tabSapHistoryPage.Controls.Add(lblSapExecutionMessageValue);
        tabSapHistoryPage.Controls.Add(lblSapExecutionMessageCaption);
        tabSapHistoryPage.Controls.Add(lblSapExecutionResultValue);
        tabSapHistoryPage.Controls.Add(lblSapExecutionResultCaption);
        tabSapHistoryPage.Controls.Add(sepSapExecutionDetailTitle);
        tabSapHistoryPage.Controls.Add(lblSapExecutionDetailTitle);
        tabSapHistoryPage.Controls.Add(lnkSapRefreshHistory);
        tabSapHistoryPage.Controls.Add(lblSapPendingRetriesSummary);
        tabSapHistoryPage.Controls.Add(lblSapLastSyncSummary);
        tabSapHistoryPage.Controls.Add(lblSapCurrentStatusSummary);
        tabSapHistoryPage.Controls.Add(lblSapHistoryTitle);
        tabSapHistoryPage.Controls.Add(grdSapSyncHistory);
        tabSapHistoryPage.Name = "tabSapHistoryPage";
        tabSapHistoryPage.Size = new Size(1402, 398);
        tabSapHistoryPage.Text = "Historial";
        // 
        // lblSapHistoryNote
        // 
        lblSapHistoryNote.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapHistoryNote.Appearance.ForeColor = Color.FromArgb((int)(byte)35, (int)(byte)66, (int)(byte)111);
        lblSapHistoryNote.Appearance.Options.UseFont = true;
        lblSapHistoryNote.Appearance.Options.UseForeColor = true;
        lblSapHistoryNote.Location = new Point(28, 370);
        lblSapHistoryNote.Name = "lblSapHistoryNote";
        lblSapHistoryNote.Size = new Size(511, 15);
        lblSapHistoryNote.TabIndex = 0;
        lblSapHistoryNote.Text = "ⓘ   El historial es informativo; las operaciones se ejecutan mediante la API y el worker autorizado.";
        // 
        // lnkSapRetry
        // 
        lnkSapRetry.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lnkSapRetry.Appearance.ForeColor = Color.FromArgb((int)(byte)148, (int)(byte)163, (int)(byte)184);
        lnkSapRetry.Appearance.Options.UseFont = true;
        lnkSapRetry.Appearance.Options.UseForeColor = true;
        lnkSapRetry.Enabled = false;
        lnkSapRetry.Location = new Point(810, 342);
        lnkSapRetry.Name = "lnkSapRetry";
        lnkSapRetry.Size = new Size(54, 15);
        lnkSapRetry.TabIndex = 1;
        lnkSapRetry.Text = "Reintentar";
        // 
        // lnkSapCopyTracking
        // 
        lnkSapCopyTracking.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lnkSapCopyTracking.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)91, (int)(byte)211);
        lnkSapCopyTracking.Appearance.Options.UseFont = true;
        lnkSapCopyTracking.Appearance.Options.UseForeColor = true;
        lnkSapCopyTracking.Location = new Point(810, 310);
        lnkSapCopyTracking.Name = "lnkSapCopyTracking";
        lnkSapCopyTracking.Size = new Size(103, 15);
        lnkSapCopyTracking.TabIndex = 2;
        lnkSapCopyTracking.Text = "Copiar seguimiento";
        // 
        // lnkSapViewDetail
        // 
        lnkSapViewDetail.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lnkSapViewDetail.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)91, (int)(byte)211);
        lnkSapViewDetail.Appearance.Options.UseFont = true;
        lnkSapViewDetail.Appearance.Options.UseForeColor = true;
        lnkSapViewDetail.Location = new Point(810, 278);
        lnkSapViewDetail.Name = "lnkSapViewDetail";
        lnkSapViewDetail.Size = new Size(56, 15);
        lnkSapViewDetail.TabIndex = 3;
        lnkSapViewDetail.Text = "Ver detalle";
        // 
        // sepSapActionsTitle
        // 
        sepSapActionsTitle.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)231);
        sepSapActionsTitle.Appearance.Options.UseBackColor = true;
        sepSapActionsTitle.AutoSizeMode = LabelAutoSizeMode.None;
        sepSapActionsTitle.Location = new Point(900, 249);
        sepSapActionsTitle.Name = "sepSapActionsTitle";
        sepSapActionsTitle.Size = new Size(445, 1);
        sepSapActionsTitle.TabIndex = 4;
        // 
        // lblSapActionsTitle
        // 
        lblSapActionsTitle.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblSapActionsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSapActionsTitle.Appearance.Options.UseFont = true;
        lblSapActionsTitle.Appearance.Options.UseForeColor = true;
        lblSapActionsTitle.Location = new Point(810, 240);
        lblSapActionsTitle.Name = "lblSapActionsTitle";
        lblSapActionsTitle.Size = new Size(67, 17);
        lblSapActionsTitle.TabIndex = 5;
        lblSapActionsTitle.Text = "3. Acciones";
        // 
        // lblSapExecutionProfileValue
        // 
        lblSapExecutionProfileValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExecutionProfileValue.Appearance.Options.UseFont = true;
        lblSapExecutionProfileValue.Location = new Point(480, 339);
        lblSapExecutionProfileValue.Name = "lblSapExecutionProfileValue";
        lblSapExecutionProfileValue.Size = new Size(5, 15);
        lblSapExecutionProfileValue.TabIndex = 6;
        lblSapExecutionProfileValue.Text = "-";
        // 
        // lblSapExecutionProfileCaption
        // 
        lblSapExecutionProfileCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExecutionProfileCaption.Appearance.Options.UseFont = true;
        lblSapExecutionProfileCaption.Location = new Point(420, 339);
        lblSapExecutionProfileCaption.Name = "lblSapExecutionProfileCaption";
        lblSapExecutionProfileCaption.Size = new Size(30, 15);
        lblSapExecutionProfileCaption.TabIndex = 7;
        lblSapExecutionProfileCaption.Text = "Perfil:";
        // 
        // lblSapExecutionUserValue
        // 
        lblSapExecutionUserValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExecutionUserValue.Appearance.Options.UseFont = true;
        lblSapExecutionUserValue.Location = new Point(230, 339);
        lblSapExecutionUserValue.Name = "lblSapExecutionUserValue";
        lblSapExecutionUserValue.Size = new Size(5, 15);
        lblSapExecutionUserValue.TabIndex = 8;
        lblSapExecutionUserValue.Text = "-";
        // 
        // lblSapExecutionUserCaption
        // 
        lblSapExecutionUserCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExecutionUserCaption.Appearance.Options.UseFont = true;
        lblSapExecutionUserCaption.Location = new Point(28, 339);
        lblSapExecutionUserCaption.Name = "lblSapExecutionUserCaption";
        lblSapExecutionUserCaption.Size = new Size(76, 15);
        lblSapExecutionUserCaption.TabIndex = 9;
        lblSapExecutionUserCaption.Text = "Ejecutado por:";
        // 
        // lblSapExecutionTrackingValue
        // 
        lblSapExecutionTrackingValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExecutionTrackingValue.Appearance.Options.UseFont = true;
        lblSapExecutionTrackingValue.Location = new Point(230, 317);
        lblSapExecutionTrackingValue.Name = "lblSapExecutionTrackingValue";
        lblSapExecutionTrackingValue.Size = new Size(5, 15);
        lblSapExecutionTrackingValue.TabIndex = 10;
        lblSapExecutionTrackingValue.Text = "-";
        // 
        // lblSapExecutionTrackingCaption
        // 
        lblSapExecutionTrackingCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExecutionTrackingCaption.Appearance.Options.UseFont = true;
        lblSapExecutionTrackingCaption.Location = new Point(28, 317);
        lblSapExecutionTrackingCaption.Name = "lblSapExecutionTrackingCaption";
        lblSapExecutionTrackingCaption.Size = new Size(127, 15);
        lblSapExecutionTrackingCaption.TabIndex = 11;
        lblSapExecutionTrackingCaption.Text = "Código de seguimiento:";
        // 
        // lblSapExecutionMessageValue
        // 
        lblSapExecutionMessageValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExecutionMessageValue.Appearance.Options.UseFont = true;
        lblSapExecutionMessageValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblSapExecutionMessageValue.Location = new Point(230, 295);
        lblSapExecutionMessageValue.Name = "lblSapExecutionMessageValue";
        lblSapExecutionMessageValue.Size = new Size(485, 18);
        lblSapExecutionMessageValue.TabIndex = 12;
        lblSapExecutionMessageValue.Text = "Seleccione una ejecución para consultar el detalle.";
        // 
        // lblSapExecutionMessageCaption
        // 
        lblSapExecutionMessageCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExecutionMessageCaption.Appearance.Options.UseFont = true;
        lblSapExecutionMessageCaption.Location = new Point(28, 295);
        lblSapExecutionMessageCaption.Name = "lblSapExecutionMessageCaption";
        lblSapExecutionMessageCaption.Size = new Size(47, 15);
        lblSapExecutionMessageCaption.TabIndex = 13;
        lblSapExecutionMessageCaption.Text = "Mensaje:";
        // 
        // lblSapExecutionResultValue
        // 
        lblSapExecutionResultValue.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblSapExecutionResultValue.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)153, (int)(byte)51);
        lblSapExecutionResultValue.Appearance.Options.UseFont = true;
        lblSapExecutionResultValue.Appearance.Options.UseForeColor = true;
        lblSapExecutionResultValue.Location = new Point(230, 273);
        lblSapExecutionResultValue.Name = "lblSapExecutionResultValue";
        lblSapExecutionResultValue.Size = new Size(5, 15);
        lblSapExecutionResultValue.TabIndex = 14;
        lblSapExecutionResultValue.Text = "-";
        // 
        // lblSapExecutionResultCaption
        // 
        lblSapExecutionResultCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapExecutionResultCaption.Appearance.Options.UseFont = true;
        lblSapExecutionResultCaption.Location = new Point(28, 273);
        lblSapExecutionResultCaption.Name = "lblSapExecutionResultCaption";
        lblSapExecutionResultCaption.Size = new Size(55, 15);
        lblSapExecutionResultCaption.TabIndex = 15;
        lblSapExecutionResultCaption.Text = "Resultado:";
        // 
        // sepSapExecutionDetailTitle
        // 
        sepSapExecutionDetailTitle.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)224, (int)(byte)231);
        sepSapExecutionDetailTitle.Appearance.Options.UseBackColor = true;
        sepSapExecutionDetailTitle.AutoSizeMode = LabelAutoSizeMode.None;
        sepSapExecutionDetailTitle.Location = new Point(205, 249);
        sepSapExecutionDetailTitle.Name = "sepSapExecutionDetailTitle";
        sepSapExecutionDetailTitle.Size = new Size(510, 1);
        sepSapExecutionDetailTitle.TabIndex = 16;
        // 
        // lblSapExecutionDetailTitle
        // 
        lblSapExecutionDetailTitle.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblSapExecutionDetailTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSapExecutionDetailTitle.Appearance.Options.UseFont = true;
        lblSapExecutionDetailTitle.Appearance.Options.UseForeColor = true;
        lblSapExecutionDetailTitle.Location = new Point(28, 240);
        lblSapExecutionDetailTitle.Name = "lblSapExecutionDetailTitle";
        lblSapExecutionDetailTitle.Size = new Size(148, 17);
        lblSapExecutionDetailTitle.TabIndex = 17;
        lblSapExecutionDetailTitle.Text = "2. Detalle de la ejecución";
        // 
        // lnkSapRefreshHistory
        // 
        lnkSapRefreshHistory.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lnkSapRefreshHistory.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lnkSapRefreshHistory.Appearance.Options.UseFont = true;
        lnkSapRefreshHistory.Appearance.Options.UseForeColor = true;
        lnkSapRefreshHistory.Location = new Point(1288, 16);
        lnkSapRefreshHistory.Name = "lnkSapRefreshHistory";
        lnkSapRefreshHistory.Size = new Size(53, 15);
        lnkSapRefreshHistory.TabIndex = 18;
        lnkSapRefreshHistory.Text = "Actualizar";
        // 
        // lblSapPendingRetriesSummary
        // 
        lblSapPendingRetriesSummary.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapPendingRetriesSummary.Appearance.Options.UseFont = true;
        lblSapPendingRetriesSummary.Location = new Point(690, 16);
        lblSapPendingRetriesSummary.Name = "lblSapPendingRetriesSummary";
        lblSapPendingRetriesSummary.Size = new Size(132, 15);
        lblSapPendingRetriesSummary.TabIndex = 19;
        lblSapPendingRetriesSummary.Text = "Reintentos pendientes:  0";
        // 
        // lblSapLastSyncSummary
        // 
        lblSapLastSyncSummary.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastSyncSummary.Appearance.Options.UseFont = true;
        lblSapLastSyncSummary.Location = new Point(330, 16);
        lblSapLastSyncSummary.Name = "lblSapLastSyncSummary";
        lblSapLastSyncSummary.Size = new Size(218, 15);
        lblSapLastSyncSummary.TabIndex = 20;
        lblSapLastSyncSummary.Text = "Última sincronización:  Sin sincronización";
        // 
        // lblSapCurrentStatusSummary
        // 
        lblSapCurrentStatusSummary.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblSapCurrentStatusSummary.Appearance.ForeColor = Color.FromArgb((int)(byte)31, (int)(byte)42, (int)(byte)68);
        lblSapCurrentStatusSummary.Appearance.Options.UseFont = true;
        lblSapCurrentStatusSummary.Appearance.Options.UseForeColor = true;
        lblSapCurrentStatusSummary.Location = new Point(28, 16);
        lblSapCurrentStatusSummary.Name = "lblSapCurrentStatusSummary";
        lblSapCurrentStatusSummary.Size = new Size(149, 15);
        lblSapCurrentStatusSummary.TabIndex = 21;
        lblSapCurrentStatusSummary.Text = "Estado actual:  Sincronizado";
        // 
        // lblSapHistoryTitle
        // 
        lblSapHistoryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapHistoryTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSapHistoryTitle.Appearance.Options.UseFont = true;
        lblSapHistoryTitle.Appearance.Options.UseForeColor = true;
        lblSapHistoryTitle.Location = new Point(28, 46);
        lblSapHistoryTitle.Name = "lblSapHistoryTitle";
        lblSapHistoryTitle.Size = new Size(194, 20);
        lblSapHistoryTitle.TabIndex = 57;
        lblSapHistoryTitle.Text = "1. Historial de sincronización";
        // 
        // grdSapSyncHistory
        // 
        grdSapSyncHistory.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        grdSapSyncHistory.Location = new Point(28, 72);
        grdSapSyncHistory.MainView = grvSapSyncHistory;
        grdSapSyncHistory.Name = "grdSapSyncHistory";
        grdSapSyncHistory.Size = new Size(1346, 152);
        grdSapSyncHistory.TabIndex = 60;
        grdSapSyncHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvSapSyncHistory });
        // 
        // grvSapSyncHistory
        // 
        grvSapSyncHistory.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSapSyncHistory.Appearance.HeaderPanel.Options.UseFont = true;
        grvSapSyncHistory.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSapSyncHistory.Appearance.Row.Options.UseFont = true;
        grvSapSyncHistory.Columns.AddRange(new GridColumn[] { colSapHistoryDate, colSapHistoryDocEntry, colSapHistoryOperation, colSapHistoryStatus, colSapHistoryDocNum, colSapHistoryRetryCount, colSapHistoryDuration, colSapHistoryTracking, colSapHistoryMessage });
        grvSapSyncHistory.GridControl = grdSapSyncHistory;
        grvSapSyncHistory.Name = "grvSapSyncHistory";
        grvSapSyncHistory.OptionsBehavior.Editable = false;
        grvSapSyncHistory.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvSapSyncHistory.OptionsView.ShowGroupPanel = false;
        grvSapSyncHistory.OptionsView.ShowIndicator = false;
        grvSapSyncHistory.RowHeight = 28;
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
        // colSapHistoryDocEntry
        // 
        colSapHistoryDocEntry.Caption = "Dirección";
        colSapHistoryDocEntry.FieldName = "SapDocEntry";
        colSapHistoryDocEntry.Name = "colSapHistoryDocEntry";
        colSapHistoryDocEntry.Visible = true;
        colSapHistoryDocEntry.VisibleIndex = 1;
        colSapHistoryDocEntry.Width = 115;
        // 
        // colSapHistoryOperation
        // 
        colSapHistoryOperation.Caption = "Operacion";
        colSapHistoryOperation.FieldName = "Operation";
        colSapHistoryOperation.Name = "colSapHistoryOperation";
        colSapHistoryOperation.Visible = true;
        colSapHistoryOperation.VisibleIndex = 2;
        colSapHistoryOperation.Width = 120;
        // 
        // colSapHistoryStatus
        // 
        colSapHistoryStatus.Caption = "Estado";
        colSapHistoryStatus.FieldName = "Status";
        colSapHistoryStatus.Name = "colSapHistoryStatus";
        colSapHistoryStatus.Visible = true;
        colSapHistoryStatus.VisibleIndex = 3;
        colSapHistoryStatus.Width = 90;
        // 
        // colSapHistoryDocNum
        // 
        colSapHistoryDocNum.Caption = "Código SAP";
        colSapHistoryDocNum.FieldName = "SapDocNum";
        colSapHistoryDocNum.Name = "colSapHistoryDocNum";
        colSapHistoryDocNum.Visible = true;
        colSapHistoryDocNum.VisibleIndex = 4;
        colSapHistoryDocNum.Width = 125;
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
        // colSapHistoryDuration
        // 
        colSapHistoryDuration.Caption = "Duración";
        colSapHistoryDuration.FieldName = "Duration";
        colSapHistoryDuration.Name = "colSapHistoryDuration";
        colSapHistoryDuration.Visible = true;
        colSapHistoryDuration.VisibleIndex = 6;
        colSapHistoryDuration.Width = 85;
        // 
        // colSapHistoryTracking
        // 
        colSapHistoryTracking.Caption = "Seguimiento";
        colSapHistoryTracking.FieldName = "Tracking";
        colSapHistoryTracking.Name = "colSapHistoryTracking";
        colSapHistoryTracking.Visible = true;
        colSapHistoryTracking.VisibleIndex = 7;
        colSapHistoryTracking.Width = 135;
        // 
        // colSapHistoryMessage
        // 
        colSapHistoryMessage.Caption = "Mensaje";
        colSapHistoryMessage.FieldName = "Message";
        colSapHistoryMessage.Name = "colSapHistoryMessage";
        colSapHistoryMessage.Visible = true;
        colSapHistoryMessage.VisibleIndex = 8;
        colSapHistoryMessage.Width = 260;
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
        tabLots.Appearance.PageClient.BackColor = Color.White;
        tabLots.Appearance.PageClient.Options.UseBackColor = true;
        tabLots.Controls.Add(sepTraceabilityHeader);
        tabLots.Controls.Add(sepTraceabilityColumnOne);
        tabLots.Controls.Add(sepTraceabilityColumnTwo);
        tabLots.Controls.Add(sepTraceabilityGeneration);
        tabLots.Controls.Add(sepTraceabilityExpiration);
        tabLots.Controls.Add(sepTraceabilityOperations);
        tabLots.Controls.Add(lblInheritedTraceabilityTitle);
        tabLots.Controls.Add(lblInheritedBatchStatus);
        tabLots.Controls.Add(lblInheritedSerialStatus);
        tabLots.Controls.Add(lblInheritedPerishableStatus);
        tabLots.Controls.Add(lblInheritedExpirationStatus);
        tabLots.Controls.Add(lblLotOperationalRulesTitle);
        tabLots.Controls.Add(lblIssueMethod);
        tabLots.Controls.Add(lueIssueMethod);
        tabLots.Controls.Add(lblAllowMultipleBatches);
        tabLots.Controls.Add(tglAllowMultipleBatches);
        tabLots.Controls.Add(lblAllowReceiptWithoutLot);
        tabLots.Controls.Add(tglAllowReceiptWithoutLot);
        tabLots.Controls.Add(lblAllowExpiredBatchSale);
        tabLots.Controls.Add(tglAllowExpiredBatchSale);
        tabLots.Controls.Add(lblBlockQuarantineBatch);
        tabLots.Controls.Add(tglBlockQuarantineBatch);
        tabLots.Controls.Add(lblBlockExpiredBatch);
        tabLots.Controls.Add(tglBlockExpiredBatch);
        tabLots.Controls.Add(lblLotOperationalNotes);
        tabLots.Controls.Add(memLotOperationalNotes);
        tabLots.Controls.Add(lblLotTraceabilityTitle);
        tabLots.Controls.Add(lblLotExpirationTitle);
        tabLots.Controls.Add(lblRequiresExpiration);
        tabLots.Controls.Add(tglRequiresExpiration);
        tabLots.Controls.Add(lblExpirationMandatory);
        tabLots.Controls.Add(tglExpirationMandatory);
        tabLots.Controls.Add(lblManufacturingDateRequired);
        tabLots.Controls.Add(tglManufacturingDateRequired);
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
        tabLots.Controls.Add(lblLotTransferRuleIcon);
        tabLots.Controls.Add(lblLotTransferRule);
        tabLots.Controls.Add(lblSerialDispatchRuleIcon);
        tabLots.Controls.Add(lblSerialDispatchRule);
        tabLots.Controls.Add(lblTraceabilityFooterIcon);
        tabLots.Controls.Add(lblTraceabilityFooter);
        tabLots.ImageOptions.SvgImageSize = new Size(22, 22);
        tabLots.Name = "tabLots";
        tabLots.Size = new Size(1406, 426);
        tabLots.Text = "Trazabilidad";
        // 
        // sepTraceabilityHeader
        // 
        sepTraceabilityHeader.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)225, (int)(byte)232);
        sepTraceabilityHeader.Appearance.Options.UseBackColor = true;
        sepTraceabilityHeader.AutoSizeMode = LabelAutoSizeMode.None;
        sepTraceabilityHeader.Location = new Point(18, 49);
        sepTraceabilityHeader.Name = "sepTraceabilityHeader";
        sepTraceabilityHeader.Size = new Size(1368, 1);
        sepTraceabilityHeader.TabIndex = 90;
        // 
        // sepTraceabilityColumnOne
        // 
        sepTraceabilityColumnOne.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)225, (int)(byte)232);
        sepTraceabilityColumnOne.Appearance.Options.UseBackColor = true;
        sepTraceabilityColumnOne.AutoSizeMode = LabelAutoSizeMode.None;
        sepTraceabilityColumnOne.Location = new Point(432, 70);
        sepTraceabilityColumnOne.Name = "sepTraceabilityColumnOne";
        sepTraceabilityColumnOne.Size = new Size(1, 286);
        sepTraceabilityColumnOne.TabIndex = 91;
        // 
        // sepTraceabilityColumnTwo
        // 
        sepTraceabilityColumnTwo.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)225, (int)(byte)232);
        sepTraceabilityColumnTwo.Appearance.Options.UseBackColor = true;
        sepTraceabilityColumnTwo.AutoSizeMode = LabelAutoSizeMode.None;
        sepTraceabilityColumnTwo.Location = new Point(842, 70);
        sepTraceabilityColumnTwo.Name = "sepTraceabilityColumnTwo";
        sepTraceabilityColumnTwo.Size = new Size(1, 286);
        sepTraceabilityColumnTwo.TabIndex = 92;
        // 
        // sepTraceabilityGeneration
        // 
        sepTraceabilityGeneration.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)225, (int)(byte)232);
        sepTraceabilityGeneration.Appearance.Options.UseBackColor = true;
        sepTraceabilityGeneration.AutoSizeMode = LabelAutoSizeMode.None;
        sepTraceabilityGeneration.Location = new Point(252, 79);
        sepTraceabilityGeneration.Name = "sepTraceabilityGeneration";
        sepTraceabilityGeneration.Size = new Size(160, 1);
        sepTraceabilityGeneration.TabIndex = 93;
        // 
        // sepTraceabilityExpiration
        // 
        sepTraceabilityExpiration.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)225, (int)(byte)232);
        sepTraceabilityExpiration.Appearance.Options.UseBackColor = true;
        sepTraceabilityExpiration.AutoSizeMode = LabelAutoSizeMode.None;
        sepTraceabilityExpiration.Location = new Point(641, 79);
        sepTraceabilityExpiration.Name = "sepTraceabilityExpiration";
        sepTraceabilityExpiration.Size = new Size(180, 1);
        sepTraceabilityExpiration.TabIndex = 94;
        // 
        // sepTraceabilityOperations
        // 
        sepTraceabilityOperations.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)225, (int)(byte)232);
        sepTraceabilityOperations.Appearance.Options.UseBackColor = true;
        sepTraceabilityOperations.AutoSizeMode = LabelAutoSizeMode.None;
        sepTraceabilityOperations.Location = new Point(1010, 79);
        sepTraceabilityOperations.Name = "sepTraceabilityOperations";
        sepTraceabilityOperations.Size = new Size(376, 1);
        sepTraceabilityOperations.TabIndex = 95;
        // 
        // lblInheritedTraceabilityTitle
        // 
        lblInheritedTraceabilityTitle.Appearance.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblInheritedTraceabilityTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)31, (int)(byte)42, (int)(byte)68);
        lblInheritedTraceabilityTitle.Appearance.Options.UseFont = true;
        lblInheritedTraceabilityTitle.Appearance.Options.UseForeColor = true;
        lblInheritedTraceabilityTitle.Location = new Point(18, 18);
        lblInheritedTraceabilityTitle.Name = "lblInheritedTraceabilityTitle";
        lblInheritedTraceabilityTitle.Size = new Size(217, 17);
        lblInheritedTraceabilityTitle.TabIndex = 96;
        lblInheritedTraceabilityTitle.Text = "Configuración heredada de General:";
        // 
        // lblInheritedBatchStatus
        // 
        lblInheritedBatchStatus.AllowHtmlString = true;
        lblInheritedBatchStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblInheritedBatchStatus.Appearance.Options.UseFont = true;
        lblInheritedBatchStatus.Location = new Point(275, 19);
        lblInheritedBatchStatus.Name = "lblInheritedBatchStatus";
        lblInheritedBatchStatus.Size = new Size(118, 15);
        lblInheritedBatchStatus.TabIndex = 97;
        lblInheritedBatchStatus.Text = "Control por lote   ✓ Sí";
        // 
        // lblInheritedSerialStatus
        // 
        lblInheritedSerialStatus.AllowHtmlString = true;
        lblInheritedSerialStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblInheritedSerialStatus.Appearance.Options.UseFont = true;
        lblInheritedSerialStatus.Location = new Point(446, 19);
        lblInheritedSerialStatus.Name = "lblInheritedSerialStatus";
        lblInheritedSerialStatus.Size = new Size(124, 15);
        lblInheritedSerialStatus.TabIndex = 98;
        lblInheritedSerialStatus.Text = "Control por serie   × No";
        // 
        // lblInheritedPerishableStatus
        // 
        lblInheritedPerishableStatus.AllowHtmlString = true;
        lblInheritedPerishableStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblInheritedPerishableStatus.Appearance.Options.UseFont = true;
        lblInheritedPerishableStatus.Location = new Point(617, 19);
        lblInheritedPerishableStatus.Name = "lblInheritedPerishableStatus";
        lblInheritedPerishableStatus.Size = new Size(82, 15);
        lblInheritedPerishableStatus.TabIndex = 99;
        lblInheritedPerishableStatus.Text = "Perecible   ✓ Sí";
        // 
        // lblInheritedExpirationStatus
        // 
        lblInheritedExpirationStatus.AllowHtmlString = true;
        lblInheritedExpirationStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblInheritedExpirationStatus.Appearance.Options.UseFont = true;
        lblInheritedExpirationStatus.Location = new Point(750, 19);
        lblInheritedExpirationStatus.Name = "lblInheritedExpirationStatus";
        lblInheritedExpirationStatus.Size = new Size(142, 15);
        lblInheritedExpirationStatus.TabIndex = 100;
        lblInheritedExpirationStatus.Text = "Maneja vencimiento   ✓ Sí";
        // 
        // lblLotOperationalRulesTitle
        // 
        lblLotOperationalRulesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblLotOperationalRulesTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblLotOperationalRulesTitle.Appearance.Options.UseFont = true;
        lblLotOperationalRulesTitle.Appearance.Options.UseForeColor = true;
        lblLotOperationalRulesTitle.Location = new Point(870, 69);
        lblLotOperationalRulesTitle.Name = "lblLotOperationalRulesTitle";
        lblLotOperationalRulesTitle.Size = new Size(135, 20);
        lblLotOperationalRulesTitle.TabIndex = 52;
        lblLotOperationalRulesTitle.Text = "3. Reglas operativas";
        // 
        // lblIssueMethod
        // 
        lblIssueMethod.Appearance.Font = new Font("Segoe UI", 9F);
        lblIssueMethod.Appearance.Options.UseFont = true;
        lblIssueMethod.Location = new Point(870, 107);
        lblIssueMethod.Name = "lblIssueMethod";
        lblIssueMethod.Size = new Size(94, 15);
        lblIssueMethod.TabIndex = 53;
        lblIssueMethod.Text = "Método de salida:";
        // 
        // lueIssueMethod
        // 
        lueIssueMethod.EditValue = "FEFO - Primero en vencer";
        lueIssueMethod.Location = new Point(1080, 104);
        lueIssueMethod.Name = "lueIssueMethod";
        lueIssueMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueIssueMethod.Properties.Appearance.Options.UseFont = true;
        lueIssueMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueIssueMethod.Properties.NullText = "";
        lueIssueMethod.Size = new Size(280, 22);
        lueIssueMethod.TabIndex = 54;
        // 
        // lblAllowMultipleBatches
        // 
        lblAllowMultipleBatches.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowMultipleBatches.Appearance.Options.UseFont = true;
        lblAllowMultipleBatches.Location = new Point(870, 134);
        lblAllowMultipleBatches.Name = "lblAllowMultipleBatches";
        lblAllowMultipleBatches.Size = new Size(122, 15);
        lblAllowMultipleBatches.TabIndex = 55;
        lblAllowMultipleBatches.Text = "Admite múltiples lotes:";
        // 
        // tglAllowMultipleBatches
        // 
        tglAllowMultipleBatches.EditValue = true;
        tglAllowMultipleBatches.Location = new Point(1080, 132);
        tglAllowMultipleBatches.Name = "tglAllowMultipleBatches";
        tglAllowMultipleBatches.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAllowMultipleBatches.Properties.Appearance.Options.UseFont = true;
        tglAllowMultipleBatches.Properties.OffText = "No";
        tglAllowMultipleBatches.Properties.OnText = "Sí";
        tglAllowMultipleBatches.Size = new Size(86, 20);
        tglAllowMultipleBatches.TabIndex = 56;
        // 
        // lblAllowReceiptWithoutLot
        // 
        lblAllowReceiptWithoutLot.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowReceiptWithoutLot.Appearance.Options.UseFont = true;
        lblAllowReceiptWithoutLot.Location = new Point(870, 162);
        lblAllowReceiptWithoutLot.Name = "lblAllowReceiptWithoutLot";
        lblAllowReceiptWithoutLot.Size = new Size(140, 15);
        lblAllowReceiptWithoutLot.TabIndex = 57;
        lblAllowReceiptWithoutLot.Text = "Permite recepción sin lote:";
        // 
        // tglAllowReceiptWithoutLot
        // 
        tglAllowReceiptWithoutLot.Location = new Point(1080, 160);
        tglAllowReceiptWithoutLot.Name = "tglAllowReceiptWithoutLot";
        tglAllowReceiptWithoutLot.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAllowReceiptWithoutLot.Properties.Appearance.Options.UseFont = true;
        tglAllowReceiptWithoutLot.Properties.OffText = "No";
        tglAllowReceiptWithoutLot.Properties.OnText = "Sí";
        tglAllowReceiptWithoutLot.Size = new Size(86, 20);
        tglAllowReceiptWithoutLot.TabIndex = 58;
        // 
        // lblAllowExpiredBatchSale
        // 
        lblAllowExpiredBatchSale.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowExpiredBatchSale.Appearance.Options.UseFont = true;
        lblAllowExpiredBatchSale.Location = new Point(870, 190);
        lblAllowExpiredBatchSale.Name = "lblAllowExpiredBatchSale";
        lblAllowExpiredBatchSale.Size = new Size(163, 15);
        lblAllowExpiredBatchSale.TabIndex = 59;
        lblAllowExpiredBatchSale.Text = "Permite venta del lote vencido:";
        // 
        // tglAllowExpiredBatchSale
        // 
        tglAllowExpiredBatchSale.Location = new Point(1080, 188);
        tglAllowExpiredBatchSale.Name = "tglAllowExpiredBatchSale";
        tglAllowExpiredBatchSale.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAllowExpiredBatchSale.Properties.Appearance.Options.UseFont = true;
        tglAllowExpiredBatchSale.Properties.OffText = "No";
        tglAllowExpiredBatchSale.Properties.OnText = "Sí";
        tglAllowExpiredBatchSale.Size = new Size(86, 20);
        tglAllowExpiredBatchSale.TabIndex = 60;
        // 
        // lblBlockQuarantineBatch
        // 
        lblBlockQuarantineBatch.Appearance.Font = new Font("Segoe UI", 9F);
        lblBlockQuarantineBatch.Appearance.Options.UseFont = true;
        lblBlockQuarantineBatch.Location = new Point(450, 273);
        lblBlockQuarantineBatch.Name = "lblBlockQuarantineBatch";
        lblBlockQuarantineBatch.Size = new Size(147, 15);
        lblBlockQuarantineBatch.TabIndex = 69;
        lblBlockQuarantineBatch.Text = "Bloquea lote en cuarentena:";
        // 
        // tglBlockQuarantineBatch
        // 
        tglBlockQuarantineBatch.EditValue = true;
        tglBlockQuarantineBatch.Location = new Point(690, 271);
        tglBlockQuarantineBatch.Name = "tglBlockQuarantineBatch";
        tglBlockQuarantineBatch.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglBlockQuarantineBatch.Properties.Appearance.Options.UseFont = true;
        tglBlockQuarantineBatch.Properties.OffText = "No";
        tglBlockQuarantineBatch.Properties.OnText = "Sí";
        tglBlockQuarantineBatch.Size = new Size(86, 20);
        tglBlockQuarantineBatch.TabIndex = 70;
        // 
        // lblBlockExpiredBatch
        // 
        lblBlockExpiredBatch.Appearance.Font = new Font("Segoe UI", 9F);
        lblBlockExpiredBatch.Appearance.Options.UseFont = true;
        lblBlockExpiredBatch.Location = new Point(450, 299);
        lblBlockExpiredBatch.Name = "lblBlockExpiredBatch";
        lblBlockExpiredBatch.Size = new Size(114, 15);
        lblBlockExpiredBatch.TabIndex = 71;
        lblBlockExpiredBatch.Text = "Bloquea lote vencido:";
        // 
        // tglBlockExpiredBatch
        // 
        tglBlockExpiredBatch.EditValue = true;
        tglBlockExpiredBatch.Location = new Point(690, 297);
        tglBlockExpiredBatch.Name = "tglBlockExpiredBatch";
        tglBlockExpiredBatch.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglBlockExpiredBatch.Properties.Appearance.Options.UseFont = true;
        tglBlockExpiredBatch.Properties.OffText = "No";
        tglBlockExpiredBatch.Properties.OnText = "Sí";
        tglBlockExpiredBatch.Size = new Size(86, 20);
        tglBlockExpiredBatch.TabIndex = 72;
        // 
        // lblLotOperationalNotes
        // 
        lblLotOperationalNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblLotOperationalNotes.Appearance.Options.UseFont = true;
        lblLotOperationalNotes.Location = new Point(870, 218);
        lblLotOperationalNotes.Name = "lblLotOperationalNotes";
        lblLotOperationalNotes.Size = new Size(121, 15);
        lblLotOperationalNotes.TabIndex = 73;
        lblLotOperationalNotes.Text = "Observación operativa:";
        // 
        // memLotOperationalNotes
        // 
        memLotOperationalNotes.EditValue = "Usar método FEFO para despacho.\r\nNo se permite vender lotes vencidos.\r\nCumplir con días de cuarentena en recepción.";
        memLotOperationalNotes.Location = new Point(1080, 216);
        memLotOperationalNotes.Name = "memLotOperationalNotes";
        memLotOperationalNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memLotOperationalNotes.Properties.Appearance.Options.UseFont = true;
        memLotOperationalNotes.Size = new Size(280, 70);
        memLotOperationalNotes.TabIndex = 74;
        // 
        // lblLotTraceabilityTitle
        // 
        lblLotTraceabilityTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblLotTraceabilityTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblLotTraceabilityTitle.Appearance.Options.UseFont = true;
        lblLotTraceabilityTitle.Appearance.Options.UseForeColor = true;
        lblLotTraceabilityTitle.Location = new Point(18, 69);
        lblLotTraceabilityTitle.Name = "lblLotTraceabilityTitle";
        lblLotTraceabilityTitle.Size = new Size(200, 20);
        lblLotTraceabilityTitle.TabIndex = 26;
        lblLotTraceabilityTitle.Text = "1. Generación e identificación";
        // 
        // lblLotExpirationTitle
        // 
        lblLotExpirationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblLotExpirationTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblLotExpirationTitle.Appearance.Options.UseFont = true;
        lblLotExpirationTitle.Appearance.Options.UseForeColor = true;
        lblLotExpirationTitle.Location = new Point(450, 69);
        lblLotExpirationTitle.Name = "lblLotExpirationTitle";
        lblLotExpirationTitle.Size = new Size(167, 20);
        lblLotExpirationTitle.TabIndex = 27;
        lblLotExpirationTitle.Text = "2. Vencimiento y calidad";
        // 
        // lblRequiresExpiration
        // 
        lblRequiresExpiration.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiresExpiration.Appearance.Options.UseFont = true;
        lblRequiresExpiration.Location = new Point(450, 107);
        lblRequiresExpiration.Name = "lblRequiresExpiration";
        lblRequiresExpiration.Size = new Size(118, 15);
        lblRequiresExpiration.TabIndex = 31;
        lblRequiresExpiration.Text = "Requiere vencimiento:";
        // 
        // tglRequiresExpiration
        // 
        tglRequiresExpiration.EditValue = true;
        tglRequiresExpiration.Location = new Point(690, 105);
        tglRequiresExpiration.Name = "tglRequiresExpiration";
        tglRequiresExpiration.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglRequiresExpiration.Properties.Appearance.Options.UseFont = true;
        tglRequiresExpiration.Properties.OffText = "No";
        tglRequiresExpiration.Properties.OnText = "Sí";
        tglRequiresExpiration.Size = new Size(86, 20);
        tglRequiresExpiration.TabIndex = 32;
        // 
        // lblExpirationMandatory
        // 
        lblExpirationMandatory.Appearance.Font = new Font("Segoe UI", 9F);
        lblExpirationMandatory.Appearance.Options.UseFont = true;
        lblExpirationMandatory.Location = new Point(450, 134);
        lblExpirationMandatory.Name = "lblExpirationMandatory";
        lblExpirationMandatory.Size = new Size(131, 15);
        lblExpirationMandatory.TabIndex = 33;
        lblExpirationMandatory.Text = "Vencimiento obligatorio:";
        // 
        // tglExpirationMandatory
        // 
        tglExpirationMandatory.EditValue = true;
        tglExpirationMandatory.Location = new Point(690, 132);
        tglExpirationMandatory.Name = "tglExpirationMandatory";
        tglExpirationMandatory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglExpirationMandatory.Properties.Appearance.Options.UseFont = true;
        tglExpirationMandatory.Properties.OffText = "No";
        tglExpirationMandatory.Properties.OnText = "Sí";
        tglExpirationMandatory.Size = new Size(86, 20);
        tglExpirationMandatory.TabIndex = 34;
        // 
        // lblManufacturingDateRequired
        // 
        lblManufacturingDateRequired.Appearance.Font = new Font("Segoe UI", 9F);
        lblManufacturingDateRequired.Appearance.Options.UseFont = true;
        lblManufacturingDateRequired.Location = new Point(450, 162);
        lblManufacturingDateRequired.Name = "lblManufacturingDateRequired";
        lblManufacturingDateRequired.Size = new Size(156, 15);
        lblManufacturingDateRequired.TabIndex = 35;
        lblManufacturingDateRequired.Text = "Fecha fabricación obligatoria:";
        // 
        // tglManufacturingDateRequired
        // 
        tglManufacturingDateRequired.EditValue = true;
        tglManufacturingDateRequired.Location = new Point(690, 160);
        tglManufacturingDateRequired.Name = "tglManufacturingDateRequired";
        tglManufacturingDateRequired.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglManufacturingDateRequired.Properties.Appearance.Options.UseFont = true;
        tglManufacturingDateRequired.Properties.OffText = "No";
        tglManufacturingDateRequired.Properties.OnText = "Sí";
        tglManufacturingDateRequired.Size = new Size(86, 20);
        tglManufacturingDateRequired.TabIndex = 36;
        // 
        // lblAutoGenerateBatch
        // 
        lblAutoGenerateBatch.Appearance.Font = new Font("Segoe UI", 9F);
        lblAutoGenerateBatch.Appearance.Options.UseFont = true;
        lblAutoGenerateBatch.Location = new Point(18, 107);
        lblAutoGenerateBatch.Name = "lblAutoGenerateBatch";
        lblAutoGenerateBatch.Size = new Size(160, 15);
        lblAutoGenerateBatch.TabIndex = 35;
        lblAutoGenerateBatch.Text = "Genera lote automáticamente:";
        // 
        // tglAutoGenerateBatch
        // 
        tglAutoGenerateBatch.EditValue = true;
        tglAutoGenerateBatch.Location = new Point(220, 105);
        tglAutoGenerateBatch.Name = "tglAutoGenerateBatch";
        tglAutoGenerateBatch.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAutoGenerateBatch.Properties.Appearance.Options.UseFont = true;
        tglAutoGenerateBatch.Properties.OffText = "No";
        tglAutoGenerateBatch.Properties.OnText = "Sí";
        tglAutoGenerateBatch.Size = new Size(86, 20);
        tglAutoGenerateBatch.TabIndex = 36;
        // 
        // lblBatchPrefix
        // 
        lblBatchPrefix.Appearance.Font = new Font("Segoe UI", 9F);
        lblBatchPrefix.Appearance.Options.UseFont = true;
        lblBatchPrefix.Location = new Point(18, 134);
        lblBatchPrefix.Name = "lblBatchPrefix";
        lblBatchPrefix.Size = new Size(76, 15);
        lblBatchPrefix.TabIndex = 37;
        lblBatchPrefix.Text = "Prefijo de lote:";
        // 
        // txtBatchPrefix
        // 
        txtBatchPrefix.EditValue = "LOT-";
        txtBatchPrefix.Location = new Point(220, 131);
        txtBatchPrefix.Name = "txtBatchPrefix";
        txtBatchPrefix.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBatchPrefix.Properties.Appearance.Options.UseFont = true;
        txtBatchPrefix.Size = new Size(190, 22);
        txtBatchPrefix.TabIndex = 38;
        // 
        // lblSerialLength
        // 
        lblSerialLength.Appearance.Font = new Font("Segoe UI", 9F);
        lblSerialLength.Appearance.Options.UseFont = true;
        lblSerialLength.Location = new Point(18, 218);
        lblSerialLength.Name = "lblSerialLength";
        lblSerialLength.Size = new Size(94, 15);
        lblSerialLength.TabIndex = 39;
        lblSerialLength.Text = "Longitud de serie:";
        // 
        // spnSerialLength
        // 
        spnSerialLength.EditValue = new decimal(new int[] { 12, 0, 0, 0 });
        spnSerialLength.Location = new Point(220, 215);
        spnSerialLength.Name = "spnSerialLength";
        spnSerialLength.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSerialLength.Properties.Appearance.Options.UseFont = true;
        spnSerialLength.Properties.Appearance.Options.UseTextOptions = true;
        spnSerialLength.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSerialLength.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSerialLength.Properties.IsFloatValue = false;
        spnSerialLength.Properties.MaskSettings.Set("mask", "N00");
        spnSerialLength.Size = new Size(190, 22);
        spnSerialLength.TabIndex = 40;
        // 
        // lblShelfLifeDays
        // 
        lblShelfLifeDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblShelfLifeDays.Appearance.Options.UseFont = true;
        lblShelfLifeDays.Location = new Point(450, 190);
        lblShelfLifeDays.Name = "lblShelfLifeDays";
        lblShelfLifeDays.Size = new Size(120, 15);
        lblShelfLifeDays.TabIndex = 41;
        lblShelfLifeDays.Text = "Vida útil del lote (días):";
        // 
        // spnShelfLifeDays
        // 
        spnShelfLifeDays.EditValue = new decimal(new int[] { 180, 0, 0, 0 });
        spnShelfLifeDays.Location = new Point(690, 187);
        spnShelfLifeDays.Name = "spnShelfLifeDays";
        spnShelfLifeDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnShelfLifeDays.Properties.Appearance.Options.UseFont = true;
        spnShelfLifeDays.Properties.Appearance.Options.UseTextOptions = true;
        spnShelfLifeDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnShelfLifeDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnShelfLifeDays.Properties.IsFloatValue = false;
        spnShelfLifeDays.Properties.MaskSettings.Set("mask", "N00");
        spnShelfLifeDays.Size = new Size(120, 22);
        spnShelfLifeDays.TabIndex = 42;
        // 
        // lblExpirationAlertDays
        // 
        lblExpirationAlertDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblExpirationAlertDays.Appearance.Options.UseFont = true;
        lblExpirationAlertDays.Location = new Point(450, 218);
        lblExpirationAlertDays.Name = "lblExpirationAlertDays";
        lblExpirationAlertDays.Size = new Size(151, 15);
        lblExpirationAlertDays.TabIndex = 43;
        lblExpirationAlertDays.Text = "Alerta de vencimiento (días):";
        // 
        // spnExpirationAlertDays
        // 
        spnExpirationAlertDays.EditValue = new decimal(new int[] { 30, 0, 0, 0 });
        spnExpirationAlertDays.Location = new Point(690, 215);
        spnExpirationAlertDays.Name = "spnExpirationAlertDays";
        spnExpirationAlertDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnExpirationAlertDays.Properties.Appearance.Options.UseFont = true;
        spnExpirationAlertDays.Properties.Appearance.Options.UseTextOptions = true;
        spnExpirationAlertDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnExpirationAlertDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnExpirationAlertDays.Properties.IsFloatValue = false;
        spnExpirationAlertDays.Properties.MaskSettings.Set("mask", "N00");
        spnExpirationAlertDays.Size = new Size(120, 22);
        spnExpirationAlertDays.TabIndex = 44;
        // 
        // lblQuarantineDays
        // 
        lblQuarantineDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblQuarantineDays.Appearance.Options.UseFont = true;
        lblQuarantineDays.Location = new Point(450, 246);
        lblQuarantineDays.Name = "lblQuarantineDays";
        lblQuarantineDays.Size = new Size(96, 15);
        lblQuarantineDays.TabIndex = 45;
        lblQuarantineDays.Text = "Cuarentena (días):";
        // 
        // spnQuarantineDays
        // 
        spnQuarantineDays.EditValue = new decimal(new int[] { 5, 0, 0, 0 });
        spnQuarantineDays.Location = new Point(690, 243);
        spnQuarantineDays.Name = "spnQuarantineDays";
        spnQuarantineDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnQuarantineDays.Properties.Appearance.Options.UseFont = true;
        spnQuarantineDays.Properties.Appearance.Options.UseTextOptions = true;
        spnQuarantineDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnQuarantineDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnQuarantineDays.Properties.IsFloatValue = false;
        spnQuarantineDays.Properties.MaskSettings.Set("mask", "N00");
        spnQuarantineDays.Size = new Size(120, 22);
        spnQuarantineDays.TabIndex = 46;
        // 
        // lblBatchFormat
        // 
        lblBatchFormat.Appearance.Font = new Font("Segoe UI", 9F);
        lblBatchFormat.Appearance.Options.UseFont = true;
        lblBatchFormat.Location = new Point(18, 162);
        lblBatchFormat.Name = "lblBatchFormat";
        lblBatchFormat.Size = new Size(87, 15);
        lblBatchFormat.TabIndex = 47;
        lblBatchFormat.Text = "Formato de lote:";
        // 
        // txtBatchFormat
        // 
        txtBatchFormat.EditValue = "yyyyMMdd-####";
        txtBatchFormat.Location = new Point(220, 159);
        txtBatchFormat.Name = "txtBatchFormat";
        txtBatchFormat.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBatchFormat.Properties.Appearance.Options.UseFont = true;
        txtBatchFormat.Size = new Size(190, 22);
        txtBatchFormat.TabIndex = 48;
        // 
        // lblNumberingMethod
        // 
        lblNumberingMethod.Appearance.Font = new Font("Segoe UI", 9F);
        lblNumberingMethod.Appearance.Options.UseFont = true;
        lblNumberingMethod.Location = new Point(18, 190);
        lblNumberingMethod.Name = "lblNumberingMethod";
        lblNumberingMethod.Size = new Size(128, 15);
        lblNumberingMethod.TabIndex = 49;
        lblNumberingMethod.Text = "Método de numeración:";
        // 
        // lueNumberingMethod
        // 
        lueNumberingMethod.EditValue = "Automático por recepción";
        lueNumberingMethod.Location = new Point(220, 187);
        lueNumberingMethod.Name = "lueNumberingMethod";
        lueNumberingMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueNumberingMethod.Properties.Appearance.Options.UseFont = true;
        lueNumberingMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueNumberingMethod.Properties.NullText = "";
        lueNumberingMethod.Size = new Size(190, 22);
        lueNumberingMethod.TabIndex = 50;
        // 
        // lblLotTransferRuleIcon
        // 
        lblLotTransferRuleIcon.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblLotTransferRuleIcon.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblLotTransferRuleIcon.Appearance.Options.UseFont = true;
        lblLotTransferRuleIcon.Appearance.Options.UseForeColor = true;
        lblLotTransferRuleIcon.Location = new Point(864, 301);
        lblLotTransferRuleIcon.Name = "lblLotTransferRuleIcon";
        lblLotTransferRuleIcon.Size = new Size(16, 20);
        lblLotTransferRuleIcon.TabIndex = 101;
        lblLotTransferRuleIcon.Text = "↗";
        // 
        // lblLotTransferRule
        // 
        lblLotTransferRule.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblLotTransferRule.Appearance.ForeColor = Color.FromArgb((int)(byte)31, (int)(byte)42, (int)(byte)68);
        lblLotTransferRule.Appearance.Options.UseFont = true;
        lblLotTransferRule.Appearance.Options.UseForeColor = true;
        lblLotTransferRule.Location = new Point(889, 304);
        lblLotTransferRule.Name = "lblLotTransferRule";
        lblLotTransferRule.Size = new Size(296, 13);
        lblLotTransferRule.TabIndex = 102;
        lblLotTransferRule.Text = "Lote en transferencias: según Control por lote de General";
        // 
        // lblSerialDispatchRuleIcon
        // 
        lblSerialDispatchRuleIcon.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSerialDispatchRuleIcon.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSerialDispatchRuleIcon.Appearance.Options.UseFont = true;
        lblSerialDispatchRuleIcon.Appearance.Options.UseForeColor = true;
        lblSerialDispatchRuleIcon.Location = new Point(864, 326);
        lblSerialDispatchRuleIcon.Name = "lblSerialDispatchRuleIcon";
        lblSerialDispatchRuleIcon.Size = new Size(16, 20);
        lblSerialDispatchRuleIcon.TabIndex = 103;
        lblSerialDispatchRuleIcon.Text = "↗";
        // 
        // lblSerialDispatchRule
        // 
        lblSerialDispatchRule.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblSerialDispatchRule.Appearance.ForeColor = Color.FromArgb((int)(byte)31, (int)(byte)42, (int)(byte)68);
        lblSerialDispatchRule.Appearance.Options.UseFont = true;
        lblSerialDispatchRule.Appearance.Options.UseForeColor = true;
        lblSerialDispatchRule.Location = new Point(889, 329);
        lblSerialDispatchRule.Name = "lblSerialDispatchRule";
        lblSerialDispatchRule.Size = new Size(281, 13);
        lblSerialDispatchRule.TabIndex = 104;
        lblSerialDispatchRule.Text = "Serie en despacho: según Control por serie de General";
        // 
        // lblTraceabilityFooterIcon
        // 
        lblTraceabilityFooterIcon.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblTraceabilityFooterIcon.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblTraceabilityFooterIcon.Appearance.Options.UseFont = true;
        lblTraceabilityFooterIcon.Appearance.Options.UseForeColor = true;
        lblTraceabilityFooterIcon.Location = new Point(26, 369);
        lblTraceabilityFooterIcon.Name = "lblTraceabilityFooterIcon";
        lblTraceabilityFooterIcon.Size = new Size(16, 21);
        lblTraceabilityFooterIcon.TabIndex = 105;
        lblTraceabilityFooterIcon.Text = "ⓘ";
        // 
        // lblTraceabilityFooter
        // 
        lblTraceabilityFooter.Appearance.Font = new Font("Segoe UI", 9F);
        lblTraceabilityFooter.Appearance.ForeColor = Color.FromArgb((int)(byte)31, (int)(byte)42, (int)(byte)68);
        lblTraceabilityFooter.Appearance.Options.UseFont = true;
        lblTraceabilityFooter.Appearance.Options.UseForeColor = true;
        lblTraceabilityFooter.Location = new Point(58, 373);
        lblTraceabilityFooter.Name = "lblTraceabilityFooter";
        lblTraceabilityFooter.Size = new Size(438, 15);
        lblTraceabilityFooter.TabIndex = 106;
        lblTraceabilityFooter.Text = "La configuración se aplicará en recepciones, transferencias, inventario y despachos.";
        // 
        // pnlLotOperationalNote
        // 
        pnlLotOperationalNote.Appearance.BackColor = Color.FromArgb((int)(byte)238, (int)(byte)248, (int)(byte)255);
        pnlLotOperationalNote.Appearance.Options.UseBackColor = true;
        pnlLotOperationalNote.Controls.Add(lblLotOperationalNoteIcon);
        pnlLotOperationalNote.Controls.Add(lblLotOperationalNote);
        pnlLotOperationalNote.Location = new Point(374, 268);
        pnlLotOperationalNote.Name = "pnlLotOperationalNote";
        pnlLotOperationalNote.Size = new Size(346, 26);
        pnlLotOperationalNote.TabIndex = 75;
        // 
        // lblLotOperationalNoteIcon
        // 
        lblLotOperationalNoteIcon.Appearance.BackColor = Color.FromArgb((int)(byte)0, (int)(byte)122, (int)(byte)204);
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
        // lblLotOperationalNote
        // 
        lblLotOperationalNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblLotOperationalNote.Appearance.ForeColor = Color.FromArgb((int)(byte)31, (int)(byte)42, (int)(byte)68);
        lblLotOperationalNote.Appearance.Options.UseFont = true;
        lblLotOperationalNote.Appearance.Options.UseForeColor = true;
        lblLotOperationalNote.Location = new Point(38, 6);
        lblLotOperationalNote.Name = "lblLotOperationalNote";
        lblLotOperationalNote.Size = new Size(321, 13);
        lblLotOperationalNote.TabIndex = 1;
        lblLotOperationalNote.Text = "Reglas usadas en documentos y movimientos con trazabilidad.";
        // 
        // pnlLotTraceabilityNote
        // 
        pnlLotTraceabilityNote.Appearance.BackColor = Color.FromArgb((int)(byte)238, (int)(byte)248, (int)(byte)255);
        pnlLotTraceabilityNote.Appearance.Options.UseBackColor = true;
        pnlLotTraceabilityNote.Controls.Add(lblLotTraceabilityNoteIcon);
        pnlLotTraceabilityNote.Controls.Add(lblLotTraceabilityNote);
        pnlLotTraceabilityNote.Location = new Point(18, 330);
        pnlLotTraceabilityNote.Name = "pnlLotTraceabilityNote";
        pnlLotTraceabilityNote.Size = new Size(304, 34);
        pnlLotTraceabilityNote.TabIndex = 51;
        // 
        // lblLotTraceabilityNoteIcon
        // 
        lblLotTraceabilityNoteIcon.Appearance.BackColor = Color.FromArgb((int)(byte)0, (int)(byte)122, (int)(byte)204);
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
        // lblLotTraceabilityNote
        // 
        lblLotTraceabilityNote.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblLotTraceabilityNote.Appearance.ForeColor = Color.FromArgb((int)(byte)31, (int)(byte)42, (int)(byte)68);
        lblLotTraceabilityNote.Appearance.Options.UseFont = true;
        lblLotTraceabilityNote.Appearance.Options.UseForeColor = true;
        lblLotTraceabilityNote.AutoSizeMode = LabelAutoSizeMode.Vertical;
        lblLotTraceabilityNote.Location = new Point(38, 5);
        lblLotTraceabilityNote.Name = "lblLotTraceabilityNote";
        lblLotTraceabilityNote.Size = new Size(252, 26);
        lblLotTraceabilityNote.TabIndex = 1;
        lblLotTraceabilityNote.Text = "Permite identificar origen, vencimiento, lote o serie de cada movimiento.";
        // 
        // tabTaxes
        // 
        tabTaxes.Appearance.PageClient.BackColor = Color.White;
        tabTaxes.Appearance.PageClient.Options.UseBackColor = true;
        tabTaxes.Controls.Add(sepTaxesColumnOne);
        tabTaxes.Controls.Add(sepTaxesColumnTwo);
        tabTaxes.Controls.Add(sepTaxConfiguration);
        tabTaxes.Controls.Add(sepTaxRates);
        tabTaxes.Controls.Add(sepTaxApplicability);
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
        tabTaxes.Controls.Add(lblTaxRatesTitle);
        tabTaxes.Controls.Add(lblTaxApplicabilityTitle);
        tabTaxes.ImageOptions.SvgImageSize = new Size(20, 20);
        tabTaxes.Name = "tabTaxes";
        tabTaxes.Size = new Size(1402, 398);
        tabTaxes.Text = "Impuestos";
        // 
        // sepTaxesColumnOne
        // 
        sepTaxesColumnOne.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepTaxesColumnOne.Appearance.Options.UseBackColor = true;
        sepTaxesColumnOne.AutoSizeMode = LabelAutoSizeMode.None;
        sepTaxesColumnOne.Location = new Point(440, 12);
        sepTaxesColumnOne.Name = "sepTaxesColumnOne";
        sepTaxesColumnOne.Size = new Size(1, 130);
        sepTaxesColumnOne.TabIndex = 50;
        // 
        // sepTaxesColumnTwo
        // 
        sepTaxesColumnTwo.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepTaxesColumnTwo.Appearance.Options.UseBackColor = true;
        sepTaxesColumnTwo.AutoSizeMode = LabelAutoSizeMode.None;
        sepTaxesColumnTwo.Location = new Point(890, 12);
        sepTaxesColumnTwo.Name = "sepTaxesColumnTwo";
        sepTaxesColumnTwo.Size = new Size(1, 130);
        sepTaxesColumnTwo.TabIndex = 51;
        // 
        // sepTaxConfiguration
        // 
        sepTaxConfiguration.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepTaxConfiguration.Appearance.Options.UseBackColor = true;
        sepTaxConfiguration.AutoSizeMode = LabelAutoSizeMode.None;
        sepTaxConfiguration.Location = new Point(202, 22);
        sepTaxConfiguration.Name = "sepTaxConfiguration";
        sepTaxConfiguration.Size = new Size(218, 1);
        sepTaxConfiguration.TabIndex = 52;
        // 
        // sepTaxRates
        // 
        sepTaxRates.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepTaxRates.Appearance.Options.UseBackColor = true;
        sepTaxRates.AutoSizeMode = LabelAutoSizeMode.None;
        sepTaxRates.Location = new Point(661, 22);
        sepTaxRates.Name = "sepTaxRates";
        sepTaxRates.Size = new Size(209, 1);
        sepTaxRates.TabIndex = 53;
        // 
        // sepTaxApplicability
        // 
        sepTaxApplicability.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        sepTaxApplicability.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepTaxApplicability.Appearance.Options.UseBackColor = true;
        sepTaxApplicability.AutoSizeMode = LabelAutoSizeMode.None;
        sepTaxApplicability.Location = new Point(1036, 22);
        sepTaxApplicability.Name = "sepTaxApplicability";
        sepTaxApplicability.Size = new Size(348, 1);
        sepTaxApplicability.TabIndex = 54;
        // 
        // lblTaxConfigurationTitle
        // 
        lblTaxConfigurationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblTaxConfigurationTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblTaxConfigurationTitle.Appearance.Options.UseFont = true;
        lblTaxConfigurationTitle.Appearance.Options.UseForeColor = true;
        lblTaxConfigurationTitle.Location = new Point(20, 12);
        lblTaxConfigurationTitle.Name = "lblTaxConfigurationTitle";
        lblTaxConfigurationTitle.Size = new Size(166, 20);
        lblTaxConfigurationTitle.TabIndex = 24;
        lblTaxConfigurationTitle.Text = "1. Clasificación tributaria";
        // 
        // lblFiscalItemType
        // 
        lblFiscalItemType.Appearance.Font = new Font("Segoe UI", 9F);
        lblFiscalItemType.Appearance.Options.UseFont = true;
        lblFiscalItemType.Location = new Point(20, 51);
        lblFiscalItemType.Name = "lblFiscalItemType";
        lblFiscalItemType.Size = new Size(119, 15);
        lblFiscalItemType.TabIndex = 25;
        lblFiscalItemType.Text = "Tipo fiscal del artículo:";
        // 
        // lueFiscalItemType
        // 
        lueFiscalItemType.EditValue = "Gravado";
        lueFiscalItemType.Location = new Point(155, 47);
        lueFiscalItemType.Name = "lueFiscalItemType";
        lueFiscalItemType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueFiscalItemType.Properties.Appearance.Options.UseFont = true;
        lueFiscalItemType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFiscalItemType.Properties.NullText = "";
        lueFiscalItemType.Size = new Size(260, 22);
        lueFiscalItemType.TabIndex = 26;
        // 
        // lblPurchaseVat
        // 
        lblPurchaseVat.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseVat.Appearance.Options.UseFont = true;
        lblPurchaseVat.Location = new Point(465, 51);
        lblPurchaseVat.Name = "lblPurchaseVat";
        lblPurchaseVat.Size = new Size(65, 15);
        lblPurchaseVat.TabIndex = 27;
        lblPurchaseVat.Text = "IVA compra:";
        // 
        // luePurchaseVat
        // 
        luePurchaseVat.EditValue = "IVA 15% - Credito tributario";
        luePurchaseVat.Location = new Point(590, 47);
        luePurchaseVat.Name = "luePurchaseVat";
        luePurchaseVat.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseVat.Properties.Appearance.Options.UseFont = true;
        luePurchaseVat.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseVat.Properties.NullText = "";
        luePurchaseVat.Size = new Size(270, 22);
        luePurchaseVat.TabIndex = 28;
        // 
        // lblTaxesSalesVat
        // 
        lblTaxesSalesVat.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxesSalesVat.Appearance.Options.UseFont = true;
        lblTaxesSalesVat.Location = new Point(465, 79);
        lblTaxesSalesVat.Name = "lblTaxesSalesVat";
        lblTaxesSalesVat.Size = new Size(53, 15);
        lblTaxesSalesVat.TabIndex = 29;
        lblTaxesSalesVat.Text = "IVA venta:";
        // 
        // lueTaxesSalesVat
        // 
        lueTaxesSalesVat.EditValue = "IVA 15% - Tarifa general";
        lueTaxesSalesVat.Location = new Point(590, 75);
        lueTaxesSalesVat.Name = "lueTaxesSalesVat";
        lueTaxesSalesVat.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueTaxesSalesVat.Properties.Appearance.Options.UseFont = true;
        lueTaxesSalesVat.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueTaxesSalesVat.Properties.NullText = "";
        lueTaxesSalesVat.Size = new Size(270, 22);
        lueTaxesSalesVat.TabIndex = 30;
        // 
        // lblExciseTax
        // 
        lblExciseTax.Appearance.Font = new Font("Segoe UI", 9F);
        lblExciseTax.Appearance.Options.UseFont = true;
        lblExciseTax.Location = new Point(465, 107);
        lblExciseTax.Name = "lblExciseTax";
        lblExciseTax.Size = new Size(20, 15);
        lblExciseTax.TabIndex = 31;
        lblExciseTax.Text = "ICE:";
        // 
        // lueExciseTax
        // 
        lueExciseTax.EditValue = "No aplica";
        lueExciseTax.Location = new Point(590, 103);
        lueExciseTax.Name = "lueExciseTax";
        lueExciseTax.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueExciseTax.Properties.Appearance.Options.UseFont = true;
        lueExciseTax.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueExciseTax.Properties.NullText = "";
        lueExciseTax.Size = new Size(270, 22);
        lueExciseTax.TabIndex = 32;
        // 
        // lblTaxesSuggestedWithholding
        // 
        lblTaxesSuggestedWithholding.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxesSuggestedWithholding.Appearance.Options.UseFont = true;
        lblTaxesSuggestedWithholding.Location = new Point(465, 135);
        lblTaxesSuggestedWithholding.Name = "lblTaxesSuggestedWithholding";
        lblTaxesSuggestedWithholding.Size = new Size(104, 15);
        lblTaxesSuggestedWithholding.TabIndex = 33;
        lblTaxesSuggestedWithholding.Text = "Retención sugerida:";
        // 
        // lueTaxesSuggestedWithholding
        // 
        lueTaxesSuggestedWithholding.EditValue = "1% - Bienes";
        lueTaxesSuggestedWithholding.Location = new Point(590, 131);
        lueTaxesSuggestedWithholding.Name = "lueTaxesSuggestedWithholding";
        lueTaxesSuggestedWithholding.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueTaxesSuggestedWithholding.Properties.Appearance.Options.UseFont = true;
        lueTaxesSuggestedWithholding.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueTaxesSuggestedWithholding.Properties.NullText = "";
        lueTaxesSuggestedWithholding.Size = new Size(270, 22);
        lueTaxesSuggestedWithholding.TabIndex = 34;
        // 
        // lblTaxSupport
        // 
        lblTaxSupport.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxSupport.Appearance.Options.UseFont = true;
        lblTaxSupport.Location = new Point(20, 135);
        lblTaxSupport.Name = "lblTaxSupport";
        lblTaxSupport.Size = new Size(101, 15);
        lblTaxSupport.TabIndex = 35;
        lblTaxSupport.Text = "Sustento tributario:";
        // 
        // lueTaxSupport
        // 
        lueTaxSupport.EditValue = "01 - Credito tributario para declaracion";
        lueTaxSupport.Location = new Point(155, 131);
        lueTaxSupport.Name = "lueTaxSupport";
        lueTaxSupport.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueTaxSupport.Properties.Appearance.Options.UseFont = true;
        lueTaxSupport.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueTaxSupport.Properties.NullText = "";
        lueTaxSupport.Size = new Size(260, 22);
        lueTaxSupport.TabIndex = 36;
        // 
        // lblFiscalCode
        // 
        lblFiscalCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblFiscalCode.Appearance.Options.UseFont = true;
        lblFiscalCode.Location = new Point(20, 79);
        lblFiscalCode.Name = "lblFiscalCode";
        lblFiscalCode.Size = new Size(72, 15);
        lblFiscalCode.TabIndex = 37;
        lblFiscalCode.Text = "Código fiscal:";
        // 
        // txtFiscalCode
        // 
        txtFiscalCode.EditValue = "ALIM-GRA-001";
        txtFiscalCode.Location = new Point(155, 75);
        txtFiscalCode.Name = "txtFiscalCode";
        txtFiscalCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtFiscalCode.Properties.Appearance.Options.UseFont = true;
        txtFiscalCode.Size = new Size(260, 22);
        txtFiscalCode.TabIndex = 38;
        // 
        // lblFiscalCountry
        // 
        lblFiscalCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblFiscalCountry.Appearance.Options.UseFont = true;
        lblFiscalCountry.Location = new Point(20, 107);
        lblFiscalCountry.Name = "lblFiscalCountry";
        lblFiscalCountry.Size = new Size(54, 15);
        lblFiscalCountry.TabIndex = 39;
        lblFiscalCountry.Text = "País fiscal:";
        // 
        // lueFiscalCountry
        // 
        lueFiscalCountry.EditValue = "Ecuador";
        lueFiscalCountry.Location = new Point(155, 103);
        lueFiscalCountry.Name = "lueFiscalCountry";
        lueFiscalCountry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueFiscalCountry.Properties.Appearance.Options.UseFont = true;
        lueFiscalCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFiscalCountry.Properties.NullText = "";
        lueFiscalCountry.Size = new Size(260, 22);
        lueFiscalCountry.TabIndex = 40;
        // 
        // lblTaxableGoods
        // 
        lblTaxableGoods.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxableGoods.Appearance.Options.UseFont = true;
        lblTaxableGoods.Location = new Point(915, 51);
        lblTaxableGoods.Name = "lblTaxableGoods";
        lblTaxableGoods.Size = new Size(72, 15);
        lblTaxableGoods.TabIndex = 41;
        lblTaxableGoods.Text = "Bien gravado:";
        // 
        // tglTaxableGoods
        // 
        tglTaxableGoods.EditValue = true;
        tglTaxableGoods.Location = new Point(1060, 47);
        tglTaxableGoods.Name = "tglTaxableGoods";
        tglTaxableGoods.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglTaxableGoods.Properties.Appearance.Options.UseFont = true;
        tglTaxableGoods.Properties.OffText = "No";
        tglTaxableGoods.Properties.OnText = "Si";
        tglTaxableGoods.Size = new Size(86, 20);
        tglTaxableGoods.TabIndex = 42;
        // 
        // lblTaxableService
        // 
        lblTaxableService.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxableService.Appearance.Options.UseFont = true;
        lblTaxableService.Location = new Point(915, 79);
        lblTaxableService.Name = "lblTaxableService";
        lblTaxableService.Size = new Size(90, 15);
        lblTaxableService.TabIndex = 43;
        lblTaxableService.Text = "Servicio gravado:";
        // 
        // tglTaxableService
        // 
        tglTaxableService.Location = new Point(1060, 75);
        tglTaxableService.Name = "tglTaxableService";
        tglTaxableService.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglTaxableService.Properties.Appearance.Options.UseFont = true;
        tglTaxableService.Properties.OffText = "No";
        tglTaxableService.Properties.OnText = "Si";
        tglTaxableService.Size = new Size(86, 20);
        tglTaxableService.TabIndex = 44;
        // 
        // lblTaxExemptGoods
        // 
        lblTaxExemptGoods.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxExemptGoods.Appearance.Options.UseFont = true;
        lblTaxExemptGoods.Location = new Point(915, 107);
        lblTaxExemptGoods.Name = "lblTaxExemptGoods";
        lblTaxExemptGoods.Size = new Size(64, 15);
        lblTaxExemptGoods.TabIndex = 45;
        lblTaxExemptGoods.Text = "Bien exento:";
        // 
        // tglTaxExemptGoods
        // 
        tglTaxExemptGoods.Location = new Point(1060, 103);
        tglTaxExemptGoods.Name = "tglTaxExemptGoods";
        tglTaxExemptGoods.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglTaxExemptGoods.Properties.Appearance.Options.UseFont = true;
        tglTaxExemptGoods.Properties.OffText = "No";
        tglTaxExemptGoods.Properties.OnText = "Si";
        tglTaxExemptGoods.Size = new Size(80, 20);
        tglTaxExemptGoods.TabIndex = 46;
        // 
        // lblTaxRatesTitle
        // 
        lblTaxRatesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblTaxRatesTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblTaxRatesTitle.Appearance.Options.UseFont = true;
        lblTaxRatesTitle.Appearance.Options.UseForeColor = true;
        lblTaxRatesTitle.Location = new Point(465, 12);
        lblTaxRatesTitle.Name = "lblTaxRatesTitle";
        lblTaxRatesTitle.Size = new Size(180, 20);
        lblTaxRatesTitle.TabIndex = 48;
        lblTaxRatesTitle.Text = "2. Impuestos y retenciones";
        // 
        // lblTaxApplicabilityTitle
        // 
        lblTaxApplicabilityTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblTaxApplicabilityTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblTaxApplicabilityTitle.Appearance.Options.UseFont = true;
        lblTaxApplicabilityTitle.Appearance.Options.UseForeColor = true;
        lblTaxApplicabilityTitle.Location = new Point(915, 12);
        lblTaxApplicabilityTitle.Name = "lblTaxApplicabilityTitle";
        lblTaxApplicabilityTitle.Size = new Size(105, 20);
        lblTaxApplicabilityTitle.TabIndex = 49;
        lblTaxApplicabilityTitle.Text = "3. Aplicabilidad";
        // 
        // tabAccounting
        // 
        tabAccounting.Appearance.PageClient.BackColor = Color.White;
        tabAccounting.Appearance.PageClient.Options.UseBackColor = true;
        tabAccounting.Controls.Add(sepAccountingColumnOne);
        tabAccounting.Controls.Add(sepAccountingColumnTwo);
        tabAccounting.Controls.Add(sepAccountingAccounts);
        tabAccounting.Controls.Add(sepAccountingComplementary);
        tabAccounting.Controls.Add(sepAccountingRules);
        tabAccounting.Controls.Add(lblAccountingRulesTitle);
        tabAccounting.Controls.Add(lblGenerateInventoryJournal);
        tabAccounting.Controls.Add(lblAccountingAccountsTitle);
        tabAccounting.Controls.Add(tglGenerateInventoryJournal);
        tabAccounting.Controls.Add(lblAccountingComplementaryTitle);
        tabAccounting.Controls.Add(lblUseWarehouseAccount);
        tabAccounting.Controls.Add(lblAccountingInventoryAccount);
        tabAccounting.Controls.Add(tglUseWarehouseAccount);
        tabAccounting.Controls.Add(slueInventoryAccount);
        tabAccounting.Controls.Add(lblUseGroupAccount);
        tabAccounting.Controls.Add(lblAccountingRevenueAccount);
        tabAccounting.Controls.Add(tglUseGroupAccount);
        tabAccounting.Controls.Add(slueRevenueAccount);
        tabAccounting.Controls.Add(lblAllowCompensation);
        tabAccounting.Controls.Add(lblCostOfGoodsSoldAccount);
        tabAccounting.Controls.Add(tglAllowCompensation);
        tabAccounting.Controls.Add(slueCostOfGoodsSoldAccount);
        tabAccounting.Controls.Add(lblAccountingBlocked);
        tabAccounting.Controls.Add(lblSalesReturnAccount);
        tabAccounting.Controls.Add(tglAccountingBlocked);
        tabAccounting.Controls.Add(slueSalesReturnAccount);
        tabAccounting.Controls.Add(lblReconciliationDays);
        tabAccounting.Controls.Add(lblPurchaseReturnAccount);
        tabAccounting.Controls.Add(spnReconciliationDays);
        tabAccounting.Controls.Add(sluePurchaseReturnAccount);
        tabAccounting.Controls.Add(lblAccountingIntegrationMethod);
        tabAccounting.Controls.Add(lblCostVarianceAccount);
        tabAccounting.Controls.Add(lueAccountingIntegrationMethod);
        tabAccounting.Controls.Add(slueCostVarianceAccount);
        tabAccounting.Controls.Add(lblAccountingNotes);
        tabAccounting.Controls.Add(lblInventoryAdjustmentAccount);
        tabAccounting.Controls.Add(memAccountingNotes);
        tabAccounting.Controls.Add(slueInventoryAdjustmentAccount);
        tabAccounting.Controls.Add(lblPurchaseExpenseAccount);
        tabAccounting.Controls.Add(sluePurchaseExpenseAccount);
        tabAccounting.ImageOptions.SvgImageSize = new Size(20, 20);
        tabAccounting.Name = "tabAccounting";
        tabAccounting.Size = new Size(1402, 398);
        tabAccounting.Text = "Contabilidad";
        // 
        // sepAccountingColumnOne
        // 
        sepAccountingColumnOne.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepAccountingColumnOne.Appearance.Options.UseBackColor = true;
        sepAccountingColumnOne.AutoSizeMode = LabelAutoSizeMode.None;
        sepAccountingColumnOne.Location = new Point(440, 12);
        sepAccountingColumnOne.Name = "sepAccountingColumnOne";
        sepAccountingColumnOne.Size = new Size(1, 342);
        sepAccountingColumnOne.TabIndex = 49;
        // 
        // sepAccountingColumnTwo
        // 
        sepAccountingColumnTwo.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepAccountingColumnTwo.Appearance.Options.UseBackColor = true;
        sepAccountingColumnTwo.AutoSizeMode = LabelAutoSizeMode.None;
        sepAccountingColumnTwo.Location = new Point(930, 12);
        sepAccountingColumnTwo.Name = "sepAccountingColumnTwo";
        sepAccountingColumnTwo.Size = new Size(1, 342);
        sepAccountingColumnTwo.TabIndex = 50;
        // 
        // sepAccountingAccounts
        // 
        sepAccountingAccounts.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepAccountingAccounts.Appearance.Options.UseBackColor = true;
        sepAccountingAccounts.AutoSizeMode = LabelAutoSizeMode.None;
        sepAccountingAccounts.Location = new Point(182, 22);
        sepAccountingAccounts.Name = "sepAccountingAccounts";
        sepAccountingAccounts.Size = new Size(238, 1);
        sepAccountingAccounts.TabIndex = 51;
        // 
        // sepAccountingComplementary
        // 
        sepAccountingComplementary.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepAccountingComplementary.Appearance.Options.UseBackColor = true;
        sepAccountingComplementary.AutoSizeMode = LabelAutoSizeMode.None;
        sepAccountingComplementary.Location = new Point(672, 22);
        sepAccountingComplementary.Name = "sepAccountingComplementary";
        sepAccountingComplementary.Size = new Size(238, 1);
        sepAccountingComplementary.TabIndex = 52;
        // 
        // sepAccountingRules
        // 
        sepAccountingRules.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        sepAccountingRules.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepAccountingRules.Appearance.Options.UseBackColor = true;
        sepAccountingRules.AutoSizeMode = LabelAutoSizeMode.None;
        sepAccountingRules.Location = new Point(1100, 22);
        sepAccountingRules.Name = "sepAccountingRules";
        sepAccountingRules.Size = new Size(284, 1);
        sepAccountingRules.TabIndex = 53;
        // 
        // lblAccountingRulesTitle
        // 
        lblAccountingRulesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAccountingRulesTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblAccountingRulesTitle.Appearance.Options.UseFont = true;
        lblAccountingRulesTitle.Appearance.Options.UseForeColor = true;
        lblAccountingRulesTitle.Location = new Point(955, 12);
        lblAccountingRulesTitle.Name = "lblAccountingRulesTitle";
        lblAccountingRulesTitle.Size = new Size(129, 20);
        lblAccountingRulesTitle.TabIndex = 0;
        lblAccountingRulesTitle.Text = "3. Reglas contables";
        // 
        // lblGenerateInventoryJournal
        // 
        lblGenerateInventoryJournal.Appearance.Font = new Font("Segoe UI", 9F);
        lblGenerateInventoryJournal.Appearance.Options.UseFont = true;
        lblGenerateInventoryJournal.Location = new Point(955, 47);
        lblGenerateInventoryJournal.Name = "lblGenerateInventoryJournal";
        lblGenerateInventoryJournal.Size = new Size(137, 15);
        lblGenerateInventoryJournal.TabIndex = 1;
        lblGenerateInventoryJournal.Text = "Genera asiento inventario:";
        // 
        // lblAccountingAccountsTitle
        // 
        lblAccountingAccountsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAccountingAccountsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblAccountingAccountsTitle.Appearance.Options.UseFont = true;
        lblAccountingAccountsTitle.Appearance.Options.UseForeColor = true;
        lblAccountingAccountsTitle.Location = new Point(20, 12);
        lblAccountingAccountsTitle.Name = "lblAccountingAccountsTitle";
        lblAccountingAccountsTitle.Size = new Size(146, 20);
        lblAccountingAccountsTitle.TabIndex = 18;
        lblAccountingAccountsTitle.Text = "1. Cuentas principales";
        // 
        // tglGenerateInventoryJournal
        // 
        tglGenerateInventoryJournal.EditValue = true;
        tglGenerateInventoryJournal.Location = new Point(1281, 43);
        tglGenerateInventoryJournal.Name = "tglGenerateInventoryJournal";
        tglGenerateInventoryJournal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGenerateInventoryJournal.Properties.Appearance.Options.UseFont = true;
        tglGenerateInventoryJournal.Properties.OffText = "No";
        tglGenerateInventoryJournal.Properties.OnText = "Si";
        tglGenerateInventoryJournal.Size = new Size(86, 20);
        tglGenerateInventoryJournal.TabIndex = 2;
        // 
        // lblAccountingComplementaryTitle
        // 
        lblAccountingComplementaryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAccountingComplementaryTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblAccountingComplementaryTitle.Appearance.Options.UseFont = true;
        lblAccountingComplementaryTitle.Appearance.Options.UseForeColor = true;
        lblAccountingComplementaryTitle.Location = new Point(465, 12);
        lblAccountingComplementaryTitle.Name = "lblAccountingComplementaryTitle";
        lblAccountingComplementaryTitle.Size = new Size(191, 20);
        lblAccountingComplementaryTitle.TabIndex = 48;
        lblAccountingComplementaryTitle.Text = "2. Cuentas complementarias";
        // 
        // lblUseWarehouseAccount
        // 
        lblUseWarehouseAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblUseWarehouseAccount.Appearance.Options.UseFont = true;
        lblUseWarehouseAccount.Location = new Point(955, 75);
        lblUseWarehouseAccount.Name = "lblUseWarehouseAccount";
        lblUseWarehouseAccount.Size = new Size(125, 15);
        lblUseWarehouseAccount.TabIndex = 3;
        lblUseWarehouseAccount.Text = "Usa cuenta por bodega:";
        // 
        // lblAccountingInventoryAccount
        // 
        lblAccountingInventoryAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingInventoryAccount.Appearance.Options.UseFont = true;
        lblAccountingInventoryAccount.Location = new Point(20, 51);
        lblAccountingInventoryAccount.Name = "lblAccountingInventoryAccount";
        lblAccountingInventoryAccount.Size = new Size(97, 15);
        lblAccountingInventoryAccount.TabIndex = 19;
        lblAccountingInventoryAccount.Text = "Cuenta inventario:";
        // 
        // tglUseWarehouseAccount
        // 
        tglUseWarehouseAccount.EditValue = true;
        tglUseWarehouseAccount.Location = new Point(1281, 71);
        tglUseWarehouseAccount.Name = "tglUseWarehouseAccount";
        tglUseWarehouseAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglUseWarehouseAccount.Properties.Appearance.Options.UseFont = true;
        tglUseWarehouseAccount.Properties.OffText = "No";
        tglUseWarehouseAccount.Properties.OnText = "Si";
        tglUseWarehouseAccount.Size = new Size(86, 20);
        tglUseWarehouseAccount.TabIndex = 4;
        // 
        // slueInventoryAccount
        // 
        slueInventoryAccount.Location = new Point(175, 47);
        slueInventoryAccount.Name = "slueInventoryAccount";
        slueInventoryAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueInventoryAccount.Properties.Appearance.Options.UseFont = true;
        slueInventoryAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueInventoryAccount.Properties.NullText = "1205-01-01 Inventario de mercaderias";
        slueInventoryAccount.Properties.PopupView = gvInventoryAccount;
        slueInventoryAccount.Size = new Size(265, 22);
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
        // lblUseGroupAccount
        // 
        lblUseGroupAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblUseGroupAccount.Appearance.Options.UseFont = true;
        lblUseGroupAccount.Location = new Point(955, 103);
        lblUseGroupAccount.Name = "lblUseGroupAccount";
        lblUseGroupAccount.Size = new Size(117, 15);
        lblUseGroupAccount.TabIndex = 5;
        lblUseGroupAccount.Text = "Usa cuenta por grupo:";
        // 
        // lblAccountingRevenueAccount
        // 
        lblAccountingRevenueAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingRevenueAccount.Appearance.Options.UseFont = true;
        lblAccountingRevenueAccount.Location = new Point(20, 79);
        lblAccountingRevenueAccount.Name = "lblAccountingRevenueAccount";
        lblAccountingRevenueAccount.Size = new Size(88, 15);
        lblAccountingRevenueAccount.TabIndex = 21;
        lblAccountingRevenueAccount.Text = "Cuenta ingresos:";
        // 
        // tglUseGroupAccount
        // 
        tglUseGroupAccount.Location = new Point(1281, 99);
        tglUseGroupAccount.Name = "tglUseGroupAccount";
        tglUseGroupAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglUseGroupAccount.Properties.Appearance.Options.UseFont = true;
        tglUseGroupAccount.Properties.OffText = "No";
        tglUseGroupAccount.Properties.OnText = "Si";
        tglUseGroupAccount.Size = new Size(86, 20);
        tglUseGroupAccount.TabIndex = 6;
        // 
        // slueRevenueAccount
        // 
        slueRevenueAccount.Location = new Point(175, 75);
        slueRevenueAccount.Name = "slueRevenueAccount";
        slueRevenueAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueRevenueAccount.Properties.Appearance.Options.UseFont = true;
        slueRevenueAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueRevenueAccount.Properties.NullText = "4105-01-01 Ventas de mercaderias";
        slueRevenueAccount.Properties.PopupView = gvRevenueAccount;
        slueRevenueAccount.Size = new Size(265, 22);
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
        // lblAllowCompensation
        // 
        lblAllowCompensation.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowCompensation.Appearance.Options.UseFont = true;
        lblAllowCompensation.Location = new Point(955, 131);
        lblAllowCompensation.Name = "lblAllowCompensation";
        lblAllowCompensation.Size = new Size(125, 15);
        lblAllowCompensation.TabIndex = 7;
        lblAllowCompensation.Text = "Permite compensacion:";
        // 
        // lblCostOfGoodsSoldAccount
        // 
        lblCostOfGoodsSoldAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostOfGoodsSoldAccount.Appearance.Options.UseFont = true;
        lblCostOfGoodsSoldAccount.Location = new Point(20, 107);
        lblCostOfGoodsSoldAccount.Name = "lblCostOfGoodsSoldAccount";
        lblCostOfGoodsSoldAccount.Size = new Size(121, 15);
        lblCostOfGoodsSoldAccount.TabIndex = 23;
        lblCostOfGoodsSoldAccount.Text = "Cuenta costo de venta:";
        // 
        // tglAllowCompensation
        // 
        tglAllowCompensation.EditValue = true;
        tglAllowCompensation.Location = new Point(1281, 127);
        tglAllowCompensation.Name = "tglAllowCompensation";
        tglAllowCompensation.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAllowCompensation.Properties.Appearance.Options.UseFont = true;
        tglAllowCompensation.Properties.OffText = "No";
        tglAllowCompensation.Properties.OnText = "Si";
        tglAllowCompensation.Size = new Size(86, 20);
        tglAllowCompensation.TabIndex = 8;
        // 
        // slueCostOfGoodsSoldAccount
        // 
        slueCostOfGoodsSoldAccount.Location = new Point(175, 103);
        slueCostOfGoodsSoldAccount.Name = "slueCostOfGoodsSoldAccount";
        slueCostOfGoodsSoldAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueCostOfGoodsSoldAccount.Properties.Appearance.Options.UseFont = true;
        slueCostOfGoodsSoldAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueCostOfGoodsSoldAccount.Properties.NullText = "5105-01-01 Costo de ventas";
        slueCostOfGoodsSoldAccount.Properties.PopupView = gvCostOfGoodsSoldAccount;
        slueCostOfGoodsSoldAccount.Size = new Size(265, 22);
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
        // lblAccountingBlocked
        // 
        lblAccountingBlocked.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingBlocked.Appearance.Options.UseFont = true;
        lblAccountingBlocked.Location = new Point(955, 159);
        lblAccountingBlocked.Name = "lblAccountingBlocked";
        lblAccountingBlocked.Size = new Size(109, 15);
        lblAccountingBlocked.TabIndex = 9;
        lblAccountingBlocked.Text = "Bloqueado contable:";
        // 
        // lblSalesReturnAccount
        // 
        lblSalesReturnAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesReturnAccount.Appearance.Options.UseFont = true;
        lblSalesReturnAccount.Location = new Point(20, 135);
        lblSalesReturnAccount.Name = "lblSalesReturnAccount";
        lblSalesReturnAccount.Size = new Size(135, 15);
        lblSalesReturnAccount.TabIndex = 25;
        lblSalesReturnAccount.Text = "Cuenta devolución venta:";
        // 
        // tglAccountingBlocked
        // 
        tglAccountingBlocked.Location = new Point(1281, 155);
        tglAccountingBlocked.Name = "tglAccountingBlocked";
        tglAccountingBlocked.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAccountingBlocked.Properties.Appearance.Options.UseFont = true;
        tglAccountingBlocked.Properties.OffText = "No";
        tglAccountingBlocked.Properties.OnText = "Si";
        tglAccountingBlocked.Size = new Size(86, 20);
        tglAccountingBlocked.TabIndex = 10;
        // 
        // slueSalesReturnAccount
        // 
        slueSalesReturnAccount.Location = new Point(175, 131);
        slueSalesReturnAccount.Name = "slueSalesReturnAccount";
        slueSalesReturnAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueSalesReturnAccount.Properties.Appearance.Options.UseFont = true;
        slueSalesReturnAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueSalesReturnAccount.Properties.NullText = "4105-02-01 Devoluciones en ventas";
        slueSalesReturnAccount.Properties.PopupView = gvSalesReturnAccount;
        slueSalesReturnAccount.Size = new Size(265, 22);
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
        // lblReconciliationDays
        // 
        lblReconciliationDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblReconciliationDays.Appearance.Options.UseFont = true;
        lblReconciliationDays.Location = new Point(955, 187);
        lblReconciliationDays.Name = "lblReconciliationDays";
        lblReconciliationDays.Size = new Size(108, 15);
        lblReconciliationDays.TabIndex = 11;
        lblReconciliationDays.Text = "Días de conciliación:";
        // 
        // lblPurchaseReturnAccount
        // 
        lblPurchaseReturnAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseReturnAccount.Appearance.Options.UseFont = true;
        lblPurchaseReturnAccount.Location = new Point(465, 51);
        lblPurchaseReturnAccount.Name = "lblPurchaseReturnAccount";
        lblPurchaseReturnAccount.Size = new Size(147, 15);
        lblPurchaseReturnAccount.TabIndex = 27;
        lblPurchaseReturnAccount.Text = "Cuenta devolución compra:";
        // 
        // spnReconciliationDays
        // 
        spnReconciliationDays.EditValue = new decimal(new int[] { 30, 0, 0, 0 });
        spnReconciliationDays.Location = new Point(1281, 183);
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
        // sluePurchaseReturnAccount
        // 
        sluePurchaseReturnAccount.Location = new Point(635, 47);
        sluePurchaseReturnAccount.Name = "sluePurchaseReturnAccount";
        sluePurchaseReturnAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sluePurchaseReturnAccount.Properties.Appearance.Options.UseFont = true;
        sluePurchaseReturnAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        sluePurchaseReturnAccount.Properties.NullText = "2105-02-01 Devoluciones en compras";
        sluePurchaseReturnAccount.Properties.PopupView = gvPurchaseReturnAccount;
        sluePurchaseReturnAccount.Size = new Size(265, 22);
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
        // lblAccountingIntegrationMethod
        // 
        lblAccountingIntegrationMethod.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingIntegrationMethod.Appearance.Options.UseFont = true;
        lblAccountingIntegrationMethod.Location = new Point(955, 215);
        lblAccountingIntegrationMethod.Name = "lblAccountingIntegrationMethod";
        lblAccountingIntegrationMethod.Size = new Size(173, 15);
        lblAccountingIntegrationMethod.TabIndex = 13;
        lblAccountingIntegrationMethod.Text = "Método de integración contable:";
        // 
        // lblCostVarianceAccount
        // 
        lblCostVarianceAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostVarianceAccount.Appearance.Options.UseFont = true;
        lblCostVarianceAccount.Location = new Point(465, 79);
        lblCostVarianceAccount.Name = "lblCostVarianceAccount";
        lblCostVarianceAccount.Size = new Size(124, 15);
        lblCostVarianceAccount.TabIndex = 29;
        lblCostVarianceAccount.Text = "Cuenta variación costo:";
        // 
        // lueAccountingIntegrationMethod
        // 
        lueAccountingIntegrationMethod.EditValue = "En tiempo real";
        lueAccountingIntegrationMethod.Location = new Point(1157, 211);
        lueAccountingIntegrationMethod.Name = "lueAccountingIntegrationMethod";
        lueAccountingIntegrationMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingIntegrationMethod.Properties.Appearance.Options.UseFont = true;
        lueAccountingIntegrationMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingIntegrationMethod.Properties.NullText = "";
        lueAccountingIntegrationMethod.Size = new Size(211, 22);
        lueAccountingIntegrationMethod.TabIndex = 14;
        // 
        // slueCostVarianceAccount
        // 
        slueCostVarianceAccount.Location = new Point(635, 75);
        slueCostVarianceAccount.Name = "slueCostVarianceAccount";
        slueCostVarianceAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueCostVarianceAccount.Properties.Appearance.Options.UseFont = true;
        slueCostVarianceAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueCostVarianceAccount.Properties.NullText = "5105-03-01 Variacion de inventario";
        slueCostVarianceAccount.Properties.PopupView = gvCostVarianceAccount;
        slueCostVarianceAccount.Size = new Size(265, 22);
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
        // lblAccountingNotes
        // 
        lblAccountingNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingNotes.Appearance.Options.UseFont = true;
        lblAccountingNotes.Location = new Point(955, 247);
        lblAccountingNotes.Name = "lblAccountingNotes";
        lblAccountingNotes.Size = new Size(134, 15);
        lblAccountingNotes.TabIndex = 15;
        lblAccountingNotes.Text = "Observaciones contables:";
        // 
        // lblInventoryAdjustmentAccount
        // 
        lblInventoryAdjustmentAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblInventoryAdjustmentAccount.Appearance.Options.UseFont = true;
        lblInventoryAdjustmentAccount.Location = new Point(465, 107);
        lblInventoryAdjustmentAccount.Name = "lblInventoryAdjustmentAccount";
        lblInventoryAdjustmentAccount.Size = new Size(131, 15);
        lblInventoryAdjustmentAccount.TabIndex = 31;
        lblInventoryAdjustmentAccount.Text = "Cuenta ajuste inventario:";
        // 
        // memAccountingNotes
        // 
        memAccountingNotes.EditValue = "Item de alta rotacion.\r\nSe utiliza metodo promedio ponderado para valoracion de inventario.";
        memAccountingNotes.Location = new Point(1157, 243);
        memAccountingNotes.Name = "memAccountingNotes";
        memAccountingNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memAccountingNotes.Properties.Appearance.Options.UseFont = true;
        memAccountingNotes.Size = new Size(211, 50);
        memAccountingNotes.TabIndex = 16;
        // 
        // slueInventoryAdjustmentAccount
        // 
        slueInventoryAdjustmentAccount.Location = new Point(635, 103);
        slueInventoryAdjustmentAccount.Name = "slueInventoryAdjustmentAccount";
        slueInventoryAdjustmentAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueInventoryAdjustmentAccount.Properties.Appearance.Options.UseFont = true;
        slueInventoryAdjustmentAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueInventoryAdjustmentAccount.Properties.NullText = "1205-04-01 Ajuste de inventario";
        slueInventoryAdjustmentAccount.Properties.PopupView = gvInventoryAdjustmentAccount;
        slueInventoryAdjustmentAccount.Size = new Size(265, 22);
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
        // lblPurchaseExpenseAccount
        // 
        lblPurchaseExpenseAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseExpenseAccount.Appearance.Options.UseFont = true;
        lblPurchaseExpenseAccount.Location = new Point(465, 135);
        lblPurchaseExpenseAccount.Name = "lblPurchaseExpenseAccount";
        lblPurchaseExpenseAccount.Size = new Size(117, 15);
        lblPurchaseExpenseAccount.TabIndex = 33;
        lblPurchaseExpenseAccount.Text = "Cuenta gasto compra:";
        // 
        // sluePurchaseExpenseAccount
        // 
        sluePurchaseExpenseAccount.Location = new Point(635, 131);
        sluePurchaseExpenseAccount.Name = "sluePurchaseExpenseAccount";
        sluePurchaseExpenseAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sluePurchaseExpenseAccount.Properties.Appearance.Options.UseFont = true;
        sluePurchaseExpenseAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        sluePurchaseExpenseAccount.Properties.NullText = "5105-04-01 Gastos de compra";
        sluePurchaseExpenseAccount.Properties.PopupView = gvPurchaseExpenseAccount;
        sluePurchaseExpenseAccount.Size = new Size(265, 22);
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
        // tabCosts
        // 
        tabCosts.Appearance.PageClient.BackColor = Color.White;
        tabCosts.Appearance.PageClient.Options.UseBackColor = true;
        tabCosts.Controls.Add(sepCostsColumnOne);
        tabCosts.Controls.Add(sepCostsColumnTwo);
        tabCosts.Controls.Add(sepCostsBase);
        tabCosts.Controls.Add(sepCostsPrices);
        tabCosts.Controls.Add(sepCostsIndicators);
        tabCosts.Controls.Add(sepCostsHistory);
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
        tabCosts.Controls.Add(spnLastCost);
        tabCosts.Controls.Add(lblCostsAverageCost);
        tabCosts.Controls.Add(spnAverageCost);
        tabCosts.Controls.Add(lblPriceUpdatedAt);
        tabCosts.Controls.Add(lblCostUpdatedAt);
        tabCosts.Controls.Add(dtPriceUpdatedAt);
        tabCosts.Controls.Add(dtCostUpdatedAt);
        tabCosts.Controls.Add(lblManualCostUpdate);
        tabCosts.Controls.Add(tglManualCostUpdate);
        tabCosts.Controls.Add(lblFinanceCostIndicatorsTitle);
        tabCosts.Controls.Add(kpiFinanceGrossMargin);
        tabCosts.Controls.Add(kpiFinanceGrossMarginPercent);
        tabCosts.Controls.Add(kpiFinanceProfitability);
        tabCosts.Controls.Add(kpiFinanceSuggestedPrice);
        tabCosts.ImageOptions.SvgImageSize = new Size(20, 20);
        tabCosts.Name = "tabCosts";
        tabCosts.Size = new Size(1402, 398);
        tabCosts.Text = "Costos y precios";
        // 
        // sepCostsColumnOne
        // 
        sepCostsColumnOne.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepCostsColumnOne.Appearance.Options.UseBackColor = true;
        sepCostsColumnOne.AutoSizeMode = LabelAutoSizeMode.None;
        sepCostsColumnOne.Location = new Point(520, 12);
        sepCostsColumnOne.Name = "sepCostsColumnOne";
        sepCostsColumnOne.Size = new Size(1, 166);
        sepCostsColumnOne.TabIndex = 43;
        // 
        // sepCostsColumnTwo
        // 
        sepCostsColumnTwo.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepCostsColumnTwo.Appearance.Options.UseBackColor = true;
        sepCostsColumnTwo.AutoSizeMode = LabelAutoSizeMode.None;
        sepCostsColumnTwo.Location = new Point(910, 12);
        sepCostsColumnTwo.Name = "sepCostsColumnTwo";
        sepCostsColumnTwo.Size = new Size(1, 166);
        sepCostsColumnTwo.TabIndex = 44;
        // 
        // sepCostsBase
        // 
        sepCostsBase.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepCostsBase.Appearance.Options.UseBackColor = true;
        sepCostsBase.AutoSizeMode = LabelAutoSizeMode.None;
        sepCostsBase.Location = new Point(210, 22);
        sepCostsBase.Name = "sepCostsBase";
        sepCostsBase.Size = new Size(290, 1);
        sepCostsBase.TabIndex = 45;
        // 
        // sepCostsPrices
        // 
        sepCostsPrices.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepCostsPrices.Appearance.Options.UseBackColor = true;
        sepCostsPrices.AutoSizeMode = LabelAutoSizeMode.None;
        sepCostsPrices.Location = new Point(713, 22);
        sepCostsPrices.Name = "sepCostsPrices";
        sepCostsPrices.Size = new Size(177, 1);
        sepCostsPrices.TabIndex = 46;
        // 
        // sepCostsIndicators
        // 
        sepCostsIndicators.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        sepCostsIndicators.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepCostsIndicators.Appearance.Options.UseBackColor = true;
        sepCostsIndicators.AutoSizeMode = LabelAutoSizeMode.None;
        sepCostsIndicators.Location = new Point(1123, 22);
        sepCostsIndicators.Name = "sepCostsIndicators";
        sepCostsIndicators.Size = new Size(261, 1);
        sepCostsIndicators.TabIndex = 47;
        // 
        // sepCostsHistory
        // 
        sepCostsHistory.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        sepCostsHistory.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepCostsHistory.Appearance.Options.UseBackColor = true;
        sepCostsHistory.AutoSizeMode = LabelAutoSizeMode.None;
        sepCostsHistory.Location = new Point(242, 208);
        sepCostsHistory.Name = "sepCostsHistory";
        sepCostsHistory.Size = new Size(1142, 1);
        sepCostsHistory.TabIndex = 48;
        // 
        // lblCostPriceHistoryTitle
        // 
        lblCostPriceHistoryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblCostPriceHistoryTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblCostPriceHistoryTitle.Appearance.Options.UseFont = true;
        lblCostPriceHistoryTitle.Appearance.Options.UseForeColor = true;
        lblCostPriceHistoryTitle.Location = new Point(20, 198);
        lblCostPriceHistoryTitle.Name = "lblCostPriceHistoryTitle";
        lblCostPriceHistoryTitle.Size = new Size(206, 20);
        lblCostPriceHistoryTitle.TabIndex = 0;
        lblCostPriceHistoryTitle.Text = "4. Historial de costos y precios";
        // 
        // grdCostPriceHistory
        // 
        grdCostPriceHistory.DataSource = costPriceHistoryTable;
        grdCostPriceHistory.Location = new Point(20, 224);
        grdCostPriceHistory.MainView = gvCostPriceHistory;
        grdCostPriceHistory.Name = "grdCostPriceHistory";
        grdCostPriceHistory.Size = new Size(1364, 146);
        grdCostPriceHistory.TabIndex = 1;
        grdCostPriceHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvCostPriceHistory, gvCostPriceHistoryAux });
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
        // gvCostPriceHistoryAux
        // 
        gvCostPriceHistoryAux.GridControl = grdCostPriceHistory;
        gvCostPriceHistoryAux.Name = "gvCostPriceHistoryAux";
        // 
        // lblPricesMarginsTitle
        // 
        lblPricesMarginsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPricesMarginsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblPricesMarginsTitle.Appearance.Options.UseFont = true;
        lblPricesMarginsTitle.Appearance.Options.UseForeColor = true;
        lblPricesMarginsTitle.Location = new Point(549, 12);
        lblPricesMarginsTitle.Name = "lblPricesMarginsTitle";
        lblPricesMarginsTitle.Size = new Size(148, 20);
        lblPricesMarginsTitle.TabIndex = 0;
        lblPricesMarginsTitle.Text = "2. Precios y márgenes";
        // 
        // lblAnalysisBasePrice
        // 
        lblAnalysisBasePrice.Appearance.Font = new Font("Segoe UI", 9F);
        lblAnalysisBasePrice.Appearance.Options.UseFont = true;
        lblAnalysisBasePrice.Location = new Point(549, 51);
        lblAnalysisBasePrice.Name = "lblAnalysisBasePrice";
        lblAnalysisBasePrice.Size = new Size(63, 15);
        lblAnalysisBasePrice.TabIndex = 1;
        lblAnalysisBasePrice.Text = "Precio base:";
        // 
        // lblCostsBaseTitle
        // 
        lblCostsBaseTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblCostsBaseTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblCostsBaseTitle.Appearance.Options.UseFont = true;
        lblCostsBaseTitle.Appearance.Options.UseForeColor = true;
        lblCostsBaseTitle.Location = new Point(20, 12);
        lblCostsBaseTitle.Name = "lblCostsBaseTitle";
        lblCostsBaseTitle.Size = new Size(174, 20);
        lblCostsBaseTitle.TabIndex = 19;
        lblCostsBaseTitle.Text = "1. Costos base del artículo";
        // 
        // spnAnalysisBasePrice
        // 
        spnAnalysisBasePrice.EditValue = new decimal(new int[] { 2850, 0, 0, 131072 });
        spnAnalysisBasePrice.Location = new Point(679, 47);
        spnAnalysisBasePrice.Name = "spnAnalysisBasePrice";
        spnAnalysisBasePrice.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnAnalysisBasePrice.Properties.Appearance.Options.UseFont = true;
        spnAnalysisBasePrice.Properties.Appearance.Options.UseTextOptions = true;
        spnAnalysisBasePrice.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnAnalysisBasePrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnAnalysisBasePrice.Properties.MaskSettings.Set("mask", "n2");
        spnAnalysisBasePrice.Size = new Size(150, 22);
        spnAnalysisBasePrice.TabIndex = 2;
        // 
        // lblCostCurrency
        // 
        lblCostCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostCurrency.Appearance.Options.UseFont = true;
        lblCostCurrency.Location = new Point(20, 51);
        lblCostCurrency.Name = "lblCostCurrency";
        lblCostCurrency.Size = new Size(79, 15);
        lblCostCurrency.TabIndex = 20;
        lblCostCurrency.Text = "Moneda costo:";
        // 
        // lblSuggestedPrice
        // 
        lblSuggestedPrice.Appearance.Font = new Font("Segoe UI", 9F);
        lblSuggestedPrice.Appearance.Options.UseFont = true;
        lblSuggestedPrice.Location = new Point(549, 79);
        lblSuggestedPrice.Name = "lblSuggestedPrice";
        lblSuggestedPrice.Size = new Size(85, 15);
        lblSuggestedPrice.TabIndex = 3;
        lblSuggestedPrice.Text = "Precio sugerido:";
        // 
        // lueCostCurrency
        // 
        lueCostCurrency.Location = new Point(160, 47);
        lueCostCurrency.Name = "lueCostCurrency";
        lueCostCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCostCurrency.Properties.Appearance.Options.UseFont = true;
        lueCostCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCostCurrency.Properties.NullText = "USD - Dólar estadounidense";
        lueCostCurrency.Size = new Size(260, 22);
        lueCostCurrency.TabIndex = 21;
        // 
        // spnSuggestedPrice
        // 
        spnSuggestedPrice.EditValue = new decimal(new int[] { 3290, 0, 0, 131072 });
        spnSuggestedPrice.Location = new Point(679, 75);
        spnSuggestedPrice.Name = "spnSuggestedPrice";
        spnSuggestedPrice.Properties.Appearance.BackColor = Color.FromArgb((int)(byte)245, (int)(byte)247, (int)(byte)250);
        spnSuggestedPrice.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSuggestedPrice.Properties.Appearance.Options.UseBackColor = true;
        spnSuggestedPrice.Properties.Appearance.Options.UseFont = true;
        spnSuggestedPrice.Properties.Appearance.Options.UseTextOptions = true;
        spnSuggestedPrice.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSuggestedPrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSuggestedPrice.Properties.MaskSettings.Set("mask", "n2");
        spnSuggestedPrice.Properties.ReadOnly = true;
        spnSuggestedPrice.Size = new Size(150, 22);
        spnSuggestedPrice.TabIndex = 4;
        // 
        // lblStandardCost
        // 
        lblStandardCost.Appearance.Font = new Font("Segoe UI", 9F);
        lblStandardCost.Appearance.Options.UseFont = true;
        lblStandardCost.Location = new Point(20, 79);
        lblStandardCost.Name = "lblStandardCost";
        lblStandardCost.Size = new Size(82, 15);
        lblStandardCost.TabIndex = 22;
        lblStandardCost.Text = "Costo estándar:";
        // 
        // lblMinimumMarginPercent
        // 
        lblMinimumMarginPercent.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumMarginPercent.Appearance.Options.UseFont = true;
        lblMinimumMarginPercent.Location = new Point(549, 107);
        lblMinimumMarginPercent.Name = "lblMinimumMarginPercent";
        lblMinimumMarginPercent.Size = new Size(102, 15);
        lblMinimumMarginPercent.TabIndex = 5;
        lblMinimumMarginPercent.Text = "Margen mínimo %:";
        // 
        // spnStandardCost
        // 
        spnStandardCost.EditValue = new decimal(new int[] { 1825, 0, 0, 131072 });
        spnStandardCost.Location = new Point(160, 75);
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
        // spnMinimumMarginPercent
        // 
        spnMinimumMarginPercent.EditValue = new decimal(new int[] { 2500, 0, 0, 131072 });
        spnMinimumMarginPercent.Location = new Point(679, 103);
        spnMinimumMarginPercent.Name = "spnMinimumMarginPercent";
        spnMinimumMarginPercent.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMinimumMarginPercent.Properties.Appearance.Options.UseFont = true;
        spnMinimumMarginPercent.Properties.Appearance.Options.UseTextOptions = true;
        spnMinimumMarginPercent.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnMinimumMarginPercent.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMinimumMarginPercent.Properties.MaskSettings.Set("mask", "n2");
        spnMinimumMarginPercent.Size = new Size(150, 22);
        spnMinimumMarginPercent.TabIndex = 6;
        // 
        // lblReplacementCost
        // 
        lblReplacementCost.Appearance.Font = new Font("Segoe UI", 9F);
        lblReplacementCost.Appearance.Options.UseFont = true;
        lblReplacementCost.Location = new Point(20, 107);
        lblReplacementCost.Name = "lblReplacementCost";
        lblReplacementCost.Size = new Size(92, 15);
        lblReplacementCost.TabIndex = 24;
        lblReplacementCost.Text = "Costo reposición:";
        // 
        // lblTargetMarginPercent
        // 
        lblTargetMarginPercent.Appearance.Font = new Font("Segoe UI", 9F);
        lblTargetMarginPercent.Appearance.Options.UseFont = true;
        lblTargetMarginPercent.Location = new Point(549, 135);
        lblTargetMarginPercent.Name = "lblTargetMarginPercent";
        lblTargetMarginPercent.Size = new Size(103, 15);
        lblTargetMarginPercent.TabIndex = 7;
        lblTargetMarginPercent.Text = "Margen objetivo %:";
        // 
        // spnReplacementCost
        // 
        spnReplacementCost.EditValue = new decimal(new int[] { 1920, 0, 0, 131072 });
        spnReplacementCost.Location = new Point(160, 103);
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
        // spnTargetMarginPercent
        // 
        spnTargetMarginPercent.EditValue = new decimal(new int[] { 3000, 0, 0, 131072 });
        spnTargetMarginPercent.Location = new Point(679, 131);
        spnTargetMarginPercent.Name = "spnTargetMarginPercent";
        spnTargetMarginPercent.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnTargetMarginPercent.Properties.Appearance.Options.UseFont = true;
        spnTargetMarginPercent.Properties.Appearance.Options.UseTextOptions = true;
        spnTargetMarginPercent.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnTargetMarginPercent.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnTargetMarginPercent.Properties.MaskSettings.Set("mask", "n2");
        spnTargetMarginPercent.Size = new Size(150, 22);
        spnTargetMarginPercent.TabIndex = 8;
        // 
        // lblLastCost
        // 
        lblLastCost.Appearance.Font = new Font("Segoe UI", 9F);
        lblLastCost.Appearance.Options.UseFont = true;
        lblLastCost.Location = new Point(20, 135);
        lblLastCost.Name = "lblLastCost";
        lblLastCost.Size = new Size(72, 15);
        lblLastCost.TabIndex = 26;
        lblLastCost.Text = "Costo último:";
        // 
        // spnLastCost
        // 
        spnLastCost.EditValue = new decimal(new int[] { 1840, 0, 0, 131072 });
        spnLastCost.Location = new Point(160, 131);
        spnLastCost.Name = "spnLastCost";
        spnLastCost.Properties.Appearance.BackColor = Color.FromArgb((int)(byte)245, (int)(byte)247, (int)(byte)250);
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
        // lblCostsAverageCost
        // 
        lblCostsAverageCost.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostsAverageCost.Appearance.Options.UseFont = true;
        lblCostsAverageCost.Location = new Point(20, 163);
        lblCostsAverageCost.Name = "lblCostsAverageCost";
        lblCostsAverageCost.Size = new Size(89, 15);
        lblCostsAverageCost.TabIndex = 28;
        lblCostsAverageCost.Text = "Costo promedio:";
        // 
        // spnAverageCost
        // 
        spnAverageCost.EditValue = new decimal(new int[] { 1865, 0, 0, 131072 });
        spnAverageCost.Location = new Point(160, 159);
        spnAverageCost.Name = "spnAverageCost";
        spnAverageCost.Properties.Appearance.BackColor = Color.FromArgb((int)(byte)245, (int)(byte)247, (int)(byte)250);
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
        // lblPriceUpdatedAt
        // 
        lblPriceUpdatedAt.Appearance.Font = new Font("Segoe UI", 9F);
        lblPriceUpdatedAt.Appearance.Options.UseFont = true;
        lblPriceUpdatedAt.Location = new Point(549, 163);
        lblPriceUpdatedAt.Name = "lblPriceUpdatedAt";
        lblPriceUpdatedAt.Size = new Size(74, 15);
        lblPriceUpdatedAt.TabIndex = 12;
        lblPriceUpdatedAt.Text = "Actualización:";
        // 
        // lblCostUpdatedAt
        // 
        lblCostUpdatedAt.Appearance.Font = new Font("Segoe UI", 9F);
        lblCostUpdatedAt.Appearance.Options.UseFont = true;
        lblCostUpdatedAt.Location = new Point(294, 79);
        lblCostUpdatedAt.Name = "lblCostUpdatedAt";
        lblCostUpdatedAt.Size = new Size(74, 15);
        lblCostUpdatedAt.TabIndex = 34;
        lblCostUpdatedAt.Text = "Actualización:";
        // 
        // dtPriceUpdatedAt
        // 
        dtPriceUpdatedAt.EditValue = new DateTime(2026, 5, 15, 9, 10, 0, 0);
        dtPriceUpdatedAt.Location = new Point(679, 159);
        dtPriceUpdatedAt.Name = "dtPriceUpdatedAt";
        dtPriceUpdatedAt.Properties.Appearance.BackColor = Color.FromArgb((int)(byte)245, (int)(byte)247, (int)(byte)250);
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
        dtPriceUpdatedAt.Size = new Size(150, 22);
        dtPriceUpdatedAt.TabIndex = 13;
        // 
        // dtCostUpdatedAt
        // 
        dtCostUpdatedAt.EditValue = new DateTime(2026, 5, 15, 8, 30, 0, 0);
        dtCostUpdatedAt.Location = new Point(294, 103);
        dtCostUpdatedAt.Name = "dtCostUpdatedAt";
        dtCostUpdatedAt.Properties.Appearance.BackColor = Color.FromArgb((int)(byte)245, (int)(byte)247, (int)(byte)250);
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
        dtCostUpdatedAt.Size = new Size(126, 22);
        dtCostUpdatedAt.TabIndex = 35;
        // 
        // lblManualCostUpdate
        // 
        lblManualCostUpdate.Appearance.Font = new Font("Segoe UI", 9F);
        lblManualCostUpdate.Appearance.Options.UseFont = true;
        lblManualCostUpdate.Location = new Point(294, 135);
        lblManualCostUpdate.Name = "lblManualCostUpdate";
        lblManualCostUpdate.Size = new Size(43, 15);
        lblManualCostUpdate.TabIndex = 36;
        lblManualCostUpdate.Text = "Manual:";
        // 
        // tglManualCostUpdate
        // 
        tglManualCostUpdate.EditValue = true;
        tglManualCostUpdate.Location = new Point(350, 131);
        tglManualCostUpdate.Name = "tglManualCostUpdate";
        tglManualCostUpdate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglManualCostUpdate.Properties.Appearance.Options.UseFont = true;
        tglManualCostUpdate.Properties.OffText = "No";
        tglManualCostUpdate.Properties.OnText = "Sí";
        tglManualCostUpdate.Size = new Size(70, 20);
        tglManualCostUpdate.TabIndex = 37;
        // 
        // lblFinanceCostIndicatorsTitle
        // 
        lblFinanceCostIndicatorsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblFinanceCostIndicatorsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblFinanceCostIndicatorsTitle.Appearance.Options.UseFont = true;
        lblFinanceCostIndicatorsTitle.Appearance.Options.UseForeColor = true;
        lblFinanceCostIndicatorsTitle.Location = new Point(933, 12);
        lblFinanceCostIndicatorsTitle.Name = "lblFinanceCostIndicatorsTitle";
        lblFinanceCostIndicatorsTitle.Size = new Size(174, 20);
        lblFinanceCostIndicatorsTitle.TabIndex = 38;
        lblFinanceCostIndicatorsTitle.Text = "3. Indicadores financieros";
        // 
        // kpiFinanceGrossMargin
        // 
        kpiFinanceGrossMargin.AccessibleRole = AccessibleRole.StaticText;
        kpiFinanceGrossMargin.FallbackIconText = "%";
        kpiFinanceGrossMargin.Location = new Point(933, 43);
        kpiFinanceGrossMargin.MinimumSize = new Size(140, 68);
        kpiFinanceGrossMargin.Name = "kpiFinanceGrossMargin";
        kpiFinanceGrossMargin.Size = new Size(200, 75);
        kpiFinanceGrossMargin.StatusBackColor = Color.FromArgb((int)(byte)236, (int)(byte)253, (int)(byte)245);
        kpiFinanceGrossMargin.StatusForeColor = Color.FromArgb((int)(byte)5, (int)(byte)150, (int)(byte)105);
        kpiFinanceGrossMargin.StatusText = "Margen unitario";
        kpiFinanceGrossMargin.TabIndex = 39;
        kpiFinanceGrossMargin.Title = "Margen bruto";
        kpiFinanceGrossMargin.UnitText = "USD";
        kpiFinanceGrossMargin.ValueText = "9.85";
        // 
        // kpiFinanceGrossMarginPercent
        // 
        kpiFinanceGrossMarginPercent.AccentColor = Color.FromArgb((int)(byte)37, (int)(byte)99, (int)(byte)235);
        kpiFinanceGrossMarginPercent.AccessibleRole = AccessibleRole.StaticText;
        kpiFinanceGrossMarginPercent.FallbackIconText = "%";
        kpiFinanceGrossMarginPercent.Location = new Point(1139, 43);
        kpiFinanceGrossMarginPercent.MinimumSize = new Size(140, 68);
        kpiFinanceGrossMarginPercent.Name = "kpiFinanceGrossMarginPercent";
        kpiFinanceGrossMarginPercent.Size = new Size(200, 75);
        kpiFinanceGrossMarginPercent.StatusBackColor = Color.FromArgb((int)(byte)239, (int)(byte)246, (int)(byte)255);
        kpiFinanceGrossMarginPercent.StatusForeColor = Color.FromArgb((int)(byte)29, (int)(byte)78, (int)(byte)216);
        kpiFinanceGrossMarginPercent.StatusText = "Sobre precio base";
        kpiFinanceGrossMarginPercent.TabIndex = 40;
        kpiFinanceGrossMarginPercent.Title = "Margen bruto %";
        kpiFinanceGrossMarginPercent.UnitText = "%";
        kpiFinanceGrossMarginPercent.ValueText = "34.56";
        // 
        // kpiFinanceProfitability
        // 
        kpiFinanceProfitability.AccentColor = Color.FromArgb((int)(byte)124, (int)(byte)58, (int)(byte)237);
        kpiFinanceProfitability.AccessibleRole = AccessibleRole.StaticText;
        kpiFinanceProfitability.FallbackIconText = "R";
        kpiFinanceProfitability.Location = new Point(933, 124);
        kpiFinanceProfitability.MinimumSize = new Size(140, 68);
        kpiFinanceProfitability.Name = "kpiFinanceProfitability";
        kpiFinanceProfitability.Size = new Size(200, 75);
        kpiFinanceProfitability.StatusBackColor = Color.FromArgb((int)(byte)245, (int)(byte)243, (int)(byte)255);
        kpiFinanceProfitability.StatusForeColor = Color.FromArgb((int)(byte)109, (int)(byte)40, (int)(byte)217);
        kpiFinanceProfitability.StatusText = "Últimos 12m";
        kpiFinanceProfitability.TabIndex = 41;
        kpiFinanceProfitability.Title = "Rentabilidad 12m";
        kpiFinanceProfitability.UnitText = "%";
        kpiFinanceProfitability.ValueText = "18.40";
        // 
        // kpiFinanceSuggestedPrice
        // 
        kpiFinanceSuggestedPrice.AccentColor = Color.FromArgb((int)(byte)234, (int)(byte)88, (int)(byte)12);
        kpiFinanceSuggestedPrice.AccessibleRole = AccessibleRole.StaticText;
        kpiFinanceSuggestedPrice.FallbackIconText = "$";
        kpiFinanceSuggestedPrice.Location = new Point(1139, 124);
        kpiFinanceSuggestedPrice.MinimumSize = new Size(140, 68);
        kpiFinanceSuggestedPrice.Name = "kpiFinanceSuggestedPrice";
        kpiFinanceSuggestedPrice.Size = new Size(200, 75);
        kpiFinanceSuggestedPrice.StatusBackColor = Color.FromArgb((int)(byte)255, (int)(byte)247, (int)(byte)237);
        kpiFinanceSuggestedPrice.StatusForeColor = Color.FromArgb((int)(byte)194, (int)(byte)65, (int)(byte)12);
        kpiFinanceSuggestedPrice.StatusText = "Precio calculado";
        kpiFinanceSuggestedPrice.TabIndex = 42;
        kpiFinanceSuggestedPrice.Title = "Precio sugerido";
        kpiFinanceSuggestedPrice.UnitText = "USD";
        kpiFinanceSuggestedPrice.ValueText = "30.00";
        // 
        // tabSales
        // 
        tabSales.Appearance.PageClient.BackColor = Color.White;
        tabSales.Appearance.PageClient.Options.UseBackColor = true;
        tabSales.Controls.Add(labelControl8);
        tabSales.Controls.Add(labelControl7);
        tabSales.Controls.Add(labelControl6);
        tabSales.Controls.Add(labelControl5);
        tabSales.Controls.Add(sepSalesColumnOne);
        tabSales.Controls.Add(sepSalesColumnTwo);
        tabSales.Controls.Add(sepSalesConfiguration);
        tabSales.Controls.Add(sepSalesConditions);
        tabSales.Controls.Add(sepSalesIndicators);
        tabSales.Controls.Add(sepSalesPriceLists);
        tabSales.Controls.Add(lblSalesConditionsTitle);
        tabSales.Controls.Add(lblSalesIndicatorsTitle);
        tabSales.Controls.Add(lblSalesPricePerformanceTitle);
        tabSales.Controls.Add(grdSalesPriceLists);
        tabSales.Controls.Add(lblSalesConfigurationTitle);
        tabSales.Controls.Add(kpiSales30d);
        tabSales.Controls.Add(lblAffectsPromotions);
        tabSales.Controls.Add(kpiSales12m);
        tabSales.Controls.Add(tglAffectsPromotions);
        tabSales.Controls.Add(kpiSalesLastPrice);
        tabSales.Controls.Add(lblSalesUnit);
        tabSales.Controls.Add(kpiSalesCustomers);
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
        tabSales.Controls.Add(lblSalesChannel);
        tabSales.Controls.Add(lueSalesChannel);
        tabSales.Controls.Add(lblSalesSegment);
        tabSales.Controls.Add(lueSalesSegment);
        tabSales.Controls.Add(lblSalesMinimumPriceList);
        tabSales.Controls.Add(lueSalesMinimumPriceList);
        tabSales.Controls.Add(lblSalesMinimumPrice);
        tabSales.Controls.Add(spnSalesMinimumPrice);
        tabSales.Controls.Add(lblSalesMinimumCurrency);
        tabSales.Controls.Add(lblSalesValidFrom);
        tabSales.Controls.Add(dtSalesValidFrom);
        tabSales.Controls.Add(lblSalesEcommerce);
        tabSales.Controls.Add(tglSalesEcommerce);
        tabSales.Controls.Add(lblSalesCommercialObservation);
        tabSales.Controls.Add(memSalesCommercialObservation);
        tabSales.Controls.Add(btnViewSalesHistory);
        tabSales.Controls.Add(btnRefreshSales);
        tabSales.ImageOptions.SvgImageSize = new Size(20, 20);
        tabSales.Name = "tabSales";
        tabSales.Size = new Size(1402, 398);
        tabSales.Text = "Ventas";
        // 
        // labelControl8
        // 
        labelControl8.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        labelControl8.Appearance.Options.UseBackColor = true;
        labelControl8.AutoSizeMode = LabelAutoSizeMode.None;
        labelControl8.Location = new Point(248, 259);
        labelControl8.Name = "labelControl8";
        labelControl8.Size = new Size(790, 1);
        labelControl8.TabIndex = 75;
        // 
        // labelControl7
        // 
        labelControl7.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        labelControl7.Appearance.Options.UseBackColor = true;
        labelControl7.AutoSizeMode = LabelAutoSizeMode.None;
        labelControl7.Location = new Point(1120, 22);
        labelControl7.Name = "labelControl7";
        labelControl7.Size = new Size(240, 1);
        labelControl7.TabIndex = 74;
        // 
        // labelControl6
        // 
        labelControl6.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        labelControl6.Appearance.Options.UseBackColor = true;
        labelControl6.AutoSizeMode = LabelAutoSizeMode.None;
        labelControl6.Location = new Point(796, 22);
        labelControl6.Name = "labelControl6";
        labelControl6.Size = new Size(120, 1);
        labelControl6.TabIndex = 73;
        // 
        // labelControl5
        // 
        labelControl5.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        labelControl5.Appearance.Options.UseBackColor = true;
        labelControl5.AutoSizeMode = LabelAutoSizeMode.None;
        labelControl5.Location = new Point(205, 24);
        labelControl5.Name = "labelControl5";
        labelControl5.Size = new Size(310, 1);
        labelControl5.TabIndex = 72;
        // 
        // sepSalesColumnOne
        // 
        sepSalesColumnOne.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepSalesColumnOne.Appearance.Options.UseBackColor = true;
        sepSalesColumnOne.AutoSizeMode = LabelAutoSizeMode.None;
        sepSalesColumnOne.Location = new Point(520, 12);
        sepSalesColumnOne.Name = "sepSalesColumnOne";
        sepSalesColumnOne.Size = new Size(1, 224);
        sepSalesColumnOne.TabIndex = 46;
        // 
        // sepSalesColumnTwo
        // 
        sepSalesColumnTwo.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepSalesColumnTwo.Appearance.Options.UseBackColor = true;
        sepSalesColumnTwo.AutoSizeMode = LabelAutoSizeMode.None;
        sepSalesColumnTwo.Location = new Point(930, 12);
        sepSalesColumnTwo.Name = "sepSalesColumnTwo";
        sepSalesColumnTwo.Size = new Size(1, 224);
        sepSalesColumnTwo.TabIndex = 47;
        // 
        // sepSalesConfiguration
        // 
        sepSalesConfiguration.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepSalesConfiguration.Appearance.Options.UseBackColor = true;
        sepSalesConfiguration.Location = new Point(205, 22);
        sepSalesConfiguration.Name = "sepSalesConfiguration";
        sepSalesConfiguration.Size = new Size(0, 13);
        sepSalesConfiguration.TabIndex = 48;
        // 
        // sepSalesConditions
        // 
        sepSalesConditions.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepSalesConditions.Appearance.Options.UseBackColor = true;
        sepSalesConditions.Location = new Point(790, 22);
        sepSalesConditions.Name = "sepSalesConditions";
        sepSalesConditions.Size = new Size(0, 13);
        sepSalesConditions.TabIndex = 49;
        // 
        // sepSalesIndicators
        // 
        sepSalesIndicators.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        sepSalesIndicators.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepSalesIndicators.Appearance.Options.UseBackColor = true;
        sepSalesIndicators.Location = new Point(1156, 22);
        sepSalesIndicators.Name = "sepSalesIndicators";
        sepSalesIndicators.Size = new Size(0, 13);
        sepSalesIndicators.TabIndex = 50;
        // 
        // sepSalesPriceLists
        // 
        sepSalesPriceLists.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        sepSalesPriceLists.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepSalesPriceLists.Appearance.Options.UseBackColor = true;
        sepSalesPriceLists.Location = new Point(263, 262);
        sepSalesPriceLists.Name = "sepSalesPriceLists";
        sepSalesPriceLists.Size = new Size(0, 13);
        sepSalesPriceLists.TabIndex = 51;
        // 
        // lblSalesConditionsTitle
        // 
        lblSalesConditionsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSalesConditionsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSalesConditionsTitle.Appearance.Options.UseFont = true;
        lblSalesConditionsTitle.Appearance.Options.UseForeColor = true;
        lblSalesConditionsTitle.Location = new Point(541, 12);
        lblSalesConditionsTitle.Name = "lblSalesConditionsTitle";
        lblSalesConditionsTitle.Size = new Size(238, 20);
        lblSalesConditionsTitle.TabIndex = 52;
        lblSalesConditionsTitle.Text = "2. Condiciones de comercialización";
        // 
        // lblSalesIndicatorsTitle
        // 
        lblSalesIndicatorsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSalesIndicatorsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSalesIndicatorsTitle.Appearance.Options.UseFont = true;
        lblSalesIndicatorsTitle.Appearance.Options.UseForeColor = true;
        lblSalesIndicatorsTitle.Location = new Point(954, 12);
        lblSalesIndicatorsTitle.Name = "lblSalesIndicatorsTitle";
        lblSalesIndicatorsTitle.Size = new Size(158, 20);
        lblSalesIndicatorsTitle.TabIndex = 53;
        lblSalesIndicatorsTitle.Text = "3. Indicadores de venta";
        // 
        // lblSalesPricePerformanceTitle
        // 
        lblSalesPricePerformanceTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSalesPricePerformanceTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSalesPricePerformanceTitle.Appearance.Options.UseFont = true;
        lblSalesPricePerformanceTitle.Appearance.Options.UseForeColor = true;
        lblSalesPricePerformanceTitle.Location = new Point(12, 248);
        lblSalesPricePerformanceTitle.Name = "lblSalesPricePerformanceTitle";
        lblSalesPricePerformanceTitle.Size = new Size(216, 20);
        lblSalesPricePerformanceTitle.TabIndex = 0;
        lblSalesPricePerformanceTitle.Text = "4. Listas de precio y desempeño";
        // 
        // grdSalesPriceLists
        // 
        grdSalesPriceLists.Anchor = (AnchorStyles)(((AnchorStyles.Top) | (AnchorStyles.Bottom)) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        grdSalesPriceLists.DataSource = salesPriceListsTable;
        grdSalesPriceLists.Location = new Point(12, 282);
        grdSalesPriceLists.MainView = gvSalesPriceLists;
        grdSalesPriceLists.Name = "grdSalesPriceLists";
        grdSalesPriceLists.RepositoryItems.AddRange(new RepositoryItem[] { repoSalesPriceListActive });
        grdSalesPriceLists.Size = new Size(1370, 100);
        grdSalesPriceLists.TabIndex = 1;
        grdSalesPriceLists.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvSalesPriceLists, gvSalesPriceListsAux });
        // 
        // gvSalesPriceLists
        // 
        gvSalesPriceLists.Appearance.FocusedRow.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        gvSalesPriceLists.Appearance.FocusedRow.ForeColor = Color.FromArgb((int)(byte)23, (int)(byte)32, (int)(byte)51);
        gvSalesPriceLists.Appearance.FocusedRow.Options.UseBackColor = true;
        gvSalesPriceLists.Appearance.FocusedRow.Options.UseForeColor = true;
        gvSalesPriceLists.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvSalesPriceLists.Appearance.HeaderPanel.Options.UseFont = true;
        gvSalesPriceLists.Appearance.HideSelectionRow.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        gvSalesPriceLists.Appearance.HideSelectionRow.ForeColor = Color.FromArgb((int)(byte)23, (int)(byte)32, (int)(byte)51);
        gvSalesPriceLists.Appearance.HideSelectionRow.Options.UseBackColor = true;
        gvSalesPriceLists.Appearance.HideSelectionRow.Options.UseForeColor = true;
        gvSalesPriceLists.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvSalesPriceLists.Appearance.Row.Options.UseFont = true;
        gvSalesPriceLists.Columns.AddRange(new GridColumn[] { colSalesPriceListName, colSalesPriceListCurrency, colSalesPriceListPrice, colSalesPriceListMargin, colSalesPriceListValidFrom, colSalesPriceListActive });
        gvSalesPriceLists.GridControl = grdSalesPriceLists;
        gvSalesPriceLists.Name = "gvSalesPriceLists";
        gvSalesPriceLists.OptionsBehavior.Editable = false;
        gvSalesPriceLists.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvSalesPriceLists.OptionsView.ShowGroupPanel = false;
        gvSalesPriceLists.OptionsView.ShowIndicator = false;
        gvSalesPriceLists.RowHeight = 28;
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
        colSalesPriceListPrice.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colSalesPriceListPrice.Caption = "Precio";
        colSalesPriceListPrice.DisplayFormat.FormatString = "N2";
        colSalesPriceListPrice.DisplayFormat.FormatType = FormatType.Numeric;
        colSalesPriceListPrice.FieldName = "Precio";
        colSalesPriceListPrice.Name = "colSalesPriceListPrice";
        colSalesPriceListPrice.Visible = true;
        colSalesPriceListPrice.VisibleIndex = 2;
        colSalesPriceListPrice.Width = 84;
        // 
        // colSalesPriceListMargin
        // 
        colSalesPriceListMargin.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colSalesPriceListMargin.Caption = "Margen %";
        colSalesPriceListMargin.DisplayFormat.FormatString = "N2";
        colSalesPriceListMargin.DisplayFormat.FormatType = FormatType.Numeric;
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
        // repoSalesPriceListActive
        // 
        repoSalesPriceListActive.AutoHeight = false;
        repoSalesPriceListActive.Name = "repoSalesPriceListActive";
        // 
        // gvSalesPriceListsAux
        // 
        gvSalesPriceListsAux.GridControl = grdSalesPriceLists;
        gvSalesPriceListsAux.Name = "gvSalesPriceListsAux";
        // 
        // lblSalesConfigurationTitle
        // 
        lblSalesConfigurationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSalesConfigurationTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblSalesConfigurationTitle.Appearance.Options.UseFont = true;
        lblSalesConfigurationTitle.Appearance.Options.UseForeColor = true;
        lblSalesConfigurationTitle.Location = new Point(12, 12);
        lblSalesConfigurationTitle.Name = "lblSalesConfigurationTitle";
        lblSalesConfigurationTitle.Size = new Size(181, 20);
        lblSalesConfigurationTitle.TabIndex = 24;
        lblSalesConfigurationTitle.Text = "1. Configuración comercial";
        // 
        // kpiSales30d
        // 
        kpiSales30d.AccessibleRole = AccessibleRole.StaticText;
        kpiSales30d.FallbackIconText = "V";
        kpiSales30d.Location = new Point(954, 46);
        kpiSales30d.MinimumSize = new Size(140, 68);
        kpiSales30d.Name = "kpiSales30d";
        kpiSales30d.Size = new Size(200, 75);
        kpiSales30d.StatusText = "Últimos 30d";
        kpiSales30d.TabIndex = 2;
        kpiSales30d.Title = "Ventas 30d";
        kpiSales30d.UnitText = "UND";
        kpiSales30d.ValueText = "1,050.00";
        // 
        // lblAffectsPromotions
        // 
        lblAffectsPromotions.Appearance.Font = new Font("Segoe UI", 9F);
        lblAffectsPromotions.Appearance.Options.UseFont = true;
        lblAffectsPromotions.Location = new Point(17, 211);
        lblAffectsPromotions.Name = "lblAffectsPromotions";
        lblAffectsPromotions.Size = new Size(110, 15);
        lblAffectsPromotions.TabIndex = 9;
        lblAffectsPromotions.Text = "Afecta promociones:";
        // 
        // kpiSales12m
        // 
        kpiSales12m.AccentColor = Color.FromArgb((int)(byte)0, (int)(byte)137, (int)(byte)123);
        kpiSales12m.AccessibleRole = AccessibleRole.StaticText;
        kpiSales12m.FallbackIconText = "12";
        kpiSales12m.Location = new Point(1160, 46);
        kpiSales12m.MinimumSize = new Size(140, 68);
        kpiSales12m.Name = "kpiSales12m";
        kpiSales12m.Size = new Size(200, 75);
        kpiSales12m.StatusBackColor = Color.FromArgb((int)(byte)229, (int)(byte)247, (int)(byte)245);
        kpiSales12m.StatusForeColor = Color.FromArgb((int)(byte)0, (int)(byte)105, (int)(byte)92);
        kpiSales12m.StatusText = "Acumulado 12m";
        kpiSales12m.TabIndex = 3;
        kpiSales12m.Title = "Ventas 12m";
        kpiSales12m.UnitText = "UND";
        kpiSales12m.ValueText = "12,420.00";
        // 
        // tglAffectsPromotions
        // 
        tglAffectsPromotions.EditValue = true;
        tglAffectsPromotions.Location = new Point(156, 209);
        tglAffectsPromotions.Name = "tglAffectsPromotions";
        tglAffectsPromotions.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAffectsPromotions.Properties.Appearance.Options.UseFont = true;
        tglAffectsPromotions.Properties.OffText = "No";
        tglAffectsPromotions.Properties.OnText = "Sí";
        tglAffectsPromotions.Size = new Size(86, 20);
        tglAffectsPromotions.TabIndex = 10;
        // 
        // kpiSalesLastPrice
        // 
        kpiSalesLastPrice.AccentColor = Color.FromArgb((int)(byte)37, (int)(byte)99, (int)(byte)235);
        kpiSalesLastPrice.AccessibleRole = AccessibleRole.StaticText;
        kpiSalesLastPrice.FallbackIconText = "$";
        kpiSalesLastPrice.Location = new Point(954, 127);
        kpiSalesLastPrice.MinimumSize = new Size(140, 68);
        kpiSalesLastPrice.Name = "kpiSalesLastPrice";
        kpiSalesLastPrice.Size = new Size(200, 75);
        kpiSalesLastPrice.StatusBackColor = Color.FromArgb((int)(byte)239, (int)(byte)246, (int)(byte)255);
        kpiSalesLastPrice.StatusForeColor = Color.FromArgb((int)(byte)29, (int)(byte)78, (int)(byte)216);
        kpiSalesLastPrice.StatusText = "Precio facturado";
        kpiSalesLastPrice.TabIndex = 4;
        kpiSalesLastPrice.Title = "Último precio";
        kpiSalesLastPrice.UnitText = "USD";
        kpiSalesLastPrice.ValueText = "28.50";
        // 
        // lblSalesUnit
        // 
        lblSalesUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesUnit.Appearance.Options.UseFont = true;
        lblSalesUnit.Location = new Point(17, 44);
        lblSalesUnit.Name = "lblSalesUnit";
        lblSalesUnit.Size = new Size(89, 15);
        lblSalesUnit.TabIndex = 25;
        lblSalesUnit.Text = "Unidad de venta:";
        // 
        // kpiSalesCustomers
        // 
        kpiSalesCustomers.AccentColor = Color.FromArgb((int)(byte)147, (int)(byte)51, (int)(byte)234);
        kpiSalesCustomers.AccessibleRole = AccessibleRole.StaticText;
        kpiSalesCustomers.FallbackIconText = "C";
        kpiSalesCustomers.Location = new Point(1160, 127);
        kpiSalesCustomers.MinimumSize = new Size(140, 68);
        kpiSalesCustomers.Name = "kpiSalesCustomers";
        kpiSalesCustomers.Size = new Size(200, 75);
        kpiSalesCustomers.StatusBackColor = Color.FromArgb((int)(byte)250, (int)(byte)245, (int)(byte)255);
        kpiSalesCustomers.StatusForeColor = Color.FromArgb((int)(byte)126, (int)(byte)34, (int)(byte)206);
        kpiSalesCustomers.StatusText = "Últimos 12m";
        kpiSalesCustomers.TabIndex = 5;
        kpiSalesCustomers.Title = "Clientes activos";
        kpiSalesCustomers.ValueText = "86";
        // 
        // lueSalesUnit
        // 
        lueSalesUnit.Location = new Point(156, 41);
        lueSalesUnit.Name = "lueSalesUnit";
        lueSalesUnit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSalesUnit.Properties.Appearance.Options.UseFont = true;
        lueSalesUnit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSalesUnit.Properties.NullText = "UND - Unidad";
        lueSalesUnit.Size = new Size(166, 22);
        lueSalesUnit.TabIndex = 26;
        // 
        // lblBaseSalesPrice
        // 
        lblBaseSalesPrice.Appearance.Font = new Font("Segoe UI", 9F);
        lblBaseSalesPrice.Appearance.Options.UseFont = true;
        lblBaseSalesPrice.Location = new Point(17, 72);
        lblBaseSalesPrice.Name = "lblBaseSalesPrice";
        lblBaseSalesPrice.Size = new Size(63, 15);
        lblBaseSalesPrice.TabIndex = 27;
        lblBaseSalesPrice.Text = "Precio base:";
        // 
        // spnBaseSalesPrice
        // 
        spnBaseSalesPrice.EditValue = new decimal(new int[] { 2850, 0, 0, 131072 });
        spnBaseSalesPrice.Location = new Point(156, 69);
        spnBaseSalesPrice.Name = "spnBaseSalesPrice";
        spnBaseSalesPrice.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnBaseSalesPrice.Properties.Appearance.Options.UseFont = true;
        spnBaseSalesPrice.Properties.Appearance.Options.UseTextOptions = true;
        spnBaseSalesPrice.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnBaseSalesPrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnBaseSalesPrice.Properties.MaskSettings.Set("mask", "n2");
        spnBaseSalesPrice.Size = new Size(98, 22);
        spnBaseSalesPrice.TabIndex = 28;
        // 
        // lueSalesCurrency
        // 
        lueSalesCurrency.Location = new Point(260, 69);
        lueSalesCurrency.Name = "lueSalesCurrency";
        lueSalesCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSalesCurrency.Properties.Appearance.Options.UseFont = true;
        lueSalesCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSalesCurrency.Properties.NullText = "USD";
        lueSalesCurrency.Size = new Size(62, 22);
        lueSalesCurrency.TabIndex = 29;
        // 
        // lblMainPriceList
        // 
        lblMainPriceList.Appearance.Font = new Font("Segoe UI", 9F);
        lblMainPriceList.Appearance.Options.UseFont = true;
        lblMainPriceList.Location = new Point(17, 100);
        lblMainPriceList.Name = "lblMainPriceList";
        lblMainPriceList.Size = new Size(133, 15);
        lblMainPriceList.TabIndex = 30;
        lblMainPriceList.Text = "Lista de precios principal:";
        // 
        // lueMainPriceList
        // 
        lueMainPriceList.Location = new Point(156, 97);
        lueMainPriceList.Name = "lueMainPriceList";
        lueMainPriceList.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueMainPriceList.Properties.Appearance.Options.UseFont = true;
        lueMainPriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueMainPriceList.Properties.NullText = "Minorista";
        lueMainPriceList.Size = new Size(166, 22);
        lueMainPriceList.TabIndex = 31;
        // 
        // lblAllowSalesDiscount
        // 
        lblAllowSalesDiscount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowSalesDiscount.Appearance.Options.UseFont = true;
        lblAllowSalesDiscount.Location = new Point(284, 184);
        lblAllowSalesDiscount.Name = "lblAllowSalesDiscount";
        lblAllowSalesDiscount.Size = new Size(102, 15);
        lblAllowSalesDiscount.TabIndex = 32;
        lblAllowSalesDiscount.Text = "Permite descuento:";
        // 
        // tglAllowSalesDiscount
        // 
        tglAllowSalesDiscount.EditValue = true;
        tglAllowSalesDiscount.Location = new Point(400, 182);
        tglAllowSalesDiscount.Name = "tglAllowSalesDiscount";
        tglAllowSalesDiscount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAllowSalesDiscount.Properties.Appearance.Options.UseFont = true;
        tglAllowSalesDiscount.Properties.OffText = "No";
        tglAllowSalesDiscount.Properties.OnText = "Sí";
        tglAllowSalesDiscount.Size = new Size(86, 20);
        tglAllowSalesDiscount.TabIndex = 33;
        // 
        // lblMaxDiscount
        // 
        lblMaxDiscount.Appearance.Font = new Font("Segoe UI", 9F);
        lblMaxDiscount.Appearance.Options.UseFont = true;
        lblMaxDiscount.Location = new Point(17, 156);
        lblMaxDiscount.Name = "lblMaxDiscount";
        lblMaxDiscount.Size = new Size(126, 15);
        lblMaxDiscount.TabIndex = 34;
        lblMaxDiscount.Text = "Descuento máximo (%):";
        // 
        // spnMaxDiscount
        // 
        spnMaxDiscount.EditValue = new decimal(new int[] { 1500, 0, 0, 131072 });
        spnMaxDiscount.Location = new Point(156, 153);
        spnMaxDiscount.Name = "spnMaxDiscount";
        spnMaxDiscount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMaxDiscount.Properties.Appearance.Options.UseFont = true;
        spnMaxDiscount.Properties.Appearance.Options.UseTextOptions = true;
        spnMaxDiscount.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnMaxDiscount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMaxDiscount.Properties.MaskSettings.Set("mask", "n2");
        spnMaxDiscount.Size = new Size(117, 22);
        spnMaxDiscount.TabIndex = 35;
        // 
        // lblMinimumMargin
        // 
        lblMinimumMargin.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumMargin.Appearance.Options.UseFont = true;
        lblMinimumMargin.Location = new Point(284, 156);
        lblMinimumMargin.Name = "lblMinimumMargin";
        lblMinimumMargin.Size = new Size(110, 15);
        lblMinimumMargin.TabIndex = 36;
        lblMinimumMargin.Text = "Margen mínimo (%):";
        // 
        // spnMinimumMargin
        // 
        spnMinimumMargin.EditValue = new decimal(new int[] { 1000, 0, 0, 131072 });
        spnMinimumMargin.Location = new Point(400, 153);
        spnMinimumMargin.Name = "spnMinimumMargin";
        spnMinimumMargin.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMinimumMargin.Properties.Appearance.Options.UseFont = true;
        spnMinimumMargin.Properties.Appearance.Options.UseTextOptions = true;
        spnMinimumMargin.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnMinimumMargin.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMinimumMargin.Properties.MaskSettings.Set("mask", "n2");
        spnMinimumMargin.Size = new Size(117, 22);
        spnMinimumMargin.TabIndex = 37;
        // 
        // lblMinimumSale
        // 
        lblMinimumSale.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumSale.Appearance.Options.UseFont = true;
        lblMinimumSale.Location = new Point(17, 128);
        lblMinimumSale.Name = "lblMinimumSale";
        lblMinimumSale.Size = new Size(77, 15);
        lblMinimumSale.TabIndex = 38;
        lblMinimumSale.Text = "Venta mínima:";
        // 
        // spnMinimumSale
        // 
        spnMinimumSale.EditValue = new decimal(new int[] { 100, 0, 0, 131072 });
        spnMinimumSale.Location = new Point(156, 125);
        spnMinimumSale.Name = "spnMinimumSale";
        spnMinimumSale.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMinimumSale.Properties.Appearance.Options.UseFont = true;
        spnMinimumSale.Properties.Appearance.Options.UseTextOptions = true;
        spnMinimumSale.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnMinimumSale.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMinimumSale.Properties.MaskSettings.Set("mask", "n2");
        spnMinimumSale.Size = new Size(86, 22);
        spnMinimumSale.TabIndex = 39;
        // 
        // lblMinimumSaleUnit
        // 
        lblMinimumSaleUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumSaleUnit.Appearance.Options.UseFont = true;
        lblMinimumSaleUnit.Location = new Point(248, 128);
        lblMinimumSaleUnit.Name = "lblMinimumSaleUnit";
        lblMinimumSaleUnit.Size = new Size(25, 15);
        lblMinimumSaleUnit.TabIndex = 40;
        lblMinimumSaleUnit.Text = "UND";
        // 
        // lblSalesMultiple
        // 
        lblSalesMultiple.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesMultiple.Appearance.Options.UseFont = true;
        lblSalesMultiple.Location = new Point(284, 128);
        lblSalesMultiple.Name = "lblSalesMultiple";
        lblSalesMultiple.Size = new Size(96, 15);
        lblSalesMultiple.TabIndex = 41;
        lblSalesMultiple.Text = "Múltiplo de venta:";
        // 
        // spnSalesMultiple
        // 
        spnSalesMultiple.EditValue = new decimal(new int[] { 100, 0, 0, 131072 });
        spnSalesMultiple.Location = new Point(400, 125);
        spnSalesMultiple.Name = "spnSalesMultiple";
        spnSalesMultiple.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSalesMultiple.Properties.Appearance.Options.UseFont = true;
        spnSalesMultiple.Properties.Appearance.Options.UseTextOptions = true;
        spnSalesMultiple.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSalesMultiple.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSalesMultiple.Properties.MaskSettings.Set("mask", "n2");
        spnSalesMultiple.Size = new Size(86, 22);
        spnSalesMultiple.TabIndex = 42;
        // 
        // lblSalesMultipleUnit
        // 
        lblSalesMultipleUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesMultipleUnit.Appearance.Options.UseFont = true;
        lblSalesMultipleUnit.Location = new Point(492, 128);
        lblSalesMultipleUnit.Name = "lblSalesMultipleUnit";
        lblSalesMultipleUnit.Size = new Size(25, 15);
        lblSalesMultipleUnit.TabIndex = 43;
        lblSalesMultipleUnit.Text = "UND";
        // 
        // lblSalesCommission
        // 
        lblSalesCommission.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesCommission.Appearance.Options.UseFont = true;
        lblSalesCommission.Location = new Point(17, 184);
        lblSalesCommission.Name = "lblSalesCommission";
        lblSalesCommission.Size = new Size(75, 15);
        lblSalesCommission.TabIndex = 44;
        lblSalesCommission.Text = "Comisión (%):";
        // 
        // spnSalesCommission
        // 
        spnSalesCommission.EditValue = new decimal(new int[] { 300, 0, 0, 131072 });
        spnSalesCommission.Location = new Point(156, 181);
        spnSalesCommission.Name = "spnSalesCommission";
        spnSalesCommission.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSalesCommission.Properties.Appearance.Options.UseFont = true;
        spnSalesCommission.Properties.Appearance.Options.UseTextOptions = true;
        spnSalesCommission.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSalesCommission.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSalesCommission.Properties.MaskSettings.Set("mask", "n2");
        spnSalesCommission.Size = new Size(117, 22);
        spnSalesCommission.TabIndex = 45;
        // 
        // lblSalesChannel
        // 
        lblSalesChannel.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesChannel.Appearance.Options.UseFont = true;
        lblSalesChannel.Location = new Point(541, 50);
        lblSalesChannel.Name = "lblSalesChannel";
        lblSalesChannel.Size = new Size(82, 15);
        lblSalesChannel.TabIndex = 55;
        lblSalesChannel.Text = "Canal principal:";
        // 
        // lueSalesChannel
        // 
        lueSalesChannel.Location = new Point(707, 47);
        lueSalesChannel.Name = "lueSalesChannel";
        lueSalesChannel.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSalesChannel.Properties.Appearance.Options.UseFont = true;
        lueSalesChannel.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSalesChannel.Properties.NullText = "";
        lueSalesChannel.Size = new Size(208, 22);
        lueSalesChannel.TabIndex = 56;
        // 
        // lblSalesSegment
        // 
        lblSalesSegment.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesSegment.Appearance.Options.UseFont = true;
        lblSalesSegment.Location = new Point(541, 78);
        lblSalesSegment.Name = "lblSalesSegment";
        lblSalesSegment.Size = new Size(57, 15);
        lblSalesSegment.TabIndex = 57;
        lblSalesSegment.Text = "Segmento:";
        // 
        // lueSalesSegment
        // 
        lueSalesSegment.Location = new Point(707, 75);
        lueSalesSegment.Name = "lueSalesSegment";
        lueSalesSegment.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSalesSegment.Properties.Appearance.Options.UseFont = true;
        lueSalesSegment.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSalesSegment.Properties.NullText = "";
        lueSalesSegment.Size = new Size(208, 22);
        lueSalesSegment.TabIndex = 58;
        // 
        // lblSalesMinimumPriceList
        // 
        lblSalesMinimumPriceList.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesMinimumPriceList.Appearance.Options.UseFont = true;
        lblSalesMinimumPriceList.Location = new Point(541, 106);
        lblSalesMinimumPriceList.Name = "lblSalesMinimumPriceList";
        lblSalesMinimumPriceList.Size = new Size(125, 15);
        lblSalesMinimumPriceList.TabIndex = 59;
        lblSalesMinimumPriceList.Text = "Lista mínima permitida:";
        // 
        // lueSalesMinimumPriceList
        // 
        lueSalesMinimumPriceList.Location = new Point(707, 103);
        lueSalesMinimumPriceList.Name = "lueSalesMinimumPriceList";
        lueSalesMinimumPriceList.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSalesMinimumPriceList.Properties.Appearance.Options.UseFont = true;
        lueSalesMinimumPriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSalesMinimumPriceList.Properties.NullText = "";
        lueSalesMinimumPriceList.Size = new Size(208, 22);
        lueSalesMinimumPriceList.TabIndex = 60;
        // 
        // lblSalesMinimumPrice
        // 
        lblSalesMinimumPrice.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesMinimumPrice.Appearance.Options.UseFont = true;
        lblSalesMinimumPrice.Location = new Point(541, 134);
        lblSalesMinimumPrice.Name = "lblSalesMinimumPrice";
        lblSalesMinimumPrice.Size = new Size(81, 15);
        lblSalesMinimumPrice.TabIndex = 61;
        lblSalesMinimumPrice.Text = "Precio mínimo:";
        // 
        // spnSalesMinimumPrice
        // 
        spnSalesMinimumPrice.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnSalesMinimumPrice.Location = new Point(707, 131);
        spnSalesMinimumPrice.Name = "spnSalesMinimumPrice";
        spnSalesMinimumPrice.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSalesMinimumPrice.Properties.Appearance.Options.UseFont = true;
        spnSalesMinimumPrice.Properties.Appearance.Options.UseTextOptions = true;
        spnSalesMinimumPrice.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSalesMinimumPrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSalesMinimumPrice.Properties.MaskSettings.Set("mask", "n2");
        spnSalesMinimumPrice.Size = new Size(120, 22);
        spnSalesMinimumPrice.TabIndex = 62;
        // 
        // lblSalesMinimumCurrency
        // 
        lblSalesMinimumCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesMinimumCurrency.Appearance.Options.UseFont = true;
        lblSalesMinimumCurrency.Location = new Point(837, 134);
        lblSalesMinimumCurrency.Name = "lblSalesMinimumCurrency";
        lblSalesMinimumCurrency.Size = new Size(22, 15);
        lblSalesMinimumCurrency.TabIndex = 63;
        lblSalesMinimumCurrency.Text = "USD";
        // 
        // lblSalesValidFrom
        // 
        lblSalesValidFrom.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesValidFrom.Appearance.Options.UseFont = true;
        lblSalesValidFrom.Location = new Point(541, 162);
        lblSalesValidFrom.Name = "lblSalesValidFrom";
        lblSalesValidFrom.Size = new Size(82, 15);
        lblSalesValidFrom.TabIndex = 64;
        lblSalesValidFrom.Text = "Vigencia desde:";
        // 
        // dtSalesValidFrom
        // 
        dtSalesValidFrom.EditValue = null;
        dtSalesValidFrom.Location = new Point(707, 159);
        dtSalesValidFrom.Name = "dtSalesValidFrom";
        dtSalesValidFrom.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dtSalesValidFrom.Properties.Appearance.Options.UseFont = true;
        dtSalesValidFrom.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dtSalesValidFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dtSalesValidFrom.Size = new Size(208, 22);
        dtSalesValidFrom.TabIndex = 65;
        // 
        // lblSalesEcommerce
        // 
        lblSalesEcommerce.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesEcommerce.Appearance.Options.UseFont = true;
        lblSalesEcommerce.Location = new Point(541, 190);
        lblSalesEcommerce.Name = "lblSalesEcommerce";
        lblSalesEcommerce.Size = new Size(130, 15);
        lblSalesEcommerce.TabIndex = 66;
        lblSalesEcommerce.Text = "Disponible e-commerce:";
        // 
        // tglSalesEcommerce
        // 
        tglSalesEcommerce.EditValue = true;
        tglSalesEcommerce.Location = new Point(707, 187);
        tglSalesEcommerce.Name = "tglSalesEcommerce";
        tglSalesEcommerce.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglSalesEcommerce.Properties.Appearance.Options.UseFont = true;
        tglSalesEcommerce.Properties.OffText = "No";
        tglSalesEcommerce.Properties.OnText = "Sí";
        tglSalesEcommerce.Size = new Size(86, 20);
        tglSalesEcommerce.TabIndex = 67;
        // 
        // lblSalesCommercialObservation
        // 
        lblSalesCommercialObservation.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesCommercialObservation.Appearance.Options.UseFont = true;
        lblSalesCommercialObservation.Location = new Point(541, 218);
        lblSalesCommercialObservation.Name = "lblSalesCommercialObservation";
        lblSalesCommercialObservation.Size = new Size(124, 15);
        lblSalesCommercialObservation.TabIndex = 68;
        lblSalesCommercialObservation.Text = "Observación comercial:";
        // 
        // memSalesCommercialObservation
        // 
        memSalesCommercialObservation.Location = new Point(707, 215);
        memSalesCommercialObservation.Name = "memSalesCommercialObservation";
        memSalesCommercialObservation.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memSalesCommercialObservation.Properties.Appearance.Options.UseFont = true;
        memSalesCommercialObservation.Size = new Size(208, 30);
        memSalesCommercialObservation.TabIndex = 69;
        // 
        // btnViewSalesHistory
        // 
        btnViewSalesHistory.Appearance.ForeColor = Color.FromArgb((int)(byte)37, (int)(byte)99, (int)(byte)235);
        btnViewSalesHistory.Appearance.Options.UseForeColor = true;
        btnViewSalesHistory.Location = new Point(1120, 248);
        btnViewSalesHistory.Name = "btnViewSalesHistory";
        btnViewSalesHistory.PaintStyle = PaintStyles.Light;
        btnViewSalesHistory.Size = new Size(112, 28);
        btnViewSalesHistory.TabIndex = 70;
        btnViewSalesHistory.Text = "Ver historial";
        // 
        // btnRefreshSales
        // 
        btnRefreshSales.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        btnRefreshSales.Appearance.Options.UseForeColor = true;
        btnRefreshSales.Location = new Point(1250, 248);
        btnRefreshSales.Name = "btnRefreshSales";
        btnRefreshSales.PaintStyle = PaintStyles.Light;
        btnRefreshSales.Size = new Size(112, 28);
        btnRefreshSales.TabIndex = 71;
        btnRefreshSales.Text = "Actualizar";
        // 
        // tabInventory
        // 
        tabInventory.Controls.Add(sepInventoryColumnOne);
        tabInventory.Controls.Add(sepInventoryColumnTwo);
        tabInventory.Controls.Add(sepInventoryParameters);
        tabInventory.Controls.Add(sepInventoryReplenishment);
        tabInventory.Controls.Add(sepInventoryLocations);
        tabInventory.Controls.Add(sepInventoryWarehouse);
        tabInventory.Controls.Add(lblWarehouseSummary);
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
        tabInventory.ImageOptions.SvgImageSize = new Size(22, 22);
        tabInventory.Name = "tabInventory";
        tabInventory.Size = new Size(1406, 426);
        tabInventory.Text = "Inventario";
        // 
        // sepInventoryColumnOne
        // 
        sepInventoryColumnOne.Appearance.BackColor = Color.FromArgb((int)(byte)235, (int)(byte)238, (int)(byte)242);
        sepInventoryColumnOne.Appearance.Options.UseBackColor = true;
        sepInventoryColumnOne.AutoSizeMode = LabelAutoSizeMode.None;
        sepInventoryColumnOne.Location = new Point(397, 12);
        sepInventoryColumnOne.Name = "sepInventoryColumnOne";
        sepInventoryColumnOne.Size = new Size(1, 224);
        sepInventoryColumnOne.TabIndex = 44;
        // 
        // sepInventoryColumnTwo
        // 
        sepInventoryColumnTwo.Appearance.BackColor = Color.FromArgb((int)(byte)235, (int)(byte)238, (int)(byte)242);
        sepInventoryColumnTwo.Appearance.Options.UseBackColor = true;
        sepInventoryColumnTwo.AutoSizeMode = LabelAutoSizeMode.None;
        sepInventoryColumnTwo.Location = new Point(895, 12);
        sepInventoryColumnTwo.Name = "sepInventoryColumnTwo";
        sepInventoryColumnTwo.Size = new Size(1, 224);
        sepInventoryColumnTwo.TabIndex = 45;
        // 
        // sepInventoryParameters
        // 
        sepInventoryParameters.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepInventoryParameters.Appearance.Options.UseBackColor = true;
        sepInventoryParameters.AutoSizeMode = LabelAutoSizeMode.None;
        sepInventoryParameters.Location = new Point(207, 22);
        sepInventoryParameters.Name = "sepInventoryParameters";
        sepInventoryParameters.Size = new Size(170, 1);
        sepInventoryParameters.TabIndex = 46;
        // 
        // sepInventoryReplenishment
        // 
        sepInventoryReplenishment.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepInventoryReplenishment.Appearance.Options.UseBackColor = true;
        sepInventoryReplenishment.AutoSizeMode = LabelAutoSizeMode.None;
        sepInventoryReplenishment.Location = new Point(604, 22);
        sepInventoryReplenishment.Name = "sepInventoryReplenishment";
        sepInventoryReplenishment.Size = new Size(270, 1);
        sepInventoryReplenishment.TabIndex = 47;
        // 
        // sepInventoryLocations
        // 
        sepInventoryLocations.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        sepInventoryLocations.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepInventoryLocations.Appearance.Options.UseBackColor = true;
        sepInventoryLocations.AutoSizeMode = LabelAutoSizeMode.None;
        sepInventoryLocations.Location = new Point(1127, 22);
        sepInventoryLocations.Name = "sepInventoryLocations";
        sepInventoryLocations.Size = new Size(259, 1);
        sepInventoryLocations.TabIndex = 48;
        // 
        // sepInventoryWarehouse
        // 
        sepInventoryWarehouse.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepInventoryWarehouse.Appearance.Options.UseBackColor = true;
        sepInventoryWarehouse.AutoSizeMode = LabelAutoSizeMode.None;
        sepInventoryWarehouse.Location = new Point(159, 262);
        sepInventoryWarehouse.Name = "sepInventoryWarehouse";
        sepInventoryWarehouse.Size = new Size(783, 1);
        sepInventoryWarehouse.TabIndex = 49;
        // 
        // lblWarehouseSummary
        // 
        lblWarehouseSummary.Anchor = (AnchorStyles)(AnchorStyles.Bottom) | (AnchorStyles.Left);
        lblWarehouseSummary.Appearance.Font = new Font("Segoe UI", 9F);
        lblWarehouseSummary.Appearance.ForeColor = Color.FromArgb((int)(byte)71, (int)(byte)85, (int)(byte)105);
        lblWarehouseSummary.Appearance.Options.UseFont = true;
        lblWarehouseSummary.Appearance.Options.UseForeColor = true;
        lblWarehouseSummary.AutoSizeMode = LabelAutoSizeMode.None;
        lblWarehouseSummary.Location = new Point(12, 403);
        lblWarehouseSummary.Name = "lblWarehouseSummary";
        lblWarehouseSummary.Size = new Size(760, 20);
        lblWarehouseSummary.TabIndex = 50;
        lblWarehouseSummary.Text = "0 bodegas   •   Disponible: 0.00 UND   •   Principal: -";
        // 
        // btnAddWarehouseStock
        // 
        btnAddWarehouseStock.Anchor = (AnchorStyles)(AnchorStyles.Top) | (AnchorStyles.Right);
        btnAddWarehouseStock.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddWarehouseStock.Appearance.Options.UseFont = true;
        btnAddWarehouseStock.Location = new Point(954, 244);
        btnAddWarehouseStock.Name = "btnAddWarehouseStock";
        btnAddWarehouseStock.Size = new Size(86, 32);
        btnAddWarehouseStock.TabIndex = 40;
        btnAddWarehouseStock.Text = "Agregar";
        // 
        // btnUpdateWarehouseStock
        // 
        btnUpdateWarehouseStock.Anchor = (AnchorStyles)(AnchorStyles.Top) | (AnchorStyles.Right);
        btnUpdateWarehouseStock.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnUpdateWarehouseStock.Appearance.Options.UseFont = true;
        btnUpdateWarehouseStock.Location = new Point(1048, 244);
        btnUpdateWarehouseStock.Name = "btnUpdateWarehouseStock";
        btnUpdateWarehouseStock.Size = new Size(86, 32);
        btnUpdateWarehouseStock.TabIndex = 41;
        btnUpdateWarehouseStock.Text = "Editar";
        // 
        // btnRemoveWarehouseStock
        // 
        btnRemoveWarehouseStock.Anchor = (AnchorStyles)(AnchorStyles.Top) | (AnchorStyles.Right);
        btnRemoveWarehouseStock.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRemoveWarehouseStock.Appearance.Options.UseFont = true;
        btnRemoveWarehouseStock.Location = new Point(1142, 244);
        btnRemoveWarehouseStock.Name = "btnRemoveWarehouseStock";
        btnRemoveWarehouseStock.Size = new Size(86, 32);
        btnRemoveWarehouseStock.TabIndex = 42;
        btnRemoveWarehouseStock.Text = "Quitar";
        // 
        // btnSetMainWarehouseStock
        // 
        btnSetMainWarehouseStock.Anchor = (AnchorStyles)(AnchorStyles.Top) | (AnchorStyles.Right);
        btnSetMainWarehouseStock.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetMainWarehouseStock.Appearance.Options.UseFont = true;
        btnSetMainWarehouseStock.Location = new Point(1236, 244);
        btnSetMainWarehouseStock.Name = "btnSetMainWarehouseStock";
        btnSetMainWarehouseStock.Size = new Size(150, 32);
        btnSetMainWarehouseStock.TabIndex = 43;
        btnSetMainWarehouseStock.Text = "Marcar principal";
        // 
        // lblInventoryLocationsRestrictionsTitle
        // 
        lblInventoryLocationsRestrictionsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblInventoryLocationsRestrictionsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblInventoryLocationsRestrictionsTitle.Appearance.Options.UseFont = true;
        lblInventoryLocationsRestrictionsTitle.Appearance.Options.UseForeColor = true;
        lblInventoryLocationsRestrictionsTitle.Location = new Point(921, 12);
        lblInventoryLocationsRestrictionsTitle.Name = "lblInventoryLocationsRestrictionsTitle";
        lblInventoryLocationsRestrictionsTitle.Size = new Size(197, 20);
        lblInventoryLocationsRestrictionsTitle.TabIndex = 0;
        lblInventoryLocationsRestrictionsTitle.Text = "3. Ubicaciones / restricciones";
        // 
        // lblDefaultBinLocation
        // 
        lblDefaultBinLocation.Location = new Point(927, 50);
        lblDefaultBinLocation.Name = "lblDefaultBinLocation";
        lblDefaultBinLocation.Size = new Size(89, 13);
        lblDefaultBinLocation.TabIndex = 1;
        lblDefaultBinLocation.Text = "Ubicación defecto:";
        // 
        // lblStockByWarehouseTitle
        // 
        lblStockByWarehouseTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblStockByWarehouseTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblStockByWarehouseTitle.Appearance.Options.UseFont = true;
        lblStockByWarehouseTitle.Appearance.Options.UseForeColor = true;
        lblStockByWarehouseTitle.Location = new Point(12, 252);
        lblStockByWarehouseTitle.Name = "lblStockByWarehouseTitle";
        lblStockByWarehouseTitle.Size = new Size(138, 20);
        lblStockByWarehouseTitle.TabIndex = 0;
        lblStockByWarehouseTitle.Text = "4. Stock por bodega";
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
        slueDefaultBinLocation.Size = new Size(323, 22);
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
        // grdWarehouseStock
        // 
        grdWarehouseStock.Anchor = (AnchorStyles)(((AnchorStyles.Top) | (AnchorStyles.Bottom)) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        grdWarehouseStock.DataSource = warehouseStockTable;
        grdWarehouseStock.Location = new Point(12, 284);
        grdWarehouseStock.MainView = gvWarehouseStock;
        grdWarehouseStock.Name = "grdWarehouseStock";
        grdWarehouseStock.Size = new Size(1374, 112);
        grdWarehouseStock.TabIndex = 1;
        grdWarehouseStock.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvWarehouseStock, gvWarehouseStockAux });
        // 
        // gvWarehouseStock
        // 
        gvWarehouseStock.Appearance.FocusedRow.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        gvWarehouseStock.Appearance.FocusedRow.ForeColor = Color.FromArgb((int)(byte)23, (int)(byte)32, (int)(byte)51);
        gvWarehouseStock.Appearance.FocusedRow.Options.UseBackColor = true;
        gvWarehouseStock.Appearance.FocusedRow.Options.UseForeColor = true;
        gvWarehouseStock.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvWarehouseStock.Appearance.HeaderPanel.Options.UseFont = true;
        gvWarehouseStock.Appearance.HideSelectionRow.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        gvWarehouseStock.Appearance.HideSelectionRow.ForeColor = Color.FromArgb((int)(byte)23, (int)(byte)32, (int)(byte)51);
        gvWarehouseStock.Appearance.HideSelectionRow.Options.UseBackColor = true;
        gvWarehouseStock.Appearance.HideSelectionRow.Options.UseForeColor = true;
        gvWarehouseStock.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvWarehouseStock.Appearance.Row.Options.UseFont = true;
        gvWarehouseStock.Columns.AddRange(new GridColumn[] { colWarehouseCode, colWarehouseName, colWarehouseStockActual, colWarehouseCommitted, colWarehouseOrdered, colWarehouseAvailable, colWarehouseMinimum, colWarehouseMaximum, colWarehouseReorder, colWarehouseStatus });
        gvWarehouseStock.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvWarehouseStock.GridControl = grdWarehouseStock;
        gvWarehouseStock.Name = "gvWarehouseStock";
        gvWarehouseStock.OptionsBehavior.Editable = false;
        gvWarehouseStock.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvWarehouseStock.OptionsView.ShowGroupPanel = false;
        gvWarehouseStock.OptionsView.ShowIndicator = false;
        gvWarehouseStock.RowHeight = 28;
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
        colWarehouseStockActual.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colWarehouseStockActual.Caption = "Stock actual";
        colWarehouseStockActual.DisplayFormat.FormatString = "N2";
        colWarehouseStockActual.DisplayFormat.FormatType = FormatType.Numeric;
        colWarehouseStockActual.FieldName = "StockActual";
        colWarehouseStockActual.Name = "colWarehouseStockActual";
        colWarehouseStockActual.Visible = true;
        colWarehouseStockActual.VisibleIndex = 2;
        // 
        // colWarehouseCommitted
        // 
        colWarehouseCommitted.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colWarehouseCommitted.Caption = "Comprometido";
        colWarehouseCommitted.DisplayFormat.FormatString = "N2";
        colWarehouseCommitted.DisplayFormat.FormatType = FormatType.Numeric;
        colWarehouseCommitted.FieldName = "Comprometido";
        colWarehouseCommitted.Name = "colWarehouseCommitted";
        colWarehouseCommitted.Visible = true;
        colWarehouseCommitted.VisibleIndex = 3;
        // 
        // colWarehouseOrdered
        // 
        colWarehouseOrdered.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colWarehouseOrdered.Caption = "Pedido";
        colWarehouseOrdered.DisplayFormat.FormatString = "N2";
        colWarehouseOrdered.DisplayFormat.FormatType = FormatType.Numeric;
        colWarehouseOrdered.FieldName = "Pedido";
        colWarehouseOrdered.Name = "colWarehouseOrdered";
        colWarehouseOrdered.Visible = true;
        colWarehouseOrdered.VisibleIndex = 4;
        // 
        // colWarehouseAvailable
        // 
        colWarehouseAvailable.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colWarehouseAvailable.Caption = "Disponible";
        colWarehouseAvailable.DisplayFormat.FormatString = "N2";
        colWarehouseAvailable.DisplayFormat.FormatType = FormatType.Numeric;
        colWarehouseAvailable.FieldName = "Disponible";
        colWarehouseAvailable.Name = "colWarehouseAvailable";
        colWarehouseAvailable.Visible = true;
        colWarehouseAvailable.VisibleIndex = 5;
        // 
        // colWarehouseMinimum
        // 
        colWarehouseMinimum.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colWarehouseMinimum.Caption = "Mínimo";
        colWarehouseMinimum.DisplayFormat.FormatString = "N2";
        colWarehouseMinimum.DisplayFormat.FormatType = FormatType.Numeric;
        colWarehouseMinimum.FieldName = "Minimo";
        colWarehouseMinimum.Name = "colWarehouseMinimum";
        colWarehouseMinimum.Visible = true;
        colWarehouseMinimum.VisibleIndex = 6;
        // 
        // colWarehouseMaximum
        // 
        colWarehouseMaximum.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colWarehouseMaximum.Caption = "Máximo";
        colWarehouseMaximum.DisplayFormat.FormatString = "N2";
        colWarehouseMaximum.DisplayFormat.FormatType = FormatType.Numeric;
        colWarehouseMaximum.FieldName = "Maximo";
        colWarehouseMaximum.Name = "colWarehouseMaximum";
        colWarehouseMaximum.Visible = true;
        colWarehouseMaximum.VisibleIndex = 7;
        // 
        // colWarehouseReorder
        // 
        colWarehouseReorder.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colWarehouseReorder.Caption = "Reorden";
        colWarehouseReorder.DisplayFormat.FormatString = "N2";
        colWarehouseReorder.DisplayFormat.FormatType = FormatType.Numeric;
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
        // gvWarehouseStockAux
        // 
        gvWarehouseStockAux.GridControl = grdWarehouseStock;
        gvWarehouseStockAux.Name = "gvWarehouseStockAux";
        // 
        // lblCoverageDays
        // 
        lblCoverageDays.Location = new Point(422, 135);
        lblCoverageDays.Name = "lblCoverageDays";
        lblCoverageDays.Size = new Size(74, 13);
        lblCoverageDays.TabIndex = 7;
        lblCoverageDays.Text = "Días cobertura:";
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
        // lblReplenishmentOperationTitle
        // 
        lblReplenishmentOperationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblReplenishmentOperationTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblReplenishmentOperationTitle.Appearance.Options.UseFont = true;
        lblReplenishmentOperationTitle.Appearance.Options.UseForeColor = true;
        lblReplenishmentOperationTitle.Location = new Point(420, 12);
        lblReplenishmentOperationTitle.Name = "lblReplenishmentOperationTitle";
        lblReplenishmentOperationTitle.Size = new Size(175, 20);
        lblReplenishmentOperationTitle.TabIndex = 0;
        lblReplenishmentOperationTitle.Text = "2. Reposición y operación";
        // 
        // lblLeadTimeDays
        // 
        lblLeadTimeDays.Location = new Point(676, 135);
        lblLeadTimeDays.Name = "lblLeadTimeDays";
        lblLeadTimeDays.Size = new Size(77, 13);
        lblLeadTimeDays.TabIndex = 9;
        lblLeadTimeDays.Text = "Reposición días:";
        // 
        // lblInventoryParametersTitle
        // 
        lblInventoryParametersTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblInventoryParametersTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblInventoryParametersTitle.Appearance.Options.UseFont = true;
        lblInventoryParametersTitle.Appearance.Options.UseForeColor = true;
        lblInventoryParametersTitle.Location = new Point(12, 12);
        lblInventoryParametersTitle.Name = "lblInventoryParametersTitle";
        lblInventoryParametersTitle.Size = new Size(187, 20);
        lblInventoryParametersTitle.TabIndex = 21;
        lblInventoryParametersTitle.Text = "1. Parámetros de inventario";
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
        // lblReplenishmentMethod
        // 
        lblReplenishmentMethod.Location = new Point(422, 107);
        lblReplenishmentMethod.Name = "lblReplenishmentMethod";
        lblReplenishmentMethod.Size = new Size(91, 13);
        lblReplenishmentMethod.TabIndex = 5;
        lblReplenishmentMethod.Text = "Método reposición:";
        // 
        // lblGlobalMinStock
        // 
        lblGlobalMinStock.Location = new Point(422, 163);
        lblGlobalMinStock.Name = "lblGlobalMinStock";
        lblGlobalMinStock.Size = new Size(84, 13);
        lblGlobalMinStock.TabIndex = 11;
        lblGlobalMinStock.Text = "Stock mín. global:";
        // 
        // lblSupplyMethod
        // 
        lblSupplyMethod.Location = new Point(422, 78);
        lblSupplyMethod.Name = "lblSupplyMethod";
        lblSupplyMethod.Size = new Size(77, 13);
        lblSupplyMethod.TabIndex = 3;
        lblSupplyMethod.Text = "Abastecimiento:";
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
        // lblGlobalMaxStock
        // 
        lblGlobalMaxStock.Location = new Point(676, 163);
        lblGlobalMaxStock.Name = "lblGlobalMaxStock";
        lblGlobalMaxStock.Size = new Size(57, 13);
        lblGlobalMaxStock.TabIndex = 13;
        lblGlobalMaxStock.Text = "Stock máx.:";
        // 
        // lblMainWarehouse
        // 
        lblMainWarehouse.Location = new Point(422, 50);
        lblMainWarehouse.Name = "lblMainWarehouse";
        lblMainWarehouse.Size = new Size(82, 13);
        lblMainWarehouse.TabIndex = 1;
        lblMainWarehouse.Text = "Bodega principal:";
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
        // lblBlockedForMovements
        // 
        lblBlockedForMovements.Location = new Point(927, 78);
        lblBlockedForMovements.Name = "lblBlockedForMovements";
        lblBlockedForMovements.Size = new Size(104, 13);
        lblBlockedForMovements.TabIndex = 17;
        lblBlockedForMovements.Text = "Bloquea movimientos:";
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
        // tglBlockedForMovements
        // 
        tglBlockedForMovements.Location = new Point(1063, 76);
        tglBlockedForMovements.Name = "tglBlockedForMovements";
        tglBlockedForMovements.Properties.OffText = "No";
        tglBlockedForMovements.Properties.OnText = "Sí";
        tglBlockedForMovements.Size = new Size(86, 18);
        tglBlockedForMovements.TabIndex = 18;
        // 
        // lblGlobalReorderPoint
        // 
        lblGlobalReorderPoint.Location = new Point(422, 193);
        lblGlobalReorderPoint.Name = "lblGlobalReorderPoint";
        lblGlobalReorderPoint.Size = new Size(73, 13);
        lblGlobalReorderPoint.TabIndex = 15;
        lblGlobalReorderPoint.Text = "Punto reorden:";
        // 
        // lblInventoryOperationNote
        // 
        lblInventoryOperationNote.Location = new Point(927, 107);
        lblInventoryOperationNote.Name = "lblInventoryOperationNote";
        lblInventoryOperationNote.Size = new Size(113, 13);
        lblInventoryOperationNote.TabIndex = 19;
        lblInventoryOperationNote.Text = "Observación operativa:";
        // 
        // lblValuationMethod
        // 
        lblValuationMethod.Location = new Point(15, 50);
        lblValuationMethod.Name = "lblValuationMethod";
        lblValuationMethod.Size = new Size(88, 13);
        lblValuationMethod.TabIndex = 22;
        lblValuationMethod.Text = "Método valuación:";
        // 
        // memInventoryOperationNote
        // 
        memInventoryOperationNote.EditValue = "Reposición automática según punto de reorden. Revisar quincenalmente el stock mínimo. Almacenar en zona seca.";
        memInventoryOperationNote.Location = new Point(1063, 104);
        memInventoryOperationNote.Name = "memInventoryOperationNote";
        memInventoryOperationNote.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memInventoryOperationNote.Properties.Appearance.Options.UseFont = true;
        memInventoryOperationNote.Size = new Size(323, 49);
        memInventoryOperationNote.TabIndex = 20;
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
        // lblSuggestedPurchaseQty
        // 
        lblSuggestedPurchaseQty.Location = new Point(676, 191);
        lblSuggestedPurchaseQty.Name = "lblSuggestedPurchaseQty";
        lblSuggestedPurchaseQty.Size = new Size(65, 13);
        lblSuggestedPurchaseQty.TabIndex = 17;
        lblSuggestedPurchaseQty.Text = "Compra sug.:";
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
        // lblNegativeStockPolicy
        // 
        lblNegativeStockPolicy.Location = new Point(15, 78);
        lblNegativeStockPolicy.Name = "lblNegativeStockPolicy";
        lblNegativeStockPolicy.Size = new Size(75, 13);
        lblNegativeStockPolicy.TabIndex = 24;
        lblNegativeStockPolicy.Text = "Stock negativo:";
        // 
        // lblReplenishmentApproval
        // 
        lblReplenishmentApproval.Location = new Point(422, 219);
        lblReplenishmentApproval.Name = "lblReplenishmentApproval";
        lblReplenishmentApproval.Size = new Size(109, 13);
        lblReplenishmentApproval.TabIndex = 19;
        lblReplenishmentApproval.Text = "Aprobación reposición:";
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
        // tglReplenishmentApproval
        // 
        tglReplenishmentApproval.Location = new Point(545, 215);
        tglReplenishmentApproval.Name = "tglReplenishmentApproval";
        tglReplenishmentApproval.Properties.OffText = "No";
        tglReplenishmentApproval.Properties.OnText = "Sí";
        tglReplenishmentApproval.Size = new Size(86, 18);
        tglReplenishmentApproval.TabIndex = 20;
        // 
        // lblAutoReplenishment
        // 
        lblAutoReplenishment.Location = new Point(15, 104);
        lblAutoReplenishment.Name = "lblAutoReplenishment";
        lblAutoReplenishment.Size = new Size(111, 13);
        lblAutoReplenishment.TabIndex = 26;
        lblAutoReplenishment.Text = "Reposición automática:";
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
        // lblManageLocations
        // 
        lblManageLocations.Location = new Point(15, 132);
        lblManageLocations.Name = "lblManageLocations";
        lblManageLocations.Size = new Size(97, 13);
        lblManageLocations.TabIndex = 28;
        lblManageLocations.Text = "Maneja ubicaciones:";
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
        // lblRequiresCycleCount
        // 
        lblRequiresCycleCount.Location = new Point(15, 160);
        lblRequiresCycleCount.Name = "lblRequiresCycleCount";
        lblRequiresCycleCount.Size = new Size(69, 13);
        lblRequiresCycleCount.TabIndex = 30;
        lblRequiresCycleCount.Text = "Conteo cíclico:";
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
        // lblAbcClassification
        // 
        lblAbcClassification.Location = new Point(15, 190);
        lblAbcClassification.Name = "lblAbcClassification";
        lblAbcClassification.Size = new Size(85, 13);
        lblAbcClassification.TabIndex = 32;
        lblAbcClassification.Text = "Clasificación ABC:";
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
        // lblInventoryControlType
        // 
        lblInventoryControlType.Location = new Point(15, 218);
        lblInventoryControlType.Name = "lblInventoryControlType";
        lblInventoryControlType.Size = new Size(60, 13);
        lblInventoryControlType.TabIndex = 34;
        lblInventoryControlType.Text = "Tipo control:";
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
        // lblInventoryBlockReason
        // 
        lblInventoryBlockReason.Location = new Point(927, 162);
        lblInventoryBlockReason.Name = "lblInventoryBlockReason";
        lblInventoryBlockReason.Size = new Size(77, 13);
        lblInventoryBlockReason.TabIndex = 38;
        lblInventoryBlockReason.Text = "Motivo bloqueo:";
        // 
        // memInventoryBlockReason
        // 
        memInventoryBlockReason.Location = new Point(1063, 159);
        memInventoryBlockReason.Name = "memInventoryBlockReason";
        memInventoryBlockReason.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memInventoryBlockReason.Properties.Appearance.Options.UseFont = true;
        memInventoryBlockReason.Size = new Size(323, 49);
        memInventoryBlockReason.TabIndex = 39;
        // 
        // tabUnits
        // 
        tabUnits.Controls.Add(sepUnitsColumn);
        tabUnits.Controls.Add(sepUnitsMeasures);
        tabUnits.Controls.Add(sepUnitsIdentifiers);
        tabUnits.Controls.Add(sepUnitsPresentations);
        tabUnits.Controls.Add(lblPresentationSummary);
        tabUnits.Controls.Add(lblCodesIdentifiersTitle);
        tabUnits.Controls.Add(lblQrCode);
        tabUnits.Controls.Add(lblPurchasePresentationsTitle);
        tabUnits.Controls.Add(txtQrCode);
        tabUnits.Controls.Add(grdItemPresentations);
        tabUnits.Controls.Add(lblPlu);
        tabUnits.Controls.Add(lblInventoryUnitTitle);
        tabUnits.Controls.Add(txtPlu);
        tabUnits.Controls.Add(btnAddItemPresentation);
        tabUnits.Controls.Add(lblPreviousInternalCode);
        tabUnits.Controls.Add(lblInventoryUnit);
        tabUnits.Controls.Add(txtPreviousInternalCode);
        tabUnits.Controls.Add(btnUpdateItemPresentation);
        tabUnits.Controls.Add(lblManufacturerReference);
        tabUnits.Controls.Add(lueInventoryUnit);
        tabUnits.Controls.Add(txtManufacturerReference);
        tabUnits.Controls.Add(btnRemoveItemPresentation);
        tabUnits.Controls.Add(lblUnspscCode);
        tabUnits.Controls.Add(txtUnspscCode);
        tabUnits.Controls.Add(btnSetMainItemPresentation);
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
        tabUnits.ImageOptions.SvgImageSize = new Size(22, 22);
        tabUnits.Name = "tabUnits";
        tabUnits.Size = new Size(1406, 426);
        tabUnits.Text = "Unidades y códigos";
        // 
        // sepUnitsColumn
        // 
        sepUnitsColumn.Appearance.BackColor = Color.FromArgb((int)(byte)235, (int)(byte)238, (int)(byte)242);
        sepUnitsColumn.Appearance.Options.UseBackColor = true;
        sepUnitsColumn.AutoSizeMode = LabelAutoSizeMode.None;
        sepUnitsColumn.Location = new Point(450, 12);
        sepUnitsColumn.Name = "sepUnitsColumn";
        sepUnitsColumn.Size = new Size(1, 405);
        sepUnitsColumn.TabIndex = 40;
        // 
        // sepUnitsMeasures
        // 
        sepUnitsMeasures.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepUnitsMeasures.Appearance.Options.UseBackColor = true;
        sepUnitsMeasures.AutoSizeMode = LabelAutoSizeMode.None;
        sepUnitsMeasures.Location = new Point(185, 22);
        sepUnitsMeasures.Name = "sepUnitsMeasures";
        sepUnitsMeasures.Size = new Size(250, 1);
        sepUnitsMeasures.TabIndex = 41;
        // 
        // sepUnitsIdentifiers
        // 
        sepUnitsIdentifiers.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepUnitsIdentifiers.Appearance.Options.UseBackColor = true;
        sepUnitsIdentifiers.AutoSizeMode = LabelAutoSizeMode.None;
        sepUnitsIdentifiers.Location = new Point(207, 207);
        sepUnitsIdentifiers.Name = "sepUnitsIdentifiers";
        sepUnitsIdentifiers.Size = new Size(228, 1);
        sepUnitsIdentifiers.TabIndex = 42;
        // 
        // sepUnitsPresentations
        // 
        sepUnitsPresentations.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        sepUnitsPresentations.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        sepUnitsPresentations.Appearance.Options.UseBackColor = true;
        sepUnitsPresentations.AutoSizeMode = LabelAutoSizeMode.None;
        sepUnitsPresentations.Location = new Point(724, 22);
        sepUnitsPresentations.Name = "sepUnitsPresentations";
        sepUnitsPresentations.Size = new Size(218, 1);
        sepUnitsPresentations.TabIndex = 43;
        // 
        // lblPresentationSummary
        // 
        lblPresentationSummary.Anchor = (AnchorStyles)(AnchorStyles.Bottom) | (AnchorStyles.Left);
        lblPresentationSummary.Appearance.Font = new Font("Segoe UI", 9F);
        lblPresentationSummary.Appearance.ForeColor = Color.FromArgb((int)(byte)71, (int)(byte)85, (int)(byte)105);
        lblPresentationSummary.Appearance.Options.UseFont = true;
        lblPresentationSummary.Appearance.Options.UseForeColor = true;
        lblPresentationSummary.AutoSizeMode = LabelAutoSizeMode.None;
        lblPresentationSummary.Location = new Point(464, 397);
        lblPresentationSummary.Name = "lblPresentationSummary";
        lblPresentationSummary.Size = new Size(520, 20);
        lblPresentationSummary.TabIndex = 44;
        lblPresentationSummary.Text = "0 presentaciones   •   0 activas   •   Principal: -";
        // 
        // lblCodesIdentifiersTitle
        // 
        lblCodesIdentifiersTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblCodesIdentifiersTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblCodesIdentifiersTitle.Appearance.Options.UseFont = true;
        lblCodesIdentifiersTitle.Appearance.Options.UseForeColor = true;
        lblCodesIdentifiersTitle.Location = new Point(12, 197);
        lblCodesIdentifiersTitle.Name = "lblCodesIdentifiersTitle";
        lblCodesIdentifiersTitle.Size = new Size(187, 20);
        lblCodesIdentifiersTitle.TabIndex = 0;
        lblCodesIdentifiersTitle.Text = "2. Identificadores generales";
        // 
        // lblQrCode
        // 
        lblQrCode.Location = new Point(17, 235);
        lblQrCode.Name = "lblQrCode";
        lblQrCode.Size = new Size(55, 13);
        lblQrCode.TabIndex = 1;
        lblQrCode.Text = "Código QR:";
        // 
        // lblPurchasePresentationsTitle
        // 
        lblPurchasePresentationsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchasePresentationsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblPurchasePresentationsTitle.Appearance.Options.UseFont = true;
        lblPurchasePresentationsTitle.Appearance.Options.UseForeColor = true;
        lblPurchasePresentationsTitle.Location = new Point(464, 12);
        lblPurchasePresentationsTitle.Name = "lblPurchasePresentationsTitle";
        lblPurchasePresentationsTitle.Size = new Size(257, 20);
        lblPurchasePresentationsTitle.TabIndex = 0;
        lblPurchasePresentationsTitle.Text = "3. Presentaciones, unidades y códigos";
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
        // grdItemPresentations
        // 
        grdItemPresentations.Anchor = (AnchorStyles)(((AnchorStyles.Top) | (AnchorStyles.Bottom)) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        grdItemPresentations.DataSource = itemPresentationsTable;
        grdItemPresentations.Location = new Point(464, 46);
        grdItemPresentations.MainView = gvItemPresentations;
        grdItemPresentations.Name = "grdItemPresentations";
        grdItemPresentations.RepositoryItems.AddRange(new RepositoryItem[] { repoPurchasePrincipal, repoPurchaseActive });
        grdItemPresentations.Size = new Size(922, 340);
        grdItemPresentations.TabIndex = 1;
        grdItemPresentations.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvItemPresentations, gvItemPresentationsAux });
        // 
        // gvItemPresentations
        // 
        gvItemPresentations.Appearance.FocusedRow.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        gvItemPresentations.Appearance.FocusedRow.ForeColor = Color.FromArgb((int)(byte)23, (int)(byte)32, (int)(byte)51);
        gvItemPresentations.Appearance.FocusedRow.Options.UseBackColor = true;
        gvItemPresentations.Appearance.FocusedRow.Options.UseForeColor = true;
        gvItemPresentations.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvItemPresentations.Appearance.HeaderPanel.Options.UseFont = true;
        gvItemPresentations.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvItemPresentations.Appearance.Row.Options.UseFont = true;
        gvItemPresentations.Columns.AddRange(new GridColumn[] { colPurchasePresentation, colPurchaseUnit, colPurchaseFactor, colPurchaseBarcode, colPurchaseEnabled, colSalesEnabled, colPurchasePrincipal, colSalesPrincipal, colPurchaseActive });
        gvItemPresentations.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvItemPresentations.GridControl = grdItemPresentations;
        gvItemPresentations.Name = "gvItemPresentations";
        gvItemPresentations.OptionsBehavior.Editable = false;
        gvItemPresentations.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvItemPresentations.OptionsView.ShowGroupPanel = false;
        gvItemPresentations.OptionsView.ShowIndicator = false;
        gvItemPresentations.RowHeight = 28;
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
        colPurchaseFactor.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colPurchaseFactor.Caption = "Factor";
        colPurchaseFactor.DisplayFormat.FormatString = "N3";
        colPurchaseFactor.DisplayFormat.FormatType = FormatType.Numeric;
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
        // repoPurchaseActive
        // 
        repoPurchaseActive.AutoHeight = false;
        repoPurchaseActive.Name = "repoPurchaseActive";
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
        // repoPurchasePrincipal
        // 
        repoPurchasePrincipal.AutoHeight = false;
        repoPurchasePrincipal.Name = "repoPurchasePrincipal";
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
        // gvItemPresentationsAux
        // 
        gvItemPresentationsAux.GridControl = grdItemPresentations;
        gvItemPresentationsAux.Name = "gvItemPresentationsAux";
        // 
        // lblPlu
        // 
        lblPlu.Location = new Point(17, 263);
        lblPlu.Name = "lblPlu";
        lblPlu.Size = new Size(22, 13);
        lblPlu.TabIndex = 3;
        lblPlu.Text = "PLU:";
        // 
        // lblInventoryUnitTitle
        // 
        lblInventoryUnitTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblInventoryUnitTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblInventoryUnitTitle.Appearance.Options.UseFont = true;
        lblInventoryUnitTitle.Appearance.Options.UseForeColor = true;
        lblInventoryUnitTitle.Location = new Point(12, 12);
        lblInventoryUnitTitle.Name = "lblInventoryUnitTitle";
        lblInventoryUnitTitle.Size = new Size(151, 20);
        lblInventoryUnitTitle.TabIndex = 20;
        lblInventoryUnitTitle.Text = "1. Unidades y medidas";
        // 
        // txtPlu
        // 
        txtPlu.Location = new Point(127, 259);
        txtPlu.Name = "txtPlu";
        txtPlu.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPlu.Properties.Appearance.Options.UseFont = true;
        txtPlu.Size = new Size(308, 22);
        txtPlu.TabIndex = 4;
        // 
        // btnAddItemPresentation
        // 
        btnAddItemPresentation.Anchor = (AnchorStyles)(AnchorStyles.Top) | (AnchorStyles.Right);
        btnAddItemPresentation.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddItemPresentation.Appearance.Options.UseFont = true;
        btnAddItemPresentation.Location = new Point(954, 5);
        btnAddItemPresentation.Name = "btnAddItemPresentation";
        btnAddItemPresentation.Size = new Size(86, 32);
        btnAddItemPresentation.TabIndex = 2;
        btnAddItemPresentation.Text = "Agregar";
        // 
        // lblPreviousInternalCode
        // 
        lblPreviousInternalCode.Location = new Point(17, 291);
        lblPreviousInternalCode.Name = "lblPreviousInternalCode";
        lblPreviousInternalCode.Size = new Size(78, 13);
        lblPreviousInternalCode.TabIndex = 5;
        lblPreviousInternalCode.Text = "Código anterior:";
        // 
        // lblInventoryUnit
        // 
        lblInventoryUnit.Location = new Point(18, 50);
        lblInventoryUnit.Name = "lblInventoryUnit";
        lblInventoryUnit.Size = new Size(103, 13);
        lblInventoryUnit.TabIndex = 21;
        lblInventoryUnit.Text = "Unidad de inventario:";
        // 
        // txtPreviousInternalCode
        // 
        txtPreviousInternalCode.Location = new Point(127, 287);
        txtPreviousInternalCode.Name = "txtPreviousInternalCode";
        txtPreviousInternalCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPreviousInternalCode.Properties.Appearance.Options.UseFont = true;
        txtPreviousInternalCode.Size = new Size(308, 22);
        txtPreviousInternalCode.TabIndex = 6;
        // 
        // btnUpdateItemPresentation
        // 
        btnUpdateItemPresentation.Anchor = (AnchorStyles)(AnchorStyles.Top) | (AnchorStyles.Right);
        btnUpdateItemPresentation.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnUpdateItemPresentation.Appearance.Options.UseFont = true;
        btnUpdateItemPresentation.Location = new Point(1048, 5);
        btnUpdateItemPresentation.Name = "btnUpdateItemPresentation";
        btnUpdateItemPresentation.Size = new Size(86, 32);
        btnUpdateItemPresentation.TabIndex = 3;
        btnUpdateItemPresentation.Text = "Editar";
        // 
        // lblManufacturerReference
        // 
        lblManufacturerReference.Location = new Point(17, 319);
        lblManufacturerReference.Name = "lblManufacturerReference";
        lblManufacturerReference.Size = new Size(77, 13);
        lblManufacturerReference.TabIndex = 7;
        lblManufacturerReference.Text = "Ref. fabricante:";
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
        // txtManufacturerReference
        // 
        txtManufacturerReference.Location = new Point(127, 315);
        txtManufacturerReference.Name = "txtManufacturerReference";
        txtManufacturerReference.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtManufacturerReference.Properties.Appearance.Options.UseFont = true;
        txtManufacturerReference.Size = new Size(308, 22);
        txtManufacturerReference.TabIndex = 8;
        // 
        // btnRemoveItemPresentation
        // 
        btnRemoveItemPresentation.Anchor = (AnchorStyles)(AnchorStyles.Top) | (AnchorStyles.Right);
        btnRemoveItemPresentation.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRemoveItemPresentation.Appearance.Options.UseFont = true;
        btnRemoveItemPresentation.Location = new Point(1142, 5);
        btnRemoveItemPresentation.Name = "btnRemoveItemPresentation";
        btnRemoveItemPresentation.Size = new Size(86, 32);
        btnRemoveItemPresentation.TabIndex = 4;
        btnRemoveItemPresentation.Text = "Quitar";
        // 
        // lblUnspscCode
        // 
        lblUnspscCode.Location = new Point(17, 347);
        lblUnspscCode.Name = "lblUnspscCode";
        lblUnspscCode.Size = new Size(66, 13);
        lblUnspscCode.TabIndex = 9;
        lblUnspscCode.Text = "SAT/UNSPSC:";
        // 
        // txtUnspscCode
        // 
        txtUnspscCode.Location = new Point(127, 343);
        txtUnspscCode.Name = "txtUnspscCode";
        txtUnspscCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtUnspscCode.Properties.Appearance.Options.UseFont = true;
        txtUnspscCode.Size = new Size(308, 22);
        txtUnspscCode.TabIndex = 10;
        // 
        // btnSetMainItemPresentation
        // 
        btnSetMainItemPresentation.Anchor = (AnchorStyles)(AnchorStyles.Top) | (AnchorStyles.Right);
        btnSetMainItemPresentation.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetMainItemPresentation.Appearance.Options.UseFont = true;
        btnSetMainItemPresentation.Location = new Point(1236, 5);
        btnSetMainItemPresentation.Name = "btnSetMainItemPresentation";
        btnSetMainItemPresentation.Size = new Size(150, 32);
        btnSetMainItemPresentation.TabIndex = 5;
        btnSetMainItemPresentation.Text = "Marcar principal";
        // 
        // lblTariffCode
        // 
        lblTariffCode.Location = new Point(17, 378);
        lblTariffCode.Name = "lblTariffCode";
        lblTariffCode.Size = new Size(58, 13);
        lblTariffCode.TabIndex = 11;
        lblTariffCode.Text = "Arancelario:";
        // 
        // txtTariffCode
        // 
        txtTariffCode.Location = new Point(127, 371);
        txtTariffCode.Name = "txtTariffCode";
        txtTariffCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtTariffCode.Properties.Appearance.Options.UseFont = true;
        txtTariffCode.Size = new Size(96, 22);
        txtTariffCode.TabIndex = 12;
        // 
        // lblNetWeight
        // 
        lblNetWeight.Location = new Point(18, 134);
        lblNetWeight.Name = "lblNetWeight";
        lblNetWeight.Size = new Size(52, 13);
        lblNetWeight.TabIndex = 27;
        lblNetWeight.Text = "Peso neto:";
        // 
        // lblCodeOrigin
        // 
        lblCodeOrigin.Location = new Point(258, 375);
        lblCodeOrigin.Name = "lblCodeOrigin";
        lblCodeOrigin.Size = new Size(36, 13);
        lblCodeOrigin.TabIndex = 13;
        lblCodeOrigin.Text = "Origen:";
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
        // lblNetWeightUnit
        // 
        lblNetWeightUnit.Location = new Point(231, 134);
        lblNetWeightUnit.Name = "lblNetWeightUnit";
        lblNetWeightUnit.Size = new Size(11, 13);
        lblNetWeightUnit.TabIndex = 29;
        lblNetWeightUnit.Text = "kg";
        // 
        // lblGrossWeight
        // 
        lblGrossWeight.Location = new Point(258, 134);
        lblGrossWeight.Name = "lblGrossWeight";
        lblGrossWeight.Size = new Size(56, 13);
        lblGrossWeight.TabIndex = 30;
        lblGrossWeight.Text = "Peso bruto:";
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
        // lblGrossWeightUnit
        // 
        lblGrossWeightUnit.Location = new Point(424, 134);
        lblGrossWeightUnit.Name = "lblGrossWeightUnit";
        lblGrossWeightUnit.Size = new Size(11, 13);
        lblGrossWeightUnit.TabIndex = 32;
        lblGrossWeightUnit.Text = "kg";
        // 
        // lblVolume
        // 
        lblVolume.Location = new Point(17, 162);
        lblVolume.Name = "lblVolume";
        lblVolume.Size = new Size(44, 13);
        lblVolume.TabIndex = 33;
        lblVolume.Text = "Volumen:";
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
        // lblVolumeUnitCaption
        // 
        lblVolumeUnitCaption.Location = new Point(231, 162);
        lblVolumeUnitCaption.Name = "lblVolumeUnitCaption";
        lblVolumeUnitCaption.Size = new Size(13, 13);
        lblVolumeUnitCaption.TabIndex = 35;
        lblVolumeUnitCaption.Text = "m³";
        // 
        // lblWeightUnit
        // 
        lblWeightUnit.Location = new Point(18, 78);
        lblWeightUnit.Name = "lblWeightUnit";
        lblWeightUnit.Size = new Size(78, 13);
        lblWeightUnit.TabIndex = 36;
        lblWeightUnit.Text = "Unidad de peso:";
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
        // lblVolumeUnit
        // 
        lblVolumeUnit.Location = new Point(18, 106);
        lblVolumeUnit.Name = "lblVolumeUnit";
        lblVolumeUnit.Size = new Size(95, 13);
        lblVolumeUnit.TabIndex = 38;
        lblVolumeUnit.Text = "Unidad de volumen:";
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
        // tabGeneral
        // 
        tabGeneral.Appearance.PageClient.BackColor = Color.White;
        tabGeneral.Appearance.PageClient.Options.UseBackColor = true;
        tabGeneral.Controls.Add(sepGeneralColumnTwo);
        tabGeneral.Controls.Add(sepGeneralColumnOne);
        tabGeneral.Controls.Add(sepGeneralSummary);
        tabGeneral.Controls.Add(sepGeneralOperation);
        tabGeneral.Controls.Add(sepGeneralIdentification);
        tabGeneral.Controls.Add(kpiStockAvailable);
        tabGeneral.Controls.Add(kpiOnOrder);
        tabGeneral.Controls.Add(kpiCommitted);
        tabGeneral.Controls.Add(kpiPurchases);
        tabGeneral.Controls.Add(kpiSales);
        tabGeneral.Controls.Add(kpiSapStatus);
        tabGeneral.Controls.Add(kpiAverageCost);
        tabGeneral.Controls.Add(kpiMargin);
        tabGeneral.Controls.Add(kpiPurchaseCost);
        tabGeneral.Controls.Add(kpiSalesPrice);
        tabGeneral.Controls.Add(lblBlockedEcommerce);
        tabGeneral.Controls.Add(tglBlockedEcommerce);
        tabGeneral.Controls.Add(lblGeneralSummaryTitle);
        tabGeneral.Controls.Add(tglGeneralMobileItem);
        tabGeneral.Controls.Add(lblGeneralMobileItem);
        tabGeneral.Controls.Add(tglGeneralRequiresScale);
        tabGeneral.Controls.Add(tglGeneralAllowDiscount);
        tabGeneral.Controls.Add(tglGeneralPerishable);
        tabGeneral.Controls.Add(btnTraceabilityNone);
        tabGeneral.Controls.Add(btnTraceabilityBatch);
        tabGeneral.Controls.Add(btnTraceabilitySerial);
        tabGeneral.Controls.Add(lblGeneralOperationTitle);
        tabGeneral.Controls.Add(lblTraceabilityManagement);
        tabGeneral.Controls.Add(lblTraceabilityHintIcon);
        tabGeneral.Controls.Add(lblTraceabilityHint);
        tabGeneral.Controls.Add(lblPerishable);
        tabGeneral.Controls.Add(lblExpirationManaged);
        tabGeneral.Controls.Add(lblRequiresScale);
        tabGeneral.Controls.Add(lblAllowDiscount);
        tabGeneral.Controls.Add(tglGeneralExpirationManaged);
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
        tabGeneral.ImageOptions.SvgImageSize = new Size(22, 22);
        tabGeneral.Name = "tabGeneral";
        tabGeneral.Size = new Size(1406, 426);
        tabGeneral.Text = "General";
        // 
        // sepGeneralColumnTwo
        // 
        sepGeneralColumnTwo.Appearance.BackColor = Color.FromArgb((int)(byte)235, (int)(byte)238, (int)(byte)242);
        sepGeneralColumnTwo.Appearance.Options.UseBackColor = true;
        sepGeneralColumnTwo.AutoSizeMode = LabelAutoSizeMode.None;
        sepGeneralColumnTwo.Location = new Point(732, 15);
        sepGeneralColumnTwo.Name = "sepGeneralColumnTwo";
        sepGeneralColumnTwo.Size = new Size(1, 400);
        sepGeneralColumnTwo.TabIndex = 89;
        // 
        // sepGeneralColumnOne
        // 
        sepGeneralColumnOne.Appearance.BackColor = Color.FromArgb((int)(byte)235, (int)(byte)238, (int)(byte)242);
        sepGeneralColumnOne.Appearance.Options.UseBackColor = true;
        sepGeneralColumnOne.AutoSizeMode = LabelAutoSizeMode.None;
        sepGeneralColumnOne.Location = new Point(425, 15);
        sepGeneralColumnOne.Name = "sepGeneralColumnOne";
        sepGeneralColumnOne.Size = new Size(1, 400);
        sepGeneralColumnOne.TabIndex = 88;
        // 
        // sepGeneralSummary
        // 
        sepGeneralSummary.Appearance.BackColor = Color.FromArgb((int)(byte)223, (int)(byte)228, (int)(byte)234);
        sepGeneralSummary.Appearance.Options.UseBackColor = true;
        sepGeneralSummary.AutoSizeMode = LabelAutoSizeMode.None;
        sepGeneralSummary.Location = new Point(960, 24);
        sepGeneralSummary.Name = "sepGeneralSummary";
        sepGeneralSummary.Size = new Size(430, 1);
        sepGeneralSummary.TabIndex = 87;
        // 
        // sepGeneralOperation
        // 
        sepGeneralOperation.Appearance.BackColor = Color.FromArgb((int)(byte)223, (int)(byte)228, (int)(byte)234);
        sepGeneralOperation.Appearance.Options.UseBackColor = true;
        sepGeneralOperation.AutoSizeMode = LabelAutoSizeMode.None;
        sepGeneralOperation.Location = new Point(678, 24);
        sepGeneralOperation.Name = "sepGeneralOperation";
        sepGeneralOperation.Size = new Size(15, 1);
        sepGeneralOperation.TabIndex = 86;
        // 
        // sepGeneralIdentification
        // 
        sepGeneralIdentification.Appearance.BackColor = Color.FromArgb((int)(byte)223, (int)(byte)228, (int)(byte)234);
        sepGeneralIdentification.Appearance.Options.UseBackColor = true;
        sepGeneralIdentification.AutoSizeMode = LabelAutoSizeMode.None;
        sepGeneralIdentification.Location = new Point(235, 24);
        sepGeneralIdentification.Name = "sepGeneralIdentification";
        sepGeneralIdentification.Size = new Size(150, 1);
        sepGeneralIdentification.TabIndex = 85;
        // 
        // kpiStockAvailable
        // 
        kpiStockAvailable.AccessibleRole = AccessibleRole.StaticText;
        kpiStockAvailable.CornerRadius = 8;
        kpiStockAvailable.FallbackIconText = "ST";
        kpiStockAvailable.Location = new Point(774, 58);
        kpiStockAvailable.MinimumSize = new Size(140, 68);
        kpiStockAvailable.Name = "kpiStockAvailable";
        kpiStockAvailable.Size = new Size(200, 75);
        kpiStockAvailable.StatusText = "Disponible";
        kpiStockAvailable.TabIndex = 75;
        kpiStockAvailable.Title = "Stock disponible";
        kpiStockAvailable.UnitText = "UND";
        kpiStockAvailable.ValueText = "0.00";
        // 
        // kpiOnOrder
        // 
        kpiOnOrder.AccentColor = Color.FromArgb((int)(byte)37, (int)(byte)99, (int)(byte)235);
        kpiOnOrder.AccessibleRole = AccessibleRole.StaticText;
        kpiOnOrder.CornerRadius = 8;
        kpiOnOrder.FallbackIconText = "EP";
        kpiOnOrder.Location = new Point(980, 58);
        kpiOnOrder.MinimumSize = new Size(140, 68);
        kpiOnOrder.Name = "kpiOnOrder";
        kpiOnOrder.Size = new Size(200, 75);
        kpiOnOrder.StatusBackColor = Color.FromArgb((int)(byte)232, (int)(byte)241, (int)(byte)255);
        kpiOnOrder.StatusForeColor = Color.FromArgb((int)(byte)29, (int)(byte)78, (int)(byte)216);
        kpiOnOrder.StatusText = "Pendiente recepción";
        kpiOnOrder.TabIndex = 76;
        kpiOnOrder.Title = "En pedido";
        kpiOnOrder.UnitText = "UND";
        kpiOnOrder.ValueText = "0.00";
        // 
        // kpiCommitted
        // 
        kpiCommitted.AccentColor = Color.FromArgb((int)(byte)124, (int)(byte)58, (int)(byte)237);
        kpiCommitted.AccessibleRole = AccessibleRole.StaticText;
        kpiCommitted.CornerRadius = 8;
        kpiCommitted.FallbackIconText = "CO";
        kpiCommitted.Location = new Point(1186, 58);
        kpiCommitted.MinimumSize = new Size(140, 68);
        kpiCommitted.Name = "kpiCommitted";
        kpiCommitted.Size = new Size(200, 75);
        kpiCommitted.StatusBackColor = Color.FromArgb((int)(byte)245, (int)(byte)243, (int)(byte)255);
        kpiCommitted.StatusForeColor = Color.FromArgb((int)(byte)109, (int)(byte)40, (int)(byte)217);
        kpiCommitted.StatusText = "Reservado";
        kpiCommitted.TabIndex = 84;
        kpiCommitted.Title = "Comprometido";
        kpiCommitted.UnitText = "UND";
        kpiCommitted.ValueText = "0.00";
        // 
        // kpiPurchases
        // 
        kpiPurchases.AccentColor = Color.FromArgb((int)(byte)15, (int)(byte)118, (int)(byte)110);
        kpiPurchases.AccessibleRole = AccessibleRole.StaticText;
        kpiPurchases.CornerRadius = 8;
        kpiPurchases.FallbackIconText = "C";
        kpiPurchases.Location = new Point(774, 139);
        kpiPurchases.MinimumSize = new Size(140, 68);
        kpiPurchases.Name = "kpiPurchases";
        kpiPurchases.Size = new Size(200, 75);
        kpiPurchases.StatusForeColor = Color.FromArgb((int)(byte)15, (int)(byte)118, (int)(byte)110);
        kpiPurchases.StatusText = "Acumulado";
        kpiPurchases.TabIndex = 77;
        kpiPurchases.Title = "Compras";
        kpiPurchases.UnitText = "UND";
        kpiPurchases.ValueText = "3,450.00";
        // 
        // kpiSales
        // 
        kpiSales.AccentColor = Color.FromArgb((int)(byte)22, (int)(byte)163, (int)(byte)74);
        kpiSales.AccessibleRole = AccessibleRole.StaticText;
        kpiSales.CornerRadius = 8;
        kpiSales.FallbackIconText = "V";
        kpiSales.Location = new Point(980, 139);
        kpiSales.MinimumSize = new Size(140, 68);
        kpiSales.Name = "kpiSales";
        kpiSales.Size = new Size(200, 75);
        kpiSales.StatusBackColor = Color.FromArgb((int)(byte)236, (int)(byte)253, (int)(byte)245);
        kpiSales.StatusForeColor = Color.FromArgb((int)(byte)21, (int)(byte)128, (int)(byte)61);
        kpiSales.StatusText = "Últimos 12m";
        kpiSales.TabIndex = 78;
        kpiSales.Title = "Ventas";
        kpiSales.UnitText = "UND";
        kpiSales.ValueText = "4,250.00";
        // 
        // kpiSapStatus
        // 
        kpiSapStatus.AccentColor = Color.FromArgb((int)(byte)37, (int)(byte)99, (int)(byte)235);
        kpiSapStatus.AccessibleRole = AccessibleRole.StaticText;
        kpiSapStatus.CornerRadius = 8;
        kpiSapStatus.FallbackIconText = "SAP";
        kpiSapStatus.Location = new Point(774, 301);
        kpiSapStatus.MinimumSize = new Size(140, 68);
        kpiSapStatus.Name = "kpiSapStatus";
        kpiSapStatus.Size = new Size(200, 75);
        kpiSapStatus.StatusBackColor = Color.FromArgb((int)(byte)232, (int)(byte)241, (int)(byte)255);
        kpiSapStatus.StatusForeColor = Color.FromArgb((int)(byte)29, (int)(byte)78, (int)(byte)216);
        kpiSapStatus.StatusText = "Integración";
        kpiSapStatus.TabIndex = 79;
        kpiSapStatus.Title = "Estado SAP";
        kpiSapStatus.ValueText = "Sincronizado";
        // 
        // kpiAverageCost
        // 
        kpiAverageCost.AccentColor = Color.FromArgb((int)(byte)217, (int)(byte)119, (int)(byte)6);
        kpiAverageCost.AccessibleRole = AccessibleRole.StaticText;
        kpiAverageCost.CornerRadius = 8;
        kpiAverageCost.FallbackIconText = "CP";
        kpiAverageCost.Location = new Point(1186, 139);
        kpiAverageCost.MinimumSize = new Size(140, 68);
        kpiAverageCost.Name = "kpiAverageCost";
        kpiAverageCost.Size = new Size(200, 75);
        kpiAverageCost.StatusBackColor = Color.FromArgb((int)(byte)255, (int)(byte)247, (int)(byte)237);
        kpiAverageCost.StatusForeColor = Color.FromArgb((int)(byte)180, (int)(byte)83, (int)(byte)9);
        kpiAverageCost.StatusText = "Promedio ponderado";
        kpiAverageCost.TabIndex = 80;
        kpiAverageCost.Title = "Costo promedio";
        kpiAverageCost.UnitText = "USD";
        kpiAverageCost.ValueText = "0.00";
        // 
        // kpiMargin
        // 
        kpiMargin.AccentColor = Color.FromArgb((int)(byte)22, (int)(byte)163, (int)(byte)74);
        kpiMargin.AccessibleRole = AccessibleRole.StaticText;
        kpiMargin.CornerRadius = 8;
        kpiMargin.FallbackIconText = "%";
        kpiMargin.Location = new Point(1186, 220);
        kpiMargin.MinimumSize = new Size(140, 68);
        kpiMargin.Name = "kpiMargin";
        kpiMargin.Size = new Size(200, 75);
        kpiMargin.StatusBackColor = Color.FromArgb((int)(byte)236, (int)(byte)253, (int)(byte)245);
        kpiMargin.StatusForeColor = Color.FromArgb((int)(byte)21, (int)(byte)128, (int)(byte)61);
        kpiMargin.StatusText = "Margen bruto";
        kpiMargin.TabIndex = 81;
        kpiMargin.Title = "Margen";
        kpiMargin.UnitText = "%";
        kpiMargin.ValueText = "0.00";
        // 
        // kpiPurchaseCost
        // 
        kpiPurchaseCost.AccentColor = Color.FromArgb((int)(byte)190, (int)(byte)18, (int)(byte)60);
        kpiPurchaseCost.AccessibleRole = AccessibleRole.StaticText;
        kpiPurchaseCost.CornerRadius = 8;
        kpiPurchaseCost.FallbackIconText = "$";
        kpiPurchaseCost.Location = new Point(774, 220);
        kpiPurchaseCost.MinimumSize = new Size(140, 68);
        kpiPurchaseCost.Name = "kpiPurchaseCost";
        kpiPurchaseCost.Size = new Size(200, 75);
        kpiPurchaseCost.StatusBackColor = Color.FromArgb((int)(byte)255, (int)(byte)241, (int)(byte)242);
        kpiPurchaseCost.StatusForeColor = Color.FromArgb((int)(byte)159, (int)(byte)18, (int)(byte)57);
        kpiPurchaseCost.StatusText = "Última compra";
        kpiPurchaseCost.TabIndex = 82;
        kpiPurchaseCost.Title = "Costo compra";
        kpiPurchaseCost.UnitText = "USD";
        kpiPurchaseCost.ValueText = "0.00";
        // 
        // kpiSalesPrice
        // 
        kpiSalesPrice.AccentColor = Color.FromArgb((int)(byte)37, (int)(byte)99, (int)(byte)235);
        kpiSalesPrice.AccessibleRole = AccessibleRole.StaticText;
        kpiSalesPrice.CornerRadius = 8;
        kpiSalesPrice.FallbackIconText = "PV";
        kpiSalesPrice.Location = new Point(980, 220);
        kpiSalesPrice.MinimumSize = new Size(140, 68);
        kpiSalesPrice.Name = "kpiSalesPrice";
        kpiSalesPrice.Size = new Size(200, 75);
        kpiSalesPrice.StatusBackColor = Color.FromArgb((int)(byte)232, (int)(byte)241, (int)(byte)255);
        kpiSalesPrice.StatusForeColor = Color.FromArgb((int)(byte)29, (int)(byte)78, (int)(byte)216);
        kpiSalesPrice.StatusText = "Precio principal";
        kpiSalesPrice.TabIndex = 83;
        kpiSalesPrice.Title = "Precio venta";
        kpiSalesPrice.UnitText = "USD";
        kpiSalesPrice.ValueText = "0.00";
        // 
        // lblBlockedEcommerce
        // 
        lblBlockedEcommerce.Appearance.Font = new Font("Segoe UI", 9F);
        lblBlockedEcommerce.Appearance.Options.UseFont = true;
        lblBlockedEcommerce.Location = new Point(468, 258);
        lblBlockedEcommerce.Name = "lblBlockedEcommerce";
        lblBlockedEcommerce.Size = new Size(98, 15);
        lblBlockedEcommerce.TabIndex = 81;
        lblBlockedEcommerce.Text = "Ítem e-commerce:";
        // 
        // tglBlockedEcommerce
        // 
        tglBlockedEcommerce.Location = new Point(621, 258);
        tglBlockedEcommerce.Name = "tglBlockedEcommerce";
        tglBlockedEcommerce.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglBlockedEcommerce.Properties.Appearance.Options.UseFont = true;
        tglBlockedEcommerce.Properties.OffText = "No";
        tglBlockedEcommerce.Properties.OnText = "Sí";
        tglBlockedEcommerce.Size = new Size(86, 20);
        tglBlockedEcommerce.TabIndex = 82;
        // 
        // lblGeneralSummaryTitle
        // 
        lblGeneralSummaryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneralSummaryTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblGeneralSummaryTitle.Appearance.Options.UseFont = true;
        lblGeneralSummaryTitle.Appearance.Options.UseForeColor = true;
        lblGeneralSummaryTitle.Location = new Point(780, 15);
        lblGeneralSummaryTitle.Name = "lblGeneralSummaryTitle";
        lblGeneralSummaryTitle.Size = new Size(159, 20);
        lblGeneralSummaryTitle.TabIndex = 74;
        lblGeneralSummaryTitle.Text = "3. Resumen del artículo";
        // 
        // tglGeneralMobileItem
        // 
        tglGeneralMobileItem.EditValue = true;
        tglGeneralMobileItem.Location = new Point(621, 230);
        tglGeneralMobileItem.Name = "tglGeneralMobileItem";
        tglGeneralMobileItem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralMobileItem.Properties.Appearance.Options.UseFont = true;
        tglGeneralMobileItem.Properties.OffText = "No";
        tglGeneralMobileItem.Properties.OnText = "Sí";
        tglGeneralMobileItem.Size = new Size(86, 20);
        tglGeneralMobileItem.TabIndex = 73;
        // 
        // lblGeneralMobileItem
        // 
        lblGeneralMobileItem.Appearance.Font = new Font("Segoe UI", 9F);
        lblGeneralMobileItem.Appearance.Options.UseFont = true;
        lblGeneralMobileItem.Location = new Point(468, 230);
        lblGeneralMobileItem.Name = "lblGeneralMobileItem";
        lblGeneralMobileItem.Size = new Size(86, 15);
        lblGeneralMobileItem.TabIndex = 72;
        lblGeneralMobileItem.Text = "Ítem para móvil:";
        // 
        // tglGeneralRequiresScale
        // 
        tglGeneralRequiresScale.EditValue = true;
        tglGeneralRequiresScale.Location = new Point(621, 174);
        tglGeneralRequiresScale.Name = "tglGeneralRequiresScale";
        tglGeneralRequiresScale.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralRequiresScale.Properties.Appearance.Options.UseFont = true;
        tglGeneralRequiresScale.Properties.OffText = "No";
        tglGeneralRequiresScale.Properties.OnText = "Sí";
        tglGeneralRequiresScale.Size = new Size(86, 20);
        tglGeneralRequiresScale.TabIndex = 71;
        // 
        // tglGeneralAllowDiscount
        // 
        tglGeneralAllowDiscount.EditValue = true;
        tglGeneralAllowDiscount.Location = new Point(621, 202);
        tglGeneralAllowDiscount.Name = "tglGeneralAllowDiscount";
        tglGeneralAllowDiscount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralAllowDiscount.Properties.Appearance.Options.UseFont = true;
        tglGeneralAllowDiscount.Properties.OffText = "No";
        tglGeneralAllowDiscount.Properties.OnText = "Sí";
        tglGeneralAllowDiscount.Size = new Size(86, 20);
        tglGeneralAllowDiscount.TabIndex = 70;
        // 
        // tglGeneralPerishable
        // 
        tglGeneralPerishable.EditValue = true;
        tglGeneralPerishable.Location = new Point(621, 118);
        tglGeneralPerishable.Name = "tglGeneralPerishable";
        tglGeneralPerishable.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralPerishable.Properties.Appearance.Options.UseFont = true;
        tglGeneralPerishable.Properties.OffText = "No";
        tglGeneralPerishable.Properties.OnText = "Sí";
        tglGeneralPerishable.Size = new Size(86, 20);
        tglGeneralPerishable.TabIndex = 69;
        // 
        // btnTraceabilityNone
        // 
        btnTraceabilityNone.Appearance.BackColor = Color.White;
        btnTraceabilityNone.Appearance.BorderColor = Color.FromArgb((int)(byte)203, (int)(byte)213, (int)(byte)225);
        btnTraceabilityNone.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnTraceabilityNone.Appearance.ForeColor = Color.FromArgb((int)(byte)51, (int)(byte)65, (int)(byte)85);
        btnTraceabilityNone.Appearance.Options.UseBackColor = true;
        btnTraceabilityNone.Appearance.Options.UseBorderColor = true;
        btnTraceabilityNone.Appearance.Options.UseFont = true;
        btnTraceabilityNone.Appearance.Options.UseForeColor = true;
        btnTraceabilityNone.ButtonStyle = BorderStyles.Simple;
        btnTraceabilityNone.Location = new Point(468, 78);
        btnTraceabilityNone.Name = "btnTraceabilityNone";
        btnTraceabilityNone.Size = new Size(80, 28);
        btnTraceabilityNone.TabIndex = 67;
        btnTraceabilityNone.Text = "Ninguna";
        btnTraceabilityNone.Click += (this.TraceabilityNoneClick);
        // 
        // btnTraceabilityBatch
        // 
        btnTraceabilityBatch.Appearance.BackColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        btnTraceabilityBatch.Appearance.BorderColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        btnTraceabilityBatch.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnTraceabilityBatch.Appearance.ForeColor = Color.White;
        btnTraceabilityBatch.Appearance.Options.UseBackColor = true;
        btnTraceabilityBatch.Appearance.Options.UseBorderColor = true;
        btnTraceabilityBatch.Appearance.Options.UseFont = true;
        btnTraceabilityBatch.Appearance.Options.UseForeColor = true;
        btnTraceabilityBatch.ButtonStyle = BorderStyles.Simple;
        btnTraceabilityBatch.Location = new Point(547, 78);
        btnTraceabilityBatch.Name = "btnTraceabilityBatch";
        btnTraceabilityBatch.Size = new Size(80, 28);
        btnTraceabilityBatch.TabIndex = 68;
        btnTraceabilityBatch.Text = "Lote";
        btnTraceabilityBatch.Click += (this.TraceabilityBatchClick);
        // 
        // btnTraceabilitySerial
        // 
        btnTraceabilitySerial.Appearance.BackColor = Color.White;
        btnTraceabilitySerial.Appearance.BorderColor = Color.FromArgb((int)(byte)203, (int)(byte)213, (int)(byte)225);
        btnTraceabilitySerial.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnTraceabilitySerial.Appearance.ForeColor = Color.FromArgb((int)(byte)51, (int)(byte)65, (int)(byte)85);
        btnTraceabilitySerial.Appearance.Options.UseBackColor = true;
        btnTraceabilitySerial.Appearance.Options.UseBorderColor = true;
        btnTraceabilitySerial.Appearance.Options.UseFont = true;
        btnTraceabilitySerial.Appearance.Options.UseForeColor = true;
        btnTraceabilitySerial.ButtonStyle = BorderStyles.Simple;
        btnTraceabilitySerial.Location = new Point(626, 78);
        btnTraceabilitySerial.Name = "btnTraceabilitySerial";
        btnTraceabilitySerial.Size = new Size(80, 28);
        btnTraceabilitySerial.TabIndex = 69;
        btnTraceabilitySerial.Text = "Serie";
        btnTraceabilitySerial.Click += (this.TraceabilitySerialClick);
        // 
        // lblGeneralOperationTitle
        // 
        lblGeneralOperationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneralOperationTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblGeneralOperationTitle.Appearance.Options.UseFont = true;
        lblGeneralOperationTitle.Appearance.Options.UseForeColor = true;
        lblGeneralOperationTitle.Location = new Point(468, 15);
        lblGeneralOperationTitle.Name = "lblGeneralOperationTitle";
        lblGeneralOperationTitle.Size = new Size(185, 20);
        lblGeneralOperationTitle.TabIndex = 46;
        lblGeneralOperationTitle.Text = "2. Clasificación y operación";
        // 
        // lblTraceabilityManagement
        // 
        lblTraceabilityManagement.Appearance.Font = new Font("Segoe UI", 9F);
        lblTraceabilityManagement.Appearance.Options.UseFont = true;
        lblTraceabilityManagement.Location = new Point(468, 54);
        lblTraceabilityManagement.Name = "lblTraceabilityManagement";
        lblTraceabilityManagement.Size = new Size(123, 15);
        lblTraceabilityManagement.TabIndex = 53;
        lblTraceabilityManagement.Text = "Gestión de trazabilidad:";
        // 
        // lblTraceabilityHintIcon
        // 
        lblTraceabilityHintIcon.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblTraceabilityHintIcon.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblTraceabilityHintIcon.Appearance.Options.UseFont = true;
        lblTraceabilityHintIcon.Appearance.Options.UseForeColor = true;
        lblTraceabilityHintIcon.Location = new Point(468, 300);
        lblTraceabilityHintIcon.Name = "lblTraceabilityHintIcon";
        lblTraceabilityHintIcon.Size = new Size(16, 17);
        lblTraceabilityHintIcon.TabIndex = 88;
        lblTraceabilityHintIcon.Text = "↗";
        // 
        // lblTraceabilityHint
        // 
        lblTraceabilityHint.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblTraceabilityHint.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)142, (int)(byte)118);
        lblTraceabilityHint.Appearance.Options.UseFont = true;
        lblTraceabilityHint.Appearance.Options.UseForeColor = true;
        lblTraceabilityHint.Location = new Point(486, 301);
        lblTraceabilityHint.Name = "lblTraceabilityHint";
        lblTraceabilityHint.Size = new Size(208, 13);
        lblTraceabilityHint.TabIndex = 89;
        lblTraceabilityHint.Text = "La selección se heredará en Trazabilidad.";
        // 
        // lblPerishable
        // 
        lblPerishable.Appearance.Font = new Font("Segoe UI", 9F);
        lblPerishable.Appearance.Options.UseFont = true;
        lblPerishable.Location = new Point(468, 118);
        lblPerishable.Name = "lblPerishable";
        lblPerishable.Size = new Size(51, 15);
        lblPerishable.TabIndex = 57;
        lblPerishable.Text = "Perecible:";
        // 
        // lblExpirationManaged
        // 
        lblExpirationManaged.Appearance.Font = new Font("Segoe UI", 9F);
        lblExpirationManaged.Appearance.Options.UseFont = true;
        lblExpirationManaged.Location = new Point(468, 146);
        lblExpirationManaged.Name = "lblExpirationManaged";
        lblExpirationManaged.Size = new Size(111, 15);
        lblExpirationManaged.TabIndex = 59;
        lblExpirationManaged.Text = "Maneja vencimiento:";
        // 
        // lblRequiresScale
        // 
        lblRequiresScale.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiresScale.Appearance.Options.UseFont = true;
        lblRequiresScale.Location = new Point(468, 174);
        lblRequiresScale.Name = "lblRequiresScale";
        lblRequiresScale.Size = new Size(92, 15);
        lblRequiresScale.TabIndex = 61;
        lblRequiresScale.Text = "Requiere balanza:";
        // 
        // lblAllowDiscount
        // 
        lblAllowDiscount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowDiscount.Appearance.Options.UseFont = true;
        lblAllowDiscount.Location = new Point(468, 202);
        lblAllowDiscount.Name = "lblAllowDiscount";
        lblAllowDiscount.Size = new Size(102, 15);
        lblAllowDiscount.TabIndex = 63;
        lblAllowDiscount.Text = "Permite descuento:";
        // 
        // tglGeneralExpirationManaged
        // 
        tglGeneralExpirationManaged.EditValue = true;
        tglGeneralExpirationManaged.Location = new Point(621, 146);
        tglGeneralExpirationManaged.Name = "tglGeneralExpirationManaged";
        tglGeneralExpirationManaged.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglGeneralExpirationManaged.Properties.Appearance.Options.UseFont = true;
        tglGeneralExpirationManaged.Properties.OffText = "No";
        tglGeneralExpirationManaged.Properties.OnText = "Sí";
        tglGeneralExpirationManaged.Size = new Size(86, 20);
        tglGeneralExpirationManaged.TabIndex = 64;
        // 
        // lblGeneralIdentificationTitle
        // 
        lblGeneralIdentificationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblGeneralIdentificationTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblGeneralIdentificationTitle.Appearance.Options.UseFont = true;
        lblGeneralIdentificationTitle.Appearance.Options.UseForeColor = true;
        lblGeneralIdentificationTitle.Location = new Point(18, 15);
        lblGeneralIdentificationTitle.Name = "lblGeneralIdentificationTitle";
        lblGeneralIdentificationTitle.Size = new Size(187, 20);
        lblGeneralIdentificationTitle.TabIndex = 23;
        lblGeneralIdentificationTitle.Text = "1. Identificación del artículo";
        // 
        // lblAlternateCode
        // 
        lblAlternateCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblAlternateCode.Appearance.Options.UseFont = true;
        lblAlternateCode.Location = new Point(19, 62);
        lblAlternateCode.Name = "lblAlternateCode";
        lblAlternateCode.Size = new Size(82, 15);
        lblAlternateCode.TabIndex = 24;
        lblAlternateCode.Text = "Código alterno:";
        // 
        // txtAlternateCode
        // 
        txtAlternateCode.EditValue = "ARZ001-PREM";
        txtAlternateCode.Location = new Point(120, 59);
        txtAlternateCode.Name = "txtAlternateCode";
        txtAlternateCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAlternateCode.Properties.Appearance.Options.UseFont = true;
        txtAlternateCode.Size = new Size(267, 22);
        txtAlternateCode.TabIndex = 25;
        // 
        // lblSupplierSku
        // 
        lblSupplierSku.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierSku.Appearance.Options.UseFont = true;
        lblSupplierSku.Location = new Point(19, 90);
        lblSupplierSku.Name = "lblSupplierSku";
        lblSupplierSku.Size = new Size(81, 15);
        lblSupplierSku.TabIndex = 26;
        lblSupplierSku.Text = "SKU proveedor:";
        // 
        // slueSupplierSku
        // 
        slueSupplierSku.Location = new Point(120, 87);
        slueSupplierSku.Name = "slueSupplierSku";
        slueSupplierSku.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueSupplierSku.Properties.Appearance.Options.UseFont = true;
        slueSupplierSku.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        slueSupplierSku.Properties.NullText = "";
        slueSupplierSku.Properties.PopupView = gvSupplierSku;
        slueSupplierSku.Size = new Size(267, 22);
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
        // lblLongDescription
        // 
        lblLongDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblLongDescription.Appearance.Options.UseFont = true;
        lblLongDescription.Location = new Point(19, 118);
        lblLongDescription.Name = "lblLongDescription";
        lblLongDescription.Size = new Size(94, 15);
        lblLongDescription.TabIndex = 28;
        lblLongDescription.Text = "Descripción larga:";
        // 
        // memLongDescription
        // 
        memLongDescription.EditValue = "Arroz blanco de grano largo, seleccionado especialmente por su calidad y consistencia.\r\nIdeal para consumo diario.\r\nPresentación de 1 kilogramo.";
        memLongDescription.Location = new Point(120, 115);
        memLongDescription.Name = "memLongDescription";
        memLongDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memLongDescription.Properties.Appearance.Options.UseFont = true;
        memLongDescription.Size = new Size(267, 107);
        memLongDescription.TabIndex = 29;
        // 
        // lblProductType
        // 
        lblProductType.Appearance.Font = new Font("Segoe UI", 9F);
        lblProductType.Appearance.Options.UseFont = true;
        lblProductType.Location = new Point(19, 231);
        lblProductType.Name = "lblProductType";
        lblProductType.Size = new Size(95, 15);
        lblProductType.TabIndex = 30;
        lblProductType.Text = "Tipo de producto:";
        // 
        // lueProductType
        // 
        lueProductType.Location = new Point(120, 228);
        lueProductType.Name = "lueProductType";
        lueProductType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueProductType.Properties.Appearance.Options.UseFont = true;
        lueProductType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueProductType.Properties.NullText = "";
        lueProductType.Size = new Size(267, 22);
        lueProductType.TabIndex = 31;
        // 
        // lblOrigin
        // 
        lblOrigin.Appearance.Font = new Font("Segoe UI", 9F);
        lblOrigin.Appearance.Options.UseFont = true;
        lblOrigin.Location = new Point(19, 259);
        lblOrigin.Name = "lblOrigin";
        lblOrigin.Size = new Size(39, 15);
        lblOrigin.TabIndex = 32;
        lblOrigin.Text = "Origen:";
        // 
        // lueOrigin
        // 
        lueOrigin.Location = new Point(120, 256);
        lueOrigin.Name = "lueOrigin";
        lueOrigin.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueOrigin.Properties.Appearance.Options.UseFont = true;
        lueOrigin.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueOrigin.Properties.NullText = "";
        lueOrigin.Size = new Size(267, 22);
        lueOrigin.TabIndex = 33;
        // 
        // lblLine
        // 
        lblLine.Appearance.Font = new Font("Segoe UI", 9F);
        lblLine.Appearance.Options.UseFont = true;
        lblLine.Location = new Point(19, 287);
        lblLine.Name = "lblLine";
        lblLine.Size = new Size(31, 15);
        lblLine.TabIndex = 34;
        lblLine.Text = "Línea:";
        // 
        // lueLine
        // 
        lueLine.Location = new Point(120, 284);
        lueLine.Name = "lueLine";
        lueLine.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueLine.Properties.Appearance.Options.UseFont = true;
        lueLine.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueLine.Properties.NullText = "";
        lueLine.Size = new Size(267, 22);
        lueLine.TabIndex = 35;
        // 
        // lblSubGroup
        // 
        lblSubGroup.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubGroup.Appearance.Options.UseFont = true;
        lblSubGroup.Location = new Point(19, 315);
        lblSubGroup.Name = "lblSubGroup";
        lblSubGroup.Size = new Size(55, 15);
        lblSubGroup.TabIndex = 36;
        lblSubGroup.Text = "Subgrupo:";
        // 
        // lueSubGroup
        // 
        lueSubGroup.Location = new Point(120, 312);
        lueSubGroup.Name = "lueSubGroup";
        lueSubGroup.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSubGroup.Properties.Appearance.Options.UseFont = true;
        lueSubGroup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        lueSubGroup.Properties.NullText = "";
        lueSubGroup.Size = new Size(267, 22);
        lueSubGroup.TabIndex = 37;
        // 
        // lblModel
        // 
        lblModel.Appearance.Font = new Font("Segoe UI", 9F);
        lblModel.Appearance.Options.UseFont = true;
        lblModel.Location = new Point(18, 343);
        lblModel.Name = "lblModel";
        lblModel.Size = new Size(44, 15);
        lblModel.TabIndex = 38;
        lblModel.Text = "Modelo:";
        // 
        // txtModel
        // 
        txtModel.EditValue = "N/A";
        txtModel.Location = new Point(120, 340);
        txtModel.Name = "txtModel";
        txtModel.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtModel.Properties.Appearance.Options.UseFont = true;
        txtModel.Size = new Size(267, 22);
        txtModel.TabIndex = 39;
        // 
        // lblReference
        // 
        lblReference.Appearance.Font = new Font("Segoe UI", 9F);
        lblReference.Appearance.Options.UseFont = true;
        lblReference.Location = new Point(18, 375);
        lblReference.Name = "lblReference";
        lblReference.Size = new Size(58, 15);
        lblReference.TabIndex = 40;
        lblReference.Text = "Referencia:";
        // 
        // txtReference
        // 
        txtReference.EditValue = "N/A";
        txtReference.Location = new Point(120, 368);
        txtReference.Name = "txtReference";
        txtReference.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtReference.Properties.Appearance.Options.UseFont = true;
        txtReference.Size = new Size(267, 22);
        txtReference.TabIndex = 41;
        // 
        // lblAffectsInventory
        // 
        lblAffectsInventory.Appearance.Font = new Font("Segoe UI", 9F);
        lblAffectsInventory.Appearance.Options.UseFont = true;
        lblAffectsInventory.Location = new Point(1065, 97);
        lblAffectsInventory.Name = "lblAffectsInventory";
        lblAffectsInventory.Size = new Size(99, 15);
        lblAffectsInventory.TabIndex = 65;
        lblAffectsInventory.Text = "Ítem de inventario:";
        // 
        // tglAffectsInventory
        // 
        tglAffectsInventory.EditValue = true;
        tglAffectsInventory.Location = new Point(1199, 95);
        tglAffectsInventory.Name = "tglAffectsInventory";
        tglAffectsInventory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglAffectsInventory.Properties.Appearance.Options.UseFont = true;
        tglAffectsInventory.Properties.OffText = "No";
        tglAffectsInventory.Properties.OnText = "Sí";
        tglAffectsInventory.Size = new Size(86, 20);
        tglAffectsInventory.TabIndex = 66;
        // 
        // lblSalesActive
        // 
        lblSalesActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesActive.Appearance.Options.UseFont = true;
        lblSalesActive.Location = new Point(1065, 41);
        lblSalesActive.Name = "lblSalesActive";
        lblSalesActive.Size = new Size(75, 15);
        lblSalesActive.TabIndex = 42;
        lblSalesActive.Text = "Ítem de venta:";
        // 
        // tglSalesActive
        // 
        tglSalesActive.EditValue = true;
        tglSalesActive.Location = new Point(1199, 36);
        tglSalesActive.Name = "tglSalesActive";
        tglSalesActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglSalesActive.Properties.Appearance.Options.UseFont = true;
        tglSalesActive.Properties.OffText = "No";
        tglSalesActive.Properties.OnText = "Sí";
        tglSalesActive.Size = new Size(86, 20);
        tglSalesActive.TabIndex = 43;
        // 
        // lblPurchaseActive
        // 
        lblPurchaseActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseActive.Appearance.Options.UseFont = true;
        lblPurchaseActive.Location = new Point(1065, 69);
        lblPurchaseActive.Name = "lblPurchaseActive";
        lblPurchaseActive.Size = new Size(87, 15);
        lblPurchaseActive.TabIndex = 44;
        lblPurchaseActive.Text = "Ítem de compra:";
        // 
        // tglPurchaseActive
        // 
        tglPurchaseActive.EditValue = true;
        tglPurchaseActive.Location = new Point(1199, 66);
        tglPurchaseActive.Name = "tglPurchaseActive";
        tglPurchaseActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglPurchaseActive.Properties.Appearance.Options.UseFont = true;
        tglPurchaseActive.Properties.OffText = "No";
        tglPurchaseActive.Properties.OnText = "Sí";
        tglPurchaseActive.Size = new Size(86, 20);
        tglPurchaseActive.TabIndex = 45;
        // 
        // tabCommercial
        // 
        tabCommercial.Controls.Add(tabCommercialSections);
        tabCommercial.ImageOptions.SvgImageSize = new Size(22, 22);
        tabCommercial.Name = "tabCommercial";
        tabCommercial.Size = new Size(1406, 426);
        tabCommercial.Text = "Comercial";
        // 
        // tabCommercialSections
        // 
        tabCommercialSections.Appearance.Font = new Font("Segoe UI", 9F);
        tabCommercialSections.Appearance.Options.UseFont = true;
        tabCommercialSections.AppearancePage.Header.Font = new Font("Segoe UI", 9F);
        tabCommercialSections.AppearancePage.Header.Options.UseFont = true;
        tabCommercialSections.AppearancePage.HeaderActive.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        tabCommercialSections.AppearancePage.HeaderActive.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        tabCommercialSections.AppearancePage.HeaderActive.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        tabCommercialSections.AppearancePage.HeaderActive.Options.UseBackColor = true;
        tabCommercialSections.AppearancePage.HeaderActive.Options.UseFont = true;
        tabCommercialSections.AppearancePage.HeaderActive.Options.UseForeColor = true;
        tabCommercialSections.AppearancePage.PageClient.BackColor = Color.White;
        tabCommercialSections.AppearancePage.PageClient.Options.UseBackColor = true;
        tabCommercialSections.Dock = DockStyle.Fill;
        tabCommercialSections.HeaderAutoFill = DefaultBoolean.False;
        tabCommercialSections.Location = new Point(0, 0);
        tabCommercialSections.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        tabCommercialSections.LookAndFeel.UseDefaultLookAndFeel = false;
        tabCommercialSections.Name = "tabCommercialSections";
        tabCommercialSections.SelectedTabPage = tabPurchases;
        tabCommercialSections.Size = new Size(1406, 426);
        tabCommercialSections.TabIndex = 0;
        tabCommercialSections.TabPages.AddRange(new XtraTabPage[] { tabPurchases, tabSales });
        tabCommercialSections.TabPageWidth = 220;
        tabCommercialSections.SelectedPageChanged += (this.tabCommercialSections_SelectedPageChanged);
        tabCommercialSections.CustomDrawTabHeader += (this.tabCommercialSections_CustomDrawTabHeader);
        tabCommercialSections.HandleCreated += (this.tabCommercialSections_HandleCreated);
        // 
        // tabPurchases
        // 
        tabPurchases.Appearance.PageClient.BackColor = Color.White;
        tabPurchases.Appearance.PageClient.Options.UseBackColor = true;
        tabPurchases.Controls.Add(labelControl4);
        tabPurchases.Controls.Add(labelControl3);
        tabPurchases.Controls.Add(labelControl2);
        tabPurchases.Controls.Add(labelControl1);
        tabPurchases.Controls.Add(sepPurchasesColumnOne);
        tabPurchases.Controls.Add(sepPurchasesColumnTwo);
        tabPurchases.Controls.Add(sepPurchasesConfiguration);
        tabPurchases.Controls.Add(sepPurchasesConditions);
        tabPurchases.Controls.Add(sepPurchasesIndicators);
        tabPurchases.Controls.Add(sepPurchasesHistory);
        tabPurchases.Controls.Add(lblPurchasesConditionsTitle);
        tabPurchases.Controls.Add(lblPurchasesIndicatorsTitle);
        tabPurchases.Controls.Add(lblPurchaseUnit);
        tabPurchases.Controls.Add(luePurchaseUnit);
        tabPurchases.Controls.Add(kpiPurchaseCompliance);
        tabPurchases.Controls.Add(lblPurchasesHistoryTitle);
        tabPurchases.Controls.Add(grdPurchaseHistory);
        tabPurchases.Controls.Add(lblPurchasesConfigurationTitle);
        tabPurchases.Controls.Add(kpiPurchaseLast);
        tabPurchases.Controls.Add(lblPurchaseApprovalRequired);
        tabPurchases.Controls.Add(kpiPurchaseAverage);
        tabPurchases.Controls.Add(tglPurchaseApprovalRequired);
        tabPurchases.Controls.Add(kpiPurchaseLeadTime);
        tabPurchases.Controls.Add(lblSupplierBackorderAllowed);
        tabPurchases.Controls.Add(tglSupplierBackorderAllowed);
        tabPurchases.Controls.Add(memReceivingNote);
        tabPurchases.Controls.Add(lblPurchaseOnDemand);
        tabPurchases.Controls.Add(lblReceivingNote);
        tabPurchases.Controls.Add(tglPurchaseOnDemand);
        tabPurchases.Controls.Add(memPurchasePolicy);
        tabPurchases.Controls.Add(lblPurchasePolicy);
        tabPurchases.Controls.Add(lblMainPurchaseSupplier);
        tabPurchases.Controls.Add(slueMainPurchaseSupplier);
        tabPurchases.Controls.Add(lblPreferredPurchasePresentation);
        tabPurchases.Controls.Add(luePreferredPurchasePresentation);
        tabPurchases.Controls.Add(lblPreferredPurchaseCurrency);
        tabPurchases.Controls.Add(luePreferredPurchaseCurrency);
        tabPurchases.Controls.Add(lblPurchaseMinimumQuantity);
        tabPurchases.Controls.Add(spnPurchaseMinimumQuantity);
        tabPurchases.Controls.Add(lblPurchaseMultiple);
        tabPurchases.Controls.Add(spnPurchaseMultiple);
        tabPurchases.Controls.Add(lblPurchaseDeliveryDays);
        tabPurchases.Controls.Add(spnPurchaseDeliveryDays);
        tabPurchases.Controls.Add(btnViewPurchaseDocument);
        tabPurchases.Controls.Add(btnRefreshPurchases);
        tabPurchases.ImageOptions.SvgImageSize = new Size(20, 20);
        tabPurchases.Name = "tabPurchases";
        tabPurchases.Size = new Size(1402, 398);
        tabPurchases.Text = "Compras";
        // 
        // labelControl4
        // 
        labelControl4.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        labelControl4.Appearance.Options.UseBackColor = true;
        labelControl4.AutoSizeMode = LabelAutoSizeMode.None;
        labelControl4.Location = new Point(286, 262);
        labelControl4.Name = "labelControl4";
        labelControl4.Size = new Size(790, 1);
        labelControl4.TabIndex = 50;
        // 
        // labelControl3
        // 
        labelControl3.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        labelControl3.Appearance.Options.UseBackColor = true;
        labelControl3.AutoSizeMode = LabelAutoSizeMode.None;
        labelControl3.Location = new Point(1141, 22);
        labelControl3.Name = "labelControl3";
        labelControl3.Size = new Size(220, 1);
        labelControl3.TabIndex = 49;
        // 
        // labelControl2
        // 
        labelControl2.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        labelControl2.Appearance.Options.UseBackColor = true;
        labelControl2.AutoSizeMode = LabelAutoSizeMode.None;
        labelControl2.Location = new Point(655, 22);
        labelControl2.Name = "labelControl2";
        labelControl2.Size = new Size(250, 1);
        labelControl2.TabIndex = 48;
        // 
        // labelControl1
        // 
        labelControl1.Appearance.BackColor = Color.FromArgb((int)(byte)218, (int)(byte)223, (int)(byte)229);
        labelControl1.Appearance.Options.UseBackColor = true;
        labelControl1.AutoSizeMode = LabelAutoSizeMode.None;
        labelControl1.Location = new Point(218, 22);
        labelControl1.Name = "labelControl1";
        labelControl1.Size = new Size(190, 1);
        labelControl1.TabIndex = 47;
        // 
        // sepPurchasesColumnOne
        // 
        sepPurchasesColumnOne.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepPurchasesColumnOne.Appearance.Options.UseBackColor = true;
        sepPurchasesColumnOne.AutoSizeMode = LabelAutoSizeMode.None;
        sepPurchasesColumnOne.Location = new Point(440, 12);
        sepPurchasesColumnOne.Name = "sepPurchasesColumnOne";
        sepPurchasesColumnOne.Size = new Size(1, 224);
        sepPurchasesColumnOne.TabIndex = 21;
        // 
        // sepPurchasesColumnTwo
        // 
        sepPurchasesColumnTwo.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepPurchasesColumnTwo.Appearance.Options.UseBackColor = true;
        sepPurchasesColumnTwo.AutoSizeMode = LabelAutoSizeMode.None;
        sepPurchasesColumnTwo.Location = new Point(930, 12);
        sepPurchasesColumnTwo.Name = "sepPurchasesColumnTwo";
        sepPurchasesColumnTwo.Size = new Size(1, 224);
        sepPurchasesColumnTwo.TabIndex = 22;
        // 
        // sepPurchasesConfiguration
        // 
        sepPurchasesConfiguration.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepPurchasesConfiguration.Appearance.Options.UseBackColor = true;
        sepPurchasesConfiguration.Location = new Point(218, 22);
        sepPurchasesConfiguration.Name = "sepPurchasesConfiguration";
        sepPurchasesConfiguration.Size = new Size(0, 13);
        sepPurchasesConfiguration.TabIndex = 23;
        // 
        // sepPurchasesConditions
        // 
        sepPurchasesConditions.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepPurchasesConditions.Appearance.Options.UseBackColor = true;
        sepPurchasesConditions.Location = new Point(655, 22);
        sepPurchasesConditions.Name = "sepPurchasesConditions";
        sepPurchasesConditions.Size = new Size(0, 13);
        sepPurchasesConditions.TabIndex = 24;
        // 
        // sepPurchasesIndicators
        // 
        sepPurchasesIndicators.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        sepPurchasesIndicators.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepPurchasesIndicators.Appearance.Options.UseBackColor = true;
        sepPurchasesIndicators.Location = new Point(1167, 22);
        sepPurchasesIndicators.Name = "sepPurchasesIndicators";
        sepPurchasesIndicators.Size = new Size(0, 13);
        sepPurchasesIndicators.TabIndex = 25;
        // 
        // sepPurchasesHistory
        // 
        sepPurchasesHistory.Anchor = (AnchorStyles)((AnchorStyles.Top) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        sepPurchasesHistory.Appearance.BackColor = Color.FromArgb((int)(byte)221, (int)(byte)226, (int)(byte)240);
        sepPurchasesHistory.Appearance.Options.UseBackColor = true;
        sepPurchasesHistory.Location = new Point(310, 262);
        sepPurchasesHistory.Name = "sepPurchasesHistory";
        sepPurchasesHistory.Size = new Size(0, 13);
        sepPurchasesHistory.TabIndex = 26;
        // 
        // lblPurchasesConditionsTitle
        // 
        lblPurchasesConditionsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchasesConditionsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblPurchasesConditionsTitle.Appearance.Options.UseFont = true;
        lblPurchasesConditionsTitle.Appearance.Options.UseForeColor = true;
        lblPurchasesConditionsTitle.Location = new Point(463, 12);
        lblPurchasesConditionsTitle.Name = "lblPurchasesConditionsTitle";
        lblPurchasesConditionsTitle.Size = new Size(174, 20);
        lblPurchasesConditionsTitle.TabIndex = 27;
        lblPurchasesConditionsTitle.Text = "2. Condiciones operativas";
        // 
        // lblPurchasesIndicatorsTitle
        // 
        lblPurchasesIndicatorsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchasesIndicatorsTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblPurchasesIndicatorsTitle.Appearance.Options.UseFont = true;
        lblPurchasesIndicatorsTitle.Appearance.Options.UseForeColor = true;
        lblPurchasesIndicatorsTitle.Location = new Point(954, 12);
        lblPurchasesIndicatorsTitle.Name = "lblPurchasesIndicatorsTitle";
        lblPurchasesIndicatorsTitle.Size = new Size(172, 20);
        lblPurchasesIndicatorsTitle.TabIndex = 28;
        lblPurchasesIndicatorsTitle.Text = "3. Indicadores de compra";
        // 
        // lblPurchaseUnit
        // 
        lblPurchaseUnit.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseUnit.Appearance.Options.UseFont = true;
        lblPurchaseUnit.Location = new Point(18, 50);
        lblPurchaseUnit.Name = "lblPurchaseUnit";
        lblPurchaseUnit.Size = new Size(101, 15);
        lblPurchaseUnit.TabIndex = 19;
        lblPurchaseUnit.Text = "Unidad de compra:";
        // 
        // luePurchaseUnit
        // 
        luePurchaseUnit.Location = new Point(137, 47);
        luePurchaseUnit.Name = "luePurchaseUnit";
        luePurchaseUnit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseUnit.Properties.Appearance.Options.UseFont = true;
        luePurchaseUnit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseUnit.Properties.NullText = "UND - Unidad";
        luePurchaseUnit.Size = new Size(273, 22);
        luePurchaseUnit.TabIndex = 20;
        // 
        // kpiPurchaseCompliance
        // 
        kpiPurchaseCompliance.AccentColor = Color.FromArgb((int)(byte)22, (int)(byte)163, (int)(byte)74);
        kpiPurchaseCompliance.AccessibleRole = AccessibleRole.StaticText;
        kpiPurchaseCompliance.FallbackIconText = "%";
        kpiPurchaseCompliance.Location = new Point(1160, 127);
        kpiPurchaseCompliance.MinimumSize = new Size(140, 68);
        kpiPurchaseCompliance.Name = "kpiPurchaseCompliance";
        kpiPurchaseCompliance.Size = new Size(200, 75);
        kpiPurchaseCompliance.StatusBackColor = Color.FromArgb((int)(byte)240, (int)(byte)253, (int)(byte)244);
        kpiPurchaseCompliance.StatusForeColor = Color.FromArgb((int)(byte)21, (int)(byte)128, (int)(byte)61);
        kpiPurchaseCompliance.StatusText = "Últimos 12m";
        kpiPurchaseCompliance.TabIndex = 5;
        kpiPurchaseCompliance.Title = "Cumplimiento proveedor";
        kpiPurchaseCompliance.UnitText = "%";
        kpiPurchaseCompliance.ValueText = "96.5";
        // 
        // lblPurchasesHistoryTitle
        // 
        lblPurchasesHistoryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchasesHistoryTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblPurchasesHistoryTitle.Appearance.Options.UseFont = true;
        lblPurchasesHistoryTitle.Appearance.Options.UseForeColor = true;
        lblPurchasesHistoryTitle.Location = new Point(12, 252);
        lblPurchasesHistoryTitle.Name = "lblPurchasesHistoryTitle";
        lblPurchasesHistoryTitle.Size = new Size(252, 20);
        lblPurchasesHistoryTitle.TabIndex = 0;
        lblPurchasesHistoryTitle.Text = "4. Historial y desempeño de compras";
        // 
        // grdPurchaseHistory
        // 
        grdPurchaseHistory.Anchor = (AnchorStyles)(((AnchorStyles.Top) | (AnchorStyles.Bottom)) | (AnchorStyles.Left)) | (AnchorStyles.Right);
        grdPurchaseHistory.DataSource = purchaseHistoryTable;
        grdPurchaseHistory.Location = new Point(12, 282);
        grdPurchaseHistory.MainView = gvPurchaseHistory;
        grdPurchaseHistory.Name = "grdPurchaseHistory";
        grdPurchaseHistory.Size = new Size(1370, 100);
        grdPurchaseHistory.TabIndex = 1;
        grdPurchaseHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvPurchaseHistory, gvPurchaseHistoryAux, gridView1 });
        // 
        // gvPurchaseHistory
        // 
        gvPurchaseHistory.Appearance.FocusedRow.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        gvPurchaseHistory.Appearance.FocusedRow.ForeColor = Color.FromArgb((int)(byte)23, (int)(byte)32, (int)(byte)51);
        gvPurchaseHistory.Appearance.FocusedRow.Options.UseBackColor = true;
        gvPurchaseHistory.Appearance.FocusedRow.Options.UseForeColor = true;
        gvPurchaseHistory.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvPurchaseHistory.Appearance.HeaderPanel.Options.UseFont = true;
        gvPurchaseHistory.Appearance.HideSelectionRow.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        gvPurchaseHistory.Appearance.HideSelectionRow.ForeColor = Color.FromArgb((int)(byte)23, (int)(byte)32, (int)(byte)51);
        gvPurchaseHistory.Appearance.HideSelectionRow.Options.UseBackColor = true;
        gvPurchaseHistory.Appearance.HideSelectionRow.Options.UseForeColor = true;
        gvPurchaseHistory.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvPurchaseHistory.Appearance.Row.Options.UseFont = true;
        gvPurchaseHistory.Columns.AddRange(new GridColumn[] { colPurchaseHistoryDate, colPurchaseHistoryDocument, colPurchaseHistorySupplier, colPurchaseHistoryPresentation, colPurchaseHistoryQuantity, colPurchaseHistoryUnit, colPurchaseHistoryInventoryQty, colPurchaseHistoryUnitCost, colPurchaseHistoryCurrency, colPurchaseHistoryStatus });
        gvPurchaseHistory.GridControl = grdPurchaseHistory;
        gvPurchaseHistory.Name = "gvPurchaseHistory";
        gvPurchaseHistory.OptionsBehavior.Editable = false;
        gvPurchaseHistory.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvPurchaseHistory.OptionsView.ShowGroupPanel = false;
        gvPurchaseHistory.OptionsView.ShowIndicator = false;
        gvPurchaseHistory.RowHeight = 28;
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
        colPurchaseHistoryQuantity.DisplayFormat.FormatString = "N2";
        colPurchaseHistoryQuantity.DisplayFormat.FormatType = FormatType.Numeric;
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
        colPurchaseHistoryInventoryQty.DisplayFormat.FormatString = "N2";
        colPurchaseHistoryInventoryQty.DisplayFormat.FormatType = FormatType.Numeric;
        colPurchaseHistoryInventoryQty.FieldName = "CantidadInventario";
        colPurchaseHistoryInventoryQty.Name = "colPurchaseHistoryInventoryQty";
        colPurchaseHistoryInventoryQty.Visible = true;
        colPurchaseHistoryInventoryQty.VisibleIndex = 6;
        // 
        // colPurchaseHistoryUnitCost
        // 
        colPurchaseHistoryUnitCost.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colPurchaseHistoryUnitCost.Caption = "Costo";
        colPurchaseHistoryUnitCost.DisplayFormat.FormatString = "N2";
        colPurchaseHistoryUnitCost.DisplayFormat.FormatType = FormatType.Numeric;
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
        // gvPurchaseHistoryAux
        // 
        gvPurchaseHistoryAux.GridControl = grdPurchaseHistory;
        gvPurchaseHistoryAux.Name = "gvPurchaseHistoryAux";
        // 
        // gridView1
        // 
        gridView1.GridControl = grdPurchaseHistory;
        gridView1.Name = "gridView1";
        // 
        // lblPurchasesConfigurationTitle
        // 
        lblPurchasesConfigurationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchasesConfigurationTitle.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        lblPurchasesConfigurationTitle.Appearance.Options.UseFont = true;
        lblPurchasesConfigurationTitle.Appearance.Options.UseForeColor = true;
        lblPurchasesConfigurationTitle.Location = new Point(12, 12);
        lblPurchasesConfigurationTitle.Name = "lblPurchasesConfigurationTitle";
        lblPurchasesConfigurationTitle.Size = new Size(194, 20);
        lblPurchasesConfigurationTitle.TabIndex = 0;
        lblPurchasesConfigurationTitle.Text = "1. Configuración de compras";
        // 
        // kpiPurchaseLast
        // 
        kpiPurchaseLast.AccentColor = Color.FromArgb((int)(byte)225, (int)(byte)29, (int)(byte)72);
        kpiPurchaseLast.AccessibleRole = AccessibleRole.StaticText;
        kpiPurchaseLast.FallbackIconText = "$";
        kpiPurchaseLast.Location = new Point(954, 46);
        kpiPurchaseLast.MinimumSize = new Size(140, 68);
        kpiPurchaseLast.Name = "kpiPurchaseLast";
        kpiPurchaseLast.Size = new Size(200, 75);
        kpiPurchaseLast.StatusBackColor = Color.FromArgb((int)(byte)255, (int)(byte)241, (int)(byte)242);
        kpiPurchaseLast.StatusForeColor = Color.FromArgb((int)(byte)190, (int)(byte)18, (int)(byte)60);
        kpiPurchaseLast.StatusText = "Última compra";
        kpiPurchaseLast.TabIndex = 2;
        kpiPurchaseLast.Title = "Última compra";
        kpiPurchaseLast.UnitText = "USD";
        kpiPurchaseLast.ValueText = "18.20";
        // 
        // lblPurchaseApprovalRequired
        // 
        lblPurchaseApprovalRequired.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseApprovalRequired.Appearance.Options.UseFont = true;
        lblPurchaseApprovalRequired.Location = new Point(218, 177);
        lblPurchaseApprovalRequired.Name = "lblPurchaseApprovalRequired";
        lblPurchaseApprovalRequired.Size = new Size(112, 15);
        lblPurchaseApprovalRequired.TabIndex = 3;
        lblPurchaseApprovalRequired.Text = "Requiere aprobación:";
        // 
        // kpiPurchaseAverage
        // 
        kpiPurchaseAverage.AccentColor = Color.FromArgb((int)(byte)234, (int)(byte)88, (int)(byte)12);
        kpiPurchaseAverage.AccessibleRole = AccessibleRole.StaticText;
        kpiPurchaseAverage.FallbackIconText = "P";
        kpiPurchaseAverage.Location = new Point(1160, 46);
        kpiPurchaseAverage.MinimumSize = new Size(140, 68);
        kpiPurchaseAverage.Name = "kpiPurchaseAverage";
        kpiPurchaseAverage.Size = new Size(200, 75);
        kpiPurchaseAverage.StatusBackColor = Color.FromArgb((int)(byte)255, (int)(byte)247, (int)(byte)237);
        kpiPurchaseAverage.StatusForeColor = Color.FromArgb((int)(byte)194, (int)(byte)65, (int)(byte)12);
        kpiPurchaseAverage.StatusText = "Promedio ponderado";
        kpiPurchaseAverage.TabIndex = 3;
        kpiPurchaseAverage.Title = "Promedio 12m";
        kpiPurchaseAverage.UnitText = "USD";
        kpiPurchaseAverage.ValueText = "18.60";
        // 
        // tglPurchaseApprovalRequired
        // 
        tglPurchaseApprovalRequired.Location = new Point(336, 175);
        tglPurchaseApprovalRequired.Name = "tglPurchaseApprovalRequired";
        tglPurchaseApprovalRequired.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglPurchaseApprovalRequired.Properties.Appearance.Options.UseFont = true;
        tglPurchaseApprovalRequired.Properties.OffText = "No";
        tglPurchaseApprovalRequired.Properties.OnText = "Sí";
        tglPurchaseApprovalRequired.Size = new Size(86, 20);
        tglPurchaseApprovalRequired.TabIndex = 4;
        // 
        // kpiPurchaseLeadTime
        // 
        kpiPurchaseLeadTime.AccentColor = Color.FromArgb((int)(byte)37, (int)(byte)99, (int)(byte)235);
        kpiPurchaseLeadTime.AccessibleRole = AccessibleRole.StaticText;
        kpiPurchaseLeadTime.FallbackIconText = "T";
        kpiPurchaseLeadTime.Location = new Point(954, 127);
        kpiPurchaseLeadTime.MinimumSize = new Size(140, 68);
        kpiPurchaseLeadTime.Name = "kpiPurchaseLeadTime";
        kpiPurchaseLeadTime.Size = new Size(200, 75);
        kpiPurchaseLeadTime.StatusBackColor = Color.FromArgb((int)(byte)239, (int)(byte)246, (int)(byte)255);
        kpiPurchaseLeadTime.StatusForeColor = Color.FromArgb((int)(byte)29, (int)(byte)78, (int)(byte)216);
        kpiPurchaseLeadTime.StatusText = "Tiempo promedio";
        kpiPurchaseLeadTime.TabIndex = 4;
        kpiPurchaseLeadTime.Title = "Lead time prom.";
        kpiPurchaseLeadTime.UnitText = "días";
        kpiPurchaseLeadTime.ValueText = "5.2";
        // 
        // lblSupplierBackorderAllowed
        // 
        lblSupplierBackorderAllowed.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierBackorderAllowed.Appearance.Options.UseFont = true;
        lblSupplierBackorderAllowed.Location = new Point(18, 203);
        lblSupplierBackorderAllowed.Name = "lblSupplierBackorderAllowed";
        lblSupplierBackorderAllowed.Size = new Size(113, 15);
        lblSupplierBackorderAllowed.TabIndex = 5;
        lblSupplierBackorderAllowed.Text = "Backorder proveedor:";
        // 
        // tglSupplierBackorderAllowed
        // 
        tglSupplierBackorderAllowed.EditValue = true;
        tglSupplierBackorderAllowed.Location = new Point(137, 201);
        tglSupplierBackorderAllowed.Name = "tglSupplierBackorderAllowed";
        tglSupplierBackorderAllowed.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglSupplierBackorderAllowed.Properties.Appearance.Options.UseFont = true;
        tglSupplierBackorderAllowed.Properties.OffText = "No";
        tglSupplierBackorderAllowed.Properties.OnText = "Sí";
        tglSupplierBackorderAllowed.Size = new Size(86, 20);
        tglSupplierBackorderAllowed.TabIndex = 6;
        // 
        // memReceivingNote
        // 
        memReceivingNote.EditValue = "Verificar empaque, lote y vencimiento.";
        memReceivingNote.Location = new Point(137, 125);
        memReceivingNote.Name = "memReceivingNote";
        memReceivingNote.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memReceivingNote.Properties.Appearance.Options.UseFont = true;
        memReceivingNote.Size = new Size(273, 44);
        memReceivingNote.TabIndex = 18;
        // 
        // lblPurchaseOnDemand
        // 
        lblPurchaseOnDemand.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseOnDemand.Appearance.Options.UseFont = true;
        lblPurchaseOnDemand.Location = new Point(18, 177);
        lblPurchaseOnDemand.Name = "lblPurchaseOnDemand";
        lblPurchaseOnDemand.Size = new Size(112, 15);
        lblPurchaseOnDemand.TabIndex = 7;
        lblPurchaseOnDemand.Text = "Compra bajo pedido:";
        // 
        // lblReceivingNote
        // 
        lblReceivingNote.Appearance.Font = new Font("Segoe UI", 9F);
        lblReceivingNote.Appearance.Options.UseFont = true;
        lblReceivingNote.Location = new Point(18, 127);
        lblReceivingNote.Name = "lblReceivingNote";
        lblReceivingNote.Size = new Size(58, 15);
        lblReceivingNote.TabIndex = 17;
        lblReceivingNote.Text = "Recepción:";
        // 
        // tglPurchaseOnDemand
        // 
        tglPurchaseOnDemand.Location = new Point(137, 175);
        tglPurchaseOnDemand.Name = "tglPurchaseOnDemand";
        tglPurchaseOnDemand.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglPurchaseOnDemand.Properties.Appearance.Options.UseFont = true;
        tglPurchaseOnDemand.Properties.OffText = "No";
        tglPurchaseOnDemand.Properties.OnText = "Sí";
        tglPurchaseOnDemand.Size = new Size(86, 20);
        tglPurchaseOnDemand.TabIndex = 8;
        // 
        // memPurchasePolicy
        // 
        memPurchasePolicy.EditValue = "Comprar a proveedores activos y certificados.";
        memPurchasePolicy.Location = new Point(137, 75);
        memPurchasePolicy.Name = "memPurchasePolicy";
        memPurchasePolicy.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memPurchasePolicy.Properties.Appearance.Options.UseFont = true;
        memPurchasePolicy.Size = new Size(273, 44);
        memPurchasePolicy.TabIndex = 16;
        // 
        // lblPurchasePolicy
        // 
        lblPurchasePolicy.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchasePolicy.Appearance.Options.UseFont = true;
        lblPurchasePolicy.Location = new Point(18, 77);
        lblPurchasePolicy.Name = "lblPurchasePolicy";
        lblPurchasePolicy.Size = new Size(42, 15);
        lblPurchasePolicy.TabIndex = 15;
        lblPurchasePolicy.Text = "Política:";
        // 
        // lblMainPurchaseSupplier
        // 
        lblMainPurchaseSupplier.Appearance.Font = new Font("Segoe UI", 9F);
        lblMainPurchaseSupplier.Appearance.Options.UseFont = true;
        lblMainPurchaseSupplier.Location = new Point(463, 50);
        lblMainPurchaseSupplier.Name = "lblMainPurchaseSupplier";
        lblMainPurchaseSupplier.Size = new Size(106, 15);
        lblMainPurchaseSupplier.TabIndex = 30;
        lblMainPurchaseSupplier.Text = "Proveedor principal:";
        // 
        // slueMainPurchaseSupplier
        // 
        slueMainPurchaseSupplier.Location = new Point(629, 47);
        slueMainPurchaseSupplier.Name = "slueMainPurchaseSupplier";
        slueMainPurchaseSupplier.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        slueMainPurchaseSupplier.Properties.Appearance.Options.UseFont = true;
        slueMainPurchaseSupplier.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo), new EditorButton(ButtonPredefines.Plus) });
        slueMainPurchaseSupplier.Properties.NullText = "";
        slueMainPurchaseSupplier.Properties.PopupView = gvMainPurchaseSupplier;
        slueMainPurchaseSupplier.Size = new Size(280, 22);
        slueMainPurchaseSupplier.TabIndex = 31;
        // 
        // gvMainPurchaseSupplier
        // 
        gvMainPurchaseSupplier.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvMainPurchaseSupplier.Name = "gvMainPurchaseSupplier";
        gvMainPurchaseSupplier.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvMainPurchaseSupplier.OptionsView.ShowGroupPanel = false;
        // 
        // lblPreferredPurchasePresentation
        // 
        lblPreferredPurchasePresentation.Appearance.Font = new Font("Segoe UI", 9F);
        lblPreferredPurchasePresentation.Appearance.Options.UseFont = true;
        lblPreferredPurchasePresentation.Location = new Point(463, 78);
        lblPreferredPurchasePresentation.Name = "lblPreferredPurchasePresentation";
        lblPreferredPurchasePresentation.Size = new Size(121, 15);
        lblPreferredPurchasePresentation.TabIndex = 32;
        lblPreferredPurchasePresentation.Text = "Presentación preferida:";
        // 
        // luePreferredPurchasePresentation
        // 
        luePreferredPurchasePresentation.Location = new Point(629, 75);
        luePreferredPurchasePresentation.Name = "luePreferredPurchasePresentation";
        luePreferredPurchasePresentation.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePreferredPurchasePresentation.Properties.Appearance.Options.UseFont = true;
        luePreferredPurchasePresentation.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePreferredPurchasePresentation.Properties.NullText = "";
        luePreferredPurchasePresentation.Size = new Size(280, 22);
        luePreferredPurchasePresentation.TabIndex = 33;
        // 
        // lblPreferredPurchaseCurrency
        // 
        lblPreferredPurchaseCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblPreferredPurchaseCurrency.Appearance.Options.UseFont = true;
        lblPreferredPurchaseCurrency.Location = new Point(463, 106);
        lblPreferredPurchaseCurrency.Name = "lblPreferredPurchaseCurrency";
        lblPreferredPurchaseCurrency.Size = new Size(47, 15);
        lblPreferredPurchaseCurrency.TabIndex = 34;
        lblPreferredPurchaseCurrency.Text = "Moneda:";
        // 
        // luePreferredPurchaseCurrency
        // 
        luePreferredPurchaseCurrency.Location = new Point(629, 103);
        luePreferredPurchaseCurrency.Name = "luePreferredPurchaseCurrency";
        luePreferredPurchaseCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePreferredPurchaseCurrency.Properties.Appearance.Options.UseFont = true;
        luePreferredPurchaseCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePreferredPurchaseCurrency.Properties.NullText = "USD";
        luePreferredPurchaseCurrency.Size = new Size(280, 22);
        luePreferredPurchaseCurrency.TabIndex = 35;
        // 
        // lblPurchaseMinimumQuantity
        // 
        lblPurchaseMinimumQuantity.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseMinimumQuantity.Appearance.Options.UseFont = true;
        lblPurchaseMinimumQuantity.Location = new Point(463, 134);
        lblPurchaseMinimumQuantity.Name = "lblPurchaseMinimumQuantity";
        lblPurchaseMinimumQuantity.Size = new Size(95, 15);
        lblPurchaseMinimumQuantity.TabIndex = 36;
        lblPurchaseMinimumQuantity.Text = "Cantidad mínima:";
        // 
        // spnPurchaseMinimumQuantity
        // 
        spnPurchaseMinimumQuantity.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnPurchaseMinimumQuantity.Location = new Point(629, 131);
        spnPurchaseMinimumQuantity.Name = "spnPurchaseMinimumQuantity";
        spnPurchaseMinimumQuantity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnPurchaseMinimumQuantity.Properties.Appearance.Options.UseFont = true;
        spnPurchaseMinimumQuantity.Properties.Appearance.Options.UseTextOptions = true;
        spnPurchaseMinimumQuantity.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnPurchaseMinimumQuantity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnPurchaseMinimumQuantity.Properties.MaskSettings.Set("mask", "n2");
        spnPurchaseMinimumQuantity.Size = new Size(120, 22);
        spnPurchaseMinimumQuantity.TabIndex = 37;
        // 
        // lblPurchaseMultiple
        // 
        lblPurchaseMultiple.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseMultiple.Appearance.Options.UseFont = true;
        lblPurchaseMultiple.Location = new Point(463, 162);
        lblPurchaseMultiple.Name = "lblPurchaseMultiple";
        lblPurchaseMultiple.Size = new Size(108, 15);
        lblPurchaseMultiple.TabIndex = 38;
        lblPurchaseMultiple.Text = "Múltiplo de compra:";
        // 
        // spnPurchaseMultiple
        // 
        spnPurchaseMultiple.EditValue = new decimal(new int[] { 100, 0, 0, 131072 });
        spnPurchaseMultiple.Location = new Point(629, 159);
        spnPurchaseMultiple.Name = "spnPurchaseMultiple";
        spnPurchaseMultiple.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnPurchaseMultiple.Properties.Appearance.Options.UseFont = true;
        spnPurchaseMultiple.Properties.Appearance.Options.UseTextOptions = true;
        spnPurchaseMultiple.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnPurchaseMultiple.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnPurchaseMultiple.Properties.MaskSettings.Set("mask", "n2");
        spnPurchaseMultiple.Size = new Size(120, 22);
        spnPurchaseMultiple.TabIndex = 39;
        // 
        // lblPurchaseDeliveryDays
        // 
        lblPurchaseDeliveryDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseDeliveryDays.Appearance.Options.UseFont = true;
        lblPurchaseDeliveryDays.Location = new Point(463, 190);
        lblPurchaseDeliveryDays.Name = "lblPurchaseDeliveryDays";
        lblPurchaseDeliveryDays.Size = new Size(84, 15);
        lblPurchaseDeliveryDays.TabIndex = 40;
        lblPurchaseDeliveryDays.Text = "Días de entrega:";
        // 
        // spnPurchaseDeliveryDays
        // 
        spnPurchaseDeliveryDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnPurchaseDeliveryDays.Location = new Point(629, 187);
        spnPurchaseDeliveryDays.Name = "spnPurchaseDeliveryDays";
        spnPurchaseDeliveryDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnPurchaseDeliveryDays.Properties.Appearance.Options.UseFont = true;
        spnPurchaseDeliveryDays.Properties.Appearance.Options.UseTextOptions = true;
        spnPurchaseDeliveryDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnPurchaseDeliveryDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnPurchaseDeliveryDays.Properties.IsFloatValue = false;
        spnPurchaseDeliveryDays.Properties.MaskSettings.Set("mask", "d");
        spnPurchaseDeliveryDays.Size = new Size(120, 22);
        spnPurchaseDeliveryDays.TabIndex = 41;
        // 
        // btnViewPurchaseDocument
        // 
        btnViewPurchaseDocument.Appearance.ForeColor = Color.FromArgb((int)(byte)37, (int)(byte)99, (int)(byte)235);
        btnViewPurchaseDocument.Appearance.Options.UseForeColor = true;
        btnViewPurchaseDocument.Location = new Point(1120, 248);
        btnViewPurchaseDocument.Name = "btnViewPurchaseDocument";
        btnViewPurchaseDocument.PaintStyle = PaintStyles.Light;
        btnViewPurchaseDocument.Size = new Size(112, 28);
        btnViewPurchaseDocument.TabIndex = 42;
        btnViewPurchaseDocument.Text = "Ver documento";
        // 
        // btnRefreshPurchases
        // 
        btnRefreshPurchases.Appearance.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        btnRefreshPurchases.Appearance.Options.UseForeColor = true;
        btnRefreshPurchases.Location = new Point(1250, 248);
        btnRefreshPurchases.Name = "btnRefreshPurchases";
        btnRefreshPurchases.PaintStyle = PaintStyles.Light;
        btnRefreshPurchases.Size = new Size(112, 28);
        btnRefreshPurchases.TabIndex = 43;
        btnRefreshPurchases.Text = "Actualizar";
        // 
        // tabFinance
        // 
        tabFinance.Controls.Add(tabFinanceSections);
        tabFinance.ImageOptions.SvgImageSize = new Size(22, 22);
        tabFinance.Name = "tabFinance";
        tabFinance.Size = new Size(1406, 426);
        tabFinance.Text = "Finanzas";
        // 
        // tabFinanceSections
        // 
        tabFinanceSections.Appearance.Font = new Font("Segoe UI", 9F);
        tabFinanceSections.Appearance.Options.UseFont = true;
        tabFinanceSections.AppearancePage.Header.Font = new Font("Segoe UI", 9F);
        tabFinanceSections.AppearancePage.Header.Options.UseFont = true;
        tabFinanceSections.AppearancePage.HeaderActive.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        tabFinanceSections.AppearancePage.HeaderActive.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        tabFinanceSections.AppearancePage.HeaderActive.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        tabFinanceSections.AppearancePage.HeaderActive.Options.UseBackColor = true;
        tabFinanceSections.AppearancePage.HeaderActive.Options.UseFont = true;
        tabFinanceSections.AppearancePage.HeaderActive.Options.UseForeColor = true;
        tabFinanceSections.AppearancePage.PageClient.BackColor = Color.White;
        tabFinanceSections.AppearancePage.PageClient.Options.UseBackColor = true;
        tabFinanceSections.Dock = DockStyle.Fill;
        tabFinanceSections.HeaderAutoFill = DefaultBoolean.False;
        tabFinanceSections.Location = new Point(0, 0);
        tabFinanceSections.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        tabFinanceSections.LookAndFeel.UseDefaultLookAndFeel = false;
        tabFinanceSections.Name = "tabFinanceSections";
        tabFinanceSections.SelectedTabPage = tabCosts;
        tabFinanceSections.Size = new Size(1406, 426);
        tabFinanceSections.TabIndex = 0;
        tabFinanceSections.TabPages.AddRange(new XtraTabPage[] { tabCosts, tabAccounting, tabTaxes });
        tabFinanceSections.TabPageWidth = 220;
        tabFinanceSections.SelectedPageChanged += (this.tabFinanceSections_SelectedPageChanged);
        tabFinanceSections.CustomDrawTabHeader += (this.tabFinanceSections_CustomDrawTabHeader);
        tabFinanceSections.HandleCreated += (this.tabFinanceSections_HandleCreated);
        // 
        // tabDocuments
        // 
        tabDocuments.Appearance.PageClient.BackColor = Color.White;
        tabDocuments.Appearance.PageClient.BackColor2 = Color.White;
        tabDocuments.Appearance.PageClient.Options.UseBackColor = true;
        tabDocuments.BackColor = Color.White;
        tabDocuments.Controls.Add(tabDocumentSections);
        tabDocuments.ImageOptions.SvgImageSize = new Size(22, 22);
        tabDocuments.Name = "tabDocuments";
        tabDocuments.Size = new Size(1406, 426);
        tabDocuments.Text = "Documentos";
        // 
        // tabDocumentSections
        // 
        tabDocumentSections.Appearance.BackColor = Color.White;
        tabDocumentSections.Appearance.Font = new Font("Segoe UI", 9F);
        tabDocumentSections.Appearance.Options.UseBackColor = true;
        tabDocumentSections.Appearance.Options.UseFont = true;
        tabDocumentSections.AppearancePage.Header.Font = new Font("Segoe UI", 9F);
        tabDocumentSections.AppearancePage.Header.Options.UseFont = true;
        tabDocumentSections.AppearancePage.HeaderActive.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        tabDocumentSections.AppearancePage.HeaderActive.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        tabDocumentSections.AppearancePage.HeaderActive.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        tabDocumentSections.AppearancePage.HeaderActive.Options.UseBackColor = true;
        tabDocumentSections.AppearancePage.HeaderActive.Options.UseFont = true;
        tabDocumentSections.AppearancePage.HeaderActive.Options.UseForeColor = true;
        tabDocumentSections.AppearancePage.PageClient.BackColor = Color.White;
        tabDocumentSections.AppearancePage.PageClient.BackColor2 = Color.White;
        tabDocumentSections.AppearancePage.PageClient.Options.UseBackColor = true;
        tabDocumentSections.BackColor = Color.White;
        tabDocumentSections.Dock = DockStyle.Fill;
        tabDocumentSections.HeaderAutoFill = DefaultBoolean.False;
        tabDocumentSections.Location = new Point(0, 0);
        tabDocumentSections.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        tabDocumentSections.LookAndFeel.UseDefaultLookAndFeel = false;
        tabDocumentSections.Name = "tabDocumentSections";
        tabDocumentSections.SelectedTabPage = tabAttachments;
        tabDocumentSections.Size = new Size(1406, 426);
        tabDocumentSections.TabIndex = 0;
        tabDocumentSections.TabPages.AddRange(new XtraTabPage[] { tabAttachments, tabRemarks });
        tabDocumentSections.TabPageWidth = 220;
        // 
        // tabMain
        // 
        tabMain.Appearance.BackColor = Color.White;
        tabMain.Appearance.Font = new Font("Segoe UI", 9F);
        tabMain.Appearance.Options.UseBackColor = true;
        tabMain.Appearance.Options.UseFont = true;
        tabMain.AppearancePage.Header.BackColor = Color.White;
        tabMain.AppearancePage.Header.BorderColor = Color.White;
        tabMain.AppearancePage.Header.Font = new Font("Segoe UI", 9F);
        tabMain.AppearancePage.Header.ForeColor = Color.FromArgb((int)(byte)51, (int)(byte)65, (int)(byte)85);
        tabMain.AppearancePage.Header.Options.UseBackColor = true;
        tabMain.AppearancePage.Header.Options.UseBorderColor = true;
        tabMain.AppearancePage.Header.Options.UseFont = true;
        tabMain.AppearancePage.Header.Options.UseForeColor = true;
        tabMain.AppearancePage.Header.Options.UseTextOptions = true;
        tabMain.AppearancePage.Header.TextOptions.HAlignment = HorzAlignment.Near;
        tabMain.AppearancePage.Header.TextOptions.VAlignment = VertAlignment.Center;
        tabMain.AppearancePage.HeaderActive.BackColor = Color.FromArgb((int)(byte)230, (int)(byte)250, (int)(byte)246);
        tabMain.AppearancePage.HeaderActive.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        tabMain.AppearancePage.HeaderActive.ForeColor = Color.FromArgb((int)(byte)0, (int)(byte)184, (int)(byte)148);
        tabMain.AppearancePage.HeaderActive.Options.UseBackColor = true;
        tabMain.AppearancePage.HeaderActive.Options.UseFont = true;
        tabMain.AppearancePage.HeaderActive.Options.UseForeColor = true;
        tabMain.AppearancePage.PageClient.BackColor = Color.White;
        tabMain.AppearancePage.PageClient.Options.UseBackColor = true;
        tabMain.BorderStyle = BorderStyles.NoBorder;
        tabMain.BorderStylePage = BorderStyles.NoBorder;
        tabMain.HeaderLocation = TabHeaderLocation.Left;
        tabMain.HeaderOrientation = TabOrientation.Horizontal;
        tabMain.Location = new Point(0, 174);
        tabMain.Name = "tabMain";
        tabMain.PaintStyleName = "PropertyView";
        tabMain.SelectedTabPage = tabGeneral;
        tabMain.Size = new Size(1582, 426);
        tabMain.TabIndex = 1;
        tabMain.TabPages.AddRange(new XtraTabPage[] { tabGeneral, tabUnits, tabInventory, tabCommercial, tabFinance, tabLots, tabSap, tabDocuments });
        tabMain.TabPageWidth = 164;
        tabMain.SelectedPageChanged += (this.tabMain_SelectedPageChanged);
        tabMain.CustomDrawTabHeader += (this.tabMain_CustomDrawTabHeader);
        tabMain.HandleCreated += (this.tabMain_HandleCreated);
        // 
        // sepHeaderData
        // 
        sepHeaderData.Appearance.BackColor = Color.FromArgb((int)(byte)223, (int)(byte)228, (int)(byte)234);
        sepHeaderData.Appearance.Options.UseBackColor = true;
        sepHeaderData.AutoSizeMode = LabelAutoSizeMode.None;
        sepHeaderData.Location = new Point(332, 21);
        sepHeaderData.Name = "sepHeaderData";
        sepHeaderData.Size = new Size(700, 1);
        sepHeaderData.TabIndex = 35;
        // 
        // ItemEditForm
        // 
        this.Appearance.BackColor = Color.White;
        this.Appearance.Options.UseBackColor = true;
        this.Appearance.Options.UseFont = true;
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.CancelButton = null;
        this.CancelButtonLocation = new Point(1286, 606);
        this.ClientSize = new Size(1594, 828);
        this.Controls.Add(lblHeaderCommercialSummaryTitle);
        this.Controls.Add(lblHeaderClassificationTitle);
        this.Controls.Add(lblHeaderDataTitle);
        this.Controls.Add(sepHeaderCommercialSummary);
        this.Controls.Add(sepHeaderClassification);
        this.Controls.Add(sepHeaderData);
        this.Controls.Add(lblHeaderSalesPriceValue);
        this.Controls.Add(lblHeaderSalesPriceCaption);
        this.Controls.Add(lblHeaderAverageCostValue);
        this.Controls.Add(lblHeaderAverageCostCaption);
        this.Controls.Add(lblHeaderStockValue);
        this.Controls.Add(lblHeaderStockCaption);
        this.Controls.Add(picItem);
        this.Controls.Add(lblItemCode);
        this.Controls.Add(tabMain);
        this.Controls.Add(txtItemCode);
        this.Controls.Add(lblDescription);
        this.Controls.Add(lblValidationIndicator);
        this.Controls.Add(txtDescription);
        this.Controls.Add(lblUnsavedIndicator);
        this.Controls.Add(lblCommercialName);
        this.Controls.Add(lblStatus);
        this.Controls.Add(txtCommercialName);
        this.Controls.Add(lblStatusCaption);
        this.Controls.Add(lblItemType);
        this.Controls.Add(lueBaseUnit);
        this.Controls.Add(lueItemType);
        this.Controls.Add(lblBaseUnit);
        this.Controls.Add(lblItemGroup);
        this.Controls.Add(lueBrand);
        this.Controls.Add(lueItemGroup);
        this.Controls.Add(lblBrand);
        this.Controls.Add(lblAffectsInventory);
        this.Controls.Add(lblItemFamily);
        this.Controls.Add(tglAffectsInventory);
        this.Controls.Add(lueItemFamily);
        this.Controls.Add(tglPurchaseActive);
        this.Controls.Add(lblPurchaseActive);
        this.Controls.Add(tglSalesActive);
        this.Controls.Add(lblSalesActive);
        this.MinimumSize = new Size(1440, 860);
        this.Name = "ItemEditForm";
        this.SaveButtonLocation = new Point(1434, 606);
        this.Text = "Maestro de ítems / Artículos";
        this.Controls.SetChildIndex(lblSalesActive, 0);
        this.Controls.SetChildIndex(tglSalesActive, 0);
        this.Controls.SetChildIndex(lblPurchaseActive, 0);
        this.Controls.SetChildIndex(tglPurchaseActive, 0);
        this.Controls.SetChildIndex(lueItemFamily, 0);
        this.Controls.SetChildIndex(tglAffectsInventory, 0);
        this.Controls.SetChildIndex(lblItemFamily, 0);
        this.Controls.SetChildIndex(lblAffectsInventory, 0);
        this.Controls.SetChildIndex(lblBrand, 0);
        this.Controls.SetChildIndex(lueItemGroup, 0);
        this.Controls.SetChildIndex(lueBrand, 0);
        this.Controls.SetChildIndex(lblItemGroup, 0);
        this.Controls.SetChildIndex(lblBaseUnit, 0);
        this.Controls.SetChildIndex(lueItemType, 0);
        this.Controls.SetChildIndex(lueBaseUnit, 0);
        this.Controls.SetChildIndex(lblItemType, 0);
        this.Controls.SetChildIndex(lblStatusCaption, 0);
        this.Controls.SetChildIndex(txtCommercialName, 0);
        this.Controls.SetChildIndex(lblStatus, 0);
        this.Controls.SetChildIndex(lblCommercialName, 0);
        this.Controls.SetChildIndex(lblUnsavedIndicator, 0);
        this.Controls.SetChildIndex(txtDescription, 0);
        this.Controls.SetChildIndex(lblValidationIndicator, 0);
        this.Controls.SetChildIndex(lblDescription, 0);
        this.Controls.SetChildIndex(txtItemCode, 0);
        this.Controls.SetChildIndex(tabMain, 0);
        this.Controls.SetChildIndex(lblItemCode, 0);
        this.Controls.SetChildIndex(picItem, 0);
        this.Controls.SetChildIndex(lblHeaderStockCaption, 0);
        this.Controls.SetChildIndex(lblHeaderStockValue, 0);
        this.Controls.SetChildIndex(lblHeaderAverageCostCaption, 0);
        this.Controls.SetChildIndex(lblHeaderAverageCostValue, 0);
        this.Controls.SetChildIndex(lblHeaderSalesPriceCaption, 0);
        this.Controls.SetChildIndex(lblHeaderSalesPriceValue, 0);
        this.Controls.SetChildIndex(sepHeaderData, 0);
        this.Controls.SetChildIndex(sepHeaderClassification, 0);
        this.Controls.SetChildIndex(sepHeaderCommercialSummary, 0);
        this.Controls.SetChildIndex(lblHeaderDataTitle, 0);
        this.Controls.SetChildIndex(lblHeaderClassificationTitle, 0);
        this.Controls.SetChildIndex(lblHeaderCommercialSummaryTitle, 0);
        this.Controls.SetChildIndex(this.btnGuardar, 0);
        this.Controls.SetChildIndex(this.btnCancelar, 0);
        ((System.ComponentModel.ISupportInitialize)picItem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtItemCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCommercialName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueItemType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueItemGroup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueItemFamily.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBrand.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBaseUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)itemPresentationsTable).EndInit();
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
        tabRemarks.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memGeneralNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memGeneralOperationalAlert.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueNotePriority.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueNoteVisibility.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkGeneralNoteActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memPurchaseNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memSalesNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memInventoryNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memLogisticsQualityNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdOperationalAlerts).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvOperationalAlerts).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoOperationalAlertCheck).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvOperationalAlertsAux).EndInit();
        tabAttachments.ResumeLayout(false);
        tabAttachments.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoAttachmentCheck).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentFileName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memAttachmentDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentCategory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInSales.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInPurchases.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkVisibleInPortal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAttachmentStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentReference.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAttachmentPrincipal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAttachmentConfidential.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnAttachmentOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentValidFrom.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentValidFrom.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentValidTo.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentValidTo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memAttachmentAlternativeText.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)picMainAttachmentPreview.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentExtension.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentSize.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentUploadedAt.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteAttachmentUploadedAt.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentUser.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView2).EndInit();
        tabSap.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)tabSapSections).EndInit();
        tabSapSections.ResumeLayout(false);
        tabSapStatusPage.ResumeLayout(false);
        tabSapStatusPage.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglSapSynchronize.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapMapEnabled.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapMapRequired.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapSapField.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapMapSystemField.Properties).EndInit();
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
        tabSapHistoryPage.ResumeLayout(false);
        tabSapHistoryPage.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdSapSyncHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSapSyncHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdSapFieldMapping).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSapFieldMapping).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapEnabled.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView3).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView4).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView5).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView6).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView7).EndInit();
        tabLots.ResumeLayout(false);
        tabLots.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueIssueMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowMultipleBatches.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowReceiptWithoutLot.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowExpiredBatchSale.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockQuarantineBatch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockExpiredBatch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memLotOperationalNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresExpiration.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglExpirationMandatory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglManufacturingDateRequired.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAutoGenerateBatch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBatchPrefix.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSerialLength.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnShelfLifeDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnExpirationAlertDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnQuarantineDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBatchFormat.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueNumberingMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlLotOperationalNote).EndInit();
        pnlLotOperationalNote.ResumeLayout(false);
        pnlLotOperationalNote.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlLotTraceabilityNote).EndInit();
        pnlLotTraceabilityNote.ResumeLayout(false);
        tabTaxes.ResumeLayout(false);
        tabTaxes.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueFiscalItemType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseVat.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxesSalesVat.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueExciseTax.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxesSuggestedWithholding.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueTaxSupport.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtFiscalCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglTaxableGoods.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglTaxableService.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglTaxExemptGoods.Properties).EndInit();
        tabAccounting.ResumeLayout(false);
        tabAccounting.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglGenerateInventoryJournal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglUseWarehouseAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueInventoryAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvInventoryAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglUseGroupAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueRevenueAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvRevenueAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowCompensation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueCostOfGoodsSoldAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvCostOfGoodsSoldAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAccountingBlocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueSalesReturnAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvSalesReturnAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnReconciliationDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluePurchaseReturnAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseReturnAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingIntegrationMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueCostVarianceAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvCostVarianceAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)memAccountingNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueInventoryAdjustmentAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvInventoryAdjustmentAccount).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluePurchaseExpenseAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseExpenseAccount).EndInit();
        tabCosts.ResumeLayout(false);
        tabCosts.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdCostPriceHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvCostPriceHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvCostPriceHistoryAux).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnAnalysisBasePrice.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCostCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSuggestedPrice.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnStandardCost.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumMarginPercent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnReplacementCost.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnTargetMarginPercent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnLastCost.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnAverageCost.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtPriceUpdatedAt.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtPriceUpdatedAt.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtCostUpdatedAt.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtCostUpdatedAt.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglManualCostUpdate.Properties).EndInit();
        tabSales.ResumeLayout(false);
        tabSales.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdSalesPriceLists).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvSalesPriceLists).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoSalesPriceListActive).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvSalesPriceListsAux).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAffectsPromotions.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnBaseSalesPrice.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueMainPriceList.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowSalesDiscount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMaxDiscount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumMargin.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumSale.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSalesMultiple.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSalesCommission.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesChannel.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesSegment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesMinimumPriceList.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSalesMinimumPrice.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtSalesValidFrom.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtSalesValidFrom.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSalesEcommerce.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memSalesCommercialObservation.Properties).EndInit();
        tabInventory.ResumeLayout(false);
        tabInventory.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)slueDefaultBinLocation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvDefaultBinLocation).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdWarehouseStock).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvWarehouseStock).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvWarehouseStockAux).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCoverageDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnLeadTimeDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalMinStock.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueReplenishmentMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalMaxStock.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplyMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglBlockedForMovements.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memInventoryOperationNote.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnGlobalReorderPoint.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueMainWarehouse.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvMainWarehouse).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueValuationMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSuggestedPurchaseQty.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueNegativeStockPolicy.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglReplenishmentApproval.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAutoReplenishment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglManageLocations.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresCycleCount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAbcClassification.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueInventoryControlType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memInventoryBlockReason.Properties).EndInit();
        tabUnits.ResumeLayout(false);
        tabUnits.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)txtQrCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdItemPresentations).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvItemPresentations).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoPurchaseActive).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoPurchasePrincipal).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvItemPresentationsAux).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPlu.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPreviousInternalCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueInventoryUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtManufacturerReference.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtUnspscCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtTariffCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnNetWeight.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCodeOrigin.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnGrossWeight.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnVolume.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueWeightUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueVolumeUnit.Properties).EndInit();
        tabGeneral.ResumeLayout(false);
        tabGeneral.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglBlockedEcommerce.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralMobileItem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralRequiresScale.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralAllowDiscount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralPerishable.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglGeneralExpirationManaged.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAlternateCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueSupplierSku.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvSupplierSku).EndInit();
        ((System.ComponentModel.ISupportInitialize)memLongDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueProductType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueOrigin.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueLine.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSubGroup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtModel.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtReference.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAffectsInventory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSalesActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseActive.Properties).EndInit();
        tabCommercial.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)tabCommercialSections).EndInit();
        tabCommercialSections.ResumeLayout(false);
        tabPurchases.ResumeLayout(false);
        tabPurchases.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)luePurchaseUnit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdPurchaseHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseHistoryAux).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridView1).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseApprovalRequired.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSupplierBackorderAllowed.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memReceivingNote.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglPurchaseOnDemand.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memPurchasePolicy.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)slueMainPurchaseSupplier.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvMainPurchaseSupplier).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePreferredPurchasePresentation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePreferredPurchaseCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnPurchaseMinimumQuantity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnPurchaseMultiple.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnPurchaseDeliveryDays.Properties).EndInit();
        tabFinance.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)tabFinanceSections).EndInit();
        tabFinanceSections.ResumeLayout(false);
        tabDocuments.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)tabDocumentSections).EndInit();
        tabDocumentSections.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)tabMain).EndInit();
        tabMain.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();
    }
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
    private System.Data.DataTable itemPresentationsTable;
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
    private LabelControl lblNotesGeneralTitle;
    private LabelControl lblGeneralNotes;
    private MemoEdit memGeneralNotes;
    private LabelControl lblGeneralOperationalAlert;
    private MemoEdit memGeneralOperationalAlert;
    private LabelControl lblNotePriority;
    private LookUpEdit lueNotePriority;
    private LabelControl lblNoteVisibility;
    private LookUpEdit lueNoteVisibility;
    private LabelControl lblGeneralNoteActive;
    private ToggleSwitch chkGeneralNoteActive;
    private LabelControl lblNotesProcessTitle;
    private LabelControl lblPurchaseNotes;
    private MemoEdit memPurchaseNotes;
    private LabelControl lblSalesNotes;
    private MemoEdit memSalesNotes;
    private LabelControl lblInventoryNotes;
    private MemoEdit memInventoryNotes;
    private LabelControl lblLogisticsQualityNotes;
    private MemoEdit memLogisticsQualityNotes;
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
    private GridColumn colOperationalAlertPriority;
    private GridColumn colOperationalAlertConfirmation;
    private SimpleButton btnAddOperationalAlert;
    private SimpleButton btnUpdateOperationalAlert;
    private SimpleButton btnRemoveOperationalAlert;
    private SimpleButton btnClearOperationalAlert;
    private GridView gvOperationalAlertsAux;
    private XtraTabPage tabAttachments;
    private GridView gridView2;
    private XtraTabPage tabSap;
    private GridView gridView3;
    private GridView gridView4;
    private GridView gridView5;
    private GridView gridView6;
    private GridView gridView7;
    private XtraTabPage tabLots;
    private LabelControl sepTraceabilityHeader;
    private LabelControl sepTraceabilityColumnOne;
    private LabelControl sepTraceabilityColumnTwo;
    private LabelControl sepTraceabilityGeneration;
    private LabelControl sepTraceabilityExpiration;
    private LabelControl sepTraceabilityOperations;
    private LabelControl lblInheritedTraceabilityTitle;
    private LabelControl lblInheritedBatchStatus;
    private LabelControl lblInheritedSerialStatus;
    private LabelControl lblInheritedPerishableStatus;
    private LabelControl lblInheritedExpirationStatus;
    private LabelControl lblLotOperationalRulesTitle;
    private LabelControl lblIssueMethod;
    private LookUpEdit lueIssueMethod;
    private LabelControl lblAllowMultipleBatches;
    private ToggleSwitch tglAllowMultipleBatches;
    private LabelControl lblAllowReceiptWithoutLot;
    private ToggleSwitch tglAllowReceiptWithoutLot;
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
    private LabelControl lblLotExpirationTitle;
    private LabelControl lblRequiresExpiration;
    private ToggleSwitch tglRequiresExpiration;
    private LabelControl lblExpirationMandatory;
    private ToggleSwitch tglExpirationMandatory;
    private LabelControl lblManufacturingDateRequired;
    private ToggleSwitch tglManufacturingDateRequired;
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
    private LabelControl lblLotTransferRuleIcon;
    private LabelControl lblLotTransferRule;
    private LabelControl lblSerialDispatchRuleIcon;
    private LabelControl lblSerialDispatchRule;
    private LabelControl lblTraceabilityFooterIcon;
    private LabelControl lblTraceabilityFooter;
    private XtraTabPage tabTaxes;
    private LabelControl sepTaxesColumnOne;
    private LabelControl sepTaxesColumnTwo;
    private LabelControl sepTaxConfiguration;
    private LabelControl sepTaxRates;
    private LabelControl sepTaxApplicability;
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
    private LabelControl lblTaxRatesTitle;
    private LabelControl lblTaxApplicabilityTitle;
    private XtraTabPage tabAccounting;
    private LabelControl sepAccountingColumnOne;
    private LabelControl sepAccountingColumnTwo;
    private LabelControl sepAccountingAccounts;
    private LabelControl sepAccountingComplementary;
    private LabelControl sepAccountingRules;
    private LabelControl lblAccountingAccountsTitle;
    private LabelControl lblAccountingComplementaryTitle;
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
    private XtraTabPage tabCosts;
    private LabelControl sepCostsColumnOne;
    private LabelControl sepCostsColumnTwo;
    private LabelControl sepCostsBase;
    private LabelControl sepCostsPrices;
    private LabelControl sepCostsIndicators;
    private LabelControl sepCostsHistory;
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
    private SpinEdit spnLastCost;
    private LabelControl lblCostsAverageCost;
    private SpinEdit spnAverageCost;
    private LabelControl lblPriceUpdatedAt;
    private LabelControl lblCostUpdatedAt;
    private DateEdit dtPriceUpdatedAt;
    private DateEdit dtCostUpdatedAt;
    private LabelControl lblManualCostUpdate;
    private ToggleSwitch tglManualCostUpdate;
    private LabelControl lblFinanceCostIndicatorsTitle;
    private NuanOperationalKpiCardControl kpiFinanceGrossMargin;
    private NuanOperationalKpiCardControl kpiFinanceGrossMarginPercent;
    private NuanOperationalKpiCardControl kpiFinanceProfitability;
    private NuanOperationalKpiCardControl kpiFinanceSuggestedPrice;
    private GridView gvCostPriceHistoryAux;
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
    private LabelControl sepSalesColumnOne;
    private LabelControl sepSalesColumnTwo;
    private LabelControl sepSalesConfiguration;
    private LabelControl sepSalesConditions;
    private LabelControl sepSalesIndicators;
    private LabelControl sepSalesPriceLists;
    private LabelControl lblSalesConditionsTitle;
    private LabelControl lblSalesIndicatorsTitle;
    private LabelControl lblSalesConfigurationTitle;
    private NuanOperationalKpiCardControl kpiSales30d;
    private LabelControl lblAffectsPromotions;
    private NuanOperationalKpiCardControl kpiSales12m;
    private ToggleSwitch tglAffectsPromotions;
    private NuanOperationalKpiCardControl kpiSalesLastPrice;
    private LabelControl lblSalesUnit;
    private NuanOperationalKpiCardControl kpiSalesCustomers;
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
    private LabelControl lblSalesChannel;
    private LookUpEdit lueSalesChannel;
    private LabelControl lblSalesSegment;
    private LookUpEdit lueSalesSegment;
    private LabelControl lblSalesMinimumPriceList;
    private LookUpEdit lueSalesMinimumPriceList;
    private LabelControl lblSalesMinimumPrice;
    private SpinEdit spnSalesMinimumPrice;
    private LabelControl lblSalesMinimumCurrency;
    private LabelControl lblSalesValidFrom;
    private DateEdit dtSalesValidFrom;
    private LabelControl lblSalesEcommerce;
    private ToggleSwitch tglSalesEcommerce;
    private LabelControl lblSalesCommercialObservation;
    private MemoEdit memSalesCommercialObservation;
    private SimpleButton btnViewSalesHistory;
    private SimpleButton btnRefreshSales;
    private GridView gvSalesPriceListsAux;
    private XtraTabPage tabInventory;
    private LabelControl sepInventoryColumnOne;
    private LabelControl sepInventoryColumnTwo;
    private LabelControl sepInventoryParameters;
    private LabelControl sepInventoryReplenishment;
    private LabelControl sepInventoryLocations;
    private LabelControl sepInventoryWarehouse;
    private LabelControl lblWarehouseSummary;
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
    private GridView gvWarehouseStockAux;
    private XtraTabPage tabUnits;
    private LabelControl sepUnitsColumn;
    private LabelControl sepUnitsMeasures;
    private LabelControl sepUnitsIdentifiers;
    private LabelControl sepUnitsPresentations;
    private LabelControl lblPresentationSummary;
    private LabelControl lblCodesIdentifiersTitle;
    private LabelControl lblQrCode;
    private LabelControl lblPurchasePresentationsTitle;
    private TextEdit txtQrCode;
    private GridControl grdItemPresentations;
    private GridView gvItemPresentations;
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
    private SimpleButton btnAddItemPresentation;
    private LabelControl lblPreviousInternalCode;
    private LabelControl lblInventoryUnit;
    private TextEdit txtPreviousInternalCode;
    private SimpleButton btnUpdateItemPresentation;
    private LabelControl lblManufacturerReference;
    private LookUpEdit lueInventoryUnit;
    private TextEdit txtManufacturerReference;
    private SimpleButton btnRemoveItemPresentation;
    private LabelControl lblUnspscCode;
    private TextEdit txtUnspscCode;
    private SimpleButton btnSetMainItemPresentation;
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
    private GridView gvItemPresentationsAux;
    private XtraTabPage tabGeneral;
    private LabelControl sepGeneralIdentification;
    private LabelControl sepGeneralOperation;
    private LabelControl sepGeneralSummary;
    private LabelControl sepGeneralColumnOne;
    private LabelControl sepGeneralColumnTwo;
    private LabelControl lblBlockedEcommerce;
    private ToggleSwitch tglBlockedEcommerce;
    private LabelControl lblGeneralSummaryTitle;
    private NuanOperationalKpiCardControl kpiStockAvailable;
    private NuanOperationalKpiCardControl kpiOnOrder;
    private NuanOperationalKpiCardControl kpiPurchases;
    private NuanOperationalKpiCardControl kpiSales;
    private NuanOperationalKpiCardControl kpiSapStatus;
    private NuanOperationalKpiCardControl kpiAverageCost;
    private NuanOperationalKpiCardControl kpiMargin;
    private NuanOperationalKpiCardControl kpiPurchaseCost;
    private NuanOperationalKpiCardControl kpiSalesPrice;
    private NuanOperationalKpiCardControl kpiCommitted;
    private ToggleSwitch tglGeneralMobileItem;
    private LabelControl lblGeneralMobileItem;
    private ToggleSwitch tglGeneralRequiresScale;
    private ToggleSwitch tglGeneralAllowDiscount;
    private ToggleSwitch tglGeneralPerishable;
    private SimpleButton btnTraceabilityNone;
    private SimpleButton btnTraceabilityBatch;
    private SimpleButton btnTraceabilitySerial;
    private LabelControl lblGeneralOperationTitle;
    private LabelControl lblTraceabilityManagement;
    private LabelControl lblTraceabilityHintIcon;
    private LabelControl lblTraceabilityHint;
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
    private XtraTabPage tabCommercial;
    private XtraTabControl tabCommercialSections;
    private XtraTabPage tabFinance;
    private XtraTabControl tabFinanceSections;
    private XtraTabPage tabDocuments;
    private XtraTabControl tabDocumentSections;
    private XtraTabControl tabMain;
    private XtraTabControl tabSapSections;
    private XtraTabPage tabSapStatusPage;
    private XtraTabPage tabSapHistoryPage;
    private ToggleSwitch tglSapSynchronize;
    private LabelControl lblSapExternalSystemCaption;
    private LabelControl lblSapExternalSystemValue;
    private LabelControl lblSapExternalCodeCaption;
    private LabelControl lblSapExternalCodeValue;
    private LabelControl lblSapAuthorityCaption;
    private LabelControl lblSapAuthorityValue;
    private LabelControl lblSapBatchCaption;
    private LabelControl lblSapBatchValue;
    private LabelControl lblSapSerialCaption;
    private LabelControl lblSapSerialValue;
    private LabelControl sepSapColumnOne;
    private LabelControl sepSapColumnTwo;
    private LabelControl sepSapStatusTitleLine;
    private LabelControl sepSapConfigTitleLine;
    private LabelControl sepSapCorrespondenceTitleLine;
    private HyperlinkLabelControl lnkSapViewProfile;
    private HyperlinkLabelControl lnkSapRefreshStatus;
    private HyperlinkLabelControl lnkSapSynchronizeNow;
    private LabelControl lblSapIntegrationNote;
    private LabelControl lblSapCurrentStatusSummary;
    private LabelControl lblSapLastSyncSummary;
    private LabelControl lblSapPendingRetriesSummary;
    private HyperlinkLabelControl lnkSapRefreshHistory;
    private LabelControl lblSapExecutionDetailTitle;
    private LabelControl sepSapExecutionDetailTitle;
    private LabelControl lblSapExecutionResultCaption;
    private LabelControl lblSapExecutionResultValue;
    private LabelControl lblSapExecutionMessageCaption;
    private LabelControl lblSapExecutionMessageValue;
    private LabelControl lblSapExecutionTrackingCaption;
    private LabelControl lblSapExecutionTrackingValue;
    private LabelControl lblSapExecutionUserCaption;
    private LabelControl lblSapExecutionUserValue;
    private LabelControl lblSapExecutionProfileCaption;
    private LabelControl lblSapExecutionProfileValue;
    private LabelControl lblSapActionsTitle;
    private LabelControl sepSapActionsTitle;
    private HyperlinkLabelControl lnkSapViewDetail;
    private HyperlinkLabelControl lnkSapCopyTracking;
    private HyperlinkLabelControl lnkSapRetry;
    private LabelControl lblSapHistoryNote;
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
    private GridColumn colSapHistoryDuration;
    private GridColumn colSapHistoryTracking;
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
    private GridColumn colAttachmentCategory;
    private GridColumn colAttachmentVisiblePortal;
    private GridColumn colAttachmentStatus;
    private SimpleButton btnAddAttachment;
    private SimpleButton btnUpdateAttachment;
    private SimpleButton btnRemoveAttachment;
    private SimpleButton btnDownloadAttachment;
    private SimpleButton btnOpenAttachment;
    private SimpleButton btnSetMainAttachment;
    private LabelControl lblAttachmentMetadataTitle;
    private LabelControl lblAttachmentPublicationTitle;
    private LabelControl lblAttachmentType;
    private LookUpEdit lueAttachmentType;
    private LabelControl lblAttachmentFileName;
    private TextEdit txtAttachmentFileName;
    private LabelControl lblAttachmentDescription;
    private MemoEdit memAttachmentDescription;
    private LabelControl lblAttachmentCategory;
    private LookUpEdit lueAttachmentCategory;
    private LabelControl lblAttachmentPrincipal;
    private LabelControl lblAttachmentVisibleSales;
    private LabelControl lblAttachmentVisiblePurchases;
    private LabelControl lblAttachmentVisiblePortal;
    private LabelControl lblAttachmentConfidential;
    private ToggleSwitch chkVisibleInSales;
    private ToggleSwitch chkVisibleInPurchases;
    private ToggleSwitch chkVisibleInPortal;
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
    private LabelControl lblAttachmentReference;
    private TextEdit txtAttachmentReference;
    private ToggleSwitch chkAttachmentPrincipal;
    private ToggleSwitch chkAttachmentConfidential;
    private LabelControl lblAttachmentOrder;
    private SpinEdit spnAttachmentOrder;
    private LabelControl lblAttachmentValidFrom;
    private DateEdit dteAttachmentValidFrom;
    private LabelControl lblAttachmentValidTo;
    private DateEdit dteAttachmentValidTo;
    private LabelControl lblAttachmentAlternativeText;
    private MemoEdit memAttachmentAlternativeText;
    private LabelControl sepDocumentsColumnOne;
    private LabelControl sepDocumentsColumnTwo;
    private LabelControl sepRemarksColumn;
    private LabelControl sepNotesGeneralTitle;
    private LabelControl sepNotesProcessTitle;
    private LabelControl sepNotesAlertsTitle;
    private LabelControl sepAttachmentPreviewTitle;
    private LabelControl sepAttachmentMetadataTitle;
    private LabelControl sepAttachmentPublicationTitle;
    private LabelControl sepAttachmentGridTitle;
    private LabelControl lblAttachmentPreviewTitle;
    private PictureEdit picMainAttachmentPreview;
    private SimpleButton btnLoadImage;
    private SimpleButton btnRemoveImage;
    private SimpleButton btnPreviewImage;
    private SimpleButton btnSetMainImage;
    private LabelControl lblAttachmentPreviewNoteIcon;
    private LabelControl lblAttachmentPreviewNote;
    private LabelControl sepHeaderData;
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
    private XtraTabPage tabPurchases;
    private LabelControl labelControl3;
    private LabelControl labelControl2;
    private LabelControl labelControl1;
    private LabelControl sepPurchasesColumnOne;
    private LabelControl sepPurchasesColumnTwo;
    private LabelControl sepPurchasesConfiguration;
    private LabelControl sepPurchasesConditions;
    private LabelControl sepPurchasesIndicators;
    private LabelControl sepPurchasesHistory;
    private LabelControl lblPurchasesConditionsTitle;
    private LabelControl lblPurchasesIndicatorsTitle;
    private LabelControl lblPurchaseUnit;
    private LookUpEdit luePurchaseUnit;
    private NuanOperationalKpiCardControl kpiPurchaseCompliance;
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
    private GridView gvPurchaseHistoryAux;
    private LabelControl lblPurchasesConfigurationTitle;
    private NuanOperationalKpiCardControl kpiPurchaseLast;
    private LabelControl lblPurchaseApprovalRequired;
    private NuanOperationalKpiCardControl kpiPurchaseAverage;
    private ToggleSwitch tglPurchaseApprovalRequired;
    private NuanOperationalKpiCardControl kpiPurchaseLeadTime;
    private LabelControl lblSupplierBackorderAllowed;
    private ToggleSwitch tglSupplierBackorderAllowed;
    private MemoEdit memReceivingNote;
    private LabelControl lblPurchaseOnDemand;
    private LabelControl lblReceivingNote;
    private ToggleSwitch tglPurchaseOnDemand;
    private MemoEdit memPurchasePolicy;
    private LabelControl lblPurchasePolicy;
    private LabelControl lblMainPurchaseSupplier;
    private SearchLookUpEdit slueMainPurchaseSupplier;
    private GridView gvMainPurchaseSupplier;
    private LabelControl lblPreferredPurchasePresentation;
    private LookUpEdit luePreferredPurchasePresentation;
    private LabelControl lblPreferredPurchaseCurrency;
    private LookUpEdit luePreferredPurchaseCurrency;
    private LabelControl lblPurchaseMinimumQuantity;
    private SpinEdit spnPurchaseMinimumQuantity;
    private LabelControl lblPurchaseMultiple;
    private SpinEdit spnPurchaseMultiple;
    private LabelControl lblPurchaseDeliveryDays;
    private SpinEdit spnPurchaseDeliveryDays;
    private SimpleButton btnViewPurchaseDocument;
    private SimpleButton btnRefreshPurchases;
    private GridView gridView1;
    private LabelControl labelControl4;
    private LabelControl labelControl7;
    private LabelControl labelControl6;
    private LabelControl labelControl5;
    private LabelControl labelControl8;
}

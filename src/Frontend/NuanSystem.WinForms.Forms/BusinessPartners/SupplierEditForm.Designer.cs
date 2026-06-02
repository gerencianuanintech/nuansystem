using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

partial class SupplierEditForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SupplierEditForm));
        tabSupplier = new XtraTabControl();
        xtpGeneral = new XtraTabPage();
        lblSummaryTitle = new LabelControl();
        pnlSummaryBalance = new PanelControl();
        lblPayableBalanceCaption = new LabelControl();
        lblPayableBalanceValue = new LabelControl();
        pnlSummaryOrders = new PanelControl();
        lblOpenOrdersCaption = new LabelControl();
        lblOpenOrdersValue = new LabelControl();
        pnlSummaryLastPurchase = new PanelControl();
        lblLastPurchaseCaption = new LabelControl();
        lblLastPurchaseValue = new LabelControl();
        pnlSummaryPurchases12m = new PanelControl();
        lblPurchases12mCaption = new LabelControl();
        lblPurchases12mValue = new LabelControl();
        pnlSummarySap = new PanelControl();
        lblSapStatusCaption = new LabelControl();
        lblSapStatusValue = new LabelControl();
        pnlSummaryRetentions = new PanelControl();
        lblRetentionsCaption = new LabelControl();
        lblRetentionsValue = new LabelControl();
        lblCommercialTitle = new LabelControl();
        lblBuyer = new LabelControl();
        lueBuyer = new SearchLookUpEdit();
        grvBuyerLookup = new GridView();
        lblChannel = new LabelControl();
        lueChannel = new LookUpEdit();
        lblSupplyMethod = new LabelControl();
        lueSupplyMethod = new LookUpEdit();
        lblDeliveryDays = new LabelControl();
        spnDeliveryDays = new SpinEdit();
        lblMinimumOrder = new LabelControl();
        spnMinimumOrder = new SpinEdit();
        lblReturnPolicy = new LabelControl();
        memReturnPolicy = new MemoEdit();
        lblClassificationTitle = new LabelControl();
        lblSupplierGroup = new LabelControl();
        lueSupplierGroup = new SearchLookUpEdit();
        grvSupplierGroupLookup = new GridView();
        lblSupplierClass = new LabelControl();
        lueSupplierClass = new SearchLookUpEdit();
        grvSupplierClassLookup = new GridView();
        lblEconomicActivity = new LabelControl();
        lueEconomicActivity = new SearchLookUpEdit();
        grvEconomicActivityLookup = new GridView();
        lblZone = new LabelControl();
        lueZone = new SearchLookUpEdit();
        grvZoneLookup = new GridView();
        lblCountry = new LabelControl();
        lueCountry = new SearchLookUpEdit();
        grvCountryLookup = new GridView();
        lblProvince = new LabelControl();
        lueProvince = new SearchLookUpEdit();
        grvProvinceLookup = new GridView();
        lblCity = new LabelControl();
        lueCity = new SearchLookUpEdit();
        grvCityLookup = new GridView();
        lblPriceList = new LabelControl();
        luePriceList = new SearchLookUpEdit();
        grvPriceListLookup = new GridView();
        lblCreditDays = new LabelControl();
        spnCreditDays = new SpinEdit();
        xtpContacts = new XtraTabPage();
        btnContactClear = new SimpleButton();
        btnContactRemove = new SimpleButton();
        btnContactUpdate = new SimpleButton();
        btnContactAdd = new SimpleButton();
        lblContactListTitle = new LabelControl();
        grdSupplierContacts = new GridControl();
        grvSupplierContacts = new GridView();
        colSupplierContactName = new GridColumn();
        colSupplierContactPosition = new GridColumn();
        colSupplierContactPhone = new GridColumn();
        colSupplierContactExtension = new GridColumn();
        colSupplierContactMobile = new GridColumn();
        colSupplierContactEmail = new GridColumn();
        colSupplierContactPrimary = new GridColumn();
        colSupplierContactActive = new GridColumn();
        lblContactClassificationTitle = new LabelControl();
        lblSupplierContactType = new LabelControl();
        lblContactDataTitle = new LabelControl();
        lueSupplierContactType = new LookUpEdit();
        lblSupplierContactName = new LabelControl();
        lblSupplierContactDepartment = new LabelControl();
        txtSupplierContactName = new TextEdit();
        lueSupplierContactDepartment = new LookUpEdit();
        lblSupplierContactPosition = new LabelControl();
        lblSupplierContactChannel = new LabelControl();
        lueSupplierContactPosition = new LookUpEdit();
        lueSupplierContactChannel = new LookUpEdit();
        lblSupplierContactPhone = new LabelControl();
        lblSupplierContactLanguage = new LabelControl();
        txtSupplierContactPhone = new TextEdit();
        lueSupplierContactLanguage = new LookUpEdit();
        lblSupplierContactExtension = new LabelControl();
        lblSupplierContactNotifications = new LabelControl();
        txtSupplierContactExtension = new TextEdit();
        lueSupplierContactNotifications = new LookUpEdit();
        lblSupplierContactMobile = new LabelControl();
        lblSupplierContactNotes = new LabelControl();
        txtSupplierContactMobile = new TextEdit();
        memSupplierContactNotes = new MemoEdit();
        lblSupplierContactEmail = new LabelControl();
        txtSupplierContactEmail = new TextEdit();
        lblSupplierContactPrincipal = new LabelControl();
        lueSupplierContactPrincipal = new LookUpEdit();
        lblSupplierContactStatus = new LabelControl();
        lueSupplierContactStatus = new LookUpEdit();
        xtpAddresses = new XtraTabPage();
        btnAddressClear = new SimpleButton();
        btnAddressRemove = new SimpleButton();
        btnAddressUpdate = new SimpleButton();
        btnAddressAdd = new SimpleButton();
        lblAddressListTitle = new LabelControl();
        grdSupplierAddresses = new GridControl();
        grvSupplierAddresses = new GridView();
        colSupplierAddressType = new GridColumn();
        colSupplierAddressLine = new GridColumn();
        colSupplierAddressCountry = new GridColumn();
        colSupplierAddressProvince = new GridColumn();
        colSupplierAddressCity = new GridColumn();
        colSupplierAddressPostal = new GridColumn();
        colSupplierAddressPrimary = new GridColumn();
        colSupplierAddressActive = new GridColumn();
        lblAddressMapTitle = new LabelControl();
        lblAddressMapPlaceholder = new LabelControl();
        picAddressMap = new PictureEdit();
        lblAddressGeoTitle = new LabelControl();
        lblSupplierLatitude = new LabelControl();
        lblAddressDataTitle = new LabelControl();
        spnSupplierLatitude = new SpinEdit();
        lblSupplierAddressType = new LabelControl();
        lblSupplierLongitude = new LabelControl();
        lueSupplierAddressType = new LookUpEdit();
        spnSupplierLongitude = new SpinEdit();
        lblSupplierAddressLine1 = new LabelControl();
        lblSupplierAddressReference = new LabelControl();
        txtSupplierAddressLine1 = new TextEdit();
        txtSupplierAddressReference = new TextEdit();
        lblSupplierAddressLine2 = new LabelControl();
        btnValidateCoordinates = new SimpleButton();
        txtSupplierAddressLine2 = new TextEdit();
        btnClearCoordinates = new SimpleButton();
        lblSupplierAddressCountry = new LabelControl();
        lueSupplierAddressCountry = new SearchLookUpEdit();
        grvSupplierAddressCountryLookup = new GridView();
        lblSupplierAddressProvince = new LabelControl();
        lueSupplierAddressProvince = new SearchLookUpEdit();
        grvSupplierAddressProvinceLookup = new GridView();
        lblSupplierAddressCity = new LabelControl();
        lueSupplierAddressCity = new SearchLookUpEdit();
        grvSupplierAddressCityLookup = new GridView();
        lblSupplierAddressPostal = new LabelControl();
        txtSupplierAddressPostal = new TextEdit();
        lblSupplierAddressPrimary = new LabelControl();
        lueSupplierAddressPrimary = new LookUpEdit();
        lblSupplierAddressStatus = new LabelControl();
        lueSupplierAddressStatus = new LookUpEdit();
        xtpPurchases = new XtraTabPage();
        lblPurchaseProductsTitle = new LabelControl();
        grdPurchaseProducts = new GridControl();
        grvPurchaseProducts = new GridView();
        colPurchaseProductCode = new GridColumn();
        colPurchaseProductName = new GridColumn();
        colPurchaseProductUnit = new GridColumn();
        colPurchaseProductLastPrice = new GridColumn();
        colPurchaseProductCurrency = new GridColumn();
        colPurchaseProductLastDate = new GridColumn();
        lblPurchaseDocumentsTitle = new LabelControl();
        grdPurchaseDocuments = new GridControl();
        grvPurchaseDocuments = new GridView();
        colPurchaseDocumentDate = new GridColumn();
        colPurchaseDocumentType = new GridColumn();
        colPurchaseDocumentNumber = new GridColumn();
        colPurchaseDocumentStatus = new GridColumn();
        colPurchaseDocumentTotal = new GridColumn();
        colPurchaseDocumentCurrency = new GridColumn();
        colPurchaseDocumentSap = new GridColumn();
        lblPurchaseStatsTitle = new LabelControl();
        lblPurchaseLastDateCaption = new LabelControl();
        lblPurchaseLastDateValue = new LabelControl();
        lblPurchase12mCaption = new LabelControl();
        lblPurchase12mValue = new LabelControl();
        lblPurchaseOpenOrdersCaption = new LabelControl();
        lblPurchaseOpenOrdersValue = new LabelControl();
        lblPurchasePayableCaption = new LabelControl();
        lblPurchasePayableValue = new LabelControl();
        lblPurchaseAvgDeliveryCaption = new LabelControl();
        lblPurchaseAvgDeliveryValue = new LabelControl();
        lblPurchaseComplianceCaption = new LabelControl();
        lblPurchaseComplianceValue = new LabelControl();
        lblAllowBackorder = new LabelControl();
        tsAllowSales = new ToggleSwitch();
        lblPurchaseConditionsTitle = new LabelControl();
        lblPurchasePaymentTerm = new LabelControl();
        luePurchasePaymentTerm = new SearchLookUpEdit();
        grvPurchasePaymentTermLookup = new GridView();
        lblPurchaseCurrency = new LabelControl();
        lblCreditLimit = new LabelControl();
        spnCreditLimit = new SpinEdit();
        luePurchaseCurrency = new SearchLookUpEdit();
        grvPurchaseCurrencyLookup = new GridView();
        lblPurchaseBuyer = new LabelControl();
        luePurchaseBuyer = new SearchLookUpEdit();
        grvPurchaseBuyerLookup = new GridView();
        xtpBanks = new XtraTabPage();
        lblBankAccountsTitle = new LabelControl();
        grdBankAccounts = new GridControl();
        grvBankAccounts = new GridView();
        colBankName = new GridColumn();
        colBankAccountType = new GridColumn();
        colBankAccountNumber = new GridColumn();
        colBankHolder = new GridColumn();
        colBankIdentification = new GridColumn();
        colBankCurrency = new GridColumn();
        colBankPrimary = new GridColumn();
        colBankActive = new GridColumn();
        btnBankClear = new SimpleButton();
        btnAddressClear0 = new SimpleButton();
        btnAddressClear1 = new SimpleButton();
        btnAddressClear2 = new SimpleButton();
        lblBankTransferTitle = new LabelControl();
        lblBankSwift = new LabelControl();
        lblBankDataTitle = new LabelControl();
        txtBankSwift = new TextEdit();
        lblBankName = new LabelControl();
        lblBankAba = new LabelControl();
        txtBankAba = new TextEdit();
        lueBankName = new SearchLookUpEdit();
        grvBankNameLookup = new GridView();
        lblBankIban = new LabelControl();
        txtBankIban = new TextEdit();
        lblBankAccountType = new LabelControl();
        lblBankCountry = new LabelControl();
        lueBankAccountType = new LookUpEdit();
        lueBankCountry = new SearchLookUpEdit();
        grvBankCountryLookup = new GridView();
        lueBankStatus = new LookUpEdit();
        lblBankCity = new LabelControl();
        lblBankAccountNumber = new LabelControl();
        lueBankCity = new SearchLookUpEdit();
        grvBankCityLookup = new GridView();
        lblBankStatus = new LabelControl();
        lblBankNotes = new LabelControl();
        txtBankAccountNumber = new TextEdit();
        memBankNotes = new MemoEdit();
        lueBankPrimary = new LookUpEdit();
        lblBankHolder = new LabelControl();
        lblBankPrimary = new LabelControl();
        txtBankHolder = new TextEdit();
        lueBankCurrency = new SearchLookUpEdit();
        grvBankCurrencyLookup = new GridView();
        lblBankHolderIdentification = new LabelControl();
        lblBankCurrency = new LabelControl();
        txtBankHolderIdentification = new TextEdit();
        xtpAccounting = new XtraTabPage();
        lblAccountingDimensionsTitle = new LabelControl();
        lblAccountingBranch = new LabelControl();
        lueAccountingBranch = new SearchLookUpEdit();
        grvAccountingBranchLookup = new GridView();
        lblAccountingDepartment = new LabelControl();
        lueAccountingDepartment = new SearchLookUpEdit();
        grvAccountingDepartmentLookup = new GridView();
        lblAccountingBusinessLine = new LabelControl();
        lueAccountingBusinessLine = new SearchLookUpEdit();
        grvAccountingBusinessLineLookup = new GridView();
        lblAccountingCostCenter = new LabelControl();
        lueAccountingCostCenter = new SearchLookUpEdit();
        grvAccountingCostCenterLookup = new GridView();
        lblAccountingProject = new LabelControl();
        lueAccountingProject = new SearchLookUpEdit();
        grvAccountingProjectLookup = new GridView();
        chkAccountingConciliationRequired = new ToggleSwitch();
        spnAccountingPaymentTolerance = new SpinEdit();
        lblAccountingPaymentTolerance = new LabelControl();
        lblAccountingAveragePaymentDays = new LabelControl();
        spnAccountingAveragePaymentDays = new SpinEdit();
        lblAccountingPaymentMethod = new LabelControl();
        chkAccountingUsesWithholdingBase = new ToggleSwitch();
        lueAccountingPaymentMethod = new LookUpEdit();
        lblAccountingPaymentPriority = new LabelControl();
        chkAccountingBlocked = new ToggleSwitch();
        lueAccountingPaymentPriority = new LookUpEdit();
        chkAccountingAllowsPartialPayments = new ToggleSwitch();
        lblAccountingRequiredPaymentDay = new LabelControl();
        lblAccountingConciliationRequired = new LabelControl();
        lueAccountingRequiredPaymentDay = new LookUpEdit();
        lblAccountingUsesWithholdingBase = new LabelControl();
        lblAccountingPaymentDocumentType = new LabelControl();
        lblAccountingBlocked = new LabelControl();
        lueAccountingPaymentDocumentType = new LookUpEdit();
        lblAccountingAllowsPartialPayments = new LabelControl();
        lblAccountingApprovalFlow = new LabelControl();
        lblAccountingAllowsCompensation = new LabelControl();
        lueAccountingApprovalFlow = new LookUpEdit();
        lblAccountingAllowsAdvance = new LabelControl();
        lblAccountingRequiresProvision = new LabelControl();
        lblAccountingBySupplier = new LabelControl();
        chkAccountingAllowsCompensation = new ToggleSwitch();
        chkAccountingAllowsAdvance = new ToggleSwitch();
        chkAccountingRequiresProvision = new ToggleSwitch();
        chkAccountingBySupplier = new ToggleSwitch();
        lueAccountingRetentionPayableAccount = new SearchLookUpEdit();
        grvAccountingRetentionPayableAccountLookup = new GridView();
        lueAccountingDiscountAccount = new SearchLookUpEdit();
        grvAccountingDiscountAccountLookup = new GridView();
        lueAccountingClearingAccount = new SearchLookUpEdit();
        grvAccountingClearingAccountLookup = new GridView();
        lueAccountingRoundingAccount = new SearchLookUpEdit();
        grvAccountingRoundingAccountLookup = new GridView();
        lueAccountingDifferenceAccount = new SearchLookUpEdit();
        grvAccountingDifferenceAccountLookup = new GridView();
        lueAccountingAdvanceAccount = new SearchLookUpEdit();
        grvAccountingAdvanceAccountLookup = new GridView();
        lueAccountingDefaultExpenseAccount = new SearchLookUpEdit();
        grvAccountingDefaultExpenseAccountLookup = new GridView();
        lueAccountingSupplierAccount = new SearchLookUpEdit();
        grvAccountingSupplierAccountLookup = new GridView();
        lblAccountingAccountsTitle = new LabelControl();
        lblAccountingSupplierAccount = new LabelControl();
        lblAccountingAdvanceAccount = new LabelControl();
        lblAccountingDefaultExpenseAccount = new LabelControl();
        lblAccountingDifferenceAccount = new LabelControl();
        lblAccountingRetentionPayableAccount = new LabelControl();
        lblAccountingRoundingAccount = new LabelControl();
        lblAccountingClearingAccount = new LabelControl();
        lblAccountingDiscountAccount = new LabelControl();
        xtpRetentions = new XtraTabPage();
        btnAddressClear3 = new SimpleButton();
        btnAddressClear4 = new SimpleButton();
        btnAddressClear5 = new SimpleButton();
        btnAddressClear6 = new SimpleButton();
        lblRetentionRulesTitle = new LabelControl();
        grdRetentionRules = new GridControl();
        grvRetentionRules = new GridView();
        colRetentionCode = new GridColumn();
        colRetentionConcept = new GridColumn();
        colRetentionType = new GridColumn();
        colRetentionPercent = new GridColumn();
        colRetentionValidFrom = new GridColumn();
        colRetentionActive = new GridColumn();
        lblRetentionTaxConfigTitle = new LabelControl();
        lblRetentionAccountingRequired = new LabelControl();
        lblRetentionEntryTitle = new LabelControl();
        lueRetentionAccountingRequired = new LookUpEdit();
        lblRetentionAgentConfig = new LabelControl();
        lblRetentionEntryType = new LabelControl();
        lueRetentionAgentConfig = new LookUpEdit();
        lueRetentionEntryType = new LookUpEdit();
        lblRetentionFiscalRegime = new LabelControl();
        lueRetentionFiscalRegime = new LookUpEdit();
        lblRetentionEntrySriCode = new LabelControl();
        lblRetentionSpecialTaxpayer = new LabelControl();
        lueRetentionEntrySriCode = new LookUpEdit();
        lueRetentionSpecialTaxpayer = new LookUpEdit();
        lblRetentionEntryPercent = new LabelControl();
        lblRetentionTaxpayerType = new LabelControl();
        lueRetentionEntryCurrent = new LookUpEdit();
        lueRetentionTaxpayerType = new LookUpEdit();
        spnRetentionEntryPercent = new SpinEdit();
        lblRetentionFiscalCountry = new LabelControl();
        lblRetentionEntryCurrent = new LabelControl();
        lueRetentionFiscalCountry = new SearchLookUpEdit();
        grvRetentionFiscalCountryLookup = new GridView();
        lblRetentionEntryAccount = new LabelControl();
        lueRetentionEntryAppliesIncome = new LookUpEdit();
        lueRetentionEntryAccount = new SearchLookUpEdit();
        grvRetentionEntryAccountLookup = new GridView();
        lblRetentionEntryAppliesIncome = new LabelControl();
        lblRetentionEntrySupport = new LabelControl();
        lueRetentionEntryAppliesIva = new LookUpEdit();
        lueRetentionEntrySupport = new LookUpEdit();
        lblRetentionEntryAppliesIva = new LabelControl();
        xtpSap = new XtraTabPage();
        btnAddressRemove1 = new SimpleButton();
        btnAddressRemove2 = new SimpleButton();
        btnAddressRemove3 = new SimpleButton();
        btnAddressRemove4 = new SimpleButton();
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
        xtpNotes = new XtraTabPage();
        btnAddressClear8 = new SimpleButton();
        btnAddressClear9 = new SimpleButton();
        btnAddressRemove0 = new SimpleButton();
        lblAttachmentsTitle = new LabelControl();
        grdSupplierAttachments = new GridControl();
        grvSupplierAttachments = new GridView();
        colAttachmentType = new GridColumn();
        colAttachmentFileName = new GridColumn();
        colAttachmentDescription = new GridColumn();
        colAttachmentDate = new GridColumn();
        colAttachmentUser = new GridColumn();
        colAttachmentStatus = new GridColumn();
        lblNotesGeneralTitle = new LabelControl();
        lblSupplierInternalNotes = new LabelControl();
        memSupplierInternalNotes = new MemoEdit();
        lblSupplierPurchasingNotes = new LabelControl();
        memSupplierPurchasingNotes = new MemoEdit();
        lblSupplierPaymentNotes = new LabelControl();
        memSupplierPaymentNotes = new MemoEdit();
        lblSupplierOperationalAlert = new LabelControl();
        txtSupplierOperationalAlert = new TextEdit();
        lblCode = new LabelControl();
        lblSapHeaderStatus = new LabelControl();
        lblSapHeaderStatusValue = new LabelControl();
        lblPayableHeader = new LabelControl();
        lblPayableHeaderValue = new LabelControl();
        txtSupplierCode = new TextEdit();
        lblSupplierName = new LabelControl();
        lblSupplierType = new LabelControl();
        lblIdentificationType = new LabelControl();
        lueIdentificationType = new SearchLookUpEdit();
        grvIdentificationTypeLookup = new GridView();
        lblIdentificationNumber = new LabelControl();
        txtIdentificationNumber = new TextEdit();
        txtSupplierName = new TextEdit();
        lblSupplierCommercialName = new LabelControl();
        txtSupplierCommercialName = new TextEdit();
        lueSupplierType = new LookUpEdit();
        lblStatus = new LabelControl();
        btnStatusToggle = new SimpleButton();
        btnSave = new SimpleButton();
        btnCancel = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)tabSupplier).BeginInit();
        tabSupplier.SuspendLayout();
        xtpGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummaryBalance).BeginInit();
        pnlSummaryBalance.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummaryOrders).BeginInit();
        pnlSummaryOrders.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummaryLastPurchase).BeginInit();
        pnlSummaryLastPurchase.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummaryPurchases12m).BeginInit();
        pnlSummaryPurchases12m.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummarySap).BeginInit();
        pnlSummarySap.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummaryRetentions).BeginInit();
        pnlSummaryRetentions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueBuyer.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvBuyerLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueChannel.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplyMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnDeliveryDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memReturnPolicy.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierGroup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierGroupLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierClass.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierClassLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueEconomicActivity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvEconomicActivityLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueZone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvZoneLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvCountryLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueProvince.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvProvinceLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvCityLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePriceList.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvPriceListLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditDays.Properties).BeginInit();
        xtpContacts.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdSupplierContacts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierContacts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContactName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactDepartment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactPosition.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactChannel.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContactPhone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactLanguage.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContactExtension.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactNotifications.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContactMobile.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memSupplierContactNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContactEmail.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactPrincipal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactStatus.Properties).BeginInit();
        xtpAddresses.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdSupplierAddresses).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierAddresses).BeginInit();
        ((System.ComponentModel.ISupportInitialize)picAddressMap.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSupplierLatitude.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnSupplierLongitude.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierAddressLine1.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierAddressReference.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierAddressLine2.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierAddressCountryLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressProvince.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierAddressProvinceLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressCity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierAddressCityLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierAddressPostal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressPrimary.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressStatus.Properties).BeginInit();
        xtpPurchases.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdPurchaseProducts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvPurchaseProducts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdPurchaseDocuments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvPurchaseDocuments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsAllowSales.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchasePaymentTerm.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvPurchasePaymentTermLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditLimit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvPurchaseCurrencyLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseBuyer.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvPurchaseBuyerLookup).BeginInit();
        xtpBanks.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdBankAccounts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvBankAccounts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBankSwift.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBankAba.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBankName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvBankNameLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBankIban.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBankAccountType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBankCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvBankCountryLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBankStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBankCity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvBankCityLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBankAccountNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memBankNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBankPrimary.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBankHolder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBankCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvBankCurrencyLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBankHolderIdentification.Properties).BeginInit();
        xtpAccounting.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueAccountingBranch.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingBranchLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingDepartment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingDepartmentLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingBusinessLine.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingBusinessLineLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingCostCenter.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingCostCenterLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingProject.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingProjectLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingConciliationRequired.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnAccountingPaymentTolerance.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnAccountingAveragePaymentDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingUsesWithholdingBase.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingPaymentMethod.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingBlocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingPaymentPriority.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingAllowsPartialPayments.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingRequiredPaymentDay.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingPaymentDocumentType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingApprovalFlow.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingAllowsCompensation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingAllowsAdvance.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingRequiresProvision.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingBySupplier.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingRetentionPayableAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingRetentionPayableAccountLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingDiscountAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingDiscountAccountLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingClearingAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingClearingAccountLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingRoundingAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingRoundingAccountLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingDifferenceAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingDifferenceAccountLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingAdvanceAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingAdvanceAccountLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingDefaultExpenseAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingDefaultExpenseAccountLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingSupplierAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingSupplierAccountLookup).BeginInit();
        xtpRetentions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdRetentionRules).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvRetentionRules).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionAccountingRequired.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionAgentConfig.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntryType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionFiscalRegime.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntrySriCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionSpecialTaxpayer.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntryCurrent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionTaxpayerType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnRetentionEntryPercent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionFiscalCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvRetentionFiscalCountryLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntryAppliesIncome.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntryAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvRetentionEntryAccountLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntryAppliesIva.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntrySupport.Properties).BeginInit();
        xtpSap.SuspendLayout();
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
        xtpNotes.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdSupplierAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memSupplierInternalNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memSupplierPurchasingNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memSupplierPaymentNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierOperationalAlert.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueIdentificationType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvIdentificationTypeLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtIdentificationNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierCommercialName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierType.Properties).BeginInit();
        SuspendLayout();
        // 
        // tabSupplier
        // 
        tabSupplier.AppearancePage.Header.Font = new Font("Segoe UI", 9F);
        tabSupplier.AppearancePage.Header.Options.UseFont = true;
        tabSupplier.AppearancePage.Header.Options.UseTextOptions = true;
        tabSupplier.AppearancePage.Header.TextOptions.HAlignment = HorzAlignment.Center;
        tabSupplier.AppearancePage.HeaderActive.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        tabSupplier.AppearancePage.HeaderActive.Options.UseFont = true;
        tabSupplier.AppearancePage.HeaderActive.Options.UseTextOptions = true;
        tabSupplier.AppearancePage.HeaderActive.TextOptions.HAlignment = HorzAlignment.Center;
        tabSupplier.Location = new Point(12, 177);
        tabSupplier.Name = "tabSupplier";
        tabSupplier.SelectedTabPage = xtpGeneral;
        tabSupplier.Size = new Size(1096, 429);
        tabSupplier.TabIndex = 12;
        tabSupplier.TabPages.AddRange(new XtraTabPage[] { xtpGeneral, xtpContacts, xtpAddresses, xtpPurchases, xtpBanks, xtpAccounting, xtpRetentions, xtpSap, xtpNotes });
        tabSupplier.TabPageWidth = 120;
        // 
        // xtpGeneral
        // 
        xtpGeneral.Appearance.Header.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        xtpGeneral.Appearance.Header.Options.UseFont = true;
        xtpGeneral.Controls.Add(lblSummaryTitle);
        xtpGeneral.Controls.Add(pnlSummaryBalance);
        xtpGeneral.Controls.Add(pnlSummaryOrders);
        xtpGeneral.Controls.Add(pnlSummaryLastPurchase);
        xtpGeneral.Controls.Add(pnlSummaryPurchases12m);
        xtpGeneral.Controls.Add(pnlSummarySap);
        xtpGeneral.Controls.Add(pnlSummaryRetentions);
        xtpGeneral.Controls.Add(lblCommercialTitle);
        xtpGeneral.Controls.Add(lblBuyer);
        xtpGeneral.Controls.Add(lueBuyer);
        xtpGeneral.Controls.Add(lblChannel);
        xtpGeneral.Controls.Add(lueChannel);
        xtpGeneral.Controls.Add(lblSupplyMethod);
        xtpGeneral.Controls.Add(lueSupplyMethod);
        xtpGeneral.Controls.Add(lblDeliveryDays);
        xtpGeneral.Controls.Add(spnDeliveryDays);
        xtpGeneral.Controls.Add(lblMinimumOrder);
        xtpGeneral.Controls.Add(spnMinimumOrder);
        xtpGeneral.Controls.Add(lblReturnPolicy);
        xtpGeneral.Controls.Add(memReturnPolicy);
        xtpGeneral.Controls.Add(lblClassificationTitle);
        xtpGeneral.Controls.Add(lblSupplierGroup);
        xtpGeneral.Controls.Add(lueSupplierGroup);
        xtpGeneral.Controls.Add(lblSupplierClass);
        xtpGeneral.Controls.Add(lueSupplierClass);
        xtpGeneral.Controls.Add(lblEconomicActivity);
        xtpGeneral.Controls.Add(lueEconomicActivity);
        xtpGeneral.Controls.Add(lblZone);
        xtpGeneral.Controls.Add(lueZone);
        xtpGeneral.Controls.Add(lblCountry);
        xtpGeneral.Controls.Add(lueCountry);
        xtpGeneral.Controls.Add(lblProvince);
        xtpGeneral.Controls.Add(lueProvince);
        xtpGeneral.Controls.Add(lblCity);
        xtpGeneral.Controls.Add(lueCity);
        xtpGeneral.Controls.Add(lblPriceList);
        xtpGeneral.Controls.Add(luePriceList);
        xtpGeneral.Controls.Add(lblCreditDays);
        xtpGeneral.Controls.Add(spnCreditDays);
        xtpGeneral.Name = "xtpGeneral";
        xtpGeneral.Size = new Size(1094, 402);
        xtpGeneral.Text = "General";
        // 
        // lblSummaryTitle
        // 
        lblSummaryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSummaryTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSummaryTitle.Appearance.Options.UseFont = true;
        lblSummaryTitle.Appearance.Options.UseForeColor = true;
        lblSummaryTitle.Location = new Point(773, 12);
        lblSummaryTitle.Name = "lblSummaryTitle";
        lblSummaryTitle.Size = new Size(179, 20);
        lblSummaryTitle.TabIndex = 63;
        lblSummaryTitle.Text = "3. Resumen del proveedor";
        // 
        // pnlSummaryBalance
        // 
        pnlSummaryBalance.Controls.Add(lblPayableBalanceCaption);
        pnlSummaryBalance.Controls.Add(lblPayableBalanceValue);
        pnlSummaryBalance.Location = new Point(773, 49);
        pnlSummaryBalance.Name = "pnlSummaryBalance";
        pnlSummaryBalance.Size = new Size(151, 72);
        pnlSummaryBalance.TabIndex = 64;
        // 
        // lblPayableBalanceCaption
        // 
        lblPayableBalanceCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblPayableBalanceCaption.Appearance.Options.UseFont = true;
        lblPayableBalanceCaption.Location = new Point(16, 12);
        lblPayableBalanceCaption.Name = "lblPayableBalanceCaption";
        lblPayableBalanceCaption.Size = new Size(83, 15);
        lblPayableBalanceCaption.TabIndex = 0;
        lblPayableBalanceCaption.Text = "Saldo por pagar";
        // 
        // lblPayableBalanceValue
        // 
        lblPayableBalanceValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblPayableBalanceValue.Appearance.Options.UseFont = true;
        lblPayableBalanceValue.Location = new Point(16, 34);
        lblPayableBalanceValue.Name = "lblPayableBalanceValue";
        lblPayableBalanceValue.Size = new Size(83, 25);
        lblPayableBalanceValue.TabIndex = 1;
        lblPayableBalanceValue.Text = "12,475.60";
        // 
        // pnlSummaryOrders
        // 
        pnlSummaryOrders.Controls.Add(lblOpenOrdersCaption);
        pnlSummaryOrders.Controls.Add(lblOpenOrdersValue);
        pnlSummaryOrders.Location = new Point(930, 49);
        pnlSummaryOrders.Name = "pnlSummaryOrders";
        pnlSummaryOrders.Size = new Size(146, 72);
        pnlSummaryOrders.TabIndex = 65;
        // 
        // lblOpenOrdersCaption
        // 
        lblOpenOrdersCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblOpenOrdersCaption.Appearance.Options.UseFont = true;
        lblOpenOrdersCaption.Location = new Point(16, 12);
        lblOpenOrdersCaption.Name = "lblOpenOrdersCaption";
        lblOpenOrdersCaption.Size = new Size(87, 15);
        lblOpenOrdersCaption.TabIndex = 0;
        lblOpenOrdersCaption.Text = "Pedidos abiertos";
        // 
        // lblOpenOrdersValue
        // 
        lblOpenOrdersValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblOpenOrdersValue.Appearance.Options.UseFont = true;
        lblOpenOrdersValue.Location = new Point(16, 34);
        lblOpenOrdersValue.Name = "lblOpenOrdersValue";
        lblOpenOrdersValue.Size = new Size(11, 25);
        lblOpenOrdersValue.TabIndex = 1;
        lblOpenOrdersValue.Text = "5";
        // 
        // pnlSummaryLastPurchase
        // 
        pnlSummaryLastPurchase.Controls.Add(lblLastPurchaseCaption);
        pnlSummaryLastPurchase.Controls.Add(lblLastPurchaseValue);
        pnlSummaryLastPurchase.Location = new Point(773, 132);
        pnlSummaryLastPurchase.Name = "pnlSummaryLastPurchase";
        pnlSummaryLastPurchase.Size = new Size(151, 72);
        pnlSummaryLastPurchase.TabIndex = 66;
        // 
        // lblLastPurchaseCaption
        // 
        lblLastPurchaseCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblLastPurchaseCaption.Appearance.Options.UseFont = true;
        lblLastPurchaseCaption.Location = new Point(16, 12);
        lblLastPurchaseCaption.Name = "lblLastPurchaseCaption";
        lblLastPurchaseCaption.Size = new Size(79, 15);
        lblLastPurchaseCaption.TabIndex = 0;
        lblLastPurchaseCaption.Text = "Ultima compra";
        // 
        // lblLastPurchaseValue
        // 
        lblLastPurchaseValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblLastPurchaseValue.Appearance.Options.UseFont = true;
        lblLastPurchaseValue.Location = new Point(16, 34);
        lblLastPurchaseValue.Name = "lblLastPurchaseValue";
        lblLastPurchaseValue.Size = new Size(101, 25);
        lblLastPurchaseValue.TabIndex = 1;
        lblLastPurchaseValue.Text = "15/05/2026";
        // 
        // pnlSummaryPurchases12m
        // 
        pnlSummaryPurchases12m.Controls.Add(lblPurchases12mCaption);
        pnlSummaryPurchases12m.Controls.Add(lblPurchases12mValue);
        pnlSummaryPurchases12m.Location = new Point(930, 132);
        pnlSummaryPurchases12m.Name = "pnlSummaryPurchases12m";
        pnlSummaryPurchases12m.Size = new Size(146, 72);
        pnlSummaryPurchases12m.TabIndex = 67;
        // 
        // lblPurchases12mCaption
        // 
        lblPurchases12mCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchases12mCaption.Appearance.Options.UseFont = true;
        lblPurchases12mCaption.Location = new Point(16, 12);
        lblPurchases12mCaption.Name = "lblPurchases12mCaption";
        lblPurchases12mCaption.Size = new Size(74, 15);
        lblPurchases12mCaption.TabIndex = 0;
        lblPurchases12mCaption.Text = "Compras 12m";
        // 
        // lblPurchases12mValue
        // 
        lblPurchases12mValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblPurchases12mValue.Appearance.Options.UseFont = true;
        lblPurchases12mValue.Location = new Point(16, 34);
        lblPurchases12mValue.Name = "lblPurchases12mValue";
        lblPurchases12mValue.Size = new Size(94, 25);
        lblPurchases12mValue.TabIndex = 1;
        lblPurchases12mValue.Text = "128,450.75";
        // 
        // pnlSummarySap
        // 
        pnlSummarySap.Controls.Add(lblSapStatusCaption);
        pnlSummarySap.Controls.Add(lblSapStatusValue);
        pnlSummarySap.Location = new Point(773, 215);
        pnlSummarySap.Name = "pnlSummarySap";
        pnlSummarySap.Size = new Size(151, 72);
        pnlSummarySap.TabIndex = 68;
        // 
        // lblSapStatusCaption
        // 
        lblSapStatusCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapStatusCaption.Appearance.Options.UseFont = true;
        lblSapStatusCaption.Location = new Point(16, 12);
        lblSapStatusCaption.Name = "lblSapStatusCaption";
        lblSapStatusCaption.Size = new Size(59, 15);
        lblSapStatusCaption.TabIndex = 0;
        lblSapStatusCaption.Text = "Estado SAP";
        // 
        // lblSapStatusValue
        // 
        lblSapStatusValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblSapStatusValue.Appearance.ForeColor = Color.FromArgb(22, 163, 74);
        lblSapStatusValue.Appearance.Options.UseFont = true;
        lblSapStatusValue.Appearance.Options.UseForeColor = true;
        lblSapStatusValue.Location = new Point(16, 34);
        lblSapStatusValue.Name = "lblSapStatusValue";
        lblSapStatusValue.Size = new Size(110, 25);
        lblSapStatusValue.TabIndex = 1;
        lblSapStatusValue.Text = "Sincronizado";
        // 
        // pnlSummaryRetentions
        // 
        pnlSummaryRetentions.Controls.Add(lblRetentionsCaption);
        pnlSummaryRetentions.Controls.Add(lblRetentionsValue);
        pnlSummaryRetentions.Location = new Point(930, 215);
        pnlSummaryRetentions.Name = "pnlSummaryRetentions";
        pnlSummaryRetentions.Size = new Size(146, 72);
        pnlSummaryRetentions.TabIndex = 69;
        // 
        // lblRetentionsCaption
        // 
        lblRetentionsCaption.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionsCaption.Appearance.Options.UseFont = true;
        lblRetentionsCaption.Location = new Point(16, 12);
        lblRetentionsCaption.Name = "lblRetentionsCaption";
        lblRetentionsCaption.Size = new Size(103, 15);
        lblRetentionsCaption.TabIndex = 0;
        lblRetentionsCaption.Text = "Retenciones activas";
        // 
        // lblRetentionsValue
        // 
        lblRetentionsValue.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblRetentionsValue.Appearance.Options.UseFont = true;
        lblRetentionsValue.Location = new Point(16, 34);
        lblRetentionsValue.Name = "lblRetentionsValue";
        lblRetentionsValue.Size = new Size(11, 25);
        lblRetentionsValue.TabIndex = 1;
        lblRetentionsValue.Text = "3";
        // 
        // lblCommercialTitle
        // 
        lblCommercialTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblCommercialTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblCommercialTitle.Appearance.Options.UseFont = true;
        lblCommercialTitle.Appearance.Options.UseForeColor = true;
        lblCommercialTitle.Location = new Point(402, 12);
        lblCommercialTitle.Name = "lblCommercialTitle";
        lblCommercialTitle.Size = new Size(169, 20);
        lblCommercialTitle.TabIndex = 50;
        lblCommercialTitle.Text = "2. Informacion comercial";
        // 
        // lblBuyer
        // 
        lblBuyer.Appearance.Font = new Font("Segoe UI", 9F);
        lblBuyer.Appearance.Options.UseFont = true;
        lblBuyer.Location = new Point(406, 52);
        lblBuyer.Name = "lblBuyer";
        lblBuyer.Size = new Size(103, 15);
        lblBuyer.TabIndex = 51;
        lblBuyer.Text = "Asesor de compras:";
        // 
        // lueBuyer
        // 
        lueBuyer.Location = new Point(536, 49);
        lueBuyer.Name = "lueBuyer";
        lueBuyer.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBuyer.Properties.Appearance.Options.UseFont = true;
        lueBuyer.Properties.AutoHeight = false;
        lueBuyer.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBuyer.Properties.NullText = "";
        lueBuyer.Properties.PopupView = grvBuyerLookup;
        lueBuyer.Size = new Size(170, 22);
        lueBuyer.TabIndex = 57;
        // 
        // grvBuyerLookup
        // 
        grvBuyerLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvBuyerLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvBuyerLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvBuyerLookup.Appearance.Row.Options.UseFont = true;
        grvBuyerLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvBuyerLookup.Name = "grvBuyerLookup";
        grvBuyerLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvBuyerLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblChannel
        // 
        lblChannel.Appearance.Font = new Font("Segoe UI", 9F);
        lblChannel.Appearance.Options.UseFont = true;
        lblChannel.Location = new Point(406, 80);
        lblChannel.Name = "lblChannel";
        lblChannel.Size = new Size(33, 15);
        lblChannel.TabIndex = 52;
        lblChannel.Text = "Canal:";
        // 
        // lueChannel
        // 
        lueChannel.Location = new Point(536, 77);
        lueChannel.Name = "lueChannel";
        lueChannel.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueChannel.Properties.Appearance.Options.UseFont = true;
        lueChannel.Properties.AutoHeight = false;
        lueChannel.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueChannel.Properties.NullText = "";
        lueChannel.Size = new Size(170, 22);
        lueChannel.TabIndex = 58;
        // 
        // lblSupplyMethod
        // 
        lblSupplyMethod.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplyMethod.Appearance.Options.UseFont = true;
        lblSupplyMethod.Location = new Point(406, 108);
        lblSupplyMethod.Name = "lblSupplyMethod";
        lblSupplyMethod.Size = new Size(121, 15);
        lblSupplyMethod.TabIndex = 53;
        lblSupplyMethod.Text = "Forma abastecimiento:";
        // 
        // lueSupplyMethod
        // 
        lueSupplyMethod.Location = new Point(536, 105);
        lueSupplyMethod.Name = "lueSupplyMethod";
        lueSupplyMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplyMethod.Properties.Appearance.Options.UseFont = true;
        lueSupplyMethod.Properties.AutoHeight = false;
        lueSupplyMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplyMethod.Properties.NullText = "";
        lueSupplyMethod.Size = new Size(170, 22);
        lueSupplyMethod.TabIndex = 59;
        // 
        // lblDeliveryDays
        // 
        lblDeliveryDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblDeliveryDays.Appearance.Options.UseFont = true;
        lblDeliveryDays.Location = new Point(406, 136);
        lblDeliveryDays.Name = "lblDeliveryDays";
        lblDeliveryDays.Size = new Size(119, 15);
        lblDeliveryDays.TabIndex = 54;
        lblDeliveryDays.Text = "Tiempo entrega (dias):";
        // 
        // spnDeliveryDays
        // 
        spnDeliveryDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnDeliveryDays.Location = new Point(536, 133);
        spnDeliveryDays.Name = "spnDeliveryDays";
        spnDeliveryDays.Properties.Appearance.Options.UseTextOptions = true;
        spnDeliveryDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnDeliveryDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnDeliveryDays.Properties.IsFloatValue = false;
        spnDeliveryDays.Properties.MaskSettings.Set("mask", "N00");
        spnDeliveryDays.Size = new Size(170, 20);
        spnDeliveryDays.TabIndex = 60;
        // 
        // lblMinimumOrder
        // 
        lblMinimumOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumOrder.Appearance.Options.UseFont = true;
        lblMinimumOrder.Location = new Point(406, 162);
        lblMinimumOrder.Name = "lblMinimumOrder";
        lblMinimumOrder.Size = new Size(85, 15);
        lblMinimumOrder.TabIndex = 55;
        lblMinimumOrder.Text = "Pedido minimo:";
        // 
        // spnMinimumOrder
        // 
        spnMinimumOrder.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnMinimumOrder.Location = new Point(536, 159);
        spnMinimumOrder.Name = "spnMinimumOrder";
        spnMinimumOrder.Properties.Appearance.Options.UseTextOptions = true;
        spnMinimumOrder.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnMinimumOrder.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMinimumOrder.Properties.DisplayFormat.FormatString = "n2";
        spnMinimumOrder.Properties.DisplayFormat.FormatType = FormatType.Numeric;
        spnMinimumOrder.Properties.EditFormat.FormatString = "n2";
        spnMinimumOrder.Properties.EditFormat.FormatType = FormatType.Numeric;
        spnMinimumOrder.Properties.MaskSettings.Set("mask", "n2");
        spnMinimumOrder.Size = new Size(170, 20);
        spnMinimumOrder.TabIndex = 61;
        // 
        // lblReturnPolicy
        // 
        lblReturnPolicy.Appearance.Font = new Font("Segoe UI", 9F);
        lblReturnPolicy.Appearance.Options.UseFont = true;
        lblReturnPolicy.Location = new Point(406, 188);
        lblReturnPolicy.Name = "lblReturnPolicy";
        lblReturnPolicy.Size = new Size(104, 15);
        lblReturnPolicy.TabIndex = 56;
        lblReturnPolicy.Text = "Politica devolucion:";
        // 
        // memReturnPolicy
        // 
        memReturnPolicy.Location = new Point(536, 185);
        memReturnPolicy.Name = "memReturnPolicy";
        memReturnPolicy.Size = new Size(170, 108);
        memReturnPolicy.TabIndex = 62;
        // 
        // lblClassificationTitle
        // 
        lblClassificationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblClassificationTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblClassificationTitle.Appearance.Options.UseFont = true;
        lblClassificationTitle.Appearance.Options.UseForeColor = true;
        lblClassificationTitle.Location = new Point(12, 12);
        lblClassificationTitle.Name = "lblClassificationTitle";
        lblClassificationTitle.Size = new Size(183, 20);
        lblClassificationTitle.TabIndex = 31;
        lblClassificationTitle.Text = "1. Clasificacion y operacion";
        // 
        // lblSupplierGroup
        // 
        lblSupplierGroup.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierGroup.Appearance.Options.UseFont = true;
        lblSupplierGroup.Location = new Point(16, 52);
        lblSupplierGroup.Name = "lblSupplierGroup";
        lblSupplierGroup.Size = new Size(109, 15);
        lblSupplierGroup.TabIndex = 32;
        lblSupplierGroup.Text = "Grupo de proveedor:";
        // 
        // lueSupplierGroup
        // 
        lueSupplierGroup.Location = new Point(146, 49);
        lueSupplierGroup.Name = "lueSupplierGroup";
        lueSupplierGroup.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierGroup.Properties.Appearance.Options.UseFont = true;
        lueSupplierGroup.Properties.AutoHeight = false;
        lueSupplierGroup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierGroup.Properties.NullText = "";
        lueSupplierGroup.Properties.PopupView = grvSupplierGroupLookup;
        lueSupplierGroup.Size = new Size(195, 22);
        lueSupplierGroup.TabIndex = 41;
        // 
        // grvSupplierGroupLookup
        // 
        grvSupplierGroupLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSupplierGroupLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvSupplierGroupLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSupplierGroupLookup.Appearance.Row.Options.UseFont = true;
        grvSupplierGroupLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvSupplierGroupLookup.Name = "grvSupplierGroupLookup";
        grvSupplierGroupLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvSupplierGroupLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblSupplierClass
        // 
        lblSupplierClass.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierClass.Appearance.Options.UseFont = true;
        lblSupplierClass.Location = new Point(16, 80);
        lblSupplierClass.Name = "lblSupplierClass";
        lblSupplierClass.Size = new Size(88, 15);
        lblSupplierClass.TabIndex = 33;
        lblSupplierClass.Text = "Clase proveedor:";
        // 
        // lueSupplierClass
        // 
        lueSupplierClass.Location = new Point(146, 77);
        lueSupplierClass.Name = "lueSupplierClass";
        lueSupplierClass.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierClass.Properties.Appearance.Options.UseFont = true;
        lueSupplierClass.Properties.AutoHeight = false;
        lueSupplierClass.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierClass.Properties.NullText = "";
        lueSupplierClass.Properties.PopupView = grvSupplierClassLookup;
        lueSupplierClass.Size = new Size(195, 22);
        lueSupplierClass.TabIndex = 42;
        // 
        // grvSupplierClassLookup
        // 
        grvSupplierClassLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSupplierClassLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvSupplierClassLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSupplierClassLookup.Appearance.Row.Options.UseFont = true;
        grvSupplierClassLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvSupplierClassLookup.Name = "grvSupplierClassLookup";
        grvSupplierClassLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvSupplierClassLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblEconomicActivity
        // 
        lblEconomicActivity.Appearance.Font = new Font("Segoe UI", 9F);
        lblEconomicActivity.Appearance.Options.UseFont = true;
        lblEconomicActivity.Location = new Point(16, 108);
        lblEconomicActivity.Name = "lblEconomicActivity";
        lblEconomicActivity.Size = new Size(115, 15);
        lblEconomicActivity.TabIndex = 34;
        lblEconomicActivity.Text = "Actividad economica:";
        // 
        // lueEconomicActivity
        // 
        lueEconomicActivity.Location = new Point(146, 105);
        lueEconomicActivity.Name = "lueEconomicActivity";
        lueEconomicActivity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueEconomicActivity.Properties.Appearance.Options.UseFont = true;
        lueEconomicActivity.Properties.AutoHeight = false;
        lueEconomicActivity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueEconomicActivity.Properties.NullText = "";
        lueEconomicActivity.Properties.PopupView = grvEconomicActivityLookup;
        lueEconomicActivity.Size = new Size(195, 22);
        lueEconomicActivity.TabIndex = 43;
        // 
        // grvEconomicActivityLookup
        // 
        grvEconomicActivityLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvEconomicActivityLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvEconomicActivityLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvEconomicActivityLookup.Appearance.Row.Options.UseFont = true;
        grvEconomicActivityLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvEconomicActivityLookup.Name = "grvEconomicActivityLookup";
        grvEconomicActivityLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvEconomicActivityLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblZone
        // 
        lblZone.Appearance.Font = new Font("Segoe UI", 9F);
        lblZone.Appearance.Options.UseFont = true;
        lblZone.Location = new Point(16, 136);
        lblZone.Name = "lblZone";
        lblZone.Size = new Size(30, 15);
        lblZone.TabIndex = 35;
        lblZone.Text = "Zona:";
        // 
        // lueZone
        // 
        lueZone.Location = new Point(146, 133);
        lueZone.Name = "lueZone";
        lueZone.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueZone.Properties.Appearance.Options.UseFont = true;
        lueZone.Properties.AutoHeight = false;
        lueZone.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueZone.Properties.NullText = "";
        lueZone.Properties.PopupView = grvZoneLookup;
        lueZone.Size = new Size(195, 22);
        lueZone.TabIndex = 44;
        // 
        // grvZoneLookup
        // 
        grvZoneLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvZoneLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvZoneLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvZoneLookup.Appearance.Row.Options.UseFont = true;
        grvZoneLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvZoneLookup.Name = "grvZoneLookup";
        grvZoneLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvZoneLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblCountry
        // 
        lblCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblCountry.Appearance.Options.UseFont = true;
        lblCountry.Location = new Point(16, 164);
        lblCountry.Name = "lblCountry";
        lblCountry.Size = new Size(24, 15);
        lblCountry.TabIndex = 36;
        lblCountry.Text = "Pais:";
        // 
        // lueCountry
        // 
        lueCountry.Location = new Point(146, 161);
        lueCountry.Name = "lueCountry";
        lueCountry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCountry.Properties.Appearance.Options.UseFont = true;
        lueCountry.Properties.AutoHeight = false;
        lueCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCountry.Properties.NullText = "";
        lueCountry.Properties.PopupView = grvCountryLookup;
        lueCountry.Size = new Size(195, 22);
        lueCountry.TabIndex = 45;
        // 
        // grvCountryLookup
        // 
        grvCountryLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvCountryLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvCountryLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvCountryLookup.Appearance.Row.Options.UseFont = true;
        grvCountryLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvCountryLookup.Name = "grvCountryLookup";
        grvCountryLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvCountryLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblProvince
        // 
        lblProvince.Appearance.Font = new Font("Segoe UI", 9F);
        lblProvince.Appearance.Options.UseFont = true;
        lblProvince.Location = new Point(16, 192);
        lblProvince.Name = "lblProvince";
        lblProvince.Size = new Size(52, 15);
        lblProvince.TabIndex = 37;
        lblProvince.Text = "Provincia:";
        // 
        // lueProvince
        // 
        lueProvince.Location = new Point(146, 189);
        lueProvince.Name = "lueProvince";
        lueProvince.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueProvince.Properties.Appearance.Options.UseFont = true;
        lueProvince.Properties.AutoHeight = false;
        lueProvince.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueProvince.Properties.NullText = "";
        lueProvince.Properties.PopupView = grvProvinceLookup;
        lueProvince.Size = new Size(195, 22);
        lueProvince.TabIndex = 46;
        // 
        // grvProvinceLookup
        // 
        grvProvinceLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvProvinceLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvProvinceLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvProvinceLookup.Appearance.Row.Options.UseFont = true;
        grvProvinceLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvProvinceLookup.Name = "grvProvinceLookup";
        grvProvinceLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvProvinceLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblCity
        // 
        lblCity.Appearance.Font = new Font("Segoe UI", 9F);
        lblCity.Appearance.Options.UseFont = true;
        lblCity.Location = new Point(16, 220);
        lblCity.Name = "lblCity";
        lblCity.Size = new Size(41, 15);
        lblCity.TabIndex = 38;
        lblCity.Text = "Ciudad:";
        // 
        // lueCity
        // 
        lueCity.Location = new Point(146, 217);
        lueCity.Name = "lueCity";
        lueCity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCity.Properties.Appearance.Options.UseFont = true;
        lueCity.Properties.AutoHeight = false;
        lueCity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCity.Properties.NullText = "";
        lueCity.Properties.PopupView = grvCityLookup;
        lueCity.Size = new Size(195, 22);
        lueCity.TabIndex = 47;
        // 
        // grvCityLookup
        // 
        grvCityLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvCityLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvCityLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvCityLookup.Appearance.Row.Options.UseFont = true;
        grvCityLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvCityLookup.Name = "grvCityLookup";
        grvCityLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvCityLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblPriceList
        // 
        lblPriceList.Appearance.Font = new Font("Segoe UI", 9F);
        lblPriceList.Appearance.Options.UseFont = true;
        lblPriceList.Location = new Point(16, 248);
        lblPriceList.Name = "lblPriceList";
        lblPriceList.Size = new Size(84, 15);
        lblPriceList.TabIndex = 39;
        lblPriceList.Text = "Lista de precios:";
        // 
        // luePriceList
        // 
        luePriceList.Location = new Point(146, 245);
        luePriceList.Name = "luePriceList";
        luePriceList.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePriceList.Properties.Appearance.Options.UseFont = true;
        luePriceList.Properties.AutoHeight = false;
        luePriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePriceList.Properties.NullText = "";
        luePriceList.Properties.PopupView = grvPriceListLookup;
        luePriceList.Size = new Size(195, 22);
        luePriceList.TabIndex = 48;
        // 
        // grvPriceListLookup
        // 
        grvPriceListLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvPriceListLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvPriceListLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvPriceListLookup.Appearance.Row.Options.UseFont = true;
        grvPriceListLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvPriceListLookup.Name = "grvPriceListLookup";
        grvPriceListLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvPriceListLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblCreditDays
        // 
        lblCreditDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblCreditDays.Appearance.Options.UseFont = true;
        lblCreditDays.Location = new Point(16, 276);
        lblCreditDays.Name = "lblCreditDays";
        lblCreditDays.Size = new Size(63, 15);
        lblCreditDays.TabIndex = 40;
        lblCreditDays.Text = "Plazo (dias):";
        // 
        // spnCreditDays
        // 
        spnCreditDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnCreditDays.Location = new Point(146, 273);
        spnCreditDays.Name = "spnCreditDays";
        spnCreditDays.Properties.Appearance.Options.UseTextOptions = true;
        spnCreditDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnCreditDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnCreditDays.Properties.IsFloatValue = false;
        spnCreditDays.Properties.MaskSettings.Set("mask", "N00");
        spnCreditDays.Size = new Size(195, 20);
        spnCreditDays.TabIndex = 49;
        // 
        // xtpContacts
        // 
        xtpContacts.Controls.Add(btnContactClear);
        xtpContacts.Controls.Add(btnContactRemove);
        xtpContacts.Controls.Add(btnContactUpdate);
        xtpContacts.Controls.Add(btnContactAdd);
        xtpContacts.Controls.Add(lblContactListTitle);
        xtpContacts.Controls.Add(grdSupplierContacts);
        xtpContacts.Controls.Add(lblContactClassificationTitle);
        xtpContacts.Controls.Add(lblSupplierContactType);
        xtpContacts.Controls.Add(lblContactDataTitle);
        xtpContacts.Controls.Add(lueSupplierContactType);
        xtpContacts.Controls.Add(lblSupplierContactName);
        xtpContacts.Controls.Add(lblSupplierContactDepartment);
        xtpContacts.Controls.Add(txtSupplierContactName);
        xtpContacts.Controls.Add(lueSupplierContactDepartment);
        xtpContacts.Controls.Add(lblSupplierContactPosition);
        xtpContacts.Controls.Add(lblSupplierContactChannel);
        xtpContacts.Controls.Add(lueSupplierContactPosition);
        xtpContacts.Controls.Add(lueSupplierContactChannel);
        xtpContacts.Controls.Add(lblSupplierContactPhone);
        xtpContacts.Controls.Add(lblSupplierContactLanguage);
        xtpContacts.Controls.Add(txtSupplierContactPhone);
        xtpContacts.Controls.Add(lueSupplierContactLanguage);
        xtpContacts.Controls.Add(lblSupplierContactExtension);
        xtpContacts.Controls.Add(lblSupplierContactNotifications);
        xtpContacts.Controls.Add(txtSupplierContactExtension);
        xtpContacts.Controls.Add(lueSupplierContactNotifications);
        xtpContacts.Controls.Add(lblSupplierContactMobile);
        xtpContacts.Controls.Add(lblSupplierContactNotes);
        xtpContacts.Controls.Add(txtSupplierContactMobile);
        xtpContacts.Controls.Add(memSupplierContactNotes);
        xtpContacts.Controls.Add(lblSupplierContactEmail);
        xtpContacts.Controls.Add(txtSupplierContactEmail);
        xtpContacts.Controls.Add(lblSupplierContactPrincipal);
        xtpContacts.Controls.Add(lueSupplierContactPrincipal);
        xtpContacts.Controls.Add(lblSupplierContactStatus);
        xtpContacts.Controls.Add(lueSupplierContactStatus);
        xtpContacts.Name = "xtpContacts";
        xtpContacts.Size = new Size(1094, 402);
        xtpContacts.Text = "Contactos";
        // 
        // btnContactClear
        // 
        btnContactClear.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnContactClear.Appearance.Options.UseFont = true;
        btnContactClear.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnContactClear.ImageOptions.SvgImage");
        btnContactClear.Location = new Point(384, 189);
        btnContactClear.Name = "btnContactClear";
        btnContactClear.Size = new Size(118, 28);
        btnContactClear.TabIndex = 47;
        btnContactClear.Text = "Limpiar";
        // 
        // btnContactRemove
        // 
        btnContactRemove.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnContactRemove.Appearance.Options.UseFont = true;
        btnContactRemove.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnContactRemove.ImageOptions.SvgImage");
        btnContactRemove.Location = new Point(260, 189);
        btnContactRemove.Name = "btnContactRemove";
        btnContactRemove.Size = new Size(118, 28);
        btnContactRemove.TabIndex = 46;
        btnContactRemove.Text = "Quitar";
        // 
        // btnContactUpdate
        // 
        btnContactUpdate.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnContactUpdate.Appearance.Options.UseFont = true;
        btnContactUpdate.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnContactUpdate.ImageOptions.SvgImage");
        btnContactUpdate.Location = new Point(136, 189);
        btnContactUpdate.Name = "btnContactUpdate";
        btnContactUpdate.Size = new Size(118, 28);
        btnContactUpdate.TabIndex = 45;
        btnContactUpdate.Text = "Actualizar";
        // 
        // btnContactAdd
        // 
        btnContactAdd.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnContactAdd.Appearance.Options.UseFont = true;
        btnContactAdd.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnContactAdd.ImageOptions.SvgImage");
        btnContactAdd.Location = new Point(12, 189);
        btnContactAdd.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnContactAdd.Name = "btnContactAdd";
        btnContactAdd.Size = new Size(118, 28);
        btnContactAdd.TabIndex = 44;
        btnContactAdd.Text = "Agregar";
        // 
        // lblContactListTitle
        // 
        lblContactListTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblContactListTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblContactListTitle.Appearance.Options.UseFont = true;
        lblContactListTitle.Appearance.Options.UseForeColor = true;
        lblContactListTitle.Location = new Point(12, 235);
        lblContactListTitle.Name = "lblContactListTitle";
        lblContactListTitle.Size = new Size(163, 20);
        lblContactListTitle.TabIndex = 0;
        lblContactListTitle.Text = "3. Contactos registrados";
        // 
        // grdSupplierContacts
        // 
        grdSupplierContacts.Font = new Font("Segoe UI", 9F);
        grdSupplierContacts.Location = new Point(12, 261);
        grdSupplierContacts.MainView = grvSupplierContacts;
        grdSupplierContacts.Name = "grdSupplierContacts";
        grdSupplierContacts.Size = new Size(1065, 134);
        grdSupplierContacts.TabIndex = 1;
        grdSupplierContacts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvSupplierContacts });
        // 
        // grvSupplierContacts
        // 
        grvSupplierContacts.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSupplierContacts.Appearance.HeaderPanel.ForeColor = Color.FromArgb(23, 32, 51);
        grvSupplierContacts.Appearance.HeaderPanel.Options.UseFont = true;
        grvSupplierContacts.Appearance.HeaderPanel.Options.UseForeColor = true;
        grvSupplierContacts.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSupplierContacts.Appearance.Row.ForeColor = Color.FromArgb(23, 32, 51);
        grvSupplierContacts.Appearance.Row.Options.UseFont = true;
        grvSupplierContacts.Appearance.Row.Options.UseForeColor = true;
        grvSupplierContacts.Columns.AddRange(new GridColumn[] { colSupplierContactName, colSupplierContactPosition, colSupplierContactPhone, colSupplierContactExtension, colSupplierContactMobile, colSupplierContactEmail, colSupplierContactPrimary, colSupplierContactActive });
        grvSupplierContacts.GridControl = grdSupplierContacts;
        grvSupplierContacts.Name = "grvSupplierContacts";
        grvSupplierContacts.OptionsBehavior.Editable = false;
        grvSupplierContacts.OptionsView.ShowGroupPanel = false;
        // 
        // colSupplierContactName
        // 
        colSupplierContactName.Caption = "Nombre";
        colSupplierContactName.FieldName = "Name";
        colSupplierContactName.Name = "colSupplierContactName";
        colSupplierContactName.Visible = true;
        colSupplierContactName.VisibleIndex = 0;
        colSupplierContactName.Width = 160;
        // 
        // colSupplierContactPosition
        // 
        colSupplierContactPosition.Caption = "Cargo";
        colSupplierContactPosition.FieldName = "Position";
        colSupplierContactPosition.Name = "colSupplierContactPosition";
        colSupplierContactPosition.Visible = true;
        colSupplierContactPosition.VisibleIndex = 1;
        colSupplierContactPosition.Width = 150;
        // 
        // colSupplierContactPhone
        // 
        colSupplierContactPhone.Caption = "Telefono";
        colSupplierContactPhone.FieldName = "Phone";
        colSupplierContactPhone.Name = "colSupplierContactPhone";
        colSupplierContactPhone.Visible = true;
        colSupplierContactPhone.VisibleIndex = 2;
        colSupplierContactPhone.Width = 110;
        // 
        // colSupplierContactExtension
        // 
        colSupplierContactExtension.Caption = "Ext.";
        colSupplierContactExtension.FieldName = "Extension";
        colSupplierContactExtension.Name = "colSupplierContactExtension";
        colSupplierContactExtension.Visible = true;
        colSupplierContactExtension.VisibleIndex = 3;
        colSupplierContactExtension.Width = 55;
        // 
        // colSupplierContactMobile
        // 
        colSupplierContactMobile.Caption = "Celular";
        colSupplierContactMobile.FieldName = "Mobile";
        colSupplierContactMobile.Name = "colSupplierContactMobile";
        colSupplierContactMobile.Visible = true;
        colSupplierContactMobile.VisibleIndex = 4;
        colSupplierContactMobile.Width = 110;
        // 
        // colSupplierContactEmail
        // 
        colSupplierContactEmail.Caption = "Correo";
        colSupplierContactEmail.FieldName = "Email";
        colSupplierContactEmail.Name = "colSupplierContactEmail";
        colSupplierContactEmail.Visible = true;
        colSupplierContactEmail.VisibleIndex = 5;
        colSupplierContactEmail.Width = 210;
        // 
        // colSupplierContactPrimary
        // 
        colSupplierContactPrimary.Caption = "Principal";
        colSupplierContactPrimary.FieldName = "IsPrimary";
        colSupplierContactPrimary.Name = "colSupplierContactPrimary";
        colSupplierContactPrimary.Visible = true;
        colSupplierContactPrimary.VisibleIndex = 6;
        colSupplierContactPrimary.Width = 70;
        // 
        // colSupplierContactActive
        // 
        colSupplierContactActive.Caption = "Activo";
        colSupplierContactActive.FieldName = "IsActive";
        colSupplierContactActive.Name = "colSupplierContactActive";
        colSupplierContactActive.Visible = true;
        colSupplierContactActive.VisibleIndex = 7;
        colSupplierContactActive.Width = 65;
        // 
        // lblContactClassificationTitle
        // 
        lblContactClassificationTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblContactClassificationTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblContactClassificationTitle.Appearance.Options.UseFont = true;
        lblContactClassificationTitle.Appearance.Options.UseForeColor = true;
        lblContactClassificationTitle.Location = new Point(549, 12);
        lblContactClassificationTitle.Name = "lblContactClassificationTitle";
        lblContactClassificationTitle.Size = new Size(100, 20);
        lblContactClassificationTitle.TabIndex = 0;
        lblContactClassificationTitle.Text = "2. Clasificacion";
        // 
        // lblSupplierContactType
        // 
        lblSupplierContactType.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactType.Appearance.Options.UseFont = true;
        lblSupplierContactType.Location = new Point(551, 44);
        lblSupplierContactType.Name = "lblSupplierContactType";
        lblSupplierContactType.Size = new Size(93, 15);
        lblSupplierContactType.TabIndex = 1;
        lblSupplierContactType.Text = "Tipo de contacto:";
        // 
        // lblContactDataTitle
        // 
        lblContactDataTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblContactDataTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblContactDataTitle.Appearance.Options.UseFont = true;
        lblContactDataTitle.Appearance.Options.UseForeColor = true;
        lblContactDataTitle.Location = new Point(12, 12);
        lblContactDataTitle.Name = "lblContactDataTitle";
        lblContactDataTitle.Size = new Size(141, 20);
        lblContactDataTitle.TabIndex = 17;
        lblContactDataTitle.Text = "1. Datos del contacto";
        // 
        // lueSupplierContactType
        // 
        lueSupplierContactType.Location = new Point(685, 41);
        lueSupplierContactType.Name = "lueSupplierContactType";
        lueSupplierContactType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierContactType.Properties.Appearance.Options.UseFont = true;
        lueSupplierContactType.Properties.AutoHeight = false;
        lueSupplierContactType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierContactType.Properties.NullText = "";
        lueSupplierContactType.Size = new Size(200, 22);
        lueSupplierContactType.TabIndex = 2;
        // 
        // lblSupplierContactName
        // 
        lblSupplierContactName.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactName.Appearance.Options.UseFont = true;
        lblSupplierContactName.Location = new Point(14, 44);
        lblSupplierContactName.Name = "lblSupplierContactName";
        lblSupplierContactName.Size = new Size(101, 15);
        lblSupplierContactName.TabIndex = 18;
        lblSupplierContactName.Text = "Nombre completo:";
        // 
        // lblSupplierContactDepartment
        // 
        lblSupplierContactDepartment.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactDepartment.Appearance.Options.UseFont = true;
        lblSupplierContactDepartment.Location = new Point(551, 72);
        lblSupplierContactDepartment.Name = "lblSupplierContactDepartment";
        lblSupplierContactDepartment.Size = new Size(79, 15);
        lblSupplierContactDepartment.TabIndex = 3;
        lblSupplierContactDepartment.Text = "Departamento:";
        // 
        // txtSupplierContactName
        // 
        txtSupplierContactName.Location = new Point(148, 41);
        txtSupplierContactName.Name = "txtSupplierContactName";
        txtSupplierContactName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierContactName.Properties.Appearance.Options.UseFont = true;
        txtSupplierContactName.Size = new Size(354, 22);
        txtSupplierContactName.TabIndex = 19;
        // 
        // lueSupplierContactDepartment
        // 
        lueSupplierContactDepartment.Location = new Point(685, 69);
        lueSupplierContactDepartment.Name = "lueSupplierContactDepartment";
        lueSupplierContactDepartment.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierContactDepartment.Properties.Appearance.Options.UseFont = true;
        lueSupplierContactDepartment.Properties.AutoHeight = false;
        lueSupplierContactDepartment.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierContactDepartment.Properties.NullText = "";
        lueSupplierContactDepartment.Size = new Size(200, 22);
        lueSupplierContactDepartment.TabIndex = 4;
        // 
        // lblSupplierContactPosition
        // 
        lblSupplierContactPosition.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactPosition.Appearance.Options.UseFont = true;
        lblSupplierContactPosition.Location = new Point(14, 72);
        lblSupplierContactPosition.Name = "lblSupplierContactPosition";
        lblSupplierContactPosition.Size = new Size(35, 15);
        lblSupplierContactPosition.TabIndex = 20;
        lblSupplierContactPosition.Text = "Cargo:";
        // 
        // lblSupplierContactChannel
        // 
        lblSupplierContactChannel.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactChannel.Appearance.Options.UseFont = true;
        lblSupplierContactChannel.Location = new Point(551, 100);
        lblSupplierContactChannel.Name = "lblSupplierContactChannel";
        lblSupplierContactChannel.Size = new Size(84, 15);
        lblSupplierContactChannel.TabIndex = 5;
        lblSupplierContactChannel.Text = "Canal preferido:";
        // 
        // lueSupplierContactPosition
        // 
        lueSupplierContactPosition.Location = new Point(148, 69);
        lueSupplierContactPosition.Name = "lueSupplierContactPosition";
        lueSupplierContactPosition.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierContactPosition.Properties.Appearance.Options.UseFont = true;
        lueSupplierContactPosition.Properties.AutoHeight = false;
        lueSupplierContactPosition.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierContactPosition.Properties.NullText = "";
        lueSupplierContactPosition.Size = new Size(354, 22);
        lueSupplierContactPosition.TabIndex = 21;
        // 
        // lueSupplierContactChannel
        // 
        lueSupplierContactChannel.Location = new Point(685, 97);
        lueSupplierContactChannel.Name = "lueSupplierContactChannel";
        lueSupplierContactChannel.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierContactChannel.Properties.Appearance.Options.UseFont = true;
        lueSupplierContactChannel.Properties.AutoHeight = false;
        lueSupplierContactChannel.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierContactChannel.Properties.NullText = "";
        lueSupplierContactChannel.Size = new Size(200, 22);
        lueSupplierContactChannel.TabIndex = 6;
        // 
        // lblSupplierContactPhone
        // 
        lblSupplierContactPhone.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactPhone.Appearance.Options.UseFont = true;
        lblSupplierContactPhone.Location = new Point(14, 100);
        lblSupplierContactPhone.Name = "lblSupplierContactPhone";
        lblSupplierContactPhone.Size = new Size(50, 15);
        lblSupplierContactPhone.TabIndex = 22;
        lblSupplierContactPhone.Text = "Telefono:";
        // 
        // lblSupplierContactLanguage
        // 
        lblSupplierContactLanguage.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactLanguage.Appearance.Options.UseFont = true;
        lblSupplierContactLanguage.Location = new Point(551, 128);
        lblSupplierContactLanguage.Name = "lblSupplierContactLanguage";
        lblSupplierContactLanguage.Size = new Size(40, 15);
        lblSupplierContactLanguage.TabIndex = 7;
        lblSupplierContactLanguage.Text = "Idioma:";
        // 
        // txtSupplierContactPhone
        // 
        txtSupplierContactPhone.Location = new Point(148, 97);
        txtSupplierContactPhone.Name = "txtSupplierContactPhone";
        txtSupplierContactPhone.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierContactPhone.Properties.Appearance.Options.UseFont = true;
        txtSupplierContactPhone.Size = new Size(168, 22);
        txtSupplierContactPhone.TabIndex = 23;
        // 
        // lueSupplierContactLanguage
        // 
        lueSupplierContactLanguage.Location = new Point(685, 125);
        lueSupplierContactLanguage.Name = "lueSupplierContactLanguage";
        lueSupplierContactLanguage.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierContactLanguage.Properties.Appearance.Options.UseFont = true;
        lueSupplierContactLanguage.Properties.AutoHeight = false;
        lueSupplierContactLanguage.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierContactLanguage.Properties.NullText = "";
        lueSupplierContactLanguage.Size = new Size(200, 22);
        lueSupplierContactLanguage.TabIndex = 8;
        // 
        // lblSupplierContactExtension
        // 
        lblSupplierContactExtension.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactExtension.Appearance.Options.UseFont = true;
        lblSupplierContactExtension.Location = new Point(336, 100);
        lblSupplierContactExtension.Name = "lblSupplierContactExtension";
        lblSupplierContactExtension.Size = new Size(21, 15);
        lblSupplierContactExtension.TabIndex = 24;
        lblSupplierContactExtension.Text = "Ext.:";
        // 
        // lblSupplierContactNotifications
        // 
        lblSupplierContactNotifications.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactNotifications.Appearance.Options.UseFont = true;
        lblSupplierContactNotifications.Location = new Point(551, 156);
        lblSupplierContactNotifications.Name = "lblSupplierContactNotifications";
        lblSupplierContactNotifications.Size = new Size(115, 15);
        lblSupplierContactNotifications.TabIndex = 9;
        lblSupplierContactNotifications.Text = "Recibe notificaciones:";
        // 
        // txtSupplierContactExtension
        // 
        txtSupplierContactExtension.Location = new Point(393, 97);
        txtSupplierContactExtension.Name = "txtSupplierContactExtension";
        txtSupplierContactExtension.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierContactExtension.Properties.Appearance.Options.UseFont = true;
        txtSupplierContactExtension.Size = new Size(109, 22);
        txtSupplierContactExtension.TabIndex = 25;
        // 
        // lueSupplierContactNotifications
        // 
        lueSupplierContactNotifications.Location = new Point(685, 153);
        lueSupplierContactNotifications.Name = "lueSupplierContactNotifications";
        lueSupplierContactNotifications.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierContactNotifications.Properties.Appearance.Options.UseFont = true;
        lueSupplierContactNotifications.Properties.AutoHeight = false;
        lueSupplierContactNotifications.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierContactNotifications.Properties.NullText = "";
        lueSupplierContactNotifications.Size = new Size(200, 22);
        lueSupplierContactNotifications.TabIndex = 10;
        // 
        // lblSupplierContactMobile
        // 
        lblSupplierContactMobile.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactMobile.Appearance.Options.UseFont = true;
        lblSupplierContactMobile.Location = new Point(14, 128);
        lblSupplierContactMobile.Name = "lblSupplierContactMobile";
        lblSupplierContactMobile.Size = new Size(40, 15);
        lblSupplierContactMobile.TabIndex = 26;
        lblSupplierContactMobile.Text = "Celular:";
        // 
        // lblSupplierContactNotes
        // 
        lblSupplierContactNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactNotes.Appearance.Options.UseFont = true;
        lblSupplierContactNotes.Location = new Point(928, 44);
        lblSupplierContactNotes.Name = "lblSupplierContactNotes";
        lblSupplierContactNotes.Size = new Size(34, 15);
        lblSupplierContactNotes.TabIndex = 11;
        lblSupplierContactNotes.Text = "Notas:";
        // 
        // txtSupplierContactMobile
        // 
        txtSupplierContactMobile.Location = new Point(148, 125);
        txtSupplierContactMobile.Name = "txtSupplierContactMobile";
        txtSupplierContactMobile.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierContactMobile.Properties.Appearance.Options.UseFont = true;
        txtSupplierContactMobile.Size = new Size(168, 22);
        txtSupplierContactMobile.TabIndex = 27;
        // 
        // memSupplierContactNotes
        // 
        memSupplierContactNotes.Location = new Point(928, 70);
        memSupplierContactNotes.Name = "memSupplierContactNotes";
        memSupplierContactNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memSupplierContactNotes.Properties.Appearance.Options.UseFont = true;
        memSupplierContactNotes.Size = new Size(149, 105);
        memSupplierContactNotes.TabIndex = 12;
        // 
        // lblSupplierContactEmail
        // 
        lblSupplierContactEmail.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactEmail.Appearance.Options.UseFont = true;
        lblSupplierContactEmail.Location = new Point(336, 128);
        lblSupplierContactEmail.Name = "lblSupplierContactEmail";
        lblSupplierContactEmail.Size = new Size(39, 15);
        lblSupplierContactEmail.TabIndex = 28;
        lblSupplierContactEmail.Text = "Correo:";
        // 
        // txtSupplierContactEmail
        // 
        txtSupplierContactEmail.Location = new Point(393, 125);
        txtSupplierContactEmail.Name = "txtSupplierContactEmail";
        txtSupplierContactEmail.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierContactEmail.Properties.Appearance.Options.UseFont = true;
        txtSupplierContactEmail.Size = new Size(109, 22);
        txtSupplierContactEmail.TabIndex = 29;
        // 
        // lblSupplierContactPrincipal
        // 
        lblSupplierContactPrincipal.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactPrincipal.Appearance.Options.UseFont = true;
        lblSupplierContactPrincipal.Location = new Point(14, 156);
        lblSupplierContactPrincipal.Name = "lblSupplierContactPrincipal";
        lblSupplierContactPrincipal.Size = new Size(49, 15);
        lblSupplierContactPrincipal.TabIndex = 30;
        lblSupplierContactPrincipal.Text = "Principal:";
        // 
        // lueSupplierContactPrincipal
        // 
        lueSupplierContactPrincipal.Location = new Point(148, 153);
        lueSupplierContactPrincipal.Name = "lueSupplierContactPrincipal";
        lueSupplierContactPrincipal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierContactPrincipal.Properties.Appearance.Options.UseFont = true;
        lueSupplierContactPrincipal.Properties.AutoHeight = false;
        lueSupplierContactPrincipal.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierContactPrincipal.Properties.NullText = "";
        lueSupplierContactPrincipal.Size = new Size(168, 22);
        lueSupplierContactPrincipal.TabIndex = 31;
        // 
        // lblSupplierContactStatus
        // 
        lblSupplierContactStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierContactStatus.Appearance.Options.UseFont = true;
        lblSupplierContactStatus.Location = new Point(336, 156);
        lblSupplierContactStatus.Name = "lblSupplierContactStatus";
        lblSupplierContactStatus.Size = new Size(38, 15);
        lblSupplierContactStatus.TabIndex = 32;
        lblSupplierContactStatus.Text = "Estado:";
        // 
        // lueSupplierContactStatus
        // 
        lueSupplierContactStatus.Location = new Point(393, 153);
        lueSupplierContactStatus.Name = "lueSupplierContactStatus";
        lueSupplierContactStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierContactStatus.Properties.Appearance.Options.UseFont = true;
        lueSupplierContactStatus.Properties.AutoHeight = false;
        lueSupplierContactStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierContactStatus.Properties.NullText = "";
        lueSupplierContactStatus.Size = new Size(109, 22);
        lueSupplierContactStatus.TabIndex = 33;
        // 
        // xtpAddresses
        // 
        xtpAddresses.Controls.Add(btnAddressClear);
        xtpAddresses.Controls.Add(btnAddressRemove);
        xtpAddresses.Controls.Add(btnAddressUpdate);
        xtpAddresses.Controls.Add(btnAddressAdd);
        xtpAddresses.Controls.Add(lblAddressListTitle);
        xtpAddresses.Controls.Add(grdSupplierAddresses);
        xtpAddresses.Controls.Add(lblAddressMapTitle);
        xtpAddresses.Controls.Add(lblAddressMapPlaceholder);
        xtpAddresses.Controls.Add(picAddressMap);
        xtpAddresses.Controls.Add(lblAddressGeoTitle);
        xtpAddresses.Controls.Add(lblSupplierLatitude);
        xtpAddresses.Controls.Add(lblAddressDataTitle);
        xtpAddresses.Controls.Add(spnSupplierLatitude);
        xtpAddresses.Controls.Add(lblSupplierAddressType);
        xtpAddresses.Controls.Add(lblSupplierLongitude);
        xtpAddresses.Controls.Add(lueSupplierAddressType);
        xtpAddresses.Controls.Add(spnSupplierLongitude);
        xtpAddresses.Controls.Add(lblSupplierAddressLine1);
        xtpAddresses.Controls.Add(lblSupplierAddressReference);
        xtpAddresses.Controls.Add(txtSupplierAddressLine1);
        xtpAddresses.Controls.Add(txtSupplierAddressReference);
        xtpAddresses.Controls.Add(lblSupplierAddressLine2);
        xtpAddresses.Controls.Add(btnValidateCoordinates);
        xtpAddresses.Controls.Add(txtSupplierAddressLine2);
        xtpAddresses.Controls.Add(btnClearCoordinates);
        xtpAddresses.Controls.Add(lblSupplierAddressCountry);
        xtpAddresses.Controls.Add(lueSupplierAddressCountry);
        xtpAddresses.Controls.Add(lblSupplierAddressProvince);
        xtpAddresses.Controls.Add(lueSupplierAddressProvince);
        xtpAddresses.Controls.Add(lblSupplierAddressCity);
        xtpAddresses.Controls.Add(lueSupplierAddressCity);
        xtpAddresses.Controls.Add(lblSupplierAddressPostal);
        xtpAddresses.Controls.Add(txtSupplierAddressPostal);
        xtpAddresses.Controls.Add(lblSupplierAddressPrimary);
        xtpAddresses.Controls.Add(lueSupplierAddressPrimary);
        xtpAddresses.Controls.Add(lblSupplierAddressStatus);
        xtpAddresses.Controls.Add(lueSupplierAddressStatus);
        xtpAddresses.Name = "xtpAddresses";
        xtpAddresses.Size = new Size(1094, 402);
        xtpAddresses.Text = "Direcciones";
        // 
        // btnAddressClear
        // 
        btnAddressClear.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressClear.Appearance.Options.UseFont = true;
        btnAddressClear.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressClear.ImageOptions.SvgImage");
        btnAddressClear.Location = new Point(384, 219);
        btnAddressClear.Name = "btnAddressClear";
        btnAddressClear.Size = new Size(118, 28);
        btnAddressClear.TabIndex = 43;
        btnAddressClear.Text = "Limpiar";
        // 
        // btnAddressRemove
        // 
        btnAddressRemove.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressRemove.Appearance.Options.UseFont = true;
        btnAddressRemove.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressRemove.ImageOptions.SvgImage");
        btnAddressRemove.Location = new Point(260, 219);
        btnAddressRemove.Name = "btnAddressRemove";
        btnAddressRemove.Size = new Size(118, 28);
        btnAddressRemove.TabIndex = 42;
        btnAddressRemove.Text = "Quitar";
        // 
        // btnAddressUpdate
        // 
        btnAddressUpdate.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressUpdate.Appearance.Options.UseFont = true;
        btnAddressUpdate.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressUpdate.ImageOptions.SvgImage");
        btnAddressUpdate.Location = new Point(136, 219);
        btnAddressUpdate.Name = "btnAddressUpdate";
        btnAddressUpdate.Size = new Size(118, 28);
        btnAddressUpdate.TabIndex = 41;
        btnAddressUpdate.Text = "Actualizar";
        // 
        // btnAddressAdd
        // 
        btnAddressAdd.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressAdd.Appearance.Options.UseFont = true;
        btnAddressAdd.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressAdd.ImageOptions.SvgImage");
        btnAddressAdd.Location = new Point(12, 219);
        btnAddressAdd.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAddressAdd.Name = "btnAddressAdd";
        btnAddressAdd.Size = new Size(118, 28);
        btnAddressAdd.TabIndex = 40;
        btnAddressAdd.Text = "Agregar";
        // 
        // lblAddressListTitle
        // 
        lblAddressListTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAddressListTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAddressListTitle.Appearance.Options.UseFont = true;
        lblAddressListTitle.Appearance.Options.UseForeColor = true;
        lblAddressListTitle.Location = new Point(12, 256);
        lblAddressListTitle.Name = "lblAddressListTitle";
        lblAddressListTitle.Size = new Size(175, 20);
        lblAddressListTitle.TabIndex = 38;
        lblAddressListTitle.Text = "4. Direcciones registradas";
        // 
        // grdSupplierAddresses
        // 
        grdSupplierAddresses.Font = new Font("Segoe UI", 9F);
        grdSupplierAddresses.Location = new Point(14, 282);
        grdSupplierAddresses.MainView = grvSupplierAddresses;
        grdSupplierAddresses.Name = "grdSupplierAddresses";
        grdSupplierAddresses.Size = new Size(1067, 111);
        grdSupplierAddresses.TabIndex = 39;
        grdSupplierAddresses.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvSupplierAddresses });
        // 
        // grvSupplierAddresses
        // 
        grvSupplierAddresses.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSupplierAddresses.Appearance.HeaderPanel.ForeColor = Color.FromArgb(23, 32, 51);
        grvSupplierAddresses.Appearance.HeaderPanel.Options.UseFont = true;
        grvSupplierAddresses.Appearance.HeaderPanel.Options.UseForeColor = true;
        grvSupplierAddresses.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSupplierAddresses.Appearance.Row.ForeColor = Color.FromArgb(23, 32, 51);
        grvSupplierAddresses.Appearance.Row.Options.UseFont = true;
        grvSupplierAddresses.Appearance.Row.Options.UseForeColor = true;
        grvSupplierAddresses.Columns.AddRange(new GridColumn[] { colSupplierAddressType, colSupplierAddressLine, colSupplierAddressCountry, colSupplierAddressProvince, colSupplierAddressCity, colSupplierAddressPostal, colSupplierAddressPrimary, colSupplierAddressActive });
        grvSupplierAddresses.GridControl = grdSupplierAddresses;
        grvSupplierAddresses.Name = "grvSupplierAddresses";
        grvSupplierAddresses.OptionsBehavior.Editable = false;
        grvSupplierAddresses.OptionsView.ShowGroupPanel = false;
        // 
        // colSupplierAddressType
        // 
        colSupplierAddressType.Caption = "Tipo";
        colSupplierAddressType.FieldName = "AddressType";
        colSupplierAddressType.Name = "colSupplierAddressType";
        colSupplierAddressType.Visible = true;
        colSupplierAddressType.VisibleIndex = 0;
        colSupplierAddressType.Width = 80;
        // 
        // colSupplierAddressLine
        // 
        colSupplierAddressLine.Caption = "Direccion";
        colSupplierAddressLine.FieldName = "Line1";
        colSupplierAddressLine.Name = "colSupplierAddressLine";
        colSupplierAddressLine.Visible = true;
        colSupplierAddressLine.VisibleIndex = 1;
        colSupplierAddressLine.Width = 260;
        // 
        // colSupplierAddressCountry
        // 
        colSupplierAddressCountry.Caption = "Pais";
        colSupplierAddressCountry.FieldName = "Country";
        colSupplierAddressCountry.Name = "colSupplierAddressCountry";
        colSupplierAddressCountry.Visible = true;
        colSupplierAddressCountry.VisibleIndex = 2;
        colSupplierAddressCountry.Width = 110;
        // 
        // colSupplierAddressProvince
        // 
        colSupplierAddressProvince.Caption = "Provincia";
        colSupplierAddressProvince.FieldName = "Province";
        colSupplierAddressProvince.Name = "colSupplierAddressProvince";
        colSupplierAddressProvince.Visible = true;
        colSupplierAddressProvince.VisibleIndex = 3;
        colSupplierAddressProvince.Width = 120;
        // 
        // colSupplierAddressCity
        // 
        colSupplierAddressCity.Caption = "Ciudad";
        colSupplierAddressCity.FieldName = "City";
        colSupplierAddressCity.Name = "colSupplierAddressCity";
        colSupplierAddressCity.Visible = true;
        colSupplierAddressCity.VisibleIndex = 4;
        colSupplierAddressCity.Width = 110;
        // 
        // colSupplierAddressPostal
        // 
        colSupplierAddressPostal.Caption = "Codigo postal";
        colSupplierAddressPostal.FieldName = "PostalCode";
        colSupplierAddressPostal.Name = "colSupplierAddressPostal";
        colSupplierAddressPostal.Visible = true;
        colSupplierAddressPostal.VisibleIndex = 5;
        colSupplierAddressPostal.Width = 105;
        // 
        // colSupplierAddressPrimary
        // 
        colSupplierAddressPrimary.Caption = "Principal";
        colSupplierAddressPrimary.FieldName = "IsPrimary";
        colSupplierAddressPrimary.Name = "colSupplierAddressPrimary";
        colSupplierAddressPrimary.Visible = true;
        colSupplierAddressPrimary.VisibleIndex = 6;
        colSupplierAddressPrimary.Width = 70;
        // 
        // colSupplierAddressActive
        // 
        colSupplierAddressActive.Caption = "Activa";
        colSupplierAddressActive.FieldName = "IsActive";
        colSupplierAddressActive.Name = "colSupplierAddressActive";
        colSupplierAddressActive.Visible = true;
        colSupplierAddressActive.VisibleIndex = 7;
        colSupplierAddressActive.Width = 65;
        // 
        // lblAddressMapTitle
        // 
        lblAddressMapTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAddressMapTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAddressMapTitle.Appearance.Options.UseFont = true;
        lblAddressMapTitle.Appearance.Options.UseForeColor = true;
        lblAddressMapTitle.Location = new Point(775, 12);
        lblAddressMapTitle.Name = "lblAddressMapTitle";
        lblAddressMapTitle.Size = new Size(55, 20);
        lblAddressMapTitle.TabIndex = 0;
        lblAddressMapTitle.Text = "3. Mapa";
        // 
        // lblAddressMapPlaceholder
        // 
        lblAddressMapPlaceholder.Appearance.Font = new Font("Segoe UI", 9F);
        lblAddressMapPlaceholder.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblAddressMapPlaceholder.Appearance.Options.UseFont = true;
        lblAddressMapPlaceholder.Appearance.Options.UseForeColor = true;
        lblAddressMapPlaceholder.Appearance.Options.UseTextOptions = true;
        lblAddressMapPlaceholder.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
        lblAddressMapPlaceholder.Appearance.TextOptions.VAlignment = VertAlignment.Center;
        lblAddressMapPlaceholder.AutoSizeMode = LabelAutoSizeMode.None;
        lblAddressMapPlaceholder.Location = new Point(775, 38);
        lblAddressMapPlaceholder.Name = "lblAddressMapPlaceholder";
        lblAddressMapPlaceholder.Size = new Size(306, 178);
        lblAddressMapPlaceholder.TabIndex = 1;
        lblAddressMapPlaceholder.Text = "Vista previa de mapa pendiente de integracion";
        // 
        // picAddressMap
        // 
        picAddressMap.Location = new Point(775, 38);
        picAddressMap.Name = "picAddressMap";
        picAddressMap.Properties.Appearance.BackColor = Color.FromArgb(245, 248, 250);
        picAddressMap.Properties.Appearance.Options.UseBackColor = true;
        picAddressMap.Properties.BorderStyle = BorderStyles.Simple;
        picAddressMap.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
        picAddressMap.Properties.SizeMode = PictureSizeMode.Zoom;
        picAddressMap.Size = new Size(306, 178);
        picAddressMap.TabIndex = 77;
        picAddressMap.Visible = false;
        // 
        // lblAddressGeoTitle
        // 
        lblAddressGeoTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAddressGeoTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAddressGeoTitle.Appearance.Options.UseFont = true;
        lblAddressGeoTitle.Appearance.Options.UseForeColor = true;
        lblAddressGeoTitle.Location = new Point(526, 12);
        lblAddressGeoTitle.Name = "lblAddressGeoTitle";
        lblAddressGeoTitle.Size = new Size(123, 20);
        lblAddressGeoTitle.TabIndex = 0;
        lblAddressGeoTitle.Text = "2. Geolocalizacion";
        // 
        // lblSupplierLatitude
        // 
        lblSupplierLatitude.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierLatitude.Appearance.Options.UseFont = true;
        lblSupplierLatitude.Location = new Point(528, 44);
        lblSupplierLatitude.Name = "lblSupplierLatitude";
        lblSupplierLatitude.Size = new Size(40, 15);
        lblSupplierLatitude.TabIndex = 1;
        lblSupplierLatitude.Text = "Latitud:";
        // 
        // lblAddressDataTitle
        // 
        lblAddressDataTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAddressDataTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAddressDataTitle.Appearance.Options.UseFont = true;
        lblAddressDataTitle.Appearance.Options.UseForeColor = true;
        lblAddressDataTitle.Location = new Point(12, 12);
        lblAddressDataTitle.Name = "lblAddressDataTitle";
        lblAddressDataTitle.Size = new Size(141, 20);
        lblAddressDataTitle.TabIndex = 19;
        lblAddressDataTitle.Text = "1. Datos de direccion";
        // 
        // spnSupplierLatitude
        // 
        spnSupplierLatitude.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnSupplierLatitude.Location = new Point(612, 41);
        spnSupplierLatitude.Name = "spnSupplierLatitude";
        spnSupplierLatitude.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSupplierLatitude.Properties.Appearance.Options.UseFont = true;
        spnSupplierLatitude.Properties.Appearance.Options.UseTextOptions = true;
        spnSupplierLatitude.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSupplierLatitude.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSupplierLatitude.Properties.DisplayFormat.FormatString = "n6";
        spnSupplierLatitude.Properties.DisplayFormat.FormatType = FormatType.Numeric;
        spnSupplierLatitude.Properties.EditFormat.FormatString = "n6";
        spnSupplierLatitude.Properties.EditFormat.FormatType = FormatType.Numeric;
        spnSupplierLatitude.Properties.MaskSettings.Set("mask", "n6");
        spnSupplierLatitude.Properties.MaxValue = new decimal(new int[] { 90, 0, 0, 0 });
        spnSupplierLatitude.Properties.MinValue = new decimal(new int[] { 90, 0, 0, int.MinValue });
        spnSupplierLatitude.Size = new Size(130, 22);
        spnSupplierLatitude.TabIndex = 2;
        // 
        // lblSupplierAddressType
        // 
        lblSupplierAddressType.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierAddressType.Appearance.Options.UseFont = true;
        lblSupplierAddressType.Location = new Point(14, 44);
        lblSupplierAddressType.Name = "lblSupplierAddressType";
        lblSupplierAddressType.Size = new Size(95, 15);
        lblSupplierAddressType.TabIndex = 20;
        lblSupplierAddressType.Text = "Tipo de direccion:";
        // 
        // lblSupplierLongitude
        // 
        lblSupplierLongitude.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierLongitude.Appearance.Options.UseFont = true;
        lblSupplierLongitude.Location = new Point(528, 73);
        lblSupplierLongitude.Name = "lblSupplierLongitude";
        lblSupplierLongitude.Size = new Size(51, 15);
        lblSupplierLongitude.TabIndex = 3;
        lblSupplierLongitude.Text = "Longitud:";
        // 
        // lueSupplierAddressType
        // 
        lueSupplierAddressType.Location = new Point(133, 41);
        lueSupplierAddressType.Name = "lueSupplierAddressType";
        lueSupplierAddressType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierAddressType.Properties.Appearance.Options.UseFont = true;
        lueSupplierAddressType.Properties.AutoHeight = false;
        lueSupplierAddressType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierAddressType.Properties.NullText = "";
        lueSupplierAddressType.Size = new Size(145, 22);
        lueSupplierAddressType.TabIndex = 21;
        // 
        // spnSupplierLongitude
        // 
        spnSupplierLongitude.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnSupplierLongitude.Location = new Point(612, 69);
        spnSupplierLongitude.Name = "spnSupplierLongitude";
        spnSupplierLongitude.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnSupplierLongitude.Properties.Appearance.Options.UseFont = true;
        spnSupplierLongitude.Properties.Appearance.Options.UseTextOptions = true;
        spnSupplierLongitude.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnSupplierLongitude.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnSupplierLongitude.Properties.DisplayFormat.FormatString = "n6";
        spnSupplierLongitude.Properties.DisplayFormat.FormatType = FormatType.Numeric;
        spnSupplierLongitude.Properties.EditFormat.FormatString = "n6";
        spnSupplierLongitude.Properties.EditFormat.FormatType = FormatType.Numeric;
        spnSupplierLongitude.Properties.MaskSettings.Set("mask", "n6");
        spnSupplierLongitude.Properties.MaxValue = new decimal(new int[] { 180, 0, 0, 0 });
        spnSupplierLongitude.Properties.MinValue = new decimal(new int[] { 180, 0, 0, int.MinValue });
        spnSupplierLongitude.Size = new Size(130, 22);
        spnSupplierLongitude.TabIndex = 4;
        // 
        // lblSupplierAddressLine1
        // 
        lblSupplierAddressLine1.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierAddressLine1.Appearance.Options.UseFont = true;
        lblSupplierAddressLine1.Location = new Point(14, 73);
        lblSupplierAddressLine1.Name = "lblSupplierAddressLine1";
        lblSupplierAddressLine1.Size = new Size(90, 15);
        lblSupplierAddressLine1.TabIndex = 22;
        lblSupplierAddressLine1.Text = "Direccion linea 1:";
        // 
        // lblSupplierAddressReference
        // 
        lblSupplierAddressReference.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierAddressReference.Appearance.Options.UseFont = true;
        lblSupplierAddressReference.Location = new Point(528, 100);
        lblSupplierAddressReference.Name = "lblSupplierAddressReference";
        lblSupplierAddressReference.Size = new Size(58, 15);
        lblSupplierAddressReference.TabIndex = 5;
        lblSupplierAddressReference.Text = "Referencia:";
        // 
        // txtSupplierAddressLine1
        // 
        txtSupplierAddressLine1.Location = new Point(133, 69);
        txtSupplierAddressLine1.Name = "txtSupplierAddressLine1";
        txtSupplierAddressLine1.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierAddressLine1.Properties.Appearance.Options.UseFont = true;
        txtSupplierAddressLine1.Properties.AutoHeight = false;
        txtSupplierAddressLine1.Size = new Size(369, 22);
        txtSupplierAddressLine1.TabIndex = 23;
        // 
        // txtSupplierAddressReference
        // 
        txtSupplierAddressReference.Location = new Point(528, 121);
        txtSupplierAddressReference.Name = "txtSupplierAddressReference";
        txtSupplierAddressReference.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierAddressReference.Properties.Appearance.Options.UseFont = true;
        txtSupplierAddressReference.Properties.AutoHeight = false;
        txtSupplierAddressReference.Size = new Size(214, 54);
        txtSupplierAddressReference.TabIndex = 6;
        // 
        // lblSupplierAddressLine2
        // 
        lblSupplierAddressLine2.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierAddressLine2.Appearance.Options.UseFont = true;
        lblSupplierAddressLine2.Location = new Point(14, 101);
        lblSupplierAddressLine2.Name = "lblSupplierAddressLine2";
        lblSupplierAddressLine2.Size = new Size(90, 15);
        lblSupplierAddressLine2.TabIndex = 24;
        lblSupplierAddressLine2.Text = "Direccion linea 2:";
        // 
        // btnValidateCoordinates
        // 
        btnValidateCoordinates.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnValidateCoordinates.Appearance.Options.UseFont = true;
        btnValidateCoordinates.Location = new Point(528, 181);
        btnValidateCoordinates.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnValidateCoordinates.LookAndFeel.UseDefaultLookAndFeel = false;
        btnValidateCoordinates.Name = "btnValidateCoordinates";
        btnValidateCoordinates.Size = new Size(94, 28);
        btnValidateCoordinates.TabIndex = 7;
        btnValidateCoordinates.Text = "Validar coordenadas";
        // 
        // txtSupplierAddressLine2
        // 
        txtSupplierAddressLine2.Location = new Point(133, 97);
        txtSupplierAddressLine2.Name = "txtSupplierAddressLine2";
        txtSupplierAddressLine2.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierAddressLine2.Properties.Appearance.Options.UseFont = true;
        txtSupplierAddressLine2.Properties.AutoHeight = false;
        txtSupplierAddressLine2.Size = new Size(369, 22);
        txtSupplierAddressLine2.TabIndex = 25;
        // 
        // btnClearCoordinates
        // 
        btnClearCoordinates.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnClearCoordinates.Appearance.Options.UseFont = true;
        btnClearCoordinates.Location = new Point(628, 181);
        btnClearCoordinates.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnClearCoordinates.LookAndFeel.UseDefaultLookAndFeel = false;
        btnClearCoordinates.Name = "btnClearCoordinates";
        btnClearCoordinates.Size = new Size(114, 28);
        btnClearCoordinates.TabIndex = 8;
        btnClearCoordinates.Text = "Limpiar coordenadas";
        // 
        // lblSupplierAddressCountry
        // 
        lblSupplierAddressCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierAddressCountry.Appearance.Options.UseFont = true;
        lblSupplierAddressCountry.Location = new Point(14, 131);
        lblSupplierAddressCountry.Name = "lblSupplierAddressCountry";
        lblSupplierAddressCountry.Size = new Size(24, 15);
        lblSupplierAddressCountry.TabIndex = 26;
        lblSupplierAddressCountry.Text = "Pais:";
        // 
        // lueSupplierAddressCountry
        // 
        lueSupplierAddressCountry.Location = new Point(133, 125);
        lueSupplierAddressCountry.Name = "lueSupplierAddressCountry";
        lueSupplierAddressCountry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierAddressCountry.Properties.Appearance.Options.UseFont = true;
        lueSupplierAddressCountry.Properties.AutoHeight = false;
        lueSupplierAddressCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierAddressCountry.Properties.NullText = "";
        lueSupplierAddressCountry.Properties.PopupView = grvSupplierAddressCountryLookup;
        lueSupplierAddressCountry.Size = new Size(145, 22);
        lueSupplierAddressCountry.TabIndex = 27;
        // 
        // grvSupplierAddressCountryLookup
        // 
        grvSupplierAddressCountryLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSupplierAddressCountryLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvSupplierAddressCountryLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSupplierAddressCountryLookup.Appearance.Row.Options.UseFont = true;
        grvSupplierAddressCountryLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvSupplierAddressCountryLookup.Name = "grvSupplierAddressCountryLookup";
        grvSupplierAddressCountryLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvSupplierAddressCountryLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblSupplierAddressProvince
        // 
        lblSupplierAddressProvince.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierAddressProvince.Appearance.Options.UseFont = true;
        lblSupplierAddressProvince.Location = new Point(294, 128);
        lblSupplierAddressProvince.Name = "lblSupplierAddressProvince";
        lblSupplierAddressProvince.Size = new Size(52, 15);
        lblSupplierAddressProvince.TabIndex = 28;
        lblSupplierAddressProvince.Text = "Provincia:";
        // 
        // lueSupplierAddressProvince
        // 
        lueSupplierAddressProvince.Location = new Point(368, 125);
        lueSupplierAddressProvince.Name = "lueSupplierAddressProvince";
        lueSupplierAddressProvince.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierAddressProvince.Properties.Appearance.Options.UseFont = true;
        lueSupplierAddressProvince.Properties.AutoHeight = false;
        lueSupplierAddressProvince.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierAddressProvince.Properties.NullText = "";
        lueSupplierAddressProvince.Properties.PopupView = grvSupplierAddressProvinceLookup;
        lueSupplierAddressProvince.Size = new Size(134, 22);
        lueSupplierAddressProvince.TabIndex = 29;
        // 
        // grvSupplierAddressProvinceLookup
        // 
        grvSupplierAddressProvinceLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSupplierAddressProvinceLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvSupplierAddressProvinceLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSupplierAddressProvinceLookup.Appearance.Row.Options.UseFont = true;
        grvSupplierAddressProvinceLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvSupplierAddressProvinceLookup.Name = "grvSupplierAddressProvinceLookup";
        grvSupplierAddressProvinceLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvSupplierAddressProvinceLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblSupplierAddressCity
        // 
        lblSupplierAddressCity.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierAddressCity.Appearance.Options.UseFont = true;
        lblSupplierAddressCity.Location = new Point(14, 156);
        lblSupplierAddressCity.Name = "lblSupplierAddressCity";
        lblSupplierAddressCity.Size = new Size(41, 15);
        lblSupplierAddressCity.TabIndex = 30;
        lblSupplierAddressCity.Text = "Ciudad:";
        // 
        // lueSupplierAddressCity
        // 
        lueSupplierAddressCity.Location = new Point(133, 153);
        lueSupplierAddressCity.Name = "lueSupplierAddressCity";
        lueSupplierAddressCity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierAddressCity.Properties.Appearance.Options.UseFont = true;
        lueSupplierAddressCity.Properties.AutoHeight = false;
        lueSupplierAddressCity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierAddressCity.Properties.NullText = "";
        lueSupplierAddressCity.Properties.PopupView = grvSupplierAddressCityLookup;
        lueSupplierAddressCity.Size = new Size(145, 22);
        lueSupplierAddressCity.TabIndex = 31;
        // 
        // grvSupplierAddressCityLookup
        // 
        grvSupplierAddressCityLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSupplierAddressCityLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvSupplierAddressCityLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSupplierAddressCityLookup.Appearance.Row.Options.UseFont = true;
        grvSupplierAddressCityLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvSupplierAddressCityLookup.Name = "grvSupplierAddressCityLookup";
        grvSupplierAddressCityLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvSupplierAddressCityLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblSupplierAddressPostal
        // 
        lblSupplierAddressPostal.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierAddressPostal.Appearance.Options.UseFont = true;
        lblSupplierAddressPostal.Location = new Point(294, 156);
        lblSupplierAddressPostal.Name = "lblSupplierAddressPostal";
        lblSupplierAddressPostal.Size = new Size(63, 15);
        lblSupplierAddressPostal.TabIndex = 32;
        lblSupplierAddressPostal.Text = "Cod. postal:";
        // 
        // txtSupplierAddressPostal
        // 
        txtSupplierAddressPostal.Location = new Point(368, 153);
        txtSupplierAddressPostal.Name = "txtSupplierAddressPostal";
        txtSupplierAddressPostal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierAddressPostal.Properties.Appearance.Options.UseFont = true;
        txtSupplierAddressPostal.Properties.AutoHeight = false;
        txtSupplierAddressPostal.Size = new Size(134, 22);
        txtSupplierAddressPostal.TabIndex = 33;
        // 
        // lblSupplierAddressPrimary
        // 
        lblSupplierAddressPrimary.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierAddressPrimary.Appearance.Options.UseFont = true;
        lblSupplierAddressPrimary.Location = new Point(14, 184);
        lblSupplierAddressPrimary.Name = "lblSupplierAddressPrimary";
        lblSupplierAddressPrimary.Size = new Size(63, 15);
        lblSupplierAddressPrimary.TabIndex = 34;
        lblSupplierAddressPrimary.Text = "Es principal:";
        // 
        // lueSupplierAddressPrimary
        // 
        lueSupplierAddressPrimary.Location = new Point(133, 181);
        lueSupplierAddressPrimary.Name = "lueSupplierAddressPrimary";
        lueSupplierAddressPrimary.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierAddressPrimary.Properties.Appearance.Options.UseFont = true;
        lueSupplierAddressPrimary.Properties.AutoHeight = false;
        lueSupplierAddressPrimary.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierAddressPrimary.Properties.NullText = "";
        lueSupplierAddressPrimary.Size = new Size(145, 22);
        lueSupplierAddressPrimary.TabIndex = 35;
        // 
        // lblSupplierAddressStatus
        // 
        lblSupplierAddressStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierAddressStatus.Appearance.Options.UseFont = true;
        lblSupplierAddressStatus.Location = new Point(294, 184);
        lblSupplierAddressStatus.Name = "lblSupplierAddressStatus";
        lblSupplierAddressStatus.Size = new Size(38, 15);
        lblSupplierAddressStatus.TabIndex = 36;
        lblSupplierAddressStatus.Text = "Estado:";
        // 
        // lueSupplierAddressStatus
        // 
        lueSupplierAddressStatus.Location = new Point(368, 181);
        lueSupplierAddressStatus.Name = "lueSupplierAddressStatus";
        lueSupplierAddressStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierAddressStatus.Properties.Appearance.Options.UseFont = true;
        lueSupplierAddressStatus.Properties.AutoHeight = false;
        lueSupplierAddressStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierAddressStatus.Properties.NullText = "";
        lueSupplierAddressStatus.Size = new Size(134, 22);
        lueSupplierAddressStatus.TabIndex = 37;
        // 
        // xtpPurchases
        // 
        xtpPurchases.Controls.Add(lblPurchaseProductsTitle);
        xtpPurchases.Controls.Add(grdPurchaseProducts);
        xtpPurchases.Controls.Add(lblPurchaseDocumentsTitle);
        xtpPurchases.Controls.Add(grdPurchaseDocuments);
        xtpPurchases.Controls.Add(lblPurchaseStatsTitle);
        xtpPurchases.Controls.Add(lblPurchaseLastDateCaption);
        xtpPurchases.Controls.Add(lblPurchaseLastDateValue);
        xtpPurchases.Controls.Add(lblPurchase12mCaption);
        xtpPurchases.Controls.Add(lblPurchase12mValue);
        xtpPurchases.Controls.Add(lblPurchaseOpenOrdersCaption);
        xtpPurchases.Controls.Add(lblPurchaseOpenOrdersValue);
        xtpPurchases.Controls.Add(lblPurchasePayableCaption);
        xtpPurchases.Controls.Add(lblPurchasePayableValue);
        xtpPurchases.Controls.Add(lblPurchaseAvgDeliveryCaption);
        xtpPurchases.Controls.Add(lblPurchaseAvgDeliveryValue);
        xtpPurchases.Controls.Add(lblPurchaseComplianceCaption);
        xtpPurchases.Controls.Add(lblPurchaseComplianceValue);
        xtpPurchases.Controls.Add(lblAllowBackorder);
        xtpPurchases.Controls.Add(tsAllowSales);
        xtpPurchases.Controls.Add(lblPurchaseConditionsTitle);
        xtpPurchases.Controls.Add(lblPurchasePaymentTerm);
        xtpPurchases.Controls.Add(luePurchasePaymentTerm);
        xtpPurchases.Controls.Add(lblPurchaseCurrency);
        xtpPurchases.Controls.Add(lblCreditLimit);
        xtpPurchases.Controls.Add(spnCreditLimit);
        xtpPurchases.Controls.Add(luePurchaseCurrency);
        xtpPurchases.Controls.Add(lblPurchaseBuyer);
        xtpPurchases.Controls.Add(luePurchaseBuyer);
        xtpPurchases.Name = "xtpPurchases";
        xtpPurchases.Size = new Size(1094, 402);
        xtpPurchases.Text = "Compras";
        // 
        // lblPurchaseProductsTitle
        // 
        lblPurchaseProductsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchaseProductsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblPurchaseProductsTitle.Appearance.Options.UseFont = true;
        lblPurchaseProductsTitle.Appearance.Options.UseForeColor = true;
        lblPurchaseProductsTitle.Location = new Point(421, 190);
        lblPurchaseProductsTitle.Name = "lblPurchaseProductsTitle";
        lblPurchaseProductsTitle.Size = new Size(161, 20);
        lblPurchaseProductsTitle.TabIndex = 45;
        lblPurchaseProductsTitle.Text = "4. Productos frecuentes";
        // 
        // grdPurchaseProducts
        // 
        grdPurchaseProducts.Font = new Font("Segoe UI", 9F);
        grdPurchaseProducts.Location = new Point(421, 216);
        grdPurchaseProducts.MainView = grvPurchaseProducts;
        grdPurchaseProducts.Name = "grdPurchaseProducts";
        grdPurchaseProducts.Size = new Size(651, 148);
        grdPurchaseProducts.TabIndex = 46;
        grdPurchaseProducts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvPurchaseProducts });
        // 
        // grvPurchaseProducts
        // 
        grvPurchaseProducts.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvPurchaseProducts.Appearance.HeaderPanel.Options.UseFont = true;
        grvPurchaseProducts.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvPurchaseProducts.Appearance.Row.Options.UseFont = true;
        grvPurchaseProducts.Columns.AddRange(new GridColumn[] { colPurchaseProductCode, colPurchaseProductName, colPurchaseProductUnit, colPurchaseProductLastPrice, colPurchaseProductCurrency, colPurchaseProductLastDate });
        grvPurchaseProducts.GridControl = grdPurchaseProducts;
        grvPurchaseProducts.Name = "grvPurchaseProducts";
        grvPurchaseProducts.OptionsBehavior.Editable = false;
        grvPurchaseProducts.OptionsView.ShowGroupPanel = false;
        // 
        // colPurchaseProductCode
        // 
        colPurchaseProductCode.Caption = "Codigo";
        colPurchaseProductCode.FieldName = "Code";
        colPurchaseProductCode.Name = "colPurchaseProductCode";
        colPurchaseProductCode.Visible = true;
        colPurchaseProductCode.VisibleIndex = 0;
        colPurchaseProductCode.Width = 65;
        // 
        // colPurchaseProductName
        // 
        colPurchaseProductName.Caption = "Producto";
        colPurchaseProductName.FieldName = "Name";
        colPurchaseProductName.Name = "colPurchaseProductName";
        colPurchaseProductName.Visible = true;
        colPurchaseProductName.VisibleIndex = 1;
        colPurchaseProductName.Width = 120;
        // 
        // colPurchaseProductUnit
        // 
        colPurchaseProductUnit.Caption = "Unidad";
        colPurchaseProductUnit.FieldName = "Unit";
        colPurchaseProductUnit.Name = "colPurchaseProductUnit";
        colPurchaseProductUnit.Visible = true;
        colPurchaseProductUnit.VisibleIndex = 2;
        colPurchaseProductUnit.Width = 50;
        // 
        // colPurchaseProductLastPrice
        // 
        colPurchaseProductLastPrice.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colPurchaseProductLastPrice.Caption = "Ult. precio";
        colPurchaseProductLastPrice.DisplayFormat.FormatString = "n2";
        colPurchaseProductLastPrice.DisplayFormat.FormatType = FormatType.Numeric;
        colPurchaseProductLastPrice.FieldName = "LastPrice";
        colPurchaseProductLastPrice.Name = "colPurchaseProductLastPrice";
        colPurchaseProductLastPrice.Visible = true;
        colPurchaseProductLastPrice.VisibleIndex = 3;
        // 
        // colPurchaseProductCurrency
        // 
        colPurchaseProductCurrency.Caption = "Moneda";
        colPurchaseProductCurrency.FieldName = "Currency";
        colPurchaseProductCurrency.Name = "colPurchaseProductCurrency";
        // 
        // colPurchaseProductLastDate
        // 
        colPurchaseProductLastDate.Caption = "Ultima compra";
        colPurchaseProductLastDate.FieldName = "LastPurchaseDate";
        colPurchaseProductLastDate.Name = "colPurchaseProductLastDate";
        // 
        // lblPurchaseDocumentsTitle
        // 
        lblPurchaseDocumentsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchaseDocumentsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblPurchaseDocumentsTitle.Appearance.Options.UseFont = true;
        lblPurchaseDocumentsTitle.Appearance.Options.UseForeColor = true;
        lblPurchaseDocumentsTitle.Location = new Point(421, 12);
        lblPurchaseDocumentsTitle.Name = "lblPurchaseDocumentsTitle";
        lblPurchaseDocumentsTitle.Size = new Size(167, 20);
        lblPurchaseDocumentsTitle.TabIndex = 0;
        lblPurchaseDocumentsTitle.Text = "3. Documentos recientes";
        // 
        // grdPurchaseDocuments
        // 
        grdPurchaseDocuments.Font = new Font("Segoe UI", 9F);
        grdPurchaseDocuments.Location = new Point(421, 41);
        grdPurchaseDocuments.MainView = grvPurchaseDocuments;
        grdPurchaseDocuments.Name = "grdPurchaseDocuments";
        grdPurchaseDocuments.Size = new Size(651, 132);
        grdPurchaseDocuments.TabIndex = 1;
        grdPurchaseDocuments.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvPurchaseDocuments });
        // 
        // grvPurchaseDocuments
        // 
        grvPurchaseDocuments.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvPurchaseDocuments.Appearance.HeaderPanel.Options.UseFont = true;
        grvPurchaseDocuments.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvPurchaseDocuments.Appearance.Row.Options.UseFont = true;
        grvPurchaseDocuments.Columns.AddRange(new GridColumn[] { colPurchaseDocumentDate, colPurchaseDocumentType, colPurchaseDocumentNumber, colPurchaseDocumentStatus, colPurchaseDocumentTotal, colPurchaseDocumentCurrency, colPurchaseDocumentSap });
        grvPurchaseDocuments.GridControl = grdPurchaseDocuments;
        grvPurchaseDocuments.Name = "grvPurchaseDocuments";
        grvPurchaseDocuments.OptionsBehavior.Editable = false;
        grvPurchaseDocuments.OptionsView.ShowGroupPanel = false;
        // 
        // colPurchaseDocumentDate
        // 
        colPurchaseDocumentDate.Caption = "Fecha";
        colPurchaseDocumentDate.FieldName = "Date";
        colPurchaseDocumentDate.Name = "colPurchaseDocumentDate";
        colPurchaseDocumentDate.Visible = true;
        colPurchaseDocumentDate.VisibleIndex = 0;
        colPurchaseDocumentDate.Width = 72;
        // 
        // colPurchaseDocumentType
        // 
        colPurchaseDocumentType.Caption = "Documento";
        colPurchaseDocumentType.FieldName = "DocumentType";
        colPurchaseDocumentType.Name = "colPurchaseDocumentType";
        colPurchaseDocumentType.Visible = true;
        colPurchaseDocumentType.VisibleIndex = 1;
        colPurchaseDocumentType.Width = 85;
        // 
        // colPurchaseDocumentNumber
        // 
        colPurchaseDocumentNumber.Caption = "Numero";
        colPurchaseDocumentNumber.FieldName = "Number";
        colPurchaseDocumentNumber.Name = "colPurchaseDocumentNumber";
        colPurchaseDocumentNumber.Visible = true;
        colPurchaseDocumentNumber.VisibleIndex = 2;
        colPurchaseDocumentNumber.Width = 78;
        // 
        // colPurchaseDocumentStatus
        // 
        colPurchaseDocumentStatus.Caption = "Estado";
        colPurchaseDocumentStatus.FieldName = "Status";
        colPurchaseDocumentStatus.Name = "colPurchaseDocumentStatus";
        colPurchaseDocumentStatus.Visible = true;
        colPurchaseDocumentStatus.VisibleIndex = 3;
        colPurchaseDocumentStatus.Width = 74;
        // 
        // colPurchaseDocumentTotal
        // 
        colPurchaseDocumentTotal.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colPurchaseDocumentTotal.Caption = "Total";
        colPurchaseDocumentTotal.DisplayFormat.FormatString = "n2";
        colPurchaseDocumentTotal.DisplayFormat.FormatType = FormatType.Numeric;
        colPurchaseDocumentTotal.FieldName = "Total";
        colPurchaseDocumentTotal.Name = "colPurchaseDocumentTotal";
        colPurchaseDocumentTotal.Visible = true;
        colPurchaseDocumentTotal.VisibleIndex = 4;
        colPurchaseDocumentTotal.Width = 78;
        // 
        // colPurchaseDocumentCurrency
        // 
        colPurchaseDocumentCurrency.Caption = "Moneda";
        colPurchaseDocumentCurrency.FieldName = "Currency";
        colPurchaseDocumentCurrency.Name = "colPurchaseDocumentCurrency";
        colPurchaseDocumentCurrency.Visible = true;
        colPurchaseDocumentCurrency.VisibleIndex = 5;
        colPurchaseDocumentCurrency.Width = 58;
        // 
        // colPurchaseDocumentSap
        // 
        colPurchaseDocumentSap.Caption = "SAP";
        colPurchaseDocumentSap.FieldName = "SapStatus";
        colPurchaseDocumentSap.Name = "colPurchaseDocumentSap";
        colPurchaseDocumentSap.Visible = true;
        colPurchaseDocumentSap.VisibleIndex = 6;
        colPurchaseDocumentSap.Width = 65;
        // 
        // lblPurchaseStatsTitle
        // 
        lblPurchaseStatsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchaseStatsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblPurchaseStatsTitle.Appearance.Options.UseFont = true;
        lblPurchaseStatsTitle.Appearance.Options.UseForeColor = true;
        lblPurchaseStatsTitle.Location = new Point(12, 190);
        lblPurchaseStatsTitle.Name = "lblPurchaseStatsTitle";
        lblPurchaseStatsTitle.Size = new Size(92, 20);
        lblPurchaseStatsTitle.TabIndex = 32;
        lblPurchaseStatsTitle.Text = "2. Estadisticas";
        // 
        // lblPurchaseLastDateCaption
        // 
        lblPurchaseLastDateCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchaseLastDateCaption.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblPurchaseLastDateCaption.Appearance.Options.UseFont = true;
        lblPurchaseLastDateCaption.Appearance.Options.UseForeColor = true;
        lblPurchaseLastDateCaption.Location = new Point(14, 223);
        lblPurchaseLastDateCaption.Name = "lblPurchaseLastDateCaption";
        lblPurchaseLastDateCaption.Size = new Size(74, 13);
        lblPurchaseLastDateCaption.TabIndex = 33;
        lblPurchaseLastDateCaption.Text = "Ultima compra";
        // 
        // lblPurchaseLastDateValue
        // 
        lblPurchaseLastDateValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblPurchaseLastDateValue.Appearance.Options.UseFont = true;
        lblPurchaseLastDateValue.Location = new Point(14, 238);
        lblPurchaseLastDateValue.Name = "lblPurchaseLastDateValue";
        lblPurchaseLastDateValue.Size = new Size(64, 17);
        lblPurchaseLastDateValue.TabIndex = 34;
        lblPurchaseLastDateValue.Text = "15/05/2026";
        // 
        // lblPurchase12mCaption
        // 
        lblPurchase12mCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchase12mCaption.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblPurchase12mCaption.Appearance.Options.UseFont = true;
        lblPurchase12mCaption.Appearance.Options.UseForeColor = true;
        lblPurchase12mCaption.Location = new Point(135, 223);
        lblPurchase12mCaption.Name = "lblPurchase12mCaption";
        lblPurchase12mCaption.Size = new Size(89, 13);
        lblPurchase12mCaption.TabIndex = 35;
        lblPurchase12mCaption.Text = "Compras ult. 12m";
        // 
        // lblPurchase12mValue
        // 
        lblPurchase12mValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblPurchase12mValue.Appearance.Options.UseFont = true;
        lblPurchase12mValue.Location = new Point(135, 238);
        lblPurchase12mValue.Name = "lblPurchase12mValue";
        lblPurchase12mValue.Size = new Size(60, 17);
        lblPurchase12mValue.TabIndex = 36;
        lblPurchase12mValue.Text = "128,450.75";
        // 
        // lblPurchaseOpenOrdersCaption
        // 
        lblPurchaseOpenOrdersCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchaseOpenOrdersCaption.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblPurchaseOpenOrdersCaption.Appearance.Options.UseFont = true;
        lblPurchaseOpenOrdersCaption.Appearance.Options.UseForeColor = true;
        lblPurchaseOpenOrdersCaption.Location = new Point(278, 223);
        lblPurchaseOpenOrdersCaption.Name = "lblPurchaseOpenOrdersCaption";
        lblPurchaseOpenOrdersCaption.Size = new Size(86, 13);
        lblPurchaseOpenOrdersCaption.TabIndex = 37;
        lblPurchaseOpenOrdersCaption.Text = "Pedidos abiertos";
        // 
        // lblPurchaseOpenOrdersValue
        // 
        lblPurchaseOpenOrdersValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblPurchaseOpenOrdersValue.Appearance.Options.UseFont = true;
        lblPurchaseOpenOrdersValue.Location = new Point(278, 238);
        lblPurchaseOpenOrdersValue.Name = "lblPurchaseOpenOrdersValue";
        lblPurchaseOpenOrdersValue.Size = new Size(7, 17);
        lblPurchaseOpenOrdersValue.TabIndex = 38;
        lblPurchaseOpenOrdersValue.Text = "5";
        // 
        // lblPurchasePayableCaption
        // 
        lblPurchasePayableCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchasePayableCaption.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblPurchasePayableCaption.Appearance.Options.UseFont = true;
        lblPurchasePayableCaption.Appearance.Options.UseForeColor = true;
        lblPurchasePayableCaption.Location = new Point(14, 276);
        lblPurchasePayableCaption.Name = "lblPurchasePayableCaption";
        lblPurchasePayableCaption.Size = new Size(83, 13);
        lblPurchasePayableCaption.TabIndex = 39;
        lblPurchasePayableCaption.Text = "Saldo por pagar";
        // 
        // lblPurchasePayableValue
        // 
        lblPurchasePayableValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblPurchasePayableValue.Appearance.Options.UseFont = true;
        lblPurchasePayableValue.Location = new Point(14, 291);
        lblPurchasePayableValue.Name = "lblPurchasePayableValue";
        lblPurchasePayableValue.Size = new Size(53, 17);
        lblPurchasePayableValue.TabIndex = 40;
        lblPurchasePayableValue.Text = "12,475.60";
        // 
        // lblPurchaseAvgDeliveryCaption
        // 
        lblPurchaseAvgDeliveryCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchaseAvgDeliveryCaption.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblPurchaseAvgDeliveryCaption.Appearance.Options.UseFont = true;
        lblPurchaseAvgDeliveryCaption.Appearance.Options.UseForeColor = true;
        lblPurchaseAvgDeliveryCaption.Location = new Point(135, 276);
        lblPurchaseAvgDeliveryCaption.Name = "lblPurchaseAvgDeliveryCaption";
        lblPurchaseAvgDeliveryCaption.Size = new Size(96, 13);
        lblPurchaseAvgDeliveryCaption.TabIndex = 41;
        lblPurchaseAvgDeliveryCaption.Text = "Prom. entrega dias";
        // 
        // lblPurchaseAvgDeliveryValue
        // 
        lblPurchaseAvgDeliveryValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblPurchaseAvgDeliveryValue.Appearance.Options.UseFont = true;
        lblPurchaseAvgDeliveryValue.Location = new Point(135, 291);
        lblPurchaseAvgDeliveryValue.Name = "lblPurchaseAvgDeliveryValue";
        lblPurchaseAvgDeliveryValue.Size = new Size(7, 17);
        lblPurchaseAvgDeliveryValue.TabIndex = 42;
        lblPurchaseAvgDeliveryValue.Text = "7";
        // 
        // lblPurchaseComplianceCaption
        // 
        lblPurchaseComplianceCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchaseComplianceCaption.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblPurchaseComplianceCaption.Appearance.Options.UseFont = true;
        lblPurchaseComplianceCaption.Appearance.Options.UseForeColor = true;
        lblPurchaseComplianceCaption.Location = new Point(278, 276);
        lblPurchaseComplianceCaption.Name = "lblPurchaseComplianceCaption";
        lblPurchaseComplianceCaption.Size = new Size(84, 13);
        lblPurchaseComplianceCaption.TabIndex = 43;
        lblPurchaseComplianceCaption.Text = "Cumplimiento %";
        // 
        // lblPurchaseComplianceValue
        // 
        lblPurchaseComplianceValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblPurchaseComplianceValue.Appearance.Options.UseFont = true;
        lblPurchaseComplianceValue.Location = new Point(278, 291);
        lblPurchaseComplianceValue.Name = "lblPurchaseComplianceValue";
        lblPurchaseComplianceValue.Size = new Size(29, 17);
        lblPurchaseComplianceValue.TabIndex = 44;
        lblPurchaseComplianceValue.Text = "96 %";
        // 
        // lblAllowBackorder
        // 
        lblAllowBackorder.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowBackorder.Appearance.Options.UseFont = true;
        lblAllowBackorder.Location = new Point(12, 155);
        lblAllowBackorder.Name = "lblAllowBackorder";
        lblAllowBackorder.Size = new Size(101, 15);
        lblAllowBackorder.TabIndex = 31;
        lblAllowBackorder.Text = "Permitir backorder:";
        // 
        // tsAllowSales
        // 
        tsAllowSales.Location = new Point(144, 153);
        tsAllowSales.Name = "tsAllowSales";
        tsAllowSales.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsAllowSales.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsAllowSales.Properties.Appearance.Options.UseFont = true;
        tsAllowSales.Properties.Appearance.Options.UseForeColor = true;
        tsAllowSales.Properties.OffText = "";
        tsAllowSales.Properties.OnText = "";
        tsAllowSales.Size = new Size(56, 20);
        tsAllowSales.TabIndex = 30;
        // 
        // lblPurchaseConditionsTitle
        // 
        lblPurchaseConditionsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblPurchaseConditionsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblPurchaseConditionsTitle.Appearance.Options.UseFont = true;
        lblPurchaseConditionsTitle.Appearance.Options.UseForeColor = true;
        lblPurchaseConditionsTitle.Location = new Point(12, 12);
        lblPurchaseConditionsTitle.Name = "lblPurchaseConditionsTitle";
        lblPurchaseConditionsTitle.Size = new Size(174, 20);
        lblPurchaseConditionsTitle.TabIndex = 21;
        lblPurchaseConditionsTitle.Text = "1. Condiciones de compra";
        // 
        // lblPurchasePaymentTerm
        // 
        lblPurchasePaymentTerm.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchasePaymentTerm.Appearance.Options.UseFont = true;
        lblPurchasePaymentTerm.Location = new Point(14, 45);
        lblPurchasePaymentTerm.Name = "lblPurchasePaymentTerm";
        lblPurchasePaymentTerm.Size = new Size(104, 15);
        lblPurchasePaymentTerm.TabIndex = 23;
        lblPurchasePaymentTerm.Text = "Condicion de pago:";
        // 
        // luePurchasePaymentTerm
        // 
        luePurchasePaymentTerm.Location = new Point(144, 41);
        luePurchasePaymentTerm.Name = "luePurchasePaymentTerm";
        luePurchasePaymentTerm.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchasePaymentTerm.Properties.Appearance.Options.UseFont = true;
        luePurchasePaymentTerm.Properties.AutoHeight = false;
        luePurchasePaymentTerm.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchasePaymentTerm.Properties.NullText = "";
        luePurchasePaymentTerm.Properties.PopupView = grvPurchasePaymentTermLookup;
        luePurchasePaymentTerm.Size = new Size(220, 22);
        luePurchasePaymentTerm.TabIndex = 24;
        // 
        // grvPurchasePaymentTermLookup
        // 
        grvPurchasePaymentTermLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvPurchasePaymentTermLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvPurchasePaymentTermLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvPurchasePaymentTermLookup.Appearance.Row.Options.UseFont = true;
        grvPurchasePaymentTermLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvPurchasePaymentTermLookup.Name = "grvPurchasePaymentTermLookup";
        grvPurchasePaymentTermLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvPurchasePaymentTermLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblPurchaseCurrency
        // 
        lblPurchaseCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseCurrency.Appearance.Options.UseFont = true;
        lblPurchaseCurrency.Location = new Point(14, 73);
        lblPurchaseCurrency.Name = "lblPurchaseCurrency";
        lblPurchaseCurrency.Size = new Size(97, 15);
        lblPurchaseCurrency.TabIndex = 25;
        lblPurchaseCurrency.Text = "Moneda preferida:";
        // 
        // lblCreditLimit
        // 
        lblCreditLimit.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblCreditLimit.Appearance.Options.UseFont = true;
        lblCreditLimit.Location = new Point(14, 129);
        lblCreditLimit.Name = "lblCreditLimit";
        lblCreditLimit.Size = new Size(124, 13);
        lblCreditLimit.TabIndex = 22;
        lblCreditLimit.Text = "Cupo / limite de credito:";
        // 
        // spnCreditLimit
        // 
        spnCreditLimit.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnCreditLimit.Location = new Point(144, 125);
        spnCreditLimit.Name = "spnCreditLimit";
        spnCreditLimit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnCreditLimit.Properties.Appearance.Options.UseFont = true;
        spnCreditLimit.Properties.Appearance.Options.UseTextOptions = true;
        spnCreditLimit.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnCreditLimit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnCreditLimit.Properties.DisplayFormat.FormatString = "n2";
        spnCreditLimit.Properties.DisplayFormat.FormatType = FormatType.Numeric;
        spnCreditLimit.Properties.EditFormat.FormatString = "n2";
        spnCreditLimit.Properties.EditFormat.FormatType = FormatType.Numeric;
        spnCreditLimit.Properties.MaskSettings.Set("mask", "n2");
        spnCreditLimit.Size = new Size(220, 22);
        spnCreditLimit.TabIndex = 29;
        // 
        // luePurchaseCurrency
        // 
        luePurchaseCurrency.Location = new Point(144, 69);
        luePurchaseCurrency.Name = "luePurchaseCurrency";
        luePurchaseCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseCurrency.Properties.Appearance.Options.UseFont = true;
        luePurchaseCurrency.Properties.AutoHeight = false;
        luePurchaseCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseCurrency.Properties.NullText = "";
        luePurchaseCurrency.Properties.PopupView = grvPurchaseCurrencyLookup;
        luePurchaseCurrency.Size = new Size(220, 22);
        luePurchaseCurrency.TabIndex = 26;
        // 
        // grvPurchaseCurrencyLookup
        // 
        grvPurchaseCurrencyLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvPurchaseCurrencyLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvPurchaseCurrencyLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvPurchaseCurrencyLookup.Appearance.Row.Options.UseFont = true;
        grvPurchaseCurrencyLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvPurchaseCurrencyLookup.Name = "grvPurchaseCurrencyLookup";
        grvPurchaseCurrencyLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvPurchaseCurrencyLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblPurchaseBuyer
        // 
        lblPurchaseBuyer.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseBuyer.Appearance.Options.UseFont = true;
        lblPurchaseBuyer.Location = new Point(14, 101);
        lblPurchaseBuyer.Name = "lblPurchaseBuyer";
        lblPurchaseBuyer.Size = new Size(115, 15);
        lblPurchaseBuyer.TabIndex = 27;
        lblPurchaseBuyer.Text = "Comprador asignado:";
        // 
        // luePurchaseBuyer
        // 
        luePurchaseBuyer.Location = new Point(144, 97);
        luePurchaseBuyer.Name = "luePurchaseBuyer";
        luePurchaseBuyer.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseBuyer.Properties.Appearance.Options.UseFont = true;
        luePurchaseBuyer.Properties.AutoHeight = false;
        luePurchaseBuyer.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseBuyer.Properties.NullText = "";
        luePurchaseBuyer.Properties.PopupView = grvPurchaseBuyerLookup;
        luePurchaseBuyer.Size = new Size(220, 22);
        luePurchaseBuyer.TabIndex = 28;
        // 
        // grvPurchaseBuyerLookup
        // 
        grvPurchaseBuyerLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvPurchaseBuyerLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvPurchaseBuyerLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvPurchaseBuyerLookup.Appearance.Row.Options.UseFont = true;
        grvPurchaseBuyerLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvPurchaseBuyerLookup.Name = "grvPurchaseBuyerLookup";
        grvPurchaseBuyerLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvPurchaseBuyerLookup.OptionsView.ShowGroupPanel = false;
        // 
        // xtpBanks
        // 
        xtpBanks.Controls.Add(lblBankAccountsTitle);
        xtpBanks.Controls.Add(grdBankAccounts);
        xtpBanks.Controls.Add(btnBankClear);
        xtpBanks.Controls.Add(btnAddressClear0);
        xtpBanks.Controls.Add(btnAddressClear1);
        xtpBanks.Controls.Add(btnAddressClear2);
        xtpBanks.Controls.Add(lblBankTransferTitle);
        xtpBanks.Controls.Add(lblBankSwift);
        xtpBanks.Controls.Add(lblBankDataTitle);
        xtpBanks.Controls.Add(txtBankSwift);
        xtpBanks.Controls.Add(lblBankName);
        xtpBanks.Controls.Add(lblBankAba);
        xtpBanks.Controls.Add(txtBankAba);
        xtpBanks.Controls.Add(lueBankName);
        xtpBanks.Controls.Add(lblBankIban);
        xtpBanks.Controls.Add(txtBankIban);
        xtpBanks.Controls.Add(lblBankAccountType);
        xtpBanks.Controls.Add(lblBankCountry);
        xtpBanks.Controls.Add(lueBankAccountType);
        xtpBanks.Controls.Add(lueBankCountry);
        xtpBanks.Controls.Add(lueBankStatus);
        xtpBanks.Controls.Add(lblBankCity);
        xtpBanks.Controls.Add(lblBankAccountNumber);
        xtpBanks.Controls.Add(lueBankCity);
        xtpBanks.Controls.Add(lblBankStatus);
        xtpBanks.Controls.Add(lblBankNotes);
        xtpBanks.Controls.Add(txtBankAccountNumber);
        xtpBanks.Controls.Add(memBankNotes);
        xtpBanks.Controls.Add(lueBankPrimary);
        xtpBanks.Controls.Add(lblBankHolder);
        xtpBanks.Controls.Add(lblBankPrimary);
        xtpBanks.Controls.Add(txtBankHolder);
        xtpBanks.Controls.Add(lueBankCurrency);
        xtpBanks.Controls.Add(lblBankHolderIdentification);
        xtpBanks.Controls.Add(lblBankCurrency);
        xtpBanks.Controls.Add(txtBankHolderIdentification);
        xtpBanks.Name = "xtpBanks";
        xtpBanks.Size = new Size(1094, 402);
        xtpBanks.Text = "Bancos";
        // 
        // lblBankAccountsTitle
        // 
        lblBankAccountsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblBankAccountsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblBankAccountsTitle.Appearance.Options.UseFont = true;
        lblBankAccountsTitle.Appearance.Options.UseForeColor = true;
        lblBankAccountsTitle.Location = new Point(12, 264);
        lblBankAccountsTitle.Name = "lblBankAccountsTitle";
        lblBankAccountsTitle.Size = new Size(139, 20);
        lblBankAccountsTitle.TabIndex = 48;
        lblBankAccountsTitle.Text = "3. Cuentas bancarias";
        // 
        // grdBankAccounts
        // 
        grdBankAccounts.Font = new Font("Segoe UI", 9F);
        grdBankAccounts.Location = new Point(12, 290);
        grdBankAccounts.MainView = grvBankAccounts;
        grdBankAccounts.Name = "grdBankAccounts";
        grdBankAccounts.Size = new Size(1058, 98);
        grdBankAccounts.TabIndex = 49;
        grdBankAccounts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvBankAccounts });
        // 
        // grvBankAccounts
        // 
        grvBankAccounts.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvBankAccounts.Appearance.HeaderPanel.Options.UseFont = true;
        grvBankAccounts.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvBankAccounts.Appearance.Row.Options.UseFont = true;
        grvBankAccounts.Columns.AddRange(new GridColumn[] { colBankName, colBankAccountType, colBankAccountNumber, colBankHolder, colBankIdentification, colBankCurrency, colBankPrimary, colBankActive });
        grvBankAccounts.GridControl = grdBankAccounts;
        grvBankAccounts.Name = "grvBankAccounts";
        grvBankAccounts.OptionsBehavior.Editable = false;
        grvBankAccounts.OptionsView.ShowGroupPanel = false;
        // 
        // colBankName
        // 
        colBankName.Caption = "Banco";
        colBankName.FieldName = "BankName";
        colBankName.Name = "colBankName";
        colBankName.Visible = true;
        colBankName.VisibleIndex = 0;
        colBankName.Width = 160;
        // 
        // colBankAccountType
        // 
        colBankAccountType.Caption = "Tipo cuenta";
        colBankAccountType.FieldName = "AccountType";
        colBankAccountType.Name = "colBankAccountType";
        colBankAccountType.Visible = true;
        colBankAccountType.VisibleIndex = 1;
        colBankAccountType.Width = 110;
        // 
        // colBankAccountNumber
        // 
        colBankAccountNumber.Caption = "Numero cuenta";
        colBankAccountNumber.FieldName = "AccountNumber";
        colBankAccountNumber.Name = "colBankAccountNumber";
        colBankAccountNumber.Visible = true;
        colBankAccountNumber.VisibleIndex = 2;
        colBankAccountNumber.Width = 130;
        // 
        // colBankHolder
        // 
        colBankHolder.Caption = "Titular";
        colBankHolder.FieldName = "Holder";
        colBankHolder.Name = "colBankHolder";
        colBankHolder.Visible = true;
        colBankHolder.VisibleIndex = 3;
        colBankHolder.Width = 180;
        // 
        // colBankIdentification
        // 
        colBankIdentification.Caption = "Identificacion";
        colBankIdentification.FieldName = "Identification";
        colBankIdentification.Name = "colBankIdentification";
        colBankIdentification.Visible = true;
        colBankIdentification.VisibleIndex = 4;
        colBankIdentification.Width = 120;
        // 
        // colBankCurrency
        // 
        colBankCurrency.Caption = "Moneda";
        colBankCurrency.FieldName = "Currency";
        colBankCurrency.Name = "colBankCurrency";
        colBankCurrency.Visible = true;
        colBankCurrency.VisibleIndex = 5;
        colBankCurrency.Width = 90;
        // 
        // colBankPrimary
        // 
        colBankPrimary.Caption = "Principal";
        colBankPrimary.FieldName = "IsPrimary";
        colBankPrimary.Name = "colBankPrimary";
        colBankPrimary.Visible = true;
        colBankPrimary.VisibleIndex = 6;
        // 
        // colBankActive
        // 
        colBankActive.Caption = "Activa";
        colBankActive.FieldName = "IsActive";
        colBankActive.Name = "colBankActive";
        colBankActive.Visible = true;
        colBankActive.VisibleIndex = 7;
        colBankActive.Width = 65;
        // 
        // btnBankClear
        // 
        btnBankClear.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnBankClear.Appearance.Options.UseFont = true;
        btnBankClear.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnBankClear.ImageOptions.SvgImage");
        btnBankClear.Location = new Point(384, 218);
        btnBankClear.Name = "btnBankClear";
        btnBankClear.Size = new Size(118, 28);
        btnBankClear.TabIndex = 47;
        btnBankClear.Text = "Limpiar";
        // 
        // btnAddressClear0
        // 
        btnAddressClear0.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressClear0.Appearance.Options.UseFont = true;
        btnAddressClear0.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressClear0.ImageOptions.SvgImage");
        btnAddressClear0.Location = new Point(260, 218);
        btnAddressClear0.Name = "btnAddressClear0";
        btnAddressClear0.Size = new Size(118, 28);
        btnAddressClear0.TabIndex = 46;
        btnAddressClear0.Text = "Quitar";
        // 
        // btnAddressClear1
        // 
        btnAddressClear1.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressClear1.Appearance.Options.UseFont = true;
        btnAddressClear1.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressClear1.ImageOptions.SvgImage");
        btnAddressClear1.Location = new Point(136, 218);
        btnAddressClear1.Name = "btnAddressClear1";
        btnAddressClear1.Size = new Size(118, 28);
        btnAddressClear1.TabIndex = 45;
        btnAddressClear1.Text = "Actualizar";
        // 
        // btnAddressClear2
        // 
        btnAddressClear2.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressClear2.Appearance.Options.UseFont = true;
        btnAddressClear2.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressClear2.ImageOptions.SvgImage");
        btnAddressClear2.Location = new Point(12, 218);
        btnAddressClear2.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAddressClear2.Name = "btnAddressClear2";
        btnAddressClear2.Size = new Size(118, 28);
        btnAddressClear2.TabIndex = 44;
        btnAddressClear2.Text = "Agregar";
        // 
        // lblBankTransferTitle
        // 
        lblBankTransferTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblBankTransferTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblBankTransferTitle.Appearance.Options.UseFont = true;
        lblBankTransferTitle.Appearance.Options.UseForeColor = true;
        lblBankTransferTitle.Location = new Point(597, 12);
        lblBankTransferTitle.Name = "lblBankTransferTitle";
        lblBankTransferTitle.Size = new Size(108, 20);
        lblBankTransferTitle.TabIndex = 0;
        lblBankTransferTitle.Text = "2. Transferencia";
        // 
        // lblBankSwift
        // 
        lblBankSwift.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankSwift.Appearance.Options.UseFont = true;
        lblBankSwift.Location = new Point(599, 45);
        lblBankSwift.Name = "lblBankSwift";
        lblBankSwift.Size = new Size(107, 15);
        lblBankSwift.TabIndex = 1;
        lblBankSwift.Text = "Codigo SWIFT / BIC:";
        // 
        // lblBankDataTitle
        // 
        lblBankDataTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblBankDataTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblBankDataTitle.Appearance.Options.UseFont = true;
        lblBankDataTitle.Appearance.Options.UseForeColor = true;
        lblBankDataTitle.Location = new Point(12, 12);
        lblBankDataTitle.Name = "lblBankDataTitle";
        lblBankDataTitle.Size = new Size(123, 20);
        lblBankDataTitle.TabIndex = 0;
        lblBankDataTitle.Text = "1. Datos bancarios";
        // 
        // txtBankSwift
        // 
        txtBankSwift.Location = new Point(733, 41);
        txtBankSwift.Name = "txtBankSwift";
        txtBankSwift.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBankSwift.Properties.Appearance.Options.UseFont = true;
        txtBankSwift.Properties.AutoHeight = false;
        txtBankSwift.Size = new Size(160, 22);
        txtBankSwift.TabIndex = 2;
        // 
        // lblBankName
        // 
        lblBankName.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankName.Appearance.Options.UseFont = true;
        lblBankName.Location = new Point(14, 44);
        lblBankName.Name = "lblBankName";
        lblBankName.Size = new Size(36, 15);
        lblBankName.TabIndex = 1;
        lblBankName.Text = "Banco:";
        // 
        // lblBankAba
        // 
        lblBankAba.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankAba.Appearance.Options.UseFont = true;
        lblBankAba.Location = new Point(599, 73);
        lblBankAba.Name = "lblBankAba";
        lblBankAba.Size = new Size(118, 15);
        lblBankAba.TabIndex = 3;
        lblBankAba.Text = "Codigo ABA / routing:";
        // 
        // txtBankAba
        // 
        txtBankAba.Location = new Point(733, 69);
        txtBankAba.Name = "txtBankAba";
        txtBankAba.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBankAba.Properties.Appearance.Options.UseFont = true;
        txtBankAba.Properties.AutoHeight = false;
        txtBankAba.Size = new Size(160, 22);
        txtBankAba.TabIndex = 4;
        // 
        // lueBankName
        // 
        lueBankName.Location = new Point(148, 41);
        lueBankName.Name = "lueBankName";
        lueBankName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBankName.Properties.Appearance.Options.UseFont = true;
        lueBankName.Properties.AutoHeight = false;
        lueBankName.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBankName.Properties.NullText = "";
        lueBankName.Properties.PopupView = grvBankNameLookup;
        lueBankName.Size = new Size(250, 22);
        lueBankName.TabIndex = 2;
        // 
        // grvBankNameLookup
        // 
        grvBankNameLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvBankNameLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvBankNameLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvBankNameLookup.Appearance.Row.Options.UseFont = true;
        grvBankNameLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvBankNameLookup.Name = "grvBankNameLookup";
        grvBankNameLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvBankNameLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblBankIban
        // 
        lblBankIban.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankIban.Appearance.Options.UseFont = true;
        lblBankIban.Location = new Point(599, 100);
        lblBankIban.Name = "lblBankIban";
        lblBankIban.Size = new Size(30, 15);
        lblBankIban.TabIndex = 5;
        lblBankIban.Text = "IBAN:";
        // 
        // txtBankIban
        // 
        txtBankIban.Location = new Point(733, 97);
        txtBankIban.Name = "txtBankIban";
        txtBankIban.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBankIban.Properties.Appearance.Options.UseFont = true;
        txtBankIban.Properties.AutoHeight = false;
        txtBankIban.Size = new Size(160, 22);
        txtBankIban.TabIndex = 6;
        // 
        // lblBankAccountType
        // 
        lblBankAccountType.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankAccountType.Appearance.Options.UseFont = true;
        lblBankAccountType.Location = new Point(14, 73);
        lblBankAccountType.Name = "lblBankAccountType";
        lblBankAccountType.Size = new Size(82, 15);
        lblBankAccountType.TabIndex = 3;
        lblBankAccountType.Text = "Tipo de cuenta:";
        // 
        // lblBankCountry
        // 
        lblBankCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankCountry.Appearance.Options.UseFont = true;
        lblBankCountry.Location = new Point(599, 128);
        lblBankCountry.Name = "lblBankCountry";
        lblBankCountry.Size = new Size(60, 15);
        lblBankCountry.TabIndex = 7;
        lblBankCountry.Text = "Pais banco:";
        // 
        // lueBankAccountType
        // 
        lueBankAccountType.Location = new Point(148, 69);
        lueBankAccountType.Name = "lueBankAccountType";
        lueBankAccountType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBankAccountType.Properties.Appearance.Options.UseFont = true;
        lueBankAccountType.Properties.AutoHeight = false;
        lueBankAccountType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBankAccountType.Properties.NullText = "";
        lueBankAccountType.Size = new Size(250, 22);
        lueBankAccountType.TabIndex = 4;
        // 
        // lueBankCountry
        // 
        lueBankCountry.Location = new Point(733, 125);
        lueBankCountry.Name = "lueBankCountry";
        lueBankCountry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBankCountry.Properties.Appearance.Options.UseFont = true;
        lueBankCountry.Properties.AutoHeight = false;
        lueBankCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBankCountry.Properties.NullText = "";
        lueBankCountry.Properties.PopupView = grvBankCountryLookup;
        lueBankCountry.Size = new Size(160, 22);
        lueBankCountry.TabIndex = 8;
        // 
        // grvBankCountryLookup
        // 
        grvBankCountryLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvBankCountryLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvBankCountryLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvBankCountryLookup.Appearance.Row.Options.UseFont = true;
        grvBankCountryLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvBankCountryLookup.Name = "grvBankCountryLookup";
        grvBankCountryLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvBankCountryLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lueBankStatus
        // 
        lueBankStatus.Location = new Point(473, 41);
        lueBankStatus.Name = "lueBankStatus";
        lueBankStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBankStatus.Properties.Appearance.Options.UseFont = true;
        lueBankStatus.Properties.AutoHeight = false;
        lueBankStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBankStatus.Properties.NullText = "";
        lueBankStatus.Size = new Size(100, 22);
        lueBankStatus.TabIndex = 16;
        // 
        // lblBankCity
        // 
        lblBankCity.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankCity.Appearance.Options.UseFont = true;
        lblBankCity.Location = new Point(599, 156);
        lblBankCity.Name = "lblBankCity";
        lblBankCity.Size = new Size(77, 15);
        lblBankCity.TabIndex = 9;
        lblBankCity.Text = "Ciudad banco:";
        // 
        // lblBankAccountNumber
        // 
        lblBankAccountNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankAccountNumber.Appearance.Options.UseFont = true;
        lblBankAccountNumber.Location = new Point(14, 100);
        lblBankAccountNumber.Name = "lblBankAccountNumber";
        lblBankAccountNumber.Size = new Size(102, 15);
        lblBankAccountNumber.TabIndex = 5;
        lblBankAccountNumber.Text = "Numero de cuenta:";
        // 
        // lueBankCity
        // 
        lueBankCity.Location = new Point(733, 153);
        lueBankCity.Name = "lueBankCity";
        lueBankCity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBankCity.Properties.Appearance.Options.UseFont = true;
        lueBankCity.Properties.AutoHeight = false;
        lueBankCity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBankCity.Properties.NullText = "";
        lueBankCity.Properties.PopupView = grvBankCityLookup;
        lueBankCity.Size = new Size(160, 22);
        lueBankCity.TabIndex = 10;
        // 
        // grvBankCityLookup
        // 
        grvBankCityLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvBankCityLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvBankCityLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvBankCityLookup.Appearance.Row.Options.UseFont = true;
        grvBankCityLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvBankCityLookup.Name = "grvBankCityLookup";
        grvBankCityLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvBankCityLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblBankStatus
        // 
        lblBankStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankStatus.Appearance.Options.UseFont = true;
        lblBankStatus.Location = new Point(404, 44);
        lblBankStatus.Name = "lblBankStatus";
        lblBankStatus.Size = new Size(38, 15);
        lblBankStatus.TabIndex = 15;
        lblBankStatus.Text = "Estado:";
        // 
        // lblBankNotes
        // 
        lblBankNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankNotes.Appearance.Options.UseFont = true;
        lblBankNotes.Location = new Point(901, 44);
        lblBankNotes.Name = "lblBankNotes";
        lblBankNotes.Size = new Size(80, 15);
        lblBankNotes.TabIndex = 11;
        lblBankNotes.Text = "Observaciones:";
        // 
        // txtBankAccountNumber
        // 
        txtBankAccountNumber.Location = new Point(148, 97);
        txtBankAccountNumber.Name = "txtBankAccountNumber";
        txtBankAccountNumber.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBankAccountNumber.Properties.Appearance.Options.UseFont = true;
        txtBankAccountNumber.Properties.AutoHeight = false;
        txtBankAccountNumber.Size = new Size(250, 22);
        txtBankAccountNumber.TabIndex = 6;
        // 
        // memBankNotes
        // 
        memBankNotes.Location = new Point(902, 69);
        memBankNotes.Name = "memBankNotes";
        memBankNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memBankNotes.Properties.Appearance.Options.UseFont = true;
        memBankNotes.Size = new Size(168, 106);
        memBankNotes.TabIndex = 12;
        // 
        // lueBankPrimary
        // 
        lueBankPrimary.Location = new Point(473, 69);
        lueBankPrimary.Name = "lueBankPrimary";
        lueBankPrimary.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBankPrimary.Properties.Appearance.Options.UseFont = true;
        lueBankPrimary.Properties.AutoHeight = false;
        lueBankPrimary.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBankPrimary.Properties.NullText = "";
        lueBankPrimary.Size = new Size(100, 22);
        lueBankPrimary.TabIndex = 14;
        // 
        // lblBankHolder
        // 
        lblBankHolder.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankHolder.Appearance.Options.UseFont = true;
        lblBankHolder.Location = new Point(14, 128);
        lblBankHolder.Name = "lblBankHolder";
        lblBankHolder.Size = new Size(37, 15);
        lblBankHolder.TabIndex = 7;
        lblBankHolder.Text = "Titular:";
        // 
        // lblBankPrimary
        // 
        lblBankPrimary.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankPrimary.Appearance.Options.UseFont = true;
        lblBankPrimary.Location = new Point(404, 73);
        lblBankPrimary.Name = "lblBankPrimary";
        lblBankPrimary.Size = new Size(63, 15);
        lblBankPrimary.TabIndex = 13;
        lblBankPrimary.Text = "Es principal:";
        // 
        // txtBankHolder
        // 
        txtBankHolder.Location = new Point(148, 125);
        txtBankHolder.Name = "txtBankHolder";
        txtBankHolder.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBankHolder.Properties.Appearance.Options.UseFont = true;
        txtBankHolder.Properties.AutoHeight = false;
        txtBankHolder.Size = new Size(250, 22);
        txtBankHolder.TabIndex = 8;
        // 
        // lueBankCurrency
        // 
        lueBankCurrency.Location = new Point(148, 181);
        lueBankCurrency.Name = "lueBankCurrency";
        lueBankCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueBankCurrency.Properties.Appearance.Options.UseFont = true;
        lueBankCurrency.Properties.AutoHeight = false;
        lueBankCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBankCurrency.Properties.NullText = "";
        lueBankCurrency.Properties.PopupView = grvBankCurrencyLookup;
        lueBankCurrency.Size = new Size(250, 22);
        lueBankCurrency.TabIndex = 12;
        // 
        // grvBankCurrencyLookup
        // 
        grvBankCurrencyLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvBankCurrencyLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvBankCurrencyLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvBankCurrencyLookup.Appearance.Row.Options.UseFont = true;
        grvBankCurrencyLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvBankCurrencyLookup.Name = "grvBankCurrencyLookup";
        grvBankCurrencyLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvBankCurrencyLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblBankHolderIdentification
        // 
        lblBankHolderIdentification.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankHolderIdentification.Appearance.Options.UseFont = true;
        lblBankHolderIdentification.Location = new Point(14, 156);
        lblBankHolderIdentification.Name = "lblBankHolderIdentification";
        lblBankHolderIdentification.Size = new Size(109, 15);
        lblBankHolderIdentification.TabIndex = 9;
        lblBankHolderIdentification.Text = "Identificacion titular:";
        // 
        // lblBankCurrency
        // 
        lblBankCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankCurrency.Appearance.Options.UseFont = true;
        lblBankCurrency.Location = new Point(14, 184);
        lblBankCurrency.Name = "lblBankCurrency";
        lblBankCurrency.Size = new Size(47, 15);
        lblBankCurrency.TabIndex = 11;
        lblBankCurrency.Text = "Moneda:";
        // 
        // txtBankHolderIdentification
        // 
        txtBankHolderIdentification.Location = new Point(148, 153);
        txtBankHolderIdentification.Name = "txtBankHolderIdentification";
        txtBankHolderIdentification.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBankHolderIdentification.Properties.Appearance.Options.UseFont = true;
        txtBankHolderIdentification.Properties.AutoHeight = false;
        txtBankHolderIdentification.Size = new Size(250, 22);
        txtBankHolderIdentification.TabIndex = 10;
        // 
        // xtpAccounting
        // 
        xtpAccounting.Controls.Add(lblAccountingDimensionsTitle);
        xtpAccounting.Controls.Add(lblAccountingBranch);
        xtpAccounting.Controls.Add(lueAccountingBranch);
        xtpAccounting.Controls.Add(lblAccountingDepartment);
        xtpAccounting.Controls.Add(lueAccountingDepartment);
        xtpAccounting.Controls.Add(lblAccountingBusinessLine);
        xtpAccounting.Controls.Add(lueAccountingBusinessLine);
        xtpAccounting.Controls.Add(lblAccountingCostCenter);
        xtpAccounting.Controls.Add(lueAccountingCostCenter);
        xtpAccounting.Controls.Add(lblAccountingProject);
        xtpAccounting.Controls.Add(lueAccountingProject);
        xtpAccounting.Controls.Add(chkAccountingConciliationRequired);
        xtpAccounting.Controls.Add(spnAccountingPaymentTolerance);
        xtpAccounting.Controls.Add(lblAccountingPaymentTolerance);
        xtpAccounting.Controls.Add(lblAccountingAveragePaymentDays);
        xtpAccounting.Controls.Add(spnAccountingAveragePaymentDays);
        xtpAccounting.Controls.Add(lblAccountingPaymentMethod);
        xtpAccounting.Controls.Add(chkAccountingUsesWithholdingBase);
        xtpAccounting.Controls.Add(lueAccountingPaymentMethod);
        xtpAccounting.Controls.Add(lblAccountingPaymentPriority);
        xtpAccounting.Controls.Add(chkAccountingBlocked);
        xtpAccounting.Controls.Add(lueAccountingPaymentPriority);
        xtpAccounting.Controls.Add(chkAccountingAllowsPartialPayments);
        xtpAccounting.Controls.Add(lblAccountingRequiredPaymentDay);
        xtpAccounting.Controls.Add(lblAccountingConciliationRequired);
        xtpAccounting.Controls.Add(lueAccountingRequiredPaymentDay);
        xtpAccounting.Controls.Add(lblAccountingUsesWithholdingBase);
        xtpAccounting.Controls.Add(lblAccountingPaymentDocumentType);
        xtpAccounting.Controls.Add(lblAccountingBlocked);
        xtpAccounting.Controls.Add(lueAccountingPaymentDocumentType);
        xtpAccounting.Controls.Add(lblAccountingAllowsPartialPayments);
        xtpAccounting.Controls.Add(lblAccountingApprovalFlow);
        xtpAccounting.Controls.Add(lblAccountingAllowsCompensation);
        xtpAccounting.Controls.Add(lueAccountingApprovalFlow);
        xtpAccounting.Controls.Add(lblAccountingAllowsAdvance);
        xtpAccounting.Controls.Add(lblAccountingRequiresProvision);
        xtpAccounting.Controls.Add(lblAccountingBySupplier);
        xtpAccounting.Controls.Add(chkAccountingAllowsCompensation);
        xtpAccounting.Controls.Add(chkAccountingAllowsAdvance);
        xtpAccounting.Controls.Add(chkAccountingRequiresProvision);
        xtpAccounting.Controls.Add(chkAccountingBySupplier);
        xtpAccounting.Controls.Add(lueAccountingRetentionPayableAccount);
        xtpAccounting.Controls.Add(lueAccountingDiscountAccount);
        xtpAccounting.Controls.Add(lueAccountingClearingAccount);
        xtpAccounting.Controls.Add(lueAccountingRoundingAccount);
        xtpAccounting.Controls.Add(lueAccountingDifferenceAccount);
        xtpAccounting.Controls.Add(lueAccountingAdvanceAccount);
        xtpAccounting.Controls.Add(lueAccountingDefaultExpenseAccount);
        xtpAccounting.Controls.Add(lueAccountingSupplierAccount);
        xtpAccounting.Controls.Add(lblAccountingAccountsTitle);
        xtpAccounting.Controls.Add(lblAccountingSupplierAccount);
        xtpAccounting.Controls.Add(lblAccountingAdvanceAccount);
        xtpAccounting.Controls.Add(lblAccountingDefaultExpenseAccount);
        xtpAccounting.Controls.Add(lblAccountingDifferenceAccount);
        xtpAccounting.Controls.Add(lblAccountingRetentionPayableAccount);
        xtpAccounting.Controls.Add(lblAccountingRoundingAccount);
        xtpAccounting.Controls.Add(lblAccountingClearingAccount);
        xtpAccounting.Controls.Add(lblAccountingDiscountAccount);
        xtpAccounting.Name = "xtpAccounting";
        xtpAccounting.Size = new Size(1094, 402);
        xtpAccounting.Text = "Contabilidad";
        // 
        // lblAccountingDimensionsTitle
        // 
        lblAccountingDimensionsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAccountingDimensionsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAccountingDimensionsTitle.Appearance.Options.UseFont = true;
        lblAccountingDimensionsTitle.Appearance.Options.UseForeColor = true;
        lblAccountingDimensionsTitle.Location = new Point(660, 258);
        lblAccountingDimensionsTitle.Name = "lblAccountingDimensionsTitle";
        lblAccountingDimensionsTitle.Size = new Size(227, 20);
        lblAccountingDimensionsTitle.TabIndex = 66;
        lblAccountingDimensionsTitle.Text = "4. Dimensiones / centros de costo";
        // 
        // lblAccountingBranch
        // 
        lblAccountingBranch.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingBranch.Appearance.Options.UseFont = true;
        lblAccountingBranch.Location = new Point(660, 292);
        lblAccountingBranch.Name = "lblAccountingBranch";
        lblAccountingBranch.Size = new Size(49, 15);
        lblAccountingBranch.TabIndex = 67;
        lblAccountingBranch.Text = "Sucursal:";
        // 
        // lueAccountingBranch
        // 
        lueAccountingBranch.Location = new Point(813, 289);
        lueAccountingBranch.Name = "lueAccountingBranch";
        lueAccountingBranch.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingBranch.Properties.Appearance.Options.UseFont = true;
        lueAccountingBranch.Properties.AutoHeight = false;
        lueAccountingBranch.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingBranch.Properties.NullText = "";
        lueAccountingBranch.Properties.PopupView = grvAccountingBranchLookup;
        lueAccountingBranch.Size = new Size(245, 22);
        lueAccountingBranch.TabIndex = 68;
        // 
        // grvAccountingBranchLookup
        // 
        grvAccountingBranchLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingBranchLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingBranchLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingBranchLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingBranchLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingBranchLookup.Name = "grvAccountingBranchLookup";
        grvAccountingBranchLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingBranchLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblAccountingDepartment
        // 
        lblAccountingDepartment.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingDepartment.Appearance.Options.UseFont = true;
        lblAccountingDepartment.Location = new Point(660, 320);
        lblAccountingDepartment.Name = "lblAccountingDepartment";
        lblAccountingDepartment.Size = new Size(79, 15);
        lblAccountingDepartment.TabIndex = 69;
        lblAccountingDepartment.Text = "Departamento:";
        // 
        // lueAccountingDepartment
        // 
        lueAccountingDepartment.Location = new Point(813, 317);
        lueAccountingDepartment.Name = "lueAccountingDepartment";
        lueAccountingDepartment.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingDepartment.Properties.Appearance.Options.UseFont = true;
        lueAccountingDepartment.Properties.AutoHeight = false;
        lueAccountingDepartment.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingDepartment.Properties.NullText = "";
        lueAccountingDepartment.Properties.PopupView = grvAccountingDepartmentLookup;
        lueAccountingDepartment.Size = new Size(245, 22);
        lueAccountingDepartment.TabIndex = 70;
        // 
        // grvAccountingDepartmentLookup
        // 
        grvAccountingDepartmentLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingDepartmentLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingDepartmentLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingDepartmentLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingDepartmentLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingDepartmentLookup.Name = "grvAccountingDepartmentLookup";
        grvAccountingDepartmentLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingDepartmentLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblAccountingBusinessLine
        // 
        lblAccountingBusinessLine.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingBusinessLine.Appearance.Options.UseFont = true;
        lblAccountingBusinessLine.Location = new Point(660, 348);
        lblAccountingBusinessLine.Name = "lblAccountingBusinessLine";
        lblAccountingBusinessLine.Size = new Size(84, 15);
        lblAccountingBusinessLine.TabIndex = 71;
        lblAccountingBusinessLine.Text = "Linea negocio:";
        // 
        // lueAccountingBusinessLine
        // 
        lueAccountingBusinessLine.Location = new Point(813, 345);
        lueAccountingBusinessLine.Name = "lueAccountingBusinessLine";
        lueAccountingBusinessLine.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingBusinessLine.Properties.Appearance.Options.UseFont = true;
        lueAccountingBusinessLine.Properties.AutoHeight = false;
        lueAccountingBusinessLine.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingBusinessLine.Properties.NullText = "";
        lueAccountingBusinessLine.Properties.PopupView = grvAccountingBusinessLineLookup;
        lueAccountingBusinessLine.Size = new Size(245, 22);
        lueAccountingBusinessLine.TabIndex = 72;
        // 
        // grvAccountingBusinessLineLookup
        // 
        grvAccountingBusinessLineLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingBusinessLineLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingBusinessLineLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingBusinessLineLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingBusinessLineLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingBusinessLineLookup.Name = "grvAccountingBusinessLineLookup";
        grvAccountingBusinessLineLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingBusinessLineLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblAccountingCostCenter
        // 
        lblAccountingCostCenter.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingCostCenter.Appearance.Options.UseFont = true;
        lblAccountingCostCenter.Location = new Point(12, 300);
        lblAccountingCostCenter.Name = "lblAccountingCostCenter";
        lblAccountingCostCenter.Size = new Size(84, 15);
        lblAccountingCostCenter.TabIndex = 73;
        lblAccountingCostCenter.Text = "Centro de costo:";
        // 
        // lueAccountingCostCenter
        // 
        lueAccountingCostCenter.Location = new Point(165, 297);
        lueAccountingCostCenter.Name = "lueAccountingCostCenter";
        lueAccountingCostCenter.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingCostCenter.Properties.Appearance.Options.UseFont = true;
        lueAccountingCostCenter.Properties.AutoHeight = false;
        lueAccountingCostCenter.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingCostCenter.Properties.NullText = "";
        lueAccountingCostCenter.Properties.PopupView = grvAccountingCostCenterLookup;
        lueAccountingCostCenter.Size = new Size(195, 22);
        lueAccountingCostCenter.TabIndex = 74;
        // 
        // grvAccountingCostCenterLookup
        // 
        grvAccountingCostCenterLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingCostCenterLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingCostCenterLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingCostCenterLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingCostCenterLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingCostCenterLookup.Name = "grvAccountingCostCenterLookup";
        grvAccountingCostCenterLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingCostCenterLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblAccountingProject
        // 
        lblAccountingProject.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingProject.Appearance.Options.UseFont = true;
        lblAccountingProject.Location = new Point(12, 328);
        lblAccountingProject.Name = "lblAccountingProject";
        lblAccountingProject.Size = new Size(48, 15);
        lblAccountingProject.TabIndex = 75;
        lblAccountingProject.Text = "Proyecto:";
        // 
        // lueAccountingProject
        // 
        lueAccountingProject.Location = new Point(165, 325);
        lueAccountingProject.Name = "lueAccountingProject";
        lueAccountingProject.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingProject.Properties.Appearance.Options.UseFont = true;
        lueAccountingProject.Properties.AutoHeight = false;
        lueAccountingProject.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingProject.Properties.NullText = "";
        lueAccountingProject.Properties.PopupView = grvAccountingProjectLookup;
        lueAccountingProject.Size = new Size(195, 22);
        lueAccountingProject.TabIndex = 76;
        // 
        // grvAccountingProjectLookup
        // 
        grvAccountingProjectLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingProjectLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingProjectLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingProjectLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingProjectLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingProjectLookup.Name = "grvAccountingProjectLookup";
        grvAccountingProjectLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingProjectLookup.OptionsView.ShowGroupPanel = false;
        // 
        // chkAccountingConciliationRequired
        // 
        chkAccountingConciliationRequired.Location = new Point(555, 236);
        chkAccountingConciliationRequired.Name = "chkAccountingConciliationRequired";
        chkAccountingConciliationRequired.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkAccountingConciliationRequired.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        chkAccountingConciliationRequired.Properties.Appearance.Options.UseFont = true;
        chkAccountingConciliationRequired.Properties.Appearance.Options.UseForeColor = true;
        chkAccountingConciliationRequired.Properties.OffText = "";
        chkAccountingConciliationRequired.Properties.OnText = "";
        chkAccountingConciliationRequired.Size = new Size(56, 20);
        chkAccountingConciliationRequired.TabIndex = 65;
        // 
        // spnAccountingPaymentTolerance
        // 
        spnAccountingPaymentTolerance.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnAccountingPaymentTolerance.Location = new Point(813, 213);
        spnAccountingPaymentTolerance.Name = "spnAccountingPaymentTolerance";
        spnAccountingPaymentTolerance.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnAccountingPaymentTolerance.Properties.Appearance.Options.UseFont = true;
        spnAccountingPaymentTolerance.Properties.Appearance.Options.UseTextOptions = true;
        spnAccountingPaymentTolerance.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnAccountingPaymentTolerance.Properties.AutoHeight = false;
        spnAccountingPaymentTolerance.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnAccountingPaymentTolerance.Properties.DisplayFormat.FormatString = "n2";
        spnAccountingPaymentTolerance.Properties.DisplayFormat.FormatType = FormatType.Numeric;
        spnAccountingPaymentTolerance.Size = new Size(166, 22);
        spnAccountingPaymentTolerance.TabIndex = 16;
        // 
        // lblAccountingPaymentTolerance
        // 
        lblAccountingPaymentTolerance.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingPaymentTolerance.Appearance.Options.UseFont = true;
        lblAccountingPaymentTolerance.Location = new Point(660, 216);
        lblAccountingPaymentTolerance.Name = "lblAccountingPaymentTolerance";
        lblAccountingPaymentTolerance.Size = new Size(58, 15);
        lblAccountingPaymentTolerance.TabIndex = 15;
        lblAccountingPaymentTolerance.Text = "Tolerancia:";
        // 
        // lblAccountingAveragePaymentDays
        // 
        lblAccountingAveragePaymentDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingAveragePaymentDays.Appearance.Options.UseFont = true;
        lblAccountingAveragePaymentDays.Location = new Point(660, 188);
        lblAccountingAveragePaymentDays.Name = "lblAccountingAveragePaymentDays";
        lblAccountingAveragePaymentDays.Size = new Size(68, 15);
        lblAccountingAveragePaymentDays.TabIndex = 13;
        lblAccountingAveragePaymentDays.Text = "Plazo medio:";
        // 
        // spnAccountingAveragePaymentDays
        // 
        spnAccountingAveragePaymentDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnAccountingAveragePaymentDays.Location = new Point(813, 185);
        spnAccountingAveragePaymentDays.Name = "spnAccountingAveragePaymentDays";
        spnAccountingAveragePaymentDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnAccountingAveragePaymentDays.Properties.Appearance.Options.UseFont = true;
        spnAccountingAveragePaymentDays.Properties.Appearance.Options.UseTextOptions = true;
        spnAccountingAveragePaymentDays.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnAccountingAveragePaymentDays.Properties.AutoHeight = false;
        spnAccountingAveragePaymentDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnAccountingAveragePaymentDays.Size = new Size(166, 22);
        spnAccountingAveragePaymentDays.TabIndex = 14;
        // 
        // lblAccountingPaymentMethod
        // 
        lblAccountingPaymentMethod.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingPaymentMethod.Appearance.Options.UseFont = true;
        lblAccountingPaymentMethod.Location = new Point(660, 48);
        lblAccountingPaymentMethod.Name = "lblAccountingPaymentMethod";
        lblAccountingPaymentMethod.Size = new Size(124, 15);
        lblAccountingPaymentMethod.TabIndex = 1;
        lblAccountingPaymentMethod.Text = "Metodo pago contable:";
        // 
        // chkAccountingUsesWithholdingBase
        // 
        chkAccountingUsesWithholdingBase.Location = new Point(555, 209);
        chkAccountingUsesWithholdingBase.Name = "chkAccountingUsesWithholdingBase";
        chkAccountingUsesWithholdingBase.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkAccountingUsesWithholdingBase.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        chkAccountingUsesWithholdingBase.Properties.Appearance.Options.UseFont = true;
        chkAccountingUsesWithholdingBase.Properties.Appearance.Options.UseForeColor = true;
        chkAccountingUsesWithholdingBase.Properties.OffText = "";
        chkAccountingUsesWithholdingBase.Properties.OnText = "";
        chkAccountingUsesWithholdingBase.Size = new Size(56, 20);
        chkAccountingUsesWithholdingBase.TabIndex = 64;
        // 
        // lueAccountingPaymentMethod
        // 
        lueAccountingPaymentMethod.Location = new Point(813, 45);
        lueAccountingPaymentMethod.Name = "lueAccountingPaymentMethod";
        lueAccountingPaymentMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingPaymentMethod.Properties.Appearance.Options.UseFont = true;
        lueAccountingPaymentMethod.Properties.AutoHeight = false;
        lueAccountingPaymentMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingPaymentMethod.Properties.NullText = "";
        lueAccountingPaymentMethod.Size = new Size(166, 22);
        lueAccountingPaymentMethod.TabIndex = 2;
        // 
        // lblAccountingPaymentPriority
        // 
        lblAccountingPaymentPriority.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingPaymentPriority.Appearance.Options.UseFont = true;
        lblAccountingPaymentPriority.Location = new Point(660, 76);
        lblAccountingPaymentPriority.Name = "lblAccountingPaymentPriority";
        lblAccountingPaymentPriority.Size = new Size(81, 15);
        lblAccountingPaymentPriority.TabIndex = 3;
        lblAccountingPaymentPriority.Text = "Prioridad pago:";
        // 
        // chkAccountingBlocked
        // 
        chkAccountingBlocked.Location = new Point(555, 182);
        chkAccountingBlocked.Name = "chkAccountingBlocked";
        chkAccountingBlocked.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkAccountingBlocked.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        chkAccountingBlocked.Properties.Appearance.Options.UseFont = true;
        chkAccountingBlocked.Properties.Appearance.Options.UseForeColor = true;
        chkAccountingBlocked.Properties.OffText = "";
        chkAccountingBlocked.Properties.OnText = "";
        chkAccountingBlocked.Size = new Size(56, 20);
        chkAccountingBlocked.TabIndex = 63;
        // 
        // lueAccountingPaymentPriority
        // 
        lueAccountingPaymentPriority.Location = new Point(813, 73);
        lueAccountingPaymentPriority.Name = "lueAccountingPaymentPriority";
        lueAccountingPaymentPriority.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingPaymentPriority.Properties.Appearance.Options.UseFont = true;
        lueAccountingPaymentPriority.Properties.AutoHeight = false;
        lueAccountingPaymentPriority.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingPaymentPriority.Properties.NullText = "";
        lueAccountingPaymentPriority.Size = new Size(166, 22);
        lueAccountingPaymentPriority.TabIndex = 4;
        // 
        // chkAccountingAllowsPartialPayments
        // 
        chkAccountingAllowsPartialPayments.Location = new Point(555, 155);
        chkAccountingAllowsPartialPayments.Name = "chkAccountingAllowsPartialPayments";
        chkAccountingAllowsPartialPayments.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkAccountingAllowsPartialPayments.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        chkAccountingAllowsPartialPayments.Properties.Appearance.Options.UseFont = true;
        chkAccountingAllowsPartialPayments.Properties.Appearance.Options.UseForeColor = true;
        chkAccountingAllowsPartialPayments.Properties.OffText = "";
        chkAccountingAllowsPartialPayments.Properties.OnText = "";
        chkAccountingAllowsPartialPayments.Size = new Size(56, 20);
        chkAccountingAllowsPartialPayments.TabIndex = 62;
        // 
        // lblAccountingRequiredPaymentDay
        // 
        lblAccountingRequiredPaymentDay.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingRequiredPaymentDay.Appearance.Options.UseFont = true;
        lblAccountingRequiredPaymentDay.Location = new Point(660, 104);
        lblAccountingRequiredPaymentDay.Name = "lblAccountingRequiredPaymentDay";
        lblAccountingRequiredPaymentDay.Size = new Size(99, 15);
        lblAccountingRequiredPaymentDay.TabIndex = 5;
        lblAccountingRequiredPaymentDay.Text = "Dia sugerido pago:";
        // 
        // lblAccountingConciliationRequired
        // 
        lblAccountingConciliationRequired.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingConciliationRequired.Appearance.Options.UseFont = true;
        lblAccountingConciliationRequired.Location = new Point(409, 244);
        lblAccountingConciliationRequired.Name = "lblAccountingConciliationRequired";
        lblAccountingConciliationRequired.Size = new Size(129, 15);
        lblAccountingConciliationRequired.TabIndex = 61;
        lblAccountingConciliationRequired.Text = "Conciliación obligatoria:";
        // 
        // lueAccountingRequiredPaymentDay
        // 
        lueAccountingRequiredPaymentDay.Location = new Point(813, 101);
        lueAccountingRequiredPaymentDay.Name = "lueAccountingRequiredPaymentDay";
        lueAccountingRequiredPaymentDay.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingRequiredPaymentDay.Properties.Appearance.Options.UseFont = true;
        lueAccountingRequiredPaymentDay.Properties.AutoHeight = false;
        lueAccountingRequiredPaymentDay.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingRequiredPaymentDay.Properties.NullText = "";
        lueAccountingRequiredPaymentDay.Size = new Size(166, 22);
        lueAccountingRequiredPaymentDay.TabIndex = 6;
        // 
        // lblAccountingUsesWithholdingBase
        // 
        lblAccountingUsesWithholdingBase.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingUsesWithholdingBase.Appearance.Options.UseFont = true;
        lblAccountingUsesWithholdingBase.Location = new Point(409, 216);
        lblAccountingUsesWithholdingBase.Name = "lblAccountingUsesWithholdingBase";
        lblAccountingUsesWithholdingBase.Size = new Size(126, 15);
        lblAccountingUsesWithholdingBase.TabIndex = 60;
        lblAccountingUsesWithholdingBase.Text = "Base retención de pago:";
        // 
        // lblAccountingPaymentDocumentType
        // 
        lblAccountingPaymentDocumentType.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingPaymentDocumentType.Appearance.Options.UseFont = true;
        lblAccountingPaymentDocumentType.Location = new Point(660, 160);
        lblAccountingPaymentDocumentType.Name = "lblAccountingPaymentDocumentType";
        lblAccountingPaymentDocumentType.Size = new Size(122, 15);
        lblAccountingPaymentDocumentType.TabIndex = 9;
        lblAccountingPaymentDocumentType.Text = "Tipo documento pago:";
        // 
        // lblAccountingBlocked
        // 
        lblAccountingBlocked.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingBlocked.Appearance.Options.UseFont = true;
        lblAccountingBlocked.Location = new Point(409, 188);
        lblAccountingBlocked.Name = "lblAccountingBlocked";
        lblAccountingBlocked.Size = new Size(60, 15);
        lblAccountingBlocked.TabIndex = 59;
        lblAccountingBlocked.Text = "Bloqueado:";
        // 
        // lueAccountingPaymentDocumentType
        // 
        lueAccountingPaymentDocumentType.Location = new Point(813, 157);
        lueAccountingPaymentDocumentType.Name = "lueAccountingPaymentDocumentType";
        lueAccountingPaymentDocumentType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingPaymentDocumentType.Properties.Appearance.Options.UseFont = true;
        lueAccountingPaymentDocumentType.Properties.AutoHeight = false;
        lueAccountingPaymentDocumentType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingPaymentDocumentType.Properties.NullText = "";
        lueAccountingPaymentDocumentType.Size = new Size(166, 22);
        lueAccountingPaymentDocumentType.TabIndex = 10;
        // 
        // lblAccountingAllowsPartialPayments
        // 
        lblAccountingAllowsPartialPayments.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingAllowsPartialPayments.Appearance.Options.UseFont = true;
        lblAccountingAllowsPartialPayments.Location = new Point(409, 160);
        lblAccountingAllowsPartialPayments.Name = "lblAccountingAllowsPartialPayments";
        lblAccountingAllowsPartialPayments.Size = new Size(128, 15);
        lblAccountingAllowsPartialPayments.TabIndex = 58;
        lblAccountingAllowsPartialPayments.Text = "Permite pagos parciales:";
        // 
        // lblAccountingApprovalFlow
        // 
        lblAccountingApprovalFlow.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingApprovalFlow.Appearance.Options.UseFont = true;
        lblAccountingApprovalFlow.Location = new Point(660, 132);
        lblAccountingApprovalFlow.Name = "lblAccountingApprovalFlow";
        lblAccountingApprovalFlow.Size = new Size(92, 15);
        lblAccountingApprovalFlow.TabIndex = 11;
        lblAccountingApprovalFlow.Text = "Flujo aprobacion:";
        // 
        // lblAccountingAllowsCompensation
        // 
        lblAccountingAllowsCompensation.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingAllowsCompensation.Appearance.Options.UseFont = true;
        lblAccountingAllowsCompensation.Location = new Point(409, 132);
        lblAccountingAllowsCompensation.Name = "lblAccountingAllowsCompensation";
        lblAccountingAllowsCompensation.Size = new Size(125, 15);
        lblAccountingAllowsCompensation.TabIndex = 57;
        lblAccountingAllowsCompensation.Text = "Permite compensación:";
        // 
        // lueAccountingApprovalFlow
        // 
        lueAccountingApprovalFlow.Location = new Point(813, 129);
        lueAccountingApprovalFlow.Name = "lueAccountingApprovalFlow";
        lueAccountingApprovalFlow.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingApprovalFlow.Properties.Appearance.Options.UseFont = true;
        lueAccountingApprovalFlow.Properties.AutoHeight = false;
        lueAccountingApprovalFlow.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingApprovalFlow.Properties.NullText = "";
        lueAccountingApprovalFlow.Size = new Size(166, 22);
        lueAccountingApprovalFlow.TabIndex = 12;
        // 
        // lblAccountingAllowsAdvance
        // 
        lblAccountingAllowsAdvance.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingAllowsAdvance.Appearance.Options.UseFont = true;
        lblAccountingAllowsAdvance.Location = new Point(409, 104);
        lblAccountingAllowsAdvance.Name = "lblAccountingAllowsAdvance";
        lblAccountingAllowsAdvance.Size = new Size(95, 15);
        lblAccountingAllowsAdvance.TabIndex = 56;
        lblAccountingAllowsAdvance.Text = "Permite anticipos:";
        // 
        // lblAccountingRequiresProvision
        // 
        lblAccountingRequiresProvision.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingRequiresProvision.Appearance.Options.UseFont = true;
        lblAccountingRequiresProvision.Location = new Point(409, 76);
        lblAccountingRequiresProvision.Name = "lblAccountingRequiresProvision";
        lblAccountingRequiresProvision.Size = new Size(101, 15);
        lblAccountingRequiresProvision.TabIndex = 55;
        lblAccountingRequiresProvision.Text = "Requiere provisión:";
        // 
        // lblAccountingBySupplier
        // 
        lblAccountingBySupplier.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingBySupplier.Appearance.Options.UseFont = true;
        lblAccountingBySupplier.Location = new Point(409, 48);
        lblAccountingBySupplier.Name = "lblAccountingBySupplier";
        lblAccountingBySupplier.Size = new Size(140, 15);
        lblAccountingBySupplier.TabIndex = 54;
        lblAccountingBySupplier.Text = "Contabiliza por proveedor:";
        // 
        // chkAccountingAllowsCompensation
        // 
        chkAccountingAllowsCompensation.Location = new Point(555, 128);
        chkAccountingAllowsCompensation.Name = "chkAccountingAllowsCompensation";
        chkAccountingAllowsCompensation.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkAccountingAllowsCompensation.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        chkAccountingAllowsCompensation.Properties.Appearance.Options.UseFont = true;
        chkAccountingAllowsCompensation.Properties.Appearance.Options.UseForeColor = true;
        chkAccountingAllowsCompensation.Properties.OffText = "";
        chkAccountingAllowsCompensation.Properties.OnText = "";
        chkAccountingAllowsCompensation.Size = new Size(56, 20);
        chkAccountingAllowsCompensation.TabIndex = 53;
        // 
        // chkAccountingAllowsAdvance
        // 
        chkAccountingAllowsAdvance.Location = new Point(555, 101);
        chkAccountingAllowsAdvance.Name = "chkAccountingAllowsAdvance";
        chkAccountingAllowsAdvance.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkAccountingAllowsAdvance.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        chkAccountingAllowsAdvance.Properties.Appearance.Options.UseFont = true;
        chkAccountingAllowsAdvance.Properties.Appearance.Options.UseForeColor = true;
        chkAccountingAllowsAdvance.Properties.OffText = "";
        chkAccountingAllowsAdvance.Properties.OnText = "";
        chkAccountingAllowsAdvance.Size = new Size(56, 20);
        chkAccountingAllowsAdvance.TabIndex = 52;
        // 
        // chkAccountingRequiresProvision
        // 
        chkAccountingRequiresProvision.Location = new Point(555, 74);
        chkAccountingRequiresProvision.Name = "chkAccountingRequiresProvision";
        chkAccountingRequiresProvision.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkAccountingRequiresProvision.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        chkAccountingRequiresProvision.Properties.Appearance.Options.UseFont = true;
        chkAccountingRequiresProvision.Properties.Appearance.Options.UseForeColor = true;
        chkAccountingRequiresProvision.Properties.OffText = "";
        chkAccountingRequiresProvision.Properties.OnText = "";
        chkAccountingRequiresProvision.Size = new Size(56, 20);
        chkAccountingRequiresProvision.TabIndex = 51;
        // 
        // chkAccountingBySupplier
        // 
        chkAccountingBySupplier.Location = new Point(555, 47);
        chkAccountingBySupplier.Name = "chkAccountingBySupplier";
        chkAccountingBySupplier.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        chkAccountingBySupplier.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        chkAccountingBySupplier.Properties.Appearance.Options.UseFont = true;
        chkAccountingBySupplier.Properties.Appearance.Options.UseForeColor = true;
        chkAccountingBySupplier.Properties.OffText = "";
        chkAccountingBySupplier.Properties.OnText = "";
        chkAccountingBySupplier.Size = new Size(56, 20);
        chkAccountingBySupplier.TabIndex = 50;
        // 
        // lueAccountingRetentionPayableAccount
        // 
        lueAccountingRetentionPayableAccount.Location = new Point(165, 241);
        lueAccountingRetentionPayableAccount.Name = "lueAccountingRetentionPayableAccount";
        lueAccountingRetentionPayableAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingRetentionPayableAccount.Properties.Appearance.Options.UseFont = true;
        lueAccountingRetentionPayableAccount.Properties.AutoHeight = false;
        lueAccountingRetentionPayableAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingRetentionPayableAccount.Properties.NullText = "";
        lueAccountingRetentionPayableAccount.Properties.PopupView = grvAccountingRetentionPayableAccountLookup;
        lueAccountingRetentionPayableAccount.Size = new Size(195, 22);
        lueAccountingRetentionPayableAccount.TabIndex = 49;
        // 
        // grvAccountingRetentionPayableAccountLookup
        // 
        grvAccountingRetentionPayableAccountLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingRetentionPayableAccountLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingRetentionPayableAccountLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingRetentionPayableAccountLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingRetentionPayableAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingRetentionPayableAccountLookup.Name = "grvAccountingRetentionPayableAccountLookup";
        grvAccountingRetentionPayableAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingRetentionPayableAccountLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lueAccountingDiscountAccount
        // 
        lueAccountingDiscountAccount.Location = new Point(165, 213);
        lueAccountingDiscountAccount.Name = "lueAccountingDiscountAccount";
        lueAccountingDiscountAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingDiscountAccount.Properties.Appearance.Options.UseFont = true;
        lueAccountingDiscountAccount.Properties.AutoHeight = false;
        lueAccountingDiscountAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingDiscountAccount.Properties.NullText = "";
        lueAccountingDiscountAccount.Properties.PopupView = grvAccountingDiscountAccountLookup;
        lueAccountingDiscountAccount.Size = new Size(195, 22);
        lueAccountingDiscountAccount.TabIndex = 48;
        // 
        // grvAccountingDiscountAccountLookup
        // 
        grvAccountingDiscountAccountLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingDiscountAccountLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingDiscountAccountLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingDiscountAccountLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingDiscountAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingDiscountAccountLookup.Name = "grvAccountingDiscountAccountLookup";
        grvAccountingDiscountAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingDiscountAccountLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lueAccountingClearingAccount
        // 
        lueAccountingClearingAccount.Location = new Point(165, 185);
        lueAccountingClearingAccount.Name = "lueAccountingClearingAccount";
        lueAccountingClearingAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingClearingAccount.Properties.Appearance.Options.UseFont = true;
        lueAccountingClearingAccount.Properties.AutoHeight = false;
        lueAccountingClearingAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingClearingAccount.Properties.NullText = "";
        lueAccountingClearingAccount.Properties.PopupView = grvAccountingClearingAccountLookup;
        lueAccountingClearingAccount.Size = new Size(195, 22);
        lueAccountingClearingAccount.TabIndex = 47;
        // 
        // grvAccountingClearingAccountLookup
        // 
        grvAccountingClearingAccountLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingClearingAccountLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingClearingAccountLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingClearingAccountLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingClearingAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingClearingAccountLookup.Name = "grvAccountingClearingAccountLookup";
        grvAccountingClearingAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingClearingAccountLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lueAccountingRoundingAccount
        // 
        lueAccountingRoundingAccount.Location = new Point(165, 157);
        lueAccountingRoundingAccount.Name = "lueAccountingRoundingAccount";
        lueAccountingRoundingAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingRoundingAccount.Properties.Appearance.Options.UseFont = true;
        lueAccountingRoundingAccount.Properties.AutoHeight = false;
        lueAccountingRoundingAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingRoundingAccount.Properties.NullText = "";
        lueAccountingRoundingAccount.Properties.PopupView = grvAccountingRoundingAccountLookup;
        lueAccountingRoundingAccount.Size = new Size(195, 22);
        lueAccountingRoundingAccount.TabIndex = 46;
        // 
        // grvAccountingRoundingAccountLookup
        // 
        grvAccountingRoundingAccountLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingRoundingAccountLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingRoundingAccountLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingRoundingAccountLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingRoundingAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingRoundingAccountLookup.Name = "grvAccountingRoundingAccountLookup";
        grvAccountingRoundingAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingRoundingAccountLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lueAccountingDifferenceAccount
        // 
        lueAccountingDifferenceAccount.Location = new Point(165, 129);
        lueAccountingDifferenceAccount.Name = "lueAccountingDifferenceAccount";
        lueAccountingDifferenceAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingDifferenceAccount.Properties.Appearance.Options.UseFont = true;
        lueAccountingDifferenceAccount.Properties.AutoHeight = false;
        lueAccountingDifferenceAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingDifferenceAccount.Properties.NullText = "";
        lueAccountingDifferenceAccount.Properties.PopupView = grvAccountingDifferenceAccountLookup;
        lueAccountingDifferenceAccount.Size = new Size(195, 22);
        lueAccountingDifferenceAccount.TabIndex = 45;
        // 
        // grvAccountingDifferenceAccountLookup
        // 
        grvAccountingDifferenceAccountLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingDifferenceAccountLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingDifferenceAccountLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingDifferenceAccountLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingDifferenceAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingDifferenceAccountLookup.Name = "grvAccountingDifferenceAccountLookup";
        grvAccountingDifferenceAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingDifferenceAccountLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lueAccountingAdvanceAccount
        // 
        lueAccountingAdvanceAccount.Location = new Point(165, 73);
        lueAccountingAdvanceAccount.Name = "lueAccountingAdvanceAccount";
        lueAccountingAdvanceAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingAdvanceAccount.Properties.Appearance.Options.UseFont = true;
        lueAccountingAdvanceAccount.Properties.AutoHeight = false;
        lueAccountingAdvanceAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingAdvanceAccount.Properties.NullText = "";
        lueAccountingAdvanceAccount.Properties.PopupView = grvAccountingAdvanceAccountLookup;
        lueAccountingAdvanceAccount.Size = new Size(195, 22);
        lueAccountingAdvanceAccount.TabIndex = 44;
        // 
        // grvAccountingAdvanceAccountLookup
        // 
        grvAccountingAdvanceAccountLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingAdvanceAccountLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingAdvanceAccountLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingAdvanceAccountLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingAdvanceAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingAdvanceAccountLookup.Name = "grvAccountingAdvanceAccountLookup";
        grvAccountingAdvanceAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingAdvanceAccountLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lueAccountingDefaultExpenseAccount
        // 
        lueAccountingDefaultExpenseAccount.Location = new Point(165, 101);
        lueAccountingDefaultExpenseAccount.Name = "lueAccountingDefaultExpenseAccount";
        lueAccountingDefaultExpenseAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingDefaultExpenseAccount.Properties.Appearance.Options.UseFont = true;
        lueAccountingDefaultExpenseAccount.Properties.AutoHeight = false;
        lueAccountingDefaultExpenseAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingDefaultExpenseAccount.Properties.NullText = "";
        lueAccountingDefaultExpenseAccount.Properties.PopupView = grvAccountingDefaultExpenseAccountLookup;
        lueAccountingDefaultExpenseAccount.Size = new Size(195, 22);
        lueAccountingDefaultExpenseAccount.TabIndex = 43;
        // 
        // grvAccountingDefaultExpenseAccountLookup
        // 
        grvAccountingDefaultExpenseAccountLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingDefaultExpenseAccountLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingDefaultExpenseAccountLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingDefaultExpenseAccountLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingDefaultExpenseAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingDefaultExpenseAccountLookup.Name = "grvAccountingDefaultExpenseAccountLookup";
        grvAccountingDefaultExpenseAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingDefaultExpenseAccountLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lueAccountingSupplierAccount
        // 
        lueAccountingSupplierAccount.Location = new Point(165, 45);
        lueAccountingSupplierAccount.Name = "lueAccountingSupplierAccount";
        lueAccountingSupplierAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingSupplierAccount.Properties.Appearance.Options.UseFont = true;
        lueAccountingSupplierAccount.Properties.AutoHeight = false;
        lueAccountingSupplierAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingSupplierAccount.Properties.NullText = "";
        lueAccountingSupplierAccount.Properties.PopupView = grvAccountingSupplierAccountLookup;
        lueAccountingSupplierAccount.Size = new Size(195, 22);
        lueAccountingSupplierAccount.TabIndex = 42;
        // 
        // grvAccountingSupplierAccountLookup
        // 
        grvAccountingSupplierAccountLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvAccountingSupplierAccountLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvAccountingSupplierAccountLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvAccountingSupplierAccountLookup.Appearance.Row.Options.UseFont = true;
        grvAccountingSupplierAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvAccountingSupplierAccountLookup.Name = "grvAccountingSupplierAccountLookup";
        grvAccountingSupplierAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvAccountingSupplierAccountLookup.OptionsView.ShowGroupPanel = false;
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
        lblAccountingAccountsTitle.TabIndex = 17;
        lblAccountingAccountsTitle.Text = "1. Cuentas contables";
        // 
        // lblAccountingSupplierAccount
        // 
        lblAccountingSupplierAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingSupplierAccount.Appearance.Options.UseFont = true;
        lblAccountingSupplierAccount.Location = new Point(12, 48);
        lblAccountingSupplierAccount.Name = "lblAccountingSupplierAccount";
        lblAccountingSupplierAccount.Size = new Size(147, 15);
        lblAccountingSupplierAccount.TabIndex = 18;
        lblAccountingSupplierAccount.Text = "Cuenta asociada proveedor:";
        // 
        // lblAccountingAdvanceAccount
        // 
        lblAccountingAdvanceAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingAdvanceAccount.Appearance.Options.UseFont = true;
        lblAccountingAdvanceAccount.Location = new Point(12, 76);
        lblAccountingAdvanceAccount.Name = "lblAccountingAdvanceAccount";
        lblAccountingAdvanceAccount.Size = new Size(92, 15);
        lblAccountingAdvanceAccount.TabIndex = 20;
        lblAccountingAdvanceAccount.Text = "Cuenta anticipos:";
        // 
        // lblAccountingDefaultExpenseAccount
        // 
        lblAccountingDefaultExpenseAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingDefaultExpenseAccount.Appearance.Options.UseFont = true;
        lblAccountingDefaultExpenseAccount.Location = new Point(12, 104);
        lblAccountingDefaultExpenseAccount.Name = "lblAccountingDefaultExpenseAccount";
        lblAccountingDefaultExpenseAccount.Size = new Size(142, 15);
        lblAccountingDefaultExpenseAccount.TabIndex = 22;
        lblAccountingDefaultExpenseAccount.Text = "Cuenta gastos por defecto:";
        // 
        // lblAccountingDifferenceAccount
        // 
        lblAccountingDifferenceAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingDifferenceAccount.Appearance.Options.UseFont = true;
        lblAccountingDifferenceAccount.Location = new Point(12, 132);
        lblAccountingDifferenceAccount.Name = "lblAccountingDifferenceAccount";
        lblAccountingDifferenceAccount.Size = new Size(146, 15);
        lblAccountingDifferenceAccount.TabIndex = 24;
        lblAccountingDifferenceAccount.Text = "Cuenta diferencia centavos:";
        // 
        // lblAccountingRetentionPayableAccount
        // 
        lblAccountingRetentionPayableAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingRetentionPayableAccount.Appearance.Options.UseFont = true;
        lblAccountingRetentionPayableAccount.Location = new Point(12, 244);
        lblAccountingRetentionPayableAccount.Name = "lblAccountingRetentionPayableAccount";
        lblAccountingRetentionPayableAccount.Size = new Size(121, 15);
        lblAccountingRetentionPayableAccount.TabIndex = 32;
        lblAccountingRetentionPayableAccount.Text = "Retenciones por pagar:";
        // 
        // lblAccountingRoundingAccount
        // 
        lblAccountingRoundingAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingRoundingAccount.Appearance.Options.UseFont = true;
        lblAccountingRoundingAccount.Location = new Point(12, 160);
        lblAccountingRoundingAccount.Name = "lblAccountingRoundingAccount";
        lblAccountingRoundingAccount.Size = new Size(95, 15);
        lblAccountingRoundingAccount.TabIndex = 26;
        lblAccountingRoundingAccount.Text = "Cuenta redondeo:";
        // 
        // lblAccountingClearingAccount
        // 
        lblAccountingClearingAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingClearingAccount.Appearance.Options.UseFont = true;
        lblAccountingClearingAccount.Location = new Point(12, 181);
        lblAccountingClearingAccount.Name = "lblAccountingClearingAccount";
        lblAccountingClearingAccount.Size = new Size(47, 15);
        lblAccountingClearingAccount.TabIndex = 28;
        lblAccountingClearingAccount.Text = "Clearing:";
        // 
        // lblAccountingDiscountAccount
        // 
        lblAccountingDiscountAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingDiscountAccount.Appearance.Options.UseFont = true;
        lblAccountingDiscountAccount.Location = new Point(12, 216);
        lblAccountingDiscountAccount.Name = "lblAccountingDiscountAccount";
        lblAccountingDiscountAccount.Size = new Size(104, 15);
        lblAccountingDiscountAccount.TabIndex = 30;
        lblAccountingDiscountAccount.Text = "Cuenta descuentos:";
        // 
        // xtpRetentions
        // 
        xtpRetentions.Controls.Add(btnAddressClear3);
        xtpRetentions.Controls.Add(btnAddressClear4);
        xtpRetentions.Controls.Add(btnAddressClear5);
        xtpRetentions.Controls.Add(btnAddressClear6);
        xtpRetentions.Controls.Add(lblRetentionRulesTitle);
        xtpRetentions.Controls.Add(grdRetentionRules);
        xtpRetentions.Controls.Add(lblRetentionTaxConfigTitle);
        xtpRetentions.Controls.Add(lblRetentionAccountingRequired);
        xtpRetentions.Controls.Add(lblRetentionEntryTitle);
        xtpRetentions.Controls.Add(lueRetentionAccountingRequired);
        xtpRetentions.Controls.Add(lblRetentionAgentConfig);
        xtpRetentions.Controls.Add(lblRetentionEntryType);
        xtpRetentions.Controls.Add(lueRetentionAgentConfig);
        xtpRetentions.Controls.Add(lueRetentionEntryType);
        xtpRetentions.Controls.Add(lblRetentionFiscalRegime);
        xtpRetentions.Controls.Add(lueRetentionFiscalRegime);
        xtpRetentions.Controls.Add(lblRetentionEntrySriCode);
        xtpRetentions.Controls.Add(lblRetentionSpecialTaxpayer);
        xtpRetentions.Controls.Add(lueRetentionEntrySriCode);
        xtpRetentions.Controls.Add(lueRetentionSpecialTaxpayer);
        xtpRetentions.Controls.Add(lblRetentionEntryPercent);
        xtpRetentions.Controls.Add(lblRetentionTaxpayerType);
        xtpRetentions.Controls.Add(lueRetentionEntryCurrent);
        xtpRetentions.Controls.Add(lueRetentionTaxpayerType);
        xtpRetentions.Controls.Add(spnRetentionEntryPercent);
        xtpRetentions.Controls.Add(lblRetentionFiscalCountry);
        xtpRetentions.Controls.Add(lblRetentionEntryCurrent);
        xtpRetentions.Controls.Add(lueRetentionFiscalCountry);
        xtpRetentions.Controls.Add(lblRetentionEntryAccount);
        xtpRetentions.Controls.Add(lueRetentionEntryAppliesIncome);
        xtpRetentions.Controls.Add(lueRetentionEntryAccount);
        xtpRetentions.Controls.Add(lblRetentionEntryAppliesIncome);
        xtpRetentions.Controls.Add(lblRetentionEntrySupport);
        xtpRetentions.Controls.Add(lueRetentionEntryAppliesIva);
        xtpRetentions.Controls.Add(lueRetentionEntrySupport);
        xtpRetentions.Controls.Add(lblRetentionEntryAppliesIva);
        xtpRetentions.Name = "xtpRetentions";
        xtpRetentions.Size = new Size(1094, 402);
        xtpRetentions.Text = "Retenciones";
        // 
        // btnAddressClear3
        // 
        btnAddressClear3.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressClear3.Appearance.Options.UseFont = true;
        btnAddressClear3.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressClear3.ImageOptions.SvgImage");
        btnAddressClear3.Location = new Point(384, 210);
        btnAddressClear3.Name = "btnAddressClear3";
        btnAddressClear3.Size = new Size(118, 28);
        btnAddressClear3.TabIndex = 51;
        btnAddressClear3.Text = "Limpiar";
        // 
        // btnAddressClear4
        // 
        btnAddressClear4.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressClear4.Appearance.Options.UseFont = true;
        btnAddressClear4.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressClear4.ImageOptions.SvgImage");
        btnAddressClear4.Location = new Point(260, 210);
        btnAddressClear4.Name = "btnAddressClear4";
        btnAddressClear4.Size = new Size(118, 28);
        btnAddressClear4.TabIndex = 50;
        btnAddressClear4.Text = "Quitar";
        // 
        // btnAddressClear5
        // 
        btnAddressClear5.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressClear5.Appearance.Options.UseFont = true;
        btnAddressClear5.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressClear5.ImageOptions.SvgImage");
        btnAddressClear5.Location = new Point(136, 210);
        btnAddressClear5.Name = "btnAddressClear5";
        btnAddressClear5.Size = new Size(118, 28);
        btnAddressClear5.TabIndex = 49;
        btnAddressClear5.Text = "Actualizar";
        // 
        // btnAddressClear6
        // 
        btnAddressClear6.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressClear6.Appearance.Options.UseFont = true;
        btnAddressClear6.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressClear6.ImageOptions.SvgImage");
        btnAddressClear6.Location = new Point(12, 210);
        btnAddressClear6.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAddressClear6.Name = "btnAddressClear6";
        btnAddressClear6.Size = new Size(118, 28);
        btnAddressClear6.TabIndex = 48;
        btnAddressClear6.Text = "Agregar";
        // 
        // lblRetentionRulesTitle
        // 
        lblRetentionRulesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblRetentionRulesTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblRetentionRulesTitle.Appearance.Options.UseFont = true;
        lblRetentionRulesTitle.Appearance.Options.UseForeColor = true;
        lblRetentionRulesTitle.Location = new Point(12, 258);
        lblRetentionRulesTitle.Name = "lblRetentionRulesTitle";
        lblRetentionRulesTitle.Size = new Size(169, 20);
        lblRetentionRulesTitle.TabIndex = 0;
        lblRetentionRulesTitle.Text = "3. Retenciones aplicables";
        // 
        // grdRetentionRules
        // 
        grdRetentionRules.Font = new Font("Segoe UI", 9F);
        grdRetentionRules.Location = new Point(12, 287);
        grdRetentionRules.MainView = grvRetentionRules;
        grdRetentionRules.Name = "grdRetentionRules";
        grdRetentionRules.Size = new Size(1062, 103);
        grdRetentionRules.TabIndex = 1;
        grdRetentionRules.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvRetentionRules });
        // 
        // grvRetentionRules
        // 
        grvRetentionRules.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvRetentionRules.Appearance.HeaderPanel.Options.UseFont = true;
        grvRetentionRules.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvRetentionRules.Appearance.Row.Options.UseFont = true;
        grvRetentionRules.Columns.AddRange(new GridColumn[] { colRetentionCode, colRetentionConcept, colRetentionType, colRetentionPercent, colRetentionValidFrom, colRetentionActive });
        grvRetentionRules.GridControl = grdRetentionRules;
        grvRetentionRules.Name = "grvRetentionRules";
        grvRetentionRules.OptionsBehavior.Editable = false;
        grvRetentionRules.OptionsView.ShowGroupPanel = false;
        // 
        // colRetentionCode
        // 
        colRetentionCode.Caption = "Codigo";
        colRetentionCode.FieldName = "Code";
        colRetentionCode.Name = "colRetentionCode";
        colRetentionCode.Visible = true;
        colRetentionCode.VisibleIndex = 0;
        colRetentionCode.Width = 65;
        // 
        // colRetentionConcept
        // 
        colRetentionConcept.Caption = "Concepto";
        colRetentionConcept.FieldName = "Concept";
        colRetentionConcept.Name = "colRetentionConcept";
        colRetentionConcept.Visible = true;
        colRetentionConcept.VisibleIndex = 1;
        colRetentionConcept.Width = 145;
        // 
        // colRetentionType
        // 
        colRetentionType.Caption = "Tipo";
        colRetentionType.FieldName = "Type";
        colRetentionType.Name = "colRetentionType";
        colRetentionType.Visible = true;
        colRetentionType.VisibleIndex = 2;
        colRetentionType.Width = 70;
        // 
        // colRetentionPercent
        // 
        colRetentionPercent.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
        colRetentionPercent.Caption = "Porcentaje";
        colRetentionPercent.DisplayFormat.FormatString = "n2";
        colRetentionPercent.DisplayFormat.FormatType = FormatType.Numeric;
        colRetentionPercent.FieldName = "Percent";
        colRetentionPercent.Name = "colRetentionPercent";
        colRetentionPercent.Visible = true;
        colRetentionPercent.VisibleIndex = 3;
        // 
        // colRetentionValidFrom
        // 
        colRetentionValidFrom.Caption = "Vigente desde";
        colRetentionValidFrom.FieldName = "ValidFrom";
        colRetentionValidFrom.Name = "colRetentionValidFrom";
        colRetentionValidFrom.Visible = true;
        colRetentionValidFrom.VisibleIndex = 4;
        colRetentionValidFrom.Width = 85;
        // 
        // colRetentionActive
        // 
        colRetentionActive.Caption = "Activa";
        colRetentionActive.FieldName = "IsActive";
        colRetentionActive.Name = "colRetentionActive";
        colRetentionActive.Visible = true;
        colRetentionActive.VisibleIndex = 5;
        colRetentionActive.Width = 55;
        // 
        // lblRetentionTaxConfigTitle
        // 
        lblRetentionTaxConfigTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblRetentionTaxConfigTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblRetentionTaxConfigTitle.Appearance.Options.UseFont = true;
        lblRetentionTaxConfigTitle.Appearance.Options.UseForeColor = true;
        lblRetentionTaxConfigTitle.Location = new Point(560, 12);
        lblRetentionTaxConfigTitle.Name = "lblRetentionTaxConfigTitle";
        lblRetentionTaxConfigTitle.Size = new Size(181, 20);
        lblRetentionTaxConfigTitle.TabIndex = 0;
        lblRetentionTaxConfigTitle.Text = "2. Configuracion tributaria";
        // 
        // lblRetentionAccountingRequired
        // 
        lblRetentionAccountingRequired.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionAccountingRequired.Appearance.Options.UseFont = true;
        lblRetentionAccountingRequired.Location = new Point(562, 45);
        lblRetentionAccountingRequired.Name = "lblRetentionAccountingRequired";
        lblRetentionAccountingRequired.Size = new Size(152, 15);
        lblRetentionAccountingRequired.TabIndex = 1;
        lblRetentionAccountingRequired.Text = "Obligado llevar contabilidad:";
        // 
        // lblRetentionEntryTitle
        // 
        lblRetentionEntryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblRetentionEntryTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblRetentionEntryTitle.Appearance.Options.UseFont = true;
        lblRetentionEntryTitle.Appearance.Options.UseForeColor = true;
        lblRetentionEntryTitle.Location = new Point(12, 12);
        lblRetentionEntryTitle.Name = "lblRetentionEntryTitle";
        lblRetentionEntryTitle.Size = new Size(143, 20);
        lblRetentionEntryTitle.TabIndex = 0;
        lblRetentionEntryTitle.Text = "1. Datos de retencion";
        // 
        // lueRetentionAccountingRequired
        // 
        lueRetentionAccountingRequired.Location = new Point(738, 42);
        lueRetentionAccountingRequired.Name = "lueRetentionAccountingRequired";
        lueRetentionAccountingRequired.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionAccountingRequired.Properties.Appearance.Options.UseFont = true;
        lueRetentionAccountingRequired.Properties.AutoHeight = false;
        lueRetentionAccountingRequired.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionAccountingRequired.Properties.NullText = "";
        lueRetentionAccountingRequired.Size = new Size(150, 22);
        lueRetentionAccountingRequired.TabIndex = 2;
        // 
        // lblRetentionAgentConfig
        // 
        lblRetentionAgentConfig.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionAgentConfig.Appearance.Options.UseFont = true;
        lblRetentionAgentConfig.Location = new Point(562, 73);
        lblRetentionAgentConfig.Name = "lblRetentionAgentConfig";
        lblRetentionAgentConfig.Size = new Size(94, 15);
        lblRetentionAgentConfig.TabIndex = 3;
        lblRetentionAgentConfig.Text = "Agente retencion:";
        // 
        // lblRetentionEntryType
        // 
        lblRetentionEntryType.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionEntryType.Appearance.Options.UseFont = true;
        lblRetentionEntryType.Location = new Point(14, 45);
        lblRetentionEntryType.Name = "lblRetentionEntryType";
        lblRetentionEntryType.Size = new Size(96, 15);
        lblRetentionEntryType.TabIndex = 13;
        lblRetentionEntryType.Text = "Tipo de retencion:";
        // 
        // lueRetentionAgentConfig
        // 
        lueRetentionAgentConfig.Location = new Point(738, 70);
        lueRetentionAgentConfig.Name = "lueRetentionAgentConfig";
        lueRetentionAgentConfig.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionAgentConfig.Properties.Appearance.Options.UseFont = true;
        lueRetentionAgentConfig.Properties.AutoHeight = false;
        lueRetentionAgentConfig.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionAgentConfig.Properties.NullText = "";
        lueRetentionAgentConfig.Size = new Size(150, 22);
        lueRetentionAgentConfig.TabIndex = 4;
        // 
        // lueRetentionEntryType
        // 
        lueRetentionEntryType.Location = new Point(121, 42);
        lueRetentionEntryType.Name = "lueRetentionEntryType";
        lueRetentionEntryType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionEntryType.Properties.Appearance.Options.UseFont = true;
        lueRetentionEntryType.Properties.AutoHeight = false;
        lueRetentionEntryType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionEntryType.Properties.NullText = "";
        lueRetentionEntryType.Size = new Size(153, 22);
        lueRetentionEntryType.TabIndex = 14;
        // 
        // lblRetentionFiscalRegime
        // 
        lblRetentionFiscalRegime.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionFiscalRegime.Appearance.Options.UseFont = true;
        lblRetentionFiscalRegime.Location = new Point(562, 101);
        lblRetentionFiscalRegime.Name = "lblRetentionFiscalRegime";
        lblRetentionFiscalRegime.Size = new Size(102, 15);
        lblRetentionFiscalRegime.TabIndex = 5;
        lblRetentionFiscalRegime.Text = "Regimen tributario:";
        // 
        // lueRetentionFiscalRegime
        // 
        lueRetentionFiscalRegime.Location = new Point(738, 98);
        lueRetentionFiscalRegime.Name = "lueRetentionFiscalRegime";
        lueRetentionFiscalRegime.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionFiscalRegime.Properties.Appearance.Options.UseFont = true;
        lueRetentionFiscalRegime.Properties.AutoHeight = false;
        lueRetentionFiscalRegime.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionFiscalRegime.Properties.NullText = "";
        lueRetentionFiscalRegime.Size = new Size(150, 22);
        lueRetentionFiscalRegime.TabIndex = 6;
        // 
        // lblRetentionEntrySriCode
        // 
        lblRetentionEntrySriCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionEntrySriCode.Appearance.Options.UseFont = true;
        lblRetentionEntrySriCode.Location = new Point(14, 73);
        lblRetentionEntrySriCode.Name = "lblRetentionEntrySriCode";
        lblRetentionEntrySriCode.Size = new Size(61, 15);
        lblRetentionEntrySriCode.TabIndex = 15;
        lblRetentionEntrySriCode.Text = "Codigo SRI:";
        // 
        // lblRetentionSpecialTaxpayer
        // 
        lblRetentionSpecialTaxpayer.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionSpecialTaxpayer.Appearance.Options.UseFont = true;
        lblRetentionSpecialTaxpayer.Location = new Point(562, 129);
        lblRetentionSpecialTaxpayer.Name = "lblRetentionSpecialTaxpayer";
        lblRetentionSpecialTaxpayer.Size = new Size(124, 15);
        lblRetentionSpecialTaxpayer.TabIndex = 7;
        lblRetentionSpecialTaxpayer.Text = "Contribuyente especial:";
        // 
        // lueRetentionEntrySriCode
        // 
        lueRetentionEntrySriCode.Location = new Point(121, 70);
        lueRetentionEntrySriCode.Name = "lueRetentionEntrySriCode";
        lueRetentionEntrySriCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionEntrySriCode.Properties.Appearance.Options.UseFont = true;
        lueRetentionEntrySriCode.Properties.AutoHeight = false;
        lueRetentionEntrySriCode.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionEntrySriCode.Properties.NullText = "";
        lueRetentionEntrySriCode.Size = new Size(153, 22);
        lueRetentionEntrySriCode.TabIndex = 16;
        // 
        // lueRetentionSpecialTaxpayer
        // 
        lueRetentionSpecialTaxpayer.Location = new Point(738, 126);
        lueRetentionSpecialTaxpayer.Name = "lueRetentionSpecialTaxpayer";
        lueRetentionSpecialTaxpayer.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionSpecialTaxpayer.Properties.Appearance.Options.UseFont = true;
        lueRetentionSpecialTaxpayer.Properties.AutoHeight = false;
        lueRetentionSpecialTaxpayer.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionSpecialTaxpayer.Properties.NullText = "";
        lueRetentionSpecialTaxpayer.Size = new Size(150, 22);
        lueRetentionSpecialTaxpayer.TabIndex = 8;
        // 
        // lblRetentionEntryPercent
        // 
        lblRetentionEntryPercent.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionEntryPercent.Appearance.Options.UseFont = true;
        lblRetentionEntryPercent.Location = new Point(282, 73);
        lblRetentionEntryPercent.Name = "lblRetentionEntryPercent";
        lblRetentionEntryPercent.Size = new Size(80, 15);
        lblRetentionEntryPercent.TabIndex = 17;
        lblRetentionEntryPercent.Text = "Porcentaje (%):";
        // 
        // lblRetentionTaxpayerType
        // 
        lblRetentionTaxpayerType.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionTaxpayerType.Appearance.Options.UseFont = true;
        lblRetentionTaxpayerType.Location = new Point(562, 157);
        lblRetentionTaxpayerType.Name = "lblRetentionTaxpayerType";
        lblRetentionTaxpayerType.Size = new Size(104, 15);
        lblRetentionTaxpayerType.TabIndex = 9;
        lblRetentionTaxpayerType.Text = "Tipo contribuyente:";
        // 
        // lueRetentionEntryCurrent
        // 
        lueRetentionEntryCurrent.Location = new Point(121, 182);
        lueRetentionEntryCurrent.Name = "lueRetentionEntryCurrent";
        lueRetentionEntryCurrent.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionEntryCurrent.Properties.Appearance.Options.UseFont = true;
        lueRetentionEntryCurrent.Properties.AutoHeight = false;
        lueRetentionEntryCurrent.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionEntryCurrent.Properties.NullText = "";
        lueRetentionEntryCurrent.Size = new Size(153, 22);
        lueRetentionEntryCurrent.TabIndex = 28;
        // 
        // lueRetentionTaxpayerType
        // 
        lueRetentionTaxpayerType.Location = new Point(738, 154);
        lueRetentionTaxpayerType.Name = "lueRetentionTaxpayerType";
        lueRetentionTaxpayerType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionTaxpayerType.Properties.Appearance.Options.UseFont = true;
        lueRetentionTaxpayerType.Properties.AutoHeight = false;
        lueRetentionTaxpayerType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionTaxpayerType.Properties.NullText = "";
        lueRetentionTaxpayerType.Size = new Size(150, 22);
        lueRetentionTaxpayerType.TabIndex = 10;
        // 
        // spnRetentionEntryPercent
        // 
        spnRetentionEntryPercent.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnRetentionEntryPercent.Location = new Point(368, 70);
        spnRetentionEntryPercent.Name = "spnRetentionEntryPercent";
        spnRetentionEntryPercent.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnRetentionEntryPercent.Properties.Appearance.Options.UseFont = true;
        spnRetentionEntryPercent.Properties.Appearance.Options.UseTextOptions = true;
        spnRetentionEntryPercent.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
        spnRetentionEntryPercent.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnRetentionEntryPercent.Properties.DisplayFormat.FormatString = "n2";
        spnRetentionEntryPercent.Properties.DisplayFormat.FormatType = FormatType.Numeric;
        spnRetentionEntryPercent.Properties.EditFormat.FormatString = "n2";
        spnRetentionEntryPercent.Properties.EditFormat.FormatType = FormatType.Numeric;
        spnRetentionEntryPercent.Properties.MaskSettings.Set("mask", "n2");
        spnRetentionEntryPercent.Size = new Size(134, 22);
        spnRetentionEntryPercent.TabIndex = 18;
        // 
        // lblRetentionFiscalCountry
        // 
        lblRetentionFiscalCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionFiscalCountry.Appearance.Options.UseFont = true;
        lblRetentionFiscalCountry.Location = new Point(562, 185);
        lblRetentionFiscalCountry.Name = "lblRetentionFiscalCountry";
        lblRetentionFiscalCountry.Size = new Size(54, 15);
        lblRetentionFiscalCountry.TabIndex = 11;
        lblRetentionFiscalCountry.Text = "Pais fiscal:";
        // 
        // lblRetentionEntryCurrent
        // 
        lblRetentionEntryCurrent.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionEntryCurrent.Appearance.Options.UseFont = true;
        lblRetentionEntryCurrent.Location = new Point(14, 185);
        lblRetentionEntryCurrent.Name = "lblRetentionEntryCurrent";
        lblRetentionEntryCurrent.Size = new Size(43, 15);
        lblRetentionEntryCurrent.TabIndex = 27;
        lblRetentionEntryCurrent.Text = "Vigente:";
        // 
        // lueRetentionFiscalCountry
        // 
        lueRetentionFiscalCountry.Location = new Point(738, 182);
        lueRetentionFiscalCountry.Name = "lueRetentionFiscalCountry";
        lueRetentionFiscalCountry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionFiscalCountry.Properties.Appearance.Options.UseFont = true;
        lueRetentionFiscalCountry.Properties.AutoHeight = false;
        lueRetentionFiscalCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionFiscalCountry.Properties.NullText = "";
        lueRetentionFiscalCountry.Properties.PopupView = grvRetentionFiscalCountryLookup;
        lueRetentionFiscalCountry.Size = new Size(150, 22);
        lueRetentionFiscalCountry.TabIndex = 12;
        // 
        // grvRetentionFiscalCountryLookup
        // 
        grvRetentionFiscalCountryLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvRetentionFiscalCountryLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvRetentionFiscalCountryLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvRetentionFiscalCountryLookup.Appearance.Row.Options.UseFont = true;
        grvRetentionFiscalCountryLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvRetentionFiscalCountryLookup.Name = "grvRetentionFiscalCountryLookup";
        grvRetentionFiscalCountryLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvRetentionFiscalCountryLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblRetentionEntryAccount
        // 
        lblRetentionEntryAccount.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionEntryAccount.Appearance.Options.UseFont = true;
        lblRetentionEntryAccount.Location = new Point(14, 101);
        lblRetentionEntryAccount.Name = "lblRetentionEntryAccount";
        lblRetentionEntryAccount.Size = new Size(90, 15);
        lblRetentionEntryAccount.TabIndex = 19;
        lblRetentionEntryAccount.Text = "Cuenta contable:";
        // 
        // lueRetentionEntryAppliesIncome
        // 
        lueRetentionEntryAppliesIncome.Location = new Point(368, 154);
        lueRetentionEntryAppliesIncome.Name = "lueRetentionEntryAppliesIncome";
        lueRetentionEntryAppliesIncome.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionEntryAppliesIncome.Properties.Appearance.Options.UseFont = true;
        lueRetentionEntryAppliesIncome.Properties.AutoHeight = false;
        lueRetentionEntryAppliesIncome.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionEntryAppliesIncome.Properties.NullText = "";
        lueRetentionEntryAppliesIncome.Size = new Size(134, 22);
        lueRetentionEntryAppliesIncome.TabIndex = 26;
        // 
        // lueRetentionEntryAccount
        // 
        lueRetentionEntryAccount.Location = new Point(121, 98);
        lueRetentionEntryAccount.Name = "lueRetentionEntryAccount";
        lueRetentionEntryAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionEntryAccount.Properties.Appearance.Options.UseFont = true;
        lueRetentionEntryAccount.Properties.AutoHeight = false;
        lueRetentionEntryAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionEntryAccount.Properties.NullText = "";
        lueRetentionEntryAccount.Properties.PopupView = grvRetentionEntryAccountLookup;
        lueRetentionEntryAccount.Size = new Size(381, 22);
        lueRetentionEntryAccount.TabIndex = 20;
        // 
        // grvRetentionEntryAccountLookup
        // 
        grvRetentionEntryAccountLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvRetentionEntryAccountLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvRetentionEntryAccountLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvRetentionEntryAccountLookup.Appearance.Row.Options.UseFont = true;
        grvRetentionEntryAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvRetentionEntryAccountLookup.Name = "grvRetentionEntryAccountLookup";
        grvRetentionEntryAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvRetentionEntryAccountLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblRetentionEntryAppliesIncome
        // 
        lblRetentionEntryAppliesIncome.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionEntryAppliesIncome.Appearance.Options.UseFont = true;
        lblRetentionEntryAppliesIncome.Location = new Point(293, 157);
        lblRetentionEntryAppliesIncome.Name = "lblRetentionEntryAppliesIncome";
        lblRetentionEntryAppliesIncome.Size = new Size(69, 15);
        lblRetentionEntryAppliesIncome.TabIndex = 25;
        lblRetentionEntryAppliesIncome.Text = "Aplica Renta:";
        // 
        // lblRetentionEntrySupport
        // 
        lblRetentionEntrySupport.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionEntrySupport.Appearance.Options.UseFont = true;
        lblRetentionEntrySupport.Location = new Point(14, 129);
        lblRetentionEntrySupport.Name = "lblRetentionEntrySupport";
        lblRetentionEntrySupport.Size = new Size(101, 15);
        lblRetentionEntrySupport.TabIndex = 21;
        lblRetentionEntrySupport.Text = "Sustento tributario:";
        // 
        // lueRetentionEntryAppliesIva
        // 
        lueRetentionEntryAppliesIva.Location = new Point(121, 154);
        lueRetentionEntryAppliesIva.Name = "lueRetentionEntryAppliesIva";
        lueRetentionEntryAppliesIva.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionEntryAppliesIva.Properties.Appearance.Options.UseFont = true;
        lueRetentionEntryAppliesIva.Properties.AutoHeight = false;
        lueRetentionEntryAppliesIva.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionEntryAppliesIva.Properties.NullText = "";
        lueRetentionEntryAppliesIva.Size = new Size(153, 22);
        lueRetentionEntryAppliesIva.TabIndex = 24;
        // 
        // lueRetentionEntrySupport
        // 
        lueRetentionEntrySupport.Location = new Point(121, 126);
        lueRetentionEntrySupport.Name = "lueRetentionEntrySupport";
        lueRetentionEntrySupport.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRetentionEntrySupport.Properties.Appearance.Options.UseFont = true;
        lueRetentionEntrySupport.Properties.AutoHeight = false;
        lueRetentionEntrySupport.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRetentionEntrySupport.Properties.NullText = "";
        lueRetentionEntrySupport.Size = new Size(381, 22);
        lueRetentionEntrySupport.TabIndex = 22;
        // 
        // lblRetentionEntryAppliesIva
        // 
        lblRetentionEntryAppliesIva.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionEntryAppliesIva.Appearance.Options.UseFont = true;
        lblRetentionEntryAppliesIva.Location = new Point(14, 157);
        lblRetentionEntryAppliesIva.Name = "lblRetentionEntryAppliesIva";
        lblRetentionEntryAppliesIva.Size = new Size(57, 15);
        lblRetentionEntryAppliesIva.TabIndex = 23;
        lblRetentionEntryAppliesIva.Text = "Aplica IVA:";
        // 
        // xtpSap
        // 
        xtpSap.Controls.Add(btnAddressRemove1);
        xtpSap.Controls.Add(btnAddressRemove2);
        xtpSap.Controls.Add(btnAddressRemove3);
        xtpSap.Controls.Add(btnAddressRemove4);
        xtpSap.Controls.Add(grdSapFieldMapping);
        xtpSap.Controls.Add(lblSapFieldMappingTitle);
        xtpSap.Controls.Add(lblSapMapEnabled);
        xtpSap.Controls.Add(lblSapMapRequired);
        xtpSap.Controls.Add(lueSapMapEnabled);
        xtpSap.Controls.Add(lblSapMapDescription);
        xtpSap.Controls.Add(lueSapMapRequired);
        xtpSap.Controls.Add(lblSapMapSapField);
        xtpSap.Controls.Add(txtSapMapDescription);
        xtpSap.Controls.Add(lblSapMapSystemField);
        xtpSap.Controls.Add(txtSapMapSapField);
        xtpSap.Controls.Add(lblSapHistoryTitle);
        xtpSap.Controls.Add(txtSapMapSystemField);
        xtpSap.Controls.Add(grdSapSyncHistory);
        xtpSap.Controls.Add(lblSapSyncAsSupplier);
        xtpSap.Controls.Add(lblSapMode);
        xtpSap.Controls.Add(lueSapSyncAsSupplier);
        xtpSap.Controls.Add(lblSapConfigTitle);
        xtpSap.Controls.Add(lblSapManualRetry);
        xtpSap.Controls.Add(lueSapMode);
        xtpSap.Controls.Add(lueSapManualRetry);
        xtpSap.Controls.Add(lblSapCompany);
        xtpSap.Controls.Add(lblSapRequiresApproval);
        xtpSap.Controls.Add(lblSapStatusTitle);
        xtpSap.Controls.Add(lueSapRequiresApproval);
        xtpSap.Controls.Add(lueSapCompany);
        xtpSap.Controls.Add(lblSapSyncStatus);
        xtpSap.Controls.Add(lueSapSyncStatus);
        xtpSap.Controls.Add(lblSapLastSync);
        xtpSap.Controls.Add(txtSapLastSync);
        xtpSap.Controls.Add(lblSapLastError);
        xtpSap.Controls.Add(txtSapLastError);
        xtpSap.Controls.Add(lblSapRetryCount);
        xtpSap.Controls.Add(txtSapRetryCount);
        xtpSap.Controls.Add(lblSapEnabled);
        xtpSap.Controls.Add(lueSapEnabled);
        xtpSap.Name = "xtpSap";
        xtpSap.Size = new Size(1094, 402);
        xtpSap.Text = "SAP";
        // 
        // btnAddressRemove1
        // 
        btnAddressRemove1.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressRemove1.Appearance.Options.UseFont = true;
        btnAddressRemove1.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressRemove1.ImageOptions.SvgImage");
        btnAddressRemove1.Location = new Point(384, 364);
        btnAddressRemove1.Name = "btnAddressRemove1";
        btnAddressRemove1.Size = new Size(118, 28);
        btnAddressRemove1.TabIndex = 55;
        btnAddressRemove1.Text = "Limpiar";
        // 
        // btnAddressRemove2
        // 
        btnAddressRemove2.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressRemove2.Appearance.Options.UseFont = true;
        btnAddressRemove2.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressRemove2.ImageOptions.SvgImage");
        btnAddressRemove2.Location = new Point(260, 364);
        btnAddressRemove2.Name = "btnAddressRemove2";
        btnAddressRemove2.Size = new Size(118, 28);
        btnAddressRemove2.TabIndex = 54;
        btnAddressRemove2.Text = "Quitar";
        // 
        // btnAddressRemove3
        // 
        btnAddressRemove3.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressRemove3.Appearance.Options.UseFont = true;
        btnAddressRemove3.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressRemove3.ImageOptions.SvgImage");
        btnAddressRemove3.Location = new Point(136, 364);
        btnAddressRemove3.Name = "btnAddressRemove3";
        btnAddressRemove3.Size = new Size(118, 28);
        btnAddressRemove3.TabIndex = 53;
        btnAddressRemove3.Text = "Actualizar";
        // 
        // btnAddressRemove4
        // 
        btnAddressRemove4.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressRemove4.Appearance.Options.UseFont = true;
        btnAddressRemove4.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressRemove4.ImageOptions.SvgImage");
        btnAddressRemove4.Location = new Point(12, 364);
        btnAddressRemove4.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAddressRemove4.Name = "btnAddressRemove4";
        btnAddressRemove4.Size = new Size(118, 28);
        btnAddressRemove4.TabIndex = 52;
        btnAddressRemove4.Text = "Agregar";
        // 
        // grdSapFieldMapping
        // 
        grdSapFieldMapping.Location = new Point(508, 225);
        grdSapFieldMapping.MainView = grvSapFieldMapping;
        grdSapFieldMapping.Name = "grdSapFieldMapping";
        grdSapFieldMapping.Size = new Size(565, 167);
        grdSapFieldMapping.TabIndex = 1;
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
        lblSapFieldMappingTitle.TabIndex = 0;
        lblSapFieldMappingTitle.Text = "4. Campos sincronizados";
        // 
        // lblSapMapEnabled
        // 
        lblSapMapEnabled.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapEnabled.Appearance.Options.UseFont = true;
        lblSapMapEnabled.Location = new Point(12, 339);
        lblSapMapEnabled.Name = "lblSapMapEnabled";
        lblSapMapEnabled.Size = new Size(37, 15);
        lblSapMapEnabled.TabIndex = 10;
        lblSapMapEnabled.Text = "Activo:";
        // 
        // lblSapMapRequired
        // 
        lblSapMapRequired.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapRequired.Appearance.Options.UseFont = true;
        lblSapMapRequired.Location = new Point(12, 311);
        lblSapMapRequired.Name = "lblSapMapRequired";
        lblSapMapRequired.Size = new Size(63, 15);
        lblSapMapRequired.TabIndex = 8;
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
        lueSapMapEnabled.TabIndex = 11;
        // 
        // lblSapMapDescription
        // 
        lblSapMapDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapDescription.Appearance.Options.UseFont = true;
        lblSapMapDescription.Location = new Point(12, 283);
        lblSapMapDescription.Name = "lblSapMapDescription";
        lblSapMapDescription.Size = new Size(65, 15);
        lblSapMapDescription.TabIndex = 6;
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
        lueSapMapRequired.TabIndex = 9;
        // 
        // lblSapMapSapField
        // 
        lblSapMapSapField.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapSapField.Appearance.Options.UseFont = true;
        lblSapMapSapField.Location = new Point(12, 255);
        lblSapMapSapField.Name = "lblSapMapSapField";
        lblSapMapSapField.Size = new Size(24, 15);
        lblSapMapSapField.TabIndex = 4;
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
        txtSapMapDescription.TabIndex = 7;
        // 
        // lblSapMapSystemField
        // 
        lblSapMapSystemField.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMapSystemField.Appearance.Options.UseFont = true;
        lblSapMapSystemField.Location = new Point(12, 227);
        lblSapMapSystemField.Name = "lblSapMapSystemField";
        lblSapMapSystemField.Size = new Size(44, 15);
        lblSapMapSystemField.TabIndex = 2;
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
        txtSapMapSapField.TabIndex = 5;
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
        lblSapHistoryTitle.TabIndex = 0;
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
        txtSapMapSystemField.TabIndex = 3;
        // 
        // grdSapSyncHistory
        // 
        grdSapSyncHistory.Location = new Point(620, 38);
        grdSapSyncHistory.MainView = grvSapSyncHistory;
        grdSapSyncHistory.Name = "grdSapSyncHistory";
        grdSapSyncHistory.Size = new Size(453, 142);
        grdSapSyncHistory.TabIndex = 1;
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
        lblSapSyncAsSupplier.TabIndex = 7;
        lblSapSyncAsSupplier.Text = "Sincronizar proveedor:";
        // 
        // lblSapMode
        // 
        lblSapMode.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapMode.Appearance.Options.UseFont = true;
        lblSapMode.Location = new Point(319, 45);
        lblSapMode.Name = "lblSapMode";
        lblSapMode.Size = new Size(59, 15);
        lblSapMode.TabIndex = 1;
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
        lueSapSyncAsSupplier.TabIndex = 8;
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
        lblSapConfigTitle.TabIndex = 0;
        lblSapConfigTitle.Text = "2. Configuracion de integracion";
        // 
        // lblSapManualRetry
        // 
        lblSapManualRetry.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapManualRetry.Appearance.Options.UseFont = true;
        lblSapManualRetry.Location = new Point(319, 129);
        lblSapManualRetry.Name = "lblSapManualRetry";
        lblSapManualRetry.Size = new Size(97, 15);
        lblSapManualRetry.TabIndex = 9;
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
        lueSapMode.TabIndex = 2;
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
        lueSapManualRetry.TabIndex = 10;
        // 
        // lblSapCompany
        // 
        lblSapCompany.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapCompany.Appearance.Options.UseFont = true;
        lblSapCompany.Location = new Point(319, 73);
        lblSapCompany.Name = "lblSapCompany";
        lblSapCompany.Size = new Size(72, 15);
        lblSapCompany.TabIndex = 3;
        lblSapCompany.Text = "Empresa SAP:";
        // 
        // lblSapRequiresApproval
        // 
        lblSapRequiresApproval.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapRequiresApproval.Appearance.Options.UseFont = true;
        lblSapRequiresApproval.Location = new Point(319, 157);
        lblSapRequiresApproval.Name = "lblSapRequiresApproval";
        lblSapRequiresApproval.Size = new Size(112, 15);
        lblSapRequiresApproval.TabIndex = 11;
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
        lblSapStatusTitle.TabIndex = 15;
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
        lueSapRequiresApproval.TabIndex = 12;
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
        lueSapCompany.TabIndex = 4;
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
        lblSapSyncStatus.TabIndex = 16;
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
        lueSapSyncStatus.TabIndex = 17;
        // 
        // lblSapLastSync
        // 
        lblSapLastSync.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastSync.Appearance.Options.UseFont = true;
        lblSapLastSync.Location = new Point(12, 101);
        lblSapLastSync.Name = "lblSapLastSync";
        lblSapLastSync.Size = new Size(117, 15);
        lblSapLastSync.TabIndex = 22;
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
        txtSapLastSync.TabIndex = 23;
        // 
        // lblSapLastError
        // 
        lblSapLastError.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastError.Appearance.Options.UseFont = true;
        lblSapLastError.Location = new Point(12, 129);
        lblSapLastError.Name = "lblSapLastError";
        lblSapLastError.Size = new Size(67, 15);
        lblSapLastError.TabIndex = 24;
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
        txtSapLastError.TabIndex = 25;
        // 
        // lblSapRetryCount
        // 
        lblSapRetryCount.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapRetryCount.Appearance.Options.UseFont = true;
        lblSapRetryCount.Location = new Point(12, 73);
        lblSapRetryCount.Name = "lblSapRetryCount";
        lblSapRetryCount.Size = new Size(59, 15);
        lblSapRetryCount.TabIndex = 26;
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
        txtSapRetryCount.TabIndex = 27;
        // 
        // lblSapEnabled
        // 
        lblSapEnabled.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapEnabled.Appearance.Options.UseFont = true;
        lblSapEnabled.Location = new Point(12, 157);
        lblSapEnabled.Name = "lblSapEnabled";
        lblSapEnabled.Size = new Size(118, 15);
        lblSapEnabled.TabIndex = 28;
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
        lueSapEnabled.TabIndex = 29;
        // 
        // xtpNotes
        // 
        xtpNotes.Controls.Add(btnAddressClear8);
        xtpNotes.Controls.Add(btnAddressClear9);
        xtpNotes.Controls.Add(btnAddressRemove0);
        xtpNotes.Controls.Add(lblAttachmentsTitle);
        xtpNotes.Controls.Add(grdSupplierAttachments);
        xtpNotes.Controls.Add(lblNotesGeneralTitle);
        xtpNotes.Controls.Add(lblSupplierInternalNotes);
        xtpNotes.Controls.Add(memSupplierInternalNotes);
        xtpNotes.Controls.Add(lblSupplierPurchasingNotes);
        xtpNotes.Controls.Add(memSupplierPurchasingNotes);
        xtpNotes.Controls.Add(lblSupplierPaymentNotes);
        xtpNotes.Controls.Add(memSupplierPaymentNotes);
        xtpNotes.Controls.Add(lblSupplierOperationalAlert);
        xtpNotes.Controls.Add(txtSupplierOperationalAlert);
        xtpNotes.Name = "xtpNotes";
        xtpNotes.Size = new Size(1094, 402);
        xtpNotes.Text = "Obs. / Anexos";
        // 
        // btnAddressClear8
        // 
        btnAddressClear8.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressClear8.Appearance.Options.UseFont = true;
        btnAddressClear8.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressClear8.ImageOptions.SvgImage");
        btnAddressClear8.Location = new Point(260, 365);
        btnAddressClear8.Name = "btnAddressClear8";
        btnAddressClear8.Size = new Size(118, 28);
        btnAddressClear8.TabIndex = 46;
        btnAddressClear8.Text = "Quitar";
        // 
        // btnAddressClear9
        // 
        btnAddressClear9.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressClear9.Appearance.Options.UseFont = true;
        btnAddressClear9.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressClear9.ImageOptions.SvgImage");
        btnAddressClear9.Location = new Point(136, 365);
        btnAddressClear9.Name = "btnAddressClear9";
        btnAddressClear9.Size = new Size(118, 28);
        btnAddressClear9.TabIndex = 45;
        btnAddressClear9.Text = "Actualizar";
        // 
        // btnAddressRemove0
        // 
        btnAddressRemove0.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddressRemove0.Appearance.Options.UseFont = true;
        btnAddressRemove0.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("btnAddressRemove0.ImageOptions.SvgImage");
        btnAddressRemove0.Location = new Point(12, 365);
        btnAddressRemove0.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnAddressRemove0.Name = "btnAddressRemove0";
        btnAddressRemove0.Size = new Size(118, 28);
        btnAddressRemove0.TabIndex = 44;
        btnAddressRemove0.Text = "Agregar";
        // 
        // lblAttachmentsTitle
        // 
        lblAttachmentsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAttachmentsTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblAttachmentsTitle.Appearance.Options.UseFont = true;
        lblAttachmentsTitle.Appearance.Options.UseForeColor = true;
        lblAttachmentsTitle.Location = new Point(12, 207);
        lblAttachmentsTitle.Name = "lblAttachmentsTitle";
        lblAttachmentsTitle.Size = new Size(66, 20);
        lblAttachmentsTitle.TabIndex = 0;
        lblAttachmentsTitle.Text = "2. Anexos";
        // 
        // grdSupplierAttachments
        // 
        grdSupplierAttachments.Location = new Point(12, 235);
        grdSupplierAttachments.MainView = grvSupplierAttachments;
        grdSupplierAttachments.Name = "grdSupplierAttachments";
        grdSupplierAttachments.Size = new Size(1065, 124);
        grdSupplierAttachments.TabIndex = 1;
        grdSupplierAttachments.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvSupplierAttachments });
        // 
        // grvSupplierAttachments
        // 
        grvSupplierAttachments.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvSupplierAttachments.Appearance.HeaderPanel.Options.UseFont = true;
        grvSupplierAttachments.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvSupplierAttachments.Appearance.Row.Options.UseFont = true;
        grvSupplierAttachments.Columns.AddRange(new GridColumn[] { colAttachmentType, colAttachmentFileName, colAttachmentDescription, colAttachmentDate, colAttachmentUser, colAttachmentStatus });
        grvSupplierAttachments.GridControl = grdSupplierAttachments;
        grvSupplierAttachments.Name = "grvSupplierAttachments";
        grvSupplierAttachments.OptionsBehavior.Editable = false;
        grvSupplierAttachments.OptionsView.ShowGroupPanel = false;
        // 
        // colAttachmentType
        // 
        colAttachmentType.Caption = "Tipo";
        colAttachmentType.FieldName = "Type";
        colAttachmentType.Name = "colAttachmentType";
        colAttachmentType.Visible = true;
        colAttachmentType.VisibleIndex = 0;
        colAttachmentType.Width = 90;
        // 
        // colAttachmentFileName
        // 
        colAttachmentFileName.Caption = "Nombre archivo";
        colAttachmentFileName.FieldName = "FileName";
        colAttachmentFileName.Name = "colAttachmentFileName";
        colAttachmentFileName.Visible = true;
        colAttachmentFileName.VisibleIndex = 1;
        colAttachmentFileName.Width = 150;
        // 
        // colAttachmentDescription
        // 
        colAttachmentDescription.Caption = "Descripcion";
        colAttachmentDescription.FieldName = "Description";
        colAttachmentDescription.Name = "colAttachmentDescription";
        colAttachmentDescription.Visible = true;
        colAttachmentDescription.VisibleIndex = 2;
        colAttachmentDescription.Width = 170;
        // 
        // colAttachmentDate
        // 
        colAttachmentDate.Caption = "Fecha";
        colAttachmentDate.FieldName = "Date";
        colAttachmentDate.Name = "colAttachmentDate";
        colAttachmentDate.Visible = true;
        colAttachmentDate.VisibleIndex = 3;
        colAttachmentDate.Width = 85;
        // 
        // colAttachmentUser
        // 
        colAttachmentUser.Caption = "Usuario";
        colAttachmentUser.FieldName = "User";
        colAttachmentUser.Name = "colAttachmentUser";
        colAttachmentUser.Visible = true;
        colAttachmentUser.VisibleIndex = 4;
        colAttachmentUser.Width = 70;
        // 
        // colAttachmentStatus
        // 
        colAttachmentStatus.Caption = "Estado";
        colAttachmentStatus.FieldName = "Status";
        colAttachmentStatus.Name = "colAttachmentStatus";
        colAttachmentStatus.Visible = true;
        colAttachmentStatus.VisibleIndex = 5;
        // 
        // lblNotesGeneralTitle
        // 
        lblNotesGeneralTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblNotesGeneralTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblNotesGeneralTitle.Appearance.Options.UseFont = true;
        lblNotesGeneralTitle.Appearance.Options.UseForeColor = true;
        lblNotesGeneralTitle.Location = new Point(12, 12);
        lblNotesGeneralTitle.Name = "lblNotesGeneralTitle";
        lblNotesGeneralTitle.Size = new Size(183, 20);
        lblNotesGeneralTitle.TabIndex = 0;
        lblNotesGeneralTitle.Text = "1. Observaciones generales";
        // 
        // lblSupplierInternalNotes
        // 
        lblSupplierInternalNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierInternalNotes.Appearance.Options.UseFont = true;
        lblSupplierInternalNotes.Location = new Point(12, 43);
        lblSupplierInternalNotes.Name = "lblSupplierInternalNotes";
        lblSupplierInternalNotes.Size = new Size(42, 15);
        lblSupplierInternalNotes.TabIndex = 2;
        lblSupplierInternalNotes.Text = "Internas:";
        // 
        // memSupplierInternalNotes
        // 
        memSupplierInternalNotes.Location = new Point(12, 62);
        memSupplierInternalNotes.Name = "memSupplierInternalNotes";
        memSupplierInternalNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memSupplierInternalNotes.Properties.Appearance.Options.UseFont = true;
        memSupplierInternalNotes.Size = new Size(340, 95);
        memSupplierInternalNotes.TabIndex = 3;
        // 
        // lblSupplierPurchasingNotes
        // 
        lblSupplierPurchasingNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierPurchasingNotes.Appearance.Options.UseFont = true;
        lblSupplierPurchasingNotes.Location = new Point(374, 43);
        lblSupplierPurchasingNotes.Name = "lblSupplierPurchasingNotes";
        lblSupplierPurchasingNotes.Size = new Size(118, 15);
        lblSupplierPurchasingNotes.TabIndex = 4;
        lblSupplierPurchasingNotes.Text = "Observaciones compra:";
        // 
        // memSupplierPurchasingNotes
        // 
        memSupplierPurchasingNotes.Location = new Point(374, 62);
        memSupplierPurchasingNotes.Name = "memSupplierPurchasingNotes";
        memSupplierPurchasingNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memSupplierPurchasingNotes.Properties.Appearance.Options.UseFont = true;
        memSupplierPurchasingNotes.Size = new Size(340, 95);
        memSupplierPurchasingNotes.TabIndex = 5;
        // 
        // lblSupplierPaymentNotes
        // 
        lblSupplierPaymentNotes.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierPaymentNotes.Appearance.Options.UseFont = true;
        lblSupplierPaymentNotes.Location = new Point(736, 43);
        lblSupplierPaymentNotes.Name = "lblSupplierPaymentNotes";
        lblSupplierPaymentNotes.Size = new Size(101, 15);
        lblSupplierPaymentNotes.TabIndex = 6;
        lblSupplierPaymentNotes.Text = "Observaciones pago:";
        // 
        // memSupplierPaymentNotes
        // 
        memSupplierPaymentNotes.Location = new Point(736, 62);
        memSupplierPaymentNotes.Name = "memSupplierPaymentNotes";
        memSupplierPaymentNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memSupplierPaymentNotes.Properties.Appearance.Options.UseFont = true;
        memSupplierPaymentNotes.Size = new Size(340, 95);
        memSupplierPaymentNotes.TabIndex = 7;
        // 
        // lblSupplierOperationalAlert
        // 
        lblSupplierOperationalAlert.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierOperationalAlert.Appearance.Options.UseFont = true;
        lblSupplierOperationalAlert.Location = new Point(12, 173);
        lblSupplierOperationalAlert.Name = "lblSupplierOperationalAlert";
        lblSupplierOperationalAlert.Size = new Size(89, 15);
        lblSupplierOperationalAlert.TabIndex = 8;
        lblSupplierOperationalAlert.Text = "Alerta operativa:";
        // 
        // txtSupplierOperationalAlert
        // 
        txtSupplierOperationalAlert.Location = new Point(118, 170);
        txtSupplierOperationalAlert.Name = "txtSupplierOperationalAlert";
        txtSupplierOperationalAlert.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierOperationalAlert.Properties.Appearance.Options.UseFont = true;
        txtSupplierOperationalAlert.Properties.AutoHeight = false;
        txtSupplierOperationalAlert.Size = new Size(958, 22);
        txtSupplierOperationalAlert.TabIndex = 9;
        // 
        // lblCode
        // 
        lblCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCode.Appearance.Options.UseFont = true;
        lblCode.Location = new Point(15, 11);
        lblCode.Name = "lblCode";
        lblCode.Size = new Size(42, 15);
        lblCode.TabIndex = 2;
        lblCode.Text = "Codigo:";
        // 
        // lblSapHeaderStatus
        // 
        lblSapHeaderStatus.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblSapHeaderStatus.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblSapHeaderStatus.Appearance.Options.UseFont = true;
        lblSapHeaderStatus.Appearance.Options.UseForeColor = true;
        lblSapHeaderStatus.Location = new Point(906, 69);
        lblSapHeaderStatus.Name = "lblSapHeaderStatus";
        lblSapHeaderStatus.Size = new Size(89, 13);
        lblSapHeaderStatus.TabIndex = 1;
        lblSapHeaderStatus.Text = "Sincronizado SAP";
        // 
        // lblSapHeaderStatusValue
        // 
        lblSapHeaderStatusValue.Appearance.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
        lblSapHeaderStatusValue.Appearance.ForeColor = Color.FromArgb(22, 163, 74);
        lblSapHeaderStatusValue.Appearance.Options.UseFont = true;
        lblSapHeaderStatusValue.Appearance.Options.UseForeColor = true;
        lblSapHeaderStatusValue.Location = new Point(1011, 69);
        lblSapHeaderStatusValue.Name = "lblSapHeaderStatusValue";
        lblSapHeaderStatusValue.Size = new Size(61, 13);
        lblSapHeaderStatusValue.TabIndex = 2;
        lblSapHeaderStatusValue.Text = "Confirmado";
        // 
        // lblPayableHeader
        // 
        lblPayableHeader.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPayableHeader.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        lblPayableHeader.Appearance.Options.UseFont = true;
        lblPayableHeader.Appearance.Options.UseForeColor = true;
        lblPayableHeader.Location = new Point(906, 41);
        lblPayableHeader.Name = "lblPayableHeader";
        lblPayableHeader.Size = new Size(86, 13);
        lblPayableHeader.TabIndex = 5;
        lblPayableHeader.Text = "Saldo por pagar:";
        // 
        // lblPayableHeaderValue
        // 
        lblPayableHeaderValue.Appearance.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
        lblPayableHeaderValue.Appearance.Options.UseFont = true;
        lblPayableHeaderValue.Location = new Point(1011, 40);
        lblPayableHeaderValue.Name = "lblPayableHeaderValue";
        lblPayableHeaderValue.Size = new Size(46, 13);
        lblPayableHeaderValue.TabIndex = 6;
        lblPayableHeaderValue.Text = "12,475.60";
        // 
        // txtSupplierCode
        // 
        txtSupplierCode.Location = new Point(123, 8);
        txtSupplierCode.Name = "txtSupplierCode";
        txtSupplierCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierCode.Properties.Appearance.Options.UseFont = true;
        txtSupplierCode.Size = new Size(159, 22);
        txtSupplierCode.TabIndex = 0;
        // 
        // lblSupplierName
        // 
        lblSupplierName.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierName.Appearance.Options.UseFont = true;
        lblSupplierName.Location = new Point(15, 39);
        lblSupplierName.Name = "lblSupplierName";
        lblSupplierName.Size = new Size(68, 15);
        lblSupplierName.TabIndex = 4;
        lblSupplierName.Text = "Razon social:";
        // 
        // lblSupplierType
        // 
        lblSupplierType.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierType.Appearance.Options.UseFont = true;
        lblSupplierType.Location = new Point(15, 123);
        lblSupplierType.Name = "lblSupplierType";
        lblSupplierType.Size = new Size(84, 15);
        lblSupplierType.TabIndex = 0;
        lblSupplierType.Text = "Tipo proveedor:";
        // 
        // lblIdentificationType
        // 
        lblIdentificationType.Appearance.Font = new Font("Segoe UI", 9F);
        lblIdentificationType.Appearance.Options.UseFont = true;
        lblIdentificationType.Location = new Point(15, 95);
        lblIdentificationType.Name = "lblIdentificationType";
        lblIdentificationType.Size = new Size(41, 15);
        lblIdentificationType.TabIndex = 2;
        lblIdentificationType.Text = "Tipo ID:";
        // 
        // lueIdentificationType
        // 
        lueIdentificationType.Location = new Point(123, 92);
        lueIdentificationType.Name = "lueIdentificationType";
        lueIdentificationType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueIdentificationType.Properties.Appearance.Options.UseFont = true;
        lueIdentificationType.Properties.AutoHeight = false;
        lueIdentificationType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueIdentificationType.Properties.NullText = "";
        lueIdentificationType.Properties.PopupView = grvIdentificationTypeLookup;
        lueIdentificationType.Size = new Size(159, 22);
        lueIdentificationType.TabIndex = 3;
        // 
        // grvIdentificationTypeLookup
        // 
        grvIdentificationTypeLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        grvIdentificationTypeLookup.Appearance.HeaderPanel.Options.UseFont = true;
        grvIdentificationTypeLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvIdentificationTypeLookup.Appearance.Row.Options.UseFont = true;
        grvIdentificationTypeLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        grvIdentificationTypeLookup.Name = "grvIdentificationTypeLookup";
        grvIdentificationTypeLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        grvIdentificationTypeLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblIdentificationNumber
        // 
        lblIdentificationNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblIdentificationNumber.Appearance.Options.UseFont = true;
        lblIdentificationNumber.Location = new Point(296, 95);
        lblIdentificationNumber.Name = "lblIdentificationNumber";
        lblIdentificationNumber.Size = new Size(74, 15);
        lblIdentificationNumber.TabIndex = 4;
        lblIdentificationNumber.Text = "RUC / Cedula:";
        // 
        // txtIdentificationNumber
        // 
        txtIdentificationNumber.Location = new Point(376, 92);
        txtIdentificationNumber.Name = "txtIdentificationNumber";
        txtIdentificationNumber.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtIdentificationNumber.Properties.Appearance.Options.UseFont = true;
        txtIdentificationNumber.Size = new Size(141, 22);
        txtIdentificationNumber.TabIndex = 4;
        // 
        // txtSupplierName
        // 
        txtSupplierName.Location = new Point(123, 36);
        txtSupplierName.Name = "txtSupplierName";
        txtSupplierName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierName.Properties.Appearance.Options.UseFont = true;
        txtSupplierName.Size = new Size(394, 22);
        txtSupplierName.TabIndex = 1;
        // 
        // lblSupplierCommercialName
        // 
        lblSupplierCommercialName.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierCommercialName.Appearance.Options.UseFont = true;
        lblSupplierCommercialName.Location = new Point(15, 67);
        lblSupplierCommercialName.Name = "lblSupplierCommercialName";
        lblSupplierCommercialName.Size = new Size(102, 15);
        lblSupplierCommercialName.TabIndex = 5;
        lblSupplierCommercialName.Text = "Nombre comercial:";
        // 
        // txtSupplierCommercialName
        // 
        txtSupplierCommercialName.Location = new Point(123, 64);
        txtSupplierCommercialName.Name = "txtSupplierCommercialName";
        txtSupplierCommercialName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierCommercialName.Properties.Appearance.Options.UseFont = true;
        txtSupplierCommercialName.Size = new Size(394, 22);
        txtSupplierCommercialName.TabIndex = 2;
        // 
        // lueSupplierType
        // 
        lueSupplierType.Location = new Point(123, 120);
        lueSupplierType.Name = "lueSupplierType";
        lueSupplierType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierType.Properties.Appearance.Options.UseFont = true;
        lueSupplierType.Properties.AutoHeight = false;
        lueSupplierType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierType.Properties.NullText = "";
        lueSupplierType.Size = new Size(159, 22);
        lueSupplierType.TabIndex = 13;
        // 
        // lblStatus
        // 
        lblStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblStatus.Appearance.ForeColor = Color.Black;
        lblStatus.Appearance.Options.UseFont = true;
        lblStatus.Appearance.Options.UseForeColor = true;
        lblStatus.Location = new Point(15, 152);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(35, 15);
        lblStatus.TabIndex = 33;
        lblStatus.Text = "Estado";
        // 
        // btnStatusToggle
        // 
        btnStatusToggle.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnStatusToggle.Appearance.Font = new Font("Segoe UI", 9F);
        btnStatusToggle.Appearance.ForeColor = Color.White;
        btnStatusToggle.Appearance.Options.UseBackColor = true;
        btnStatusToggle.Appearance.Options.UseFont = true;
        btnStatusToggle.Appearance.Options.UseForeColor = true;
        btnStatusToggle.Location = new Point(123, 148);
        btnStatusToggle.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnStatusToggle.LookAndFeel.UseDefaultLookAndFeel = false;
        btnStatusToggle.Name = "btnStatusToggle";
        btnStatusToggle.Size = new Size(72, 22);
        btnStatusToggle.TabIndex = 34;
        btnStatusToggle.Text = "Activo";
        // 
        // btnSave
        // 
        btnSave.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.AppearanceHovered.BackColor = Color.FromArgb(0, 161, 132);
        btnSave.AppearanceHovered.ForeColor = Color.White;
        btnSave.AppearanceHovered.Options.UseBackColor = true;
        btnSave.AppearanceHovered.Options.UseForeColor = true;
        btnSave.Location = new Point(901, 611);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 40;
        btnSave.Text = "Guardar";
        // 
        // btnCancel
        // 
        btnCancel.Appearance.BackColor = Color.White;
        btnCancel.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancel.Appearance.ForeColor = Color.FromArgb(100, 112, 132);
        btnCancel.Appearance.Options.UseBackColor = true;
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.Appearance.Options.UseForeColor = true;
        btnCancel.AppearanceHovered.BackColor = Color.FromArgb(247, 248, 252);
        btnCancel.AppearanceHovered.ForeColor = Color.FromArgb(23, 32, 51);
        btnCancel.AppearanceHovered.Options.UseBackColor = true;
        btnCancel.AppearanceHovered.Options.UseForeColor = true;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(1011, 611);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 41;
        btnCancel.Text = "Cancelar";
        // 
        // SupplierEditForm
        // 
        AcceptButton = btnSave;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(1130, 654);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(lblSapHeaderStatusValue);
        Controls.Add(lblSapHeaderStatus);
        Controls.Add(lblPayableHeader);
        Controls.Add(lblPayableHeaderValue);
        Controls.Add(lblStatus);
        Controls.Add(btnStatusToggle);
        Controls.Add(lueSupplierType);
        Controls.Add(lblSupplierType);
        Controls.Add(lblCode);
        Controls.Add(lblIdentificationNumber);
        Controls.Add(lblIdentificationType);
        Controls.Add(txtIdentificationNumber);
        Controls.Add(lueIdentificationType);
        Controls.Add(tabSupplier);
        Controls.Add(txtSupplierCode);
        Controls.Add(lblSupplierName);
        Controls.Add(txtSupplierCommercialName);
        Controls.Add(lblSupplierCommercialName);
        Controls.Add(txtSupplierName);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "SupplierEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Proveedor";
        ((System.ComponentModel.ISupportInitialize)tabSupplier).EndInit();
        tabSupplier.ResumeLayout(false);
        xtpGeneral.ResumeLayout(false);
        xtpGeneral.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummaryBalance).EndInit();
        pnlSummaryBalance.ResumeLayout(false);
        pnlSummaryBalance.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummaryOrders).EndInit();
        pnlSummaryOrders.ResumeLayout(false);
        pnlSummaryOrders.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummaryLastPurchase).EndInit();
        pnlSummaryLastPurchase.ResumeLayout(false);
        pnlSummaryLastPurchase.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummaryPurchases12m).EndInit();
        pnlSummaryPurchases12m.ResumeLayout(false);
        pnlSummaryPurchases12m.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummarySap).EndInit();
        pnlSummarySap.ResumeLayout(false);
        pnlSummarySap.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSummaryRetentions).EndInit();
        pnlSummaryRetentions.ResumeLayout(false);
        pnlSummaryRetentions.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueBuyer.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvBuyerLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueChannel.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplyMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnDeliveryDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memReturnPolicy.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierGroup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierGroupLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierClass.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierClassLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueEconomicActivity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvEconomicActivityLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueZone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvZoneLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvCountryLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueProvince.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvProvinceLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvCityLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePriceList.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvPriceListLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditDays.Properties).EndInit();
        xtpContacts.ResumeLayout(false);
        xtpContacts.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdSupplierContacts).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierContacts).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContactName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactDepartment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactPosition.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactChannel.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContactPhone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactLanguage.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContactExtension.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactNotifications.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContactMobile.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memSupplierContactNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContactEmail.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactPrincipal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierContactStatus.Properties).EndInit();
        xtpAddresses.ResumeLayout(false);
        xtpAddresses.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdSupplierAddresses).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierAddresses).EndInit();
        ((System.ComponentModel.ISupportInitialize)picAddressMap.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSupplierLatitude.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnSupplierLongitude.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierAddressLine1.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierAddressReference.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierAddressLine2.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierAddressCountryLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressProvince.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierAddressProvinceLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressCity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierAddressCityLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierAddressPostal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressPrimary.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierAddressStatus.Properties).EndInit();
        xtpPurchases.ResumeLayout(false);
        xtpPurchases.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdPurchaseProducts).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvPurchaseProducts).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdPurchaseDocuments).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvPurchaseDocuments).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsAllowSales.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchasePaymentTerm.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvPurchasePaymentTermLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditLimit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvPurchaseCurrencyLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseBuyer.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvPurchaseBuyerLookup).EndInit();
        xtpBanks.ResumeLayout(false);
        xtpBanks.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdBankAccounts).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvBankAccounts).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBankSwift.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBankAba.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBankName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvBankNameLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBankIban.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBankAccountType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBankCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvBankCountryLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBankStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBankCity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvBankCityLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBankAccountNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memBankNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBankPrimary.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBankHolder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBankCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvBankCurrencyLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBankHolderIdentification.Properties).EndInit();
        xtpAccounting.ResumeLayout(false);
        xtpAccounting.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueAccountingBranch.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingBranchLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingDepartment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingDepartmentLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingBusinessLine.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingBusinessLineLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingCostCenter.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingCostCenterLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingProject.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingProjectLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingConciliationRequired.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnAccountingPaymentTolerance.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnAccountingAveragePaymentDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingUsesWithholdingBase.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingPaymentMethod.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingBlocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingPaymentPriority.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingAllowsPartialPayments.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingRequiredPaymentDay.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingPaymentDocumentType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingApprovalFlow.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingAllowsCompensation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingAllowsAdvance.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingRequiresProvision.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)chkAccountingBySupplier.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingRetentionPayableAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingRetentionPayableAccountLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingDiscountAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingDiscountAccountLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingClearingAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingClearingAccountLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingRoundingAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingRoundingAccountLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingDifferenceAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingDifferenceAccountLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingAdvanceAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingAdvanceAccountLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingDefaultExpenseAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingDefaultExpenseAccountLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingSupplierAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvAccountingSupplierAccountLookup).EndInit();
        xtpRetentions.ResumeLayout(false);
        xtpRetentions.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdRetentionRules).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvRetentionRules).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionAccountingRequired.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionAgentConfig.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntryType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionFiscalRegime.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntrySriCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionSpecialTaxpayer.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntryCurrent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionTaxpayerType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnRetentionEntryPercent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionFiscalCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvRetentionFiscalCountryLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntryAppliesIncome.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntryAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvRetentionEntryAccountLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntryAppliesIva.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRetentionEntrySupport.Properties).EndInit();
        xtpSap.ResumeLayout(false);
        xtpSap.PerformLayout();
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
        xtpNotes.ResumeLayout(false);
        xtpNotes.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdSupplierAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvSupplierAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)memSupplierInternalNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memSupplierPurchasingNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memSupplierPaymentNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierOperationalAlert.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueIdentificationType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvIdentificationTypeLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtIdentificationNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierCommercialName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierType.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
    private XtraTabControl tabSupplier;
    private XtraTabPage xtpGeneral;
    private XtraTabPage xtpContacts;
    private LabelControl lblContactListTitle;
    private GridControl grdSupplierContacts;
    private GridView grvSupplierContacts;
    private GridColumn colSupplierContactName;
    private GridColumn colSupplierContactPosition;
    private GridColumn colSupplierContactPhone;
    private GridColumn colSupplierContactExtension;
    private GridColumn colSupplierContactMobile;
    private GridColumn colSupplierContactEmail;
    private GridColumn colSupplierContactPrimary;
    private GridColumn colSupplierContactActive;
    private LabelControl lblContactClassificationTitle;
    private LabelControl lblSupplierContactType;
    private LookUpEdit lueSupplierContactType;
    private LabelControl lblSupplierContactDepartment;
    private LookUpEdit lueSupplierContactDepartment;
    private LabelControl lblSupplierContactChannel;
    private LookUpEdit lueSupplierContactChannel;
    private LabelControl lblSupplierContactLanguage;
    private LookUpEdit lueSupplierContactLanguage;
    private LabelControl lblSupplierContactNotifications;
    private LookUpEdit lueSupplierContactNotifications;
    private LabelControl lblSupplierContactNotes;
    private MemoEdit memSupplierContactNotes;
    private XtraTabPage xtpAddresses;
    private LabelControl lblAddressGeoTitle;
    private LabelControl lblSupplierLatitude;
    private SpinEdit spnSupplierLatitude;
    private LabelControl lblSupplierLongitude;
    private SpinEdit spnSupplierLongitude;
    private LabelControl lblSupplierAddressReference;
    private TextEdit txtSupplierAddressReference;
    private SimpleButton btnValidateCoordinates;
    private SimpleButton btnClearCoordinates;
    private LabelControl lblAddressMapTitle;
    private LabelControl lblAddressMapPlaceholder;
    private PictureEdit picAddressMap;
    private XtraTabPage xtpPurchases;
    private LabelControl lblPurchaseDocumentsTitle;
    private GridControl grdPurchaseDocuments;
    private GridView grvPurchaseDocuments;
    private GridColumn colPurchaseDocumentDate;
    private GridColumn colPurchaseDocumentType;
    private GridColumn colPurchaseDocumentNumber;
    private GridColumn colPurchaseDocumentStatus;
    private GridColumn colPurchaseDocumentTotal;
    private GridColumn colPurchaseDocumentCurrency;
    private GridColumn colPurchaseDocumentSap;
    private XtraTabPage xtpBanks;
    private LabelControl lblBankDataTitle;
    private LabelControl lblBankName;
    private SearchLookUpEdit lueBankName;
    private GridView grvBankNameLookup;
    private LabelControl lblBankAccountType;
    private LookUpEdit lueBankAccountType;
    private LabelControl lblBankAccountNumber;
    private TextEdit txtBankAccountNumber;
    private LabelControl lblBankHolder;
    private TextEdit txtBankHolder;
    private LabelControl lblBankHolderIdentification;
    private TextEdit txtBankHolderIdentification;
    private LabelControl lblBankCurrency;
    private SearchLookUpEdit lueBankCurrency;
    private GridView grvBankCurrencyLookup;
    private LabelControl lblBankPrimary;
    private LookUpEdit lueBankPrimary;
    private LabelControl lblBankStatus;
    private LookUpEdit lueBankStatus;
    private LabelControl lblBankTransferTitle;
    private LabelControl lblBankSwift;
    private TextEdit txtBankSwift;
    private LabelControl lblBankAba;
    private TextEdit txtBankAba;
    private LabelControl lblBankIban;
    private TextEdit txtBankIban;
    private LabelControl lblBankCountry;
    private SearchLookUpEdit lueBankCountry;
    private GridView grvBankCountryLookup;
    private LabelControl lblBankCity;
    private SearchLookUpEdit lueBankCity;
    private GridView grvBankCityLookup;
    private LabelControl lblBankNotes;
    private MemoEdit memBankNotes;
    private XtraTabPage xtpAccounting;
    private LabelControl lblAccountingPaymentMethod;
    private LookUpEdit lueAccountingPaymentMethod;
    private LabelControl lblAccountingPaymentPriority;
    private LookUpEdit lueAccountingPaymentPriority;
    private LabelControl lblAccountingRequiredPaymentDay;
    private LookUpEdit lueAccountingRequiredPaymentDay;
    private LabelControl lblAccountingPaymentDocumentType;
    private LookUpEdit lueAccountingPaymentDocumentType;
    private LabelControl lblAccountingApprovalFlow;
    private LookUpEdit lueAccountingApprovalFlow;
    private LabelControl lblAccountingAveragePaymentDays;
    private SpinEdit spnAccountingAveragePaymentDays;
    private LabelControl lblAccountingPaymentTolerance;
    private SpinEdit spnAccountingPaymentTolerance;
    private XtraTabPage xtpRetentions;
    private LabelControl lblRetentionTaxConfigTitle;
    private LabelControl lblRetentionAccountingRequired;
    private LookUpEdit lueRetentionAccountingRequired;
    private LabelControl lblRetentionAgentConfig;
    private LookUpEdit lueRetentionAgentConfig;
    private LabelControl lblRetentionFiscalRegime;
    private LookUpEdit lueRetentionFiscalRegime;
    private LabelControl lblRetentionSpecialTaxpayer;
    private LookUpEdit lueRetentionSpecialTaxpayer;
    private LabelControl lblRetentionTaxpayerType;
    private LookUpEdit lueRetentionTaxpayerType;
    private LabelControl lblRetentionFiscalCountry;
    private SearchLookUpEdit lueRetentionFiscalCountry;
    private GridView grvRetentionFiscalCountryLookup;
    private LabelControl lblRetentionRulesTitle;
    private GridControl grdRetentionRules;
    private GridView grvRetentionRules;
    private GridColumn colRetentionCode;
    private GridColumn colRetentionConcept;
    private GridColumn colRetentionType;
    private GridColumn colRetentionPercent;
    private GridColumn colRetentionValidFrom;
    private GridColumn colRetentionActive;
    private LabelControl lblRetentionEntryTitle;
    private LabelControl lblRetentionEntryType;
    private LookUpEdit lueRetentionEntryType;
    private LabelControl lblRetentionEntrySriCode;
    private LookUpEdit lueRetentionEntrySriCode;
    private LabelControl lblRetentionEntryPercent;
    private SpinEdit spnRetentionEntryPercent;
    private LabelControl lblRetentionEntryAccount;
    private SearchLookUpEdit lueRetentionEntryAccount;
    private GridView grvRetentionEntryAccountLookup;
    private LabelControl lblRetentionEntrySupport;
    private LookUpEdit lueRetentionEntrySupport;
    private LabelControl lblRetentionEntryAppliesIva;
    private LookUpEdit lueRetentionEntryAppliesIva;
    private LabelControl lblRetentionEntryAppliesIncome;
    private LookUpEdit lueRetentionEntryAppliesIncome;
    private LabelControl lblRetentionEntryCurrent;
    private LookUpEdit lueRetentionEntryCurrent;
    private XtraTabPage xtpSap;
    private LabelControl lblSapConfigTitle;
    private LabelControl lblSapMode;
    private LookUpEdit lueSapMode;
    private LabelControl lblSapCompany;
    private SearchLookUpEdit lueSapCompany;
    private GridView grvSapCompanyLookup;
    private LabelControl lblSapSyncAsSupplier;
    private LookUpEdit lueSapSyncAsSupplier;
    private LabelControl lblSapManualRetry;
    private LookUpEdit lueSapManualRetry;
    private LabelControl lblSapRequiresApproval;
    private LookUpEdit lueSapRequiresApproval;
    private LabelControl lblSapHistoryTitle;
    private GridControl grdSapSyncHistory;
    private GridView grvSapSyncHistory;
    private GridColumn colSapHistoryDate;
    private GridColumn colSapHistoryOperation;
    private GridColumn colSapHistoryStatus;
    private GridColumn colSapHistoryDocEntry;
    private GridColumn colSapHistoryDocNum;
    private GridColumn colSapHistoryRetryCount;
    private GridColumn colSapHistoryMessage;
    private LabelControl lblSapFieldMappingTitle;
    private GridControl grdSapFieldMapping;
    private GridView grvSapFieldMapping;
    private GridColumn colSapMapSystemField;
    private GridColumn colSapMapSapField;
    private GridColumn colSapMapDescription;
    private GridColumn colSapMapRequired;
    private GridColumn colSapMapEnabled;
    private LabelControl lblSapMapSystemField;
    private TextEdit txtSapMapSystemField;
    private LabelControl lblSapMapSapField;
    private TextEdit txtSapMapSapField;
    private LabelControl lblSapMapDescription;
    private TextEdit txtSapMapDescription;
    private LabelControl lblSapMapRequired;
    private LookUpEdit lueSapMapRequired;
    private LabelControl lblSapMapEnabled;
    private LookUpEdit lueSapMapEnabled;
    private XtraTabPage xtpNotes;
    private LabelControl lblNotesGeneralTitle;
    private LabelControl lblSupplierInternalNotes;
    private MemoEdit memSupplierInternalNotes;
    private LabelControl lblSupplierPurchasingNotes;
    private MemoEdit memSupplierPurchasingNotes;
    private LabelControl lblSupplierPaymentNotes;
    private MemoEdit memSupplierPaymentNotes;
    private LabelControl lblSupplierOperationalAlert;
    private TextEdit txtSupplierOperationalAlert;
    private LabelControl lblAttachmentsTitle;
    private GridControl grdSupplierAttachments;
    private GridView grvSupplierAttachments;
    private GridColumn colAttachmentType;
    private GridColumn colAttachmentFileName;
    private GridColumn colAttachmentDescription;
    private GridColumn colAttachmentDate;
    private GridColumn colAttachmentUser;
    private GridColumn colAttachmentStatus;
    private LabelControl lblCode;
    private LabelControl lblSapHeaderStatus;
    private LabelControl lblSapHeaderStatusValue;
    private LabelControl lblPayableHeader;
    private LabelControl lblPayableHeaderValue;
    private TextEdit txtSupplierCode;
    private LabelControl lblSupplierName;
    private LabelControl lblSupplierType;
    private LabelControl lblIdentificationType;
    private SearchLookUpEdit lueIdentificationType;
    private GridView grvIdentificationTypeLookup;
    private LabelControl lblIdentificationNumber;
    private TextEdit txtIdentificationNumber;
    private TextEdit txtSupplierName;
    private LabelControl lblSupplierCommercialName;
    private TextEdit txtSupplierCommercialName;
    private LookUpEdit lueSupplierType;
    private LabelControl lblStatus;
    private SimpleButton btnStatusToggle;
    private SimpleButton btnSave;
    private SimpleButton btnCancel;
    private LabelControl lblSummaryTitle;
    private PanelControl pnlSummaryBalance;
    private LabelControl lblPayableBalanceCaption;
    private LabelControl lblPayableBalanceValue;
    private PanelControl pnlSummaryOrders;
    private LabelControl lblOpenOrdersCaption;
    private LabelControl lblOpenOrdersValue;
    private PanelControl pnlSummaryLastPurchase;
    private LabelControl lblLastPurchaseCaption;
    private LabelControl lblLastPurchaseValue;
    private PanelControl pnlSummaryPurchases12m;
    private LabelControl lblPurchases12mCaption;
    private LabelControl lblPurchases12mValue;
    private PanelControl pnlSummarySap;
    private LabelControl lblSapStatusCaption;
    private LabelControl lblSapStatusValue;
    private PanelControl pnlSummaryRetentions;
    private LabelControl lblRetentionsCaption;
    private LabelControl lblRetentionsValue;
    private LabelControl lblCommercialTitle;
    private LabelControl lblBuyer;
    private SearchLookUpEdit lueBuyer;
    private GridView grvBuyerLookup;
    private LabelControl lblChannel;
    private LookUpEdit lueChannel;
    private LabelControl lblSupplyMethod;
    private LookUpEdit lueSupplyMethod;
    private LabelControl lblDeliveryDays;
    private SpinEdit spnDeliveryDays;
    private LabelControl lblMinimumOrder;
    private SpinEdit spnMinimumOrder;
    private LabelControl lblReturnPolicy;
    private MemoEdit memReturnPolicy;
    private LabelControl lblClassificationTitle;
    private LabelControl lblSupplierGroup;
    private SearchLookUpEdit lueSupplierGroup;
    private GridView grvSupplierGroupLookup;
    private LabelControl lblSupplierClass;
    private SearchLookUpEdit lueSupplierClass;
    private GridView grvSupplierClassLookup;
    private LabelControl lblEconomicActivity;
    private SearchLookUpEdit lueEconomicActivity;
    private GridView grvEconomicActivityLookup;
    private LabelControl lblZone;
    private SearchLookUpEdit lueZone;
    private GridView grvZoneLookup;
    private LabelControl lblCountry;
    private SearchLookUpEdit lueCountry;
    private GridView grvCountryLookup;
    private LabelControl lblProvince;
    private SearchLookUpEdit lueProvince;
    private GridView grvProvinceLookup;
    private LabelControl lblCity;
    private SearchLookUpEdit lueCity;
    private GridView grvCityLookup;
    private LabelControl lblPriceList;
    private SearchLookUpEdit luePriceList;
    private GridView grvPriceListLookup;
    private LabelControl lblCreditDays;
    private SpinEdit spnCreditDays;
    private LabelControl lblContactDataTitle;
    private LabelControl lblSupplierContactName;
    private TextEdit txtSupplierContactName;
    private LabelControl lblSupplierContactPosition;
    private LookUpEdit lueSupplierContactPosition;
    private LabelControl lblSupplierContactPhone;
    private TextEdit txtSupplierContactPhone;
    private LabelControl lblSupplierContactExtension;
    private TextEdit txtSupplierContactExtension;
    private LabelControl lblSupplierContactMobile;
    private TextEdit txtSupplierContactMobile;
    private LabelControl lblSupplierContactEmail;
    private TextEdit txtSupplierContactEmail;
    private LabelControl lblSupplierContactPrincipal;
    private LookUpEdit lueSupplierContactPrincipal;
    private LabelControl lblSupplierContactStatus;
    private LookUpEdit lueSupplierContactStatus;
    private LabelControl lblAddressListTitle;
    private GridControl grdSupplierAddresses;
    private GridView grvSupplierAddresses;
    private GridColumn colSupplierAddressType;
    private GridColumn colSupplierAddressLine;
    private GridColumn colSupplierAddressCountry;
    private GridColumn colSupplierAddressProvince;
    private GridColumn colSupplierAddressCity;
    private GridColumn colSupplierAddressPostal;
    private GridColumn colSupplierAddressPrimary;
    private GridColumn colSupplierAddressActive;
    private LabelControl lblAddressDataTitle;
    private LabelControl lblSupplierAddressType;
    private LookUpEdit lueSupplierAddressType;
    private LabelControl lblSupplierAddressLine1;
    private TextEdit txtSupplierAddressLine1;
    private LabelControl lblSupplierAddressLine2;
    private TextEdit txtSupplierAddressLine2;
    private LabelControl lblSupplierAddressCountry;
    private SearchLookUpEdit lueSupplierAddressCountry;
    private GridView grvSupplierAddressCountryLookup;
    private LabelControl lblSupplierAddressProvince;
    private SearchLookUpEdit lueSupplierAddressProvince;
    private GridView grvSupplierAddressProvinceLookup;
    private LabelControl lblSupplierAddressCity;
    private SearchLookUpEdit lueSupplierAddressCity;
    private GridView grvSupplierAddressCityLookup;
    private LabelControl lblSupplierAddressPostal;
    private TextEdit txtSupplierAddressPostal;
    private LabelControl lblSupplierAddressPrimary;
    private LookUpEdit lueSupplierAddressPrimary;
    private LabelControl lblSupplierAddressStatus;
    private LookUpEdit lueSupplierAddressStatus;
    private SimpleButton btnAddressClear;
    private SimpleButton btnAddressRemove;
    private SimpleButton btnAddressUpdate;
    private SimpleButton btnAddressAdd;
    private SimpleButton btnContactClear;
    private SimpleButton btnContactRemove;
    private SimpleButton btnContactUpdate;
    private SimpleButton btnContactAdd;
    private LabelControl lblPurchaseStatsTitle;
    private LabelControl lblPurchaseLastDateCaption;
    private LabelControl lblPurchaseLastDateValue;
    private LabelControl lblPurchase12mCaption;
    private LabelControl lblPurchase12mValue;
    private LabelControl lblPurchaseOpenOrdersCaption;
    private LabelControl lblPurchaseOpenOrdersValue;
    private LabelControl lblPurchasePayableCaption;
    private LabelControl lblPurchasePayableValue;
    private LabelControl lblPurchaseAvgDeliveryCaption;
    private LabelControl lblPurchaseAvgDeliveryValue;
    private LabelControl lblPurchaseComplianceCaption;
    private LabelControl lblPurchaseComplianceValue;
    private LabelControl lblAllowBackorder;
    private ToggleSwitch tsAllowSales;
    private LabelControl lblPurchaseConditionsTitle;
    private LabelControl lblPurchasePaymentTerm;
    private SearchLookUpEdit luePurchasePaymentTerm;
    private GridView grvPurchasePaymentTermLookup;
    private LabelControl lblPurchaseCurrency;
    private LabelControl lblCreditLimit;
    private SpinEdit spnCreditLimit;
    private SearchLookUpEdit luePurchaseCurrency;
    private GridView grvPurchaseCurrencyLookup;
    private LabelControl lblPurchaseBuyer;
    private SearchLookUpEdit luePurchaseBuyer;
    private GridView grvPurchaseBuyerLookup;
    private LabelControl lblPurchaseProductsTitle;
    private GridControl grdPurchaseProducts;
    private GridView grvPurchaseProducts;
    private GridColumn colPurchaseProductCode;
    private GridColumn colPurchaseProductName;
    private GridColumn colPurchaseProductUnit;
    private GridColumn colPurchaseProductLastPrice;
    private GridColumn colPurchaseProductCurrency;
    private GridColumn colPurchaseProductLastDate;
    private SimpleButton btnBankClear;
    private SimpleButton btnAddressClear0;
    private SimpleButton btnAddressClear1;
    private SimpleButton btnAddressClear2;
    private LabelControl lblBankAccountsTitle;
    private GridControl grdBankAccounts;
    private GridView grvBankAccounts;
    private GridColumn colBankName;
    private GridColumn colBankAccountType;
    private GridColumn colBankAccountNumber;
    private GridColumn colBankHolder;
    private GridColumn colBankIdentification;
    private GridColumn colBankCurrency;
    private GridColumn colBankPrimary;
    private GridColumn colBankActive;
    private SimpleButton btnAddressClear3;
    private SimpleButton btnAddressClear4;
    private SimpleButton btnAddressClear5;
    private SimpleButton btnAddressClear6;
    private SimpleButton btnAddressClear8;
    private SimpleButton btnAddressClear9;
    private SimpleButton btnAddressRemove0;
    private LabelControl lblSapStatusTitle;
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
    private SimpleButton btnAddressRemove1;
    private SimpleButton btnAddressRemove2;
    private SimpleButton btnAddressRemove3;
    private SimpleButton btnAddressRemove4;
    private LabelControl lblAccountingDimensionsTitle;
    private LabelControl lblAccountingBranch;
    private SearchLookUpEdit lueAccountingBranch;
    private GridView grvAccountingBranchLookup;
    private LabelControl lblAccountingDepartment;
    private SearchLookUpEdit lueAccountingDepartment;
    private GridView grvAccountingDepartmentLookup;
    private LabelControl lblAccountingBusinessLine;
    private SearchLookUpEdit lueAccountingBusinessLine;
    private GridView grvAccountingBusinessLineLookup;
    private LabelControl lblAccountingCostCenter;
    private SearchLookUpEdit lueAccountingCostCenter;
    private GridView grvAccountingCostCenterLookup;
    private LabelControl lblAccountingProject;
    private SearchLookUpEdit lueAccountingProject;
    private GridView grvAccountingProjectLookup;
    private LabelControl lblAccountingAccountsTitle;
    private LabelControl lblAccountingSupplierAccount;
    private LabelControl lblAccountingAdvanceAccount;
    private LabelControl lblAccountingDefaultExpenseAccount;
    private LabelControl lblAccountingDifferenceAccount;
    private LabelControl lblAccountingRoundingAccount;
    private LabelControl lblAccountingClearingAccount;
    private LabelControl lblAccountingDiscountAccount;
    private LabelControl lblAccountingRetentionPayableAccount;
    private LabelControl lblAccountingAllowsCompensation;
    private LabelControl lblAccountingAllowsAdvance;
    private LabelControl lblAccountingRequiresProvision;
    private LabelControl lblAccountingBySupplier;
    private ToggleSwitch chkAccountingAllowsCompensation;
    private ToggleSwitch chkAccountingAllowsAdvance;
    private ToggleSwitch chkAccountingRequiresProvision;
    private ToggleSwitch chkAccountingBySupplier;
    private SearchLookUpEdit lueAccountingRetentionPayableAccount;
    private GridView grvAccountingRetentionPayableAccountLookup;
    private SearchLookUpEdit lueAccountingDiscountAccount;
    private GridView grvAccountingDiscountAccountLookup;
    private SearchLookUpEdit lueAccountingClearingAccount;
    private GridView grvAccountingClearingAccountLookup;
    private SearchLookUpEdit lueAccountingRoundingAccount;
    private GridView grvAccountingRoundingAccountLookup;
    private SearchLookUpEdit lueAccountingDifferenceAccount;
    private GridView grvAccountingDifferenceAccountLookup;
    private SearchLookUpEdit lueAccountingAdvanceAccount;
    private GridView grvAccountingAdvanceAccountLookup;
    private SearchLookUpEdit lueAccountingDefaultExpenseAccount;
    private GridView grvAccountingDefaultExpenseAccountLookup;
    private SearchLookUpEdit lueAccountingSupplierAccount;
    private GridView grvAccountingSupplierAccountLookup;
    private ToggleSwitch chkAccountingConciliationRequired;
    private ToggleSwitch chkAccountingUsesWithholdingBase;
    private ToggleSwitch chkAccountingBlocked;
    private ToggleSwitch chkAccountingAllowsPartialPayments;
    private LabelControl lblAccountingConciliationRequired;
    private LabelControl lblAccountingUsesWithholdingBase;
    private LabelControl lblAccountingBlocked;
    private LabelControl lblAccountingAllowsPartialPayments;
}


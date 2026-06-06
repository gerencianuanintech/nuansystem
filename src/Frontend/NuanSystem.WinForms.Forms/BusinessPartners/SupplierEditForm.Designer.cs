using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

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
        pnlMain = new PanelControl();
        tabSupplier = new XtraTabControl();
        tabGeneral = new XtraTabPage();
        pnlGeneralContent = new PanelControl();
        lblCountry = new LabelControl();
        lueCountry = new LookUpEdit();
        lblProvinceCity = new LabelControl();
        txtProvinceCity = new TextEdit();
        lblWebsite = new LabelControl();
        txtWebsite = new TextEdit();
        lblRegistrationDate = new LabelControl();
        dteRegistrationDate = new DateEdit();
        lblCreditLimit = new LabelControl();
        spnCreditLimit = new SpinEdit();
        lblPaymentTermDays = new LabelControl();
        spnPaymentTermDays = new SpinEdit();
        lblAlternateCurrency = new LabelControl();
        lueAlternateCurrency = new LookUpEdit();
        lblInternalClassification = new LabelControl();
        lueInternalClassification = new LookUpEdit();
        lblSupplierSegment = new LabelControl();
        lueSupplierSegment = new LookUpEdit();
        lblActiveForPurchases = new LabelControl();
        tglActiveForPurchases = new ToggleSwitch();
        lblActiveForPurchasesValue = new LabelControl();
        lblSubjectToWithholding = new LabelControl();
        tglSubjectToWithholding = new ToggleSwitch();
        lblSubjectToWithholdingValue = new LabelControl();
        lblHandlesCredit = new LabelControl();
        tglHandlesCredit = new ToggleSwitch();
        lblHandlesCreditValue = new LabelControl();
        lblBlocked = new LabelControl();
        tglBlocked = new ToggleSwitch();
        lblBlockedValue = new LabelControl();
        lblGeneralComments = new LabelControl();
        memGeneralComments = new MemoEdit();
        tabContacts = new XtraTabPage();
        pnlContactsContent = new PanelControl();
        grdContacts = new GridControl();
        gvContacts = new GridView();
        colContactFullName = new GridColumn();
        colContactPosition = new GridColumn();
        colContactDepartment = new GridColumn();
        colContactIsPrimary = new GridColumn();
        colContactIsActive = new GridColumn();
        pnlContactsActions = new PanelControl();
        btnAddContact = new SimpleButton();
        btnEditContact = new SimpleButton();
        btnDeleteContact = new SimpleButton();
        btnSetDefaultContact = new SimpleButton();
        tabAddresses = new XtraTabPage();
        pnlAddressesContent = new PanelControl();
        grdAddresses = new GridControl();
        gvAddresses = new GridView();
        colAddressType = new GridColumn();
        colAddressCode = new GridColumn();
        colAddressFullAddress = new GridColumn();
        colAddressProvinceCity = new GridColumn();
        colAddressCountry = new GridColumn();
        colAddressReference = new GridColumn();
        colAddressIsPrimary = new GridColumn();
        colAddressIsActive = new GridColumn();
        pnlAddressesActions = new PanelControl();
        btnAddAddress = new SimpleButton();
        btnEditAddress = new SimpleButton();
        btnDeleteAddress = new SimpleButton();
        btnDuplicateAddress = new SimpleButton();
        btnSetDefaultAddress = new SimpleButton();
        tabPurchases = new XtraTabPage();
        pnlPurchasesContent = new PanelControl();
        lblPurchasePaymentCondition = new LabelControl();
        luePurchasePaymentCondition = new LookUpEdit();
        lblPurchasePriceList = new LabelControl();
        luePurchasePriceList = new LookUpEdit();
        lblDeliveryTermDays = new LabelControl();
        spnDeliveryTermDays = new SpinEdit();
        lblIncoterm = new LabelControl();
        lueIncoterm = new LookUpEdit();
        lblPurchaseCurrency = new LabelControl();
        luePurchaseCurrency = new LookUpEdit();
        lblCommercialDiscountPercent = new LabelControl();
        spnCommercialDiscountPercent = new SpinEdit();
        lblPurchaseSupplierType = new LabelControl();
        luePurchaseSupplierType = new LookUpEdit();
        lblAssignedBuyer = new LabelControl();
        lueAssignedBuyer = new LookUpEdit();
        lblSuggestedCostCenter = new LabelControl();
        lueSuggestedCostCenter = new LookUpEdit();
        lblPreferredWarehouse = new LabelControl();
        luePreferredWarehouse = new LookUpEdit();
        lblAverageDeliveryDays = new LabelControl();
        spnAverageDeliveryDays = new SpinEdit();
        lblMinimumOrderAmount = new LabelControl();
        spnMinimumOrderAmount = new SpinEdit();
        lblMinimumOrderQuantity = new LabelControl();
        spnMinimumOrderQuantity = new SpinEdit();
        lblLeadTimeDays = new LabelControl();
        spnLeadTimeDays = new SpinEdit();
        lblDeliveryToleranceDays = new LabelControl();
        spnDeliveryToleranceDays = new SpinEdit();
        lblRequiresPurchaseOrder = new LabelControl();
        tglRequiresPurchaseOrder = new ToggleSwitch();
        lblRequiresPurchaseOrderValue = new LabelControl();
        lblSubjectToEvaluation = new LabelControl();
        tglSubjectToEvaluation = new ToggleSwitch();
        lblSubjectToEvaluationValue = new LabelControl();
        lblActiveForImport = new LabelControl();
        tglActiveForImport = new ToggleSwitch();
        lblActiveForImportValue = new LabelControl();
        lblAllowsUrgentPurchases = new LabelControl();
        tglAllowsUrgentPurchases = new ToggleSwitch();
        lblAllowsUrgentPurchasesValue = new LabelControl();
        lblPurchaseHistoryTitle = new LabelControl();
        grdPurchaseHistory = new GridControl();
        gvPurchaseHistory = new GridView();
        colPurchaseDate = new GridColumn();
        colPurchaseDocumentNumber = new GridColumn();
        colPurchaseAmount = new GridColumn();
        colPurchaseCurrency = new GridColumn();
        colPurchaseAverageDeliveryDays = new GridColumn();
        pnlPurchasesLast12Months = new PanelControl();
        lblPurchasesLast12MonthsCaption = new LabelControl();
        lblPurchasesLast12MonthsValue = new LabelControl();
        pnlAveragePurchase = new PanelControl();
        lblAveragePurchaseCaption = new LabelControl();
        lblAveragePurchaseValue = new LabelControl();
        pnlAverageDelivery12Months = new PanelControl();
        lblAverageDelivery12MonthsCaption = new LabelControl();
        lblAverageDelivery12MonthsValue = new LabelControl();
        pnlPurchaseOrdersLast12Months = new PanelControl();
        lblPurchaseOrdersLast12MonthsCaption = new LabelControl();
        lblPurchaseOrdersLast12MonthsValue = new LabelControl();
        tabBanks = new XtraTabPage();
        pnlBanksContent = new PanelControl();
        pnlBanksActions = new PanelControl();
        btnAddBankAccount = new SimpleButton();
        btnEditBankAccount = new SimpleButton();
        btnDeleteBankAccount = new SimpleButton();
        btnSetDefaultBankAccount = new SimpleButton();
        grdBankAccounts = new GridControl();
        gvBankAccounts = new GridView();
        colBankName = new GridColumn();
        colBankAccountType = new GridColumn();
        colBankAccountNumber = new GridColumn();
        colBankCurrency = new GridColumn();
        colBankSwiftBic = new GridColumn();
        colBankCciIban = new GridColumn();
        colBankAccountHolder = new GridColumn();
        colBankIsDefault = new GridColumn();
        colBankIsActive = new GridColumn();
        lblBankAccountsTotal = new LabelControl();
        tabWithholdings = new XtraTabPage();
        pnlWithholdingsContent = new PanelControl();
        pnlWithholdingsGeneral = new PanelControl();
        lblWithholdingAgent = new LabelControl();
        tglWithholdingAgent = new ToggleSwitch();
        lblWithholdingAgentValue = new LabelControl();
        lblGeneralWithholdingType = new LabelControl();
        lueGeneralWithholdingType = new LookUpEdit();
        lblBaseWithholdingPercent = new LabelControl();
        spnBaseWithholdingPercent = new SpinEdit();
        lblWithholdingEffectiveDate = new LabelControl();
        dteWithholdingEffectiveDate = new DateEdit();
        lblWithholdingResolutionNumber = new LabelControl();
        txtWithholdingResolutionNumber = new TextEdit();
        lblWithholdsVat = new LabelControl();
        tglWithholdsVat = new ToggleSwitch();
        lblWithholdsVatValue = new LabelControl();
        lblWithholdsIncomeTax = new LabelControl();
        tglWithholdsIncomeTax = new ToggleSwitch();
        lblWithholdsIncomeTaxValue = new LabelControl();
        lblIssuesElectronicReceipts = new LabelControl();
        tglIssuesElectronicReceipts = new ToggleSwitch();
        lblIssuesElectronicReceiptsValue = new LabelControl();
        lblSubjectToPerception = new LabelControl();
        tglSubjectToPerception = new ToggleSwitch();
        lblSubjectToPerceptionValue = new LabelControl();
        pnlWithholdingsActions = new PanelControl();
        btnAddWithholding = new SimpleButton();
        btnEditWithholding = new SimpleButton();
        btnDeleteWithholding = new SimpleButton();
        btnSetDefaultWithholding = new SimpleButton();
        grdWithholdings = new GridControl();
        gvWithholdings = new GridView();
        colWithholdingDocument = new GridColumn();
        colWithholdingType = new GridColumn();
        colWithholdingValidity = new GridColumn();
        colWithholdingIsDefault = new GridColumn();
        colWithholdingStatus = new GridColumn();
        tabAccounting = new XtraTabPage();
        pnlAccountingContent = new PanelControl();
        pnlAccountingGeneral = new PanelControl();
        lblDefaultProject = new LabelControl();
        lueDefaultProject = new LookUpEdit();
        lblFiscalCondition = new LabelControl();
        lueFiscalCondition = new LookUpEdit();
        lblThirdPartyType = new LabelControl();
        lueThirdPartyType = new LookUpEdit();
        lblAutomaticAccounting = new LabelControl();
        tglAutomaticAccounting = new ToggleSwitch();
        lblAutomaticAccountingValue = new LabelControl();
        lblRequiresReconciliation = new LabelControl();
        tglRequiresReconciliation = new ToggleSwitch();
        lblRequiresReconciliationValue = new LabelControl();
        lblHandlesAdvances = new LabelControl();
        tglHandlesAdvances = new ToggleSwitch();
        lblHandlesAdvancesValue = new LabelControl();
        lblAccountingBlocked = new LabelControl();
        tglAccountingBlocked = new ToggleSwitch();
        lblAccountingBlockedValue = new LabelControl();
        pnlAccountingActions = new PanelControl();
        btnAddAccountingAccount = new SimpleButton();
        btnEditAccountingAccount = new SimpleButton();
        btnDeleteAccountingAccount = new SimpleButton();
        btnSetDefaultAccountingAccount = new SimpleButton();
        grdAccountingAccounts = new GridControl();
        gvAccountingAccounts = new GridView();
        colAccountingAccountType = new GridColumn();
        colAccountingAccountCodeName = new GridColumn();
        colAccountingDimension1 = new GridColumn();
        colAccountingDimension2 = new GridColumn();
        colAccountingDimension3 = new GridColumn();
        colAccountingDimension4 = new GridColumn();
        colAccountingDimension5 = new GridColumn();
        colAccountingIsDefault = new GridColumn();
        colAccountingIsActive = new GridColumn();
        tabSap = new XtraTabPage();
        pnlSapContent = new PanelControl();
        pnlSapSyncData = new PanelControl();
        lblSapSynchronized = new LabelControl();
        tglSapSynchronized = new ToggleSwitch();
        lblSapSynchronizedValue = new LabelControl();
        lblSapIntegrationValid = new LabelControl();
        tglSapIntegrationValid = new ToggleSwitch();
        lblSapIntegrationValidValue = new LabelControl();
        lblSapErrorBlocked = new LabelControl();
        tglSapErrorBlocked = new ToggleSwitch();
        lblSapErrorBlockedValue = new LabelControl();
        lblSapAutoUpdate = new LabelControl();
        tglSapAutoUpdate = new ToggleSwitch();
        lblSapAutoUpdateValue = new LabelControl();
        lblSapLastSync = new LabelControl();
        txtSapLastSync = new TextEdit();
        lblSapLastSyncUser = new LabelControl();
        txtSapLastSyncUser = new TextEdit();
        lblSapDataOrigin = new LabelControl();
        txtSapDataOrigin = new TextEdit();
        lblSapIntegrationStatus = new LabelControl();
        txtSapIntegrationStatus = new TextEdit();
        pnlSapAudit = new PanelControl();
        lblSapAuditTitle = new LabelControl();
        grdSapAudit = new GridControl();
        gvSapAudit = new GridView();
        colSapAuditDate = new GridColumn();
        colSapAuditAction = new GridColumn();
        colSapAuditResult = new GridColumn();
        colSapAuditUser = new GridColumn();
        colSapAuditMessage = new GridColumn();
        tabAttachments = new XtraTabPage();
        pnlAttachmentsContent = new PanelControl();
        pnlObservationsSection = new PanelControl();
        lblSupplierObservationsTitle = new LabelControl();
        memSupplierObservations = new MemoEdit();
        pnlDocumentsSection = new PanelControl();
        lblAttachmentsTitle = new LabelControl();
        pnlAttachmentActions = new PanelControl();
        btnAttachDocument = new SimpleButton();
        btnDownloadDocument = new SimpleButton();
        btnViewDocument = new SimpleButton();
        btnDeleteDocument = new SimpleButton();
        grdAttachments = new GridControl();
        gvAttachments = new GridView();
        colAttachmentDocumentType = new GridColumn();
        colAttachmentFileName = new GridColumn();
        colAttachmentUploadDate = new GridColumn();
        colAttachmentUser = new GridColumn();
        colAttachmentFileSize = new GridColumn();
        colAttachmentStatus = new GridColumn();
        lblAttachmentPath = new LabelControl();
        txtAttachmentPath = new TextEdit();
        lblAttachmentCategory = new LabelControl();
        txtAttachmentCategory = new TextEdit();
        lblAttachmentExpirationDate = new LabelControl();
        txtAttachmentExpirationDate = new TextEdit();
        lblAttachmentDescription = new LabelControl();
        memAttachmentDescription = new MemoEdit();
        pnlAttachmentPreview = new PanelControl();
        lblAttachmentPreviewTitle = new LabelControl();
        lblAttachmentPreview = new LabelControl();
        pnlHeader = new PanelControl();
        lblTitle = new LabelControl();
        lblSupplierCode = new LabelControl();
        txtSupplierCode = new TextEdit();
        lblSupplierActive = new LabelControl();
        tglSupplierActive = new ToggleSwitch();
        lblBusinessName = new LabelControl();
        txtBusinessName = new TextEdit();
        lblTradeName = new LabelControl();
        txtTradeName = new TextEdit();
        lblDocumentType = new LabelControl();
        lueDocumentType = new LookUpEdit();
        lblDocumentNumber = new LabelControl();
        txtDocumentNumber = new TextEdit();
        lblPersonType = new LabelControl();
        luePersonType = new LookUpEdit();
        lblSupplierType = new LabelControl();
        lueSupplierType = new LookUpEdit();
        lblMainContact = new LabelControl();
        txtMainContact = new TextEdit();
        lblPhone = new LabelControl();
        txtPhone = new TextEdit();
        lblEmail = new LabelControl();
        txtEmail = new TextEdit();
        lblCurrency = new LabelControl();
        lueCurrency = new LookUpEdit();
        lblPaymentCondition = new LabelControl();
        luePaymentCondition = new LookUpEdit();
        lblSupplierCategory = new LabelControl();
        lueSupplierCategory = new LookUpEdit();
        lblShortObservation = new LabelControl();
        memShortObservation = new MemoEdit();
        ((System.ComponentModel.ISupportInitialize)pnlMain).BeginInit();
        pnlMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tabSupplier).BeginInit();
        tabSupplier.SuspendLayout();
        tabGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlGeneralContent).BeginInit();
        pnlGeneralContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtProvinceCity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtWebsite.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteRegistrationDate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteRegistrationDate.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditLimit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnPaymentTermDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAlternateCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueInternalClassification.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierSegment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglActiveForPurchases.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSubjectToWithholding.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglHandlesCredit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglBlocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memGeneralComments.Properties).BeginInit();
        tabContacts.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlContactsContent).BeginInit();
        pnlContactsContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdContacts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvContacts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlContactsActions).BeginInit();
        pnlContactsActions.SuspendLayout();
        tabAddresses.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAddressesContent).BeginInit();
        pnlAddressesContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdAddresses).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvAddresses).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlAddressesActions).BeginInit();
        pnlAddressesActions.SuspendLayout();
        tabPurchases.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlPurchasesContent).BeginInit();
        pnlPurchasesContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)luePurchasePaymentCondition.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchasePriceList.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnDeliveryTermDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueIncoterm.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCommercialDiscountPercent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseSupplierType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAssignedBuyer.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSuggestedCostCenter.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePreferredWarehouse.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnAverageDeliveryDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumOrderAmount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumOrderQuantity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnLeadTimeDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnDeliveryToleranceDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresPurchaseOrder.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSubjectToEvaluation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglActiveForImport.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowsUrgentPurchases.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdPurchaseHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseHistory).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlPurchasesLast12Months).BeginInit();
        pnlPurchasesLast12Months.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAveragePurchase).BeginInit();
        pnlAveragePurchase.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAverageDelivery12Months).BeginInit();
        pnlAverageDelivery12Months.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlPurchaseOrdersLast12Months).BeginInit();
        pnlPurchaseOrdersLast12Months.SuspendLayout();
        tabBanks.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlBanksContent).BeginInit();
        pnlBanksContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlBanksActions).BeginInit();
        pnlBanksActions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdBankAccounts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvBankAccounts).BeginInit();
        tabWithholdings.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlWithholdingsContent).BeginInit();
        pnlWithholdingsContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlWithholdingsGeneral).BeginInit();
        pnlWithholdingsGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglWithholdingAgent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueGeneralWithholdingType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnBaseWithholdingPercent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteWithholdingEffectiveDate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteWithholdingEffectiveDate.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtWithholdingResolutionNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglWithholdsVat.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglWithholdsIncomeTax.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglIssuesElectronicReceipts.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSubjectToPerception.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlWithholdingsActions).BeginInit();
        pnlWithholdingsActions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdWithholdings).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvWithholdings).BeginInit();
        tabAccounting.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingContent).BeginInit();
        pnlAccountingContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingGeneral).BeginInit();
        pnlAccountingGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueDefaultProject.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCondition.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueThirdPartyType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAutomaticAccounting.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresReconciliation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglHandlesAdvances.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAccountingBlocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingActions).BeginInit();
        pnlAccountingActions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdAccountingAccounts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvAccountingAccounts).BeginInit();
        tabSap.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSapContent).BeginInit();
        pnlSapContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSapSyncData).BeginInit();
        pnlSapSyncData.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglSapSynchronized.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSapIntegrationValid.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSapErrorBlocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSapAutoUpdate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastSync.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastSyncUser.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapDataOrigin.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapIntegrationStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlSapAudit).BeginInit();
        pnlSapAudit.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdSapAudit).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvSapAudit).BeginInit();
        tabAttachments.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentsContent).BeginInit();
        pnlAttachmentsContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlObservationsSection).BeginInit();
        pnlObservationsSection.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memSupplierObservations.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlDocumentsSection).BeginInit();
        pnlDocumentsSection.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentActions).BeginInit();
        pnlAttachmentActions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentPath.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentCategory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentExpirationDate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memAttachmentDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentPreview).BeginInit();
        pnlAttachmentPreview.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlHeader).BeginInit();
        pnlHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)txtSupplierCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSupplierActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBusinessName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtTradeName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueDocumentType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDocumentNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePersonType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtMainContact.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPhone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtEmail.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePaymentCondition.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierCategory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memShortObservation.Properties).BeginInit();
        SuspendLayout();
        // 
        // pnlMain
        // 
        pnlMain.BorderStyle = BorderStyles.NoBorder;
        pnlMain.Controls.Add(tabSupplier);
        pnlMain.Controls.Add(pnlHeader);
        pnlMain.Dock = DockStyle.Fill;
        pnlMain.Location = new Point(0, 0);
        pnlMain.Name = "pnlMain";
        pnlMain.Size = new Size(1344, 749);
        pnlMain.TabIndex = 0;
        // 
        // tabSupplier
        // 
        tabSupplier.Appearance.Font = new Font("Segoe UI", 9F);
        tabSupplier.Appearance.Options.UseFont = true;
        tabSupplier.AppearancePage.Header.Font = new Font("Segoe UI", 9F);
        tabSupplier.AppearancePage.Header.Options.UseFont = true;
        tabSupplier.AppearancePage.HeaderActive.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        tabSupplier.AppearancePage.HeaderActive.ForeColor = Color.FromArgb(0, 102, 204);
        tabSupplier.AppearancePage.HeaderActive.Options.UseFont = true;
        tabSupplier.AppearancePage.HeaderActive.Options.UseForeColor = true;
        tabSupplier.Dock = DockStyle.Fill;
        tabSupplier.Location = new Point(0, 318);
        tabSupplier.Name = "tabSupplier";
        tabSupplier.SelectedTabPage = tabGeneral;
        tabSupplier.Size = new Size(1344, 431);
        tabSupplier.TabIndex = 1;
        tabSupplier.TabPages.AddRange(new XtraTabPage[] { tabGeneral, tabContacts, tabAddresses, tabPurchases, tabBanks, tabWithholdings, tabAccounting, tabSap, tabAttachments });
        // 
        // tabGeneral
        // 
        tabGeneral.Controls.Add(pnlGeneralContent);
        tabGeneral.Name = "tabGeneral";
        tabGeneral.Size = new Size(1342, 404);
        tabGeneral.Text = "General";
        // 
        // pnlGeneralContent
        // 
        pnlGeneralContent.BorderStyle = BorderStyles.Simple;
        pnlGeneralContent.Controls.Add(lblCountry);
        pnlGeneralContent.Controls.Add(lueCountry);
        pnlGeneralContent.Controls.Add(lblProvinceCity);
        pnlGeneralContent.Controls.Add(txtProvinceCity);
        pnlGeneralContent.Controls.Add(lblWebsite);
        pnlGeneralContent.Controls.Add(txtWebsite);
        pnlGeneralContent.Controls.Add(lblRegistrationDate);
        pnlGeneralContent.Controls.Add(dteRegistrationDate);
        pnlGeneralContent.Controls.Add(lblCreditLimit);
        pnlGeneralContent.Controls.Add(spnCreditLimit);
        pnlGeneralContent.Controls.Add(lblPaymentTermDays);
        pnlGeneralContent.Controls.Add(spnPaymentTermDays);
        pnlGeneralContent.Controls.Add(lblAlternateCurrency);
        pnlGeneralContent.Controls.Add(lueAlternateCurrency);
        pnlGeneralContent.Controls.Add(lblInternalClassification);
        pnlGeneralContent.Controls.Add(lueInternalClassification);
        pnlGeneralContent.Controls.Add(lblSupplierSegment);
        pnlGeneralContent.Controls.Add(lueSupplierSegment);
        pnlGeneralContent.Controls.Add(lblActiveForPurchases);
        pnlGeneralContent.Controls.Add(tglActiveForPurchases);
        pnlGeneralContent.Controls.Add(lblActiveForPurchasesValue);
        pnlGeneralContent.Controls.Add(lblSubjectToWithholding);
        pnlGeneralContent.Controls.Add(tglSubjectToWithholding);
        pnlGeneralContent.Controls.Add(lblSubjectToWithholdingValue);
        pnlGeneralContent.Controls.Add(lblHandlesCredit);
        pnlGeneralContent.Controls.Add(tglHandlesCredit);
        pnlGeneralContent.Controls.Add(lblHandlesCreditValue);
        pnlGeneralContent.Controls.Add(lblBlocked);
        pnlGeneralContent.Controls.Add(tglBlocked);
        pnlGeneralContent.Controls.Add(lblBlockedValue);
        pnlGeneralContent.Controls.Add(lblGeneralComments);
        pnlGeneralContent.Controls.Add(memGeneralComments);
        pnlGeneralContent.Dock = DockStyle.Fill;
        pnlGeneralContent.Location = new Point(0, 0);
        pnlGeneralContent.Name = "pnlGeneralContent";
        pnlGeneralContent.Size = new Size(1342, 404);
        pnlGeneralContent.TabIndex = 0;
        // 
        // lblCountry
        // 
        lblCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblCountry.Appearance.Options.UseFont = true;
        lblCountry.Location = new Point(44, 33);
        lblCountry.Name = "lblCountry";
        lblCountry.Size = new Size(24, 15);
        lblCountry.TabIndex = 0;
        lblCountry.Text = "País:";
        // 
        // lueCountry
        // 
        lueCountry.Location = new Point(194, 30);
        lueCountry.Name = "lueCountry";
        lueCountry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCountry.Properties.Appearance.Options.UseFont = true;
        lueCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCountry.Properties.NullText = "";
        lueCountry.Size = new Size(246, 22);
        lueCountry.TabIndex = 1;
        // 
        // lblProvinceCity
        // 
        lblProvinceCity.Appearance.Font = new Font("Segoe UI", 9F);
        lblProvinceCity.Appearance.Options.UseFont = true;
        lblProvinceCity.Location = new Point(44, 71);
        lblProvinceCity.Name = "lblProvinceCity";
        lblProvinceCity.Size = new Size(101, 15);
        lblProvinceCity.TabIndex = 2;
        lblProvinceCity.Text = "Provincia / Ciudad:";
        // 
        // txtProvinceCity
        // 
        txtProvinceCity.Location = new Point(194, 68);
        txtProvinceCity.Name = "txtProvinceCity";
        txtProvinceCity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtProvinceCity.Properties.Appearance.Options.UseFont = true;
        txtProvinceCity.Size = new Size(246, 22);
        txtProvinceCity.TabIndex = 3;
        // 
        // lblWebsite
        // 
        lblWebsite.Appearance.Font = new Font("Segoe UI", 9F);
        lblWebsite.Appearance.Options.UseFont = true;
        lblWebsite.Location = new Point(44, 109);
        lblWebsite.Name = "lblWebsite";
        lblWebsite.Size = new Size(53, 15);
        lblWebsite.TabIndex = 4;
        lblWebsite.Text = "Sitio Web:";
        // 
        // txtWebsite
        // 
        txtWebsite.Location = new Point(194, 106);
        txtWebsite.Name = "txtWebsite";
        txtWebsite.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtWebsite.Properties.Appearance.Options.UseFont = true;
        txtWebsite.Size = new Size(246, 22);
        txtWebsite.TabIndex = 5;
        // 
        // lblRegistrationDate
        // 
        lblRegistrationDate.Appearance.Font = new Font("Segoe UI", 9F);
        lblRegistrationDate.Appearance.Options.UseFont = true;
        lblRegistrationDate.Location = new Point(44, 147);
        lblRegistrationDate.Name = "lblRegistrationDate";
        lblRegistrationDate.Size = new Size(96, 15);
        lblRegistrationDate.TabIndex = 6;
        lblRegistrationDate.Text = "Fecha de Registro:";
        // 
        // dteRegistrationDate
        // 
        dteRegistrationDate.EditValue = null;
        dteRegistrationDate.Location = new Point(194, 144);
        dteRegistrationDate.Name = "dteRegistrationDate";
        dteRegistrationDate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dteRegistrationDate.Properties.Appearance.Options.UseFont = true;
        dteRegistrationDate.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteRegistrationDate.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteRegistrationDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
        dteRegistrationDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        dteRegistrationDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
        dteRegistrationDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        dteRegistrationDate.Properties.MaskSettings.Set("mask", "dd/MM/yyyy");
        dteRegistrationDate.Size = new Size(246, 22);
        dteRegistrationDate.TabIndex = 7;
        // 
        // lblCreditLimit
        // 
        lblCreditLimit.Appearance.Font = new Font("Segoe UI", 9F);
        lblCreditLimit.Appearance.Options.UseFont = true;
        lblCreditLimit.Location = new Point(44, 185);
        lblCreditLimit.Name = "lblCreditLimit";
        lblCreditLimit.Size = new Size(94, 15);
        lblCreditLimit.TabIndex = 8;
        lblCreditLimit.Text = "Límite de Crédito:";
        // 
        // spnCreditLimit
        // 
        spnCreditLimit.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnCreditLimit.Location = new Point(194, 182);
        spnCreditLimit.Name = "spnCreditLimit";
        spnCreditLimit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnCreditLimit.Properties.Appearance.Options.UseFont = true;
        spnCreditLimit.Properties.Appearance.Options.UseTextOptions = true;
        spnCreditLimit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnCreditLimit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnCreditLimit.Properties.DisplayFormat.FormatString = "n2";
        spnCreditLimit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnCreditLimit.Properties.EditFormat.FormatString = "n2";
        spnCreditLimit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnCreditLimit.Properties.MaskSettings.Set("mask", "n2");
        spnCreditLimit.Size = new Size(196, 22);
        spnCreditLimit.TabIndex = 9;
        // 
        // lblPaymentTermDays
        // 
        lblPaymentTermDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblPaymentTermDays.Appearance.Options.UseFont = true;
        lblPaymentTermDays.Location = new Point(44, 223);
        lblPaymentTermDays.Name = "lblPaymentTermDays";
        lblPaymentTermDays.Size = new Size(109, 15);
        lblPaymentTermDays.TabIndex = 10;
        lblPaymentTermDays.Text = "Plazo de Pago (días):";
        // 
        // spnPaymentTermDays
        // 
        spnPaymentTermDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnPaymentTermDays.Location = new Point(194, 220);
        spnPaymentTermDays.Name = "spnPaymentTermDays";
        spnPaymentTermDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnPaymentTermDays.Properties.Appearance.Options.UseFont = true;
        spnPaymentTermDays.Properties.Appearance.Options.UseTextOptions = true;
        spnPaymentTermDays.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnPaymentTermDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnPaymentTermDays.Properties.IsFloatValue = false;
        spnPaymentTermDays.Properties.MaskSettings.Set("mask", "d");
        spnPaymentTermDays.Size = new Size(196, 22);
        spnPaymentTermDays.TabIndex = 11;
        // 
        // lblAlternateCurrency
        // 
        lblAlternateCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblAlternateCurrency.Appearance.Options.UseFont = true;
        lblAlternateCurrency.Location = new Point(44, 261);
        lblAlternateCurrency.Name = "lblAlternateCurrency";
        lblAlternateCurrency.Size = new Size(88, 15);
        lblAlternateCurrency.TabIndex = 12;
        lblAlternateCurrency.Text = "Moneda Alterna:";
        // 
        // lueAlternateCurrency
        // 
        lueAlternateCurrency.Location = new Point(194, 258);
        lueAlternateCurrency.Name = "lueAlternateCurrency";
        lueAlternateCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAlternateCurrency.Properties.Appearance.Options.UseFont = true;
        lueAlternateCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAlternateCurrency.Properties.NullText = "";
        lueAlternateCurrency.Size = new Size(246, 22);
        lueAlternateCurrency.TabIndex = 13;
        // 
        // lblInternalClassification
        // 
        lblInternalClassification.Appearance.Font = new Font("Segoe UI", 9F);
        lblInternalClassification.Appearance.Options.UseFont = true;
        lblInternalClassification.Location = new Point(520, 33);
        lblInternalClassification.Name = "lblInternalClassification";
        lblInternalClassification.Size = new Size(110, 15);
        lblInternalClassification.TabIndex = 14;
        lblInternalClassification.Text = "Clasificación Interna:";
        // 
        // lueInternalClassification
        // 
        lueInternalClassification.Location = new Point(682, 30);
        lueInternalClassification.Name = "lueInternalClassification";
        lueInternalClassification.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueInternalClassification.Properties.Appearance.Options.UseFont = true;
        lueInternalClassification.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueInternalClassification.Properties.NullText = "";
        lueInternalClassification.Size = new Size(246, 22);
        lueInternalClassification.TabIndex = 15;
        // 
        // lblSupplierSegment
        // 
        lblSupplierSegment.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierSegment.Appearance.Options.UseFont = true;
        lblSupplierSegment.Location = new Point(520, 71);
        lblSupplierSegment.Name = "lblSupplierSegment";
        lblSupplierSegment.Size = new Size(57, 15);
        lblSupplierSegment.TabIndex = 16;
        lblSupplierSegment.Text = "Segmento:";
        // 
        // lueSupplierSegment
        // 
        lueSupplierSegment.Location = new Point(682, 68);
        lueSupplierSegment.Name = "lueSupplierSegment";
        lueSupplierSegment.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierSegment.Properties.Appearance.Options.UseFont = true;
        lueSupplierSegment.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierSegment.Properties.NullText = "";
        lueSupplierSegment.Size = new Size(246, 22);
        lueSupplierSegment.TabIndex = 17;
        // 
        // lblActiveForPurchases
        // 
        lblActiveForPurchases.Appearance.Font = new Font("Segoe UI", 9F);
        lblActiveForPurchases.Appearance.Options.UseFont = true;
        lblActiveForPurchases.Location = new Point(520, 109);
        lblActiveForPurchases.Name = "lblActiveForPurchases";
        lblActiveForPurchases.Size = new Size(114, 15);
        lblActiveForPurchases.TabIndex = 18;
        lblActiveForPurchases.Text = "Activo para Compras:";
        // 
        // tglActiveForPurchases
        // 
        tglActiveForPurchases.Location = new Point(682, 104);
        tglActiveForPurchases.Name = "tglActiveForPurchases";
        tglActiveForPurchases.Properties.OffText = "";
        tglActiveForPurchases.Properties.OnText = "";
        tglActiveForPurchases.Size = new Size(70, 18);
        tglActiveForPurchases.TabIndex = 19;
        // 
        // lblActiveForPurchasesValue
        // 
        lblActiveForPurchasesValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblActiveForPurchasesValue.Appearance.Options.UseFont = true;
        lblActiveForPurchasesValue.Location = new Point(756, 109);
        lblActiveForPurchasesValue.Name = "lblActiveForPurchasesValue";
        lblActiveForPurchasesValue.Size = new Size(10, 15);
        lblActiveForPurchasesValue.TabIndex = 20;
        lblActiveForPurchasesValue.Text = "Sí";
        // 
        // lblSubjectToWithholding
        // 
        lblSubjectToWithholding.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubjectToWithholding.Appearance.Options.UseFont = true;
        lblSubjectToWithholding.Location = new Point(520, 147);
        lblSubjectToWithholding.Name = "lblSubjectToWithholding";
        lblSubjectToWithholding.Size = new Size(101, 15);
        lblSubjectToWithholding.TabIndex = 21;
        lblSubjectToWithholding.Text = "Sujeto a Retención:";
        // 
        // tglSubjectToWithholding
        // 
        tglSubjectToWithholding.Location = new Point(682, 142);
        tglSubjectToWithholding.Name = "tglSubjectToWithholding";
        tglSubjectToWithholding.Properties.OffText = "";
        tglSubjectToWithholding.Properties.OnText = "";
        tglSubjectToWithholding.Size = new Size(70, 18);
        tglSubjectToWithholding.TabIndex = 22;
        // 
        // lblSubjectToWithholdingValue
        // 
        lblSubjectToWithholdingValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubjectToWithholdingValue.Appearance.Options.UseFont = true;
        lblSubjectToWithholdingValue.Location = new Point(756, 147);
        lblSubjectToWithholdingValue.Name = "lblSubjectToWithholdingValue";
        lblSubjectToWithholdingValue.Size = new Size(10, 15);
        lblSubjectToWithholdingValue.TabIndex = 23;
        lblSubjectToWithholdingValue.Text = "Sí";
        // 
        // lblHandlesCredit
        // 
        lblHandlesCredit.Appearance.Font = new Font("Segoe UI", 9F);
        lblHandlesCredit.Appearance.Options.UseFont = true;
        lblHandlesCredit.Location = new Point(520, 185);
        lblHandlesCredit.Name = "lblHandlesCredit";
        lblHandlesCredit.Size = new Size(84, 15);
        lblHandlesCredit.TabIndex = 24;
        lblHandlesCredit.Text = "Maneja Crédito:";
        // 
        // tglHandlesCredit
        // 
        tglHandlesCredit.Location = new Point(682, 180);
        tglHandlesCredit.Name = "tglHandlesCredit";
        tglHandlesCredit.Properties.OffText = "";
        tglHandlesCredit.Properties.OnText = "";
        tglHandlesCredit.Size = new Size(70, 18);
        tglHandlesCredit.TabIndex = 25;
        // 
        // lblHandlesCreditValue
        // 
        lblHandlesCreditValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblHandlesCreditValue.Appearance.Options.UseFont = true;
        lblHandlesCreditValue.Location = new Point(756, 185);
        lblHandlesCreditValue.Name = "lblHandlesCreditValue";
        lblHandlesCreditValue.Size = new Size(10, 15);
        lblHandlesCreditValue.TabIndex = 26;
        lblHandlesCreditValue.Text = "Sí";
        // 
        // lblBlocked
        // 
        lblBlocked.Appearance.Font = new Font("Segoe UI", 9F);
        lblBlocked.Appearance.Options.UseFont = true;
        lblBlocked.Location = new Point(520, 223);
        lblBlocked.Name = "lblBlocked";
        lblBlocked.Size = new Size(60, 15);
        lblBlocked.TabIndex = 27;
        lblBlocked.Text = "Bloqueado:";
        // 
        // tglBlocked
        // 
        tglBlocked.Location = new Point(682, 218);
        tglBlocked.Name = "tglBlocked";
        tglBlocked.Properties.OffText = "";
        tglBlocked.Properties.OnText = "";
        tglBlocked.Size = new Size(70, 18);
        tglBlocked.TabIndex = 28;
        // 
        // lblBlockedValue
        // 
        lblBlockedValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblBlockedValue.Appearance.Options.UseFont = true;
        lblBlockedValue.Location = new Point(756, 223);
        lblBlockedValue.Name = "lblBlockedValue";
        lblBlockedValue.Size = new Size(16, 15);
        lblBlockedValue.TabIndex = 29;
        lblBlockedValue.Text = "No";
        // 
        // lblGeneralComments
        // 
        lblGeneralComments.Appearance.Font = new Font("Segoe UI", 9F);
        lblGeneralComments.Appearance.Options.UseFont = true;
        lblGeneralComments.Location = new Point(980, 33);
        lblGeneralComments.Name = "lblGeneralComments";
        lblGeneralComments.Size = new Size(125, 15);
        lblGeneralComments.TabIndex = 30;
        lblGeneralComments.Text = "Comentarios Generales:";
        // 
        // memGeneralComments
        // 
        memGeneralComments.Location = new Point(980, 58);
        memGeneralComments.Name = "memGeneralComments";
        memGeneralComments.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memGeneralComments.Properties.Appearance.Options.UseFont = true;
        memGeneralComments.Size = new Size(348, 218);
        memGeneralComments.TabIndex = 31;
        // 
        // tabContacts
        // 
        tabContacts.Controls.Add(pnlContactsContent);
        tabContacts.Name = "tabContacts";
        tabContacts.Size = new Size(1342, 404);
        tabContacts.Text = "Contactos";
        // 
        // pnlContactsContent
        // 
        pnlContactsContent.BorderStyle = BorderStyles.Simple;
        pnlContactsContent.Controls.Add(grdContacts);
        pnlContactsContent.Controls.Add(pnlContactsActions);
        pnlContactsContent.Dock = DockStyle.Fill;
        pnlContactsContent.Location = new Point(0, 0);
        pnlContactsContent.Name = "pnlContactsContent";
        pnlContactsContent.Size = new Size(1342, 404);
        pnlContactsContent.TabIndex = 0;
        // 
        // grdContacts
        // 
        grdContacts.Dock = DockStyle.Fill;
        grdContacts.Location = new Point(2, 50);
        grdContacts.MainView = gvContacts;
        grdContacts.Name = "grdContacts";
        grdContacts.Size = new Size(1338, 352);
        grdContacts.TabIndex = 1;
        grdContacts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvContacts });
        // 
        // gvContacts
        // 
        gvContacts.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvContacts.Appearance.HeaderPanel.Options.UseFont = true;
        gvContacts.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvContacts.Appearance.Row.Options.UseFont = true;
        gvContacts.Columns.AddRange(new GridColumn[] { colContactFullName, colContactPosition, colContactDepartment, colContactIsPrimary, colContactIsActive });
        gvContacts.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
        gvContacts.GridControl = grdContacts;
        gvContacts.Name = "gvContacts";
        gvContacts.OptionsBehavior.Editable = false;
        gvContacts.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvContacts.OptionsView.ShowGroupPanel = false;
        // 
        // colContactFullName
        // 
        colContactFullName.Caption = "Nombre";
        colContactFullName.FieldName = "FullName";
        colContactFullName.Name = "colContactFullName";
        colContactFullName.Visible = true;
        colContactFullName.VisibleIndex = 0;
        colContactFullName.Width = 260;
        // 
        // colContactPosition
        // 
        colContactPosition.Caption = "Cargo";
        colContactPosition.FieldName = "Position";
        colContactPosition.Name = "colContactPosition";
        colContactPosition.Visible = true;
        colContactPosition.VisibleIndex = 1;
        colContactPosition.Width = 220;
        // 
        // colContactDepartment
        // 
        colContactDepartment.Caption = "Área";
        colContactDepartment.FieldName = "Department";
        colContactDepartment.Name = "colContactDepartment";
        colContactDepartment.Visible = true;
        colContactDepartment.VisibleIndex = 2;
        colContactDepartment.Width = 180;
        // 
        // colContactIsPrimary
        // 
        colContactIsPrimary.Caption = "Es Principal";
        colContactIsPrimary.FieldName = "IsPrimary";
        colContactIsPrimary.Name = "colContactIsPrimary";
        colContactIsPrimary.Visible = true;
        colContactIsPrimary.VisibleIndex = 3;
        colContactIsPrimary.Width = 140;
        // 
        // colContactIsActive
        // 
        colContactIsActive.Caption = "Activo";
        colContactIsActive.FieldName = "IsActive";
        colContactIsActive.Name = "colContactIsActive";
        colContactIsActive.Visible = true;
        colContactIsActive.VisibleIndex = 4;
        colContactIsActive.Width = 120;
        // 
        // pnlContactsActions
        // 
        pnlContactsActions.BorderStyle = BorderStyles.NoBorder;
        pnlContactsActions.Controls.Add(btnAddContact);
        pnlContactsActions.Controls.Add(btnEditContact);
        pnlContactsActions.Controls.Add(btnDeleteContact);
        pnlContactsActions.Controls.Add(btnSetDefaultContact);
        pnlContactsActions.Dock = DockStyle.Top;
        pnlContactsActions.Location = new Point(2, 2);
        pnlContactsActions.Name = "pnlContactsActions";
        pnlContactsActions.Size = new Size(1338, 48);
        pnlContactsActions.TabIndex = 0;
        // 
        // btnAddContact
        // 
        btnAddContact.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddContact.Appearance.Options.UseFont = true;
        btnAddContact.Location = new Point(12, 9);
        btnAddContact.Name = "btnAddContact";
        btnAddContact.Size = new Size(86, 28);
        btnAddContact.TabIndex = 0;
        btnAddContact.Text = "Agregar";
        // 
        // btnEditContact
        // 
        btnEditContact.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditContact.Appearance.Options.UseFont = true;
        btnEditContact.Location = new Point(110, 9);
        btnEditContact.Name = "btnEditContact";
        btnEditContact.Size = new Size(86, 28);
        btnEditContact.TabIndex = 1;
        btnEditContact.Text = "Editar";
        // 
        // btnDeleteContact
        // 
        btnDeleteContact.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteContact.Appearance.Options.UseFont = true;
        btnDeleteContact.Location = new Point(208, 9);
        btnDeleteContact.Name = "btnDeleteContact";
        btnDeleteContact.Size = new Size(86, 28);
        btnDeleteContact.TabIndex = 2;
        btnDeleteContact.Text = "Eliminar";
        // 
        // btnSetDefaultContact
        // 
        btnSetDefaultContact.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetDefaultContact.Appearance.Options.UseFont = true;
        btnSetDefaultContact.Location = new Point(306, 9);
        btnSetDefaultContact.Name = "btnSetDefaultContact";
        btnSetDefaultContact.Size = new Size(112, 28);
        btnSetDefaultContact.TabIndex = 3;
        btnSetDefaultContact.Text = "Predeterminar";
        // 
        // tabAddresses
        // 
        tabAddresses.Controls.Add(pnlAddressesContent);
        tabAddresses.Name = "tabAddresses";
        tabAddresses.Size = new Size(1342, 404);
        tabAddresses.Text = "Direcciones";
        // 
        // pnlAddressesContent
        // 
        pnlAddressesContent.BorderStyle = BorderStyles.Simple;
        pnlAddressesContent.Controls.Add(grdAddresses);
        pnlAddressesContent.Controls.Add(pnlAddressesActions);
        pnlAddressesContent.Dock = DockStyle.Fill;
        pnlAddressesContent.Location = new Point(0, 0);
        pnlAddressesContent.Name = "pnlAddressesContent";
        pnlAddressesContent.Size = new Size(1342, 404);
        pnlAddressesContent.TabIndex = 0;
        // 
        // grdAddresses
        // 
        grdAddresses.Dock = DockStyle.Fill;
        grdAddresses.Location = new Point(2, 50);
        grdAddresses.MainView = gvAddresses;
        grdAddresses.Name = "grdAddresses";
        grdAddresses.Size = new Size(1338, 352);
        grdAddresses.TabIndex = 1;
        grdAddresses.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvAddresses });
        // 
        // gvAddresses
        // 
        gvAddresses.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvAddresses.Appearance.HeaderPanel.Options.UseFont = true;
        gvAddresses.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvAddresses.Appearance.Row.Options.UseFont = true;
        gvAddresses.Columns.AddRange(new GridColumn[] { colAddressType, colAddressCode, colAddressFullAddress, colAddressProvinceCity, colAddressCountry, colAddressReference, colAddressIsPrimary, colAddressIsActive });
        gvAddresses.FocusRectStyle = DrawFocusRectStyle.RowFullFocus;
        gvAddresses.GridControl = grdAddresses;
        gvAddresses.Name = "gvAddresses";
        gvAddresses.OptionsBehavior.Editable = false;
        gvAddresses.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvAddresses.OptionsView.ShowGroupPanel = false;
        // 
        // colAddressType
        // 
        colAddressType.Caption = "Tipo";
        colAddressType.FieldName = "AddressType";
        colAddressType.Name = "colAddressType";
        colAddressType.Visible = true;
        colAddressType.VisibleIndex = 0;
        colAddressType.Width = 90;
        // 
        // colAddressCode
        // 
        colAddressCode.Caption = "Código";
        colAddressCode.FieldName = "Code";
        colAddressCode.Name = "colAddressCode";
        colAddressCode.Visible = true;
        colAddressCode.VisibleIndex = 1;
        colAddressCode.Width = 90;
        // 
        // colAddressFullAddress
        // 
        colAddressFullAddress.Caption = "Dirección";
        colAddressFullAddress.FieldName = "FullAddress";
        colAddressFullAddress.Name = "colAddressFullAddress";
        colAddressFullAddress.Visible = true;
        colAddressFullAddress.VisibleIndex = 2;
        colAddressFullAddress.Width = 300;
        // 
        // colAddressProvinceCity
        // 
        colAddressProvinceCity.Caption = "Provincia / Ciudad";
        colAddressProvinceCity.FieldName = "ProvinceCity";
        colAddressProvinceCity.Name = "colAddressProvinceCity";
        colAddressProvinceCity.Visible = true;
        colAddressProvinceCity.VisibleIndex = 3;
        colAddressProvinceCity.Width = 220;
        // 
        // colAddressCountry
        // 
        colAddressCountry.Caption = "País";
        colAddressCountry.FieldName = "Country";
        colAddressCountry.Name = "colAddressCountry";
        colAddressCountry.Visible = true;
        colAddressCountry.VisibleIndex = 4;
        colAddressCountry.Width = 100;
        // 
        // colAddressReference
        // 
        colAddressReference.Caption = "Referencia";
        colAddressReference.FieldName = "Reference";
        colAddressReference.Name = "colAddressReference";
        colAddressReference.Visible = true;
        colAddressReference.VisibleIndex = 5;
        colAddressReference.Width = 240;
        // 
        // colAddressIsPrimary
        // 
        colAddressIsPrimary.Caption = "Principal";
        colAddressIsPrimary.FieldName = "IsPrimary";
        colAddressIsPrimary.Name = "colAddressIsPrimary";
        colAddressIsPrimary.Visible = true;
        colAddressIsPrimary.VisibleIndex = 6;
        colAddressIsPrimary.Width = 90;
        // 
        // colAddressIsActive
        // 
        colAddressIsActive.Caption = "Activa";
        colAddressIsActive.FieldName = "IsActive";
        colAddressIsActive.Name = "colAddressIsActive";
        colAddressIsActive.Visible = true;
        colAddressIsActive.VisibleIndex = 7;
        colAddressIsActive.Width = 90;
        // 
        // pnlAddressesActions
        // 
        pnlAddressesActions.BorderStyle = BorderStyles.NoBorder;
        pnlAddressesActions.Controls.Add(btnAddAddress);
        pnlAddressesActions.Controls.Add(btnEditAddress);
        pnlAddressesActions.Controls.Add(btnDeleteAddress);
        pnlAddressesActions.Controls.Add(btnDuplicateAddress);
        pnlAddressesActions.Controls.Add(btnSetDefaultAddress);
        pnlAddressesActions.Dock = DockStyle.Top;
        pnlAddressesActions.Location = new Point(2, 2);
        pnlAddressesActions.Name = "pnlAddressesActions";
        pnlAddressesActions.Size = new Size(1338, 48);
        pnlAddressesActions.TabIndex = 0;
        // 
        // btnAddAddress
        // 
        btnAddAddress.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddAddress.Appearance.Options.UseFont = true;
        btnAddAddress.Location = new Point(12, 9);
        btnAddAddress.Name = "btnAddAddress";
        btnAddAddress.Size = new Size(86, 28);
        btnAddAddress.TabIndex = 0;
        btnAddAddress.Text = "Agregar";
        // 
        // btnEditAddress
        // 
        btnEditAddress.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditAddress.Appearance.Options.UseFont = true;
        btnEditAddress.Location = new Point(110, 9);
        btnEditAddress.Name = "btnEditAddress";
        btnEditAddress.Size = new Size(86, 28);
        btnEditAddress.TabIndex = 1;
        btnEditAddress.Text = "Editar";
        // 
        // btnDeleteAddress
        // 
        btnDeleteAddress.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteAddress.Appearance.Options.UseFont = true;
        btnDeleteAddress.Location = new Point(208, 9);
        btnDeleteAddress.Name = "btnDeleteAddress";
        btnDeleteAddress.Size = new Size(86, 28);
        btnDeleteAddress.TabIndex = 2;
        btnDeleteAddress.Text = "Eliminar";
        // 
        // btnDuplicateAddress
        // 
        btnDuplicateAddress.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDuplicateAddress.Appearance.Options.UseFont = true;
        btnDuplicateAddress.Location = new Point(306, 9);
        btnDuplicateAddress.Name = "btnDuplicateAddress";
        btnDuplicateAddress.Size = new Size(86, 28);
        btnDuplicateAddress.TabIndex = 3;
        btnDuplicateAddress.Text = "Duplicar";
        // 
        // btnSetDefaultAddress
        // 
        btnSetDefaultAddress.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetDefaultAddress.Appearance.Options.UseFont = true;
        btnSetDefaultAddress.Location = new Point(404, 9);
        btnSetDefaultAddress.Name = "btnSetDefaultAddress";
        btnSetDefaultAddress.Size = new Size(112, 28);
        btnSetDefaultAddress.TabIndex = 4;
        btnSetDefaultAddress.Text = "Predeterminada";
        // 
        // tabPurchases
        // 
        tabPurchases.Controls.Add(pnlPurchasesContent);
        tabPurchases.Name = "tabPurchases";
        tabPurchases.Size = new Size(1342, 404);
        tabPurchases.Text = "Compras";
        // 
        // pnlPurchasesContent
        // 
        pnlPurchasesContent.BorderStyle = BorderStyles.Simple;
        pnlPurchasesContent.Controls.Add(lblPurchasePaymentCondition);
        pnlPurchasesContent.Controls.Add(luePurchasePaymentCondition);
        pnlPurchasesContent.Controls.Add(lblPurchasePriceList);
        pnlPurchasesContent.Controls.Add(luePurchasePriceList);
        pnlPurchasesContent.Controls.Add(lblDeliveryTermDays);
        pnlPurchasesContent.Controls.Add(spnDeliveryTermDays);
        pnlPurchasesContent.Controls.Add(lblIncoterm);
        pnlPurchasesContent.Controls.Add(lueIncoterm);
        pnlPurchasesContent.Controls.Add(lblPurchaseCurrency);
        pnlPurchasesContent.Controls.Add(luePurchaseCurrency);
        pnlPurchasesContent.Controls.Add(lblCommercialDiscountPercent);
        pnlPurchasesContent.Controls.Add(spnCommercialDiscountPercent);
        pnlPurchasesContent.Controls.Add(lblPurchaseSupplierType);
        pnlPurchasesContent.Controls.Add(luePurchaseSupplierType);
        pnlPurchasesContent.Controls.Add(lblAssignedBuyer);
        pnlPurchasesContent.Controls.Add(lueAssignedBuyer);
        pnlPurchasesContent.Controls.Add(lblSuggestedCostCenter);
        pnlPurchasesContent.Controls.Add(lueSuggestedCostCenter);
        pnlPurchasesContent.Controls.Add(lblPreferredWarehouse);
        pnlPurchasesContent.Controls.Add(luePreferredWarehouse);
        pnlPurchasesContent.Controls.Add(lblAverageDeliveryDays);
        pnlPurchasesContent.Controls.Add(spnAverageDeliveryDays);
        pnlPurchasesContent.Controls.Add(lblMinimumOrderAmount);
        pnlPurchasesContent.Controls.Add(spnMinimumOrderAmount);
        pnlPurchasesContent.Controls.Add(lblMinimumOrderQuantity);
        pnlPurchasesContent.Controls.Add(spnMinimumOrderQuantity);
        pnlPurchasesContent.Controls.Add(lblLeadTimeDays);
        pnlPurchasesContent.Controls.Add(spnLeadTimeDays);
        pnlPurchasesContent.Controls.Add(lblDeliveryToleranceDays);
        pnlPurchasesContent.Controls.Add(spnDeliveryToleranceDays);
        pnlPurchasesContent.Controls.Add(lblRequiresPurchaseOrder);
        pnlPurchasesContent.Controls.Add(tglRequiresPurchaseOrder);
        pnlPurchasesContent.Controls.Add(lblRequiresPurchaseOrderValue);
        pnlPurchasesContent.Controls.Add(lblSubjectToEvaluation);
        pnlPurchasesContent.Controls.Add(tglSubjectToEvaluation);
        pnlPurchasesContent.Controls.Add(lblSubjectToEvaluationValue);
        pnlPurchasesContent.Controls.Add(lblActiveForImport);
        pnlPurchasesContent.Controls.Add(tglActiveForImport);
        pnlPurchasesContent.Controls.Add(lblActiveForImportValue);
        pnlPurchasesContent.Controls.Add(lblAllowsUrgentPurchases);
        pnlPurchasesContent.Controls.Add(tglAllowsUrgentPurchases);
        pnlPurchasesContent.Controls.Add(lblAllowsUrgentPurchasesValue);
        pnlPurchasesContent.Controls.Add(lblPurchaseHistoryTitle);
        pnlPurchasesContent.Controls.Add(grdPurchaseHistory);
        pnlPurchasesContent.Controls.Add(pnlPurchasesLast12Months);
        pnlPurchasesContent.Controls.Add(pnlAveragePurchase);
        pnlPurchasesContent.Controls.Add(pnlAverageDelivery12Months);
        pnlPurchasesContent.Controls.Add(pnlPurchaseOrdersLast12Months);
        pnlPurchasesContent.Dock = DockStyle.Fill;
        pnlPurchasesContent.Location = new Point(0, 0);
        pnlPurchasesContent.Name = "pnlPurchasesContent";
        pnlPurchasesContent.Size = new Size(1342, 404);
        pnlPurchasesContent.TabIndex = 0;
        // 
        // lblPurchasePaymentCondition
        // 
        lblPurchasePaymentCondition.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchasePaymentCondition.Appearance.Options.UseFont = true;
        lblPurchasePaymentCondition.Location = new Point(36, 31);
        lblPurchasePaymentCondition.Name = "lblPurchasePaymentCondition";
        lblPurchasePaymentCondition.Size = new Size(158, 15);
        lblPurchasePaymentCondition.TabIndex = 0;
        lblPurchasePaymentCondition.Text = "Condición de Pago (Compra):";
        // 
        // luePurchasePaymentCondition
        // 
        luePurchasePaymentCondition.Location = new Point(218, 28);
        luePurchasePaymentCondition.Name = "luePurchasePaymentCondition";
        luePurchasePaymentCondition.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchasePaymentCondition.Properties.Appearance.Options.UseFont = true;
        luePurchasePaymentCondition.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchasePaymentCondition.Properties.NullText = "";
        luePurchasePaymentCondition.Size = new Size(210, 22);
        luePurchasePaymentCondition.TabIndex = 1;
        // 
        // lblPurchasePriceList
        // 
        lblPurchasePriceList.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchasePriceList.Appearance.Options.UseFont = true;
        lblPurchasePriceList.Location = new Point(36, 61);
        lblPurchasePriceList.Name = "lblPurchasePriceList";
        lblPurchasePriceList.Size = new Size(146, 15);
        lblPurchasePriceList.TabIndex = 2;
        lblPurchasePriceList.Text = "Lista de Precios de Compra:";
        // 
        // luePurchasePriceList
        // 
        luePurchasePriceList.Location = new Point(218, 58);
        luePurchasePriceList.Name = "luePurchasePriceList";
        luePurchasePriceList.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchasePriceList.Properties.Appearance.Options.UseFont = true;
        luePurchasePriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchasePriceList.Properties.NullText = "";
        luePurchasePriceList.Size = new Size(210, 22);
        luePurchasePriceList.TabIndex = 3;
        // 
        // lblDeliveryTermDays
        // 
        lblDeliveryTermDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblDeliveryTermDays.Appearance.Options.UseFont = true;
        lblDeliveryTermDays.Location = new Point(36, 91);
        lblDeliveryTermDays.Name = "lblDeliveryTermDays";
        lblDeliveryTermDays.Size = new Size(122, 15);
        lblDeliveryTermDays.TabIndex = 4;
        lblDeliveryTermDays.Text = "Plazo de Entrega (días):";
        // 
        // spnDeliveryTermDays
        // 
        spnDeliveryTermDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnDeliveryTermDays.Location = new Point(218, 88);
        spnDeliveryTermDays.Name = "spnDeliveryTermDays";
        spnDeliveryTermDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnDeliveryTermDays.Properties.Appearance.Options.UseFont = true;
        spnDeliveryTermDays.Properties.Appearance.Options.UseTextOptions = true;
        spnDeliveryTermDays.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnDeliveryTermDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnDeliveryTermDays.Properties.IsFloatValue = false;
        spnDeliveryTermDays.Properties.MaskSettings.Set("mask", "d");
        spnDeliveryTermDays.Size = new Size(210, 22);
        spnDeliveryTermDays.TabIndex = 5;
        // 
        // lblIncoterm
        // 
        lblIncoterm.Appearance.Font = new Font("Segoe UI", 9F);
        lblIncoterm.Appearance.Options.UseFont = true;
        lblIncoterm.Location = new Point(36, 121);
        lblIncoterm.Name = "lblIncoterm";
        lblIncoterm.Size = new Size(51, 15);
        lblIncoterm.TabIndex = 6;
        lblIncoterm.Text = "Incoterm:";
        // 
        // lueIncoterm
        // 
        lueIncoterm.Location = new Point(218, 118);
        lueIncoterm.Name = "lueIncoterm";
        lueIncoterm.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueIncoterm.Properties.Appearance.Options.UseFont = true;
        lueIncoterm.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueIncoterm.Properties.NullText = "";
        lueIncoterm.Size = new Size(210, 22);
        lueIncoterm.TabIndex = 7;
        // 
        // lblPurchaseCurrency
        // 
        lblPurchaseCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseCurrency.Appearance.Options.UseFont = true;
        lblPurchaseCurrency.Location = new Point(36, 151);
        lblPurchaseCurrency.Name = "lblPurchaseCurrency";
        lblPurchaseCurrency.Size = new Size(109, 15);
        lblPurchaseCurrency.TabIndex = 8;
        lblPurchaseCurrency.Text = "Moneda de Compra:";
        // 
        // luePurchaseCurrency
        // 
        luePurchaseCurrency.Location = new Point(218, 148);
        luePurchaseCurrency.Name = "luePurchaseCurrency";
        luePurchaseCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseCurrency.Properties.Appearance.Options.UseFont = true;
        luePurchaseCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseCurrency.Properties.NullText = "";
        luePurchaseCurrency.Size = new Size(210, 22);
        luePurchaseCurrency.TabIndex = 9;
        // 
        // lblCommercialDiscountPercent
        // 
        lblCommercialDiscountPercent.Appearance.Font = new Font("Segoe UI", 9F);
        lblCommercialDiscountPercent.Appearance.Options.UseFont = true;
        lblCommercialDiscountPercent.Location = new Point(36, 181);
        lblCommercialDiscountPercent.Name = "lblCommercialDiscountPercent";
        lblCommercialDiscountPercent.Size = new Size(137, 15);
        lblCommercialDiscountPercent.TabIndex = 10;
        lblCommercialDiscountPercent.Text = "Descuento Comercial (%):";
        // 
        // spnCommercialDiscountPercent
        // 
        spnCommercialDiscountPercent.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnCommercialDiscountPercent.Location = new Point(218, 178);
        spnCommercialDiscountPercent.Name = "spnCommercialDiscountPercent";
        spnCommercialDiscountPercent.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnCommercialDiscountPercent.Properties.Appearance.Options.UseFont = true;
        spnCommercialDiscountPercent.Properties.Appearance.Options.UseTextOptions = true;
        spnCommercialDiscountPercent.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnCommercialDiscountPercent.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnCommercialDiscountPercent.Properties.DisplayFormat.FormatString = "n2";
        spnCommercialDiscountPercent.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnCommercialDiscountPercent.Properties.MaskSettings.Set("mask", "n2");
        spnCommercialDiscountPercent.Size = new Size(210, 22);
        spnCommercialDiscountPercent.TabIndex = 11;
        // 
        // lblPurchaseSupplierType
        // 
        lblPurchaseSupplierType.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseSupplierType.Appearance.Options.UseFont = true;
        lblPurchaseSupplierType.Location = new Point(36, 211);
        lblPurchaseSupplierType.Name = "lblPurchaseSupplierType";
        lblPurchaseSupplierType.Size = new Size(154, 15);
        lblPurchaseSupplierType.TabIndex = 12;
        lblPurchaseSupplierType.Text = "Tipo de Proveedor (Compra):";
        // 
        // luePurchaseSupplierType
        // 
        luePurchaseSupplierType.Location = new Point(218, 208);
        luePurchaseSupplierType.Name = "luePurchaseSupplierType";
        luePurchaseSupplierType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseSupplierType.Properties.Appearance.Options.UseFont = true;
        luePurchaseSupplierType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseSupplierType.Properties.NullText = "";
        luePurchaseSupplierType.Size = new Size(210, 22);
        luePurchaseSupplierType.TabIndex = 13;
        // 
        // lblAssignedBuyer
        // 
        lblAssignedBuyer.Appearance.Font = new Font("Segoe UI", 9F);
        lblAssignedBuyer.Appearance.Options.UseFont = true;
        lblAssignedBuyer.Location = new Point(36, 241);
        lblAssignedBuyer.Name = "lblAssignedBuyer";
        lblAssignedBuyer.Size = new Size(117, 15);
        lblAssignedBuyer.TabIndex = 14;
        lblAssignedBuyer.Text = "Comprador Asignado:";
        // 
        // lueAssignedBuyer
        // 
        lueAssignedBuyer.Location = new Point(218, 238);
        lueAssignedBuyer.Name = "lueAssignedBuyer";
        lueAssignedBuyer.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAssignedBuyer.Properties.Appearance.Options.UseFont = true;
        lueAssignedBuyer.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAssignedBuyer.Properties.NullText = "";
        lueAssignedBuyer.Size = new Size(210, 22);
        lueAssignedBuyer.TabIndex = 15;
        // 
        // lblSuggestedCostCenter
        // 
        lblSuggestedCostCenter.Appearance.Font = new Font("Segoe UI", 9F);
        lblSuggestedCostCenter.Appearance.Options.UseFont = true;
        lblSuggestedCostCenter.Location = new Point(36, 271);
        lblSuggestedCostCenter.Name = "lblSuggestedCostCenter";
        lblSuggestedCostCenter.Size = new Size(139, 15);
        lblSuggestedCostCenter.TabIndex = 16;
        lblSuggestedCostCenter.Text = "Centro de Costo Sugerido:";
        // 
        // lueSuggestedCostCenter
        // 
        lueSuggestedCostCenter.Location = new Point(218, 268);
        lueSuggestedCostCenter.Name = "lueSuggestedCostCenter";
        lueSuggestedCostCenter.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSuggestedCostCenter.Properties.Appearance.Options.UseFont = true;
        lueSuggestedCostCenter.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSuggestedCostCenter.Properties.NullText = "";
        lueSuggestedCostCenter.Size = new Size(210, 22);
        lueSuggestedCostCenter.TabIndex = 17;
        // 
        // lblPreferredWarehouse
        // 
        lblPreferredWarehouse.Appearance.Font = new Font("Segoe UI", 9F);
        lblPreferredWarehouse.Appearance.Options.UseFont = true;
        lblPreferredWarehouse.Location = new Point(36, 301);
        lblPreferredWarehouse.Name = "lblPreferredWarehouse";
        lblPreferredWarehouse.Size = new Size(93, 15);
        lblPreferredWarehouse.TabIndex = 18;
        lblPreferredWarehouse.Text = "Bodega Preferida:";
        // 
        // luePreferredWarehouse
        // 
        luePreferredWarehouse.Location = new Point(218, 298);
        luePreferredWarehouse.Name = "luePreferredWarehouse";
        luePreferredWarehouse.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePreferredWarehouse.Properties.Appearance.Options.UseFont = true;
        luePreferredWarehouse.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePreferredWarehouse.Properties.NullText = "";
        luePreferredWarehouse.Size = new Size(210, 22);
        luePreferredWarehouse.TabIndex = 19;
        // 
        // lblAverageDeliveryDays
        // 
        lblAverageDeliveryDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblAverageDeliveryDays.Appearance.Options.UseFont = true;
        lblAverageDeliveryDays.Location = new Point(456, 31);
        lblAverageDeliveryDays.Name = "lblAverageDeliveryDays";
        lblAverageDeliveryDays.Size = new Size(139, 15);
        lblAverageDeliveryDays.TabIndex = 20;
        lblAverageDeliveryDays.Text = "Días de Entrega Promedio:";
        // 
        // spnAverageDeliveryDays
        // 
        spnAverageDeliveryDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnAverageDeliveryDays.Location = new Point(626, 28);
        spnAverageDeliveryDays.Name = "spnAverageDeliveryDays";
        spnAverageDeliveryDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnAverageDeliveryDays.Properties.Appearance.Options.UseFont = true;
        spnAverageDeliveryDays.Properties.Appearance.Options.UseTextOptions = true;
        spnAverageDeliveryDays.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnAverageDeliveryDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnAverageDeliveryDays.Properties.IsFloatValue = false;
        spnAverageDeliveryDays.Properties.MaskSettings.Set("mask", "d");
        spnAverageDeliveryDays.Size = new Size(110, 22);
        spnAverageDeliveryDays.TabIndex = 21;
        // 
        // lblMinimumOrderAmount
        // 
        lblMinimumOrderAmount.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumOrderAmount.Appearance.Options.UseFont = true;
        lblMinimumOrderAmount.Location = new Point(456, 61);
        lblMinimumOrderAmount.Name = "lblMinimumOrderAmount";
        lblMinimumOrderAmount.Size = new Size(136, 15);
        lblMinimumOrderAmount.TabIndex = 22;
        lblMinimumOrderAmount.Text = "Monto Mínimo de Orden:";
        // 
        // spnMinimumOrderAmount
        // 
        spnMinimumOrderAmount.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnMinimumOrderAmount.Location = new Point(626, 58);
        spnMinimumOrderAmount.Name = "spnMinimumOrderAmount";
        spnMinimumOrderAmount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMinimumOrderAmount.Properties.Appearance.Options.UseFont = true;
        spnMinimumOrderAmount.Properties.Appearance.Options.UseTextOptions = true;
        spnMinimumOrderAmount.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnMinimumOrderAmount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMinimumOrderAmount.Properties.DisplayFormat.FormatString = "n2";
        spnMinimumOrderAmount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnMinimumOrderAmount.Properties.MaskSettings.Set("mask", "n2");
        spnMinimumOrderAmount.Size = new Size(110, 22);
        spnMinimumOrderAmount.TabIndex = 23;
        // 
        // lblMinimumOrderQuantity
        // 
        lblMinimumOrderQuantity.Appearance.Font = new Font("Segoe UI", 9F);
        lblMinimumOrderQuantity.Appearance.Options.UseFont = true;
        lblMinimumOrderQuantity.Location = new Point(456, 91);
        lblMinimumOrderQuantity.Name = "lblMinimumOrderQuantity";
        lblMinimumOrderQuantity.Size = new Size(80, 15);
        lblMinimumOrderQuantity.TabIndex = 24;
        lblMinimumOrderQuantity.Text = "Orden Mínima:";
        // 
        // spnMinimumOrderQuantity
        // 
        spnMinimumOrderQuantity.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnMinimumOrderQuantity.Location = new Point(626, 88);
        spnMinimumOrderQuantity.Name = "spnMinimumOrderQuantity";
        spnMinimumOrderQuantity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnMinimumOrderQuantity.Properties.Appearance.Options.UseFont = true;
        spnMinimumOrderQuantity.Properties.Appearance.Options.UseTextOptions = true;
        spnMinimumOrderQuantity.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnMinimumOrderQuantity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnMinimumOrderQuantity.Properties.DisplayFormat.FormatString = "n2";
        spnMinimumOrderQuantity.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        spnMinimumOrderQuantity.Properties.MaskSettings.Set("mask", "n2");
        spnMinimumOrderQuantity.Size = new Size(110, 22);
        spnMinimumOrderQuantity.TabIndex = 25;
        // 
        // lblLeadTimeDays
        // 
        lblLeadTimeDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblLeadTimeDays.Appearance.Options.UseFont = true;
        lblLeadTimeDays.Location = new Point(456, 121);
        lblLeadTimeDays.Name = "lblLeadTimeDays";
        lblLeadTimeDays.Size = new Size(90, 15);
        lblLeadTimeDays.TabIndex = 26;
        lblLeadTimeDays.Text = "Lead Time (días):";
        // 
        // spnLeadTimeDays
        // 
        spnLeadTimeDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnLeadTimeDays.Location = new Point(626, 118);
        spnLeadTimeDays.Name = "spnLeadTimeDays";
        spnLeadTimeDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnLeadTimeDays.Properties.Appearance.Options.UseFont = true;
        spnLeadTimeDays.Properties.Appearance.Options.UseTextOptions = true;
        spnLeadTimeDays.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnLeadTimeDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnLeadTimeDays.Properties.IsFloatValue = false;
        spnLeadTimeDays.Properties.MaskSettings.Set("mask", "d");
        spnLeadTimeDays.Size = new Size(110, 22);
        spnLeadTimeDays.TabIndex = 27;
        // 
        // lblDeliveryToleranceDays
        // 
        lblDeliveryToleranceDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblDeliveryToleranceDays.Appearance.Options.UseFont = true;
        lblDeliveryToleranceDays.Location = new Point(456, 151);
        lblDeliveryToleranceDays.Name = "lblDeliveryToleranceDays";
        lblDeliveryToleranceDays.Size = new Size(149, 15);
        lblDeliveryToleranceDays.TabIndex = 28;
        lblDeliveryToleranceDays.Text = "Tolerancia de Entrega (días):";
        // 
        // spnDeliveryToleranceDays
        // 
        spnDeliveryToleranceDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnDeliveryToleranceDays.Location = new Point(626, 148);
        spnDeliveryToleranceDays.Name = "spnDeliveryToleranceDays";
        spnDeliveryToleranceDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnDeliveryToleranceDays.Properties.Appearance.Options.UseFont = true;
        spnDeliveryToleranceDays.Properties.Appearance.Options.UseTextOptions = true;
        spnDeliveryToleranceDays.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnDeliveryToleranceDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnDeliveryToleranceDays.Properties.IsFloatValue = false;
        spnDeliveryToleranceDays.Properties.MaskSettings.Set("mask", "d");
        spnDeliveryToleranceDays.Size = new Size(110, 22);
        spnDeliveryToleranceDays.TabIndex = 29;
        // 
        // lblRequiresPurchaseOrder
        // 
        lblRequiresPurchaseOrder.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiresPurchaseOrder.Appearance.Options.UseFont = true;
        lblRequiresPurchaseOrder.Location = new Point(456, 201);
        lblRequiresPurchaseOrder.Name = "lblRequiresPurchaseOrder";
        lblRequiresPurchaseOrder.Size = new Size(147, 15);
        lblRequiresPurchaseOrder.TabIndex = 30;
        lblRequiresPurchaseOrder.Text = "Requiere Orden de Compra:";
        // 
        // tglRequiresPurchaseOrder
        // 
        tglRequiresPurchaseOrder.Location = new Point(626, 196);
        tglRequiresPurchaseOrder.Name = "tglRequiresPurchaseOrder";
        tglRequiresPurchaseOrder.Properties.OffText = "";
        tglRequiresPurchaseOrder.Properties.OnText = "";
        tglRequiresPurchaseOrder.Size = new Size(62, 18);
        tglRequiresPurchaseOrder.TabIndex = 31;
        // 
        // lblRequiresPurchaseOrderValue
        // 
        lblRequiresPurchaseOrderValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiresPurchaseOrderValue.Appearance.Options.UseFont = true;
        lblRequiresPurchaseOrderValue.Location = new Point(694, 201);
        lblRequiresPurchaseOrderValue.Name = "lblRequiresPurchaseOrderValue";
        lblRequiresPurchaseOrderValue.Size = new Size(10, 15);
        lblRequiresPurchaseOrderValue.TabIndex = 32;
        lblRequiresPurchaseOrderValue.Text = "Sí";
        // 
        // lblSubjectToEvaluation
        // 
        lblSubjectToEvaluation.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubjectToEvaluation.Appearance.Options.UseFont = true;
        lblSubjectToEvaluation.Location = new Point(456, 231);
        lblSubjectToEvaluation.Name = "lblSubjectToEvaluation";
        lblSubjectToEvaluation.Size = new Size(105, 15);
        lblSubjectToEvaluation.TabIndex = 33;
        lblSubjectToEvaluation.Text = "Sujeto a Evaluación:";
        // 
        // tglSubjectToEvaluation
        // 
        tglSubjectToEvaluation.Location = new Point(626, 226);
        tglSubjectToEvaluation.Name = "tglSubjectToEvaluation";
        tglSubjectToEvaluation.Properties.OffText = "";
        tglSubjectToEvaluation.Properties.OnText = "";
        tglSubjectToEvaluation.Size = new Size(62, 18);
        tglSubjectToEvaluation.TabIndex = 34;
        // 
        // lblSubjectToEvaluationValue
        // 
        lblSubjectToEvaluationValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubjectToEvaluationValue.Appearance.Options.UseFont = true;
        lblSubjectToEvaluationValue.Location = new Point(694, 231);
        lblSubjectToEvaluationValue.Name = "lblSubjectToEvaluationValue";
        lblSubjectToEvaluationValue.Size = new Size(10, 15);
        lblSubjectToEvaluationValue.TabIndex = 35;
        lblSubjectToEvaluationValue.Text = "Sí";
        // 
        // lblActiveForImport
        // 
        lblActiveForImport.Appearance.Font = new Font("Segoe UI", 9F);
        lblActiveForImport.Appearance.Options.UseFont = true;
        lblActiveForImport.Location = new Point(456, 261);
        lblActiveForImport.Name = "lblActiveForImport";
        lblActiveForImport.Size = new Size(131, 15);
        lblActiveForImport.TabIndex = 36;
        lblActiveForImport.Text = "Activo para Importación:";
        // 
        // tglActiveForImport
        // 
        tglActiveForImport.Location = new Point(626, 256);
        tglActiveForImport.Name = "tglActiveForImport";
        tglActiveForImport.Properties.OffText = "";
        tglActiveForImport.Properties.OnText = "";
        tglActiveForImport.Size = new Size(62, 18);
        tglActiveForImport.TabIndex = 37;
        // 
        // lblActiveForImportValue
        // 
        lblActiveForImportValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblActiveForImportValue.Appearance.Options.UseFont = true;
        lblActiveForImportValue.Location = new Point(694, 261);
        lblActiveForImportValue.Name = "lblActiveForImportValue";
        lblActiveForImportValue.Size = new Size(16, 15);
        lblActiveForImportValue.TabIndex = 38;
        lblActiveForImportValue.Text = "No";
        // 
        // lblAllowsUrgentPurchases
        // 
        lblAllowsUrgentPurchases.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowsUrgentPurchases.Appearance.Options.UseFont = true;
        lblAllowsUrgentPurchases.Location = new Point(456, 291);
        lblAllowsUrgentPurchases.Name = "lblAllowsUrgentPurchases";
        lblAllowsUrgentPurchases.Size = new Size(145, 15);
        lblAllowsUrgentPurchases.TabIndex = 39;
        lblAllowsUrgentPurchases.Text = "Permite Compras Urgentes:";
        // 
        // tglAllowsUrgentPurchases
        // 
        tglAllowsUrgentPurchases.Location = new Point(626, 286);
        tglAllowsUrgentPurchases.Name = "tglAllowsUrgentPurchases";
        tglAllowsUrgentPurchases.Properties.OffText = "";
        tglAllowsUrgentPurchases.Properties.OnText = "";
        tglAllowsUrgentPurchases.Size = new Size(62, 18);
        tglAllowsUrgentPurchases.TabIndex = 40;
        // 
        // lblAllowsUrgentPurchasesValue
        // 
        lblAllowsUrgentPurchasesValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowsUrgentPurchasesValue.Appearance.Options.UseFont = true;
        lblAllowsUrgentPurchasesValue.Location = new Point(694, 291);
        lblAllowsUrgentPurchasesValue.Name = "lblAllowsUrgentPurchasesValue";
        lblAllowsUrgentPurchasesValue.Size = new Size(10, 15);
        lblAllowsUrgentPurchasesValue.TabIndex = 41;
        lblAllowsUrgentPurchasesValue.Text = "Sí";
        // 
        // lblPurchaseHistoryTitle
        // 
        lblPurchaseHistoryTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblPurchaseHistoryTitle.Appearance.Options.UseFont = true;
        lblPurchaseHistoryTitle.Location = new Point(806, 31);
        lblPurchaseHistoryTitle.Name = "lblPurchaseHistoryTitle";
        lblPurchaseHistoryTitle.Size = new Size(242, 15);
        lblPurchaseHistoryTitle.TabIndex = 42;
        lblPurchaseHistoryTitle.Text = "Historial de Compras (Últimos 6 documentos)";
        // 
        // grdPurchaseHistory
        // 
        grdPurchaseHistory.Location = new Point(806, 54);
        grdPurchaseHistory.MainView = gvPurchaseHistory;
        grdPurchaseHistory.Name = "grdPurchaseHistory";
        grdPurchaseHistory.Size = new Size(500, 188);
        grdPurchaseHistory.TabIndex = 44;
        grdPurchaseHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvPurchaseHistory });
        // 
        // gvPurchaseHistory
        // 
        gvPurchaseHistory.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvPurchaseHistory.Appearance.HeaderPanel.Options.UseFont = true;
        gvPurchaseHistory.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvPurchaseHistory.Appearance.Row.Options.UseFont = true;
        gvPurchaseHistory.Columns.AddRange(new GridColumn[] { colPurchaseDate, colPurchaseDocumentNumber, colPurchaseAmount, colPurchaseCurrency, colPurchaseAverageDeliveryDays });
        gvPurchaseHistory.GridControl = grdPurchaseHistory;
        gvPurchaseHistory.Name = "gvPurchaseHistory";
        gvPurchaseHistory.OptionsBehavior.Editable = false;
        gvPurchaseHistory.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvPurchaseHistory.OptionsView.ShowGroupPanel = false;
        // 
        // colPurchaseDate
        // 
        colPurchaseDate.Caption = "Última compra";
        colPurchaseDate.DisplayFormat.FormatString = "dd/MM/yyyy";
        colPurchaseDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        colPurchaseDate.FieldName = "PurchaseDate";
        colPurchaseDate.Name = "colPurchaseDate";
        colPurchaseDate.Visible = true;
        colPurchaseDate.VisibleIndex = 0;
        // 
        // colPurchaseDocumentNumber
        // 
        colPurchaseDocumentNumber.Caption = "N° Documento";
        colPurchaseDocumentNumber.FieldName = "DocumentNumber";
        colPurchaseDocumentNumber.Name = "colPurchaseDocumentNumber";
        colPurchaseDocumentNumber.Visible = true;
        colPurchaseDocumentNumber.VisibleIndex = 1;
        // 
        // colPurchaseAmount
        // 
        colPurchaseAmount.Caption = "Monto";
        colPurchaseAmount.DisplayFormat.FormatString = "n2";
        colPurchaseAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        colPurchaseAmount.FieldName = "Amount";
        colPurchaseAmount.Name = "colPurchaseAmount";
        colPurchaseAmount.Visible = true;
        colPurchaseAmount.VisibleIndex = 2;
        // 
        // colPurchaseCurrency
        // 
        colPurchaseCurrency.Caption = "Moneda";
        colPurchaseCurrency.FieldName = "Currency";
        colPurchaseCurrency.Name = "colPurchaseCurrency";
        colPurchaseCurrency.Visible = true;
        colPurchaseCurrency.VisibleIndex = 3;
        // 
        // colPurchaseAverageDeliveryDays
        // 
        colPurchaseAverageDeliveryDays.Caption = "Días entrega promedio";
        colPurchaseAverageDeliveryDays.FieldName = "AverageDeliveryDays";
        colPurchaseAverageDeliveryDays.Name = "colPurchaseAverageDeliveryDays";
        colPurchaseAverageDeliveryDays.Visible = true;
        colPurchaseAverageDeliveryDays.VisibleIndex = 4;
        // 
        // pnlPurchasesLast12Months
        // 
        pnlPurchasesLast12Months.BorderStyle = BorderStyles.Simple;
        pnlPurchasesLast12Months.Controls.Add(lblPurchasesLast12MonthsCaption);
        pnlPurchasesLast12Months.Controls.Add(lblPurchasesLast12MonthsValue);
        pnlPurchasesLast12Months.Location = new Point(806, 256);
        pnlPurchasesLast12Months.Name = "pnlPurchasesLast12Months";
        pnlPurchasesLast12Months.Size = new Size(116, 64);
        pnlPurchasesLast12Months.TabIndex = 45;
        // 
        // lblPurchasesLast12MonthsCaption
        // 
        lblPurchasesLast12MonthsCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchasesLast12MonthsCaption.Appearance.Options.UseFont = true;
        lblPurchasesLast12MonthsCaption.Location = new Point(12, 10);
        lblPurchasesLast12MonthsCaption.Name = "lblPurchasesLast12MonthsCaption";
        lblPurchasesLast12MonthsCaption.Size = new Size(103, 13);
        lblPurchasesLast12MonthsCaption.TabIndex = 0;
        lblPurchasesLast12MonthsCaption.Text = "Compras (12 meses):";
        // 
        // lblPurchasesLast12MonthsValue
        // 
        lblPurchasesLast12MonthsValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblPurchasesLast12MonthsValue.Appearance.Options.UseFont = true;
        lblPurchasesLast12MonthsValue.Location = new Point(12, 36);
        lblPurchasesLast12MonthsValue.Name = "lblPurchasesLast12MonthsValue";
        lblPurchasesLast12MonthsValue.Size = new Size(24, 17);
        lblPurchasesLast12MonthsValue.TabIndex = 1;
        lblPurchasesLast12MonthsValue.Text = "0.00";
        // 
        // pnlAveragePurchase
        // 
        pnlAveragePurchase.BorderStyle = BorderStyles.Simple;
        pnlAveragePurchase.Controls.Add(lblAveragePurchaseCaption);
        pnlAveragePurchase.Controls.Add(lblAveragePurchaseValue);
        pnlAveragePurchase.Location = new Point(932, 256);
        pnlAveragePurchase.Name = "pnlAveragePurchase";
        pnlAveragePurchase.Size = new Size(116, 64);
        pnlAveragePurchase.TabIndex = 46;
        // 
        // lblAveragePurchaseCaption
        // 
        lblAveragePurchaseCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblAveragePurchaseCaption.Appearance.Options.UseFont = true;
        lblAveragePurchaseCaption.Location = new Point(12, 10);
        lblAveragePurchaseCaption.Name = "lblAveragePurchaseCaption";
        lblAveragePurchaseCaption.Size = new Size(96, 13);
        lblAveragePurchaseCaption.TabIndex = 0;
        lblAveragePurchaseCaption.Text = "Compra promedio:";
        // 
        // lblAveragePurchaseValue
        // 
        lblAveragePurchaseValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblAveragePurchaseValue.Appearance.Options.UseFont = true;
        lblAveragePurchaseValue.Location = new Point(12, 36);
        lblAveragePurchaseValue.Name = "lblAveragePurchaseValue";
        lblAveragePurchaseValue.Size = new Size(24, 17);
        lblAveragePurchaseValue.TabIndex = 1;
        lblAveragePurchaseValue.Text = "0.00";
        // 
        // pnlAverageDelivery12Months
        // 
        pnlAverageDelivery12Months.BorderStyle = BorderStyles.Simple;
        pnlAverageDelivery12Months.Controls.Add(lblAverageDelivery12MonthsCaption);
        pnlAverageDelivery12Months.Controls.Add(lblAverageDelivery12MonthsValue);
        pnlAverageDelivery12Months.Location = new Point(1058, 256);
        pnlAverageDelivery12Months.Name = "pnlAverageDelivery12Months";
        pnlAverageDelivery12Months.Size = new Size(120, 64);
        pnlAverageDelivery12Months.TabIndex = 47;
        // 
        // lblAverageDelivery12MonthsCaption
        // 
        lblAverageDelivery12MonthsCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblAverageDelivery12MonthsCaption.Appearance.Options.UseFont = true;
        lblAverageDelivery12MonthsCaption.Location = new Point(10, 10);
        lblAverageDelivery12MonthsCaption.Name = "lblAverageDelivery12MonthsCaption";
        lblAverageDelivery12MonthsCaption.Size = new Size(101, 13);
        lblAverageDelivery12MonthsCaption.TabIndex = 0;
        lblAverageDelivery12MonthsCaption.Text = "Días entrega prom.:";
        // 
        // lblAverageDelivery12MonthsValue
        // 
        lblAverageDelivery12MonthsValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblAverageDelivery12MonthsValue.Appearance.Options.UseFont = true;
        lblAverageDelivery12MonthsValue.Location = new Point(10, 36);
        lblAverageDelivery12MonthsValue.Name = "lblAverageDelivery12MonthsValue";
        lblAverageDelivery12MonthsValue.Size = new Size(7, 17);
        lblAverageDelivery12MonthsValue.TabIndex = 1;
        lblAverageDelivery12MonthsValue.Text = "0";
        // 
        // pnlPurchaseOrdersLast12Months
        // 
        pnlPurchaseOrdersLast12Months.BorderStyle = BorderStyles.Simple;
        pnlPurchaseOrdersLast12Months.Controls.Add(lblPurchaseOrdersLast12MonthsCaption);
        pnlPurchaseOrdersLast12Months.Controls.Add(lblPurchaseOrdersLast12MonthsValue);
        pnlPurchaseOrdersLast12Months.Location = new Point(1188, 256);
        pnlPurchaseOrdersLast12Months.Name = "pnlPurchaseOrdersLast12Months";
        pnlPurchaseOrdersLast12Months.Size = new Size(118, 64);
        pnlPurchaseOrdersLast12Months.TabIndex = 48;
        // 
        // lblPurchaseOrdersLast12MonthsCaption
        // 
        lblPurchaseOrdersLast12MonthsCaption.Appearance.Font = new Font("Segoe UI", 8.5F);
        lblPurchaseOrdersLast12MonthsCaption.Appearance.Options.UseFont = true;
        lblPurchaseOrdersLast12MonthsCaption.Location = new Point(12, 10);
        lblPurchaseOrdersLast12MonthsCaption.Name = "lblPurchaseOrdersLast12MonthsCaption";
        lblPurchaseOrdersLast12MonthsCaption.Size = new Size(102, 13);
        lblPurchaseOrdersLast12MonthsCaption.TabIndex = 0;
        lblPurchaseOrdersLast12MonthsCaption.Text = "Órdenes (12 meses):";
        // 
        // lblPurchaseOrdersLast12MonthsValue
        // 
        lblPurchaseOrdersLast12MonthsValue.Appearance.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblPurchaseOrdersLast12MonthsValue.Appearance.Options.UseFont = true;
        lblPurchaseOrdersLast12MonthsValue.Location = new Point(12, 36);
        lblPurchaseOrdersLast12MonthsValue.Name = "lblPurchaseOrdersLast12MonthsValue";
        lblPurchaseOrdersLast12MonthsValue.Size = new Size(7, 17);
        lblPurchaseOrdersLast12MonthsValue.TabIndex = 1;
        lblPurchaseOrdersLast12MonthsValue.Text = "0";
        // 
        // tabBanks
        // 
        tabBanks.Controls.Add(pnlBanksContent);
        tabBanks.Name = "tabBanks";
        tabBanks.Size = new Size(1342, 404);
        tabBanks.Text = "Bancos";
        // 
        // pnlBanksContent
        // 
        pnlBanksContent.BorderStyle = BorderStyles.Simple;
        pnlBanksContent.Controls.Add(pnlBanksActions);
        pnlBanksContent.Controls.Add(grdBankAccounts);
        pnlBanksContent.Controls.Add(lblBankAccountsTotal);
        pnlBanksContent.Dock = DockStyle.Fill;
        pnlBanksContent.Location = new Point(0, 0);
        pnlBanksContent.Name = "pnlBanksContent";
        pnlBanksContent.Size = new Size(1342, 404);
        pnlBanksContent.TabIndex = 0;
        // 
        // pnlBanksActions
        // 
        pnlBanksActions.BorderStyle = BorderStyles.NoBorder;
        pnlBanksActions.Controls.Add(btnAddBankAccount);
        pnlBanksActions.Controls.Add(btnEditBankAccount);
        pnlBanksActions.Controls.Add(btnDeleteBankAccount);
        pnlBanksActions.Controls.Add(btnSetDefaultBankAccount);
        pnlBanksActions.Dock = DockStyle.Top;
        pnlBanksActions.Location = new Point(2, 2);
        pnlBanksActions.Name = "pnlBanksActions";
        pnlBanksActions.Size = new Size(1338, 48);
        pnlBanksActions.TabIndex = 0;
        // 
        // btnAddBankAccount
        // 
        btnAddBankAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddBankAccount.Appearance.Options.UseFont = true;
        btnAddBankAccount.Location = new Point(14, 10);
        btnAddBankAccount.Name = "btnAddBankAccount";
        btnAddBankAccount.Size = new Size(88, 28);
        btnAddBankAccount.TabIndex = 0;
        btnAddBankAccount.Text = "Agregar";
        // 
        // btnEditBankAccount
        // 
        btnEditBankAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditBankAccount.Appearance.Options.UseFont = true;
        btnEditBankAccount.Location = new Point(112, 10);
        btnEditBankAccount.Name = "btnEditBankAccount";
        btnEditBankAccount.Size = new Size(88, 28);
        btnEditBankAccount.TabIndex = 1;
        btnEditBankAccount.Text = "Editar";
        // 
        // btnDeleteBankAccount
        // 
        btnDeleteBankAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteBankAccount.Appearance.Options.UseFont = true;
        btnDeleteBankAccount.Location = new Point(210, 10);
        btnDeleteBankAccount.Name = "btnDeleteBankAccount";
        btnDeleteBankAccount.Size = new Size(88, 28);
        btnDeleteBankAccount.TabIndex = 2;
        btnDeleteBankAccount.Text = "Eliminar";
        // 
        // btnSetDefaultBankAccount
        // 
        btnSetDefaultBankAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetDefaultBankAccount.Appearance.Options.UseFont = true;
        btnSetDefaultBankAccount.Location = new Point(308, 10);
        btnSetDefaultBankAccount.Name = "btnSetDefaultBankAccount";
        btnSetDefaultBankAccount.Size = new Size(116, 28);
        btnSetDefaultBankAccount.TabIndex = 3;
        btnSetDefaultBankAccount.Text = "Predeterminada";
        // 
        // grdBankAccounts
        // 
        grdBankAccounts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdBankAccounts.Location = new Point(12, 58);
        grdBankAccounts.MainView = gvBankAccounts;
        grdBankAccounts.Name = "grdBankAccounts";
        grdBankAccounts.Size = new Size(1316, 304);
        grdBankAccounts.TabIndex = 1;
        grdBankAccounts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvBankAccounts });
        // 
        // gvBankAccounts
        // 
        gvBankAccounts.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvBankAccounts.Appearance.HeaderPanel.Options.UseFont = true;
        gvBankAccounts.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvBankAccounts.Appearance.Row.Options.UseFont = true;
        gvBankAccounts.Columns.AddRange(new GridColumn[] { colBankName, colBankAccountType, colBankAccountNumber, colBankCurrency, colBankSwiftBic, colBankCciIban, colBankAccountHolder, colBankIsDefault, colBankIsActive });
        gvBankAccounts.GridControl = grdBankAccounts;
        gvBankAccounts.Name = "gvBankAccounts";
        gvBankAccounts.OptionsBehavior.Editable = false;
        gvBankAccounts.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvBankAccounts.OptionsView.ShowGroupPanel = false;
        // 
        // colBankName
        // 
        colBankName.Caption = "Banco";
        colBankName.FieldName = "BankName";
        colBankName.Name = "colBankName";
        colBankName.Visible = true;
        colBankName.VisibleIndex = 0;
        colBankName.Width = 220;
        // 
        // colBankAccountType
        // 
        colBankAccountType.Caption = "Tipo Cuenta";
        colBankAccountType.FieldName = "AccountType";
        colBankAccountType.Name = "colBankAccountType";
        colBankAccountType.Visible = true;
        colBankAccountType.VisibleIndex = 1;
        colBankAccountType.Width = 140;
        // 
        // colBankAccountNumber
        // 
        colBankAccountNumber.Caption = "N° Cuenta";
        colBankAccountNumber.FieldName = "AccountNumber";
        colBankAccountNumber.Name = "colBankAccountNumber";
        colBankAccountNumber.Visible = true;
        colBankAccountNumber.VisibleIndex = 2;
        colBankAccountNumber.Width = 170;
        // 
        // colBankCurrency
        // 
        colBankCurrency.Caption = "Moneda";
        colBankCurrency.FieldName = "Currency";
        colBankCurrency.Name = "colBankCurrency";
        colBankCurrency.Visible = true;
        colBankCurrency.VisibleIndex = 3;
        colBankCurrency.Width = 90;
        // 
        // colBankSwiftBic
        // 
        colBankSwiftBic.Caption = "SWIFT";
        colBankSwiftBic.FieldName = "SwiftBic";
        colBankSwiftBic.Name = "colBankSwiftBic";
        colBankSwiftBic.Visible = true;
        colBankSwiftBic.VisibleIndex = 4;
        colBankSwiftBic.Width = 110;
        // 
        // colBankCciIban
        // 
        colBankCciIban.Caption = "CCI / IBAN";
        colBankCciIban.FieldName = "CciIban";
        colBankCciIban.Name = "colBankCciIban";
        colBankCciIban.Visible = true;
        colBankCciIban.VisibleIndex = 5;
        colBankCciIban.Width = 190;
        // 
        // colBankAccountHolder
        // 
        colBankAccountHolder.Caption = "Titular";
        colBankAccountHolder.FieldName = "AccountHolder";
        colBankAccountHolder.Name = "colBankAccountHolder";
        colBankAccountHolder.Visible = true;
        colBankAccountHolder.VisibleIndex = 6;
        colBankAccountHolder.Width = 130;
        // 
        // colBankIsDefault
        // 
        colBankIsDefault.Caption = "Predeterminada";
        colBankIsDefault.FieldName = "IsDefault";
        colBankIsDefault.Name = "colBankIsDefault";
        colBankIsDefault.Visible = true;
        colBankIsDefault.VisibleIndex = 7;
        colBankIsDefault.Width = 110;
        // 
        // colBankIsActive
        // 
        colBankIsActive.Caption = "Activa";
        colBankIsActive.FieldName = "IsActive";
        colBankIsActive.Name = "colBankIsActive";
        colBankIsActive.Visible = true;
        colBankIsActive.VisibleIndex = 8;
        colBankIsActive.Width = 80;
        // 
        // lblBankAccountsTotal
        // 
        lblBankAccountsTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        lblBankAccountsTotal.Appearance.Font = new Font("Segoe UI", 9F);
        lblBankAccountsTotal.Appearance.Options.UseFont = true;
        lblBankAccountsTotal.Location = new Point(14, 372);
        lblBankAccountsTotal.Name = "lblBankAccountsTotal";
        lblBankAccountsTotal.Size = new Size(103, 15);
        lblBankAccountsTotal.TabIndex = 2;
        lblBankAccountsTotal.Text = "Total de registros: 0";
        // 
        // tabWithholdings
        // 
        tabWithholdings.Controls.Add(pnlWithholdingsContent);
        tabWithholdings.Name = "tabWithholdings";
        tabWithholdings.Size = new Size(1342, 404);
        tabWithholdings.Text = "Retenciones";
        // 
        // pnlWithholdingsContent
        // 
        pnlWithholdingsContent.BorderStyle = BorderStyles.Simple;
        pnlWithholdingsContent.Controls.Add(pnlWithholdingsGeneral);
        pnlWithholdingsContent.Controls.Add(pnlWithholdingsActions);
        pnlWithholdingsContent.Controls.Add(grdWithholdings);
        pnlWithholdingsContent.Dock = DockStyle.Fill;
        pnlWithholdingsContent.Location = new Point(0, 0);
        pnlWithholdingsContent.Name = "pnlWithholdingsContent";
        pnlWithholdingsContent.Size = new Size(1342, 404);
        pnlWithholdingsContent.TabIndex = 0;
        // 
        // pnlWithholdingsGeneral
        // 
        pnlWithholdingsGeneral.BorderStyle = BorderStyles.NoBorder;
        pnlWithholdingsGeneral.Controls.Add(lblWithholdingAgent);
        pnlWithholdingsGeneral.Controls.Add(tglWithholdingAgent);
        pnlWithholdingsGeneral.Controls.Add(lblWithholdingAgentValue);
        pnlWithholdingsGeneral.Controls.Add(lblGeneralWithholdingType);
        pnlWithholdingsGeneral.Controls.Add(lueGeneralWithholdingType);
        pnlWithholdingsGeneral.Controls.Add(lblBaseWithholdingPercent);
        pnlWithholdingsGeneral.Controls.Add(spnBaseWithholdingPercent);
        pnlWithholdingsGeneral.Controls.Add(lblWithholdingEffectiveDate);
        pnlWithholdingsGeneral.Controls.Add(dteWithholdingEffectiveDate);
        pnlWithholdingsGeneral.Controls.Add(lblWithholdingResolutionNumber);
        pnlWithholdingsGeneral.Controls.Add(txtWithholdingResolutionNumber);
        pnlWithholdingsGeneral.Controls.Add(lblWithholdsVat);
        pnlWithholdingsGeneral.Controls.Add(tglWithholdsVat);
        pnlWithholdingsGeneral.Controls.Add(lblWithholdsVatValue);
        pnlWithholdingsGeneral.Controls.Add(lblWithholdsIncomeTax);
        pnlWithholdingsGeneral.Controls.Add(tglWithholdsIncomeTax);
        pnlWithholdingsGeneral.Controls.Add(lblWithholdsIncomeTaxValue);
        pnlWithholdingsGeneral.Controls.Add(lblIssuesElectronicReceipts);
        pnlWithholdingsGeneral.Controls.Add(tglIssuesElectronicReceipts);
        pnlWithholdingsGeneral.Controls.Add(lblIssuesElectronicReceiptsValue);
        pnlWithholdingsGeneral.Controls.Add(lblSubjectToPerception);
        pnlWithholdingsGeneral.Controls.Add(tglSubjectToPerception);
        pnlWithholdingsGeneral.Controls.Add(lblSubjectToPerceptionValue);
        pnlWithholdingsGeneral.Dock = DockStyle.Top;
        pnlWithholdingsGeneral.Location = new Point(2, 2);
        pnlWithholdingsGeneral.Name = "pnlWithholdingsGeneral";
        pnlWithholdingsGeneral.Size = new Size(1338, 116);
        pnlWithholdingsGeneral.TabIndex = 0;
        // 
        // lblWithholdingAgent
        // 
        lblWithholdingAgent.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdingAgent.Appearance.Options.UseFont = true;
        lblWithholdingAgent.Location = new Point(18, 22);
        lblWithholdingAgent.Name = "lblWithholdingAgent";
        lblWithholdingAgent.Size = new Size(113, 15);
        lblWithholdingAgent.TabIndex = 0;
        lblWithholdingAgent.Text = "Agente de Retención:";
        // 
        // tglWithholdingAgent
        // 
        tglWithholdingAgent.Location = new Point(218, 17);
        tglWithholdingAgent.Name = "tglWithholdingAgent";
        tglWithholdingAgent.Properties.OffText = "";
        tglWithholdingAgent.Properties.OnText = "";
        tglWithholdingAgent.Size = new Size(50, 18);
        tglWithholdingAgent.TabIndex = 1;
        // 
        // lblWithholdingAgentValue
        // 
        lblWithholdingAgentValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdingAgentValue.Appearance.Options.UseFont = true;
        lblWithholdingAgentValue.Location = new Point(274, 22);
        lblWithholdingAgentValue.Name = "lblWithholdingAgentValue";
        lblWithholdingAgentValue.Size = new Size(10, 15);
        lblWithholdingAgentValue.TabIndex = 2;
        lblWithholdingAgentValue.Text = "Sí";
        // 
        // lblGeneralWithholdingType
        // 
        lblGeneralWithholdingType.Appearance.Font = new Font("Segoe UI", 9F);
        lblGeneralWithholdingType.Appearance.Options.UseFont = true;
        lblGeneralWithholdingType.Location = new Point(18, 50);
        lblGeneralWithholdingType.Name = "lblGeneralWithholdingType";
        lblGeneralWithholdingType.Size = new Size(99, 15);
        lblGeneralWithholdingType.TabIndex = 3;
        lblGeneralWithholdingType.Text = "Tipo de Retención:";
        // 
        // lueGeneralWithholdingType
        // 
        lueGeneralWithholdingType.Location = new Point(218, 47);
        lueGeneralWithholdingType.Name = "lueGeneralWithholdingType";
        lueGeneralWithholdingType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueGeneralWithholdingType.Properties.Appearance.Options.UseFont = true;
        lueGeneralWithholdingType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueGeneralWithholdingType.Properties.NullText = "";
        lueGeneralWithholdingType.Size = new Size(210, 22);
        lueGeneralWithholdingType.TabIndex = 4;
        // 
        // lblBaseWithholdingPercent
        // 
        lblBaseWithholdingPercent.Appearance.Font = new Font("Segoe UI", 9F);
        lblBaseWithholdingPercent.Appearance.Options.UseFont = true;
        lblBaseWithholdingPercent.Location = new Point(18, 78);
        lblBaseWithholdingPercent.Name = "lblBaseWithholdingPercent";
        lblBaseWithholdingPercent.Size = new Size(179, 15);
        lblBaseWithholdingPercent.TabIndex = 5;
        lblBaseWithholdingPercent.Text = "Porcentaje Base de Retención (%):";
        // 
        // spnBaseWithholdingPercent
        // 
        spnBaseWithholdingPercent.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnBaseWithholdingPercent.Location = new Point(218, 75);
        spnBaseWithholdingPercent.Name = "spnBaseWithholdingPercent";
        spnBaseWithholdingPercent.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnBaseWithholdingPercent.Properties.Appearance.Options.UseFont = true;
        spnBaseWithholdingPercent.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnBaseWithholdingPercent.Properties.MaskSettings.Set("mask", "n2");
        spnBaseWithholdingPercent.Size = new Size(110, 22);
        spnBaseWithholdingPercent.TabIndex = 6;
        // 
        // lblWithholdingEffectiveDate
        // 
        lblWithholdingEffectiveDate.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdingEffectiveDate.Appearance.Options.UseFont = true;
        lblWithholdingEffectiveDate.Location = new Point(456, 22);
        lblWithholdingEffectiveDate.Name = "lblWithholdingEffectiveDate";
        lblWithholdingEffectiveDate.Size = new Size(98, 15);
        lblWithholdingEffectiveDate.TabIndex = 7;
        lblWithholdingEffectiveDate.Text = "Fecha de Vigencia:";
        // 
        // dteWithholdingEffectiveDate
        // 
        dteWithholdingEffectiveDate.EditValue = new DateTime(2026, 6, 6, 0, 0, 0, 0);
        dteWithholdingEffectiveDate.Location = new Point(636, 19);
        dteWithholdingEffectiveDate.Name = "dteWithholdingEffectiveDate";
        dteWithholdingEffectiveDate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dteWithholdingEffectiveDate.Properties.Appearance.Options.UseFont = true;
        dteWithholdingEffectiveDate.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteWithholdingEffectiveDate.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dteWithholdingEffectiveDate.Size = new Size(140, 22);
        dteWithholdingEffectiveDate.TabIndex = 8;
        // 
        // lblWithholdingResolutionNumber
        // 
        lblWithholdingResolutionNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdingResolutionNumber.Appearance.Options.UseFont = true;
        lblWithholdingResolutionNumber.Location = new Point(456, 50);
        lblWithholdingResolutionNumber.Name = "lblWithholdingResolutionNumber";
        lblWithholdingResolutionNumber.Size = new Size(124, 15);
        lblWithholdingResolutionNumber.TabIndex = 9;
        lblWithholdingResolutionNumber.Text = "Número de Resolución:";
        // 
        // txtWithholdingResolutionNumber
        // 
        txtWithholdingResolutionNumber.Location = new Point(636, 47);
        txtWithholdingResolutionNumber.Name = "txtWithholdingResolutionNumber";
        txtWithholdingResolutionNumber.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtWithholdingResolutionNumber.Properties.Appearance.Options.UseFont = true;
        txtWithholdingResolutionNumber.Size = new Size(220, 22);
        txtWithholdingResolutionNumber.TabIndex = 10;
        // 
        // lblWithholdsVat
        // 
        lblWithholdsVat.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdsVat.Appearance.Options.UseFont = true;
        lblWithholdsVat.Location = new Point(896, 22);
        lblWithholdsVat.Name = "lblWithholdsVat";
        lblWithholdsVat.Size = new Size(63, 15);
        lblWithholdsVat.TabIndex = 11;
        lblWithholdsVat.Text = "Retiene IVA:";
        // 
        // tglWithholdsVat
        // 
        tglWithholdsVat.Location = new Point(1070, 17);
        tglWithholdsVat.Name = "tglWithholdsVat";
        tglWithholdsVat.Properties.OffText = "";
        tglWithholdsVat.Properties.OnText = "";
        tglWithholdsVat.Size = new Size(50, 18);
        tglWithholdsVat.TabIndex = 12;
        // 
        // lblWithholdsVatValue
        // 
        lblWithholdsVatValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdsVatValue.Appearance.Options.UseFont = true;
        lblWithholdsVatValue.Location = new Point(1126, 22);
        lblWithholdsVatValue.Name = "lblWithholdsVatValue";
        lblWithholdsVatValue.Size = new Size(10, 15);
        lblWithholdsVatValue.TabIndex = 13;
        lblWithholdsVatValue.Text = "Sí";
        // 
        // lblWithholdsIncomeTax
        // 
        lblWithholdsIncomeTax.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdsIncomeTax.Appearance.Options.UseFont = true;
        lblWithholdsIncomeTax.Location = new Point(896, 50);
        lblWithholdsIncomeTax.Name = "lblWithholdsIncomeTax";
        lblWithholdsIncomeTax.Size = new Size(75, 15);
        lblWithholdsIncomeTax.TabIndex = 14;
        lblWithholdsIncomeTax.Text = "Retiene Renta:";
        // 
        // tglWithholdsIncomeTax
        // 
        tglWithholdsIncomeTax.Location = new Point(1070, 45);
        tglWithholdsIncomeTax.Name = "tglWithholdsIncomeTax";
        tglWithholdsIncomeTax.Properties.OffText = "";
        tglWithholdsIncomeTax.Properties.OnText = "";
        tglWithholdsIncomeTax.Size = new Size(50, 18);
        tglWithholdsIncomeTax.TabIndex = 15;
        // 
        // lblWithholdsIncomeTaxValue
        // 
        lblWithholdsIncomeTaxValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdsIncomeTaxValue.Appearance.Options.UseFont = true;
        lblWithholdsIncomeTaxValue.Location = new Point(1126, 50);
        lblWithholdsIncomeTaxValue.Name = "lblWithholdsIncomeTaxValue";
        lblWithholdsIncomeTaxValue.Size = new Size(10, 15);
        lblWithholdsIncomeTaxValue.TabIndex = 16;
        lblWithholdsIncomeTaxValue.Text = "Sí";
        // 
        // lblIssuesElectronicReceipts
        // 
        lblIssuesElectronicReceipts.Appearance.Font = new Font("Segoe UI", 9F);
        lblIssuesElectronicReceipts.Appearance.Options.UseFont = true;
        lblIssuesElectronicReceipts.Location = new Point(896, 78);
        lblIssuesElectronicReceipts.Name = "lblIssuesElectronicReceipts";
        lblIssuesElectronicReceipts.Size = new Size(182, 15);
        lblIssuesElectronicReceipts.TabIndex = 17;
        lblIssuesElectronicReceipts.Text = "Emite Comprobantes Electrónicos:";
        // 
        // tglIssuesElectronicReceipts
        // 
        tglIssuesElectronicReceipts.Location = new Point(1070, 73);
        tglIssuesElectronicReceipts.Name = "tglIssuesElectronicReceipts";
        tglIssuesElectronicReceipts.Properties.OffText = "";
        tglIssuesElectronicReceipts.Properties.OnText = "";
        tglIssuesElectronicReceipts.Size = new Size(50, 18);
        tglIssuesElectronicReceipts.TabIndex = 18;
        // 
        // lblIssuesElectronicReceiptsValue
        // 
        lblIssuesElectronicReceiptsValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblIssuesElectronicReceiptsValue.Appearance.Options.UseFont = true;
        lblIssuesElectronicReceiptsValue.Location = new Point(1126, 78);
        lblIssuesElectronicReceiptsValue.Name = "lblIssuesElectronicReceiptsValue";
        lblIssuesElectronicReceiptsValue.Size = new Size(10, 15);
        lblIssuesElectronicReceiptsValue.TabIndex = 19;
        lblIssuesElectronicReceiptsValue.Text = "Sí";
        // 
        // lblSubjectToPerception
        // 
        lblSubjectToPerception.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubjectToPerception.Appearance.Options.UseFont = true;
        lblSubjectToPerception.Location = new Point(456, 78);
        lblSubjectToPerception.Name = "lblSubjectToPerception";
        lblSubjectToPerception.Size = new Size(107, 15);
        lblSubjectToPerception.TabIndex = 20;
        lblSubjectToPerception.Text = "Sujeto a Percepción:";
        // 
        // tglSubjectToPerception
        // 
        tglSubjectToPerception.Location = new Point(636, 73);
        tglSubjectToPerception.Name = "tglSubjectToPerception";
        tglSubjectToPerception.Properties.OffText = "";
        tglSubjectToPerception.Properties.OnText = "";
        tglSubjectToPerception.Size = new Size(50, 18);
        tglSubjectToPerception.TabIndex = 21;
        // 
        // lblSubjectToPerceptionValue
        // 
        lblSubjectToPerceptionValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubjectToPerceptionValue.Appearance.Options.UseFont = true;
        lblSubjectToPerceptionValue.Location = new Point(692, 78);
        lblSubjectToPerceptionValue.Name = "lblSubjectToPerceptionValue";
        lblSubjectToPerceptionValue.Size = new Size(16, 15);
        lblSubjectToPerceptionValue.TabIndex = 22;
        lblSubjectToPerceptionValue.Text = "No";
        // 
        // pnlWithholdingsActions
        // 
        pnlWithholdingsActions.BorderStyle = BorderStyles.NoBorder;
        pnlWithholdingsActions.Controls.Add(btnAddWithholding);
        pnlWithholdingsActions.Controls.Add(btnEditWithholding);
        pnlWithholdingsActions.Controls.Add(btnDeleteWithholding);
        pnlWithholdingsActions.Controls.Add(btnSetDefaultWithholding);
        pnlWithholdingsActions.Location = new Point(2, 118);
        pnlWithholdingsActions.Name = "pnlWithholdingsActions";
        pnlWithholdingsActions.Size = new Size(1338, 48);
        pnlWithholdingsActions.TabIndex = 1;
        // 
        // btnAddWithholding
        // 
        btnAddWithholding.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddWithholding.Appearance.Options.UseFont = true;
        btnAddWithholding.Location = new Point(14, 10);
        btnAddWithholding.Name = "btnAddWithholding";
        btnAddWithholding.Size = new Size(88, 28);
        btnAddWithholding.TabIndex = 0;
        btnAddWithholding.Text = "Agregar";
        // 
        // btnEditWithholding
        // 
        btnEditWithholding.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditWithholding.Appearance.Options.UseFont = true;
        btnEditWithholding.Location = new Point(112, 10);
        btnEditWithholding.Name = "btnEditWithholding";
        btnEditWithholding.Size = new Size(88, 28);
        btnEditWithholding.TabIndex = 1;
        btnEditWithholding.Text = "Editar";
        // 
        // btnDeleteWithholding
        // 
        btnDeleteWithholding.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteWithholding.Appearance.Options.UseFont = true;
        btnDeleteWithholding.Location = new Point(210, 10);
        btnDeleteWithholding.Name = "btnDeleteWithholding";
        btnDeleteWithholding.Size = new Size(88, 28);
        btnDeleteWithholding.TabIndex = 2;
        btnDeleteWithholding.Text = "Eliminar";
        // 
        // btnSetDefaultWithholding
        // 
        btnSetDefaultWithholding.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetDefaultWithholding.Appearance.Options.UseFont = true;
        btnSetDefaultWithholding.Location = new Point(308, 10);
        btnSetDefaultWithholding.Name = "btnSetDefaultWithholding";
        btnSetDefaultWithholding.Size = new Size(116, 28);
        btnSetDefaultWithholding.TabIndex = 3;
        btnSetDefaultWithholding.Text = "Predeterminar";
        // 
        // grdWithholdings
        // 
        grdWithholdings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdWithholdings.Location = new Point(14, 174);
        grdWithholdings.MainView = gvWithholdings;
        grdWithholdings.Name = "grdWithholdings";
        grdWithholdings.Size = new Size(1314, 212);
        grdWithholdings.TabIndex = 2;
        grdWithholdings.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvWithholdings });
        // 
        // gvWithholdings
        // 
        gvWithholdings.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvWithholdings.Appearance.HeaderPanel.Options.UseFont = true;
        gvWithholdings.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvWithholdings.Appearance.Row.Options.UseFont = true;
        gvWithholdings.Columns.AddRange(new GridColumn[] { colWithholdingDocument, colWithholdingType, colWithholdingValidity, colWithholdingIsDefault, colWithholdingStatus });
        gvWithholdings.GridControl = grdWithholdings;
        gvWithholdings.Name = "gvWithholdings";
        gvWithholdings.OptionsBehavior.Editable = false;
        gvWithholdings.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvWithholdings.OptionsView.ShowGroupPanel = false;
        // 
        // colWithholdingDocument
        // 
        colWithholdingDocument.Caption = "Documento";
        colWithholdingDocument.FieldName = "Document";
        colWithholdingDocument.Name = "colWithholdingDocument";
        colWithholdingDocument.Visible = true;
        colWithholdingDocument.VisibleIndex = 0;
        colWithholdingDocument.Width = 220;
        // 
        // colWithholdingType
        // 
        colWithholdingType.Caption = "Tipo";
        colWithholdingType.FieldName = "Type";
        colWithholdingType.Name = "colWithholdingType";
        colWithholdingType.Visible = true;
        colWithholdingType.VisibleIndex = 1;
        colWithholdingType.Width = 200;
        // 
        // colWithholdingValidity
        // 
        colWithholdingValidity.Caption = "Vigencia";
        colWithholdingValidity.FieldName = "ValidityText";
        colWithholdingValidity.Name = "colWithholdingValidity";
        colWithholdingValidity.Visible = true;
        colWithholdingValidity.VisibleIndex = 2;
        colWithholdingValidity.Width = 260;
        // 
        // colWithholdingIsDefault
        // 
        colWithholdingIsDefault.Caption = "Predeterminado";
        colWithholdingIsDefault.FieldName = "IsDefault";
        colWithholdingIsDefault.Name = "colWithholdingIsDefault";
        colWithholdingIsDefault.Visible = true;
        colWithholdingIsDefault.VisibleIndex = 3;
        colWithholdingIsDefault.Width = 140;
        // 
        // colWithholdingStatus
        // 
        colWithholdingStatus.Caption = "Estado";
        colWithholdingStatus.FieldName = "Status";
        colWithholdingStatus.Name = "colWithholdingStatus";
        colWithholdingStatus.Visible = true;
        colWithholdingStatus.VisibleIndex = 4;
        colWithholdingStatus.Width = 120;
        // 
        // tabAccounting
        // 
        tabAccounting.Controls.Add(pnlAccountingContent);
        tabAccounting.Name = "tabAccounting";
        tabAccounting.Size = new Size(1342, 404);
        tabAccounting.Text = "Contabilidad";
        // 
        // pnlAccountingContent
        // 
        pnlAccountingContent.BorderStyle = BorderStyles.Simple;
        pnlAccountingContent.Controls.Add(pnlAccountingGeneral);
        pnlAccountingContent.Controls.Add(pnlAccountingActions);
        pnlAccountingContent.Controls.Add(grdAccountingAccounts);
        pnlAccountingContent.Dock = DockStyle.Fill;
        pnlAccountingContent.Location = new Point(0, 0);
        pnlAccountingContent.Name = "pnlAccountingContent";
        pnlAccountingContent.Size = new Size(1342, 404);
        pnlAccountingContent.TabIndex = 0;
        // 
        // pnlAccountingGeneral
        // 
        pnlAccountingGeneral.BorderStyle = BorderStyles.NoBorder;
        pnlAccountingGeneral.Controls.Add(lblDefaultProject);
        pnlAccountingGeneral.Controls.Add(lueDefaultProject);
        pnlAccountingGeneral.Controls.Add(lblFiscalCondition);
        pnlAccountingGeneral.Controls.Add(lueFiscalCondition);
        pnlAccountingGeneral.Controls.Add(lblThirdPartyType);
        pnlAccountingGeneral.Controls.Add(lueThirdPartyType);
        pnlAccountingGeneral.Controls.Add(lblAutomaticAccounting);
        pnlAccountingGeneral.Controls.Add(tglAutomaticAccounting);
        pnlAccountingGeneral.Controls.Add(lblAutomaticAccountingValue);
        pnlAccountingGeneral.Controls.Add(lblRequiresReconciliation);
        pnlAccountingGeneral.Controls.Add(tglRequiresReconciliation);
        pnlAccountingGeneral.Controls.Add(lblRequiresReconciliationValue);
        pnlAccountingGeneral.Controls.Add(lblHandlesAdvances);
        pnlAccountingGeneral.Controls.Add(tglHandlesAdvances);
        pnlAccountingGeneral.Controls.Add(lblHandlesAdvancesValue);
        pnlAccountingGeneral.Controls.Add(lblAccountingBlocked);
        pnlAccountingGeneral.Controls.Add(tglAccountingBlocked);
        pnlAccountingGeneral.Controls.Add(lblAccountingBlockedValue);
        pnlAccountingGeneral.Dock = DockStyle.Top;
        pnlAccountingGeneral.Location = new Point(2, 2);
        pnlAccountingGeneral.Name = "pnlAccountingGeneral";
        pnlAccountingGeneral.Size = new Size(1338, 116);
        pnlAccountingGeneral.TabIndex = 0;
        // 
        // lblDefaultProject
        // 
        lblDefaultProject.Appearance.Font = new Font("Segoe UI", 9F);
        lblDefaultProject.Appearance.Options.UseFont = true;
        lblDefaultProject.Location = new Point(18, 22);
        lblDefaultProject.Name = "lblDefaultProject";
        lblDefaultProject.Size = new Size(115, 15);
        lblDefaultProject.TabIndex = 0;
        lblDefaultProject.Text = "Proyecto por Defecto:";
        // 
        // lueDefaultProject
        // 
        lueDefaultProject.Location = new Point(178, 19);
        lueDefaultProject.Name = "lueDefaultProject";
        lueDefaultProject.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueDefaultProject.Properties.Appearance.Options.UseFont = true;
        lueDefaultProject.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueDefaultProject.Properties.NullText = "";
        lueDefaultProject.Size = new Size(230, 22);
        lueDefaultProject.TabIndex = 1;
        // 
        // lblFiscalCondition
        // 
        lblFiscalCondition.Appearance.Font = new Font("Segoe UI", 9F);
        lblFiscalCondition.Appearance.Options.UseFont = true;
        lblFiscalCondition.Location = new Point(18, 50);
        lblFiscalCondition.Name = "lblFiscalCondition";
        lblFiscalCondition.Size = new Size(90, 15);
        lblFiscalCondition.TabIndex = 2;
        lblFiscalCondition.Text = "Condición Fiscal:";
        // 
        // lueFiscalCondition
        // 
        lueFiscalCondition.Location = new Point(178, 47);
        lueFiscalCondition.Name = "lueFiscalCondition";
        lueFiscalCondition.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueFiscalCondition.Properties.Appearance.Options.UseFont = true;
        lueFiscalCondition.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFiscalCondition.Properties.NullText = "";
        lueFiscalCondition.Size = new Size(230, 22);
        lueFiscalCondition.TabIndex = 3;
        // 
        // lblThirdPartyType
        // 
        lblThirdPartyType.Appearance.Font = new Font("Segoe UI", 9F);
        lblThirdPartyType.Appearance.Options.UseFont = true;
        lblThirdPartyType.Location = new Point(18, 78);
        lblThirdPartyType.Name = "lblThirdPartyType";
        lblThirdPartyType.Size = new Size(86, 15);
        lblThirdPartyType.TabIndex = 4;
        lblThirdPartyType.Text = "Tipo de Tercero:";
        // 
        // lueThirdPartyType
        // 
        lueThirdPartyType.Location = new Point(178, 75);
        lueThirdPartyType.Name = "lueThirdPartyType";
        lueThirdPartyType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueThirdPartyType.Properties.Appearance.Options.UseFont = true;
        lueThirdPartyType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueThirdPartyType.Properties.NullText = "";
        lueThirdPartyType.Size = new Size(230, 22);
        lueThirdPartyType.TabIndex = 5;
        // 
        // lblAutomaticAccounting
        // 
        lblAutomaticAccounting.Appearance.Font = new Font("Segoe UI", 9F);
        lblAutomaticAccounting.Appearance.Options.UseFont = true;
        lblAutomaticAccounting.Location = new Point(488, 22);
        lblAutomaticAccounting.Name = "lblAutomaticAccounting";
        lblAutomaticAccounting.Size = new Size(150, 15);
        lblAutomaticAccounting.TabIndex = 6;
        lblAutomaticAccounting.Text = "Contabilización Automática:";
        // 
        // tglAutomaticAccounting
        // 
        tglAutomaticAccounting.Location = new Point(680, 17);
        tglAutomaticAccounting.Name = "tglAutomaticAccounting";
        tglAutomaticAccounting.Properties.OffText = "";
        tglAutomaticAccounting.Properties.OnText = "";
        tglAutomaticAccounting.Size = new Size(50, 18);
        tglAutomaticAccounting.TabIndex = 7;
        // 
        // lblAutomaticAccountingValue
        // 
        lblAutomaticAccountingValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblAutomaticAccountingValue.Appearance.Options.UseFont = true;
        lblAutomaticAccountingValue.Location = new Point(736, 22);
        lblAutomaticAccountingValue.Name = "lblAutomaticAccountingValue";
        lblAutomaticAccountingValue.Size = new Size(10, 15);
        lblAutomaticAccountingValue.TabIndex = 8;
        lblAutomaticAccountingValue.Text = "Sí";
        // 
        // lblRequiresReconciliation
        // 
        lblRequiresReconciliation.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiresReconciliation.Appearance.Options.UseFont = true;
        lblRequiresReconciliation.Location = new Point(488, 50);
        lblRequiresReconciliation.Name = "lblRequiresReconciliation";
        lblRequiresReconciliation.Size = new Size(118, 15);
        lblRequiresReconciliation.TabIndex = 9;
        lblRequiresReconciliation.Text = "Requiere Conciliación:";
        // 
        // tglRequiresReconciliation
        // 
        tglRequiresReconciliation.Location = new Point(680, 45);
        tglRequiresReconciliation.Name = "tglRequiresReconciliation";
        tglRequiresReconciliation.Properties.OffText = "";
        tglRequiresReconciliation.Properties.OnText = "";
        tglRequiresReconciliation.Size = new Size(50, 18);
        tglRequiresReconciliation.TabIndex = 10;
        // 
        // lblRequiresReconciliationValue
        // 
        lblRequiresReconciliationValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiresReconciliationValue.Appearance.Options.UseFont = true;
        lblRequiresReconciliationValue.Location = new Point(736, 50);
        lblRequiresReconciliationValue.Name = "lblRequiresReconciliationValue";
        lblRequiresReconciliationValue.Size = new Size(10, 15);
        lblRequiresReconciliationValue.TabIndex = 11;
        lblRequiresReconciliationValue.Text = "Sí";
        // 
        // lblHandlesAdvances
        // 
        lblHandlesAdvances.Appearance.Font = new Font("Segoe UI", 9F);
        lblHandlesAdvances.Appearance.Options.UseFont = true;
        lblHandlesAdvances.Location = new Point(870, 22);
        lblHandlesAdvances.Name = "lblHandlesAdvances";
        lblHandlesAdvances.Size = new Size(95, 15);
        lblHandlesAdvances.TabIndex = 12;
        lblHandlesAdvances.Text = "Maneja Anticipos:";
        // 
        // tglHandlesAdvances
        // 
        tglHandlesAdvances.Location = new Point(1060, 17);
        tglHandlesAdvances.Name = "tglHandlesAdvances";
        tglHandlesAdvances.Properties.OffText = "";
        tglHandlesAdvances.Properties.OnText = "";
        tglHandlesAdvances.Size = new Size(50, 18);
        tglHandlesAdvances.TabIndex = 13;
        // 
        // lblHandlesAdvancesValue
        // 
        lblHandlesAdvancesValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblHandlesAdvancesValue.Appearance.Options.UseFont = true;
        lblHandlesAdvancesValue.Location = new Point(1116, 22);
        lblHandlesAdvancesValue.Name = "lblHandlesAdvancesValue";
        lblHandlesAdvancesValue.Size = new Size(10, 15);
        lblHandlesAdvancesValue.TabIndex = 14;
        lblHandlesAdvancesValue.Text = "Sí";
        // 
        // lblAccountingBlocked
        // 
        lblAccountingBlocked.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingBlocked.Appearance.Options.UseFont = true;
        lblAccountingBlocked.Location = new Point(870, 50);
        lblAccountingBlocked.Name = "lblAccountingBlocked";
        lblAccountingBlocked.Size = new Size(145, 15);
        lblAccountingBlocked.TabIndex = 15;
        lblAccountingBlocked.Text = "Bloqueado Contablemente:";
        // 
        // tglAccountingBlocked
        // 
        tglAccountingBlocked.Location = new Point(1060, 45);
        tglAccountingBlocked.Name = "tglAccountingBlocked";
        tglAccountingBlocked.Properties.OffText = "";
        tglAccountingBlocked.Properties.OnText = "";
        tglAccountingBlocked.Size = new Size(50, 18);
        tglAccountingBlocked.TabIndex = 16;
        // 
        // lblAccountingBlockedValue
        // 
        lblAccountingBlockedValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingBlockedValue.Appearance.Options.UseFont = true;
        lblAccountingBlockedValue.Location = new Point(1116, 50);
        lblAccountingBlockedValue.Name = "lblAccountingBlockedValue";
        lblAccountingBlockedValue.Size = new Size(16, 15);
        lblAccountingBlockedValue.TabIndex = 17;
        lblAccountingBlockedValue.Text = "No";
        // 
        // pnlAccountingActions
        // 
        pnlAccountingActions.BorderStyle = BorderStyles.NoBorder;
        pnlAccountingActions.Controls.Add(btnAddAccountingAccount);
        pnlAccountingActions.Controls.Add(btnEditAccountingAccount);
        pnlAccountingActions.Controls.Add(btnDeleteAccountingAccount);
        pnlAccountingActions.Controls.Add(btnSetDefaultAccountingAccount);
        pnlAccountingActions.Location = new Point(2, 118);
        pnlAccountingActions.Name = "pnlAccountingActions";
        pnlAccountingActions.Size = new Size(1338, 48);
        pnlAccountingActions.TabIndex = 1;
        // 
        // btnAddAccountingAccount
        // 
        btnAddAccountingAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddAccountingAccount.Appearance.Options.UseFont = true;
        btnAddAccountingAccount.Location = new Point(14, 10);
        btnAddAccountingAccount.Name = "btnAddAccountingAccount";
        btnAddAccountingAccount.Size = new Size(88, 28);
        btnAddAccountingAccount.TabIndex = 0;
        btnAddAccountingAccount.Text = "Agregar";
        // 
        // btnEditAccountingAccount
        // 
        btnEditAccountingAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditAccountingAccount.Appearance.Options.UseFont = true;
        btnEditAccountingAccount.Location = new Point(112, 10);
        btnEditAccountingAccount.Name = "btnEditAccountingAccount";
        btnEditAccountingAccount.Size = new Size(88, 28);
        btnEditAccountingAccount.TabIndex = 1;
        btnEditAccountingAccount.Text = "Editar";
        // 
        // btnDeleteAccountingAccount
        // 
        btnDeleteAccountingAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteAccountingAccount.Appearance.Options.UseFont = true;
        btnDeleteAccountingAccount.Location = new Point(210, 10);
        btnDeleteAccountingAccount.Name = "btnDeleteAccountingAccount";
        btnDeleteAccountingAccount.Size = new Size(88, 28);
        btnDeleteAccountingAccount.TabIndex = 2;
        btnDeleteAccountingAccount.Text = "Eliminar";
        // 
        // btnSetDefaultAccountingAccount
        // 
        btnSetDefaultAccountingAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetDefaultAccountingAccount.Appearance.Options.UseFont = true;
        btnSetDefaultAccountingAccount.Location = new Point(308, 10);
        btnSetDefaultAccountingAccount.Name = "btnSetDefaultAccountingAccount";
        btnSetDefaultAccountingAccount.Size = new Size(116, 28);
        btnSetDefaultAccountingAccount.TabIndex = 3;
        btnSetDefaultAccountingAccount.Text = "Predeterminada";
        // 
        // grdAccountingAccounts
        // 
        grdAccountingAccounts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdAccountingAccounts.Location = new Point(14, 174);
        grdAccountingAccounts.MainView = gvAccountingAccounts;
        grdAccountingAccounts.Name = "grdAccountingAccounts";
        grdAccountingAccounts.Size = new Size(1314, 212);
        grdAccountingAccounts.TabIndex = 2;
        grdAccountingAccounts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvAccountingAccounts });
        // 
        // gvAccountingAccounts
        // 
        gvAccountingAccounts.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvAccountingAccounts.Appearance.HeaderPanel.Options.UseFont = true;
        gvAccountingAccounts.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvAccountingAccounts.Appearance.Row.Options.UseFont = true;
        gvAccountingAccounts.Columns.AddRange(new GridColumn[] { colAccountingAccountType, colAccountingAccountCodeName, colAccountingDimension1, colAccountingDimension2, colAccountingDimension3, colAccountingDimension4, colAccountingDimension5, colAccountingIsDefault, colAccountingIsActive });
        gvAccountingAccounts.GridControl = grdAccountingAccounts;
        gvAccountingAccounts.Name = "gvAccountingAccounts";
        gvAccountingAccounts.OptionsBehavior.Editable = false;
        gvAccountingAccounts.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvAccountingAccounts.OptionsView.ShowGroupPanel = false;
        // 
        // colAccountingAccountType
        // 
        colAccountingAccountType.Caption = "Tipo";
        colAccountingAccountType.FieldName = "AccountType";
        colAccountingAccountType.Name = "colAccountingAccountType";
        colAccountingAccountType.Visible = true;
        colAccountingAccountType.VisibleIndex = 0;
        colAccountingAccountType.Width = 160;
        // 
        // colAccountingAccountCodeName
        // 
        colAccountingAccountCodeName.Caption = "Cuenta Contable";
        colAccountingAccountCodeName.FieldName = "AccountCodeName";
        colAccountingAccountCodeName.Name = "colAccountingAccountCodeName";
        colAccountingAccountCodeName.Visible = true;
        colAccountingAccountCodeName.VisibleIndex = 1;
        colAccountingAccountCodeName.Width = 280;
        // 
        // colAccountingDimension1
        // 
        colAccountingDimension1.Caption = "Dim. 1";
        colAccountingDimension1.FieldName = "Dimension1";
        colAccountingDimension1.Name = "colAccountingDimension1";
        colAccountingDimension1.Visible = true;
        colAccountingDimension1.VisibleIndex = 2;
        colAccountingDimension1.Width = 85;
        // 
        // colAccountingDimension2
        // 
        colAccountingDimension2.Caption = "Dim. 2";
        colAccountingDimension2.FieldName = "Dimension2";
        colAccountingDimension2.Name = "colAccountingDimension2";
        colAccountingDimension2.Visible = true;
        colAccountingDimension2.VisibleIndex = 3;
        colAccountingDimension2.Width = 85;
        // 
        // colAccountingDimension3
        // 
        colAccountingDimension3.Caption = "Dim. 3";
        colAccountingDimension3.FieldName = "Dimension3";
        colAccountingDimension3.Name = "colAccountingDimension3";
        colAccountingDimension3.Visible = true;
        colAccountingDimension3.VisibleIndex = 4;
        colAccountingDimension3.Width = 85;
        // 
        // colAccountingDimension4
        // 
        colAccountingDimension4.Caption = "Dim. 4";
        colAccountingDimension4.FieldName = "Dimension4";
        colAccountingDimension4.Name = "colAccountingDimension4";
        colAccountingDimension4.Visible = true;
        colAccountingDimension4.VisibleIndex = 5;
        colAccountingDimension4.Width = 85;
        // 
        // colAccountingDimension5
        // 
        colAccountingDimension5.Caption = "Dim. 5";
        colAccountingDimension5.FieldName = "Dimension5";
        colAccountingDimension5.Name = "colAccountingDimension5";
        colAccountingDimension5.Visible = true;
        colAccountingDimension5.VisibleIndex = 6;
        colAccountingDimension5.Width = 85;
        // 
        // colAccountingIsDefault
        // 
        colAccountingIsDefault.Caption = "Predeterminada";
        colAccountingIsDefault.FieldName = "IsDefault";
        colAccountingIsDefault.Name = "colAccountingIsDefault";
        colAccountingIsDefault.Visible = true;
        colAccountingIsDefault.VisibleIndex = 7;
        colAccountingIsDefault.Width = 120;
        // 
        // colAccountingIsActive
        // 
        colAccountingIsActive.Caption = "Activa";
        colAccountingIsActive.FieldName = "IsActive";
        colAccountingIsActive.Name = "colAccountingIsActive";
        colAccountingIsActive.Visible = true;
        colAccountingIsActive.VisibleIndex = 8;
        colAccountingIsActive.Width = 80;
        // 
        // tabSap
        // 
        tabSap.Controls.Add(pnlSapContent);
        tabSap.Name = "tabSap";
        tabSap.Size = new Size(1342, 404);
        tabSap.Text = "SAP";
        // 
        // pnlSapContent
        // 
        pnlSapContent.BorderStyle = BorderStyles.Simple;
        pnlSapContent.Controls.Add(pnlSapSyncData);
        pnlSapContent.Controls.Add(pnlSapAudit);
        pnlSapContent.Dock = DockStyle.Fill;
        pnlSapContent.Location = new Point(0, 0);
        pnlSapContent.Name = "pnlSapContent";
        pnlSapContent.Size = new Size(1342, 404);
        pnlSapContent.TabIndex = 0;
        // 
        // pnlSapSyncData
        // 
        pnlSapSyncData.BorderStyle = BorderStyles.NoBorder;
        pnlSapSyncData.Controls.Add(lblSapSynchronized);
        pnlSapSyncData.Controls.Add(tglSapSynchronized);
        pnlSapSyncData.Controls.Add(lblSapSynchronizedValue);
        pnlSapSyncData.Controls.Add(lblSapIntegrationValid);
        pnlSapSyncData.Controls.Add(tglSapIntegrationValid);
        pnlSapSyncData.Controls.Add(lblSapIntegrationValidValue);
        pnlSapSyncData.Controls.Add(lblSapErrorBlocked);
        pnlSapSyncData.Controls.Add(tglSapErrorBlocked);
        pnlSapSyncData.Controls.Add(lblSapErrorBlockedValue);
        pnlSapSyncData.Controls.Add(lblSapAutoUpdate);
        pnlSapSyncData.Controls.Add(tglSapAutoUpdate);
        pnlSapSyncData.Controls.Add(lblSapAutoUpdateValue);
        pnlSapSyncData.Controls.Add(lblSapLastSync);
        pnlSapSyncData.Controls.Add(txtSapLastSync);
        pnlSapSyncData.Controls.Add(lblSapLastSyncUser);
        pnlSapSyncData.Controls.Add(txtSapLastSyncUser);
        pnlSapSyncData.Controls.Add(lblSapDataOrigin);
        pnlSapSyncData.Controls.Add(txtSapDataOrigin);
        pnlSapSyncData.Controls.Add(lblSapIntegrationStatus);
        pnlSapSyncData.Controls.Add(txtSapIntegrationStatus);
        pnlSapSyncData.Dock = DockStyle.Top;
        pnlSapSyncData.Location = new Point(2, 2);
        pnlSapSyncData.Name = "pnlSapSyncData";
        pnlSapSyncData.Size = new Size(1338, 132);
        pnlSapSyncData.TabIndex = 0;
        // 
        // lblSapSynchronized
        // 
        lblSapSynchronized.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSynchronized.Appearance.Options.UseFont = true;
        lblSapSynchronized.Location = new Point(32, 34);
        lblSapSynchronized.Name = "lblSapSynchronized";
        lblSapSynchronized.Size = new Size(118, 15);
        lblSapSynchronized.TabIndex = 0;
        lblSapSynchronized.Text = "Sincronizado con SAP:";
        // 
        // tglSapSynchronized
        // 
        tglSapSynchronized.Enabled = false;
        tglSapSynchronized.Location = new Point(198, 29);
        tglSapSynchronized.Name = "tglSapSynchronized";
        tglSapSynchronized.Properties.OffText = "";
        tglSapSynchronized.Properties.OnText = "";
        tglSapSynchronized.Size = new Size(50, 18);
        tglSapSynchronized.TabIndex = 1;
        // 
        // lblSapSynchronizedValue
        // 
        lblSapSynchronizedValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSynchronizedValue.Appearance.Options.UseFont = true;
        lblSapSynchronizedValue.Location = new Point(264, 34);
        lblSapSynchronizedValue.Name = "lblSapSynchronizedValue";
        lblSapSynchronizedValue.Size = new Size(10, 15);
        lblSapSynchronizedValue.TabIndex = 2;
        lblSapSynchronizedValue.Text = "Sí";
        // 
        // lblSapIntegrationValid
        // 
        lblSapIntegrationValid.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapIntegrationValid.Appearance.Options.UseFont = true;
        lblSapIntegrationValid.Location = new Point(32, 66);
        lblSapIntegrationValid.Name = "lblSapIntegrationValid";
        lblSapIntegrationValid.Size = new Size(125, 15);
        lblSapIntegrationValid.TabIndex = 3;
        lblSapIntegrationValid.Text = "Válido para Integración:";
        // 
        // tglSapIntegrationValid
        // 
        tglSapIntegrationValid.Enabled = false;
        tglSapIntegrationValid.Location = new Point(198, 61);
        tglSapIntegrationValid.Name = "tglSapIntegrationValid";
        tglSapIntegrationValid.Properties.OffText = "";
        tglSapIntegrationValid.Properties.OnText = "";
        tglSapIntegrationValid.Size = new Size(50, 18);
        tglSapIntegrationValid.TabIndex = 4;
        // 
        // lblSapIntegrationValidValue
        // 
        lblSapIntegrationValidValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapIntegrationValidValue.Appearance.Options.UseFont = true;
        lblSapIntegrationValidValue.Location = new Point(264, 66);
        lblSapIntegrationValidValue.Name = "lblSapIntegrationValidValue";
        lblSapIntegrationValidValue.Size = new Size(10, 15);
        lblSapIntegrationValidValue.TabIndex = 5;
        lblSapIntegrationValidValue.Text = "Sí";
        // 
        // lblSapErrorBlocked
        // 
        lblSapErrorBlocked.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapErrorBlocked.Appearance.Options.UseFont = true;
        lblSapErrorBlocked.Location = new Point(32, 98);
        lblSapErrorBlocked.Name = "lblSapErrorBlocked";
        lblSapErrorBlocked.Size = new Size(96, 15);
        lblSapErrorBlocked.TabIndex = 6;
        lblSapErrorBlocked.Text = "Bloqueo por Error:";
        // 
        // tglSapErrorBlocked
        // 
        tglSapErrorBlocked.Enabled = false;
        tglSapErrorBlocked.Location = new Point(198, 93);
        tglSapErrorBlocked.Name = "tglSapErrorBlocked";
        tglSapErrorBlocked.Properties.OffText = "";
        tglSapErrorBlocked.Properties.OnText = "";
        tglSapErrorBlocked.Size = new Size(50, 18);
        tglSapErrorBlocked.TabIndex = 7;
        // 
        // lblSapErrorBlockedValue
        // 
        lblSapErrorBlockedValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapErrorBlockedValue.Appearance.Options.UseFont = true;
        lblSapErrorBlockedValue.Location = new Point(264, 98);
        lblSapErrorBlockedValue.Name = "lblSapErrorBlockedValue";
        lblSapErrorBlockedValue.Size = new Size(16, 15);
        lblSapErrorBlockedValue.TabIndex = 8;
        lblSapErrorBlockedValue.Text = "No";
        // 
        // lblSapAutoUpdate
        // 
        lblSapAutoUpdate.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapAutoUpdate.Appearance.Options.UseFont = true;
        lblSapAutoUpdate.Location = new Point(454, 34);
        lblSapAutoUpdate.Name = "lblSapAutoUpdate";
        lblSapAutoUpdate.Size = new Size(139, 15);
        lblSapAutoUpdate.TabIndex = 9;
        lblSapAutoUpdate.Text = "Actualización Automática:";
        // 
        // tglSapAutoUpdate
        // 
        tglSapAutoUpdate.Enabled = false;
        tglSapAutoUpdate.Location = new Point(646, 29);
        tglSapAutoUpdate.Name = "tglSapAutoUpdate";
        tglSapAutoUpdate.Properties.OffText = "";
        tglSapAutoUpdate.Properties.OnText = "";
        tglSapAutoUpdate.Size = new Size(50, 18);
        tglSapAutoUpdate.TabIndex = 10;
        // 
        // lblSapAutoUpdateValue
        // 
        lblSapAutoUpdateValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapAutoUpdateValue.Appearance.Options.UseFont = true;
        lblSapAutoUpdateValue.Location = new Point(712, 34);
        lblSapAutoUpdateValue.Name = "lblSapAutoUpdateValue";
        lblSapAutoUpdateValue.Size = new Size(10, 15);
        lblSapAutoUpdateValue.TabIndex = 11;
        lblSapAutoUpdateValue.Text = "Sí";
        // 
        // lblSapLastSync
        // 
        lblSapLastSync.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastSync.Appearance.Options.UseFont = true;
        lblSapLastSync.Location = new Point(454, 66);
        lblSapLastSync.Name = "lblSapLastSync";
        lblSapLastSync.Size = new Size(118, 15);
        lblSapLastSync.TabIndex = 12;
        lblSapLastSync.Text = "Última Sincronización:";
        // 
        // txtSapLastSync
        // 
        txtSapLastSync.Location = new Point(646, 62);
        txtSapLastSync.Name = "txtSapLastSync";
        txtSapLastSync.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapLastSync.Properties.Appearance.Options.UseFont = true;
        txtSapLastSync.Properties.ReadOnly = true;
        txtSapLastSync.Size = new Size(210, 22);
        txtSapLastSync.TabIndex = 13;
        // 
        // lblSapLastSyncUser
        // 
        lblSapLastSyncUser.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastSyncUser.Appearance.Options.UseFont = true;
        lblSapLastSyncUser.Location = new Point(454, 98);
        lblSapLastSyncUser.Name = "lblSapLastSyncUser";
        lblSapLastSyncUser.Size = new Size(161, 15);
        lblSapLastSyncUser.TabIndex = 14;
        lblSapLastSyncUser.Text = "Usuario Última Sincronización:";
        // 
        // txtSapLastSyncUser
        // 
        txtSapLastSyncUser.Location = new Point(646, 94);
        txtSapLastSyncUser.Name = "txtSapLastSyncUser";
        txtSapLastSyncUser.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapLastSyncUser.Properties.Appearance.Options.UseFont = true;
        txtSapLastSyncUser.Properties.ReadOnly = true;
        txtSapLastSyncUser.Size = new Size(210, 22);
        txtSapLastSyncUser.TabIndex = 15;
        // 
        // lblSapDataOrigin
        // 
        lblSapDataOrigin.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapDataOrigin.Appearance.Options.UseFont = true;
        lblSapDataOrigin.Location = new Point(916, 34);
        lblSapDataOrigin.Name = "lblSapDataOrigin";
        lblSapDataOrigin.Size = new Size(88, 15);
        lblSapDataOrigin.TabIndex = 16;
        lblSapDataOrigin.Text = "Origen de Datos:";
        // 
        // txtSapDataOrigin
        // 
        txtSapDataOrigin.Location = new Point(1088, 30);
        txtSapDataOrigin.Name = "txtSapDataOrigin";
        txtSapDataOrigin.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapDataOrigin.Properties.Appearance.Options.UseFont = true;
        txtSapDataOrigin.Properties.ReadOnly = true;
        txtSapDataOrigin.Size = new Size(210, 22);
        txtSapDataOrigin.TabIndex = 17;
        // 
        // lblSapIntegrationStatus
        // 
        lblSapIntegrationStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapIntegrationStatus.Appearance.Options.UseFont = true;
        lblSapIntegrationStatus.Location = new Point(916, 66);
        lblSapIntegrationStatus.Name = "lblSapIntegrationStatus";
        lblSapIntegrationStatus.Size = new Size(117, 15);
        lblSapIntegrationStatus.TabIndex = 18;
        lblSapIntegrationStatus.Text = "Estado de Integración:";
        // 
        // txtSapIntegrationStatus
        // 
        txtSapIntegrationStatus.Location = new Point(1088, 62);
        txtSapIntegrationStatus.Name = "txtSapIntegrationStatus";
        txtSapIntegrationStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapIntegrationStatus.Properties.Appearance.Options.UseFont = true;
        txtSapIntegrationStatus.Properties.ReadOnly = true;
        txtSapIntegrationStatus.Size = new Size(210, 22);
        txtSapIntegrationStatus.TabIndex = 19;
        // 
        // pnlSapAudit
        // 
        pnlSapAudit.BorderStyle = BorderStyles.NoBorder;
        pnlSapAudit.Controls.Add(lblSapAuditTitle);
        pnlSapAudit.Controls.Add(grdSapAudit);
        pnlSapAudit.Location = new Point(2, 134);
        pnlSapAudit.Name = "pnlSapAudit";
        pnlSapAudit.Size = new Size(1338, 271);
        pnlSapAudit.TabIndex = 1;
        // 
        // lblSapAuditTitle
        // 
        lblSapAuditTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapAuditTitle.Appearance.Options.UseFont = true;
        lblSapAuditTitle.Location = new Point(22, 14);
        lblSapAuditTitle.Name = "lblSapAuditTitle";
        lblSapAuditTitle.Size = new Size(156, 20);
        lblSapAuditTitle.TabIndex = 0;
        lblSapAuditTitle.Text = "Auditoría / Integración";
        // 
        // grdSapAudit
        // 
        grdSapAudit.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdSapAudit.Location = new Point(18, 40);
        grdSapAudit.MainView = gvSapAudit;
        grdSapAudit.Name = "grdSapAudit";
        grdSapAudit.Size = new Size(1302, 213);
        grdSapAudit.TabIndex = 1;
        grdSapAudit.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvSapAudit });
        // 
        // gvSapAudit
        // 
        gvSapAudit.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvSapAudit.Appearance.HeaderPanel.Options.UseFont = true;
        gvSapAudit.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvSapAudit.Appearance.Row.Options.UseFont = true;
        gvSapAudit.Columns.AddRange(new GridColumn[] { colSapAuditDate, colSapAuditAction, colSapAuditResult, colSapAuditUser, colSapAuditMessage });
        gvSapAudit.GridControl = grdSapAudit;
        gvSapAudit.Name = "gvSapAudit";
        gvSapAudit.OptionsBehavior.Editable = false;
        gvSapAudit.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvSapAudit.OptionsView.ShowGroupPanel = false;
        // 
        // colSapAuditDate
        // 
        colSapAuditDate.Caption = "Fecha";
        colSapAuditDate.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
        colSapAuditDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        colSapAuditDate.FieldName = "Date";
        colSapAuditDate.Name = "colSapAuditDate";
        colSapAuditDate.Visible = true;
        colSapAuditDate.VisibleIndex = 0;
        colSapAuditDate.Width = 190;
        // 
        // colSapAuditAction
        // 
        colSapAuditAction.Caption = "Acción";
        colSapAuditAction.FieldName = "Action";
        colSapAuditAction.Name = "colSapAuditAction";
        colSapAuditAction.Visible = true;
        colSapAuditAction.VisibleIndex = 1;
        colSapAuditAction.Width = 180;
        // 
        // colSapAuditResult
        // 
        colSapAuditResult.Caption = "Resultado";
        colSapAuditResult.FieldName = "Result";
        colSapAuditResult.Name = "colSapAuditResult";
        colSapAuditResult.Visible = true;
        colSapAuditResult.VisibleIndex = 2;
        colSapAuditResult.Width = 150;
        // 
        // colSapAuditUser
        // 
        colSapAuditUser.Caption = "Usuario";
        colSapAuditUser.FieldName = "User";
        colSapAuditUser.Name = "colSapAuditUser";
        colSapAuditUser.Visible = true;
        colSapAuditUser.VisibleIndex = 3;
        colSapAuditUser.Width = 140;
        // 
        // colSapAuditMessage
        // 
        colSapAuditMessage.Caption = "Mensaje";
        colSapAuditMessage.FieldName = "Message";
        colSapAuditMessage.Name = "colSapAuditMessage";
        colSapAuditMessage.Visible = true;
        colSapAuditMessage.VisibleIndex = 4;
        colSapAuditMessage.Width = 620;
        // 
        // tabAttachments
        // 
        tabAttachments.Controls.Add(pnlAttachmentsContent);
        tabAttachments.Name = "tabAttachments";
        tabAttachments.Size = new Size(1342, 404);
        tabAttachments.Text = "Observaciones y Anexos";
        // 
        // pnlAttachmentsContent
        // 
        pnlAttachmentsContent.BorderStyle = BorderStyles.Simple;
        pnlAttachmentsContent.Controls.Add(pnlObservationsSection);
        pnlAttachmentsContent.Controls.Add(pnlDocumentsSection);
        pnlAttachmentsContent.Dock = DockStyle.Fill;
        pnlAttachmentsContent.Location = new Point(0, 0);
        pnlAttachmentsContent.Name = "pnlAttachmentsContent";
        pnlAttachmentsContent.Size = new Size(1342, 404);
        pnlAttachmentsContent.TabIndex = 0;
        // 
        // pnlObservationsSection
        // 
        pnlObservationsSection.BorderStyle = BorderStyles.NoBorder;
        pnlObservationsSection.Controls.Add(lblSupplierObservationsTitle);
        pnlObservationsSection.Controls.Add(memSupplierObservations);
        pnlObservationsSection.Dock = DockStyle.Left;
        pnlObservationsSection.Location = new Point(2, 2);
        pnlObservationsSection.Name = "pnlObservationsSection";
        pnlObservationsSection.Size = new Size(560, 400);
        pnlObservationsSection.TabIndex = 0;
        // 
        // lblSupplierObservationsTitle
        // 
        lblSupplierObservationsTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblSupplierObservationsTitle.Appearance.Options.UseFont = true;
        lblSupplierObservationsTitle.Location = new Point(18, 14);
        lblSupplierObservationsTitle.Name = "lblSupplierObservationsTitle";
        lblSupplierObservationsTitle.Size = new Size(77, 15);
        lblSupplierObservationsTitle.TabIndex = 0;
        lblSupplierObservationsTitle.Text = "Observaciones";
        // 
        // memSupplierObservations
        // 
        memSupplierObservations.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        memSupplierObservations.Location = new Point(18, 42);
        memSupplierObservations.Name = "memSupplierObservations";
        memSupplierObservations.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memSupplierObservations.Properties.Appearance.Options.UseFont = true;
        memSupplierObservations.Size = new Size(520, 340);
        memSupplierObservations.TabIndex = 1;
        // 
        // pnlDocumentsSection
        // 
        pnlDocumentsSection.AutoSize = true;
        pnlDocumentsSection.BorderStyle = BorderStyles.NoBorder;
        pnlDocumentsSection.Controls.Add(lblAttachmentsTitle);
        pnlDocumentsSection.Controls.Add(pnlAttachmentActions);
        pnlDocumentsSection.Controls.Add(grdAttachments);
        pnlDocumentsSection.Controls.Add(lblAttachmentPath);
        pnlDocumentsSection.Controls.Add(txtAttachmentPath);
        pnlDocumentsSection.Controls.Add(lblAttachmentCategory);
        pnlDocumentsSection.Controls.Add(txtAttachmentCategory);
        pnlDocumentsSection.Controls.Add(lblAttachmentExpirationDate);
        pnlDocumentsSection.Controls.Add(txtAttachmentExpirationDate);
        pnlDocumentsSection.Controls.Add(lblAttachmentDescription);
        pnlDocumentsSection.Controls.Add(memAttachmentDescription);
        pnlDocumentsSection.Controls.Add(pnlAttachmentPreview);
        pnlDocumentsSection.Location = new Point(562, 2);
        pnlDocumentsSection.Name = "pnlDocumentsSection";
        pnlDocumentsSection.Size = new Size(781, 400);
        pnlDocumentsSection.TabIndex = 1;
        // 
        // lblAttachmentsTitle
        // 
        lblAttachmentsTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblAttachmentsTitle.Appearance.Options.UseFont = true;
        lblAttachmentsTitle.Location = new Point(18, 14);
        lblAttachmentsTitle.Name = "lblAttachmentsTitle";
        lblAttachmentsTitle.Size = new Size(111, 15);
        lblAttachmentsTitle.TabIndex = 0;
        lblAttachmentsTitle.Text = "Documentos Anexos";
        // 
        // pnlAttachmentActions
        // 
        pnlAttachmentActions.BorderStyle = BorderStyles.NoBorder;
        pnlAttachmentActions.Controls.Add(btnAttachDocument);
        pnlAttachmentActions.Controls.Add(btnDownloadDocument);
        pnlAttachmentActions.Controls.Add(btnViewDocument);
        pnlAttachmentActions.Controls.Add(btnDeleteDocument);
        pnlAttachmentActions.Location = new Point(18, 34);
        pnlAttachmentActions.Name = "pnlAttachmentActions";
        pnlAttachmentActions.Size = new Size(520, 36);
        pnlAttachmentActions.TabIndex = 1;
        // 
        // btnAttachDocument
        // 
        btnAttachDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAttachDocument.Appearance.Options.UseFont = true;
        btnAttachDocument.Location = new Point(0, 3);
        btnAttachDocument.Name = "btnAttachDocument";
        btnAttachDocument.Size = new Size(92, 28);
        btnAttachDocument.TabIndex = 0;
        btnAttachDocument.Text = "Adjuntar";
        // 
        // btnDownloadDocument
        // 
        btnDownloadDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDownloadDocument.Appearance.Options.UseFont = true;
        btnDownloadDocument.Location = new Point(104, 3);
        btnDownloadDocument.Name = "btnDownloadDocument";
        btnDownloadDocument.Size = new Size(92, 28);
        btnDownloadDocument.TabIndex = 1;
        btnDownloadDocument.Text = "Descargar";
        // 
        // btnViewDocument
        // 
        btnViewDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnViewDocument.Appearance.Options.UseFont = true;
        btnViewDocument.Location = new Point(208, 3);
        btnViewDocument.Name = "btnViewDocument";
        btnViewDocument.Size = new Size(92, 28);
        btnViewDocument.TabIndex = 2;
        btnViewDocument.Text = "Ver";
        // 
        // btnDeleteDocument
        // 
        btnDeleteDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteDocument.Appearance.Options.UseFont = true;
        btnDeleteDocument.Location = new Point(312, 3);
        btnDeleteDocument.Name = "btnDeleteDocument";
        btnDeleteDocument.Size = new Size(92, 28);
        btnDeleteDocument.TabIndex = 3;
        btnDeleteDocument.Text = "Eliminar";
        // 
        // grdAttachments
        // 
        grdAttachments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        grdAttachments.Location = new Point(18, 78);
        grdAttachments.MainView = gvAttachments;
        grdAttachments.Name = "grdAttachments";
        grdAttachments.Size = new Size(739, 130);
        grdAttachments.TabIndex = 2;
        grdAttachments.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { gvAttachments });
        // 
        // gvAttachments
        // 
        gvAttachments.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvAttachments.Appearance.HeaderPanel.Options.UseFont = true;
        gvAttachments.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvAttachments.Appearance.Row.Options.UseFont = true;
        gvAttachments.Columns.AddRange(new GridColumn[] { colAttachmentDocumentType, colAttachmentFileName, colAttachmentUploadDate, colAttachmentUser, colAttachmentFileSize, colAttachmentStatus });
        gvAttachments.GridControl = grdAttachments;
        gvAttachments.Name = "gvAttachments";
        gvAttachments.OptionsBehavior.Editable = false;
        gvAttachments.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvAttachments.OptionsView.ShowGroupPanel = false;
        // 
        // colAttachmentDocumentType
        // 
        colAttachmentDocumentType.Caption = "Tipo";
        colAttachmentDocumentType.FieldName = "DocumentType";
        colAttachmentDocumentType.Name = "colAttachmentDocumentType";
        colAttachmentDocumentType.Visible = true;
        colAttachmentDocumentType.VisibleIndex = 0;
        colAttachmentDocumentType.Width = 90;
        // 
        // colAttachmentFileName
        // 
        colAttachmentFileName.Caption = "Nombre Archivo";
        colAttachmentFileName.FieldName = "FileName";
        colAttachmentFileName.Name = "colAttachmentFileName";
        colAttachmentFileName.Visible = true;
        colAttachmentFileName.VisibleIndex = 1;
        colAttachmentFileName.Width = 230;
        // 
        // colAttachmentUploadDate
        // 
        colAttachmentUploadDate.Caption = "Fecha";
        colAttachmentUploadDate.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
        colAttachmentUploadDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        colAttachmentUploadDate.FieldName = "UploadDate";
        colAttachmentUploadDate.Name = "colAttachmentUploadDate";
        colAttachmentUploadDate.Visible = true;
        colAttachmentUploadDate.VisibleIndex = 2;
        colAttachmentUploadDate.Width = 120;
        // 
        // colAttachmentUser
        // 
        colAttachmentUser.Caption = "Usuario";
        colAttachmentUser.FieldName = "User";
        colAttachmentUser.Name = "colAttachmentUser";
        colAttachmentUser.Visible = true;
        colAttachmentUser.VisibleIndex = 3;
        colAttachmentUser.Width = 90;
        // 
        // colAttachmentFileSize
        // 
        colAttachmentFileSize.Caption = "Tamaño";
        colAttachmentFileSize.FieldName = "FileSize";
        colAttachmentFileSize.Name = "colAttachmentFileSize";
        colAttachmentFileSize.Visible = true;
        colAttachmentFileSize.VisibleIndex = 4;
        colAttachmentFileSize.Width = 85;
        // 
        // colAttachmentStatus
        // 
        colAttachmentStatus.Caption = "Estado";
        colAttachmentStatus.FieldName = "Status";
        colAttachmentStatus.Name = "colAttachmentStatus";
        colAttachmentStatus.Visible = true;
        colAttachmentStatus.VisibleIndex = 5;
        colAttachmentStatus.Width = 90;
        // 
        // lblAttachmentPath
        // 
        lblAttachmentPath.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentPath.Appearance.Options.UseFont = true;
        lblAttachmentPath.Location = new Point(18, 224);
        lblAttachmentPath.Name = "lblAttachmentPath";
        lblAttachmentPath.Size = new Size(91, 15);
        lblAttachmentPath.TabIndex = 3;
        lblAttachmentPath.Text = "Ruta / Ubicación:";
        // 
        // txtAttachmentPath
        // 
        txtAttachmentPath.Location = new Point(154, 220);
        txtAttachmentPath.Name = "txtAttachmentPath";
        txtAttachmentPath.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentPath.Properties.Appearance.Options.UseFont = true;
        txtAttachmentPath.Properties.ReadOnly = true;
        txtAttachmentPath.Size = new Size(310, 22);
        txtAttachmentPath.TabIndex = 4;
        // 
        // lblAttachmentCategory
        // 
        lblAttachmentCategory.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentCategory.Appearance.Options.UseFont = true;
        lblAttachmentCategory.Location = new Point(18, 252);
        lblAttachmentCategory.Name = "lblAttachmentCategory";
        lblAttachmentCategory.Size = new Size(136, 15);
        lblAttachmentCategory.TabIndex = 5;
        lblAttachmentCategory.Text = "Categoría de Documento:";
        // 
        // txtAttachmentCategory
        // 
        txtAttachmentCategory.Location = new Point(154, 248);
        txtAttachmentCategory.Name = "txtAttachmentCategory";
        txtAttachmentCategory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentCategory.Properties.Appearance.Options.UseFont = true;
        txtAttachmentCategory.Properties.ReadOnly = true;
        txtAttachmentCategory.Size = new Size(310, 22);
        txtAttachmentCategory.TabIndex = 6;
        // 
        // lblAttachmentExpirationDate
        // 
        lblAttachmentExpirationDate.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentExpirationDate.Appearance.Options.UseFont = true;
        lblAttachmentExpirationDate.Location = new Point(18, 280);
        lblAttachmentExpirationDate.Name = "lblAttachmentExpirationDate";
        lblAttachmentExpirationDate.Size = new Size(120, 15);
        lblAttachmentExpirationDate.TabIndex = 7;
        lblAttachmentExpirationDate.Text = "Fecha de Vencimiento:";
        // 
        // txtAttachmentExpirationDate
        // 
        txtAttachmentExpirationDate.Location = new Point(154, 276);
        txtAttachmentExpirationDate.Name = "txtAttachmentExpirationDate";
        txtAttachmentExpirationDate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentExpirationDate.Properties.Appearance.Options.UseFont = true;
        txtAttachmentExpirationDate.Properties.ReadOnly = true;
        txtAttachmentExpirationDate.Size = new Size(150, 22);
        txtAttachmentExpirationDate.TabIndex = 8;
        // 
        // lblAttachmentDescription
        // 
        lblAttachmentDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentDescription.Appearance.Options.UseFont = true;
        lblAttachmentDescription.Location = new Point(18, 308);
        lblAttachmentDescription.Name = "lblAttachmentDescription";
        lblAttachmentDescription.Size = new Size(65, 15);
        lblAttachmentDescription.TabIndex = 9;
        lblAttachmentDescription.Text = "Descripción:";
        // 
        // memAttachmentDescription
        // 
        memAttachmentDescription.Location = new Point(154, 304);
        memAttachmentDescription.Name = "memAttachmentDescription";
        memAttachmentDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memAttachmentDescription.Properties.Appearance.Options.UseFont = true;
        memAttachmentDescription.Properties.ReadOnly = true;
        memAttachmentDescription.Size = new Size(310, 36);
        memAttachmentDescription.TabIndex = 10;
        // 
        // pnlAttachmentPreview
        // 
        pnlAttachmentPreview.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        pnlAttachmentPreview.BorderStyle = BorderStyles.Simple;
        pnlAttachmentPreview.Controls.Add(lblAttachmentPreviewTitle);
        pnlAttachmentPreview.Controls.Add(lblAttachmentPreview);
        pnlAttachmentPreview.Location = new Point(493, 220);
        pnlAttachmentPreview.Name = "pnlAttachmentPreview";
        pnlAttachmentPreview.Size = new Size(264, 120);
        pnlAttachmentPreview.TabIndex = 11;
        // 
        // lblAttachmentPreviewTitle
        // 
        lblAttachmentPreviewTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblAttachmentPreviewTitle.Appearance.Options.UseFont = true;
        lblAttachmentPreviewTitle.Location = new Point(14, 12);
        lblAttachmentPreviewTitle.Name = "lblAttachmentPreviewTitle";
        lblAttachmentPreviewTitle.Size = new Size(61, 15);
        lblAttachmentPreviewTitle.TabIndex = 0;
        lblAttachmentPreviewTitle.Text = "Vista Previa";
        // 
        // lblAttachmentPreview
        // 
        lblAttachmentPreview.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentPreview.Appearance.ForeColor = Color.FromArgb(90, 90, 90);
        lblAttachmentPreview.Appearance.Options.UseFont = true;
        lblAttachmentPreview.Appearance.Options.UseForeColor = true;
        lblAttachmentPreview.Appearance.Options.UseTextOptions = true;
        lblAttachmentPreview.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lblAttachmentPreview.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        lblAttachmentPreview.AutoSizeMode = LabelAutoSizeMode.None;
        lblAttachmentPreview.Location = new Point(18, 38);
        lblAttachmentPreview.Name = "lblAttachmentPreview";
        lblAttachmentPreview.Size = new Size(226, 58);
        lblAttachmentPreview.TabIndex = 1;
        lblAttachmentPreview.Text = "Vista previa no disponible.\r\nSeleccione un documento.";
        // 
        // pnlHeader
        // 
        pnlHeader.BorderStyle = BorderStyles.NoBorder;
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(lblSupplierCode);
        pnlHeader.Controls.Add(txtSupplierCode);
        pnlHeader.Controls.Add(lblSupplierActive);
        pnlHeader.Controls.Add(tglSupplierActive);
        pnlHeader.Controls.Add(lblBusinessName);
        pnlHeader.Controls.Add(txtBusinessName);
        pnlHeader.Controls.Add(lblTradeName);
        pnlHeader.Controls.Add(txtTradeName);
        pnlHeader.Controls.Add(lblDocumentType);
        pnlHeader.Controls.Add(lueDocumentType);
        pnlHeader.Controls.Add(lblDocumentNumber);
        pnlHeader.Controls.Add(txtDocumentNumber);
        pnlHeader.Controls.Add(lblPersonType);
        pnlHeader.Controls.Add(luePersonType);
        pnlHeader.Controls.Add(lblSupplierType);
        pnlHeader.Controls.Add(lueSupplierType);
        pnlHeader.Controls.Add(lblMainContact);
        pnlHeader.Controls.Add(txtMainContact);
        pnlHeader.Controls.Add(lblPhone);
        pnlHeader.Controls.Add(txtPhone);
        pnlHeader.Controls.Add(lblEmail);
        pnlHeader.Controls.Add(txtEmail);
        pnlHeader.Controls.Add(lblCurrency);
        pnlHeader.Controls.Add(lueCurrency);
        pnlHeader.Controls.Add(lblPaymentCondition);
        pnlHeader.Controls.Add(luePaymentCondition);
        pnlHeader.Controls.Add(lblSupplierCategory);
        pnlHeader.Controls.Add(lueSupplierCategory);
        pnlHeader.Controls.Add(lblShortObservation);
        pnlHeader.Controls.Add(memShortObservation);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1344, 318);
        pnlHeader.TabIndex = 0;
        // 
        // lblTitle
        // 
        lblTitle.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblTitle.Appearance.Options.UseFont = true;
        lblTitle.Location = new Point(44, 14);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(270, 25);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "Mantenimiento de Proveedores";
        // 
        // lblSupplierCode
        // 
        lblSupplierCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierCode.Appearance.Options.UseFont = true;
        lblSupplierCode.Location = new Point(36, 76);
        lblSupplierCode.Name = "lblSupplierCode";
        lblSupplierCode.Size = new Size(42, 15);
        lblSupplierCode.TabIndex = 1;
        lblSupplierCode.Text = "Código:";
        // 
        // txtSupplierCode
        // 
        txtSupplierCode.Location = new Point(198, 73);
        txtSupplierCode.Name = "txtSupplierCode";
        txtSupplierCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierCode.Properties.Appearance.Options.UseFont = true;
        txtSupplierCode.Size = new Size(120, 22);
        txtSupplierCode.TabIndex = 2;
        // 
        // lblSupplierActive
        // 
        lblSupplierActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierActive.Appearance.Options.UseFont = true;
        lblSupplierActive.Location = new Point(410, 76);
        lblSupplierActive.Name = "lblSupplierActive";
        lblSupplierActive.Size = new Size(38, 15);
        lblSupplierActive.TabIndex = 3;
        lblSupplierActive.Text = "Estado:";
        // 
        // tglSupplierActive
        // 
        tglSupplierActive.Location = new Point(493, 70);
        tglSupplierActive.Name = "tglSupplierActive";
        tglSupplierActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglSupplierActive.Properties.Appearance.Options.UseFont = true;
        tglSupplierActive.Properties.OffText = "Inactivo";
        tglSupplierActive.Properties.OnText = "Activo";
        tglSupplierActive.Size = new Size(116, 20);
        tglSupplierActive.TabIndex = 4;
        // 
        // lblBusinessName
        // 
        lblBusinessName.Appearance.Font = new Font("Segoe UI", 9F);
        lblBusinessName.Appearance.Options.UseFont = true;
        lblBusinessName.Location = new Point(36, 113);
        lblBusinessName.Name = "lblBusinessName";
        lblBusinessName.Size = new Size(69, 15);
        lblBusinessName.TabIndex = 5;
        lblBusinessName.Text = "Razón Social:";
        // 
        // txtBusinessName
        // 
        txtBusinessName.Location = new Point(198, 110);
        txtBusinessName.Name = "txtBusinessName";
        txtBusinessName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBusinessName.Properties.Appearance.Options.UseFont = true;
        txtBusinessName.Size = new Size(640, 22);
        txtBusinessName.TabIndex = 6;
        // 
        // lblTradeName
        // 
        lblTradeName.Appearance.Font = new Font("Segoe UI", 9F);
        lblTradeName.Appearance.Options.UseFont = true;
        lblTradeName.Location = new Point(36, 149);
        lblTradeName.Name = "lblTradeName";
        lblTradeName.Size = new Size(104, 15);
        lblTradeName.TabIndex = 7;
        lblTradeName.Text = "Nombre Comercial:";
        // 
        // txtTradeName
        // 
        txtTradeName.Location = new Point(198, 146);
        txtTradeName.Name = "txtTradeName";
        txtTradeName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtTradeName.Properties.Appearance.Options.UseFont = true;
        txtTradeName.Size = new Size(640, 22);
        txtTradeName.TabIndex = 8;
        // 
        // lblDocumentType
        // 
        lblDocumentType.Appearance.Font = new Font("Segoe UI", 9F);
        lblDocumentType.Appearance.Options.UseFont = true;
        lblDocumentType.Location = new Point(36, 185);
        lblDocumentType.Name = "lblDocumentType";
        lblDocumentType.Size = new Size(109, 15);
        lblDocumentType.TabIndex = 9;
        lblDocumentType.Text = "Tipo de Documento:";
        // 
        // lueDocumentType
        // 
        lueDocumentType.Location = new Point(198, 182);
        lueDocumentType.Name = "lueDocumentType";
        lueDocumentType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueDocumentType.Properties.Appearance.Options.UseFont = true;
        lueDocumentType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueDocumentType.Properties.NullText = "";
        lueDocumentType.Size = new Size(206, 22);
        lueDocumentType.TabIndex = 10;
        // 
        // lblDocumentNumber
        // 
        lblDocumentNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblDocumentNumber.Appearance.Options.UseFont = true;
        lblDocumentNumber.Location = new Point(478, 185);
        lblDocumentNumber.Name = "lblDocumentNumber";
        lblDocumentNumber.Size = new Size(109, 15);
        lblDocumentNumber.TabIndex = 11;
        lblDocumentNumber.Text = "RUC / Identificación:";
        // 
        // txtDocumentNumber
        // 
        txtDocumentNumber.Location = new Point(628, 182);
        txtDocumentNumber.Name = "txtDocumentNumber";
        txtDocumentNumber.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtDocumentNumber.Properties.Appearance.Options.UseFont = true;
        txtDocumentNumber.Size = new Size(210, 22);
        txtDocumentNumber.TabIndex = 12;
        // 
        // lblPersonType
        // 
        lblPersonType.Appearance.Font = new Font("Segoe UI", 9F);
        lblPersonType.Appearance.Options.UseFont = true;
        lblPersonType.Location = new Point(36, 221);
        lblPersonType.Name = "lblPersonType";
        lblPersonType.Size = new Size(88, 15);
        lblPersonType.TabIndex = 13;
        lblPersonType.Text = "Tipo de Persona:";
        // 
        // luePersonType
        // 
        luePersonType.Location = new Point(198, 218);
        luePersonType.Name = "luePersonType";
        luePersonType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePersonType.Properties.Appearance.Options.UseFont = true;
        luePersonType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePersonType.Properties.NullText = "";
        luePersonType.Size = new Size(206, 22);
        luePersonType.TabIndex = 14;
        // 
        // lblSupplierType
        // 
        lblSupplierType.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierType.Appearance.Options.UseFont = true;
        lblSupplierType.Location = new Point(478, 221);
        lblSupplierType.Name = "lblSupplierType";
        lblSupplierType.Size = new Size(100, 15);
        lblSupplierType.TabIndex = 15;
        lblSupplierType.Text = "Tipo de Proveedor:";
        // 
        // lueSupplierType
        // 
        lueSupplierType.Location = new Point(628, 218);
        lueSupplierType.Name = "lueSupplierType";
        lueSupplierType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierType.Properties.Appearance.Options.UseFont = true;
        lueSupplierType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierType.Properties.NullText = "";
        lueSupplierType.Size = new Size(210, 22);
        lueSupplierType.TabIndex = 16;
        // 
        // lblMainContact
        // 
        lblMainContact.Appearance.Font = new Font("Segoe UI", 9F);
        lblMainContact.Appearance.Options.UseFont = true;
        lblMainContact.Location = new Point(36, 257);
        lblMainContact.Name = "lblMainContact";
        lblMainContact.Size = new Size(101, 15);
        lblMainContact.TabIndex = 17;
        lblMainContact.Text = "Contacto Principal:";
        // 
        // txtMainContact
        // 
        txtMainContact.Location = new Point(198, 254);
        txtMainContact.Name = "txtMainContact";
        txtMainContact.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtMainContact.Properties.Appearance.Options.UseFont = true;
        txtMainContact.Size = new Size(640, 22);
        txtMainContact.TabIndex = 18;
        // 
        // lblPhone
        // 
        lblPhone.Appearance.Font = new Font("Segoe UI", 9F);
        lblPhone.Appearance.Options.UseFont = true;
        lblPhone.Location = new Point(36, 293);
        lblPhone.Name = "lblPhone";
        lblPhone.Size = new Size(50, 15);
        lblPhone.TabIndex = 19;
        lblPhone.Text = "Teléfono:";
        // 
        // txtPhone
        // 
        txtPhone.Location = new Point(198, 290);
        txtPhone.Name = "txtPhone";
        txtPhone.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPhone.Properties.Appearance.Options.UseFont = true;
        txtPhone.Size = new Size(206, 22);
        txtPhone.TabIndex = 20;
        // 
        // lblEmail
        // 
        lblEmail.Appearance.Font = new Font("Segoe UI", 9F);
        lblEmail.Appearance.Options.UseFont = true;
        lblEmail.Location = new Point(478, 293);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(101, 15);
        lblEmail.TabIndex = 21;
        lblEmail.Text = "Correo Electrónico:";
        // 
        // txtEmail
        // 
        txtEmail.Location = new Point(628, 290);
        txtEmail.Name = "txtEmail";
        txtEmail.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtEmail.Properties.Appearance.Options.UseFont = true;
        txtEmail.Size = new Size(210, 22);
        txtEmail.TabIndex = 22;
        // 
        // lblCurrency
        // 
        lblCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblCurrency.Appearance.Options.UseFont = true;
        lblCurrency.Location = new Point(894, 86);
        lblCurrency.Name = "lblCurrency";
        lblCurrency.Size = new Size(47, 15);
        lblCurrency.TabIndex = 23;
        lblCurrency.Text = "Moneda:";
        // 
        // lueCurrency
        // 
        lueCurrency.Location = new Point(1078, 83);
        lueCurrency.Name = "lueCurrency";
        lueCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCurrency.Properties.Appearance.Options.UseFont = true;
        lueCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCurrency.Properties.NullText = "";
        lueCurrency.Size = new Size(290, 22);
        lueCurrency.TabIndex = 24;
        // 
        // lblPaymentCondition
        // 
        lblPaymentCondition.Appearance.Font = new Font("Segoe UI", 9F);
        lblPaymentCondition.Appearance.Options.UseFont = true;
        lblPaymentCondition.Location = new Point(894, 123);
        lblPaymentCondition.Name = "lblPaymentCondition";
        lblPaymentCondition.Size = new Size(104, 15);
        lblPaymentCondition.TabIndex = 25;
        lblPaymentCondition.Text = "Condición de Pago:";
        // 
        // luePaymentCondition
        // 
        luePaymentCondition.Location = new Point(1078, 120);
        luePaymentCondition.Name = "luePaymentCondition";
        luePaymentCondition.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePaymentCondition.Properties.Appearance.Options.UseFont = true;
        luePaymentCondition.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePaymentCondition.Properties.NullText = "";
        luePaymentCondition.Size = new Size(290, 22);
        luePaymentCondition.TabIndex = 26;
        // 
        // lblSupplierCategory
        // 
        lblSupplierCategory.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierCategory.Appearance.Options.UseFont = true;
        lblSupplierCategory.Location = new Point(894, 160);
        lblSupplierCategory.Name = "lblSupplierCategory";
        lblSupplierCategory.Size = new Size(132, 15);
        lblSupplierCategory.TabIndex = 27;
        lblSupplierCategory.Text = "Categoría / Clasificación:";
        // 
        // lueSupplierCategory
        // 
        lueSupplierCategory.Location = new Point(1078, 157);
        lueSupplierCategory.Name = "lueSupplierCategory";
        lueSupplierCategory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierCategory.Properties.Appearance.Options.UseFont = true;
        lueSupplierCategory.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierCategory.Properties.NullText = "";
        lueSupplierCategory.Size = new Size(290, 22);
        lueSupplierCategory.TabIndex = 28;
        // 
        // lblShortObservation
        // 
        lblShortObservation.Appearance.Font = new Font("Segoe UI", 9F);
        lblShortObservation.Appearance.Options.UseFont = true;
        lblShortObservation.Location = new Point(894, 197);
        lblShortObservation.Name = "lblShortObservation";
        lblShortObservation.Size = new Size(99, 15);
        lblShortObservation.TabIndex = 29;
        lblShortObservation.Text = "Observación corta:";
        // 
        // memShortObservation
        // 
        memShortObservation.Location = new Point(1078, 194);
        memShortObservation.Name = "memShortObservation";
        memShortObservation.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memShortObservation.Properties.Appearance.Options.UseFont = true;
        memShortObservation.Size = new Size(322, 68);
        memShortObservation.TabIndex = 30;
        // 
        // SupplierEditForm
        // 
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1344, 749);
        Controls.Add(pnlMain);
        MinimumSize = new Size(1180, 720);
        Name = "SupplierEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Mantenimiento de Proveedores";
        ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
        pnlMain.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)tabSupplier).EndInit();
        tabSupplier.ResumeLayout(false);
        tabGeneral.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlGeneralContent).EndInit();
        pnlGeneralContent.ResumeLayout(false);
        pnlGeneralContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtProvinceCity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtWebsite.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteRegistrationDate.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteRegistrationDate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditLimit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnPaymentTermDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAlternateCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueInternalClassification.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierSegment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglActiveForPurchases.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSubjectToWithholding.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglHandlesCredit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglBlocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memGeneralComments.Properties).EndInit();
        tabContacts.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlContactsContent).EndInit();
        pnlContactsContent.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grdContacts).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvContacts).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlContactsActions).EndInit();
        pnlContactsActions.ResumeLayout(false);
        tabAddresses.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlAddressesContent).EndInit();
        pnlAddressesContent.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grdAddresses).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvAddresses).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlAddressesActions).EndInit();
        pnlAddressesActions.ResumeLayout(false);
        tabPurchases.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlPurchasesContent).EndInit();
        pnlPurchasesContent.ResumeLayout(false);
        pnlPurchasesContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)luePurchasePaymentCondition.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchasePriceList.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnDeliveryTermDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueIncoterm.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCommercialDiscountPercent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseSupplierType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAssignedBuyer.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSuggestedCostCenter.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePreferredWarehouse.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnAverageDeliveryDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumOrderAmount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnMinimumOrderQuantity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnLeadTimeDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnDeliveryToleranceDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresPurchaseOrder.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSubjectToEvaluation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglActiveForImport.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAllowsUrgentPurchases.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdPurchaseHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvPurchaseHistory).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlPurchasesLast12Months).EndInit();
        pnlPurchasesLast12Months.ResumeLayout(false);
        pnlPurchasesLast12Months.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAveragePurchase).EndInit();
        pnlAveragePurchase.ResumeLayout(false);
        pnlAveragePurchase.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAverageDelivery12Months).EndInit();
        pnlAverageDelivery12Months.ResumeLayout(false);
        pnlAverageDelivery12Months.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlPurchaseOrdersLast12Months).EndInit();
        pnlPurchaseOrdersLast12Months.ResumeLayout(false);
        pnlPurchaseOrdersLast12Months.PerformLayout();
        tabBanks.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlBanksContent).EndInit();
        pnlBanksContent.ResumeLayout(false);
        pnlBanksContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlBanksActions).EndInit();
        pnlBanksActions.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grdBankAccounts).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvBankAccounts).EndInit();
        tabWithholdings.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlWithholdingsContent).EndInit();
        pnlWithholdingsContent.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlWithholdingsGeneral).EndInit();
        pnlWithholdingsGeneral.ResumeLayout(false);
        pnlWithholdingsGeneral.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglWithholdingAgent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueGeneralWithholdingType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnBaseWithholdingPercent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteWithholdingEffectiveDate.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteWithholdingEffectiveDate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtWithholdingResolutionNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglWithholdsVat.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglWithholdsIncomeTax.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglIssuesElectronicReceipts.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSubjectToPerception.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlWithholdingsActions).EndInit();
        pnlWithholdingsActions.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grdWithholdings).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvWithholdings).EndInit();
        tabAccounting.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlAccountingContent).EndInit();
        pnlAccountingContent.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlAccountingGeneral).EndInit();
        pnlAccountingGeneral.ResumeLayout(false);
        pnlAccountingGeneral.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueDefaultProject.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCondition.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueThirdPartyType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAutomaticAccounting.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresReconciliation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglHandlesAdvances.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAccountingBlocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingActions).EndInit();
        pnlAccountingActions.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grdAccountingAccounts).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvAccountingAccounts).EndInit();
        tabSap.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlSapContent).EndInit();
        pnlSapContent.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlSapSyncData).EndInit();
        pnlSapSyncData.ResumeLayout(false);
        pnlSapSyncData.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglSapSynchronized.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSapIntegrationValid.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSapErrorBlocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSapAutoUpdate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastSync.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastSyncUser.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapDataOrigin.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapIntegrationStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlSapAudit).EndInit();
        pnlSapAudit.ResumeLayout(false);
        pnlSapAudit.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdSapAudit).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvSapAudit).EndInit();
        tabAttachments.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentsContent).EndInit();
        pnlAttachmentsContent.ResumeLayout(false);
        pnlAttachmentsContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlObservationsSection).EndInit();
        pnlObservationsSection.ResumeLayout(false);
        pnlObservationsSection.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memSupplierObservations.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlDocumentsSection).EndInit();
        pnlDocumentsSection.ResumeLayout(false);
        pnlDocumentsSection.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentActions).EndInit();
        pnlAttachmentActions.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grdAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentPath.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentCategory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentExpirationDate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memAttachmentDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentPreview).EndInit();
        pnlAttachmentPreview.ResumeLayout(false);
        pnlAttachmentPreview.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlHeader).EndInit();
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)txtSupplierCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSupplierActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBusinessName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtTradeName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueDocumentType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDocumentNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePersonType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtMainContact.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPhone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtEmail.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePaymentCondition.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierCategory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memShortObservation.Properties).EndInit();
        ResumeLayout(false);
    }

    private PanelControl pnlMain;
    private PanelControl pnlHeader;
    private LabelControl lblTitle;
    private LabelControl lblSupplierCode;
    private TextEdit txtSupplierCode;
    private LabelControl lblSupplierActive;
    private ToggleSwitch tglSupplierActive;
    private LabelControl lblBusinessName;
    private TextEdit txtBusinessName;
    private LabelControl lblTradeName;
    private TextEdit txtTradeName;
    private LabelControl lblDocumentType;
    private LookUpEdit lueDocumentType;
    private LabelControl lblDocumentNumber;
    private TextEdit txtDocumentNumber;
    private LabelControl lblPersonType;
    private LookUpEdit luePersonType;
    private LabelControl lblSupplierType;
    private LookUpEdit lueSupplierType;
    private LabelControl lblMainContact;
    private TextEdit txtMainContact;
    private LabelControl lblPhone;
    private TextEdit txtPhone;
    private LabelControl lblEmail;
    private TextEdit txtEmail;
    private LabelControl lblCurrency;
    private LookUpEdit lueCurrency;
    private LabelControl lblPaymentCondition;
    private LookUpEdit luePaymentCondition;
    private LabelControl lblSupplierCategory;
    private LookUpEdit lueSupplierCategory;
    private LabelControl lblShortObservation;
    private MemoEdit memShortObservation;
    private XtraTabControl tabSupplier;
    private XtraTabPage tabGeneral;
    private XtraTabPage tabContacts;
    private XtraTabPage tabAddresses;
    private XtraTabPage tabPurchases;
    private XtraTabPage tabBanks;
    private XtraTabPage tabWithholdings;
    private XtraTabPage tabAccounting;
    private XtraTabPage tabSap;
    private XtraTabPage tabAttachments;
    private PanelControl pnlGeneralContent;
    private LabelControl lblCountry;
    private LookUpEdit lueCountry;
    private LabelControl lblProvinceCity;
    private TextEdit txtProvinceCity;
    private LabelControl lblWebsite;
    private TextEdit txtWebsite;
    private LabelControl lblRegistrationDate;
    private DateEdit dteRegistrationDate;
    private LabelControl lblCreditLimit;
    private SpinEdit spnCreditLimit;
    private LabelControl lblPaymentTermDays;
    private SpinEdit spnPaymentTermDays;
    private LabelControl lblAlternateCurrency;
    private LookUpEdit lueAlternateCurrency;
    private LabelControl lblInternalClassification;
    private LookUpEdit lueInternalClassification;
    private LabelControl lblSupplierSegment;
    private LookUpEdit lueSupplierSegment;
    private LabelControl lblActiveForPurchases;
    private ToggleSwitch tglActiveForPurchases;
    private LabelControl lblActiveForPurchasesValue;
    private LabelControl lblSubjectToWithholding;
    private ToggleSwitch tglSubjectToWithholding;
    private LabelControl lblSubjectToWithholdingValue;
    private LabelControl lblHandlesCredit;
    private ToggleSwitch tglHandlesCredit;
    private LabelControl lblHandlesCreditValue;
    private LabelControl lblBlocked;
    private ToggleSwitch tglBlocked;
    private LabelControl lblBlockedValue;
    private LabelControl lblGeneralComments;
    private MemoEdit memGeneralComments;
    private PanelControl pnlContactsContent;
    private PanelControl pnlContactsActions;
    private SimpleButton btnAddContact;
    private SimpleButton btnEditContact;
    private SimpleButton btnDeleteContact;
    private SimpleButton btnSetDefaultContact;
    private GridControl grdContacts;
    private GridView gvContacts;
    private GridColumn colContactFullName;
    private GridColumn colContactPosition;
    private GridColumn colContactDepartment;
    private GridColumn colContactIsPrimary;
    private GridColumn colContactIsActive;
    private PanelControl pnlAddressesContent;
    private PanelControl pnlAddressesActions;
    private SimpleButton btnAddAddress;
    private SimpleButton btnEditAddress;
    private SimpleButton btnDeleteAddress;
    private SimpleButton btnDuplicateAddress;
    private SimpleButton btnSetDefaultAddress;
    private GridControl grdAddresses;
    private GridView gvAddresses;
    private GridColumn colAddressType;
    private GridColumn colAddressCode;
    private GridColumn colAddressFullAddress;
    private GridColumn colAddressProvinceCity;
    private GridColumn colAddressCountry;
    private GridColumn colAddressReference;
    private GridColumn colAddressIsPrimary;
    private GridColumn colAddressIsActive;
    private PanelControl pnlPurchasesContent;
    private LabelControl lblPurchasePaymentCondition;
    private LookUpEdit luePurchasePaymentCondition;
    private LabelControl lblPurchasePriceList;
    private LookUpEdit luePurchasePriceList;
    private LabelControl lblDeliveryTermDays;
    private SpinEdit spnDeliveryTermDays;
    private LabelControl lblIncoterm;
    private LookUpEdit lueIncoterm;
    private LabelControl lblPurchaseCurrency;
    private LookUpEdit luePurchaseCurrency;
    private LabelControl lblCommercialDiscountPercent;
    private SpinEdit spnCommercialDiscountPercent;
    private LabelControl lblPurchaseSupplierType;
    private LookUpEdit luePurchaseSupplierType;
    private LabelControl lblAssignedBuyer;
    private LookUpEdit lueAssignedBuyer;
    private LabelControl lblSuggestedCostCenter;
    private LookUpEdit lueSuggestedCostCenter;
    private LabelControl lblPreferredWarehouse;
    private LookUpEdit luePreferredWarehouse;
    private LabelControl lblAverageDeliveryDays;
    private SpinEdit spnAverageDeliveryDays;
    private LabelControl lblMinimumOrderAmount;
    private SpinEdit spnMinimumOrderAmount;
    private LabelControl lblMinimumOrderQuantity;
    private SpinEdit spnMinimumOrderQuantity;
    private LabelControl lblLeadTimeDays;
    private SpinEdit spnLeadTimeDays;
    private LabelControl lblDeliveryToleranceDays;
    private SpinEdit spnDeliveryToleranceDays;
    private LabelControl lblRequiresPurchaseOrder;
    private ToggleSwitch tglRequiresPurchaseOrder;
    private LabelControl lblRequiresPurchaseOrderValue;
    private LabelControl lblSubjectToEvaluation;
    private ToggleSwitch tglSubjectToEvaluation;
    private LabelControl lblSubjectToEvaluationValue;
    private LabelControl lblActiveForImport;
    private ToggleSwitch tglActiveForImport;
    private LabelControl lblActiveForImportValue;
    private LabelControl lblAllowsUrgentPurchases;
    private ToggleSwitch tglAllowsUrgentPurchases;
    private LabelControl lblAllowsUrgentPurchasesValue;
    private LabelControl lblPurchaseHistoryTitle;
    private GridControl grdPurchaseHistory;
    private GridView gvPurchaseHistory;
    private GridColumn colPurchaseDate;
    private GridColumn colPurchaseDocumentNumber;
    private GridColumn colPurchaseAmount;
    private GridColumn colPurchaseCurrency;
    private GridColumn colPurchaseAverageDeliveryDays;
    private PanelControl pnlPurchasesLast12Months;
    private LabelControl lblPurchasesLast12MonthsCaption;
    private LabelControl lblPurchasesLast12MonthsValue;
    private PanelControl pnlAveragePurchase;
    private LabelControl lblAveragePurchaseCaption;
    private LabelControl lblAveragePurchaseValue;
    private PanelControl pnlAverageDelivery12Months;
    private LabelControl lblAverageDelivery12MonthsCaption;
    private LabelControl lblAverageDelivery12MonthsValue;
    private PanelControl pnlPurchaseOrdersLast12Months;
    private LabelControl lblPurchaseOrdersLast12MonthsCaption;
    private LabelControl lblPurchaseOrdersLast12MonthsValue;
    private PanelControl pnlBanksContent;
    private PanelControl pnlBanksActions;
    private SimpleButton btnAddBankAccount;
    private SimpleButton btnEditBankAccount;
    private SimpleButton btnDeleteBankAccount;
    private SimpleButton btnSetDefaultBankAccount;
    private GridControl grdBankAccounts;
    private GridView gvBankAccounts;
    private GridColumn colBankName;
    private GridColumn colBankAccountType;
    private GridColumn colBankAccountNumber;
    private GridColumn colBankCurrency;
    private GridColumn colBankSwiftBic;
    private GridColumn colBankCciIban;
    private GridColumn colBankAccountHolder;
    private GridColumn colBankIsDefault;
    private GridColumn colBankIsActive;
    private LabelControl lblBankAccountsTotal;
    private PanelControl pnlWithholdingsContent;
    private PanelControl pnlWithholdingsGeneral;
    private LabelControl lblWithholdingAgent;
    private ToggleSwitch tglWithholdingAgent;
    private LabelControl lblWithholdingAgentValue;
    private LabelControl lblGeneralWithholdingType;
    private LookUpEdit lueGeneralWithholdingType;
    private LabelControl lblBaseWithholdingPercent;
    private SpinEdit spnBaseWithholdingPercent;
    private LabelControl lblWithholdingEffectiveDate;
    private DateEdit dteWithholdingEffectiveDate;
    private LabelControl lblWithholdingResolutionNumber;
    private TextEdit txtWithholdingResolutionNumber;
    private LabelControl lblWithholdsVat;
    private ToggleSwitch tglWithholdsVat;
    private LabelControl lblWithholdsVatValue;
    private LabelControl lblWithholdsIncomeTax;
    private ToggleSwitch tglWithholdsIncomeTax;
    private LabelControl lblWithholdsIncomeTaxValue;
    private LabelControl lblIssuesElectronicReceipts;
    private ToggleSwitch tglIssuesElectronicReceipts;
    private LabelControl lblIssuesElectronicReceiptsValue;
    private LabelControl lblSubjectToPerception;
    private ToggleSwitch tglSubjectToPerception;
    private LabelControl lblSubjectToPerceptionValue;
    private PanelControl pnlWithholdingsActions;
    private SimpleButton btnAddWithholding;
    private SimpleButton btnEditWithholding;
    private SimpleButton btnDeleteWithholding;
    private SimpleButton btnSetDefaultWithholding;
    private GridControl grdWithholdings;
    private GridView gvWithholdings;
    private GridColumn colWithholdingDocument;
    private GridColumn colWithholdingType;
    private GridColumn colWithholdingValidity;
    private GridColumn colWithholdingIsDefault;
    private GridColumn colWithholdingStatus;
    private PanelControl pnlAccountingContent;
    private PanelControl pnlAccountingGeneral;
    private LabelControl lblDefaultProject;
    private LookUpEdit lueDefaultProject;
    private LabelControl lblFiscalCondition;
    private LookUpEdit lueFiscalCondition;
    private LabelControl lblThirdPartyType;
    private LookUpEdit lueThirdPartyType;
    private LabelControl lblAutomaticAccounting;
    private ToggleSwitch tglAutomaticAccounting;
    private LabelControl lblAutomaticAccountingValue;
    private LabelControl lblRequiresReconciliation;
    private ToggleSwitch tglRequiresReconciliation;
    private LabelControl lblRequiresReconciliationValue;
    private LabelControl lblHandlesAdvances;
    private ToggleSwitch tglHandlesAdvances;
    private LabelControl lblHandlesAdvancesValue;
    private LabelControl lblAccountingBlocked;
    private ToggleSwitch tglAccountingBlocked;
    private LabelControl lblAccountingBlockedValue;
    private PanelControl pnlAccountingActions;
    private SimpleButton btnAddAccountingAccount;
    private SimpleButton btnEditAccountingAccount;
    private SimpleButton btnDeleteAccountingAccount;
    private SimpleButton btnSetDefaultAccountingAccount;
    private GridControl grdAccountingAccounts;
    private GridView gvAccountingAccounts;
    private GridColumn colAccountingAccountType;
    private GridColumn colAccountingAccountCodeName;
    private GridColumn colAccountingDimension1;
    private GridColumn colAccountingDimension2;
    private GridColumn colAccountingDimension3;
    private GridColumn colAccountingDimension4;
    private GridColumn colAccountingDimension5;
    private GridColumn colAccountingIsDefault;
    private GridColumn colAccountingIsActive;
    private PanelControl pnlSapContent;
    private PanelControl pnlSapSyncData;
    private LabelControl lblSapSynchronized;
    private ToggleSwitch tglSapSynchronized;
    private LabelControl lblSapSynchronizedValue;
    private LabelControl lblSapIntegrationValid;
    private ToggleSwitch tglSapIntegrationValid;
    private LabelControl lblSapIntegrationValidValue;
    private LabelControl lblSapErrorBlocked;
    private ToggleSwitch tglSapErrorBlocked;
    private LabelControl lblSapErrorBlockedValue;
    private LabelControl lblSapAutoUpdate;
    private ToggleSwitch tglSapAutoUpdate;
    private LabelControl lblSapAutoUpdateValue;
    private LabelControl lblSapLastSync;
    private TextEdit txtSapLastSync;
    private LabelControl lblSapLastSyncUser;
    private TextEdit txtSapLastSyncUser;
    private LabelControl lblSapDataOrigin;
    private TextEdit txtSapDataOrigin;
    private LabelControl lblSapIntegrationStatus;
    private TextEdit txtSapIntegrationStatus;
    private PanelControl pnlSapAudit;
    private LabelControl lblSapAuditTitle;
    private GridControl grdSapAudit;
    private GridView gvSapAudit;
    private GridColumn colSapAuditDate;
    private GridColumn colSapAuditAction;
    private GridColumn colSapAuditResult;
    private GridColumn colSapAuditUser;
    private GridColumn colSapAuditMessage;
    private PanelControl pnlAttachmentsContent;
    private PanelControl pnlObservationsSection;
    private LabelControl lblSupplierObservationsTitle;
    private MemoEdit memSupplierObservations;
    private PanelControl pnlDocumentsSection;
    private LabelControl lblAttachmentsTitle;
    private PanelControl pnlAttachmentActions;
    private SimpleButton btnAttachDocument;
    private SimpleButton btnDownloadDocument;
    private SimpleButton btnViewDocument;
    private SimpleButton btnDeleteDocument;
    private GridControl grdAttachments;
    private GridView gvAttachments;
    private GridColumn colAttachmentDocumentType;
    private GridColumn colAttachmentFileName;
    private GridColumn colAttachmentUploadDate;
    private GridColumn colAttachmentUser;
    private GridColumn colAttachmentFileSize;
    private GridColumn colAttachmentStatus;
    private LabelControl lblAttachmentPath;
    private TextEdit txtAttachmentPath;
    private LabelControl lblAttachmentCategory;
    private TextEdit txtAttachmentCategory;
    private LabelControl lblAttachmentExpirationDate;
    private TextEdit txtAttachmentExpirationDate;
    private LabelControl lblAttachmentDescription;
    private MemoEdit memAttachmentDescription;
    private PanelControl pnlAttachmentPreview;
    private LabelControl lblAttachmentPreviewTitle;
    private LabelControl lblAttachmentPreview;
}

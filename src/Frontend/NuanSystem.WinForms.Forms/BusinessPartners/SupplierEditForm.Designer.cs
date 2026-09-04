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
        lblCountry = new LabelControl();
        lueCountry = new LookUpEdit();
        lblProvinceCity = new LabelControl();
        txtProvinceCity = new TextEdit();
        lblRegistrationDate = new LabelControl();
        dteRegistrationDate = new DateEdit();
        lblPaymentTermDays = new LabelControl();
        spnPaymentTermDays = new SpinEdit();
        lblGeneralComments = new LabelControl();
        memGeneralComments = new MemoEdit();
        lblSupplierCode = new LabelControl();
        txtSupplierCode = new TextEdit();
        lblMasterSyncStatus = new LabelControl();
        lblMasterSyncMessage = new LabelControl();
        tabSupplier = new XtraTabControl();
        tabGeneral = new XtraTabPage();
        pnlGeneralContent = new PanelControl();
        lblWebsite = new LabelControl();
        txtWebsite = new TextEdit();
        lblDocumentType = new LabelControl();
        lblActiveForPurchases = new LabelControl();
        lueDocumentType = new LookUpEdit();
        lblDocumentNumber = new LabelControl();
        tglActiveForPurchases = new ToggleSwitch();
        txtDocumentNumber = new TextEdit();
        lblSubjectToWithholding = new LabelControl();
        lblPersonType = new LabelControl();
        lblSupplierCategory = new LabelControl();
        lueCurrency = new LookUpEdit();
        lblShortObservation = new LabelControl();
        lueSupplierCategory = new LookUpEdit();
        memShortObservation = new MemoEdit();
        lblCurrency = new LabelControl();
        tglSubjectToWithholding = new ToggleSwitch();
        luePersonType = new LookUpEdit();
        lblHandlesCredit = new LabelControl();
        lblSupplierType = new LabelControl();
        tglHandlesCredit = new ToggleSwitch();
        lueSupplierType = new LookUpEdit();
        lblBlocked = new LabelControl();
        lblMainContact = new LabelControl();
        tglBlocked = new ToggleSwitch();
        txtMainContact = new TextEdit();
        txtEmail = new TextEdit();
        lblPhone = new LabelControl();
        lblEmail = new LabelControl();
        txtPhone = new TextEdit();
        lblSupplierClass = new LabelControl();
        lueSupplierClass = new LookUpEdit();
        lblEconomicActivity = new LabelControl();
        lueEconomicActivity = new LookUpEdit();
        lblSupplierZone = new LabelControl();
        lueSupplierZone = new LookUpEdit();
        lblSupplyMethod = new LabelControl();
        lueSupplyMethod = new LookUpEdit();
        tabContacts = new XtraTabPage();
        pnlContactsContent = new PanelControl();
        grdContacts = new GridControl();
        gvContacts = new GridView();
        colContactFullName = new GridColumn();
        colContactPosition = new GridColumn();
        colContactDepartment = new GridColumn();
        colContactIsPrimary = new GridColumn();
        colContactIsActive = new GridColumn();
        btnAddContact = new SimpleButton();
        btnSetDefaultContact = new SimpleButton();
        btnEditContact = new SimpleButton();
        btnDeleteContact = new SimpleButton();
        tabAddresses = new XtraTabPage();
        pnlAddressesContent = new PanelControl();
        btnAddAddress = new SimpleButton();
        btnEditAddress = new SimpleButton();
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
        btnSetDefaultAddress = new SimpleButton();
        btnDeleteAddress = new SimpleButton();
        btnDuplicateAddress = new SimpleButton();
        tabPurchases = new XtraTabPage();
        pnlPurchasesContent = new PanelControl();
        lblPurchasePaymentCondition = new LabelControl();
        luePurchasePaymentCondition = new LookUpEdit();
        lblCreditLimit = new LabelControl();
        lblPurchasePriceList = new LabelControl();
        luePurchasePriceList = new LookUpEdit();
        spnCreditLimit = new SpinEdit();
        lblDeliveryTermDays = new LabelControl();
        spnDeliveryTermDays = new SpinEdit();
        lblIncoterm = new LabelControl();
        lueIncoterm = new LookUpEdit();
        lblCommercialDiscountPercent = new LabelControl();
        spnCommercialDiscountPercent = new SpinEdit();
        lblAssignedBuyer = new LabelControl();
        lueAssignedBuyer = new LookUpEdit();
        lblPreferredWarehouse = new LabelControl();
        luePreferredWarehouse = new LookUpEdit();
        lblPurchaseCurrency = new LabelControl();
        luePurchaseCurrency = new LookUpEdit();
        lblPurchaseSupplierType = new LabelControl();
        luePurchaseSupplierType = new LookUpEdit();
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
        lblSubjectToEvaluation = new LabelControl();
        tglSubjectToEvaluation = new ToggleSwitch();
        lblActiveForImport = new LabelControl();
        tglActiveForImport = new ToggleSwitch();
        lblAllowsUrgentPurchases = new LabelControl();
        tglAllowsUrgentPurchases = new ToggleSwitch();
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
        btnAddBankAccount = new SimpleButton();
        btnEditBankAccount = new SimpleButton();
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
        btnDeleteBankAccount = new SimpleButton();
        btnSetDefaultBankAccount = new SimpleButton();
        tabWithholdings = new XtraTabPage();
        pnlWithholdingsContent = new PanelControl();
        lblWithholdingAgent = new LabelControl();
        tglWithholdingAgent = new ToggleSwitch();
        btnAddWithholding = new SimpleButton();
        lblGeneralWithholdingType = new LabelControl();
        lueGeneralWithholdingType = new LookUpEdit();
        btnEditWithholding = new SimpleButton();
        lblWithholdingResolutionNumber = new LabelControl();
        grdWithholdings = new GridControl();
        gvWithholdings = new GridView();
        colWithholdingDocument = new GridColumn();
        colWithholdingType = new GridColumn();
        colWithholdingValidity = new GridColumn();
        colWithholdingIsDefault = new GridColumn();
        colWithholdingStatus = new GridColumn();
        txtWithholdingResolutionNumber = new TextEdit();
        btnSetDefaultWithholding = new SimpleButton();
        lblWithholdsVat = new LabelControl();
        btnDeleteWithholding = new SimpleButton();
        tglWithholdsVat = new ToggleSwitch();
        tglSubjectToPerception = new ToggleSwitch();
        lblWithholdsIncomeTax = new LabelControl();
        lblSubjectToPerception = new LabelControl();
        tglWithholdsIncomeTax = new ToggleSwitch();
        tglIssuesElectronicReceipts = new ToggleSwitch();
        lblIssuesElectronicReceipts = new LabelControl();
        tabAccounting = new XtraTabPage();
        pnlAccountingContent = new PanelControl();
        btnAddAccountingAccount = new SimpleButton();
        btnEditAccountingAccount = new SimpleButton();
        lblDefaultProject = new LabelControl();
        lueDefaultProject = new LookUpEdit();
        btnDeleteAccountingAccount = new SimpleButton();
        lblFiscalCondition = new LabelControl();
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
        btnSetDefaultAccountingAccount = new SimpleButton();
        lueFiscalCondition = new LookUpEdit();
        lblAccountingBlockedValue = new LabelControl();
        lblThirdPartyType = new LabelControl();
        tglAccountingBlocked = new ToggleSwitch();
        lueThirdPartyType = new LookUpEdit();
        lblAccountingBlocked = new LabelControl();
        lblAutomaticAccounting = new LabelControl();
        lblHandlesAdvancesValue = new LabelControl();
        tglAutomaticAccounting = new ToggleSwitch();
        tglHandlesAdvances = new ToggleSwitch();
        lblAutomaticAccountingValue = new LabelControl();
        lblHandlesAdvances = new LabelControl();
        lblRequiresReconciliation = new LabelControl();
        lblRequiresReconciliationValue = new LabelControl();
        tglRequiresReconciliation = new ToggleSwitch();
        tabSap = new XtraTabPage();
        pnlSapContent = new PanelControl();
        lblSapSynchronized = new LabelControl();
        tglSapSynchronized = new ToggleSwitch();
        lblSapAuditTitle = new LabelControl();
        lblSapSynchronizedValue = new LabelControl();
        grdSapAudit = new GridControl();
        gvSapAudit = new GridView();
        colSapAuditDate = new GridColumn();
        colSapAuditAction = new GridColumn();
        colSapAuditResult = new GridColumn();
        colSapAuditUser = new GridColumn();
        colSapAuditMessage = new GridColumn();
        lblSapIntegrationValid = new LabelControl();
        tglSapIntegrationValid = new ToggleSwitch();
        txtSapIntegrationStatus = new TextEdit();
        lblSapIntegrationValidValue = new LabelControl();
        lblSapIntegrationStatus = new LabelControl();
        lblSapErrorBlocked = new LabelControl();
        txtSapDataOrigin = new TextEdit();
        tglSapErrorBlocked = new ToggleSwitch();
        lblSapDataOrigin = new LabelControl();
        lblSapErrorBlockedValue = new LabelControl();
        txtSapLastSyncUser = new TextEdit();
        lblSapAutoUpdate = new LabelControl();
        lblSapLastSyncUser = new LabelControl();
        tglSapAutoUpdate = new ToggleSwitch();
        txtSapLastSync = new TextEdit();
        lblSapAutoUpdateValue = new LabelControl();
        lblSapLastSync = new LabelControl();
        tabAttachments = new XtraTabPage();
        pnlAttachmentsContent = new PanelControl();
        btnAttachDocument = new SimpleButton();
        btnDownloadDocument = new SimpleButton();
        lblAttachmentsTitle = new LabelControl();
        btnViewDocument = new SimpleButton();
        btnDeleteDocument = new SimpleButton();
        lblSupplierObservationsTitle = new LabelControl();
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
        memAttachmentDescription = new MemoEdit();
        lblAttachmentCategory = new LabelControl();
        lblAttachmentDescription = new LabelControl();
        txtAttachmentCategory = new TextEdit();
        txtAttachmentExpirationDate = new TextEdit();
        lblAttachmentExpirationDate = new LabelControl();
        lblSupplierActive = new LabelControl();
        tglSupplierActive = new ToggleSwitch();
        txtTradeName = new TextEdit();
        lblSupplierSegment = new LabelControl();
        lblTradeName = new LabelControl();
        lblInternalClassification = new LabelControl();
        txtBusinessName = new TextEdit();
        lueSupplierSegment = new LookUpEdit();
        lueInternalClassification = new LookUpEdit();
        lblBusinessName = new LabelControl();
        btnCancel = new SimpleButton();
        btnSave = new SimpleButton();
        memSupplierObservations = new MemoEdit();
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtProvinceCity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteRegistrationDate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dteRegistrationDate.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnPaymentTermDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memGeneralComments.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tabSupplier).BeginInit();
        tabSupplier.SuspendLayout();
        tabGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlGeneralContent).BeginInit();
        pnlGeneralContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)txtWebsite.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueDocumentType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglActiveForPurchases.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDocumentNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierCategory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memShortObservation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSubjectToWithholding.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePersonType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglHandlesCredit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglBlocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtMainContact.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtEmail.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPhone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierClass.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueEconomicActivity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierZone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplyMethod.Properties).BeginInit();
        tabContacts.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlContactsContent).BeginInit();
        pnlContactsContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdContacts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvContacts).BeginInit();
        tabAddresses.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAddressesContent).BeginInit();
        pnlAddressesContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdAddresses).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvAddresses).BeginInit();
        tabPurchases.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlPurchasesContent).BeginInit();
        pnlPurchasesContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)luePurchasePaymentCondition.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchasePriceList.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditLimit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnDeliveryTermDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueIncoterm.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCommercialDiscountPercent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAssignedBuyer.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePreferredWarehouse.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseSupplierType.Properties).BeginInit();
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
        ((System.ComponentModel.ISupportInitialize)grdBankAccounts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvBankAccounts).BeginInit();
        tabWithholdings.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlWithholdingsContent).BeginInit();
        pnlWithholdingsContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglWithholdingAgent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueGeneralWithholdingType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdWithholdings).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvWithholdings).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtWithholdingResolutionNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglWithholdsVat.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSubjectToPerception.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglWithholdsIncomeTax.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglIssuesElectronicReceipts.Properties).BeginInit();
        tabAccounting.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAccountingContent).BeginInit();
        pnlAccountingContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueDefaultProject.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdAccountingAccounts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvAccountingAccounts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCondition.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAccountingBlocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueThirdPartyType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglAutomaticAccounting.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglHandlesAdvances.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresReconciliation.Properties).BeginInit();
        tabSap.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSapContent).BeginInit();
        pnlSapContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)tglSapSynchronized.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdSapAudit).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvSapAudit).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSapIntegrationValid.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapIntegrationStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapDataOrigin.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSapErrorBlocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastSyncUser.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSapAutoUpdate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastSync.Properties).BeginInit();
        tabAttachments.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentsContent).BeginInit();
        pnlAttachmentsContent.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentPath.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memAttachmentDescription.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentCategory.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentExpirationDate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tglSupplierActive.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtTradeName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBusinessName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierSegment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueInternalClassification.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memSupplierObservations.Properties).BeginInit();
        SuspendLayout();
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
        // lblSupplierCode
        //
        lblSupplierCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierCode.Appearance.Options.UseFont = true;
        lblSupplierCode.Location = new Point(12, 12);
        lblSupplierCode.Name = "lblSupplierCode";
        lblSupplierCode.Size = new Size(42, 15);
        lblSupplierCode.TabIndex = 18;
        lblSupplierCode.Text = "Código:";
        //
        // txtSupplierCode
        //
        txtSupplierCode.Location = new Point(150, 9);
        txtSupplierCode.Name = "txtSupplierCode";
        txtSupplierCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSupplierCode.Properties.Appearance.Options.UseFont = true;
        txtSupplierCode.Properties.NullValuePrompt = "Se asigna al guardar";
        txtSupplierCode.Properties.ReadOnly = true;
        txtSupplierCode.Properties.ShowNullValuePromptWhenFocused = true;
        txtSupplierCode.Size = new Size(170, 22);
        txtSupplierCode.TabIndex = 20;
        //
        // lblMasterSyncStatus
        //
        lblMasterSyncStatus.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblMasterSyncStatus.Appearance.ForeColor = Color.FromArgb(0, 120, 215);
        lblMasterSyncStatus.Appearance.Options.UseFont = true;
        lblMasterSyncStatus.Appearance.Options.UseForeColor = true;
        lblMasterSyncStatus.AutoSizeMode = LabelAutoSizeMode.None;
        lblMasterSyncStatus.Location = new Point(700, 12);
        lblMasterSyncStatus.Name = "lblMasterSyncStatus";
        lblMasterSyncStatus.Size = new Size(150, 20);
        lblMasterSyncStatus.TabIndex = 33;
        lblMasterSyncStatus.Text = "Aceptado";
        //
        // lblMasterSyncMessage
        //
        lblMasterSyncMessage.Appearance.Font = new Font("Segoe UI", 8F);
        lblMasterSyncMessage.Appearance.ForeColor = Color.FromArgb(87, 96, 111);
        lblMasterSyncMessage.Appearance.Options.UseFont = true;
        lblMasterSyncMessage.Appearance.Options.UseForeColor = true;
        lblMasterSyncMessage.AutoSizeMode = LabelAutoSizeMode.None;
        lblMasterSyncMessage.Location = new Point(700, 33);
        lblMasterSyncMessage.Name = "lblMasterSyncMessage";
        lblMasterSyncMessage.Size = new Size(420, 34);
        lblMasterSyncMessage.TabIndex = 34;
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
        tabSupplier.Location = new Point(0, 149);
        tabSupplier.Name = "tabSupplier";
        tabSupplier.SelectedTabPage = tabGeneral;
        tabSupplier.Size = new Size(1530, 383);
        tabSupplier.TabIndex = 19;
        tabSupplier.TabPages.AddRange(new XtraTabPage[] { tabGeneral, tabContacts, tabAddresses, tabPurchases, tabBanks, tabWithholdings, tabAccounting, tabSap, tabAttachments });
        //
        // tabGeneral
        //
        tabGeneral.Controls.Add(pnlGeneralContent);
        tabGeneral.Name = "tabGeneral";
        tabGeneral.Size = new Size(1528, 356);
        tabGeneral.Text = "General";
        //
        // pnlGeneralContent
        //
        pnlGeneralContent.BorderStyle = BorderStyles.Simple;
        pnlGeneralContent.Controls.Add(lblWebsite);
        pnlGeneralContent.Controls.Add(txtWebsite);
        pnlGeneralContent.Controls.Add(lblDocumentType);
        pnlGeneralContent.Controls.Add(lblActiveForPurchases);
        pnlGeneralContent.Controls.Add(lueDocumentType);
        pnlGeneralContent.Controls.Add(lblDocumentNumber);
        pnlGeneralContent.Controls.Add(tglActiveForPurchases);
        pnlGeneralContent.Controls.Add(txtDocumentNumber);
        pnlGeneralContent.Controls.Add(lblSubjectToWithholding);
        pnlGeneralContent.Controls.Add(lblPersonType);
        pnlGeneralContent.Controls.Add(lblSupplierCategory);
        pnlGeneralContent.Controls.Add(lueCurrency);
        pnlGeneralContent.Controls.Add(lblShortObservation);
        pnlGeneralContent.Controls.Add(lueSupplierCategory);
        pnlGeneralContent.Controls.Add(memShortObservation);
        pnlGeneralContent.Controls.Add(lblCurrency);
        pnlGeneralContent.Controls.Add(tglSubjectToWithholding);
        pnlGeneralContent.Controls.Add(luePersonType);
        pnlGeneralContent.Controls.Add(lblHandlesCredit);
        pnlGeneralContent.Controls.Add(lblSupplierType);
        pnlGeneralContent.Controls.Add(tglHandlesCredit);
        pnlGeneralContent.Controls.Add(lueSupplierType);
        pnlGeneralContent.Controls.Add(lblBlocked);
        pnlGeneralContent.Controls.Add(lblMainContact);
        pnlGeneralContent.Controls.Add(tglBlocked);
        pnlGeneralContent.Controls.Add(txtMainContact);
        pnlGeneralContent.Controls.Add(txtEmail);
        pnlGeneralContent.Controls.Add(lblPhone);
        pnlGeneralContent.Controls.Add(lblEmail);
        pnlGeneralContent.Controls.Add(txtPhone);
        pnlGeneralContent.Controls.Add(lblSupplierClass);
        pnlGeneralContent.Controls.Add(lueSupplierClass);
        pnlGeneralContent.Controls.Add(lblEconomicActivity);
        pnlGeneralContent.Controls.Add(lueEconomicActivity);
        pnlGeneralContent.Controls.Add(lblSupplierZone);
        pnlGeneralContent.Controls.Add(lueSupplierZone);
        pnlGeneralContent.Controls.Add(lblSupplyMethod);
        pnlGeneralContent.Controls.Add(lueSupplyMethod);
        pnlGeneralContent.Dock = DockStyle.Fill;
        pnlGeneralContent.Location = new Point(0, 0);
        pnlGeneralContent.Name = "pnlGeneralContent";
        pnlGeneralContent.Size = new Size(1528, 356);
        pnlGeneralContent.TabIndex = 0;
        //
        // lblWebsite
        //
        lblWebsite.Appearance.Font = new Font("Segoe UI", 9F);
        lblWebsite.Appearance.Options.UseFont = true;
        lblWebsite.Location = new Point(11, 159);
        lblWebsite.Name = "lblWebsite";
        lblWebsite.Size = new Size(53, 15);
        lblWebsite.TabIndex = 4;
        lblWebsite.Text = "Sitio Web:";
        //
        // txtWebsite
        //
        txtWebsite.Location = new Point(149, 156);
        txtWebsite.Name = "txtWebsite";
        txtWebsite.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtWebsite.Properties.Appearance.Options.UseFont = true;
        txtWebsite.Size = new Size(524, 22);
        txtWebsite.TabIndex = 5;
        //
        // lblDocumentType
        //
        lblDocumentType.Appearance.Font = new Font("Segoe UI", 9F);
        lblDocumentType.Appearance.Options.UseFont = true;
        lblDocumentType.Location = new Point(11, 19);
        lblDocumentType.Name = "lblDocumentType";
        lblDocumentType.Size = new Size(109, 15);
        lblDocumentType.TabIndex = 9;
        lblDocumentType.Text = "Tipo de Documento:";
        //
        // lblActiveForPurchases
        //
        lblActiveForPurchases.Appearance.Font = new Font("Segoe UI", 9F);
        lblActiveForPurchases.Appearance.Options.UseFont = true;
        lblActiveForPurchases.Location = new Point(855, 19);
        lblActiveForPurchases.Name = "lblActiveForPurchases";
        lblActiveForPurchases.Size = new Size(114, 15);
        lblActiveForPurchases.TabIndex = 18;
        lblActiveForPurchases.Text = "Activo para Compras:";
        //
        // lueDocumentType
        //
        lueDocumentType.Location = new Point(149, 16);
        lueDocumentType.Name = "lueDocumentType";
        lueDocumentType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueDocumentType.Properties.Appearance.Options.UseFont = true;
        lueDocumentType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueDocumentType.Properties.NullText = "";
        lueDocumentType.Size = new Size(170, 22);
        lueDocumentType.TabIndex = 10;
        //
        // lblDocumentNumber
        //
        lblDocumentNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblDocumentNumber.Appearance.Options.UseFont = true;
        lblDocumentNumber.Location = new Point(336, 19);
        lblDocumentNumber.Name = "lblDocumentNumber";
        lblDocumentNumber.Size = new Size(109, 15);
        lblDocumentNumber.TabIndex = 11;
        lblDocumentNumber.Text = "RUC / Identificación:";
        //
        // tglActiveForPurchases
        //
        tglActiveForPurchases.Location = new Point(1017, 18);
        tglActiveForPurchases.Name = "tglActiveForPurchases";
        tglActiveForPurchases.Properties.OffText = "";
        tglActiveForPurchases.Properties.OnText = "";
        tglActiveForPurchases.Size = new Size(70, 18);
        tglActiveForPurchases.TabIndex = 19;
        //
        // txtDocumentNumber
        //
        txtDocumentNumber.Location = new Point(463, 16);
        txtDocumentNumber.Name = "txtDocumentNumber";
        txtDocumentNumber.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtDocumentNumber.Properties.Appearance.Options.UseFont = true;
        txtDocumentNumber.Size = new Size(210, 22);
        txtDocumentNumber.TabIndex = 12;
        //
        // lblSubjectToWithholding
        //
        lblSubjectToWithholding.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubjectToWithholding.Appearance.Options.UseFont = true;
        lblSubjectToWithholding.Location = new Point(855, 75);
        lblSubjectToWithholding.Name = "lblSubjectToWithholding";
        lblSubjectToWithholding.Size = new Size(101, 15);
        lblSubjectToWithholding.TabIndex = 21;
        lblSubjectToWithholding.Text = "Sujeto a Retención:";
        //
        // lblPersonType
        //
        lblPersonType.Appearance.Font = new Font("Segoe UI", 9F);
        lblPersonType.Appearance.Options.UseFont = true;
        lblPersonType.Location = new Point(11, 47);
        lblPersonType.Name = "lblPersonType";
        lblPersonType.Size = new Size(88, 15);
        lblPersonType.TabIndex = 13;
        lblPersonType.Text = "Tipo de Persona:";
        //
        // lblSupplierCategory
        //
        lblSupplierCategory.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierCategory.Appearance.Options.UseFont = true;
        lblSupplierCategory.Location = new Point(11, 187);
        lblSupplierCategory.Name = "lblSupplierCategory";
        lblSupplierCategory.Size = new Size(132, 15);
        lblSupplierCategory.TabIndex = 27;
        lblSupplierCategory.Text = "Categoría / Clasificación:";
        //
        // lueCurrency
        //
        lueCurrency.Location = new Point(463, 100);
        lueCurrency.Name = "lueCurrency";
        lueCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCurrency.Properties.Appearance.Options.UseFont = true;
        lueCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCurrency.Properties.NullText = "";
        lueCurrency.Size = new Size(210, 22);
        lueCurrency.TabIndex = 24;
        //
        // lblShortObservation
        //
        lblShortObservation.Appearance.Font = new Font("Segoe UI", 9F);
        lblShortObservation.Appearance.Options.UseFont = true;
        lblShortObservation.Location = new Point(10, 214);
        lblShortObservation.Name = "lblShortObservation";
        lblShortObservation.Size = new Size(99, 15);
        lblShortObservation.TabIndex = 29;
        lblShortObservation.Text = "Observación corta:";
        //
        // lueSupplierCategory
        //
        lueSupplierCategory.Location = new Point(149, 184);
        lueSupplierCategory.Name = "lueSupplierCategory";
        lueSupplierCategory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierCategory.Properties.Appearance.Options.UseFont = true;
        lueSupplierCategory.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierCategory.Properties.NullText = "";
        lueSupplierCategory.Size = new Size(170, 22);
        lueSupplierCategory.TabIndex = 28;
        //
        // memShortObservation
        //
        memShortObservation.Location = new Point(149, 212);
        memShortObservation.Name = "memShortObservation";
        memShortObservation.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memShortObservation.Properties.Appearance.Options.UseFont = true;
        memShortObservation.Size = new Size(524, 78);
        memShortObservation.TabIndex = 30;
        //
        // lblCurrency
        //
        lblCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblCurrency.Appearance.Options.UseFont = true;
        lblCurrency.Location = new Point(336, 103);
        lblCurrency.Name = "lblCurrency";
        lblCurrency.Size = new Size(47, 15);
        lblCurrency.TabIndex = 23;
        lblCurrency.Text = "Moneda:";
        //
        // tglSubjectToWithholding
        //
        tglSubjectToWithholding.Location = new Point(1017, 74);
        tglSubjectToWithholding.Name = "tglSubjectToWithholding";
        tglSubjectToWithholding.Properties.OffText = "";
        tglSubjectToWithholding.Properties.OnText = "";
        tglSubjectToWithholding.Size = new Size(70, 18);
        tglSubjectToWithholding.TabIndex = 22;
        //
        // luePersonType
        //
        luePersonType.Location = new Point(149, 44);
        luePersonType.Name = "luePersonType";
        luePersonType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePersonType.Properties.Appearance.Options.UseFont = true;
        luePersonType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePersonType.Properties.NullText = "";
        luePersonType.Size = new Size(170, 22);
        luePersonType.TabIndex = 14;
        //
        // lblHandlesCredit
        //
        lblHandlesCredit.Appearance.Font = new Font("Segoe UI", 9F);
        lblHandlesCredit.Appearance.Options.UseFont = true;
        lblHandlesCredit.Location = new Point(855, 47);
        lblHandlesCredit.Name = "lblHandlesCredit";
        lblHandlesCredit.Size = new Size(84, 15);
        lblHandlesCredit.TabIndex = 24;
        lblHandlesCredit.Text = "Maneja Crédito:";
        //
        // lblSupplierType
        //
        lblSupplierType.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierType.Appearance.Options.UseFont = true;
        lblSupplierType.Location = new Point(336, 47);
        lblSupplierType.Name = "lblSupplierType";
        lblSupplierType.Size = new Size(100, 15);
        lblSupplierType.TabIndex = 15;
        lblSupplierType.Text = "Tipo de Proveedor:";
        //
        // tglHandlesCredit
        //
        tglHandlesCredit.Location = new Point(1017, 46);
        tglHandlesCredit.Name = "tglHandlesCredit";
        tglHandlesCredit.Properties.OffText = "";
        tglHandlesCredit.Properties.OnText = "";
        tglHandlesCredit.Size = new Size(70, 18);
        tglHandlesCredit.TabIndex = 25;
        //
        // lueSupplierType
        //
        lueSupplierType.Location = new Point(463, 44);
        lueSupplierType.Name = "lueSupplierType";
        lueSupplierType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierType.Properties.Appearance.Options.UseFont = true;
        lueSupplierType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierType.Properties.NullText = "";
        lueSupplierType.Size = new Size(210, 22);
        lueSupplierType.TabIndex = 16;
        //
        // lblBlocked
        //
        lblBlocked.Appearance.Font = new Font("Segoe UI", 9F);
        lblBlocked.Appearance.Options.UseFont = true;
        lblBlocked.Location = new Point(855, 103);
        lblBlocked.Name = "lblBlocked";
        lblBlocked.Size = new Size(60, 15);
        lblBlocked.TabIndex = 27;
        lblBlocked.Text = "Bloqueado:";
        //
        // lblMainContact
        //
        lblMainContact.Appearance.Font = new Font("Segoe UI", 9F);
        lblMainContact.Appearance.Options.UseFont = true;
        lblMainContact.Location = new Point(11, 75);
        lblMainContact.Name = "lblMainContact";
        lblMainContact.Size = new Size(101, 15);
        lblMainContact.TabIndex = 17;
        lblMainContact.Text = "Contacto Principal:";
        //
        // tglBlocked
        //
        tglBlocked.Location = new Point(1017, 102);
        tglBlocked.Name = "tglBlocked";
        tglBlocked.Properties.OffText = "";
        tglBlocked.Properties.OnText = "";
        tglBlocked.Size = new Size(70, 18);
        tglBlocked.TabIndex = 28;
        //
        // txtMainContact
        //
        txtMainContact.Location = new Point(149, 72);
        txtMainContact.Name = "txtMainContact";
        txtMainContact.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtMainContact.Properties.Appearance.Options.UseFont = true;
        txtMainContact.Size = new Size(524, 22);
        txtMainContact.TabIndex = 18;
        //
        // txtEmail
        //
        txtEmail.Location = new Point(149, 128);
        txtEmail.Name = "txtEmail";
        txtEmail.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtEmail.Properties.Appearance.Options.UseFont = true;
        txtEmail.Size = new Size(524, 22);
        txtEmail.TabIndex = 22;
        //
        // lblPhone
        //
        lblPhone.Appearance.Font = new Font("Segoe UI", 9F);
        lblPhone.Appearance.Options.UseFont = true;
        lblPhone.Location = new Point(11, 103);
        lblPhone.Name = "lblPhone";
        lblPhone.Size = new Size(50, 15);
        lblPhone.TabIndex = 19;
        lblPhone.Text = "Teléfono:";
        //
        // lblEmail
        //
        lblEmail.Appearance.Font = new Font("Segoe UI", 9F);
        lblEmail.Appearance.Options.UseFont = true;
        lblEmail.Location = new Point(10, 131);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(101, 15);
        lblEmail.TabIndex = 21;
        lblEmail.Text = "Correo Electrónico:";
        //
        // txtPhone
        //
        txtPhone.Location = new Point(149, 100);
        txtPhone.Name = "txtPhone";
        txtPhone.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPhone.Properties.Appearance.Options.UseFont = true;
        txtPhone.Size = new Size(170, 22);
        txtPhone.TabIndex = 20;
        //
        // lblSupplierClass
        //
        lblSupplierClass.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierClass.Appearance.Options.UseFont = true;
        lblSupplierClass.Location = new Point(855, 131);
        lblSupplierClass.Name = "lblSupplierClass";
        lblSupplierClass.Size = new Size(86, 15);
        lblSupplierClass.TabIndex = 31;
        lblSupplierClass.Text = "Clase proveedor:";
        //
        // lueSupplierClass
        //
        lueSupplierClass.Location = new Point(1017, 128);
        lueSupplierClass.Name = "lueSupplierClass";
        lueSupplierClass.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierClass.Properties.Appearance.Options.UseFont = true;
        lueSupplierClass.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierClass.Properties.NullText = "";
        lueSupplierClass.Size = new Size(250, 22);
        lueSupplierClass.TabIndex = 32;
        //
        // lblEconomicActivity
        //
        lblEconomicActivity.Appearance.Font = new Font("Segoe UI", 9F);
        lblEconomicActivity.Appearance.Options.UseFont = true;
        lblEconomicActivity.Location = new Point(855, 159);
        lblEconomicActivity.Name = "lblEconomicActivity";
        lblEconomicActivity.Size = new Size(100, 15);
        lblEconomicActivity.TabIndex = 33;
        lblEconomicActivity.Text = "Actividad económica:";
        //
        // lueEconomicActivity
        //
        lueEconomicActivity.Location = new Point(1017, 156);
        lueEconomicActivity.Name = "lueEconomicActivity";
        lueEconomicActivity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueEconomicActivity.Properties.Appearance.Options.UseFont = true;
        lueEconomicActivity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueEconomicActivity.Properties.NullText = "";
        lueEconomicActivity.Size = new Size(250, 22);
        lueEconomicActivity.TabIndex = 34;
        //
        // lblSupplierZone
        //
        lblSupplierZone.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierZone.Appearance.Options.UseFont = true;
        lblSupplierZone.Location = new Point(855, 187);
        lblSupplierZone.Name = "lblSupplierZone";
        lblSupplierZone.Size = new Size(31, 15);
        lblSupplierZone.TabIndex = 35;
        lblSupplierZone.Text = "Zona:";
        //
        // lueSupplierZone
        //
        lueSupplierZone.Location = new Point(1017, 184);
        lueSupplierZone.Name = "lueSupplierZone";
        lueSupplierZone.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierZone.Properties.Appearance.Options.UseFont = true;
        lueSupplierZone.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierZone.Properties.NullText = "";
        lueSupplierZone.Size = new Size(250, 22);
        lueSupplierZone.TabIndex = 36;
        //
        // lblSupplyMethod
        //
        lblSupplyMethod.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplyMethod.Appearance.Options.UseFont = true;
        lblSupplyMethod.Location = new Point(855, 215);
        lblSupplyMethod.Name = "lblSupplyMethod";
        lblSupplyMethod.Size = new Size(134, 15);
        lblSupplyMethod.TabIndex = 37;
        lblSupplyMethod.Text = "Método abastecimiento:";
        //
        // lueSupplyMethod
        //
        lueSupplyMethod.Location = new Point(1017, 212);
        lueSupplyMethod.Name = "lueSupplyMethod";
        lueSupplyMethod.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplyMethod.Properties.Appearance.Options.UseFont = true;
        lueSupplyMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplyMethod.Properties.NullText = "";
        lueSupplyMethod.Size = new Size(250, 22);
        lueSupplyMethod.TabIndex = 38;
        //
        // tabContacts
        //
        tabContacts.Controls.Add(pnlContactsContent);
        tabContacts.Name = "tabContacts";
        tabContacts.Size = new Size(1528, 356);
        tabContacts.Text = "Contactos";
        //
        // pnlContactsContent
        //
        pnlContactsContent.BorderStyle = BorderStyles.Simple;
        pnlContactsContent.Controls.Add(grdContacts);
        pnlContactsContent.Controls.Add(btnAddContact);
        pnlContactsContent.Controls.Add(btnSetDefaultContact);
        pnlContactsContent.Controls.Add(btnEditContact);
        pnlContactsContent.Controls.Add(btnDeleteContact);
        pnlContactsContent.Dock = DockStyle.Fill;
        pnlContactsContent.Location = new Point(0, 0);
        pnlContactsContent.Name = "pnlContactsContent";
        pnlContactsContent.Size = new Size(1528, 356);
        pnlContactsContent.TabIndex = 0;
        //
        // grdContacts
        //
        grdContacts.Location = new Point(11, 50);
        grdContacts.MainView = gvContacts;
        grdContacts.Name = "grdContacts";
        grdContacts.Size = new Size(1456, 262);
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
        // btnAddContact
        //
        btnAddContact.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddContact.Appearance.Options.UseFont = true;
        btnAddContact.Location = new Point(11, 16);
        btnAddContact.Name = "btnAddContact";
        btnAddContact.Size = new Size(86, 28);
        btnAddContact.TabIndex = 0;
        btnAddContact.Text = "Agregar";
        //
        // btnSetDefaultContact
        //
        btnSetDefaultContact.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetDefaultContact.Appearance.Options.UseFont = true;
        btnSetDefaultContact.Location = new Point(305, 16);
        btnSetDefaultContact.Name = "btnSetDefaultContact";
        btnSetDefaultContact.Size = new Size(112, 28);
        btnSetDefaultContact.TabIndex = 3;
        btnSetDefaultContact.Text = "Predeterminar";
        //
        // btnEditContact
        //
        btnEditContact.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditContact.Appearance.Options.UseFont = true;
        btnEditContact.Location = new Point(109, 16);
        btnEditContact.Name = "btnEditContact";
        btnEditContact.Size = new Size(86, 28);
        btnEditContact.TabIndex = 1;
        btnEditContact.Text = "Editar";
        //
        // btnDeleteContact
        //
        btnDeleteContact.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteContact.Appearance.Options.UseFont = true;
        btnDeleteContact.Location = new Point(207, 16);
        btnDeleteContact.Name = "btnDeleteContact";
        btnDeleteContact.Size = new Size(86, 28);
        btnDeleteContact.TabIndex = 2;
        btnDeleteContact.Text = "Eliminar";
        //
        // tabAddresses
        //
        tabAddresses.Controls.Add(pnlAddressesContent);
        tabAddresses.Name = "tabAddresses";
        tabAddresses.Size = new Size(1528, 356);
        tabAddresses.Text = "Direcciones";
        //
        // pnlAddressesContent
        //
        pnlAddressesContent.BorderStyle = BorderStyles.Simple;
        pnlAddressesContent.Controls.Add(btnAddAddress);
        pnlAddressesContent.Controls.Add(btnEditAddress);
        pnlAddressesContent.Controls.Add(grdAddresses);
        pnlAddressesContent.Controls.Add(btnSetDefaultAddress);
        pnlAddressesContent.Controls.Add(btnDeleteAddress);
        pnlAddressesContent.Controls.Add(btnDuplicateAddress);
        pnlAddressesContent.Dock = DockStyle.Fill;
        pnlAddressesContent.Location = new Point(0, 0);
        pnlAddressesContent.Name = "pnlAddressesContent";
        pnlAddressesContent.Size = new Size(1528, 356);
        pnlAddressesContent.TabIndex = 0;
        //
        // btnAddAddress
        //
        btnAddAddress.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddAddress.Appearance.Options.UseFont = true;
        btnAddAddress.Location = new Point(11, 15);
        btnAddAddress.Name = "btnAddAddress";
        btnAddAddress.Size = new Size(86, 28);
        btnAddAddress.TabIndex = 0;
        btnAddAddress.Text = "Agregar";
        //
        // btnEditAddress
        //
        btnEditAddress.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditAddress.Appearance.Options.UseFont = true;
        btnEditAddress.Location = new Point(109, 15);
        btnEditAddress.Name = "btnEditAddress";
        btnEditAddress.Size = new Size(86, 28);
        btnEditAddress.TabIndex = 1;
        btnEditAddress.Text = "Editar";
        //
        // grdAddresses
        //
        grdAddresses.Location = new Point(11, 49);
        grdAddresses.MainView = gvAddresses;
        grdAddresses.Name = "grdAddresses";
        grdAddresses.Size = new Size(1470, 262);
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
        // btnSetDefaultAddress
        //
        btnSetDefaultAddress.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetDefaultAddress.Appearance.Options.UseFont = true;
        btnSetDefaultAddress.Location = new Point(403, 15);
        btnSetDefaultAddress.Name = "btnSetDefaultAddress";
        btnSetDefaultAddress.Size = new Size(112, 28);
        btnSetDefaultAddress.TabIndex = 4;
        btnSetDefaultAddress.Text = "Predeterminada";
        //
        // btnDeleteAddress
        //
        btnDeleteAddress.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteAddress.Appearance.Options.UseFont = true;
        btnDeleteAddress.Location = new Point(207, 15);
        btnDeleteAddress.Name = "btnDeleteAddress";
        btnDeleteAddress.Size = new Size(86, 28);
        btnDeleteAddress.TabIndex = 2;
        btnDeleteAddress.Text = "Eliminar";
        //
        // btnDuplicateAddress
        //
        btnDuplicateAddress.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDuplicateAddress.Appearance.Options.UseFont = true;
        btnDuplicateAddress.Location = new Point(305, 15);
        btnDuplicateAddress.Name = "btnDuplicateAddress";
        btnDuplicateAddress.Size = new Size(86, 28);
        btnDuplicateAddress.TabIndex = 3;
        btnDuplicateAddress.Text = "Duplicar";
        //
        // tabPurchases
        //
        tabPurchases.Controls.Add(pnlPurchasesContent);
        tabPurchases.Name = "tabPurchases";
        tabPurchases.Size = new Size(1528, 356);
        tabPurchases.Text = "Compras";
        //
        // pnlPurchasesContent
        //
        pnlPurchasesContent.BorderStyle = BorderStyles.Simple;
        pnlPurchasesContent.Controls.Add(lblPurchasePaymentCondition);
        pnlPurchasesContent.Controls.Add(luePurchasePaymentCondition);
        pnlPurchasesContent.Controls.Add(lblCreditLimit);
        pnlPurchasesContent.Controls.Add(lblPurchasePriceList);
        pnlPurchasesContent.Controls.Add(luePurchasePriceList);
        pnlPurchasesContent.Controls.Add(spnCreditLimit);
        pnlPurchasesContent.Controls.Add(lblDeliveryTermDays);
        pnlPurchasesContent.Controls.Add(spnDeliveryTermDays);
        pnlPurchasesContent.Controls.Add(lblIncoterm);
        pnlPurchasesContent.Controls.Add(lueIncoterm);
        pnlPurchasesContent.Controls.Add(lblCommercialDiscountPercent);
        pnlPurchasesContent.Controls.Add(spnCommercialDiscountPercent);
        pnlPurchasesContent.Controls.Add(lblAssignedBuyer);
        pnlPurchasesContent.Controls.Add(lueAssignedBuyer);
        pnlPurchasesContent.Controls.Add(lblPreferredWarehouse);
        pnlPurchasesContent.Controls.Add(luePreferredWarehouse);
        pnlPurchasesContent.Controls.Add(lblPurchaseCurrency);
        pnlPurchasesContent.Controls.Add(luePurchaseCurrency);
        pnlPurchasesContent.Controls.Add(lblPurchaseSupplierType);
        pnlPurchasesContent.Controls.Add(luePurchaseSupplierType);
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
        pnlPurchasesContent.Controls.Add(lblSubjectToEvaluation);
        pnlPurchasesContent.Controls.Add(tglSubjectToEvaluation);
        pnlPurchasesContent.Controls.Add(lblActiveForImport);
        pnlPurchasesContent.Controls.Add(tglActiveForImport);
        pnlPurchasesContent.Controls.Add(lblAllowsUrgentPurchases);
        pnlPurchasesContent.Controls.Add(tglAllowsUrgentPurchases);
        pnlPurchasesContent.Controls.Add(lblPurchaseHistoryTitle);
        pnlPurchasesContent.Controls.Add(grdPurchaseHistory);
        pnlPurchasesContent.Controls.Add(pnlPurchasesLast12Months);
        pnlPurchasesContent.Controls.Add(pnlAveragePurchase);
        pnlPurchasesContent.Controls.Add(pnlAverageDelivery12Months);
        pnlPurchasesContent.Controls.Add(pnlPurchaseOrdersLast12Months);
        pnlPurchasesContent.Dock = DockStyle.Fill;
        pnlPurchasesContent.Location = new Point(0, 0);
        pnlPurchasesContent.Name = "pnlPurchasesContent";
        pnlPurchasesContent.Size = new Size(1528, 356);
        pnlPurchasesContent.TabIndex = 0;
        //
        // lblPurchasePaymentCondition
        //
        lblPurchasePaymentCondition.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchasePaymentCondition.Appearance.Options.UseFont = true;
        lblPurchasePaymentCondition.Location = new Point(11, 19);
        lblPurchasePaymentCondition.Name = "lblPurchasePaymentCondition";
        lblPurchasePaymentCondition.Size = new Size(158, 15);
        lblPurchasePaymentCondition.TabIndex = 0;
        lblPurchasePaymentCondition.Text = "Condición de Pago (Compra):";
        //
        // luePurchasePaymentCondition
        //
        luePurchasePaymentCondition.Location = new Point(175, 16);
        luePurchasePaymentCondition.Name = "luePurchasePaymentCondition";
        luePurchasePaymentCondition.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchasePaymentCondition.Properties.Appearance.Options.UseFont = true;
        luePurchasePaymentCondition.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchasePaymentCondition.Properties.NullText = "";
        luePurchasePaymentCondition.Size = new Size(210, 22);
        luePurchasePaymentCondition.TabIndex = 1;
        //
        // lblCreditLimit
        //
        lblCreditLimit.Appearance.Font = new Font("Segoe UI", 9F);
        lblCreditLimit.Appearance.Options.UseFont = true;
        lblCreditLimit.Location = new Point(11, 215);
        lblCreditLimit.Name = "lblCreditLimit";
        lblCreditLimit.Size = new Size(94, 15);
        lblCreditLimit.TabIndex = 8;
        lblCreditLimit.Text = "Límite de Crédito:";
        //
        // lblPurchasePriceList
        //
        lblPurchasePriceList.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchasePriceList.Appearance.Options.UseFont = true;
        lblPurchasePriceList.Location = new Point(11, 47);
        lblPurchasePriceList.Name = "lblPurchasePriceList";
        lblPurchasePriceList.Size = new Size(146, 15);
        lblPurchasePriceList.TabIndex = 2;
        lblPurchasePriceList.Text = "Lista de Precios de Compra:";
        //
        // luePurchasePriceList
        //
        luePurchasePriceList.Location = new Point(175, 44);
        luePurchasePriceList.Name = "luePurchasePriceList";
        luePurchasePriceList.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchasePriceList.Properties.Appearance.Options.UseFont = true;
        luePurchasePriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchasePriceList.Properties.NullText = "";
        luePurchasePriceList.Size = new Size(210, 22);
        luePurchasePriceList.TabIndex = 3;
        //
        // spnCreditLimit
        //
        spnCreditLimit.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnCreditLimit.Location = new Point(175, 212);
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
        spnCreditLimit.Size = new Size(130, 22);
        spnCreditLimit.TabIndex = 9;
        //
        // lblDeliveryTermDays
        //
        lblDeliveryTermDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblDeliveryTermDays.Appearance.Options.UseFont = true;
        lblDeliveryTermDays.Location = new Point(11, 75);
        lblDeliveryTermDays.Name = "lblDeliveryTermDays";
        lblDeliveryTermDays.Size = new Size(122, 15);
        lblDeliveryTermDays.TabIndex = 4;
        lblDeliveryTermDays.Text = "Plazo de Entrega (días):";
        //
        // spnDeliveryTermDays
        //
        spnDeliveryTermDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnDeliveryTermDays.Location = new Point(175, 72);
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
        lblIncoterm.Location = new Point(11, 103);
        lblIncoterm.Name = "lblIncoterm";
        lblIncoterm.Size = new Size(51, 15);
        lblIncoterm.TabIndex = 6;
        lblIncoterm.Text = "Incoterm:";
        //
        // lueIncoterm
        //
        lueIncoterm.Location = new Point(175, 100);
        lueIncoterm.Name = "lueIncoterm";
        lueIncoterm.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueIncoterm.Properties.Appearance.Options.UseFont = true;
        lueIncoterm.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueIncoterm.Properties.NullText = "";
        lueIncoterm.Size = new Size(210, 22);
        lueIncoterm.TabIndex = 7;
        //
        // lblCommercialDiscountPercent
        //
        lblCommercialDiscountPercent.Appearance.Font = new Font("Segoe UI", 9F);
        lblCommercialDiscountPercent.Appearance.Options.UseFont = true;
        lblCommercialDiscountPercent.Location = new Point(11, 131);
        lblCommercialDiscountPercent.Name = "lblCommercialDiscountPercent";
        lblCommercialDiscountPercent.Size = new Size(137, 15);
        lblCommercialDiscountPercent.TabIndex = 10;
        lblCommercialDiscountPercent.Text = "Descuento Comercial (%):";
        //
        // spnCommercialDiscountPercent
        //
        spnCommercialDiscountPercent.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnCommercialDiscountPercent.Location = new Point(175, 128);
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
        // lblAssignedBuyer
        //
        lblAssignedBuyer.Appearance.Font = new Font("Segoe UI", 9F);
        lblAssignedBuyer.Appearance.Options.UseFont = true;
        lblAssignedBuyer.Location = new Point(11, 159);
        lblAssignedBuyer.Name = "lblAssignedBuyer";
        lblAssignedBuyer.Size = new Size(117, 15);
        lblAssignedBuyer.TabIndex = 14;
        lblAssignedBuyer.Text = "Comprador Asignado:";
        //
        // lueAssignedBuyer
        //
        lueAssignedBuyer.Location = new Point(175, 156);
        lueAssignedBuyer.Name = "lueAssignedBuyer";
        lueAssignedBuyer.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAssignedBuyer.Properties.Appearance.Options.UseFont = true;
        lueAssignedBuyer.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAssignedBuyer.Properties.NullText = "";
        lueAssignedBuyer.Size = new Size(210, 22);
        lueAssignedBuyer.TabIndex = 15;
        //
        // lblPreferredWarehouse
        //
        lblPreferredWarehouse.Appearance.Font = new Font("Segoe UI", 9F);
        lblPreferredWarehouse.Appearance.Options.UseFont = true;
        lblPreferredWarehouse.Location = new Point(11, 189);
        lblPreferredWarehouse.Name = "lblPreferredWarehouse";
        lblPreferredWarehouse.Size = new Size(93, 15);
        lblPreferredWarehouse.TabIndex = 18;
        lblPreferredWarehouse.Text = "Bodega Preferida:";
        //
        // luePreferredWarehouse
        //
        luePreferredWarehouse.Location = new Point(175, 184);
        luePreferredWarehouse.Name = "luePreferredWarehouse";
        luePreferredWarehouse.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePreferredWarehouse.Properties.Appearance.Options.UseFont = true;
        luePreferredWarehouse.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePreferredWarehouse.Properties.NullText = "";
        luePreferredWarehouse.Size = new Size(210, 22);
        luePreferredWarehouse.TabIndex = 19;
        //
        // lblPurchaseCurrency
        //
        lblPurchaseCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseCurrency.Appearance.Options.UseFont = true;
        lblPurchaseCurrency.Location = new Point(11, 243);
        lblPurchaseCurrency.Name = "lblPurchaseCurrency";
        lblPurchaseCurrency.Size = new Size(89, 15);
        lblPurchaseCurrency.TabIndex = 40;
        lblPurchaseCurrency.Text = "Moneda compra:";
        //
        // luePurchaseCurrency
        //
        luePurchaseCurrency.Location = new Point(175, 240);
        luePurchaseCurrency.Name = "luePurchaseCurrency";
        luePurchaseCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseCurrency.Properties.Appearance.Options.UseFont = true;
        luePurchaseCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseCurrency.Properties.NullText = "";
        luePurchaseCurrency.Size = new Size(210, 22);
        luePurchaseCurrency.TabIndex = 41;
        //
        // lblPurchaseSupplierType
        //
        lblPurchaseSupplierType.Appearance.Font = new Font("Segoe UI", 9F);
        lblPurchaseSupplierType.Appearance.Options.UseFont = true;
        lblPurchaseSupplierType.Location = new Point(11, 271);
        lblPurchaseSupplierType.Name = "lblPurchaseSupplierType";
        lblPurchaseSupplierType.Size = new Size(120, 15);
        lblPurchaseSupplierType.TabIndex = 42;
        lblPurchaseSupplierType.Text = "Tipo proveedor compra:";
        //
        // luePurchaseSupplierType
        //
        luePurchaseSupplierType.Location = new Point(175, 268);
        luePurchaseSupplierType.Name = "luePurchaseSupplierType";
        luePurchaseSupplierType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseSupplierType.Properties.Appearance.Options.UseFont = true;
        luePurchaseSupplierType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseSupplierType.Properties.NullText = "";
        luePurchaseSupplierType.Size = new Size(210, 22);
        luePurchaseSupplierType.TabIndex = 43;
        //
        // lblAverageDeliveryDays
        //
        lblAverageDeliveryDays.Appearance.Font = new Font("Segoe UI", 9F);
        lblAverageDeliveryDays.Appearance.Options.UseFont = true;
        lblAverageDeliveryDays.Location = new Point(413, 19);
        lblAverageDeliveryDays.Name = "lblAverageDeliveryDays";
        lblAverageDeliveryDays.Size = new Size(139, 15);
        lblAverageDeliveryDays.TabIndex = 20;
        lblAverageDeliveryDays.Text = "Días de Entrega Promedio:";
        //
        // spnAverageDeliveryDays
        //
        spnAverageDeliveryDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnAverageDeliveryDays.Location = new Point(583, 16);
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
        lblMinimumOrderAmount.Location = new Point(413, 47);
        lblMinimumOrderAmount.Name = "lblMinimumOrderAmount";
        lblMinimumOrderAmount.Size = new Size(136, 15);
        lblMinimumOrderAmount.TabIndex = 22;
        lblMinimumOrderAmount.Text = "Monto Mínimo de Orden:";
        //
        // spnMinimumOrderAmount
        //
        spnMinimumOrderAmount.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnMinimumOrderAmount.Location = new Point(583, 44);
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
        lblMinimumOrderQuantity.Location = new Point(413, 75);
        lblMinimumOrderQuantity.Name = "lblMinimumOrderQuantity";
        lblMinimumOrderQuantity.Size = new Size(80, 15);
        lblMinimumOrderQuantity.TabIndex = 24;
        lblMinimumOrderQuantity.Text = "Orden Mínima:";
        //
        // spnMinimumOrderQuantity
        //
        spnMinimumOrderQuantity.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnMinimumOrderQuantity.Location = new Point(583, 72);
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
        lblLeadTimeDays.Location = new Point(413, 103);
        lblLeadTimeDays.Name = "lblLeadTimeDays";
        lblLeadTimeDays.Size = new Size(90, 15);
        lblLeadTimeDays.TabIndex = 26;
        lblLeadTimeDays.Text = "Lead Time (días):";
        //
        // spnLeadTimeDays
        //
        spnLeadTimeDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnLeadTimeDays.Location = new Point(583, 100);
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
        lblDeliveryToleranceDays.Location = new Point(413, 131);
        lblDeliveryToleranceDays.Name = "lblDeliveryToleranceDays";
        lblDeliveryToleranceDays.Size = new Size(149, 15);
        lblDeliveryToleranceDays.TabIndex = 28;
        lblDeliveryToleranceDays.Text = "Tolerancia de Entrega (días):";
        //
        // spnDeliveryToleranceDays
        //
        spnDeliveryToleranceDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnDeliveryToleranceDays.Location = new Point(583, 128);
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
        lblRequiresPurchaseOrder.Location = new Point(413, 159);
        lblRequiresPurchaseOrder.Name = "lblRequiresPurchaseOrder";
        lblRequiresPurchaseOrder.Size = new Size(147, 15);
        lblRequiresPurchaseOrder.TabIndex = 30;
        lblRequiresPurchaseOrder.Text = "Requiere Orden de Compra:";
        //
        // tglRequiresPurchaseOrder
        //
        tglRequiresPurchaseOrder.Location = new Point(583, 158);
        tglRequiresPurchaseOrder.Name = "tglRequiresPurchaseOrder";
        tglRequiresPurchaseOrder.Properties.OffText = "";
        tglRequiresPurchaseOrder.Properties.OnText = "";
        tglRequiresPurchaseOrder.Size = new Size(62, 18);
        tglRequiresPurchaseOrder.TabIndex = 31;
        //
        // lblSubjectToEvaluation
        //
        lblSubjectToEvaluation.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubjectToEvaluation.Appearance.Options.UseFont = true;
        lblSubjectToEvaluation.Location = new Point(413, 187);
        lblSubjectToEvaluation.Name = "lblSubjectToEvaluation";
        lblSubjectToEvaluation.Size = new Size(105, 15);
        lblSubjectToEvaluation.TabIndex = 33;
        lblSubjectToEvaluation.Text = "Sujeto a Evaluación:";
        //
        // tglSubjectToEvaluation
        //
        tglSubjectToEvaluation.Location = new Point(583, 186);
        tglSubjectToEvaluation.Name = "tglSubjectToEvaluation";
        tglSubjectToEvaluation.Properties.OffText = "";
        tglSubjectToEvaluation.Properties.OnText = "";
        tglSubjectToEvaluation.Size = new Size(62, 18);
        tglSubjectToEvaluation.TabIndex = 34;
        //
        // lblActiveForImport
        //
        lblActiveForImport.Appearance.Font = new Font("Segoe UI", 9F);
        lblActiveForImport.Appearance.Options.UseFont = true;
        lblActiveForImport.Location = new Point(413, 215);
        lblActiveForImport.Name = "lblActiveForImport";
        lblActiveForImport.Size = new Size(131, 15);
        lblActiveForImport.TabIndex = 36;
        lblActiveForImport.Text = "Activo para Importación:";
        //
        // tglActiveForImport
        //
        tglActiveForImport.Location = new Point(583, 214);
        tglActiveForImport.Name = "tglActiveForImport";
        tglActiveForImport.Properties.OffText = "";
        tglActiveForImport.Properties.OnText = "";
        tglActiveForImport.Size = new Size(62, 18);
        tglActiveForImport.TabIndex = 37;
        //
        // lblAllowsUrgentPurchases
        //
        lblAllowsUrgentPurchases.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowsUrgentPurchases.Appearance.Options.UseFont = true;
        lblAllowsUrgentPurchases.Location = new Point(413, 243);
        lblAllowsUrgentPurchases.Name = "lblAllowsUrgentPurchases";
        lblAllowsUrgentPurchases.Size = new Size(145, 15);
        lblAllowsUrgentPurchases.TabIndex = 39;
        lblAllowsUrgentPurchases.Text = "Permite Compras Urgentes:";
        //
        // tglAllowsUrgentPurchases
        //
        tglAllowsUrgentPurchases.Location = new Point(583, 242);
        tglAllowsUrgentPurchases.Name = "tglAllowsUrgentPurchases";
        tglAllowsUrgentPurchases.Properties.OffText = "";
        tglAllowsUrgentPurchases.Properties.OnText = "";
        tglAllowsUrgentPurchases.Size = new Size(62, 18);
        tglAllowsUrgentPurchases.TabIndex = 40;
        //
        // lblPurchaseHistoryTitle
        //
        lblPurchaseHistoryTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblPurchaseHistoryTitle.Appearance.Options.UseFont = true;
        lblPurchaseHistoryTitle.Location = new Point(763, 19);
        lblPurchaseHistoryTitle.Name = "lblPurchaseHistoryTitle";
        lblPurchaseHistoryTitle.Size = new Size(242, 15);
        lblPurchaseHistoryTitle.TabIndex = 42;
        lblPurchaseHistoryTitle.Text = "Historial de Compras (Últimos 6 documentos)";
        //
        // grdPurchaseHistory
        //
        grdPurchaseHistory.Location = new Point(763, 40);
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
        pnlPurchasesLast12Months.Location = new Point(763, 238);
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
        pnlAveragePurchase.Location = new Point(889, 238);
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
        pnlAverageDelivery12Months.Location = new Point(1011, 238);
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
        pnlPurchaseOrdersLast12Months.Location = new Point(1141, 238);
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
        tabBanks.Size = new Size(1528, 356);
        tabBanks.Text = "Bancos";
        //
        // pnlBanksContent
        //
        pnlBanksContent.BorderStyle = BorderStyles.Simple;
        pnlBanksContent.Controls.Add(btnAddBankAccount);
        pnlBanksContent.Controls.Add(btnEditBankAccount);
        pnlBanksContent.Controls.Add(grdBankAccounts);
        pnlBanksContent.Controls.Add(lblBankAccountsTotal);
        pnlBanksContent.Controls.Add(btnDeleteBankAccount);
        pnlBanksContent.Controls.Add(btnSetDefaultBankAccount);
        pnlBanksContent.Dock = DockStyle.Fill;
        pnlBanksContent.Location = new Point(0, 0);
        pnlBanksContent.Name = "pnlBanksContent";
        pnlBanksContent.Size = new Size(1528, 356);
        pnlBanksContent.TabIndex = 0;
        //
        // btnAddBankAccount
        //
        btnAddBankAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddBankAccount.Appearance.Options.UseFont = true;
        btnAddBankAccount.Location = new Point(11, 16);
        btnAddBankAccount.Name = "btnAddBankAccount";
        btnAddBankAccount.Size = new Size(88, 28);
        btnAddBankAccount.TabIndex = 0;
        btnAddBankAccount.Text = "Agregar";
        //
        // btnEditBankAccount
        //
        btnEditBankAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditBankAccount.Appearance.Options.UseFont = true;
        btnEditBankAccount.Location = new Point(109, 16);
        btnEditBankAccount.Name = "btnEditBankAccount";
        btnEditBankAccount.Size = new Size(88, 28);
        btnEditBankAccount.TabIndex = 1;
        btnEditBankAccount.Text = "Editar";
        //
        // grdBankAccounts
        //
        grdBankAccounts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdBankAccounts.Location = new Point(11, 50);
        grdBankAccounts.MainView = gvBankAccounts;
        grdBankAccounts.Name = "grdBankAccounts";
        grdBankAccounts.Size = new Size(2830, 663);
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
        lblBankAccountsTotal.Location = new Point(14, 731);
        lblBankAccountsTotal.Name = "lblBankAccountsTotal";
        lblBankAccountsTotal.Size = new Size(103, 15);
        lblBankAccountsTotal.TabIndex = 2;
        lblBankAccountsTotal.Text = "Total de registros: 0";
        //
        // btnDeleteBankAccount
        //
        btnDeleteBankAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteBankAccount.Appearance.Options.UseFont = true;
        btnDeleteBankAccount.Location = new Point(207, 16);
        btnDeleteBankAccount.Name = "btnDeleteBankAccount";
        btnDeleteBankAccount.Size = new Size(88, 28);
        btnDeleteBankAccount.TabIndex = 2;
        btnDeleteBankAccount.Text = "Eliminar";
        //
        // btnSetDefaultBankAccount
        //
        btnSetDefaultBankAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetDefaultBankAccount.Appearance.Options.UseFont = true;
        btnSetDefaultBankAccount.Location = new Point(305, 16);
        btnSetDefaultBankAccount.Name = "btnSetDefaultBankAccount";
        btnSetDefaultBankAccount.Size = new Size(116, 28);
        btnSetDefaultBankAccount.TabIndex = 3;
        btnSetDefaultBankAccount.Text = "Predeterminada";
        //
        // tabWithholdings
        //
        tabWithholdings.Controls.Add(pnlWithholdingsContent);
        tabWithholdings.Name = "tabWithholdings";
        tabWithholdings.Size = new Size(1528, 356);
        tabWithholdings.Text = "Retenciones";
        //
        // pnlWithholdingsContent
        //
        pnlWithholdingsContent.BorderStyle = BorderStyles.Simple;
        pnlWithholdingsContent.Controls.Add(lblWithholdingAgent);
        pnlWithholdingsContent.Controls.Add(tglWithholdingAgent);
        pnlWithholdingsContent.Controls.Add(btnAddWithholding);
        pnlWithholdingsContent.Controls.Add(lblGeneralWithholdingType);
        pnlWithholdingsContent.Controls.Add(lueGeneralWithholdingType);
        pnlWithholdingsContent.Controls.Add(btnEditWithholding);
        pnlWithholdingsContent.Controls.Add(lblWithholdingResolutionNumber);
        pnlWithholdingsContent.Controls.Add(grdWithholdings);
        pnlWithholdingsContent.Controls.Add(txtWithholdingResolutionNumber);
        pnlWithholdingsContent.Controls.Add(btnSetDefaultWithholding);
        pnlWithholdingsContent.Controls.Add(lblWithholdsVat);
        pnlWithholdingsContent.Controls.Add(btnDeleteWithholding);
        pnlWithholdingsContent.Controls.Add(tglWithholdsVat);
        pnlWithholdingsContent.Controls.Add(tglSubjectToPerception);
        pnlWithholdingsContent.Controls.Add(lblWithholdsIncomeTax);
        pnlWithholdingsContent.Controls.Add(lblSubjectToPerception);
        pnlWithholdingsContent.Controls.Add(tglWithholdsIncomeTax);
        pnlWithholdingsContent.Controls.Add(tglIssuesElectronicReceipts);
        pnlWithholdingsContent.Controls.Add(lblIssuesElectronicReceipts);
        pnlWithholdingsContent.Dock = DockStyle.Fill;
        pnlWithholdingsContent.Location = new Point(0, 0);
        pnlWithholdingsContent.Name = "pnlWithholdingsContent";
        pnlWithholdingsContent.Size = new Size(1528, 356);
        pnlWithholdingsContent.TabIndex = 0;
        //
        // lblWithholdingAgent
        //
        lblWithholdingAgent.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdingAgent.Appearance.Options.UseFont = true;
        lblWithholdingAgent.Location = new Point(11, 21);
        lblWithholdingAgent.Name = "lblWithholdingAgent";
        lblWithholdingAgent.Size = new Size(113, 15);
        lblWithholdingAgent.TabIndex = 0;
        lblWithholdingAgent.Text = "Agente de Retención:";
        //
        // tglWithholdingAgent
        //
        tglWithholdingAgent.Location = new Point(155, 20);
        tglWithholdingAgent.Name = "tglWithholdingAgent";
        tglWithholdingAgent.Properties.OffText = "";
        tglWithholdingAgent.Properties.OnText = "";
        tglWithholdingAgent.Size = new Size(50, 18);
        tglWithholdingAgent.TabIndex = 1;
        //
        // btnAddWithholding
        //
        btnAddWithholding.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddWithholding.Appearance.Options.UseFont = true;
        btnAddWithholding.Location = new Point(14, 124);
        btnAddWithholding.Name = "btnAddWithholding";
        btnAddWithholding.Size = new Size(88, 28);
        btnAddWithholding.TabIndex = 0;
        btnAddWithholding.Text = "Agregar";
        //
        // lblGeneralWithholdingType
        //
        lblGeneralWithholdingType.Appearance.Font = new Font("Segoe UI", 9F);
        lblGeneralWithholdingType.Appearance.Options.UseFont = true;
        lblGeneralWithholdingType.Location = new Point(11, 51);
        lblGeneralWithholdingType.Name = "lblGeneralWithholdingType";
        lblGeneralWithholdingType.Size = new Size(99, 15);
        lblGeneralWithholdingType.TabIndex = 3;
        lblGeneralWithholdingType.Text = "Tipo de Retención:";
        //
        // lueGeneralWithholdingType
        //
        lueGeneralWithholdingType.Location = new Point(155, 48);
        lueGeneralWithholdingType.Name = "lueGeneralWithholdingType";
        lueGeneralWithholdingType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueGeneralWithholdingType.Properties.Appearance.Options.UseFont = true;
        lueGeneralWithholdingType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueGeneralWithholdingType.Properties.NullText = "";
        lueGeneralWithholdingType.Size = new Size(220, 22);
        lueGeneralWithholdingType.TabIndex = 4;
        //
        // btnEditWithholding
        //
        btnEditWithholding.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditWithholding.Appearance.Options.UseFont = true;
        btnEditWithholding.Location = new Point(112, 124);
        btnEditWithholding.Name = "btnEditWithholding";
        btnEditWithholding.Size = new Size(88, 28);
        btnEditWithholding.TabIndex = 1;
        btnEditWithholding.Text = "Editar";
        //
        // lblWithholdingResolutionNumber
        //
        lblWithholdingResolutionNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdingResolutionNumber.Appearance.Options.UseFont = true;
        lblWithholdingResolutionNumber.Location = new Point(11, 79);
        lblWithholdingResolutionNumber.Name = "lblWithholdingResolutionNumber";
        lblWithholdingResolutionNumber.Size = new Size(124, 15);
        lblWithholdingResolutionNumber.TabIndex = 9;
        lblWithholdingResolutionNumber.Text = "Número de Resolución:";
        //
        // grdWithholdings
        //
        grdWithholdings.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdWithholdings.Location = new Point(11, 158);
        grdWithholdings.MainView = gvWithholdings;
        grdWithholdings.Name = "grdWithholdings";
        grdWithholdings.Size = new Size(2828, 419);
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
        // txtWithholdingResolutionNumber
        //
        txtWithholdingResolutionNumber.Location = new Point(155, 76);
        txtWithholdingResolutionNumber.Name = "txtWithholdingResolutionNumber";
        txtWithholdingResolutionNumber.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtWithholdingResolutionNumber.Properties.Appearance.Options.UseFont = true;
        txtWithholdingResolutionNumber.Size = new Size(220, 22);
        txtWithholdingResolutionNumber.TabIndex = 10;
        //
        // btnSetDefaultWithholding
        //
        btnSetDefaultWithholding.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetDefaultWithholding.Appearance.Options.UseFont = true;
        btnSetDefaultWithholding.Location = new Point(308, 124);
        btnSetDefaultWithholding.Name = "btnSetDefaultWithholding";
        btnSetDefaultWithholding.Size = new Size(116, 28);
        btnSetDefaultWithholding.TabIndex = 3;
        btnSetDefaultWithholding.Text = "Predeterminar";
        //
        // lblWithholdsVat
        //
        lblWithholdsVat.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdsVat.Appearance.Options.UseFont = true;
        lblWithholdsVat.Location = new Point(441, 51);
        lblWithholdsVat.Name = "lblWithholdsVat";
        lblWithholdsVat.Size = new Size(63, 15);
        lblWithholdsVat.TabIndex = 11;
        lblWithholdsVat.Text = "Retiene IVA:";
        //
        // btnDeleteWithholding
        //
        btnDeleteWithholding.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteWithholding.Appearance.Options.UseFont = true;
        btnDeleteWithholding.Location = new Point(210, 124);
        btnDeleteWithholding.Name = "btnDeleteWithholding";
        btnDeleteWithholding.Size = new Size(88, 28);
        btnDeleteWithholding.TabIndex = 2;
        btnDeleteWithholding.Text = "Eliminar";
        //
        // tglWithholdsVat
        //
        tglWithholdsVat.Location = new Point(572, 50);
        tglWithholdsVat.Name = "tglWithholdsVat";
        tglWithholdsVat.Properties.OffText = "";
        tglWithholdsVat.Properties.OnText = "";
        tglWithholdsVat.Size = new Size(50, 18);
        tglWithholdsVat.TabIndex = 12;
        //
        // tglSubjectToPerception
        //
        tglSubjectToPerception.Location = new Point(572, 20);
        tglSubjectToPerception.Name = "tglSubjectToPerception";
        tglSubjectToPerception.Properties.OffText = "";
        tglSubjectToPerception.Properties.OnText = "";
        tglSubjectToPerception.Size = new Size(50, 18);
        tglSubjectToPerception.TabIndex = 21;
        //
        // lblWithholdsIncomeTax
        //
        lblWithholdsIncomeTax.Appearance.Font = new Font("Segoe UI", 9F);
        lblWithholdsIncomeTax.Appearance.Options.UseFont = true;
        lblWithholdsIncomeTax.Location = new Point(441, 79);
        lblWithholdsIncomeTax.Name = "lblWithholdsIncomeTax";
        lblWithholdsIncomeTax.Size = new Size(75, 15);
        lblWithholdsIncomeTax.TabIndex = 14;
        lblWithholdsIncomeTax.Text = "Retiene Renta:";
        //
        // lblSubjectToPerception
        //
        lblSubjectToPerception.Appearance.Font = new Font("Segoe UI", 9F);
        lblSubjectToPerception.Appearance.Options.UseFont = true;
        lblSubjectToPerception.Location = new Point(441, 21);
        lblSubjectToPerception.Name = "lblSubjectToPerception";
        lblSubjectToPerception.Size = new Size(107, 15);
        lblSubjectToPerception.TabIndex = 20;
        lblSubjectToPerception.Text = "Sujeto a Percepción:";
        //
        // tglWithholdsIncomeTax
        //
        tglWithholdsIncomeTax.Location = new Point(572, 78);
        tglWithholdsIncomeTax.Name = "tglWithholdsIncomeTax";
        tglWithholdsIncomeTax.Properties.OffText = "";
        tglWithholdsIncomeTax.Properties.OnText = "";
        tglWithholdsIncomeTax.Size = new Size(50, 18);
        tglWithholdsIncomeTax.TabIndex = 15;
        //
        // tglIssuesElectronicReceipts
        //
        tglIssuesElectronicReceipts.Location = new Point(948, 20);
        tglIssuesElectronicReceipts.Name = "tglIssuesElectronicReceipts";
        tglIssuesElectronicReceipts.Properties.OffText = "";
        tglIssuesElectronicReceipts.Properties.OnText = "";
        tglIssuesElectronicReceipts.Size = new Size(50, 18);
        tglIssuesElectronicReceipts.TabIndex = 18;
        //
        // lblIssuesElectronicReceipts
        //
        lblIssuesElectronicReceipts.Appearance.Font = new Font("Segoe UI", 9F);
        lblIssuesElectronicReceipts.Appearance.Options.UseFont = true;
        lblIssuesElectronicReceipts.Location = new Point(757, 21);
        lblIssuesElectronicReceipts.Name = "lblIssuesElectronicReceipts";
        lblIssuesElectronicReceipts.Size = new Size(182, 15);
        lblIssuesElectronicReceipts.TabIndex = 17;
        lblIssuesElectronicReceipts.Text = "Emite Comprobantes Electrónicos:";
        //
        // tabAccounting
        //
        tabAccounting.Controls.Add(pnlAccountingContent);
        tabAccounting.Name = "tabAccounting";
        tabAccounting.Size = new Size(1528, 356);
        tabAccounting.Text = "Contabilidad";
        //
        // pnlAccountingContent
        //
        pnlAccountingContent.BorderStyle = BorderStyles.Simple;
        pnlAccountingContent.Controls.Add(btnAddAccountingAccount);
        pnlAccountingContent.Controls.Add(btnEditAccountingAccount);
        pnlAccountingContent.Controls.Add(lblDefaultProject);
        pnlAccountingContent.Controls.Add(lueDefaultProject);
        pnlAccountingContent.Controls.Add(btnDeleteAccountingAccount);
        pnlAccountingContent.Controls.Add(lblFiscalCondition);
        pnlAccountingContent.Controls.Add(grdAccountingAccounts);
        pnlAccountingContent.Controls.Add(btnSetDefaultAccountingAccount);
        pnlAccountingContent.Controls.Add(lueFiscalCondition);
        pnlAccountingContent.Controls.Add(lblAccountingBlockedValue);
        pnlAccountingContent.Controls.Add(lblThirdPartyType);
        pnlAccountingContent.Controls.Add(tglAccountingBlocked);
        pnlAccountingContent.Controls.Add(lueThirdPartyType);
        pnlAccountingContent.Controls.Add(lblAccountingBlocked);
        pnlAccountingContent.Controls.Add(lblAutomaticAccounting);
        pnlAccountingContent.Controls.Add(lblHandlesAdvancesValue);
        pnlAccountingContent.Controls.Add(tglAutomaticAccounting);
        pnlAccountingContent.Controls.Add(tglHandlesAdvances);
        pnlAccountingContent.Controls.Add(lblAutomaticAccountingValue);
        pnlAccountingContent.Controls.Add(lblHandlesAdvances);
        pnlAccountingContent.Controls.Add(lblRequiresReconciliation);
        pnlAccountingContent.Controls.Add(lblRequiresReconciliationValue);
        pnlAccountingContent.Controls.Add(tglRequiresReconciliation);
        pnlAccountingContent.Dock = DockStyle.Fill;
        pnlAccountingContent.Location = new Point(0, 0);
        pnlAccountingContent.Name = "pnlAccountingContent";
        pnlAccountingContent.Size = new Size(1528, 356);
        pnlAccountingContent.TabIndex = 0;
        //
        // btnAddAccountingAccount
        //
        btnAddAccountingAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddAccountingAccount.Appearance.Options.UseFont = true;
        btnAddAccountingAccount.Location = new Point(14, 112);
        btnAddAccountingAccount.Name = "btnAddAccountingAccount";
        btnAddAccountingAccount.Size = new Size(88, 28);
        btnAddAccountingAccount.TabIndex = 0;
        btnAddAccountingAccount.Text = "Agregar";
        //
        // btnEditAccountingAccount
        //
        btnEditAccountingAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnEditAccountingAccount.Appearance.Options.UseFont = true;
        btnEditAccountingAccount.Location = new Point(112, 112);
        btnEditAccountingAccount.Name = "btnEditAccountingAccount";
        btnEditAccountingAccount.Size = new Size(88, 28);
        btnEditAccountingAccount.TabIndex = 1;
        btnEditAccountingAccount.Text = "Editar";
        //
        // lblDefaultProject
        //
        lblDefaultProject.Appearance.Font = new Font("Segoe UI", 9F);
        lblDefaultProject.Appearance.Options.UseFont = true;
        lblDefaultProject.Location = new Point(14, 20);
        lblDefaultProject.Name = "lblDefaultProject";
        lblDefaultProject.Size = new Size(115, 15);
        lblDefaultProject.TabIndex = 0;
        lblDefaultProject.Text = "Proyecto por Defecto:";
        //
        // lueDefaultProject
        //
        lueDefaultProject.Location = new Point(174, 17);
        lueDefaultProject.Name = "lueDefaultProject";
        lueDefaultProject.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueDefaultProject.Properties.Appearance.Options.UseFont = true;
        lueDefaultProject.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueDefaultProject.Properties.NullText = "";
        lueDefaultProject.Size = new Size(230, 22);
        lueDefaultProject.TabIndex = 1;
        //
        // btnDeleteAccountingAccount
        //
        btnDeleteAccountingAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteAccountingAccount.Appearance.Options.UseFont = true;
        btnDeleteAccountingAccount.Location = new Point(210, 112);
        btnDeleteAccountingAccount.Name = "btnDeleteAccountingAccount";
        btnDeleteAccountingAccount.Size = new Size(88, 28);
        btnDeleteAccountingAccount.TabIndex = 2;
        btnDeleteAccountingAccount.Text = "Eliminar";
        //
        // lblFiscalCondition
        //
        lblFiscalCondition.Appearance.Font = new Font("Segoe UI", 9F);
        lblFiscalCondition.Appearance.Options.UseFont = true;
        lblFiscalCondition.Location = new Point(14, 48);
        lblFiscalCondition.Name = "lblFiscalCondition";
        lblFiscalCondition.Size = new Size(90, 15);
        lblFiscalCondition.TabIndex = 2;
        lblFiscalCondition.Text = "Condición Fiscal:";
        //
        // grdAccountingAccounts
        //
        grdAccountingAccounts.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdAccountingAccounts.Location = new Point(14, 146);
        grdAccountingAccounts.MainView = gvAccountingAccounts;
        grdAccountingAccounts.Name = "grdAccountingAccounts";
        grdAccountingAccounts.Size = new Size(2828, 498);
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
        // btnSetDefaultAccountingAccount
        //
        btnSetDefaultAccountingAccount.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSetDefaultAccountingAccount.Appearance.Options.UseFont = true;
        btnSetDefaultAccountingAccount.Location = new Point(308, 112);
        btnSetDefaultAccountingAccount.Name = "btnSetDefaultAccountingAccount";
        btnSetDefaultAccountingAccount.Size = new Size(116, 28);
        btnSetDefaultAccountingAccount.TabIndex = 3;
        btnSetDefaultAccountingAccount.Text = "Predeterminada";
        //
        // lueFiscalCondition
        //
        lueFiscalCondition.Location = new Point(174, 45);
        lueFiscalCondition.Name = "lueFiscalCondition";
        lueFiscalCondition.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueFiscalCondition.Properties.Appearance.Options.UseFont = true;
        lueFiscalCondition.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFiscalCondition.Properties.NullText = "";
        lueFiscalCondition.Size = new Size(230, 22);
        lueFiscalCondition.TabIndex = 3;
        //
        // lblAccountingBlockedValue
        //
        lblAccountingBlockedValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingBlockedValue.Appearance.Options.UseFont = true;
        lblAccountingBlockedValue.Location = new Point(1135, 175);
        lblAccountingBlockedValue.Name = "lblAccountingBlockedValue";
        lblAccountingBlockedValue.Size = new Size(16, 15);
        lblAccountingBlockedValue.TabIndex = 17;
        lblAccountingBlockedValue.Text = "No";
        //
        // lblThirdPartyType
        //
        lblThirdPartyType.Appearance.Font = new Font("Segoe UI", 9F);
        lblThirdPartyType.Appearance.Options.UseFont = true;
        lblThirdPartyType.Location = new Point(14, 76);
        lblThirdPartyType.Name = "lblThirdPartyType";
        lblThirdPartyType.Size = new Size(86, 15);
        lblThirdPartyType.TabIndex = 4;
        lblThirdPartyType.Text = "Tipo de Tercero:";
        //
        // tglAccountingBlocked
        //
        tglAccountingBlocked.Location = new Point(1056, 43);
        tglAccountingBlocked.Name = "tglAccountingBlocked";
        tglAccountingBlocked.Properties.OffText = "";
        tglAccountingBlocked.Properties.OnText = "";
        tglAccountingBlocked.Size = new Size(50, 18);
        tglAccountingBlocked.TabIndex = 16;
        //
        // lueThirdPartyType
        //
        lueThirdPartyType.Location = new Point(174, 73);
        lueThirdPartyType.Name = "lueThirdPartyType";
        lueThirdPartyType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueThirdPartyType.Properties.Appearance.Options.UseFont = true;
        lueThirdPartyType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueThirdPartyType.Properties.NullText = "";
        lueThirdPartyType.Size = new Size(230, 22);
        lueThirdPartyType.TabIndex = 5;
        //
        // lblAccountingBlocked
        //
        lblAccountingBlocked.Appearance.Font = new Font("Segoe UI", 9F);
        lblAccountingBlocked.Appearance.Options.UseFont = true;
        lblAccountingBlocked.Location = new Point(866, 48);
        lblAccountingBlocked.Name = "lblAccountingBlocked";
        lblAccountingBlocked.Size = new Size(145, 15);
        lblAccountingBlocked.TabIndex = 15;
        lblAccountingBlocked.Text = "Bloqueado Contablemente:";
        //
        // lblAutomaticAccounting
        //
        lblAutomaticAccounting.Appearance.Font = new Font("Segoe UI", 9F);
        lblAutomaticAccounting.Appearance.Options.UseFont = true;
        lblAutomaticAccounting.Location = new Point(484, 20);
        lblAutomaticAccounting.Name = "lblAutomaticAccounting";
        lblAutomaticAccounting.Size = new Size(150, 15);
        lblAutomaticAccounting.TabIndex = 6;
        lblAutomaticAccounting.Text = "Contabilización Automática:";
        //
        // lblHandlesAdvancesValue
        //
        lblHandlesAdvancesValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblHandlesAdvancesValue.Appearance.Options.UseFont = true;
        lblHandlesAdvancesValue.Location = new Point(1135, 147);
        lblHandlesAdvancesValue.Name = "lblHandlesAdvancesValue";
        lblHandlesAdvancesValue.Size = new Size(10, 15);
        lblHandlesAdvancesValue.TabIndex = 14;
        lblHandlesAdvancesValue.Text = "Sí";
        //
        // tglAutomaticAccounting
        //
        tglAutomaticAccounting.Location = new Point(676, 15);
        tglAutomaticAccounting.Name = "tglAutomaticAccounting";
        tglAutomaticAccounting.Properties.OffText = "";
        tglAutomaticAccounting.Properties.OnText = "";
        tglAutomaticAccounting.Size = new Size(50, 18);
        tglAutomaticAccounting.TabIndex = 7;
        //
        // tglHandlesAdvances
        //
        tglHandlesAdvances.Location = new Point(1056, 15);
        tglHandlesAdvances.Name = "tglHandlesAdvances";
        tglHandlesAdvances.Properties.OffText = "";
        tglHandlesAdvances.Properties.OnText = "";
        tglHandlesAdvances.Size = new Size(50, 18);
        tglHandlesAdvances.TabIndex = 13;
        //
        // lblAutomaticAccountingValue
        //
        lblAutomaticAccountingValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblAutomaticAccountingValue.Appearance.Options.UseFont = true;
        lblAutomaticAccountingValue.Location = new Point(732, 20);
        lblAutomaticAccountingValue.Name = "lblAutomaticAccountingValue";
        lblAutomaticAccountingValue.Size = new Size(10, 15);
        lblAutomaticAccountingValue.TabIndex = 8;
        lblAutomaticAccountingValue.Text = "Sí";
        //
        // lblHandlesAdvances
        //
        lblHandlesAdvances.Appearance.Font = new Font("Segoe UI", 9F);
        lblHandlesAdvances.Appearance.Options.UseFont = true;
        lblHandlesAdvances.Location = new Point(866, 20);
        lblHandlesAdvances.Name = "lblHandlesAdvances";
        lblHandlesAdvances.Size = new Size(95, 15);
        lblHandlesAdvances.TabIndex = 12;
        lblHandlesAdvances.Text = "Maneja Anticipos:";
        //
        // lblRequiresReconciliation
        //
        lblRequiresReconciliation.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiresReconciliation.Appearance.Options.UseFont = true;
        lblRequiresReconciliation.Location = new Point(484, 48);
        lblRequiresReconciliation.Name = "lblRequiresReconciliation";
        lblRequiresReconciliation.Size = new Size(118, 15);
        lblRequiresReconciliation.TabIndex = 9;
        lblRequiresReconciliation.Text = "Requiere Conciliación:";
        //
        // lblRequiresReconciliationValue
        //
        lblRequiresReconciliationValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblRequiresReconciliationValue.Appearance.Options.UseFont = true;
        lblRequiresReconciliationValue.Location = new Point(732, 48);
        lblRequiresReconciliationValue.Name = "lblRequiresReconciliationValue";
        lblRequiresReconciliationValue.Size = new Size(10, 15);
        lblRequiresReconciliationValue.TabIndex = 11;
        lblRequiresReconciliationValue.Text = "Sí";
        //
        // tglRequiresReconciliation
        //
        tglRequiresReconciliation.Location = new Point(676, 43);
        tglRequiresReconciliation.Name = "tglRequiresReconciliation";
        tglRequiresReconciliation.Properties.OffText = "";
        tglRequiresReconciliation.Properties.OnText = "";
        tglRequiresReconciliation.Size = new Size(50, 18);
        tglRequiresReconciliation.TabIndex = 10;
        //
        // tabSap
        //
        tabSap.Controls.Add(pnlSapContent);
        tabSap.Name = "tabSap";
        tabSap.Size = new Size(1528, 356);
        tabSap.Text = "SAP";
        //
        // pnlSapContent
        //
        pnlSapContent.BorderStyle = BorderStyles.Simple;
        pnlSapContent.Controls.Add(lblSapSynchronized);
        pnlSapContent.Controls.Add(tglSapSynchronized);
        pnlSapContent.Controls.Add(lblSapAuditTitle);
        pnlSapContent.Controls.Add(lblSapSynchronizedValue);
        pnlSapContent.Controls.Add(grdSapAudit);
        pnlSapContent.Controls.Add(lblSapIntegrationValid);
        pnlSapContent.Controls.Add(tglSapIntegrationValid);
        pnlSapContent.Controls.Add(txtSapIntegrationStatus);
        pnlSapContent.Controls.Add(lblSapIntegrationValidValue);
        pnlSapContent.Controls.Add(lblSapIntegrationStatus);
        pnlSapContent.Controls.Add(lblSapErrorBlocked);
        pnlSapContent.Controls.Add(txtSapDataOrigin);
        pnlSapContent.Controls.Add(tglSapErrorBlocked);
        pnlSapContent.Controls.Add(lblSapDataOrigin);
        pnlSapContent.Controls.Add(lblSapErrorBlockedValue);
        pnlSapContent.Controls.Add(txtSapLastSyncUser);
        pnlSapContent.Controls.Add(lblSapAutoUpdate);
        pnlSapContent.Controls.Add(lblSapLastSyncUser);
        pnlSapContent.Controls.Add(tglSapAutoUpdate);
        pnlSapContent.Controls.Add(txtSapLastSync);
        pnlSapContent.Controls.Add(lblSapAutoUpdateValue);
        pnlSapContent.Controls.Add(lblSapLastSync);
        pnlSapContent.Dock = DockStyle.Fill;
        pnlSapContent.Location = new Point(0, 0);
        pnlSapContent.Name = "pnlSapContent";
        pnlSapContent.Size = new Size(1528, 356);
        pnlSapContent.TabIndex = 0;
        //
        // lblSapSynchronized
        //
        lblSapSynchronized.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSynchronized.Appearance.Options.UseFont = true;
        lblSapSynchronized.Location = new Point(11, 18);
        lblSapSynchronized.Name = "lblSapSynchronized";
        lblSapSynchronized.Size = new Size(118, 15);
        lblSapSynchronized.TabIndex = 0;
        lblSapSynchronized.Text = "Sincronizado con SAP:";
        //
        // tglSapSynchronized
        //
        tglSapSynchronized.Enabled = false;
        tglSapSynchronized.Location = new Point(177, 13);
        tglSapSynchronized.Name = "tglSapSynchronized";
        tglSapSynchronized.Properties.OffText = "";
        tglSapSynchronized.Properties.OnText = "";
        tglSapSynchronized.Size = new Size(50, 18);
        tglSapSynchronized.TabIndex = 1;
        //
        // lblSapAuditTitle
        //
        lblSapAuditTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapAuditTitle.Appearance.Options.UseFont = true;
        lblSapAuditTitle.Location = new Point(11, 103);
        lblSapAuditTitle.Name = "lblSapAuditTitle";
        lblSapAuditTitle.Size = new Size(156, 20);
        lblSapAuditTitle.TabIndex = 0;
        lblSapAuditTitle.Text = "Auditoría / Integración";
        //
        // lblSapSynchronizedValue
        //
        lblSapSynchronizedValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapSynchronizedValue.Appearance.Options.UseFont = true;
        lblSapSynchronizedValue.Location = new Point(243, 18);
        lblSapSynchronizedValue.Name = "lblSapSynchronizedValue";
        lblSapSynchronizedValue.Size = new Size(10, 15);
        lblSapSynchronizedValue.TabIndex = 2;
        lblSapSynchronizedValue.Text = "Sí";
        //
        // grdSapAudit
        //
        grdSapAudit.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grdSapAudit.Location = new Point(7, 129);
        grdSapAudit.MainView = gvSapAudit;
        grdSapAudit.Name = "grdSapAudit";
        grdSapAudit.Size = new Size(2630, 469);
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
        // lblSapIntegrationValid
        //
        lblSapIntegrationValid.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapIntegrationValid.Appearance.Options.UseFont = true;
        lblSapIntegrationValid.Location = new Point(11, 50);
        lblSapIntegrationValid.Name = "lblSapIntegrationValid";
        lblSapIntegrationValid.Size = new Size(125, 15);
        lblSapIntegrationValid.TabIndex = 3;
        lblSapIntegrationValid.Text = "Válido para Integración:";
        //
        // tglSapIntegrationValid
        //
        tglSapIntegrationValid.Enabled = false;
        tglSapIntegrationValid.Location = new Point(177, 45);
        tglSapIntegrationValid.Name = "tglSapIntegrationValid";
        tglSapIntegrationValid.Properties.OffText = "";
        tglSapIntegrationValid.Properties.OnText = "";
        tglSapIntegrationValid.Size = new Size(50, 18);
        tglSapIntegrationValid.TabIndex = 4;
        //
        // txtSapIntegrationStatus
        //
        txtSapIntegrationStatus.Location = new Point(1067, 46);
        txtSapIntegrationStatus.Name = "txtSapIntegrationStatus";
        txtSapIntegrationStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapIntegrationStatus.Properties.Appearance.Options.UseFont = true;
        txtSapIntegrationStatus.Properties.ReadOnly = true;
        txtSapIntegrationStatus.Size = new Size(210, 22);
        txtSapIntegrationStatus.TabIndex = 19;
        //
        // lblSapIntegrationValidValue
        //
        lblSapIntegrationValidValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapIntegrationValidValue.Appearance.Options.UseFont = true;
        lblSapIntegrationValidValue.Location = new Point(243, 50);
        lblSapIntegrationValidValue.Name = "lblSapIntegrationValidValue";
        lblSapIntegrationValidValue.Size = new Size(10, 15);
        lblSapIntegrationValidValue.TabIndex = 5;
        lblSapIntegrationValidValue.Text = "Sí";
        //
        // lblSapIntegrationStatus
        //
        lblSapIntegrationStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapIntegrationStatus.Appearance.Options.UseFont = true;
        lblSapIntegrationStatus.Location = new Point(895, 50);
        lblSapIntegrationStatus.Name = "lblSapIntegrationStatus";
        lblSapIntegrationStatus.Size = new Size(117, 15);
        lblSapIntegrationStatus.TabIndex = 18;
        lblSapIntegrationStatus.Text = "Estado de Integración:";
        //
        // lblSapErrorBlocked
        //
        lblSapErrorBlocked.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapErrorBlocked.Appearance.Options.UseFont = true;
        lblSapErrorBlocked.Location = new Point(11, 82);
        lblSapErrorBlocked.Name = "lblSapErrorBlocked";
        lblSapErrorBlocked.Size = new Size(96, 15);
        lblSapErrorBlocked.TabIndex = 6;
        lblSapErrorBlocked.Text = "Bloqueo por Error:";
        //
        // txtSapDataOrigin
        //
        txtSapDataOrigin.Location = new Point(1067, 14);
        txtSapDataOrigin.Name = "txtSapDataOrigin";
        txtSapDataOrigin.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapDataOrigin.Properties.Appearance.Options.UseFont = true;
        txtSapDataOrigin.Properties.ReadOnly = true;
        txtSapDataOrigin.Size = new Size(210, 22);
        txtSapDataOrigin.TabIndex = 17;
        //
        // tglSapErrorBlocked
        //
        tglSapErrorBlocked.Enabled = false;
        tglSapErrorBlocked.Location = new Point(177, 77);
        tglSapErrorBlocked.Name = "tglSapErrorBlocked";
        tglSapErrorBlocked.Properties.OffText = "";
        tglSapErrorBlocked.Properties.OnText = "";
        tglSapErrorBlocked.Size = new Size(50, 18);
        tglSapErrorBlocked.TabIndex = 7;
        //
        // lblSapDataOrigin
        //
        lblSapDataOrigin.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapDataOrigin.Appearance.Options.UseFont = true;
        lblSapDataOrigin.Location = new Point(895, 18);
        lblSapDataOrigin.Name = "lblSapDataOrigin";
        lblSapDataOrigin.Size = new Size(88, 15);
        lblSapDataOrigin.TabIndex = 16;
        lblSapDataOrigin.Text = "Origen de Datos:";
        //
        // lblSapErrorBlockedValue
        //
        lblSapErrorBlockedValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapErrorBlockedValue.Appearance.Options.UseFont = true;
        lblSapErrorBlockedValue.Location = new Point(243, 82);
        lblSapErrorBlockedValue.Name = "lblSapErrorBlockedValue";
        lblSapErrorBlockedValue.Size = new Size(16, 15);
        lblSapErrorBlockedValue.TabIndex = 8;
        lblSapErrorBlockedValue.Text = "No";
        //
        // txtSapLastSyncUser
        //
        txtSapLastSyncUser.Location = new Point(625, 78);
        txtSapLastSyncUser.Name = "txtSapLastSyncUser";
        txtSapLastSyncUser.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapLastSyncUser.Properties.Appearance.Options.UseFont = true;
        txtSapLastSyncUser.Properties.ReadOnly = true;
        txtSapLastSyncUser.Size = new Size(210, 22);
        txtSapLastSyncUser.TabIndex = 15;
        //
        // lblSapAutoUpdate
        //
        lblSapAutoUpdate.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapAutoUpdate.Appearance.Options.UseFont = true;
        lblSapAutoUpdate.Location = new Point(433, 18);
        lblSapAutoUpdate.Name = "lblSapAutoUpdate";
        lblSapAutoUpdate.Size = new Size(139, 15);
        lblSapAutoUpdate.TabIndex = 9;
        lblSapAutoUpdate.Text = "Actualización Automática:";
        //
        // lblSapLastSyncUser
        //
        lblSapLastSyncUser.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastSyncUser.Appearance.Options.UseFont = true;
        lblSapLastSyncUser.Location = new Point(433, 82);
        lblSapLastSyncUser.Name = "lblSapLastSyncUser";
        lblSapLastSyncUser.Size = new Size(161, 15);
        lblSapLastSyncUser.TabIndex = 14;
        lblSapLastSyncUser.Text = "Usuario Última Sincronización:";
        //
        // tglSapAutoUpdate
        //
        tglSapAutoUpdate.Enabled = false;
        tglSapAutoUpdate.Location = new Point(625, 13);
        tglSapAutoUpdate.Name = "tglSapAutoUpdate";
        tglSapAutoUpdate.Properties.OffText = "";
        tglSapAutoUpdate.Properties.OnText = "";
        tglSapAutoUpdate.Size = new Size(50, 18);
        tglSapAutoUpdate.TabIndex = 10;
        //
        // txtSapLastSync
        //
        txtSapLastSync.Location = new Point(625, 46);
        txtSapLastSync.Name = "txtSapLastSync";
        txtSapLastSync.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapLastSync.Properties.Appearance.Options.UseFont = true;
        txtSapLastSync.Properties.ReadOnly = true;
        txtSapLastSync.Size = new Size(210, 22);
        txtSapLastSync.TabIndex = 13;
        //
        // lblSapAutoUpdateValue
        //
        lblSapAutoUpdateValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapAutoUpdateValue.Appearance.Options.UseFont = true;
        lblSapAutoUpdateValue.Location = new Point(691, 18);
        lblSapAutoUpdateValue.Name = "lblSapAutoUpdateValue";
        lblSapAutoUpdateValue.Size = new Size(10, 15);
        lblSapAutoUpdateValue.TabIndex = 11;
        lblSapAutoUpdateValue.Text = "Sí";
        //
        // lblSapLastSync
        //
        lblSapLastSync.Appearance.Font = new Font("Segoe UI", 9F);
        lblSapLastSync.Appearance.Options.UseFont = true;
        lblSapLastSync.Location = new Point(433, 50);
        lblSapLastSync.Name = "lblSapLastSync";
        lblSapLastSync.Size = new Size(118, 15);
        lblSapLastSync.TabIndex = 12;
        lblSapLastSync.Text = "Última Sincronización:";
        //
        // tabAttachments
        //
        tabAttachments.Controls.Add(pnlAttachmentsContent);
        tabAttachments.Name = "tabAttachments";
        tabAttachments.Size = new Size(1528, 356);
        tabAttachments.Text = "Observaciones y Anexos";
        //
        // pnlAttachmentsContent
        //
        pnlAttachmentsContent.BorderStyle = BorderStyles.Simple;
        pnlAttachmentsContent.Controls.Add(memSupplierObservations);
        pnlAttachmentsContent.Controls.Add(btnAttachDocument);
        pnlAttachmentsContent.Controls.Add(btnDownloadDocument);
        pnlAttachmentsContent.Controls.Add(lblAttachmentsTitle);
        pnlAttachmentsContent.Controls.Add(btnViewDocument);
        pnlAttachmentsContent.Controls.Add(btnDeleteDocument);
        pnlAttachmentsContent.Controls.Add(lblSupplierObservationsTitle);
        pnlAttachmentsContent.Controls.Add(grdAttachments);
        pnlAttachmentsContent.Controls.Add(lblAttachmentPath);
        pnlAttachmentsContent.Controls.Add(txtAttachmentPath);
        pnlAttachmentsContent.Controls.Add(memAttachmentDescription);
        pnlAttachmentsContent.Controls.Add(lblAttachmentCategory);
        pnlAttachmentsContent.Controls.Add(lblAttachmentDescription);
        pnlAttachmentsContent.Controls.Add(txtAttachmentCategory);
        pnlAttachmentsContent.Controls.Add(txtAttachmentExpirationDate);
        pnlAttachmentsContent.Controls.Add(lblAttachmentExpirationDate);
        pnlAttachmentsContent.Dock = DockStyle.Fill;
        pnlAttachmentsContent.Location = new Point(0, 0);
        pnlAttachmentsContent.Name = "pnlAttachmentsContent";
        pnlAttachmentsContent.Size = new Size(1528, 356);
        pnlAttachmentsContent.TabIndex = 0;
        //
        // btnAttachDocument
        //
        btnAttachDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAttachDocument.Appearance.Options.UseFont = true;
        btnAttachDocument.Location = new Point(640, 37);
        btnAttachDocument.Name = "btnAttachDocument";
        btnAttachDocument.Size = new Size(92, 28);
        btnAttachDocument.TabIndex = 0;
        btnAttachDocument.Text = "Adjuntar";
        //
        // btnDownloadDocument
        //
        btnDownloadDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDownloadDocument.Appearance.Options.UseFont = true;
        btnDownloadDocument.Location = new Point(744, 37);
        btnDownloadDocument.Name = "btnDownloadDocument";
        btnDownloadDocument.Size = new Size(92, 28);
        btnDownloadDocument.TabIndex = 1;
        btnDownloadDocument.Text = "Descargar";
        //
        // lblAttachmentsTitle
        //
        lblAttachmentsTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblAttachmentsTitle.Appearance.Options.UseFont = true;
        lblAttachmentsTitle.Location = new Point(641, 16);
        lblAttachmentsTitle.Name = "lblAttachmentsTitle";
        lblAttachmentsTitle.Size = new Size(111, 15);
        lblAttachmentsTitle.TabIndex = 0;
        lblAttachmentsTitle.Text = "Documentos Anexos";
        //
        // btnViewDocument
        //
        btnViewDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnViewDocument.Appearance.Options.UseFont = true;
        btnViewDocument.Location = new Point(848, 37);
        btnViewDocument.Name = "btnViewDocument";
        btnViewDocument.Size = new Size(92, 28);
        btnViewDocument.TabIndex = 2;
        btnViewDocument.Text = "Ver";
        //
        // btnDeleteDocument
        //
        btnDeleteDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnDeleteDocument.Appearance.Options.UseFont = true;
        btnDeleteDocument.Location = new Point(952, 37);
        btnDeleteDocument.Name = "btnDeleteDocument";
        btnDeleteDocument.Size = new Size(92, 28);
        btnDeleteDocument.TabIndex = 3;
        btnDeleteDocument.Text = "Eliminar";
        //
        // lblSupplierObservationsTitle
        //
        lblSupplierObservationsTitle.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblSupplierObservationsTitle.Appearance.Options.UseFont = true;
        lblSupplierObservationsTitle.Location = new Point(11, 16);
        lblSupplierObservationsTitle.Name = "lblSupplierObservationsTitle";
        lblSupplierObservationsTitle.Size = new Size(77, 15);
        lblSupplierObservationsTitle.TabIndex = 0;
        lblSupplierObservationsTitle.Text = "Observaciones";
        //
        // grdAttachments
        //
        grdAttachments.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        grdAttachments.Location = new Point(641, 80);
        grdAttachments.MainView = gvAttachments;
        grdAttachments.Name = "grdAttachments";
        grdAttachments.Size = new Size(2067, 130);
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
        lblAttachmentPath.Location = new Point(641, 226);
        lblAttachmentPath.Name = "lblAttachmentPath";
        lblAttachmentPath.Size = new Size(91, 15);
        lblAttachmentPath.TabIndex = 3;
        lblAttachmentPath.Text = "Ruta / Ubicación:";
        //
        // txtAttachmentPath
        //
        txtAttachmentPath.Location = new Point(777, 222);
        txtAttachmentPath.Name = "txtAttachmentPath";
        txtAttachmentPath.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentPath.Properties.Appearance.Options.UseFont = true;
        txtAttachmentPath.Properties.ReadOnly = true;
        txtAttachmentPath.Size = new Size(310, 22);
        txtAttachmentPath.TabIndex = 4;
        //
        // memAttachmentDescription
        //
        memAttachmentDescription.Location = new Point(777, 306);
        memAttachmentDescription.Name = "memAttachmentDescription";
        memAttachmentDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memAttachmentDescription.Properties.Appearance.Options.UseFont = true;
        memAttachmentDescription.Properties.ReadOnly = true;
        memAttachmentDescription.Size = new Size(310, 36);
        memAttachmentDescription.TabIndex = 10;
        //
        // lblAttachmentCategory
        //
        lblAttachmentCategory.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentCategory.Appearance.Options.UseFont = true;
        lblAttachmentCategory.Location = new Point(641, 254);
        lblAttachmentCategory.Name = "lblAttachmentCategory";
        lblAttachmentCategory.Size = new Size(136, 15);
        lblAttachmentCategory.TabIndex = 5;
        lblAttachmentCategory.Text = "Categoría de Documento:";
        //
        // lblAttachmentDescription
        //
        lblAttachmentDescription.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentDescription.Appearance.Options.UseFont = true;
        lblAttachmentDescription.Location = new Point(641, 310);
        lblAttachmentDescription.Name = "lblAttachmentDescription";
        lblAttachmentDescription.Size = new Size(65, 15);
        lblAttachmentDescription.TabIndex = 9;
        lblAttachmentDescription.Text = "Descripción:";
        //
        // txtAttachmentCategory
        //
        txtAttachmentCategory.Location = new Point(777, 250);
        txtAttachmentCategory.Name = "txtAttachmentCategory";
        txtAttachmentCategory.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentCategory.Properties.Appearance.Options.UseFont = true;
        txtAttachmentCategory.Properties.ReadOnly = true;
        txtAttachmentCategory.Size = new Size(310, 22);
        txtAttachmentCategory.TabIndex = 6;
        //
        // txtAttachmentExpirationDate
        //
        txtAttachmentExpirationDate.Location = new Point(777, 278);
        txtAttachmentExpirationDate.Name = "txtAttachmentExpirationDate";
        txtAttachmentExpirationDate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAttachmentExpirationDate.Properties.Appearance.Options.UseFont = true;
        txtAttachmentExpirationDate.Properties.ReadOnly = true;
        txtAttachmentExpirationDate.Size = new Size(150, 22);
        txtAttachmentExpirationDate.TabIndex = 8;
        //
        // lblAttachmentExpirationDate
        //
        lblAttachmentExpirationDate.Appearance.Font = new Font("Segoe UI", 9F);
        lblAttachmentExpirationDate.Appearance.Options.UseFont = true;
        lblAttachmentExpirationDate.Location = new Point(641, 282);
        lblAttachmentExpirationDate.Name = "lblAttachmentExpirationDate";
        lblAttachmentExpirationDate.Size = new Size(120, 15);
        lblAttachmentExpirationDate.TabIndex = 7;
        lblAttachmentExpirationDate.Text = "Fecha de Vencimiento:";
        //
        // lblSupplierActive
        //
        lblSupplierActive.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierActive.Appearance.Options.UseFont = true;
        lblSupplierActive.Location = new Point(337, 12);
        lblSupplierActive.Name = "lblSupplierActive";
        lblSupplierActive.Size = new Size(38, 15);
        lblSupplierActive.TabIndex = 21;
        lblSupplierActive.Text = "Estado:";
        //
        // tglSupplierActive
        //
        tglSupplierActive.Location = new Point(381, 10);
        tglSupplierActive.Name = "tglSupplierActive";
        tglSupplierActive.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tglSupplierActive.Properties.Appearance.Options.UseFont = true;
        tglSupplierActive.Properties.OffText = "Inactivo";
        tglSupplierActive.Properties.OnText = "Activo";
        tglSupplierActive.Size = new Size(116, 20);
        tglSupplierActive.TabIndex = 22;
        //
        // txtTradeName
        //
        txtTradeName.Location = new Point(150, 65);
        txtTradeName.Name = "txtTradeName";
        txtTradeName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtTradeName.Properties.Appearance.Options.UseFont = true;
        txtTradeName.Size = new Size(524, 22);
        txtTradeName.TabIndex = 26;
        //
        // lblSupplierSegment
        //
        lblSupplierSegment.Appearance.Font = new Font("Segoe UI", 9F);
        lblSupplierSegment.Appearance.Options.UseFont = true;
        lblSupplierSegment.Location = new Point(12, 124);
        lblSupplierSegment.Name = "lblSupplierSegment";
        lblSupplierSegment.Size = new Size(57, 15);
        lblSupplierSegment.TabIndex = 29;
        lblSupplierSegment.Text = "Segmento:";
        //
        // lblTradeName
        //
        lblTradeName.Appearance.Font = new Font("Segoe UI", 9F);
        lblTradeName.Appearance.Options.UseFont = true;
        lblTradeName.Location = new Point(12, 68);
        lblTradeName.Name = "lblTradeName";
        lblTradeName.Size = new Size(104, 15);
        lblTradeName.TabIndex = 25;
        lblTradeName.Text = "Nombre Comercial:";
        //
        // lblInternalClassification
        //
        lblInternalClassification.Appearance.Font = new Font("Segoe UI", 9F);
        lblInternalClassification.Appearance.Options.UseFont = true;
        lblInternalClassification.Location = new Point(11, 96);
        lblInternalClassification.Name = "lblInternalClassification";
        lblInternalClassification.Size = new Size(36, 15);
        lblInternalClassification.TabIndex = 27;
        lblInternalClassification.Text = "Grupo:";
        //
        // txtBusinessName
        //
        txtBusinessName.Location = new Point(150, 37);
        txtBusinessName.Name = "txtBusinessName";
        txtBusinessName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtBusinessName.Properties.Appearance.Options.UseFont = true;
        txtBusinessName.Size = new Size(524, 22);
        txtBusinessName.TabIndex = 24;
        //
        // lueSupplierSegment
        //
        lueSupplierSegment.Location = new Point(150, 121);
        lueSupplierSegment.Name = "lueSupplierSegment";
        lueSupplierSegment.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSupplierSegment.Properties.Appearance.Options.UseFont = true;
        lueSupplierSegment.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSupplierSegment.Properties.NullText = "";
        lueSupplierSegment.Size = new Size(225, 22);
        lueSupplierSegment.TabIndex = 30;
        //
        // lueInternalClassification
        //
        lueInternalClassification.Location = new Point(150, 93);
        lueInternalClassification.Name = "lueInternalClassification";
        lueInternalClassification.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueInternalClassification.Properties.Appearance.Options.UseFont = true;
        lueInternalClassification.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueInternalClassification.Properties.NullText = "";
        lueInternalClassification.Size = new Size(225, 22);
        lueInternalClassification.TabIndex = 28;
        //
        // lblBusinessName
        //
        lblBusinessName.Appearance.Font = new Font("Segoe UI", 9F);
        lblBusinessName.Appearance.Options.UseFont = true;
        lblBusinessName.Location = new Point(12, 40);
        lblBusinessName.Name = "lblBusinessName";
        lblBusinessName.Size = new Size(69, 15);
        lblBusinessName.TabIndex = 23;
        lblBusinessName.Text = "Razón Social:";
        //
        // btnCancel
        //
        btnCancel.Appearance.BackColor = Color.FromArgb(99, 110, 114);
        btnCancel.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnCancel.Appearance.ForeColor = Color.White;
        btnCancel.Appearance.Options.UseBackColor = true;
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.Appearance.Options.UseForeColor = true;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(1323, 538);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 31;
        btnCancel.Text = "Cancelar";
        //
        // btnSave
        //
        btnSave.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.Location = new Point(1429, 538);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 32;
        btnSave.Text = "Guardar";
        //
        // memSupplierObservations
        //
        memSupplierObservations.Location = new Point(11, 37);
        memSupplierObservations.Name = "memSupplierObservations";
        memSupplierObservations.Size = new Size(623, 305);
        memSupplierObservations.TabIndex = 11;
        //
        // SupplierEditForm
        //
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1546, 688);
        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(lblSupplierCode);
        Controls.Add(txtSupplierCode);
        Controls.Add(lblMasterSyncStatus);
        Controls.Add(lblMasterSyncMessage);
        Controls.Add(tabSupplier);
        Controls.Add(lblSupplierActive);
        Controls.Add(tglSupplierActive);
        Controls.Add(txtTradeName);
        Controls.Add(lblSupplierSegment);
        Controls.Add(lblTradeName);
        Controls.Add(lblInternalClassification);
        Controls.Add(txtBusinessName);
        Controls.Add(lueSupplierSegment);
        Controls.Add(lueInternalClassification);
        Controls.Add(lblBusinessName);
        MinimumSize = new Size(1180, 720);
        Name = "SupplierEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Mantenimiento de Proveedores";
        ((System.ComponentModel.ISupportInitialize)lueCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtProvinceCity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteRegistrationDate.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dteRegistrationDate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnPaymentTermDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memGeneralComments.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tabSupplier).EndInit();
        tabSupplier.ResumeLayout(false);
        tabGeneral.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlGeneralContent).EndInit();
        pnlGeneralContent.ResumeLayout(false);
        pnlGeneralContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)txtWebsite.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueDocumentType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglActiveForPurchases.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDocumentNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierCategory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memShortObservation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSubjectToWithholding.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePersonType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglHandlesCredit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglBlocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtMainContact.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtEmail.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPhone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierClass.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueEconomicActivity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierZone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplyMethod.Properties).EndInit();
        tabContacts.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlContactsContent).EndInit();
        pnlContactsContent.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grdContacts).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvContacts).EndInit();
        tabAddresses.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlAddressesContent).EndInit();
        pnlAddressesContent.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grdAddresses).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvAddresses).EndInit();
        tabPurchases.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlPurchasesContent).EndInit();
        pnlPurchasesContent.ResumeLayout(false);
        pnlPurchasesContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)luePurchasePaymentCondition.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchasePriceList.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditLimit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnDeliveryTermDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueIncoterm.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCommercialDiscountPercent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAssignedBuyer.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePreferredWarehouse.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseSupplierType.Properties).EndInit();
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
        ((System.ComponentModel.ISupportInitialize)grdBankAccounts).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvBankAccounts).EndInit();
        tabWithholdings.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlWithholdingsContent).EndInit();
        pnlWithholdingsContent.ResumeLayout(false);
        pnlWithholdingsContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglWithholdingAgent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueGeneralWithholdingType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdWithholdings).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvWithholdings).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtWithholdingResolutionNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglWithholdsVat.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSubjectToPerception.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglWithholdsIncomeTax.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglIssuesElectronicReceipts.Properties).EndInit();
        tabAccounting.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlAccountingContent).EndInit();
        pnlAccountingContent.ResumeLayout(false);
        pnlAccountingContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueDefaultProject.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdAccountingAccounts).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvAccountingAccounts).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCondition.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAccountingBlocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueThirdPartyType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglAutomaticAccounting.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglHandlesAdvances.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglRequiresReconciliation.Properties).EndInit();
        tabSap.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlSapContent).EndInit();
        pnlSapContent.ResumeLayout(false);
        pnlSapContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)tglSapSynchronized.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdSapAudit).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvSapAudit).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSapIntegrationValid.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapIntegrationStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapDataOrigin.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSapErrorBlocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastSyncUser.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSapAutoUpdate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastSync.Properties).EndInit();
        tabAttachments.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentsContent).EndInit();
        pnlAttachmentsContent.ResumeLayout(false);
        pnlAttachmentsContent.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentPath.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memAttachmentDescription.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentCategory.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAttachmentExpirationDate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tglSupplierActive.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtTradeName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBusinessName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSupplierSegment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueInternalClassification.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memSupplierObservations.Properties).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
    private LabelControl lblCountry;
    private LookUpEdit lueCountry;
    private LabelControl lblProvinceCity;
    private TextEdit txtProvinceCity;
    private LabelControl lblRegistrationDate;
    private DateEdit dteRegistrationDate;
    private LabelControl lblPaymentTermDays;
    private SpinEdit spnPaymentTermDays;
    private LabelControl lblGeneralComments;
    private MemoEdit memGeneralComments;
    private LabelControl lblSupplierCode;
    private TextEdit txtSupplierCode;
    private LabelControl lblMasterSyncStatus;
    private LabelControl lblMasterSyncMessage;
    private XtraTabControl tabSupplier;
    private XtraTabPage tabGeneral;
    private PanelControl pnlGeneralContent;
    private LabelControl lblWebsite;
    private TextEdit txtWebsite;
    private LabelControl lblDocumentType;
    private LabelControl lblActiveForPurchases;
    private LookUpEdit lueDocumentType;
    private LabelControl lblDocumentNumber;
    private ToggleSwitch tglActiveForPurchases;
    private TextEdit txtDocumentNumber;
    private LabelControl lblSubjectToWithholding;
    private LabelControl lblPersonType;
    private LabelControl lblSupplierCategory;
    private LookUpEdit lueCurrency;
    private LabelControl lblShortObservation;
    private LookUpEdit lueSupplierCategory;
    private MemoEdit memShortObservation;
    private LabelControl lblCurrency;
    private ToggleSwitch tglSubjectToWithholding;
    private LookUpEdit luePersonType;
    private LabelControl lblHandlesCredit;
    private LabelControl lblSupplierType;
    private ToggleSwitch tglHandlesCredit;
    private LookUpEdit lueSupplierType;
    private LabelControl lblBlocked;
    private LabelControl lblMainContact;
    private ToggleSwitch tglBlocked;
    private TextEdit txtMainContact;
    private TextEdit txtEmail;
    private LabelControl lblPhone;
    private LabelControl lblEmail;
    private TextEdit txtPhone;
    private LabelControl lblSupplierClass;
    private LookUpEdit lueSupplierClass;
    private LabelControl lblEconomicActivity;
    private LookUpEdit lueEconomicActivity;
    private LabelControl lblSupplierZone;
    private LookUpEdit lueSupplierZone;
    private LabelControl lblSupplyMethod;
    private LookUpEdit lueSupplyMethod;
    private XtraTabPage tabContacts;
    private PanelControl pnlContactsContent;
    private GridControl grdContacts;
    private GridView gvContacts;
    private GridColumn colContactFullName;
    private GridColumn colContactPosition;
    private GridColumn colContactDepartment;
    private GridColumn colContactIsPrimary;
    private GridColumn colContactIsActive;
    private SimpleButton btnAddContact;
    private SimpleButton btnSetDefaultContact;
    private SimpleButton btnEditContact;
    private SimpleButton btnDeleteContact;
    private XtraTabPage tabAddresses;
    private PanelControl pnlAddressesContent;
    private SimpleButton btnAddAddress;
    private SimpleButton btnEditAddress;
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
    private SimpleButton btnSetDefaultAddress;
    private SimpleButton btnDeleteAddress;
    private SimpleButton btnDuplicateAddress;
    private XtraTabPage tabPurchases;
    private PanelControl pnlPurchasesContent;
    private LabelControl lblPurchasePaymentCondition;
    private LookUpEdit luePurchasePaymentCondition;
    private LabelControl lblCreditLimit;
    private LabelControl lblPurchasePriceList;
    private LookUpEdit luePurchasePriceList;
    private SpinEdit spnCreditLimit;
    private LabelControl lblDeliveryTermDays;
    private SpinEdit spnDeliveryTermDays;
    private LabelControl lblIncoterm;
    private LookUpEdit lueIncoterm;
    private LabelControl lblCommercialDiscountPercent;
    private SpinEdit spnCommercialDiscountPercent;
    private LabelControl lblAssignedBuyer;
    private LookUpEdit lueAssignedBuyer;
    private LabelControl lblPreferredWarehouse;
    private LookUpEdit luePreferredWarehouse;
    private LabelControl lblPurchaseCurrency;
    private LookUpEdit luePurchaseCurrency;
    private LabelControl lblPurchaseSupplierType;
    private LookUpEdit luePurchaseSupplierType;
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
    private LabelControl lblSubjectToEvaluation;
    private ToggleSwitch tglSubjectToEvaluation;
    private LabelControl lblActiveForImport;
    private ToggleSwitch tglActiveForImport;
    private LabelControl lblAllowsUrgentPurchases;
    private ToggleSwitch tglAllowsUrgentPurchases;
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
    private XtraTabPage tabBanks;
    private PanelControl pnlBanksContent;
    private SimpleButton btnAddBankAccount;
    private SimpleButton btnEditBankAccount;
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
    private SimpleButton btnDeleteBankAccount;
    private SimpleButton btnSetDefaultBankAccount;
    private XtraTabPage tabWithholdings;
    private PanelControl pnlWithholdingsContent;
    private LabelControl lblWithholdingAgent;
    private ToggleSwitch tglWithholdingAgent;
    private SimpleButton btnAddWithholding;
    private LabelControl lblGeneralWithholdingType;
    private LookUpEdit lueGeneralWithholdingType;
    private SimpleButton btnEditWithholding;
    private LabelControl lblWithholdingResolutionNumber;
    private GridControl grdWithholdings;
    private GridView gvWithholdings;
    private GridColumn colWithholdingDocument;
    private GridColumn colWithholdingType;
    private GridColumn colWithholdingValidity;
    private GridColumn colWithholdingIsDefault;
    private GridColumn colWithholdingStatus;
    private TextEdit txtWithholdingResolutionNumber;
    private SimpleButton btnSetDefaultWithholding;
    private LabelControl lblWithholdsVat;
    private SimpleButton btnDeleteWithholding;
    private ToggleSwitch tglWithholdsVat;
    private ToggleSwitch tglSubjectToPerception;
    private LabelControl lblWithholdsIncomeTax;
    private LabelControl lblSubjectToPerception;
    private ToggleSwitch tglWithholdsIncomeTax;
    private ToggleSwitch tglIssuesElectronicReceipts;
    private LabelControl lblIssuesElectronicReceipts;
    private XtraTabPage tabAccounting;
    private PanelControl pnlAccountingContent;
    private SimpleButton btnAddAccountingAccount;
    private SimpleButton btnEditAccountingAccount;
    private LabelControl lblDefaultProject;
    private LookUpEdit lueDefaultProject;
    private SimpleButton btnDeleteAccountingAccount;
    private LabelControl lblFiscalCondition;
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
    private SimpleButton btnSetDefaultAccountingAccount;
    private LookUpEdit lueFiscalCondition;
    private LabelControl lblAccountingBlockedValue;
    private LabelControl lblThirdPartyType;
    private ToggleSwitch tglAccountingBlocked;
    private LookUpEdit lueThirdPartyType;
    private LabelControl lblAccountingBlocked;
    private LabelControl lblAutomaticAccounting;
    private LabelControl lblHandlesAdvancesValue;
    private ToggleSwitch tglAutomaticAccounting;
    private ToggleSwitch tglHandlesAdvances;
    private LabelControl lblAutomaticAccountingValue;
    private LabelControl lblHandlesAdvances;
    private LabelControl lblRequiresReconciliation;
    private LabelControl lblRequiresReconciliationValue;
    private ToggleSwitch tglRequiresReconciliation;
    private XtraTabPage tabSap;
    private PanelControl pnlSapContent;
    private LabelControl lblSapSynchronized;
    private ToggleSwitch tglSapSynchronized;
    private LabelControl lblSapAuditTitle;
    private LabelControl lblSapSynchronizedValue;
    private GridControl grdSapAudit;
    private GridView gvSapAudit;
    private GridColumn colSapAuditDate;
    private GridColumn colSapAuditAction;
    private GridColumn colSapAuditResult;
    private GridColumn colSapAuditUser;
    private GridColumn colSapAuditMessage;
    private LabelControl lblSapIntegrationValid;
    private ToggleSwitch tglSapIntegrationValid;
    private TextEdit txtSapIntegrationStatus;
    private LabelControl lblSapIntegrationValidValue;
    private LabelControl lblSapIntegrationStatus;
    private LabelControl lblSapErrorBlocked;
    private TextEdit txtSapDataOrigin;
    private ToggleSwitch tglSapErrorBlocked;
    private LabelControl lblSapDataOrigin;
    private LabelControl lblSapErrorBlockedValue;
    private TextEdit txtSapLastSyncUser;
    private LabelControl lblSapAutoUpdate;
    private LabelControl lblSapLastSyncUser;
    private ToggleSwitch tglSapAutoUpdate;
    private TextEdit txtSapLastSync;
    private LabelControl lblSapAutoUpdateValue;
    private LabelControl lblSapLastSync;
    private XtraTabPage tabAttachments;
    private PanelControl pnlAttachmentsContent;
    private SimpleButton btnAttachDocument;
    private SimpleButton btnDownloadDocument;
    private LabelControl lblAttachmentsTitle;
    private SimpleButton btnViewDocument;
    private SimpleButton btnDeleteDocument;
    private LabelControl lblSupplierObservationsTitle;
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
    private MemoEdit memAttachmentDescription;
    private LabelControl lblAttachmentCategory;
    private LabelControl lblAttachmentDescription;
    private TextEdit txtAttachmentCategory;
    private TextEdit txtAttachmentExpirationDate;
    private LabelControl lblAttachmentExpirationDate;
    private LabelControl lblSupplierActive;
    private ToggleSwitch tglSupplierActive;
    private TextEdit txtTradeName;
    private LabelControl lblSupplierSegment;
    private LabelControl lblTradeName;
    private LabelControl lblInternalClassification;
    private TextEdit txtBusinessName;
    private LookUpEdit lueSupplierSegment;
    private LookUpEdit lueInternalClassification;
    private LabelControl lblBusinessName;
    private SimpleButton btnCancel;
    private SimpleButton btnSave;
    private MemoEdit memSupplierObservations;
}

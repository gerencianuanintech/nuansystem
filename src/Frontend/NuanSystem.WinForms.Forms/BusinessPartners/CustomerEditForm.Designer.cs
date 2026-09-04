using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.BusinessPartners;

partial class CustomerEditForm
{
    private System.ComponentModel.IContainer components = null;
    private PanelControl pnlMain;
    private PanelControl grpGeneralInfo;
    private PanelControl grpClassification;
    private LabelControl lblGeneralInfoTitle;
    private LabelControl lblClassificationTitle;
    private LabelControl lblMasterSyncStatus;
    private LabelControl lblMasterSyncMessage;
    private XtraTabControl xtcMain;
    private XtraTabPage xtpGeneral;
    private XtraTabPage xtpFiscal;
    private XtraTabPage xtpAddresses;
    private XtraTabPage xtpContacts;
    private XtraTabPage xtpCommercial;
    private XtraTabPage xtpAccounting;
    private XtraTabPage xtpSap;
    private PanelControl pnlFooter;
    private SimpleButton btnSave;
    private SimpleButton btnCancel;
    private TextEdit txtCustomerCode;
    private TextEdit txtCustomerName;
    private TextEdit txtCustomerCommercialName;
    private LookUpEdit lueIdentificationType;
    private TextEdit txtIdentificationNumber;
    private ButtonEdit txtExternalCode;
    private SearchLookUpEdit lueCustomerGroup;
    private GridView grvCustomerGroupLookup;
    private SearchLookUpEdit lueSalesPerson;
    private GridView grvSalesPersonLookup;
    private TextEdit txtPhone;
    private TextEdit txtEmail;
    private TextEdit txtShortAddress;
    private LookUpEdit lueStatus;
    private LookUpEdit lueCustomerType;
    private LookUpEdit luePriceList;
    private LookUpEdit luePaymentTerm;
    private LookUpEdit lueCurrency;
    private SpinEdit spnCreditLimit;
    private LookUpEdit lueChannel;
    private LookUpEdit lueZone;
    private LookUpEdit lueRiskLevel;
    private ToggleSwitch tsAllowSales;
    private ToggleSwitch tsCreditBlocked;
    private ToggleSwitch tsTaxExempt;
    private ToggleSwitch tsStrategicCustomer;
    private PictureEdit picCustomerLogo;
    private SimpleButton btnLoadImage;
    private SimpleButton btnRemoveImage;
    private MemoEdit memObservations;
    private DateEdit dtpStartDate;
    private LookUpEdit lueCustomerOrigin;
    private LookUpEdit lueAbcClassification;
    private LookUpEdit luePurchaseFrequency;
    private SpinEdit spnCurrentBalance;
    private SpinEdit spnCreditAvailable;
    private DateEdit dtpLastPurchase;
    private SpinEdit spnOpenOrders;
    private SpinEdit spnYtdSales;
    private GridControl grdCustomerContacts;
    private GridView grvCustomerContacts;
    private MemoEdit memInternalNotes;
    private LookUpEdit lueSegment;
    private LookUpEdit lueInternalClassification;
    private MemoEdit memCommercialTerms;
    private TextEdit txtTags;
    private LookUpEdit lueTaxpayerType;
    private LookUpEdit lueFiscalRegime;
    private ToggleSwitch tsAccountingRequired;
    private ToggleSwitch tsWithholdingAgent;
    private ToggleSwitch tsSubjectToWithholding;
    private SpinEdit spnWithholdingPercent;
    private LookUpEdit lueRentType;
    private LookUpEdit lueFiscalCountry;
    private LookUpEdit lueFiscalProvince;
    private LookUpEdit lueFiscalCity;
    private MemoEdit memFiscalAddress;
    private TextEdit txtFiscalPostalCode;
    private LookUpEdit lueEmissionType;
    private TextEdit txtDefaultSeries;
    private SpinEdit spnInitialNumber;
    private LookUpEdit luePrintFormat;
    private MemoEdit memFiscalNotes;
    private GridControl grdCustomerAddresses;
    private GridView grvCustomerAddresses;
    private SimpleButton btnAddAddress;
    private SimpleButton btnEditAddress;
    private SimpleButton btnDeleteAddress;
    private SimpleButton btnSetPrimaryAddress;
    private LookUpEdit lueAddressType;
    private MemoEdit memAddress;
    private LookUpEdit lueAddressCountry;
    private LookUpEdit lueAddressProvince;
    private LookUpEdit lueAddressCity;
    private TextEdit txtPostalCode;
    private TextEdit txtAddressReference;
    private ToggleSwitch tsPrimaryAddress;
    private GridControl grdCustomerContactList;
    private GridView grvCustomerContactList;
    private SimpleButton btnAddContact;
    private SimpleButton btnEditContact;
    private SimpleButton btnDeleteContact;
    private TextEdit txtContactName;
    private TextEdit txtContactPosition;
    private TextEdit txtContactPhone;
    private TextEdit txtContactMobile;
    private TextEdit txtContactEmail;
    private ToggleSwitch tsPrimaryContact;
    private ToggleSwitch tsActiveContact;
    private MemoEdit memContactNotes;
    private SpinEdit spnOverdueDays;
    private MemoEdit memCommercialNotes;
    private SearchLookUpEdit sluReceivableAccount;
    private GridView grvReceivableAccountLookup;
    private SearchLookUpEdit sluCustomerAdvanceAccount;
    private GridView grvCustomerAdvanceAccountLookup;
    private SearchLookUpEdit sluDiscountAccount;
    private GridView grvDiscountAccountLookup;
    private SearchLookUpEdit sluInterestAccount;
    private GridView grvInterestAccountLookup;
    private LookUpEdit lueCostCenter;
    private LookUpEdit lueProject;
    private SearchLookUpEdit sluIncomeWithholding;
    private GridView grvIncomeWithholdingLookup;
    private SearchLookUpEdit sluVatWithholding;
    private GridView grvVatWithholdingLookup;
    private LookUpEdit lueIcaWithholding;
    private LookUpEdit lueAccountingCurrency;
    private SpinEdit spnExchangeRate;
    private LookUpEdit lueValidationStatus;
    private TextEdit txtSapCardCode;
    private LookUpEdit lueSapGroup;
    private LookUpEdit lueSapPaymentTerm;
    private LookUpEdit lueSapCurrency;
    private LookUpEdit lueSapStatus;
    private DateEdit dtpSapLastSync;
    private TextEdit txtSapUser;
    private TextEdit txtSapSourceSystem;
    private TextEdit txtSapCompany;
    private SimpleButton btnSyncSap;
    private SimpleButton btnValidateSap;
    private SimpleButton btnOpenSap;
    private GridControl grdCustomerSapLog;
    private GridView grvCustomerSapLog;

    private void InitializeComponent()
    {
        pnlMain = new PanelControl();
        grpGeneralInfo = new PanelControl();
        lblGeneralInfoTitle = new LabelControl();
        lblMasterSyncStatus = new LabelControl();
        lblMasterSyncMessage = new LabelControl();
        lblCustomerCode = new LabelControl();
        txtCustomerCode = new TextEdit();
        lblCustomerName = new LabelControl();
        txtCustomerName = new TextEdit();
        lblCustomerCommercialName = new LabelControl();
        txtCustomerCommercialName = new TextEdit();
        lblIdentificationType = new LabelControl();
        lueIdentificationType = new LookUpEdit();
        lblIdentificationNumber = new LabelControl();
        txtIdentificationNumber = new TextEdit();
        lblExternalCode = new LabelControl();
        txtExternalCode = new ButtonEdit();
        lblCustomerGroup = new LabelControl();
        lueCustomerGroup = new SearchLookUpEdit();
        lueCustomerGroupView = new GridView();
        grvCustomerGroupLookup = lueCustomerGroupView;
        lblSalesPerson = new LabelControl();
        lueSalesPerson = new SearchLookUpEdit();
        lueSalesPersonView = new GridView();
        grvSalesPersonLookup = lueSalesPersonView;
        lblPhone = new LabelControl();
        txtPhone = new TextEdit();
        lblEmail = new LabelControl();
        txtEmail = new TextEdit();
        lblShortAddress = new LabelControl();
        txtShortAddress = new TextEdit();
        lblStatus = new LabelControl();
        lueStatus = new LookUpEdit();
        grpClassification = new PanelControl();
        lblClassificationTitle = new LabelControl();
        lblCustomerType = new LabelControl();
        lueCustomerType = new LookUpEdit();
        lblPriceList = new LabelControl();
        luePriceList = new LookUpEdit();
        lblPaymentTerm = new LabelControl();
        luePaymentTerm = new LookUpEdit();
        lblCurrency = new LabelControl();
        lueCurrency = new LookUpEdit();
        lblCreditLimit = new LabelControl();
        spnCreditLimit = new SpinEdit();
        lblChannel = new LabelControl();
        lueChannel = new LookUpEdit();
        lblZone = new LabelControl();
        lueZone = new LookUpEdit();
        lblRisk = new LabelControl();
        lueRiskLevel = new LookUpEdit();
        lblAllowSales = new LabelControl();
        tsAllowSales = new ToggleSwitch();
        lblCreditBlocked = new LabelControl();
        tsCreditBlocked = new ToggleSwitch();
        lblTaxExempt = new LabelControl();
        tsTaxExempt = new ToggleSwitch();
        lblStrategic = new LabelControl();
        tsStrategicCustomer = new ToggleSwitch();
        xtcMain = new XtraTabControl();
        xtpGeneral = new XtraTabPage();
        picCustomerLogo = new PictureEdit();
        btnLoadImage = new SimpleButton();
        btnRemoveImage = new SimpleButton();
        memObservations = new MemoEdit();
        dtpStartDate = new DateEdit();
        lueCustomerOrigin = new LookUpEdit();
        lueAbcClassification = new LookUpEdit();
        luePurchaseFrequency = new LookUpEdit();
        spnCurrentBalance = new SpinEdit();
        spnCreditAvailable = new SpinEdit();
        dtpLastPurchase = new DateEdit();
        spnOpenOrders = new SpinEdit();
        spnYtdSales = new SpinEdit();
        grdCustomerContacts = new GridControl();
        grvCustomerContacts = new GridView();
        memInternalNotes = new MemoEdit();
        lueSegment = new LookUpEdit();
        lueInternalClassification = new LookUpEdit();
        memCommercialTerms = new MemoEdit();
        txtTags = new TextEdit();
        grpCustomerImage = new PanelControl();
        grpCustomerObservations = new PanelControl();
        grpCustomerComplement = new PanelControl();
        lblCustomerStartDate = new LabelControl();
        lblCustomerOrigin = new LabelControl();
        lblCustomerAbc = new LabelControl();
        lblCustomerFrequency = new LabelControl();
        grpCustomerSummary = new PanelControl();
        lblCustomerSalesYtd = new LabelControl();
        lblCustomerBalance = new LabelControl();
        lblCustomerAvailable = new LabelControl();
        lblCustomerLastPurchase = new LabelControl();
        lblCustomerOpenOrders = new LabelControl();
        grpCustomerPreview = new PanelControl();
        grpCustomerTerms = new PanelControl();
        lblCustomerSegment = new LabelControl();
        lblCustomerInternalClass = new LabelControl();
        lblCustomerTags = new LabelControl();
        xtpFiscal = new XtraTabPage();
        lblFiscalTitle = new LabelControl();
        lueTaxpayerType = new LookUpEdit();
        lueFiscalRegime = new LookUpEdit();
        tsAccountingRequired = new ToggleSwitch();
        tsWithholdingAgent = new ToggleSwitch();
        lblRetentionTitle = new LabelControl();
        tsSubjectToWithholding = new ToggleSwitch();
        spnWithholdingPercent = new SpinEdit();
        lueRentType = new LookUpEdit();
        lueFiscalCountry = new LookUpEdit();
        lueFiscalProvince = new LookUpEdit();
        lueFiscalCity = new LookUpEdit();
        memFiscalAddress = new MemoEdit();
        txtFiscalPostalCode = new TextEdit();
        lueEmissionType = new LookUpEdit();
        txtDefaultSeries = new TextEdit();
        spnInitialNumber = new SpinEdit();
        luePrintFormat = new LookUpEdit();
        memFiscalNotes = new MemoEdit();
        grpCustomerFiscalInfo = new PanelControl();
        lblCustomerTaxpayer = new LabelControl();
        lblCustomerFiscalRegime = new LabelControl();
        lblCustomerAccountingRequired = new LabelControl();
        lblCustomerWithholdingAgent = new LabelControl();
        grpCustomerRetentions = new PanelControl();
        lblCustomerSubjectWithholding = new LabelControl();
        lblCustomerWithholdingPercent = new LabelControl();
        lblCustomerRentType = new LabelControl();
        grpCustomerFiscalLocation = new PanelControl();
        lblCustomerFiscalCountry = new LabelControl();
        lblCustomerFiscalProvince = new LabelControl();
        lblCustomerFiscalCity = new LabelControl();
        lblCustomerFiscalAddress = new LabelControl();
        lblCustomerFiscalPostal = new LabelControl();
        grpCustomerFiscalDocuments = new PanelControl();
        lblCustomerEmissionType = new LabelControl();
        lblCustomerDefaultSeries = new LabelControl();
        lblCustomerInitialNumber = new LabelControl();
        lblCustomerPrintFormat = new LabelControl();
        grpCustomerFiscalNotes = new PanelControl();
        xtpAddresses = new XtraTabPage();
        grdCustomerAddresses = new GridControl();
        grvCustomerAddresses = new GridView();
        lueAddressType = new LookUpEdit();
        memAddress = new MemoEdit();
        lueAddressCountry = new LookUpEdit();
        lueAddressProvince = new LookUpEdit();
        lueAddressCity = new LookUpEdit();
        txtPostalCode = new TextEdit();
        txtAddressReference = new TextEdit();
        tsPrimaryAddress = new ToggleSwitch();
        lblAddressButtons = new LabelControl();
        btnAddAddress = new SimpleButton();
        btnEditAddress = new SimpleButton();
        btnDeleteAddress = new SimpleButton();
        btnSetPrimaryAddress = new SimpleButton();
        grpCustomerAddressList = new PanelControl();
        grpCustomerAddressDetail = new PanelControl();
        lblCustomerAddressType = new LabelControl();
        lblCustomerAddress = new LabelControl();
        lblCustomerAddressCountry = new LabelControl();
        lblCustomerAddressProvince = new LabelControl();
        lblCustomerAddressCity = new LabelControl();
        lblCustomerPostal = new LabelControl();
        lblCustomerReference = new LabelControl();
        lblCustomerPrimaryAddress = new LabelControl();
        xtpContacts = new XtraTabPage();
        grdCustomerContactList = new GridControl();
        grvCustomerContactList = new GridView();
        txtContactName = new TextEdit();
        txtContactPosition = new TextEdit();
        txtContactPhone = new TextEdit();
        txtContactMobile = new TextEdit();
        txtContactEmail = new TextEdit();
        tsPrimaryContact = new ToggleSwitch();
        tsActiveContact = new ToggleSwitch();
        memContactNotes = new MemoEdit();
        lblContactButtons = new LabelControl();
        btnAddContact = new SimpleButton();
        btnEditContact = new SimpleButton();
        btnDeleteContact = new SimpleButton();
        grpCustomerContactList = new PanelControl();
        grpCustomerContactDetail = new PanelControl();
        lblCustomerContactName = new LabelControl();
        lblCustomerContactPosition = new LabelControl();
        lblCustomerContactPhone = new LabelControl();
        lblCustomerContactMobile = new LabelControl();
        lblCustomerContactEmail = new LabelControl();
        lblCustomerPrimaryContact = new LabelControl();
        lblCustomerActiveContact = new LabelControl();
        xtpCommercial = new XtraTabPage();
        spnOverdueDays = new SpinEdit();
        memCommercialNotes = new MemoEdit();
        grpCustomerCommercialConditions = new PanelControl();
        grpCustomerCredit = new PanelControl();
        lblCustomerOverdue = new LabelControl();
        grpCustomerCommercialSummary = new PanelControl();
        lblCustomerCommercialSummaryHint = new LabelControl();
        grpCustomerCommercialNotes = new PanelControl();
        xtpAccounting = new XtraTabPage();
        sluReceivableAccount = new SearchLookUpEdit();
        sluReceivableAccountView = new GridView();
        grvReceivableAccountLookup = sluReceivableAccountView;
        sluCustomerAdvanceAccount = new SearchLookUpEdit();
        sluCustomerAdvanceAccountView = new GridView();
        grvCustomerAdvanceAccountLookup = sluCustomerAdvanceAccountView;
        sluDiscountAccount = new SearchLookUpEdit();
        sluDiscountAccountView = new GridView();
        grvDiscountAccountLookup = sluDiscountAccountView;
        sluInterestAccount = new SearchLookUpEdit();
        sluInterestAccountView = new GridView();
        grvInterestAccountLookup = sluInterestAccountView;
        lueCostCenter = new LookUpEdit();
        lueProject = new LookUpEdit();
        sluIncomeWithholding = new SearchLookUpEdit();
        sluIncomeWithholdingView = new GridView();
        grvIncomeWithholdingLookup = sluIncomeWithholdingView;
        sluVatWithholding = new SearchLookUpEdit();
        sluVatWithholdingView = new GridView();
        grvVatWithholdingLookup = sluVatWithholdingView;
        lueIcaWithholding = new LookUpEdit();
        lueAccountingCurrency = new LookUpEdit();
        spnExchangeRate = new SpinEdit();
        lueValidationStatus = new LookUpEdit();
        grpCustomerAccounts = new PanelControl();
        lblCustomerReceivable = new LabelControl();
        lblCustomerAdvance = new LabelControl();
        lblCustomerDiscount = new LabelControl();
        lblCustomerInterest = new LabelControl();
        grpCustomerAssignments = new PanelControl();
        lblCustomerCostCenter = new LabelControl();
        lblCustomerProject = new LabelControl();
        grpCustomerWithholdings = new PanelControl();
        lblCustomerIncomeWh = new LabelControl();
        lblCustomerVatWh = new LabelControl();
        lblCustomerIcaWh = new LabelControl();
        grpCustomerCurrency = new PanelControl();
        lblCustomerAccountingCurrency = new LabelControl();
        lblCustomerExchangeRate = new LabelControl();
        lblCustomerValidation = new LabelControl();
        xtpSap = new XtraTabPage();
        txtSapCardCode = new TextEdit();
        lueSapGroup = new LookUpEdit();
        lueSapPaymentTerm = new LookUpEdit();
        lueSapCurrency = new LookUpEdit();
        lueSapStatus = new LookUpEdit();
        dtpSapLastSync = new DateEdit();
        txtSapUser = new TextEdit();
        txtSapSourceSystem = new TextEdit();
        txtSapCompany = new TextEdit();
        btnSyncSap = new SimpleButton();
        btnValidateSap = new SimpleButton();
        btnOpenSap = new SimpleButton();
        grdCustomerSapLog = new GridControl();
        grvCustomerSapLog = new GridView();
        grpCustomerSapSync = new PanelControl();
        lblCustomerSapCard = new LabelControl();
        lblCustomerSapGroup = new LabelControl();
        lblCustomerSapTerm = new LabelControl();
        lblCustomerSapCurrency = new LabelControl();
        grpCustomerSapStatus = new PanelControl();
        lblCustomerSapStatus = new LabelControl();
        lblCustomerSapLastSync = new LabelControl();
        lblCustomerSapUser = new LabelControl();
        lblCustomerSapSource = new LabelControl();
        lblCustomerSapCompany = new LabelControl();
        grpCustomerSapTools = new PanelControl();
        grpCustomerSapLog = new PanelControl();
        pnlFooter = new PanelControl();
        btnSave = new SimpleButton();
        btnCancel = new SimpleButton();
        ((System.ComponentModel.ISupportInitialize)pnlMain).BeginInit();
        pnlMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpGeneralInfo).BeginInit();
        grpGeneralInfo.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)txtCustomerCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCustomerName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtCustomerCommercialName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueIdentificationType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtIdentificationNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCustomerGroup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCustomerGroupView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesPerson.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesPersonView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPhone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtEmail.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtShortAddress.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grpClassification).BeginInit();
        grpClassification.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueCustomerType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePriceList.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePaymentTerm.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditLimit.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueChannel.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueZone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRiskLevel.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsAllowSales.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsCreditBlocked.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsTaxExempt.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsStrategicCustomer.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)xtcMain).BeginInit();
        xtcMain.SuspendLayout();
        xtpGeneral.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picCustomerLogo.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memObservations.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtpStartDate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtpStartDate.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCustomerOrigin.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAbcClassification.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseFrequency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCurrentBalance.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditAvailable.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtpLastPurchase.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtpLastPurchase.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnOpenOrders.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnYtdSales.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdCustomerContacts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvCustomerContacts).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memInternalNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSegment.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueInternalClassification.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memCommercialTerms.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtTags.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerImage).BeginInit();
        grpCustomerImage.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerObservations).BeginInit();
        grpCustomerObservations.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerComplement).BeginInit();
        grpCustomerComplement.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerSummary).BeginInit();
        grpCustomerSummary.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerPreview).BeginInit();
        grpCustomerPreview.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerTerms).BeginInit();
        grpCustomerTerms.SuspendLayout();
        xtpFiscal.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueTaxpayerType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalRegime.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsAccountingRequired.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsWithholdingAgent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsSubjectToWithholding.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnWithholdingPercent.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueRentType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalProvince.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memFiscalAddress.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtFiscalPostalCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueEmissionType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDefaultSeries.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnInitialNumber.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePrintFormat.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memFiscalNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerFiscalInfo).BeginInit();
        grpCustomerFiscalInfo.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerRetentions).BeginInit();
        grpCustomerRetentions.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerFiscalLocation).BeginInit();
        grpCustomerFiscalLocation.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerFiscalDocuments).BeginInit();
        grpCustomerFiscalDocuments.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerFiscalNotes).BeginInit();
        grpCustomerFiscalNotes.SuspendLayout();
        xtpAddresses.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdCustomerAddresses).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvCustomerAddresses).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAddressType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memAddress.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAddressCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAddressProvince.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAddressCity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtPostalCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtAddressReference.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsPrimaryAddress.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerAddressList).BeginInit();
        grpCustomerAddressList.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerAddressDetail).BeginInit();
        grpCustomerAddressDetail.SuspendLayout();
        xtpContacts.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grdCustomerContactList).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvCustomerContactList).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactPosition.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactPhone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactMobile.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtContactEmail.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsPrimaryContact.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tsActiveContact.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memContactNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerContactList).BeginInit();
        grpCustomerContactList.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerContactDetail).BeginInit();
        grpCustomerContactDetail.SuspendLayout();
        xtpCommercial.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)spnOverdueDays.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memCommercialNotes.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerCommercialConditions).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerCredit).BeginInit();
        grpCustomerCredit.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerCommercialSummary).BeginInit();
        grpCustomerCommercialSummary.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerCommercialNotes).BeginInit();
        grpCustomerCommercialNotes.SuspendLayout();
        xtpAccounting.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)sluReceivableAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluReceivableAccountView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluCustomerAdvanceAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluCustomerAdvanceAccountView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluDiscountAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluDiscountAccountView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluInterestAccount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluInterestAccountView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCostCenter.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueProject.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluIncomeWithholding.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluIncomeWithholdingView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluVatWithholding.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)sluVatWithholdingView).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueIcaWithholding.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)spnExchangeRate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueValidationStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerAccounts).BeginInit();
        grpCustomerAccounts.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerAssignments).BeginInit();
        grpCustomerAssignments.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerWithholdings).BeginInit();
        grpCustomerWithholdings.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerCurrency).BeginInit();
        grpCustomerCurrency.SuspendLayout();
        xtpSap.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)txtSapCardCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapGroup.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapPaymentTerm.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueSapStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtpSapLastSync.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dtpSapLastSync.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapUser.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapSourceSystem.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCompany.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grdCustomerSapLog).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grvCustomerSapLog).BeginInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerSapSync).BeginInit();
        grpCustomerSapSync.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerSapStatus).BeginInit();
        grpCustomerSapStatus.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerSapTools).BeginInit();
        grpCustomerSapTools.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerSapLog).BeginInit();
        grpCustomerSapLog.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlFooter).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();
        //
        // pnlMain
        //
        pnlMain.Appearance.BackColor = Color.White;
        pnlMain.Appearance.Options.UseBackColor = true;
        pnlMain.BorderStyle = BorderStyles.NoBorder;
        pnlMain.Controls.Add(grpGeneralInfo);
        pnlMain.Controls.Add(grpClassification);
        pnlMain.Controls.Add(xtcMain);
        pnlMain.Controls.Add(pnlFooter);
        pnlMain.Dock = DockStyle.Fill;
        pnlMain.Location = new Point(0, 0);
        pnlMain.Name = "pnlMain";
        pnlMain.Size = new Size(1286, 737);
        pnlMain.TabIndex = 0;
        //
        // grpGeneralInfo
        //
        grpGeneralInfo.Controls.Add(lblGeneralInfoTitle);
        grpGeneralInfo.Controls.Add(lblMasterSyncStatus);
        grpGeneralInfo.Controls.Add(lblMasterSyncMessage);
        grpGeneralInfo.Controls.Add(lblCustomerCode);
        grpGeneralInfo.Controls.Add(txtCustomerCode);
        grpGeneralInfo.Controls.Add(lblCustomerName);
        grpGeneralInfo.Controls.Add(txtCustomerName);
        grpGeneralInfo.Controls.Add(lblCustomerCommercialName);
        grpGeneralInfo.Controls.Add(txtCustomerCommercialName);
        grpGeneralInfo.Controls.Add(lblIdentificationType);
        grpGeneralInfo.Controls.Add(lueIdentificationType);
        grpGeneralInfo.Controls.Add(lblIdentificationNumber);
        grpGeneralInfo.Controls.Add(txtIdentificationNumber);
        grpGeneralInfo.Controls.Add(lblExternalCode);
        grpGeneralInfo.Controls.Add(txtExternalCode);
        grpGeneralInfo.Controls.Add(lblCustomerGroup);
        grpGeneralInfo.Controls.Add(lueCustomerGroup);
        grpGeneralInfo.Controls.Add(lblSalesPerson);
        grpGeneralInfo.Controls.Add(lueSalesPerson);
        grpGeneralInfo.Controls.Add(lblPhone);
        grpGeneralInfo.Controls.Add(txtPhone);
        grpGeneralInfo.Controls.Add(lblEmail);
        grpGeneralInfo.Controls.Add(txtEmail);
        grpGeneralInfo.Controls.Add(lblShortAddress);
        grpGeneralInfo.Controls.Add(txtShortAddress);
        grpGeneralInfo.Controls.Add(lblStatus);
        grpGeneralInfo.Controls.Add(lueStatus);
        grpGeneralInfo.Location = new Point(14, 12);
        grpGeneralInfo.Name = "grpGeneralInfo";
        grpGeneralInfo.Size = new Size(555, 268);
        grpGeneralInfo.TabIndex = 0;
        //
        // lblGeneralInfoTitle
        //
        lblGeneralInfoTitle.Appearance.Font = new Font("Segoe UI", 10F);
        lblGeneralInfoTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblGeneralInfoTitle.Appearance.Options.UseFont = true;
        lblGeneralInfoTitle.Appearance.Options.UseForeColor = true;
        lblGeneralInfoTitle.Location = new Point(15, 5);
        lblGeneralInfoTitle.Name = "lblGeneralInfoTitle";
        lblGeneralInfoTitle.Size = new Size(117, 17);
        lblGeneralInfoTitle.TabIndex = 0;
        lblGeneralInfoTitle.Text = "Informacion general";
        //
        // lblMasterSyncStatus
        //
        lblMasterSyncStatus.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lblMasterSyncStatus.Appearance.ForeColor = Color.FromArgb(0, 120, 215);
        lblMasterSyncStatus.Appearance.Options.UseFont = true;
        lblMasterSyncStatus.Appearance.Options.UseForeColor = true;
        lblMasterSyncStatus.AutoSizeMode = LabelAutoSizeMode.None;
        lblMasterSyncStatus.Location = new Point(255, 4);
        lblMasterSyncStatus.Name = "lblMasterSyncStatus";
        lblMasterSyncStatus.Size = new Size(145, 20);
        lblMasterSyncStatus.TabIndex = 25;
        lblMasterSyncStatus.Text = "Aceptado";
        //
        // lblMasterSyncMessage
        //
        lblMasterSyncMessage.Appearance.Font = new Font("Segoe UI", 8F);
        lblMasterSyncMessage.Appearance.ForeColor = Color.FromArgb(87, 96, 111);
        lblMasterSyncMessage.Appearance.Options.UseFont = true;
        lblMasterSyncMessage.Appearance.Options.UseForeColor = true;
        lblMasterSyncMessage.AutoSizeMode = LabelAutoSizeMode.None;
        lblMasterSyncMessage.Location = new Point(407, 4);
        lblMasterSyncMessage.Name = "lblMasterSyncMessage";
        lblMasterSyncMessage.Size = new Size(130, 20);
        lblMasterSyncMessage.TabIndex = 26;
        //
        // lblCustomerCode
        //
        lblCustomerCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerCode.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerCode.Appearance.Options.UseFont = true;
        lblCustomerCode.Appearance.Options.UseForeColor = true;
        lblCustomerCode.Location = new Point(15, 35);
        lblCustomerCode.Name = "lblCustomerCode";
        lblCustomerCode.Size = new Size(96, 15);
        lblCustomerCode.TabIndex = 1;
        lblCustomerCode.Text = "Codigo del cliente";
        //
        // txtCustomerCode
        //
        txtCustomerCode.Location = new Point(155, 32);
        txtCustomerCode.Name = "txtCustomerCode";
        txtCustomerCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCustomerCode.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtCustomerCode.Properties.Appearance.Options.UseFont = true;
        txtCustomerCode.Properties.Appearance.Options.UseForeColor = true;
        txtCustomerCode.Properties.NullValuePrompt = "Se asigna al guardar";
        txtCustomerCode.Properties.ReadOnly = true;
        txtCustomerCode.Properties.ShowNullValuePromptWhenFocused = true;
        txtCustomerCode.Size = new Size(360, 22);
        txtCustomerCode.TabIndex = 2;
        //
        // lblCustomerName
        //
        lblCustomerName.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerName.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerName.Appearance.Options.UseFont = true;
        lblCustomerName.Appearance.Options.UseForeColor = true;
        lblCustomerName.Location = new Point(15, 63);
        lblCustomerName.Name = "lblCustomerName";
        lblCustomerName.Size = new Size(101, 15);
        lblCustomerName.TabIndex = 3;
        lblCustomerName.Text = "Nombre del cliente";
        //
        // txtCustomerName
        //
        txtCustomerName.Location = new Point(155, 60);
        txtCustomerName.Name = "txtCustomerName";
        txtCustomerName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCustomerName.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtCustomerName.Properties.Appearance.Options.UseFont = true;
        txtCustomerName.Properties.Appearance.Options.UseForeColor = true;
        txtCustomerName.Size = new Size(360, 22);
        txtCustomerName.TabIndex = 4;
        //
        // lblCustomerCommercialName
        //
        lblCustomerCommercialName.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerCommercialName.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerCommercialName.Appearance.Options.UseFont = true;
        lblCustomerCommercialName.Appearance.Options.UseForeColor = true;
        lblCustomerCommercialName.Location = new Point(15, 91);
        lblCustomerCommercialName.Name = "lblCustomerCommercialName";
        lblCustomerCommercialName.Size = new Size(99, 15);
        lblCustomerCommercialName.TabIndex = 5;
        lblCustomerCommercialName.Text = "Nombre comercial";
        //
        // txtCustomerCommercialName
        //
        txtCustomerCommercialName.Location = new Point(155, 88);
        txtCustomerCommercialName.Name = "txtCustomerCommercialName";
        txtCustomerCommercialName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtCustomerCommercialName.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtCustomerCommercialName.Properties.Appearance.Options.UseFont = true;
        txtCustomerCommercialName.Properties.Appearance.Options.UseForeColor = true;
        txtCustomerCommercialName.Size = new Size(360, 22);
        txtCustomerCommercialName.TabIndex = 6;
        //
        // lblIdentificationType
        //
        lblIdentificationType.Appearance.Font = new Font("Segoe UI", 9F);
        lblIdentificationType.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblIdentificationType.Appearance.Options.UseFont = true;
        lblIdentificationType.Appearance.Options.UseForeColor = true;
        lblIdentificationType.Location = new Point(15, 119);
        lblIdentificationType.Name = "lblIdentificationType";
        lblIdentificationType.Size = new Size(115, 15);
        lblIdentificationType.TabIndex = 7;
        lblIdentificationType.Text = "Tipo de identificacion";
        //
        // lueIdentificationType
        //
        lueIdentificationType.Location = new Point(155, 116);
        lueIdentificationType.Name = "lueIdentificationType";
        lueIdentificationType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueIdentificationType.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueIdentificationType.Properties.Appearance.Options.UseFont = true;
        lueIdentificationType.Properties.Appearance.Options.UseForeColor = true;
        lueIdentificationType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueIdentificationType.Size = new Size(150, 22);
        lueIdentificationType.TabIndex = 8;
        //
        // lblIdentificationNumber
        //
        lblIdentificationNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblIdentificationNumber.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblIdentificationNumber.Appearance.Options.UseFont = true;
        lblIdentificationNumber.Appearance.Options.UseForeColor = true;
        lblIdentificationNumber.Location = new Point(311, 119);
        lblIdentificationNumber.Name = "lblIdentificationNumber";
        lblIdentificationNumber.Size = new Size(72, 15);
        lblIdentificationNumber.TabIndex = 9;
        lblIdentificationNumber.Text = "Identificacion";
        //
        // txtIdentificationNumber
        //
        txtIdentificationNumber.Location = new Point(394, 116);
        txtIdentificationNumber.Name = "txtIdentificationNumber";
        txtIdentificationNumber.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtIdentificationNumber.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtIdentificationNumber.Properties.Appearance.Options.UseFont = true;
        txtIdentificationNumber.Properties.Appearance.Options.UseForeColor = true;
        txtIdentificationNumber.Size = new Size(121, 22);
        txtIdentificationNumber.TabIndex = 10;
        //
        // lblExternalCode
        //
        lblExternalCode.Appearance.Font = new Font("Segoe UI", 9F);
        lblExternalCode.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblExternalCode.Appearance.Options.UseFont = true;
        lblExternalCode.Appearance.Options.UseForeColor = true;
        lblExternalCode.Location = new Point(15, 147);
        lblExternalCode.Name = "lblExternalCode";
        lblExternalCode.Size = new Size(81, 15);
        lblExternalCode.TabIndex = 11;
        lblExternalCode.Text = "Codigo externo";
        //
        // txtExternalCode
        //
        txtExternalCode.Location = new Point(155, 144);
        txtExternalCode.Name = "txtExternalCode";
        txtExternalCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtExternalCode.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtExternalCode.Properties.Appearance.Options.UseFont = true;
        txtExternalCode.Properties.Appearance.Options.UseForeColor = true;
        txtExternalCode.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton() });
        txtExternalCode.Size = new Size(360, 22);
        txtExternalCode.TabIndex = 12;
        //
        // lblCustomerGroup
        //
        lblCustomerGroup.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerGroup.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerGroup.Appearance.Options.UseFont = true;
        lblCustomerGroup.Appearance.Options.UseForeColor = true;
        lblCustomerGroup.Location = new Point(15, 176);
        lblCustomerGroup.Name = "lblCustomerGroup";
        lblCustomerGroup.Size = new Size(92, 15);
        lblCustomerGroup.TabIndex = 13;
        lblCustomerGroup.Text = "Grupo de clientes";
        //
        // lueCustomerGroup
        //
        lueCustomerGroup.Location = new Point(155, 172);
        lueCustomerGroup.Name = "lueCustomerGroup";
        lueCustomerGroup.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCustomerGroup.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueCustomerGroup.Properties.Appearance.Options.UseFont = true;
        lueCustomerGroup.Properties.Appearance.Options.UseForeColor = true;
        lueCustomerGroup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCustomerGroup.Properties.PopupView = lueCustomerGroupView;
        lueCustomerGroup.Size = new Size(360, 22);
        lueCustomerGroup.TabIndex = 14;
        //
        // lueCustomerGroupView
        //
        lueCustomerGroupView.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        lueCustomerGroupView.Appearance.FilterPanel.Options.UseFont = true;
        lueCustomerGroupView.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lueCustomerGroupView.Appearance.FooterPanel.Options.UseFont = true;
        lueCustomerGroupView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lueCustomerGroupView.Appearance.HeaderPanel.Options.UseFont = true;
        lueCustomerGroupView.Appearance.Row.Font = new Font("Segoe UI", 9F);
        lueCustomerGroupView.Appearance.Row.Options.UseFont = true;
        lueCustomerGroupView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        lueCustomerGroupView.Name = "lueCustomerGroupView";
        lueCustomerGroupView.OptionsSelection.EnableAppearanceFocusedCell = false;
        lueCustomerGroupView.OptionsView.ShowGroupPanel = false;
        //
        // lblSalesPerson
        //
        lblSalesPerson.Appearance.Font = new Font("Segoe UI", 9F);
        lblSalesPerson.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblSalesPerson.Appearance.Options.UseFont = true;
        lblSalesPerson.Appearance.Options.UseForeColor = true;
        lblSalesPerson.Location = new Point(15, 203);
        lblSalesPerson.Name = "lblSalesPerson";
        lblSalesPerson.Size = new Size(51, 15);
        lblSalesPerson.TabIndex = 15;
        lblSalesPerson.Text = "Vendedor";
        //
        // lueSalesPerson
        //
        lueSalesPerson.Location = new Point(155, 200);
        lueSalesPerson.Name = "lueSalesPerson";
        lueSalesPerson.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSalesPerson.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueSalesPerson.Properties.Appearance.Options.UseFont = true;
        lueSalesPerson.Properties.Appearance.Options.UseForeColor = true;
        lueSalesPerson.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSalesPerson.Properties.PopupView = lueSalesPersonView;
        lueSalesPerson.Size = new Size(360, 22);
        lueSalesPerson.TabIndex = 16;
        //
        // lueSalesPersonView
        //
        lueSalesPersonView.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        lueSalesPersonView.Appearance.FilterPanel.Options.UseFont = true;
        lueSalesPersonView.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lueSalesPersonView.Appearance.FooterPanel.Options.UseFont = true;
        lueSalesPersonView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        lueSalesPersonView.Appearance.HeaderPanel.Options.UseFont = true;
        lueSalesPersonView.Appearance.Row.Font = new Font("Segoe UI", 9F);
        lueSalesPersonView.Appearance.Row.Options.UseFont = true;
        lueSalesPersonView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        lueSalesPersonView.Name = "lueSalesPersonView";
        lueSalesPersonView.OptionsSelection.EnableAppearanceFocusedCell = false;
        lueSalesPersonView.OptionsView.ShowGroupPanel = false;
        //
        // lblPhone
        //
        lblPhone.Appearance.Font = new Font("Segoe UI", 9F);
        lblPhone.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblPhone.Appearance.Options.UseFont = true;
        lblPhone.Appearance.Options.UseForeColor = true;
        lblPhone.Location = new Point(16, 234);
        lblPhone.Name = "lblPhone";
        lblPhone.Size = new Size(47, 15);
        lblPhone.TabIndex = 17;
        lblPhone.Text = "Telefono";
        //
        // txtPhone
        //
        txtPhone.Location = new Point(171, 230);
        txtPhone.Name = "txtPhone";
        txtPhone.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPhone.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtPhone.Properties.Appearance.Options.UseFont = true;
        txtPhone.Properties.Appearance.Options.UseForeColor = true;
        txtPhone.Size = new Size(160, 22);
        txtPhone.TabIndex = 18;
        //
        // lblEmail
        //
        lblEmail.Appearance.Font = new Font("Segoe UI", 9F);
        lblEmail.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblEmail.Appearance.Options.UseFont = true;
        lblEmail.Appearance.Options.UseForeColor = true;
        lblEmail.Location = new Point(330, 234);
        lblEmail.Name = "lblEmail";
        lblEmail.Size = new Size(36, 15);
        lblEmail.TabIndex = 19;
        lblEmail.Text = "Correo";
        //
        // txtEmail
        //
        txtEmail.Location = new Point(400, 230);
        txtEmail.Name = "txtEmail";
        txtEmail.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtEmail.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtEmail.Properties.Appearance.Options.UseFont = true;
        txtEmail.Properties.Appearance.Options.UseForeColor = true;
        txtEmail.Size = new Size(131, 22);
        txtEmail.TabIndex = 20;
        //
        // lblShortAddress
        //
        lblShortAddress.Appearance.Font = new Font("Segoe UI", 9F);
        lblShortAddress.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblShortAddress.Appearance.Options.UseFont = true;
        lblShortAddress.Appearance.Options.UseForeColor = true;
        lblShortAddress.Location = new Point(16, 258);
        lblShortAddress.Name = "lblShortAddress";
        lblShortAddress.Size = new Size(80, 15);
        lblShortAddress.TabIndex = 21;
        lblShortAddress.Text = "Direccion corta";
        //
        // txtShortAddress
        //
        txtShortAddress.Location = new Point(171, 254);
        txtShortAddress.Name = "txtShortAddress";
        txtShortAddress.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtShortAddress.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtShortAddress.Properties.Appearance.Options.UseFont = true;
        txtShortAddress.Properties.Appearance.Options.UseForeColor = true;
        txtShortAddress.Size = new Size(250, 22);
        txtShortAddress.TabIndex = 22;
        //
        // lblStatus
        //
        lblStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblStatus.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblStatus.Appearance.Options.UseFont = true;
        lblStatus.Appearance.Options.UseForeColor = true;
        lblStatus.Location = new Point(430, 258);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(35, 15);
        lblStatus.TabIndex = 23;
        lblStatus.Text = "Estado";
        //
        // lueStatus
        //
        lueStatus.Location = new Point(485, 254);
        lueStatus.Name = "lueStatus";
        lueStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueStatus.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueStatus.Properties.Appearance.Options.UseFont = true;
        lueStatus.Properties.Appearance.Options.UseForeColor = true;
        lueStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueStatus.Size = new Size(46, 22);
        lueStatus.TabIndex = 24;
        //
        // grpClassification
        //
        grpClassification.Controls.Add(lblClassificationTitle);
        grpClassification.Controls.Add(lblCustomerType);
        grpClassification.Controls.Add(lueCustomerType);
        grpClassification.Controls.Add(lblPriceList);
        grpClassification.Controls.Add(luePriceList);
        grpClassification.Controls.Add(lblPaymentTerm);
        grpClassification.Controls.Add(luePaymentTerm);
        grpClassification.Controls.Add(lblCurrency);
        grpClassification.Controls.Add(lueCurrency);
        grpClassification.Controls.Add(lblCreditLimit);
        grpClassification.Controls.Add(spnCreditLimit);
        grpClassification.Controls.Add(lblChannel);
        grpClassification.Controls.Add(lueChannel);
        grpClassification.Controls.Add(lblZone);
        grpClassification.Controls.Add(lueZone);
        grpClassification.Controls.Add(lblRisk);
        grpClassification.Controls.Add(lueRiskLevel);
        grpClassification.Controls.Add(lblAllowSales);
        grpClassification.Controls.Add(tsAllowSales);
        grpClassification.Controls.Add(lblCreditBlocked);
        grpClassification.Controls.Add(tsCreditBlocked);
        grpClassification.Controls.Add(lblTaxExempt);
        grpClassification.Controls.Add(tsTaxExempt);
        grpClassification.Controls.Add(lblStrategic);
        grpClassification.Controls.Add(tsStrategicCustomer);
        grpClassification.Location = new Point(580, 12);
        grpClassification.Name = "grpClassification";
        grpClassification.Size = new Size(694, 268);
        grpClassification.TabIndex = 1;
        //
        // lblClassificationTitle
        //
        lblClassificationTitle.Appearance.Font = new Font("Segoe UI", 10F);
        lblClassificationTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblClassificationTitle.Appearance.Options.UseFont = true;
        lblClassificationTitle.Appearance.Options.UseForeColor = true;
        lblClassificationTitle.Location = new Point(5, 5);
        lblClassificationTitle.Name = "lblClassificationTitle";
        lblClassificationTitle.Size = new Size(126, 17);
        lblClassificationTitle.TabIndex = 0;
        lblClassificationTitle.Text = "Clasificacion y control";
        //
        // lblCustomerType
        //
        lblCustomerType.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerType.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerType.Appearance.Options.UseFont = true;
        lblCustomerType.Appearance.Options.UseForeColor = true;
        lblCustomerType.Location = new Point(5, 28);
        lblCustomerType.Name = "lblCustomerType";
        lblCustomerType.Size = new Size(78, 15);
        lblCustomerType.TabIndex = 1;
        lblCustomerType.Text = "Tipo de cliente";
        //
        // lueCustomerType
        //
        lueCustomerType.Location = new Point(132, 25);
        lueCustomerType.Name = "lueCustomerType";
        lueCustomerType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCustomerType.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueCustomerType.Properties.Appearance.Options.UseFont = true;
        lueCustomerType.Properties.Appearance.Options.UseForeColor = true;
        lueCustomerType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCustomerType.Size = new Size(230, 22);
        lueCustomerType.TabIndex = 2;
        //
        // lblPriceList
        //
        lblPriceList.Appearance.Font = new Font("Segoe UI", 9F);
        lblPriceList.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblPriceList.Appearance.Options.UseFont = true;
        lblPriceList.Appearance.Options.UseForeColor = true;
        lblPriceList.Location = new Point(18, 66);
        lblPriceList.Name = "lblPriceList";
        lblPriceList.Size = new Size(81, 15);
        lblPriceList.TabIndex = 3;
        lblPriceList.Text = "Lista de precios";
        //
        // luePriceList
        //
        luePriceList.Location = new Point(163, 62);
        luePriceList.Name = "luePriceList";
        luePriceList.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePriceList.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        luePriceList.Properties.Appearance.Options.UseFont = true;
        luePriceList.Properties.Appearance.Options.UseForeColor = true;
        luePriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePriceList.Size = new Size(230, 22);
        luePriceList.TabIndex = 4;
        //
        // lblPaymentTerm
        //
        lblPaymentTerm.Appearance.Font = new Font("Segoe UI", 9F);
        lblPaymentTerm.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblPaymentTerm.Appearance.Options.UseFont = true;
        lblPaymentTerm.Appearance.Options.UseForeColor = true;
        lblPaymentTerm.Location = new Point(18, 94);
        lblPaymentTerm.Name = "lblPaymentTerm";
        lblPaymentTerm.Size = new Size(101, 15);
        lblPaymentTerm.TabIndex = 5;
        lblPaymentTerm.Text = "Condicion de pago";
        //
        // luePaymentTerm
        //
        luePaymentTerm.Location = new Point(163, 90);
        luePaymentTerm.Name = "luePaymentTerm";
        luePaymentTerm.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePaymentTerm.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        luePaymentTerm.Properties.Appearance.Options.UseFont = true;
        luePaymentTerm.Properties.Appearance.Options.UseForeColor = true;
        luePaymentTerm.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePaymentTerm.Size = new Size(230, 22);
        luePaymentTerm.TabIndex = 6;
        //
        // lblCurrency
        //
        lblCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblCurrency.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCurrency.Appearance.Options.UseFont = true;
        lblCurrency.Appearance.Options.UseForeColor = true;
        lblCurrency.Location = new Point(18, 122);
        lblCurrency.Name = "lblCurrency";
        lblCurrency.Size = new Size(44, 15);
        lblCurrency.TabIndex = 7;
        lblCurrency.Text = "Moneda";
        //
        // lueCurrency
        //
        lueCurrency.Location = new Point(163, 118);
        lueCurrency.Name = "lueCurrency";
        lueCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCurrency.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueCurrency.Properties.Appearance.Options.UseFont = true;
        lueCurrency.Properties.Appearance.Options.UseForeColor = true;
        lueCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCurrency.Size = new Size(230, 22);
        lueCurrency.TabIndex = 8;
        //
        // lblCreditLimit
        //
        lblCreditLimit.Appearance.Font = new Font("Segoe UI", 9F);
        lblCreditLimit.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCreditLimit.Appearance.Options.UseFont = true;
        lblCreditLimit.Appearance.Options.UseForeColor = true;
        lblCreditLimit.Location = new Point(18, 150);
        lblCreditLimit.Name = "lblCreditLimit";
        lblCreditLimit.Size = new Size(89, 15);
        lblCreditLimit.TabIndex = 9;
        lblCreditLimit.Text = "Limite de credito";
        //
        // spnCreditLimit
        //
        spnCreditLimit.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnCreditLimit.Location = new Point(163, 146);
        spnCreditLimit.Name = "spnCreditLimit";
        spnCreditLimit.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnCreditLimit.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        spnCreditLimit.Properties.Appearance.Options.UseFont = true;
        spnCreditLimit.Properties.Appearance.Options.UseForeColor = true;
        spnCreditLimit.Properties.Appearance.Options.UseTextOptions = true;
        spnCreditLimit.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnCreditLimit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnCreditLimit.Size = new Size(230, 22);
        spnCreditLimit.TabIndex = 10;
        //
        // lblChannel
        //
        lblChannel.Appearance.Font = new Font("Segoe UI", 9F);
        lblChannel.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblChannel.Appearance.Options.UseFont = true;
        lblChannel.Appearance.Options.UseForeColor = true;
        lblChannel.Location = new Point(18, 178);
        lblChannel.Name = "lblChannel";
        lblChannel.Size = new Size(30, 15);
        lblChannel.TabIndex = 11;
        lblChannel.Text = "Canal";
        //
        // lueChannel
        //
        lueChannel.Location = new Point(163, 174);
        lueChannel.Name = "lueChannel";
        lueChannel.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueChannel.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueChannel.Properties.Appearance.Options.UseFont = true;
        lueChannel.Properties.Appearance.Options.UseForeColor = true;
        lueChannel.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueChannel.Size = new Size(230, 22);
        lueChannel.TabIndex = 12;
        //
        // lblZone
        //
        lblZone.Appearance.Font = new Font("Segoe UI", 9F);
        lblZone.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblZone.Appearance.Options.UseFont = true;
        lblZone.Appearance.Options.UseForeColor = true;
        lblZone.Location = new Point(18, 206);
        lblZone.Name = "lblZone";
        lblZone.Size = new Size(27, 15);
        lblZone.TabIndex = 13;
        lblZone.Text = "Zona";
        //
        // lueZone
        //
        lueZone.Location = new Point(163, 202);
        lueZone.Name = "lueZone";
        lueZone.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueZone.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueZone.Properties.Appearance.Options.UseFont = true;
        lueZone.Properties.Appearance.Options.UseForeColor = true;
        lueZone.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueZone.Size = new Size(230, 22);
        lueZone.TabIndex = 14;
        //
        // lblRisk
        //
        lblRisk.Appearance.Font = new Font("Segoe UI", 9F);
        lblRisk.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblRisk.Appearance.Options.UseFont = true;
        lblRisk.Appearance.Options.UseForeColor = true;
        lblRisk.Location = new Point(18, 234);
        lblRisk.Name = "lblRisk";
        lblRisk.Size = new Size(35, 15);
        lblRisk.TabIndex = 15;
        lblRisk.Text = "Riesgo";
        //
        // lueRiskLevel
        //
        lueRiskLevel.Location = new Point(163, 230);
        lueRiskLevel.Name = "lueRiskLevel";
        lueRiskLevel.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRiskLevel.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueRiskLevel.Properties.Appearance.Options.UseFont = true;
        lueRiskLevel.Properties.Appearance.Options.UseForeColor = true;
        lueRiskLevel.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRiskLevel.Size = new Size(230, 22);
        lueRiskLevel.TabIndex = 16;
        //
        // lblAllowSales
        //
        lblAllowSales.Appearance.Font = new Font("Segoe UI", 9F);
        lblAllowSales.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblAllowSales.Appearance.Options.UseFont = true;
        lblAllowSales.Appearance.Options.UseForeColor = true;
        lblAllowSales.Location = new Point(440, 47);
        lblAllowSales.Name = "lblAllowSales";
        lblAllowSales.Size = new Size(79, 15);
        lblAllowSales.TabIndex = 17;
        lblAllowSales.Text = "Permitir ventas";
        //
        // tsAllowSales
        //
        tsAllowSales.Location = new Point(585, 44);
        tsAllowSales.Name = "tsAllowSales";
        tsAllowSales.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsAllowSales.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsAllowSales.Properties.Appearance.Options.UseFont = true;
        tsAllowSales.Properties.Appearance.Options.UseForeColor = true;
        tsAllowSales.Properties.OffText = "";
        tsAllowSales.Properties.OnText = "";
        tsAllowSales.Size = new Size(55, 20);
        tsAllowSales.TabIndex = 18;
        //
        // lblCreditBlocked
        //
        lblCreditBlocked.Appearance.Font = new Font("Segoe UI", 9F);
        lblCreditBlocked.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCreditBlocked.Appearance.Options.UseFont = true;
        lblCreditBlocked.Appearance.Options.UseForeColor = true;
        lblCreditBlocked.Location = new Point(440, 87);
        lblCreditBlocked.Name = "lblCreditBlocked";
        lblCreditBlocked.Size = new Size(87, 15);
        lblCreditBlocked.TabIndex = 19;
        lblCreditBlocked.Text = "Bloquear credito";
        //
        // tsCreditBlocked
        //
        tsCreditBlocked.Location = new Point(585, 84);
        tsCreditBlocked.Name = "tsCreditBlocked";
        tsCreditBlocked.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsCreditBlocked.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsCreditBlocked.Properties.Appearance.Options.UseFont = true;
        tsCreditBlocked.Properties.Appearance.Options.UseForeColor = true;
        tsCreditBlocked.Properties.OffText = "";
        tsCreditBlocked.Properties.OnText = "";
        tsCreditBlocked.Size = new Size(55, 20);
        tsCreditBlocked.TabIndex = 20;
        //
        // lblTaxExempt
        //
        lblTaxExempt.Appearance.Font = new Font("Segoe UI", 9F);
        lblTaxExempt.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTaxExempt.Appearance.Options.UseFont = true;
        lblTaxExempt.Appearance.Options.UseForeColor = true;
        lblTaxExempt.Location = new Point(440, 127);
        lblTaxExempt.Name = "lblTaxExempt";
        lblTaxExempt.Size = new Size(88, 15);
        lblTaxExempt.TabIndex = 21;
        lblTaxExempt.Text = "Exento retencion";
        //
        // tsTaxExempt
        //
        tsTaxExempt.Location = new Point(585, 124);
        tsTaxExempt.Name = "tsTaxExempt";
        tsTaxExempt.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsTaxExempt.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsTaxExempt.Properties.Appearance.Options.UseFont = true;
        tsTaxExempt.Properties.Appearance.Options.UseForeColor = true;
        tsTaxExempt.Properties.OffText = "";
        tsTaxExempt.Properties.OnText = "";
        tsTaxExempt.Size = new Size(55, 20);
        tsTaxExempt.TabIndex = 22;
        //
        // lblStrategic
        //
        lblStrategic.Appearance.Font = new Font("Segoe UI", 9F);
        lblStrategic.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblStrategic.Appearance.Options.UseFont = true;
        lblStrategic.Appearance.Options.UseForeColor = true;
        lblStrategic.Location = new Point(440, 167);
        lblStrategic.Name = "lblStrategic";
        lblStrategic.Size = new Size(98, 15);
        lblStrategic.TabIndex = 23;
        lblStrategic.Text = "Cliente estrategico";
        //
        // tsStrategicCustomer
        //
        tsStrategicCustomer.Location = new Point(585, 164);
        tsStrategicCustomer.Name = "tsStrategicCustomer";
        tsStrategicCustomer.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsStrategicCustomer.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsStrategicCustomer.Properties.Appearance.Options.UseFont = true;
        tsStrategicCustomer.Properties.Appearance.Options.UseForeColor = true;
        tsStrategicCustomer.Properties.OffText = "";
        tsStrategicCustomer.Properties.OnText = "";
        tsStrategicCustomer.Size = new Size(55, 20);
        tsStrategicCustomer.TabIndex = 24;
        //
        // xtcMain
        //
        xtcMain.Location = new Point(14, 286);
        xtcMain.Name = "xtcMain";
        xtcMain.SelectedTabPage = xtpGeneral;
        xtcMain.Size = new Size(1260, 389);
        xtcMain.TabIndex = 2;
        xtcMain.TabPages.AddRange(new XtraTabPage[] { xtpGeneral, xtpFiscal, xtpAddresses, xtpContacts, xtpCommercial, xtpAccounting, xtpSap });
        //
        // xtpGeneral
        //
        xtpGeneral.Controls.Add(grpCustomerImage);
        xtpGeneral.Controls.Add(grpCustomerObservations);
        xtpGeneral.Controls.Add(grpCustomerComplement);
        xtpGeneral.Controls.Add(grpCustomerSummary);
        xtpGeneral.Controls.Add(grpCustomerPreview);
        xtpGeneral.Controls.Add(grpCustomerTerms);
        xtpGeneral.Name = "xtpGeneral";
        xtpGeneral.Size = new Size(1258, 364);
        xtpGeneral.Text = "General";
        //
        // picCustomerLogo
        //
        picCustomerLogo.Location = new Point(18, 32);
        picCustomerLogo.Name = "picCustomerLogo";
        picCustomerLogo.Properties.SizeMode = PictureSizeMode.Squeeze;
        picCustomerLogo.Size = new Size(168, 130);
        picCustomerLogo.TabIndex = 0;
        //
        // btnLoadImage
        //
        btnLoadImage.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnLoadImage.Appearance.Options.UseFont = true;
        btnLoadImage.Location = new Point(18, 174);
        btnLoadImage.Name = "btnLoadImage";
        btnLoadImage.Size = new Size(72, 28);
        btnLoadImage.TabIndex = 1;
        btnLoadImage.Text = "Cargar";
        //
        // btnRemoveImage
        //
        btnRemoveImage.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnRemoveImage.Appearance.Options.UseFont = true;
        btnRemoveImage.Location = new Point(108, 174);
        btnRemoveImage.Name = "btnRemoveImage";
        btnRemoveImage.Size = new Size(72, 28);
        btnRemoveImage.TabIndex = 2;
        btnRemoveImage.Text = "Quitar";
        //
        // memObservations
        //
        memObservations.Location = new Point(16, 32);
        memObservations.Name = "memObservations";
        memObservations.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memObservations.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        memObservations.Properties.Appearance.Options.UseFont = true;
        memObservations.Properties.Appearance.Options.UseForeColor = true;
        memObservations.Size = new Size(328, 70);
        memObservations.TabIndex = 3;
        //
        // dtpStartDate
        //
        dtpStartDate.EditValue = new DateTime(2026, 5, 22, 0, 0, 0, 0);
        dtpStartDate.Location = new Point(140, 32);
        dtpStartDate.Name = "dtpStartDate";
        dtpStartDate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dtpStartDate.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        dtpStartDate.Properties.Appearance.Options.UseFont = true;
        dtpStartDate.Properties.Appearance.Options.UseForeColor = true;
        dtpStartDate.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dtpStartDate.Size = new Size(180, 22);
        dtpStartDate.TabIndex = 4;
        //
        // lueCustomerOrigin
        //
        lueCustomerOrigin.Location = new Point(140, 62);
        lueCustomerOrigin.Name = "lueCustomerOrigin";
        lueCustomerOrigin.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCustomerOrigin.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueCustomerOrigin.Properties.Appearance.Options.UseFont = true;
        lueCustomerOrigin.Properties.Appearance.Options.UseForeColor = true;
        lueCustomerOrigin.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCustomerOrigin.Size = new Size(180, 22);
        lueCustomerOrigin.TabIndex = 5;
        //
        // lueAbcClassification
        //
        lueAbcClassification.Location = new Point(140, 92);
        lueAbcClassification.Name = "lueAbcClassification";
        lueAbcClassification.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAbcClassification.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueAbcClassification.Properties.Appearance.Options.UseFont = true;
        lueAbcClassification.Properties.Appearance.Options.UseForeColor = true;
        lueAbcClassification.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAbcClassification.Size = new Size(180, 22);
        lueAbcClassification.TabIndex = 6;
        //
        // luePurchaseFrequency
        //
        luePurchaseFrequency.Location = new Point(140, 122);
        luePurchaseFrequency.Name = "luePurchaseFrequency";
        luePurchaseFrequency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePurchaseFrequency.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        luePurchaseFrequency.Properties.Appearance.Options.UseFont = true;
        luePurchaseFrequency.Properties.Appearance.Options.UseForeColor = true;
        luePurchaseFrequency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseFrequency.Size = new Size(180, 22);
        luePurchaseFrequency.TabIndex = 7;
        //
        // spnCurrentBalance
        //
        spnCurrentBalance.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnCurrentBalance.Location = new Point(150, 58);
        spnCurrentBalance.Name = "spnCurrentBalance";
        spnCurrentBalance.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnCurrentBalance.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        spnCurrentBalance.Properties.Appearance.Options.UseFont = true;
        spnCurrentBalance.Properties.Appearance.Options.UseForeColor = true;
        spnCurrentBalance.Properties.Appearance.Options.UseTextOptions = true;
        spnCurrentBalance.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnCurrentBalance.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnCurrentBalance.Size = new Size(120, 22);
        spnCurrentBalance.TabIndex = 8;
        //
        // spnCreditAvailable
        //
        spnCreditAvailable.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnCreditAvailable.Location = new Point(276, 58);
        spnCreditAvailable.Name = "spnCreditAvailable";
        spnCreditAvailable.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnCreditAvailable.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        spnCreditAvailable.Properties.Appearance.Options.UseFont = true;
        spnCreditAvailable.Properties.Appearance.Options.UseForeColor = true;
        spnCreditAvailable.Properties.Appearance.Options.UseTextOptions = true;
        spnCreditAvailable.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnCreditAvailable.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnCreditAvailable.Size = new Size(120, 22);
        spnCreditAvailable.TabIndex = 9;
        //
        // dtpLastPurchase
        //
        dtpLastPurchase.EditValue = new DateTime(2026, 5, 22, 0, 0, 0, 0);
        dtpLastPurchase.Location = new Point(402, 58);
        dtpLastPurchase.Name = "dtpLastPurchase";
        dtpLastPurchase.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dtpLastPurchase.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        dtpLastPurchase.Properties.Appearance.Options.UseFont = true;
        dtpLastPurchase.Properties.Appearance.Options.UseForeColor = true;
        dtpLastPurchase.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dtpLastPurchase.Size = new Size(120, 22);
        dtpLastPurchase.TabIndex = 10;
        //
        // spnOpenOrders
        //
        spnOpenOrders.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnOpenOrders.Location = new Point(520, 58);
        spnOpenOrders.Name = "spnOpenOrders";
        spnOpenOrders.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnOpenOrders.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        spnOpenOrders.Properties.Appearance.Options.UseFont = true;
        spnOpenOrders.Properties.Appearance.Options.UseForeColor = true;
        spnOpenOrders.Properties.Appearance.Options.UseTextOptions = true;
        spnOpenOrders.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnOpenOrders.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnOpenOrders.Size = new Size(100, 22);
        spnOpenOrders.TabIndex = 11;
        //
        // spnYtdSales
        //
        spnYtdSales.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnYtdSales.Location = new Point(24, 58);
        spnYtdSales.Name = "spnYtdSales";
        spnYtdSales.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnYtdSales.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        spnYtdSales.Properties.Appearance.Options.UseFont = true;
        spnYtdSales.Properties.Appearance.Options.UseForeColor = true;
        spnYtdSales.Properties.Appearance.Options.UseTextOptions = true;
        spnYtdSales.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnYtdSales.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnYtdSales.Size = new Size(120, 22);
        spnYtdSales.TabIndex = 12;
        //
        // grdCustomerContacts
        //
        grdCustomerContacts.Font = new Font("Segoe UI", 9F);
        grdCustomerContacts.Location = new Point(14, 30);
        grdCustomerContacts.MainView = grvCustomerContacts;
        grdCustomerContacts.Name = "grdCustomerContacts";
        grdCustomerContacts.Size = new Size(592, 128);
        grdCustomerContacts.TabIndex = 13;
        grdCustomerContacts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvCustomerContacts });
        //
        // grvCustomerContacts
        //
        grvCustomerContacts.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        grvCustomerContacts.Appearance.FilterPanel.Options.UseFont = true;
        grvCustomerContacts.Appearance.FooterPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grvCustomerContacts.Appearance.FooterPanel.Options.UseFont = true;
        grvCustomerContacts.Appearance.HeaderPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grvCustomerContacts.Appearance.HeaderPanel.ForeColor = Color.FromArgb(23, 32, 51);
        grvCustomerContacts.Appearance.HeaderPanel.Options.UseFont = true;
        grvCustomerContacts.Appearance.HeaderPanel.Options.UseForeColor = true;
        grvCustomerContacts.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvCustomerContacts.Appearance.Row.ForeColor = Color.FromArgb(23, 32, 51);
        grvCustomerContacts.Appearance.Row.Options.UseFont = true;
        grvCustomerContacts.Appearance.Row.Options.UseForeColor = true;
        grvCustomerContacts.GridControl = grdCustomerContacts;
        grvCustomerContacts.Name = "grvCustomerContacts";
        grvCustomerContacts.OptionsBehavior.Editable = false;
        grvCustomerContacts.OptionsView.ShowGroupPanel = false;
        //
        // memInternalNotes
        //
        memInternalNotes.Location = new Point(14, 32);
        memInternalNotes.Name = "memInternalNotes";
        memInternalNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memInternalNotes.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        memInternalNotes.Properties.Appearance.Options.UseFont = true;
        memInternalNotes.Properties.Appearance.Options.UseForeColor = true;
        memInternalNotes.Size = new Size(320, 62);
        memInternalNotes.TabIndex = 14;
        //
        // lueSegment
        //
        lueSegment.Location = new Point(440, 32);
        lueSegment.Name = "lueSegment";
        lueSegment.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSegment.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueSegment.Properties.Appearance.Options.UseFont = true;
        lueSegment.Properties.Appearance.Options.UseForeColor = true;
        lueSegment.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSegment.Size = new Size(170, 22);
        lueSegment.TabIndex = 15;
        //
        // lueInternalClassification
        //
        lueInternalClassification.Location = new Point(440, 62);
        lueInternalClassification.Name = "lueInternalClassification";
        lueInternalClassification.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueInternalClassification.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueInternalClassification.Properties.Appearance.Options.UseFont = true;
        lueInternalClassification.Properties.Appearance.Options.UseForeColor = true;
        lueInternalClassification.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueInternalClassification.Size = new Size(170, 22);
        lueInternalClassification.TabIndex = 16;
        //
        // memCommercialTerms
        //
        memCommercialTerms.Location = new Point(560, 32);
        memCommercialTerms.Name = "memCommercialTerms";
        memCommercialTerms.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memCommercialTerms.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        memCommercialTerms.Properties.Appearance.Options.UseFont = true;
        memCommercialTerms.Properties.Appearance.Options.UseForeColor = true;
        memCommercialTerms.Size = new Size(360, 62);
        memCommercialTerms.TabIndex = 17;
        //
        // txtTags
        //
        txtTags.Location = new Point(1075, 32);
        txtTags.Name = "txtTags";
        txtTags.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtTags.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtTags.Properties.Appearance.Options.UseFont = true;
        txtTags.Properties.Appearance.Options.UseForeColor = true;
        txtTags.Size = new Size(150, 22);
        txtTags.TabIndex = 18;
        //
        // grpCustomerImage
        //
        grpCustomerImage.Controls.Add(picCustomerLogo);
        grpCustomerImage.Controls.Add(btnLoadImage);
        grpCustomerImage.Controls.Add(btnRemoveImage);
        grpCustomerImage.Location = new Point(14, 14);
        grpCustomerImage.Name = "grpCustomerImage";
        grpCustomerImage.Size = new Size(205, 218);
        grpCustomerImage.TabIndex = 0;
        var grpCustomerImageTitle = new LabelControl();
        grpCustomerImageTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerImageTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerImageTitle.Appearance.Options.UseFont = true;
        grpCustomerImageTitle.Appearance.Options.UseForeColor = true;
        grpCustomerImageTitle.Location = new Point(13, 10);
        grpCustomerImageTitle.Name = "grpCustomerImageTitle";
        grpCustomerImageTitle.Text = "Imagen / Logo";
        grpCustomerImage.Controls.Add(grpCustomerImageTitle);
        grpCustomerImageTitle.BringToFront();
        //
        // grpCustomerObservations
        //
        grpCustomerObservations.Controls.Add(memObservations);
        grpCustomerObservations.Location = new Point(230, 14);
        grpCustomerObservations.Name = "grpCustomerObservations";
        grpCustomerObservations.Size = new Size(360, 118);
        grpCustomerObservations.TabIndex = 1;
        var grpCustomerObservationsTitle = new LabelControl();
        grpCustomerObservationsTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerObservationsTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerObservationsTitle.Appearance.Options.UseFont = true;
        grpCustomerObservationsTitle.Appearance.Options.UseForeColor = true;
        grpCustomerObservationsTitle.Location = new Point(13, 10);
        grpCustomerObservationsTitle.Name = "grpCustomerObservationsTitle";
        grpCustomerObservationsTitle.Text = "Descripcion / Observaciones";
        grpCustomerObservations.Controls.Add(grpCustomerObservationsTitle);
        grpCustomerObservationsTitle.BringToFront();
        //
        // grpCustomerComplement
        //
        grpCustomerComplement.Controls.Add(lblCustomerStartDate);
        grpCustomerComplement.Controls.Add(dtpStartDate);
        grpCustomerComplement.Controls.Add(lblCustomerOrigin);
        grpCustomerComplement.Controls.Add(lueCustomerOrigin);
        grpCustomerComplement.Controls.Add(lblCustomerAbc);
        grpCustomerComplement.Controls.Add(lueAbcClassification);
        grpCustomerComplement.Controls.Add(lblCustomerFrequency);
        grpCustomerComplement.Controls.Add(luePurchaseFrequency);
        grpCustomerComplement.Location = new Point(230, 140);
        grpCustomerComplement.Name = "grpCustomerComplement";
        grpCustomerComplement.Size = new Size(360, 174);
        grpCustomerComplement.TabIndex = 2;
        var grpCustomerComplementTitle = new LabelControl();
        grpCustomerComplementTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerComplementTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerComplementTitle.Appearance.Options.UseFont = true;
        grpCustomerComplementTitle.Appearance.Options.UseForeColor = true;
        grpCustomerComplementTitle.Location = new Point(13, 10);
        grpCustomerComplementTitle.Name = "grpCustomerComplementTitle";
        grpCustomerComplementTitle.Text = "Datos complementarios";
        grpCustomerComplement.Controls.Add(grpCustomerComplementTitle);
        grpCustomerComplementTitle.BringToFront();
        //
        // lblCustomerStartDate
        //
        lblCustomerStartDate.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerStartDate.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerStartDate.Appearance.Options.UseFont = true;
        lblCustomerStartDate.Appearance.Options.UseForeColor = true;
        lblCustomerStartDate.Location = new Point(16, 36);
        lblCustomerStartDate.Name = "lblCustomerStartDate";
        lblCustomerStartDate.Size = new Size(69, 15);
        lblCustomerStartDate.TabIndex = 0;
        lblCustomerStartDate.Text = "Fecha de alta";
        //
        // lblCustomerOrigin
        //
        lblCustomerOrigin.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerOrigin.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerOrigin.Appearance.Options.UseFont = true;
        lblCustomerOrigin.Appearance.Options.UseForeColor = true;
        lblCustomerOrigin.Location = new Point(16, 66);
        lblCustomerOrigin.Name = "lblCustomerOrigin";
        lblCustomerOrigin.Size = new Size(74, 15);
        lblCustomerOrigin.TabIndex = 5;
        lblCustomerOrigin.Text = "Origen cliente";
        //
        // lblCustomerAbc
        //
        lblCustomerAbc.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerAbc.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerAbc.Appearance.Options.UseFont = true;
        lblCustomerAbc.Appearance.Options.UseForeColor = true;
        lblCustomerAbc.Location = new Point(16, 96);
        lblCustomerAbc.Name = "lblCustomerAbc";
        lblCustomerAbc.Size = new Size(93, 15);
        lblCustomerAbc.TabIndex = 6;
        lblCustomerAbc.Text = "Clasificacion ABC";
        //
        // lblCustomerFrequency
        //
        lblCustomerFrequency.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerFrequency.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerFrequency.Appearance.Options.UseFont = true;
        lblCustomerFrequency.Appearance.Options.UseForeColor = true;
        lblCustomerFrequency.Location = new Point(16, 126);
        lblCustomerFrequency.Name = "lblCustomerFrequency";
        lblCustomerFrequency.Size = new Size(101, 15);
        lblCustomerFrequency.TabIndex = 7;
        lblCustomerFrequency.Text = "Frecuencia compra";
        //
        // grpCustomerSummary
        //
        grpCustomerSummary.Controls.Add(lblCustomerSalesYtd);
        grpCustomerSummary.Controls.Add(spnYtdSales);
        grpCustomerSummary.Controls.Add(lblCustomerBalance);
        grpCustomerSummary.Controls.Add(spnCurrentBalance);
        grpCustomerSummary.Controls.Add(lblCustomerAvailable);
        grpCustomerSummary.Controls.Add(spnCreditAvailable);
        grpCustomerSummary.Controls.Add(lblCustomerLastPurchase);
        grpCustomerSummary.Controls.Add(dtpLastPurchase);
        grpCustomerSummary.Controls.Add(lblCustomerOpenOrders);
        grpCustomerSummary.Controls.Add(spnOpenOrders);
        grpCustomerSummary.Location = new Point(610, 14);
        grpCustomerSummary.Name = "grpCustomerSummary";
        grpCustomerSummary.Size = new Size(620, 118);
        grpCustomerSummary.TabIndex = 3;
        var grpCustomerSummaryTitle = new LabelControl();
        grpCustomerSummaryTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerSummaryTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerSummaryTitle.Appearance.Options.UseFont = true;
        grpCustomerSummaryTitle.Appearance.Options.UseForeColor = true;
        grpCustomerSummaryTitle.Location = new Point(13, 10);
        grpCustomerSummaryTitle.Name = "grpCustomerSummaryTitle";
        grpCustomerSummaryTitle.Text = "Resumen comercial";
        grpCustomerSummary.Controls.Add(grpCustomerSummaryTitle);
        grpCustomerSummaryTitle.BringToFront();
        //
        // lblCustomerSalesYtd
        //
        lblCustomerSalesYtd.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSalesYtd.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSalesYtd.Appearance.Options.UseFont = true;
        lblCustomerSalesYtd.Appearance.Options.UseForeColor = true;
        lblCustomerSalesYtd.Location = new Point(24, 32);
        lblCustomerSalesYtd.Name = "lblCustomerSalesYtd";
        lblCustomerSalesYtd.Size = new Size(60, 15);
        lblCustomerSalesYtd.TabIndex = 0;
        lblCustomerSalesYtd.Text = "Ventas YTD";
        //
        // lblCustomerBalance
        //
        lblCustomerBalance.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerBalance.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerBalance.Appearance.Options.UseFont = true;
        lblCustomerBalance.Appearance.Options.UseForeColor = true;
        lblCustomerBalance.Location = new Point(150, 32);
        lblCustomerBalance.Name = "lblCustomerBalance";
        lblCustomerBalance.Size = new Size(64, 15);
        lblCustomerBalance.TabIndex = 13;
        lblCustomerBalance.Text = "Saldo actual";
        //
        // lblCustomerAvailable
        //
        lblCustomerAvailable.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerAvailable.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerAvailable.Appearance.Options.UseFont = true;
        lblCustomerAvailable.Appearance.Options.UseForeColor = true;
        lblCustomerAvailable.Location = new Point(276, 32);
        lblCustomerAvailable.Name = "lblCustomerAvailable";
        lblCustomerAvailable.Size = new Size(56, 15);
        lblCustomerAvailable.TabIndex = 14;
        lblCustomerAvailable.Text = "Disponible";
        //
        // lblCustomerLastPurchase
        //
        lblCustomerLastPurchase.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerLastPurchase.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerLastPurchase.Appearance.Options.UseFont = true;
        lblCustomerLastPurchase.Appearance.Options.UseForeColor = true;
        lblCustomerLastPurchase.Location = new Point(402, 32);
        lblCustomerLastPurchase.Name = "lblCustomerLastPurchase";
        lblCustomerLastPurchase.Size = new Size(79, 15);
        lblCustomerLastPurchase.TabIndex = 15;
        lblCustomerLastPurchase.Text = "Ultima compra";
        //
        // lblCustomerOpenOrders
        //
        lblCustomerOpenOrders.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerOpenOrders.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerOpenOrders.Appearance.Options.UseFont = true;
        lblCustomerOpenOrders.Appearance.Options.UseForeColor = true;
        lblCustomerOpenOrders.Location = new Point(520, 32);
        lblCustomerOpenOrders.Name = "lblCustomerOpenOrders";
        lblCustomerOpenOrders.Size = new Size(42, 15);
        lblCustomerOpenOrders.TabIndex = 16;
        lblCustomerOpenOrders.Text = "Pedidos";
        //
        // grpCustomerPreview
        //
        grpCustomerPreview.Controls.Add(grdCustomerContacts);
        grpCustomerPreview.Location = new Point(610, 140);
        grpCustomerPreview.Name = "grpCustomerPreview";
        grpCustomerPreview.Size = new Size(620, 174);
        grpCustomerPreview.TabIndex = 4;
        var grpCustomerPreviewTitle = new LabelControl();
        grpCustomerPreviewTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerPreviewTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerPreviewTitle.Appearance.Options.UseFont = true;
        grpCustomerPreviewTitle.Appearance.Options.UseForeColor = true;
        grpCustomerPreviewTitle.Location = new Point(13, 10);
        grpCustomerPreviewTitle.Name = "grpCustomerPreviewTitle";
        grpCustomerPreviewTitle.Text = "Contactos / Direcciones";
        grpCustomerPreview.Controls.Add(grpCustomerPreviewTitle);
        grpCustomerPreviewTitle.BringToFront();
        //
        // grpCustomerTerms
        //
        grpCustomerTerms.Controls.Add(memInternalNotes);
        grpCustomerTerms.Controls.Add(lblCustomerSegment);
        grpCustomerTerms.Controls.Add(lueSegment);
        grpCustomerTerms.Controls.Add(lblCustomerInternalClass);
        grpCustomerTerms.Controls.Add(lueInternalClassification);
        grpCustomerTerms.Controls.Add(lblCustomerTags);
        grpCustomerTerms.Controls.Add(txtTags);
        grpCustomerTerms.Location = new Point(14, 238);
        grpCustomerTerms.Name = "grpCustomerTerms";
        grpCustomerTerms.Size = new Size(1216, 112);
        grpCustomerTerms.TabIndex = 5;
        var grpCustomerTermsTitle = new LabelControl();
        grpCustomerTermsTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerTermsTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerTermsTitle.Appearance.Options.UseFont = true;
        grpCustomerTermsTitle.Appearance.Options.UseForeColor = true;
        grpCustomerTermsTitle.Location = new Point(13, 10);
        grpCustomerTermsTitle.Name = "grpCustomerTermsTitle";
        grpCustomerTermsTitle.Text = "Notas internas y terminos";
        grpCustomerTerms.Controls.Add(grpCustomerTermsTitle);
        grpCustomerTermsTitle.BringToFront();
        //
        // lblCustomerSegment
        //
        lblCustomerSegment.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSegment.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSegment.Appearance.Options.UseFont = true;
        lblCustomerSegment.Appearance.Options.UseForeColor = true;
        lblCustomerSegment.Location = new Point(354, 36);
        lblCustomerSegment.Name = "lblCustomerSegment";
        lblCustomerSegment.Size = new Size(54, 15);
        lblCustomerSegment.TabIndex = 15;
        lblCustomerSegment.Text = "Segmento";
        //
        // lblCustomerInternalClass
        //
        lblCustomerInternalClass.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerInternalClass.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerInternalClass.Appearance.Options.UseFont = true;
        lblCustomerInternalClass.Appearance.Options.UseForeColor = true;
        lblCustomerInternalClass.Location = new Point(354, 66);
        lblCustomerInternalClass.Name = "lblCustomerInternalClass";
        lblCustomerInternalClass.Size = new Size(67, 15);
        lblCustomerInternalClass.TabIndex = 16;
        lblCustomerInternalClass.Text = "Clasificacion";
        //
        // lblCustomerTags
        //
        lblCustomerTags.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerTags.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerTags.Appearance.Options.UseFont = true;
        lblCustomerTags.Appearance.Options.UseForeColor = true;
        lblCustomerTags.Location = new Point(1010, 36);
        lblCustomerTags.Name = "lblCustomerTags";
        lblCustomerTags.Size = new Size(48, 15);
        lblCustomerTags.TabIndex = 18;
        lblCustomerTags.Text = "Etiquetas";
        //
        // xtpFiscal
        //
        xtpFiscal.Controls.Add(lblFiscalTitle);
        xtpFiscal.Controls.Add(lblRetentionTitle);
        xtpFiscal.Controls.Add(grpCustomerFiscalInfo);
        xtpFiscal.Controls.Add(grpCustomerRetentions);
        xtpFiscal.Controls.Add(grpCustomerFiscalLocation);
        xtpFiscal.Controls.Add(grpCustomerFiscalDocuments);
        xtpFiscal.Controls.Add(grpCustomerFiscalNotes);
        xtpFiscal.Name = "xtpFiscal";
        xtpFiscal.Size = new Size(1258, 364);
        xtpFiscal.Text = "Fiscal";
        //
        // lblFiscalTitle
        //
        lblFiscalTitle.Appearance.Font = new Font("Segoe UI", 9F);
        lblFiscalTitle.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblFiscalTitle.Appearance.Options.UseFont = true;
        lblFiscalTitle.Appearance.Options.UseForeColor = true;
        lblFiscalTitle.Location = new Point(24, 24);
        lblFiscalTitle.Name = "lblFiscalTitle";
        lblFiscalTitle.Size = new Size(95, 15);
        lblFiscalTitle.TabIndex = 0;
        lblFiscalTitle.Text = "Informacion fiscal";
        //
        // lueTaxpayerType
        //
        lueTaxpayerType.Location = new Point(180, 32);
        lueTaxpayerType.Name = "lueTaxpayerType";
        lueTaxpayerType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueTaxpayerType.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueTaxpayerType.Properties.Appearance.Options.UseFont = true;
        lueTaxpayerType.Properties.Appearance.Options.UseForeColor = true;
        lueTaxpayerType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueTaxpayerType.Size = new Size(180, 22);
        lueTaxpayerType.TabIndex = 1;
        //
        // lueFiscalRegime
        //
        lueFiscalRegime.Location = new Point(180, 62);
        lueFiscalRegime.Name = "lueFiscalRegime";
        lueFiscalRegime.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueFiscalRegime.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueFiscalRegime.Properties.Appearance.Options.UseFont = true;
        lueFiscalRegime.Properties.Appearance.Options.UseForeColor = true;
        lueFiscalRegime.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFiscalRegime.Size = new Size(180, 22);
        lueFiscalRegime.TabIndex = 2;
        //
        // tsAccountingRequired
        //
        tsAccountingRequired.Location = new Point(180, 92);
        tsAccountingRequired.Name = "tsAccountingRequired";
        tsAccountingRequired.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsAccountingRequired.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsAccountingRequired.Properties.Appearance.Options.UseFont = true;
        tsAccountingRequired.Properties.Appearance.Options.UseForeColor = true;
        tsAccountingRequired.Properties.OffText = "";
        tsAccountingRequired.Properties.OnText = "";
        tsAccountingRequired.Size = new Size(95, 20);
        tsAccountingRequired.TabIndex = 3;
        //
        // tsWithholdingAgent
        //
        tsWithholdingAgent.Location = new Point(180, 122);
        tsWithholdingAgent.Name = "tsWithholdingAgent";
        tsWithholdingAgent.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsWithholdingAgent.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsWithholdingAgent.Properties.Appearance.Options.UseFont = true;
        tsWithholdingAgent.Properties.Appearance.Options.UseForeColor = true;
        tsWithholdingAgent.Properties.OffText = "";
        tsWithholdingAgent.Properties.OnText = "";
        tsWithholdingAgent.Size = new Size(95, 20);
        tsWithholdingAgent.TabIndex = 4;
        //
        // lblRetentionTitle
        //
        lblRetentionTitle.Appearance.Font = new Font("Segoe UI", 9F);
        lblRetentionTitle.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblRetentionTitle.Appearance.Options.UseFont = true;
        lblRetentionTitle.Appearance.Options.UseForeColor = true;
        lblRetentionTitle.Location = new Point(320, 24);
        lblRetentionTitle.Name = "lblRetentionTitle";
        lblRetentionTitle.Size = new Size(64, 15);
        lblRetentionTitle.TabIndex = 5;
        lblRetentionTitle.Text = "Retenciones";
        //
        // tsSubjectToWithholding
        //
        tsSubjectToWithholding.Location = new Point(170, 32);
        tsSubjectToWithholding.Name = "tsSubjectToWithholding";
        tsSubjectToWithholding.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsSubjectToWithholding.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsSubjectToWithholding.Properties.Appearance.Options.UseFont = true;
        tsSubjectToWithholding.Properties.Appearance.Options.UseForeColor = true;
        tsSubjectToWithholding.Properties.OffText = "";
        tsSubjectToWithholding.Properties.OnText = "";
        tsSubjectToWithholding.Size = new Size(95, 20);
        tsSubjectToWithholding.TabIndex = 6;
        //
        // spnWithholdingPercent
        //
        spnWithholdingPercent.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnWithholdingPercent.Location = new Point(170, 66);
        spnWithholdingPercent.Name = "spnWithholdingPercent";
        spnWithholdingPercent.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnWithholdingPercent.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        spnWithholdingPercent.Properties.Appearance.Options.UseFont = true;
        spnWithholdingPercent.Properties.Appearance.Options.UseForeColor = true;
        spnWithholdingPercent.Properties.Appearance.Options.UseTextOptions = true;
        spnWithholdingPercent.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnWithholdingPercent.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnWithholdingPercent.Size = new Size(120, 22);
        spnWithholdingPercent.TabIndex = 7;
        //
        // lueRentType
        //
        lueRentType.Location = new Point(170, 98);
        lueRentType.Name = "lueRentType";
        lueRentType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueRentType.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueRentType.Properties.Appearance.Options.UseFont = true;
        lueRentType.Properties.Appearance.Options.UseForeColor = true;
        lueRentType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueRentType.Size = new Size(220, 22);
        lueRentType.TabIndex = 8;
        //
        // lueFiscalCountry
        //
        lueFiscalCountry.Location = new Point(105, 32);
        lueFiscalCountry.Name = "lueFiscalCountry";
        lueFiscalCountry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueFiscalCountry.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueFiscalCountry.Properties.Appearance.Options.UseFont = true;
        lueFiscalCountry.Properties.Appearance.Options.UseForeColor = true;
        lueFiscalCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFiscalCountry.Size = new Size(160, 22);
        lueFiscalCountry.TabIndex = 9;
        //
        // lueFiscalProvince
        //
        lueFiscalProvince.Location = new Point(365, 32);
        lueFiscalProvince.Name = "lueFiscalProvince";
        lueFiscalProvince.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueFiscalProvince.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueFiscalProvince.Properties.Appearance.Options.UseFont = true;
        lueFiscalProvince.Properties.Appearance.Options.UseForeColor = true;
        lueFiscalProvince.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFiscalProvince.Size = new Size(160, 22);
        lueFiscalProvince.TabIndex = 10;
        //
        // lueFiscalCity
        //
        lueFiscalCity.Location = new Point(600, 32);
        lueFiscalCity.Name = "lueFiscalCity";
        lueFiscalCity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueFiscalCity.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueFiscalCity.Properties.Appearance.Options.UseFont = true;
        lueFiscalCity.Properties.Appearance.Options.UseForeColor = true;
        lueFiscalCity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueFiscalCity.Size = new Size(160, 22);
        lueFiscalCity.TabIndex = 11;
        //
        // memFiscalAddress
        //
        memFiscalAddress.Location = new Point(105, 66);
        memFiscalAddress.Name = "memFiscalAddress";
        memFiscalAddress.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memFiscalAddress.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        memFiscalAddress.Properties.Appearance.Options.UseFont = true;
        memFiscalAddress.Properties.Appearance.Options.UseForeColor = true;
        memFiscalAddress.Size = new Size(512, 70);
        memFiscalAddress.TabIndex = 12;
        //
        // txtFiscalPostalCode
        //
        txtFiscalPostalCode.Location = new Point(635, 66);
        txtFiscalPostalCode.Name = "txtFiscalPostalCode";
        txtFiscalPostalCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtFiscalPostalCode.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtFiscalPostalCode.Properties.Appearance.Options.UseFont = true;
        txtFiscalPostalCode.Properties.Appearance.Options.UseForeColor = true;
        txtFiscalPostalCode.Size = new Size(160, 22);
        txtFiscalPostalCode.TabIndex = 13;
        //
        // lueEmissionType
        //
        lueEmissionType.Location = new Point(155, 32);
        lueEmissionType.Name = "lueEmissionType";
        lueEmissionType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueEmissionType.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueEmissionType.Properties.Appearance.Options.UseFont = true;
        lueEmissionType.Properties.Appearance.Options.UseForeColor = true;
        lueEmissionType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueEmissionType.Size = new Size(200, 22);
        lueEmissionType.TabIndex = 14;
        //
        // txtDefaultSeries
        //
        txtDefaultSeries.Location = new Point(155, 62);
        txtDefaultSeries.Name = "txtDefaultSeries";
        txtDefaultSeries.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtDefaultSeries.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtDefaultSeries.Properties.Appearance.Options.UseFont = true;
        txtDefaultSeries.Properties.Appearance.Options.UseForeColor = true;
        txtDefaultSeries.Size = new Size(200, 22);
        txtDefaultSeries.TabIndex = 15;
        //
        // spnInitialNumber
        //
        spnInitialNumber.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnInitialNumber.Location = new Point(155, 92);
        spnInitialNumber.Name = "spnInitialNumber";
        spnInitialNumber.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnInitialNumber.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        spnInitialNumber.Properties.Appearance.Options.UseFont = true;
        spnInitialNumber.Properties.Appearance.Options.UseForeColor = true;
        spnInitialNumber.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnInitialNumber.Size = new Size(200, 22);
        spnInitialNumber.TabIndex = 16;
        //
        // luePrintFormat
        //
        luePrintFormat.Location = new Point(155, 122);
        luePrintFormat.Name = "luePrintFormat";
        luePrintFormat.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        luePrintFormat.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        luePrintFormat.Properties.Appearance.Options.UseFont = true;
        luePrintFormat.Properties.Appearance.Options.UseForeColor = true;
        luePrintFormat.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePrintFormat.Size = new Size(200, 22);
        luePrintFormat.TabIndex = 17;
        //
        // memFiscalNotes
        //
        memFiscalNotes.Location = new Point(16, 32);
        memFiscalNotes.Name = "memFiscalNotes";
        memFiscalNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memFiscalNotes.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        memFiscalNotes.Properties.Appearance.Options.UseFont = true;
        memFiscalNotes.Properties.Appearance.Options.UseForeColor = true;
        memFiscalNotes.Size = new Size(402, 112);
        memFiscalNotes.TabIndex = 18;
        //
        // grpCustomerFiscalInfo
        //
        grpCustomerFiscalInfo.Controls.Add(lblCustomerTaxpayer);
        grpCustomerFiscalInfo.Controls.Add(lueTaxpayerType);
        grpCustomerFiscalInfo.Controls.Add(lblCustomerFiscalRegime);
        grpCustomerFiscalInfo.Controls.Add(lueFiscalRegime);
        grpCustomerFiscalInfo.Controls.Add(lblCustomerAccountingRequired);
        grpCustomerFiscalInfo.Controls.Add(tsAccountingRequired);
        grpCustomerFiscalInfo.Controls.Add(lblCustomerWithholdingAgent);
        grpCustomerFiscalInfo.Controls.Add(tsWithholdingAgent);
        grpCustomerFiscalInfo.Location = new Point(14, 14);
        grpCustomerFiscalInfo.Name = "grpCustomerFiscalInfo";
        grpCustomerFiscalInfo.Size = new Size(380, 162);
        grpCustomerFiscalInfo.TabIndex = 6;
        var grpCustomerFiscalInfoTitle = new LabelControl();
        grpCustomerFiscalInfoTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerFiscalInfoTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerFiscalInfoTitle.Appearance.Options.UseFont = true;
        grpCustomerFiscalInfoTitle.Appearance.Options.UseForeColor = true;
        grpCustomerFiscalInfoTitle.Location = new Point(13, 10);
        grpCustomerFiscalInfoTitle.Name = "grpCustomerFiscalInfoTitle";
        grpCustomerFiscalInfoTitle.Text = "Informacion fiscal";
        grpCustomerFiscalInfo.Controls.Add(grpCustomerFiscalInfoTitle);
        grpCustomerFiscalInfoTitle.BringToFront();
        //
        // lblCustomerTaxpayer
        //
        lblCustomerTaxpayer.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerTaxpayer.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerTaxpayer.Appearance.Options.UseFont = true;
        lblCustomerTaxpayer.Appearance.Options.UseForeColor = true;
        lblCustomerTaxpayer.Location = new Point(18, 36);
        lblCustomerTaxpayer.Name = "lblCustomerTaxpayer";
        lblCustomerTaxpayer.Size = new Size(101, 15);
        lblCustomerTaxpayer.TabIndex = 0;
        lblCustomerTaxpayer.Text = "Tipo contribuyente";
        //
        // lblCustomerFiscalRegime
        //
        lblCustomerFiscalRegime.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerFiscalRegime.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerFiscalRegime.Appearance.Options.UseFont = true;
        lblCustomerFiscalRegime.Appearance.Options.UseForeColor = true;
        lblCustomerFiscalRegime.Location = new Point(18, 66);
        lblCustomerFiscalRegime.Name = "lblCustomerFiscalRegime";
        lblCustomerFiscalRegime.Size = new Size(77, 15);
        lblCustomerFiscalRegime.TabIndex = 2;
        lblCustomerFiscalRegime.Text = "Regimen fiscal";
        //
        // lblCustomerAccountingRequired
        //
        lblCustomerAccountingRequired.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerAccountingRequired.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerAccountingRequired.Appearance.Options.UseFont = true;
        lblCustomerAccountingRequired.Appearance.Options.UseForeColor = true;
        lblCustomerAccountingRequired.Location = new Point(18, 98);
        lblCustomerAccountingRequired.Name = "lblCustomerAccountingRequired";
        lblCustomerAccountingRequired.Size = new Size(96, 15);
        lblCustomerAccountingRequired.TabIndex = 3;
        lblCustomerAccountingRequired.Text = "Lleva contabilidad";
        //
        // lblCustomerWithholdingAgent
        //
        lblCustomerWithholdingAgent.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerWithholdingAgent.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerWithholdingAgent.Appearance.Options.UseFont = true;
        lblCustomerWithholdingAgent.Appearance.Options.UseForeColor = true;
        lblCustomerWithholdingAgent.Location = new Point(18, 128);
        lblCustomerWithholdingAgent.Name = "lblCustomerWithholdingAgent";
        lblCustomerWithholdingAgent.Size = new Size(91, 15);
        lblCustomerWithholdingAgent.TabIndex = 4;
        lblCustomerWithholdingAgent.Text = "Agente retencion";
        //
        // grpCustomerRetentions
        //
        grpCustomerRetentions.Controls.Add(lblCustomerSubjectWithholding);
        grpCustomerRetentions.Controls.Add(tsSubjectToWithholding);
        grpCustomerRetentions.Controls.Add(lblCustomerWithholdingPercent);
        grpCustomerRetentions.Controls.Add(spnWithholdingPercent);
        grpCustomerRetentions.Controls.Add(lblCustomerRentType);
        grpCustomerRetentions.Controls.Add(lueRentType);
        grpCustomerRetentions.Location = new Point(410, 14);
        grpCustomerRetentions.Name = "grpCustomerRetentions";
        grpCustomerRetentions.Size = new Size(370, 162);
        grpCustomerRetentions.TabIndex = 7;
        var grpCustomerRetentionsTitle = new LabelControl();
        grpCustomerRetentionsTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerRetentionsTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerRetentionsTitle.Appearance.Options.UseFont = true;
        grpCustomerRetentionsTitle.Appearance.Options.UseForeColor = true;
        grpCustomerRetentionsTitle.Location = new Point(13, 10);
        grpCustomerRetentionsTitle.Name = "grpCustomerRetentionsTitle";
        grpCustomerRetentionsTitle.Text = "Retenciones";
        grpCustomerRetentions.Controls.Add(grpCustomerRetentionsTitle);
        grpCustomerRetentionsTitle.BringToFront();
        //
        // lblCustomerSubjectWithholding
        //
        lblCustomerSubjectWithholding.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSubjectWithholding.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSubjectWithholding.Appearance.Options.UseFont = true;
        lblCustomerSubjectWithholding.Appearance.Options.UseForeColor = true;
        lblCustomerSubjectWithholding.Location = new Point(18, 38);
        lblCustomerSubjectWithholding.Name = "lblCustomerSubjectWithholding";
        lblCustomerSubjectWithholding.Size = new Size(95, 15);
        lblCustomerSubjectWithholding.TabIndex = 0;
        lblCustomerSubjectWithholding.Text = "Sujeto a retencion";
        //
        // lblCustomerWithholdingPercent
        //
        lblCustomerWithholdingPercent.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerWithholdingPercent.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerWithholdingPercent.Appearance.Options.UseFont = true;
        lblCustomerWithholdingPercent.Appearance.Options.UseForeColor = true;
        lblCustomerWithholdingPercent.Location = new Point(18, 70);
        lblCustomerWithholdingPercent.Name = "lblCustomerWithholdingPercent";
        lblCustomerWithholdingPercent.Size = new Size(56, 15);
        lblCustomerWithholdingPercent.TabIndex = 7;
        lblCustomerWithholdingPercent.Text = "Porcentaje";
        //
        // lblCustomerRentType
        //
        lblCustomerRentType.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerRentType.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerRentType.Appearance.Options.UseFont = true;
        lblCustomerRentType.Appearance.Options.UseForeColor = true;
        lblCustomerRentType.Location = new Point(18, 102);
        lblCustomerRentType.Name = "lblCustomerRentType";
        lblCustomerRentType.Size = new Size(70, 15);
        lblCustomerRentType.TabIndex = 8;
        lblCustomerRentType.Text = "Tipo de renta";
        //
        // grpCustomerFiscalLocation
        //
        grpCustomerFiscalLocation.Controls.Add(lblCustomerFiscalCountry);
        grpCustomerFiscalLocation.Controls.Add(lueFiscalCountry);
        grpCustomerFiscalLocation.Controls.Add(lblCustomerFiscalProvince);
        grpCustomerFiscalLocation.Controls.Add(lueFiscalProvince);
        grpCustomerFiscalLocation.Controls.Add(lblCustomerFiscalCity);
        grpCustomerFiscalLocation.Controls.Add(lueFiscalCity);
        grpCustomerFiscalLocation.Controls.Add(lblCustomerFiscalAddress);
        grpCustomerFiscalLocation.Controls.Add(memFiscalAddress);
        grpCustomerFiscalLocation.Controls.Add(lblCustomerFiscalPostal);
        grpCustomerFiscalLocation.Controls.Add(txtFiscalPostalCode);
        grpCustomerFiscalLocation.Location = new Point(14, 188);
        grpCustomerFiscalLocation.Name = "grpCustomerFiscalLocation";
        grpCustomerFiscalLocation.Size = new Size(766, 162);
        grpCustomerFiscalLocation.TabIndex = 8;
        var grpCustomerFiscalLocationTitle = new LabelControl();
        grpCustomerFiscalLocationTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerFiscalLocationTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerFiscalLocationTitle.Appearance.Options.UseFont = true;
        grpCustomerFiscalLocationTitle.Appearance.Options.UseForeColor = true;
        grpCustomerFiscalLocationTitle.Location = new Point(13, 10);
        grpCustomerFiscalLocationTitle.Name = "grpCustomerFiscalLocationTitle";
        grpCustomerFiscalLocationTitle.Text = "Ubicacion fiscal";
        grpCustomerFiscalLocation.Controls.Add(grpCustomerFiscalLocationTitle);
        grpCustomerFiscalLocationTitle.BringToFront();
        //
        // lblCustomerFiscalCountry
        //
        lblCustomerFiscalCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerFiscalCountry.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerFiscalCountry.Appearance.Options.UseFont = true;
        lblCustomerFiscalCountry.Appearance.Options.UseForeColor = true;
        lblCustomerFiscalCountry.Location = new Point(18, 36);
        lblCustomerFiscalCountry.Name = "lblCustomerFiscalCountry";
        lblCustomerFiscalCountry.Size = new Size(21, 15);
        lblCustomerFiscalCountry.TabIndex = 0;
        lblCustomerFiscalCountry.Text = "Pais";
        //
        // lblCustomerFiscalProvince
        //
        lblCustomerFiscalProvince.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerFiscalProvince.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerFiscalProvince.Appearance.Options.UseFont = true;
        lblCustomerFiscalProvince.Appearance.Options.UseForeColor = true;
        lblCustomerFiscalProvince.Location = new Point(275, 36);
        lblCustomerFiscalProvince.Name = "lblCustomerFiscalProvince";
        lblCustomerFiscalProvince.Size = new Size(49, 15);
        lblCustomerFiscalProvince.TabIndex = 10;
        lblCustomerFiscalProvince.Text = "Provincia";
        //
        // lblCustomerFiscalCity
        //
        lblCustomerFiscalCity.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerFiscalCity.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerFiscalCity.Appearance.Options.UseFont = true;
        lblCustomerFiscalCity.Appearance.Options.UseForeColor = true;
        lblCustomerFiscalCity.Location = new Point(535, 36);
        lblCustomerFiscalCity.Name = "lblCustomerFiscalCity";
        lblCustomerFiscalCity.Size = new Size(38, 15);
        lblCustomerFiscalCity.TabIndex = 11;
        lblCustomerFiscalCity.Text = "Ciudad";
        //
        // lblCustomerFiscalAddress
        //
        lblCustomerFiscalAddress.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerFiscalAddress.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerFiscalAddress.Appearance.Options.UseFont = true;
        lblCustomerFiscalAddress.Appearance.Options.UseForeColor = true;
        lblCustomerFiscalAddress.Location = new Point(18, 70);
        lblCustomerFiscalAddress.Name = "lblCustomerFiscalAddress";
        lblCustomerFiscalAddress.Size = new Size(80, 15);
        lblCustomerFiscalAddress.TabIndex = 12;
        lblCustomerFiscalAddress.Text = "Direccion fiscal";
        //
        // lblCustomerFiscalPostal
        //
        lblCustomerFiscalPostal.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerFiscalPostal.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerFiscalPostal.Appearance.Options.UseFont = true;
        lblCustomerFiscalPostal.Appearance.Options.UseForeColor = true;
        lblCustomerFiscalPostal.Location = new Point(535, 70);
        lblCustomerFiscalPostal.Name = "lblCustomerFiscalPostal";
        lblCustomerFiscalPostal.Size = new Size(74, 15);
        lblCustomerFiscalPostal.TabIndex = 13;
        lblCustomerFiscalPostal.Text = "Codigo postal";
        //
        // grpCustomerFiscalDocuments
        //
        grpCustomerFiscalDocuments.Controls.Add(lblCustomerEmissionType);
        grpCustomerFiscalDocuments.Controls.Add(lueEmissionType);
        grpCustomerFiscalDocuments.Controls.Add(lblCustomerDefaultSeries);
        grpCustomerFiscalDocuments.Controls.Add(txtDefaultSeries);
        grpCustomerFiscalDocuments.Controls.Add(lblCustomerInitialNumber);
        grpCustomerFiscalDocuments.Controls.Add(spnInitialNumber);
        grpCustomerFiscalDocuments.Controls.Add(lblCustomerPrintFormat);
        grpCustomerFiscalDocuments.Controls.Add(luePrintFormat);
        grpCustomerFiscalDocuments.Location = new Point(796, 14);
        grpCustomerFiscalDocuments.Name = "grpCustomerFiscalDocuments";
        grpCustomerFiscalDocuments.Size = new Size(434, 162);
        grpCustomerFiscalDocuments.TabIndex = 9;
        var grpCustomerFiscalDocumentsTitle = new LabelControl();
        grpCustomerFiscalDocumentsTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerFiscalDocumentsTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerFiscalDocumentsTitle.Appearance.Options.UseFont = true;
        grpCustomerFiscalDocumentsTitle.Appearance.Options.UseForeColor = true;
        grpCustomerFiscalDocumentsTitle.Location = new Point(13, 10);
        grpCustomerFiscalDocumentsTitle.Name = "grpCustomerFiscalDocumentsTitle";
        grpCustomerFiscalDocumentsTitle.Text = "Documentos fiscales";
        grpCustomerFiscalDocuments.Controls.Add(grpCustomerFiscalDocumentsTitle);
        grpCustomerFiscalDocumentsTitle.BringToFront();
        //
        // lblCustomerEmissionType
        //
        lblCustomerEmissionType.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerEmissionType.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerEmissionType.Appearance.Options.UseFont = true;
        lblCustomerEmissionType.Appearance.Options.UseForeColor = true;
        lblCustomerEmissionType.Location = new Point(18, 36);
        lblCustomerEmissionType.Name = "lblCustomerEmissionType";
        lblCustomerEmissionType.Size = new Size(69, 15);
        lblCustomerEmissionType.TabIndex = 0;
        lblCustomerEmissionType.Text = "Tipo emision";
        //
        // lblCustomerDefaultSeries
        //
        lblCustomerDefaultSeries.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerDefaultSeries.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerDefaultSeries.Appearance.Options.UseFont = true;
        lblCustomerDefaultSeries.Appearance.Options.UseForeColor = true;
        lblCustomerDefaultSeries.Location = new Point(18, 66);
        lblCustomerDefaultSeries.Name = "lblCustomerDefaultSeries";
        lblCustomerDefaultSeries.Size = new Size(89, 15);
        lblCustomerDefaultSeries.TabIndex = 15;
        lblCustomerDefaultSeries.Text = "Serie por defecto";
        //
        // lblCustomerInitialNumber
        //
        lblCustomerInitialNumber.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerInitialNumber.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerInitialNumber.Appearance.Options.UseFont = true;
        lblCustomerInitialNumber.Appearance.Options.UseForeColor = true;
        lblCustomerInitialNumber.Location = new Point(18, 96);
        lblCustomerInitialNumber.Name = "lblCustomerInitialNumber";
        lblCustomerInitialNumber.Size = new Size(78, 15);
        lblCustomerInitialNumber.TabIndex = 16;
        lblCustomerInitialNumber.Text = "Numero inicial";
        //
        // lblCustomerPrintFormat
        //
        lblCustomerPrintFormat.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerPrintFormat.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerPrintFormat.Appearance.Options.UseFont = true;
        lblCustomerPrintFormat.Appearance.Options.UseForeColor = true;
        lblCustomerPrintFormat.Location = new Point(18, 126);
        lblCustomerPrintFormat.Name = "lblCustomerPrintFormat";
        lblCustomerPrintFormat.Size = new Size(101, 15);
        lblCustomerPrintFormat.TabIndex = 17;
        lblCustomerPrintFormat.Text = "Formato impresion";
        //
        // grpCustomerFiscalNotes
        //
        grpCustomerFiscalNotes.Controls.Add(memFiscalNotes);
        grpCustomerFiscalNotes.Location = new Point(796, 188);
        grpCustomerFiscalNotes.Name = "grpCustomerFiscalNotes";
        grpCustomerFiscalNotes.Size = new Size(434, 162);
        grpCustomerFiscalNotes.TabIndex = 10;
        var grpCustomerFiscalNotesTitle = new LabelControl();
        grpCustomerFiscalNotesTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerFiscalNotesTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerFiscalNotesTitle.Appearance.Options.UseFont = true;
        grpCustomerFiscalNotesTitle.Appearance.Options.UseForeColor = true;
        grpCustomerFiscalNotesTitle.Location = new Point(13, 10);
        grpCustomerFiscalNotesTitle.Name = "grpCustomerFiscalNotesTitle";
        grpCustomerFiscalNotesTitle.Text = "Notas fiscales";
        grpCustomerFiscalNotes.Controls.Add(grpCustomerFiscalNotesTitle);
        grpCustomerFiscalNotesTitle.BringToFront();
        //
        // xtpAddresses
        //
        xtpAddresses.Controls.Add(lblAddressButtons);
        xtpAddresses.Controls.Add(grpCustomerAddressList);
        xtpAddresses.Controls.Add(grpCustomerAddressDetail);
        xtpAddresses.Name = "xtpAddresses";
        xtpAddresses.Size = new Size(1258, 364);
        xtpAddresses.Text = "Direcciones";
        //
        // grdCustomerAddresses
        //
        grdCustomerAddresses.Font = new Font("Segoe UI", 9F);
        grdCustomerAddresses.Location = new Point(14, 30);
        grdCustomerAddresses.MainView = grvCustomerAddresses;
        grdCustomerAddresses.Name = "grdCustomerAddresses";
        grdCustomerAddresses.Size = new Size(1188, 128);
        grdCustomerAddresses.TabIndex = 0;
        grdCustomerAddresses.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvCustomerAddresses });
        //
        // grvCustomerAddresses
        //
        grvCustomerAddresses.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        grvCustomerAddresses.Appearance.FilterPanel.Options.UseFont = true;
        grvCustomerAddresses.Appearance.FooterPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grvCustomerAddresses.Appearance.FooterPanel.Options.UseFont = true;
        grvCustomerAddresses.Appearance.HeaderPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grvCustomerAddresses.Appearance.HeaderPanel.ForeColor = Color.FromArgb(23, 32, 51);
        grvCustomerAddresses.Appearance.HeaderPanel.Options.UseFont = true;
        grvCustomerAddresses.Appearance.HeaderPanel.Options.UseForeColor = true;
        grvCustomerAddresses.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvCustomerAddresses.Appearance.Row.ForeColor = Color.FromArgb(23, 32, 51);
        grvCustomerAddresses.Appearance.Row.Options.UseFont = true;
        grvCustomerAddresses.Appearance.Row.Options.UseForeColor = true;
        grvCustomerAddresses.GridControl = grdCustomerAddresses;
        grvCustomerAddresses.Name = "grvCustomerAddresses";
        grvCustomerAddresses.OptionsBehavior.Editable = false;
        grvCustomerAddresses.OptionsView.ShowGroupPanel = false;
        //
        // lueAddressType
        //
        lueAddressType.Location = new Point(105, 32);
        lueAddressType.Name = "lueAddressType";
        lueAddressType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAddressType.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueAddressType.Properties.Appearance.Options.UseFont = true;
        lueAddressType.Properties.Appearance.Options.UseForeColor = true;
        lueAddressType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAddressType.Size = new Size(180, 22);
        lueAddressType.TabIndex = 1;
        //
        // memAddress
        //
        memAddress.Location = new Point(105, 62);
        memAddress.Name = "memAddress";
        memAddress.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memAddress.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        memAddress.Properties.Appearance.Options.UseFont = true;
        memAddress.Properties.Appearance.Options.UseForeColor = true;
        memAddress.Size = new Size(420, 70);
        memAddress.TabIndex = 2;
        //
        // lueAddressCountry
        //
        lueAddressCountry.Location = new Point(580, 32);
        lueAddressCountry.Name = "lueAddressCountry";
        lueAddressCountry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAddressCountry.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueAddressCountry.Properties.Appearance.Options.UseFont = true;
        lueAddressCountry.Properties.Appearance.Options.UseForeColor = true;
        lueAddressCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAddressCountry.Size = new Size(150, 22);
        lueAddressCountry.TabIndex = 3;
        //
        // lueAddressProvince
        //
        lueAddressProvince.Location = new Point(580, 62);
        lueAddressProvince.Name = "lueAddressProvince";
        lueAddressProvince.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAddressProvince.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueAddressProvince.Properties.Appearance.Options.UseFont = true;
        lueAddressProvince.Properties.Appearance.Options.UseForeColor = true;
        lueAddressProvince.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAddressProvince.Size = new Size(150, 22);
        lueAddressProvince.TabIndex = 4;
        //
        // lueAddressCity
        //
        lueAddressCity.Location = new Point(580, 92);
        lueAddressCity.Name = "lueAddressCity";
        lueAddressCity.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAddressCity.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueAddressCity.Properties.Appearance.Options.UseFont = true;
        lueAddressCity.Properties.Appearance.Options.UseForeColor = true;
        lueAddressCity.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAddressCity.Size = new Size(150, 22);
        lueAddressCity.TabIndex = 5;
        //
        // txtPostalCode
        //
        txtPostalCode.Location = new Point(880, 32);
        txtPostalCode.Name = "txtPostalCode";
        txtPostalCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtPostalCode.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtPostalCode.Properties.Appearance.Options.UseFont = true;
        txtPostalCode.Properties.Appearance.Options.UseForeColor = true;
        txtPostalCode.Size = new Size(140, 22);
        txtPostalCode.TabIndex = 6;
        //
        // txtAddressReference
        //
        txtAddressReference.Location = new Point(880, 62);
        txtAddressReference.Name = "txtAddressReference";
        txtAddressReference.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtAddressReference.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtAddressReference.Properties.Appearance.Options.UseFont = true;
        txtAddressReference.Properties.Appearance.Options.UseForeColor = true;
        txtAddressReference.Size = new Size(300, 22);
        txtAddressReference.TabIndex = 7;
        //
        // tsPrimaryAddress
        //
        tsPrimaryAddress.Location = new Point(880, 94);
        tsPrimaryAddress.Name = "tsPrimaryAddress";
        tsPrimaryAddress.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsPrimaryAddress.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsPrimaryAddress.Properties.Appearance.Options.UseFont = true;
        tsPrimaryAddress.Properties.Appearance.Options.UseForeColor = true;
        tsPrimaryAddress.Properties.OffText = "";
        tsPrimaryAddress.Properties.OnText = "";
        tsPrimaryAddress.Size = new Size(95, 20);
        tsPrimaryAddress.TabIndex = 8;
        //
        // lblAddressButtons
        //
        lblAddressButtons.Appearance.Font = new Font("Segoe UI", 9F);
        lblAddressButtons.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblAddressButtons.Appearance.Options.UseFont = true;
        lblAddressButtons.Appearance.Options.UseForeColor = true;
        lblAddressButtons.Location = new Point(24, 325);
        lblAddressButtons.Name = "lblAddressButtons";
        lblAddressButtons.Size = new Size(51, 15);
        lblAddressButtons.TabIndex = 9;
        lblAddressButtons.Text = "Acciones:";
        //
        // btnAddAddress
        //
        btnAddAddress.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnAddAddress.Appearance.Options.UseFont = true;
        btnAddAddress.Location = new Point(100, 118);
        btnAddAddress.Name = "btnAddAddress";
        btnAddAddress.Size = new Size(90, 30);
        btnAddAddress.TabIndex = 10;
        btnAddAddress.Text = "Agregar";
        //
        // btnEditAddress
        //
        btnEditAddress.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnEditAddress.Appearance.Options.UseFont = true;
        btnEditAddress.Location = new Point(202, 118);
        btnEditAddress.Name = "btnEditAddress";
        btnEditAddress.Size = new Size(90, 30);
        btnEditAddress.TabIndex = 11;
        btnEditAddress.Text = "Editar";
        //
        // btnDeleteAddress
        //
        btnDeleteAddress.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnDeleteAddress.Appearance.Options.UseFont = true;
        btnDeleteAddress.Location = new Point(304, 118);
        btnDeleteAddress.Name = "btnDeleteAddress";
        btnDeleteAddress.Size = new Size(90, 30);
        btnDeleteAddress.TabIndex = 12;
        btnDeleteAddress.Text = "Eliminar";
        //
        // btnSetPrimaryAddress
        //
        btnSetPrimaryAddress.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSetPrimaryAddress.Appearance.Options.UseFont = true;
        btnSetPrimaryAddress.Location = new Point(406, 118);
        btnSetPrimaryAddress.Name = "btnSetPrimaryAddress";
        btnSetPrimaryAddress.Size = new Size(150, 30);
        btnSetPrimaryAddress.TabIndex = 13;
        btnSetPrimaryAddress.Text = "Establecer principal";
        //
        // grpCustomerAddressList
        //
        grpCustomerAddressList.Controls.Add(grdCustomerAddresses);
        grpCustomerAddressList.Location = new Point(14, 14);
        grpCustomerAddressList.Name = "grpCustomerAddressList";
        grpCustomerAddressList.Size = new Size(1216, 174);
        grpCustomerAddressList.TabIndex = 10;
        var grpCustomerAddressListTitle = new LabelControl();
        grpCustomerAddressListTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerAddressListTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerAddressListTitle.Appearance.Options.UseFont = true;
        grpCustomerAddressListTitle.Appearance.Options.UseForeColor = true;
        grpCustomerAddressListTitle.Location = new Point(13, 10);
        grpCustomerAddressListTitle.Name = "grpCustomerAddressListTitle";
        grpCustomerAddressListTitle.Text = "Lista de direcciones";
        grpCustomerAddressList.Controls.Add(grpCustomerAddressListTitle);
        grpCustomerAddressListTitle.BringToFront();
        //
        // grpCustomerAddressDetail
        //
        grpCustomerAddressDetail.Controls.Add(lblCustomerAddressType);
        grpCustomerAddressDetail.Controls.Add(lueAddressType);
        grpCustomerAddressDetail.Controls.Add(lblCustomerAddress);
        grpCustomerAddressDetail.Controls.Add(memAddress);
        grpCustomerAddressDetail.Controls.Add(lblCustomerAddressCountry);
        grpCustomerAddressDetail.Controls.Add(lueAddressCountry);
        grpCustomerAddressDetail.Controls.Add(lblCustomerAddressProvince);
        grpCustomerAddressDetail.Controls.Add(lueAddressProvince);
        grpCustomerAddressDetail.Controls.Add(lblCustomerAddressCity);
        grpCustomerAddressDetail.Controls.Add(lueAddressCity);
        grpCustomerAddressDetail.Controls.Add(lblCustomerPostal);
        grpCustomerAddressDetail.Controls.Add(txtPostalCode);
        grpCustomerAddressDetail.Controls.Add(lblCustomerReference);
        grpCustomerAddressDetail.Controls.Add(txtAddressReference);
        grpCustomerAddressDetail.Controls.Add(lblCustomerPrimaryAddress);
        grpCustomerAddressDetail.Controls.Add(tsPrimaryAddress);
        grpCustomerAddressDetail.Controls.Add(btnAddAddress);
        grpCustomerAddressDetail.Controls.Add(btnEditAddress);
        grpCustomerAddressDetail.Controls.Add(btnDeleteAddress);
        grpCustomerAddressDetail.Controls.Add(btnSetPrimaryAddress);
        grpCustomerAddressDetail.Location = new Point(14, 198);
        grpCustomerAddressDetail.Name = "grpCustomerAddressDetail";
        grpCustomerAddressDetail.Size = new Size(1216, 152);
        grpCustomerAddressDetail.TabIndex = 11;
        var grpCustomerAddressDetailTitle = new LabelControl();
        grpCustomerAddressDetailTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerAddressDetailTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerAddressDetailTitle.Appearance.Options.UseFont = true;
        grpCustomerAddressDetailTitle.Appearance.Options.UseForeColor = true;
        grpCustomerAddressDetailTitle.Location = new Point(13, 10);
        grpCustomerAddressDetailTitle.Name = "grpCustomerAddressDetailTitle";
        grpCustomerAddressDetailTitle.Text = "Detalle de direccion seleccionada";
        grpCustomerAddressDetail.Controls.Add(grpCustomerAddressDetailTitle);
        grpCustomerAddressDetailTitle.BringToFront();
        //
        // lblCustomerAddressType
        //
        lblCustomerAddressType.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerAddressType.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerAddressType.Appearance.Options.UseFont = true;
        lblCustomerAddressType.Appearance.Options.UseForeColor = true;
        lblCustomerAddressType.Location = new Point(18, 36);
        lblCustomerAddressType.Name = "lblCustomerAddressType";
        lblCustomerAddressType.Size = new Size(24, 15);
        lblCustomerAddressType.TabIndex = 0;
        lblCustomerAddressType.Text = "Tipo";
        //
        // lblCustomerAddress
        //
        lblCustomerAddress.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerAddress.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerAddress.Appearance.Options.UseFont = true;
        lblCustomerAddress.Appearance.Options.UseForeColor = true;
        lblCustomerAddress.Location = new Point(18, 66);
        lblCustomerAddress.Name = "lblCustomerAddress";
        lblCustomerAddress.Size = new Size(50, 15);
        lblCustomerAddress.TabIndex = 2;
        lblCustomerAddress.Text = "Direccion";
        //
        // lblCustomerAddressCountry
        //
        lblCustomerAddressCountry.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerAddressCountry.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerAddressCountry.Appearance.Options.UseFont = true;
        lblCustomerAddressCountry.Appearance.Options.UseForeColor = true;
        lblCustomerAddressCountry.Location = new Point(500, 36);
        lblCustomerAddressCountry.Name = "lblCustomerAddressCountry";
        lblCustomerAddressCountry.Size = new Size(21, 15);
        lblCustomerAddressCountry.TabIndex = 3;
        lblCustomerAddressCountry.Text = "Pais";
        //
        // lblCustomerAddressProvince
        //
        lblCustomerAddressProvince.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerAddressProvince.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerAddressProvince.Appearance.Options.UseFont = true;
        lblCustomerAddressProvince.Appearance.Options.UseForeColor = true;
        lblCustomerAddressProvince.Location = new Point(500, 66);
        lblCustomerAddressProvince.Name = "lblCustomerAddressProvince";
        lblCustomerAddressProvince.Size = new Size(49, 15);
        lblCustomerAddressProvince.TabIndex = 4;
        lblCustomerAddressProvince.Text = "Provincia";
        //
        // lblCustomerAddressCity
        //
        lblCustomerAddressCity.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerAddressCity.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerAddressCity.Appearance.Options.UseFont = true;
        lblCustomerAddressCity.Appearance.Options.UseForeColor = true;
        lblCustomerAddressCity.Location = new Point(500, 96);
        lblCustomerAddressCity.Name = "lblCustomerAddressCity";
        lblCustomerAddressCity.Size = new Size(38, 15);
        lblCustomerAddressCity.TabIndex = 5;
        lblCustomerAddressCity.Text = "Ciudad";
        //
        // lblCustomerPostal
        //
        lblCustomerPostal.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerPostal.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerPostal.Appearance.Options.UseFont = true;
        lblCustomerPostal.Appearance.Options.UseForeColor = true;
        lblCustomerPostal.Location = new Point(770, 36);
        lblCustomerPostal.Name = "lblCustomerPostal";
        lblCustomerPostal.Size = new Size(74, 15);
        lblCustomerPostal.TabIndex = 6;
        lblCustomerPostal.Text = "Codigo postal";
        //
        // lblCustomerReference
        //
        lblCustomerReference.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerReference.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerReference.Appearance.Options.UseFont = true;
        lblCustomerReference.Appearance.Options.UseForeColor = true;
        lblCustomerReference.Location = new Point(770, 66);
        lblCustomerReference.Name = "lblCustomerReference";
        lblCustomerReference.Size = new Size(55, 15);
        lblCustomerReference.TabIndex = 7;
        lblCustomerReference.Text = "Referencia";
        //
        // lblCustomerPrimaryAddress
        //
        lblCustomerPrimaryAddress.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerPrimaryAddress.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerPrimaryAddress.Appearance.Options.UseFont = true;
        lblCustomerPrimaryAddress.Appearance.Options.UseForeColor = true;
        lblCustomerPrimaryAddress.Location = new Point(770, 100);
        lblCustomerPrimaryAddress.Name = "lblCustomerPrimaryAddress";
        lblCustomerPrimaryAddress.Size = new Size(46, 15);
        lblCustomerPrimaryAddress.TabIndex = 8;
        lblCustomerPrimaryAddress.Text = "Principal";
        //
        // xtpContacts
        //
        xtpContacts.Controls.Add(lblContactButtons);
        xtpContacts.Controls.Add(grpCustomerContactList);
        xtpContacts.Controls.Add(grpCustomerContactDetail);
        xtpContacts.Name = "xtpContacts";
        xtpContacts.Size = new Size(1258, 364);
        xtpContacts.Text = "Contactos";
        //
        // grdCustomerContactList
        //
        grdCustomerContactList.Font = new Font("Segoe UI", 9F);
        grdCustomerContactList.Location = new Point(14, 30);
        grdCustomerContactList.MainView = grvCustomerContactList;
        grdCustomerContactList.Name = "grdCustomerContactList";
        grdCustomerContactList.Size = new Size(735, 250);
        grdCustomerContactList.TabIndex = 0;
        grdCustomerContactList.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvCustomerContactList });
        //
        // grvCustomerContactList
        //
        grvCustomerContactList.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        grvCustomerContactList.Appearance.FilterPanel.Options.UseFont = true;
        grvCustomerContactList.Appearance.FooterPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grvCustomerContactList.Appearance.FooterPanel.Options.UseFont = true;
        grvCustomerContactList.Appearance.HeaderPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grvCustomerContactList.Appearance.HeaderPanel.ForeColor = Color.FromArgb(23, 32, 51);
        grvCustomerContactList.Appearance.HeaderPanel.Options.UseFont = true;
        grvCustomerContactList.Appearance.HeaderPanel.Options.UseForeColor = true;
        grvCustomerContactList.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvCustomerContactList.Appearance.Row.ForeColor = Color.FromArgb(23, 32, 51);
        grvCustomerContactList.Appearance.Row.Options.UseFont = true;
        grvCustomerContactList.Appearance.Row.Options.UseForeColor = true;
        grvCustomerContactList.GridControl = grdCustomerContactList;
        grvCustomerContactList.Name = "grvCustomerContactList";
        grvCustomerContactList.OptionsBehavior.Editable = false;
        grvCustomerContactList.OptionsView.ShowGroupPanel = false;
        //
        // txtContactName
        //
        txtContactName.Location = new Point(150, 32);
        txtContactName.Name = "txtContactName";
        txtContactName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtContactName.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtContactName.Properties.Appearance.Options.UseFont = true;
        txtContactName.Properties.Appearance.Options.UseForeColor = true;
        txtContactName.Size = new Size(240, 22);
        txtContactName.TabIndex = 1;
        //
        // txtContactPosition
        //
        txtContactPosition.Location = new Point(150, 62);
        txtContactPosition.Name = "txtContactPosition";
        txtContactPosition.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtContactPosition.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtContactPosition.Properties.Appearance.Options.UseFont = true;
        txtContactPosition.Properties.Appearance.Options.UseForeColor = true;
        txtContactPosition.Size = new Size(240, 22);
        txtContactPosition.TabIndex = 2;
        //
        // txtContactPhone
        //
        txtContactPhone.Location = new Point(150, 92);
        txtContactPhone.Name = "txtContactPhone";
        txtContactPhone.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtContactPhone.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtContactPhone.Properties.Appearance.Options.UseFont = true;
        txtContactPhone.Properties.Appearance.Options.UseForeColor = true;
        txtContactPhone.Size = new Size(240, 22);
        txtContactPhone.TabIndex = 3;
        //
        // txtContactMobile
        //
        txtContactMobile.Location = new Point(150, 122);
        txtContactMobile.Name = "txtContactMobile";
        txtContactMobile.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtContactMobile.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtContactMobile.Properties.Appearance.Options.UseFont = true;
        txtContactMobile.Properties.Appearance.Options.UseForeColor = true;
        txtContactMobile.Size = new Size(240, 22);
        txtContactMobile.TabIndex = 4;
        //
        // txtContactEmail
        //
        txtContactEmail.Location = new Point(150, 152);
        txtContactEmail.Name = "txtContactEmail";
        txtContactEmail.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtContactEmail.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtContactEmail.Properties.Appearance.Options.UseFont = true;
        txtContactEmail.Properties.Appearance.Options.UseForeColor = true;
        txtContactEmail.Size = new Size(240, 22);
        txtContactEmail.TabIndex = 5;
        //
        // tsPrimaryContact
        //
        tsPrimaryContact.Location = new Point(150, 184);
        tsPrimaryContact.Name = "tsPrimaryContact";
        tsPrimaryContact.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsPrimaryContact.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsPrimaryContact.Properties.Appearance.Options.UseFont = true;
        tsPrimaryContact.Properties.Appearance.Options.UseForeColor = true;
        tsPrimaryContact.Properties.OffText = "";
        tsPrimaryContact.Properties.OnText = "";
        tsPrimaryContact.Size = new Size(95, 20);
        tsPrimaryContact.TabIndex = 6;
        //
        // tsActiveContact
        //
        tsActiveContact.Location = new Point(150, 214);
        tsActiveContact.Name = "tsActiveContact";
        tsActiveContact.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        tsActiveContact.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        tsActiveContact.Properties.Appearance.Options.UseFont = true;
        tsActiveContact.Properties.Appearance.Options.UseForeColor = true;
        tsActiveContact.Properties.OffText = "";
        tsActiveContact.Properties.OnText = "";
        tsActiveContact.Size = new Size(95, 20);
        tsActiveContact.TabIndex = 7;
        //
        // memContactNotes
        //
        memContactNotes.Location = new Point(150, 246);
        memContactNotes.Name = "memContactNotes";
        memContactNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memContactNotes.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        memContactNotes.Properties.Appearance.Options.UseFont = true;
        memContactNotes.Properties.Appearance.Options.UseForeColor = true;
        memContactNotes.Size = new Size(360, 70);
        memContactNotes.TabIndex = 8;
        //
        // lblContactButtons
        //
        lblContactButtons.Appearance.Font = new Font("Segoe UI", 9F);
        lblContactButtons.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblContactButtons.Appearance.Options.UseFont = true;
        lblContactButtons.Appearance.Options.UseForeColor = true;
        lblContactButtons.Location = new Point(24, 325);
        lblContactButtons.Name = "lblContactButtons";
        lblContactButtons.Size = new Size(51, 15);
        lblContactButtons.TabIndex = 9;
        lblContactButtons.Text = "Acciones:";
        //
        // btnAddContact
        //
        btnAddContact.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnAddContact.Appearance.Options.UseFont = true;
        btnAddContact.Location = new Point(14, 292);
        btnAddContact.Name = "btnAddContact";
        btnAddContact.Size = new Size(90, 30);
        btnAddContact.TabIndex = 10;
        btnAddContact.Text = "Agregar";
        //
        // btnEditContact
        //
        btnEditContact.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnEditContact.Appearance.Options.UseFont = true;
        btnEditContact.Location = new Point(116, 292);
        btnEditContact.Name = "btnEditContact";
        btnEditContact.Size = new Size(90, 30);
        btnEditContact.TabIndex = 11;
        btnEditContact.Text = "Editar";
        //
        // btnDeleteContact
        //
        btnDeleteContact.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnDeleteContact.Appearance.Options.UseFont = true;
        btnDeleteContact.Location = new Point(218, 292);
        btnDeleteContact.Name = "btnDeleteContact";
        btnDeleteContact.Size = new Size(90, 30);
        btnDeleteContact.TabIndex = 12;
        btnDeleteContact.Text = "Eliminar";
        //
        // grpCustomerContactList
        //
        grpCustomerContactList.Controls.Add(grdCustomerContactList);
        grpCustomerContactList.Controls.Add(btnAddContact);
        grpCustomerContactList.Controls.Add(btnEditContact);
        grpCustomerContactList.Controls.Add(btnDeleteContact);
        grpCustomerContactList.Location = new Point(14, 14);
        grpCustomerContactList.Name = "grpCustomerContactList";
        grpCustomerContactList.Size = new Size(765, 336);
        grpCustomerContactList.TabIndex = 10;
        var grpCustomerContactListTitle = new LabelControl();
        grpCustomerContactListTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerContactListTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerContactListTitle.Appearance.Options.UseFont = true;
        grpCustomerContactListTitle.Appearance.Options.UseForeColor = true;
        grpCustomerContactListTitle.Location = new Point(13, 10);
        grpCustomerContactListTitle.Name = "grpCustomerContactListTitle";
        grpCustomerContactListTitle.Text = "Lista de contactos";
        grpCustomerContactList.Controls.Add(grpCustomerContactListTitle);
        grpCustomerContactListTitle.BringToFront();
        //
        // grpCustomerContactDetail
        //
        grpCustomerContactDetail.Controls.Add(lblCustomerContactName);
        grpCustomerContactDetail.Controls.Add(txtContactName);
        grpCustomerContactDetail.Controls.Add(lblCustomerContactPosition);
        grpCustomerContactDetail.Controls.Add(txtContactPosition);
        grpCustomerContactDetail.Controls.Add(lblCustomerContactPhone);
        grpCustomerContactDetail.Controls.Add(txtContactPhone);
        grpCustomerContactDetail.Controls.Add(lblCustomerContactMobile);
        grpCustomerContactDetail.Controls.Add(txtContactMobile);
        grpCustomerContactDetail.Controls.Add(lblCustomerContactEmail);
        grpCustomerContactDetail.Controls.Add(txtContactEmail);
        grpCustomerContactDetail.Controls.Add(lblCustomerPrimaryContact);
        grpCustomerContactDetail.Controls.Add(tsPrimaryContact);
        grpCustomerContactDetail.Controls.Add(lblCustomerActiveContact);
        grpCustomerContactDetail.Controls.Add(tsActiveContact);
        grpCustomerContactDetail.Controls.Add(memContactNotes);
        grpCustomerContactDetail.Location = new Point(795, 14);
        grpCustomerContactDetail.Name = "grpCustomerContactDetail";
        grpCustomerContactDetail.Size = new Size(435, 336);
        grpCustomerContactDetail.TabIndex = 11;
        var grpCustomerContactDetailTitle = new LabelControl();
        grpCustomerContactDetailTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerContactDetailTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerContactDetailTitle.Appearance.Options.UseFont = true;
        grpCustomerContactDetailTitle.Appearance.Options.UseForeColor = true;
        grpCustomerContactDetailTitle.Location = new Point(13, 10);
        grpCustomerContactDetailTitle.Name = "grpCustomerContactDetailTitle";
        grpCustomerContactDetailTitle.Text = "Detalle del contacto seleccionado";
        grpCustomerContactDetail.Controls.Add(grpCustomerContactDetailTitle);
        grpCustomerContactDetailTitle.BringToFront();
        //
        // lblCustomerContactName
        //
        lblCustomerContactName.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerContactName.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerContactName.Appearance.Options.UseFont = true;
        lblCustomerContactName.Appearance.Options.UseForeColor = true;
        lblCustomerContactName.Location = new Point(18, 36);
        lblCustomerContactName.Name = "lblCustomerContactName";
        lblCustomerContactName.Size = new Size(98, 15);
        lblCustomerContactName.TabIndex = 0;
        lblCustomerContactName.Text = "Nombre completo";
        //
        // lblCustomerContactPosition
        //
        lblCustomerContactPosition.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerContactPosition.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerContactPosition.Appearance.Options.UseFont = true;
        lblCustomerContactPosition.Appearance.Options.UseForeColor = true;
        lblCustomerContactPosition.Location = new Point(18, 66);
        lblCustomerContactPosition.Name = "lblCustomerContactPosition";
        lblCustomerContactPosition.Size = new Size(32, 15);
        lblCustomerContactPosition.TabIndex = 2;
        lblCustomerContactPosition.Text = "Cargo";
        //
        // lblCustomerContactPhone
        //
        lblCustomerContactPhone.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerContactPhone.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerContactPhone.Appearance.Options.UseFont = true;
        lblCustomerContactPhone.Appearance.Options.UseForeColor = true;
        lblCustomerContactPhone.Location = new Point(18, 96);
        lblCustomerContactPhone.Name = "lblCustomerContactPhone";
        lblCustomerContactPhone.Size = new Size(47, 15);
        lblCustomerContactPhone.TabIndex = 3;
        lblCustomerContactPhone.Text = "Telefono";
        //
        // lblCustomerContactMobile
        //
        lblCustomerContactMobile.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerContactMobile.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerContactMobile.Appearance.Options.UseFont = true;
        lblCustomerContactMobile.Appearance.Options.UseForeColor = true;
        lblCustomerContactMobile.Location = new Point(18, 126);
        lblCustomerContactMobile.Name = "lblCustomerContactMobile";
        lblCustomerContactMobile.Size = new Size(30, 15);
        lblCustomerContactMobile.TabIndex = 4;
        lblCustomerContactMobile.Text = "Movil";
        //
        // lblCustomerContactEmail
        //
        lblCustomerContactEmail.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerContactEmail.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerContactEmail.Appearance.Options.UseFont = true;
        lblCustomerContactEmail.Appearance.Options.UseForeColor = true;
        lblCustomerContactEmail.Location = new Point(18, 156);
        lblCustomerContactEmail.Name = "lblCustomerContactEmail";
        lblCustomerContactEmail.Size = new Size(36, 15);
        lblCustomerContactEmail.TabIndex = 5;
        lblCustomerContactEmail.Text = "Correo";
        //
        // lblCustomerPrimaryContact
        //
        lblCustomerPrimaryContact.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerPrimaryContact.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerPrimaryContact.Appearance.Options.UseFont = true;
        lblCustomerPrimaryContact.Appearance.Options.UseForeColor = true;
        lblCustomerPrimaryContact.Location = new Point(18, 190);
        lblCustomerPrimaryContact.Name = "lblCustomerPrimaryContact";
        lblCustomerPrimaryContact.Size = new Size(46, 15);
        lblCustomerPrimaryContact.TabIndex = 6;
        lblCustomerPrimaryContact.Text = "Principal";
        //
        // lblCustomerActiveContact
        //
        lblCustomerActiveContact.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerActiveContact.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerActiveContact.Appearance.Options.UseFont = true;
        lblCustomerActiveContact.Appearance.Options.UseForeColor = true;
        lblCustomerActiveContact.Location = new Point(18, 220);
        lblCustomerActiveContact.Name = "lblCustomerActiveContact";
        lblCustomerActiveContact.Size = new Size(34, 15);
        lblCustomerActiveContact.TabIndex = 7;
        lblCustomerActiveContact.Text = "Activo";
        //
        // xtpCommercial
        //
        xtpCommercial.Controls.Add(grpCustomerCommercialConditions);
        xtpCommercial.Controls.Add(grpCustomerCredit);
        xtpCommercial.Controls.Add(grpCustomerCommercialSummary);
        xtpCommercial.Controls.Add(grpCustomerCommercialNotes);
        xtpCommercial.Name = "xtpCommercial";
        xtpCommercial.Size = new Size(1258, 364);
        xtpCommercial.Text = "Comercial";
        //
        // spnOverdueDays
        //
        spnOverdueDays.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnOverdueDays.Location = new Point(160, 100);
        spnOverdueDays.Name = "spnOverdueDays";
        spnOverdueDays.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnOverdueDays.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        spnOverdueDays.Properties.Appearance.Options.UseFont = true;
        spnOverdueDays.Properties.Appearance.Options.UseForeColor = true;
        spnOverdueDays.Properties.Appearance.Options.UseTextOptions = true;
        spnOverdueDays.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnOverdueDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnOverdueDays.Size = new Size(100, 22);
        spnOverdueDays.TabIndex = 0;
        //
        // memCommercialNotes
        //
        memCommercialNotes.Location = new Point(16, 32);
        memCommercialNotes.Name = "memCommercialNotes";
        memCommercialNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memCommercialNotes.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        memCommercialNotes.Properties.Appearance.Options.UseFont = true;
        memCommercialNotes.Properties.Appearance.Options.UseForeColor = true;
        memCommercialNotes.Size = new Size(530, 150);
        memCommercialNotes.TabIndex = 1;
        //
        // grpCustomerCommercialConditions
        //
        grpCustomerCommercialConditions.Location = new Point(14, 14);
        grpCustomerCommercialConditions.Name = "grpCustomerCommercialConditions";
        grpCustomerCommercialConditions.Size = new Size(405, 210);
        grpCustomerCommercialConditions.TabIndex = 0;
        var grpCustomerCommercialConditionsTitle = new LabelControl();
        grpCustomerCommercialConditionsTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerCommercialConditionsTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerCommercialConditionsTitle.Appearance.Options.UseFont = true;
        grpCustomerCommercialConditionsTitle.Appearance.Options.UseForeColor = true;
        grpCustomerCommercialConditionsTitle.Location = new Point(13, 10);
        grpCustomerCommercialConditionsTitle.Name = "grpCustomerCommercialConditionsTitle";
        grpCustomerCommercialConditionsTitle.Text = "Condiciones comerciales";
        grpCustomerCommercialConditions.Controls.Add(grpCustomerCommercialConditionsTitle);
        grpCustomerCommercialConditionsTitle.BringToFront();
        //
        // grpCustomerCredit
        //
        grpCustomerCredit.Controls.Add(lblCustomerOverdue);
        grpCustomerCredit.Controls.Add(spnOverdueDays);
        grpCustomerCredit.Location = new Point(432, 14);
        grpCustomerCredit.Name = "grpCustomerCredit";
        grpCustomerCredit.Size = new Size(385, 210);
        grpCustomerCredit.TabIndex = 1;
        var grpCustomerCreditTitle = new LabelControl();
        grpCustomerCreditTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerCreditTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerCreditTitle.Appearance.Options.UseFont = true;
        grpCustomerCreditTitle.Appearance.Options.UseForeColor = true;
        grpCustomerCreditTitle.Location = new Point(13, 10);
        grpCustomerCreditTitle.Name = "grpCustomerCreditTitle";
        grpCustomerCreditTitle.Text = "Situacion crediticia";
        grpCustomerCredit.Controls.Add(grpCustomerCreditTitle);
        grpCustomerCreditTitle.BringToFront();
        //
        // lblCustomerOverdue
        //
        lblCustomerOverdue.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerOverdue.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerOverdue.Appearance.Options.UseFont = true;
        lblCustomerOverdue.Appearance.Options.UseForeColor = true;
        lblCustomerOverdue.Location = new Point(20, 104);
        lblCustomerOverdue.Name = "lblCustomerOverdue";
        lblCustomerOverdue.Size = new Size(72, 15);
        lblCustomerOverdue.TabIndex = 0;
        lblCustomerOverdue.Text = "Dias vencidos";
        //
        // grpCustomerCommercialSummary
        //
        grpCustomerCommercialSummary.Controls.Add(lblCustomerCommercialSummaryHint);
        grpCustomerCommercialSummary.Location = new Point(830, 14);
        grpCustomerCommercialSummary.Name = "grpCustomerCommercialSummary";
        grpCustomerCommercialSummary.Size = new Size(400, 210);
        grpCustomerCommercialSummary.TabIndex = 2;
        var grpCustomerCommercialSummaryTitle = new LabelControl();
        grpCustomerCommercialSummaryTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerCommercialSummaryTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerCommercialSummaryTitle.Appearance.Options.UseFont = true;
        grpCustomerCommercialSummaryTitle.Appearance.Options.UseForeColor = true;
        grpCustomerCommercialSummaryTitle.Location = new Point(13, 10);
        grpCustomerCommercialSummaryTitle.Name = "grpCustomerCommercialSummaryTitle";
        grpCustomerCommercialSummaryTitle.Text = "Resumen comercial";
        grpCustomerCommercialSummary.Controls.Add(grpCustomerCommercialSummaryTitle);
        grpCustomerCommercialSummaryTitle.BringToFront();
        //
        // lblCustomerCommercialSummaryHint
        //
        lblCustomerCommercialSummaryHint.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerCommercialSummaryHint.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerCommercialSummaryHint.Appearance.Options.UseFont = true;
        lblCustomerCommercialSummaryHint.Appearance.Options.UseForeColor = true;
        lblCustomerCommercialSummaryHint.Location = new Point(24, 44);
        lblCustomerCommercialSummaryHint.Name = "lblCustomerCommercialSummaryHint";
        lblCustomerCommercialSummaryHint.Size = new Size(243, 15);
        lblCustomerCommercialSummaryHint.TabIndex = 0;
        lblCustomerCommercialSummaryHint.Text = "Ventas YTD, ultima compra y pedidos abiertos";
        //
        // grpCustomerCommercialNotes
        //
        grpCustomerCommercialNotes.Controls.Add(memCommercialNotes);
        grpCustomerCommercialNotes.Controls.Add(memCommercialTerms);
        grpCustomerCommercialNotes.Location = new Point(14, 236);
        grpCustomerCommercialNotes.Name = "grpCustomerCommercialNotes";
        grpCustomerCommercialNotes.Size = new Size(1216, 114);
        grpCustomerCommercialNotes.TabIndex = 3;
        var grpCustomerCommercialNotesTitle = new LabelControl();
        grpCustomerCommercialNotesTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerCommercialNotesTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerCommercialNotesTitle.Appearance.Options.UseFont = true;
        grpCustomerCommercialNotesTitle.Appearance.Options.UseForeColor = true;
        grpCustomerCommercialNotesTitle.Location = new Point(13, 10);
        grpCustomerCommercialNotesTitle.Name = "grpCustomerCommercialNotesTitle";
        grpCustomerCommercialNotesTitle.Text = "Notas y terminos comerciales";
        grpCustomerCommercialNotes.Controls.Add(grpCustomerCommercialNotesTitle);
        grpCustomerCommercialNotesTitle.BringToFront();
        //
        // xtpAccounting
        //
        xtpAccounting.Controls.Add(grpCustomerAccounts);
        xtpAccounting.Controls.Add(grpCustomerAssignments);
        xtpAccounting.Controls.Add(grpCustomerWithholdings);
        xtpAccounting.Controls.Add(grpCustomerCurrency);
        xtpAccounting.Name = "xtpAccounting";
        xtpAccounting.Size = new Size(1258, 364);
        xtpAccounting.Text = "Contable";
        //
        // sluReceivableAccount
        //
        sluReceivableAccount.Location = new Point(170, 32);
        sluReceivableAccount.Name = "sluReceivableAccount";
        sluReceivableAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sluReceivableAccount.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        sluReceivableAccount.Properties.Appearance.Options.UseFont = true;
        sluReceivableAccount.Properties.Appearance.Options.UseForeColor = true;
        sluReceivableAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sluReceivableAccount.Properties.PopupView = sluReceivableAccountView;
        sluReceivableAccount.Size = new Size(320, 22);
        sluReceivableAccount.TabIndex = 0;
        //
        // sluReceivableAccountView
        //
        sluReceivableAccountView.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        sluReceivableAccountView.Appearance.FilterPanel.Options.UseFont = true;
        sluReceivableAccountView.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluReceivableAccountView.Appearance.FooterPanel.Options.UseFont = true;
        sluReceivableAccountView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluReceivableAccountView.Appearance.HeaderPanel.Options.UseFont = true;
        sluReceivableAccountView.Appearance.Row.Font = new Font("Segoe UI", 9F);
        sluReceivableAccountView.Appearance.Row.Options.UseFont = true;
        sluReceivableAccountView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        sluReceivableAccountView.Name = "sluReceivableAccountView";
        sluReceivableAccountView.OptionsSelection.EnableAppearanceFocusedCell = false;
        sluReceivableAccountView.OptionsView.ShowGroupPanel = false;
        //
        // sluCustomerAdvanceAccount
        //
        sluCustomerAdvanceAccount.Location = new Point(170, 62);
        sluCustomerAdvanceAccount.Name = "sluCustomerAdvanceAccount";
        sluCustomerAdvanceAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sluCustomerAdvanceAccount.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        sluCustomerAdvanceAccount.Properties.Appearance.Options.UseFont = true;
        sluCustomerAdvanceAccount.Properties.Appearance.Options.UseForeColor = true;
        sluCustomerAdvanceAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sluCustomerAdvanceAccount.Properties.PopupView = sluCustomerAdvanceAccountView;
        sluCustomerAdvanceAccount.Size = new Size(320, 22);
        sluCustomerAdvanceAccount.TabIndex = 1;
        //
        // sluCustomerAdvanceAccountView
        //
        sluCustomerAdvanceAccountView.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        sluCustomerAdvanceAccountView.Appearance.FilterPanel.Options.UseFont = true;
        sluCustomerAdvanceAccountView.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluCustomerAdvanceAccountView.Appearance.FooterPanel.Options.UseFont = true;
        sluCustomerAdvanceAccountView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluCustomerAdvanceAccountView.Appearance.HeaderPanel.Options.UseFont = true;
        sluCustomerAdvanceAccountView.Appearance.Row.Font = new Font("Segoe UI", 9F);
        sluCustomerAdvanceAccountView.Appearance.Row.Options.UseFont = true;
        sluCustomerAdvanceAccountView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        sluCustomerAdvanceAccountView.Name = "sluCustomerAdvanceAccountView";
        sluCustomerAdvanceAccountView.OptionsSelection.EnableAppearanceFocusedCell = false;
        sluCustomerAdvanceAccountView.OptionsView.ShowGroupPanel = false;
        //
        // sluDiscountAccount
        //
        sluDiscountAccount.Location = new Point(170, 92);
        sluDiscountAccount.Name = "sluDiscountAccount";
        sluDiscountAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sluDiscountAccount.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        sluDiscountAccount.Properties.Appearance.Options.UseFont = true;
        sluDiscountAccount.Properties.Appearance.Options.UseForeColor = true;
        sluDiscountAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sluDiscountAccount.Properties.PopupView = sluDiscountAccountView;
        sluDiscountAccount.Size = new Size(320, 22);
        sluDiscountAccount.TabIndex = 2;
        //
        // sluDiscountAccountView
        //
        sluDiscountAccountView.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        sluDiscountAccountView.Appearance.FilterPanel.Options.UseFont = true;
        sluDiscountAccountView.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluDiscountAccountView.Appearance.FooterPanel.Options.UseFont = true;
        sluDiscountAccountView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluDiscountAccountView.Appearance.HeaderPanel.Options.UseFont = true;
        sluDiscountAccountView.Appearance.Row.Font = new Font("Segoe UI", 9F);
        sluDiscountAccountView.Appearance.Row.Options.UseFont = true;
        sluDiscountAccountView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        sluDiscountAccountView.Name = "sluDiscountAccountView";
        sluDiscountAccountView.OptionsSelection.EnableAppearanceFocusedCell = false;
        sluDiscountAccountView.OptionsView.ShowGroupPanel = false;
        //
        // sluInterestAccount
        //
        sluInterestAccount.Location = new Point(170, 122);
        sluInterestAccount.Name = "sluInterestAccount";
        sluInterestAccount.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sluInterestAccount.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        sluInterestAccount.Properties.Appearance.Options.UseFont = true;
        sluInterestAccount.Properties.Appearance.Options.UseForeColor = true;
        sluInterestAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sluInterestAccount.Properties.PopupView = sluInterestAccountView;
        sluInterestAccount.Size = new Size(320, 22);
        sluInterestAccount.TabIndex = 3;
        //
        // sluInterestAccountView
        //
        sluInterestAccountView.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        sluInterestAccountView.Appearance.FilterPanel.Options.UseFont = true;
        sluInterestAccountView.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluInterestAccountView.Appearance.FooterPanel.Options.UseFont = true;
        sluInterestAccountView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluInterestAccountView.Appearance.HeaderPanel.Options.UseFont = true;
        sluInterestAccountView.Appearance.Row.Font = new Font("Segoe UI", 9F);
        sluInterestAccountView.Appearance.Row.Options.UseFont = true;
        sluInterestAccountView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        sluInterestAccountView.Name = "sluInterestAccountView";
        sluInterestAccountView.OptionsSelection.EnableAppearanceFocusedCell = false;
        sluInterestAccountView.OptionsView.ShowGroupPanel = false;
        //
        // lueCostCenter
        //
        lueCostCenter.Location = new Point(145, 32);
        lueCostCenter.Name = "lueCostCenter";
        lueCostCenter.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueCostCenter.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueCostCenter.Properties.Appearance.Options.UseFont = true;
        lueCostCenter.Properties.Appearance.Options.UseForeColor = true;
        lueCostCenter.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCostCenter.Size = new Size(240, 22);
        lueCostCenter.TabIndex = 4;
        //
        // lueProject
        //
        lueProject.Location = new Point(145, 62);
        lueProject.Name = "lueProject";
        lueProject.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueProject.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueProject.Properties.Appearance.Options.UseFont = true;
        lueProject.Properties.Appearance.Options.UseForeColor = true;
        lueProject.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueProject.Size = new Size(240, 22);
        lueProject.TabIndex = 5;
        //
        // sluIncomeWithholding
        //
        sluIncomeWithholding.Location = new Point(160, 32);
        sluIncomeWithholding.Name = "sluIncomeWithholding";
        sluIncomeWithholding.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sluIncomeWithholding.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        sluIncomeWithholding.Properties.Appearance.Options.UseFont = true;
        sluIncomeWithholding.Properties.Appearance.Options.UseForeColor = true;
        sluIncomeWithholding.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sluIncomeWithholding.Properties.PopupView = sluIncomeWithholdingView;
        sluIncomeWithholding.Size = new Size(260, 22);
        sluIncomeWithholding.TabIndex = 6;
        //
        // sluIncomeWithholdingView
        //
        sluIncomeWithholdingView.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        sluIncomeWithholdingView.Appearance.FilterPanel.Options.UseFont = true;
        sluIncomeWithholdingView.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluIncomeWithholdingView.Appearance.FooterPanel.Options.UseFont = true;
        sluIncomeWithholdingView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluIncomeWithholdingView.Appearance.HeaderPanel.Options.UseFont = true;
        sluIncomeWithholdingView.Appearance.Row.Font = new Font("Segoe UI", 9F);
        sluIncomeWithholdingView.Appearance.Row.Options.UseFont = true;
        sluIncomeWithholdingView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        sluIncomeWithholdingView.Name = "sluIncomeWithholdingView";
        sluIncomeWithholdingView.OptionsSelection.EnableAppearanceFocusedCell = false;
        sluIncomeWithholdingView.OptionsView.ShowGroupPanel = false;
        //
        // sluVatWithholding
        //
        sluVatWithholding.Location = new Point(160, 62);
        sluVatWithholding.Name = "sluVatWithholding";
        sluVatWithholding.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        sluVatWithholding.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        sluVatWithholding.Properties.Appearance.Options.UseFont = true;
        sluVatWithholding.Properties.Appearance.Options.UseForeColor = true;
        sluVatWithholding.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        sluVatWithholding.Properties.PopupView = sluVatWithholdingView;
        sluVatWithholding.Size = new Size(260, 22);
        sluVatWithholding.TabIndex = 7;
        //
        // sluVatWithholdingView
        //
        sluVatWithholdingView.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        sluVatWithholdingView.Appearance.FilterPanel.Options.UseFont = true;
        sluVatWithholdingView.Appearance.FooterPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluVatWithholdingView.Appearance.FooterPanel.Options.UseFont = true;
        sluVatWithholdingView.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        sluVatWithholdingView.Appearance.HeaderPanel.Options.UseFont = true;
        sluVatWithholdingView.Appearance.Row.Font = new Font("Segoe UI", 9F);
        sluVatWithholdingView.Appearance.Row.Options.UseFont = true;
        sluVatWithholdingView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        sluVatWithholdingView.Name = "sluVatWithholdingView";
        sluVatWithholdingView.OptionsSelection.EnableAppearanceFocusedCell = false;
        sluVatWithholdingView.OptionsView.ShowGroupPanel = false;
        //
        // lueIcaWithholding
        //
        lueIcaWithholding.Location = new Point(160, 92);
        lueIcaWithholding.Name = "lueIcaWithholding";
        lueIcaWithholding.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueIcaWithholding.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueIcaWithholding.Properties.Appearance.Options.UseFont = true;
        lueIcaWithholding.Properties.Appearance.Options.UseForeColor = true;
        lueIcaWithholding.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueIcaWithholding.Size = new Size(260, 22);
        lueIcaWithholding.TabIndex = 8;
        //
        // lueAccountingCurrency
        //
        lueAccountingCurrency.Location = new Point(150, 32);
        lueAccountingCurrency.Name = "lueAccountingCurrency";
        lueAccountingCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueAccountingCurrency.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueAccountingCurrency.Properties.Appearance.Options.UseFont = true;
        lueAccountingCurrency.Properties.Appearance.Options.UseForeColor = true;
        lueAccountingCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueAccountingCurrency.Size = new Size(240, 22);
        lueAccountingCurrency.TabIndex = 9;
        //
        // spnExchangeRate
        //
        spnExchangeRate.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnExchangeRate.Location = new Point(540, 32);
        spnExchangeRate.Name = "spnExchangeRate";
        spnExchangeRate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        spnExchangeRate.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        spnExchangeRate.Properties.Appearance.Options.UseFont = true;
        spnExchangeRate.Properties.Appearance.Options.UseForeColor = true;
        spnExchangeRate.Properties.Appearance.Options.UseTextOptions = true;
        spnExchangeRate.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        spnExchangeRate.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        spnExchangeRate.Size = new Size(240, 22);
        spnExchangeRate.TabIndex = 10;
        //
        // lueValidationStatus
        //
        lueValidationStatus.Location = new Point(875, 32);
        lueValidationStatus.Name = "lueValidationStatus";
        lueValidationStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueValidationStatus.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueValidationStatus.Properties.Appearance.Options.UseFont = true;
        lueValidationStatus.Properties.Appearance.Options.UseForeColor = true;
        lueValidationStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueValidationStatus.Size = new Size(260, 22);
        lueValidationStatus.TabIndex = 11;
        //
        // grpCustomerAccounts
        //
        grpCustomerAccounts.Controls.Add(lblCustomerReceivable);
        grpCustomerAccounts.Controls.Add(sluReceivableAccount);
        grpCustomerAccounts.Controls.Add(lblCustomerAdvance);
        grpCustomerAccounts.Controls.Add(sluCustomerAdvanceAccount);
        grpCustomerAccounts.Controls.Add(lblCustomerDiscount);
        grpCustomerAccounts.Controls.Add(sluDiscountAccount);
        grpCustomerAccounts.Controls.Add(lblCustomerInterest);
        grpCustomerAccounts.Controls.Add(sluInterestAccount);
        grpCustomerAccounts.Location = new Point(14, 14);
        grpCustomerAccounts.Name = "grpCustomerAccounts";
        grpCustomerAccounts.Size = new Size(410, 210);
        grpCustomerAccounts.TabIndex = 0;
        var grpCustomerAccountsTitle = new LabelControl();
        grpCustomerAccountsTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerAccountsTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerAccountsTitle.Appearance.Options.UseFont = true;
        grpCustomerAccountsTitle.Appearance.Options.UseForeColor = true;
        grpCustomerAccountsTitle.Location = new Point(13, 10);
        grpCustomerAccountsTitle.Name = "grpCustomerAccountsTitle";
        grpCustomerAccountsTitle.Text = "Cuentas contables";
        grpCustomerAccounts.Controls.Add(grpCustomerAccountsTitle);
        grpCustomerAccountsTitle.BringToFront();
        //
        // lblCustomerReceivable
        //
        lblCustomerReceivable.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerReceivable.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerReceivable.Appearance.Options.UseFont = true;
        lblCustomerReceivable.Appearance.Options.UseForeColor = true;
        lblCustomerReceivable.Location = new Point(18, 36);
        lblCustomerReceivable.Name = "lblCustomerReceivable";
        lblCustomerReceivable.Size = new Size(96, 15);
        lblCustomerReceivable.TabIndex = 0;
        lblCustomerReceivable.Text = "Cuenta por cobrar";
        //
        // lblCustomerAdvance
        //
        lblCustomerAdvance.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerAdvance.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerAdvance.Appearance.Options.UseFont = true;
        lblCustomerAdvance.Appearance.Options.UseForeColor = true;
        lblCustomerAdvance.Location = new Point(18, 66);
        lblCustomerAdvance.Name = "lblCustomerAdvance";
        lblCustomerAdvance.Size = new Size(88, 15);
        lblCustomerAdvance.TabIndex = 1;
        lblCustomerAdvance.Text = "Anticipo clientes";
        //
        // lblCustomerDiscount
        //
        lblCustomerDiscount.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerDiscount.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerDiscount.Appearance.Options.UseFont = true;
        lblCustomerDiscount.Appearance.Options.UseForeColor = true;
        lblCustomerDiscount.Location = new Point(18, 96);
        lblCustomerDiscount.Name = "lblCustomerDiscount";
        lblCustomerDiscount.Size = new Size(61, 15);
        lblCustomerDiscount.TabIndex = 2;
        lblCustomerDiscount.Text = "Descuentos";
        //
        // lblCustomerInterest
        //
        lblCustomerInterest.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerInterest.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerInterest.Appearance.Options.UseFont = true;
        lblCustomerInterest.Appearance.Options.UseForeColor = true;
        lblCustomerInterest.Location = new Point(18, 126);
        lblCustomerInterest.Name = "lblCustomerInterest";
        lblCustomerInterest.Size = new Size(98, 15);
        lblCustomerInterest.TabIndex = 3;
        lblCustomerInterest.Text = "Intereses cobrados";
        //
        // grpCustomerAssignments
        //
        grpCustomerAssignments.Controls.Add(lblCustomerCostCenter);
        grpCustomerAssignments.Controls.Add(lueCostCenter);
        grpCustomerAssignments.Controls.Add(lblCustomerProject);
        grpCustomerAssignments.Controls.Add(lueProject);
        grpCustomerAssignments.Location = new Point(438, 14);
        grpCustomerAssignments.Name = "grpCustomerAssignments";
        grpCustomerAssignments.Size = new Size(350, 210);
        grpCustomerAssignments.TabIndex = 1;
        var grpCustomerAssignmentsTitle = new LabelControl();
        grpCustomerAssignmentsTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerAssignmentsTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerAssignmentsTitle.Appearance.Options.UseFont = true;
        grpCustomerAssignmentsTitle.Appearance.Options.UseForeColor = true;
        grpCustomerAssignmentsTitle.Location = new Point(13, 10);
        grpCustomerAssignmentsTitle.Name = "grpCustomerAssignmentsTitle";
        grpCustomerAssignmentsTitle.Text = "Asignaciones";
        grpCustomerAssignments.Controls.Add(grpCustomerAssignmentsTitle);
        grpCustomerAssignmentsTitle.BringToFront();
        //
        // lblCustomerCostCenter
        //
        lblCustomerCostCenter.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerCostCenter.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerCostCenter.Appearance.Options.UseFont = true;
        lblCustomerCostCenter.Appearance.Options.UseForeColor = true;
        lblCustomerCostCenter.Location = new Point(18, 36);
        lblCustomerCostCenter.Name = "lblCustomerCostCenter";
        lblCustomerCostCenter.Size = new Size(84, 15);
        lblCustomerCostCenter.TabIndex = 0;
        lblCustomerCostCenter.Text = "Centro de costo";
        //
        // lblCustomerProject
        //
        lblCustomerProject.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerProject.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerProject.Appearance.Options.UseFont = true;
        lblCustomerProject.Appearance.Options.UseForeColor = true;
        lblCustomerProject.Location = new Point(18, 66);
        lblCustomerProject.Name = "lblCustomerProject";
        lblCustomerProject.Size = new Size(47, 15);
        lblCustomerProject.TabIndex = 5;
        lblCustomerProject.Text = "Proyecto";
        //
        // grpCustomerWithholdings
        //
        grpCustomerWithholdings.Controls.Add(lblCustomerIncomeWh);
        grpCustomerWithholdings.Controls.Add(sluIncomeWithholding);
        grpCustomerWithholdings.Controls.Add(lblCustomerVatWh);
        grpCustomerWithholdings.Controls.Add(sluVatWithholding);
        grpCustomerWithholdings.Controls.Add(lblCustomerIcaWh);
        grpCustomerWithholdings.Controls.Add(lueIcaWithholding);
        grpCustomerWithholdings.Location = new Point(802, 14);
        grpCustomerWithholdings.Name = "grpCustomerWithholdings";
        grpCustomerWithholdings.Size = new Size(428, 210);
        grpCustomerWithholdings.TabIndex = 2;
        var grpCustomerWithholdingsTitle = new LabelControl();
        grpCustomerWithholdingsTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerWithholdingsTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerWithholdingsTitle.Appearance.Options.UseFont = true;
        grpCustomerWithholdingsTitle.Appearance.Options.UseForeColor = true;
        grpCustomerWithholdingsTitle.Location = new Point(13, 10);
        grpCustomerWithholdingsTitle.Name = "grpCustomerWithholdingsTitle";
        grpCustomerWithholdingsTitle.Text = "Retenciones por defecto";
        grpCustomerWithholdings.Controls.Add(grpCustomerWithholdingsTitle);
        grpCustomerWithholdingsTitle.BringToFront();
        //
        // lblCustomerIncomeWh
        //
        lblCustomerIncomeWh.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerIncomeWh.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerIncomeWh.Appearance.Options.UseFont = true;
        lblCustomerIncomeWh.Appearance.Options.UseForeColor = true;
        lblCustomerIncomeWh.Location = new Point(18, 36);
        lblCustomerIncomeWh.Name = "lblCustomerIncomeWh";
        lblCustomerIncomeWh.Size = new Size(83, 15);
        lblCustomerIncomeWh.TabIndex = 0;
        lblCustomerIncomeWh.Text = "Retencion renta";
        //
        // lblCustomerVatWh
        //
        lblCustomerVatWh.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerVatWh.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerVatWh.Appearance.Options.UseFont = true;
        lblCustomerVatWh.Appearance.Options.UseForeColor = true;
        lblCustomerVatWh.Location = new Point(18, 66);
        lblCustomerVatWh.Name = "lblCustomerVatWh";
        lblCustomerVatWh.Size = new Size(74, 15);
        lblCustomerVatWh.TabIndex = 7;
        lblCustomerVatWh.Text = "Retencion IVA";
        //
        // lblCustomerIcaWh
        //
        lblCustomerIcaWh.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerIcaWh.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerIcaWh.Appearance.Options.UseFont = true;
        lblCustomerIcaWh.Appearance.Options.UseForeColor = true;
        lblCustomerIcaWh.Location = new Point(18, 96);
        lblCustomerIcaWh.Name = "lblCustomerIcaWh";
        lblCustomerIcaWh.Size = new Size(75, 15);
        lblCustomerIcaWh.TabIndex = 8;
        lblCustomerIcaWh.Text = "Retencion ICA";
        //
        // grpCustomerCurrency
        //
        grpCustomerCurrency.Controls.Add(lblCustomerAccountingCurrency);
        grpCustomerCurrency.Controls.Add(lueAccountingCurrency);
        grpCustomerCurrency.Controls.Add(lblCustomerExchangeRate);
        grpCustomerCurrency.Controls.Add(spnExchangeRate);
        grpCustomerCurrency.Controls.Add(lblCustomerValidation);
        grpCustomerCurrency.Controls.Add(lueValidationStatus);
        grpCustomerCurrency.Location = new Point(14, 236);
        grpCustomerCurrency.Name = "grpCustomerCurrency";
        grpCustomerCurrency.Size = new Size(1216, 114);
        grpCustomerCurrency.TabIndex = 3;
        var grpCustomerCurrencyTitle = new LabelControl();
        grpCustomerCurrencyTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerCurrencyTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerCurrencyTitle.Appearance.Options.UseFont = true;
        grpCustomerCurrencyTitle.Appearance.Options.UseForeColor = true;
        grpCustomerCurrencyTitle.Location = new Point(13, 10);
        grpCustomerCurrencyTitle.Name = "grpCustomerCurrencyTitle";
        grpCustomerCurrencyTitle.Text = "Moneda y validacion";
        grpCustomerCurrency.Controls.Add(grpCustomerCurrencyTitle);
        grpCustomerCurrencyTitle.BringToFront();
        //
        // lblCustomerAccountingCurrency
        //
        lblCustomerAccountingCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerAccountingCurrency.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerAccountingCurrency.Appearance.Options.UseFont = true;
        lblCustomerAccountingCurrency.Appearance.Options.UseForeColor = true;
        lblCustomerAccountingCurrency.Location = new Point(18, 36);
        lblCustomerAccountingCurrency.Name = "lblCustomerAccountingCurrency";
        lblCustomerAccountingCurrency.Size = new Size(93, 15);
        lblCustomerAccountingCurrency.TabIndex = 0;
        lblCustomerAccountingCurrency.Text = "Moneda contable";
        //
        // lblCustomerExchangeRate
        //
        lblCustomerExchangeRate.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerExchangeRate.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerExchangeRate.Appearance.Options.UseFont = true;
        lblCustomerExchangeRate.Appearance.Options.UseForeColor = true;
        lblCustomerExchangeRate.Location = new Point(420, 36);
        lblCustomerExchangeRate.Name = "lblCustomerExchangeRate";
        lblCustomerExchangeRate.Size = new Size(83, 15);
        lblCustomerExchangeRate.TabIndex = 10;
        lblCustomerExchangeRate.Text = "Tipo de cambio";
        //
        // lblCustomerValidation
        //
        lblCustomerValidation.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerValidation.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerValidation.Appearance.Options.UseFont = true;
        lblCustomerValidation.Appearance.Options.UseForeColor = true;
        lblCustomerValidation.Location = new Point(730, 36);
        lblCustomerValidation.Name = "lblCustomerValidation";
        lblCustomerValidation.Size = new Size(92, 15);
        lblCustomerValidation.TabIndex = 11;
        lblCustomerValidation.Text = "Estado validacion";
        //
        // xtpSap
        //
        xtpSap.Controls.Add(grpCustomerSapSync);
        xtpSap.Controls.Add(grpCustomerSapStatus);
        xtpSap.Controls.Add(grpCustomerSapTools);
        xtpSap.Controls.Add(grpCustomerSapLog);
        xtpSap.Name = "xtpSap";
        xtpSap.Size = new Size(1258, 364);
        xtpSap.Text = "SAP Business One";
        //
        // txtSapCardCode
        //
        txtSapCardCode.Location = new Point(150, 32);
        txtSapCardCode.Name = "txtSapCardCode";
        txtSapCardCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapCardCode.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtSapCardCode.Properties.Appearance.Options.UseFont = true;
        txtSapCardCode.Properties.Appearance.Options.UseForeColor = true;
        txtSapCardCode.Size = new Size(220, 22);
        txtSapCardCode.TabIndex = 0;
        //
        // lueSapGroup
        //
        lueSapGroup.Location = new Point(150, 62);
        lueSapGroup.Name = "lueSapGroup";
        lueSapGroup.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapGroup.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueSapGroup.Properties.Appearance.Options.UseFont = true;
        lueSapGroup.Properties.Appearance.Options.UseForeColor = true;
        lueSapGroup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapGroup.Size = new Size(220, 22);
        lueSapGroup.TabIndex = 1;
        //
        // lueSapPaymentTerm
        //
        lueSapPaymentTerm.Location = new Point(150, 92);
        lueSapPaymentTerm.Name = "lueSapPaymentTerm";
        lueSapPaymentTerm.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapPaymentTerm.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueSapPaymentTerm.Properties.Appearance.Options.UseFont = true;
        lueSapPaymentTerm.Properties.Appearance.Options.UseForeColor = true;
        lueSapPaymentTerm.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapPaymentTerm.Size = new Size(220, 22);
        lueSapPaymentTerm.TabIndex = 2;
        //
        // lueSapCurrency
        //
        lueSapCurrency.Location = new Point(150, 122);
        lueSapCurrency.Name = "lueSapCurrency";
        lueSapCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapCurrency.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueSapCurrency.Properties.Appearance.Options.UseFont = true;
        lueSapCurrency.Properties.Appearance.Options.UseForeColor = true;
        lueSapCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapCurrency.Size = new Size(220, 22);
        lueSapCurrency.TabIndex = 3;
        //
        // lueSapStatus
        //
        lueSapStatus.Location = new Point(150, 32);
        lueSapStatus.Name = "lueSapStatus";
        lueSapStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueSapStatus.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lueSapStatus.Properties.Appearance.Options.UseFont = true;
        lueSapStatus.Properties.Appearance.Options.UseForeColor = true;
        lueSapStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueSapStatus.Size = new Size(220, 22);
        lueSapStatus.TabIndex = 4;
        //
        // dtpSapLastSync
        //
        dtpSapLastSync.EditValue = new DateTime(2026, 5, 22, 0, 0, 0, 0);
        dtpSapLastSync.Location = new Point(150, 62);
        dtpSapLastSync.Name = "dtpSapLastSync";
        dtpSapLastSync.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        dtpSapLastSync.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        dtpSapLastSync.Properties.Appearance.Options.UseFont = true;
        dtpSapLastSync.Properties.Appearance.Options.UseForeColor = true;
        dtpSapLastSync.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        dtpSapLastSync.Size = new Size(220, 22);
        dtpSapLastSync.TabIndex = 5;
        //
        // txtSapUser
        //
        txtSapUser.Location = new Point(150, 92);
        txtSapUser.Name = "txtSapUser";
        txtSapUser.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapUser.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtSapUser.Properties.Appearance.Options.UseFont = true;
        txtSapUser.Properties.Appearance.Options.UseForeColor = true;
        txtSapUser.Size = new Size(220, 22);
        txtSapUser.TabIndex = 6;
        //
        // txtSapSourceSystem
        //
        txtSapSourceSystem.Location = new Point(150, 122);
        txtSapSourceSystem.Name = "txtSapSourceSystem";
        txtSapSourceSystem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapSourceSystem.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtSapSourceSystem.Properties.Appearance.Options.UseFont = true;
        txtSapSourceSystem.Properties.Appearance.Options.UseForeColor = true;
        txtSapSourceSystem.Size = new Size(220, 22);
        txtSapSourceSystem.TabIndex = 7;
        //
        // txtSapCompany
        //
        txtSapCompany.Location = new Point(150, 152);
        txtSapCompany.Name = "txtSapCompany";
        txtSapCompany.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapCompany.Properties.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        txtSapCompany.Properties.Appearance.Options.UseFont = true;
        txtSapCompany.Properties.Appearance.Options.UseForeColor = true;
        txtSapCompany.Size = new Size(220, 22);
        txtSapCompany.TabIndex = 8;
        //
        // btnSyncSap
        //
        btnSyncSap.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSyncSap.Appearance.Options.UseFont = true;
        btnSyncSap.Location = new Point(36, 70);
        btnSyncSap.Name = "btnSyncSap";
        btnSyncSap.Size = new Size(140, 32);
        btnSyncSap.TabIndex = 9;
        btnSyncSap.Text = "Sincronizar ahora";
        //
        // btnValidateSap
        //
        btnValidateSap.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnValidateSap.Appearance.Options.UseFont = true;
        btnValidateSap.Location = new Point(180, 70);
        btnValidateSap.Name = "btnValidateSap";
        btnValidateSap.Size = new Size(120, 32);
        btnValidateSap.TabIndex = 10;
        btnValidateSap.Text = "Validar datos";
        //
        // btnOpenSap
        //
        btnOpenSap.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnOpenSap.Appearance.Options.UseFont = true;
        btnOpenSap.Location = new Point(314, 70);
        btnOpenSap.Name = "btnOpenSap";
        btnOpenSap.Size = new Size(110, 32);
        btnOpenSap.TabIndex = 11;
        btnOpenSap.Text = "Ver en SAP";
        //
        // grdCustomerSapLog
        //
        grdCustomerSapLog.Font = new Font("Segoe UI", 9F);
        grdCustomerSapLog.Location = new Point(14, 30);
        grdCustomerSapLog.MainView = grvCustomerSapLog;
        grdCustomerSapLog.Name = "grdCustomerSapLog";
        grdCustomerSapLog.Size = new Size(1188, 88);
        grdCustomerSapLog.TabIndex = 12;
        grdCustomerSapLog.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvCustomerSapLog });
        //
        // grvCustomerSapLog
        //
        grvCustomerSapLog.Appearance.FilterPanel.Font = new Font("Segoe UI", 9F);
        grvCustomerSapLog.Appearance.FilterPanel.Options.UseFont = true;
        grvCustomerSapLog.Appearance.FooterPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grvCustomerSapLog.Appearance.FooterPanel.Options.UseFont = true;
        grvCustomerSapLog.Appearance.HeaderPanel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grvCustomerSapLog.Appearance.HeaderPanel.ForeColor = Color.FromArgb(23, 32, 51);
        grvCustomerSapLog.Appearance.HeaderPanel.Options.UseFont = true;
        grvCustomerSapLog.Appearance.HeaderPanel.Options.UseForeColor = true;
        grvCustomerSapLog.Appearance.Row.Font = new Font("Segoe UI", 9F);
        grvCustomerSapLog.Appearance.Row.ForeColor = Color.FromArgb(23, 32, 51);
        grvCustomerSapLog.Appearance.Row.Options.UseFont = true;
        grvCustomerSapLog.Appearance.Row.Options.UseForeColor = true;
        grvCustomerSapLog.GridControl = grdCustomerSapLog;
        grvCustomerSapLog.Name = "grvCustomerSapLog";
        grvCustomerSapLog.OptionsBehavior.Editable = false;
        grvCustomerSapLog.OptionsView.ShowGroupPanel = false;
        //
        // grpCustomerSapSync
        //
        grpCustomerSapSync.Controls.Add(lblCustomerSapCard);
        grpCustomerSapSync.Controls.Add(txtSapCardCode);
        grpCustomerSapSync.Controls.Add(lblCustomerSapGroup);
        grpCustomerSapSync.Controls.Add(lueSapGroup);
        grpCustomerSapSync.Controls.Add(lblCustomerSapTerm);
        grpCustomerSapSync.Controls.Add(lueSapPaymentTerm);
        grpCustomerSapSync.Controls.Add(lblCustomerSapCurrency);
        grpCustomerSapSync.Controls.Add(lueSapCurrency);
        grpCustomerSapSync.Location = new Point(14, 14);
        grpCustomerSapSync.Name = "grpCustomerSapSync";
        grpCustomerSapSync.Size = new Size(360, 190);
        grpCustomerSapSync.TabIndex = 0;
        var grpCustomerSapSyncTitle = new LabelControl();
        grpCustomerSapSyncTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerSapSyncTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerSapSyncTitle.Appearance.Options.UseFont = true;
        grpCustomerSapSyncTitle.Appearance.Options.UseForeColor = true;
        grpCustomerSapSyncTitle.Location = new Point(13, 10);
        grpCustomerSapSyncTitle.Name = "grpCustomerSapSyncTitle";
        grpCustomerSapSyncTitle.Text = "Sincronizacion SAP";
        grpCustomerSapSync.Controls.Add(grpCustomerSapSyncTitle);
        grpCustomerSapSyncTitle.BringToFront();
        //
        // lblCustomerSapCard
        //
        lblCustomerSapCard.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSapCard.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSapCard.Appearance.Options.UseFont = true;
        lblCustomerSapCard.Appearance.Options.UseForeColor = true;
        lblCustomerSapCard.Location = new Point(18, 36);
        lblCustomerSapCard.Name = "lblCustomerSapCard";
        lblCustomerSapCard.Size = new Size(53, 15);
        lblCustomerSapCard.TabIndex = 0;
        lblCustomerSapCard.Text = "CardCode";
        //
        // lblCustomerSapGroup
        //
        lblCustomerSapGroup.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSapGroup.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSapGroup.Appearance.Options.UseFont = true;
        lblCustomerSapGroup.Appearance.Options.UseForeColor = true;
        lblCustomerSapGroup.Location = new Point(18, 66);
        lblCustomerSapGroup.Name = "lblCustomerSapGroup";
        lblCustomerSapGroup.Size = new Size(85, 15);
        lblCustomerSapGroup.TabIndex = 1;
        lblCustomerSapGroup.Text = "Grupo de socios";
        //
        // lblCustomerSapTerm
        //
        lblCustomerSapTerm.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSapTerm.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSapTerm.Appearance.Options.UseFont = true;
        lblCustomerSapTerm.Appearance.Options.UseForeColor = true;
        lblCustomerSapTerm.Location = new Point(18, 96);
        lblCustomerSapTerm.Name = "lblCustomerSapTerm";
        lblCustomerSapTerm.Size = new Size(79, 15);
        lblCustomerSapTerm.TabIndex = 2;
        lblCustomerSapTerm.Text = "Condicion SAP";
        //
        // lblCustomerSapCurrency
        //
        lblCustomerSapCurrency.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSapCurrency.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSapCurrency.Appearance.Options.UseFont = true;
        lblCustomerSapCurrency.Appearance.Options.UseForeColor = true;
        lblCustomerSapCurrency.Location = new Point(18, 126);
        lblCustomerSapCurrency.Name = "lblCustomerSapCurrency";
        lblCustomerSapCurrency.Size = new Size(68, 15);
        lblCustomerSapCurrency.TabIndex = 3;
        lblCustomerSapCurrency.Text = "Moneda SAP";
        //
        // grpCustomerSapStatus
        //
        grpCustomerSapStatus.Controls.Add(lblCustomerSapStatus);
        grpCustomerSapStatus.Controls.Add(lueSapStatus);
        grpCustomerSapStatus.Controls.Add(lblCustomerSapLastSync);
        grpCustomerSapStatus.Controls.Add(dtpSapLastSync);
        grpCustomerSapStatus.Controls.Add(lblCustomerSapUser);
        grpCustomerSapStatus.Controls.Add(txtSapUser);
        grpCustomerSapStatus.Controls.Add(lblCustomerSapSource);
        grpCustomerSapStatus.Controls.Add(txtSapSourceSystem);
        grpCustomerSapStatus.Controls.Add(lblCustomerSapCompany);
        grpCustomerSapStatus.Controls.Add(txtSapCompany);
        grpCustomerSapStatus.Location = new Point(390, 14);
        grpCustomerSapStatus.Name = "grpCustomerSapStatus";
        grpCustomerSapStatus.Size = new Size(360, 190);
        grpCustomerSapStatus.TabIndex = 1;
        var grpCustomerSapStatusTitle = new LabelControl();
        grpCustomerSapStatusTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerSapStatusTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerSapStatusTitle.Appearance.Options.UseFont = true;
        grpCustomerSapStatusTitle.Appearance.Options.UseForeColor = true;
        grpCustomerSapStatusTitle.Location = new Point(13, 10);
        grpCustomerSapStatusTitle.Name = "grpCustomerSapStatusTitle";
        grpCustomerSapStatusTitle.Text = "Estado de sincronizacion";
        grpCustomerSapStatus.Controls.Add(grpCustomerSapStatusTitle);
        grpCustomerSapStatusTitle.BringToFront();
        //
        // lblCustomerSapStatus
        //
        lblCustomerSapStatus.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSapStatus.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSapStatus.Appearance.Options.UseFont = true;
        lblCustomerSapStatus.Appearance.Options.UseForeColor = true;
        lblCustomerSapStatus.Location = new Point(18, 36);
        lblCustomerSapStatus.Name = "lblCustomerSapStatus";
        lblCustomerSapStatus.Size = new Size(35, 15);
        lblCustomerSapStatus.TabIndex = 0;
        lblCustomerSapStatus.Text = "Estado";
        //
        // lblCustomerSapLastSync
        //
        lblCustomerSapLastSync.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSapLastSync.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSapLastSync.Appearance.Options.UseFont = true;
        lblCustomerSapLastSync.Appearance.Options.UseForeColor = true;
        lblCustomerSapLastSync.Location = new Point(18, 66);
        lblCustomerSapLastSync.Name = "lblCustomerSapLastSync";
        lblCustomerSapLastSync.Size = new Size(114, 15);
        lblCustomerSapLastSync.TabIndex = 5;
        lblCustomerSapLastSync.Text = "Ultima sincronizacion";
        //
        // lblCustomerSapUser
        //
        lblCustomerSapUser.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSapUser.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSapUser.Appearance.Options.UseFont = true;
        lblCustomerSapUser.Appearance.Options.UseForeColor = true;
        lblCustomerSapUser.Location = new Point(18, 96);
        lblCustomerSapUser.Name = "lblCustomerSapUser";
        lblCustomerSapUser.Size = new Size(89, 15);
        lblCustomerSapUser.TabIndex = 6;
        lblCustomerSapUser.Text = "Sincronizado por";
        //
        // lblCustomerSapSource
        //
        lblCustomerSapSource.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSapSource.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSapSource.Appearance.Options.UseFont = true;
        lblCustomerSapSource.Appearance.Options.UseForeColor = true;
        lblCustomerSapSource.Location = new Point(18, 126);
        lblCustomerSapSource.Name = "lblCustomerSapSource";
        lblCustomerSapSource.Size = new Size(78, 15);
        lblCustomerSapSource.TabIndex = 7;
        lblCustomerSapSource.Text = "Sistema origen";
        //
        // lblCustomerSapCompany
        //
        lblCustomerSapCompany.Appearance.Font = new Font("Segoe UI", 9F);
        lblCustomerSapCompany.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblCustomerSapCompany.Appearance.Options.UseFont = true;
        lblCustomerSapCompany.Appearance.Options.UseForeColor = true;
        lblCustomerSapCompany.Location = new Point(18, 156);
        lblCustomerSapCompany.Name = "lblCustomerSapCompany";
        lblCustomerSapCompany.Size = new Size(69, 15);
        lblCustomerSapCompany.TabIndex = 8;
        lblCustomerSapCompany.Text = "Empresa SAP";
        //
        // grpCustomerSapTools
        //
        grpCustomerSapTools.Controls.Add(btnSyncSap);
        grpCustomerSapTools.Controls.Add(btnValidateSap);
        grpCustomerSapTools.Controls.Add(btnOpenSap);
        grpCustomerSapTools.Location = new Point(766, 14);
        grpCustomerSapTools.Name = "grpCustomerSapTools";
        grpCustomerSapTools.Size = new Size(464, 190);
        grpCustomerSapTools.TabIndex = 2;
        var grpCustomerSapToolsTitle = new LabelControl();
        grpCustomerSapToolsTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerSapToolsTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerSapToolsTitle.Appearance.Options.UseFont = true;
        grpCustomerSapToolsTitle.Appearance.Options.UseForeColor = true;
        grpCustomerSapToolsTitle.Location = new Point(13, 10);
        grpCustomerSapToolsTitle.Name = "grpCustomerSapToolsTitle";
        grpCustomerSapToolsTitle.Text = "Herramientas";
        grpCustomerSapTools.Controls.Add(grpCustomerSapToolsTitle);
        grpCustomerSapToolsTitle.BringToFront();
        //
        // grpCustomerSapLog
        //
        grpCustomerSapLog.Controls.Add(grdCustomerSapLog);
        grpCustomerSapLog.Location = new Point(14, 216);
        grpCustomerSapLog.Name = "grpCustomerSapLog";
        grpCustomerSapLog.Size = new Size(1216, 134);
        grpCustomerSapLog.TabIndex = 3;
        var grpCustomerSapLogTitle = new LabelControl();
        grpCustomerSapLogTitle.Appearance.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        grpCustomerSapLogTitle.Appearance.ForeColor = BrandResources.Primary;
        grpCustomerSapLogTitle.Appearance.Options.UseFont = true;
        grpCustomerSapLogTitle.Appearance.Options.UseForeColor = true;
        grpCustomerSapLogTitle.Location = new Point(13, 10);
        grpCustomerSapLogTitle.Name = "grpCustomerSapLogTitle";
        grpCustomerSapLogTitle.Text = "Bitacora de integracion";
        grpCustomerSapLog.Controls.Add(grpCustomerSapLogTitle);
        grpCustomerSapLogTitle.BringToFront();
        //
        // pnlFooter
        //
        pnlFooter.BorderStyle = BorderStyles.NoBorder;
        pnlFooter.Controls.Add(btnSave);
        pnlFooter.Controls.Add(btnCancel);
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Location = new Point(0, 685);
        pnlFooter.Name = "pnlFooter";
        pnlFooter.Size = new Size(1286, 52);
        pnlFooter.TabIndex = 3;
        //
        // btnSave
        //
        btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSave.Appearance.BackColor = Color.FromArgb(0, 86, 210);
        btnSave.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.Location = new Point(2154, 9);
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(100, 36);
        btnSave.TabIndex = 0;
        btnSave.Text = "Guardar";
        //
        // btnCancel
        //
        btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCancel.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(2262, 9);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 36);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "Cancelar";
        //
        // CustomerEditForm
        //
        AcceptButton = btnSave;
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = btnCancel;
        ClientSize = new Size(1286, 737);
        Controls.Add(pnlMain);
        Name = "CustomerEditForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "NuanSystem ERP - Maestro de Clientes";
        ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
        pnlMain.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grpGeneralInfo).EndInit();
        grpGeneralInfo.ResumeLayout(false);
        grpGeneralInfo.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)txtCustomerCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCustomerName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtCustomerCommercialName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueIdentificationType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtIdentificationNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtExternalCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCustomerGroup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCustomerGroupView).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesPerson.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSalesPersonView).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPhone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtEmail.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtShortAddress.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grpClassification).EndInit();
        grpClassification.ResumeLayout(false);
        grpClassification.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueCustomerType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePriceList.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePaymentTerm.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditLimit.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueChannel.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueZone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRiskLevel.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsAllowSales.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsCreditBlocked.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsTaxExempt.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsStrategicCustomer.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)xtcMain).EndInit();
        xtcMain.ResumeLayout(false);
        xtpGeneral.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)picCustomerLogo.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memObservations.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtpStartDate.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtpStartDate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCustomerOrigin.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAbcClassification.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseFrequency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCurrentBalance.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnCreditAvailable.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtpLastPurchase.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtpLastPurchase.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnOpenOrders.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnYtdSales.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdCustomerContacts).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvCustomerContacts).EndInit();
        ((System.ComponentModel.ISupportInitialize)memInternalNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSegment.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueInternalClassification.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memCommercialTerms.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtTags.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerImage).EndInit();
        grpCustomerImage.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grpCustomerObservations).EndInit();
        grpCustomerObservations.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grpCustomerComplement).EndInit();
        grpCustomerComplement.ResumeLayout(false);
        grpCustomerComplement.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerSummary).EndInit();
        grpCustomerSummary.ResumeLayout(false);
        grpCustomerSummary.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerPreview).EndInit();
        grpCustomerPreview.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grpCustomerTerms).EndInit();
        grpCustomerTerms.ResumeLayout(false);
        grpCustomerTerms.PerformLayout();
        xtpFiscal.ResumeLayout(false);
        xtpFiscal.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueTaxpayerType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalRegime.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsAccountingRequired.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsWithholdingAgent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsSubjectToWithholding.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnWithholdingPercent.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueRentType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalProvince.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueFiscalCity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memFiscalAddress.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtFiscalPostalCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueEmissionType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDefaultSeries.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnInitialNumber.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePrintFormat.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memFiscalNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerFiscalInfo).EndInit();
        grpCustomerFiscalInfo.ResumeLayout(false);
        grpCustomerFiscalInfo.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerRetentions).EndInit();
        grpCustomerRetentions.ResumeLayout(false);
        grpCustomerRetentions.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerFiscalLocation).EndInit();
        grpCustomerFiscalLocation.ResumeLayout(false);
        grpCustomerFiscalLocation.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerFiscalDocuments).EndInit();
        grpCustomerFiscalDocuments.ResumeLayout(false);
        grpCustomerFiscalDocuments.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerFiscalNotes).EndInit();
        grpCustomerFiscalNotes.ResumeLayout(false);
        xtpAddresses.ResumeLayout(false);
        xtpAddresses.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdCustomerAddresses).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvCustomerAddresses).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAddressType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memAddress.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAddressCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAddressProvince.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAddressCity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtPostalCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtAddressReference.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsPrimaryAddress.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerAddressList).EndInit();
        grpCustomerAddressList.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grpCustomerAddressDetail).EndInit();
        grpCustomerAddressDetail.ResumeLayout(false);
        grpCustomerAddressDetail.PerformLayout();
        xtpContacts.ResumeLayout(false);
        xtpContacts.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grdCustomerContactList).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvCustomerContactList).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactPosition.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactPhone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactMobile.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtContactEmail.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsPrimaryContact.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tsActiveContact.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memContactNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerContactList).EndInit();
        grpCustomerContactList.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grpCustomerContactDetail).EndInit();
        grpCustomerContactDetail.ResumeLayout(false);
        grpCustomerContactDetail.PerformLayout();
        xtpCommercial.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)spnOverdueDays.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memCommercialNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerCommercialConditions).EndInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerCredit).EndInit();
        grpCustomerCredit.ResumeLayout(false);
        grpCustomerCredit.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerCommercialSummary).EndInit();
        grpCustomerCommercialSummary.ResumeLayout(false);
        grpCustomerCommercialSummary.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerCommercialNotes).EndInit();
        grpCustomerCommercialNotes.ResumeLayout(false);
        xtpAccounting.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)sluReceivableAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluReceivableAccountView).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluCustomerAdvanceAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluCustomerAdvanceAccountView).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluDiscountAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluDiscountAccountView).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluInterestAccount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluInterestAccountView).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCostCenter.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueProject.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluIncomeWithholding.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluIncomeWithholdingView).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluVatWithholding.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)sluVatWithholdingView).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueIcaWithholding.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueAccountingCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)spnExchangeRate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueValidationStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerAccounts).EndInit();
        grpCustomerAccounts.ResumeLayout(false);
        grpCustomerAccounts.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerAssignments).EndInit();
        grpCustomerAssignments.ResumeLayout(false);
        grpCustomerAssignments.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerWithholdings).EndInit();
        grpCustomerWithholdings.ResumeLayout(false);
        grpCustomerWithholdings.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerCurrency).EndInit();
        grpCustomerCurrency.ResumeLayout(false);
        grpCustomerCurrency.PerformLayout();
        xtpSap.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)txtSapCardCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapGroup.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapPaymentTerm.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueSapStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtpSapLastSync.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)dtpSapLastSync.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapUser.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapSourceSystem.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCompany.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)grdCustomerSapLog).EndInit();
        ((System.ComponentModel.ISupportInitialize)grvCustomerSapLog).EndInit();
        ((System.ComponentModel.ISupportInitialize)grpCustomerSapSync).EndInit();
        grpCustomerSapSync.ResumeLayout(false);
        grpCustomerSapSync.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerSapStatus).EndInit();
        grpCustomerSapStatus.ResumeLayout(false);
        grpCustomerSapStatus.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)grpCustomerSapTools).EndInit();
        grpCustomerSapTools.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)grpCustomerSapLog).EndInit();
        grpCustomerSapLog.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlFooter).EndInit();
        pnlFooter.ResumeLayout(false);
        ResumeLayout(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }

        base.Dispose(disposing);
    }
    private LabelControl lblCustomerCode;
    private LabelControl lblCustomerName;
    private LabelControl lblCustomerCommercialName;
    private LabelControl lblIdentificationType;
    private LabelControl lblIdentificationNumber;
    private LabelControl lblExternalCode;
    private LabelControl lblCustomerGroup;
    private GridView lueCustomerGroupView;
    private LabelControl lblSalesPerson;
    private GridView lueSalesPersonView;
    private LabelControl lblPhone;
    private LabelControl lblEmail;
    private LabelControl lblShortAddress;
    private LabelControl lblStatus;
    private LabelControl lblCustomerType;
    private LabelControl lblPriceList;
    private LabelControl lblPaymentTerm;
    private LabelControl lblCurrency;
    private LabelControl lblCreditLimit;
    private LabelControl lblChannel;
    private LabelControl lblZone;
    private LabelControl lblRisk;
    private LabelControl lblAllowSales;
    private LabelControl lblCreditBlocked;
    private LabelControl lblTaxExempt;
    private LabelControl lblStrategic;
    private PanelControl grpCustomerImage;
    private PanelControl grpCustomerObservations;
    private PanelControl grpCustomerComplement;
    private LabelControl lblCustomerStartDate;
    private LabelControl lblCustomerOrigin;
    private LabelControl lblCustomerAbc;
    private LabelControl lblCustomerFrequency;
    private PanelControl grpCustomerSummary;
    private LabelControl lblCustomerSalesYtd;
    private LabelControl lblCustomerBalance;
    private LabelControl lblCustomerAvailable;
    private LabelControl lblCustomerLastPurchase;
    private LabelControl lblCustomerOpenOrders;
    private PanelControl grpCustomerPreview;
    private PanelControl grpCustomerTerms;
    private LabelControl lblCustomerSegment;
    private LabelControl lblCustomerInternalClass;
    private LabelControl lblCustomerTags;
    private LabelControl lblFiscalTitle;
    private LabelControl lblRetentionTitle;
    private PanelControl grpCustomerFiscalInfo;
    private LabelControl lblCustomerTaxpayer;
    private LabelControl lblCustomerFiscalRegime;
    private LabelControl lblCustomerAccountingRequired;
    private LabelControl lblCustomerWithholdingAgent;
    private PanelControl grpCustomerRetentions;
    private LabelControl lblCustomerSubjectWithholding;
    private LabelControl lblCustomerWithholdingPercent;
    private LabelControl lblCustomerRentType;
    private PanelControl grpCustomerFiscalLocation;
    private LabelControl lblCustomerFiscalCountry;
    private LabelControl lblCustomerFiscalProvince;
    private LabelControl lblCustomerFiscalCity;
    private LabelControl lblCustomerFiscalAddress;
    private LabelControl lblCustomerFiscalPostal;
    private PanelControl grpCustomerFiscalDocuments;
    private LabelControl lblCustomerEmissionType;
    private LabelControl lblCustomerDefaultSeries;
    private LabelControl lblCustomerInitialNumber;
    private LabelControl lblCustomerPrintFormat;
    private PanelControl grpCustomerFiscalNotes;
    private LabelControl lblAddressButtons;
    private PanelControl grpCustomerAddressList;
    private PanelControl grpCustomerAddressDetail;
    private LabelControl lblCustomerAddressType;
    private LabelControl lblCustomerAddress;
    private LabelControl lblCustomerAddressCountry;
    private LabelControl lblCustomerAddressProvince;
    private LabelControl lblCustomerAddressCity;
    private LabelControl lblCustomerPostal;
    private LabelControl lblCustomerReference;
    private LabelControl lblCustomerPrimaryAddress;
    private LabelControl lblContactButtons;
    private PanelControl grpCustomerContactList;
    private PanelControl grpCustomerContactDetail;
    private LabelControl lblCustomerContactName;
    private LabelControl lblCustomerContactPosition;
    private LabelControl lblCustomerContactPhone;
    private LabelControl lblCustomerContactMobile;
    private LabelControl lblCustomerContactEmail;
    private LabelControl lblCustomerPrimaryContact;
    private LabelControl lblCustomerActiveContact;
    private PanelControl grpCustomerCommercialConditions;
    private PanelControl grpCustomerCredit;
    private LabelControl lblCustomerOverdue;
    private PanelControl grpCustomerCommercialSummary;
    private LabelControl lblCustomerCommercialSummaryHint;
    private PanelControl grpCustomerCommercialNotes;
    private PanelControl grpCustomerAccounts;
    private LabelControl lblCustomerReceivable;
    private GridView sluReceivableAccountView;
    private LabelControl lblCustomerAdvance;
    private GridView sluCustomerAdvanceAccountView;
    private LabelControl lblCustomerDiscount;
    private GridView sluDiscountAccountView;
    private LabelControl lblCustomerInterest;
    private GridView sluInterestAccountView;
    private PanelControl grpCustomerAssignments;
    private LabelControl lblCustomerCostCenter;
    private LabelControl lblCustomerProject;
    private PanelControl grpCustomerWithholdings;
    private LabelControl lblCustomerIncomeWh;
    private GridView sluIncomeWithholdingView;
    private LabelControl lblCustomerVatWh;
    private GridView sluVatWithholdingView;
    private LabelControl lblCustomerIcaWh;
    private PanelControl grpCustomerCurrency;
    private LabelControl lblCustomerAccountingCurrency;
    private LabelControl lblCustomerExchangeRate;
    private LabelControl lblCustomerValidation;
    private PanelControl grpCustomerSapSync;
    private LabelControl lblCustomerSapCard;
    private LabelControl lblCustomerSapGroup;
    private LabelControl lblCustomerSapTerm;
    private LabelControl lblCustomerSapCurrency;
    private PanelControl grpCustomerSapStatus;
    private LabelControl lblCustomerSapStatus;
    private LabelControl lblCustomerSapLastSync;
    private LabelControl lblCustomerSapUser;
    private LabelControl lblCustomerSapSource;
    private LabelControl lblCustomerSapCompany;
    private PanelControl grpCustomerSapTools;
    private PanelControl grpCustomerSapLog;
}





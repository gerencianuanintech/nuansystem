using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Purchasing.PurchaseOrders;

partial class FrmPurchaseOrderEdit
{
    private System.ComponentModel.IContainer components = null;
    private PanelControl pnlMain;
    private LabelControl lblSupplier;
    private SearchLookUpEdit slueSupplier;
    private GridView gvSupplierLookup;
    private LabelControl lblSupplierTaxId;
    private TextEdit txtSupplierTaxId;
    private LabelControl lblSupplierContact;
    private TextEdit txtSupplierContact;
    private LabelControl lblSupplierPhone;
    private TextEdit txtSupplierPhone;
    private LabelControl lblSupplierEmail;
    private TextEdit txtSupplierEmail;
    private LabelControl lblDocumentDate;
    private DateEdit deDocumentDate;
    private LabelControl lblDeliveryDate;
    private DateEdit deDeliveryDate;
    private LabelControl lblCurrency;
    private LookUpEdit lueCurrency;
    private LabelControl lblPaymentTerm;
    private LookUpEdit luePaymentTerm;
    private LabelControl lblPriceList;
    private LookUpEdit luePriceList;
    private LabelControl lblBuyer;
    private LookUpEdit lueBuyer;
    private LabelControl lblMainWarehouse;
    private LookUpEdit lueMainWarehouse;
    private LabelControl lblProject;
    private LookUpEdit lueProject;
    private LabelControl lblCostCenter;
    private LookUpEdit lueCostCenter;
    private LabelControl lblPurchaseType;
    private LookUpEdit luePurchaseType;
    private LabelControl lblComments;
    private MemoEdit memoComments;
    private XtraTabControl tabPurchaseOrder;
    private XtraTabPage tabDetail;
    private XtraTabPage tabAddresses;
    private XtraTabPage tabApproval;
    private XtraTabPage tabRelatedDocuments;
    private XtraTabPage tabSap;
    private XtraTabPage tabAttachments;
    private LabelControl lblDetailHint;
    private GridControl gridLines;
    private GridView viewLines;
    private GridColumn colLineNumber;
    private GridColumn colLineItemCode;
    private GridColumn colLineDescription;
    private GridColumn colLineUnit;
    private GridColumn colLineQuantity;
    private GridColumn colLineOpenQuantity;
    private GridColumn colLineUnitPrice;
    private GridColumn colLineDiscountPercent;
    private GridColumn colLineTax;
    private GridColumn colLineWarehouse;
    private GridColumn colLineDeliveryDate;
    private GridColumn colLineCostCenter;
    private GridColumn colLineProject;
    private GridColumn colLineTotal;
    private RepositoryItemSearchLookUpEdit repoItem;
    private GridView gvItemRepository;
    private RepositoryItemLookUpEdit repoUnit;
    private RepositoryItemLookUpEdit repoTax;
    private RepositoryItemLookUpEdit repoWarehouse;
    private RepositoryItemLookUpEdit repoCostCenter;
    private RepositoryItemLookUpEdit repoProject;
    private RepositoryItemDateEdit repoDeliveryDate;
    private RepositoryItemSpinEdit repoQuantity;
    private RepositoryItemCalcEdit repoMoney;
    private PanelControl pnlDetailTotals;
    private LabelControl lblDetailSubtotalCaption;
    private LabelControl lblDetailSubtotal;
    private LabelControl lblGlobalDiscountPercent;
    private SpinEdit spnGlobalDiscountPercent;
    private LabelControl lblDetailDiscountCaption;
    private LabelControl lblDetailDiscount;
    private LabelControl lblDetailBaseCaption;
    private LabelControl lblDetailBase;
    private LabelControl lblDetailTaxCaption;
    private LabelControl lblDetailTax;
    private LabelControl lblDetailTotalCaption;
    private LabelControl lblDetailTotal;
    private PanelControl pnlDeliveryAddress;
    private PanelControl pnlBillingAddress;
    private LabelControl lblDeliveryAddressTitle;
    private LabelControl lblBillingAddressTitle;
    private LabelControl lblDeliveryAddressSelector;
    private LookUpEdit lueDeliveryAddress;
    private SimpleButton btnDeliveryAddressLookup;
    private LabelControl lblDeliveryNameCaption;
    private TextEdit txtDeliveryAddressName;
    private LabelControl lblDeliveryStreetCaption;
    private MemoEdit memoDeliveryStreet;
    private LabelControl lblDeliveryReferenceCaption;
    private TextEdit txtDeliveryReference;
    private LabelControl lblDeliveryCityCaption;
    private TextEdit txtDeliveryCity;
    private LabelControl lblDeliveryStateCaption;
    private TextEdit txtDeliveryState;
    private LabelControl lblDeliveryZipCodeCaption;
    private TextEdit txtDeliveryZipCode;
    private LabelControl lblDeliveryCountryCaption;
    private TextEdit txtDeliveryCountry;
    private LabelControl lblDeliveryPhoneCaption;
    private TextEdit txtDeliveryPhone;
    private LabelControl lblDeliveryInfo;
    private LabelControl lblBillingAddressSelector;
    private LookUpEdit lueBillingAddress;
    private SimpleButton btnBillingAddressLookup;
    private LabelControl lblBillingNameCaption;
    private TextEdit txtBillingAddressName;
    private LabelControl lblBillingStreetCaption;
    private MemoEdit memoBillingStreet;
    private LabelControl lblBillingReferenceCaption;
    private TextEdit txtBillingReference;
    private LabelControl lblBillingCityCaption;
    private TextEdit txtBillingCity;
    private LabelControl lblBillingStateCaption;
    private TextEdit txtBillingState;
    private LabelControl lblBillingZipCodeCaption;
    private TextEdit txtBillingZipCode;
    private LabelControl lblBillingCountryCaption;
    private TextEdit txtBillingCountry;
    private LabelControl lblBillingPhoneCaption;
    private TextEdit txtBillingPhone;
    private LabelControl lblBillingEmailCaption;
    private TextEdit txtBillingEmail;
    private LabelControl lblBillingInfo;
    private PanelControl pnlApprovalAmountCard;
    private PanelControl pnlApprovalPolicyCard;
    private PanelControl pnlApprovalLevelCard;
    private PanelControl pnlApprovalStatusCard;
    private LabelControl lblApprovalAmountIcon;
    private LabelControl lblApprovalPolicyIcon;
    private LabelControl lblApprovalLevelIcon;
    private LabelControl lblApprovalStatusIcon;
    private LabelControl lblApprovalAmount;
    private TextEdit txtApprovalAmount;
    private LabelControl lblApprovalAmountCurrency;
    private LabelControl lblApprovalPolicy;
    private TextEdit txtApprovalPolicy;
    private LabelControl lblApprovalPolicyDescription;
    private LabelControl lblApprovalLevel;
    private TextEdit txtApprovalLevel;
    private LabelControl lblApprovalLevelDescription;
    private LabelControl lblApprovalStatus;
    private TextEdit txtApprovalStatus;
    private LabelControl lblApprovalStatusDescription;
    private LabelControl lblApprovalHistoryTitle;
    private PanelControl pnlApprovalComment;
    private LabelControl lblApprovalCommentTitle;
    private MemoEdit memoApprovalObservation;
    private PanelControl pnlApprovalFlow;
    private LabelControl lblApprovalFlowTitle;
    private GridControl gridApprovalFlow;
    private GridView viewApprovalFlow;
    private GridColumn colApprovalFlowStep;
    private GridColumn colApprovalFlowRole;
    private GridColumn colApprovalFlowUser;
    private GridColumn colApprovalFlowStatus;
    private GridColumn colApprovalFlowDate;
    private GridControl gridApprovals;
    private GridView viewApprovals;
    private GridControl gridRelatedDocuments;
    private GridView viewRelatedDocuments;
    private GridColumn colRelatedDocumentIcon;
    private GridColumn colRelatedDocumentType;
    private GridColumn colRelatedDocumentSeries;
    private GridColumn colRelatedDocumentNumber;
    private GridColumn colRelatedDocumentDate;
    private GridColumn colRelatedDocumentStatus;
    private GridColumn colRelatedDocumentReference;
    private GridColumn colRelatedDocumentComment;
    private GridColumn colRelatedDocumentTotal;
    private GridColumn colRelatedDocumentAction;
    private RepositoryItemButtonEdit repoRelatedDocumentAction;
    private LabelControl lblRelatedDocumentsTitle;
    private PanelControl pnlRelatedDocumentNotes;
    private LabelControl lblRelatedDocumentNotesTitle;
    private MemoEdit memoRelatedDocumentNotes;
    private SimpleButton btnAddRelatedDocument;
    private SimpleButton btnViewRelatedDocument;
    private SimpleButton btnUnlinkRelatedDocument;
    private SimpleButton btnRefreshRelatedDocuments;
    private PanelControl pnlSapSync;
    private PanelControl pnlSapDocument;
    private PanelControl pnlSapMessages;
    private LabelControl lblSapSyncTitle;
    private LabelControl lblSapStatusCaption;
    private TextEdit txtSapStatus;
    private LabelControl lblSapSyncDocEntryCaption;
    private TextEdit txtSapSyncDocEntry;
    private LabelControl lblSapSyncDocNumCaption;
    private TextEdit txtSapSyncDocNum;
    private LabelControl lblSapObjectTypeCaption;
    private TextEdit txtSapObjectType;
    private LabelControl lblSapSyncDateCaption;
    private TextEdit txtSapSyncDate;
    private LabelControl lblSapUserCaption;
    private TextEdit txtSapUser;
    private LabelControl lblSapLastErrorCaption;
    private TextEdit txtSapLastError;
    private LabelControl lblSapDocumentTitle;
    private LabelControl lblSapDocEntryCaption;
    private TextEdit txtSapDocEntry;
    private LabelControl lblSapDocNumCaption;
    private TextEdit txtSapDocNum;
    private LabelControl lblSapCurrencyCaption;
    private TextEdit txtSapCurrency;
    private LabelControl lblSapTotalCaption;
    private TextEdit txtSapTotal;
    private SimpleButton btnSyncSap;
    private SimpleButton btnRefreshSapStatus;
    private SimpleButton btnCancelSapSync;
    private LabelControl lblSapMessagesTitle;
    private MemoEdit memoSapMessage;
    private LabelControl lblSapLogsTitle;
    private GridControl gridSapLogs;
    private GridView viewSapLogs;
    private GridColumn colSapLogCreatedAt;
    private GridColumn colSapLogProcess;
    private GridColumn colSapLogStatus;
    private GridColumn colSapLogMessage;
    private GridColumn colSapLogUser;
    private GridColumn colSapLogAttempt;
    private GridControl gridAttachments;
    private GridView viewAttachments;
    private GridColumn colAttachmentIcon;
    private GridColumn colAttachmentFileName;
    private GridColumn colAttachmentType;
    private GridColumn colAttachmentSize;
    private GridColumn colAttachmentCreatedAt;
    private GridColumn colAttachmentUser;
    private GridColumn colAttachmentStatus;
    private GridColumn colAttachmentComment;
    private SimpleButton btnAddAttachment;
    private SimpleButton btnDownloadAttachment;
    private SimpleButton btnOpenAttachment;
    private SimpleButton btnRemoveAttachment;
    private SimpleButton btnRefreshAttachments;
    private LabelControl lblAttachmentFooterCount;
    private LabelControl lblAttachmentFooterSize;
    private PanelControl pnlAttachmentPreview;
    private LabelControl lblAttachmentPreviewTitle;
    private LabelControl lblAttachmentTypeCaption;
    private LabelControl lblAttachmentTypeValue;
    private LabelControl lblAttachmentSizeCaption;
    private LabelControl lblAttachmentSizeValue;
    private LabelControl lblAttachmentDateCaption;
    private LabelControl lblAttachmentDateValue;
    private LabelControl lblAttachmentUserCaption;
    private LabelControl lblAttachmentUserValue;
    private LabelControl lblAttachmentStatusCaption;
    private LabelControl lblAttachmentStatusValue;
    private LabelControl lblAttachmentCommentCaption;
    private LabelControl lblAttachmentCommentValue;
    private PictureEdit picAttachmentPreview;
    private PanelControl pnlSummary;
    private LabelControl lblSummaryTitle;
    private LabelControl lblSummarySubtotalCaption;
    private LabelControl lblSummarySubtotal;
    private LabelControl lblSummaryDiscountCaption;
    private LabelControl lblSummaryDiscount;
    private LabelControl lblSummaryBaseCaption;
    private LabelControl lblSummaryBase;
    private LabelControl lblSummaryTaxCaption;
    private LabelControl lblSummaryTax;
    private LabelControl lblSummaryTotalCaption;
    private LabelControl lblSummaryTotal;
    private LabelControl lblSummaryItemsCaption;
    private LabelControl lblSummaryItems;
    private LabelControl lblSummaryQuantityCaption;
    private LabelControl lblSummaryQuantity;
    private LabelControl lblSummaryWeightCaption;
    private LabelControl lblSummaryWeight;
    private PanelControl pnlFooter;
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
        pnlMain = new PanelControl();
        lblSupplier = new LabelControl();
        slueSupplier = new SearchLookUpEdit();
        gvSupplierLookup = new GridView();
        lblSupplierTaxId = new LabelControl();
        txtSupplierTaxId = new TextEdit();
        lblSupplierContact = new LabelControl();
        txtSupplierContact = new TextEdit();
        lblSupplierPhone = new LabelControl();
        txtSupplierPhone = new TextEdit();
        lblSupplierEmail = new LabelControl();
        txtSupplierEmail = new TextEdit();
        lblDocumentDate = new LabelControl();
        deDocumentDate = new DateEdit();
        lblDeliveryDate = new LabelControl();
        deDeliveryDate = new DateEdit();
        lblCurrency = new LabelControl();
        lueCurrency = new LookUpEdit();
        lblPaymentTerm = new LabelControl();
        luePaymentTerm = new LookUpEdit();
        lblPriceList = new LabelControl();
        luePriceList = new LookUpEdit();
        lblBuyer = new LabelControl();
        lueBuyer = new LookUpEdit();
        lblMainWarehouse = new LabelControl();
        lueMainWarehouse = new LookUpEdit();
        lblProject = new LabelControl();
        lueProject = new LookUpEdit();
        lblCostCenter = new LabelControl();
        lueCostCenter = new LookUpEdit();
        lblPurchaseType = new LabelControl();
        luePurchaseType = new LookUpEdit();
        lblComments = new LabelControl();
        memoComments = new MemoEdit();
        tabPurchaseOrder = new XtraTabControl();
        tabDetail = new XtraTabPage();
        lblDetailHint = new LabelControl();
        gridLines = new GridControl();
        viewLines = new GridView();
        colLineNumber = new GridColumn();
        colLineItemCode = new GridColumn();
        repoItem = new RepositoryItemSearchLookUpEdit();
        gvItemRepository = new GridView();
        colLineDescription = new GridColumn();
        colLineUnit = new GridColumn();
        repoUnit = new RepositoryItemLookUpEdit();
        colLineQuantity = new GridColumn();
        repoQuantity = new RepositoryItemSpinEdit();
        colLineOpenQuantity = new GridColumn();
        colLineUnitPrice = new GridColumn();
        repoMoney = new RepositoryItemCalcEdit();
        colLineDiscountPercent = new GridColumn();
        colLineTax = new GridColumn();
        repoTax = new RepositoryItemLookUpEdit();
        colLineWarehouse = new GridColumn();
        repoWarehouse = new RepositoryItemLookUpEdit();
        colLineDeliveryDate = new GridColumn();
        repoDeliveryDate = new RepositoryItemDateEdit();
        colLineCostCenter = new GridColumn();
        repoCostCenter = new RepositoryItemLookUpEdit();
        colLineProject = new GridColumn();
        repoProject = new RepositoryItemLookUpEdit();
        colLineTotal = new GridColumn();
        pnlDetailTotals = new PanelControl();
        lblDetailSubtotalCaption = new LabelControl();
        lblDetailSubtotal = new LabelControl();
        lblGlobalDiscountPercent = new LabelControl();
        spnGlobalDiscountPercent = new SpinEdit();
        lblDetailDiscountCaption = new LabelControl();
        lblDetailDiscount = new LabelControl();
        lblDetailBaseCaption = new LabelControl();
        lblDetailBase = new LabelControl();
        lblDetailTaxCaption = new LabelControl();
        lblDetailTax = new LabelControl();
        lblDetailTotalCaption = new LabelControl();
        lblDetailTotal = new LabelControl();
        tabAddresses = new XtraTabPage();
        pnlDeliveryAddress = new PanelControl();
        lblDeliveryAddressTitle = new LabelControl();
        lblDeliveryAddressSelector = new LabelControl();
        lueDeliveryAddress = new LookUpEdit();
        btnDeliveryAddressLookup = new SimpleButton();
        lblDeliveryNameCaption = new LabelControl();
        txtDeliveryAddressName = new TextEdit();
        lblDeliveryStreetCaption = new LabelControl();
        memoDeliveryStreet = new MemoEdit();
        lblDeliveryReferenceCaption = new LabelControl();
        txtDeliveryReference = new TextEdit();
        lblDeliveryCityCaption = new LabelControl();
        txtDeliveryCity = new TextEdit();
        lblDeliveryStateCaption = new LabelControl();
        txtDeliveryState = new TextEdit();
        lblDeliveryZipCodeCaption = new LabelControl();
        txtDeliveryZipCode = new TextEdit();
        lblDeliveryCountryCaption = new LabelControl();
        txtDeliveryCountry = new TextEdit();
        lblDeliveryPhoneCaption = new LabelControl();
        txtDeliveryPhone = new TextEdit();
        lblDeliveryInfo = new LabelControl();
        pnlBillingAddress = new PanelControl();
        lblBillingAddressTitle = new LabelControl();
        lblBillingAddressSelector = new LabelControl();
        lueBillingAddress = new LookUpEdit();
        btnBillingAddressLookup = new SimpleButton();
        lblBillingNameCaption = new LabelControl();
        txtBillingAddressName = new TextEdit();
        lblBillingStreetCaption = new LabelControl();
        memoBillingStreet = new MemoEdit();
        lblBillingReferenceCaption = new LabelControl();
        txtBillingReference = new TextEdit();
        lblBillingCityCaption = new LabelControl();
        txtBillingCity = new TextEdit();
        lblBillingStateCaption = new LabelControl();
        txtBillingState = new TextEdit();
        lblBillingZipCodeCaption = new LabelControl();
        txtBillingZipCode = new TextEdit();
        lblBillingCountryCaption = new LabelControl();
        txtBillingCountry = new TextEdit();
        lblBillingPhoneCaption = new LabelControl();
        txtBillingPhone = new TextEdit();
        lblBillingEmailCaption = new LabelControl();
        txtBillingEmail = new TextEdit();
        lblBillingInfo = new LabelControl();
        tabApproval = new XtraTabPage();
        pnlApprovalAmountCard = new PanelControl();
        pnlApprovalPolicyCard = new PanelControl();
        pnlApprovalLevelCard = new PanelControl();
        pnlApprovalStatusCard = new PanelControl();
        lblApprovalAmountIcon = new LabelControl();
        lblApprovalPolicyIcon = new LabelControl();
        lblApprovalLevelIcon = new LabelControl();
        lblApprovalStatusIcon = new LabelControl();
        lblApprovalAmount = new LabelControl();
        txtApprovalAmount = new TextEdit();
        lblApprovalAmountCurrency = new LabelControl();
        lblApprovalPolicy = new LabelControl();
        txtApprovalPolicy = new TextEdit();
        lblApprovalPolicyDescription = new LabelControl();
        lblApprovalLevel = new LabelControl();
        txtApprovalLevel = new TextEdit();
        lblApprovalLevelDescription = new LabelControl();
        lblApprovalStatus = new LabelControl();
        txtApprovalStatus = new TextEdit();
        lblApprovalStatusDescription = new LabelControl();
        lblApprovalHistoryTitle = new LabelControl();
        gridApprovals = new GridControl();
        viewApprovals = new GridView();
        pnlApprovalComment = new PanelControl();
        lblApprovalCommentTitle = new LabelControl();
        memoApprovalObservation = new MemoEdit();
        pnlApprovalFlow = new PanelControl();
        lblApprovalFlowTitle = new LabelControl();
        gridApprovalFlow = new GridControl();
        viewApprovalFlow = new GridView();
        colApprovalFlowStep = new GridColumn();
        colApprovalFlowRole = new GridColumn();
        colApprovalFlowUser = new GridColumn();
        colApprovalFlowStatus = new GridColumn();
        colApprovalFlowDate = new GridColumn();
        tabRelatedDocuments = new XtraTabPage();
        btnAddRelatedDocument = new SimpleButton();
        btnViewRelatedDocument = new SimpleButton();
        btnUnlinkRelatedDocument = new SimpleButton();
        btnRefreshRelatedDocuments = new SimpleButton();
        lblRelatedDocumentsTitle = new LabelControl();
        gridRelatedDocuments = new GridControl();
        viewRelatedDocuments = new GridView();
        colRelatedDocumentIcon = new GridColumn();
        colRelatedDocumentType = new GridColumn();
        colRelatedDocumentSeries = new GridColumn();
        colRelatedDocumentNumber = new GridColumn();
        colRelatedDocumentDate = new GridColumn();
        colRelatedDocumentStatus = new GridColumn();
        colRelatedDocumentReference = new GridColumn();
        colRelatedDocumentComment = new GridColumn();
        colRelatedDocumentTotal = new GridColumn();
        colRelatedDocumentAction = new GridColumn();
        repoRelatedDocumentAction = new RepositoryItemButtonEdit();
        pnlRelatedDocumentNotes = new PanelControl();
        lblRelatedDocumentNotesTitle = new LabelControl();
        memoRelatedDocumentNotes = new MemoEdit();
        tabSap = new XtraTabPage();
        pnlSapSync = new PanelControl();
        pnlSapDocument = new PanelControl();
        pnlSapMessages = new PanelControl();
        lblSapSyncTitle = new LabelControl();
        lblSapStatusCaption = new LabelControl();
        txtSapStatus = new TextEdit();
        lblSapSyncDocEntryCaption = new LabelControl();
        txtSapSyncDocEntry = new TextEdit();
        lblSapSyncDocNumCaption = new LabelControl();
        txtSapSyncDocNum = new TextEdit();
        lblSapObjectTypeCaption = new LabelControl();
        txtSapObjectType = new TextEdit();
        lblSapSyncDateCaption = new LabelControl();
        txtSapSyncDate = new TextEdit();
        lblSapUserCaption = new LabelControl();
        txtSapUser = new TextEdit();
        lblSapLastErrorCaption = new LabelControl();
        txtSapLastError = new TextEdit();
        lblSapDocumentTitle = new LabelControl();
        lblSapDocEntryCaption = new LabelControl();
        txtSapDocEntry = new TextEdit();
        lblSapDocNumCaption = new LabelControl();
        txtSapDocNum = new TextEdit();
        lblSapCurrencyCaption = new LabelControl();
        txtSapCurrency = new TextEdit();
        lblSapTotalCaption = new LabelControl();
        txtSapTotal = new TextEdit();
        btnSyncSap = new SimpleButton();
        btnRefreshSapStatus = new SimpleButton();
        btnCancelSapSync = new SimpleButton();
        lblSapMessagesTitle = new LabelControl();
        memoSapMessage = new MemoEdit();
        lblSapLogsTitle = new LabelControl();
        gridSapLogs = new GridControl();
        viewSapLogs = new GridView();
        colSapLogCreatedAt = new GridColumn();
        colSapLogProcess = new GridColumn();
        colSapLogStatus = new GridColumn();
        colSapLogMessage = new GridColumn();
        colSapLogUser = new GridColumn();
        colSapLogAttempt = new GridColumn();
        tabAttachments = new XtraTabPage();
        btnAddAttachment = new SimpleButton();
        btnDownloadAttachment = new SimpleButton();
        btnOpenAttachment = new SimpleButton();
        btnRemoveAttachment = new SimpleButton();
        btnRefreshAttachments = new SimpleButton();
        gridAttachments = new GridControl();
        viewAttachments = new GridView();
        colAttachmentIcon = new GridColumn();
        colAttachmentFileName = new GridColumn();
        colAttachmentType = new GridColumn();
        colAttachmentSize = new GridColumn();
        colAttachmentCreatedAt = new GridColumn();
        colAttachmentUser = new GridColumn();
        colAttachmentStatus = new GridColumn();
        colAttachmentComment = new GridColumn();
        lblAttachmentFooterCount = new LabelControl();
        lblAttachmentFooterSize = new LabelControl();
        pnlAttachmentPreview = new PanelControl();
        lblAttachmentPreviewTitle = new LabelControl();
        lblAttachmentTypeCaption = new LabelControl();
        lblAttachmentTypeValue = new LabelControl();
        lblAttachmentSizeCaption = new LabelControl();
        lblAttachmentSizeValue = new LabelControl();
        lblAttachmentDateCaption = new LabelControl();
        lblAttachmentDateValue = new LabelControl();
        lblAttachmentUserCaption = new LabelControl();
        lblAttachmentUserValue = new LabelControl();
        lblAttachmentStatusCaption = new LabelControl();
        lblAttachmentStatusValue = new LabelControl();
        lblAttachmentCommentCaption = new LabelControl();
        lblAttachmentCommentValue = new LabelControl();
        picAttachmentPreview = new PictureEdit();
        pnlSummary = new PanelControl();
        lblSummaryTitle = new LabelControl();
        lblSummarySubtotalCaption = new LabelControl();
        lblSummarySubtotal = new LabelControl();
        lblSummaryDiscountCaption = new LabelControl();
        lblSummaryDiscount = new LabelControl();
        lblSummaryBaseCaption = new LabelControl();
        lblSummaryBase = new LabelControl();
        lblSummaryTaxCaption = new LabelControl();
        lblSummaryTax = new LabelControl();
        lblSummaryTotalCaption = new LabelControl();
        lblSummaryTotal = new LabelControl();
        lblSummaryItemsCaption = new LabelControl();
        lblSummaryItems = new LabelControl();
        lblSummaryQuantityCaption = new LabelControl();
        lblSummaryQuantity = new LabelControl();
        lblSummaryWeightCaption = new LabelControl();
        lblSummaryWeight = new LabelControl();
        pnlFooter = new PanelControl();
        btnSave = new SimpleButton();
        btnCancel = new SimpleButton();
        pnlHeader = new PanelControl();
        lblStatus = new LabelControl();
        lblNumberValue = new LabelControl();
        lblNumberCaption = new LabelControl();
        lueDocumentSeries = new LookUpEdit();
        lblSeriesValue = new LabelControl();
        lblSeriesCaption = new LabelControl();
        lblDocumentNumber = new LabelControl();
        lblTitle = new LabelControl();
        picLogo = new PictureEdit();
        ((System.ComponentModel.ISupportInitialize)pnlMain).BeginInit();
        pnlMain.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)slueSupplier.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvSupplierLookup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierTaxId.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContact.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierPhone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierEmail.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)deDocumentDate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)deDocumentDate.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)deDeliveryDate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)deDeliveryDate.Properties.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePaymentTerm.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePriceList.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueBuyer.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueMainWarehouse.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueProject.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)lueCostCenter.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memoComments.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)tabPurchaseOrder).BeginInit();
        tabPurchaseOrder.SuspendLayout();
        tabDetail.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridLines).BeginInit();
        ((System.ComponentModel.ISupportInitialize)viewLines).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gvItemRepository).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoUnit).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoQuantity).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoMoney).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoTax).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoWarehouse).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoDeliveryDate).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoDeliveryDate.CalendarTimeProperties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoCostCenter).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoProject).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlDetailTotals).BeginInit();
        pnlDetailTotals.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)spnGlobalDiscountPercent.Properties).BeginInit();
        tabAddresses.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlDeliveryAddress).BeginInit();
        pnlDeliveryAddress.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueDeliveryAddress.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryAddressName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memoDeliveryStreet.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryReference.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryCity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryState.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryZipCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryPhone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlBillingAddress).BeginInit();
        pnlBillingAddress.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueBillingAddress.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingAddressName.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memoBillingStreet.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingReference.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingCity.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingState.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingZipCode.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingCountry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingPhone.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingEmail.Properties).BeginInit();
        tabApproval.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalAmountCard).BeginInit();
        pnlApprovalAmountCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalPolicyCard).BeginInit();
        pnlApprovalPolicyCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalLevelCard).BeginInit();
        pnlApprovalLevelCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalStatusCard).BeginInit();
        pnlApprovalStatusCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)txtApprovalAmount.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtApprovalPolicy.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtApprovalLevel.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtApprovalStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridApprovals).BeginInit();
        ((System.ComponentModel.ISupportInitialize)viewApprovals).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalComment).BeginInit();
        pnlApprovalComment.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memoApprovalObservation.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalFlow).BeginInit();
        pnlApprovalFlow.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridApprovalFlow).BeginInit();
        ((System.ComponentModel.ISupportInitialize)viewApprovalFlow).BeginInit();
        tabRelatedDocuments.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridRelatedDocuments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)viewRelatedDocuments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)repoRelatedDocumentAction).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlRelatedDocumentNotes).BeginInit();
        pnlRelatedDocumentNotes.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)memoRelatedDocumentNotes.Properties).BeginInit();
        tabSap.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSapSync).BeginInit();
        pnlSapSync.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSapDocument).BeginInit();
        pnlSapDocument.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSapMessages).BeginInit();
        pnlSapMessages.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)txtSapStatus.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapSyncDocEntry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapSyncDocNum.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapObjectType.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapSyncDate.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapUser.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastError.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapDocEntry.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapDocNum.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCurrency.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)txtSapTotal.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memoSapMessage.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)gridSapLogs).BeginInit();
        ((System.ComponentModel.ISupportInitialize)viewSapLogs).BeginInit();
        tabAttachments.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)gridAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)viewAttachments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentPreview).BeginInit();
        pnlAttachmentPreview.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picAttachmentPreview.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pnlSummary).BeginInit();
        pnlSummary.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlFooter).BeginInit();
        pnlFooter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pnlHeader).BeginInit();
        pnlHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)lueDocumentSeries.Properties).BeginInit();
        ((System.ComponentModel.ISupportInitialize)picLogo.Properties).BeginInit();
        SuspendLayout();
        // 
        // pnlMain
        // 
        pnlMain.Appearance.BackColor = Color.White;
        pnlMain.Appearance.Options.UseBackColor = true;
        pnlMain.BorderStyle = BorderStyles.NoBorder;
        pnlMain.Controls.Add(lblSupplier);
        pnlMain.Controls.Add(slueSupplier);
        pnlMain.Controls.Add(lblSupplierTaxId);
        pnlMain.Controls.Add(txtSupplierTaxId);
        pnlMain.Controls.Add(lblSupplierContact);
        pnlMain.Controls.Add(txtSupplierContact);
        pnlMain.Controls.Add(lblSupplierPhone);
        pnlMain.Controls.Add(txtSupplierPhone);
        pnlMain.Controls.Add(lblSupplierEmail);
        pnlMain.Controls.Add(txtSupplierEmail);
        pnlMain.Controls.Add(lblDocumentDate);
        pnlMain.Controls.Add(deDocumentDate);
        pnlMain.Controls.Add(lblDeliveryDate);
        pnlMain.Controls.Add(deDeliveryDate);
        pnlMain.Controls.Add(lblCurrency);
        pnlMain.Controls.Add(lueCurrency);
        pnlMain.Controls.Add(lblPaymentTerm);
        pnlMain.Controls.Add(luePaymentTerm);
        pnlMain.Controls.Add(lblPriceList);
        pnlMain.Controls.Add(luePriceList);
        pnlMain.Controls.Add(lblBuyer);
        pnlMain.Controls.Add(lueBuyer);
        pnlMain.Controls.Add(lblMainWarehouse);
        pnlMain.Controls.Add(lueMainWarehouse);
        pnlMain.Controls.Add(lblProject);
        pnlMain.Controls.Add(lueProject);
        pnlMain.Controls.Add(lblCostCenter);
        pnlMain.Controls.Add(lueCostCenter);
        pnlMain.Controls.Add(lblPurchaseType);
        pnlMain.Controls.Add(luePurchaseType);
        pnlMain.Controls.Add(lblComments);
        pnlMain.Controls.Add(memoComments);
        pnlMain.Controls.Add(tabPurchaseOrder);
        pnlMain.Controls.Add(pnlDetailTotals);
        pnlMain.Controls.Add(pnlSummary);
        pnlMain.Dock = DockStyle.Fill;
        pnlMain.Location = new Point(0, 112);
        pnlMain.Name = "pnlMain";
        pnlMain.Size = new Size(1600, 716);
        pnlMain.TabIndex = 1;
        // 
        // lblSupplier
        // 
        lblSupplier.Location = new Point(22, 26);
        lblSupplier.Name = "lblSupplier";
        lblSupplier.Size = new Size(54, 13);
        lblSupplier.TabIndex = 0;
        lblSupplier.Text = "Proveedor:";
        // 
        // slueSupplier
        // 
        slueSupplier.Location = new Point(96, 22);
        slueSupplier.Name = "slueSupplier";
        slueSupplier.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        slueSupplier.Properties.DisplayMember = "DisplayText";
        slueSupplier.Properties.PopupView = gvSupplierLookup;
        slueSupplier.Properties.ValueMember = "Id";
        slueSupplier.Size = new Size(292, 22);
        slueSupplier.TabIndex = 1;
        // 
        // gvSupplierLookup
        // 
        gvSupplierLookup.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvSupplierLookup.Appearance.HeaderPanel.Options.UseFont = true;
        gvSupplierLookup.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvSupplierLookup.Appearance.Row.Options.UseFont = true;
        gvSupplierLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvSupplierLookup.Name = "gvSupplierLookup";
        gvSupplierLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvSupplierLookup.OptionsView.ShowGroupPanel = false;
        // 
        // lblSupplierTaxId
        // 
        lblSupplierTaxId.Location = new Point(22, 52);
        lblSupplierTaxId.Name = "lblSupplierTaxId";
        lblSupplierTaxId.Size = new Size(52, 13);
        lblSupplierTaxId.TabIndex = 2;
        lblSupplierTaxId.Text = "RUC / NIT:";
        // 
        // txtSupplierTaxId
        // 
        txtSupplierTaxId.Location = new Point(96, 48);
        txtSupplierTaxId.Name = "txtSupplierTaxId";
        txtSupplierTaxId.Size = new Size(292, 22);
        txtSupplierTaxId.TabIndex = 3;
        // 
        // lblSupplierContact
        // 
        lblSupplierContact.Location = new Point(22, 78);
        lblSupplierContact.Name = "lblSupplierContact";
        lblSupplierContact.Size = new Size(48, 13);
        lblSupplierContact.TabIndex = 4;
        lblSupplierContact.Text = "Contacto:";
        // 
        // txtSupplierContact
        // 
        txtSupplierContact.Location = new Point(96, 74);
        txtSupplierContact.Name = "txtSupplierContact";
        txtSupplierContact.Size = new Size(292, 22);
        txtSupplierContact.TabIndex = 5;
        // 
        // lblSupplierPhone
        // 
        lblSupplierPhone.Location = new Point(22, 104);
        lblSupplierPhone.Name = "lblSupplierPhone";
        lblSupplierPhone.Size = new Size(46, 13);
        lblSupplierPhone.TabIndex = 6;
        lblSupplierPhone.Text = "Teléfono:";
        // 
        // txtSupplierPhone
        // 
        txtSupplierPhone.Location = new Point(96, 100);
        txtSupplierPhone.Name = "txtSupplierPhone";
        txtSupplierPhone.Size = new Size(292, 22);
        txtSupplierPhone.TabIndex = 7;
        // 
        // lblSupplierEmail
        // 
        lblSupplierEmail.Location = new Point(22, 130);
        lblSupplierEmail.Name = "lblSupplierEmail";
        lblSupplierEmail.Size = new Size(28, 13);
        lblSupplierEmail.TabIndex = 8;
        lblSupplierEmail.Text = "Email:";
        // 
        // txtSupplierEmail
        // 
        txtSupplierEmail.Location = new Point(96, 126);
        txtSupplierEmail.Name = "txtSupplierEmail";
        txtSupplierEmail.Size = new Size(292, 22);
        txtSupplierEmail.TabIndex = 9;
        // 
        // lblDocumentDate
        // 
        lblDocumentDate.Location = new Point(424, 26);
        lblDocumentDate.Name = "lblDocumentDate";
        lblDocumentDate.Size = new Size(90, 13);
        lblDocumentDate.TabIndex = 10;
        lblDocumentDate.Text = "Fecha Documento:";
        // 
        // deDocumentDate
        // 
        deDocumentDate.EditValue = new DateTime(2026, 6, 2, 0, 0, 0, 0);
        deDocumentDate.Location = new Point(580, 22);
        deDocumentDate.Name = "deDocumentDate";
        deDocumentDate.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        deDocumentDate.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        deDocumentDate.Size = new Size(170, 22);
        deDocumentDate.TabIndex = 11;
        // 
        // lblDeliveryDate
        // 
        lblDeliveryDate.Location = new Point(424, 52);
        lblDeliveryDate.Name = "lblDeliveryDate";
        lblDeliveryDate.Size = new Size(120, 13);
        lblDeliveryDate.TabIndex = 12;
        lblDeliveryDate.Text = "Fecha Entrega Estimada:";
        // 
        // deDeliveryDate
        // 
        deDeliveryDate.EditValue = new DateTime(2026, 6, 2, 0, 0, 0, 0);
        deDeliveryDate.Location = new Point(580, 48);
        deDeliveryDate.Name = "deDeliveryDate";
        deDeliveryDate.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        deDeliveryDate.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        deDeliveryDate.Size = new Size(170, 22);
        deDeliveryDate.TabIndex = 13;
        // 
        // lblCurrency
        // 
        lblCurrency.Location = new Point(424, 78);
        lblCurrency.Name = "lblCurrency";
        lblCurrency.Size = new Size(42, 13);
        lblCurrency.TabIndex = 14;
        lblCurrency.Text = "Moneda:";
        // 
        // lueCurrency
        // 
        lueCurrency.Location = new Point(580, 74);
        lueCurrency.Name = "lueCurrency";
        lueCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCurrency.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        lueCurrency.Properties.DisplayMember = "DisplayText";
        lueCurrency.Properties.ValueMember = "Id";
        lueCurrency.Size = new Size(170, 22);
        lueCurrency.TabIndex = 15;
        // 
        // lblPaymentTerm
        // 
        lblPaymentTerm.Location = new Point(424, 104);
        lblPaymentTerm.Name = "lblPaymentTerm";
        lblPaymentTerm.Size = new Size(92, 13);
        lblPaymentTerm.TabIndex = 16;
        lblPaymentTerm.Text = "Condición de Pago:";
        // 
        // luePaymentTerm
        // 
        luePaymentTerm.Location = new Point(580, 100);
        luePaymentTerm.Name = "luePaymentTerm";
        luePaymentTerm.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePaymentTerm.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        luePaymentTerm.Properties.DisplayMember = "DisplayText";
        luePaymentTerm.Properties.ValueMember = "Id";
        luePaymentTerm.Size = new Size(170, 22);
        luePaymentTerm.TabIndex = 17;
        // 
        // lblPriceList
        // 
        lblPriceList.Location = new Point(424, 130);
        lblPriceList.Name = "lblPriceList";
        lblPriceList.Size = new Size(78, 13);
        lblPriceList.TabIndex = 18;
        lblPriceList.Text = "Lista de Precios:";
        // 
        // luePriceList
        // 
        luePriceList.Location = new Point(580, 126);
        luePriceList.Name = "luePriceList";
        luePriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePriceList.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        luePriceList.Properties.DisplayMember = "DisplayText";
        luePriceList.Properties.ValueMember = "Id";
        luePriceList.Size = new Size(170, 22);
        luePriceList.TabIndex = 19;
        // 
        // lblBuyer
        // 
        lblBuyer.Location = new Point(788, 26);
        lblBuyer.Name = "lblBuyer";
        lblBuyer.Size = new Size(57, 13);
        lblBuyer.TabIndex = 20;
        lblBuyer.Text = "Comprador:";
        // 
        // lueBuyer
        // 
        lueBuyer.Location = new Point(910, 22);
        lueBuyer.Name = "lueBuyer";
        lueBuyer.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBuyer.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        lueBuyer.Properties.DisplayMember = "DisplayText";
        lueBuyer.Properties.ValueMember = "Id";
        lueBuyer.Size = new Size(270, 22);
        lueBuyer.TabIndex = 21;
        // 
        // lblMainWarehouse
        // 
        lblMainWarehouse.Location = new Point(788, 52);
        lblMainWarehouse.Name = "lblMainWarehouse";
        lblMainWarehouse.Size = new Size(82, 13);
        lblMainWarehouse.TabIndex = 22;
        lblMainWarehouse.Text = "Bodega Principal:";
        // 
        // lueMainWarehouse
        // 
        lueMainWarehouse.Location = new Point(910, 48);
        lueMainWarehouse.Name = "lueMainWarehouse";
        lueMainWarehouse.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueMainWarehouse.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        lueMainWarehouse.Properties.DisplayMember = "DisplayText";
        lueMainWarehouse.Properties.ValueMember = "Id";
        lueMainWarehouse.Size = new Size(270, 22);
        lueMainWarehouse.TabIndex = 23;
        // 
        // lblProject
        // 
        lblProject.Location = new Point(788, 78);
        lblProject.Name = "lblProject";
        lblProject.Size = new Size(47, 13);
        lblProject.TabIndex = 24;
        lblProject.Text = "Proyecto:";
        // 
        // lueProject
        // 
        lueProject.Location = new Point(910, 74);
        lueProject.Name = "lueProject";
        lueProject.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueProject.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        lueProject.Properties.DisplayMember = "DisplayText";
        lueProject.Properties.ValueMember = "Id";
        lueProject.Size = new Size(270, 22);
        lueProject.TabIndex = 25;
        // 
        // lblCostCenter
        // 
        lblCostCenter.Location = new Point(788, 104);
        lblCostCenter.Name = "lblCostCenter";
        lblCostCenter.Size = new Size(83, 13);
        lblCostCenter.TabIndex = 26;
        lblCostCenter.Text = "Centro de Costo:";
        // 
        // lueCostCenter
        // 
        lueCostCenter.Location = new Point(910, 100);
        lueCostCenter.Name = "lueCostCenter";
        lueCostCenter.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueCostCenter.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        lueCostCenter.Properties.DisplayMember = "DisplayText";
        lueCostCenter.Properties.ValueMember = "Id";
        lueCostCenter.Size = new Size(270, 22);
        lueCostCenter.TabIndex = 27;
        // 
        // lblPurchaseType
        // 
        lblPurchaseType.Location = new Point(788, 130);
        lblPurchaseType.Name = "lblPurchaseType";
        lblPurchaseType.Size = new Size(79, 13);
        lblPurchaseType.TabIndex = 28;
        lblPurchaseType.Text = "Tipo de Compra:";
        // 
        // luePurchaseType
        // 
        luePurchaseType.Location = new Point(910, 126);
        luePurchaseType.Name = "luePurchaseType";
        luePurchaseType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        luePurchaseType.Properties.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        luePurchaseType.Properties.DisplayMember = "DisplayText";
        luePurchaseType.Properties.ValueMember = "Id";
        luePurchaseType.Size = new Size(270, 22);
        luePurchaseType.TabIndex = 29;
        // 
        // lblComments
        // 
        lblComments.Location = new Point(1230, 26);
        lblComments.Name = "lblComments";
        lblComments.Size = new Size(145, 13);
        lblComments.TabIndex = 30;
        lblComments.Text = "Comentarios / Observaciones:";
        // 
        // memoComments
        // 
        memoComments.Location = new Point(1230, 58);
        memoComments.Name = "memoComments";
        memoComments.Size = new Size(338, 134);
        memoComments.TabIndex = 31;
        // 
        // tabPurchaseOrder
        // 
        tabPurchaseOrder.Location = new Point(16, 214);
        tabPurchaseOrder.Name = "tabPurchaseOrder";
        tabPurchaseOrder.SelectedTabPage = tabRelatedDocuments;
        tabPurchaseOrder.Size = new Size(1328, 430);
        tabPurchaseOrder.TabIndex = 32;
        tabPurchaseOrder.TabPages.AddRange(new XtraTabPage[] { tabDetail, tabAddresses, tabApproval, tabRelatedDocuments, tabSap, tabAttachments });
        // 
        // tabDetail
        // 
        tabDetail.Controls.Add(lblDetailHint);
        tabDetail.Controls.Add(gridLines);
        tabDetail.Name = "tabDetail";
        tabDetail.Size = new Size(1326, 389);
        tabDetail.Text = "Detalle";
        // 
        // lblDetailHint
        // 
        lblDetailHint.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblDetailHint.Appearance.Options.UseForeColor = true;
        lblDetailHint.Location = new Point(16, 18);
        lblDetailHint.Name = "lblDetailHint";
        lblDetailHint.Size = new Size(493, 13);
        lblDetailHint.TabIndex = 0;
        lblDetailHint.Text = "Ingrese las líneas directamente en la cuadrícula. Agregue una nueva línea al final y comience a escribir.";
        // 
        // gridLines
        // 
        gridLines.Location = new Point(8, 58);
        gridLines.MainView = viewLines;
        gridLines.Name = "gridLines";
        gridLines.RepositoryItems.AddRange(new RepositoryItem[] { repoItem, repoUnit, repoTax, repoWarehouse, repoCostCenter, repoProject, repoDeliveryDate, repoQuantity, repoMoney });
        gridLines.Size = new Size(1310, 318);
        gridLines.TabIndex = 1;
        gridLines.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { viewLines });
        // 
        // viewLines
        // 
        viewLines.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        viewLines.Appearance.HeaderPanel.Options.UseFont = true;
        viewLines.Appearance.Row.Font = new Font("Segoe UI", 9F);
        viewLines.Appearance.Row.Options.UseFont = true;
        viewLines.Columns.AddRange(new GridColumn[] { colLineNumber, colLineItemCode, colLineDescription, colLineUnit, colLineQuantity, colLineOpenQuantity, colLineUnitPrice, colLineDiscountPercent, colLineTax, colLineWarehouse, colLineDeliveryDate, colLineCostCenter, colLineProject, colLineTotal });
        viewLines.GridControl = gridLines;
        viewLines.Name = "viewLines";
        viewLines.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
        viewLines.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
        viewLines.OptionsNavigation.AutoFocusNewRow = true;
        viewLines.OptionsView.NewItemRowPosition = NewItemRowPosition.Bottom;
        viewLines.OptionsView.ShowGroupPanel = false;
        // 
        // colLineNumber
        // 
        colLineNumber.Caption = "#";
        colLineNumber.FieldName = "LineNumber";
        colLineNumber.Name = "colLineNumber";
        colLineNumber.OptionsColumn.AllowEdit = false;
        colLineNumber.Visible = true;
        colLineNumber.VisibleIndex = 0;
        colLineNumber.Width = 40;
        // 
        // colLineItemCode
        // 
        colLineItemCode.Caption = "Código";
        colLineItemCode.ColumnEdit = repoItem;
        colLineItemCode.FieldName = "ItemId";
        colLineItemCode.Name = "colLineItemCode";
        colLineItemCode.Visible = true;
        colLineItemCode.VisibleIndex = 1;
        colLineItemCode.Width = 90;
        // 
        // repoItem
        // 
        repoItem.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        repoItem.DisplayMember = "DisplayText";
        repoItem.Name = "repoItem";
        repoItem.PopupView = gvItemRepository;
        repoItem.ValueMember = "Id";
        // 
        // gvItemRepository
        // 
        gvItemRepository.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        gvItemRepository.Appearance.HeaderPanel.Options.UseFont = true;
        gvItemRepository.Appearance.Row.Font = new Font("Segoe UI", 9F);
        gvItemRepository.Appearance.Row.Options.UseFont = true;
        gvItemRepository.FocusRectStyle = DrawFocusRectStyle.RowFocus;
        gvItemRepository.Name = "gvItemRepository";
        gvItemRepository.OptionsSelection.EnableAppearanceFocusedCell = false;
        gvItemRepository.OptionsView.ShowGroupPanel = false;
        // 
        // colLineDescription
        // 
        colLineDescription.Caption = "Descripción";
        colLineDescription.FieldName = "ItemName";
        colLineDescription.Name = "colLineDescription";
        colLineDescription.Visible = true;
        colLineDescription.VisibleIndex = 2;
        colLineDescription.Width = 230;
        // 
        // colLineUnit
        // 
        colLineUnit.Caption = "Unidad";
        colLineUnit.ColumnEdit = repoUnit;
        colLineUnit.FieldName = "UnitId";
        colLineUnit.Name = "colLineUnit";
        colLineUnit.Visible = true;
        colLineUnit.VisibleIndex = 3;
        // 
        // repoUnit
        // 
        repoUnit.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        repoUnit.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        repoUnit.DisplayMember = "DisplayText";
        repoUnit.Name = "repoUnit";
        repoUnit.ValueMember = "Id";
        // 
        // colLineQuantity
        // 
        colLineQuantity.Caption = "Cantidad";
        colLineQuantity.ColumnEdit = repoQuantity;
        colLineQuantity.FieldName = "Quantity";
        colLineQuantity.Name = "colLineQuantity";
        colLineQuantity.Visible = true;
        colLineQuantity.VisibleIndex = 4;
        colLineQuantity.Width = 80;
        // 
        // repoQuantity
        // 
        repoQuantity.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        repoQuantity.Name = "repoQuantity";
        // 
        // colLineOpenQuantity
        // 
        colLineOpenQuantity.Caption = "Pendiente";
        colLineOpenQuantity.FieldName = "OpenQuantity";
        colLineOpenQuantity.Name = "colLineOpenQuantity";
        colLineOpenQuantity.OptionsColumn.AllowEdit = false;
        colLineOpenQuantity.Visible = true;
        colLineOpenQuantity.VisibleIndex = 5;
        colLineOpenQuantity.Width = 80;
        // 
        // colLineUnitPrice
        // 
        colLineUnitPrice.Caption = "Precio Unit.";
        colLineUnitPrice.ColumnEdit = repoMoney;
        colLineUnitPrice.FieldName = "UnitPrice";
        colLineUnitPrice.Name = "colLineUnitPrice";
        colLineUnitPrice.Visible = true;
        colLineUnitPrice.VisibleIndex = 6;
        colLineUnitPrice.Width = 85;
        // 
        // repoMoney
        // 
        repoMoney.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        repoMoney.Name = "repoMoney";
        // 
        // colLineDiscountPercent
        // 
        colLineDiscountPercent.Caption = "Desc %";
        colLineDiscountPercent.ColumnEdit = repoQuantity;
        colLineDiscountPercent.FieldName = "DiscountPercent";
        colLineDiscountPercent.Name = "colLineDiscountPercent";
        colLineDiscountPercent.Visible = true;
        colLineDiscountPercent.VisibleIndex = 7;
        colLineDiscountPercent.Width = 65;
        // 
        // colLineTax
        // 
        colLineTax.Caption = "Impuesto";
        colLineTax.ColumnEdit = repoTax;
        colLineTax.FieldName = "TaxId";
        colLineTax.Name = "colLineTax";
        colLineTax.Visible = true;
        colLineTax.VisibleIndex = 8;
        colLineTax.Width = 85;
        // 
        // repoTax
        // 
        repoTax.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        repoTax.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        repoTax.DisplayMember = "DisplayText";
        repoTax.Name = "repoTax";
        repoTax.ValueMember = "Id";
        // 
        // colLineWarehouse
        // 
        colLineWarehouse.Caption = "Bodega";
        colLineWarehouse.ColumnEdit = repoWarehouse;
        colLineWarehouse.FieldName = "WarehouseId";
        colLineWarehouse.Name = "colLineWarehouse";
        colLineWarehouse.Visible = true;
        colLineWarehouse.VisibleIndex = 9;
        colLineWarehouse.Width = 145;
        // 
        // repoWarehouse
        // 
        repoWarehouse.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        repoWarehouse.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        repoWarehouse.DisplayMember = "DisplayText";
        repoWarehouse.Name = "repoWarehouse";
        repoWarehouse.ValueMember = "Id";
        // 
        // colLineDeliveryDate
        // 
        colLineDeliveryDate.Caption = "Fecha Entrega";
        colLineDeliveryDate.ColumnEdit = repoDeliveryDate;
        colLineDeliveryDate.FieldName = "DeliveryDate";
        colLineDeliveryDate.Name = "colLineDeliveryDate";
        colLineDeliveryDate.Visible = true;
        colLineDeliveryDate.VisibleIndex = 10;
        colLineDeliveryDate.Width = 120;
        // 
        // repoDeliveryDate
        // 
        repoDeliveryDate.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        repoDeliveryDate.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        repoDeliveryDate.Name = "repoDeliveryDate";
        // 
        // colLineCostCenter
        // 
        colLineCostCenter.Caption = "Centro Costo";
        colLineCostCenter.ColumnEdit = repoCostCenter;
        colLineCostCenter.FieldName = "CostCenterId";
        colLineCostCenter.Name = "colLineCostCenter";
        colLineCostCenter.Visible = true;
        colLineCostCenter.VisibleIndex = 11;
        colLineCostCenter.Width = 95;
        // 
        // repoCostCenter
        // 
        repoCostCenter.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        repoCostCenter.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        repoCostCenter.DisplayMember = "DisplayText";
        repoCostCenter.Name = "repoCostCenter";
        repoCostCenter.ValueMember = "Id";
        // 
        // colLineProject
        // 
        colLineProject.Caption = "Proyecto";
        colLineProject.ColumnEdit = repoProject;
        colLineProject.FieldName = "ProjectId";
        colLineProject.Name = "colLineProject";
        colLineProject.Visible = true;
        colLineProject.VisibleIndex = 12;
        colLineProject.Width = 95;
        // 
        // repoProject
        // 
        repoProject.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        repoProject.Columns.AddRange(new LookUpColumnInfo[] { new LookUpColumnInfo("Code", "Código", 80, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default), new LookUpColumnInfo("Name", "Nombre", 180, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default) });
        repoProject.DisplayMember = "DisplayText";
        repoProject.Name = "repoProject";
        repoProject.ValueMember = "Id";
        // 
        // colLineTotal
        // 
        colLineTotal.Caption = "Total Línea";
        colLineTotal.FieldName = "LineTotal";
        colLineTotal.Name = "colLineTotal";
        colLineTotal.OptionsColumn.AllowEdit = false;
        colLineTotal.Visible = true;
        colLineTotal.VisibleIndex = 13;
        colLineTotal.Width = 105;
        // 
        // pnlDetailTotals
        // 
        pnlDetailTotals.BorderStyle = BorderStyles.NoBorder;
        pnlDetailTotals.Controls.Add(lblDetailSubtotalCaption);
        pnlDetailTotals.Controls.Add(lblDetailSubtotal);
        pnlDetailTotals.Controls.Add(lblGlobalDiscountPercent);
        pnlDetailTotals.Controls.Add(spnGlobalDiscountPercent);
        pnlDetailTotals.Controls.Add(lblDetailDiscountCaption);
        pnlDetailTotals.Controls.Add(lblDetailDiscount);
        pnlDetailTotals.Controls.Add(lblDetailBaseCaption);
        pnlDetailTotals.Controls.Add(lblDetailBase);
        pnlDetailTotals.Controls.Add(lblDetailTaxCaption);
        pnlDetailTotals.Controls.Add(lblDetailTax);
        pnlDetailTotals.Controls.Add(lblDetailTotalCaption);
        pnlDetailTotals.Controls.Add(lblDetailTotal);
        pnlDetailTotals.Location = new Point(16, 640);
        pnlDetailTotals.Name = "pnlDetailTotals";
        pnlDetailTotals.Size = new Size(1328, 64);
        pnlDetailTotals.TabIndex = 2;
        // 
        // lblDetailSubtotalCaption
        // 
        lblDetailSubtotalCaption.Location = new Point(18, 14);
        lblDetailSubtotalCaption.Name = "lblDetailSubtotalCaption";
        lblDetailSubtotalCaption.Size = new Size(44, 13);
        lblDetailSubtotalCaption.TabIndex = 0;
        lblDetailSubtotalCaption.Text = "Subtotal:";
        // 
        // lblDetailSubtotal
        // 
        lblDetailSubtotal.Appearance.Options.UseTextOptions = true;
        lblDetailSubtotal.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblDetailSubtotal.AutoSizeMode = LabelAutoSizeMode.None;
        lblDetailSubtotal.BorderStyle = BorderStyles.Simple;
        lblDetailSubtotal.Location = new Point(94, 30);
        lblDetailSubtotal.Name = "lblDetailSubtotal";
        lblDetailSubtotal.Size = new Size(138, 28);
        lblDetailSubtotal.TabIndex = 1;
        lblDetailSubtotal.Text = "0.00";
        // 
        // lblGlobalDiscountPercent
        // 
        lblGlobalDiscountPercent.Location = new Point(280, 14);
        lblGlobalDiscountPercent.Name = "lblGlobalDiscountPercent";
        lblGlobalDiscountPercent.Size = new Size(109, 13);
        lblGlobalDiscountPercent.TabIndex = 2;
        lblGlobalDiscountPercent.Text = "Descuento Global (%):";
        // 
        // spnGlobalDiscountPercent
        // 
        spnGlobalDiscountPercent.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
        spnGlobalDiscountPercent.Location = new Point(405, 30);
        spnGlobalDiscountPercent.Name = "spnGlobalDiscountPercent";
        spnGlobalDiscountPercent.Properties.MaxValue = new decimal(new int[] { 100, 0, 0, 0 });
        spnGlobalDiscountPercent.Size = new Size(72, 22);
        spnGlobalDiscountPercent.TabIndex = 3;
        // 
        // lblDetailDiscountCaption
        // 
        lblDetailDiscountCaption.Location = new Point(530, 14);
        lblDetailDiscountCaption.Name = "lblDetailDiscountCaption";
        lblDetailDiscountCaption.Size = new Size(87, 13);
        lblDetailDiscountCaption.TabIndex = 4;
        lblDetailDiscountCaption.Text = "Descuento Global:";
        // 
        // lblDetailDiscount
        // 
        lblDetailDiscount.Appearance.Options.UseTextOptions = true;
        lblDetailDiscount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblDetailDiscount.AutoSizeMode = LabelAutoSizeMode.None;
        lblDetailDiscount.BorderStyle = BorderStyles.Simple;
        lblDetailDiscount.Location = new Point(640, 30);
        lblDetailDiscount.Name = "lblDetailDiscount";
        lblDetailDiscount.Size = new Size(150, 28);
        lblDetailDiscount.TabIndex = 5;
        lblDetailDiscount.Text = "0.00";
        // 
        // lblDetailBaseCaption
        // 
        lblDetailBaseCaption.Location = new Point(840, 14);
        lblDetailBaseCaption.Name = "lblDetailBaseCaption";
        lblDetailBaseCaption.Size = new Size(76, 13);
        lblDetailBaseCaption.TabIndex = 6;
        lblDetailBaseCaption.Text = "Base Imponible:";
        // 
        // lblDetailBase
        // 
        lblDetailBase.Appearance.Options.UseTextOptions = true;
        lblDetailBase.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblDetailBase.AutoSizeMode = LabelAutoSizeMode.None;
        lblDetailBase.BorderStyle = BorderStyles.Simple;
        lblDetailBase.Location = new Point(910, 30);
        lblDetailBase.Name = "lblDetailBase";
        lblDetailBase.Size = new Size(130, 28);
        lblDetailBase.TabIndex = 7;
        lblDetailBase.Text = "0.00";
        // 
        // lblDetailTaxCaption
        // 
        lblDetailTaxCaption.Location = new Point(1080, 14);
        lblDetailTaxCaption.Name = "lblDetailTaxCaption";
        lblDetailTaxCaption.Size = new Size(55, 13);
        lblDetailTaxCaption.TabIndex = 8;
        lblDetailTaxCaption.Text = "IVA (12%):";
        // 
        // lblDetailTax
        // 
        lblDetailTax.Appearance.Options.UseTextOptions = true;
        lblDetailTax.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblDetailTax.AutoSizeMode = LabelAutoSizeMode.None;
        lblDetailTax.BorderStyle = BorderStyles.Simple;
        lblDetailTax.Location = new Point(1125, 30);
        lblDetailTax.Name = "lblDetailTax";
        lblDetailTax.Size = new Size(92, 28);
        lblDetailTax.TabIndex = 9;
        lblDetailTax.Text = "0.00";
        // 
        // lblDetailTotalCaption
        // 
        lblDetailTotalCaption.Location = new Point(1230, 14);
        lblDetailTotalCaption.Name = "lblDetailTotalCaption";
        lblDetailTotalCaption.Size = new Size(28, 13);
        lblDetailTotalCaption.TabIndex = 10;
        lblDetailTotalCaption.Text = "Total:";
        // 
        // lblDetailTotal
        // 
        lblDetailTotal.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblDetailTotal.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblDetailTotal.Appearance.Options.UseFont = true;
        lblDetailTotal.Appearance.Options.UseForeColor = true;
        lblDetailTotal.Appearance.Options.UseTextOptions = true;
        lblDetailTotal.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblDetailTotal.AutoSizeMode = LabelAutoSizeMode.None;
        lblDetailTotal.BorderStyle = BorderStyles.Simple;
        lblDetailTotal.Location = new Point(1230, 30);
        lblDetailTotal.Name = "lblDetailTotal";
        lblDetailTotal.Size = new Size(90, 28);
        lblDetailTotal.TabIndex = 11;
        lblDetailTotal.Text = "0.00";
        // 
        // tabAddresses
        // 
        tabAddresses.Controls.Add(pnlDeliveryAddress);
        tabAddresses.Controls.Add(pnlBillingAddress);
        tabAddresses.Name = "tabAddresses";
        tabAddresses.Size = new Size(1326, 389);
        tabAddresses.Text = "Direcciones";
        // 
        // pnlDeliveryAddress
        // 
        pnlDeliveryAddress.BorderStyle = BorderStyles.Simple;
        pnlDeliveryAddress.Controls.Add(lblDeliveryAddressTitle);
        pnlDeliveryAddress.Controls.Add(lblDeliveryAddressSelector);
        pnlDeliveryAddress.Controls.Add(lueDeliveryAddress);
        pnlDeliveryAddress.Controls.Add(btnDeliveryAddressLookup);
        pnlDeliveryAddress.Controls.Add(lblDeliveryNameCaption);
        pnlDeliveryAddress.Controls.Add(txtDeliveryAddressName);
        pnlDeliveryAddress.Controls.Add(lblDeliveryStreetCaption);
        pnlDeliveryAddress.Controls.Add(memoDeliveryStreet);
        pnlDeliveryAddress.Controls.Add(lblDeliveryReferenceCaption);
        pnlDeliveryAddress.Controls.Add(txtDeliveryReference);
        pnlDeliveryAddress.Controls.Add(lblDeliveryCityCaption);
        pnlDeliveryAddress.Controls.Add(txtDeliveryCity);
        pnlDeliveryAddress.Controls.Add(lblDeliveryStateCaption);
        pnlDeliveryAddress.Controls.Add(txtDeliveryState);
        pnlDeliveryAddress.Controls.Add(lblDeliveryZipCodeCaption);
        pnlDeliveryAddress.Controls.Add(txtDeliveryZipCode);
        pnlDeliveryAddress.Controls.Add(lblDeliveryCountryCaption);
        pnlDeliveryAddress.Controls.Add(txtDeliveryCountry);
        pnlDeliveryAddress.Controls.Add(lblDeliveryPhoneCaption);
        pnlDeliveryAddress.Controls.Add(txtDeliveryPhone);
        pnlDeliveryAddress.Controls.Add(lblDeliveryInfo);
        pnlDeliveryAddress.Location = new Point(16, 28);
        pnlDeliveryAddress.Name = "pnlDeliveryAddress";
        pnlDeliveryAddress.Size = new Size(640, 360);
        pnlDeliveryAddress.TabIndex = 0;
        // 
        // lblDeliveryAddressTitle
        // 
        lblDeliveryAddressTitle.Appearance.Font = AppTypography.SectionFont;
        lblDeliveryAddressTitle.Appearance.ForeColor = BrandResources.Primary;
        lblDeliveryAddressTitle.Appearance.Options.UseFont = true;
        lblDeliveryAddressTitle.Appearance.Options.UseForeColor = true;
        lblDeliveryAddressTitle.Location = new Point(20, 12);
        lblDeliveryAddressTitle.Name = "lblDeliveryAddressTitle";
        lblDeliveryAddressTitle.Size = new Size(139, 20);
        lblDeliveryAddressTitle.TabIndex = 0;
        lblDeliveryAddressTitle.Text = "Dirección de Entrega";
        // 
        // lblDeliveryAddressSelector
        // 
        lblDeliveryAddressSelector.Location = new Point(20, 44);
        lblDeliveryAddressSelector.Name = "lblDeliveryAddressSelector";
        lblDeliveryAddressSelector.Size = new Size(99, 13);
        lblDeliveryAddressSelector.TabIndex = 0;
        lblDeliveryAddressSelector.Text = "Seleccione dirección:";
        // 
        // lueDeliveryAddress
        // 
        lueDeliveryAddress.Location = new Point(170, 40);
        lueDeliveryAddress.Name = "lueDeliveryAddress";
        lueDeliveryAddress.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueDeliveryAddress.Properties.DisplayMember = "DisplayText";
        lueDeliveryAddress.Properties.NullText = "Bodega Central - Av. de los Industriales 1234";
        lueDeliveryAddress.Properties.ValueMember = "Id";
        lueDeliveryAddress.Size = new Size(430, 22);
        lueDeliveryAddress.TabIndex = 1;
        // 
        // btnDeliveryAddressLookup
        // 
        btnDeliveryAddressLookup.Location = new Point(606, 39);
        btnDeliveryAddressLookup.Name = "btnDeliveryAddressLookup";
        btnDeliveryAddressLookup.Size = new Size(30, 24);
        btnDeliveryAddressLookup.TabIndex = 2;
        btnDeliveryAddressLookup.Text = "...";
        // 
        // lblDeliveryNameCaption
        // 
        lblDeliveryNameCaption.Appearance.Font = AppTypography.LabelFont;
        lblDeliveryNameCaption.Appearance.Options.UseFont = true;
        lblDeliveryNameCaption.Location = new Point(44, 88);
        lblDeliveryNameCaption.Name = "lblDeliveryNameCaption";
        lblDeliveryNameCaption.Size = new Size(43, 13);
        lblDeliveryNameCaption.TabIndex = 3;
        lblDeliveryNameCaption.Text = "Nombre:";
        // 
        // txtDeliveryAddressName
        // 
        txtDeliveryAddressName.Location = new Point(160, 84);
        txtDeliveryAddressName.Name = "txtDeliveryAddressName";
        txtDeliveryAddressName.Properties.BorderStyle = BorderStyles.NoBorder;
        txtDeliveryAddressName.Properties.ReadOnly = true;
        txtDeliveryAddressName.Size = new Size(440, 22);
        txtDeliveryAddressName.TabIndex = 4;
        // 
        // lblDeliveryStreetCaption
        // 
        lblDeliveryStreetCaption.Appearance.Font = AppTypography.LabelFont;
        lblDeliveryStreetCaption.Appearance.Options.UseFont = true;
        lblDeliveryStreetCaption.Location = new Point(44, 114);
        lblDeliveryStreetCaption.Name = "lblDeliveryStreetCaption";
        lblDeliveryStreetCaption.Size = new Size(51, 13);
        lblDeliveryStreetCaption.TabIndex = 5;
        lblDeliveryStreetCaption.Text = "Dirección:";
        // 
        // memoDeliveryStreet
        // 
        memoDeliveryStreet.Location = new Point(160, 110);
        memoDeliveryStreet.Name = "memoDeliveryStreet";
        memoDeliveryStreet.Properties.BorderStyle = BorderStyles.NoBorder;
        memoDeliveryStreet.Properties.ReadOnly = true;
        memoDeliveryStreet.Size = new Size(440, 22);
        memoDeliveryStreet.TabIndex = 6;
        // 
        // lblDeliveryReferenceCaption
        // 
        lblDeliveryReferenceCaption.Appearance.Font = AppTypography.LabelFont;
        lblDeliveryReferenceCaption.Appearance.Options.UseFont = true;
        lblDeliveryReferenceCaption.Location = new Point(44, 140);
        lblDeliveryReferenceCaption.Name = "lblDeliveryReferenceCaption";
        lblDeliveryReferenceCaption.Size = new Size(55, 13);
        lblDeliveryReferenceCaption.TabIndex = 7;
        lblDeliveryReferenceCaption.Text = "Referencia:";
        // 
        // txtDeliveryReference
        // 
        txtDeliveryReference.Location = new Point(160, 136);
        txtDeliveryReference.Name = "txtDeliveryReference";
        txtDeliveryReference.Properties.BorderStyle = BorderStyles.NoBorder;
        txtDeliveryReference.Properties.ReadOnly = true;
        txtDeliveryReference.Size = new Size(440, 22);
        txtDeliveryReference.TabIndex = 8;
        // 
        // lblDeliveryCityCaption
        // 
        lblDeliveryCityCaption.Appearance.Font = AppTypography.LabelFont;
        lblDeliveryCityCaption.Appearance.Options.UseFont = true;
        lblDeliveryCityCaption.Location = new Point(44, 166);
        lblDeliveryCityCaption.Name = "lblDeliveryCityCaption";
        lblDeliveryCityCaption.Size = new Size(36, 13);
        lblDeliveryCityCaption.TabIndex = 9;
        lblDeliveryCityCaption.Text = "Ciudad:";
        // 
        // txtDeliveryCity
        // 
        txtDeliveryCity.Location = new Point(160, 162);
        txtDeliveryCity.Name = "txtDeliveryCity";
        txtDeliveryCity.Properties.BorderStyle = BorderStyles.NoBorder;
        txtDeliveryCity.Properties.ReadOnly = true;
        txtDeliveryCity.Size = new Size(160, 22);
        txtDeliveryCity.TabIndex = 10;
        // 
        // lblDeliveryStateCaption
        // 
        lblDeliveryStateCaption.Appearance.Font = AppTypography.LabelFont;
        lblDeliveryStateCaption.Appearance.Options.UseFont = true;
        lblDeliveryStateCaption.Location = new Point(44, 192);
        lblDeliveryStateCaption.Name = "lblDeliveryStateCaption";
        lblDeliveryStateCaption.Size = new Size(49, 13);
        lblDeliveryStateCaption.TabIndex = 11;
        lblDeliveryStateCaption.Text = "Provincia:";
        // 
        // txtDeliveryState
        // 
        txtDeliveryState.Location = new Point(160, 188);
        txtDeliveryState.Name = "txtDeliveryState";
        txtDeliveryState.Properties.BorderStyle = BorderStyles.NoBorder;
        txtDeliveryState.Properties.ReadOnly = true;
        txtDeliveryState.Size = new Size(160, 22);
        txtDeliveryState.TabIndex = 12;
        // 
        // lblDeliveryZipCodeCaption
        // 
        lblDeliveryZipCodeCaption.Appearance.Font = AppTypography.LabelFont;
        lblDeliveryZipCodeCaption.Appearance.Options.UseFont = true;
        lblDeliveryZipCodeCaption.Location = new Point(44, 218);
        lblDeliveryZipCodeCaption.Name = "lblDeliveryZipCodeCaption";
        lblDeliveryZipCodeCaption.Size = new Size(73, 13);
        lblDeliveryZipCodeCaption.TabIndex = 13;
        lblDeliveryZipCodeCaption.Text = "Código Postal:";
        // 
        // txtDeliveryZipCode
        // 
        txtDeliveryZipCode.Location = new Point(160, 214);
        txtDeliveryZipCode.Name = "txtDeliveryZipCode";
        txtDeliveryZipCode.Properties.BorderStyle = BorderStyles.NoBorder;
        txtDeliveryZipCode.Properties.ReadOnly = true;
        txtDeliveryZipCode.Size = new Size(160, 22);
        txtDeliveryZipCode.TabIndex = 14;
        // 
        // lblDeliveryCountryCaption
        // 
        lblDeliveryCountryCaption.Appearance.Font = AppTypography.LabelFont;
        lblDeliveryCountryCaption.Appearance.Options.UseFont = true;
        lblDeliveryCountryCaption.Location = new Point(44, 244);
        lblDeliveryCountryCaption.Name = "lblDeliveryCountryCaption";
        lblDeliveryCountryCaption.Size = new Size(24, 13);
        lblDeliveryCountryCaption.TabIndex = 15;
        lblDeliveryCountryCaption.Text = "País:";
        // 
        // txtDeliveryCountry
        // 
        txtDeliveryCountry.Location = new Point(160, 240);
        txtDeliveryCountry.Name = "txtDeliveryCountry";
        txtDeliveryCountry.Properties.BorderStyle = BorderStyles.NoBorder;
        txtDeliveryCountry.Properties.ReadOnly = true;
        txtDeliveryCountry.Size = new Size(160, 22);
        txtDeliveryCountry.TabIndex = 16;
        // 
        // lblDeliveryPhoneCaption
        // 
        lblDeliveryPhoneCaption.Appearance.Font = AppTypography.LabelFont;
        lblDeliveryPhoneCaption.Appearance.Options.UseFont = true;
        lblDeliveryPhoneCaption.Location = new Point(44, 270);
        lblDeliveryPhoneCaption.Name = "lblDeliveryPhoneCaption";
        lblDeliveryPhoneCaption.Size = new Size(46, 13);
        lblDeliveryPhoneCaption.TabIndex = 17;
        lblDeliveryPhoneCaption.Text = "Teléfono:";
        // 
        // txtDeliveryPhone
        // 
        txtDeliveryPhone.Location = new Point(160, 266);
        txtDeliveryPhone.Name = "txtDeliveryPhone";
        txtDeliveryPhone.Properties.BorderStyle = BorderStyles.NoBorder;
        txtDeliveryPhone.Properties.ReadOnly = true;
        txtDeliveryPhone.Size = new Size(160, 22);
        txtDeliveryPhone.TabIndex = 18;
        // 
        // lblDeliveryInfo
        // 
        lblDeliveryInfo.Appearance.ForeColor = BrandResources.Primary;
        lblDeliveryInfo.Appearance.Options.UseForeColor = true;
        lblDeliveryInfo.Location = new Point(20, 316);
        lblDeliveryInfo.Name = "lblDeliveryInfo";
        lblDeliveryInfo.Size = new Size(413, 13);
        lblDeliveryInfo.TabIndex = 19;
        lblDeliveryInfo.Text = "ⓘ  Seleccione una dirección del proveedor o modifíquela según sea necesario.";
        // 
        // pnlBillingAddress
        // 
        pnlBillingAddress.BorderStyle = BorderStyles.Simple;
        pnlBillingAddress.Controls.Add(lblBillingAddressTitle);
        pnlBillingAddress.Controls.Add(lblBillingAddressSelector);
        pnlBillingAddress.Controls.Add(lueBillingAddress);
        pnlBillingAddress.Controls.Add(btnBillingAddressLookup);
        pnlBillingAddress.Controls.Add(lblBillingNameCaption);
        pnlBillingAddress.Controls.Add(txtBillingAddressName);
        pnlBillingAddress.Controls.Add(lblBillingStreetCaption);
        pnlBillingAddress.Controls.Add(memoBillingStreet);
        pnlBillingAddress.Controls.Add(lblBillingReferenceCaption);
        pnlBillingAddress.Controls.Add(txtBillingReference);
        pnlBillingAddress.Controls.Add(lblBillingCityCaption);
        pnlBillingAddress.Controls.Add(txtBillingCity);
        pnlBillingAddress.Controls.Add(lblBillingStateCaption);
        pnlBillingAddress.Controls.Add(txtBillingState);
        pnlBillingAddress.Controls.Add(lblBillingZipCodeCaption);
        pnlBillingAddress.Controls.Add(txtBillingZipCode);
        pnlBillingAddress.Controls.Add(lblBillingCountryCaption);
        pnlBillingAddress.Controls.Add(txtBillingCountry);
        pnlBillingAddress.Controls.Add(lblBillingPhoneCaption);
        pnlBillingAddress.Controls.Add(txtBillingPhone);
        pnlBillingAddress.Controls.Add(lblBillingEmailCaption);
        pnlBillingAddress.Controls.Add(txtBillingEmail);
        pnlBillingAddress.Controls.Add(lblBillingInfo);
        pnlBillingAddress.Location = new Point(690, 28);
        pnlBillingAddress.Name = "pnlBillingAddress";
        pnlBillingAddress.Size = new Size(640, 360);
        pnlBillingAddress.TabIndex = 1;
        // 
        // lblBillingAddressTitle
        // 
        lblBillingAddressTitle.Appearance.Font = AppTypography.SectionFont;
        lblBillingAddressTitle.Appearance.ForeColor = BrandResources.Primary;
        lblBillingAddressTitle.Appearance.Options.UseFont = true;
        lblBillingAddressTitle.Appearance.Options.UseForeColor = true;
        lblBillingAddressTitle.Location = new Point(20, 12);
        lblBillingAddressTitle.Name = "lblBillingAddressTitle";
        lblBillingAddressTitle.Size = new Size(130, 20);
        lblBillingAddressTitle.TabIndex = 0;
        lblBillingAddressTitle.Text = "Dirección de Factura";
        // 
        // lblBillingAddressSelector
        // 
        lblBillingAddressSelector.Location = new Point(20, 44);
        lblBillingAddressSelector.Name = "lblBillingAddressSelector";
        lblBillingAddressSelector.Size = new Size(99, 13);
        lblBillingAddressSelector.TabIndex = 0;
        lblBillingAddressSelector.Text = "Seleccione dirección:";
        // 
        // lueBillingAddress
        // 
        lueBillingAddress.Location = new Point(170, 40);
        lueBillingAddress.Name = "lueBillingAddress";
        lueBillingAddress.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueBillingAddress.Properties.DisplayMember = "DisplayText";
        lueBillingAddress.Properties.NullText = "Oficina Matriz - Calle 10 de Agosto N35-45";
        lueBillingAddress.Properties.ValueMember = "Id";
        lueBillingAddress.Size = new Size(430, 22);
        lueBillingAddress.TabIndex = 1;
        // 
        // btnBillingAddressLookup
        // 
        btnBillingAddressLookup.Location = new Point(606, 39);
        btnBillingAddressLookup.Name = "btnBillingAddressLookup";
        btnBillingAddressLookup.Size = new Size(30, 24);
        btnBillingAddressLookup.TabIndex = 2;
        btnBillingAddressLookup.Text = "...";
        // 
        // lblBillingNameCaption
        // 
        lblBillingNameCaption.Appearance.Font = AppTypography.LabelFont;
        lblBillingNameCaption.Appearance.Options.UseFont = true;
        lblBillingNameCaption.Location = new Point(44, 88);
        lblBillingNameCaption.Name = "lblBillingNameCaption";
        lblBillingNameCaption.Size = new Size(43, 13);
        lblBillingNameCaption.TabIndex = 3;
        lblBillingNameCaption.Text = "Nombre:";
        // 
        // txtBillingAddressName
        // 
        txtBillingAddressName.Location = new Point(160, 84);
        txtBillingAddressName.Name = "txtBillingAddressName";
        txtBillingAddressName.Properties.BorderStyle = BorderStyles.NoBorder;
        txtBillingAddressName.Properties.ReadOnly = true;
        txtBillingAddressName.Size = new Size(440, 22);
        txtBillingAddressName.TabIndex = 4;
        // 
        // lblBillingStreetCaption
        // 
        lblBillingStreetCaption.Appearance.Font = AppTypography.LabelFont;
        lblBillingStreetCaption.Appearance.Options.UseFont = true;
        lblBillingStreetCaption.Location = new Point(44, 114);
        lblBillingStreetCaption.Name = "lblBillingStreetCaption";
        lblBillingStreetCaption.Size = new Size(51, 13);
        lblBillingStreetCaption.TabIndex = 5;
        lblBillingStreetCaption.Text = "Dirección:";
        // 
        // memoBillingStreet
        // 
        memoBillingStreet.Location = new Point(160, 110);
        memoBillingStreet.Name = "memoBillingStreet";
        memoBillingStreet.Properties.BorderStyle = BorderStyles.NoBorder;
        memoBillingStreet.Properties.ReadOnly = true;
        memoBillingStreet.Size = new Size(440, 22);
        memoBillingStreet.TabIndex = 6;
        // 
        // lblBillingReferenceCaption
        // 
        lblBillingReferenceCaption.Appearance.Font = AppTypography.LabelFont;
        lblBillingReferenceCaption.Appearance.Options.UseFont = true;
        lblBillingReferenceCaption.Location = new Point(44, 140);
        lblBillingReferenceCaption.Name = "lblBillingReferenceCaption";
        lblBillingReferenceCaption.Size = new Size(55, 13);
        lblBillingReferenceCaption.TabIndex = 7;
        lblBillingReferenceCaption.Text = "Referencia:";
        // 
        // txtBillingReference
        // 
        txtBillingReference.Location = new Point(160, 136);
        txtBillingReference.Name = "txtBillingReference";
        txtBillingReference.Properties.BorderStyle = BorderStyles.NoBorder;
        txtBillingReference.Properties.ReadOnly = true;
        txtBillingReference.Size = new Size(440, 22);
        txtBillingReference.TabIndex = 8;
        // 
        // lblBillingCityCaption
        // 
        lblBillingCityCaption.Appearance.Font = AppTypography.LabelFont;
        lblBillingCityCaption.Appearance.Options.UseFont = true;
        lblBillingCityCaption.Location = new Point(44, 166);
        lblBillingCityCaption.Name = "lblBillingCityCaption";
        lblBillingCityCaption.Size = new Size(36, 13);
        lblBillingCityCaption.TabIndex = 9;
        lblBillingCityCaption.Text = "Ciudad:";
        // 
        // txtBillingCity
        // 
        txtBillingCity.Location = new Point(160, 162);
        txtBillingCity.Name = "txtBillingCity";
        txtBillingCity.Properties.BorderStyle = BorderStyles.NoBorder;
        txtBillingCity.Properties.ReadOnly = true;
        txtBillingCity.Size = new Size(160, 22);
        txtBillingCity.TabIndex = 10;
        // 
        // lblBillingStateCaption
        // 
        lblBillingStateCaption.Appearance.Font = AppTypography.LabelFont;
        lblBillingStateCaption.Appearance.Options.UseFont = true;
        lblBillingStateCaption.Location = new Point(44, 192);
        lblBillingStateCaption.Name = "lblBillingStateCaption";
        lblBillingStateCaption.Size = new Size(49, 13);
        lblBillingStateCaption.TabIndex = 11;
        lblBillingStateCaption.Text = "Provincia:";
        // 
        // txtBillingState
        // 
        txtBillingState.Location = new Point(160, 188);
        txtBillingState.Name = "txtBillingState";
        txtBillingState.Properties.BorderStyle = BorderStyles.NoBorder;
        txtBillingState.Properties.ReadOnly = true;
        txtBillingState.Size = new Size(160, 22);
        txtBillingState.TabIndex = 12;
        // 
        // lblBillingZipCodeCaption
        // 
        lblBillingZipCodeCaption.Appearance.Font = AppTypography.LabelFont;
        lblBillingZipCodeCaption.Appearance.Options.UseFont = true;
        lblBillingZipCodeCaption.Location = new Point(44, 218);
        lblBillingZipCodeCaption.Name = "lblBillingZipCodeCaption";
        lblBillingZipCodeCaption.Size = new Size(73, 13);
        lblBillingZipCodeCaption.TabIndex = 13;
        lblBillingZipCodeCaption.Text = "Código Postal:";
        // 
        // txtBillingZipCode
        // 
        txtBillingZipCode.Location = new Point(160, 214);
        txtBillingZipCode.Name = "txtBillingZipCode";
        txtBillingZipCode.Properties.BorderStyle = BorderStyles.NoBorder;
        txtBillingZipCode.Properties.ReadOnly = true;
        txtBillingZipCode.Size = new Size(160, 22);
        txtBillingZipCode.TabIndex = 14;
        // 
        // lblBillingCountryCaption
        // 
        lblBillingCountryCaption.Appearance.Font = AppTypography.LabelFont;
        lblBillingCountryCaption.Appearance.Options.UseFont = true;
        lblBillingCountryCaption.Location = new Point(44, 244);
        lblBillingCountryCaption.Name = "lblBillingCountryCaption";
        lblBillingCountryCaption.Size = new Size(24, 13);
        lblBillingCountryCaption.TabIndex = 15;
        lblBillingCountryCaption.Text = "País:";
        // 
        // txtBillingCountry
        // 
        txtBillingCountry.Location = new Point(160, 240);
        txtBillingCountry.Name = "txtBillingCountry";
        txtBillingCountry.Properties.BorderStyle = BorderStyles.NoBorder;
        txtBillingCountry.Properties.ReadOnly = true;
        txtBillingCountry.Size = new Size(160, 22);
        txtBillingCountry.TabIndex = 16;
        // 
        // lblBillingPhoneCaption
        // 
        lblBillingPhoneCaption.Appearance.Font = AppTypography.LabelFont;
        lblBillingPhoneCaption.Appearance.Options.UseFont = true;
        lblBillingPhoneCaption.Location = new Point(44, 270);
        lblBillingPhoneCaption.Name = "lblBillingPhoneCaption";
        lblBillingPhoneCaption.Size = new Size(46, 13);
        lblBillingPhoneCaption.TabIndex = 17;
        lblBillingPhoneCaption.Text = "Teléfono:";
        // 
        // txtBillingPhone
        // 
        txtBillingPhone.Location = new Point(160, 266);
        txtBillingPhone.Name = "txtBillingPhone";
        txtBillingPhone.Properties.BorderStyle = BorderStyles.NoBorder;
        txtBillingPhone.Properties.ReadOnly = true;
        txtBillingPhone.Size = new Size(160, 22);
        txtBillingPhone.TabIndex = 18;
        // 
        // lblBillingEmailCaption
        // 
        lblBillingEmailCaption.Appearance.Font = AppTypography.LabelFont;
        lblBillingEmailCaption.Appearance.Options.UseFont = true;
        lblBillingEmailCaption.Location = new Point(44, 296);
        lblBillingEmailCaption.Name = "lblBillingEmailCaption";
        lblBillingEmailCaption.Size = new Size(29, 13);
        lblBillingEmailCaption.TabIndex = 19;
        lblBillingEmailCaption.Text = "Email:";
        // 
        // txtBillingEmail
        // 
        txtBillingEmail.Location = new Point(160, 292);
        txtBillingEmail.Name = "txtBillingEmail";
        txtBillingEmail.Properties.BorderStyle = BorderStyles.NoBorder;
        txtBillingEmail.Properties.ReadOnly = true;
        txtBillingEmail.Size = new Size(300, 22);
        txtBillingEmail.TabIndex = 20;
        // 
        // lblBillingInfo
        // 
        lblBillingInfo.Appearance.ForeColor = BrandResources.Primary;
        lblBillingInfo.Appearance.Options.UseForeColor = true;
        lblBillingInfo.Location = new Point(20, 330);
        lblBillingInfo.Name = "lblBillingInfo";
        lblBillingInfo.Size = new Size(413, 13);
        lblBillingInfo.TabIndex = 21;
        lblBillingInfo.Text = "ⓘ  Seleccione una dirección del proveedor o modifíquela según sea necesario.";
        // 
        // tabApproval
        // 
        tabApproval.Controls.Add(pnlApprovalAmountCard);
        tabApproval.Controls.Add(pnlApprovalPolicyCard);
        tabApproval.Controls.Add(pnlApprovalLevelCard);
        tabApproval.Controls.Add(pnlApprovalStatusCard);
        tabApproval.Controls.Add(lblApprovalHistoryTitle);
        tabApproval.Controls.Add(gridApprovals);
        tabApproval.Controls.Add(pnlApprovalComment);
        tabApproval.Controls.Add(pnlApprovalFlow);
        tabApproval.Name = "tabApproval";
        tabApproval.Size = new Size(1326, 389);
        tabApproval.Text = "Autorización";
        // 
        // pnlApprovalAmountCard
        // 
        pnlApprovalAmountCard.BorderStyle = BorderStyles.Simple;
        pnlApprovalAmountCard.Controls.Add(lblApprovalAmountIcon);
        pnlApprovalAmountCard.Controls.Add(lblApprovalAmount);
        pnlApprovalAmountCard.Controls.Add(txtApprovalAmount);
        pnlApprovalAmountCard.Controls.Add(lblApprovalAmountCurrency);
        pnlApprovalAmountCard.Location = new Point(18, 12);
        pnlApprovalAmountCard.Name = "pnlApprovalAmountCard";
        pnlApprovalAmountCard.Size = new Size(320, 86);
        pnlApprovalAmountCard.TabIndex = 0;
        // 
        // lblApprovalAmountIcon
        // 
        lblApprovalAmountIcon.Appearance.BackColor = Color.FromArgb(230, 241, 255);
        lblApprovalAmountIcon.Appearance.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
        lblApprovalAmountIcon.Appearance.ForeColor = BrandResources.Primary;
        lblApprovalAmountIcon.Appearance.Options.UseBackColor = true;
        lblApprovalAmountIcon.Appearance.Options.UseFont = true;
        lblApprovalAmountIcon.Appearance.Options.UseForeColor = true;
        lblApprovalAmountIcon.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lblApprovalAmountIcon.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        lblApprovalAmountIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblApprovalAmountIcon.BorderStyle = BorderStyles.Simple;
        lblApprovalAmountIcon.Location = new Point(18, 18);
        lblApprovalAmountIcon.Name = "lblApprovalAmountIcon";
        lblApprovalAmountIcon.Size = new Size(54, 54);
        lblApprovalAmountIcon.TabIndex = 0;
        lblApprovalAmountIcon.Text = "$";
        // 
        // lblApprovalAmount
        // 
        lblApprovalAmount.Location = new Point(118, 14);
        lblApprovalAmount.Name = "lblApprovalAmount";
        lblApprovalAmount.Size = new Size(87, 13);
        lblApprovalAmount.TabIndex = 1;
        lblApprovalAmount.Text = "Monto Documento";
        // 
        // txtApprovalAmount
        // 
        txtApprovalAmount.Location = new Point(116, 32);
        txtApprovalAmount.Name = "txtApprovalAmount";
        txtApprovalAmount.Properties.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        txtApprovalAmount.Properties.Appearance.Options.UseFont = true;
        txtApprovalAmount.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        txtApprovalAmount.Properties.BorderStyle = BorderStyles.NoBorder;
        txtApprovalAmount.Properties.ReadOnly = true;
        txtApprovalAmount.Size = new Size(120, 22);
        txtApprovalAmount.TabIndex = 2;
        // 
        // lblApprovalAmountCurrency
        // 
        lblApprovalAmountCurrency.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lblApprovalAmountCurrency.AutoSizeMode = LabelAutoSizeMode.None;
        lblApprovalAmountCurrency.Location = new Point(116, 60);
        lblApprovalAmountCurrency.Name = "lblApprovalAmountCurrency";
        lblApprovalAmountCurrency.Size = new Size(120, 16);
        lblApprovalAmountCurrency.TabIndex = 3;
        lblApprovalAmountCurrency.Text = "USD";
        // 
        // pnlApprovalPolicyCard
        // 
        pnlApprovalPolicyCard.BorderStyle = BorderStyles.Simple;
        pnlApprovalPolicyCard.Controls.Add(lblApprovalPolicyIcon);
        pnlApprovalPolicyCard.Controls.Add(lblApprovalPolicy);
        pnlApprovalPolicyCard.Controls.Add(txtApprovalPolicy);
        pnlApprovalPolicyCard.Controls.Add(lblApprovalPolicyDescription);
        pnlApprovalPolicyCard.Location = new Point(348, 12);
        pnlApprovalPolicyCard.Name = "pnlApprovalPolicyCard";
        pnlApprovalPolicyCard.Size = new Size(320, 86);
        pnlApprovalPolicyCard.TabIndex = 1;
        // 
        // lblApprovalPolicyIcon
        // 
        lblApprovalPolicyIcon.Appearance.BackColor = Color.FromArgb(230, 241, 255);
        lblApprovalPolicyIcon.Appearance.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
        lblApprovalPolicyIcon.Appearance.ForeColor = BrandResources.Primary;
        lblApprovalPolicyIcon.Appearance.Options.UseBackColor = true;
        lblApprovalPolicyIcon.Appearance.Options.UseFont = true;
        lblApprovalPolicyIcon.Appearance.Options.UseForeColor = true;
        lblApprovalPolicyIcon.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lblApprovalPolicyIcon.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        lblApprovalPolicyIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblApprovalPolicyIcon.BorderStyle = BorderStyles.Simple;
        lblApprovalPolicyIcon.Location = new Point(18, 18);
        lblApprovalPolicyIcon.Name = "lblApprovalPolicyIcon";
        lblApprovalPolicyIcon.Size = new Size(54, 54);
        lblApprovalPolicyIcon.TabIndex = 0;
        lblApprovalPolicyIcon.Text = "V";
        // 
        // lblApprovalPolicy
        // 
        lblApprovalPolicy.Location = new Point(146, 14);
        lblApprovalPolicy.Name = "lblApprovalPolicy";
        lblApprovalPolicy.Size = new Size(78, 13);
        lblApprovalPolicy.TabIndex = 1;
        lblApprovalPolicy.Text = "Política Aplicada";
        // 
        // txtApprovalPolicy
        // 
        txtApprovalPolicy.Location = new Point(118, 32);
        txtApprovalPolicy.Name = "txtApprovalPolicy";
        txtApprovalPolicy.Properties.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        txtApprovalPolicy.Properties.Appearance.Options.UseFont = true;
        txtApprovalPolicy.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        txtApprovalPolicy.Properties.BorderStyle = BorderStyles.NoBorder;
        txtApprovalPolicy.Properties.ReadOnly = true;
        txtApprovalPolicy.Size = new Size(150, 22);
        txtApprovalPolicy.TabIndex = 2;
        // 
        // lblApprovalPolicyDescription
        // 
        lblApprovalPolicyDescription.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lblApprovalPolicyDescription.AutoSizeMode = LabelAutoSizeMode.None;
        lblApprovalPolicyDescription.Location = new Point(106, 60);
        lblApprovalPolicyDescription.Name = "lblApprovalPolicyDescription";
        lblApprovalPolicyDescription.Size = new Size(178, 16);
        lblApprovalPolicyDescription.TabIndex = 3;
        lblApprovalPolicyDescription.Text = "Compras Operativas";
        // 
        // pnlApprovalLevelCard
        // 
        pnlApprovalLevelCard.BorderStyle = BorderStyles.Simple;
        pnlApprovalLevelCard.Controls.Add(lblApprovalLevelIcon);
        pnlApprovalLevelCard.Controls.Add(lblApprovalLevel);
        pnlApprovalLevelCard.Controls.Add(txtApprovalLevel);
        pnlApprovalLevelCard.Controls.Add(lblApprovalLevelDescription);
        pnlApprovalLevelCard.Location = new Point(678, 12);
        pnlApprovalLevelCard.Name = "pnlApprovalLevelCard";
        pnlApprovalLevelCard.Size = new Size(320, 86);
        pnlApprovalLevelCard.TabIndex = 2;
        // 
        // lblApprovalLevelIcon
        // 
        lblApprovalLevelIcon.Appearance.BackColor = Color.FromArgb(230, 241, 255);
        lblApprovalLevelIcon.Appearance.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
        lblApprovalLevelIcon.Appearance.ForeColor = BrandResources.Primary;
        lblApprovalLevelIcon.Appearance.Options.UseBackColor = true;
        lblApprovalLevelIcon.Appearance.Options.UseFont = true;
        lblApprovalLevelIcon.Appearance.Options.UseForeColor = true;
        lblApprovalLevelIcon.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lblApprovalLevelIcon.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        lblApprovalLevelIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblApprovalLevelIcon.BorderStyle = BorderStyles.Simple;
        lblApprovalLevelIcon.Location = new Point(18, 18);
        lblApprovalLevelIcon.Name = "lblApprovalLevelIcon";
        lblApprovalLevelIcon.Size = new Size(54, 54);
        lblApprovalLevelIcon.TabIndex = 0;
        lblApprovalLevelIcon.Text = "2";
        // 
        // lblApprovalLevel
        // 
        lblApprovalLevel.Location = new Point(142, 14);
        lblApprovalLevel.Name = "lblApprovalLevel";
        lblApprovalLevel.Size = new Size(75, 13);
        lblApprovalLevel.TabIndex = 1;
        lblApprovalLevel.Text = "Nivel Requerido";
        // 
        // txtApprovalLevel
        // 
        txtApprovalLevel.Location = new Point(128, 32);
        txtApprovalLevel.Name = "txtApprovalLevel";
        txtApprovalLevel.Properties.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        txtApprovalLevel.Properties.Appearance.Options.UseFont = true;
        txtApprovalLevel.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        txtApprovalLevel.Properties.BorderStyle = BorderStyles.NoBorder;
        txtApprovalLevel.Properties.ReadOnly = true;
        txtApprovalLevel.Size = new Size(120, 22);
        txtApprovalLevel.TabIndex = 2;
        // 
        // lblApprovalLevelDescription
        // 
        lblApprovalLevelDescription.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lblApprovalLevelDescription.AutoSizeMode = LabelAutoSizeMode.None;
        lblApprovalLevelDescription.Location = new Point(128, 60);
        lblApprovalLevelDescription.Name = "lblApprovalLevelDescription";
        lblApprovalLevelDescription.Size = new Size(120, 16);
        lblApprovalLevelDescription.TabIndex = 3;
        lblApprovalLevelDescription.Text = "aprobaciones";
        // 
        // pnlApprovalStatusCard
        // 
        pnlApprovalStatusCard.BorderStyle = BorderStyles.Simple;
        pnlApprovalStatusCard.Controls.Add(lblApprovalStatusIcon);
        pnlApprovalStatusCard.Controls.Add(lblApprovalStatus);
        pnlApprovalStatusCard.Controls.Add(txtApprovalStatus);
        pnlApprovalStatusCard.Controls.Add(lblApprovalStatusDescription);
        pnlApprovalStatusCard.Location = new Point(1008, 12);
        pnlApprovalStatusCard.Name = "pnlApprovalStatusCard";
        pnlApprovalStatusCard.Size = new Size(310, 86);
        pnlApprovalStatusCard.TabIndex = 3;
        // 
        // lblApprovalStatusIcon
        // 
        lblApprovalStatusIcon.Appearance.BackColor = Color.FromArgb(230, 241, 255);
        lblApprovalStatusIcon.Appearance.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
        lblApprovalStatusIcon.Appearance.ForeColor = BrandResources.Primary;
        lblApprovalStatusIcon.Appearance.Options.UseBackColor = true;
        lblApprovalStatusIcon.Appearance.Options.UseFont = true;
        lblApprovalStatusIcon.Appearance.Options.UseForeColor = true;
        lblApprovalStatusIcon.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lblApprovalStatusIcon.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        lblApprovalStatusIcon.AutoSizeMode = LabelAutoSizeMode.None;
        lblApprovalStatusIcon.BorderStyle = BorderStyles.Simple;
        lblApprovalStatusIcon.Location = new Point(18, 18);
        lblApprovalStatusIcon.Name = "lblApprovalStatusIcon";
        lblApprovalStatusIcon.Size = new Size(54, 54);
        lblApprovalStatusIcon.TabIndex = 0;
        lblApprovalStatusIcon.Text = "H";
        // 
        // lblApprovalStatus
        // 
        lblApprovalStatus.Location = new Point(128, 20);
        lblApprovalStatus.Name = "lblApprovalStatus";
        lblApprovalStatus.Size = new Size(77, 13);
        lblApprovalStatus.TabIndex = 1;
        lblApprovalStatus.Text = "Estado del Flujo";
        // 
        // txtApprovalStatus
        // 
        txtApprovalStatus.Location = new Point(116, 40);
        txtApprovalStatus.Name = "txtApprovalStatus";
        txtApprovalStatus.Properties.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        txtApprovalStatus.Properties.Appearance.ForeColor = BrandResources.Primary;
        txtApprovalStatus.Properties.Appearance.Options.UseFont = true;
        txtApprovalStatus.Properties.Appearance.Options.UseForeColor = true;
        txtApprovalStatus.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        txtApprovalStatus.Properties.BorderStyle = BorderStyles.NoBorder;
        txtApprovalStatus.Properties.ReadOnly = true;
        txtApprovalStatus.Size = new Size(120, 22);
        txtApprovalStatus.TabIndex = 2;
        // 
        // lblApprovalStatusDescription
        // 
        lblApprovalStatusDescription.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lblApprovalStatusDescription.AutoSizeMode = LabelAutoSizeMode.None;
        lblApprovalStatusDescription.Location = new Point(92, 66);
        lblApprovalStatusDescription.Name = "lblApprovalStatusDescription";
        lblApprovalStatusDescription.Size = new Size(180, 16);
        lblApprovalStatusDescription.TabIndex = 3;
        lblApprovalStatusDescription.Text = "Pendiente de aprobación";
        // 
        // lblApprovalHistoryTitle
        // 
        lblApprovalHistoryTitle.Appearance.Font = AppTypography.SectionFont;
        lblApprovalHistoryTitle.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblApprovalHistoryTitle.Appearance.Options.UseFont = true;
        lblApprovalHistoryTitle.Appearance.Options.UseForeColor = true;
        lblApprovalHistoryTitle.Location = new Point(18, 106);
        lblApprovalHistoryTitle.Name = "lblApprovalHistoryTitle";
        lblApprovalHistoryTitle.Size = new Size(153, 20);
        lblApprovalHistoryTitle.TabIndex = 4;
        lblApprovalHistoryTitle.Text = "Historial de Aprobaciones";
        // 
        // gridApprovals
        // 
        gridApprovals.Location = new Point(18, 132);
        gridApprovals.MainView = viewApprovals;
        gridApprovals.Name = "gridApprovals";
        gridApprovals.Size = new Size(1300, 126);
        gridApprovals.TabIndex = 5;
        gridApprovals.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { viewApprovals });
        // 
        // viewApprovals
        // 
        viewApprovals.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        viewApprovals.Appearance.HeaderPanel.Options.UseFont = true;
        viewApprovals.Appearance.Row.Font = new Font("Segoe UI", 9F);
        viewApprovals.Appearance.Row.Options.UseFont = true;
        viewApprovals.GridControl = gridApprovals;
        viewApprovals.Name = "viewApprovals";
        viewApprovals.OptionsBehavior.Editable = false;
        viewApprovals.OptionsView.ShowGroupPanel = false;
        // 
        // pnlApprovalComment
        // 
        pnlApprovalComment.BorderStyle = BorderStyles.NoBorder;
        pnlApprovalComment.Controls.Add(lblApprovalCommentTitle);
        pnlApprovalComment.Controls.Add(memoApprovalObservation);
        pnlApprovalComment.Location = new Point(18, 270);
        pnlApprovalComment.Name = "pnlApprovalComment";
        pnlApprovalComment.Size = new Size(520, 112);
        pnlApprovalComment.TabIndex = 6;
        // 
        // lblApprovalCommentTitle
        // 
        lblApprovalCommentTitle.Appearance.Font = AppTypography.SectionFont;
        lblApprovalCommentTitle.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblApprovalCommentTitle.Appearance.Options.UseFont = true;
        lblApprovalCommentTitle.Appearance.Options.UseForeColor = true;
        lblApprovalCommentTitle.Location = new Point(0, 0);
        lblApprovalCommentTitle.Name = "lblApprovalCommentTitle";
        lblApprovalCommentTitle.Size = new Size(146, 20);
        lblApprovalCommentTitle.TabIndex = 0;
        lblApprovalCommentTitle.Text = "Observación / Comentario";
        // 
        // memoApprovalObservation
        // 
        memoApprovalObservation.Location = new Point(0, 26);
        memoApprovalObservation.Name = "memoApprovalObservation";
        memoApprovalObservation.Properties.NullValuePrompt = "Observación / comentario";
        memoApprovalObservation.Size = new Size(520, 78);
        memoApprovalObservation.TabIndex = 1;
        // 
        // pnlApprovalFlow
        // 
        pnlApprovalFlow.BorderStyle = BorderStyles.NoBorder;
        pnlApprovalFlow.Controls.Add(lblApprovalFlowTitle);
        pnlApprovalFlow.Controls.Add(gridApprovalFlow);
        pnlApprovalFlow.Location = new Point(570, 270);
        pnlApprovalFlow.Name = "pnlApprovalFlow";
        pnlApprovalFlow.Size = new Size(748, 112);
        pnlApprovalFlow.TabIndex = 7;
        lblApprovalFlowTitle.Appearance.Font = AppTypography.SectionFont;
        lblApprovalFlowTitle.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblApprovalFlowTitle.Appearance.Options.UseFont = true;
        lblApprovalFlowTitle.Appearance.Options.UseForeColor = true;
        lblApprovalFlowTitle.Location = new Point(0, 0);
        lblApprovalFlowTitle.Name = "lblApprovalFlowTitle";
        lblApprovalFlowTitle.Size = new Size(186, 20);
        lblApprovalFlowTitle.TabIndex = 0;
        lblApprovalFlowTitle.Text = "Flujo de Aprobación Configurado";
        // 
        // gridApprovalFlow
        // 
        gridApprovalFlow.Location = new Point(0, 26);
        gridApprovalFlow.MainView = viewApprovalFlow;
        gridApprovalFlow.Name = "gridApprovalFlow";
        gridApprovalFlow.Size = new Size(748, 86);
        gridApprovalFlow.TabIndex = 1;
        gridApprovalFlow.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { viewApprovalFlow });
        // 
        // viewApprovalFlow
        // 
        viewApprovalFlow.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        viewApprovalFlow.Appearance.HeaderPanel.Options.UseFont = true;
        viewApprovalFlow.Appearance.Row.Font = new Font("Segoe UI", 9F);
        viewApprovalFlow.Appearance.Row.Options.UseFont = true;
        viewApprovalFlow.Columns.AddRange(new GridColumn[] { colApprovalFlowStep, colApprovalFlowRole, colApprovalFlowUser, colApprovalFlowStatus, colApprovalFlowDate });
        viewApprovalFlow.GridControl = gridApprovalFlow;
        viewApprovalFlow.Name = "viewApprovalFlow";
        viewApprovalFlow.OptionsBehavior.Editable = false;
        viewApprovalFlow.OptionsView.ShowGroupPanel = false;
        // 
        // colApprovalFlowStep
        // 
        colApprovalFlowStep.Caption = "Paso";
        colApprovalFlowStep.FieldName = "Step";
        colApprovalFlowStep.Name = "colApprovalFlowStep";
        colApprovalFlowStep.Visible = true;
        colApprovalFlowStep.VisibleIndex = 0;
        colApprovalFlowStep.Width = 55;
        // 
        // colApprovalFlowRole
        // 
        colApprovalFlowRole.Caption = "Rol";
        colApprovalFlowRole.FieldName = "Role";
        colApprovalFlowRole.Name = "colApprovalFlowRole";
        colApprovalFlowRole.Visible = true;
        colApprovalFlowRole.VisibleIndex = 1;
        colApprovalFlowRole.Width = 190;
        // 
        // colApprovalFlowUser
        // 
        colApprovalFlowUser.Caption = "Usuario";
        colApprovalFlowUser.FieldName = "User";
        colApprovalFlowUser.Name = "colApprovalFlowUser";
        colApprovalFlowUser.Visible = true;
        colApprovalFlowUser.VisibleIndex = 2;
        colApprovalFlowUser.Width = 180;
        // 
        // colApprovalFlowStatus
        // 
        colApprovalFlowStatus.Caption = "Estado";
        colApprovalFlowStatus.FieldName = "Status";
        colApprovalFlowStatus.Name = "colApprovalFlowStatus";
        colApprovalFlowStatus.Visible = true;
        colApprovalFlowStatus.VisibleIndex = 3;
        colApprovalFlowStatus.Width = 120;
        // 
        // colApprovalFlowDate
        // 
        colApprovalFlowDate.Caption = "Fecha";
        colApprovalFlowDate.FieldName = "DateText";
        colApprovalFlowDate.Name = "colApprovalFlowDate";
        colApprovalFlowDate.Visible = true;
        colApprovalFlowDate.VisibleIndex = 4;
        colApprovalFlowDate.Width = 150;
        // 
        // tabRelatedDocuments
        // 
        tabRelatedDocuments.Controls.Add(btnAddRelatedDocument);
        tabRelatedDocuments.Controls.Add(btnViewRelatedDocument);
        tabRelatedDocuments.Controls.Add(btnUnlinkRelatedDocument);
        tabRelatedDocuments.Controls.Add(btnRefreshRelatedDocuments);
        tabRelatedDocuments.Controls.Add(lblRelatedDocumentsTitle);
        tabRelatedDocuments.Controls.Add(gridRelatedDocuments);
        tabRelatedDocuments.Controls.Add(pnlRelatedDocumentNotes);
        tabRelatedDocuments.Name = "tabRelatedDocuments";
        tabRelatedDocuments.Size = new Size(1326, 389);
        tabRelatedDocuments.Text = "Documentos Relacionados";
        // 
        // btnAddRelatedDocument
        // 
        btnAddRelatedDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnAddRelatedDocument.Appearance.ForeColor = Color.FromArgb(5, 35, 85);
        btnAddRelatedDocument.Appearance.Options.UseFont = true;
        btnAddRelatedDocument.Appearance.Options.UseForeColor = true;
        btnAddRelatedDocument.ImageOptions.SvgImageSize = new Size(16, 16);
        btnAddRelatedDocument.Location = new Point(18, 16);
        btnAddRelatedDocument.Name = "btnAddRelatedDocument";
        btnAddRelatedDocument.Size = new Size(145, 32);
        btnAddRelatedDocument.TabIndex = 0;
        btnAddRelatedDocument.Text = "Agregar Relación";
        // 
        // btnViewRelatedDocument
        // 
        btnViewRelatedDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnViewRelatedDocument.Appearance.ForeColor = Color.FromArgb(5, 35, 85);
        btnViewRelatedDocument.Appearance.Options.UseFont = true;
        btnViewRelatedDocument.Appearance.Options.UseForeColor = true;
        btnViewRelatedDocument.ImageOptions.SvgImageSize = new Size(16, 16);
        btnViewRelatedDocument.Location = new Point(178, 16);
        btnViewRelatedDocument.Name = "btnViewRelatedDocument";
        btnViewRelatedDocument.Size = new Size(145, 32);
        btnViewRelatedDocument.TabIndex = 1;
        btnViewRelatedDocument.Text = "Ver Documento";
        // 
        // btnUnlinkRelatedDocument
        // 
        btnUnlinkRelatedDocument.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnUnlinkRelatedDocument.Appearance.ForeColor = Color.FromArgb(5, 35, 85);
        btnUnlinkRelatedDocument.Appearance.Options.UseFont = true;
        btnUnlinkRelatedDocument.Appearance.Options.UseForeColor = true;
        btnUnlinkRelatedDocument.ImageOptions.SvgImageSize = new Size(16, 16);
        btnUnlinkRelatedDocument.Location = new Point(338, 16);
        btnUnlinkRelatedDocument.Name = "btnUnlinkRelatedDocument";
        btnUnlinkRelatedDocument.Size = new Size(145, 32);
        btnUnlinkRelatedDocument.TabIndex = 2;
        btnUnlinkRelatedDocument.Text = "Desvincular";
        // 
        // btnRefreshRelatedDocuments
        // 
        btnRefreshRelatedDocuments.Appearance.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        btnRefreshRelatedDocuments.Appearance.ForeColor = Color.FromArgb(5, 35, 85);
        btnRefreshRelatedDocuments.Appearance.Options.UseFont = true;
        btnRefreshRelatedDocuments.Appearance.Options.UseForeColor = true;
        btnRefreshRelatedDocuments.ImageOptions.SvgImageSize = new Size(16, 16);
        btnRefreshRelatedDocuments.Location = new Point(498, 16);
        btnRefreshRelatedDocuments.Name = "btnRefreshRelatedDocuments";
        btnRefreshRelatedDocuments.Size = new Size(120, 32);
        btnRefreshRelatedDocuments.TabIndex = 3;
        btnRefreshRelatedDocuments.Text = "Actualizar";
        // 
        // lblRelatedDocumentsTitle
        // 
        lblRelatedDocumentsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblRelatedDocumentsTitle.Appearance.ForeColor = Color.FromArgb(0, 92, 255);
        lblRelatedDocumentsTitle.Appearance.Options.UseFont = true;
        lblRelatedDocumentsTitle.Appearance.Options.UseForeColor = true;
        lblRelatedDocumentsTitle.Location = new Point(18, 63);
        lblRelatedDocumentsTitle.Name = "lblRelatedDocumentsTitle";
        lblRelatedDocumentsTitle.Size = new Size(181, 20);
        lblRelatedDocumentsTitle.TabIndex = 4;
        lblRelatedDocumentsTitle.Text = "Documentos Relacionados";
        // 
        // gridRelatedDocuments
        // 
        gridRelatedDocuments.Location = new Point(18, 88);
        gridRelatedDocuments.MainView = viewRelatedDocuments;
        gridRelatedDocuments.Name = "gridRelatedDocuments";
        gridRelatedDocuments.RepositoryItems.AddRange(new RepositoryItem[] { repoRelatedDocumentAction });
        gridRelatedDocuments.Size = new Size(1300, 195);
        gridRelatedDocuments.TabIndex = 5;
        gridRelatedDocuments.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { viewRelatedDocuments });
        // 
        // viewRelatedDocuments
        // 
        viewRelatedDocuments.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        viewRelatedDocuments.Appearance.HeaderPanel.Options.UseFont = true;
        viewRelatedDocuments.Appearance.Row.Font = new Font("Segoe UI", 9F);
        viewRelatedDocuments.Appearance.Row.Options.UseFont = true;
        viewRelatedDocuments.Columns.AddRange(new GridColumn[] { colRelatedDocumentIcon, colRelatedDocumentType, colRelatedDocumentSeries, colRelatedDocumentNumber, colRelatedDocumentDate, colRelatedDocumentStatus, colRelatedDocumentReference, colRelatedDocumentComment, colRelatedDocumentTotal, colRelatedDocumentAction });
        viewRelatedDocuments.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
        viewRelatedDocuments.GridControl = gridRelatedDocuments;
        viewRelatedDocuments.Name = "viewRelatedDocuments";
        viewRelatedDocuments.OptionsBehavior.Editable = false;
        viewRelatedDocuments.OptionsSelection.EnableAppearanceFocusedCell = false;
        viewRelatedDocuments.OptionsView.ShowGroupPanel = false;
        viewRelatedDocuments.OptionsView.ShowIndicator = false;
        // 
        // colRelatedDocumentIcon
        // 
        colRelatedDocumentIcon.Caption = "";
        colRelatedDocumentIcon.FieldName = "IconText";
        colRelatedDocumentIcon.Name = "colRelatedDocumentIcon";
        colRelatedDocumentIcon.OptionsColumn.AllowEdit = false;
        colRelatedDocumentIcon.Visible = true;
        colRelatedDocumentIcon.VisibleIndex = 0;
        colRelatedDocumentIcon.Width = 36;
        // 
        // colRelatedDocumentType
        // 
        colRelatedDocumentType.Caption = "Tipo Documento";
        colRelatedDocumentType.FieldName = "RelatedDocumentType";
        colRelatedDocumentType.Name = "colRelatedDocumentType";
        colRelatedDocumentType.Visible = true;
        colRelatedDocumentType.VisibleIndex = 1;
        colRelatedDocumentType.Width = 190;
        // 
        // colRelatedDocumentSeries
        // 
        colRelatedDocumentSeries.Caption = "Serie";
        colRelatedDocumentSeries.FieldName = "Series";
        colRelatedDocumentSeries.Name = "colRelatedDocumentSeries";
        colRelatedDocumentSeries.Visible = true;
        colRelatedDocumentSeries.VisibleIndex = 2;
        colRelatedDocumentSeries.Width = 110;
        // 
        // colRelatedDocumentNumber
        // 
        colRelatedDocumentNumber.Caption = "Número";
        colRelatedDocumentNumber.FieldName = "Number";
        colRelatedDocumentNumber.Name = "colRelatedDocumentNumber";
        colRelatedDocumentNumber.Visible = true;
        colRelatedDocumentNumber.VisibleIndex = 3;
        colRelatedDocumentNumber.Width = 120;
        // 
        // colRelatedDocumentDate
        // 
        colRelatedDocumentDate.Caption = "Fecha";
        colRelatedDocumentDate.DisplayFormat.FormatString = "dd/MM/yyyy";
        colRelatedDocumentDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        colRelatedDocumentDate.FieldName = "Date";
        colRelatedDocumentDate.Name = "colRelatedDocumentDate";
        colRelatedDocumentDate.Visible = true;
        colRelatedDocumentDate.VisibleIndex = 4;
        colRelatedDocumentDate.Width = 105;
        // 
        // colRelatedDocumentStatus
        // 
        colRelatedDocumentStatus.Caption = "Estado";
        colRelatedDocumentStatus.FieldName = "Status";
        colRelatedDocumentStatus.Name = "colRelatedDocumentStatus";
        colRelatedDocumentStatus.Visible = true;
        colRelatedDocumentStatus.VisibleIndex = 5;
        colRelatedDocumentStatus.Width = 110;
        // 
        // colRelatedDocumentReference
        // 
        colRelatedDocumentReference.Caption = "Referencia";
        colRelatedDocumentReference.FieldName = "Reference";
        colRelatedDocumentReference.Name = "colRelatedDocumentReference";
        colRelatedDocumentReference.Visible = true;
        colRelatedDocumentReference.VisibleIndex = 6;
        colRelatedDocumentReference.Width = 210;
        // 
        // colRelatedDocumentComment
        // 
        colRelatedDocumentComment.Caption = "Comentario";
        colRelatedDocumentComment.FieldName = "Comment";
        colRelatedDocumentComment.Name = "colRelatedDocumentComment";
        colRelatedDocumentComment.Visible = true;
        colRelatedDocumentComment.VisibleIndex = 7;
        colRelatedDocumentComment.Width = 270;
        // 
        // colRelatedDocumentTotal
        // 
        colRelatedDocumentTotal.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        colRelatedDocumentTotal.Caption = "Total";
        colRelatedDocumentTotal.DisplayFormat.FormatString = "N2";
        colRelatedDocumentTotal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        colRelatedDocumentTotal.FieldName = "Total";
        colRelatedDocumentTotal.Name = "colRelatedDocumentTotal";
        colRelatedDocumentTotal.Visible = true;
        colRelatedDocumentTotal.VisibleIndex = 8;
        colRelatedDocumentTotal.Width = 100;
        // 
        // colRelatedDocumentAction
        // 
        colRelatedDocumentAction.Caption = "";
        colRelatedDocumentAction.ColumnEdit = repoRelatedDocumentAction;
        colRelatedDocumentAction.FieldName = "ActionText";
        colRelatedDocumentAction.Name = "colRelatedDocumentAction";
        colRelatedDocumentAction.OptionsColumn.AllowEdit = true;
        colRelatedDocumentAction.Visible = true;
        colRelatedDocumentAction.VisibleIndex = 9;
        colRelatedDocumentAction.Width = 42;
        // 
        // repoRelatedDocumentAction
        // 
        repoRelatedDocumentAction.AutoHeight = false;
        repoRelatedDocumentAction.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Ellipsis) });
        repoRelatedDocumentAction.Name = "repoRelatedDocumentAction";
        repoRelatedDocumentAction.TextEditStyle = TextEditStyles.HideTextEditor;
        // 
        // pnlRelatedDocumentNotes
        // 
        pnlRelatedDocumentNotes.Controls.Add(lblRelatedDocumentNotesTitle);
        pnlRelatedDocumentNotes.Controls.Add(memoRelatedDocumentNotes);
        pnlRelatedDocumentNotes.Location = new Point(18, 302);
        pnlRelatedDocumentNotes.Name = "pnlRelatedDocumentNotes";
        pnlRelatedDocumentNotes.Size = new Size(1300, 78);
        pnlRelatedDocumentNotes.TabIndex = 6;
        // 
        // lblRelatedDocumentNotesTitle
        // 
        lblRelatedDocumentNotesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblRelatedDocumentNotesTitle.Appearance.ForeColor = Color.FromArgb(0, 92, 255);
        lblRelatedDocumentNotesTitle.Appearance.Options.UseFont = true;
        lblRelatedDocumentNotesTitle.Appearance.Options.UseForeColor = true;
        lblRelatedDocumentNotesTitle.Location = new Point(14, 10);
        lblRelatedDocumentNotesTitle.Name = "lblRelatedDocumentNotesTitle";
        lblRelatedDocumentNotesTitle.Size = new Size(101, 20);
        lblRelatedDocumentNotesTitle.TabIndex = 0;
        lblRelatedDocumentNotesTitle.Text = "Observaciones";
        // 
        // memoRelatedDocumentNotes
        // 
        memoRelatedDocumentNotes.Location = new Point(14, 34);
        memoRelatedDocumentNotes.Name = "memoRelatedDocumentNotes";
        memoRelatedDocumentNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memoRelatedDocumentNotes.Properties.Appearance.Options.UseFont = true;
        memoRelatedDocumentNotes.Properties.NullValuePrompt = "Ingrese observaciones adicionales relacionadas con los documentos.";
        memoRelatedDocumentNotes.Size = new Size(1268, 34);
        memoRelatedDocumentNotes.TabIndex = 1;
        // 
        // tabSap
        tabSap.Controls.Add(pnlSapSync);
        tabSap.Controls.Add(pnlSapDocument);
        tabSap.Controls.Add(pnlSapMessages);
        tabSap.Controls.Add(btnSyncSap);
        tabSap.Controls.Add(btnRefreshSapStatus);
        tabSap.Controls.Add(btnCancelSapSync);
        tabSap.Controls.Add(lblSapLogsTitle);
        tabSap.Controls.Add(gridSapLogs);
        tabSap.Name = "tabSap";
        tabSap.Size = new Size(1326, 389);
        tabSap.Text = "SAP";
        // 
        // pnlSapSync
        // 
        pnlSapSync.BorderStyle = BorderStyles.NoBorder;
        pnlSapSync.Controls.Add(lblSapSyncTitle);
        pnlSapSync.Controls.Add(lblSapStatusCaption);
        pnlSapSync.Controls.Add(txtSapStatus);
        pnlSapSync.Controls.Add(lblSapSyncDocEntryCaption);
        pnlSapSync.Controls.Add(txtSapSyncDocEntry);
        pnlSapSync.Controls.Add(lblSapSyncDocNumCaption);
        pnlSapSync.Controls.Add(txtSapSyncDocNum);
        pnlSapSync.Controls.Add(lblSapObjectTypeCaption);
        pnlSapSync.Controls.Add(txtSapObjectType);
        pnlSapSync.Controls.Add(lblSapSyncDateCaption);
        pnlSapSync.Controls.Add(txtSapSyncDate);
        pnlSapSync.Controls.Add(lblSapUserCaption);
        pnlSapSync.Controls.Add(txtSapUser);
        pnlSapSync.Location = new Point(18, 10);
        pnlSapSync.Name = "pnlSapSync";
        pnlSapSync.Size = new Size(378, 177);
        pnlSapSync.TabIndex = 0;
        // 
        // lblSapSyncTitle
        // 
        lblSapSyncTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapSyncTitle.Appearance.ForeColor = Color.FromArgb(0, 92, 255);
        lblSapSyncTitle.Appearance.Options.UseFont = true;
        lblSapSyncTitle.Appearance.Options.UseForeColor = true;
        lblSapSyncTitle.Location = new Point(0, 0);
        lblSapSyncTitle.Name = "lblSapSyncTitle";
        lblSapSyncTitle.Size = new Size(149, 20);
        lblSapSyncTitle.TabIndex = 0;
        lblSapSyncTitle.Text = "Sincronización con SAP";
        // 
        // lblSapStatusCaption
        // 
        lblSapStatusCaption.Location = new Point(8, 32);
        lblSapStatusCaption.Name = "lblSapStatusCaption";
        lblSapStatusCaption.Size = new Size(59, 13);
        lblSapStatusCaption.TabIndex = 1;
        lblSapStatusCaption.Text = "Estado SAP:";
        // 
        // txtSapStatus
        // 
        txtSapStatus.Location = new Point(190, 29);
        txtSapStatus.Name = "txtSapStatus";
        txtSapStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapStatus.Properties.Appearance.Options.UseFont = true;
        txtSapStatus.Properties.ReadOnly = true;
        txtSapStatus.Size = new Size(170, 22);
        txtSapStatus.TabIndex = 2;
        // 
        // lblSapSyncDocEntryCaption
        // 
        lblSapSyncDocEntryCaption.Location = new Point(8, 58);
        lblSapSyncDocEntryCaption.Name = "lblSapSyncDocEntryCaption";
        lblSapSyncDocEntryCaption.Size = new Size(50, 13);
        lblSapSyncDocEntryCaption.TabIndex = 3;
        lblSapSyncDocEntryCaption.Text = "DocEntry:";
        // 
        // txtSapSyncDocEntry
        // 
        txtSapSyncDocEntry.Location = new Point(190, 55);
        txtSapSyncDocEntry.Name = "txtSapSyncDocEntry";
        txtSapSyncDocEntry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapSyncDocEntry.Properties.Appearance.Options.UseFont = true;
        txtSapSyncDocEntry.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        txtSapSyncDocEntry.Properties.Appearance.Options.UseTextOptions = true;
        txtSapSyncDocEntry.Properties.ReadOnly = true;
        txtSapSyncDocEntry.Size = new Size(170, 22);
        txtSapSyncDocEntry.TabIndex = 4;
        // 
        // lblSapSyncDocNumCaption
        // 
        lblSapSyncDocNumCaption.Location = new Point(8, 84);
        lblSapSyncDocNumCaption.Name = "lblSapSyncDocNumCaption";
        lblSapSyncDocNumCaption.Size = new Size(45, 13);
        lblSapSyncDocNumCaption.TabIndex = 5;
        lblSapSyncDocNumCaption.Text = "DocNum:";
        // 
        // txtSapSyncDocNum
        // 
        txtSapSyncDocNum.Location = new Point(190, 81);
        txtSapSyncDocNum.Name = "txtSapSyncDocNum";
        txtSapSyncDocNum.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapSyncDocNum.Properties.Appearance.Options.UseFont = true;
        txtSapSyncDocNum.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        txtSapSyncDocNum.Properties.Appearance.Options.UseTextOptions = true;
        txtSapSyncDocNum.Properties.ReadOnly = true;
        txtSapSyncDocNum.Size = new Size(170, 22);
        txtSapSyncDocNum.TabIndex = 6;
        // 
        // lblSapObjectTypeCaption
        // 
        lblSapObjectTypeCaption.Location = new Point(8, 110);
        lblSapObjectTypeCaption.Name = "lblSapObjectTypeCaption";
        lblSapObjectTypeCaption.Size = new Size(60, 13);
        lblSapObjectTypeCaption.TabIndex = 7;
        lblSapObjectTypeCaption.Text = "ObjectType:";
        // 
        // txtSapObjectType
        // 
        txtSapObjectType.Location = new Point(190, 107);
        txtSapObjectType.Name = "txtSapObjectType";
        txtSapObjectType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapObjectType.Properties.Appearance.Options.UseFont = true;
        txtSapObjectType.Properties.ReadOnly = true;
        txtSapObjectType.Size = new Size(170, 22);
        txtSapObjectType.TabIndex = 8;
        // 
        // lblSapSyncDateCaption
        // 
        lblSapSyncDateCaption.Location = new Point(8, 136);
        lblSapSyncDateCaption.Name = "lblSapSyncDateCaption";
        lblSapSyncDateCaption.Size = new Size(106, 13);
        lblSapSyncDateCaption.TabIndex = 9;
        lblSapSyncDateCaption.Text = "Fecha Sincronización:";
        // 
        // txtSapSyncDate
        // 
        txtSapSyncDate.Location = new Point(190, 133);
        txtSapSyncDate.Name = "txtSapSyncDate";
        txtSapSyncDate.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapSyncDate.Properties.Appearance.Options.UseFont = true;
        txtSapSyncDate.Properties.ReadOnly = true;
        txtSapSyncDate.Size = new Size(170, 22);
        txtSapSyncDate.TabIndex = 10;
        // 
        // lblSapUserCaption
        // 
        lblSapUserCaption.Location = new Point(8, 154);
        lblSapUserCaption.Name = "lblSapUserCaption";
        lblSapUserCaption.Size = new Size(113, 13);
        lblSapUserCaption.TabIndex = 11;
        lblSapUserCaption.Text = "Usuario Sincronización:";
        // 
        // txtSapUser
        // 
        txtSapUser.Location = new Point(190, 151);
        txtSapUser.Name = "txtSapUser";
        txtSapUser.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapUser.Properties.Appearance.Options.UseFont = true;
        txtSapUser.Properties.ReadOnly = true;
        txtSapUser.Size = new Size(170, 22);
        txtSapUser.TabIndex = 12;
        // 
        // lblSapLastErrorCaption
        // 
        lblSapLastErrorCaption.Location = new Point(8, 188);
        lblSapLastErrorCaption.Name = "lblSapLastErrorCaption";
        lblSapLastErrorCaption.Size = new Size(65, 13);
        lblSapLastErrorCaption.TabIndex = 13;
        lblSapLastErrorCaption.Text = "Último Error:";
        lblSapLastErrorCaption.Visible = false;
        // 
        // txtSapLastError
        // 
        txtSapLastError.Location = new Point(190, 185);
        txtSapLastError.Name = "txtSapLastError";
        txtSapLastError.Properties.ReadOnly = true;
        txtSapLastError.Size = new Size(170, 22);
        txtSapLastError.TabIndex = 14;
        txtSapLastError.Visible = false;
        // 
        // btnSyncSap
        // 
        btnSyncSap.Location = new Point(18, 190);
        btnSyncSap.Name = "btnSyncSap";
        btnSyncSap.Size = new Size(378, 24);
        btnSyncSap.TabIndex = 15;
        btnSyncSap.Text = "Sincronizar con SAP";
        // 
        // btnRefreshSapStatus
        // 
        btnRefreshSapStatus.Location = new Point(18, 218);
        btnRefreshSapStatus.Name = "btnRefreshSapStatus";
        btnRefreshSapStatus.Size = new Size(378, 24);
        btnRefreshSapStatus.TabIndex = 16;
        btnRefreshSapStatus.Text = "Actualizar Estado";
        // 
        // btnCancelSapSync
        // 
        btnCancelSapSync.Location = new Point(18, 246);
        btnCancelSapSync.Name = "btnCancelSapSync";
        btnCancelSapSync.Size = new Size(378, 24);
        btnCancelSapSync.TabIndex = 17;
        btnCancelSapSync.Text = "Cancelar Sincronización";
        // 
        // pnlSapDocument
        // 
        pnlSapDocument.BorderStyle = BorderStyles.NoBorder;
        pnlSapDocument.Controls.Add(lblSapDocumentTitle);
        pnlSapDocument.Controls.Add(lblSapDocEntryCaption);
        pnlSapDocument.Controls.Add(txtSapDocEntry);
        pnlSapDocument.Controls.Add(lblSapDocNumCaption);
        pnlSapDocument.Controls.Add(txtSapDocNum);
        pnlSapDocument.Controls.Add(lblSapCurrencyCaption);
        pnlSapDocument.Controls.Add(txtSapCurrency);
        pnlSapDocument.Controls.Add(lblSapTotalCaption);
        pnlSapDocument.Controls.Add(txtSapTotal);
        pnlSapDocument.Location = new Point(410, 10);
        pnlSapDocument.Name = "pnlSapDocument";
        pnlSapDocument.Size = new Size(390, 177);
        pnlSapDocument.TabIndex = 1;
        // 
        // lblSapDocumentTitle
        // 
        lblSapDocumentTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapDocumentTitle.Appearance.ForeColor = Color.FromArgb(0, 92, 255);
        lblSapDocumentTitle.Appearance.Options.UseFont = true;
        lblSapDocumentTitle.Appearance.Options.UseForeColor = true;
        lblSapDocumentTitle.Location = new Point(0, 0);
        lblSapDocumentTitle.Name = "lblSapDocumentTitle";
        lblSapDocumentTitle.Size = new Size(111, 20);
        lblSapDocumentTitle.TabIndex = 0;
        lblSapDocumentTitle.Text = "Documento SAP";
        // 
        // lblSapDocEntryCaption
        // 
        lblSapDocEntryCaption.Location = new Point(8, 42);
        lblSapDocEntryCaption.Name = "lblSapDocEntryCaption";
        lblSapDocEntryCaption.Size = new Size(70, 13);
        lblSapDocEntryCaption.TabIndex = 1;
        lblSapDocEntryCaption.Text = "DocEntry SAP:";
        // 
        // txtSapDocEntry
        // 
        txtSapDocEntry.Location = new Point(140, 39);
        txtSapDocEntry.Name = "txtSapDocEntry";
        txtSapDocEntry.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapDocEntry.Properties.Appearance.Options.UseFont = true;
        txtSapDocEntry.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        txtSapDocEntry.Properties.Appearance.Options.UseTextOptions = true;
        txtSapDocEntry.Properties.ReadOnly = true;
        txtSapDocEntry.Size = new Size(230, 22);
        txtSapDocEntry.TabIndex = 2;
        // 
        // lblSapDocNumCaption
        // 
        lblSapDocNumCaption.Location = new Point(8, 68);
        lblSapDocNumCaption.Name = "lblSapDocNumCaption";
        lblSapDocNumCaption.Size = new Size(65, 13);
        lblSapDocNumCaption.TabIndex = 3;
        lblSapDocNumCaption.Text = "DocNum SAP:";
        // 
        // txtSapDocNum
        // 
        txtSapDocNum.Location = new Point(140, 65);
        txtSapDocNum.Name = "txtSapDocNum";
        txtSapDocNum.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapDocNum.Properties.Appearance.Options.UseFont = true;
        txtSapDocNum.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        txtSapDocNum.Properties.Appearance.Options.UseTextOptions = true;
        txtSapDocNum.Properties.ReadOnly = true;
        txtSapDocNum.Size = new Size(230, 22);
        txtSapDocNum.TabIndex = 4;
        // 
        // lblSapCurrencyCaption
        // 
        lblSapCurrencyCaption.Location = new Point(8, 94);
        lblSapCurrencyCaption.Name = "lblSapCurrencyCaption";
        lblSapCurrencyCaption.Size = new Size(64, 13);
        lblSapCurrencyCaption.TabIndex = 5;
        lblSapCurrencyCaption.Text = "Moneda SAP:";
        // 
        // txtSapCurrency
        // 
        txtSapCurrency.Location = new Point(140, 91);
        txtSapCurrency.Name = "txtSapCurrency";
        txtSapCurrency.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapCurrency.Properties.Appearance.Options.UseFont = true;
        txtSapCurrency.Properties.ReadOnly = true;
        txtSapCurrency.Size = new Size(230, 22);
        txtSapCurrency.TabIndex = 6;
        // 
        // lblSapTotalCaption
        // 
        lblSapTotalCaption.Location = new Point(8, 120);
        lblSapTotalCaption.Name = "lblSapTotalCaption";
        lblSapTotalCaption.Size = new Size(50, 13);
        lblSapTotalCaption.TabIndex = 7;
        lblSapTotalCaption.Text = "Total SAP:";
        // 
        // txtSapTotal
        // 
        txtSapTotal.Location = new Point(140, 117);
        txtSapTotal.Name = "txtSapTotal";
        txtSapTotal.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        txtSapTotal.Properties.Appearance.Options.UseFont = true;
        txtSapTotal.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        txtSapTotal.Properties.Appearance.Options.UseTextOptions = true;
        txtSapTotal.Properties.ReadOnly = true;
        txtSapTotal.Size = new Size(230, 22);
        txtSapTotal.TabIndex = 8;
        // 
        // pnlSapMessages
        // 
        pnlSapMessages.BorderStyle = BorderStyles.NoBorder;
        pnlSapMessages.Controls.Add(lblSapMessagesTitle);
        pnlSapMessages.Controls.Add(memoSapMessage);
        pnlSapMessages.Location = new Point(812, 10);
        pnlSapMessages.Name = "pnlSapMessages";
        pnlSapMessages.Size = new Size(506, 177);
        pnlSapMessages.TabIndex = 2;
        // 
        // lblSapMessagesTitle
        // 
        lblSapMessagesTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapMessagesTitle.Appearance.ForeColor = Color.FromArgb(0, 92, 255);
        lblSapMessagesTitle.Appearance.Options.UseFont = true;
        lblSapMessagesTitle.Appearance.Options.UseForeColor = true;
        lblSapMessagesTitle.Location = new Point(0, 0);
        lblSapMessagesTitle.Name = "lblSapMessagesTitle";
        lblSapMessagesTitle.Size = new Size(129, 20);
        lblSapMessagesTitle.TabIndex = 0;
        lblSapMessagesTitle.Text = "Mensajes / Respuesta";
        // 
        // memoSapMessage
        // 
        memoSapMessage.Location = new Point(0, 32);
        memoSapMessage.Name = "memoSapMessage";
        memoSapMessage.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        memoSapMessage.Properties.Appearance.Options.UseFont = true;
        memoSapMessage.Properties.ReadOnly = true;
        memoSapMessage.Size = new Size(506, 132);
        memoSapMessage.TabIndex = 1;
        // 
        // lblSapLogsTitle
        // 
        lblSapLogsTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSapLogsTitle.Appearance.ForeColor = Color.FromArgb(0, 92, 255);
        lblSapLogsTitle.Appearance.Options.UseFont = true;
        lblSapLogsTitle.Appearance.Options.UseForeColor = true;
        lblSapLogsTitle.Location = new Point(18, 276);
        lblSapLogsTitle.Name = "lblSapLogsTitle";
        lblSapLogsTitle.Size = new Size(190, 20);
        lblSapLogsTitle.TabIndex = 3;
        lblSapLogsTitle.Text = "Historial de Intentos de Integración";
        // 
        // gridSapLogs
        // 
        gridSapLogs.Location = new Point(18, 302);
        gridSapLogs.MainView = viewSapLogs;
        gridSapLogs.Name = "gridSapLogs";
        gridSapLogs.Size = new Size(1300, 72);
        gridSapLogs.TabIndex = 4;
        gridSapLogs.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { viewSapLogs });
        // 
        // viewSapLogs
        // 
        viewSapLogs.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        viewSapLogs.Appearance.HeaderPanel.Options.UseFont = true;
        viewSapLogs.Appearance.Row.Font = new Font("Segoe UI", 9F);
        viewSapLogs.Appearance.Row.Options.UseFont = true;
        viewSapLogs.Columns.AddRange(new GridColumn[] { colSapLogCreatedAt, colSapLogProcess, colSapLogStatus, colSapLogMessage, colSapLogUser, colSapLogAttempt });
        viewSapLogs.GridControl = gridSapLogs;
        viewSapLogs.Name = "viewSapLogs";
        viewSapLogs.OptionsBehavior.Editable = false;
        viewSapLogs.OptionsView.ShowGroupPanel = false;
        // 
        // colSapLogCreatedAt
        // 
        colSapLogCreatedAt.Caption = "Fecha y Hora";
        colSapLogCreatedAt.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
        colSapLogCreatedAt.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        colSapLogCreatedAt.FieldName = "CreatedAt";
        colSapLogCreatedAt.Name = "colSapLogCreatedAt";
        colSapLogCreatedAt.Visible = true;
        colSapLogCreatedAt.VisibleIndex = 0;
        colSapLogCreatedAt.Width = 180;
        // 
        // colSapLogProcess
        // 
        colSapLogProcess.Caption = "Proceso";
        colSapLogProcess.FieldName = "Process";
        colSapLogProcess.Name = "colSapLogProcess";
        colSapLogProcess.Visible = true;
        colSapLogProcess.VisibleIndex = 1;
        colSapLogProcess.Width = 120;
        // 
        // colSapLogStatus
        // 
        colSapLogStatus.Caption = "Estado";
        colSapLogStatus.FieldName = "Status";
        colSapLogStatus.Name = "colSapLogStatus";
        colSapLogStatus.Visible = true;
        colSapLogStatus.VisibleIndex = 2;
        colSapLogStatus.Width = 120;
        // 
        // colSapLogMessage
        // 
        colSapLogMessage.Caption = "Mensaje / Respuesta";
        colSapLogMessage.FieldName = "Message";
        colSapLogMessage.Name = "colSapLogMessage";
        colSapLogMessage.Visible = true;
        colSapLogMessage.VisibleIndex = 3;
        colSapLogMessage.Width = 560;
        // 
        // colSapLogUser
        // 
        colSapLogUser.Caption = "Usuario";
        colSapLogUser.FieldName = "UserName";
        colSapLogUser.Name = "colSapLogUser";
        colSapLogUser.Visible = true;
        colSapLogUser.VisibleIndex = 4;
        colSapLogUser.Width = 150;
        // 
        // colSapLogAttempt
        // 
        colSapLogAttempt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        colSapLogAttempt.Caption = "Intento";
        colSapLogAttempt.FieldName = "AttemptNumber";
        colSapLogAttempt.Name = "colSapLogAttempt";
        colSapLogAttempt.Visible = true;
        colSapLogAttempt.VisibleIndex = 5;
        colSapLogAttempt.Width = 80;
        // tabAttachments
        // 
        tabAttachments.Controls.Add(btnAddAttachment);
        tabAttachments.Controls.Add(btnDownloadAttachment);
        tabAttachments.Controls.Add(btnOpenAttachment);
        tabAttachments.Controls.Add(btnRemoveAttachment);
        tabAttachments.Controls.Add(btnRefreshAttachments);
        tabAttachments.Controls.Add(gridAttachments);
        tabAttachments.Controls.Add(lblAttachmentFooterCount);
        tabAttachments.Controls.Add(lblAttachmentFooterSize);
        tabAttachments.Controls.Add(pnlAttachmentPreview);
        tabAttachments.Name = "tabAttachments";
        tabAttachments.Size = new Size(1326, 389);
        tabAttachments.Text = "Anexos";
        // 
        // btnAddAttachment
        // 
        btnAddAttachment.Location = new Point(18, 16);
        btnAddAttachment.Name = "btnAddAttachment";
        btnAddAttachment.Size = new Size(110, 30);
        btnAddAttachment.TabIndex = 0;
        btnAddAttachment.Text = "Agregar";
        // 
        // btnDownloadAttachment
        // 
        btnDownloadAttachment.Location = new Point(138, 16);
        btnDownloadAttachment.Name = "btnDownloadAttachment";
        btnDownloadAttachment.Size = new Size(110, 30);
        btnDownloadAttachment.TabIndex = 1;
        btnDownloadAttachment.Text = "Descargar";
        // 
        // btnOpenAttachment
        // 
        btnOpenAttachment.Location = new Point(258, 16);
        btnOpenAttachment.Name = "btnOpenAttachment";
        btnOpenAttachment.Size = new Size(110, 30);
        btnOpenAttachment.TabIndex = 2;
        btnOpenAttachment.Text = "Abrir";
        // 
        // btnRemoveAttachment
        // 
        btnRemoveAttachment.Location = new Point(378, 16);
        btnRemoveAttachment.Name = "btnRemoveAttachment";
        btnRemoveAttachment.Size = new Size(110, 30);
        btnRemoveAttachment.TabIndex = 3;
        btnRemoveAttachment.Text = "Eliminar";
        // 
        // btnRefreshAttachments
        // 
        btnRefreshAttachments.Location = new Point(498, 16);
        btnRefreshAttachments.Name = "btnRefreshAttachments";
        btnRefreshAttachments.Size = new Size(110, 30);
        btnRefreshAttachments.TabIndex = 4;
        btnRefreshAttachments.Text = "Actualizar";
        // 
        // gridAttachments
        // 
        gridAttachments.Location = new Point(18, 54);
        gridAttachments.MainView = viewAttachments;
        gridAttachments.Name = "gridAttachments";
        gridAttachments.Size = new Size(910, 292);
        gridAttachments.TabIndex = 5;
        gridAttachments.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { viewAttachments });
        // 
        // viewAttachments
        // 
        viewAttachments.Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
        viewAttachments.Appearance.HeaderPanel.Options.UseFont = true;
        viewAttachments.Appearance.Row.Font = new Font("Segoe UI", 9F);
        viewAttachments.Appearance.Row.Options.UseFont = true;
        viewAttachments.Columns.AddRange(new GridColumn[] { colAttachmentIcon, colAttachmentFileName, colAttachmentType, colAttachmentSize, colAttachmentCreatedAt, colAttachmentUser, colAttachmentStatus, colAttachmentComment });
        viewAttachments.GridControl = gridAttachments;
        viewAttachments.Name = "viewAttachments";
        viewAttachments.OptionsBehavior.Editable = false;
        viewAttachments.OptionsView.ShowGroupPanel = false;
        // 
        // colAttachmentIcon
        // 
        colAttachmentIcon.Caption = "#";
        colAttachmentIcon.FieldName = "FileExtension";
        colAttachmentIcon.Name = "colAttachmentIcon";
        colAttachmentIcon.Visible = true;
        colAttachmentIcon.VisibleIndex = 0;
        colAttachmentIcon.Width = 42;
        // 
        // colAttachmentFileName
        // 
        colAttachmentFileName.Caption = "Archivo";
        colAttachmentFileName.FieldName = "OriginalFileName";
        colAttachmentFileName.Name = "colAttachmentFileName";
        colAttachmentFileName.Visible = true;
        colAttachmentFileName.VisibleIndex = 1;
        colAttachmentFileName.Width = 245;
        // 
        // colAttachmentType
        // 
        colAttachmentType.Caption = "Tipo";
        colAttachmentType.FieldName = "MimeType";
        colAttachmentType.Name = "colAttachmentType";
        colAttachmentType.Visible = true;
        colAttachmentType.VisibleIndex = 2;
        colAttachmentType.Width = 130;
        // 
        // colAttachmentSize
        // 
        colAttachmentSize.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        colAttachmentSize.Caption = "Tamaño";
        colAttachmentSize.DisplayFormat.FormatString = "N0";
        colAttachmentSize.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
        colAttachmentSize.FieldName = "FileSize";
        colAttachmentSize.Name = "colAttachmentSize";
        colAttachmentSize.Visible = true;
        colAttachmentSize.VisibleIndex = 3;
        colAttachmentSize.Width = 78;
        // 
        // colAttachmentCreatedAt
        // 
        colAttachmentCreatedAt.Caption = "Fecha";
        colAttachmentCreatedAt.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm";
        colAttachmentCreatedAt.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
        colAttachmentCreatedAt.FieldName = "CreatedAt";
        colAttachmentCreatedAt.Name = "colAttachmentCreatedAt";
        colAttachmentCreatedAt.Visible = true;
        colAttachmentCreatedAt.VisibleIndex = 4;
        colAttachmentCreatedAt.Width = 125;
        // 
        // colAttachmentUser
        // 
        colAttachmentUser.Caption = "Usuario";
        colAttachmentUser.FieldName = "CreatedByUserName";
        colAttachmentUser.Name = "colAttachmentUser";
        colAttachmentUser.Visible = true;
        colAttachmentUser.VisibleIndex = 5;
        colAttachmentUser.Width = 95;
        // 
        // colAttachmentStatus
        // 
        colAttachmentStatus.Caption = "Estado";
        colAttachmentStatus.FieldName = "Status";
        colAttachmentStatus.Name = "colAttachmentStatus";
        colAttachmentStatus.Visible = true;
        colAttachmentStatus.VisibleIndex = 6;
        colAttachmentStatus.Width = 78;
        // 
        // colAttachmentComment
        // 
        colAttachmentComment.Caption = "Comentario";
        colAttachmentComment.FieldName = "Comment";
        colAttachmentComment.Name = "colAttachmentComment";
        colAttachmentComment.Visible = true;
        colAttachmentComment.VisibleIndex = 7;
        colAttachmentComment.Width = 168;
        // 
        // lblAttachmentFooterCount
        // 
        lblAttachmentFooterCount.Location = new Point(18, 358);
        lblAttachmentFooterCount.Name = "lblAttachmentFooterCount";
        lblAttachmentFooterCount.Size = new Size(98, 13);
        lblAttachmentFooterCount.TabIndex = 6;
        lblAttachmentFooterCount.Text = "Total de archivos: 0";
        // 
        // lblAttachmentFooterSize
        // 
        lblAttachmentFooterSize.Appearance.Options.UseTextOptions = true;
        lblAttachmentFooterSize.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblAttachmentFooterSize.AutoSizeMode = LabelAutoSizeMode.None;
        lblAttachmentFooterSize.Location = new Point(690, 358);
        lblAttachmentFooterSize.Name = "lblAttachmentFooterSize";
        lblAttachmentFooterSize.Size = new Size(238, 18);
        lblAttachmentFooterSize.TabIndex = 7;
        lblAttachmentFooterSize.Text = "Tamaño total: 0 KB";
        // 
        // pnlAttachmentPreview
        // 
        pnlAttachmentPreview.BorderStyle = BorderStyles.NoBorder;
        pnlAttachmentPreview.Controls.Add(lblAttachmentPreviewTitle);
        pnlAttachmentPreview.Controls.Add(lblAttachmentTypeCaption);
        pnlAttachmentPreview.Controls.Add(lblAttachmentTypeValue);
        pnlAttachmentPreview.Controls.Add(lblAttachmentSizeCaption);
        pnlAttachmentPreview.Controls.Add(lblAttachmentSizeValue);
        pnlAttachmentPreview.Controls.Add(lblAttachmentDateCaption);
        pnlAttachmentPreview.Controls.Add(lblAttachmentDateValue);
        pnlAttachmentPreview.Controls.Add(lblAttachmentUserCaption);
        pnlAttachmentPreview.Controls.Add(lblAttachmentUserValue);
        pnlAttachmentPreview.Controls.Add(lblAttachmentStatusCaption);
        pnlAttachmentPreview.Controls.Add(lblAttachmentStatusValue);
        pnlAttachmentPreview.Controls.Add(lblAttachmentCommentCaption);
        pnlAttachmentPreview.Controls.Add(lblAttachmentCommentValue);
        pnlAttachmentPreview.Controls.Add(picAttachmentPreview);
        pnlAttachmentPreview.Location = new Point(945, 54);
        pnlAttachmentPreview.Name = "pnlAttachmentPreview";
        pnlAttachmentPreview.Size = new Size(373, 322);
        pnlAttachmentPreview.TabIndex = 8;
        // 
        // lblAttachmentPreviewTitle
        // 
        lblAttachmentPreviewTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblAttachmentPreviewTitle.Appearance.ForeColor = Color.FromArgb(0, 92, 255);
        lblAttachmentPreviewTitle.Appearance.Options.UseFont = true;
        lblAttachmentPreviewTitle.Appearance.Options.UseForeColor = true;
        lblAttachmentPreviewTitle.AutoSizeMode = LabelAutoSizeMode.None;
        lblAttachmentPreviewTitle.Location = new Point(0, 0);
        lblAttachmentPreviewTitle.Name = "lblAttachmentPreviewTitle";
        lblAttachmentPreviewTitle.Size = new Size(360, 24);
        lblAttachmentPreviewTitle.TabIndex = 0;
        lblAttachmentPreviewTitle.Text = "Archivo seleccionado";
        // 
        // lblAttachmentTypeCaption
        // 
        lblAttachmentTypeCaption.Location = new Point(0, 36);
        lblAttachmentTypeCaption.Name = "lblAttachmentTypeCaption";
        lblAttachmentTypeCaption.Size = new Size(25, 13);
        lblAttachmentTypeCaption.TabIndex = 1;
        lblAttachmentTypeCaption.Text = "Tipo:";
        // 
        // lblAttachmentTypeValue
        // 
        lblAttachmentTypeValue.Location = new Point(88, 36);
        lblAttachmentTypeValue.Name = "lblAttachmentTypeValue";
        lblAttachmentTypeValue.Size = new Size(4, 13);
        lblAttachmentTypeValue.TabIndex = 2;
        lblAttachmentTypeValue.Text = "-";
        // 
        // lblAttachmentSizeCaption
        // 
        lblAttachmentSizeCaption.Location = new Point(0, 54);
        lblAttachmentSizeCaption.Name = "lblAttachmentSizeCaption";
        lblAttachmentSizeCaption.Size = new Size(44, 13);
        lblAttachmentSizeCaption.TabIndex = 3;
        lblAttachmentSizeCaption.Text = "Tamaño:";
        // 
        // lblAttachmentSizeValue
        // 
        lblAttachmentSizeValue.Location = new Point(88, 54);
        lblAttachmentSizeValue.Name = "lblAttachmentSizeValue";
        lblAttachmentSizeValue.Size = new Size(4, 13);
        lblAttachmentSizeValue.TabIndex = 4;
        lblAttachmentSizeValue.Text = "-";
        // 
        // lblAttachmentDateCaption
        // 
        lblAttachmentDateCaption.Location = new Point(0, 72);
        lblAttachmentDateCaption.Name = "lblAttachmentDateCaption";
        lblAttachmentDateCaption.Size = new Size(33, 13);
        lblAttachmentDateCaption.TabIndex = 5;
        lblAttachmentDateCaption.Text = "Fecha:";
        // 
        // lblAttachmentDateValue
        // 
        lblAttachmentDateValue.Location = new Point(88, 72);
        lblAttachmentDateValue.Name = "lblAttachmentDateValue";
        lblAttachmentDateValue.Size = new Size(4, 13);
        lblAttachmentDateValue.TabIndex = 6;
        lblAttachmentDateValue.Text = "-";
        // 
        // lblAttachmentUserCaption
        // 
        lblAttachmentUserCaption.Location = new Point(0, 90);
        lblAttachmentUserCaption.Name = "lblAttachmentUserCaption";
        lblAttachmentUserCaption.Size = new Size(40, 13);
        lblAttachmentUserCaption.TabIndex = 7;
        lblAttachmentUserCaption.Text = "Usuario:";
        // 
        // lblAttachmentUserValue
        // 
        lblAttachmentUserValue.Location = new Point(88, 90);
        lblAttachmentUserValue.Name = "lblAttachmentUserValue";
        lblAttachmentUserValue.Size = new Size(4, 13);
        lblAttachmentUserValue.TabIndex = 8;
        lblAttachmentUserValue.Text = "-";
        // 
        // lblAttachmentStatusCaption
        // 
        lblAttachmentStatusCaption.Location = new Point(0, 108);
        lblAttachmentStatusCaption.Name = "lblAttachmentStatusCaption";
        lblAttachmentStatusCaption.Size = new Size(37, 13);
        lblAttachmentStatusCaption.TabIndex = 9;
        lblAttachmentStatusCaption.Text = "Estado:";
        // 
        // lblAttachmentStatusValue
        // 
        lblAttachmentStatusValue.Location = new Point(88, 108);
        lblAttachmentStatusValue.Name = "lblAttachmentStatusValue";
        lblAttachmentStatusValue.Size = new Size(4, 13);
        lblAttachmentStatusValue.TabIndex = 10;
        lblAttachmentStatusValue.Text = "-";
        // 
        // lblAttachmentCommentCaption
        // 
        lblAttachmentCommentCaption.Location = new Point(0, 126);
        lblAttachmentCommentCaption.Name = "lblAttachmentCommentCaption";
        lblAttachmentCommentCaption.Size = new Size(60, 13);
        lblAttachmentCommentCaption.TabIndex = 11;
        lblAttachmentCommentCaption.Text = "Comentario:";
        // 
        // lblAttachmentCommentValue
        // 
        lblAttachmentCommentValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblAttachmentCommentValue.Location = new Point(88, 126);
        lblAttachmentCommentValue.Name = "lblAttachmentCommentValue";
        lblAttachmentCommentValue.Size = new Size(280, 34);
        lblAttachmentCommentValue.TabIndex = 12;
        lblAttachmentCommentValue.Text = "-";
        // 
        // picAttachmentPreview
        // 
        picAttachmentPreview.Location = new Point(0, 172);
        picAttachmentPreview.Name = "picAttachmentPreview";
        picAttachmentPreview.Properties.BorderStyle = BorderStyles.Simple;
        picAttachmentPreview.Properties.SizeMode = PictureSizeMode.Zoom;
        picAttachmentPreview.Size = new Size(373, 150);
        picAttachmentPreview.TabIndex = 13;
        // 
        // pnlSummary
        // 
        pnlSummary.Appearance.BackColor = Color.White;
        pnlSummary.Appearance.Options.UseBackColor = true;
        pnlSummary.Controls.Add(lblSummaryTitle);
        pnlSummary.Controls.Add(lblSummarySubtotalCaption);
        pnlSummary.Controls.Add(lblSummarySubtotal);
        pnlSummary.Controls.Add(lblSummaryDiscountCaption);
        pnlSummary.Controls.Add(lblSummaryDiscount);
        pnlSummary.Controls.Add(lblSummaryBaseCaption);
        pnlSummary.Controls.Add(lblSummaryBase);
        pnlSummary.Controls.Add(lblSummaryTaxCaption);
        pnlSummary.Controls.Add(lblSummaryTax);
        pnlSummary.Controls.Add(lblSummaryTotalCaption);
        pnlSummary.Controls.Add(lblSummaryTotal);
        pnlSummary.Controls.Add(lblSummaryItemsCaption);
        pnlSummary.Controls.Add(lblSummaryItems);
        pnlSummary.Controls.Add(lblSummaryQuantityCaption);
        pnlSummary.Controls.Add(lblSummaryQuantity);
        pnlSummary.Controls.Add(lblSummaryWeightCaption);
        pnlSummary.Controls.Add(lblSummaryWeight);
        pnlSummary.Location = new Point(1348, 214);
        pnlSummary.Name = "pnlSummary";
        pnlSummary.Size = new Size(238, 494);
        pnlSummary.TabIndex = 33;
        // 
        // lblSummaryTitle
        // 
        lblSummaryTitle.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSummaryTitle.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSummaryTitle.Appearance.Options.UseFont = true;
        lblSummaryTitle.Appearance.Options.UseForeColor = true;
        lblSummaryTitle.Location = new Point(18, 18);
        lblSummaryTitle.Name = "lblSummaryTitle";
        lblSummaryTitle.Size = new Size(146, 20);
        lblSummaryTitle.TabIndex = 0;
        lblSummaryTitle.Text = "Resumen de la Orden";
        // 
        // lblSummarySubtotalCaption
        // 
        lblSummarySubtotalCaption.Location = new Point(18, 60);
        lblSummarySubtotalCaption.Name = "lblSummarySubtotalCaption";
        lblSummarySubtotalCaption.Size = new Size(44, 13);
        lblSummarySubtotalCaption.TabIndex = 1;
        lblSummarySubtotalCaption.Text = "Subtotal:";
        // 
        // lblSummarySubtotal
        // 
        lblSummarySubtotal.Appearance.Options.UseTextOptions = true;
        lblSummarySubtotal.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblSummarySubtotal.AutoSizeMode = LabelAutoSizeMode.None;
        lblSummarySubtotal.Location = new Point(128, 60);
        lblSummarySubtotal.Name = "lblSummarySubtotal";
        lblSummarySubtotal.Size = new Size(90, 20);
        lblSummarySubtotal.TabIndex = 2;
        lblSummarySubtotal.Text = "0.00";
        // 
        // lblSummaryDiscountCaption
        // 
        lblSummaryDiscountCaption.Location = new Point(18, 95);
        lblSummaryDiscountCaption.Name = "lblSummaryDiscountCaption";
        lblSummaryDiscountCaption.Size = new Size(87, 13);
        lblSummaryDiscountCaption.TabIndex = 3;
        lblSummaryDiscountCaption.Text = "Descuento Global:";
        // 
        // lblSummaryDiscount
        // 
        lblSummaryDiscount.Appearance.Options.UseTextOptions = true;
        lblSummaryDiscount.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblSummaryDiscount.AutoSizeMode = LabelAutoSizeMode.None;
        lblSummaryDiscount.Location = new Point(128, 95);
        lblSummaryDiscount.Name = "lblSummaryDiscount";
        lblSummaryDiscount.Size = new Size(90, 20);
        lblSummaryDiscount.TabIndex = 4;
        lblSummaryDiscount.Text = "0.00";
        // 
        // lblSummaryBaseCaption
        // 
        lblSummaryBaseCaption.Location = new Point(18, 130);
        lblSummaryBaseCaption.Name = "lblSummaryBaseCaption";
        lblSummaryBaseCaption.Size = new Size(76, 13);
        lblSummaryBaseCaption.TabIndex = 5;
        lblSummaryBaseCaption.Text = "Base Imponible:";
        // 
        // lblSummaryBase
        // 
        lblSummaryBase.Appearance.Options.UseTextOptions = true;
        lblSummaryBase.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblSummaryBase.AutoSizeMode = LabelAutoSizeMode.None;
        lblSummaryBase.Location = new Point(128, 130);
        lblSummaryBase.Name = "lblSummaryBase";
        lblSummaryBase.Size = new Size(90, 20);
        lblSummaryBase.TabIndex = 6;
        lblSummaryBase.Text = "0.00";
        // 
        // lblSummaryTaxCaption
        // 
        lblSummaryTaxCaption.Location = new Point(18, 165);
        lblSummaryTaxCaption.Name = "lblSummaryTaxCaption";
        lblSummaryTaxCaption.Size = new Size(55, 13);
        lblSummaryTaxCaption.TabIndex = 7;
        lblSummaryTaxCaption.Text = "IVA (12%):";
        // 
        // lblSummaryTax
        // 
        lblSummaryTax.Appearance.Options.UseTextOptions = true;
        lblSummaryTax.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblSummaryTax.AutoSizeMode = LabelAutoSizeMode.None;
        lblSummaryTax.Location = new Point(128, 165);
        lblSummaryTax.Name = "lblSummaryTax";
        lblSummaryTax.Size = new Size(90, 20);
        lblSummaryTax.TabIndex = 8;
        lblSummaryTax.Text = "0.00";
        // 
        // lblSummaryTotalCaption
        // 
        lblSummaryTotalCaption.Appearance.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        lblSummaryTotalCaption.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSummaryTotalCaption.Appearance.Options.UseFont = true;
        lblSummaryTotalCaption.Appearance.Options.UseForeColor = true;
        lblSummaryTotalCaption.Location = new Point(18, 215);
        lblSummaryTotalCaption.Name = "lblSummaryTotalCaption";
        lblSummaryTotalCaption.Size = new Size(38, 20);
        lblSummaryTotalCaption.TabIndex = 9;
        lblSummaryTotalCaption.Text = "Total:";
        // 
        // lblSummaryTotal
        // 
        lblSummaryTotal.Appearance.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        lblSummaryTotal.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblSummaryTotal.Appearance.Options.UseFont = true;
        lblSummaryTotal.Appearance.Options.UseForeColor = true;
        lblSummaryTotal.Appearance.Options.UseTextOptions = true;
        lblSummaryTotal.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblSummaryTotal.AutoSizeMode = LabelAutoSizeMode.None;
        lblSummaryTotal.Location = new Point(108, 208);
        lblSummaryTotal.Name = "lblSummaryTotal";
        lblSummaryTotal.Size = new Size(110, 30);
        lblSummaryTotal.TabIndex = 10;
        lblSummaryTotal.Text = "0.00";
        // 
        // lblSummaryItemsCaption
        // 
        lblSummaryItemsCaption.Location = new Point(18, 285);
        lblSummaryItemsCaption.Name = "lblSummaryItemsCaption";
        lblSummaryItemsCaption.Size = new Size(58, 13);
        lblSummaryItemsCaption.TabIndex = 11;
        lblSummaryItemsCaption.Text = "Total Items:";
        // 
        // lblSummaryItems
        // 
        lblSummaryItems.Appearance.Options.UseTextOptions = true;
        lblSummaryItems.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblSummaryItems.AutoSizeMode = LabelAutoSizeMode.None;
        lblSummaryItems.Location = new Point(128, 285);
        lblSummaryItems.Name = "lblSummaryItems";
        lblSummaryItems.Size = new Size(90, 20);
        lblSummaryItems.TabIndex = 12;
        lblSummaryItems.Text = "0";
        // 
        // lblSummaryQuantityCaption
        // 
        lblSummaryQuantityCaption.Location = new Point(18, 320);
        lblSummaryQuantityCaption.Name = "lblSummaryQuantityCaption";
        lblSummaryQuantityCaption.Size = new Size(74, 13);
        lblSummaryQuantityCaption.TabIndex = 13;
        lblSummaryQuantityCaption.Text = "Cantidad Total:";
        // 
        // lblSummaryQuantity
        // 
        lblSummaryQuantity.Appearance.Options.UseTextOptions = true;
        lblSummaryQuantity.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblSummaryQuantity.AutoSizeMode = LabelAutoSizeMode.None;
        lblSummaryQuantity.Location = new Point(128, 320);
        lblSummaryQuantity.Name = "lblSummaryQuantity";
        lblSummaryQuantity.Size = new Size(90, 20);
        lblSummaryQuantity.TabIndex = 14;
        lblSummaryQuantity.Text = "0.00";
        // 
        // lblSummaryWeightCaption
        // 
        lblSummaryWeightCaption.Location = new Point(18, 355);
        lblSummaryWeightCaption.Name = "lblSummaryWeightCaption";
        lblSummaryWeightCaption.Size = new Size(77, 13);
        lblSummaryWeightCaption.TabIndex = 15;
        lblSummaryWeightCaption.Text = "Peso Total (Kg):";
        // 
        // lblSummaryWeight
        // 
        lblSummaryWeight.Appearance.Options.UseTextOptions = true;
        lblSummaryWeight.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
        lblSummaryWeight.AutoSizeMode = LabelAutoSizeMode.None;
        lblSummaryWeight.Location = new Point(128, 355);
        lblSummaryWeight.Name = "lblSummaryWeight";
        lblSummaryWeight.Size = new Size(90, 20);
        lblSummaryWeight.TabIndex = 16;
        lblSummaryWeight.Text = "0.00";
        // 
        // pnlFooter
        // 
        pnlFooter.Appearance.BackColor = Color.White;
        pnlFooter.Appearance.Options.UseBackColor = true;
        pnlFooter.BorderStyle = BorderStyles.NoBorder;
        pnlFooter.Controls.Add(btnSave);
        pnlFooter.Controls.Add(btnCancel);
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Location = new Point(0, 828);
        pnlFooter.Name = "pnlFooter";
        pnlFooter.Size = new Size(1600, 72);
        pnlFooter.TabIndex = 2;
        // 
        // btnSave
        // 
        btnSave.Appearance.BackColor = Color.FromArgb(0, 184, 148);
        btnSave.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        btnSave.Appearance.ForeColor = Color.White;
        btnSave.Appearance.Options.UseBackColor = true;
        btnSave.Appearance.Options.UseFont = true;
        btnSave.Appearance.Options.UseForeColor = true;
        btnSave.Location = new Point(545, 14);
        btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
        btnSave.Name = "btnSave";
        btnSave.Size = new Size(224, 48);
        btnSave.TabIndex = 0;
        btnSave.Text = "Guardar";
        // 
        // btnCancel
        // 
        btnCancel.Appearance.BorderColor = Color.FromArgb(255, 64, 64);
        btnCancel.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        btnCancel.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        btnCancel.Appearance.Options.UseBorderColor = true;
        btnCancel.Appearance.Options.UseFont = true;
        btnCancel.Appearance.Options.UseForeColor = true;
        btnCancel.DialogResult = DialogResult.Cancel;
        btnCancel.Location = new Point(790, 14);
        btnCancel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        btnCancel.LookAndFeel.UseDefaultLookAndFeel = false;
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(224, 48);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "Cancelar";
        // 
        // pnlHeader
        // 
        pnlHeader.Appearance.BackColor = Color.White;
        pnlHeader.Appearance.Options.UseBackColor = true;
        pnlHeader.BorderStyle = BorderStyles.NoBorder;
        pnlHeader.Controls.Add(picLogo);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(lblDocumentNumber);
        pnlHeader.Controls.Add(lblSeriesCaption);
        pnlHeader.Controls.Add(lueDocumentSeries);
        pnlHeader.Controls.Add(lblSeriesValue);
        pnlHeader.Controls.Add(lblNumberCaption);
        pnlHeader.Controls.Add(lblNumberValue);
        pnlHeader.Controls.Add(lblStatus);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1600, 112);
        pnlHeader.TabIndex = 0;
        // 
        // lblStatus
        // 
        lblStatus.Appearance.BackColor = Color.FromArgb(214, 232, 255);
        lblStatus.Appearance.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
        lblStatus.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblStatus.Appearance.Options.UseBackColor = true;
        lblStatus.Appearance.Options.UseFont = true;
        lblStatus.Appearance.Options.UseForeColor = true;
        lblStatus.Appearance.Options.UseTextOptions = true;
        lblStatus.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        lblStatus.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        lblStatus.AutoSizeMode = LabelAutoSizeMode.None;
        lblStatus.Location = new Point(1440, 32);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(150, 42);
        lblStatus.TabIndex = 7;
        lblStatus.Text = "BORRADOR";
        // 
        // lblNumberValue
        // 
        lblNumberValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblNumberValue.Appearance.Options.UseFont = true;
        lblNumberValue.Appearance.Options.UseTextOptions = true;
        lblNumberValue.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        lblNumberValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblNumberValue.BorderStyle = BorderStyles.Simple;
        lblNumberValue.Location = new Point(1265, 39);
        lblNumberValue.Name = "lblNumberValue";
        lblNumberValue.Size = new Size(150, 32);
        lblNumberValue.TabIndex = 6;
        lblNumberValue.Text = "OC-000001";
        // 
        // lblNumberCaption
        // 
        lblNumberCaption.Location = new Point(1230, 46);
        lblNumberCaption.Name = "lblNumberCaption";
        lblNumberCaption.Size = new Size(21, 13);
        lblNumberCaption.TabIndex = 5;
        lblNumberCaption.Text = "No.:";
        // 
        // lueDocumentSeries
        // 
        lueDocumentSeries.Location = new Point(1070, 44);
        lueDocumentSeries.Name = "lueDocumentSeries";
        lueDocumentSeries.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        lueDocumentSeries.Properties.Appearance.Options.UseFont = true;
        lueDocumentSeries.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
        lueDocumentSeries.Properties.DisplayMember = "DisplayText";
        lueDocumentSeries.Properties.NullText = "";
        lueDocumentSeries.Properties.ValueMember = "Id";
        lueDocumentSeries.Size = new Size(135, 22);
        lueDocumentSeries.TabIndex = 8;
        lueDocumentSeries.Visible = false;
        // 
        // lblSeriesValue
        // 
        lblSeriesValue.Appearance.Font = new Font("Segoe UI", 9F);
        lblSeriesValue.Appearance.Options.UseFont = true;
        lblSeriesValue.Appearance.Options.UseTextOptions = true;
        lblSeriesValue.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        lblSeriesValue.AutoSizeMode = LabelAutoSizeMode.None;
        lblSeriesValue.BorderStyle = BorderStyles.Simple;
        lblSeriesValue.Location = new Point(1070, 39);
        lblSeriesValue.Name = "lblSeriesValue";
        lblSeriesValue.Size = new Size(135, 32);
        lblSeriesValue.TabIndex = 4;
        lblSeriesValue.Text = "OC-2026";
        // 
        // lblSeriesCaption
        // 
        lblSeriesCaption.Location = new Point(1030, 46);
        lblSeriesCaption.Name = "lblSeriesCaption";
        lblSeriesCaption.Size = new Size(28, 13);
        lblSeriesCaption.TabIndex = 3;
        lblSeriesCaption.Text = "Serie:";
        // 
        // lblDocumentNumber
        // 
        lblDocumentNumber.Appearance.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
        lblDocumentNumber.Appearance.ForeColor = Color.FromArgb(0, 184, 148);
        lblDocumentNumber.Appearance.Options.UseFont = true;
        lblDocumentNumber.Appearance.Options.UseForeColor = true;
        lblDocumentNumber.Location = new Point(728, 62);
        lblDocumentNumber.Name = "lblDocumentNumber";
        lblDocumentNumber.Size = new Size(98, 28);
        lblDocumentNumber.TabIndex = 2;
        lblDocumentNumber.Text = "OC-000001";
        // 
        // lblTitle
        // 
        lblTitle.Appearance.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
        lblTitle.Appearance.ForeColor = Color.FromArgb(23, 32, 51);
        lblTitle.Appearance.Options.UseFont = true;
        lblTitle.Appearance.Options.UseForeColor = true;
        lblTitle.Location = new Point(640, 24);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(251, 37);
        lblTitle.TabIndex = 1;
        lblTitle.Text = "ORDEN DE COMPRA";
        // 
        // picLogo
        // 
        picLogo.Location = new Point(20, 18);
        picLogo.Name = "picLogo";
        picLogo.Properties.AllowFocused = false;
        picLogo.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
        picLogo.Properties.SizeMode = PictureSizeMode.Zoom;
        picLogo.Size = new Size(210, 72);
        picLogo.TabIndex = 0;
        // 
        // FrmPurchaseOrderEdit
        // 
        Appearance.Options.UseFont = true;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1600, 900);
        Controls.Add(pnlMain);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);
        MinimumSize = new Size(1500, 860);
        Name = "FrmPurchaseOrderEdit";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Orden de compra";
        ((System.ComponentModel.ISupportInitialize)pnlMain).EndInit();
        pnlMain.ResumeLayout(false);
        pnlMain.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)slueSupplier.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvSupplierLookup).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierTaxId.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierContact.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierPhone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSupplierEmail.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)deDocumentDate.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)deDocumentDate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)deDeliveryDate.Properties.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)deDeliveryDate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePaymentTerm.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePriceList.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueBuyer.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueMainWarehouse.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueProject.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)lueCostCenter.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)luePurchaseType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memoComments.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)tabPurchaseOrder).EndInit();
        tabPurchaseOrder.ResumeLayout(false);
        tabDetail.ResumeLayout(false);
        tabDetail.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)gridLines).EndInit();
        ((System.ComponentModel.ISupportInitialize)viewLines).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)gvItemRepository).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoUnit).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoQuantity).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoMoney).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoTax).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoWarehouse).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoDeliveryDate.CalendarTimeProperties).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoDeliveryDate).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoCostCenter).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoProject).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlDetailTotals).EndInit();
        pnlDetailTotals.ResumeLayout(false);
        pnlDetailTotals.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)spnGlobalDiscountPercent.Properties).EndInit();
        tabAddresses.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlDeliveryAddress).EndInit();
        pnlDeliveryAddress.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)lueDeliveryAddress.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryAddressName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memoDeliveryStreet.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryReference.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryCity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryState.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryZipCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtDeliveryPhone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlBillingAddress).EndInit();
        pnlBillingAddress.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)lueBillingAddress.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingAddressName.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memoBillingStreet.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingReference.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingCity.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingState.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingZipCode.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingCountry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingPhone.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtBillingEmail.Properties).EndInit();
        tabApproval.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlApprovalAmountCard).EndInit();
        pnlApprovalAmountCard.ResumeLayout(false);
        pnlApprovalAmountCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalPolicyCard).EndInit();
        pnlApprovalPolicyCard.ResumeLayout(false);
        pnlApprovalPolicyCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalLevelCard).EndInit();
        pnlApprovalLevelCard.ResumeLayout(false);
        pnlApprovalLevelCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalStatusCard).EndInit();
        pnlApprovalStatusCard.ResumeLayout(false);
        pnlApprovalStatusCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)txtApprovalAmount.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtApprovalPolicy.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtApprovalLevel.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtApprovalStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridApprovals).EndInit();
        ((System.ComponentModel.ISupportInitialize)viewApprovals).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalComment).EndInit();
        pnlApprovalComment.ResumeLayout(false);
        pnlApprovalComment.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memoApprovalObservation.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlApprovalFlow).EndInit();
        pnlApprovalFlow.ResumeLayout(false);
        pnlApprovalFlow.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)gridApprovalFlow).EndInit();
        ((System.ComponentModel.ISupportInitialize)viewApprovalFlow).EndInit();
        tabRelatedDocuments.ResumeLayout(false);
        tabRelatedDocuments.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)gridRelatedDocuments).EndInit();
        ((System.ComponentModel.ISupportInitialize)viewRelatedDocuments).EndInit();
        ((System.ComponentModel.ISupportInitialize)repoRelatedDocumentAction).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlRelatedDocumentNotes).EndInit();
        pnlRelatedDocumentNotes.ResumeLayout(false);
        pnlRelatedDocumentNotes.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)memoRelatedDocumentNotes.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlSapSync).EndInit();
        pnlSapSync.ResumeLayout(false);
        pnlSapSync.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSapDocument).EndInit();
        pnlSapDocument.ResumeLayout(false);
        pnlSapDocument.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlSapMessages).EndInit();
        pnlSapMessages.ResumeLayout(false);
        pnlSapMessages.PerformLayout();
        tabSap.ResumeLayout(false);
        tabSap.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)txtSapStatus.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapSyncDocEntry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapSyncDocNum.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapObjectType.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapSyncDate.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapUser.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapLastError.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapDocEntry.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapDocNum.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapCurrency.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)txtSapTotal.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)memoSapMessage.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)gridSapLogs).EndInit();
        ((System.ComponentModel.ISupportInitialize)viewSapLogs).EndInit();
        tabAttachments.ResumeLayout(false);
        tabAttachments.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)gridAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)viewAttachments).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlAttachmentPreview).EndInit();
        pnlAttachmentPreview.ResumeLayout(false);
        pnlAttachmentPreview.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picAttachmentPreview.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)pnlSummary).EndInit();
        pnlSummary.ResumeLayout(false);
        pnlSummary.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pnlFooter).EndInit();
        pnlFooter.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pnlHeader).EndInit();
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)lueDocumentSeries.Properties).EndInit();
        ((System.ComponentModel.ISupportInitialize)picLogo.Properties).EndInit();
        ResumeLayout(false);
    }
    private PanelControl pnlHeader;
    private PictureEdit picLogo;
    private LabelControl lblTitle;
    private LabelControl lblDocumentNumber;
    private LabelControl lblSeriesCaption;
    private LookUpEdit lueDocumentSeries;
    private LabelControl lblSeriesValue;
    private LabelControl lblNumberCaption;
    private LabelControl lblNumberValue;
    private LabelControl lblStatus;
}

using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace NuanSystem.WinForms.Forms.InventoryItems
{
    partial class ItemEditForm
    {
        private IContainer components = null;
        private PanelControl pnlMain;
        private GroupControl grpHeader;
        private SplitContainerControl splContent;
        private XtraTabControl xtcMain;
        private XtraTabPage xtpGeneral;
        private XtraTabPage xtpInventory;
        private XtraTabPage xtpPurchases;
        private XtraTabPage xtpSales;
        private XtraTabPage xtpCosts;
        private XtraTabPage xtpSap;
        private PanelControl pnlSummary;
        private PanelControl pnlFooter;
        private LabelControl lblItemCode;
        private LabelControl lblBarCode;
        private LabelControl lblDescription;
        private LabelControl lblCommercialName;
        private LabelControl lblItemGroup;
        private LabelControl lblBrand;
        private LabelControl lblLine;
        private LabelControl lblUom;
        private LabelControl lblItemType;
        private LabelControl lblStatus;
        private TextEdit txtItemCode;
        private TextEdit txtBarCode;
        private TextEdit txtDescription;
        private TextEdit txtCommercialName;
        private SearchLookUpEdit sleItemGroup;
        private SearchLookUpEdit sleBrand;
        private SearchLookUpEdit sleLine;
        private SearchLookUpEdit sleHeaderUom;
        private GridView grvItemGroupLookup;
        private GridView grvBrandLookup;
        private GridView grvLineLookup;
        private GridView grvHeaderUomLookup;
        private LookUpEdit lueItemType;
        private LookUpEdit lueStatus;
        private CheckEdit chkInventoryItem;
        private CheckEdit chkPurchaseItem;
        private CheckEdit chkSalesItem;
        private PictureEdit picItem;
        private SimpleButton btnChangeImage;
        private SimpleButton btnRemoveImage;
        private GroupControl grpGeneralData;
        private GroupControl grpAdditionalInfo;
        private GroupControl grpAttributes;
        private GroupControl grpAdditionalImages;
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
        private MemoEdit memLongDescription;
        private SearchLookUpEdit sleCategory;
        private SearchLookUpEdit sleSubCategory;
        private SearchLookUpEdit sleManufacturer;
        private GridView grvCategoryLookup;
        private GridView grvSubCategoryLookup;
        private GridView grvManufacturerLookup;
        private TextEdit txtModel;
        private LookUpEdit lueCountry;
        private TextEdit txtAlternateCode;
        private SpinEdit sedWeight;
        private SpinEdit sedVolume;
        private SearchLookUpEdit slePurchaseUom;
        private SearchLookUpEdit sleSalesUom;
        private SearchLookUpEdit sleInventoryUom;
        private GridView grvPurchaseUomLookup;
        private GridView grvSalesUomLookup;
        private GridView grvInventoryUomLookup;
        private MemoEdit memGeneralNotes;
        private LabelControl lblSatCode;
        private LabelControl lblUnspscCode;
        private LabelControl lblTaxCode;
        private LabelControl lblTaxType;
        private LabelControl lblBaseCurrency;
        private LabelControl lblDefaultPriceList;
        private LabelControl lblMaxDiscount;
        private LabelControl lblLastChange;
        private LabelControl lblLastChangeUser;
        private LabelControl lblMaxDiscountSymbol;
        private TextEdit txtSatCode;
        private TextEdit txtUnspscCode;
        private TextEdit txtTaxCode;
        private LookUpEdit lueTaxType;
        private LookUpEdit lueBaseCurrency;
        private LookUpEdit lueDefaultPriceList;
        private SpinEdit sedMaxDiscount;
        private DateEdit dtpLastChange;
        private TextEdit txtLastChangeUser;
        private GridControl grcAttributes;
        private GridView grvAttributes;
        private GridColumn colAttribute;
        private GridColumn colValue;
        private PictureEdit picAdditionalImage1;
        private SimpleButton btnAddAdditionalImage;
        private LabelControl lblSummaryTitle;
        private LabelControl lblSummaryStatusTitle;
        private LabelControl lblSummaryStatus;
        private LabelControl lblSummaryStockTitle;
        private LabelControl lblSummaryStock;
        private LabelControl lblSummaryStockUnit;
        private LabelControl lblSummaryCostTitle;
        private LabelControl lblSummaryCost;
        private LabelControl lblSummaryPriceTitle;
        private LabelControl lblSummaryPrice;
        private LabelControl lblSummaryMarginTitle;
        private LabelControl lblSummaryMargin;
        private LabelControl lblSummarySapTitle;
        private LabelControl lblSummarySap;
        private LabelControl lblSummarySyncTitle;
        private LabelControl lblSummarySync;
        private LabelControl lblSummarySapDbTitle;
        private LabelControl lblSummarySapDb;
        private MemoEdit memQuickNotes;
        private LabelControl lblFooterRecord;
        private SimpleButton btnFirst;
        private SimpleButton btnPrevious;
        private SimpleButton btnNext;
        private SimpleButton btnLast;
        private LabelControl lblFooterMode;
        private LabelControl lblFooterCreated;
        private LabelControl lblFooterModified;
        private LabelControl lblFooterDatabase;
        private SimpleButton btnSave;
        private SimpleButton btnCancel;
        private GroupControl grpInventoryParams;
        private GroupControl grpStockControl;
        private GroupControl grpStockByWarehouse;
        private LabelControl lblDefaultWarehouse;
        private LabelControl lblValuationMethod;
        private LabelControl lblInventoryAccount;
        private LabelControl lblCostAccount;
        private LabelControl lblAdditionalCostHandling;
        private LabelControl lblManageBatch;
        private LabelControl lblManageSerial;
        private LabelControl lblManageBinLocation;
        private SearchLookUpEdit sleDefaultWarehouse;
        private GridView grvDefaultWarehouseLookup;
        private LookUpEdit lueValuationMethod;
        private SearchLookUpEdit sleInventoryAccount;
        private GridView grvInventoryAccountLookup;
        private SearchLookUpEdit sleCostAccount;
        private GridView grvCostAccountLookup;
        private LookUpEdit lueAdditionalCostHandling;
        private CheckEdit chkBatch;
        private CheckEdit chkSerial;
        private CheckEdit chkBinLocation;
        private LabelControl lblMinStock;
        private LabelControl lblMaxStock;
        private LabelControl lblReorderPoint;
        private LabelControl lblCurrentStock;
        private LabelControl lblCommitted;
        private LabelControl lblOrdered;
        private LabelControl lblAvailable;
        private LabelControl lblMinStockUnit;
        private LabelControl lblMaxStockUnit;
        private LabelControl lblReorderPointUnit;
        private LabelControl lblCurrentStockUnit;
        private LabelControl lblCommittedUnit;
        private LabelControl lblOrderedUnit;
        private LabelControl lblAvailableUnit;
        private SpinEdit sedMinStock;
        private SpinEdit sedMaxStock;
        private SpinEdit sedReorderPoint;
        private CalcEdit cleCurrentStock;
        private CalcEdit cleCommitted;
        private CalcEdit cleOrdered;
        private CalcEdit cleAvailable;
        private SimpleButton btnRefreshStock;
        private SimpleButton btnExportStock;
        private ButtonEdit btnSearchStock;
        private GridControl grcStock;
        private GridView grvStock;
        private GridColumn colWarehouseCode;
        private GridColumn colWarehouseName;
        private GridColumn colStock;
        private GridColumn colCommitted;
        private GridColumn colOrdered;
        private GridColumn colAvailable;
        private GridColumn colBinLocation;
        private GroupControl grpPurchaseConfig;
        private GroupControl grpPreferredVendor;
        private GroupControl grpAlternativeVendors;
        private LabelControl lblPreferredVendor;
        private LabelControl lblVendorCode;
        private LabelControl lblPurchaseUnit;
        private LabelControl lblMinPurchaseQty;
        private LabelControl lblDeliveryDays;
        private LabelControl lblLastPurchasePrice;
        private LabelControl lblPurchaseCurrency;
        private LabelControl lblPurchaseTax;
        private LabelControl lblPurchaseAccount;
        private LabelControl lblVendorDiscount;
        private LabelControl lblRepositionDays;
        private LabelControl lblMinPurchaseQtyUnit;
        private LabelControl lblDeliveryDaysUnit;
        private LabelControl lblLastPurchaseCurrency;
        private LabelControl lblRepositionDaysUnit;
        private SearchLookUpEdit slePreferredVendor;
        private GridView grvPreferredVendorLookup;
        private TextEdit txtVendorCode;
        private LookUpEdit luePurchaseUnit;
        private SpinEdit sedMinPurchaseQty;
        private SpinEdit sedDeliveryDays;
        private CalcEdit cleLastPurchasePrice;
        private LookUpEdit luePurchaseCurrency;
        private LookUpEdit luePurchaseTax;
        private SearchLookUpEdit slePurchaseAccount;
        private GridView grvPurchaseAccountLookup;
        private SpinEdit sedVendorDiscount;
        private SpinEdit sedRepositionDays;
        private LabelControl lblPreferredVendorTitle;
        private LabelControl lblVendorNitTitle;
        private LabelControl lblVendorNit;
        private LabelControl lblVendorContactTitle;
        private LabelControl lblVendorContact;
        private LabelControl lblVendorPhoneTitle;
        private LabelControl lblVendorPhone;
        private LabelControl lblVendorEmailTitle;
        private LabelControl lblVendorEmail;
        private LabelControl lblVendorAddressTitle;
        private LabelControl lblVendorAddress;
        private LabelControl lblVendorCityTitle;
        private LabelControl lblVendorCity;
        private SimpleButton btnOpenVendorFile;
        private SimpleButton btnAddVendor;
        private SimpleButton btnEditVendor;
        private SimpleButton btnDeleteVendor;
        private SimpleButton btnMoveVendorUp;
        private SimpleButton btnMoveVendorDown;
        private SimpleButton btnRefreshVendorPrices;
        private GridControl grcVendors;
        private GridView grvVendors;
        private GridColumn colVendorPriority;
        private GridColumn colVendorName;
        private GridColumn colVendorCode;
        private GridColumn colVendorPrice;
        private GridColumn colVendorCurrency;
        private GridColumn colVendorDeliveryDays;
        private GridColumn colVendorActive;
        private GroupControl grpSalesConfig;
        private GroupControl grpSalesMargins;
        private GroupControl grpPriceLists;
        private LabelControl lblSalesUnit;
        private LabelControl lblSalesTax;
        private LabelControl lblSalesAccount;
        private LabelControl lblSalesDefaultPriceList;
        private LabelControl lblSalesCurrency;
        private LabelControl lblSalesNotes;
        private LookUpEdit lueSalesUnit;
        private LookUpEdit lueSalesTax;
        private SearchLookUpEdit sleSalesAccount;
        private GridView grvSalesAccountLookup;
        private LookUpEdit lueSalesDefaultPriceList;
        private LookUpEdit lueSalesCurrency;
        private MemoEdit memSalesNotes;
        private LabelControl lblBasePrice;
        private LabelControl lblSalesMaxDiscount;
        private LabelControl lblMinMargin;
        private LabelControl lblCurrentMargin;
        private CalcEdit cleBasePrice;
        private SpinEdit sedSalesMaxDiscount;
        private SpinEdit sedMinMargin;
        private CalcEdit cleCurrentMargin;
        private CheckEdit chkValidatePriceBelowCost;
        private CheckEdit chkRequireDiscountAuthorization;
        private SimpleButton btnAddPriceList;
        private SimpleButton btnEditPriceList;
        private SimpleButton btnDeletePriceList;
        private SimpleButton btnRefreshPrices;
        private ButtonEdit btnSearchPriceList;
        private GridControl grcPrices;
        private GridView grvPrices;
        private GridColumn colPriceListName;
        private GridColumn colPrice;
        private GridColumn colPriceCurrency;
        private GridColumn colPriceMargin;
        private GridColumn colPriceStartDate;
        private GridColumn colPriceEndDate;
        private GridColumn colPriceActive;
        private GroupControl grpCostIndicators;
        private GroupControl grpProfitability;
        private GroupControl grpCostDates;
        private GroupControl grpCostHistory;
        private LabelControl lblAverageCost;
        private LabelControl lblLastCost;
        private LabelControl lblStandardCost;
        private LabelControl lblReplacementCost;
        private LabelControl lblCostCurrency;
        private CalcEdit cleAverageCost;
        private CalcEdit cleLastCost;
        private CalcEdit cleStandardCost;
        private CalcEdit cleReplacementCost;
        private LookUpEdit lueCostCurrency;
        private LabelControl lblProfitBasePrice;
        private LabelControl lblEstimatedMargin;
        private LabelControl lblEstimatedUtility;
        private LabelControl lblMarkup;
        private LabelControl lblProfitability;
        private CalcEdit cleProfitBasePrice;
        private CalcEdit cleEstimatedMargin;
        private CalcEdit cleEstimatedUtility;
        private CalcEdit cleMarkup;
        private CalcEdit cleProfitability;
        private LabelControl lblLastPurchaseDate;
        private LabelControl lblLastSaleDate;
        private LabelControl lblDaysFromLastPurchase;
        private LabelControl lblDaysFromLastSale;
        private LabelControl lblRotation30;
        private LabelControl lblRotation90;
        private DateEdit dtpLastPurchase;
        private DateEdit dtpLastSale;
        private SpinEdit sedDaysFromLastPurchase;
        private SpinEdit sedDaysFromLastSale;
        private CalcEdit cleRotation30;
        private CalcEdit cleRotation90;
        private GridControl grcCosts;
        private GridView grvCosts;
        private GridColumn colCostDate;
        private GridColumn colCostDocument;
        private GridColumn colCostVendor;
        private GridColumn colCostQuantity;
        private GridColumn colCostPrevious;
        private GridColumn colCostNew;
        private GridColumn colCostCurrency;
        private GridColumn colCostUser;
        private SimpleButton btnRefreshCosts;
        private SimpleButton btnExportCosts;
        private GroupControl grpSapIntegration;
        private GroupControl grpSapActions;
        private GroupControl grpSapUdf;
        private LabelControl lblSapCode;
        private LabelControl lblSapStatus;
        private LabelControl lblLastSapSync;
        private LabelControl lblSapDatabase;
        private LabelControl lblSapGroup;
        private LabelControl lblSapUom;
        private LabelControl lblSapMessage;
        private TextEdit txtSapCode;
        private LookUpEdit lueSapStatus;
        private DateEdit dtpLastSapSync;
        private TextEdit txtSapDatabase;
        private TextEdit txtSapGroup;
        private TextEdit txtSapUom;
        private MemoEdit memSapMessage;
        private SimpleButton btnSyncNow;
        private LabelControl lblSyncNowTitle;
        private LabelControl lblSyncNowDescription;
        private SimpleButton btnOpenSap;
        private LabelControl lblOpenSapTitle;
        private LabelControl lblOpenSapDescription;
        private SimpleButton btnViewIntegrationLog;
        private LabelControl lblViewLogTitle;
        private LabelControl lblViewLogDescription;
        private GridControl grcSapUdf;
        private GridView grvSapUdf;
        private GridColumn colSapUdfField;
        private GridColumn colSapUdfDescription;
        private GridColumn colSapUdfLocalValue;
        private GridColumn colSapUdfSapValue;
        private GridColumn colSapUdfStatus;
        private LabelControl lblSapUdfTotalRecords;
        private SimpleButton btnSapUdfFirst;
        private SimpleButton btnSapUdfPrevious;
        private SimpleButton btnSapUdfNext;
        private SimpleButton btnSapUdfLast;
        private LookUpEdit lueSapUdfPageSize;
        private LabelControl lblSapUdfPageInfo;

        private void InitializeComponent()
        {
            pnlMain = new PanelControl();
            splContent = new SplitContainerControl();
            xtcMain = new XtraTabControl();
            xtpGeneral = new XtraTabPage();
            grpGeneralData = new GroupControl();
            lblLongDescription = new LabelControl();
            memLongDescription = new MemoEdit();
            lblCategory = new LabelControl();
            sleCategory = new SearchLookUpEdit();
            grvCategoryLookup = new GridView();
            lblSubCategory = new LabelControl();
            sleSubCategory = new SearchLookUpEdit();
            grvSubCategoryLookup = new GridView();
            lblManufacturer = new LabelControl();
            sleManufacturer = new SearchLookUpEdit();
            grvManufacturerLookup = new GridView();
            lblModel = new LabelControl();
            txtModel = new TextEdit();
            lblCountry = new LabelControl();
            lueCountry = new LookUpEdit();
            lblAlternateCode = new LabelControl();
            txtAlternateCode = new TextEdit();
            lblWeight = new LabelControl();
            sedWeight = new SpinEdit();
            lblVolume = new LabelControl();
            sedVolume = new SpinEdit();
            lblPurchaseUom = new LabelControl();
            slePurchaseUom = new SearchLookUpEdit();
            grvPurchaseUomLookup = new GridView();
            lblSalesUom = new LabelControl();
            sleSalesUom = new SearchLookUpEdit();
            grvSalesUomLookup = new GridView();
            lblInventoryUom = new LabelControl();
            sleInventoryUom = new SearchLookUpEdit();
            grvInventoryUomLookup = new GridView();
            lblGeneralNotes = new LabelControl();
            memGeneralNotes = new MemoEdit();
            grpAdditionalInfo = new GroupControl();
            lblSatCode = new LabelControl();
            txtSatCode = new TextEdit();
            lblUnspscCode = new LabelControl();
            txtUnspscCode = new TextEdit();
            lblTaxCode = new LabelControl();
            txtTaxCode = new TextEdit();
            lblTaxType = new LabelControl();
            lueTaxType = new LookUpEdit();
            lblBaseCurrency = new LabelControl();
            lueBaseCurrency = new LookUpEdit();
            lblDefaultPriceList = new LabelControl();
            lueDefaultPriceList = new LookUpEdit();
            lblMaxDiscount = new LabelControl();
            sedMaxDiscount = new SpinEdit();
            lblMaxDiscountSymbol = new LabelControl();
            lblLastChange = new LabelControl();
            dtpLastChange = new DateEdit();
            lblLastChangeUser = new LabelControl();
            txtLastChangeUser = new TextEdit();
            grpAttributes = new GroupControl();
            grcAttributes = new GridControl();
            grvAttributes = new GridView();
            colAttribute = new GridColumn();
            colValue = new GridColumn();
            grpAdditionalImages = new GroupControl();
            picAdditionalImage1 = new PictureEdit();
            btnAddAdditionalImage = new SimpleButton();
            xtpInventory = new XtraTabPage();
            grpInventoryParams = new GroupControl();
            lblDefaultWarehouse = new LabelControl();
            sleDefaultWarehouse = new SearchLookUpEdit();
            grvDefaultWarehouseLookup = new GridView();
            lblValuationMethod = new LabelControl();
            lueValuationMethod = new LookUpEdit();
            lblInventoryAccount = new LabelControl();
            sleInventoryAccount = new SearchLookUpEdit();
            grvInventoryAccountLookup = new GridView();
            lblCostAccount = new LabelControl();
            sleCostAccount = new SearchLookUpEdit();
            grvCostAccountLookup = new GridView();
            lblAdditionalCostHandling = new LabelControl();
            lueAdditionalCostHandling = new LookUpEdit();
            lblManageBatch = new LabelControl();
            chkBatch = new CheckEdit();
            lblManageSerial = new LabelControl();
            chkSerial = new CheckEdit();
            lblManageBinLocation = new LabelControl();
            chkBinLocation = new CheckEdit();
            grpStockControl = new GroupControl();
            lblMinStock = new LabelControl();
            sedMinStock = new SpinEdit();
            lblMinStockUnit = new LabelControl();
            lblMaxStock = new LabelControl();
            sedMaxStock = new SpinEdit();
            lblMaxStockUnit = new LabelControl();
            lblReorderPoint = new LabelControl();
            sedReorderPoint = new SpinEdit();
            lblReorderPointUnit = new LabelControl();
            lblCurrentStock = new LabelControl();
            cleCurrentStock = new CalcEdit();
            lblCurrentStockUnit = new LabelControl();
            lblCommitted = new LabelControl();
            cleCommitted = new CalcEdit();
            lblCommittedUnit = new LabelControl();
            lblOrdered = new LabelControl();
            cleOrdered = new CalcEdit();
            lblOrderedUnit = new LabelControl();
            lblAvailable = new LabelControl();
            cleAvailable = new CalcEdit();
            lblAvailableUnit = new LabelControl();
            grpStockByWarehouse = new GroupControl();
            btnRefreshStock = new SimpleButton();
            btnExportStock = new SimpleButton();
            btnSearchStock = new ButtonEdit();
            grcStock = new GridControl();
            grvStock = new GridView();
            colWarehouseCode = new GridColumn();
            colWarehouseName = new GridColumn();
            colStock = new GridColumn();
            colCommitted = new GridColumn();
            colOrdered = new GridColumn();
            colAvailable = new GridColumn();
            colBinLocation = new GridColumn();
            xtpPurchases = new XtraTabPage();
            grpPurchaseConfig = new GroupControl();
            lblPreferredVendor = new LabelControl();
            slePreferredVendor = new SearchLookUpEdit();
            grvPreferredVendorLookup = new GridView();
            lblVendorCode = new LabelControl();
            txtVendorCode = new TextEdit();
            lblPurchaseUnit = new LabelControl();
            luePurchaseUnit = new LookUpEdit();
            lblMinPurchaseQty = new LabelControl();
            sedMinPurchaseQty = new SpinEdit();
            lblMinPurchaseQtyUnit = new LabelControl();
            lblDeliveryDays = new LabelControl();
            sedDeliveryDays = new SpinEdit();
            lblDeliveryDaysUnit = new LabelControl();
            lblLastPurchasePrice = new LabelControl();
            cleLastPurchasePrice = new CalcEdit();
            lblLastPurchaseCurrency = new LabelControl();
            lblPurchaseCurrency = new LabelControl();
            luePurchaseCurrency = new LookUpEdit();
            lblPurchaseTax = new LabelControl();
            luePurchaseTax = new LookUpEdit();
            lblPurchaseAccount = new LabelControl();
            slePurchaseAccount = new SearchLookUpEdit();
            grvPurchaseAccountLookup = new GridView();
            lblVendorDiscount = new LabelControl();
            sedVendorDiscount = new SpinEdit();
            lblRepositionDays = new LabelControl();
            sedRepositionDays = new SpinEdit();
            lblRepositionDaysUnit = new LabelControl();
            grpPreferredVendor = new GroupControl();
            lblPreferredVendorTitle = new LabelControl();
            lblVendorNitTitle = new LabelControl();
            lblVendorNit = new LabelControl();
            lblVendorContactTitle = new LabelControl();
            lblVendorContact = new LabelControl();
            lblVendorPhoneTitle = new LabelControl();
            lblVendorPhone = new LabelControl();
            lblVendorEmailTitle = new LabelControl();
            lblVendorEmail = new LabelControl();
            lblVendorAddressTitle = new LabelControl();
            lblVendorAddress = new LabelControl();
            lblVendorCityTitle = new LabelControl();
            lblVendorCity = new LabelControl();
            btnOpenVendorFile = new SimpleButton();
            grpAlternativeVendors = new GroupControl();
            btnAddVendor = new SimpleButton();
            btnEditVendor = new SimpleButton();
            btnDeleteVendor = new SimpleButton();
            btnMoveVendorUp = new SimpleButton();
            btnMoveVendorDown = new SimpleButton();
            btnRefreshVendorPrices = new SimpleButton();
            grcVendors = new GridControl();
            grvVendors = new GridView();
            colVendorPriority = new GridColumn();
            colVendorName = new GridColumn();
            colVendorCode = new GridColumn();
            colVendorPrice = new GridColumn();
            colVendorCurrency = new GridColumn();
            colVendorDeliveryDays = new GridColumn();
            colVendorActive = new GridColumn();
            xtpSales = new XtraTabPage();
            grpSalesConfig = new GroupControl();
            lblSalesUnit = new LabelControl();
            lueSalesUnit = new LookUpEdit();
            lblSalesTax = new LabelControl();
            lueSalesTax = new LookUpEdit();
            lblSalesAccount = new LabelControl();
            sleSalesAccount = new SearchLookUpEdit();
            grvSalesAccountLookup = new GridView();
            lblSalesDefaultPriceList = new LabelControl();
            lueSalesDefaultPriceList = new LookUpEdit();
            lblSalesCurrency = new LabelControl();
            lueSalesCurrency = new LookUpEdit();
            lblSalesNotes = new LabelControl();
            memSalesNotes = new MemoEdit();
            grpSalesMargins = new GroupControl();
            lblBasePrice = new LabelControl();
            cleBasePrice = new CalcEdit();
            lblSalesMaxDiscount = new LabelControl();
            sedSalesMaxDiscount = new SpinEdit();
            lblMinMargin = new LabelControl();
            sedMinMargin = new SpinEdit();
            lblCurrentMargin = new LabelControl();
            cleCurrentMargin = new CalcEdit();
            chkValidatePriceBelowCost = new CheckEdit();
            chkRequireDiscountAuthorization = new CheckEdit();
            grpPriceLists = new GroupControl();
            btnAddPriceList = new SimpleButton();
            btnEditPriceList = new SimpleButton();
            btnDeletePriceList = new SimpleButton();
            btnRefreshPrices = new SimpleButton();
            btnSearchPriceList = new ButtonEdit();
            grcPrices = new GridControl();
            grvPrices = new GridView();
            colPriceListName = new GridColumn();
            colPrice = new GridColumn();
            colPriceCurrency = new GridColumn();
            colPriceMargin = new GridColumn();
            colPriceStartDate = new GridColumn();
            colPriceEndDate = new GridColumn();
            colPriceActive = new GridColumn();
            xtpCosts = new XtraTabPage();
            grpCostIndicators = new GroupControl();
            lblAverageCost = new LabelControl();
            cleAverageCost = new CalcEdit();
            lblLastCost = new LabelControl();
            cleLastCost = new CalcEdit();
            lblStandardCost = new LabelControl();
            cleStandardCost = new CalcEdit();
            lblReplacementCost = new LabelControl();
            cleReplacementCost = new CalcEdit();
            lblCostCurrency = new LabelControl();
            lueCostCurrency = new LookUpEdit();
            grpProfitability = new GroupControl();
            lblProfitBasePrice = new LabelControl();
            cleProfitBasePrice = new CalcEdit();
            lblEstimatedMargin = new LabelControl();
            cleEstimatedMargin = new CalcEdit();
            lblEstimatedUtility = new LabelControl();
            cleEstimatedUtility = new CalcEdit();
            lblMarkup = new LabelControl();
            cleMarkup = new CalcEdit();
            lblProfitability = new LabelControl();
            cleProfitability = new CalcEdit();
            grpCostDates = new GroupControl();
            lblLastPurchaseDate = new LabelControl();
            dtpLastPurchase = new DateEdit();
            lblLastSaleDate = new LabelControl();
            dtpLastSale = new DateEdit();
            lblDaysFromLastPurchase = new LabelControl();
            sedDaysFromLastPurchase = new SpinEdit();
            lblDaysFromLastSale = new LabelControl();
            sedDaysFromLastSale = new SpinEdit();
            lblRotation30 = new LabelControl();
            cleRotation30 = new CalcEdit();
            lblRotation90 = new LabelControl();
            cleRotation90 = new CalcEdit();
            grpCostHistory = new GroupControl();
            btnRefreshCosts = new SimpleButton();
            btnExportCosts = new SimpleButton();
            grcCosts = new GridControl();
            grvCosts = new GridView();
            colCostDate = new GridColumn();
            colCostDocument = new GridColumn();
            colCostVendor = new GridColumn();
            colCostQuantity = new GridColumn();
            colCostPrevious = new GridColumn();
            colCostNew = new GridColumn();
            colCostCurrency = new GridColumn();
            colCostUser = new GridColumn();
            xtpSap = new XtraTabPage();
            grpSapIntegration = new GroupControl();
            lblSapCode = new LabelControl();
            txtSapCode = new TextEdit();
            lblSapStatus = new LabelControl();
            lueSapStatus = new LookUpEdit();
            lblLastSapSync = new LabelControl();
            dtpLastSapSync = new DateEdit();
            lblSapDatabase = new LabelControl();
            txtSapDatabase = new TextEdit();
            lblSapGroup = new LabelControl();
            txtSapGroup = new TextEdit();
            lblSapUom = new LabelControl();
            txtSapUom = new TextEdit();
            lblSapMessage = new LabelControl();
            memSapMessage = new MemoEdit();
            grpSapActions = new GroupControl();
            btnSyncNow = new SimpleButton();
            lblSyncNowTitle = new LabelControl();
            lblSyncNowDescription = new LabelControl();
            btnOpenSap = new SimpleButton();
            lblOpenSapTitle = new LabelControl();
            lblOpenSapDescription = new LabelControl();
            btnViewIntegrationLog = new SimpleButton();
            lblViewLogTitle = new LabelControl();
            lblViewLogDescription = new LabelControl();
            grpSapUdf = new GroupControl();
            grcSapUdf = new GridControl();
            grvSapUdf = new GridView();
            colSapUdfField = new GridColumn();
            colSapUdfDescription = new GridColumn();
            colSapUdfLocalValue = new GridColumn();
            colSapUdfSapValue = new GridColumn();
            colSapUdfStatus = new GridColumn();
            lblSapUdfTotalRecords = new LabelControl();
            btnSapUdfFirst = new SimpleButton();
            btnSapUdfPrevious = new SimpleButton();
            lueSapUdfPageSize = new LookUpEdit();
            lblSapUdfPageInfo = new LabelControl();
            btnSapUdfNext = new SimpleButton();
            btnSapUdfLast = new SimpleButton();
            grpHeader = new GroupControl();
            lblItemCode = new LabelControl();
            txtItemCode = new TextEdit();
            lblBarCode = new LabelControl();
            txtBarCode = new TextEdit();
            lblDescription = new LabelControl();
            txtDescription = new TextEdit();
            lblCommercialName = new LabelControl();
            txtCommercialName = new TextEdit();
            lblItemGroup = new LabelControl();
            sleItemGroup = new SearchLookUpEdit();
            grvItemGroupLookup = new GridView();
            lblBrand = new LabelControl();
            sleBrand = new SearchLookUpEdit();
            grvBrandLookup = new GridView();
            lblLine = new LabelControl();
            sleLine = new SearchLookUpEdit();
            grvLineLookup = new GridView();
            lblUom = new LabelControl();
            sleHeaderUom = new SearchLookUpEdit();
            grvHeaderUomLookup = new GridView();
            lblItemType = new LabelControl();
            lueItemType = new LookUpEdit();
            lblStatus = new LabelControl();
            lueStatus = new LookUpEdit();
            chkInventoryItem = new CheckEdit();
            chkPurchaseItem = new CheckEdit();
            chkSalesItem = new CheckEdit();
            picItem = new PictureEdit();
            btnChangeImage = new SimpleButton();
            btnRemoveImage = new SimpleButton();
            pnlSummary = new PanelControl();
            lblSummaryTitle = new LabelControl();
            lblSummaryStatusTitle = new LabelControl();
            lblSummaryStatus = new LabelControl();
            lblSummaryStockTitle = new LabelControl();
            lblSummaryStock = new LabelControl();
            lblSummaryStockUnit = new LabelControl();
            lblSummaryCostTitle = new LabelControl();
            lblSummaryCost = new LabelControl();
            lblSummaryPriceTitle = new LabelControl();
            lblSummaryPrice = new LabelControl();
            lblSummaryMarginTitle = new LabelControl();
            lblSummaryMargin = new LabelControl();
            lblSummarySapTitle = new LabelControl();
            lblSummarySap = new LabelControl();
            lblSummarySyncTitle = new LabelControl();
            lblSummarySync = new LabelControl();
            lblSummarySapDbTitle = new LabelControl();
            lblSummarySapDb = new LabelControl();
            memQuickNotes = new MemoEdit();
            pnlFooter = new PanelControl();
            lblFooterRecord = new LabelControl();
            btnFirst = new SimpleButton();
            btnPrevious = new SimpleButton();
            btnNext = new SimpleButton();
            btnLast = new SimpleButton();
            lblFooterMode = new LabelControl();
            lblFooterCreated = new LabelControl();
            lblFooterModified = new LabelControl();
            lblFooterDatabase = new LabelControl();
            btnSave = new SimpleButton();
            btnCancel = new SimpleButton();
            ((ISupportInitialize)pnlMain).BeginInit();
            pnlMain.SuspendLayout();
            ((ISupportInitialize)splContent).BeginInit();
            ((ISupportInitialize)splContent.Panel1).BeginInit();
            splContent.Panel1.SuspendLayout();
            ((ISupportInitialize)splContent.Panel2).BeginInit();
            splContent.Panel2.SuspendLayout();
            splContent.SuspendLayout();
            ((ISupportInitialize)xtcMain).BeginInit();
            xtcMain.SuspendLayout();
            xtpGeneral.SuspendLayout();
            ((ISupportInitialize)grpGeneralData).BeginInit();
            grpGeneralData.SuspendLayout();
            ((ISupportInitialize)memLongDescription.Properties).BeginInit();
            ((ISupportInitialize)sleCategory.Properties).BeginInit();
            ((ISupportInitialize)grvCategoryLookup).BeginInit();
            ((ISupportInitialize)sleSubCategory.Properties).BeginInit();
            ((ISupportInitialize)grvSubCategoryLookup).BeginInit();
            ((ISupportInitialize)sleManufacturer.Properties).BeginInit();
            ((ISupportInitialize)grvManufacturerLookup).BeginInit();
            ((ISupportInitialize)txtModel.Properties).BeginInit();
            ((ISupportInitialize)lueCountry.Properties).BeginInit();
            ((ISupportInitialize)txtAlternateCode.Properties).BeginInit();
            ((ISupportInitialize)sedWeight.Properties).BeginInit();
            ((ISupportInitialize)sedVolume.Properties).BeginInit();
            ((ISupportInitialize)slePurchaseUom.Properties).BeginInit();
            ((ISupportInitialize)grvPurchaseUomLookup).BeginInit();
            ((ISupportInitialize)sleSalesUom.Properties).BeginInit();
            ((ISupportInitialize)grvSalesUomLookup).BeginInit();
            ((ISupportInitialize)sleInventoryUom.Properties).BeginInit();
            ((ISupportInitialize)grvInventoryUomLookup).BeginInit();
            ((ISupportInitialize)memGeneralNotes.Properties).BeginInit();
            ((ISupportInitialize)grpAdditionalInfo).BeginInit();
            grpAdditionalInfo.SuspendLayout();
            ((ISupportInitialize)txtSatCode.Properties).BeginInit();
            ((ISupportInitialize)txtUnspscCode.Properties).BeginInit();
            ((ISupportInitialize)txtTaxCode.Properties).BeginInit();
            ((ISupportInitialize)lueTaxType.Properties).BeginInit();
            ((ISupportInitialize)lueBaseCurrency.Properties).BeginInit();
            ((ISupportInitialize)lueDefaultPriceList.Properties).BeginInit();
            ((ISupportInitialize)sedMaxDiscount.Properties).BeginInit();
            ((ISupportInitialize)dtpLastChange.Properties).BeginInit();
            ((ISupportInitialize)dtpLastChange.Properties.CalendarTimeProperties).BeginInit();
            ((ISupportInitialize)txtLastChangeUser.Properties).BeginInit();
            ((ISupportInitialize)grpAttributes).BeginInit();
            grpAttributes.SuspendLayout();
            ((ISupportInitialize)grcAttributes).BeginInit();
            ((ISupportInitialize)grvAttributes).BeginInit();
            ((ISupportInitialize)grpAdditionalImages).BeginInit();
            grpAdditionalImages.SuspendLayout();
            ((ISupportInitialize)picAdditionalImage1.Properties).BeginInit();
            xtpInventory.SuspendLayout();
            ((ISupportInitialize)grpInventoryParams).BeginInit();
            grpInventoryParams.SuspendLayout();
            ((ISupportInitialize)sleDefaultWarehouse.Properties).BeginInit();
            ((ISupportInitialize)grvDefaultWarehouseLookup).BeginInit();
            ((ISupportInitialize)lueValuationMethod.Properties).BeginInit();
            ((ISupportInitialize)sleInventoryAccount.Properties).BeginInit();
            ((ISupportInitialize)grvInventoryAccountLookup).BeginInit();
            ((ISupportInitialize)sleCostAccount.Properties).BeginInit();
            ((ISupportInitialize)grvCostAccountLookup).BeginInit();
            ((ISupportInitialize)lueAdditionalCostHandling.Properties).BeginInit();
            ((ISupportInitialize)chkBatch.Properties).BeginInit();
            ((ISupportInitialize)chkSerial.Properties).BeginInit();
            ((ISupportInitialize)chkBinLocation.Properties).BeginInit();
            ((ISupportInitialize)grpStockControl).BeginInit();
            grpStockControl.SuspendLayout();
            ((ISupportInitialize)sedMinStock.Properties).BeginInit();
            ((ISupportInitialize)sedMaxStock.Properties).BeginInit();
            ((ISupportInitialize)sedReorderPoint.Properties).BeginInit();
            ((ISupportInitialize)cleCurrentStock.Properties).BeginInit();
            ((ISupportInitialize)cleCommitted.Properties).BeginInit();
            ((ISupportInitialize)cleOrdered.Properties).BeginInit();
            ((ISupportInitialize)cleAvailable.Properties).BeginInit();
            ((ISupportInitialize)grpStockByWarehouse).BeginInit();
            grpStockByWarehouse.SuspendLayout();
            ((ISupportInitialize)btnSearchStock.Properties).BeginInit();
            ((ISupportInitialize)grcStock).BeginInit();
            ((ISupportInitialize)grvStock).BeginInit();
            xtpPurchases.SuspendLayout();
            ((ISupportInitialize)grpPurchaseConfig).BeginInit();
            grpPurchaseConfig.SuspendLayout();
            ((ISupportInitialize)slePreferredVendor.Properties).BeginInit();
            ((ISupportInitialize)grvPreferredVendorLookup).BeginInit();
            ((ISupportInitialize)txtVendorCode.Properties).BeginInit();
            ((ISupportInitialize)luePurchaseUnit.Properties).BeginInit();
            ((ISupportInitialize)sedMinPurchaseQty.Properties).BeginInit();
            ((ISupportInitialize)sedDeliveryDays.Properties).BeginInit();
            ((ISupportInitialize)cleLastPurchasePrice.Properties).BeginInit();
            ((ISupportInitialize)luePurchaseCurrency.Properties).BeginInit();
            ((ISupportInitialize)luePurchaseTax.Properties).BeginInit();
            ((ISupportInitialize)slePurchaseAccount.Properties).BeginInit();
            ((ISupportInitialize)grvPurchaseAccountLookup).BeginInit();
            ((ISupportInitialize)sedVendorDiscount.Properties).BeginInit();
            ((ISupportInitialize)sedRepositionDays.Properties).BeginInit();
            ((ISupportInitialize)grpPreferredVendor).BeginInit();
            grpPreferredVendor.SuspendLayout();
            ((ISupportInitialize)grpAlternativeVendors).BeginInit();
            grpAlternativeVendors.SuspendLayout();
            ((ISupportInitialize)grcVendors).BeginInit();
            ((ISupportInitialize)grvVendors).BeginInit();
            xtpSales.SuspendLayout();
            ((ISupportInitialize)grpSalesConfig).BeginInit();
            grpSalesConfig.SuspendLayout();
            ((ISupportInitialize)lueSalesUnit.Properties).BeginInit();
            ((ISupportInitialize)lueSalesTax.Properties).BeginInit();
            ((ISupportInitialize)sleSalesAccount.Properties).BeginInit();
            ((ISupportInitialize)grvSalesAccountLookup).BeginInit();
            ((ISupportInitialize)lueSalesDefaultPriceList.Properties).BeginInit();
            ((ISupportInitialize)lueSalesCurrency.Properties).BeginInit();
            ((ISupportInitialize)memSalesNotes.Properties).BeginInit();
            ((ISupportInitialize)grpSalesMargins).BeginInit();
            grpSalesMargins.SuspendLayout();
            ((ISupportInitialize)cleBasePrice.Properties).BeginInit();
            ((ISupportInitialize)sedSalesMaxDiscount.Properties).BeginInit();
            ((ISupportInitialize)sedMinMargin.Properties).BeginInit();
            ((ISupportInitialize)cleCurrentMargin.Properties).BeginInit();
            ((ISupportInitialize)chkValidatePriceBelowCost.Properties).BeginInit();
            ((ISupportInitialize)chkRequireDiscountAuthorization.Properties).BeginInit();
            ((ISupportInitialize)grpPriceLists).BeginInit();
            grpPriceLists.SuspendLayout();
            ((ISupportInitialize)btnSearchPriceList.Properties).BeginInit();
            ((ISupportInitialize)grcPrices).BeginInit();
            ((ISupportInitialize)grvPrices).BeginInit();
            xtpCosts.SuspendLayout();
            ((ISupportInitialize)grpCostIndicators).BeginInit();
            grpCostIndicators.SuspendLayout();
            ((ISupportInitialize)cleAverageCost.Properties).BeginInit();
            ((ISupportInitialize)cleLastCost.Properties).BeginInit();
            ((ISupportInitialize)cleStandardCost.Properties).BeginInit();
            ((ISupportInitialize)cleReplacementCost.Properties).BeginInit();
            ((ISupportInitialize)lueCostCurrency.Properties).BeginInit();
            ((ISupportInitialize)grpProfitability).BeginInit();
            grpProfitability.SuspendLayout();
            ((ISupportInitialize)cleProfitBasePrice.Properties).BeginInit();
            ((ISupportInitialize)cleEstimatedMargin.Properties).BeginInit();
            ((ISupportInitialize)cleEstimatedUtility.Properties).BeginInit();
            ((ISupportInitialize)cleMarkup.Properties).BeginInit();
            ((ISupportInitialize)cleProfitability.Properties).BeginInit();
            ((ISupportInitialize)grpCostDates).BeginInit();
            grpCostDates.SuspendLayout();
            ((ISupportInitialize)dtpLastPurchase.Properties).BeginInit();
            ((ISupportInitialize)dtpLastPurchase.Properties.CalendarTimeProperties).BeginInit();
            ((ISupportInitialize)dtpLastSale.Properties).BeginInit();
            ((ISupportInitialize)dtpLastSale.Properties.CalendarTimeProperties).BeginInit();
            ((ISupportInitialize)sedDaysFromLastPurchase.Properties).BeginInit();
            ((ISupportInitialize)sedDaysFromLastSale.Properties).BeginInit();
            ((ISupportInitialize)cleRotation30.Properties).BeginInit();
            ((ISupportInitialize)cleRotation90.Properties).BeginInit();
            ((ISupportInitialize)grpCostHistory).BeginInit();
            grpCostHistory.SuspendLayout();
            ((ISupportInitialize)grcCosts).BeginInit();
            ((ISupportInitialize)grvCosts).BeginInit();
            xtpSap.SuspendLayout();
            ((ISupportInitialize)grpSapIntegration).BeginInit();
            grpSapIntegration.SuspendLayout();
            ((ISupportInitialize)txtSapCode.Properties).BeginInit();
            ((ISupportInitialize)lueSapStatus.Properties).BeginInit();
            ((ISupportInitialize)dtpLastSapSync.Properties).BeginInit();
            ((ISupportInitialize)dtpLastSapSync.Properties.CalendarTimeProperties).BeginInit();
            ((ISupportInitialize)txtSapDatabase.Properties).BeginInit();
            ((ISupportInitialize)txtSapGroup.Properties).BeginInit();
            ((ISupportInitialize)txtSapUom.Properties).BeginInit();
            ((ISupportInitialize)memSapMessage.Properties).BeginInit();
            ((ISupportInitialize)grpSapActions).BeginInit();
            grpSapActions.SuspendLayout();
            ((ISupportInitialize)grpSapUdf).BeginInit();
            grpSapUdf.SuspendLayout();
            ((ISupportInitialize)grcSapUdf).BeginInit();
            ((ISupportInitialize)grvSapUdf).BeginInit();
            ((ISupportInitialize)lueSapUdfPageSize.Properties).BeginInit();
            ((ISupportInitialize)grpHeader).BeginInit();
            grpHeader.SuspendLayout();
            ((ISupportInitialize)txtItemCode.Properties).BeginInit();
            ((ISupportInitialize)txtBarCode.Properties).BeginInit();
            ((ISupportInitialize)txtDescription.Properties).BeginInit();
            ((ISupportInitialize)txtCommercialName.Properties).BeginInit();
            ((ISupportInitialize)sleItemGroup.Properties).BeginInit();
            ((ISupportInitialize)grvItemGroupLookup).BeginInit();
            ((ISupportInitialize)sleBrand.Properties).BeginInit();
            ((ISupportInitialize)grvBrandLookup).BeginInit();
            ((ISupportInitialize)sleLine.Properties).BeginInit();
            ((ISupportInitialize)grvLineLookup).BeginInit();
            ((ISupportInitialize)sleHeaderUom.Properties).BeginInit();
            ((ISupportInitialize)grvHeaderUomLookup).BeginInit();
            ((ISupportInitialize)lueItemType.Properties).BeginInit();
            ((ISupportInitialize)lueStatus.Properties).BeginInit();
            ((ISupportInitialize)chkInventoryItem.Properties).BeginInit();
            ((ISupportInitialize)chkPurchaseItem.Properties).BeginInit();
            ((ISupportInitialize)chkSalesItem.Properties).BeginInit();
            ((ISupportInitialize)picItem.Properties).BeginInit();
            ((ISupportInitialize)pnlSummary).BeginInit();
            pnlSummary.SuspendLayout();
            ((ISupportInitialize)memQuickNotes.Properties).BeginInit();
            ((ISupportInitialize)pnlFooter).BeginInit();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            // 
            // pnlMain
            // 
            pnlMain.Controls.Add(splContent);
            pnlMain.Controls.Add(pnlFooter);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(0, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(1286, 737);
            pnlMain.TabIndex = 0;
            // 
            // splContent
            // 
            splContent.Dock = DockStyle.Fill;
            splContent.FixedPanel = SplitFixedPanel.Panel2;
            splContent.Location = new Point(2, 2);
            splContent.Name = "splContent";
            // 
            // splContent.Panel1
            // 
            splContent.Panel1.Controls.Add(xtcMain);
            splContent.Panel1.Controls.Add(grpHeader);
            splContent.Panel1.Text = "Panel1";
            // 
            // splContent.Panel2
            // 
            splContent.Panel2.Controls.Add(pnlSummary);
            splContent.Panel2.Text = "Panel2";
            splContent.Size = new Size(1282, 697);
            splContent.SplitterPosition = 182;
            splContent.TabIndex = 1;
            // 
            // xtcMain
            // 
            xtcMain.Dock = DockStyle.Fill;
            xtcMain.Location = new Point(0, 217);
            xtcMain.Name = "xtcMain";
            xtcMain.SelectedTabPage = xtpGeneral;
            xtcMain.Size = new Size(1090, 480);
            xtcMain.TabIndex = 0;
            xtcMain.TabPages.AddRange(new XtraTabPage[] { xtpGeneral, xtpInventory, xtpPurchases, xtpSales, xtpCosts, xtpSap });
            // 
            // xtpGeneral
            // 
            xtpGeneral.Controls.Add(grpGeneralData);
            xtpGeneral.Controls.Add(grpAdditionalInfo);
            xtpGeneral.Controls.Add(grpAttributes);
            xtpGeneral.Controls.Add(grpAdditionalImages);
            xtpGeneral.Name = "xtpGeneral";
            xtpGeneral.Size = new Size(1088, 455);
            xtpGeneral.Text = "General";
            // 
            // grpGeneralData
            // 
            grpGeneralData.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpGeneralData.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpGeneralData.AppearanceCaption.Options.UseFont = true;
            grpGeneralData.AppearanceCaption.Options.UseForeColor = true;
            grpGeneralData.Controls.Add(lblLongDescription);
            grpGeneralData.Controls.Add(memLongDescription);
            grpGeneralData.Controls.Add(lblCategory);
            grpGeneralData.Controls.Add(sleCategory);
            grpGeneralData.Controls.Add(lblSubCategory);
            grpGeneralData.Controls.Add(sleSubCategory);
            grpGeneralData.Controls.Add(lblManufacturer);
            grpGeneralData.Controls.Add(sleManufacturer);
            grpGeneralData.Controls.Add(lblModel);
            grpGeneralData.Controls.Add(txtModel);
            grpGeneralData.Controls.Add(lblCountry);
            grpGeneralData.Controls.Add(lueCountry);
            grpGeneralData.Controls.Add(lblAlternateCode);
            grpGeneralData.Controls.Add(txtAlternateCode);
            grpGeneralData.Controls.Add(lblWeight);
            grpGeneralData.Controls.Add(sedWeight);
            grpGeneralData.Controls.Add(lblVolume);
            grpGeneralData.Controls.Add(sedVolume);
            grpGeneralData.Controls.Add(lblPurchaseUom);
            grpGeneralData.Controls.Add(slePurchaseUom);
            grpGeneralData.Controls.Add(lblSalesUom);
            grpGeneralData.Controls.Add(sleSalesUom);
            grpGeneralData.Controls.Add(lblInventoryUom);
            grpGeneralData.Controls.Add(sleInventoryUom);
            grpGeneralData.Controls.Add(lblGeneralNotes);
            grpGeneralData.Controls.Add(memGeneralNotes);
            grpGeneralData.Location = new Point(9, 10);
            grpGeneralData.Name = "grpGeneralData";
            grpGeneralData.Size = new Size(381, 433);
            grpGeneralData.TabIndex = 0;
            grpGeneralData.Text = "Datos Generales";
            // 
            // lblLongDescription
            // 
            lblLongDescription.Appearance.ForeColor = Color.Black;
            lblLongDescription.Appearance.Options.UseForeColor = true;
            lblLongDescription.Location = new Point(15, 35);
            lblLongDescription.Name = "lblLongDescription";
            lblLongDescription.Size = new Size(88, 13);
            lblLongDescription.TabIndex = 0;
            lblLongDescription.Text = "Descripción Larga:";
            // 
            // memLongDescription
            // 
            memLongDescription.EditValue = "Arroz blanco de grano largo, seleccionado y empacado bajo estandares de calidad premium.";
            memLongDescription.Location = new Point(129, 31);
            memLongDescription.Name = "memLongDescription";
            memLongDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            memLongDescription.Properties.Appearance.Options.UseFont = true;
            memLongDescription.Size = new Size(231, 50);
            memLongDescription.TabIndex = 1;
            // 
            // lblCategory
            // 
            lblCategory.Appearance.ForeColor = Color.Black;
            lblCategory.Appearance.Options.UseForeColor = true;
            lblCategory.Location = new Point(15, 97);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(51, 13);
            lblCategory.TabIndex = 2;
            lblCategory.Text = "Categoría:";
            // 
            // sleCategory
            // 
            sleCategory.EditValue = "ALIMENTOS";
            sleCategory.Location = new Point(129, 94);
            sleCategory.Name = "sleCategory";
            sleCategory.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleCategory.Properties.DisplayMember = "DisplayText";
            sleCategory.Properties.NullText = "";
            sleCategory.Properties.PopupView = grvCategoryLookup;
            sleCategory.Properties.ValueMember = "Id";
            sleCategory.Size = new Size(231, 20);
            sleCategory.TabIndex = 3;
            // 
            // grvCategoryLookup
            // 
            grvCategoryLookup.DetailHeight = 303;
            grvCategoryLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvCategoryLookup.Name = "grvCategoryLookup";
            grvCategoryLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvCategoryLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvCategoryLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblSubCategory
            // 
            lblSubCategory.Appearance.ForeColor = Color.Black;
            lblSubCategory.Appearance.Options.UseForeColor = true;
            lblSubCategory.Location = new Point(15, 125);
            lblSubCategory.Name = "lblSubCategory";
            lblSubCategory.Size = new Size(67, 13);
            lblSubCategory.TabIndex = 4;
            lblSubCategory.Text = "Subcategoría:";
            // 
            // sleSubCategory
            // 
            sleSubCategory.EditValue = "GRANOS";
            sleSubCategory.Location = new Point(129, 121);
            sleSubCategory.Name = "sleSubCategory";
            sleSubCategory.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleSubCategory.Properties.DisplayMember = "DisplayText";
            sleSubCategory.Properties.NullText = "";
            sleSubCategory.Properties.PopupView = grvSubCategoryLookup;
            sleSubCategory.Properties.ValueMember = "Id";
            sleSubCategory.Size = new Size(231, 20);
            sleSubCategory.TabIndex = 5;
            // 
            // grvSubCategoryLookup
            // 
            grvSubCategoryLookup.DetailHeight = 303;
            grvSubCategoryLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvSubCategoryLookup.Name = "grvSubCategoryLookup";
            grvSubCategoryLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvSubCategoryLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvSubCategoryLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblManufacturer
            // 
            lblManufacturer.Appearance.ForeColor = Color.Black;
            lblManufacturer.Appearance.Options.UseForeColor = true;
            lblManufacturer.Location = new Point(15, 153);
            lblManufacturer.Name = "lblManufacturer";
            lblManufacturer.Size = new Size(55, 13);
            lblManufacturer.TabIndex = 6;
            lblManufacturer.Text = "Fabricante:";
            // 
            // sleManufacturer
            // 
            sleManufacturer.EditValue = "INDUSTRIAL XYZ S.A.";
            sleManufacturer.Location = new Point(129, 149);
            sleManufacturer.Name = "sleManufacturer";
            sleManufacturer.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleManufacturer.Properties.DisplayMember = "DisplayText";
            sleManufacturer.Properties.NullText = "";
            sleManufacturer.Properties.PopupView = grvManufacturerLookup;
            sleManufacturer.Properties.ValueMember = "Id";
            sleManufacturer.Size = new Size(231, 20);
            sleManufacturer.TabIndex = 7;
            // 
            // grvManufacturerLookup
            // 
            grvManufacturerLookup.DetailHeight = 303;
            grvManufacturerLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvManufacturerLookup.Name = "grvManufacturerLookup";
            grvManufacturerLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvManufacturerLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvManufacturerLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblModel
            // 
            lblModel.Appearance.ForeColor = Color.Black;
            lblModel.Appearance.Options.UseForeColor = true;
            lblModel.Location = new Point(15, 180);
            lblModel.Name = "lblModel";
            lblModel.Size = new Size(38, 13);
            lblModel.TabIndex = 8;
            lblModel.Text = "Modelo:";
            // 
            // txtModel
            // 
            txtModel.EditValue = "ESTANDAR";
            txtModel.Location = new Point(129, 177);
            txtModel.Name = "txtModel";
            txtModel.Size = new Size(231, 20);
            txtModel.TabIndex = 9;
            // 
            // lblCountry
            // 
            lblCountry.Appearance.ForeColor = Color.Black;
            lblCountry.Appearance.Options.UseForeColor = true;
            lblCountry.Location = new Point(15, 208);
            lblCountry.Name = "lblCountry";
            lblCountry.Size = new Size(73, 13);
            lblCountry.TabIndex = 10;
            lblCountry.Text = "País de Origen:";
            // 
            // lueCountry
            // 
            lueCountry.EditValue = "ECUADOR";
            lueCountry.Location = new Point(129, 205);
            lueCountry.Name = "lueCountry";
            lueCountry.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueCountry.Properties.NullText = "";
            lueCountry.Size = new Size(231, 20);
            lueCountry.TabIndex = 11;
            // 
            // lblAlternateCode
            // 
            lblAlternateCode.Appearance.ForeColor = Color.Black;
            lblAlternateCode.Appearance.Options.UseForeColor = true;
            lblAlternateCode.Location = new Point(15, 236);
            lblAlternateCode.Name = "lblAlternateCode";
            lblAlternateCode.Size = new Size(75, 13);
            lblAlternateCode.TabIndex = 12;
            lblAlternateCode.Text = "Código Alterno:";
            // 
            // txtAlternateCode
            // 
            txtAlternateCode.EditValue = "ARZ-FLOR-2KG";
            txtAlternateCode.Location = new Point(129, 232);
            txtAlternateCode.Name = "txtAlternateCode";
            txtAlternateCode.Size = new Size(231, 20);
            txtAlternateCode.TabIndex = 13;
            // 
            // lblWeight
            // 
            lblWeight.Appearance.ForeColor = Color.Black;
            lblWeight.Appearance.Options.UseForeColor = true;
            lblWeight.Location = new Point(15, 263);
            lblWeight.Name = "lblWeight";
            lblWeight.Size = new Size(27, 13);
            lblWeight.TabIndex = 14;
            lblWeight.Text = "Peso:";
            // 
            // sedWeight
            // 
            sedWeight.EditValue = new decimal(new int[] { 200, 0, 0, 131072 });
            sedWeight.Location = new Point(129, 260);
            sedWeight.Name = "sedWeight";
            sedWeight.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedWeight.Properties.MaskSettings.Set("mask", "n2");
            sedWeight.Size = new Size(96, 20);
            sedWeight.TabIndex = 15;
            // 
            // lblVolume
            // 
            lblVolume.Appearance.ForeColor = Color.Black;
            lblVolume.Appearance.Options.UseForeColor = true;
            lblVolume.Location = new Point(15, 291);
            lblVolume.Name = "lblVolume";
            lblVolume.Size = new Size(44, 13);
            lblVolume.TabIndex = 16;
            lblVolume.Text = "Volumen:";
            // 
            // sedVolume
            // 
            sedVolume.EditValue = new decimal(new int[] { 4, 0, 0, 196608 });
            sedVolume.Location = new Point(129, 288);
            sedVolume.Name = "sedVolume";
            sedVolume.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedVolume.Properties.MaskSettings.Set("mask", "n3");
            sedVolume.Size = new Size(96, 20);
            sedVolume.TabIndex = 17;
            // 
            // lblPurchaseUom
            // 
            lblPurchaseUom.Appearance.ForeColor = Color.Black;
            lblPurchaseUom.Appearance.Options.UseForeColor = true;
            lblPurchaseUom.Location = new Point(15, 319);
            lblPurchaseUom.Name = "lblPurchaseUom";
            lblPurchaseUom.Size = new Size(92, 13);
            lblPurchaseUom.TabIndex = 18;
            lblPurchaseUom.Text = "Unidad de Compra:";
            // 
            // slePurchaseUom
            // 
            slePurchaseUom.EditValue = "UNIDAD";
            slePurchaseUom.Location = new Point(129, 315);
            slePurchaseUom.Name = "slePurchaseUom";
            slePurchaseUom.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            slePurchaseUom.Properties.DisplayMember = "DisplayText";
            slePurchaseUom.Properties.NullText = "";
            slePurchaseUom.Properties.PopupView = grvPurchaseUomLookup;
            slePurchaseUom.Properties.ValueMember = "Id";
            slePurchaseUom.Size = new Size(231, 20);
            slePurchaseUom.TabIndex = 19;
            // 
            // grvPurchaseUomLookup
            // 
            grvPurchaseUomLookup.DetailHeight = 303;
            grvPurchaseUomLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvPurchaseUomLookup.Name = "grvPurchaseUomLookup";
            grvPurchaseUomLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvPurchaseUomLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvPurchaseUomLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblSalesUom
            // 
            lblSalesUom.Appearance.ForeColor = Color.Black;
            lblSalesUom.Appearance.Options.UseForeColor = true;
            lblSalesUom.Location = new Point(15, 347);
            lblSalesUom.Name = "lblSalesUom";
            lblSalesUom.Size = new Size(83, 13);
            lblSalesUom.TabIndex = 20;
            lblSalesUom.Text = "Unidad de Venta:";
            // 
            // sleSalesUom
            // 
            sleSalesUom.EditValue = "UNIDAD";
            sleSalesUom.Location = new Point(129, 343);
            sleSalesUom.Name = "sleSalesUom";
            sleSalesUom.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleSalesUom.Properties.DisplayMember = "DisplayText";
            sleSalesUom.Properties.NullText = "";
            sleSalesUom.Properties.PopupView = grvSalesUomLookup;
            sleSalesUom.Properties.ValueMember = "Id";
            sleSalesUom.Size = new Size(231, 20);
            sleSalesUom.TabIndex = 21;
            // 
            // grvSalesUomLookup
            // 
            grvSalesUomLookup.DetailHeight = 303;
            grvSalesUomLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvSalesUomLookup.Name = "grvSalesUomLookup";
            grvSalesUomLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvSalesUomLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvSalesUomLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblInventoryUom
            // 
            lblInventoryUom.Appearance.ForeColor = Color.Black;
            lblInventoryUom.Appearance.Options.UseForeColor = true;
            lblInventoryUom.Location = new Point(15, 374);
            lblInventoryUom.Name = "lblInventoryUom";
            lblInventoryUom.Size = new Size(105, 13);
            lblInventoryUom.TabIndex = 22;
            lblInventoryUom.Text = "Unidad de Inventario:";
            // 
            // sleInventoryUom
            // 
            sleInventoryUom.EditValue = "UNIDAD";
            sleInventoryUom.Location = new Point(129, 371);
            sleInventoryUom.Name = "sleInventoryUom";
            sleInventoryUom.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleInventoryUom.Properties.DisplayMember = "DisplayText";
            sleInventoryUom.Properties.NullText = "";
            sleInventoryUom.Properties.PopupView = grvInventoryUomLookup;
            sleInventoryUom.Properties.ValueMember = "Id";
            sleInventoryUom.Size = new Size(231, 20);
            sleInventoryUom.TabIndex = 23;
            // 
            // grvInventoryUomLookup
            // 
            grvInventoryUomLookup.DetailHeight = 303;
            grvInventoryUomLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvInventoryUomLookup.Name = "grvInventoryUomLookup";
            grvInventoryUomLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvInventoryUomLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvInventoryUomLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblGeneralNotes
            // 
            lblGeneralNotes.Appearance.ForeColor = Color.Black;
            lblGeneralNotes.Appearance.Options.UseForeColor = true;
            lblGeneralNotes.Location = new Point(15, 402);
            lblGeneralNotes.Name = "lblGeneralNotes";
            lblGeneralNotes.Size = new Size(75, 13);
            lblGeneralNotes.TabIndex = 24;
            lblGeneralNotes.Text = "Observaciones:";
            // 
            // memGeneralNotes
            // 
            memGeneralNotes.EditValue = "Conservar en lugar fresco y seco.";
            memGeneralNotes.Location = new Point(129, 399);
            memGeneralNotes.Name = "memGeneralNotes";
            memGeneralNotes.Size = new Size(231, 24);
            memGeneralNotes.TabIndex = 25;
            // 
            // grpAdditionalInfo
            // 
            grpAdditionalInfo.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpAdditionalInfo.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpAdditionalInfo.AppearanceCaption.Options.UseFont = true;
            grpAdditionalInfo.AppearanceCaption.Options.UseForeColor = true;
            grpAdditionalInfo.Controls.Add(lblSatCode);
            grpAdditionalInfo.Controls.Add(txtSatCode);
            grpAdditionalInfo.Controls.Add(lblUnspscCode);
            grpAdditionalInfo.Controls.Add(txtUnspscCode);
            grpAdditionalInfo.Controls.Add(lblTaxCode);
            grpAdditionalInfo.Controls.Add(txtTaxCode);
            grpAdditionalInfo.Controls.Add(lblTaxType);
            grpAdditionalInfo.Controls.Add(lueTaxType);
            grpAdditionalInfo.Controls.Add(lblBaseCurrency);
            grpAdditionalInfo.Controls.Add(lueBaseCurrency);
            grpAdditionalInfo.Controls.Add(lblDefaultPriceList);
            grpAdditionalInfo.Controls.Add(lueDefaultPriceList);
            grpAdditionalInfo.Controls.Add(lblMaxDiscount);
            grpAdditionalInfo.Controls.Add(sedMaxDiscount);
            grpAdditionalInfo.Controls.Add(lblMaxDiscountSymbol);
            grpAdditionalInfo.Controls.Add(lblLastChange);
            grpAdditionalInfo.Controls.Add(dtpLastChange);
            grpAdditionalInfo.Controls.Add(lblLastChangeUser);
            grpAdditionalInfo.Controls.Add(txtLastChangeUser);
            grpAdditionalInfo.Location = new Point(403, 10);
            grpAdditionalInfo.Name = "grpAdditionalInfo";
            grpAdditionalInfo.Size = new Size(317, 269);
            grpAdditionalInfo.TabIndex = 1;
            grpAdditionalInfo.Text = "Informacion Adicional";
            // 
            // lblSatCode
            // 
            lblSatCode.Appearance.ForeColor = Color.Black;
            lblSatCode.Appearance.Options.UseForeColor = true;
            lblSatCode.Location = new Point(14, 36);
            lblSatCode.Name = "lblSatCode";
            lblSatCode.Size = new Size(59, 13);
            lblSatCode.TabIndex = 0;
            lblSatCode.Text = "Código SAT:";
            // 
            // txtSatCode
            // 
            txtSatCode.EditValue = "10063001";
            txtSatCode.Location = new Point(129, 33);
            txtSatCode.Name = "txtSatCode";
            txtSatCode.Size = new Size(171, 20);
            txtSatCode.TabIndex = 1;
            // 
            // lblUnspscCode
            // 
            lblUnspscCode.Appearance.ForeColor = Color.Black;
            lblUnspscCode.Appearance.Options.UseForeColor = true;
            lblUnspscCode.Location = new Point(14, 64);
            lblUnspscCode.Name = "lblUnspscCode";
            lblUnspscCode.Size = new Size(79, 13);
            lblUnspscCode.TabIndex = 2;
            lblUnspscCode.Text = "Código UNSPSC:";
            // 
            // txtUnspscCode
            // 
            txtUnspscCode.EditValue = "50161500";
            txtUnspscCode.Location = new Point(129, 61);
            txtUnspscCode.Name = "txtUnspscCode";
            txtUnspscCode.Size = new Size(171, 20);
            txtUnspscCode.TabIndex = 3;
            // 
            // lblTaxCode
            // 
            lblTaxCode.Appearance.ForeColor = Color.Black;
            lblTaxCode.Appearance.Options.UseForeColor = true;
            lblTaxCode.Location = new Point(14, 92);
            lblTaxCode.Name = "lblTaxCode";
            lblTaxCode.Size = new Size(86, 13);
            lblTaxCode.TabIndex = 4;
            lblTaxCode.Text = "Código Tributario:";
            // 
            // txtTaxCode
            // 
            txtTaxCode.EditValue = "A0";
            txtTaxCode.Location = new Point(129, 88);
            txtTaxCode.Name = "txtTaxCode";
            txtTaxCode.Size = new Size(171, 20);
            txtTaxCode.TabIndex = 5;
            // 
            // lblTaxType
            // 
            lblTaxType.Appearance.ForeColor = Color.Black;
            lblTaxType.Appearance.Options.UseForeColor = true;
            lblTaxType.Location = new Point(14, 120);
            lblTaxType.Name = "lblTaxType";
            lblTaxType.Size = new Size(87, 13);
            lblTaxType.TabIndex = 6;
            lblTaxType.Text = "Tipo de Impuesto:";
            // 
            // lueTaxType
            // 
            lueTaxType.EditValue = "IVA 12%";
            lueTaxType.Location = new Point(129, 116);
            lueTaxType.Name = "lueTaxType";
            lueTaxType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueTaxType.Properties.NullText = "";
            lueTaxType.Size = new Size(171, 20);
            lueTaxType.TabIndex = 7;
            // 
            // lblBaseCurrency
            // 
            lblBaseCurrency.Appearance.ForeColor = Color.Black;
            lblBaseCurrency.Appearance.Options.UseForeColor = true;
            lblBaseCurrency.Location = new Point(14, 147);
            lblBaseCurrency.Name = "lblBaseCurrency";
            lblBaseCurrency.Size = new Size(68, 13);
            lblBaseCurrency.TabIndex = 8;
            lblBaseCurrency.Text = "Moneda Base:";
            // 
            // lueBaseCurrency
            // 
            lueBaseCurrency.EditValue = "USD - Dolar Americano";
            lueBaseCurrency.Location = new Point(129, 144);
            lueBaseCurrency.Name = "lueBaseCurrency";
            lueBaseCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueBaseCurrency.Properties.NullText = "";
            lueBaseCurrency.Size = new Size(171, 20);
            lueBaseCurrency.TabIndex = 9;
            // 
            // lblDefaultPriceList
            // 
            lblDefaultPriceList.Appearance.ForeColor = Color.Black;
            lblDefaultPriceList.Appearance.Options.UseForeColor = true;
            lblDefaultPriceList.Location = new Point(14, 175);
            lblDefaultPriceList.Name = "lblDefaultPriceList";
            lblDefaultPriceList.Size = new Size(116, 13);
            lblDefaultPriceList.TabIndex = 10;
            lblDefaultPriceList.Text = "Lista de Precios Default:";
            // 
            // lueDefaultPriceList
            // 
            lueDefaultPriceList.EditValue = "LISTA GENERAL";
            lueDefaultPriceList.Location = new Point(129, 172);
            lueDefaultPriceList.Name = "lueDefaultPriceList";
            lueDefaultPriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueDefaultPriceList.Properties.NullText = "";
            lueDefaultPriceList.Size = new Size(171, 20);
            lueDefaultPriceList.TabIndex = 11;
            // 
            // lblMaxDiscount
            // 
            lblMaxDiscount.Appearance.ForeColor = Color.Black;
            lblMaxDiscount.Appearance.Options.UseForeColor = true;
            lblMaxDiscount.Location = new Point(14, 203);
            lblMaxDiscount.Name = "lblMaxDiscount";
            lblMaxDiscount.Size = new Size(116, 13);
            lblMaxDiscount.TabIndex = 12;
            lblMaxDiscount.Text = "Descuento Máximo (%):";
            // 
            // sedMaxDiscount
            // 
            sedMaxDiscount.EditValue = new decimal(new int[] { 1500, 0, 0, 131072 });
            sedMaxDiscount.Location = new Point(129, 199);
            sedMaxDiscount.Name = "sedMaxDiscount";
            sedMaxDiscount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedMaxDiscount.Properties.MaskSettings.Set("mask", "n2");
            sedMaxDiscount.Size = new Size(120, 20);
            sedMaxDiscount.TabIndex = 13;
            // 
            // lblMaxDiscountSymbol
            // 
            lblMaxDiscountSymbol.Appearance.ForeColor = Color.Black;
            lblMaxDiscountSymbol.Appearance.Options.UseForeColor = true;
            lblMaxDiscountSymbol.Location = new Point(257, 203);
            lblMaxDiscountSymbol.Name = "lblMaxDiscountSymbol";
            lblMaxDiscountSymbol.Size = new Size(11, 13);
            lblMaxDiscountSymbol.TabIndex = 14;
            lblMaxDiscountSymbol.Text = "%";
            // 
            // lblLastChange
            // 
            lblLastChange.Appearance.ForeColor = Color.Black;
            lblLastChange.Appearance.Options.UseForeColor = true;
            lblLastChange.Location = new Point(14, 231);
            lblLastChange.Name = "lblLastChange";
            lblLastChange.Size = new Size(103, 13);
            lblLastChange.TabIndex = 15;
            lblLastChange.Text = "Fecha Último Cambio:";
            // 
            // dtpLastChange
            // 
            dtpLastChange.EditValue = new DateTime(2026, 5, 10, 0, 0, 0, 0);
            dtpLastChange.Location = new Point(129, 227);
            dtpLastChange.Name = "dtpLastChange";
            dtpLastChange.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            dtpLastChange.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            dtpLastChange.Size = new Size(120, 20);
            dtpLastChange.TabIndex = 16;
            // 
            // lblLastChangeUser
            // 
            lblLastChangeUser.Appearance.ForeColor = Color.Black;
            lblLastChangeUser.Appearance.Options.UseForeColor = true;
            lblLastChangeUser.Location = new Point(14, 255);
            lblLastChangeUser.Name = "lblLastChangeUser";
            lblLastChangeUser.Size = new Size(98, 13);
            lblLastChangeUser.TabIndex = 17;
            lblLastChangeUser.Text = "Usuario Últ. Cambio:";
            // 
            // txtLastChangeUser
            // 
            txtLastChangeUser.EditValue = "admin";
            txtLastChangeUser.Location = new Point(129, 251);
            txtLastChangeUser.Name = "txtLastChangeUser";
            txtLastChangeUser.Size = new Size(120, 20);
            txtLastChangeUser.TabIndex = 18;
            // 
            // grpAttributes
            // 
            grpAttributes.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpAttributes.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpAttributes.AppearanceCaption.Options.UseFont = true;
            grpAttributes.AppearanceCaption.Options.UseForeColor = true;
            grpAttributes.Controls.Add(grcAttributes);
            grpAttributes.Location = new Point(733, 10);
            grpAttributes.Name = "grpAttributes";
            grpAttributes.Size = new Size(291, 269);
            grpAttributes.TabIndex = 2;
            grpAttributes.Text = "Atributos Personalizados";
            // 
            // grcAttributes
            // 
            grcAttributes.Dock = DockStyle.Fill;
            grcAttributes.Location = new Point(2, 23);
            grcAttributes.MainView = grvAttributes;
            grcAttributes.Name = "grcAttributes";
            grcAttributes.Size = new Size(287, 244);
            grcAttributes.TabIndex = 0;
            grcAttributes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvAttributes });
            // 
            // grvAttributes
            // 
            grvAttributes.Columns.AddRange(new GridColumn[] { colAttribute, colValue });
            grvAttributes.DetailHeight = 303;
            grvAttributes.GridControl = grcAttributes;
            grvAttributes.Name = "grvAttributes";
            grvAttributes.OptionsEditForm.PopupEditFormWidth = 686;
            grvAttributes.OptionsView.ShowGroupPanel = false;
            // 
            // colAttribute
            // 
            colAttribute.Caption = "Atributo";
            colAttribute.FieldName = "Atributo";
            colAttribute.MinWidth = 17;
            colAttribute.Name = "colAttribute";
            colAttribute.Visible = true;
            colAttribute.VisibleIndex = 0;
            colAttribute.Width = 137;
            // 
            // colValue
            // 
            colValue.Caption = "Valor";
            colValue.FieldName = "Valor";
            colValue.MinWidth = 17;
            colValue.Name = "colValue";
            colValue.Visible = true;
            colValue.VisibleIndex = 1;
            colValue.Width = 129;
            // 
            // grpAdditionalImages
            // 
            grpAdditionalImages.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpAdditionalImages.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpAdditionalImages.AppearanceCaption.Options.UseFont = true;
            grpAdditionalImages.AppearanceCaption.Options.UseForeColor = true;
            grpAdditionalImages.Controls.Add(picAdditionalImage1);
            grpAdditionalImages.Controls.Add(btnAddAdditionalImage);
            grpAdditionalImages.Location = new Point(403, 290);
            grpAdditionalImages.Name = "grpAdditionalImages";
            grpAdditionalImages.Size = new Size(621, 153);
            grpAdditionalImages.TabIndex = 3;
            grpAdditionalImages.Text = "Imagenes Adicionales";
            // 
            // picAdditionalImage1
            // 
            picAdditionalImage1.Location = new Point(15, 31);
            picAdditionalImage1.Name = "picAdditionalImage1";
            picAdditionalImage1.Properties.NullText = "Imagen";
            picAdditionalImage1.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
            picAdditionalImage1.Properties.SizeMode = PictureSizeMode.Zoom;
            picAdditionalImage1.Size = new Size(86, 87);
            picAdditionalImage1.TabIndex = 0;
            // 
            // btnAddAdditionalImage
            // 
            btnAddAdditionalImage.Appearance.Font = new Font("Segoe UI", 9F);
            btnAddAdditionalImage.Appearance.Options.UseFont = true;
            btnAddAdditionalImage.Location = new Point(120, 61);
            btnAddAdditionalImage.Name = "btnAddAdditionalImage";
            btnAddAdditionalImage.Size = new Size(103, 26);
            btnAddAdditionalImage.TabIndex = 1;
            btnAddAdditionalImage.Text = "Agregar Imagen";
            // 
            // xtpInventory
            // 
            xtpInventory.Controls.Add(grpInventoryParams);
            xtpInventory.Controls.Add(grpStockControl);
            xtpInventory.Controls.Add(grpStockByWarehouse);
            xtpInventory.Name = "xtpInventory";
            xtpInventory.Size = new Size(1088, 455);
            xtpInventory.Text = "Inventario";
            // 
            // grpInventoryParams
            // 
            grpInventoryParams.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpInventoryParams.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpInventoryParams.AppearanceCaption.Options.UseFont = true;
            grpInventoryParams.AppearanceCaption.Options.UseForeColor = true;
            grpInventoryParams.Controls.Add(lblDefaultWarehouse);
            grpInventoryParams.Controls.Add(sleDefaultWarehouse);
            grpInventoryParams.Controls.Add(lblValuationMethod);
            grpInventoryParams.Controls.Add(lueValuationMethod);
            grpInventoryParams.Controls.Add(lblInventoryAccount);
            grpInventoryParams.Controls.Add(sleInventoryAccount);
            grpInventoryParams.Controls.Add(lblCostAccount);
            grpInventoryParams.Controls.Add(sleCostAccount);
            grpInventoryParams.Controls.Add(lblAdditionalCostHandling);
            grpInventoryParams.Controls.Add(lueAdditionalCostHandling);
            grpInventoryParams.Controls.Add(lblManageBatch);
            grpInventoryParams.Controls.Add(chkBatch);
            grpInventoryParams.Controls.Add(lblManageSerial);
            grpInventoryParams.Controls.Add(chkSerial);
            grpInventoryParams.Controls.Add(lblManageBinLocation);
            grpInventoryParams.Controls.Add(chkBinLocation);
            grpInventoryParams.Location = new Point(9, 9);
            grpInventoryParams.Name = "grpInventoryParams";
            grpInventoryParams.Size = new Size(446, 199);
            grpInventoryParams.TabIndex = 0;
            grpInventoryParams.Text = "Parametros de Inventario";
            // 
            // lblDefaultWarehouse
            // 
            lblDefaultWarehouse.Appearance.ForeColor = Color.Black;
            lblDefaultWarehouse.Appearance.Options.UseForeColor = true;
            lblDefaultWarehouse.Location = new Point(17, 35);
            lblDefaultWarehouse.Name = "lblDefaultWarehouse";
            lblDefaultWarehouse.Size = new Size(100, 13);
            lblDefaultWarehouse.TabIndex = 0;
            lblDefaultWarehouse.Text = "Bodega por Defecto:";
            // 
            // sleDefaultWarehouse
            // 
            sleDefaultWarehouse.EditValue = "01 - PRINCIPAL";
            sleDefaultWarehouse.Location = new Point(189, 31);
            sleDefaultWarehouse.Name = "sleDefaultWarehouse";
            sleDefaultWarehouse.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleDefaultWarehouse.Properties.NullText = "";
            sleDefaultWarehouse.Properties.PopupView = grvDefaultWarehouseLookup;
            sleDefaultWarehouse.Size = new Size(231, 20);
            sleDefaultWarehouse.TabIndex = 1;
            // 
            // grvDefaultWarehouseLookup
            // 
            grvDefaultWarehouseLookup.DetailHeight = 303;
            grvDefaultWarehouseLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvDefaultWarehouseLookup.Name = "grvDefaultWarehouseLookup";
            grvDefaultWarehouseLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvDefaultWarehouseLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvDefaultWarehouseLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblValuationMethod
            // 
            lblValuationMethod.Appearance.ForeColor = Color.Black;
            lblValuationMethod.Appearance.Options.UseForeColor = true;
            lblValuationMethod.Location = new Point(17, 61);
            lblValuationMethod.Name = "lblValuationMethod";
            lblValuationMethod.Size = new Size(107, 13);
            lblValuationMethod.TabIndex = 2;
            lblValuationMethod.Text = "Método de Valoración:";
            // 
            // lueValuationMethod
            // 
            lueValuationMethod.EditValue = "Promedio Ponderado";
            lueValuationMethod.Location = new Point(189, 57);
            lueValuationMethod.Name = "lueValuationMethod";
            lueValuationMethod.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueValuationMethod.Properties.DataSource = new string[]
    {
    "Promedio Ponderado",
    "FIFO",
    "Costo Estandar"
    };
            lueValuationMethod.Properties.NullText = "";
            lueValuationMethod.Size = new Size(231, 20);
            lueValuationMethod.TabIndex = 3;
            // 
            // lblInventoryAccount
            // 
            lblInventoryAccount.Appearance.ForeColor = Color.Black;
            lblInventoryAccount.Appearance.Options.UseForeColor = true;
            lblInventoryAccount.Location = new Point(17, 87);
            lblInventoryAccount.Name = "lblInventoryAccount";
            lblInventoryAccount.Size = new Size(107, 13);
            lblInventoryAccount.TabIndex = 4;
            lblInventoryAccount.Text = "Cuenta de Inventario:";
            // 
            // sleInventoryAccount
            // 
            sleInventoryAccount.EditValue = "1.1.01.01 - Inventario de Mercaderias";
            sleInventoryAccount.Location = new Point(189, 83);
            sleInventoryAccount.Name = "sleInventoryAccount";
            sleInventoryAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleInventoryAccount.Properties.NullText = "";
            sleInventoryAccount.Properties.PopupView = grvInventoryAccountLookup;
            sleInventoryAccount.Size = new Size(231, 20);
            sleInventoryAccount.TabIndex = 5;
            // 
            // grvInventoryAccountLookup
            // 
            grvInventoryAccountLookup.DetailHeight = 303;
            grvInventoryAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvInventoryAccountLookup.Name = "grvInventoryAccountLookup";
            grvInventoryAccountLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvInventoryAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvInventoryAccountLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblCostAccount
            // 
            lblCostAccount.Appearance.ForeColor = Color.Black;
            lblCostAccount.Appearance.Options.UseForeColor = true;
            lblCostAccount.Location = new Point(17, 113);
            lblCostAccount.Name = "lblCostAccount";
            lblCostAccount.Size = new Size(121, 13);
            lblCostAccount.TabIndex = 6;
            lblCostAccount.Text = "Cuenta de Costo Ventas:";
            // 
            // sleCostAccount
            // 
            sleCostAccount.EditValue = "5.1.01.01 - Costo de Ventas";
            sleCostAccount.Location = new Point(189, 109);
            sleCostAccount.Name = "sleCostAccount";
            sleCostAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleCostAccount.Properties.NullText = "";
            sleCostAccount.Properties.PopupView = grvCostAccountLookup;
            sleCostAccount.Size = new Size(231, 20);
            sleCostAccount.TabIndex = 7;
            // 
            // grvCostAccountLookup
            // 
            grvCostAccountLookup.DetailHeight = 303;
            grvCostAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvCostAccountLookup.Name = "grvCostAccountLookup";
            grvCostAccountLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvCostAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvCostAccountLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblAdditionalCostHandling
            // 
            lblAdditionalCostHandling.Appearance.ForeColor = Color.Black;
            lblAdditionalCostHandling.Appearance.Options.UseForeColor = true;
            lblAdditionalCostHandling.Location = new Point(17, 139);
            lblAdditionalCostHandling.Name = "lblAdditionalCostHandling";
            lblAdditionalCostHandling.Size = new Size(146, 13);
            lblAdditionalCostHandling.TabIndex = 8;
            lblAdditionalCostHandling.Text = "Manejo de Costos Adicionales:";
            // 
            // lueAdditionalCostHandling
            // 
            lueAdditionalCostHandling.EditValue = "Incluir en el Costo";
            lueAdditionalCostHandling.Location = new Point(189, 135);
            lueAdditionalCostHandling.Name = "lueAdditionalCostHandling";
            lueAdditionalCostHandling.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueAdditionalCostHandling.Properties.DataSource = new string[]
    {
    "Incluir en el Costo",
    "Registrar como Gasto",
    "No Aplicar"
    };
            lueAdditionalCostHandling.Properties.NullText = "";
            lueAdditionalCostHandling.Size = new Size(231, 20);
            lueAdditionalCostHandling.TabIndex = 9;
            // 
            // lblManageBatch
            // 
            lblManageBatch.Appearance.ForeColor = Color.Black;
            lblManageBatch.Appearance.Options.UseForeColor = true;
            lblManageBatch.Location = new Point(17, 165);
            lblManageBatch.Name = "lblManageBatch";
            lblManageBatch.Size = new Size(59, 13);
            lblManageBatch.TabIndex = 10;
            lblManageBatch.Text = "Maneja Lote";
            // 
            // chkBatch
            // 
            chkBatch.EditValue = true;
            chkBatch.Location = new Point(94, 163);
            chkBatch.Name = "chkBatch";
            chkBatch.Properties.Caption = "";
            chkBatch.Size = new Size(21, 20);
            chkBatch.TabIndex = 11;
            // 
            // lblManageSerial
            // 
            lblManageSerial.Appearance.ForeColor = Color.Black;
            lblManageSerial.Appearance.Options.UseForeColor = true;
            lblManageSerial.Location = new Point(154, 165);
            lblManageSerial.Name = "lblManageSerial";
            lblManageSerial.Size = new Size(62, 13);
            lblManageSerial.TabIndex = 12;
            lblManageSerial.Text = "Maneja Serie";
            // 
            // chkSerial
            // 
            chkSerial.Location = new Point(231, 163);
            chkSerial.Name = "chkSerial";
            chkSerial.Properties.Caption = "";
            chkSerial.Size = new Size(21, 20);
            chkSerial.TabIndex = 13;
            // 
            // lblManageBinLocation
            // 
            lblManageBinLocation.Appearance.ForeColor = Color.Black;
            lblManageBinLocation.Appearance.Options.UseForeColor = true;
            lblManageBinLocation.Location = new Point(283, 165);
            lblManageBinLocation.Name = "lblManageBinLocation";
            lblManageBinLocation.Size = new Size(83, 13);
            lblManageBinLocation.TabIndex = 14;
            lblManageBinLocation.Text = "Maneja Ubicación";
            // 
            // chkBinLocation
            // 
            chkBinLocation.EditValue = true;
            chkBinLocation.Location = new Point(381, 163);
            chkBinLocation.Name = "chkBinLocation";
            chkBinLocation.Properties.Caption = "";
            chkBinLocation.Size = new Size(21, 20);
            chkBinLocation.TabIndex = 15;
            // 
            // grpStockControl
            // 
            grpStockControl.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpStockControl.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpStockControl.AppearanceCaption.Options.UseFont = true;
            grpStockControl.AppearanceCaption.Options.UseForeColor = true;
            grpStockControl.Controls.Add(lblMinStock);
            grpStockControl.Controls.Add(sedMinStock);
            grpStockControl.Controls.Add(lblMinStockUnit);
            grpStockControl.Controls.Add(lblMaxStock);
            grpStockControl.Controls.Add(sedMaxStock);
            grpStockControl.Controls.Add(lblMaxStockUnit);
            grpStockControl.Controls.Add(lblReorderPoint);
            grpStockControl.Controls.Add(sedReorderPoint);
            grpStockControl.Controls.Add(lblReorderPointUnit);
            grpStockControl.Controls.Add(lblCurrentStock);
            grpStockControl.Controls.Add(cleCurrentStock);
            grpStockControl.Controls.Add(lblCurrentStockUnit);
            grpStockControl.Controls.Add(lblCommitted);
            grpStockControl.Controls.Add(cleCommitted);
            grpStockControl.Controls.Add(lblCommittedUnit);
            grpStockControl.Controls.Add(lblOrdered);
            grpStockControl.Controls.Add(cleOrdered);
            grpStockControl.Controls.Add(lblOrderedUnit);
            grpStockControl.Controls.Add(lblAvailable);
            grpStockControl.Controls.Add(cleAvailable);
            grpStockControl.Controls.Add(lblAvailableUnit);
            grpStockControl.Location = new Point(463, 9);
            grpStockControl.Name = "grpStockControl";
            grpStockControl.Size = new Size(591, 199);
            grpStockControl.TabIndex = 1;
            grpStockControl.Text = "Control de Stock";
            // 
            // lblMinStock
            // 
            lblMinStock.Appearance.ForeColor = Color.Black;
            lblMinStock.Appearance.Options.UseForeColor = true;
            lblMinStock.Location = new Point(17, 48);
            lblMinStock.Name = "lblMinStock";
            lblMinStock.Size = new Size(65, 13);
            lblMinStock.TabIndex = 0;
            lblMinStock.Text = "Stock Mínimo:";
            // 
            // sedMinStock
            // 
            sedMinStock.EditValue = new decimal(new int[] { 20000, 0, 0, 131072 });
            sedMinStock.Location = new Point(129, 43);
            sedMinStock.Name = "sedMinStock";
            sedMinStock.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedMinStock.Properties.MaskSettings.Set("mask", "n2");
            sedMinStock.Size = new Size(103, 20);
            sedMinStock.TabIndex = 1;
            // 
            // lblMinStockUnit
            // 
            lblMinStockUnit.Appearance.ForeColor = Color.Black;
            lblMinStockUnit.Appearance.Options.UseForeColor = true;
            lblMinStockUnit.Location = new Point(240, 48);
            lblMinStockUnit.Name = "lblMinStockUnit";
            lblMinStockUnit.Size = new Size(14, 13);
            lblMinStockUnit.TabIndex = 2;
            lblMinStockUnit.Text = "UN";
            // 
            // lblMaxStock
            // 
            lblMaxStock.Appearance.ForeColor = Color.Black;
            lblMaxStock.Appearance.Options.UseForeColor = true;
            lblMaxStock.Location = new Point(17, 82);
            lblMaxStock.Name = "lblMaxStock";
            lblMaxStock.Size = new Size(69, 13);
            lblMaxStock.TabIndex = 3;
            lblMaxStock.Text = "Stock Máximo:";
            // 
            // sedMaxStock
            // 
            sedMaxStock.EditValue = new decimal(new int[] { 200000, 0, 0, 131072 });
            sedMaxStock.Location = new Point(129, 78);
            sedMaxStock.Name = "sedMaxStock";
            sedMaxStock.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedMaxStock.Properties.MaskSettings.Set("mask", "n2");
            sedMaxStock.Size = new Size(103, 20);
            sedMaxStock.TabIndex = 4;
            // 
            // lblMaxStockUnit
            // 
            lblMaxStockUnit.Appearance.ForeColor = Color.Black;
            lblMaxStockUnit.Appearance.Options.UseForeColor = true;
            lblMaxStockUnit.Location = new Point(240, 82);
            lblMaxStockUnit.Name = "lblMaxStockUnit";
            lblMaxStockUnit.Size = new Size(14, 13);
            lblMaxStockUnit.TabIndex = 5;
            lblMaxStockUnit.Text = "UN";
            // 
            // lblReorderPoint
            // 
            lblReorderPoint.Appearance.ForeColor = Color.Black;
            lblReorderPoint.Appearance.Options.UseForeColor = true;
            lblReorderPoint.Location = new Point(17, 117);
            lblReorderPoint.Name = "lblReorderPoint";
            lblReorderPoint.Size = new Size(91, 13);
            lblReorderPoint.TabIndex = 6;
            lblReorderPoint.Text = "Punto de Reorden:";
            // 
            // sedReorderPoint
            // 
            sedReorderPoint.EditValue = new decimal(new int[] { 30000, 0, 0, 131072 });
            sedReorderPoint.Location = new Point(129, 113);
            sedReorderPoint.Name = "sedReorderPoint";
            sedReorderPoint.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedReorderPoint.Properties.MaskSettings.Set("mask", "n2");
            sedReorderPoint.Size = new Size(103, 20);
            sedReorderPoint.TabIndex = 7;
            // 
            // lblReorderPointUnit
            // 
            lblReorderPointUnit.Appearance.ForeColor = Color.Black;
            lblReorderPointUnit.Appearance.Options.UseForeColor = true;
            lblReorderPointUnit.Location = new Point(240, 117);
            lblReorderPointUnit.Name = "lblReorderPointUnit";
            lblReorderPointUnit.Size = new Size(14, 13);
            lblReorderPointUnit.TabIndex = 8;
            lblReorderPointUnit.Text = "UN";
            // 
            // lblCurrentStock
            // 
            lblCurrentStock.Appearance.ForeColor = Color.Black;
            lblCurrentStock.Appearance.Options.UseForeColor = true;
            lblCurrentStock.Location = new Point(317, 48);
            lblCurrentStock.Name = "lblCurrentStock";
            lblCurrentStock.Size = new Size(63, 13);
            lblCurrentStock.TabIndex = 9;
            lblCurrentStock.Text = "Stock Actual:";
            // 
            // cleCurrentStock
            // 
            cleCurrentStock.EditValue = new decimal(new int[] { 125000, 0, 0, 131072 });
            cleCurrentStock.Location = new Point(429, 43);
            cleCurrentStock.Name = "cleCurrentStock";
            cleCurrentStock.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cleCurrentStock.Properties.Appearance.ForeColor = Color.FromArgb(0, 135, 60);
            cleCurrentStock.Properties.Appearance.Options.UseFont = true;
            cleCurrentStock.Properties.Appearance.Options.UseForeColor = true;
            cleCurrentStock.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleCurrentStock.Properties.DisplayFormat.FormatString = "n2";
            cleCurrentStock.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleCurrentStock.Properties.ReadOnly = true;
            cleCurrentStock.Size = new Size(103, 22);
            cleCurrentStock.TabIndex = 10;
            // 
            // lblCurrentStockUnit
            // 
            lblCurrentStockUnit.Appearance.ForeColor = Color.Black;
            lblCurrentStockUnit.Appearance.Options.UseForeColor = true;
            lblCurrentStockUnit.Location = new Point(540, 48);
            lblCurrentStockUnit.Name = "lblCurrentStockUnit";
            lblCurrentStockUnit.Size = new Size(14, 13);
            lblCurrentStockUnit.TabIndex = 11;
            lblCurrentStockUnit.Text = "UN";
            // 
            // lblCommitted
            // 
            lblCommitted.Appearance.ForeColor = Color.Black;
            lblCommitted.Appearance.Options.UseForeColor = true;
            lblCommitted.Location = new Point(317, 82);
            lblCommitted.Name = "lblCommitted";
            lblCommitted.Size = new Size(73, 13);
            lblCommitted.TabIndex = 12;
            lblCommitted.Text = "Comprometido:";
            // 
            // cleCommitted
            // 
            cleCommitted.EditValue = new decimal(new int[] { 12000, 0, 0, 131072 });
            cleCommitted.Location = new Point(429, 78);
            cleCommitted.Name = "cleCommitted";
            cleCommitted.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cleCommitted.Properties.Appearance.ForeColor = Color.FromArgb(0, 86, 210);
            cleCommitted.Properties.Appearance.Options.UseFont = true;
            cleCommitted.Properties.Appearance.Options.UseForeColor = true;
            cleCommitted.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleCommitted.Properties.DisplayFormat.FormatString = "n2";
            cleCommitted.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleCommitted.Properties.ReadOnly = true;
            cleCommitted.Size = new Size(103, 22);
            cleCommitted.TabIndex = 13;
            // 
            // lblCommittedUnit
            // 
            lblCommittedUnit.Appearance.ForeColor = Color.Black;
            lblCommittedUnit.Appearance.Options.UseForeColor = true;
            lblCommittedUnit.Location = new Point(540, 82);
            lblCommittedUnit.Name = "lblCommittedUnit";
            lblCommittedUnit.Size = new Size(14, 13);
            lblCommittedUnit.TabIndex = 14;
            lblCommittedUnit.Text = "UN";
            // 
            // lblOrdered
            // 
            lblOrdered.Appearance.ForeColor = Color.Black;
            lblOrdered.Appearance.Options.UseForeColor = true;
            lblOrdered.Location = new Point(317, 117);
            lblOrdered.Name = "lblOrdered";
            lblOrdered.Size = new Size(36, 13);
            lblOrdered.TabIndex = 15;
            lblOrdered.Text = "Pedido:";
            // 
            // cleOrdered
            // 
            cleOrdered.EditValue = new decimal(new int[] { 8000, 0, 0, 131072 });
            cleOrdered.Location = new Point(429, 113);
            cleOrdered.Name = "cleOrdered";
            cleOrdered.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cleOrdered.Properties.Appearance.ForeColor = Color.FromArgb(218, 100, 0);
            cleOrdered.Properties.Appearance.Options.UseFont = true;
            cleOrdered.Properties.Appearance.Options.UseForeColor = true;
            cleOrdered.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleOrdered.Properties.DisplayFormat.FormatString = "n2";
            cleOrdered.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleOrdered.Properties.ReadOnly = true;
            cleOrdered.Size = new Size(103, 22);
            cleOrdered.TabIndex = 16;
            // 
            // lblOrderedUnit
            // 
            lblOrderedUnit.Appearance.ForeColor = Color.Black;
            lblOrderedUnit.Appearance.Options.UseForeColor = true;
            lblOrderedUnit.Location = new Point(540, 117);
            lblOrderedUnit.Name = "lblOrderedUnit";
            lblOrderedUnit.Size = new Size(14, 13);
            lblOrderedUnit.TabIndex = 17;
            lblOrderedUnit.Text = "UN";
            // 
            // lblAvailable
            // 
            lblAvailable.Appearance.ForeColor = Color.Black;
            lblAvailable.Appearance.Options.UseForeColor = true;
            lblAvailable.Location = new Point(317, 152);
            lblAvailable.Name = "lblAvailable";
            lblAvailable.Size = new Size(52, 13);
            lblAvailable.TabIndex = 18;
            lblAvailable.Text = "Disponible:";
            // 
            // cleAvailable
            // 
            cleAvailable.EditValue = new decimal(new int[] { 105000, 0, 0, 131072 });
            cleAvailable.Location = new Point(429, 147);
            cleAvailable.Name = "cleAvailable";
            cleAvailable.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cleAvailable.Properties.Appearance.ForeColor = Color.FromArgb(0, 135, 60);
            cleAvailable.Properties.Appearance.Options.UseFont = true;
            cleAvailable.Properties.Appearance.Options.UseForeColor = true;
            cleAvailable.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleAvailable.Properties.DisplayFormat.FormatString = "n2";
            cleAvailable.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleAvailable.Properties.ReadOnly = true;
            cleAvailable.Size = new Size(103, 22);
            cleAvailable.TabIndex = 19;
            // 
            // lblAvailableUnit
            // 
            lblAvailableUnit.Appearance.ForeColor = Color.Black;
            lblAvailableUnit.Appearance.Options.UseForeColor = true;
            lblAvailableUnit.Location = new Point(540, 152);
            lblAvailableUnit.Name = "lblAvailableUnit";
            lblAvailableUnit.Size = new Size(14, 13);
            lblAvailableUnit.TabIndex = 20;
            lblAvailableUnit.Text = "UN";
            // 
            // grpStockByWarehouse
            // 
            grpStockByWarehouse.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpStockByWarehouse.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpStockByWarehouse.AppearanceCaption.Options.UseFont = true;
            grpStockByWarehouse.AppearanceCaption.Options.UseForeColor = true;
            grpStockByWarehouse.Controls.Add(btnRefreshStock);
            grpStockByWarehouse.Controls.Add(btnExportStock);
            grpStockByWarehouse.Controls.Add(btnSearchStock);
            grpStockByWarehouse.Controls.Add(grcStock);
            grpStockByWarehouse.Location = new Point(9, 217);
            grpStockByWarehouse.Name = "grpStockByWarehouse";
            grpStockByWarehouse.Size = new Size(1046, 225);
            grpStockByWarehouse.TabIndex = 2;
            grpStockByWarehouse.Text = "Stock por Bodega";
            // 
            // btnRefreshStock
            // 
            btnRefreshStock.Location = new Point(17, 30);
            btnRefreshStock.Name = "btnRefreshStock";
            btnRefreshStock.Size = new Size(86, 24);
            btnRefreshStock.TabIndex = 0;
            btnRefreshStock.Text = "Actualizar";
            // 
            // btnExportStock
            // 
            btnExportStock.Location = new Point(111, 30);
            btnExportStock.Name = "btnExportStock";
            btnExportStock.Size = new Size(86, 24);
            btnExportStock.TabIndex = 1;
            btnExportStock.Text = "Exportar";
            // 
            // btnSearchStock
            // 
            btnSearchStock.Location = new Point(771, 30);
            btnSearchStock.Name = "btnSearchStock";
            btnSearchStock.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Search) });
            btnSearchStock.Properties.NullValuePrompt = "Buscar en la tabla...";
            btnSearchStock.Size = new Size(240, 20);
            btnSearchStock.TabIndex = 2;
            // 
            // grcStock
            // 
            grcStock.Location = new Point(13, 65);
            grcStock.MainView = grvStock;
            grcStock.Name = "grcStock";
            grcStock.Size = new Size(1020, 143);
            grcStock.TabIndex = 3;
            grcStock.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvStock });
            // 
            // grvStock
            // 
            grvStock.Columns.AddRange(new GridColumn[] { colWarehouseCode, colWarehouseName, colStock, colCommitted, colOrdered, colAvailable, colBinLocation });
            grvStock.DetailHeight = 303;
            grvStock.GridControl = grcStock;
            grvStock.Name = "grvStock";
            grvStock.OptionsBehavior.Editable = false;
            grvStock.OptionsEditForm.PopupEditFormWidth = 686;
            grvStock.OptionsView.ShowFooter = true;
            grvStock.OptionsView.ShowGroupPanel = false;
            // 
            // colWarehouseCode
            // 
            colWarehouseCode.Caption = "Bodega";
            colWarehouseCode.FieldName = "WarehouseCode";
            colWarehouseCode.MinWidth = 17;
            colWarehouseCode.Name = "colWarehouseCode";
            colWarehouseCode.Visible = true;
            colWarehouseCode.VisibleIndex = 0;
            colWarehouseCode.Width = 77;
            // 
            // colWarehouseName
            // 
            colWarehouseName.Caption = "Nombre";
            colWarehouseName.FieldName = "WarehouseName";
            colWarehouseName.MinWidth = 17;
            colWarehouseName.Name = "colWarehouseName";
            colWarehouseName.Visible = true;
            colWarehouseName.VisibleIndex = 1;
            colWarehouseName.Width = 154;
            // 
            // colStock
            // 
            colStock.Caption = "Stock";
            colStock.DisplayFormat.FormatString = "n2";
            colStock.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colStock.FieldName = "Stock";
            colStock.MinWidth = 17;
            colStock.Name = "colStock";
            colStock.Summary.AddRange(new GridSummaryItem[] { new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Stock", "{0:n2}") });
            colStock.Visible = true;
            colStock.VisibleIndex = 2;
            colStock.Width = 103;
            // 
            // colCommitted
            // 
            colCommitted.Caption = "Comprometido";
            colCommitted.DisplayFormat.FormatString = "n2";
            colCommitted.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colCommitted.FieldName = "Committed";
            colCommitted.MinWidth = 17;
            colCommitted.Name = "colCommitted";
            colCommitted.Summary.AddRange(new GridSummaryItem[] { new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Committed", "{0:n2}") });
            colCommitted.Visible = true;
            colCommitted.VisibleIndex = 3;
            colCommitted.Width = 103;
            // 
            // colOrdered
            // 
            colOrdered.Caption = "Pedido";
            colOrdered.DisplayFormat.FormatString = "n2";
            colOrdered.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colOrdered.FieldName = "Ordered";
            colOrdered.MinWidth = 17;
            colOrdered.Name = "colOrdered";
            colOrdered.Summary.AddRange(new GridSummaryItem[] { new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Ordered", "{0:n2}") });
            colOrdered.Visible = true;
            colOrdered.VisibleIndex = 4;
            colOrdered.Width = 103;
            // 
            // colAvailable
            // 
            colAvailable.Caption = "Disponible";
            colAvailable.DisplayFormat.FormatString = "n2";
            colAvailable.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colAvailable.FieldName = "Available";
            colAvailable.MinWidth = 17;
            colAvailable.Name = "colAvailable";
            colAvailable.Summary.AddRange(new GridSummaryItem[] { new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Available", "{0:n2}") });
            colAvailable.Visible = true;
            colAvailable.VisibleIndex = 5;
            colAvailable.Width = 103;
            // 
            // colBinLocation
            // 
            colBinLocation.Caption = "Ubicacion";
            colBinLocation.FieldName = "BinLocation";
            colBinLocation.MinWidth = 17;
            colBinLocation.Name = "colBinLocation";
            colBinLocation.Visible = true;
            colBinLocation.VisibleIndex = 6;
            colBinLocation.Width = 206;
            // 
            // xtpPurchases
            // 
            xtpPurchases.Controls.Add(grpPurchaseConfig);
            xtpPurchases.Controls.Add(grpPreferredVendor);
            xtpPurchases.Controls.Add(grpAlternativeVendors);
            xtpPurchases.Name = "xtpPurchases";
            xtpPurchases.Size = new Size(1088, 455);
            xtpPurchases.Text = "Compras";
            // 
            // grpPurchaseConfig
            // 
            grpPurchaseConfig.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpPurchaseConfig.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpPurchaseConfig.AppearanceCaption.Options.UseFont = true;
            grpPurchaseConfig.AppearanceCaption.Options.UseForeColor = true;
            grpPurchaseConfig.Controls.Add(lblPreferredVendor);
            grpPurchaseConfig.Controls.Add(slePreferredVendor);
            grpPurchaseConfig.Controls.Add(lblVendorCode);
            grpPurchaseConfig.Controls.Add(txtVendorCode);
            grpPurchaseConfig.Controls.Add(lblPurchaseUnit);
            grpPurchaseConfig.Controls.Add(luePurchaseUnit);
            grpPurchaseConfig.Controls.Add(lblMinPurchaseQty);
            grpPurchaseConfig.Controls.Add(sedMinPurchaseQty);
            grpPurchaseConfig.Controls.Add(lblMinPurchaseQtyUnit);
            grpPurchaseConfig.Controls.Add(lblDeliveryDays);
            grpPurchaseConfig.Controls.Add(sedDeliveryDays);
            grpPurchaseConfig.Controls.Add(lblDeliveryDaysUnit);
            grpPurchaseConfig.Controls.Add(lblLastPurchasePrice);
            grpPurchaseConfig.Controls.Add(cleLastPurchasePrice);
            grpPurchaseConfig.Controls.Add(lblLastPurchaseCurrency);
            grpPurchaseConfig.Controls.Add(lblPurchaseCurrency);
            grpPurchaseConfig.Controls.Add(luePurchaseCurrency);
            grpPurchaseConfig.Controls.Add(lblPurchaseTax);
            grpPurchaseConfig.Controls.Add(luePurchaseTax);
            grpPurchaseConfig.Controls.Add(lblPurchaseAccount);
            grpPurchaseConfig.Controls.Add(slePurchaseAccount);
            grpPurchaseConfig.Controls.Add(lblVendorDiscount);
            grpPurchaseConfig.Controls.Add(sedVendorDiscount);
            grpPurchaseConfig.Controls.Add(lblRepositionDays);
            grpPurchaseConfig.Controls.Add(sedRepositionDays);
            grpPurchaseConfig.Controls.Add(lblRepositionDaysUnit);
            grpPurchaseConfig.Location = new Point(9, 9);
            grpPurchaseConfig.Name = "grpPurchaseConfig";
            grpPurchaseConfig.Size = new Size(497, 243);
            grpPurchaseConfig.TabIndex = 0;
            grpPurchaseConfig.Text = "Configuracion de Compras";
            // 
            // lblPreferredVendor
            // 
            lblPreferredVendor.Appearance.ForeColor = Color.Black;
            lblPreferredVendor.Appearance.Options.UseForeColor = true;
            lblPreferredVendor.Location = new Point(17, 35);
            lblPreferredVendor.Name = "lblPreferredVendor";
            lblPreferredVendor.Size = new Size(101, 13);
            lblPreferredVendor.TabIndex = 0;
            lblPreferredVendor.Text = "Proveedor Preferido:";
            // 
            // slePreferredVendor
            // 
            slePreferredVendor.EditValue = "ALIMENTOS DEL VALLE S.A.S.";
            slePreferredVendor.Location = new Point(180, 31);
            slePreferredVendor.Name = "slePreferredVendor";
            slePreferredVendor.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            slePreferredVendor.Properties.NullText = "";
            slePreferredVendor.Properties.PopupView = grvPreferredVendorLookup;
            slePreferredVendor.Size = new Size(274, 20);
            slePreferredVendor.TabIndex = 1;
            // 
            // grvPreferredVendorLookup
            // 
            grvPreferredVendorLookup.DetailHeight = 303;
            grvPreferredVendorLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvPreferredVendorLookup.Name = "grvPreferredVendorLookup";
            grvPreferredVendorLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvPreferredVendorLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvPreferredVendorLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblVendorCode
            // 
            lblVendorCode.Appearance.ForeColor = Color.Black;
            lblVendorCode.Appearance.Options.UseForeColor = true;
            lblVendorCode.Location = new Point(17, 61);
            lblVendorCode.Name = "lblVendorCode";
            lblVendorCode.Size = new Size(90, 13);
            lblVendorCode.TabIndex = 2;
            lblVendorCode.Text = "Código Proveedor:";
            // 
            // txtVendorCode
            // 
            txtVendorCode.EditValue = "P000001";
            txtVendorCode.Location = new Point(180, 57);
            txtVendorCode.Name = "txtVendorCode";
            txtVendorCode.Size = new Size(274, 20);
            txtVendorCode.TabIndex = 3;
            // 
            // lblPurchaseUnit
            // 
            lblPurchaseUnit.Appearance.ForeColor = Color.Black;
            lblPurchaseUnit.Appearance.Options.UseForeColor = true;
            lblPurchaseUnit.Location = new Point(17, 87);
            lblPurchaseUnit.Name = "lblPurchaseUnit";
            lblPurchaseUnit.Size = new Size(92, 13);
            lblPurchaseUnit.TabIndex = 4;
            lblPurchaseUnit.Text = "Unidad de Compra:";
            // 
            // luePurchaseUnit
            // 
            luePurchaseUnit.EditValue = "BOLSA";
            luePurchaseUnit.Location = new Point(180, 83);
            luePurchaseUnit.Name = "luePurchaseUnit";
            luePurchaseUnit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            luePurchaseUnit.Properties.DataSource = new string[]
    {
    "BOLSA",
    "UNIDAD",
    "CAJA"
    };
            luePurchaseUnit.Properties.NullText = "";
            luePurchaseUnit.Size = new Size(274, 20);
            luePurchaseUnit.TabIndex = 5;
            // 
            // lblMinPurchaseQty
            // 
            lblMinPurchaseQty.Appearance.ForeColor = Color.Black;
            lblMinPurchaseQty.Appearance.Options.UseForeColor = true;
            lblMinPurchaseQty.Location = new Point(17, 113);
            lblMinPurchaseQty.Name = "lblMinPurchaseQty";
            lblMinPurchaseQty.Size = new Size(82, 13);
            lblMinPurchaseQty.TabIndex = 6;
            lblMinPurchaseQty.Text = "Cantidad Mínima:";
            // 
            // sedMinPurchaseQty
            // 
            sedMinPurchaseQty.EditValue = new decimal(new int[] { 1000, 0, 0, 131072 });
            sedMinPurchaseQty.Location = new Point(180, 109);
            sedMinPurchaseQty.Name = "sedMinPurchaseQty";
            sedMinPurchaseQty.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedMinPurchaseQty.Properties.MaskSettings.Set("mask", "n2");
            sedMinPurchaseQty.Size = new Size(103, 20);
            sedMinPurchaseQty.TabIndex = 7;
            // 
            // lblMinPurchaseQtyUnit
            // 
            lblMinPurchaseQtyUnit.Appearance.ForeColor = Color.Black;
            lblMinPurchaseQtyUnit.Appearance.Options.UseForeColor = true;
            lblMinPurchaseQtyUnit.Location = new Point(291, 113);
            lblMinPurchaseQtyUnit.Name = "lblMinPurchaseQtyUnit";
            lblMinPurchaseQtyUnit.Size = new Size(38, 13);
            lblMinPurchaseQtyUnit.TabIndex = 8;
            lblMinPurchaseQtyUnit.Text = "BOLSAS";
            // 
            // lblDeliveryDays
            // 
            lblDeliveryDays.Appearance.ForeColor = Color.Black;
            lblDeliveryDays.Appearance.Options.UseForeColor = true;
            lblDeliveryDays.Location = new Point(17, 139);
            lblDeliveryDays.Name = "lblDeliveryDays";
            lblDeliveryDays.Size = new Size(94, 13);
            lblDeliveryDays.TabIndex = 9;
            lblDeliveryDays.Text = "Tiempo de Entrega:";
            // 
            // sedDeliveryDays
            // 
            sedDeliveryDays.EditValue = new decimal(new int[] { 2, 0, 0, 0 });
            sedDeliveryDays.Location = new Point(180, 135);
            sedDeliveryDays.Name = "sedDeliveryDays";
            sedDeliveryDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedDeliveryDays.Properties.IsFloatValue = false;
            sedDeliveryDays.Properties.MaskSettings.Set("mask", "N00");
            sedDeliveryDays.Size = new Size(103, 20);
            sedDeliveryDays.TabIndex = 10;
            // 
            // lblDeliveryDaysUnit
            // 
            lblDeliveryDaysUnit.Appearance.ForeColor = Color.Black;
            lblDeliveryDaysUnit.Appearance.Options.UseForeColor = true;
            lblDeliveryDaysUnit.Location = new Point(291, 139);
            lblDeliveryDaysUnit.Name = "lblDeliveryDaysUnit";
            lblDeliveryDaysUnit.Size = new Size(19, 13);
            lblDeliveryDaysUnit.TabIndex = 11;
            lblDeliveryDaysUnit.Text = "días";
            // 
            // lblLastPurchasePrice
            // 
            lblLastPurchasePrice.Appearance.ForeColor = Color.Black;
            lblLastPurchasePrice.Appearance.Options.UseForeColor = true;
            lblLastPurchasePrice.Location = new Point(17, 165);
            lblLastPurchasePrice.Name = "lblLastPurchasePrice";
            lblLastPurchasePrice.Size = new Size(120, 13);
            lblLastPurchasePrice.TabIndex = 12;
            lblLastPurchasePrice.Text = "Último Precio de Compra:";
            // 
            // cleLastPurchasePrice
            // 
            cleLastPurchasePrice.EditValue = new decimal(new int[] { 118, 0, 0, 131072 });
            cleLastPurchasePrice.Location = new Point(180, 161);
            cleLastPurchasePrice.Name = "cleLastPurchasePrice";
            cleLastPurchasePrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleLastPurchasePrice.Properties.DisplayFormat.FormatString = "n2";
            cleLastPurchasePrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleLastPurchasePrice.Size = new Size(103, 20);
            cleLastPurchasePrice.TabIndex = 13;
            // 
            // lblLastPurchaseCurrency
            // 
            lblLastPurchaseCurrency.Appearance.ForeColor = Color.Black;
            lblLastPurchaseCurrency.Appearance.Options.UseForeColor = true;
            lblLastPurchaseCurrency.Location = new Point(291, 165);
            lblLastPurchaseCurrency.Name = "lblLastPurchaseCurrency";
            lblLastPurchaseCurrency.Size = new Size(20, 13);
            lblLastPurchaseCurrency.TabIndex = 14;
            lblLastPurchaseCurrency.Text = "USD";
            // 
            // lblPurchaseCurrency
            // 
            lblPurchaseCurrency.Appearance.ForeColor = Color.Black;
            lblPurchaseCurrency.Appearance.Options.UseForeColor = true;
            lblPurchaseCurrency.Location = new Point(17, 191);
            lblPurchaseCurrency.Name = "lblPurchaseCurrency";
            lblPurchaseCurrency.Size = new Size(42, 13);
            lblPurchaseCurrency.TabIndex = 15;
            lblPurchaseCurrency.Text = "Moneda:";
            // 
            // luePurchaseCurrency
            // 
            luePurchaseCurrency.EditValue = "USD - Dolar Americano";
            luePurchaseCurrency.Location = new Point(180, 187);
            luePurchaseCurrency.Name = "luePurchaseCurrency";
            luePurchaseCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            luePurchaseCurrency.Properties.DataSource = new string[]
    {
    "USD - Dolar Americano",
    "EUR - Euro"
    };
            luePurchaseCurrency.Properties.NullText = "";
            luePurchaseCurrency.Size = new Size(274, 20);
            luePurchaseCurrency.TabIndex = 16;
            // 
            // lblPurchaseTax
            // 
            lblPurchaseTax.Appearance.ForeColor = Color.Black;
            lblPurchaseTax.Appearance.Options.UseForeColor = true;
            lblPurchaseTax.Location = new Point(17, 217);
            lblPurchaseTax.Name = "lblPurchaseTax";
            lblPurchaseTax.Size = new Size(89, 13);
            lblPurchaseTax.TabIndex = 17;
            lblPurchaseTax.Text = "Impuesto Compra:";
            // 
            // luePurchaseTax
            // 
            luePurchaseTax.EditValue = "IVA 12% COMPRAS";
            luePurchaseTax.Location = new Point(180, 213);
            luePurchaseTax.Name = "luePurchaseTax";
            luePurchaseTax.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            luePurchaseTax.Properties.DataSource = new string[]
    {
    "IVA 12% COMPRAS",
    "IVA 0% COMPRAS"
    };
            luePurchaseTax.Properties.NullText = "";
            luePurchaseTax.Size = new Size(274, 20);
            luePurchaseTax.TabIndex = 18;
            // 
            // lblPurchaseAccount
            // 
            lblPurchaseAccount.Appearance.ForeColor = Color.Black;
            lblPurchaseAccount.Appearance.Options.UseForeColor = true;
            lblPurchaseAccount.Location = new Point(17, 269);
            lblPurchaseAccount.Name = "lblPurchaseAccount";
            lblPurchaseAccount.Size = new Size(130, 13);
            lblPurchaseAccount.TabIndex = 19;
            lblPurchaseAccount.Text = "Cuenta Contable Compras:";
            lblPurchaseAccount.Visible = false;
            // 
            // slePurchaseAccount
            // 
            slePurchaseAccount.EditValue = "5-01-02-01 - Compras de Mercaderias";
            slePurchaseAccount.Location = new Point(180, 265);
            slePurchaseAccount.Name = "slePurchaseAccount";
            slePurchaseAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            slePurchaseAccount.Properties.NullText = "";
            slePurchaseAccount.Properties.PopupView = grvPurchaseAccountLookup;
            slePurchaseAccount.Size = new Size(274, 20);
            slePurchaseAccount.TabIndex = 20;
            slePurchaseAccount.Visible = false;
            // 
            // grvPurchaseAccountLookup
            // 
            grvPurchaseAccountLookup.DetailHeight = 303;
            grvPurchaseAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvPurchaseAccountLookup.Name = "grvPurchaseAccountLookup";
            grvPurchaseAccountLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvPurchaseAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvPurchaseAccountLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblVendorDiscount
            // 
            lblVendorDiscount.Appearance.ForeColor = Color.Black;
            lblVendorDiscount.Appearance.Options.UseForeColor = true;
            lblVendorDiscount.Location = new Point(339, 113);
            lblVendorDiscount.Name = "lblVendorDiscount";
            lblVendorDiscount.Size = new Size(56, 13);
            lblVendorDiscount.TabIndex = 21;
            lblVendorDiscount.Text = "Desc. Prov.";
            // 
            // sedVendorDiscount
            // 
            sedVendorDiscount.EditValue = new decimal(new int[] { 200, 0, 0, 131072 });
            sedVendorDiscount.Location = new Point(399, 109);
            sedVendorDiscount.Name = "sedVendorDiscount";
            sedVendorDiscount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedVendorDiscount.Properties.MaskSettings.Set("mask", "n2");
            sedVendorDiscount.Size = new Size(56, 20);
            sedVendorDiscount.TabIndex = 22;
            // 
            // lblRepositionDays
            // 
            lblRepositionDays.Appearance.ForeColor = Color.Black;
            lblRepositionDays.Appearance.Options.UseForeColor = true;
            lblRepositionDays.Location = new Point(339, 139);
            lblRepositionDays.Name = "lblRepositionDays";
            lblRepositionDays.Size = new Size(55, 13);
            lblRepositionDays.TabIndex = 23;
            lblRepositionDays.Text = "Reposición:";
            // 
            // sedRepositionDays
            // 
            sedRepositionDays.EditValue = new decimal(new int[] { 7, 0, 0, 0 });
            sedRepositionDays.Location = new Point(399, 135);
            sedRepositionDays.Name = "sedRepositionDays";
            sedRepositionDays.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedRepositionDays.Properties.IsFloatValue = false;
            sedRepositionDays.Properties.MaskSettings.Set("mask", "N00");
            sedRepositionDays.Size = new Size(56, 20);
            sedRepositionDays.TabIndex = 24;
            // 
            // lblRepositionDaysUnit
            // 
            lblRepositionDaysUnit.Appearance.ForeColor = Color.Black;
            lblRepositionDaysUnit.Appearance.Options.UseForeColor = true;
            lblRepositionDaysUnit.Location = new Point(459, 139);
            lblRepositionDaysUnit.Name = "lblRepositionDaysUnit";
            lblRepositionDaysUnit.Size = new Size(19, 13);
            lblRepositionDaysUnit.TabIndex = 25;
            lblRepositionDaysUnit.Text = "días";
            // 
            // grpPreferredVendor
            // 
            grpPreferredVendor.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpPreferredVendor.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpPreferredVendor.AppearanceCaption.Options.UseFont = true;
            grpPreferredVendor.AppearanceCaption.Options.UseForeColor = true;
            grpPreferredVendor.Controls.Add(lblPreferredVendorTitle);
            grpPreferredVendor.Controls.Add(lblVendorNitTitle);
            grpPreferredVendor.Controls.Add(lblVendorNit);
            grpPreferredVendor.Controls.Add(lblVendorContactTitle);
            grpPreferredVendor.Controls.Add(lblVendorContact);
            grpPreferredVendor.Controls.Add(lblVendorPhoneTitle);
            grpPreferredVendor.Controls.Add(lblVendorPhone);
            grpPreferredVendor.Controls.Add(lblVendorEmailTitle);
            grpPreferredVendor.Controls.Add(lblVendorEmail);
            grpPreferredVendor.Controls.Add(lblVendorAddressTitle);
            grpPreferredVendor.Controls.Add(lblVendorAddress);
            grpPreferredVendor.Controls.Add(lblVendorCityTitle);
            grpPreferredVendor.Controls.Add(lblVendorCity);
            grpPreferredVendor.Controls.Add(btnOpenVendorFile);
            grpPreferredVendor.Location = new Point(514, 9);
            grpPreferredVendor.Name = "grpPreferredVendor";
            grpPreferredVendor.Size = new Size(540, 243);
            grpPreferredVendor.TabIndex = 1;
            grpPreferredVendor.Text = "Proveedor Preferido";
            // 
            // lblPreferredVendorTitle
            // 
            lblPreferredVendorTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPreferredVendorTitle.Appearance.ForeColor = Color.Black;
            lblPreferredVendorTitle.Appearance.Options.UseFont = true;
            lblPreferredVendorTitle.Appearance.Options.UseForeColor = true;
            lblPreferredVendorTitle.Location = new Point(21, 39);
            lblPreferredVendorTitle.Name = "lblPreferredVendorTitle";
            lblPreferredVendorTitle.Size = new Size(162, 15);
            lblPreferredVendorTitle.TabIndex = 0;
            lblPreferredVendorTitle.Text = "ALIMENTOS DEL VALLE S.A.S.";
            // 
            // lblVendorNitTitle
            // 
            lblVendorNitTitle.Appearance.ForeColor = Color.Black;
            lblVendorNitTitle.Appearance.Options.UseForeColor = true;
            lblVendorNitTitle.Location = new Point(21, 74);
            lblVendorNitTitle.Name = "lblVendorNitTitle";
            lblVendorNitTitle.Size = new Size(21, 13);
            lblVendorNitTitle.TabIndex = 1;
            lblVendorNitTitle.Text = "NIT:";
            // 
            // lblVendorNit
            // 
            lblVendorNit.Appearance.ForeColor = Color.Black;
            lblVendorNit.Appearance.Options.UseForeColor = true;
            lblVendorNit.Location = new Point(129, 74);
            lblVendorNit.Name = "lblVendorNit";
            lblVendorNit.Size = new Size(72, 13);
            lblVendorNit.TabIndex = 2;
            lblVendorNit.Text = "890.321.456-1";
            // 
            // lblVendorContactTitle
            // 
            lblVendorContactTitle.Appearance.ForeColor = Color.Black;
            lblVendorContactTitle.Appearance.Options.UseForeColor = true;
            lblVendorContactTitle.Location = new Point(21, 95);
            lblVendorContactTitle.Name = "lblVendorContactTitle";
            lblVendorContactTitle.Size = new Size(48, 13);
            lblVendorContactTitle.TabIndex = 3;
            lblVendorContactTitle.Text = "Contacto:";
            // 
            // lblVendorContact
            // 
            lblVendorContact.Appearance.ForeColor = Color.Black;
            lblVendorContact.Appearance.Options.UseForeColor = true;
            lblVendorContact.Location = new Point(129, 95);
            lblVendorContact.Name = "lblVendorContact";
            lblVendorContact.Size = new Size(97, 13);
            lblVendorContact.TabIndex = 4;
            lblVendorContact.Text = "Juan Carlos Ramírez";
            // 
            // lblVendorPhoneTitle
            // 
            lblVendorPhoneTitle.Appearance.ForeColor = Color.Black;
            lblVendorPhoneTitle.Appearance.Options.UseForeColor = true;
            lblVendorPhoneTitle.Location = new Point(21, 117);
            lblVendorPhoneTitle.Name = "lblVendorPhoneTitle";
            lblVendorPhoneTitle.Size = new Size(46, 13);
            lblVendorPhoneTitle.TabIndex = 5;
            lblVendorPhoneTitle.Text = "Teléfono:";
            // 
            // lblVendorPhone
            // 
            lblVendorPhone.Appearance.ForeColor = Color.Black;
            lblVendorPhone.Appearance.Options.UseForeColor = true;
            lblVendorPhone.Location = new Point(129, 117);
            lblVendorPhone.Name = "lblVendorPhone";
            lblVendorPhone.Size = new Size(74, 13);
            lblVendorPhone.TabIndex = 6;
            lblVendorPhone.Text = "(602) 555 1234";
            // 
            // lblVendorEmailTitle
            // 
            lblVendorEmailTitle.Appearance.ForeColor = Color.Black;
            lblVendorEmailTitle.Appearance.Options.UseForeColor = true;
            lblVendorEmailTitle.Location = new Point(21, 139);
            lblVendorEmailTitle.Name = "lblVendorEmailTitle";
            lblVendorEmailTitle.Size = new Size(28, 13);
            lblVendorEmailTitle.TabIndex = 7;
            lblVendorEmailTitle.Text = "Email:";
            // 
            // lblVendorEmail
            // 
            lblVendorEmail.Appearance.ForeColor = Color.Black;
            lblVendorEmail.Appearance.Options.UseForeColor = true;
            lblVendorEmail.Location = new Point(129, 139);
            lblVendorEmail.Name = "lblVendorEmail";
            lblVendorEmail.Size = new Size(154, 13);
            lblVendorEmail.TabIndex = 8;
            lblVendorEmail.Text = "compras@alimentosdelvalle.com";
            // 
            // lblVendorAddressTitle
            // 
            lblVendorAddressTitle.Appearance.ForeColor = Color.Black;
            lblVendorAddressTitle.Appearance.Options.UseForeColor = true;
            lblVendorAddressTitle.Location = new Point(21, 160);
            lblVendorAddressTitle.Name = "lblVendorAddressTitle";
            lblVendorAddressTitle.Size = new Size(47, 13);
            lblVendorAddressTitle.TabIndex = 9;
            lblVendorAddressTitle.Text = "Dirección:";
            // 
            // lblVendorAddress
            // 
            lblVendorAddress.Appearance.ForeColor = Color.Black;
            lblVendorAddress.Appearance.Options.UseForeColor = true;
            lblVendorAddress.Location = new Point(129, 160);
            lblVendorAddress.Name = "lblVendorAddress";
            lblVendorAddress.Size = new Size(80, 13);
            lblVendorAddress.TabIndex = 10;
            lblVendorAddress.Text = "Calle 10 # 25-45";
            // 
            // lblVendorCityTitle
            // 
            lblVendorCityTitle.Appearance.ForeColor = Color.Black;
            lblVendorCityTitle.Appearance.Options.UseForeColor = true;
            lblVendorCityTitle.Location = new Point(21, 182);
            lblVendorCityTitle.Name = "lblVendorCityTitle";
            lblVendorCityTitle.Size = new Size(37, 13);
            lblVendorCityTitle.TabIndex = 11;
            lblVendorCityTitle.Text = "Ciudad:";
            // 
            // lblVendorCity
            // 
            lblVendorCity.Appearance.ForeColor = Color.Black;
            lblVendorCity.Appearance.Options.UseForeColor = true;
            lblVendorCity.Location = new Point(129, 182);
            lblVendorCity.Name = "lblVendorCity";
            lblVendorCity.Size = new Size(96, 13);
            lblVendorCity.TabIndex = 12;
            lblVendorCity.Text = "Cali, Valle del Cauca";
            // 
            // btnOpenVendorFile
            // 
            btnOpenVendorFile.Location = new Point(21, 208);
            btnOpenVendorFile.Name = "btnOpenVendorFile";
            btnOpenVendorFile.Size = new Size(154, 26);
            btnOpenVendorFile.TabIndex = 13;
            btnOpenVendorFile.Text = "Ver Ficha del Proveedor";
            // 
            // grpAlternativeVendors
            // 
            grpAlternativeVendors.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpAlternativeVendors.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpAlternativeVendors.AppearanceCaption.Options.UseFont = true;
            grpAlternativeVendors.AppearanceCaption.Options.UseForeColor = true;
            grpAlternativeVendors.Controls.Add(btnAddVendor);
            grpAlternativeVendors.Controls.Add(btnEditVendor);
            grpAlternativeVendors.Controls.Add(btnDeleteVendor);
            grpAlternativeVendors.Controls.Add(btnMoveVendorUp);
            grpAlternativeVendors.Controls.Add(btnMoveVendorDown);
            grpAlternativeVendors.Controls.Add(btnRefreshVendorPrices);
            grpAlternativeVendors.Controls.Add(grcVendors);
            grpAlternativeVendors.Location = new Point(9, 260);
            grpAlternativeVendors.Name = "grpAlternativeVendors";
            grpAlternativeVendors.Size = new Size(1046, 191);
            grpAlternativeVendors.TabIndex = 2;
            grpAlternativeVendors.Text = "Proveedores Alternativos";
            // 
            // btnAddVendor
            // 
            btnAddVendor.Location = new Point(17, 30);
            btnAddVendor.Name = "btnAddVendor";
            btnAddVendor.Size = new Size(69, 24);
            btnAddVendor.TabIndex = 0;
            btnAddVendor.Text = "Agregar";
            // 
            // btnEditVendor
            // 
            btnEditVendor.Location = new Point(90, 30);
            btnEditVendor.Name = "btnEditVendor";
            btnEditVendor.Size = new Size(69, 24);
            btnEditVendor.TabIndex = 1;
            btnEditVendor.Text = "Editar";
            // 
            // btnDeleteVendor
            // 
            btnDeleteVendor.Location = new Point(163, 30);
            btnDeleteVendor.Name = "btnDeleteVendor";
            btnDeleteVendor.Size = new Size(69, 24);
            btnDeleteVendor.TabIndex = 2;
            btnDeleteVendor.Text = "Eliminar";
            // 
            // btnMoveVendorUp
            // 
            btnMoveVendorUp.Location = new Point(240, 30);
            btnMoveVendorUp.Name = "btnMoveVendorUp";
            btnMoveVendorUp.Size = new Size(69, 24);
            btnMoveVendorUp.TabIndex = 3;
            btnMoveVendorUp.Text = "Subir";
            // 
            // btnMoveVendorDown
            // 
            btnMoveVendorDown.Location = new Point(313, 30);
            btnMoveVendorDown.Name = "btnMoveVendorDown";
            btnMoveVendorDown.Size = new Size(69, 24);
            btnMoveVendorDown.TabIndex = 4;
            btnMoveVendorDown.Text = "Bajar";
            // 
            // btnRefreshVendorPrices
            // 
            btnRefreshVendorPrices.Location = new Point(390, 30);
            btnRefreshVendorPrices.Name = "btnRefreshVendorPrices";
            btnRefreshVendorPrices.Size = new Size(129, 24);
            btnRefreshVendorPrices.TabIndex = 5;
            btnRefreshVendorPrices.Text = "Actualizar Precios";
            // 
            // grcVendors
            // 
            grcVendors.Location = new Point(13, 61);
            grcVendors.MainView = grvVendors;
            grcVendors.Name = "grcVendors";
            grcVendors.Size = new Size(1020, 117);
            grcVendors.TabIndex = 6;
            grcVendors.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvVendors });
            // 
            // grvVendors
            // 
            grvVendors.Columns.AddRange(new GridColumn[] { colVendorPriority, colVendorName, colVendorCode, colVendorPrice, colVendorCurrency, colVendorDeliveryDays, colVendorActive });
            grvVendors.DetailHeight = 303;
            grvVendors.GridControl = grcVendors;
            grvVendors.Name = "grvVendors";
            grvVendors.OptionsBehavior.Editable = false;
            grvVendors.OptionsEditForm.PopupEditFormWidth = 686;
            grvVendors.OptionsView.ShowGroupPanel = false;
            grvVendors.OptionsView.ShowIndicator = false;
            // 
            // colVendorPriority
            // 
            colVendorPriority.Caption = "Prioridad";
            colVendorPriority.FieldName = "Priority";
            colVendorPriority.MinWidth = 17;
            colVendorPriority.Name = "colVendorPriority";
            colVendorPriority.Visible = true;
            colVendorPriority.VisibleIndex = 0;
            colVendorPriority.Width = 69;
            // 
            // colVendorName
            // 
            colVendorName.Caption = "Proveedor";
            colVendorName.FieldName = "VendorName";
            colVendorName.MinWidth = 17;
            colVendorName.Name = "colVendorName";
            colVendorName.Visible = true;
            colVendorName.VisibleIndex = 1;
            colVendorName.Width = 257;
            // 
            // colVendorCode
            // 
            colVendorCode.Caption = "Codigo";
            colVendorCode.FieldName = "VendorCode";
            colVendorCode.MinWidth = 17;
            colVendorCode.Name = "colVendorCode";
            colVendorCode.Visible = true;
            colVendorCode.VisibleIndex = 2;
            colVendorCode.Width = 103;
            // 
            // colVendorPrice
            // 
            colVendorPrice.Caption = "Precio (USD)";
            colVendorPrice.DisplayFormat.FormatString = "n2";
            colVendorPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colVendorPrice.FieldName = "Price";
            colVendorPrice.MinWidth = 17;
            colVendorPrice.Name = "colVendorPrice";
            colVendorPrice.Visible = true;
            colVendorPrice.VisibleIndex = 3;
            colVendorPrice.Width = 103;
            // 
            // colVendorCurrency
            // 
            colVendorCurrency.Caption = "Moneda";
            colVendorCurrency.FieldName = "Currency";
            colVendorCurrency.MinWidth = 17;
            colVendorCurrency.Name = "colVendorCurrency";
            colVendorCurrency.Visible = true;
            colVendorCurrency.VisibleIndex = 4;
            colVendorCurrency.Width = 86;
            // 
            // colVendorDeliveryDays
            // 
            colVendorDeliveryDays.Caption = "Tiempo Entrega (dias)";
            colVendorDeliveryDays.FieldName = "DeliveryDays";
            colVendorDeliveryDays.MinWidth = 17;
            colVendorDeliveryDays.Name = "colVendorDeliveryDays";
            colVendorDeliveryDays.Visible = true;
            colVendorDeliveryDays.VisibleIndex = 5;
            colVendorDeliveryDays.Width = 137;
            // 
            // colVendorActive
            // 
            colVendorActive.Caption = "Activo";
            colVendorActive.FieldName = "Active";
            colVendorActive.MinWidth = 17;
            colVendorActive.Name = "colVendorActive";
            colVendorActive.Visible = true;
            colVendorActive.VisibleIndex = 6;
            colVendorActive.Width = 77;
            // 
            // xtpSales
            // 
            xtpSales.Controls.Add(grpSalesConfig);
            xtpSales.Controls.Add(grpSalesMargins);
            xtpSales.Controls.Add(grpPriceLists);
            xtpSales.Name = "xtpSales";
            xtpSales.Size = new Size(1088, 455);
            xtpSales.Text = "Ventas";
            // 
            // grpSalesConfig
            // 
            grpSalesConfig.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpSalesConfig.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpSalesConfig.AppearanceCaption.Options.UseFont = true;
            grpSalesConfig.AppearanceCaption.Options.UseForeColor = true;
            grpSalesConfig.Controls.Add(lblSalesUnit);
            grpSalesConfig.Controls.Add(lueSalesUnit);
            grpSalesConfig.Controls.Add(lblSalesTax);
            grpSalesConfig.Controls.Add(lueSalesTax);
            grpSalesConfig.Controls.Add(lblSalesAccount);
            grpSalesConfig.Controls.Add(sleSalesAccount);
            grpSalesConfig.Controls.Add(lblSalesDefaultPriceList);
            grpSalesConfig.Controls.Add(lueSalesDefaultPriceList);
            grpSalesConfig.Controls.Add(lblSalesCurrency);
            grpSalesConfig.Controls.Add(lueSalesCurrency);
            grpSalesConfig.Controls.Add(lblSalesNotes);
            grpSalesConfig.Controls.Add(memSalesNotes);
            grpSalesConfig.Location = new Point(9, 9);
            grpSalesConfig.Name = "grpSalesConfig";
            grpSalesConfig.Size = new Size(309, 373);
            grpSalesConfig.TabIndex = 0;
            grpSalesConfig.Text = "Configuracion de Ventas";
            // 
            // lblSalesUnit
            // 
            lblSalesUnit.Appearance.ForeColor = Color.Black;
            lblSalesUnit.Appearance.Options.UseForeColor = true;
            lblSalesUnit.Location = new Point(17, 48);
            lblSalesUnit.Name = "lblSalesUnit";
            lblSalesUnit.Size = new Size(83, 13);
            lblSalesUnit.TabIndex = 0;
            lblSalesUnit.Text = "Unidad de Venta:";
            // 
            // lueSalesUnit
            // 
            lueSalesUnit.EditValue = "UNIDAD";
            lueSalesUnit.Location = new Point(17, 68);
            lueSalesUnit.Name = "lueSalesUnit";
            lueSalesUnit.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueSalesUnit.Properties.DataSource = new string[]
    {
    "UNIDAD",
    "BOLSA",
    "CAJA"
    };
            lueSalesUnit.Properties.NullText = "";
            lueSalesUnit.Size = new Size(257, 20);
            lueSalesUnit.TabIndex = 1;
            // 
            // lblSalesTax
            // 
            lblSalesTax.Appearance.ForeColor = Color.Black;
            lblSalesTax.Appearance.Options.UseForeColor = true;
            lblSalesTax.Location = new Point(17, 97);
            lblSalesTax.Name = "lblSalesTax";
            lblSalesTax.Size = new Size(80, 13);
            lblSalesTax.TabIndex = 2;
            lblSalesTax.Text = "Impuesto Venta:";
            // 
            // lueSalesTax
            // 
            lueSalesTax.EditValue = "IVA 12%";
            lueSalesTax.Location = new Point(17, 117);
            lueSalesTax.Name = "lueSalesTax";
            lueSalesTax.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueSalesTax.Properties.DataSource = new string[]
    {
    "IVA 12%",
    "IVA 0%"
    };
            lueSalesTax.Properties.NullText = "";
            lueSalesTax.Size = new Size(257, 20);
            lueSalesTax.TabIndex = 3;
            // 
            // lblSalesAccount
            // 
            lblSalesAccount.Appearance.ForeColor = Color.Black;
            lblSalesAccount.Appearance.Options.UseForeColor = true;
            lblSalesAccount.Location = new Point(17, 146);
            lblSalesAccount.Name = "lblSalesAccount";
            lblSalesAccount.Size = new Size(90, 13);
            lblSalesAccount.TabIndex = 4;
            lblSalesAccount.Text = "Cuenta de Ventas:";
            // 
            // sleSalesAccount
            // 
            sleSalesAccount.EditValue = "41010001 - VENTAS DE PRODUCTOS";
            sleSalesAccount.Location = new Point(17, 166);
            sleSalesAccount.Name = "sleSalesAccount";
            sleSalesAccount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleSalesAccount.Properties.NullText = "";
            sleSalesAccount.Properties.PopupView = grvSalesAccountLookup;
            sleSalesAccount.Size = new Size(257, 20);
            sleSalesAccount.TabIndex = 5;
            // 
            // grvSalesAccountLookup
            // 
            grvSalesAccountLookup.DetailHeight = 303;
            grvSalesAccountLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvSalesAccountLookup.Name = "grvSalesAccountLookup";
            grvSalesAccountLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvSalesAccountLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvSalesAccountLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblSalesDefaultPriceList
            // 
            lblSalesDefaultPriceList.Appearance.ForeColor = Color.Black;
            lblSalesDefaultPriceList.Appearance.Options.UseForeColor = true;
            lblSalesDefaultPriceList.Location = new Point(17, 196);
            lblSalesDefaultPriceList.Name = "lblSalesDefaultPriceList";
            lblSalesDefaultPriceList.Size = new Size(116, 13);
            lblSalesDefaultPriceList.TabIndex = 6;
            lblSalesDefaultPriceList.Text = "Lista de Precios Default:";
            // 
            // lueSalesDefaultPriceList
            // 
            lueSalesDefaultPriceList.EditValue = "LISTA GENERAL";
            lueSalesDefaultPriceList.Location = new Point(17, 216);
            lueSalesDefaultPriceList.Name = "lueSalesDefaultPriceList";
            lueSalesDefaultPriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueSalesDefaultPriceList.Properties.DataSource = new string[]
    {
    "LISTA GENERAL",
    "LISTA MAYORISTA",
    "LISTA DISTRIBUIDOR"
    };
            lueSalesDefaultPriceList.Properties.NullText = "";
            lueSalesDefaultPriceList.Size = new Size(257, 20);
            lueSalesDefaultPriceList.TabIndex = 7;
            // 
            // lblSalesCurrency
            // 
            lblSalesCurrency.Appearance.ForeColor = Color.Black;
            lblSalesCurrency.Appearance.Options.UseForeColor = true;
            lblSalesCurrency.Location = new Point(17, 245);
            lblSalesCurrency.Name = "lblSalesCurrency";
            lblSalesCurrency.Size = new Size(68, 13);
            lblSalesCurrency.TabIndex = 8;
            lblSalesCurrency.Text = "Moneda Base:";
            // 
            // lueSalesCurrency
            // 
            lueSalesCurrency.EditValue = "USD - Dolar Americano";
            lueSalesCurrency.Location = new Point(17, 265);
            lueSalesCurrency.Name = "lueSalesCurrency";
            lueSalesCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueSalesCurrency.Properties.DataSource = new string[]
    {
    "USD - Dolar Americano",
    "EUR - Euro"
    };
            lueSalesCurrency.Properties.NullText = "";
            lueSalesCurrency.Size = new Size(257, 20);
            lueSalesCurrency.TabIndex = 9;
            // 
            // lblSalesNotes
            // 
            lblSalesNotes.Appearance.ForeColor = Color.Black;
            lblSalesNotes.Appearance.Options.UseForeColor = true;
            lblSalesNotes.Location = new Point(17, 295);
            lblSalesNotes.Name = "lblSalesNotes";
            lblSalesNotes.Size = new Size(111, 13);
            lblSalesNotes.TabIndex = 10;
            lblSalesNotes.Text = "Observaciones Ventas:";
            // 
            // memSalesNotes
            // 
            memSalesNotes.EditValue = "Producto de consumo masivo.\r\nAlta rotacion.";
            memSalesNotes.Location = new Point(17, 315);
            memSalesNotes.Name = "memSalesNotes";
            memSalesNotes.Size = new Size(257, 43);
            memSalesNotes.TabIndex = 11;
            // 
            // grpSalesMargins
            // 
            grpSalesMargins.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpSalesMargins.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpSalesMargins.AppearanceCaption.Options.UseFont = true;
            grpSalesMargins.AppearanceCaption.Options.UseForeColor = true;
            grpSalesMargins.Controls.Add(lblBasePrice);
            grpSalesMargins.Controls.Add(cleBasePrice);
            grpSalesMargins.Controls.Add(lblSalesMaxDiscount);
            grpSalesMargins.Controls.Add(sedSalesMaxDiscount);
            grpSalesMargins.Controls.Add(lblMinMargin);
            grpSalesMargins.Controls.Add(sedMinMargin);
            grpSalesMargins.Controls.Add(lblCurrentMargin);
            grpSalesMargins.Controls.Add(cleCurrentMargin);
            grpSalesMargins.Controls.Add(chkValidatePriceBelowCost);
            grpSalesMargins.Controls.Add(chkRequireDiscountAuthorization);
            grpSalesMargins.Location = new Point(326, 9);
            grpSalesMargins.Name = "grpSalesMargins";
            grpSalesMargins.Size = new Size(197, 373);
            grpSalesMargins.TabIndex = 1;
            grpSalesMargins.Text = "Precios y Margenes";
            // 
            // lblBasePrice
            // 
            lblBasePrice.Appearance.ForeColor = Color.Black;
            lblBasePrice.Appearance.Options.UseForeColor = true;
            lblBasePrice.Location = new Point(17, 61);
            lblBasePrice.Name = "lblBasePrice";
            lblBasePrice.Size = new Size(90, 13);
            lblBasePrice.TabIndex = 0;
            lblBasePrice.Text = "Precio Base (USD):";
            // 
            // cleBasePrice
            // 
            cleBasePrice.EditValue = new decimal(new int[] { 160, 0, 0, 131072 });
            cleBasePrice.Location = new Point(111, 57);
            cleBasePrice.Name = "cleBasePrice";
            cleBasePrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleBasePrice.Properties.DisplayFormat.FormatString = "c2";
            cleBasePrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleBasePrice.Size = new Size(69, 20);
            cleBasePrice.TabIndex = 1;
            // 
            // lblSalesMaxDiscount
            // 
            lblSalesMaxDiscount.Appearance.ForeColor = Color.Black;
            lblSalesMaxDiscount.Appearance.Options.UseForeColor = true;
            lblSalesMaxDiscount.Location = new Point(17, 104);
            lblSalesMaxDiscount.Name = "lblSalesMaxDiscount";
            lblSalesMaxDiscount.Size = new Size(116, 13);
            lblSalesMaxDiscount.TabIndex = 2;
            lblSalesMaxDiscount.Text = "Descuento Máximo (%):";
            // 
            // sedSalesMaxDiscount
            // 
            sedSalesMaxDiscount.EditValue = new decimal(new int[] { 1500, 0, 0, 131072 });
            sedSalesMaxDiscount.Location = new Point(111, 101);
            sedSalesMaxDiscount.Name = "sedSalesMaxDiscount";
            sedSalesMaxDiscount.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedSalesMaxDiscount.Properties.MaskSettings.Set("mask", "n2");
            sedSalesMaxDiscount.Size = new Size(69, 20);
            sedSalesMaxDiscount.TabIndex = 3;
            // 
            // lblMinMargin
            // 
            lblMinMargin.Appearance.ForeColor = Color.Black;
            lblMinMargin.Appearance.Options.UseForeColor = true;
            lblMinMargin.Location = new Point(17, 147);
            lblMinMargin.Name = "lblMinMargin";
            lblMinMargin.Size = new Size(97, 13);
            lblMinMargin.TabIndex = 4;
            lblMinMargin.Text = "Margen Mínimo (%):";
            // 
            // sedMinMargin
            // 
            sedMinMargin.EditValue = new decimal(new int[] { 2000, 0, 0, 131072 });
            sedMinMargin.Location = new Point(111, 144);
            sedMinMargin.Name = "sedMinMargin";
            sedMinMargin.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedMinMargin.Properties.MaskSettings.Set("mask", "n2");
            sedMinMargin.Size = new Size(69, 20);
            sedMinMargin.TabIndex = 5;
            // 
            // lblCurrentMargin
            // 
            lblCurrentMargin.Appearance.ForeColor = Color.Black;
            lblCurrentMargin.Appearance.Options.UseForeColor = true;
            lblCurrentMargin.Location = new Point(17, 191);
            lblCurrentMargin.Name = "lblCurrentMargin";
            lblCurrentMargin.Size = new Size(95, 13);
            lblCurrentMargin.TabIndex = 6;
            lblCurrentMargin.Text = "Margen Actual (%):";
            // 
            // cleCurrentMargin
            // 
            cleCurrentMargin.EditValue = new decimal(new int[] { 2800, 0, 0, 131072 });
            cleCurrentMargin.Location = new Point(111, 187);
            cleCurrentMargin.Name = "cleCurrentMargin";
            cleCurrentMargin.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cleCurrentMargin.Properties.Appearance.ForeColor = Color.FromArgb(0, 135, 60);
            cleCurrentMargin.Properties.Appearance.Options.UseFont = true;
            cleCurrentMargin.Properties.Appearance.Options.UseForeColor = true;
            cleCurrentMargin.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleCurrentMargin.Properties.DisplayFormat.FormatString = "n2";
            cleCurrentMargin.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleCurrentMargin.Properties.ReadOnly = true;
            cleCurrentMargin.Size = new Size(69, 22);
            cleCurrentMargin.TabIndex = 7;
            // 
            // chkValidatePriceBelowCost
            // 
            chkValidatePriceBelowCost.EditValue = true;
            chkValidatePriceBelowCost.Location = new Point(17, 247);
            chkValidatePriceBelowCost.Name = "chkValidatePriceBelowCost";
            chkValidatePriceBelowCost.Properties.Caption = "Validar Precio Menor al Costo";
            chkValidatePriceBelowCost.Size = new Size(171, 20);
            chkValidatePriceBelowCost.TabIndex = 8;
            // 
            // chkRequireDiscountAuthorization
            // 
            chkRequireDiscountAuthorization.Location = new Point(17, 286);
            chkRequireDiscountAuthorization.Name = "chkRequireDiscountAuthorization";
            chkRequireDiscountAuthorization.Properties.Caption = "Requiere Autorizacion para Descuento";
            chkRequireDiscountAuthorization.Size = new Size(180, 20);
            chkRequireDiscountAuthorization.TabIndex = 9;
            // 
            // grpPriceLists
            // 
            grpPriceLists.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpPriceLists.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpPriceLists.AppearanceCaption.Options.UseFont = true;
            grpPriceLists.AppearanceCaption.Options.UseForeColor = true;
            grpPriceLists.Controls.Add(btnAddPriceList);
            grpPriceLists.Controls.Add(btnEditPriceList);
            grpPriceLists.Controls.Add(btnDeletePriceList);
            grpPriceLists.Controls.Add(btnRefreshPrices);
            grpPriceLists.Controls.Add(btnSearchPriceList);
            grpPriceLists.Controls.Add(grcPrices);
            grpPriceLists.Location = new Point(531, 9);
            grpPriceLists.Name = "grpPriceLists";
            grpPriceLists.Size = new Size(523, 373);
            grpPriceLists.TabIndex = 2;
            grpPriceLists.Text = "Listas de Precios";
            // 
            // btnAddPriceList
            // 
            btnAddPriceList.Location = new Point(17, 30);
            btnAddPriceList.Name = "btnAddPriceList";
            btnAddPriceList.Size = new Size(69, 24);
            btnAddPriceList.TabIndex = 0;
            btnAddPriceList.Text = "Agregar";
            // 
            // btnEditPriceList
            // 
            btnEditPriceList.Location = new Point(90, 30);
            btnEditPriceList.Name = "btnEditPriceList";
            btnEditPriceList.Size = new Size(69, 24);
            btnEditPriceList.TabIndex = 1;
            btnEditPriceList.Text = "Editar";
            // 
            // btnDeletePriceList
            // 
            btnDeletePriceList.Location = new Point(163, 30);
            btnDeletePriceList.Name = "btnDeletePriceList";
            btnDeletePriceList.Size = new Size(77, 24);
            btnDeletePriceList.TabIndex = 2;
            btnDeletePriceList.Text = "Eliminar";
            // 
            // btnRefreshPrices
            // 
            btnRefreshPrices.Location = new Point(249, 30);
            btnRefreshPrices.Name = "btnRefreshPrices";
            btnRefreshPrices.Size = new Size(86, 24);
            btnRefreshPrices.TabIndex = 3;
            btnRefreshPrices.Text = "Actualizar";
            // 
            // btnSearchPriceList
            // 
            btnSearchPriceList.Location = new Point(386, 30);
            btnSearchPriceList.Name = "btnSearchPriceList";
            btnSearchPriceList.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Search) });
            btnSearchPriceList.Properties.NullValuePrompt = "Buscar...";
            btnSearchPriceList.Size = new Size(120, 20);
            btnSearchPriceList.TabIndex = 4;
            // 
            // grcPrices
            // 
            grcPrices.Location = new Point(13, 65);
            grcPrices.MainView = grvPrices;
            grcPrices.Name = "grcPrices";
            grcPrices.Size = new Size(497, 260);
            grcPrices.TabIndex = 5;
            grcPrices.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvPrices });
            // 
            // grvPrices
            // 
            grvPrices.Columns.AddRange(new GridColumn[] { colPriceListName, colPrice, colPriceCurrency, colPriceMargin, colPriceStartDate, colPriceEndDate, colPriceActive });
            grvPrices.DetailHeight = 303;
            grvPrices.GridControl = grcPrices;
            grvPrices.Name = "grvPrices";
            grvPrices.OptionsBehavior.Editable = false;
            grvPrices.OptionsEditForm.PopupEditFormWidth = 686;
            grvPrices.OptionsView.ShowGroupPanel = false;
            grvPrices.OptionsView.ShowIndicator = false;
            // 
            // colPriceListName
            // 
            colPriceListName.Caption = "Lista";
            colPriceListName.FieldName = "PriceListName";
            colPriceListName.MinWidth = 17;
            colPriceListName.Name = "colPriceListName";
            colPriceListName.Visible = true;
            colPriceListName.VisibleIndex = 0;
            colPriceListName.Width = 154;
            // 
            // colPrice
            // 
            colPrice.Caption = "Precio (USD)";
            colPrice.DisplayFormat.FormatString = "c2";
            colPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colPrice.FieldName = "Price";
            colPrice.MinWidth = 17;
            colPrice.Name = "colPrice";
            colPrice.Visible = true;
            colPrice.VisibleIndex = 1;
            colPrice.Width = 103;
            // 
            // colPriceCurrency
            // 
            colPriceCurrency.Caption = "Moneda";
            colPriceCurrency.FieldName = "Currency";
            colPriceCurrency.MinWidth = 17;
            colPriceCurrency.Name = "colPriceCurrency";
            colPriceCurrency.Visible = true;
            colPriceCurrency.VisibleIndex = 2;
            colPriceCurrency.Width = 77;
            // 
            // colPriceMargin
            // 
            colPriceMargin.Caption = "Margen %";
            colPriceMargin.DisplayFormat.FormatString = "n2";
            colPriceMargin.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colPriceMargin.FieldName = "Margin";
            colPriceMargin.MinWidth = 17;
            colPriceMargin.Name = "colPriceMargin";
            colPriceMargin.Visible = true;
            colPriceMargin.VisibleIndex = 3;
            colPriceMargin.Width = 94;
            // 
            // colPriceStartDate
            // 
            colPriceStartDate.Caption = "Desde";
            colPriceStartDate.DisplayFormat.FormatString = "dd/MM/yyyy";
            colPriceStartDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colPriceStartDate.FieldName = "StartDate";
            colPriceStartDate.MinWidth = 17;
            colPriceStartDate.Name = "colPriceStartDate";
            colPriceStartDate.Visible = true;
            colPriceStartDate.VisibleIndex = 4;
            colPriceStartDate.Width = 103;
            // 
            // colPriceEndDate
            // 
            colPriceEndDate.Caption = "Hasta";
            colPriceEndDate.DisplayFormat.FormatString = "dd/MM/yyyy";
            colPriceEndDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colPriceEndDate.FieldName = "EndDate";
            colPriceEndDate.MinWidth = 17;
            colPriceEndDate.Name = "colPriceEndDate";
            colPriceEndDate.Visible = true;
            colPriceEndDate.VisibleIndex = 5;
            colPriceEndDate.Width = 103;
            // 
            // colPriceActive
            // 
            colPriceActive.Caption = "Activo";
            colPriceActive.FieldName = "Active";
            colPriceActive.MinWidth = 17;
            colPriceActive.Name = "colPriceActive";
            colPriceActive.Visible = true;
            colPriceActive.VisibleIndex = 6;
            colPriceActive.Width = 77;
            // 
            // xtpCosts
            // 
            xtpCosts.Controls.Add(grpCostIndicators);
            xtpCosts.Controls.Add(grpProfitability);
            xtpCosts.Controls.Add(grpCostDates);
            xtpCosts.Controls.Add(grpCostHistory);
            xtpCosts.Name = "xtpCosts";
            xtpCosts.Size = new Size(1088, 455);
            xtpCosts.Text = "Costos";
            // 
            // grpCostIndicators
            // 
            grpCostIndicators.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpCostIndicators.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpCostIndicators.AppearanceCaption.Options.UseFont = true;
            grpCostIndicators.AppearanceCaption.Options.UseForeColor = true;
            grpCostIndicators.Controls.Add(lblAverageCost);
            grpCostIndicators.Controls.Add(cleAverageCost);
            grpCostIndicators.Controls.Add(lblLastCost);
            grpCostIndicators.Controls.Add(cleLastCost);
            grpCostIndicators.Controls.Add(lblStandardCost);
            grpCostIndicators.Controls.Add(cleStandardCost);
            grpCostIndicators.Controls.Add(lblReplacementCost);
            grpCostIndicators.Controls.Add(cleReplacementCost);
            grpCostIndicators.Controls.Add(lblCostCurrency);
            grpCostIndicators.Controls.Add(lueCostCurrency);
            grpCostIndicators.Location = new Point(9, 9);
            grpCostIndicators.Name = "grpCostIndicators";
            grpCostIndicators.Size = new Size(360, 165);
            grpCostIndicators.TabIndex = 0;
            grpCostIndicators.Text = "Indicadores de Costo";
            // 
            // lblAverageCost
            // 
            lblAverageCost.Appearance.ForeColor = Color.Black;
            lblAverageCost.Appearance.Options.UseForeColor = true;
            lblAverageCost.Location = new Point(17, 39);
            lblAverageCost.Name = "lblAverageCost";
            lblAverageCost.Size = new Size(79, 13);
            lblAverageCost.TabIndex = 0;
            lblAverageCost.Text = "Costo Promedio:";
            // 
            // cleAverageCost
            // 
            cleAverageCost.EditValue = new decimal(new int[] { 125, 0, 0, 131072 });
            cleAverageCost.Location = new Point(146, 36);
            cleAverageCost.Name = "cleAverageCost";
            cleAverageCost.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleAverageCost.Properties.DisplayFormat.FormatString = "c2";
            cleAverageCost.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleAverageCost.Properties.ReadOnly = true;
            cleAverageCost.Size = new Size(103, 20);
            cleAverageCost.TabIndex = 1;
            // 
            // lblLastCost
            // 
            lblLastCost.Appearance.ForeColor = Color.Black;
            lblLastCost.Appearance.Options.UseForeColor = true;
            lblLastCost.Location = new Point(17, 65);
            lblLastCost.Name = "lblLastCost";
            lblLastCost.Size = new Size(64, 13);
            lblLastCost.TabIndex = 2;
            lblLastCost.Text = "Último Costo:";
            // 
            // cleLastCost
            // 
            cleLastCost.EditValue = new decimal(new int[] { 124, 0, 0, 131072 });
            cleLastCost.Location = new Point(146, 62);
            cleLastCost.Name = "cleLastCost";
            cleLastCost.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleLastCost.Properties.DisplayFormat.FormatString = "c2";
            cleLastCost.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleLastCost.Properties.ReadOnly = true;
            cleLastCost.Size = new Size(103, 20);
            cleLastCost.TabIndex = 3;
            // 
            // lblStandardCost
            // 
            lblStandardCost.Appearance.ForeColor = Color.Black;
            lblStandardCost.Appearance.Options.UseForeColor = true;
            lblStandardCost.Location = new Point(17, 91);
            lblStandardCost.Name = "lblStandardCost";
            lblStandardCost.Size = new Size(78, 13);
            lblStandardCost.TabIndex = 4;
            lblStandardCost.Text = "Costo Estándar:";
            // 
            // cleStandardCost
            // 
            cleStandardCost.EditValue = new decimal(new int[] { 120, 0, 0, 131072 });
            cleStandardCost.Location = new Point(146, 88);
            cleStandardCost.Name = "cleStandardCost";
            cleStandardCost.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleStandardCost.Properties.DisplayFormat.FormatString = "c2";
            cleStandardCost.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleStandardCost.Properties.ReadOnly = true;
            cleStandardCost.Size = new Size(103, 20);
            cleStandardCost.TabIndex = 5;
            // 
            // lblReplacementCost
            // 
            lblReplacementCost.Appearance.ForeColor = Color.Black;
            lblReplacementCost.Appearance.Options.UseForeColor = true;
            lblReplacementCost.Location = new Point(17, 117);
            lblReplacementCost.Name = "lblReplacementCost";
            lblReplacementCost.Size = new Size(86, 13);
            lblReplacementCost.TabIndex = 6;
            lblReplacementCost.Text = "Costo Reposición:";
            // 
            // cleReplacementCost
            // 
            cleReplacementCost.EditValue = new decimal(new int[] { 132, 0, 0, 131072 });
            cleReplacementCost.Location = new Point(146, 114);
            cleReplacementCost.Name = "cleReplacementCost";
            cleReplacementCost.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleReplacementCost.Properties.DisplayFormat.FormatString = "c2";
            cleReplacementCost.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleReplacementCost.Properties.ReadOnly = true;
            cleReplacementCost.Size = new Size(103, 20);
            cleReplacementCost.TabIndex = 7;
            // 
            // lblCostCurrency
            // 
            lblCostCurrency.Appearance.ForeColor = Color.Black;
            lblCostCurrency.Appearance.Options.UseForeColor = true;
            lblCostCurrency.Location = new Point(17, 143);
            lblCostCurrency.Name = "lblCostCurrency";
            lblCostCurrency.Size = new Size(88, 13);
            lblCostCurrency.TabIndex = 8;
            lblCostCurrency.Text = "Moneda de Costo:";
            // 
            // lueCostCurrency
            // 
            lueCostCurrency.EditValue = "USD - Dolar Americano";
            lueCostCurrency.Location = new Point(146, 140);
            lueCostCurrency.Name = "lueCostCurrency";
            lueCostCurrency.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueCostCurrency.Properties.DataSource = new string[]
    {
    "USD - Dolar Americano",
    "EUR - Euro"
    };
            lueCostCurrency.Properties.NullText = "";
            lueCostCurrency.Size = new Size(180, 20);
            lueCostCurrency.TabIndex = 9;
            // 
            // grpProfitability
            // 
            grpProfitability.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpProfitability.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpProfitability.AppearanceCaption.Options.UseFont = true;
            grpProfitability.AppearanceCaption.Options.UseForeColor = true;
            grpProfitability.Controls.Add(lblProfitBasePrice);
            grpProfitability.Controls.Add(cleProfitBasePrice);
            grpProfitability.Controls.Add(lblEstimatedMargin);
            grpProfitability.Controls.Add(cleEstimatedMargin);
            grpProfitability.Controls.Add(lblEstimatedUtility);
            grpProfitability.Controls.Add(cleEstimatedUtility);
            grpProfitability.Controls.Add(lblMarkup);
            grpProfitability.Controls.Add(cleMarkup);
            grpProfitability.Controls.Add(lblProfitability);
            grpProfitability.Controls.Add(cleProfitability);
            grpProfitability.Location = new Point(377, 9);
            grpProfitability.Name = "grpProfitability";
            grpProfitability.Size = new Size(317, 165);
            grpProfitability.TabIndex = 1;
            grpProfitability.Text = "Rentabilidad";
            // 
            // lblProfitBasePrice
            // 
            lblProfitBasePrice.Appearance.ForeColor = Color.Black;
            lblProfitBasePrice.Appearance.Options.UseForeColor = true;
            lblProfitBasePrice.Location = new Point(17, 39);
            lblProfitBasePrice.Name = "lblProfitBasePrice";
            lblProfitBasePrice.Size = new Size(105, 13);
            lblProfitBasePrice.TabIndex = 0;
            lblProfitBasePrice.Text = "Precio Base de Venta:";
            // 
            // cleProfitBasePrice
            // 
            cleProfitBasePrice.EditValue = new decimal(new int[] { 160, 0, 0, 131072 });
            cleProfitBasePrice.Location = new Point(163, 36);
            cleProfitBasePrice.Name = "cleProfitBasePrice";
            cleProfitBasePrice.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleProfitBasePrice.Properties.DisplayFormat.FormatString = "n2";
            cleProfitBasePrice.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleProfitBasePrice.Properties.ReadOnly = true;
            cleProfitBasePrice.Size = new Size(86, 20);
            cleProfitBasePrice.TabIndex = 1;
            // 
            // lblEstimatedMargin
            // 
            lblEstimatedMargin.Appearance.ForeColor = Color.Black;
            lblEstimatedMargin.Appearance.Options.UseForeColor = true;
            lblEstimatedMargin.Location = new Point(17, 65);
            lblEstimatedMargin.Name = "lblEstimatedMargin";
            lblEstimatedMargin.Size = new Size(86, 13);
            lblEstimatedMargin.TabIndex = 2;
            lblEstimatedMargin.Text = "Margen Estimado:";
            // 
            // cleEstimatedMargin
            // 
            cleEstimatedMargin.EditValue = new decimal(new int[] { 2800, 0, 0, 131072 });
            cleEstimatedMargin.Location = new Point(163, 62);
            cleEstimatedMargin.Name = "cleEstimatedMargin";
            cleEstimatedMargin.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cleEstimatedMargin.Properties.Appearance.ForeColor = Color.FromArgb(0, 135, 60);
            cleEstimatedMargin.Properties.Appearance.Options.UseFont = true;
            cleEstimatedMargin.Properties.Appearance.Options.UseForeColor = true;
            cleEstimatedMargin.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleEstimatedMargin.Properties.DisplayFormat.FormatString = "n2";
            cleEstimatedMargin.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleEstimatedMargin.Properties.ReadOnly = true;
            cleEstimatedMargin.Size = new Size(86, 22);
            cleEstimatedMargin.TabIndex = 3;
            // 
            // lblEstimatedUtility
            // 
            lblEstimatedUtility.Appearance.ForeColor = Color.Black;
            lblEstimatedUtility.Appearance.Options.UseForeColor = true;
            lblEstimatedUtility.Location = new Point(17, 91);
            lblEstimatedUtility.Name = "lblEstimatedUtility";
            lblEstimatedUtility.Size = new Size(85, 13);
            lblEstimatedUtility.TabIndex = 4;
            lblEstimatedUtility.Text = "Utilidad Estimada:";
            // 
            // cleEstimatedUtility
            // 
            cleEstimatedUtility.EditValue = new decimal(new int[] { 35, 0, 0, 131072 });
            cleEstimatedUtility.Location = new Point(163, 88);
            cleEstimatedUtility.Name = "cleEstimatedUtility";
            cleEstimatedUtility.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleEstimatedUtility.Properties.DisplayFormat.FormatString = "n2";
            cleEstimatedUtility.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleEstimatedUtility.Properties.ReadOnly = true;
            cleEstimatedUtility.Size = new Size(86, 20);
            cleEstimatedUtility.TabIndex = 5;
            // 
            // lblMarkup
            // 
            lblMarkup.Appearance.ForeColor = Color.Black;
            lblMarkup.Appearance.Options.UseForeColor = true;
            lblMarkup.Location = new Point(17, 117);
            lblMarkup.Name = "lblMarkup";
            lblMarkup.Size = new Size(43, 13);
            lblMarkup.TabIndex = 6;
            lblMarkup.Text = "Mark-up:";
            // 
            // cleMarkup
            // 
            cleMarkup.EditValue = new decimal(new int[] { 2800, 0, 0, 131072 });
            cleMarkup.Location = new Point(163, 114);
            cleMarkup.Name = "cleMarkup";
            cleMarkup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleMarkup.Properties.DisplayFormat.FormatString = "n2";
            cleMarkup.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleMarkup.Properties.ReadOnly = true;
            cleMarkup.Size = new Size(86, 20);
            cleMarkup.TabIndex = 7;
            // 
            // lblProfitability
            // 
            lblProfitability.Appearance.ForeColor = Color.Black;
            lblProfitability.Appearance.Options.UseForeColor = true;
            lblProfitability.Location = new Point(17, 143);
            lblProfitability.Name = "lblProfitability";
            lblProfitability.Size = new Size(63, 13);
            lblProfitability.TabIndex = 8;
            lblProfitability.Text = "Rentabilidad:";
            // 
            // cleProfitability
            // 
            cleProfitability.EditValue = new decimal(new int[] { 2188, 0, 0, 131072 });
            cleProfitability.Location = new Point(163, 140);
            cleProfitability.Name = "cleProfitability";
            cleProfitability.Properties.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            cleProfitability.Properties.Appearance.ForeColor = Color.FromArgb(0, 135, 60);
            cleProfitability.Properties.Appearance.Options.UseFont = true;
            cleProfitability.Properties.Appearance.Options.UseForeColor = true;
            cleProfitability.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleProfitability.Properties.DisplayFormat.FormatString = "n2";
            cleProfitability.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleProfitability.Properties.ReadOnly = true;
            cleProfitability.Size = new Size(86, 22);
            cleProfitability.TabIndex = 9;
            // 
            // grpCostDates
            // 
            grpCostDates.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpCostDates.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpCostDates.AppearanceCaption.Options.UseFont = true;
            grpCostDates.AppearanceCaption.Options.UseForeColor = true;
            grpCostDates.Controls.Add(lblLastPurchaseDate);
            grpCostDates.Controls.Add(dtpLastPurchase);
            grpCostDates.Controls.Add(lblLastSaleDate);
            grpCostDates.Controls.Add(dtpLastSale);
            grpCostDates.Controls.Add(lblDaysFromLastPurchase);
            grpCostDates.Controls.Add(sedDaysFromLastPurchase);
            grpCostDates.Controls.Add(lblDaysFromLastSale);
            grpCostDates.Controls.Add(sedDaysFromLastSale);
            grpCostDates.Controls.Add(lblRotation30);
            grpCostDates.Controls.Add(cleRotation30);
            grpCostDates.Controls.Add(lblRotation90);
            grpCostDates.Controls.Add(cleRotation90);
            grpCostDates.Location = new Point(703, 9);
            grpCostDates.Name = "grpCostDates";
            grpCostDates.Size = new Size(351, 165);
            grpCostDates.TabIndex = 2;
            grpCostDates.Text = "Fechas y Rotacion";
            // 
            // lblLastPurchaseDate
            // 
            lblLastPurchaseDate.Appearance.ForeColor = Color.Black;
            lblLastPurchaseDate.Appearance.Options.UseForeColor = true;
            lblLastPurchaseDate.Location = new Point(17, 39);
            lblLastPurchaseDate.Name = "lblLastPurchaseDate";
            lblLastPurchaseDate.Size = new Size(105, 13);
            lblLastPurchaseDate.TabIndex = 0;
            lblLastPurchaseDate.Text = "Fecha Última Compra:";
            // 
            // dtpLastPurchase
            // 
            dtpLastPurchase.EditValue = new DateTime(2026, 5, 10, 0, 0, 0, 0);
            dtpLastPurchase.Location = new Point(163, 36);
            dtpLastPurchase.Name = "dtpLastPurchase";
            dtpLastPurchase.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            dtpLastPurchase.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            dtpLastPurchase.Size = new Size(154, 20);
            dtpLastPurchase.TabIndex = 1;
            // 
            // lblLastSaleDate
            // 
            lblLastSaleDate.Appearance.ForeColor = Color.Black;
            lblLastSaleDate.Appearance.Options.UseForeColor = true;
            lblLastSaleDate.Location = new Point(17, 65);
            lblLastSaleDate.Name = "lblLastSaleDate";
            lblLastSaleDate.Size = new Size(96, 13);
            lblLastSaleDate.TabIndex = 2;
            lblLastSaleDate.Text = "Fecha Última Venta:";
            // 
            // dtpLastSale
            // 
            dtpLastSale.EditValue = new DateTime(2026, 5, 10, 0, 0, 0, 0);
            dtpLastSale.Location = new Point(163, 62);
            dtpLastSale.Name = "dtpLastSale";
            dtpLastSale.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            dtpLastSale.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            dtpLastSale.Size = new Size(154, 20);
            dtpLastSale.TabIndex = 3;
            // 
            // lblDaysFromLastPurchase
            // 
            lblDaysFromLastPurchase.Appearance.ForeColor = Color.Black;
            lblDaysFromLastPurchase.Appearance.Options.UseForeColor = true;
            lblDaysFromLastPurchase.Location = new Point(17, 91);
            lblDaysFromLastPurchase.Name = "lblDaysFromLastPurchase";
            lblDaysFromLastPurchase.Size = new Size(128, 13);
            lblDaysFromLastPurchase.TabIndex = 4;
            lblDaysFromLastPurchase.Text = "Días desde Última Compra:";
            // 
            // sedDaysFromLastPurchase
            // 
            sedDaysFromLastPurchase.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
            sedDaysFromLastPurchase.Location = new Point(163, 88);
            sedDaysFromLastPurchase.Name = "sedDaysFromLastPurchase";
            sedDaysFromLastPurchase.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedDaysFromLastPurchase.Properties.IsFloatValue = false;
            sedDaysFromLastPurchase.Properties.MaskSettings.Set("mask", "N00");
            sedDaysFromLastPurchase.Size = new Size(154, 20);
            sedDaysFromLastPurchase.TabIndex = 5;
            // 
            // lblDaysFromLastSale
            // 
            lblDaysFromLastSale.Appearance.ForeColor = Color.Black;
            lblDaysFromLastSale.Appearance.Options.UseForeColor = true;
            lblDaysFromLastSale.Location = new Point(17, 117);
            lblDaysFromLastSale.Name = "lblDaysFromLastSale";
            lblDaysFromLastSale.Size = new Size(119, 13);
            lblDaysFromLastSale.TabIndex = 6;
            lblDaysFromLastSale.Text = "Días desde Última Venta:";
            // 
            // sedDaysFromLastSale
            // 
            sedDaysFromLastSale.EditValue = new decimal(new int[] { 0, 0, 0, 0 });
            sedDaysFromLastSale.Location = new Point(163, 114);
            sedDaysFromLastSale.Name = "sedDaysFromLastSale";
            sedDaysFromLastSale.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sedDaysFromLastSale.Properties.IsFloatValue = false;
            sedDaysFromLastSale.Properties.MaskSettings.Set("mask", "N00");
            sedDaysFromLastSale.Size = new Size(154, 20);
            sedDaysFromLastSale.TabIndex = 7;
            // 
            // lblRotation30
            // 
            lblRotation30.Appearance.ForeColor = Color.Black;
            lblRotation30.Appearance.Options.UseForeColor = true;
            lblRotation30.Location = new Point(17, 143);
            lblRotation30.Name = "lblRotation30";
            lblRotation30.Size = new Size(91, 13);
            lblRotation30.TabIndex = 8;
            lblRotation30.Text = "Rotación (30 días):";
            // 
            // cleRotation30
            // 
            cleRotation30.EditValue = new decimal(new int[] { 1560, 0, 0, 131072 });
            cleRotation30.Location = new Point(163, 140);
            cleRotation30.Name = "cleRotation30";
            cleRotation30.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleRotation30.Properties.DisplayFormat.FormatString = "n2";
            cleRotation30.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleRotation30.Properties.ReadOnly = true;
            cleRotation30.Size = new Size(64, 20);
            cleRotation30.TabIndex = 9;
            // 
            // lblRotation90
            // 
            lblRotation90.Appearance.ForeColor = Color.Black;
            lblRotation90.Appearance.Options.UseForeColor = true;
            lblRotation90.Location = new Point(236, 143);
            lblRotation90.Name = "lblRotation90";
            lblRotation90.Size = new Size(16, 13);
            lblRotation90.TabIndex = 10;
            lblRotation90.Text = "90:";
            // 
            // cleRotation90
            // 
            cleRotation90.EditValue = new decimal(new int[] { 4830, 0, 0, 131072 });
            cleRotation90.Location = new Point(257, 140);
            cleRotation90.Name = "cleRotation90";
            cleRotation90.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            cleRotation90.Properties.DisplayFormat.FormatString = "n2";
            cleRotation90.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            cleRotation90.Properties.ReadOnly = true;
            cleRotation90.Size = new Size(60, 20);
            cleRotation90.TabIndex = 11;
            // 
            // grpCostHistory
            // 
            grpCostHistory.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpCostHistory.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpCostHistory.AppearanceCaption.Options.UseFont = true;
            grpCostHistory.AppearanceCaption.Options.UseForeColor = true;
            grpCostHistory.Controls.Add(btnRefreshCosts);
            grpCostHistory.Controls.Add(btnExportCosts);
            grpCostHistory.Controls.Add(grcCosts);
            grpCostHistory.Location = new Point(9, 182);
            grpCostHistory.Name = "grpCostHistory";
            grpCostHistory.Size = new Size(1046, 269);
            grpCostHistory.TabIndex = 3;
            grpCostHistory.Text = "Historial de Costos";
            // 
            // btnRefreshCosts
            // 
            btnRefreshCosts.Location = new Point(17, 30);
            btnRefreshCosts.Name = "btnRefreshCosts";
            btnRefreshCosts.Size = new Size(86, 24);
            btnRefreshCosts.TabIndex = 0;
            btnRefreshCosts.Text = "Actualizar";
            // 
            // btnExportCosts
            // 
            btnExportCosts.Location = new Point(111, 30);
            btnExportCosts.Name = "btnExportCosts";
            btnExportCosts.Size = new Size(86, 24);
            btnExportCosts.TabIndex = 1;
            btnExportCosts.Text = "Exportar";
            // 
            // grcCosts
            // 
            grcCosts.Location = new Point(13, 65);
            grcCosts.MainView = grvCosts;
            grcCosts.Name = "grcCosts";
            grcCosts.Size = new Size(1020, 182);
            grcCosts.TabIndex = 2;
            grcCosts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvCosts });
            // 
            // grvCosts
            // 
            grvCosts.Columns.AddRange(new GridColumn[] { colCostDate, colCostDocument, colCostVendor, colCostQuantity, colCostPrevious, colCostNew, colCostCurrency, colCostUser });
            grvCosts.DetailHeight = 303;
            grvCosts.GridControl = grcCosts;
            grvCosts.Name = "grvCosts";
            grvCosts.OptionsBehavior.Editable = false;
            grvCosts.OptionsEditForm.PopupEditFormWidth = 686;
            grvCosts.OptionsView.ShowFooter = true;
            grvCosts.OptionsView.ShowGroupPanel = false;
            grvCosts.OptionsView.ShowIndicator = false;
            // 
            // colCostDate
            // 
            colCostDate.Caption = "Fecha";
            colCostDate.DisplayFormat.FormatString = "dd/MM/yyyy";
            colCostDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            colCostDate.FieldName = "CostDate";
            colCostDate.MinWidth = 17;
            colCostDate.Name = "colCostDate";
            colCostDate.Visible = true;
            colCostDate.VisibleIndex = 0;
            colCostDate.Width = 94;
            // 
            // colCostDocument
            // 
            colCostDocument.Caption = "Documento";
            colCostDocument.FieldName = "Document";
            colCostDocument.MinWidth = 17;
            colCostDocument.Name = "colCostDocument";
            colCostDocument.Visible = true;
            colCostDocument.VisibleIndex = 1;
            colCostDocument.Width = 111;
            // 
            // colCostVendor
            // 
            colCostVendor.Caption = "Proveedor";
            colCostVendor.FieldName = "Vendor";
            colCostVendor.MinWidth = 17;
            colCostVendor.Name = "colCostVendor";
            colCostVendor.Visible = true;
            colCostVendor.VisibleIndex = 2;
            colCostVendor.Width = 223;
            // 
            // colCostQuantity
            // 
            colCostQuantity.Caption = "Cantidad";
            colCostQuantity.DisplayFormat.FormatString = "n2";
            colCostQuantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colCostQuantity.FieldName = "Quantity";
            colCostQuantity.MinWidth = 17;
            colCostQuantity.Name = "colCostQuantity";
            colCostQuantity.Summary.AddRange(new GridSummaryItem[] { new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Quantity", "{0:n2}") });
            colCostQuantity.Visible = true;
            colCostQuantity.VisibleIndex = 3;
            colCostQuantity.Width = 103;
            // 
            // colCostPrevious
            // 
            colCostPrevious.Caption = "Costo Anterior";
            colCostPrevious.DisplayFormat.FormatString = "c2";
            colCostPrevious.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colCostPrevious.FieldName = "PreviousCost";
            colCostPrevious.MinWidth = 17;
            colCostPrevious.Name = "colCostPrevious";
            colCostPrevious.Summary.AddRange(new GridSummaryItem[] { new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Average, "PreviousCost", "{0:c2}") });
            colCostPrevious.Visible = true;
            colCostPrevious.VisibleIndex = 4;
            colCostPrevious.Width = 111;
            // 
            // colCostNew
            // 
            colCostNew.Caption = "Nuevo Costo";
            colCostNew.DisplayFormat.FormatString = "c2";
            colCostNew.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            colCostNew.FieldName = "NewCost";
            colCostNew.MinWidth = 17;
            colCostNew.Name = "colCostNew";
            colCostNew.Summary.AddRange(new GridSummaryItem[] { new GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Average, "NewCost", "{0:c2}") });
            colCostNew.Visible = true;
            colCostNew.VisibleIndex = 5;
            colCostNew.Width = 111;
            // 
            // colCostCurrency
            // 
            colCostCurrency.Caption = "Moneda";
            colCostCurrency.FieldName = "Currency";
            colCostCurrency.MinWidth = 17;
            colCostCurrency.Name = "colCostCurrency";
            colCostCurrency.Visible = true;
            colCostCurrency.VisibleIndex = 6;
            colCostCurrency.Width = 86;
            // 
            // colCostUser
            // 
            colCostUser.Caption = "Usuario";
            colCostUser.FieldName = "UserName";
            colCostUser.MinWidth = 17;
            colCostUser.Name = "colCostUser";
            colCostUser.Visible = true;
            colCostUser.VisibleIndex = 7;
            colCostUser.Width = 103;
            // 
            // xtpSap
            // 
            xtpSap.Controls.Add(grpSapIntegration);
            xtpSap.Controls.Add(grpSapActions);
            xtpSap.Controls.Add(grpSapUdf);
            xtpSap.Name = "xtpSap";
            xtpSap.Size = new Size(1088, 455);
            xtpSap.Text = "SAP Business One";
            // 
            // grpSapIntegration
            // 
            grpSapIntegration.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpSapIntegration.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpSapIntegration.AppearanceCaption.Options.UseFont = true;
            grpSapIntegration.AppearanceCaption.Options.UseForeColor = true;
            grpSapIntegration.Controls.Add(lblSapCode);
            grpSapIntegration.Controls.Add(txtSapCode);
            grpSapIntegration.Controls.Add(lblSapStatus);
            grpSapIntegration.Controls.Add(lueSapStatus);
            grpSapIntegration.Controls.Add(lblLastSapSync);
            grpSapIntegration.Controls.Add(dtpLastSapSync);
            grpSapIntegration.Controls.Add(lblSapDatabase);
            grpSapIntegration.Controls.Add(txtSapDatabase);
            grpSapIntegration.Controls.Add(lblSapGroup);
            grpSapIntegration.Controls.Add(txtSapGroup);
            grpSapIntegration.Controls.Add(lblSapUom);
            grpSapIntegration.Controls.Add(txtSapUom);
            grpSapIntegration.Controls.Add(lblSapMessage);
            grpSapIntegration.Controls.Add(memSapMessage);
            grpSapIntegration.Location = new Point(9, 9);
            grpSapIntegration.Name = "grpSapIntegration";
            grpSapIntegration.Size = new Size(557, 225);
            grpSapIntegration.TabIndex = 0;
            grpSapIntegration.Text = "Integracion SAP Business One";
            // 
            // lblSapCode
            // 
            lblSapCode.Appearance.ForeColor = Color.Black;
            lblSapCode.Appearance.Options.UseForeColor = true;
            lblSapCode.Location = new Point(17, 35);
            lblSapCode.Name = "lblSapCode";
            lblSapCode.Size = new Size(59, 13);
            lblSapCode.TabIndex = 0;
            lblSapCode.Text = "Código SAP:";
            // 
            // txtSapCode
            // 
            txtSapCode.EditValue = "A000001";
            txtSapCode.Location = new Point(180, 31);
            txtSapCode.Name = "txtSapCode";
            txtSapCode.Size = new Size(351, 20);
            txtSapCode.TabIndex = 1;
            // 
            // lblSapStatus
            // 
            lblSapStatus.Appearance.ForeColor = Color.Black;
            lblSapStatus.Appearance.Options.UseForeColor = true;
            lblSapStatus.Location = new Point(17, 61);
            lblSapStatus.Name = "lblSapStatus";
            lblSapStatus.Size = new Size(107, 13);
            lblSapStatus.TabIndex = 2;
            lblSapStatus.Text = "Estado Sincronización:";
            // 
            // lueSapStatus
            // 
            lueSapStatus.EditValue = "Sincronizado";
            lueSapStatus.Location = new Point(180, 57);
            lueSapStatus.Name = "lueSapStatus";
            lueSapStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueSapStatus.Properties.DataSource = new string[]
    {
    "Sincronizado",
    "Pendiente",
    "Error"
    };
            lueSapStatus.Properties.NullText = "";
            lueSapStatus.Size = new Size(351, 20);
            lueSapStatus.TabIndex = 3;
            // 
            // lblLastSapSync
            // 
            lblLastSapSync.Appearance.ForeColor = Color.Black;
            lblLastSapSync.Appearance.Options.UseForeColor = true;
            lblLastSapSync.Location = new Point(17, 87);
            lblLastSapSync.Name = "lblLastSapSync";
            lblLastSapSync.Size = new Size(91, 13);
            lblLastSapSync.TabIndex = 4;
            lblLastSapSync.Text = "Fecha Última Sync:";
            // 
            // dtpLastSapSync
            // 
            dtpLastSapSync.EditValue = new DateTime(2026, 5, 10, 9, 25, 11, 0);
            dtpLastSapSync.Location = new Point(180, 83);
            dtpLastSapSync.Name = "dtpLastSapSync";
            dtpLastSapSync.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            dtpLastSapSync.Properties.CalendarTimeProperties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            dtpLastSapSync.Size = new Size(351, 20);
            dtpLastSapSync.TabIndex = 5;
            // 
            // lblSapDatabase
            // 
            lblSapDatabase.Appearance.ForeColor = Color.Black;
            lblSapDatabase.Appearance.Options.UseForeColor = true;
            lblSapDatabase.Location = new Point(17, 113);
            lblSapDatabase.Name = "lblSapDatabase";
            lblSapDatabase.Size = new Size(88, 13);
            lblSapDatabase.TabIndex = 6;
            lblSapDatabase.Text = "Base SAP Destino:";
            // 
            // txtSapDatabase
            // 
            txtSapDatabase.EditValue = "SBODEMOUS";
            txtSapDatabase.Location = new Point(180, 109);
            txtSapDatabase.Name = "txtSapDatabase";
            txtSapDatabase.Size = new Size(351, 20);
            txtSapDatabase.TabIndex = 7;
            // 
            // lblSapGroup
            // 
            lblSapGroup.Appearance.ForeColor = Color.Black;
            lblSapGroup.Appearance.Options.UseForeColor = true;
            lblSapGroup.Location = new Point(17, 139);
            lblSapGroup.Name = "lblSapGroup";
            lblSapGroup.Size = new Size(55, 13);
            lblSapGroup.TabIndex = 8;
            lblSapGroup.Text = "Grupo SAP:";
            // 
            // txtSapGroup
            // 
            txtSapGroup.EditValue = "01 - PRODUCTOS PRIMERA NECESIDAD";
            txtSapGroup.Location = new Point(180, 135);
            txtSapGroup.Name = "txtSapGroup";
            txtSapGroup.Size = new Size(351, 20);
            txtSapGroup.TabIndex = 9;
            // 
            // lblSapUom
            // 
            lblSapUom.Appearance.ForeColor = Color.Black;
            lblSapUom.Appearance.Options.UseForeColor = true;
            lblSapUom.Location = new Point(17, 165);
            lblSapUom.Name = "lblSapUom";
            lblSapUom.Size = new Size(96, 13);
            lblSapUom.TabIndex = 10;
            lblSapUom.Text = "Unidad Medida SAP:";
            // 
            // txtSapUom
            // 
            txtSapUom.EditValue = "UNIDAD";
            txtSapUom.Location = new Point(180, 161);
            txtSapUom.Name = "txtSapUom";
            txtSapUom.Size = new Size(351, 20);
            txtSapUom.TabIndex = 11;
            // 
            // lblSapMessage
            // 
            lblSapMessage.Appearance.ForeColor = Color.Black;
            lblSapMessage.Appearance.Options.UseForeColor = true;
            lblSapMessage.Location = new Point(17, 191);
            lblSapMessage.Name = "lblSapMessage";
            lblSapMessage.Size = new Size(66, 13);
            lblSapMessage.TabIndex = 12;
            lblSapMessage.Text = "Mensaje SAP:";
            // 
            // memSapMessage
            // 
            memSapMessage.EditValue = "Sincronizacion exitosa.";
            memSapMessage.Location = new Point(180, 187);
            memSapMessage.Name = "memSapMessage";
            memSapMessage.Size = new Size(351, 30);
            memSapMessage.TabIndex = 13;
            // 
            // grpSapActions
            // 
            grpSapActions.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpSapActions.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpSapActions.AppearanceCaption.Options.UseFont = true;
            grpSapActions.AppearanceCaption.Options.UseForeColor = true;
            grpSapActions.Controls.Add(btnSyncNow);
            grpSapActions.Controls.Add(lblSyncNowTitle);
            grpSapActions.Controls.Add(lblSyncNowDescription);
            grpSapActions.Controls.Add(btnOpenSap);
            grpSapActions.Controls.Add(lblOpenSapTitle);
            grpSapActions.Controls.Add(lblOpenSapDescription);
            grpSapActions.Controls.Add(btnViewIntegrationLog);
            grpSapActions.Controls.Add(lblViewLogTitle);
            grpSapActions.Controls.Add(lblViewLogDescription);
            grpSapActions.Location = new Point(574, 9);
            grpSapActions.Name = "grpSapActions";
            grpSapActions.Size = new Size(480, 225);
            grpSapActions.TabIndex = 1;
            grpSapActions.Text = "Acciones de Integracion";
            // 
            // btnSyncNow
            // 
            btnSyncNow.Location = new Point(21, 39);
            btnSyncNow.Name = "btnSyncNow";
            btnSyncNow.Size = new Size(137, 35);
            btnSyncNow.TabIndex = 0;
            btnSyncNow.Text = "Sincronizar Ahora";
            // 
            // lblSyncNowTitle
            // 
            lblSyncNowTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSyncNowTitle.Appearance.ForeColor = Color.Black;
            lblSyncNowTitle.Appearance.Options.UseFont = true;
            lblSyncNowTitle.Appearance.Options.UseForeColor = true;
            lblSyncNowTitle.Location = new Point(171, 39);
            lblSyncNowTitle.Name = "lblSyncNowTitle";
            lblSyncNowTitle.Size = new Size(98, 15);
            lblSyncNowTitle.TabIndex = 1;
            lblSyncNowTitle.Text = "Sincronizar Ahora";
            // 
            // lblSyncNowDescription
            // 
            lblSyncNowDescription.Appearance.ForeColor = Color.Black;
            lblSyncNowDescription.Appearance.Options.UseForeColor = true;
            lblSyncNowDescription.Location = new Point(171, 59);
            lblSyncNowDescription.Name = "lblSyncNowDescription";
            lblSyncNowDescription.Size = new Size(176, 13);
            lblSyncNowDescription.TabIndex = 2;
            lblSyncNowDescription.Text = "Envía los cambios de este ítem a SAP";
            // 
            // btnOpenSap
            // 
            btnOpenSap.Location = new Point(21, 91);
            btnOpenSap.Name = "btnOpenSap";
            btnOpenSap.Size = new Size(137, 35);
            btnOpenSap.TabIndex = 3;
            btnOpenSap.Text = "Consultar en SAP";
            // 
            // lblOpenSapTitle
            // 
            lblOpenSapTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblOpenSapTitle.Appearance.ForeColor = Color.Black;
            lblOpenSapTitle.Appearance.Options.UseFont = true;
            lblOpenSapTitle.Appearance.Options.UseForeColor = true;
            lblOpenSapTitle.Location = new Point(171, 91);
            lblOpenSapTitle.Name = "lblOpenSapTitle";
            lblOpenSapTitle.Size = new Size(94, 15);
            lblOpenSapTitle.TabIndex = 4;
            lblOpenSapTitle.Text = "Consultar en SAP";
            // 
            // lblOpenSapDescription
            // 
            lblOpenSapDescription.Appearance.ForeColor = Color.Black;
            lblOpenSapDescription.Appearance.Options.UseForeColor = true;
            lblOpenSapDescription.Location = new Point(171, 111);
            lblOpenSapDescription.Name = "lblOpenSapDescription";
            lblOpenSapDescription.Size = new Size(180, 13);
            lblOpenSapDescription.TabIndex = 5;
            lblOpenSapDescription.Text = "Consulta la información actual en SAP";
            // 
            // btnViewIntegrationLog
            // 
            btnViewIntegrationLog.Location = new Point(21, 143);
            btnViewIntegrationLog.Name = "btnViewIntegrationLog";
            btnViewIntegrationLog.Size = new Size(137, 35);
            btnViewIntegrationLog.TabIndex = 6;
            btnViewIntegrationLog.Text = "Ver Log Integración";
            // 
            // lblViewLogTitle
            // 
            lblViewLogTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblViewLogTitle.Appearance.ForeColor = Color.Black;
            lblViewLogTitle.Appearance.Options.UseFont = true;
            lblViewLogTitle.Appearance.Options.UseForeColor = true;
            lblViewLogTitle.Location = new Point(171, 143);
            lblViewLogTitle.Name = "lblViewLogTitle";
            lblViewLogTitle.Size = new Size(110, 15);
            lblViewLogTitle.TabIndex = 7;
            lblViewLogTitle.Text = "Ver Log Integración";
            // 
            // lblViewLogDescription
            // 
            lblViewLogDescription.Appearance.ForeColor = Color.Black;
            lblViewLogDescription.Appearance.Options.UseForeColor = true;
            lblViewLogDescription.Location = new Point(171, 163);
            lblViewLogDescription.Name = "lblViewLogDescription";
            lblViewLogDescription.Size = new Size(178, 13);
            lblViewLogDescription.TabIndex = 8;
            lblViewLogDescription.Text = "Revisa el historial de sincronizaciones";
            // 
            // grpSapUdf
            // 
            grpSapUdf.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpSapUdf.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpSapUdf.AppearanceCaption.Options.UseFont = true;
            grpSapUdf.AppearanceCaption.Options.UseForeColor = true;
            grpSapUdf.Controls.Add(grcSapUdf);
            grpSapUdf.Controls.Add(lblSapUdfTotalRecords);
            grpSapUdf.Controls.Add(btnSapUdfFirst);
            grpSapUdf.Controls.Add(btnSapUdfPrevious);
            grpSapUdf.Controls.Add(lueSapUdfPageSize);
            grpSapUdf.Controls.Add(lblSapUdfPageInfo);
            grpSapUdf.Controls.Add(btnSapUdfNext);
            grpSapUdf.Controls.Add(btnSapUdfLast);
            grpSapUdf.Location = new Point(9, 243);
            grpSapUdf.Name = "grpSapUdf";
            grpSapUdf.Size = new Size(1046, 208);
            grpSapUdf.TabIndex = 2;
            grpSapUdf.Text = "Campos UDF";
            // 
            // grcSapUdf
            // 
            grcSapUdf.Location = new Point(13, 30);
            grcSapUdf.MainView = grvSapUdf;
            grcSapUdf.Name = "grcSapUdf";
            grcSapUdf.Size = new Size(1020, 130);
            grcSapUdf.TabIndex = 0;
            grcSapUdf.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvSapUdf });
            // 
            // grvSapUdf
            // 
            grvSapUdf.Columns.AddRange(new GridColumn[] { colSapUdfField, colSapUdfDescription, colSapUdfLocalValue, colSapUdfSapValue, colSapUdfStatus });
            grvSapUdf.DetailHeight = 303;
            grvSapUdf.GridControl = grcSapUdf;
            grvSapUdf.Name = "grvSapUdf";
            grvSapUdf.OptionsBehavior.Editable = false;
            grvSapUdf.OptionsEditForm.PopupEditFormWidth = 686;
            grvSapUdf.OptionsView.ShowGroupPanel = false;
            grvSapUdf.OptionsView.ShowIndicator = false;
            // 
            // colSapUdfField
            // 
            colSapUdfField.Caption = "Campo";
            colSapUdfField.FieldName = "Field";
            colSapUdfField.MinWidth = 17;
            colSapUdfField.Name = "colSapUdfField";
            colSapUdfField.Visible = true;
            colSapUdfField.VisibleIndex = 0;
            colSapUdfField.Width = 171;
            // 
            // colSapUdfDescription
            // 
            colSapUdfDescription.Caption = "Descripcion";
            colSapUdfDescription.FieldName = "Description";
            colSapUdfDescription.MinWidth = 17;
            colSapUdfDescription.Name = "colSapUdfDescription";
            colSapUdfDescription.Visible = true;
            colSapUdfDescription.VisibleIndex = 1;
            colSapUdfDescription.Width = 223;
            // 
            // colSapUdfLocalValue
            // 
            colSapUdfLocalValue.Caption = "Valor Local";
            colSapUdfLocalValue.FieldName = "LocalValue";
            colSapUdfLocalValue.MinWidth = 17;
            colSapUdfLocalValue.Name = "colSapUdfLocalValue";
            colSapUdfLocalValue.Visible = true;
            colSapUdfLocalValue.VisibleIndex = 2;
            colSapUdfLocalValue.Width = 163;
            // 
            // colSapUdfSapValue
            // 
            colSapUdfSapValue.Caption = "Valor SAP";
            colSapUdfSapValue.FieldName = "SapValue";
            colSapUdfSapValue.MinWidth = 17;
            colSapUdfSapValue.Name = "colSapUdfSapValue";
            colSapUdfSapValue.Visible = true;
            colSapUdfSapValue.VisibleIndex = 3;
            colSapUdfSapValue.Width = 163;
            // 
            // colSapUdfStatus
            // 
            colSapUdfStatus.Caption = "Estado";
            colSapUdfStatus.FieldName = "Status";
            colSapUdfStatus.MinWidth = 17;
            colSapUdfStatus.Name = "colSapUdfStatus";
            colSapUdfStatus.Visible = true;
            colSapUdfStatus.VisibleIndex = 4;
            colSapUdfStatus.Width = 137;
            // 
            // lblSapUdfTotalRecords
            // 
            lblSapUdfTotalRecords.Appearance.ForeColor = Color.Black;
            lblSapUdfTotalRecords.Appearance.Options.UseForeColor = true;
            lblSapUdfTotalRecords.Location = new Point(17, 182);
            lblSapUdfTotalRecords.Name = "lblSapUdfTotalRecords";
            lblSapUdfTotalRecords.Size = new Size(85, 13);
            lblSapUdfTotalRecords.TabIndex = 1;
            lblSapUdfTotalRecords.Text = "Total Registros: 5";
            // 
            // btnSapUdfFirst
            // 
            btnSapUdfFirst.Location = new Point(369, 178);
            btnSapUdfFirst.Name = "btnSapUdfFirst";
            btnSapUdfFirst.Size = new Size(27, 24);
            btnSapUdfFirst.TabIndex = 2;
            btnSapUdfFirst.Text = "|<";
            // 
            // btnSapUdfPrevious
            // 
            btnSapUdfPrevious.Location = new Point(401, 178);
            btnSapUdfPrevious.Name = "btnSapUdfPrevious";
            btnSapUdfPrevious.Size = new Size(27, 24);
            btnSapUdfPrevious.TabIndex = 3;
            btnSapUdfPrevious.Text = "<";
            // 
            // lueSapUdfPageSize
            // 
            lueSapUdfPageSize.EditValue = "20";
            lueSapUdfPageSize.Location = new Point(446, 178);
            lueSapUdfPageSize.Name = "lueSapUdfPageSize";
            lueSapUdfPageSize.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueSapUdfPageSize.Properties.DataSource = new string[]
    {
    "20",
    "50",
    "100"
    };
            lueSapUdfPageSize.Properties.NullText = "";
            lueSapUdfPageSize.Size = new Size(47, 20);
            lueSapUdfPageSize.TabIndex = 4;
            // 
            // lblSapUdfPageInfo
            // 
            lblSapUdfPageInfo.Appearance.ForeColor = Color.Black;
            lblSapUdfPageInfo.Appearance.Options.UseForeColor = true;
            lblSapUdfPageInfo.Location = new Point(501, 182);
            lblSapUdfPageInfo.Name = "lblSapUdfPageInfo";
            lblSapUdfPageInfo.Size = new Size(129, 13);
            lblSapUdfPageInfo.TabIndex = 5;
            lblSapUdfPageInfo.Text = "de 1    registros por página";
            // 
            // btnSapUdfNext
            // 
            btnSapUdfNext.Location = new Point(669, 178);
            btnSapUdfNext.Name = "btnSapUdfNext";
            btnSapUdfNext.Size = new Size(27, 24);
            btnSapUdfNext.TabIndex = 6;
            btnSapUdfNext.Text = ">";
            // 
            // btnSapUdfLast
            // 
            btnSapUdfLast.Location = new Point(701, 178);
            btnSapUdfLast.Name = "btnSapUdfLast";
            btnSapUdfLast.Size = new Size(27, 24);
            btnSapUdfLast.TabIndex = 7;
            btnSapUdfLast.Text = ">|";
            // 
            // grpHeader
            // 
            grpHeader.AppearanceCaption.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpHeader.AppearanceCaption.ForeColor = Color.FromArgb(0, 70, 170);
            grpHeader.AppearanceCaption.Options.UseFont = true;
            grpHeader.AppearanceCaption.Options.UseForeColor = true;
            grpHeader.Controls.Add(lblItemCode);
            grpHeader.Controls.Add(txtItemCode);
            grpHeader.Controls.Add(lblBarCode);
            grpHeader.Controls.Add(txtBarCode);
            grpHeader.Controls.Add(lblDescription);
            grpHeader.Controls.Add(txtDescription);
            grpHeader.Controls.Add(lblCommercialName);
            grpHeader.Controls.Add(txtCommercialName);
            grpHeader.Controls.Add(lblItemGroup);
            grpHeader.Controls.Add(sleItemGroup);
            grpHeader.Controls.Add(lblBrand);
            grpHeader.Controls.Add(sleBrand);
            grpHeader.Controls.Add(lblLine);
            grpHeader.Controls.Add(sleLine);
            grpHeader.Controls.Add(lblUom);
            grpHeader.Controls.Add(sleHeaderUom);
            grpHeader.Controls.Add(lblItemType);
            grpHeader.Controls.Add(lueItemType);
            grpHeader.Controls.Add(lblStatus);
            grpHeader.Controls.Add(lueStatus);
            grpHeader.Controls.Add(chkInventoryItem);
            grpHeader.Controls.Add(chkPurchaseItem);
            grpHeader.Controls.Add(chkSalesItem);
            grpHeader.Controls.Add(picItem);
            grpHeader.Controls.Add(btnChangeImage);
            grpHeader.Controls.Add(btnRemoveImage);
            grpHeader.Dock = DockStyle.Top;
            grpHeader.Location = new Point(0, 0);
            grpHeader.Name = "grpHeader";
            grpHeader.Size = new Size(1090, 217);
            grpHeader.TabIndex = 0;
            grpHeader.Text = "Datos principales del item";
            // 
            // lblItemCode
            // 
            lblItemCode.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblItemCode.Appearance.ForeColor = Color.Black;
            lblItemCode.Appearance.Options.UseFont = true;
            lblItemCode.Appearance.Options.UseForeColor = true;
            lblItemCode.Location = new Point(19, 36);
            lblItemCode.Name = "lblItemCode";
            lblItemCode.Size = new Size(71, 15);
            lblItemCode.TabIndex = 0;
            lblItemCode.Text = "Código Ítem:";
            // 
            // txtItemCode
            // 
            txtItemCode.EditValue = "A000001";
            txtItemCode.Location = new Point(146, 33);
            txtItemCode.Name = "txtItemCode";
            txtItemCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            txtItemCode.Properties.Appearance.Options.UseFont = true;
            txtItemCode.Size = new Size(176, 22);
            txtItemCode.TabIndex = 1;
            // 
            // lblBarCode
            // 
            lblBarCode.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblBarCode.Appearance.ForeColor = Color.Black;
            lblBarCode.Appearance.Options.UseFont = true;
            lblBarCode.Appearance.Options.UseForeColor = true;
            lblBarCode.Location = new Point(351, 36);
            lblBarCode.Name = "lblBarCode";
            lblBarCode.Size = new Size(79, 15);
            lblBarCode.TabIndex = 2;
            lblBarCode.Text = "Código Barras:";
            // 
            // txtBarCode
            // 
            txtBarCode.EditValue = "7861234567890";
            txtBarCode.Location = new Point(454, 33);
            txtBarCode.Name = "txtBarCode";
            txtBarCode.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            txtBarCode.Properties.Appearance.Options.UseFont = true;
            txtBarCode.Size = new Size(223, 22);
            txtBarCode.TabIndex = 3;
            // 
            // lblDescription
            // 
            lblDescription.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDescription.Appearance.ForeColor = Color.Black;
            lblDescription.Appearance.Options.UseFont = true;
            lblDescription.Appearance.Options.UseForeColor = true;
            lblDescription.Location = new Point(19, 64);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(68, 15);
            lblDescription.TabIndex = 4;
            lblDescription.Text = "Descripción:";
            // 
            // txtDescription
            // 
            txtDescription.EditValue = "ARROZ FLOR 2KG";
            txtDescription.Location = new Point(146, 61);
            txtDescription.Name = "txtDescription";
            txtDescription.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            txtDescription.Properties.Appearance.Options.UseFont = true;
            txtDescription.Size = new Size(531, 22);
            txtDescription.TabIndex = 5;
            // 
            // lblCommercialName
            // 
            lblCommercialName.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCommercialName.Appearance.ForeColor = Color.Black;
            lblCommercialName.Appearance.Options.UseFont = true;
            lblCommercialName.Appearance.Options.UseForeColor = true;
            lblCommercialName.Location = new Point(19, 92);
            lblCommercialName.Name = "lblCommercialName";
            lblCommercialName.Size = new Size(107, 15);
            lblCommercialName.TabIndex = 6;
            lblCommercialName.Text = "Nombre Comercial:";
            // 
            // txtCommercialName
            // 
            txtCommercialName.EditValue = "ARROZ FLOR PREMIUM 2 KILOS";
            txtCommercialName.Location = new Point(146, 88);
            txtCommercialName.Name = "txtCommercialName";
            txtCommercialName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            txtCommercialName.Properties.Appearance.Options.UseFont = true;
            txtCommercialName.Size = new Size(531, 22);
            txtCommercialName.TabIndex = 7;
            // 
            // lblItemGroup
            // 
            lblItemGroup.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblItemGroup.Appearance.ForeColor = Color.Black;
            lblItemGroup.Appearance.Options.UseFont = true;
            lblItemGroup.Appearance.Options.UseForeColor = true;
            lblItemGroup.Location = new Point(19, 120);
            lblItemGroup.Name = "lblItemGroup";
            lblItemGroup.Size = new Size(107, 15);
            lblItemGroup.TabIndex = 8;
            lblItemGroup.Text = "Grupo de Artículos:";
            // 
            // sleItemGroup
            // 
            sleItemGroup.EditValue = "01 - PRODUCTOS PRIMERA NECESIDAD";
            sleItemGroup.Location = new Point(146, 116);
            sleItemGroup.Name = "sleItemGroup";
            sleItemGroup.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            sleItemGroup.Properties.Appearance.Options.UseFont = true;
            sleItemGroup.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleItemGroup.Properties.DisplayMember = "DisplayText";
            sleItemGroup.Properties.NullText = "";
            sleItemGroup.Properties.PopupView = grvItemGroupLookup;
            sleItemGroup.Properties.ValueMember = "Id";
            sleItemGroup.Size = new Size(264, 22);
            sleItemGroup.TabIndex = 9;
            // 
            // grvItemGroupLookup
            // 
            grvItemGroupLookup.DetailHeight = 303;
            grvItemGroupLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvItemGroupLookup.Name = "grvItemGroupLookup";
            grvItemGroupLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvItemGroupLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvItemGroupLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblBrand
            // 
            lblBrand.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblBrand.Appearance.ForeColor = Color.Black;
            lblBrand.Appearance.Options.UseFont = true;
            lblBrand.Appearance.Options.UseForeColor = true;
            lblBrand.Location = new Point(19, 147);
            lblBrand.Name = "lblBrand";
            lblBrand.Size = new Size(37, 15);
            lblBrand.TabIndex = 10;
            lblBrand.Text = "Marca:";
            // 
            // sleBrand
            // 
            sleBrand.EditValue = "FLOR";
            sleBrand.Location = new Point(146, 144);
            sleBrand.Name = "sleBrand";
            sleBrand.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            sleBrand.Properties.Appearance.Options.UseFont = true;
            sleBrand.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleBrand.Properties.DisplayMember = "DisplayText";
            sleBrand.Properties.NullText = "";
            sleBrand.Properties.PopupView = grvBrandLookup;
            sleBrand.Properties.ValueMember = "Id";
            sleBrand.Size = new Size(264, 22);
            sleBrand.TabIndex = 11;
            // 
            // grvBrandLookup
            // 
            grvBrandLookup.DetailHeight = 303;
            grvBrandLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvBrandLookup.Name = "grvBrandLookup";
            grvBrandLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvBrandLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvBrandLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblLine
            // 
            lblLine.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblLine.Appearance.ForeColor = Color.Black;
            lblLine.Appearance.Options.UseFont = true;
            lblLine.Appearance.Options.UseForeColor = true;
            lblLine.Location = new Point(19, 175);
            lblLine.Name = "lblLine";
            lblLine.Size = new Size(81, 15);
            lblLine.TabIndex = 12;
            lblLine.Text = "Línea / Familia:";
            // 
            // sleLine
            // 
            sleLine.EditValue = "ALIMENTOS";
            sleLine.Location = new Point(146, 172);
            sleLine.Name = "sleLine";
            sleLine.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            sleLine.Properties.Appearance.Options.UseFont = true;
            sleLine.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleLine.Properties.DisplayMember = "DisplayText";
            sleLine.Properties.NullText = "";
            sleLine.Properties.PopupView = grvLineLookup;
            sleLine.Properties.ValueMember = "Id";
            sleLine.Size = new Size(264, 22);
            sleLine.TabIndex = 13;
            // 
            // grvLineLookup
            // 
            grvLineLookup.DetailHeight = 303;
            grvLineLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvLineLookup.Name = "grvLineLookup";
            grvLineLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvLineLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvLineLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblUom
            // 
            lblUom.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUom.Appearance.ForeColor = Color.Black;
            lblUom.Appearance.Options.UseFont = true;
            lblUom.Appearance.Options.UseForeColor = true;
            lblUom.Location = new Point(419, 120);
            lblUom.Name = "lblUom";
            lblUom.Size = new Size(103, 15);
            lblUom.TabIndex = 14;
            lblUom.Text = "Unidad de Medida:";
            // 
            // sleHeaderUom
            // 
            sleHeaderUom.EditValue = "UNIDAD";
            sleHeaderUom.Location = new Point(533, 116);
            sleHeaderUom.Name = "sleHeaderUom";
            sleHeaderUom.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            sleHeaderUom.Properties.Appearance.Options.UseFont = true;
            sleHeaderUom.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            sleHeaderUom.Properties.DisplayMember = "DisplayText";
            sleHeaderUom.Properties.NullText = "";
            sleHeaderUom.Properties.PopupView = grvHeaderUomLookup;
            sleHeaderUom.Properties.ValueMember = "Id";
            sleHeaderUom.Size = new Size(144, 22);
            sleHeaderUom.TabIndex = 15;
            // 
            // grvHeaderUomLookup
            // 
            grvHeaderUomLookup.DetailHeight = 303;
            grvHeaderUomLookup.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            grvHeaderUomLookup.Name = "grvHeaderUomLookup";
            grvHeaderUomLookup.OptionsEditForm.PopupEditFormWidth = 686;
            grvHeaderUomLookup.OptionsSelection.EnableAppearanceFocusedCell = false;
            grvHeaderUomLookup.OptionsView.ShowGroupPanel = false;
            // 
            // lblItemType
            // 
            lblItemType.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblItemType.Appearance.ForeColor = Color.Black;
            lblItemType.Appearance.Options.UseFont = true;
            lblItemType.Appearance.Options.UseForeColor = true;
            lblItemType.Location = new Point(419, 147);
            lblItemType.Name = "lblItemType";
            lblItemType.Size = new Size(74, 15);
            lblItemType.TabIndex = 16;
            lblItemType.Text = "Tipo de Ítem:";
            // 
            // lueItemType
            // 
            lueItemType.EditValue = "Inventariable";
            lueItemType.Location = new Point(533, 144);
            lueItemType.Name = "lueItemType";
            lueItemType.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            lueItemType.Properties.Appearance.Options.UseFont = true;
            lueItemType.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueItemType.Properties.DataSource = new string[]
    {
    "Product",
    "Service",
    "FixedAsset"
    };
            lueItemType.Properties.NullText = "";
            lueItemType.Size = new Size(144, 22);
            lueItemType.TabIndex = 17;
            // 
            // lblStatus
            // 
            lblStatus.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.Appearance.ForeColor = Color.Black;
            lblStatus.Appearance.Options.UseFont = true;
            lblStatus.Appearance.Options.UseForeColor = true;
            lblStatus.Location = new Point(727, 36);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(39, 15);
            lblStatus.TabIndex = 18;
            lblStatus.Text = "Estado:";
            // 
            // lueStatus
            // 
            lueStatus.EditValue = "Activo";
            lueStatus.Location = new Point(772, 33);
            lueStatus.Name = "lueStatus";
            lueStatus.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            lueStatus.Properties.Appearance.Options.UseFont = true;
            lueStatus.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
            lueStatus.Properties.DataSource = new string[]
    {
    "Activo",
    "Inactivo",
    "Bloqueado"
    };
            lueStatus.Properties.NullText = "";
            lueStatus.Size = new Size(129, 22);
            lueStatus.TabIndex = 19;
            // 
            // chkInventoryItem
            // 
            chkInventoryItem.EditValue = true;
            chkInventoryItem.Location = new Point(727, 87);
            chkInventoryItem.Name = "chkInventoryItem";
            chkInventoryItem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            chkInventoryItem.Properties.Appearance.Options.UseFont = true;
            chkInventoryItem.Properties.Caption = "Artículo de inventario";
            chkInventoryItem.Size = new Size(148, 20);
            chkInventoryItem.TabIndex = 20;
            // 
            // chkPurchaseItem
            // 
            chkPurchaseItem.EditValue = true;
            chkPurchaseItem.Location = new Point(727, 115);
            chkPurchaseItem.Name = "chkPurchaseItem";
            chkPurchaseItem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            chkPurchaseItem.Properties.Appearance.Options.UseFont = true;
            chkPurchaseItem.Properties.Caption = "Artículo de compra";
            chkPurchaseItem.Size = new Size(148, 20);
            chkPurchaseItem.TabIndex = 21;
            // 
            // chkSalesItem
            // 
            chkSalesItem.EditValue = true;
            chkSalesItem.Location = new Point(727, 145);
            chkSalesItem.Name = "chkSalesItem";
            chkSalesItem.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            chkSalesItem.Properties.Appearance.Options.UseFont = true;
            chkSalesItem.Properties.Caption = "Artículo de venta";
            chkSalesItem.Size = new Size(148, 20);
            chkSalesItem.TabIndex = 22;
            // 
            // picItem
            // 
            picItem.Location = new Point(941, 32);
            picItem.Name = "picItem";
            picItem.Properties.NullText = "Imagen";
            picItem.Properties.ShowCameraMenuItem = CameraMenuItemVisibility.Auto;
            picItem.Properties.SizeMode = PictureSizeMode.Zoom;
            picItem.Size = new Size(144, 130);
            picItem.TabIndex = 24;
            // 
            // btnChangeImage
            // 
            btnChangeImage.Appearance.Font = new Font("Segoe UI", 9F);
            btnChangeImage.Appearance.Options.UseFont = true;
            btnChangeImage.Location = new Point(909, 170);
            btnChangeImage.Name = "btnChangeImage";
            btnChangeImage.Size = new Size(111, 24);
            btnChangeImage.TabIndex = 25;
            btnChangeImage.Text = "Cambiar Imagen";
            // 
            // btnRemoveImage
            // 
            btnRemoveImage.Appearance.Font = new Font("Segoe UI", 9F);
            btnRemoveImage.Appearance.Options.UseFont = true;
            btnRemoveImage.Location = new Point(1025, 170);
            btnRemoveImage.Name = "btnRemoveImage";
            btnRemoveImage.Size = new Size(60, 24);
            btnRemoveImage.TabIndex = 26;
            btnRemoveImage.Text = "Quitar";
            // 
            // pnlSummary
            // 
            pnlSummary.Controls.Add(lblSummaryTitle);
            pnlSummary.Controls.Add(lblSummaryStatusTitle);
            pnlSummary.Controls.Add(lblSummaryStatus);
            pnlSummary.Controls.Add(lblSummaryStockTitle);
            pnlSummary.Controls.Add(lblSummaryStock);
            pnlSummary.Controls.Add(lblSummaryStockUnit);
            pnlSummary.Controls.Add(lblSummaryCostTitle);
            pnlSummary.Controls.Add(lblSummaryCost);
            pnlSummary.Controls.Add(lblSummaryPriceTitle);
            pnlSummary.Controls.Add(lblSummaryPrice);
            pnlSummary.Controls.Add(lblSummaryMarginTitle);
            pnlSummary.Controls.Add(lblSummaryMargin);
            pnlSummary.Controls.Add(lblSummarySapTitle);
            pnlSummary.Controls.Add(lblSummarySap);
            pnlSummary.Controls.Add(lblSummarySyncTitle);
            pnlSummary.Controls.Add(lblSummarySync);
            pnlSummary.Controls.Add(lblSummarySapDbTitle);
            pnlSummary.Controls.Add(lblSummarySapDb);
            pnlSummary.Controls.Add(memQuickNotes);
            pnlSummary.Dock = DockStyle.Fill;
            pnlSummary.Location = new Point(0, 0);
            pnlSummary.Name = "pnlSummary";
            pnlSummary.Size = new Size(182, 697);
            pnlSummary.TabIndex = 0;
            // 
            // lblSummaryTitle
            // 
            lblSummaryTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummaryTitle.Appearance.ForeColor = Color.Black;
            lblSummaryTitle.Appearance.Options.UseFont = true;
            lblSummaryTitle.Appearance.Options.UseForeColor = true;
            lblSummaryTitle.Location = new Point(15, 16);
            lblSummaryTitle.Name = "lblSummaryTitle";
            lblSummaryTitle.Size = new Size(102, 15);
            lblSummaryTitle.TabIndex = 0;
            lblSummaryTitle.Text = "Resumen del Ítem";
            // 
            // lblSummaryStatusTitle
            // 
            lblSummaryStatusTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummaryStatusTitle.Appearance.ForeColor = Color.Black;
            lblSummaryStatusTitle.Appearance.Options.UseFont = true;
            lblSummaryStatusTitle.Appearance.Options.UseForeColor = true;
            lblSummaryStatusTitle.Location = new Point(15, 50);
            lblSummaryStatusTitle.Name = "lblSummaryStatusTitle";
            lblSummaryStatusTitle.Size = new Size(86, 15);
            lblSummaryStatusTitle.TabIndex = 1;
            lblSummaryStatusTitle.Text = "Estado del Ítem";
            // 
            // lblSummaryStatus
            // 
            lblSummaryStatus.Appearance.ForeColor = Color.Black;
            lblSummaryStatus.Appearance.Options.UseForeColor = true;
            lblSummaryStatus.Location = new Point(15, 69);
            lblSummaryStatus.Name = "lblSummaryStatus";
            lblSummaryStatus.Size = new Size(39, 13);
            lblSummaryStatus.TabIndex = 2;
            lblSummaryStatus.Text = "* Activo";
            // 
            // lblSummaryStockTitle
            // 
            lblSummaryStockTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummaryStockTitle.Appearance.ForeColor = Color.Black;
            lblSummaryStockTitle.Appearance.Options.UseFont = true;
            lblSummaryStockTitle.Appearance.Options.UseForeColor = true;
            lblSummaryStockTitle.Location = new Point(15, 97);
            lblSummaryStockTitle.Name = "lblSummaryStockTitle";
            lblSummaryStockTitle.Size = new Size(93, 15);
            lblSummaryStockTitle.TabIndex = 3;
            lblSummaryStockTitle.Text = "Stock Disponible";
            // 
            // lblSummaryStock
            // 
            lblSummaryStock.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummaryStock.Appearance.ForeColor = Color.Black;
            lblSummaryStock.Appearance.Options.UseFont = true;
            lblSummaryStock.Appearance.Options.UseForeColor = true;
            lblSummaryStock.Location = new Point(15, 114);
            lblSummaryStock.Name = "lblSummaryStock";
            lblSummaryStock.Size = new Size(48, 15);
            lblSummaryStock.TabIndex = 4;
            lblSummaryStock.Text = "1,250.00";
            // 
            // lblSummaryStockUnit
            // 
            lblSummaryStockUnit.Appearance.ForeColor = Color.Black;
            lblSummaryStockUnit.Appearance.Options.UseForeColor = true;
            lblSummaryStockUnit.Location = new Point(194, 123);
            lblSummaryStockUnit.Name = "lblSummaryStockUnit";
            lblSummaryStockUnit.Size = new Size(14, 13);
            lblSummaryStockUnit.TabIndex = 5;
            lblSummaryStockUnit.Text = "UN";
            // 
            // lblSummaryCostTitle
            // 
            lblSummaryCostTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummaryCostTitle.Appearance.ForeColor = Color.Black;
            lblSummaryCostTitle.Appearance.Options.UseFont = true;
            lblSummaryCostTitle.Appearance.Options.UseForeColor = true;
            lblSummaryCostTitle.Location = new Point(15, 151);
            lblSummaryCostTitle.Name = "lblSummaryCostTitle";
            lblSummaryCostTitle.Size = new Size(88, 15);
            lblSummaryCostTitle.TabIndex = 6;
            lblSummaryCostTitle.Text = "Costo Promedio";
            // 
            // lblSummaryCost
            // 
            lblSummaryCost.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummaryCost.Appearance.ForeColor = Color.Black;
            lblSummaryCost.Appearance.Options.UseFont = true;
            lblSummaryCost.Appearance.Options.UseForeColor = true;
            lblSummaryCost.Location = new Point(15, 168);
            lblSummaryCost.Name = "lblSummaryCost";
            lblSummaryCost.Size = new Size(34, 15);
            lblSummaryCost.TabIndex = 7;
            lblSummaryCost.Text = "$ 1.25";
            // 
            // lblSummaryPriceTitle
            // 
            lblSummaryPriceTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummaryPriceTitle.Appearance.ForeColor = Color.Black;
            lblSummaryPriceTitle.Appearance.Options.UseFont = true;
            lblSummaryPriceTitle.Appearance.Options.UseForeColor = true;
            lblSummaryPriceTitle.Location = new Point(15, 201);
            lblSummaryPriceTitle.Name = "lblSummaryPriceTitle";
            lblSummaryPriceTitle.Size = new Size(64, 15);
            lblSummaryPriceTitle.TabIndex = 8;
            lblSummaryPriceTitle.Text = "Precio Base";
            // 
            // lblSummaryPrice
            // 
            lblSummaryPrice.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummaryPrice.Appearance.ForeColor = Color.Black;
            lblSummaryPrice.Appearance.Options.UseFont = true;
            lblSummaryPrice.Appearance.Options.UseForeColor = true;
            lblSummaryPrice.Location = new Point(15, 218);
            lblSummaryPrice.Name = "lblSummaryPrice";
            lblSummaryPrice.Size = new Size(34, 15);
            lblSummaryPrice.TabIndex = 9;
            lblSummaryPrice.Text = "$ 1.60";
            // 
            // lblSummaryMarginTitle
            // 
            lblSummaryMarginTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummaryMarginTitle.Appearance.ForeColor = Color.Black;
            lblSummaryMarginTitle.Appearance.Options.UseFont = true;
            lblSummaryMarginTitle.Appearance.Options.UseForeColor = true;
            lblSummaryMarginTitle.Location = new Point(15, 251);
            lblSummaryMarginTitle.Name = "lblSummaryMarginTitle";
            lblSummaryMarginTitle.Size = new Size(96, 15);
            lblSummaryMarginTitle.TabIndex = 10;
            lblSummaryMarginTitle.Text = "Margen Estimado";
            // 
            // lblSummaryMargin
            // 
            lblSummaryMargin.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummaryMargin.Appearance.ForeColor = Color.Black;
            lblSummaryMargin.Appearance.Options.UseFont = true;
            lblSummaryMargin.Appearance.Options.UseForeColor = true;
            lblSummaryMargin.Location = new Point(15, 269);
            lblSummaryMargin.Name = "lblSummaryMargin";
            lblSummaryMargin.Size = new Size(44, 15);
            lblSummaryMargin.TabIndex = 11;
            lblSummaryMargin.Text = "28.00 %";
            // 
            // lblSummarySapTitle
            // 
            lblSummarySapTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummarySapTitle.Appearance.ForeColor = Color.Black;
            lblSummarySapTitle.Appearance.Options.UseFont = true;
            lblSummarySapTitle.Appearance.Options.UseForeColor = true;
            lblSummarySapTitle.Location = new Point(15, 303);
            lblSummarySapTitle.Name = "lblSummarySapTitle";
            lblSummarySapTitle.Size = new Size(61, 15);
            lblSummarySapTitle.TabIndex = 12;
            lblSummarySapTitle.Text = "Estado SAP";
            // 
            // lblSummarySap
            // 
            lblSummarySap.Appearance.ForeColor = Color.Black;
            lblSummarySap.Appearance.Options.UseForeColor = true;
            lblSummarySap.Location = new Point(15, 322);
            lblSummarySap.Name = "lblSummarySap";
            lblSummarySap.Size = new Size(69, 13);
            lblSummarySap.TabIndex = 13;
            lblSummarySap.Text = "* Sincronizado";
            // 
            // lblSummarySyncTitle
            // 
            lblSummarySyncTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummarySyncTitle.Appearance.ForeColor = Color.Black;
            lblSummarySyncTitle.Appearance.Options.UseFont = true;
            lblSummarySyncTitle.Appearance.Options.UseForeColor = true;
            lblSummarySyncTitle.Location = new Point(15, 348);
            lblSummarySyncTitle.Name = "lblSummarySyncTitle";
            lblSummarySyncTitle.Size = new Size(120, 15);
            lblSummarySyncTitle.TabIndex = 14;
            lblSummarySyncTitle.Text = "Última Sincronización";
            // 
            // lblSummarySync
            // 
            lblSummarySync.Appearance.ForeColor = Color.Black;
            lblSummarySync.Appearance.Options.UseForeColor = true;
            lblSummarySync.Location = new Point(15, 367);
            lblSummarySync.Name = "lblSummarySync";
            lblSummarySync.Size = new Size(103, 13);
            lblSummarySync.TabIndex = 15;
            lblSummarySync.Text = "10/05/2026 09:30:15";
            // 
            // lblSummarySapDbTitle
            // 
            lblSummarySapDbTitle.Appearance.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSummarySapDbTitle.Appearance.ForeColor = Color.Black;
            lblSummarySapDbTitle.Appearance.Options.UseFont = true;
            lblSummarySapDbTitle.Appearance.Options.UseForeColor = true;
            lblSummarySapDbTitle.Location = new Point(15, 393);
            lblSummarySapDbTitle.Name = "lblSummarySapDbTitle";
            lblSummarySapDbTitle.Size = new Size(97, 15);
            lblSummarySapDbTitle.TabIndex = 16;
            lblSummarySapDbTitle.Text = "Base SAP Destino";
            // 
            // lblSummarySapDb
            // 
            lblSummarySapDb.Appearance.ForeColor = Color.Black;
            lblSummarySapDb.Appearance.Options.UseForeColor = true;
            lblSummarySapDb.Location = new Point(15, 413);
            lblSummarySapDb.Name = "lblSummarySapDb";
            lblSummarySapDb.Size = new Size(62, 13);
            lblSummarySapDb.TabIndex = 17;
            lblSummarySapDb.Text = "SBODEMOUS";
            // 
            // memQuickNotes
            // 
            memQuickNotes.EditValue = "Item de alta rotacion.\r\nProveedor preferido entrega\r\n2 veces por semana.";
            memQuickNotes.Location = new Point(15, 438);
            memQuickNotes.Name = "memQuickNotes";
            memQuickNotes.Properties.Appearance.BackColor = Color.FromArgb(255, 246, 205);
            memQuickNotes.Properties.Appearance.Font = new Font("Segoe UI", 9F);
            memQuickNotes.Properties.Appearance.Options.UseBackColor = true;
            memQuickNotes.Properties.Appearance.Options.UseFont = true;
            memQuickNotes.Properties.ReadOnly = true;
            memQuickNotes.Size = new Size(204, 36);
            memQuickNotes.TabIndex = 18;
            // 
            // pnlFooter
            // 
            pnlFooter.Controls.Add(lblFooterRecord);
            pnlFooter.Controls.Add(btnFirst);
            pnlFooter.Controls.Add(btnPrevious);
            pnlFooter.Controls.Add(btnNext);
            pnlFooter.Controls.Add(btnLast);
            pnlFooter.Controls.Add(lblFooterMode);
            pnlFooter.Controls.Add(lblFooterCreated);
            pnlFooter.Controls.Add(lblFooterModified);
            pnlFooter.Controls.Add(lblFooterDatabase);
            pnlFooter.Controls.Add(btnSave);
            pnlFooter.Controls.Add(btnCancel);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(2, 699);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(1282, 36);
            pnlFooter.TabIndex = 2;
            // 
            // lblFooterRecord
            // 
            lblFooterRecord.Appearance.ForeColor = Color.Black;
            lblFooterRecord.Appearance.Options.UseForeColor = true;
            lblFooterRecord.Location = new Point(17, 12);
            lblFooterRecord.Name = "lblFooterRecord";
            lblFooterRecord.Size = new Size(95, 13);
            lblFooterRecord.TabIndex = 0;
            lblFooterRecord.Text = "Registro: 1 de 2458";
            // 
            // btnFirst
            // 
            btnFirst.Location = new Point(129, 7);
            btnFirst.Name = "btnFirst";
            btnFirst.Size = new Size(27, 23);
            btnFirst.TabIndex = 1;
            btnFirst.Text = "|<";
            // 
            // btnPrevious
            // 
            btnPrevious.Location = new Point(161, 7);
            btnPrevious.Name = "btnPrevious";
            btnPrevious.Size = new Size(27, 23);
            btnPrevious.TabIndex = 2;
            btnPrevious.Text = "<";
            // 
            // btnNext
            // 
            btnNext.Location = new Point(194, 7);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(27, 23);
            btnNext.TabIndex = 3;
            btnNext.Text = ">";
            // 
            // btnLast
            // 
            btnLast.Location = new Point(226, 7);
            btnLast.Name = "btnLast";
            btnLast.Size = new Size(27, 23);
            btnLast.TabIndex = 4;
            btnLast.Text = ">|";
            // 
            // lblFooterMode
            // 
            lblFooterMode.Appearance.BackColor = Color.FromArgb(0, 86, 210);
            lblFooterMode.Appearance.ForeColor = Color.Black;
            lblFooterMode.Appearance.Options.UseBackColor = true;
            lblFooterMode.Appearance.Options.UseForeColor = true;
            lblFooterMode.Appearance.Options.UseTextOptions = true;
            lblFooterMode.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            lblFooterMode.AutoSizeMode = LabelAutoSizeMode.None;
            lblFooterMode.Location = new Point(283, 8);
            lblFooterMode.Name = "lblFooterMode";
            lblFooterMode.Size = new Size(69, 21);
            lblFooterMode.TabIndex = 5;
            lblFooterMode.Text = "Editando";
            // 
            // lblFooterCreated
            // 
            lblFooterCreated.Appearance.ForeColor = Color.Black;
            lblFooterCreated.Appearance.Options.UseForeColor = true;
            lblFooterCreated.Location = new Point(390, 12);
            lblFooterCreated.Name = "lblFooterCreated";
            lblFooterCreated.Size = new Size(285, 13);
            lblFooterCreated.TabIndex = 6;
            lblFooterCreated.Text = "Creado por: admin    Fecha Creación: 10/05/2026 08:15:33";
            // 
            // lblFooterModified
            // 
            lblFooterModified.Appearance.ForeColor = Color.Black;
            lblFooterModified.Appearance.Options.UseForeColor = true;
            lblFooterModified.Location = new Point(699, 12);
            lblFooterModified.Name = "lblFooterModified";
            lblFooterModified.Size = new Size(317, 13);
            lblFooterModified.TabIndex = 7;
            lblFooterModified.Text = "Modificado por: admin    Fecha Modificación: 10/05/2026 09:25:11";
            // 
            // lblFooterDatabase
            // 
            lblFooterDatabase.Appearance.ForeColor = Color.Black;
            lblFooterDatabase.Appearance.Options.UseForeColor = true;
            lblFooterDatabase.Location = new Point(909, 12);
            lblFooterDatabase.Name = "lblFooterDatabase";
            lblFooterDatabase.Size = new Size(149, 13);
            lblFooterDatabase.TabIndex = 8;
            lblFooterDatabase.Text = "Base de Datos: NuanSystemDB";
            // 
            // btnSave
            // 
            btnSave.Appearance.BackColor = Color.FromArgb(0, 86, 210);
            btnSave.Appearance.ForeColor = Color.White;
            btnSave.Appearance.Options.UseBackColor = true;
            btnSave.Appearance.Options.UseForeColor = true;
            btnSave.Location = new Point(1063, 6);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(77, 24);
            btnSave.TabIndex = 9;
            btnSave.Text = "Guardar";
            btnSave.Click += SaveButtonClick;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(1149, 6);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(77, 24);
            btnCancel.TabIndex = 10;
            btnCancel.Text = "Cancelar";
            // 
            // ItemEditForm
            // 
            AcceptButton = btnSave;
            Appearance.Options.UseFont = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(1286, 737);
            Controls.Add(pnlMain);
            Font = new Font("Segoe UI", 9F);
            Name = "ItemEditForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "NuanSystem ERP - Maestro de Items";
            WindowState = FormWindowState.Maximized;
            ((ISupportInitialize)pnlMain).EndInit();
            pnlMain.ResumeLayout(false);
            ((ISupportInitialize)splContent.Panel1).EndInit();
            splContent.Panel1.ResumeLayout(false);
            ((ISupportInitialize)splContent.Panel2).EndInit();
            splContent.Panel2.ResumeLayout(false);
            ((ISupportInitialize)splContent).EndInit();
            splContent.ResumeLayout(false);
            ((ISupportInitialize)xtcMain).EndInit();
            xtcMain.ResumeLayout(false);
            xtpGeneral.ResumeLayout(false);
            ((ISupportInitialize)grpGeneralData).EndInit();
            grpGeneralData.ResumeLayout(false);
            grpGeneralData.PerformLayout();
            ((ISupportInitialize)memLongDescription.Properties).EndInit();
            ((ISupportInitialize)sleCategory.Properties).EndInit();
            ((ISupportInitialize)grvCategoryLookup).EndInit();
            ((ISupportInitialize)sleSubCategory.Properties).EndInit();
            ((ISupportInitialize)grvSubCategoryLookup).EndInit();
            ((ISupportInitialize)sleManufacturer.Properties).EndInit();
            ((ISupportInitialize)grvManufacturerLookup).EndInit();
            ((ISupportInitialize)txtModel.Properties).EndInit();
            ((ISupportInitialize)lueCountry.Properties).EndInit();
            ((ISupportInitialize)txtAlternateCode.Properties).EndInit();
            ((ISupportInitialize)sedWeight.Properties).EndInit();
            ((ISupportInitialize)sedVolume.Properties).EndInit();
            ((ISupportInitialize)slePurchaseUom.Properties).EndInit();
            ((ISupportInitialize)grvPurchaseUomLookup).EndInit();
            ((ISupportInitialize)sleSalesUom.Properties).EndInit();
            ((ISupportInitialize)grvSalesUomLookup).EndInit();
            ((ISupportInitialize)sleInventoryUom.Properties).EndInit();
            ((ISupportInitialize)grvInventoryUomLookup).EndInit();
            ((ISupportInitialize)memGeneralNotes.Properties).EndInit();
            ((ISupportInitialize)grpAdditionalInfo).EndInit();
            grpAdditionalInfo.ResumeLayout(false);
            grpAdditionalInfo.PerformLayout();
            ((ISupportInitialize)txtSatCode.Properties).EndInit();
            ((ISupportInitialize)txtUnspscCode.Properties).EndInit();
            ((ISupportInitialize)txtTaxCode.Properties).EndInit();
            ((ISupportInitialize)lueTaxType.Properties).EndInit();
            ((ISupportInitialize)lueBaseCurrency.Properties).EndInit();
            ((ISupportInitialize)lueDefaultPriceList.Properties).EndInit();
            ((ISupportInitialize)sedMaxDiscount.Properties).EndInit();
            ((ISupportInitialize)dtpLastChange.Properties.CalendarTimeProperties).EndInit();
            ((ISupportInitialize)dtpLastChange.Properties).EndInit();
            ((ISupportInitialize)txtLastChangeUser.Properties).EndInit();
            ((ISupportInitialize)grpAttributes).EndInit();
            grpAttributes.ResumeLayout(false);
            ((ISupportInitialize)grcAttributes).EndInit();
            ((ISupportInitialize)grvAttributes).EndInit();
            ((ISupportInitialize)grpAdditionalImages).EndInit();
            grpAdditionalImages.ResumeLayout(false);
            ((ISupportInitialize)picAdditionalImage1.Properties).EndInit();
            xtpInventory.ResumeLayout(false);
            ((ISupportInitialize)grpInventoryParams).EndInit();
            grpInventoryParams.ResumeLayout(false);
            grpInventoryParams.PerformLayout();
            ((ISupportInitialize)sleDefaultWarehouse.Properties).EndInit();
            ((ISupportInitialize)grvDefaultWarehouseLookup).EndInit();
            ((ISupportInitialize)lueValuationMethod.Properties).EndInit();
            ((ISupportInitialize)sleInventoryAccount.Properties).EndInit();
            ((ISupportInitialize)grvInventoryAccountLookup).EndInit();
            ((ISupportInitialize)sleCostAccount.Properties).EndInit();
            ((ISupportInitialize)grvCostAccountLookup).EndInit();
            ((ISupportInitialize)lueAdditionalCostHandling.Properties).EndInit();
            ((ISupportInitialize)chkBatch.Properties).EndInit();
            ((ISupportInitialize)chkSerial.Properties).EndInit();
            ((ISupportInitialize)chkBinLocation.Properties).EndInit();
            ((ISupportInitialize)grpStockControl).EndInit();
            grpStockControl.ResumeLayout(false);
            grpStockControl.PerformLayout();
            ((ISupportInitialize)sedMinStock.Properties).EndInit();
            ((ISupportInitialize)sedMaxStock.Properties).EndInit();
            ((ISupportInitialize)sedReorderPoint.Properties).EndInit();
            ((ISupportInitialize)cleCurrentStock.Properties).EndInit();
            ((ISupportInitialize)cleCommitted.Properties).EndInit();
            ((ISupportInitialize)cleOrdered.Properties).EndInit();
            ((ISupportInitialize)cleAvailable.Properties).EndInit();
            ((ISupportInitialize)grpStockByWarehouse).EndInit();
            grpStockByWarehouse.ResumeLayout(false);
            ((ISupportInitialize)btnSearchStock.Properties).EndInit();
            ((ISupportInitialize)grcStock).EndInit();
            ((ISupportInitialize)grvStock).EndInit();
            xtpPurchases.ResumeLayout(false);
            ((ISupportInitialize)grpPurchaseConfig).EndInit();
            grpPurchaseConfig.ResumeLayout(false);
            grpPurchaseConfig.PerformLayout();
            ((ISupportInitialize)slePreferredVendor.Properties).EndInit();
            ((ISupportInitialize)grvPreferredVendorLookup).EndInit();
            ((ISupportInitialize)txtVendorCode.Properties).EndInit();
            ((ISupportInitialize)luePurchaseUnit.Properties).EndInit();
            ((ISupportInitialize)sedMinPurchaseQty.Properties).EndInit();
            ((ISupportInitialize)sedDeliveryDays.Properties).EndInit();
            ((ISupportInitialize)cleLastPurchasePrice.Properties).EndInit();
            ((ISupportInitialize)luePurchaseCurrency.Properties).EndInit();
            ((ISupportInitialize)luePurchaseTax.Properties).EndInit();
            ((ISupportInitialize)slePurchaseAccount.Properties).EndInit();
            ((ISupportInitialize)grvPurchaseAccountLookup).EndInit();
            ((ISupportInitialize)sedVendorDiscount.Properties).EndInit();
            ((ISupportInitialize)sedRepositionDays.Properties).EndInit();
            ((ISupportInitialize)grpPreferredVendor).EndInit();
            grpPreferredVendor.ResumeLayout(false);
            grpPreferredVendor.PerformLayout();
            ((ISupportInitialize)grpAlternativeVendors).EndInit();
            grpAlternativeVendors.ResumeLayout(false);
            ((ISupportInitialize)grcVendors).EndInit();
            ((ISupportInitialize)grvVendors).EndInit();
            xtpSales.ResumeLayout(false);
            ((ISupportInitialize)grpSalesConfig).EndInit();
            grpSalesConfig.ResumeLayout(false);
            grpSalesConfig.PerformLayout();
            ((ISupportInitialize)lueSalesUnit.Properties).EndInit();
            ((ISupportInitialize)lueSalesTax.Properties).EndInit();
            ((ISupportInitialize)sleSalesAccount.Properties).EndInit();
            ((ISupportInitialize)grvSalesAccountLookup).EndInit();
            ((ISupportInitialize)lueSalesDefaultPriceList.Properties).EndInit();
            ((ISupportInitialize)lueSalesCurrency.Properties).EndInit();
            ((ISupportInitialize)memSalesNotes.Properties).EndInit();
            ((ISupportInitialize)grpSalesMargins).EndInit();
            grpSalesMargins.ResumeLayout(false);
            grpSalesMargins.PerformLayout();
            ((ISupportInitialize)cleBasePrice.Properties).EndInit();
            ((ISupportInitialize)sedSalesMaxDiscount.Properties).EndInit();
            ((ISupportInitialize)sedMinMargin.Properties).EndInit();
            ((ISupportInitialize)cleCurrentMargin.Properties).EndInit();
            ((ISupportInitialize)chkValidatePriceBelowCost.Properties).EndInit();
            ((ISupportInitialize)chkRequireDiscountAuthorization.Properties).EndInit();
            ((ISupportInitialize)grpPriceLists).EndInit();
            grpPriceLists.ResumeLayout(false);
            ((ISupportInitialize)btnSearchPriceList.Properties).EndInit();
            ((ISupportInitialize)grcPrices).EndInit();
            ((ISupportInitialize)grvPrices).EndInit();
            xtpCosts.ResumeLayout(false);
            ((ISupportInitialize)grpCostIndicators).EndInit();
            grpCostIndicators.ResumeLayout(false);
            grpCostIndicators.PerformLayout();
            ((ISupportInitialize)cleAverageCost.Properties).EndInit();
            ((ISupportInitialize)cleLastCost.Properties).EndInit();
            ((ISupportInitialize)cleStandardCost.Properties).EndInit();
            ((ISupportInitialize)cleReplacementCost.Properties).EndInit();
            ((ISupportInitialize)lueCostCurrency.Properties).EndInit();
            ((ISupportInitialize)grpProfitability).EndInit();
            grpProfitability.ResumeLayout(false);
            grpProfitability.PerformLayout();
            ((ISupportInitialize)cleProfitBasePrice.Properties).EndInit();
            ((ISupportInitialize)cleEstimatedMargin.Properties).EndInit();
            ((ISupportInitialize)cleEstimatedUtility.Properties).EndInit();
            ((ISupportInitialize)cleMarkup.Properties).EndInit();
            ((ISupportInitialize)cleProfitability.Properties).EndInit();
            ((ISupportInitialize)grpCostDates).EndInit();
            grpCostDates.ResumeLayout(false);
            grpCostDates.PerformLayout();
            ((ISupportInitialize)dtpLastPurchase.Properties.CalendarTimeProperties).EndInit();
            ((ISupportInitialize)dtpLastPurchase.Properties).EndInit();
            ((ISupportInitialize)dtpLastSale.Properties.CalendarTimeProperties).EndInit();
            ((ISupportInitialize)dtpLastSale.Properties).EndInit();
            ((ISupportInitialize)sedDaysFromLastPurchase.Properties).EndInit();
            ((ISupportInitialize)sedDaysFromLastSale.Properties).EndInit();
            ((ISupportInitialize)cleRotation30.Properties).EndInit();
            ((ISupportInitialize)cleRotation90.Properties).EndInit();
            ((ISupportInitialize)grpCostHistory).EndInit();
            grpCostHistory.ResumeLayout(false);
            ((ISupportInitialize)grcCosts).EndInit();
            ((ISupportInitialize)grvCosts).EndInit();
            xtpSap.ResumeLayout(false);
            ((ISupportInitialize)grpSapIntegration).EndInit();
            grpSapIntegration.ResumeLayout(false);
            grpSapIntegration.PerformLayout();
            ((ISupportInitialize)txtSapCode.Properties).EndInit();
            ((ISupportInitialize)lueSapStatus.Properties).EndInit();
            ((ISupportInitialize)dtpLastSapSync.Properties.CalendarTimeProperties).EndInit();
            ((ISupportInitialize)dtpLastSapSync.Properties).EndInit();
            ((ISupportInitialize)txtSapDatabase.Properties).EndInit();
            ((ISupportInitialize)txtSapGroup.Properties).EndInit();
            ((ISupportInitialize)txtSapUom.Properties).EndInit();
            ((ISupportInitialize)memSapMessage.Properties).EndInit();
            ((ISupportInitialize)grpSapActions).EndInit();
            grpSapActions.ResumeLayout(false);
            grpSapActions.PerformLayout();
            ((ISupportInitialize)grpSapUdf).EndInit();
            grpSapUdf.ResumeLayout(false);
            grpSapUdf.PerformLayout();
            ((ISupportInitialize)grcSapUdf).EndInit();
            ((ISupportInitialize)grvSapUdf).EndInit();
            ((ISupportInitialize)lueSapUdfPageSize.Properties).EndInit();
            ((ISupportInitialize)grpHeader).EndInit();
            grpHeader.ResumeLayout(false);
            grpHeader.PerformLayout();
            ((ISupportInitialize)txtItemCode.Properties).EndInit();
            ((ISupportInitialize)txtBarCode.Properties).EndInit();
            ((ISupportInitialize)txtDescription.Properties).EndInit();
            ((ISupportInitialize)txtCommercialName.Properties).EndInit();
            ((ISupportInitialize)sleItemGroup.Properties).EndInit();
            ((ISupportInitialize)grvItemGroupLookup).EndInit();
            ((ISupportInitialize)sleBrand.Properties).EndInit();
            ((ISupportInitialize)grvBrandLookup).EndInit();
            ((ISupportInitialize)sleLine.Properties).EndInit();
            ((ISupportInitialize)grvLineLookup).EndInit();
            ((ISupportInitialize)sleHeaderUom.Properties).EndInit();
            ((ISupportInitialize)grvHeaderUomLookup).EndInit();
            ((ISupportInitialize)lueItemType.Properties).EndInit();
            ((ISupportInitialize)lueStatus.Properties).EndInit();
            ((ISupportInitialize)chkInventoryItem.Properties).EndInit();
            ((ISupportInitialize)chkPurchaseItem.Properties).EndInit();
            ((ISupportInitialize)chkSalesItem.Properties).EndInit();
            ((ISupportInitialize)picItem.Properties).EndInit();
            ((ISupportInitialize)pnlSummary).EndInit();
            pnlSummary.ResumeLayout(false);
            pnlSummary.PerformLayout();
            ((ISupportInitialize)memQuickNotes.Properties).EndInit();
            ((ISupportInitialize)pnlFooter).EndInit();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
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

        private void SaveButtonClick(object sender, EventArgs e)
        {
            Save();
        }
    }
}




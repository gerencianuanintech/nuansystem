namespace NuanSystem.WinForms.Services.InventoryItems.Models;

public sealed class ItemMasterData
{
    public ItemGeneralData? General { get; set; }
    public ItemUnitsData? Units { get; set; }
    public ItemInventoryData? Inventory { get; set; }
    public ItemPurchasingData? Purchasing { get; set; }
    public ItemSalesData? Sales { get; set; }
    public ItemCostsData? Costs { get; set; }
    public ItemAccountingData? Accounting { get; set; }
    public ItemTaxesData? Taxes { get; set; }
    public ItemTraceabilityData? Traceability { get; set; }
    public ItemVariantsData? Variants { get; set; }
    public ItemSapData? Sap { get; set; }
    public ItemAttachmentsData? Attachments { get; set; }
    public ItemRemarksData? Remarks { get; set; }
}

public sealed class ItemGeneralData
{
    public string? AlternateCode { get; set; }
    public string? SupplierSku { get; set; }
    public string? LongDescription { get; set; }
    public string? ProductType { get; set; }
    public string? Origin { get; set; }
    public string? Line { get; set; }
    public string? SubGroup { get; set; }
    public string? Model { get; set; }
    public string? Reference { get; set; }
    public bool SalesActive { get; set; }
    public bool PurchaseActive { get; set; }
    public bool ManageInventory { get; set; }
    public bool IsService { get; set; }
    public bool IsKit { get; set; }
    public bool BatchManaged { get; set; }
    public bool SerialManaged { get; set; }
    public bool Perishable { get; set; }
    public bool ExpirationManaged { get; set; }
    public bool RequiresScale { get; set; }
    public bool AllowDiscount { get; set; }
    public bool AffectsInventory { get; set; }
}

public sealed class ItemUnitsData
{
    public int? InventoryUnitOfMeasureId { get; set; }
    public int? PurchaseUnitOfMeasureId { get; set; }
    public int? SalesUnitOfMeasureId { get; set; }
    public decimal PurchaseFactor { get; set; } = 1m;
    public decimal SalesFactor { get; set; } = 1m;
    public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal Volume { get; set; }
    public string? WeightUnit { get; set; }
    public string? VolumeUnit { get; set; }
    public decimal QuantityRounding { get; set; }
    public bool AllowFractions { get; set; }
    public List<ItemPresentationData> Presentations { get; set; } = [];
    public List<ItemBarcodeData> Barcodes { get; set; } = [];
}

public sealed class ItemPresentationData
{
    public string Presentation { get; set; } = string.Empty;
    public int? UnitOfMeasureId { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public decimal InventoryFactor { get; set; } = 1m;
    public string? Barcode { get; set; }
    public bool AppliesToPurchase { get; set; }
    public bool AppliesToSale { get; set; }
    public bool AppliesToInventory { get; set; }
    public bool IsMain { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class ItemBarcodeData
{
    public string Barcode { get; set; } = string.Empty;
    public string Scope { get; set; } = "General";
    public string Presentation { get; set; } = string.Empty;
    public int? UnitOfMeasureId { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public decimal InventoryFactor { get; set; } = 1m;
    public bool IsMain { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class ItemInventoryData
{
    public bool ManageInventory { get; set; }
    public string ValuationMethod { get; set; } = "MovingAverage";
    public string NegativeStockPolicy { get; set; } = "None";
    public bool AutoReplenishment { get; set; }
    public bool ManageLocations { get; set; }
    public bool RequiresCycleCount { get; set; }
    public int CoverageDays { get; set; }
    public decimal GlobalMinimumStock { get; set; }
    public decimal GlobalMaximumStock { get; set; }
    public decimal GlobalReorderPoint { get; set; }
    public int LeadTimeDays { get; set; }
    public int? MainWarehouseId { get; set; }
    public string? SupplyMethod { get; set; }
    public string? ReplenishmentMethod { get; set; }
    public string? AbcClassification { get; set; }
    public string? DefaultLocationCode { get; set; }
    public string? Zone { get; set; }
    public string? Condition { get; set; }
    public bool BatchRequired { get; set; }
    public bool SerialRequired { get; set; }
    public bool AllowTransfers { get; set; }
    public bool Storable { get; set; }
    public string? OperationNote { get; set; }
    public List<ItemWarehouseData> Warehouses { get; set; } = [];
}

public sealed class ItemWarehouseData
{
    public int? WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal CommittedStock { get; set; }
    public decimal OrderedStock { get; set; }
    public decimal AvailableStock { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal MaximumStock { get; set; }
    public decimal ReorderPoint { get; set; }
    public decimal RequiredStock { get; set; }
    public string? DefaultLocationCode { get; set; }
    public decimal WarehouseCost { get; set; }
    public bool IsDefaultWarehouse { get; set; }
    public bool IsLocked { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class ItemPurchasingData
{
    public bool PurchaseEnabled { get; set; }
    public string? MainSupplierCode { get; set; }
    public string? AlternateSupplierCode { get; set; }
    public int? PurchaseUnitOfMeasureId { get; set; }
    public decimal PurchaseMultiple { get; set; } = 1m;
    public decimal MinimumOrderQuantity { get; set; }
    public int LeadTimeDays { get; set; }
    public string? PreferredPurchaseCurrency { get; set; }
    public bool AllowBackorder { get; set; }
    public bool RequiresPurchaseApproval { get; set; }
    public decimal LastPurchaseCost { get; set; }
    public decimal StandardPurchaseCost { get; set; }
    public decimal SupplierDiscountPercent { get; set; }
    public int? PurchaseTaxId { get; set; }
    public string? PurchaseRetention { get; set; }
    public string? PurchaseExpenseAccountCode { get; set; }
    public string? AssignedBuyer { get; set; }
    public string? ReturnPolicy { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
}

public sealed class ItemSalesData
{
    public bool SalesEnabled { get; set; }
    public int? SalesUnitOfMeasureId { get; set; }
    public decimal BasePrice { get; set; }
    public string? MainPriceList { get; set; }
    public bool AllowDiscount { get; set; }
    public decimal MaximumDiscountPercent { get; set; }
    public decimal MinimumMarginPercent { get; set; }
    public decimal MinimumSaleQuantity { get; set; } = 1m;
    public decimal SalesMultiple { get; set; } = 1m;
    public decimal CommissionPercent { get; set; }
    public int? SalesTaxId { get; set; }
    public string? ExciseTax { get; set; }
    public string? SuggestedRetention { get; set; }
    public bool TaxableProduct { get; set; }
    public bool AffectsPromotions { get; set; }
    public bool AllowsReturns { get; set; }
    public bool BlockedForEcommerce { get; set; }
    public string? PreferredChannel { get; set; }
    public string? CommercialPolicy { get; set; }
}

public sealed class ItemCostsData
{
    public decimal AverageCost { get; set; }
    public decimal LastCost { get; set; }
    public decimal StandardCost { get; set; }
    public decimal ReplacementCost { get; set; }
    public string? CostCurrency { get; set; }
    public DateTime? CostUpdatedAt { get; set; }
    public string CostingMethod { get; set; } = "MovingAverage";
    public decimal BasePrice { get; set; }
    public decimal SuggestedPrice { get; set; }
    public decimal GrossMargin { get; set; }
    public decimal GrossMarginPercent { get; set; }
    public decimal MinimumAllowedMarginPercent { get; set; }
    public decimal TwelveMonthProfitabilityPercent { get; set; }
    public DateTime? PriceUpdatedAt { get; set; }
    public List<ItemCostComponentData> Components { get; set; } = [];
}

public sealed class ItemCostComponentData
{
    public string Concept { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Percent { get; set; }
    public string? Note { get; set; }
}

public sealed class ItemAccountingData
{
    public string? InventoryAccountCode { get; set; }
    public string? IncomeAccountCode { get; set; }
    public string? CostOfSalesAccountCode { get; set; }
    public string? SalesReturnAccountCode { get; set; }
    public string? PurchaseReturnAccountCode { get; set; }
    public string? CostVarianceAccountCode { get; set; }
    public string? InventoryAdjustmentAccountCode { get; set; }
    public string? PurchaseExpenseAccountCode { get; set; }
    public string? DefaultBranchCode { get; set; }
    public string? CostCenterCode { get; set; }
    public string? ProjectCode { get; set; }
    public string? BusinessLineCode { get; set; }
    public string? DepartmentCode { get; set; }
    public bool AllowDocumentOverride { get; set; }
    public bool RequiresDimensionInMovements { get; set; }
    public bool GeneratesInventoryEntry { get; set; }
    public bool UsesWarehouseAccount { get; set; }
    public bool UsesGroupAccount { get; set; }
    public bool AllowsCompensation { get; set; }
    public bool AccountingBlocked { get; set; }
    public int ReconciliationDays { get; set; }
    public string? AccountingIntegrationMethod { get; set; }
    public string? AccountingNotes { get; set; }
}

public sealed class ItemTaxesData
{
    public string? FiscalItemType { get; set; }
    public int? PurchaseVatId { get; set; }
    public int? SalesVatId { get; set; }
    public string? ExciseTax { get; set; }
    public bool TaxableService { get; set; }
    public bool ExemptGood { get; set; }
    public string? SuggestedRetention { get; set; }
    public string? TaxSupport { get; set; }
    public string? FiscalCode { get; set; }
    public string? FiscalCountry { get; set; }
    public bool AppliesToPurchases { get; set; }
    public bool AppliesToSales { get; set; }
    public bool AffectsRetention { get; set; }
    public bool AppliesCreditNote { get; set; }
    public bool AppliesExport { get; set; }
    public bool RequiresTariffCode { get; set; }
    public string? TariffCode { get; set; }
    public string? CustomsClassification { get; set; }
    public string? TaxNote { get; set; }
}

public sealed class ItemTraceabilityData
{
    public bool BatchControl { get; set; }
    public bool SerialControl { get; set; }
    public bool RequiresExpiration { get; set; }
    public bool ExpirationRequired { get; set; }
    public int ExpirationAlertDays { get; set; }
    public int QuarantineDays { get; set; }
    public bool GeneratesBatchAutomatically { get; set; }
    public string? BatchPrefix { get; set; }
    public int SerialLength { get; set; }
    public string? FefoFifoMethod { get; set; }
    public bool AllowsMultipleLotsPerDocument { get; set; }
    public bool AllowsReceiptWithoutLot { get; set; }
    public bool AllowsExpiredLotSale { get; set; }
    public bool RequiresLotInTransfers { get; set; }
    public bool RequiresSerialInDispatch { get; set; }
    public string? OperationNote { get; set; }
}

public sealed class ItemVariantsData
{
    public bool ManagesVariants { get; set; }
    public string? VariantType { get; set; }
    public bool AutoGenerateCode { get; set; }
    public string? CodeMask { get; set; }
    public string? BaseVariant { get; set; }
    public bool AllowsSalesByVariant { get; set; }
    public bool AllowsPurchasesByVariant { get; set; }
    public bool AllowsStockByVariant { get; set; }
}

public sealed class ItemSapData
{
    public bool IsSynchronized { get; set; }
    public string? SapCode { get; set; }
    public string? SapItemCode { get; set; }
    public DateTime? LastSynchronizationAt { get; set; }
    public string? SynchronizationStatus { get; set; }
    public string? SapCompany { get; set; }
    public string? TargetDatabase { get; set; }
    public string? LastError { get; set; }
    public bool SynchronizeItem { get; set; }
    public string? SapGroup { get; set; }
    public string? SapUnitGroup { get; set; }
    public string? SapPlanningMethod { get; set; }
    public string? SapSupplyMethod { get; set; }
    public string? SapValuationMethod { get; set; }
    public bool ManagesBatchInSap { get; set; }
    public bool ManagesSerialInSap { get; set; }
    public List<ItemSapFieldMappingData> FieldMappings { get; set; } = [];
}

public sealed class ItemSapFieldMappingData
{
    public string SystemField { get; set; } = string.Empty;
    public string SapField { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool Required { get; set; }
    public bool Enabled { get; set; } = true;
}

public sealed class ItemAttachmentsData
{
    public List<ItemAttachmentData> Files { get; set; } = [];
}

public sealed class ItemAttachmentData
{
    public string DocumentType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Extension { get; set; }
    public string? Size { get; set; }
    public DateTime? UploadDate { get; set; }
    public string? User { get; set; }
    public bool IsMain { get; set; }
    public bool VisibleInSales { get; set; }
    public bool VisibleInPurchases { get; set; }
    public bool VisibleInPortal { get; set; }
    public string Status { get; set; } = "Activo";
    public string? DocumentReference { get; set; }
    public bool IsConfidential { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? AlternativeText { get; set; }
}

public sealed class ItemRemarksData
{
    public string? GeneralRemarks { get; set; }
    public string? GeneralOperationalAlert { get; set; }
    public string? PurchasingRemarks { get; set; }
    public string? SalesRemarks { get; set; }
    public string? InventoryRemarks { get; set; }
    public string? LogisticsQualityRemarks { get; set; }
    public List<ItemOperationalAlertData> OperationalAlerts { get; set; } = [];
    public string? GeneralVisibility { get; set; }
    public string? GeneralPriority { get; set; }
    public bool GeneralIsActive { get; set; } = true;
}

public sealed class ItemOperationalAlertData
{
    public string AlertType { get; set; } = string.Empty;
    public string Process { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime ValidFrom { get; set; } = DateTime.Today;
    public DateTime? ValidTo { get; set; }
    public bool IsBlocking { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Priority { get; set; }
    public bool RequiresConfirmation { get; set; }
}

namespace NuanSystem.Application.Features.BusinessPartners.Dtos;

public sealed class BusinessPartnerDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? CommercialName { get; set; }
    public string PartnerType { get; set; } = "Customer";
    public int IdentificationTypeId { get; set; }
    public string? IdentificationTypeCode { get; set; }
    public string? IdentificationTypeName { get; set; }
    public string IdentificationNumber { get; set; } = string.Empty;
    public int? SupplierGroupId { get; set; }
    public int? SupplierClassId { get; set; }
    public int? EconomicActivityId { get; set; }
    public int? ZoneId { get; set; }
    public int? SupplyMethodId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Website { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
    public int? TaxpayerTypeId { get; set; }
    public int? TaxRegimeId { get; set; }
    public int? FiscalCountryId { get; set; }
    public string? TaxpayerType { get; set; }
    public bool IsAccountingRequired { get; set; }
    public bool AppliesRetention { get; set; }
    public string? FiscalRegime { get; set; }
    public string? CountryCode { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public int? PaymentTermId { get; set; }
    public string? PaymentTermCode { get; set; }
    public string? PaymentTermName { get; set; }
    public int CreditDays { get; set; }
    public decimal CreditLimit { get; set; }
    public int DeliveryDays { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public bool AllowsBackorder { get; set; }
    public string? PreferredCurrencyCode { get; set; }
    public string? PriceListCode { get; set; }
    public string? AssignedSellerCode { get; set; }
    public string? AssignedBuyerCode { get; set; }
    public string? Incoterm { get; set; }
    public decimal CommercialDiscountPercent { get; set; }
    public string? PurchaseCurrencyCode { get; set; }
    public int? PreferredWarehouseId { get; set; }
    public string? PurchaseSupplierType { get; set; }
    public string? PreferredWarehouseCode { get; set; }
    public decimal MinimumOrderQuantity { get; set; }
    public bool ActiveForImport { get; set; }
    public bool SubjectToEvaluation { get; set; }
    public bool AllowsUrgentPurchases { get; set; }
    public int AverageDeliveryDays { get; set; }
    public int LeadTimeDays { get; set; }
    public int DeliveryToleranceDays { get; set; }
    public bool RequiresPurchaseOrder { get; set; }
    public string CreditStatus { get; set; } = "Normal";
    public int? CustomerAccountId { get; set; }
    public string? CustomerAccountCode { get; set; }
    public string? CustomerAccountName { get; set; }
    public int? SupplierAccountId { get; set; }
    public string? SupplierAccountCode { get; set; }
    public string? SupplierAccountName { get; set; }
    public int? CustomerAdvanceAccountId { get; set; }
    public int? SupplierAdvanceAccountId { get; set; }
    public int? RetentionAccountId { get; set; }
    public int? BranchId { get; set; }
    public int? DepartmentId { get; set; }
    public int? BusinessLineId { get; set; }
    public int? CostCenterId { get; set; }
    public int? ProjectId { get; set; }
    public string? BranchName { get; set; }
    public string? DepartmentName { get; set; }
    public string? BusinessLineName { get; set; }
    public string? CostCenterName { get; set; }
    public string? ProjectName { get; set; }
    public string? CostCenterCode { get; set; }
    public int? DefaultExpenseAccountId { get; set; }
    public int? DifferenceAccountId { get; set; }
    public int? RoundingAccountId { get; set; }
    public int? ClearingAccountId { get; set; }
    public int? DiscountAccountId { get; set; }
    public bool AccountingBySupplier { get; set; }
    public bool RequiresProvision { get; set; }
    public bool AllowsAdvance { get; set; }
    public bool AllowsCompensation { get; set; }
    public bool AllowsPartialPayments { get; set; }
    public bool IsPaymentBlocked { get; set; }
    public bool UsesWithholdingBase { get; set; }
    public bool ConciliationRequired { get; set; }
    public int? AccountingPaymentMethodId { get; set; }
    public int? PaymentPriorityId { get; set; }
    public int? ApprovalFlowId { get; set; }
    public int? PaymentDocumentTypeId { get; set; }
    public string? AccountingPaymentMethod { get; set; }
    public string? PaymentPriority { get; set; }
    public string? RequiredPaymentDay { get; set; }
    public string? ApprovalFlow { get; set; }
    public string? PaymentDocumentType { get; set; }
    public int AveragePaymentDays { get; set; }
    public decimal PaymentTolerancePercent { get; set; }
    public string? SapCardCode { get; set; }
    public string? SapCardType { get; set; }
    public string SapSyncStatus { get; set; } = "Pending";
    public DateTime? SapLastSyncAt { get; set; }
    public string? SapLastError { get; set; }
    public bool SapEnabled { get; set; }
    public string? SapMode { get; set; }
    public string? SapCompanyCode { get; set; }
    public int SapRetryCount { get; set; }
    public bool SyncAsSupplier { get; set; }
    public bool AllowManualSapRetry { get; set; }
    public bool RequiresApprovalBeforeSapSync { get; set; }
    public int? CreatedByUserId { get; set; }
    public string? CreatedByUserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public string? UpdatedByUserName { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyCollection<BusinessPartnerAddressDto> Addresses { get; set; } = [];
    public IReadOnlyCollection<BusinessPartnerContactDto> Contacts { get; set; } = [];
    public IReadOnlyCollection<BusinessPartnerBankAccountDto> BankAccounts { get; set; } = [];
    public IReadOnlyCollection<BusinessPartnerRetentionSettingDto> RetentionSettings { get; set; } = [];
    public BusinessPartnerNotesDto? Notes { get; set; }
    public IReadOnlyCollection<BusinessPartnerSapFieldMappingDto> SapFieldMappings { get; set; } = [];
    public IReadOnlyCollection<BusinessPartnerAttachmentDto> Attachments { get; set; } = [];
}

public sealed record BusinessPartnerAddressDto(
    int Id,
    int BusinessPartnerId,
    int? CountryId,
    int? ProvinceId,
    int? CityId,
    string AddressType,
    string Line1,
    string? Line2,
    string? CountryCode,
    string? Province,
    string? City,
    string? PostalCode,
    decimal? Latitude,
    decimal? Longitude,
    bool IsPrimary,
    bool IsActive);

public sealed record BusinessPartnerContactDto(
    int Id,
    int BusinessPartnerId,
    int? ContactTypeId,
    int? ContactChannelId,
    string Name,
    string? Position,
    string? Department,
    string? Phone,
    string? Extension,
    string? Mobile,
    string? Email,
    string? Language,
    bool ReceivesNotifications,
    bool IsPrimary,
    bool IsActive,
    string? Notes);

public sealed record BusinessPartnerBankAccountDto(
    int Id,
    int BusinessPartnerId,
    int? BankId,
    int? BankAccountTypeId,
    string? BankName,
    string? AccountType,
    string AccountNumber,
    string? HolderName,
    string? HolderIdentification,
    string? CurrencyCode,
    string? SwiftCode,
    string? AbaRoutingCode,
    string? Iban,
    string? BankCountry,
    string? BankCity,
    string? Notes,
    bool IsPrimary,
    bool IsActive);

public sealed record BusinessPartnerRetentionSettingDto(
    int Id,
    int BusinessPartnerId,
    int? RetentionTypeId,
    int? RetentionConceptId,
    int? TaxSupportId,
    string? RetentionType,
    string? SriCode,
    decimal Percent,
    int? EntryAccountId,
    string? TaxSupport,
    bool AppliesIva,
    bool AppliesIncome,
    bool IsCurrent,
    string? Notes);

public sealed record BusinessPartnerNotesDto(
    int BusinessPartnerId,
    string? InternalNotes,
    string? PurchasingNotes,
    string? PaymentNotes,
    string? OperationalAlert);

public sealed record BusinessPartnerSapFieldMappingDto(
    int Id,
    int BusinessPartnerId,
    string SystemField,
    string SapField,
    string? Description,
    bool IsRequired,
    bool IsEnabled);

public sealed record BusinessPartnerAttachmentDto(
    int Id,
    int BusinessPartnerId,
    string? AttachmentType,
    string FileName,
    string? Description,
    string? ReferencePath,
    long? FileSize,
    string? UploadedBy,
    DateTime? UploadedAt,
    bool IsActive);

public sealed record BusinessPartnerLookupOptionDto(int Id, string Code, string Name, bool IsActive = true);
public sealed record BusinessPartnerGeoLookupOptionDto(int Id, string Code, string Name, bool IsActive = true, int? CountryId = null, int? ProvinceId = null, string? PostalCode = null);
public sealed record BusinessPartnerIdentificationTypeLookupDto(int Id, string Code, string Name, string? CountryCode);
public sealed record BusinessPartnerPaymentTermLookupDto(int Id, string Code, string Name, int Days, bool IsCredit);
public sealed record BusinessPartnerCodeNameLookupDto(string Code, string Name);
public sealed record BusinessPartnerRetentionConceptLookupDto(
    int Id,
    string Code,
    string Name,
    bool IsActive,
    string? SriCode,
    decimal Percent,
    bool AppliesIva,
    bool AppliesIncome,
    int? RetentionTypeId);

public sealed record BusinessPartnerLookupsDto(
    IReadOnlyCollection<BusinessPartnerIdentificationTypeLookupDto> IdentificationTypes,
    IReadOnlyCollection<BusinessPartnerPaymentTermLookupDto> PaymentTerms,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> Accounts,
    IReadOnlyCollection<BusinessPartnerCodeNameLookupDto> PartnerTypes,
    IReadOnlyCollection<BusinessPartnerCodeNameLookupDto> States,
    IReadOnlyCollection<BusinessPartnerCodeNameLookupDto> SapStatuses,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> SupplierGroups,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> SupplierClasses,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> EconomicActivities,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> Zones,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> SupplyMethods,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> ContactTypes,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> ContactChannels,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> Countries,
    IReadOnlyCollection<BusinessPartnerGeoLookupOptionDto> Provinces,
    IReadOnlyCollection<BusinessPartnerGeoLookupOptionDto> Cities,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> Banks,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> BankAccountTypes,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> Currencies,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> PriceLists,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> PurchasingAgents,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> TaxRegimes,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> TaxpayerTypes,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> RetentionTypes,
    IReadOnlyCollection<BusinessPartnerRetentionConceptLookupDto> RetentionConcepts,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> TaxSupports,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> AccountingPaymentMethods,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> PaymentPriorities,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> ApprovalFlows,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> PaymentDocumentTypes,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> Branches,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> Departments,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> BusinessLines,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> CostCenters,
    IReadOnlyCollection<BusinessPartnerLookupOptionDto> Projects);

public sealed record BusinessPartnerSapImportData(
    string CardCode,
    string CardName,
    string? TaxIdentification,
    string CardType,
    string? Phone,
    string? Email,
    string? Currency,
    bool IsActive,
    int? AuditUserId,
    string? AuditUserName);

public sealed record BusinessPartnerSapImportResultData(
    int BusinessPartnerId,
    string Action,
    string Message);

public sealed record SaveBusinessPartnerAddressData(
    int? CountryId,
    int? ProvinceId,
    int? CityId,
    string AddressType,
    string Line1,
    string? Line2,
    string? CountryCode,
    string? Province,
    string? City,
    string? PostalCode,
    decimal? Latitude,
    decimal? Longitude,
    bool IsPrimary,
    bool IsActive);

public sealed record SaveBusinessPartnerContactData(
    int? ContactTypeId,
    int? ContactChannelId,
    string Name,
    string? Position,
    string? Department,
    string? Phone,
    string? Extension,
    string? Mobile,
    string? Email,
    string? Language,
    bool ReceivesNotifications,
    bool IsPrimary,
    bool IsActive,
    string? Notes);

public sealed record SaveBusinessPartnerBankAccountData(
    int? BankId,
    int? BankAccountTypeId,
    string? BankName,
    string? AccountType,
    string AccountNumber,
    string? HolderName,
    string? HolderIdentification,
    string? CurrencyCode,
    string? SwiftCode,
    string? AbaRoutingCode,
    string? Iban,
    string? BankCountry,
    string? BankCity,
    string? Notes,
    bool IsPrimary,
    bool IsActive);

public sealed record SaveBusinessPartnerRetentionSettingData(
    int? RetentionTypeId,
    int? RetentionConceptId,
    int? TaxSupportId,
    string? RetentionType,
    string? SriCode,
    decimal Percent,
    int? EntryAccountId,
    string? TaxSupport,
    bool AppliesIva,
    bool AppliesIncome,
    bool IsCurrent,
    string? Notes);

public sealed record SaveBusinessPartnerNotesData(
    string? InternalNotes,
    string? PurchasingNotes,
    string? PaymentNotes,
    string? OperationalAlert);

public sealed record SaveBusinessPartnerSapFieldMappingData(
    string SystemField,
    string SapField,
    string? Description,
    bool IsRequired,
    bool IsEnabled);

public sealed record SaveBusinessPartnerAttachmentData(
    string? AttachmentType,
    string FileName,
    string? Description,
    string? ReferencePath,
    long? FileSize,
    bool IsActive);

public sealed record CreateBusinessPartnerData(
    string Code,
    string Name,
    string? CommercialName,
    string PartnerType,
    int IdentificationTypeId,
    string IdentificationNumber,
    int? SupplierGroupId,
    int? SupplierClassId,
    int? EconomicActivityId,
    int? ZoneId,
    int? SupplyMethodId,
    string? Email,
    string? Phone,
    string? Website,
    string? Remarks,
    bool IsActive,
    int? TaxpayerTypeId,
    int? TaxRegimeId,
    int? FiscalCountryId,
    string? TaxpayerType,
    bool IsAccountingRequired,
    bool AppliesRetention,
    string? FiscalRegime,
    string? CountryCode,
    string? Province,
    string? City,
    int? CustomerAccountId,
    int? SupplierAccountId,
    int? CustomerAdvanceAccountId,
    int? SupplierAdvanceAccountId,
    int? RetentionAccountId,
    int? BranchId,
    int? DepartmentId,
    int? BusinessLineId,
    int? CostCenterId,
    int? ProjectId,
    string? CostCenterCode,
    int? DefaultExpenseAccountId,
    int? DifferenceAccountId,
    int? RoundingAccountId,
    int? ClearingAccountId,
    int? DiscountAccountId,
    bool AccountingBySupplier,
    bool RequiresProvision,
    bool AllowsAdvance,
    bool AllowsCompensation,
    bool AllowsPartialPayments,
    bool IsPaymentBlocked,
    bool UsesWithholdingBase,
    bool ConciliationRequired,
    int? AccountingPaymentMethodId,
    int? PaymentPriorityId,
    int? ApprovalFlowId,
    int? PaymentDocumentTypeId,
    string? AccountingPaymentMethod,
    string? PaymentPriority,
    string? RequiredPaymentDay,
    string? ApprovalFlow,
    string? PaymentDocumentType,
    int AveragePaymentDays,
    decimal PaymentTolerancePercent,
    int? PaymentTermId,
    int CreditDays,
    decimal CreditLimit,
    int DeliveryDays,
    decimal MinimumOrderAmount,
    bool AllowsBackorder,
    string? PreferredCurrencyCode,
    string? PriceListCode,
    string? AssignedSellerCode,
    string? AssignedBuyerCode,
    string? Incoterm,
    decimal CommercialDiscountPercent,
    string? PurchaseCurrencyCode,
    int? PreferredWarehouseId,
    string? PurchaseSupplierType,
    string? PreferredWarehouseCode,
    decimal MinimumOrderQuantity,
    bool ActiveForImport,
    bool SubjectToEvaluation,
    bool AllowsUrgentPurchases,
    int AverageDeliveryDays,
    int LeadTimeDays,
    int DeliveryToleranceDays,
    bool RequiresPurchaseOrder,
    string CreditStatus,
    string? SapCardCode,
    string? SapCardType,
    string SapSyncStatus,
    DateTime? SapLastSyncAt,
    string? SapLastError,
    bool SapEnabled,
    string? SapMode,
    string? SapCompanyCode,
    int SapRetryCount,
    bool SyncAsSupplier,
    bool AllowManualSapRetry,
    bool RequiresApprovalBeforeSapSync,
    IReadOnlyCollection<SaveBusinessPartnerAddressData> Addresses,
    IReadOnlyCollection<SaveBusinessPartnerContactData> Contacts,
    IReadOnlyCollection<SaveBusinessPartnerBankAccountData> BankAccounts,
    IReadOnlyCollection<SaveBusinessPartnerRetentionSettingData> RetentionSettings,
    SaveBusinessPartnerNotesData? Notes,
    IReadOnlyCollection<SaveBusinessPartnerSapFieldMappingData> SapFieldMappings,
    IReadOnlyCollection<SaveBusinessPartnerAttachmentData>? Attachments,
    int? CreatedByUserId,
    string? CreatedByUserName);

public sealed record UpdateBusinessPartnerData(
    int Id,
    string Code,
    string Name,
    string? CommercialName,
    string PartnerType,
    int IdentificationTypeId,
    string IdentificationNumber,
    int? SupplierGroupId,
    int? SupplierClassId,
    int? EconomicActivityId,
    int? ZoneId,
    int? SupplyMethodId,
    string? Email,
    string? Phone,
    string? Website,
    string? Remarks,
    bool IsActive,
    int? TaxpayerTypeId,
    int? TaxRegimeId,
    int? FiscalCountryId,
    string? TaxpayerType,
    bool IsAccountingRequired,
    bool AppliesRetention,
    string? FiscalRegime,
    string? CountryCode,
    string? Province,
    string? City,
    int? CustomerAccountId,
    int? SupplierAccountId,
    int? CustomerAdvanceAccountId,
    int? SupplierAdvanceAccountId,
    int? RetentionAccountId,
    int? BranchId,
    int? DepartmentId,
    int? BusinessLineId,
    int? CostCenterId,
    int? ProjectId,
    string? CostCenterCode,
    int? DefaultExpenseAccountId,
    int? DifferenceAccountId,
    int? RoundingAccountId,
    int? ClearingAccountId,
    int? DiscountAccountId,
    bool AccountingBySupplier,
    bool RequiresProvision,
    bool AllowsAdvance,
    bool AllowsCompensation,
    bool AllowsPartialPayments,
    bool IsPaymentBlocked,
    bool UsesWithholdingBase,
    bool ConciliationRequired,
    int? AccountingPaymentMethodId,
    int? PaymentPriorityId,
    int? ApprovalFlowId,
    int? PaymentDocumentTypeId,
    string? AccountingPaymentMethod,
    string? PaymentPriority,
    string? RequiredPaymentDay,
    string? ApprovalFlow,
    string? PaymentDocumentType,
    int AveragePaymentDays,
    decimal PaymentTolerancePercent,
    int? PaymentTermId,
    int CreditDays,
    decimal CreditLimit,
    int DeliveryDays,
    decimal MinimumOrderAmount,
    bool AllowsBackorder,
    string? PreferredCurrencyCode,
    string? PriceListCode,
    string? AssignedSellerCode,
    string? AssignedBuyerCode,
    string? Incoterm,
    decimal CommercialDiscountPercent,
    string? PurchaseCurrencyCode,
    int? PreferredWarehouseId,
    string? PurchaseSupplierType,
    string? PreferredWarehouseCode,
    decimal MinimumOrderQuantity,
    bool ActiveForImport,
    bool SubjectToEvaluation,
    bool AllowsUrgentPurchases,
    int AverageDeliveryDays,
    int LeadTimeDays,
    int DeliveryToleranceDays,
    bool RequiresPurchaseOrder,
    string CreditStatus,
    string? SapCardCode,
    string? SapCardType,
    string SapSyncStatus,
    DateTime? SapLastSyncAt,
    string? SapLastError,
    bool SapEnabled,
    string? SapMode,
    string? SapCompanyCode,
    int SapRetryCount,
    bool SyncAsSupplier,
    bool AllowManualSapRetry,
    bool RequiresApprovalBeforeSapSync,
    IReadOnlyCollection<SaveBusinessPartnerAddressData> Addresses,
    IReadOnlyCollection<SaveBusinessPartnerContactData> Contacts,
    IReadOnlyCollection<SaveBusinessPartnerBankAccountData> BankAccounts,
    IReadOnlyCollection<SaveBusinessPartnerRetentionSettingData> RetentionSettings,
    SaveBusinessPartnerNotesData? Notes,
    IReadOnlyCollection<SaveBusinessPartnerSapFieldMappingData> SapFieldMappings,
    IReadOnlyCollection<SaveBusinessPartnerAttachmentData>? Attachments,
    int? UpdatedByUserId,
    string? UpdatedByUserName);

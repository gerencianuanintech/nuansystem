namespace NuanSystem.WinForms.Services.BusinessPartners.Models;

public sealed class BusinessPartnerItem
{
    public int Id { get; set; }
    public Guid GlobalId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? CommercialName { get; set; }
    public string PartnerType { get; set; } = "Customer";
    public int IdentificationTypeId { get; set; }
    public string? IdentificationTypeCode { get; set; }
    public string? IdentificationTypeName { get; set; }
    public string IdentificationNumber { get; set; } = string.Empty;
    public string NormalizedIdentificationNumber { get; set; } = string.Empty;
    public long CanonicalVersion { get; set; }
    public string RowVersion { get; set; } = string.Empty;
    public string MasterSyncStatus { get; set; } = "Accepted";
    public string? MasterSyncMessage { get; set; }
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
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public IReadOnlyCollection<BusinessPartnerAddressItem> Addresses { get; set; } = [];
    public IReadOnlyCollection<BusinessPartnerContactItem> Contacts { get; set; } = [];
    public IReadOnlyCollection<BusinessPartnerBankAccountItem> BankAccounts { get; set; } = [];
    public IReadOnlyCollection<BusinessPartnerRetentionSettingItem> RetentionSettings { get; set; } = [];
    public BusinessPartnerNotesItem? Notes { get; set; }
    public IReadOnlyCollection<BusinessPartnerSapFieldMappingItem> SapFieldMappings { get; set; } = [];
    public IReadOnlyCollection<BusinessPartnerAttachmentItem> Attachments { get; set; } = [];

    public BusinessPartnerItem CreateCopyDraft()
    {
        var copy = (BusinessPartnerItem)MemberwiseClone();
        copy.Id = 0;
        copy.GlobalId = Guid.Empty;
        copy.Code = string.Empty;
        copy.IdentificationNumber = string.Empty;
        copy.NormalizedIdentificationNumber = string.Empty;
        copy.CanonicalVersion = 0;
        copy.RowVersion = string.Empty;
        copy.MasterSyncStatus = "Accepted";
        copy.MasterSyncMessage = null;
        copy.SapCardCode = null;
        copy.SapSyncStatus = "Pending";
        copy.SapLastSyncAt = null;
        copy.SapLastError = null;
        copy.SapRetryCount = 0;
        copy.CreatedAt = default;
        copy.UpdatedAt = null;
        copy.Addresses = Addresses.Select(item => item with
        {
            Id = 0,
            GlobalId = Guid.Empty,
            BusinessPartnerId = 0
        }).ToArray();
        copy.Contacts = Contacts.Select(item => item with
        {
            Id = 0,
            GlobalId = Guid.Empty,
            BusinessPartnerId = 0
        }).ToArray();
        copy.BankAccounts = BankAccounts.Select(item => item with
        {
            Id = 0,
            BusinessPartnerId = 0
        }).ToArray();
        copy.RetentionSettings = RetentionSettings.Select(item => item with
        {
            Id = 0,
            BusinessPartnerId = 0
        }).ToArray();
        copy.Notes = Notes is null ? null : Notes with { BusinessPartnerId = 0 };
        copy.SapFieldMappings = [];
        copy.Attachments = [];
        return copy;
    }
}

public sealed record BusinessPartnerAddressItem(
    int Id,
    Guid GlobalId,
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

public sealed record BusinessPartnerContactItem(
    int Id,
    Guid GlobalId,
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

public sealed record BusinessPartnerBankAccountItem(
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

public sealed record BusinessPartnerRetentionSettingItem(
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

public sealed record BusinessPartnerNotesItem(
    int BusinessPartnerId,
    string? InternalNotes,
    string? PurchasingNotes,
    string? PaymentNotes,
    string? OperationalAlert);

public sealed record BusinessPartnerSapFieldMappingItem(
    int Id,
    int BusinessPartnerId,
    string SystemField,
    string SapField,
    string? Description,
    bool IsRequired,
    bool IsEnabled);

public sealed record BusinessPartnerAttachmentItem(
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

public sealed record SaveBusinessPartnerAddressRequest(
    Guid? GlobalId,
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

public sealed record SaveBusinessPartnerContactRequest(
    Guid? GlobalId,
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

public sealed record SaveBusinessPartnerBankAccountRequest(
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

public sealed record SaveBusinessPartnerRetentionSettingRequest(
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

public sealed record SaveBusinessPartnerNotesRequest(
    string? InternalNotes,
    string? PurchasingNotes,
    string? PaymentNotes,
    string? OperationalAlert);

public sealed record SaveBusinessPartnerSapFieldMappingRequest(
    string SystemField,
    string SapField,
    string? Description,
    bool IsRequired,
    bool IsEnabled);

public sealed record SaveBusinessPartnerAttachmentRequest(
    string? AttachmentType,
    string FileName,
    string? Description,
    string? ReferencePath,
    long? FileSize,
    bool IsActive);

public sealed record SaveBusinessPartnerRequest(
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
    string? CreditStatus,
    string? SapCardCode,
    string? SapCardType,
    string? SapSyncStatus,
    DateTime? SapLastSyncAt,
    string? SapLastError,
    bool SapEnabled,
    string? SapMode,
    string? SapCompanyCode,
    int SapRetryCount,
    bool SyncAsSupplier,
    bool AllowManualSapRetry,
    bool RequiresApprovalBeforeSapSync,
    IReadOnlyCollection<SaveBusinessPartnerAddressRequest> Addresses,
    IReadOnlyCollection<SaveBusinessPartnerContactRequest> Contacts,
    IReadOnlyCollection<SaveBusinessPartnerBankAccountRequest> BankAccounts,
    IReadOnlyCollection<SaveBusinessPartnerRetentionSettingRequest> RetentionSettings,
    SaveBusinessPartnerNotesRequest? Notes,
    IReadOnlyCollection<SaveBusinessPartnerSapFieldMappingRequest> SapFieldMappings,
    IReadOnlyCollection<SaveBusinessPartnerAttachmentRequest>? Attachments = null,
    string? Incoterm = null,
    decimal CommercialDiscountPercent = 0,
    string? PurchaseCurrencyCode = null,
    int? PreferredWarehouseId = null,
    string? PurchaseSupplierType = null,
    string? PreferredWarehouseCode = null,
    decimal MinimumOrderQuantity = 0,
    bool ActiveForImport = false,
    bool SubjectToEvaluation = false,
    bool AllowsUrgentPurchases = false,
    int AverageDeliveryDays = 0,
    int LeadTimeDays = 0,
    int DeliveryToleranceDays = 0,
    bool RequiresPurchaseOrder = false,
    string? ExpectedRowVersion = null);

public sealed record BusinessPartnerEditPolicyDto(
    bool IsSyncedBranch,
    bool CanEditManagedFields,
    IReadOnlyCollection<string> EditableFields);

public sealed record BusinessPartnerEditPolicy(
    bool IsSyncedBranch,
    bool CanEditManagedFields,
    IReadOnlyCollection<string> EditableFields)
{
    public bool Allows(string fieldName) =>
        CanEditManagedFields || EditableFields.Contains(fieldName, StringComparer.OrdinalIgnoreCase);

    public static BusinessPartnerEditPolicy From(BusinessPartnerEditPolicyDto? policy) => policy is null
        ? new(false, true, [])
        : new(policy.IsSyncedBranch, policy.CanEditManagedFields, policy.EditableFields);
}

public sealed record BusinessPartnerSyncPresentation(
    string Caption,
    string Message,
    bool CanSave,
    string BadgeKind);

public static class BusinessPartnerSyncPresentationPolicy
{
    public static BusinessPartnerSyncPresentation Describe(string? status, string? message)
    {
        var detail = message ?? string.Empty;
        return status?.Trim().ToUpperInvariant() switch
        {
            "PENDINGMASTER" => new("Pendiente en central", detail, false, "Warning"),
            "REJECTED" => new("Rechazado", detail, true, "Error"),
            "CONFLICT" => new("Conflicto", detail, false, "Error"),
            "LEGACYREVIEW" => new("Revisión requerida", detail, false, "Warning"),
            _ => new("Aceptado", detail, true, "Success")
        };
    }
}

public sealed record BusinessPartnerFormEditState(
    bool IsCreating,
    string CodeText,
    string CodeHint,
    bool IdentificationEditable,
    bool CanSave,
    bool NameEditable,
    bool CommercialNameEditable,
    bool PhoneEditable,
    bool EmailEditable,
    bool AddressesEditable,
    bool ContactsEditable,
    bool ManagedFieldsEditable,
    BusinessPartnerSyncPresentation SyncPresentation);

public static class BusinessPartnerFormEditStatePolicy
{
    public static BusinessPartnerFormEditState Evaluate(
        BusinessPartnerItem? partner,
        BusinessPartnerEditPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(policy);
        var isCreating = partner is null || partner.Id <= 0;
        var presentation = BusinessPartnerSyncPresentationPolicy.Describe(
            partner?.MasterSyncStatus,
            partner?.MasterSyncMessage);
        var managedFieldsEditable = !policy.IsSyncedBranch || policy.CanEditManagedFields;

        return new BusinessPartnerFormEditState(
            isCreating,
            isCreating ? string.Empty : ResolveDisplayCode(partner!),
            isCreating ? "Se asigna al guardar" : string.Empty,
            isCreating,
            presentation.CanSave,
            Allows(policy, "Name"),
            Allows(policy, "CommercialName"),
            Allows(policy, "Phone"),
            Allows(policy, "Email"),
            Allows(policy, "Addresses"),
            Allows(policy, "Contacts"),
            managedFieldsEditable,
            presentation);
    }

    private static bool Allows(BusinessPartnerEditPolicy policy, string field) =>
        !policy.IsSyncedBranch || policy.Allows(field);

    private static string ResolveDisplayCode(BusinessPartnerItem partner)
    {
        if (!string.IsNullOrWhiteSpace(partner.Code))
        {
            return partner.Code;
        }

        return partner.GlobalId == Guid.Empty
            ? string.Empty
            : $"BP-{partner.GlobalId:N}".ToUpperInvariant();
    }
}

public sealed record BusinessPartnerLookupOption(int Id, string Code, string Name, bool IsActive = true);
public sealed record BusinessPartnerGeoLookupOption(int Id, string Code, string Name, bool IsActive = true, int? CountryId = null, int? ProvinceId = null, string? PostalCode = null);
public sealed record BusinessPartnerIdentificationTypeLookup(int Id, string Code, string Name, string? CountryCode);
public sealed record BusinessPartnerPaymentTermLookup(int Id, string Code, string Name, int Days, bool IsCredit);
public sealed record BusinessPartnerCodeNameLookup(string Code, string Name);
public sealed record BusinessPartnerRetentionConceptLookup(
    int Id,
    string Code,
    string Name,
    bool IsActive,
    string? SriCode,
    decimal Percent,
    bool AppliesIva,
    bool AppliesIncome,
    int? RetentionTypeId);

public sealed record BusinessPartnerLookups(
    IReadOnlyCollection<BusinessPartnerIdentificationTypeLookup> IdentificationTypes,
    IReadOnlyCollection<BusinessPartnerPaymentTermLookup> PaymentTerms,
    IReadOnlyCollection<BusinessPartnerLookupOption> Accounts,
    IReadOnlyCollection<BusinessPartnerCodeNameLookup> PartnerTypes,
    IReadOnlyCollection<BusinessPartnerCodeNameLookup> States,
    IReadOnlyCollection<BusinessPartnerCodeNameLookup> SapStatuses,
    IReadOnlyCollection<BusinessPartnerLookupOption> SupplierGroups,
    IReadOnlyCollection<BusinessPartnerLookupOption> SupplierClasses,
    IReadOnlyCollection<BusinessPartnerLookupOption> EconomicActivities,
    IReadOnlyCollection<BusinessPartnerLookupOption> Zones,
    IReadOnlyCollection<BusinessPartnerLookupOption> SupplyMethods,
    IReadOnlyCollection<BusinessPartnerLookupOption> ContactTypes,
    IReadOnlyCollection<BusinessPartnerLookupOption> ContactChannels,
    IReadOnlyCollection<BusinessPartnerLookupOption> Countries,
    IReadOnlyCollection<BusinessPartnerGeoLookupOption> Provinces,
    IReadOnlyCollection<BusinessPartnerGeoLookupOption> Cities,
    IReadOnlyCollection<BusinessPartnerLookupOption> Banks,
    IReadOnlyCollection<BusinessPartnerLookupOption> BankAccountTypes,
    IReadOnlyCollection<BusinessPartnerLookupOption> Currencies,
    IReadOnlyCollection<BusinessPartnerLookupOption> PriceLists,
    IReadOnlyCollection<BusinessPartnerLookupOption> PurchasingAgents,
    IReadOnlyCollection<BusinessPartnerLookupOption> TaxRegimes,
    IReadOnlyCollection<BusinessPartnerLookupOption> TaxpayerTypes,
    IReadOnlyCollection<BusinessPartnerLookupOption> RetentionTypes,
    IReadOnlyCollection<BusinessPartnerRetentionConceptLookup> RetentionConcepts,
    IReadOnlyCollection<BusinessPartnerLookupOption> TaxSupports,
    IReadOnlyCollection<BusinessPartnerLookupOption> AccountingPaymentMethods,
    IReadOnlyCollection<BusinessPartnerLookupOption> PaymentPriorities,
    IReadOnlyCollection<BusinessPartnerLookupOption> ApprovalFlows,
    IReadOnlyCollection<BusinessPartnerLookupOption> PaymentDocumentTypes,
    IReadOnlyCollection<BusinessPartnerLookupOption> Branches,
    IReadOnlyCollection<BusinessPartnerLookupOption> Departments,
    IReadOnlyCollection<BusinessPartnerLookupOption> BusinessLines,
    IReadOnlyCollection<BusinessPartnerLookupOption> CostCenters,
    IReadOnlyCollection<BusinessPartnerLookupOption> Projects,
    BusinessPartnerEditPolicyDto? EditPolicy = null);

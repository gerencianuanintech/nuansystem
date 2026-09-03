using System.Collections;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Commands;
using NuanSystem.Application.Features.BusinessPartners.Dtos;

namespace NuanSystem.Application.Features.BusinessPartners.Policies;

public static class BusinessPartnerWritePolicy
{
    public static readonly IReadOnlyCollection<string> BranchEditableFields =
        ["Name", "CommercialName", "Phone", "Email", "Addresses", "Contacts"];

    private static readonly HashSet<string> BranchCreateAllowedInputFields =
    [
        nameof(CreateBusinessPartnerCommand.Name),
        nameof(CreateBusinessPartnerCommand.CommercialName),
        nameof(CreateBusinessPartnerCommand.Phone),
        nameof(CreateBusinessPartnerCommand.Email),
        nameof(CreateBusinessPartnerCommand.Addresses),
        nameof(CreateBusinessPartnerCommand.Contacts),
        nameof(CreateBusinessPartnerCommand.PartnerType),
        nameof(CreateBusinessPartnerCommand.IdentificationTypeId),
        nameof(CreateBusinessPartnerCommand.IdentificationNumber),
        nameof(CreateBusinessPartnerCommand.AuditUserId),
        nameof(CreateBusinessPartnerCommand.AuditUserName)
    ];

    public static bool IsSynchronizedBranch(CompanyConnectionInfo? company) =>
        company is { IsMaster: false, SyncEnabled: true };

    public static bool IsSynchronizedCentral(CompanyConnectionInfo? company) =>
        company is { IsMaster: true, SyncEnabled: true };

    public static bool IsProposalInFlight(string status) =>
        status.Equals("PendingMaster", StringComparison.OrdinalIgnoreCase)
        || status.Equals("Conflict", StringComparison.OrdinalIgnoreCase);

    public static bool RequiresLegacyReview(string status) =>
        status.Equals("LegacyReview", StringComparison.OrdinalIgnoreCase);

    public static BusinessPartnerEditPolicyDto GetEditPolicy(CompanyConnectionInfo? company)
    {
        var branch = IsSynchronizedBranch(company);
        return new BusinessPartnerEditPolicyDto(branch, !branch, branch ? BranchEditableFields : []);
    }

    public static IReadOnlyCollection<string> GetNonDefaultProtectedPaths(CreateBusinessPartnerCommand proposed) =>
        proposed.GetType()
            .GetProperties()
            .Where(property => !BranchCreateAllowedInputFields.Contains(property.Name))
            .Where(property => HasUserValue(property.PropertyType, property.GetValue(proposed)))
            .Select(property => property.Name)
            .ToArray();

    public static IReadOnlyCollection<string> GetChangedProtectedPaths(
        BusinessPartnerDto current,
        UpdateBusinessPartnerData proposed)
    {
        var changed = new List<string>();

        Add(nameof(proposed.SupplierGroupId), current.SupplierGroupId, proposed.SupplierGroupId);
        Add(nameof(proposed.SupplierClassId), current.SupplierClassId, proposed.SupplierClassId);
        Add(nameof(proposed.EconomicActivityId), current.EconomicActivityId, proposed.EconomicActivityId);
        Add(nameof(proposed.ZoneId), current.ZoneId, proposed.ZoneId);
        Add(nameof(proposed.SupplyMethodId), current.SupplyMethodId, proposed.SupplyMethodId);
        Add(nameof(proposed.Website), current.Website, proposed.Website);
        Add(nameof(proposed.Remarks), current.Remarks, proposed.Remarks);
        Add(nameof(proposed.IsActive), current.IsActive, proposed.IsActive);
        Add(nameof(proposed.TaxpayerTypeId), current.TaxpayerTypeId, proposed.TaxpayerTypeId);
        Add(nameof(proposed.TaxRegimeId), current.TaxRegimeId, proposed.TaxRegimeId);
        Add(nameof(proposed.FiscalCountryId), current.FiscalCountryId, proposed.FiscalCountryId);
        Add(nameof(proposed.TaxpayerType), current.TaxpayerType, proposed.TaxpayerType);
        Add(nameof(proposed.IsAccountingRequired), current.IsAccountingRequired, proposed.IsAccountingRequired);
        Add(nameof(proposed.AppliesRetention), current.AppliesRetention, proposed.AppliesRetention);
        Add(nameof(proposed.FiscalRegime), current.FiscalRegime, proposed.FiscalRegime);
        Add(nameof(proposed.CountryCode), current.CountryCode, proposed.CountryCode);
        Add(nameof(proposed.Province), current.Province, proposed.Province);
        Add(nameof(proposed.City), current.City, proposed.City);
        Add(nameof(proposed.CustomerAccountId), current.CustomerAccountId, proposed.CustomerAccountId);
        Add(nameof(proposed.SupplierAccountId), current.SupplierAccountId, proposed.SupplierAccountId);
        Add(nameof(proposed.CustomerAdvanceAccountId), current.CustomerAdvanceAccountId, proposed.CustomerAdvanceAccountId);
        Add(nameof(proposed.SupplierAdvanceAccountId), current.SupplierAdvanceAccountId, proposed.SupplierAdvanceAccountId);
        Add(nameof(proposed.RetentionAccountId), current.RetentionAccountId, proposed.RetentionAccountId);
        Add(nameof(proposed.BranchId), current.BranchId, proposed.BranchId);
        Add(nameof(proposed.DepartmentId), current.DepartmentId, proposed.DepartmentId);
        Add(nameof(proposed.BusinessLineId), current.BusinessLineId, proposed.BusinessLineId);
        Add(nameof(proposed.CostCenterId), current.CostCenterId, proposed.CostCenterId);
        Add(nameof(proposed.ProjectId), current.ProjectId, proposed.ProjectId);
        Add(nameof(proposed.CostCenterCode), current.CostCenterCode, proposed.CostCenterCode);
        Add(nameof(proposed.DefaultExpenseAccountId), current.DefaultExpenseAccountId, proposed.DefaultExpenseAccountId);
        Add(nameof(proposed.DifferenceAccountId), current.DifferenceAccountId, proposed.DifferenceAccountId);
        Add(nameof(proposed.RoundingAccountId), current.RoundingAccountId, proposed.RoundingAccountId);
        Add(nameof(proposed.ClearingAccountId), current.ClearingAccountId, proposed.ClearingAccountId);
        Add(nameof(proposed.DiscountAccountId), current.DiscountAccountId, proposed.DiscountAccountId);
        Add(nameof(proposed.AccountingBySupplier), current.AccountingBySupplier, proposed.AccountingBySupplier);
        Add(nameof(proposed.RequiresProvision), current.RequiresProvision, proposed.RequiresProvision);
        Add(nameof(proposed.AllowsAdvance), current.AllowsAdvance, proposed.AllowsAdvance);
        Add(nameof(proposed.AllowsCompensation), current.AllowsCompensation, proposed.AllowsCompensation);
        Add(nameof(proposed.AllowsPartialPayments), current.AllowsPartialPayments, proposed.AllowsPartialPayments);
        Add(nameof(proposed.IsPaymentBlocked), current.IsPaymentBlocked, proposed.IsPaymentBlocked);
        Add(nameof(proposed.UsesWithholdingBase), current.UsesWithholdingBase, proposed.UsesWithholdingBase);
        Add(nameof(proposed.ConciliationRequired), current.ConciliationRequired, proposed.ConciliationRequired);
        Add(nameof(proposed.AccountingPaymentMethodId), current.AccountingPaymentMethodId, proposed.AccountingPaymentMethodId);
        Add(nameof(proposed.PaymentPriorityId), current.PaymentPriorityId, proposed.PaymentPriorityId);
        Add(nameof(proposed.ApprovalFlowId), current.ApprovalFlowId, proposed.ApprovalFlowId);
        Add(nameof(proposed.PaymentDocumentTypeId), current.PaymentDocumentTypeId, proposed.PaymentDocumentTypeId);
        Add(nameof(proposed.AccountingPaymentMethod), current.AccountingPaymentMethod, proposed.AccountingPaymentMethod);
        Add(nameof(proposed.PaymentPriority), current.PaymentPriority, proposed.PaymentPriority);
        Add(nameof(proposed.RequiredPaymentDay), current.RequiredPaymentDay, proposed.RequiredPaymentDay);
        Add(nameof(proposed.ApprovalFlow), current.ApprovalFlow, proposed.ApprovalFlow);
        Add(nameof(proposed.PaymentDocumentType), current.PaymentDocumentType, proposed.PaymentDocumentType);
        Add(nameof(proposed.AveragePaymentDays), current.AveragePaymentDays, proposed.AveragePaymentDays);
        Add(nameof(proposed.PaymentTolerancePercent), current.PaymentTolerancePercent, proposed.PaymentTolerancePercent);
        Add(nameof(proposed.PaymentTermId), current.PaymentTermId, proposed.PaymentTermId);
        Add(nameof(proposed.CreditDays), current.CreditDays, proposed.CreditDays);
        Add(nameof(proposed.CreditLimit), current.CreditLimit, proposed.CreditLimit);
        Add(nameof(proposed.DeliveryDays), current.DeliveryDays, proposed.DeliveryDays);
        Add(nameof(proposed.MinimumOrderAmount), current.MinimumOrderAmount, proposed.MinimumOrderAmount);
        Add(nameof(proposed.AllowsBackorder), current.AllowsBackorder, proposed.AllowsBackorder);
        Add(nameof(proposed.PreferredCurrencyCode), current.PreferredCurrencyCode, proposed.PreferredCurrencyCode);
        Add(nameof(proposed.PriceListCode), current.PriceListCode, proposed.PriceListCode);
        Add(nameof(proposed.AssignedSellerCode), current.AssignedSellerCode, proposed.AssignedSellerCode);
        Add(nameof(proposed.AssignedBuyerCode), current.AssignedBuyerCode, proposed.AssignedBuyerCode);
        Add(nameof(proposed.Incoterm), current.Incoterm, proposed.Incoterm);
        Add(nameof(proposed.CommercialDiscountPercent), current.CommercialDiscountPercent, proposed.CommercialDiscountPercent);
        Add(nameof(proposed.PurchaseCurrencyCode), current.PurchaseCurrencyCode, proposed.PurchaseCurrencyCode);
        Add(nameof(proposed.PreferredWarehouseId), current.PreferredWarehouseId, proposed.PreferredWarehouseId);
        Add(nameof(proposed.PurchaseSupplierType), current.PurchaseSupplierType, proposed.PurchaseSupplierType);
        Add(nameof(proposed.PreferredWarehouseCode), current.PreferredWarehouseCode, proposed.PreferredWarehouseCode);
        Add(nameof(proposed.MinimumOrderQuantity), current.MinimumOrderQuantity, proposed.MinimumOrderQuantity);
        Add(nameof(proposed.ActiveForImport), current.ActiveForImport, proposed.ActiveForImport);
        Add(nameof(proposed.SubjectToEvaluation), current.SubjectToEvaluation, proposed.SubjectToEvaluation);
        Add(nameof(proposed.AllowsUrgentPurchases), current.AllowsUrgentPurchases, proposed.AllowsUrgentPurchases);
        Add(nameof(proposed.AverageDeliveryDays), current.AverageDeliveryDays, proposed.AverageDeliveryDays);
        Add(nameof(proposed.LeadTimeDays), current.LeadTimeDays, proposed.LeadTimeDays);
        Add(nameof(proposed.DeliveryToleranceDays), current.DeliveryToleranceDays, proposed.DeliveryToleranceDays);
        Add(nameof(proposed.RequiresPurchaseOrder), current.RequiresPurchaseOrder, proposed.RequiresPurchaseOrder);
        Add(nameof(proposed.CreditStatus), current.CreditStatus, proposed.CreditStatus);

        if (!BankAccountsEqual(current.BankAccounts, proposed.BankAccounts)) changed.Add(nameof(proposed.BankAccounts));
        if (!RetentionSettingsEqual(current.RetentionSettings, proposed.RetentionSettings)) changed.Add(nameof(proposed.RetentionSettings));
        if (!NotesEqual(current.Notes, proposed.Notes)) changed.Add(nameof(proposed.Notes));
        if (!SapFieldMappingsEqual(current.SapFieldMappings, proposed.SapFieldMappings)) changed.Add(nameof(proposed.SapFieldMappings));
        if (proposed.Attachments is not null && !AttachmentsEqual(current.Attachments, proposed.Attachments)) changed.Add(nameof(proposed.Attachments));

        return changed;

        void Add<T>(string path, T currentValue, T proposedValue)
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, proposedValue)) changed.Add(path);
        }
    }

    private static bool BankAccountsEqual(IReadOnlyCollection<BusinessPartnerBankAccountDto> current, IReadOnlyCollection<SaveBusinessPartnerBankAccountData> proposed) =>
        current.Select(x => new SaveBusinessPartnerBankAccountData(x.BankId, x.BankAccountTypeId, x.BankName, x.AccountType, x.AccountNumber, x.HolderName, x.HolderIdentification, x.CurrencyCode, x.SwiftCode, x.AbaRoutingCode, x.Iban, x.BankCountry, x.BankCity, x.Notes, x.IsPrimary, x.IsActive)).SequenceEqual(proposed);

    private static bool RetentionSettingsEqual(IReadOnlyCollection<BusinessPartnerRetentionSettingDto> current, IReadOnlyCollection<SaveBusinessPartnerRetentionSettingData> proposed) =>
        current.Select(x => new SaveBusinessPartnerRetentionSettingData(x.RetentionTypeId, x.RetentionConceptId, x.TaxSupportId, x.RetentionType, x.SriCode, x.Percent, x.EntryAccountId, x.TaxSupport, x.AppliesIva, x.AppliesIncome, x.IsCurrent, x.Notes)).SequenceEqual(proposed);

    private static bool NotesEqual(BusinessPartnerNotesDto? current, SaveBusinessPartnerNotesData? proposed) =>
        current is null ? proposed is null : proposed is not null && new SaveBusinessPartnerNotesData(current.InternalNotes, current.PurchasingNotes, current.PaymentNotes, current.OperationalAlert) == proposed;

    private static bool SapFieldMappingsEqual(IReadOnlyCollection<BusinessPartnerSapFieldMappingDto> current, IReadOnlyCollection<SaveBusinessPartnerSapFieldMappingData> proposed) =>
        current.Select(x => new SaveBusinessPartnerSapFieldMappingData(x.SystemField, x.SapField, x.Description, x.IsRequired, x.IsEnabled)).SequenceEqual(proposed);

    private static bool AttachmentsEqual(IReadOnlyCollection<BusinessPartnerAttachmentDto> current, IReadOnlyCollection<SaveBusinessPartnerAttachmentData> proposed) =>
        current.Select(x => new SaveBusinessPartnerAttachmentData(x.AttachmentType, x.FileName, x.Description, x.ReferencePath, x.FileSize, x.IsActive)).SequenceEqual(proposed);

    private static bool HasUserValue(Type propertyType, object? value)
    {
        if (value is not null && Nullable.GetUnderlyingType(propertyType) is not null)
        {
            return true;
        }

        return value switch
        {
            null => false,
            string text => !string.IsNullOrWhiteSpace(text),
            bool flag => flag,
            byte number => number != 0,
            short number => number != 0,
            int number => number != 0,
            long number => number != 0,
            float number => number != 0,
            double number => number != 0,
            decimal number => number != 0,
            IEnumerable values => values.Cast<object?>().Any(),
            _ => true
        };
    }
}

using System.Globalization;
using NuanSystem.WinForms.Services.BusinessPartners.Models;

namespace NuanSystem.WinForms.ViewModels.BusinessPartners.Suppliers;

public static class SupplierBusinessPartnerMapper
{
    public static IReadOnlyCollection<SupplierContactViewModel> ToContactViewModels(
        BusinessPartnerItem? partner,
        BusinessPartnerLookups? lookups = null)
    {
        return partner?.Contacts.Select(contact =>
        {
            var names = SplitContactName(contact.Name);
            var contactType = LookupOption(lookups?.ContactTypes, contact.ContactTypeId);
            var contactChannel = LookupOption(lookups?.ContactChannels, contact.ContactChannelId);
            return new SupplierContactViewModel
            {
                GlobalId = contact.GlobalId,
                ContactTypeId = contact.ContactTypeId,
                ContactTypeCode = contactType?.Code ?? string.Empty,
                ContactTypeName = contactType?.Name ?? string.Empty,
                ContactChannelId = contact.ContactChannelId,
                ContactChannelCode = contactChannel?.Code ?? string.Empty,
                ContactChannelName = contactChannel?.Name ?? string.Empty,
                FirstName = names.FirstName,
                LastName = names.LastName,
                Position = contact.Position ?? string.Empty,
                Department = contact.Department ?? string.Empty,
                Phone = contact.Phone ?? string.Empty,
                Extension = contact.Extension ?? string.Empty,
                Mobile = contact.Mobile ?? string.Empty,
                Email = contact.Email ?? string.Empty,
                Language = contact.Language,
                ReceivesNotifications = contact.ReceivesNotifications,
                IsPrimary = contact.IsPrimary,
                IsActive = contact.IsActive,
                Notes = contact.Notes ?? string.Empty
            };
        }).ToArray() ?? [];
    }

    public static IReadOnlyCollection<SupplierAddressViewModel> ToAddressViewModels(BusinessPartnerItem? partner)
    {
        if (partner?.Addresses is null)
        {
            return [];
        }

        var index = 1;
        return partner.Addresses.Select(address => new SupplierAddressViewModel
        {
            GlobalId = address.GlobalId,
            CountryId = address.CountryId,
            ProvinceId = address.ProvinceId,
            CityId = address.CityId,
            AddressType = FromApiAddressType(address.AddressType),
            Code = $"DIR-{index++:000}",
            AddressName = address.AddressType,
            MainStreet = address.Line1,
            SecondaryStreet = address.Line2 ?? string.Empty,
            Reference = address.Line2 ?? string.Empty,
            Province = address.Province ?? string.Empty,
            City = address.City ?? string.Empty,
            Country = address.CountryCode ?? string.Empty,
            PostalCode = address.PostalCode ?? string.Empty,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            IsPrimary = address.IsPrimary,
            IsDefaultBilling = string.Equals(address.AddressType, "Billing", StringComparison.OrdinalIgnoreCase),
            IsDefaultDelivery = string.Equals(address.AddressType, "Shipping", StringComparison.OrdinalIgnoreCase),
            IsActive = address.IsActive
        }).ToArray();
    }

    public static IReadOnlyCollection<SupplierBankAccountViewModel> ToBankAccountViewModels(BusinessPartnerItem? partner)
    {
        return partner?.BankAccounts.Select(account => new SupplierBankAccountViewModel
        {
            BankName = account.BankName ?? string.Empty,
            AccountType = account.AccountType ?? string.Empty,
            AccountNumber = account.AccountNumber,
            Currency = account.CurrencyCode ?? string.Empty,
            AccountHolder = account.HolderName ?? string.Empty,
            HolderIdentification = account.HolderIdentification ?? string.Empty,
            SwiftBic = account.SwiftCode ?? string.Empty,
            CciIban = account.Iban ?? string.Empty,
            Country = account.BankCountry ?? string.Empty,
            Notes = account.Notes ?? string.Empty,
            IsDefault = account.IsPrimary,
            IsActive = account.IsActive
        }).ToArray() ?? [];
    }

    public static IReadOnlyCollection<SupplierWithholdingViewModel> ToWithholdingViewModels(BusinessPartnerItem? partner)
    {
        return partner?.RetentionSettings.Select(setting => new SupplierWithholdingViewModel
        {
            Document = setting.SriCode ?? setting.RetentionType ?? string.Empty,
            Type = setting.RetentionType ?? string.Empty,
            IncomeTaxWithholdingPercent = setting.AppliesIncome ? setting.Percent : 0m,
            VatWithholdingPercent = setting.AppliesIva ? setting.Percent : 0m,
            TaxSupport = setting.TaxSupport ?? string.Empty,
            IsRequiredAccounting = setting.EntryAccountId.HasValue,
            IsDefault = setting.IsCurrent,
            IsActive = setting.IsCurrent,
            Notes = setting.Notes ?? string.Empty
        }).ToArray() ?? [];
    }

    public static IReadOnlyCollection<SupplierAccountingAccountViewModel> ToAccountingAccountViewModels(BusinessPartnerItem? partner)
    {
        if (partner is null)
        {
            return [];
        }

        var accounts = new List<SupplierAccountingAccountViewModel>();
        AddAccountingAccount(accounts, "Cuenta por Pagar", partner.SupplierAccountCode, partner.SupplierAccountName, isDefault: true);
        AddAccountingAccount(accounts, "Anticipo Proveedor", null, null, isDefault: false);
        AddAccountingAccount(accounts, "Gasto", null, null, isDefault: false);
        AddAccountingAccount(accounts, "Retención", null, null, isDefault: false);
        return accounts;
    }

    public static IReadOnlyCollection<SupplierSapAuditViewModel> ToSapAuditViewModels(BusinessPartnerItem? partner)
    {
        if (partner is null || partner.SapLastSyncAt is null)
        {
            return [];
        }

        var status = string.IsNullOrWhiteSpace(partner.SapSyncStatus) ? "Pending" : partner.SapSyncStatus;
        var message = string.IsNullOrWhiteSpace(partner.SapLastError)
            ? "Último estado SAP registrado para el proveedor."
            : partner.SapLastError;

        return
        [
            new SupplierSapAuditViewModel(
                partner.SapLastSyncAt.Value,
                "Sincronización",
                status,
                "SAP",
                message)
        ];
    }

    public static IReadOnlyCollection<SupplierAttachmentViewModel> ToAttachmentViewModels(BusinessPartnerItem? partner)
    {
        return partner?.Attachments.Select(attachment => new SupplierAttachmentViewModel
        {
            DocumentType = attachment.AttachmentType ?? "Anexo",
            FileName = attachment.FileName,
            UploadDate = attachment.UploadedAt ?? DateTime.MinValue,
            User = attachment.UploadedBy ?? string.Empty,
            FileSize = FormatFileSize(attachment.FileSize ?? 0),
            Status = attachment.IsActive ? "Vigente" : "Inactivo",
            FilePath = attachment.ReferencePath ?? string.Empty,
            Category = attachment.AttachmentType ?? string.Empty,
            Description = attachment.Description ?? string.Empty
        }).ToArray() ?? [];
    }

    public static IReadOnlyCollection<SaveBusinessPartnerContactRequest> ToContactRequests(IEnumerable<SupplierContactViewModel> contacts)
    {
        return contacts
            .Where(contact => !string.IsNullOrWhiteSpace(contact.FullName))
            .Select(contact => new SaveBusinessPartnerContactRequest(
                GlobalId: contact.GlobalId is null || contact.GlobalId == Guid.Empty ? null : contact.GlobalId,
                ContactTypeId: contact.ContactTypeId,
                ContactChannelId: contact.ContactChannelId,
                Name: contact.FullName,
                Position: TrimOrNull(contact.Position),
                Department: TrimOrNull(contact.Department),
                Phone: TrimOrNull(contact.Phone),
                Extension: TrimOrNull(contact.Extension),
                Mobile: TrimOrNull(contact.Mobile),
                Email: TrimOrNull(contact.Email),
                Language: TrimOrNull(contact.Language),
                ReceivesNotifications: contact.ReceivesNotifications,
                IsPrimary: contact.IsPrimary,
                IsActive: contact.IsActive,
                Notes: TrimOrNull(contact.Notes)))
            .ToArray();
    }

    public static IReadOnlyCollection<SaveBusinessPartnerAddressRequest> ToAddressRequests(
        IEnumerable<SupplierAddressViewModel> addresses,
        BusinessPartnerLookups lookups)
    {
        return addresses
            .Where(address => !string.IsNullOrWhiteSpace(address.MainStreet))
            .Select(address => new SaveBusinessPartnerAddressRequest(
                GlobalId: address.GlobalId is null || address.GlobalId == Guid.Empty ? null : address.GlobalId,
                CountryId: LookupId(lookups.Countries, address.Country) ?? address.CountryId,
                ProvinceId: LookupGeoId(lookups.Provinces, address.Province) ?? address.ProvinceId,
                CityId: LookupGeoId(lookups.Cities, address.City) ?? address.CityId,
                AddressType: ToApiAddressType(address),
                Line1: address.FullAddress.Length == 0 ? address.MainStreet.Trim() : address.FullAddress,
                Line2: TrimOrNull(address.SecondaryStreet) ?? TrimOrNull(address.Reference),
                CountryCode: LookupCode(lookups.Countries, address.Country) ?? TrimOrNull(address.Country),
                Province: TrimOrNull(address.Province),
                City: TrimOrNull(address.City),
                PostalCode: TrimOrNull(address.PostalCode),
                Latitude: address.Latitude,
                Longitude: address.Longitude,
                IsPrimary: address.IsPrimary,
                IsActive: address.IsActive))
            .ToArray();
    }

    public static IReadOnlyCollection<SaveBusinessPartnerBankAccountRequest> ToBankAccountRequests(
        IEnumerable<SupplierBankAccountViewModel> bankAccounts,
        BusinessPartnerLookups lookups)
    {
        return bankAccounts
            .Where(account => !string.IsNullOrWhiteSpace(account.AccountNumber))
            .Select(account => new SaveBusinessPartnerBankAccountRequest(
                BankId: LookupId(lookups.Banks, account.BankName),
                BankAccountTypeId: LookupId(lookups.BankAccountTypes, account.AccountType),
                BankName: TrimOrNull(account.BankName),
                AccountType: TrimOrNull(account.AccountType),
                AccountNumber: account.AccountNumber.Trim(),
                HolderName: TrimOrNull(account.AccountHolder),
                HolderIdentification: TrimOrNull(account.HolderIdentification),
                CurrencyCode: LookupCode(lookups.Currencies, account.Currency) ?? TrimOrNull(account.Currency),
                SwiftCode: TrimOrNull(account.SwiftBic),
                AbaRoutingCode: null,
                Iban: TrimOrNull(account.CciIban),
                BankCountry: LookupCode(lookups.Countries, account.Country) ?? TrimOrNull(account.Country),
                BankCity: TrimOrNull(account.Branch),
                Notes: TrimOrNull(account.Notes),
                IsPrimary: account.IsDefault,
                IsActive: account.IsActive))
            .ToArray();
    }

    public static IReadOnlyCollection<SaveBusinessPartnerRetentionSettingRequest> ToRetentionRequests(
        IEnumerable<SupplierWithholdingViewModel> withholdings,
        BusinessPartnerLookups lookups)
    {
        return withholdings
            .Where(withholding => !string.IsNullOrWhiteSpace(withholding.Type) || !string.IsNullOrWhiteSpace(withholding.Document))
            .Select(withholding =>
            {
                var percent = withholding.VatWithholdingPercent > 0 ? withholding.VatWithholdingPercent : withholding.IncomeTaxWithholdingPercent;
                var concept = LookupRetentionConcept(lookups.RetentionConcepts, withholding.Document);
                return new SaveBusinessPartnerRetentionSettingRequest(
                    RetentionTypeId: concept?.RetentionTypeId ?? LookupId(lookups.RetentionTypes, withholding.Type),
                    RetentionConceptId: concept?.Id,
                    TaxSupportId: LookupId(lookups.TaxSupports, withholding.TaxSupport),
                    RetentionType: TrimOrNull(withholding.Type),
                    SriCode: TrimOrNull(concept?.SriCode) ?? TrimOrNull(withholding.Document),
                    Percent: percent,
                    EntryAccountId: null,
                    TaxSupport: TrimOrNull(withholding.TaxSupport),
                    AppliesIva: withholding.VatWithholdingPercent > 0,
                    AppliesIncome: withholding.IncomeTaxWithholdingPercent > 0,
                    IsCurrent: withholding.IsActive,
                    Notes: TrimOrNull(withholding.Notes));
            })
            .ToArray();
    }

    public static IReadOnlyCollection<SaveBusinessPartnerSapFieldMappingRequest> ToSapFieldMappingRequests(BusinessPartnerItem? partner)
    {
        return partner?.SapFieldMappings.Select(mapping => new SaveBusinessPartnerSapFieldMappingRequest(
            mapping.SystemField,
            mapping.SapField,
            mapping.Description,
            mapping.IsRequired,
            mapping.IsEnabled)).ToArray() ?? [];
    }

    public static IReadOnlyCollection<SaveBusinessPartnerAttachmentRequest> ToAttachmentRequests(IEnumerable<SupplierAttachmentViewModel> attachments)
    {
        return attachments
            .Where(attachment => !string.IsNullOrWhiteSpace(attachment.FileName))
            .Select(attachment => new SaveBusinessPartnerAttachmentRequest(
                AttachmentType: TrimOrNull(attachment.DocumentType) ?? TrimOrNull(attachment.Category),
                FileName: attachment.FileName.Trim(),
                Description: TrimOrNull(attachment.Description),
                ReferencePath: TrimOrNull(attachment.FilePath),
                FileSize: ParseFileSize(attachment.FileSize),
                IsActive: !string.Equals(attachment.Status, "Inactivo", StringComparison.OrdinalIgnoreCase)))
            .ToArray();
    }

    public static SaveBusinessPartnerRequest ApplyAccountingFields(
        SaveBusinessPartnerRequest request,
        IEnumerable<SupplierAccountingAccountViewModel> accounts,
        BusinessPartnerLookups lookups)
    {
        int? supplierAccountId = request.SupplierAccountId;
        int? supplierAdvanceAccountId = request.SupplierAdvanceAccountId;
        int? defaultExpenseAccountId = request.DefaultExpenseAccountId;
        int? retentionAccountId = request.RetentionAccountId;

        foreach (var account in accounts)
        {
            var accountId = LookupId(lookups.Accounts, account.AccountCode);
            if (accountId is null)
            {
                continue;
            }

            if (Contains(account.AccountType, "pagar"))
            {
                supplierAccountId = accountId;
            }
            else if (Contains(account.AccountType, "anticipo"))
            {
                supplierAdvanceAccountId = accountId;
            }
            else if (Contains(account.AccountType, "gasto"))
            {
                defaultExpenseAccountId = accountId;
            }
            else if (Contains(account.AccountType, "retenc"))
            {
                retentionAccountId = accountId;
            }
        }

        return request with
        {
            SupplierAccountId = supplierAccountId,
            SupplierAdvanceAccountId = supplierAdvanceAccountId,
            DefaultExpenseAccountId = defaultExpenseAccountId,
            RetentionAccountId = retentionAccountId
        };
    }

    public static SaveBusinessPartnerRequest ProjectRequest(
        SaveBusinessPartnerRequest proposed,
        BusinessPartnerItem? current,
        BusinessPartnerEditPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(proposed);
        ArgumentNullException.ThrowIfNull(policy);
        if (!policy.IsSyncedBranch)
        {
            return proposed;
        }

        return current is null || current.Id <= 0
            ? ProjectBranchCreate(proposed)
            : ProjectBranchUpdate(proposed, current);
    }

    public static SaveBusinessPartnerRequest ComposeCustomerRequest(
        SaveBusinessPartnerRequest formDraft,
        BusinessPartnerItem? current,
        BusinessPartnerEditPolicy policy)
    {
        ArgumentNullException.ThrowIfNull(formDraft);
        ArgumentNullException.ThrowIfNull(policy);

        if (current is null || current.Id <= 0)
        {
            return ProjectRequest(formDraft, current, policy);
        }

        var completeRequest = ProjectBranchUpdate(formDraft, current) with
        {
            Remarks = formDraft.Remarks,
            IsActive = formDraft.IsActive,
            TaxpayerType = formDraft.TaxpayerType,
            IsAccountingRequired = formDraft.IsAccountingRequired,
            AppliesRetention = formDraft.AppliesRetention,
            FiscalRegime = formDraft.FiscalRegime,
            CountryCode = formDraft.CountryCode,
            Province = formDraft.Province,
            City = formDraft.City,
            CustomerAccountId = formDraft.CustomerAccountId,
            CustomerAdvanceAccountId = formDraft.CustomerAdvanceAccountId,
            RetentionAccountId = formDraft.RetentionAccountId,
            CostCenterCode = formDraft.CostCenterCode,
            PaymentTermId = formDraft.PaymentTermId,
            CreditLimit = formDraft.CreditLimit,
            PriceListCode = formDraft.PriceListCode,
            AssignedSellerCode = formDraft.AssignedSellerCode,
            CreditStatus = formDraft.CreditStatus,
            SapCardCode = formDraft.SapCardCode,
            SapSyncStatus = formDraft.SapSyncStatus
        };

        return ProjectRequest(completeRequest, current, policy);
    }

    public static CustomerContactDetailViewModel ToCustomerContactDetail(SupplierContactViewModel? contact) =>
        new(
            contact?.FullName ?? string.Empty,
            contact?.Position ?? string.Empty,
            contact?.Phone ?? string.Empty,
            contact?.Mobile ?? string.Empty,
            contact?.Email ?? string.Empty,
            contact?.IsPrimary == true,
            contact?.IsActive == true,
            contact?.Notes ?? string.Empty);

    public static SupplierAddressViewModel ComposeCustomerAddressEditResult(
        SupplierAddressViewModel original,
        SupplierAddressViewModel dialogResult)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(dialogResult);

        var result = dialogResult.Clone();
        result.IsPrimary = original.IsPrimary;
        return result;
    }

    private static SaveBusinessPartnerRequest ProjectBranchCreate(SaveBusinessPartnerRequest proposed)
    {
        return proposed with
        {
            SupplierGroupId = null,
            SupplierClassId = null,
            EconomicActivityId = null,
            ZoneId = null,
            SupplyMethodId = null,
            Website = null,
            Remarks = null,
            IsActive = false,
            TaxpayerTypeId = null,
            TaxRegimeId = null,
            FiscalCountryId = null,
            TaxpayerType = null,
            IsAccountingRequired = false,
            AppliesRetention = false,
            FiscalRegime = null,
            CountryCode = null,
            Province = null,
            City = null,
            CustomerAccountId = null,
            SupplierAccountId = null,
            CustomerAdvanceAccountId = null,
            SupplierAdvanceAccountId = null,
            RetentionAccountId = null,
            BranchId = null,
            DepartmentId = null,
            BusinessLineId = null,
            CostCenterId = null,
            ProjectId = null,
            CostCenterCode = null,
            DefaultExpenseAccountId = null,
            DifferenceAccountId = null,
            RoundingAccountId = null,
            ClearingAccountId = null,
            DiscountAccountId = null,
            AccountingBySupplier = false,
            RequiresProvision = false,
            AllowsAdvance = false,
            AllowsCompensation = false,
            AllowsPartialPayments = false,
            IsPaymentBlocked = false,
            UsesWithholdingBase = false,
            ConciliationRequired = false,
            AccountingPaymentMethodId = null,
            PaymentPriorityId = null,
            ApprovalFlowId = null,
            PaymentDocumentTypeId = null,
            AccountingPaymentMethod = null,
            PaymentPriority = null,
            RequiredPaymentDay = null,
            ApprovalFlow = null,
            PaymentDocumentType = null,
            AveragePaymentDays = 0,
            PaymentTolerancePercent = 0,
            PaymentTermId = null,
            CreditDays = 0,
            CreditLimit = 0,
            DeliveryDays = 0,
            MinimumOrderAmount = 0,
            AllowsBackorder = false,
            PreferredCurrencyCode = null,
            PriceListCode = null,
            AssignedSellerCode = null,
            AssignedBuyerCode = null,
            CreditStatus = null,
            SapCardCode = null,
            SapCardType = null,
            SapSyncStatus = null,
            SapLastSyncAt = null,
            SapLastError = null,
            SapEnabled = false,
            SapMode = null,
            SapCompanyCode = null,
            SapRetryCount = 0,
            SyncAsSupplier = false,
            AllowManualSapRetry = false,
            RequiresApprovalBeforeSapSync = false,
            BankAccounts = [],
            RetentionSettings = [],
            Notes = null,
            SapFieldMappings = [],
            Attachments = [],
            Incoterm = null,
            CommercialDiscountPercent = 0,
            PurchaseCurrencyCode = null,
            PreferredWarehouseId = null,
            PurchaseSupplierType = null,
            PreferredWarehouseCode = null,
            MinimumOrderQuantity = 0,
            ActiveForImport = false,
            SubjectToEvaluation = false,
            AllowsUrgentPurchases = false,
            AverageDeliveryDays = 0,
            LeadTimeDays = 0,
            DeliveryToleranceDays = 0,
            RequiresPurchaseOrder = false,
            ExpectedRowVersion = null
        };
    }

    private static SaveBusinessPartnerRequest ProjectBranchUpdate(
        SaveBusinessPartnerRequest proposed,
        BusinessPartnerItem current)
    {
        return proposed with
        {
            PartnerType = current.PartnerType,
            IdentificationTypeId = current.IdentificationTypeId,
            IdentificationNumber = current.IdentificationNumber,
            SupplierGroupId = current.SupplierGroupId,
            SupplierClassId = current.SupplierClassId,
            EconomicActivityId = current.EconomicActivityId,
            ZoneId = current.ZoneId,
            SupplyMethodId = current.SupplyMethodId,
            Website = current.Website,
            Remarks = current.Remarks,
            IsActive = current.IsActive,
            TaxpayerTypeId = current.TaxpayerTypeId,
            TaxRegimeId = current.TaxRegimeId,
            FiscalCountryId = current.FiscalCountryId,
            TaxpayerType = current.TaxpayerType,
            IsAccountingRequired = current.IsAccountingRequired,
            AppliesRetention = current.AppliesRetention,
            FiscalRegime = current.FiscalRegime,
            CountryCode = current.CountryCode,
            Province = current.Province,
            City = current.City,
            CustomerAccountId = current.CustomerAccountId,
            SupplierAccountId = current.SupplierAccountId,
            CustomerAdvanceAccountId = current.CustomerAdvanceAccountId,
            SupplierAdvanceAccountId = current.SupplierAdvanceAccountId,
            RetentionAccountId = current.RetentionAccountId,
            BranchId = current.BranchId,
            DepartmentId = current.DepartmentId,
            BusinessLineId = current.BusinessLineId,
            CostCenterId = current.CostCenterId,
            ProjectId = current.ProjectId,
            CostCenterCode = current.CostCenterCode,
            DefaultExpenseAccountId = current.DefaultExpenseAccountId,
            DifferenceAccountId = current.DifferenceAccountId,
            RoundingAccountId = current.RoundingAccountId,
            ClearingAccountId = current.ClearingAccountId,
            DiscountAccountId = current.DiscountAccountId,
            AccountingBySupplier = current.AccountingBySupplier,
            RequiresProvision = current.RequiresProvision,
            AllowsAdvance = current.AllowsAdvance,
            AllowsCompensation = current.AllowsCompensation,
            AllowsPartialPayments = current.AllowsPartialPayments,
            IsPaymentBlocked = current.IsPaymentBlocked,
            UsesWithholdingBase = current.UsesWithholdingBase,
            ConciliationRequired = current.ConciliationRequired,
            AccountingPaymentMethodId = current.AccountingPaymentMethodId,
            PaymentPriorityId = current.PaymentPriorityId,
            ApprovalFlowId = current.ApprovalFlowId,
            PaymentDocumentTypeId = current.PaymentDocumentTypeId,
            AccountingPaymentMethod = current.AccountingPaymentMethod,
            PaymentPriority = current.PaymentPriority,
            RequiredPaymentDay = current.RequiredPaymentDay,
            ApprovalFlow = current.ApprovalFlow,
            PaymentDocumentType = current.PaymentDocumentType,
            AveragePaymentDays = current.AveragePaymentDays,
            PaymentTolerancePercent = current.PaymentTolerancePercent,
            PaymentTermId = current.PaymentTermId,
            CreditDays = current.CreditDays,
            CreditLimit = current.CreditLimit,
            DeliveryDays = current.DeliveryDays,
            MinimumOrderAmount = current.MinimumOrderAmount,
            AllowsBackorder = current.AllowsBackorder,
            PreferredCurrencyCode = current.PreferredCurrencyCode,
            PriceListCode = current.PriceListCode,
            AssignedSellerCode = current.AssignedSellerCode,
            AssignedBuyerCode = current.AssignedBuyerCode,
            CreditStatus = current.CreditStatus,
            SapCardCode = current.SapCardCode,
            SapCardType = current.SapCardType,
            SapSyncStatus = current.SapSyncStatus,
            SapLastSyncAt = current.SapLastSyncAt,
            SapLastError = current.SapLastError,
            SapEnabled = current.SapEnabled,
            SapMode = current.SapMode,
            SapCompanyCode = current.SapCompanyCode,
            SapRetryCount = current.SapRetryCount,
            SyncAsSupplier = current.SyncAsSupplier,
            AllowManualSapRetry = current.AllowManualSapRetry,
            RequiresApprovalBeforeSapSync = current.RequiresApprovalBeforeSapSync,
            BankAccounts = current.BankAccounts.Select(ToRequest).ToArray(),
            RetentionSettings = current.RetentionSettings.Select(ToRequest).ToArray(),
            Notes = current.Notes is null ? null : new SaveBusinessPartnerNotesRequest(
                current.Notes.InternalNotes,
                current.Notes.PurchasingNotes,
                current.Notes.PaymentNotes,
                current.Notes.OperationalAlert),
            SapFieldMappings = current.SapFieldMappings.Select(mapping => new SaveBusinessPartnerSapFieldMappingRequest(
                mapping.SystemField,
                mapping.SapField,
                mapping.Description,
                mapping.IsRequired,
                mapping.IsEnabled)).ToArray(),
            Attachments = current.Attachments.Select(attachment => new SaveBusinessPartnerAttachmentRequest(
                attachment.AttachmentType,
                attachment.FileName,
                attachment.Description,
                attachment.ReferencePath,
                attachment.FileSize,
                attachment.IsActive)).ToArray(),
            Incoterm = current.Incoterm,
            CommercialDiscountPercent = current.CommercialDiscountPercent,
            PurchaseCurrencyCode = current.PurchaseCurrencyCode,
            PreferredWarehouseId = current.PreferredWarehouseId,
            PurchaseSupplierType = current.PurchaseSupplierType,
            PreferredWarehouseCode = current.PreferredWarehouseCode,
            MinimumOrderQuantity = current.MinimumOrderQuantity,
            ActiveForImport = current.ActiveForImport,
            SubjectToEvaluation = current.SubjectToEvaluation,
            AllowsUrgentPurchases = current.AllowsUrgentPurchases,
            AverageDeliveryDays = current.AverageDeliveryDays,
            LeadTimeDays = current.LeadTimeDays,
            DeliveryToleranceDays = current.DeliveryToleranceDays,
            RequiresPurchaseOrder = current.RequiresPurchaseOrder,
            ExpectedRowVersion = current.RowVersion
        };
    }

    private static SaveBusinessPartnerBankAccountRequest ToRequest(BusinessPartnerBankAccountItem account) =>
        new(account.BankId, account.BankAccountTypeId, account.BankName, account.AccountType,
            account.AccountNumber, account.HolderName, account.HolderIdentification, account.CurrencyCode,
            account.SwiftCode, account.AbaRoutingCode, account.Iban, account.BankCountry, account.BankCity,
            account.Notes, account.IsPrimary, account.IsActive);

    private static SaveBusinessPartnerRetentionSettingRequest ToRequest(BusinessPartnerRetentionSettingItem setting) =>
        new(setting.RetentionTypeId, setting.RetentionConceptId, setting.TaxSupportId, setting.RetentionType,
            setting.SriCode, setting.Percent, setting.EntryAccountId, setting.TaxSupport, setting.AppliesIva,
            setting.AppliesIncome, setting.IsCurrent, setting.Notes);

    public static string? TrimOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    public static int? LookupId(IReadOnlyCollection<BusinessPartnerLookupOption> options, string? value)
    {
        var normalized = NormalizeLookupValue(value);
        if (normalized is null)
        {
            return null;
        }

        return options.FirstOrDefault(option =>
            string.Equals(option.Code, normalized, StringComparison.OrdinalIgnoreCase)
            || string.Equals(option.Name, normalized, StringComparison.OrdinalIgnoreCase))?.Id;
    }

    public static string? LookupCode(IReadOnlyCollection<BusinessPartnerLookupOption> options, string? value)
    {
        var normalized = NormalizeLookupValue(value);
        if (normalized is null)
        {
            return null;
        }

        return options.FirstOrDefault(option =>
            string.Equals(option.Code, normalized, StringComparison.OrdinalIgnoreCase)
            || string.Equals(option.Name, normalized, StringComparison.OrdinalIgnoreCase))?.Code;
    }

    private static string? NormalizeLookupValue(string? value)
    {
        var normalized = TrimOrNull(value);
        if (normalized is null)
        {
            return null;
        }

        var separator = normalized.IndexOf(" - ", StringComparison.Ordinal);
        return separator > 0 ? normalized[..separator].Trim() : normalized;
    }

    private static int? LookupGeoId(IReadOnlyCollection<BusinessPartnerGeoLookupOption> options, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return options.FirstOrDefault(option =>
            string.Equals(option.Code, value, StringComparison.OrdinalIgnoreCase)
            || string.Equals(option.Name, value, StringComparison.OrdinalIgnoreCase))?.Id;
    }

    private static BusinessPartnerRetentionConceptLookup? LookupRetentionConcept(
        IReadOnlyCollection<BusinessPartnerRetentionConceptLookup> options,
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return options.FirstOrDefault(option =>
            string.Equals(option.Code, value, StringComparison.OrdinalIgnoreCase)
            || string.Equals(option.Name, value, StringComparison.OrdinalIgnoreCase)
            || string.Equals(option.SriCode, value, StringComparison.OrdinalIgnoreCase));
    }

    private static BusinessPartnerLookupOption? LookupOption(
        IReadOnlyCollection<BusinessPartnerLookupOption>? options,
        int? id)
    {
        return id is null ? null : options?.FirstOrDefault(option => option.Id == id.Value);
    }

    private static void AddAccountingAccount(
        ICollection<SupplierAccountingAccountViewModel> accounts,
        string accountType,
        string? accountCode,
        string? accountName,
        bool isDefault)
    {
        if (string.IsNullOrWhiteSpace(accountCode) && string.IsNullOrWhiteSpace(accountName))
        {
            return;
        }

        accounts.Add(new SupplierAccountingAccountViewModel
        {
            AccountType = accountType,
            AccountCode = accountCode ?? string.Empty,
            AccountName = accountName ?? string.Empty,
            IsDefault = isDefault,
            IsActive = true
        });
    }

    private static (string FirstName, string LastName) SplitContactName(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return (string.Empty, string.Empty);
        }

        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length <= 2)
        {
            return (fullName.Trim(), string.Empty);
        }

        return (string.Join(' ', parts.Take(2)), string.Join(' ', parts.Skip(2)));
    }

    private static string ToApiAddressType(SupplierAddressViewModel address)
    {
        if (Contains(address.AddressType, "fact") || Contains(address.AddressType, "billing"))
        {
            return "Billing";
        }

        if (Contains(address.AddressType, "entrega") || Contains(address.AddressType, "shipping"))
        {
            return "Shipping";
        }

        if (Contains(address.AddressType, "fiscal") || Contains(address.AddressType, "main"))
        {
            return "Main";
        }

        if (Contains(address.AddressType, "otro") || Contains(address.AddressType, "other"))
        {
            return "Other";
        }

        return "Other";
    }

    private static string FromApiAddressType(string value)
    {
        return value switch
        {
            "Billing" => "Facturación",
            "Shipping" => "Entrega",
            "Main" => "Fiscal",
            _ => "Otro"
        };
    }

    private static bool Contains(string? value, string text)
    {
        return value?.Contains(text, StringComparison.OrdinalIgnoreCase) == true;
    }

    private static string FormatFileSize(long bytes)
    {
        if (bytes <= 0)
        {
            return string.Empty;
        }

        if (bytes >= 1024 * 1024)
        {
            return $"{bytes / 1024d / 1024d:0.#} MB";
        }

        return $"{Math.Max(1, bytes / 1024)} KB";
    }

    private static long? ParseFileSize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0 || !double.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var amount))
        {
            return null;
        }

        var multiplier = parts.Length > 1 && parts[1].StartsWith("M", StringComparison.OrdinalIgnoreCase)
            ? 1024d * 1024d
            : 1024d;

        return Convert.ToInt64(amount * multiplier);
    }
}

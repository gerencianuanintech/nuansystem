using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.Application.Features.BusinessPartners.SapCodes;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class CreateBusinessPartnerCommandHandler(
    IBusinessPartnerRepository repository,
    ITransactionRunner transactionRunner,
    IBusinessPartnerLocalOutboxWriter localOutboxWriter,
    ICompanyContext companyContext,
    IBusinessPartnerSapCodePolicyRepository sapCodePolicyRepository)
    : ICommandHandler<CreateBusinessPartnerCommand, BusinessPartnerDto>
{
    public async Task<Result<BusinessPartnerDto>> Handle(CreateBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var company = companyContext.CurrentCompany;
        var partnerType = request.PartnerType.Trim();
        if (partnerType is not "Customer" and not "Supplier")
        {
            return Failure("BP_ROLE_INVALID", "El tipo de socio debe ser Customer o Supplier.", nameof(request.PartnerType));
        }

        var globalId = Guid.NewGuid();
        var code = BusinessPartnerIdentityPolicy.CreateInternalCode(globalId);
        var identificationNumber = request.IdentificationNumber.Trim();
        var normalizedIdentification = BusinessPartnerIdentityPolicy.NormalizeIdentification(identificationNumber);
        if (normalizedIdentification.Length == 0)
        {
            return Failure("BP_IDENTIFICATION_INVALID", "La identificacion debe contener letras o numeros.", nameof(request.IdentificationNumber));
        }

        var isBranch = BusinessPartnerWritePolicy.IsSynchronizedBranch(company);
        var isCentral = BusinessPartnerWritePolicy.IsSynchronizedCentral(company);
        if (isBranch)
        {
            var protectedPaths = BusinessPartnerWritePolicy.GetNonDefaultProtectedPaths(request);
            if (protectedPaths.Count > 0)
            {
                return Result<BusinessPartnerDto>.Failure(
                    "La sucursal no puede establecer campos gobernados por la central.",
                    protectedPaths.Select(path => new ApiError("BP_PROTECTED_FIELD", "El campo es administrado por la central.", path)).ToArray());
            }
        }

        var canonicalVersion = isCentral ? 1L : 0L;
        var masterSyncStatus = isBranch ? "PendingMaster" : "Accepted";
        var sapPolicy = company is { IsMaster: true }
            ? await sapCodePolicyRepository.GetByCompanyIdAsync(company.CompanyId, cancellationToken)
            : null;

        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                if (await repository.ExistsByCodeAsync(code, null, connection, transaction, token))
                {
                    return Result<BusinessPartnerDto>.Failure(
                        "Ya existe un tercero comercial con el codigo indicado.",
                        [new ApiError("BusinessPartnerCodeAlreadyExists", "El codigo interno ya existe.", "Code")]);
                }

                if (request.IsActive && await repository.ExistsByIdentificationAsync(
                        partnerType, request.IdentificationTypeId, normalizedIdentification, null, connection, transaction, token))
                {
                    return Result<BusinessPartnerDto>.Failure(
                        "Ya existe un tercero comercial con la identificacion indicada.",
                        [new ApiError("BP_IDENTIFICATION_ALREADY_EXISTS", "La identificacion ya existe para el mismo rol.", nameof(request.IdentificationNumber))]);
                }

                string? sapCardCode = null;
                if (sapPolicy is { IsEnabled: true })
                {
                    var identificationTypeCode = await repository.GetIdentificationTypeCodeAsync(
                        request.IdentificationTypeId, connection, transaction, token);
                    if (identificationTypeCode is null)
                    {
                        return Failure("BP_IDENTIFICATION_TYPE_NOT_FOUND", "El tipo de identificacion no existe.", nameof(request.IdentificationTypeId));
                    }

                    var calculated = BusinessPartnerSapCardCodePolicy.CreateSapCardCode(
                        new BusinessPartnerSapCodePolicyData(
                            GetBusinessPartnerSapCodePolicyQueryHandler.ParsePrefixMode(sapPolicy.PrefixMode),
                            sapPolicy.PassportIdentificationTypeCode),
                        partnerType,
                        identificationTypeCode,
                        normalizedIdentification);
                    if (!calculated.IsSuccess)
                    {
                        return Result<BusinessPartnerDto>.Failure(calculated.Message, calculated.Errors);
                    }

                    sapCardCode = calculated.Value;
                }

                var id = await repository.CreateAsync(
                    ToCreateData(request, globalId, code, partnerType, identificationNumber,
                        normalizedIdentification, canonicalVersion, masterSyncStatus, sapCardCode),
                    connection, transaction, token);
                if (id == -1)
                {
                    return Failure("BP_IDENTIFICATION_ALREADY_EXISTS", "La identificacion ya existe para el mismo rol.", nameof(request.IdentificationNumber));
                }
                if (id == -2)
                {
                    return Failure("BusinessPartnerCodeAlreadyExists", "El codigo interno ya existe.", "Code");
                }
                if (id == -3)
                {
                    return Failure("BP_SAP_CARD_CODE_ALREADY_EXISTS", "El codigo SAP ya esta asignado a otro tercero.", "SapCardCode");
                }
                if (id <= 0)
                {
                    throw new InvalidOperationException($"El procedimiento de creacion devolvio un resultado inesperado: {id}.");
                }

                var partner = await repository.GetByIdAsync(id, connection, transaction, token)
                    ?? throw new InvalidOperationException("El tercero comercial fue creado pero no pudo consultarse.");

                await localOutboxWriter.EnqueueAsync(
                    partner, SyncOperation.Created, connection, transaction, token);
                return Result<BusinessPartnerDto>.Success(partner, "Tercero comercial creado correctamente.");
            },
            cancellationToken);
    }

    internal static CreateBusinessPartnerData ToCreateData(
        CreateBusinessPartnerCommand request,
        Guid globalId,
        string code,
        string partnerType,
        string identificationNumber,
        string normalizedIdentificationNumber,
        long canonicalVersion,
        string masterSyncStatus,
        string? sapCardCode)
    {
        return new CreateBusinessPartnerData(
            globalId,
            code,
            request.Name.Trim(),
            TrimOrNull(request.CommercialName),
            partnerType,
            request.IdentificationTypeId,
            identificationNumber,
            normalizedIdentificationNumber,
            canonicalVersion,
            masterSyncStatus,
            request.SupplierGroupId,
            request.SupplierClassId,
            request.EconomicActivityId,
            request.ZoneId,
            request.SupplyMethodId,
            TrimOrNull(request.Email),
            TrimOrNull(request.Phone),
            TrimOrNull(request.Website),
            TrimOrNull(request.Remarks),
            request.IsActive,
            request.TaxpayerTypeId,
            request.TaxRegimeId,
            request.FiscalCountryId,
            TrimOrNull(request.TaxpayerType),
            request.IsAccountingRequired,
            request.AppliesRetention,
            TrimOrNull(request.FiscalRegime),
            TrimOrNull(request.CountryCode),
            TrimOrNull(request.Province),
            TrimOrNull(request.City),
            request.CustomerAccountId,
            request.SupplierAccountId,
            request.CustomerAdvanceAccountId,
            request.SupplierAdvanceAccountId,
            request.RetentionAccountId,
            request.BranchId,
            request.DepartmentId,
            request.BusinessLineId,
            request.CostCenterId,
            request.ProjectId,
            TrimOrNull(request.CostCenterCode),
            request.DefaultExpenseAccountId,
            request.DifferenceAccountId,
            request.RoundingAccountId,
            request.ClearingAccountId,
            request.DiscountAccountId,
            request.AccountingBySupplier,
            request.RequiresProvision,
            request.AllowsAdvance,
            request.AllowsCompensation,
            request.AllowsPartialPayments,
            request.IsPaymentBlocked,
            request.UsesWithholdingBase,
            request.ConciliationRequired,
            request.AccountingPaymentMethodId,
            request.PaymentPriorityId,
            request.ApprovalFlowId,
            request.PaymentDocumentTypeId,
            TrimOrNull(request.AccountingPaymentMethod),
            TrimOrNull(request.PaymentPriority),
            TrimOrNull(request.RequiredPaymentDay),
            TrimOrNull(request.ApprovalFlow),
            TrimOrNull(request.PaymentDocumentType),
            request.AveragePaymentDays,
            request.PaymentTolerancePercent,
            request.PaymentTermId,
            request.CreditDays,
            request.CreditLimit,
            request.DeliveryDays,
            request.MinimumOrderAmount,
            request.AllowsBackorder,
            NormalizeCurrencyCode(request.PreferredCurrencyCode),
            TrimOrNull(request.PriceListCode),
            TrimOrNull(request.AssignedSellerCode),
            TrimOrNull(request.AssignedBuyerCode),
            TrimOrNull(request.Incoterm),
            request.CommercialDiscountPercent,
            NormalizeCurrencyCode(request.PurchaseCurrencyCode),
            request.PreferredWarehouseId,
            TrimOrNull(request.PurchaseSupplierType),
            TrimOrNull(request.PreferredWarehouseCode),
            request.MinimumOrderQuantity,
            request.ActiveForImport,
            request.SubjectToEvaluation,
            request.AllowsUrgentPurchases,
            request.AverageDeliveryDays,
            request.LeadTimeDays,
            request.DeliveryToleranceDays,
            request.RequiresPurchaseOrder,
            string.IsNullOrWhiteSpace(request.CreditStatus) ? "Normal" : request.CreditStatus.Trim(),
            sapCardCode,
            partnerType == "Supplier" ? "S" : "C",
            "Pending",
            null,
            null,
            sapCardCode is not null,
            null,
            null,
            0,
            partnerType == "Supplier",
            false,
            false,
            NormalizeAddresses(request.Addresses),
            NormalizeContacts(request.Contacts),
            NormalizeBankAccounts(request.BankAccounts),
            NormalizeRetentionSettings(request.RetentionSettings),
            NormalizeNotes(request.Notes),
            NormalizeSapFieldMappings(request.SapFieldMappings),
            NormalizeAttachments(request.Attachments),
            request.AuditUserId,
            TrimOrNull(request.AuditUserName));
    }

    internal static IReadOnlyCollection<SaveBusinessPartnerAddressData> NormalizeAddresses(IReadOnlyCollection<SaveBusinessPartnerAddressData>? addresses)
    {
        return (addresses ?? [])
            .Where(item => !string.IsNullOrWhiteSpace(item.Line1))
            .Select(item => item with
            {
                GlobalId = item.GlobalId.GetValueOrDefault() == Guid.Empty ? Guid.NewGuid() : item.GlobalId,
                CountryId = item.CountryId,
                ProvinceId = item.ProvinceId,
                CityId = item.CityId,
                AddressType = string.IsNullOrWhiteSpace(item.AddressType) ? "Main" : item.AddressType.Trim(),
                Line1 = item.Line1.Trim(),
                Line2 = TrimOrNull(item.Line2),
                CountryCode = TrimOrNull(item.CountryCode),
                Province = TrimOrNull(item.Province),
                City = TrimOrNull(item.City),
                PostalCode = TrimOrNull(item.PostalCode)
            })
            .ToArray();
    }

    internal static IReadOnlyCollection<SaveBusinessPartnerContactData> NormalizeContacts(IReadOnlyCollection<SaveBusinessPartnerContactData>? contacts)
    {
        return (contacts ?? [])
            .Where(item => !string.IsNullOrWhiteSpace(item.Name))
            .Select(item => item with
            {
                GlobalId = item.GlobalId.GetValueOrDefault() == Guid.Empty ? Guid.NewGuid() : item.GlobalId,
                Name = item.Name.Trim(),
                Position = TrimOrNull(item.Position),
                Department = TrimOrNull(item.Department),
                Phone = TrimOrNull(item.Phone),
                Extension = TrimOrNull(item.Extension),
                Mobile = TrimOrNull(item.Mobile),
                Email = TrimOrNull(item.Email),
                Language = TrimOrNull(item.Language),
                Notes = TrimOrNull(item.Notes)
            })
            .ToArray();
    }

    internal static IReadOnlyCollection<SaveBusinessPartnerBankAccountData> NormalizeBankAccounts(IReadOnlyCollection<SaveBusinessPartnerBankAccountData>? bankAccounts)
    {
        return (bankAccounts ?? [])
            .Where(item => !string.IsNullOrWhiteSpace(item.AccountNumber))
            .Select(item => item with
            {
                BankId = item.BankId,
                BankAccountTypeId = item.BankAccountTypeId,
                BankName = TrimOrNull(item.BankName),
                AccountType = TrimOrNull(item.AccountType),
                AccountNumber = item.AccountNumber.Trim(),
                HolderName = TrimOrNull(item.HolderName),
                HolderIdentification = TrimOrNull(item.HolderIdentification),
                CurrencyCode = TrimOrNull(item.CurrencyCode),
                SwiftCode = TrimOrNull(item.SwiftCode),
                AbaRoutingCode = TrimOrNull(item.AbaRoutingCode),
                Iban = TrimOrNull(item.Iban),
                BankCountry = TrimOrNull(item.BankCountry),
                BankCity = TrimOrNull(item.BankCity),
                Notes = TrimOrNull(item.Notes)
            })
            .ToArray();
    }

    internal static IReadOnlyCollection<SaveBusinessPartnerRetentionSettingData> NormalizeRetentionSettings(IReadOnlyCollection<SaveBusinessPartnerRetentionSettingData>? settings)
    {
        return (settings ?? [])
            .Where(item => !string.IsNullOrWhiteSpace(item.RetentionType) || !string.IsNullOrWhiteSpace(item.SriCode))
            .Select(item => item with
            {
                RetentionTypeId = item.RetentionTypeId,
                RetentionConceptId = item.RetentionConceptId,
                TaxSupportId = item.TaxSupportId,
                RetentionType = TrimOrNull(item.RetentionType),
                SriCode = TrimOrNull(item.SriCode),
                TaxSupport = TrimOrNull(item.TaxSupport),
                Notes = TrimOrNull(item.Notes)
            })
            .ToArray();
    }

    internal static SaveBusinessPartnerNotesData? NormalizeNotes(SaveBusinessPartnerNotesData? notes)
    {
        if (notes is null)
        {
            return null;
        }

        var normalized = notes with
        {
            InternalNotes = TrimOrNull(notes.InternalNotes),
            PurchasingNotes = TrimOrNull(notes.PurchasingNotes),
            PaymentNotes = TrimOrNull(notes.PaymentNotes),
            OperationalAlert = TrimOrNull(notes.OperationalAlert)
        };

        return normalized.InternalNotes is null
            && normalized.PurchasingNotes is null
            && normalized.PaymentNotes is null
            && normalized.OperationalAlert is null
            ? null
            : normalized;
    }

    internal static IReadOnlyCollection<SaveBusinessPartnerSapFieldMappingData> NormalizeSapFieldMappings(IReadOnlyCollection<SaveBusinessPartnerSapFieldMappingData>? mappings)
    {
        return (mappings ?? [])
            .Where(item => !string.IsNullOrWhiteSpace(item.SystemField) && !string.IsNullOrWhiteSpace(item.SapField))
            .Select(item => item with
            {
                SystemField = item.SystemField.Trim(),
                SapField = item.SapField.Trim(),
                Description = TrimOrNull(item.Description)
            })
            .ToArray();
    }

    internal static IReadOnlyCollection<SaveBusinessPartnerAttachmentData>? NormalizeAttachments(IReadOnlyCollection<SaveBusinessPartnerAttachmentData>? attachments)
    {
        if (attachments is null)
        {
            return null;
        }

        return attachments
            .Where(item => !string.IsNullOrWhiteSpace(item.FileName))
            .Select(item => item with
            {
                AttachmentType = TrimOrNull(item.AttachmentType),
                FileName = item.FileName.Trim(),
                Description = TrimOrNull(item.Description),
                ReferencePath = TrimOrNull(item.ReferencePath)
            })
            .ToArray();
    }

    internal static string? TrimOrNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    internal static string? NormalizeCurrencyCode(string? value)
    {
        var normalized = TrimOrNull(value);
        return normalized is null ? null : normalized.ToUpperInvariant();
    }

    private static Result<BusinessPartnerDto> Failure(string code, string message, string field) =>
        Result<BusinessPartnerDto>.Failure(
            "No fue posible guardar el tercero comercial.",
            [new ApiError(code, message, field)]);
}

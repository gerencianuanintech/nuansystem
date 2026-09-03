using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.Shared.Sync;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class UpdateBusinessPartnerCommandHandler(
    IBusinessPartnerRepository repository,
    ITransactionRunner transactionRunner,
    IBusinessPartnerLocalOutboxWriter localOutboxWriter,
    ICompanyContext companyContext)
    : ICommandHandler<UpdateBusinessPartnerCommand, BusinessPartnerDto>
{
    public async Task<Result<BusinessPartnerDto>> Handle(UpdateBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        if (!TryDecodeRowVersion(request.ExpectedRowVersion, out var expectedRowVersion))
        {
            return Failure("BP_ROW_VERSION_INVALID", "ExpectedRowVersion debe ser un rowversion base64 valido.", nameof(request.ExpectedRowVersion));
        }

        var company = companyContext.CurrentCompany;
        var isBranch = BusinessPartnerWritePolicy.IsSynchronizedBranch(company);
        var isCentral = BusinessPartnerWritePolicy.IsSynchronizedCentral(company);

        return await transactionRunner.ExecuteInTenantTransactionAsync(
            async (connection, transaction, token) =>
            {
                var current = await repository.GetByIdAsync(request.Id, connection, transaction, token);
                if (current is null)
                {
                    return Result<BusinessPartnerDto>.Failure(
                        "Tercero comercial no encontrado.",
                        [new ApiError("BusinessPartnerNotFound", "Tercero comercial no encontrado.", nameof(request.Id))]);
                }

                if (BusinessPartnerWritePolicy.RequiresLegacyReview(current.MasterSyncStatus))
                {
                    return Failure("BP_LEGACY_REVIEW_REQUIRED", "El tercero debe salir de LegacyReview antes de editarse.", nameof(current.MasterSyncStatus));
                }

                if (isBranch && BusinessPartnerWritePolicy.IsProposalInFlight(current.MasterSyncStatus))
                {
                    return Failure("BP_MASTER_PROPOSAL_IN_FLIGHT", "Ya existe una propuesta pendiente o en conflicto.", nameof(current.MasterSyncStatus));
                }

                var canonicalVersion = isCentral ? current.CanonicalVersion + 1 : current.CanonicalVersion;
                var masterSyncStatus = isBranch ? "PendingMaster" : "Accepted";
                var updateData = ToUpdateData(request, current, expectedRowVersion, canonicalVersion, masterSyncStatus);
                if (isBranch)
                {
                    var protectedPaths = BusinessPartnerWritePolicy.GetChangedProtectedPaths(current, updateData);
                    if (protectedPaths.Count > 0)
                    {
                        return Result<BusinessPartnerDto>.Failure(
                            "La sucursal no puede modificar campos gobernados por la central.",
                            protectedPaths.Select(path => new ApiError("BP_PROTECTED_FIELD", "El campo es administrado por la central.", path)).ToArray());
                    }
                }

                var updated = await repository.UpdateAsync(
                    updateData, connection, transaction, token);
                if (!updated)
                {
                    return Failure("BP_CONCURRENCY_CONFLICT", "El tercero fue modificado por otro proceso. Recargue e intente nuevamente.", nameof(request.ExpectedRowVersion));
                }

                var partner = await repository.GetByIdAsync(request.Id, connection, transaction, token)
                    ?? throw new InvalidOperationException("El tercero comercial fue actualizado pero no pudo consultarse.");
                await localOutboxWriter.EnqueueAsync(
                    partner, SyncOperation.Updated, connection, transaction, token);
                return Result<BusinessPartnerDto>.Success(partner, "Tercero comercial actualizado correctamente.");
            },
            cancellationToken);
    }

    internal static UpdateBusinessPartnerData ToUpdateData(
        UpdateBusinessPartnerCommand request,
        BusinessPartnerDto current,
        byte[] expectedRowVersion,
        long canonicalVersion,
        string masterSyncStatus)
    {
        var createData = CreateBusinessPartnerCommandHandler.ToCreateData(
            new CreateBusinessPartnerCommand(
                request.Name,
                request.CommercialName,
                current.PartnerType,
                current.IdentificationTypeId,
                current.IdentificationNumber,
                request.SupplierGroupId,
                request.SupplierClassId,
                request.EconomicActivityId,
                request.ZoneId,
                request.SupplyMethodId,
                request.Email,
                request.Phone,
                request.Website,
                request.Remarks,
                request.IsActive,
                request.TaxpayerTypeId,
                request.TaxRegimeId,
                request.FiscalCountryId,
                request.TaxpayerType,
                request.IsAccountingRequired,
                request.AppliesRetention,
                request.FiscalRegime,
                request.CountryCode,
                request.Province,
                request.City,
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
                request.CostCenterCode,
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
                request.AccountingPaymentMethod,
                request.PaymentPriority,
                request.RequiredPaymentDay,
                request.ApprovalFlow,
                request.PaymentDocumentType,
                request.AveragePaymentDays,
                request.PaymentTolerancePercent,
                request.PaymentTermId,
                request.CreditDays,
                request.CreditLimit,
                request.DeliveryDays,
                request.MinimumOrderAmount,
                request.AllowsBackorder,
                request.PreferredCurrencyCode,
                request.PriceListCode,
                request.AssignedSellerCode,
                request.AssignedBuyerCode,
                request.Incoterm,
                request.CommercialDiscountPercent,
                request.PurchaseCurrencyCode,
                request.PreferredWarehouseId,
                request.PurchaseSupplierType,
                request.PreferredWarehouseCode,
                request.MinimumOrderQuantity,
                request.ActiveForImport,
                request.SubjectToEvaluation,
                request.AllowsUrgentPurchases,
                request.AverageDeliveryDays,
                request.LeadTimeDays,
                request.DeliveryToleranceDays,
                request.RequiresPurchaseOrder,
                request.CreditStatus,
                request.Addresses,
                request.Contacts,
                request.BankAccounts,
                request.RetentionSettings,
                request.Notes,
                request.SapFieldMappings,
                request.Attachments,
                request.AuditUserId,
                request.AuditUserName),
            current.GlobalId,
            current.Code,
            current.PartnerType,
            current.IdentificationNumber,
            current.NormalizedIdentificationNumber,
            canonicalVersion,
            masterSyncStatus,
            current.SapCardCode);

        return new UpdateBusinessPartnerData(
            request.Id,
            expectedRowVersion,
            createData.Code,
            createData.Name,
            createData.CommercialName,
            createData.PartnerType,
            createData.IdentificationTypeId,
            createData.IdentificationNumber,
            createData.NormalizedIdentificationNumber,
            canonicalVersion,
            masterSyncStatus,
            createData.SupplierGroupId,
            createData.SupplierClassId,
            createData.EconomicActivityId,
            createData.ZoneId,
            createData.SupplyMethodId,
            createData.Email,
            createData.Phone,
            createData.Website,
            createData.Remarks,
            createData.IsActive,
            createData.TaxpayerTypeId,
            createData.TaxRegimeId,
            createData.FiscalCountryId,
            createData.TaxpayerType,
            createData.IsAccountingRequired,
            createData.AppliesRetention,
            createData.FiscalRegime,
            createData.CountryCode,
            createData.Province,
            createData.City,
            createData.CustomerAccountId,
            createData.SupplierAccountId,
            createData.CustomerAdvanceAccountId,
            createData.SupplierAdvanceAccountId,
            createData.RetentionAccountId,
            createData.BranchId,
            createData.DepartmentId,
            createData.BusinessLineId,
            createData.CostCenterId,
            createData.ProjectId,
            createData.CostCenterCode,
            createData.DefaultExpenseAccountId,
            createData.DifferenceAccountId,
            createData.RoundingAccountId,
            createData.ClearingAccountId,
            createData.DiscountAccountId,
            createData.AccountingBySupplier,
            createData.RequiresProvision,
            createData.AllowsAdvance,
            createData.AllowsCompensation,
            createData.AllowsPartialPayments,
            createData.IsPaymentBlocked,
            createData.UsesWithholdingBase,
            createData.ConciliationRequired,
            createData.AccountingPaymentMethodId,
            createData.PaymentPriorityId,
            createData.ApprovalFlowId,
            createData.PaymentDocumentTypeId,
            createData.AccountingPaymentMethod,
            createData.PaymentPriority,
            createData.RequiredPaymentDay,
            createData.ApprovalFlow,
            createData.PaymentDocumentType,
            createData.AveragePaymentDays,
            createData.PaymentTolerancePercent,
            createData.PaymentTermId,
            createData.CreditDays,
            createData.CreditLimit,
            createData.DeliveryDays,
            createData.MinimumOrderAmount,
            createData.AllowsBackorder,
            createData.PreferredCurrencyCode,
            createData.PriceListCode,
            createData.AssignedSellerCode,
            createData.AssignedBuyerCode,
            createData.Incoterm,
            createData.CommercialDiscountPercent,
            createData.PurchaseCurrencyCode,
            createData.PreferredWarehouseId,
            createData.PurchaseSupplierType,
            createData.PreferredWarehouseCode,
            createData.MinimumOrderQuantity,
            createData.ActiveForImport,
            createData.SubjectToEvaluation,
            createData.AllowsUrgentPurchases,
            createData.AverageDeliveryDays,
            createData.LeadTimeDays,
            createData.DeliveryToleranceDays,
            createData.RequiresPurchaseOrder,
            createData.CreditStatus,
            createData.SapCardCode,
            createData.SapCardType,
            createData.SapSyncStatus,
            createData.SapLastSyncAt,
            createData.SapLastError,
            createData.SapEnabled,
            createData.SapMode,
            createData.SapCompanyCode,
            createData.SapRetryCount,
            createData.SyncAsSupplier,
            createData.AllowManualSapRetry,
            createData.RequiresApprovalBeforeSapSync,
            createData.Addresses,
            createData.Contacts,
            createData.BankAccounts,
            createData.RetentionSettings,
            createData.Notes,
            createData.SapFieldMappings,
            createData.Attachments,
            request.AuditUserId,
            CreateBusinessPartnerCommandHandler.TrimOrNull(request.AuditUserName));
    }

    private static bool TryDecodeRowVersion(string? value, out byte[] rowVersion)
    {
        try
        {
            rowVersion = string.IsNullOrWhiteSpace(value) ? [] : Convert.FromBase64String(value);
            return rowVersion.Length == 8;
        }
        catch (FormatException)
        {
            rowVersion = [];
            return false;
        }
    }

    private static Result<BusinessPartnerDto> Failure(string code, string message, string field) =>
        Result<BusinessPartnerDto>.Failure(
            "No fue posible actualizar el tercero comercial.",
            [new ApiError(code, message, field)]);
}

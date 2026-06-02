using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.Commands;

public sealed class UpdateBusinessPartnerCommandHandler(IBusinessPartnerRepository repository)
    : ICommandHandler<UpdateBusinessPartnerCommand, BusinessPartnerDto>
{
    public async Task<Result<BusinessPartnerDto>> Handle(UpdateBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var current = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (current is null)
        {
            return Result<BusinessPartnerDto>.Failure(
                "Tercero comercial no encontrado.",
                [new ApiError("BusinessPartnerNotFound", "Tercero comercial no encontrado.", nameof(request.Id))]);
        }

        var code = request.Code.Trim().ToUpperInvariant();
        var identificationNumber = request.IdentificationNumber.Trim();

        if (await repository.ExistsByCodeAsync(code, request.Id, cancellationToken))
        {
            return Result<BusinessPartnerDto>.Failure(
                "Ya existe un tercero comercial con el codigo indicado.",
                [new ApiError("BusinessPartnerCodeAlreadyExists", "El codigo ya existe.", nameof(request.Code))]);
        }

        if (await repository.ExistsByIdentificationAsync(request.IdentificationTypeId, identificationNumber, request.Id, cancellationToken))
        {
            return Result<BusinessPartnerDto>.Failure(
                "Ya existe un tercero comercial con la identificacion indicada.",
                [new ApiError("BusinessPartnerIdentificationAlreadyExists", "La identificacion ya existe.", nameof(request.IdentificationNumber))]);
        }

        var updated = await repository.UpdateAsync(ToUpdateData(request, code, identificationNumber), cancellationToken);
        if (!updated)
        {
            return Result<BusinessPartnerDto>.Failure("No se pudo actualizar el tercero comercial.");
        }

        var partner = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new InvalidOperationException("El tercero comercial fue actualizado pero no pudo consultarse.");

        return Result<BusinessPartnerDto>.Success(partner, "Tercero comercial actualizado correctamente.");
    }

    private static UpdateBusinessPartnerData ToUpdateData(UpdateBusinessPartnerCommand request, string code, string identificationNumber)
    {
        var createData = CreateBusinessPartnerCommandHandler.ToCreateData(
            new CreateBusinessPartnerCommand(
                request.Code,
                request.Name,
                request.CommercialName,
                request.PartnerType,
                request.IdentificationTypeId,
                request.IdentificationNumber,
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
                request.CreditStatus,
                request.SapCardCode,
                request.SapCardType,
                request.SapSyncStatus,
                request.SapLastSyncAt,
                request.SapLastError,
                request.SapEnabled,
                request.SapMode,
                request.SapCompanyCode,
                request.SapRetryCount,
                request.SyncAsSupplier,
                request.AllowManualSapRetry,
                request.RequiresApprovalBeforeSapSync,
                request.Addresses,
                request.Contacts,
                request.BankAccounts,
                request.RetentionSettings,
                request.Notes,
                request.SapFieldMappings,
                request.Attachments,
                request.AuditUserId,
                request.AuditUserName),
            code,
            identificationNumber);

        return new UpdateBusinessPartnerData(
            request.Id,
            createData.Code,
            createData.Name,
            createData.CommercialName,
            createData.PartnerType,
            createData.IdentificationTypeId,
            createData.IdentificationNumber,
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
}

using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.SapCodes;

public sealed class GetBusinessPartnerSapCodePolicyQueryHandler(
    ICompanyContext companyContext,
    IBusinessPartnerSapCodePolicyRepository repository)
    : IQueryHandler<GetBusinessPartnerSapCodePolicyQuery, BusinessPartnerSapCodePolicyDto>
{
    public async Task<Result<BusinessPartnerSapCodePolicyDto>> Handle(
        GetBusinessPartnerSapCodePolicyQuery request,
        CancellationToken cancellationToken)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null)
        {
            return CompanyRequired();
        }

        if (!company.IsMaster)
        {
            return MasterRequired();
        }

        var policy = await repository.GetByCompanyIdAsync(company.CompanyId, cancellationToken);
        return Result<BusinessPartnerSapCodePolicyDto>.Success(Map(company.CompanyId, policy));
    }

    internal static BusinessPartnerSapCodePolicyDto Map(
        int companyId,
        BusinessPartnerSapCodePolicyRecord? persistedPolicy)
    {
        var prefixMode = persistedPolicy is null
            ? BusinessPartnerSapPrefixMode.NationalForeign
            : ParsePrefixMode(persistedPolicy.PrefixMode);
        var passportCode = persistedPolicy?.PassportIdentificationTypeCode ?? "PASSPORT";
        var policy = new BusinessPartnerSapCodePolicyData(prefixMode, passportCode);
        var nationalIdentificationType = string.Equals(
            passportCode,
            "NATIONAL",
            StringComparison.OrdinalIgnoreCase)
            ? "TAX_ID"
            : "NATIONAL";

        return new BusinessPartnerSapCodePolicyDto(
            companyId,
            persistedPolicy?.IsEnabled ?? false,
            prefixMode.ToString(),
            passportCode,
            Example(policy, "Customer", nationalIdentificationType, "0999999999001"),
            Example(policy, "Customer", passportCode, "AB123"),
            Example(policy, "Supplier", nationalIdentificationType, "0999999999001"),
            Example(policy, "Supplier", passportCode, "AB123"),
            persistedPolicy is null ? string.Empty : Convert.ToBase64String(persistedPolicy.RowVersion));
    }

    internal static BusinessPartnerSapPrefixMode ParsePrefixMode(string value) =>
        Enum.TryParse<BusinessPartnerSapPrefixMode>(value, ignoreCase: false, out var mode)
        && Enum.IsDefined(mode)
            ? mode
            : throw new InvalidOperationException("La politica central contiene un modo de prefijo no soportado.");

    internal static Result<BusinessPartnerSapCodePolicyDto> CompanyRequired() =>
        Result<BusinessPartnerSapCodePolicyDto>.Failure(
            "No hay empresa activa para consultar la politica central de codigos SAP.",
            [new ApiError(
                "COMPANY_REQUIRED",
                "Seleccione una empresa central antes de configurar la politica de codigos SAP.",
                "X-Company-Code")]);

    internal static Result<BusinessPartnerSapCodePolicyDto> MasterRequired() =>
        Result<BusinessPartnerSapCodePolicyDto>.Failure(
            "La politica de codigos SAP solo puede administrarse desde una empresa central.",
            [new ApiError(
                "BP_SAP_CODE_POLICY_MASTER_REQUIRED",
                "Seleccione la empresa central para administrar esta politica.",
                "X-Company-Code")]);

    private static string Example(
        BusinessPartnerSapCodePolicyData policy,
        string partnerType,
        string identificationTypeCode,
        string identificationNumber)
    {
        var result = BusinessPartnerSapCardCodePolicy.CreateSapCardCode(
            policy,
            partnerType,
            identificationTypeCode,
            identificationNumber);
        return result.IsSuccess && result.Value is not null
            ? result.Value
            : throw new InvalidOperationException("No fue posible calcular el ejemplo de codigo SAP.");
    }
}

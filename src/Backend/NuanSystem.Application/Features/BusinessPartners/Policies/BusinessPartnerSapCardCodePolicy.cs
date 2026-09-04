using NuanSystem.Application.Common.Models;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.Policies;

public enum BusinessPartnerSapPrefixMode
{
    NationalForeign,
    RoleOnly
}

public sealed record BusinessPartnerSapCodePolicyData(
    BusinessPartnerSapPrefixMode PrefixMode,
    string PassportIdentificationTypeCode);

public static class BusinessPartnerSapCardCodePolicy
{
    public static Result<string> CreateSapCardCode(
        BusinessPartnerSapCodePolicyData policy,
        string partnerType,
        string identificationTypeCode,
        string normalizedIdentificationNumber)
    {
        if (partnerType is not "Customer" and not "Supplier")
        {
            return Result<string>.Failure(
                "El tipo de socio de negocio debe ser Customer o Supplier.",
                [new ApiError("BP_ROLE_INVALID", "El tipo de socio de negocio no es valido.", nameof(partnerType))]);
        }

        if (string.IsNullOrWhiteSpace(normalizedIdentificationNumber))
        {
            return Result<string>.Failure(
                "El numero de identificacion es obligatorio.",
                [new ApiError("BP_IDENTIFICATION_REQUIRED", "El numero de identificacion es obligatorio.", nameof(normalizedIdentificationNumber))]);
        }

        var isPassport = string.Equals(
            identificationTypeCode,
            policy.PassportIdentificationTypeCode,
            StringComparison.OrdinalIgnoreCase);
        var prefix = policy.PrefixMode switch
        {
            BusinessPartnerSapPrefixMode.NationalForeign when partnerType == "Customer" && !isPassport => "CN",
            BusinessPartnerSapPrefixMode.NationalForeign when partnerType == "Customer" => "CE",
            BusinessPartnerSapPrefixMode.NationalForeign when !isPassport => "PL",
            BusinessPartnerSapPrefixMode.NationalForeign => "PE",
            BusinessPartnerSapPrefixMode.RoleOnly when partnerType == "Customer" => "C",
            BusinessPartnerSapPrefixMode.RoleOnly => "P",
            _ => throw new ArgumentOutOfRangeException(nameof(policy))
        };
        var cardCode = prefix + normalizedIdentificationNumber;

        return cardCode.Length > 15
            ? Result<string>.Failure(
                "El codigo SAP del socio de negocio no puede superar 15 caracteres.",
                [new ApiError("BP_SAP_CARD_CODE_TOO_LONG", "El codigo SAP del socio de negocio no puede superar 15 caracteres.", nameof(normalizedIdentificationNumber))])
            : Result<string>.Success(cardCode);
    }
}

using FluentValidation;
using NuanSystem.Application.Features.BusinessPartners.Policies;

namespace NuanSystem.Application.Features.BusinessPartners.SapCodes;

public sealed class UpdateBusinessPartnerSapCodePolicyCommandValidator
    : AbstractValidator<UpdateBusinessPartnerSapCodePolicyCommand>
{
    public UpdateBusinessPartnerSapCodePolicyCommandValidator()
    {
        RuleFor(command => command.PrefixMode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("BP_SAP_CODE_POLICY_PREFIX_MODE_REQUIRED")
            .Must(value => BusinessPartnerSapPrefixModeAllowlist.TryParse(value, out _))
            .WithMessage("El modo de prefijo debe ser NationalForeign o RoleOnly.")
            .WithErrorCode("BP_SAP_CODE_POLICY_PREFIX_MODE_INVALID");

        RuleFor(command => command.PassportIdentificationTypeCode)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithErrorCode("BP_SAP_CODE_POLICY_PASSPORT_CODE_REQUIRED")
            .MaximumLength(30)
            .WithErrorCode("BP_SAP_CODE_POLICY_PASSPORT_CODE_MAX_LENGTH");

        RuleFor(command => command.ExpectedRowVersion)
            .Must(IsOptionalBase64)
            .WithMessage("ExpectedRowVersion debe ser un valor base64 valido.")
            .WithErrorCode("BP_SAP_CODE_POLICY_ROW_VERSION_INVALID");
    }

    private static bool IsOptionalBase64(string? value)
    {
        if (value is null)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            return Convert.FromBase64String(value).Length > 0;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

internal static class BusinessPartnerSapPrefixModeAllowlist
{
    public static bool TryParse(
        string? value,
        out BusinessPartnerSapPrefixMode prefixMode)
    {
        switch (value?.Trim())
        {
            case nameof(BusinessPartnerSapPrefixMode.NationalForeign):
                prefixMode = BusinessPartnerSapPrefixMode.NationalForeign;
                return true;
            case nameof(BusinessPartnerSapPrefixMode.RoleOnly):
                prefixMode = BusinessPartnerSapPrefixMode.RoleOnly;
                return true;
            default:
                prefixMode = default;
                return false;
        }
    }
}

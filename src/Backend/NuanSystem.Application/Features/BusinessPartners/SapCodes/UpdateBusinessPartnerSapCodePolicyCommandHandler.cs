using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.BusinessPartners.Policies;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.BusinessPartners.SapCodes;

public sealed class UpdateBusinessPartnerSapCodePolicyCommandHandler(
    ICompanyContext companyContext,
    IBusinessPartnerSapCodePolicyRepository repository)
    : ICommandHandler<UpdateBusinessPartnerSapCodePolicyCommand, BusinessPartnerSapCodePolicyDto>
{
    public async Task<Result<BusinessPartnerSapCodePolicyDto>> Handle(
        UpdateBusinessPartnerSapCodePolicyCommand request,
        CancellationToken cancellationToken)
    {
        var company = companyContext.CurrentCompany;
        if (!companyContext.HasActiveCompany || company is null)
        {
            return GetBusinessPartnerSapCodePolicyQueryHandler.CompanyRequired();
        }

        if (!company.IsMaster)
        {
            return GetBusinessPartnerSapCodePolicyQueryHandler.MasterRequired();
        }

        if (!TryParsePrefixMode(request.PrefixMode, out var prefixMode))
        {
            return Invalid(
                "BP_SAP_CODE_POLICY_PREFIX_MODE_INVALID",
                "El modo de prefijo debe ser NationalForeign o RoleOnly.",
                nameof(request.PrefixMode));
        }

        if (!TryDecodeRowVersion(request.ExpectedRowVersion, out var expectedRowVersion))
        {
            return Invalid(
                "BP_SAP_CODE_POLICY_ROW_VERSION_INVALID",
                "ExpectedRowVersion debe ser un valor base64 valido.",
                nameof(request.ExpectedRowVersion));
        }

        var current = await repository.GetByCompanyIdAsync(company.CompanyId, cancellationToken);
        if ((current is null) != (expectedRowVersion is null))
        {
            return ConcurrencyConflict();
        }

        var writeResult = await repository.SaveAsync(
            new SaveBusinessPartnerSapCodePolicyData(
                company.CompanyId,
                request.IsEnabled,
                prefixMode.ToString(),
                request.PassportIdentificationTypeCode.Trim(),
                expectedRowVersion,
                request.AuditUserId,
                Clean(request.AuditUserName)),
            cancellationToken);

        if (writeResult.Outcome == BusinessPartnerSapCodePolicyWriteOutcome.ConcurrencyConflict)
        {
            return ConcurrencyConflict();
        }

        var saved = writeResult.Policy
            ?? throw new InvalidOperationException("La politica fue guardada pero no se devolvio su estado persistido.");

        return Result<BusinessPartnerSapCodePolicyDto>.Success(
            GetBusinessPartnerSapCodePolicyQueryHandler.Map(company.CompanyId, saved),
            "Politica central de codigos SAP actualizada correctamente.");
    }

    private static bool TryParsePrefixMode(
        string value,
        out BusinessPartnerSapPrefixMode prefixMode) =>
        Enum.TryParse(value.Trim(), ignoreCase: false, out prefixMode)
        && Enum.IsDefined(prefixMode);

    private static bool TryDecodeRowVersion(string? value, out byte[]? rowVersion)
    {
        if (value is null)
        {
            rowVersion = null;
            return true;
        }

        try
        {
            rowVersion = Convert.FromBase64String(value);
            return rowVersion.Length > 0;
        }
        catch (FormatException)
        {
            rowVersion = null;
            return false;
        }
    }

    private static Result<BusinessPartnerSapCodePolicyDto> ConcurrencyConflict() =>
        Invalid(
            "BP_SAP_CODE_POLICY_CONCURRENCY_CONFLICT",
            "La politica fue modificada por otro proceso. Recargue e intente nuevamente.",
            "ExpectedRowVersion");

    private static Result<BusinessPartnerSapCodePolicyDto> Invalid(
        string code,
        string message,
        string field) =>
        Result<BusinessPartnerSapCodePolicyDto>.Failure(
            "No fue posible guardar la politica central de codigos SAP.",
            [new ApiError(code, message, field)]);

    private static string? Clean(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

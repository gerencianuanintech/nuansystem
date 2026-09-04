namespace NuanSystem.Application.Features.BusinessPartners.SapCodes;

public sealed record BusinessPartnerSapCodePolicyDto(
    int CompanyId,
    bool IsEnabled,
    string PrefixMode,
    string PassportIdentificationTypeCode,
    string CustomerNationalExample,
    string CustomerForeignExample,
    string SupplierNationalExample,
    string SupplierForeignExample,
    string RowVersion);

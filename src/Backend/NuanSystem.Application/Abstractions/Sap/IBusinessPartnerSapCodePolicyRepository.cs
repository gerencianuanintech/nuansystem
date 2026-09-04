namespace NuanSystem.Application.Abstractions.Sap;

public interface IBusinessPartnerSapCodePolicyRepository
{
    Task<BusinessPartnerSapCodePolicyRecord?> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default);

    Task<BusinessPartnerSapCodePolicyWriteResult> SaveAsync(
        SaveBusinessPartnerSapCodePolicyData policy,
        CancellationToken cancellationToken = default);
}

public sealed record BusinessPartnerSapCodePolicyRecord(
    int CompanyId,
    bool IsEnabled,
    string PrefixMode,
    string PassportIdentificationTypeCode,
    byte[] RowVersion);

public sealed record SaveBusinessPartnerSapCodePolicyData(
    int CompanyId,
    bool IsEnabled,
    string PrefixMode,
    string PassportIdentificationTypeCode,
    byte[]? ExpectedRowVersion,
    int? AuditUserId,
    string? AuditUserName);

public enum BusinessPartnerSapCodePolicyWriteOutcome
{
    Saved,
    ConcurrencyConflict
}

public sealed record BusinessPartnerSapCodePolicyWriteResult(
    BusinessPartnerSapCodePolicyWriteOutcome Outcome,
    BusinessPartnerSapCodePolicyRecord? Policy);

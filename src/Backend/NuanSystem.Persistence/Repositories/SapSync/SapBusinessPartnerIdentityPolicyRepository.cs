using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Mapping;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapBusinessPartnerIdentityPolicyRepository(IMasterConnectionFactory connectionFactory) : ISapBusinessPartnerIdentityPolicyRepository
{
    public async Task<SapBusinessPartnerIdentityPolicy?> GetAsync(int companyId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var row = await connection.QuerySingleOrDefaultAsync<PolicyRow>(new CommandDefinition("dbo.SP_NA_GET_SAPIDENTITYPOLICY_BUSCAR", new { CompanyId = companyId }, cancellationToken: cancellationToken, commandType: System.Data.CommandType.StoredProcedure));
        return row is null ? null : ToPolicy(row);
    }

    public async Task<long> SaveAsync(int companyId, SapBusinessPartnerIdentityPolicy policy, int? auditUserId, string? auditUserName, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<long>(new CommandDefinition("dbo.SP_NA_POST_SAPIDENTITYPOLICY_GUARDAR", new { CompanyId = companyId, policy.CustomerNationalPrefix, policy.CustomerForeignPrefix, policy.SupplierNationalPrefix, policy.SupplierForeignPrefix, policy.PassportTaxpayerTypeCodes, policy.PassportIdentificationTypeCode, AuditUserId = auditUserId, AuditUserName = auditUserName }, cancellationToken: cancellationToken, commandType: System.Data.CommandType.StoredProcedure));
    }

    private static SapBusinessPartnerIdentityPolicy ToPolicy(PolicyRow row) => new(row.CustomerNationalPrefix, row.CustomerForeignPrefix, row.SupplierNationalPrefix, row.SupplierForeignPrefix, row.PassportTaxpayerTypeCodes, row.PassportIdentificationTypeCode);
    private sealed record PolicyRow(
        long Id,
        int CompanyId,
        string CustomerNationalPrefix,
        string CustomerForeignPrefix,
        string SupplierNationalPrefix,
        string SupplierForeignPrefix,
        string PassportTaxpayerTypeCodes,
        string PassportIdentificationTypeCode,
        bool IsActive,
        byte[] RowVersion);
}

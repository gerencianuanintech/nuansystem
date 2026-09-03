using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sap;

namespace NuanSystem.Persistence.Repositories;

public sealed class BusinessPartnerSapCodePolicyRepository(IMasterConnectionFactory connectionFactory)
    : IBusinessPartnerSapCodePolicyRepository
{
    internal const string GetByCompanyIdProcedure =
        "dbo.SP_NA_GET_BUSINESSPARTNERSAPCODEPOLICY_BUSCARPOREMPRESAID";
    internal const string SaveProcedure =
        "dbo.SP_NA_PUT_BUSINESSPARTNERSAPCODEPOLICY_GUARDAR";
    private const int ConcurrencyConflictSqlErrorNumber = 52232;

    public async Task<BusinessPartnerSapCodePolicyRecord?> GetByCompanyIdAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<BusinessPartnerSapCodePolicyRecord>(
            new CommandDefinition(
                GetByCompanyIdProcedure,
                new { CompanyId = companyId },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));
    }

    public async Task<BusinessPartnerSapCodePolicyWriteResult> SaveAsync(
        SaveBusinessPartnerSapCodePolicyData policy,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        try
        {
            var saved = await connection.QuerySingleAsync<BusinessPartnerSapCodePolicyRecord>(
                new CommandDefinition(
                    SaveProcedure,
                    new
                    {
                        policy.CompanyId,
                        policy.IsEnabled,
                        policy.PrefixMode,
                        policy.PassportIdentificationTypeCode,
                        policy.ExpectedRowVersion,
                        UpdatedByUserId = policy.AuditUserId,
                        UpdatedByUserName = policy.AuditUserName
                    },
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));
            return new BusinessPartnerSapCodePolicyWriteResult(
                BusinessPartnerSapCodePolicyWriteOutcome.Saved,
                saved);
        }
        catch (SqlException exception) when (exception.Number == ConcurrencyConflictSqlErrorNumber)
        {
            return new BusinessPartnerSapCodePolicyWriteResult(
                BusinessPartnerSapCodePolicyWriteOutcome.ConcurrencyConflict,
                null);
        }
    }
}

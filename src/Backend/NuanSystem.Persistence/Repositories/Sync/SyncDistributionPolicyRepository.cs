using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Distribution;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class SyncDistributionPolicyRepository(IMasterConnectionFactory connectionFactory)
    : ISyncDistributionPolicyRepository
{
    public async Task<SyncDistributionPolicyDto?> GetByMatrixIdAsync(int matrixId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        using var grid = await connection.QueryMultipleAsync(new CommandDefinition(
            "dbo.SP_NA_GET_SYNCDISTRIBUTIONPOLICYBYMATRIXID",
            new { MatrixId = matrixId },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        var header = await grid.ReadSingleOrDefaultAsync<PolicyHeader>();
        if (header is null)
        {
            return null;
        }

        var selections = (await grid.ReadAsync<SyncDistributionSelectionDto>()).AsList();
        return new SyncDistributionPolicyDto(
            header.SyncProfileEntityBranchId,
            header.SyncProfileId,
            header.SyncProfileCode,
            header.CompanyId,
            header.CompanyCode,
            header.EntityCode,
            header.BranchCompanyId,
            header.BranchCompanyCode,
            header.BranchCompanyName,
            header.DistributionMode,
            header.OnNoMatch,
            header.RuleExpressionJson,
            header.RuleVersion,
            selections);
    }

    public async Task<bool> UpdateAsync(UpdateSyncDistributionPolicyData data, CancellationToken cancellationToken = default)
    {
        var selectionsJson = JsonSerializer.Serialize(data.Selections.Select(item => new
        {
            entityGlobalId = item.EntityGlobalId,
            entityCode = item.EntityCode
        }));
        using var connection = connectionFactory.CreateConnection();
        var affected = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "dbo.SP_NA_PUT_SYNCDISTRIBUTIONPOLICYACTUALIZAR",
            new
            {
                MatrixId = data.SyncProfileEntityBranchId,
                data.DistributionMode,
                data.OnNoMatch,
                data.RuleExpressionJson,
                SelectionsJson = selectionsJson,
                data.AuditUserId,
                data.AuditUserName
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
        return affected > 0;
    }

    private sealed record PolicyHeader(
        int SyncProfileEntityBranchId,
        int SyncProfileId,
        string SyncProfileCode,
        int CompanyId,
        string CompanyCode,
        string EntityCode,
        int BranchCompanyId,
        string BranchCompanyCode,
        string BranchCompanyName,
        string DistributionMode,
        string OnNoMatch,
        string? RuleExpressionJson,
        int RuleVersion);
}

using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class SyncRuleEvaluator(IMasterConnectionFactory connectionFactory) : ISyncRuleEvaluator
{
    public async Task<SyncRuleEvaluationResult> EvaluateAsync(
        SyncRuleEvaluationContext context,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var branchCompanyIds = await connection.QueryAsync<int>(new CommandDefinition(
            "dbo.SP_NA_GET_SYNCDISTRIBUTIONRULETARGETS",
            new
            {
                context.CompanyId,
                context.EntityName,
                context.EntityCode
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        var targets = branchCompanyIds.AsList();
        return targets.Count == 0
            ? new SyncRuleEvaluationResult(false, targets, "No existen reglas de distribucion activas para sucursales habilitadas.")
            : new SyncRuleEvaluationResult(true, targets);
    }
}

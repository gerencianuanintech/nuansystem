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
        const string sql = """
SELECT DISTINCT
    distRule.BranchCompanyId
FROM dbo.SyncDistributionRules AS distRule
INNER JOIN dbo.Companies AS branch
    ON branch.Id = distRule.BranchCompanyId
WHERE distRule.CompanyId = @CompanyId
  AND distRule.EntityName = @EntityName
  AND distRule.IsEnabled = 1
  AND branch.IsActive = 1
  AND branch.IsMaster = 0
  AND branch.SyncEnabled = 1
  AND branch.ParentCompanyId = @CompanyId
  AND branch.IsDeleted = 0
  AND (
        distRule.RuleType = N'All'
        OR (distRule.RuleType = N'ByEntityCode' AND distRule.RuleValue = @EntityCode)
        OR (distRule.RuleType = N'ByBranch' AND distRule.RuleValue = branch.BranchCode)
      )
ORDER BY distRule.BranchCompanyId;
""";

        using var connection = connectionFactory.CreateConnection();
        var branchCompanyIds = await connection.QueryAsync<int>(new CommandDefinition(
            sql,
            new
            {
                context.CompanyId,
                context.EntityName,
                context.EntityCode
            },
            cancellationToken: cancellationToken));

        var targets = branchCompanyIds.AsList();
        return targets.Count == 0
            ? new SyncRuleEvaluationResult(false, targets, "No existen reglas de distribucion activas para sucursales habilitadas.")
            : new SyncRuleEvaluationResult(true, targets);
    }
}

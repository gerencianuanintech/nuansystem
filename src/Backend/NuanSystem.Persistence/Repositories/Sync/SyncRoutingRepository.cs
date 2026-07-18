using System.Data;
using System.Text.Json;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class SyncRoutingRepository(IMasterConnectionFactory connectionFactory) : ISyncRoutingRepository
{
    public async Task<IReadOnlyCollection<SyncRoutingTargetDto>> ResolveTargetsAsync(
        SyncRoutingContext context,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncRoutingTargetDto>(new CommandDefinition(
            "SP_NA_GET_SYNCROUTINGTARGETS",
            new
            {
                context.SourceCompanyId,
                EntityCode = context.EntityCode,
                context.SyncProfileId,
                context.TargetBranchCode,
                context.RequireTargetBranchMatch,
                context.EntityGlobalId
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return rows.AsList();
    }

    public async Task RecordDecisionAsync(
        long outboxId,
        Guid entityGlobalId,
        SyncDistributionDecisionDto decision,
        CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.SP_NA_POST_SYNCDISTRIBUTIONDECISIONREGISTRAR",
            new
            {
                OutboxId = outboxId,
                decision.SyncProfileEntityBranchId,
                decision.BranchCompanyId,
                EntityGlobalId = entityGlobalId,
                decision.DistributionMode,
                decision.Matched,
                decision.Reason,
                decision.RuleVersion
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<SyncRoutingConflictDto>> FindActiveConflictsAsync(
        int? profileId,
        int companyId,
        IReadOnlyCollection<SyncRoutingConflictCheckItem> combinations,
        CancellationToken cancellationToken = default)
    {
        if (combinations.Count == 0)
        {
            return [];
        }

        var combinationsJson = JsonSerializer.Serialize(combinations.Select(item => new
        {
            item.EntityCode,
            item.BranchCompanyId
        }));

        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SyncRoutingConflictDto>(new CommandDefinition(
            "SP_NA_GET_SYNCPROFILEACTIVECONFLICTS",
            new
            {
                ProfileId = profileId,
                CompanyId = companyId,
                CombinationsJson = combinationsJson
            },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return rows.AsList();
    }
}

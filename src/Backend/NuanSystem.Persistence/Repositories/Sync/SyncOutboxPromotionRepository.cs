using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class SyncOutboxPromotionRepository(IMasterConnectionFactory connectionFactory)
    : ISyncOutboxPromotionRepository
{
    public async Task<SyncOutboxPromotionResult> PromoteAsync(
        SyncOutboxPromotionData data,
        CancellationToken cancellationToken = default)
    {
        await using var connection = (SqlConnection)connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            var existing = await connection.QuerySingleOrDefaultAsync<ExistingOutbox>(new CommandDefinition(
                """
SELECT Id,CompanyId,CausationEventId,EntityName,EntityGlobalId,Operation,PayloadJson
FROM dbo.SyncOutbox WITH (UPDLOCK,HOLDLOCK)
WHERE EventId=@EventId;
""",
                new { data.Event.EventId },
                transaction,
                cancellationToken: cancellationToken));

            if (existing is not null)
            {
                var existingTargets = (await connection.QueryAsync<int>(new CommandDefinition(
                    "SELECT BranchCompanyId FROM dbo.SyncOutboxTargets WHERE OutboxId=@OutboxId ORDER BY BranchCompanyId;",
                    new { OutboxId = existing.Id },
                    transaction,
                    cancellationToken: cancellationToken))).ToArray();
                var requestedTargets = data.Targets
                    .Select(target => target.BranchCompanyId)
                    .Distinct()
                    .OrderBy(companyId => companyId)
                    .ToArray();
                var matches = existing.CompanyId == data.Event.CompanyId
                    && existing.CausationEventId == data.Event.CausationEventId
                    && existingTargets.SequenceEqual(requestedTargets)
                    && existing.EntityGlobalId == data.Event.EntityGlobalId
                    && string.Equals(existing.EntityName, data.Event.EntityName, StringComparison.Ordinal)
                    && string.Equals(existing.Operation, data.Event.Operation.ToString(), StringComparison.Ordinal)
                    && string.Equals(existing.PayloadJson, data.Event.PayloadJson, StringComparison.Ordinal);

                await transaction.RollbackAsync(cancellationToken);
                return matches
                    ? new SyncOutboxPromotionResult(SyncOutboxPromotionStatus.Existing, existing.Id, "EventId ya promovido con el mismo contrato.")
                    : new SyncOutboxPromotionResult(SyncOutboxPromotionStatus.Conflict, existing.Id, "EventId existente con contenido diferente.");
            }

            var outboxId = await connection.ExecuteScalarAsync<long>(new CommandDefinition(
                """
INSERT dbo.SyncOutbox
    (EventId,CompanyId,CausationEventId,EntityName,EntityGlobalId,EntityCode,Operation,PayloadJson,
     SourceSystem,SourceReference,MaxAttempts)
VALUES
    (@EventId,@CompanyId,@CausationEventId,@EntityName,@EntityGlobalId,@EntityCode,@Operation,@PayloadJson,
     N'LocalOutbox',@SourceReference,@MaxAttempts);
SELECT CAST(SCOPE_IDENTITY() AS bigint);
""",
                new
                {
                    data.Event.EventId,
                    data.Event.CompanyId,
                    data.Event.CausationEventId,
                    data.Event.EntityName,
                    data.Event.EntityGlobalId,
                    data.Event.EntityCode,
                    Operation = data.Event.Operation.ToString(),
                    data.Event.PayloadJson,
                    SourceReference = data.Event.Id.ToString(),
                    data.Event.MaxAttempts
                },
                transaction,
                cancellationToken: cancellationToken));

            foreach (var decision in data.Decisions)
            {
                await connection.ExecuteAsync(new CommandDefinition(
                    """
INSERT dbo.SyncDistributionDecisionLog
    (OutboxId,SyncProfileEntityBranchId,BranchCompanyId,EntityGlobalId,DistributionMode,Matched,Reason,RuleVersion)
VALUES
    (@OutboxId,@SyncProfileEntityBranchId,@BranchCompanyId,@EntityGlobalId,@DistributionMode,@Matched,@Reason,@RuleVersion);
""",
                    new
                    {
                        OutboxId = outboxId,
                        decision.SyncProfileEntityBranchId,
                        decision.BranchCompanyId,
                        data.Event.EntityGlobalId,
                        decision.DistributionMode,
                        decision.Matched,
                        Reason = decision.Reason.Length > 500 ? decision.Reason[..500] : decision.Reason,
                        decision.RuleVersion
                    },
                    transaction,
                    cancellationToken: cancellationToken));
            }

            foreach (var target in data.Targets)
            {
                await connection.ExecuteAsync(new CommandDefinition(
                    """
INSERT dbo.SyncOutboxTargets (OutboxId,BranchCompanyId,MaxAttempts)
VALUES (@OutboxId,@BranchCompanyId,@MaxAttempts);
""",
                    new { OutboxId = outboxId, target.BranchCompanyId, MaxAttempts = target.MaxRetries },
                    transaction,
                    cancellationToken: cancellationToken));
            }

            await connection.ExecuteAsync(new CommandDefinition(
                """
INSERT dbo.SyncAudit
    (CompanyId,EventId,EntityName,EntityGlobalId,[Action],NewStatus,[Message],CreatedBy)
VALUES
    (@CompanyId,@EventId,@EntityName,@EntityGlobalId,N'Created',N'Pending',
     N'Evento promovido atomicamente desde LocalOutbox.',@CreatedBy);
""",
                new
                {
                    data.Event.CompanyId,
                    data.Event.EventId,
                    data.Event.EntityName,
                    data.Event.EntityGlobalId,
                    CreatedBy = data.WorkerInstance
                },
                transaction,
                cancellationToken: cancellationToken));

            await transaction.CommitAsync(cancellationToken);
            return new SyncOutboxPromotionResult(SyncOutboxPromotionStatus.Created, outboxId, "Evento promovido.");
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private sealed record ExistingOutbox(
        long Id,
        int CompanyId,
        Guid? CausationEventId,
        string EntityName,
        Guid EntityGlobalId,
        string Operation,
        string PayloadJson);
}

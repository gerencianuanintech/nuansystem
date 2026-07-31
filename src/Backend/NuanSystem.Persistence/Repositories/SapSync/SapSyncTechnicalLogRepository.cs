using Dapper;
using System.Data;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncTechnicalLogRepository(ITenantConnectionFactory connectionFactory) : ISapSyncTechnicalLogRepository
{
    public async Task<long> WriteAsync(SapSyncLogWriteDto log, CancellationToken cancellationToken = default)
    {
        const string procedure = "dbo.SP_NA_POST_SAPSYNCTECHNICALLOGCREAR";
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(new CommandDefinition(procedure, new
        {
            log.CompanyId,
            log.CompanyCode,
            log.EntityCode,
            Direction = log.Direction.ToString(),
            Operation = log.Operation.ToString(),
            Status = log.Status.ToString(),
            log.CorrelationId,
            log.WorkerInstance,
            log.AttemptCount,
            log.QueueItemId,
            log.LocalEntityId,
            log.SapEntityId,
            log.SapDocEntry,
            log.SapDocNum,
            log.RequestJson,
            log.ResponseJson,
            log.ErrorCode,
            log.ErrorMessage,
            log.DurationMs,
            log.StartedAtUtc,
            log.FinishedAtUtc
        }, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken));
    }
}

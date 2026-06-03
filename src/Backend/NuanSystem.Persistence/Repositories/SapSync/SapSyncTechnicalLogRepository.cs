using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncTechnicalLogRepository(ITenantConnectionFactory connectionFactory) : ISapSyncTechnicalLogRepository
{
    public async Task<long> WriteAsync(SapSyncLogWriteDto log, CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT INTO dbo.SapSyncTechnicalLog (CompanyId, CompanyCode, EntityCode, Direction, Operation, Status, CorrelationId, WorkerInstance, AttemptCount, QueueItemId, LocalEntityId, SapEntityId, SapDocEntry, SapDocNum, RequestJson, ResponseJson, ErrorCode, ErrorMessage, DurationMs, StartedAtUtc, FinishedAtUtc)
OUTPUT INSERTED.Id
VALUES (@CompanyId, @CompanyCode, @EntityCode, @Direction, @Operation, @Status, @CorrelationId, @WorkerInstance, @AttemptCount, @QueueItemId, @LocalEntityId, @SapEntityId, @SapDocEntry, @SapDocNum, @RequestJson, @ResponseJson, @ErrorCode, @ErrorMessage, @DurationMs, @StartedAtUtc, @FinishedAtUtc);
""";
        using var connection = connectionFactory.CreateConnection();
        return await connection.ExecuteScalarAsync<long>(new CommandDefinition(sql, new
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
        }, cancellationToken: cancellationToken));
    }
}

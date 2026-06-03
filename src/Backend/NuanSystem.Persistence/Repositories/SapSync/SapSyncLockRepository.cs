using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Application.Features.SapSync.Enums;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class SapSyncLockRepository(ITenantConnectionFactory connectionFactory) : ISapSyncLockRepository
{
    public async Task<SapSyncLockDto?> TryAcquireAsync(int companyId, string entityCode, SapSyncDirection direction, string workerInstance, string correlationId, DateTime expiresAtUtc, CancellationToken cancellationToken = default)
    {
        const string sql = """
DELETE FROM dbo.SapSyncLock WHERE CompanyId=@CompanyId AND EntityCode=@EntityCode AND Direction=@Direction AND ExpiresAt <= SYSUTCDATETIME();
IF NOT EXISTS (SELECT 1 FROM dbo.SapSyncLock WITH (UPDLOCK, HOLDLOCK) WHERE CompanyId=@CompanyId AND EntityCode=@EntityCode AND Direction=@Direction)
BEGIN
    INSERT INTO dbo.SapSyncLock (CompanyId, EntityCode, Direction, WorkerInstance, CorrelationId, ExpiresAt)
    OUTPUT INSERTED.Id, INSERTED.CompanyId, INSERTED.EntityCode, INSERTED.Direction, INSERTED.WorkerInstance, INSERTED.CorrelationId, INSERTED.LockedAt AS LockedAtUtc, INSERTED.ExpiresAt AS ExpiresAtUtc
    VALUES (@CompanyId, @EntityCode, @Direction, @WorkerInstance, @CorrelationId, @ExpiresAtUtc);
END;
""";
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SapSyncLockDto>(new CommandDefinition(sql, new { CompanyId = companyId, EntityCode = entityCode, Direction = direction.ToString(), WorkerInstance = workerInstance, CorrelationId = correlationId, ExpiresAtUtc = expiresAtUtc }, cancellationToken: cancellationToken));
    }

    public async Task ReleaseAsync(long id, string workerInstance, string correlationId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition("DELETE FROM dbo.SapSyncLock WHERE Id=@Id AND WorkerInstance=@WorkerInstance AND CorrelationId=@CorrelationId;", new { Id = id, WorkerInstance = workerInstance, CorrelationId = correlationId }, cancellationToken: cancellationToken));
    }
}

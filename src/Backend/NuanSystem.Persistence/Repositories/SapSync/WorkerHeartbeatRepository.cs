using Dapper;
using NuanSystem.Application.Abstractions.SapSync;
using NuanSystem.Application.Features.SapSync.Dtos;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Repositories.SapSync;

public sealed class WorkerHeartbeatRepository(MasterConnectionFactory connectionFactory) : IWorkerHeartbeatRepository
{
    public async Task UpsertAsync(WorkerHeartbeatDto heartbeat, CancellationToken cancellationToken = default)
    {
        const string sql = """
MERGE dbo.WorkerHeartbeat AS target
USING (SELECT @InstanceName AS InstanceName) AS source
ON target.InstanceName = source.InstanceName
WHEN MATCHED THEN
    UPDATE SET CompanyId=@CompanyId, CompanyCode=@CompanyCode, LastBeatAt=@LastBeatAtUtc, Status=@Status, CurrentJob=@CurrentJob, WorkerVersion=@WorkerVersion, UpdatedAt=SYSUTCDATETIME()
WHEN NOT MATCHED THEN
    INSERT (InstanceName, CompanyId, CompanyCode, LastBeatAt, Status, CurrentJob, WorkerVersion)
    VALUES (@InstanceName, @CompanyId, @CompanyCode, @LastBeatAtUtc, @Status, @CurrentJob, @WorkerVersion);
""";
        await using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(sql, heartbeat, cancellationToken: cancellationToken));
    }
}

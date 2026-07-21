using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Operations;
using NuanSystem.Application.Features.Operations;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Repositories.Operations;

public sealed class WorkerHeartbeatRepository(MasterConnectionFactory connectionFactory) : IWorkerHeartbeatRepository
{
    public async Task UpsertAsync(WorkerHeartbeatDto heartbeat, CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.ExecuteAsync(new CommandDefinition(
            "dbo.SP_NA_POST_WORKERHEARTBEAT_REGISTRAR",
            heartbeat,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));
    }

    public async Task<IReadOnlyCollection<WorkerHeartbeatSnapshotDto>> GetByWorkerTypeAsync(string workerType,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<WorkerHeartbeatSnapshotDto>(new CommandDefinition(
            "dbo.SP_NA_GET_WORKERHEARTBEAT_LISTARPORCONFIGURACION",
            new { WorkerType = workerType },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));
        return rows.AsList();
    }
}

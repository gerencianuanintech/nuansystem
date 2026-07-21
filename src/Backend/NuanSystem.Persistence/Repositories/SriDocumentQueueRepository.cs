using System.Data;
using Dapper;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.SriDocuments.Dtos;

namespace NuanSystem.Persistence.Repositories;

public sealed class SriDocumentQueueRepository(ITenantConnectionFactory connectionFactory) : ISriDocumentQueueRepository
{
    public async Task<SriDocumentQueuePersistenceResult> EnqueueAsync(EnqueueSriDocumentData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var row = await connection.QuerySingleAsync<SriEnqueueRow>(Command("dbo.SP_NA_POST_SRIDOCUMENTQUEUE_ENCOLAR", data, cancellationToken));
        return new SriDocumentQueuePersistenceResult(row, row.IsCreated);
    }

    public async Task<IReadOnlyCollection<SriDocumentQueueListItemDto>> SearchAsync(SriDocumentQueueFilter filter, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SriDocumentQueueListItemDto>(Command("dbo.SP_NA_GET_SRIDOCUMENTQUEUE_LISTAR", filter, cancellationToken));
        return rows.AsList();
    }

    public async Task<SriDocumentQueueDetailDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SriDocumentQueueDetailDto>(Command("dbo.SP_NA_GET_SRIDOCUMENTQUEUE_BUSCARPORID", new { Id = id }, cancellationToken));
    }

    public async Task<IReadOnlyCollection<SriDocumentAttemptDto>> GetAttemptsAsync(long queueId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SriDocumentAttemptDto>(Command("dbo.SP_NA_GET_SRIDOCUMENTQUEUE_INTENTOS", new { QueueId = queueId }, cancellationToken));
        return rows.AsList();
    }

    public Task<SriDocumentQueueActionCode> CancelAsync(SriDocumentQueueActionData data, CancellationToken cancellationToken = default) =>
        ExecuteActionAsync("dbo.SP_NA_PATCH_SRIDOCUMENTQUEUE_CANCELAR", data, cancellationToken);

    public Task<SriDocumentQueueActionCode> ReprocessAsync(SriDocumentQueueActionData data, CancellationToken cancellationToken = default) =>
        ExecuteActionAsync("dbo.SP_NA_PATCH_SRIDOCUMENTQUEUE_REPROCESAR", data, cancellationToken);

    private async Task<SriDocumentQueueActionCode> ExecuteActionAsync(string procedure, SriDocumentQueueActionData data, CancellationToken cancellationToken)
    {
        using var connection = connectionFactory.CreateConnection();
        var code = await connection.ExecuteScalarAsync<int>(Command(procedure, data, cancellationToken));
        return Enum.IsDefined(typeof(SriDocumentQueueActionCode), code) ? (SriDocumentQueueActionCode)code : SriDocumentQueueActionCode.NotFound;
    }

    private static CommandDefinition Command(string name, object parameters, CancellationToken cancellationToken) =>
        new(name, parameters, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);

    private sealed class SriEnqueueRow : SriDocumentQueueDetailDto
    {
        public bool IsCreated { get; set; }
    }
}

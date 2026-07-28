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

    public async Task<SriDocumentMonitorSummaryDto> GetMonitorSummaryAsync(long? importId = null, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleAsync<SriDocumentMonitorSummaryDto>(Command("dbo.SP_NA_GET_SRIDOCUMENTMONITOR_RESUMEN", new { ImportId = importId }, cancellationToken));
    }

    public async Task<IReadOnlyCollection<SriDocumentMonitorListItemDto>> SearchMonitorAsync(SriDocumentMonitorFilter filter, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<SriDocumentMonitorListItemDto>(Command("dbo.SP_NA_GET_SRIDOCUMENTMONITOR_LISTAR", filter, cancellationToken))).AsList();
    }

    public async Task<SriDocumentMonitorDetailDto?> GetMonitorDetailAsync(long queueId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<SriDocumentMonitorDetailDto>(Command("dbo.SP_NA_GET_SRIDOCUMENTMONITOR_BUSCARPORID", new { QueueId = queueId }, cancellationToken));
    }

    public async Task<IReadOnlyCollection<SriDocumentAuditDto>> GetAuditAsync(long queueId, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        return (await connection.QueryAsync<SriDocumentAuditDto>(Command("dbo.SP_NA_GET_SRIDOCUMENTMONITOR_AUDITORIA", new { QueueId = queueId }, cancellationToken))).AsList();
    }

    public async Task<SriAuthorizedXmlPersistenceResult> DownloadAuthorizedXmlAsync(SriAuthorizedXmlDownloadData data, CancellationToken cancellationToken = default)
    {
        using var connection = connectionFactory.CreateConnection();
        var row = await connection.QuerySingleAsync<SriDownloadRow>(Command("dbo.SP_NA_POST_SRIDOCUMENTAUTORIZADO_DESCARGAR", data, cancellationToken));
        return new SriAuthorizedXmlPersistenceResult((SriAuthorizedXmlDownloadCode)row.ResultCode, row.DocumentId, data.QueueId, row.XmlContent ?? [], row.ContentType, row.SizeBytes);
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

    private sealed class SriDownloadRow
    {
        public int ResultCode { get; set; }
        public long? DocumentId { get; set; }
        public byte[]? XmlContent { get; set; }
        public string? ContentType { get; set; }
        public int SizeBytes { get; set; }
    }
}

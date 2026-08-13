using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.UnitMeasures.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class UnitMeasureSyncApplyRepository(ICompanyResolver companyResolver) : IUnitMeasureSyncApplyRepository
{
    private const string ApplyProcedure = "dbo.SP_NA_POST_UNIT_OF_MEASURE_SYNC_APPLY";

    public async Task<UnitMeasureSyncApplyResult> ApplyAsync(int branchCompanyId,
        SyncEventApplyContext context, UnitMeasureSyncPayload payload, SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        var company = await companyResolver.ResolveByIdAsync(branchCompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontro la sucursal destino {branchCompanyId}.");
        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
            throw new NotSupportedException($"El motor {company.DatabaseEngine} no esta implementado para Sync UnitOfMeasure.");

        await using var connection = new SqlConnection(company.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            var inbox = await GetInboxAsync(connection, transaction, context.EventId, cancellationToken);
            if (inbox?.Status == SyncEventStatus.Applied.ToString())
            {
                await transaction.CommitAsync(cancellationToken);
                return new(true, true, false, null, "Evento ya aplicado en SyncInbox.");
            }
            if (inbox?.Status == SyncEventStatus.DeadLetter.ToString())
            {
                await transaction.CommitAsync(cancellationToken);
                return new(false, false, true, null,
                    inbox.LastErrorMessage ?? "Evento ya clasificado como conflicto terminal.",
                    "SYNC_UNIT_OF_MEASURE_CODE_CONFLICT");
            }

            var inboxId = inbox?.Id ?? await InsertInboxAsync(connection, transaction, context, cancellationToken);
            var isDeleted = operation == SyncOperation.Deleted || payload.IsDeleted;
            var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;
            var apply = await connection.QuerySingleAsync<ApplyRow>(new CommandDefinition(
                ApplyProcedure, new
                {
                    payload.GlobalId,
                    Code = Required(payload.Code, nameof(payload.Code), 50),
                    Name = Required(payload.Name, nameof(payload.Name), 150),
                    Description = Optional(payload.Description, nameof(payload.Description), 500),
                    Symbol = Optional(payload.Symbol, nameof(payload.Symbol), 20),
                    MagnitudeCode = Required(payload.MagnitudeCode, nameof(payload.MagnitudeCode), 20),
                    payload.SortOrder,
                    IsActive = isActive,
                    IsDeleted = isDeleted,
                    UpdatedAt = payload.UpdatedAt == default ? DateTime.UtcNow : payload.UpdatedAt
                }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

            if (apply.ResultCode == -2)
            {
                const string message = "El codigo de UnitOfMeasure pertenece a otro GlobalId; no se realizo adopcion automatica.";
                await MarkDeadLetterAsync(connection, transaction, inboxId, message, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return new(false, false, true, null, message, "SYNC_UNIT_OF_MEASURE_CODE_CONFLICT");
            }
            if (apply.ResultCode <= 0)
                throw new InvalidOperationException("El procedimiento de sincronizacion UnitOfMeasure no aplico el evento.");

            await MarkAppliedAsync(connection, transaction, inboxId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new(true, false, false, apply.UnitMeasureId,
                $"Unidad de medida sincronizada por GlobalId {payload.GlobalId}.");
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            await RecordErrorAsync(connection, context, exception.Message, CancellationToken.None);
            throw;
        }
    }

    private static Task<InboxState?> GetInboxAsync(SqlConnection connection, IDbTransaction transaction,
        Guid eventId, CancellationToken ct) => connection.QuerySingleOrDefaultAsync<InboxState>(new CommandDefinition(
        "SELECT TOP (1) Id,Status,LastErrorMessage FROM dbo.SyncInbox WITH (UPDLOCK,HOLDLOCK) WHERE EventId=@EventId;",
        new { EventId = eventId }, transaction, cancellationToken: ct));

    private static Task<long> InsertInboxAsync(SqlConnection connection, IDbTransaction transaction,
        SyncEventApplyContext context, CancellationToken ct) => connection.ExecuteScalarAsync<long>(new CommandDefinition("""
        INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
        VALUES(@EventId,@SourceCompanyId,@EntityName,@EntityGlobalId,@Operation,@PayloadJson,N'Pending');
        SELECT CAST(SCOPE_IDENTITY() AS bigint);
        """, context, transaction, cancellationToken: ct));

    private static Task MarkAppliedAsync(SqlConnection connection, IDbTransaction transaction, long id, CancellationToken ct) =>
        connection.ExecuteAsync(new CommandDefinition("""
        UPDATE dbo.SyncInbox SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,
        LastErrorMessage=NULL,NextRetryAt=NULL WHERE Id=@Id;
        """, new { Id = id }, transaction, cancellationToken: ct));

    private static Task MarkDeadLetterAsync(SqlConnection connection, IDbTransaction transaction,
        long id, string message, CancellationToken ct) => connection.ExecuteAsync(new CommandDefinition("""
        UPDATE dbo.SyncInbox SET Status=N'DeadLetter',ErrorMessage=@Message,LastErrorMessage=@Message,
        NextRetryAt=NULL WHERE Id=@Id;
        """, new { Id = id, Message = message }, transaction, cancellationToken: ct));

    private static Task RecordErrorAsync(SqlConnection connection, SyncEventApplyContext context,
        string message, CancellationToken ct) => connection.ExecuteAsync(new CommandDefinition("""
        IF EXISTS(SELECT 1 FROM dbo.SyncInbox WHERE EventId=@EventId)
          UPDATE dbo.SyncInbox SET Status=N'Error',AttemptCount=AttemptCount+1,ErrorMessage=@Message,
          LastErrorMessage=@Message,NextRetryAt=DATEADD(second,30,SYSUTCDATETIME())
          WHERE EventId=@EventId AND Status NOT IN(N'Applied',N'DeadLetter');
        ELSE
          INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,
          Status,AttemptCount,ErrorMessage,LastErrorMessage,NextRetryAt)
          VALUES(@EventId,@SourceCompanyId,@EntityName,@EntityGlobalId,@Operation,@PayloadJson,
          N'Error',1,@Message,@Message,DATEADD(second,30,SYSUTCDATETIME()));
        """, new
        {
            context.EventId, context.SourceCompanyId, context.EntityName, context.EntityGlobalId,
            context.Operation, context.PayloadJson, Message = message
        }, cancellationToken: ct));

    private static string Required(string value, string field, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException($"{field} es requerido para sincronizar UnitOfMeasure.");
        var normalized = value.Trim();
        if (normalized.Length > max) throw new InvalidOperationException($"{field} excede la longitud permitida.");
        return normalized;
    }

    private static string? Optional(string? value, string field, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        if (normalized.Length > max) throw new InvalidOperationException($"{field} excede la longitud permitida.");
        return normalized;
    }

    private sealed record InboxState(long Id, string Status, string? LastErrorMessage);
    private sealed class ApplyRow { public int ResultCode { get; set; } public int? UnitMeasureId { get; set; } }
}

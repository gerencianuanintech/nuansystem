using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class ItemOriginSyncApplyRepository(ICompanyResolver companyResolver) : IItemOriginSyncApplyRepository
{
    public async Task<ItemOriginSyncApplyResult> ApplyAsync(int branchCompanyId,
        SyncEventApplyContext context, ItemOriginSyncPayload payload, SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        var company = await companyResolver.ResolveByIdAsync(branchCompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontro la sucursal destino {branchCompanyId}.");
        if (company.DatabaseEngine != DatabaseEngine.SqlServer)
            throw new NotSupportedException($"El motor {company.DatabaseEngine} no esta implementado para Sync ItemOrigin.");

        await using var connection = new SqlConnection(company.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            var inbox = await connection.QuerySingleOrDefaultAsync<InboxState>(new CommandDefinition(
                "SELECT TOP (1) Id,Status,LastErrorMessage FROM dbo.SyncInbox WITH (UPDLOCK,HOLDLOCK) WHERE EventId=@EventId;",
                new { context.EventId }, transaction, cancellationToken: cancellationToken));
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
                    "SYNC_ITEM_ORIGIN_CODE_CONFLICT");
            }

            var inboxId = inbox?.Id ?? await connection.ExecuteScalarAsync<long>(new CommandDefinition("""
                INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
                VALUES(@EventId,@SourceCompanyId,@EntityName,@EntityGlobalId,@Operation,@PayloadJson,N'Pending');
                SELECT CAST(SCOPE_IDENTITY() AS bigint);
                """, context, transaction, cancellationToken: cancellationToken));
            var isDeleted = operation == SyncOperation.Deleted || payload.IsDeleted;
            var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;
            var apply = await connection.QuerySingleAsync<ApplyRow>(new CommandDefinition(
                "dbo.SP_NA_POST_ITEM_ORIGIN_SYNC_APPLY", new
                {
                    payload.GlobalId,
                    Code = Required(payload.Code, nameof(payload.Code), 50),
                    Name = Required(payload.Name, nameof(payload.Name), 150),
                    Description = Optional(payload.Description, nameof(payload.Description), 500),
                    payload.SortOrder,
                    IsActive = isActive,
                    IsDeleted = isDeleted,
                    UpdatedAt = payload.UpdatedAt == default ? DateTime.UtcNow : payload.UpdatedAt
                }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

            if (apply.ResultCode == -2)
            {
                const string message = "El codigo de ItemOrigin pertenece a otro GlobalId; no se realizo adopcion automatica.";
                await connection.ExecuteAsync(new CommandDefinition("""
                    UPDATE dbo.SyncInbox SET Status=N'DeadLetter',ErrorMessage=@Message,
                    LastErrorMessage=@Message,NextRetryAt=NULL WHERE Id=@Id;
                    """, new { Id = inboxId, Message = message }, transaction,
                    cancellationToken: cancellationToken));
                await transaction.CommitAsync(cancellationToken);
                return new(false, false, true, null, message, "SYNC_ITEM_ORIGIN_CODE_CONFLICT");
            }
            if (apply.ResultCode <= 0)
                throw new InvalidOperationException("El procedimiento de sincronizacion ItemOrigin no aplico el evento.");

            await connection.ExecuteAsync(new CommandDefinition("""
                UPDATE dbo.SyncInbox SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,
                LastErrorMessage=NULL,NextRetryAt=NULL WHERE Id=@Id;
                """, new { Id = inboxId }, transaction, cancellationToken: cancellationToken));
            await transaction.CommitAsync(cancellationToken);
            return new(true, false, false, apply.ItemOriginId,
                $"Origen de articulo sincronizado por GlobalId {payload.GlobalId}.");
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            await connection.ExecuteAsync(new CommandDefinition("""
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
                context.Operation, context.PayloadJson, Message = exception.Message
            }, cancellationToken: CancellationToken.None));
            throw;
        }
    }

    private static string Required(string value, string field, int max)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException($"{field} es requerido para sincronizar ItemOrigin.");
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
    private sealed class ApplyRow { public int ResultCode { get; set; } public int? ItemOriginId { get; set; } }
}

using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class ItemSubgroupSyncApplyRepository(ICompanyResolver companyResolver) : IItemSubgroupSyncApplyRepository
{
    private const string ApplyProcedure = "dbo.SP_NA_POST_ITEM_SUBGROUP_SYNC_APPLY";

    public async Task<bool> ItemFamilyExistsAsync(int branchCompanyId, Guid itemFamilyGlobalId,
        CancellationToken cancellationToken = default)
    {
        var company = await ResolveBranchAsync(branchCompanyId, cancellationToken);
        await using var connection = CreateSqlConnection(company);
        await connection.OpenAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            "SELECT COUNT(1) FROM dbo.ItemFamilies WHERE GlobalId=@ItemFamilyGlobalId AND IsDeleted=0;",
            new { ItemFamilyGlobalId = itemFamilyGlobalId }, cancellationToken: cancellationToken)) > 0;
    }

    public async Task<ItemSubgroupSyncApplyResult> ApplyAsync(int branchCompanyId,
        SyncEventApplyContext context, ItemSubgroupSyncPayload payload, SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        var company = await ResolveBranchAsync(branchCompanyId, cancellationToken);
        await using var connection = CreateSqlConnection(company);
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
                    "SYNC_ITEM_SUBGROUP_CODE_CONFLICT");
            }

            var inboxId = inbox?.Id ?? await InsertInboxAsync(connection, transaction, context, cancellationToken);
            var isDeleted = operation == SyncOperation.Deleted || payload.IsDeleted;
            var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;
            var apply = await connection.QuerySingleAsync<ApplyRow>(new CommandDefinition(ApplyProcedure, new
            {
                payload.GlobalId,
                payload.ItemFamilyGlobalId,
                Code = Required(payload.Code, nameof(payload.Code), 50),
                Name = Required(payload.Name, nameof(payload.Name), 150),
                Description = Optional(payload.Description, nameof(payload.Description), 500),
                payload.SortOrder,
                IsActive = isActive,
                IsDeleted = isDeleted,
                CreatedAt = payload.CreatedAt == default ? DateTime.UtcNow : payload.CreatedAt,
                UpdatedAt = payload.UpdatedAt ?? DateTime.UtcNow
            }, transaction, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure));

            if (apply.ResultCode == -2)
            {
                const string message = "El código de ItemSubgroup pertenece a otro GlobalId dentro de la familia; no se realizó adopción automática.";
                await MarkDeadLetterAsync(connection, transaction, inboxId, message, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return new(false, false, true, null, message, "SYNC_ITEM_SUBGROUP_CODE_CONFLICT");
            }
            if (apply.ResultCode <= 0)
                throw new InvalidOperationException("El procedimiento de sincronización ItemSubgroup no aplicó el evento.");

            await MarkAppliedAsync(connection, transaction, inboxId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return new(true, false, false, apply.ItemSubgroupId,
                $"Subgrupo sincronizado por GlobalId {payload.GlobalId}.");
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            await RecordErrorAsync(connection, context, exception.Message, CancellationToken.None);
            throw;
        }
    }

    private async Task<CompanyConnectionInfo> ResolveBranchAsync(int branchCompanyId, CancellationToken cancellationToken) =>
        await companyResolver.ResolveByIdAsync(branchCompanyId, cancellationToken)
        ?? throw new InvalidOperationException($"No se encontró la sucursal destino {branchCompanyId}.");

    private static SqlConnection CreateSqlConnection(CompanyConnectionInfo company) => company.DatabaseEngine == DatabaseEngine.SqlServer
        ? new SqlConnection(company.ConnectionString)
        : throw new NotSupportedException($"El motor {company.DatabaseEngine} no está implementado para Sync ItemSubgroup.");

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
        if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException($"{field} es requerido para sincronizar ItemSubgroup.");
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
    private sealed class ApplyRow { public int ResultCode { get; set; } public int? ItemSubgroupId { get; set; } }
}

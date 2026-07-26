using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class ItemGroupSyncApplyRepository(ICompanyResolver companyResolver) : IItemGroupSyncApplyRepository
{
    private const string ExistsProcedure = "dbo.SP_NA_GET_ITEM_GROUP_SYNC_EXISTS_BY_GLOBAL_ID";
    private const string ApplyProcedure = "dbo.SP_NA_POST_ITEM_GROUP_SYNC_APPLY";

    public async Task<bool> ExistsByGlobalIdAsync(
        int branchCompanyId,
        Guid globalId,
        CancellationToken cancellationToken = default)
    {
        var company = await ResolveBranchAsync(branchCompanyId, cancellationToken);
        await using var connection = CreateSqlConnection(company);
        await connection.OpenAsync(cancellationToken);

        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            ExistsProcedure,
            new { GlobalId = globalId },
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));
        return count > 0;
    }

    public Task<ItemGroupSyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemGroupSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        return ApplyAsync(branchCompanyId, context, payload, operation, markDeleted: false, cancellationToken);
    }

    public Task<ItemGroupSyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemGroupSyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default)
    {
        return ApplyAsync(branchCompanyId, context, payload, SyncOperation.Disabled, markDeleted, cancellationToken);
    }

    private async Task<ItemGroupSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemGroupSyncPayload payload,
        SyncOperation operation,
        bool markDeleted,
        CancellationToken cancellationToken)
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
                return new ItemGroupSyncApplyResult(true, true, false, null, "Evento ya aplicado en SyncInbox.");
            }

            if (inbox?.Status == SyncEventStatus.DeadLetter.ToString())
            {
                await transaction.CommitAsync(cancellationToken);
                return new ItemGroupSyncApplyResult(
                    false,
                    false,
                    true,
                    null,
                    inbox.LastErrorMessage ?? "Evento ya clasificado como conflicto terminal.",
                    "SYNC_ITEM_GROUP_CODE_CONFLICT");
            }

            var inboxId = inbox?.Id ?? await InsertInboxAsync(connection, transaction, context, cancellationToken);
            var applyResult = await UpsertItemGroupAsync(
                connection, transaction, payload, operation, markDeleted, cancellationToken);

            if (applyResult.ResultCode == -2)
            {
                const string conflictMessage =
                    "El codigo de ItemGroup ya pertenece a otro GlobalId en la sucursal; no se realizo adopcion automatica.";
                await MarkInboxDeadLetterAsync(
                    connection, transaction, inboxId, conflictMessage, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return new ItemGroupSyncApplyResult(
                    false,
                    false,
                    true,
                    null,
                    conflictMessage,
                    "SYNC_ITEM_GROUP_CODE_CONFLICT");
            }

            await MarkInboxAppliedAsync(connection, transaction, inboxId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ItemGroupSyncApplyResult(
                true,
                false,
                false,
                applyResult.ItemGroupId,
                $"Grupo de articulos sincronizado por GlobalId {payload.GlobalId}.");
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            await RecordInboxErrorAsync(connection, context, exception.Message, CancellationToken.None);
            throw;
        }
    }

    private static async Task<InboxState?> GetInboxAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        const string sql = """
SELECT TOP (1) Id, Status, LastErrorMessage
FROM dbo.SyncInbox WITH (UPDLOCK, HOLDLOCK)
WHERE EventId = @EventId;
""";

        return await connection.QuerySingleOrDefaultAsync<InboxState>(new CommandDefinition(
            sql,
            new { EventId = eventId },
            transaction,
            cancellationToken: cancellationToken));
    }

    private static async Task<long> InsertInboxAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        SyncEventApplyContext context,
        CancellationToken cancellationToken)
    {
        const string sql = """
INSERT INTO dbo.SyncInbox
(
    EventId, SourceCompanyId, EntityName, EntityGlobalId, Operation, PayloadJson, Status
)
VALUES
(
    @EventId, @SourceCompanyId, @EntityName, @EntityGlobalId, @Operation, @PayloadJson, N'Pending'
);

SELECT CAST(SCOPE_IDENTITY() AS bigint);
""";

        return await connection.ExecuteScalarAsync<long>(new CommandDefinition(
            sql,
            new
            {
                context.EventId,
                context.SourceCompanyId,
                context.EntityName,
                context.EntityGlobalId,
                context.Operation,
                context.PayloadJson
            },
            transaction,
            cancellationToken: cancellationToken));
    }

    private static Task<ItemGroupApplyRow> UpsertItemGroupAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        ItemGroupSyncPayload payload,
        SyncOperation operation,
        bool markDeleted,
        CancellationToken cancellationToken)
    {
        var isDeleted = markDeleted || operation == SyncOperation.Deleted;
        var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;

        return connection.QuerySingleAsync<ItemGroupApplyRow>(new CommandDefinition(
            ApplyProcedure,
            new
            {
                payload.GlobalId,
                Code = NormalizeRequired(payload.Code, "Code", 50),
                Name = NormalizeRequired(payload.Name, "Name", 150),
                Description = NormalizeOptional(payload.Description, 500),
                InventoryAccountCode = NormalizeOptional(payload.InventoryAccountCode, 120),
                CostOfSalesAccountCode = NormalizeOptional(payload.CostOfSalesAccountCode, 120),
                SalesAccountCode = NormalizeOptional(payload.SalesAccountCode, 120),
                PurchaseAccountCode = NormalizeOptional(payload.PurchaseAccountCode, 120),
                SapGroupCode = NormalizeOptional(payload.SapGroupCode, 100),
                SapCode = NormalizeOptional(payload.SapCode, 50),
                IsActive = isActive,
                IsDeleted = isDeleted,
                ExternalSystem = NormalizeOptional(payload.ExternalSystem, 50),
                ExternalCode = NormalizeOptional(payload.ExternalCode, 100),
                CreatedAt = payload.CreatedAt == default ? DateTime.UtcNow : payload.CreatedAt,
                UpdatedAt = payload.UpdatedAt ?? DateTime.UtcNow
            },
            transaction,
            cancellationToken: cancellationToken,
            commandType: CommandType.StoredProcedure));
    }

    private static async Task MarkInboxAppliedAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        long inboxId,
        CancellationToken cancellationToken)
    {
        const string sql = """
UPDATE dbo.SyncInbox
SET Status = N'Applied', AppliedAt = SYSUTCDATETIME(), ErrorMessage = NULL,
    LastErrorMessage = NULL, NextRetryAt = NULL
WHERE Id = @InboxId;
""";

        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { InboxId = inboxId },
            transaction,
            cancellationToken: cancellationToken));
    }

    private static Task MarkInboxDeadLetterAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        long inboxId,
        string message,
        CancellationToken cancellationToken)
    {
        const string sql = """
UPDATE dbo.SyncInbox
SET Status = N'DeadLetter',
    ErrorMessage = @Message,
    LastErrorMessage = @Message,
    NextRetryAt = NULL
WHERE Id = @InboxId;
""";

        return connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { InboxId = inboxId, Message = message },
            transaction,
            cancellationToken: cancellationToken));
    }

    private static async Task RecordInboxErrorAsync(
        SqlConnection connection,
        SyncEventApplyContext context,
        string errorMessage,
        CancellationToken cancellationToken)
    {
        const string sql = """
IF EXISTS (SELECT 1 FROM dbo.SyncInbox WHERE EventId = @EventId)
BEGIN
    UPDATE dbo.SyncInbox
    SET Status = N'Error', AttemptCount = AttemptCount + 1,
        ErrorMessage = @ErrorMessage, LastErrorMessage = @ErrorMessage,
        NextRetryAt = DATEADD(second, 30, SYSUTCDATETIME())
    WHERE EventId = @EventId AND Status NOT IN (N'Applied', N'DeadLetter');
END
ELSE
BEGIN
    INSERT INTO dbo.SyncInbox
    (
        EventId, SourceCompanyId, EntityName, EntityGlobalId, Operation, PayloadJson,
        Status, AttemptCount, ErrorMessage, LastErrorMessage, NextRetryAt
    )
    VALUES
    (
        @EventId, @SourceCompanyId, @EntityName, @EntityGlobalId, @Operation, @PayloadJson,
        N'Error', 1, @ErrorMessage, @ErrorMessage, DATEADD(second, 30, SYSUTCDATETIME())
    );
END;
""";

        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                context.EventId,
                context.SourceCompanyId,
                context.EntityName,
                context.EntityGlobalId,
                context.Operation,
                context.PayloadJson,
                ErrorMessage = errorMessage
            },
            cancellationToken: cancellationToken));
    }

    private async Task<CompanyConnectionInfo> ResolveBranchAsync(int branchCompanyId, CancellationToken cancellationToken)
    {
        return await companyResolver.ResolveByIdAsync(branchCompanyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontro la sucursal destino {branchCompanyId}.");
    }

    private static SqlConnection CreateSqlConnection(CompanyConnectionInfo company)
    {
        return company.DatabaseEngine == DatabaseEngine.SqlServer
            ? new SqlConnection(company.ConnectionString)
            : throw new NotSupportedException($"El motor {company.DatabaseEngine} todavia no esta implementado para Sync ItemGroups.");
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"El campo {fieldName} es requerido para sincronizar ItemGroups.");
        }

        var trimmed = value.Trim();
        return trimmed[..Math.Min(trimmed.Length, maxLength)];
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed[..Math.Min(trimmed.Length, maxLength)];
    }

    private sealed record InboxState(long Id, string Status, string? LastErrorMessage);

    private sealed class ItemGroupApplyRow
    {
        public int ResultCode { get; set; }

        public int? ItemGroupId { get; set; }
    }
}

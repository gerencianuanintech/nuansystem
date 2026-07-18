using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Items.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class ItemSyncApplyRepository(ICompanyResolver companyResolver) : IItemSyncApplyRepository
{
    public async Task<ItemSyncDependencyCheckResult> CheckDependenciesAsync(
        int branchCompanyId,
        ItemSyncPayload payload,
        CancellationToken cancellationToken = default)
    {
        var company = await ResolveBranchAsync(branchCompanyId, cancellationToken);
        await using var connection = CreateSqlConnection(company);
        await connection.OpenAsync(cancellationToken);

        var itemGroupCode = NormalizeOptional(payload.ItemGroupCode, 50);
        if (itemGroupCode is null && payload.ItemGroupId is null)
        {
            return ItemSyncDependencyCheckResult.Satisfied;
        }

        const string sql = """
SELECT COUNT(1)
FROM dbo.ItemGroups
WHERE IsDeleted = 0
  AND ((@Code IS NOT NULL AND Code = @Code)
       OR (@Code IS NULL AND @Id IS NOT NULL AND Id = @Id));
""";

        var exists = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new { Id = payload.ItemGroupId, Code = itemGroupCode },
            cancellationToken: cancellationToken));

        return exists > 0
            ? ItemSyncDependencyCheckResult.Satisfied
            : new ItemSyncDependencyCheckResult(
                false,
                "ItemGroups",
                $"El grupo {itemGroupCode ?? payload.ItemGroupId?.ToString() ?? "sin codigo"} requerido por Item {payload.Code} aun no existe en la sucursal.");
    }

    public async Task<bool> ExistsByGlobalIdAsync(
        int branchCompanyId,
        Guid globalId,
        CancellationToken cancellationToken = default)
    {
        var company = await ResolveBranchAsync(branchCompanyId, cancellationToken);
        await using var connection = CreateSqlConnection(company);
        await connection.OpenAsync(cancellationToken);

        const string sql = """
SELECT COUNT(1)
FROM dbo.Items
WHERE GlobalId = @GlobalId;
""";

        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new { GlobalId = globalId },
            cancellationToken: cancellationToken));
        return count > 0;
    }

    public Task<ItemSyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        return ApplyAsync(branchCompanyId, context, payload, operation, markDeleted: false, cancellationToken);
    }

    public Task<ItemSyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemSyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default)
    {
        return ApplyAsync(branchCompanyId, context, payload, SyncOperation.Disabled, markDeleted, cancellationToken);
    }

    private async Task<ItemSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        ItemSyncPayload payload,
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
                return new ItemSyncApplyResult(true, true, null, "Evento ya aplicado en SyncInbox.");
            }

            var inboxId = inbox?.Id ?? await InsertInboxAsync(connection, transaction, context, cancellationToken);
            var itemId = await UpsertItemAsync(connection, transaction, payload, operation, markDeleted, cancellationToken);

            await MarkInboxAppliedAsync(connection, transaction, inboxId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new ItemSyncApplyResult(true, false, itemId, $"Item sincronizado por GlobalId {payload.GlobalId}.");
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
SELECT TOP (1) Id, Status
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
    EventId,
    SourceCompanyId,
    EntityName,
    EntityGlobalId,
    Operation,
    PayloadJson,
    Status
)
VALUES
(
    @EventId,
    @SourceCompanyId,
    @EntityName,
    @EntityGlobalId,
    @Operation,
    @PayloadJson,
    N'Pending'
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

    private static async Task<int> UpsertItemAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        ItemSyncPayload payload,
        SyncOperation operation,
        bool markDeleted,
        CancellationToken cancellationToken)
    {
        var isDeleted = markDeleted || operation == SyncOperation.Deleted;
        var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;
        var itemGroupId = await ResolveIdByCodeOrIdAsync(connection, transaction, "dbo.ItemGroups", payload.ItemGroupId, payload.ItemGroupCode, cancellationToken);
        var itemFamilyId = await ResolveIdByCodeOrIdAsync(connection, transaction, "dbo.ItemFamilies", payload.ItemFamilyId, payload.ItemFamilyCode, cancellationToken);
        var inventoryUnitOfMeasureId = await ResolveIdByCodeOrIdAsync(connection, transaction, "dbo.UnitOfMeasures", payload.InventoryUnitOfMeasureId, payload.InventoryUnitOfMeasureCode, cancellationToken);

        const string sql = """
DECLARE @ItemId int;

SELECT @ItemId = Id
FROM dbo.Items WITH (UPDLOCK, HOLDLOCK)
WHERE GlobalId = @GlobalId;

IF @ItemId IS NULL
BEGIN
    INSERT INTO dbo.Items
    (
        GlobalId,
        Code,
        Name,
        ExternalSystem,
        ExternalCode,
        SapCode,
        Description,
        ItemGroupId,
        ItemFamilyId,
        ItemType,
        InventoryUnitOfMeasureId,
        PurchaseUnitOfMeasureId,
        SalesUnitOfMeasureId,
        IsPurchaseItem,
        IsSalesItem,
        IsInventoryItem,
        ValuationMethod,
        ManagedBy,
        BatchSerialManagementMethod,
        PurchaseFactor,
        SalesFactor,
        AllowDiscount,
        IsActive,
        CreatedByUserName,
        CreatedAt,
        IsDeleted,
        DeletedByUserName,
        DeletedAt
    )
    VALUES
    (
        @GlobalId,
        @Code,
        @Name,
        @ExternalSystem,
        @ExternalCode,
        @SapCode,
        @Description,
        @ItemGroupId,
        @ItemFamilyId,
        @ItemType,
        @InventoryUnitOfMeasureId,
        @InventoryUnitOfMeasureId,
        @InventoryUnitOfMeasureId,
        @IsPurchaseItem,
        @IsSalesItem,
        @IsInventoryItem,
        N'MovingAverage',
        N'None',
        N'EveryTransaction',
        1,
        1,
        1,
        @IsActive,
        N'MasterBranchSyncWorker',
        SYSUTCDATETIME(),
        @IsDeleted,
        CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END,
        CASE WHEN @IsDeleted = 1 THEN SYSUTCDATETIME() ELSE NULL END
    );

    SET @ItemId = CONVERT(int, SCOPE_IDENTITY());
END
ELSE
BEGIN
    UPDATE dbo.Items
    SET Code = @Code,
        Name = @Name,
        ExternalSystem = @ExternalSystem,
        ExternalCode = @ExternalCode,
        SapCode = @SapCode,
        Description = @Description,
        ItemGroupId = @ItemGroupId,
        ItemFamilyId = @ItemFamilyId,
        ItemType = @ItemType,
        InventoryUnitOfMeasureId = @InventoryUnitOfMeasureId,
        PurchaseUnitOfMeasureId = COALESCE(PurchaseUnitOfMeasureId, @InventoryUnitOfMeasureId),
        SalesUnitOfMeasureId = COALESCE(SalesUnitOfMeasureId, @InventoryUnitOfMeasureId),
        IsPurchaseItem = @IsPurchaseItem,
        IsSalesItem = @IsSalesItem,
        IsInventoryItem = @IsInventoryItem,
        IsActive = @IsActive,
        UpdatedByUserName = N'MasterBranchSyncWorker',
        UpdatedAt = SYSUTCDATETIME(),
        IsDeleted = @IsDeleted,
        DeletedByUserName = CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END,
        DeletedAt = CASE WHEN @IsDeleted = 1 THEN COALESCE(DeletedAt, SYSUTCDATETIME()) ELSE NULL END
    WHERE Id = @ItemId;
END;

SELECT @ItemId;
""";

        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new
            {
                payload.GlobalId,
                Code = NormalizeRequired(payload.Code, "Code", 50),
                Name = NormalizeRequired(payload.Name, "Name", 200),
                ExternalSystem = NormalizeOptional(payload.ExternalSystem, 50),
                ExternalCode = NormalizeOptional(payload.ExternalCode, 100),
                SapCode = NormalizeOptional(payload.SapCode, 100),
                Description = NormalizeOptional(payload.Description, 500),
                ItemGroupId = itemGroupId,
                ItemFamilyId = itemFamilyId,
                ItemType = NormalizeItemType(payload.ItemType),
                InventoryUnitOfMeasureId = inventoryUnitOfMeasureId,
                payload.IsPurchaseItem,
                payload.IsSalesItem,
                payload.IsInventoryItem,
                IsActive = isActive,
                IsDeleted = isDeleted
            },
            transaction,
            cancellationToken: cancellationToken));
    }

    private static async Task<int?> ResolveIdByCodeOrIdAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        string tableName,
        int? id,
        string? code,
        CancellationToken cancellationToken)
    {
        var sql = $"""
SELECT TOP (1) Id
FROM {tableName}
WHERE IsDeleted = 0
  AND ((@Code IS NOT NULL AND Code = @Code)
       OR (@Code IS NULL AND @Id IS NOT NULL AND Id = @Id))
ORDER BY CASE WHEN @Code IS NOT NULL AND Code = @Code THEN 0 ELSE 1 END;
""";

        return await connection.ExecuteScalarAsync<int?>(new CommandDefinition(
            sql,
            new { Id = id, Code = NormalizeOptional(code, 50) },
            transaction,
            cancellationToken: cancellationToken));
    }

    private static async Task MarkInboxAppliedAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        long inboxId,
        CancellationToken cancellationToken)
    {
        const string sql = """
UPDATE dbo.SyncInbox
SET Status = N'Applied',
    AppliedAt = SYSUTCDATETIME(),
    ErrorMessage = NULL,
    LastErrorMessage = NULL,
    NextRetryAt = NULL
WHERE Id = @InboxId;
""";

        await connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { InboxId = inboxId },
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
    SET Status = N'Error',
        AttemptCount = AttemptCount + 1,
        ErrorMessage = @ErrorMessage,
        LastErrorMessage = @ErrorMessage,
        NextRetryAt = DATEADD(second, 30, SYSUTCDATETIME())
    WHERE EventId = @EventId
      AND Status <> N'Applied';
END
ELSE
BEGIN
    INSERT INTO dbo.SyncInbox
    (
        EventId,
        SourceCompanyId,
        EntityName,
        EntityGlobalId,
        Operation,
        PayloadJson,
        Status,
        AttemptCount,
        ErrorMessage,
        LastErrorMessage,
        NextRetryAt
    )
    VALUES
    (
        @EventId,
        @SourceCompanyId,
        @EntityName,
        @EntityGlobalId,
        @Operation,
        @PayloadJson,
        N'Error',
        1,
        @ErrorMessage,
        @ErrorMessage,
        DATEADD(second, 30, SYSUTCDATETIME())
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
            : throw new NotSupportedException($"El motor {company.DatabaseEngine} todavia no esta implementado para Sync Item.");
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"El campo {fieldName} es requerido para sincronizar Item.");
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

    private static string NormalizeItemType(string? itemType)
    {
        return itemType is "Product" or "Service" or "Supply" or "Asset"
            ? itemType
            : "Product";
    }

    private sealed record InboxState(long Id, string Status);
}

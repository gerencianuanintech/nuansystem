using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.GeneralInventory.Warehouses.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class WarehouseSyncApplyRepository(ICompanyResolver companyResolver) : IWarehouseSyncApplyRepository
{
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
FROM dbo.Warehouses
WHERE GlobalId = @GlobalId;
""";

        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new { GlobalId = globalId },
            cancellationToken: cancellationToken));
        return count > 0;
    }

    public Task<WarehouseSyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        WarehouseSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        return ApplyAsync(branchCompanyId, context, payload, operation, markDeleted: false, cancellationToken);
    }

    public Task<WarehouseSyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        WarehouseSyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default)
    {
        return ApplyAsync(branchCompanyId, context, payload, SyncOperation.Disabled, markDeleted, cancellationToken);
    }

    private async Task<WarehouseSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        WarehouseSyncPayload payload,
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
                return new WarehouseSyncApplyResult(true, true, null, "Evento ya aplicado en SyncInbox.");
            }

            var inboxId = inbox?.Id ?? await InsertInboxAsync(connection, transaction, context, cancellationToken);
            var warehouseId = await UpsertWarehouseAsync(connection, transaction, payload, operation, markDeleted, cancellationToken);

            await MarkInboxAppliedAsync(connection, transaction, inboxId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new WarehouseSyncApplyResult(true, false, warehouseId, $"Warehouse sincronizado por GlobalId {payload.GlobalId}.");
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

    private static async Task<int> UpsertWarehouseAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        WarehouseSyncPayload payload,
        SyncOperation operation,
        bool markDeleted,
        CancellationToken cancellationToken)
    {
        var isDeleted = markDeleted || operation == SyncOperation.Deleted;
        var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;

        const string sql = """
DECLARE @WarehouseId int;

SELECT @WarehouseId = Id
FROM dbo.Warehouses WITH (UPDLOCK, HOLDLOCK)
WHERE GlobalId = @GlobalId;

IF @WarehouseId IS NULL
BEGIN
    INSERT INTO dbo.Warehouses
    (
        GlobalId,
        Code,
        Name,
        Description,
        BranchCode,
        Address,
        City,
        Province,
        Country,
        Phone,
        Email,
        ManagerName,
        AllowsSales,
        AllowsPurchases,
        AllowsTransfers,
        AllowsProduction,
        IsDefault,
        ExternalSystem,
        ExternalCode,
        SapCode,
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
        @Description,
        @BranchCode,
        @Address,
        @City,
        @Province,
        @Country,
        @Phone,
        @Email,
        @ManagerName,
        @AllowsSales,
        @AllowsPurchases,
        @AllowsTransfers,
        @AllowsProduction,
        @IsDefault,
        @ExternalSystem,
        @ExternalCode,
        @SapCode,
        @IsActive,
        N'MasterBranchSyncWorker',
        @CreatedAt,
        @IsDeleted,
        CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END,
        CASE WHEN @IsDeleted = 1 THEN SYSUTCDATETIME() ELSE NULL END
    );

    SET @WarehouseId = CONVERT(int, SCOPE_IDENTITY());
END
ELSE
BEGIN
    UPDATE dbo.Warehouses
    SET Code = @Code,
        Name = @Name,
        Description = @Description,
        BranchCode = @BranchCode,
        Address = @Address,
        City = @City,
        Province = @Province,
        Country = @Country,
        Phone = @Phone,
        Email = @Email,
        ManagerName = @ManagerName,
        AllowsSales = @AllowsSales,
        AllowsPurchases = @AllowsPurchases,
        AllowsTransfers = @AllowsTransfers,
        AllowsProduction = @AllowsProduction,
        IsDefault = @IsDefault,
        ExternalSystem = @ExternalSystem,
        ExternalCode = @ExternalCode,
        SapCode = @SapCode,
        IsActive = @IsActive,
        UpdatedByUserName = N'MasterBranchSyncWorker',
        UpdatedAt = @UpdatedAt,
        IsDeleted = @IsDeleted,
        DeletedByUserName = CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END,
        DeletedAt = CASE WHEN @IsDeleted = 1 THEN COALESCE(DeletedAt, SYSUTCDATETIME()) ELSE NULL END
    WHERE Id = @WarehouseId;
END;

SELECT @WarehouseId;
""";

        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new
            {
                payload.GlobalId,
                Code = NormalizeRequired(payload.Code, "Code", 50),
                Name = NormalizeRequired(payload.Name, "Name", 150),
                Description = NormalizeOptional(payload.Description, 500),
                BranchCode = NormalizeOptional(payload.BranchCode, 50),
                Address = NormalizeOptional(payload.Address, 250),
                City = NormalizeOptional(payload.City, 100),
                Province = NormalizeOptional(payload.Province, 100),
                Country = NormalizeOptional(payload.Country, 100),
                Phone = NormalizeOptional(payload.Phone, 50),
                Email = NormalizeOptional(payload.Email, 150),
                ManagerName = NormalizeOptional(payload.ManagerName, 150),
                payload.AllowsSales,
                payload.AllowsPurchases,
                payload.AllowsTransfers,
                payload.AllowsProduction,
                payload.IsDefault,
                ExternalSystem = NormalizeOptional(payload.ExternalSystem, 50),
                ExternalCode = NormalizeOptional(payload.ExternalCode, 100),
                SapCode = NormalizeOptional(payload.SapCode, 100),
                IsActive = isActive,
                CreatedAt = payload.CreatedAt == default ? DateTime.UtcNow : payload.CreatedAt,
                UpdatedAt = payload.UpdatedAt ?? DateTime.UtcNow,
                IsDeleted = isDeleted
            },
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
            : throw new NotSupportedException($"El motor {company.DatabaseEngine} todavia no esta implementado para Sync Warehouse.");
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"El campo {fieldName} es requerido para sincronizar Warehouse.");
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

    private sealed record InboxState(long Id, string Status);
}

using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Sync.Configuration;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class ReferenceCatalogSyncApplyRepository(ICompanyResolver companyResolver)
    : IReferenceCatalogSyncApplyRepository
{
    public async Task<bool> ExistsByGlobalIdAsync(int branchCompanyId, string entityCode, Guid globalId, CancellationToken cancellationToken = default)
    {
        var table = ResolveTable(entityCode);
        var company = await ResolveBranchAsync(branchCompanyId, cancellationToken);
        await using var connection = CreateSqlConnection(company);
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            $"SELECT COUNT(1) FROM dbo.{table} WHERE GlobalId=@GlobalId;",
            new { GlobalId = globalId }, cancellationToken: cancellationToken)) > 0;
    }

    public async Task<ReferenceCatalogSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        string entityCode,
        SyncEventApplyContext context,
        ReferenceCatalogSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        var table = ResolveTable(entityCode);
        var company = await ResolveBranchAsync(branchCompanyId, cancellationToken);
        await using var connection = CreateSqlConnection(company);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
        try
        {
            var inbox = await connection.QuerySingleOrDefaultAsync<InboxState>(new CommandDefinition(
                "SELECT TOP (1) Id,Status FROM dbo.SyncInbox WITH (UPDLOCK,HOLDLOCK) WHERE EventId=@EventId;",
                new { context.EventId }, transaction, cancellationToken: cancellationToken));
            if (inbox?.Status == nameof(SyncEventStatus.Applied))
            {
                await transaction.CommitAsync(cancellationToken);
                return new(true, true, null, "Evento ya aplicado en SyncInbox.");
            }

            var inboxId = inbox?.Id ?? await connection.ExecuteScalarAsync<long>(new CommandDefinition(
                """
                INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status)
                VALUES(@EventId,@SourceCompanyId,@EntityName,@EntityGlobalId,@Operation,@PayloadJson,N'Pending');
                SELECT CONVERT(bigint,SCOPE_IDENTITY());
                """, context, transaction, cancellationToken: cancellationToken));

            var localId = await UpsertAsync(connection, transaction, table, payload, operation, cancellationToken);
            await connection.ExecuteAsync(new CommandDefinition(
                "UPDATE dbo.SyncInbox SET Status=N'Applied',AppliedAt=SYSUTCDATETIME(),ErrorMessage=NULL,LastErrorMessage=NULL,NextRetryAt=NULL WHERE Id=@Id;",
                new { Id = inboxId }, transaction, cancellationToken: cancellationToken));
            await transaction.CommitAsync(cancellationToken);
            return new(true, false, localId, $"{entityCode} sincronizado por GlobalId {payload.GlobalId}.");
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            await transaction.RollbackAsync(CancellationToken.None);
            await RecordErrorAsync(connection, context, exception.Message);
            throw;
        }
    }

    private static Task<int> UpsertAsync(SqlConnection connection, IDbTransaction transaction, string table,
        ReferenceCatalogSyncPayload payload, SyncOperation operation, CancellationToken cancellationToken)
    {
        var isDeleted = operation == SyncOperation.Deleted;
        var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;
        var sql = table switch
        {
            "Taxes" => BuildUpsertSql(table, "Id", "Rate=@Rate,", "Rate,", "@Rate,"),
            "UnitOfMeasures" => BuildUpsertSql(table, "Id", string.Empty, string.Empty, string.Empty),
            "PriceLists" => BuildUpsertSql(table, "PriceListId",
                "CurrencyCode=@CurrencyCode,AppliesTo=@AppliesTo,IsDefault=@IsDefault,",
                "CurrencyCode,AppliesTo,IsDefault,", "@CurrencyCode,@AppliesTo,@IsDefault,"),
            "BusinessPartnerPaymentTerms" => BuildUpsertSql(table, "Id",
                "Days=@Days,IsCredit=@IsCredit,",
                "Days,IsCredit,", "@Days,@IsCredit,",
                allowCodeReconciliation: false,
                includeDescription: false),
            _ => throw new InvalidOperationException($"Catalogo de referencia no soportado: {table}.")
        };
        return connection.ExecuteScalarAsync<int>(new CommandDefinition(sql, new
        {
            payload.GlobalId,
            Code = Required(payload.Code, 50),
            Name = Required(payload.Name, 200),
            Description = Optional(payload.Description, 500),
            Rate = payload.Rate ?? 0m,
            CurrencyCode = Required(payload.CurrencyCode ?? "USD", 10),
            AppliesTo = Required(payload.AppliesTo ?? "All", 30),
            payload.IsDefault,
            Days = payload.Days ?? 0,
            IsCredit = payload.IsCredit ?? false,
            IsActive = isActive,
            IsDeleted = isDeleted,
            ExternalSystem = Optional(payload.ExternalSystem, 50),
            ExternalCode = Optional(payload.ExternalCode, 100),
            CreatedAt = payload.CreatedAt == default ? DateTime.UtcNow : payload.CreatedAt,
            UpdatedAt = payload.UpdatedAt ?? DateTime.UtcNow
        }, transaction, cancellationToken: cancellationToken));
    }

    private static string BuildUpsertSql(
        string table,
        string id,
        string updateExtra,
        string insertExtra,
        string valuesExtra,
        bool allowCodeReconciliation = true,
        bool includeDescription = true)
    {
        var codeResolution = allowCodeReconciliation
            ? $"IF @Id IS NULL SELECT @Id={id} FROM dbo.{table} WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code;"
            : $"IF @Id IS NULL AND EXISTS(SELECT 1 FROM dbo.{table} WITH(UPDLOCK,HOLDLOCK) WHERE Code=@Code AND IsDeleted=0) THROW 51114, 'El codigo local ya existe y no se adopta automaticamente durante la sincronizacion.', 1;";
        var descriptionColumn = includeDescription ? "Description," : string.Empty;
        var descriptionValue = includeDescription ? "@Description," : string.Empty;
        var descriptionUpdate = includeDescription ? "Description=@Description," : string.Empty;

        return $"""
        DECLARE @Id int;
        SELECT @Id={id} FROM dbo.{table} WITH(UPDLOCK,HOLDLOCK) WHERE GlobalId=@GlobalId;
        {codeResolution}
        IF @Id IS NULL
        BEGIN
          INSERT dbo.{table}(GlobalId,Code,Name,{descriptionColumn}{insertExtra}IsActive,IsDeleted,ExternalSystem,ExternalCode,CreatedAt,CreatedByUserName)
          VALUES(@GlobalId,@Code,@Name,{descriptionValue}{valuesExtra}@IsActive,@IsDeleted,@ExternalSystem,@ExternalCode,@CreatedAt,N'MasterBranchSyncWorker');
          SET @Id=CONVERT(int,SCOPE_IDENTITY());
        END
        ELSE
          UPDATE dbo.{table} SET GlobalId=@GlobalId,Code=@Code,Name=@Name,{descriptionUpdate}{updateExtra}IsActive=@IsActive,
          IsDeleted=@IsDeleted,ExternalSystem=@ExternalSystem,ExternalCode=@ExternalCode,UpdatedAt=@UpdatedAt,UpdatedByUserName=N'MasterBranchSyncWorker'
          WHERE {id}=@Id;
        SELECT @Id;
        """;
    }

    private static async Task RecordErrorAsync(SqlConnection connection, SyncEventApplyContext context, string message)
    {
        const string sql = """
        IF EXISTS(SELECT 1 FROM dbo.SyncInbox WHERE EventId=@EventId)
          UPDATE dbo.SyncInbox SET Status=N'Error',AttemptCount=AttemptCount+1,ErrorMessage=@Message,LastErrorMessage=@Message,
          NextRetryAt=DATEADD(second,30,SYSUTCDATETIME()) WHERE EventId=@EventId AND Status<>N'Applied';
        ELSE
          INSERT dbo.SyncInbox(EventId,SourceCompanyId,EntityName,EntityGlobalId,Operation,PayloadJson,Status,AttemptCount,ErrorMessage,LastErrorMessage,NextRetryAt)
          VALUES(@EventId,@SourceCompanyId,@EntityName,@EntityGlobalId,@Operation,@PayloadJson,N'Error',1,@Message,@Message,DATEADD(second,30,SYSUTCDATETIME()));
        """;
        await connection.ExecuteAsync(sql, new { context.EventId, context.SourceCompanyId, context.EntityName, context.EntityGlobalId, context.Operation, context.PayloadJson, Message = message });
    }

    private static string ResolveTable(string entityCode) => entityCode switch
    {
        SyncMasterBranchEntityCodes.Taxes => "Taxes",
        SyncMasterBranchEntityCodes.UnitOfMeasures => "UnitOfMeasures",
        SyncMasterBranchEntityCodes.PriceLists => "PriceLists",
        SyncMasterBranchEntityCodes.BusinessPartnerPaymentTerms => "BusinessPartnerPaymentTerms",
        _ => throw new InvalidOperationException($"Entidad de referencia no soportada: {entityCode}.")
    };

    private async Task<CompanyConnectionInfo> ResolveBranchAsync(int id, CancellationToken cancellationToken) =>
        await companyResolver.ResolveByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException($"No se encontro la sucursal destino {id}.");
    private static SqlConnection CreateSqlConnection(CompanyConnectionInfo company) => company.DatabaseEngine == DatabaseEngine.SqlServer
        ? new SqlConnection(company.ConnectionString) : throw new NotSupportedException($"Motor {company.DatabaseEngine} no soportado para catalogos de referencia.");
    private static string Required(string value, int max) => string.IsNullOrWhiteSpace(value)
        ? throw new InvalidOperationException("El valor requerido del catalogo esta vacio.") : value.Trim()[..Math.Min(value.Trim().Length, max)];
    private static string? Optional(string? value, int max) => string.IsNullOrWhiteSpace(value) ? null : value.Trim()[..Math.Min(value.Trim().Length, max)];
    private sealed record InboxState(long Id, string Status);
}

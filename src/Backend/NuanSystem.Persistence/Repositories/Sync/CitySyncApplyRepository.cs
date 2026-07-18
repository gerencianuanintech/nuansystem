using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Geography.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class CitySyncApplyRepository(ICompanyResolver companyResolver) : ICitySyncApplyRepository
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
FROM dbo.Cities
WHERE GlobalId = @GlobalId;
""";

        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new { GlobalId = globalId },
            cancellationToken: cancellationToken));
        return count > 0;
    }

    public Task<CitySyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CitySyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        return ApplyAsync(branchCompanyId, context, payload, operation, markDeleted: false, cancellationToken);
    }

    public Task<CitySyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CitySyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default)
    {
        return ApplyAsync(branchCompanyId, context, payload, SyncOperation.Disabled, markDeleted, cancellationToken);
    }

    private async Task<CitySyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        CitySyncPayload payload,
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
                return new CitySyncApplyResult(true, true, null, "Evento ya aplicado en SyncInbox.");
            }

            var inboxId = inbox?.Id ?? await InsertInboxAsync(connection, transaction, context, cancellationToken);
            var cityId = await UpsertCityAsync(connection, transaction, payload, operation, markDeleted, cancellationToken);

            await MarkInboxAppliedAsync(connection, transaction, inboxId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new CitySyncApplyResult(true, false, cityId, $"Ciudad sincronizada por GlobalId {payload.GlobalId}.");
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

    private static async Task<int> UpsertCityAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        CitySyncPayload payload,
        SyncOperation operation,
        bool markDeleted,
        CancellationToken cancellationToken)
    {
        var isDeleted = markDeleted || operation == SyncOperation.Deleted;
        var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;

        const string sql = """
DECLARE @CountryId int;
DECLARE @ProvinceId int;
DECLARE @ProvinceCountryId int;
DECLARE @CityId int;

SELECT @CountryId = CountryId
FROM dbo.Countries WITH (UPDLOCK, HOLDLOCK)
WHERE GlobalId = @CountryGlobalId;

IF @CountryId IS NULL
BEGIN
    SELECT @CountryId = CountryId
    FROM dbo.Countries WITH (UPDLOCK, HOLDLOCK)
    WHERE Code = @CountryCode;

    IF @CountryId IS NOT NULL
    BEGIN
        UPDATE dbo.Countries
        SET GlobalId = @CountryGlobalId,
            UpdatedAt = SYSUTCDATETIME(),
            UpdatedByUserName = N'MasterBranchSyncWorker'
        WHERE CountryId = @CountryId;
    END;
END;

IF @CountryId IS NULL
BEGIN
    THROW 51087, 'No se encontro el pais padre requerido para sincronizar Cities.', 1;
END;

SELECT
    @ProvinceId = ProvinceId,
    @ProvinceCountryId = CountryId
FROM dbo.Provinces WITH (UPDLOCK, HOLDLOCK)
WHERE GlobalId = @ProvinceGlobalId;

IF @ProvinceId IS NOT NULL AND @ProvinceCountryId <> @CountryId
BEGIN
    THROW 51088, 'La provincia indicada no pertenece al pais del payload Cities.', 1;
END;

IF @ProvinceId IS NULL
BEGIN
    SELECT @ProvinceId = ProvinceId
    FROM dbo.Provinces WITH (UPDLOCK, HOLDLOCK)
    WHERE CountryId = @CountryId
      AND Code = @ProvinceCode;

    IF @ProvinceId IS NOT NULL
    BEGIN
        UPDATE dbo.Provinces
        SET GlobalId = @ProvinceGlobalId,
            UpdatedAt = SYSUTCDATETIME(),
            UpdatedByUserName = N'MasterBranchSyncWorker'
        WHERE ProvinceId = @ProvinceId;
    END;
END;

IF @ProvinceId IS NULL
BEGIN
    THROW 51089, 'No se encontro la provincia padre requerida para sincronizar Cities.', 1;
END;

SELECT @CityId = CityId
FROM dbo.Cities WITH (UPDLOCK, HOLDLOCK)
WHERE GlobalId = @GlobalId;

IF @CityId IS NULL
BEGIN
    SELECT @CityId = CityId
    FROM dbo.Cities WITH (UPDLOCK, HOLDLOCK)
    WHERE ProvinceId = @ProvinceId
      AND Code = @Code;
END;

IF @CityId IS NULL
BEGIN
    INSERT INTO dbo.Cities
    (
        GlobalId, CountryId, ProvinceId, Code, Name, IsActive, IsDeleted,
        CreatedAt, CreatedByUserName
    )
    VALUES
    (
        @GlobalId, @CountryId, @ProvinceId, @Code, @Name, @IsActive, @IsDeleted,
        @CreatedAt, N'MasterBranchSyncWorker'
    );

    SET @CityId = CONVERT(int, SCOPE_IDENTITY());
END
ELSE
BEGIN
    UPDATE dbo.Cities
    SET GlobalId = @GlobalId,
        CountryId = @CountryId,
        ProvinceId = @ProvinceId,
        Code = @Code,
        Name = @Name,
        IsActive = @IsActive,
        IsDeleted = @IsDeleted,
        UpdatedAt = @UpdatedAt,
        UpdatedByUserName = N'MasterBranchSyncWorker'
    WHERE CityId = @CityId;
END;

SELECT @CityId;
""";

        return await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new
            {
                payload.GlobalId,
                payload.CountryGlobalId,
                CountryCode = NormalizeRequired(payload.CountryCode, "CountryCode", 10),
                payload.ProvinceGlobalId,
                ProvinceCode = NormalizeRequired(payload.ProvinceCode, "ProvinceCode", 20),
                Code = NormalizeRequired(payload.Code, "Code", 20),
                Name = NormalizeRequired(payload.Name, "Name", 120),
                IsActive = isActive,
                IsDeleted = isDeleted,
                CreatedAt = payload.CreatedAt == default ? DateTime.UtcNow : payload.CreatedAt,
                UpdatedAt = payload.UpdatedAt ?? DateTime.UtcNow
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
    WHERE EventId = @EventId AND Status <> N'Applied';
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
            : throw new NotSupportedException($"El motor {company.DatabaseEngine} todavia no esta implementado para Sync Cities.");
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"El campo {fieldName} es requerido para sincronizar Cities.");
        }

        var trimmed = value.Trim();
        return trimmed[..Math.Min(trimmed.Length, maxLength)];
    }

    private sealed record InboxState(long Id, string Status);
}

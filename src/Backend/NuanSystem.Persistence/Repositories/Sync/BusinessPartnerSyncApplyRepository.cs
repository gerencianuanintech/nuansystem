using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.BusinessPartners.Dtos;
using NuanSystem.Application.Features.Sync.Dtos;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Shared.Sync;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class BusinessPartnerSyncApplyRepository(ICompanyResolver companyResolver) : IBusinessPartnerSyncApplyRepository
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
FROM dbo.BusinessPartners
WHERE GlobalId = @GlobalId;
""";

        var count = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new { GlobalId = globalId },
            cancellationToken: cancellationToken));
        return count > 0;
    }

    public Task<BusinessPartnerSyncApplyResult> UpsertFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerSyncPayload payload,
        SyncOperation operation,
        CancellationToken cancellationToken = default)
    {
        return ApplyAsync(branchCompanyId, context, payload, operation, markDeleted: false, cancellationToken);
    }

    public Task<BusinessPartnerSyncApplyResult> DisableFromSyncAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerSyncPayload payload,
        bool markDeleted,
        CancellationToken cancellationToken = default)
    {
        return ApplyAsync(branchCompanyId, context, payload, SyncOperation.Disabled, markDeleted, cancellationToken);
    }

    private async Task<BusinessPartnerSyncApplyResult> ApplyAsync(
        int branchCompanyId,
        SyncEventApplyContext context,
        BusinessPartnerSyncPayload payload,
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
                return new BusinessPartnerSyncApplyResult(true, true, null, "Evento ya aplicado en SyncInbox.");
            }

            var inboxId = inbox?.Id ?? await InsertInboxAsync(connection, transaction, context, cancellationToken);
            var identificationTypeId = await ResolveIdentificationTypeIdAsync(
                connection,
                transaction,
                payload.IdentificationTypeCode,
                cancellationToken);

            var businessPartnerId = await UpsertBusinessPartnerAsync(
                connection,
                transaction,
                payload,
                operation,
                markDeleted,
                identificationTypeId,
                cancellationToken);

            await MarkInboxAppliedAsync(connection, transaction, inboxId, cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new BusinessPartnerSyncApplyResult(
                true,
                false,
                businessPartnerId,
                $"BusinessPartner sincronizado por GlobalId {payload.GlobalId}.");
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

    private static async Task<int> ResolveIdentificationTypeIdAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        string? identificationTypeCode,
        CancellationToken cancellationToken)
    {
        const string sql = """
SELECT TOP (1) Id
FROM dbo.BusinessPartnerIdentificationTypes
WHERE IsDeleted = 0
  AND IsActive = 1
  AND (@Code IS NULL OR Code = @Code)
ORDER BY CASE WHEN Code = @Code THEN 0 WHEN Code = N'RUC' THEN 1 ELSE 2 END, Id;
""";

        var identificationTypeId = await connection.ExecuteScalarAsync<int?>(new CommandDefinition(
            sql,
            new { Code = string.IsNullOrWhiteSpace(identificationTypeCode) ? null : identificationTypeCode.Trim() },
            transaction,
            cancellationToken: cancellationToken));

        return identificationTypeId
            ?? throw new InvalidOperationException("No existe tipo de identificacion activo para sincronizar BusinessPartner.");
    }

    private static async Task<int> UpsertBusinessPartnerAsync(
        SqlConnection connection,
        IDbTransaction transaction,
        BusinessPartnerSyncPayload payload,
        SyncOperation operation,
        bool markDeleted,
        int identificationTypeId,
        CancellationToken cancellationToken)
    {
        var isDeleted = markDeleted || operation == SyncOperation.Deleted;
        var isActive = !isDeleted && operation != SyncOperation.Disabled && payload.IsActive;

        const string sql = """
DECLARE @BusinessPartnerId int;

SELECT @BusinessPartnerId = Id
FROM dbo.BusinessPartners WITH (UPDLOCK, HOLDLOCK)
WHERE GlobalId = @GlobalId;

IF @BusinessPartnerId IS NULL
BEGIN
    INSERT INTO dbo.BusinessPartners
    (
        GlobalId,
        Code,
        Name,
        ExternalSystem,
        ExternalCode,
        CommercialName,
        PartnerType,
        IdentificationTypeId,
        IdentificationNumber,
        Email,
        Phone,
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
        @CommercialName,
        @PartnerType,
        @IdentificationTypeId,
        @IdentificationNumber,
        @Email,
        @Phone,
        @IsActive,
        N'MasterBranchSyncWorker',
        SYSUTCDATETIME(),
        @IsDeleted,
        CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END,
        CASE WHEN @IsDeleted = 1 THEN SYSUTCDATETIME() ELSE NULL END
    );

    SET @BusinessPartnerId = CONVERT(int, SCOPE_IDENTITY());
END
ELSE
BEGIN
    UPDATE dbo.BusinessPartners
    SET Code = @Code,
        Name = @Name,
        ExternalSystem = @ExternalSystem,
        ExternalCode = @ExternalCode,
        CommercialName = @CommercialName,
        PartnerType = @PartnerType,
        IdentificationTypeId = @IdentificationTypeId,
        IdentificationNumber = @IdentificationNumber,
        Email = @Email,
        Phone = @Phone,
        IsActive = @IsActive,
        UpdatedByUserName = N'MasterBranchSyncWorker',
        UpdatedAt = SYSUTCDATETIME(),
        IsDeleted = @IsDeleted,
        DeletedByUserName = CASE WHEN @IsDeleted = 1 THEN N'MasterBranchSyncWorker' ELSE NULL END,
        DeletedAt = CASE WHEN @IsDeleted = 1 THEN COALESCE(DeletedAt, SYSUTCDATETIME()) ELSE NULL END
    WHERE Id = @BusinessPartnerId;
END;

SELECT @BusinessPartnerId;
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
                CommercialName = NormalizeOptional(payload.CommercialName, 200),
                PartnerType = NormalizePartnerType(payload.PartnerType),
                IdentificationTypeId = identificationTypeId,
                IdentificationNumber = NormalizeRequired(payload.IdentificationNumber, "IdentificationNumber", 50),
                Email = NormalizeOptional(payload.Email, 256),
                Phone = NormalizeOptional(payload.Phone, 50),
                IsActive = isActive,
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
            : throw new NotSupportedException($"El motor {company.DatabaseEngine} todavia no esta implementado para Sync BusinessPartner.");
    }

    private static string NormalizeRequired(string value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"El campo {fieldName} es requerido para sincronizar BusinessPartner.");
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

    private static string NormalizePartnerType(string partnerType)
    {
        return partnerType is "Customer" or "Supplier" or "Both"
            ? partnerType
            : "Customer";
    }

    private sealed record InboxState(long Id, string Status);
}

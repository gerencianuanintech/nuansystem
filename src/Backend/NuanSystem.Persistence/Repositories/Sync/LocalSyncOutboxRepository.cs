using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Application.Features.Sync.Dtos;

namespace NuanSystem.Persistence.Repositories.Sync;

public sealed class LocalSyncOutboxRepository(
    IMasterConnectionFactory masterConnectionFactory,
    ICompanyResolver companyResolver) : ILocalSyncOutboxRepository
{
    public Task<long> CreateAsync(
        CreateLocalSyncOutboxData data,
        IDbConnection connection,
        IDbTransaction transaction,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
INSERT dbo.LocalOutbox
    (EventId,CompanyId,EntityName,EntityGlobalId,EntityCode,Operation,PayloadJson,MaxAttempts)
VALUES
    (@EventId,@CompanyId,@EntityName,@EntityGlobalId,@EntityCode,@Operation,@PayloadJson,@MaxAttempts);
DECLARE @Id bigint=CAST(SCOPE_IDENTITY() AS bigint);
INSERT dbo.SyncAudit
    (CompanyId,EventId,EntityName,EntityGlobalId,[Action],NewStatus,[Message],CreatedBy)
VALUES
    (@CompanyId,@EventId,@EntityName,@EntityGlobalId,N'Created',N'Pending',
     N'Intencion durable registrada en LocalOutbox.',N'Application');
SELECT @Id;
""";
        return connection.ExecuteScalarAsync<long>(new CommandDefinition(
            sql,
            new
            {
                data.EventId,
                data.CompanyId,
                EntityName = data.EntityName.Trim(),
                data.EntityGlobalId,
                EntityCode = data.EntityCode?.Trim(),
                Operation = data.Operation.ToString(),
                data.PayloadJson,
                data.MaxAttempts
            },
            transaction,
            cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyCollection<LocalSyncOutboxCompanyDto>> GetRelayCompaniesAsync(
        CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT Id AS CompanyId, Code AS CompanyCode
FROM dbo.Companies
WHERE IsActive=1 AND IsDeleted=0 AND IsMaster=1 AND SyncEnabled=1
ORDER BY Code;
""";
        using var connection = masterConnectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<LocalSyncOutboxCompanyDto>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<int> ReleaseExpiredLeasesAsync(
        int companyId,
        string workerInstance,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenTenantAsync(companyId, cancellationToken);
        return await connection.ExecuteScalarAsync<int>(Command(
            "dbo.SP_NA_POST_LOCALOUTBOX_LIBERARLEASESVENCIDOS",
            new { WorkerInstance = workerInstance },
            cancellationToken));
    }

    public async Task<IReadOnlyCollection<LocalSyncOutboxDto>> ClaimAsync(
        int companyId,
        string workerInstance,
        int batchSize,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenTenantAsync(companyId, cancellationToken);
        var rows = await connection.QueryAsync<LocalSyncOutboxDto>(Command(
            "dbo.SP_NA_POST_LOCALOUTBOX_RECLAMAR",
            new
            {
                WorkerInstance = workerInstance,
                BatchSize = Math.Clamp(batchSize, 1, 500),
                LeaseSeconds = Math.Clamp((int)leaseDuration.TotalSeconds, 30, 14400)
            },
            cancellationToken));
        return rows.AsList();
    }

    public async Task MarkPromotedAsync(
        int companyId,
        long id,
        string workerInstance,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenTenantAsync(companyId, cancellationToken);
        await connection.ExecuteAsync(Command(
            "dbo.SP_NA_POST_LOCALOUTBOX_COMPLETARPROMOCION",
            new { Id = id, WorkerInstance = workerInstance },
            cancellationToken));
    }

    public async Task MarkRetryAsync(
        int companyId,
        long id,
        string workerInstance,
        string errorMessage,
        TimeSpan retryDelay,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenTenantAsync(companyId, cancellationToken);
        await connection.ExecuteAsync(Command(
            "dbo.SP_NA_POST_LOCALOUTBOX_PROGRAMARREINTENTO",
            new
            {
                Id = id,
                WorkerInstance = workerInstance,
                ErrorMessage = errorMessage,
                RetrySeconds = Math.Clamp((int)retryDelay.TotalSeconds, 1, 86400)
            },
            cancellationToken));
    }

    public async Task MarkConflictAsync(
        int companyId,
        long id,
        string workerInstance,
        string errorMessage,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenTenantAsync(companyId, cancellationToken);
        await connection.ExecuteAsync(Command(
            "dbo.SP_NA_POST_LOCALOUTBOX_COMPLETARCONFLICTO",
            new
            {
                Id = id,
                WorkerInstance = workerInstance,
                ErrorMessage = errorMessage
            },
            cancellationToken));
    }

    private async Task<SqlConnection> OpenTenantAsync(int companyId, CancellationToken cancellationToken)
    {
        var company = await companyResolver.ResolveByIdAsync(companyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontro una empresa activa para el identificador {companyId}.");
        var connection = new SqlConnection(company.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    private static CommandDefinition Command(
        string procedure,
        object parameters,
        CancellationToken cancellationToken) =>
        new(procedure, parameters, commandType: CommandType.StoredProcedure, cancellationToken: cancellationToken);
}

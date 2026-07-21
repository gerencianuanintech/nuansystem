using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Sri;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Repositories;

public sealed class SriWorkerRepository(MasterConnectionFactory masterConnectionFactory, ICompanyResolver companyResolver)
    : ISriWorkerCompanyRepository, ISriWorkerQueueRepository
{
    public async Task<IReadOnlyCollection<SriWorkerCompanyDto>> GetEnabledCompaniesAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
SELECT c.Id AS CompanyId, c.Code AS CompanyCode,
       COALESCE(JSON_VALUE(i.ConfigurationJson, '$.environment'), N'') AS Environment
FROM dbo.Companies c
INNER JOIN dbo.TenantFeatures f ON f.CompanyId=c.Id AND f.FeatureCode=N'SRI_DOCUMENTS' AND f.IsEnabled=1
INNER JOIN dbo.TenantIntegrations i ON i.CompanyId=c.Id AND i.IntegrationCode=N'SRI' AND i.IsEnabled=1
WHERE c.IsActive=1 AND JSON_VALUE(i.ConfigurationJson, '$.environment') IN (N'Test', N'Production')
ORDER BY c.Code;
""";
        await using var connection = masterConnectionFactory.CreateConnection();
        var rows = await connection.QueryAsync<SriWorkerCompanyDto>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<int> ReleaseExpiredLeasesAsync(int companyId, string workerInstance, int maxAttempts, CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenTenantAsync(companyId, cancellationToken);
        return await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_POST_SRIDOCUMENTQUEUE_LIBERARLEASESVENCIDOS",
            new { WorkerInstance = workerInstance, MaxAttempts = maxAttempts }, cancellationToken));
    }

    public async Task<IReadOnlyCollection<SriClaimedDocumentDto>> ClaimAsync(int companyId, string environment, string workerInstance, int batchSize, int leaseSeconds, int maxAttempts, CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenTenantAsync(companyId, cancellationToken);
        var rows = await connection.QueryAsync<SriClaimedDocumentDto>(Command("dbo.SP_NA_POST_SRIDOCUMENTQUEUE_RECLAMAR",
            new { Environment = environment, WorkerInstance = workerInstance, BatchSize = batchSize, LeaseSeconds = leaseSeconds, MaxAttempts = maxAttempts }, cancellationToken));
        return rows.AsList();
    }

    public async Task<SriWorkerCompletionCode> CompleteAuthorizedAsync(int companyId, SriAuthorizedDocumentData document, CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenTenantAsync(companyId, cancellationToken);
        return ToCode(await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_POST_SRIDOCUMENTQUEUE_COMPLETARAUTORIZADO", document, cancellationToken)));
    }

    public async Task<SriWorkerCompletionCode> CompleteAttemptAsync(int companyId, SriAttemptCompletionData completion, CancellationToken cancellationToken = default)
    {
        await using var connection = await OpenTenantAsync(companyId, cancellationToken);
        return ToCode(await connection.ExecuteScalarAsync<int>(Command("dbo.SP_NA_POST_SRIDOCUMENTQUEUE_COMPLETARINTENTO", completion, cancellationToken)));
    }

    private async Task<SqlConnection> OpenTenantAsync(int companyId, CancellationToken cancellationToken)
    {
        var company = await companyResolver.ResolveByIdAsync(companyId, cancellationToken)
            ?? throw new InvalidOperationException($"No se encontro una empresa activa para el identificador {companyId}.");
        var connection = new SqlConnection(company.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }

    private static CommandDefinition Command(string procedure, object parameters, CancellationToken cancellationToken) =>
        new(procedure, parameters, cancellationToken: cancellationToken, commandType: CommandType.StoredProcedure);

    private static SriWorkerCompletionCode ToCode(int value) => Enum.IsDefined(typeof(SriWorkerCompletionCode), value)
        ? (SriWorkerCompletionCode)value : SriWorkerCompletionCode.InvalidState;
}

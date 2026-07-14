using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Connections;
using NuanSystem.Persistence.Options;

namespace NuanSystem.Persistence.Tenancy;

public sealed class SqlServerCompanyResolver(
    MasterConnectionFactory connectionFactory,
    ISecretProtector secretProtector,
    IOptions<SqlConnectionPolicyOptions> sqlConnectionPolicyOptions) : ICompanyResolver
{
    public async Task<CompanyConnectionInfo?> ResolveByCodeAsync(
        string companyCode,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        var projection = await LoadCompanyProjectionAsync(connection, string.Empty, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
SELECT TOP (1)
    Id,
    Code,
    CommercialName,
    DatabaseEngine,
    [Server],
    Port,
    DatabaseName,
    DatabaseUser,
    DatabasePasswordEncrypted,
    IsActive,
    SapIntegrationMode,
    {projection.OperationMode},
    {projection.IsMaster},
    {projection.ParentCompanyId},
    {projection.BranchCode},
    {projection.SyncEnabled}
FROM dbo.Companies
WHERE Code = @code;
""";
        command.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = companyCode;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var isActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
        if (!isActive)
        {
            return null;
        }

        var databaseEngine = (DatabaseEngine)reader.GetInt32(reader.GetOrdinal("DatabaseEngine"));
        var password = secretProtector.Unprotect(reader.GetString(reader.GetOrdinal("DatabasePasswordEncrypted")));
        var connectionString = BuildConnectionString(databaseEngine, reader, password, sqlConnectionPolicyOptions.Value);

        return new CompanyConnectionInfo(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Code")),
            reader.GetString(reader.GetOrdinal("CommercialName")),
            databaseEngine,
            connectionString,
            (SapIntegrationMode)reader.GetInt32(reader.GetOrdinal("SapIntegrationMode")),
            (CompanyOperationMode)reader.GetInt32(reader.GetOrdinal("OperationMode")),
            reader.GetBoolean(reader.GetOrdinal("IsMaster")),
            reader.IsDBNull(reader.GetOrdinal("ParentCompanyId")) ? null : reader.GetInt32(reader.GetOrdinal("ParentCompanyId")),
            reader.IsDBNull(reader.GetOrdinal("BranchCode")) ? null : reader.GetString(reader.GetOrdinal("BranchCode")),
            reader.GetBoolean(reader.GetOrdinal("SyncEnabled")));
    }

    public async Task<CompanyConnectionInfo?> ResolveByIdAsync(
        int companyId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        var projection = await LoadCompanyProjectionAsync(connection, string.Empty, cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
SELECT TOP (1)
    Id,
    Code,
    CommercialName,
    DatabaseEngine,
    [Server],
    Port,
    DatabaseName,
    DatabaseUser,
    DatabasePasswordEncrypted,
    IsActive,
    SapIntegrationMode,
    {projection.OperationMode},
    {projection.IsMaster},
    {projection.ParentCompanyId},
    {projection.BranchCode},
    {projection.SyncEnabled}
FROM dbo.Companies
WHERE Id = @id;
""";
        command.Parameters.Add("@id", SqlDbType.Int).Value = companyId;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var isActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
        if (!isActive)
        {
            return null;
        }

        var databaseEngine = (DatabaseEngine)reader.GetInt32(reader.GetOrdinal("DatabaseEngine"));
        var password = secretProtector.Unprotect(reader.GetString(reader.GetOrdinal("DatabasePasswordEncrypted")));
        var connectionString = BuildConnectionString(databaseEngine, reader, password, sqlConnectionPolicyOptions.Value);

        return new CompanyConnectionInfo(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Code")),
            reader.GetString(reader.GetOrdinal("CommercialName")),
            databaseEngine,
            connectionString,
            (SapIntegrationMode)reader.GetInt32(reader.GetOrdinal("SapIntegrationMode")),
            (CompanyOperationMode)reader.GetInt32(reader.GetOrdinal("OperationMode")),
            reader.GetBoolean(reader.GetOrdinal("IsMaster")),
            reader.IsDBNull(reader.GetOrdinal("ParentCompanyId")) ? null : reader.GetInt32(reader.GetOrdinal("ParentCompanyId")),
            reader.IsDBNull(reader.GetOrdinal("BranchCode")) ? null : reader.GetString(reader.GetOrdinal("BranchCode")),
            reader.GetBoolean(reader.GetOrdinal("SyncEnabled")));
    }

    public async Task<CompanyConnectionInfo?> ResolveByCodeForUserAsync(
        string companyCode,
        int userId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        var projection = await LoadCompanyProjectionAsync(connection, "c", cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = $"""
SELECT TOP (1)
    c.Id,
    c.Code,
    c.CommercialName,
    c.DatabaseEngine,
    c.[Server],
    c.Port,
    c.DatabaseName,
    c.DatabaseUser,
    c.DatabasePasswordEncrypted,
    c.IsActive,
    c.SapIntegrationMode,
    {projection.OperationMode},
    {projection.IsMaster},
    {projection.ParentCompanyId},
    {projection.BranchCode},
    {projection.SyncEnabled}
FROM dbo.Companies c
INNER JOIN dbo.UserCompanies uc ON uc.CompanyId = c.Id
WHERE c.Code = @code
  AND c.IsActive = 1
  AND uc.UserId = @userId
  AND uc.IsActive = 1;
""";
        command.Parameters.Add("@code", SqlDbType.NVarChar, 50).Value = companyCode;
        command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        var databaseEngine = (DatabaseEngine)reader.GetInt32(reader.GetOrdinal("DatabaseEngine"));
        var password = secretProtector.Unprotect(reader.GetString(reader.GetOrdinal("DatabasePasswordEncrypted")));
        var connectionString = BuildConnectionString(databaseEngine, reader, password, sqlConnectionPolicyOptions.Value);

        return new CompanyConnectionInfo(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Code")),
            reader.GetString(reader.GetOrdinal("CommercialName")),
            databaseEngine,
            connectionString,
            (SapIntegrationMode)reader.GetInt32(reader.GetOrdinal("SapIntegrationMode")),
            (CompanyOperationMode)reader.GetInt32(reader.GetOrdinal("OperationMode")),
            reader.GetBoolean(reader.GetOrdinal("IsMaster")),
            reader.IsDBNull(reader.GetOrdinal("ParentCompanyId")) ? null : reader.GetInt32(reader.GetOrdinal("ParentCompanyId")),
            reader.IsDBNull(reader.GetOrdinal("BranchCode")) ? null : reader.GetString(reader.GetOrdinal("BranchCode")),
            reader.GetBoolean(reader.GetOrdinal("SyncEnabled")));
    }

    private static string BuildConnectionString(
        DatabaseEngine databaseEngine,
        SqlDataReader reader,
        string password,
        SqlConnectionPolicyOptions sqlConnectionPolicy)
    {
        if (databaseEngine == DatabaseEngine.SqlServer)
        {
            var server = reader.GetString(reader.GetOrdinal("Server"));
            var portOrdinal = reader.GetOrdinal("Port");
            if (!reader.IsDBNull(portOrdinal))
            {
                server = $"{server},{reader.GetInt32(portOrdinal)}";
            }

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = reader.GetString(reader.GetOrdinal("DatabaseName")),
                UserID = reader.GetString(reader.GetOrdinal("DatabaseUser")),
                Password = password,
                Encrypt = sqlConnectionPolicy.Encrypt,
                TrustServerCertificate = sqlConnectionPolicy.TrustServerCertificate,
                MultipleActiveResultSets = false
            };

            return builder.ConnectionString;
        }

        throw new NotSupportedException($"El motor {databaseEngine} todavia no esta implementado.");
    }

    private static async Task<CompanyProjection> LoadCompanyProjectionAsync(
        SqlConnection connection,
        string? tableAlias,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
SELECT
    HasOperationMode = CONVERT(bit, CASE WHEN COL_LENGTH(N'dbo.Companies', N'OperationMode') IS NULL THEN 0 ELSE 1 END),
    HasIsMaster = CONVERT(bit, CASE WHEN COL_LENGTH(N'dbo.Companies', N'IsMaster') IS NULL THEN 0 ELSE 1 END),
    HasParentCompanyId = CONVERT(bit, CASE WHEN COL_LENGTH(N'dbo.Companies', N'ParentCompanyId') IS NULL THEN 0 ELSE 1 END),
    HasBranchCode = CONVERT(bit, CASE WHEN COL_LENGTH(N'dbo.Companies', N'BranchCode') IS NULL THEN 0 ELSE 1 END),
    HasSyncEnabled = CONVERT(bit, CASE WHEN COL_LENGTH(N'dbo.Companies', N'SyncEnabled') IS NULL THEN 0 ELSE 1 END);
""";

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        var prefix = string.IsNullOrWhiteSpace(tableAlias) ? string.Empty : $"{tableAlias}.";
        return new CompanyProjection(
            reader.GetBoolean(reader.GetOrdinal("HasOperationMode")) ? $"{prefix}OperationMode" : "CAST(0 AS int) AS OperationMode",
            reader.GetBoolean(reader.GetOrdinal("HasIsMaster")) ? $"{prefix}IsMaster" : "CONVERT(bit, 1) AS IsMaster",
            reader.GetBoolean(reader.GetOrdinal("HasParentCompanyId")) ? $"{prefix}ParentCompanyId" : "CAST(NULL AS int) AS ParentCompanyId",
            reader.GetBoolean(reader.GetOrdinal("HasBranchCode")) ? $"{prefix}BranchCode" : "CAST(NULL AS nvarchar(50)) AS BranchCode",
            reader.GetBoolean(reader.GetOrdinal("HasSyncEnabled")) ? $"{prefix}SyncEnabled" : "CONVERT(bit, 0) AS SyncEnabled");
    }

    private sealed record CompanyProjection(
        string OperationMode,
        string IsMaster,
        string ParentCompanyId,
        string BranchCode,
        string SyncEnabled);
}

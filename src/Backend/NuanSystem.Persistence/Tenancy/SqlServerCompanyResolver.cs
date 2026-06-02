using System.Data;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Connections;

namespace NuanSystem.Persistence.Tenancy;

public sealed class SqlServerCompanyResolver(
    MasterConnectionFactory connectionFactory,
    ISecretProtector secretProtector) : ICompanyResolver
{
    public async Task<CompanyConnectionInfo?> ResolveByCodeAsync(
        string companyCode,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
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
    SapIntegrationMode
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
        var connectionString = BuildConnectionString(databaseEngine, reader, password);

        return new CompanyConnectionInfo(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Code")),
            reader.GetString(reader.GetOrdinal("CommercialName")),
            databaseEngine,
            connectionString,
            (SapIntegrationMode)reader.GetInt32(reader.GetOrdinal("SapIntegrationMode")));
    }

    public async Task<CompanyConnectionInfo?> ResolveByCodeForUserAsync(
        string companyCode,
        int userId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
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
    c.SapIntegrationMode
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
        var connectionString = BuildConnectionString(databaseEngine, reader, password);

        return new CompanyConnectionInfo(
            reader.GetInt32(reader.GetOrdinal("Id")),
            reader.GetString(reader.GetOrdinal("Code")),
            reader.GetString(reader.GetOrdinal("CommercialName")),
            databaseEngine,
            connectionString,
            (SapIntegrationMode)reader.GetInt32(reader.GetOrdinal("SapIntegrationMode")));
    }

    private static string BuildConnectionString(
        DatabaseEngine databaseEngine,
        SqlDataReader reader,
        string password)
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
                TrustServerCertificate = true,
                Encrypt = true,
                MultipleActiveResultSets = false
            };

            return builder.ConnectionString;
        }

        throw new NotSupportedException($"El motor {databaseEngine} todavia no esta implementado.");
    }
}

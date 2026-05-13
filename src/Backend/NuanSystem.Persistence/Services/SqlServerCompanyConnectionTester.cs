using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Features.Companies.Dtos;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Persistence.Services;

public sealed class SqlServerCompanyConnectionTester : ICompanyConnectionTester
{
    public async Task<CompanyConnectionTestResult> TestAsync(
        CompanyConnectionTestData connection,
        CancellationToken cancellationToken = default)
    {
        if (connection.DatabaseEngine != DatabaseEngine.SqlServer)
        {
            return new CompanyConnectionTestResult(false, "Motor de base de datos no soportado todavia.", null);
        }

        try
        {
            var server = connection.Port.HasValue
                ? $"{connection.Server},{connection.Port.Value}"
                : connection.Server;

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = connection.DatabaseName,
                UserID = connection.DatabaseUser,
                Password = connection.DatabasePassword,
                TrustServerCertificate = true,
                Encrypt = true,
                ConnectTimeout = 5
            };

            await using var sqlConnection = new SqlConnection(builder.ConnectionString);
            await sqlConnection.OpenAsync(cancellationToken);

            await using var command = sqlConnection.CreateCommand();
            command.CommandText = "SELECT CAST(SERVERPROPERTY('ProductVersion') AS nvarchar(128));";
            var version = (string?)await command.ExecuteScalarAsync(cancellationToken);

            return new CompanyConnectionTestResult(true, "Conexion exitosa.", version);
        }
        catch (Exception exception) when (exception is SqlException or InvalidOperationException)
        {
            return new CompanyConnectionTestResult(false, exception.Message, null);
        }
    }
}

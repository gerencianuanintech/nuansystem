using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Persistence.Options;

namespace NuanSystem.Persistence.Connections;

public sealed class MasterConnectionFactory(
    IConfiguration configuration,
    IOptions<MasterDatabaseOptions> options,
    IOptions<SqlConnectionPolicyOptions> sqlConnectionPolicyOptions) : IMasterConnectionFactory
{
    System.Data.IDbConnection IMasterConnectionFactory.CreateConnection()
    {
        return CreateConnection();
    }

    public SqlConnection CreateConnection()
    {
        var serverConnectionString = configuration.GetConnectionString("SqlServerAdmin");
        if (string.IsNullOrWhiteSpace(serverConnectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:SqlServerAdmin no esta configurado.");
        }

        var builder = new SqlConnectionStringBuilder(serverConnectionString)
        {
            InitialCatalog = options.Value.DatabaseName,
            Encrypt = sqlConnectionPolicyOptions.Value.Encrypt,
            TrustServerCertificate = sqlConnectionPolicyOptions.Value.TrustServerCertificate
        };

        return new SqlConnection(builder.ConnectionString);
    }
}

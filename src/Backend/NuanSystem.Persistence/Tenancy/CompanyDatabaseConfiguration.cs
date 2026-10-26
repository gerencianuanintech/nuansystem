using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Security;
using NuanSystem.Domain.Tenancy;
using NuanSystem.Persistence.Options;

namespace NuanSystem.Persistence.Tenancy;

internal sealed record CompanyDatabaseConfiguration(
    DatabaseEngine Engine, string Server, int? Port, string DatabaseName,
    string User, string PasswordEncrypted)
{
    internal string BuildConnectionString(ISecretProtector protector, SqlConnectionPolicyOptions sqlConnectionPolicy)
    {
        var password = protector.Unprotect(PasswordEncrypted);
        if (Engine != DatabaseEngine.SqlServer)
            throw new NotSupportedException($"El motor {Engine} todavia no esta implementado.");

        var builder = new SqlConnectionStringBuilder
        {
            DataSource = Port is null ? Server : $"{Server},{Port.Value}",
            InitialCatalog = DatabaseName,
            UserID = User,
            Password = password,
            Encrypt = sqlConnectionPolicy.Encrypt,
            TrustServerCertificate = sqlConnectionPolicy.TrustServerCertificate,
            MultipleActiveResultSets = false
        };
        return builder.ConnectionString;
    }

    public override string ToString() => nameof(CompanyDatabaseConfiguration);
}

using System.Data;
using Microsoft.Data.SqlClient;
using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Tenancy;
using NuanSystem.Domain.Tenancy;

namespace NuanSystem.Persistence.Connections;

public sealed class TenantConnectionFactory(ICompanyContext companyContext) : ITenantConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var company = companyContext.CurrentCompany
            ?? throw new InvalidOperationException("No hay empresa activa para crear la conexion tenant.");

        return company.DatabaseEngine switch
        {
            DatabaseEngine.SqlServer => new SqlConnection(company.ConnectionString),
            DatabaseEngine.MySql => throw new NotSupportedException("MySQL todavia no esta implementado."),
            _ => throw new NotSupportedException($"Motor de base de datos no soportado: {company.DatabaseEngine}.")
        };
    }
}

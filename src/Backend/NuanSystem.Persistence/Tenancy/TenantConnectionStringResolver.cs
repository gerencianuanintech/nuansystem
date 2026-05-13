using NuanSystem.Application.Abstractions.Tenancy;

namespace NuanSystem.Persistence.Tenancy;

public sealed class TenantConnectionStringResolver(ICompanyContext companyContext) : ITenantConnectionStringResolver
{
    public string GetRequiredConnectionString()
    {
        return companyContext.CurrentCompany?.ConnectionString
            ?? throw new InvalidOperationException("No hay empresa activa para resolver la conexion del tenant.");
    }
}

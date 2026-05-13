namespace NuanSystem.Application.Abstractions.Tenancy;

public interface ITenantConnectionStringResolver
{
    string GetRequiredConnectionString();
}

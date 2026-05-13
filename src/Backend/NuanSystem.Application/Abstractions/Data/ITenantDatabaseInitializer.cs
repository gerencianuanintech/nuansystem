namespace NuanSystem.Application.Abstractions.Data;

public interface ITenantDatabaseInitializer
{
    Task InitializeCurrentTenantAsync(CancellationToken cancellationToken = default);
}

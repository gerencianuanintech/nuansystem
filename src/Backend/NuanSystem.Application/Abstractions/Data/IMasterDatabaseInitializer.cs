namespace NuanSystem.Application.Abstractions.Data;

public interface IMasterDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}

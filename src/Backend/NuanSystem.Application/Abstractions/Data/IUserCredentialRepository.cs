namespace NuanSystem.Application.Abstractions.Data;

public interface IUserCredentialRepository
{
    Task<string?> GetActivePasswordHashAsync(int userId, CancellationToken cancellationToken);
    Task UpdatePasswordAsync(int userId, string passwordHash, CancellationToken cancellationToken);
}

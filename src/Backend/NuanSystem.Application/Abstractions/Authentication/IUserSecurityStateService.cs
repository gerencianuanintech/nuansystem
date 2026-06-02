namespace NuanSystem.Application.Abstractions.Authentication;

public interface IUserSecurityStateService
{
    Task<string?> GetSecurityStampAsync(int userId, CancellationToken cancellationToken = default);
}

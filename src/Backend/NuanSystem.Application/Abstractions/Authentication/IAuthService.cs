namespace NuanSystem.Application.Abstractions.Authentication;

public interface IAuthService
{
    Task<AuthResult?> LoginAsync(
        string userNameOrEmail,
        string password,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<AuthCompanyDto>> GetCompaniesForUserAsync(
        int userId,
        CancellationToken cancellationToken = default);
}

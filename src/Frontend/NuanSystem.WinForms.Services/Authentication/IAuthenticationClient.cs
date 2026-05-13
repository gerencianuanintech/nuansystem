using NuanSystem.Shared.Contracts.Auth;

namespace NuanSystem.WinForms.Services.Authentication;

public interface IAuthenticationClient
{
    Task<LoginResponse> LoginAsync(string userNameOrEmail, string password, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}

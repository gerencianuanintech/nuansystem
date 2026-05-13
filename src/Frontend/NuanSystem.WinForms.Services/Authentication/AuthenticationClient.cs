using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Session;

namespace NuanSystem.WinForms.Services.Authentication;

public sealed class AuthenticationClient : IAuthenticationClient
{
    private readonly INuanApiClient apiClient;
    private readonly ApiSession session;

    public AuthenticationClient(INuanApiClient apiClient, ApiSession session)
    {
        this.apiClient = apiClient;
        this.session = session;
    }

    public async Task<LoginResponse> LoginAsync(string userNameOrEmail, string password, CancellationToken cancellationToken = default)
    {
        var response = await apiClient.PostAsync<LoginRequest, LoginResponse>(
            "/api/auth/login",
            new LoginRequest(userNameOrEmail, password),
            cancellationToken);

        session.SetUser(response);
        return response;
    }

    public async Task ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        await apiClient.PostAsync<ChangePasswordRequest, object>(
            "/api/auth/change-password",
            new ChangePasswordRequest(currentPassword, newPassword),
            cancellationToken);

        if (session.CurrentUser is not null)
        {
            session.SetUser(session.CurrentUser with { MustChangePassword = false });
        }
    }
}

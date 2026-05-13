using NuanSystem.Shared.Contracts.Auth;
using NuanSystem.WinForms.Services.Authentication;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Auth;

public sealed class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationClient authenticationClient;
    private string userNameOrEmail = string.Empty;
    private string password = string.Empty;
    private bool isBusy;
    private string statusMessage = string.Empty;

    public LoginViewModel(IAuthenticationClient authenticationClient)
    {
        this.authenticationClient = authenticationClient;
    }

    public string UserNameOrEmail
    {
        get => userNameOrEmail;
        set => SetProperty(ref userNameOrEmail, value);
    }

    public string Password
    {
        get => password;
        set => SetProperty(ref password, value);
    }

    public bool IsBusy
    {
        get => isBusy;
        private set => SetProperty(ref isBusy, value);
    }

    public string StatusMessage
    {
        get => statusMessage;
        private set => SetProperty(ref statusMessage, value);
    }

    public async Task<LoginResponse> LoginAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(UserNameOrEmail))
        {
            throw new InvalidOperationException("Ingrese el usuario o correo.");
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            throw new InvalidOperationException("Ingrese la contrasena.");
        }

        IsBusy = true;
        StatusMessage = "Validando credenciales...";

        try
        {
            var response = await authenticationClient.LoginAsync(UserNameOrEmail.Trim(), Password, cancellationToken);
            StatusMessage = "Login correcto.";
            return response;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public Task ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        return authenticationClient.ChangePasswordAsync(currentPassword, newPassword, cancellationToken);
    }
}

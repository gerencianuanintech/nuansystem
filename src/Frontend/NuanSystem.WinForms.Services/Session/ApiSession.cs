using NuanSystem.Shared.Contracts.Auth;

namespace NuanSystem.WinForms.Services.Session;

public sealed class ApiSession
{
    public LoginResponse? CurrentUser { get; private set; }
    public UserCompanyResponse? CurrentCompany { get; private set; }

    public bool IsAuthenticated => CurrentUser is not null && !string.IsNullOrWhiteSpace(CurrentUser.AccessToken);
    public string? AccessToken => CurrentUser?.AccessToken;
    public string? CompanyCode => CurrentCompany?.Code;

    public bool HasPermission(string permission)
    {
        return CurrentUser?.Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase) == true;
    }

    public void SetUser(LoginResponse user)
    {
        CurrentUser = user;
        CurrentCompany = null;
    }

    public void SelectCompany(UserCompanyResponse company)
    {
        CurrentCompany = company;
    }

    public void Clear()
    {
        CurrentUser = null;
        CurrentCompany = null;
    }
}

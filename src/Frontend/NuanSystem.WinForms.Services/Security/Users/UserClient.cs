using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Security.Users.Models;

namespace NuanSystem.WinForms.Services.Security.Users;

public sealed class UserClient(INuanApiClient apiClient) : IUserClient
{
    public async Task<IReadOnlyCollection<UserAdminItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<UserAdminItem>>("/api/security/users", cancellationToken);
    }

    public async Task<IReadOnlyCollection<RoleItem>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<RoleItem>>("/api/security/users/roles", cancellationToken);
    }

    public Task<UserAdminItem> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<CreateUserRequest, UserAdminItem>("/api/security/users", request, cancellationToken);
    }

    public Task<UserAdminItem> UpdateAsync(int id, CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            Id = id,
            request.UserName,
            request.Email,
            request.PhoneNumber,
            request.EmailConfirmed,
            request.PhoneNumberConfirmed,
            request.FirstName,
            request.LastName,
            request.DisplayName,
            request.Password,
            request.RoleId,
            request.IsActive,
            request.IsLocked,
            request.CanUseWeb,
            request.CanUseMobile,
            request.MustChangePassword,
            request.LockoutEndAt,
            request.TwoFactorEnabled,
            request.ProfileImageUrl,
            request.ProfileImage,
            request.ProfileImageContentType,
            request.ProfileImageFileName
        };

        return apiClient.PutAsync<object, UserAdminItem>($"/api/security/users/{id}", payload, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await apiClient.DeleteAsync<object>($"/api/security/users/{id}", cancellationToken);
    }
}


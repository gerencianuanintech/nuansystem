using NuanSystem.WinForms.Services.Security.Users.Models;

namespace NuanSystem.WinForms.Services.Security.Users;

public interface IUserClient
{
    Task<IReadOnlyCollection<UserAdminItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RoleItem>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<UserAdminItem> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<UserAdminItem> UpdateAsync(int id, CreateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}


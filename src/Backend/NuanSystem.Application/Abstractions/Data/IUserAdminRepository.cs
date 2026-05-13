using NuanSystem.Application.Features.SecurityUsers.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IUserAdminRepository
{
    Task<IReadOnlyCollection<UserAdminDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RoleDto>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserNameAsync(string userName, int excludingId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateUserData user, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateUserData user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
    Task<UserAdminDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}


using NuanSystem.Application.Features.SecurityRoles.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityRoleRepository
{
    Task<IReadOnlyCollection<SecurityRoleDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SecurityRoleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, int excludingId, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(CreateSecurityRoleData role, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(UpdateSecurityRoleData role, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}

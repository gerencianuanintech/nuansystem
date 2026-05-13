using NuanSystem.Application.Features.SecurityMenus.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityMenuRepository
{
    Task<IReadOnlyCollection<SecurityMenuDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SecurityMenuDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateSecurityMenuData menu, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateSecurityMenuData menu, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}

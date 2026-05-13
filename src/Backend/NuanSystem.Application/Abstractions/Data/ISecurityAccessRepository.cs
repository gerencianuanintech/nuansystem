using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityAccessRepository
{
    Task<IReadOnlyCollection<NavigationMenuDto>> GetNavigationAsync(int userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<FormOperationAccessDto>> GetFormOperationsAsync(int userId, string formKey, CancellationToken cancellationToken = default);

    Task<RoleAccessDto> GetRoleAccessAsync(int roleId, CancellationToken cancellationToken = default);

    Task SaveRoleAccessAsync(
        int roleId,
        IReadOnlyCollection<SaveRoleAccessMenuData> menus,
        IReadOnlyCollection<SaveRoleAccessOperationData> operations,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default);
}

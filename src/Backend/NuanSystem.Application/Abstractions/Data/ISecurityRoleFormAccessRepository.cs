using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityRoleFormAccessRepository
{
    Task<IReadOnlyCollection<SecurityFormAccessFormDto>> GetFormsAsync(
        int? formType,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityFormAccessOperationDto>> GetOperationsAsync(
        int roleId,
        int formId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default);

    Task SaveOperationsAsync(
        int roleId,
        int formId,
        IReadOnlyCollection<SaveSecurityFormAccessOperationData> operations,
        int? updatedByUserId,
        string? updatedByUserName,
        CancellationToken cancellationToken = default);

    Task<bool> ValidateUserOperationAsync(
        int userId,
        string formKey,
        string actionKey,
        CancellationToken cancellationToken = default);
}

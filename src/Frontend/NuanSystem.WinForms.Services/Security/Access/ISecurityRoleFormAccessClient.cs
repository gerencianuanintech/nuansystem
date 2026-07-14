using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.Services.Security.Access;

public interface ISecurityRoleFormAccessClient
{
    Task<IReadOnlyCollection<SecurityFormAccessFormItem>> GetFormsAsync(
        int? formType,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityFormAccessOperationItem>> GetOperationsAsync(
        int roleId,
        int formId,
        bool onlyActive,
        string? search,
        CancellationToken cancellationToken = default);

    Task<bool> SaveOperationsAsync(
        int roleId,
        int formId,
        SaveSecurityFormAccessRequest request,
        CancellationToken cancellationToken = default);
}

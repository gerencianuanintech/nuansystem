using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.Services.Security.Access;

public interface ISecurityTransactionalFormAccessClient
{
    Task<IReadOnlyCollection<SecurityFormAccessFormItem>> GetFormsAsync(
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

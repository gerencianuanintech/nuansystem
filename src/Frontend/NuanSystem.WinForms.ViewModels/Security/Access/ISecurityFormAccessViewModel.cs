using NuanSystem.WinForms.Services.Security.Roles.Models;
using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.ViewModels.Security.Access;

public interface ISecurityFormAccessViewModel
{
    IReadOnlyCollection<RoleItem> Roles { get; }
    IReadOnlyCollection<SecurityFormAccessFormItem> Forms { get; }
    IReadOnlyCollection<SecurityFormAccessOperationRow> Operations { get; }
    RoleItem? SelectedRole { get; }
    SecurityFormAccessFormItem? SelectedForm { get; }

    Task LoadAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default);

    Task LoadFormsAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default);

    Task SelectRoleAsync(RoleItem? role, bool onlyActive, string? operationSearch, CancellationToken cancellationToken = default);

    Task SelectFormAsync(SecurityFormAccessFormItem? form, bool onlyActive, string? operationSearch, CancellationToken cancellationToken = default);

    Task LoadOperationsAsync(string? search, bool onlyActive, CancellationToken cancellationToken = default);

    Task SaveAsync(CancellationToken cancellationToken = default);
}

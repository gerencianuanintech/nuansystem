using NuanSystem.WinForms.Services.Roles.Models;
using NuanSystem.WinForms.Services.SecurityAccess.Models;

namespace NuanSystem.WinForms.ViewModels.SecurityAccess;

public interface ISecurityFormAccessViewModel
{
    IReadOnlyCollection<RoleAdminItem> Roles { get; }
    IReadOnlyCollection<SecurityFormAccessFormItem> Forms { get; }
    IReadOnlyCollection<SecurityFormAccessOperationRow> Operations { get; }
    RoleAdminItem? SelectedRole { get; }
    SecurityFormAccessFormItem? SelectedForm { get; }

    Task LoadAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default);

    Task LoadFormsAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default);

    Task SelectRoleAsync(RoleAdminItem? role, bool onlyActive, string? operationSearch, CancellationToken cancellationToken = default);

    Task SelectFormAsync(SecurityFormAccessFormItem? form, bool onlyActive, string? operationSearch, CancellationToken cancellationToken = default);

    Task LoadOperationsAsync(string? search, bool onlyActive, CancellationToken cancellationToken = default);

    Task SaveAsync(CancellationToken cancellationToken = default);
}

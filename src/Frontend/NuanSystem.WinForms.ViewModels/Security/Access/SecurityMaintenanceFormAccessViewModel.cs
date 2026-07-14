using NuanSystem.WinForms.Services.Security.Roles;
using NuanSystem.WinForms.Services.Security.Roles.Models;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.ViewModels.Security.Access;

public sealed class SecurityMaintenanceFormAccessViewModel(
    IRoleClient roleClient,
    ISecurityRoleFormAccessClient formAccessClient) : ISecurityFormAccessViewModel
{
    private const int MaintenanceFormType = 1;

    public IReadOnlyCollection<RoleItem> Roles { get; private set; } = Array.Empty<RoleItem>();
    public IReadOnlyCollection<SecurityFormAccessFormItem> Forms { get; private set; } = Array.Empty<SecurityFormAccessFormItem>();
    public IReadOnlyCollection<SecurityFormAccessOperationRow> Operations { get; private set; } = Array.Empty<SecurityFormAccessOperationRow>();

    public RoleItem? SelectedRole { get; private set; }
    public SecurityFormAccessFormItem? SelectedForm { get; private set; }

    public async Task LoadAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default)
    {
        Roles = await roleClient.GetAsync(cancellationToken);
        Forms = await formAccessClient.GetFormsAsync(MaintenanceFormType, onlyActive, search, cancellationToken);
        SelectedRole = Roles.FirstOrDefault(role => role.IsActive) ?? Roles.FirstOrDefault();
        SelectedForm = Forms.FirstOrDefault();
        await LoadOperationsAsync(null, onlyActive, cancellationToken);
    }

    public async Task LoadFormsAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default)
    {
        Forms = await formAccessClient.GetFormsAsync(MaintenanceFormType, onlyActive, search, cancellationToken);

        if (SelectedForm is null || Forms.All(form => form.Id != SelectedForm.Id))
        {
            SelectedForm = Forms.FirstOrDefault();
        }
    }

    public async Task SelectRoleAsync(RoleItem? role, bool onlyActive, string? operationSearch, CancellationToken cancellationToken = default)
    {
        SelectedRole = role;
        await LoadOperationsAsync(operationSearch, onlyActive, cancellationToken);
    }

    public async Task SelectFormAsync(SecurityFormAccessFormItem? form, bool onlyActive, string? operationSearch, CancellationToken cancellationToken = default)
    {
        SelectedForm = form;
        await LoadOperationsAsync(operationSearch, onlyActive, cancellationToken);
    }

    public async Task LoadOperationsAsync(string? search, bool onlyActive, CancellationToken cancellationToken = default)
    {
        if (SelectedRole is null || SelectedForm is null)
        {
            Operations = Array.Empty<SecurityFormAccessOperationRow>();
            return;
        }

        var operations = await formAccessClient.GetOperationsAsync(
            SelectedRole.Id,
            SelectedForm.Id,
            onlyActive,
            search,
            cancellationToken);

        Operations = operations.Select(operation => new SecurityFormAccessOperationRow(operation)).ToArray();
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedRole is null || SelectedForm is null)
        {
            return Task.CompletedTask;
        }

        var request = new SaveSecurityFormAccessRequest(
            Operations
                .Select(operation => new SaveSecurityFormAccessOperationRequest(operation.OperationId, operation.IsAllowed))
                .ToArray());

        return formAccessClient.SaveOperationsAsync(SelectedRole.Id, SelectedForm.Id, request, cancellationToken);
    }
}

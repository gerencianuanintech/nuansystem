using NuanSystem.WinForms.Services.Security.Roles;
using NuanSystem.WinForms.Services.Security.Roles.Models;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;

namespace NuanSystem.WinForms.ViewModels.Security.Access;

public sealed class SecurityFormFieldAccessViewModel(
    IRoleClient roleClient,
    ISecurityRoleFormAccessClient formAccessClient,
    ISecurityRoleFormFieldAccessClient fieldAccessClient,
    int formType)
{
    public IReadOnlyCollection<RoleItem> Roles { get; private set; } = Array.Empty<RoleItem>();
    public IReadOnlyCollection<SecurityFormAccessFormItem> Forms { get; private set; } = Array.Empty<SecurityFormAccessFormItem>();
    public IReadOnlyCollection<SecurityFormFieldAccessRow> Fields { get; private set; } = Array.Empty<SecurityFormFieldAccessRow>();

    public RoleItem? SelectedRole { get; private set; }
    public SecurityFormAccessFormItem? SelectedForm { get; private set; }

    public async Task LoadAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default)
    {
        Roles = await roleClient.GetAsync(cancellationToken);
        Forms = await formAccessClient.GetFormsAsync(formType, onlyActive, search, cancellationToken);
        SelectedRole = Roles.FirstOrDefault(role => role.IsActive) ?? Roles.FirstOrDefault();
        SelectedForm = Forms.FirstOrDefault();
        await LoadFieldsAsync(null, onlyActive, cancellationToken);
    }

    public async Task LoadFormsAsync(bool onlyActive, string? search, CancellationToken cancellationToken = default)
    {
        Forms = await formAccessClient.GetFormsAsync(formType, onlyActive, search, cancellationToken);
        if (SelectedForm is null || Forms.All(form => form.Id != SelectedForm.Id))
        {
            SelectedForm = Forms.FirstOrDefault();
        }

        await LoadFieldsAsync(null, onlyActive, cancellationToken);
    }

    public async Task SelectRoleAsync(RoleItem? role, string? search, bool onlyActive, CancellationToken cancellationToken = default)
    {
        SelectedRole = role;
        await LoadFieldsAsync(search, onlyActive, cancellationToken);
    }

    public async Task SelectFormAsync(SecurityFormAccessFormItem? form, string? search, bool onlyActive, CancellationToken cancellationToken = default)
    {
        SelectedForm = form;
        await LoadFieldsAsync(search, onlyActive, cancellationToken);
    }

    public async Task LoadFieldsAsync(string? search, bool onlyActive, CancellationToken cancellationToken = default)
    {
        if (SelectedRole is null || SelectedForm is null)
        {
            Fields = Array.Empty<SecurityFormFieldAccessRow>();
            return;
        }

        var fields = await fieldAccessClient.GetFieldsAsync(
            SelectedRole.Id,
            SelectedForm.Id,
            onlyActive,
            search,
            cancellationToken);

        Fields = fields.Select(field => new SecurityFormFieldAccessRow(field)).ToArray();
    }

    public Task SaveAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedRole is null || SelectedForm is null)
        {
            return Task.CompletedTask;
        }

        var request = new SaveSecurityFormFieldAccessRequest(
            Fields
                .Select(field => new SaveSecurityFormFieldAccessItemRequest(
                    field.FieldId,
                    field.IsVisible,
                    field.IsEditable,
                    field.IsRequired,
                    field.IsReadOnly,
                    field.IsActive))
                .ToArray());

        return fieldAccessClient.SaveAsync(SelectedRole.Id, SelectedForm.Id, request, cancellationToken);
    }

    public int FormType => formType;
}

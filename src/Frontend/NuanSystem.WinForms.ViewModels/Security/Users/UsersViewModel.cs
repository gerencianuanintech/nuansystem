using NuanSystem.WinForms.Services.Companies;
using NuanSystem.WinForms.Services.Companies.Models;
using NuanSystem.WinForms.Services.Security.Users;
using NuanSystem.WinForms.Services.Security.Users.Models;
using NuanSystem.WinForms.Services.Security.Access;
using NuanSystem.WinForms.Services.Security.Access.Models;
using NuanSystem.WinForms.ViewModels.Common;
using ISecurityRoleClient = NuanSystem.WinForms.Services.Security.Roles.IRoleClient;
using SaveSecurityRoleRequest = NuanSystem.WinForms.Services.Security.Roles.Models.SaveRoleRequest;

namespace NuanSystem.WinForms.ViewModels.Security.Users;

public sealed class UsersViewModel(
    IUserClient userClient,
    ICompanyClient companyClient,
    ISecurityRoleClient roleClient,
    ISecurityAccessClient securityAccessClient) : CrudViewModel<UserAdminItem, CreateUserRequest>
{
    private IReadOnlyCollection<RoleItem> roles = Array.Empty<RoleItem>();
    private IReadOnlyCollection<CompanyAdminItem> companies = Array.Empty<CompanyAdminItem>();

    public IReadOnlyCollection<RoleItem> Roles
    {
        get => roles;
        private set => SetProperty(ref roles, value);
    }

    public IReadOnlyCollection<CompanyAdminItem> Companies
    {
        get => companies;
        private set => SetProperty(ref companies, value);
    }

    public IReadOnlyCollection<UserAdminItem> Users => Items;

    public bool CanCreateRoles { get; private set; }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(userClient.GetAsync, cancellationToken);
    }

    public async Task LoadCatalogsAsync(CancellationToken cancellationToken = default)
    {
        Roles = (await userClient.GetRolesAsync(cancellationToken)).Where(role => role.IsActive).ToArray();
        Companies = (await companyClient.GetAllAsync(cancellationToken)).Where(company => company.IsActive).ToArray();
        CanCreateRoles = await HasCreateAccessAsync("security-roles", cancellationToken);
    }

    public override Task CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        return userClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        return userClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return userClient.DeleteAsync(id, cancellationToken);
    }

    public Task AssignCompanyAsync(int userId, int companyId, CancellationToken cancellationToken = default)
    {
        return companyClient.AssignUserAsync(new AssignUserCompanyRequest(userId, companyId), cancellationToken);
    }

    public async Task<RoleItem> CreateRoleAsync(SaveSecurityRoleRequest request, CancellationToken cancellationToken = default)
    {
        var created = await roleClient.CreateAsync(request, cancellationToken);
        return new RoleItem(
            created.Id,
            created.Code,
            created.Name,
            created.Description,
            created.IsActive);
    }

    private static bool IsCreateOperation(FormOperationAccessItem operation)
    {
        return operation.IsAllowed
            && (MatchesOperation(operation.ActionKey, "create", "new", "nuevo", "crear", "post")
                || MatchesOperation(operation.Code, "create", "new", "nuevo", "crear", "post")
                || MatchesOperation(operation.Name, "create", "new", "nuevo", "crear", "post"));
    }

    private async Task<bool> HasCreateAccessAsync(string formKey, CancellationToken cancellationToken)
    {
        try
        {
            var operations = await securityAccessClient.GetFormOperationsAsync(formKey, cancellationToken);
            return operations.Any(IsCreateOperation);
        }
        catch
        {
            return false;
        }
    }

    private static bool MatchesOperation(string? value, params string[] keys)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Trim();

        return keys.Any(key => string.Equals(normalized, key, StringComparison.OrdinalIgnoreCase));
    }
}


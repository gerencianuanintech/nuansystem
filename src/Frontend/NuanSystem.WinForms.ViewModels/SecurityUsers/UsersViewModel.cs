using NuanSystem.WinForms.Services.Companies;
using NuanSystem.WinForms.Services.Companies.Models;
using NuanSystem.WinForms.Services.SecurityUsers;
using NuanSystem.WinForms.Services.SecurityUsers.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.SecurityUsers;

public sealed class UsersViewModel(
    IUserClient userClient,
    ICompanyClient companyClient) : CrudViewModel<UserAdminItem, CreateUserRequest>
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

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(userClient.GetAsync, cancellationToken);
    }

    public async Task LoadCatalogsAsync(CancellationToken cancellationToken = default)
    {
        Roles = (await userClient.GetRolesAsync(cancellationToken)).Where(role => role.IsActive).ToArray();
        Companies = (await companyClient.GetAllAsync(cancellationToken)).Where(company => company.IsActive).ToArray();
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
}


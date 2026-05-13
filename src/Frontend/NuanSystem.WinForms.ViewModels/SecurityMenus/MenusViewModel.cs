using NuanSystem.WinForms.Services.SecurityMenus;
using NuanSystem.WinForms.Services.SecurityMenus.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.SecurityMenus;

public sealed class MenusViewModel(ISecurityMenuClient menuClient)
    : CrudViewModel<SecurityMenuItem, SaveSecurityMenuRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(menuClient.GetAsync, cancellationToken);
    }

    public override Task CreateAsync(SaveSecurityMenuRequest request, CancellationToken cancellationToken = default)
    {
        return menuClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveSecurityMenuRequest request, CancellationToken cancellationToken = default)
    {
        return menuClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return menuClient.DeleteAsync(id, cancellationToken);
    }
}

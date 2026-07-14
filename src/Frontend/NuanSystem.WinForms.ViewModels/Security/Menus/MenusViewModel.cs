using NuanSystem.WinForms.Services.Security.Forms;
using NuanSystem.WinForms.Services.Security.Forms.Models;
using NuanSystem.WinForms.Services.Security.Menus;
using NuanSystem.WinForms.Services.Security.Menus.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Security.Menus;

public sealed class MenusViewModel(IMenuClient menuClient, IFormClient formClient)
    : CrudViewModel<MenuItem, SaveMenuRequest>
{
    public IReadOnlyCollection<FormItem> Forms { get; private set; } = [];

    public async Task LoadFormsAsync(CancellationToken cancellationToken = default)
    {
        Forms = await formClient.GetAsync(cancellationToken);
    }

    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(menuClient.GetAsync, cancellationToken);
    }

    public override Task CreateAsync(SaveMenuRequest request, CancellationToken cancellationToken = default)
    {
        return menuClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveMenuRequest request, CancellationToken cancellationToken = default)
    {
        return menuClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return menuClient.DeleteAsync(id, cancellationToken);
    }
}

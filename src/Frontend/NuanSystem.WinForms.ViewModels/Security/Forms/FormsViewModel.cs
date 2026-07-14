using NuanSystem.WinForms.Services.Security.Forms;
using NuanSystem.WinForms.Services.Security.Forms.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Security.Forms;

public sealed class FormsViewModel(IFormClient formClient)
    : CrudViewModel<FormItem, SaveFormRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(formClient.GetAsync, cancellationToken);
    }

    public override Task CreateAsync(SaveFormRequest request, CancellationToken cancellationToken = default)
    {
        return formClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveFormRequest request, CancellationToken cancellationToken = default)
    {
        return formClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return formClient.DeleteAsync(id, cancellationToken);
    }
}

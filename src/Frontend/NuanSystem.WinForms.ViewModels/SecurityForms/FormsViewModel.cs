using NuanSystem.WinForms.Services.SecurityForms;
using NuanSystem.WinForms.Services.SecurityForms.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.SecurityForms;

public sealed class FormsViewModel(ISecurityFormClient formClient)
    : CrudViewModel<SecurityFormItem, SaveSecurityFormRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(formClient.GetAsync, cancellationToken);
    }

    public override Task CreateAsync(SaveSecurityFormRequest request, CancellationToken cancellationToken = default)
    {
        return formClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveSecurityFormRequest request, CancellationToken cancellationToken = default)
    {
        return formClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return formClient.DeleteAsync(id, cancellationToken);
    }
}

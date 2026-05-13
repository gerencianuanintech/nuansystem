using NuanSystem.WinForms.Services.SecurityFields;
using NuanSystem.WinForms.Services.SecurityFields.Models;
using NuanSystem.WinForms.Services.SecurityForms;
using NuanSystem.WinForms.Services.SecurityForms.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.SecurityFields;

public sealed class FieldsViewModel(ISecurityFieldClient fieldClient, ISecurityFormClient formClient)
    : CrudViewModel<SecurityFieldItem, SaveSecurityFieldRequest>
{
    public IReadOnlyCollection<SecurityFormItem> Forms { get; private set; } = Array.Empty<SecurityFormItem>();

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Forms = await formClient.GetAsync(cancellationToken);
        await LoadItemsAsync(fieldClient.GetAsync, cancellationToken);
    }

    public override Task CreateAsync(SaveSecurityFieldRequest request, CancellationToken cancellationToken = default)
    {
        return fieldClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveSecurityFieldRequest request, CancellationToken cancellationToken = default)
    {
        return fieldClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return fieldClient.DeleteAsync(id, cancellationToken);
    }
}

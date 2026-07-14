using NuanSystem.WinForms.Services.Security.Fields;
using NuanSystem.WinForms.Services.Security.Fields.Models;
using NuanSystem.WinForms.Services.Security.Forms;
using NuanSystem.WinForms.Services.Security.Forms.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Security.Fields;

public sealed class FieldsViewModel(IFieldClient fieldClient, IFormClient formClient)
    : CrudViewModel<FieldItem, SaveFieldRequest>
{
    public IReadOnlyCollection<FormItem> Forms { get; private set; } = Array.Empty<FormItem>();

    public override async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        Forms = await formClient.GetAsync(cancellationToken);
        await LoadItemsAsync(fieldClient.GetAsync, cancellationToken);
    }

    public override Task CreateAsync(SaveFieldRequest request, CancellationToken cancellationToken = default)
    {
        return fieldClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveFieldRequest request, CancellationToken cancellationToken = default)
    {
        return fieldClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return fieldClient.DeleteAsync(id, cancellationToken);
    }
}

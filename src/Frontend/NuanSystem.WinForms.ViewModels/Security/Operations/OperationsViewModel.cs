using NuanSystem.WinForms.Services.Security.Operations;
using NuanSystem.WinForms.Services.Security.Operations.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.Security.Operations;

public sealed class OperationsViewModel(IOperationClient operationClient)
    : CrudViewModel<OperationItem, SaveOperationRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(operationClient.GetAsync, cancellationToken);
    }

    public override Task CreateAsync(SaveOperationRequest request, CancellationToken cancellationToken = default)
    {
        return operationClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveOperationRequest request, CancellationToken cancellationToken = default)
    {
        return operationClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return operationClient.DeleteAsync(id, cancellationToken);
    }
}

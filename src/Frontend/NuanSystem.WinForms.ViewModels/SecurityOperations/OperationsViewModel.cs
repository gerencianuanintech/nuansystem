using NuanSystem.WinForms.Services.SecurityOperations;
using NuanSystem.WinForms.Services.SecurityOperations.Models;
using NuanSystem.WinForms.ViewModels.Common;

namespace NuanSystem.WinForms.ViewModels.SecurityOperations;

public sealed class OperationsViewModel(ISecurityOperationClient operationClient)
    : CrudViewModel<SecurityOperationItem, SaveSecurityOperationRequest>
{
    public override Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return LoadItemsAsync(operationClient.GetAsync, cancellationToken);
    }

    public override Task CreateAsync(SaveSecurityOperationRequest request, CancellationToken cancellationToken = default)
    {
        return operationClient.CreateAsync(request, cancellationToken);
    }

    public override Task UpdateAsync(int id, SaveSecurityOperationRequest request, CancellationToken cancellationToken = default)
    {
        return operationClient.UpdateAsync(id, request, cancellationToken);
    }

    public override Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return operationClient.DeleteAsync(id, cancellationToken);
    }
}

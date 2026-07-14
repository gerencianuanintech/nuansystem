using NuanSystem.WinForms.Services.Security.Operations.Models;

namespace NuanSystem.WinForms.Services.Security.Operations;

public interface IOperationClient
{
    Task<IReadOnlyCollection<OperationItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<OperationItem> CreateAsync(SaveOperationRequest request, CancellationToken cancellationToken = default);

    Task<OperationItem> UpdateAsync(int id, SaveOperationRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

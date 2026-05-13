using NuanSystem.WinForms.Services.SecurityOperations.Models;

namespace NuanSystem.WinForms.Services.SecurityOperations;

public interface ISecurityOperationClient
{
    Task<IReadOnlyCollection<SecurityOperationItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<SecurityOperationItem> CreateAsync(SaveSecurityOperationRequest request, CancellationToken cancellationToken = default);

    Task<SecurityOperationItem> UpdateAsync(int id, SaveSecurityOperationRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

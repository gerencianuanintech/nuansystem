using NuanSystem.WinForms.Services.SecurityFields.Models;

namespace NuanSystem.WinForms.Services.SecurityFields;

public interface ISecurityFieldClient
{
    Task<IReadOnlyCollection<SecurityFieldItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<SecurityFieldItem> CreateAsync(SaveSecurityFieldRequest request, CancellationToken cancellationToken = default);

    Task<SecurityFieldItem> UpdateAsync(int id, SaveSecurityFieldRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

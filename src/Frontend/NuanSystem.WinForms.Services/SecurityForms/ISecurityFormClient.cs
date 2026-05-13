using NuanSystem.WinForms.Services.SecurityForms.Models;

namespace NuanSystem.WinForms.Services.SecurityForms;

public interface ISecurityFormClient
{
    Task<IReadOnlyCollection<SecurityFormItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<SecurityFormItem> CreateAsync(SaveSecurityFormRequest request, CancellationToken cancellationToken = default);

    Task<SecurityFormItem> UpdateAsync(int id, SaveSecurityFormRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

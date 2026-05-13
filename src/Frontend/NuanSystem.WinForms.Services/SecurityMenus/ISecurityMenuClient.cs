using NuanSystem.WinForms.Services.SecurityMenus.Models;

namespace NuanSystem.WinForms.Services.SecurityMenus;

public interface ISecurityMenuClient
{
    Task<IReadOnlyCollection<SecurityMenuItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<SecurityMenuItem> CreateAsync(SaveSecurityMenuRequest request, CancellationToken cancellationToken = default);

    Task<SecurityMenuItem> UpdateAsync(int id, SaveSecurityMenuRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

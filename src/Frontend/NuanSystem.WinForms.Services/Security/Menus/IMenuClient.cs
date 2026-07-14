using NuanSystem.WinForms.Services.Security.Menus.Models;

namespace NuanSystem.WinForms.Services.Security.Menus;

public interface IMenuClient
{
    Task<IReadOnlyCollection<MenuItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<MenuItem> CreateAsync(SaveMenuRequest request, CancellationToken cancellationToken = default);

    Task<MenuItem> UpdateAsync(int id, SaveMenuRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

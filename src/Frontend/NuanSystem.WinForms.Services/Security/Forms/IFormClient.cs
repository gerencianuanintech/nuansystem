using NuanSystem.WinForms.Services.Security.Forms.Models;

namespace NuanSystem.WinForms.Services.Security.Forms;

public interface IFormClient
{
    Task<IReadOnlyCollection<FormItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<FormItem> CreateAsync(SaveFormRequest request, CancellationToken cancellationToken = default);

    Task<FormItem> UpdateAsync(int id, SaveFormRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

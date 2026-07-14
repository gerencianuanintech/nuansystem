using NuanSystem.WinForms.Services.Security.Fields.Models;

namespace NuanSystem.WinForms.Services.Security.Fields;

public interface IFieldClient
{
    Task<IReadOnlyCollection<FieldItem>> GetAsync(CancellationToken cancellationToken = default);

    Task<FieldItem> CreateAsync(SaveFieldRequest request, CancellationToken cancellationToken = default);

    Task<FieldItem> UpdateAsync(int id, SaveFieldRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

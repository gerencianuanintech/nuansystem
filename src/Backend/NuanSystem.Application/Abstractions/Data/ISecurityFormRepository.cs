using NuanSystem.Application.Features.SecurityForms.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityFormRepository
{
    Task<IReadOnlyCollection<SecurityFormDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SecurityFormDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateSecurityFormData form, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByFormKeyAsync(string formKey, CancellationToken cancellationToken = default);

    Task<bool> ExistsByFormKeyAsync(string formKey, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateSecurityFormData form, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}

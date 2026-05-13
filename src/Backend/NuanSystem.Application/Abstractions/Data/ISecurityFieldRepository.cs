using NuanSystem.Application.Features.SecurityFields.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityFieldRepository
{
    Task<IReadOnlyCollection<SecurityFieldDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SecurityFieldDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateSecurityFieldData field, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByFieldKeyAsync(int formId, string fieldKey, CancellationToken cancellationToken = default);

    Task<bool> ExistsByFieldKeyAsync(int formId, string fieldKey, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateSecurityFieldData field, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}

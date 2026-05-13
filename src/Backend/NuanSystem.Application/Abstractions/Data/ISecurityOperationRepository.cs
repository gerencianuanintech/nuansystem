using NuanSystem.Application.Features.SecurityOperations.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityOperationRepository
{
    Task<IReadOnlyCollection<SecurityOperationDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<SecurityOperationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateSecurityOperationData operation, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name, int excludingId, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateSecurityOperationData operation, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}

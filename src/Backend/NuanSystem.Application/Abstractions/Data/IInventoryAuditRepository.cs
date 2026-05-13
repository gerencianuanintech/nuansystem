using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IInventoryAuditRepository
{
    Task<IReadOnlyCollection<SecurityChangeDto>> GetChangesAsync(
        string entityName,
        string recordId,
        int take,
        CancellationToken cancellationToken = default);
}

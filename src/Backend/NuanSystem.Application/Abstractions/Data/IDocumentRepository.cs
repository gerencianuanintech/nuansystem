using NuanSystem.Application.Features.Documents.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IDocumentRepository : IRepository
{
    Task<IReadOnlyCollection<DocumentSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DocumentDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<long> CreateAsync(CreateDocumentData document, CancellationToken cancellationToken = default);

    Task<bool> CustomerExistsAsync(int customerId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<int>> GetMissingItemIdsAsync(
        IEnumerable<int> itemIds,
        CancellationToken cancellationToken = default);
}

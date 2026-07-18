using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Abstractions.Sap;

public interface ISapItemReader
{
    Task<IReadOnlyCollection<SapItemRecord>> GetItemsAsync(
        int companyId,
        SapItemReadOptions? options = null,
        CancellationToken cancellationToken = default);
}

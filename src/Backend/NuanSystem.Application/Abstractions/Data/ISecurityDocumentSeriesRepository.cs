using NuanSystem.Application.Features.Documents.SecurityDocumentSeries.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface ISecurityDocumentSeriesRepository
{
    Task<IReadOnlyCollection<SecurityDocumentSeriesDto>> GetAllAsync(SecurityDocumentSeriesFilterData filter, CancellationToken cancellationToken = default);

    Task<SecurityDocumentSeriesDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityDocumentSeriesLookupDto>> GetLookupAsync(string? documentType, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string code, int? excludeId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsBySeriesKeyAsync(string documentType, string prefix, string establishment, string emissionPoint, int? excludeId = null, CancellationToken cancellationToken = default);

    Task<int> CreateAsync(CreateSecurityDocumentSeriesData data, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(UpdateSecurityDocumentSeriesData data, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, int? deletedByUserId, string? deletedByUserName, CancellationToken cancellationToken = default);
}

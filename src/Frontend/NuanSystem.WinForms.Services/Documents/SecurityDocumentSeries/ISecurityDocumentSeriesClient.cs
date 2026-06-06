using NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries.Models;

namespace NuanSystem.WinForms.Services.Documents.SecurityDocumentSeries;

public interface ISecurityDocumentSeriesClient
{
    Task<IReadOnlyCollection<SecurityDocumentSeriesItem>> GetAsync(
        string? search = null,
        string? documentType = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<SecurityDocumentSeriesItem> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityDocumentSeriesLookupItem>> GetLookupAsync(
        string? documentType = null,
        CancellationToken cancellationToken = default);

    Task<SecurityDocumentSeriesItem> CreateAsync(
        SaveSecurityDocumentSeriesRequest request,
        CancellationToken cancellationToken = default);

    Task<SecurityDocumentSeriesItem> UpdateAsync(
        int id,
        SaveSecurityDocumentSeriesRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<ReserveSecurityDocumentNumberResult> ReserveNumberAsync(
        int id,
        CancellationToken cancellationToken = default);
}

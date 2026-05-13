using NuanSystem.WinForms.Services.Documents.Models;

namespace NuanSystem.WinForms.Services.Documents;

public interface IDocumentClient
{
    Task<IReadOnlyCollection<DocumentSummaryItem>> GetAsync(CancellationToken cancellationToken = default);
    Task<DocumentDetailItem> CreateAsync(CreateDocumentRequest request, CancellationToken cancellationToken = default);
}

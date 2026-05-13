using NuanSystem.WinForms.Services.Documents.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Documents;

public sealed class DocumentClient : IDocumentClient
{
    private readonly INuanApiClient apiClient;

    public DocumentClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<DocumentSummaryItem>> GetAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<DocumentSummaryItem>>("/api/documents", cancellationToken);
    }

    public Task<DocumentDetailItem> CreateAsync(CreateDocumentRequest request, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<CreateDocumentRequest, DocumentDetailItem>(
            "/api/documents",
            request,
            cancellationToken);
    }
}

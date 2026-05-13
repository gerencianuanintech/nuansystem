using NuanSystem.WinForms.Services.Http;
using NuanSystem.WinForms.Services.Sap.Models;

namespace NuanSystem.WinForms.Services.Sap;

public sealed class SapClient : ISapClient
{
    private readonly INuanApiClient apiClient;

    public SapClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<SapSyncLogItem>> GetSyncLogsAsync(CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<SapSyncLogItem>>(
            "/api/sap/sync-logs",
            cancellationToken);
    }

    public Task<SapSendResult> SendDocumentAsync(long documentId, CancellationToken cancellationToken = default)
    {
        return apiClient.PostAsync<object, SapSendResult>(
            $"/api/sap/send-document/{documentId}",
            new { },
            cancellationToken);
    }
}

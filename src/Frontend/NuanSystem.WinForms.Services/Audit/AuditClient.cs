using NuanSystem.WinForms.Services.Audit.Models;
using NuanSystem.WinForms.Services.Http;

namespace NuanSystem.WinForms.Services.Audit;

public sealed class AuditClient : IAuditClient
{
    private readonly INuanApiClient apiClient;

    public AuditClient(INuanApiClient apiClient)
    {
        this.apiClient = apiClient;
    }

    public async Task<IReadOnlyCollection<AuditLogItem>> GetLogsAsync(int take = 200, CancellationToken cancellationToken = default)
    {
        return await apiClient.GetAsync<List<AuditLogItem>>(
            $"/api/audit/logs?take={take}",
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<SecurityChangeItem>> GetSecurityChangesAsync(
        string entityName,
        string recordId,
        int take = 200,
        CancellationToken cancellationToken = default)
    {
        var encodedEntity = Uri.EscapeDataString(entityName);
        var encodedRecord = Uri.EscapeDataString(recordId);

        return await apiClient.GetAsync<List<SecurityChangeItem>>(
            $"/api/audit/security-changes?entityName={encodedEntity}&recordId={encodedRecord}&take={take}",
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<SecurityChangeItem>> GetInventoryChangesAsync(
        string entityName,
        string recordId,
        int take = 200,
        CancellationToken cancellationToken = default)
    {
        var encodedEntity = Uri.EscapeDataString(entityName);
        var encodedRecord = Uri.EscapeDataString(recordId);

        return await apiClient.GetAsync<List<SecurityChangeItem>>(
            $"/api/audit/inventory-changes?entityName={encodedEntity}&recordId={encodedRecord}&take={take}",
            cancellationToken);
    }

    public async Task RegisterErrorAsync(CreateAuditErrorLogRequest request, CancellationToken cancellationToken = default)
    {
        await apiClient.PostAsync<CreateAuditErrorLogRequest, bool>("/api/audit/error-logs", request, cancellationToken);
    }
}

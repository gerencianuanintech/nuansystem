using NuanSystem.WinForms.Services.Audit.Models;

namespace NuanSystem.WinForms.Services.Audit;

public interface IAuditClient
{
    Task<IReadOnlyCollection<AuditLogItem>> GetLogsAsync(int take = 200, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SecurityChangeItem>> GetSecurityChangesAsync(
        string entityName,
        string recordId,
        int take = 200,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SecurityChangeItem>> GetInventoryChangesAsync(
        string entityName,
        string recordId,
        int take = 200,
        CancellationToken cancellationToken = default);

    Task RegisterErrorAsync(CreateAuditErrorLogRequest request, CancellationToken cancellationToken = default);
}

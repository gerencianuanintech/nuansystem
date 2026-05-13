using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Application.Abstractions.Data;

public interface IAuditLogRepository
{
    Task AddAsync(CreateAuditLogData auditLog, CancellationToken cancellationToken = default);
    Task AddErrorAsync(CreateAuditErrorLogData errorLog, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuditLogDto>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AuditErrorLogDto>> GetRecentErrorsAsync(int take, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SecurityChangeDto>> GetSecurityChangesAsync(
        string entityName,
        string recordId,
        int take,
        CancellationToken cancellationToken = default);
}

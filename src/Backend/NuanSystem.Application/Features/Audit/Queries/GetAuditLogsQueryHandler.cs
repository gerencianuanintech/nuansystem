using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Application.Features.Audit.Queries;

public sealed class GetAuditLogsQueryHandler(IAuditLogRepository repository)
    : IQueryHandler<GetAuditLogsQuery, IReadOnlyCollection<AuditLogDto>>
{
    public async Task<Result<IReadOnlyCollection<AuditLogDto>>> Handle(
        GetAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var take = Math.Clamp(request.Take, 1, 500);
        var logs = await repository.GetRecentAsync(take, cancellationToken);

        return Result<IReadOnlyCollection<AuditLogDto>>.Success(logs);
    }
}

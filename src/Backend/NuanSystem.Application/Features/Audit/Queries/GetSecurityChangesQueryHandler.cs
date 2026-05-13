using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Audit.Dtos;

namespace NuanSystem.Application.Features.Audit.Queries;

public sealed class GetSecurityChangesQueryHandler(IAuditLogRepository repository)
    : IQueryHandler<GetSecurityChangesQuery, IReadOnlyCollection<SecurityChangeDto>>
{
    public async Task<Result<IReadOnlyCollection<SecurityChangeDto>>> Handle(
        GetSecurityChangesQuery request,
        CancellationToken cancellationToken)
    {
        var entityName = request.EntityName.Trim();
        var recordId = request.RecordId.Trim();

        if (string.IsNullOrWhiteSpace(entityName) || string.IsNullOrWhiteSpace(recordId))
        {
            return Result<IReadOnlyCollection<SecurityChangeDto>>.Success([]);
        }

        var take = Math.Clamp(request.Take, 1, 500);
        var changes = await repository.GetSecurityChangesAsync(entityName, recordId, take, cancellationToken);

        return Result<IReadOnlyCollection<SecurityChangeDto>>.Success(changes);
    }
}

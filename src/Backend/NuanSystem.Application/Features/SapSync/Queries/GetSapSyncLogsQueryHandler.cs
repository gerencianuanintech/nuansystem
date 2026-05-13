using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sap;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SapSync.Dtos;

namespace NuanSystem.Application.Features.SapSync.Queries;

public sealed class GetSapSyncLogsQueryHandler(ISapSyncLogRepository sapSyncLogRepository)
    : IQueryHandler<GetSapSyncLogsQuery, IReadOnlyCollection<SapSyncLogDto>>
{
    public async Task<Result<IReadOnlyCollection<SapSyncLogDto>>> Handle(
        GetSapSyncLogsQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await sapSyncLogRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<SapSyncLogDto>>.Success(logs);
    }
}

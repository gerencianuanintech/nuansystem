using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Abstractions.Sync;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Sync.Configuration.Dtos;
using NuanSystem.Application.Features.Sync.Execution.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Sync.Execution.Queries;

public sealed class GetSyncProfileExecutionsQueryHandler(ISyncProfileExecutionRepository repository)
    : IQueryHandler<GetSyncProfileExecutionsQuery, PagedResultDto<SyncProfileExecutionListItemDto>>
{
    public async Task<Result<PagedResultDto<SyncProfileExecutionListItemDto>>> Handle(
        GetSyncProfileExecutionsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await repository.SearchAsync(
            new SyncProfileExecutionFilter(
                request.ProfileId,
                request.Status,
                request.ExecutionType,
                request.DateFrom,
                request.DateTo,
                request.PageNumber,
                request.PageSize),
            cancellationToken);

        return Result<PagedResultDto<SyncProfileExecutionListItemDto>>.Success(result);
    }
}

public sealed class GetSyncProfileExecutionByIdQueryHandler(ISyncProfileExecutionRepository repository)
    : IQueryHandler<GetSyncProfileExecutionByIdQuery, SyncProfileExecutionDetailDto>
{
    public async Task<Result<SyncProfileExecutionDetailDto>> Handle(
        GetSyncProfileExecutionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var execution = await repository.GetByIdAsync(request.ExecutionId, cancellationToken);
        return execution is null
            ? Result<SyncProfileExecutionDetailDto>.Failure(
                "Ejecucion de sincronizacion no encontrada.",
                [new ApiError("SyncProfileExecutionNotFound", "La ejecucion no existe.", nameof(request.ExecutionId))])
            : Result<SyncProfileExecutionDetailDto>.Success(execution);
    }
}

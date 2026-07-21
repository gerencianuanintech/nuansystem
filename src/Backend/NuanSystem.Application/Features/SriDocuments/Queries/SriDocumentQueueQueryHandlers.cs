using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.SriDocuments.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.SriDocuments.Queries;

public sealed class GetSriDocumentQueueQueryHandler(ISriDocumentQueueRepository repository) : IQueryHandler<GetSriDocumentQueueQuery, IReadOnlyCollection<SriDocumentQueueListItemDto>>
{
    public async Task<Result<IReadOnlyCollection<SriDocumentQueueListItemDto>>> Handle(GetSriDocumentQueueQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter with
        {
            Environment = string.IsNullOrWhiteSpace(request.Filter.Environment) ? null : SriEnvironmentCodes.Normalize(request.Filter.Environment),
            Status = SriDocumentQueueStatusCodes.NormalizeOptional(request.Filter.Status),
            SourceType = string.IsNullOrWhiteSpace(request.Filter.SourceType) ? null : SriSourceTypeCodes.Normalize(request.Filter.SourceType),
            AccessKey = string.IsNullOrWhiteSpace(request.Filter.AccessKey) ? null : request.Filter.AccessKey.Trim()
        };
        return Result<IReadOnlyCollection<SriDocumentQueueListItemDto>>.Success(await repository.SearchAsync(filter, cancellationToken));
    }
}

public sealed class GetSriDocumentQueueByIdQueryHandler(ISriDocumentQueueRepository repository) : IQueryHandler<GetSriDocumentQueueByIdQuery, SriDocumentQueueDetailDto>
{
    public async Task<Result<SriDocumentQueueDetailDto>> Handle(GetSriDocumentQueueByIdQuery request, CancellationToken cancellationToken)
    {
        var detail = await repository.GetByIdAsync(request.Id, cancellationToken);
        return detail is null ? Result<SriDocumentQueueDetailDto>.Failure("Consulta SRI no encontrada.", [new ApiError("SRI_QUEUE_NOT_FOUND", "La consulta SRI no existe.", "Id")]) : Result<SriDocumentQueueDetailDto>.Success(detail);
    }
}

public sealed class GetSriDocumentAttemptsQueryHandler(ISriDocumentQueueRepository repository) : IQueryHandler<GetSriDocumentAttemptsQuery, IReadOnlyCollection<SriDocumentAttemptDto>>
{
    public async Task<Result<IReadOnlyCollection<SriDocumentAttemptDto>>> Handle(GetSriDocumentAttemptsQuery request, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<SriDocumentAttemptDto>>.Success(await repository.GetAttemptsAsync(request.QueueId, cancellationToken));
}

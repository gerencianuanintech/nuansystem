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

public sealed class GetSriDocumentMonitorSummaryQueryHandler(ISriDocumentQueueRepository repository) : IQueryHandler<GetSriDocumentMonitorSummaryQuery, SriDocumentMonitorSummaryDto>
{
    public async Task<Result<SriDocumentMonitorSummaryDto>> Handle(GetSriDocumentMonitorSummaryQuery request, CancellationToken cancellationToken) => Result<SriDocumentMonitorSummaryDto>.Success(await repository.GetMonitorSummaryAsync(request.ImportId, cancellationToken));
}

public sealed class SearchSriDocumentMonitorQueryHandler(ISriDocumentQueueRepository repository) : IQueryHandler<SearchSriDocumentMonitorQuery, IReadOnlyCollection<SriDocumentMonitorListItemDto>>
{
    public async Task<Result<IReadOnlyCollection<SriDocumentMonitorListItemDto>>> Handle(SearchSriDocumentMonitorQuery request, CancellationToken cancellationToken) => Result<IReadOnlyCollection<SriDocumentMonitorListItemDto>>.Success(await repository.SearchMonitorAsync(request.Filter, cancellationToken));
}

public sealed class GetSriDocumentMonitorDetailQueryHandler(ISriDocumentQueueRepository repository) : IQueryHandler<GetSriDocumentMonitorDetailQuery, SriDocumentMonitorDetailDto>
{
    public async Task<Result<SriDocumentMonitorDetailDto>> Handle(GetSriDocumentMonitorDetailQuery request, CancellationToken cancellationToken)
    {
        var value = await repository.GetMonitorDetailAsync(request.QueueId, cancellationToken);
        return value is null
            ? Result<SriDocumentMonitorDetailDto>.Failure("Documento SRI no encontrado.", [new ApiError("SRI_DOCUMENT_NOT_FOUND", "El documento SRI no existe.", "QueueId")])
            : Result<SriDocumentMonitorDetailDto>.Success(value);
    }
}

public sealed class GetSriDocumentAuditQueryHandler(ISriDocumentQueueRepository repository) : IQueryHandler<GetSriDocumentAuditQuery, IReadOnlyCollection<SriDocumentAuditDto>>
{
    public async Task<Result<IReadOnlyCollection<SriDocumentAuditDto>>> Handle(GetSriDocumentAuditQuery request, CancellationToken cancellationToken) => Result<IReadOnlyCollection<SriDocumentAuditDto>>.Success(await repository.GetAuditAsync(request.QueueId, cancellationToken));
}

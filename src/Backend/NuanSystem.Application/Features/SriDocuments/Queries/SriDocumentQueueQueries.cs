using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SriDocuments.Dtos;

namespace NuanSystem.Application.Features.SriDocuments.Queries;

public sealed record GetSriDocumentQueueQuery(SriDocumentQueueFilter Filter) : IQuery<IReadOnlyCollection<SriDocumentQueueListItemDto>>;
public sealed record GetSriDocumentQueueByIdQuery(long Id) : IQuery<SriDocumentQueueDetailDto>;
public sealed record GetSriDocumentAttemptsQuery(long QueueId) : IQuery<IReadOnlyCollection<SriDocumentAttemptDto>>;

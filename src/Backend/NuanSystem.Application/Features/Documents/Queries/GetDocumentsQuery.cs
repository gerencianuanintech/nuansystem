using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Documents.Dtos;

namespace NuanSystem.Application.Features.Documents.Queries;

public sealed record GetDocumentsQuery : IQuery<IReadOnlyCollection<DocumentSummaryDto>>;

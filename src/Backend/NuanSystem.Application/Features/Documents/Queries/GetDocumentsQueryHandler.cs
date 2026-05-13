using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Documents.Dtos;

namespace NuanSystem.Application.Features.Documents.Queries;

public sealed class GetDocumentsQueryHandler(IDocumentRepository documentRepository)
    : IQueryHandler<GetDocumentsQuery, IReadOnlyCollection<DocumentSummaryDto>>
{
    public async Task<Result<IReadOnlyCollection<DocumentSummaryDto>>> Handle(
        GetDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        var documents = await documentRepository.GetAllAsync(cancellationToken);

        return Result<IReadOnlyCollection<DocumentSummaryDto>>.Success(documents);
    }
}

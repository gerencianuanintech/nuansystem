using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Documents.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Documents.Queries;

public sealed class GetDocumentByIdQueryHandler(IDocumentRepository documentRepository)
    : IQueryHandler<GetDocumentByIdQuery, DocumentDto>
{
    public async Task<Result<DocumentDto>> Handle(
        GetDocumentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var document = await documentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (document is null)
        {
            return Result<DocumentDto>.Failure(
                "Documento no encontrado.",
                new[] { new ApiError("DocumentNotFound", "No existe el documento indicado.", nameof(request.Id)) });
        }

        return Result<DocumentDto>.Success(document);
    }
}

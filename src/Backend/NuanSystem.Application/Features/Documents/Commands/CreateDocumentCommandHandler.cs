using NuanSystem.Application.Abstractions.Data;
using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Common.Models;
using NuanSystem.Application.Features.Documents.Dtos;
using NuanSystem.Shared.Responses;

namespace NuanSystem.Application.Features.Documents.Commands;

public sealed class CreateDocumentCommandHandler(IDocumentRepository documentRepository)
    : ICommandHandler<CreateDocumentCommand, DocumentDto>
{
    public async Task<Result<DocumentDto>> Handle(
        CreateDocumentCommand request,
        CancellationToken cancellationToken)
    {
        if (!await documentRepository.CustomerExistsAsync(request.CustomerId, cancellationToken))
        {
            return Result<DocumentDto>.Failure(
                "Cliente no encontrado.",
                new[] { new ApiError("CustomerNotFound", "No existe el cliente indicado.", nameof(request.CustomerId)) });
        }

        var itemIds = request.Lines.Select(line => line.ItemId).Distinct().ToArray();
        var missingItemIds = await documentRepository.GetMissingItemIdsAsync(itemIds, cancellationToken);
        if (missingItemIds.Count > 0)
        {
            return Result<DocumentDto>.Failure(
                "Uno o mas articulos no existen o estan inactivos.",
                missingItemIds.Select(id => new ApiError("ItemNotFound", $"Articulo no disponible: {id}", nameof(request.Lines))).ToArray());
        }

        var lines = request.Lines
            .Select((line, index) =>
            {
                var lineTotal = decimal.Round(line.Quantity * line.UnitPrice, 6, MidpointRounding.AwayFromZero);

                return new CreateDocumentLineData(
                    index + 1,
                    line.ItemId,
                    line.Quantity,
                    line.UnitPrice,
                    line.TaxRate,
                    lineTotal);
            })
            .ToArray();

        var subtotal = lines.Sum(line => line.LineTotal);
        var taxTotal = lines.Sum(line => decimal.Round(line.LineTotal * line.TaxRate, 6, MidpointRounding.AwayFromZero));
        var total = subtotal + taxTotal;

        var documentId = await documentRepository.CreateAsync(new CreateDocumentData(
            NormalizeDocumentType(request.DocumentType),
            request.CustomerId,
            request.DocumentDate,
            request.Currency.Trim().ToUpperInvariant(),
            subtotal,
            taxTotal,
            total,
            lines), cancellationToken);

        var document = await documentRepository.GetByIdAsync(documentId, cancellationToken)
            ?? throw new InvalidOperationException("El documento fue creado pero no pudo consultarse.");

        return Result<DocumentDto>.Success(document, "Documento creado correctamente.");
    }

    private static string NormalizeDocumentType(string documentType)
    {
        return documentType.Trim() switch
        {
            var value when value.Equals("SalesOrder", StringComparison.OrdinalIgnoreCase) => "SalesOrder",
            var value when value.Equals("Delivery", StringComparison.OrdinalIgnoreCase) => "Delivery",
            var value when value.Equals("Invoice", StringComparison.OrdinalIgnoreCase) => "Invoice",
            _ => documentType.Trim()
        };
    }
}

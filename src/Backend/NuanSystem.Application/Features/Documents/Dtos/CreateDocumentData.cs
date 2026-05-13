namespace NuanSystem.Application.Features.Documents.Dtos;

public sealed record CreateDocumentData(
    string DocumentType,
    int CustomerId,
    DateOnly DocumentDate,
    string Currency,
    decimal Subtotal,
    decimal TaxTotal,
    decimal Total,
    IReadOnlyCollection<CreateDocumentLineData> Lines);

namespace NuanSystem.Application.Features.Documents.Dtos;

public sealed record DocumentDto(
    long Id,
    string DocumentType,
    string? DocumentNumber,
    int CustomerId,
    string CustomerCode,
    string CustomerName,
    DateOnly DocumentDate,
    string Status,
    string Currency,
    decimal Subtotal,
    decimal TaxTotal,
    decimal Total,
    IReadOnlyCollection<DocumentLineDto> Lines);

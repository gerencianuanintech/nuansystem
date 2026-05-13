namespace NuanSystem.Application.Features.Documents.Dtos;

public sealed record CreateDocumentLineData(
    int LineNumber,
    int ItemId,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRate,
    decimal LineTotal);

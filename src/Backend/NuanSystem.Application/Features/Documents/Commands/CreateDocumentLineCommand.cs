namespace NuanSystem.Application.Features.Documents.Commands;

public sealed record CreateDocumentLineCommand(
    int ItemId,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRate);

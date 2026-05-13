namespace NuanSystem.Application.Features.Documents.Dtos;

public sealed record DocumentLineDto(
    long Id,
    long DocumentId,
    int LineNumber,
    int ItemId,
    string ItemCode,
    string ItemName,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRate,
    decimal LineTotal);

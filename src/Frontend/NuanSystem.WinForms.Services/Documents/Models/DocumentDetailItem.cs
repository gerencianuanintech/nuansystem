namespace NuanSystem.WinForms.Services.Documents.Models;

public sealed record DocumentDetailItem(
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
    IReadOnlyCollection<DocumentLineItem> Lines);

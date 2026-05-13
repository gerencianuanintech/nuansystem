namespace NuanSystem.WinForms.Services.Documents.Models;

public sealed record CreateDocumentLineRequest(
    int ItemId,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRate);

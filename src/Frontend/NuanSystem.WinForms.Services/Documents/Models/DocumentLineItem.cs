namespace NuanSystem.WinForms.Services.Documents.Models;

public sealed record DocumentLineItem(
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

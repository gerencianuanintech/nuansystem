namespace NuanSystem.SapIntegration.Documents;

public sealed record SapDocumentLinePayload(
    int LineNumber,
    string ItemCode,
    decimal Quantity,
    decimal UnitPrice,
    decimal TaxRate);

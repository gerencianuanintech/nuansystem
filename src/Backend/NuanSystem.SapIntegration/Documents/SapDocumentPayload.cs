namespace NuanSystem.SapIntegration.Documents;

public sealed record SapDocumentPayload(
    long DocumentId,
    string DocumentType,
    string? DocumentNumber,
    string CustomerCode,
    DateOnly DocumentDate,
    string Currency,
    decimal Total,
    IReadOnlyCollection<SapDocumentLinePayload> Lines);

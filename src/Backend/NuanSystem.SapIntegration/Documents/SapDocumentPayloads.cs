namespace NuanSystem.SapIntegration.Documents;

public sealed record SapDocumentPayload(
    long DocumentId,
    string DocumentType,
    string? DocumentNumber,
    string CustomerCode,
    DateOnly DocumentDate,
    string Currency,
    IReadOnlyCollection<SapDocumentLinePayload> Lines);

public sealed record SapDocumentLinePayload(
    string ItemCode,
    decimal Quantity,
    decimal UnitPrice);

public sealed record SapClientResult(
    bool Success,
    string Status,
    string? ErrorMessage,
    string? RequestJson,
    string? ResponseJson,
    int? SapDocEntry,
    int? SapDocNum);

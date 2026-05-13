namespace NuanSystem.WinForms.Services.Documents.Models;

public sealed record CreateDocumentRequest(
    string DocumentType,
    int CustomerId,
    DateOnly DocumentDate,
    string Currency,
    IReadOnlyCollection<CreateDocumentLineRequest> Lines);

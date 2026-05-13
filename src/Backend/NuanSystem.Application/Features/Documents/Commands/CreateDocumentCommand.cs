using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Documents.Dtos;

namespace NuanSystem.Application.Features.Documents.Commands;

public sealed record CreateDocumentCommand(
    string DocumentType,
    int CustomerId,
    DateOnly DocumentDate,
    string Currency,
    IReadOnlyCollection<CreateDocumentLineCommand> Lines) : ICommand<DocumentDto>;

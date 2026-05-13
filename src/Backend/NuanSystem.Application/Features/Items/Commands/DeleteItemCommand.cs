using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.Items.Commands;

public sealed record DeleteItemCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

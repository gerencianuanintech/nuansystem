using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.SecurityUsers.Commands;

public sealed record DeleteUserCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;


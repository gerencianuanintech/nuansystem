using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.SecurityMenus.Commands;

public sealed record DeleteSecurityMenuCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

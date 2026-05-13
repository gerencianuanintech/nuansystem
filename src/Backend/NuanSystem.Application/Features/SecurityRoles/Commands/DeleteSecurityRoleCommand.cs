using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.SecurityRoles.Commands;

public sealed record DeleteSecurityRoleCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

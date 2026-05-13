using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.SecurityFields.Commands;

public sealed record DeleteSecurityFieldCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

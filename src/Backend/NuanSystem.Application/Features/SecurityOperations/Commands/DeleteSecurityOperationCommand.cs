using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.SecurityOperations.Commands;

public sealed record DeleteSecurityOperationCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

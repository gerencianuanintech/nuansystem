using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.SecurityForms.Commands;

public sealed record DeleteSecurityFormCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

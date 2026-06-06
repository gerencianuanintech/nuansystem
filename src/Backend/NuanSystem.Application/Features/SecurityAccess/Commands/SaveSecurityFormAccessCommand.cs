using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed record SaveSecurityFormAccessCommand(
    int RoleId,
    int FormId,
    IReadOnlyCollection<SaveSecurityFormAccessOperationData> Operations,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

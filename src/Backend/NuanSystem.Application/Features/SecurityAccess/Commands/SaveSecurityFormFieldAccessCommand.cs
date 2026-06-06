using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityAccess.Dtos;

namespace NuanSystem.Application.Features.SecurityAccess.Commands;

public sealed record SaveSecurityFormFieldAccessCommand(
    int RoleId,
    int FormId,
    IReadOnlyCollection<SaveSecurityFormFieldAccessData> Fields,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

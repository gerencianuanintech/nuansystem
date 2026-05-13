using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityForms.Dtos;

namespace NuanSystem.Application.Features.SecurityForms.Commands;

public sealed record CreateSecurityFormCommand(
    string Code,
    string Name,
    string? Description,
    string FormKey,
    int FormType,
    bool IsVisible,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<SecurityFormDto>;

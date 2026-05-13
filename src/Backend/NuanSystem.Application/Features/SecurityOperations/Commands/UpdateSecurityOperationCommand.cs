using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityOperations.Dtos;

namespace NuanSystem.Application.Features.SecurityOperations.Commands;

public sealed record UpdateSecurityOperationCommand(
    int Id,
    string Code,
    string Name,
    string? Description,
    string RibbonPageName,
    string RibbonGroupName,
    string ActionKey,
    string? IconLarge,
    string? IconSmall,
    int DisplayOrder,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<SecurityOperationDto>;

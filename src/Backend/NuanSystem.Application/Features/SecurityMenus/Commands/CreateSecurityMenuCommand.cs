using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.SecurityMenus.Dtos;

namespace NuanSystem.Application.Features.SecurityMenus.Commands;

public sealed record CreateSecurityMenuCommand(
    int? ParentId,
    string Code,
    string Name,
    string? Description,
    int MenuType,
    string? FormKey,
    string? IconLarge,
    string? IconSmall,
    int DisplayOrder,
    bool IsVisible,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<SecurityMenuDto>;

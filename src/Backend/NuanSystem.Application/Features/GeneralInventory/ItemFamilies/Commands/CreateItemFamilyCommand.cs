using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.ItemFamilies.Commands;

public sealed record CreateItemFamilyCommand(
    int ItemGroupId,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? SapFamilyCode,
    string? SapCode,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ItemFamilyDto>;

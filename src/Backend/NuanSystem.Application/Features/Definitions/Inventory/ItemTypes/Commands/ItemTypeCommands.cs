using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemTypes.Commands;

public sealed record CreateItemTypeCommand(
    string Code,
    string Name,
    string? Description,
    string BehaviorCode,
    bool DefaultIsPurchaseItem,
    bool DefaultIsSalesItem,
    bool DefaultIsInventoryItem,
    int SortOrder,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ItemTypeDto>;

public sealed record UpdateItemTypeCommand(
    int Id,
    string Code,
    string Name,
    string? Description,
    string BehaviorCode,
    bool DefaultIsPurchaseItem,
    bool DefaultIsSalesItem,
    bool DefaultIsInventoryItem,
    int SortOrder,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ItemTypeDto>;

public sealed record DeleteItemTypeCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

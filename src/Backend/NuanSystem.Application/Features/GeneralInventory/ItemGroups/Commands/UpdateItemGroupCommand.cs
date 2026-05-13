using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralInventory.ItemGroups.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.ItemGroups.Commands;

public sealed record UpdateItemGroupCommand(
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    string? InventoryAccountCode,
    string? CostOfSalesAccountCode,
    string? SalesAccountCode,
    string? PurchaseAccountCode,
    string? SapGroupCode,
    string? SapCode,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ItemGroupDto>;

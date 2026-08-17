using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemSubgroups.Commands;

public sealed record CreateItemSubgroupCommand(int ItemFamilyId, string Code, string Name, string? Description, int SortOrder, bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemSubgroupDto>;
public sealed record UpdateItemSubgroupCommand(int Id, int ItemFamilyId, string Code, string Name, string? Description, int SortOrder, bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemSubgroupDto>;
public sealed record DeleteItemSubgroupCommand(int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;


using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemAlertTypes.Commands;

public sealed record CreateItemAlertTypeCommand(string Code, string Name, string? Description, int SortOrder, bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemAlertTypeDto>;
public sealed record UpdateItemAlertTypeCommand(int Id, string Code, string Name, string? Description, int SortOrder, bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemAlertTypeDto>;
public sealed record DeleteItemAlertTypeCommand(int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;


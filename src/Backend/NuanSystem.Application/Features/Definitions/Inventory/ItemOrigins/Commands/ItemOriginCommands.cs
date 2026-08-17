using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemOrigins.Commands;

public sealed record CreateItemOriginCommand(string Code, string Name, string? Description, int SortOrder,
    bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemOriginDto>;
public sealed record UpdateItemOriginCommand(int Id, string Code, string Name, string? Description, int SortOrder,
    bool IsActive, int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemOriginDto>;
public sealed record DeleteItemOriginCommand(int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;

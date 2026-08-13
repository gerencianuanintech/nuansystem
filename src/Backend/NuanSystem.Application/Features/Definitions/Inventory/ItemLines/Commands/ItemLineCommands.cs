using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemLines.Commands;

public sealed record CreateItemLineCommand(
    string Code, string Name, string? Description, int SortOrder, bool IsActive,
    int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemLineDto>;

public sealed record UpdateItemLineCommand(
    int Id, string Code, string Name, string? Description, int SortOrder, bool IsActive,
    int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemLineDto>;

public sealed record DeleteItemLineCommand(
    int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;

using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ItemBrands.Commands;

public sealed record CreateItemBrandCommand(
    string Code, string Name, string? Description, int SortOrder, bool IsActive,
    string? ExternalSystem, string? ExternalCode, string? SapManufacturerCode, string? SapCode,
    int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemBrandDto>;

public sealed record UpdateItemBrandCommand(
    int Id, string Code, string Name, string? Description, int SortOrder, bool IsActive,
    string? ExternalSystem, string? ExternalCode, string? SapManufacturerCode, string? SapCode,
    int? AuditUserId = null, string? AuditUserName = null) : ICommand<ItemBrandDto>;

public sealed record DeleteItemBrandCommand(
    int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;

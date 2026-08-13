using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Dtos;

namespace NuanSystem.Application.Features.Definitions.Inventory.ProductTypes.Commands;

public sealed record CreateProductTypeCommand(
    string Code, string Name, string? Description, string NatureCode, int SortOrder, bool IsActive,
    int? AuditUserId = null, string? AuditUserName = null) : ICommand<ProductTypeDto>;

public sealed record UpdateProductTypeCommand(
    int Id, string Code, string Name, string? Description, string NatureCode, int SortOrder, bool IsActive,
    int? AuditUserId = null, string? AuditUserName = null) : ICommand<ProductTypeDto>;

public sealed record DeleteProductTypeCommand(
    int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;

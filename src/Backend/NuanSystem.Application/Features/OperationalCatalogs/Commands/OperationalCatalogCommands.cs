using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.OperationalCatalogs.Dtos;

namespace NuanSystem.Application.Features.OperationalCatalogs.Commands;

public sealed record CreateOperationalCatalogCommand(
    string CatalogKey,
    string Code,
    string Name,
    string? Description,
    string? ParentCatalogKey,
    string? ParentCode,
    int DisplayOrder,
    bool IsDefault,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<OperationalCatalogDto>;

public sealed record UpdateOperationalCatalogCommand(
    int Id,
    string CatalogKey,
    string Code,
    string Name,
    string? Description,
    string? ParentCatalogKey,
    string? ParentCode,
    int DisplayOrder,
    bool IsDefault,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<OperationalCatalogDto>;

public sealed record DeleteOperationalCatalogCommand(
    string CatalogKey,
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

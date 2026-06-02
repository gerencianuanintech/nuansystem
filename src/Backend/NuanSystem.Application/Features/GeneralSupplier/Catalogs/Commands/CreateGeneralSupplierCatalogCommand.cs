using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralSupplier.Catalogs.Dtos;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Commands;

public sealed record CreateGeneralSupplierCatalogCommand(
    string CatalogKey,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<GeneralSupplierCatalogDto>;


using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.GeneralInventory.Catalogs.Dtos;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Commands;

public sealed record CreateGeneralInventoryCatalogCommand(
    string CatalogKey,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<GeneralInventoryCatalogDto>;

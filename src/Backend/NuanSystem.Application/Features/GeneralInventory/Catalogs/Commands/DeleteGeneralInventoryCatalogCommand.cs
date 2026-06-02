using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.GeneralInventory.Catalogs.Commands;

public sealed record DeleteGeneralInventoryCatalogCommand(
    string CatalogKey,
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

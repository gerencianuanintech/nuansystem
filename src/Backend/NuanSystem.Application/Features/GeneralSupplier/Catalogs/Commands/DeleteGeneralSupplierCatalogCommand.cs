using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.GeneralSupplier.Catalogs.Commands;

public sealed record DeleteGeneralSupplierCatalogCommand(
    string CatalogKey,
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;


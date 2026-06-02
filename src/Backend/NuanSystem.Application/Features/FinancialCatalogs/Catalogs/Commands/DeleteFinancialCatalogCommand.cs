using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;

public sealed record DeleteFinancialCatalogCommand(
    string CatalogKey,
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

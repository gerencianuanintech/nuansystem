using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Dtos;

namespace NuanSystem.Application.Features.FinancialCatalogs.Catalogs.Commands;

public sealed record UpdateFinancialCatalogCommand(
    string CatalogKey,
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<FinancialCatalogDto>;

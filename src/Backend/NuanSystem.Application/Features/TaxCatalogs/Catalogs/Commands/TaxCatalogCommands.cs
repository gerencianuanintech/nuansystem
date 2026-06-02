using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.TaxCatalogs.Catalogs.Dtos;

namespace NuanSystem.Application.Features.TaxCatalogs.Catalogs.Commands;

public sealed record CreateTaxCatalogCommand(
    string CatalogKey,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<TaxCatalogDto>;

public sealed record UpdateTaxCatalogCommand(
    string CatalogKey,
    int Id,
    string Code,
    string Name,
    string? Description,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<TaxCatalogDto>;

public sealed record DeleteTaxCatalogCommand(
    string CatalogKey,
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

public sealed record CreateRetentionConceptCommand(
    string Code,
    string Name,
    string? Description,
    int? RetentionTypeId,
    string? SriCode,
    decimal Percent,
    bool AppliesIva,
    bool AppliesIncome,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<RetentionConceptDto>;

public sealed record UpdateRetentionConceptCommand(
    int Id,
    string Code,
    string Name,
    string? Description,
    int? RetentionTypeId,
    string? SriCode,
    decimal Percent,
    bool AppliesIva,
    bool AppliesIncome,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<RetentionConceptDto>;

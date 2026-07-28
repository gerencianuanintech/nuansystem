using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.TaxCatalogs.Taxes.Dtos;

namespace NuanSystem.Application.Features.TaxCatalogs.Taxes.Commands;

public sealed record CreateTaxCommand(
    string Code,
    string Name,
    string? Description,
    decimal Rate,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<TaxDto>;

public sealed record UpdateTaxCommand(
    int Id,
    string Code,
    string Name,
    string? Description,
    decimal Rate,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<TaxDto>;

public sealed record DeleteTaxCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

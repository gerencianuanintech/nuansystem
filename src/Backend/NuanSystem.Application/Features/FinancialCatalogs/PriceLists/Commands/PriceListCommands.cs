using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Dtos;

namespace NuanSystem.Application.Features.FinancialCatalogs.PriceLists.Commands;

public sealed record CreatePriceListCommand(
    string Code,
    string Name,
    string? Description,
    string CurrencyCode,
    string AppliesTo,
    bool IsDefault,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PriceListDto>;

public sealed record UpdatePriceListCommand(
    int Id,
    string Code,
    string Name,
    string? Description,
    string CurrencyCode,
    string AppliesTo,
    bool IsDefault,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<PriceListDto>;

public sealed record DeletePriceListCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

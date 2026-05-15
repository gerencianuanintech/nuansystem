using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.Accounting.ChartOfAccounts.Dtos;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Commands;

public sealed record UpdateChartOfAccountCommand(
    int Id,
    int CompanyId,
    string Code,
    string Name,
    string? Description,
    string? ExternalCode,
    string AccountType,
    string? AccountClass,
    int? ParentAccountId,
    bool IsTitle,
    bool AllowsMovement,
    bool IsActive,
    string? CurrencyCode,
    decimal Balance,
    bool IsConfidential,
    bool IsMonetaryAccount,
    bool IsAssociatedAccount,
    bool RevalueByIndex,
    bool BlockManualPosting,
    bool RelevantForCashFlow,
    bool RequiresCostCenter,
    bool RequiresThirdParty,
    bool RequiresProject,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ChartOfAccountDto>;

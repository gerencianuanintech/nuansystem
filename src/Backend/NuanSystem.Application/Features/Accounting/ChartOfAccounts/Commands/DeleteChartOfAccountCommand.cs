using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.Accounting.ChartOfAccounts.Commands;

public sealed record DeleteChartOfAccountCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

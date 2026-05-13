using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.ConfigurationCompanies.Commands;

public sealed record DeleteConfigurationCompanyCommand(
    int Id,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<bool>;

using NuanSystem.Application.Abstractions.Messaging;

namespace NuanSystem.Application.Features.ConfigurationSettings.Commands;

public sealed record DeleteConfigurationSettingCommand(int Id, int? AuditUserId = null, string? AuditUserName = null) : ICommand<bool>;

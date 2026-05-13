using NuanSystem.Application.Abstractions.Messaging;
using NuanSystem.Application.Features.ConfigurationSettings.Dtos;

namespace NuanSystem.Application.Features.ConfigurationSettings.Commands;

public sealed record CreateConfigurationSettingCommand(
    string Key,
    string? Value,
    string? Description,
    string DataType,
    string? Category,
    bool IsEncrypted,
    bool IsSystemParameter,
    bool IsEditable,
    int DisplayOrder,
    string? DefaultValue,
    string? ValidationExpression,
    bool IsActive,
    int? AuditUserId = null,
    string? AuditUserName = null) : ICommand<ConfigurationSettingDto>;

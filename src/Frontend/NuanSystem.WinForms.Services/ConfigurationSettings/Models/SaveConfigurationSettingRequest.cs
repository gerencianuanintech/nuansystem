namespace NuanSystem.WinForms.Services.ConfigurationSettings.Models;

public sealed record SaveConfigurationSettingRequest(
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
    bool IsActive);

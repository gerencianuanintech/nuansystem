namespace NuanSystem.Application.Features.ConfigurationSettings.Dtos;

public sealed record CreateConfigurationSettingData(
    int CompanyId,
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
    int? CreatedByUserId,
    string? CreatedByUserName);

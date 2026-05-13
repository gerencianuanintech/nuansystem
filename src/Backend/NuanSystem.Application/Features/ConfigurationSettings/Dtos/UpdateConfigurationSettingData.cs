namespace NuanSystem.Application.Features.ConfigurationSettings.Dtos;

public sealed record UpdateConfigurationSettingData(
    int CompanyId,
    int Id,
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
    int? UpdatedByUserId,
    string? UpdatedByUserName);

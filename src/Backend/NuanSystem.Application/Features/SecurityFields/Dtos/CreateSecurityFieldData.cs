namespace NuanSystem.Application.Features.SecurityFields.Dtos;

public sealed record CreateSecurityFieldData(
    int FormId,
    string Code,
    string Name,
    string FieldKey,
    string? Description,
    string ControlType,
    string DataType,
    bool IsRequired,
    string? ValidationMessage,
    bool IsReadOnly,
    bool IsVisible,
    bool IsCustom,
    int DisplayOrder,
    bool IsActive,
    int? CreatedByUserId,
    string? CreatedByUserName);

namespace NuanSystem.WinForms.Services.SecurityFields.Models;

public sealed record SaveSecurityFieldRequest(
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
    bool IsActive);
